# Enemy state VFX ? 2026-10-23

## Contract / checkpoint

Implement GuardBreak at accepted zero-stamina stun start, once per stun episode;
BodyFallDust at first torso/floor contact, once per death. Existing stock art only,
no generated/painted pixels, gameplay values, sound, hitstop or camera changes.
Worktree: `micharel09/feat-enemy-state-vfx`, base `1f22565e3124`.
Parent dirty UI/branding checkout is preserved.

Profiles use the generic ImpactVfxEmitter. New flash heightScale defaults to1;
square MetalClash path is retained. New completion policy defaults to stop on
combat end; only death dust opts into survival until results. Result/pool/scene
cleanup and pause remain required. Scene owns dust leases independently of actor.

## Source acquisition and direct inspection

- Guard: existing Pimen Battle Effects Effect2,64x160, five64x32 rows,
  four visible cels and transparent tail. CC BY4.0, existing attribution retained.
- Dust: original author free download https://pimen.itch.io/smoke-n-dust-03,
  acquired with browser-skill; browser download reported cross-frame attribution
  failure but actual9034byte archive arrived in Downloads; copied/read without
  retry. Selected VFX5 originalPNG256x32, **six visible plus two transparent cels**;
  page manifest counts visible cels only. Retain all8cells,30ms each,total240ms.
  Source PNG unchanged; hashes/permission in SmokeNDust03/LICENSE.txt.
- Output/source review artifacts under E:/CodexTemp/TypingHero/enemy-state-vfx.
  guard-source-review.png and dust-source-review.png are nearest-neighbor review
  composites only, never imported as game art. Death source sheets: sword,
  sword-shield,axe,legacy-death-source.png. These sheets do not establish runtime PASS.
- Reviewed source contact: sword/shieldF7(.7s),axeF5(.5s),legacyF7(.48333332s).
  Later skull contact is not another impact. Existing cleanup times/keys retained.

## Verification status

Implementation complete. Scoped behavioral, authoring and native frame checks pass;
remote integration is the final delivery step. Initial serialization failure
(restoring an empty scene setup) was fixed and the authoring retest passed.
Final evidence and fixture limits are recorded below.

Source importer discovery: original Battle Effects Effect2 has textureShape2
(cubemap); AssetDatabase correctly returns Cubemap, not Texture2D. Preserve
original PNG/meta/GUID and import a byte-identical GuardBreak-Sheet.png copy
as2D. New dust importer is2D. No pixel generation or recoloring.

## Direct source frame review (art unchanged; runtime reviewed separately)

Rows below were filled after inspecting the numbered nearest-neighbor source
sheets. Status covers the source cel and adjacent source transition only.
All source cels are retained; no loop. Both runtime facings are covered by the final output review below.

| Source | Frame | Duration | Observed cel | Next transition | Source status |
|---|---:|---:|---|---|---|
| guard | F0 | 30ms | White core inside small yellow star | Cross grows outward | inspected |
| guard | F1 | 30ms | Broad yellow cross, white center and separated small shards | Center breaks into diagonal white fragments | inspected |
| guard | F2 | 30ms | Open cross with white diagonal fragments and yellow tip shards | Cross dissolves to sparse dots | inspected |
| guard | F3 | 30ms | Small yellow/white dots, no solid core | All pixels disappear | inspected |
| guard | F4 | 30ms | Transparent tail | End, no loop | inspected |
| dust | F0 | 30ms | Compact gray wedge and rising pointed plume; common base row25 | Separates into two lobes | inspected |
| dust | F1 | 30ms | Two gray lobes above broad base patch | Lobes fragment while base broadens | inspected |
| dust | F2 | 30ms | Broken lobes and low scattered base | Plume thins to separated outlines | inspected |
| dust | F3 | 30ms | Thin scattered plume and broken floor-level segments | Fewer floating pieces | inspected |
| dust | F4 | 30ms | Sparse gray fragments with open center | Shrinks to small dots | inspected |
| dust | F5 | 30ms | A few separated gray flecks | Disappears completely | inspected |
| dust | F6 | 30ms | Transparent | Transparent hold retained | inspected |
| dust | F7 | 30ms | Transparent | End, no loop | inspected |
| sword Die | F0 | 100ms | Standing, intact torso and sword raised. | Head lifts | inspected |
| sword Die | F1 | 100ms | Head rises above intact chest. | Head rises again | inspected |
| sword Die | F2 | 100ms | Head higher; arms/chest intact. | Chest/hips separate | inspected |
| sword Die | F3 | 100ms | Separated chest and hips, sword angles down. | Hips descend | inspected |
| sword Die | F4 | 100ms | Hips lower, legs buckle. | Knees reach ground | inspected |
| sword Die | F5 | 100ms | Legs flattened; ribs still elevated. | Ribs drop toward legs | inspected |
| sword Die | F6 | 100ms | Ribs just above floor, sword almost horizontal. | Ribs contact floor | inspected |
| sword Die | F7 | 100ms | Ribs on floor, head still above; first torso contact. | Head descends independently | inspected |
| sword Die | F8 | 100ms | Head lower, ribs remain grounded. | Head reaches ground | inspected |
| sword Die | F9 | 100ms | Head grounded beside ribs, sword flat. | Head rolls sideways | inspected |
| sword Die | F10 | 100ms | Head rolls left, ribs and weapon stay grounded. | Head settles | inspected |
| sword Die | F11 | 100ms | Head settles, body stays flat. | Held final pose | inspected |
| sword Die | F12 | 100ms | Final settled pose. | Cleanup; no loop | inspected |
| sword-shield Die | F0 | 100ms | Standing, intact torso and sword raised. Hat stays with skull; shield remains left of torso. | Head lifts | inspected |
| sword-shield Die | F1 | 100ms | Head rises above intact chest. Hat stays with skull; shield remains left of torso. | Head rises again | inspected |
| sword-shield Die | F2 | 100ms | Head higher; arms/chest intact. Hat stays with skull; shield remains left of torso. | Chest/hips separate | inspected |
| sword-shield Die | F3 | 100ms | Separated chest and hips, sword angles down. Hat stays with skull; shield remains left of torso. | Hips descend | inspected |
| sword-shield Die | F4 | 100ms | Hips lower, legs buckle. Hat stays with skull; shield remains left of torso. | Knees reach ground | inspected |
| sword-shield Die | F5 | 100ms | Legs flattened; ribs still elevated. Hat stays with skull; shield remains left of torso. | Ribs drop toward legs | inspected |
| sword-shield Die | F6 | 100ms | Ribs just above floor, sword almost horizontal. Hat stays with skull; shield remains left of torso. | Ribs contact floor | inspected |
| sword-shield Die | F7 | 100ms | Ribs on floor, head still above; first torso contact. Hat stays with skull; shield remains left of torso. | Head descends independently | inspected |
| sword-shield Die | F8 | 100ms | Head lower, ribs remain grounded. Hat stays with skull; shield remains left of torso. | Head reaches ground | inspected |
| sword-shield Die | F9 | 100ms | Head grounded beside ribs, sword flat. Hat stays with skull; shield remains left of torso. | Head rolls sideways | inspected |
| sword-shield Die | F10 | 100ms | Head rolls left, ribs and weapon stay grounded. Hat stays with skull; shield remains left of torso. | Head settles | inspected |
| sword-shield Die | F11 | 100ms | Head settles, body stays flat. Hat stays with skull; shield remains left of torso. | Held final pose | inspected |
| sword-shield Die | F12 | 100ms | Final settled pose. Hat stays with skull; shield remains left of torso. | Cleanup; no loop | inspected |
| axe Die | F0 | 100ms | Standing, axe vertical. | Torso separates | inspected |
| axe Die | F1 | 100ms | Head/chest separate above hips. | Hips descend | inspected |
| axe Die | F2 | 100ms | Hips lower and legs buckle. | Legs reach floor | inspected |
| axe Die | F3 | 100ms | Legs flat; ribs elevated; axe stays upright. | Ribs drop | inspected |
| axe Die | F4 | 100ms | Ribs near floor, head/axe above. | Torso reaches floor | inspected |
| axe Die | F5 | 100ms | Ribs grounded; head above; axe tilts across head. | Head and axe descend | inspected |
| axe Die | F6 | 100ms | Head lower; axe falling diagonally. | Head contacts floor | inspected |
| axe Die | F7 | 100ms | Head grounded, axe lies horizontally. | Head rolls | inspected |
| axe Die | F8 | 100ms | Head rolled beside ribs, axe remains flat. | Head settles | inspected |
| axe Die | F9 | 100ms | Settled corpse and horizontal axe. | Held final pose | inspected |
| axe Die | F10 | 100ms | Final held corpse. | Cleanup; no loop | inspected |
| legacy Die | F0 | 66.667ms | Standing, intact torso and sword raised. | Head lifts | inspected |
| legacy Die | F1 | 66.667ms | Head rises above intact chest. | Head rises again | inspected |
| legacy Die | F2 | 83.333ms | Head higher; arms/chest intact. | Chest/hips separate | inspected |
| legacy Die | F3 | 66.667ms | Separated chest and hips, sword angles down. | Hips descend | inspected |
| legacy Die | F4 | 66.667ms | Hips lower, legs buckle. | Knees reach ground | inspected |
| legacy Die | F5 | 66.667ms | Legs flattened; ribs still elevated. | Ribs drop toward legs | inspected |
| legacy Die | F6 | 66.667ms | Ribs just above floor, sword almost horizontal. | Ribs contact floor | inspected |
| legacy Die | F7 | 66.667ms | Ribs on floor, head still above; first torso contact. | Head descends independently | inspected |
| legacy Die | F8 | 83.333ms | Head lower, ribs remain grounded. | Head reaches ground | inspected |
| legacy Die | F9 | 66.667ms | Head grounded beside ribs, sword flat. | Head rolls sideways | inspected |
| legacy Die | F10 | 66.667ms | Head rolls left, ribs and weapon stay grounded. | Head settles | inspected |
| legacy Die | F11 | 66.667ms | Head settles, body stays flat. | Held final pose | inspected |
| legacy Die | F12 | 16.667ms | Final settled pose. | Original cleanup event starts this final cel; runtime visibility checked separately | inspected |

Source coverage: guard5/5cels,4/4pairs; dust8/8cels,7/7pairs; sword13/13+12/12,shield13/13+12/12,axe11/11+10/10,legacy13/13+12/12. These are source observations, not native render acceptance.

Source SHA256 Assets/Art/VFX/EnemyState/GuardBreak-Sheet.png: `851deb070970947c8ff68a1aa22fe8078d37c2d069abd18308294c3ea4762dec`.

Source SHA256 Assets/Art/VFX/Pimen/SmokeNDust03/SmokeNDust P03 VFX 5.png: `34521772ffe22b0efd0b8c60512a3f99579f3fb63877e8ceb56ae9312967cb0c`.

## Technical verification checkpoint

- Editor authoring test1/1PASS: repeated builder bytes/GUID references unchanged,
  scene owns independent two-lease dust emitter; actual scene collider floor is
  bound once independently of the prefab transform. Results085028-quick.
- Focused PlayMode48/48PASS,0failed/skipped,085345-quick, source fcb3f1e46045.
  Includes real hitbox/parry drain, lethal exclusion, pause/result/owner teardown,
  alternate profiles/snapshots/aspect, frozen MetalClash every-step baseline and
  directly affected legacy death receiver. Actual Slayer coverage is recorded below.
- Unity scene serialization reordered existing YAML documents/blank whitespace.
  Retained original scene order/IDs and transplanted only the3changed existing
  VFX ownership/override blocks and3new emitter blocks. No existing object removed;
  semantic differences outside this task are zero. Clip diffs add only events.
- First graphics fixture090115-quick FAILED after two sword directions: shield
  first-render shader compilation stalled software Mesa and exhausted a wallclock
  capture window before torso contact (trace0.05 ->0.682 ->2.092s). Preserve this
  failed result and source. Fixture now captures at Unity fixed60fps clock and
  runs until its full authored sequence ends. Runtime VFX clock is unchanged;
  this is deterministic image evidence, not device performance/cold-start proof.

- Render capture now runs after PostLateUpdate.UpdateAllRenderers, after native
  particle geometry is ready; capturing inside the test coroutine gave a
  pre-particle-stage image. Full fixture passes090806 and091211 were mechanical
  passes, not final visual acceptance. For reproducible **appearance** coverage,
  the final fixture uses scaled Time.captureFramerate60 for its timeline and
  temporarily switches only its instantiated flash systems to that clock. The
  production prefab remains useUnscaledTime=true and realtime/pause/expiry are
  independently covered by the passing behavioral tests. A10s fixture stun is
  killed at captured600ms, covering the real disabled-stun death path without a
  realtime coroutine ending early during software shader compilation. Raw trace
  stores both capture and wall times. No runtime duration/render-ack extension.

Actual Slayer controller additional test1/1PASS (092150-quick): accepted stun enters real PlayerSlayerMode, guard persists through activation, death while stun component is disabled exits mode and preserves independent dust until expiry. Combined PlayMode evidence49cases; no full suite.

## Final native output review

Final Unity rendered run `20261023T092302-quick`, source
`34c64bb0d9a9aa963e48b43791ef41fc0d762667814751cdb9da45c18519ae1c`,
passed its one fixture covering all eight variants. Direct review covered
1036/1036 frames and 1028/1028 consecutive transitions, including held frames
and sheet boundaries: sword left/right129/129, shield129/130, axe130/130,
legacy129/130. Also inspected148/148 VFX-off pose diagnostics and195/195
distinct pixel states at native320x180, supplementing the complete2x sequence.

Evidence root: `E:/CodexTemp/TypingHero/enemy-state-vfx/review/`.
`frame-review.csv` has one row per rendered frame with original timestamp,
reference pose/phase, concrete observation, transition and evidence path;
`off-review.csv` records the148 off-effect poses. `observations.json` records
the direct sheet observations; `all-00.png` through `all-32.png`, `off-00.png`
through `off-04.png`, and `native-00.png` through `native-06.png` were viewed.
`preview.mp4` retains all captured frames at60fps; `index.html` provides1x/0.25x.

Guard expands from white/gold core to cross, fragments then clears without
stretching or opaque edges. Dust begins at torso/floor contact, stays at the
visible baseline, fragments and clears; skull landing never triggers another
puff. Sword/grip, shield and hat remain intact in the off-effect comparison.
Legacy source final cel is preserved but its existing cleanup event occurs at
that cel start, so it is not visibly presented by the original runtime.

This is a neutral orthographic Unity appearance fixture, not a full-room
lighting/WebGL/device performance acceptance. Runtime unscaled lifetime,
pause, expiry, scene ownership and result cleanup have independent behavioral
evidence. MetalClash frozen every-step seed/mirror tests passed with original
art retained; no new parry movie/reference-game comparison is claimed.
