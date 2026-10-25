"""Pinned local Windows verification; isolated LF snapshot, existing Unity builders."""
import argparse
from contextlib import contextmanager
import hashlib
import json
import os
import signal
import subprocess
from datetime import datetime, timezone
from pathlib import Path
from uuid import uuid4
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]


class Cancellation:
    """Defer SIGINT until the Editor has exited and source cleanup is complete."""
    def __init__(self):
        self.requested = False

    def request(self, signum, frame):
        self.requested = True

    def __enter__(self):
        self.previous = signal.signal(signal.SIGINT, self.request)
        return self

    def __exit__(self, *exc):
        signal.signal(signal.SIGINT, self.previous)


@contextmanager
def workspace_lock(workspace):
    import msvcrt
    workspace.mkdir(parents=True, exist_ok=True)
    # Keep the file: unlinking would allow two callers to lock different files.
    with (workspace / '.local-windows.lock').open('a+b') as stream:
        if os.fstat(stream.fileno()).st_size == 0:
            stream.write(b'\0')
            stream.flush()
        stream.seek(0)
        try:
            msvcrt.locking(stream.fileno(), msvcrt.LK_NBLCK, 1)
        except OSError as exc:
            raise RuntimeError(f'Workspace busy or lock unavailable: {workspace}: {exc}') from exc
        try:
            yield
        finally:
            stream.seek(0)
            msvcrt.locking(stream.fileno(), msvcrt.LK_UNLCK, 1)


def stop_editor(process):
    if process.poll() is not None:
        return
    try:
        process.terminate()
    except OSError as exc:
        if process.poll() is None:
            print(f'Editor terminate failed: {exc}; waiting for owned process', flush=True)
            try:
                process.kill()
            except OSError as error:
                print(f'Editor kill failed: {error}; source remains locked', flush=True)
    while process.poll() is None:
        try:
            process.wait(timeout=30)
        except subprocess.TimeoutExpired:
            try:
                process.kill()
            except OSError as exc:
                print(f'Editor kill failed: {exc}; source remains locked', flush=True)


def run_editor(args, env, cancel):
    if cancel.requested:
        return 130
    flags = subprocess.CREATE_NEW_PROCESS_GROUP if os.name == 'nt' else 0
    process = subprocess.Popen(args, env=env, creationflags=flags)
    try:
        while not cancel.requested:
            try:
                return process.wait(timeout=0.2)
            except subprocess.TimeoutExpired:
                pass
        return 130
    finally:
        # Also reap the owned Editor after unexpected wait errors, before restore.
        stop_editor(process)


def restore_file(original, backup):
    data = backup.read_bytes()
    temporary = original.with_name(original.name + '.' + uuid4().hex + '.restore-tmp')
    try:
        with temporary.open('xb') as stream:
            stream.write(data)
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, original)
        if original.read_bytes() != data:
            raise OSError(f'Restored bytes differ from backup: {original}')
    finally:
        try:
            temporary.unlink(missing_ok=True)
        except OSError as exc:
            raise OSError(f'Restore temporary file retained at {temporary}: {exc}') from exc


def validate_tests(path):
    try:
        root = ET.parse(path).getroot()
    except (OSError, ET.ParseError) as exc:
        raise RuntimeError(f"Missing or invalid test XML: {path}") from exc
    cases = root.findall(".//test-case")
    failed = [case.attrib for case in cases if case.get("result") != "Passed"]
    print(json.dumps({"count": len(cases), "nonpassing": failed}))
    if root.get("result") != "Passed" or len(cases) != 44 or failed:
        raise RuntimeError("Expected root Passed and all 44 ImpactVfxTests passed")


def main(argv=None):
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('action', choices=['snapshot', 'tests', 'build'])
    p.add_argument('--workspace', type=Path, required=True)
    p.add_argument('--mirror-fixture', type=Path)
    p.add_argument('--editor', type=Path, default=Path('E:/Unity/Hub/Editor/2022.3.25f1/Editor/Unity.exe'))
    a = p.parse_args(argv)
    a.workspace = a.workspace.resolve()
    label = a.action + ('-left' if a.mirror_fixture else '-right') if a.action == 'build' else a.action
    run_id = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S.%fZ') + '-' + uuid4().hex
    results = a.workspace / 'results' / (run_id + '-' + label)
    results.mkdir(parents=True, exist_ok=False)
    print('Evidence:', results.resolve(), flush=True)
    with Cancellation() as cancel:
        with workspace_lock(a.workspace):
            if cancel.requested:
                return 130
            return execute(a, results, cancel, label)


def execute(a, results, cancel, label):
    source = a.workspace / 'source'
    if a.action=='snapshot':
        if source.exists(): raise SystemExit('Existing snapshot preserved; use a new workspace')
        names=subprocess.check_output(['git','ls-files','-z'],cwd=ROOT).decode().split('\0')
        rows=[]; digest=hashlib.sha256()
        for name in sorted(filter(None,names)):
            if cancel.requested: return 130
            if not name.startswith(('Assets/','Packages/','ProjectSettings/','tools/game_loop/')): continue
            path=ROOT/name
            if not path.is_file(): continue
            data=path.read_bytes()
            if b'\0' not in data:
                try: data=data.decode('utf-8').replace('\r\n','\n').encode('utf-8')
                except UnicodeDecodeError: pass
            dest=source/name; dest.parent.mkdir(parents=True,exist_ok=True); dest.write_bytes(data)
            digest.update(name.encode()+b'\0'+data)
            rows.append({'path':name,'sha256':hashlib.sha256(data).hexdigest()})
        if cancel.requested: return 130
        (a.workspace/'snapshot.json').write_text(json.dumps({'sha256':digest.hexdigest(),'files':rows},indent=2))
        print('Snapshot:',len(rows),'files',digest.hexdigest()); return 0
    if not (a.workspace / 'snapshot.json').is_file():
        raise RuntimeError('Snapshot required')
    fixture = source / 'Assets/Editor/PixelSparkMirrorQA.cs'
    fixture_meta = fixture.with_suffix('.cs.meta')
    if fixture.exists() or fixture_meta.exists():
        raise RuntimeError('Existing mirror fixture or meta preserved')
    output = a.workspace / ('player-left' if a.mirror_fixture else 'player-right')
    if a.action == 'build' and output.exists():
        raise RuntimeError('Existing build preserved')
    env = {**os.environ, 'TYPINGHERO_RESULTS_PATH': str(results.resolve())}
    args = [str(a.editor), '-batchmode', '-nographics', '-projectPath', str(source.resolve()),
            '-logFile', str((results / (label + '.log')).resolve())]
    if a.action == 'tests':
        xml = results / 'tests.xml'
        args += ['-runTests', '-testPlatform', 'PlayMode', '-testCategory', 'Quick',
                 '-testFilter', 'ImpactVfxTests', '-testResults', str(xml.resolve())]
        code = run_editor(args, env, cancel)
        if cancel.requested:
            return 130
        if code:
            return code
        validate_tests(xml)
        return 130 if cancel.requested else 0
    staging = results / 'player'
    env.update(TYPINGHERO_QA='1', TYPINGHERO_BUILD_PATH=str(staging.resolve()))
    args += ['-quit', '-buildTarget', 'WebGL', '-executeMethod', 'GameLoopBuild.WebGL']
    backups = []
    created = False
    try:
        if a.mirror_fixture:
            scene = source / 'Assets/Scenes/SampleScene.unity'
            for original in (scene, scene.with_suffix('.unity.meta')):
                backup = results / (original.name + '.backup')
                backup.write_bytes(original.read_bytes())
                backups.append((original, backup))
            data = a.mirror_fixture.read_bytes().replace(b'\r\n', b'\n')
            if cancel.requested:
                return 130
            with fixture.open('xb') as stream:
                created = True
                stream.write(data)
        code = run_editor(args, env, cancel)
    finally:
        errors = []
        for original, backup in backups if created else []:
            try:
                restore_file(original, backup)
            except OSError as exc:
                errors.append(f'{original}: {exc}')
        if created:
            for path in (fixture, fixture_meta):
                try:
                    path.unlink(missing_ok=True)
                except OSError as exc:
                    errors.append(f'{path}: {exc}')
        if errors:
            raise RuntimeError('Source restoration failed; backups retained: ' + '; '.join(errors))
    if cancel.requested:
        return 130
    if code:
        return code
    if not (staging / 'index.html').is_file():
        raise RuntimeError('Missing WebGL player')
    # Path.rename refuses an existing destination on Windows; never overwrite a player.
    if output.exists():
        raise RuntimeError('Existing build preserved')
    if cancel.requested:
        return 130
    staging.rename(output)
    if cancel.requested:
        output.rename(staging)
        return 130
    # Publication is now committed; later SIGINT cannot undo a completed run.
    print('Player:', output.resolve(), flush=True)
    return 0


if __name__ == '__main__':
    try:
        raise SystemExit(main())
    except (RuntimeError, OSError) as exc:
        raise SystemExit(str(exc))
