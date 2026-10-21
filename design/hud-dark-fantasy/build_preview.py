"""Build review artifacts from native QA screenshots; never render a mock gameplay state."""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[2]
OUT = Path(__file__).parent

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("baseline", type=Path)
    parser.add_argument("candidate", type=Path)
    parser.add_argument("--production-before", type=Path, required=True)
    parser.add_argument("--production-after", type=Path, required=True)
    args = parser.parse_args()
    previews = OUT / "preview"
    previews.mkdir(exist_ok=True)
    receipt = {"kind": "native real-input captures, independent runs in the same dungeon; actor frames may differ", "baseline": json.loads((args.baseline / "snapshot.json").read_text()), "candidate": json.loads((args.candidate / "snapshot.json").read_text()), "images": {}}
    production_snapshot = args.production_after / "snapshot.json"
    if production_snapshot.exists():
        receipt["productionCandidate"] = json.loads(production_snapshot.read_text())
    receipt["qaScope"] = "QA captures verify final HP ink/combo. Production additionally includes coal track flatten and pause palette."
    states = ["normal", "low-hp", "combo"]
    if args.production_before and args.production_after:
        states.insert(0, "normal-production")
    for state in states:
        for width, height in [(320, 180), (640, 360)]:
            pair = Image.new("RGB", (width * 2, height + 20), "#121014")
            draw = ImageDraw.Draw(pair)
            folders = [args.production_before, args.production_after] if state == "normal-production" else [args.baseline, args.candidate]
            for i, folder in enumerate(folders):
                source = folder / (f"combat-{width}x{height}-dpr1.png" if state == "normal-production" else f"{state}-{width}x{height}.png")
                image = Image.open(source).convert("RGB")
                assert image.size == (width, height)
                pair.paste(image, (i * width, 20))
                draw.text((i * width + 4, 4), "Cobblestone baseline" if i == 0 else "Dark fantasy HUD", fill="#f2e7cf")
                receipt["images"][source.name + ("-before" if i == 0 else "-after")] = hashlib.sha256(source.read_bytes()).hexdigest()
            pair.save(previews / f"{state}-{width}x{height}.png")
            if width == 320:
                pair.resize((pair.width * 3, pair.height * 3), Image.Resampling.NEAREST).save(previews / f"{state}-nearest-3x.png")
    (OUT / "preview-receipt.json").write_text(json.dumps(receipt, indent=2) + "\n")
    (OUT / "index.html").write_text("""<!doctype html><html lang="vi"><meta charset="utf-8"><title>HUD dark fantasy</title>
<style>body{background:#121014;color:#f2e7cf;font:16px system-ui;margin:24px}select{font:inherit}img{display:block;image-rendering:pixelated;max-width:none}figure{margin:16px 0;overflow:auto}a{color:#e5be72}</style>
<h1>HUD dark fantasy — local review</h1><p>Trái: Cobblestone gốc. Phải: HUD đồng cổ. Hai lượt chơi thật trong cùng dungeon; frame nhân vật có thể khác.</p>
<label>Trạng thái <select id="state"><option value="normal-production">Bình thường — production</option><option value="normal">Bình thường — QA</option><option value="low-hp">HP thấp</option><option value="combo">Combo</option></select></label>
<label>Kích thước <select id="size"><option>320x180</option><option>640x360</option><option value="nearest-3x">320x180 — nearest 3×</option></select></label>
<p>Ảnh QA có nhãn debug Spark sẵn có; player production không hiển thị nhãn này.</p>
<figure><img id="pair" alt="Trước và sau thay màu HUD"></figure><p><a href="pause-production.png">Pause production</a> · <a href="preview-receipt.json">Nguồn ảnh và hash</a> · <a href="contrast.json">Tương phản</a></p>
<script>function update(){document.querySelector('#pair').src='preview/'+document.querySelector('#state').value+'-'+document.querySelector('#size').value+'.png'}document.querySelectorAll('select').forEach(e=>e.onchange=update);update()</script></html>""", encoding="utf-8")
    print(f"PASS:{len(states)*2} actual paired previews and{len(states)} nearest-neighbor enlargements")

if __name__ == "__main__":
    main()
