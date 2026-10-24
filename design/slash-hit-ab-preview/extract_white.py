"""Extract the approved white B preview into lossless native RGBA cels.

Run build.py first, then this script. No paid download or artwork reconstruction.
"""
import base64
import hashlib
import io
import json
from pathlib import Path

from PIL import Image

from build import REVIEWED_HASHES

ROOT = Path(__file__).resolve().parent


def main():
    html = (ROOT / 'local/preview.html').read_text(encoding='utf-8')
    data = json.JSONDecoder().raw_decode(html.split('const DATA = ', 1)[1])[0]['b']
    assert data['sha256'] == REVIEWED_HASHES['b']
    assert data['size'] == [1260, 1000]
    assert [f['ms'] for f in data['frames']] == [100, 100, 100, 100, 80, 100]
    sheet = Image.new('RGBA', (384, 64), (255, 255, 255, 0))
    for i, frame in enumerate(data['frames']):
        image = Image.open(io.BytesIO(base64.b64decode(frame['src'].split(',')[1])))
        tile = image.convert('RGB').crop((640, 640, 896, 896))
        assert {color for _, color in tile.getcolors(65536)} <= {(0, 0, 0), (255, 255, 255)}
        native = tile.resize((64, 64), Image.Resampling.NEAREST)
        assert native.resize((256, 256), Image.Resampling.NEAREST).tobytes() == tile.tobytes()
        rgba = Image.new('RGBA', (64, 64), (255, 255, 255, 255))
        rgba.putalpha(native.getchannel('R'))
        sheet.paste(rgba, (i * 64, 0))
    target = ROOT.parents[1] / 'Assets/Art/VFX/SlayerHits/metpxl-white.png'
    sheet.save(target)
    print(target, hashlib.sha256(target.read_bytes()).hexdigest())


if __name__ == '__main__':
    main()
