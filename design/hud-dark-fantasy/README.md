# HUD dark fantasy — local candidate

Source branch: `micharel09/cobblestone-itch-release`, based on `56433a4`.
Cobblestone 56/48/42 typography, three typing cells and feedback timing are
preserved. User follow-ups shorten health/status rails102to82native pixels,
then scale the two complete side status groups to80% (about66px effective width).
Both HP digit labels and difficulty/DUEL line are hidden. Names, icons, stamina
and backplates scale together; score/combo and typing tray remain unchanged.
Corner anchors remain. No itch.io upload is authorized.

`tools/game_loop/export_combat_r7.py` generates the original shared art unchanged
and 26 isolated HUD variants under `Assets/Art/UI/CombatR7/DarkFantasy/`.
Menu consumers retain the original art. `CombatR7Build` owns HUD sprite selection,
palette, pause overlay ink and the two HP text material bindings. `ScoreSystem` changes only the
combo display tint; score math and decay behavior are unchanged.

`verify_palette.py` checks silhouettes, ink contrast and the HP material settings.
Ivory directly against blood red reaches only 3.837:1, so HP text uses a hard coal
underlay behind the original glyph face. The stock TMP shader uses zero offset
and softness; face and outline dilation remain zero. The material uniform stores
linear coal for this project's Linear color space.

The hard underlay passed actual native visual review and 11 focused checks in
30.014 seconds on Edge155/NVIDIA GTX1650Ti D3D11. Final production passed strict
repeat serialization, QA bridge exclusion, four combat/pause viewport captures
and resume at320/640 DPR1. Actual production HUD/pause and paired native/nearest
previews were inspected. See `focused-qa.json`, `production-smoke.json`,
`scope-audit.json` and `delivery.json`. No full suite or final DPR matrix claim.

At the user's request, runtime checks were limited to changed HUD ink,
HP face preservation, combo tint and a focused production startup/pause check.
No full gameplay/tutorial suite, difficulty matrix, stress or soak is required.

`build_preview.py` pairs real baseline/candidate screenshots at native 320×180
and 640×360, plus nearest-neighbor enlargement. It records hashes in
`preview-receipt.json`; independently played actor frames may differ.

The latest compact follow-up is in `compact.html`; `build_compact_preview.py`
pairs actual production images before/after at320/640 plus nearest3x.
`compact-smoke.json` records the focused production combat/damage check.
Latest scale follow-up uses `scaled-smoke.json`: six combat/damage samples at
320/640/400x300. `score-preservation.json` proves pixel-identical score regions
at320/640. Stamina highlight scales with its target; no tutorial suite rerun.
