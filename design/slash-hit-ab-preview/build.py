"""Build a private offline comparison from the authors' public promotional GIFs.

Requires Pillow. Run: python design/slash-hit-ab-preview/build.py
Only code/provenance is committed; downloaded promotional media stays in local/.
"""
import base64
import hashlib
import io
import json
from pathlib import Path
import urllib.request

from PIL import Image

ROOT = Path(__file__).resolve().parent
SOURCES = {
    'a': 'https://img.itch.zone/aW1hZ2UvMTk4NDQ5NC8xMTY2OTUzNC5naWY=/original/VlLIuI.gif',
    'sheet': 'https://img.itch.zone/aW1hZ2UvMTk4NDQ5NC8xMTY2OTM5MS5naWY=/original/l7UxUi.gif',
    'b': 'https://img.itch.zone/aW1nLzI4MDAzMDI0LmdpZg==/original/1%2B4p2y.gif',
}
REVIEWED_HASHES = {
    'a': '73add428aa4e91c0f01107ff9235ef12dd08ffb1ee409ed0f4b8eade556ba5e1',
    'sheet': '7967ca59d73611d1d9668ef2bff8b483b8f7659a48b84cce70b55770bae1feae',
    'b': '9a6e0a01cefb1b10a7db3b6c250259a1a1b3f414861f136654395674ab5cb139',
}


def png_data(image):
    buf = io.BytesIO()
    image.save(buf, format='PNG')
    return 'data:image/png;base64,' + base64.b64encode(buf.getvalue()).decode()


def main():
    output = ROOT / 'local'
    output.mkdir(exist_ok=True)
    data = {}
    for name, url in SOURCES.items():
        path = output / (name + '.gif')
        if not path.exists():
            with urllib.request.urlopen(url, timeout=30) as response:
                path.write_bytes(response.read())
        raw = path.read_bytes()
        if hashlib.sha256(raw).hexdigest() != REVIEWED_HASHES[name]:
            raise ValueError(f'{name}: public preview changed; review the new source before rebuilding')
        image = Image.open(io.BytesIO(raw))
        frames = []
        for index in range(image.n_frames):
            image.seek(index)
            frames.append({'src': png_data(image.convert('RGBA')),
                           'ms': image.info.get('duration', 0)})
        data[name] = {'frames': frames, 'size': list(image.size), 'url': url,
                      'sha256': hashlib.sha256(raw).hexdigest()}
    # A single public montage contains all three slash + hit variants, including
    # preparation and blank ending. Do not infer missing frames from the sheet.
    assert len(data['a']['frames']) == 26 and data['a']['size'] == [376, 314]
    assert len(data['b']['frames']) == 6 and data['b']['size'] == [1260, 1000]
    assert [f['ms'] for f in data['a']['frames']] == [100] * 26
    assert [f['ms'] for f in data['b']['frames']] == [100, 100, 100, 100, 80, 100]
    html = (ROOT / 'template.html').read_text(encoding='utf-8')
    html = html.replace('/*PREVIEW_DATA*/', 'const DATA = ' + json.dumps(data) + ';')
    html = html.replace('/*PREVIEW_PLAYER*/', (ROOT / 'player.js').read_text(encoding='utf-8'))
    target = output / 'preview.html'
    target.write_text(html, encoding='utf-8')
    (output / 'provenance.json').write_text(json.dumps(
        {k: {p: v[p] for p in ('size', 'url', 'sha256')} for k, v in data.items()},
        indent=2), encoding='utf-8')
    print(target)


if __name__ == '__main__':
    main()
