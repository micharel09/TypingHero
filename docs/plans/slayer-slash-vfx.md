## Black Slayer B impact (2026-10-25)

Approved scope: only the B impact centered on the skeleton, pure black#000000,
including normal Slayer hits and finishers. The large player sword trail,
afterimages and ivory body pulse keep their existing colors. The source white
PNG/importers, frame sequence,120/160ms timing, alpha/residual fade, pivot,
direction and fixed body-center anchor remain unchanged.

HitSparkSpawner supplies Color.black only in its SlayerHitVfx spawn branch;
captured progression tint remains available to unrelated feedback/legacy routes.
No public API, shader/material, animation, builder or prefab changes are needed.
Existing color expectations are updated but not executed under the user's waiver.

Worktree C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-black-impact,
branch micharel09/slayer-black-impact, base2b07f3f includes the newly integrated
duel intro. Reuse the private source/Library at
E:/CodexTemp/TypingHero/slayer-hit-feel; new artifacts go under black-impact/.
The real-input capture helper waits for the intro before tutorial controls.
No test-suite or exhaustive visual-frame acceptance claim is made.

### Build and clip receipt

Pinned Unity2022.3.25f1 WebGL build exited0. Accepted input
1954990d05567189c649d924807c80d09ea50b99e6c8b767b65ccab6ba53f0c1 is preserved in
black-impact/snapshot.json and accepted-unity-input.tar.gz. Player fingerprint
077f252553ba3b31 and per-file hashes are in player-manifest.json. Built source
matches the scoped spawner/expectation changes and unchanged clips/depth index.

Final actual canvas/audio clip black-impact/slayer-black-impact.webm is2426938bytes,
SHA25653a61ea475ee3db1ee48f69ce4ab609e458d29f01081ebdd13ef50d7805831e6.
Opened black-impact/open-clip.html in Edge at2026-10-25T04:54:58.597088+00:00,
with1x/0.25x and S1/S2/S3/rapid-typing jump buttons. Isolated variants begin
at0.902/1.869/1.391s and include their full authored recovery; rapid input follows
at35ms per letter/100ms between words. Final capture reaches playerHP40/enemyHP0
and includes five observations of B with enemyHP0.204render observations contain
117B samples, all RGB(0,0,0);50birth samples coincide with recorded body center.
No runtime exceptions or provider reads/writes were captured. These observations
are capture evidence, not a test-suite or exhaustive encoded-frame-review pass.

The first capture is retained under black-impact/first-capture: its slower typing
exited Slayer with7enemyHP before the normal lethal hit, so it is not the finisher
evidence. The final capture uses the same built player and records a Slayer B
finisher. User visual approval and exhaustive frame/transition inspection remain
unverified under the explicit waiver. git diff --check is clean; no tests ran.
Keep the artifacts and warm private source/Library until user review/reproduction
is finished; no earlier clip or original dirty checkout was changed.
### Main integration receipt

Feature8645a27 was pushed to micharel09/slayer-black-impact. A fresh fetch found
new dungeon lighting on origin/main15f61fa; merged both sides as36aa2fd without
conflicts. Inbound changes are floor surface maps/materials, cool fill, lantern,
world scene and the existing parry-lift surface binding. All runtime scripts,
character clips/prefabs, VFX assets/materials/shaders, B builder and ActorDepth
are unchanged from the captured feature. B uses its unlit PixelSpark material;
its black tint/body anchor do not depend on the new world-light parameters.
Reused the scoped capture and checked the dependency diff/whitespace; no repeated
build, suite or broad lighting profile was run. The clip's full-world baseline is
2b07f3f plus the B change, not a claim of a new full merged-world visual review.

Direct fast-forward main push15f61fa->36aa2fd7d556dba3dfb3bef87de8b617b48c1fcd
succeeded. Subsequent fetch confirmed the feature is an ancestor of origin/main.
This following receipt changes documentation only. After it is remotely preserved,
remove only the clean task worktree/local branch; retain the remote task branch
and review/cache artifacts until user review and reproduction are finished.

## Final strike and combo_end correction (2026-10-25)

The user identified the screenshot's pose as an unfinished combo. Source inspection
confirms the prior S3 held combo_3_4 and none of the three Slayer clips used the
authored combo_end sheets. Approved choice: one final strike plus complete sword
recovery per typed key, rather than playing all remaining combo rotations.

S1: combo_1_0/1/2 then combo_1_end_0..3. S2: combo_2_3/4/5 then combo_2_end_0..3.
S3: combo_3_11 then combo_3_end_0..5. Every clip has seven40ms poses at
0/40/80/120/160/200/240ms and ends280ms; an unchanged final-sprite key270ms
preserves the last hold at100samples/second. The Player prefab's serialized
AttackSpeedMul remains1. Nonlooping clips hold the recovered final pose. Another
correct key immediately interrupts recovery through the existing animator.Play.
Damage-window/collision/word-event order, B120/160ms, body pulse40ms and trail
envelope120ms are unchanged. No public API or source-art/importer changes.

Worktree: C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-combo-end,
branch micharel09/slayer-combo-end, base669df07. Reuse the retained private Unity
source/Library; new artifacts go under E:/CodexTemp/TypingHero/slayer-hit-feel/combo-end.
Existing clip expectations are updated, not executed. User waiver still applies:
no test suite or exhaustive output-frame/runtime acceptance claim. Build and
real-input clip with separated S1/S2/S3 and rapid typing are the delivery route.
### Build and immediate clip delivery

Accepted immutable input9cfab4727b4ee61b4bfa387fdd8cff0396d39b45d90c68fd6b6d292fadd3012f
is archived in combo-end/accepted-unity-input.tar.gz with snapshot.json. Pinned
Unity2022.3.25f1 serialization and WebGL build both exited0. The derived depth
index has281mapped/0missing; only ActorDepth.asset is imported from generated
metadata. Source PNGs/importers, controllers, production C# and game APIs are
unchanged. git diff --check is clean. Existing clip assertions were updated but
not run, matching the user's explicit waiver.

Actual canvas/audio clip: combo-end/slayer-combo-end.webm,3134643bytes,
SHA256 d557afc546d5ecee9b09322225065b83b070e255376623aa82789ba9a58b912f.
Player fingerprint8c5e1a2c4f3d302c; Edge156.0.4314.42/Intel UHD/D3D11.
Real keyboard capture includes separate S3/S1/S2 at0.916/1.426/1.927s, each with
460ms after its key before another input. Recorded final poses are respectively
combo_3_end_5, combo_1_end_3, combo_2_end_3. Twelve rapid-word entries follow.
One preceding key recorded no classified strike; it is retained in the clip,
not counted as an isolated variant.324render observations and0captured runtime
exceptions/provider reads/provider writes are capture data, not test results.

Opened combo-end/open-clip.html in a visible Edge window at
2026-10-25T04:26:53.016940+00:00. It supplies1x/0.25x and S1/S2/S3/rapid-typing
jump buttons. User visual approval and exhaustive encoded-frame/transition review
remain unverified; do not label this as a complete sprite-review pass.
Retain this clip, the earlier clips and warm private source/Library while the user
reviews or requests adjustments; remove after review/reproduction is finished.
Feature commit4519935abc980548fbeb6df27d4f271238374e88 was pushed to
micharel09/slayer-combo-end. A fresh fetch still had origin/main669df07; direct
fast-forward push to main succeeded. Subsequent fetch confirmed feature HEAD is
an ancestor of origin/main4519935abc980548fbeb6df27d4f271238374e88. This receipt
changes documentation only; the built/captured Unity input is unchanged.
After preserving this receipt remotely, remove only the clean task-created
worktree/local branch; retain the remote task branch and review artifacts.

## Contact-first B revision (2026-10-25; clip delivered, tests waived)

The user rejected the body-centered580ms/random-eight-angle result as flying
around the skeleton. Reviewed reference evidence is retained separately at
`E:/CodexTemp/TypingHero/slayer-hit-research`: Blasphemous official full0-6.05s,
121/121frames+120/120pairs+promotional loop boundary; Moonscars melee1.5-4.5s,
300/300frames+299/299pairs, with55additional spell frames excluded from sword-hit
claims. REA verifies official page observation; no commercial game binary or
native hitstop implementation was inspected.

Approved decisions: retain B artwork; keep immediate typing/damage; start Slayer
clips at contact. S1 uses combo_1_0/1/2, S2 combo_2_3/4/5, S3 combo_3_3/4.
Each clip ends120ms; repeated final sprite key110ms preserves the last hold at
100samples/second. Their local strike axes are(1,-1),(1,0),(1,1), mirrored by
source transform. Apply the selected pose before opening the existing hitbox.
B uses F0/F1/F2/F3:40/30/30/20ms hit,60/40/30/30ms finisher; common pivot,
32/40reference-pixel width and original PNG/GUIDs preserved. Each recipient owns
one primary plus one<=35% residual fading<=30ms; finisher replaces both. Captured
RGB/center/direction survive lethal Slayer exit. Original weapon trail expires
120ms even when its cel is held; a new swing resets that envelope.

Skeleton root jitter is disabled. Its existing hurt poses remain; the silhouette
compositor supplies an ivory#FFF3CF pulse20ms full+20ms fade, composed after the
silhouette in DungeonSprite without changing alpha/depth or baseline color.
Pause/reading hold these presentation clocks. Public input/damage/scoring/audio/
camera/hitstop contracts remain unchanged. QA additions are observation-only.

**Latest user instruction:** stop tests and open the clip immediately when ready.
The initial scoped test launch was cancelled during first project initialization;
no test-pass claim is made. Continue with the necessary game build and capture;
full sprite/runtime acceptance and the authored test cases remain unexecuted.
Do not present a built preview as verified game-feel acceptance.

Worktree:`C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-hit-feel`, branch
`micharel09/slayer-hit-feel`, base7dc84f9. Original dirty checkout remains intact.
Build/capture cache:`E:/CodexTemp/TypingHero/slayer-hit-feel`.

### Final build, clip and integration receipt

Compatibility-corrected immutable input `3272ecc39ea954d4e9e55850b31d3c8acb5dbc68a2ffb6e6476a72710e10aaaa` builds with
Unity2022.3.25f1 exit0, player`03a0033e57fdf3a2`. Final real-input capture records
HP40/enemyHP0,50correct letters of51attempts and4parries; these are capture data,
not test assertions. No new unit/runtime suite, mirror capture or exhaustive frame
inspection was run. The user explicitly waived those checks.

Final clip:`E:/CodexTemp/TypingHero/slayer-hit-feel/final/slayer-hit-feel.webm`
(2816364bytes, SHA256`b82190675a58f0a2194a8a16510653e750a4cfcb9a92b992c923e32cf7a0d144`).
Final viewer:`E:/CodexTemp/TypingHero/slayer-hit-feel/final/open-clip.html`.
Opened directly in Edge at2026-10-25T03:47:05.960450+00:00, with1x/0.25x controls. The first capture
remains separately available; final input preserves the original hitbox timing.
The offline asset viewer is rebuilt and retained at the cache root/preview.html;
its Edge-loading/interaction acceptance was not run under the waiver.

Feature8cb68a5 plus collision-order correction6227d17 pushed to the task branch.
Fetched origin/main7dc84f9 was already contained; direct fast-forward main push to
6227d173c45b8dacbe8486cc3f32e44f5fbc092d succeeded. A subsequent fetch and ancestry
check confirmed integration. This following receipt is documentation only; no
Unity input changes follow the final captured build.

Retain `E:/CodexTemp/TypingHero/slayer-hit-feel` for both clips, players, input
archives/manifests/logs and the private warm `source/Library` cache. Keep it while
the user reviews/requests visual adjustments; remove after visual review ends and
reproduction is no longer needed. This avoids repeating the expensive first import.
The abandoned first-import Library can be removed; the pre-existing space-press-cue
cache was only read/copied and is preserved. Close no user clip window. After this
receipt is remotely preserved, remove only the clean task worktree/local branch;
retain the remote task branch. The original dirty main checkout remains untouched.

### Collision-order compatibility before integration

Retain the original `OnAttackStart()`/physics confirmation timing for Slayer.
Do not confirm damage reentrantly inside `TypingManager.OnCorrectChar`: lethal
Slayer exit unmutes word completion while that callback is still processing its
last letter. Retaining the old window avoids changing the final-character event
order. Contact-first poses, B, pulse, tint and overlap budgets are unchanged.
The first delivered clip remains a preview; rebuild/capture the final input after
this small compatibility correction. User test waiver remains in force.

### First build and immediate clip delivery

Pinned Unity2022.3.25f1 WebGL build completed with exit0; player fingerprint
`b1ba91ff3a8e7223`. The normal build's existing preprocess/repeated serialization ran;
no unit/runtime suite or exhaustive sprite review was run after the waiver.
Capture-only real keyboard playback reached Slayer and typed through the finisher;
276render observations retained,0captured runtime exceptions,
0provider reads/writes. These observations are not a test-pass or visual-acceptance
claim. Initial menu navigation missed Play before its reveal completed; the
capture helper waits for the reveal and the successful retry is retained.

Actual canvas/audio MediaRecorder clip: `E:/CodexTemp/TypingHero/slayer-hit-feel/slayer-hit-feel.webm`
(2663768bytes, SHA256`cbe000a62cbc1a86861b78f67620ef5d9a7e965ff5ffd86dca214428abf9c630`).
Opened in a normal Edge window at2026-10-25T03:26:35.922851+00:00; `open-clip.html` supplies1x/0.25x
playback. It contains the isolated hit, rapid typed sequence and lethal ending.
Encoded frame inspection, opposite-facing capture and subjective game-feel approval
remain unverified. No mirror build, full suite, profiling or publication ran.

The derived ActorDepth index now covers280referenced frames,0missing reported by
the builder. Removing unused Slayer preparation cels prunes the derived index;
authored atlas/depth images are unchanged. Only that index and the two generated
Slayer role prefabs are imported from Unity output; unrelated generated scenes
and UI/material rewrites are not imported.



# Slayer slash impact replacement

## Slayer body-center correction — 2026-10-25

User rejected the left-edge placement in the integrated B clip. Reference:
`C:/Users/ADMIN/AppData/Local/Temp/orca-paste-1792891405560-f00d77d7-e507-4ed1-9bfb-e7f9bea09230.png`.
The old visual review did not establish the newly clarified body-center requirement.
Start from origin/main88953ab in independent Orca worktree slayer-center-fix.
Evidence root: `E:/CodexTemp/TypingHero/slayer-center-fix/`.

Capture the struck collider bounds center before damage/hurt/death; forward it only
to the Slayer sprite branch. Preserve contact for damage/camera/audio and legacy
consumers. Hold world center through each lease; no following/recentering per cel.
Keep original PNG/hash/pivot/scales, six cels F4/F5/F0/F1/F2/F3, 80/100ms timing,
eight nonrepeating angles, tint, pool and lifecycle unchanged. Read-only QA adds
recipient world/screen center, including disabled BoxCollider2D offset observation.

Acceptance checklist: nonlethal/lethal capture before recipient movement/disable;
offset/mirrored/scaled body; unchanged contact consumers; all 8 angles x2 roles x6
cels (96cels/80internal pairs); native real-keyboard hits/overlap/pause/resume/
expiry/restart/finisher/result; 1x/0.25x playback and clean center overlay. Review
every original encoded frame/adjacent pair in declared native windows, retaining
held frames/PTS; source-cel/static review is distinct from native video review.
Unity tests, build, direct visual review and remote integration completed below.

Static output review: opened all four `static-review/{hit,finisher}-{0,1}.png`
sheets at native/2x nearest size, 96/96cels and80/80internal adjacent pairs.
Fixture uses actual sword Hurt_0,16PPU/pivot(.47,.41), mirrored prefab collider
offset,32/40px B scales; gold hit/red finisher distinguish the two roles. F4
starts on the incoming side, F5 reaches the torso, F0 crosses it, F1 breaks into
exit-side dots, F2/F3 leave a terminal speck. All eight orientations preserve
body-relative pivot and clear canvas borders. Per-cel notes/durations/transitions:
`static-review/frame-review.csv`; exact source SHA256 and cells in manifest.json.
This is a model-space composition, not native runtime evidence or a new palette
review. Original source pixels/import/prefabs are unchanged. Native results below.

Scoped Unity2022.3.25f1 tests:51/51PASS,0failed/skipped,5.2562155s in tests.xml.
24SlayerHitVfxTests,18CombatFeedbackTests,9NormalStrikeEffectsTests. New cases
exercise nonlethal/lethal damage moving the mirrored/scaled offset collider and
disabling it synchronously; every cel retains the exact pre-damage center while
damage/LastContact retain the original contact. Legacy/non-Slayer override cases
also pass. Initial input6098dd4c; build inputfb987f97 differs only in the read-only
QA probe fields added after tests began (probe excluded in PlayMode compilation).

Existing Skeleton.prefab hit-pose jitterAmplitude is0.2WU, with jitterRoot on the
same transform as the body. Therefore current collider center during a live tail
is not its captured birth center: two successive jitter offsets can differ by
<=0.4WU. Native current-center distance is bounded by that existing motion;
exact pre-damage capture is established by the synchronous Unity cases. Do not
claim a frozen VFX follows later body jitter or treat the offset as a new anchor.
QA overlay cyan square=current body center, magenta cross=frozen VFX pivot.

Final QA WebGL build PASS, strict repeated builder PASS; immutable33-file player
`b450043660a03328`, hashes in player-manifest.json. Final249 C#/package/version
inputs match the accepted worktree after CRLF normalization; no package/version
or source PNG change. Native Edge156.0.4314.42/Intel UHD/ANGLE D3D11/1280x720:
2772checks PASS,0exceptions/provider reads/writes. Real keyboard/mouse covers
first hit, pause/resume, natural exit, restart, rapid words, lethal hit and result.
All8angles observed;139stable lease observations retain position/tint/angle.
Maximum current-body-to-frozen-pivot distance0.3518551WU is within the existing
two-offset0.4WU jitter bound; it is not a tolerance on exact birth capture.

Video: `video/778132f6ca8d0d3b9c22f71b80556c75.webm`,83.00s/25fps/2075encoded
frames, SHA256787e85bce7b6c75fcca848b54c7bb28c38e4e43d2941f7e6462b61ab869d5e42.
Native event-wall clock differs from encoded PTS; use original decoder timestamps
for review/delivery, not inferred event latency. One-second locator images were
used only to locate complete behaviors before declaring the review windows below.
No window was shortened. Crop410,120–1100,640 encloses every observed B renderer
bound (x599.83–1037.58,y141.75–603.62); full video/screenshots retain context.

| Selected native segment | Original PTS seconds | Frames/pairs inspected | Purpose |
|---|---|---|---|
| hit-pause-resume | 37.00–41.00 | 101/100 | Preparation, first hit, pause hold, resume/full tail |
| normal-exit-restart | 49.50–54.20 | 118/117 | Exit, restart, restored encounter and next normal parry |
| rapid-finisher-result | 72.00–81.60 | 241/240 | Reentry, complete rapid sequence, lethal/death/result |

Directly opened all40numbered native sheets in order,460/460frames and457/457pairs,
including held/repeated frames and sheet boundaries. Per-frame observations and
next-frame notes are in runtime-review/frame-review.csv; scopes/counts/PTS in
manifest.json and video.json. Incoming strokes reach the torso, cross it and thin
into exit-side flecks; no left-edge anchor, pivot spin or canvas clipping found.
Old detached leases remain independent of later hit jitter. Pale first-hit core
is partly occluded by the pause menu/retained weapon trail; overlay and read-only
freeze observations supplement it, and unoccluded rapid gold/red hits establish
actual placement. Broad ivory weapon crescent is unchanged preexisting artwork.
Red lethal tail persists through mode exit and disappears cleanly as bones fall.

Edge playback73.00–75.20s at1x/0.25x PASS: measured2155.8/8625.2ms versus media
2159.9999/8639.9999ms; callbacks coalesce held frames and do not replace the full
PNG review. Source timing is independently covered by Unity. No general FPS,
subjective audio, full-suite, production release or itch.io publication claim.
Delivery stills: centered-in-game.png, pause-center-overlay.png, finisher.png.
Original rejected screenshot retained as rejected-contact-anchor.png.

Fetched newer main2fc3e6a before integration. Its Windows adapter/retry changes
touch only tooling and separate receipts; no Assets/Packages/ProjectSettings
change. Preserve those updates and reuse passing game tests/player/visual evidence.

Remote integration verified: feature5fccdcd merged current main2fc3e6a as3f6a5e6;
fast-forward push2fc3e6a ->3f6a5e69e0cc5afcaec58844c0d488ed69b9283c, then fetch
confirmed HEAD==origin/main and feature ancestry.249source/config inputs still
match the tested player; merged tooling changes do not alter game inputs.

Accepted input archive: accepted-unity-input.tar.gz,20,775,537bytes, SHA256
d748e942a844a22cd2c5135142cb5ed19e36e0473fa3c16caae18abca994f983.
Archived changed sources/source PNG and all33player hashes verified before
disposable source/Library cleanup. Keep video, selected460PNGs,40native sheets,
96-cel static review, per-frame ledgers, tests/build logs, player/source archive and
rejected/corrected stills under the evidence root for user review; delete that
directory only after review. No demo host was started. Browser contexts closed.
After this documentation-only receipt is remotely verified, remove the clean
task-created Orca worktree/local branch and isolated Unity source/Library plus
unselected decoded frame cache. Remote task branch remains; original dirty
checkout and all preexisting resources remain untouched. External completion.json
records final receipt commit and actual cleanup results.

## Met.Pxl B integration — 2026-10-25

This follow-up supersedes the tbbk artwork/timing/mirror contract below only for
SlayerHit and SlayerFinisher. User approved reusing existing B preview pixels,
Slayer-only routing, captured progression tint, and the full 580ms source timing.
No purchase, normal/parry replacement, gameplay/API change or itch publication.

Source: approved [A/B preview](../../design/slash-hit-ab-preview/README.md), full
B GIF0–580ms/6frames; original hash9a6e0a01cefb1b10a7db3b6c250259a1a1b3f414861f136654395674ab5cb139.
White crop640,640–896,896 contains only black/white and exactly uniform4x blocks
in all6frames. `extract_white.py` reduces to64x64 without pixel loss, turns black
into alpha0, and stores white RGB plus binary alpha in384x64 source-order strip.
PNG SHA25662ad20c15fb52b44b4b49f6aa0bb5e583c1ddc5209964530724edc43350f7356.
Original tbbk PNG/license and prefab GUIDs retained; new source notice/credits added.

Runtime contract: F4/80ms→F5/F0/F1/F2/F3 at100ms each, once, then pool release.
Fixed center32,32 at16PPU, Point/no-mips/uncompressed; unrotated visible widths32/40px.
Each confirmed hit chooses an absolute clockwise0/45/90/135/180/225/270/315degree
direction without mirroring. History belongs to the HitSparkSpawner owner and is
shared between hit/finisher; seven equally likely alternatives, eight for first hit.
Rotation/tint/contact remain fixed; owner disable resets history. Existing shared
eight-impact oldest-first reclaim, pause/reading/hitstop and cleanup remain.

Checklist: extraction/source equality; six cel durations; all56 nonrepeat choices;
both role sizes; all21 combo colors0–20 from authored gradient; pool reuse/overlap;
real contact/finisher/tint capture; pause/resume/exit/result/restart; builder stability;
native WebGL1280x720; remote integration and task resource cleanup.

### Static output review

Evidence root: `E:/CodexTemp/TypingHero/slayer-b-integration/`.
Directly opened static-review/combo-00.png through combo-20.png, every cell at
nominal native size and2x nearest zoom. Each sheet contains8angle columns and
12rows: hit F4/F5/F0/F1/F2/F3, then finisher in the same order.
Inspected2016/2016cels and1680/1680within-effect adjacent pairs (336variants).
No static clipping/blur; F4short leading seed grows through F5/F0, breaks in F1,
shrinks in F2 and leaves sparse dots in F3. Every orientation retains its axis
through those phases. Small final fragments intentionally become very sparse at
32px nominal size. Static previews approximate raster scaling; they do not prove
Unity rendering, final release, timing or playback. Final runtime checks are below.
Per-frame observations: static-review/frame-review.csv; source/variant manifest:
static-review/manifest.json. Reference GIF research remains separate from this
output review; no new game/video-reference match is claimed.

### Execution checkpoint

Branch micharel09/slayer-b-integration from origin/main18e03a2; original dirty
checkout preserved. fly-server SSH timed out, Tailscale peer reported offline.
Found existing local Unity2022.3.25f1 plus WebGL module; isolated LF snapshot and
Library under evidence root/unity-source, without touching other Unity jobs.
Initial compile caught an NUnit collection assertion using a string-only overload;
corrected to Has.No.Member. No runtime PASS claimed for that initial attempt.
Snapshot b52a71cfab995666fb2dbf5285968bff050d34ec7ae0dc2646d549960184cb13:
serialize-retry.log PASS (strict two-pass builder); tests.xml52/52PlayMode PASS,
0failed/skipped,6.210376s:20SlayerHitVfxTests,18CombatFeedbackTests,
9NormalStrikeEffectsTests,5affected ParryClashTests. Only generated B texture meta
and two role prefabs imported; Player.prefab differs only in trailing whitespace,
Skeleton.prefab is identical. Unrelated scene regeneration was not imported.
The generated role scale precision is canonical Unity serialization of the same
float values; whitespace stripped for diff-check. All C# files match tested input
after LF normalization; manifests/locks/Unity version are unchanged.

### Final native and visual receipt

QA WebGL build.log PASS, including repeated builder during build. Immutable33-file
player hashes are in player-manifest.json; build version9b813909fdd2dab1.
Native Edge156.0.4314.42/Intel UHD/ANGLE D3D11/1280x720:3153checks PASS,
0managed/JS exceptions/provider requests/writes. Real keyboard/mouse covers live
pause/resume, first Slayer natural exit, Restart, second entry, rapid typing,
Slayer finisher and result cleanup. All8directions observed,179stable observations
of immutable rotation/tint/contact, maximum8concurrent Slayer effects.

Scene tint audit found authored SampleScene overrides: white→gold→red with
21discrete progression states0–20, distinct from the prefab cyan gradient used
by the parameterized unit fixture. No palette was changed. All20colors actually
emitted by typed steps1–20 match the scene ramp; state0white covered statically.
Directly inspected a second complete matrix in scene-static-review/combo-00.png
through combo-20.png:2016/2016cels,1680/1680pairs. Same observations and no
clipping/blur; per-frame ledger/manifest beside those sheets. Including the
retained prefab-color matrix totals4032cels/3360pairs, with evidence kept separate.

Video video/a3b85b4d9b537c9dbe55df710be4ff00.webm:
SHA2568e8ba98118db578bf6cf1bcc4d7ce18810a3c1dca2685d05d1ad069bd33b55df.
Decoded original25fps PTS with the existing Playwright FFmpeg; no resampling,
discarded held frames or inferred frame counts. Selected windows were recorded
before direct review from input events and screenshot/PTS anchors:

| Segment | Start/end seconds | Inspected frames/pairs | Purpose |
|---|---|---|---|
| hit-pause-resume |20.75–23.00|57/56|Preparation, first contact, pause hold, resume and full tail|
| normal-exit-restart |31.80–33.30|38/37|Natural exit, actual Restart and clean restored encounter|
| rapid-first-word |44.95–46.55|40/39|Both impacts of first word “on” through release, with neighboring overlap|
| red-finisher-result |49.20–51.70|63/62|Last two words, lethal red tail, death and clean result|

Directly opened all19contact sheets:198/198encoded frames,194/194adjacent pairs.
Initial actor crop was expanded to330,160–950,640 to include all B bounds; no
time window was narrowed. See runtime-review/manifest.json and frame-review.csv.
Pause controls naturally obscure part of the sprites; repeated visible state and
read-only age/cel/tint/angle observations establish the freeze. The thin B strokes
grow on fixed axes, break into aligned fragments/dots and vanish cleanly. Overlap
retains separate axes/colors; red persists through mode exit and fades before
the result. Original broad ivory sword trails are separate, retained artwork.
Every observed B renderer bound stays inside the1280x720 canvas.

Browser playback-clock check of the full pause/resume window at1×/0.25×:
2213.4/8785.3ms versus nominal2200/8800ms; runtime-review/playback.json.
Presented callbacks can coalesce held video frames; they do not replace the
complete decoded-frame review. No complete-video/full-animation/performance/
broad-suite/production-release or itch publication claim. Implementation and
scoped verification complete; remote integration is recorded below.

### B remote integration and retained evidence

Feature commit753c90505a6c589067785e08e028978cb8064030 pushed to the task branch
and origin/main as fast-forward18e03a2→753c905. Fetched and verified HEAD equals
origin/main; feature commit is an ancestor. No runtime or asset changed after
accepted checks. This receipt update is documentation-only.

Keep `E:/CodexTemp/TypingHero/slayer-b-integration/` for user review: immutable QA
player, input archive, tests/logs, original video and selected-frame/static ledgers.
Delete that evidence directory when user review no longer needs it. No task demo
server is left running. Task-owned Unity source/Library and out-of-scope decoded
frame caches are removed after archiving; shared editor, activation, packages,
other projects and the pre-existing dirty checkout are retained. Remove only the
clean task-created worktree/local branch once this receipt is preserved remotely;
leave the remote task branch. Final cleanup/remote HEAD is in the local receipt.

Base origin/main46d4dda. Worktree C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-slash-vfx.
Original dirty checkout preserved; no itch.io publication.

## Approved contract

Original tbbk CC0 nine-frame64x47 sheet; hit150ms/finisher180ms; visible width32/40px
at16PPU. Center pivot, direction mirror, immutable captured hit tint, no random transforms.
Replace only Slayer hit/lethal bindings. Keep parry, non-Slayer VFX and gameplay.
Eight impact leases per owner; pause/tutorial freeze; finish after Slayer exit;
clear on result overlay/disable/restart/unload.

Source receipt: E:/CodexTemp/TypingHero/slayer-vfx-plan-20261024/asset-source-review.json.
PNG SHA2567642aa4698828f6cc908b3fd2873c3b3d8555138bae188991c6b775ae0c96915.
Attribution/license beside imported asset. ScourgeBringer research remains limited
to the previous recorded scope; no complete reference-match claim.

## Acceptance checklist

Scoped Unity routing/contact/pool/lifecycle; stable two-pass LF builder; direct
per-frame/per-transition review of all eight duration/direction/tint variants;
WebGL1280x720 real input with read-only bridge/provider isolation; local demo;
commit/push/integration/remote ancestry/cleanup. Runtime/asset verification is complete in the final receipts below; integration is verified in the final receipt below.

## Static visual review (2026-10-24)

Directly opened original source sheet and all eight adaptation sheets under
E:/CodexTemp/TypingHero/slayer-vfx-implementation/. Both native reference size and
3x nearest-neighbor are shown in every sheet. All72/72 adapted cels and64/64
adjacent pairs inspected. These are CPU nearest-neighbor previews of the selected
scale/mirror/tint, **not runtime renderer or playback acceptance**. F0 is only
one/two pixels at native hit size; its faint preparation is original artwork.
No clipping, pivot recentering or detached physics fragments observed. At this initial checkpoint runtime
readability/overlap/on-off were pending; final native evidence below supersedes it.

### hit-right-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-right-white | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-right-white.png F0 |
| hit-right-white | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-right-white.png F1 |
| hit-right-white | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-right-white.png F2 |
| hit-right-white | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-right-white.png F3 |
| hit-right-white | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-right-white.png F4 |
| hit-right-white | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-right-white.png F5 |
| hit-right-white | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-right-white.png F6 |
| hit-right-white | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-right-white.png F7 |
| hit-right-white | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side right; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-right-white.png F8 |

### hit-right-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-right-red | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-right-red.png F0 |
| hit-right-red | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-right-red.png F1 |
| hit-right-red | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-right-red.png F2 |
| hit-right-red | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-right-red.png F3 |
| hit-right-red | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-right-red.png F4 |
| hit-right-red | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-right-red.png F5 |
| hit-right-red | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-right-red.png F6 |
| hit-right-red | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-right-red.png F7 |
| hit-right-red | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side right; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-right-red.png F8 |

### hit-left-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-left-white | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-left-white.png F0 |
| hit-left-white | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-left-white.png F1 |
| hit-left-white | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-left-white.png F2 |
| hit-left-white | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-left-white.png F3 |
| hit-left-white | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-left-white.png F4 |
| hit-left-white | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-left-white.png F5 |
| hit-left-white | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-left-white.png F6 |
| hit-left-white | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-left-white.png F7 |
| hit-left-white | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side left; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-left-white.png F8 |

### hit-left-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-left-red | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-left-red.png F0 |
| hit-left-red | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-left-red.png F1 |
| hit-left-red | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-left-red.png F2 |
| hit-left-red | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-left-red.png F3 |
| hit-left-red | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-left-red.png F4 |
| hit-left-red | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-left-red.png F5 |
| hit-left-red | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-left-red.png F6 |
| hit-left-red | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-left-red.png F7 |
| hit-left-red | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side left; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-left-red.png F8 |

### finisher-right-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-right-white | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-right-white.png F0 |
| finisher-right-white | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-right-white.png F1 |
| finisher-right-white | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-right-white.png F2 |
| finisher-right-white | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-right-white.png F3 |
| finisher-right-white | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-right-white.png F4 |
| finisher-right-white | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-right-white.png F5 |
| finisher-right-white | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-right-white.png F6 |
| finisher-right-white | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-right-white.png F7 |
| finisher-right-white | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side right; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-right-white.png F8 |

### finisher-right-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-right-red | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-right-red.png F0 |
| finisher-right-red | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-right-red.png F1 |
| finisher-right-red | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-right-red.png F2 |
| finisher-right-red | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-right-red.png F3 |
| finisher-right-red | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-right-red.png F4 |
| finisher-right-red | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-right-red.png F5 |
| finisher-right-red | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-right-red.png F6 |
| finisher-right-red | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-right-red.png F7 |
| finisher-right-red | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side right; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-right-red.png F8 |

### finisher-left-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-left-white | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-left-white.png F0 |
| finisher-left-white | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-left-white.png F1 |
| finisher-left-white | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-left-white.png F2 |
| finisher-left-white | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-left-white.png F3 |
| finisher-left-white | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-left-white.png F4 |
| finisher-left-white | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-left-white.png F5 |
| finisher-left-white | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-left-white.png F6 |
| finisher-left-white | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-left-white.png F7 |
| finisher-left-white | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side left; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-left-white.png F8 |

### finisher-left-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-left-red | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-left-red.png F0 |
| finisher-left-red | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-left-red.png F1 |
| finisher-left-red | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-left-red.png F2 |
| finisher-left-red | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-left-red.png F3 |
| finisher-left-red | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-left-red.png F4 |
| finisher-left-red | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-left-red.png F5 |
| finisher-left-red | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-left-red.png F6 |
| finisher-left-red | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-left-red.png F7 |
| finisher-left-red | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side left; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-left-red.png F8 |


## Runner checkpoint

First60s lock wait expired before Unity ran (no test/build claim). Retried with
3600s lock wait; shared jobs preserved. Snapshot adapter only normalizes known
text extensions toLF; PNG/font bytes stay intact. Source/build/results isolated
under /tmp/typinghero-slayer-slash-vfx; shared runner lock, pinned image, Library,
cache and private license reused. No changes to repository runner/toolchain.
Static review hashes retained in external artifact root/static-review-hashes.json.

## Initial serialize receipt

Unity2022.3.25f1/fly-server20261024T111720-serialize PASS, snapshot
24a74dd60b207b7f448dac521c95ebe4d8db8b1442212d7f41b2e1f369361270.
Strict repeated builder passed for the initial implementation. This initial
snapshot predates the final root-metadata stability list, tint fallback correction,
expanded regression tests and QA observations; it is not final acceptance.
Receipt archived locally in initial-serialization.tar.gz. Final combined scoped
checks/build started20261024T112049-webgl, input
3f0bb14c2649b8b63ed5bf8a8f796efe0189a8ff658c22d170517c1fbd06c9a6.

## Scoped test pilot

20261024T112049-webgl: strict serialize stage succeeded;50PlayMode cases ran,
49PASS/1teardown failure in OrdinarySlayerContactRejectsClosedDeadAndReadingTargets.
The manually activated PlayerSlayerMode fixture lacked an Animator; OnDisable.Exit
therefore logged NullReferenceException after all behavior assertions passed.
Fixed by undoing the manual active flag at the end of that fixture; no production
fix was needed. Original XML saved as tests-initial.xml. WebGL build did not run.

Imported Unity-generated SlayerHits assets/metas and only the two new role
bindings into Player.prefab. SampleScene inherits that prefab with no Slayer
binding overrides, so no scene edit is needed. Full scene builder output contains
unrelated regenerated UI IDs; retained the authored scene instead. Skeleton.prefab
output has no diff. New asset GUID/sprite IDs are now included in the retry input.

## Final scoped Unity receipt

Unity2022.3.25f1/fly-server20261024T113433-webgl, canonicalLF input
`dc32bba70c576eaf074b00dc3e38f6ba9e675fe7ddf5f4f45c610270ee5d24f1`.
Strict repeated SerializeOnly PASS including every SlayerHits file/metadata.
PlayMode **50/50PASS**,0failed/skipped,8.8784652s:18SlayerHitVfxTests,
18CombatFeedbackTests,9NormalStrikeEffectsTests,5directly affected ParryClashTests.
Covers original PNG hash/slices/pivot/scale/timing, every scheduled cel, both
mirrors, immutable RGB tint/opaque renderer alpha, real contact/duplicate/dead/
closed/reading guards, lethal pre-damage capture, legacy/non-Slayer routing,
shared eight-impact oldest-first reclaim,50/100/200ms cadence, pause/reading,
reuse, post-finish tail, result/disable/unload cleanup and old parry isolation.
No gameplay, typing/balance or broad animation suite change/claim.
WebGL build and native visual verification remain pending at this checkpoint.

## Native and selected output review scope (recorded before frame inspection)

Immutable player9b76edc5beddd984,33/33local/server SHA256 match. Native2 PASS:
Edge156.0.4314.42/NVIDIA GTX1650Ti/ANGLE D3D11/1280x720, real keyboard/mouse,
read-only bridge; no managed/JS exceptions or provider calls. Native1 driver
paused after the150ms tail had already expired; no runtime failure. Native2
pauses immediately after the render-frame contact observation and verifies an
unchanged frame_3/age0.06154000014/tint during pause, then resumes.

Video: video/24179a6590825748b109d10a5280aa59.webm, SHA256
544ad07cda76ff7daf7424b6010b3955ee53b1e022f1cb50082ba8271d3aeaac. Original ffprobe PTS retained; passthrough frame decode,
no fps resampling or dropped held/repeated frames. Event clock is approximate;
windows include context and are validated visually, not treated as precise input
latency measurements. These are output review, separate from reference research.

| Segment | Start/end seconds | Frames / pairs | Reason | Inspected |
|---|---|---|---|---|
| hit-pause-resume | 25.5?27.85 | 59/58 | Slash birth, pause hold, resume and complete tail | pending |
| normal-exit | 36.4?38.3 | 48/47 | Full mode exit and actors without impact | pending |
| white-rapid | 55.65?56.85 | 30/29 | First complete word after second entry, overlapping white hits | pending |
| red-finisher | 61.3?64.1 | 70/69 | Final three words, overlapping red hits and lethal tail | pending |

207frames/203pairs selected across four complete behavior windows; review pending.

Visual validation found the event clock increasingly differs from encoded PTS:
initial white-rapid contains idle preparation, and red-finisher ends before the
actual lethal tail. Before reviewing extra frames, expanded coverage to55.50?60.20s
and61.30?video end68.60s; retained original frames/PTS and their review evidence.
No window was shortened. Extensions listed in runtime-review/manifest.json.
The unchanged large ivory weapon trail is distinguishable from the smaller gray/
red hit crescent; empty-impact samples retain that original trail. Do not label
that preserved artwork as a new Slayer hit or claim it was removed.

## Final visual acceptance

All72/72static adaptation cels and64/64source-adjacent pairs inspected at native
reference size and3xnearest; all407/407selected encoded output frames and403/403
adjacent pairs inspected, including extension boundaries. Four final windows:
25.50?27.85s (59/58),36.40?38.30s (48/47),55.50?60.20s (118/117),
61.30?68.60s (182/181). Numbered source images and per-frame runtime notes/PTS/
sheet hashes are in runtime-review/manifest.json and per-frame-review.csv.

Small gray/white crescent opens at contact, becomes a thin lower trace and clears;
white-to-yellow/red rapid hits overlap without producing falling fragments or a
continuous new opaque body mask. Existing ivory weapon trail and parry particles
remain visible by design; empty-impact samples show that original large trail
without the new crescent, providing an on/off comparison in the same scene.
Native observations saw up to two concurrent slash impacts; Unity tests cover the
full eight-impact limit. The larger red finisher remains red as Slayer ends,
vanishes before enemy death/results, and leaves no retained effect. Source alpha,
fixed pivot, no random motion and all nine original cels are preserved.

Both directions and endpoint tints/durations were directly reviewed in the static
variants; native real-input production combat covers its default right-facing
path. Encoded25fps output quantizes visible cels, so this is not a claim that every
16.667/20ms source cel appears in that recording. All nine scheduled cels and both
mirror flags are independently verified by Unity. No full character-animation,
reference-game equivalence, performance/stress/soak or subjective audio claim.
No artwork, shader, controller or lighting-map changes outside the new asset root.
Only trailing YAML whitespace in new assets/metas was trimmed for diff hygiene;
all generated values/bindings/IDs match the accepted Unity export.

## Demo and integration checkpoint

Local immutable QA demo: http://127.0.0.1:8364/ . Host PID25340,
E:/CodexTemp/TypingHero/slayer-vfx-implementation/host.py. Fresh native browser
smoke PASS: player loads at1280x720, service worker controls navigation, explicit
Firebase GET returns X-Clackblade-Demo:local-fixture,0real external requests or
page errors. Source player33hashes remain unchanged; only host sizing/bootstrap
and provider fixtures wrap it. Host/evidence remain for user review; stop PID25340
and remove this task artifact directory only when the demo/evidence are no longer
needed. No itch.io publication.

Upstream7d69938 changes production-only Brotli and its scan, plus documentation.
QA remains uncompressed; runtime VFX/gameplay sources and assets are unaffected.
Preserve both that upstream behavior and this task's appended source notice when
integrating. Reuse the scoped/runtime results for that unrelated merge; no second
QA/full-suite build is required by an unchanged QA path.

## Remote integration receipt

Feature commit67a94d9; merged upstream7d69938 as09e3389. Push to origin/main was
fast-forward7d69938?09e33897fe27aee15d024648f57b891f6f64e286, then fetched and
verified HEAD==origin/main and67a94d9is an ancestor. Both verification entries
were kept when resolving their documentation-only conflict. All245C#source files
other than the reviewed production build helper still match the accepted LF input.
No VFX/gameplay source or asset changed after the passing build; QA compression
semantics stay unchanged. Source PNG hash and generated sprite/prefab bindings
were rechecked. The original dirty checkout was never checked out or modified.

A final documentation-only receipt commit records this integration and cleanup.
Retain remote task branch micharel09/slayer-slash-vfx (no automatic remote deletion).
Remove the clean task-created worktree/local branch after confirming that receipt
on remote main. Remove only /tmp/typinghero-slayer-slash-vfx after verifying no
active container mounts it; all required player/source/receipts are already archived
under the local demo/evidence root. Shared Library/toolchain/cache/license and all
pre-existing resources remain untouched. Demo host/evidence retention is described
above; its cleanup condition is completion of user review.


## Shared chest anchor implementation (2026-10-25; active)

User intent: B follows the center of the visible rib cage; non-chest enemies author
main-body center. EnemyHitAnchor lives beside IDamageable, explicitly binds a body
renderer and a sprite-pose profile. Keep public IDamageable/Spawn, PNG/GUIDs, black,
120/160ms, overlap/fade and combo_end. No test suite execution authorized.

Worktree C:/Users/ADMIN/orca/workspaces/TypingHero/enemy-chest-anchor,
branch micharel09/enemy-chest-anchor, base origin/main24f74e0. Original dirty checkout
is preserved. Evidence: E:/CodexTemp/TypingHero/enemy-chest-anchor.

Coverage required: ske plus sword/axe/sword-shield, all reachable authored poses;
single strike, pose change, rapid hits, lethal tail, pause, actor lifecycle/pool reset.
Three current Endless forms share profiles across six roles. Sprite coordinates are
bottom-left rect pixels, converted using actual pivot/PPU/flip/body transform.
Root follows in LateUpdate; child compensates alpha-weighted pixel-center centroid
baked by SlayerHitVfxAssets for every scheduled B cel. Rotation/world scale stay
fixed. Missing/invalid/duplicate pose entries fail the build. New enemies supply
profile data and an explicit renderer; VFX has no enemy-name/type dispatch.

Direct static inspection: all210 numbered original poses and210 marked-anchor
poses were opened at3x nearest, with original alpha. Inventory and22 source/anchor
sheets are in the evidence folder: sword58, axe58, sword-shield58, ske36. Chest
tracks the rib cage, excluding weapons/shields/head; collapsed Die poses use the
remaining torso pile. This is anchor-authoring evidence, not an exhaustive review
or modification of existing character animation timing. No reference game research
is needed: the user's landmark and the existing project sprites are authoritative.

Cache/build: copying the prior task's existing Unity Library was stopped after
copying usable artifacts because disk space was low. Automatic approval rejected
recursive deletion/junction replacement; no deletion/replacement was performed.
The copied cache is retained. Shared source/cache and earlier task evidence remain.
Build/native clip/Edge playback/integration receipts are pending; no test PASS or
runtime/visual acceptance is claimed at this checkpoint.


### Chest anchor coverage and output review

Extended coverage after native evidence revealed held ParryWeaponContact sprites,
assigned by ParrySystem outside controller curves. Directly inspected all6 original
held cels and their marked anchors, adding7 profile entries (ske uses contact0;
sword0/1, shield2/3, axe4/5). Final217 entries: ske37 and60 per Endless form.
Build validation now follows ParryContactLibrary source-to-contact substitutions,
so future held cels cannot silently miss the profile. No sprite artwork was changed.

Unity2022.3.25f1 builds reviewed and parry-complete exit0; strict repeated builder
passes. Final input1e261d0d48ebb2a603b0859f41b3d8d3b1b95495cc92ef8c123c9d0fe0c6077b,
player a0bdc30c8ff8344a. All252 C# files and final profiles match the build snapshot.
PNG B SHA25662ad20c15fb52b44b4b49f6aa0bb5e583c1ddc5209964530724edc43350f7356
matches HEAD. Exported only the task prefab/resource bindings; scene/UI IDs stay at
HEAD because the scene inherits the anchor through Skeleton.prefab.

Native full capture105d5465563bb43d: Edge156.0.4314.42 / Intel UHD / ANGLE D3D11 /
1280x720, real keyboard/mouse, observation-only bridge and external/provider fixture
interception. Four duels cleared, HP35;225/228correct letters,18parries. Zero observed
JS/managed exceptions or fixture reads/writes in this qualified capture. All645
active samples have a valid chest and exact root-to-chest distance0; all four B cels
observed, maximum2 simultaneous impacts. Pause holds identical age0.03528/frame0/
position/tint over30 observed frames. No test suite was executed.

Three output scopes were declared before decoding/review; original variable PTS and
held/repeated frames were retained. Directly opened every sequential sheet at a
readable crop, including all adjacent pairs and segment boundaries:

| Form | Source video seconds | Frames inspected/selected | Pairs inspected/selected |
|---|---|---|---|
| sword |11.758–21.893|239/239|238/238|
| axe |67.080–77.772|293/293|292/292|
| sword-shield |90.845–101.551|276/276|275/275|

Exact declared timestamps and original numbered PNGs/PTS/sheet hashes are in
runtime-review/manifest.json; per-frame observations/transitions are in
runtime-review/per-frame-review.csv. Source/marked pose evidence is distinct in
anchor-review.csv and the22 original/marked sheets plus parry-poses/parry-anchors.
Static B offset review covers48 composites/36 adjacent B-cel pairs across three
Hurt poses for each of the four profiles, with per-frame notes in b-static-review.csv.

The black cut stays in the torso during isolated letters, Hurt pose changes and
rapid swings; the final tail clears during death without a stationary patch left
at the former body position. The preserved large ivory weapon arc is separate.
Some black-on-black and arc-occluded frames cannot visually isolate the tiny B;
those limits are explicit in the frame notes. Chest geometry is checked against
visible source ribs and authored markers, never collider center. Body pulse is
opaque ivory in some frames, so it exposes torso placement rather than individual
rib lines. No subjective game-feel, speaker, performance or subframe claim.

The final augmentation changes only the Editor validator and adds previously
missing held-parry entries. All210 previously captured anchor entries and runtime
code remain identical; no active B in the qualified capture used a missing entry.
Source-comparison.json records this reuse boundary. Thus the three-form clips and
exhaustive selected-frame review remain applicable to their unchanged poses; they
are not mislabeled as a full capture of the augmented player's new held-pose path.

Unverified synthetic cases: destroying/hiding an enemy within its120/160ms tail,
reusing the same live actor while old slashes remain, and reverse-facing native
combat. The implementation handles these through captured point/lifetime, enable
revision and pool cleanup; the real capture exercises ordinary death, scene reload
and pause, not those artificial cases. Build validation is not a runtime test pass.

Authoring a new enemy: create Combat/Enemy Hit Anchor Profile, add a pixel point for
its body sprite and each reachable pose (main torso center if it has no chest),
then bind body/profile on EnemyHitAnchor beside IDamageable. New EndlessOpponent
assets reference the matching profile; ConfigureEndless starts a new lifetime.
No VFX enemy-name/type dispatch is required.

### Pending final integration / supplemental smoke investigation

All three exported clips completed playback in Edge at1x and0.25x (playback.json),
and open-clips.html was opened in a visible Edge window. The full-capture visual
review above remains qualified for its stated revision and unchanged active poses.
Supplemental augmented-player smoke attempts did not reach Slayer: one tutorial
handoff timing failure, then failed parries/player death. Reports native-smoke*.json
are retained as failures, never counted as successful acceptance. Diagnostic video
shows a delay between the driver's key marker and the visible pose; focus calls in
the critical timing path were removed for the latest attempt. Outcome of that
attempt remains pending. No runtime code changed during this investigation.

At this checkpoint, local command execution became nonresponsive, including a
minimal cmd.exe echo. Several owned command sessions remain pending; do not rerun
mutations until their actual outcome is checked. Source/build/clip artifacts and
the task worktree are retained. No task commit, push or main integration has been
verified. Do not report the task complete or merge while the supplemental behavior
and final Git state remain unresolved.

Recovery narrowed the command issue: cmd.exe from C:/ and Git against the task
worktree respond immediately; reading the evidence directory on E:/ still hangs.
The supplemental smoke outcome therefore remains unavailable. Preserve the task
branch/worktree and evidence cache until E:/ is accessible and the smoke result
has been inspected. Saving a task commit does not authorize calling acceptance
complete; main integration remains withheld for this unresolved verification.


### Fixture/lifecycle verification continuation (2026-10-26)

Recovered branch284601a after its former C: checkout was removed by an already
running external worktree-removal process. Orca restored the same branch
`micharel09/enemy-chest-anchor` at
`D:/Projects/TypingHero-worktrees/enemy-chest-anchor`; dirty primary preserved.
`E:/CodexTemp/TypingHero/enemy-chest-anchor` is accessible now; earlier access-failure
notes describe the previous session, not current state.

The five requested inactive actor fixtures now explicitly bind body renderer and
initial ske profile. ConfigureEndless still selects the EndlessOpponent profile;
classic/tutorial paths retain ske. All call sites reviewed: the other pre-existing
test uses the real scene prefab. No production fallback, public API, artwork/GUID,
duration, overlap or combo_end changes in this continuation.

User steering requires Ubuntu fly-server for Unity. The initial owned Windows
Editor was stopped before a result (runs/selected-20261026-131305). Ubuntu061604
was stopped after the new pause test blocked on WaitForEndOfFrame in batchmode;
replaced with normal frame yield. Neither attempt counts as PASS. Busy-runner
attempts produced no test results and are not passes.

Ubuntu Unity2022.3.25f1 filtered run
`/data/typinghero-game-loop/results/20261026T062003-quick/tests.xml`:
18/18Passed,0failed/skipped,40.6681716s,
input639c4c07a52f757e907129267e4b71a0227827225c9290835a6098d2241e1c3a.
Local XML: `E:/CodexTemp/TypingHero/enemy-chest-anchor/runs/ubuntu-062003-tests.xml`.
Exact requested methods: the five fixtures in the user plan;
ChestGeometryAndEveryPosePreserveWorldAngleScaleAndAlphaCentroid(2),
HiddenDestroyedAndDeadActorsKeepLastPointAndReleaseOnSchedule(2),
ActorNewLifetimeReclaimsOldSlashAndPoolReuseFollowsOnlyNewRecipient(2),
AttachedPauseHoldsAgeFramePositionAlphaAndResumesFollowing(1),
HitboxCapturesChestBeforeDamageAndKeepsContactSeparate(2),
EveryFrameKeepsBlackAndAngleUntilRelease(2),
TutorialPauseHoldsAndResultClearsEvenWhenFinished(1), and
RecipientOwnsOnePrimaryAndOneThirtyMillisecondResidualAndFinisherReplacesBoth(1).

Final supplemental revision explicitly asserts seven held-parry mappings and
extends hitbox callbacks to deactivate or immediately destroy actors for both
roles. These two changed methods require a fresh6-case result; their old4-case
result is superseded. Other14unchanged cases remain valid. Supplemental result,
final build, native capture/frame review and integration remain pending.

External final-capture driver keeps stage markers in memory, focuses canvas once,
records keydown/up and read-only combat timing, saves failure state/clip, and
rejects missing Slayer frames/incomplete three-form runs. Its files and previous
failed captures remain under the existing evidence root. No final acceptance yet.


Continuation source-image inspection: directly opened the existing marked
anchor-sword-Hurt.png, anchor-axe-Hurt.png and anchor-sword-shield-Hurt.png under
the evidence root. All5poses per form (15images,12within-sheet adjacent pairs)
were inspected. Hurt0/2/3/4 markers sit within visible ribs; Hurt1's white pulse
obscures rib detail but the marker remains within the shifted main torso. Across
Hurt0->1->2->3->4 the marker follows the torso displacement, not weapon/shield
bounds. These are static source/anchor comparisons, not final native video review.


Supplemental Ubuntu result063715:6/6Passed,0failed/skipped,0.2530389s,
input1deb30f4c1c92bd767450571d8fa4c63815a3662dda1f6b09a92b69d45891647.
Remote XML: `/data/typinghero-game-loop/results/20261026T063715-quick/tests.xml`;
local XML: runs/ubuntu-063715-tests.xml. Both geometry cases assert all7held-parry
mappings; four hitbox cases cover both roles with deactivate/destroy callbacks.
The exact union is20unique final cases:14unchanged from062003 plus6from063715,
allPassed,0failed/skipped. Evidence manifest: runs/selected-final.json. No full
suite/stress/soak/multiple-viewports were run. Production code remains unchanged.


Build attempt064732-webgl (Ubuntu) failed its existing protected-font byte guard
on CobblestoneUI64.asset. The external adapter initially retained CRLF, unlike
this task's earlier LF adapter; retry normalizes UTF-8 text to canonical LF without
changing source, protected assets, guard code or repository runner/CI. Failed log
remains at `/data/typinghero-game-loop/results/20261026T064732-webgl/unity.log`.

Old-player driver pilot is separate from final-build acceptance. First pilot
(driver-pilot-20261026) correctly failed with0Slayer frames and saved state/video:
tutorial button was clicked before DungeonSession transition readiness. The driver
now waits for scene + current word + !dungeonTransitioning and lets two startup
frames settle before tutorial input. Second pilot(driver-pilot-ready-20261026)
completed tutorial transition,4natural parries, Slayer, isolated hits and pause/
resume,190recorded Slayer frames,0exceptions/provider reads/writes, on old player
a0bdc30c8ff8344a. Native Edge156/GTX1650Ti/ANGLE D3D11/1280x720. No gameplay timing,
parry window, damage or QA mutation was changed. Final Ubuntu player still pending.


Additional direct source inspection in this continuation: unmarked sword-Hurt,
axe-Hurt and sword-shield-Hurt sheets (15poses) compared with their15marked poses;
also parry-poses.png and parry-anchors.png, all6held cels in each. Contact0's marker
is centered in the upright ribs (shared ske/sword mapping); contact1 follows the
lowered/leaning sword torso; shield2/3 markers stay in exposed ribs despite shield
and arm extension; axe4/5 stay within the raised/sideways-strike rib cage. These six
held cels belong to distinct attacks, so no artificial six-cel loop is inferred.
Seven profile mappings are independently exercised by both geometry test roles.


### Final Ubuntu/native acceptance (2026-10-26)

Ubuntu Unity2022.3.25f1 build070505-webgl completed successfully, including the
strict repeated builder and protected font/material checks. Canonical LF snapshot
input15bd6781c5fc751631c7ae323a7eea1b03f577796d3d5256e301a831535e55bf;
player104dd2400c6dba53. All2494input files (252C#) match the pre-integration
worktree91f7cad after UTF-8
CRLF->LF normalization;33player SHA256receipts are in runs/final-build-receipt.json.
The snapshot normalization is the only input difference from the passing scoped
tests; code/config/asset content is unchanged. B PNG remains
62ad20c15fb52b44b4b49f6aa0bb5e583c1ddc5209964530724edc43350f7356.
No cache deletion/junction replacement or repository runner/CI change occurred.
Final immutable remote player, input archive and log:
`/data/typinghero-game-loop/results/20261026T070505-webgl/`.
Local player: `E:/CodexTemp/TypingHero/enemy-chest-anchor/final-ubuntu-player`.

Final capture: `final-capture/chest-anchor.webm`, SHA256
9694e371970eecee90d61c40e955e62fcd16bc21af03d1202c12652cf496a560;
report native-final-ubuntu.json, original events/diagnostics clip.json.
Native Edge156.0.4314.42 / NVIDIA GTX1650Ti / ANGLE D3D11 /1280x720;
real keyboard/mouse, read-only bridge, provider fixtures/external blocking retained.
Four duels completed, HP32,17parries,0exceptions/provider reads/writes.
1299active frames /1455impact observations have valid chest points and root-anchor
distance0; blackRGB, all4B cels and maximum2overlap verified. Pause holds identical
age/frame/point/tint/angle across48sword,52axe and55shield observations.
Active poses include Hurt0..4 and Die0/1. Measurements are independent of visual review.

One late parry is explained, not hidden: Space key10 arrived at browser performance
time68207.485ms, after frame2474(68178.300ms) already showed HP40->32 and no parry
window. Frame2475(68215.945ms) first showed the open window. Thus input delivery
was at least29.185ms late relative to already observed damage. Stale QA observation/
driver delivery explains this miss; no parry-window, damage or speed adjustment.
Exact key and surrounding frames: final-capture/late-parry-diagnosis.json.

Final source video scopes were declared before decoding/review in
final-capture/runtime-review/declared-scopes.json. All numbered frames and adjacent
pairs, including sheet boundaries and held/repeated frames, were directly inspected:

| Form | Original video seconds | Frames inspected/selected | Pairs inspected/selected |
|---|---|---|---|
| sword |20.361-31.061|240/240|239/239|
| axe |74.224-86.941|273/273|272/272|
| sword-shield |99.338-111.821|276/276|275/275|

Initial shield cut ended111.817; export included one additional held Continue frame
F2350 at111.821. Its supplemental scope was declared before opening that raw frame
and comparing2349->2350; original scope was expanded4ms, never narrowed. Total789
frames/786pairs:66sequential sheets plus the additional original frame. Exported
MP4 frame counts and relative PTS match source exactly within floating-point noise;
no FPS conversion/dropped held frames. See manifest.json, per-frame-review.csv,
direct-observations.json, supplemental-declared-scope.json and export-timing.json.

Visible horizontal/diagonal B extensions cross the current torso region through
upright/leaning Hurt poses; the large ivory weapon arcs remain separate. Death
shows no stationary patch at the old chest after the short tail clears. Exact B
center is difficult to isolate against the black silhouette/shield; Pause UI and
ivory arcs add occlusion. In particular sword389-412, axe1500-1524 and
shield2119-2147 have the torso covered by Pause; the ledger records individual arc/
diagonal frames and black-on-black limits throughout the strike phases. Do not
claim independent visual centroid visibility in those interiors. Source ribs/
authored markers, actual texture-centroid tests and numeric root-anchor data are
separate evidence. No collider-center, full-raw-video, native reverse-facing,
performance or speaker-listening claim.

All three final MP4 clips completed Edge playback at1x and0.25x. The first playback
window closed before the last shield0.25x pass; five completed rows are preserved
in playback-interrupted.json, and only the missing pass was repeated. Final6rows
in playback.json all reach the media duration. Frame-callback counts are playback
observations, not exhaustive-review counts. Viewer: final-capture/open-clips.html.

| Lifecycle/geometry acceptance | Final evidence |
|---|---|
| Real pivots, X/Y flips, rotated/nonuniform mirrored parent and reverse-facing strike | Geometry cases, both roles; independent basis calculation <=0.0001world units |
| Live pose changes and fixed angle/world size | Every profile pose with actor motion; geometry cases, both roles |
| Seven held-parry mappings | Explicit7mapping count; all corresponding real sprites visited, both roles |
| Dead visible body, hidden renderer/actor, destroyed actor | Lifecycle cases, both roles; follow or last-point freeze and120/160ms release |
| Actor vanishes inside damage callback | Four fresh hitbox cases: deactivate/destroy x normal/finisher, separate real contact |
| Disable/enable and ConfigureEndless reuse | Old lease reclaimed; real heavy profile selected; same pooled VFX follows only new moving recipient |
| Pause/resume | Age/frame/position/alpha held for both roles; real LateUpdate resumes; native pause evidence for all3forms |
| Alpha centroid, frame offset and pool reset | Original texture weighted pixels transformed through actual renderer match root; four frames, both roles; real instance reuse |
| Duration and overlap | Existing EveryFrame/Recipient tests plus native maximum2overlap |

All requested scoped test and final-build/capture/review gates are satisfied with
the explicit visibility limits above. Ready for commit/push/integration; remote
integration verification remains a separate final step. Retain all prior failures,
XML, player/input archives, clips and review artifacts under the existing E: root
and the immutable remote result until user review/reproduction is complete.


### Verified integration with current main (2026-10-26)

Task verification/fixture commit91f7cad was pushed, then origin/mainb68b020's
first-word-preview change was merged as
b9754858ce991b00ab32eec3d053c8b09e85d027, preserving both implementations.
No conflict or additional task runtime edit. Exact normalized source comparison
(runs/merged-source-comparison.json):2490/2494build inputs identical; only
TypingManager.cs, DuelIntroPresentation.cs, GameLoopProbe.cs and DuelIntroTests.cs
changed upstream. Every VFX/anchor/profile/rendering asset and relevant combat
runtime file remains identical. Thus the full three-form B capture/frame review
and20scoped cases are reused for their unchanged behavior; they are explicitly
pre-integration player evidence, not a newly built merged player.

Affected intro/typing interaction was rechecked on merged source in Ubuntu
Unity2022.3.25f1 run074357-quick:3/3Passed,0failed/skipped,9.1915085s,
input9356c4be2ad703a4fc1046036e31b641609a899856bab93a48ef5e7e64c139b3.
Exact cases: FullIntroBlocksTypingDamageStatisticsAndSchedulesFirstAttackFromHandoff;
ShortPreviewPausesWithoutConsumingWordAndCancelDoesNotSignalCombat;
CorrectCharacterAppliesContactPoseBeforeOpeningTheExistingDamageWindow.
Local XML runs/ubuntu-074357-merged-tests.xml and merged-tests-receipt.json preserve
the exact set. This brings scoped evidence to23distinct passing cases, with no
full suite or redundant build/native capture of unchanged B behavior.

The verified merge HEAD was pushed fast-forward to origin/main, fetched again,
and both b975485 and91f7cad were confirmed ancestors of remote main. This records
actual integration, not only a pushed task branch. A final documentation-only
receipt commit follows; its remote/main ancestry and clean-worktree cleanup are
recorded in `E:/CodexTemp/TypingHero/enemy-chest-anchor/runs/remote-integration.json`.
No publication/deployment. Keep the existing E: evidence/cache and immutable Ubuntu
result for review/reproduction; remove only the clean restored D: task checkout
and local branch after final remote verification. Pre-existing worktrees and remote
branches are preserved.
