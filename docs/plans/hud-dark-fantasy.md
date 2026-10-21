# Compact dark fantasy HUD - verified local handoff

## Intent and scope

The later compact-HUD user request explicitly supersedes the original unchanged
rail geometry/encounter-label requirement; see verified follow-up below.

Source branch `micharel09/cobblestone-itch-release`, base `56433a4` (approved
Cobblestone typography). Keep layout/control sizes, three typing cells, all
bindings/GUIDs, Cobblestone56/48/42 and Blackbit title-only. Preserve hidden
PARRY/Space/clock and feedback timing. No menu, gameplay, actors, dungeon, camera,
provider, dependency or public API changes. No itch.io upload.

Approved material: coal121014, iron302B2B, antique bronzeA17B4F/D1AD73,
ivoryF2E7CF, secondaryC5B9A7/queuedA99C90, HP C34B55/ED8080,
stamina/typing73B6AA, combo/parryE5BE72, error/damageF16B67.

The user narrowed acceptance to testing changed parts only. No final full
suite/tutorial/difficulty/DPR matrix/stress/soak run is required or claimed.

## Final implementation

- `export_combat_r7.py` generates26 isolated PNG variants, preserving every
  dimension/alpha silhouette. Original shared textures remain unchanged for menus.
- `CombatR7Build` selects combat variants, applies typing/resource/score ink and
  opaque coal backplates. Pause-only PixelOverlay gets the coal modal and two
  generated bronze rim subrect sprites; controls and geometry are preserved.
- `ScoreRules` changes only three tint fields. `ScoreSystem` changes only the
  ComboHUD tint argument so its first visible multiplier is gold. Scoring math,
  thresholds, decay, events, input and feedback durations are unchanged.
- HP numbers use a hard, zero-offset TMP UNDERLAY_ON behind their unchanged face.
  UnderlayDilate1, Softness0, FaceDilate/OutlineWidth0; material coal is stored
  linear for this Linear project. Approximate native outer radius0.8125px.
- `PixelUIBuild` preserves its complete typing/font/material guard and includes
  new assets/ScoreRules in strict repeat comparison. Generated final scene/art
  and canonical HP material were retrieved from the pinned Unity build.

## Verification and evidence

- Pinned Unity2022.3.25f1 on fly-server. QA062108 input72528e35 and final
  production064056 inputeb36b3b1; full hashes in design/hud-dark-fantasy receipts.
- Focused native real-input HUD11checks PASS in30.014s, Edge155/NVIDIA GTX1650Ti
  ANGLE D3D11. Normal, combo, popup, low HP and pause/resume at320/640 DPR1.
  Actual native and enlarged HP/combo/low-HP images inspected; original white
  glyph face retained. Zero exceptions/provider writes.
- QA predates the final coal-track flatten and pause migration. Its evidence is
  scoped to unchanged final HP material/combo ink. Final production includes both
  additions: strict two-pass serialization PASS; original-HTML combat/pause4
  viewport captures plus resume PASS, zero JS/shader errors, QA bridge undefined.
  All5JS/WASM/data payloads exclude ASCII/UTF16 QA markers, qa=false.
- Final production combat/pause screenshots inspected. `verify_scope.py` proves
  all874scene IDs and the complete normalized scene unchanged except palette,
  one-for-one sprite refs and two HP material bindings. Menu unchanged; font
  assets and original shared PNGs unchanged. No clipping/layout/API changes.
- `verify_palette.py`:26 unchanged silhouettes; text ratios against coal main
  15.412, secondary9.795, queued7.068, teal8.109, gold10.757, error6.342.
  Every opaque resource pixel>=3:1. Ivory directly on red is3.837:1, resolved by
  the coal HP underlay. HP red4.016/highlight7.208 against coal.
-8paired real previews at320/640 plus4nearest enlargements reviewed. Baseline
  accepted94c4adbd; QA candidate72528e35; default clean productioneb36b3b1.
  Independent runs share the dungeon but actor frames/words can differ. QA debug
  Spark label is pre-existing; production does not show it. Receipt explicitly
  scopes QA captures; final pause has a separate clean production image.
- Prior unchanged typography/layout/gameplay evidence is retained in existing
  memory. Final material review is DPR1 only; no full matrix/performance claim.

## Rejected candidates and environment limits

Earlier OUTLINE_ON/.5 consumed thin HP faces and used a gamma uniform, so its
images are rejected. The hard underlay replacement passed actual game review.
Legacy software harness expected old y11 status geometry, unlike approved y20;
no pass claimed for that harness. Shared Bee cache referenced another task's
removed LightingLab sources, then an old GameAssembly archive caused duplicate
symbols. A temporary wrapper invalidated only generated graph/archive files under
the existing global lock; final QA and production builds passed. Other jobs were
preserved. Superseded task-owned queued builds were stopped; none remain active.

## Local handoff and resources

- Game: http://127.0.0.1:8245/ ; preview: http://127.0.0.1:8245/review/compact.html
- Root: `Builds/hud-dark-fantasy` in this source worktree.
- ZIP: `Builds/clackblade-hud-dark-fantasy-local.zip`,30members/16,983,712bytes,
  CRC PASS, SHA2567adb8b5931622875938a2bac16fc035582cfbef023f1cbd0bc506f8484fb6c81.
- Local hidden Node server PID22600, helper/logs under
  `E:/CodexTemp/TypingHero/hud-dark-fantasy`. Kept for user review; stop PID22600
  when review ends, then remove only this task's temporary folder if no longer
  needed. Native test contexts/servers closed. Main dirty worktree untouched.
- Immutable remote receipts/players remain at
  `/data/typinghero-game-loop/results/20261021T062108-webgl` and
  `/data/typinghero-game-loop/results/20261021T064056-production` for reproducibility.
- Source artifact receipts: `design/hud-dark-fantasy/`. No push, PR or upload.

## User follow-up - compact health HUD (verified)

- Remove enemy HP digits and difficulty/DUEL line, retain player HP digits.
- Shorten status/resource rails102to82native pixels (~20%), retaining original
  height, font size, socket/icon art and outer corner anchors. Nine-slice HP and
  stamina frames preserve their border pixels. Right-aligned enemy name stays put.
- Implemented in final builder pass; hide EnemyHpR7/RoundR7/RoundReadabilityR7
  GameObjects so existing runtime bindings cannot reactivate their presentation.
  No runtime gameplay changes. Frame HP importer gains horizontal21/3borders.
- Production070959 /77f90925 strict repeat builder and bridge exclusion PASS.
  Original-HTML combat/damage4captures at320/640 DPR1 PASS with zero JS/shader
  errors/external requests. Actual native and nearest before/after inspected.
  Player HP35/40to20/40 and fill work; enemy digits/DUEL stay hidden.
  No full suite or unrelated pause/tutorial flow test.
- Local player, ZIP, receipts and compact preview updated to verified production.
  Same local game URL; refresh to load the new player.
- Final canonical scene proves all874IDs and every non-authorized field unchanged:
  only the explicit compact RectTransform/sliced-image/visibility fields differ
  beyond the previously approved palette. MainMenu/font/typing bindings unchanged.
- Immutable compact build: `/data/typinghero-game-loop/results/20261021T070959-production`.

## User follow-up - uniform side HUD scale (verified)

- Hide player HP digits as well as enemy digits; keep encounter line hidden.
- Scale only PlayerStatus/EnemyStatus (names, HP, sockets/icons and enemy stamina)
  to80% of the current compact layout. Reposition top-left/top-right anchors so
  the overall top row stays aligned and the three-pixel corner inset is preserved.
  Readability backplates scale with their respective names.
- ScoreCluster/score/multiplier/decay and typing tray stay unchanged; no runtime
  gameplay code edits. Final production075801 /0f97006d visually checked and copied into the served root.
- Superseded queued96466 stopped before build to include the coupled stamina
  highlight scale. Final075801 /0f97006d strict repeat builder and QA exclusion
  PASS. Tool99711 completed; no task build job remains active.
  Actual six320/640/400x300 combat/damage captures PASS with zero JS/shader
  errors/external requests. All three sizes inspected; damage fill works and
  both HP labels remain absent. Score region pixel-identical to77f90925 at320/640.
  Highlight scale follows the stamina target without tutorial runtime edits.
  Complete scene audit retains874IDs and all non-authorized fields, including
  the entire score/combo tree and typing tray. Before/after nearest preview reviewed.
  Local player, ZIP, delivery and preview updated. Immutable player preserved at
  `/data/typinghero-game-loop/results/20261021T075801-production`.
  No full suite/tutorial/difficulty matrix.
