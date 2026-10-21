"""Reconstruct the visible specimen glyphs; this is not the original Blackbit OTF."""
from pathlib import Path
import hashlib
import json
import unicodedata
from PIL import Image, ImageDraw, ImageFont
from fontTools.fontBuilder import FontBuilder
from fontTools.pens.ttGlyphPen import TTGlyphPen
from fontTools.ttLib import TTFont

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

def trace_mask(mask, baseline, pen, bearing=0):
 # Remove shared pixel edges: internal row contours cause rasterization seams.
 edges = set()
 for y in range(mask.height):
  for x in range(mask.width):
   if not mask.getpixel((x,y)): continue
   bottom = baseline-y-1
   corners = [(x+bearing,bottom),(x+bearing,bottom+1),(x+bearing+1,bottom+1),(x+bearing+1,bottom)]
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
  if chars == 'rstuvwxyz': baseline = 470
  for char, (left, right) in zip(chars, boxes):
   # The earlier reduced screenshot rounded off some glyph-edge pixels.
   # Recover the full foreground inside the clear inter-glyph margins.
   crop_left,crop_top = max(0,left-4),max(0,top-4)
   crop_bottom = 394 if char == 'f' else min(source.height,bottom+4)
   if char == 't': crop_top = 396
   crop = source.crop((crop_left,crop_top,min(source.width,right+4),crop_bottom))
   mask = Image.new('L', crop.size)
   mask.putdata([255 if g >= 170 and g >= r + 25 and g >= b + 70 else 0
                 for r, g, b in crop.get_flattened_data()])
   bounds = mask.getbbox()
   assert bounds, char
   mask = mask.crop(bounds)
   glyph_left,glyph_top = crop_left+bounds[0],crop_top+bounds[1]
   masks[char] = mask
   metadata[char] = dict(source_rect=[glyph_left,glyph_top,mask.width,mask.height],
                         baseline=baseline-glyph_top,
                         bearing_left=3 if char in 'ijlI1' else 4,
                         bearing_right=3 if char in 'fjltI1' else 4)
   metadata[char]['advance'] = mask.width + metadata[char]['bearing_left'] + metadata[char]['bearing_right']
   metadata[char]['source_to_font'] = [16,0,0,-16,
       (metadata[char]['bearing_left']-glyph_left)*16,baseline*16]
   metadata[char]['mask_sha256'] = hashlib.sha256(mask.tobytes()).hexdigest()
 # The specimen's punctuation display row does not supply usable math-sign
 # metrics. Keep each mask, but center these signs on the measured x-height
 # (54 source pixels), rather than making +100 look like a superscript.
 for char in '+<>':
  info = metadata[char]
  info['specimen_row_baseline'] = info['baseline']
  info['baseline'] = round(27 + masks[char].height / 2)
  info['source_to_font'][5] = (info['source_rect'][1] + info['baseline']) * 16
  info['vertical_metric_note'] = 'Inferred math center at half measured x-height; shape unchanged.'
 # Optical anchor for the pictured three-pixel cross strokes. At small physical
 # sizes their center must hit a pixel center; this changes placement, not shape.
 metadata['+']['pixel_snap_anchor'] = [metadata['+']['bearing_left'] + 7.5,
                                      metadata['+']['baseline'] - 10.5]
 chars = ''.join(sorted(masks))
 assert len(chars) == 78
 ascent = max(metadata[c]['baseline']*16 for c in chars)
 descent = max((masks[c].height-metadata[c]['baseline'])*16 for c in chars)
 cap_height = metadata['H']['baseline']*16
 x_height = metadata['x']['baseline']*16
 order = ['.notdef', 'space'] + ['uni%04X' % ord(c) for c in chars]
 fb = FontBuilder(1600, isTTF=True)
 fb.setupGlyphOrder(order)
 fb.setupCharacterMap({32:'space', **{ord(c):'uni%04X'%ord(c) for c in chars}})
 glyphs = {}; metrics = {}
 for name in ['.notdef', 'space']:
  glyphs[name] = TTGlyphPen(None).glyph(); metrics[name] = (384, 0)
 for char in chars:
  mask = masks[char]; info = metadata[char]; pen = TTGlyphPen(None)
  trace_mask(mask,info['baseline'],pen,info['bearing_left'])
  name = 'uni%04X'%ord(char); glyphs[name] = pen.glyph(); metrics[name] = (info['advance']*16, info['bearing_left']*16)
 # A missing glyph must not silently become whitespace. Reuse the supplied
 # question-mark outline; do not invent an additional shape or Unicode mapping.
 glyphs['.notdef'] = glyphs['uni003F']
 metrics['.notdef'] = metrics['uni003F']
 fb.setupGlyf(glyphs); fb.setupHorizontalMetrics(metrics)
 fb.setupHorizontalHeader(ascent=ascent, descent=-descent, lineGap=0)
 fb.setupNameTable(dict(familyName='Clackblade Specimen Pixel',styleName='Regular',
   uniqueFontIdentifier='ClackbladeSpecimenPixel-2.0',fullName='Clackblade Specimen Pixel Regular',
   psName='ClackbladeSpecimenPixel-Regular',version='Version 2.0',
   description='Glyphs traced from user-supplied Blackbit specimen by Angin Studio. Not the original font file.'))
 fb.setupOS2(version=4,sTypoAscender=ascent,sTypoDescender=-descent,sTypoLineGap=0,usWinAscent=ascent,
             usWinDescent=descent,sCapHeight=cap_height,sxHeight=x_height,fsSelection=0xC0)
 fb.setupPost(); fb.setupMaxp(); fb.font.recalcTimestamp=False
 fb.font['head'].created=fb.font['head'].modified=2082844800
 target=OUT/'ClackbladeSpecimenPixel.ttf';fb.save(target)
 # Conservative optical kerning inferred from overlapping scanline profiles.
 # No ligatures; all pair values remain in source-pixel units.
 from fontTools.feaLib.builder import addOpenTypeFeaturesFromString
 profiles = {}
 for c in chars:
  profiles[c] = {}
  for row in range(masks[c].height):
   occupied=[x for x in range(masks[c].width) if masks[c].getpixel((x,row))]
   if occupied: profiles[c][metadata[c]['baseline']-row-1]=(min(occupied),max(occupied))
 pairs = {}
 for a in chars:
  for b in chars:
   gaps=[metadata[a]['advance']+metadata[b]['bearing_left']+profiles[b][y][0]-metadata[a]['bearing_left']-profiles[a][y][1]-1
         for y in profiles[a].keys() & profiles[b].keys()]
   adjustment = -min(4,max(0,min(gaps)-7)) if gaps else 0
   if adjustment: pairs[a+b]=adjustment
 feature='feature kern {\n'+''.join('pos uni%04X uni%04X %d;\n'%(ord(a),ord(b),v*16) for (a,b),v in sorted(pairs.items()))+'} kern;'
 addOpenTypeFeaturesFromString(fb.font,feature)
 from fontTools.ttLib import newTable
 from fontTools.ttLib.tables._k_e_r_n import KernTable_format_0
 kern=newTable('kern');kern.version=0
 sub=KernTable_format_0();sub.version=0;sub.coverage=1
 sub.kernTable={('uni%04X'%ord(a),'uni%04X'%ord(b)):v*16 for (a,b),v in pairs.items()}
 kern.kernTables=[sub];fb.font['kern']=kern
 fb.save(target)
 # Check against the whole reference, so an accidentally tight crop cannot
 # validate itself while silently omitting a glyph edge or descender.
 reference = {(x,y) for y in range(source.height) for x in range(source.width)
              for r,g,b in [source.getpixel((x,y))]
              if g >= 170 and g >= r+25 and g >= b+70}
 covered = set()
 rendered_font = ImageFont.truetype(str(target),100)
 for char,mask in masks.items():
  left,top,_,_ = metadata[char]['source_rect']
  covered.update((left+x,top+y) for y in range(mask.height) for x in range(mask.width)
                 if mask.getpixel((x,y)))
  rendered = Image.new('L',(200,200))
  ImageDraw.Draw(rendered).text((30,130),char,font=rendered_font,fill=255,anchor='ls')
  expected=Image.new('L',rendered.size)
  expected.paste(mask,(30+metadata[char]['bearing_left'],130-metadata[char]['baseline']))
  assert rendered == expected, 'Origin/baseline raster mismatch: '+char
  assert rendered_font.getlength(char) == metadata[char]['advance'], 'Advance mismatch: '+char
 assert covered == reference, ('Reference pixels omitted/added',len(reference-covered),len(covered-reference))
 atlas=Image.new('RGBA',(768,1280))
 for i,c in enumerate(chars):
  x=(i%8)*96+4;y=(i//8)*128+88-metadata[c]['baseline']
  tile=Image.new('RGBA',masks[c].size,'white');tile.putalpha(masks[c]);atlas.alpha_composite(tile,(x,y))
  metadata[c]['atlas_rect']=[x,y,masks[c].width,masks[c].height]
 atlas.save(OUT/'specimen-atlas.png')
 fallback_path = ROOT/'Assets/Art/UI/Pixel/VT323-Regular.ttf'
 fallback_characters = ''.join(chr(c) for c in sorted(TTFont(fallback_path).getBestCmap())
                               if not unicodedata.category(chr(c)).startswith('C'))
 (OUT/'glyphs.json').write_text(json.dumps(dict(family='Clackblade Specimen Pixel',
  specimen_sha256=hashlib.sha256(SOURCE.read_bytes()).hexdigest(),font_sha256=hashlib.sha256(target.read_bytes()).hexdigest(),
  version=2,units_per_em=1600,source_pixel_units=16,
  primary_characters=' '+''.join(chars),
  fallback=dict(source='Assets/Art/UI/Pixel/VT323-Regular.ttf',
                sha256=hashlib.sha256(fallback_path.read_bytes()).hexdigest(),
                characters=fallback_characters,
                effective_characters=''.join(c for c in fallback_characters if c not in ' '+''.join(chars)),
                letter_scale=1.35,slash_scale=0.76),
  metrics=dict(ascent=ascent,descent=descent,cap_height=cap_height,x_height=x_height,line_gap=0),
  reconstruction_notes=['Source bounds measured from screenshot; not original font metrics.',
   't ascender starts at y396; f ends at y393. They must never share a crop.',
   '.notdef reuses the pictured question mark; no unsupported character is mapped to space.',
   '+, < and > retain their masks but use an inferred x-height-centered vertical position.',
   'Side bearings and optical kerning inferred; no original spacing data supplied.'],
  kerning_source_pixels=pairs,qa_glyphs=[dict(character=c,**metadata[c]) for c in chars],glyphs=metadata),indent=2)+'\n')
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
