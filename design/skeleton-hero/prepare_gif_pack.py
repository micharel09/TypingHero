"""Extract visible source pixels only; no pose synthesis or palette conversion."""
import base64
import hashlib
import io
import json
from pathlib import Path
import struct
import zlib

from PIL import Image

from prepare_weapons import chunk, string

HERE = Path(__file__).resolve().parent
OUT = HERE / 'gif-pack'
SOURCE = HERE / 'references/monopixelart-full.gif'
KINDS = ['sword', 'axe', 'sword-shield']
STANDARD = dict(Idle=[0,7], Hurt=[8,12], Die=[13,25], Walk=[26,35],
                Attack1=[36,47], Attack2=[48,57])
AXE = dict(Idle=[0,7], Hurt=[8,12], Walk=[13,24], Die=[25,35],
           Attack1=[36,47], Attack2=[48,57])


def png_url(im):
    buffer = io.BytesIO()
    im.save(buffer, format='PNG')
    return 'data:image/png;base64,' + base64.b64encode(buffer.getvalue()).decode()


def native(path, frames, tags):
    """One composite source layer, source order, 100ms per cel."""
    encoded = []
    for i, im in enumerate(frames):
        chunks = []
        if i == 0:
            chunks.append(chunk(0x2004, struct.pack('<6HB3x',3,0,0,0,0,0,255)
                                + string('Visible White composite + VFX')))
            data = struct.pack('<H8x',len(tags))
            for name,(start,end) in sorted(tags.items(), key=lambda item:item[1][0]):
                data += struct.pack('<HHB8x3BB',start,end,0,100,180,180,0)+string(name)
            chunks.append(chunk(0x2018,data))
        chunks.append(chunk(0x2005,struct.pack('<HhhBH7xHH',0,0,0,255,2,96,64)
                            + zlib.compress(im.tobytes())))
        content = b''.join(chunks)
        encoded.append(struct.pack('<IHHH2xI',16+len(content),0xF1FA,len(chunks),100,0)+content)
    header = bytearray(128)
    struct.pack_into('<I5HIH',header,0,128+sum(map(len,encoded)),0xA5E0,58,96,64,32,1,100)
    header[34:36] = bytes([1,1])
    path.write_bytes(bytes(header)+b''.join(encoded))


def owners(frame, rgb):
    """Per-cel ownership in the 220x120 source grid. -1 means scenery.

    Default lanes split empty gaps. Exceptions follow visible weapon/VFX edges,
    including disconnected death fragments. Never infer hidden pixels.
    """
    mask = Image.new('L',(220,120),255)
    left_split = 90 if 13<=frame<=25 else 70 if 26<=frame<=35 else 80
    right_split = 140 if 13<=frame<=25 else 144
    for y in range(18,82):
        for x in range(6,215):
            if rgb.getpixel((x,y)) != (104,83,65):
                mask.putpixel((x,y),0 if x<left_split else 1 if x<right_split else 2)
    # Moving lane boundaries follow separated tips, rather than clipping at
    # a fixed third of the image. Empty background remains unassigned.
    if frame in (39,40):
        for y in range(18,82):
            for x in range(136,144):
                if mask.getpixel((x,y))!=255:mask.putpixel((x,y),2)
    if frame in (42,43):
        for y in range(18,82):
            for x in range(80,96):
                if mask.getpixel((x,y))!=255:mask.putpixel((x,y),0)
        for y in range(18,44):
            for x in range(140,160):
                if mask.getpixel((x,y))!=255:mask.putpixel((x,y),1)
    if frame==49:
        for y in range(18,45):
            for x in range(75,96):
                if mask.getpixel((x,y))!=255:mask.putpixel((x,y),1)
    # Sword sweep ends at x93; axe's dark shaft starts behind its edge.
    if frame == 41:
        for y in range(40,65):
            for x in range(80,95):
                color = rgb.getpixel((x,y))
                if mask.getpixel((x,y)) != 255:
                    mask.putpixel((x,y),0 if min(color)>=190 else 1)
    if frame == 42:
        for y in range(18,82):
            for x in range(80,96):
                if mask.getpixel((x,y)) != 255: mask.putpixel((x,y),0)
        # Axe sweep covers the left portion of the shield. Red shield pixels
        # and the exposed forearm on its right retain shield ownership.
        for y in range(18,82):
            for x in range(140,158):
                c = rgb.getpixel((x,y))
                if mask.getpixel((x,y)) != 255 and min(c)>=190:
                    mask.putpixel((x,y),1)
                    if x>=156 and 51<=y<=53:mask.putpixel((x,y),2)
    if frame in (50,51):
        # Axe head crosses the sword thrust. Polygon traces the visible head;
        # the diagonal wood/steel shaft belongs to axe, white thrust behind
        # it belongs to sword. This retains occlusion holes in both exports.
        other = Image.open(SOURCE)
        other.seek(51 if frame==50 else 50)
        other = other.convert('RGB').resize((220,120),Image.Resampling.NEAREST)
        shift = -1 if frame==50 else 1
        for y in range(43,61):
            for x in range(60,96):
                c = rgb.getpixel((x,y))
                if mask.getpixel((x,y)) == 255:
                    continue
                axe = (67<=x<=85 and y<=53 and c==other.getpixel((x+shift,y)))
                axe |= x>=76 and c[0]>c[2]*1.3
                axe |= x>=94
                # Exposed axe hand around the shaft.
                axe |= (85<=x<=89 and 53<=y<=56)
                mask.putpixel((x,y),1 if axe else 0)
    if frame == 52:
        # Long axe smear starts at the sword tip and passes over shield.
        for y in range(47,74):
            for x in range(73,159):
                c = rgb.getpixel((x,y))
                if mask.getpixel((x,y)) == 255:
                    continue
                if x<96:
                    mask.putpixel((x,y),0 if y<=53 else 1)
                elif x>=144 and min(c)>=190:
                    mask.putpixel((x,y),1)
    if frame in (52,53,54,55):
        for y in range(18,54):
            for x in range(144,160):
                c=rgb.getpixel((x,y))
                if mask.getpixel((x,y))!=255 and (min(c)>=190 or c in [(73,16,9),(112,39,7)]):
                    mask.putpixel((x,y),1)
    # Border is scenery even when VFX meets the right frame.
    for y in range(120):
        for x in range(215,220):
            if rgb.getpixel((x,y)) in [(104,83,65),(113,64,39),(202,174,119)]:
                mask.putpixel((x,y),255)
    return mask


def main():
    OUT.mkdir(exist_ok=True)
    protected = {str(p.relative_to(HERE)):hashlib.sha256(p.read_bytes()).hexdigest()
                 for p in HERE.rglob('*') if p.is_file()
                 and OUT not in p.parents and '__pycache__' not in p.parts
                 and p.name not in ['prepare_gif_pack.py','README.md']}
    src = Image.open(SOURCE)
    assert src.size == (1100,600) and src.n_frames == 58
    frames = {kind:[] for kind in KINDS}
    source_images, masks = [], []
    for i in range(58):
        src.seek(i)
        assert src.info['duration'] == 100
        rgb = src.convert('RGB')
        small = rgb.resize((220,120),Image.Resampling.NEAREST)
        assert small.resize(rgb.size,Image.Resampling.NEAREST).tobytes() == rgb.tobytes(), (i,'5x grid')
        ownership = owners(i,small)
        masks.append(ownership)
        source_images.append(small)
        for k,kind in enumerate(KINDS):
            cel = Image.new('RGBA',(96,64))
            ox = k*64
            for y in range(18,82):
                for x in range(220):
                    if ownership.getpixel((x,y)) == k:
                        assert ox <= x < ox+96,(i,kind,x,y,'clipped')
                        cel.putpixel((x-ox,y-18),(*small.getpixel((x,y)),255))
            assert cel.getbbox() and set(cel.getchannel('A').tobytes()) <= {0,255}
            frames[kind].append(cel)
    payload = dict(kinds=KINDS,clips={},frames={},source=[png_url(im) for im in source_images],
                   origins=[[k*64,18] for k in range(3)],durationMs=100)
    for kind in KINDS:
        tags = AXE if kind=='axe' else STANDARD
        payload['clips'][kind] = tags
        payload['frames'][kind] = [png_url(im) for im in frames[kind]]
        folder = OUT/kind
        folder.mkdir(exist_ok=True)
        for name,(start,end) in tags.items():
            sheet = Image.new('RGBA',(96*(end-start+1),64))
            for j,cel in enumerate(frames[kind][start:end+1]):
                sheet.paste(cel,(j*96,0))
            sheet.save(folder/(name+'.png'))
            check=Image.open(folder/(name+'.png')).convert('RGBA')
            for j,cel in enumerate(frames[kind][start:end+1]):
                assert check.crop((j*96,0,(j+1)*96,64)).tobytes()==cel.tobytes()
        native(OUT/(kind+'-White.aseprite'),frames[kind],tags)
    # Auditable per-frame labels: 0 sword, 1 axe, 2 shield, 255 scenery.
    mask_sheet=Image.new('L',(220*58,120),255)
    for i,mask in enumerate(masks):mask_sheet.paste(mask,(220*i,0))
    mask_sheet.save(OUT/'ownership.png')
    (OUT/'manifest.json').write_text(json.dumps(dict(source=SOURCE.name,
        sourceSHA256=hashlib.sha256(SOURCE.read_bytes()).hexdigest(),canvas=[96,64],
        frames=58,durationMs=100,origins=payload['origins'],clips=payload['clips'],
        occlusionFrames=[41,42,50,51,52,53,54],layer='Visible White composite + VFX',
        preservation=dict(files=len(protected),sha256=hashlib.sha256(
            json.dumps(protected,sort_keys=True).encode()).hexdigest())),indent=2)+'\n')
    template=(HERE/'gif-pack-preview.template.html').read_text(encoding='utf8')
    (OUT/'review.html').write_text(template.replace('__DATA__',json.dumps(payload)),encoding='utf8')
    assert all(hashlib.sha256((HERE/p).read_bytes()).hexdigest()==h for p,h in protected.items())
    print(f'PASS: 58 uniform 5x frames, 174 extracted cels, 18 sheets, 3 natives; {len(protected)} prior files preserved')


if __name__ == '__main__':
    main()
