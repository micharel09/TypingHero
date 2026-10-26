"""Pack whole 16:9 gameplay PNG frames with one nearest-neighbor resize.
Usage: python tutorial_pack.py selection.json destination
Clip events use the 640x360 display coordinates. Source timing and hashes are retained.
"""
import hashlib
import json
import math
import sys
from pathlib import Path
from PIL import Image


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def pack(selection, output):
    output.mkdir(parents=True, exist_ok=True)
    for name, clip in selection['clips'].items():
        root = Path(clip['source'])
        timeline = sorted(json.loads((root / 'frames.json').read_text()), key=lambda f: f['timestamp'])
        events = sorted(clip['events'], key=lambda e: e['timestamp'])
        frames, images = [], []
        for index, frame in enumerate(timeline[:-1]):
            timestamp = frame['timestamp']
            if not clip['start'] <= timestamp < clip['end']:
                continue
            path = root / frame['file']
            with Image.open(path) as original:
                if original.width * 9 != original.height * 16:
                    raise ValueError(f'Expected full 16:9 frame: {path}')
                size = list(original.size)
                image = original.convert('RGBA').resize((320, 180), Image.Resampling.NEAREST)
            event = next(e for e in reversed(events) if e['timestamp'] <= timestamp)
            duration = min(timeline[index + 1]['timestamp'], clip['end']) - timestamp
            if duration <= 0:
                raise ValueError(f'Invalid duration: {path}')
            images.append(image)
            frames.append(dict(source=str(path), sha256=digest(path), timestamp=timestamp,
                               duration=duration, sourceSize=size, width=320, height=180,
                               resizedRgbaSha256=hashlib.sha256(image.tobytes()).hexdigest(),
                               atlas=(len(images) - 1) // 66, caption=event['caption'],
                               focus=event['focus'], label={'x': 144, 'y': 414}))
        if not frames:
            raise ValueError(f'Empty clip: {name}')
        files = []
        for page in range(math.ceil(len(images) / 66)):
            group = images[page * 66:(page + 1) * 66]
            atlas = Image.new('RGBA', (1944, math.ceil(len(group) / 6) * 184))
            for index, image in enumerate(group):
                atlas.paste(image, (index % 6 * 324 + 2, index // 6 * 184 + 2))
            filename = f'{name}-{page}.png'
            atlas.save(output / filename)
            files.append(filename)
        manifest = dict(columns=6, cellWidth=324, cellHeight=184, width=320, height=180,
                        atlases=files, selection=clip, frames=frames,
                        transform='full-frame RGBA resize 320x180 nearest-neighbor only',
                        sourceTimelineSha256=digest(root / 'frames.json'),
                        atlasSha256={f: digest(output / f) for f in files})
        (output / f'{name}.json').write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
        print(name, len(frames), sum(f['duration'] for f in frames))


if __name__ == '__main__':
    pack(json.loads(Path(sys.argv[1]).read_text(encoding='utf-8')), Path(sys.argv[2]))
