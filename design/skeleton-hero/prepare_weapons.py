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
    # Broad cheek, flared toe and hooked beard follow references/axe-blade-user.png.
    # Each silhouette, face, edge and socket is authored in cel coordinates.
    poses = [
        pose('axe',[43,31,43,44],[48,48],[64,20],[[57,40],[38,34]],[[38,54],[51,54]],
             [[62,19],[64,19],[64,22],[64,26],[66,26],[69,25],[69,27],[69,30],[66,33],[62,35],[60,36],[61,34],[61,31],[60,28],[59,26],[60,23]],
             [[64,26],[66,26],[69,26],[68,29],[65,32],[62,34],[61,34],[61,31],[60,28]],
             [[69,26],[68,29],[65,32],[61,35],[60,35]],[[63,20],[61,23],[62,24],[63,24],[65,20]]),
        pose('axe',[42,32,42,45],[48,40],[52,8],[[56,32],[55,28]],[[38,55],[51,55]],
             [[50,8],[52,7],[53,10],[54,13],[56,13],[59,11],[60,12],[60,15],[59,19],[56,23],[54,24],[54,22],[54,19],[52,17],[50,16],[49,13]],
             [[54,13],[56,13],[59,12],[59,15],[58,19],[55,22],[54,22],[54,19],[52,17]],
             [[59,12],[59,15],[58,19],[55,23],[54,23]],[[51,8],[51,12],[51,13],[52,12],[53,8]]),
        pose('axe',[41,32,41,45],[54,25],[29,5],[[56,23],[30,24]],[[38,55],[50,55]],
             [[28,7],[28,4],[31,5],[35,6],[35,4],[35,1],[36,1],[39,2],[42,5],[44,9],[44,12],[42,11],[39,10],[37,10],[35,11],[32,10]],
             [[35,6],[35,4],[36,1],[39,3],[41,6],[42,9],[42,11],[39,10],[37,10]],
             [[36,1],[39,3],[41,6],[43,10],[43,11]],[[28,6],[31,8],[33,8],[33,7],[30,4]]),
        pose('axe',[40,33,40,46],[53,26],[28,6],[[55,24],[30,25]],[[38,56],[50,56]],
             [[27,8],[27,5],[30,6],[34,7],[34,5],[34,2],[35,2],[38,3],[41,6],[43,10],[43,13],[41,12],[38,11],[36,11],[34,12],[31,11]],
             [[34,7],[34,5],[35,2],[38,4],[40,7],[41,10],[41,12],[38,11],[36,11]],
             [[35,2],[38,4],[40,7],[42,11],[42,12]],[[27,7],[30,9],[32,9],[32,8],[29,5]]),
        pose('axe',[44,32,43,45],[52,32],[65,3],[[57,28],[41,25]],[[39,55],[52,55]],
             [[63,2],[65,2],[65,5],[66,9],[68,9],[71,8],[71,9],[71,12],[68,15],[65,18],[62,19],[63,17],[63,14],[62,12],[61,10],[61,7]],
             [[66,9],[68,9],[70,9],[70,12],[67,15],[64,17],[63,17],[63,14],[62,12]],
             [[70,9],[70,12],[67,15],[64,18],[63,18]],[[64,3],[62,6],[63,8],[64,7],[66,3]]),
        pose('axe',[49,35,46,48],[57,35],[77,60],[[62,40],[53,45]],[[42,57],[55,57]],
             [[79,59],[78,61],[75,59],[72,58],[71,60],[70,63],[69,63],[66,61],[65,57],[64,53],[64,50],[65,52],[68,53],[71,54],[73,53],[75,55]],
             [[72,58],[71,60],[70,62],[67,60],[65,57],[65,53],[65,52],[68,53],[71,54]],
             [[70,62],[67,60],[65,57],[64,52],[65,51]],[[78,59],[75,56],[74,56],[74,58],[76,61]],True,
             [1,1,1,1,0,0,0,0,0,-1,-1,-1,-1]),
        pose('axe',[49,37,47,50],[58,33],[74,61],[[62,39],[54,45]],[[42,58],[55,58]],
             [[76,60],[74,62],[72,60],[69,58],[68,60],[67,63],[66,62],[63,60],[62,56],[62,52],[63,49],[64,51],[66,53],[69,54],[71,54],[73,56]],
             [[69,58],[68,60],[66,62],[64,60],[63,56],[63,52],[64,51],[66,53],[69,54]],
             [[66,62],[64,60],[63,56],[63,51],[63,50]],[[75,61],[73,57],[72,57],[71,58],[73,61]],False,
             [1,1,1,1,0,0,0,0,0,-1,-1,-1,-1]),
        pose('axe',[48,36,46,49],[58,33],[74,61],[[62,39],[54,45]],[[42,57],[55,58]],
             [[76,60],[74,62],[72,60],[69,58],[68,60],[67,63],[66,62],[63,60],[62,56],[62,52],[63,49],[64,51],[66,53],[69,54],[71,54],[73,56]],
             [[69,58],[68,60],[66,62],[64,60],[63,56],[63,52],[64,51],[66,53],[69,54]],
             [[66,62],[64,60],[63,56],[63,51],[63,50]],[[75,61],[73,57],[72,57],[71,58],[73,61]],False,
             [1,1,1,1,0,0,0,0,0,-1,-1,-1,-1])
    ]
    # Hold the compression at6/7, settle at8, then end grounded at9.
    return poses[:7]+[poses[6],poses[7],poses[7]]


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


def native(path, frames, durations):
    """Package two untransformed RGBA layers; LibreSprite then reads and resaves."""
    encoded=[]
    for i,(body,fx) in enumerate(frames):
        chunks=[]
        if i==0:
            for name in ['Body + weapon','VFX']:
                chunks.append(chunk(0x2004,struct.pack('<6HB3x',3,0,0,0,0,0,255)+string(name)))
            tags=struct.pack('<H8x',2)
            for start,end,name in [(0,0,'Idle'),(1,10,'Attack')]:
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
    guard_paths=list((ROOT/'Assets').rglob('*'))+list((DESIGN/'complete').glob('*'))+list((DESIGN/'cluster-milestone').glob('*'))
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
        assert len(poses)==10
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
        assert exported.size==(1056,64)
        assert [f['duration'] for f in meta['frames']]==durations
        assert [tag['name'] for tag in meta['meta']['frameTags']]==['Idle','Attack']
        assert [layer['name'] for layer in meta['meta']['layers']]==['Body + weapon','VFX']
        body_sheet=TEMP/f'{kind}-body-roundtrip.png'
        run(args.libresprite,'--layer','Body + weapon',destination,'--sheet',body_sheet,'--sheet-type','horizontal')
        body_export=Image.open(body_sheet).convert('RGBA')
        payload['images'][kind]=[]
        payload['images'][kind+'-body']=[]
        bounds=[]
        for index,(body,fx) in enumerate(frames):
            merged=Image.alpha_composite(body,fx)
            got=exported.crop((index*96,0,(index+1)*96,64))
            assert got.tobytes()==merged.tobytes(),(kind,index,'Native roundtrip')
            assert body_export.crop((index*96,0,(index+1)*96,64)).tobytes()==body.tobytes(),(kind,index,'Body layer roundtrip')
            bbox=got.getbbox()
            assert bbox and bbox[0]>0 and bbox[1]>0 and bbox[2]<96
            feet=set(range(34,40))|set(range(51,57))
            ground={x for x in range(96) if body.getpixel((x,63))[3]}
            assert ground>=feet,(kind,index,'Planted feet')
            if kind=='axe' and index>=6:
                blade=poses[index-1]['blade']
                assert max(q[1] for q in blade)==63
                contact=ground-feet
                assert contact and contact<={x for x in range(min(q[0] for q in blade),max(q[0] for q in blade)+1)},(index,'Grounded cutting edge')
            else:
                assert ground==feet,(kind,index,'Unexpected ground contact')
            name='idle-0' if index==0 else f'attack-{index-1}'
            got.save(OUT/f'{kind}-{name}.png')
            payload['images'][kind].append(data(got))
            payload['images'][kind+'-body'].append(data(body))
            bounds.append(list(bbox))
        if kind=='spear':
            assert frames[0][0].tobytes()==frames[-1][0].tobytes()
        else:
            assert frames[-1][0].tobytes()==frames[-2][0].tobytes()
            assert frames[-1][0].tobytes()!=frames[0][0].tobytes()
        assert all(abs(math.dist(p['butt'],p['tip'])-(32 if kind=='axe' else 58))<1 for p in poses)
        reports[kind]={'nativeSha256':sha(destination),'frames':11,'bounds':bounds,'durationsMs':durations,'nativeRoundTrip':True,'binaryAlpha':True,'plantedFeet':True}
        reports[kind]['groundContactFrames']=list(range(5,10)) if kind=='axe' else []
    payload['images']['sword']=[data(Image.open(DESIGN/'complete'/('idle-0.png' if i==0 else f'attack1-{i-1}.png'))) for i in range(11)]
    payload['images']['hero']=[data(Image.open(ROOT/'Assets/Art/male_hero/individual_sheets/male_hero-idle.png').crop((0,0,128,128)))]
    (OUT/'review.html').write_text((DESIGN/'weapons-preview.template.html').read_text(encoding='utf8').replace('/*PAYLOAD*/',json.dumps(payload)),encoding='utf8')
    assert all(sha(p)==digest for p,digest in guard.items()),'Input modified during export'
    report={'inputFilesUnchanged':len(guard),'inputs':{str(p.relative_to(ROOT)):digest for p,digest in guard.items() if p.parent==DESIGN/'cluster-milestone'},'exports':reports,'timing':clip}
    (OUT/'verification.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf8')
    (OUT/'poses.json').write_text(json.dumps(payload['poses'],indent=2)+'\n',encoding='utf8')
    assert len(list(OUT.glob('*.png')))==22
    print(f'PASS: 22 PNGs, two layered11-frame natives, exact roundtrips; {len(guard)} input files unchanged.')


if __name__=='__main__':
    main()
