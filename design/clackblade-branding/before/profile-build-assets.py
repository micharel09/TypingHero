from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageEnhance
root=Path(__file__).resolve().parents[3]; out=root/'marketing/itch-io/profile'
icon=Image.open(root/'design/ui-kit/typinghero/v2/png/icon-sword.png').convert('RGBA'); icon=icon.crop(icon.getbbox()); scale=176/max(icon.size); icon=icon.resize((round(icon.width*scale),round(icon.height*scale)),Image.Resampling.NEAREST)
a=Image.new('RGBA',(256,256),'black'); a.alpha_composite(icon,((256-icon.width)//2,(256-icon.height)//2)); a.convert('RGB').save(out/'avatar.png')
b=Image.new('RGBA',(960,300),'#0B0B0F')
for name in ['scene-far','scene-mid','scene-floor','scene-foreground']:
 im=Image.open(root/f'Assets/Art/UI/CombatR7/{name}.png').convert('RGBA')
 print(name,im.size)
 # Uniform scale and centered crop; nearest-neighbor keeps sprite aspect.
 scale=max(960/im.width,300/im.height); im=im.resize((round(im.width*scale),round(im.height*scale)),Image.Resampling.NEAREST)
 b.alpha_composite(im,((960-im.width)//2,(300-im.height)//2))
b=ImageEnhance.Brightness(b).enhance(.38)
d=ImageDraw.Draw(b); font=str(root/'Assets/Art/UI/Pixel/VT323-Regular.ttf')
for text,y,size,col in [('CLACKBLADE',82,128,'#E6DDCC'),('TYPE TO SLASH',216,32,'#C46B65')]:
 f=ImageFont.truetype(font,size); box=d.textbbox((0,0),text,font=f); x=(960-(box[2]-box[0]))//2
 d.text((x,y-box[1]),text,font=f,fill=col,stroke_width=2 if size>50 else 0,stroke_fill='#0B0B0F')
b.convert('RGB').save(out/'banner.png')
