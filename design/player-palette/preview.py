"""Package a standalone local review page and PNG archive."""
import base64
import json
from zipfile import ZipFile, ZIP_DEFLATED
from materials import HERE, ROOT, SOURCE


def uri(path, mime='image/png'):
    return f'data:{mime};base64,'+base64.b64encode(path.read_bytes()).decode()


def build_preview():
    manifest=json.loads((HERE/'manifest.json').read_text())
    images={s['name']:[uri(SOURCE/'individual_sheets'/f"male_hero-{s['name']}.png"),
                       uri(HERE/'sheets'/f"male_hero-{s['name']}.png")] for s in manifest['sheets']}
    html=(HERE/'preview.template.html').read_text(encoding='utf-8')
    html=html.replace('__MANIFEST__',json.dumps(manifest)).replace('__IMAGES__',json.dumps(images))
    html=html.replace('__BACKGROUND__',uri(ROOT/'design/ui-kit/typinghero/v2/source/dungeon.svg','image/svg+xml'))
    (HERE/'preview.html').write_text(html,encoding='utf-8')
    with ZipFile(HERE/'player-dark-pack.zip','w',ZIP_DEFLATED) as archive:
        for path in sorted((HERE/'sheets').glob('*.png')):
            archive.write(path,path.relative_to(HERE))
        for name in ('male_hero.png','palette.json','manifest.json','README.md'):
            archive.write(HERE/name,name)


if __name__=='__main__':
    build_preview()
