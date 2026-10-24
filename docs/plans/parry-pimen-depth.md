# Pimen parry core and physical depth

## Contract and checkpoint

- Base: origin/main 46d4dda1; isolated Orca worktree `parry-pimen-depth`, branch
  `micharel09/parry-pimen-depth`. Original dirty checkout preserved.
- One 40 logical pixel core, capped 2.5 world units, white tint/material gain 1.
  Seven 48x48 cells, pivot (24,24); six visible cells at 80ms, expiry 480ms.
- Preserve the existing 22 physical particles, random draws, launch/drag/gravity,
  floor bounce/rest and recipient snapshot. Only specks change channel 2 -> 3.
  Bindings: core0, fragments1, near2, specks3, far4. No arc.
- Preserve camera, 3D depth testing, point pulse, bloom, sound/hitstop/pools.
  No itch.io upload. Integration requires scoped checks and complete runtime QA.

## Source and reproducibility

[Pimen Hit Spark](https://pimen.itch.io/battle-vfx-hit-spark), free Hit Effect 01.
Author page rechecked 2026-10-24: personal/commercial use and modification allowed.
Original assets/license retained. `tools/game_loop/export_parry_pimen.py` exports
only original layer pixels; `Assets/Art/VFX/ParryClash/PimenSource.json` records
source/output SHA256, layers, zero-based frames and exclusive top-left crop bounds.
All seven full composites match shipped palette-one PNG RGBA byte-for-byte.
Core excludes Particle/Particle2; no redrawing, resampling or generated artwork.

Reference scope: F0 glint, F1 peak, F2-F3 opening, F4-F5 orange dissipation,
F6 transparent; detached ray/fragment/speck crops. Runtime scope: preparation,
real-input contact in both directions, mirror, 480ms core end, complete physical
tail/bounce, occlusion and final expiry; pause/resume/reuse/restart. Reference-video qualifications are recorded below; runtime review is complete.

## Direct asset review

Artifact root: `E:/CodexTemp/TypingHero/parry-pimen-depth`. `core-right.png` and
`core-left.png` show every cell at source size and 3x nearest-neighbor. Both images
and `crops.png` directly inspected. This is asset review, not runtime certification.

| Variant | Frame | Duration | Source | Observed frame | Next transition | Status |
|---|---|---|---|---|---|---|
| Right | F0 | 80ms | F0 | Small ivory/yellow diagonal glint, central anchor | F1 expands into full white peak | PASS |
| Right | F1 | 80ms | F1 | White center, yellow spokes and long upper-right spike | F2 opens white center, increases orange | PASS |
| Right | F2 | 80ms | F2 | Orange perimeter, yellow inner spokes, split right rays | F3 center narrows and shifts within unchanged canvas | PASS |
| Right | F3 | 80ms | F3 | Narrow yellow center with orange right spoke and upper tips | F4 removes filled center | PASS |
| Right | F4 | 80ms | F4 | Thin orange skeleton, detached tips; no physical-layer pixels | F5 fragments remaining thin rays | PASS |
| Right | F5 | 80ms | F5 | Sparse orange fragments, transparent former center | F6 clears the residue at expiry | PASS |
| Right | F6 | terminal | F6 | Entirely transparent | Nonlooping end, no return to F0 | PASS |
| Left | F0 | 80ms | mirror F0 | Small ivory glint mirrored in fixed 48px cell | F1 expands to left-facing peak | PASS |
| Left | F1 | 80ms | mirror F1 | White/yellow center, long upper-left spike | F2 opens with orange on left perimeter | PASS |
| Left | F2 | 80ms | mirror F2 | Split left-facing spokes, same intact white center | F3 narrows center | PASS |
| Left | F3 | 80ms | mirror F3 | Yellow center and left orange horizontal spoke | F4 becomes hollow orange skeleton | PASS |
| Left | F4 | 80ms | mirror F4 | Thin orange spokes and detached tips | F5 breaks into sparse residue | PASS |
| Left | F5 | 80ms | mirror F5 | Isolated orange pixels, center transparent | F6 clears at expiry | PASS |
| Left | F6 | terminal | mirror F6 | Transparent cell | Nonlooping terminal state | PASS |

Inspected assets: 7/7 cells and 6/6 consecutive pairs per direction. Crops:
3x5 yellow diagonal cluster, 4x3 white bent fragment, 1x1 white speck inspected.
Runtime material/size/timing and depth evidence are recorded below.

## Verification history

Static verification PASS (`static-verification.json`): protected runtime/MetalClash/
GUID files byte-identical to base; physics differs only in speck channel; all prefab
local references resolve. Initial Quick/serialize submissions timed out600s waiting
for the shared lock, executing no tests. An obsolete queued QA snapshot was
cancelled only after exact server-code SHA and lock-wait state verification to
include the missing-default null-reference fix. No unrelated process was signalled.
The completed scoped runs and final QA follow below.

## Primary-source motion reference

Selected scope before output/runtime review: the requested seven palette-one PNG
cells, Aseprite F0-F6, 0-560ms including the final transparent 80ms cell. This
covers glint, peak, opening, dissipation and clear. Aseprite palette variants and
blank separator cells outside that requested PNG sequence are outside scope.
`source-palette-one.gif` and `.mp4` are explicitly local exports of the author PNG,
not footage downloaded from the author. Video contains 7 frames at 80ms (560ms),
SHA256 e9131a95a4fcb7b1ea45eb426c16cf196263f320c095596f1c9af69254e14569.
Original-size and 3x source cells in `source-palette-one-review.png` all directly
inspected, 7/7 frames and 6/6 consecutive pairs. Native Edge source-video playback
completed at 1x and 0.25x (`source-playback.json`). Source has no audio.

| Source frame | Duration | Observation | Consecutive transition |
|---|---|---|---|
| F0 | 80ms | Tiny centered yellow/ivory glint; surrounding canvas clear | F1 expands at same anchor into white peak |
| F1 | 80ms | White center with yellow spokes and white detached upper fragment | F2 opens core and sends detached yellow clusters outward |
| F2 | 80ms | Yellow/orange core and distinct upper/right particle clusters | F3 thins center while particles separate farther |
| F3 | 80ms | Narrow yellow center, orange lower/right rays and detached upper flecks | F4 removes central fill and thins both core and debris |
| F4 | 80ms | Sparse orange skeleton with small detached particles | F5 breaks skeleton into isolated residual orange pixels |
| F5 | 80ms | Disconnected orange residue and clear center | F6 removes all visible residue |
| F6 | 80ms | Entirely transparent | One-shot terminal clear; no loop comparison required |

The author-page public GIF #5 was directly inspected (6/6 frames, 5/5 pairs);
it depicts another effect and is excluded as a match reference. GIF #6/#7 first
images likewise show another effect. Embedded YouTube S7XbucLRiA8 was found but
could not be fetched; no online-video comparison is claimed. The exact requested
Hit Effect 01 source remains the authority. Temporary remote reference export
was removed after local hash/count/duration verification; local source artifacts
remain with the task evidence.

- Unity 2022.3.25f1/fly-server result 20261024T111935-webgl, immutable input 3d8e8ccae079c31c54409a4501eeaed100829f93da38703a36c71077a6984144: 43 executed, 42 passed, 1 failed, 0 skipped, 10.5037729s. Material/crop/importer, all22 seed traces, null-prefab builder/repeat/custom reference, standalone MetalClash and lifecycle cases passed. Failure: exact particle lifetime comparison differed by 3e-8 seconds due to Unity float representation. Only that assertion gained 1e-6s tolerance; runtime/assets unchanged. Retry selects that one test then builds the immutable QA player; no full-suite rerun.
- Retry input ff2373a9d0e606b68262df6eb2c7cc114c54513aed5dc483cf7123a462442624, result 20261024T112344-webgl: remaining test PASS 1/1, 0 failed/skipped, 0.102281s (24 seeds x two directions). All43 scoped cases qualified across compatible inputs. Actual immutable archive comparison covers2433files; only ImpactVfxTests.cs changed. Runtime/assets identical. Strict repeated builder PASS in actual build log line3115. QA WebGL and native results completed below.

## Completed runtime acceptance

- Normal QA result20261024T112344-webgl and mirrored20261024T114327-webgl:
  WebGL build and strict repeated builder PASS. Mirror input
  e6691e95a14fd5bf76a40bad01a7476d31c5369b185964da94c2678ae69d089a uses an
  external prebuild fixture reflecting actor X/facing only; no product source edit.
  Tests reuse the compatible passing runtime/assets above. All33 downloaded files
  per player hash-verified (`player-right-receipt.json`, `player-left-receipt.json`).
  Compiled WASM/framework identical between variants; all68 serialized VFX files
  identical.67/68 match local text-normalized source exactly; ParryContacts differs
  only by Unity's trailing space on blank m_EditorClassIdentifier, no semantic edit.
  Removed tooling-only Python cache was included in snapshot but not consumed by game.
- Actual1280x720 native Edge/ANGLE D3D11/Intel UHD: right16checks, left18checks,
  zero exceptions. Real Space preserves40HP, +30parry; real complete-word keyboard
  input triggers counter. Pause/resume freezes active samples, pool expires after
  final particle, R restart clears; fixture/provider traffic is local only.
  Left warms one complete real-input parry+word in the same session before selected
  capture (40score baseline), no forced game state. Excluded cold/pilot captures
  are retained separately and are not passing visual evidence.
- GPU read-only draw instrumentation:10checks PASS. Actor frame241 draw36964 has
  DepthMap uniforms, depthTest=true,LEQUAL515,depthWrite=true,FBO14/depthAttachment15.
  Pimen3x5 ray draw36973 uses the same buffer/test,depthWrite=false;336x48 core
  next frame likewise. DungeonSprite AlphaTest/ZWrite On writes its sprite-plane
  silhouette (no gl_FragDepth claim); actual near/far physicalZ provides occlusion.
  No camera/per-particle lights or runtime API/update changes.

## Complete frame and clip review

Scope selected before review: full captured idle/cue, enemy preparation, actual
Space contact, full core/tail, real-word counter, recovered clear idle. Captures
are variable-time, not claimed60FPS; every captured frame and adjacent pair is
retained, including held/repeated states. Runtime drift of wall-time capture is
not used to redefine the tested480ms independent effect clock.

| Capture | Segment from first PNG | Frames/pairs directly inspected | Scale |
|---|---|---|---|
| native-right |0-3965ms final hold |147/147,146/146 |320x180 full and4x logical crop |
| native-left |0-3294ms final hold |131/131,130/130 |320x180 full and4x nearest crop |
| final VP9 right |0-3966ms |148/148,147/147 |Every decoded full frame; original source zoom reviewed |
| final VP9 left |0-3295ms |132/132,131/131 |Every decoded full frame; original source zoom reviewed |

Each final clip adds only a1ms terminal held duplicate. Per-frame observations,
source/time mapping and transition notes: `native-{right,left}/review/frame-review.csv`,
`final-decoded-{right,left}/frame-review.csv`; sheets and scope.json alongside.
Right source PNG is1280x720; left dumps the game's320x180 logical raster from the
actual1280x720 canvas, exported4x nearest. Source asset remains pixel-exact;
JPEG100/4:4:4 intermediate and high-quality all-keyframe VP9 video are not lossless.
No source frame interpolation or omission. Final packet proof verifies all PNG
PTS (ms) and every original Opus audio payload, then decodes all148/132frames.
Realtime MediaRecorder video dropped critical frames, so it is excluded as final;
only its audio is copied. `export-local.py`, `verify-final-media.py`, export JSON,
`final-media-{right,left}.json` preserve the complete recipe/evidence outside repo.

Observed: white/yellow peak at sword crossing, orange thinning core, independent
near/far rays with differing apparent sizes, grounded descending/bouncing/fading
embers visible beyond480ms, full expiry by1.1s, no observed floor penetration or
abrupt end; player's body/feet/guard/counter remain readable. Existing counter slash
and normal-hit star are preserved and distinct from the deleted parry shock arc.
No atlas bleed observed; exact importer/crop RGBA,40logical-pixel scale/2.5WU cap,
material gain1 and mirror/timing checked in focused Unity tests.

Supplement `depth-native-right`: entire254frames/253pairs reviewed normal size;
F168-F21548frames/47pairs additionally viewed4x, including all38active VFX frames
F178-F215. Earlier missed enemy attack/hurt F49-F54 is explicitly retained in CSV;
this diagnostic is not the clean acceptance clip. No claim that unzoomed diagnostic
idle frames were reviewed at4x. Actual depth-buffer evidence above resolves depth
qualification independently of the sparse natural body crossings in this capture.

Final1280x720 `ParryPimen-right.webm` and `ParryPimen-left.webm` opened to end in
native Edge at1x and0.25x, unmuted/volume1; measured nonzero audio peaks right
0.496/0.499,left0.471/0.480 (`playback-{right,left}.json`). Subjective human listening
is not claimed. Browser presentation drops29/29right and26/20left during playback;
all encoded frames separately decoded and directly reviewed, so these are viewer
presentation limits, not omitted encoded frames or a product performance claim.

## Integration and retained evidence

Gameplay/systems/verification memory updated only for this parry change. No itch.io
upload. Task evidence root above is retained for review (source PNGs, clips, logs,
receipts and diagnostic/pilot distinctions); remove after user no longer needs QA.
Immutable Unity result archives remain on fly-server under
`/data/typinghero-game-loop/results/{20261024T111935-webgl,20261024T112344-webgl,20261024T114327-webgl}`
for reproducibility. No runner/build/browser task process remains intentionally
running. Task worktree/local branch are retained until verified remote integration;
remote task branch remains. Original dirty checkout preserved.


### Integration checkpoint: awaiting runner

Implemented commit34ef5ee; merged newer origin/main903344c in2d346ce. The sole
conflict was the VFX systems-memory row: retained the Pimen section plus upstream
Slayer class/bindings/verification. Upstream Slayer effects and production Brotli
settings remain untouched. All26task build-input delta paths are byte-identical to
34ef5ee (including intentional deletions), recorded in external
`integration-checkpoint.json`.

Additional integration ImpactVfxTests and strict builder execution is **UNVERIFIED**:
SSH10s timeout, Tailscale peer quanubutu100.115.189.84 offline (last seen
2026-10-24T11:54:12.1Z). This does not invalidate the pre-integration43cases,
normal/mirror builds and full native/frame/GPU evidence; it prevents final main
integration acceptance. No local Unity installation was found. No server/service
configuration was changed. Main push is withheld pending these checks.

Resume in the retained clean Orca worktree
`C:/Users/ADMIN/orca/workspaces/TypingHero/parry-pimen-depth`, branch
`micharel09/parry-pimen-depth`. Once the existing runner is online, execute the
scoped normalized-input adapter below (same repo runner, no runtime changes):

```powershell
python E:/CodexTemp/TypingHero/parry-pimen-depth/run-integration.py quick --test-filter ImpactVfxTests --wait-seconds 600
python E:/CodexTemp/TypingHero/parry-pimen-depth/run-integration.py serialize --wait-seconds 600
```

Then record actual results, fetch/integrate any newer main and recheck only affected
behavior, push verified task HEAD to origin/main fast-forward, verify task ancestry
on remote, remove clean task worktree/local branch via Orca. Remote task branch is
retained. Task Edge cache and temporary MJPEG exports removed after final clip packet/
decode verification; source PNGs, clips and evidence remain. No task browser/node/
Python process was found still running at cleanup.


## Per-parry variation follow-up (2026-10-25)

User clarified variation must happen between successive parries within one fight,
not merely between sessions. The existing incrementing parry seed is retained;
no session salt/random-source change. Implemented candidate adds two private
serialized profile settings: size reduction0..2logical pixels in whole-pixel steps
and initial rotation-3..3degrees. Only ParryImpact enables them; missing fields in
MetalClash/custom profiles default0, preserving authored particle rotation and size.
Sampling mixes impact seed/channel independently of Unity/gameplay random and the
physical PRNG. Mirror reverses the initial angle; cap2.5WU applies after sizing.
No new public API/dependency/update loop, no asset redrawing/reordering/retiming.
Core is still the original six80ms visible cells, total480ms, same tint/contact;
22physical particles retain all current group settings and random draw order.

### Review and available verification

- Source review: particles already randomize independent angles/speed/depth/length/
  lifetime on each increasing seed. The previously fixed core was the remaining
  visibly repeated component. No physical random expansion is required.
- Local actual sampling method compiled with existing .NET Framework csc /checked+;
  2050seed cases including negative/extreme int seeds pass repeatability,38/39/40
  discrete size choices and +/-3degree bounds; first24seeds have24distinct angles.
  Default settings return without changing size or authored rotation.
  Artifact `variation-local-check.json`, actual-source-method SHA and sample CSV at
  the existing evidence root. This is algorithm/static evidence, NOT Unity pass.
- Physical simulation/profile, parry seed producer and standalone MetalClash profile
  match the previous task commit byte-for-byte after newline normalization. The
  renderer's physical path is untouched. Gameplay Random is not sampled by new code.
- Focused Unity test added:24seeds, same-seed replay, mirrored angle/size,38-40range,
  2.5WU cap,22particles x67time samples exact position/velocity/alive against cloned
  variation-disabled profile, different seeded launches, pool reuse/default reset,
  gameplay Random preservation. Existing size assertions now cover the intentional
  range; pause/timeScale0 assertions additionally hold rotation/size.

### Still blocked / acceptance retained

Unity44scoped cases (43previous +1new), integrated strict builder, new QA build and
new runtime/frame review are **UNVERIFIED**. Targeted runner invocation again times
out connecting to100.115.189.84:22; Tailscale confirms quanubutu offline, last seen
2026-10-24T11:54:12.1Z. No Unity test executed for this candidate. Local dotnet has
no SDK; the existing framework compiler only qualifies the isolated sampling method.
Previous43Unity cases and278reviewed runtime frames refer to the unvaried build;
they must not be reported as verification of this changed renderer/profile.
New variation frames actually inspected:0. No claim of pixel stability at +/-3deg.
No authoritative gameplay/system-memory acceptance update until runtime verified.

Resume: run the existing normalized input adapter's targeted ImpactVfxTests and
strict serialize commands above; its helper reflects the integrated repo runner.
Then build QA from this candidate and record THREE CONSECUTIVE real Space parries
and real-word counters per direction in actual1280x720. Select complete clips from
idle/preparation through every contact/core/full physical tail/recovered idle before
review; no timing/frame sampling. Keep all held/repeated frames and timestamps.
Review every frame and adjacent pair at logical play size and integer nearest zoom;
record one observation/transition row per frame in this task evidence. Open final
clips with audio at1x/0.25x. Verify visible per-impact difference, stable contact and
480ms timing, no rotation raster jitter, no body/counter occlusion, correct depth,
visible post480ms embers, floor bounce/fade and pool/pause/restart. Fix any observed
artifact and re-review the affected complete variants. Main integration remains
withheld until those checks and the existing integration checks pass. No itch.io.


### Continuous multi-parry clip request

User explicitly requires multiple real parries to judge variation; single-impact
clips are insufficient and must not be resent as proof. Prepared external
`driver-multi/parry_clash.cjs` plus `native-multi.py` at the same evidence root:
three consecutive successful real Space parries and typed counters in one continuous
recording, full preparation/core/tail each, no restart/duplicated segments. Same
1280x720 native harness, fixture interception, PNG timestamps and original audio.
Launcher requires NEW variation player directories and a receipt for daa995f's
source/input hashes; it cannot silently fall back to the old unvaried player.
Node syntax/Python parse PASS only. No browser/game capture executed.
SSH timeout and Tailscale offline reconfirmed for quanubutu; no local Unity Hub
editor/WSL installation available. New multi-parry clip remains blocked on runner.
After verified candidate normal/mirror QA builds, create hash-backed
`variation-build-receipt.json` and `variation-snapshot-{right,left}.json`, place
verified players in `variation-player-{right,left}`, then execute
`python E:/CodexTemp/TypingHero/parry-pimen-depth/native-multi.py right` and `left`.
Review/export every recorded frame and pair using the complete-scope rules above.
