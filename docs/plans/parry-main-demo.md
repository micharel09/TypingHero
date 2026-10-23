# Latest main + original parry playable demo ? 2026-10-22

User requested fetching the latest primary branch and reopening the demo.
Origin has no master branch; origin/HEAD is main. Fetched tip:
`fd3f21de716964e4cb1dc60a4ce21e70e67d29f0` from
`https://github.com/micharel09/TypingHero.git`.

Created Orca child `micharel09/parry-main-demo` at
`C:/Users/ADMIN/orca/workspaces/TypingHero/parry-main-demo`, based on that tip,
parent `C:/Users/ADMIN/orca/workspaces/TypingHero/kaluga`. Parent dirty checkout
and unrelated WIP are untouched. This is a local demo integration; no remote
publication or integration requested.

Transferred only original-sprite parry clips/state transition changes, the
800ms one-word follow-up and iframe/bypass consumers, builder and tests. Three-way
integration retained main's contact snapshots, cleaned enemy contact cels,
physical ember-tail VFX and damage notification event. Success no longer inherits
main's old automatic-attack events. The builder now maps contact defenses to
original frame2 as well, so the main LateUpdate contact hold cannot restore0/1.
HUD guide, pause fixes, dungeon and all other main sources remain on their main base.

Protected56 original asset/import/clip-GUID files against the latest-main Git
blobs; all match main, with no pixel edits by this integration. Refreshed13 older
contract hashes to those unchanged main bytes. Decoded combo_3_end and idle PNG
pixels are identical to the previously reviewed parent source. Static source
2/3/4/5/idle plate re-inspected at nearest2x. Source frame2 has raised blade/grip,
3 lowers that blade slightly,4 retains the authored detached blue blade fragments,
5 releases those fragments, idle restores the lowered hand/support stance.
No source silhouette, equipment, pivot or normal redraw. This is static/source
inspection; perceived continuous motion and new runtime VFX alignment are not
accepted by that observation.

Input snapshot for scoped checks/build:
`8f7076249f0168cee5d1c5fd5851137f62a016ef8396deebc9323f205aa35a72`.
Static56-file/GUID/clip/timer/retired-reference checks and preview fake-clock tests
PASS. Scoped Unity26-case parry/contact run and WebGL demo build pending shared
runner availability. Temporary runner changes only select testFilter, serialize
before tests, and retain the QA player without launching an unrelated browser suite.

Temporary resources: `E:/CodexTemp/TypingHero/parry-main-demo`. Retain source child
and local demo player/host during user review, then close only the owned host/page
and remove owned temp resources. Keep the child while unintegrated source changes
are needed. Original demo remains available until verified replacement is ready.

The first scoped quick attempt timed out after600s waiting for the shared lock;
no tests executed and no acceptance inferred. Retry combines scoped26 tests and
the QA build under one lock acquisition, waiting up to3600s. Other jobs preserved.

The first execution140156 failed before tests: main SlashVolumeAssets still
required attack events in the now-defensive success clip. Updated its attack-only
consumer list, replaced obsolete success-as-attack test cases with a no-word
no-attack-window regression, and retained actual attack slash/contact validators.
Expanded scope31 cases to cover these directly affected consumers. Retry input:
`5d9100b34377d7661d152b2b9082349472c5fba6fd2e4c5d28b0f0562b132019`. No tests/build PASS claimed for140156.

Second execution141205 failed before tests at the protected typing-font guard.
PlayerParryAssets.SaveAssets ran mid-pipeline and flushed loaded dirty font assets.
Internal Apply now defers saving to main PixelUIBuild; the standalone menu wrapper
still saves explicitly. Font protection and strict repeat checks remain unchanged.
Retry input `f3ab734743f8f787ee2044cee7150649660b34b26f9cb3fcd677d725025a35ae`, same31-case scope.

Third execution141448 reproduced the font guard. Byte comparison establishes
that CobblestoneUI64.asset differs ONLY by6014 CRLF-to-LF line endings; normalized
contents are identical. The mid-pipeline-save hypothesis was incorrect. Restored
the original parry Apply/menu structure and wrote the font asset using its exact
canonical origin/main blob (no Git content change). No font data, protection or
acceptance rule changed. Final retry input `e17c70958e4e7cf0ac08e71d65ff7c03293c623fe23bf5eea878717eed6a7382`.

Final execution141745 on e17c70958e4e7cf0ac08e71d65ff7c03293c623fe23bf5eea878717eed6a7382:
strict repeated serialization/font protection PASS, scoped31/31 Unity tests PASS,
0failed/skipped,2.4495452s. Includes the new no-word no-hitbox case, transformed
main contact math/entry frame, existing actual attack contact/damage/spark, and
missing-contact validator. WebGL build now running under the same lock/snapshot.
Post-serialize protected hash audit: 55/56 exact.
The remaining normal-importer meta differs only in trailing spaces on empty
YAML scalar fields added by Unity serialization. Settings/GUID are identical;
original normal PNG and all source sprite pixels/import references are unchanged.
Input contract remains exact56/56 against main; do not claim post-serialize56/56.

## Playable result

Final WebGL141745 build/strict repeated builder PASS. Downloaded32files/
48,027,035bytes; every local SHA256 matches the immutable server player. Imported
its tested22-row ParryContacts.asset into the source checkout, preserving its GUID
and normalizing only empty-field trailing whitespace for Git. All rows now use
original frame2 for defense, including editor play before rebuilding. Numeric
values/references equal the asset used by the passing31 tests and compiled player.
Frozen player input remains e17c7095; resulting source-after-generated-asset hash:
`e6cdb4ad2bb0e996162b56e40fc1c8fe4d57c24fa6c85d83243d6eaac4b3ccb2`.

Native Edge155.0.4283.62 / Playwright1.58.2 / Intel UHD ANGLE D3D11,640x360:
**17/17PASS in15.865s**, real keyboard/mouse, read-only QA bridge, existing unchanged
browser.cjs fixture/external interception harness. Normal and contact entry show
original frame2; no-word success returns idle with no attack; two completed words
release one follow-up and exactly10 damage (180->170), player40HP, two parries.
Pause holds the accepted follow-up and ignores qz input; resume releases it.
Sorting mismatches0;0JS/managed exceptions,0fixture/provider reads/writes. This is
right-facing1x functional integration coverage; dense direction/speed/key/boundary
checks remain the31-case Unity receipt, not new browser coverage of those variants.
First native pilots failed because the harness sent Space during the still-frozen
encounter intro, then waited for a running clock without clicking its Fight button.
Corrected only harness UI sequencing and reused the exact same player; no gameplay
change or relaxed acceptance. Initial4/3 checks are not counted as passes.

Actual Orca menu and native success-contact/paused-queue captures inspected:
latest dungeon/menu/HUD skin, original raised grip/blade and main physical sparks
remain visible. Continuous perceived force/fluidity and every runtime VFX alignment
are unverified; no full difficulty, performance, balance or release claim.

Demo is open in parent kaluga's Orca browser at http://127.0.0.1:8136/,
page e98327a8-914c-45c1-8810-75f3e56380f7; owned host terminal
term_493bae17-a400-4301-9582-111f117ae4ee. Source child stays at
C:/Users/ADMIN/orca/workspaces/TypingHero/parry-main-demo,
branch micharel09/parry-main-demo, based on origin/main fd3f21d. Retain this source
and E:/CodexTemp/TypingHero/parry-main-demo host/player/session/manifest/worker
while user review is open. Close the owned host/page and remove owned temp when
review ends; retain unintegrated source child until its changes are preserved.
No push, merge or publication. Parent WIP untouched.

Cleanup complete: removed four owned server result directories after snapshot,
symlink and active-mount checks; removed temporary helpers/captures/isolated QA
profile and the retired e2 demo after ownership/hash/process checks. Retained only
the current32-file player, host/fixture worker, manifest and session/ready markers.
All32 hashes rechecked unchanged; only the requested user-play host/page remain.
Source child kept for review; parent checkout and all other jobs preserved.

## Remote advance during delivery

Final remote check found main916c752 (tutorial callout cards9927025 and floor-rest
embers7077a35) beyond the initially fetched fd3f21d. Preserved verified fd demo
source in local commit c568bb1, merged latest main in the same child, retaining both
documentation sections. No runtime C# changed upstream; its scene/editor guide and
particle profile changes are retained. No remote push/merge. Rebuilding latest
input `662223b4e2dc26cddad814d0668c6a2f6e258e6e41dc858493e1f69fd3122cf7`, scoped33 cases (prior31 plus two-sided floor-rest cases).
Existing fd demo stays available until this latest replacement is verified.

The first916 attempt stopped at the existing3GiB disk gate before Unity.
Temporary helper now removes only its own just-extracted duplicate input archive
under the shared lock after verifying snapshot662223b4. Source/Library/cache and
other jobs stay intact; disk thresholds and validation unchanged. Retrying same
source input, not claiming the capacity failure as tests/build PASS.

Capacity retry still lacked a few MiB at the strict3GiB gate after archive cleanup.
Verified the previous shared QA output matches all32 files of this task local
e17 demo exactly. Temporary helper may retire only that exact old artifact under
the shared lock (and asserts its full manifest again), retaining the verified
local player and leaving other tasks/cache/source intact. No gate lowered.

Data volume still lacked the configured3GiB, so latest916 verification moves
to an owned /tmp/typinghero-parry-main-demo-916-662223b4 workspace (tmpfs7.5GiB
free before cloning). Same global /data runner.lock, pinned image/CPU/memory,
activated private HOME and unchanged3GiB thresholds. Library/cache are copied
under that lock; originals and shared source are preserved. Only this private
workspace/output is eligible for later cleanup. No dependency or service setup.


## Latest main demo verified (2026-10-22)

Local integration includes main916c75291250c34aed774db1c7ecc489b435b1b0.
Frozen source662223b4e2dc26cddad814d0668c6a2f6e258e6e41dc858493e1f69fd3122cf7:
33/33 scoped Unity cases passed, zero failed/skipped,2.6048547s, including
both cooled-floor-ember cases. Strict builder repeat/font protection and WebGL
build passed on pinned Unity2022.3.25f1. Native Edge155.0.4283.62,
Playwright1.58.2,640x360, NVIDIA GTX1650Ti ANGLE D3D11:20/20 checks passed
in16.998s. Real keyboard/mouse confirmed latest guide cards/text/arrows,
original frame2 on press/contact, no-word recovery without attack, two typed
words queue exactly one10-damage follow-up, pause ignores letters and retains
queue, two parries retain40HP, enemy180->170, valid sorting, zero exceptions
and scoreboard writes. Menu, guide and success entry screenshots inspected.
This is technical/static evidence; continuous perceived motion/force and a full
performance matrix remain unverified. Previous e17 evidence remains separate.
Downloaded32 player files/48,026,728bytes; every SHA256 matched the immutable
build. Reopened user page e98327a8-914c-45c1-8810-75f3e56380f7 at
http://127.0.0.1:8136/; loading finished, no warning, actual menu observed.
Owned private server workspace/results were removed after verifying task marker,
snapshot and no active Docker mounts. Shared Library/cache/source/home preserved.

Retained for user play review: child worktree/branch micharel09/parry-main-demo
and E:/CodexTemp/TypingHero/parry-main-demo containing host.py,ServiceWorker.js,
session.json,player-manifest.json,ready.json,player/. Host terminal
term_493bae17-a400-4301-9582-111f117ae4ee. Old e17 player, latest916 helpers,
profiles and captures removed after verification. Source is locally committed;
no push or remote merge. Retain branch until reviewed/integrated; stop owned host
and delete this demo temp only when the user finishes play review.


## Push to main (2026-10-23)

User authorized pushing the reviewed original-sprite parry/typed follow-up to main.
Merged origin/main bab5ee6 (lighting profiles, MetalClash, camera impact and flame
updates); only verification documentation conflicted, both histories retained.
Pinned Unity2022.3.25f1/fly-server input
9b7d4191c6951aa543a79a4e8efe8e198d5cafccf1467d473292e88a102d1e69:
strict repeat serialization and98/98 focused PlayMode tests PASS,0failed/skipped.
Selection: PlayerParryTests, CombatCameraImpactTests, ParryClashTests,
LightingProfileTests, CombatFeedbackTests and three actual parry/damage GameplayTests.
No new WebGL player was built: prior33 Unity/20 native results remain specific to
916c752 integration, and the running8136 demo remains that reviewed binary.
Latest merge has new Unity integration evidence, no new rendered-motion claim.
Private temporary runner uses same global lock and unchanged disk/toolchain gates.
After remote ancestry verification, remove integrated child branch/worktree and
owned private runner/results/helpers; retain8136 host/player for user review.
Parent kaluga WIP is preserved.


## Dedicated combo_2 follow-up - 2026-10-23 (verified implementation)

Scope: successful-parry follow-up only; normal combo_1 and Slayer unchanged.
Hold800ms/recovery950ms/window[0,800)ms; one completed word queues one attack.
New Attack-tag state starts0/crossfades20ms, duration566.667ms;
open150/contact200/immediate close250ms, exit/disable cancellation.

Reference is the existing combo_2 and combo_2_end strips at origin/main e5bbc76,
not an external game's motion. Checklist:10cels/9internal pairs, parry-to-combo
and combo-to-idle joins; normal/mirror, source-only and runtime VFX, real early/late
typing, no input, pause, lifecycle, damage/bypass and strict repeated builder.

- `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png` SHA256 `e3b79c0d29ffcee9d7ce8e1a3e36ed123945b1a0c951881af91c82c8a9de9599`.
- `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png` SHA256 `cf94240b9338d6e0cbbc9bc14a4e65607bf021e4f85198da3061a77f5f980bac`.

Direct source review:20/20unique cels,22/22key positions including both terminal
hold repeats,18/18pose pairs +2/2held-repeat pairs,4/4joins at1x and nearest zoom.
Canvas128x128/cel; original PNGs, pivots, palette and import metadata retained.
Evidence: `E:/CodexTemp/TypingHero/parry-combo2/review-{0,1}-{0,1,2}.png`,
full source strips and full mirrored strips in the same directory. The white-purple
slash is baked into the source sprite; this is not runtime lighting/VFX acceptance.

| Clip / variant | Frame | Duration ms | Reference / phase | Frame observation | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| combo_2 / normal | F0: combo_2_0 | 50 | original same-name cel / anticipation | Low stance; visible forward blade, connected grip and grounded feet. | Blade retracts while feet remain anchored. | Source pass | review-0-0.png |
| combo_2 / normal | F1: combo_2_1 | 50 | original same-name cel / anticipation | Torso turns inward; hidden blade belongs to anticipation, not missing contact. | Grip rises in front of chest; head and feet remain continuous. | Source pass | review-0-0.png |
| combo_2 / normal | F2: combo_2_2 | 50 | original same-name cel / anticipation | Grip near shoulder; no forward slash arc yet. | Fast torso reversal becomes the first authored horizontal smear. | Source pass | review-0-0.png |
| combo_2 / normal | F3: combo_2_3 | 50 | original same-name cel / slash | Broad white-purple forward arc; rearward arm and stable planted foot. | Arc thins; body and grip remain connected. | Source pass | review-0-1.png |
| combo_2 / normal | F4: combo_2_4 | 50 | original same-name cel / slash | Separated arc segments follow original smear; contact stays forward. | Arc dissipates as blade rises behind shoulder. | Source pass | review-0-1.png |
| combo_2 / normal | F5: combo_2_5 | 50 | original same-name cel / slash | Arc almost gone; diagonal blade overhead; stance returns upright. | Arm lowers into recovery without anchor jump. | Source pass | review-0-1.png |
| combo_2 / normal | F6: combo_2_end_0 | 66.6667 | original same-name cel / recovery | Arm lowers horizontally; blade remnants visible; scarf lifted by momentum. | Blade remnants shorten, hand and scarf lower. | Source pass | review-0-1.png |
| combo_2 / normal | F7: combo_2_end_1 | 66.6667 | original same-name cel / recovery | Hand below shoulder, short blade remnant, stable torso. | Wrist lowers toward hip; bright remnant diminishes. | Source pass | review-0-2.png |
| combo_2 / normal | F8: combo_2_end_2 | 66.6667 | original same-name cel / recovery | Small trailing blade remnant; pelvis and feet stay fixed. | Hand finishes lowering; scarf approaches idle direction. | Source pass | review-0-2.png |
| combo_2 / normal | F9: combo_2_end_3 | 50 | original same-name cel / recovery | Relaxed final stance; original canvas has no clipping. | To repeated end_3 key at550ms: same grip, feet and scarf; no new pose. | Source pass | review-0-2.png |
| combo_2 / normal | F9b: held end_3 at550ms | 16.6667 | original end_3 / terminal hold | Same end_3 pixels retained through566.667ms; anchor and grip unchanged (also visible in runtime frames190->191). | To idle_0: hand/scarf nearly align, slight head-angle change; nonlooping. | Held-position pass | review-0-2.png |
| combo_2 / mirror | F0: combo_2_0 | 50 | original same-name cel / anticipation | Low stance; visible forward blade, connected grip and grounded feet. Left-facing grip and arc on correct side. | Blade retracts while feet remain anchored. | Source pass | review-1-0.png |
| combo_2 / mirror | F1: combo_2_1 | 50 | original same-name cel / anticipation | Torso turns inward; hidden blade belongs to anticipation, not missing contact. Left-facing grip and arc on correct side. | Grip rises in front of chest; head and feet remain continuous. | Source pass | review-1-0.png |
| combo_2 / mirror | F2: combo_2_2 | 50 | original same-name cel / anticipation | Grip near shoulder; no forward slash arc yet. Left-facing grip and arc on correct side. | Fast torso reversal becomes the first authored horizontal smear. | Source pass | review-1-0.png |
| combo_2 / mirror | F3: combo_2_3 | 50 | original same-name cel / slash | Broad white-purple forward arc; rearward arm and stable planted foot. Left-facing grip and arc on correct side. | Arc thins; body and grip remain connected. | Source pass | review-1-1.png |
| combo_2 / mirror | F4: combo_2_4 | 50 | original same-name cel / slash | Separated arc segments follow original smear; contact stays forward. Left-facing grip and arc on correct side. | Arc dissipates as blade rises behind shoulder. | Source pass | review-1-1.png |
| combo_2 / mirror | F5: combo_2_5 | 50 | original same-name cel / slash | Arc almost gone; diagonal blade overhead; stance returns upright. Left-facing grip and arc on correct side. | Arm lowers into recovery without anchor jump. | Source pass | review-1-1.png |
| combo_2 / mirror | F6: combo_2_end_0 | 66.6667 | original same-name cel / recovery | Arm lowers horizontally; blade remnants visible; scarf lifted by momentum. Left-facing grip and arc on correct side. | Blade remnants shorten, hand and scarf lower. | Source pass | review-1-1.png |
| combo_2 / mirror | F7: combo_2_end_1 | 66.6667 | original same-name cel / recovery | Hand below shoulder, short blade remnant, stable torso. Left-facing grip and arc on correct side. | Wrist lowers toward hip; bright remnant diminishes. | Source pass | review-1-2.png |
| combo_2 / mirror | F8: combo_2_end_2 | 66.6667 | original same-name cel / recovery | Small trailing blade remnant; pelvis and feet stay fixed. Left-facing grip and arc on correct side. | Hand finishes lowering; scarf approaches idle direction. | Source pass | review-1-2.png |
| combo_2 / mirror | F9: combo_2_end_3 | 50 | original same-name cel / recovery | Relaxed final stance; original canvas has no clipping. Left-facing grip and arc on correct side. | To repeated end_3 key at550ms: same grip, feet and scarf; no new pose. | Source pass | review-1-2.png |
| combo_2 / mirror | F9b: held end_3 at550ms | 16.6667 | original end_3 / terminal hold | Same end_3 pixels retained through566.667ms; anchor and grip unchanged (also visible in runtime frames190->191). | To idle_0: hand/scarf nearly align, slight head-angle change; nonlooping. | Held-position pass | review-1-2.png |

Parry exit combo_3_end_5 to combo_2_0 inspected in both directions: upright torso
lowers into anticipation, trailing blade remnant becomes forward blade at hip,
feet retain ground line. Final runtime timing/lighting/VFX and playback evidence follows below.

First builder attempts stopped before tests: three typing labels have stale
serialized m_fontColor32 caches inconsistent with their existing float m_fontColor.
Only these caches were synchronized to existing float colors; no font, float color,
input or geometry changes, and the preservation guard remains unchanged.
The final receipts below supersede these initial failures.


### Final implementation and receipts

- Follow-up uses `AC_ParryFollowUp` / `Base Layer.parry_followup`, tagged Attack,
  speed1, start0 and a fixed20ms CrossFadeInFixedTime. Normal keeps its original
  normalized CrossFade/start offset; Slayer clips and their states are unchanged.
- Ten unique cels at0/50/100/150/200/250/300/366.667/433.333/500ms.
  Unity derives clip length from its last key plus one sample: a repeated final
  cel at550ms holds the same end_3 pose through566.667ms. This is11curve keys,
  not an eleventh pose. Initial516.667ms truncation was caught and corrected.
- OnAttackStartCel(combo_2_3)150ms, contact combo_2_4 at200ms, immediate close250ms.
  Start marks follow-up before Animator request. Queue/input/recovery windows stay
  unchanged; hit/death/Slayer/interruption/disable clear queue, pause retains it.
- CancelAttack stops delayed close, disables hitbox and notifies closure once.
  Follow-up end/exit supplies its opening cel as owner: late callbacks cannot stop
  a newer normal/Slayer window or its coroutine. Controller/component disable is
  unconditional. Source-backed repro ca861185, result20261023T140708-quick failed
  both replacement-window cases before the guard; final tests cover both callbacks.
  An earlier fixture incorrectly expected Slayer clips to emit opening events;
  actual PlayerSlayerMode.OnCorrectChar opens the window through code. That fixture
  failure is not counted as product evidence.
- Final Unity2022.3.25f1/fly-server result20261023T144919-webgl,
  input `8be3e31c25b53c32b7ec40ca8b47cbb138b5aeee48b42374489a12e689a1f050`:
  strict repeat builder and WebGL build PASS. **89 distinct PlayMode cases covered**
  (PlayerParry33, CombatFeedback18, CombatCameraImpact38). Final runtime source
  first passed87/89 in result20261023T144229-webgl; exact800ms fixture arithmetic
  had a floating-point future-deadline residue and the existing180ms camera
  receiving-frame case hit its timing-sensitive boundary. Parenthesizing the
  fixture offset fixes exact800ms without changing gameplay; camera assertion/
  production code were unchanged. Final targeted rerun passed9/9 including both
  failures. This is combined coverage, not a single89/89 run. Prior0c74 source
  passed88/88 before adding replacement-before-close bypass coverage.
  Starting a replacement attack now closes the old logical window before the new
  open event, clearing follow-up bypass without cancelling the incoming hitbox.
  Includes799/800/801ms, early/late words, no input, two words/one release,
  pause/interruption, no anticipation damage, one hit across multiple colliders,
  actual mirrored/nonuniform-scale contact, hit-flash, bypass consumption/closure,
  exact250ms closure, exit/disable and replacement normal/Slayer window preservation.
- All19old controller subasset IDs retained unchanged except the Base Layer child
  list gains the new state; three subassets added (state/transition/exit behaviour).
  Existing GUIDs, all source PNGs/normal maps/pivots/palette and normal attack config
  retained. Pivots0.49/0.51,16PPU,128x128canvas throughout the new sequence.
  Six authored asset files equal immutable build exports after whitespace-only
  normalization. Prefab serialization adds two previously implicit null cameraImpact
  fields; values remain null. Three typing Color32 caches now match existing float
  colors; every other protected typing field/font remains protected by the guard.
- Final local player33files all match immutable server SHA256s. Edge155.0.4283.62,
  Playwright1.58.2, Intel UHD/ANGLE D3D11, one1280x720 viewport: **47/47** real-input
  checks PASS in35.3055221s (`native-final/browser.json`). Early and late word completion plus queued Pause/Resume
  each show all10combo_2 cels, enemy180->170 once, Attack sorting, hit-flash,
  hit spark, normal camera peak and WebAudio buffer starts; no combo_1 in the branch.
  No input produces no follow-up. Zero managed/JS exceptions, fixture/production
  scoreboard reads/writes0; interception and external blocking remain enabled.
- Same final player, separate direct lifecycle run: **13/13** checks PASS in
  19.8399111s (`native-final-lifecycle/browser.json`). Real parry/typed queue -> Pause -> Restart leaves no pending attack
  or enemy damage after waiting beyond recovery; real queued Pause -> Menu clears
  queue and leaves0active gameplay actors/hitboxes. This is scoped acceptance,
  not a full difficulty, performance, stress/soak or release result.
- Final-player first headed pilot passed14checks then missed a real Space
  acknowledgement before the late scenario; it is retained as incomplete, not PASS.
  Its copied prior snapshot label is explicitly corrected by provenance-note.json;
  accepted rerun uses the final marker and verifies the same player bytes.
- Headless pilot passed no-input then missed a later real Space acknowledgement;
  no overall PASS. Headed final driver focuses the canvas and acknowledges the raw
  per-render window; product timing/thresholds were not changed to pass the driver.

### Final frame evidence

Source references and22per-key rows for20unique cels above remain valid: no PNG/import changes.
Final clip/config canonical hashes: `E:/CodexTemp/TypingHero/parry-combo2/final-asset-hashes.json`.
Reviewed recording: `E:/CodexTemp/TypingHero/parry-combo2/native/followup-runtime.webm`
from immutable result20261023T140929-webgl/input0c74b4ae. Reused for final source:
all six authored asset canonical hashes are identical; the later logical-window
transfer fix is not taken in this complete parry-to-idle capture. Final native
functional/lifecycle receipts are separate; this video is not relabeled as a new build.
Selected BEFORE visual inspection: requested PTS5.155810..7.578580s, complete parry
preparation/success hold, two typed words, full follow-up and idle tail. Actual
source frames136..206 (PTS5.161..7.550), **71/71frames and70/70consecutive pairs**
directly inspected, including every held frame. No sampling or FPS conversion.
Manifest preserves original PTS/durations and video SHA256; raw PNGs remain linked.

At native320x180 and nearest gameplay zoom: held upright parry/grip stays grounded;
parry recovery lowers blade through3/4/5, then enters combo_2_0 directly at frame172.
Frames172..176 show all anticipation before damage;177starts the broad forward
arc, spark and single HP drop. Hitstop repeats177..180, then thinner arc181,
raised blade182..183 and four recovery cels184..191. Frame192returns to idle;
through206no repeat, residue or foot jump is visible. Final hold key produces no
new pose. Runtime lighting/normal maps, camera contact and enemy flash remain readable.

All71frames/70pairs were also inspected as a labeled pixel-reflected VFX preview.
This is **not a second runtime flipX capture** and does not establish mirrored
runtime lighting; actual flipped contact/math are separately covered by Unity.
Source-only normal/mirror sheets provide the VFX-free silhouette/grip comparison
(the baked arc belongs to the original sprite and remains).1x/.25x normal and
pixel-mirror playback supplement, never replace, the complete frame inspection.

Evidence directory `E:/CodexTemp/TypingHero/parry-combo2/runtime-review/`:
`manifest.json`, `frames/`,18normal sheets,18mirror-preview sheets and
`frame-observations.csv` with **one explicit row for each of142reviewed frames**
(variant, source frame, PTS, original duration, reference phase, observation,
next transition, status and image). The preview limits above apply to every mirror row.

Retain local player/host/fixture and evidence for user review. Delivery session
records the integrated commit and owned terminal/page IDs; source task worktree
is removable only after remote ancestry verification. No itch.io upload.


## User correction: omit first follow-up cel (2026-10-23)

User reports a small player jerk and requests removing the first follow-up frame.
This supersedes the previous full-ten-cel choice: omit only combo_2_0; do not edit
pixels, pivot, transforms, normal maps, normal attacks or Slayer. Reference is the
same original source strips above, with the user-selected sequence deletion.

New sequence combo_2_1..5 then combo_2_end_0..3 begins at0/50/100/150/200/
250/316.667/383.333/450ms. Repeated final key500ms retains the original terminal
hold through516.667ms. Start/contact/close events move together to100/150/200ms,
using the same combo_2_3/combo_2_4 contacts and opening-cel ownership. Parry
800ms hold,950ms recovery, queue rules, fixed20ms crossfade and10damage stay unchanged.
Builder skips the first source cel so rebuilding cannot restore it.

Source-only direct review:9/9unique cels plus terminal-hold key, all9key pairs and
both parry-exit/idle joins per normal/mirror variant. Sheets source-0-0..2 and
source-1-0..2 at native128px/nearest zoom and20explicit per-key rows are retained
at E:/CodexTemp/TypingHero/parry-trim-entry/source-frame-review.csv. The omitted
low extended-blade jab is absent; entry now goes directly from upright parry
recovery to the tucked grip of combo_2_1. That original anticipation still leans
forward; no new in-between pose or smoothing was invented. Connected grip,
blade rotation, planted feet and recovery scarf were checked across every pair.
Source mirror is a pixel preview; runtime mirror-lighting coverage is not implied.

Pinned Unity2022.3.25f1/fly-server20261023T150834-webgl, input
09307d5508e136d66cd6218f2e27c6ba4678c00c63e1da6ad42e62ff9b35a955:
37/37focused PlayMode cases PASS,0failed/skipped,1.1736806s. Filters are
PlayerParryTests and the four actual animation-event contact cases (normal/
follow-up, ordinary/mirrored scale). No broad camera/performance suite repeated.
Runtime queue/cancellation code and unrelated consumer code are unchanged.

Strict repeated builder/WebGL PASS. Unity-authored clip export copied back with
trailing whitespace normalized; GUID/controller/config/source strips unchanged.
All33immutable player files match local/server hashes. Native Edge155/Playwright
1.58.2/Intel UHD ANGLE D3D11 at1280x720:51/51PASS, zero runtime exceptions/provider
writes. Early/late/two-word/no-input/Pause-Resume retain one10damage hit and idle;
all9remaining cels appear and combo_2_0 never appears. Prior keyboard pilots are
retained as incomplete: full-state polling missed windows that raw per-render
telemetry proves existed. Driver now waits on the read-only per-frame condition
in the browser before sending actual Space; no gameplay mutation or threshold change.

Before review selected final video window7.043685..8.348160s: last250ms before
follow-up detection includes original parry recovery and entry, complete trimmed
follow-up and idle tail. Actual source F177..215, PTS7.060..8.334:39/39frames and
38/38pairs directly inspected at native320x180/nearest zoom; all held encoded
frames retained. Also39/39frames and38/38pairs in labeled pixel mirror preview;
source-only normal/mirror9poses/terminal hold reviewed separately above. F180->181
joins directly to tucked combo_2_1, F183..184raise grip, F185first slash/single hit,
F185..187hold through hitstop, F188..190finish cut, F191..198recover and F199..215idle.
No extended initial jab, residual slash or foot-anchor jump in this selected segment.
The original compact anticipation lean is retained; this is the requested deletion,
not a promise of interpolated motion. Runtime mirror lighting is outside this preview.

Final per-frame evidence: runtime-review/manifest.json (source SHA/PTS/scope),
20sheets and78explicit normal/mirror per-frame rows in frame-observations.csv,
under E:/CodexTemp/TypingHero/parry-trim-entry. Normal/slow playback plus source-only
normal/mirror playback supplements direct frame inspection. Local player/host and
receipts retained for user review; after review stop the owned host and remove this
evidence directory. Delivery session records commit, remote verification and host.

Playback uses an indexed39-frame lossless derivative with reset timestamps; its
reported terminal duration is not the timing authority. Original WebM, complete
per-frame PTS and held durations remain authoritative. Final clip matches the
immutable serialized export after trailing-whitespace normalization.
