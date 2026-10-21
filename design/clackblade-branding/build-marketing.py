"""Replace only cover title band and initial still; preserve the hover timeline."""
import argparse, hashlib, json
from pathlib import Path
from PIL import Image, ImageSequence, ImageChops
ROOT=Path(__file__).resolve().parents[2]
parser=argparse.ArgumentParser();parser.add_argument('--rendered-cover',type=Path,required=True);args=parser.parse_args()
source=ROOT/'design/clackblade-branding/before';out=ROOT/'marketing/itch-io/game-page'
before=Image.open(source/'cover.png').convert('RGB');render=Image.open(args.rendered_cover).convert('RGB');assert render.size==(630,500)
cover=before.copy();cover.paste(render.crop((0,0,630,116)),(0,0));assert ImageChops.difference(before.crop((0,116,630,500)),cover.crop((0,116,630,500))).getbbox() is None
for w,h in [(630,500),(315,250),(158,125)]:cover.resize((w,h),Image.Resampling.NEAREST).save(ROOT/f'marketing/ItchIO_Optimization/Thumbnails/clackblade-{w}x{h}.png')
old=Image.open(source/'cover.gif');frames=[];durations=[];original=[];first=None;initial_ms=0
for frame in ImageSequence.Iterator(old):
 rgb=frame.convert('RGB');duration=frame.info['duration'];original.append((hashlib.sha256(rgb.tobytes()).hexdigest(),duration))
 if first is None:first=rgb.tobytes()
 if rgb.tobytes()==first and not frames:
  initial_ms+=duration;continue
 frames.append(rgb.quantize(colors=256));durations.append(duration)
static=cover.quantize(colors=256);gif=out/'cover.gif';static.save(gif,save_all=True,append_images=frames,duration=[initial_ms]+durations,loop=0,disposal=1,optimize=True)
def merged(values):
 result=[]
 for digest,duration in values:
  if result and result[-1][0]==digest:result[-1]=(digest,result[-1][1]+duration)
  else:result.append((digest,duration))
 return result
new=Image.open(gif);timeline=[(hashlib.sha256(f.convert('RGB').tobytes()).hexdigest(),f.info['duration']) for f in ImageSequence.Iterator(new)]
assert merged(timeline)[1:]==merged(original)[1:],'Hover gameplay pixels/timing changed'
assert sum(d for _,d in timeline)==sum(d for _,d in original)
assert gif.stat().st_size<3_000_000
metadata=dict(size=list(new.size),duration_ms=sum(d for _,d in timeline),initial_still_ms=initial_ms,bytes=gif.stat().st_size,
 sha256=hashlib.sha256(gif.read_bytes()).hexdigest(),gameplay_timeline_unchanged=True,source_gif_sha256=hashlib.sha256((source/'cover.gif').read_bytes()).hexdigest())
(ROOT/'design/clackblade-branding/cover.json').write_text(json.dumps(metadata,indent=2)+'\n');print(json.dumps(metadata))
