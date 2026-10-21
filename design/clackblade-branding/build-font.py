"""Reconstruct the visible specimen glyphs; this is not the original Blackbit OTF."""
from pathlib import Path
import hashlib
import json
from PIL import Image, ImageDraw, ImageFont
from fontTools.fontBuilder import FontBuilder
from fontTools.pens.ttGlyphPen import TTGlyphPen

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'Assets/Art/UI/Branding'
SOURCE = Path(__file__).with_name('specimen-v2.png')
ROWS = [
 ('ABCDEFGHIJKLMN',66,115,115,[(140,170),(177,207),(216,242),(249,279),(288,314),(322,348),(355,385),(394,420),(429,440),(448,466),(474,503),(511,535),(542,588),(596,625)]),
 ('OPQRSTUVWXYZ',128,182,176,[(156,184),(193,220),(229,257),(265,298),(306,331),(340,368),(376,403),(411,439),(447,493),(502,532),(539,573),(581,612)]),
 ('abcdefghijklmnopq',193,254,240,[(126,151),(158,186),(193,216),(223,249),(255,280),(289,314),(322,342),(349,375),(383,394),(400,415),(422,449),(456,466),(476,510),(519,541),(551,576),(585,610),(619,641)]),
 ('rstuvwxyz',265,310,302,[(237,260),(267,290),(298,316),(325,351),(357,384),(392,433),(440,464),(472,500),(506,530)]),
 ('1234567890',355,390,390,[(243,253),(263,281),(293,314),(324,346),(354,377),(387,407),(414,434),(442,462),(472,493),(502,523)]),
]

def trace_mask(mask, baseline, pen):
 # Remove shared pixel edges: internal row contours cause rasterization seams.
 edges = set()
 for y in range(mask.height):
  for x in range(mask.width):
   if not mask.getpixel((x,y)): continue
   bottom = baseline-y-1
   corners = [(x,bottom),(x,bottom+1),(x+1,bottom+1),(x+1,bottom)]
   for a,b in zip(corners,corners[1:]+corners[:1]):
    if (b,a) in edges: edges.remove((b,a))
    else: edges.add((a,b))
 outgoing = {}
 for a,b in edges: outgoing.setdefault(a,set()).add(b)
 directions = {(1,0):0,(0,-1):1,(-1,0):2,(0,1):3}
 priority = {1:0,0:1,3:2,2:3}
 while edges:
  start,current = min(edges); points = [start]
  edges.remove((start,current)); outgoing[start].remove(current)
  previous = start
  while current != start:
   points.append(current)
   incoming = directions[(current[0]-previous[0],current[1]-previous[1])]
   def turn(b):
    outgoing_direction = directions[(b[0]-current[0],b[1]-current[1])]
    return priority[(outgoing_direction-incoming)%4],b
   following = min(outgoing[current],key=turn)
   edges.remove((current,following)); outgoing[current].remove(following)
   previous,current = current,following
  corners = []
  for i,b in enumerate(points):
   a,c = points[i-1],points[(i+1)%len(points)]
   if (b[0]-a[0])*(c[1]-b[1]) != (b[1]-a[1])*(c[0]-b[0]): corners.append(b)
  pen.moveTo(tuple(v*16 for v in corners[0]))
  for point in corners[1:]: pen.lineTo(tuple(v*16 for v in point))
  pen.closePath()

def build(keep_wordmark=False):
 OUT.mkdir(parents=True, exist_ok=True)
 source = Image.open(SOURCE).convert('RGB')
 masks = {}
 metadata = {}
 scale = 25/16 # The new 1200x800 reference is the full-resolution 768x512 specimen.
 rows = [(chars,round(top*scale),round(bottom*scale),round(baseline*scale),
          [(round(left*scale),round(right*scale)) for left,right in boxes])
         for chars,top,bottom,baseline,boxes in ROWS]
 rows.append(('~!@#$%^&*()_+<>?',628,710,704,
  [(256,279),(293,308),(324,382),(397,442),(457,485),(500,534),(549,564),(579,625),
   (640,656),(671,694),(708,731),(745,784),(799,816),(830,852),(866,889),(906,940)]))
 for chars, top, bottom, baseline, boxes in rows:
  assert len(chars) == len(boxes)
  for char, (left, right) in zip(chars, boxes):
   crop = source.crop((left, top, right, bottom))
   mask = Image.new('L', crop.size)
   mask.putdata([255 if g >= 170 and g >= r + 25 and g >= b + 70 else 0
                 for r, g, b in crop.get_flattened_data()])
   assert mask.getbbox(), char
   masks[char] = mask
   metadata[char] = dict(source_rect=[left, top, right-left, bottom-top],
                         baseline=baseline-top, advance=right-left+round(6*scale))
 chars = ''.join(sorted(masks))
 assert len(chars) == 78
 order = ['.notdef', 'space'] + ['uni%04X' % ord(c) for c in chars]
 fb = FontBuilder(1600, isTTF=True)
 fb.setupGlyphOrder(order)
 fb.setupCharacterMap({32:'space', **{ord(c):'uni%04X'%ord(c) for c in chars}})
 glyphs = {}; metrics = {}
 for name in ['.notdef', 'space']:
  glyphs[name] = TTGlyphPen(None).glyph(); metrics[name] = (600, 0)
 for char in chars:
  mask = masks[char]; info = metadata[char]; pen = TTGlyphPen(None)
  trace_mask(mask,info['baseline'],pen)
  name = 'uni%04X'%ord(char); glyphs[name] = pen.glyph(); metrics[name] = (info['advance']*16, 0)
 fb.setupGlyf(glyphs); fb.setupHorizontalMetrics(metrics)
 fb.setupHorizontalHeader(ascent=1300, descent=-400)
 fb.setupNameTable(dict(familyName='Clackblade Specimen Pixel',styleName='Regular',
   uniqueFontIdentifier='ClackbladeSpecimenPixel-1.1',fullName='Clackblade Specimen Pixel Regular',
   psName='ClackbladeSpecimenPixel-Regular',version='Version 1.1',
   description='Glyphs traced from user-supplied Blackbit specimen by Angin Studio. Not the original font file.'))
 fb.setupOS2(sTypoAscender=1300,sTypoDescender=-400,sTypoLineGap=0,usWinAscent=1300,
             usWinDescent=400,sCapHeight=1200,sxHeight=850)
 fb.setupPost(); fb.setupMaxp(); fb.font.recalcTimestamp=False
 fb.font['head'].created=fb.font['head'].modified=2082844800
 target=OUT/'ClackbladeSpecimenPixel.ttf';fb.save(target)
 atlas=Image.new('RGBA',(768,1280))
 for i,c in enumerate(chars):
  x=(i%8)*96+4;y=(i//8)*128+88-metadata[c]['baseline']
  tile=Image.new('RGBA',masks[c].size,'white');tile.putalpha(masks[c]);atlas.alpha_composite(tile,(x,y))
  metadata[c]['atlas_rect']=[x,y,masks[c].width,masks[c].height]
 atlas.save(OUT/'specimen-atlas.png')
 (OUT/'glyphs.json').write_text(json.dumps(dict(family='Clackblade Specimen Pixel',
  specimen_sha256=hashlib.sha256(SOURCE.read_bytes()).hexdigest(),font_sha256=hashlib.sha256(target.read_bytes()).hexdigest(),
  units_per_em=1600,source_pixel_units=16,glyphs=metadata),indent=2)+'\n')
 # The shared wordmark is a product asset for marketing and reference review.
 if not keep_wordmark:
  font=ImageFont.truetype(str(target),128);box=font.getbbox('Clackblade')
  logo=Image.new('RGBA',(box[2]-box[0]+8,box[3]-box[1]+8));ImageDraw.Draw(logo).text((4-box[0],4-box[1]),'Clackblade',font=font,fill='#E6DDCC')
  logo.save(OUT/'wordmark.png')
 print('Generated 78 visible glyphs + space:',target)

if __name__ == '__main__':
 import argparse
 parser=argparse.ArgumentParser(description=__doc__)
 parser.add_argument('--keep-wordmark',action='store_true',help='Preserve the existing marketing wordmark export')
 build(parser.parse_args().keep_wordmark)
