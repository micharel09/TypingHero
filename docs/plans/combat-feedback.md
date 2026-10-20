# Combat feedback â€” implementation checkpoint

Current task: BCompact continuous 3D pixel sparks in child `feat/pixel-hit-sparks`.
Implementation, Quick78, repeated builders, two-direction/two-size native art and
lifecycle checks pass. Matched-configuration performance verification is complete;
see the final volume receipt for measured limits and retained review resources. Current/default and the earlier
published prototype remain unchanged. Keep review players/clips; integration and
publication are separate from this request.
Earlier intent/checkpoints are historical; preserve their source/license/release
receipts and use the latest approved BCompact volume requirements for this task.

## Intent and acceptance

Implement the approved pixel combat feedback plan, preserving character animations,
Dark Ages UI, arena, public APIs, Firebase schema, dependencies and combat rules
(player 40 HP / skeleton 850 HP, damage/scoring/typing/parry/statistics unchanged).
Deliver source and a new production WebGL ZIP; preserve the previous Dark Ages ZIP.
No stress/soak and no itch.io publish.

## Approved implementation

- Author assets: free `Hit Effect 01.rar` from https://pimen.itch.io/battle-vfx-hit-spark
  and `Effects.rar` from https://pimen.itch.io/pixel-battle-effects. Obtain original
  archives and inspect frames/palettes/license before importing; never use promo GIFs.
  Import under `Assets/Art/VFX/Pimen/`, retain provenance/license, Point filtering,
  no mipmap/compression on WebGL, consistent pivots and uniform scaling.
- Normal: white/orange spark 150 ms, about 24 px at 960x540; ivory flash 60 ms
  and at least two rendered frames; retain existing 50 ms hitstop, no shake.
- Parry: blue spark 180 ms / 32 px at sword intersection; retain clang and
  100 ms camera impulse at power .01; no new hitstop.
- Slayer: rotate four white hit effects, 100 ms / 18 px; retain existing slash
  animation and IgnoreHitstop; no shake per character.
- Lethal: white/orange burst 180 ms / 40 px; current death animation;
  camera impulse 100 ms at .015, no new stop.
- PlayerWeaponHitbox remains damage authority; one feedback per target/attack,
  emitted after damage. Capture Slayer before damage and death after damage.
  No feedback for misses, invalid layers, dead targets, closed windows or locked match.
  Use contact point, remove (1.2,.7) offset, vertical jitter +/- .03 world units,
  no attacker parenting. Preserve heavy-swing reaction and collider/Rigidbody positions.
- Afterimages: three per burst, 25 ms interval; life .12 + index*.02 seconds;
  initial alpha .4 with .6 falloff. Preserve sprite/combo tint. Pause schedule/fade;
  exit Slayer, death, disable and scene change cancel pending bursts and reclaim images.
- Kenney Impact Sounds CC0: impactWood_medium_000â€“002 for normal/Slayer,
  impactWood_heavy_000â€“002 for lethal. Separate SfxEvents with .65/.45/.8 volume,
  small pitch jitter; existing pool/mixer/preferences, independent of whoosh/parry.
  Existing parry clang pitch narrowed to +/- 1 semitone.
- Extend current pools; VFX real time during hitstop, frozen on pause; reset Animator,
  color/scale/lease, exactly-once return. Actual clips release via animation event.
  Unity builder applies feedback after core-match and before Dark Ages; two runs
  must preserve components/listeners/references/GUIDs.
- Credits: Pimen Battle Effects CC BY 4.0, identify timing/scale edits; Hit Spark
  author license permits game use/modification but forbids standalone redistribution.
  Retain source privately with project rather than publishing standalone asset packs.

## Milestones and checks

A. Obtain and inspect both archives; baseline checkout, scene/prefab/config; doctor.
B. Complete normal hit path; runtime hit/miss/multiple-collider and normal/slow-motion
   visual check in actual arena before extending profiles.
C. Add parry/Slayer/lethal, audio and lifecycle; meaningful PlayMode checks for
   profile selection including lethal Slayer exit, pause/cancel afterimages, actual
   VFX animation-event reuse, restart/scene cleanup, volume/mute/pause and separate clang.
   Run `python tools/game_loop/run.py quick`, repair failures.
D. Run `python tools/game_loop/run.py webgl`; real keyboard/mouse, read-only bridge,
   Firebase fixtures and blocked real writes. Windows browser QA at 960x540,
   1280x720, 1920x1080, 640x360 plus iframe/fullscreen. Exercise hits/misses,
   parry success/failure, rapid Slayer, stamina break, win/loss, pause during
   impact/burst, resume/restart/menu and long words/type-ahead/errors/timeout.
   Confirm readable typing/gold cue, crisp pixels/contact placement/timing. Listen
   to isolated and rapid impacts with music; explicitly mark audio limits.
   Server SwiftShader is functional evidence only, not player-machine smoothness.
E. Once acceptance and checks pass, run production; new ZIP with credits/licenses,
   QA bridge exclusion, CRC and SHA-256. Update affected docs/game memory only with
   verified changes. Review diff and preserve pre-existing changes.

## 2026-10-17 checkpoint â€” A blocked on original assets

- Repository root confirmed; Unity 2022.3.25f1. Existing checkout has substantial
  staged/unstaged/untracked Dark Ages, core-match, test and automation work.
  No existing source, prefab or scene was modified by this task.
- Baseline tracked diff retained at
  `E:/CodexTemp/TypingHero/combat-feedback/baseline.patch` for continuation.
  Existing untracked source remains in place; baseline is not a complete backup.
- `python tools/game_loop/run.py doctor`: PASS; license present, server free 3.3 GiB,
  pinned Unity image resolved. No quick/WebGL/production run: implementation pending.
- Both author pages read and license/download names reconfirmed. Direct local HTTP
  download-page access returns 403 Cloudflare `Just a moment...` for both packs.
  No original RAR found in the user Downloads folder.
- Browser Skill 0.3.2 foreground daemon launched as managed exec session 32357;
  separate status confirms IPC healthy. `bsk doctor --json` reports FAIL:
  extension connected = 0 browsers. Cannot use authenticated browser download.
  No browser session was obtained; no browser settings were changed.
- Existing source confirms hitbox currently reads Slayer again after damage,
  HitSparkSpawner parents to attacker by default, and afterimage schedule currently
  uses WaitForSecondsRealtime without pause cancellation. These are investigation
  findings only; no behavior changes are claimed.
- Required next input: provide the two original free RAR files in Downloads/project
  input location, or connect Browser Skill extension to allow author-site downloads.
  Then inspect real sheets/license and resume milestone A. Do not substitute previews
  or report acceptance/production ZIP complete while these inputs are missing.

## 2026-10-17 continuation ? runtime implemented, assets still blocked

The user requested completing all authorized work and checking the source approach.
Continued independent runtime/configuration work while original-sheet import remains
blocked. Do not treat this as acceptance for the missing Pimen profiles.

- Retrieved original Kenney Impact Sounds ZIP from the author's download link;
  imported exactly the six approved OGG clips and original CC0 license/provenance.
- Author pages and Unity 2022.3 documentation were checked: use existing ObjectPool
  ownership and Animator.UnscaledTime with explicit pause hold. No additional system
  or dependency was introduced.
- Hitbox now snapshots Slayer/IgnoreHitstop before damage, picks feedback after
  damage, skips dead-target Slayer hit pose and rejects locked matches. Profiles
  route to normal / rotating Slayer / lethal plus independent pooled SFX.
- Existing spark/parry pools have independent scene-owned roots, cleanup on disable
  and destroy, and exactly-once animation-event release. Reuse resets Animator,
  renderer color/flips, scale and speed. Animation advances through hitstop, holds
  during pause. Contact placement uses zero offset and +/- .03 vertical jitter.
- Afterimages now default to approved count/timing/alpha/lifetime; pending schedule
  and fades hold on pause. Slayer exit, signal exit, match death and disable cancel
  bursts and reclaim active images. Independent pool root avoids Unity rejecting
  reparenting while the owner is deactivating.
- CombatFeedbackBuild applies audio, flash, afterimage/placement and impulse settings
  after core-match and before Dark Ages, through Unity serialization. Repeat-builder
  checks now include character prefabs and impact/parry SfxEvent assets.
  Pimen prefab generation/bindings are NOT implemented until sheets are obtained.
- New CombatFeedbackTests exercise hit dedup/invalid targets, captured lethal Slayer
  profile/IgnoreHitstop, four-profile rotation and pool reset, pause/exit/death/disable,
  actual existing VFX animation-event return, ivory flash, gain/mute/audio pause and
  independent voices. These tests do not prove missing Pimen prefab appearance.
- Quick attempt 1 found real reparent-on-disable lifecycle error, repaired using the
  independent afterimage pool root. Attempt 2 caught an editor assembly reference
  issue; used existing SerializedObject API rather than adding a Cinemachine reference.
  Attempt 3 identified test timing depending on headless audio playback; gain checks
  now invoke the real LateUpdate synchronously and pause before yielding. This is
  volume/pool-state evidence, not a listening test.
- Quick PASS: 33/33, snapshot
  `7577eca571c9e5d133b6b40b14844846faa48325d82c4b5108fcaabfd067b7e9`.
  Subsequently added an empty-collection fast path to afterimage cancellation;
  final checks must use the resulting snapshot.
- Pimen author tab opened in Orca, Cloudflare checkbox completed through native UI.
  Both purchase pages render, but native Download/No-thanks controls do not navigate
  or issue a download request. Initial apparent escaping was JSON encoding, not malformed site JavaScript;
  direct character-code inspection disproved that hypothesis. The purchase handler
  is bound and native focus reaches the download link, but no download request is
  observed after native click/Enter. Root cause remains unknown. Do not modify the
  external site or use promotional images as replacement.
  Local and fly-server HTTP still return 403. No browser cookies/tokens extracted.
  Awaiting original `Hit Effect 01.rar` / `Effects.rar` (or a working author download).
- WebGL regression/build repeat verification started; results pending. No production
  ZIP or completed Pimen visual/audio acceptance claimed. Existing Dark Ages ZIP kept.

References checked:
- https://pimen.itch.io/battle-vfx-hit-spark
- https://pimen.itch.io/pixel-battle-effects
- https://kenney.nl/assets/impact-sounds
- https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AnimatorUpdateMode.UnscaledTime.html
- https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Pool.ObjectPool_1.html

- Unity repeat-builder PASS (including the two character prefabs and four SfxEvents).
  Imported its serialized character/audio settings locally, verifying existing GUIDs.
  Scene outputs had only whitespace differences, so their original bytes were retained.
- Compatibility for the concrete existing serialized prefabs: retain their original
  spark when the new role prefab references are unassigned. This prevents losing
  baseline Slayer/lethal feedback while author assets remain unavailable; it is NOT
  acceptance of the new profiles. Original Pimen sheets, timing/size/palette import,
  prefab binding and visual QA remain required before production delivery.
- Added real scene-unload coverage for active/idle VFX and disabled parry listeners;
  final Quick remains pending. Added Kenney credits/license to the packaging hook;
  this change postdates the currently running WebGL snapshot.

## Final runtime verification checkpoint (asset work remains open)

- Final runtime/configuration source snapshot:
  `66030443c59a7bafa667ec0021b60800d0670f0d669d21d351ea427d824b0d7d`.
  Quick PASS: 34/34 actual PlayMode cases, including scene unload and listener removal.
- Earlier WebGL PASS: 54 checks, snapshot
  `1f81fdc7dcaa6da87d4f5ee3f1e85837c9502c3d53db2e809d114000414e0e5c`;
  three reference wins 91.536 / 91.798 / 84.886 combat seconds at 850 HP.
  This predates the final pause execution order, existing-prefab compatibility,
  disabled-hitbox guard and Kenney packaging changes; final WebGL rerun is pending.
- Source reviewed and whitespace checks passed. The only character prefab changes
  relative to this task's baseline are the approved presentation settings, new
  sound references and retired Slayer flash field; scene content is unchanged.
- Pending assets: obtain original free archives through author downloads. Import,
  inspect actual sheets, generate/bind role clips/prefabs, verify exact duration/size
  and actual Pimen animation-event reuse, then do Windows visual/listening QA and
  production ZIP/CRC/SHA256/QA-exclusion checks. Do NOT report this task complete yet.
- Retain `E:/CodexTemp/TypingHero/combat-feedback` baseline and serialization-before
  files for safe continuation over the pre-existing dirty checkout. Orca author tab
  `bae472d1-d82f-4a4a-bc0e-8d6757b554ab` remains open for manual asset download.
  No branch/worktree or publication was created. Existing Dark Ages ZIP untouched.

- Kenney original ZIP: CRC PASS; SHA256
  `029d734af1582474edf3a694d1b0cebc97c1c152f2f39fa34d4c2bafc5de77f8`.
  All six imported OGG files match their original archive bytes exactly. The temporary
  Kenney ZIP and redundant Unity serialization archive were removed; product clips,
  provenance/license and the pre-change serialization baseline are retained.

### Final results / remaining blocker

- Final Quick: 34/34 PASS. Final WebGL: 54 checks PASS, 373.143 seconds, same
  `66030443c59a7bafa667ec0021b60800d0670f0d669d21d351ea427d824b0d7d` input snapshot.
  Repeat builder PASS; Kenney credit/license present in QA output. Local source
  re-hash matches; final diff/whitespace review passed.
- Chromium 153.0.8010.12 / SwiftShader under the existing 2CPU/8GiB/shm1GiB limits.
  QA viewport 960x600; 102 fixture reads / 5 fixture writes; no exceptions.
  Three reference wins: 85.942 / 89.693 / 90.408 combat seconds (850 HP).
- Current final source retains original sparks for unassigned role references;
  original Pimen import/prefabs and full visual/listening acceptance remain pending.
  No RAR appeared in Downloads at the final check. User was asked asynchronously
  for an existing archive location. Do not recreate or substitute the original art.
- No production build/ZIP, stress/soak or publish was run. No full completion claimed.
  Next step is original-sheet inspection/import, then actual role clip/prefab tests,
  Windows visual/listening QA and production packaging/CRC/SHA256/QA-exclusion.
- Latest results retained for continuation under
  `/data/typinghero-game-loop/results/20261017T044218-webgl` and the adjacent final
  Quick result. Persistent source/Library/toolchain/QA build remain in the runner's
  established task root. Local baseline and pre-change serialized files remain at
  `E:/CodexTemp/TypingHero/combat-feedback` because this task is not complete.
## Original Pimen assets obtained (2026-10-17)

- Downloaded both free original RARs using the author's anonymous download flow;
  no promotional GIF or substitute art. 7-Zip extraction/CRC successful.
  Hit Effect 01.rar SHA256: `956831ecc5e74504095cf93159cba0cd7f7f879fd3190a9ac67d2197c8816fe3`.
  Effects.rar SHA256: `126429aca7542f620802ce501e1ad827dda29ffe9ec1095fd07f87e2f3290817`.
- Author licenses rechecked on both original product pages. Imported original PNGs,
  Aseprite source and author license/provenance notices under `Assets/Art/VFX/Pimen`.
- Actual sheets: Hit Spark 336x48, seven horizontal 48x48 cells (last transparent);
  Battle Effect 1 has leading/trailing blank rows around four 64x32 frames;
  Effects 2-4 have four frames followed by a blank row. Slice only visible Battle frames.
- Builder now generates seven role prefabs/one-shot clips/controllers: normal150ms,
  parry/lethal180ms, four Slayer variants100ms, centered pivot and uniform scales
  at 42 screen pixels/world unit (420px combat viewport / 10 world units).
  Slayer derived sheets preserve alpha/shape with white palette. Originals retained.
- Acceptance remains pending actual generated asset tests and visual/browser QA.

- Import correction: initial slicing during build preprocessing returned zero sprite
  subassets. Reused the existing DarkAges `SpriteDataProviderFactories` pattern,
  preserving sprite IDs by name, with explicit Texture2D shape and synchronous
  reimport. A standalone Unity `PixelUIBuild.SerializeOnly` completed successfully;
  imported all generated assets and importer metadata back into source, preserving
  existing GUIDs. No package/dependency change or checker relaxation.
- Added actual Pimen prefab event/lifetime/reuse tests for all seven profiles;
  final Quick and repeat-builder/WebGL checks are now pending on these assets.

### Pimen prefab verification

- Corrected Unity clip end rounding: the last held key is just before the end,
  explicit clip start/stop and release event remain exactly at the requested
  100/150/180ms. Original failed test caught 151ms; no assertion was weakened.
- Quick PASS 35/35, snapshot
  `ad0b1e438d1f6b1e4043079c8d989307b1651ce0cc834e142295d743d0d593a8`.
  The actual seven Pimen prefabs passed length/event/uncompressed Point import,
  non-looping, return through real unscaled animation, duplicate-release protection,
  and color/scale/speed reset on reuse. Parry's actual new clip holds on pause.
- WebGL/repeat-builder is running from the same snapshot. No production yet.

### Windows visual QA / correction

- Native Chromium 153.0.8010.12, Intel UHD Graphics ANGLE D3D11 (no SwiftShader).
  16 checks passed: 960x540/1280x720/1920x1080/640x360, pause/resume, real parry/
  typing victory and Slayer, three restarts at 40/850 HP, menu, iframe and fullscreen.
  21 fixture reads / one fixture write; zero exceptions. No real Firebase write.
- First native driver had a missed immediate restart key; a repeat used actual
  Restart buttons with held native input and full resume state/timeScale checks.
  Product code was unchanged for this harness correction. All three restarts passed.
- Frame inspection found normal orange spark and ivory flash at contact, but BLUE
  PARRY WAS MISALIGNED: old anchor projected near skeleton's head (~405,300) while
  swords cross near (~344,320) at 960x540. Correct its local position under Attack1
  from (1.04,1.47) to (-.55,1.0). Same existing anchor, VFX-only change; no collider,
  Rigidbody, damage, animation or parry-window change. Re-serialize and rerun affected
  checks/Windows frames before delivery. Other profile visual review remains open.
- Audio routing/preferences/pause verified by tests; direct speaker listening remains
  unverified in this environment. Do not claim listening or measured player-device FPS.

### Final visual correction verified

- Final source snapshot: `f3146419f10dcfd1f9e42bb9237c61913d3958c112ef59945374b68d08b3111e`.
  Quick remains 35/35 PASS after anchor correction. Repeat builder PASS with exact
  comparison of generated prefab/clip/controller/PNG/meta bytes and existing config.
- Native Windows final rerun PASS 16 checks, same Chromium/Intel UHD ANGLE D3D11.
  20 fixture reads / one fixture write, no exceptions, three restarts at 40/850.
  All four requested resolutions plus actual iframe/fullscreen verified again.
- Inspected actual native video frames: normal spark/ivory flash at contact;
  corrected BLUE parry now at (~340,320), sword crossing, not the skeleton's head;
  white Slayer sparks preserve silhouettes and existing trails; lethal switches
  from Slayer to a larger orange burst before death animation. Typing/cues stay clear.
  Reference visual frames will be included in the source delivery verification folder.
- Unity output comparison against local source shows only whitespace differences
  in the two scenes; all generated assets and character prefabs match exactly.
  Original local scene bytes retained. No dependency/public API/health/balance change.
- Final WebGL 54-check regression is still running; production/ZIP checks follow.

- Final native frame review confirmed the corrected blue parry overlaps the sword
  crossing (final video frames 052/143), normal ivory/orange (frame105), white
  Slayer feedback (representative late frame045) and enlarged lethal burst (late frame297).
  These actual-render frames are selected for source ZIP verification evidence.

## Final delivery (2026-10-17)

- Implementation complete: original Pimen art/roles, contact sparks/flash/audio,
  corrected parry sword anchor, Slayer afterimages and lifecycle/pause/pooling.
  Public APIs, character animations, arena/UI, HP40/850, balance/typing/scoring/
  statistics, Firebase schema and dependencies preserved.
- Same final source `f3146419f10dcfd1f9e42bb9237c61913d3958c112ef59945374b68d08b3111e`: Quick35/35, WebGL54 checks, repeat builder and
  native Windows16 checks PASS. Final server duration378.643s;
  104 fixture reads / five fixture writes, zero exceptions. Reference wins
  90.295 / 92.321 / 89.434 combat seconds. Native Intel UHD ANGLE D3D11 used
  actual keyboard/mouse; all requested resolutions and iframe/fullscreen passed.
- Production PASS, QA bridge absent from JS/wasm/data, credits/licenses included.
  ZIP `TypingHero-WebGL-combat-feedback-20261017.zip`: 26 files / 15896613 bytes,
  local/server CRC PASS, SHA256 `f4e64cdbf60ec47616bff549046c6a1984c47e3f2709b62685b07f5d71f97cdd`.
  Source `TypingHero-Source-combat-feedback-20261017.zip` includes portable verification reports and four real-render frames.
  Both artifacts are in `C:/Users/ADMIN/Downloads`; SHA256SUMS stored alongside.
- Direct speaker listening remains UNVERIFIED; audio routing/mute/volume/pitch/
  pause/channel separation passed tests. No claimed player-device FPS measurement.
  No stress/soak or itch.io upload/publish performed.
- Existing Dark Ages ZIP unchanged, SHA256 `b7b6ea706fbed516d91223e6cb95fb0fce9c26d52fae38365c205ff0ca48b118`.
  Existing dirty checkout preserved; no new branch/worktree or commit. Temporary
  `E:/CodexTemp/TypingHero/combat-feedback` captures/baselines are removed after
  verified packaging; persistent runner/toolchain/results retained as test evidence.

## Shogun concept continuation (2026-10-17)

User approved the supplied implementation plan: replace background/UI presentation
with Shogun Showdown direction; preserve40/850HP and all gameplay contracts.
Existing combat-feedback delivery above is baseline, not acceptance for this theme.

- Baseline doctor PASS at original f3146419 snapshot,3.2GiB free,license present.
  Existing staged/unstaged/untracked changes preserved. Tracked baseline patch:
  E:/CodexTemp/TypingHero/shogun/baseline.patch; no branch/worktree/commit created.
- Mapping: Shogun Steam reference -> layered Japanese setting / dark foreground /
  restrained interface; existing PixelUIFactory UGUI components and VT323 reused.
  Approved geometry is recorded by wireframe bundle design/layouts/shogun/web;
  actual browser-rendered captures and helper check PASS. Geometry is from user
  plan, not measured from the reference. Source-game art/code is not copied.
- ShogunTheme replaces final DarkAgesSkin.Apply in builder. Migration preserves
  existing UGUI objects; adds modal Top1000 in menu/pause, input/focus ownership,
  refresh and unchanged provider/row pool. Camera340/84 ortho and Slayer ratio.78.
  Typing error/timeout border180ms without text shake. Button1px depress and
  unscaled120ms panel reveal. Existing APIs/gameplay/schema/dependencies unchanged.
- Original Kenney UI Audio ZIP downloaded,CRC verified; click1/switch2 imported
  byte-identically with original CC0 license/provenance. Existing SfxEvent/pool/
  mixer/preferences extended with per-event pause configuration; reset on return.
  UI cues expire while paused; combat voices hold. Direct listening unverified.
- First Quick PASS37/37, snapshot1339d33490f991c167eaf619a522c7d4c7c8b0da18ce4d0307af188161a10917.
  Strengthened pool test afterward to force same AudioSource; final rerun pending.
  WebGL/repeat-builder currently running snapshot8f54ccb2722648944304f9f98490cf6f12bbcc263562d635819a50c96933e6f3.
- Original Japan ZIP still missing. Author page confirms13layers353x180 and
  commercial/noncommercial game use/modification, forbids asset redistribution.
  https://lyniadesign.itch.io/tiny-pixel-japan-parallax-background
  Direct local/server HTTP403. Orca page0d8a7260-a5b5-41c9-a71a-611befefb9d0
  passed Cloudflare checkbox and renders source page, but native Download click
  emits no download. Public anonymous download endpoint also403. No cookie/token
  extraction and no promotional-image substitution. Asked asynchronously for an
  existing original ZIP path; continue independent UI/runtime checks meanwhile.
- Pending: original ZIP inspection/import/parallax/recolor/license packaging;
  actual gameplay representative visual correction,all affected screen QA,
  leaderboard keyboard shortcuts/1000rows/refresh,final Quick+WebGL,builder repeat,
  Windows4resolutions/iframe/fullscreen,listening limitation,production/source ZIP
  with credits/CRC/SHA256/QA exclusion. No stress/soak or itch publication.

### Original Japan layers obtained / representative corrections

- Original ZIP downloaded through the author's anonymous file endpoint (not previews).
  HTTP request used only Cloudflare clearance in memory, no account cookies and no
  token/cookie values in output/files. ZIP CRC PASS; SHA256
  c268962fcbe98d43de1e7c2fd43cc669cc903e9a2a8635d5e272d5ae7fe86667.
  All13original353x180PNG files imported byte-identically under
  Assets/Art/Environment/TinyPixelJapan with source/license/credits. No bundled
  license file existed; retained the author's exact published license with URL/date.
- ShogunEnvironment owns original Point/uncompressed import, layered sky/ridges/
  forest/shrine/house/ground, camera-relative parallax and dark edge trees. Palette
  is renderer tint; physics objects untouched,old environment renderers disabled.
  Scene migration remains pending final serialization/runtime acceptance.
- First repeat-builder caught a real unstable UIButtonFeedback component ID on
  recreated primitive templates. Runtime bindings now live on scene consumers;
  second repeat-builder PASS. No comparison/threshold was relaxed.
- First native Windows inspection found low-contrast white/gray leaderboard rows
  and labels retaining old312px centering inside400px buttons. Fixed semantic row
  colors and common label sizing. Preserve alpha on pause shade so arena stays
  visible; assign presentation pause/result colors. Final rendered QA pending.
- Added read-only QA fields for modal/camera, and real keyboard/scrollbar checks
  for menu/pause,all1000rows,refresh retaining bottom position,R/M blocking and
  Escape returning to pause. New modal's Open also rejects a hidden pause canvas
  so a retained keyboard selection cannot open it while playing.
- Earlier native driver's second immediate Escape missed due to key release/frame
  timing; driver now needs held input/release observation,as the prior delivered
  native harness did. Product GetKeyDown path is unchanged. No pass claimed yet.

### Keyboard regression and final presentation correction

- New full WebGL modal checks passed16checks before actual combat input unexpectedly
  returned to MainMenu. Last state had no errors,not a crash. Investigation verified
  the hidden pause Canvas left its Selectables interactive: WASD navigation/Space
  could submit Main Menu after modal focus restoration. GamePauseController now
  owns pause visibility and its CanvasGroup,clears selected descendants on hide,
  and disables interaction/raycasts until shown. No typing/parry timing changed.
- Quick PASS39/39 on fa0f612f9db3a323d0e692e5432ddea4d88b3ee10a4e96d50483471ee82d7592,
  including the regression: resume clears focus and a real UGUI Submit cannot
  invoke a hidden pause action; original Kenney clips,mixer and pause routing.
  Existing35combat/game cases remain passing. Runtime/tests unchanged afterward.
- Initial native Windows candidate PASS31checks,Chromium151.0.7922.34,Intel UHD
  ANGLE D3D11,22fixture reads/2writes,zero exceptions. All4resolutions,real typing
  victory/parry/Slayer,defeat,3restarts,menu/help,1000-row modal,iframe/fullscreen.
  This is not final appearance acceptance: frame review found straight tree cuts,
  invisible scrollbar thumb,SCORE label height clipping and short menu volume bars.
- Fixed forest edges by slicing the original Trees PNG at transparent gaps,not
  shifting the entire opaque-bottom sheet. Left140x150,Right53x150; originals remain
  byte-identical. Added a flat dark ground shade from original Sky sprite so floor
  silhouette is continuous. More mint in sky tint. Updated SCORE bounds,menu field/
  button centering and volume geometry,explicit jade scrollbar handle,full4-edge
  typing error/timeout flash,and larger bold gold enemy cue with unchanged timing.
- Second final WebGL is running snapshot1f59c5f2c967aadf36b1c6bb090fbfb4045cacecd6d62ad62be4f9a718175379;
  repeat-builder PASS includes generated sources/audio/layers. Native final rerun
  follows its build. Later editor-only cleanup clears obsolete SpriteSwap references
  from ColorTint buttons; no runtime behavior change. Final serialization/import,
  production QA-exclusion and artifact checks remain pending.

## Final Shogun delivery (2026-10-17)

- Completed the approved Shogun concept: original Japan pack,layered jade/coal
  environment,dark foreground,flat themed UI and modalTop1000 in menu/pause.
  Corrected background cuts,bar geometry,button centering/cue readability and the
  actual hidden-pause keyboard navigation bug. Retained40/850HP and all gameplay,
  schema,public contracts,character animation and package versions.
- Original Japan13PNGs and Kenney2OGGs match author archives. Licenses/credits are
  in source and compiled distribution. Source is a private project-owner delivery;
  author layers must not be published as a standalone/public asset package.
- Quick39/39,full WebGL66/66,native Windows34,actual gameplay iframe/fullscreen6
  and production smoke8 PASS. Final103-file builder repeat PASS. All browser
  Firebase traffic used fixtures and external requests were blocked; no real write.
  Audio routing/gain/mute/pause/reuse verified; direct speaker listening UNVERIFIED.
  No device FPS claim,stress/soak or itch.io publication.
- Production inputc5066e5a6d455c309d17e8f1767aff660872ef5c3acddddacfecbc1556c092a1,
  qa=false and no bridge marker in JS/WASM/data. ZIP28files/15,945,248bytes,
  CRC32b64fbfdb,SHA256b5e56079314c73fac75e20495db47a4a2e77897fa793f46a81aab21ebced53b3.
  New WebGL/source ZIPs and shared SHA256/CRC32 receipt are in Downloads with
  shogun-20261017 suffixes. Earlier Dark Ages/combat ZIPs remain unchanged.
- Final source/verification details: docs/game/verification.md and approved
  design/layouts/shogun/web. No existing scene ID was removed; existing GUIDs and
  dirty checkout preserved. Final diff/whitespace review passes. No Git commit,
  branch/worktree or publish created. Task-owned native captures/downloads and
  serialization backups are removed after checked packaging; established runner
  source/Library/toolchain/results remain for reproducibility.

Editor maintenance note: DarkAgesSkin's former Apply entry remains unused; its
importer/primitives are still used by the existing factory bootstrap. It has no
runtime effect. Remove that legacy entry with the bootstrap cleanup when a future
task replaces those editor primitives; this task only replaces the final skin.

## Windows portable continuation (2026-10-17)

- Approved: Windows x64 Mono production ZIP of current game, windowed1280x720,
  resizable; no gameplay/art changes. Particle/reference direction remains pending.
- Add GameLoopBuild.Windows64 and runner windows profile; reuse existing credits,
  activated2022.3.25f1 image and source snapshot. Restore build-only player settings.
- Keep modules/assets/cache/builds on Ubuntu. Only deliver the final ZIP locally.
  /data has3.2GiB free; /home/quan has4.2GiB. Install only official Windows Mono
  module180,709,365bytes, verified author MD5cfcfd0c210f4094cdbd5ad9f02871b2b,
  under /home/quan/typinghero-game-loop/toolchain/windows-mono-2022.3.25f1.
- Acceptance pending: doctor, production build, PE/dependencies/QA exclusion,
  isolated Wine/Xvfb startup/render/log smoke, archiveCRC/SHA256 and delivery.
  No real Firebase writes during QA; no stress/soak or itch publication.
- Doctor PASS; runner contract8/8 and Quick39/39 PASS (snapshot
  1749db85a8f0a57257ed4ec50218adf8b2dd3b09371205b6c5185f2c0ac53819).
  Windows module installed883entries; SHA256
  f9ab3e979220dea8ca12c9e3221be3f0d8f157c84831296aea54bfabee9aaf48.
  Package/extracted-payload downloads removed after verification. Actual pinned
  module variation is win64_player_nondevelopment_mono; corrected initial preflight
  assumption before build. Build currently running from snapshot
  2c3ade94f8f2b835136a6a1f1d0a6092d39d5272c9c3b723e4bc536cb5f1f36c.
- First Windows build PASS. Initial packaging check incorrectly compared CRLF
  snapshot field values (including CR) against newline-normalized file values;
  suspected PlayerSettings persistence failure was not established. Corrected
  comparator to strip line endings. Explicit SaveAssets remains in finally;
  final normalized restoration check PASS. Rebuilt snapshot
  251293113ae167f93a14a1377d31b07a5e6f3b38afa63d7ba8ab09d584472027.
  Rebuild preflight correctly rejected2.8GiB; removed only verified, regenerable
  Library/Bee/artifacts/WebGL (671,390,937bytes), keeping assets/ZIPs/ShaderCache
  and other Library content. No disk threshold lowered; next preflight5.8GiB.
- First Wine smoke used inherited win32 image setting and could not launch x64.
  Corrected harness to an isolated win64 prefix and removed unrelated legacy DLL
  overrides; no product code changed for this harness issue. Final smoke pending.
- Final production build PASS: results/20261017T080628-windows, snapshot2512931
  as above. PE64 executable/player/Mono DLL checks PASS; all118managed DLLs exclude
  QA markers. ZIP173files/33,657,976bytes; SHA256
  7b7720461515d3cd2ea09996cc7f00c7ec3362435e81fdffa4865f87dc36bb0c,
  CRC327aac5c0b. Ubuntu and downloaded ZIP memberCRC/SHA checks PASS. Ten credits/
  license files byte-identical to prior verified WebGL ZIP. Compared951local
  Assets/Packages/ProjectSettings files against prior source ZIP: only the editor
  build helper changed; gameplay/art/scene/config inputs unchanged.
- Native smoke tooling: original image has only Wine32, even after setting win64.
  Wine64 dependency installation succeeded but a task image commit exceeded180s;
  that setup container was removed and no image tag was created. Switched to a
  temporary running container, typinghero-windows-nativeqa-20261017, with official
  Wine64 packages. Disconnect all Docker networks after setup, before running game.
  Temporary helpers/captures/prefixes: /home/quan/typinghero-game-loop/windows-smoke.
  Keep until native smoke is verified, then remove container/temp; no product change.

### Final Windows delivery

- Native smoke PASS with Wine9.0 Ubuntu9.0~repack-4build3, Xvfb1600x1000,
  software-rendered D3D11 featurelevel11.1. Live TypingHero1280x720 window;
  MainMenu initialized and rendered title/name/Play/help/Top1000/volume controls.
  Inspected actual frame; zero managed exceptions over47.21s including fresh
  Wine prefix initialization. All container networks disconnected before player
  launch; no real Firebase write. No native interactive-combat/FPS/audio claim;
  Wine reports ALSA/no device and DXGI/touch/vsync limitations. Real Windows
  interaction and direct speaker listening remain unverified for this target.
- Delivered Downloads/TypingHero-Windows-x64-20261017.zip and checksums.txt,
  with SHA256/CRC/memberCRC confirmed locally. Extract the entire ZIP and launch
  TypingHero.exe; keep Data/MonoBleedingEdge/DLL companions. No Python required.
- Source adds only editor Windows64 build/shared credits and runner windows
  profile; gameplay/public contracts/Firebase/packages/assets unchanged. Preserve
  prior ZIPs and dirty checkout. New modules/assets/build caches stay on Ubuntu;
  only final requested ZIP/receipt delivered locally. No publish or stress/soak.
- Cleanup complete: owned nativeqa/setup containers and windows-smoke temp tree
  (including inspected Wine symlinks, captures/logs/prefixes) removed; no task QA
  image retained. Official Windows module and normal source/Library/results/build
  outputs retained. No local temporary asset/toolchain directory was created.

## Interactive tutorial continuation (2026-10-17)

- User approved implementation of the supplied six-lesson English safe-practice
  plan: BLADE hit; two timed words; two real parries; 25/250 stamina break;
  real Slayer FIRE hit; two words plus one parry, then Start Battle. Retry only
  the current lesson at full HP; Slayer expiry repeats the stamina break.
- First Play enters tutorial independently of HowToPlaySeen; menu replays it and
  retains quick help. Separate Unseen/Skipped/Completed preference. Skip and
  Start Battle load a clean 40/850HP,250 stamina,zero-stat match. No score writes.
- Mapping: existing Shogun theme/UGUI factory, gold cue and gameplay layout â†’
  TutorialBuild/Overlay below-HUD prompt and targeted highlights. Selected short
  instruction / live arena practice reference is the user-supplied Shogun Showdown
  opening URL. Fetch failed (cache miss); exact flow is specified by the plan.
  Wireframe records approved placement and routine theme composition.
- Implementation decision: scene reloads isolate ordinary lesson transitions and
  retries while TutorialRun retains the in-session checkpoint. Stamina-break â†’
  Slayer stays in the live scene, including its real stun and timer. Reading lock
  is separate from pause; Enter/click continues, SPACE remains Parry.
- Baseline dirty checkout preserved; no branch/worktree/commit created. Doctor
  PASS: activated2022.3.25f1,pinned images,24.4GiB available. Pilot lesson1 in
  progress. Remaining: pilot runtime check, expand lessons,Quick/WebGL including
  preference/retry/pause/focus/restart/iframe/fullscreen/4sizes and modal regression;
  production WebGL/source on Ubuntu plus new Windows ZIP locally,credits,CRC/SHA
  and QA exclusion. Preserve old ZIPs; no publishing or stress/soak. Direct speaker
  listening remains unverified. Temporary work owned by this continuation uses
  /home/quan/typinghero-game-loop/tutorial-20261017 (server) and
  E:/CodexTemp/TypingHero/tutorial-20261017 (small local QA helpers only).

### Tutorial pilot checkpoint

- Wireframe rendered/captured/reviewed and consistency check PASS: tutorial bundle
  revision1,fingerprint4128e0ff438310c403f64ee01b81dcd76ff232c12de3fe412eaa736b9df04bf9.
  Prompt uses the approved below-HUD slot and existing theme. Initial capture
  included the preview editor overlay; replaced with the actual isolated960x540
  root capture before review.
- Quick42/42 PASS on4aec0bf5701b53cff25d6e6986accb2fdbcb3790c4a5218077bbc0ea21ef9119.
  First run41/42: pre-existing ivory-flash test did not restore within80ms under
  that run's frame scheduling. Same runtime passed on rerun; no flash code,
  timing,assertion or baseline changed. New3tutorial contracts passed both runs.
- WebGL pilot snapshot6971e44f678cff451be5b511c67c81de0920fbbedc7a506b20040be42637954e
  is building. All source/cache/build data remain on Ubuntu. Runtime completion
  of lesson1 must precede expansion; no pass claimed yet.

- Pilot build exposed a real idempotence failure: new Tutorial button inherited
  the template's muted label color on the first pass, then received explicit color
  prefab overrides on the second. Captured exact serialized diff (only label RGB
  overrides) and moved TutorialBuild before final ShogunTheme/template styling.
  Repeat builder now PASS on43c6304c1c956fabb934645c3d1218bf55972280690f374fe772b29ba22e3ef1.
  Temporary diagnostic export was removed from code; no comparator was weakened.
  WebGL IL2CPP build continues; Windows-to-WebGL target switch rebuilds cached C++.
- Added dedicated keyboard acceptance drivers for full lessons/failure/checkpoints,
  real focus changes and iframe/native fullscreen. Drivers are syntax checked but
  not yet executed. New TypingManager practice input/reset contracts await Quick.

- Delivery target fixed: Downloads/TypingHero-Windows-x64-tutorial-20261017.zip
  and its checksum receipt. Existing Windows/WebGL/source ZIPs remain untouched.
  New source/WebGL production archives stay on Ubuntu as explicitly requested;
  no itch.io publication. QA will continue using network-blocked Firebase fixtures.
  Final Windows executable also needs isolated startup evidence and PE/managed
  assembly/credit checks; actual Windows hardware interaction/audio is not inferred
  from WebGL or Wine.

### Tutorial representative path verified

- WebGL pilot PASS on43c6304c1c956fabb934645c3d1218bf55972280690f374fe772b29ba22e3ef1:
  actual BLADE keyboard slash reduced enemy HP850 to840 before lesson completion;
  reading ignored typing/Space and held word timer,Resume kept the reading lock,
  no tutorial score writes. Actual960x600 reading/hit frames reviewed: prompt
  above both characters,full labels,gold typing outline. Core regression continues
  in the same frozen build while local implementation expands.
- Expanded TutorialDirector to all6lessons with real word/HP,parry-target,stun/Slayer
  checks. Ordinary transitions/retries reload the session checkpoint; real stamina
  break transitions directly to live Slayer reading. 25/250 preparation is an
  internal stamina hook,not a fake stun. Phase5restart reconstructs phase4.
- Added cue/stamina/temporal mixed highlights,noninteractive tutorial canvas during
  pause,and Enter through the existing button/SFX event. Full lesson progress,
  retries,focus and final normal match remain UNVERIFIED pending next profiles.

- Pilot full regression73/73 PASS (6new pilot checks + existing flow):
  results/20261017T101236-webgl, three850HP wins87.921/93.713/87.722s, zero
  JS/Unity exceptions. Tutorial itself had0fixture writes. Full expanded runtime
  Quick44/44 PASS on3525f5a8e9391e85aae6d000f89c98c9e15bf61caa37a6fe39205eb138d46dd6.
  One subsequent guard also blocks tutorial actions while pause Restart/Menu is
  loading; this delta awaits final Quick and WebGL.

### Expanded tutorial acceptance checkpoint

- Full expanded QA build/repeat-builder PASS, input84057368a15b8030afb57eb1f0d5359a9c4369eb8dd589b616a22a053d7de005,
  results/20261017T103513-webgl. Ubuntu browser reached21checks, then a real miss
  correctly retried lesson3. Instrumented focused rerun reached28checks and
  real stamina break/Slayer before a later miss after Restart. Trace showed
  180-200ms software-render frames,pose request one frame before animation-open;
  at impact the window was still closed. No gameplay timing/window was changed.
- Reused Playwright1.63 client in task temp with existing Windows Edge155.0.4283.45
  and actual NVIDIA GTX1650Ti ANGLE D3D11. Build/assets remain Ubuntu; a task-only
  localhost8123 SSH-forwarded static QA host serves the unchanged QA build. Native
  keyboard input passed ALL6lessons,two-word/Parry checkpoints,intentional mistakes/
  timeout/hit retries,Slayer expiry,Restart in Slayer,persistent skipped/completed
  preferences,replay,clean Start Battle and0tutorial writes. Frame trace about18ms
  in those samples; not a device-performance or human-playtest claim.
- Native overall driver39checks did not finish: iframe NameInput received no text
  and stayed MainMenu. Added explicit native canvas click before field selection
  and read-only DOM focus diagnostics; focused iframe rerun pending. No fake input
  or bridge mutation. Runner now supports the same driver via native Edge and
  external task host,while keeping fixture interception/external blocking.
- Actual cue frame review found the opaque prompt background hid most of '!'.
  TutorialBuild now draws only70px of instruction background; unchanged action
  buttons leave their middle gap clear for actual cue/highlight. This is a visual
  acceptance defect,not timing adjustment. New serialization/runtime QA pending.
  Wireframe r1 preserved under revisions/r1; r2same geometry, updated background
  contract,fingerprint1e1148f8566a16a8172d1ec0cf8f9d8a71c408f3bed0c8292ef3962b2ec8afa0,
  render/capture/check PASS for both revisions.
- Current task host SSH/HTTP process: exec session73954; stop after all QA. Local
  native Playwright client/captures/reports in E:/CodexTemp/TypingHero/tutorial-20261017.
  Server task temp /home/quan/typinghero-game-loop/tutorial-20261017. Final cleanup
  must remove task-owned clients,captures,hosts and Wine temp after recording receipts.
  Established Ubuntu source/Library/toolchain and requested release archives stay.

- Latest Quick44/44 PASS on751be035c8c10f23a19a9700e1173d5391df5c3227dada00fdb895734746731b.
  Subsequent edits are editor presentation (70px background +22pt secondary-menu
  labels to remove observed How-to-Play ellipsis) and conditional read-only QA
  input/focus observation; combat/runtime tutorial code unchanged since this pass.
- Iframe key trace proved keydown events reached the correct focused canvas/frame
  (and not the outer page),but TMP had no text. Testing a real parent?iframe focus
  cycle; added read-only Application.isFocused/last-input fields for next QA build.
  All source investigation uses the pinned Unity build/official2022.3 input docs;
  no engine/input implementation change or mutation bridge was introduced.
- Driver now permits up to6actual UI retries for a bot's miss on slow software
  rendering. Two parries must still succeed within one fresh attempt,all counts/HP
  are observed,windows stay unchanged,and failure to progress still fails. Mixed
  challenge retypes its first word after any retry. This models the accepted safe
  retry flow rather than relying on100ms taps fitting a180-200ms render frame.

### Final verification in progress

- Build893467a01ecd5589472f19e8907d9ddb1205480d338a29cf1475fdd0ec88dab1
  (results/20261017T110714-webgl) passed repeat-builder and all6tutorial lessons
  on Ubuntu after bounded real-input retries. Full ordinary-match regression is
  finishing: first two reference wins84.520/90.473s and third84.688s. Canonical
  exec67782 remains active for final iframe/fullscreen checks.
- Actual new cue screenshot reviewed: complete yellow exclamation and gold box
  visible between actions,characters unobscured. Typography/menu source uses22pt
  secondary labels. New tutorial replay button validation ref now follows the
  existing name-validity rule (prevents a silent active action on empty name);
  this small subsequent menu delta needs final build/smoke,then final Quick.
- Iframe issue narrowed to fixture lifecycle: previous helper loaded Unity in the
  top page then navigated into an iframe. A fresh wrapper loading only the iframe
  accepts physical name entry and enters tutorial960x540. Native fullscreen1280x720
  works. The next Enter was sent before Resume's camera transition completed;
  driver now waits until actual Resume control disappears,not only _paused=false.
  No product input behavior changed. Temporary extra C# input/focus diagnostics
  were removed before build893; only the read-only fields needed by tutorial remain.
- Native headed checks confirmed real focus loss in Parry practice. Reading-Slayer
  background tab suspends bridge publication; helper now checks fresh state after
  returning,without resuming. Removed detached focus-emulation overrides between
  checks; one per-page observer stays in real-focus mode until the task context
  closes. Current native focused flow exec1309,output E:/CodexTemp/TypingHero/
  tutorial-20261017/focus-verified. These focus checks are not yet all passed.
- Live task host still exec73954,localhost8123 to fly-server8123. Native edge profile,
  downloads,traces and captures stay inside task temp; no existing browser profile
  or user tab is borrowed. Build files stay Ubuntu.
- Remaining: complete canonical/real-focus+iframe; final menu-delta Quick/build;
  reconcile serialized scenes/IDs into local source; production bridge exclusion
  and visual smoke; Windows64 production and isolated offline Wine startup; source/
  WebGL production archives on Ubuntu,only new Windows ZIP+receipt to Downloads;
  credit/CRC/SHA checks,diff review,write verified memory and clean task temp/hosts.
  Production smoke helper is task temp/production-smoke.cjs. No Windows build for
  tutorial has been run yet. Old ZIPs still preserved.

- Native focused verification completed PASS55checks,3real focus-loss cases,
  iframe960x540 and native fullscreen1280x720 keyboard hit,38fixture reads/0writes,
  zero JS/Unity exceptions. Edge155.0.4283.45/Intel UHD ANGLE D3D11. Actual current
  cue/Slayer/menu/fullscreen frames reviewed; full cue visible,secondary labels
  uncut,characters unobscured. Direct speaker listening remains unverified.
- Final Quick44/44 PASS on957b6cfdee5109b03eb4f43f8f2c9727e7016510022828b06979a9b1034e6935,
  including final menu validation reference and preference-presence cleanup.
  Final canonical WebGL on the same input is running at
  results/20261017T113623-webgl (exec46631),all6lessons already passed.

- Code frozen. Final Quick44/44 and WebGL build/repeat-builder share input
  957b6cfdee5109b03eb4f43f8f2c9727e7016510022828b06979a9b1034e6935.
  The final browser profile has passed all tutorial cases and is completing normal
  combat; exec46631,results/20261017T113623-webgl. No source edits remain planned.
  Helper scripts for ZIP/PE/credits/checksums,production smoke and offline Wine
  startup are prepared under the existing task temp,not repository build inputs.

### Tutorial release checkpoint

- Canonical WebGL build/repeat-builder and first118browser checks passed on957b6c.
  All6lessons and ordinary combat/modal regression passed; three850HP reference
  wins93.477/94.915/95.207s. The profile then failed iframe Resume: render size
  published before reference-canvas layout settled,so the driver clicked an old
  button coordinate. No product input/timing change. Focused same-build iframe
  rerun8/8 PASS after waiting3actual frames following resize. Full browser-only
  rerun uses the same frozen QA build and updated helper; task temp/browser-final.
- Reconciled Unity-generated MainMenu/SampleScene into the local source, retaining
  all262/563original document IDs and unchanged meta GUIDs; only5/89IDs added.
  Other10generated file deltas were whitespace only and were left untouched.
  Source runtime code remains the Quick44/44 input; production will build these
  already-verified serialized scenes. No new asset import or package change.
- Full same-build browser rerun PASS120checks/548.093s,117fixture reads/5normal
  writes/0tutorial writes,0exceptions. Three850HP wins91.709/90.535/86.424s.
  Native additional27checks PASS: real BLADE hits840HP at all4requested sizes,
  reading input locks,8fixture reads/0writes/0exceptions; actual frames reviewed.
  Previous native55checks cover all6lessons and3real focus losses. No remaining
  tutorial behavior blocker. Production build inputef5da24af7d8f9a5623472d76841f960d9d0d6008f8cb7e0d755637874519ed9
  incorporates verified serialization and driver-only changes; C# unchanged.
  Production exec3892 is building; Windows will reuse this identical server-side
  snapshot through the existing runner. Live QA host exec65412 remains solely
  until production needs are resolved,then must stop and remove task temp.

### Tutorial verified release

- Six English lessons complete through existing typing/combat; Quick44/44,
  full browser120checks,native55plus27size checks,runner contracts8/8 PASS.
  Wrong keys/timeouts/hits,retry checkpoints,Parry/Slayer pause and real focus,
  skip/replay/preference/restart and clean40/850HP/250stamina battle verified.
  Tutorial0fixture writes; ordinary combat and1000-row modal remain passing.
- Production WebGL and Windows64 PASS on identicalef5da24a input; runtime C#
  remains the final Quick input. FiveWebGL payload files and118managed Windows
  DLLs exclude QA markers; PE64 and10matching credits/licenses pass. Production
  browser5checks/7reads/0writes/0exceptions; actual tutorial hit and normal modal
  frames inspected. First blank menu capture was rejected,returned menu reviewed.
- Windows ZIP174members/33,666,777bytes,SHA256
  4aaa37dfc6279104842b1a662536f997391e0ce5faed03f4b235cbdedc53a634,
  CRC32b35fa287; server/local member CRC and checksum PASS. Extracted ZIP launched
  in Wine6.0.3/Xvfb,1280x720,D3D11level11.1; actual menu/practice/BLADE hit reviewed,
  0managed exceptions,networks disconnected before launch,container removed.
- Final delivery names: TypingHero-Windows-x64-tutorial-20261017.zip and
  TypingHero-tutorial-20261017-checksums.txt in Downloads. Source/WebGL archives
  stay in /data/typinghero-game-loop/releases/tutorial-20261017; checksums receipt
  covers all3. Existing10local ZIPs are preserved. Source includes these written
  receipts and approved wireframes; excludes generated QA reports/captures/cache.
- QA hosts exec73954/65412 have stopped; native Edge contexts and Wine container
  are closed. Remove task temp and owned generated reports after source packaging
  and final receipt/diff checks. Reusable Ubuntu source/Library/toolchain/builds
  and requested release archives remain. No branch/worktree/commit/push/publication.
  Direct speaker listening and real Windows hardware QA remain unverified;
  no performance claim or stress/soak. Detailed receipt: docs/game/verification.md.

### Tutorial handoff checkpoint

- Source archive created and CRC-checked:1019members/8,821,677bytes,
  SHA2560c160fa2d0bfc46b744e17f88b74426e04c6fa9c387c0e38bcfb01e955ed743b,
  CRC327ad1ea3f. All staged source members matched their local SHA256 before
  packaging. This post-package checkpoint changes documentation only; no code
  or serialized build input changed after the verified source snapshot.
- Windows ZIP and3-archive checksum receipt delivered to Downloads and verified;
  all10previous local archive hashes unchanged. New source/WebGL ZIPs were not
  downloaded. Both stay with Windows/server receipt in the requested Ubuntu
  releases/tutorial-20261017 directory.
- Cleanup PASS: local task temp/client/profile/captures removed; remote task temp
  and14owned generated-result directories removed after provenance/hash checks.
  Eight Wine links were unlinked without following their targets. Task hosts,
  Edge contexts and disposable containers closed. Source/Library/toolchain/builds,
  prior result history and3requested release archives retained on Ubuntu.
- Implementation,verification,packaging and cleanup complete. Dirty checkout and
  pre-existing work preserved; no branch/worktree/commit/push/publication. Remaining
  evidence limits are direct speaker listening and real Windows hardware execution,
  as recorded above; no runtime acceptance blocker remains.

## Pixel physics sparks â€” foundation checkpoint (2026-10-18)

- User approved reviewing, verifying and committing the existing foundation on main
  before creating the feature child worktree. Existing presentation/tutorial/art,
  source-backed docs, tooling and product marketing assets are preserved as baseline;
  no pre-existing work is discarded, stashed or moved.
- origin/main fetched and matches main at 5f2f427. Doctor PASS; Quick 52/52 PASS
  on e4f99c581d3125aa640dcac9403ae6fc9d983598aa03cad7a9a4591206d4760d,
  matching the prior verified sorting foundation. Runner contracts 8/8 PASS on Ubuntu.
  Windows unittest invocation cannot import Linux-only fcntl; rerun on fly-server passes.
- Whitespace checks and targeted secret-pattern review pass. No push/release requested.
- Next: create one Orca child worktree based on this committed main, branch
  feat/pixel-hit-sparks; implement parry first, then hit/Slayer/lethal, lifecycle,
  Quick and real-keyboard pixel-sparks WebGL checks. Merge only after acceptance.

### Pixel sparks feature checkpoint

- Base ff1c696873fb234b62a7b0524f97ecec32380387; target main; remote origin.
- Orca child id e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks,
  parent e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/TypingHero.
  Orca added a user prefix initially; renamed the one branch to feat/pixel-hit-sparks.
- Parry prototype: two world ParticleSystems, explicit emission after placement,
  16PPU integer sizes, unlit alpha material, gravity18 and damped collision plane.
  Runtime/builder currently unverified. Lethal final burst must finish while earlier
  effects clear at match finish; owner disable/unload always clears everything.
- Remaining: generate assets, verify parry physics/playback, extend hit profiles,
  meaningful lifecycle tests/QA scenario, visual/performance comparison, final review.

### Pixel sparks acceptance retained from approved plan

- Replace only normal/parry/Slayer/lethal impact bursts; keep slash, flash,
  afterimages, SFX, shake, hitstop and all gameplay contracts. No player hurt,
  bone destruction, character-animation, pipeline, plugin or purchased-asset changes.
- One white binary-alpha Point texture, flat unlit alpha material, no soft/additive
  trail or arbitrary bitmap rotation. Integer1x1/2x2 dots70%,3x1/1x3 streaks30%,
  one source pixel=1/16world unit. Bright/main/dark stepped palette, then expire.
- Normal24/.45-.65s orange; Slayer32/.35-.55s ivory/faster;
  Parry40/.60-.85s pale blue; Lethal56/.75-.95s orange/widest.
 80% forward/20% reverse-up, horizontal3-7, vertical1-4, gravity18,
  bounce.3-.45, dampening.35-.5, slow collision particles die early.
- Two nonlooping World ParticleSystems/profile, no playOnAwake. Fixed collider-foot
  plane assigned before explicit emission; order30. Attacker-to-target direction;
  parry target context, mirror; lethal priority survives Slayer exit during damage.
- At most8active bursts/owner; oldest recycled while confirmed-hit audio remains.
  Clear/reset on reuse; release exactly once after both groups die. Unscaled
  during hitstop; pause/tutorial reading freezes simulation/age, no re-emission.
  Disable/restart/unload clears; Slayer exit leaves already-emitted profile intact.
- Quick plus actual-keyboard pixel-sparks scenario at1280x720/640x360 and both camera
  modes, fixtures/network isolation/read-only bridge retained. Same-device before/
  after object/particle/allocation/frame-time samples. No stress/soak/release/push.
- Inspect real playback and nearest4x details: crisp marks, no halo/pink/old burst
  overlap or cue/pose/text obstruction, visible falling/bounce, bounded cleanup.
- Before merge: final checks/diff review; update feature from newer main if needed,
  main must be clean; local merge commit and integrated smoke, then Orca cleanup.
- First prototype WebGL9c545631 built and repeat-builder passed, but old attack-sorting
  driver timed out before its first hit after stale-word input (0correct letters).
  This is not a VFX runtime PASS. Read-only probe now handles particle renderers.
  First Quick92c4c91 compilation rejected NUnit Is.AnyOf; replaced with supported
  EqualTo.Or.EqualTo. Retry2c08249 is running. No acceptance threshold changed.
- Task temporary root E:/CodexTemp/TypingHero/pixel-hit-sparks. Native QA can reuse
  existing Playwright1.58.2 at itch-shogun/pw158; use a fresh isolated Edge context.
  Per-frame allocation counters require Development QA (Unity2022.3 Memory Profiler
  docs); opt-in --profile-allocations leaves ordinary/production build flags intact.

### Parry offline milestone

- Quick56/56 PASS, snapshot766f56f59d40956d1eb01b410b05b42f767bb21699a1753c1bf85efec4140af4.
  Actual generated Parry prefab:40particles,28dots/12streaks,32forward/8reverse,
  integer16PPU sizes, mirror and context direction, real native particle gravity/
  bounce/plane containment, pause reading/age, unscaled hitstop, exactly-once release,
  clean reuse,20spawn calls bounded to8active and owner disable cleanup all pass.
- Two initial test defects fixed: transform float equality now uses1e-6world-unit
  precision, and reuse actually leases from ObjectPool before enabling (rather than
  manually enabling an idle object). No runtime behavior/acceptance was weakened.
- Parry-only Development WebGL now running with fixed particle-aware observer.
  Native playback/captures remain unverified. Other hit profiles remain Pimen until
  this parry milestone is inspected.
- Main gained new art-task edits after the authorized baseline commit; preserve
  these independent changes. Recheck main cleanliness/new commits at integration.

### Parry playback corrections (in progress)

- Development build b76d6f9141ef56875913d9d7c81fd89020dd8eeac01b9247848af48ca9e890ad
  completed with repeat-builder. Software802checks reached parries at both sizes,
  native Edge/Intel UHD D3D11 reached40particles at1280 and exact pause hold.
  Both failed restart with NullReferenceException; neither is an acceptance PASS.
- Cleanup now removes list entries before releasing and skips already-destroyed
  instances (independent pool roots may die before the owner). Probe skips dying
  renderers and records full exception traces. Added root-before-owner regression.
- Actual video reviewed at play size and nearest4x: preserved parry pose/slash;
  new sparks were too dark too early. Unity Fixed gradients select the next key
  (https://docs.unity3d.com/kr/2022.2/ScriptReference/GradientMode.Fixed.html).
  Corrected key endpoints to.15white/.7main/1dark; alpha remains solid. Added
  palette-phase assertions. Live capture now precedes pause (menu covers the arena).
- PixelSparkBurst order200 follows TutorialDirector100, before particle simulation;
  added a same-frame reading-lock driver regression. Expanded lifecycle/profile
  checks pending Quick and rebuilt parry playback. Hit profiles still not wired.
- First Development build09b1edd4 hung in native asset collection/ALSA mixer.
  Read-only gdb stack showed engine asset-GC waiting and native FMOD mixer spinning;
  stopped only that task container. Retry b76d6f914 passed build; no engine/settings
  workaround or gameplay change. First gdb attach required task-target sudo; disabled
  automatic shared-library loading to avoid host BFD's ELF-loader error.
- Same-observer baseline1be19ffc built from ff1c696 wiring overrides plus identical
  QA/performance instrumentation; software driver missed its first normal word at
  ~212ms/frame. Native baseline pending, never count this as WebGL profile PASS.
  Immutable baseline/prototype players preserved only under task temp for comparison.

### Pixel sparks current checkpoint (2026-10-18, before extending hit profiles)

- Quick61/61 PASS, input8e211a2b147aba8a4ae47443621b1dedca543484369783cae005959daba4e55c,
  results20261018T083158-quick. Includes all9PixelSparkTests then present: profile
  config/palette phases, mirroring, gravity/bounce, pause/hitstop/reuse/release,
  context/cap, order100reading-lock/same-frame freeze, Slayer exit/match finish,
  pool-root-before-owner teardown and scene unload. No skips or errors.
- BuilderRetainsInspectorTuningAndPrefabGuid was added AFTER that snapshot;
  it is not covered by61/61. PixelSparkAssets now creates missing profiles only,
  retaining Inspector tuning on existing assets. This needs the next Quick.
- Parry tracking now uses one ordered List, recycles8new pixel bursts while
  preserving legacy animated-prefab pool behavior, and removes dead handles safely.
  Pixel bursts are always reparented to independent world pool roots before emission.
- Baseline native copy was INVALID: copied shared build-qa after another task had
  started replacing it (only loader present). It timed out with0checks; discard,
  do not use as baseline/performance evidence. Runner now copies pixel-sparks
  players into each result/player while holding its lock, even if browser fails.
- Shared runner/library were occupied by another task's non-Development WebGL
  (081946, snapshot2b5fd2a6); do not stop/delete it or its0831430Quick predecessor.
  One overlapping queued WebGL attempt was rejected by the lock before unpacking.
- To avoid cross-task Development/normal cache thrashing, task-only clients
  E:/CodexTemp/TypingHero/pixel-hit-sparks/feature_run.py and baseline_run.py use
  the unchanged standard runner/lock,2CPU/8GiB limits, with only the Library mount
  redirected to /data/typinghero-game-loop/task-pixel-sparks/library. No project
  dependency/version/render/build-resource setting changed. Shared Library remains.
- Current parry-only Development build: exec24860, results20261018T083445-webgl,
  inputa09bebb85e204a05a32c8c76ae6c73e9dd42fc4526ceb4925f73fa0ba3bc19ef.
  Cold task Library import is underway. Use result/player for native copy, NOT
  shared build-qa. C# includes cleanup guards/full error trace, corrected gradient,
  order200 and Inspector-preserving builder. Latest driver-only edits deduplicate
  frame samples, tag phases, and capture live AFTER resume; native may reuse this
  unchanged player with the current driver and its independently recorded hash.
- First native prototype artifact remains task-temp/parry-player; it has the OLD
  gradient and restart exception. Video/nearest4x reviewed for diagnosis only.
  The new parry milestone is NOT PASSED yet. Do not extend hit profiles until it is.
- Next: wait24860; copy its immutable player + matching snapshot; native parry-only
  two-size playback/pause/restart; inspect live/video/4x and fix remaining errors.
  Then wire normal/Slayer/lethal, add hit-direction/priority/cap/reuse checks, Quick,
  baseline/final native comparison and mirrored-actor QA fixture, final review,
  recheck/merge clean main via merge commit, integrated smoke and owned cleanup.
- Main has further independent art/runtime-generated edits after ff1c696; preserve
  them. Feature is still uncommitted on feat/pixel-hit-sparks, baseff1c696.

### Parry complete; all hit profiles wired

- Parry forward native138checks PASS at1280x720/640x360, plus motion/release
  extension151checks PASS. Inspected live full-size images, nearest4x640 details,
  recorded fall/ground/bounce frames, flat blue rectangles and preserved slash/cue/UI.
  Exact pause age/position holds; resume has no duplicate emission; restart no errors.
  Inputa09bebb85e204a05a32c8c76ae6c73e9dd42fc4526ceb4925f73fa0ba3bc19ef.
- Mirrored actor fixture native142checks PASS at both sizes, zero exceptions.
  Force means -2.6764/-2.7063WU/s versus positive forward means; actual blue fan,
  reversed parry pose and normal readable HUD inspected. Fixture changes only actor
  xpositions/xscale via a disposable Editor hook; no runtime/bridge mutation.
  Artifact091510-webgl, input7bd1158bfcf7f30a31e344e58a5b655b1edfa1f6f5ab4671fd7159882bd89b3a.
- Initial mirror091106 failed builder groundY lookup because custom tar entries
  all had timestamp0: Unity reused the baseline assembly when new source had the
  same timestamp. Set per-run archive mtimes; rebuilt same bytes successfully.
  Timestamp policy is in snapshot metadata. Do not use091106 as an artifact.
- Quick62/62 PASS (622c4e318b1f6a37f71952d69f1db40862f698564f9156e85f464a4ee3823007),
  including the Inspector-preserving builder test. Baseline valid player090104
  is immutable and includes expected old animator/prefab wiring plus same observer.
  Native measurement run3796checks completed both victories, zero exceptions;
  second size's poll missed180ms lethal but per-render mask127 proves it rendered.
  That run's status is failed capture assertion, NOT a full baseline profile PASS.
  Data/video remain valid measurements; driver now uses render mask for baseline
  transient lethal only. Final feature still requires live particles for all4roles.
- After parry milestone, wired all profiles via CombatFeedbackBuild. Hitbox changes
  only add target transform to internal VFX context; damage/timing/dedup unchanged.
  Ordered hit pool caps8new bursts across profiles, reclaims oldest, keeps audio;
  legacy animator consumers retain original tuning/cap behavior. Pixel spawn owns
  identity scale/rotation/contact position and attacker-target force before emission.
- Quick65/65 PASS, inputd85dc0f2ca47747637e1a6f36e717dcb64591f7e20a7fab14280a24025950cd2,
  results092126-quick. Adds real-prefab hit selection/cross-profile cap/direction,
  clean reuse/color/scale/contact, real hitbox duplicate colliders and captured
  lethal Slayer exit/IgnoreHitstop. Full final WebGL is now running (exec follows).
- Native artifacts now transfer as gzip tar from immutable result/player, verify
  required nonempty files/QA marker, then extract into task temp. No shared build-qa
  copy. Final runner also retains matching serialized product outputs under
  result/serialized, needed to commit final scene/prefab references without races.

### Final lifecycle correction and integration gate

- First full native run092524 reached all four particle roles and a40/0HP victory,
  but failed result cleanup: three lethal particles were frozen at age .941s by
  the result overlay. Added internal read-only ResultOverlayVisible context from
  GamePauseController; PixelSparkBurst clears on results while ordinary pause holds
  lethal particles. Regression uses actual pause/results APIs, not a simulated flag.
- Final Quick **66/66 PASS**, result20261018T093521-quick, input
  `c6d86c7acee7b370d7185a9eeb1bfa11c8e7578df58706c921771876753a7837`.
  Runner contracts8/8, JS syntax and diff whitespace checks PASS.
- Final WebGL093620 build and repeated-builder byte checks PASS on that same input.
  Its software browser failed before first hit at ~198ms/frame; this is NOT a full
  software profile PASS. Native QA uses its immutable result/player plus snapshot.
  Serialized scene/prefab outputs match committed-intent source except whitespace;
  PixelSparks asset bytes/metas match exactly before trailing-whitespace cleanup,
  with no duplicate or changed GUIDs. Staged review found Unity empty-field spaces
  in new generated assets/metas; removed only trailing whitespace. Runtime/source
  behavior and GUIDs are unchanged, so existing runtime evidence is reused.
- Main remains ff1c696 with independent uncommitted skeleton-art and CodeManu impact
  changes. A separate task reports CodeManu complete, but has not committed it.
  Preserve all those changes. Asked user about conflicting impact intent and whether
  the other task has stopped; no answer yet. Never merge into this dirty main or
  commit another task's work by inference. Feature worktree/branch retained until
  this gate is resolved and integrated smoke succeeds.

- Final native feature-final-native **25,242 checks PASS**,196.756s, both viewports,
  four live particle profiles, normal/Slayer camera, pause/resume,40/0HP victories,
  results cleanup/restart and zero exceptions. Reviewed final captures/video and4x
  crops: fall/bounce/flat colors/readable HUD and retained slash/flash/afterimages.
  Detailed measurements/limits recorded in docs/game/verification.md. Frame p95
  baseline32.840/32.395ms ? final31.825/32.000ms; bounded extra pooled objects,
  particles maxima243/237, active burst max8, no significant allocation regression.
- Feature implementation/art/lifecycle acceptance PASS on isolated source. Source
  diff reviewed: no gameplay contract/dependency/pipeline/version changes, only
  internal VFX context and QA instrumentation/build retention. No QA captures in repo.
  Remaining: commit this feature, resolve clean-main/competing-impact intent gate,
  incorporate any new main commit, affected checks, local no-ff merge, integrated
  smoke and owned worktree/temp cleanup. Preserve child and final player for that.


## White CodeManu sparks ? accepted implementation (2026-10-18)

Supersedes pixel-physics proposal and prior Pimen appearance only. Use existing main
checkout; preserve unrelated dirty skeleton design work. No production/itch release.
Source: https://opengameart.org/content/free-vfx-asset-pack (CodeManu, CC0), archive
SHA256 d706ff61b7b72ba444a76916f74d1549a5afec573594bf656661e4187d1f118e.

- Normal SmallHit64px/150ms; Parry Impact72px/180ms; Lethal BigHit96px/180ms;
  Slayer SmallHit56px/100ms at0/90/180/270 degrees. Sizes use maximum nonzero-alpha
  extent at960x540. White RGB, retain source alpha/shape, nearest resampling.
- Builder trims empty ends, selects8 evenly spaced frames, makes128x128 cells;
  initial flash center stays at pivot across frames. First frame20ms, seven equal
  remaining intervals. Keep seven prefab/controller/clip paths and GUIDs, release
  event, pools, pause/unscaled animation, anchor, order30 and actor sorting1/0.
- Import only three original30fps sheets into CodeManu with source/license/credits;
  build packaging appends notice. Preserve character/slash/audio/camera/gameplay/API.
- Acceptance pending: generated assets and metadata; Quick (runner has no test
  filter); real keyboard WebGL960x540/640x360 normal/parry/all4Slayer/lethal,
  rapid combo,pause/resume,restart; review post-LateUpdate frames and full-speed
  playback, contact/white/core-rays-dispersal/cue+tray clearance. Fixtures block writes.
- Implementation in progress: builder + prefab tests + credits implemented, not yet
  run in Unity. Shared server runner currently locked by an existing unrelated job;
  do not interrupt it. Source cache retained at
  E:/CodexTemp/TypingHero/pinterest-spark-reference until QA/cleanup.

### CodeManu checkpoint: generated assets + Quick

- Unity2022.3.25f1 serialized all7prefabs/clips; retained every original prefab,
  controller and clip metadata/GUID. Source sheets/white strips and metas are in
  Assets/Art/VFX/CodeManu. Removed four unused Pimen generated white strips after
  confirming their GUIDs have no remaining consumers; original author art retained.
- Quick52/52 PASS20261018T081430-quick, input
  6122b54f59213aed994fde9bff82ac1b99970bf1a1ebe19fa3ba8cfe7792de00.
  Includes actual animation-event return,hitstop/pause,reused scale/color/speed,
  new white palette/128cell/pivot64/timing/alpha-derived screen sizes.
- Independent image/serialized check: alpha extents105/102/113 for SmallHit/Impact/
  BigHit; scales project to64.000002/72.000001/96.000000px. All four Slayer strips
  are exact quarter-turn pixel permutations at the same pivot and56.000001px.
  Sample indices SmallHit/Impact1,3,5,7,8,10,12,14; BigHit1,2,4,5,7,8,10,11.
- QA build queued under the existing server lock; another session still uses the
  shared runner between stages. No job/process interrupted. Browser acceptance,
  full-speed visual review and cleanup remain pending; no release requested.

### CodeManu final verification

- QA build20261018T081946-webgl PASS, input
  2b5fd2a683dbae905469418dbaf6e6c9304585ae0726caaf1c49185630ff648f.
  Repeat-builder passes and generated bytes match checkout; packaged credits and
  CodeManu-License.txt match source. Runtime/test source unchanged since Quick.
- Native keyboard WebGL8640checks PASS in175.171s at960x540 and640x360:
  normal,parry,all4Slayer,lethal (mask127at both), rapid combos, active spark pause/
  resume, clean restart40/850HP/score0. Every observed lease has authored scale/
  order30; no Attack/Parry1/skeleton0 sorting mismatches.34fixture reads/2mock writes,
  zero JS/Unity errors. Detailed environment/driver hash in verification memory.
- Reviewed25fps native contact sequences/full frames for both sizes: white spark
  at existing contact, visible above Slayer silhouette, original sword trails
  retained, cue/typing clear. Playback1x measures .999956x/.999967x over2s.
  No device-FPS/audio claim; no production build,release or itch.io action.
- Driver corrections only: destroyed enemy uses-1HP sentinel after victory; missed
  near-expiry pause observations retry another contact and only pass with held
  active lease. No product or acceptance change. Shared runner queue was allowed
  to finish; other sessions/processes/results were preserved.
- Diff review: only builder,7clips/7prefabs,new CodeManu source+strips+metas,
  obsolete4white strips,the targeted prefab test,build notices and affected docs.
  Every retained prefab/controller/clip GUID unchanged. No runtime/public API,
  character/slash/audio/camera/gameplay change; unrelated skeleton art preserved.
- Acceptance complete. Final scoped diff/whitespace/path review PASS. Cleanup
  PASS: local pinterest-spark-reference cache (archive,client,QA player,captures,
  videos,drivers,reports) removed after written receipts; owned server task-temp
  white-codemanu-sparks and results081430-quick/081946-webgl removed after snapshot
  hash checks. No owned browser remains. Shared Ubuntu source/Library/toolchain/
  builds and all other sessions/results preserved. No branch/worktree/commit/push
  created; no remaining acceptance blocker or task temp.

### User-authorized integration and itch publication

- User explicitly requested merge to main and upload to existing itch.io game.
  Preserve the reviewed independent art/CodeManu foundation in main610352b, then
  merge it into the feature. Source conflicts limited to repeated-builder asset
  coverage; retain both legacy CodeManu and new PixelSparks coverage. Documentation
  retains both historical receipts and the current physical pixel implementation.
- Design full-pack stays outside Unity build inputs. A concurrent art task created
  axe_reference.py after foundation snapshot; leave it intact and request a pause
  in writes during integration. No fabricated art acceptance or runtime integration.
- Re-run Quick and final QA build/native after reconciling new foundation; then
  merge clean main using no-ff, integrated smoke, production QA-exclusion build,
  exact production smoke and existing itch.io upload/save/CDN verification. No
  GitHub push requested. Prior uploads stay hidden for rollback.

- Post-foundation Quick66/66 PASS20261018T095638, input
  39d1ff867e100a65ffafddc764561681ce8276372ac49b4c3e25d1302fbdcc44.
  QA095748 immutable player complete; native two-size replay in progress.
  Production queued with the same input and task-owned Library; temporary runner
  waits on the shared lock and preserves the exact production player under results.
- Main now has a separate active HUD/scoring change plus continuing design art;
  do not commit or release those unfinished edits. Requested the other work pause.
  BrowserSkill daemon starts and IPC doctor passes, but extension doctor fails:
  zero browsers connected. Asked user to connect existing logged-in browser.
  Feature remains isolated/clean apart from this checkpoint; no merge/publication
  claim until main cleanliness and authenticated browser are resolved.

- Native foundation-native PASS25,619checks/207.352s, both render sizes/all four
  live profiles and both camera states, ordinary pause/resume, victory results
  clear and restart; zero exceptions,37fixture reads/two mocked writes. Quick,
  QA build and this native player all share input39d1ff867e100a65ffafddc764561681ce8276372ac49b4c3e25d1302fbdcc44.
- Production095953 is still compiling. Task temp retains final/baseline comparison,
  post-foundation native player/report and production-smoke driver under
  E:/CodexTemp/TypingHero/pixel-hit-sparks for exact-HTML QA and pending integration.
  No secrets, QA video/capture/report or release ZIP committed to the repository.

### Release candidate ready; external integration blockers

- Production20261018T095953 PASS on39d1ff867e100a65ffafddc764561681ce8276372ac49b4c3e25d1302fbdcc44.
  MarkerQA=false; JS/WASM/data scans exclude typingHeroQA,TypingHeroQA_Publish,
  GameLoopProbe. Immutable result/player preserved before runner-lock release.
- Exact production HTML native smoke PASS: nameSparkQA, tutorialBLADE completion,
  score10; skip into real combat, OCR-read word `it`, keyboard hit and orange
  physical pixels visually checked; pause/resume,640x360 actual render, QAundefined,
  11mocked fixture reads/zero writes, zero JS/Unity exceptions. Two optional URP
  GPU-unsupported shaders were identified by full startup console; actual game/
  pixel material unaffected. No dependency/pipeline workaround. OCR JSON C0 escape
  fixed only in temporary QA driver. Persistent-context saveAs export failed after
  close; inspected screenshots are valid, no finalized-video claim. Explicit
  close/owned Node exit confirmed; shared BrowserSkill daemon kept as instructed.
- Candidate `/data/typinghero-game-loop/releases/pixel-sparks-20261018/` holds
  TypingHero-WebGL-PixelSparks-20261018.zip and release.json (code372b9f0).
  ZIP29members/16,311,433bytes; CRC and local/server SHA256 match:
  c58174cc5e84498e41151a3c006b853b528117a69dbff91a8c720a02c88eee08.
  Local candidate/QA helpers/player/reports remain under
  E:/CodexTemp/TypingHero/pixel-hit-sparks, needed for pending upload/integration.
- STILL NOT merged to main or published. Main610352b now has active HUD/scoring,
  SampleScene/prefab/QA tooling and design changes from other tasks, including
  overlap with feature scene/probe/browser files. Preserve their WIP, never merge
  over or release it without final checks. BrowserSkill doctor: IPC healthy, zero
  browser extensions connected; no authenticated upload possible. User asked to
  pause concurrent writes and connect existing logged-in browser. No pending own
  build/native job; only shared daemon remains. All meaningful independent work done.
- Resume: establish clean main (review/ch?t other completed work without losing
  WIP), merge any new main commits into feature, resolve/check affected flows,
  no-ff merge to main, integrated smoke, reuse candidate only if build inputs still
  match39d1ff... otherwise rebuild. Then upload/save/playable/maximized settings
  on existing itch game3857509, retain prior uploads hidden, verify public CDN
  against manifest without production writes, record receipt and clean owned
  child worktree/branch/temp. No GitHub push requested.

### Retry: completed HUD foundation integrated, 2026-10-18

- Retry verified parent build inputs equal the HUD task's final Quick53 hash
  6a6e68b810b1e7531268235820aff4f88a6e5364fe5532e8ebd75351114847e2.
  HUD/art receipts complete; reviewed and preserved in main47f1830. Removed only
  regenerated Python bytecode, no source/design output. Main tracked source clean.
- Merged47f1830 into feature ated785c5, no conflicts; scene ground references,
  particle observer and browser scenario guards retained beside latest HUD fields.
  Combined Quick **67/67 PASS**20261018T130858, input
  51e8c9ed54b26782faff346eb0b4f41cf3fcd864517fd646923c70c9d0d4198d.
  QA131049 and production queued on same input. QA uses task Library; production
  reuses the standard normal-build Library under the same exclusive runner lock,
  avoiding Development/normal cache thrashing. No resource/checker changes.
- Started shared BrowserSkill daemon, opened existing Chrome normally (the earlier
  no-startup-window attempt did not connect), browser9c92c27f/fly now connected.
  Owned sessionxkja reached itch login, not dashboard. One successfully registered
  human sign-in request timed out after5m; do not repeat it or read credentials.
  Re-observed and closed owned session per skill; preserve other sessionyzeq and
  shared daemon. Browser connection resolved; authenticated sign-in remains needed.
- Another HUD publication task is also active, with its own main-only source and
  sessionyzeq. Preserve its files/processes and wait for its publication to finish
  before uploading the final pixel build, to avoid the older upload replacing it.
  Main may receive its documentation receipt; review/preserve that before no-ff.

### Final combined acceptance before merge

- Combined Quick67/67 PASS130858; QA131049 build and repeat-builder PASS;
  production132059 PASS, QA=false and JS/WASM/data marker scans PASS. Build input
  51e8c9ed54b26782faff346eb0b4f41cf3fcd864517fd646923c70c9d0d4198d.
  Generated scene/player/pixel assets match local semantics/GUIDs (only Unity
  trailing whitespace differs). No game source/config/dependency changes afterward:
  git diff ed785c5 -- Assets Packages ProjectSettings is empty.
- Two initial native attempts failed ONLY the640lethal observation, while1280passed
  and late render mask135 confirmed a lethal root. Per-character observation did
  not solve it and was reverted. Source inspection identified mixed timing:
  Publish reads current Finished but cached sparks from previous LateUpdate. Driver
  now waits for an actual live Lethal particle sample, records that exact sample,
  and accepts the documented -1HP sentinel after enemy destruction. No mask-only
  feature acceptance or relaxed particle budget/plane/profile checks.
- Final integrated-presentation-native **24,212 checks PASS**,165.652s,1280x720 and
  640x360, all four live profiles, pause/resume, victories/results clear/restart,
  zero exceptions;4fixture reads/two mocked writes. Final driver SHA256
  7db2d84408087a33f1abd0124fbdf07650b210c31e28a81ec969bed694f033d2.
  Frame p50/p95:31.235/31.805ms and31.240/31.945ms; maxima232/220particles,
  active burst maxima8/7. Reviewed new HUD+pixel normal/parry/Slayer/lethal frames;
  text/cue/poses remain clear, no pink/halo. Prior two-direction bounce/playback
  evidence applies because the particle/physics source is unchanged.
- Production candidate releases/pixel-sparks-integrated-20261018:
  TypingHero-WebGL-PixelSparks-Integrated-20261018.zip,29members/16,311,465bytes,
  CRC PASS, SHA25691d422ce47437e225a4c186df4704c5b4223fb955a5f27fe9992723a5f5f8b4c.
  Local/server hashes match; safely extracted exact ZIP for original-HTML smoke.
- Existing Edge61a46c05 has authenticated micharel09 session. Owned BrowserSkill
  jjqx verified dashboard and /game/edit/3857509; no credentials were read. Chrome
  help timeout is superseded by this existing authenticated Edge session, without
  another login request. Editor's five existing uploads/metadata/embed settings
  recorded read-only before changes. No upload yet; main no-ff merge, post-merge
  smoke, production smoke and native upload/save/CDN check remain next.


## 2026-10-18 ? approved 2.5D lighting / Rising parry replacement

Supersedes four pixel impact profiles, flat colors and no-glow restrictions.
Only successful parry emits sparks. Normal/Slayer/lethal retain audio, slash,
hit flash, hitstop, camera and afterimages. Gameplay/public ParryContext unchanged.
Reference: https://www.platinumgames.com/official-blog/article/4717
Secondary textures: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/SecondaryTextures.html

- Reused Orca child feat/pixel-hit-sparks at
  C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks; clean before work,
  fast-forwarded from 92da0d2 to committed main 701830b. Parent remains dirty with
  independent Endless/HUD/QA work, untouched. No agents started.
- Milestone A in progress: explicit four-group ParryClashBurst, pooled eight-clash
  cap, one scene pulse light with bounded strongest pulse, post-environment lighting
  builder and Inspector-preserving asset creation. No emission on enable.
- Still required before A acceptance: compile/tests, normal maps with frame-aligned
  authored surfaces, fixed-foot shadows, bloom isolation and menu world backdrop,
  real keyboard QA/1280x720+640x360 clips (idle, two directions, slow motion,
  close-up, normal/Slayer), visual review and user art approval.
- B after art approval: complete surfaces/variants, lifecycle/repeated-builder,
  C performance comparison and all affected runtime flows. Merge/itch only after
  approval + verified integration, preserve main WIP and rollback build; no GitHub push.
- Existing pixel assets/licenses remain until reference audit. No visual acceptance,
  GPU performance, release or completed implementation claimed at this checkpoint.


### Prototype implementation checkpoint (awaiting new runtime checks)

- Added ParryClashBurst / ClashLightingRig / WorldMenuBackdrop, ParryClashAssets,
  SurfaceNormalAssets and WorldLightingBuild. Four particle groups emit only from
  successful parry after placement; unscaled simulation freezes on tutorial/pause,
  owner clear cancels pulse immediately, bounded strongest-pulse light, ember
  collision consumes particles. Built-in release still guards exactly-once return.
- Builder removes all three hit particle assignments and binds the new clash,
  then applies world materials/rig after ShogunEnvironment. Existing legacy assets
  retained for now; SFX/shake path remains outside optional particle spawn.
- Prototype normal maps use alpha/material-region bevels, never color luminance.
  These are draft surfaces for milestone A; they do not establish acceptance for
  authored anatomical/armor/roof forms or frame/mirror stability. No art claim yet.
- Added fixed-collider-foot shadows, thresholded world bloom and menu background
  world sprites mapped to existing RectTransforms. HUD remains overlay. No camera
  lens/zoom/gameplay values changed.
- First compile failed on read-only URP Light2D normal settings; fixed with the
  package's serialized m_NormalMapQuality/m_NormalMapDistance fields. Follow-up
  Quick67/67 passed input66209dc02ee35e160f3bfccb1f51e55c090bc71d084662e92db88444f1e8ff50.
  This predates new tests and subsequent prototype code; not final verification.
- New tests cover explicit emit/direction/depth, eight leases, unscaled/pause age
  and position, release/reuse, pulse overlap/cancel/hold and scene unload. Added
  parry-clash keyboard browser scenario with existing fixtures/network isolation,
  both render sizes, normal-no-burst / real parry / Slayer coverage and video.
- Shared runner was occupied by a separate WebGL job. Owned wrapper under
  E:/CodexTemp/TypingHero/parry-clash-lighting waits on its existing lock instead
  of failing busy; no other task process/container/source was interrupted.
  Queued checks use immutable invocation snapshots; rerun affected checks after
  the final prototype inputs are stable. No clip or milestone A completion yet.


- Final prototype Quick **73/73 PASS**, input
  6497f5556fb2d31df68cefca478f49488ef3fc09715094c1fcee32936c1da100.
  One earlier new test exposed default ParticleSystem duration5s holding a lease
  after all particles died. Set authored one-shot duration10ms, then verified
  release exactly once, clean reuse and all new lifecycle/pulse checks. Other
  queued intermediate snapshots are superseded; do not reuse their failure as
  final state. WebGL parry-clash + allocation build now queued on stable inputs.


- Correction: shared result143344-webgl hash0b72b046... belongs to another job,
  not this feature. Its repeat-builder log must not be used as lighting evidence.
  The pending parry-clash job179067 was verified by exact argv/no child process
  and stopped before lock acquisition; other tasks were preserved (a different
  score-layout PID was rejected by the ownership guard and not signalled).
- Own isolated source/output root now /data/typinghero-game-loop/task-parry-clash-lighting;
  reuse previously owned /data/typinghero-game-loop/task-pixel-sparks/library.
  143702-webgl inputaf7fab18a7d3bb936cf911f906c723174b82e79ec465a9edd475fcecfbc64a03
  failed before import: concurrent editor reported no active license. No activation
  changes made. Own isolated runner now takes the shared global lock, so Unity
  remains serial while source/output identities stay separate. Latest WebGL pending.
  af7fab... differs from Quick6497... only by removal of generated Python bytecode;
  game/editor/test source is unchanged. No builder acceptance is claimed yet.


### Builder verification in progress

- Own143702/144003 license failures were caused by the temporary isolated wrapper
  mounting an empty task HOME; fixed the mount to the standard activated HOME.
  This was not evidence of a concurrent-license limitation. Kept shared lock.
- Own144042 was stopped after no stage progress; temporary diagnostic markers
  live only in snapshot builder code. Own144513 identified editor fake-null
  component handling in ContactShadow (also fixed menu component creation), and
  ambient registration while the legacy global was active. Moved rig creation
  after legacy-light disable. Superseded144627 build was stopped before IL2CPP
  completion, after verifying its exact owned container/results mount.
- Own **144811-webgl**, diagnostic input
  94ac9c144bf1045008b98934a6b69c52ae256a59eaece1b18796f6df3e04c263:
  repeated builder PASS, scene/prefab/material/normal/lighting data unchanged on
  second apply; no duplicate-global or MissingComponent errors. IL2CPP compiling.
  Not a completed WebGL/runtime/art acceptance result yet.
- Runner contracts8/8 and JS syntax PASS. Keyboard scenario now reuses the
  existing verified attack-time calculation to press Space before impact;
  player parryWindow is an input-owned state, not an enemy attack cue.
- Normal draft still needs visual/frame/mirror review, especially the large
  death atlas and anatomical/armor forms. Full authoring is milestone B.


### Milestone A ? review clips ready; stop at approved art gate

- Normal QA144811 build/repeated-builder PASS; software art scenario failed on a
  missed defensive input, not an exception. Its source/output identities were
  verified before retaining the player after the parry-clash retention guard
  was added to the existing runner. The own isolated source was not replaced
  between that build and retention; snapshot/marker/no active mount verified.
- Native first attempt failed because the initial idle/normal case had already
  taken a hit. Harness now uses real Escape/R restart between normal and parry
  cases and checks clean40/850HP/score0/no sparks before the latter. No game change.
- Forward native **1767checks PASS**,70.047s; mirrored **2223checks PASS**,67.548s,
 1280x720 and640x360 each. Actual GPU Intel UHD/Edge155/D3D11, read-only bridge,
  isolated fixture routes/external blocking, zero JS/Unity exceptions or real
  provider writes. Mirrored actual particles have negative mean X force at both
  sizes; the fixture changes only actor positions/scales in its disposable build.
- After importing69 Unity-serialized files, every retained asset GUID matched.
  All16 changed original PNG metadata files differ ONLY by secondary _NormalMap
  bindings; sprite IDs/rects/pivots/PPU/import settings unchanged. New text lines
  normalized with original matching-line whitespace preserved; semantics intact.
- Final Quick **73/73 PASS**, own20261018T145948-quick, input
  c352ac1237c8e406dbd994289484254555e1d6dce8b2e983239258ad1753d183.
  This includes the imported real prefab/material data. Subsequent edits only
  normalize new-line whitespace or update documentation. Runner8/8 and JS syntax
  checks PASS. Final source/diff/GUID review complete; no diagnostic/mirror fixture
  source included in product checkout. No owned active server containers remain.
- Reviewed full frames and contact-frame sequences: idle/menu, two clash directions,
  both render sizes, visible white-gold pulse on actors, thin directed rays,
  retained slash/Slayer silhouette and readable HUD/cue. No pink material. Draft
  normal surfaces still need authoring/review in B; not final lighting acceptance.
- Review artifacts (silent actual25fps footage,1x and0.25x without interpolation):
  E:/CodexTemp/TypingHero/parry-clash-lighting/ParryClash-Lighting-1280.mp4
  (12s,1838287bytes,SHA256563b5ccb642844ad37dd38e8c40611275a2d1759b0f644858cba01df9e47d85e)
  and ParryClash-Lighting-640.mp4
  (1394364bytes,SHA256f44eec2c50b16808f3c48b6d88b77fb79d23fe7cab80edce6e0bc9119726f113).
  640render is nearest-neighbor enlarged in the review export; product render sizes
  were verified directly. QA driver SHA256
  d95d8e478e9481ce0100ebf50b693546ecf77b1d3e282bd2c056015cf312c6c0.
- Keep child feat/pixel-hit-sparks, product changes and necessary review/player/
  receipts/helpers under E:/CodexTemp/TypingHero/parry-clash-lighting and
  /data/typinghero-game-loop/task-parry-clash-lighting for user review/continuation.
  Remove redundant failed captures/browser profiles/intermediate review images;
  retain requested clips until review/release is complete, then clean task resources
  after written receipts and verified integration/publication per the approved plan.
- Next depends on art approval: B authored normals/all animation/scene variants,
  complete Inspector tuning and lifecycle/transition/restart/tutorial integration;
  C same-device baseline/p95/allocation tests and full affected runtime acceptance.
  Then clean-main reconciliation/no-ff merge/integrated smoke/production QA exclusion/
  existing itch3857509 rollback-safe upload/public verification. No GitHub push.
  Do not include parent Endless/HUD/QA WIP or treat this prototype as release-ready.


- Cleanup verified: removed owned failed local capture/profile directory, completed
  browser profiles, duplicate transfer archives and intermediate review stills;
  removed isolated failed build results143702/144003/144042/144513/144627 and
  redundant server review/transfer files after written receipts. Preserved requested
  clips, passing native raw evidence/reports, normal/mirror immutable players,
  final Quick/build results and required helpers/cache for art feedback/B continuation.
  Child branch/worktree remains intentionally; clean it only after final verified
  integration/publication. Parent and all other task resources remain untouched.

## Pixel parry ray revision (2026-10-18, implementation in progress)

- Approved intent: pixel-edged main/secondary rays, soft contact retained, small hot
  heads only. Keep world lighting/normals/camera/gameplay and all other hits unchanged.
- Reuse child feat/pixel-hit-sparks and existing isolated runner/native fixtures;
  no additional agent, merge or publication. Parent WIP preserved.
- Five systems: contact2, main12 (.125WU), front28 (.0625WU), back16 (.03125WU),
  ember8. Total66; same lifetime/force/depth/lease/pulse paths. Layer count/width/
  lifetime/brightness editable in Inspector. One-time in-place prefab migration
  detects assigned main system; subsequent builder runs retain tuning and GUIDs.
- Implemented local-UV stepped ray mask, distinct contact/ray/ember materials and
  width hold through65% lifetime followed by one step. Pending actual compilation,
  migration serialization, repeated-builder/tuning tests, Quick, forward/mirrored
  keyboard QA at1280x720/640x360, UV-head playback and same-device performance.
- Keep old clips/raw baseline and task helpers at
  E:/CodexTemp/TypingHero/parry-clash-lighting for before/after art review.

- First revision Quick73 passed; QA155431 build/repeat-builder passed. Software
  browser missed defensive input (80 checks, damage), not a product exception.
  Native screenshot showed wide/hot sections near the origin with thin points
  outward: initial assumed UV0 head was reversed. Flip local longitudinal UV,
  increase cross-mask raster to8 bins so tail actually narrows below the body.
  Rebuild and verify playback both directions; first build is superseded for art.

### Pixel-ray revision verification ? art review ready

- Implemented five systems/66particles, role materials/pixel UV mask, width hold,
  per-layer Inspector settings and one-time in-place migration. No gameplay API,
  lighting strength, normal map, animation or camera change. Existing22prefab IDs
  and GUIDs retained; new4IDs represent exactly one child system.
- Quick74/74 PASS160100; normal QA155853/repeat-builder PASS, same final input
  50d2b168619477d87596fd32ffcc5b441b7a5d763c612354e30d3aea5cfa46bd.
  Mirror160147/repeat-builder PASS654d8f6e25ed5cef6eb7976e1214619ec75f457f8708b12a53805826d75ababa,
  disposable mirror fixture only. Native normal2304/mirror2330checks PASS, both
  sizes,0exceptions/writes. Software drivers missed parry (120/62checks), so no
  full software profile PASS claimed. See verification memory for measurements.
- Fresh baseline2134checks PASS; p95 normal1280 33.280?33.465ms and640
  33.585?33.380ms. Median allocation582B/frame unchanged; objects227?229 from
  added prefab/live child. Budget remains66/clash/eight leases/one pulse light.
  This is the requested ray-revision comparison, not full milestone C acceptance.
- Actual25fps before/after comparisons exported at both native render sizes,
  both directions,1x and0.25x close-up with nearest-neighbor scaling, no
  interpolation. Reviewed contact sequences: leading hot head stays forward
  in both directions; main rays read beside slash at640, no pink/wide halo.
- Retain child and E:/CodexTemp/TypingHero/parry-clash-lighting review clips,
  baseline/current immutable players, source-before migration and native receipts
  for art feedback. Old ParryClash-Lighting clips remain byte-identical. No merge,
  release or itch upload. Next: user art review, then approved B/C work.
- Additional same-player keyboard live-clash pause/restart PASS313checks/20.937s,
  both sizes: counts/age/world bounds hold, restart clears roots, new parry works;
  no exceptions/writes. First supplemental wrapper used a non-normalized Windows
  require.cache key and reran the ordinary art scenario; corrected via require.resolve
  before the actual313-check lifecycle run. No product change was needed.

- Same immutable normal player: supplemental real-keyboard live-clash pause120ms,
  held particle count/age/world bounds, restart removes roots and new parry works,
  both1280x720/640x360:313checks PASS20.937s,0exceptions/provider writes.
- Input identity reconciliation PASS: raw checkout build inputs
  46f55c0090cfc4411cc8e8b49269d562afe9b4472fa7bb6b7a9cf96e6da461be;
  existing isolated wrapper adds only editor stage-log statements, yielding the
  verified50d2b168... Quick/normal marker. Adding only the disposable mirror editor
  fixture to raw inputs yields654d8f6e..., matching mirror marker exactly. No
  diagnostic or mirror fixture was added to product source.
- Both side-by-side exports decode cleanly,25fps/7.040s/2560x720; each half shows
  the labeled native render size enlarged with nearest-neighbor when needed.
  1280 clip1941603bytes SHA256204f58e2f5f27c77ce4f06ceb16394c07acda4da7db078c7052fce2c774703b2;
  640 clip1599056bytes SHA25698e6b5d9b9b738b91600dcfd21863ac4fdaa68ccce70ac0f311199eaffa71995.
  Local/server hashes match. Prior two review MP4 hashes remain unchanged.

- Cleanup verified after receipts: removed27owned local superseded captures/player,
  profiles/downloads/traces and transfer/archive targets; removed17owned server
  superseded result/archive/intermediate targets. No owned active server container.
  Retain final normal/mirror/restart native receipts/raw videos, baseline repeat,
  old+new review MP4s, old/current immutable players and migration-before bytes
  under E:/CodexTemp/TypingHero/parry-clash-lighting for pending art review/B/C.
  Server final Quick160100, normal155853 and mirror160147 results/players plus
  established source/cache/helpers remain reusable. Clean after approved verified
  integration/publication; parent WIP and original clips/players were preserved.
- Final scoped source/asset/GUID/whitespace and JS syntax review PASS. Raw/diagnostic/
  mirror input identity reconciliation matches all retained final result markers.
  Art review is the only next gate for this revision; do not start B/C or merge.

## Reviewed prototype publication (2026-10-19)

- User explicitly requested itch.io deployment after the pixel-ray review sample.
  Publish exactly the verified child snapshot, exclude parent Endless/HUD/provider
  WIP; do not broaden into B/C, normal authoring or main reconciliation.
- Production runner queued on prior verified50d2b168... diagnostic input; raw child
  remains46f55c00... . Existing Quick74/native normal+mirror/restart receipts reused
  because product source/assets/settings/dependencies are unchanged.
- Target remains game3857509 at https://micharel09.itch.io/typing-hero. Own Orca
  manager page cba58fb8-bbb7-446d-a006-bcc0609610d3 is Cloudflare-blocked; user was
  asked asynchronously to complete it. No upload/storefront change yet. Continue
  production/QA-exclusion/package/native smoke while external access is pending.

## Parry pixel prototype publication (2026-10-19)

- User explicitly requested publishing the reviewed ray revision. Shipped the
  verified child prototype; B/C normal authoring and parent integration remain
  separate. Parent Endless/HUD/provider WIP was excluded, no merge/push.
- Production20261019T015346 PASS, same verified diagnostic input
  50d2b168619477d87596fd32ffcc5b441b7a5d763c612354e30d3aea5cfa46bd;
  raw child46f55c0090cfc4411cc8e8b49269d562afe9b4472fa7bb6b7a9cf96e6da461be.
  All JS/WASM/data lack typingHeroQA/TypingHeroQA_Publish/GameLoopProbe.
- ZIP29members/16,398,742bytes, local/server SHA256 and member CRC PASS:
  06415cfd0d002d60e526d994124ac35ea83388a822bfdac03458d39ebde23f6a.
- Exact production HTML, native Edge155/Intel UHD D3D11: real name/Enter,
  BLADE score10/ATTACK COMPLETE, accuracy2/2, cue-timed parry2/2/COMPLETE,
  visible clash material/light, pause and native Resume after640x360 resize.
  Independently inspected resumed640 frame. QA undefined,0JS exceptions/provider
  writes. Initial blind Space/full-frame delayed cue attempts missed the lesson;
  final cropped visual-cue capture pressed immediately and succeeded. No product
  timing/threshold/input or test expectation changed.
- Native helper corrections: first HTTP load preceded completed ZIP extraction
  and got404; rerun only after hash/CRC/extraction passed. A screenshot/OCR helper
  returned500 after two successful parry inputs; a fresh actual-canvas OCR read
  and images confirmed2/2 COMPLETE. Initial Escape after resize did not resume;
  corrected QA uses the actual Resume button and asserts overlay disappearance.
  Existing video.path-before-close helper replaced a hanging saveAs-after-close.
  These were harness issues; final independent behavior checks pass.
- Initial Orca manager hit Cloudflare. Restored existing BrowserSkill daemon with
  original OS user/default home, reused verified Edge61a46c05 login, own Agent
  Window only. No credentials/cookies exported or browser settings changed.
  Idle session expired before upload; re-bound Edge and verified account, same
  seven old uploads and unchanged settings before publishing.
- Saved new playable upload19865461 to https://micharel09.itch.io/typing-hero,
  game3857509. Seven prior uploads retained hidden, including previous playable
  19847423. Storefront/embed/pricing/gallery settings hash unchanged:
  1fabaea4decac5ab34647f4249f96700ea29cbd34bd6a7b6a561b844062d445f.
- Public page reports the new ZIP and Run game. Anonymous CDN28non-HTML hashes
  match release; HTML differs only by itch htmlgame.js integration. New iframe:
  https://html-classic.itch.zone/html/19865461/index.html?v=1792376251.
  Same hosted payload menu visually verified1280x720 after Unity splash, QA absent,
  external/provider routes blocked/fixtured,0exceptions/writes. No public gameplay
  or real leaderboard writes. Earlier one-second hosted capture was splash only;
  discarded after waiting past the splash and checking the actual menu.
- Release ZIP/release.json/published.json retained for rollback under
  /data/typinghero-game-loop/releases/parry-pixel-20261019/. Prior release retained.
  Child feat/pixel-hit-sparks and pre-existing review artifacts remain for B/C and
  later verified parent integration; remove child only after that work is complete.

- Publication cleanup verified: stopped the owned BrowserSkill session and closed
  only this task's Orca tab;0local QA HTTP listeners and0owned server containers.
  Removed18owned local release QA/player/profile/capture/helper/archive targets
  and3owned server production build/temp/result targets after written receipts.
  Actual release ZIP/manifest/publication receipt remain in the release directory;
  existing review clips, QA baseline/current players, source/cache/toolchain and
  child remain for unfinished B/C and parent integration. Shared BrowserSkill
  daemon remains available with its original configuration; no user/other-task
  browser tabs or parent WIP were changed. Final source identity/diff check PASS.


## Thicker varied rays / camera fragments (2026-10-19)

- Approved continuation: same child `feat/pixel-hit-sparks`, parent
  `C:/Users/ADMIN/orca/TypingHero`; Orca id
  `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks`.
  Parent WIP preserved. Art-review sample only; no merge/push/itch update.
- Implemented Custom1 mask/body ratio/head pixels/visible length, 50/30/20
  stable silhouettes, stratified fan ?6deg/90?110%/energy ?8%, length ?15%.
  Main head/body/tail 3/2/1 source pixels (16PPU); front head/body .09375/.0625WU.
  Three existing main particles become near-camera fragments: 4/3 pixel head/body,
  shortened .55?.70 multiplier and negative Z4?6WU/s. Back Z2?4WU/s.
- Versioned in-place migration, UV+UV2+Custom1 streams, no new materials/systems;
  budgets remain five systems/66particles/eight leases/one light. Contact, bloom,
  lighting, camera, normals, gameplay API and lifecycle retained.
- Quick20261019T024026 **75/75 PASS**, final diagnostic input
  `ed929399dd2986bee5ac5fcb1af8d17401ae7988153cc0e4ba5ca4c1d9eb9d24`.
  Includes 32 seeded mirrored bursts, stratified angle/energy/length bounds,
  exactly three camera fragments/back depth, custom data stable during simulation,
  reuse, forced migration and two reruns preserving Inspector tuning/references.
- QA20261019T024122 queued/running on same input. Pending serialized asset copy,
  repeated-builder outcome, actual keyboard ten clashes/direction at640x360 and
  1280x720, 1x/0.25x art review, pause/restart and same-device baseline p95.
  Existing baseline players/clips and retained helpers remain under
  `E:/CodexTemp/TypingHero/parry-clash-lighting`; keep for requested comparison.

### Thicker-ray sample verification complete; art review pending

- Final lifetime/compaction Quick02552475/75PASS; normal/mirror build and repeated
  builder PASS. Software parry drivers each fail94checks from timed-input HP loss;
  no software profile PASS claimed. Exact native players pass2197normal/2412mirror
  checks, ten clashes/direction/size at1280x720 and640x360; repeat2369 and real live
  pause/restart256checks also PASS,0exceptions/provider writes. See verification
  memory for source hashes, measurements, limits and export receipts.
- Migration retained26object IDs, every ordered GUID reference and identical meta.
  Copied only generated ParryClash.prefab back; semantic match after empty-field
  whitespace cleanup. No other generated scene, camera, normal or prefab copied.
- p95 repeat versus fresh same-device baseline: normal+1.8%/+0.5%, mirror+1.2%/-1.3%.
  First normal1280+17.7% did not repeat; investigation found idle samples elevated
  too. Median allocation582B/frame unchanged. No performance tuning outside scope.
- Review sample: `E:/CodexTemp/TypingHero/parry-clash-lighting/ParrySpark-Thicker-BeforeAfter-640.mp4`
  and `ParrySpark-Thicker-BeforeAfter-1280.mp4`, both28.8s/25fps; each includes both
  directions at1x and0.25x. Direct PNG/contact/comparison frame review confirms
  readable body, forward hot head and short brighter near fragments; human art
  approval still pending. Same camera/state-driven input scenario, RNG not locked.
- Retain existing child (unintegrated foundation/B/C), old clips/player and current
  normal/mirror player/native receipts/review MP4s until art approval and later
  verified integration. No production build/release/itch change for this revision.

- Final source identity reconciliation PASS: raw input `5ab127945686531a133dd13468e57daae9bee628a98cf71defeb2d4635af341c`;
  restoring only pre-migration prefab bytes and diagnostic stage logging reproduces
  final Quick e216a12e... exactly. Generated prefab semantic/ID/meta comparison PASS.

- Cleanup verified: removed own superseded7-clash baseline run, local/player
  transfer archives and inspected image/prefab intermediates; removed server
  export/upload intermediates, transfer archives and two superseded Quick results.
  Final Quick025524, normal024122/mirror025210 immutable results/players and local
  ten-clash native receipts/raw videos/new review MP4s remain for pending art review.
  Old two review MP4 hashes remain byte-identical. No owned active browser/HTTP
  listener or server container remains; other tasks/services/cache preserved.
  Current sample is ready for user art review; approval/publication not inferred.


## Three spark variants review (2026-10-19, delivered for art review)

- Continue the existing Orca child `feat/pixel-hit-sparks`, full identity and parent
  as recorded above; parent WIP preserved. No publication or integration authorized.
- Current remains default. Inspector selects Current/A/B/C on the emitting owner;
  automation-only F6 cycles that selection and labels it. Selection applies to the
  next emission. Per-preset sequences live on the owner across eight pooled leases.
- A alternates narrow/wide fans; B alternates dominant offset clusters; C rotates
  three long rays among nine ordinary main rays. All retain three near-camera
  fragments, five systems, 66 particles, eight leases and the single pulse light.
- Independent length-support migration expands renderer/shader support to 130%,
  preserving the previous rendered length and Inspector tuning after migration.
- First existing-suite Quick 20261019T031342:75/75 PASS. Expanded preset tests and
  final observer snapshot are running separately; no new browser/art/performance
  acceptance claimed yet.
- Reuse `E:/CodexTemp/TypingHero/parry-clash-lighting` and isolated server task root
  `/data/typinghero-game-loop/task-parry-clash-lighting`. Keep old review clips and
  players intact. New task helpers:variants_clash.cjs,variants_runner.cjs,
  run_variants.ps1. Retain until requested comparison/interactive QA is delivered;
  remove task artifacts after art review and verified integration.


### Three-variant acceptance receipts (2026-10-19)

- Final Quick20261019T032249: **76/76 PASS**, snapshot
  `8ac5244b36c022e077329936b6fa8487847271e248a6972bdde68a90145f6ab7`.
  Includes72seeded A/B/C bursts mirrored with stable Custom1 throughout lifetime,
  and owner-local C sequence assertions across12emissions/eight recycled leases.
  Existing lifecycle/light/budget and two-run Inspector-preserving migration pass.
- Normal QA031618 snapshot04df09be928225ace2fad8f88ee39e50e7ef20b77620a95f4b8091238a0b1ecb;
  mirror QA032014 snapshot6a196212a5e791a15f45bbcd23459a81d33585938c0272b3e6c6f70e1d10ec17.
  Build/repeated-builder pass. Normal native **9068checks PASS /349.646s**;
  mirror **10501checks PASS**. Each has10real keyboard clashes/preset/direction/size:
  160total across Current/A/B/C and1280x720/640x360. Normal and Slayer hits retain
  no burst. Both reports have0exceptions/0provider writes, fixture/blocking retained.
- Native F6/lifecycle **1718checks PASS** across all4presets/both sizes: F6 while
  paused preserves each existing lease's emitted preset/age/position, the next
  clash uses the newly selected preset; restart clears roots and new parry works.
- Windows Edge155.0.4283.62/Playwright1.58.2; actual native renderer is NVIDIA
  GeForce GTX1650Ti/D3D11. First software browser profiles failed114/129checks
  from missed parry/HP loss; no software profile PASS claimed. Initial native
  headless acceptance timed out after27checks; final headed runs above pass.
  Port8123 was occupied; task harness uses8137, leaving its owner untouched.
- Initial headed p95 comparisons triggered investigation (>10% in several rows).
  C1280's27samples above50ms all occurred without live particles. A diagnostic
  repeat had Current p95 **1006.005ms**, so it is invalid as a comparative baseline.
  Earlier diagnostic harnesses also hit video.path-after-close hanging and browser
  shutdown between contexts; final benchmark uses one tab, no recording/screenshots,
  real canvas click/focus, visibility observation and disabled background throttling.
  A later headed diagnostic missed its fourth C640parry; it is not a profile PASS.
- Controlled native headless normal benchmark **3789checks PASS**; all8rows record
  visible/focused tab, four real keyboard clashes each. Overall p95 (1280/640ms):
  Current18.685/14.230; A13.705/14.335; B13.500/14.945; C14.115/14.525.
  Largest positive change+5.03%. These are short sampled QA frame times, not GPU
  timings or a device-FPS guarantee; smaller1280numbers are not an optimization claim.
  Mirror controlled repeat2689checks PASS: Current30.105/27.280,
  A21.640/31.715,B20.265/21.365,C28.580/26.970ms. Mirror A640+16.26% triggered
  a separate order-balanced Current/A/A/Current repeat:1862checks PASS,8clashes
  per preset, Current208samples p9515.160ms versus A207samples14.645ms(-3.40%).
  Live-particle p95 Current15.845/A14.030ms; idle14.845/14.385ms. It did not repeat.
  Active-only samples are sparse: normal C64021.370ms versus Current15.035ms
  came from13live samples each; mirror C64017.910ms versus24.010ms did not repeat
  that increase. No stable >10% regression established; this is not proof of
  uniformly flat frame time or an absence of spikes. No product performance tuning.
- Normal build032354 PASS on final Quick snapshot; qa=false and all JS/WASM/data
  exclude the bridge markers and F6 label (UTF8/UTF16 scan). No ZIP/upload/itch change.
  All6changed C# / shader inputs match that verified server source byte-for-byte.
  Generated prefab normalized SHA256
  `9a7d954cc829d892a2f7838b2a8017b4b6997ab028b46d31328112080dd719c1`;
  all26object IDs/ordered GUID references and prefab meta preserved. Local prefab
  matches normal-build generated serialization after empty-field whitespace cleanup.
- Final review MP4s: ParrySpark-ABC-640.mp4 (9329102bytes, SHA256
  583b059723db4705cdd2c268c7a9bc610a3df834ee17a0d0d63e3e7b6e752b19)
  and ParrySpark-ABC-1280.mp4 (22785078bytes, SHA256
  c88e86d247e00c18a1109645d6b8a78789108acac1d8c5612b0bb7ae306e04b4).
  Both44.8s/25fps,20s1x+2.4s0.25x per direction; four native-size panes make output
  1280x720/2560x1440 respectively. Full decode and local/server hashes PASS.
  Initial close-up picked the cue in A/B and was rejected; final ROI detects
  the actual sword contact. Body frames at640normal/1280mirror inspected: B offset
  clusters, C short/long split, A fan and near fragments remain readable. Different
  RNG contacts, not a deterministic replay. Art choice remains with the user.


### Interactive QA and retention

- Edge QA is open at1280x720, paused, Current selected; actual ready frame inspected.
  Click Resume then use F6:Current -> A -> B -> C -> Current. F6 changes the next
  clash only. Inspector selection is on ParrySparkVFX_Pooled; runtime default remains
  Current. No changes to itch.io, parent WIP, animation/camera/normal/contact/bloom.
- QA loopback host8138: PowerShell PID4488, Node PID24816. Own fresh Edge profile
  and unchanged Firebase interception/external blocking; bridge observation-only.
  Closing the actual game window resolves the helper and closes its host/browser.
- Reopen after closing that window, from PowerShell:
  `& 'E:/CodexTemp/TypingHero/parry-clash-lighting/open_variants.ps1' -Name variants-interactive -Player 'E:/CodexTemp/TypingHero/parry-clash-lighting/variants-normal-player'`
  Keep the named result directory's snapshot.json; the immutable player, wrapper,
  existing Playwright path and child checkout are dependencies of this command.
- Retain local final normal/mirror immutable players, passed native/lifecycle/
  benchmark reports, raw review recordings and both ParrySpark-ABC MP4s under
  E:/CodexTemp/TypingHero/parry-clash-lighting for requested art review/re-export.
  Keep active interactive profile/log/ready frame while the user is trying it.
  Existing earlier clips/players and child remain untouched. Cleanup is due after
  art choice and verified integration; child removal also requires the earlier
  unintegrated foundation/B/C work to be resolved and authorization for integration.
- Discarded diagnostic folders/transfer archives/intermediate frame exports may be
  removed after the written receipts above; no diagnostic failure is a profile PASS.
  Shared source/Library/toolchain and reusable build output remain on Ubuntu.

- Cleanup verified after recording receipts:18owned local diagnostic/transfer/profile
  targets and31server transfer/segment/frame/superseded Quick targets removed.
  Only interactive Node24816/PowerShell4488 remains for this task; no benchmark
  host remains. Final players, passed reports, raw review videos and MP4s retained;
  all earlier review artifacts and parent checkout preserved.


## BCompact implementation (2026-10-19, ready for art review)

- User-approved plan: retain B as comparison; append BCompact enum value4.
  Current remains normal default; no itch publication/integration requested.
- Reuse child e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks,
  parent e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/TypingHero,
  branch feat/pixel-hit-sparks, base main701830b. Existing dirty foundation,
  parent WIP and all prior review players/clips are preserved.
- Compact emits contact2/main6/Front0/Back0/embers2; main contains four
  3/2/1pixel rays and two4/3/1pixel camera fragments. Integer local dimensions
  use16PPU, billboard rotation leads XY velocity, no velocity stretch/lifetime
  shrink. Two lobes -16/+45deg, paired -6/+6deg with2deg jitter; dominant
  12-16pixel lobe alternates against6-9pixels via owner sequence. Near4-6pixels,
  Z toward camera; lifetimes .14-.22/.12-.18. World movement stays continuous.
- Per-lease renderer/material/module restoration follows clear; independent
  compact migration v1 adds a separate pixel material and preserves old material
  tuning. F6 now cycles five presets, with unchanged next-emission semantics.
- Initial Quick20261019T042847 passed77/77 on188231b6703d105a636447c367a3bd6b5af64ab461dbc54c32c1c0cae45f3388.
  Expanded compact Inspector/80particle/lifecycle assertions are included in
  the final full Quick below.
- Task helpers/players/reports live under E:/CodexTemp/TypingHero/parry-clash-lighting;
  isolated server root /data/typinghero-game-loop/task-parry-clash-lighting.
  First Quick used the shared runner lock/source/cache; subsequent build uses
  existing isolated task source/cache. Final runtime/browser/art/performance receipts
  for this revision follow below. Retain resources for review and later
  verified integration; do not clean previous delivered artifacts.


### BCompact final acceptance and delivery receipts

- Final Quick050959:77/77 PASS, c40ca3d7188dacc63a7718d61629946d4725de2506db391d1638ae9b10a8538c.
  Previous050224 was76/77 because the compact lifecycle test still read empty
  Front; fixed to MainRays. Reverting that single test selector reproduces the
  normal QA input3da8b6d3b176dcc40b2e4799efb77715cf2db27267830cf82056a431988961d8
  exactly (both local/remote digest checks). Runtime/editor/shader/material inputs
  are identical; final test-only correction needs no player rebuild.
- Normal044126/mirror044505 build/repeated-builder PASS; mirror input
  f5f6c1a4ce5b886ca226890aef6035a575f9d51a3631803f8844d519a13c053d adds only
  the existing disposable actor-mirroring build fixture. Both immutable players
  are retained under compact-normal-player and compact-mirror-player.
- Normal native3424checks PASS/189.047s, each B/BCompact10clashes per size.
  Mirror qualified rows: compact-native-mirror-r2 B1280/64010each (overall failed
  later at compact1280); compact-native-mirror-compact-r2 compact1280=10 (overall
  failed later at640); compact-native-mirror-640-r2 PASS806/51.720s, compact640=10.
  Those reports are partial/full exactly as stated;80qualified art clashes total.
  Each selected row passed ordinary/Slayer particle exclusion checks; no exceptions
  or provider writes. Fixture/blocking retained, bridge observation-only.
- Lifecycle qualified rows: compact-lifecycle-r5 B1280/640 (overall373checks,
  later failed on a missed parry); compact-lifecycle-compact compact1280 (overall
 205checks, later paused640after its short particles had expired); compact-lifecycle-
  compact-640 PASS170/19.514s. Both preset directions/sizes preserve paused
  particle preset/count/age/Y bounds; next clash uses the new choice and restart
  clears roots/restores Current. Real canvas focus after resume and5ms transient
  observation resolve setup/short-lifetime capture; product timing unchanged.
- Controlled performance compact-perf-normal1506/compact-perf-mirror1569 PASS:
  four keyboard parries per row, one focused/visible tab, no video/screenshots.
  B/compact p95 ms1280/640: normal17.710/18.670 and15.360/14.575;
  mirror17.975/15.550 and16.685/14.735. Sample counts97/97,99/97,100/100,98/98.
  Largest positive+5.42%; live-only maximum+3.51% with only5-13samples per row.
  No >10% positive result triggered ABBA. These short QA-observer delta samples
  are not GPU timings, device FPS or a no-spike guarantee; no stress/soak.
- Windows Edge155.0.4283.62/Playwright1.58.2, actual NVIDIA GTX1650Ti/ANGLE D3D11.
  Ubuntu Unity2022.3.25f1 pinned digest,2CPU/8GiB. Software browser timing misses
 112/128/143checks are not PASS. Native timing/setup retries also remain failed.
  Earlier helper preflight flags/scenario/SSH EOF ran0tests; final Quick uses
  an archive file as stdin. Only verified own stalled wrapper was terminated;
  other jobs/services/runner cache were preserved. Browser/video finalization
  and a busy owned8137listener were cleaned only by verified task process/profile.
- Two final review MP4s retained in E:/CodexTemp/TypingHero/parry-clash-lighting:
  ParrySpark-BCompact-640.mp4:4,218,450bytes,
  SHA25665aaa80afeb0a16805170d48536106b46a25b60d8abc0928007c5bffe79897cb.
  ParrySpark-BCompact-1280.mp4:9,927,814bytes,
  SHA256125ecb3364c2ac1fe5a55c38defff8378c09d08fa1df708d046ba1835537270d.
  Both44.8s/25fps/1120frames, B66versus compact10, both directions,1x plus0.25x
  nearest-neighbor close-ups. Output1280x360/2560x720 keeps each pane at native
 640x360/1280x720. Full decode/hash PASS; all four delivery close-ups inspected.
  Golden-pixel ROI excludes the white slash/cue when locating contacts. Individual
 640compact17.88/.92/.96s frames show clear lobes/leading hot head and held block
  fading, with less dust. Full motion is not deterministic RNG or human art approval.
- Current stays normal default; no production build, upload, itch change, commit,
  merge or push. Child and parent WIP/old review players/clips remain intact.
  Keep this child until all earlier foundation/A/B/C plus compact changes are
  reviewed and explicitly integrated/verified on the intended target branch.


### BCompact interactive QA and retention

- Separate Edge QA is open at1280x720, paused, BCompact selected, score0 and clean
 40/850HP reset. Actual interactive-ready.png inspected. Click Resume to play;
  F6 cycles Current -> A -> B -> C -> BCompact -> Current, next emission only.
  Normal serialized default remains Current. This is a local QA host, not itch.
- Owned loopback host127.0.0.1:8139, PowerShell20368/Node21328. Fresh profile,
  Firebase interception/external blocking and read-only bridge are unchanged.
  Leave this window/profile/host alive for user review; close only these owned
  processes after review, checking current PID/command/profile ownership first.
- Reopen after the existing window/host has closed, from PowerShell:
  `& 'E:/CodexTemp/TypingHero/parry-clash-lighting/open_compact.ps1' -Name compact-interactive -Player 'E:/CodexTemp/TypingHero/parry-clash-lighting/compact-normal-player'`
  Player, adjacent compact-normal/snapshot.json, wrappers and existing Playwright
  path are dependencies. The script resets via real UI, pauses and selects compact.
- Retain E:/CodexTemp/TypingHero/parry-clash-lighting: two final BCompact MP4s,
  compact-normal/mirror-player plus snapshot folders; qualified native/lifecycle/
  performance reports and raw recordings, compact-frames, final helpers,
  interactive profile/log/ready image; compact-original.prefab/identity and final
  compact-quick-input.tar.gz for recovery/reproduction of uncommitted source.
  Preserve all pre-existing Current/A/B/C/earlier clips/players and parent WIP.
- Server retention: final results044126-webgl/044505-webgl/050959-quick and
  compact-review raw WebMs/export script/delivery manifest under the existing
  isolated task root. Source/library/toolchain/private Unity HOME/cache stay.
  Remove these task review resources after art review and verified integration;
  child cleanup additionally requires the earlier foundation/A/B/C work resolved.
- Cleanup targets now authorized/verified as this revision's disposable diagnostics:
  failed unqualified compact native/lifecycle profiles, duplicate transfer archives,
  generated-prefab duplicate, initial043006warmup player, initial shared042847Quick,
  failed050224Quick and encoded segment/preview intermediates. Written failure,
  hash/identity/measurement receipts above supersede them. Qualified partial
  reports are retained with their actual failed status, not relabeled or erased.

- Cleanup verified:16own local diagnostic/duplicate targets and37server obsolete/
  transfer/segment/preview targets removed after the receipts above; path boundaries,
  links and source snapshot ownership checked. Only compact interactive Node21328/
  PowerShell20368/Edge profile remains;8137has no listener and0noninteractive compact
  test Nodes remain. Final MP4s/players/qualified partial reports/frames/recovery
  archive are retained intentionally for art review. Four prior ABC/thicker review
  MP4 hashes still match their earlier receipts. Shared tooling/source/cache,
  unrelated jobs and all earlier review artifacts remain intact.
- Final scoped source/docs whitespace/path/new checkpoint-anchor checks PASS;
  Orca child is in-review with final Quick77/80clash/F6/p95/clip/QA status recorded.
  Ready frame confirms BCompact label/paused clean reset. Human art choice and
  authorized parent integration remain future actions; default/publication unchanged.


## BCompact 12 particles (2026-10-19, review sample; performance caveat)

- Continue in the existing child feat/pixel-hit-sparks and preserve all earlier
  dirty foundation, parent WIP and delivered 10-particle comparison clips/players.
- MainRays now emits six main rays (three per -16/+45 degree cluster, offsets
  -10/0/+10 with +/-2 degree jitter) and two toward-camera fragments. Together
  with two contact particles and two embers:12 per clash,96 over eight leases.
  Three long rays alternate clusters by owner sequence. Existing pixel dimensions,
  lifetime, palette, motion, camera, default Current and public API are preserved.
- Updated compact tests cover12 emissions, mirror X/preserved Y/Z, main/near Z,
  dimensions/lifetime/custom data, B restoration and eight-lease96-particle reuse.
- Quick archive b1fe9f0210bdcbd9fb1967e438b04660b3d8aa0fceba4a36cfd84d44b62f74a7
  is queued behind another task's shared runner lock; no unrelated job interrupted.
  New helpers/archive use compact12 names under
  E:/CodexTemp/TypingHero/parry-clash-lighting. Runtime/art/performance acceptance
  remains unverified until the new player is built and exercised.

- Final strengthened Quick055443 PASS77/77 (0failed/skipped), input
  dd83bea99d6378e96deffb4c3cffd832db008e885584292ba535b4a452c2b6e5.
  The original queued snapshot never executed; its own waiting wrapper was
  stopped before replacement. New layout assertion limits shared-rotation
  residual range to4degrees across all eight rays/fragments.
- Ten-particle performance baseline PASS: compact12-baseline-normal659checks,
  compact12-baseline-mirror650checks. All-state p95 milliseconds1280/640:
  normal16.500/17.450 (96/96samples), mirror16.450/15.445 (95/94samples).
  Four keyboard parries per row, one visible/focused tab, no video/screenshots.
- Normal QA build055551 has the exact final Quick input; mirror build queued.

- Normal QA055551 and mirror055822 builds PASS; immutable players downloaded
  to compact12-normal-player/compact12-mirror-player. Mirror snapshot
  324beb2e094f683af0e0f71a281bafbc3091f41d3f41c58b5be49d38b7a3385a
  adds only the existing disposable editor actor-mirror fixture.
- compact12-art-normal PASS1602checks,10keyboard parries per size, no exceptions
  or fixture writes; Edge155.0.4283.62, GTX1650Ti/ANGLE D3D11.
- compact12-art-mirror failed430checks at1280 after a missed keyboard parry
  caused player damage; no PASS claimed. Retry keeps product/input timing and
  acceptance unchanged.

- compact12-art-mirror-r2 failed149checks after a second missed parry. Final
  compact12-art-mirror-r3 PASS1782checks,10parries per size. Task-local capture
  helper now polls5ms and sends keyboard40ms earlier to accommodate100ms bridge
  publication; gameplay timing and acceptance unchanged. Original failure reports
  remain failures. Normal1602 plus mirror1782 gives40qualified art parries.
- Inspected native peak and four successive40ms crop frames at both sizes/directions:
  two clusters separate on flight, long/short lengths and head/body pixels remain,
  contact has brief overlapping light but sword edge remains readable. At640,
  individual short rays are less distinct during initial contact than after travel.
  These are recorded samples, not deterministic RNG or human art approval.
- ParrySpark-BCompact-10vs12-640.mp4 and1280.mp4 exported under the task temp root.
  Both44.8s/25fps/1120frames, two directions,1x and0.25x nearest-neighbor closeups;
  side-by-side native-size panes1280x360/2560x720. Full decode PASS. SHA256:
  640=05338a817be9cc52cdb512e929dd4435fa06b6967b9fc13c3d740356ed54ce01;
  1280=34a2186f968b5bb70aa8bec90b8941cf71fc105c71e4c38caac76b7ef9f45205.
- First12-particle performance runs PASS587/671checks (normal/mirror); p95ms
  normal1280/64022.235/18.460, mirror20.200/15.955. Initial1280 increases exceed10%;
  ordered10/12/12/10 runs are in progress for both directions and sizes.

- Final lifecycle compact12-lifecycle PASS1241checks: B->compact and compact->B
  at1280/640; paused preset/count/age/Y bounds held, next emission changed,
  restart cleared leases and restored Current.
- Ordered ABBA all eight runs PASS: normal a1/b1/b2/a2 checks663/684/643/647;
  mirror760/616/640/680. Four real keyboard parries per row; same observer,
  visible/focused single tab, no video/screenshots. Per-run p95ms in order10/12/12/10:

  | Direction/size | p95 sequence | Sample counts | Mean p95 10 ->12 | Delta |
  |---|---|---|---|---|
  | normal1280 |15.930/14.825/19.030/16.705|97/96/98/94|16.3175->16.9275|+3.74%|
  | normal640 |16.585/15.015/16.305/15.080|96/97/97/98|15.8325->15.6600|-1.09%|
  | mirror1280 |15.000/19.105/20.900/15.500|99/96/95/96|15.2500->20.0025|+31.16%|
  | mirror640 |15.020/20.190/15.800/14.840|101/95/95/99|14.9300->17.9950|+20.53%|

- Performance caveat remains: mirror repeat p95 exceeds10%; no equivalence or
  no-spike claim. Values are mean of the two per-run p95s, not pooled p95.
  Only5-7live-clash frames per row; active medians12.8-14.5ms and idle medians
 13.2-13.7ms do not explain tail variation. These observer samples cannot assign
  the tail increase to GPU particle cost. No stress/soak or unrelated optimization.
  Runtime and art checks passed; this revision remains a review sample with
  the measured performance concern, not a performance-approved release.

- Interactive12-particle QA is open/paused at1280x720 with BCompact selected,
  clean real-UI reset (score0,40/850HP). Ready screenshot inspected. Owned
  PowerShell18504/Node17856, loopback8139; old21328/20368 processes and old8139
  listener were absent before launch. Keep compact12-interactive profile/window
  for user review; Resume plays, F6 cycles next-emission presets. Reopen using
  existing open_compact.ps1 -Name compact12-interactive -Player
  E:/CodexTemp/TypingHero/parry-clash-lighting/compact12-normal-player.
- Retain new compact12 players/snapshots, final Quick recovery archive, mirror
  input fixture archive, raw qualified art videos, failed-run receipts, lifecycle/
  performance JSON, measurement summary, inspected frames, final helpers and two
 10vs12 MP4s under the existing task temp root for art review/performance audit.
  Server retains final055443Quick/055551normal/055822mirror results and
  compact12-review raw videos/frames/final MP4s. Remove after review, concern
  disposition and verified integration; keep earlier review resources unchanged.
- Cleanup verified:17completed compact12 Edge profile caches,3redundant local
  transfer archives and13remote transfer/encoded-segment/concat files removed.
  Final reports, players/clips, interactive profile and shared caches preserved.
  Scoped source/docs trailing whitespace and Git diff checks PASS; Quick XML
  confirms77total/77passed/0failed/0skipped. No commit/push/merge/deploy.


## BCompact debris tail (2026-10-19, functional PASS; performance caveat)

- Reuse Orca child `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks`,
  parent `C:/Users/ADMIN/orca/TypingHero`, branch `feat/pixel-hit-sparks`, base/main701830b.
  Existing dirty art/scenes/builders/QA and all prior review clips preserved. No release requested.
- Keep12particles (contact2/main8/embers2), six fast rays and two camera fragments unchanged.
  Only compact embers use analytical two-flight trajectory: horizontal1.2-2.2 mirrored,
  upward.6-1.4, gravity9.81, center groundY+1/32. First contact retains vertical22%
  and horizontal55%; second contact stops. Rest180ms, fixed1x1 at16PPU,
  yellow/orange/dark-red cooling and alpha fade from first contact through rest.
- Ember gravity/collision/color modules disabled only for compact and restored on preset reuse.
  State cleared on Clear/disable/re-emission; unscaled updates respect existing pause/reading locks.
  No collider/Rigidbody, new preset/public API, camera/gameplay/default or itch changes.
- Intermediate Quick77/77 PASS; final Quick78/78 PASS on snapshot
  `63171dbeb36002ddd417fc7b37fdb8830ecc2f807f6d97cb48515aebd4b09efb`,
  `/data/typinghero-game-loop/results/20261019T063719-quick`.
  New deterministic test covers both directions, single bounce, floor clamp, stationary rest,
  fixed size, monotonic alpha/cooling, expiry/Clear and module restoration.
  Existing real-time pause/hitstop/release/reuse test now allows2.5s for deep-floor tail;
  pool regression still asserts96particles/eight leases. Final test count78,0fail/skip.
- Required WebGL/native keyboard QA, clips and no-video performance/ABBA are completed
  in the final receipts below. Functional acceptance passes; performance caveat remains.
  Earlier mirror p95+31.16%/+20.53% remains unresolved; this tail change does not resolve it.
- Preserve `E:/CodexTemp/TypingHero/parry-clash-lighting` and prior server review resources
  for the requested comparisons; cleanup after review and verified integration.


### Debris-tail final native receipts

- Immutable normal QA063931 uses final Quick snapshot63171dbeb36002ddd417fc7b37fdb8830ecc2f807f6d97cb48515aebd4b09efb;
  mirror QA064919 input6bfd9a00d9e206b1694019411f8f44a97cdc5cc5bc71caed91d6049e2ceb9336
  adds only the retained editor mirror fixture. Build/repeated builder pass both.
  Full software browser fails106/111checks after missed parries, not a profile PASS.
- Edge155.0.4283.62/GTX1650Ti/ANGLE D3D11: art normal3475/mirror2867 PASS,
  ten real keyboard clashes per size/direction (40total), normal/Slayer.
  B/BCompact lifecycle normal789/mirror685 PASS, pause/restart/F6 next emission.
  Tail screenshots normal100/mirror120 PASS, ages.55/.75/.95/1.1s at both sizes;
  inspected arc/low bounce/rest/fade, fixed1pixel geometry and sword clearance.
  Rest floor center-2.8713007, no undershoot in sampled tail states.
  All reports0fixture reads/writes/exceptions; networking blocked.
- Performance measured separately without video/screenshots, visible/focused tab,
  four real parries per size/run. Nearest-rank p95; A=before12/B=after12:

  | Side/size | A1/B1/B2/A2 p95ms | Samples | Mean A ->B | Delta |
  |---|---|---|---|---|
  | normal1280 |21.600/19.385 (AB only)|92/94|21.600 ->19.385|-10.25%|
  | normal640 |25.310/17.050 (AB only)|92/99|25.310 ->17.050|-32.64%|
  | mirror1280 |21.855/19.155/16.745/14.675|95/94/99/97|18.265 ->17.950|-1.72%|
  | mirror640 |16.650/21.810/14.030/14.260|98/92/101/102|15.455 ->17.920|+15.95%|

  Mirror640 first increased30.99%, triggering the required B2/A2 repeat.
  The repeated mean still exceeds10%; no no-regression/performance approval claim.
  This does not establish causation or resolve the earlier10vs12 mirror concern.
- Task temp clips (49.6s each, both directions,1x and.25x; old clips untouched):
  `ParrySpark-BCompact-Tail-BeforeAfter-640.mp4`, SHA256
  53a0f0518d3b2a8eba211215868495a61db09357c3e4bff756f930a536e19c06;
  `ParrySpark-BCompact-Tail-BeforeAfter-1280.mp4`, SHA256
  56ac9bfb0fa936d70ff0cf6e490bea8a134ab92a62e511e460155cf6e3a4f4fc.
  Export uses nearest-neighbor close-up with preserved aspect, inspected labels/frame;
  source videos and before12clips retained for comparison. No generated capture committed.
- Final task temp root remains E:/CodexTemp/TypingHero/parry-clash-lighting;
  new tail-* reports/players/helpers, two clips and server tail-review retained for review
  and the unresolved performance concern. Reuse existing child; no new worktree/agent.
  Current/default/camera/gameplay/itch unchanged. No commit/push/merge/deploy.

- Final cleanup/QA: removed12completed tail Edge profile caches after all processes exited;
  retained tail-interactive profile only. Fresh PowerShell29660/Node20664 hosts loopback8139,
  ready screenshot inspected:1280x720, paused, BCompact, score0/clean restart.
  Resume plays; F6 cycles five presets for next emission. Reopen via existing open_compact.ps1
  -Name tail-interactive -Player E:/CodexTemp/TypingHero/parry-clash-lighting/tail-normal-player.
  Shared caches, earlier review clips and parent/child checkout remain. Cleanup of retained
  task resources waits for review/performance disposition and verified integration.
  Final runtime snapshot still matches Quick78/native player input; scoped whitespace PASS.


## BCompact volume and continuous spark flight (2026-10-19, in progress)

- Requested plan: 12 total (contact2/middle2/front4/back4/embers0), eight continuous
  falling sparks and two fast camera fragments; 96 over eight leases. Current,
  camera, gameplay and itch unchanged. Reuse existing child/parent/base above.
- Runtime now evaluates ten fixed spark states by absolute unscaled age, with
  exponential launch drag over120ms then gravity9.81/coast drag; analytical
  single bounce retains22% vertical and55% X/Z, second contact stops, rest180ms.
  Pixel silhouette follows projected speed and bounded depth scale.
- Full Quick, immutable native players/repeated builder, keyboard QA at both sizes
  and directions, before/after clips and separate no-video p95 remain pending.
  Do not infer acceptance from implementation. Earlier mirror performance concerns
  remain unresolved. Retain volume-* resources under existing task temp root.

### Volume implementation verification checkpoint

- First Quick072346 input3736d668:75/78. Fixed cross-emission test seed matching
  and real compact lease completion (zero simulationSpeed keeps IsAlive true
  after particle removal; completion now uses manual state plus contact).
- Quick073018 input8621b782:77/78; only pool fixture failed because its contact
  was exactly at ground0, deliberately reducing initial ray lengths to1pixel.
  Fixture now sets groundY=-2; runtime remains unchanged. Final Quick queued.
- Final local input28e395dab45e1ac3e6c05d2779a47e7b56ccf97878ecf0f5da4322ecd10339de.
  Removing only that fixture line exactly reproduces player input8621b782006105dd031ccd25328836cc0f39899faf63ff279143b5c645ca3f06.
- Obsolete QA072457 was canceled by stopping only its owned Docker container;
  no result PASS claimed. Normal QA073121 has repeated-builder PASS, compilation
  in progress. Mirror/final Quick wrappers remain queued on the common lock.
- Native no-video baseline A1 PASS:normal1198/mirror977checks,4keyboard parries
  per size; Edge155.0.4283.62/NVIDIA GTX1650Ti ANGLE D3D11. Nearest-rank p95ms
  normal1280/640=14.420/14.585, mirror=15.145/19.250 (100/101/100/96samples).
  New volume-baseline.json/source-hashes.json/build/fetch/native/export helpers
  are under the retained task temp root. After measurements/art acceptance pending.

### Volume native / performance continuation

- Final Quick073726 PASS78/78,0failed/skipped, input28e395dab45e1ac3e6c05d2779a47e7b56ccf97878ecf0f5da4322ecd10339de.
- QA073121normal/073528mirror build/repeated-builder PASS. Prefab and meta are
  byte-identical locally/generated, including all IDs/ordered GUID references.
  Normal input8621b782 differs from final Quick only by the ground fixture line;
  mirror57872d58 adds only the retained editor mirror fixture to that normal input.
- Native art normal3318/mirror4034checks PASS,10parries per size/direction,40total.
  Lifecycle normal1062/mirror1139 PASS; reports additionally confirm restart roots
  empty and new emitted preset Current. Native tail normal117 PASS; mirror1280
  completed all4ages in the failed57check full run (missed640parry),640retry48 PASS.
  All native reports0fixture reads/writes/exceptions. Floors sampled no lower than
  -2.8713007, matching scene groundY-2.9025507 plus1/32.
- Interactive Edge19476PowerShell/6772Node, loopback8139,1280x720, paused BCompact
  score0/clean restart. Ready PNG inspected. Reopen using open_compact.ps1 -Name
  volume-interactive -Player E:/CodexTemp/TypingHero/parry-clash-lighting/volume-normal-player.
- Normal first performance/ABBA results are DIAGNOSTIC ONLY: the old normal player
  lacks allocation profiling (allocatedBytes=-1), while the new normal art player
  enables it (median1002). They cannot establish the revision's performance delta.
  A fresh normal --no-profile build is queued behind another task's shared runner
  lock. Do not interrupt the unrelated job. Fetch it as volume-normal-perf-player,
  then run volume_matched_perf.py (ordered fresh A1/B1/B2/A2, asserts all counters-1).
- Mirror A/B already use matching allocation-enabled builds: p95ms1280/640
  15.145/19.250 ->14.135/14.200,100/96 ->100/100samples,977/1307checks PASS.
  Initial mirror increases stay below10%, so no mirror ABBA required. Earlier mirror
  concerns remain unresolved; these short samples do not demonstrate a fix.
- New clips are being aligned to the first score30 parry using actual HUD glyphs
  (generic brightness/score-change selectors can accidentally pick normal hit10).
  Two54.4s outputs include both directions,1x and full-flight0.25x. Retain older clips.

### Volume final receipt ? implementation and verification complete

- All requested technical/keyboard/native/media checks are completed above. Final
  source remains Quick28e395dab45e1ac3e6c05d2779a47e7b56ccf97878ecf0f5da4322ecd10339de.
  Normal art/player8621b782 and mirror57872d58 have the same product runtime; only
  the final test fixture line/editor mirror fixture differ. Normal performance
  player080156 is the exact final Quick input, with matching non-development flags.
- Matched no-video/no-screenshot performance is complete. A=previous12particle tail,
  B=continuous8falling sparks. Four keyboard clashes/size/run, visible/focused tab,
  same observer/machine. Normal uses allocation profiling OFF on both players
  (every sample allocatedBytes=-1); mirror uses profiling ON on both players.
  Initial normal profiling-mismatched diagnostics are excluded from conclusions.
  Final normal QA080156 uses final Quick input28e395da with --no-profile; build and
  repeated-builder PASS. Fresh ordered normal ABBA checks1275/1388/1428/1259 PASS;
  mirror A/B checks977/1307 PASS. Nearest-rank sampled frame p95ms:

  | Side/size | A1/B1/B2/A2 p95ms | Samples | Mean A ->B | Delta |
  |---|---|---|---|---|
  | normal1280 |14.050/14.175/13.925/15.520|101/101/102/99|14.785 ->14.050|-4.97%|
  | normal640 |14.385/14.500/13.670/13.785|102/101/102/102|14.085 ->14.085|0.00%|
  | mirror1280 |15.145/14.135 (AB only)|100/100|15.145 ->14.135|-6.67%|
  | mirror640 |19.250/14.200 (AB only)|96/100|19.250 ->14.200|-26.23%|

  Normal means are means of the two per-run p95s, not a pooled percentile. Live
  samples/row:normal34-45,mirror33-42. Matched normal live-only mean p95 changes
  15.1625->13.7175ms at1280 and14.3975->14.2275ms at640; mirror15.955->14.795 /
  16.265->13.730ms. No matched increase exceeds10%. Short sampled QA deltas include
  observer/idle/background overhead; they do not establish GPU cause, no spikes,
  whole-device FPS or a fix for the earlier mirror+31.16%/+20.53%/+15.95% concerns.

- Final clips in E:/CodexTemp/TypingHero/parry-clash-lighting:
  `ParrySpark-BCompact-Volume-BeforeAfter-640.mp4`,4,521,299bytes,SHA256
  9baafa820f6687ff3215b5765e28414f60d48eb34b18a8b979f758e334d89ec7;
  `ParrySpark-BCompact-Volume-BeforeAfter-1280.mp4`,11,358,828bytes,SHA256
  ebdd51988b556fe937e8c1d3d38b5f7dc04a22a5515791e9ad157992eb2686b0.
  Both54.4s/25fps/1360frames, both directions,1x/.25x, nearest-neighbor close-up,
  score30 parry onset alignment. Full decode and local/server hashes PASS; output
  labels/aspect/contact and native trajectory grids/PNGs inspected. No deterministic
  RNG replay or human art approval claimed. Earlier clips/players remain intact.
- Edge review19476PowerShell/6772Node opened paused at1280x720 on loopback8139,
  BCompact selected, clean score0/40-850HP; ready PNG inspected. It closed normally
  at08:07UTC (passed5checks), before the matched normal ABBA. Both PIDs and the8139
  listener are absent at final verification. Keep ready PNG/launcher/reports,
  volume-normal/mirror players and volume-normal-perf player, all qualified/failed
  receipts, raw videos/frames, final clips and helpers under the existing task temp
  root. Server retains QA073121/073528/080156, Quick073726 and volume-review.
  Reopen command is recorded above. Cleanup after review/performance disposition
  and verified integration; no merge/push/deploy/production build was performed.
- Cleanup verified:20completed volume Edge caches and4duplicate local archives
  removed;14duplicate remote transfer/encoded-segment/concat files removed.
  Interactive profile, source/worktree, shared toolchain/Library, players, raw
  evidence and all old/new final clips retained as requested. Current/default,
  camera/gameplay and itch remain unchanged. Source/docs whitespace checks pass.

## BCompact reverse sparks - 2026-10-19 (verified, ready for review)

- User approved16particles/clash and128over8leases, superseding12/96 for BCompact.
  Keep the existing10moving sparks/contact2; append4reverse-X falling sparks,
  two near and two far. Counts2/2/6/6/0;14fixed states and6particle scratch slots.
  Same launch/brake/fall/one-bounce/rest/color/pixel path; X mirrors, Y/Z preserved.
- Child remains feat/pixel-hit-sparks at
  C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks; parent/main and
  preexisting dirty assets untouched. Current/default, camera/gameplay/itch unchanged.
- Runtime and compact tests updated; Quick and normal QA build submitted. Browser,
  performance and clips are pending, not yet verified. No merge/push/deploy.
- Existing volume-normal/mirror(-perf) players are immutable12particle baselines.
  New reverse-* helpers/artifacts and reverse-source-before backup are under
  E:/CodexTemp/TypingHero/parry-clash-lighting. Keep old clips/players for review.
  Earlier interactive Edge cache was removed after viewer exit; the previous
  cleanup paragraph saying its profile was retained is superseded here.

- Preflight diff found an unintended reverse variable in the generic emitter; fixed
  before verification. Three queued stale snapshots were canceled before acquiring
  Unity lock (no test/build result); corrected normal/Quick/mirror snapshots queued.
  No other task job/container was stopped. All prefab/scene/meta bytes unchanged.

- Verified checkpoint: Quick20261019T084732 passes78/78,0failed/skipped; normal
  QA20261019T083644 build/repeated-builder PASS. Both inputc8d34d84840756d629276ee8e012b43c056e0908dbbf40daa1461d5f498f6c03.
  Mirror QA20261019T084910 build/repeated-builder PASS; input7eea5996085debd8da4f7249e75ff61d1aed962bdb9da777cc03ed447012b1ed
  adds only disposable editor actor mirror fixture. No product fixture edits.
- Native normal art6274checks/20keyboard parries, lifecycle1274, tail135 PASS.
  Native motion grids640/1280 inspected; reverse branches separate, sword contact
  remains readable. Tail center >=-2.8713007 at sampled ages. Mirror QA pending.
- Matched profiling-ON normal12->16 no-capture p95ms:1280 13.575->13.560 (-0.11%),
  n101/99;640 13.760->14.060 (+2.18%),n100/100. Live-only n41/43,44/40;
  p95 14.355->13.920 /13.640->13.710ms. Checks1420/1386 PASS; no >10% trigger.
  Actual counters verified>=0 on both players; old marker predates profile flag,
  so baseline identity is checked by exact prior build hash instead. Short sampled
  deltas do not prove GPU cause or resolve prior mirror performance concerns.

- Mirror retry art5608checks/20parries, lifecycle1179, tail146 PASS. First mirror
  art58check run failed on missed keyboard timing after one parry; retained failed,
  no product timing/threshold changes. Combined qualified art40parries at640/1280,
  two directions, normal/Slayer. All native reports0exceptions/fixture reads/writes.
- Initial mirror AB checks1377/1365 PASS. All-state1280 p95 16.065->16.055ms,
  n101/101;640 15.110->15.430ms,n99/101. Live-only640 13.845->15.700ms (+13.40%),
  n42/44 triggered fresh mirror ABBA despite all-state deltas staying below10%.
  ABBA is pending. Normal/Slayer exclusion, post-restart Current and floor sample
  bounds are independently checked in reverse-native-summary.json.
- New final clips decode and local/server SHA256 PASS; old clips untouched.
  reverse-check640/1280 labels/aspect/contact inspected. Two54.4s/25fps/1360frame
  side-by-side12->16clips include both directions and1x/.25x nearest-neighbor crop.
  Onsets align by first score30 parry; RNG and exact animation phase are not locked.

## BCompact reverse sparks (2026-10-19, verified 16-particle review revision)

- Approved16/clash,128over8leases supersedes12/96: groups2/2/6/6/0; original10moving
  sparks plus4reverse-X falling sparks, two near/two far. Fourteen fixed states,
  six-particle scratch;12fall/one-bounce/rest180ms paths plus2fast front fragments.
  No shader/prefab/public API/default/camera/gameplay/itch changes.
- Quick20261019T084732:78/78 PASS,0failed/skipped. NormalQA20261019T083644 build/
  repeated-builder PASS; both inputc8d34d84840756d629276ee8e012b43c056e0908dbbf40daa1461d5f498f6c03.
  MirrorQA20261019T084910 PASS, input7eea5996085debd8da4f7249e75ff61d1aed962bdb9da777cc03ed447012b1ed
  adds only disposable editor actor mirror fixture. All13serialized parry assets
  retain identical bytes/IDs/ordered GUID references;614local prefab/scene/meta bytes unchanged.
- Edge155.0.4283.62/Playwright1.58.2/GTX1650Ti ANGLE D3D11: normal6274/mirror retry5608
  checks PASS,40keyboard parries (10per640x360/1280x720/direction), normal/Slayer hit
  exclusion. First mirror58check run missed keyboard timing after1parry; retained FAIL.
  B<->compact pause/restart normal1274/mirror1179 and tail135/146 PASS. Postchecks
  confirm restart clears roots/restores Current, sampled centerY>=-2.8713007 at
  ages.55/.75/.95/1.1. Motion grids at both sizes/directions show both branches,
  pixel shrink/fall/cooling and readable sword contact, no observed ground penetration,
  blur or pink. Quick covers12single bounces, X/Y/Z rest/fade, spawn/mirror and128cap.
  All native reports0exceptions/provider reads/writes; external blocking and read-only
  bridge preserved. Sampled visuals are not every RNG outcome, sub-frame proof or human approval.
- Matched12->16 no-video/no-screenshot p95, focused visible tab,4parries/size/run,
  profiling ON both players, actual counters>=0. Legacy baseline marker predates
  profile flag; exact prior volume input hashes checked instead. Normal AB1420/1386
  PASS. Initial mirror AB1377/1365 PASS:640live p95+13.40% triggered fresh ABBA,
  checks1392/1393/1422/1497 PASS. ABBA means below average per-run p95s, not pooled p95.

| Direction/size | All-state p95 A -> B (ms) | Change | Live p95 A -> B (ms) | Change |
|---|---|---|---|---|
|normal1280 AB|13.5750 -> 13.5600|-0.11%|14.3550 -> 13.9200|-3.03%|
|normal640 AB|13.7600 -> 14.0600|+2.18%|13.6400 -> 13.7100|+0.51%|
|mirror1280 ABBA|13.6925 -> 14.5950|+6.59%|13.6700 -> 15.0050|+9.77%|
|mirror640 ABBA|14.2050 -> 13.7450|-3.24%|14.6125 -> 13.6800|-6.38%|

- Normal all/live n1280=101/99 and41/43;640=100/100 and44/40. Mirror ABBA
  A1/B1/B2/A2 all1280 p95=13.685/15.380/13.810/13.700ms,n100/102/100/103;
 640=14.780/13.755/13.735/13.630ms,n100/100/101/101; live n1280=43/41/42/43,
 640=40/43/43/44. Repeat means below10%; live1280+9.77% is close to threshold.
  Short sampled deltas include observer/idle overhead, not GPU cause/device FPS/
  no-spike/equivalence evidence. Prior mirror+31.16%/+20.53%/+15.95% concerns remain unresolved.
- Two new12->16 clips, both directions,1x and.25x nearest-neighbor closeups;
  54.4s/25fps/1360frames each. Full decode and local/server SHA256 PASS; labels,
  aspect/contact inspected. First score30parry aligns onset; RNG/animation phase not locked.
  `ParrySpark-BCompact-Reverse-BeforeAfter-640.mp4`: 4682308bytes, SHA25654f4b35918a7b9a66f0fd47f4710bed47035653371547e383bdccb11d45d1719.
  `ParrySpark-BCompact-Reverse-BeforeAfter-1280.mp4`: 11606552bytes, SHA256c21e984af3b67d27560d2d09cb5afe17890a8c93d9ce521ff6767dff1df1a0a6.
- Review resources retained at E:/CodexTemp/TypingHero/parry-clash-lighting:
  reverse-normal/mirror-player, raw captures, qualified/failed reports, source backup,
  helpers and all old/new clips. Completed reverse QA profiles15 and duplicate local
  archives3 removed (18targets verified); no persistent Edge viewer retained.
  Keep child feat/pixel-hit-sparks until review and verified integration; no commit/push/merge/deploy.

- Final cleanup also removed12duplicate remote transfer/segment/concat files; final
  clips/raw videos/players remain. Code hashes match qualified Quick/QA snapshot,
 614asset bytes and unrelated UTF-8 document prefixes verified unchanged; scoped
  whitespace checks PASS. Task QA listener8137 closed. Review reopen command:
  `powershell -NoProfile -File E:/CodexTemp/TypingHero/parry-clash-lighting/open_compact.ps1 -Name reverse-interactive -Player E:/CodexTemp/TypingHero/parry-clash-lighting/reverse-normal-player`.

### GitHub push approved (2026-10-19)

- User approved pushing the reviewed child. Target origin=https://github.com/micharel09/TypingHero.git,
  branch feat/pixel-hit-sparks (remote branch absent before push); no main merge or deploy.
- Include the existing uncommitted parry/lighting foundation recorded in the prototype
  checkpoint: builders, world materials/normals/scenes, lifecycle/QA wiring and docs,
  together with final16particle BCompact. All belong to the verified player snapshot.
- Recomputed full Assets/Packages/ProjectSettings/runner input matches final Quick/normal
  QA c8d34d84840756d629276ee8e012b43c056e0908dbbf40daa1461d5f498f6c03 exactly.
  No runtime edits since qualified verification; no repeated runtime suite required.
  Parent checkout/main and retained review artifacts remain untouched.


## BCompact strong reverse fan ? 2026-10-19 (implementation; verification pending)

- Continue existing Orca child e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/feat-pixel-hit-sparks,
  parent C:/Users/ADMIN/orca/TypingHero; branch feat/pixel-hit-sparks, remote
  https://github.com/micharel09/TypingHero.git, integration target main.
  Clean child fast-forwarded 4f8b09c -> origin/main c1f4841 before edits;
  parent checkout and its existing WIP untouched. No push/merge/deploy authorized by this revision.
- Requested24/clash,192/eight leases: contact2/main2/front10/back10/embers0.
  Preserve first10 moving spark random draws; append12 reverse sparks,6near/6far,
  each3long/3short. Stratified -12..36degree fan plus existing shared +/-15degree
  and +/-2degree jitter. Reverse X speed32..44 times energy; Y retains14..24 speed.
  Launch X drag10/s only for reverse states; Y/Z20/s,120ms braking; coast X/Z2/s,
  gravity9.81, single22%vertical/55%tangent bounce and180ms full-axis rest unchanged.
  Fixed22states/10scratch slots. No prefab/shader/dependency/public API/default/camera/gameplay/itch changes.
- Added24seed/two-direction1unit-high fixture acceptance at300ms: >=8/12 travel
  >=2.5units reverse X, >=6 retain2pixel length. Updated count/mirror/bounce/reuse tests.
  Unity Quick queued behind existing shared runner jobs; no passing runtime result yet.
- Review artifacts/helpers use strong24-* under E:/CodexTemp/TypingHero/parry-clash-lighting.
  reverse-normal/mirror-player are immutable16particle baselines; preserve prior clips.
  Native art/lifecycle/floor QA, repeated-builder/serialized identity, 16->24clips,
  matched no-capture p95 and conditionalABBA remain pending. Prior mirror performance
  concerns remain separate. Retain child/player/clips for requested review.


### BCompact24 Quick receipt (2026-10-19)

- Unity2022.3.25f1 on fly-server: Quick20261019T093602 passes79/79 actual tests.
  Immutable input fe4c72f5d87ef91a8fa511865f57fd95d778caba5b562745ae87598ed282ce23.
 24/192budget/groups, same-frame launch/mirror X/preserved Y/Z,20single bounces,
  full-axis rest/fade/expiry, pause/reading/hitstop/reuse/module restoration pass.
  New24seed/two-direction1unit-high fixture passes300ms >=8/12reverse particles
  beyond2.5Xunits and>=6streaks at least2pixels. Repeated builder/tuning test passes.
- Local623serialized/meta files match HEAD content after Git newline normalization;
  SHA256 inventory retained in strong24-asset-identity.json. Native QA/build/clip/
  performance acceptance remains pending; do not infer visual PASS from Quick.


### BCompact24 normal QA, performance and release steering (2026-10-19)

- Normal immutable QA20261019T093707 build/repeated-builder PASS, same fe4c72f5 input
  as Quick79. Edge155/Playwright1.58.2/GTX1650Ti ANGLE D3D11: art3336checks,
 20keyboard parries (10per640x360/1280x720), normal/Slayer; lifecycle967 and tail118
  checks PASS. No exceptions/provider reads/writes; fixture/external blocking kept.
  Recorded motion grids show reverse sparks beyond the player's rear silhouette,
  readable contact and no observed floor penetration; sampled floor>=-2.8713007.
- First71check art attempt failed after missed real-keyboard parry. QA-only harness
  now holds Space until observed parryPose/window, rather than only a newer frame;
  product timing unchanged. Failed attempt retained as failed.
- Matched no-capture profiling-ON16->24 normal AB checks1936/2162 PASS;1280 live
  p9517.305->19.670ms (+13.67%) triggered freshABBA. Four ordered runs pass
  checks2078/1955/2209/2029, four keyboard clashes/size/run, focused visible tab.
  All-state p95 A1/B1/B2/A2:1280=14.450/14.845/13.750/14.375ms;
 640=14.175/16.845/13.650/16.410ms. Means A->B14.4125->14.2975 (-0.80%) /
 15.2925->15.2475 (-0.29%). Live means14.2475->15.0825 (+5.86%) /
 15.185->14.720 (-3.06%). All sample counts99..103;live40..43 per run/size.
  Means average per-run p95s, not pooled percentiles. No repeated increase>10%;
  short sampled data does not establish GPU cause/equivalence/device FPS or resolve
  previous mirror+31.16%/+20.53%/+15.95% concerns.
- User explicitly canceled further mirrored QA and requested Git push plus itch.io
  publication. Mirror build094745/repeated-builder already PASS (input8eee591e),
  but running native mirror art/process tree was stopped; do not claim mirrorQA or
  mirrorperformance PASS. Offline24seed/two-direction Quick evidence remains valid.
  Clips now cover normal only at both sizes,1x/.25x full viewport; previous clips kept.
- Remote main advanced tobf98c27 with approved HUDR7/Endless/full-release work while
  this task ran. Committed spark changes e018961, merged latest origin/main cleanly
  into feature child before release to preserve those features. Parent WIP untouched.
  Integrated Quick and production build are queued; upload not yet performed.
  Current remains the serialized default, as requested by original plan.


### BCompact24 published ? 2026-10-19

- User canceled mirrored native QA/performance and authorized push plus itch.io.
  Integrated Quick20261019T095939:117/117 PASS,0failed/skipped. Production100117
  PASS on the same63e6532d5a2c69a00e43af0e4544943bd3c34731b06681d2559baafb703f0c43
  input; qa=false, all JS/WASM/data exclude bridge markers. Original-HTML native
  smoke3checks PASS,0exceptions/requests/writes; menu/Fight/pause/resume screenshots
  inspected. This scoped production smoke does not replace prior full-mode QA.
- Published https://micharel09.itch.io/clackblade, game3857509, new playable19872290.
  Saved previous19871730 hidden; other uploads/settings/gallery/pricing unchanged
  (settings SHA25602f423e4de577aa9926d0f7fa8e4757d228a6cb6109264a2b9593f477a4a4a90).
  Public Run game renders CLACKBLADE;29non-HTML CDN files match the ZIP exactly,
  index.html contains original HTML plus expected itch wrapper. No Firebase change.
- ZIP CLACKBLADE-WebGL-BCompact24-20261019.zip:30members/16,595,759bytes, CRC PASS,
  SHA256e50893f0081ac5715f1215c8a76d27938c0b4276ff80c9a3ed09f96a4512b0d1.
  Canonical receipt marketing/itch-io/clackblade-release-20261019.json; ignored ZIP
  under Builds/. Source pushed to origin/feat/pixel-hit-sparks at93f8eb7 before upload;
  final receipt/doc commit follows. No push to main; parent checkout/WIP preserved.
- Review clips normal only (mirrored continuation canceled):640/1280,27.2s each,
 16->24 at1x/.25x full viewport. Full decode/hash/labels/aspect checked; RNG/contact
  animation phase are not locked.640 SHA2565ff54e05fec76103dad202934cdfe5a7d2e591ec7933ce867f9af480fd6fb100;
 1280 SHA25670df40579895bf543eef8ac265c00bb75725de01d8d3842f15584a64f87486a9.
  Old clips retained. New files ParrySpark-BCompact-Strong24-BeforeAfter-640.mp4 and
  -1280.mp4 under E:/CodexTemp/TypingHero/parry-clash-lighting.
- Keep child feat/pixel-hit-sparks, strong24-normal/mirror/production-player, clips,
  qualified/failed reports and immutable source inputs for requested review and
  reproduction.14completed task Edge profiles already removed; production-smoke
  profiles are cleaned at final handoff. Mirror native process tree stopped on request.
  Earlier mirror performance concerns remain separate/unresolved. Current stays default.
  Feature is published from its pushed branch, not integrated into main; retain child
  until requested review and verified integration. Authenticated browser session is
  stopped after public verification. No devlog/social posts or unrelated changes.


## Physical spark release correction (2026-10-19, active)

- User reports missing physical falling sparks on public. Prior release explicitly
  retained Current; production smoke only proved menu/Fight/pause, not a successful
  parry using compact physics. Original BCompact QA used F6 and did not prove the
  default production route. This is a release-selection verification gap.
- Public now points to full release19872500 after the separate latest main release;
  merged origin/main933dbbd into child to preserve all current fixes before correction.
  Resolve system map using compact VFX row plus newest other owners; current release
  receipt uses newest main receipt until new publication, preserving prior history.
- Change production owner default and builder to BCompact; serialize player prefab4.
  Current/A/B/C remain selectable legacy variants. Add no-F6 owner/reuse regression
  and builder regression that converts serialized Current to compact on two runs.
  Native normal QA must use default from fresh game/restart with no F6; final
  production/public check must capture actual successful parry and falling particles.
  No mirrored QA requested. Tests/builds use compact-default-* artifacts in existing
  E:/CodexTemp/TypingHero/parry-clash-lighting. Publication remains pending.

- Focused Quick123645 passes14/14 ParryClashTests, including the two new default
  selection regressions and existing24seed/two-direction trajectory/lifecycle tests.
  Input8ba1dda11a10270a96b8f096c30df977ee6f4e7c085b0c1340196a52216400a2.
- QA123737 build/repeated-builder PASS, generated player prefab has sparkPreset4.
  Native Edge default test r2 passes16checks, noF6, two keyboard parries separated
  by restart. Actual BCompact samples:20particles at300/550/800ms and12at1050ms;
  qualified r2 minimumY descends -2.16464 -> -2.83872 -> floor-2.87130. Launch/fall/rest frames
  inspected on current R7/Endless scene. First10check attempt failed on stale
  one-frame spark snapshot immediately after scene reload; retained FAIL. Harness
  waits>=3fresh frames and empty roots before asserting restart, without runtime changes.
- Earlier production baseline real-keyboard parry succeeds(score30/40HP) but old
  Current tail is absent at falling sample. New production124717 is building from
  the same8ba1input; actual production/public parry images still required.


### Physical parry default correction published (2026-10-19)

- Root cause: prior default Current remained selected in production; earlier compact
  tests used F6, and the first production/public smoke only proved menu/Fight/pause.
  User's report was valid. Latest separate full release19872500 also used Current.
  Preserved its main933dbbd fixes; owner default, serialized Player prefab and
  CombatFeedbackBuild now all select BCompact4 without F6. Current stays explicit.
- Focused Quick123645:14/14 PASS,0skips; QA123737 build/repeated-builder and native
  default/r2:16checks PASS. Fresh/restarted game emits BCompact without selection;
  20particles remain at300/550/800ms,12at1050ms, minY reaches-2.8713007 floor.
  Initial10check native attempt failed on stale post-reload probe data, retained FAIL;
  corrected observer waits3fresh frames/empty roots. No gameplay timing changes.
- Production124717 PASS on the same8ba1dda11a10270a96b8f096c30df977ee6f4e7c085b0c1340196a52216400a2
  input. qa=false/all JS/WASM/data scanned. Real-keyboard production parry smoke5
  checks PASS,0JS exceptions/writes,1successful parry,0F6. Launch/fall458ms/
  bounce704ms/rest966ms images visually verified. Current-baseline parry lacks tail.
- Published playable19874589 at https://micharel09.itch.io/clackblade; old19872500
  retained hidden. Public page points to that exact iframe;29non-HTML CDN hashes
  match ZIP and HTML adds only expected itch wrapper. Same public CDN actual game
  independently passes5checks/1real keyboard parry/0F6/0writes/0JS exceptions;
  inspected launch,fall457ms,bounce703ms,rest964ms images show physical particles.
  This directly verifies default public combat, not just a menu or QA-only preset.
- ZIP CLACKBLADE-Physics24-20261019.zip:30members/16,596,608bytes,CRC PASS,SHA256
 541e806b5557e7091198b1d269f5104149f1135bf65d254c06a5cb545c907e1c.
  Receipt marketing/itch-io/full-release-20261019.json, brand receipt reconciled;
  ignored ZIP in Builds/. Storefront settings hashcb998ee631f065a1c43833b8836b490e6300ecc4ebfc0a8e304f3330bd9bab27
  unchanged. No Firebase/schema/other gameplay changes; mirrored QA not requested.
- Correction e79c185 pushed atomically to origin/main and feat/pixel-hit-sparks;
  receipt commit follows. Parent local checkout/WIP untouched. Keep requested child,
  compact-default-qa/production players, screenshots/reports, source input and all
  earlier review clips under E:/CodexTemp/TypingHero/parry-clash-lighting; temporary
  Edge caches cleaned after receipt. Browser session stopped after public check.
  Prior mirror performance concerns remain unresolved; this correction changes routing,
  not the already-reviewed24particle trajectory/appearance. No new broad performance claim.

- Handoff:5completed compact-default Edge profiles removed; evidence/player/clips
  retained. Automatic review rejected the combined cleanup/receipt push to main
  because publication did not explicitly authorize that documentation update to the
  default branch. No rejected command ran. Completed safe cleanup independently;
  final receipt is committed/pushed only to the already-authorized feature branch.
  Runtime correction e79c185 is already on main and public19874589 verified above.


## Contact flash / Sekiro direction (2026-10-19, implementation in progress)

- Authorized plan: preserve released24/192 physics and default BCompact; replace only two contacts with .5WU white-gold core (60ms hold/200ms end), .75->1.5WU pixel orange ring (90ms growth/260ms end), compact light5 (70ms hold/280ms smoothstep end). Legacy140ms quadratic pulse and radius/bloom unchanged. No mirrored QA.
- Parent e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/TypingHero remains untouched with marketing/art WIP. Child e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/parry-contact-flash, branch micharel09/parry-contact-flash. Base main933dbbd was stale16particle input; fast-forwarded child to existing verified/released feature b86e4fa (24/default correction). Remote origin github.com/micharel09/TypingHero.git. No push/merge requested for source; itch publication authorized after acceptance.
- Runtime implementation plus focused tests underway. Retain task contact-flash-* helpers/input/results under E:/CodexTemp/TypingHero/parry-clash-lighting until verification completes; preserve all old physics clips.
- Pending: focused final Quick, repeat builder, normal/Slayer noF6 Edge640/1280 and0/70/140/220/280ms visuals, clips1x/.25x, matched no-capture p95/ABBA if>10%, production actual parry/no bridge, itch payload/CDN verification, final receipts. No acceptance claimed yet.

- Final focused Quick20261019T133029:17/17PASS,0failed/skipped; input cae5ad525fef5884c6f060cb7f0f0a5881733b15fd19754034b95db7e911d6c5. Exact hold/fade, strongest mixed overlap, compact pause/reuse, legacy140ms, contact endpoints/modules/material restoration,24/192 budget and repeated-builder covered. Earlier Quick132019 failed pool return because frozen contact IsAlive stayed true after particles ended; completion now checks manual contact particleCount. Quick13280616/16passed before final compact-pulse pause test was added; no acceptance assigned to failed run.
- Baseline Edge155.0.4283.62, noF6: contact-flash-before-r3 PASS22checks/two sizes, three parries per size and Slayer, restart. Earlier r1/r2 driver failures retained (insufficient parry count; Slayer transition during cue wait); fixed observation only. Baseline no-capture perf-a1 PASS16checks, three parries/size. Driver/raw video/report retained in task temp for comparison; no product changes to satisfy driver.

- QA20261019T133110 build/repeated serialization PASS on same cae5ad input. Production134644 input cf10f42b34c40229079654df0108f8bd83521f5f2571b9d6d1985d4886662404 is building; archive comparison proves the only delta is four whitespace-only prefab fields (token-identical). All runtime/tests/material/shader/config inputs identical.
- Native Edge155.0.4283.62/Playwright1.58.2/Intel UHD0x9BC4 ANGLE D3D11: contact-flash-after PASS22checks, default noF6,3real keyboard parries per640x360/1280x720, Slayer and restart.0exceptions/provider reads/writes; fixtures/network blocking retained. Native before-r3 also22PASS; no mirrored QA performed. First sequential1280 screenshots arrived late; visual timing acceptance uses raw25fps video frames aligned to first score0->30 transition (up to40ms sampling uncertainty, distinct RNG/animation phases). Offline tests establish precise endpoints, not the video.
- Inspected native0/70/140/220/280ms frames, normal/Slayer silhouettes,800ms falling particles and before/after comparison frames: white-gold sword contact, bounded stepped orange ring, local light that fades without scene fog; sword/face/body remain readable, no observed pink/blur. Existing white sword slash is retained. No human art approval or all-RNG/subframe claim.
- Matched no-video/no-screenshot performance A1/B1 each PASS16checks,3parries/size, same QA profiling enabled.1280 all p95=33.820->33.805ms(-.044%,n57/57), live33.875->33.460(-1.225%,n26/25).640 all33.215->33.280(+.196%,n58/57), live33.070->33.280(+.635%,n25/26). No>10% increase, no ABBA needed. Short observed samples include browser/idle overhead; no device-FPS/equivalence/GPU-causality claim. Earlier mirror concerns remain unresolved.
- Two final clips under task temp/contact-flash-review: Parry-Contact-BeforeAfter-640.mp4 (975593bytes,SHA256245e5245e9efd9759b534009c8527767f0aba2a09c1bc388a04d3f71b3f62d35) and1280.mp4 (1738384bytes,SHA256c3c41cf817f208b2ee72e1237ed23d7c44c18b936b8da63b1a67e142ef795736). Each15.2s/25fps/380frames, side-by-side8s1x+7.2s.25x; full decode/local-server hashes PASS. Labels/aspect/native contact frames inspected. Earlier Physics24 clips remain untouched.
- Browser-skill own session rhyc confirms public still19874589 before publication. Another task's unsg editor session was inspected read-only for overlap, never controlled; storefront fields must be snapshotted fresh immediately before upload. No new upload yet. Remaining: production actual parry/no bridge, ZIP, itch update, fresh public actual parry/CDN hashes, final receipt/cleanup.

- Lifecycle follow-up before publication: contact now owns contactAge, independent of debrisAge, so completing the spark simulation cannot truncate/freeze the260ms contact. Clear resets both. Added a18th test completing spark simulation via AdvanceDebris(10) then asserting contact survives the next frame and returns its lease after300ms. Normal-frame math is identical while sparks live; no changes to the four byte-identical physics methods.
- One queued Quick and one queued final-production helper were canceled while still holding no children/containers, so their obsolete inputs did not execute. Earlier production134644 completed bridge exclusion but is superseded and will not be published. Final18test Quick/new QA and production are required before release; earlier17/native/perf/clip receipts describe the pre-independent-clock snapshot.
- Browser-skill rhyc session was automatically unregistered after idle; session list is empty and instance61a46c05 remains connected. Open a fresh task session on that same instance at publication, verify game3857509 and capture current settings. No source push/merge/public mutation yet.

- Independent-clock final Quick20261019T135959 PASS18/18,0failed/skipped. Final frozen input c594476614434566fda7309073d7b78ce88a550c99f0a3b5923be10dd6229db3. Runtime/tests/assets remain frozen. Native/player acceptance must now use the final QA build. Live tool sessions at this checkpoint:25341 final QA build-only,19516 final production, both queued behind shared global runner; obsolete production134644 is completed, do not publish it. Correct final helper is contact_flash_final_run.py under task temp. New final QA result path is not created yet. Source parent/WIP untouched.

- Source implementation committed locally as725b5ad on micharel09/parry-contact-flash (8scoped files). No push/merge. Full final input remains c594476614434566fda7309073d7b78ce88a550c99f0a3b5923be10dd6229db3; documentation changes are outside the input archive.
- Final QA20261019T140934 build/repeated-builder PASS on c5944766, native contact-flash-final-after26checks PASS (six noF6 real parries,3per640/1280, Slayer/pause/restart, same visible/focused tab and allocation profiling),0exceptions/provider reads/writes. This supersedes the17test/22native contact snapshot. Final Quick13595918/18/0skip and this QA match exactly.
- Final matched no-capture A2/B2 each20checks PASS,3real parries per size, identical Edge155/Intel UHD0x9BC4 ANGLE D3D11, profiling enabled and visible/focused.1280 all p95=33.500->31.940ms(-4.657%,n57/60), live33.425->31.640(-5.340%,n26/25);640 all33.340->33.595(+.765%,n57/57), live33.340->33.170(-.510%,n25/26). No>10% regression, so ABBA not triggered. These sampled observations supersede A1/B1; no claim of a performance improvement or complete frame distribution.
- Final clip export is replacing only the new task comparison clips with raw final-after videos, preserving all older Physics24 clips. Production final tool session19516 remains queued behind global runner jobs. Do not use obsolete production134644. Parent Ronin task explicitly says no publication; its new9sprites/8normals remain only in parent dirty checkout and are excluded from this Physics24-based release.

- Final native frames and clips visually rechecked after independent-clock build:0/70/140/220/280ms, Slayer,800ms tail at both sizes retain crisp contact/ring and sword/face/silhouette. Initial final1280 export falsely matched name-entry glyphs; exporter now gates score0->30 detection on the red combat HP fill, removing menu false positives. This only changed artifact extraction. Correct first onsets:before6407.60s/after6407.12s,before12808.28s/after12807.44s.25fps samples are within40ms, not deterministic replay.
- Qualified final comparison clips supersede earlier contact clip hashes:6401000942bytes,SHA25684fdbd6d4dcaf3d7a9900b750d3b21c4ad9e7defc2fc7e3d23567166c893cbd6;12801681863bytes,SHA25630025a297680c36debc30f4bee0c80d8547043db37b5ce56743375ff2bbfe6f0. Each15.2s/25fps/380frames,8s1x+7.2s.25x, correct hstack aspect/labels/native images reviewed; full decode and local/server hashes PASS. Old Strong24 comparison hashes remain6405ff54e05fec76103dad202934cdfe5a7d2e591ec7933ce867f9af480fd6fb100 and128070df40579895bf543eef8ac265c00bb75725de01d8d3842f15584a64f87486a9.


### Preserve the newer public DungeonUI release before uploading

- Fresh itch form at publication showed DungeonUI19876564 now embedded, Physics2419874589 hidden. No parry ZIP was uploaded/saved. This supersedes the earlier assumed public target. The already-built c594 production and its5check smoke remain valid pre-UI receipts but must not be published because doing so would revert the newer public UI.
- Published DungeonUI source is clean committed1bedcff, based on e79c185 (already default BCompact24). Upstream source input781c881452f74d06b79ee7f47efb4efe4e6f09f46e3078ee613c94a88cf6e8a0, production142518, ZIP SHA2565907f752234508235aa17e13a4468e82a1329f6eb0f9cde4a8f2ed0c9288a212,30files/16575624bytes. Its own public-payload.json verifies29files and iframe19876564; source/UI receipts81native+6modal-size+1172victory are reused for byte-identical published UI handlers/geometry. No redesign or new UI decision.
- Saved qualified pre-integration parry docs locally as a32c63c, merged exact1bedcff into task branch without conflicts, committed local merge cc0638558269ce4ca7e376f40fea05fc3dfc75b6. No push/default-branch merge. Diff against1bedcff under Assets/Packages/ProjectSettings is exactly the8parry files from725b5ad; all published UI changes are retained, parent Ronin WIP excluded. Parent/source scopes remain intact.
- Final combined frozen input48780cca238ebdfff564cb21927eb052de661cc6ae60e331016ca3f401fa5d06. Combined focused Quick PASS18/18. Combined QA tool session42402 currently builds; production9145 is queued from the exact same input. Correct helper contact_flash_dungeon_run.py uses QA profiling flag0 to match the existing public-UI QA baseline, not the older flag1 parry comparison.
- Preserved independent baseline copy contact-flash-dungeon-baseline-player (all30files SHA256-verified against upstream player-pixel-final) plus snapshot781c from global QA141923, so upstream task cleanup cannot remove the baseline mid-test. Baseline driver contact-flash-dungeon-before PASS24checks,6default noF6 parries/two sizes, Slayer/pause/restart. Counters are recorded, not forced ON; final matched performance must assert equal actual availability/config between these flag0 players.
- Owned bsk session xpyg was stopped after discovering the public change. Before-upload form hash0fcd9eb74423a2969c57c50e89170fbe0c5fd33a4d761f75712cfcf2708c72b9 describes DungeonUI stage but must be refreshed again before final upload. No public mutation by this task yet. Final clips/performance must use DungeonUI baseline and combined player; keep all old physics clips.

### Preview handoff â€” 2026-10-19
User requested no deeper QA and immediate preview. Opened headed Edge using combined DungeonUI + contact-flash QA player (input 48780cca238ebdfff564cb21927eb052de661cc6ae60e331016ca3f401fa5d06), BCompact default, no F6. Preview ready paused; keyboard/mouse only, fixture interception retained. Helper PID 12748; artifacts E:/CodexTemp/TypingHero/parry-clash-lighting/contact-flash-preview-ready. Keep helper/player while preview is in use; close browser to end. No itch upload performed; publication and public payload verification remain pending. Combined quick passed 18/18; no further deep QA launched after user steering.

### Voxel revision after visual rejection â€” 2026-10-19
User rejected the flat disk/ring and selected actual 3D voxel blocks (not thin metallic shavings). Implemented shared closed cube mesh with hard face normals, three-axis spin, 2-3pixel edges, compact offset contact cuboids with no ring, and floor-volume support/rest rotation. Kept launch/flight equations, RNG draws, pulse timing, 24/192 budgets, legacy presets and DungeonUI. No public API/dependency. Current focused Quick20261019T152205 passed19/19 on source8470def229e035afaf4bd833c139d3c103d4c32689a8909f921e5b611f7f1da8; prior attempt failed compilation (struct property assignment), fixed before passing. QA WebGL20261019T152304 building same input. Only two real default noF6 parries/1280+640 and immediate headed preview planned; no deep QA/performance/publication for this revision. Old physics clips retained.

- Voxel QA WebGL152304 PASS. Headed Edge155 preview is ready (helper PID18684, port8141), source8470def2. Native12checks PASS/two real default noF6 parries1280+640;0exceptions/provider reads/writes. Impact/flight images inspected: explicit shaded cube faces and volume, no ring. Keep 1280 window paused for user; Esc resumes, Space parries. Current player voxel-parry-preview-player; artifacts voxel-parry-interactive under task temp. Copied only built voxel mesh/material+metas and prefab; verified prefab equals prior bytes after removing just the three new serialized fields (asset refs/version), with whitespace normalized. No source behavior changed after qualified build. No publication/performance/deep QA claimed. Source branch/worktree retained for visual feedback, not merged or pushed; retain preview helper/player/captures until review ends, then close browser/clean task resources as appropriate. Old physics clips untouched.

### Full 2.5D conversion implementation - 2026-10-20
Approved plan: use prior voxel source/worktree; separate slash-only RGBA into durable layers and closed thin meshes, add Normal6/Slayer8/Lethal14 and received4/lethal6 mesh impacts, keep screen/flash/cues2D. Keep pose/clip/hit/gameplay/audio/camera contracts and old physics clips; no deep/mirror QA/perf/itch publication. Baseline voxel source saved locally as70d789d. Nine source atlases split with cel regions/weapon guards; exact outside-mask RGBA and recomposition checked, source PNG metadata/clip files not changed. Runtime mesh follows actual Sprite cel, not hit window. New impact PRNG is isolated from Unity random; volume support/rest,8leases per emitter,12afterimage cap with generation guards, shared rig normals/heat channel and strongest pulse arbitration implemented. Initial focused24/24 passed; front/back impact sort and received direction then adjusted; focused final rerun queued. New code/art only in child; parent Ronin WIP excluded. Keep task temp E:/CodexTemp/TypingHero/parry-clash-lighting until preview review completes.

- Full source frozen for QA build input9572dd6b7c150c5f5ee651648241835eae3f58ce66d2423a1b9558207b86f6a7 (pending tool session78952). Final focused26case run pending64943; earlier25case run88c5 passed. Delta includes captured Slayer tint across synchronous death reset, suppressing Slayer glow for normal lethal, and a torso contact point because AttackStrike reports attacker root. Body/sprite metadata content verified unchanged after CRLF normalization; the initial raw-byte assertion compared checkout CRLF to Git LF and was corrected to content comparison. Obsolete queued builds/tests were canceled only while sleeping on this task's global lock; no Unity process or other task was stopped. Shared global runner is currently held by another main webgl job; frozen jobs remain queued. Native driver full_voxel_preview.cjs prepared: actual keyboard/mouse, noF6, readonly QA, fixtures/external blocking, four short duels for sword/axe/shield and one640 sample, then a fresh1280 paused preview.

- Focused26/26 PASS on86d1b16bb6f197fd52b1079efc0d2dae82c536e40b8abfc294367c828d221c4e, including lethal tint across synchronous Slayer exit and normal-lethal no emission. Initial final QA023116 on9572 failed the strict builder repeat check: CompactVoxel.mat lazily persisted new shader default _Emission on the second pass. Fixed explicit complete shader property initialization in existing/new materials, persisted current hot material fields, and added CombatVoxel/SlashVolume roots to existing repeat fingerprint (no checks bypassed). Runtime gameplay/geometry did not change after focused tests except received torso-only presentation point. Retrying QA via tool24848; output not yet qualified.

- Retry frozen input55fa1d2284bf42271949119dcab6a9cf29231d19bfa8f8871f10e5228fd70acd via24848 is queued behind shared main jobs. No more runtime edits planned before native evidence. Focused26 PASS is source86d1; final deltas are received torso position and complete shader-default serialization/repeat coverage. Local helpers full_voxel_run.py (ownBASE, shared cache/global lock, preserves new VFX dirs), volume_fetch.py, full_voxel_preview.cjs/open_full_voxel_preview.ps1 all under task temp. New preview uses port8142; no process launched yet. Once build passes: fetch immutable player, copy generated CombatVoxel/SlashVolume plus changed actor prefabs/scenes/hot material only, preserving GUIDs and original texture metadata; run headed driver; inspect captures; reconcile docs; keep1280 preview open. No production/public work.

### Follow the concurrent 3D environment â€” 2026-10-20

User explicitly asked to continue against their new 3D environment. Read sibling `dungeon-3d` WIP on e20c4ab9 and froze only its36 environment source/model/shader/camera files into this child, preserving GUIDs. Sibling and parent remain untouched; unrelated palette/branding WIP is excluded. Imported arena authoring source is `Assets/Editor/PixelUI/DungeonArenaBuild.cs`, original KayKit assets under `Assets/Art/Environment/KayKitDungeon` with SOURCE.json/license, and `Assets/Scripts/Rendering`. This is a scoped preview integration, not acceptance or a main-branch merge of the sibling task.

Local integration adds the final arena builder pass, uses its actual Forward lights for VFX face shading and the same strongest per-pulse profiles for its combat spotlight, and aligns scene emitters to its visible floor. Legacy2D/menu lighting remains supported. New focused real3D-light timing/disable test added. Own obsolete24848 was canceled while waiting on `locks_lock_inode_wait` before creating a Unity child; no other task stopped. Rebuild and immediate actual-input preview are pending; no deep/performance/publication work.

- Current frozen QA source `b79020b658e14bbc93c68a0bc3310d869e37f2aae20f7e7026658438e9cdf7a2`; own tool60223 waits on the shared global lock. Focused27-case tool82975 is queued ahead of it from5f8562c3, with unchanged runtime/tests; final presentation deltas seal adjacent slash-mesh thickness corners, initialize hot-material defaults only at creation to preserve tuning, and include the sibling's latest floor/framing/legacy-light override and Forward-compatible red-overlay fixes. DungeonCombatLighting stays outside the Slayer-hidden geometry group. Static verification: zero duplicate asset GUIDs, all12 KayKit original model/atlas hashes match SOURCE.json, all9 source sprite metadata files match70d789d by content. Parent and sibling WIP remain untouched.

- Quick025745 and QA025824 stopped at compilation, zero tests executed: imported DungeonArenaBuild directly references Cinemachine but the editor asmdef lacked its existing package reference. Added the same `Cinemachine` reference now present in the sibling's editor asmdef, with no package/version change. Prior26 passing VFX tests remain historical; final27/combined WebGL are retried. No visual acceptance assigned to these failed builds.

- Active final jobs: focused27 tool85389 on `cddebed9d46c512da9204e9272c4173d844a7e029cba3d209fe7cd22bc7879a1`, WebGL build-only tool73006 on `c639e3d9eaca0c146d847ac0dfe6e454a5e0adc1f0a4eaa08f90f93e55c20d79`. The sole input delta is the sibling's stone tint `.11/.12/.13` (presentation builder only). Old tool52938 was canceled only while queued. Both final jobs wait behind global serialize after main production025919; no running job from another task was interrupted. Ready-to-use helpers: volume_fetch.py, fetch_full_voxel_assets.py, open_full_voxel_preview.ps1/full_voxel_preview.cjs. The browser driver now waits for the real hurt lock to end before typing its first normal hit, then leaves a fresh1280 fight paused for review.

- Quick030739 PASS27/27 on cddebed9. QA030935 failed strict repeat at DungeonStone.mat. Diagnostic-only serialize031638 (`e7b4dbc6ae9659571457dd01622560ae95ec0608f7f54606379f2690e0a7811e`) dumped both material passes: the only difference was legacy `_Color` white becoming the Lit `_BaseColor` via URP import validation. Fixed explicit `_Color = GetColor(_BaseColor)`, `_MainTex` alias and material dirty persistence; strict checks remain enabled, with .mat dumps added for diagnosis. QA032020 (`094aba33607b7178a478a0effc956e2654b323cfac1cce3d1c97773ee1013e01`, tool35360) passed repeat and is creating its player. A stale first Bee compile reported VoxelImpactBurst missing, then reevaluated/recompiled and proceeded to IL2CPP; only final status counts.

- Detected a separate imported-environment defect before visual acceptance: generated stone BaseMap/MainTex references were null while the original valid1024x1024 PNG decoded correctly. Original atlas metadata contained only its GUID. Added explicit Texture2D/sRGB/Point/Clamp/no-mips import metadata using the existing Unity-generated importer format, preserving GUID54fcf6fd51724a4b8c2776ebeb709ffd and original PNG bytes, and an editor error if atlas import is absent. Final QA tool47416 is queued from `757f20fb2074ae8641221db06b80fd410ff9a18b913190c87b81a3acb47988f2`; runtime/tests unchanged from passing27. Do not qualify/publish032020's untextured scene. After final PASS, verify actual non-null atlas binding, fetch player/serialized assets and run the short native preview driver. Parent/sibling still untouched.

- QA032020 completed build/repeat successfully but remains superseded/untextured. Final QA033055 is linking from757f20fb: strict repeat PASS, no missing-atlas editor error, actual BaseMap fileID2800000/GUID54fcf6fd51724a4b8c2776ebeb709ffd confirmed in generated DungeonStone.mat. Only final tool47416/player is for native visual review. Full generated assets must be copied with fetch_full_voxel_assets.py after success, preserving generated GUID pairs; then native preview output/player should use distinct `full-voxel-dungeon-*` names on port8142. No new runtime tests, deep QA or production steps needed for these editor/import-only fixes.

### Full conversion preview ready â€” 2026-10-20

- Final QA033055 build/strict repeat PASS on757f20fb; immutable player fetched as `E:/CodexTemp/TypingHero/parry-clash-lighting/full-voxel-dungeon-preview-player`. Copied112generated asset files with GUID checks. Source retains all407original menu/853combat IDs,24slash mappings and original9sprite metadata; zero duplicate GUIDs. MainMenu generated changes were only trailing whitespace, so restored its exact baseline bytes. Package/project version and gameplay/audio/clip contracts remain.
- Edge155/Playwright1.58.2 `full-voxel-dungeon-interactive-r4`:70scoped checks passed, preview-ready, zero exceptions/fixture writes/F6 keys. Four real opponents Swordsman/DoubleStrike/Heavy/Guard at1280, accepted-hit/normal/parry/Slayer/lethal particles, held pause ages/positions, Continue cleanup and640Slayer. Contact/chunks/slash pose, axe grip/trail, lethal tint, silhouettes and640 sample inspected. No human visual approval, deep/mirror/performance/publication claim.
- Earlier native r1-r3 each reached17checks then failed driver assumptions at Continue. Source shows Run.Win advances the label before Continue; reloaded continuing scenes auto-start combat. Spark records cache the previous frame, observed p.frame1766/state.frame1767 on r3. Driver now waits for nonfinished/live HP and two fresh frames before checking empty effects. R4 completes with no product edits between native attempts.
- Review window is a fresh1280fight paused; Esc resumes, type words to attack, Space parries. Wrapper PID32008, port8142. Retain current preview helper/player, `full-voxel-dungeon-interactive-r4` screenshots/progress/summary and old physics clips until user review completes. Keep child `micharel09/parry-contact-flash` unmerged for source review; parent/sibling untouched. No further acceptance work is required for this requested immediate-preview iteration; visual feedback drives any next change.

- Interactive review page closed at10:51local after a136s paused review window; final R4 report is PASS72/72 (70behavior checks plus2harness postconditions),0exceptions/fixture reads/writes/external requests,250.243s total. Wrapper32008 and server8142 exited normally; they are no longer live preview resources. Keep final player/screenshots and `open_full_voxel_preview.ps1` launcher/driver for any requested reopen. Old Strong24 clips rehashed and match original6405ff54e05fec76103dad202934cdfe5a7d2e591ec7933ce867f9af480fd6fb100 /128070df40579895bf543eef8ac265c00bb75725de01d8d3842f15584a64f87486a9. Source remains a reviewed local candidate, not published or merged; final visual approval remains with user.

- User-requested video follow-up (2026-10-20): exported `E:/CodexTemp/TypingHero/parry-clash-lighting/CLACKBLADE-VFX3D-Dungeon-Preview.mp4`, 29.68s/1280x720/25fps/H.264/yuv420p, 2994682bytes, SHA256 `d560c11aba2ff7ac41a1bf3db74f8329b9ea45b8578290045c6e9edd081df03e`. Existing qualified player/input757f20fb at runtime source ebdd386; no rebuild or runtime edit. One real keyboard/mouse Swordsman duel shows received/normal/default BCompact parry/Slayer/lethal, followed by0.25x replay of the first parry and falling fragments. Native headless Edge155.0.4283.62, ANGLE GTX1650Ti/D3D11, isolated provider fixtures/read-only bridge,13capture checks passed and0exceptions/writes/external requests. First headed take passed behavior but had gray recording corruption at its end and was excluded. Final export fully decodes; inspected normal/parry/Slayer/lethal/slow falling frames have full frame, readable labels/silhouettes and no gray capture corruption. Video is silent; no new deep QA/performance/publication claim. Retain final MP4, lightweight provenance/clip receipt and recording/export helpers under task temp for user review; closed recording profiles, discarded first take, raw WebM and contact sheets can be removed. Prior physics clips remain unchanged. No human visual acceptance claimed.


### Cinematic pixel parry implementation â€” 2026-10-20 (ready for visual review)

- User approved the detailed hybrid parry plan: use the licensed free Pimen01 already present, pixel flipbook + stock 3D ParticleSystem motion/collision + local light, restrained camera/audio. Parry only; preserve normal/received/Slayer/lethal art and gameplay timings. Local preview with sound requested; no push/merge/itch publication. Prior voxel visuals were rejected, irrespective of passing historical checks.
- Reusing child micharel09/parry-contact-flash at C:/Users/ADMIN/orca/workspaces/TypingHero/parry-contact-flash, base ddad604. Parent/sibling environment WIP untouched. Replaced analytical voxel BCompact path with 14 main +4 reflected rays +4 embers +2 fixed-pivot Pimen contact particles; authored contact data snapshots before Hurt/success. Added HDR/bloom4/.2/.3 and a parry Point light, separate impulse channel2 after existing50ms hitstop; parry clang jitter .03/.35 semitone and80ms hand duck. Source art/importers remain unchanged.
- First Quick051633 stopped at compile: Cinemachine2.9.7 Cancel requires(time,bool), corrected using installed package source. Quick051811 executed25 cases,10pass/15fail: all failures came from trying to AddSprite(null) while restoring the disabled legacy flipbook placeholder. Changed contacts to grid animation and restore tile/curve fields without touching the original sprite list. New focused Quick and representative WebGL/repeat build are queued behind the shared runner; no acceptance assigned yet.
- Baseline headless native Edge155/GTX1650Ti/D3D11, Playwright1.58.2, existing qualified757f20fb player: actual keyboard Swordsman duel captured with canvas30fps + game WebAudio (no microphone),13checks passed; source MP4 from the old iteration remains untouched. New raw is task temp/pixel-parry-before-video/game-audio.webm. No-capture baseline1280 p95=13.435ms,n646/live341 p95=13.420;640 p95=13.510,n640/live350 p95=13.515. These are short RAF observations, not a device performance guarantee.
- Retain E:/CodexTemp/TypingHero/parry-clash-lighting helpers/players/reference clips and all original Strong24 physics clips for review. Pending: new runtime/build qualification, actual contact gap/pose and glow checks on sword/shield/axe, pause/reuse/shared consumer smoke, matched p95, continuous audio MP4 at1x/.25x and final receipts. No deep/stress/soak/mirrored QA.

- Grid restoration Quick052747 (input4ff89cec) PASS25/25. Representative QA052927 (d673400c) build/strict repeat PASS; fetched immutable pixel-parry-stage1 player and24 generated files with GUID checks. Native sample shows a real presentation defect: animation events see Attack1_4 before its contact cel is applied; the success/Hurt poses replace weapon contact immediately. Fixed by explicit authored contact-cel mapping and70ms reuse of existing contact sprites in ParrySystem.LateUpdate; gameplay animator states/events/50ms hitstop continue unchanged and pause/disable restore the actual animation sprite. No source pixels/clip timings were edited.
- Additional corrections from that sample: velocity-facing billboard rays could be edge-on, now use stock Stretched Billboard; HDR8 did not clear URP bloom's gamma threshold4, contact multiplier raised to32 while retaining threshold4/intensity.2/scatter.3. The additional-light enum diagnosis was initially wrong: installed URP source confirms PerPixel=1 and PerVertex=2. Existing mode1 is already correct; builder now explicitly retains PerPixel. Actual missing dependency is DungeonForward.postProcessData=null; it now binds the installed URP Runtime/Data/PostProcessData.asset so Bloom can execute. HDR32 was premature and is reverted to the planned8 for the first correctly configured post-process preview. Shared normal/lethal smoke is required on final player. First native sample reached15 checks then a driver assertion compared HP from before a long cue wait; the driver now compares HP immediately before Space. No art acceptance assigned to stage1.
- Audio capture failure resolved: Unity disconnects/reconnects destination nodes; a WeakSet dedup guard prevented reconnecting the parallel capture route. Removed that guard following existing typing-sfx audio-tap behavior. Qualified baseline native capture pixel-parry-before-audio-r3:12checks, real stereo48k PCM peak.489592, native25fps1280x720; muxed pixel-parry-before-with-audio.mp4 has H264+AAC. Failed empty/silent MediaRecorder/PCM takes are unqualified and will be cleaned. Final uses the proven silent-output PCM tap plus Playwright video, not the unsuccessful combined MediaRecorder route; no microphone or production change.
- Current final source adds explicit Point-vs-Spot strongest-winner coverage (26 focused cases). New Quick/build jobs launched via parry_cinematic_run.py after source/data fixes; final geometry/physics/light/camera/audio preview and matched no-capture p95 remain pending. Retain stage1 screenshots only until the correction is reviewed; no publication.

- Stage1 native-r2 reached47 checks, confirmed sword1 gap0, sword2 centerline gap.207WU, axe1 sampled-shaft gap.341WU; stopped during Guard because the old driver waited for a normal effect after a short word/lowHP. Updated driver to smoke normal/received only once, parry other roles immediately and type Slayer letters faster. Use immediate post-Space HP rather than pre-cue HP. No accepted result for the failed run.
- Authored contact library v3 uses existing lower guard cel1 for horizontal Attack2, and axe1's actual metal cutting edge pixels(78,11)->(83,6) rather than the shaft-side line. This avoids new drawing and closes the visible contact without altering clip/frame timing. Contact poses are held70ms for presentation only. All actor/source image bytes stay unchanged.
- Canceled own queued obsolete snapshots after these discoveries. One Quick055105 had just started when the TERM arrived; its owned orphan container typinghero-loop-1487163-1792475465583994444 was then explicitly removed and absence confirmed. That run is unqualified. Other jobs/containers remain untouched. Final current Quick/build are tools58738/16608; wait for their frozen receipts. No further source edits before their result unless a concrete failure requires it.

- Quick055203 PASS26/26 on95dc7692, including v3 contact snapshot, stock physics/pause/pool/preset restoration, mixed point/spot arbitration and all legacy consumers. Corrected another concrete texture timing issue from Unity2022.3 API docs: startFrame is a tile index (1), whereas frameOverTime remains normalized(5/6); the earlier1/6 delayed the outer sequence. Runtime/test delta is this literal plus its comment/assert. Final Quick46187 and QA75999 are queued on that corrected source; no more edits before evidence. QA055327 on95dc is the representative pose/post-process player and remains running (tool16608), useful for shared weapon/light smoke; it is superseded for final flipbook timing.

- Important correction: Quick060043 on98f7986f ran25/26; the failed assertion shows the native startFrame setter clamps1 to.9999. The API example's tile-index interpretation was wrong. Reconstructed exact95dc bytes from98f archive by reverting only that literal/comment/assert, and restored the exact passing runtime/tests. No tolerance widening. QA060130 on98f is a negative-control/unqualified player, never use it for preview. A temporary BakeMesh UV diagnostic is queued with a unique hash-named input to verify actual cell UVs, not just settings. Its earlier wrapper1536418 actually completed before the cancel attempt; it ran0filtered cases because the shared quick input path had been overwritten by the newer queued snapshot. That result is unqualified. Requeued the diagnostic with its own immutable hash-named archive.
- Native source95dc final-qa PASS64/64 after driver corrections: four opponents Swordsman/Double/Heavy/Guard at1280, defaultBCompact/noF6, accepted normal/Slayer/lethal, pause and Continue cleanup, plus640Slayer. Failure root cause in previous finish loops: typing only current[0] in Slayer repeatedly reset a longer word and cannot farm already rewarded positions. Driver now types each complete current word; gameplay was not modified to satisfy automation. Native scoped checks stop after this acceptance set, no deep/mirror/stress work.
- No-capture95dc same Edge155/GTX1650Ti/D3D11/Playwright1.58.2:1280 p95 13.435->13.445ms(n646->642), live13.420->13.450(n341->316);64013.510->13.470(n640->647), live13.515->13.445(n350->316). No>10% regression, no ABBA. Short RAF observations only, not a hardware guarantee.
- Actual audio+video intermediate exported as task-temp/CLACKBLADE-Parry-Pixel-Preview-WIP.mp4 (normal1x +.25x), shared for user preview. Source pixel-parry-after-audio-stage2 passed12capture checks, actual stereo PCM and native25fps video; no microphone. This source is95dc, before the final trail cleanup below.
- Visual review found the original enemy slash sweep obscures the player during axe parry. Final implementation reuses finished clean weapon cels (lossless crops; sword from existing free Without_VFX sheets, shield/axe from clean follow-through cels) in PixelWeaponContacts.png, library version4. Separate sprite identities naturally avoid SlashLibrary's attack sweeps; ParrySystem also suppresses only the parried enemy's SlashVolume for200ms and restores its prior enabled state after that, on disable/results or before another parry. Normal/Slayer attack trails and all animator/event/gameplay timing stay. No original actor pixels, metadata or clips edited.
- Final v4/trail source Quick65592 and WebGL97588 are queued; prior v4-before-trail wrappers56865/78593 were canceled with INT while queued. Current UV diagnostic was requeued separately with immutable hash filename. Final generated assets/player, native follow-up, AV clip/before-after export, p95 and receipt still pending. Keep WIP clip/player for review until final qualifies. Do not publish/merge/push.

- Final clean-contact source2158fcdf07c678134da9bc716996019c855294d2e1be57fe996ebf8a6327410a: Quick063136 PASS26/26; WebGL063220 build/strict repeat PASS. Fetched immutable parry-cinematic-final player and10generated files, preserving GUIDs. All6 PixelWeaponContacts cells match their original RGBA crops exactly; profilev4 confirmed. No original actor PNG/metadata/animation change.
- Native parry-cinematic-final-qa: PASS66checks, same Edge155/GTX1650Ti/D3D11/Playwright1.58.2, actual keyboard only/defaultBCompact/noF6, four opponents, normal/Slayer/lethal/received smoke, pause/Continue cleanup,640Slayer. Zero exceptions/provider mutations observed; fixtures/external blocking retained. Actual contact targets are ParryWeaponContact0/1/4/2/3; measured centerline gaps sword1=0,sword2=.084WU,axe1=.071WU,shield1/2~0 (within visible blade thickness). Inspected final axe contact sample: original large enemy sweep no longer covers the player; existing player's success-counter trail remains. Human aesthetic approval still belongs to the user.
- Final no-capture `parry-cinematic-final-perf` PASS14, same Edge155/GTX1650Ti/D3D11/Playwright1.58.2:1280 p95 13.435â†’13.485ms(n646â†’633), live13.420â†’13.520(n341â†’309);64013.510â†’13.505(n640â†’646), live13.515â†’13.505(n350â†’314). Short RAF observations only; no>10% increase, no ABBA. Final `parry-cinematic-final-audio` PASS12, actual stereo48k PCM peak.516182 and native25fps video. Three real parries at1x plus0.25x replay exported to `CLACKBLADE-Parry-Pixel-Cinematic.mp4`, H264/yuv420p+AAC, full decode PASS. Final glow/contact/tail frames inspected; sampled visual onset9.964s, not the late driver marker10.204s. No runtime changes after qualification.
- Optional UV diagnostic96282 completed Quick065007 on unique6bf8d739 snapshot,1executed case. BakeMesh minU confirms startFrame1/6 begins tile1 and advances through the outer sequence;0begins coretile0, .9999begins tile5/wraps. Final normalized1/6 is retained. Temporary diagnostic is not in source. WIP95dc and failed98f are historical; use2158/player for preview.
- Final before/after `CLACKBLADE-Parry-Pixel-BeforeAfter.mp4`:20.021328s,1280x720/25fps,H264/yuv420p+AAC48k stereo,1295591bytes,SHA256 `f4392f7151ba6521d59825a9e4a0b6958681299a693e3dd5bb3d53415e169629`. Prior757f20fb voxel and final2158 raw frames aligned at sampled visual onset9.418/9.964s, not delayed driver marks; before/after1x then before/after.25x. Full decode and full-scene frame/label/aspect review PASS; separate actual runs, not RNG-locked. Main final `CLACKBLADE-Parry-Pixel-Cinematic.mp4`:16.501328s,same codecs,1113280bytes,SHA256 `ac9b996339e30ea230306283d3074b47564fc625f738f4ffdf71b7d60bc5475a`. Metadata at task-temp/parry-cinematic-comparison/receipt.json. AV capture scope descriptions corrected from the old multi-impact template to the actual three parries/tail; event records/checks unchanged.
- Headed native1280 interactive preview ready paused on source2158/defaultBCompact/noF6: `http://127.0.0.1:8142`, wrapperPID25836/node13812. Esc resumes, Space parries, type the current word to attack. Launcher `E:/CodexTemp/TypingHero/parry-clash-lighting/open_parry_cinematic_preview.ps1`, state/screenshot in `parry-cinematic-interactive`. Closing the review page ends its browser/server; retain the final player, current clips/reports/launcher and original physics clips while review remains active. Nine known failed-capture/closed-profile directories removed after path/reparse checks; no user/toolchain/other-task resources touched. Source stays in child `micharel09/parry-contact-flash` for review, unmerged/unpushed; parent/sibling untouched. Visual acceptance is still the user's decision.
- Final static review:49scoped files, no package/project/actor-source/clip edits, no duplicate GUID or missing new meta. Whitespace trimmed only in four new Unity script metas (GUIDs/settings unchanged); staged diff whitespace and affected local provenance/document links pass. Runtime/build receipts are reused because only documentation and nonsemantic meta whitespace changed after them. Local commit contains the verified source/generated assets and reconciled memory, with preview artifacts kept outside Git.

### Restore approved flash and continuous physics - 2026-10-20 (verified local preview)

- Reuse Orca child e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/parry-contact-flash;
  parent C:/Users/ADMIN/orca/TypingHero, branch micharel09/parry-contact-flash,
  integration target main, remote https://github.com/micharel09/TypingHero.git.
  HEAD c12c3de is the approved pixel flash. The uncommitted cinematic48 revision is
  explicitly discarded; its patch and all prior review clips remain in task temp.
  Parent/sibling WIP and 3D dungeon/other combat effects are preserved.
- Restore exact c12c3de Pimen cels/material/pivot/size/alpha/200ms timeline,
  Point5/70ms hold/280ms end, recoil1pixel/100ms, existing sound and hitstop.
  Transplant cc06385 analytical launch/brake/fall/single-bounce/rest180ms paths.
  Five systems now emit1/2/10/1/10,24total/192over8leases. Old Back goes to Embers;
  the Back system is reserved for the approved Pimen outer sequence. Existing
  Compact material/pixel masks and real Z/character/environment depth tests remain.
  Local PRNG replaces Unity random; no asset/dependency/GUID/public API changes.
- Lifetime of20falling particles derives from the trajectory, bounce and rest;
  two short fragments retain120-180ms. Fixed22states/10scratch/reused Custom1 data.
  Completion waits for both contacts and debris, ignoring IsAlive on frozen systems.
- Focused offline tests and one QA build are pending behind another task's runner
  lock. A superseded queued snapshot a02e418d was canceled before Unity started;
  final fixtures use a2WU-high contact for the420ms survivor assertion.
  Scoped native sword/axe+Slayer at640/1280, short p95, actual sound1x/.25x clips
  and reopened preview remain. No mirrored/deep/stress/soak/publication scope.
  Keep E:/CodexTemp/TypingHero/parry-clash-lighting/physics-pixel24 and existing
  baseline players/clips while visual review remains active. Local commit follows
  verified implementation; no push/merge/deploy.

- User requested faster preview before the remaining checks. Cancel the queued
  focused Quick552f job while it is blocked on the global lock; prioritize the
  already-queued QA build552f, then open a headed1280 review window immediately.
  Do not wait for the complete sword/axe capture or p95 comparison before preview.
  Initial1356protected serialized/art/package/project files match c12c3de;
  27prior clips fingerprinted for preservation. Native baseline p95 passed14checks.

- Final552f build: WebGL082208 and strict repeated-builder PASS; focused
  Quick08330230/30PASS,0failed/skipped. Preview was opened immediately after build
  at1280/paused/default24, before the native/capture/performance checks. Launcher
  physics-pixel24/open-preview.ps1; loopback8152, wrapper19156/node13728 at launch.
  Esc resumes, Space parries, typing attacks. Closing the page ends its helper.
- Runner remained scoped to task source/cache and the existing isolated pixel
  Library after verifying other Unity mounts. The global queue was replaced only
  in the temporary helper by its original task mutex and a resource gate (max two
  observed active Unity containers, at least8GiB available); task stages stayed
  sequential. No repository runner/service/toolchain changes; other jobs untouched.
- Native qa-r2 PASS83/97.185s,18keyboard parries, sword/axe/Slayer at640/1280,
  pause and Continue cleanup; zero exceptions/provider reads/writes/requests.
  Failed first capture5checks is a helper variable TDZ, retained as failed; fixed
  only that script, no product changes after the qualified build. Raw game PCM
  peaks.557749/.587604; no microphone. Separate perf14PASS:1280/640 all-state
  p9513.455/13.540ms, live13.520/13.575. Baseline13.505/13.560 and13.515/13.575;
  review window was open during the new run, so no strict equivalence claim.
- Inspected actual full contact-to-floor/bounce/rest/expiry and Slayer at both
  sizes. Approved flash visual source2158/c12c3de and Strong24 physics reference
  were compared; exact flash properties/assets remain source- and test-backed.
  Video is native25fps/40ms sampling and separate RNG/pose phases, not exact replay.
  Two42.021328s actual-sound clips contain sword,axe and axe-Slayer at1x/.25x,
  H264/yuv420p+AAC48k stereo; full decode PASS. All27prior clip hashes preserved;
  1356protected input contents match c12c3de after newline normalization.
- `CLACKBLADE-Parry-Physics24-1280.mp4`: 2848849bytes, SHA256 `80d46079022ab8ecc5185f05c0f3364368633b2bca7320be66c9af701136b147`.
- `CLACKBLADE-Parry-Physics24-640.mp4`: 1206212bytes, SHA256 `ac5beeaa33344112e45125df3dae055eecb3c84347f79fbf05aa3fca17497e4e`.
- Implementation, scoped verification and requested clips are complete. Keep
  the existing unmerged child and physics-pixel24 player/launcher/clips/receipts
  while user visual review remains active; clean the review resources after that
  and retain the branch until verified integration. Old reference clips stay.
  Local commit contains only this task; no push/merge/publication.
