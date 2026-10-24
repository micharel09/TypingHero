# WebGL startup render diagnosis — 2026-10-24

## Lantern optimization result — accepted

The shared lantern candidate meets the measured startup gate on the frozen
`c86d72f` baseline: five fresh profiles per build give world-to-first-background
medians **2337.01 → 1829.30 ms (21.72% lower)**; the longest overlapping blocking
task median is **1931 → 1409 ms (27.03% lower)**. These include the prescribed
initial alternating runs and the ABBA follow-up, rather than selecting the best
runs. Fixed-light material/reflection review and clean integrated-player smoke
also pass. Retain the three-material shader change; no itch release is included.

### Scope and immutable inputs

- A: production diagnostic `20261024T075349-production`, input
  `778fd59130141568a4df9238d7e97913d58d85ddb8c3352721c8ae728b31e2cf`.
- B: production diagnostic `20261024T081134-production`, input
  `616eae41892e4c2d05aca4c2a07e584baabb46c0dbbdcdc97e09c05158f7e7aa`.
- A/B WASM and framework JavaScript are byte-identical. Data payload is
  15,625,567 → 15,448,378 bytes. Source comparison identifies only the three
  lantern material shader/keyword changes and seven new shader/include/meta files;
  the diagnostic code is identical. Textures, material values, GUIDs, queues,
  culling, blend/depth states and disabled glass passes are preserved.
- `LanternLit` uses URP14 vertex/input, BRDF, GI and light helpers, with uniform
  material switches for cutoff, emission, specular/environment reflection and
  shadow receiving. Its contract is the existing lantern surfaces, without normal,
  detail or metallic maps. It does not replace actor/floor shaders. The existing
  builder already preserves these authored materials and needs no new assignment.
- Runtime WebGL link count remains **26 → 26** in the measured interval. Do not
  describe the result as a measured reduction in total linked programs.

### Measurement protocol and results

Windows native Edge156.0.4314.42 headless, Intel UHD/D3D11,1280x720, production
settings, no requested throttling. Order `ABABAB`, then `ABBA` because the initial
samples varied. Each letter launches a new browser process/temporary profile and
then reloads the same context: five fresh plus five reload observations per build,
20 navigations total. HTTP interception disables HTTP caching; OS/driver-wide
caches are not purged. Reload is a browser/shader/Unity-cache comparison, not a
pure HTTP-cache comparison. Firebase fixtures/external blocking remain enabled;
no production writes or unhandled page exceptions occurred.

Timing runs contain no screenshots, canvas readbacks or pixel comparisons. JS
wrappers time synchronous shader/status queries; these are browser/driver waits,
not isolated GPU execution. Temporary C#/jslib markers identify actual world,
camera/RT and URP presentation boundaries. The first-frame interval is bounded by
completion of the submission callback and the next newer-timestamp rAF callback;
separate visual-only readbacks at the submission marker show the complete room.
UI text can become visible in a later frame; no changed menu-reveal behavior is
claimed. Phase medians overlap and must not be added together.

| Metric, combined observations | A median (ms) | B median (ms) | Change |
| --- | ---: | ---: | ---: |
| Fresh: world load → first background | 2337.01 | 1829.30 | -21.72% |
| Reload: world load → first background | 761.81 | 507.19 | -33.42% |
| Fresh: longest overlapping blocking task | 1931 | 1409 | -27.03% |
| Fresh: largest observed blocking task across all runs | 2373 | 2014 | -15.13% |
| Reload: longest overlapping blocking task | 440 | 222 | -49.55% |
| Fresh: shader/status waits | 1889.87 | 1390.14 | -26.44% |
| Reload: shader/status waits | 435.47 | 121.07 | -72.20% |
| Fresh: scene loaded from world-load start | 521.87 | 366.88 | -29.70% |
| Fresh: camera initialization | 1.23 | 1.18 | -4.07% |
| Fresh: RenderTexture.Create | 0.06 | 0.06 | unchanged |
| Fresh: world initialized → first background | 1910.98 | 1388.65 | -27.33% |
| Menu p95, fresh groups | 36.63 | 37.22 | +1.61% |
| Menu p95, reload groups | 36.23 | 38.26 | +5.62% |
| Menu pooled-frame p95, fresh (770/774 frames) | 36.42 | 38.14 | +4.74% |
| Menu pooled-frame p95, reload (770/769 frames) | 37.09 | 37.68 | +1.56% |

The initial three-per-build fresh medians were2579.29 →1860.86ms (-27.85%),
but menu p95 in that fresh group increased12.97%. The ABBA follow-up has fresh
medians2252.09 →1666.35ms (-26.01%) and menu p95 +0.54%; the combined results
above retain every original and follow-up observation. Pilots are excluded.

### Gameplay and visual evidence so far

- Same-input10-second normal gameplay segments, no screenshots during sampling:
  A319frames/p95 **33.19ms**, B320frames/p95 **35.35ms (+6.51%)**, within the10%
  limit. The preliminary unmatched setup produced88.59/33.23ms and is retained
  as exploratory evidence, not attributed to the shader or used for acceptance.
  No general FPS, balance, stress or soak claim.
- Native keyboard/mouse Play → guide → Start Battle → pause → Main Menu was
  inspected on both A and B. All nine startup marks remain single occurrences
  after returning: no new world-load/camera/RT initialization. `typingHeroQA` is
  undefined; zero page/console errors and fixture writes in the accepted segments.
- Normal room and menu images were inspected, including the actual three-material
  hanging lantern. Four correctly timed Space inputs deplete guard and enter
  Slayer on both builds. Stable red-background/black-silhouette states were
  directly inspected. Cue-threshold/spam pilots are exploratory, not evidence of
  a gameplay failure. No sprite-animation authoring or frame-review claim.
- Clean candidate production `20261024T085059-production`, input
  `45b651b20749a3faa1eb039647be4a6cfd0311552ecaff3b2439a9d82f9ad006`, passed.
  All QA and StartupProfile markers are absent from JS/WASM/data in ASCII and
  UTF16; no diagnostic/fixture C# or jslib remains in the working source.
- Fixed-light fixture build `20261024T084828-production` failed because its
  temporary editor script was placed in the automation assembly without a URP
  reference. Moved it into the existing URP-aware editor assembly, without asmdef
  or package changes. Corrected fixture input
  `20a48ba4a8fbd88e3b00178df373612abb54c34b0aee7a1f90bf978ca2752af5`
  passed as `20261024T085906-production`. Two captures2seconds apart are
  identical. Direct comparison shows matching alpha/cutoff cards, emission and
  surface lighting;99.766% of pixels match exactly, with differences localized
  to shadow raster edges from the30-unit camera/group translation. However the
  production quality disables realtime probes, so this is not reflection
  acceptance. The last fixture explicitly enables probes only in the diagnostic
  build, positions them outside the sample geometry, and uses the actual scene's
  iron/glass meshes. Input
  `eedd03a5b59ec92ea6054e745145e5ba7d7bdaf251661bf1a82d54e60d049d29`
  passed as `20261024T093335-production`, but its native probes still used the
  project's default Renderer2D, so its reflection comparison is excluded. Final
  fixture `20261024T094239-production`, input
  `16ddea2f20a13bdb472af2550c4b246476ed1ee2a52fff17ce23b32cfddc615e`,
  explicitly uses forward-rendered, box-projected probes outside the geometry.
  Both sides retain the same fixed lighting; actual scene iron/glass meshes,
  wood, cutoff controls at.25/.5/.75, transparent glass, emission, shadows and
  their floor reflections were directly inspected. Visual PASS. Two captures
  separated by2seconds are byte-identical; A/B halves have99.422% equal pixels,
  mean absolute RGB difference0.0259/255 and0.119% pixels differing by more than
  8levels. Remaining differences are localized to shadow/reflection raster edges;
  the two fixtures are30world units apart, so no pixel-identity claim is made.
  Game quality/settings, default renderer and reflection camera cadence are
  unchanged; these probe overrides exist only in the temporary diagnostic build.
- New main `3d6b74a` was merged into the task branch. Its changes concern parry
  assets/runtime flipping and editor-only depth inventory, with no lantern,
  floor/actor shader, world lighting/camera or world/menu scene changes. The
  isolated timing results above remain explicitly scoped to the measured base.
  Integrated source `e59a86dcb2f4d9bf30c3965f3182f912dd6c5cabfe0fff736847f6e7381209f4`
  passed in `20261024T090837-production`; ASCII/UTF16 marker exclusion and native
  real-input normal/Slayer/return-menu smoke pass with no page/console errors or
  fixture writes. The normal room, stable Slayer silhouette and restored menu
  were directly inspected; QA is undefined and startup marks are absent.

### Acceptance and retained resources

Startup threshold, reduced longest blocking task, fixed-light material/reflection
comparison, normal/Slayer presentation, retained-world return flow and the scoped
menu/gameplay p95 limit pass. All diagnostic/fixture code has been removed from
the runtime source. Only three material shader/keyword edits and the new shared
shader/includes/metas are retained; no scene, actor/floor material, gameplay API,
lighting, camera frequency, package or loading-cover change is introduced.
The existing material-preserving builder was verified by the production builds;
no redundant builder mutation was added. This is machine-specific evidence, not
a general FPS or cross-device guarantee. No itch upload/deployment was performed.

Keep the accepted A/B inputs/players, integrated player and final fixture plus
timings, images and build receipts in the evidence directory until a reviewed
follow-up replaces this experiment. Owned build requests and native test browsers
have ended; remove the clean task worktree/local branch only after Git remote
integration verification. Remote task branch and shared runner caches are retained.

Raw inputs, per-run values, player hashes, harness source and images are retained
under `E:/CodexTemp/TypingHero/startup-render-profile/lantern-experiment/`.
`timings.json`, `abba.json`, `analysis.json`, `source-delta.json`,
`material-contract.json`, `player-hashes.json`, accepted `A/B-flow2` and
`A-slayer2`/`B-slayer` captures distinguish measurements from visual checks.
Keep this evidence until a reviewed follow-up replaces the experiment.

## Original investigation — request and conclusion

Investigate the delay between entering the menu and seeing the live dungeon
background. Preserve the shared 3D world, lighting and gameplay.

The measured production players spend most of the first world-render stall
waiting inside WebGL shader/program status queries. Fresh-browser shader work
takes 2.26–3.20 seconds; same-context reload takes 0.19–0.29 seconds. URP Lit
environment programs are the largest individually captured program waits.
This establishes an actionable first-use shader bottleneck in these players;
it does not quantify every startup phase or establish a performance fix.

Recommended first change: keep a loading cover until the real world and
presentation camera have completed their initial rendering, then reveal/enable
the menu together. Prewarm only the required world shader variants under that
cover, or use the real world render itself as the warmup. Scene load completion
and RenderTexture allocation alone are insufficient readiness conditions.

For shorter actual startup, next measure an isolated optimization of the active
URP Lit environment programs/material keyword combinations. Removing unused
build variants alone does not remove compilation of the variants actually drawn.
Keep shadows, reflections, material appearance and current gameplay unchanged;
accept a shader change only with cold-start A/B timing and visual comparison.
Warming everything at startup would add work; moving compilation under a cover
improves presentation but is not evidence of reduced loading time.

## Inputs and environment

- Fetched `origin/main`: `0dc451157dcaaa5809358aab67394786ed7d29ec`.
  The original dirty `main` checkout was preserved; exploration used an independent
  Orca worktree based on this remote commit.
- Published-player URL measured directly:
  `https://html-classic.itch.zone/html/19943928/index.html?v=1792658720`.
  This upload is identified by the repository's pause-release receipt. Anonymous
  access to the game page returned Cloudflare HTTP403, so its current selected
  upload was **not independently verified** in this investigation.
- Retained normal production player: immutable input
  `af65179bd627f37e871c8014c0a4c45a1be1fd4e59c77ae7c99e3f5dd6cc40a1`,
  `/data/typinghero-game-loop/task-self-shadow-20261023/results/20261024T052952-production`.
  All33local player files still match its recorded download hashes; `qa=false`.
- Compared448source inputs in rendering/settings/world/material areas equal to
  fetched main, including source MainMenu/DungeonWorld scenes and MainMenuUI.
  The cached pre-build inventory differs in four entries: a scene template,
  ActorDepth, DungeonPresentationRender and SpriteDepthVisibility. Actual saved
  build outputs for ActorDepth and DungeonPresentationRender equal main.
  The generated DungeonWorld YAML is **not byte-identical** to source YAML;
  source parity must not be presented as fresh-build output equivalence.
- An instrumented build of the fetched main was queued twice with300-second
  shared-lock waits; both timed out before Unity ran. No phase-level measurements
  or fresh-main build PASS are claimed. Temporary C# logs/observer were removed.
- Unity2022.3.25f1 players; native Windows Edge156.0.4314.42, headless, ANGLE Intel
  UHD Graphics/D3D11,1280x720. Numbers are specific to this machine and harness.

## Measurements

Three independent fresh browser processes/profiles per player, each followed by
one reload of the same context. Runtime network interception blocks external
effects and fixtures Firebase; HTTP caching is disabled by interception. Reload
is a warm browser/shader/Unity cache comparison, not a pure HTTP-cache comparison.

Timing runs used no intermediate screenshots or canvas readbacks. Wrappers time
the original WebGL functions without changing their arguments/results;
PerformanceObserver records long tasks and rAF records callback gaps. Status-query
durations measure synchronous browser/driver waiting, not standalone GPU execution
time. Wrappers/observers have overhead. No CPU/GPU throttling was requested.

| Player / state | Shader calls total, three runs (ms) | Longest world-stage task, three runs (ms) | Texture upload calls total (ms) |
| --- | --- | --- | --- |
| itch19943928 / fresh | 3196,2599,2565 | 3252,2596,2602 | 17,17,21 |
| itch19943928 / reload | 225,285,251 | 433,463,460 | 24,14,20 |
| retained production / fresh | 2455,3193,2258 | 2511,3201,2308 | 96,73,238 |
| retained production / reload | 194,227,247 | 259,300,346 | 85,74,74 |

Shader total sums compileShader/linkProgram/getShaderParameter/getProgramParameter.
Texture total covers texImage2D/texSubImage2D only, not download, decode or all
allocation work. World-stage task is the longest task starting more than1800ms
after loader hide; this identifies the observed post-splash render stall, not a
C# method boundary. Larger reload tasks during engine initialization remain
outside that column. No general FPS/steady-state performance claim.

Both production timing sets have `typingHeroQA` undefined, no unhandled page
exceptions and zero observed provider writes. itch records one failed-resource
console message per navigation with external blocking enabled; the failing URL
was not recorded, so this is not claimed as a wholly error-free network run.

Separate visual runs sampled the canvas at100ms and inspected the last flat
background and first complete dungeon/menu image. A flat frame followed by the
live dungeon was observed. Those captures do **not** establish that menu text was
already visible during the flat interval; that exact reported visual ordering
remains unverified. They are not sprite-animation review or timing acceptance.

## Evidence and remaining verification

Local evidence: `E:/CodexTemp/TypingHero/startup-render-profile/` contains the
driver, `summary.json`, raw timing/visual reports and PNGs, player identity and
source comparison, and the failed anonymous-page status. The initial
`retained-production-visual` directory is exploratory and actually used an older
QA player; it is excluded from the production table above.

The exact fetched-main scene-load/initialization/first-submit breakdown remains
unmeasured. A proposed implementation still requires a fresh same-input baseline
and candidate: verify first visible menu includes the complete dungeon, cold
startup blocking/ready times, and retained-world Play/return behavior. No runtime
optimization, new release or itch deployment was performed here.

Both owned build requests ended; the empty remote task runner directory and its
three toolchain symlinks were removed without touching shared targets. Keep local
measurement evidence until the proposed startup fix is reviewed/replaces it.

Unity2022.3 references:
[shader loading](https://docs.unity3d.com/2022.3/Documentation/Manual/shader-loading.html),
[targeted WarmUp](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/ShaderVariantCollection.WarmUp.html).

## Historical baseline blocker (2026-10-24)

- Fresh fetch and independent Orca worktree use main commit
  `213e67d6861d83116d62d960906d35bb11efe6ff`, branch
  `micharel09/webgl-startup-lantern`, checkout
  `C:/Users/ADMIN/orca/workspaces/TypingHero/webgl-startup-lantern`.
  The original dirty checkout remains untouched.
- `python tools/game_loop/run.py doctor` failed with
  `[Errno 11] Resource temporarily unavailable` while acquiring the shared runner
  lock. `python tools/game_loop/run.py production --wait-seconds 300` then failed
  with `Timed out waiting for shared runner lock`, before snapshot unpacking or
  Unity execution. No baseline build, build result directory or new timing data
  was produced. SSH works; `/data` has 76 GiB available and memory has roughly
  8.6 GiB available. Read-only lock inspection showed another Python process
  holding `/data/typinghero-game-loop/runner.lock`, with multiple queued waiters
  and an active Unity task container. Existing jobs were preserved.
- Source inspection: `DungeonHangingLanternBuild.cs` now only declares the art
  path and explicitly preserves authored assets. It no longer assigns shaders;
  do not introduce a builder mutation without evidence that one is necessary.
  All three lantern materials use URP Lit and disable specular highlights and
  environment reflections. Iron uses alpha cutoff 0.5 and writes depth; wood
  is opaque and writes depth. Glass uses SrcAlpha/OneMinusSrcAlpha, no depth
  writes, emission `(1, .38, .08)`, no receive shadows, and disabled DepthOnly
  and ShadowCaster passes. Preserve material GUIDs, queues, textures and culling.
- No runtime candidate was applied. The shader experiment, phase instrumentation,
  visual fixture, menu/Normal/Slayer checks, world reuse and frame-time checks
  remain **unverified**. This is an infrastructure block, not evidence that a
  candidate failed the performance threshold. No main integration or itch release.

Resume this same branch/worktree when the runner is available. First build the
fresh-main baseline, then use identical diagnostic instrumentation for baseline
and candidate to distinguish world load, camera/RenderTexture initialization,
shader waits and the first complete background frame. Exclude instrumentation
from the final runtime change. Alternate baseline/candidate: three fresh browser
processes/profiles and three same-context reloads each, same machine, viewport and
production configuration, without screenshots during timing. Require at least
20% lower median world-load-to-first-background time and a lower longest blocking
task; use ABBA if results are noisy. Require fixed-light visual equivalence for
all three materials (alpha, emission, shadows, reflections), menu/Normal/Slayer,
Play → gameplay → menu world reuse, and no more than 10% frame-time p95 regression
in short menu/gameplay runs. Keep character/floor shaders, lighting, reflection
camera cadence and gameplay APIs unchanged; no loading cover or warmup credit.
Retain this worktree for continuation; remove it only after verified integration
or explicit abandonment. Commit/push this checkpoint separately from any future
runtime acceptance; merge runtime changes only after every gate passes.

### Runner retry and baseline build (2026-10-24, 07:32 UTC)

The runner queue advanced and the retry acquired the lock. Production attempt
`/data/typinghero-game-loop/results/20261024T072716-production`, snapshot
`8c4dd3c1e1612955895015b4933c55b5f7aff7e4107a43c4f8f14ce6697d1a4c`,
failed during preprocessing: `Protected typing font/material asset changed:
Assets/Art/UI/Branding/CobblestoneUI64.asset`. No player or timing was produced.
The Windows checkout has `core.autocrlf=true`; this font had 6014 CRLF line
endings. This matches the previously documented baseline issue in
[parry-main-demo](parry-main-demo.md) and [dungeon-3d](dungeon-3d.md).

Restored only this worktree's font file to the exact `git show HEAD:<path>` blob
(LF). Git reports no font content diff; no guard or font settings were changed.
Fetched and merged newer main `c86d72f` (scene-aware sprite depth work). The next
production baseline acquired the lock at
`/data/typinghero-game-loop/results/20261024T073226-production`, snapshot
`26aab389f0b29f724718bfdce0d4c12bf2c10082771742161c754b67b6ea1b86`.
Unity is running; build success and startup measurements are still pending.
The previous baseline-lock blocker is no longer the current state. No shader
candidate has been applied. Preserve this result for the next checkpoint and
inspect its completion before launching another build.

### Historical A/B execution checkpoint (2026-10-24)

- Production baseline `20261024T073226-production` succeeded; normal-player QA
  exclusion passed. Native Edge visual smoke has no page/console errors or
  provider writes; menu, preparation and the current single-page battle guide
  were inspected. This is not optimization acceptance.
- Diagnostic A `20261024T075349-production` succeeded, input
  `778fd59130141568a4df9238d7e97913d58d85ddb8c3352721c8ae728b31e2cf`.
  Temporary C# and jslib markers identify world load, scene loaded, camera/RT
  initialization and first presentation/frame submission. These files must be
  removed from the final runtime change. A separate visual-only first-frame
  readback shows the complete dungeon/menu; no readbacks belong in timing runs.
- Candidate B input
  `616eae41892e4c2d05aca4c2a07e584baabb46c0dbbdcdc97e09c05158f7e7aa`
  is queued/running. It changes only the three lantern materials' shader and
  obsolete keywords, plus seven new shader/include/meta files. The material
  property and GUID comparison passes; no builder shader assignment exists.
- Evidence and runnable drivers are under
  `E:/CodexTemp/TypingHero/startup-render-profile/lantern-experiment/`.
  `A-input.tar.gz`, `B-input.tar.gz` and `inputs.json` preserve exact source
  identity; `material-contract.json` records the scoped asset comparison.
  `pilot-A.json` is diagnostic only, excluded from final A/B statistics.
  The timing driver now waits for a strictly newer rAF timestamp after first
  submission; submission-end/next-rAF bounds describe the presentation proxy,
  not standalone GPU execution time. Formal alternating runs remain pending.
