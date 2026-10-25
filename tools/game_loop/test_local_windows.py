"""Windows adapter contracts, with isolated files and an owned fake Editor."""
import importlib.util
import os
import contextlib
import io
import json
import signal
import sys
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


@unittest.skipUnless(os.name == 'nt', 'Windows adapter contracts')
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
            with contextlib.redirect_stdout(io.StringIO()):
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
                def fake(args, env, cancel):
                    Path(args[args.index('-testResults') + 1]).write_text(content)
                    return 0
                if valid:
                    self.assertEqual(self.run_action('tests', fake), 0)
                else:
                    with self.assertRaises(RuntimeError):
                        self.run_action('tests', fake)

    def test_cancel_during_xml_validation_is_not_reported_as_success(self):
        validate = adapter.validate_tests
        def validation(path):
            validate(path)
            signal.raise_signal(signal.SIGINT)
        def editor(args, env, cancel):
            Path(args[args.index('-testResults') + 1]).write_text(xml())
            return 0
        with patch.object(adapter, 'validate_tests', validation):
            self.assertEqual(self.run_action('tests', editor), 130)
        with adapter.workspace_lock(self.root):
            pass

    def build_fake(self, code=0):
        def fake(args, env, cancel):
            self.assertTrue(self.fixture.exists())
            self.scene.write_bytes(b'mirrored')
            self.meta.write_bytes(b'changed')
            self.fixture.with_suffix('.cs.meta').write_bytes(b'generated')
            output = Path(env['TYPINGHERO_BUILD_PATH'])
            output.mkdir()
            (output / 'index.html').write_text('player')
            return code
        return fake

    def test_normal_build_publishes_right_player_and_releases_lock(self):
        def editor(args, env, cancel):
            self.assertFalse(self.fixture.exists())
            output = Path(env['TYPINGHERO_BUILD_PATH'])
            output.mkdir()
            (output / 'index.html').write_text('normal player')
            return 0
        self.assertEqual(self.run_action('build', editor), 0)
        self.assertEqual((self.root / 'player-right/index.html').read_text(), 'normal player')
        self.assert_restored()
        with adapter.workspace_lock(self.root):
            pass

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
        replace = os.replace
        def fail(source, destination):
            if destination == self.scene:
                raise OSError('restore denied')
            return replace(source, destination)
        with patch.object(adapter.os, 'replace', side_effect=fail):
            with self.assertRaisesRegex(RuntimeError, 'restoration failed'):
                self.run_action('build', self.build_fake(), True)
        self.assertFalse((self.root / 'player-left').exists())
        backup = next((self.root / 'results').glob('*/SampleScene.unity.backup'))
        self.assertEqual(backup.read_bytes(), self.original[0])
        self.assertEqual(self.meta.read_bytes(), self.original[1])
        self.assertFalse(self.fixture.exists())
        self.assertFalse(self.fixture.with_suffix('.cs.meta').exists())
        self.assertFalse(list(self.scene.parent.glob('*.restore-tmp')))
        with adapter.workspace_lock(self.root):
            pass

    def test_ctrl_c_waits_for_owned_editor(self):
        process = Mock()
        process.poll.side_effect = [None, None, None, 0]
        process.wait.side_effect = [subprocess.TimeoutExpired('editor', 30), 0]
        with adapter.Cancellation() as cancel:
            cancel.requested = True
            # No child at all if cancellation was already pending before launch.
            with patch.object(adapter.subprocess, 'Popen') as launch:
                self.assertEqual(adapter.run_editor(['editor'], {}, cancel), 130)
                launch.assert_not_called()
            process.terminate.side_effect = lambda: signal.raise_signal(signal.SIGINT)
            adapter.stop_editor(process)
        process.terminate.assert_called_once()
        process.kill.assert_called_once()
        self.assertEqual(process.wait.call_count, 2)

    def test_termination_errors_still_wait_for_owned_editor(self):
        process = Mock()
        process.poll.side_effect = [None, None, None, None, 0]
        process.terminate.side_effect = OSError('terminate denied')
        process.kill.side_effect = [OSError('kill initially denied'), None]
        process.wait.side_effect = [subprocess.TimeoutExpired('editor', 30), 0]
        with contextlib.redirect_stdout(io.StringIO()):
            adapter.stop_editor(process)
        self.assertEqual(process.wait.call_count, 2)
        self.assertEqual(process.kill.call_count, 2)

    def test_failed_fixture_creation_never_restores_other_owner(self):
        real_open = Path.open
        def open_file(path, *args, **kwargs):
            if path == self.fixture and args == ('xb',):
                self.scene.write_bytes(b'other active editor')
                raise FileExistsError('fixture created by another owner')
            return real_open(path, *args, **kwargs)
        with patch.object(Path, 'open', open_file), patch.object(adapter, 'restore_file') as restore:
            with self.assertRaises(FileExistsError):
                self.run_action('build', self.build_fake(), True)
            restore.assert_not_called()
        self.assertEqual(self.scene.read_bytes(), b'other active editor')

    def test_cancel_during_publication_keeps_staged_output(self):
        rename = Path.rename
        def renamed(path, target):
            result = rename(path, target)
            if target == self.root / 'player-left':
                signal.raise_signal(signal.SIGINT)
            return result
        with patch.object(Path, 'rename', renamed):
            self.assertEqual(self.run_action('build', self.build_fake(), True), 130)
        self.assert_restored()
        self.assertFalse((self.root / 'player-left').exists())
        self.assertEqual(len(list((self.root / 'results').glob('*/player/index.html'))), 1)

    def test_retained_restore_temp_is_reported(self):
        original = self.root / 'restore-target'
        backup = self.root / 'backup'
        original.write_bytes(b'mirrored')
        backup.write_bytes(b'original')
        real_unlink = Path.unlink
        def unlink(path, *args, **kwargs):
            if path.name.endswith('.restore-tmp'):
                raise OSError('temp delete denied')
            return real_unlink(path, *args, **kwargs)
        with patch.object(adapter.os, 'replace', side_effect=OSError('replace denied')), \
                patch.object(Path, 'unlink', unlink):
            with self.assertRaisesRegex(OSError, 'temporary file retained at'):
                adapter.restore_file(original, backup)
        self.assertEqual(original.read_bytes(), b'mirrored')
        self.assertEqual(len(list(self.root.glob('*.restore-tmp'))), 1)

    def test_atomic_restore_io_failures(self):
        for fault in ('write', 'fsync', 'replace', 'verify'):
            with self.subTest(fault=fault):
                original = self.root / 'restore-target'
                backup = self.root / 'backup'
                original.write_bytes(b'mirrored-but-intact')
                backup.write_bytes(b'original')
                real_open, real_read = Path.open, Path.read_bytes
                def opened(path, *args, **kwargs):
                    if fault == 'write' and path.name.endswith('.restore-tmp'):
                        raise OSError('temp write denied')
                    return real_open(path, *args, **kwargs)
                def read(path):
                    if fault == 'verify' and path == original:
                        return b'wrong readback'
                    return real_read(path)
                with patch.object(Path, 'open', opened), patch.object(Path, 'read_bytes', read):
                    if fault in ('replace', 'fsync'):
                        with patch.object(adapter.os, fault, side_effect=OSError(fault + ' denied')):
                            with self.assertRaises(OSError):
                                adapter.restore_file(original, backup)
                    else:
                        with self.assertRaises(OSError):
                            adapter.restore_file(original, backup)
                expected = b'original' if fault == 'verify' else b'mirrored-but-intact'
                self.assertEqual(original.read_bytes(), expected)
                self.assertEqual(backup.read_bytes(), b'original')
                self.assertFalse(list(self.root.glob('*.restore-tmp')))

    def test_signal_handler_and_lock_released(self):
        previous = signal.getsignal(signal.SIGINT)
        for fake in (self.build_fake(1), OSError('launch failed'), self.build_fake()):
            with self.subTest(fake=fake):
                if isinstance(fake, OSError):
                    with self.assertRaises(OSError):
                        self.run_action('build', Mock(side_effect=fake), True)
                else:
                    self.run_action('build', fake, True)
                self.assertIs(signal.getsignal(signal.SIGINT), previous)
                with adapter.workspace_lock(self.root):
                    pass

    def test_cancellation_before_launch(self):
        real_read = Path.read_bytes
        def read(path):
            data = real_read(path)
            if path == self.input:
                signal.raise_signal(signal.SIGINT)
            return data
        with patch.object(Path, 'read_bytes', read):
            fake = Mock(side_effect=AssertionError('unexpected launch'))
            self.assertEqual(self.run_action('build', fake, True), 130)
            fake.assert_not_called()
        self.assert_restored()
        with adapter.workspace_lock(self.root):
            pass

    def worker(self, mode):
        process = subprocess.Popen([sys.executable, '-B', '-u', __file__, mode, str(self.root)],
                                   stdin=subprocess.PIPE, stdout=subprocess.PIPE,
                                   stderr=subprocess.PIPE, text=True)
        def cleanup():
            if process.poll() is None:
                process.kill()
                process.wait()
            for stream in (process.stdin, process.stdout, process.stderr):
                stream.close()
        self.addCleanup(cleanup)
        return process

    def ready(self, process, marker):
        for line in iter(process.stdout.readline, ''):
            if line.strip() == marker:
                return
        self.fail('Worker exited before ready: ' + process.stderr.read())

    def test_cross_process_lock_excludes_all_actions(self):
        holder = self.worker('--hold-build')
        self.ready(holder, 'ACTIVE')
        self.assertEqual(self.scene.read_bytes(), b'mirrored')
        for action in ('snapshot', 'tests', 'build'):
            command = [sys.executable, '-B', str(Path(adapter.__file__)), action,
                       '--workspace', str(self.root / '.'), '--mirror-fixture', str(self.input)]
            loser = subprocess.run(command, capture_output=True, text=True, timeout=10)
            self.assertNotEqual(loser.returncode, 0)
            self.assertIn('Workspace busy', loser.stderr)
            self.assertEqual(self.scene.read_bytes(), b'mirrored')
            self.assertTrue(self.fixture.exists())
        self.assertEqual(len(list((self.root / 'results').glob('*/SampleScene.unity.backup'))), 1)
        holder.stdin.write('finish\n'); holder.stdin.flush()
        _, error = holder.communicate(timeout=10)
        self.assertEqual(holder.returncode, 1, error)
        self.assert_restored()
        with adapter.workspace_lock(self.root):
            pass

    def test_os_releases_crashed_owner_lock_and_separate_workspaces(self):
        holder = self.worker('--hold-lock')
        self.ready(holder, 'LOCKED')
        with self.assertRaisesRegex(RuntimeError, 'Workspace busy'):
            with adapter.workspace_lock(self.root):
                pass
        with adapter.workspace_lock(self.root / 'independent'):
            pass
        holder.kill(); holder.wait(timeout=10)
        self.assertTrue((self.root / '.local-windows.lock').exists())
        with adapter.workspace_lock(self.root):
            pass

    def test_cancellation_subprocess_boundaries(self):
        for phase in ('wait', 'temp-write', 'between-restores', 'cleanup'):
            with self.subTest(phase=phase):
                process = self.worker('--cancel-' + phase)
                output, error = process.communicate(timeout=15)
                self.assertEqual(process.returncode, 0, error + output)
                report = json.loads(output.splitlines()[-1])
                self.assertEqual(report['code'], 130)
                self.assertTrue(report['child_exited_before_restore'])
                self.assertTrue(report['handler_restored'])
                self.assert_restored()
                self.assertFalse((self.root / 'player-left').exists())
                self.assertFalse(list(self.scene.parent.glob('*.restore-tmp')))
                with adapter.workspace_lock(self.root):
                    pass


def worker_main(mode, root):
    scene = root / 'source/Assets/Scenes/SampleScene.unity'
    fixture = root / 'source/Assets/Editor/PixelSparkMirrorQA.cs'
    if mode == '--hold-lock':
        with adapter.workspace_lock(root):
            print('LOCKED', flush=True)
            sys.stdin.readline()
        return 0
    if mode == '--fake-editor':
        scene.write_bytes(b'mirrored')
        fixture.with_suffix('.cs.meta').write_bytes(b'generated')
        print('READY', flush=True)
        sys.stdin.readline()
        return 0
    args = ['build', '--workspace', str(root), '--mirror-fixture', str(root / 'mirror.cs')]
    if mode == '--hold-build':
        def editor(*unused):
            scene.write_bytes(b'mirrored')
            fixture.with_suffix('.cs.meta').write_bytes(b'generated')
            print('ACTIVE', flush=True)
            sys.stdin.readline()
            return 1
        with patch.object(adapter, 'run_editor', editor):
            return adapter.main(args)
    phase = mode.removeprefix('--cancel-')
    real_popen, real_fsync = subprocess.Popen, os.fsync
    previous = signal.getsignal(signal.SIGINT)
    children = []
    order = []
    def interrupt():
        signal.raise_signal(signal.SIGINT)
        signal.raise_signal(signal.SIGINT)
    def launch(*unused, **kwargs):
        process = real_popen([sys.executable, '-B', '-u', __file__, '--fake-editor', str(root)],
                             stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                             text=True, creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)
        children.append(process)
        assert process.stdout.readline().strip() == 'READY'
        real_wait, real_terminate = process.wait, process.terminate
        def wait(timeout=None):
            if phase == 'wait' and not order:
                order.append('cancelled')
                interrupt()
            return real_wait(timeout=timeout)
        def terminate():
            interrupt()
            return real_terminate()
        if phase == 'wait':
            process.wait = wait
            process.terminate = terminate
        else:
            # Successful fake build, so cancellation in cleanup must block promotion.
            process.stdin.write('finish\n'); process.stdin.flush()
            output = Path(kwargs['env']['TYPINGHERO_BUILD_PATH'])
            output.mkdir(); (output / 'index.html').write_text('player')
        return process
    restored = []
    real_restore = adapter.restore_file
    def restore(original, backup):
        assert children[0].poll() is not None, 'restored while child still alive'
        restored.append(original.name)
        result = real_restore(original, backup)
        if phase == 'between-restores' and original == scene:
            interrupt()
        return result
    def fsync(fd):
        if phase == 'temp-write':
            assert scene.stat().st_size > 0
            interrupt()
        return real_fsync(fd)
    real_unlink = Path.unlink
    def unlink(path, *args, **kwargs):
        if phase == 'cleanup' and path == fixture:
            interrupt()
        return real_unlink(path, *args, **kwargs)
    try:
        with patch.object(adapter.subprocess, 'Popen', launch), patch.object(adapter, 'restore_file', restore), \
                patch.object(adapter.os, 'fsync', fsync), patch.object(Path, 'unlink', unlink):
            code = adapter.main(args)
        assert len(restored) == 2
        assert signal.getsignal(signal.SIGINT) is previous
        with adapter.workspace_lock(root):
            pass
        print(json.dumps({'code': code, 'child_exited_before_restore': True, 'handler_restored': True}))
        return 0
    finally:
        for process in children:
            if process.poll() is None:
                process.kill(); process.wait()
            for stream in (process.stdin, process.stdout, process.stderr):
                stream.close()


if __name__ == '__main__':
    if len(sys.argv) > 1 and sys.argv[1].startswith('--'):
        raise SystemExit(worker_main(sys.argv[1], Path(sys.argv[2])))
    unittest.main()
