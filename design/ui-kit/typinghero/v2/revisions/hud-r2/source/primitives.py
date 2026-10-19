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


def word_layer(word='resolve', progress=0, wrong=None, size=16):
    a = blank(158, 30)
    f = ImageFont.truetype(str(ROOT/'fonts/VT323-Regular.ttf'), size)
    left = round((158-f.getlength(word))/2)
    for i, ch in enumerate(word):
        text(a, left+round(f.getlength(word[:i])), 5, ch, size,
             'error' if i == wrong else 'jade' if i < progress else 'ivory')
    return a


def actor(im, x, enemy=False, boss=False):
    name = 'captain' if boss else 'opponent' if enemy else 'hero'
    src = Image.open(ROOT/'artwork'/f'{name}-existing.png').convert('RGBA')
    if enemy:
        src = src.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    paste(im, src, x, 138-src.height)


