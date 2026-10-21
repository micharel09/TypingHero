"""Compare final generated scene to the approved layout, allowing only HUD ink."""
from pathlib import Path
import json
import re
import subprocess

ROOT = Path(__file__).resolve().parents[2]
ART = ROOT / "Assets/Art/UI/CombatR7/DarkFantasy"
BASE_REF = "56433a4"
HUD_REF = "8179a0c"


def guid(path):
    return re.search(r"(?m)^guid: (\w+)", path.read_text())[1]


def restore_compact_fields(text):
    # Explicit authorized follow-up: 102->82px rails, corner/name alignment,
    # nine-slice frames, and three hidden presentation nodes. All other fields
    # still undergo the complete scene comparison below.
    changes = {
        "251735559": [("m_AnchoredPosition: {x: -9, y: -48}", "m_AnchoredPosition: {x: 645, y: -60}"), ("m_SizeDelta: {x: 246, y: 108}", "m_SizeDelta: {x: 306, y: 108}")],
        "494717920": [("m_SizeDelta: {x: 183, y: 42}", "m_SizeDelta: {x: 243, y: 42}")],
        "1077804929": [("m_AnchoredPosition: {x: -72, y: 60}", "m_AnchoredPosition: {x: -12, y: 60}")],
        "965796967": [("m_SizeDelta: {x: 246, y: 39}", "m_SizeDelta: {x: 306, y: 39}")],
        "1795675206": [("m_SizeDelta: {x: 192, y: 12}", "m_SizeDelta: {x: 252, y: 12}")],
    }
    for identity in ["939622850", "1631290904", "2053201633"]:
        changes[identity] = [("m_SizeDelta: {x: 246, y: 63}", "m_SizeDelta: {x: 306, y: 63}")]
    for identity in ["1875854537", "2091849781"]:
        changes[identity] = [("m_SizeDelta: {x: 171, y: 18}", "m_SizeDelta: {x: 231, y: 18}")]
    for identity in ["494717919", "607546061", "1032934641", "1393682040"]:
        changes[identity] = [("m_IsActive: 0", "m_IsActive: 1")]
    scale = ("m_LocalScale: {x: 0.8, y: 0.8, z: 1}", "m_LocalScale: {x: 1, y: 1, z: 1}")
    changes["1631290904"].extend([scale, ("m_AnchoredPosition: {x: 9, y: -48}", "m_AnchoredPosition: {x: 9, y: -60}")])
    changes["251735559"].append(scale)
    changes["1082511720"] = [scale]
    changes["1085631724"] = [scale]
    changes["175680816"] = [scale, ("m_AnchoredPosition: {x: -9, y: 0}", "m_AnchoredPosition: {x: 627, y: -0}")]
    for identity in ["251735559", "175680816"]:
        for field in ["m_AnchorMin", "m_AnchorMax", "m_Pivot"]:
            changes[identity].append((field + ": {x: 1, y: 1}", field + ": {x: 0, y: 1}"))
    for identity in ["939622851", "965796968", "2053201634"]:
        changes[identity] = [("m_Type: 1", "m_Type: 0"), ("m_PixelsPerUnitMultiplier: 0.33333334", "m_PixelsPerUnitMultiplier: 1")]
    blocks = re.split(r"(?m)(?=^--- !u!)", text)
    seen = set()
    for index, block in enumerate(blocks[1:], 1):
        identity = re.search(r"&(-?\d+)", block)[1]
        if identity in changes:
            for new, old in changes[identity]:
                assert block.count(new) == 1, (identity, new)
                block = block.replace(new, old)
            seen.add(identity)
        if identity in ["1110096436", "1519305575", "1903749367"]:
            new, old = ("192", "252") if identity == "1110096436" else ("171", "231")
            block, count = re.subn(r"(propertyPath: m_SizeDelta.x\n\s+value:) " + new + r"\n", r"\g<1> " + old + "\n", block)
            assert count == 1, identity
        blocks[index] = block
    assert seen == set(changes)
    return "".join(blocks)


def main():
    original = subprocess.check_output(
        ["git", "show", BASE_REF + ":Assets/Scenes/SampleScene.unity"], cwd=ROOT).decode()
    candidate = subprocess.check_output(
        ["git", "show", HUD_REF + ":Assets/Scenes/SampleScene.unity"], cwd=ROOT).decode()
    ids = lambda s: re.findall(r"(?m)^--- !u!\d+ &(-?\d+)", s)
    assert ids(original) == ids(candidate), "Scene object identities changed"
    mapping = {}
    for meta in ART.glob("*.meta"):
        if meta.stem == "HpText.mat":
            continue
        folder = "DungeonV2" if meta.stem in (
            "modal-normal.png", "secondary-frame.asset", "secondary-frame-gold.asset") else "CombatR7"
        source = ROOT / "Assets/Art/UI" / folder / meta.name
        assert source.exists(), meta
        mapping[guid(meta)] = guid(source)
    normalized = restore_compact_fields(candidate)
    for new, old in mapping.items():
        normalized = normalized.replace(new, old)
    material = guid(ART / "HpText.mat.meta")
    normalized, count = re.subn(
        r"\{fileID: 2100000, guid: " + material + r", type: 2\}",
        "{fileID: 6703591609346202795, guid: 95ad28fb625f1a3ea9d5886cb268d465, type: 2}", normalized)
    assert count == 2, "Unexpected HP material consumers"

    def omit_ink(text):
        text = text.replace("\r\n", "\n")
        text = re.sub(r"\{r: [^}]+, g: [^}]+, b: [^}]+, a: [^}]+\}", "{COLOR}", text)
        text = re.sub(r"(?m)^(\s+rgba:) \d+", r"\1 COLOR", text)
        text = re.sub(
            r"(propertyPath: [\w.]*?(?:Color|color|red|gold|success)\.[rgba]\n\s+value:) [^\n]*",
            r"\1 COLOR", text)
        return text

    before, after = omit_ink(original), omit_ink(normalized)
    if before != after:
        import difflib
        print("".join(difflib.unified_diff(before.splitlines(True), after.splitlines(True)))[:6000])
        raise AssertionError("Unexpected non-palette scene changes")
    menu = subprocess.check_output(["git", "show", HUD_REF + ":Assets/Scenes/MainMenu.unity"], cwd=ROOT)
    assert menu.replace(b"\r\n", b"\n") == subprocess.check_output(
        ["git", "show", BASE_REF + ":Assets/Scenes/MainMenu.unity"], cwd=ROOT).replace(b"\r\n", b"\n")
    receipt = {"baseRef": BASE_REF, "hudRef": HUD_REF, "scope": "approved HUD before main arena integration",
               "sceneIDs": len(ids(original)), "menuUnchanged": True,
               "completeSceneOnlyPaletteSpritesHpMaterialAndExplicitCompactFields": True,
               "healthBarLogicalWidth": 82, "sideHudScale": 0.8,
               "healthBarNativeWidthAfter": 65.6,
               "bothHpDigitsAndEncounterLineHidden": True, "scoreClusterUnchanged": True}
    (Path(__file__).parent / "scope-audit.json").write_text(json.dumps(receipt, indent=2) + "\n")
    print("PASS:", receipt)


if __name__ == "__main__":
    main()
