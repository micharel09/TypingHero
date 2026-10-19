"""Validate delivered sprites, source SVG, atlas, scaling, HP and text bounds."""
from pathlib import Path
from PIL import Image
import importlib.util
import json
import sys
import xml.etree.ElementTree as ET

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[1]
spec = importlib.util.spec_from_file_location('hud', ROOT/'source/build.py')
hud = importlib.util.module_from_spec(spec)
spec.loader.exec_module(hud)
meta = json.loads((ROOT/'metadata.json').read_text())
atlas = Image.open(ROOT/'sheets/ui-atlas.png').convert('RGBA')
rects = []
for name, info in meta['assets'].items():
    a = Image.open(ROOT/info['png']).convert('RGBA')
    assert list(a.size) == info['size']
    assert set(a.getchannel('A').get_flattened_data()) <= {0, 255}, name
    x, y, w, h = info['atlas_xywh']
    assert atlas.crop((x, y, x+w, y+h)).tobytes() == a.tobytes(), name
    for xx, yy, ww, hh in rects:
        assert x+w+4 <= xx or xx+ww+4 <= x or y+h+4 <= yy or yy+hh+4 <= y
    rects.append((x, y, w, h))
    svg = ET.parse(ROOT/info['svg']).getroot()
    rebuilt = hud.blank(w, h)
    for r in svg:
        hud.rect(rebuilt, int(r.attrib['x']), int(r.attrib['y']),
                 int(r.attrib['width']), int(r.attrib['height']), r.attrib['fill'])
    assert rebuilt.tobytes() == a.tobytes(), ('SVG mismatch', name)

for name in meta['screens']:
    a = Image.open(ROOT/f'screens/320x180/{name}.png').convert('RGBA')
    assert a.size == (320, 180)
    for scale in [2, 4]:
        actual = Image.open(ROOT/f'screens/{320*scale}x{180*scale}/{name}.png').convert('RGBA')
        assert actual.tobytes() == a.resize(actual.size, Image.Resampling.NEAREST).tobytes()
    # The visible portions of both actors and combat floor remain the v2 pixels.
    bg = Image.open(ROOT/'artwork/01-backdrop.png').convert('RGBA')
    hud.actor(bg, 90)
    hud.actor(bg, 203, True, name in ['hud-boss', 'hud-slayer', 'hud-stress-low-hp'])
    assert a.crop((0, 65, 320, 133)).tobytes() == bg.crop((0, 65, 320, 133)).tobytes()

fill = hud.load('hp-fill')
track = hud.load('resource-track')
for hp, expected in [(0, 0), (.01, 1), (.5, 38), (1, 77)]:
    a = hud.blank(106, 24)
    hud.resource(a, 0, 0, 'hp', hp)
    # A middle row unaffected by highlights, collar, icon, ticks, or endcap.
    actual = sum(a.getpixel((20+x, 10)) == fill.getpixel((x, 3)) for x in range(77))
    assert actual == expected, (hp, actual, expected)
    for x in range(expected, 77):
        assert a.getpixel((20+x, 10)) == track.getpixel((x, 3))

# Full HUD boundary cases beyond the six display variants.
for hp in [0, .01, .5, 1]:
    a = hud.hud(boss=True, slayer=True, stress=True, hp=hp)
    assert a.size == (320, 180)
for name in ['score-frame', 'typing-frame', 'typing-frame-error']:
    a = hud.load(name)
    for w, h in [(a.width+30, a.height+12), (a.width+75, a.height)]:
        b = hud.sliced(a, w, h)
        for x, xx in [(0, 0), (a.width-8, w-8)]:
            for y, yy in [(0, 0), (a.height-8, h-8)]:
                assert a.crop((x, y, x+8, y+8)).tobytes() == b.crop((xx, yy, xx+8, yy+8)).tobytes()
checks = json.loads((ROOT/'source/text-layout.json').read_text()) + hud.TEXT
for t in checks:
    if t['limit'] is not None:
        assert t['width'] <= t['limit'], t
    assert t['x'] >= 0 and t['x']+t['width'] <= 960, t

for rank in ['normal', 'elite', 'boss']:
    for gold in [False, True]:
        for hp in [0, .01, .5, 1]:
            hud.hud(boss=rank=='boss', elite=rank=='elite', slayer=gold,
                    stress=True, hp=hp, enemy_hp=hp)
            a = hud.blank(106,27)
            hud.resource(a,0,3,'hp',hp,gold,rank)
            expected = 0 if hp == 0 else max(1,round(77*hp))
            assert sum(a.getpixel((20+x,13)) == fill.getpixel((x,3)) for x in range(77)) == expected
for gold in ['', '-gold']:
    elite, boss = hud.load('elite-socket'+gold), hud.load('boss-socket'+gold)
    assert elite.getbbox()[1] == 1 and boss.getbbox()[1] == 0
    assert elite.convert('L').tobytes() != boss.convert('L').tobytes()
assert meta['typing']['rect'] == [81,146,158,30]
assert len(meta['screens']) == 7
for name in ['typing-frame','typing-frame-gold','typing-frame-error']:
    assert hud.load(name).size == (158,30)
for removed in ['TYPE / ATTACK','CORRECT','! MISS / TRY AGAIN','NEXT courage / honor','resxlve']:
    assert all(t['text'] != removed for t in checks)
print('PASS: 26 PNG/SVG assets; atlas; 21 nearest-neighbor exports; unchanged combat; all ranks + Slayer at HP 0/1/50/100%; compact word; text bounds.')
