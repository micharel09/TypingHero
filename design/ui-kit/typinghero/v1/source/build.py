"""Rebuild review assets with Pillow. No Unity files are written.
Run from any directory: python design/ui-kit/typinghero/v1/source/build.py
ImageGen originals are immutable inputs; PNG/SVG geometry shares this source.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import json, shutil, html, zipfile, math

ROOT = Path(__file__).resolve().parents[1]
REPO = ROOT.parents[3]
P = dict(coal='#111b20', deep='#192b2c', jade='#528d83', light='#8ab5a5',
         ivory='#f2e7cf', muted='#a6b2a6', vermilion='#c65140', error='#ff685c',
         slayer='#dfb965', disabled='#536160', shadow='#090f14')
for folder in ['png','svg','screens/640x360','screens/1280x720','sheets','fonts','intro','artwork']:
    (ROOT/folder).mkdir(parents=True, exist_ok=True)
FONT_SOURCE = REPO/'Assets/Art/UI/Pixel/VT323-Regular.ttf'
shutil.copyfile(FONT_SOURCE, ROOT/'fonts/VT323-Regular.ttf')
shutil.copyfile(REPO/'Assets/Art/UI/Pixel/VT323-OFL.txt', ROOT/'fonts/VT323-OFL.txt')
assets = []
text_checks = []

def font(n): return ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'), n)
def text(im, xy, s, size=18, color='ivory', center=False, maxw=None):
    f=font(size); d=ImageDraw.Draw(im)
    if maxw:
        while d.textlength(s,font=f)>maxw and size>10:
            size-=1; f=font(size)
        assert d.textlength(s,font=f)<=maxw, (s,maxw)
    x,y=xy
    if center: x-=d.textlength(s,font=f)/2
    d.text((round(x),y),s,font=f,fill=P.get(color,color),stroke_width=0)
    text_checks.append(dict(text=s,size=size,x=round(x),y=y,width=round(d.textlength(s,font=f)),limit=maxw))

class Art:
    def __init__(self,w,h):
        self.im=Image.new('RGBA',(w,h)); self.d=ImageDraw.Draw(self.im); self.svg=[]; self.w=w; self.h=h
    def rect(self,x,y,w,h,c):
        c=P.get(c,c); self.d.rectangle((x,y,x+w-1,y+h-1),fill=c)
        self.svg.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{c}"/>')
    def poly(self,pts,c):
        c=P.get(c,c); self.d.polygon(pts,fill=c)
        self.svg.append(f'<polygon points="{" ".join(f"{x},{y}" for x,y in pts)}" fill="{c}"/>')
    def step(self,x,y,w,h,c,cut=3):
        self.poly([(x+cut,y),(x+w-cut-1,y),(x+w-cut-1,y+cut),(x+w-1,y+cut),
                   (x+w-1,y+h-cut-1),(x+w-cut-1,y+h-cut-1),(x+w-cut-1,y+h-1),
                   (x+cut,y+h-1),(x+cut,y+h-cut-1),(x,y+h-cut-1),(x,y+cut),(x+cut,y+cut)],c)
    def save(self,name,group,border=(0,0,0,0),notes=''):
        self.im.save(ROOT/'png'/f'{name}.png')
        (ROOT/'svg'/f'{name}.svg').write_text(f'<svg xmlns="http://www.w3.org/2000/svg" width="{self.w}" height="{self.h}" viewBox="0 0 {self.w} {self.h}" shape-rendering="crispEdges">'+''.join(self.svg)+'</svg>',encoding='utf-8')
        assets.append(dict(id=name,group=group,png=f'png/{name}.png',svg=f'svg/{name}.svg',size=[self.w,self.h],pivot=[.5,.5],border_LBRT=list(border),filter='Point',wrap='Clamp',mipmaps=False,alpha='straight RGBA; transparent RGB zero',notes=notes))
        return self.im

STATES=['normal','hover','pressed','focus','disabled']
def frame(name,w,h,group='frames',state='normal',primary=False,border=6):
    a=Art(w,h); edge='jade'; fill='deep'
    if primary: edge='vermilion'; fill='vermilion'
    if state=='hover': edge='ivory'; fill='jade' if primary else 'deep'
    if state=='focus': edge='ivory'
    if state=='disabled': edge='disabled'; fill='coal'
    if state=='error': edge='error'
    if state=='selected': edge='slayer'
    if state=='pressed': edge='jade'; fill='coal'
    a.step(0,1,w,h-1,'shadow',4); a.step(0,0,w,h-3,edge,3)
    a.step(2,2,w-4,h-7,fill,2)
    if state=='focus':
        for x in range(8,w-8,6): a.rect(x,h-6,3,1,'ivory')
    a.rect(6,3,8,1,'ivory' if state!='disabled' else 'disabled')
    return a.save(name,group,(border, border, border, border),'Stretch only central region; preserve corner pixel grid.')

def make_controls():
    for primary in [False,True]:
        for state in STATES: frame(f'button-{"primary" if primary else "secondary"}-{state}',120,32,'controls',state,primary)
    for state in STATES+['error']:
        frame('input-'+state,220,34,'controls',state)
    for state in STATES+['selected']:
        frame('tab-'+state,112,28,'controls',state)
    for kind in ['panel','modal','tooltip','leaderboard-row','opponent-card','achievement-card']:
        w,h={'panel':(96,64),'modal':(128,96),'tooltip':(96,40),'leaderboard-row':(160,28),'opponent-card':(112,72),'achievement-card':(160,56)}[kind]
        for state in (['normal','selected','disabled'] if 'card' in kind or 'row' in kind else ['normal']):
            frame(kind+'-'+state,w,h,'frames',state)
    for kind in ['slider','scrollbar']:
        for state in STATES:
            a=Art(120,16) if kind=='slider' else Art(12,80)
            c='disabled' if state=='disabled' else 'ivory' if state in ['focus','hover'] else 'jade'
            if kind=='slider':
                a.rect(1,6,118,4,'coal'); a.rect(1,6,70,4,'jade'); a.step(66,1,9,14,c,2)
            else:
                a.rect(4,0,4,80,'coal'); a.step(1,12,10,25,c,2)
            if state=='pressed': a.rect(69,4,3,8,'coal') if kind=='slider' else a.rect(4,16,4,17,'coal')
            a.save(kind+'-'+state,'controls')
    for kind in ['checkbox','toggle']:
        for on in [False,True]:
            for state in STATES:
                a=Art(20 if kind=='checkbox' else 36,20); c='disabled' if state=='disabled' else 'ivory' if state in ['focus','hover'] else 'jade'
                a.step(0,0,a.w,20,c,3); a.step(2,2,a.w-4,16,'coal',2)
                if kind=='toggle': a.step(18 if on else 3,3,15,14,c,2)
                elif on:
                    a.poly([(4,9),(7,12),(14,5),(16,7),(7,16),(2,11)],c)
                if state=='pressed': a.rect(4,17,a.w-8,1,'ivory')
                a.save(f'{kind}-{"on" if on else "off"}-{state}','controls')
    for kind in ['hp','stamina','combo','timer','slayer']:
        a=Art(180,12); a.step(0,0,180,12,'ivory',2); a.rect(2,2,176,8,'coal'); a.save('bar-'+kind+'-track','hud',(3,3,3,3))
        a=Art(176,8); a.rect(0,0,176,8,{'hp':'vermilion','stamina':'jade','combo':'light','timer':'muted','slayer':'slayer'}[kind]); a.rect(0,0,176,1,'ivory'); a.save('bar-'+kind+'-fill','hud')
    for kind in ['typing','typing-correct','typing-error','slayer','cue']:
        frame(kind+'-frame',160,64,'hud','error' if kind=='typing-error' else 'selected' if kind=='slayer' else 'focus' if kind=='typing-correct' else 'normal')

def make_icons():
    names=['hp','stamina','score','combo','audio-on','audio-off','music','pause','restart','continue','close','back','check','warning','slayer','trophy','lock','sword','shield','axe','timer','chevron-left','chevron-right']
    for name in names:
        a=Art(20,20); c='ivory'
        if name=='hp': a.poly([(3,3),(7,3),(10,6),(13,3),(17,3),(19,6),(19,10),(10,18),(1,10),(1,6)],'vermilion')
        elif name=='stamina': a.poly([(11,1),(3,12),(9,12),(7,19),(17,7),(11,7)],'jade')
        elif name in ['score','trophy']:
            a.poly([(5,2),(15,2),(15,10),(11,14),(11,17),(16,17),(16,19),(4,19),(4,17),(9,17),(9,14),(5,10)],'slayer'); a.rect(2,4,2,6,'slayer'); a.rect(16,4,2,6,'slayer')
        elif name in ['combo','slayer']:
            a.poly([(9,1),(14,8),(13,4),(18,10),(18,15),(13,19),(5,19),(2,13),(5,6),(6,11)],'slayer' if name=='slayer' else 'light'); a.rect(8,12,4,5,'coal')
        elif name.startswith('audio'):
            a.rect(2,7,4,7,c); a.poly([(6,7),(11,2),(11,18),(6,14)],c)
            if name=='audio-on': a.rect(14,6,2,8,c); a.rect(18,3,2,14,c)
            else:
                for i in range(6): a.rect(13+i,7+i,2,2,'error'); a.rect(18-i,7+i,2,2,'error')
        elif name=='music': a.rect(7,2,2,13,c); a.rect(7,2,10,3,c); a.rect(15,2,2,11,c); a.rect(2,13,7,5,c); a.rect(10,11,7,5,c)
        elif name=='pause': a.rect(4,3,4,14,c); a.rect(12,3,4,14,c)
        elif name=='continue': a.poly([(5,2),(17,10),(5,18)],c)
        elif name in ['close','warning','check','back','chevron-left','chevron-right']:
            if name=='close':
                for i in range(12): a.rect(4+i,4+i,2,2,c); a.rect(15-i,4+i,2,2,c)
            elif name=='warning': a.poly([(10,1),(20,18),(0,18)],'error'); a.rect(9,6,2,6,'coal'); a.rect(9,14,2,2,'coal')
            elif name=='check': a.poly([(1,10),(7,16),(19,4),(17,2),(7,12),(3,8)],'jade')
            elif name in ['back','chevron-left']: a.poly([(13,2),(5,10),(13,18),(15,16),(9,10),(15,4)],c)
            else: a.poly([(5,2),(13,10),(5,18),(3,16),(9,10),(3,4)],c)
        elif name=='restart':
            a.rect(5,2,11,3,c); a.rect(14,4,3,11,c); a.rect(4,14,12,3,c); a.rect(2,7,3,9,c); a.poly([(1,1),(9,7),(1,7)],c)
        elif name=='lock': a.rect(4,9,12,10,c); a.rect(5,3,3,7,c); a.rect(12,3,3,7,c); a.rect(7,1,6,3,c); a.rect(9,12,2,4,'coal')
        elif name=='shield': a.poly([(2,3),(10,1),(18,3),(17,13),(10,19),(3,13)],'jade'); a.rect(9,4,2,11,c)
        elif name=='axe': a.rect(9,2,3,17,c); a.poly([(3,2),(15,2),(19,9),(12,12),(12,6),(3,8)],'muted')
        elif name=='sword': a.poly([(14,1),(19,1),(19,6),(8,17),(4,13)],c); a.poly([(2,12),(9,19),(11,17),(4,10)],'slayer'); a.rect(1,16,4,4,'jade')
        elif name=='timer': a.rect(7,1,6,2,c); a.step(3,4,14,15,c,3); a.step(5,6,10,11,'coal',2); a.rect(9,7,2,6,c); a.rect(10,11,4,2,c)
        a.save('icon-'+name,'icons')
    for key in ['enter','esc','space','r','m','a','backspace']:
        a=Art(48 if key in ['enter','space','backspace'] else 28,24); a.step(0,0,a.w,24,'muted',3); a.step(2,2,a.w-4,18,'deep',2)
        # Glyphs remain separate in the keycap PNG; semantic label belongs to text layer.
        if key=='enter': a.rect(28,5,2,7,'ivory'); a.rect(15,10,14,2,'ivory'); a.poly([(14,11),(20,6),(20,16)],'ivory')
        elif key=='space': a.rect(8,11,2,4,'ivory'); a.rect(8,14,32,2,'ivory'); a.rect(38,11,2,4,'ivory')
        elif key=='backspace':
            a.poly([(8,10),(15,4),(36,4),(36,17),(15,17)],'ivory'); a.rect(18,8,13,5,'coal')
        else:
            word=key.upper(); width=sum((len(GLYPHS[ch][0])+1) for ch in word)-1
            lettering(a,word,(a.w-width)//2,7,1,'ivory')
        a.save('keycap-'+key,'icons',notes='Static physical key symbol; never use as the typing word.')

# Bespoke angular lettering: original 5x7 matrices, sheared stems and cut corners.
GLYPHS={
'T':['11111','00100','00100','00100','00100','00100','00100'],
'Y':['10001','10001','01010','00100','00100','00100','00100'],
'P':['11110','10001','10001','11110','10000','10000','10000'],
'I':['111','010','010','010','010','010','111'],
'N':['10001','11001','11001','10101','10011','10011','10001'],
'G':['01111','10000','10000','10111','10001','10001','01110'],
'H':['10001','10001','10001','11111','10001','10001','10001'],
'E':['11111','10000','10000','11110','10000','10000','11111'],
'R':['11110','10001','10001','11110','10100','10010','10001'],
'O':['01110','10001','10001','10001','10001','10001','01110'],
'A':['01110','10001','10001','11111','10001','10001','10001'],
'M':['10001','11011','10101','10101','10001','10001','10001'],
'S':['01111','10000','10000','01110','00001','00001','11110'],
'C':['01111','10000','10000','10000','10000','10000','01111']}
def lettering(a,word,x,y,scale,color):
    for ch in word:
        glyph=GLYPHS[ch]
        for row,line in enumerate(glyph):
            shift=(6-row)//3
            for col,v in enumerate(line):
                if v=='1': a.rect(x+col*scale+shift,y+row*scale,scale,scale,color)
        x+=(len(glyph[0])+1)*scale
    return x
def make_brand():
    for name,mono in [('logo-main',False),('logo-mono',True)]:
        a=Art(290,90)
        lettering(a,'TYPING',6,5,5,'shadow'); lettering(a,'HERO',42,47,5,'shadow')
        lettering(a,'TYPING',3,2,5,'ivory'); lettering(a,'HERO',39,44,5,'ivory' if mono else 'vermilion')
        a.poly([(3,84),(259,80),(247,86),(3,88)],'ivory' if mono else 'jade')
        a.save(name,'brand')
    a=Art(224,34); lettering(a,'TYPINGHERO',2,2,3,'ivory'); a.save('logo-compact','brand')
    a=Art(64,64); a.step(0,0,64,64,'coal',8); a.step(2,2,60,60,'vermilion',7); a.step(5,5,54,54,'coal',5)
    lettering(a,'T',10,13,4,'ivory'); lettering(a,'H',32,23,4,'ivory'); a.poly([(9,53),(55,48),(51,54),(9,57)],'jade'); a.save('game-icon','brand')

def load(name): return Image.open(ROOT/'png'/f'{name}.png').convert('RGBA')
def paste(im,a,x,y): im.alpha_composite(a,(int(x),int(y)))
def stretch(a,w,h,b=6):
    out=Image.new('RGBA',(w,h)); sw,sh=a.size
    for i,(sx0,sx1,dx0,dx1) in enumerate([(0,b,0,b),(b,sw-b,b,w-b),(sw-b,sw,w-b,w)]):
        for sy0,sy1,dy0,dy1 in [(0,b,0,b),(b,sh-b,b,h-b),(sh-b,sh,h-b,h)]:
            tile=a.crop((sx0,sy0,sx1,sy1)).resize((dx1-dx0,dy1-dy0),Image.Resampling.NEAREST); paste(out,tile,dx0,dy0)
    return out
def box(im,x,y,w,h,name='panel-normal'): paste(im,stretch(load(name),w,h),x,y)
def button(im,x,y,w,label,primary=False,state='normal'):
    box(im,x,y,w,30,f'button-{"primary" if primary else "secondary"}-{state}')
    text(im,(x+w/2,y+5+(state=='pressed')),label,20,'disabled' if state=='disabled' else 'ivory',True,w-14)

def prepare_art():
    def clean_transparent_rgb(im):
        # Preserve every visible source pixel, clear invisible RGB only.
        out=im.copy(); out.putdata([(0,0,0,0) if px[3]==0 else px for px in im.get_flattened_data()]); return out
    bg=Image.open(ROOT/'artwork/title-original.png').convert('RGBA').resize((640,360),Image.Resampling.NEAREST)
    # Original generator's noninteger pixel clusters are normalized to one logical pixel.
    bg=bg.convert('RGB').quantize(colors=32,method=Image.Quantize.MEDIANCUT).convert('RGBA')
    bg.save(ROOT/'artwork/01-backdrop.png')
    a=Art(640,360)
    a.poly([(0,334),(65,333),(112,340),(180,336),(246,344),(318,338),(400,342),(489,333),(560,338),(640,330),(640,360),(0,360)],'coal')
    for x in [12,68,510,585,610]:
        a.poly([(x,350),(x-6,330),(x+2,340),(x+6,324),(x+8,350)],'deep')
        a.rect(x+12,338,5,2,'vermilion')
    a.im.save(ROOT/'artwork/foreground-vector-alternative.png')
    (ROOT/'svg/foreground.svg').write_text('<svg xmlns="http://www.w3.org/2000/svg" width="640" height="360" shape-rendering="crispEdges">'+''.join(a.svg)+'</svg>')
    fg=Image.open(ROOT/'artwork/foreground-original.png').convert('RGBA').resize((640,360),Image.Resampling.NEAREST)
    alpha=fg.getchannel('A').point(lambda x:255 if x>=128 else 0)
    fg=fg.convert('RGB').quantize(colors=24,method=Image.Quantize.MEDIANCUT).convert('RGBA'); fg.putalpha(alpha)
    clean=Image.new('RGBA',fg.size); clean.paste(fg,(0,0),alpha); clean.save(ROOT/'artwork/03-foreground.png')
    hero=Image.open(REPO/'Assets/Art/male_hero/individual_sheets/male_hero-idle.png').convert('RGBA').crop((0,0,128,128))
    hero=clean_transparent_rgb(hero.crop(hero.getbbox())); hero.save(ROOT/'artwork/hero-existing.png')
    enemy_path=REPO/'Assets/Art/EndlessSkeletons/sword/Idle.png'
    enemy=Image.open(enemy_path).convert('RGBA')
    # Source TextureImporter sprite rect: 96x64. Crop exactly the first frame.
    enemy=enemy.crop((0,0,96,64)); enemy=clean_transparent_rgb(enemy.crop(enemy.getbbox())); enemy.save(ROOT/'artwork/opponent-existing.png')
    layer=Image.new('RGBA',(640,360)); paste(layer,hero.resize((hero.width*4,hero.height*4),Image.Resampling.NEAREST),460,177)
    layer.save(ROOT/'artwork/02-character.png')
    return bg

def background(dim=False):
    im=Image.open(ROOT/'artwork/01-backdrop.png').convert('RGBA')
    if dim: im.alpha_composite(Image.new('RGBA',im.size,(9,15,20,172)))
    return im
def hero_at(im,x,y,scale=3,enemy=False):
    a=Image.open(ROOT/('artwork/opponent-existing.png' if enemy else 'artwork/hero-existing.png')).convert('RGBA')
    a=a.resize((a.width*scale,a.height*scale),Image.Resampling.NEAREST)
    if enemy: a=a.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    paste(im,a,x,y-a.height)

def menu(error=False,long_name=False):
    im=background(); paste(im,load('logo-main'),29,24)
    # Narrow opaque surface ensures legibility over illustration.
    box(im,32,126,254,150)
    text(im,(46,134),'PLAYER NAME',16,'muted')
    box(im,44,154,230,32,'input-error' if error else 'input-focus')
    text(im,(54,159),'WWWWWWWWWW' if long_name else 'Your name',22,'ivory' if long_name else 'muted',maxw=206)
    text(im,(46,188),'Difficulty:   <   Normal   >',17,'muted')
    button(im,44,213,230,'PLAY   [ENTER]',True)
    text(im,(46,248),'Please enter your name (3-10 characters).' if error else 'Parry, then type during recovery.',14,'error' if error else 'muted',maxw=230)
    hero_at(im,456,313,4)
    for i,(label,w) in enumerate([('How to Play',116),('Tutorial',90),('Collection',110)]):
        x=[32,154,250][i]; button(im,x,287,w,label)
    box(im,390,323,224,26)
    text(im,(400,328),'MUSIC',14); paste(im,load('slider-normal').resize((72,12),Image.Resampling.NEAREST),444,328)
    text(im,(525,328),'SFX',14); paste(im,load('slider-normal').resize((50,12),Image.Resampling.NEAREST),552,328)
    return im

def resource(im,x,y,label,value,ratio,kind,width=180,color='ivory'):
    text(im,(x,y),label,14,'muted'); text(im,(x+width,y),value,14,color,True)
    paste(im,load('bar-'+kind+'-track'),x,y+18)
    if ratio>0: paste(im,load('bar-'+kind+'-fill').crop((0,0,max(1,round(176*ratio)),8)),x+2,y+20)

def hud(slayer=False,edge=False,feedback='normal'):
    im=background(); im.alpha_composite(Image.new('RGBA',im.size,(9,15,20,55)))
    box(im,10,8,204,75); box(im,426,8,204,75)
    resource(im,20,15,'WWWWWWWWWW' if edge else 'HERO','1 / 100' if edge else '85 / 100',.01 if edge else .85,'hp',160,'error' if edge else 'ivory')
    resource(im,20,48,'COMBO DECAY','',.72,'combo',160)
    resource(im,438,15,'CAPTAIN','200 / 550',.36,'hp',160)
    resource(im,438,48,'STAMINA','4 / 12',.33,'stamina',160)
    box(im,228,8,184,75,'slayer-frame' if slayer else 'panel-normal')
    text(im,(320,13),'SCORE',14,'muted',True)
    text(im,(320,28),'2,147,483,647' if edge else '012,840',27,'slayer' if slayer else 'ivory',True,168)
    text(im,(320,57),'999 COMBO / x16' if edge else '24 COMBO / x4',17,'slayer' if slayer else 'light',True,170)
    text(im,(320,103),'SLAYER / x16' if slayer else 'ROUND 12 / NORMAL',19,'slayer' if slayer else 'ivory',True)
    hero_at(im,196,250,3); hero_at(im,397,250,3,True)
    paste(im,load('03-foreground') if (ROOT/'png/03-foreground.png').exists() else Image.open(ROOT/'artwork/03-foreground.png'),0,0)
    box(im,162,252,316,92,'slayer-frame' if slayer else 'typing-'+feedback+'-frame' if feedback!='normal' else 'typing-frame')
    text(im,(320,259),'TYPE / ATTACK' if feedback=='normal' else 'CORRECT' if feedback=='correct' else 'MISS / TRY AGAIN',15,'error' if feedback=='error' else 'light',True)
    text(im,(320,276),'counterstrike' if edge else 'resolve',34,'ivory',True,280)
    text(im,(320,313),'NEXT  courage   /   honor',17,'muted',True,288)
    text(im,(132,280),'PARRY',17,'slayer',True); paste(im,load('keycap-space'),108,299)
    text(im,(510,280),'00:42',20,'ivory',True); paste(im,load('icon-pause'),500,309)
    if slayer:
        d=ImageDraw.Draw(im); d.rectangle((1,1,638,358),outline=P['slayer'],width=2)
        paste(im,load('icon-slayer'),310,131)
    return im

def overlay(title,lines,actions,base=None):
    im=base or hud(); im.alpha_composite(Image.new('RGBA',im.size,(9,15,20,195)))
    box(im,147,38,346,286,'modal-normal'); text(im,(320,53),title,30,'ivory',True,312)
    y=99
    for line in lines:
        text(im,(320,y),line,18,'muted',True,304); y+=23
    y=max(188,y+12)
    for i,label in enumerate(actions):
        button(im,173,y+i*37,294,label,i==0)
    return im

def collection(mode,status='ready'):
    im=background(True); box(im,20,16,600,328,'modal-normal')
    text(im,(38,27),'COLLECTION',28); text(im,(592,29),'X',22)
    modes=['Endless','Classic','Opponents','Achievements']
    for i,label in enumerate(modes):
        x=34+i*143; box(im,x,65,139,28,'tab-selected' if mode==label else 'tab-normal'); text(im,(x+69,69),label,20,center=True,maxw=125)
    if mode=='Endless': text(im,(40,103),'DIFFICULTY   <  Normal  >',17,'muted')
    if status!='ready':
        text(im,(320,179),{'loading':'Loading...','empty':'No scores yet.','error':'Could not load leaderboard.'}[status],26,'error' if status=='error' else 'ivory',True)
        if status=='error': text(im,(320,218),'Close and reopen Collection to try again.',17,'muted',True)
        if status=='empty': text(im,(320,218),'Complete a run to submit your score.',18,'muted',True)
    elif mode in ['Endless','Classic']:
        text(im,(42,134),'#   PLAYER',16,'muted'); text(im,(365,134),'ROUND / KILLS / SCORE' if mode=='Endless' else 'SCORE',16,'muted')
        for i in range(5):
            y=158+i*31; box(im,34,y,563,29,'leaderboard-row-selected' if i==1 else 'leaderboard-row-normal')
            text(im,(44,y+5),f'{i+1:02}',18,'slayer' if i==0 else 'muted')
            text(im,(80,y+5),'WWWWWWWWWW' if i==1 else ['Ronin','Jade','Ember','Sora','Ash'][i],20,maxw=248)
            text(im,(368,y+5),f'{98-i:02} / {550-i*24:03} / 2,147,483,647' if mode=='Endless' else '2,147,483,647',17,maxw=218)
        paste(im,load('scrollbar-normal'),600,158)
    elif mode=='Opponents':
        names=['SWORDSMAN','DOUBLE STRIKE','HEAVY','BERSERKER','GUARD','CAPTAIN / BOSS']
        for i,label in enumerate(names):
            x=34+(i%3)*190; y=108+(i//3)*105; box(im,x,y,184,96,'opponent-card-normal' if i<5 else 'opponent-card-disabled')
            paste(im,load('icon-'+('sword' if i<2 else 'axe' if i<4 else 'shield')),x+10,y+10)
            text(im,(x+37,y+10),label if i<5 else '???',18,maxw=136)
            text(im,(x+10,y+39),'SEEN 12 / DEFEATED 8' if i<5 else 'NOT ENCOUNTERED',16,'muted',maxw=164)
            text(im,(x+10,y+60),'ELITE 3 / DEFEATED 1' if i<5 else 'Discover in Endless.',15,'muted',maxw=164)
    else:
        labels=[('FIRST CAPTAIN','Defeat your first boss.'),('THREE PARRIES','Parry every strike in a triple chain.'),('UNTOUCHED','Defeat a boss without losing HP.')]
        for i,(label,desc) in enumerate(labels):
            y=108+i*70; box(im,34,y,563,64,'achievement-card-selected' if i==0 else 'achievement-card-disabled')
            paste(im,load('icon-trophy' if i==0 else 'icon-lock'),48,y+20)
            text(im,(82,y+7),label,23,'slayer' if i==0 else 'muted'); text(im,(82,y+34),desc,18,'muted',maxw=494)
    text(im,(40,323),'ESC  CLOSE',14,'muted'); return im

def screens():
    result={'menu':menu(),'menu-name-error':menu(True),'menu-long-name':menu(long_name=True),'hud-normal':hud(),'hud-slayer':hud(True),
            'hud-stress-low-hp':hud(True,True),'hud-correct':hud(feedback='correct'),'hud-error':hud(feedback='error'),
            'pause':overlay('PAUSED',[],['Resume','Restart [R]','Main Menu [M]']),
            'results-defeat':overlay('DEFEAT',['SCORE 2,147,483,647 / TIME 428.0s','ACCURACY 98.4% / PARRIES 128','NORMAL / CLEARED 24'],['Restart [R]','Main Menu [M]']),
            'results-victory':overlay('THANKS FOR PLAYING',['SCORE 012,840 / TIME 42.0s','ACCURACY 98.4% / PARRIES 12'],['Restart [R]','Main Menu [M]']),
            'continue':overlay('ROUND CLEARED',['NORMAL / CLEARED 12 / HP 85/100','NEXT: ROUND 13 / SWORDSMAN','Read each cue. Watch for thrusts.'],['Continue [ENTER]']),
            'how-to-play':overlay('HOW TO PLAY',['Type the word to attack.','Parry with Space when the cue appears.','Type during enemy recovery.'],['Continue']),
            'tutorial':overlay('PRACTICE / PARRY',['Watch the enemy cue.','Press Space to parry.'],['Practice [ENTER]']),
            'tutorial-passed':overlay('PARRY / COMPLETE',['Good timing. Ready for the next lesson.'],['Next [ENTER]']),
            'tutorial-complete':overlay('TRAINING COMPLETE',['Combine typing and parrying.'],['Start Battle'])}
    for mode in ['Endless','Classic','Opponents','Achievements']: result['collection-'+mode.lower()]=collection(mode)
    lessons=[('ATTACK','Type BLADE to attack.'),('ACCURACY','Mistakes reset the word.'),('PARRY','Press SPACE when the yellow cue appears.'),('STAMINA','One more parry will break its stamina.'),('SLAYER','Each new letter strikes. Type FIRE.'),('COMBINE','Type to attack. Watch the cue. Parry.')]
    for i,(title,instruction) in enumerate(lessons):
        result['tutorial-'+title.lower()]=overlay(f'{i+1} / 6 / {title}',[instruction],['Practice [ENTER]'])
    result['tutorial-retry']=overlay('2 / 6 / ACCURACY / TRY AGAIN',['Mistakes reset the word.','Finish before time runs out.'],['Practice [ENTER]'])
    for status in ['loading','empty','error']: result['collection-'+status]=collection('Endless',status)
    for name,im in result.items():
        im.convert('RGB').save(ROOT/'screens/640x360'/f'{name}.png')
        im.resize((1280,720),Image.Resampling.NEAREST).convert('RGB').save(ROOT/'screens/1280x720'/f'{name}.png')
    return result

def sheets_and_contact(previews):
    # Atlas with 2px extrusion, 4px spacing. Top-left pixel coordinates in JSON.
    atlas=Image.new('RGBA',(1024,1024)); x=y=4; rowh=0
    for item in assets:
        a=load(item['id']); w,h=a.size
        if x+w+6>1024: x=4; y+=rowh+8; rowh=0
        assert y+h+6<=1024
        # extrude boundary two pixels; transparent corner pixels stay transparent
        paste(atlas,a,x,y)
        for pad in [1,2]:
            paste(atlas,a.crop((0,0,w,1)),x,y-pad); paste(atlas,a.crop((0,h-1,w,h)),x,y+h-1+pad)
            paste(atlas,a.crop((0,0,1,h)),x-pad,y); paste(atlas,a.crop((w-1,0,w,h)),x+w-1+pad,y)
        for sx,sy,dx,dy in [(0,0,x-2,y-2),(w-1,0,x+w,y-2),(0,h-1,x-2,y+h),(w-1,h-1,x+w,y+h)]:
            paste(atlas,a.crop((sx,sy,sx+1,sy+1)).resize((2,2),Image.Resampling.NEAREST),dx,dy)
        item['atlas']=dict(file='sheets/ui-atlas.png',rect_top_left=[x,y,w,h],extrude=2)
        x+=w+8; rowh=max(rowh,h)
    atlas.save(ROOT/'sheets/ui-atlas.png')
    # Actual alpha assets shown on checkerboard, independently named.
    cols=5; cellw=310; cellh=132; rows=math.ceil(len(assets)/cols)
    sheet=Image.new('RGB',(cols*cellw,rows*cellh),P['coal'])
    for i,item in enumerate(assets):
        x=(i%cols)*cellw; y=(i//cols)*cellh
        for yy in range(y+25,y+120,8):
            for xx in range(x+4,x+302,8):
                ImageDraw.Draw(sheet).rectangle((xx,yy,xx+7,yy+7),fill='#263a3c' if ((xx//8+yy//8)%2) else '#1c2c30')
        text(sheet,(x+6,y+3),item['id'],16,maxw=298)
        a=load(item['id']); sheet.paste(a,(x+10,y+30),a)
    sheet.save(ROOT/'contact-sheet-assets.png')
    sheet=Image.new('RGB',(1280,math.ceil(len(previews)/2)*393),P['coal'])
    for i,(name,im) in enumerate(previews.items()):
        x=i%2*640; y=i//2*393; text(sheet,(x+12,y+5),name,22); sheet.paste(im.convert('RGB'),(x,y+30))
    sheet.save(ROOT/'contact-sheet-screens.png')

def intro():
    bg=background(); char=Image.open(ROOT/'artwork/02-character.png'); fg=Image.open(ROOT/'artwork/03-foreground.png'); logo=load('logo-main'); menu_im=menu()
    frames=[]
    def opacity(a,value):
        b=a.copy(); b.putalpha(b.getchannel('A').point(lambda x:round(x*value))); return b
    def progress(t,start,end): return max(0,min(1,(t-start)/(end-start)))
    for i in range(60):
        t=i/25; im=Image.new('RGBA',(640,360),P['coal'])
        paste(im,opacity(bg,progress(t,0,.4)),0,0)
        paste(im,opacity(char,progress(t,.4,.85)),0,round(8*(1-progress(t,.4,.85))))
        paste(im,opacity(fg,progress(t,.4,.85)),0,0)
        paste(im,opacity(logo,progress(t,.85,1.5)),29,24)
        if t>=1.65: im=Image.blend(im,menu_im,progress(t,1.65,2.2))
        frames.append(im.convert('RGB'))
    frames[-1]=menu_im.convert('RGB')
    frames[0].save(ROOT/'intro/intro-preview.gif',save_all=True,append_images=frames[1:],duration=40,loop=0,optimize=False,disposal=2)
    # Silent 2.4s Motion JPEG AVI: standard RIFF container, no external encoder needed.
    import io,struct
    jpg=[]
    for im in frames:
        stream=io.BytesIO(); im.save(stream,'JPEG',quality=95,subsampling=0); jpg.append(stream.getvalue())
    def chunk(tag,payload): return tag+struct.pack('<I',len(payload))+payload+(b'\0' if len(payload)%2 else b'')
    def listchunk(kind,payload): return chunk(b'LIST',kind+payload)
    avih=struct.pack('<14I',40000,0,0,16,len(frames),0,1,max(map(len,jpg)),640,360,0,0,0,0)
    strh=struct.pack('<4s4sIHHIIIIIIIIhhhh',b'vids',b'MJPG',0,0,0,0,1,25,0,len(frames),max(map(len,jpg)),0xffffffff,0,0,0,640,360)
    strf=struct.pack('<IiiHH4sIiiII',40,640,360,1,24,b'MJPG',640*360*3,0,0,0,0)
    hdrl=listchunk(b'hdrl',chunk(b'avih',avih)+listchunk(b'strl',chunk(b'strh',strh)+chunk(b'strf',strf)))
    movi=b''; idx=b''; offset=4
    for data in jpg:
        packet=chunk(b'00dc',data); idx+=struct.pack('<4sIII',b'00dc',16,offset,len(data)); movi+=packet; offset+=len(packet)
    body=b'AVI '+hdrl+listchunk(b'movi',movi)+chunk(b'idx1',idx)
    (ROOT/'intro/intro-preview.avi').write_bytes(b'RIFF'+struct.pack('<I',len(body))+body)
    import subprocess
    encoder=shutil.which('ffmpeg') or 'E:/codex-data/tools/ffmpeg-9.0.2-essentials_build/bin/ffmpeg.exe'
    if Path(encoder).exists():
        subprocess.run([encoder,'-v','error','-y','-i',str(ROOT/'intro/intro-preview.avi'),'-an','-c:v','libx264','-crf','16','-pix_fmt','yuv420p','-movflags','+faststart',str(ROOT/'intro/intro-preview.mp4')],check=True)

def gallery(previews):
    cards=''.join(f'<article><h3>{html.escape(n)}</h3><a href="screens/1280x720/{n}.png"><img loading="lazy" src="screens/640x360/{n}.png" alt="Mockup {n}" width="640" height="360"></a></article>' for n in previews)
    items=''.join(f'<button class="asset" data-group="{a["group"]}" data-name="{a["id"]}" onclick="inspect(this)" data-size="{a["size"]}" data-border="{a["border_LBRT"]}"><img loading="lazy" src="{a["png"]}" alt="{a["id"]}"><span>{a["id"]}</span></button>' for a in assets)
    page=r'''<!doctype html><html lang="vi"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>TypingHero — UI kit v1 review</title>
<style>@font-face{font-family:VT;src:url(fonts/VT323-Regular.ttf)}*{box-sizing:border-box}body{margin:0;background:#111b20;color:#f2e7cf;font:18px system-ui}header,main{max-width:1400px;margin:auto;padding:32px}h1,h2,h3,button{font-family:VT,monospace;font-weight:400}h1{font-size:58px;margin:8px 0}h2{font-size:36px;margin-top:48px}h3{font-size:24px;margin:12px 0}a{color:#8ab5a5}nav{display:flex;flex-wrap:wrap;gap:20px}img{image-rendering:pixelated;max-width:100%;height:auto}.screens{display:grid;grid-template-columns:repeat(auto-fit,minmax(min(100%,560px),1fr));gap:24px}.screens article{min-width:0}button,select,input{background:#192b2c;color:#f2e7cf;border:1px solid #528d83;padding:10px;font-size:20px}button{cursor:pointer}button:hover,button:focus-visible{outline:2px solid #f2e7cf}.assets{display:grid;grid-template-columns:repeat(auto-fit,minmax(245px,1fr));gap:12px}.asset{min-height:130px;display:flex;flex-direction:column;align-items:center;justify-content:space-around;gap:12px;background:repeating-conic-gradient(#1c2c30 0% 25%,#263a3c 0% 50%) 0/16px 16px}.asset img{max-height:90px;object-fit:contain}.asset span{font-size:17px;background:#111b20;padding:3px}#intro{position:relative;max-width:960px;aspect-ratio:16/9;background:#111b20;overflow:hidden}#intro>img{position:absolute;inset:0;width:100%;height:100%}#intro .logo{inset:6.66% auto auto 4.53%;width:45.31%;height:auto}#intro button{position:absolute;right:12px;bottom:12px}dialog{background:#192b2c;color:#f2e7cf;border:2px solid #528d83;max-width:92vw}dialog img{display:block;min-width:160px;max-height:250px;object-fit:contain;margin:20px auto}code{font-size:14px}.swatch{display:inline-block;padding:20px;margin:5px;border-bottom:8px solid var(--c)}.note{color:#a6b2a6;max-width:900px;line-height:1.6}.toolbar{display:flex;gap:12px;flex-wrap:wrap;margin:20px 0}footer{padding:40px}video{max-width:100%}</style>
<header><p>DESIGN REVIEW / V1 / 19.10.2026</p><h1>TYPING HERO</h1><p class="note">Bộ asset riêng để review. PNG/SVG là asset hoàn chỉnh trong phạm vi thiết kế; ảnh màn hình và intro là mô phỏng bố cục, chưa tích hợp Unity. Sprite nhân vật hiện có được giữ nguyên.</p><nav><a href="#screens">Màn hình</a><a href="#assets">Asset</a><a href="#motion">Intro 2,4s</a><a href="contact-sheet-assets.png">Contact sheet kit</a><a href="contact-sheet-screens.png">Contact sheet màn hình</a><a href="typinghero-ui-kit-v1.zip">Tải ZIP</a><a href="README.md">Bàn giao / credits / QA</a></nav></header>
<main><section id="motion"><h2>Intro / 2,4 giây</h2><div id="intro"><img class="bg" src="artwork/01-backdrop.png" alt="Night shrine"><img class="char" src="artwork/02-character.png" alt="Existing hero"><img class="fg" src="artwork/03-foreground.png" alt="Foreground"><img class="logo" src="png/logo-main.png" alt="Typing Hero"><img class="menu" src="screens/640x360/menu.png" alt="Menu"><button onclick="skipIntro()">Bỏ qua</button></div><div class="toolbar"><button onclick="playIntro()">Chạy lại</button><a href="intro/intro-preview.gif">GIF không tiếng</a><a href="intro/intro-preview.avi">Clip AVI / 25 fps</a></div><p class="note">0–0,4s nền → 0,4–0,85s nhân vật/foreground → 0,85–1,5s logo → 1,65–2,2s menu; giữ đến 2,4s. Bỏ qua bằng nút hoặc Esc. Không có bước Press Enter sau intro.</p></section>
<section><h2>Palette</h2>PALETTE</section><section id="screens"><h2>Mockup / 640×360 & 1280×720</h2><p class="note">Chọn ảnh để xem bản 1280×720. Dữ liệu trong ảnh chỉ dùng thử layout; text không nằm trong asset nền/panel.</p><div class="screens">SCREENS</div></section>
<section id="assets"><h2>Asset library</h2><div class="toolbar"><input type="search" id="search" aria-label="Tìm asset" placeholder="Tìm asset..."><select id="group" aria-label="Nhóm"><option value="">Tất cả</option><option>brand</option><option>controls</option><option>frames</option><option>icons</option><option>hud</option></select></div><div class="assets">ASSETS</div></section><dialog id="detail"><button onclick="document.querySelector('#detail').close()">Đóng</button><h3></h3><img alt="Asset"><pre></pre><a class="download" download>PNG</a> / <a class="vector" download>SVG</a></dialog></main><footer><a href="metadata.json">Metadata / pivot / 9-slice / atlas</a> · <a href="source/build.py">Nguồn tái tạo</a> · <a href="CREDITS.md">Credits</a></footer>
<script>let animations=[];function playIntro(){animations.forEach(a=>a.cancel());let q=s=>document.querySelector('#intro '+s);q('button').hidden=false;const start=(s,k,o)=>{let a=q(s).animate(k,{fill:'both',...o});animations.push(a);return a};start('.bg',[{opacity:0},{opacity:1}],{duration:400});for(let s of ['.char','.fg'])start(s,[{opacity:0,transform:'translateY(8px)'},{opacity:1,transform:'translateY(0)'}],{delay:400,duration:450,easing:'steps(8,end)'});start('.logo',[{opacity:0},{opacity:1}],{delay:850,duration:650,easing:'steps(10,end)'});const a=start('.menu',[{opacity:0},{opacity:1}],{delay:1650,duration:550});a.finished.then(()=>{q('button').hidden=true}).catch(()=>{})}function skipIntro(){animations.forEach(a=>a.finish());document.querySelector('#intro button').hidden=true}document.addEventListener('keydown',e=>{if(e.key==='Escape')skipIntro()});function filter(){let s=document.querySelector('#search').value.toLowerCase(),g=document.querySelector('#group').value;document.querySelectorAll('.asset').forEach(a=>a.hidden=!(a.dataset.name.includes(s)&&(!g||g===a.dataset.group)))}document.querySelector('#search').oninput=filter;document.querySelector('#group').onchange=filter;function inspect(b){const d=document.querySelector('#detail'),n=b.dataset.name;d.querySelector('h3').textContent=n;d.querySelector('img').src='png/'+n+'.png';d.querySelector('pre').textContent='Size: '+b.dataset.size+'\nPivot: [0.5, 0.5]\nBorder L B R T: '+b.dataset.border+'\nFilter: Point / mipmaps off';d.querySelector('.download').href='png/'+n+'.png';d.querySelector('.vector').href='svg/'+n+'.svg';d.showModal()}playIntro();</script></html>'''
    page=page.replace('SCREENS',cards).replace('ASSETS',items).replace('PALETTE',''.join(f'<span class="swatch" style="--c:{v}">{k}<br><code>{v}</code></span>' for k,v in P.items()))
    page=page.replace('<a href="intro/intro-preview.avi">Clip AVI / 25 fps</a>','<a href="intro/intro-preview.mp4">Clip MP4 / 25 fps</a> · <a href="intro/intro-preview.avi">AVI gốc</a>')
    page=page.replace('</style>','[hidden]{display:none!important}</style>')
    (ROOT/'index.html').write_text(page,encoding='utf-8')

def main():
    make_controls(); make_icons(); make_brand(); prepare_art(); previews=screens(); sheets_and_contact(previews); intro(); gallery(previews)
    metadata=dict(version=1,logical_canvas=[640,360],preview_scales=[1,2],coordinate_system='PNG/atlas rects top-left; border uses Unity left,bottom,right,top; normalized pivot bottom-left',palette=P,assets=assets,screens=list(previews),text_limit=10)
    metadata['artwork_layers']=[dict(file=f'artwork/{n}.png',size=[640,360],pivot=[.5,.5],border_LBRT=[0,0,0,0],filter='Point',order=i) for i,n in enumerate(['01-backdrop','02-character','03-foreground'])]
    (ROOT/'metadata.json').write_text(json.dumps(metadata,indent=2),encoding='utf-8')
    (ROOT/'source/text-layout.json').write_text(json.dumps(text_checks,indent=2),encoding='utf-8')
    print(f'Built {len(assets)} assets, {len(previews)} screens x 2 sizes; GIF + AVI intro 60 frames / 25 fps.')
    pack()

def pack():
    with zipfile.ZipFile(ROOT/'typinghero-ui-kit-v1.zip','w',zipfile.ZIP_DEFLATED) as z:
        for p in sorted(ROOT.rglob('*')):
            if p.is_file() and p.suffix!='.zip' and '__pycache__' not in p.parts: z.write(p,p.relative_to(ROOT))

if __name__=='__main__':
    import sys
    if '--pack-only' in sys.argv: pack()
    else: main()
