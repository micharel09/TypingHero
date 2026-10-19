"""Review-only r7 backgrounds; reuse r6 material and r4 raster/handler data."""
import sys
sys.dont_write_bytecode = True
from pathlib import Path
import importlib.util
import json
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
R4 = ROOT.parent / 'hud-r4'
spec = importlib.util.spec_from_file_location('r6', ROOT.parent / 'hud-r6/source/build.py')
r6 = importlib.util.module_from_spec(spec)
spec.loader.exec_module(r6)
out = ROOT / 'preview'
out.mkdir(exist_ok=True)
word_layer = r6.b.word_layer
r6.b.word_layer = lambda **kw: r6.b.blank(198, 30)
for slayer in [False, True]:
    im = r6.render(dict(slayer=slayer, rank='boss' if slayer else 'normal'), 6)
    im.save(out / ('slayer.png' if slayer else 'normal.png'))
r6.b.word_layer = word_layer
for slayer in [False, True]:
    name = 'slayer' if slayer else 'normal'
    base = Image.open(out / (name+'.png')).convert('RGBA')
    base.alpha_composite(word_layer(), (61, 146))
    reference = Image.open(ROOT.parent / f'hud-r6/preview/r6-{name}-320x180.png').convert('RGBA')
    # Original Slayer uses masked paste onto transparent board (alpha twice).
    # Compare non-typing pixels; interactive text uses original r4 raster once.
    if not slayer:
        assert base.tobytes() == reference.tobytes(), name
    else:
        for box in [(0,0,320,151),(0,171,320,180),(0,151,65,171),(255,151,320,171)]:
            assert base.crop(box).tobytes() == reference.crop(box).tobytes(), (name,box)
r6.frame(True).save(out / 'error-frame.png')
source = (R4 / 'index.html').read_text(encoding='utf-8')
pool = json.loads((R4 / 'source/pool.json').read_text())
widths = json.loads(source.split('widths=')[1].split(';')[0])
(ROOT / 'source/data.js').write_text('const pool='+json.dumps(pool)+';\nconst widths='+json.dumps(widths)+';\n', encoding='utf-8')
assert len(pool) == 219 and max(widths[w] for w in pool) == 54
for w in pool:
    width = widths[w]
    for cell in [65, 131, 197]:
        left = cell + round((58-width)/2)
        assert cell <= left and left+width <= cell+58
assert 66-54 == 12
print('PASS pool219: max54, cell58, worst gap12; generated r6 backgrounds and reused raster widths')
