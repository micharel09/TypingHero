"""Build the complete approved player recolor outside Assets. Python + Pillow only."""
import argparse
import base64
import hashlib
import json
import re
from PIL import Image
from materials import HERE, ROOT, SOURCE, REFERENCE, PALETTE, decode, overlay, draft_mask, rgb


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def author():
    overrides = json.loads((HERE/'overrides.json').read_text())
    locked = {}
    for path in sorted((SOURCE/'individual_sheets').glob('*.png')):
        name = path.stem.removeprefix('male_hero-')
        image = Image.open(path).convert('RGBA')
        folder = HERE/'masks'/name
        folder.mkdir(parents=True,exist_ok=True)
        locked[path.name] = digest(path)
        for frame in range(image.width//128):
            tile = image.crop((frame*128,0,(frame+1)*128,128))
            mask, _, _ = draft_mask(tile,name,frame,overrides)
            mask.save(folder/f'{frame+1:02}.png')
    locked['male_hero.png'] = digest(SOURCE/'male_hero.png')
    (HERE/'sources.json').write_text(json.dumps(locked,indent=2)+'\n')


def read_clips():
    refs = {}
    for meta in (SOURCE/'individual_sheets').glob('*.png.meta'):
        text = meta.read_text()
        guid = re.search(r'^guid: (\w+)',text,re.M)[1]
        for name, identifier in re.findall(r'name: (male_hero-[\w]+_\d+).*?internalID: (-?\d+)',text,re.S):
            sheet, frame = name.removeprefix('male_hero-').rsplit('_',1)
            refs[(guid,identifier)] = [sheet,int(frame)]
    paths = sorted((ROOT/'Assets/Characters/Player').rglob('*.anim'))
    paths += sorted((SOURCE/'individual_sheets').glob('*.anim'))
    clips = []
    for path in paths:
        text = path.read_text()
        keys = re.findall(r'- time: ([\d.]+)\s+value: \{fileID: (-?\d+), guid: (\w+)',text)
        if not keys: continue
        assert all((guid,identifier) in refs for _,identifier,guid in keys), path
        stop = float(re.search(r'm_StopTime: ([\d.]+)',text)[1])
        times = [float(t) for t,_,_ in keys]
        assert times[0]==0 and stop>times[-1], path
        clips.append({'name':path.stem, 'source':path.relative_to(ROOT).as_posix(),
                      'frames':[refs[(guid,identifier)] for _,identifier,guid in keys],
                      'durations':[(b-a)*1000 for a,b in zip(times,times[1:]+[stop])],
                      'loop':bool(int(re.search(r'm_LoopTime: (\d+)',text)[1]))})
    return clips


def build():
    locked = json.loads((HERE/'sources.json').read_text())
    palettes = {i:{rgb(k):rgb(v) for k,v in colors.items()} for i,colors in enumerate(PALETTE.values(),1)}
    result = []; cell_map = {}; total_changed = 0
    (HERE/'sheets').mkdir(exist_ok=True)
    for path in sorted((SOURCE/'individual_sheets').glob('*.png')):
        assert digest(path)==locked[path.name], path
        name = path.stem.removeprefix('male_hero-')
        original = Image.open(path).convert('RGBA'); output = original.copy()
        counts = []
        for frame in range(original.width//128):
            tile = original.crop((frame*128,0,(frame+1)*128,128))
            decoded = decode(tile,name,frame)
            mask = Image.open(HERE/'masks'/name/f'{frame+1:02}.png').convert('L')
            assert mask.size==(128,128)
            factor,_ = overlay(name,frame); changed=0
            for y in range(128):
                for x in range(128):
                    old = tile.getpixel((x,y)); label = mask.getpixel((x,y))
                    assert label in (0,1,2,3,4)
                    if not label: continue
                    base = decoded[(x,y)]
                    assert base in palettes[label], (name,frame,x,y,base,label)
                    target = palettes[label][base]
                    color = tuple(max(0,min(255,round(old[c]+factor*(target[c]-base[c])))) for c in range(3))
                    output.putpixel((128*frame+x,y),(*color,old[3]));changed+=color!=old[:3]
            new_tile = output.crop((frame*128,0,(frame+1)*128,128))
            assert tile.getchannel('A').tobytes()==new_tile.getchannel('A').tobytes()
            for p,base in decoded.items():
                old = tile.getpixel(p); new = new_tile.getpixel(p); label = mask.getpixel(p)
                if not label: assert old==new
                if base in {rgb(c) for c in ('ffc5b4','d77e86','24523b','59c135','1a7a3e','b3b9d1')}:
                    assert old==new, (name,frame,p,'face/eyes')
                if base in {rgb('a24e57'),rgb('d2947f')}: assert label in (0,4)
                if base==rgb('ffffff') and label: assert label==3
            key=hashlib.sha256(tile.tobytes()).hexdigest()
            if key in cell_map:
                assert cell_map[key].tobytes()==new_tile.tobytes(), (name,frame,'Same source cel received inconsistent masks')
            cell_map[key]=new_tile
            counts.append(changed); total_changed+=changed
        assert all(a==b for a,b in zip(original.get_flattened_data(),output.get_flattened_data()) if not a[3])
        if name=='idle':
            approved=Image.open(REFERENCE/'male_hero-idle-dark.png').convert('RGBA')
            assert output.tobytes()==approved.tobytes(), 'Approved idle changed'
        output.save(HERE/'sheets'/path.name)
        result.append({'name':name,'frames':original.width//128,'changed':counts,'sourceSha256':digest(path)})
    atlas_path=SOURCE/'male_hero.png';assert digest(atlas_path)==locked[atlas_path.name]
    atlas=Image.open(atlas_path).convert('RGBA'); atlas_output=atlas.copy(); matched=0
    for y in range(atlas.height//128):
        for x in range(atlas.width//128):
            tile=atlas.crop((x*128,y*128,(x+1)*128,(y+1)*128))
            key=hashlib.sha256(tile.tobytes()).hexdigest()
            if key in cell_map:
                atlas_output.paste(cell_map[key],(x*128,y*128));matched+=1
            else: assert not tile.getbbox(), (x,y,'Unmatched occupied master cell')
    assert matched==sum(item['frames'] for item in result)==166
    atlas_output.save(HERE/'male_hero.png')
    manifest={'sheets':result,'sheetCount':len(result),'frameCount':matched,'changedPixels':total_changed,
              'approvedIdleIdentical':True,'alphaAndUnmaskedPixelsIdentical':True,
              'clips':read_clips()}
    (HERE/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
    (HERE/'palette.json').write_text(json.dumps(PALETTE,indent=2)+'\n')
    print(f"PASS: {len(result)} sheets / {matched} frames / master atlas; {total_changed} pixels recolored.")


if __name__=='__main__':
    parser=argparse.ArgumentParser();parser.add_argument('--author-masks',action='store_true');args=parser.parse_args()
    if args.author_masks: author()
    build()
