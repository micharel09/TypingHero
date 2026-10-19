"""Build review assets through LibreSprite; never writes runtime/source sheets."""
import argparse
from collections import Counter
import json
import math
from pathlib import Path

from PIL import Image

from build_preview import DESIGN, ROOT, data, timeline
from full_pack_poses import clips
import axe_reference
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
    if kind=='axe' and clip=='Idle':return [100]*9
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
    if 'referenceIdle' in p:
        return  # The extracted GIF owns its breathing deformation and landmarks.
    assert abs(math.dist(p['butt'],p['tip'])-(49 if kind=='axe' else 58))<1,(kind,clip,index,'shaft')
    for hand,distance in zip(p['hands'],p['grips']):
        if distance is not None:assert abs(math.dist(p['butt'],hand)-distance)<.85,(kind,clip,index,'grip')
    if p.get('sweep') and p['fx']:
        assert all(0<x<95 and 0<y<63 for rim in p['sweep'] for x,y in rim),(kind,clip,index,'sweep bounds')
    if kind!='axe' and clip in ['Idle','Attack1','Attack2','Hurt']:
        assert p['feet']==[[36,62],[53,62]],(kind,clip,index,'feet')


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--libresprite',required=True)
    parser.add_argument('--weapon',choices=['all','axe'],default='all')
    args=parser.parse_args()
    OUT.mkdir(exist_ok=True);TEMP.mkdir(parents=True,exist_ok=True)
    selected=['axe'] if args.weapon=='axe' else ['sword','axe','spear']
    folders=[ROOT/'Assets',DESIGN/'complete',DESIGN/'cluster-milestone',DESIGN/'originals',DESIGN/'weapon-milestone']
    guard={p:sha(p) for folder in folders for p in folder.rglob('*') if p.is_file()}
    if args.weapon=='axe':
        guard.update({p:sha(p) for kind in ['sword','spear'] for p in list((OUT/kind).rglob('*.png'))+list(OUT.glob(kind+'-*.aseprite'))})
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
    if args.weapon=='axe':
        previous=json.loads((OUT/'manifest.json').read_text(encoding='utf8'))
        for group in ['clips','poses']:
            payload[group]={key:value for key,value in previous[group].items() if not key.startswith('axe/')}
        for kind in ['sword','spear']:
            payload['provenance'][kind]=previous['provenance'][kind]
            for color in ['White','Yellow']:
                for clip in COUNTS:
                    key=f'{kind}/{color}/{clip}'
                    payload['images'][key]={variant:data(Image.open(OUT/kind/color/variant/(clip+'.png')).convert('RGBA')) for variant in ['Without_VFX','With_VFX']}
        reports=[r for r in json.loads((OUT/'verification.json').read_text())['natives'] if not r['native'].startswith('axe-')]
    reference=axe_reference.frames()
    axe_patches=dict(head=patch(reference[0],(40,18,51,31)),ribs=patch(reference[0],(41,31,51,43)),pelvis=patch(reference[0],(41,44,51,49)),weapon=axe_reference.weapon_pixels())
    payload['reference']={'images':[data(cel) for cel in reference],'durationsMs':[100]*9}
    payload['provenance']['axe']='Idle: exact native GIF RGB/alpha/order, 9 x 100ms. Other clips reconstructed with the source anatomy and weapon.'
    all_idle={}
    for kind in selected:
        for color in ['White','Yellow']:
            frames=[];times=[];tags=[]
            for clip,default_count in COUNTS.items():
                count=(16 if clip=='Attack1' else 9 if clip=='Idle' else default_count) if kind=='axe' else default_count
                timing=durations(kind,clip)
                assert len(timing)==count
                key=f'{kind}/{color}/{clip}'
                payload['clips'][key]=dict(count=count,durationsMs=timing,loop=clip in ['Idle','Walk'],
                                          contactCel=5 if clip=='Attack1' else 4 if clip=='Attack2' else None)
                clip_frames=[];landmarks=[]
                for i in range(count):
                    p=all_poses[kind][clip][i] if kind!='sword' else None
                    validate_pose(kind,clip,i,p)
                    if kind=='axe' and 'referenceIdle' in p:
                        body=reference[p['referenceIdle']].copy()
                        if color=='Yellow':
                            white=body
                            body=axe_reference.yellow(body)
                            offset=[0,0,1,2,2,1,0,0,0][p['referenceIdle']]
                            assert all(body.getpixel((x,y))==white.getpixel((x,y))
                                       for y in range(30+offset) for x in range(56,70)),(key,i,'Idle steel RGB')
                        fx=Image.new('RGBA',(96,64))
                    else:
                        body,fx=(sword_cel(clip,i,color) if kind=='sword' else
                                 custom_cel(args.libresprite,kind,clip,i,p,color,axe_patches if kind=='axe' else patches,helpers,renderer))
                    if kind=='axe' and 'referenceIdle' not in p:
                        material=axe_reference.posed_weapon(p,axe_patches['weapon'])
                        fingers={(hand[0]+dx,hand[1]+dy) for hand,grip in zip(p['hands'],p['grips'])
                                 if grip is not None for dx in [-1,0,1] for dy in [0,1]}
                        assert all(body.getpixel(q)==rgba for q,rgba in material.items() if q not in fingers),(key,i,'weapon RGB')
                        # Skull has its own rotated patch envelope, independent of the staff.
                        angle=math.radians(p['headAngle']);co,si=math.cos(angle),math.sin(angle)
                        skull={(q[0],q[1]) for q in axe_patches['head']}
                        for x,y in material:
                            dx,dy=x-p['head'][0]-5.5,y-p['head'][1]-6.5
                            sample=(math.floor(dx*co+dy*si+6),math.floor(-dx*si+dy*co+7))
                            assert sample not in skull,(key,i,'weapon crosses skull',x,y)
                    for im in [body,fx]:
                        assert im.size==(96,64) and set(im.getchannel('A').tobytes())<={0,255}
                    bounds=body.getbbox()
                    assert bounds,(key,i,'empty body')
                    # The original sword thrust deliberately reaches x95.
                    if kind!='sword':assert bounds[0]>0 and bounds[1]>0 and bounds[2]<96,(key,i,'body bounds')
                    if kind=='spear' and clip in ['Idle','Attack1','Attack2','Hurt']:
                        assert all(body.getpixel((x,63))[3]==255 for x in [34,35,36,37,38,39,51,52,53,54,55,56]),(key,i,'foot pixels')
                    clip_frames.append((body,fx))
                    landmarks.append(p if p else sword_landmarks(clip,i))
                if kind=='axe' and clip=='Attack1':
                    assert clip_frames[-1][0].tobytes()==all_idle[(kind,color)].tobytes()
                    for i in range(5,11):
                        p=landmarks[i]
                        assert p['edge']==landmarks[5]['edge'] and max(q[1] for q in p['edge'])==63
                        contact=axe_reference.posed_weapon(p,axe_patches['weapon'])
                        assert any(y==63 and rgba==(250,250,250,255) and clip_frames[i][0].getpixel((x,y))==rgba
                                   for (x,y),rgba in contact.items()),(key,i,'bright cutting edge contact')
                        assert not clip_frames[i][1].getbbox()
                    assert [i for i,(_,fx) in enumerate(clip_frames) if fx.getbbox()]==[4]
                if kind=='axe' and clip in ['Attack2','Hurt']:
                    assert clip_frames[-1][0].tobytes()==all_idle[(kind,color)].tobytes(),(key,'recovery')
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
                    if kind=='axe' and color=='White' and clip=='Idle':
                        assert exported.tobytes()==sheet(reference).tobytes(),(variant,'source Idle RGBA/order')
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
    for kind in selected:
        for clip in COUNTS:
            for variant in ['Without_VFX','With_VFX']:
                w=Image.open(OUT/kind/'White'/variant/(clip+'.png')).convert('RGBA')
                y=Image.open(OUT/kind/'Yellow'/variant/(clip+'.png')).convert('RGBA')
                assert w.getchannel('A').tobytes()==y.getchannel('A').tobytes(),(kind,clip,'yellow alpha')
                assert any(a!=b for a,b in zip(w.tobytes(),y.tobytes())),(kind,clip,'yellow palette')
                if kind=='spear':
                    for i,p in enumerate(all_poses[kind][clip]):
                        for x0,y0 in [p['butt'],p['tip']]+p['edge']:
                            point=(i*96+x0,y0)
                            assert w.getpixel(point)==y.getpixel(point),(kind,clip,i,'weapon material palette')
    assert len(list(OUT.rglob('*.png')))==72
    assert len(list(OUT.glob('*.aseprite')))==6
    assert all(sha(OUT/report['native'])==report['nativeSha256'] for report in reports),'Native hash changed'
    assert all(sha(p)==digest for p,digest in guard.items()),'Protected source changed during build'
    template=(DESIGN/'full-pack-preview.template.html').read_text(encoding='utf8')
    (OUT/'review.html').write_text(template.replace('/*PAYLOAD*/',json.dumps(payload)),encoding='utf8')
    (OUT/'manifest.json').write_text(json.dumps(dict(clips=payload['clips'],poses=payload['poses'],provenance=payload['provenance']),indent=2)+'\n',encoding='utf8')
    (OUT/'verification.json').write_text(json.dumps(dict(pngSheets=72,natives=reports,
        protectedFiles=len(guard),protectedHashes={str(p.relative_to(ROOT)):digest for p,digest in guard.items()},
        idleReferenceExact=True,idleReferenceDurationsMs=[100]*9,technical='PASS',
        motion='Axe White Idle matches the source GIF exactly. Other axe clips are authored; no source-motion match or gameplay verification claimed.'),indent=2)+'\n',encoding='utf8')
    print(f'PASS 72 sheets / 6 natives; {len(guard)} protected files unchanged.',flush=True)


if __name__=='__main__':main()
