# Verification and performance memory

## Pixel spark integration/release preparation (2026-10-18)

- User authorized local main merge and itch.io update. Reviewed/preserved completed
  design and legacy CodeManu foundation in main610352b, merged into the feature at
  372b9f0; current rendering remains physical pixel sparks. Design-only assets are
  excluded from WebGL snapshots and have no runtime links. Legacy notices retained.
- Post-foundation Quick66/66 PASS20261018T095638, source input
  `39d1ff867e100a65ffafddc764561681ce8276372ac49b4c3e25d1302fbdcc44`.
  QA095748 build/repeated-builder PASS. Software browser again failed before first
  hit due slow rendering; no full software profile PASS claimed.
- Same immutable player, native Edge155/Intel UHD ANGLE D3D11: **25,619 checks PASS**,
  207.352s,1280x720/640x360, all four live particle roles, normal/Slayer camera,
  actual keyboard, pause/resume, lethal/results/restart.37fixture reads/two mocked
  writes, zero JS/Unity exceptions. Reviewed final640lethal/normal and retained
  previous art/motion evidence (runtime pixels/physics unchanged by legacy merge).
- Production095953 PASS on that same input. QA=false; JS/WASM/data exclude all
  three bridge markers. Exact original HTML smoke on native Edge155 passes name,
  Enter/tutorial BLADE/ATTACK COMPLETE/score10, real battle word `it`, orange pixel
  impact (visually reviewed, no pink material), pause/resume and actual640x360
  canvas.11fixture reads/zero writes/zero JS or Unity exceptions. Startup logs
  identify unsupported optional URP EASU/HDR-debug shaders on this GPU; the actual
  game and pixel material render correctly. No pipeline change made for these.
- ZIP29members/16,311,433bytes, local/server SHA256 and CRC PASS:
  `c58174cc5e84498e41151a3c006b853b528117a69dbff91a8c720a02c88eee08`.
  Candidate ZIP/manifest retained under server releases/pixel-sparks-20261018.
  Existing score-hud release remains untouched. Production smoke tool corrected
  OCR control-character JSON parsing; final explicit context shutdown and Node
  exit confirmed. A saveAs after persistent-context close failed, so non-QA
  evidence uses inspected native frames/screenshots, not a finalized export claim.
- Publication is pending: main has further active HUD/scoring/art edits outside
  this feature; do not commit or publish their unfinished source. BrowserSkill
  daemon/IPC works, but extension doctor reports zero connected browsers. User was
  asked to pause concurrent writes and connect the logged-in browser. No target
  main merge or itch upload claimed. See combat-feedback checkpoint for resumption.

## Physical pixel impacts (2026-10-18)

- Final Quick **66/66 PASS**, result20261018T093521-quick, input
  `c6d86c7acee7b370d7185a9eeb1bfa11c8e7578df58706c921771876753a7837`.
  Covers four real prefabs, integer pixel sizes/contact/force, actual gravity/bounce,
  hitbox collider dedup and lethal priority after Slayer exit, same-frame tutorial
  lock, hitstop, pause/results distinction, single release, clean reuse, eight-burst
  owner limits, disable/unload and Inspector tuning/GUID retention. Contracts8/8,
  browser/driver syntax and diff whitespace checks PASS.
- WebGL20261018T093620 build and repeated-builder checks PASS on the same input.
  Serialized outputs match source semantically; all PixelSparks assets/metas matched
  byte-for-byte before removing Unity-generated trailing whitespace for commit. Software input driver failed before first hit at ~198ms/frame;
  this is not a full software WebGL profile PASS. No timing thresholds relaxed.
- Native Edge155/Intel UHD ANGLE D3D11, existing Playwright1.58.2, QA Development
  allocation counters and recorded video: **25,242 checks PASS**,196.756s, exact
  1280x720 and640x360. Real keyboard Normal/Parry/Slayer/Lethal at both sizes,
  ordinary and Slayer camera, pause age/position hold, resume, victory results
  cleanup and restart. All four live particle roles observed; zero JS/Unity
  exceptions,36fixture reads/two intercepted writes, external blocking retained.
  Driver SHA256 `b3b07bf5904b60b0d00946871d7d67367a5c3d296ba2eb9bbee5db3e408fc2b1`.
- Forward parry motion151checks and mirrored parry142checks PASS at both sizes.
  Mirror fixture only changes actor positions/scales during disposable QA build;
  observed negative mean force -2.6764/-2.7063WU/s, with readable normal HUD.
  Actual live/video frames and nearest-neighbor4x crops reviewed: sharp flat
  squares/streaks, blue/orange/ivory distinction, fall/bounce and natural release,
  no halo/pink shader, no old animated impact stacked, preserved slash/flash/
  afterimages and readable cues/poses/text. Direct speaker listening unverified.
- Relative measurements on the same machine/Development flags and keyboard
  sequence, unpaused distinct published frames (QA capture/transition spikes kept):

  | Render | Frame ms p50/p95, before ? after | Allocation B/frame p50/p95, before ? after | Objects p50/p95, before ? after |
  | --- | --- | --- | --- |
  |1280x720|31.280/32.840 ?31.235/31.825|1618/12006 ?636/3980|233/238 ?274/280|
  |640x360|31.260/32.395 ?31.255/32.000|588/11706 ?636/11818|233/239 ?272/279|

  Final maxima243/237particles and8active bursts; no unbounded accumulation.
  Extra objects are bounded pooled roots/two systems/plane per burst, versus one
  animated object previously. Median memory rises ~226KB/231KB; no material frame
  or allocation regression observed. These are instrumented headless recordings,
  not a device-FPS promise. Baseline inputd1898611ff53e4c0c3ce7dd050bcb11961dcb42333fc8df76bcd18763e0263b0,
  player090104: both victories/zero exceptions, but overall3796check status failed
  because640poll missed its180ms legacy lethal (render mask127/video confirmed).
  Its measurement samples remain valid; do not label baseline a full profile PASS.
- No production build, push, release, stress or soak. Main integration remains
  gated by independent uncommitted main edits; see [checkpoint](../plans/combat-feedback.md).

## White CodeManu sparks (2026-10-18)

- Quick **52/52 PASS**, no failed/skipped tests, `20261018T081430-quick`,
  input `6122b54f59213aed994fde9bff82ac1b99970bf1a1ebe19fa3ba8cfe7792de00`.
  EveryCodeManuProfileReleasesOnceAndResetsItsActualAnimation and actual pause/
  hitstop animation-event tests executed and passed. All seven profiles verify
  source, white RGB, alpha-derived size,128cell/pivot64,first20ms,clip/event/reuse.
- Independent asset check confirms64/72/96/56px, exact quarter-turn permutations,
  unchanged retained prefab/controller/clip metadata/GUIDs. Original archive SHA256
  matches the accepted plan; CodeManu CC0 source/license/credits are retained.
- QA WebGL `20261018T081946-webgl` PASS; input
  `2b5fd2a683dbae905469418dbaf6e6c9304585ae0726caaf1c49185630ff648f`.
  Repeat-builder PASS; every generated asset matches local checkout. This input
  adds Unity-generated strips/metas/clips/prefabs and removes4unused Pimen strips
  after Quick; relevant runtime/test source is unchanged. Packaged CodeManu notice
  and license match source. A stale shared Library initially referenced the other
  session's PixelSparkBurst.cs; Unity refreshed/recompiled and completed the build.
- Focused native WebGL **8640checks PASS** in175.171s, exact960x540/640x360;
  Edge155.0.4283.62, Intel UHD Graphics/ANGLE D3D11, Playwright1.63.0. Actual keys
  reach normal/parry/all4Slayer/lethal at each size (mask127/127), fast combos,
  active normal+Slayer spark pause/resume and clean restart. All observed leases
  keep authored scale/order30; post-LateUpdate sorting has0mismatches (player
  Attack/Parry1, skeleton0). Each restart restores40/850HP,score0,no active sparks.
- Reviewed native25fps contact sequences and full frames at both sizes: white
  flash/rays/dispersal at existing contact, Slayer spark above silhouette, gold
  cue and typing words clear. All observed sprite rectangles avoid typing words.
  Recording playbackRate1 advances at .999956x/.999967x over2s; no time stretching.
  This is functional/visual evidence, not a device FPS benchmark or audio review.
- Fixtures:34reads/2intercepted writes;0JS/Unity exceptions, error count0. Existing
  harness blocks other external requests. No real leaderboard write. Final helper
  SHA256 `dcb801b65f56c3ecc36e7b359d46476834111b68994466db646c5209ec2114d8` (temporary harness using
  existing session/start/state/fixture helpers and real keyboard input).
- Earlier native attempts:960covered all7profiles but driver expected enemyHp0
  after destroyed enemy reports-1; another attempt sampled a spark near its end
  before pause arrived. Driver now accepts the documented terminal sentinel with
  finished/victory and retries missed pause contacts, requiring an actual held
  lease before passing. Product/timing/acceptance unchanged; final both-size run
  passes. No full WebGL/stress/soak/production build or itch publication claimed.
- Final scoped diff/whitespace/path review PASS; task captures/videos/client/cache,
  copied QA player and both owned server result folders removed after these
  receipts. Shared source/Library/toolchain/builds and other sessions preserved.
  No branch/worktree/commit/push; see combat-feedback checkpoint.

## Spark clarity and parry sorting (2026-10-18)

- Doctor PASS. `python tools/game_loop/run.py quick` PASS **52/52**,
  result20261018T072736-quick, input
  `e4f99c581d3125aa640dcac9403ae6fc9d983598aa03cad7a9a4591206d4760d`.
  Actual-controller checks cover Attack/Parry tags, attack/parry/success crossfades,
  idle/hit/death interruptions, pause/resume, disable/re-enable and captured order7.
  All7real prefabs verify48/56/64/28px scales, order30, unchanged clip duration/
  release event, actual animation completion and reused scale/color/speed reset.
- QA build20261018T072017-webgl and repeat-builder PASS on
  `2b7009e7dc7c6fbccbd28b47d695a96d4535f2798de18832fd47d2018a783553`.
  Final Quick inputs differ only by the7prefabs regenerated by that builder;
  their output bytes match local source. Other generated deltas are whitespace.
  Generated clip/controller/texture/meta bytes and GUIDs remain unchanged.
- Focused native WebGL **5314checks PASS**,171.829s, exact960x540 and640x360,
  Edge155.0.4283.62/Intel UHD Graphics/ANGLE D3D11, Playwright1.63.0. The temporary
  task driver reused browser.cjs session/start/state/fixture helpers, with
  TYPINGHERO_NATIVE=1 and TYPINGHERO_ATTACK_SORTING_ONLY=1; task helper SHA256
  `1390c33165f35871e9961a84af36973727de6039a6e3aad9fb590f800f3e3fc6`.
  Real keyboard normal hits and successful parries at both sizes; post-LateUpdate
  sorting has0mismatches (Attack/Parry1,skeleton0). Both parry states render.
  Parry pause holds order1; resume/restarts restore presentation. At640, all3
  Slayer attack states/all4spark variants, Slayer pause/resume/exit and lethal
  victory pass, ending40/0HP after11parries, spark mask127; restart resets all.
- Reviewed actual video collision frames at both sizes: orange normal/blue parry
  remain at contact; parry sword/trail draws in front of skeleton; gold cue and
  typing tray remain clear. Inspected white Slayer impact frames and lethal.
  All observed active sparks retain order30 and expected scale, with no tray
  overlap.26fixture reads/1intercepted write,0JS/Unity exceptions; all external
  page requests remain blocked by the existing harness. No real leaderboard write.
- The existing software attack-sorting driver stopped before its first normal hit
  after typing across a stale/expired word; this is not a full WebGL profile PASS.
  Native acceptance above uses the same completed QA player. An initial native
  load started before WASM output existed and timed out; rerun began after build.
  No gameplay/input/timing/camera changes or relaxed thresholds. No production
  build/itch.io publication, stress/soak or device-FPS/audio claim.
- Source diff/whitespace/GUID review and task cleanup are recorded in the existing
  [sorting checkpoint](../plans/player-attack-sorting.md). QA captures/temporary
  drivers/reports are disposable; shared Unity source/Library/toolchain/build stay.

## Player attack sorting (2026-10-18)

- Doctor PASS. Quick **51/51**, including four actual-controller/prefab sorting
  tests: all four attacks, interruption/parry/death/idle, transition destination,
  consecutive attacks, pause/resume, disable/re-enable and original-order restore.
  Input `aacc162b49e95ee8d0856ddda79571b93b5efc4d57146f4cbffdf59b30499bfd`.
- Targeted WebGL `--browser-scenario attack-sorting`: **924 checks PASS**,69.609s,
  input `a1a06d81537fe6c96d123f1e47fa393be48b768d50cc2012224f6c80aa54f1b6`.
  Real keyboard normal attack/parries/all three Slayer states (mask14), repeated
  attacks, pause/resume, Slayer exit/VFX fade, hit/death and restart. Post-LateUpdate
  observer recorded zero sorting mismatches: attacks1, skeleton0, restored player0.
- Reviewed actual normal sword/skeleton overlap before/after and all three Slayer
  poses/trails. Chromium153.0.8010.12/SwiftShader,640x360,2CPU/8GiB/shm1GiB;
  17fixture reads/1intercepted write,zero exceptions. No real leaderboard writes.
  Baseline1280x720 was too slow for parry timing (~224ms/frame); use the verified
  640x360 run as functional evidence, not player-device performance evidence.
- Initial test metadata GUID was corrected before final Quick. Initial browser
  driver checked red overlay before its exit fade ended; final driver waits for
  actual restoration. No gameplay timing changed. Runner contracts8/8 and syntax/
  whitespace/GUID checks PASS. No stress/soak/production/native run or publication.
- Task captures/generated reports cleaned after receipts; shared source/Library/
  toolchain/latest QA build remain reusable. See player-attack-sorting checkpoint.

## Impact-based check selection

Identify the changed behavior and affected dependent flows before testing. Select
the smallest checks that prove acceptance; the commands below are available
profiles, not a checklist to execute on every task.

- Do not default to full Quick followed by full WebGL. Use offline tests for
  affected gameplay contracts and real-input browser checks when the changed
  integration or user flow needs them. Add viewports only when layout, camera,
  or responsive behavior is affected.
- The current `quick` runner executes the entire Quick category and exposes no
  per-test filter. WebGL supports `--browser-scenario skeleton-art` and
  `--browser-scenario attack-sorting` and `--browser-scenario score-layout`
  (four-size near-tray HUD plus keyboard Slayer/popup/pause/restart). These do not
  cover arbitrary changes.
  When no suitable filter exists, state that limitation and choose a supported
  targeted verification path that covers acceptance. Run a broader profile only
  when its coverage is needed, not merely because it is convenient. Do not invent
  filter flags or claim a partial run passed the whole profile. Runner changes
  are not an automatic prerequisite for ordinary tasks.
- Expand coverage only for affected shared logic, a relevant failure, a concrete
  unresolved concern, or an explicit request. Briefly explain the reason before
  expanding. Heavy stress/soak needs a relevant performance, pooling, or lifecycle
  question or an explicit request, within the user's current test scope.
- Reuse passing results while relevant source, config, dependencies, and
  environment remain unchanged. Rerun affected checks after changes; stop when
  acceptance and relevant checks pass with no concrete unresolved concern.
- Use doctor when runner/environment verification is needed; setup only when
  necessary. Documentation-only changes need format/path/link and diff checks,
  not Unity or browser profiles. Report unmet acceptance as unverified; profiles
  outside the task scope do not become automatic follow-up work.

Use Python 3 from the repository root:

```text
python tools/game_loop/run.py doctor
python tools/game_loop/run.py setup
python tools/game_loop/run.py quick
python tools/game_loop/run.py webgl
python tools/game_loop/run.py stress
python tools/game_loop/run.py soak
python tools/game_loop/run.py production
python tools/game_loop/run.py windows
```

Runner targets SSH alias `fly-server`. All source snapshots (including uncommitted
tracked/untracked source), toolchain, Library/import cache and results live under
`/data/typinghero-game-loop`. Secrets are excluded: supply an activated Unity
activation state outside the repo in `/data/typinghero-game-loop/unity-home`.
Account licenses use `.config/unity3d/Unity/licenses/*.xml`; legacy ULF files use
`.local/share/unity3d/Unity/Unity_lic.ulf`. An explicit `--license` can mount an
existing matching server ULF instead. The existing GitHub WebGL pipeline uses Unity
account secrets; those are not readable from repository source and are not copied.
Do not log secret contents. Docker containers run sequentially with 2 CPUs/8 GiB.
Chromium has a private 1 GiB shared-memory mount within that memory limit.
Snapshots replace only the task source directory; persistent Library is separate.
Disk checks precede setup, import/build and browser stages. Lock prevents concurrent runs.

Setup measures and verifies the pinned image layers before import; the containerd
storage budget includes compressed content and unpacked snapshots, plus 3 GiB
headroom. Cache bytes are released as the import consumes them. Each execution
stage requires 3 GiB free initially; the task container is stopped if free space
drops below 1 GiB while running. These replace earlier unmeasured 35/12 GiB
estimates. Do not remove unrelated projects or Docker images to gain capacity.

When account activation is needed, `tools/game_loop/activate.py` runs on the server
in a trusted ephemeral Actions job with the pipeline's existing Unity account
secrets. Credentials stay in the job environment; only the resulting license is
persisted in a private task HOME. The official GameCI CLI is pinned to 0.1.72.
Administrative bootstrap helpers remain task artifacts, not a public service.
Activation and runtime use the same configured container hostname/MAC on this server.

Quick runs offline PlayMode NUnit tests with cleanup; test XML must exist and report
nonzero passing tests. Browser runs Chromium on a locally served QA WebGL build.
State comes from the conditional read-only bridge; interactions use actual input.
The QA embedding host sizes the canvas to the requested viewport and verifies Unity's
actual render dimensions. It does not establish responsiveness of the production
HTML template. Leaderboard refresh checks actual row instance IDs and Top fetches,
as well as pool size and retained scroll position.
Firebase reads/writes are fulfilled by fixtures; other external requests abort.
Stress uses large pool/1,000 word/VFX tests and leaderboard fixtures 500/1,000 rows
at 960×600 and 400×600. Soak defaults to 1,800 elapsed seconds, repeatedly completes
words and restarts rounds, checks progress deadlines and records frame time,
memory and object count. Exceptions, missing state/results, timeouts and zero tests fail.
Production builds without `TYPINGHERO_AUTOMATION` and the build hook checks exclusion.

The `windows` profile builds Windows x64 Mono using the existing Ubuntu editor
and an official2022.3.25f1 Windows Mono module mounted read-only from
`~/typinghero-game-loop/toolchain/windows-mono-2022.3.25f1/WindowsStandaloneSupport`.
Output is `/data/typinghero-game-loop/build-windows/TypingHero.exe` with its Data,
DLL and MonoBleedingEdge directories. It uses production defines and verifies
the runtime assembly excludes GameLoopProbe. Do not distribute the executable
alone; ZIP the complete player directory with its credits/licenses.

## Windows portable verification (2026-10-17)

- Doctor, runner contracts8/8 and Quick39/39 passed. Final Windows production
  build: results/20261017T080628-windows, snapshot
  251293113ae167f93a14a1377d31b07a5e6f3b38afa63d7ba8ab09d584472027.
- x64 PE headers/player/Mono dependencies, both enabled scenes, production flag,
  all118managed DLL QA-marker scans and restored player settings passed. Ten
  notices match prior verified WebGL release byte-for-byte. All951 prior local
  Assets/Packages/ProjectSettings inputs unchanged except the editor build helper.
- Wine9.0/Xvfb startup47.21s, software D3D11: live1280x720 player, MainMenu ready,
  actual frame inspected,0managed exceptions. Networks disconnected before game
  launch; no real Firebase writes. Native keyboard/combat/audio and real Windows
  hardware not tested. Wine ALSA/no-device,DXGI/touch/vsync limitations observed;
  direct speaker listening and device performance remain unverified.
- ZIP173files/33,657,976bytes; server/local entryCRC and archiveCRC PASS:
  CRC32 7aac5c0b, SHA256
  7b7720461515d3cd2ea09996cc7f00c7ec3362435e81fdffa4865f87dc36bb0c.
  Filename TypingHero-Windows-x64-20261017.zip; extract all files before launch.
  Asset/toolchain/cache/build data stay on Ubuntu; only requested final ZIPs are
  delivered locally. Previous ZIPs retained. No publishing or stress/soak.

## Skeleton art verification (2026-10-18)

- Original-alpha/weapon/VFX/native re-export checks PASS on36frames;11 approved
  PNG/native/masks remain byte-identical. Four runtime PNGs integrated, importers
  and skeleton clips/events/prefab/combat unchanged. Hurt1 baked flash unchanged.
- Doctor PASS; final Quick51/51 PASS, input
  `a1a06d81537fe6c96d123f1e47fa393be48b768d50cc2012224f6c80aa54f1b6`.
- QA WebGL build input
  `0fa4d3e9c89ea6606e4a2b2aa5c03c2627cdefaccd53c725c7cecf4b4620c45d`;
  subsequent edits are observation/driver/host-path changes, not player code/assets.
  Final targeted native WebGL32/32 PASS in122.675s: exact1280x720 and640x360,
  real typing/Hurt, cue/5HP impact, parry/Stun/Slayer, death/cleanup, pause/restart.
  Edge155.0.4283.62/Intel UHD Graphics D3D11, Playwright1.58.2 reused locally;
  28fixture reads/2mock writes,0exceptions; external page requests blocked.
- Runtime16frames reviewed: bright bone/gaps, mirror, weapon/slash, original
  flash/Slayer silhouettes and Die/restart. All36 source frames reviewed separately.
  Driver SHA256`b0a3c67081ad9241b824c407c77f24a7bbe613cc6f7f124e09b50d4fabdd7468`.
- SwiftShader2CPU/8GiB attempts are failed measurements, not a full WebGL PASS:
  a409.355ms frame yielded sampled cue lead.604345s, exceeding.6s. Thresholds
  unchanged; final native lead samples.261480/.260160s. Driver fixes synchronized
  pause after Animator, removed capture from timing measurement and avoided empty
  promotion/hit-locked input. No device-FPS, release,stress or soak claim.
- Full receipt, source ownership and temporary cleanup: skeleton-hero-art plan.

## Baseline

No completed soak baseline. The user stopped heavy testing during the post-fix
soak at approximately 1,355 elapsed browser seconds (51 rounds, 750 completed
words observed). This is partial diagnostic evidence, not a 1,800-second pass.
Before the core-match task, the production QA-exclusion build was cancelled before starting; the completed core-match production result is recorded below.
Select checks using the impact-based guidance above; full Quick/WebGL are not
defaults. Heavy profiles are opt-in or justified by the affected behavior,
within the user's current scope.
First successful full soak, when requested,
must record commit/snapshot hash, image digest, CPU/RAM limits, Chromium version,
viewport, elapsed duration, samples/progress, frame percentiles, memory/object
range and exceptions. Headless software rendering measures only regression under
the same environment; it does not define player-device FPS acceptance.

The QA probe samples frame time, allocated Unity memory and loaded GameObject count
every 100 ms; browser reports retain approximately one sample per second. Its scene
queries and allocations affect measurements. Compare instrumented runs with the
same toolchain/limits rather than extrapolating to a production build.

## Initial environment (2026-10-16)

fly-server: Ubuntu 26.04 LTS, kernel 7.0.0-34-generic, Intel Core i5-4570 at 3.20 GHz
(4 logical CPUs), Docker 29.7.2, host RAM 15 GiB, `/data` free 22 GiB initially.
Docker-managed files use `/data/docker`; containerd image contents/snapshots use
`/data/containerd` (verified from active config), on the same filesystem as task cache.
Unity 2022.3.25f1 and Test Framework 1.1.33 unchanged. Unity image imported and
account activation succeeded in the trusted ephemeral Actions job:
https://github.com/micharel09/TypingHero/actions/runs/38856603799
License remains only in the private server HOME. The ephemeral runner deregistered
and its binaries were removed after the job. GameCI requires a disposable empty
Git repository in the activation snapshot; the user checkout is never initialized
or committed by this bootstrap.

Quick passed all 10 actual PlayMode cases after the hitstop regression fix.
The new regression failed before the fix (expected original scale 0.75, actual
0.0001) and passed afterward with stacking both enabled and disabled.
WebGL passed 13 checks after that fix, including Slayer and round restart.
Stress passed 3 Unity tests and 34 browser checks across all four dataset/viewport
combinations. Its typing/VFX/leaderboard inputs are unchanged by the hitstop fix.
Latest verified WebGL/partial-soak snapshot:
`c9c37e2ebb05beaf51bb92de531a7644dc313548bafa5fb769b9d3d91b2c5fdc`.
WebGL initially stopped at the disk preflight (2.5 GiB available, 3 GiB required).
Removed only task-owned duplicate extracted global package cache after verifying
all packages exist in retained Library/PackageCache, plus the completed ephemeral
runner. This restored 4.2 GiB; WebGL resumed. npm download archives, Library, Unity
image and toolchain remain cached. Full soak and production remain unverified,
following the user's instruction to stop heavy testing.

Infrastructure checks passed: skill validator, relative links, asmdef/metadata,
Python/Node syntax, whitespace and eight Ubuntu runner-result contract tests.
Pinned image storage measured: 4.79 GiB compressed + 9.94 GiB uncompressed layer
tar bytes. Actual Docker 29 OCI import validates canonical image name and exact
manifest ID. Public transfer caches are no longer served; no service was installed.

Browser toolchain smoke passed with the actual 2 CPU / 8 GiB / 1 GiB shared-memory
limits and network disabled: Playwright 1.63.0, Chromium 153.0.8010.12, WebGL2 via
ANGLE Vulkan 1.3.0 SwiftShader (Subzero). Browser image digest:
`sha256:eff16c30e6f3f4af0a03fa4b706120d5e9b0891c344a27d64559aff5900a4a27`.
This smoke verifies browser/graphics prerequisites; actual game profile results
are recorded above. Temporary test reports/build captures were cleaned after
recording conclusions here. Persistent server license/toolchain/Library and the
captured source remain under `/data/typinghero-game-loop` for future runs.
See `docs/plans/typinghero-game-loop.md` for the resume checkpoint and retained server directory.


## Core match acceptance (2026-10-16)

Quick: 26/26 passed on runtime snapshot
`4e1dce5e4580dd356fd45273217ef22f4183dfae2804acb7719a6eeb5fc5f08f`.
Final WebGL: 54 checks passed on snapshot
`c0ecf7d502deb7718350596766730c74ffbb925609ccb0a3ee4441a948c45a5f`.
Only enemy HP/editor serialization changed after the Quick runtime checks.
Three actual keyboard rounds at850HP won in86.524/94.404/90.706 combat seconds,
median90.706s,44.716–44.935WPM,94.334–95.266%accuracy,78.571–81.250%successful parries.
These are reference balance runs, not novice human playtests.

Chromium153.0.8010.12/SwiftShader, 2CPU/8GiB, shm1GiB, network none;
Firebase fixtures cover reads/writes. Read-only probe/frame sampling affects timings.
Real focus loss was checked separately in headed Xvfb Chromium (7 checks passed);
headless focus emulation is not evidence of real focus loss.
Edge61a46c05/155 was inspected at960×540,1280×720,1920×1080,
iframe960×600 and native iframe fullscreen1280×720. No stress/soak rerun for this task.
See [core-match checkpoint](../plans/core-match.md) for acceptance, fixes, HP selection,
fixture incident/remediation and production artifact status.

Production `61ba8e37bdda95125cb1d376a198b00a2d5c612e9111f6dd960f4582b1347e14` passed QA bridge exclusion (qa=false and all JS scanned). ZIP CRC passed; artifact path/hash are in the checkpoint.

Production smoke on Edge61a46c05 passed first-help, pause/resume, defeat25.5s/no-letter accuracy, restart and menu return. Bridge is undefined in browser; Firebase PATCH had mock intervention applied. The only capture error was favicon.ico404 on the QA cleanup page, not a runtime exception. Task captures/hosts/output directories were cleaned after writing receipts.

## Dark Ages UI verification (2026-10-16)

- Final Quick:26/26 PlayMode cases, source snapshot
  `1108a5ebd9b76c1b25b6afb2a82ec3ba9d2cc9dfa3d1a11c49c0d6bb34c3b9ad`.
  Final serialized scenes/prefabs include the role-specific skin and preserved
  original scene IDs, prefab GUIDs and40/850HP values.
- Full WebGL regression:54 checks,381.562s, snapshot
  `886bc0f608d0602e34d81348c327b4309f5b0aff1af1edaa6ec66fe45ef77421`.
  Runtime C# and gameplay configuration unchanged by this skin task; three850HP
  reference wins88.402/91.465/93.900s. This predates appearance-only serialization
  fixes; the final visual integration checks below verify those fixes.
- Final visual/interaction profile:21 checks, QA source
  `6ff902abd1477b6266314f91b2343e3491f41f36a8cf2b951f965fa7c5dcedc3`.
  Reviewed960×540,1280×720,1920×1080,640×360, quiet HUD/typing, native ornate
  corners/tiled edges, button states, full/half/empty bars, help, defeat/victory,
  restart/menu and actual Unicode keyboard entry/persistence of Nguyễn.
  Visible-row pool19 for1,000 long-name fixtures; native wheel/thumb to rank1000,
  ellipsis, column alignment and refresh retaining scroll verified.
- Supplementary iframe/fullscreen:5 checks on the same QA build; native name entry,
  Enter/help/game/pause at960×540 and native fullscreen1280×720, images inspected.
  Host fullscreen-button focus must be returned by a native canvas click so the
  iframe player resumes publishing actual render dimensions. An earlier splash
  capture and an earlier insertText-only attempt were rejected as visual/input evidence.
- All browser profiles used Chromium153.0.8010.12/SwiftShader on the existing
  isolated2CPU/8GiB/shm1GiB runner. Firebase reads/writes fulfilled by fixtures,
  external network blocked; no JS/Unity exceptions. Measurements are not device FPS.
  QA embedding host supplies viewport sizing; production HTML template unchanged.
- Repeat builder passed exact serialized scene/four-prefab comparison after two
  Apply calls. Direct prefab changes now record overrides, and reopened visual
  inspection confirms HP remains red and HUD/typing use the dark inset.
- Required native Edge instance61a46c05 is disconnected in BrowserSkill; exact-profile
  QA remains unverified. No stress/soak or itch.io publication in this task.
- Production passed from the final Quick snapshot above; qa=false and all JS exclude the bridge. ZIP C:/Users/ADMIN/Downloads/TypingHero-WebGL-dark-ages-20261016.zip:23 files,15,859,154 bytes, local/server CRC passed, SHA256 7b6ea706fbed516d91223e6cb95fb0fce9c26d52fae38365c205ff0ca48b118. Credits/four licenses included; original core-match ZIP unchanged. See the existing pixel-ui-refresh checkpoint for retained native-Edge QA resources.

## Combat feedback verification (2026-10-17)

- Final build-input SHA256: `f3146419f10dcfd1f9e42bb9237c61913d3958c112ef59945374b68d08b3111e`. Quick **35/35 PASS**;
  WebGL **54 checks PASS**, 378.643 seconds. Repeat builder
  passed exact serialized comparisons, including all new Pimen role assets/meta.
- Final server regression: Chromium153 / SwiftShader, 2CPU/8GiB/shm1GiB,
  viewport960x600; 104 fixture reads / 5 fixture writes, zero JS/Unity exceptions.
  Three 850HP reference wins: 90.295 / 92.321 / 89.434 combat seconds.
  These are functional automation results, not player-device FPS or human feel.
- Native Windows Chromium153, Intel UHD Graphics via ANGLE D3D11: **16 checks PASS**.
  960x540,1280x720,1920x1080,640x360 plus actual iframe/fullscreen, real typing/
  parry/Slayer victory, pause/resume, three restarts at40/850 and menu return.
  20 fixture reads / 1 fixture write, zero exceptions. All network writes intercepted.
- Actual original Pimen sheets, seven one-shot prefab clips and four white Slayer
  derivatives verified: timing100/150/180ms, Point/uncompressed WebGL import,
  alpha preserved, pool release/reset and pause/hitstop/exit/unload behavior.
  Native frame review corrected the old parry anchor to the sword crossing.
  Normal ivory flash/contact, Slayer silhouettes and larger lethal burst verified.
- Impact mute/gain/pitch/pause and separate whoosh/parry voices passed PlayMode.
  **Direct speaker listening remains unverified.** No stress/soak or publication.
- Production PASS, qa=false; QA markers absent from JS/wasm/data. Credits and all
  licenses included. ZIP CRC passed locally and on server. Production ZIP SHA256:
  `f4e64cdbf60ec47616bff549046c6a1984c47e3f2709b62685b07f5d71f97cdd`.
  [WebGL production](C:/Users/ADMIN/Downloads/TypingHero-WebGL-combat-feedback-20261017.zip) and
  [Unity source ZIP](C:/Users/ADMIN/Downloads/TypingHero-Source-combat-feedback-20261017.zip); adjacent SHA256SUMS file
  covers both archives. Prior Dark Ages ZIP retained byte-identically.
- Portable Quick/WebGL/native reports and four actual-render reference frames are
  included under `Verification/` in the source ZIP. Persistent runner evidence:
  `/data/typinghero-game-loop/results/20261017T052707-quick`,
  `/data/typinghero-game-loop/results/20261017T052751-webgl`,
  `/data/typinghero-game-loop/results/20261017T053514-production`. Task temp captures/baselines are removed after packaging;
  reusable runner source/Library/toolchain and result history are retained.

## Shogun presentation verification (2026-10-17)

- Runtime Quick39/39 PASS,snapshot
  fa0f612f9db3a323d0e692e5432ddea4d88b3ee10a4e96d50483471ee82d7592.
  Includes original35 cases,modal focus/hidden-canvas guard,actual UI clip/mixer
  routing,pause-safe voice return/reuse,and hidden pause Submit regression.
- Full WebGL66/66 PASS,390.482s,input
  1f59c5f2c967aadf36b1c6bb090fbfb4045cacecd6d62ad62be4f9a718175379.
  Chromium153/SwiftShader,2CPU/8GiB/shm1GiB,960x600;63fixture reads/5writes,
  zero JS/Unity exceptions. Native scrollbar reaches rank1000; refresh retains
  bottom scroll/pool; modal R/M/Escape/focus ownership and pause hold verified.
  Three850HP reference wins90.505/94.278/83.489s; no balance change.
- Native Windows Chromium151.0.7922.34,Intel UHD ANGLE D3D11:34checks PASS,
  23fixture reads/2writes,zero exceptions. Real typing/parry/Slayer victory,
  defeat,3restarts40/850,errors/timeout,menu/help/pause,4requested resolutions,
  actual focus loss/return,modal1000rows,iframe/fullscreen. Additional6checks
  verify actual gameplay960x540 iframe,1280x720 fullscreen,resume and a keyboard
  slash causing damage.4fixture reads/0writes,zero exceptions.
- Native frames verified jade/coal layers,dark natural edge trees,clear contact
  orange/blue and retained sword trails,cue,typing hierarchy and modal row1000.
  Original sprite profile timing/scale/pool tests remain passing. Direct speaker
  listening is UNVERIFIED; no player-device FPS claim,no stress/soak or publication.
- Final presentation-only deltas after full regression:zero fill inset fixes thin
  bars and obsolete SpriteSwap references are cleared from ColorTint controls.
  Runtime/tests are unchanged. Final serialized builder ran twice and compared
  103 files byte-for-byte:PASS. Input99161950a389a82e1af8792a59fed22077b21a045a5752a9608da59f7e86873b.
- Production inputc5066e5a6d455c309d17e8f1767aff660872ef5c3acddddacfecbc1556c092a1:
  PASS,qa=false; bridge markers absent from JS/WASM/data. Native normal-build
  smoke8checks PASS without a bridge,3fixture reads/0writes,zero exceptions;
  menu/help,4render sizes,pause/modal/close/resume,combat input and menu return.
  Real frame pixels confirm visible HP/stamina/timer fills after inset correction.
  A first smoke driver acted before Unity's first UI frame; it was corrected to
  wait for actual rendered title/HUD text,without changing product code.
- Extra iframe driver initially assumed default2x pause zoom; the serialized scene
  owns1.1x/295ms. Corrected the driver to wait for actual iframe focus/transition;
  product pause settings unchanged. The final gameplay iframe6checks passed.
- Production ZIP28files/15,945,248bytes,server/local ZIP CRC PASS; archive CRC32
  b64fbfdb;SHA256b5e56079314c73fac75e20495db47a4a2e77897fa793f46a81aab21ebced53b3.
  Deliveries: C:/Users/ADMIN/Downloads/TypingHero-WebGL-shogun-20261017.zip and
  TypingHero-Source-shogun-20261017.zip; adjacent
  TypingHero-shogun-20261017-checksums.txt records SHA256/CRC32 for both.
  Source includes credits/licenses,approved wireframes and these written receipts;
  runtime captures are temporary and are not copied into the source archive.
- Persistent runner receipts: /data/typinghero-game-loop/results/20261017T065257-quick,
  20261017T065419-webgl,20261017T070900-serialization,20261017T071107-production.
  Reusable toolchain/Library/results retained. No new branch/worktree/commit.

## Interactive tutorial verification (2026-10-17)

- Doctor PASS with pinned Unity2022.3.25f1 and existing activated Ubuntu toolchain.
  Final runtime Quick44/44 PASS, input
  `957b6cfdee5109b03eb4f43f8f2c9727e7016510022828b06979a9b1034e6935`.
  Runner contracts8/8 PASS on Ubuntu; JS driver syntax checks PASS.
- The same QA build passed the repeat builder's exact serialized comparison.
  Full browser rerun120checks PASS in548.093s: all6lessons, actual hits/parries,
  mistakes/timeout/hit retries, Slayer expiry/restart, skip/replay/preferences,
  reading/pause locks, clean Start Battle, ordinary defeat/victory, full1000-row
  modal and native iframe/fullscreen keyboard interaction.117fixture reads/5normal
  match writes,0tutorial writes,0JS/Unity exceptions. Three850HP reference wins
  91.709/90.535/86.424s. Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB;
  these functional timings are not player-device FPS or human-playtest evidence.
- An earlier final browser attempt passed118checks, then clicked stale Resume
  coordinates immediately after fullscreen resize. The corrected driver waits
  3actual layout frames and the end of Resume's camera transition. Accuracy's
  wrong-key assertion likewise waits for observed input. No game timing, window,
  expectation or passing threshold changed to accommodate the driver.
- Native Edge155.0.4283.45/Intel UHD ANGLE D3D11:55checks PASS, including3actual
  browser focus losses during Parry/Slayer.38fixture reads/0writes,0exceptions.
  Four requested render sizes and actual960x540 iframe/1280x720fullscreen were
  reviewed: full yellow cue, gold focus borders, complete secondary menu labels
  and unobscured characters. Additional same-build native27checks PASS in203.771s:
  fresh-context real BLADE input causes exactly10damage at960x540,1280x720,
  1920x1080and640x360; reading blocks Space/typing at each size.8fixture reads,
  0writes,0exceptions. All four actual hit frames were inspected.
- Serialized MainMenu/SampleScene retain all262/563original document IDs and
  unchanged GUIDs, adding5/89IDs. Other10generated deltas were whitespace only
  and were excluded. Unity/package versions and normal40/850HP/250stamina remain.
  Wireframe r2fingerprint:
  `1e1148f8566a16a8172d1ec0cf8f9d8a71c408f3bed0c8292ef3962b2ec8afa0`.
  Both saved wireframe revisions passed their render/check workflow.
- WebGL production and Windows64 build PASS from identical input
  `ef5da24af7d8f9a5623472d76841f960d9d0d6008f8cb7e0d755637874519ed9`.
  This adds the already-tested serialized scenes and driver-only changes to the
  Quick/QA input; runtime C# is unchanged. Production flags are false; all5WebGL
  JS/WASM/data files and118Windows managed DLLs exclude the QA markers. PE64
  checks pass for executable,UnityPlayer and Mono runtime. Ten credits/licenses
  match byte-for-byte across both players.
- Production browser smoke without a bridge:5checks PASS,7fixture reads/0writes,
  0exceptions. Actual reading/BLADE-complete frames, ordinary pause,1000-row
  leaderboard and returned menu reviewed. First menu capture preceded the first
  UI frame and is rejected as visual evidence; returned menu verifies the layout.
- WebGL ZIP28members,15,953,468bytes,CRC32b89efbbe,SHA256
  `12ec4286fafd69d5a0c1e49272cca9530ea6ec4a6221950c8b0db06118a0c1f0`.
  Windows ZIP174members,33,666,777bytes,CRC32b35fa287,SHA256
  `4aaa37dfc6279104842b1a662536f997391e0ce5faed03f4b235cbdedc53a634`.
  Server member CRC checks passed for both. Downloaded Windows ZIP matches SHA256,
  archive CRC32 and all174member CRCs; the complete executable/Data/Mono/DLL set
  and production metadata were verified locally without extracting assets here.
- Windows ZIP was extracted and launched offline under Wine6.0.3/Xvfb1600x1000:
  actual1280x720 window,D3D11featurelevel11.1,menu/first practice/BLADE-hit frames
  reviewed; score10and ATTACK COMPLETE verify the real hit.0managed exceptions.
  All container networks disconnected before launch; the disposable container
  was removed.233.462s includes Wine package setup and fresh-prefix initialization,
  not player startup time. This does not establish real Windows hardware QA,
  device FPS or speaker output. Direct speaker listening remains unverified.
- Delivery: new Windows ZIP and TypingHero-tutorial-20261017-checksums.txt in
  C:/Users/ADMIN/Downloads. Ten existing TypingHero ZIPs are preserved,including
  the earlier Windows ZIP SHA2567b7720461515d3cd2ea09996cc7f00c7ec3362435e81fdffa4865f87dc36bb0c.
  WebGL and Unity source archives remain only on Ubuntu under
  /data/typinghero-game-loop/releases/tutorial-20261017. The adjacent checksum
  receipt records SHA256/CRC32/bytes for all3archives. Source contains project
  files,credits,approved wireframes and written verification; no caches,builds,
  secrets or runtime QA captures. No itch.io publication or stress/soak.
- Run identifiers: Quick20261017T113032,QA build20261017T113623,production20261017T120740,
  Windows20261017T121315; browser rerun used the unchanged QA build with corrected
  observation-only helpers. Task captures/generated reports/clients and temporary
  hosts are cleaned after these written receipts. Normal source/Library/toolchain/
  build output and release archives stay on Ubuntu; no new branch/worktree/commit.

## Slayer cinematic verification (2026-10-18)

- Doctor and runner contracts8/8 PASS. Quick46/46 at20261018T033558 includes
  score award-only pulse/configured tiers and zoom exit before camera completion.
  Quick snapshot bcd4c310d0f27d734e031e9ae8d42943cffd3a46494dac7fe893354e7747721c.
- QA WebGL20261018T033806 built input
  29dba92a69cbd61c5a6866c7fe982be49549cfdf12bdd0c2c1deb7600f865186.
  Final standing-head inset .28 (versus earlier Quick .55) and serialized bindings
  were verified in this player. Repeat-builder PASS; all652 original scene IDs and
  GUIDs retained,659 total IDs. Local scene matches Unity output after whitespace
  normalization. Subsequent C# delta is trailing whitespace only.
- Browser broad rerun completed134 checks: six tutorial lessons/retries, pause,
  restart/clean battle, ordinary combat/defeat, preferences, modal/embedding and
  three850HP wins91.195/84.933/88.018s. Then its x3 driver expired Slayer while
  waiting/capturing between words, so its overall status is failed, not a full
  profile PASS. That driver-only failure was rechecked separately below; none of
  the already passing gameplay paths or their acceptance thresholds changed.
- Body gap assertions and actual images reviewed at960x540,1280x720,1920x1080,
  640x360: top/bottom gaps8 reference pixels (largest float error .000016), full
  render-width bars, original red background, readable tutorial/words/corner HUD.
  Initial640 resize assertion sampled stale1920 canvas scale; driver now waits
  four actual frames after the observed render-size change. Same QA payload passes.
- Corrected standalone Slayer browser PASS30/30 in50.575s: real keyboard parry/
  typing reaches x2 yellow and x3 RGB(1,0,0), total287 at x3 matches each observed
  character award using existing float32 multiplier and ties-to-even rounding.
  Zoom1.282051; award pulse observed, idle combo does not pulse; pause keeps actual
  score, bars span all4sizes, exit restores environment/panels/scale and restart
  resets the score/layout. Reviewed x2 popup, x3 and restored normal frames.
- Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB. Broad123fixture reads/6mock
  writes; targeted10reads/0writes; no JS/Unity exceptions. All external networking
  blocked; tutorial produces0writes. These are functional checks, not device FPS.
- User deferred release/ZIP. No new production/Windows build or release archive;
  previous ZIPs remain untouched. No native Windows/audio or stress/soak claim.
  Runtime build/source/cache remain on Ubuntu. Task QA captures/reports are removed
  after recording this receipt; corrected drivers remain in tools/game_loop.
- Final source synchronization/Doctor snapshot:
  `6adde6ec8a9bf33e6a8d63b1abb3fe9022a86f80b080893c783a76b67bda7c5d`.
  This includes the actual serialized scene and corrected test drivers; runtime
  logic matches the QA player above. Final Slayer driver SHA256:
  `f8ba00cb8c8c3cbe1852133929eaa3100eef5025c836dc80b11335ae2a3dcab3`.
  All11prior local ZIP SHA256 values unchanged; diff whitespace checks PASS.

## Foot-local combat HUD verification (2026-10-18)

- Doctor PASS before/after; Quick47/47 PASS20261018T041510, snapshot
  `51fe66ab40ef29a8b787fe85a8a8d2f6c37727a7736510e458dc9f4606df6a84`.
  Includes camera-projected feet and centered zoom1/1.282 plus pulse1.2 score.
  Product runtime is unchanged after Quick; subsequent changes are editor layout
  cleanup, actual scene serialization and QA-only observation/driver fixes.
  Runner contracts8/8, JS syntax, whitespace and wireframe r1/r2 checks PASS.
- Final QA player20261018T042543, input
  `ac4c2952c7e5ca6e77e5995702a4549ff276fda7034e6acf32d41488096281cd`.
  Repeat-builder PASS on both builds. SampleScene retains all659 previous IDs,
  adds5/664total, removes no referenced GUID and adds only CombatHUDLayout's GUID.
  Normalized local scene matches the final Unity serialization, SHA256
  `5cd400538351b13aae9669c9065cbc949ad8622c8014f9781d3d18ba9699fc36`.
- Both normal/Slayer at960x540,1280x720,1920x1080,640x360: measured112px status,
  foot gap12, centered672x56 tray/gap12, measured score/multiplier center/top32,
  three words inside tray and HP/stamina fills matching state. Eight real frames
  plus stamina25/250 highlight reviewed; no text clipping/body overlap. Full-width
  bars retain8px gaps; largest measured error.000016 reference pixels.
- Broad player run completed181checks, including six lessons/retries/Slayer
  expiry, pause/restart, ordinary real HP damage/fills, eight-hit defeat and result
  freeze/restart.86fixture reads/2mock writes,0JS/Unity exceptions. It was explicitly
  stopped during the first balance round; it is NOT a full WebGL profile PASS.
  A previous142-check attempt exposed a QA observer dereference after actor
  destruction. Guarding both colliders fixed it; later defeat/victory/restart
  observation continues without exceptions. No product timing/threshold changed.
- Targeted real-input victory against850HP:79.9combat seconds,score2798,
  accuracy98.7%,9parries; actual result frame reviewed. Restart restores all three
  bars,zero score and normal HUD. Targeted run completed12checks before its later
  x3 driver timed out;28fixture reads/2mock writes,0exceptions. The observed yellow
  x2 popup is correctly placed beside the centered score. Capturing it inside the
  short typing window delayed input; remove that capture from the live input loop.
- Same-player standalone Slayer rerun PASS27/27 in46.307s: x2 yellow,x3 pure red,
  score275 matches every observed character award/float rounding,zoom1.282051,
  award-only pulse,idle no pulse,pause at all four render sizes,full-width bars,
  exit restores environment/tray/white score and restart resets presentation.
  Reviewed x3 and exit/error-border frames.9fixture reads/0writes,0exceptions.
  Final slayer.cjs SHA256:
  `880bac734fb635dba2fd0c84926e049a93b047f784f0dc41b3a0b775a2583c4d`.
- Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB,external networking blocked
  and Firebase fixtures retained throughout. These are functional/layout results,
  not device FPS or native/audio verification. No stress/soak/release/ZIP build.
- Final synchronized Doctor snapshot
  `1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588`:
  same product runtime/serialized scene,final QA drivers. Wireframe r2 fingerprint
  `902c0ea39127ccbe8d51208f1bca6fd1c1656b9cdc58dc9172f2dcfa2e690c35`.
  See combat-hud checkpoint for scope and cleanup; source/Library/toolchain/QA
  player remain reusable on Ubuntu. No branch/worktree/commit/push created.
  Four task result directories and combat-hud task-temp/captures on both hosts
  were removed after recording these receipts; design source revisions remain.

## HUD production publication (2026-10-18)

- User subsequently authorized itch.io publication. Production20261018T044548
  built verified input1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588.
  Normal-build QA exclusion PASS; every Build JS/WASM/data file scanned for
  typingHeroQA/TypingHeroQA_Publish/GameLoopProbe markers: absent.
- Original production HTML smoke on Chromium153/SwiftShader,2CPU/8GiB/shm1GiB,
  no network: native name/Enter,real BLADE hit and score10/ATTACK COMPLETE,
  pause/resume,next lesson and640x360 HUD reviewed. Also renders1280x800.
  17fixture reads/0writes/0exceptions; bridge undefined. No product code changed.
- ZIP28members/15,981,634bytes,CRC PASS; downloaded SHA256 matches server:
  `be81e04bb9163ee4cda8c46e16fac12e50e8d976e27761d8986fc947e2cbdeee`.
  Release and per-file manifest retained at
  `/data/typinghero-game-loop/releases/hud-20261018/`.
- Published and saved to https://micharel09.itch.io/typing-hero, game3857509;
  new browser-playable upload19839230. Three prior uploads retained and hidden;
  existing maximized embed/theme/copy/pricing and draft devlog preserved.
- Public Run game renders the menu. Anonymous hosted payload checks match all27
  non-HTML file hashes; index.html only adds itch.io's standard htmlgame.js script.
  No public gameplay/provider writes. Task captures/results cleaned; actual
  server release ZIP/manifest retained. See itch-discovery for publication receipts.

## Score near typing tray (2026-10-18)

- Quick52/52 PASS20261018T063624, input
  `24893dfb136c6a6d19e372ff4413b6d7cca8784b3d333de555d6d98036790d84`.
  Layout regression covers0/6/10digits,x1 hidden/x2/x3,pulse1/1.2,zoom1/1.282;
  fixed tray,centered text,decay within48px and popup gap12. Moving-anchor award
  regression also confirms fade remains half elapsed after relocation.
- QA WebGL20261018T063120,input
  `70c98ea4c61ba45e02061ec219cc459a290b6a5d9901a45ea1b6510952085d85`;
  repeat-builder PASS. Local normalized serialization matches its SampleScene
  (664/664IDs) and floating prefab(5/5IDs),with original GUIDs intact.
- Final targeted score-layout browser PASS166checks in171.817s on the same player;
  corrected JS drivers, no runtime rebuild. Actual tutorial keyboard hits/parries,
  read/pause input locks,normal/Slayer projection at640x360,960x540,1280x720,
  1920x1080; all eight layout frames reviewed. Real ordinary Slayer typing reaches
  x2 yellow/x3red,score275 matches character awards/rounding,pulse then idle,
  font18 award follows anchor,pause holds score,exit restores/restart resets.
  Actual popup and restored normal frames reviewed; no HUD/text/timer overlap or
  gameplay tray overflow. Fixed row reserve holds at maximum tested pulse/zoom.
- Measured center-to-center score-word gap70px normal/66.615px Slayer at all sizes
  (top-to-top60px). Minimum tray bottom43.142px normal/18.080px Slayer.
  Calculated old-layout center gaps from the same observed camera/body projection
  and previous top32/tray-gap12 formulas:368.466-368.858px normal,
  389.469-389.971px Slayer. This baseline is reconstructed geometry,not a before
  screenshot or eye-tracking measurement. New gap is below80px.
- Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB,45fixture reads/0writes,
  zero JS/Unity exceptions; external networking blocked. Functional/layout evidence,
  no device-FPS/native/audio claim. No production build,release or itch publication.
- Earlier browser attempts failed on driver sampling completed tutorial words,
  measuring hidden HUD under the pause camera,then letting Slayer expire while
  taking active screenshots. Final geometry is measured in populated reading states
  after real combat activation; functional Slayer runs separately within the same
  scenario. Pause camera's existing zoom/focus can put the underlying foot-following
  HUD outside the frame under its overlay; resume restores gameplay projection.
  No gameplay timing,fixture,acceptance threshold or product camera changed.
- Wireframe r3 and preserved r2 checks PASS; r3 fingerprint
  `49ad851305cd8144427738228e5136278d376a2644057e7d6a4b394ef0469df6`.
  Runner contracts8/8,driver syntax and task whitespace checks PASS. Written
  receipts remain here/ combat-hud plan; task captures/reports are removed after
  review. Reusable server source/Library/toolchain/QA player remain.

## Score HUD production publication (2026-10-18)

- User subsequently requested itch.io deployment. Production20261018T064806 PASS
  from verified input `e49808a9fe7f779d2bec93160e240e07362bdc002aadec015b3d30a7a56489db`.
  All Build JS/WASM/data exclude typingHeroQA/TypingHeroQA_Publish/GameLoopProbe.
- Exact production HTML, Chromium153/SwiftShader,2CPU/8GiB/shm1GiB/network none:
  real name/Enter/BLADE hit,score10/ATTACK COMPLETE,pause/resume,next lesson and
  640x360 render reviewed. QA bridge undefined;5fixture reads/0writes/0exceptions.
- ZIP28members/15,957,828bytes,CRC PASS; local/server SHA256
  `237151c63e41baa2a64676d026d2e1b19fd6e6383d8f70a61500251c8272d558`.
  Published to https://micharel09.itch.io/typing-hero,game3857509,new upload19840745;
  native Saved confirmation and saved playable/maximized state verified. Four
  previous uploads retained hidden; storefront copy/theme/pricing/devlog untouched.
- Public Run game renders the actual menu; anonymous CDN check matches all27
  non-HTML files. HTML differs only by itch.io's htmlgame.js integration script.
  No public gameplay/provider writes. Release ZIP/release.json/published.json stay
  at `/data/typinghero-game-loop/releases/score-hud-20261018/` for rollback.
  Task captures/reports/results cleaned after written receipts; owned browser
  session closed,shared source/cache/toolchain/builds and browser daemon retained.
