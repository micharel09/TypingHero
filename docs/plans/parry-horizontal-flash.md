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
