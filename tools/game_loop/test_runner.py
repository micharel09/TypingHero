"""Runner contract tests; these do not replace Unity or browser acceptance."""
import importlib.util
from pathlib import Path
import tempfile
import unittest
from unittest.mock import Mock, patch
import subprocess
from types import SimpleNamespace

spec = importlib.util.spec_from_file_location("game_loop_server", Path(__file__).with_name("server.py"))
# server uses Linux-only fcntl; run this module on fly-server.
server = importlib.util.module_from_spec(spec)
spec.loader.exec_module(server)


class ResultContractTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(dir="/data/typinghero-game-loop")
        self.xml = Path(self.temp.name) / "tests.xml"

    def tearDown(self):
        self.temp.cleanup()

    def test_missing_result_fails(self):
        with self.assertRaisesRegex(RuntimeError, "did not produce"):
            server.verify_xml(self.xml)

    def test_zero_test_result_fails(self):
        self.xml.write_text('<test-run result="Passed" total="0"/>')
        with self.assertRaisesRegex(RuntimeError, "zero tests"):
            server.verify_xml(self.xml)

    def test_failed_or_skipped_case_fails(self):
        for result in ["Failed", "Skipped", "Inconclusive"]:
            with self.subTest(result=result):
                self.xml.write_text(f'<test-run result="Passed"><test-case result="Passed"/><test-case result="{result}"/></test-run>')
                with self.assertRaises(RuntimeError): server.verify_xml(self.xml)

    def test_executed_passing_cases_accepted(self):
        self.xml.write_text('<test-run result="Passed"><test-suite><test-case result="Passed"/></test-suite></test-run>')
        server.verify_xml(self.xml)

    def test_failed_suite_with_passing_cases_fails(self):
        self.xml.write_text('<test-run result="Failed"><test-case result="Passed"/></test-run>')
        with self.assertRaises(RuntimeError): server.verify_xml(self.xml)

    def test_malformed_result_fails(self):
        self.xml.write_text('<test-run>')
        with self.assertRaises(Exception): server.verify_xml(self.xml)

    def test_resource_preflight_rejects_insufficient_disk(self):
        with patch.object(server.shutil, 'disk_usage', return_value=SimpleNamespace(free=0)):
            with self.assertRaisesRegex(RuntimeError, 'short 3.0 GiB'):
                server.space(Path(self.temp.name), 3)

    def test_timeout_stops_only_the_task_container(self):
        process = Mock()
        process.wait.side_effect = [subprocess.TimeoutExpired('docker', 5), 0]
        process.poll.return_value = None
        with patch.object(server.subprocess, 'Popen', return_value=process) as launch, \
             patch.object(server.subprocess, 'run') as cleanup, \
             patch.object(server, 'space'), \
             patch.object(server.time, 'monotonic', side_effect=[0, 2]):
            with self.assertRaises(TimeoutError): server.container(['test-image'], timeout=1)
        name = launch.call_args.args[0][3]
        self.assertTrue(name.startswith('typinghero-loop-'))
        self.assertTrue(cleanup.called)
        for call in cleanup.call_args_list:
            self.assertEqual(call.args[0], ['docker', 'rm', '-f', name])


if __name__ == "__main__": unittest.main()
