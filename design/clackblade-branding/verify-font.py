"""Verify reconstruction metrics and strings; write temporary evidence to --output."""
import argparse
from collections import Counter
import hashlib
import json
import unicodedata
from pathlib import Path

from fontTools.ttLib import TTFont
from PIL import Image, ImageChops, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[2]
ART = ROOT / "Assets/Art/UI/Branding"
SOURCE = Path(__file__).with_name("specimen-v2.png")
SAMPLES = [
    "ff fi fl ft AV VA To Ty Wa rn il 1I",
    "Difficulty Guest Clackblade PLAYER NAME Normal",
    "0123456789 40 40 00 00 x1 5 +100",
    "i j f l t y g p q",
    "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
    "abcdefghijklmnopqrstuvwxyz",
    "~!@#$%^&*()_+<>?",
]


def main(output):
    output.mkdir(parents=True, exist_ok=True)
    manifest = json.loads((ART / "glyphs.json").read_text())
    glyphs = manifest["glyphs"]
    source = Image.open(SOURCE).convert("RGB")
    masks = {}
    for c, info in glyphs.items():
        x, y, w, h = info["source_rect"]
        crop = source.crop((x, y, x+w, y+h))
        mask = Image.new("L", (w, h))
        mask.putdata([255 if g >= 170 and g >= r+25 and g >= b+70 else 0
                      for r, g, b in crop.get_flattened_data()])
        masks[c] = mask
    assert glyphs["f"]["source_rect"] == [451, 322, 40, 71]
    assert glyphs["t"]["source_rect"] == [465, 396, 30, 75]
    assert sum(v > 0 for v in masks["f"].get_flattened_data()) == 1295
    assert sum(v > 0 for v in masks["t"].get_flattened_data()) == 859
    assert sum(masks["i"].getpixel((x, y)) > 0 for y in range(7)
               for x in range(masks["i"].width)) > 0
    font = TTFont(ART / "ClackbladeSpecimenPixel.ttf")
    cmap = font.getBestCmap()
    assert set(cmap) == {32} | {ord(c) for c in glyphs}
    assert font["glyf"][".notdef"].numberOfContours > 0
    metrics = manifest["metrics"]
    assert font["hhea"].ascent == font["OS/2"].sTypoAscender == metrics["ascent"]
    assert font["hhea"].descent == font["OS/2"].sTypoDescender == -metrics["descent"]
    assert font["hhea"].lineGap == font["OS/2"].sTypoLineGap == 0
    assert font["OS/2"].usWinAscent >= font["head"].yMax
    assert font["OS/2"].usWinDescent >= -font["head"].yMin
    for c, info in glyphs.items():
        assert font["hmtx"][cmap[ord(c)]] == (info["advance"]*16, info["bearing_left"]*16)
    sheet = Image.new("RGB", (2600, len(SAMPLES)*150), "#14111e")
    draw = ImageDraw.Draw(sheet)
    string_results = []
    for row, text in enumerate(SAMPLES):
        expected = Image.new("L", (2600, 140))
        actual_path = output / f"browser-string-{row}.png"
        if not actual_path.is_file():
            raise RuntimeError("Run verify-font-browser.cjs first (Pillow BASIC cannot shape kerning correctly).")
        actual = Image.open(actual_path).convert("L").point(lambda value: 255 if value >= 128 else 0)
        origin, baseline, pen = 12, 105, 12
        for index, c in enumerate(text):
            if c != " ":
                info = glyphs[c]
                expected.paste(masks[c], (round(pen+info["bearing_left"]), baseline-info["baseline"]))
                pen += info["advance"]
            else:
                pen += font["hmtx"]["space"][0]/16
            if index+1 < len(text):
                pen += manifest["kerning_source_pixels"].get(text[index:index+2], 0)
        expected.save(output / f"expected-string-{row}.png")
        assert pen + 12 < expected.width, "QA canvas clips the string"
        # Chromium's grayscale/LCD antialiasing can cover a few outside corners.
        # Require every source pixel and constrain extras to the immediate edge;
        # report their count instead of claiming browser pixel identity.
        assert not ImageChops.subtract(expected, actual).getbbox(), "Source pixels missing: " + text
        assert not ImageChops.subtract(actual, expected.filter(ImageFilter.MaxFilter(3))).getbbox(), text
        extras = sum(v > 0 for v in ImageChops.subtract(actual, expected).get_flattened_data())
        assert extras <= sum(v > 0 for v in expected.get_flattened_data()) * .001, text
        measurements = json.loads((output / "browser-metrics.json").read_text())
        assert abs(measurements[row]["advance"]-(pen-origin)) < .01, text
        draw.line((0, row*150+baseline, 2599, row*150+baseline), fill="#494056")
        sheet.paste("#f2e7cf", (0, row*150), actual)
        for index in range(len(text)+1):
            advance = measurements[row]["prefix_advances"][index]
            # Marks retain the origin/advance rather than tight-cropping each word.
            draw.line((origin+advance, row*150+110, origin+advance, row*150+116), fill="#a3844d")
        string_results.append({"text": text, "advance": pen-origin, "baseline": baseline,
                               "browser_antialias_corner_pixels": extras, "missing_source_pixels": 0})
    sheet.save(output / "positioned-strings.png")
    grid = {}
    for name, group in [("uppercase", "ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                        ("lowercase", "abcdefghijklmnopqrstuvwxyz"), ("digits", "0123456789")]:
        segments = Counter()
        for c in group:
            m = masks[c]
            for x in range(m.width+1):
                previous, run = 0, 0
                for y in range(m.height+1):
                    value = (bool(m.getpixel((x, y))) if y < m.height and x < m.width else 0) - (
                        bool(m.getpixel((x-1, y))) if y < m.height and x else 0)
                    if value != previous:
                        if previous and run >= 2:
                            segments[run] += 1
                        run = 0
                    previous, run = value, run+1
        error, pitch = min((sum(count*(length/pitch-round(length/pitch))**2
                               for length, count in segments.items())/sum(segments.values()), pitch)
                           for pitch in [i/100 for i in range(300, 601)])
        grid[name] = {"estimated_vertical_step_pixels": pitch, "normalized_squared_residual": error,
                      "edge_run_counts": dict(sorted(segments.items()))}
    fallback = TTFont(ROOT / "Assets/Art/UI/Pixel/VT323-Regular.ttf").getBestCmap()
    # PlayerIdentity accepts Unicode letters/digits, so its domain exceeds both fonts.
    coverage = {
        "primary": "".join(chr(c) for c in sorted(cmap)),
        "fallback_only": "".join(chr(c) for c in sorted(set(fallback)-set(cmap))
                                 if not unicodedata.category(chr(c)).startswith("C")),
        "known_fallback_samples": {c: ord(c) in fallback for c in ".,:;-/…×≥≤–—éĐđăâêôơưắệễộổ"},
        "unbounded_names": "PlayerIdentity accepts Unicode letters/digits; VT323 covers only its cmap. No validation change.",
    }
    report = {
        "font_sha256": hashlib.sha256((ART / "ClackbladeSpecimenPixel.ttf").read_bytes()).hexdigest(),
        "source_sha256": hashlib.sha256(SOURCE.read_bytes()).hexdigest(),
        "positioned_strings": string_results, "grid_analysis": grid, "coverage": coverage,
        "limits": [
            "Grid fits are estimates from a scaled screenshot; residuals do not support one exact common lattice.",
            "Outlines retain measured edges and ratios; this is not recovery of the original designer grid.",
            "Passing positioned strings at100px/em does not accept small-size Unity rendering.",
        ],
    }
    (output / "font-audit.json").write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print("PASS: 79 cmap entries, measured metrics, 7 positioned strings; grid/coverage report written.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True)
    main(parser.parse_args().output)
