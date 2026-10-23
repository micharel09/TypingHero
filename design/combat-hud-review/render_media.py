"""Encode the deterministic UI storyboard and contact sheets for visual review."""
import argparse
import json
import shutil
import subprocess
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent


def contact_sheets(paths, dest, prefix, labels, thumb=(640, 210), per_sheet=8):
    font = ImageFont.truetype('C:/Windows/Fonts/consola.ttf', 13)
    dest.mkdir(exist_ok=True, parents=True)
    names = []
    for start in range(0, len(paths), per_sheet):
        w, h = thumb
        sheet = Image.new('RGB', (w * 2, (h + 22) * (per_sheet // 2)), '#100F13')
        draw = ImageDraw.Draw(sheet)
        for j, path in enumerate(paths[start:start + per_sheet]):
            x, y = j % 2 * w, j // 2 * (h + 22)
            image = Image.open(path).convert('RGB').resize(thumb, Image.Resampling.NEAREST)
            sheet.paste(image, (x, y + 22))
            draw.text((x + 4, y + 3), labels[start + j], font=font, fill='#E5BE72')
        name = f'{prefix}-{start // per_sheet:02d}.jpg'
        sheet.save(dest / name, quality=96)
        names.append(name)
    return names


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--ffmpeg', type=Path, required=True)
    ap.add_argument('--review-cache', type=Path, required=True)
    ap.add_argument('--source-video', type=Path, required=True)
    args = ap.parse_args()
    frames = sorted((ROOT / '.render-frames').glob('*.png'))
    if len(frames) != 144:
        raise RuntimeError('Run verify.cjs --video-frames first; expected exactly 144 frames.')
    output = ROOT / 'exports/comparison-storyboard.mp4'
    subprocess.run([str(args.ffmpeg), '-v', 'error', '-framerate', '12', '-i',
                    str(ROOT / '.render-frames/%04d.png'), '-c:v', 'libx264', '-crf', '18',
                    '-pix_fmt', 'yuv420p', '-movflags', '+faststart', '-an', '-y', str(output)], check=True)
    # Review the delivered encoding, not just its input PNGs.
    decoded = args.review_cache / 'delivered'; decoded.mkdir(parents=True, exist_ok=True)
    subprocess.run([str(args.ffmpeg), '-v', 'error', '-i', str(output), '-fps_mode', 'passthrough',
                    '-start_number', '0', '-y', str(decoded / '%04d.png')], check=True)
    delivered = sorted(decoded.glob('*.png'))
    shutil.copyfile(delivered[0], ROOT / 'exports/comparison.png')
    sheets = contact_sheets(delivered, args.review_cache, 'storyboard',
        [f'frame {i:03d} / {i / 12:.3f}s' for i in range(len(delivered))])
    record = {'storyboard': {'video': output.name, 'fps': 12, 'frames': len(delivered),
              'transitions': len(delivered) - 1, 'reviewed_frames': 0, 'sheets': sheets}, 'source_windows': []}
    # Declare selected windows and purpose before extracting/reviewing their frames.
    for name, begin, end in [('entry', 20.8, 22.0), ('exit', 30.8, 32.0)]:
        record['source_windows'].append({'name': name, 'video': args.source_video.name,
            'start': begin, 'end': end, 'purpose': 'Complete camera/band transition: inspect actor clearance only; no sprite-art or gameplay-timing claim.',
            'reviewed_frames': 0})
    (args.review_cache / 'review-counts.json').write_text(json.dumps(record, indent=2), encoding='utf-8')
    probe = args.ffmpeg.with_name('ffprobe.exe')
    metadata = json.loads(subprocess.check_output([str(probe), '-v', 'error', '-select_streams', 'v:0',
        '-show_entries', 'frame=best_effort_timestamp_time,pkt_duration_time', '-of', 'json', str(args.source_video)]))
    for window in record['source_windows']:
        folder = args.review_cache / window['name']; folder.mkdir(exist_ok=True)
        subprocess.run([str(args.ffmpeg), '-v', 'error', '-i', str(args.source_video), '-vf',
            f"trim=start={window['start']}:end={window['end']},setpts=PTS-STARTPTS", '-fps_mode', 'passthrough',
            '-start_number', '0', '-y', str(folder / '%04d.png')], check=True)
        pts = [f for f in metadata['frames'] if window['start'] <= float(f['best_effort_timestamp_time']) < window['end']]
        paths = sorted(folder.glob('*.png')); assert len(paths) == len(pts)
        window['frames'] = len(paths); window['transitions'] = len(paths) - 1
        window['sheets'] = contact_sheets(paths, args.review_cache, window['name'],
            [f"frame {i:03d} / source {float(f['best_effort_timestamp_time']):.3f}s" for i, f in enumerate(pts)], thumb=(480, 270), per_sheet=12)
        (args.review_cache / (window['name'] + '-pts.json')).write_text(json.dumps(pts, indent=2), encoding='utf-8')
    (args.review_cache / 'review-counts.json').write_text(json.dumps(record, indent=2), encoding='utf-8')
    print(json.dumps(record, indent=2))


if __name__ == '__main__':
    main()
