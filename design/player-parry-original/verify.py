"""Check original imports and both authored timelines without modifying assets."""
from pathlib import Path
import hashlib
import json
import re

here = Path(__file__).resolve().parent
root = here.parents[1]
contract = json.loads((here / 'source-contract.json').read_text())
contacts = json.loads((here / 'contacts.json').read_text())
for path, expected in contract['hashes'].items():
    assert hashlib.sha256((root / path).read_bytes()).hexdigest() == expected, path
meta = (root / (contract['source'] + '.meta')).read_text()
assert 'guid: ' + contract['guid'] in meta
assert 'spritePixelsToUnits: 16' in meta
assert 'guid: ' + contract['normalGuid'] in meta
assert contacts['sourceGuid'] == contract['guid']
ids = []
for frame in contacts['frames']:
    block = meta.split('name: ' + frame['sprite'] + '\n')[1].split('    - serializedVersion:')[0]
    assert re.search(r'internalID: (-?\d+)', block)[1] == str(frame['fileId'])
    assert 'pivot: {x: 0.49, y: 0.51}' in block
    assert int(re.search(r'\n        x: (\d+)', block)[1]) == frame['rect'][0]
    ids.append(frame['fileId'])
assert [f['index'] for f in contacts['frames']] == [2, 3, 4, 5]

for path, guid in contract['clipGuids'].items():
    assert 'guid: ' + guid in (root / (path + '.meta')).read_text(), path
    text = (root / path).read_text()
    success = 'success' in path
    expected = [0, .8, 52 / 60, 56 / 60] if success else [0, .25, 19 / 60, 23 / 60]
    curve = text.split('  m_PPtrCurves:')[1].split('  m_SampleRate:')[0]
    keys = re.findall(r'- time: ([\d.e-]+)\s+value: \{fileID: (-?\d+), guid: (\w+)', curve)
    assert [int(k[1]) for k in keys] == ids, path
    assert all(k[2] == contract['guid'] for k in keys), path
    assert all(abs(float(k[0]) - t) < 1e-6 for k, t in zip(keys, expected)), path
    assert 'm_StopTime: ' + ('0.95' if success else '0.4') in text
    assert 'm_LoopTime: 0' in text
    events = re.findall(r'- time: ([\d.e-]+)\s+functionName: (\w+)', text.split('  m_Events:')[1])
    assert events == ([('0.95', 'Anim_ParryEnd')] if success else
                      [('0', 'Anim_ParryOpen'), ('0', 'Anim_ParryPoseSfx'), ('0.25', 'Anim_ParryClose'), ('0.4', 'Anim_ParryEnd')]), path
    assert 'intParameter: ' + ('1' if success else '0') in text.split('functionName: Anim_ParryEnd')[1]

controller = (root / 'Assets/Characters/Player/AC_Controller.controller').read_text()
for file_id in contract['controllerStateIds'].values():
    assert '--- !u!1102 &' + str(file_id) + '\n' in controller
for file_id in ['-7033551236960827186', '1303878647651666134']:
    part = controller.split('--- !u!1101 &' + file_id + '\n')[1].split('--- !u!')[0]
    assert 'm_TransitionDuration: 0\n' in part and 'm_ExitTime: 1\n' in part
data = json.loads((here / 'preview-data.js').read_text(encoding='utf-8').removeprefix('window.PARRY_PREVIEW = ').strip().removesuffix(';'))
assert data['frames'] == contacts['frames'] and data['loopPauseMs'] == 450
for authored, preview in zip(contacts['clips'], data['clips']):
    assert authored['endMs'] == preview['endMs']
    starts = [sum(preview['holds'][:i]) for i in range(4)]
    assert all(abs(a - b) < 1e-8 for a, b in zip(starts, authored['startsMs']))
    assert abs(sum(preview['holds']) - preview['endMs']) < 1e-8
assert not (root / 'design/player-parry-ninja/versions/ver1').exists()
for folder in ['Assets', 'tools']:
    for path in (root / folder).rglob('*'):
        if path.is_file() and path.suffix in ['.cs', '.anim', '.controller', '.prefab', '.unity', '.asset', '.meta', '.py', '.json', '.cjs']:
            assert b'player-parry-ninja' not in path.read_bytes(), path
print(f"PASS: {len(contract['hashes'])} original/import/GUID hashes, all clip keys/events, contact table, preview and removed-reference audit.")
