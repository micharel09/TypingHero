"""Explicit offline normal authoring recipe; requires the existing Pillow tooling.

Run with --write to replace the existing actor normal PNGs in place. The runtime
and builders never call this tool. Albedo, sprite metadata and GUIDs are untouched.
Normals use semantic material regions and their silhouettes, never luminance as height.
"""
import argparse
import json
import math
from pathlib import Path
import re
from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
# Artist controls: curvature in X/Y and a broad plane bias in X/Y.
FORMS = {
    'outline': (0, 0, 0, 0),
    'skin': (.65, .4, .05, .05),
    'cloth': (.45, .25, 0, 0),
    'steel': (.25, .15, .08, .18),
    'bone': (.6, .4, 0, .05),
    'wood': (.45, .2, 0, 0),
    'accent': (0, 0, 0, 0),
}
BONE = {(250,250,250), (192,213,220), (160,172,197), (105,121,158),
        (191,203,214), (139,152,170), (103,108,134)}
WOOD = {(73,16,9), (112,39,7), (49,4,4), (95,26,9)}

def role(color, hero):
    r,g,b,a = color
    if a < 128: return 'transparent'
    if max(r,g,b) < 65 and (not hero or r == g == b == 0): return 'outline'
    if not hero:
        if (r,g,b) in BONE: return 'bone'
        if (r,g,b) in WOOD: return 'wood'
        return 'steel' if max(r,g,b)-min(r,g,b) < 35 else 'bone'
    if r > g*1.08 and g > b*1.03 and g > 95: return 'skin'
    if r > g*1.3 and r > b*1.2: return 'cloth'
    if g > r*1.2 or b > r*1.3: return 'accent'
    return 'steel' if max(r,g,b) > 125 else 'cloth'

def author(source):
    meta = Path(str(source)+'.meta').read_text()
    guid = re.search(r'^guid: (.+)$',meta,re.M).group(1)
    target = ROOT/'Assets/Art/Lighting/Normals'/(guid+'.png')
    if not target.is_file(): return None
    rects = re.findall(r'      rect:\n        serializedVersion: 2\n        x: ([0-9]+)\n        y: ([0-9]+)\n        width: ([0-9]+)\n        height: ([0-9]+)',meta)
    if not rects: raise ValueError('Missing sprite rectangles: '+str(source))
    albedo = Image.open(source).convert('RGBA'); pix = albedo.load()
    output = Image.new('RGBA',albedo.size,(128,128,255,255)); out = output.load()
    hero = 'male_hero' in str(source)
    for values in rects:
        x0,y0,w,h = map(int,values); top = albedo.height-y0-h
        labels = {(x,y):role(pix[x,y],hero) for y in range(top,top+h) for x in range(x0,x0+w)}
        for (x,y), label in labels.items():
            if label == 'transparent': continue
            sx,sy,bx,by = FORMS[label]
            def distance(dx,dy):
                d=1
                while d < 16 and labels.get((x+dx*d,y+dy*d)) == label: d+=1
                return d
            left,right,up,down = distance(-1,0),distance(1,0),distance(0,-1),distance(0,1)
            nx = bx + sx*(left-right)/(left+right)
            ny = by + sy*(down-up)/(down+up)*min(1,(left+right)/(down+up))
            # Broad planes, not a rounded bevel around every painted pixel.
            nx=round(nx*8)/8;ny=round(ny*8)/8
            length=math.sqrt(nx*nx+ny*ny+1)
            out[x,y]=(round((nx/length*.5+.5)*255),round((ny/length*.5+.5)*255),round((1/length*.5+.5)*255),255)
    return target,output,len(rects)

def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--write',action='store_true');args=parser.parse_args()
    results=[]
    for meta in sorted((ROOT/'Assets/Art').rglob('*.png.meta')):
        if not any(k in str(meta).lower() for k in ('male_hero','skeleton')): continue
        if '_NormalMap' not in meta.read_text(errors='replace'): continue
        source=Path(str(meta)[:-5]); result=author(source)
        if result is None: continue
        target,image,count=result
        if args.write: image.save(target)
        results.append({'source':str(source.relative_to(ROOT)), 'normal':str(target.relative_to(ROOT)), 'rectangles':count})
    print(json.dumps({'written':args.write,'atlases':results},indent=2))

if __name__ == '__main__': main()
