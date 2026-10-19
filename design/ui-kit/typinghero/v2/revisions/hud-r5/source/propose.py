"""Static reference adaptation only. No production atlas/gallery or Unity writes."""
import sys
sys.dont_write_bytecode = True
from pathlib import Path
import importlib.util
import json
from PIL import Image, ImageDraw, ImageOps

ROOT = Path(__file__).resolve().parents[1]
R4 = ROOT.parent / 'hud-r4'
sys.path.insert(0, str(R4 / 'source'))
spec = importlib.util.spec_from_file_location('r4build', R4 / 'source/build.py')
b = importlib.util.module_from_spec(spec)
spec.loader.exec_module(b)


def prepare_contract():
    """Persist draft structure before any preview raster is written."""
    target = ROOT / 'wireframe'
    target.mkdir(exist_ok=True)
    data = json.loads((R4 / 'wireframe/native.wireframe.json').read_text())
    data['name'] = 'HUD r5 draft / fixed r4 layout / thin rim and coal surface'
    (target / 'native.wireframe.json').write_text(json.dumps(data, indent=2), encoding='utf-8')
    manifest = dict(layoutId='hud-r5', revision=1, platform='web',
                    owner='Static HUD proposal; no Unity', contract='contract.md',
                    variants=[dict(id='native', viewport=data['viewport'],
                                   wireframe='native.wireframe.json',
                                   preview='native.wireframe.html', capture='native.png')],
                    approval=dict(status='draft'))
    # Existing renderer/capture receipts stay valid until inputs change.
    path = target / 'layout.json'
    if not path.exists():
        path.write_text(json.dumps(manifest, indent=2), encoding='utf-8')


def frame(error=False):
    a = b.blank(198, 30)
    b.step(a, 0, 0, 198, 30, 1, 'shadow')
    b.step(a, 1, 1, 196, 28, 1, 'error' if error else 'rim')
    b.step(a, 2, 2, 194, 26, 1, 'coal')
    return a


def slayer_bands():
    # Static adaptation of SlayerScreenFX.LateUpdate, not barHeight=64:
    # project the review actors' head/foot landmarks, then leave bodyPadding.
    feet = 138  # r4 scene actor anchor
    head = min(feet - Image.open(R4 / f'artwork/{name}-existing.png').height
               for name in ['hero', 'captain'])
    padding = 8  # bodyPadding mapped to this review's native pixel grid
    return max(0, head - padding), min(180, feet + padding)


def main():
    prepare_contract()
    out = ROOT / 'preview'
    out.mkdir(exist_ok=True)
    source = Image.open(ROOT / 'references/overkill.jpg').convert('RGB')
    thumb = ImageOps.pad(source, (320, 180), color='#090811')
    thumb.save(out / 'source-320x180.png')
    source.crop((640, 585, 1135, 680)).save(out / 'source-frame.png')
    for state in ['normal', 'correct', 'error', 'slayer']:
        old = Image.open(R4 / f'screens/320x180/hud-{state}.png').convert('RGBA')
        new = old.copy()
        # Slayer owns two full-width filmframe bands; no separate typing board.
        if state == 'slayer':
            top, bottom = slayer_bands()
            d = ImageDraw.Draw(new)
            d.rectangle((0, 0, 319, top - 1), fill='#000000')
            d.rectangle((0, bottom, 319, 179), fill='#000000')
            # Restore the unchanged HUD over the bands, excluding its old board.
            b.paste(new, b.overlay(rank='boss', slayer=True, typing=False), 0, 0)
            a = b.blank(198, 30)
        else:
            a = frame(state == 'error')
        b.paste(a, b.word_layer(progress=3 if state == 'correct' else 0,
                              wrong=3 if state == 'error' else None), 0, 0)
        new.paste(a, (61, 146), a)
        # Every pixel outside board must remain identical, including cut corners.
        for y in range(180):
            for x in range(320):
                changed_region = (y < top or y >= bottom) if state == 'slayer' else (61 <= x < 259 and 146 <= y < 176)
                if not changed_region:
                    assert old.getpixel((x, y)) == new.getpixel((x, y))
        for scale in [1, 2, 4]:
            name = f'{320*scale}x{180*scale}'
            new.resize((320*scale, 180*scale), Image.Resampling.NEAREST).save(out / f'{state}-{name}.png')
        if state == 'normal':
            new.save(out / '320x180.png')
            board = Image.new('RGB', (960, 180), '#090811')
            for x, im in [(0, thumb), (320, old), (640, new)]:
                board.paste(im, (x, 0))
            board.save(out / 'comparison.png')
    html = '''<!doctype html><html lang="vi"><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>HUD r5 — đề xuất</title>
<style>body{background:#14111e;color:#f2e7cf;font:16px system-ui;margin:24px}a{color:#afa1b3}button,select{font:inherit}img.pixel{image-rendering:pixelated}figure{margin:0}figcaption{margin:8px 0}main{display:flex;gap:16px;flex-wrap:wrap}section{overflow:auto;margin-top:24px}p{max-width:900px}img{max-width:none}</style>
<h1>Khung gõ r5 · đề xuất chờ chốt</h1><p>Thường: nền coal, viền rim mảnh1px. Slayer: ba từ trực tiếp trên dải đen dưới, không nền/viền riêng. Hai dải bám đầu/chân actor theo SlayerScreenFX. Giữ geometry chữ và hành vi r4. Đây là preview tĩnh; chưa có asset/gallery tương tác r5.</p>
<p><a href="README.md">Nguồn, chọn/loại và mapping</a> · <a href="wireframe/contract.md">Contract draft</a> · <a href="wireframe/native.wireframe.html">Wireframe</a> · <a href="../hud-r4/index.html">R4 tương tác gốc</a></p>
<label>Phóng <select id="scale"><option value="1">1× · 320×180</option><option value="2">2× · 640×360</option><option value="4">4× · 1280×720</option></select></label>
<label>Trạng thái <select id="state"><option>normal</option><option>correct</option><option>error</option><option>slayer</option></select></label>
<label><input type="checkbox" id="gray"> Grayscale</label>
<main><figure><figcaption>Ảnh gameplay nguồn · letterbox</figcaption><img class="pixel compare" src="preview/source-320x180.png" width="320" height="180"></figure>
<figure><figcaption>R4 · giữ nguyên</figcaption><img id="old" class="pixel compare" src="../hud-r4/screens/320x180/hud-normal.png" width="320" height="180"></figure>
<figure><figcaption>R5 · adaptation đề xuất</figcaption><img id="new" class="pixel compare" src="preview/normal-320x180.png" width="320" height="180"></figure></main>
<section><p>Crop nguồn: bảng đen, viền nhẹ, khoảng trống quanh chữ. Không lấy màu vàng hay texture.</p><img src="preview/source-frame.png" alt="Crop bảng chữ từ ảnh Overkill"></section>
<section><p>Ảnh nguồn đầy đủ · SEGA / Modern Dream · <a href="references/sources.json">URL gốc</a></p><a href="references/overkill.jpg"><img src="references/overkill.jpg" width="864" alt="Overkill gameplay đầy đủ"></a></section>
<script>function update(){let s=+scale.value;document.querySelectorAll('.compare').forEach(i=>{i.width=320*s;i.height=180*s;i.style.filter=gray.checked?'grayscale(1)':''});document.querySelector('#old').src='../hud-r4/screens/320x180/hud-'+state.value+'.png';document.querySelector('#new').src='preview/'+state.value+'-320x180.png'}document.querySelectorAll('select,input').forEach(e=>e.onchange=update);</script></html>'''
    (ROOT / 'index.html').write_text(html, encoding='utf-8')
    print('PASS preview: r4 word_layer reused; unchanged pixels outside board/bands; 4 states x 3 scales; Slayer bands', slayer_bands())


if __name__ == '__main__':
    main()
