"""HUD review revision. Run with Python + Pillow; writes only this revision.

All geometry is drawn at 1x. No ImageGen, Unity changes, or network needed.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import json
import shutil
import html

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT.parents[1]
P = dict(shadow='#090811', coal='#14111e', deep='#211c2d', steel='#494153',
         edge='#77677f', gold='#a3844d', light='#dcc18a', ivory='#f2e7cf',
         muted='#afa1b3', red='#a6354c', error='#f16b67', jade='#588b8c',
         rim='#b5acb8', metal='#827789', bevel='#5e536a', inner='#282231')
ASSETS = {}
TEXT = []
# Compact 3x5 native glyphs for HUD captions; VT323 retained for score and word.
GLYPHS = dict(zip('ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789', [
    '010101111101101', '110101110101110', '011100100100011', '110101101101110',
    '111100110100111', '111100110100100', '011100101101011', '101101111101101',
    '111010010010111', '001001001101010', '101101110101101', '100100100100111',
    '101111111101101', '101111111111101', '010101101101010', '110101110100100',
    '010101101111011', '110101110101101', '011100010001110', '111010010010010',
    '101101101101111', '101101101101010', '101101111111101', '101101010101101',
    '101101010010010', '111001010100111',
    '111101101101111', '010110010010111', '110001010100111', '110001010001110',
    '101101111001001', '111100110001110', '011100111101111', '111001010010010',
    '111101111101111', '111101111001110']))
GLYPHS.update({' ': '000000000000000', '/': '001001010100100',
               ':': '000010000010000', '!': '010010010000010',
               ',': '000000000010100', '.': '000000000000010',
               '-': '000000111000000'})
for folder in ['source', 'png', 'svg', 'sheets', 'artwork', 'fonts',
               'screens/320x180', 'screens/640x360', 'screens/1280x720']:
    (ROOT/folder).mkdir(parents=True, exist_ok=True)
for folder, names in [('artwork', ['01-backdrop.png', '03-foreground.png',
                                   'hero-existing.png', 'opponent-existing.png', 'captain-existing.png']),
                      ('fonts', ['VT323-Regular.ttf', 'VT323-OFL.txt'])]:
    for name in names:
        target = ROOT/folder/name
        if not target.exists():
            shutil.copyfile(BASE/folder/name, target)
before_review = ROOT/'artwork/hud-before-pinterest-review.png'
if not before_review.exists() and (ROOT/'screens/320x180/hud-normal.png').exists():
    shutil.copyfile(ROOT/'screens/320x180/hud-normal.png', before_review)


def blank(w, h):
    return Image.new('RGBA', (w, h))


def rect(im, x, y, w, h, color):
    if w > 0 and h > 0:
        ImageDraw.Draw(im).rectangle((x, y, x+w-1, y+h-1), fill=P.get(color, color))


def step(im, x, y, w, h, cut, color):
    # Stair-stepped bevel: all coordinates and edges remain native pixels.
    ImageDraw.Draw(im).polygon([(x+cut, y), (x+w-1-cut, y), (x+w-1, y+cut),
                               (x+w-1, y+h-1-cut), (x+w-1-cut, y+h-1),
                               (x+cut, y+h-1), (x, y+h-1-cut), (x, y+cut)],
                              fill=P.get(color, color))


def paste(im, layer, x, y):
    im.alpha_composite(layer, (x, y))


def save(name, im, role, border=None):
    im.save(ROOT/'png'/f'{name}.png')
    # Editable code-native SVG; horizontal runs preserve exact RGBA pixels.
    runs = []
    for y in range(im.height):
        x = 0
        while x < im.width:
            pixel = im.getpixel((x, y))
            end = x+1
            while end < im.width and im.getpixel((end, y)) == pixel:
                end += 1
            if pixel[3]:
                c = '#%02x%02x%02x' % pixel[:3]
                runs.append(f'<rect x="{x}" y="{y}" width="{end-x}" height="1" fill="{c}"/>')
            x = end
    (ROOT/'svg'/f'{name}.svg').write_text(
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{im.width}" height="{im.height}" '
        'shape-rendering="crispEdges">'+''.join(runs)+'</svg>', encoding='utf8')
    ASSETS[name] = dict(size=list(im.size), role=role, border_LBRT=border or [0]*4,
                        png=f'png/{name}.png', svg=f'svg/{name}.svg', pivot=[.5, .5])
    return im


def load(name):
    return Image.open(ROOT/'png'/f'{name}.png').convert('RGBA')


def sliced(a, width, height, border=8):
    """Review the declared frame border with fixed corners at native scale."""
    assert min(width, height) >= border*2
    out = blank(width, height)
    for sx, ex, dx, fx in [(0, border, 0, border),
                           (border, a.width-border, border, width-border),
                           (a.width-border, a.width, width-border, width)]:
        for sy, ey, dy, fy in [(0, border, 0, border),
                               (border, a.height-border, border, height-border),
                               (a.height-border, a.height, height-border, height)]:
            paste(out, a.crop((sx, sy, ex, ey)).resize((fx-dx, fy-dy), Image.Resampling.NEAREST), dx, dy)
    return out


def text(im, x, y, value, size=10, color='ivory', center=False, limit=None):
    if size <= 10:
        width = len(value)*4-1
        assert limit is None or width <= limit, (value, width, limit)
        left = round(x-width/2) if center else x
        for i, ch in enumerate(value.upper()):
            glyph = GLYPHS[ch]
            for n, bit in enumerate(glyph):
                if bit == '1':
                    rect(im, left+i*4+n%3, y+2+n//3, 1, 1, color)
        TEXT.append(dict(text=value, size='3x5', x=left, y=y+2, width=width, limit=limit))
        return
    f = ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'), size)
    width = f.getlength(value)
    assert limit is None or width <= limit, (value, width, limit)
    left = round(x-width/2) if center else x
    mask = Image.new('L', im.size)
    ImageDraw.Draw(mask).text((left, y), value, font=f, fill=255)
    # Rasterize once at native size, retaining v2 font coverage; exports duplicate pixels.
    im.paste(P.get(color, color), (0, 0), mask)
    TEXT.append(dict(text=value, size=size, x=left, y=y, width=width, limit=limit))


def bar_assets(gold=False):
    suffix = '-gold' if gold else ''
    rim, metal = ('light', 'gold') if gold else ('rim', 'metal')
    a = blank(102, 21)
    # Cast shadow, external glint, metal body, inner dark lip.
    for x, y, w, h, cut, c in [(12, 6, 90, 13, 3, 'shadow'),
                               (12, 3, 90, 14, 3, rim),
                               (13, 4, 88, 12, 2, metal),
                               (14, 5, 86, 10, 2, 'bevel'),
                               (17, 6, 81, 8, 2, 'shadow')]:
        step(a, x, y, w, h, cut, c)
    rect(a, 19, 4, 76, 1, 'ivory' if gold else 'rim')
    # Wide upper plane and darker underside: bevel has area, not only an outline.
    rect(a, 20, 5, 76, 1, 'gold' if gold else 'metal')
    rect(a, 20, 14, 76, 1, '#705338' if gold else '#39313f')
    rect(a, 20, 16, 75, 1, 'shadow')
    rect(a, 99, 7, 1, 6, '#705338' if gold else '#39313f')
    rect(a, 97, 5, 1, 2, 'light' if gold else 'rim')
    rect(a, 19, 15, 77, 1, 'inner')
    # Narrow collar joining the circular socket to the recessed track.
    rect(a, 14, 4, 5, 12, 'shadow')
    rect(a, 15, 4, 3, 11, metal)
    rect(a, 15, 4, 1, 10, rim)
    rect(a, 18, 6, 1, 8, 'inner')
    save('resource-frame'+suffix, a, 'Fixed socket neck and stepped endcap. Never stretch.')
    socket = blank(21, 21)
    for x, y, w, h, cut, c in [(0, 2, 20, 19, 5, 'shadow'),
                               (0, 0, 20, 19, 5, rim),
                               (1, 1, 18, 17, 5, metal),
                               (2, 2, 16, 15, 4, 'bevel'),
                               (3, 3, 14, 13, 4, 'shadow'),
                               (4, 4, 12, 11, 3, 'inner')]:
        ImageDraw.Draw(socket).ellipse((x, y, x+w-1, y+h-1), fill=P.get(c, c))
    rect(socket, 6, 1, 7, 1, 'ivory')
    rect(socket, 1, 6, 1, 6, rim)
    rect(socket, 6, 17, 7, 1, 'inner')
    rect(socket, 3, 4, 1, 3, 'light' if gold else 'rim')
    rect(socket, 16, 11, 1, 3, '#705338' if gold else '#39313f')
    save('resource-socket'+suffix, socket, 'Fixed multilayer circular pixel socket.')


def panel(name, w, h, gold=False, error=False):
    a = blank(w, h)
    rim = 'error' if error else 'light' if gold else 'rim'
    step(a, 0, 2, w, h-2, 3, 'shadow')
    step(a, 0, 0, w, h-3, 3, rim)
    step(a, 1, 1, w-2, h-5, 2, 'gold' if gold else 'bevel')
    step(a, 3, 3, w-6, h-9, 1, 'shadow')
    rect(a, 4, 4, w-8, h-11, 'deep')
    rect(a, 5, 4, w-10, 1, 'coal')
    rect(a, 5, h-7, w-10, 1, 'steel')
    for x in [1, w-7]:
        for y in [1, h-10]:
            rect(a, x, y, 6, 6, 'steel')
            rect(a, x, y, 5, 1, rim)
            rect(a, x, y, 1, 5, rim)
            rect(a, x+2, y+2, 2, 2, 'shadow')
            rect(a, x+2, y+2, 1, 1, 'light')
    return save(name, a, 'Dark inset board with reinforced corners.', [8]*4)


def assets():
    bar_assets()
    bar_assets(True)
    track = blank(77, 6)
    rect(track, 0, 0, 77, 6, 'coal')
    rect(track, 0, 0, 77, 1, 'shadow')
    rect(track, 0, 5, 77, 1, 'steel')
    save('resource-track', track, 'Recess visible at zero HP; not part of fill.')
    for kind, colors in [('hp', ['#f17c81', '#d45165', '#b43852', '#b43852', '#74243e', '#542039']),
                         ('combo', ['#edda9e', '#c2a263', '#a3844d', '#a3844d', '#705338', '#473245']),
                         ('stamina', ['#a3c5ba', '#71a89f', '#588b8c', '#588b8c', '#39626b', '#2a414f'])]:
        fill = blank(77, 6)
        for y, c in enumerate(colors):
            rect(fill, 0, y, 77, 1, c)
        save(kind+'-fill', fill, 'Continuous horizontal crop, no quantized segments.')
    ticks = blank(77, 6)
    for x in range(12, 77, 13):
        rect(ticks, x, 0, 1, 1, 'light')
    save('resource-decoration', ticks, 'Top glints only. Never control fill amount.')
    basis_path = ROOT/'source/semantic-basis.json'
    if not basis_path.exists():
        basis = {}
        for name in ['icon-hp', 'icon-combo', 'icon-stamina', 'captain-badge',
                     'icon-pause', 'keycap-space']:
            a = Image.open(BASE/'png'/f'{name}.png').convert('RGBA')
            basis[name] = dict(size=list(a.size), pixels=list(a.get_flattened_data()))
        basis_path.write_text(json.dumps(basis), encoding='utf8')
    for name, info in json.loads(basis_path.read_text()).items():
        a = blank(*info['size'])
        a.putdata([tuple(px) for px in info['pixels']])
        if name == 'icon-hp':
            # Keep semantic silhouette, use HP colors and a top-plane glint.
            for y in range(a.height):
                for x in range(a.width):
                    if a.getpixel((x, y))[3]:
                        a.putpixel((x, y), (241, 124, 129, 255) if y < 2 else
                                   (212, 81, 101, 255) if y < a.height-2 else (116, 36, 62, 255))
        elif name == 'icon-stamina':
            for y in range(a.height):
                for x in range(a.width):
                    if a.getpixel((x, y))[3]:
                        a.putpixel((x, y), (163, 197, 186, 255) if y < 3 else (88, 139, 140, 255))
        save(name, a, 'Reused v2 semantic artwork; fixed size.')
    panel('score-frame', 98, 43)
    panel('score-frame-gold', 98, 43, True)
    panel('typing-frame', 158, 30)
    panel('typing-frame-gold', 158, 30, True)
    panel('typing-frame-error', 158, 30, error=True)
    label = blank(96, 13)
    step(label, 0, 2, 96, 11, 2, 'shadow')
    step(label, 0, 0, 96, 11, 2, 'rim')
    step(label, 1, 1, 94, 9, 1, 'steel')
    rect(label, 3, 2, 90, 7, 'coal')
    rect(label, 4, 2, 88, 1, 'shadow')
    rect(label, 3, 9, 90, 1, 'inner')
    for x in [1, 92]:
        rect(label, x, 3, 2, 4, 'metal')
        rect(label, x, 3, 1, 1, 'ivory')
    save('label-frame', label, 'Separate thin label; fixed 4px corners.', [4, 4, 4, 4])


def rank_assets():
    for gold in [False, True]:
        suffix = '-gold' if gold else ''
        for rank, rise, peaks in [('elite', 2, [5, 13]), ('boss', 3, [3, 9, 15])]:
            a = blank(21, 24)
            paste(a, load('resource-socket'+suffix), 0, 3)
            for x in peaks:
                rect(a, x, 3-rise, 3, rise+3, 'light' if gold else 'rim')
                rect(a, x+1, 4-rise, 1, rise+2, 'gold' if gold else 'bevel')
            save(rank+'-socket'+suffix, a, 'Fixed rank silhouette; socket origin at y=3.')
    a = blank(10, 10)
    for y in [1, 5]:
        for x in range(4):
            rect(a, x+1, y+x//2, 1, 2, 'ivory')
            rect(a, 8-x, y+x//2, 1, 2, 'ivory')
    save('elite-badge', a, 'Two chevrons inside HP socket, never on fill.')


def word_layer(word='resolve', progress=0, wrong=None, size=16):
    a = blank(158, 30)
    f = ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'), size)
    left = round((158-f.getlength(word))/2)
    for i, ch in enumerate(word):
        text(a, left+round(f.getlength(word[:i])), 5, ch, size,
             'error' if i == wrong else 'jade' if i < progress else 'ivory')
    return a


def resource(im, x, y, kind, value, gold=False, rank="normal"):
    assert 0 <= value <= 1
    suffix = '-gold' if gold else ''
    paste(im, load('resource-frame'+suffix), x, y)
    paste(im, load('resource-track'), x+20, y+7)
    pixels = 0 if value == 0 else max(1, round(77*value))
    if pixels:
        paste(im, load(kind+'-fill').crop((0, 0, pixels, 6)), x+20, y+7)
    paste(im, load('resource-decoration'), x+20, y+7)
    paste(im, load(rank+'-socket'+suffix), x, y-3) if rank != 'normal' else paste(im, load('resource-socket'+suffix), x, y)
    icon = load('elite-badge' if rank == 'elite' else 'captain-badge' if rank == 'boss' else 'icon-'+kind)
    paste(im, icon, x+(20-icon.width)//2, y+(19-icon.height)//2)


def actor(im, x, enemy=False, boss=False):
    name = 'captain' if boss else 'opponent' if enemy else 'hero'
    src = Image.open(ROOT/'artwork'/f'{name}-existing.png').convert('RGBA')
    if enemy:
        src = src.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    paste(im, src, x, 138-src.height)


def hud(boss=False, slayer=False, stress=False, feedback='normal', hp=.85, elite=False, enemy_hp=None, typing=True):
    im = Image.open(ROOT/'artwork/01-backdrop.png').convert('RGBA')
    text(im, 5, 1, 'WWWWWWWWWW' if stress else 'HERO', 8, limit=58)
    text(im, 73, 1, f'{round(hp*100)}/100', 8, 'error' if hp <= .01 else 'ivory', limit=31)
    resource(im, 3, 11, 'hp', hp, slayer)
    resource(im, 3, 32, 'combo', .99 if stress else .72, slayer)
    text(im, 217, 1, 'CAPTAIN' if boss else 'ELITE SWORDSMAN' if elite else 'SWORDSMAN', 8, limit=64)
    text(im, 281, 1, '200/550' if boss else '80/100', 8, limit=36)
    resource(im, 215, 11, 'hp', enemy_hp if enemy_hp is not None else 200/550 if boss else .8, slayer, 'boss' if boss else 'elite' if elite else 'normal')
    resource(im, 215, 32, 'stamina', .33, slayer)
    paste(im, load('score-frame-gold' if slayer else 'score-frame'), 111, 3)
    label = load('label-frame')
    # Fixed label corners, straight center shortened using a crop, no icon stretch.
    compact = blank(50, 13)
    paste(compact, label.crop((0, 0, 25, 13)), 0, 0)
    paste(compact, label.crop((71, 0, 96, 13)), 25, 0)
    paste(im, compact, 135, 0)
    text(im, 160, 1, 'SCORE', 8, 'light', True)
    text(im, 160, 14, '2,147,483,647' if stress else '012,840', 12, 'light', True, 82)
    text(im, 160, 29, '999 / x16' if stress else '24 COMBO / x16' if slayer else '24 COMBO / x4', 8, 'light', True, 82)
    text(im, 160, 48, 'SLAYER / x16' if slayer else 'ROUND 12 / NORMAL', 10, 'light', True, 102)
    actor(im, 90)
    actor(im, 203, True, boss)
    paste(im, Image.open(ROOT/'artwork/03-foreground.png').convert('RGBA'), 0, 0)
    if typing:
        state = '-error' if feedback == 'error' else '-gold' if slayer else ''
        paste(im, load('typing-frame'+state), 81, 146)
        word = 'counterrevolutionary' if stress else 'resolve'
        paste(im, word_layer(word, 3 if feedback == 'correct' else 0,
                             3 if feedback == 'error' else None, 12 if stress else 16), 81, 146)
    text(im, 47, 146, 'PARRY', 10, 'light', True)
    paste(im, load('keycap-space'), 35, 160)
    text(im, 272, 146, '00:42', 10, center=True)
    paste(im, load('icon-pause'), 267, 163)
    return im


def atlas():
    sheet = blank(512, 256)
    x = y = 3
    row = 0
    for name, info in ASSETS.items():
        a = load(name)
        if x+a.width+3 > sheet.width:
            x, y, row = 3, y+row+6, 0
        assert y+a.height+3 <= sheet.height
        # Two pixels of edge extrusion outside the recorded sprite rect.
        padded = a.resize((a.width+4, a.height+4), Image.Resampling.NEAREST)
        padded.paste(a, (2, 2))
        for i in range(2):
            padded.paste(a.crop((0, 0, a.width, 1)), (2, i))
            padded.paste(a.crop((0, a.height-1, a.width, a.height)), (2, a.height+2+i))
            padded.paste(a.crop((0, 0, 1, a.height)), (i, 2))
            padded.paste(a.crop((a.width-1, 0, a.width, a.height)), (a.width+2+i, 2))
        paste(sheet, padded, x-2, y-2)
        info['atlas_xywh'] = [x, y, a.width, a.height]
        x += a.width+6
        row = max(row, a.height)
    sheet.save(ROOT/'sheets/ui-atlas.png')


def gallery(names):
    cards = ''.join(f'<article><h2>{n}</h2><img width="640" height="360" src="screens/640x360/{n}.png" alt="{n}"><p>'+
                    ' · '.join(f'<a href="screens/{s}/{n}.png">{s}</a>' for s in ['320x180', '640x360', '1280x720'])+'</p></article>' for n in names)
    refs = [
        ('CHỌN: ổ tròn nối thanh, mặt bevel sáng/tối (hàng giữa dưới)', '993184524050127407', '39/c9/2f/39c92f4bea88ccd62bfa1bfee3b89e59.jpg', 'resource-socket'),
        ('CHỌN: mặt bảng tối, cạnh kim loại nhiều sắc; không lấy ornament', '194499277651664276', '04/8c/2c/048c2c5a5919fa6ccdbaf86f5674e8af.jpg', 'typing-frame'),
        ('ĐỐI CHIẾU: mặt trước / cạnh sáng / bóng chân của bảng', '334321972354241632', 'e2/29/03/e22903ed69e61a13e361fc532b8c4646.jpg', 'score-frame'),
        ('The Druid — boss endcaps, fill/ticks tách', '1025272671408964433', '1d/de/5d/1dde5dc0367406526f6ac1bc7002e078.jpg', 'resource-frame'),
        ('Bóng chân và mặt nhiều lớp; không lấy gỗ/lá', '804666658453520783', '57/be/a8/57bea8754244636a46083e748ea0684a.jpg', 'typing-frame'),
        ('Thanh cạn vẫn thấy lòng rãnh; không lấy hiệu ứng vỡ', '1089871178602252731', '2d/53/25/2d5325ca0c0cf265a6e58c6522bb85c1.jpg', 'resource-track')]
    mapping = ''.join(f'<tr><td><a href="https://www.pinterest.com/pin/{pin}/">{html.escape(title)}</a><br><img class="reference" src="https://i.pinimg.com/736x/{url}" alt="{html.escape(title)}"></td><td><img class="component" src="png/{asset}.png" alt="{asset}"><p>{asset}</p></td><td><img width="320" height="180" src="screens/320x180/hud-boss.png" alt="HUD cạnh sprite thật"></td></tr>' for title, pin, url, asset in refs)
    page = '''<!doctype html><html lang="vi"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>TypingHero — HUD r1 review</title>
<style>body{margin:24px;background:#14111e;color:#f2e7cf;font:15px system-ui}a{color:#dcc18a}main{display:flex;flex-wrap:wrap;gap:24px}article{max-width:100%}img{image-rendering:pixelated;max-width:100%;height:auto}h2{font-size:16px}table{border-collapse:collapse}td,th{padding:16px;border:1px solid #494153;text-align:left;vertical-align:middle}.reference{max-width:280px;max-height:260px;object-fit:contain}.component{width:306px}p{max-width:960px;line-height:1.6}</style>
<h1>HUD r1 ? compact typing / normal, elite, boss</h1><p>Giữ background, sprite và bố cục chiến đấu v2. Không tích hợp Unity. Đã tự tìm, xem chín pin mới và chọn reference ổ tròn / mặt bevel / bảng tối theo hướng plan; không chờ user tìm thêm ảnh. Nguồn chọn và đánh giá nằm trong README.</p>
<p><a href="#mapping">Reference → component → sprite thật</a> · <a href="component-sheet.png">Lớp asset / HP 0–100%</a> · <a href="README.md">Nguồn và kiểm chứng</a></p><main>'''+cards+'''</main><h2>Trước / sau tự review</h2><p>Đổi ổ bậc chéo thành ổ tròn pixel, tăng diện tích bevel sáng/tối và tương phản icon HP/tài nguyên. Giữ bố cục và dữ liệu.</p><img width="640" src="artwork/hud-before-pinterest-review.png" alt="Trước review Pinterest"><img width="640" src="screens/320x180/hud-normal.png" alt="Sau review Pinterest"><h2 id="mapping">Đối chiếu nguồn đã mở</h2><p>Ảnh reference tải trực tiếp từ Pinterest khi có mạng; không đóng gói asset của tác giả. Hai nguồn CHỌN bổ sung bằng chứng cho kết cấu đã mô tả trong plan, theo quyền tự chọn/tự đánh giá của user.</p><table><tr><th>Reference</th><th>Component của revision</th><th>HUD cạnh sprite thật</th></tr>'''+mapping+'</table></html>'
    (ROOT/'index.html').write_text(page, encoding='utf8')


def build():
    assets()
    rank_assets()
    hud(typing=False).save(ROOT/'artwork/preview-background.png')
    screens = {'hud-normal': hud(), 'hud-elite': hud(elite=True), 'hud-boss': hud(boss=True),
               'hud-slayer': hud(boss=True, slayer=True),
               'hud-stress-low-hp': hud(boss=True, slayer=True, stress=True, hp=.01),
               'hud-correct': hud(feedback='correct'), 'hud-error': hud(feedback='error')}
    for name, im in screens.items():
        for scale in [1, 2, 4]:
            im.resize((320*scale, 180*scale), Image.Resampling.NEAREST).save(
                ROOT/f'screens/{320*scale}x{180*scale}/{name}.png')
    contact = Image.new('RGBA', (960, 600), P['coal'])
    for i, (name, im) in enumerate(screens.items()):
        x, y = i%3*320, i//3*200
        text(contact, x+5, y+1, name, 12)
        paste(contact, im, x, y+20)
    contact = contact.crop((0, 0, 960, 600))
    contact.save(ROOT/'contact-sheet.png')
    parts = Image.new('RGBA', (640, 680), P['coal'])
    for i, hp in enumerate([0, .01, .5, 1]):
        text(parts, 8+i*158, 2, f'HP {hp*100:g}%', 12)
        demo = blank(106, 24)
        resource(demo, 0, 0, 'hp', hp)
        paste(parts, demo, 8+i*158, 24)
    for i, name in enumerate(['resource-socket', 'resource-frame', 'resource-track',
                              'hp-fill', 'resource-decoration', 'icon-hp']):
        y = 72+i*48
        text(parts, 8, y, name, 12)
        a = load(name)
        paste(parts, a.resize((a.width*2, a.height*2), Image.Resampling.NEAREST), 190, y)
    text(parts, 8, 368, '9-SLICE / FIXED 8PX CORNERS', 10)
    paste(parts, sliced(load('score-frame'), 128, 43), 8, 390)
    paste(parts, sliced(load('typing-frame'), 230, 55), 160, 390)
    for row, rank in enumerate(['normal', 'elite', 'boss']):
        text(parts, 8, 468+row*64, rank.upper(), 10)
        for col, gold in enumerate([False, True]):
            demo = blank(106,27)
            resource(demo,0,3,'hp',.5,gold,rank)
            paste(parts,demo.resize((212,54),Image.Resampling.NEAREST),100+col*270,460+row*64)
    parts.save(ROOT/'component-sheet.png')
    atlas()
    metadata = dict(revision='hud-r1', canvas=[320, 180], filter='Point', wrap='Clamp',
                    mipmaps=False, scales=[1, 2, 4], palette=P, assets=ASSETS,
                    atlas=dict(size=[512, 256], origin='top-left', padding=6, extrusion=2),
                    resource=dict(size=[102, 21], fill_xywh=[20, 7, 77, 6],
                                  mode='continuous crop; zero HP draws zero fill pixels',
                                  stretch='fixed socket, neck and cap; regenerate native length, never stretch composite'),
                    typing=dict(rect=[81,146,158,30], error_ms=180, shake_ms=120, max_offset=1, reset_on_error=True),
                    screens=list(screens), unity_integration=False)
    (ROOT/'metadata.json').write_text(json.dumps(metadata, indent=2), encoding='utf8')
    (ROOT/'source/text-layout.json').write_text(json.dumps(TEXT, indent=2), encoding='utf8')
    for n, progress, wrong in ([(f'word-progress-{i}',i,None) for i in range(8)] + [(f'word-wrong-{i}',0,i) for i in range(7)]):
        word_layer(progress=progress, wrong=wrong).save(ROOT/'png'/f'{n}.png')
    gallery(screens)
    preview = (ROOT/'source/preview.html').read_text(encoding='utf8')
    page = (ROOT/'index.html').read_text(encoding='utf8')
    (ROOT/'index.html').write_text(page.replace('<main>', preview+'<main>', 1), encoding='utf8')
    print(f'Built {len(ASSETS)} layered assets and {len(screens)} screens x 3 sizes.')


if __name__ == '__main__':
    build()
