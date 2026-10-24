"""Lossless Pimen layer/crop export. Run from the repository root (Pillow required)."""
import hashlib
import json
from pathlib import Path
import struct
import zlib
from PIL import Image

SOURCE = Path('Assets/Art/VFX/Pimen/HitSpark/Hit Effect 01/Hit Effect 1.aseprite')
OUTPUT = Path('Assets/Art/VFX/ParryClash')


def export():
    data = SOURCE.read_bytes()
    _, magic, count, width, height, depth = struct.unpack_from('<IHHHHH', data)
    assert (magic, width, height, depth) == (0xA5E0, 48, 48, 32)
    layers, frames, durations = [], [], []
    offset = 128
    for frame in range(count):
        size, marker, chunks, duration = struct.unpack_from('<IHHH', data, offset)
        assert marker == 0xF1FA
        cels = {}
        cursor = offset + 16
        for _ in range(chunks):
            length, kind = struct.unpack_from('<IH', data, cursor)
            body = data[cursor + 6:cursor + length]
            if kind == 0x2004:
                name_length = struct.unpack_from('<H', body, 16)[0]
                layers.append(body[18:18 + name_length].decode())
            elif kind == 0x2005:
                layer, x, y, opacity, encoding = struct.unpack_from('<HhhBH', body)
                assert opacity == 255 and encoding == 2
                w, h = struct.unpack_from('<HH', body, 16)
                cel = Image.frombytes('RGBA', (w, h), zlib.decompress(body[20:]))
                canvas = Image.new('RGBA', (48, 48))
                canvas.paste(cel, (x, y))
                cels[layers[layer]] = canvas
            cursor += length
        frames.append(cels)
        durations.append(duration)
        offset += size
    assert durations[:7] == [80] * 7
    clear = Image.new('RGBA', (48, 48))
    sheet = Image.new('RGBA', (336, 48))
    original = Image.open(SOURCE.with_name('Hit Effect 01 1.png')).convert('RGBA')
    for f in range(7):
        core = Image.alpha_composite(frames[f].get('Hit Effect', clear), frames[f].get('Color', clear))
        sheet.paste(core, (f * 48, 0))
        # Independently confirm layer composition/order against the shipped palette-one PNG.
        composite = clear.copy()
        for layer in ('Hit Effect', 'Particle', 'Particle 2', 'Color'):
            composite = Image.alpha_composite(composite, frames[f].get(layer, clear))
        expected = original.crop((f * 48, 0, (f + 1) * 48, 48))
        assert composite.tobytes() == expected.tobytes(), f
    sheet.save(OUTPUT / 'PimenCore.png')
    crops = [('PimenRay', 'Particle 2', 2, (35, 2, 38, 7)),
             ('PimenFragment', 'Particle', 1, (26, 13, 30, 16)),
             ('PimenSpeck', 'Particle', 1, (26, 13, 27, 14))]
    for name, layer, frame, box in crops:
        frames[frame][layer].crop(box).save(OUTPUT / (name + '.png'))
    receipt = {'source': str(SOURCE).replace('\\', '/'),
               'url': 'https://pimen.itch.io/battle-vfx-hit-spark',
               'sha256': hashlib.sha256(data).hexdigest(),
               'core': {'layers': ['Hit Effect', 'Color'], 'frames': list(range(7)),
                        'cell': [48, 48], 'pivot': [24, 24], 'duration_ms': 80},
               'crops': [{'name': n, 'layer': l, 'frame': f, 'box_top_left_exclusive': b}
                         for n, l, f, b in crops],
               'outputs': {p.name: hashlib.sha256(p.read_bytes()).hexdigest()
                           for p in sorted(OUTPUT.glob('Pimen*.png'))}}
    (OUTPUT / 'PimenSource.json').write_text(json.dumps(receipt, indent=2) + '\n', encoding='utf-8')


if __name__ == '__main__':
    export()
