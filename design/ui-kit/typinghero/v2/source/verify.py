"""Artifact acceptance: exports, source pixel invariants, slicing, links and ZIP."""
from pathlib import Path
from PIL import Image
from html.parser import HTMLParser
from xml.etree import ElementTree
import json, zipfile, sys
sys.dont_write_bytecode=True
root=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(root/'source'))
import build
m=json.loads((root/'metadata.json').read_text())
atlas=Image.open(root/'sheets/ui-atlas.png').convert('RGBA')
assert m['logical_canvas']==[320,180]
for a in m['assets']:
    im=Image.open(root/a['png']).convert('RGBA'); assert list(im.size)==a['size']
    assert set(im.getchannel('A').get_flattened_data())<={0,255},a['id']
    assert all(px==(0,0,0,0) for px in im.get_flattened_data() if px[3]==0),a['id']
    ElementTree.parse(root/a['svg'])
    x,y,w,h=a['atlas']['rect_top_left']; assert atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes()
    l,b,r,t=a['border_LBRT']; assert l+r<w and b+t<h
    if l:
        out=build.stretch(im,w+23,h+17,l)
        for xx,yy,dx,dy in [(0,0,0,0),(w-l,0,out.width-l,0),(0,h-l,0,out.height-l),(w-l,h-l,out.width-l,out.height-l)]:
            assert im.crop((xx,yy,xx+l,yy+l)).tobytes()==out.crop((dx,dy,dx+l,dy+l)).tobytes(),a['id']
for n in m['screens']:
    im=Image.open(root/f'screens/320x180/{n}.png'); assert im.size==(320,180)
    for scale in [2,4]:
        out=Image.open(root/f'screens/{320*scale}x{180*scale}/{n}.png')
        assert im.resize(out.size,Image.Resampling.NEAREST).tobytes()==out.tobytes(),n
for name,path,rect in [('hero','male_hero/individual_sheets/male_hero-idle.png',(0,0,128,128)),('opponent','EndlessSkeletons/sword/Idle.png',(0,0,96,64)),('captain','EndlessSkeletons/sword-shield/Idle.png',(0,0,96,64))]:
    p=build.REPO/'Assets/Art'/path
    if p.exists():
        src=Image.open(p).convert('RGBA').crop(rect); src=src.crop(src.getbbox()); out=Image.open(root/f'artwork/{name}-existing.png')
        assert src.size==out.size
        assert all(a==b for a,b in zip(src.get_flattened_data(),out.get_flattened_data()) if a[3]),name
for t in json.loads((root/'source/text-layout.json').read_text()):
    if t['limit'] is not None: assert t['width']<=t['limit'],t['text']
with Image.open(root/'intro/intro-preview.gif') as gif:
    duration=0
    for i in range(gif.n_frames): gif.seek(i); duration+=gif.info.get('duration',0)
    assert duration==2400,duration
class Links(HTMLParser):
    def handle_starttag(self,tag,attrs):
        for k,v in attrs:
            if k in ['href','src'] and v and not v.startswith(('#','http','data:')): assert (root/v).is_file(),v
Links().feed((root/'index.html').read_text(encoding='utf8'))
with zipfile.ZipFile(root/'typinghero-ui-kit-v2.zip') as z:
    assert z.testzip() is None
    for p in root.rglob('*'):
        if p.is_file() and p.suffix!='.zip' and '__pycache__' not in p.parts: assert z.read(p.relative_to(root).as_posix())==p.read_bytes(),p
print(f'PASS {len(m["assets"])} assets: XML/alpha/atlas/fixed corners; {len(m["screens"])} screens x3 exact scales; source sprite pixels; text limits; 2400ms GIF; gallery links; ZIP parity.')
