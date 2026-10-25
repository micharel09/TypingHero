"""Pinned local Windows verification; isolated LF snapshot, existing Unity builders."""
import argparse
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


def run_editor(args, env):
    # Own the exact child, isolated from console Ctrl+C on Windows.
    flags = subprocess.CREATE_NEW_PROCESS_GROUP if os.name == "nt" else 0
    process = subprocess.Popen(args, env=env, creationflags=flags)
    try:
        return process.wait()
    except KeyboardInterrupt:
        previous = signal.signal(signal.SIGINT, signal.SIG_IGN)
        try:
            process.terminate()
            try:
                process.wait(timeout=30)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
        finally:
            signal.signal(signal.SIGINT, previous)
        raise


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
    source = a.workspace / 'source'
    label = a.action + ('-left' if a.mirror_fixture else '-right') if a.action == 'build' else a.action
    run_id = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S.%fZ') + '-' + uuid4().hex
    results = a.workspace / 'results' / (run_id + '-' + label)
    results.mkdir(parents=True, exist_ok=False)
    print('Evidence:', results.resolve(), flush=True)
    if a.action=='snapshot':
        if source.exists(): raise SystemExit('Existing snapshot preserved; use a new workspace')
        names=subprocess.check_output(['git','ls-files','-z'],cwd=ROOT).decode().split('\0')
        rows=[]; digest=hashlib.sha256()
        for name in sorted(filter(None,names)):
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
        code = run_editor(args, env)
        if code:
            return code
        validate_tests(xml)
        return 0
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
            with fixture.open('xb') as stream:
                created = True
                stream.write(data)
        code = run_editor(args, env)
    finally:
        errors = []
        for original, backup in backups:
            try:
                original.write_bytes(backup.read_bytes())
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
    if code:
        return code
    if not (staging / 'index.html').is_file():
        raise RuntimeError('Missing WebGL player')
    # Path.rename refuses an existing destination on Windows; never overwrite a player.
    if output.exists():
        raise RuntimeError('Existing build preserved')
    staging.rename(output)
    print('Player:', output.resolve(), flush=True)
    return 0


if __name__ == '__main__':
    try:
        raise SystemExit(main())
    except (RuntimeError, OSError) as exc:
        raise SystemExit(str(exc))
