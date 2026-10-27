"""Offline stone-plane recipe. Default is read-only; --write authors Floor1..8 normals.

URP tangent normal RGB; A=255. Surface masks remain R=0 metal, A=smoothness,
with material scalar .5. Broad tops/mortar stay flat; chipped rims lean gently
away from neighboring stone. No albedo luminance is interpreted as height.
"""
import argparse
import hashlib
import json
import math
from pathlib import Path
import re
from uuid import UUID, uuid5
from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
ART = ROOT / 'Assets/Art/Environment/PixelDungeon'
TOP, BEVEL, MORTAR = (72,59,58), (119,92,85), (34,34,34)
BEVEL_SLOPE = .16
FACE_SMOOTHNESS_BYTES = (184,168,176,156,188,162,172,160)

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--write', action='store_true')
    args = parser.parse_args()
    template = (ROOT/'Assets/Art/Lighting/Normals/751874d2767140c29532b8eaf0124627.png.meta').read_text()
    rows = []
    for i in range(1,9):
        source, target = ART/f'floor_{i}.png', ART/f'Floor{i}Normal.png'
        image = Image.open(source).convert('RGB')
        assert set(image.getdata()) == {TOP, BEVEL, MORTAR}
        pix = image.load()
        out = Image.new('RGBA', image.size, (128,128,255,255))
        for y in range(image.height):
            for x in range(image.width):
                if pix[x,y] != BEVEL: continue
                # Periodic neighbor lookup keeps the repeating tile seam coherent.
                def stone(dx,dy):
                    return pix[(x+dx)%image.width,(y+dy)%image.height] == TOP
                nx = BEVEL_SLOPE*(stone(-1,0)-stone(1,0))
                ny = BEVEL_SLOPE*(stone(0,1)-stone(0,-1))
                length = math.sqrt(1+nx*nx+ny*ny)
                out.putpixel((x,y), tuple(round((v/length*.5+.5)*255) for v in (nx,ny,1))+(255,))
        meta = Path(str(target)+'.meta')
        guid = re.search(r'^guid: (.+)$',meta.read_text(),re.M).group(1) if meta.exists() else uuid5(UUID('d915250d-e107-46c2-956c-9967fe132ab8'),target.name).hex
        if args.write:
            out.save(target)
            if not meta.exists():
                meta.write_text(re.sub(r'(?m)^guid: .*$', 'guid: '+guid, template),newline='\n')
            material = ART/f'Floor{i}.mat'
            data = material.read_text()
            if '  - _NORMALMAP' not in data:
                data=data.replace('  m_ValidKeywords:\n', '  m_ValidKeywords:\n  - _NORMALMAP\n')
            data=data.replace('    - _BumpMap:\n        m_Texture: {fileID: 0}', '    - _BumpMap:\n        m_Texture: {fileID: 2800000, guid: '+guid+', type: 3}')
            material.write_text(data,newline='\n')
        import io
        encoded=io.BytesIO();out.save(encoded,format='PNG')
        surface=ART/f'Floor{i}Surface.png'
        rows.append({'source':source.relative_to(ROOT).as_posix(),'source_sha256':hashlib.sha256(source.read_bytes()).hexdigest(),
                     'normal':target.relative_to(ROOT).as_posix(),'normal_sha256':hashlib.sha256(encoded.getvalue()).hexdigest(),
                     'surface':surface.relative_to(ROOT).as_posix(),'surface_sha256':hashlib.sha256(surface.read_bytes()).hexdigest(),
                     'surface_alpha':{'stone_top':FACE_SMOOTHNESS_BYTES[i-1],'chipped_bevel':72,'mortar':6},'normal_guid':guid})
    print(json.dumps({'written':args.write,'normal_channels':'RGB tangent normal; A=255','bevel_slope':BEVEL_SLOPE,
                      'surface_channels':'R metallic=0; A smoothness; G/B=0','smoothness_scalar':.5,'tiles':rows},indent=2))

if __name__ == '__main__': main()
