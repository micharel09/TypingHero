"""Rebuild three review samples through LibreSprite, never writing Unity assets.

Run from repository root with --libresprite pointing at LibreSprite 1.3.
Pillow only reads/crops sources, prepares masks and verifies exported pixels.
Actual body drawing and native source/PNG export run inside LibreSprite.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import subprocess

from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
DESIGN = Path(__file__).resolve().parent
ART = ROOT / 'Assets/Art/Skeletons_Free_Pack/Skeleton_Sword/Skeleton_White'
TEMP = Path('E:/CodexTemp/TypingHero/skeleton-hero-art/samples')
SHEETS = {'Idle': 8, 'Attack1': 10, 'Hurt': 5, 'Die': 13}
SAMPLES = [('idle', 'Idle', 0), ('anticipation', 'Attack1', 3), ('contact', 'Attack1', 5)]


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def inside(x, y, polygon):
    result = False
    for (ax, ay), (bx, by) in zip(polygon, polygon[1:] + polygon[:1]):
        if (ay > y) != (by > y) and x < (bx-ax)*(y-ay)/(by-ay)+ax:
            result = not result
    return result


def capture_baseline():
    originals = DESIGN / 'originals'
    originals.mkdir(exist_ok=True)
    records = {}
    for sheet, count in SHEETS.items():
        texture = ART / f'Skeleton_With_VFX/Skeleton_01_White_{sheet}.png'
        for path in [texture, Path(str(texture)+'.meta')]:
            copy = originals / path.name
            if copy.exists():
                assert sha(copy) == sha(path), f'Original differs: {path}'
            else:
                shutil.copy2(path, copy)
            records[str(path.relative_to(ROOT))] = {'sha256': sha(path)}
        metadata = Path(str(texture)+'.meta').read_text()
        assert Image.open(texture).size == (count*96, 64)
        records[str(texture.relative_to(ROOT))].update(
            size=[count*96, 64], frames=count,
            guid=re.search(r'^guid: (\w+)', metadata, re.M)[1],
            # Full importer bytes are preserved alongside this structured index.
            sprites=re.findall(r'name: (\S+)', metadata),
            spriteIDs=re.findall(r'spriteID: (\S+)', metadata),
            internalIDs=re.findall(r'internalID: (-?\d+)', metadata))
    controller = ROOT / 'Assets/Characters/Enermy/AC_Skeleton.controller'
    guids = re.findall(r'm_Motion: .*guid: (\w+)', controller.read_text())
    clips = []
    for meta in (ROOT/'Assets').rglob('*.anim.meta'):
        if any(f'guid: {guid}' in meta.read_text() for guid in guids):
            clip = Path(str(meta)[:-5])
            clips.append(clip)
    assert len(clips) == 6
    contracts = clips + [controller]
    contracts += list((ROOT/'Assets/Art/male_hero').rglob('*'))
    contracts += [ROOT/'Assets/Characters/Enermy/Skeleton.prefab', ROOT/'Assets/Characters/Player/Player.prefab']
    contracts += list((ROOT/'Assets/Scripts').rglob('*.cs'))
    contracts += list((ROOT/'Assets/Data/Attack').rglob('*'))
    for path in contracts:
        if path.is_file():
            records[str(path.relative_to(ROOT))] = {'sha256': sha(path)}
    for clip in clips:
        # Full YAML retains every keyframe, event and setting, including dirty inputs.
        copy = originals / 'clips' / clip.relative_to(ROOT)
        copy.parent.mkdir(parents=True, exist_ok=True)
        if copy.exists():
            assert sha(copy) == sha(clip)
        else:
            shutil.copy2(clip, copy)
        records[str(clip.relative_to(ROOT))]['yaml'] = clip.read_text()
    output = DESIGN / 'baseline.json'
    if output.exists():
        baseline = json.loads(output.read_text())
        for path, record in baseline.items():
            assert sha(ROOT/path) == record['sha256'], f'Baseline input changed: {path}'
    else:
        output.write_text(json.dumps(records, indent=2)+'\n')
    return records


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--libresprite', required=True)
    args = parser.parse_args()
    capture_baseline()
    TEMP.mkdir(parents=True, exist_ok=True)
    output = DESIGN/'samples'
    output.mkdir(exist_ok=True)
    reports = []
    # Explicit native-coordinate weapon envelopes, independent of pixel colors.
    polygons = {
        'idle': [(56,39),(66,21),(69,21),(61,41),(61,47),(56,47),(54,43)],
        'anticipation': [(18,27),(23,27),(40,32),(43,33),(44,38),(36,38),(34,36)],
        'contact': [(64,26),(69,25),(95,24),(95,28),(71,32),(64,34)]
    }
    for pose, sheet, index in SAMPLES:
        source = Image.open(DESIGN/f'originals/Skeleton_01_White_{sheet}.png').convert('RGBA')
        frame = source.crop((index*96,0,(index+1)*96,64))
        no_vfx = Image.open(ART/f'Skeleton_Without_VFX/Skeleton_01_White_{sheet}.png').convert('RGBA').crop((index*96,0,(index+1)*96,64))
        protected = []
        mask = Image.new('L', (96,64))
        for y in range(64):
            for x in range(96):
                # Preserve all visible VFX pixels plus the geometry-based weapon mask.
                vfx = frame.getpixel((x,y)) != no_vfx.getpixel((x,y)) and frame.getpixel((x,y))[3] != 0
                if inside(x+.5,y+.5,polygons[pose]) or vfx:
                    protected.append([x,y,*frame.getpixel((x,y))])
                    mask.putpixel((x,y),255)
        mask.save(output/f'{pose}-protected.png')
        frame.save(TEMP/f'{pose}-original.png')
        script = TEMP/f'{pose}.js'
        script.write_text('var pose='+json.dumps(pose)+';\nvar protectedPixels='+json.dumps(protected)+';\n'+(DESIGN/'samples.js').read_text())
        native = output/f'{pose}.aseprite'
        process = subprocess.run([args.libresprite,'-b',str(TEMP/f'{pose}-original.png'),'--script',str(script),'--save-as',str(native)], check=True, capture_output=True, text=True)
        assert not re.search(r'(?:Type|Reference|Syntax|Range)?Error:', process.stdout+process.stderr), process.stdout+process.stderr
        png = output/f'{pose}.png'
        subprocess.run([args.libresprite,'-b',str(native),'--save-as',str(png)],check=True,capture_output=True)
        result = Image.open(png).convert('RGBA')
        assert result.tobytes() != frame.tobytes(), 'LibreSprite did not draw the sample'
        assert result.size == (96,64)
        assert set(result.getchannel('A').tobytes()) <= {0,255}
        assert all(result.getpixel((p[0],p[1])) == tuple(p[2:]) for p in protected)
        # Exact original bottom row is the footprint contract.
        assert [result.getpixel((x,63))[3] for x in range(96)] == [frame.getpixel((x,63))[3] for x in range(96)], pose+' footprint'
        reports.append({'pose':pose,'sheet':sheet,'frame':index,'size':[96,64], 'protectedPixels':len(protected),'alpha':[0,255],'sha256':sha(png)})
    (TEMP/'sample-checks.json').write_text(json.dumps(reports,indent=2)+'\n')
    print(json.dumps(reports,indent=2))


if __name__ == '__main__':
    main()
