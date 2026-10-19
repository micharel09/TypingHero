"""Original-based LibreSprite art milestone. Never writes runtime assets."""
import argparse,json,re,subprocess
from PIL import Image
from prepare_samples import ROOT,DESIGN,TEMP,ART,sha,inside

# Conservative envelopes retain blade, grip and neighbouring overlap pixels.
WEAPONS=[[(54,48),(54,39),(66,21),(70,21),(63,42),(63,48)],
[(42,17),(46,17),(52,35),(57,43),(51,46),(47,36)],
[(18,21),(24,21),(51,34),(51,38),(35,38),(35,33)],
[(18,26),(24,26),(45,32),(45,39),(34,39),(34,35)],
[(18,26),(24,26),(46,32),(46,40),(34,40),(34,35)],
[(63,24),(96,22),(96,33),(63,35)],
[(62,22),(96,18),(96,31),(62,35)],
[(60,25),(96,22),(96,30),(60,34)],
[(53,44),(53,38),(75,20),(79,20),(62,42),(62,45)],
[(54,48),(54,39),(66,21),(70,21),(63,42),(63,48)]]
ANCHORS=[([57,43],[67,23]),([57,42],[57,36]),([38,34],[21,24]),
([38,35],[20,28]),([39,36],[20,28]),([67,30],[94,26]),
([66,29],[93,21]),([63,31],[91,25]),([57,39],[76,24]),([57,43],[67,23])]

def run(command):
    p=subprocess.run(command,check=True,capture_output=True,text=True)
    assert not re.search(r'(?:Type|Reference|Syntax|Range)?Error:',p.stdout+p.stderr),p.stdout+p.stderr

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--libresprite',required=True)
    args=parser.parse_args()
    baseline=json.loads((DESIGN/'baseline.json').read_text())
    before={p:sha(ROOT/p) for p in baseline}
    drift=[p for p,r in baseline.items() if before[p]!=r['sha256']]
    allowed=['Assets/Scripts/UI/TutorialOverlay.cs','Assets/Scripts/VFX_SFX/ScreenFx/SlayerScreenFX.cs']
    assert all(p.replace('\\','/') in allowed for p in drift),'Investigate new baseline drift: '+str(drift)
    out=DESIGN/'cluster-milestone';out.mkdir(exist_ok=True);TEMP.mkdir(parents=True,exist_ok=True)
    reports=[];frames=[]
    script=(DESIGN/'original-pixel-edit.js').read_text()
    for ordinal in range(11):
        sheet,index=('Idle',0) if ordinal==0 else ('Attack1',ordinal-1)
        name='idle-0' if ordinal==0 else f'attack-{index}'
        path=DESIGN/f'originals/Skeleton_01_White_{sheet}.png'
        key=str((ART/f'Skeleton_With_VFX/{path.name}').relative_to(ROOT))
        assert sha(path)==baseline[key]['sha256']
        original=Image.open(path).convert('RGBA').crop((index*96,0,(index+1)*96,64))
        plain=Image.open(ART/f'Skeleton_Without_VFX/{path.name}').convert('RGBA').crop((index*96,0,(index+1)*96,64))
        protected=[];mask=Image.new('L',(96,64))
        for y in range(64):
            for x in range(96):
                p=original.getpixel((x,y))
                material=p[:3] in [(255,255,255),(204,204,204),(73,16,9),(112,39,7)]
                if material or inside(x+.5,y+.5,WEAPONS[index]) or p!=plain.getpixel((x,y)):
                    protected.append([x,y,*p]);mask.putpixel((x,y),255)
        source=TEMP/(name+'-original.png');original.save(source)
        js=TEMP/(name+'-preserved.js');js.write_text('var pose='+json.dumps(name)+';\nvar protectedPixels='+json.dumps(protected)+';\n'+script)
        native,png=out/(name+'.aseprite'),out/(name+'.png')
        run([args.libresprite,'-b',str(source),'--script',str(js),'--save-as',str(native)])
        run([args.libresprite,'-b',str(native),'--save-as',str(png)])
        edited=Image.open(png).convert('RGBA')
        roundtrip=TEMP/(name+'-roundtrip.png')
        run([args.libresprite,'-b',str(native),'--save-as',str(roundtrip)])
        assert Image.open(roundtrip).convert('RGBA').tobytes()==edited.tobytes()
        assert edited.size==(96,64)
        assert edited.getchannel('A').tobytes()==original.getchannel('A').tobytes()
        assert set(edited.getchannel('A').tobytes())=={0,255}
        assert all(edited.getpixel((p[0],p[1]))==tuple(p[2:]) for p in protected)
        changed=sum(a!=b for a,b in zip(original.get_flattened_data(),edited.get_flattened_data()))
        assert changed>0
        palette=Image.open(DESIGN/'preserved-original-milestone'/(name+'.png')).convert('RGBA')
        # A palette-only remap cannot change the partition of an original color.
        # Require at least one source color to split into multiple painted tones.
        partitions={}
        for y in range(64):
            for x in range(96):
                a,b=original.getpixel((x,y)),edited.getpixel((x,y))
                if a[3] and not mask.getpixel((x,y)):
                    partitions.setdefault(a,set()).add(b)
        assert any(len(v)>1 for v in partitions.values()),'Palette-only edit'
        assert edited.tobytes()!=palette.tobytes()
        diff=Image.new('RGBA',(96,64))
        for y in range(64):
            for x in range(96):
                if edited.getpixel((x,y))!=palette.getpixel((x,y)):
                    diff.putpixel((x,y),edited.getpixel((x,y)))
        diff.save(out/(name+'-diff.png'))
        mask.save(out/(name+'-protected.png'))
        grip,tip=ANCHORS[index]
        reports.append(dict(name=name,sha256=sha(png),grip=grip,tip=tip,changedPixels=changed,
                            protectedPixels=len(protected),alphaIdentical=True,protectedIdentical=True,
                            nativeRoundTrip=True,sourceColorPartitionChanged=True))
        if ordinal:frames.append(edited)
    sheet=Image.new('RGBA',(960,64))
    for i,frame in enumerate(frames):sheet.paste(frame,(96*i,0))
    source=TEMP/'preserved-attack-sheet.png';sheet.save(source)
    run([args.libresprite,'-b',str(source),'--save-as',str(out/'Attack1.aseprite')])
    run([args.libresprite,'-b',str(out/'Attack1.aseprite'),'--save-as',str(out/'Attack1.png')])
    assert Image.open(out/'Attack1.png').convert('RGBA').tobytes()==sheet.tobytes()
    assert frames[0].tobytes()==frames[9].tobytes(),'Recovery must match original identical pose'
    assert Image.open(out/'idle-0.png').convert('RGBA').tobytes()==frames[0].tobytes()
    assert all(sha(ROOT/p)==digest for p,digest in before.items()),'Protected input changed during export'
    (out/'landmarks.json').write_text(json.dumps(reports,indent=2)+'\n')
    print(json.dumps(dict(frames=11,checks='PASS: exact alpha, protected RGBA, native round-trip, session hashes',
                         preexistingBaselineDrift=drift,editedPixels=[r['changedPixels'] for r in reports]),indent=2))
if __name__=='__main__':main()
