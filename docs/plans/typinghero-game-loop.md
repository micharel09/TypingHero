# TypingHero game loop implementation

## Scope and acceptance

Implement the approved repo-local skill, three source-backed memory documents,
Ubuntu Docker Unity 2022.3.25f1 runner on fly-server, offline Unity tests, read-only
conditional QA bridge and keyboard/mouse Playwright profiles. Preserve gameplay,
asset GUIDs, Test Framework 1.1.33, and existing dirty files.

Original acceptance (heavy profiles superseded by the user instruction below):
skill validator, path checks, diff review; nonzero passing quick tests;
WebGL name/start/combat/parry/Slayer/pause/round/restart; stress large word pool,
1,000 words, repeated VFX and leaderboard 500/1,000 rows at two viewports; 1,800
wall-clock seconds soak with progress/exception/frame/memory/object monitoring;
normal WebGL build without QA bridge. No profile may silently pass missing results.

## Implementation checkpoint

- 2026-10-16: inspected source, scene/config/prefabs and existing WebGL pipeline.
  Existing untracked `design/` and `docs/plans/pixel-ui-refresh.md` belong to user.
- SSH fly-server works; Ubuntu Docker 29.7.2, 15 GiB RAM (12 GiB available),
  `/data` 22 GiB free, Docker root `/data/docker`. No Unity image present.
- Resolved Unity Linux amd64 manifest digest from registry (no tag-only execution).
- Unity license at `/data/secrets/typinghero/Unity_lic.ulf` absent; requested external
  activated license path from user. Never extract GitHub Actions secrets.
- Added repo-local skill and AGENTS routing, gameplay/system/verification memory;
  runtime, editor and PlayMode test assemblies with new asset metadata (old GUIDs untouched).
- TypingManager extracts only `ProcessInput` from the real input path. Concurrent
  UI work added InputBlocked and other UI/editor changes during this task; retained
  those changes. Added Assets/Editor assembly boundary because the new root runtime
  assembly otherwise absorbs editor-only scripts.
- Added 9 quick tests and 3 stress tests: typing/type-ahead/timeout, word pool,
  input gate, health/parry, score combo; 50,000-word pool, 1,000 completions,
  repeated 16-instance VFX bursts with reuse/scale assertions and fixture cleanup.
- Added conditional read-only QA state probe and WebGL build hook. Browser profiles
  use real keyboard/mouse, fixture all Firebase reads/writes, block external traffic
  and run in a network-disabled Docker container. Soak has a 30-second progress
  deadline, 1,800-second wall-clock duration and sampled metrics.
- Runner snapshots dirty source, locks before replacement, pins Unity amd64 digest,
  persists cache, limits resources, checks free disk and kills its own container on
  timeout. Test XML and browser result checks reject empty/missing/failed output.
- Checks passed: skill quick_validate, Markdown link resolution, asmdef JSON/new
  automation metadata, Python AST, Node syntax (local and Ubuntu), git diff --check.
  Six result-contract unit tests passed on fly-server (including suite failure with
  passing cases). These are infrastructure checks, not Unity gameplay evidence.
- Remote doctor, setup and quick attempted: fail explicitly on absent activated
  license. Unity image absent. `/data` free 21.4 GiB; setup reserve configured 35 GiB
  (short 13.6 GiB); compressed Unity layers measured 4.79 GiB, unpacked size unknown.
  No image downloaded, unrelated Docker resources untouched.
- Unity compilation, quick, WebGL, stress, 1,800-second soak and normal production
  build **not verified**. No performance baseline or runtime acceptance claim.
- Retained `/data/typinghero-game-loop/source` (latest synchronized source) and lock
  file for resume; no build/capture results generated. Source may lag the current
  checkout after final edits; run.py always snapshots again. Keep this task directory
  until verification completes, then remove task snapshots/results/builds while
  retaining explicitly useful toolchain/import cache.

## Autonomous setup continuation — 2026-10-16

- User requested autonomous completion of setup. Rechecked local/server license
  locations: Windows Hub has an entitlement XML, no portable ULF; no server ULF.
  Repository secrets UNITY_EMAIL, UNITY_PASSWORD and UNITY_LICENSE exist (names
  only inspected; no values exported). Prepared use of the same account through
  a trusted ephemeral self-hosted Actions activation job on fly-server.
- Downloaded and SHA-256 verified official Actions runner 2.338.0 and GameCI CLI
  0.1.72 to `/data/typinghero-game-loop`. Runner binary starts successfully.
  Runner has since been registered ephemerally; main and the local index remain unchanged.
- Prepared administrative helpers under `E:/CodexTemp/TypingHero/game-loop/`;
  prospective isolated remote activation branch `codex/game-loop-setup-20261016`.
  Existing local index/staged UI changes are preserved.
- Complete pinned image measured and SHA-256 verified: compressed 5,143,833,841
  bytes (4.79 GiB); uncompressed layer tar bytes 10,670,690,304 (9.94 GiB).
  Complete transport cache is `E:/CodexTemp/TypingHero/game-loop/image-layout`.
  Retain until Docker import is verified. No need for the old 35 GiB estimate.
- SSH alias uses Tailscale. Direct LAN 192.168.11.30 is reachable; server host key
  fetched through the trusted alias and pinned in task `known_hosts`. LAN bulk
  transfers also showed low throughput/packet loss. Parallel SCP worsened it;
  stopped only task-owned clients and removed their abandoned parts. Current
  single-stream LAN HTTP transfer is about .9 MiB/s with Range resume and full
  SHA-256 verification. It serves only public image blobs, no credentials/source.
  Windows server exec session 61287; active Ubuntu pull exec session 71356.
- Captured runtime source snapshot is `1ab7e26d56fd710ce398162d9d9b13106a6667e52a5706ee8136ba0e691c98f3`;
  master archive `/data/typinghero-game-loop/input-snapshot.tar.gz` (private).
  `local_profile.py` reuses these inputs for every profile instead of resnapshotting
  Unity-generated scene/asset modifications. Later doc edits do not change runtime inputs.
- Added digest-verified, bounded-range image measurement/cache and streaming OCI
  import helpers to avoid downloading the image twice. Resource preflight now
  accounts for measured compressed+unpacked image storage; task containers check
  free disk during execution and stop below 1 GiB. Initial stage headroom is 3 GiB,
  replacing the earlier unmeasured 35/12 GiB estimates. Gameplay acceptance unchanged.
- Setup no longer requires a license before installing activation prerequisites.
  Default uses the dedicated private Unity HOME, supporting both account entitlement
  XML under `.config/unity3d/Unity/licenses/` and a legacy ULF. Explicit `--license`
  remains supported. Activation/runtime containers have fixed project hostname/MAC
  to keep the licensed virtual workstation identity stable; legitimate account
  activation is still required. Do not copy the Windows entitlement across machines.
- Eight runner contract tests now pass on Ubuntu, including timeout container cleanup
  and insufficient-disk rejection. Fixed the browser HTTP root path returning 403;
  parry input timing is based on actual .9833-second strike / 1.3333-second clip and
  .25-second player parry window, still using real keyboard input.
- Actual offline Docker OCI import probe passed. First probe exposed Docker 29
  canonical-name and manifest-ID differences; loader now writes the daemon's own
  annotation format and validates the exact pinned manifest ID. Both probe images
  and fixture directories were removed; unrelated Docker images left intact.
- Playwright 1.63.0 installed; real Chromium/WebGL2 smoke passed under the intended
  2 CPU / 8 GiB / 1 GiB SHM limits with network disabled (Chromium 153.0.8010.12,
  ANGLE/SwiftShader). This is tooling evidence only, not the game WebGL profile.
  Host/kernel/image-store facts are recorded in `docs/game/verification.md`.
- Browser host now verifies actual Unity render size, observes paused timer/HP/score,
  and checks leaderboard instance IDs across a real Top refresh. Probe reads the
  actual attack animator state for parry timing. Startup console exceptions are
  also collected. These changes await runtime validation, not just static checks.
  The soak watchdog advances only on completed words (restarting alone cannot
  conceal a stalled loop); each round must show score progress and restart at max
  HP. Chromium receives 1 GiB private shared memory within the 8 GiB container cap.
- Trusted ephemeral Actions job dispatched:
  https://github.com/micharel09/TypingHero/actions/runs/38849680145
  Remote task branch `codex/game-loop-setup-20261016`, commit
  `3d8db6e426027ac5fdbd3341ff0a03bb2db43b26` changes only the dispatchable workflow.
  Runner label/name `typinghero-game-loop-20261016`; registration stored privately
  under `/data/typinghero-game-loop/actions-runner`. Job is waiting for the verified
  cache, then installs toolchain, activates using the existing account secrets and
  runs doctor/quick/WebGL/stress/1800-second soak/production sequentially, failing
  on the first error. No secret value has been exported to local files/chat/repo.
  The runner is ephemeral and should deregister after its one job; verify cleanup.

Latest checkpoint: Unity image imported with matching pinned manifest ID. Public
HTTP server session 61287 stopped. First job failed because GameCI activation
requires Git metadata; activate.py now initializes only an empty disposable
snapshot repository. Second job 38856603799 activated successfully, doctor passed,
and Quick passed 9/9 actual tests (results/20261016T053630-quick/tests.xml).
WebGL then failed preflight with 2.5 GiB free vs 3 required. Removed only duplicate
extracted global package cache (all packages verified present in Library/PackageCache)
and the completed auto-deregistered ephemeral runner; 4.2 GiB restored. Retained
Library, npm archives, image, toolchain and private license. No unrelated data removed.
WebGL resumed directly over SSH, exec session 9789, using snapshot 1ab7e26d...
No need to reactivate or register another Actions runner. Continue actual WebGL,
stress, 1800-second soak and production; update baseline only from passing results.
Remote administrative branch remains until final cleanup; Windows image backup
remains task-local until profile validation finishes.
WebGL passed 13 assertions and one completed/restarted round at
results/20261016T060553-webgl/browser.json, snapshot
c7215bbdbc141a193c3d5ee15f3f01886633e8c5fd3e54c31e229b417d54ccfd.
First browser attempt scored/parried but missed Slayer and round completion:
serialized enemy stamina is 250, so harness now observes every strike and allows
real enemy attacks to end the round after seeing Slayer. Space is held 100 ms for
headless frame cadence. No gameplay tuning changed. Stress running, session 80680.

Stress browser initially assumed active pool count equals dataset size. Concurrent
UI implementation virtualizes rows: 500 records correctly bind to 19 objects.
Added read-only dataRows/virtualized/window geometry observations and check full
dataset plus bounded pool, window advancement and preserved instance IDs/scroll.
Pause gameplay during leaderboard checks; refresh remains real-time. A single
DOM wheel delta is one legacy Unity notch, so send repeated real wheel events
until the unchanged scroll-distance target is reached. Stress rerun session33628,
snapshot d93a43a44b47e6a4c5a23512e0bf0b8921aef94a157fd8bd8c2279425afa0a8d.

Stress passed 3 Unity tests and 34 browser checks, all four dataset/viewport
combinations: results/20261016T061754-stress. First soak correctly failed the
30-second word-progress watchdog after restart. A diagnostic rerun proved
timeScale remained 0.0001 with paused=false and unchanged animation/word state.
Root cause: HitStopper.Stop replaces an active coroutine after its deadline but
before its continuation, saving the frozen scale as its new restore value.
Added a deterministic Quick regression (expired deadline before continuation,
both stacking settings, preserve original 0.75 slow motion). It failed before
the fix with actual 0.0001 vs expected 0.75. HitStopper now keeps the original
scale while active. Quick passed 10/10 after the fix.
Current snapshot 3fc0efb2736e8b91bd88a171fa358bf89c001c346bc8f1106291ac3c1060d43f.
Sequential WebGL/soak/production running in session 62602. Stress's leaderboard,
typing and VFX inputs are unchanged by the focused hitstop fix; reuse its result.

WebGL after hitstop fix passed 13 checks: results/20261016T063518-webgl.
Harness now continues within its unchanged 180s deadline if an early round loses
before Slayer; it stops only after all required observations and a round restart.
Current snapshot c9c37e2ebb05beaf51bb92de531a7644dc313548bafa5fb769b9d3d91b2c5fdc.
Soak then production running sequentially in session49161. Soak passed midpoint
935s/35 rounds/520 completed words, no exception/watchdog failure yet; not complete.
Windows public image transport cache removed; HTTP server stopped. Temporary
remote admin workflow branch deleted after exact tip verification. Runner already
auto-deregistered and task binaries removed; private license and toolchain retained.

Final scope update from user: lightweight testing only; do not run all heavy
profiles. Stopped the exact task soak container at ~1355 browser seconds; sequence
exited immediately and production never started. Partial post-fix soak observed
51 rounds/750 words without the prior stall, but is not a full soak pass or baseline.
Current acceptance: installed/activated toolchain, valid repo skill/memory, Quick
10/10 and WebGL 13 checks passed, reviewed focused hitstop fix, static/doc checks.
Stress was already passed (3 Unity tests + 34 browser checks); no more heavy runs.
Skill defaults now honor lightweight impact-based checks. Full soak and production
QA-bridge exclusion deliberately unverified under updated scope.

Final review: current runtime inputs still exactly match verified snapshot c9c37e2e...
Skill validator, Python/Node syntax, asmdef JSON, metadata coverage, relative links
and diff whitespace passed. Preserved concurrent staged Pixel UI changes. Only
production edits from this task are input-path extraction and the focused hitstop
restore fix. Temporary server reports/build captures/runner workspace were cleaned
after recording results. Retain `/data/typinghero-game-loop` license, toolchain,
Library, Docker image and captured source/master archive for the requested reusable
automation; these are intentional persistent caches, not an active background job.
