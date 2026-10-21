"""Validate production HUD material isolation and contrast, without Unity."""
from pathlib import Path
import json
import re
from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
ART = ROOT / "Assets/Art/UI/CombatR7"

def luminance(color):
    channels = [c / 255 for c in bytes.fromhex(color)]
    linear = [c / 12.92 if c <= .04045 else ((c + .055) / 1.055) ** 2.4 for c in channels]
    return sum(c * weight for c, weight in zip(linear, [.2126, .7152, .0722]))

def main():
    variants = sorted((ART / "DarkFantasy").glob("*.png"))
    assert len(variants) == 26
    for path in variants:
        original = ROOT / "Assets/Art/UI/DungeonV2" if path.stem == "modal-normal" else ART
        old, new = Image.open(original / path.name).convert("RGBA"), Image.open(path).convert("RGBA")
        assert old.size == new.size, path
        assert old.getchannel("A").tobytes() == new.getchannel("A").tobytes(), path
    contrast = {}
    for color in ["f2e7cf", "c5b9a7", "a99c90", "73b6aa", "e5be72", "f16b67", "c34b55", "ed8080"]:
        ratio = (luminance(color) + .05) / (luminance("121014") + .05)
        assert ratio >= (3 if color in ["c34b55", "ed8080"] else 4.5), (color, ratio)
        contrast[color] = round(ratio, 3)
    # Every opaque resource pixel must clear its actual coal track.
    for name in ["hp-fill", "stamina-fill", "combo-fill", "combo-gold-fill"]:
        image = Image.open(ART / "DarkFantasy" / (name + ".png")).convert("RGBA")
        for pixel in image.getdata():
            if pixel[3]:
                assert pixel[3] == 255
                ink = bytes(pixel[:3]).hex()
                assert (luminance(ink) + .05) / (luminance("121014") + .05) >= 3
    material = (ART / "DarkFantasy/HpText.mat").read_text(encoding="utf-8")
    scalar = lambda name: float(re.search(r"- " + name + r": ([0-9.eE+-]+)", material)[1])
    assert scalar("_OutlineWidth") == 0 and scalar("_FaceDilate") == 0
    assert scalar("_UnderlayDilate") == 1 and scalar("_UnderlaySoftness") == 0
    assert scalar("_UnderlayOffsetX") == scalar("_UnderlayOffsetY") == 0
    assert "UNDERLAY_ON" in material and "- OUTLINE_ON" not in material
    outline = re.search(r"_UnderlayColor: \{r: ([^,]+), g: ([^,]+), b: ([^,]+), a: ([^}]+)\}", material)
    linear_to_srgb = lambda c: c * 12.92 if c <= .0031308 else 1.055 * c ** (1 / 2.4) - .055
    rgb = [round(linear_to_srgb(float(v)) * 255) for v in outline.groups()[:3]]
    assert rgb == [18, 16, 20] and float(outline[4]) == 1
    pixels = scalar("_UnderlayDilate") * scalar("_GradientScale") * scalar("_ScaleRatioC") * .5 * 16 / 64
    assert .7 <= pixels <= 1
    result = {"background": "121014", "ratios": contrast,
              "resourceText": {"outline": "121014", "outlineDesignPixels": round(pixels, 3),
                               "ivoryAgainstRedWithoutOutline": round((luminance("f2e7cf") + .05) / (luminance("c34b55") + .05), 3),
                               "ivoryAgainstOutline": contrast["f2e7cf"],
                               "damageRedAgainstOutline": contrast["f16b67"]}}
    (Path(__file__).parent / "contrast.json").write_text(json.dumps(result, indent=2) + "\n")
    print("PASS:26 unchanged silhouettes; text inks>=4.5:1 with thin outer HP stroke; every resource pixel>=3:1")

if __name__ == "__main__":
    main()
