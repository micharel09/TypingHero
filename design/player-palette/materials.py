"""Material-mask authoring helpers; no Unity files are written."""
from collections import deque
import json
from pathlib import Path
from PIL import Image

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1]
SOURCE = HERE / 'originals' if (HERE / 'originals').is_dir() else ROOT / 'Assets/Art/male_hero'
REFERENCE = HERE.parent / 'hero-idle-palette'
PALETTE = json.loads((REFERENCE / 'palette.json').read_text())
PALETTE['scarf']['e74088'] = 'f5f1e5'  # Extra original scarf highlight in movement sheets.
PALETTE['scarf'].update({'931897':'9b9b9f', '890f8d':'919195', '8a118e':'929296'})
PALETTE['clothing']['283584'] = '232631'  # One original blue edge pixel in ledge_hang.
BASE = [tuple(bytes.fromhex(c)) for c in (
    '000000 8b108f a24e57 7d5556 d2947f c0d5dc cd3073 4c2641 1c548e '
    '5c2c4f ffc5b4 143464 69799e ffffff 9f95ab a0acc5 d77e86 '
    '24523b b3b9d1 59c135 1a7a3e e74088').split()]


def rgb(value):
    return tuple(bytes.fromhex(value))


def components(points):
    remaining = set(points)
    result = []
    while remaining:
        start = min(remaining, key=lambda p: (p[1], p[0]))
        remaining.remove(start)
        group = {start}
        queue = deque([start])
        while queue:
            x, y = queue.popleft()
            for q in ((x-1,y), (x+1,y), (x,y-1), (x,y+1)):
                if q in remaining:
                    remaining.remove(q)
                    group.add(q)
                    queue.append(q)
        result.append(group)
    return result


def overlay(name, frame):
    if name == 'hurt' and frame in (0, 1):
        return ((0.4, 0.7)[frame], (255, 0, 17))
    if name == 'death' and frame in (10, 11, 12):
        return ((0.77, 0.45, 0.16)[frame-10], (255, 255, 255))
    return 1, (0, 0, 0)


def decode(tile, name, frame):
    factor, tint = overlay(name, frame)
    pixels = { (x,y): tile.getpixel((x,y)) for y in range(128) for x in range(128) if tile.getpixel((x,y))[3] }
    if factor == 1:
        return {p: c[:3] for p, c in pixels.items()}
    lookup = {}
    for observed in {c[:3] for c in pixels.values()}:
        def error(base):
            return sum((observed[i] - (factor*base[i]+(1-factor)*tint[i]))**2 for i in range(3))
        base = min(BASE, key=error)
        assert error(base) < 25, (name, frame, observed, base, error(base))
        lookup[observed] = base
    return {p: lookup[c[:3]] for p, c in pixels.items()}


def distance(group, target):
    return min(((x-a)**2+(y-b)**2 for x,y in group for a,b in target), default=100000)


def draft_mask(tile, name, frame, overrides):
    pixels = decode(tile, name, frame)
    mask = Image.new('L', (128,128))
    if name == 'death' and frame >= 13:
        return mask, [], []  # Authored white silhouette/dissolve VFX, preserve exactly.
    if name == 'idle':
        return Image.open(REFERENCE / 'masks' / f'idle-{frame+1:02}.png').convert('L'), [], []
    groups = {i: {rgb(c) for c in colors} for i, colors in enumerate(PALETTE.values(), 1)}
    pants = set()
    for group in components(p for p,c in pixels.items() if c in {rgb('1c548e'),rgb('143464')}):
        if any(pixels[p] == rgb('143464') for p in group):
            pants.update(group)
    armor = {p for p,c in pixels.items() if c == rgb('c0d5dc')}
    eyes = {p for p,c in pixels.items() if c in {rgb(c) for c in ('24523b','59c135','1a7a3e','b3b9d1')}}
    sword = {p for p,c in pixels.items() if c in {rgb(c) for c in ('a1c9eb','5f66be','231787','58c2ff','2a53b0')}}
    brown = components(p for p,c in pixels.items() if c in {rgb('a24e57'),rgb('d2947f')})
    hip_y = sum(y for x,y in pants)/len(pants) if pants else 1000
    chest = {p for p in armor if p[1] < hip_y-4}
    selected = [i for i,g in enumerate(brown) if distance(g,pants)<distance(g,chest) and sum(y for x,y in g)/len(g)>=hip_y-3]
    key = f'{name}/{frame+1:02}'
    selected = overrides.get(key, {}).get('boots', selected)
    boots = set().union(*(brown[i] for i in selected)) if selected else set()
    for x0,y0,x1,y1 in overrides.get(key, {}).get('bootRects', []):
        boots.update(p for p,c in pixels.items() if x0<=p[0]<x1 and y0<=p[1]<y1 and c in groups[4])
    # Purple boot shadows are distinct components, only choose those touching boots.
    for group in components(p for p,c in pixels.items() if c == rgb('5c2c4f')):
        if distance(group,boots) <= 1:
            boots.update(group)
    for p,c in pixels.items():
        label = 0
        if c in groups[1]: label = 1
        elif c in groups[2]: label = 2
        elif c in groups[3] and c not in {rgb('ffffff'),rgb('5c2c4f')}: label = 3
        if p in boots: label = 4
        mask.putpixel(p,label)
    whites = components(p for p,c in pixels.items() if c == rgb('ffffff'))
    white_selected = []
    for i,g in enumerate(whites):
        if len(g)<40 and distance(g,armor)<=2 and distance(g,eyes)>distance(g,armor) and distance(g,sword)>distance(g,armor):
            white_selected.append(i)
    white_selected = overrides.get(key, {}).get('armorWhite', white_selected)
    for i in white_selected:
        for p in whites[i]: mask.putpixel(p,3)
    return mask, brown, selected
