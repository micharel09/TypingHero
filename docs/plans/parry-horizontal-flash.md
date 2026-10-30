# Approved gold trial2 — 2026-10-30

User accepted the recommendation ("oke chốt theo bạn đi"): apply T2 to production
and use the repository's authorized commit/push/main integration workflow. This
supersedes the trial-only/no-commit restriction below, without claiming the
unverified >=80ms or original-speed playback gates have passed.

Reused exact reviewed `variants/T2.asset` and `T2Physics.asset`: production values
match after normalizing only asset names and the physics GUID. Production GUIDs,
prefab/scene bindings and all other serialized fields remain. No new VFX system,
runtime code, shader, art, trajectory/lifetime or camera change.
The existing four production tests with22/24-particle assumptions now cover30/32;
the seeded-trajectory comparison uses a count-matched clone of Metal physics so
it preserves the launch/position/velocity assertions despite intentional RNG
draw-count changes. Color/cooling/render-capacity and pause/eight-lease/reuse
tests adapt the already verified GoldSparkTrialTests fixture to the production
AssetDatabase profile; no QA Resource copies are shipped.

The retained T2 frame evidence remains the selected artwork evidence, with the
same limitations and no additional viewing credit. Promotion request/input hashes
are in `gold-spark-trials-20261029/promotion-selected-request.json`.
Scoped Ubuntu verification20261030T055639-quick passed8/8cases,0failed/skipped,
5.5193448s; source85117ed6221c54718aed1ea07b93d0e7adde911e92ad71366d1ab6d7ac696f9e,
2549inputs. All production-specific count/trajectory/palette/render/pool and
default/custom binding assertions pass. Integrated origin/main bad99db6; the only
conflict was verification.md's independent leading evidence sections, both kept.
Merged working source119bf23e66c538294e17c0010958eba3259d90896062aa9b4c76e5ee9d4687b6
(2895inputs) passed14/14scoped cases,0failed/skipped,24.0826462s. Repeated the
eight production cases plus public emission, two arena lifecycle paths, manual
cue/input, parry deadline and duplicate-success protection. All106identified
VFX/render/profile inputs remain byte-identical to selected-source85117ed6.
Production Player prefab still references profile GUIDdf72d97d1af744d19eda0382bfaa99e9;
physics remains GUIDa4335d1b5e37493fa5e7f5db154c6a46. No scene override replaces it.
Artwork review is reused for the same T2 render data; no fresh WebGL build or
new reference/motion-quality claim is needed for this identical promotion.
Commit85f5e050 is already on the task branch remote; final main push follows.

# Gold spark trials — 2026-10-29 (historical review-only handoff)

Current handoff: D/T1/T2 profile copies and comparison artifacts are ready for
user review. Recommend T2 from matched150–400ms stills and denser readable
gameplay fan in both directions, with no observed new white particle mass or
dark-orange blob. Production D and physics remain unchanged from task input;
no commit/push/merge/release. Readable-tail gain >=80ms and perceived playback
rhythm remain **unverified**, not silently passed. The later tail is still small
amber/floor flecks, not a demonstrated ReBlade-sized plume.

Review entry:
[clips, witnesses, PTS and ledgers](D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/gold-spark-trials-20261029/review.html).
All heavy work ran on Ubuntu. Build retry/source hashes, the scoped technical
results and real-key input checks passed as detailed below. No broader suite,
production build, full profile or unrelated host change was added.

Final normal cohort, all full-context/native frames and adjacent pairs directly
viewed with original CFR25 timing and all holds:

| Source | Selected frames | Viewed frames / pairs |
|---|---|---:|
|right-D-normal-fullswap1|560–692|133/132|
|left-D-normal-fullswap2|670–778|109/108|
|right-T1-normal-fullswap2|666–846|181/180|
|left-T1-normal-fullswap3|721–886|166/165|
|right-T2-normal-fullswap3|630–791|162/161|
|left-T2-normal-fullswap3|611–776|166/165|
|right-T2-lifecycle-fullswap1|576–1349|774/773|
|left-T2-lifecycle-fullswap2|455–1294|840/839|

Total2531frames/2523pairs. Each source has actual observations plus one CSV row
per frame in the existing evidence record. No actor-region black rectangle was
observed in these sources. T1-left763→764 jumps from initial star to recovery;
telemetry age.017→.402s and missing200–400ms runtime color samples confirm the
evidence gap. That pair remains unverified; static visibility acceptance does not
claim smooth motion. All sources retain their holds/gaps rather than trimming.

Only T2 proceeded to lifecycle:10input/lifecycle checks per direction passed.
Pause holds live particles at age.01667s; Resume continues the same effect,
fourth-parry Slayer red/zoom keeps detached gold visible around intact whole
silhouettes; Restart/Menu clear pool to0, with0exceptions/provider writes.
Slayer scope is the triggering parry and mode activation, not a typed Slayer
execution sequence. Source videos contain complete selected preparation through
cleanup/EOF; all1614lifecycle frames/1612pairs were directly viewed.

Six normal and two lifecycle clips have1× and4× exports. Comparison clips use
three native640×360 panels plus a40px label strip, aligned by visible score30
contact. Only exterior first/last-frame padding is added; internal source timing
is retained. Decoded frame hashes verify every exported source frame, each
comparison panel and exact4× repetition. Eight gameplay PTS witness panels were
also directly viewed. `timing-summary.json` reports source PTS/core/ray/gold
boundaries and uncertainties; unequal holds, weapon-smear masking and T1-left
phase loss prevent a causal >=80ms conclusion. Available review showed stills,
so perceived force/rhythm at original speed remains unverified.

Retain this pre-existing dirty worktree and the durable local/Ubuntu evidence
until the user chooses. No owned capture/build/export job remains running.
The notes below preserve the experiment's implementation basis and earlier
checkpoints; historical pending states are superseded by this handoff.

User requests D/T1/T2 comparison before selecting any production change. Reuse
this existing worktree/branch; HEAD62d26064, origin fetched at this continuation
and main currently bad99db6. Preserve the existing uncommitted D tint and all
other work. No integration, release, commit or push for these trials.

Implementation basis: `ParryImpact.asset`, `ParryImpactPhysics.asset`, the color
interpolation/streak shortening in `ImpactVfxPixel.cs`; retained
`contact-tint-20261029/gold-align-remote-body.py` (verified2547-input archive),
`adapters/ParryMirrorQA.cs` and isolated real-key `parry_clash.cjs`/`browser.cjs`.
Copies and adapters live in the existing durable evidence tree, subdirectory
`gold-spark-trials-20261029/`. Only isolated source archives receive review
Resources assets/QA fixtures; no production profile, prefab, shader or API edit.

T1: warm(1,.72,.16), cooled(1,.50,.08), leads cooling.63–.77s, secondary.56–.77s.
T2 adds leads2→3 and secondary6→9 per group:30physical+2flash=32 versus D/T1=24.
Hot, fragments/specks configuration, lifetime/trajectory/size ranges, core480ms,
ray/light220ms, camera/world320x180/presentation640x360 stay unchanged. Existing
40-state and64-particle near/far renderers cover this budget; eight leases total
256particles. Cooling is capped by each sampled effective lifetime. T2 changes
later RNG consumption, so identical seed means matched configuration/distribution,
not identical trajectories for every corresponding particle.
Warm/cooled are profile-wide, so unchanged fragment/speck group configuration
does not mean unchanged hue for those groups; no per-group palette is introduced.

Acceptance checklist fixed before capture: normal left/right D/T1/T2, complete
preparation/contact/recovery/fade; then only the better trial left/right through
Pause/Resume and Slayer, including restart/menu cleanup. Reuse prior ReBlade
C1–C4 source/phase review below (C4 >2s tail is not a universal parry target).
Measure core/ray/yellow visibility by original PTS; target ≥80ms additional
readable sparks and denser yellow during150–400ms, readable weapons/no white
mass/dark orange. Keep all held frames and inspect every selected frame and
transition. Playback rhythm remains unverified if only still images are available.

Technical scope: exact color/cooling/count/range invariants, rendered particle
colors/size through expiry in both directions,32-particle renderer capacity,
eight-lease bound, pause/expiry/reuse; reuse the existing cooling, snapshot,
overflow and standalone trace checks. No full suite/profile/stress/soak. Current
status: clones/adapters prepared; capture/PTS/frame review and recommendation
remain pending.

First technical result `20261029T093523-quick`, input `d474357c`:11cases,
8PASS/3FAIL/0skip. The three new color/cooling cases incorrectly applied the
1px cooled-ray assertion to the unchanged1–2px player specks (`d.small` explicitly
bypasses ray shrink in `ImpactVfxPixel.cs`). Correct only the review assertion
to exclude small specks; preserve the failed XML, no product fix. Existing seven
cases plus the new32-particle pause/eight-lease/expiry/reuse case passed. Rerun
only the three failed cases together with the GPU capture fixture, reusing the
eight passing results. An owned Docker client hung after its container became
Dead; exact name/state/parent verified before terminating only that client.

One QA build `20261029T094457-webgl`, input `b99f6bff`, is in progress. Final
combined capture request input `64d61b41` adds the corrected test and refreshes
authored SpriteDepthBinding after mirror/cel selection, following its existing
public Refresh method. Two earlier capture requests (`113f795b`, `587bfee5`)
were canceled only while verifiably waiting for runner.lock; no Unity result or
capture was produced from them. Final request remains queued behind the build.
ReBlade source hash equals the retained receipt; four six-frame contact details
were reopened here (24frames), while the prior239-frame reference review is
reused as recorded, not claimed as a new full reinspection.

Ubuntu failure: `20261029T094457-webgl` did not produce a player. Kernel evidence
at10:07–10:09UTC shows CONSTRAINT_NONE/global_oom, culminating in killed Unity
PID32758; Docker state OOMKilled=true/exit137/Dead. This is host-wide exhaustion,
not evidence of a profile or8GiB cgroup failure. All three active SSH clients
(build, queued capture, queued normal matrix) reset. No normal comparison ran.
Do not use this result as build/visual evidence. Preserve log/source; no local
build, host RAM/swap/service change or unrelated process termination. Memory
subsequently recovered (7.3GiB available observed). Retry the final64d61b41
combined GPU fixture first: `20261029T101255-quick`, executing now. Normal matrix
will be pointed only at a successfully preserved, hash-verified retry QA player.

Qualified GPU/static result `20261029T101255-quick`, source `64d61b41`, archive
SHA256 `2250844fb1e3caea90f840360f330e3b65820a12d4488a1dd1399706bfc253c2`:
4/4PASS/0skip,37.5897s, llvmpipe LLVM15.0.7. The corrected three color/cooling/
count cases and the capture case all pass; reuse the prior8PASS cases, do not
describe this as a single12-case passing run. Archive digest, all2561 input
hashes, exact four XML cases, fixture marker18ee50de and172PNG sizes/hashes
verified.86capture records=84matched samples+2T2/300ms repeat controls; both
post/world repeats equal the originals byte-for-byte.

Directly viewed all28 native1920x594 panels (three640x360 post originals and
three native320x180 world originals per panel):84samples/168originals, both
directions, every listed sample time. No extra visual credit for repeat controls.
Detailed transcription: `gold-spark-trials-20261029/20261029T101255-quick/`:
`fixed-observed.json`, `fixed-sample-review.csv`, `gold-trial-fixed-receipt.json`.
T2 provisional static preference: more and brighter amber points during150–400ms,
no additional broad white contact mass at those sampled times. T1 improves hue
without extra count. Both are still amber/orange rather than a ReBlade plume;
later particles are small floor embers. >=80ms additional readability remains
unverified; sparse frozen samples are not a PTS recording or a transition review.

Source limitation observed, not diagnosed: some post readbacks omit/darken
actor/lighting regions between independent time samples while world retains
the scene (notably right600/700/1100 and left300/350 column,700/900actors).
Do not turn those independent samples into a movie or interpret them as verified
temporal fade/profile-caused regression. Runtime clips must resolve readability.
Local viewer `gold-spark-trials-20261029/review.html` adapts the retained approval
viewer and exposes all14times/side, source profiles, tests and these limitations.

QA build retry `20261029T101824-webgl` uses exactly the same `b99f6bff` inputs;
currently compiling WebAssembly. No host/service/swap/local-build change.

Retry build finished successfully and preserved the immutable33-file QA player
under `20261029T101824-webgl/player`; both archive/2560-input digest and all33
payload hashes rechecked. Initial normal matrix:6runs x7checks PASS,0exceptions/
provider writes, actual320x180 world/640x360 output. Chrome152.0.7977.64,
ANGLE Intel HD4600 (Mesa/GLES3.2), Ubuntu native host path (not a2CPU cgroup).
Runtime mid-phase input G64–102 for D versus161–199 for T1/T2 and max24/24/32
particles confirm the selected profiles, independent of visual acceptance.

Initial `right-D-normal` source c8c7f34f: selectedF367–521,155/155frames and
154/154pairs directly viewed at full context and native400x240, including holds.
Reject whole source for black actor-region rectangles F481–486 after VFX expiry;
do not trim or export it as accepted. Manual row-per-frame ledger preserved.
Initial left D only full-contextF366–447 (82frames), native0; other initial
four sources unreviewed. All remain preserved/unqualified, not declared visual
failures or passes. See `initial-cohort-review-status.json` in trial evidence.

Read-only renderer inspection did not establish the cause. One external capture
diagnostic follows Chromium's documented `ui-disable-partial-swap` OpenGL switch:
https://chromium.googlesource.com/chromium/src/+/refs/heads/main/ui/base/ui_base_switches.h
No game/profile/shader edit or new build. `right-D-normal-fullswap1` source
b1a63f8d passes7checks and has133/133frames/132pairs inspected at context/native,
F560–692,22.400–27.680s+40ms. No black actor rectangle observed in this retry.
Its static continuity is accepted; original-speed perception remains unverified.
This is observed recovery, not a proven causal diagnosis. Five remaining sources
now capture with identical full-swap transport for a matched cohort. Initial
sources and their gaps remain separate; never mix their metrics into accepted
cohort results. Page-origin/video offsets are not calibrated; use direct visible
contact PTS, not the approximate telemetry emission times, for final durations.

# Gold alignment correction - 2026-10-29 (perceptual motion gate open)

## Flash lifetime comparison — 2026-10-29, review only

Scope: requested ReBlade segment 4.600 s through last decoded frame, all four
contacts; eight existing D contacts, normal / Pause-Resume / Slayer separated.
Reused retained source videos, manifests, numbered sheets/native crops and existing
per-frame records. No build, preset/API/gameplay edit, commit or push in this review.

Source identity: SHA-256 rechecked against retained receipts:
- [ReBlade source](E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/shorts-rp4vZPWLXsQ/source.mp4):
  `a9e1f284cdf9fff5a9286f12fc52b8df4111af16f45e141e6d4833795ffd77b6`.
  Published source: https://www.youtube.com/shorts/rp4vZPWLXsQ.
- D left: `bae46829ca0fe0e0ba50de275e171c496e986f592b4474e706bf0e92695d7d43`.
- D right exclusive retry: `b77cd3787841ce81653f3640fe1f2a212b4c51cc3141a6e343acbb5fcd8726e4`.
  The earlier rejected right recording is excluded.

Coverage selected before inspection: ReBlade F276–514, PTS4.600–8.566667,
239/239 frames and238/238 adjacent transitions; D left F476–1010,
PTS19.040–40.400,535/535 and534/534; D right F510–1051,
PTS20.400–42.040,542/542 and541/541. Total1316frames/1313pairs.
Preparation, contact, recovery, pause/resume, Slayer and subsequent menu/restart
states remain included; repeated frames were not removed.
ReBlade sheets23–42 and enlarged contact details; D focus000–033 per direction
were directly inspected. D crops preserve native400x240 within640x360;
faint particle disappearance outside those crops is not established.
[Per-frame phase/transition record](D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/20261029T073229-webgl/flash-lifetime-review-20261029.json)
supplements the existing detailed frame CSVs; it is a transcription after viewing,
not an image classifier or substitute for viewing.

Measurement: bright core means compact brightest contact center; ray means the
long contact streak, excluding weapon smears; fade means dimmer residual core;
tail means detached sparks. Phases overlap, so columns must not be summed.
Frame ranges below are inclusive, PTS are first/last visible frame in seconds.
Nominal duration includes the last frame's hold: (last-first+1)/fps.
Visual boundary uncertainty is at least one frame per endpoint: ReBlade16.7ms,
D40ms; conservative duration uncertainties33.3ms/80ms and cross-source
duration-difference uncertainty113.3ms. Ambiguous/occluded endpoints have larger
or unbounded uncertainty as noted. These are recording-time visibility estimates,
not engine lifetime measurements. Last ReBlade decoded PTS is8.566667;
container/audio duration is not used to extend its final visible hold.

| ReBlade contact | Bright core: frames; PTS; duration | Readable ray: frames; PTS; duration | Core fade | Spark tail / limit |
|---|---|---|---|---|
| C1 onset297 /4.950 |297–304;4.950–5.067;133ms|300–307;5.000–5.117;133ms|305–307;5.083–5.117;50ms|Spark-only308–312;5.133–5.200. Next contact313 overlaps; exact end unavailable.|
| C2 onset313 /5.217 |313–320;5.217–5.333;133ms|317–323;5.283–5.383;117ms|321–323;5.350–5.383;50ms|Spark-only324 onward;5.400 onward. Falling particles overlap C3; exact end unavailable.|
| C3 onset344 /5.733 |344–351;5.733–5.850;133ms|348–353;5.800–5.883;100ms|352–353;5.867–5.883;33ms|Clear sparks354–367;5.900–6.117. Flecks368–371 ambiguous; absent by372/6.200. Onset-to-tail endpoint roughly383–467ms.|
| C4 onset384 /6.400 |384–391;6.400–6.517;at least133ms before masking|388–393;6.467–6.550;100ms|From392/6.533 a dense bright plume masks isolated core fade; not measurable|Dense plume through419/6.983, then disperses; gold particles remain at514/8.567. Tail censored at EOF, at least2167ms from onset.|

C4 is visibly a different, much denser ending than C1–C3. It must not become a
single representative lifetime for all ReBlade parries. The visible slow/held
progression does not by itself identify ReBlade's internal hitstop mechanism.

| D contact / state | Bright core: frames; PTS; duration | Readable ray: frames; PTS; duration | Dim core / fade | Last visible spark estimate in crop |
|---|---|---|---|---|
| L1 normal, onset510/20.400 |511–514;20.440–20.560;160ms|511–514;20.440–20.560;160ms, faint at514|515–520;20.600–20.800;240ms|539/21.560, absent540; onset-to-next-boundary1200ms|
| L2 normal, onset598/23.920 |598–601;23.920–24.040;160ms|598–601;23.920–24.040;160ms|602–608;24.080–24.320;280ms|623/24.920, approximately1040ms from onset to next boundary|
| R1 normal, onset543/21.720 |544–546;21.760–21.840;120ms|544–545;21.760–21.800;80ms|547–556;21.880–22.240;400ms including held frames|569/22.760, absent570; approximately1080ms|
| R2 normal, onset632/25.280 |633–635;25.320–25.400;120ms|633–635;25.320–25.400;120ms|636–641;25.440–25.640;240ms|661/26.440, absent662; approximately1200ms|
| L3 Pause/Resume, onset686/27.440 |687/27.480 before overlay;730–732/29.200–29.280 after|687/27.480;730–731/29.200–29.240 after, last faint|733–739;29.320–29.560;280ms after clear resume|Approximately755/30.200; do not count overlay as active tail lifetime|
| R3 Pause/Resume, onset722/28.880 |723–724/28.920–28.960 before;774–776/30.960–31.040 after|723–724 before;774/30.960 after|777–784;31.080–31.360;320ms after clear resume|Approximately801/32.040; no combined active lifetime inferred|
| L4 Slayer, onset815/32.600 |816–823;32.640–32.920;320ms recorded|816–823;32.640–32.920;320ms recorded|Orange core824–837/32.960–33.480 under red layer;560ms visible, not isolated natural fade|839/33.560; absent840; approximately1000ms|
| R4 Slayer, onset856/34.240 |857–861;34.280–34.440;200ms recorded|857–861;34.280–34.440;200ms recorded|Orange core862–874/34.480–34.960 under red layer;520ms visible, not isolated natural fade|881/35.240; absent882; approximately1040ms|

Pause overlay: L688–729/27.520–29.160, first clear frame730/29.200;
R725–773/29.000–30.920, first clear774/30.960. These visible overlay intervals
(1.68s/1.96s to clear frame) are not exact engine pause durations. No subtraction
of wall-clock time is used to assert active VFX lifetime. Slayer red masking and
retained frames similarly prevent interpreting disappearance as an alpha curve.

| Normal-contact comparison, D minus ReBlade C1–C3 | Nominal difference | Interpretation |
|---|---|---|
| Bright core: D120–160ms versus133ms |−13 to+27ms|Within frame-resolution uncertainty; no consistent earlier D disappearance established.|
| Readable ray: D80–160ms versus100–133ms |−53 to+60ms|R1 is nominally shortest, but overlap/uncertainty prevents a general “D rays vanish sooner” claim.|
| Dim core: D240–400ms versus33–50ms |+190 to+367ms|D visibly retains a dim orange star longer. This is visibility of different artwork, not equal luminance or engine lifetime.|
| Spark tail |No valid pooled difference|C1/C2 overlap, C3 faint endpoint uncertain, C4 EOF-censored; D endpoints are crop-limited.|

Observed conclusion: D's strong center is not demonstrably shorter in these
normal contacts. D's dim core remains after the long ray is gone. ReBlade C4
maintains a much brighter, denser gold plume and a tail visible for at least2.167s;
D's small red/orange tail is much less prominent even while still visible.
Compared specifically with D Slayer crop endpoints1.00–1.04s, ReBlade C4's
recorded visible tail extends at least about1.13s longer, but states/crops differ,
so this is not a matched engine-lifetime comparison.
Hypothesis only: particle brightness, density and coverage can explain a feeling
of faster disappearance in D; this review does not isolate their causal effects.
Serialized core480ms, ray/light220ms remain unchanged and do not prove equally
long perceptual visibility.

Frame witnesses:
- [ReBlade core-to-spark C1, F305–310](E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/shorts-rp4vZPWLXsQ/detail-gold-1-305.jpg);
  [C2 F319–324](E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/shorts-rp4vZPWLXsQ/detail-gold-2-319.jpg);
  [C3 F352–357](E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/shorts-rp4vZPWLXsQ/detail-gold-3-352.jpg);
  [C4 F392–397](E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/shorts-rp4vZPWLXsQ/detail-gold-4-392.jpg).
- [D L1 F508–523](D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/20261029T073229-webgl/review-D-left/focus/focus-002.png);
  [D R1 F542–557](D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/20261029T073229-webgl/review-D-right-exclusive/focus/focus-002.png).
- [D L4 F812–827](D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/20261029T073229-webgl/review-D-left/focus/focus-021.png);
  [D R4 F846–861](D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/20261029T073229-webgl/review-D-right-exclusive/focus/focus-021.png).

Status: static phase measurement and full selected-frame inspection complete.
Original-speed perceptual rhythm remains **unverified**: available viewing here
returned still images. No claim of playback acceptance, reference match, or
reason to automatically increase lifetime. Preserve preset D pending user judgment.

Current: D `(1,.90,.35,.80)` passes static production captures and six focused
technical cases. One immutable WebGL build; final left/right-retry real-key
checks pass. All selected1077 frames/1075 within-source transitions directly
reviewed in full-context sheets and native crops. Perceived original-speed
motion remains unverified with available still-image/telemetry tools, so the
single-field preset change is uncommitted/unpushed. No main integration/release.

User rejected A `(1,.78,.28,.80)` because the core surroundings read orange
against the brighter yellow ray/sparks. A is a rejected comparison, not an
accepted color. The older selection language below is historical and superseded.
Frozen white comparison remains `4b5d4df3`; continuation starts at clean task
HEAD `62d2606491512ccc769c473705ef6811fd5e4d13`. Remote verified as
`https://github.com/micharel09/TypingHero.git`; origin fetched, no main integration.

Acceptance: small yellow-white brightest center; surrounding yellow in the same
family as existing ray/sparks; both contact weapons clearer than white; no wide
white, green-yellow, weak/dark result. Preserve distant rays, receiver and fade.
Only production edit allowed is channel0 tint. Trial order D `(1,.90,.35,.80)`,
E `(1,.96,.42,.80)` only if D still orange, F same accepted RGB/alpha .65 only
if white/occlusion remains too wide. Maximum three new tints, no B/C retry.

Reuse: external `contact-tint-20261029/ParryHdrQA-v2.cs`, hash-named adapter
`runner-captures-v2.py`, qualified white/A captures from `20261029T035847-quick`,
six-case `runner-tint-final.py`, native keyboard driver and exclusive runner.
Adapt the fixture on profile clones with per-variant expected tint; preserve
particle count/startColor/actual age/light/background assertions and separate
15ms repeat. New viewer adapts `approval-2263fb89` HTML/CSS; both old viewers
remain unchanged. Production stays A until direct static comparison passes.

Owner/consumer: this parry task. Windows worktree
`D:/Projects/TypingHero-worktrees/parry-cinematic-flash`; durable evidence root
`D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/`.
Ubuntu `fly-server` runner uses `/data/typinghero-game-loop/results/<run-id>`
under the existing `runner.lock`, source archive and result lifecycle. SSH/SCP
transfers between these exact roots. Unity/build/image/video processing stays
on Ubuntu. New result IDs and hashes are recorded here as produced. Task owner
cleans only owned results/artifacts after user visual approval/task closure;
retain worktree/evidence for review. No new temp/log system or local heavy jobs.

Commit/push gate: static images, six technical cases, final real-key WebGL QA,
all selected frames/transitions and original-timing perceptual playback review.
Playback ended/callback/drop receipts alone do not meet motion acceptance.
No main merge or itch.io release is authorized in this continuation.

## D capture execution

First local snapshot attempt failed before SSH/Unity with adapter NameError
`time` in the injected timestamp expression; no remote result or color evidence.
Corrected it and moved archive assembly to Ubuntu by adapting the existing
snapshot/server pipeline. Base archive `20261029T043751-quick`, source `eeccbfae`
was fully rehashed; all2547 input SHA256 values match the current worktree using
the existing YAML/meta-only LF normalization. No source inputs were inferred
from a clean status alone. This avoids repeated local source compression.

Candidate-D result `20261029T065856-quick`, source
`b1a0163d80dbe4a7b0ee2c06ff6b546359180129ff1279c6c5a4d348e0869ac0`,
contains the2547 verified inputs plus fixture `ParryContactTintQA_ed9ce59a95b0.cs`.
Template SHA256 `ed9ce59a95b0fc5fefd86fc945b5e5171589122f9e76daa98dd207efa2d64ff9`.
Fixture uses actual nonzero injection timestamp, per-variant tint expectations,
white/A/D clones and a separate D15ms repeat; no production asset mutation.
Exact filter selects only `ParryContactTintCaptureQA_ed9ce59a95b0.CaptureMatchedPostprocessedPairs`.
Qualified result: 1/1 PASS,0skip; exact XML test/marker, source/archive digest,
16 capture records and32 original PNGs checked together. White/A controls match
all20 corresponding prior PNGs byte-for-byte; D15ms repeat matches both outputs.
`20261029T065856-quick/gold-align-capture-receipt.json` contains exact hashes.
All10 D originals,20 white/A control originals viewed in separate native post
640x360/world320x180 sheets, five nearest2x contact comparisons, and all five
full-post D images at1280x720. Two repeated images are technical stability
evidence, not additional visual samples. Old A WebGL contact `right-620.png`
was opened at native640x360 to compare its bright gold ray/sparks.

| Time | D center and hue | Weapons / ray-sparks / receiver / static sequence |
|---|---|---|
|15ms|Small yellow-white contact, yellow shoulder; no separate orange mass|Defense blade and opposing horizontal blade distinguishable; bright diagonal ray remains dominant at range; receiver unchanged|
|30ms|Bright point retained; gold stays close to the ray family|Left detached fragment warms independently; weapon edges remain visible; no receiver/background change|
|60ms|Compact white-yellow point, warm yellow surroundings|Detached debris is orange in all three controls; not a core tint defect. Ray keeps its taper while weakening; no pose/framing jump|
|100ms|D star reads yellow/gold around the small white point; A has the stronger separate orange surround|Both weapon approaches separate better than white's larger pale area. Warm outer texture tips remain, without a broad orange field; far ray and lit actors agree with controls|
|140ms|Yellow star persists as the diagonal ray fades; no green cast or dark/weak collapse|Brightest point remains at contact; no new broad white patch; detached orange particles and receiver remain the original contributions|

D `(1,.90,.35,.80)` selected for production on this static evidence. E/F were
not tried: no remaining broad orange field or excessive white/occlusion was
observed. These five ordered stills do not establish perceived motion rhythm.
Production edit now changes exactly channel0 tint, retaining every other byte
of the normalized asset. Production four-variant capture queued, source
`b3c9a2bc7bdd1a5a3700ae6f53973eef8e16874d319a2bfeb364a7e943134efb`;
40-image qualification/review and all later gates remain pending.

## Production D and focused technical qualification

Production `20261029T071310-quick` / `b3c9a2bc` qualified 1/1 PASS,0skip,
11.367153s; fixture template `cc1a266761b52639381b664fa2e001479ecdaa29615ff75fb058db34bc75a0e8`,
20 records/40 original PNGs, correct marker/filter and rehashed archive snapshot.
All10 full-production PNGs equal the qualified D candidate bytes. All10 core-off
post/world PNGs equal the frozen white core-off bytes. PNG hashes match local
SCP copies. Details: result `gold-align-capture-receipt.json`.

Directly viewed all40 originals via five post native640x360 x four-variant
sheets and five separate world320x180 x four-variant sheets. Additionally opened
all20 post images individually at nearest1280x720. Full/core-off/ray-off/light-off
retain the intended attribution at all five times: core-off removes the100/140ms
gold star, ray-off removes only the diagonal presentation ray, light-off dims
the existing weapon/actor illumination without turning the core orange or dark.
Full and ray-off world textures remain byte-identical (no world ray duplicate).
No new wide white region/green cast or receiver/background discontinuity observed.
This accepts static hue/readability only, not perceived-speed motion.

Technical `20261029T071427-quick`, source
`937b769656ed5973a7c257f4e6ff0f865285671d85c96858d99e71e5b02dbaf6`:
6/6 PASS,0skip using exactly the prior four-name filter/six cases. No tests added
or edited. Source-backed assertions read current authored tint, check actual
startColor, live snapshot/Pause/Resume/clear/disable/pool reuse, repeat builder
profile bytes and the frozen standalone MetalClash trace. Of13 affected inputs,
12 match A; only normalized preset changes to
`f73437d3677aff854c1cafc89505c43480d5873a375a37df232ccedf83ccb15e`.
`gold-align-reused-input-hashes.json` records the comparison. All other2546 source
inputs match the verified base; unchanged shader/HDR/viewport evidence reused.

One final Ubuntu WebGL build is now requested from frozen production D inputs
plus the unchanged, URL-gated mirror fixture. No additional resolution/profile,
main integration or release. Real-key QA, frame/transition/perceptual playback
review, final viewer and commit/push gates remain open.

## Build identity correction

Final requested build source is
`6e2ac7cc0bc355ee8dad3a115eebfff8ce86e2e9837aed3910a583f3f2e02d0f`.
Its verified result directory is `20261029T073229-webgl`. Earlier live progress
was incorrectly attributed from `20261029T071532-webgl`; that directory's marker
is `f0cfb043b1593db67f7a4cb9941ec3d970e41af0df16ea1c22fa99057bed4c7f`
and belongs to another queued job. Its build progress/success is NOT task evidence.
This task requested one build; it waited behind that job and is now executing.

Two task-created helpers were mistakenly copied into that other result directory.
Their exact SHA256 values were compared with the local originals before unlinking
only those two files; no original/owner/player/log data was changed or removed.
Receipt: `gold-align-misattributed-run-cleanup.json`. Both helpers now point to
the source-verified073229 result. Future evidence uses snapshot/fixture/payload
identity, not directory recency. Qualified D/production captures and the six-case
logic source937b7696 remain correctly identified; the logic marker was rechecked.
Build completion, real-key QA and motion acceptance remain pending.

## Final D player and motion selection (recorded before export/review)

Build `20261029T073229-webgl` completed and preserved its immutable player, QA
marker true. Rehashed all2548 source entries to snapshot6e2ac7cc; archive SHA256
`b95899f45742e8142da52f8e5e7ba18df6ab25834e5f0872f115e765c10add07`.
Serialized preset matches the qualified normalized D asset f73437d3. Both hardware
runs match all33 immutable player file hashes. `gold-align-runtime-audit.json`
records the full source/payload/renderer/input/source-video identities.

Chrome152.0.7977.64, ANGLE Intel HD4600/OpenGL ES3.2, source videos640x360;
world320x180 checked by the driver. Right/left each12/12 technical checks PASS,
0exceptions/provider writes, maximum one presentation quad. Actual signed angles
right `[20,45,-15,65]`, left `[-20,-45,15,-65]`; all8 combinations, no seed supplement.
Four real-key parries each direction, fourth into Slayer, third live Pause/Resume,
pool reuse and clean Restart/Menu covered. Browser/parry drivers match the accepted
A hashes508e1e3e/8f526f20. Only the existing exclusive wrapper's job list is adapted
to run both directions serially under one lock. Technical pass is not motion acceptance.

Selected intervals are recorded now, before exporting or inspecting source frames:

| Source | SHA256 | Interval [start,end) / whole source count |
|---|---|---|
|right-tintD-6e2ac7cc/page@d292d6959841e3c02d8bc905ce918697.webm|2b5464474c6c2fec7bdee61f5c33b82e8da6b211bc1a6b59b7ef1cc2ff21decd|18.790580019533635-42.200000s /1055 frames|
|left-tintD-6e2ac7cc/page@6a8963bd50ba0b6df64b941862023f40.webm|bae46829ca0fe0e0ba50de275e171c496e986f592b4474e706bf0e92695d7d43|19.065710039049385-40.440000s /1011 frames|

Reason: one second before the first post-Restart observer through the entire
remaining source, including complete preparation, contact, fade/recovery, all four
parries/Slayer, Pause/Resume and clean Restart/Menu. No chronology errors are excluded.
Include a preceding held frame if its duration crosses the start boundary, plus
the available immediate outside boundary frames. Use actual manifest PTS, not index/FPS.
Whole-source export uses unchanged HD-2D helper39fef34b and existing Pillow deps
on Ubuntu under runner.lock. Native focus crops are additional spatial evidence,
not new timing or replacement frames. Original source order/repeats are retained.

Source scope: existing parry flipbook tint only; fixed white/A images and yellow
ray/sparks in the same scene are the color reference. No new ReBlade reference-video
review or character animation authoring is claimed. Applying sprite-frame-review to
the selected output frames/transitions; original-timing perceptual playback remains
unverified because the available tool input is still images/browser telemetry.
No commit/push until that mandatory motion gate is actually met.

## Initial right source rejected; same-player retry

Whole-source export qualified2066/2066 frames with exact PTS. Original selections
resolve to rightF469-1054 (586/585), leftF476-1010 (535/534), with held-start frames
included and immediate outside boundaries468/475. These export counts are not
visual acceptance.

Initial right2b546447 was reviewed at full-context320px/source frame through
F879: selectedF469-879 =411frames/410pairs, plusF464-468 outside context. Native
sourceF841,842,843,844,845,848,849 were opened to resolve the Slayer transition.
Actor visibility changes lit clash -> absent/black actors(F842-843) -> prior lit
clash(F844-845) -> absent/black(F848) -> opaque red silhouettes(F849). The existing
SlayerScreenFX entry weight is monotonic; telemetry has no additional emission and
quad/burst expiry progresses without a new ray. Recorder origin is suspected but
not independently proven; this recorded visibility/chronology inconsistency is
insufficient for motion acceptance. Reject the entire initial right source rather
than trimming the affected frames. Its12 technical passes remain separate.

`right-rejected-observed-ranges.json` preserves actual observations/native views;
F880-1054 remain unreviewed and excluded. Source video, all exported frames, sheets,
telemetry and initial `gold-align-runtime-audit-initial.json` are retained intact.
No normal/slow accepted clip exported from this source. No tint/gameplay change.

Adapted the existing exclusive retry wrapper only to right orientation, same
immutable073229 player and unchanged drivers. Requested label
`right-exclusive-tintD-6e2ac7cc`; waits behind an existing Unity job under the shared
runner lock. Host IO at request: some avg10=6.68, full avg10=2.41 (60s averages
still higher). No other job stopped and no additional build requested. This is
the first rejected D motion source; bounded retry/three-recurrence rule retained.
Left original source remains awaiting direct review, not accepted by technical pass.
External adapters and input manifest stay in the existing contact-tint evidence
directory. Ubuntu result owner and cleanup conditions are those above.

## Left source frame/transition review

Left source bae46829: whole1011/1011 exported; selectedF476-1010 inclusive,
19.04-40.40s with final40ms duration to40.44s. Directly inspected all535 selected
frames/534 consecutive transitions in full-context sheets29-63 (320px/source),
then focus000-033 at native400x240 crop. Full-context boundaryF464-475 adds12
outside frames, not selected-frame credit. Every sheet boundary and held frame
retained. Manual `left-observed-ranges.json` records actual ranges and observations;
`left-frame-review-6e2ac7cc.csv` expands those observed phases to one row per frame,
actual PTS/duration, next transition and evidence. Export counters stay separate.

First contactF510-514, secondF598-601: compact yellow-white point and yellow
star shoulder next to the narrow gold ray. Later broad horizontal white pixels
are the pre-existing weapon cel; small orange texture tips/detached debris remain.
PauseF688-729 holds the live clash behind the overlay. ResumeF730-731 continues
the gold star, then weapon smear/fadeF732-742 without a new ray. FourthF815-823
shows the steep mirrored ray and yellow star; red silhouettes fromF824 occlude
late hue. Unlike the rejected right source, no return to a prior lit clash is
observed throughF877. RestartF893-923 and MenuF954-1010 show no old core/ray.
The final frame is source EOF, not a loop boundary.

This establishes frame continuity/visible color only. Original-timing perceptual
playback remains unverified: enabled tools provide still images and telemetry,
not continuous video input. No commit/push. Adapt existing A exporter to this
exact reviewed interval (535 normal frames,2140 frames at4x repeated-frame slow
motion), compare every decoded hash and duration. Export waits behind the same
runner lock; no additional build or local image/video processing.

## Right retry source selection (before export/review)

Exclusive same-player retry completed12/12 technical checks,0exceptions/provider
writes, maximum one presentation quad, actual angles `[20,45,-15,65]`, all33
payload hashes match073229 player. No new build. Source
`right-exclusive-tintD-6e2ac7cc/page@e974f9d800197982270ec83b9c362e78.webm`,
SHA256 `b77cd3787841ce81653f3640fe1f2a212b4c51cc3141a6e343acbb5fcd8726e4`,
1052 frames,640x360,25fps,timebase1/1000,duration42.08s. Selected interval
`[20.407860000014306,42.08)` uses the same reason: one second before first
post-Restart observer through EOF, all preparation/four contacts/fade/recovery,
Slayer, Pause/Resume, Restart/Menu. Include any held frame crossing the start
and immediate outside boundary. Source is not yet visually accepted.

`gold-align-runtime-audit-retry.json` rehashes the immutable source/archive/payload
and identifies this retry separately; the initial audit/rejected source remain.
Export the entire new source using unchanged39fef34b helper under runner.lock to
`review-D-right-exclusive`, never overwrite initial right or left review files.
Record actual selected PTS/frame ranges after export, before direct review.

## Right retry frame review and clip export

Whole1052/1052 source frames exported with exact PTS. Selection includes held
F510 at20.40s throughF1051 at42.04s plus its40ms duration to42.08s:542frames,
541pairs. Directly inspected sheets31-65 at320px/source, then focus000-033 at
native400x240; all selected frames, held repeats and every adjacent transition
including sheet boundaries. Full-contextF496-509 adds14 outside frames, including
immediate boundaryF509. Source EOF is non-looping. Actual ranges/phase corrections
from native review are in `right-exclusive-observed-ranges.json`; one row per
frame in `right-exclusive-frame-review-6e2ac7cc.csv`.

FirstF543-545 and secondF632-635 have a small pale contact and narrow yellow ray.
Large white horizontal shapeF546 is the existing Attack1_5 weapon cel, not a tint
field. Warm original star tips/debris remain during fade. Third PauseF725-767
holds the live clash; actors resume beneath closing overlayF768-773, starF775-776
continues then fades without a second ray. FourthF858-861 stays yellow around
the pale point. F862-863 darkens actors onto red, F864-874 holds late silhouettes,
F875 onward recovers without returning to an earlier lit clash. Late color is
occluded by Slayer red. Fresh RestartF928-969 and MenuF993-1051 contain no old
core/ray. This retry resolves the initial source visibility inconsistency for
frame continuity; it does not prove perceived original-speed rhythm.

Left export qualified535 normal/2140 slow frames,21.40s/85.60s; all decoded frame
hashes match source and exactly4 repeats for slow. Normal SHA256 e1c34d14,
slow bf255e0e. Four selected left contactsF513/600/731/818 opened at native640x360
and full nearest1280x720. Right export uses the fully reviewed542-frame interval,
expected2168 slow frames,21.68s/86.72s, contactsF545/635/775/858. Both hosts retain
original source/logs, rejected right source and machine export manifests separately
from actual review records. Clip export receipt is technical only; perceived
motion gate remains open and blocks commit/push.

Right clip exporter initially waited under runner.lock behind an unrelated Unity
job. Its own waiting PID4083740 was checked against exact script path/right argument
before SIGTERM; no other job was interrupted. Existing A `export-final-clips.py`
contains independent FFmpeg export. Adapted D back to that mechanism for media
encoding only, Ubuntu `ionice -c3 nice -n19`, two FFmpeg threads. All Unity, browser
QA and whole-source frame export already completed under the existing lock.
This changes scheduling only, preserving source/interval/codec/normal timing,
4x repeated-frame slow filter and every decoded-frame/duration assertion. No
additional Unity/browser run or build. This supersedes the earlier clip-lock
choice; the original queued attempt produced no right clips.

The independent export PID4108624 was observed in `D/__wait_on_buffer` before
FFmpeg began. After re-verifying exact script/right arguments and nice19, changed
only its I/O class from idle to lowest best-effort7 (`ionice -c2 -n7 -p4108624`)
to permit progress during sustained host I/O. Nice19/two FFmpeg threads remain;
no other process priority was changed. Source/filters/hash checks remain identical.

## Final artifact status

Right export completed:542 normal/2168 slow frames,21.68s/86.72s, all decoded
hashes equal the reviewed source subset and exactly4 repeats per slow frame.
Normal SHA256 `665164a0dd43bf58ff67f85b1a42cd52874d5b2ff47c44c1479241e378b34581`;
slow `9a8031e1a9d440f24011954c7807df9bfd7c819fee99be05aa486eb01397e2e7`.
All four local clips match qualified Ubuntu hashes. All8 final contact stills
opened native640x360 and nearest1280x720. No new color candidate or game change.

New viewer is `approval-6e2ac7cc/review-6e2ac7cc.html` in the durable evidence
root, label "Chờ người dùng duyệt hình". It adapts A HTML/CSS, adds white/A-rejected/D
columns for all5 times and4 layer variants, separate world textures, four real/slow
clips,8 contacts/2x links and preset/capture/XML/source/frame-ledger evidence.
Windows encoding default initially garbled metadata text; explicit UTF-8 read
corrected it and DOM U+1EA2 check passed. Prior75+157 viewer hashes remain unchanged.
Final link/video-dimension/host-transfer receipts accompany the new viewer.

Acceptance remains open ONLY for perceptual original-timing motion review. Tools
expose still images/browser telemetry; frame inspection, decoded hashes, video
metadata and any playback-ended receipt cannot establish perceived flash rhythm,
occlusion duration or fade/Resume feel. Do not count user visual approval as agent
QA. No additional runtime test/build is needed while these source/config inputs
stay unchanged. No commit/push until actual perceptual review can be completed.
Local HEAD and remote task branch remain62d2606491512ccc769c473705ef6811fd5e4d13;
the one-field D edit and scoped docs remain uncommitted. Gameplay memory is not
promoted to a verified D preset. Keep task worktree, immutable Ubuntu073229 player,
sources/rejected captures and both-host evidence for that continuation/user review;
cleanup only after user approval and task closure. No main merge/itch.io release.

Viewer final checks:294 local links,0missing;216 PNG headers have the expected
post/world/nearest2x dimensions. Browser loads all128 displayed images and four
640x360 videos at21.68/86.72/21.40/85.60s without media errors; UTF-8 status and
single-viewport horizontal fit pass. DOM receipt is technical, not playback review.
Manifest255 files SHA256 `dd9b5fd71d7207513bd6404dbdb5f616e0e8db1cba08cd4537f7d05da10c8e34`
matches both hosts. Detailed receipts: `gold-align-viewer-check.json` and
`gold-align-viewer-transfer.json` in local073229 evidence result.

Two earlier viewer-audit attempts were rejected: a non-ASCII assertion literal
was changed by PowerShell's stdin encoding, then the checker assumed2x-world PNG
was1280x720 instead of its correct640x360. Corrected checker expectations/UTF-8
handling; no PNG/texture/asset or clip changes were made for those failures.
Final reusable audit helper and its SHA are preserved with the task adapters.

---

# Historical A candidate record (color rejected; playback perception unverified)

## Final WebGL and review checkpoint - 2026-10-29

Selected first passing candidate A: channel0 tint `(1,.78,.28,.80)`; B/C not
tried. Only that production field changed relative to frozen color baseline
`4b5d4df3`. The gradient/alpha keys, size40, lifetime.48, seven cels/frame curve,
ray128x6/core1.25/gain2/halo.35, light7/range3.5/background minimum.90 remain.
No shader, material, PNG, binding, GUID, gameplay, API or schema change.

One final Ubuntu WebGL build `20261029T044701-webgl` completed successfully,
snapshot `2263fb8951382d8cee0642ce941f785e7b196813b190d15d92b794b0cb042dd8`.
Its log records repeat-builder serialized objects/references/listeners unchanged.
All2548 archived source entries rehashed to the snapshot; both accepted hardware
keyboard runs match all33 immutable player files. Payload manifest SHA256
`36946fab9168ee6708395c964d6c3ef06ffdddf29938a5d11ce285d310d29a81`.
Archive SHA and complete source hashes are in `source-artifact-audit.json`.
Local preset/test bytes match archived inputs using the adapter's actual YAML-only
LF normalization; C# bytes remain unchanged. Preset normalized hash
`fdb203380eb7841f3a71a4fd857617c683cdbaf2dc629cccfa9257b8ed279934`;
test hash `7d07a23861002f4f6b4cc05817c2270821b9c3743476836a272e02bf8cf62130`.

Chrome152 / ANGLE Intel HD4600, native640x360: right-tintA-2263fb89 and
left-exclusive-tintA-2263fb89 each12checks PASS (24accepted checks total), zero
exceptions/provider writes, maximum one presentation quad. Actual signed angles
right `[20,45,-15,65]`, left `[-20,-45,15,-65]`: all8combinations, no supplemental
seed needed. Ordinary parry, fourth parry into Slayer, live Pause/Resume/fade,
pool reuse and clean Restart/Menu covered. Initial left-tintA video retained but
REJECTED for chronology: F810 gameplay then811-812 older Pause; F899-900score120
then901-902score90/guard. Its12technical passes are separate from accepted motion.
The retry reused the same player under the existing exclusive runner lock; no
second build, shared cache deletion or other-job interruption.

Selections were recorded before exporting/viewing:

| Accepted source | SHA256 | Selected time / frames | Direct review |
|---|---|---|---|
|Right|8fa826f0555bbd1d733ac23a83d49a7933b64377132a4bb31aa0023d66be26a7|20.000-46.800s; F500-1169|670frames/669pairs|
|Left exclusive retry|26a56cca41cb5b1272b07ded940e1006d908cd049bdad494483734f2f2282d69|17.000-41.840s; F425-1045|621frames/620pairs|

Entire source videos exported on Ubuntu with original PTS/held frames: right1170,
retry1046. Directly viewed every selected frame and consecutive transition via
ordered full-frame sheets: **1291frames/1289pairs**, plus13boundary frames outside
the selections. Per-frame observations/transitions are in right-review-2263fb89.csv
and left-exclusive-review-2263fb89.csv. Rejected initial-left review is only
F500-943,444frames/443pairs; later frames remain unreviewed and excluded.
All8accepted contact stills viewed at native640x360 and nearest1280x720.
No chronology regression observed in the accepted retry. Right third contact
Pause/Resume and retry F684-750held/F751resume fade advance correctly. Fourth
contact enters the existing opaque red Slayer presentation, which occludes late
core; do not infer hidden color. White Attack1_5 sword smear is present in the
unchanged Attack1.png sprite, independently of channel0, and is retained.

Normal clips preserve670/621source frames, durations26.80/24.84s; slow clips
contain2680/2484frames, durations107.20/99.36s, exactly four copies per frame.
Every decoded YUV420 frame hash matches the corresponding source frame; no
interpolation, dropped held frames or re-timing. An initial slow export padded
three surplus tail frames; rejected and replaced after exact-count/hash checks.
Orca original-speed playback reached the end of both normal clips at rate1,
26.879/24.943s wall time, zero dropped frames reported. Browser callback counts
are not direct-inspection counts. Playback receipts establish technical playback;
continuous perceptual/artistic approval remains with the user. No FPS claim.
Both slow previews also reached the end:107.480/99.522s wall time,
2680/2484decoded frames and zero reported drops; `playback-all.json` retains all
four playback receipts. Reloaded viewer reports640x360for every video and the
correct durations; Vietnamese approval label verified in the browser screenshot.

Viewer adapted from the preserved0ee2a52f HTML/CSS:
`D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/approval-2263fb89/review-2263fb89.html`.
Includes normal/slow clips, native/2x contacts, all80baseline/final originals with
five times/four variants/post and correctly labelled world320x180, preset and
technical/review receipts. Label: **Chờ người dùng duyệt hình**. Old viewer and
baseline profile remain untouched. Local links and clip hashes checked.

Evidence owner: this parry task. Existing evidence root and worktree retained for
user visual approval; transfer by SSH/SCP to/from fly-server. Remote build/captures
remain in `/data/typinghero-game-loop/results/` under the result IDs above/below.
Task-only Pillow13.0.0 was installed under final-result `review-tools/deps` to run
the unchanged HD-2D video contact-sheet helper; installer/source receipt retained,
no system/game dependency change. Cleanup only after visual approval/task closure,
only these task-owned artifacts and clean worktree once results are preserved.
No main merge, itch.io publication or user visual approval is implied.

## Production A and technical checks - 2026-10-29

Production capture `20261029T042724-quick`, source
`6cc969106a8c6a0e4b8e95ed8987478dfbe8e4ab3c5543e55c0ecb387421ac49`:
**1/1 PASS, 0 skip,14.0788542s**; correct production marker/hash,
20records/40originalPNG at five times/four variants/two outputs. All10full-final
post/world PNGs match the qualified candidate-A bytes exactly.
`production-A/production-A-receipt.json` records these checks. All40final images
directly viewed at native size via separate post/world sheets; all20post contact
crops additionally viewed nearest2x (same x150,y110,180x140 rectangle). Final
80baseline/production images complete;10candidate plus2repeat kept separately.

Focused technical `20261029T043751-quick`, input
`eeccbfae70ae312fc41f1a2829c316ea476f855b031f1e5877cdf6a7ac70e896`:
**6/6 PASS,0skip,4.9382462s**: changed ParryImpactPauseResumeReturnsLease,
three GenericPrefabUsesArbitraryBindingsFlashCountsAndSnapshots cases,
MissingParryPrefabBuilderCreatesSixBindingsAndPreservesCustomOnRepeat, and
MetalClashMatchesFrozenPreRefactorTraceAtEveryStep. Actual profile tint reaches
startColor; living burst retains tint during profile edits/pause/resume; pool
return/clear/disable/new emission select current tint. Repeated builder preserves
authored tint and profile bytes. No builder/runtime code change. `reused-input-hashes.json`
compares the previous13input hashes:11identical, only preset/test differ. Prior
unchanged shader/HDR/allocation/viewport evidence reused, NOT new color acceptance.

Directly re-opened saved ReBlade frames301(5.016667s),319(5.316667s),350(5.833333s),
390(6.500s) from approval-0ee2a52f. Bright center/gold surroundings observed;
this is four saved stills, NOT a fresh whole-video/segment review. Original viewer
unchanged. New output review is recorded in the final checkpoint above.

Final WebGL completion, hardware keyboard coverage and selected frame/playback
review are recorded above. Game memory core-tint description updated to verified A;
user visual approval remains pending; no main integration or itch release.

2026-10-29: continued at `50240988`, same clean task worktree before edits.
Baseline is still immutable `4b5d4df3`; no main merge or release.

## Capture recovery and qualified baseline

Two new FAILED fixture attempts are excluded: `20261029T034740-quick` /
`2a21bc13e86b4195bcddb84255c80fe5c5d69cd70c4a56f606f2f302f0a88304`
asserted ParticleSystem.time on manual MainRays; `20261029T035244-quick` /
`52ff471eff0b4f112723c3f2fba861cd75fbcb6db8e0edbe077a71581d73c722`
asserted system time 60ms but Contact system duration capped it at 50ms. Neither
indicates a production timing failure. Fixture now asserts actual core/ray particle
age (`startLifetime - remainingLifetime`); manually integrated debris stays on its
existing simulation path. No runtime fix was needed.

Qualified `20261029T035847-quick`, source
`c77abf0b78ac4ace29313f2bb33b39ab55318cfb3193b9fbc8938379c7b88e75`:
**1/1 PASS, 0 skip, 15.3484053s**, Unity2022.3.25f1/OpenGLCore llvmpipe.
Exact test `ParryContactTintCaptureQA_ee2475c64ecf.CaptureMatchedPostprocessedPairs`.
XML, marker, 26 record rows and all52PNG names/dimensions qualified together;
archive digest recomputed from exact ordered member bytes equals snapshot.
`identity-receipt.json` retains archive/fixture hashes. All52nearest2x images and
review sheets generated on Ubuntu using installed FFmpeg, threads1; host Python
lacks PIL, so the failed PIL invocation produced no transformed images.

40baseline images = four variants x five times x post640x360/world320x180;
10candidate-A images plus two separate repeated15ms images. Repeated post/world
PNG bytes equal baseline; actual core/ray counts, startColor, particle ages,
light off state and background gain asserted. Contact(1.05,-0.67,0), direction+1,
seed0, angle20, fixed camera/pose/exposure. No world ray duplication (full/ray-off
world PNGs equal at every time). Exact evidence resides under the same existing
contact-tint-20261029 directory; remote qualified result folder retained.

## Direct static review and selection

Directly viewed all40baseline and10candidate-A original images via native-size
comparison sheets; all20baseline post contact crops viewed nearest2x at source
rectangle x150,y110,width180,height140; allfive A post images viewed full nearest2x.
World320x180 images viewed separately. These are static images, not motion review.

| Time | Baseline layer attribution | A versus baseline |
|---|---|---|
|15ms|Core contribution small in first cel; ray-off removes gold diagonal, light-off reduces weapon highlight|Bright contact retained, distant ray unchanged|
|30ms|Same first cel; stock weapon highlight remains with core off|Gold contact edge, bright center retained|
|60ms|Core still first cel; detached orange debris separated from ray|Core change subtle, ray taper/length and receiver light retained|
|100ms|Core-off removes the broad white star; ray-off/light-off retain it|White star surroundings become warm gold; smaller bright center, weapon contact more legible|
|140ms|Core-off still removes star; faint ray separate, light weaker|Gold surrounds persist without dark orange collapse; small bright center remains|

Layer attribution gate PASSED: channel0 causes the broad100/140ms white region;
ray remains gold when core is disabled. A `(1,.78,.28,.80)` is the first static
candidate passing the requested criteria. B/C were NOT attempted. No new source
reference frames or motion frames reviewed yet; final runtime visual acceptance
remains pending.

Production edit: only channel0 tint in ParryImpact.asset changed to A; all other
asset fields unchanged. Existing pause/pool test adapted to verify live tint
snapshot, pause color/age hold, resume age, new tint after clear/reuse/disable.
Existing repeated missing-prefab builder test additionally compares authored tint
and serialized profile bytes after repeats. No runtime/builder implementation edit.
Production four-variant capture and six focused technical cases are queued under
the shared runner lock; no parallel Unity. No color commit/push before remaining
gates. Final hardware WebGL, keyboard coverage, complete selected-frame/playback
review, game memory and hash-labelled viewer remain pending.

---

# Resume check - 2026-10-29 03:13 UTC

Continued at clean `34e1172f`, fetched origin; same task branch/remote. Old owned
container `007da4fba4b4` is now absent and runner lock has no holder. No Unity
container running. Host memory improved to about 6.3GiB available, but I/O did not:
some avg10 72.29 -> 85.76 -> 92.42%, sda sampled 82-93% busy, read await about
141-144ms and write await 91-122ms. Load rose 17.27 -> 24.05. No new Unity attempt
was started under the unchanged blocking I/O conditions. Other jobs untouched.
Local receipt `contact-tint-20261029/resume-0313-host.txt` retains the host check.

Adapted the same external `ParryHdrQA-v2.cs` capture loop further: actual core/ray
particle counts, core `startColor` and particle age assertions; byte equality of
separate 15ms repeated post/world PNGs. This completes missing assertions in the
existing fixture approach, without production edits. Updated template SHA256:
`b8fe860b4a81811977d34d9406b9b4934764871a4c04ca6d28af9a54b5df6d28`. Adapter syntax check passes; new C# assertions have NOT executed.
`adapter-v2-manifest.json` updated. Existing exact archive hashes above/below remain
historical; the edited template must receive a fresh hash-named injection next run.
No candidate selected, no new images/frame review, game memory/viewer unchanged.
Resume only when host I/O materially improves; retain the same worktree/evidence
ownership and cleanup conditions. Remaining color and hardware QA gates unchanged.

---

# Contact tint recovery checkpoint - 2026-10-29 03:06 UTC

**Incomplete: production tint unchanged; no candidate visual acceptance.**
Continued the clean existing task worktree at `f1c8a109`; fetched origin, verified
`micharel09/TypingHero` and matching remote task HEAD. No main integration/release.
Baseline `4b5d4df3`, saved baseline and current ParryImpact bytes match SHA256
`0f81a3b070bd1c12be48559a9f99d1a8367a51f51e44e2b01737dd2aef5669a1`.
Dirty parent checkout untouched. Approval viewer `0ee2a52f` unchanged.

## Capture adapter implementation and identity evidence

Adapted the existing external `contact-tint-20261029/runner-captures.py` and
`ParryHdrQA.cs`; new external files are `runner-captures-v2.py` and
`ParryHdrQA-v2.cs` in the same evidence directory. No shared runner or production
source was edited. Reuses authored contact/pose/camera/readback and copied profiles.
Archive entries now retain original file mtimes after LF normalization; digest
uses the exact archived bytes. Injected fixture file/class use the content hash;
fixture writes version/hash/expected times/variants before executing captures.
Removed the unused historical shader branch; ray selection uses channel 5, core
selection channel 0. Adds tint, layer configuration, simulation-time and background
gain assertions, plus a separate repeat-full at 15ms. This is an unfinished
verification adapter, not qualified production color evidence.

`archive-investigation.json` records no duplicate members in either old archive
or the first new archive. Both old fixtures had mtime=0 (1970); both contained the
five-time loop. This is a cache hypothesis, NOT a proven cause of the old mismatch.
New archive fixture and extracted source both hash
`d8b1b2a450ec805c41fcb74b3c1c4c2bdb1b5dc353523a9b0a5226ddc461e8a3`.

- `20261029T024812-quick`, input
  `42a0e5ad6bbf43bfe331bf5d298a5ce9dcad63a30b2d09fa0919c052f4bd54bb`:
  correct hash-named XML test and runtime marker verified. Test FAILED, container
  exit 2: fixture asserted 15ms for every system, including an inactive system
  whose time stayed 0. No completed capture record; excluded from baseline/color QA.
  Corrected assertion to inspect systems with live particles. Correction remains
  unverified because the next run did not reach the test.
- `20261029T025325-quick`, input
  `e03fab2714ed136764026623ecd57edc8698ad6baba8d469f3385325f5664db3`:
  fixture class suffix `bd9386cbf441`. ILPP initially could not find its socket,
  later connected; compile completed in 163.493728s, assembly reload 30.501s.
  Then three consecutive observations showed no log progress, Unity state D,
  `blk_mq_get_tag` / `folio_wait_bit_common`. Host load reached 33.82;
  I/O some avg10 reached 89.54%, memory available about 2.8GiB, no swap.
  No test XML/runtime marker/images. Owner requested graceful stop of ONLY
  `typinghero-loop-3297620-1793242405812849727` (ID `007da4fba4b4`), after checking
  its /results mount. `docker stop` returned 0; container was Dead/removal pending
  immediately afterward. Unity PID 3301356 is absent. The stuck owned Docker client
  ignored SIGTERM; SIGKILL of that verified client released the helper (client -9).
  Container inspect recorded exit 143 after the owner stop, OOMKilled=false.
  This intentional cancellation is separate from historical
  unexplained exit 143. Pre-stop inspect says OOMKilled=false; no OOM cause claimed.

## Ownership, receipts and resume gates

Existing evidence root remains
`D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/`.
Owner/consumer: this task. SSH/SCP used only to `fly-server`; exact new remote result
folders are `/data/typinghero-game-loop/results/20261029T024812-quick` and
`/data/typinghero-game-loop/results/20261029T025325-quick`. Local subfolders preserve
logs, available XML/marker, snapshot and pre-stop container/host/stop-reason receipts.
Retain these task artifacts and worktree for continuation/visual approval; remove
only after needed evidence is preserved and the task is accepted/closed. Shared
Library/source/build/cache and other jobs were not deleted or stopped.
Adapter/fixture/investigation SHA256 values are in `adapter-v2-manifest.json`.
Current adapter hash `f232e28c675db0c618c5c4022d9df54aad9d4a927bff2af5c4a7ceb864428976`;
fixture template hash `33f9f183a0b24af4b971040d7264c39df4044d970885a49127028601a4a79650`.

Before retry, confirm the owned container/client have fully exited and runner lock
is available; do not kill other waiters. Require materially improved host I/O,
then use the existing exclusive adapter. Do not clear Library or repeat identical
runs under unchanged pressure. Still required: qualified XML+marker+complete
capture record+52 images (40 baseline, 10 A, 2 repeat), repeat stability, actual
particle/lamp state checks, nearest2x generation on Ubuntu and direct full layer
comparison. Marker alone only proves execution identity, not image validity.
No new images or video frames/transitions actually reviewed: 0/0. No A/B/C selected.
All production edits, final 40-image set, scoped tint/pause/pool/builder evidence,
final hardware WebGL/key-input/frame/playback review and new viewer remain pending.
No new runtime/game-memory acceptance; historical 143 cause remains unverified.

---

# Contact tint follow-up - 2026-10-29 (blocked before preset selection)

User-approved scope: reduce the broad white stock contact while retaining a small
bright center and warm gold surroundings. Continue this branch; push task branch
only, retain worktree/evidence for visual approval. No main merge or itch release.
The unchanged runtime acceptance below is historical evidence, not acceptance of
a new contact color. No new preset or visual approval has been established.

## Verified source and intended adaptation

Clean worktree `D:/Projects/TypingHero-worktrees/parry-cinematic-flash`, branch
`micharel09/parry-cinematic-flash`, local and fetched remote HEAD
`4b5d4df36236120c8748d73e0d947bfc8476138e`; origin is
`https://github.com/micharel09/TypingHero.git`. Parent dirty checkout untouched.
Unity source version remains2022.3.25f1. Ubuntu `fly-server` is reachable.

Reuse Pimen Hit Effect01 assets/licensing in `PixelSource.txt`/`PimenSource.json`,
`ImpactVfxPixel.cs` emission startColor=flash.tint, and existing controlled capture
fixture `ParryHdrQA-final-captures.cs`. Profile channel0 currently has white tint;
channel5 is the existing presentation ray. `ParryImpactAssets.Prepare()` only
creates missing prefab/bindings and repairs prefab reference; it does not replace
the existing authored tint. No builder edit/migration/version bump is needed.
Shared material edits were rejected in favor of the approved per-profile tint.

Before any production edit, compare full/core-off/ray-off/light-off at15/30/60/
100/140ms with identical pose/contact/camera/post/background gain, post640x360 and
world320x180. Only copied profiles may be modified. Then try A(1,.78,.28,.80),
B(1,.65,.12,.80), C(1,.65,.12,.65) in order, stopping at the first visually passing
candidate; keep all timing/size/gradient/alpha/light/shader values unchanged.
Do not extend the approach if all three fail. No production edit made this turn.

## New execution evidence and rejected results

Evidence root:
`D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/contact-tint-20261029/`.
Owner/consumer: this parry task. Retain local evidence and the two exact remote
result folders below until follow-up/visual review; clean only after receipts and
needed artifacts are preserved. No shared Library, builds, toolchain or unrelated
jobs were removed. Baseline profile bytes are frozen as
`baseline-ParryImpact-4b5d4df3.asset`; production profile remains byte-identical.

- External fixture adapted the existing capture loop to all five times and adds
  channel-based core-off plus candidate-A on copies. Same existing SSH/SCP runner,
  Ubuntu Unity and OpenGLCore software capture route; no Windows build/processing.
- `20261029T020214-quick`, snapshot
  `4ac7e32e8681312774f891c4c36602d3272ed4fd98e7227f86d752b7e9829f01`:
  XML says1/1PASS, but output is the OLD15-image/three-time variant set, inconsistent
  with the archived injected fixture. Rejected as contact-tint evidence. Cause of
  source/result mismatch is unverified; do not treat it as successful candidate QA.
  Local receipt: `rejected-020214-tests.xml`. Remote source archive/log/images retained.
- Renamed injected file to `ParryContactTintQA.cs` and capture class to
  `ParryContactTintCaptureQA` to distinguish the new run from the old fixture.
  `20261029T022115-quick`, snapshot
  `965d1b3df7f54fef517797b3b9ebf8c23ab90fa1719576ab6c9b8b37f815a067`:
  container exited143 after assembly reload; no XML/capture images. The cause of
  termination is unverified. Local receipt: `failed-022115-unity.log`.
- Both remote paths are under `/data/typinghero-game-loop/results/` with those exact
  names. Shared runner contention and high host I/O pressure were observed; neither
  is established as the cause of143. No other task process was stopped.
- Only historical controlled `new-presentation-15ms.png` was viewed at native640x360
  this turn: broad white contact and gold distant ray visible. It does not establish
  layer attribution. No new reference frames or output-video frames/transitions
  reviewed; counts0/0. No candidate was actually rendered/visually evaluated.

## Resume and remaining acceptance

First resolve source/result identity and obtain the five-time ablation images.
Do not select/apply A based on the rejected old run or the unexecuted candidate
fixture. All production tint changes, candidate table, baseline/final pairs,
nearest2x images, scoped tint/snapshot/pause/pool/builder tests, final hardware
WebGL build, real-keyboard eight signed angle/facing cases and full selected-frame
review remain unverified. No new source/payload/render identity can be claimed.
Reuse prior shader/HDR/allocation/resolution evidence only for unchanged inputs;
those receipts do not accept a new color. Update affected game memory only after a
verified preset change; its existing production description remains unchanged now.
Create the new hash-labelled viewer only from qualified new artifacts, retaining
`approval-0ee2a52f` unchanged. Stop at visual approval; no merge/release.

---

# HD-2D parry completion - 2026-10-28

## Current status and approval gate

Implementation and scoped technical QA pass:102logic +26graphics +5follow-up tests.
Native640x360 evidence covers all8angle/facing combinations. Controlled comparisons
and A-B-B-A measurement are complete. No consistent FPS improvement can be inferred.
**Awaiting user visual approval; no overall visual acceptance or release claim.**
Commit/push the verified task branch only; no main merge or itch.io release.
Continue on `micharel09/parry-cinematic-flash` at
`D:/Projects/TypingHero-worktrees/parry-cinematic-flash`; preserve existing task edits.
After the final source/artifact audit, commit/push only the task branch and retain
worktree/artifacts for visual approval. Do not merge main or publish.

Evidence root (QA below):
`D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/hd2d-fix-20261028/`.
The current viewer is `QA/approval-0ee2a52f/review-0ee2a52f.html`, opened in Orca page
`b83b5d8f-967b-4c15-9d59-a1f92367b3d6`. It is labelled awaiting user visual approval.

## Implementation and authoring contract

Adapted existing `ImpactVfxPixel` buffers/quad path, arena render callback,
`PresentPass`, preset snapshot and pool cleanup. No new camera, full-screen RT,
blur pass, per-burst material instance, dependency, GUID or serialized binding.
Character sprites, stock flash/debris, damage, parry timing, hitstop and camera
impact remain on their existing paths.

- Arena owns projected registrations. Its world-camera pre-render hook captures
  viewport position, normalized dimensions, angle, current color and named pass
  after lens/framing updates. Presentation draws that matching world-frame snapshot.
- Presentation ray:1`GetParticles` at capture,0`SetParticles`,0particle reads at
  draw (source-derived counts; before:2reads/1write). World projected sizing keeps
  its existing particle size updates. Legacy bursts without projected rays do not register.
- `renderInPresentation=false`: existing world route/`projectedPixels` meaning.
  `true`: projected presentation dimensions even if `projectedPixels=false`.
  Missing active arena chooses world at emission and stays there until the next emission.
  Invalid positive/finite2D dimensions or missing`PARRY_PRESENTATION` fails before
  emission/renderer hiding. Pass is found by`Material.FindPass`, captured at preparation.
- Arena disable/reset/session unbind cancels only its presentation channels,
  clears snapshots/registration and restores prior hidden/culling state. Stock,
  physical debris, world projected channels and other arenas retain their lifecycle.
  Re-enable cannot revive an old ray. New emission works normally.
- Clear/lease return/disable are repeatable. Full preset restoration stays at Play
  and legacy ConfigurePreset; cleanup restores only presentation state it changed.
  Particle/sample buffers grow only during preparation beyond retained capacity.
- Ordinary Pause preserves particle age/color/geometry; Resume continues. Rig
  disable independently clears pulses and returns its arena background gain to1.
  READY/Slayer environment gain stays separate. QA angle/quad telemetry is read-only.

Kept the initial HD-2D preset unchanged during fixes:128x6reference pixels,
1.25maximum core width, gain2, halo.35, minimum background.90, light7/range3.5,
angles20/45/-15/65 mirrored by facing; alpha0/1/.80/.35/.12/.05/0 at
0/15/30/60/100/140/220ms. Outer width/energy taper continuously toward both ends.

## Three baselines and immutable identities

| Baseline | Purpose | Identity |
|---|---|---|
| Rejected180x9 world ray | Historical visual comparison |2bcd185670baa8a33fcd005007f4f1da60041b4cb6a5c03109037994bea52f9c|
| Initial HD-2D draft | Runtime reproductions and before-optimization profiling |e787448bddd69cdbb503d95ee8daf763590a92306a96c6b61686d8b0552ba7f3; profiling build20261028T085837-webgl complete|
| Final runtime | Logic/GPU/native visual evidence |0ee2a52ff3076da69e2c44b45400adbb75ef14782e505986ebd8f9e7d924054b;20261028T075930-quick|
| Same final runtime, profiling build | Allocation-enabled A-B-B-A candidate |0d3313c5de3941d1c5317bb4b01f6c5f0ed54f2e0d1a842937c8012679a8b1e4;20261028T102003-webgl complete|

All13changed runtime/asset hashes match the tested final archive, including the
profiling archive; external fixture inclusion differs. Receipts:`source-manifest.json`,
`source-identity-check.json`,`render-work-0ee2a52f.json`, profiling`runtime-identity.json`.
Final visual player's33file hashes have canonical sorted-JSON manifest SHA256
`24641846f28b603ccb924852fc84ccb66a3d7ee47a073e969e2748738516555b`
(`payload-0ee2a52f.json`). Left supplemental payload matches all33files.

## Tests and defect closure

Ubuntu Unity2022.3.25f1, URP14.0.11, Cinemachine2.9.7. Final logic:
**102/102PASS,0failed/skipped,13.4982443s**:64ImpactVfxTests(including clock partial),
38CombatCameraImpactTests. World zoom assertions explicitly select world sizing;
presentation assertions inspect captured quads, not hidden particle startSize3D.
Seven envelope milestones, snapshot/pool reuse, pause, owner overlap and READY/
Slayer independence remain covered.100capture calls after16warm-up allocate0managed
bytes on the test thread; this result alone does not cover command recording/player.

| Closed defect | Regression evidence |
|---|---|
| Arena disable/re-enable retained old presentation ray |PresentationWithoutProjectedSizingCapturesQuadAndCancelsOnArenaDisable; PresentationResetIsScopedToOwningArena; GPU runtime reproduction PASS|
| Presentation=true/projectedPixels=false hid the only renderer |Captured normalized quad dimensions and GPU route reproduction PASS|
| Invalid dimensions/pass could hide renderer or emit first |Both InvalidPresentationFailsBeforeEmissionOrRendererHiding cases PASS|

**26/26GPU PASS,0failed/skipped,1.5780723s**, OpenGLCore/Mesa llvmpipe software:
24angle/facing/output cases at640x360,1280x720,960x600(letterbox0,30,960,540),
alternating zoom; actual capture-before-draw, HDR red peak2.287109-2.318359>1,
centroid within1output pixel. Two original runtime repros pass. This is functional
GPU evidence, not hardware timing or postprocessing acceptance.
Exact tested fixture:`ParryHdrQA-tested-0ee2a52f.cs`, SHA256
3181edf42b200235d276c449527eb3047e6615d1be77fbb6a053031d564fac36.
XMLs:`logic-tests.xml`,`hdr-tests.xml`; WebGL QA and strict repeated builder PASS.

Final external graphics follow-up **5/5PASS,0failed/skipped,33.8575974s**,
`20261028T110059-quick`, input
`2c4771e871eec46860913042c9eaf311d952f0c8a1cb8466cf26f229dfdcab58`.
All13runtime/asset hashes equal the final0ee archive. Tests cover controlled
postprocessed pairs, receiver-only diagnostic, capture+draw allocation,3viewport
contacts x4signed angles/energy taper/envelope, and fixed world fallback until
next emission.100capture+command-recording iterations after16warm-up allocate
**0managed bytes** with retained mesh/MPB/command buffer. This is not whole-player
zero allocation. Position cases retain HDR>1, centroid within1px and near energy
more than3x far energy;15/30/60/100/140ms peak decreases,0/220 covered by logic.
Frozen external fixture:`ParryHdrQA-final-captures.cs`, raw SHA256
38ce98262281b601081f9ede401996f4ca73c2cef72385cdd9a4dc2ee2045468.
Exact injected LF/Category-normalized fixture:`controlled-final-2c4771e8/tested-fixture.cs`,
SHA2566bdc88a87e988448b5e516bb049568f576750d3d80a4dc633fb5da96b24fc6b2.
XML, snapshot, runtime identity and all34PNGs retained in that directory.

### Controlled image review (all34images directly inspected)

Same authored contact(1.05,-0.67,0), pose, camera,20degree angle, ambient/post
and frozen simulation time.15post640x360 +15world320x180 images at15/60/140ms;
4receiver on/off post/world images at15ms without postprocessing.

| Pair / exact filenames under controlled-final-2c4771e8 | Observation |
|---|---|
| controlled-captures/old-world-15ms.png vs new-presentation-15ms.png | Old bright broad shaft competes with contact; new fine gold ray concentrates brightness at the stock contact, floor/character silhouettes remain legible outside center. |
| controlled-captures/new-world-15ms.png vs new-presentation-15ms.png; same pair60ms | Same shader distribution; world ray visibly stepped, presentation edge smoother at640x360. No sprite-resolution change. |
| controlled-captures/new-presentation-60ms.png vs ray-off-60ms.png | Fine gold extension is isolated from retained stock/debris. World-new-presentation and world-ray-off PNGs are byte-identical at all3times: no duplicate world ray. |
| controlled-captures/new-presentation-140ms.png vs old-world-140ms.png | New extension is almost gone while stock contact/debris remain; old shaft remains broad/bright under its held historical alpha curve. All non-ray layers remain fixed. |
| controlled-captures/new-presentation-15ms.png vs light-off-15ms.png; same pair60/140ms | Local highlight on character/weapon is stronger with light; floor response is subtle. Background gain is held identical. |
| receiver-captures/receiver-on-no-post-15ms.png vs receiver-off-no-post-15ms.png | Receiver response isolated without post: character/weapon highlight visible, floor response localized and weaker. Same presentation ray. World pair also directly inspected. |

Supplementary RGB absolute-difference diagnostic on the unmodified no-post pair:
character rectangle(180,160)-(290,245):4834changed pixels/max164of255;
floor rectangle(150,245)-(340,330):2342changed/max30. Whole difference bbox
(145,115)-(314,266). Rectangles are approximate screen regions, not material masks;
these counts demonstrate local response, not an exact light-energy measurement.
No shader/profile change after these snapshots or source-frame review.

## Native input, frame and playback evidence

Chrome152.0.7977.64, Playwright1.63.0, Ubuntu ANGLE/Intel HD4600(OpenGL ES3.2),
640x360/world320x180; keyboard/mouse only, bridge read-only, provider writes0.
Four successful runs at`20261028T075930-quick/`: right-final, left-final,
right-seed3-final and left-seed3-exclusive, each suffixed`-0ee2a52f`:
**12checks each/48total,0exceptions**. Main clips each contain4consecutive parries,
fourth enters Slayer; Pause/Resume, Restart and Menu pass. Actual cached signed
angles cover20/45/-15/65 and-20/-45/15/-65; maximum1quad, not seed labels alone.
Supplemental left warm-up parries1; its primary-runner-exclusive window still had
other host jobs(load20-24), so it is not an idle-host performance result.

Selections were recorded before detailed review. Every source frame, repeat and
consecutive transition in these spans was directly viewed at320x180 full-frame
cells, with sheet boundaries and native640x360 contacts also inspected:

| Final source | Selected original frames / PTS | Actually inspected | Per-frame log |
|---|---|---|---|
| right-final |448-1126 /17.920-45.040s|679/679frames,678/678pairs|right-review-0ee2a52f.csv|
| right-seed3-final |256-712 /10.240-28.480s|457/457,456/456|right-seed3-review-0ee2a52f.csv|
| left-final |256-1034 /10.240-41.360s|779/779,778/778|left-review-0ee2a52f.csv|
| left-seed3-exclusive |256-874 /10.240-34.960s|619/619,618/618|left-seed3-exclusive-review-0ee2a52f.csv|

Combined final output:2534unique selected source frames/2530within-source pairs.
Decoded video PTS, not provisional wall-clock offsets, identify contacts.
Native contacts: right20 warm-upF481/19.240s(the first requested20peak is unresolved
in that recording), right45F700/28.000s, right-15F789/31.560s,
right65F922/36.880s with Slayer and supplementalF361/14.440s without Slayer;
left-20F539/21.560s,-45F630/25.200s,+15F720/28.800s,-65supplementF593/23.720s.
All native/2xnearest-neighbor stills are under`approval-0ee2a52f`.
Pause holds rightF790-832 and left supplementalF594-647 behind HUD; Resume fades;
Restart/Menu leave no ray. Main left's fourth -65 peak was unresolved; the passing
supplement resolves that gap without relabelling it.

Packaged clips retain original frames, no interpolation; disclosed main/supplement
edit boundary. Right:1136normal frames/45.44s then4xheld slow181.76s,
5680encoded frames/227.200s/25fps. Left:1398normal frames/55.92s then slow223.68s,
6990frames/279.600s/25fps. FFprobe timing receipts and source maps retained.
Both browser playbacks reach end, no media errors/corrupt frames. Browser-reported
dropped presentations:right3/5692,left20/6994(includes initial starts/seeks);
callback traces are bounded diagnostics, not extra direct visual review counts.
`*-browser-playback.json` records the technical playback separately from frame review.
Continuous perceptual playback approval remains the user's visual gate; screenshots
are not represented as continuous video viewing.

## ReBlade and rejected-ray comparison

Retained Shorts`https://www.youtube.com/shorts/rp4vZPWLXsQ`, video SHA256
`a9e1f284cdf9fff5a9286f12fc52b8df4111af16f45e141e6d4833795ffd77b6`.
New direct review:sourceF276-514,4.600-8.566667s,**239/239frames/238/238pairs**,
including all held frames/boundaries and focus301/319/350/390. Initial scope ended
F419 then extended to source end because embers remained. Historical515/514review
is separate. Source ends before all embers expire. Normal/quarter-speed technical
playback receipts are separate; video cannot establish internal shader/HDR/light settings.

Old180x9 fresh motion review:sourceF490-610,19.600-24.400s,121/121frames/120/120pairs,
retained right review indices72-192; historical529/528review separate.
Per-frame`old-180-motion-review.csv`; four old contact stills also reopened.
Old sourceF550-552/22.000-22.080s holds the broad bright bar; F553-554/22.120-22.160s
fades with shaft shape; F555/22.200s ray absent, orange physical particles remain.
Technical old packaged2.880-7.720s playback at1x/.25x complete; no corrupt/media
errors; dropped24/390presentations recorded, not hidden.

Compare matching contact phase and body-height ratios: ReBladeF301~2.5-2.7attacker
body heights; old45visible>=3.5(lower end behind tray); new45F700~2.5-2.7.
These are coarse manual visible-extent estimates with approximate body bounds,
not exact world-space parameters. Source images/crop scales do not define the ratio.

| Criterion | ReBlade frame/PTS | Rejected180x9 | Final candidate frame/PTS |
|---|---|---|---|
| Contact |F301/5.016667s, compact principal center|right45/22.000s, shaft competes with center|F700/28.000s, center brightest; weapon approach/contact reads outside stock core|
| Thinness/tips |F301 fine continuous taper|right20/18.480s and45/22.000s broad shaft|F539/21.560s,F700/28.000s continuous narrowing/energy decline|
| Transparency |F350/5.833333s armor/floor outside center read|left-15/32.560s broad overlay|F720/28.800s rib silhouette/floor readable away from contact|
| Color |F390/6.500s gold periphery|right45/22.000s white-yellow shaft|F789/31.560s outer ray remains gold after tone mapping|
| Rhythm |F297-419 short contacts, then embers|F550-554 bright/fading bar shape|F700contact,F703-719stock/ember recovery; paused holds are intentional|
| Receiver/occlusion |F319/5.316667s local highlight; internals unknown|Old stills do not isolate light|HUD occlusion visibleF790-832; controlled15/60/140ms ray/light pairs and15ms no-post receivers confirm local response (floor subtle)|
| HD-2D |Pixel characters/smooth gold impression|World320x180 ray visibly stepped in old stills|NativeF481/19.240s,F630/25.200s retain pixel characters and smooth ray; other sizes functional software-GPU only|

Reference images:`E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/shorts-rp4vZPWLXsQ/focus-{301,319,350,390}.png`.
Old images:`D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/primary-review/approval/`.
Final images:`QA/native-contacts-0ee2a52f/` and`QA/approval-0ee2a52f/`.
No exact ReBlade match claimed; no visual change after final source-frame review.

## Render configuration, optimization measurement and reviews

Linear color; world320x180ARGBHalf/depth24; existing2cameras/presentation path.
`DungeonLighting.post` binds unchanged`DungeonPost.asset`:ACES(serialized2),
exposure.35, bloom threshold1.15/intensity.12/scatter.65, vignette.1.
`render-config-0ee2a52f.json` records source paths/hashes, native640x360 and software
1280x720/960x600 outputs. Existing global bloom/tone mapping remains unchanged.

A-B-B-A complete: immutable initial-draft and allocation-enabled final players,
640x360, same real-keyboard4parry+Slayer scenario, detailed-QA/GPU-query observation,
Development build without extra profiler options. All4runs pass (A10checks each,
B12each;0GPU-disjoint samples). Raw receipts and `summary.json` under
`QA/abba-0d3313c5/`; sample countsCPU/GPU505/593/434/411,
interval506/594/435/412, allocation61/67/62/58.

| Run | CPU callback p50/p95 ms | GPU p50/p95 ms | Combat interval p50/p95/max ms | Managed sample p50/p95/max bytes | >50ms hitches |
|---|---|---|---|---|---|
| A1 baseline-before |21.440/49.615|9.607/26.944|25.335/56.410/257.620|5167/63997/75293|37|
| B1 right-after1 |18.005/37.755|8.668/23.233|21.500/45.790/209.140|5279/64153/104189|21|
| B2 right-after2 |26.505/58.090|10.932/33.556|30.835/71.980/351.155|5279/64107/162836|69|
| A2 baseline-after |27.990/63.395|12.190/36.905|32.450/77.665/225.800|5165/64003/103387|80|

All combat hitches remain included. CPU is browser callback wall time, not a Unity
Profiler CPU marker; GPU timer queries correspond to those callbacks; allocation
state sampled about120ms, not every frame. Host load drifted from14.37/19.94/18.77
to10.92/16.02/17.47; main runner lock cannot exclude other host jobs. A2 is much
slower than A1; B is not consistently slower. **No causal FPS/CPU/GPU improvement
claim** from these noisy short runs. The optimization is verified through source
operation counts and isolated zero-allocation tests instead.

B allocation median is112-114bytes higher under detailed QA. Added read-only
`parryPresentationQuads`/`parryPresentationAngle` JSON fields add56characters
(112UTF16bytes) in PublishCombatSnapshot; this is a plausible observer-allocation
confound, not a proven exact attribution. Whole-game p95 remains about64KB;
never describe the whole game as zero allocation. Initial0eeplayer allocation
counter unavailable/concurrent-build timings are excluded from matched measurement.

Code review completed validation -> emission/route -> capture -> draw -> Pause ->
cleanup/reuse, including emitter/pool, legacy ConfigurePreset, camera reset/session
unbind and rig clock/overlap. No additional defect found;`code-review-0ee2a52f.json`.
Evidence review completed selected source frames/pairs and native stills as above;
all34controlled images were directly inspected. Final source/artifact audit is
recorded in final-audit.json; user continuous perceptual/visual approval remains pending.

Final audit:13/13runtime/asset identities match tested inputs;54viewer local links
resolve;75approval artifact files hashed in`approval-0ee2a52f/artifact-sha256.json`,
manifest SHA256`8d669fa6b677e26d653e89566d5be142874e13bb70c49c7afe5494166cf16c62`.
`git diff --check` passes. Runtime inputs unchanged after tests; scoped memory updates
only after image review. Git commit/remote mapping is retained in`delivery.json`
next to`final-audit.json` after pushing the verified task branch.

## Rejected diagnostics and retained resources

Original073240logic50/51failed only stale.65vs.90clock expectation; corrected final102pass.
Initial controlled083754fixture passed but disabled the camera before emission,
so both labelled routes chose world. Those15PNGs are diagnostics only, not a world/
presentation comparison. Corrected fixture asserts route, draw commands and world
texture equality with ray-off before accepting images; corrected5/5follow-up passes.
Failed left-seed3-final recording retained:391/391selected frames/390/390pairs viewed,
sourceF256-646/10.240-25.840s,`left-seed3-failed-review-0ee2a52f.csv`. Warm-up was missed;
requestedF591/23.640s has no clear -65 ray. A195.8msreceiving step aged it before
late PauseF596. It supplies no passing live-ray Pause/angle still. Passing exclusive
retry resolves the gap; no production lifetime change was made to hide host stalls.

Task owns result directories075930-quick,085837-webgl,102003-webgl and final capture
result110059-quick, plus their native child folders/exports. Export helper is the
unchanged HD-2D`video_contact_sheet.py`; task-local075930/review-venv/Pillow13.0.0.
Heavy build/export/test/profile work stays on Ubuntu; local files/Git/viewer are lightweight.
Keep D worktree/branch, QA evidence root, immutable players, raw clips, frame PNGs,
CSV/source/payload/timing receipts and review page through visual approval.
After approval/retention decision, remove only owned task resources after preserving
required evidence. Do not clean shared source/Library/toolchain, unrelated results,
other checkouts or old reference artifacts. No merge/publication during this gate.

---

## Historical checkpoints (superseded by the current record above)

# Gold diagonal parry ? QA revision (2026-10-28)

## Current acceptance and review gate

This revision supersedes the historical 72px horizontal/120ms implementation below.
Implement only successful sword parry at the existing contact, both directions.
180?9 world-render pixels (320?180), 2px gold ray, orange halo, final 20px taper at
both ends. Keep the stock contact flash. HDR float output, gain4, ZTest Always,
queue3100 after existing combat particles and before presentation HUD/menu.
Angles20?,45?,?15?,65? selected sequentially by presentation seed modulo4 and
mirrored by opponent direction; no gameplay Random calls. Rise0?15ms, hold to60ms,
fade to220ms. Gold local light peak7/range3.5; separate background gain down to.65.
Global bloom/tone mapping, sword sprites, parry timing, damage, hitstop and camera
impact remain unchanged. Old profiles default to existing behavior.

**Stop at visual approval. Do not merge main or publish itch.io.** Commit/push only
after QA evidence establishes the acceptance below; retain task worktree/artifacts.
Worktree: D:/Projects/TypingHero-worktrees/parry-cinematic-flash;
branch: micharel09/parry-cinematic-flash.

## Reference and reuse basis

Primary: https://www.youtube.com/shorts/rp4vZPWLXsQ . Retained source SHA256
`a9e1f284cdf9fff5a9286f12fc52b8df4111af16f45e141e6d4833795ffd77b6`.
The previous reference review records515/515frames and514/514transitions over
0?8.621s; that is prior-agent evidence, not a new inspection by this revision.
Four gold contacts around4.95/5.23/5.73/6.40s establish visual direction, not exact
angles or internal rendering parameters. Directly reopened the four-panel
`gold-streak-comparison.jpg` in the retained Shorts evidence directory.
Source record: E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027/
shorts-rp4vZPWLXsQ/{selection.json,review.json,review.csv,source.mp4}.
Required output coverage: all4angles?2directions, complete preparation/contact/
expiry, actor/weapon overlay, Pause HUD, local floor/actor light, Restart/Menu,
projected size through zoom, pulse overlap/owner cancellation/pause/world reset.
Source flare, curved sword sweep and spark cluster are separate features.

Adapt the existing HorizontalFlash.shader/material/channel5, profile snapshot,
pool and lease cleanup in ImpactVfxPixel/ImpactVfxBurst. Adapt beginCameraRendering
subscription from DungeonPlanarReflection; update only the captured arena camera,
using retained particle buffers. Bind rig arena via DungeonSessionBindings.Bind;
multiply ParryBackgroundGain with EnvironmentPresentationGain before ActorPass.
Reuse ParryImpactAssets' existing six-binding missing-prefab builder; it already
preserves custom references and repeated calls. Keep GUIDs and prefab bindings.
Tradeoff: projection tracks camera every render; shader's 2px line remains subject
to final pixel-grid rasterization/bloom, so source inspection alone cannot prove
final perceived thickness/color. No rendering-internals claim about ReBlade.

## Implementation checkpoint

- Four-angle list and optional projected sizing; empty list/disabled option retain
  old behavior. Size and chosen rotation are captured per burst; later asset edits
  do not resize or rotate it. Refresh immediately before the matching arena render.
- Cinematic rise/hold/duration/minimum gain are profile values captured per pulse;
  light/background share the existing pause clock. Overlap uses maximum light and
  minimum background gain. No scene-tree scan in background updates.
- Clear/cancel/disable/session unbind/camera reset restore parry gain independently
  of READY/Slayer presentation values. Legacy pulse envelope remains unchanged.
- Latest origin/main9e37447f integrated into task branch only, retaining upstream
  actor/shield source-ownership changes and updated execution-host rules.

## Verification in progress ? not accepted

The plan mentions a prior in-flight run, but no task-matched source/result pair
was established. A pre-existing5/5XML was observed at021251; its task identity
was not verified and it is excluded from this task acceptance.
Intermediate20261028T030947-quick:49/49PASS/0skip, input333d86937210cfb8b41ad9762aadbc5e7635d9ade948dc6e30889661781ef354.
Two later queued attempts timed out waiting for the shared runner; unrelated
03:14 test/production outputs are not attributed to this task.

Final logic snapshot6df402dde83937a6fd5b5959f5ce7f2ff03adaccce21e83d46724358302ac901,
result /data/typinghero-game-loop/results/20261028T033605-quick:
**51/51 ImpactVfxTests PASS,0failed/skipped,11.6657992s**. Includes all4angles?2faces,
seed/pool/legacy particle trace, live zoom/snapshot, rise/hold/fade/expiry,
pause/overlap/owner cancel, bound-arena independence and camera reset preserving
legacy pulses. Existing missing-prefab/custom-reference/repeat-builder tests pass.

Same source, pinned Unity2022.3.25f1, OpenGLCore4.5/Mesa23.2.1 llvmpipe LLVM15:
**1/1 HDR fixture PASS,0skip,1.461067s**. Actual shader draw into ARGBHalf, linear
RGBAFloat readback before tone mapping: RGBA(4.000,2.318,0.100,1.000); expected
(4,2.32,.1,1), tolerance.02. Material queue3100 asserted. This is a real graphics
API/software-renderer check, not a physical-GPU performance measurement.
The external fixture draws the existing shader into the HDR target; fixture,
source.tar.gz and separate logic/HDR logs are retained with the result.

A Windows Unity graphics process was started, then stopped without results when
latest-main rules requiring Ubuntu heavy execution were loaded. No local pass claim.
Current runner continues from the same source to QA build. Baseline uses main9e37447f;
normal/mirror output clips, post-tone-map color/order/local light, native lifecycle,
CPU/GPU/allocation comparison and per-frame output review remain **unverified**.
Do not substitute the historical videos below.
Local evidence: D:/Projects/TypingHero-evidence/parry-gold-qa-20261028/.

Native QA checkpoint: Linux host Chrome152.0.7977.64, Playwright1.63.0,
ANGLE GL-EGL/OpenGL ES3.2 on Intel HD4600, timer queries available. Use the existing
host Chrome and a process-scoped render group (`sudo -n -u quan -g render`);
no persistent group/system changes. Docker/SwiftShader is rejected for visual QA
(observed401?1217ms frames). Initial Vulkan images at the fourth clash include
Slayer presentation; they do not establish a new shader/renderer defect.

Driver correction: CombatSession.Parries is the successful-parry counter.
CombatCameraImpact.ParryCount can remain3 when fourth parry enters Slayer, so it
must not drive four-parry acceptance. Earlier falsely-failed recordings remain
retained with their raw counters. Pause layer/hold is checked on the third normal
parry; retain the fourth parry and Slayer transition. Supplemental VFX seed3 shows
65? before Slayer, without changing gameplay RNG/stamina/damage/timing.

Reuse the existing keyboard down/observed-pose/up protocol and real-input warm-up
plus UI Restart from camera_impact.cjs. The publication observer sends notifications
to Node through Playwright page.exposeBinding; Node sends actual keyboard events.
No Unity gameplay setters or bridge mutation. Use compositor video only; dual
canvas/compositor recording and full-particle CDP polling were removed from the
external driver. Docs: https://playwright.dev/docs/api/class-page#page-expose-binding
and https://chromium.googlesource.com/chromium/src/+/HEAD/docs/gpu/using-gpu-hardware-in-headless-chrome.md .

Baseline9e37447f: sourceec721e725f1c5acb6d7d3637f4d7b8006d675249d3f43f2e31ca44238fcfd8d4,
result20261028T040651-webgl. Development QA/repeat builder pass; allocation recorder
available. Native baseline-right-egl-04 passes10 checks after warm-up;0 exceptions.
It is a harness/integration pass, not a completed matched performance comparison.
Bidirectional QA source2bcd185670baa8a33fcd005007f4f1da60041b4cb6a5c03109037994bea52f9c,
result20261028T044432-webgl, development QA/repeat builder pass. Native/frame review
and supplemental-seed build remain pending; no candidate visual acceptance yet.

---

# Historical horizontal implementation and acceptance (superseded)

# Horizontal parry flash

## Acceptance and reference scope

- Successful sword parry only, at the existing contact point, in both directions.
- Horizontal 72 world-resolution pixels; warm white core 1px, gold halo 3px, tapered ends.
- Rise 0–20ms, hold 20–40ms, fade 40–120ms. Existing flash, sparks, light and gameplay unchanged.
- Inspect output preparation, contact, expiry; Pause/Restart/Menu and pooled reuse; typing tray clearance.

## Reference selection (before frame review)

Steam app3489340, ReBlade: The Death Spiral, SNF Trailer (EN), movie257457478.
Source: https://store.steampowered.com/app/3489340/
Video: https://video.akamai.steamstatic.com/store_trailers/3489340/1492852456/bee6c8072e420aa2221843bb4e0d636828d19217/1791775482/hls_264_3_video.m3u8
Overview 0–63s at1fps used only to locate candidate; not counted as frame review.
Selected segment42.0–46.0s,30fps, expected120frames/119transitions: stationary
sword encounter before, during and after gold contact flashes, before the scene cut.
Review pending. Exact ReBlade match remains unverified; requested dimensions are independent art direction.

## Reference review

Directly inspected120/120frames and119/119consecutive transitions, including
held/repeated poses, at original30fps timestamps42+n/30 seconds. Six numbered
contact sheets (0–19,20–39,40–59,60–79,80–99,100–119); no skipped frames.
0–6: preparation/closing;7–10: contact warm-up;11–19: gold/white flare and long
lateral rays;20–34: held contact with diminishing sparks;35–47: recovery and next
preparation;48–69: second contact, narrow lateral ray and held white/gold center;
70–99: release/separation and following approach;100–119: next sword strike/contact.
Transitions19?20,39?40,59?60,79?80,99?100 directly compared across sheets.
The segment ends during the next strike, so its final follow-through is unverified;
the first two full contact/recovery cycles are covered. Images are the extracted
video frames themselves; no independent still proves opposite-facing behavior.
No source assets were imported. No exact reproduction or exhaustive game-reference
coverage is claimed. Requested72px/120ms values remain independent art direction.

## Implementation

- ParryImpact adds channel5, a stationary camera-plane quad using an original
  procedural shader:72×3pixel envelope,1pixel warm-white center, gold halo,
  tapered8pixel ends. Shader uses the existing particle Color/UV stream.
- FlashLayer.sizePixels2D defaults to zero and lockVariation defaults false.
  Only the new layer opts into72×3 and locks rotation/variation. Existing source
  flash bytes, physical profile, parry contact path, flash RNG, lighting, damage,
  hitstop/camera and pause ownership are preserved.
- Gradient alpha at normalized0,1/6,1/3,1 is0,1,1,0; lifetime120ms.
  Reuses emission/pause/lease cleanup; no gameplay API or save-data changes.
- Missing-stock-prefab builder creates the same sixth binding and preserves
  existing authored/custom references on repeat.

## Verification checkpoint

Unity2022.3.25f1, Ubuntu runner,45/45filteredImpactVfxTests PASS,0skip;
result /data/typinghero-game-loop/results/20261027T151249-quick,
input9bc1dc0e68bb8568e05db05771265c7b4d267ec99cea4c28a979a2119850ddad.
Includes48direction/seed/pool-reuse combinations, fixed dimensions/rotation,
20/40/120ms alpha envelope, expiry with old flash still alive, pause/read holds,
shared-profile consumers and missing-prefab/repeated-builder tests. Existing
flash serialized bytes identical; only existing prefab root's children/bindings
changed, plus four cloned particle objects. git diff --check PASS.

Initial QA build stopped at unrelated protected Cobblestone font asset CRLF/LF
serialization. External task adapter normalizes YAML/meta line endings in the
immutable source archive; no checker/toolchain/repository font changes.
Final native input/render checks and integration pending. No itch.io release.

Native right pilot:10input/lifecycle checks PASS,0exceptions/provider writes.
Canvas readback inside the render callback yielded62black images; these are
rejected as visual evidence. The compositor video is valid. Before frame review,
select compositor segment10.0–13.0s (25fps), covering enemy preparation, first
successful parry and disappearance before Restart transition. Review pending.

The right segment was extended to10.0–14.5s to include recovery and Pause/Restart,
not shortened:113/113recorded frames and112/112pairs directly inspected, preserving
25fps held/repeated frames. Cross-sheet pairs19?20,39?40,59?60,79?80,99?100 checked.
Frames0–46 cover reveal/idle;47–69 warning/preparation;70 raised parry;71–74
contact and thin fixed-horizontal flare;75–88 flare gone, old core/sparks recover;
89–100 Pause;101–112 Resume/Restart cover, no lingering line. Full-size contact
frames at12.80–13.04s also inspected. Warm center, tapered gold line and clear
vertical gap above typing tray; no random angle. Recorder25fps cannot independently
resolve20ms phases; exact envelope/expiry is established by Unity tests, not video.

Before reviewing mirrored output, select compositor segment10.0–14.5s from
native-left/cb36759848fab16eb5e4d3c59ae34658.webm, expected113frames/112pairs.
Covers complete enemy preparation, accepted real Space, new flare and physical-tail
recovery through pause. Mirror fixture changes initial actor positions/facing only,
outside repository/bridge; no direct success/spawn setters. Input/lifecycle10checks
PASS,0exceptions/provider writes. Left frame review completed below.

## Native visual acceptance

Left10.0–14.5s:113/113frames and112/112pairs directly inspected, including cross-sheet
pairs19?20,39?40,59?60,79?80,99?100.0–36reveal/idle;37–57warning/preparation;
58–60raised parry;61–64contact and stationary horizontal line;65–84existing flash
and particles recover;85–101idle with line absent;102–112Pause, no residue.
Both directions visibly clear the typing tray. Total226frames/224pairs, no sampling.
Selected recordings include held/repeated frames.25fps source limits sub-frame
claims; this is compositor-video coverage, not every internal Unity render.

Final right QA:20261027T151747-webgl, source1d67ca00b76d3ddf5f0e600c6aa557ea73a3ec016cc6ffe5279a1c8c0fb7c3d0.
Mirrored QA:20261027T152503-webgl, source1f9022b9b086f8de7552ae168fdc5b2e684f4b98819d31e4700f3ea421cac09f.
Both builds retain all strict builder checks. Native Edge reports10checks each,
0exceptions and0provider writes. First obsolete start helper clicked during scene
transition and timed out in the guide; corrected external driver waits for scene
settlement before actual Practice click. No game change for this harness issue.

Durable evidence/demos/adapters:
E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027.
right.mp4/left.mp4 preserve all113selected frames at25fps; original compositor
videos and source receipts retained separately. Black render-callback readbacks
are excluded. Canonical code/checkpoint remains in this repository.

Latest main score-frame layout is separate from impact/typing-tray/camera code;
merge reviewed and scoped tests rerun below.


Final mirrored review:113/113frames and112/112pairs inspected, same cross-sheet
transitions as right.0-36reveal/idle;37-57warning;58-60raised parry;61-64contact
and fixed-horizontal gold/white flare;65-84old flash/particles recover;85-101idle
with no line;102-112Pause. Both directions retain tray clearance.
Native Edge156.0.4314.42/Playwright1.58.2,640x360,10checks each/20total PASS,
0exceptions,0fixture/provider writes. Exported MP4s decode113frames each at25fps.

Merged latest main's score-frame change, preserving both verification sections.
The update affects score/combo blocks and read-only HUD observations; impact,
parry contact, camera and typing-tray geometry are unchanged. Native VFX results
are reused for these unchanged dependencies; integrated scoped Unity run PASS45/45,0skip.


Integrated result20261027T153449-quick, input01d4e7e4458646e5f85b990ff7f12bd2eff6cbf329aa323efe16c13691a0a670:
45/45ImpactVfxTests PASS,0skip. Only subsequent asset edits trim whitespace in the
two new meta files, preserving GUIDs/values; documentation edits do not affect play.
Final diff review preserves the independently merged score-frame behavior.
Remote integration verified: feature979f04a and evidence6f89335 are ancestors
oforigin/main after fast-forward push. Dirty parent checkout remained untouched.
Only clean task-created worktree/local branch and registered temporary files are
removed after this completion record; remote task branch remains. Durable demos,
source receipts, adapters and Unity XML remain in the artifact directory above.
Owned server result folders are removed after receipt preservation; shared source,
Library, toolchain, builds and unrelated task results remain. No itch.io release.
