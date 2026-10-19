"""Artifact acceptance checks, no Unity runtime or gameplay claims."""
from pathlib import Path
from PIL import Image
from html.parser import HTMLParser
from xml.etree import ElementTree
import json, struct, zipfile

root=Path(__file__).resolve().parents[1]
m=json.loads((root/'metadata.json').read_text())
count=0
for a in m['assets']:
    im=Image.open(root/a['png']).convert('RGBA')
    assert list(im.size)==a['size'],a['id']
    assert set(im.getchannel('A').get_flattened_data()) <= {0,255},a['id']
    assert all(px==(0,0,0,0) for px in im.get_flattened_data() if px[3]==0),a['id']
    ElementTree.parse(root/a['svg'])
    l,b,r,t=a['border_LBRT']; assert l+r<im.width and b+t<im.height
    atlas=Image.open(root/a['atlas']['file']).convert('RGBA')
    x,y,w,h=a['atlas']['rect_top_left']
    assert atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),a['id']
    count+=1
for n in m['screens']:
    a=Image.open(root/'screens/640x360'/f'{n}.png')
    b=Image.open(root/'screens/1280x720'/f'{n}.png')
    assert a.size==(640,360) and b.size==(1280,720)
    assert a.resize(b.size,Image.Resampling.NEAREST).tobytes()==b.tobytes(),n
fg=Image.open(root/'artwork/03-foreground.png').convert('RGBA')
assert fg.getchannel('A').getextrema()==(0,255)
assert fg.crop((0,0,640,220)).getbbox() is None
for p in (root/'artwork').glob('*.png'):
    if p.stem.endswith('original'): continue
    im=Image.open(p).convert('RGBA')
    assert all(px==(0,0,0,0) for px in im.get_flattened_data() if px[3]==0),p.name
with Image.open(root/'intro/intro-preview.gif') as gif:
    duration=0
    for i in range(gif.n_frames): gif.seek(i); duration+=gif.info.get('duration',0)
    assert duration==2400,duration
avi=(root/'intro/intro-preview.avi').read_bytes()
assert avi[:4]==b'RIFF' and avi[8:12]==b'AVI '
assert struct.unpack('<I',avi[4:8])[0]+8==len(avi)
class Links(HTMLParser):
    def handle_starttag(self,tag,attrs):
        for k,v in attrs:
            if k in ['href','src'] and v and not v.startswith(('#','http','data:')):
                assert (root/v).is_file(),v
Links().feed((root/'index.html').read_text(encoding='utf-8'))
with zipfile.ZipFile(root/'typinghero-ui-kit-v1.zip') as z:
    assert z.testzip() is None
    for p in root.rglob('*'):
        if p.is_file() and p.suffix!='.zip' and '__pycache__' not in p.parts:
            assert z.read(p.relative_to(root).as_posix())==p.read_bytes(),p.name
print(f'PASS: {count} PNG/SVG + atlas rects, binary alpha, transparent RGB, borders; {len(m["screens"])} exact 1x/2x screen pairs; GIF 2400ms; AVI; all HTML links; ZIP byte parity.')
