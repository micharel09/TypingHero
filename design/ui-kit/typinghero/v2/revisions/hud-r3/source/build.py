"""Offline HUD r3 review generator. Python/Pillow only; never writes r1 or Unity."""
import sys
sys.dont_write_bytecode = True
from primitives import *



NORMAL=['resolve','honor','courage','guard','strike']
STRESS=['a','to','counterrevolutionary','i','resolve']
WORDS=list(dict.fromkeys(NORMAL+STRESS+['gy']))

def glyph_layer(word,progress=0,wrong=None,size=16,muted=False):
    a=blank(158,20 if size==16 else 12)
    f=ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'),size)
    left=round((158-f.getlength(word))/2)
    assert f.getlength(word)<=150
    for i,ch in enumerate(word):
        text(a,left+round(f.getlength(word[:i])),0,ch,size,'muted' if muted else 'error' if i==wrong else 'jade' if i<progress else 'ivory')
    return a

def word_layer(word='resolve',progress=0,wrong=None,size=16,next_word='honor'):
    a=blank(158,36)
    paste(a,glyph_layer(word,progress,wrong,16),0,4)
    # Preview uses its own full-opacity muted pixels, never layer opacity.
    paste(a,glyph_layer(next_word,size=12,muted=True),0,20)
    return a


def assets():
    for folder in ['png', 'svg', 'sheets', 'screens/320x180', 'screens/640x360', 'screens/1280x720']:
        (ROOT/folder).mkdir(parents=True, exist_ok=True)
    bar_assets()  # HP remains silver in every state.
    track = blank(77, 6)
    for y, c in enumerate(['shadow', 'coal', 'coal', 'coal', 'coal', 'steel']):
        rect(track, 0, y, 77, 1, c)
    save('resource-track', track, 'Fixed HP track')
    hp = blank(77, 6)
    for y, c in enumerate(['#f17c81', '#d45165', '#b43852', '#b43852', '#74243e', '#542039']):
        rect(hp, 0, y, 77, 1, c)
    save('hp-fill', hp, 'Horizontal continuous crop')
    basis = json.loads((ROOT/'source/semantic-basis.json').read_text())
    for name in ['icon-hp', 'captain-badge', 'icon-pause', 'keycap-space']:
        info = basis[name]
        a = blank(*info['size'])
        a.putdata([tuple(px) for px in info['pixels']])
        if name == 'icon-hp':
            for y in range(a.height):
                for x in range(a.width):
                    if a.getpixel((x, y))[3]:
                        a.putpixel((x, y), (241,124,129,255) if y<2 else (212,81,101,255) if y<a.height-2 else (116,36,62,255))
        save(name, a, 'Semantic artwork snapshot from r1/v2')
    # Explicit 7x7 silhouettes: sword (combo), lightning (stamina).
    for kind, rows in [('combo', ['0000011','0000111','0001110','1011100','0111000','1110100','1000000']),
                       ('stamina', ['0001100','0011000','0110000','1111110','0001100','0011000','0110000'])]:
        a = blank(7,7)
        for y,row in enumerate(rows):
            for x,bit in enumerate(row):
                if bit=='1': rect(a,x,y,1,1,'light' if kind=='combo' else 'jade')
        save('small-icon-'+kind,a,'7x7 native silhouette')
    for gold in [False, True]:
        suffix = '-gold' if gold else ''
        a = blank(102,13)
        step(a,10,1,92,11,2,'gold' if gold else 'steel')
        step(a,11,2,90,9,1,'inner')
        save('secondary-frame'+suffix,a,'No corner reinforcement or glints; fixed endcap',[2,2,2,2])
        a = blank(13,13)
        for x,y,w,c in [(0,0,13,'gold' if gold else 'steel'),(1,1,11,'bevel'),(2,2,9,'shadow')]:
            ImageDraw.Draw(a).ellipse((x,y,x+w-1,y+w-1),fill=P[c])
        save('secondary-socket'+suffix,a,'Fixed 13px socket')
    a=blank(84,4);rect(a,0,0,84,4,'coal');rect(a,0,0,84,1,'shadow')
    save('secondary-track',a,'Secondary track at [14,4,84,4]')
    for kind,colors in [('combo',['#afa1b3','#827789','#77677f','#494153']),
                        ('combo-gold',['#edda9e','#c2a263','#a3844d','#705338']),
                        ('stamina',['#a3c5ba','#71a89f','#588b8c','#39626b'])]:
        a=blank(84,4)
        for y,c in enumerate(colors): rect(a,0,y,84,1,c)
        save(kind+'-fill',a,'Continuous secondary fill')
    a=blank(98,22)
    for x,y,w,h,c in [(0,2,98,20,'shadow'),(0,0,98,20,'steel'),(1,1,96,18,'inner'),(3,3,92,14,'deep')]:
        step(a,x,y,w,h,2,c)
    save('score-frame',a,'Thin dark score board; no label or bright corner studs',[3,4,3,3])
    for error in [False,True]:
        a=blank(158,36)
        for inset,color in enumerate(['shadow','error' if error else 'rim','bevel','inner']):
            step(a,inset,inset,158-inset*2,36-inset*2,1,color)
        rect(a,4,4,150,28,'deep')
        save('typing-frame'+('-error' if error else ''),a,'Compact silver typing frame',[4]*4)
    for rank,rise,peaks in [('elite',2,[5,13]),('boss',3,[3,9,15])]:
        a=blank(21,24);paste(a,load('resource-socket'),0,3)
        for x in peaks:
            rect(a,x,3-rise,3,rise+2,'rim');rect(a,x+1,4-rise,1,rise+1,'bevel')
        save(rank+'-socket',a,'Socket anchor [0,3]; rank rises 2/3px')
    a=blank(10,10)
    # Two filled V bands separated by one full dark row (y4).
    for y0 in [0,5]:
        for x in range(8):
            y=y0+min(x,7-x)//2
            rect(a,1+x,y,1,2,'ivory')
    save('elite-badge',a,'Two solid separated chevrons')


def columns(value,width):
    assert 0 <= value <= 1
    return 0 if value == 0 else max(1,round(width*value))


def resource(im,x,y,kind,value,gold=False,rank='normal'):
    if kind!='hp':
        suffix='-gold' if gold and kind=='combo' else ''
        paste(im,load('secondary-frame'+suffix),x,y)
        paste(im,load('secondary-track'),x+14,y+4)
        n=columns(value,84)
        if n: paste(im,load(kind+suffix+'-fill').crop((0,0,n,4)),x+14,y+4)
        paste(im,load('secondary-socket'+suffix),x,y)
        paste(im,load('small-icon-'+kind),x+3,y+3)
        return
    paste(im,load('resource-frame'),x,y)
    paste(im,load('resource-track'),x+20,y+7)
    n=columns(value,77)
    if n: paste(im,load('hp-fill').crop((0,0,n,6)),x+20,y+7)
    paste(im,load('resource-socket' if rank=='normal' else rank+'-socket'),x,y if rank=='normal' else y-3)
    icon=load('icon-hp' if rank=='normal' else 'elite-badge' if rank=='elite' else 'captain-badge')
    paste(im,icon,x+(20-icon.width)//2,y+(19-icon.height)//2)


def scene_assets():
    bg=Image.open(ROOT/'artwork/01-backdrop.png').convert('RGBA')
    far=bg.copy()
    # Existing walls/columns keep their source pixels, extracted as a mid layer.
    mid=blank(320,180)
    for left,right in [(0,45),(73,89),(231,248),(277,320)]:
        paste(mid,bg.crop((left,0,right,112)),left,0)
        rect(far,left,0,right-left,112,'deep')
    floor=blank(320,180)
    for y in range(112,146):
        rect(floor,0,y,320,1,'#282334' if y<125 else '#2c2639' if y<138 else '#302a3c')
    d=ImageDraw.Draw(floor)
    for end in range(-480,801,64):
        # Clip rays to floor band; do not draw above y112.
        start=160+(end-160)*32/65
        d.line((round(start),112,end,145),fill='#393144')
    for y in [116,123,133,145]: rect(floor,0,y,320,1,'#393144')
    near=blank(320,180)
    original=Image.open(ROOT/'artwork/03-foreground.png').convert('RGBA')
    for x in [0,302]: paste(near,original.crop((x,0,x+18,180)),x,0)
    for name,a in [('far',far),('mid',mid),('floor',floor),('foreground',near)]:
        # All scene layers are delivered separately from the UI atlas.
        a.save(ROOT/'artwork'/f'scene-{name}.png')


def scene(depth=False,boss=False):
    if depth:
        im=blank(320,180)
        for name in ['far','mid','floor']: paste(im,Image.open(ROOT/'artwork'/f'scene-{name}.png').convert('RGBA'),0,0)
    else: im=Image.open(ROOT/'artwork/01-backdrop.png').convert('RGBA')
    actor(im,90);actor(im,203,True,boss)
    paste(im,Image.open(ROOT/'artwork'/('scene-foreground.png' if depth else '03-foreground.png')).convert('RGBA'),0,0)
    return im


def overlay(rank='normal',slayer=False,stress=False,hp=.85,enemy_hp=None,typing=True,feedback='normal'):
    im=blank(320,180)
    text(im,5,1,'WWWWWWWWWW' if stress else 'HERO',8,limit=58)
    text(im,73,1,f'{round(hp*100)}/100',8,'error' if hp<=.01 else 'ivory',limit=31)
    resource(im,3,11,'hp',hp)
    resource(im,3,34,'combo',.99 if stress else .72,slayer)
    text(im,217,1,'CAPTAIN' if rank=='boss' else 'ELITE SWORDSMAN' if rank=='elite' else 'SWORDSMAN',8,limit=64)
    eh=enemy_hp if enemy_hp is not None else 200/550 if rank=='boss' else .8
    text(im,281,1,f'{round(eh*(550 if rank=="boss" else 100))}/{550 if rank=="boss" else 100}',8,limit=36)
    resource(im,215,11,'hp',eh,rank=rank)
    resource(im,215,34,'stamina',.33)
    paste(im,load('score-frame'),111,3)
    text(im,160,5,'2,147,483,647' if stress else '012,840',12,'ivory',True,88)
    text(im,160,26,'999 COMBO / x16' if stress else '24 COMBO / x16' if slayer else '24 COMBO / x4',8,'light' if slayer else 'muted',True,98)
    text(im,160,38,'SLAYER / x16' if slayer else 'ROUND 12 / NORMAL',8,'light' if slayer else 'muted',True,102)
    if typing:
        paste(im,load('typing-frame-error' if feedback=='error' else 'typing-frame'),81,140)
        paste(im,word_layer('counterrevolutionary' if stress else 'resolve',3 if feedback=='correct' else 0,3 if feedback=='error' else None,16, 'i' if stress else 'honor'),81,140)
    text(im,47,146,'PARRY',10,'light',True)
    paste(im,load('keycap-space'),35,160)
    text(im,272,146,'00:42',10,center=True);paste(im,load('icon-pause'),267,163)
    return im


def hud(depth=False,**kwargs):
    im=scene(depth,kwargs.get('rank')=='boss');paste(im,overlay(**kwargs),0,0);return im


def atlas():
    sheet=blank(512,512);x=y=3;row=0
    for name,info in ASSETS.items():
        a=load(name)
        if x+a.width+3>512: x,y,row=3,y+row+6,0
        assert y+a.height+3<=512
        # Clamp each padded pixel, including all four extrusion corners.
        pad=blank(a.width+4,a.height+4)
        for yy in range(pad.height):
            for xx in range(pad.width): pad.putpixel((xx,yy),a.getpixel((min(a.width-1,max(0,xx-2)),min(a.height-1,max(0,yy-2)))))
        paste(sheet,pad,x-2,y-2)
        info['atlas_xywh']=[x,y,a.width,a.height];info['anchor']='top-left; fixed source pixels'
        x+=a.width+6;row=max(row,a.height)
    sheet.save(ROOT/'sheets/ui-atlas.png')


def build():
    assets();scene_assets()
    screens={'hud-normal':{},'hud-elite':dict(rank='elite'),'hud-boss':dict(rank='boss'),
             'hud-slayer':dict(rank='boss',slayer=True),'hud-stress-low-hp':dict(rank='boss',slayer=True,stress=True,hp=.01),
             'hud-correct':dict(feedback='correct'),'hud-error':dict(feedback='error'),
             'depth-normal':dict(depth=True),'depth-elite':dict(depth=True,rank='elite'),'depth-boss':dict(depth=True,rank='boss')}
    contact=Image.new('RGBA',(960,800),P['coal'])
    for i,(name,params) in enumerate(screens.items()):
        a=hud(**params)
        for scale in [1,2,4]: a.resize((320*scale,180*scale),Image.Resampling.NEAREST).save(ROOT/f'screens/{320*scale}x{180*scale}/{name}.png')
        paste(contact,a,i%3*320,i//3*200+20);text(contact,i%3*320+4,i//3*200,name,12)
    contact.save(ROOT/'contact-sheet.png')
    parts=Image.new('RGBA',(640,270),P['coal'])
    for row,rank in enumerate(['normal','elite','boss']):
        text(parts,4,row*90,rank.upper(),10)
        for col,hp in enumerate([0,.01,.5,1]):
            demo=blank(106,30);resource(demo,0,3,'hp',hp,rank=rank)
            paste(parts,demo,4+col*158,row*90+14)
            for gold in [False,True]:
                a=blank(106,13);resource(a,0,0,'combo',hp,gold)
                paste(parts,a,4+col*158,row*90+47+gold*17)
    parts.save(ROOT/'component-sheet.png')
    parts.convert('L').save(ROOT/'component-sheet-grayscale.png')
    # Static HUD overlays are lossless generated pixels; browser composes them above scene.
    for rank in ['normal','elite','boss']:
        for slayer in [False,True]:
            for stress in [False,True]:
                for percent in [0,1,50,100]:
                    overlay(rank,slayer,stress,percent/100,percent/100,False).save(ROOT/'artwork'/f'hud-{rank}-{int(slayer)}-{int(stress)}-{percent}.png')
    for word in WORDS:
        for size in [12,16]:
            for i in range(len(word)+1): glyph_layer(word,i,None,size).save(ROOT/'png'/f'word-{word}-{size}-progress-{i}.png')
            if size==16:
                for i in range(len(word)): glyph_layer(word,0,i,size).save(ROOT/'png'/f'word-{word}-{size}-wrong-{i}.png')
    for word in WORDS: glyph_layer(word,size=12,muted=True).save(ROOT/'png'/f'preview-{word}.png')
    atlas()
    metadata=dict(revision='hud-r3',status='draft review',canvas=[320,180],filter='Point',wrap='Clamp',mipmaps=False,scales=[1,2,4],palette=P,assets=ASSETS,
        atlas=dict(size=[512,512],origin='top-left',padding=6,extrusion=2),
        hp=dict(rects=[[3,11,102,21],[215,11,102,21]],fill_xywh=[20,7,77,6],columns_at_0_1_50_100=[0,1,38,77]),
        secondary=dict(rects=[[3,34,102,13],[215,34,102,13]],fill_xywh=[14,4,84,4],socket=[13,13],columns_at_0_1_50_100=[0,1,42,84]),
        score=dict(rect=[111,3,98,22],font='VT323',font_px=12,combo_y=28,status_y=40),
        typing=dict(rect=[81,140,158,36],anchor=[81,140],error_ms=180,shake_ms=0,max_offset=0,current_font_px=16,preview_font_px=12,current_draw_y=144,preview_draw_y=160,normal_words=NORMAL,stress_words=STRESS,transition_ms=0,skip_nonletters=True,reset_on_error=True,restart_on_repeated_error=True,reduced_motion='system OR checkbox: color only; stop camera'),
        scene=dict(layers=['far','mid','floor','foreground'],files={n:f'artwork/scene-{n}.png' for n in ['far','mid','floor','foreground']},layer_rect=[0,0,320,180],layer_anchor=[0,0],actors={n:dict(file=f'artwork/{n}-existing.png',size=list(Image.open(ROOT/f'artwork/{n}-existing.png').size),anchor='bottom-left',feet=[90 if n=='hero' else 203,138]) for n in ['hero','opponent','captain']},actor_feet_y=138,floor_y=[112,145],vanishing_point=[160,80],camera_ms=4000,amplitudes=dict(far=1,mid=2,actors=2,floor=4,foreground=4),default_playing=False,integer_pixels=True),screens=list(screens),unity_integration=False)
    (ROOT/'metadata.json').write_text(json.dumps(metadata,indent=2),encoding='utf8')
    (ROOT/'source/text-layout.json').write_text(json.dumps(TEXT,indent=2),encoding='utf8')
    cards=''.join(f'<article><h2>{name}</h2><img width="640" height="360" src="screens/640x360/{name}.png" alt="{name}"><p>'+ ' Â· '.join(f'<a href="screens/{s}/{name}.png">{s}</a>' for s in ['320x180','640x360','1280x720'])+'</p></article>' for name in screens)
    template=(ROOT/'source/gallery.html').read_text(encoding='utf8')
    (ROOT/'index.html').write_text(template.replace('<!-- SCREENS -->',cards),encoding='utf8')
    print(f'Built {len(ASSETS)} PNG/SVG assets; 10 screens x 3 sizes; 48 fixed HUD overlays.')


if __name__=='__main__': build()
