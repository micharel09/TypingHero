"""Draw the approval milestone in LibreSprite; never write runtime assets."""
import argparse,json,re,subprocess,hashlib
from pathlib import Path
from PIL import Image
from prepare_samples import ROOT,DESIGN,TEMP,sha
# torso x, head y, elbow, grip, tip, slash; native coordinates.
POSES=[
 (43,21,[54,40],[58,43],[69,23],0),
 (42,22,[51,34],[47,31],[27,19],0),
 (40,23,[48,31],[39,30],[17,22],0),
 (40,23,[48,31],[38,30],[16,23],0),
 (43,22,[53,29],[51,24],[40,5],0),
 (51,21,[63,31],[67,30],[92,28],2),
 (52,22,[63,40],[69,44],[87,52],1),
 (48,23,[58,44],[61,48],[78,57],0),
 (45,22,[56,42],[60,44],[78,31],0),
 (43,21,[54,40],[58,43],[69,23],0)]
def main():
 parser=argparse.ArgumentParser();parser.add_argument('--libresprite',required=True);args=parser.parse_args()
 baseline=json.loads((DESIGN/'baseline.json').read_text())
 changed=[p for p,r in baseline.items() if sha(ROOT/p)!=r['sha256']]
 assert not changed, 'Baseline changed; investigate without resetting: '+str(changed)
 out=DESIGN/'attack-milestone';out.mkdir(exist_ok=True);TEMP.mkdir(parents=True,exist_ok=True)
 script=(DESIGN/'attack-sample.js').read_text()
 frames=[];reports=[]
 for i,values in enumerate([POSES[0]]+POSES):
  name='idle-0' if i==0 else 'attack-'+str(i-1)
  variables=dict(zip(['bodyX','headY','elbow','grip','tip','slash'],values))
  source=TEMP/(name+'.png');Image.new('RGBA',(96,64)).save(source)
  js=TEMP/(name+'.js');js.write_text(''.join('var '+k+'='+json.dumps(v)+';\n' for k,v in variables.items())+script)
  native=out/(name+'.aseprite');png=out/(name+'.png')
  result=subprocess.run([args.libresprite,'-b',str(source),'--script',str(js),'--save-as',str(native)],capture_output=True,text=True,check=True)
  assert not re.search(r'(?:Type|Reference|Syntax|Range)?Error:',result.stdout+result.stderr),result.stdout+result.stderr
  subprocess.run([args.libresprite,'-b',str(native),'--save-as',str(png)],capture_output=True,check=True)
  image=Image.open(png).convert('RGBA');assert image.size==(96,64)
  assert set(image.getchannel('A').tobytes())=={0,255}
  assert image.getbbox() and image.getbbox()[0]>0 and image.getbbox()[2]<96 and image.getbbox()[1]>0
  assert [x for x in range(96) if image.getpixel((x,63))[3]]==[37,38,39,40,50,51,52,53]
  # Every visible pixel must belong to the body/sword or intentional slash cluster.
  pixels={(x,y) for y in range(64) for x in range(96) if image.getpixel((x,y))[3]};sizes=[]
  while pixels:
   pending=[pixels.pop()];size=0
   while pending:
    x,y=pending.pop();size+=1
    for dx in [-1,0,1]:
     for dy in [-1,0,1]:
      q=x+dx,y+dy
      if q in pixels:pixels.remove(q);pending.append(q)
   sizes.append(size)
  assert min(sizes)>3,(name,sizes)
  reports.append(dict(name=name,sha256=sha(png),components=sizes,**variables))
  if i:frames.append(image)
 sheet=Image.new('RGBA',(960,64))
 for i,frame in enumerate(frames):sheet.paste(frame,(i*96,0))
 # Assemble the sheet using LibreSprite as well (PNG assembly is an intermediate only).
 intermediate=TEMP/'attack-sheet.png';sheet.save(intermediate)
 subprocess.run([args.libresprite,'-b',str(intermediate),'--save-as',str(out/'Attack1.aseprite')],check=True,capture_output=True)
 subprocess.run([args.libresprite,'-b',str(out/'Attack1.aseprite'),'--save-as',str(out/'Attack1.png')],check=True,capture_output=True)
 assert Image.open(out/'Attack1.png').convert('RGBA').tobytes()==sheet.tobytes()
 (out/'landmarks.json').write_text(json.dumps(reports,indent=2)+'\n')
 print('PASS: 11 frames; binary alpha; bounds; planted feet; no isolated pixels; baseline hashes unchanged.')
if __name__=='__main__':main()
