"""Build the self-contained review artifact from verified samples and Unity timing."""
import base64
from io import BytesIO
import json
from pathlib import Path
import re

from PIL import Image
from prepare_samples import SHEETS

DESIGN = Path(__file__).resolve().parent
ROOT = DESIGN.parents[1]


def data(image):
    buffer = BytesIO()
    image.save(buffer, format='PNG')
    return 'data:image/png;base64,'+base64.b64encode(buffer.getvalue()).decode()


def timeline(clip, sheet):
    text = (ROOT/clip).read_text()
    meta = (DESIGN/f'originals/Skeleton_01_White_{sheet}.png.meta').read_text()
    entries = re.findall(r'name: \S+_(\d+)\n.*?internalID: (-?\d+)', meta, re.S)
    ids = {int(identifier): int(index) for index, identifier in entries}
    keys = [{'time': float(time), 'frame': ids[int(identifier)]} for time, identifier in
            re.findall(r'- time: ([\d.]+)\n\s+value: \{fileID: (-?\d+)', text)]
    return {'keys': keys, 'duration': float(re.search(r'm_StopTime: ([\d.]+)', text)[1]),
            'events': [{'time': float(time), 'name': name} for time, name in
                       re.findall(r'- time: ([\d.]+)\n\s+functionName: (\w+)', text)]}


def main():
    images = {}
    for name in SHEETS:
        images[name] = data(Image.open(DESIGN/f'originals/Skeleton_01_White_{name}.png'))
    images['hero'] = data(Image.open(ROOT/'Assets/Art/male_hero/individual_sheets/male_hero-idle.png').crop((0,0,128,128)))
    samples,palette_available={},[]
    for sheet,count in SHEETS.items():
        samples[sheet]={}
        original=Image.open(DESIGN/f'originals/Skeleton_01_White_{sheet}.png').convert('RGBA')
        for i in range(count):
            pose=f'{sheet.lower()}-{i}'
            key='idle' if sheet=='Idle' and i==0 else ('attack' if sheet=='Attack1' else sheet.lower())+str(i)
            samples[sheet][i]=key
            edited=Image.open(DESIGN/f'complete/{pose}.png').convert('RGBA')
            images[key]=data(edited)
            milestone='idle-0' if sheet=='Idle' and i==0 else f'attack-{i}' if sheet=='Attack1' else None
            palette=Image.open(DESIGN/f'preserved-original-milestone/{milestone}.png').convert('RGBA') if milestone else original.crop((i*96,0,(i+1)*96,64))
            images['palette-'+key]=data(palette)
            if milestone:palette_available.append(key)
            mask=Image.open(DESIGN/f'complete/{pose}-protected.png').convert('L')
            tint=Image.new('RGBA',mask.size,(255,100,190,255));tint.putalpha(mask)
            images['mask-'+key]=data(tint)
            diff=Image.new('RGBA',(96,64))
            for y in range(64):
                for x in range(96):
                    if edited.getpixel((x,y))!=palette.getpixel((x,y)):
                        diff.putpixel((x,y),edited.getpixel((x,y)))
            images['diff-'+key]=data(diff)
    environment = ROOT/'Assets/Art/Environment/TinyPixelJapan'
    for name in ['Sky','Clouds','Fuji','Mountain_Back','Mountain_Middle','Mountain_Front','BackgroundTrees','House','Ground']:
        images[name] = data(Image.open(environment/(name+'.png')))
    timelines = {'Idle': timeline('Assets/Characters/Enermy/skeleton_idle.anim', 'Idle'),
                 'Attack1': timeline('Assets/Scripts/skeleton_attack_heavy.anim', 'Attack1'),
                 'Hurt': timeline('Assets/Scripts/skeleton_hit.anim', 'Hurt'),
                 'Die': timeline('Assets/Scripts/skeleton_die.anim', 'Die'),
                 'Stun': timeline('Assets/Characters/Enermy/skeleton_stun.anim', 'Hurt'),
                 'hitBySlayer': timeline('Assets/Art/VFX/skeleton_hitBySlayer.anim', 'Hurt')}
    for alias in ['Stun','hitBySlayer']:
        images[alias]=images['Hurt'];samples[alias]=samples['Hurt']
    assert [k['frame'] for k in timelines['Attack1']['keys']] == list(range(10))
    assert timelines['Attack1']['duration'] == 1.3333334
    assert next(k['frame'] for k in reversed(timelines['Attack1']['keys']) if k['time']<=.7)==3
    assert next(k['frame'] for k in reversed(timelines['Attack1']['keys']) if k['time']<=1)==5
    payload = {'images': images, 'timelines': timelines, 'samples':samples,'paletteAvailable':palette_available,
               'landmarks': json.loads((DESIGN/'cluster-milestone/landmarks.json').read_text())}
    template = (DESIGN/'preview.template.html').read_text(encoding='utf8')
    (DESIGN/'review.html').write_text(template.replace('/*PAYLOAD*/', json.dumps(payload)),encoding='utf8')
    print('Built design/skeleton-hero/review.html; timings resolved by original sprite internalID')


if __name__ == '__main__':
    main()
