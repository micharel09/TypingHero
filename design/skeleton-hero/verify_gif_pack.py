"""Independent PNG/native/source-mask roundtrip verification through LibreSprite."""
import argparse
import json
from pathlib import Path

from PIL import Image

from prepare_weapons import run

HERE=Path(__file__).resolve().parent
OUT=HERE/'gif-pack'
TEMP=Path('E:/CodexTemp/TypingHero/skeleton-hero-art/gif-pack-qa')


def main():
    parser=argparse.ArgumentParser()
    parser.add_argument('--libresprite',required=True)
    args=parser.parse_args()
    TEMP.mkdir(exist_ok=True)
    manifest=json.loads((OUT/'manifest.json').read_text())
    source=Image.open(HERE/'references'/manifest['source'])
    masks=Image.open(OUT/'ownership.png')
    assert masks.size==(220*58,120)
    for index,kind in enumerate(['sword','axe','sword-shield']):
        png=TEMP/(kind+'.png');metadata=TEMP/(kind+'.json')
        run(args.libresprite,OUT/(kind+'-White.aseprite'),'--sheet',png,
            '--data',metadata,'--format','json-array','--list-tags')
        native=Image.open(png).convert('RGBA')
        meta=json.loads(metadata.read_text())
        assert len(meta['frames'])==58
        tags={tag['name']:[tag['from'],tag['to']] for tag in meta['meta']['frameTags']}
        assert tags==manifest['clips'][kind]
        for name,(start,end) in tags.items():
            sheet=Image.open(OUT/kind/(name+'.png')).convert('RGBA')
            assert sheet.size==(96*(end-start+1),64)
            for f in range(start,end+1):
                source.seek(f)
                assert source.info['duration']==100 and meta['frames'][f]['duration']==100
                rgb=source.convert('RGB')
                small=rgb.resize((220,120),Image.Resampling.NEAREST)
                assert small.resize((1100,600),Image.Resampling.NEAREST).tobytes()==rgb.tobytes()
                expected=Image.new('RGBA',(96,64))
                ox,oy=manifest['origins'][index]
                for y in range(120):
                    for x in range(220):
                        if masks.getpixel((f*220+x,y))==index:
                            assert ox<=x<ox+96 and oy<=y<oy+64
                            expected.putpixel((x-ox,y-oy),(*small.getpixel((x,y)),255))
                cel=sheet.crop(((f-start)*96,0,(f-start+1)*96,64))
                rect=meta['frames'][f]['frame']
                actual=native.crop((rect['x'],rect['y'],rect['x']+96,rect['y']+64))
                assert cel.tobytes()==expected.tobytes()==actual.tobytes(),(kind,name,f)
                assert set(cel.getchannel('A').tobytes())<={0,255}
    print('PASS: 174 cels source-mask RGB/alpha/silhouette exact; 18 PNG and LibreSprite roundtrips, six tags per native, source order, 100ms, 96x64')


if __name__=='__main__':main()
