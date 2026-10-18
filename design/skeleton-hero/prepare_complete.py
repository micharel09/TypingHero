"""Export all36 original-based cels through LibreSprite; integrate only explicitly."""
import argparse
import json
import shutil

from PIL import Image, ImageFilter

from prepare_samples import ROOT, DESIGN, ART, TEMP, SHEETS, sha
from prepare_preserved import run


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--libresprite', required=True)
    parser.add_argument('--integrate', action='store_true')
    args = parser.parse_args()
    out = DESIGN/'complete'
    out.mkdir(exist_ok=True)
    TEMP.mkdir(parents=True, exist_ok=True)
    approved = DESIGN/'cluster-milestone'
    approval = json.loads((approved/'approval.json').read_text())
    for name, digest in approval['files'].items():
        assert sha(approved/name) == digest, 'Approved asset changed: '+name
    baseline = json.loads((DESIGN/'baseline.json').read_text())
    before = {p: sha(ROOT/p) for p in baseline}
    runtime = {str((ART/f'Skeleton_With_VFX/Skeleton_01_White_{s}.png').relative_to(ROOT)): s for s in SHEETS}
    old_drift = ['Assets/Scripts/UI/TutorialOverlay.cs', 'Assets/Scripts/VFX_SFX/ScreenFx/SlayerScreenFX.cs']
    # Concurrent player-attack-sorting task, verified against the pre-build
    # snapshot. Preserve its added component; never reset the art baseline.
    player_sorting_drift = ('Assets/Characters/Player/Player.prefab',
                           '4a3c4c617fcf934a58dd8eee981f159d04c474dfc94659893d4b7a5b34e85dc3')
    for path, record in baseline.items():
        if before[path] == record['sha256'] or path.replace('\\', '/') in old_drift:
            continue
        if (path.replace('\\','/'),before[path]) == player_sorting_drift:
            continue
        assert path in runtime and (out/(runtime[path]+'.png')).exists(), 'Unexpected input drift: '+path
        assert before[path] == sha(out/(runtime[path]+'.png')), 'Runtime differs from verified design sheet'
    poses = json.loads((DESIGN/'remaining-poses.json').read_text())
    script = (DESIGN/'original-pixel-edit.js').read_text(encoding='utf-8')
    approved_pixels = {}
    for sheet,count in [('Idle',1),('Attack1',10)]:
        image=Image.open(DESIGN/f'originals/Skeleton_01_White_{sheet}.png').convert('RGBA')
        for i in range(count):
            approved_pixels.setdefault(image.crop((i*96,0,(i+1)*96,64)).tobytes(), 'idle-0' if sheet=='Idle' else f'attack-{i}')
    reports = []
    painted_inputs = {}
    for sheet, count in SHEETS.items():
        name = f'Skeleton_01_White_{sheet}.png'
        original_path = DESIGN/'originals'/name
        key = str((ART/'Skeleton_With_VFX'/name).relative_to(ROOT))
        assert sha(original_path) == baseline[key]['sha256']
        source = Image.open(original_path).convert('RGBA')
        plain = Image.open(ART/'Skeleton_Without_VFX'/name).convert('RGBA')
        frames = []
        for i in range(count):
            pose = f'{sheet.lower()}-{i}'
            milestone = 'idle-0' if sheet == 'Idle' and i == 0 else f'attack-{i}' if sheet == 'Attack1' else None
            original = source.crop((i*96, 0, (i+1)*96, 64))
            # Identical original cels across Idle/Attack/Hurt/Die must also have
            # identical paint. Reuse only exact RGBA matches, never another pose.
            milestone = milestone or approved_pixels.get(original.tobytes())
            native, png = out/(pose+'.aseprite'), out/(pose+'.png')
            if milestone:
                for suffix in ['.png', '.aseprite', '-protected.png']:
                    shutil.copyfile(approved/(milestone+suffix), out/(pose+suffix))
                mask = Image.open(out/(pose+'-protected.png')).convert('L')
            else:
                is_flash = sheet == 'Hurt' and i == 1
                materials = [(250,250,250),(237,237,237),(191,171,169),(205,179,168)] if is_flash else [(255,255,255),(204,204,204),(73,16,9),(112,39,7)]
                material_mask = Image.new('L', (96,64))
                for y in range(64):
                    for x in range(96):
                        if original.getpixel((x,y))[:3] in materials:
                            material_mask.putpixel((x,y),255)
                # Two-pixel padding includes the250-white blade edge, hilt and
                # neighbouring bone at overlaps, even for the flattened death cels.
                mask = material_mask.filter(ImageFilter.MaxFilter(5))
                for y in range(64):
                    for x in range(96):
                        if original.getpixel((x,y)) != plain.getpixel((i*96+x,y)):
                            mask.putpixel((x,y),255)
                protected = [[x,y,*original.getpixel((x,y))] for y in range(64) for x in range(96) if mask.getpixel((x,y))]
                source_png = TEMP/(pose+'-original.png')
                original.save(source_png)
                js = TEMP/(pose+'-complete.js')
                js.write_text('var pose='+json.dumps(pose)+';\nvar remainingPoses='+json.dumps({pose:poses[pose]})+';\nvar protectedPixels='+json.dumps(protected)+';\n'+script, encoding='utf-8')
                run([args.libresprite,'-b',str(source_png),'--script',str(js),'--save-as',str(native)])
                run([args.libresprite,'-b',str(native),'--save-as',str(png)])
                mask.save(out/(pose+'-protected.png'))
            edited = Image.open(png).convert('RGBA')
            input_pixels=original.tobytes()
            painted_inputs.setdefault(input_pixels,edited.tobytes())
            assert painted_inputs[input_pixels]==edited.tobytes(), 'Identical original cels must have identical paint: '+pose
            assert edited.size == (96,64)
            assert set(edited.getchannel('A').tobytes()) <= {0,255}
            assert edited.getchannel('A').tobytes() == original.getchannel('A').tobytes(), pose
            assert all(edited.getpixel((x,y)) == original.getpixel((x,y)) for y in range(64) for x in range(96) if mask.getpixel((x,y))), pose
            roundtrip = TEMP/(pose+'-roundtrip.png')
            run([args.libresprite,'-b',str(native),'--save-as',str(roundtrip)])
            assert Image.open(roundtrip).convert('RGBA').tobytes() == edited.tobytes(), pose
            if sheet == 'Hurt' and i == 1:
                # This entire original cel is a baked VFX flash. Exact RGBA of
                # the effect has priority over retouching the bones underneath.
                assert edited.tobytes() == original.tobytes(), 'Baked flash must remain exact'
            else:
                assert edited.tobytes() != original.tobytes(), pose
            frames.append(edited)
            reports.append(dict(pose=pose,sha256=sha(png),alphaIdentical=True,weaponVfxIdentical=True,nativeRoundTrip=True,approvedSource=milestone))
        sheet_image = Image.new('RGBA', source.size)
        for i, frame in enumerate(frames):
            sheet_image.paste(frame,(i*96,0))
        staging = TEMP/(sheet+'-complete-sheet.png')
        sheet_image.save(staging)
        run([args.libresprite,'-b',str(staging),'--save-as',str(out/(sheet+'.aseprite'))])
        run([args.libresprite,'-b',str(out/(sheet+'.aseprite')),'--save-as',str(out/(sheet+'.png'))])
        assert Image.open(out/(sheet+'.png')).convert('RGBA').tobytes() == sheet_image.tobytes()
    assert all(sha(ROOT/p) == digest for p,digest in before.items()), 'Input changed during export'
    for name,digest in approval['files'].items():
        assert sha(approved/name) == digest
    (TEMP/'complete-checks.json').write_text(json.dumps(reports,indent=2)+'\n')
    if args.integrate:
        for target, sheet in runtime.items():
            shutil.copyfile(out/(sheet+'.png'), ROOT/target)
        assert all(sha(ROOT/p) == (sha(out/(runtime[p]+'.png')) if p in runtime else digest) for p,digest in before.items())
    print(json.dumps(dict(frames=len(reports),approvedFrames=11,alpha='exact',weaponVfx='exact',nativeRoundTrip=True,integrated=args.integrate),indent=2))


if __name__ == '__main__':
    main()
