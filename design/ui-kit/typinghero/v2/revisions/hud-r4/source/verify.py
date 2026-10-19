"""Pixel contracts, SVG, extrusion, fills, rank isolation and nearest-neighbor exports."""
import sys
sys.dont_write_bytecode=True
from build import *
import xml.etree.ElementTree as ET

meta=json.loads((ROOT/'metadata.json').read_text())
sheet=Image.open(ROOT/'sheets/ui-atlas.png').convert('RGBA')
rects=[]
for name,info in meta['assets'].items():
    a=load(name);w,h=a.size;x,y,ww,hh=info['atlas_xywh']
    assert [w,h]==info['size']==[ww,hh]
    assert set(a.getchannel('A').get_flattened_data())<={0,255}
    assert sheet.crop((x,y,x+w,y+h)).tobytes()==a.tobytes(),name
    for yy in range(-2,h+2):
        for xx in range(-2,w+2):
            assert sheet.getpixel((x+xx,y+yy))==a.getpixel((min(w-1,max(0,xx)),min(h-1,max(0,yy)))),(name,xx,yy)
    for xx,yy,ww,hh in rects: assert x+w+4<=xx or xx+ww+4<=x or y+h+4<=yy or yy+hh+4<=y
    rects.append((x,y,w,h))
    rebuilt=blank(w,h)
    for r in ET.parse(ROOT/info['svg']).getroot():
        rect(rebuilt,int(r.attrib['x']),int(r.attrib['y']),int(r.attrib['width']),int(r.attrib['height']),r.attrib['fill'])
    assert rebuilt.tobytes()==a.tobytes(),name
for name in meta['screens']:
    a=Image.open(ROOT/f'screens/320x180/{name}.png')
    for scale in [2,4]:
        b=Image.open(ROOT/f'screens/{320*scale}x{180*scale}/{name}.png')
        assert b.tobytes()==a.resize(b.size,Image.Resampling.NEAREST).tobytes()
for rank in ['normal','elite','boss']:
    for hp in [0,.01,.5,1]:
        for gold in [False,True]:
            for kind,w,fy,fx in [('hp',77,10,20),('combo',84,5,14),('stamina',84,5,14)]:
                a=blank(106,27);resource(a,0,3,kind,hp,gold,rank)
                fill=load(kind+('-gold' if kind=='combo' and gold else '')+'-fill')
                track=load('resource-track' if kind=='hp' else 'secondary-track')
                n=columns(hp,w);local_y=3 if kind=='hp' else 1
                assert sum(a.getpixel((fx+x,fy+3))==fill.getpixel((x,local_y)) for x in range(w))==n,(kind,hp)
                for x in range(n,w): assert a.getpixel((fx+x,fy+3))==track.getpixel((x,local_y)),(kind,hp,x)
            # Slayer changes no HP pixels, score, stamina or typing frame.
            normal=overlay(rank,False,True,hp,hp);slayer=overlay(rank,True,True,hp,hp)
            for box in [(0,8,106,32),(215,8,320,48),(111,3,209,25),(61,146,259,176)]:
                assert normal.crop(box).tobytes()==slayer.crop(box).tobytes(),(rank,box)
    hud(depth=True,rank=rank,stress=True,slayer=True)
assert load('elite-socket').getbbox()[1]==1
assert load('boss-socket').getbbox()[1]==0
badge=load('elite-badge');assert badge.crop((0,4,10,5)).getbbox() is None
for name,border in [('score-frame',[3,4,3,3]),('secondary-frame',[2]*4),('typing-frame',[4]*4)]:
    assert meta['assets'][name]['border_LBRT']==border
for n in ['far','mid','floor','foreground']:
    a=Image.open(ROOT/f'artwork/scene-{n}.png');assert a.size==(320,180)
    if n=='floor': assert a.getbbox()==(0,112,320,146)
    if n=='foreground': assert a.crop((18,0,302,180)).getbbox() is None
checks=json.loads((ROOT/'source/text-layout.json').read_text())+TEXT
for t in checks:
    if t['limit'] is not None: assert t['width']<=t['limit'],t
assert len(meta['screens'])==10
assert meta['hp']['columns_at_0_1_50_100']==[0,1,38,77]
assert meta['secondary']['columns_at_0_1_50_100']==[0,1,42,84]
print('PASS: 24 PNG/SVG; all atlas pixels + 2px extrusion/corners; 30 exports; all ranks/Slayer/fill boundary cases; independent frame borders; scene isolation; text bounds.')

f=ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'),16)
widths=sorted([(round(f.getlength(w)),w) for w in POOL],reverse=True)
assert len(POOL)==219
assert widths[0][0]==54
assert 3*widths[0][0]+16==178<=190
for word in WORDS:
    assert f.getbbox(word)[3]<=20
    assert f.getlength(word)<=190
assert meta['typing']['rect']==[61,146,198,30]
assert meta['typing']['transition_ms']==100
print('PASS r4: pool219, longest',widths[0], 'worst triple178/190; current stress fits; fixed geometry100ms.')
