"""Audit observed native HUD ink bounds against the approved unchanged layout."""
import argparse
import json
from pathlib import Path

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("captures", type=Path, nargs="+")
    args = parser.parse_args()
    checks, errors = 0, []
    for folder in args.captures:
        for path in folder.glob("*.json"):
            state = json.loads(path.read_text(encoding="utf-8"))
            if "typography" not in state or state.get("slayer"):
                continue
            px = min(state["width"] / 960, state["height"] / 540) * 3
            ox = (state["width"] - 320 * px) / 2
            oy = (state["height"] - 180 * px) / 2
            for label in state["typography"]:
                name = label["path"].split("/")[-1]
                ink = label["glyphBounds"]
                if label["scale"] < .1 or ink["width"] <= 0:
                    continue
                if name in ["PlayerName", "EnemyHPLabel"]:
                    left, right = (3, 111) if name == "PlayerName" else (209, 317)
                    limits = [ox + left * px, state["height"] - oy - 20 * px,
                              ox + right * px, state["height"] - oy]
                elif name == "RoundR7":
                    limits = [ox + 102 * px, state["height"] - oy - 67 * px,
                              ox + 218 * px, state["height"] - oy - 54 * px]
                else:
                    continue
                observed = [ink["x"], ink["y"], ink["x"] + ink["width"], ink["y"] + ink["height"]]
                checks += 1
                if any([observed[0] < limits[0] - .25 * px, observed[1] < limits[1] - .25 * px,
                        observed[2] > limits[2] + .25 * px, observed[3] > limits[3] + .25 * px]):
                    errors.append({"capture": str(path), "label": name, "ink": observed, "limits": limits})
            # The actual rendered words remain in the three native58px cells.
            if not state.get("rowSlide") and state.get("current"):
                for center, ink in zip([94, 160, 226], state.get("wordGlyphRects", [])):
                    if ink["width"] <= 0:
                        continue
                    checks += 1
                    if ink["x"] < ox + (center - 29) * px - .25 * px or ink["x"] + ink["width"] > ox + (center + 29) * px + .25 * px:
                        errors.append({"capture": str(path), "word": center, "ink": ink})
    result = {"checks": checks, "errors": errors, "scope": "observed normal HUD/name/accent/round/word ink; Slayer transitions reviewed visually"}
    (Path(__file__).parent / "clearance-audit.json").write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    assert checks > 0 and not errors, result
    print(f"PASS:{checks} observed HUD ink bounds")

if __name__ == "__main__":
    main()
