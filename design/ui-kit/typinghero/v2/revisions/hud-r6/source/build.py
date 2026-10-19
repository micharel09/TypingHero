"""Static r5/r6 review. Reuse r4 primitives; never export production assets."""
import sys
sys.dont_write_bytecode = True
from pathlib import Path
import importlib.util
from PIL import Image, ImageDraw, ImageChops

ROOT = Path(__file__).resolve().parents[1]
R5 = ROOT.parent / 'hud-r5'
spec = importlib.util.spec_from_file_location('r5', R5 / 'source/propose.py')
r5 = importlib.util.module_from_spec(spec)
spec.loader.exec_module(r5)
b = r5.b
original_load = b.load


def material(name):
    a = original_load(name)
    if name not in ['resource-frame', 'resource-socket', 'elite-socket', 'boss-socket',
                    'secondary-frame', 'secondary-frame-gold', 'secondary-socket',
                    'secondary-socket-gold', 'score-frame']:
        return a
    old = a.copy()
    # Preserve alpha/silhouette. Shade existing metal only, never fills or icons.
    metal = {b.P[k] for k in ['rim', 'metal', 'bevel', 'steel', 'ivory']}
    gold = name.endswith('-gold')
    for y in range(a.height):
        for x in range(a.width):
            px = old.getpixel((x, y))
            if not px[3]:
                continue
            color = '#%02x%02x%02x' % px[:3]
            if color in metal or gold and color == b.P['gold']:
                up = y == 0 or not old.getpixel((x, y-1))[3]
                left = x == 0 or not old.getpixel((x-1, y))[3]
                down = y == a.height-1 or not old.getpixel((x, y+1))[3]
                right = x == a.width-1 or not old.getpixel((x+1, y))[3]
                key = ('gold' if gold else 'metal') if up or left else 'shadow' if down or right else ('gold' if gold else 'bevel') if y < a.height/2 else 'steel'
                a.putpixel((x, y), (*Image.new('RGB', (1, 1), b.P[key]).getpixel((0, 0)), px[3]))
    if name == 'score-frame':
        b.step(a, 3, 3, 92, 14, 2, 'coal')
        b.rect(a, 5, 3, 88, 1, 'shadow')
        b.rect(a, 3, 5, 1, 10, 'shadow')
        b.rect(a, 5, 16, 88, 1, 'inner')
    assert a.getchannel('A').tobytes() == old.getchannel('A').tobytes()
    return a


def frame(error=False):
    a = r5.frame(error)
    # Recolor the existing single rim; no new outline or changed inset.
    for y in range(1, 29):
        for x in range(1, 197):
            if a.getpixel((x, y))[:3] == Image.new('RGB', (1, 1), b.P['error' if error else 'rim']).getpixel((0, 0)):
                b.rect(a, x, y, 1, 1, ('error' if error else 'metal') if y == 1 or x == 1 else 'steel')
    return a


def render(params, revision):
    b.load = material if revision == 6 else original_load
    slayer = params.get('slayer', False)
    im = b.scene(boss=params.get('rank') == 'boss')
    if slayer:
        im = Image.open(r5.R4 / 'screens/320x180/hud-slayer.png').convert('RGBA')
        top, bottom = r5.slayer_bands()
        ImageDraw.Draw(im).rectangle((0, 0, 319, top-1), fill='black')
        ImageDraw.Draw(im).rectangle((0, bottom, 319, 179), fill='black')
    b.paste(im, b.overlay(**params, typing=False), 0, 0)
    board = b.blank(198, 30) if slayer else (frame if revision == 6 else r5.frame)(params.get('feedback') == 'error')
    b.paste(board, b.word_layer(progress=3 if params.get('feedback') == 'correct' else 0,
                              wrong=3 if params.get('feedback') == 'error' else None), 0, 0)
    im.paste(board, (61, 146), board)
    return im


def main():
    out = ROOT / 'preview'
    out.mkdir(parents=True, exist_ok=True)
    cases = dict(normal={}, correct=dict(feedback='correct'), error=dict(feedback='error'),
                 slayer=dict(rank='boss', slayer=True), elite=dict(rank='elite'), boss=dict(rank='boss'),
                 **{'low-hp': dict(rank='boss', hp=.01, enemy_hp=.01)})
    for rank in ['normal', 'elite', 'boss']:
        for slayer in [False, True]:
            for hp in [0, 1, 50, 100]:
                cases[f'{rank}-{ "slayer" if slayer else "normal"}-hp{hp}'] = dict(rank=rank, slayer=slayer, hp=hp/100, enemy_hp=hp/100)
    for name, params in cases.items():
        old, new = render(params, 5), render(params, 6)
        if name in ['normal', 'correct', 'error', 'slayer']:
            assert old.tobytes() == Image.open(R5 / f'preview/{name}-320x180.png').convert('RGBA').tobytes(), name
        # All modifications remain within actual opaque HUD material footprints.
        mask = b.blank(320, 180)
        for asset, x, y in [('resource-frame',3,11),('resource-socket',3,11),
                            ('resource-frame',215,11),('resource-socket' if params.get('rank','normal')=='normal' else params['rank']+'-socket',215,11 if params.get('rank','normal')=='normal' else 8),
                            ('secondary-frame',3,34),('secondary-socket',3,34),
                            ('secondary-frame',215,34),('secondary-socket',215,34),('score-frame',111,3)]:
            b.paste(mask, original_load(asset), x, y)
        if not params.get('slayer'): b.paste(mask, r5.frame(),61,146)
        diff = ImageChops.difference(old.convert('RGB'), new.convert('RGB'))
        for y in range(180):
            for x in range(320):
                if not mask.getpixel((x,y))[3]: assert diff.getpixel((x,y)) == (0,0,0), (name,x,y)
        # Continuous fills including the single HP pixel remain bit-identical.
        for box in [(23,18,100,24),(235,18,312,24),(17,38,101,42),(229,38,313,42),
                    (65,150,255,172),(25,146,60,178),(260,146,288,178)]:
            assert old.crop(box).tobytes() == new.crop(box).tobytes(), (name,box)
        for revision, im in [(5,old),(6,new)]:
            for scale in [1,2,4]:
                target=out/f'r{revision}-{name}-{320*scale}x{180*scale}.png'
                scaled=im.resize((320*scale,180*scale),Image.Resampling.NEAREST)
                scaled.save(target)
                assert Image.open(target).tobytes()==scaled.tobytes()
    options=''.join(f'<option>{name}</option>' for name in cases)
    (ROOT/'index.html').write_text('''<!doctype html><html lang="vi"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>HUD r6</title>
<style>body{background:#14111e;color:#f2e7cf;font:16px system-ui;margin:20px}a{color:#afa1b3}select{font:inherit}main{display:flex;gap:16px;flex-wrap:wrap}figure{margin:16px 0;max-width:100%;overflow:auto}img{image-rendering:pixelated;display:block}figcaption{margin-bottom:8px}label{display:inline-block;margin:8px}</style>
<h1>HUD r6 · kim loại tối</h1><p>Review tĩnh r5–r6 · cùng bố cục, chữ và fill. <a href="README.md">Mapping và kết quả kiểm</a> · <a href="contract.md">Contract</a></p>
<label>Trạng thái <select id="state">'''+options+'''</select></label><label>Phóng <select id="scale"><option value="1">1×</option><option value="2">2×</option><option value="4">4×</option></select></label><label><input id="gray" type="checkbox"> Grayscale</label>
<main><figure><figcaption>R5 · giữ nguyên vật liệu</figcaption><img id="old" alt="HUD r5" width="320" height="180"></figure><figure><figcaption>R6 · bevel và lòng lõm</figcaption><img id="new" alt="HUD r6" width="320" height="180"></figure></main>
<script>const state=document.querySelector('#state'),scale=document.querySelector('#scale'),gray=document.querySelector('#gray');function update(){const s=+scale.value;for(const [id,r] of [['old',5],['new',6]]){const im=document.getElementById(id);im.src=`preview/r${r}-${state.value}-${320*s}x${180*s}.png`;im.width=320*s;im.height=180*s;im.style.filter=gray.checked?'grayscale(1)':''}}document.querySelectorAll('select,input').forEach(e=>e.addEventListener('change',update));update()</script></html>''',encoding='utf-8')
    print(f'PASS {len(cases)} paired states x 3 scales; r5 originals, material isolation, fills, glyphs, Parry/timer and PNG decode')


if __name__ == '__main__': main()
