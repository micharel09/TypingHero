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
