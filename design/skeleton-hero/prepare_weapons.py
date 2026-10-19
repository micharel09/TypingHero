"""Render review-only weapon cels in LibreSprite, package editable layered timelines.

Pillow reads, assembles and checks exports; all new pixels are drawn by the JS
inside LibreSprite. ASE packaging follows aseprite/docs/ase-file-specs.md.
Never writes Assets or existing sword deliverables.
"""
import argparse
import hashlib
import json
import math
from pathlib import Path
import re
import struct
import subprocess
import zlib

from PIL import Image

from build_preview import DESIGN, ROOT, data, timeline

OUT = DESIGN / 'weapon-milestone'
TEMP = Path('E:/CodexTemp/TypingHero/skeleton-hero-art/weapon-build')


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def run(exe, *args):
    result = subprocess.run([exe, '-b', *map(str, args)], capture_output=True, text=True, check=True)
    assert not re.search(r'(?:Type|Reference|Syntax|Range)?Error:', result.stdout + result.stderr), result.stdout + result.stderr


def patch(image, box):
    x0, y0, x1, y1 = box
    return [[x-x0, y-y0, *image.getpixel((x, y))[:3]]
            for y in range(y0, y1) for x in range(x0, x1) if image.getpixel((x, y))[3]]


def pose(kind, body, butt, tip, elbows, knees, blade, light, edge, socket, fx=False, lean=None):
    # The two grip distances are fixed along a constant-length shaft.
    length = math.dist(butt, tip)
    distances = [7, 15] if kind == 'axe' else [17, 31]
    hands = [[round(butt[0]+(tip[0]-butt[0])*d/length),
              round(butt[1]+(tip[1]-butt[1])*d/length)] for d in distances]
    x, y, hip_x, hip_y = body
    return dict(kind=kind, head=[x-3, y-13], chest=[x, y], hip=[hip_x, hip_y],
                butt=butt, tip=tip, elbows=elbows, knees=knees, hands=hands,
                blade=blade, light=light, edge=edge, socket=socket, fx=fx, lean=lean)


def axe_poses():
    # Preserve Idle0; overhead cels retain the approved bearded blade silhouette.
    poses = [
        pose('axe',[43,31,43,44],[48,48],[64,20],[[57,40],[38,34]],[[38,54],[51,54]],
             [[62,19],[64,19],[64,22],[64,26],[66,26],[69,25],[69,27],[69,30],[66,33],[62,35],[60,36],[61,34],[61,31],[60,28],[59,26],[60,23]],
             [[64,26],[66,26],[69,26],[68,29],[65,32],[62,34],[61,34],[61,31],[60,28]],
             [[69,26],[68,29],[65,32],[61,35],[60,35]],[[63,20],[61,23],[62,24],[63,24],[65,20]])
    ]

    # One local-space head/socket, transformed from the actual shaft axis.
    source = poses[0]
    source_length = math.dist(source['butt'], source['tip'])
    ux, uy = [(source['tip'][i]-source['butt'][i])/source_length for i in range(2)]
    def head_at(butt, tip):
        length = math.dist(butt, tip)
        vx, vy = [(tip[i]-butt[i])/length for i in range(2)]
        result = []
        for name in ['blade', 'light', 'edge', 'socket']:
            points = []
            for x, y in source[name]:
                dx, dy = x-source['tip'][0], y-source['tip'][1]
                along, across = dx*ux+dy*uy, -dx*uy+dy*ux
                points.append([round(tip[0]+along*vx-across*vy),
                               round(tip[1]+along*vy+across*vx)])
            result.append(points)
        # The cutting rim ends at the beard toe in every orientation.
        toe = max(range(len(source['blade'])), key=lambda i:
                  -(source['blade'][i][0]-source['tip'][0])*uy
                  +(source['blade'][i][1]-source['tip'][1])*ux)
        result[2].append(result[0][toe][:])
        return result

    ground_head = head_at([53, 0], [85, 0])
    ground_y = 63-max(y for x, y in ground_head[2])
    # Authored projected joints: shoulder elevation and torso rotation lead the hands.
    # Occlusion/foreshortening varies per cel; equal limb segments are not a pose rule.
    landmarks = [
        ([42,30],[43,45],[34,17],[48,35],[44,3], [[48,27],[39,25]], [[53,28],[42,23]], [[38,55],[51,55]]),
        ([41,29],[42,44],[37,16],[55,19],[25,8], [[46,26],[38,25]], [[53,20],[34,18]], [[37,55],[50,55]]),
        ([40,30],[41,45],[36,17],[54,20],[24,9], [[45,27],[37,26]], [[52,21],[33,19]], [[37,55],[50,55]]),
        ([47,36],[45,47],[47,23],[53,39],[84,47], [[51,36],[44,34]], [[52,46],[54,36]], [[39,56],[54,57]]),
        ([50,40],[46,48],[51,27],[53,ground_y],[85,ground_y], [[54,40],[46,38]], [[54,50],[58,41]], [[40,57],[55,58]]),
        ([51,42],[46,50],[52,29],[53,ground_y],[85,ground_y], [[55,42],[47,40]], [[55,51],[60,44]], [[40,58],[56,58]]),
        ([50,42],[45,50],[51,29],[53,ground_y],[85,ground_y], [[54,42],[46,40]], [[54,51],[59,44]], [[39,58],[55,58]]),
        ([50,41],[45,49],[51,28],[53,ground_y],[85,ground_y], [[54,41],[46,39]], [[54,50],[59,43]], [[39,57],[55,58]]),
        ([50,41],[45,49],[51,28],[53,ground_y],[85,ground_y], [[54,41],[46,39]], [[54,50],[59,43]], [[39,57],[55,58]]),
        # Brace through the hips while the cutting edge is still grounded.
        ([49,40],[43,48],[50,27],[53,ground_y],[85,ground_y], [[53,40],[45,38]], [[53,50],[58,42]], [[38,57],[53,57]]),
        ([48,38],[42,47],[48,25],[50,43],[82,43], [[52,38],[44,36]], [[52,47],[56,39]], [[38,56],[52,56]]),
        ([46,35],[42,46],[44,22],[46,43],[76,32], [[50,35],[42,34]], [[51,44],[52,34]], [[38,55],[51,55]]),
        ([44,32],[43,45],[41,19],[46,45],[69,23], [[49,33],[41,33]], [[54,41],[44,32]], [[38,54],[51,54]]),
        ([43,31],[43,44],[40,18],[48,47],[65,20], [[50,33],[40,33]], [[56,40],[39,33]], [[38,54],[51,54]])
    ]
    poses = [source]
    for i, (chest, hip, skull, butt, tip, shoulders, elbows, knees) in enumerate(landmarks, 1):
        if i >= 11:
            # Rasterize a constant 32px shaft before deriving head and grip positions.
            length = math.dist(butt, tip)
            tip = [round(butt[j]+(tip[j]-butt[j])*32/length) for j in range(2)]
        p = pose('axe', [*chest,*hip], butt, tip, elbows, knees,
                 *head_at(butt,tip), fx=i==4)
        p.update(head=skull, shoulders=shoulders, torso=i<=12)
        poses.append(p)
    poses.append(dict(source))  # cel15 is exactly Idle0, including RGBA and grips.
    # Sweep actual blade positions with translating hands, not a fixed fan.
    start, end = poses[3], poses[4]
    start_angle = math.atan2(start['tip'][1]-start['butt'][1], start['tip'][0]-start['butt'][0])
    end_angle = math.atan2(end['tip'][1]-end['butt'][1], end['tip'][0]-end['butt'][0])
    sweep = []
    for step in range(17):
        t = step/16
        angle = start_angle+(end_angle-start_angle)*t
        butt = [start['butt'][j]+(end['butt'][j]-start['butt'][j])*t for j in range(2)]
        butt[1] += 12*math.sin(math.pi*t)
        tip = [butt[0]+32*math.cos(angle), butt[1]+32*math.sin(angle)]
        sweep.append(head_at(butt,tip)[2])
    sweep[-1] = end['edge']
    assert all(0 < y <= max(q[1] for q in end['edge'])
               for rim in sweep for x,y in rim)
    end['sweep'] = sweep
    return poses


def spear_poses():
    values = [
        ([43,31,43,44],[23,41],[80,33],[[47,45],[47,38]],[[38,54],[51,54]]),
        ([42,31,42,44],[18,40],[76,36],[[42,45],[42,35]],[[38,54],[51,54]]),
        ([40,32,41,45],[12,39],[70,37],[[37,44],[39,35]],[[37,55],[50,55]]),
        ([39,32,40,45],[9,38],[67,36],[[34,42],[36,35]],[[37,55],[50,55]]),
        ([44,31,43,45],[24,38],[82,36],[[48,44],[49,35]],[[39,55],[52,55]]),
        ([49,31,46,45],[34,38],[92,36],[[59,43],[55,33]],[[41,55],[55,55]]),
        ([49,32,47,46],[33,38],[91,36],[[58,44],[55,34]],[[41,56],[55,56]]),
        ([46,32,45,45],[27,39],[85,37],[[52,44],[51,35]],[[40,55],[53,55]]),
        ([44,31,44,44],[24,40],[82,35],[[49,45],[49,36]],[[39,54],[52,54]])
    ]
    poses = []
    # Narrow leaf: widest near the socket, with a long taper and a continuous rib.
    # Keep each rounded diagonal on the shaft axis, including Idle and recovery.
    heads = [
        ([[66,35],[68,34],[70,32],[73,32],[77,32],[80,33],[77,34],[73,36],[70,36],[68,36]],
         [[67,35],[70,33],[73,33],[77,33],[80,33]],
         [[67,35],[72,34],[77,33],[80,33]],[[63,34],[66,34],[67,35],[66,36],[63,36]]),
        ([[62,37],[64,36],[66,35],[69,34],[73,35],[76,36],[73,37],[69,38],[66,39],[64,38]],
         [[63,37],[66,36],[69,35],[73,36],[76,36]],
         [[63,37],[68,37],[73,36],[76,36]],[[59,36],[62,36],[63,37],[62,38],[59,38]]),
        ([[56,37],[58,36],[60,35],[63,35],[67,36],[70,37],[67,38],[63,39],[60,39],[58,38]],
         [[57,37],[60,36],[63,36],[67,37],[70,37]],
         [[57,37],[62,37],[67,37],[70,37]],[[53,37],[56,36],[57,37],[56,38],[53,39]]),
        ([[53,36],[55,35],[57,34],[60,34],[64,35],[67,36],[64,37],[60,38],[57,38],[55,37]],
         [[54,36],[57,35],[60,35],[64,36],[67,36]],
         [[54,36],[59,36],[64,36],[67,36]],[[50,36],[53,35],[54,36],[53,37],[50,38]]),
        ([[68,36],[70,35],[72,34],[75,34],[79,35],[82,36],[79,37],[75,38],[72,38],[70,37]],
         [[69,36],[72,35],[75,35],[79,36],[82,36]],
         [[69,36],[74,36],[79,36],[82,36]],[[65,36],[68,35],[69,36],[68,37],[65,38]]),
        ([[78,36],[80,35],[82,34],[85,34],[89,35],[92,36],[89,37],[85,38],[82,38],[80,37]],
         [[79,36],[82,35],[85,35],[89,36],[92,36]],
         [[79,36],[84,36],[89,36],[92,36]],[[75,36],[78,35],[79,36],[78,37],[75,38]]),
        ([[77,36],[79,35],[81,34],[84,34],[88,35],[91,36],[88,37],[84,38],[81,38],[79,37]],
         [[78,36],[81,35],[84,35],[88,36],[91,36]],
         [[78,36],[83,36],[88,36],[91,36]],[[74,36],[77,35],[78,36],[77,37],[74,38]]),
        ([[71,37],[73,36],[75,35],[78,35],[82,36],[85,37],[82,38],[78,39],[75,39],[73,38]],
         [[72,37],[75,36],[78,36],[82,37],[85,37]],
         [[72,37],[77,37],[82,37],[85,37]],[[68,37],[71,36],[72,37],[71,38],[68,39]]),
        ([[68,36],[70,35],[72,34],[75,34],[79,34],[82,35],[79,36],[75,38],[72,38],[70,37]],
         [[69,36],[72,35],[75,35],[79,35],[82,35]],
         [[69,36],[74,36],[79,35],[82,35]],[[65,35],[68,35],[69,36],[68,37],[65,37]])
    ]
    for i,(body,butt,tip,elbows,knees) in enumerate(values):
        poses.append(pose('spear',body,butt,tip,elbows,knees,*heads[i],i==5))
    return poses


def chunk(kind, content):
    return struct.pack('<IH', len(content)+6, kind)+content


def string(value):
    raw=value.encode('utf8')
    return struct.pack('<H',len(raw))+raw


def native(path, frames, durations, animation_tags=None):
    """Package two untransformed RGBA layers; LibreSprite then reads and resaves."""
    if len(frames) != len(durations) or not frames:
        raise ValueError('Each native cel needs one duration')
    if any(not isinstance(duration, int) or not 0 < duration <= 65535
           for duration in durations):
        raise ValueError('Native durations must be positive uint16 milliseconds')
    if animation_tags is None:
        animation_tags = [(0, 0, 'Idle'), (1, len(frames)-1, 'Attack')]
    names = set()
    next_frame = 0
    for start, end, name in animation_tags:
        if start != next_frame or not start <= end < len(frames):
            raise ValueError('Animation tags must cover consecutive, disjoint cels')
        if not name or name in names:
            raise ValueError('Animation tag names must be nonempty and unique')
        names.add(name)
        next_frame = end+1
    if next_frame != len(frames):
        raise ValueError('Animation tags must cover every native cel')
    encoded=[]
    for i,(body,fx) in enumerate(frames):
        chunks=[]
        if i==0:
            for name in ['Body + weapon','VFX']:
                chunks.append(chunk(0x2004,struct.pack('<6HB3x',3,0,0,0,0,0,255)+string(name)))
            tags=struct.pack('<H8x',len(animation_tags))
            for start,end,name in animation_tags:
                tags+=struct.pack('<HHB8x3BB',start,end,0,100,180,180,0)+string(name)
            chunks.append(chunk(0x2018,tags))
        for layer,im in enumerate([body,fx]):
            cel=struct.pack('<HhhBH7xHH',layer,0,0,255,2,96,64)+zlib.compress(im.tobytes())
            chunks.append(chunk(0x2005,cel))
        content=b''.join(chunks)
        encoded.append(struct.pack('<IHHH2xI',16+len(content),0xF1FA,len(chunks),durations[i],0)+content)
    header=bytearray(128)
    struct.pack_into('<I5HIH',header,0,128+sum(map(len,encoded)),0xA5E0,len(frames),96,64,32,1,100)
    header[34:36]=bytes([1,1])
    path.write_bytes(bytes(header)+b''.join(encoded))


def main():
    parser=argparse.ArgumentParser()
    parser.add_argument('--libresprite',required=True)
    args=parser.parse_args()
    OUT.mkdir(exist_ok=True)
    TEMP.mkdir(parents=True,exist_ok=True)
    guard_paths=(list((ROOT/'Assets').rglob('*'))+list((DESIGN/'complete').glob('*'))
                 +list((DESIGN/'cluster-milestone').glob('*'))+list(OUT.glob('spear*'))+[OUT/'axe-idle-0.png', OUT/'axe-attack-0.png'])
    guard={p:sha(p) for p in guard_paths if p.is_file()}
    approved=Image.open(DESIGN/'complete/idle-0.png').convert('RGBA')
    patches={'head':patch(approved,(39,18,50,31)),
             'ribs':patch(approved,(40,31,50,43)),
             'pelvis':patch(approved,(40,44,50,49))}
    clip=timeline('Assets/Scripts/skeleton_attack_heavy.anim','Attack1')
    times=[k['time'] for k in clip['keys']]+[clip['duration']]
    durations=[100]+[round(times[i+1]*1000)-round(times[i]*1000) for i in range(10)]
    script=(DESIGN/'weapons-pixel.js').read_text(encoding='utf8')
    blank=TEMP/'blank.png'
    Image.new('RGBA',(96,64)).save(blank)
    payload={'timeline':clip,'images':{},'poses':{},'nativeDurationsMs':durations}
    reports={}
    for kind,poses in [('axe',axe_poses()),('spear',spear_poses())]:
        if kind=='spear':
            poses=poses+[poses[0]]
        assert len(poses)==(16 if kind=='axe' else 10)
        durations=[100]+[round(times[i+1]*1000)-round(times[i]*1000) for i in range(10)]
        if kind=='axe':
            durations += [100,120,140,140,120,100]
            recovery_times=[clip['duration']]
            for duration in durations[11:]:
                recovery_times.append(recovery_times[-1]+duration/1000)
            payload['axeTimeline']={'keys':clip['keys']+[{'time':time,'frame':i+10} for i,time in enumerate(recovery_times[:-1])], 'duration':recovery_times[-1]}
        payload.setdefault('nativeDurationsByWeapon',{})[kind]=durations
        payload['poses'][kind]=poses
        frames=[]
        for index,p in enumerate([poses[0]]+poses):
            name='idle-0' if index==0 else f'attack-{index-1}'
            layers=[]
            for layer in ['body','vfx']:
                js=TEMP/f'{kind}-{name}-{layer}.js'
                js.write_text('var p='+json.dumps(p)+';\nvar patches='+json.dumps(patches)+';\nvar layer='+json.dumps(layer)+';\n'+script,encoding='utf8')
                png=TEMP/f'{kind}-{name}-{layer}.png'
                run(args.libresprite,blank,'--script',js,'--save-as',png)
                im=Image.open(png).convert('RGBA')
                assert im.size==(96,64) and set(im.getchannel('A').tobytes())<={0,255}
                layers.append(im)
            frames.append(layers)
        package=TEMP/f'{kind}.aseprite'
        native(package,frames,durations)
        destination=OUT/f'{kind}.aseprite'
        run(args.libresprite,package,'--save-as',destination)
        sheet=TEMP/f'{kind}-all.png'
        metadata=TEMP/f'{kind}.json'
        run(args.libresprite,destination,'--sheet',sheet,'--sheet-type','horizontal','--data',metadata,'--format','json-array','--list-layers','--list-tags')
        exported=Image.open(sheet).convert('RGBA')
        meta=json.loads(metadata.read_text())
        assert exported.size==(len(frames)*96,64)
        assert [f['duration'] for f in meta['frames']]==durations
        assert meta['meta']['frameTags'][1]['from']==1 and meta['meta']['frameTags'][1]['to']==len(frames)-1
        assert [tag['name'] for tag in meta['meta']['frameTags']]==['Idle','Attack']
        assert [layer['name'] for layer in meta['meta']['layers']]==['Body + weapon','VFX']
        body_sheet=TEMP/f'{kind}-body-roundtrip.png'
        run(args.libresprite,'--layer','Body + weapon',destination,'--sheet',body_sheet,'--sheet-type','horizontal')
        body_export=Image.open(body_sheet).convert('RGBA')
        fx_sheet=TEMP/f'{kind}-vfx-roundtrip.png'
        run(args.libresprite,'--layer','VFX',destination,'--sheet',fx_sheet,'--sheet-type','horizontal')
        fx_export=Image.open(fx_sheet).convert('RGBA')
        payload['images'][kind]=[]
        payload['images'][kind+'-body']=[]
        bounds=[]
        for index,(body,fx) in enumerate(frames):
            merged=Image.alpha_composite(body,fx)
            got=exported.crop((index*96,0,(index+1)*96,64))
            assert got.tobytes()==merged.tobytes(),(kind,index,'Native roundtrip')
            assert body_export.crop((index*96,0,(index+1)*96,64)).tobytes()==body.tobytes(),(kind,index,'Body layer roundtrip')
            assert fx_export.crop((index*96,0,(index+1)*96,64)).tobytes()==fx.tobytes(),(kind,index,'VFX layer roundtrip')
            bbox=got.getbbox()
            assert bbox and bbox[0]>0 and bbox[1]>0 and bbox[2]<96, (kind,index,bbox)
            feet=set(range(34,40))|set(range(51,57))
            ground={x for x in range(96) if body.getpixel((x,63))[3]}
            assert ground>=feet,(kind,index,'Planted feet')
            if kind=='axe':
                fx_bounds=fx.getbbox()
                assert bool(fx_bounds)==(index==5),(index,'Smear only on swing cel4')
                assert not fx_bounds or fx_bounds[3]<64,(index,'Smear cannot touch ground before contact')
            if kind=='axe' and 6<=index<=11:
                current=poses[index-1]
                blade=current['blade']
                assert max(q[1] for q in blade)==63
                contact=ground-feet
                assert contact and contact<={x for x in range(min(q[0] for q in blade),max(q[0] for q in blade)+1)},(index,'Grounded cutting edge')
                edge_contact={x for x,y in current['edge'] if y==63}
                assert edge_contact and all(body.getpixel((x,63))==(250,250,250,255) for x in edge_contact)
                assert current['tip']==poses[5]['tip'] and current['edge']==poses[5]['edge']
                assert current['butt'][1]==current['tip'][1] and not current['fx']
                assert not fx.getbbox(),(index,'Smear must end before contact')
            else:
                assert ground==feet,(kind,index,'Unexpected ground contact')
            name='idle-0' if index==0 else f'attack-{index-1}'
            got.save(OUT/f'{kind}-{name}.png')
            payload['images'][kind].append(data(got))
            payload['images'][kind+'-body'].append(data(body))
            bounds.append(list(bbox))
        assert frames[0][0].tobytes()==frames[-1][0].tobytes()
        assert all(abs(math.dist(p['butt'],p['tip'])-(32 if kind=='axe' else 58))<1 for p in poses)
        if kind=='axe':
            assert [i for i,p in enumerate(poses) if p['fx']]==[4]
            for p in poses:
                assert all(abs(math.dist(p['butt'],hand)-distance)<.8
                           for hand,distance in zip(p['hands'],[7,15]))
        reports[kind]={'nativeSha256':sha(destination),'frames':len(frames),'bounds':bounds,'durationsMs':durations,'nativeRoundTrip':True,'binaryAlpha':True,'plantedFeet':True}
        reports[kind]['groundContactFrames']=list(range(5,11)) if kind=='axe' else []
        if kind=='axe':
            reports[kind].update(cuttingEdgeContact=True,pinnedHeadFrames=list(range(5,11)),fixedGrips=True,smearFrames=[4],recoveryFrames=list(range(10,16)),idleReturnExact=True,previewTimeline=payload['axeTimeline'])
    payload['images']['sword']=[data(Image.open(DESIGN/'complete'/('idle-0.png' if i==0 else f'attack1-{i-1}.png'))) for i in range(11)]
    payload['images']['hero']=[data(Image.open(ROOT/'Assets/Art/male_hero/individual_sheets/male_hero-idle.png').crop((0,0,128,128)))]
    (OUT/'review.html').write_text((DESIGN/'weapons-preview.template.html').read_text(encoding='utf8').replace('/*PAYLOAD*/',json.dumps(payload)),encoding='utf8')
    assert all(sha(p)==digest for p,digest in guard.items()),'Input modified during export'
    report={'inputFilesUnchanged':len(guard),'inputs':{str(p.relative_to(ROOT)):digest for p,digest in guard.items() if p.parent==DESIGN/'cluster-milestone'},'exports':reports,'timing':clip}
    (OUT/'verification.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf8')
    (OUT/'poses.json').write_text(json.dumps(payload['poses'],indent=2)+'\n',encoding='utf8')
    assert len(list(OUT.glob('*.png')))==28
    print(f'PASS: 28 PNGs, layered axe17/spear11-frame natives, exact roundtrips; {len(guard)} input files unchanged.')


if __name__=='__main__':
    main()
