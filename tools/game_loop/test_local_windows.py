"""Windows adapter contracts, with isolated files and an owned fake Editor."""
import importlib.util
import os
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch, Mock

spec = importlib.util.spec_from_file_location('adapter', Path(__file__).with_name('local_windows.py'))
adapter = importlib.util.module_from_spec(spec)
spec.loader.exec_module(adapter)


def xml(count=44, result='Passed', case='Passed'):
    return '<test-run result="%s">%s</test-run>' % (result, ''.join(
        '<test-case result="%s" />' % case for _ in range(count)))


class AdapterTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.source = self.root / 'source'
        self.scene = self.source / 'Assets/Scenes/SampleScene.unity'
        self.scene.parent.mkdir(parents=True)
        self.scene.write_bytes(b'scene\r\n\x00original')
        self.meta = self.scene.with_suffix('.unity.meta')
        self.meta.write_bytes(b'meta\r\noriginal')
        self.fixture = self.source / 'Assets/Editor/PixelSparkMirrorQA.cs'
        self.fixture.parent.mkdir(parents=True)
        self.input = self.root / 'mirror.cs'
        self.input.write_bytes(b'fixture')
        (self.root / 'snapshot.json').write_text('{}')
        (self.source / 'Library').mkdir()
        self.original = (self.scene.read_bytes(), self.meta.read_bytes())

    def run_action(self, action, fake, mirror=False):
        args = [action, '--workspace', str(self.root)]
        if mirror:
            args += ['--mirror-fixture', str(self.input)]
        with patch.object(adapter, 'run_editor', side_effect=fake):
            return adapter.main(args)

    def assert_restored(self):
        self.assertEqual(self.original, (self.scene.read_bytes(), self.meta.read_bytes()))
        self.assertFalse(self.fixture.exists())
        self.assertFalse(self.fixture.with_suffix('.cs.meta').exists())

    def test_stale_xml_is_not_accepted(self):
        (self.root / 'results').mkdir()
        old = self.root / 'results/tests.xml'
        old.write_text(xml())
        with self.assertRaisesRegex(RuntimeError, 'Missing or invalid'):
            self.run_action('tests', lambda *args: 0)
        self.assertEqual(old.read_text(), xml())

    def test_xml_contract(self):
        for content, valid in [(xml(), True), ('broken', False),
                               (xml(result='Failed'), False), (xml(43), False), (xml(45), False),
                               (xml(case='Skipped'), False), (xml(case='Failed'), False),
                               (xml(case='Inconclusive'), False)]:
            with self.subTest(content=content[:70]):
                def fake(args, env):
                    Path(args[args.index('-testResults') + 1]).write_text(content)
                    return 0
                if valid:
                    self.assertEqual(self.run_action('tests', fake), 0)
                else:
                    with self.assertRaises(RuntimeError):
                        self.run_action('tests', fake)

    def build_fake(self, code=0):
        def fake(args, env):
            self.assertTrue(self.fixture.exists())
            self.scene.write_bytes(b'mirrored')
            self.meta.write_bytes(b'changed')
            self.fixture.with_suffix('.cs.meta').write_bytes(b'generated')
            output = Path(env['TYPINGHERO_BUILD_PATH'])
            output.mkdir()
            (output / 'index.html').write_text('player')
            return code
        return fake

    def test_failed_build_retry_same_library(self):
        self.assertEqual(self.run_action('build', self.build_fake(1), True), 1)
        self.assert_restored()
        self.assertFalse((self.root / 'player-left').exists())
        self.assertEqual(len(list((self.root / 'results').glob('*/player/index.html'))), 1)
        self.assertEqual(self.run_action('build', self.build_fake(), True), 0)
        self.assert_restored()
        self.assertTrue((self.source / 'Library').is_dir())
        self.assertTrue((self.root / 'player-left/index.html').is_file())
        with self.assertRaisesRegex(RuntimeError, 'Existing build'):
            self.run_action('build', Mock(side_effect=AssertionError('launched')), True)
        self.assertEqual((self.root / 'player-left/index.html').read_text(), 'player')

    def test_launch_failure_and_interrupt_restore(self):
        for failure in (OSError('launch failed'), KeyboardInterrupt()):
            with self.subTest(failure=failure):
                def fake(*args):
                    self.scene.write_bytes(b'mirrored')
                    raise failure
                with self.assertRaises(type(failure)):
                    self.run_action('build', fake, True)
                self.assert_restored()

    def test_preflight_preserves_existing_files(self):
        for path in (self.root / 'player-left', self.fixture, self.fixture.with_suffix('.cs.meta')):
            with self.subTest(path=path):
                path.write_bytes(b'existing')
                with self.assertRaises(RuntimeError):
                    self.run_action('build', Mock(side_effect=AssertionError('launched')), True)
                self.assertEqual(path.read_bytes(), b'existing')
                self.assertEqual(self.original, (self.scene.read_bytes(), self.meta.read_bytes()))
                if path != self.fixture:
                    self.assertFalse(self.fixture.exists())
                path.unlink()

    def test_restore_failure_retains_backup_and_does_not_publish(self):
        write_bytes = Path.write_bytes
        def write(path, data):
            if path == self.scene and data == self.original[0]:
                raise OSError('restore denied')
            return write_bytes(path, data)
        with patch.object(Path, 'write_bytes', write):
            with self.assertRaisesRegex(RuntimeError, 'restoration failed'):
                self.run_action('build', self.build_fake(), True)
        self.assertFalse((self.root / 'player-left').exists())
        backup = next((self.root / 'results').glob('*/SampleScene.unity.backup'))
        self.assertEqual(backup.read_bytes(), self.original[0])

    def test_ctrl_c_waits_for_owned_editor(self):
        process = Mock()
        process.wait.side_effect = [KeyboardInterrupt(), subprocess.TimeoutExpired('editor', 30), 0]
        with patch.object(adapter.subprocess, 'Popen', return_value=process):
            with self.assertRaises(KeyboardInterrupt):
                adapter.run_editor(['editor'], os.environ.copy())
        process.terminate.assert_called_once()
        process.kill.assert_called_once()
        self.assertEqual(process.wait.call_count, 3)


if __name__ == '__main__':
    unittest.main()
