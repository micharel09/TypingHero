"""Pinned local Windows verification; isolated LF snapshot, existing Unity builders."""
import argparse, hashlib, json, os, subprocess
from pathlib import Path
import xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[2]
p=argparse.ArgumentParser(description=__doc__)
p.add_argument('action',choices=['snapshot','tests','build'])
p.add_argument('--workspace',type=Path,required=True)
p.add_argument('--mirror-fixture',type=Path)
p.add_argument('--editor',type=Path,default=Path('E:/Unity/Hub/Editor/2022.3.25f1/Editor/Unity.exe'))
a=p.parse_args(); source=a.workspace/'source'; results=a.workspace/'results'; results.mkdir(parents=True,exist_ok=True)
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
    print('Snapshot:',len(rows),'files',digest.hexdigest()); raise SystemExit(0)
if not (a.workspace/'snapshot.json').is_file(): raise SystemExit('Snapshot required')
if (source/'Assets/Editor/PixelSparkMirrorQA.cs').exists():
    raise SystemExit('Mirror snapshot already consumed; preserve it and create a fresh workspace')
env={**os.environ,'TYPINGHERO_RESULTS_PATH':str(results.resolve())}
label=a.action+('-left' if a.mirror_fixture else '-right') if a.action=='build' else a.action
args=[str(a.editor),'-batchmode','-nographics','-projectPath',str(source.resolve()),'-logFile',str((results/(label+'.log')).resolve())]
if a.action=='tests': args+=['-runTests','-testPlatform','PlayMode','-testCategory','Quick','-testFilter','ImpactVfxTests','-testResults',str((results/'tests.xml').resolve())]
else:
    output=a.workspace/('player-left' if a.mirror_fixture else 'player-right')
    if a.mirror_fixture:
        fixture=source/'Assets/Editor/PixelSparkMirrorQA.cs'
        if fixture.exists(): raise SystemExit('Mirror fixture already present')
        fixture.write_bytes(a.mirror_fixture.read_bytes().replace(b'\r\n',b'\n'))
    if output.exists(): raise SystemExit('Existing build preserved')
    env.update(TYPINGHERO_QA='1',TYPINGHERO_BUILD_PATH=str(output.resolve()))
    args+=['-quit','-buildTarget','WebGL','-executeMethod','GameLoopBuild.WebGL']
r=subprocess.run(args,env=env)
if r.returncode: raise SystemExit(r.returncode)
if a.action=='tests':
    xml=ET.parse(results/'tests.xml').getroot(); cases=xml.findall('.//test-case')
    failed=[c.attrib for c in cases if c.get('result')!='Passed']
    print(json.dumps({'count':len(cases),'nonpassing':failed}))
    if len(cases)!=44 or failed: raise SystemExit('Expected all 44 ImpactVfxTests passed')
elif not (output/'index.html').is_file(): raise SystemExit('Missing WebGL player')
