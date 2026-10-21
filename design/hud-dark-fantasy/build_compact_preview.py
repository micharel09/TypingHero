"""Pair actual production captures of the requested compact HUD follow-up."""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageDraw


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("before", type=Path)
    parser.add_argument("after", type=Path)
    args = parser.parse_args()
    output = Path(__file__).parent
    receipt = {"kind": "independent real production runs; actor frames/words may differ", "images": {}}
    for width, height in [(320, 180), (640, 360)]:
        pair = Image.new("RGB", (2 * width, height + 20), "#121014")
        draw = ImageDraw.Draw(pair)
        for index, folder in enumerate([args.before, args.after]):
            source = folder / f"combat-{width}x{height}-dpr1.png"
            image = Image.open(source).convert("RGB")
            assert image.size == (width, height)
            pair.paste(image, (index * width, 20))
            draw.text((index * width + 4, 4), "Previous HUD" if index == 0 else "Compact HUD", fill="#f2e7cf")
            receipt["images"][source.name + ("-before" if index == 0 else "-after")] = hashlib.sha256(source.read_bytes()).hexdigest()
        pair.save(output / f"compact-{width}x{height}.png")
        if width == 320:
            pair.resize((pair.width * 3, pair.height * 3), Image.Resampling.NEAREST).save(output / "compact-nearest-3x.png")
    (output / "compact-receipt.json").write_text(json.dumps(receipt, indent=2) + "\n")
    (output / "compact.html").write_text('''<!doctype html><html lang="vi"><meta charset="utf-8"><title>HUD gọn</title>
<style>body{background:#121014;color:#f2e7cf;font:16px system-ui;margin:24px}select{font:inherit}img{display:block;image-rendering:pixelated}figure{margin:16px 0;overflow:auto}a{color:#e5be72}</style>
<h1>HUD gọn</h1><p>Trái: bản trước. Phải: bản mới — bỏ số HP cả hai bên, cụm HUD hai bên scale 80%. Bảng điểm/combo và khay gõ giữ nguyên; dòng độ khó/DUEL tiếp tục ẩn.</p>
<label>Kích thước <select><option value="320x180">320×180</option><option value="640x360">640×360</option><option value="nearest-3x">320×180 — nearest 3×</option></select></label>
<figure><img id="pair" src="compact-320x180.png" alt="HUD trước và sau thu gọn"></figure>
<p>Hai lượt chơi thật; frame và từ gõ có thể khác. <a href="compact-receipt.json">Nguồn ảnh</a> · <a href="index.html">Preview màu trước đó</a></p>
<script>document.querySelector('select').onchange=e=>document.querySelector('#pair').src='compact-'+e.target.value+'.png'</script></html>''', encoding="utf-8")
    print("PASS: two production pairs and nearest enlargement")


if __name__ == "__main__":
    main()
