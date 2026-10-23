"""Rasterize explicitly authored pose masks into linear depth data.

This is an offline artist operation, never called by a Unity builder. Coordinates
are source-cel pixels from the top left. Albedo supplies coverage only; its RGB
values never select depth or decide overlap. Requires existing Pillow tooling.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[2]


def rasterize(source, frames):
    albedo = Image.open(source).convert('RGBA')
    atlas = Image.new('RGBA', albedo.size)
    for frame in frames:
        x, y, w, h = frame['rect']
        box = (x, albedo.height-y-h, x+w, albedo.height-y)
        alpha = albedo.crop(box).getchannel('A')
        height = Image.new('L', (w, h), frame.get('baseDepth', 32))
        draw = ImageDraw.Draw(height)
        # Explicit back-to-front masks; no automatic anatomical classification.
        for layer in frame['layers']:
            shape, points, depth = layer['shape'], layer['points'], layer['depth']
            if shape == 'polygon':
                draw.polygon([tuple(p) for p in points], fill=depth)
            elif shape == 'ellipse':
                draw.ellipse(points, fill=depth)
            elif shape == 'line':
                draw.line([tuple(p) for p in points], fill=depth, width=layer['width'], joint='curve')
            else:
                raise ValueError('Unknown authored mask: '+shape)
        if frame.get('exclude'):
            mask = ImageDraw.Draw(alpha)
            for polygon in frame['exclude']:
                mask.polygon([tuple(p) for p in polygon], fill=0)
        zero = Image.new('L', (w, h))
        atlas.paste(Image.merge('RGBA', (height, zero, zero, alpha)), box[:2])
    return atlas


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--recipe', type=Path, required=True)
    parser.add_argument('--write', action='store_true')
    args = parser.parse_args()
    recipe = json.loads(args.recipe.read_text(encoding='utf-8'))
    for item in recipe['atlases']:
        source = ROOT/item['source']
        if hashlib.sha256(source.read_bytes()).hexdigest() != item['sourceSha256']:
            raise ValueError('Albedo changed; re-review pose masks: '+item['source'])
        meta = Path(str(source)+'.meta').read_text(encoding='utf-8')
        guid = re.search(r'^guid: (.+)$', meta, re.M).group(1)
        target = ROOT/'Assets/Art/Lighting/Depths'/(guid+'.png')
        image = rasterize(source, item['frames'])
        if args.write:
            target.parent.mkdir(parents=True, exist_ok=True)
            image.save(target)
        print(f'{item["source"]}: {len(item["frames"])} authored cels -> {target.name}')


if __name__ == '__main__':
    main()
