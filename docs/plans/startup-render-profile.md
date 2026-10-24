# WebGL startup render diagnosis — 2026-10-24

## Request and conclusion

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

## Shared lantern shader experiment — blocked before baseline (2026-10-24)

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
