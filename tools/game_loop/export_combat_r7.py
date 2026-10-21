"""Export original shared HUD art, isolated dark fantasy combat variants, and dungeon layers."""
import argparse
import importlib.util
import io
from pathlib import Path
import sys
from PIL import Image

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[2]
REVISIONS = ROOT / "design/ui-kit/typinghero/v2/revisions"


# Combat-only material mapping; source silhouettes, alpha and dungeon layers stay intact.
PALETTE = {
    "090811": "09080a", "14111e": "121014", "211c2d": "121014",
    "282231": "121014", "494153": "302b2b", "5e536a": "a17b4f",
    "827789": "d1ad73", "b5acb8": "d1ad73", "77677f": "302b2b",
    "a3844d": "a17b4f", "dcc18a": "e5be72", "afa1b3": "c5b9a7",
    "588b8c": "73b6aa", "f17c81": "ed8080", "d45165": "c34b55",
    "b43852": "c34b55", "74243e": "c34b55", "542039": "c34b55",
}


def dark_fantasy(image, name):
    result = image.copy()
    mapping = {tuple(bytes.fromhex(a)): tuple(bytes.fromhex(b)) for a, b in PALETTE.items()}
    result.putdata([(*mapping.get(pixel[:3], pixel[:3]), pixel[3]) for pixel in image.getdata()])
    # Resources must remain legible across their whole height, including the bottom row.
    if name in ("resource-track", "secondary-track"):
        result.putdata([(*bytes.fromhex("121014"), pixel[3]) for pixel in image.getdata()])
    if name in ("stamina-fill", "combo-fill", "combo-gold-fill"):
        color = tuple(bytes.fromhex("73b6aa" if name == "stamina-fill" else "e5be72"))
        result.putdata([(*color, pixel[3]) for pixel in image.getdata()])
    assert result.size == image.size and result.getchannel("A").tobytes() == image.getchannel("A").tobytes()
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    sys.path.insert(0, str(REVISIONS / "hud-r4/source"))
    spec = importlib.util.spec_from_file_location("r6", REVISIONS / "hud-r6/source/build.py")
    r6 = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(r6)
    names = ["resource-frame", "resource-socket", "elite-socket", "boss-socket",
             "resource-track", "hp-fill", "icon-hp", "elite-badge", "captain-badge",
             "secondary-frame", "secondary-socket", "secondary-frame-gold",
             "secondary-socket-gold", "secondary-track", "small-icon-combo",
             "small-icon-stamina", "combo-fill", "combo-gold-fill", "stamina-fill",
             "score-frame", "keycap-space", "icon-pause"]
    images = {name: r6.material(name) for name in names}
    images["typing-frame"] = r6.frame()
    out = ROOT / "Assets/Art/UI/CombatR7"
    out.mkdir(parents=True, exist_ok=True)
    payloads = {}
    for name, image in images.items():
        data = io.BytesIO()
        image.save(data, format="PNG")
        payloads[name] = data.getvalue()
    dark_out = out / "DarkFantasy"
    dark_out.mkdir(exist_ok=True)
    dark_images = dict(images)
    for name in ["icon-shield", "icon-warning"]:
        dark_images[name] = Image.open(out / (name + ".png")).convert("RGBA")
    dark_images["modal-normal"] = Image.open(ROOT / "Assets/Art/UI/DungeonV2/modal-normal.png").convert("RGBA")
    for name, image in dark_images.items():
        data = io.BytesIO()
        dark_fantasy(image, name).save(data, format="PNG")
        target = dark_out / (name + ".png")
        if args.check:
            assert target.read_bytes() == data.getvalue(), target
        elif not target.exists() or target.read_bytes() != data.getvalue():
            target.write_bytes(data.getvalue())
    for layer in ["far", "mid", "floor", "foreground"]:
        name = "scene-" + layer
        payloads[name] = (REVISIONS / "hud-r4/artwork" / (name + ".png")).read_bytes()
    for name, data in payloads.items():
        path = out / (name + ".png")
        if args.check:
            assert path.read_bytes() == data, path
        elif not path.exists() or path.read_bytes() != data:
            path.write_bytes(data)
    print(f"PASS: {len(payloads)} original component/layer PNGs; {len(dark_images)} isolated HUD variants")


if __name__ == "__main__":
    main()
