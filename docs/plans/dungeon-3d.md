# CLACKBLADE - 3D dungeon arena

## Motivated torch lighting final (2026-10-21; scoped acceptance passed)

Supersedes the earlier visual acceptance below. User required identifiable sources
for all direct room lighting and selected one visible torch on the inner left
foreground pier. Existing dungeon-3d child/branch micharel09/dungeon-3d retained;
parent/main and unrelated dirty changes preserved. No release/push/merge.

### Implemented rendering contract

- Three world-space realtime point torches: two rear40/range15, foreground180/range24;
  warm RGB(1,.78,.56), bounded +/-5percent intensity and emission on the same phase.
  Positions are derived from actual flame geometry, not camera offsets:
  rear(-5.50,3.58,9.35)/(6.50,3.98,9.35), foreground(-6.97,3.50,-2.56), room-local.
- torch_mounted has no flame mesh. Reuse existing torch_lit FBX/texture bytes, with
  a fire-only emission data mask (UVx.875..1/y.25..5). Original wood/metal casts
  through a matching non-fire shadow mesh; fire does not occlude its own light.
  Stable holder/mesh nodes avoid FBX prefab-override drift on repeated builder runs.
- ShadowKey/CombatPool disabled; all overhead/torch shaft and glowing haze cards
  disabled, preserving their GUIDs/assets. Black ambient/fog, no GI or invented
  bounce light. Sparse32dust uses lit particles. Bloom lowered.24->.12 after native
  review; foreground source moved to the real pier corner to reveal enemy/floor.
- Additional shadow atlas2048; foreground512/face, rear and temporary pulse256;
  hard alpha-aware sprite/floor shadows, four local lights per mesh maximum.
  Hit/parry point light now originates at the actual contact (old offset0,2,-4
  removed). Pulse API, strongest-overlap rules, timing and 2D consumers unchanged.
- Camera5deg/FOV35, body center/scale, original pixel floor one mesh/eight groups,
 5824upward vertices, room coverage and existing animation/gameplay retained.

### Verification receipt

Final QA20261021T041655-webgl, Unity2022.3.25f1/URP14.0.11, strict repeated builder
PASS. Input SHA256 e1cf4516b7f5f3b21ddf4237ac5bc12a42a465febe59cdf2f9b19043c47fc4ce;
player17b60d80687e332f. Generated scene/material/mask/shadow mesh imported from the
matching immutable result; original mask/mesh/FBX GUID bindings checked.

Native Edge155.0.4283.62, NVIDIA GTX1650Ti ANGLE D3D11,1280x720:

- Actual-keyboard camera11/11: idle/attack/hit/parry, pause and Slayer entry/exit;
  body center(640,360), <=8px center / <=5percent scale, FOV35->20.9597->35 restore.
  Idle/attack/Slayer/restored images inspected; three visible flame sources,
  readable actors/stone joints, connected feet. No runtime exceptions or provider
  reads/writes; external blocking retained.
- Isolated source/camera review8/8, all nine native pan{-2,0,2}/FOV{25,35,45} PNGs
  inspected. Source remains world-fixed even when outside the view. Separate
  left/right/front-only cases illuminate101965/92932/649204pixels above blackout;
  all-off world ROI(200,180,1100,590) maximum RGB0. No residual ambient or beam glow.
  Actual foreground point light plus hidden blocker darkens8460actor-region pixels
  and192floor-only pixels (ROI610,440,660,458); both full-size images inspected.
  Moving the actual torch+light together changes pool/shadows consistently.
- Targeted Unity041500-quick:3passed/0failed, torch bounded flicker plus matching
  visible emission/pause/hide restore, strongest 2D pulse owner/pause cleanup,
  actual-contact 3D overlap/hide cleanup. Runtime code unchanged after these tests.
- Matched headless20s actual-input combat: before mean14.292/p9526.610ms,
  after mean19.660/p9526.860ms (normal samples531/559, p95+0.94percent). Event mix
  differs, so these means do not isolate lighting cost. Investigated with an
  identical paused-render state,3s warmup+8s sample:592samples each, before
  mean13.54081/p9513.510, after13.54087/p9513.560ms (p95+0.37percent). Same browser,
  GPU, viewport and flags. Combined room-visible combat+parry samples686each also
  meet p95 (26.610->26.860ms); their means14.466->20.318 and p99 values27.035->53.220
  vary, so this is scoped p95 evidence, not a blanket tail/mean regression claim.
  One windowed timing run produced1Hz RAF and was rejected;
  cause not established. RAF/vsync intervals are not GPU-only timings/FPS guarantees.
- Clip preview.mp4 SHA25665f26b5fa4dedb861ef30490d3c28d3e2c2c0c6a12c82f3c52f9d8d69734b365:
  439/439decoded/exported frames,25fps, timestamps.04..17.56s. Actually viewed
  consecutive frames0..29,150..179,300..329 (90frames; .04..1.20,6.04..7.20,
  12.04..13.20s), plus native29/165/315. No unexpected global bands/detached sources
  observed in those windows. Ordered sheets/native frames only; no realtime video
  playback/audio review or full439frame visual claim; lossy capture limits pixel
  conclusions. Nine lossless full-size pose PNGs supplement this motion sampling.

### Review and retained resources

- Preview http://127.0.0.1:8207/?v=17b60d80687e332f ; comparison /comparison.png,
  source isolation /sources.png, motion /preview.mp4. Loopback server session94022.
- Worktree C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-3d (existing Orca child,
  branch micharel09/dungeon-3d), retain until review/integration. Baseline8206 remains
  untouched. Final player/screens/essential reports and reusable scoped QA scripts:
  E:/CodexTemp/TypingHero/dungeon-3d/lighting-sources. Keep for current user review;
  remove after feedback/integration when no longer needed.
- Remote isolated source/cache /data/typinghero-game-loop/task-dungeon-3d and final
  result041655 plus targeted-unit041500 retained for continuation. Removed task-only
  failed035621/035816 (mounted asset has no flame),040017/040317 (prefab idempotency)
  and superseded040613 calibration after recording conclusions. Fixed by correct
  lit asset and stable direct mesh nodes; no check was weakened. Duplicate packages,
  raw videos and reviewed frame exports were removed; final clip/PNGs/essential
  reports retained for current review. Preview/index/comparison/sources/clip HTTP200,
  correct build header17b60d80687e332f, MP4 Range206/1024bytes verified.
- No broad Quick/stress/soak, extra viewports, pipeline migration, public API change,
  production build, commit, push, merge or publication.

## Dynamic 3D lighting final (2026-10-21; scoped acceptance passed)

Supersedes earlier lighting/floor visual acceptance. User selected readable dark
pixel dungeon with future horizontal pan/zoom. Reused the existing dungeon-3d child
and branch `micharel09/dungeon-3d`; parent/main dirty work preserved. No public API,
gameplay camera controls, dependency, bake, publication, push or merge added.

### Final implementation

- World-space realtime rig: main directional1.45; two warm torch spots22/range13,
  outer140/inner65; broad cool combat fill45/range24; retained actual hit/parry pulse.
  Trilight ambient and lower haze/vignette reveal floor joints and wall footings;
  exposure.35/contrast2 keeps sprite highlights readable. No realtime-GI claim.
- Main shadows1024; additional atlas1024 and Low512 tier on torch/pulse spots.
  Explicit per-light bias avoids URP's default pipeline bias. Sprite shader now
  samples additional-light shadow attenuation with the realtime shadow-mask overload.
- DungeonTorchLight varies intensity smoothly within+/-5percent, with independent
  phases, scaled-time pause freeze and phase-preserving Slayer hide/restore.
- Found992/4160 zero-normal ground vertices: normals had been generated before the
  seven variant submeshes. Recalculate after all eight; final5824 vertices have
  upward normals and finite position/normal/tangent/UV values. Original tile bytes,
  1WU scale, hash distribution, material/mesh GUIDs retained.
- Backing covers x[-26,26],z[-16,12],wall top13WU for reviewed pan+/-2/FOV45.
  Explicit frustum rays avoid depending on editor projection state. Only offscreen
  backing extends; door/piers/approved5deg/FOV35 camera and actor positions retained.
  Viewport and Slayer consumer source SHA256 unchanged from the floor baseline.

### Final build and verification

Pinned Unity2022.3.25f1/URP14.0.11/fly-server QA `20261021T031856-webgl` and strict
repeated builder PASS. Frozen input
`10d87f5ac7638af8f6bfff0c5d9a3015959a64c0967a11397a197e32106d86e3`;
player `1669679d609ca250`. Matching generated room/scene/settings imported.
Command: `python E:/CodexTemp/TypingHero/dungeon-3d/lighting/run_review.py webgl --browser-scenario parry-clash`.
This existing isolated-runner derivative builds only; native checks run separately.

Native Edge155.0.4283.62/GTX1650Ti/ANGLE D3D11,1280x720:
- Normal keyboard camera smoke11/11 PASS: idle/attack/real parries/Slayer and pause,
  center(640,360), scale within5percent, FOV35->20.9597->35 restored. Hit pulse
  observed enabled after real attack; room/torches/shadows restore after Slayer.
  Idle/attack/restored images reviewed: readable pixel joints and connected feet,
  no missing surfaces or prior black variant tiles. Intended Slayer silhouette kept.
- Isolated visual scene8/8 PASS: all nine pan(-2,0,+2)/FOV(25,35,45) poses captured
  and reviewed, including movement.17.5s clip sampled70frames at4fps and reviewed:
  no exposed room edges, unexpected bands/black patches or abrupt light loss.
- Hidden cube occluder demonstrably shadows both player and floor:8362 changed
  sprite-region pixels and392 floor-only-region pixels (absolute RGB delta>3).
  Unblocked/blocked images reviewed; this verifies shader shadow receiving, not
  merely light flags. No writable gameplay bridge or product camera control added.
- 20s matched timing script on fresh browser processes: baseline572 normal-combat
  frames,mean13.385ms/p9513.515ms; final565frames,mean13.339ms/p9513.545ms.
  Browser/vsync-paced frame intervals, not GPU-only cost, device baseline or60FPS
  guarantee. The earlier timing-after run overlapped clip encoding and is discarded.
All final native runs0JS/managed exceptions,0provider reads/writes, external blocking
retained. No broad Quick/difficulty matrix, other viewport or production build claim.

Targeted Unity `20261021T025539-quick`:2/2 executed,0failed/skipped, covering owned
render-target lifecycle/projection and torch amplitude/pause/hide/restore. Runtime
component code unchanged since this passing run. Static final mesh/float checks,
handwritten source whitespace and changed documentation checks PASS.

Earlier lighting players024809/025701/030443/031413 are superseded. Investigation
found the temporary variant runner wrote epoch-zero mtimes, allowing Unity to reuse
stale editor/script imports despite changed snapshot content. Preserve source mtimes
in the final runner; final generated5824-vertex mesh/full backing and updated QA
script confirm imports. The isolated camera replaces gameplay camera/HUD references;
its original HUD probe produced nonfinite screen coordinates. Native review alone
stops that unused read-only snapshot publisher on scene entry and uses scene console
samples; normal original-framework gameplay checks remain unmodified and error-free.

### Review and retained resources

- Preview: http://127.0.0.1:8206/?v=1669679d609ca250
- Before/after: http://127.0.0.1:8206/comparison.png?v=1669679d609ca250
- Pan/zoom clip: http://127.0.0.1:8206/preview.mp4?v=1669679d609ca250

Keep loopback session39035 and `E:/CodexTemp/TypingHero/dungeon-3d/lighting` final
player, requested comparison/clip, final stills and minimal drivers/receipts for user
review/reproduction. Earlier transfer archives, pilot player, raw videos/contact
sheets/debug captures and superseded reports removed after this written receipt.
Remove retained preview/listener when review no longer needs them; retain existing
child until verified integration. Current server result031856/source/cache retained
for the same review; task-owned lighting pilot results and targeted-test result
removed after conclusions. Prior unrelated task/server resources untouched.

## Pixel stone floor (2026-10-21; implementation and scoped QA passed)

Reused existing Orca child `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-3d`,
branch `micharel09/dungeon-3d`, parent original TypingHero/main; existing dirty work
preserved. No push, merge, publication or broad suite.

GroundSurface retains its mesh GUID, bounds and elevation; one mesh/eight submeshes
reuse Floor1..8 materials/GUIDs. Each quad is1x1WU. Coordinate hash selects floor_1
for75percent of hash buckets and floor_2..8 for the remaining25percent, without RNG
state. Representative40x26 sample:792base/248variants (23.85percent).
All eight materials use Stone tint(.69,.75,.81), metallic0/smoothness.02;
normal/metallic-smoothness/AO links are null. Original Point/no-mipmap/uncompressed
textures retained. Poly Haven files/material retained for comparison, unused on
visible floor. Camera, viewport and Slayer source SHA256 unchanged across import.

Build command: `python E:/CodexTemp/TypingHero/dungeon-3d/run_variant.py webgl`
(build-only existing isolated runner). Unity2022.3.25f1/fly-server QA022915 PASS,
strict repeated builder includes PixelDungeon assets and PASS. Snapshot
`9a0805df82ecc324ea182d35be1c95de458c2d3933c4dca252596787886d61c8`;
player `2d423e04f2fe5845`. Imported matching generated room/scene assets only.

Native Edge155.0.4283.62/GTX1650Ti/ANGLE D3D11,1280x720: camera smoke11/11 PASS;
center(640,360) in idle/attack/Slayer/restored, normal FOV35.000004,
Slayer20.959700, restored35.000004. Body scale remains within5percent of approved
baseline; pause/resume restores center/FOV. Actual hit/parries/Slayer exercised.
Separate same-player motion smoke4/4 PASS,24consecutive captures during repeated
real attacks reviewed for floor: no observed flickering stripes, exposed edges or
floating feet. Idle/attack/restored full images reviewed: recessed pixel joints
read in depth and match wall style; original dark light pools/shadows retained.
Slayer intentionally hides room then restores it. Both runs0exceptions/provider
reads/writes; external requests blocked. No performance or other viewport claim.

Preview http://127.0.0.1:8205/?v=2d423e04f2fe5845
Comparison http://127.0.0.1:8205/comparison.png?v=2d423e04f2fe5845
HTTP200 verified. Retain session11672 and
`E:/CodexTemp/TypingHero/dungeon-3d/floor-pixel` final player, before/after,
comparison, drivers/receipts for requested visual review. Temporary motion samples,
video, transfer archive/package removed after written conclusions. Remove retained
preview files/listener after review when no longer needed; retain existing child
until verified integration. User visual approval remains separate from scoped QA.

## Perspective floor final preview (2026-10-21; runtime verified, art review pending)

Final QA `20261021T020819-webgl` and strict repeated builder PASS on input
`fb86f8d2b930c819aa2b177b53dc8d02f0f0b9daff2c8f08c5f07d06422f90b6`.
Pinned Unity2022.3.25f1/fly-server; immutable player `cebc2b18feb29c36`.
Imported matching generated scene/arena assets; four material map references are
valid Texture2D bindings. The first build's Cubemap importer bug is corrected in
GroundTexture; GUIDs retained, no camera/lighting/gameplay retuning in this fix.

Native Edge155.0.4283.62/NVIDIA GTX1650Ti/ANGLE D3D11,1280x720:11/11checks PASS,
zero runtime exceptions/provider reads/writes, external network blocking retained.
Actual keyboard hit/parries/Slayer cycle verified. Body center(640,360) at idle,
attack, Slayer and restored; normal FOV35.000004, Slayer20.959700, restored35.000004.
Body height differs from prior centered orthographic baseline by+0.00493percent
(acceptance<=5percent); center tolerance8px passes, including pause/resume.

Reviewed full-size idle/attack/Slayer/restored images: texture now visible with
perspective recession, continuous horizontal ground, feet meet surface, no observed
exposed floor edges or wall/pier gaps at this viewport. Existing shadows/torch pools,
sharp actors and cinematic hiding/restoration retained. This is scoped runtime and
visual verification, not user art approval, other-viewports or performance evidence.

Preview: http://127.0.0.1:8204/?v=cebc2b18feb29c36
Before/after: http://127.0.0.1:8204/comparison.png?v=cebc2b18feb29c36
HTTP200 and preview build header verified. Keep loopback server session1367 and
`E:/CodexTemp/TypingHero/dungeon-3d/floor-perspective` final player, before/after,
rollback and QA receipt for requested review; remove after feedback/no further use.
Keep existing dungeon-3d child/branch until verified integration. No broad suite,
production build, push, merge or publication. Earlier offline/pending notes below
are historical and superseded by this final receipt.

## Online build / texture import correction (2026-10-21; final review in progress)

fly-server is reachable again. First QA015824/strict repeat builder PASS, input
446b68b49123897ebb1843b353366e3074c5f64ce699d926494f6467e377cbf3;
generated arena outputs imported from the matching isolated source. Native
Edge155/NVIDIA GTX1650Ti D3D11,1280x720:11camera checks PASS, zero exceptions or
provider reads/writes. Actual FOV35, center(640,360), pause and real hit/parries/
Slayer20.9597/restoration verified. Visual FAIL: floor is flat blue-gray.
Generated GroundStone.mat has all texture references null. Ground PNG importer
metadata uses textureShape2 (Cubemap), incompatible with Texture2D loads.

GroundTexture now explicitly selects TextureImporterShape.Texture2D and rejects
null texture loads. Corrective QA rebuild started; final runtime/visual acceptance
is pending. This rebuild is justified by the observed material failure, not wider
test coverage. Keep floor-perspective task-temp for final review and preview.
Earlier offline notes below are historical.

## Resume verification (2026-10-21; runtime still blocked)

Reused the existing Orca child `micharel09/dungeon-3d` at
`C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-3d`; parent checkout and
unrelated dirty files preserved. Re-read ground builder, arena camera, viewport,
Slayer lens consumer and prepared 1280x720 smoke driver. No new runtime/editor
source changes were needed from this static review.

Fresh checks PASS: all five SOURCE.json SHA256 entries, every map 1024x1024,
packed RGB metallic zero and alpha exactly 255-minus-roughness.R; camera smoke
and native driver JavaScript syntax; affected camera/ground tracked whitespace.
Independent lens math remains distance17.84022076 / Slayer FOV20.95969836.
These checks do not establish compilation, rendering or camera behavior.

Fresh SSH `ssh -o BatchMode=yes -o ConnectTimeout=12 fly-server pwd` failed:
`connect to host 100.115.189.84 port 22: Connection timed out`. Tailscale peer
`quanubutu` reports Online=false. Configured local Unity2022.3.25f1 executable
is absent. No builder or QA WebGL build started, no generated scene/material
outputs imported, and no new player URL or before/after acceptance claimed.
Retain the existing floor-perspective task-temp assets/drivers/rollback for the
same pending builder + one QA build + native 1280x720 review described below.
No wide suite, installation, push, merge or release performed.

## Perspective / PBR floor repair (2026-10-20; source ready, BUILD BLOCKED)

User rejected the stretched orthographic floor, chose the Reddit-like floor route,
and authorized this repair. Reference video is now retained locally; its25s frame
is the ground/composition target. Preserve5deg side view, full-screen body center,
actor scale and closed door; replace only the ground material/projection.

Source changes: perspective FOV35 at distance5.625/tan17.5=17.84022. Arena maps the
existing half-height scalar to FOV even while pause owns position, preserving
Slayer3.3 (FOV20.9597) and normal5.625 (FOV35). Virtual cameras use perspective.
Floor corners come from ground/frustum intersections plus a2WU margin; continuous
world-XZ UV/2 surface, tangent normals and.15WU backing below the existing foot
height. Legacy Floor1..8 are disabled, retained for rollback. Backing walls and
foreground piers fit the new frustum. Fog19.84022..41.84022, shadows36/clip.1..80.

Original Poly Haven Cobblestone Floor08/Rob Tuytel/CC0, four1K PNG maps under
`Assets/Art/Environment/PixelDungeon/StoneGround`, SOURCE.json records URLs/SHA256;
upstream MD5 verified. NormalGL/.65, AO/.7, metallic0, packed alpha=1-roughness;
floor-only trilinear/mipmap/aniso4. No AI art. Lighting/haze/sprites/gameplay retained.
QA adds actual camera projection/FOV to distinguish lens behavior from the legacy
zoom scalar. Existing centered-atmosphere receipt is historical, not this repair.

Local asset/packing checks and independent pinhole math PASS (body scale ratio
1.0000425, centerY360, bottom-ray floor Z-10.737). Driver JavaScript syntax and
affected tracked diff whitespace checks pass. These are NOT Unity/runtime proof.

BLOCKER: runner SSH to fly-server/100.115.189.84 times out; Tailscale reports peer
quanubutu Online=false. No remote build started or authoring outputs imported.
No configured local Unity2022.3.25f1 found; do not install a replacement toolchain.
SampleScene and generated material/mesh outputs are still the previous preview;
run the builder before judging this repair. Compilation/render/behavior/visual
acceptance remain unverified. No new preview URL or FPS claim.

Resume when fly-server is reachable: run the existing isolated build-only wrapper
once with webgl --browser-scenario parry-clash, fetch that exact result with
`floor-perspective/fetch_room.py`, then run native.cjs camera_smoke.cjs. The prepared
1280-only smoke checks actual perspective/FOV, scale within5%, body center8px,
hit/parry/pause/Slayer return. Review floor/foot contact and captures before linking
the prepared8204 server/comparison. No expanded suite or production build.

Keep `E:/CodexTemp/TypingHero/dungeon-3d/floor-perspective` (rollback, Poly Haven
metadata, static-checks.json, prepared driver/server/comparison) and same isolated
dungeon-3d branch until runtime/visual review; no integration or publication.
Previous8203 preview is retained but does NOT contain this repair.

## Centered cinematic atmosphere revision (2026-10-20; visual approval pending)

User clarified: the pasted dark-fantasy image is the minimum atmosphere reference,
not its top-down camera. Keep5deg/ortho5.625 and center both bodies on the full
screen, with floor extending from foreground toward the fighters. Use cinematic
lighting inspired by the Reddit prototype; retain sharp sprites and closed door.

CombatHUDLayout.BodyCenter uses stable collider bounds/inset head-foot landmarks,
excluding weapon/animation bounds. Arena camera follows this presentation center;
disable its old viewport floor/zoom anchor. All virtual cameras share the centered
pose and lens. Camera backs away72WU without enlarging actors; floor extends to
z-58. Near/far.1/150, shadow distance90, fog74..96 follow the deeper view.

Main key.95 plus one shadowless cool combat spot14, two warm torch pools9.5/range8.
Procedural soft-edged depth-tested mesh shafts and rear/low haze replace the thin
lines; original32cap dust retained. Bloom.24/threshold1.15/scatter.65, contrast6.
No new raster textures, asset packs, AI artwork or gameplay changes. Environment
group/arena PP remain owned by Slayer; particles retain scaled-time pause rules.

One QA build20261020T093538-webgl and strict repeat builder PASS, frozen input
`c22718f57414ea1fd1ed79cfdf5a8cdd83be4dd57c85676596ecc7ed1cb0fb4f`,
player`04c0bc4240ec373e`. Native Edge155.0.4283.62/NVIDIA GTX1650Ti ANGLE D3D11,
1280x720 only:9checks PASS,0exceptions/provider reads/writes. Body center measured
(640,360.00003) at idle, attack, Slayer3.3 and restored5.625; pause/resume also
passes the8px assertion. Actual typed hit/parries/Slayer entry-exit verified.
Idle/attack/red/restored captures reviewed: floor fills the foreground, broad
shafts/rear haze visible, body/weapon silhouettes remain legible. This is a visual
review candidate, not a claim of matching the reference or final art approval.

Preview: http://127.0.0.1:8203/?v=04c0bc4240ec373e
Comparison: http://127.0.0.1:8203/comparison.png?v=04c0bc4240ec373e
No expanded test suite, production build or FPS claim. Retain previous8202 and
`E:/CodexTemp/TypingHero/dungeon-3d/center-atmosphere` (rollback, source reference,
player, captures, center-smoke.json), server session14084 until visual feedback.
Same isolated dungeon-3d branch; no integration or publication.

## Wide camera / stronger depth revision (2026-10-20; visual approval pending)

User rejected the close zoom and chose to retain5deg pitch. Restore normal ortho
5.625 across camera/virtual cameras/viewport/Slayer restoration, keeping the
existing foot anchor. Slayer's cinematic lens remains3.3.

The rear architecture moves3.5WU deeper; the floor extends with it. Closed door
recess gains.5WU, jamb depth increases, and two angled foreground piers/feet now
frame the edges at x+/-9.4 without entering combat. Original tiles are retained.
Floor tint increases to(.78,.83,.9); key becomes intensity1.4 at(40,-40,0),
separating side/front faces. Torches/rays move with the wall, light range7.
Ambient/postprocessing, actor art, HUD, input and gameplay remain unchanged.

One QA build20261020T091716-webgl and strict repeat builder PASS, frozen input
`71741273eaf27d4678b7b99ef0292c606c352a199888f064d14c4e549f667bca`,
player`11e640408dd5282c`. Native1280x720 Edge155/NVIDIA GTX1650Ti D3D11:
7checks PASS,0exceptions/provider reads/writes. Real hit, pause/resume, parries,
Slayer3.3 and restoration5.625 verified. Idle/Slayer/restored captures reviewed:
foreground edges and floor strip are visible, door remains closed; artistic depth
approval is still pending. No extra suite, production build or FPS claim.

Preview: http://127.0.0.1:8202/?v=11e640408dd5282c
Comparison: http://127.0.0.1:8202/comparison.png?v=11e640408dd5282c
Keep previous8201 and `E:/CodexTemp/TypingHero/dungeon-3d/depth-wide` (rollback,
player, captures, wide-smoke.json), server session64654 until visual feedback.
No integration or publication.

## Lower/closer camera preview (2026-10-20; visual approval pending)

Camera pitch is now5deg, normal orthographic size3.75 (previous10deg/5.625),
with fighter midpoint centering and the existing follow limits. Feet remain near
viewport y0.2333. Cinemachine and viewport baselines match; floor correction now
accounts for pitch. Slayer settles at3.3 and restores3.75. Room, lighting, actor
art, HUD and gameplay are retained.

One QA build20261020T090056-webgl/strict repeat PASS, frozen input
`2bf2d1c919ede6439b90390e4c754f6e37a7c9410701adc4ece1b6db91815b69`,
player`043a3a3c8860ef9c`. Native Edge155/NVIDIA GTX1650Ti at1280x720:
7checks PASS for hit, pause/resume and real parries into/out of Slayer;
0exceptions/provider reads/writes. The first pilot sampled the entry zoom tween
too early; final driver waits for the lens to settle before asserting3.3.
Idle, attack, Slayer and restored captures reviewed. No extra suite, production
build or FPS acceptance for this camera revision.

Preview: http://127.0.0.1:8201/?v=043a3a3c8860ef9c
Comparison: http://127.0.0.1:8201/comparison.png?v=043a3a3c8860ef9c
Retain `E:/CodexTemp/TypingHero/dungeon-3d/camera-close` (rollback sources,
player, captures and camera-smoke.json) and loopback server session39176 until
visual feedback. Same dungeon-3d branch; no integration or publication.
The earlier8200 room preview remains available for comparison.

## Pixel room review revision (2026-10-20; preview ready, visual approval pending)

User rejected the KayKit/clay appearance and requested one fast playable preview
before further work. Use simple thick 3D architecture with original pixel textures,
following the 2D/3D separation described in Phuc's
[3D devlog](https://drawwithphuc.com/devlog/09-tu-2d-len-3d) and
[lighting devlog](https://drawwithphuc.com/devlog/08-anh-sang-2).
Latest correction: **door stays closed**; no open corridor is wanted.

Room-only revision in the existing child/branch; parent and gameplay remain intact.
Source is 0x72's DungeonTileset II v1.7, CC0, original hand-authored pixel PNGs.
The author itch page returned403 locally;20unchanged frames are downloaded from
`marceloferreira357/bun-crawler-client` at revision
`2a00fd56341cb2278bc66350d5f71f4abfadef32`; hashes/URLs/license are in
`Assets/Art/Environment/PixelDungeon/SOURCE.json`. No raster generation. Meshes
are built in Unity and persisted as assets; old KayKit architecture is retained
disabled for rollback, with only the small original torch meshes reused.

`DungeonPixelRoomBuild` owns the floor, wall thickness, jambs, segmented stone arch,
recessed closed door, edge piers and sparse decoration. Point/uncompressed/no-mip
individual texture tiles use UVs per face; no atlas bleed. Environment blur is0;
the scratch texture is consequently not allocated. Real-time key/torches use ambient
lighting; old lightmaps/probes/reflection are not used. Dust cap32. Existing sprite
shader, hit/parry light, camera, HUD/input/scoring and red Slayer are retained.

Preview scope: build and repeated serialization, inspect actual WebGL images,
real hit/parry, pause/resume, Slayer restoration, restart/menu resource release and
640x360/1280x720/400x300 framing. Deliver local versioned play link, matched image,
20-30sclip, then stop for user visual feedback. High-quality bake, additional rooms,
full performance acceptance and publishing are explicitly deferred.

Temporary review/rollback inputs:
`E:/CodexTemp/TypingHero/dungeon-3d/pixel-room` (including before-room source archive
and1408-filehashmanifest). Existing cinematic preview8198 is preserved as reference.
Current local visual preview (QA/read-only bridge, offline network guard):
http://127.0.0.1:8200/?v=7b5dce6def6e2b15 . Matched image:
http://127.0.0.1:8200/comparison.png?v=7b5dce6def6e2b15 . Clip:
http://127.0.0.1:8200/preview.mp4?v=7b5dce6def6e2b15 . The25s1280x720/H264/25fps
clip has625decoded frames and SHA256
`ea4bf63433d581aa4354b88ad737337ba889ce224a423d4899d60b721cf9f4a6`.
Reviewed chronological samples, full-size idle/hit/parry/Slayer/restoration and
the visible room at all three sizes. Closed wood door and solid stone arch are
visible. Earlier pilot door was hidden by its recess backing and the trim tile
was mostly transparent: corrected door depth and use fully opaque stone for trim.
The tiny existing F6 label belongs to the QA preview/clip.

Frozen input `1723719437a1623ea188ec8a534a9fa3026981e05c951882985b45bb7a0ac232`:
QA083448/strict repeat PASS, player`7b5dce6def6e2b15`. Native Edge155.0.4283.62,
NVIDIA GTX1650Ti/ANGLE D3D11:1742scoped assertions pass (includes per-frame checks),
real typed hit/score,6parries, Slayer entry/restoration, pulse/pause, three sizes,
popup projection, restart and menu release. One320x180world RT/no blur scratch,
one presenter; menu0/0, lightmaps/probes0, dust32cap.0exceptions/external writes.
Matched idle3checks and visible-room three-size9checks also pass. Default facing
reviewed; no new mirrored fixture/full animation or performance suite was run.
954runtime/art/prefab/package/project files are byte-identical to before-room
inputs; all1585original scene object/component IDs remain.20PNG hashes match the
pinned mirror. Final Unity-generated scene/material/mesh/settings files imported.

Normal production084219 builds from the SAME17237194input and passes build-time
QA exclusion; immutable player remains on the isolated server. It is not the
served preview and has not received a browser smoke. User explicitly requested
an early preview and fewer tests, so stop additional checks here. Do not claim
production/browser acceptance, complete mirror coverage, bake or60FPS acceptance.
Server8200 (`pixel-room/serve_qa.cjs`, exec session49259), child, final media/player,
rollback archive and isolated cache are retained for visual feedback; clean after
accepted integration. No publish/merge/push. Older receipts below refer to the
previous KayKit room. Remote working source now contains this canonical pixel room;
the historical shadow-off warning below no longer describes the current source.

## Cinematic implementation (2026-10-20; preview ready, acceptance pending)

The supplied plan is the implementation contract. Work remains isolated in
`C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-3d`, branch
`micharel09/dungeon-3d`, parent `C:/Users/ADMIN/orca/TypingHero`, base
`e20c4ab9db39471642a76a59c02328acd860e902`. Orca child ID:
`e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-3d`.
Existing palette/font/UI changes are retained. No push, merge or publication.
Visual approval is still required; do not mark all arena acceptance complete.

Implemented:

- Original KayKit Dungeon1.1 FREE/CC0 models and atlas; four room layers, original
  hinged door opened82degrees into an actual corridor, distinct stone/damp stone/
  wood/iron/flame materials. Nothing was painted or generated by AI.
- Key45degrees/intensity1.6,1024hard shadows/one cascade; two shadowless warm torches;
  one512spot shared by strongest hit/parry pulse, disabled at idle. Separate low
  Bloom/ACES/grading/vignette profile. Sparse dust48cap,36normally alive; faint rays.
- Explicit architecture bake:1lightmap/45light probes/64px HDR custom-bound baked
  cubemap. Small props use probes; final bake063534 has no UV overlap warnings.
  UV2 Calculate margin uses minimum instance scale/resolution16. Ordinary builders
  reuse bake, guard against missing LightingData and include it in strict repeats.
- Owned320x180 ARGBHalf/24bit-depth/Point/no mip/no MSAA world target and owned blur
  scratch. Environment-only depth-aware Gaussian spans at most.75world pixel;
  orthographic depth conversion handles reversedZ. Custom DungeonActor pass keeps
  sprite alpha/depth/shadow consistent, retains normals/flip/tint/hit flash; existing
  combat VFX follow it. One presentation camera draws the explicit world texture
  before native-resolution PP; HUD stays native. Obsolete Point pass removed.
- Shared world-to-real-screen mapping serves popup/tutorial/Slayer.10degree lens,
  +/-.5follow/.35s damping, existing zoom/impulse/pause ownership retained. Slayer
  hides room/lights and disables arena PP, then restores prior state. Restart/menu
  release owned targets/presenter. NullGfx does not receive an offscreen target,
  avoiding Unity2022's native DrawLineOrTrailFromNodeQueue crash during logic tests.

Frozen final input `2d9e567ef5c9eebfaa59265a434a2dcd33cbe82a5e25570ae288d0ac04bfd191`:
QA064123 builds and strict repeat passes; four-file player version`ce34aa070e61e490`.
Unity2022.3.25f1/URP14.0.11 remain pinned. Full Quick062336 passes121/121,0skip
on93aac923(runtime identical; later builder lighting/bake change). Native Edge155
D3D11 final2112checks pass: real hit/scoring,6parries,2Slayer cycles, mask15(normal
and all3Slayer attacks), live-pulse pause,640x360/1280x720/400x300, popup projection,
hurt/death,3restarts, actual180->200HP opponent transition with carry, menuRT0/
presenter0.1map/45probes/2owned RTs in arena, stable through resize/pause/Slayer;
new targets per restart, no growth.0exceptions;1fixture read/1intercepted write.
Final unchanged source tutorial driver94checks/six lessons/retries/pause/restart/
three Slayer sizes passes,0exceptions/0writes.

Resize pilot captured Unity's first frame before Canvas layout rebuilt:400x300
briefly retained prior1280x720corners. Diagnostic9checks shows correct16:9rect and
popup after layout settles(<120ms observed). Final driver waits three new frames;
assertions/tolerances are unchanged. No gameplay or HUD workaround added.

Source preservation:12/12KayKit original FBX/PNG hashes match SOURCE.json;
290locked actor/skeleton/art/character-prefab/UI-prefab/package/project files match
pre-edit bytes, including approved player palette and animation metadata. Credits
and KayKit CC0 license are included in build. Generated Unity YAML contains normal
empty-field trailing spaces; handwritten C#/shader/docs diff whitespace checks pass.
Affected documentation links resolve. Existing unrelated font-release marketing
receipt link is absent in this child and was left unchanged.

Production070014 uses the same frozen final input, passes qa=false and ASCII/UTF16
bridge-marker exclusion across JS/WASM/data. Four-file version`7fc758df40a42902`;
output.data14,135,985bytes SHA256
`5baae0f2403ed0ab76127ae4c00f04ad757fbf337930b5dc5de660b05a3a2923`.
Original-HTML Edge155 real keyboard/mouse: menu, tutorial BLADE attack850->840HP/
10points, pause/resume and lit actors reviewed; bridge undefined,0exceptions/
external writes. Same user Orca tab23271c53-94bf-4fe5-961b-e9ba9ea0723f receives
all4versioned URLs; fetched exact data hash FROM the tab. MainMenu->tutorial room
is visually reviewed there too. Preview is offline; provider connections are blocked.

Mirror fixture064558 inputb7ecec77/version1b320b515975f5cb passes453checks/mask15.
Original sprites, flips/normals, alpha depth/shadows, normal attack, three Slayer
attacks and restoration reviewed. Only SpriteRenderer.flipX is changed in the
fixture archive; original physics/anchors remain. This is visual flip verification,
not a new mirrored game mode. No variant changes were imported into local source.

Review artifacts are ready:

- Play: http://127.0.0.1:8198/?v=7fc758df40a42902
- Matched1280x720/ortho5.625/10degree before-after:
  http://127.0.0.1:8198/comparison.png?v=7fc758df40a42902
- 48s1280x720/H264/25fps clip:
  http://127.0.0.1:8198/preview.mp4?v=7fc758df40a42902

The clip uses final QAce34(native input, existing small QA label); playable preview
is production7fc with no debug bridge/label. Clip SHA256
`ea1fc0ec02b08361382bc4886f254e40afb9c4a0b567621f17f663a742fcd0e9`.
All1200video frames decode. Reviewed96chronological samples plus full-size hit/
parry/mirror frames and worst-floor frames from an all-frame scan; no old random
black rectangles or loss of actor color observed in ordinary arena. Red/black Slayer
and brief existing hit flash are intentional. Frame scan643ordinary samples,
maxfloor-black fraction.02786; this prioritizes visual review and is not semantic
proof. Final demo has35HP/378score/10parries/mask15 and0exceptions. Resize recording
pilot with gray padding is superseded; final clip stays1280x720. Three actual sizes
are separately verified by arena/tutorial drivers.

### Performance evidence and remaining acceptance

Real headed Edge155/NVIDIA GTX1650Ti/ANGLE D3D11,1280x720/75Hz. A first final
sample has overall RAFp50/p95/p99=13.340/13.695/26.755ms, but individual phases
must be inspected; it does NOT qualify the60FPS target:

| Final headed phase | RAFp50 | RAFp95 | RAFp99 (ms) |
| --- | --- | --- | --- |
| Idle intro |13.355|26.735|27.500|
| Normal combat |13.340|13.620|26.700|
| Parry |13.345|26.615|26.785|
| Slayer |13.340|13.515|13.690|

Earlier baseline overall13.340/13.595/26.640ms. Actual match must be repeated:
later attempted A-B-B-A yields A1overall13.340/13.770/26.760 and B1
13.350/26.750/40.120; B2 browser/context was closed and A2 did not run. A separate
pilot throttled to~1Hz despite visible/focused DOM. Native occlusion/focus/power
is suspected, not established as the cause. Equal background-throttling flags,
foreground requests and temporary display-awake request did not produce a complete
ABBA. Display request was released when the helper ended. Do not infer a source
regression or stable60FPS from these inconsistent desktop samples.

Attempted same-tab Orca benchmark also stays at Unity splash/no QA state with
instrumentation; main production tab loads correctly. Three startup/focus/isolated
header variants failed before measurement. This is an unusable measurement harness,
not a gameplay PASS or a reason to weaken acceptance. Its task-only tab/server8199
were closed; production tab restored. User was asked asynchronously whether Edge
windows were being closed; no answer received by checkpoint. Resume desktop timing
when a visible foreground browser can stay open for the matched scenario, then
profile any>10%p95change by pass before changing visuals.

Shadow diagnostic070719 input77a59de8/version190291adf65a62fe changes ONLY actual
key/combat light shadows toNone, leaving pipeline capabilities/bake unchanged.
Headless Edge155/D3D11 on the same NVIDIA device, same scenario and viewport:

| Headless diagnostic | RAFp50 | RAFp95 | RAFp99 (ms) |
| --- | --- | --- | --- |
| Shadow on |13.340|13.620|26.735|
| Shadow off |13.340|13.510|13.690|

This is regression evidence only, not desktop FPS acceptance. Shadow-off reduces
long-tail intervals; paired p95difference.110ms(~.8%) is below10%. On normal combat
p95on/off13.550/13.510; parry13.585/13.505. An initial shadow-pipeline-disabled
fixture065158 stalled in the editor after MainMenu and was explicitly stopped
(code143), not PASS; final diagnostic leaves pipeline support identical.

Unity allocated memory (not total browser/VRAM): baseline~41.84-42.27MiB;
final~47.89-48.25MiB; menu~40.5MiB. Owned RT count2through warm pause/Slayer/resize,
new IDs per restart,0at menu; no growth observed across3restarts. Internal URP pools
were not separately profiled. Browser RAF is the main timing series; Unity frameMs
is a10Hz observation and must not be presented as independent per-frame samples.
No FPS conclusion is drawn from headless tests or the25fps review recording.

### Gate and retained resources

Milestones1-4 are implemented and functionally verified. Milestone5preview is
self-reviewed and ready for user visual approval. Milestone6is NOT complete:
visual feedback/approval, stable desktop ABBA/60FPS acceptance and any resulting
optimization remain, followed by authorized integration. No itch release requested.
Do not merge/push/publish or mark the entire task Done from current evidence.

Retain child/branch and `E:/CodexTemp/TypingHero/dungeon-3d` until review and remaining
performance work are complete. Main/palette/font/UI changes are preserved. Baseline
040739 data SHA256a097ecde24d2eb5add5224b91c9f0853b0163de8a321e10dda91ebc86b212f1b;
pre-edit archive SHA124bd860313d9e2603232cb5eac19636a7b31556fba88e6becdeb5922384cb5d.
Current canonical QA/player, production, before-after/clip, essential reports and
native/Orca measurement scripts live under task temp/cinematic. Static production
server73678(port8198) stays for review. Benchmark tab/server8199and owned test jobs
are closed. Duplicate archives/production copy, obsolete gray-padding video and
extracted contact-sheet frames were removed after verification; final review media,
key images and essential reports remain. No blanket temp/cache cleanup was used.

Remote task root `/data/typinghero-game-loop/task-dungeon-3d` is isolated. Keep
baseline040739, Quick062336, bake063534, QA064123, mirror064558, production070014 and
shadow070719 results for continuation; 30superseded owned result folders were removed after written receipts
(943MiB->266MiB). Original caches/toolchain and other active runner roots were preserved. IMPORTANT: remote working source currently contains the
shadow-off fixture. Never import it into local source. Rebuild from the canonical
local snapshot; immutable final QA/production results have the final normal lights.
Keep shared toolchain/home symlinks; keep owned source/library/cache until remaining
acceptance/integration, then clean only verified task resources. Historical ports/
capture references below are superseded by this current resource list.

Technical sources checked: [URP14 camera RT](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/rendering-to-a-render-texture.html),
[URP14 Bloom](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/post-processing-bloom.html),
[Unity2022 LightingSettings](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/LightingSettings.html)
and installed14.0.11 Blitter/Blit/UniversalRenderer sources. Independent material
property block fixes URP14 Blitter retaining the grading LUT during presentation;
first045814 rainbow-LUT pilot is superseded by final native proof. Initial0maps/
0probes pilot exposed blanket Git ignores; narrow SampleScene exceptions now retain
authored lighting inputs. Earlier timing/NullGfx/software failures were diagnosed,
without weakening thresholds; only Quick062336 is the full passing final receipt.
Earlier sections below preserve history and do not supersede this checkpoint.

## Intent and references (2026-10-20)

Approved scope: one KayKit Dungeon 1.1 FREE dark fantasy arena, existing sprite actors,
320?180 world, separate HUD, 3D combat lighting and alpha silhouette shadows. Preserve
input/damage/scoring, tutorial, pause, Slayer red cinematic and opponent lifecycle.
No forest/menu redesign, generated raster/model, dependency change or publication.

Primary camera reference supplied by user: https://www.instagram.com/reels/Ddolq0zO0Ef/
Viewed playing and at 3s/12s in Orca. Side-on combat, shallow downward view showing
floor, continuous battle line, rear wall receiving actor shadows. This supersedes
the earlier generic camera reference; exact projection is inferred, not source camera metadata.
Earlier supplied image informs layered architecture and dark framing at the edges.

KayKit source: https://kaylousberg.itch.io/kaykit-dungeon-pack (CC0, FREE 1.1).
Direct itch download blocked by Cloudflare; original Unity FBX/default atlas subset
retrieved from pinned public mirror, provenance and SHA256 in asset SOURCE.json.
No EXTRA assets selected. Original model/texture bytes retained.

## Workspace and baseline

Parent `C:/Users/ADMIN/orca/TypingHero`, main e20c4ab9, origin micharel09/TypingHero.
Child Orca id `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-3d`,
branch `micharel09/dungeon-3d`, explicit parent above. Existing dirty Assets/docs and
approved palette/font source copied as verification baseline; they are not owned by this task.
Target integration main after acceptance. No push/PR/deploy requested.

## Implementation checkpoint

- Added final combat-only arena builder after existing UI passes; menu renderer retained.
- Added Forward renderer slot, original KayKit meshes and stone material, sprite 3D shader,
  directional hard shadows, warm torches, shared strongest combat pulse and camera follow.
- Build/runtime/visual acceptance NOT YET VERIFIED.
- Shared runner doctor immediately busy, then 60s lock timeout; other running jobs preserved.
- Temporary reference captures: `E:/CodexTemp/TypingHero/dungeon-3d`; retain until visual QA,
  then record conclusions and clean. Child retained until verified/integrated.

## Required acceptance still open

Representative actual hit/parry render; visual review of room/sprites/normal/alpha shadows;
idle/attack/parry/mirror/death; pause/resume; Slayer enter/exit; restart/scene/opponent transition;
640?360,1280?720,400?300; matched baseline WebGL frame/shadow cost and actual QA-device FPS.

## Latest user steering

User requested a playable preview before further acceptance/integration. Build the
isolated child candidate, visually inspect representative combat, expose a local
preview with external writes blocked, then stop for visual feedback. Full lifecycle
and matched performance acceptance remain required after the visual direction is accepted.

## Preview verification / blur correction

Isolated runner root `/data/typinghero-game-loop/task-dungeon-3d` reuses shared
Unity toolchain through symlinks, with its own Library, source and lock. Shared jobs untouched.
Build 20261020T031038-webgl passed Unity compilation and strict repeat serialization;
input SHA256 745ee5b1b29f777b0e0352a5ef124c2fee125e14caeb8a65ba3bb0bc5779975a.
Original KayKit atlas was incorrectly imported as Cubemap; corrected to Texture2D,
Point, no mipmaps and verified material binding. Original asset bytes preserved.
Native Edge 155 D3D11 real keyboard: hit enemy 180 to170, score10; successful parry;
pause/resume and captures at640x360,400x300,1280x720, no observed exceptions.
These are preview checks, not complete lifecycle or performance acceptance.
Software browser parry-clash profile fails at encounter intro; its driver does not
handle current encounterReading and expects older enemy HP. Do not claim profile passed.

User rejected blur. Confirmed URP14 needs FinalPost for nearest upscaling; camera
postprocessing alone was insufficient because new renderer postProcessData was null.
Added explicit package PostProcessData binding; rebuilding and pixel verification pending.
Keep current brightness while correcting blur; dark fantasy polish awaits visual feedback.
Local offline preview http://127.0.0.1:8197 blocks external requests including writes.
Retain child and `E:/CodexTemp/TypingHero/dungeon-3d` player/server for user review;
clean after review/integration. Server root retained for next build, do not follow
its shared toolchain/home symlinks when eventually cleaning.

Build 20261020T031549-webgl also compiled/repeated successfully but native capture
still showed soft edges; do not label blur corrected. Renderer PostProcessData is
now bound and persisted. Added explicit serialized pipeline ownership to arena camera
instead of relying on currentRenderPipeline during OnEnable, and read-only
worldRenderScale to automation probe so next run proves actual resolution.

Build 20261020T032045-webgl, input833c1ac42c80c496a2708d5215a06931acee68104c575f31c58a4efc0ca7f6c1,
passed build and native hit/parry/pause/viewport checks with zero exceptions.
Read-only probe confirmed worldRenderScale0.25 at1280x720, yet image still interpolated.
Added DungeonPointBlit renderer pass to set final source RT FilterMode.Point before
URP FinalBlit (which selects sampler from RT independently of asset selection).
Added existing Core.Runtime assembly reference after concrete RTHandle compiler error.
Current candidate032933-webgl SHA25692ab5e74b039c618945c8266552d461111f4930bbdedca6b4b7a159abc83c6c5
awaits native visual verification. No full performance/lifecycle claim.

Rejected032933 runtime RT-filter mutation: world becomes black on WebGL despite
hit/parry state and no logged exceptions. Removed feature and its Core assembly
reference. Current approach uses a small arena-only coreBlitPS replacement whose
named final-blit passes sample sampler_PointClamp, preserving color conversion;
no runtime RT mutation. Camera postprocessing disabled again. Candidate033750 input
319a7721ad8b5dca33c028c414b00b228bf7f975912dd06c5d6432dc309cac1a awaits visual verification.

## Current reviewable preview (2026-10-20)

Latest build034610-webgl input4506c242af8d4fd232d0dbeeab71e5a46d721d8e41684857dbd0724269dea05f
passed Unity build/repeat serialization. Rejected custom coreBlit shader033750:
WebGL still soft and missing BLIT_DECODE_HDR keyword; removed and restored package shader.
Final Point pass sets RT filter in OnCameraSetup BEFORE drawing, never after contents
are rendered. Native Edge155 D3D11 capture now sharp: all14250 sampled world4x4
blocks at1280x720 are uniform (previous about4970/14250). Direct WebGL viewport
instrumentation confirmed320x180 world and1280x720 screen. Native keyboard hit180->170,
score10, successful parry, pause/resume,640x360,400x300,1280x720, no observed exceptions.
This verifies nearest upscaling, not final art acceptance. Hard shadow patterns/lighting
need visual polish; sprite source filtering/animation quality, full lifecycle and
matched WebGL performance remain unverified. Do not claim60FPS or completed arena.
User requested preview first; stop for feedback with offline link127.0.0.1:8197.
Retain current player/server, source child and isolated server cache for continuation;
parent untouched, no push/integration. Clean after accepted integration per resources rules.

## Preview cache correction

User screenshot remained blurry because embedded Orca tab loaded old build bytes:
output.data12710596 bytes SHA256fa07dbd0611ae692fcb78a0b5f7d247987b4064671c925ef45db890daee2a267,
whereas current file12760661 bytes SHA2569cd5c76e444a3c4a7b2aff69a091bf7a187672fd973121a74a725da6e0a3565c.
Version query bypass proved current bytes available. Updated temporary preview server:
all four loader/framework/wasm/data URLs use data hash version9cd5c76e444a3c4a;
all responses Cache-Control:no-store,no-cache,must-revalidate, X-Preview-Build header.
Node syntax and HTTP assertions passed. Navigated SAME Orca tab23271c53-94bf-4fe5-961b-e9ba9ea0723f
to http://127.0.0.1:8197/?v=9cd5c76e444a3c4a. Verified four versioned resources and exact
new data SHA256 from tab fetch. Actual Enter input opens tutorial; screenshot from this
same tab confirms crisp world/sprite outlines, HUD, room and actors visible.
Earlier QA-browser-only assertion did not prove user tab received new build.
Retain server/player/screenshots for requested visual review; no Unity rebuild needed.

## Black-frame correction in progress

User rejected version9cd5c76e444a3c4a: severe black actor silhouettes and random floor/wall
patches during combat. Previous visual acceptance was wrong; pixel-block uniformity and
zero exceptions did not prove correct depth/shadow rendering.
Compared URP14.0.11 ScriptableRenderer.SetRenderPassAttachments with ScriptableRenderPass:
ConfigureTarget(color) overrides target but keeps default backbuffer depth. First camera
color bind consumes first-depth-clear flag, so actual camera depth RT can remain uncleared.
Changed PointPass.OnCameraSetup to ConfigureTarget(color,cameraDepthTargetHandle).
Builder now removes null renderer-feature entries, reuses orphaned owned Point feature,
restores package CoreBlit and explicitly saves renderer. Generated forward renderer in
candidate040739 has valid featureID-2937895248212078226 and CoreBlit GUID93446b5c5339d4f00b85c159e1159b7c.
Candidate input0b2797e50030e04372809925a7ac8383e9ece45900b095a7fbf5c3b10e376fae.
No brightness/bias/camera changes made; awaiting runtime A/B and 60s acceptance.
Added read-only on-demand DOM relay to temporary preview HTML for Orca isolated eval
(document preview-state-request -> existing typingHeroQA snapshot). No gameplay mutation.
Primary references reviewed: Unity2022.3 Manual/SL-SamplerStates.html (inline sampler
support differs by graphics API) and URP14 ScriptableRenderPass API (color/depth overload).

## Verified correction / current preview

Build040739 strict repeat passed on the frozen input above. Imported generated product
subset from its unitypackage: arena scene/settings, KayKit materials/import metadata,
Rendering scripts/metas. Forward has valid Point feature and standard package CoreBlit,
no deleted custom shader. No brightness/bias/camera changes were necessary.

Reviewed31 native combat captures and idle/parry/hit/death/restart/three-size images.
Native video5902a8a81c56f44eecf894d9f331b7a5.webm sampled300frames at5fps across60s:
156 ordinary arena frames,144 intentional red cinematic frames. Reviewed worst-floor
frames: maximum ordinary floor-black fraction0.0261, minimum bright actor pixels508.
Same idle region changed0.2917 black before to0.0139 after, supporting the visual finding
rather than replacing visual review. No prior random black patches or all-black actors
in ordinary arena frames observed. SlayerSilhouetteGroup explicitly applies Color.black
on red cinematic; repeated enter/exit cycles restore original colors and arena lights.
Native13parries/60s; pause/resume and all3sizes inspected. Final automated R was too early,
so no whole custom-driver PASS; separate actual Restart-button check passed40/180HP,
score0/intro. Standard software profile also still fails at existing encounter intro.

Actual user Orca page23271c53-94bf-4fe5-961b-e9ba9ea0723f:64.516s keyboard/mouse,
13captures reviewed, score50, zero runtime errors. Subsequent visible Game Over Restart
restored40HP/180enemyHP/score0/intro. User tab now at Fight/Enter. Exact bytes12761468,
SHA256a097ecde24d2eb5add5224b91c9f0853b0163de8a321e10dda91ebc86b212f1b fetched FROM this tab;
all4WebGL URLs versioned, HTTP no-store. Current link:
http://127.0.0.1:8197/?v=a097ecde24d2eb5a

Retain E:/CodexTemp/TypingHero/dungeon-3d player/server (session15505), before/after,
video/receipts and isolated child/server cache for user visual review and ongoing arena
work. Clean temporary captures after review, child/cache after accepted integration;
do not follow shared toolchain/home symlinks. Parent untouched, no push/publish/merge.
Full matched WebGL frame/shadow cost, complete mirror/animation and opponent-transition
matrix remain outside this correction's verified scope. Earlier visual acceptance
statements are superseded by this section. No60FPS/performance acceptance claimed.

## Full dungeon lighting implementation â€” 2026-10-21 (in progress)

Source intent: implement all groups from pixel-25d-lighting in normal combat,
retaining the accepted room, camera, lantern, gameplay and latest UI. No release.
Base origin/main: 3669597f2cbc9bcf92d9fcd9b767a2273c7f4597.
Orca task branch: micharel09/dungeon-full-lighting; independent checkout
C:/Users/ADMIN/orca/workspaces/TypingHero/dungeon-full-lighting. Parent WIP untouched.
Evidence/helpers: E:/CodexTemp/TypingHero/dungeon-full-lighting.

Acceptance checklist (all still require final visual/runtime acceptance):
- Hero and every active opponent animation: aligned body normals, material masks,
  four white-light directions, mapped/flat A/B and both facings; every frame and transition.
- Alpha-aware depth/shadow, floor contact, material response and bloom.
- Actor focus with near/far environment DOF; haze, dust and source-aligned beams.
- Clipped planar reflection of moving actors and source lights on wet stone.
- Sparse background rain/drips, pinned hanging cloth, UV/noise emissive rune
  synchronized with its local light; accepted lantern settings retained.
- Pause, Slayer hide/restore, attack/hit/parry, menu return and render-target cleanup.
- Bright/dark backgrounds; nine camera pan/FOV poses and complete transitions.
- Same-pose/time/exposure/resolution effect A/B and full combination; matched timing.
- Builder twice, GUID/reference stability, scoped lifecycle tests, WebGL QA.

Implementation route: existing URP forward sprite pass with alpha-clipped depth and
shadow; companion maps share original sprite UVs. The material map is a palette-based
DRAFT, not approved body/material artwork. Actor normal cross-section fitting is also
unverified artwork until the required per-frame lighting review. No silhouette,
pivot, slicing, authored timing or albedo edits are intended.

New environment components own their private cloth mesh and reflection camera/texture.
Environment remains under DungeonArena3D, the existing Slayer activation boundary.
Planar reflection uses a horizontal plane, excludes its receiver, clips below the
surface, guards recursion and restores culling in finally. It cannot represent
nonplanar stone geometry. Reflection captures half the 320x180 world resolution;
this is a proposed budget, not an observed performance acceptance.

Checks so far:
- Initial serialize input 2575ef49 failed protected font byte check solely because
  Windows checkout CRLF became Unity LF. Normalized CobblestoneUI64 content hash
  937cdeb37c66fd87164ef7f8f6482ef27cdcd758fde48160badeb59f5dd2f328 matches.
  Normalized YAML line endings in this isolated checkout; no font change accepted.
- Serialize input 2a4039c7 compiled, then strict repeat found DungeonSprite metallic
  0 -> 1 between first/second outputs. Added immediate material asset save; recheck pending.
- Scoped DungeonRenderTests input d841635e: 4/4 PASS on Unity2022.3.25f1/fly-server.
  Covers pinned top seam, private mesh restore, shared clock pause/hide/resume,
  particle pause/resume, accepted fixed lamp, bounded rear flicker and world RT lifecycle.
  Does not establish GPU reflection rendering, visual quality or performance.
- Final build, generated assets, complete frame review, native QA/performance,
  commit/push/integration are pending. Do not report this task complete.


Implementation checkpoint update:
- Native candidate1 (a7f1bf14 input; player 1ace306beec40dbf): 12 scoped checks PASS,
  Edge155 / Intel UHD / ANGLE D3D11. Real attack, received hit, parry, pause,
  Slayer and restore, then menu return; zero exceptions/provider reads/writes.
  This player predates the reflection renderer correction and final palette mask;
  it is not full-effect visual acceptance. after/attack/pause/parry/Slayer/restore
  images and the complete41.76s recording remain in the task evidence directory.
- Direct inspection of the installed URP14.0.11 RenderSingleCamera source confirms
  it only retrieves additional camera data for Game cameras. The Reflection camera
  type selected the pipeline's default2D renderer. Replaced it with an owned Game
  camera plus DungeonCaptureCamera marker for the actor pass. Final native proof pending.
- Baseline HEAD-only snapshot492cb203 built with strict repeat PASS, preserving
  player at results/20261021T131938-webgl/player and local baseline-player.
- Complete authored-key inventory:28sprite clips/247keys, including held/repeated
  keys and original times, at clip-scope.json. Export count is not visual review.
- Updated full scene audit includes stripped prefab records:1866existing records,
  0removed,21new. Six existing records change (four atmosphere renderer enable flags,
  arena component list and child references). All other existing records retain their
  original content. New records are appended to avoid unrelated serializer reordering.
- Material draft now excludes the hero's cream scarf from steel, and grows skeleton
  steel from the source's neutral204palette through adjacent neutral highlights,
  stopping at pale blue bone/brown grips. Per-frame artwork acceptance remains pending.


Latest checkpoint (14:05UTC; integration pending):
- Extended the wet receiver to cover the visible foreground. Native screenshots now
  directly show hero/enemy/light reflections below the combat lane. Camera/floor/
  accepted lantern geometry and all old Light records remain unchanged.
- Candidate4 input44b8190a/player98362c163dc45cf3 (result134641): native15checks PASS,
  zero exceptions/provider reads/writes. Read-only observer proves the actual capture
  renderer is UniversalRenderer; Slayer releases the private planar target and menu
  return leaves zero owned world/planar targets and zero capture cameras.
  The single-line reflection PNG was truncated at Unity's8191-character log limit;
  that invalid PNG is NOT visual evidence. Next fixture emits2000-character chunks.
- Particle lifecycle regression was found by stronger tests: child particle data is
  cleared before parent OnDisable, and restoring during parent OnEnable is too early.
  DungeonEnvironmentMotion now retains bounded particles/playback state in LateUpdate
  and restores after child callbacks, in Update (or LateUpdate if enabled late).
  No trails are enabled in these systems. Relevant API guidance:
  https://docs.unity3d.com/kr/2022.2/ScriptReference/ParticleSystem.GetPlaybackState.html
  Actual Unity2022.3.25f1 scoped run140215/input44c7b805:4/4PASS, including count,
  time, exact position and remaining lifetime after pause/hide/restore. Earlier
  extended runs133629/134534/135711 failed and are retained, not acceptance.
- Old frame fixture's3captures are empty intro screens and do not count as visual
  review. Revised fixture directly loads byte-identical source sprites from temporary
  Resource copies with original slicing/pivot and aligned companion maps. Manifest
  resolves247keys from28atlases by original GUID/fileID/name. Four white directions,
  mapped/flat and two facings occupy16cells. A4x-native diagnostic viewport and full
  authored-timing/quarter-speed modes are prepared; actual inspection remains pending.
- Old A/B fixture threw on inactive torch Apply; its captures are not acceptance.
  Fixture now filters inactive torches and disables UI graphics after regular UI updates.
- Baseline134911 was intentionally canceled and its exact owned container stopped;
  do not use that partial run. Other projects/containers were inspected but untouched.
  Build a latest-UI baseline from origin/main for matched performance after final build.
- Source includes upstream UI patch from origin/main (edfc3b6), preserved separately as
  upstream-ui.patch/upstream-scene.patch in task evidence. These incoming changes must
  be excluded from the task commit, then merged through Git before remote integration.
- Latest final review build is running through E:/CodexTemp/TypingHero/dungeon-full-lighting/review-run.py.
  Pending: final generated artifacts/source PNG metas, native/diagnostic A/B, complete
  frame/transition evidence, camera video review and matched draw/frame samples;
  then memory updates, scoped diff checks, commit/push/merge/remote proof and cleanup.


Latest verified progress:
- WebGL66189fcb first failed g_FieldOffsetTable linkage; rebuilding only generated
  library/Bee/artifacts/WebGL under the runner lock then passed on the SAME input,
  result144147/playera0ec514d0a4c27b3. Import cache/source/toolchain unchanged.
  Exact input retained as input-66189fcb...tgz in task evidence. No Unity/package upgrade.
- Corrected frame fixture now displays real16-cell matrices. Directly inspected
  provisional key0 at4x: low forward stance, scarf/blade attachment intact, differing
  mapped/flat shoulder response and correct mirrored silhouettes, no clipping.
  Keys1/2 were captured but not inspected; no full-frame acceptance yet.
- A/B fixture failure is isolated to cloning a LIVE deformed cloth mesh: disabling
  the source destroys the shared runtime mesh. Revised fixture reinitializes its
  cloth from the source's persistent authored mesh before hiding the source.
- Atlas audit found hero death2944x128 with albedo default platform4096 while companion
  importers defaulted2048. Editor generation now raises companion max size to fit
  source dimensions, preserving original color atlas/slicing/pivot/timing.
- Preliminary matched non-video AB (before final companion importer update): native
  Edge155/IntelUHD/D3D11, same1280x720/input driver. Combat RAF p95 A33.245ms/B33.210ms,
  n240/242; sampled engine p95 A33.785/B33.175ms. Mean combat draws266.94/331.15,
  triangles12509/14356, measured through identical WebGL wrappers. Both4checks PASS,
  zero exceptions/provider reads/writes. Not GPU timings or final ABBA acceptance.
- Latest baseline built142534/input56fdef1c/player3e0bacb44182773e, based on
  origin/main edfc3b6942fb22e0b29dc16c71060718d1fb69eb with latest name-background UI.
- Final4K companion build and mesh-safe A/B fixture builds are queued/running.
  No task commit, push or main integration yet; complete visual evidence remains required.


Full-effects verification update (2026-10-21, still in progress):
- Directly inspected all247 authored keys from28clips, each in16cells: L/R/Up/Down,
  mapped/flat, both facings, at native448x352 matrix and nearest-neighbor2x.
  Also inspected all219 consecutive authored-key pairs and9 loop endpoint pairs
  in each variant (3504+144pairs). Per-key observations remain in
  E:/CodexTemp/TypingHero/dungeon-full-lighting/frame-review-notes.json;
  frame-evidence.csv contains3952per-frame/variant rows. Playback/runtime joins and
  bright-background coverage are still pending; these counts are static inspection.
- Static observations: scarf/blade grips remain attached; mapped skull/shoulder and
  round shield response changes with white source direction, mirrored UV/alpha
  stays registered. Thin bone/rib gaps remain open. Authored hit flashes, death
  white conversion and weapon smears are retained, not repainted by this task.
- All generated assets in144147 and151453 are byte-identical (final4k-hash-diff.json).
  Correction to earlier importer concern: serialized default platform for death
  companions already reads4096; legacy top-level field reads2048. Explicit size
  assignment preserves the effective4096 and produced no asset changes.
  Snapshot differences66189->75e969 contain only SurfaceNormalAssets importer
  assignment and disposable diagnostics; shipping runtime/shaders are identical.
- Source contract audit: no color PNG, AnimationClip or AnimatorController changes;
  changed source PNG metas differ only in secondaryTextures, original GUIDs retained.
  Generated501files imported with original scene record order/unchanged whitespace.
- Final matched ABBA: Edge155.0.4283.62, ANGLE/D3D11 GTX1650Ti,1280x720, isolated
  no-video20s real-input driver, identical draw wrappers, focused/visible combat.
  A1/A2 RAF p95=20.135/20.270ms(n404/412), B1/B2=20.220/20.585ms(n400/407).
  Mean draws A266.68/266.91,B331.43/331.42; triangles A12340/12502,B14425/14408.
  Sampled engine p95 A22.80/20.52ms,B21.37/21.44ms. No GPU-time claim or cross-GPU
  comparison. All four runs4checks PASS,zero exceptions/provider reads/writes.
  Raw timing-final-{A1,B1,B2,A2}.json and performance-final.json retained in evidence.
- Mesh-safe A/B diagnostic build153315/input75e969 is currently linking.
  Pending: A/B visual/every camera transition; authored playback/bright background;
  final native flow and reflection lifetime; final memory/diff/commit/integration.

Final isolated QA candidate153315/player0ecd1b6a6c221b30:
- Strict repeat builder/WebGL PASS; native15PASS,zero exceptions/provider effects.
- Inspected all26A/B stills and9camera poses. Reflection-off removes reflected actors/
  lamps; normals-off flattens skull/shoulders; metallic-off changes blade response;
  roughness-zero tightens highlights; DOF-off sharpens bricks while actors stay clear.
  Light-off distinguishes physical illumination from visible beam/emissive VFX.
  Haze/fog/bloom controls visibly differ; rain crop shows faint rear streaks.
- Bright background247keys/16variants inspected at native size. Dark review is native
  and nearest2x. camera-video-scope.json records402/402selected frames and401/401pairs
  viewed over22.000-38.080s; nine pans/FOV transitions,holds and return included.
- Authored playback recordings exist at1x(245keys reached across28clips; render cadence
  can skip16.7ms keys) andquarter(247keys/28clips). This is capture/log coverage,
  not visual real-time playback acceptance. review.html links both and final gameplay.
- User confirmation of perceived rhythm/readability requested through async question:
  available tools display static frames only; sprite-frame-review explicitly requires
  normal/slow playback. Do not infer acceptance from elapsed time or script counts.
- origin/main advanced to f7bfc67 with dungeon menu,compact pause/settings and parry
  physics. Integrate those changes normally and recheck affected native flows before
  any main push. Parent dirty checkout remains untouched. No task commit/push yet.

Integration checkpoint (2026-10-21, after isolated visual review):
- Task implementation committed9c8fc1e; main f7bfc67 integrated in87eb373.
  Scene merge preserved1907 main records plus21 lighting additions (1928 total),
  all intended task/main object changes; systems memory preserves both VFX rows.
- Full isolated native recording0.000-29.760s:744/744 frames and743/743 pairs
  directly inspected at native320x180 world resolution, including held/repeated
  frames. Hit,attack,parry,pause,Slayer hide/restore and menu return observed;
  no visual claim for real-time playback. gameplay-video-scope.json records scope.
- Integrated input47af0723 WebGL/strict builder and scoped DungeonRenderTests
  are running under the existing server lock. New menu/pause/parry flows will
  be checked on this player before pushing; main integration remains pending.

Integrated candidate results:
- QA160528/input47af072343bca6e35d522935fc088383497ff8c3d818a498956c0d71987d8bd5:
  strict builder twice, serialized object/reference/listener stability and WebGL PASS.
  Player2ac005eb7f4b60d3; native16checks PASS,zero runtime exceptions/provider effects.
  Play->preparation,attack,received hit,parry,compact pause/resume,complete Slayer,
  normal FOV35 restore and main-menu return verified with real input. Final seven
  gameplay/state images directly inspected. Reflection uses UniversalRenderer;
  Slayer releases its target,menu leaves zero owned world/reflection/camera objects.
- Quick162136/input1272a6a75dcdc482374a7f809486f083481825fa06c5fb25ec15eb9fad9a03ec:
  DungeonRenderTests4/4PASS,including exact pause/hide particle state and cloth anchor.
- Integrated generated501files retained separately. Scene1928/MainMenu482objects
  have no semantic changes from committed merge; only final scene blank line differs.
  No generated reimport needed. GUID audit scoped to Unity Assets passes; color art,
  frame timing,atlas slicing/pivot and source GUID contracts remain unchanged.
- Evidence: candidate7-render-audit.json,integrated-tests.xml,
  integrated-scene-builder-diff.json,integrated-guid-audit.json; review.html now links
  integrated native video d36973b39b19b304719e9e014ae30b1b.webm. Older744frame visual
  record remains clearly separate; no claim to have inspected every integrated-video
  frame. Unchanged lighting diagnostic/frame/camera evidence reused.
- Remaining acceptance: perceived normal/slow playback (async user review pending).
  Available image tools show static frames only. No full visual PASS/main integration
  claimed. Retain task worktree until that gap is resolved; evidence retained on E:.
