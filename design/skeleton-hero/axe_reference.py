"""Lossless native-grid extraction of the central skeleton in the source GIF."""
import math

from PIL import Image

from build_preview import DESIGN

SOURCE = DESIGN / 'references/monopixelart-pack-overview.gif'
BACKGROUND = (104, 83, 65)
SHAFT_LENGTH = 49
BONE = {(250,250,250), (191,203,214), (139,152,170), (103,108,134), (29,29,49)}
GOLD = dict(zip([(250,250,250),(191,203,214),(139,152,170),(103,108,134),(29,29,49)],
                [(252,247,215),(235,199,117),(177,130,79),(163,98,56),(36,30,23)]))


def frames():
    source = Image.open(SOURCE)
    assert source.size == (960,400) and source.n_frames == 9
    result = []
    for i in range(9):
        source.seek(i)
        assert source.info['duration'] == 100
        rgb = source.convert('RGB')
        cel = Image.new('RGBA',(96,64))
        for y in range(28,81):
            for x in range(100,144):
                block = rgb.crop((x*4,y*4,x*4+4,y*4+4))
                colors = block.getcolors()
                assert len(colors) == 1, (i,x,y,'not a native 4x block')
                color = colors[0][1]
                if color != BACKGROUND:
                    cel.putpixel((x-72,y-17),(*color,255))
        result.append(cel)
    return result


def yellow(cel):
    result = cel.copy()
    # Material ownership is spatial: the steel highlight shares a bone RGB.
    top = min(y for y in range(64) for x in range(56,70) if cel.getpixel((x,y))[3])
    steel_bottom = 29 + top - 14
    for y in range(64):
        for x in range(96):
            rgba = cel.getpixel((x,y))
            if not (x >= 56 and y <= steel_bottom) and rgba[3] and rgba[:3] in BONE:
                result.putpixel((x,y),(*GOLD[rgba[:3]],255))
    return result


def weapon_pixels():
    cel = frames()[0]
    return [[x,y,*cel.getpixel((x,y))[:3]] for y in range(64) for x in range(56,70)
            if cel.getpixel((x,y))[3] and (y <= 29 or cel.getpixel((x,y))[:3] in [(73,16,9),(112,39,7)])]


def posed_weapon(p, pixels):
    """Independent inverse-sampling check of the rendered material envelope."""
    length = math.dist(p['butt'],p['tip'])
    ux,uy = [(p['tip'][j]-p['butt'][j])/length for j in range(2)]
    cu,cv = 3/math.hypot(3,49),-49/math.hypot(3,49)
    lookup = {(x,y):tuple(rgb) for x,y,*rgb in pixels}
    result = {}
    for y in range(-20,84):
        for x in range(-20,116):
            dx,dy = x-p['tip'][0],y-p['tip'][1]
            a,b = dx*ux+dy*uy,-dx*uy+dy*ux
            # JavaScript Math.round rounds half toward positive infinity.
            source = (math.floor(61+a*cu-b*cv+.5),math.floor(14+a*cv+b*cu+.5))
            if source in lookup:
                assert 0 <= x < 96 and 0 <= y < 64, (p,'weapon clipped',x,y)
                result[(x,y)] = (*lookup[source],255)
    return result
