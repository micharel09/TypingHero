# Combat feedback — implementation checkpoint

Current task: Windows x64 portable build implemented and verified; final ZIP in Downloads.
See the Windows continuation below; earlier checkpoints
describe the preserved combat-feedback baseline and resolved intermediate issues.

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
- Kenney Impact Sounds CC0: impactWood_medium_000–002 for normal/Slayer,
  impactWood_heavy_000–002 for lethal. Separate SfxEvents with .65/.45/.8 volume,
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

## 2026-10-17 checkpoint — A blocked on original assets

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
- Mapping: existing Shogun theme/UGUI factory, gold cue and gameplay layout →
  TutorialBuild/Overlay below-HUD prompt and targeted highlights. Selected short
  instruction / live arena practice reference is the user-supplied Shogun Showdown
  opening URL. Fetch failed (cache miss); exact flow is specified by the plan.
  Wireframe records approved placement and routine theme composition.
- Implementation decision: scene reloads isolate ordinary lesson transitions and
  retries while TutorialRun retains the in-session checkpoint. Stamina-break →
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

## Pixel physics sparks — foundation checkpoint (2026-10-18)

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
