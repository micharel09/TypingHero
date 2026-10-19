"""Review-only kit. Pillow source is authoritative; never writes Unity assets."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import json, shutil, zipfile, math, html

ROOT = Path(__file__).resolve().parents[1]
REPO = ROOT.parents[3]
BASIS = json.loads((ROOT/'source/basis.json').read_text())
P = dict(shadow='#090811', coal='#14111e', deep='#211c2d', steel='#494153',
         edge='#77677f', gold='#a3844d', light='#dcc18a', ivory='#f2e7cf',
         muted='#afa1b3', red='#a6354c', error='#f16b67', jade='#588b8c', disabled='#58515f')
assets=[]; checks=[]
for folder in ['png','svg','artwork','fonts','sheets','intro','screens/320x180','screens/640x360','screens/1280x720']:
    (ROOT/folder).mkdir(parents=True,exist_ok=True)
for f in ['VT323-Regular.ttf','VT323-OFL.txt']:
    if (REPO/'Assets/Art/UI/Pixel'/f).exists(): shutil.copyfile(REPO/'Assets/Art/UI/Pixel'/f,ROOT/'fonts'/f)

def font(n): return ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'),n)
def text(im,x,y,s,size=10,c='ivory',center=False,limit=None):
    f=font(size); width=f.getlength(s)
    if limit is not None: assert width<=limit,(s,width,limit)
    x=round(x-width/2) if center else x
    # Rasterize only at native size; exported scales duplicate whole source pixels.
    mask=Image.new('L',im.size); pen=ImageDraw.Draw(mask)
    pen.text((x,y),s,font=f,fill=255)
    im.paste(P.get(c,c),(0,0),mask)
    checks.append(dict(text=s,size=size,x=x,y=y,width=width,limit=limit))

class Art:
    def __init__(self,w,h):
        self.im=Image.new('RGBA',(w,h)); self.d=ImageDraw.Draw(self.im); self.svg=[]; self.w=w; self.h=h
    def rect(self,x,y,w,h,c):
        c=P.get(c,c); self.d.rectangle((x,y,x+w-1,y+h-1),fill=c)
        self.svg.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{c}"/>')
    def poly(self,pts,c):
        c=P.get(c,c); self.d.polygon(pts,fill=c)
        self.svg.append('<polygon points="'+' '.join(f'{x},{y}' for x,y in pts)+f'" fill="{c}"/>')
    def save(self,name,group,border=0,notes=''):
        self.im.save(ROOT/'png'/f'{name}.png')
        (ROOT/'svg'/f'{name}.svg').write_text(f'<svg xmlns="http://www.w3.org/2000/svg" width="{self.w}" height="{self.h}" shape-rendering="crispEdges">'+''.join(self.svg)+'</svg>',encoding='utf8')
        assets.append(dict(id=name,group=group,png=f'png/{name}.png',svg=f'svg/{name}.svg',size=[self.w,self.h],pivot=[.5,.5],border_LBRT=[border]*4,notes=notes))
        return self.im

def paste(im,a,x,y): im.alpha_composite(a,(int(x),int(y)))
def load(n): return Image.open(ROOT/'png'/f'{n}.png').convert('RGBA')
def frame(name,w,h,group='controls',state='normal',primary=False):
    a=Art(w,h); rim='gold' if primary or state=='selected' else 'steel'
    if state in ['hover','focus']: rim='light'
    if state=='error': rim='error'
    if state=='disabled': rim='disabled'
    a.poly([(2,0),(w-3,0),(w-1,2),(w-1,h-3),(w-3,h-1),(2,h-1),(0,h-3),(0,2)],'shadow')
    a.rect(1,1,w-2,h-3,rim); a.rect(2,2,w-4,h-5,'edge' if state=='hover' else 'steel')
    a.rect(3,3,w-6,h-7,'shadow'); a.rect(4,4,w-8,h-9,'coal' if state=='pressed' else 'deep')
    a.rect(5,4,w-10,1,'shadow'); a.rect(5,h-6,w-10,1,'steel')
    for x in [2,w-5]:
        for y in [2,h-6]:
            a.rect(x,y,3,3,rim); a.rect(x,y,2,1,'light' if state!='disabled' else 'steel'); a.rect(x+1,y+1,1,1,'shadow')
    if state=='focus':
        for x in range(8,w-8,4): a.rect(x,h-3,2,1,'ivory')
    if state=='pressed': a.rect(7,5,w-14,1,'shadow')
    return a.save(name,group,6,'Corner rivets stay in 6px fixed border; no center ornaments.')

def controls():
    states=['normal','hover','pressed','focus','disabled']
    for kind,w,h in [('button-primary',100,19),('button-secondary',80,19),('input',112,20),('tab',70,18)]:
        for state in states+(['error'] if kind=='input' else ['selected'] if kind=='tab' else []):
            frame(kind+'-'+state,w,h,state=state,primary=kind=='button-primary')
    for kind,w,h in [('panel',80,48),('modal',120,90),('tooltip',90,28),('leaderboard-row',120,18),('opponent-card',90,46),('achievement-card',120,32),('typing',140,44)]:
        for state in (['normal','selected','disabled'] if 'card' in kind or 'row' in kind else ['normal','error','selected'] if kind=='typing' else ['normal']):
            frame(kind+'-'+state,w,h,'frames',state)
    # Rebuild v1 code-native icon geometry at half size with the v2 palette.
    # Each pixel is represented in SVG too; no SVG references to raster images.
    for name,item in sorted(BASIS['icons'].items()):
        src=Image.new('RGBA',tuple(item['size'])); src.putdata([tuple(p) for p in item['pixels']])
        old=BASIS['palette']
        mapping={tuple(bytes.fromhex(v[1:])):P.get(k,P['gold']) for k,v in old.items()}
        a=Art(*src.size)
        for y in range(src.height):
            for x in range(src.width):
                px=src.getpixel((x,y))
                if px[3]: a.rect(x,y,1,1,mapping.get(px[:3],P['ivory']))
        a.save(name,'icons' if name.startswith(('icon-','keycap-')) else 'controls')
    for kind,c in [('hp','red'),('stamina','jade'),('combo','gold'),('timer','muted'),('slayer','light')]:
        a=Art(84,7); a.rect(0,0,84,7,'shadow'); a.rect(1,1,82,5,'steel'); a.rect(2,2,80,3,'coal'); a.save('bar-'+kind+'-track','hud',2)
        a=Art(80,3); a.rect(0,0,80,3,c); a.rect(0,0,80,1,'ivory' if kind=='slayer' else 'light'); a.rect(0,2,80,1,'red' if kind=='hp' else 'steel'); a.save('bar-'+kind+'-fill','hud')
    a=Art(94,16)
    for x in [0,80]:
        a.poly([(x,8),(x+5,1),(x+12,4),(x+13,12),(x+7,15)],'shadow')
        a.poly([(x+2,8),(x+6,3),(x+10,5),(x+11,11),(x+7,13)],'edge')
        a.rect(x+6,6,2,5,'gold')
    a.rect(10,4,74,9,'shadow'); a.rect(11,5,72,7,'edge'); a.rect(12,6,70,5,'steel'); a.rect(13,7,68,3,'coal')
    a.save('boss-track','hud',0,'Fixed carved endcaps and separate badge. Do not nine-slice; crop fill continuously at [13,7,68,3].')
    a=Art(68,3); a.rect(0,0,68,3,'red'); a.rect(0,0,68,1,'error'); a.rect(0,2,68,1,'#63233c'); a.save('boss-fill','hud')
    a=Art(68,3)
    for x in range(8,68,9): a.rect(x,0,1,1,'light')
    a.save('boss-ticks','hud',notes='Decorative overlay only, never quantize HP.')
    for name in ['captain-badge','slayer-badge']:
        a=Art(15,17); a.poly([(7,0),(14,4),(12,12),(7,16),(2,12),(0,4)],'shadow'); a.poly([(7,1),(13,4),(11,11),(7,14),(3,11),(1,4)],'gold'); a.poly([(7,3),(11,5),(10,10),(7,12),(4,10),(3,5)],'deep')
        a.rect(6,5,3,6,'light'); a.rect(4,6,7,1,'ivory'); a.rect(5,11,5,1,'gold'); a.save(name,'hud')

def brand():
    # Original fantasy lettering: flared serifs, two-tone faces, steel shadow.
    ns={'GLYPHS':BASIS['glyphs']}
    for name,mono in [('logo-main',False),('logo-mono',True),('logo-compact',False)]:
        a=Art(142,39 if name!='logo-compact' else 20)
        words=[('TYPING',9,2),('HERO',34,21)] if name!='logo-compact' else [('TYPINGHERO',2,2)]
        scale=2
        for word,x,y in words:
            for ch in word:
                g=ns['GLYPHS'][ch]
                for row,line in enumerate(g):
                    for col,v in enumerate(line):
                        if v=='1':
                            xx=x+col*scale; yy=y+row*scale
                            a.rect(xx+1,yy+2,2,2,'shadow'); a.rect(xx,yy,2,2,'ivory' if mono else 'light' if row<3 else 'gold')
                            if row in [0,6]: a.rect(xx-1,yy,3,1,'ivory' if mono else 'light')
                x+=(len(g[0])+1)*scale
        a.save(name,'brand')
    a=Art(32,32); a.poly([(16,0),(31,9),(27,25),(16,31),(4,25),(0,9)],'gold'); a.poly([(16,3),(28,10),(25,23),(16,28),(7,23),(3,10)],'deep'); a.rect(15,6,2,19,'ivory'); a.rect(8,19,16,2,'light'); a.save('game-icon','brand')

def artwork():
    # Construct native grid geometry from concept's composition, not a downsample.
    a=Art(320,180); a.rect(0,0,320,180,'#110f1b')
    for row in range(7):
        y=12+row*18
        for col in range(-1,11):
            x=col*36+(18 if row%2 else 0)
            a.rect(x,y,34,16,['#211a30','#241c34','#1e192c'][(col+row)%3]); a.rect(x+2,y+1,30,1,'#2a213b')
    # Central recessed vault and stepped arch have broad, low contrast clusters.
    a.poly([(98,130),(98,59),(108,59),(108,45),(124,45),(124,37),(196,37),(196,45),(212,45),(212,59),(222,59),(222,130)],'#2c233e')
    a.poly([(105,132),(105,62),(115,62),(115,51),(130,51),(130,44),(190,44),(190,51),(205,51),(205,62),(215,62),(215,132)],'#100e19')
    for x in [22,270]:
        a.rect(x,25,28,110,'#171322'); a.rect(x+3,28,21,103,'#30253f'); a.rect(x+7,30,4,100,'#392d49'); a.rect(x-3,21,34,7,'#41334f'); a.rect(x-5,131,38,8,'#392c47')
        for y in [47,76,105]: a.rect(x+3,y,21,2,'#1e182a')
    for x,stop in [(68,68),(247,82)]:
        for y in range(0,stop,6):
            a.rect(x,y,3,5,'#504052'); a.rect(x+1,y+1,1,3,'#171321')
    for x in [57,258]:
        a.rect(x-7,67,19,28,'#2e2332'); a.rect(x-3,72,11,19,'#513135'); a.rect(x,85,3,15,'steel'); a.rect(x-2,83,7,3,'gold'); a.poly([(x-1,82),(x-2,77),(x+2,68),(x+5,76),(x+4,82)],'#b07747'); a.rect(x+1,76,2,6,'light')
    a.rect(0,139,320,41,'#1b1627'); a.rect(0,139,320,2,'#49364d')
    for row,y in enumerate([143,155,171]):
        for x in range(-30,320,56):
            xx=x+(24 if row%2 else 0); a.rect(xx,y,53,1,'#35283f'); a.rect(xx+53,y,2,12,'#110f1b')
    a.im.save(ROOT/'artwork/01-backdrop.png')
    (ROOT/'source/dungeon.svg').write_text('<svg xmlns="http://www.w3.org/2000/svg" width="320" height="180" shape-rendering="crispEdges">'+''.join(a.svg)+'</svg>')
    for name,path,rect in [('hero','Assets/Art/male_hero/individual_sheets/male_hero-idle.png',(0,0,128,128)),('opponent','Assets/Art/EndlessSkeletons/sword/Idle.png',(0,0,96,64)),('captain','Assets/Art/EndlessSkeletons/sword-shield/Idle.png',(0,0,96,64))]:
        if (REPO/path).exists():
            src=Image.open(REPO/path).convert('RGBA').crop(rect); src=src.crop(src.getbbox())
            src.putdata([px if px[3] else (0,0,0,0) for px in src.get_flattened_data()]); src.save(ROOT/'artwork'/f'{name}-existing.png')
    layer=Image.new('RGBA',(320,180)); actor(layer,238,140); layer.save(ROOT/'artwork/02-character.png')
    a=Art(320,180)
    for x in [0,299]: a.poly([(x,180),(x+3,165),(x+9,161),(x+16,170),(x+21,180)],'shadow')
    a.im.save(ROOT/'artwork/03-foreground.png')

def actor(im,x,y,enemy=False,boss=False):
    src=Image.open(ROOT/'artwork'/('captain-existing.png' if boss else 'opponent-existing.png' if enemy else 'hero-existing.png')).convert('RGBA')
    if enemy: src=src.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    paste(im,src,x,y-src.height)
def background(): return Image.open(ROOT/'artwork/01-backdrop.png').convert('RGBA')
def stretch(a,w,h,b=6):
    assert w>=2*b and h>=2*b
    out=Image.new('RGBA',(w,h)); sw,sh=a.size
    for sx,ex,dx,fx in [(0,b,0,b),(b,sw-b,b,w-b),(sw-b,sw,w-b,w)]:
        for sy,ey,dy,fy in [(0,b,0,b),(b,sh-b,b,h-b),(sh-b,sh,h-b,h)]:
            paste(out,a.crop((sx,sy,ex,ey)).resize((fx-dx,fy-dy),Image.Resampling.NEAREST),dx,dy)
    return out
def box(im,x,y,w,h,n='panel-normal'): paste(im,stretch(load(n),w,h),x,y)
def button(im,x,y,w,label,primary=False,state='normal'):
    box(im,x,y,w,19,f'button-{"primary" if primary else "secondary"}-{state}')
    text(im,x+w//2,y+4+(state=='pressed'),label,10,'disabled' if state=='disabled' else 'ivory',True,w-14)
def heading(im,title,x=160,y=20,w=150):
    box(im,x-w//2,y,w,20); text(im,x,y+3,title,14,'light',True,w-14)

def menu(error=False,long=False):
    im=background(); paste(im,load('logo-main'),20,9); actor(im,238,140)
    box(im,16,51,159,98); text(im,25,57,'PLAYER NAME',8,'muted')
    box(im,24,68,143,20,'input-error' if error else 'input-focus'); text(im,32,73,'WWWWWWWWWW' if long else 'Your name',10,limit=125)
    text(im,25,90,'Difficulty: < Normal >',10)
    button(im,24,105,143,'PLAY [ENTER]',True)
    if error:
        text(im,25,127,'! Please enter your name',8,'error'); text(im,25,136,'(3-10 characters).',8,'error')
    else: text(im,25,130,'Parry. Type during recovery.',8,'muted',limit=141)
    for x,w,s in [(16,100,'How to Play'),(119,80,'Tutorial'),(202,102,'Collection')]: button(im,x,153,w,s)
    text(im,190,28,'MUSIC  -----o---',8,'muted'); text(im,190,39,'SFX    ------o--',8,'muted')
    return im

def bar(im,x,y,kind,ratio):
    paste(im,load('icon-'+('hp' if kind=='hp' else 'stamina' if kind=='stamina' else 'combo')),x,y-2)
    paste(im,load('bar-'+kind+'-track'),x+11,y)
    if ratio>0: paste(im,load('bar-'+kind+'-fill').crop((0,0,max(1,round(80*ratio)),3)),x+13,y+2)
def hud(boss=False,slayer=False,stress=False,feedback='normal',hp=.36):
    im=background(); box(im,3,3,106,39); box(im,211,3,106,39)
    text(im,9,7,'WWWWWWWWWW' if stress else 'HERO',8,limit=63); text(im,76,7,'1/100' if stress else '85/100',8,'error' if stress else 'ivory')
    bar(im,8,19,'hp',.01 if stress else .85); bar(im,8,30,'combo',.72)
    text(im,219,7,'CAPTAIN' if boss else 'SWORDSMAN',8); text(im,279,7,'200/550' if boss else '80/100',8)
    if boss:
        paste(im,load('boss-track'),216,14); paste(im,load('boss-fill').crop((0,0,max(1,round(68*hp)),3)),229,21); paste(im,load('boss-ticks'),229,21); paste(im,load('captain-badge'),256,10)
        bar(im,216,32,'stamina',.33)
    else: bar(im,216,19,'hp',.8); bar(im,216,30,'stamina',.33)
    text(im,160,5,'SCORE',8,'muted',True); text(im,160,14,'2,147,483,647' if stress else '012,840',12,'light',True,98)
    text(im,160,29,'999 / x16' if stress else '24 COMBO / x4',8,'light',True,98)
    text(im,160,48,'SLAYER / x16' if slayer else 'ROUND 12 / NORMAL',10,'light',True)
    if slayer: paste(im,load('slayer-badge'),152,61)
    actor(im,90,138); actor(im,203,138,True,boss)
    paste(im,Image.open(ROOT/'artwork/03-foreground.png'),0,0)
    box(im,81,136,158,42,'typing-error' if feedback=='error' else 'typing-selected' if slayer or feedback=='correct' else 'typing-normal')
    text(im,160,141,'! MISS / TRY AGAIN' if feedback=='error' else 'CORRECT' if feedback=='correct' else 'TYPE / ATTACK',8,'error' if feedback=='error' else 'light',True)
    text(im,160,150,'counterstrike' if stress else 'resolve',16,'ivory',True,140)
    text(im,160,167,'NEXT courage / honor',8,'muted',True)
    text(im,47,146,'PARRY',10,'light',True); paste(im,load('keycap-space'),35,160)
    text(im,272,146,'00:42',10,center=True); paste(im,load('icon-pause'),267,163)
    return im

def overlay(title,lines,actions):
    im=hud(); im.alpha_composite(Image.new('RGBA',im.size,(9,8,17,210)))
    box(im,48,18,224,148,'modal-normal'); heading(im,title,y=23,w=210)
    for i,line in enumerate(lines): text(im,160,51+i*12,line,10,'muted',True,207)
    for i,label in enumerate(actions): button(im,66,99+i*21,188,label,i==0)
    return im

def collection(mode,status='ready'):
    im=background(); box(im,6,4,308,172,'modal-normal'); heading(im,'COLLECTION',y=8,w=144); text(im,300,12,'X',10)
    for i,s in enumerate(['Endless','Classic','Opponents','Achievements']):
        x=12+i*75; box(im,x,34,73,18,'tab-selected' if s==mode else 'tab-normal'); text(im,x+36,38,s,10,center=True,limit=63)
    if mode=='Endless': text(im,16,56,'DIFFICULTY < Normal >',8,'muted')
    if status!='ready':
        text(im,160,88,{'loading':'Loading...','empty':'No scores yet.','error':'! Could not load leaderboard.'}[status],12,'error' if status=='error' else 'ivory',True,280)
        text(im,160,109,'Complete a run to submit your score.' if status=='empty' else 'Close and reopen Collection to retry.' if status=='error' else 'Fetching scores...',8,'muted',True,280)
    elif mode in ['Endless','Classic']:
        text(im,16,69,'#  PLAYER',8,'muted'); text(im,180,69,'ROUND/KILLS/SCORE' if mode=='Endless' else 'SCORE',8,'muted')
        for i in range(4):
            y=81+i*19; box(im,12,y,287,18,'leaderboard-row-selected' if i==1 else 'leaderboard-row-normal')
            text(im,19,y+4,str(i+1),10,'light'); text(im,35,y+4,'WWWWWWWWWW' if i==1 else ['Ronin','Jade','Ember','Ash'][i],10,limit=135)
            text(im,181,y+5,f'{98-i}/550/2,147,483,647' if mode=='Endless' else '2,147,483,647',8,limit=109)
        paste(im,load('scrollbar-normal'),302,83)
    elif mode=='Opponents':
        for i,label in enumerate(['SWORDSMAN','DOUBLE STRIKE','HEAVY','BERSERKER','GUARD','CAPTAIN']):
            x=12+(i%3)*99; y=57+(i//3)*52; box(im,x,y,97,49,'opponent-card-disabled' if i==5 else 'opponent-card-normal')
            text(im,x+8,y+7,'???' if i==5 else label,10,'muted' if i==5 else 'light',limit=82)
            paste(im,load('icon-lock' if i==5 else 'icon-sword'),x+8,y+23)
            text(im,x+23,y+23,'NOT SEEN' if i==5 else 'SEEN 12',8,'muted'); text(im,x+23,y+33,'Discover' if i==5 else 'DEFEATED 8',8,'muted')
    else:
        for i,(name,desc) in enumerate([('FIRST CAPTAIN','Defeat your first boss.'),('THREE PARRIES','Parry every strike in a triple chain.'),('UNTOUCHED','Defeat a boss without losing HP.')]):
            y=57+i*34; box(im,12,y,293,32,'achievement-card-selected' if i==0 else 'achievement-card-disabled')
            paste(im,load('icon-trophy' if i==0 else 'icon-lock'),21,y+10); text(im,39,y+5,name,10,'light' if i==0 else 'muted'); text(im,39,y+18,desc,8,'muted',limit=256)
    text(im,16,164,'ESC CLOSE',8,'muted'); return im

def screens():
    out={'menu':menu(),'menu-name-error':menu(True),'menu-long-name':menu(long=True),'hud-normal':hud(),'hud-boss':hud(True),'hud-slayer':hud(True,True),'hud-stress-low-hp':hud(True,True,True),'hud-correct':hud(feedback='correct'),'hud-error':hud(feedback='error')}
    for name,title,lines,actions in [
        ('pause','PAUSED',[],['Resume','Restart [R]','Main Menu [M]']),
        ('results-defeat','DEFEAT',['SCORE 2,147,483,647 / TIME 428.0s','ACCURACY 98.4% / PARRIES 128','NORMAL / CLEARED 24'],['Restart [R]','Main Menu [M]']),
        ('results-victory','THANKS FOR PLAYING',['SCORE 012,840 / TIME 42.0s','ACCURACY 98.4% / PARRIES 12'],['Restart [R]','Main Menu [M]']),
        ('continue','ROUND CLEARED',['NORMAL / CLEARED 12 / HP 85/100','NEXT ROUND 13 / SWORDSMAN','Read each cue. Watch for thrusts.'],['Continue [ENTER]']),
        ('how-to-play','HOW TO PLAY',['Type the word to attack.','Space: parry when the cue appears.','Type during enemy recovery.'],['Continue']),
        ('tutorial-passed','PARRY / COMPLETE',['Good timing. Next lesson is ready.'],['Next [ENTER]']),
        ('tutorial-complete','TRAINING COMPLETE',['Combine typing and parrying.'],['Start Battle']),
        ('tutorial-retry','ACCURACY / TRY AGAIN',['Mistakes reset the word.','Finish before time runs out.'],['Practice [ENTER]'])]: out[name]=overlay(title,lines,actions)
    for i,(title,line) in enumerate([('ATTACK','Type BLADE to attack.'),('ACCURACY','Mistakes reset the word.'),('PARRY','Press SPACE on the yellow cue.'),('STAMINA','One more parry breaks its stamina.'),('SLAYER','Each new letter strikes. Type FIRE.'),('COMBINE','Type. Watch the cue. Parry.')]): out['tutorial-'+title.lower()]=overlay(f'{i+1}/6 / {title}',[line],['Practice [ENTER]'])
    for mode in ['Endless','Classic','Opponents','Achievements']: out['collection-'+mode.lower()]=collection(mode)
    for state in ['loading','empty','error']: out['collection-'+state]=collection('Endless',state)
    for n,im in out.items():
        for scale in [1,2,4]: im.resize((320*scale,180*scale),Image.Resampling.NEAREST).save(ROOT/f'screens/{320*scale}x{180*scale}'/f'{n}.png')
    return out

def deliver(previews):
    atlas=Image.new('RGBA',(1024,1024)); x=y=3; rh=0
    for item in assets:
        im=load(item['id']); w,h=im.size
        if x+w+3>1024: x=3; y+=rh+6; rh=0
        assert y+h+3<1024
        # 2px extrusion, including corner texels; transparent corners remain zero.
        for yy in range(-2,h+2):
            for xx in range(-2,w+2): atlas.putpixel((x+xx,y+yy),im.getpixel((max(0,min(w-1,xx)),max(0,min(h-1,yy)))))
        item['atlas']=dict(file='sheets/ui-atlas.png',rect_top_left=[x,y,w,h],extrude=2)
        x+=w+6; rh=max(rh,h)
    atlas.save(ROOT/'sheets/ui-atlas.png')
    for kind,items,cw,ch,cols in [('assets',[(a['id'],load(a['id'])) for a in assets],180,80,5),('screens',list(previews.items()),320,200,3)]:
        sheet=Image.new('RGBA',(cw*cols,math.ceil(len(items)/cols)*ch),P['coal'])
        for i,(n,im) in enumerate(items):
            x=i%cols*cw; y=i//cols*ch; text(sheet,x+4,y+2,n,10)
            if kind=='assets':
                d=ImageDraw.Draw(sheet)
                for cy in range(y+16,y+ch,4):
                    for cx in range(x,x+cw,4): d.rectangle((cx,cy,cx+3,cy+3),fill='#30273c' if (cx//4+cy//4)%2 else '#201b2a')
            paste(sheet,im,x,y+18)
        sheet.save(ROOT/f'contact-sheet-{kind}.png')
    frames=[]; bg=background(); target=previews['menu']
    def reveal(layer,p):
        out=layer.copy(); out.putalpha(out.getchannel('A').point(lambda a:round(a*max(0,min(1,p)))))
        return out
    for i in range(60):
        t=i*.04; im=bg.copy()
        char=Image.open(ROOT/'artwork/02-character.png').convert('RGBA'); fg=Image.open(ROOT/'artwork/03-foreground.png').convert('RGBA')
        paste(im,reveal(char,(t-.4)/.45),0,0); paste(im,reveal(fg,(t-.4)/.45),0,0)
        paste(im,reveal(load('logo-main'),(t-.85)/.65),20,9)
        if t<.4: im=Image.blend(Image.new('RGBA',im.size,P['shadow']),im,t/.4)
        if t>=1.65: im=Image.blend(im,target,min(1,(t-1.65)/.55))
        frames.append(im.convert('RGB'))
    frames[-1]=target.convert('RGB'); frames[0].save(ROOT/'intro/intro-preview.gif',save_all=True,append_images=frames[1:],duration=40,loop=0,optimize=False)
    metadata=dict(version=2,logical_canvas=[320,180],preview_scales=[1,2,4],palette=P,assets=assets,screens=list(previews),filter='Point',mipmaps=False,wrap='Clamp',coordinate_system='Atlas top-left; borders Unity left,bottom,right,top; pivot normalized',boss_fill=dict(rect=[13,7,68,3],mode='continuous crop',ticks='decorative overlay'),intro_duration_ms=2400)
    (ROOT/'metadata.json').write_text(json.dumps(metadata,indent=2))
    (ROOT/'source/text-layout.json').write_text(json.dumps(checks,indent=2))
    gallery(previews)

def gallery(previews):
    # Reuse gallery interactions only; v2 native art and sizing replace v1 content.
    raw=(ROOT/'source/gallery-template.py').read_text(encoding='utf8'); ns=globals().copy()
    exec(raw,ns); ns['gallery'](previews)
    p=ROOT/'index.html'; s=p.read_text(encoding='utf8').replace('v1','v2').replace('V1','V2').replace('Night shrine','Purple dungeon')
    s=s.replace('#111b20',P['coal']).replace('#192b2c',P['deep']).replace('#528d83',P['gold']).replace('#8ab5a5',P['light'])
    s=s.replace('inset:6.66% auto auto 4.53%;width:45.31%','inset:5% auto auto 6.25%;width:44.375%')
    s=s.replace('<a href="intro/intro-preview.mp4">Clip MP4 / 25 fps</a> · <a href="intro/intro-preview.avi">AVI gốc</a>','')
    # v1's generated UTF-8 text is reused without encoding reinterpretation.
    s=s.replace('640×360 & 1280×720','320×180 / 640×360 / 1280×720')
    s=s.replace("a.finished.then(()=>{q('button').hidden=true})", "a.finished.then(()=>{setTimeout(()=>{if(a.playState==='finished')q('button').hidden=true},200)})")
    # All three native exports available directly from each screen card.
    for n in previews:
        s=s.replace(f'<h3>{n}</h3>',f'<h3>{n}</h3><a href="screens/320x180/{n}.png">1×</a> · <a href="screens/640x360/{n}.png">2×</a> · <a href="screens/1280x720/{n}.png">4×</a>')
    p.write_text(s,encoding='utf8')

def pack():
    with zipfile.ZipFile(ROOT/'typinghero-ui-kit-v2.zip','w',zipfile.ZIP_DEFLATED) as z:
        for p in sorted(ROOT.rglob('*')):
            if p.is_file() and p.suffix!='.zip' and '__pycache__' not in p.parts: z.write(p,p.relative_to(ROOT))

if __name__=='__main__':
    import sys
    if '--pack-only' not in sys.argv:
        controls(); brand(); artwork(); previews=screens(); deliver(previews)
        print(f'Built {len(assets)} assets; {len(previews)} screens at 1x/2x/4x.')
    pack()
