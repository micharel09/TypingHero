"""Package existing art and immutable capture frames for the offline design board.

No Unity assets, gameplay code, or source screenshots are modified.
"""
import argparse
import base64
import hashlib
import json
import re
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = Path(__file__).resolve().parent


def uri(path):
    ext = path.suffix.lower()
    mime = {'.png': 'image/png', '.jpg': 'image/jpeg', '.ttf': 'font/ttf'}[ext]
    return f'data:{mime};base64,' + base64.b64encode(path.read_bytes()).decode()


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--capture-root', type=Path, required=True)
    ap.add_argument('--references', type=Path, required=True)
    ap.add_argument('--ffmpeg', type=Path, required=True)
    args = ap.parse_args()
    report = json.loads((args.capture_root / 'still-capture/browser.json').read_text())
    run = report['runs'][0]
    # Selected by direct inspection of the video, rather than assuming that the
    # browser observation clock and first encoded video frame start together.
    chosen = {'normal': 17.5, 'slayer': 25.0, 'entry': 21.4, 'exit': 31.6}
    evidence = {'purpose': 'Design storyboard only; not runtime HUD or timing validation.',
                'source_snapshot': json.loads((args.capture_root / 'still-capture/snapshot.json').read_text()),
                'source_video': Path(run['video']).name, 'frames': {},
                'fixture_writes': report['fixtureWrites'], 'exceptions': report['exceptions'],
                'clock_note': 'Video PTS defines selected pictures. Nearest QA is context, not exact frame synchronization.'}
    coverage = json.loads((args.capture_root / 'capture/browser.json').read_text())
    coverage_run = coverage['runs'][0]
    evidence['source_capture_coverage'] = {
        'status': coverage['status'], 'video': Path(coverage_run['video']).name,
        'roles': coverage_run['roles'], 'fixture_writes': coverage['fixtureWrites'],
        'exceptions': coverage['exceptions'],
        'scope': 'Existing-player source capture only; no new HUD gameplay acceptance.'}
    assets = {}
    for name, time in chosen.items():
        dest = args.capture_root / f'board-{name}.png'
        subprocess.run([str(args.ffmpeg), '-v', 'error', '-ss', str(time), '-i', run['video'],
                        '-frames:v', '1', '-y', str(dest)], check=True)
        assets[name] = uri(dest)
        nearest = min(run['samples'], key=lambda row: abs(row['at'] - time))
        q = nearest['qa']
        evidence['frames'][name] = {'source_seconds': time,
            'sha256': hashlib.sha256(dest.read_bytes()).hexdigest(),
            'nearest_observation_seconds': nearest['at'],
            'slayer': q['slayer'], 'band_reveal': q['barReveal'],
            'popup_alpha': q['popupAlpha'], 'overhead': q['overheadTyping']}
    skin = ROOT / 'Assets/Art/UI/CombatR7/DarkFantasy'
    for name in ['typing-frame', 'score-frame', 'resource-frame', 'secondary-frame',
                 'keycap-space', 'icon-hp', 'icon-shield', 'icon-pause']:
        assets[name] = uri(skin / (name + '.png'))
    assets['font'] = uri(ROOT / 'Assets/Art/UI/Branding/Cobblestone.ttf')
    words = (ROOT / 'Assets/Data/Words/WP_Common200.asset').read_text()
    raw = words.split("rawWords: '", 1)[1].split("'", 1)[0]
    raw_words = [line.strip() for line in raw.splitlines() if line.strip()]
    pool = list(dict.fromkeys(word.lower() for word in raw_words))
    refs = [
        ('Type Knight', '1006220-1.jpg', 'https://store.steampowered.com/app/1006220/Type_Knight/',
         'Một nhãn nhỏ cho một mục tiêu. Ba từ trong cảnh cận chiến cần vùng đọc riêng.'),
        ('Furi', '423230-1.jpg', 'https://store.steampowered.com/app/423230/Furi/',
         'HP sát mép; vùng giao chiến dành cho động tác và hiệu ứng. Không sao chép timing parry.'),
        ('NecroDancer', '247080-1.jpg', 'https://store.steampowered.com/app/247080/Crypt_of_the_NecroDancer/',
         'Đích nhịp ở giữa và hai phía đối xứng. Mới xác minh hình tĩnh của dải nhịp.'),
        ('Sea of Stars', '1244090-0.jpg', 'https://seaofstarsgame.co/',
         'Gom thông tin theo vai trò. Không bê menu đánh theo lượt vào trận đấu thời gian thực.'),
        ('Blasphemous', '774361-1.jpg', 'https://store.steampowered.com/app/774361/Blasphemous/',
         'Chất liệu dark fantasy nằm ở viền và góc; giữ nền chữ sạch.')]
    references = []
    for name, file, url, lesson in refs:
        # A browser displays the source photograph unchanged, as a reference only.
        references.append({'name': name, 'url': url, 'lesson': lesson,
                           'image': uri(args.references / file)})
    data = {'assets': assets, 'words': pool, 'rawWordCount': len(raw_words),
            'references': references, 'source': evidence}
    (OUT / 'review-data.js').write_text('window.HUD_REVIEW_DATA = ' +
        json.dumps(data, ensure_ascii=False, separators=(',', ':')) + ';\n', encoding='utf-8')
    (OUT / 'source-evidence.json').write_text(json.dumps(evidence, indent=2) + '\n', encoding='utf-8')
    reference_review = json.loads((args.references / 'reference-review.json').read_text())
    # Keep provenance and counts, without paths to temporary contact sheets.
    for video in reference_review['videos']:
        video.pop('sheets', None)
        video.pop('reviewed_sheets', None)
    (OUT / 'reference-evidence.json').write_text(json.dumps(reference_review, indent=2) + '\n', encoding='utf-8')
    print(f'Packaged {len(pool)} words; {len(references)} references; {len(chosen)} unchanged capture frames.')


if __name__ == '__main__':
    main()
