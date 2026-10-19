"""Export only approved r6 components and r4 dungeon layers for the r7 Unity HUD."""
import argparse
import importlib.util
import io
from pathlib import Path
import sys

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[2]
REVISIONS = ROOT / "design/ui-kit/typinghero/v2/revisions"


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
    for layer in ["far", "mid", "floor", "foreground"]:
        name = "scene-" + layer
        payloads[name] = (REVISIONS / "hud-r4/artwork" / (name + ".png")).read_bytes()
    for name, data in payloads.items():
        path = out / (name + ".png")
        if args.check:
            assert path.read_bytes() == data, path
        elif not path.exists() or path.read_bytes() != data:
            path.write_bytes(data)
    print(f"PASS: {len(payloads)} approved component/layer PNGs; no composed review screen")


if __name__ == "__main__":
    main()
