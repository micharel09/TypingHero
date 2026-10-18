"""Build review assets through LibreSprite; never writes runtime/source sheets."""
import argparse
from collections import Counter
import json
import math
from pathlib import Path

from PIL import Image

from build_preview import DESIGN, ROOT, data, timeline
from full_pack_poses import clips
from prepare_weapons import native, patch, run, sha

OUT=DESIGN/'full-pack'
TEMP=Path('E:/CodexTemp/TypingHero/skeleton-hero-art/full-pack-build')
PACK=ROOT/'Assets/Art/Skeletons_Free_Pack/Skeleton_Sword'
COUNTS=dict(Idle=8,Walk=10,Attack1=10,Attack2=9,Hurt=5,Die=13)
COOL={(29,29,49):(58,42,61),(103,108,134):(105,121,158),
      (139,152,170):(160,172,197),(191,203,214):(192,213,220)}
YELLOW={(250,250,250):(252,247,215),(192,213,220):(235,199,117),
        (160,172,197):(177,130,79),(105,121,158):(163,98,56),(58,42,61):(36,30,23)}
ANIM_PATHS=dict(Idle='Assets/Characters/Enermy/skeleton_idle.anim',
                Attack1='Assets/Scripts/skeleton_attack_heavy.anim',
                Hurt='Assets/Scripts/skeleton_hit.anim',Die='Assets/Scripts/skeleton_die.anim')


def source(color,fx,clip):
    return Image.open(PACK/f'Skeleton_{color}'/f'Skeleton_{"With" if fx else "Without"}_VFX'/
                      f'Skeleton_01_{color}_{clip}.png').convert('RGBA')


def durations(kind,clip):
    if clip=='Walk':return [100]*10
    if clip=='Attack2':return [round((i+1)*1000/12)-round(i*1000/12) for i in range(9)]
    t=timeline(ANIM_PATHS[clip],clip)
    times=[k['time'] for k in t['keys']]+[t['duration']]
    result=[round(times[i+1]*1000)-round(times[i]*1000) for i in range(len(times)-1)]
    if kind=='axe' and clip=='Attack1':result += [100,120,140,140,120,100]
    return result


def sword_cel(clip,index,color):
    box=(index*96,0,(index+1)*96,64)
    plain=source('White',False,clip).crop(box)
    full=source('White',True,clip).crop(box)
    yellow=source('Yellow',False,clip).crop(box)
    existing=DESIGN/'complete'/f'{clip.lower()}-{index}.png'
    approved=Image.open(existing).convert('RGBA') if existing.exists() else full
    body,fx=Image.new('RGBA',(96,64)),Image.new('RGBA',(96,64))
    for y in range(64):
        for x in range(96):
            w=plain.getpixel((x,y));v=full.getpixel((x,y));gold=yellow.getpixel((x,y))
            painted=approved.getpixel((x,y))
            effect=w!=v
            rgba=w if effect else painted
            # Some baked source effects replace silhouette pixels (Attack1 cel5
            # removes one rib pixel). A normal additive VFX layer cannot erase
            # Body: retain the approved cel's transparent envelope in that case.
            if effect and not painted[3]:rgba=(0,0,0,0)
            if not existing.exists() or effect:
                rgba=(*COOL.get(rgba[:3],rgba[:3]),rgba[3])
            if color=='Yellow' and w!=gold and rgba[3]:
                rgba=(*YELLOW.get(rgba[:3],gold[:3]),rgba[3])
            body.putpixel((x,y),rgba)
            if effect and painted[3]:fx.putpixel((x,y),painted)
    if color=='White' and existing.exists():
        assert Image.alpha_composite(body,fx).tobytes()==approved.tobytes(),(clip,index,'approved sword')
    return body,fx


def sword_landmarks(clip,index):
    box=(index*96,0,(index+1)*96,64)
    white=source('White',False,clip).crop(box)
    gold=source('Yellow',False,clip).crop(box)
    material=[(x,y) for y in range(64) for x in range(96)
              if white.getpixel((x,y))[3] and white.getpixel((x,y))==gold.getpixel((x,y))
              and white.getpixel((x,y))[:3] in [(250,250,250),(255,255,255),(204,204,204),(112,39,7),(73,16,9)]]
    if not material:return None
    cx=sum(x for x,y in material)/len(material);cy=sum(y for x,y in material)/len(material)
    xx=sum((x-cx)**2 for x,y in material);yy=sum((y-cy)**2 for x,y in material)
    xy=sum((x-cx)*(y-cy) for x,y in material)
    angle=.5*math.atan2(2*xy,xx-yy)
    u=(math.cos(angle),math.sin(angle))
    points=sorted(material,key=lambda q:(q[0]-cx)*u[0]+(q[1]-cy)*u[1])
    wood=[q for q in material if white.getpixel(q)[:3] in [(112,39,7),(73,16,9)]]
    hand=[round(sum(q[j] for q in wood)/len(wood)) for j in range(2)] if wood else list(points[0])
    return dict(butt=points[0],tip=points[-1],hands=[hand],edge=[points[0],points[-1]],
                material=material,sourceMaterialOverlay=True)


def custom_cel(exe,kind,clip,index,p,color,patches,helpers,renderer):
    layers=[]
    for layer in ['body','vfx']:
        # White and Yellow share an identical VFX layer.
        stem=f'{kind}-{clip}-{index}-{color if layer=="body" else "shared"}-{layer}'
        png=TEMP/(stem+'.png')
        payload=dict(p=p,patches=patches,layer=layer,yellow=color=='Yellow')
        js='\n'.join('var '+key+'='+json.dumps(value)+';' for key,value in payload.items())
        # Reuse only byte-identical render inputs, including helper source.
        script=js+'\n'+helpers+'\n'+renderer
        script_path=TEMP/(stem+'.js')
        if not png.exists() or not script_path.exists() or script_path.read_text(encoding='utf8')!=script:
            script_path.write_text(script,encoding='utf8')
            try:
                run(exe,TEMP/'blank.png','--script',script_path,'--save-as',png)
            except (AssertionError, OSError):
                png.unlink(missing_ok=True)
                raise
        layers.append(Image.open(png).convert('RGBA'))
    return layers


def sheet(frames):
    image=Image.new('RGBA',(96*len(frames),64))
    for i,frame in enumerate(frames):image.paste(frame,(96*i,0))
    return image


def validate_pose(kind,clip,index,p):
    if not p:return
    assert abs(math.dist(p['butt'],p['tip'])-(32 if kind=='axe' else 58))<1,(kind,clip,index,'shaft')
    for hand,distance in zip(p['hands'],p['grips']):
        if distance is not None:assert abs(math.dist(p['butt'],hand)-distance)<.85,(kind,clip,index,'grip')
    if p.get('sweep') and p['fx']:
        assert all(0<x<95 and 0<y<63 for rim in p['sweep'] for x,y in rim),(kind,clip,index,'sweep bounds')
    if clip in ['Idle','Attack1','Attack2','Hurt']:
        assert p['feet']==[[36,62],[53,62]],(kind,clip,index,'feet')


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--libresprite',required=True)
    args=parser.parse_args()
    OUT.mkdir(exist_ok=True);TEMP.mkdir(parents=True,exist_ok=True)
    folders=[ROOT/'Assets',DESIGN/'complete',DESIGN/'cluster-milestone',DESIGN/'originals',DESIGN/'weapon-milestone']
    guard={p:sha(p) for folder in folders for p in folder.rglob('*') if p.is_file()}
    Image.new('RGBA',(96,64)).save(TEMP/'blank.png')
    approved=Image.open(DESIGN/'complete/idle-0.png').convert('RGBA')
    patches=dict(head=patch(approved,(39,18,50,31)),ribs=patch(approved,(40,31,50,43)),pelvis=patch(approved,(40,44,50,49)))
    helpers=(DESIGN/'weapons-pixel.js').read_text(encoding='utf8').split("if(layer==='vfx'){")[0]
    renderer=(DESIGN/'full-pack-pixel.js').read_text(encoding='utf8')
    all_poses={kind:clips(kind) for kind in ['axe','spear']}
    payload=dict(clips={},images={},poses={},provenance={
        'source':'https://monopixelart.itch.io/skeletons-pack',
        'sword':'Source pose/alpha; existing approved White cels preserved.',
        'axe':'Idle observed in overview GIF; other clips reconstructed to the design plan.',
        'spear':'Authored to the approved plan; not shown in the source GIF.'})
    reports=[]
    all_idle={}
    for kind in ['sword','axe','spear']:
        for color in ['White','Yellow']:
            frames=[];times=[];tags=[]
            for clip,default_count in COUNTS.items():
                count=16 if kind=='axe' and clip=='Attack1' else default_count
                timing=durations(kind,clip)
                assert len(timing)==count
                key=f'{kind}/{color}/{clip}'
                payload['clips'][key]=dict(count=count,durationsMs=timing,loop=clip in ['Idle','Walk'],
                                          contactCel=5 if clip=='Attack1' else 4 if clip=='Attack2' else None)
                clip_frames=[];landmarks=[]
                for i in range(count):
                    p=all_poses[kind][clip][i] if kind!='sword' else None
                    validate_pose(kind,clip,i,p)
                    body,fx=(sword_cel(clip,i,color) if kind=='sword' else
                             custom_cel(args.libresprite,kind,clip,i,p,color,patches,helpers,renderer))
                    for im in [body,fx]:
                        assert im.size==(96,64) and set(im.getchannel('A').tobytes())<={0,255}
                    bounds=body.getbbox()
                    assert bounds,(key,i,'empty body')
                    # The original sword thrust deliberately reaches x95.
                    if kind!='sword':assert bounds[0]>0 and bounds[1]>0 and bounds[2]<96,(key,i,'body bounds')
                    if kind!='sword' and clip in ['Idle','Attack1','Attack2','Hurt']:
                        assert all(body.getpixel((x,63))[3]==255 for x in [34,35,36,37,38,39,51,52,53,54,55,56]),(key,i,'foot pixels')
                    clip_frames.append((body,fx))
                    landmarks.append(p if p else sword_landmarks(clip,i))
                if kind=='axe' and clip=='Attack1':
                    assert clip_frames[-1][0].tobytes()==all_idle[(kind,color)].tobytes()
                    for i in range(5,11):
                        p=landmarks[i]
                        assert p['edge']==landmarks[5]['edge'] and max(q[1] for q in p['edge'])==63
                        assert all(clip_frames[i][0].getpixel((x,y))==(250,250,250,255)
                                   for x,y in p['edge'] if y==63),(key,i,'bright cutting edge contact')
                        assert not clip_frames[i][1].getbbox()
                    assert [i for i,(_,fx) in enumerate(clip_frames) if fx.getbbox()]==[4]
                if kind!='sword' and clip=='Walk':
                    assert all(any(foot[1]==62 for foot in p['feet']) for p in landmarks)
                    assert any(any(foot[1]<62 for foot in p['feet']) for p in landmarks)
                if clip=='Idle':all_idle[(kind,color)]=clip_frames[0][0]
                start=len(frames)
                tags.append((start,start+count-1,clip))
                frames.extend(clip_frames);times.extend(timing)
                payload['poses'][key]=landmarks
                payload['images'][key]={}
                for variant in ['Without_VFX','With_VFX']:
                    cells=[body if variant=='Without_VFX' else Image.alpha_composite(body,fx) for body,fx in clip_frames]
                    exported=sheet(cells)
                    path=OUT/kind/color/variant/(clip+'.png')
                    path.parent.mkdir(parents=True,exist_ok=True)
                    exported.save(path)
                    payload['images'][key][variant]=data(exported)
            package=TEMP/f'{kind}-{color}.aseprite'
            native(package,frames,times,tags)
            destination=OUT/f'{kind}-{color}.aseprite'
            run(args.libresprite,package,'--save-as',destination)
            meta=TEMP/f'{kind}-{color}.json'
            composite=TEMP/f'{kind}-{color}-composite.png'
            run(args.libresprite,destination,'--sheet',composite,'--sheet-type','horizontal',
                '--data',meta,'--format','json-array','--list-layers','--list-tags')
            metadata=json.loads(meta.read_text())
            assert [(t['from'],t['to'],t['name']) for t in metadata['meta']['frameTags']]==tags
            assert [f['duration'] for f in metadata['frames']]==times
            assert [l['name'] for l in metadata['meta']['layers']]==['Body + weapon','VFX']
            assert Image.open(composite).convert('RGBA').tobytes()==sheet([Image.alpha_composite(*f) for f in frames]).tobytes()
            for name,index in [('Body + weapon',0),('VFX',1)]:
                path=TEMP/f'{kind}-{color}-layer-{index}.png'
                run(args.libresprite,'--layer',name,destination,'--sheet',path,'--sheet-type','horizontal')
                assert Image.open(path).convert('RGBA').tobytes()==sheet([f[index] for f in frames]).tobytes()
            reports.append(dict(native=destination.name,frames=len(frames),tags=tags,durationsMs=times,
                                nativeSha256=sha(destination),compositeRoundtrip=True,layerRoundtrips=True,binaryAlpha=True))
            print(f'PASS {kind}/{color}: {len(frames)} cels, six tags, exact layer/composite roundtrip',flush=True)
    # Yellow changes bone color, never silhouette, alpha, steel/wood or VFX.
    for kind in ['sword','axe','spear']:
        for clip in COUNTS:
            for variant in ['Without_VFX','With_VFX']:
                w=Image.open(OUT/kind/'White'/variant/(clip+'.png')).convert('RGBA')
                y=Image.open(OUT/kind/'Yellow'/variant/(clip+'.png')).convert('RGBA')
                assert w.getchannel('A').tobytes()==y.getchannel('A').tobytes(),(kind,clip,'yellow alpha')
                assert any(a!=b for a,b in zip(w.tobytes(),y.tobytes())),(kind,clip,'yellow palette')
                if kind!='sword':
                    for i,p in enumerate(all_poses[kind][clip]):
                        for x0,y0 in [p['butt'],p['tip']]+p['edge']:
                            point=(i*96+x0,y0)
                            assert w.getpixel(point)==y.getpixel(point),(kind,clip,i,'weapon material palette')
    assert len(list(OUT.rglob('*.png')))==72
    assert len(list(OUT.glob('*.aseprite')))==6
    assert all(sha(p)==digest for p,digest in guard.items()),'Protected source changed during build'
    template=(DESIGN/'full-pack-preview.template.html').read_text(encoding='utf8')
    (OUT/'review.html').write_text(template.replace('/*PAYLOAD*/',json.dumps(payload)),encoding='utf8')
    (OUT/'manifest.json').write_text(json.dumps(dict(clips=payload['clips'],poses=payload['poses'],provenance=payload['provenance']),indent=2)+'\n',encoding='utf8')
    (OUT/'verification.json').write_text(json.dumps(dict(pngSheets=72,natives=reports,
        protectedFiles=len(guard),protectedHashes={str(p.relative_to(ROOT)):digest for p,digest in guard.items()},
        technical='PASS',motion='Reconstructed poses; not a premium-source pixel match or runtime verification.'),indent=2)+'\n',encoding='utf8')
    print(f'PASS 72 sheets / 6 natives; {len(guard)} protected files unchanged.',flush=True)


if __name__=='__main__':main()
