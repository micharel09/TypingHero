## Remove invisible cool floor fill (2026-10-27; acceptance pending)

- Continued the existing clean Orca worktree at
  `C:/Users/ADMIN/orca/workspaces/TypingHero/replaced-dungeon-lighting-2`, branch
  `micharel09/replaced-dungeon-lighting-2`, from `ce57fbf7`; origin was fetched and
  destination remains `micharel09/TypingHero`. Dirty primary main is untouched.
- Deleted ColdVaultFill GameObject, Transform, DungeonTorchLight, URP light data,
  Light and its parent child-list entry. Deleted DungeonCoolFill.asset/.meta only
  after confirming the scene was its sole active consumer. Historical evidence
  under docs/plans/evidence remains intact. Current configuration docs updated.
- DungeonFrontLantern profile intensity2 ->16 matches the authored MainLantern.
  OnEnable applies the profile then retained Perlin flicker: base16, range
  15.04..16.96 (amplitude.06), rather than a fixed16 every frame. Transform,
  shadows, flame, phase, five remaining fixture components and combat lights are
  unchanged. No ambient/exposure/shader/floor-material/API/gameplay compensation.
- Static PASS: exactly5 scene YAML documents removed; the only retained document
  change removes the parent child entry. All other scene documents match HEAD.
  Five DungeonTorchLight components remain; rg across Assets finds no deleted
  name/profile/GUID/fileID consumer. The front profile's sole delta is intensity.
- New canonical build/performance source lock:
  `6bed25a62764729c94188f299e3bedc666bd4281d4bc7cc532119428e8b09ade`.
  Earlier QA A/B/production players are historical; do not reuse their timing as
  the new lighting baseline. Locking source is not a performance qualification.
- Default shared runner doctor twice returned EAGAIN on its occupied lock; no
  environment setup or job termination. Reused the task-owned Ubuntu adapter and
  pinned Unity2022.3.25f1, existing private license/cache. Build result/log:
  `/data/typinghero-game-loop/tasks/dungeon-performance-20261027-final/results/20261027T100208-webgl`.
  Import was delayed by observed60-64% I/O wait, not disk exhaustion (>40GiB free).
- Evidence root:
  `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-no-cool-fill-20261027`.
  Build input/adapter/console are retained alongside prior history at
  `dungeon-ready-20261027/20261027T100116-build-candidate`.
- Ubuntu QA100208 build and strict repeated-builder PASS, exit0. All505 downloaded
  files (33player/472serialized) match server SHA256; archive
  `8fe533635b9bc8dca1b92bd6b25a91c12ee384ab6edf831f6a92787b95933a73`.
  Serialized DungeonWorld retains exactly5fixture components, MainLantern
  m_Intensity16, and no removed name/GUID/child fileID. Builder does not recreate fill.
- Native Chromium153.0.8010.12, NVIDIA GTX1650Ti/ANGLE D3D11,1280x720:
  player `61b46a73877b0e0f`, **397checks PASS**, two real-input wins, full/partial
  READY, Pause/Resume, Restart to first encounter and observed Slayer entry/exit.
  0exceptions/provider reads/provider writes. Context closure reports closed.
  The smoke adapter bypasses registration/foreground wait markers explicitly;
  this is functional evidence, never actual-Windows-foreground or FPS evidence.
- Directly viewed menu.png, combat.png, slayer.png and slayer-exit.png. Menu and
  combat show warm floor response with no former blue floor pool. The exit still
  is an in-progress red/black transition, not proof of fully restored world.
  No matched-camera/pose old-new pair or full transition playback was inspected;
  central-lantern visual dominance, complete exit restoration and direct numeric
  OnEnable observation remain **UNVERIFIED**. Source proves the base16/retained
  flicker path; it does not replace runtime measurement.
- The source lock above is the new reproducible lighting baseline input/player;
  its performance is **UNQUALIFIED**. No new timing numbers, A/B selection,
  official ABBA or accepted production baseline are claimed. FPS>=59,
  p95<=18ms, minimum sliding1s>=40FPS, actual Windows foreground, formal ABBA,
  production checks and final review remain mandatory before main integration.
  No itch.io publication. Keep this pre-existing worktree/branch until those
  gates pass and remote-main integration is verified.

## Typed READY harness verified; actual-foreground acceptance blocked (2026-10-27)

- Continued task branch `micharel09/replaced-dungeon-lighting-2` from clean
  `dc4f489c`. Latest fetch still has `origin/main=66749da1`, already integrated.
  The dirty primary checkout was untouched. Main has not received this task;
  no itch publication. No runtime, shader, material, asset, animation, hit timing,
  Slayer duration, probe/shadow budget, resolution or package change in this continuation.
- QA observer/plugin remain byte-identical to `990105b5`. Public QA/frame-buffer
  schema, counters, flags and interval ownership are unchanged. Changes in Git
  are the task input/performance harness, contracts and this verification record.

### Verified input and teardown

- Shared `dungeon_input.cjs` recognizes the full READY or its remaining suffix
  only after a nonterminal duel with a live player, outside Pause/intro/loading.
  Each real keyboard letter requires a newer publication acknowledging progress.
  After Y it waits for actual new combat and sends no additional letters.
  `EndlessRun.Win` advances Slot before READY, so Slot correctly stays unchanged
  through the ensuing reload. Final victory/death cannot enter this path.
- Guide Enter retains its500ms settling and spans observed Unity frames.
  UI mouse presses and Ctrl+A also span observed frames and release in `finally`.
  The native adapter waits for stable input coordinates, confirmed NameInput focus
  and actual rendered `ArenaQA` text before clicking Start. No Continue call,
  gameplay setter, scene shortcut or provider write.
- A real transition exposed JsonUtility's bare `Infinity` coordinates, which
  standard JSON.parse rejects. The page-only adapter preserves +/-Infinity by
  converting only unquoted numeric tokens to overflowing JSON exponents; quoted
  text, ordinary JSON, precision and revivers retain their values. Malformed
  payloads and NaN still fail. The original C#/plugin observer is unchanged.
- Normal typing now waits for full observed stamina and expired combo as well
  as the existing wall-clock gap. Under a slow Unity clock, wall time alone let
  normal hits accumulate a guard break/Slayer; that invalid run is retained.
- **38/38 Node contracts PASS**, including all20 prior observer/frame contracts,
  fresh/stale/partial READY, scene/result boundaries, release on errors, JSON
  fidelity and actual-Windows-foreground verdicts. Command:
  `node --test tools/game_loop/test_frame_timing.cjs tools/game_loop/test_dungeon_input.cjs`.
- Fresh-profile native QA smoke `ready-smoke-5`: **297 checks PASS**, two consecutive
  wins, full READY, partial EADY held through Pause/Resume, next combat and Restart
  to duel1/40HP.0 exceptions/provider reads/provider writes. `ready-smoke-3` had
  passed262 checks on the preceding startup adapter. Failed/never-launched smoke
  attempts remain raw evidence; they are not counted as passes.
- Final drain precedes the common receipt/report verdict and closure. Native
  teardown is bounded at15s; close state and measurement status stay separate.
  A hung close can stop only a verified task node/browser tree; no forced kill
  was needed in the completed continuation runs. Windows foreground proof is now
  requested after the final drain and included in the persisted verdict before
  close. Official runs refuse an adapter without that proof.

### New immutable QA A payload

- Existing Ubuntu doctor passes: pinned Unity2022.3.25f1, activated private HOME,
  required images and35.6GiB free; no service setup or Unity Windows execution.
- QA `20261027T072142-webgl`, input
  `5b9eb6e84d34c9a296bdee9bf9e7a906ac7755fa6c020f124fc2ab678e2b17f3`:
  build and strict repeated-builder PASS;281depth mappings/0missing.
  All2495 game/asset/config inputs match the continued source after canonical LF.
- Downloaded505 files match server SHA256:33player +472serialized files.
  Archive SHA256 `0c476b9643c88e142c07a7fa54b9f250cbe4d5569a275c1f8ec06d18b5cf7f03`.
  `qa-A-hashes.json`, `source-A-manifest.json` and the immutable `qa-A/` retain proof.
- Initial literal comparison found24 differences, not25:22 whitespace-only.
  All1074DungeonWorld,440MainMenu and1121SampleScene object IDs are preserved.
  Derived light/fog/baking values match the unchanged referenced LightProfiles/
  DungeonLighting profile applied on preview/enable; one transform is rewritten
  at Unity float precision. No serialized assets were imported into Git and no
  RGB0/visual acceptance is claimed. Raw and per-object diffs are preserved.
- This is the new typed-READY runtime payload before any further optimization;
  its performance qualification is **not complete**. B remains unselected.

### Retained Unity-marker diagnostics; no performance pass

All completed measurements use15s warmup and >=60s combat, detailed-off only in
normal, with no video, CPU profile or GPU queries. Execution `passed` below means
coverage/input checks passed under that recorded harness; it is not an FPS pass.
**None has verified sustained actual Windows foreground.** Harness revisions are
kept separately; do not pool them or replace slow valid rows with later numbers.

| Raw run | Execution | Unity FPS | p95 ms | Minimum1s FPS | >50ms hitches | Parry/Slayer coverage |
|---|---|---:|---:|---:|---:|---|
| A-h3-normal-off-1 | passed | 64.804 | 27.680 | 44 | 3 | False/False |
| A-h3-normal-on-1 | passed | 0.999 | 1011.460 | 0 | 60 | False/False |
| A-h3-parry-on-1 | passed | 72.669 | 19.600 | 57 | 0 | True/True |
| A-normal-off-1 | passed | 70.168 | 24.000 | 57 | 2 | False/False |
| A-normal-off-2 | failed | 17.026 | 18.160 | 0 | 47 | False/True |
| A-normal-on-1 | passed | 70.784 | 22.785 | 52 | 1 | False/False |
| A-normal-on-2 | passed | 15.705 | 991.490 | 0 | 48 | False/False |
| A-parry-on-1 | passed | 69.009 | 23.585 | 51 | 0 | True/True |
| A-v2-normal-off-1 | passed | 70.920 | 21.995 | 46 | 4 | False/False |
| A-v2-normal-on-1 | failed | - | - | None | 0 | False/False |

- The attempted two-per-configuration qualification is incomplete. Normal-off2
  in the first harness unexpectedly entered Slayer and is invalid for normal.
  The v2 startup timeout collected0combat frames. The h3 series was interrupted
  after its first normal/normal-off/parry rows; any partial directory lacks final
  acceptance. Full raw traces, failed launches and immutable per-run harnesses remain.
- Whole diagnostic callback/observer samples showed detailed observer medians
  about1.6-1.8ms versus0.02ms minimal; minimal mode still failed p95. Long ~1s gaps
  were not ~1s Unity-containing CPU callbacks. This does not isolate a specific
  game/bridge/OS/driver hotspot. No speculative optimization or rejected
  managed-string/deferred-decoding candidate was reintroduced. No CPU/GC/heap
  attribution trace or new diagnostic C# instrumentation was added.

### Actual Windows foreground: concrete blocked gate

- Earlier nine changing-canvas controls used the same Chromium153 executable,
  GTX1650Ti/ANGLE D3D11,1280x720 and controlled flags,3s warmup/12s measure.
  Their round3 passed all metric/DOM checks (74.973/74.972/74.557FPS,
  p9513.6ms,minimum75/75/70). The earlier statement that this fully opened the
  environment gate is superseded: DOM focus is insufficient for actual foreground.
- New consumer-owned supervisor samples GetForegroundWindow PID every100ms,
  matching the Chrome child PID captured at launch. Node and Chrome consumers
  are explicitly registered before measurement. The only window activation is a
  single verified task-Chrome SetForegroundWindow attempt before warmup; there
  is no repeated focus stealing, Computer Use, app closure or power/GPU/driver change.

| Foreground attempt | Canvas result | Actual Windows result | Verdict |
|---|---|---|---|
| foreground-control-1 |0.99965FPS / p951000.5ms / min1; DOM focus true |118samples show Orca10692, not task Chrome46976 |FAIL |
| foreground-control-2 |Not measured |Windows refused activation of task Chrome47772; gate timed out |UNQUALIFIED |
| foreground-control-3, three consecutive runs |75.000/75.001/74.999FPS; p9513.5/13.5/13.4ms; min75 each |Task Chrome43212 initially foreground, then Edge12016 took it after about5s; later runs entirely Edge |FAIL |

- `foreground-control-3` preserves119/120/119per-run PID samples: run1 has8task
  samples and111Edge samples; runs2/3 have120/119Edge samples. DOM focus stayed
  true throughout. All three controls close normally. Canvas metrics alone do
  not qualify this environment, and old game runs cannot retroactively prove
  foreground. No application was stopped to manufacture a pass.
- **Stop at this checkpoint after three new foreground attempts without a
  fully qualified round.** Required external condition: keep the dedicated task
  Chromium window in actual Windows foreground for the complete controls and
  measurement, without switching to Edge/Orca. Do not keep rerunning to select
  favorable numbers. Reopen the environment gate with three consecutive controls
  including OS PID proof before component profiling/candidate work.

### Remaining acceptance and resources

- After the foreground condition is available: finish two long valid A runs per
  configuration under one locked harness; separately attribute observer gather/
  reflection/particles/serialization/JS decode/100ms publisher and browser CPU/GC/
  heap cost, then make one equivalent candidate at a time. Preserve the original
  thresholds and source/equality contracts. No B or official eight-run ABBA yet.
- Final B production/exclusion/smoke, six-encounter and separate Berserker capture,
  full1x/0.25x playback and100%selected frame/transition inspection are UNVERIFIED.
  The current tool catalog has no supported direct video inspection capability;
 0new final-video frames/transitions were reviewed. Stills/playback-control
  automation do not replace watching the required video. This is an additional
  mandatory-tool blocker; no acceptance waiver or main integration.
- Durable deliverables, players, all raw evidence/adapters and source/harness/
  executable/config SHA256 manifests:
  `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-ready-20261027`.
  Existing private Ubuntu runner/cache/license stay in their original locations;
  immutable A result stays under
  `/data/typinghero-game-loop/tasks/dungeon-performance-20261027-final/results/20261027T072142-webgl`.
- Keep the task worktree `D:/Projects/TypingHero-worktrees/replaced-dungeon-lighting-2`
  and branch `micharel09/replaced-dungeon-lighting-2` for continued acceptance;
  remove only after verified accepted remote-main integration. Primary checkout
  and historical artifacts are preserved. Task scratch cleanup is recorded below.

### Retention and cleanup receipt (2026-10-27)

- Harness/checkpoint commit `2e75db4c` was pushed and fetched back at the same
  task-branch SHA. Remote main remains `66749da1`; no task-main integration.
- Both registered scratch tasks were completed with explicit session
  `01a17c27-fa69-7572-a018-3506e42e3c9a` and `-Apply` at09:34UTC:
  `dungeon-ready-20261027` and `dungeon-foreground-20261027`. Both receipts report
  `State=deleted`, `Reason=verified-absent`. No task browser/node/Python consumer
  remained at handoff. Deliverables and raw traces are outside temp and retained.
- Cleanup receipts: `cleanup-ready.txt`, `cleanup-foreground.txt` under the durable
  evidence root. Register a new unique scratch task before any resumed browser
  profile; update the private adapter's manifest with its returned path/session.
  Existing worktree/branch and Ubuntu runner/cache/license remain for continuation
  until accepted remote-main integration. Do not remove historical evidence or
  pre-existing resources.

## Latest source checkpoint:63a78500 (2026-10-27)

Merged upstream66749da1 (typed READY) with both documentation histories preserved.
Task branch is pushed; main is unchanged by this task. All FPS players below
predate this runtime/input change and cannot accept the new source. No new
Unity build is claimed. A new A/B source lock and QA builds are required before
continuing acceptance. Existing Ready carry/reset1/1 predates this change;
upstream READY evidence is recorded separately in docs/game/verification.md.

## Acceptance stopped: performance gate failed; main unchanged (2026-10-27)

- Retained implementation: 990105b5, with main documentation merged at06105e08.
  No task integration into main, no itch release, and no quality/timing changes.
- Deferred-decoding candidate is REJECTED: no demonstrated performance win in
  parry, and neither mode meets acceptance. Its plugin change was restored to
  the committed eager independent-snapshot implementation. Candidate source is
  preserved in `deferred-candidate.patch` under the durable evidence root below.
  The delayed-first-read and cross-frame mutation contract remains useful for
  the retained implementation; all20 Node contracts pass after restoration.
- Ubuntu candidate build:052845-webgl; canonical source digest
  de8f62c123c9ed7357b9b51974f82fb5c27921cb41c1f9b2d2b06ab6f33160e5.
  All33 downloaded player hashes match. All472 serialized assets match A;
  output.wasm and output.data are byte-identical to ready-B. Original-serializer
  comparison passed1567 checks/1565 observed rows; this is correctness evidence,
  not performance acceptance.

| Deferred candidate qualification | Valid combat seconds | FPS | p95 ms | Minimum1s FPS | Hitch >50ms | Verdict |
|---|---:|---:|---:|---:|---:|---|
| deferred-normal-60 | 60.307 | 70.523 | 21.820 | 51 | 2 | FAIL |
| deferred-parry-60-v5 | 60.635 | 52.478 | 34.920 | 1 | 26 | FAIL |
| deferred-parry-60-repeat | 60.191 | 50.988 | 35.120 | 30 | 18 | FAIL |

- Both parry runs have real counter-increment and Slayer coverage. Final receipt
  and native report FPS agree for all three rows. These are qualification runs,
  not a replacement ABBA block. Earlier v4 parry startup failure remains saved.
- Startup harness v5 holds Enter across observed Unity frames before key-up.
  A70ms tap can fall entirely inside a long frame (observed1.462s at guide).
  Both v5 parry runs reach combat through real keyboard input. No game setters.
  v5 has not yet been validated on A or used for a complete official block.
- Context close hung after the first v5 receipt was saved. Only verified
  task-created Chromium descendants were stopped; native report subsequently
  completed, with0 runtime exceptions and matching receipt. Repeat closed normally.
- Post-candidate canvas controls use the same browser/GPU/viewport/flags and
  3s warmup/12s measurement. First isolated round:74.889/32.198/74.972 FPS,
  p95 13.6/13.6/13.5ms, minimum74/1/75 FPS; middle run fails despite focus.
  Rounds two and three each pass all3 runs (see isolated controls4..9), so the
  environment gate reopened. Do NOT claim the environment remained blocked or
  assign the reproduced game cost to a proven CPU/shader hotspot.
- A preceding `post-deferred-control` probe is retained but is not used for
  acceptance: a contract check overlapped its execution, and close hung.
- No further runtime/shader edit is justified by the current component-level
  evidence. Next work requires isolating the remaining recurring cost, then an
  equivalent candidate with two valid diagnostics per mode and a fresh8-run
  ABBA on a single locked harness. Do not rerun only to replace poor numbers.
- Production build/exclusion/smoke and final1x/0.25x playback/per-frame review
  remain UNVERIFIED on an accepted final B. They cannot authorize integration.
- Durable evidence: `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-performance-20261027-final`.
  Keep this worktree/branch because integration is not accepted; remove only
  after verified remote integration. Task scratch may be cleaned after consumers exit.

## ABBA1 failed; deferred-decoding candidate in flight (2026-10-27)

- Current committed HEAD06105e08 includes implementation990105b5 and latest
  docs-only main merge e5c48017. Main has NOT received this lighting task.
- FormalABBA1 ran all8slots with locked v3 harness,15s warmup and60s combat.
  Both A1 slots timed out at tutorial startup before measurement; both blocks
  are invalid. Every B slot has valid measured coverage but fails performance;
  preserve those failures rather than discarding them with the invalid block.

| Run | Valid | Unity FPS | p95 ms | Minimum1s FPS | Performance pass |
|---|---|---:|---:|---:|---|
| abba1-normal-A1 | False | - | - | None | False |
| abba1-normal-B1 | True | 62.299 | 28.000 | 40 | False |
| abba1-normal-B2 | True | 58.643 | 29.860 | 49 | False |
| abba1-normal-A2 | True | 42.416 | 41.490 | 29 | False |
| abba1-parry-A1 | False | - | - | None | False |
| abba1-parry-B1 | True | 51.579 | 34.710 | 34 | False |
| abba1-parry-B2 | True | 56.585 | 30.340 | 39 | False |
| abba1-parry-A2 | True | 56.290 | 28.590 | 43 | False |

- Receipts and reports agree after final drain. All raw runs and old harness v3
  remain under the durable root. Startup fact: Practice click did not produce a
  confirmed tutorial exit; exact event/timing cause remains unknown. v4 uses
  documented Enter after500ms guide settling and waits for tutorial exit, via
  real keyboard only. v4 succeeded once in the separate B GPU diagnostic;
  A startup validation is still pending.
- Stable observed lifecycle samples:1world camera,1presenter,2arena textures,
  2actors,1HUD; no increasing draw-call trend (normal median166). See derived
  `abba1-cost-analysis.json`; this is not a repository-wide leak-free claim.
- Separate GPU diagnostic:1489non-disjoint Unity-containing query samples,
  GPU median6.186592ms; CPU callback median13.670ms. Independent clocks, never
  summed. Correct coverage and0exceptions; thresholds fail, not accepted FPS.
- New candidate changes only the QA JS decoder: capture the immutable particle
  JSON for each frame, materialize that frame's independent mutable array only
  when read. All192slots/schema/values remain available, including old frames
  first read after later payload updates. C# still observes every physical frame.
 20Node contracts PASS, including delayed historical reads and mutation isolation.
  Actual final-object equivalence and performance are pending for this candidate.
- New build in flight: local20261027T052718-build-candidate, canonical input
  `de8f62c123c9ed7357b9b51974f82fb5c27921cb41c1f9b2d2b06ab6f33160e5`;
  unified exec session64617 was active. No Unity Windows execution.
- Keep A player `qa-ready-A/player` (source85bd1706/input393836e2); only docs have
  advanced upstream since. Keep C2B `qa-ready-B/player` for comparison. Download,
  hash and verify the new player against the original complete serializer before
  performance qualification. Because C2 fails at60s despite passing short runs,
  a single60s candidate qualification is justified before the next full ABBA.
- v4 source/config live in `locked-harness-v4` and `run_perf-v4.py`; v3 and its
  run_perf.py remain byte-preserved for ABBA1. Formal next blocks must use a single
  new frozen manifest and be rerun in full after the startup/harness change.
- Production checks, final six-encounter/Berserker capture and direct1x/.25x
  playback plus selected100%frame/transition review are still pending. Do NOT
  integrate main, lower thresholds/quality or publish itch.
- Durable root:
  `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-performance-20261027-final`.
  Registered scratch task `dungeon-fps-final-20261027` remains active; complete it
  only after consumers exit and deliverables are retained outside CodexTemp.

## Locked candidate for formal ABBA (2026-10-27)

- A source85bd1706 (merged origin/main29c03285, before QA optimization), input
  `393836e29131942b1a1149a90a23e62572a773f83f55fd09d014b079303ce9de`;
  immutable QA20261027T043436-webgl, local `qa-ready-A/player`.
- B is the current bridge-cache implementation, input
  `4348c84a804c9fa4d47d2dca8c5f19505779501c89a058b82749781544408043`;
  immutable QA20261027T045018-webgl, local `qa-ready-B/player`.
  A/B build and strict repeat PASS;33player hashes each match server;472serialized
  files byte-identical;2493game inputs unchanged, only2QA files differ.
- Ready actual-scene carry/reset/pause test20261027T044828-quick:1/1PASS.
 20Node contracts PASS. Final published snapshots match original serializer for
 1233/1233observed frames in `ready-B-verify`, including parry/Slayer. That
  verification run has a retained6.99s hitch and is NOT a performance pass.
- Comparison-OFF final diagnostics: normal74.83195FPS,p9517.730ms,min1s73;
  parry74.80227FPS,p9515.755ms,min1s71;0hitches>50ms each,valid continuity/focus/
  mode/actor coverage,0exceptions/provider reads/writes. Observer median
  1.260/1.325ms. Do not substitute these diagnostics for official acceptance.
- Freeze `locked-harness-v3`, `run_perf.py` and `final-harness-manifest.json`
  under the durable root below. Formal configuration explicitly disables video,
  legacy timing/GPU wrappers, query interception and serialization verification.
  The pre-measurement schema check requires192slots and records allfield names.
- Next: official normal A1,B1,B2,A2, then parry A1,B1,B2,A2,15s warmup and >=60s
  valid combat each; no parallel build/capture/diagnostic. Every B must pass
  FPS>=59,p95<=18ms,min1s>=40 with valid coverage. Keep failures and do not rerun
  only to replace poor numbers. No formal block has run yet.
- Production build/exclusion/input smoke, final six-encounter+fixture capture,
  direct1x/.25x playback and selected100%frame/transition review remain pending.
  Do not merge main or publish itch until all required gates pass.
- Durable evidence root:
  `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-performance-20261027-final`.
  Task worktree/branch remain retained; current scratch registration is listed
  below. Previous candidates, rejected/failed runs and source/manifest history
  remain intact. Update this section with official outcomes before handoff.

## Live performance continuation (2026-10-27; not accepted)

- Continue task branch from `67862b56`, origin/main `83d6e0a6`; original dirty main
  checkout untouched. Durable root:
  `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-performance-20261027-final`.
- Three fresh changing-canvas controls pass consecutively on Chromium153.0.8010.12,
  actual GTX1650Ti/ANGLE D3D11, foreground1280x720, 3s warmup +12s measurement:
  FPS74.556/74.972/74.808; p9513.6/13.5/13.7ms; minimum1s windows73/75/74.
  Persistent task profile/consumer registration used; prior browser-closure cause
  remains unknown. No power/GPU/driver or user-application changes.
- Baseline A: merged source `67862b56`, canonical input
  `07394443ea5f4a96d93f500d7d10b62ac7688fbab31a976dd313062e12f96083`.
  Ubuntu2022.3.25f1 result20261027T023958-webgl: fresh private Library (no reused
  assemblies/import state), QA build and strict repeated builder PASS;
  281mapped/0missing. Player `5b1df9a17132c7c0`; all33downloaded hashes match server.
- A diagnostics (5s warmup, normal10s/parry20s; not formal ABBA):

| Run | Valid | Combat ms | FPS | p95 ms | Minimum1s FPS | >50ms hitches | Thresholds |
|---|---|---:|---:|---:|---:|---:|---|
| normal detailed | yes | 10108.80 | 74.687 | 15.945 | 72 | 0 | pass |
| normal minimal | yes | 10018.21 | 73.965 | 17.845 | 69 | 0 | pass |
| parry detailed | yes | 20242.20 | 66.495 | 24.440 | 47 | 2 | fail |
| parry detailed repeat | yes | 20207.22 | 70.767 | 21.070 | 61 | 0 | fail |

- All have correct mode/actor/focus/continuity coverage, no runtime exceptions or
  provider traffic. Parry runs include actual counter increases and Slayer
  intervals. A remains failed; diagnostic execution status does not mean FPS pass.
- Observer cost recurs in both valid parry runs (roughly3-4ms median). Source
  serializes192particle slots every frame, including unchanged stale slots.
  Candidate changes only QA-only encoding: cache particle JSON when no slot was
  written; preserve all192slots/values and full public schema. Added optional
  diagnostic per-frame exact-string comparison with the original JsonUtility
  serializer; formal measurements must keep this expensive comparison OFF.
- 20 Node contracts pass, including bridge verification flag, independent snapshots,
  requiring every observed frame checked, final verification-error closure and
  preserving input-error precedence. Actual candidate equality/performance remains
  UNVERIFIED until new player checks. Initial external adapter BOM syntax failure
  occurred before any browser run and was corrected; failure log retained.
- Candidate build in flight: local20261027T033918-build-candidate, remote
  20261027T034011-webgl, canonical input
  `48ee6606e19a948e6cabb88b8857c027d5013d8233a21b3b4fae5f321a56113f`.
  Harness-only verification-count/error-report refinements postdate snapshot;
  they do not change player inputs. `locked-harness` keeps original A diagnostic
  tools; `locked-harness-v2` is the new pre-ABBA harness. Never overwrite old traces.
- Candidate1 result20261027T034011-webgl builds/repeat PASS;33player hashes match.
  `verify-parry` compares1584final JSONs against the original serializer over1583
  observed frames, including real parry/Slayer; no mismatch/exception. With the
  comparison OFF, normal64.015FPS/p9525.935ms/min57 and parry64.319FPS/p9526.055ms/
  min46 both fail. Do not accept/merge this candidate. Its source is retained in
  `candidate1-source`, player in `qa-candidate`, and all raw traces are preserved.
- Contemporary A normal (same v2 harness) also fails p95:70.658FPS/20.25ms/min58.
  This shows variation relative to early A, not a specific OS/driver cause.
  Three post-candidate controls pass:74.722/74.306/74.973FPS, p9513.5/13.7/13.6ms,
  min73/69/75. Keep all failures; never select only favorable runs.
- Candidate2 replaces managed full-payload string composition with a private
  bridge cache. C# sends full192slot payload only on first observation or any
  slot write. JS reparses the cached string per frame, preserving independent
  nested snapshots and the original public schema/stale values. Diagnostic
  verification compares the FINAL published object with the original complete
  JsonUtility JSON on every observed frame, rather than only an intermediate.
 20Node contracts pass for changed/stale slots, mutation independence, exact
  snapshot/verification failures, all-frame count and error/closure precedence.
- Candidate2 build in flight: local20261027T041113-build-candidate, canonical input
  `854b2f7a163461be791327c5cd785e9b57a568716dfd00f582d697abe79f64fb`.
  `locked-harness-v3` is the next frozen pre-ABBA harness; no formal block has run.
  Download/hash and actual final-object equality/normal/parry diagnostics remain
  pending. Retain or reject based on results before formal ABBA.
- Main advanced to29c03285 (Ready result caption; same Continue callback).
  Merged as85bd1706; conflict resolution preserves this task's performance history
  plus upstream Ready/Slayer completion documentation. Dirty observer changes
  remain outside that merge commit.85bd1706 is the NEW pre-optimization A source.
- Pre-Ready candidate2 result20261027T041159-webgl builds/repeat PASS, but is NOT
  used for runtime/FPS acceptance after this source change. Keep its immutable
  server player/log and canonical854b2f7a input as a superseded compile result.
- `build_ready_pair.py` is running sequentially: A build from revision85bd1706,
  then exactly EndlessTests.ActualSceneCarriesHealthAndStatisticsAndResetsCombatBindings,
  then B build from current observer worktree. First source input is
  `393836e29131942b1a1149a90a23e62572a773f83f55fd09d014b079303ce9de`,
  local20261027T043352-build-baseline. The authoritative local pipeline stage
  receipts are in `ready-pair-stages.json`; unified exec session99880 was active.
- A initial local serialized-copy gap was corrected from the immutable server:
  all472A/C1serialized files are byte-identical. Initial partial-copy comparison
  is retained separately. All33Aplayer hashes had already matched, so no player
  or FPS data changed in this correction.
- Formal eight-run ABBA, final production smoke/exclusion, six-encounter/Berserker
  capture and direct final playback/frame review remain pending. Main is not
  integrated; no itch publication is authorized.
- Registered scratch task `dungeon-fps-final-20261027`, path
  `E:/CodexTemp/TypingHero/dungeon-fps-final-20261027-b1b741bbdc43`; deliverables
  already outside temp. Complete scratch only after all consumers exit. Private
  Ubuntu build/cache root `/data/typinghero-game-loop/tasks/dungeon-performance-20261027-final`
  is retained for continuing builds; shared private license HOME is referenced,
  never copied or changed by cleanup.

## Fresh continuation ? environment gate blocked (2026-10-27)

- Resumed clean task HEAD `3ee8b7a5`; verified origin micharel09/TypingHero,
  fetched and merged origin/main `83d6e0a6` as `421813be`. One documentation
  conflict preserved both performance evidence and newer Slayer acceptance/audit.
  Existing dirty main checkout was untouched. No runtime optimization was made.
- Main adds intro/Slayer runtime and QA observation changes. Old QA player
  `7cd77c5fb88da486` is not evidence for this merged source. Baseline A and final B
  remain unlocked/unbuilt; no new Unity build is justified before environment pass.
- Durable evidence: `C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-performance-20261027-resume`.
  `source-manifest.json` hashes tracked game/config/harness inputs and retained
  native/Ubuntu adapters. Each completed control JSON includes raw rAF intervals,
  foreground/visibility, renderer, viewport, browser user agent, launch flags,
  browser executable SHA256 and control script SHA256.
- Changing WebGL canvas: same Chromium executable/controlled launch flags as
  retained native adapter, foreground 1280x720, warmup 3s then >=12s observation.
  No build, image export, video or concurrent diagnostic ran during these controls.
  Renderer is actual NVIDIA GTX 1650 Ti / ANGLE D3D11 in every completed run.

| Round/run | rAF FPS | p95 ms | Minimum 1s window FPS | Focus/visible | Result |
|---|---:|---:|---:|---|---|
| 1.1 | 67.316 | 13.6 | 25 | pass | fail |
| 1.2 | 32.757 | 13.7 | 1 | pass | fail |
| 1.3 | 33.700 | 13.6 | 1 | pass | fail |
| 2.1 | 18.583 | 186.9 | 1 | pass | fail |
| 2.2 / 2.3 | unavailable | unavailable | unavailable | unverified | browser closed |
| 3.1 | 3.768 | 1000.4 | 1 | pass | fail |
| 3.2 | unavailable | unavailable | unavailable | unverified | browser closed |

- Three unsuccessful rounds show no progress. Run 3.3 was not attempted after
  repeated browser closure. Closure cause is unknown; no task stop was issued.
  Error receipts preserve this gap; incomplete runs have no returned raw samples.
  Do not interpret them as performance passes. No power/GPU/driver settings or
  user applications were changed/stopped. No task profile chrome/node consumers
  remained in the final process inspection.
- Environment is **not qualified for measurement**. Canvas-only cadence fails
  independently of Unity; it does not identify a game CPU/GPU bottleneck or an
  OS/driver cause. No game diagnostic, ABBA, new production check or final playback
  was run, and no runtime-performance acceptance or main integration is claimed.
- Scoped harness recheck after merge: 16/16 Node contracts pass; performance driver
  syntax passes. Documentation whitespace check passes. These do not validate
  the merged runtime. Preserve worktree/branch until a stable environment allows
  the pending build/diagnostic/ABBA/production/playback gates in the approved plan.
- Next: requalify three consecutive control runs without changing user settings
  implicitly, then build fresh merged A and follow the original acceptance plan.
  Task branch push/remote preservation is recorded in the external receipt.

## Harness acceptance checkpoint (2026-10-27)

- Continue `micharel09/replaced-dungeon-lighting-2` from `4613b84`. Changes are
  limited to JavaScript harness/tests and documentation; reuse QA player
  `7cd77c5fb88da486`. No Unity build, video, browser profile or measurement rerun.
- Final drain now precedes the verdict and refreshes summary/coverage. Input errors
  retain precedence over separately recorded drain errors. Available trace is saved;
  context closure is attempted even if saving fails. Receipt/report share the final
  summary/status, with no drain after verdict.
- Combat intervals belong to their first active frame, retaining exit hitches and
  excluding entries. Continuity/focus/actor coverage remain independent checks.
- `minCombatWindowFps` counts completed Unity frames in sliding 1-second windows
  within each continuous combat segment. Any window below40FPS sets
  `sustainedLowFps` and fails acceptance, alongside FPS>=59 and p95<=18ms.
  `longestSlowWindowMs` remains diagnostic for existing consumers.
- Coverage uses measured combat only: Slayer must own an interval; parry requires
  a counter increase between adjacent combat observations. Counter decreases reset
  comparison; scene-entry restoration does not count. Parry protocol still requires
  `PERF_DETAILED=1`; detailed-off parry is not implemented.
- Verification: all16 focused contracts pass with
  `node --test tools/game_loop/test_frame_timing.cjs`; syntax checks for all three
  changed JavaScript files and `git diff --check` pass. Cases include final-batch
  focus/overflow/gaps, input plus drain failure, save-failure closure, exit hitch,
  mixed-frame low FPS, segmented windows and reset/scene-entry/Slayer coverage.
- Historical raw traces, measurements and evidence below remain unchanged and
  scoped to their recorded harness. **ABBA, final playback and FPS acceptance
  remain incomplete.** Harness completion does not establish game performance.
  This checkpoint starts the next runtime optimization task.
- Keep the existing worktree/branch for continuation; no main merge or cleanup.
  No filesystem scratch was created; scenario fixtures run entirely in memory.

## Frame measurement continuation — acceptance pending (2026-10-26 evening)

Continue the existing `micharel09/replaced-dungeon-lighting-2` worktree. Fresh
fetch resolves main to `480ad7039d0379d2194a6750f5dba938aa58fa24`;
neither candidate HEAD `f4f684c301556b9c60b95764307d9e6762b7c094` nor lighting draft
`c590c736` is its ancestor. The incoming plan's “candidate already on main” premise
is therefore not true for this checkout. Use that candidate as the optimization
baseline; keep the pre-lighting player strictly for visual comparison. Do not
merge the unaccepted candidate into main to make the premise true.

Automation changes only:

- The QA bridge stores scalar Unity LateUpdate markers in a fixed 16,384-row
  Float64 buffer. Batched reads expose sequence, actual `Time.frameCount`, monotonic
  callback timestamp, Unity delta, detailed-observer cost, presentation/combat flags,
  parry count, time scale, focus, visibility and focus-event epoch. Buffer loss and
  frame gaps invalidate a run. These are player-loop markers, not GPU presentation
  timestamps; the existing 100ms state is never expanded into synthetic frames.
- `tools/game_loop/frame_timing.cjs` measures rAF, CPU callbacks containing Unity
  frames, optional diagnostic GPU queries, draw calls and sampled memory separately.
  Unavailable/disjoint GPU results stay absent; CPU callback time is not GPU time.
  Combat hitches, hitstop and held frames stay in the distribution. Only explicit
  intro/loading/reading/Pause/result boundaries are excluded from combat coverage.
- `tools/game_loop/dungeon_performance.cjs` uses the existing native fixture-only
  harness, real keyboard/mouse, 320x180 world and 1280x720 player. It retains traces
  on input/assertion failure, drains every250ms, and checks continuity/focus before
  duration and thresholds. Normal acceptance requires15s warmup/60s combat; separate
  short diagnostics compare detailed QA on/off and GPU query overhead. Real-render
  contact state drives Space; Continue waits for actual encounter state.
- All6 Node contract tests pass, including overflow, focus events across drains,
  missing/unclassified actor coverage, retained combat hitch, sustained30FPS and input-failure persistence.
  Command: `node --test tools/game_loop/test_frame_timing.cjs`.

Two separate focused/visible empty-page diagnostics on the actual GTX1650Ti/D3D11:
Edge156:374intervals,31.1371FPS,p9533.2ms; Chromium153:369intervals,30.7454FPS,
p9533.4ms. Raw samples retain startup hitches. This is environment/browser evidence,
not an attribution of a Unity bottleneck. No power/GPU settings or user applications
were changed. No shader, art, probe/shadow/fixture budget, timing or gameplay edits.

Durable evidence root (outside task temp):
`C:/Users/ADMIN/orca/artifacts/TypingHero/dungeon-performance-20261026`.

QA Ubuntu `20261026T144542-webgl` PASS, immutable canonical input
`e3def152329e81f0a7f3dc9ff5ea3935084fdf274937b39924e0cb545fc205a1`, native player
`7cd77c5fb88da486`. All33 downloaded player hashes match the server; all2495 current
Assets/Packages/ProjectSettings inputs match that snapshot after canonical LF.
This run includes the normal builder's repeated serialization check. The earlier
72 scoped Unity cases and declared visual/frame coverage remain evidence for
unchanged game/render inputs; they are not new test/frame-review executions.

Native Chromium153/GTX1650Ti/ANGLE D3D11, no video, four **diagnostics**:

| Mode | Combat ms | Unity FPS | p95 ms | >50ms hitches | Longest >25ms-frame run |
|---|---:|---:|---:|---:|---:|
| Normal, detailed QA | 10283.89 | 30.922 | 34.965 | 0 | 8050.885ms |
| Normal, detailed QA off | 10213.32 | 30.646 | 33.595 | 0 | 10213.320ms |
| Normal, GPU query on | 11826.02 | 25.368 | 37.535 | 12 | 5004.205ms |
| Parry/Slayer, detailed QA | 20366.70 | 49.885 | 23.940 | 2 | 725.340ms |

Each uses5s warmup, continuous actual Unity markers and separate browser clocks;
all four diagnostic executions pass coverage/input checks with0exceptions and
0provider reads/writes. “Diagnostic passed” does **not** mean performance accepted:
every run fails FPS/p95 thresholds. Parry run records6actual parries and325Slayer
frames. No lost buffer rows/frame gaps/focus/visibility changes. Same-launch-flags
empty Chromium repeat is also slow:363intervals,30.2495FPS,p9533.5ms.

Whole-diagnostic medians (including warmup, not the combat-only window): detailed
QA observer2.8175ms vs minimal0.015ms; Unity-containing CPU callback11.205ms vs
7.235ms;166draws each. Minimal disables the expensive render/sorting observation;
the original100ms state publisher remains enabled. It is not a bridge-free or
production comparison. Query run records412non-disjoint GPU samples, median
3.56392ms, CPU callback12.285ms; its worse cadence/hitches are retained. These short
non-ABBA trials establish neither GPU causality nor production equivalence.
Allocated Unity memory ranges61,935,882..62,275,529bytes across the diagnostics;
JS heap and original per-batch memory remain in raw traces. Parry draw median167.

After reducing driver state payload and pinning per-run driver hashes, a final
normal pair uses identical harness files (driver `1ef8defc…`, collector `7a44f438…`):
detailed546frames/10116.56ms,53.9709FPS,p9520.55ms; minimal544/10069.66ms,
54.0237FPS,p9519.53ms. Both retain166draws and0hitches>50ms. Whole-run observer
medians2.3675/0.010ms and CPU callback7.885/4.630ms; both still fail thresholds.
All traces contain0unclassified live-actor frames under the final coverage rule.
Earlier four diagnostics and an intermediate53.8511FPS repeat are preserved,
not pooled with this pair. The initial detailed-vs-minimal pilot is not the final
same-harness comparison.

Same launch flags,1280x720 continuously color-cleared WebGL canvas with no Unity:
637intervals/12s,53.0312rAF-FPS,p9519.2ms,focused/visible on the same NVIDIA GPU.
Unlike an idle blank page this submits changing GPU content each frame. Its
similar cadence to the final53.97/54.02Unity pair supports retaining the browser/
environment limitation as unresolved; it does not establish a specific OS/driver
cause. No configuration/application changes were made to force a passing result.

Full raw batches/actions/states, source/player manifests, failed acceptance flags
and [versioned summary](evidence/replaced-lighting-20261025/performance-diagnostics-20261026.json)
are retained. No runtime optimization was selected: current data does not isolate
a game bottleneck from the demonstrably slow/variable browser environment.
Formal15s/60s ABBA for each mode, original-speed/quarter-speed playback acceptance
and final main integration remain **unverified**, with unchanged≥59FPS/p95≤18ms
and no sustained30FPS criteria. Keep this branch/worktree and evidence. Resume
matched baseline/candidate diagnostics in a stable foreground session before
selecting a runtime change; do not average failed runs into a pass.

Production Ubuntu `20261026T150148-production` exits0, QA=false, canonical input
`30cb77880ca45d1bf96808069a2b9ffdd968d0159a919b4cc63de3586d2281ea`.
Its2495 Assets/Packages/ProjectSettings inputs equal the QA snapshot; only tooling
inputs differ. All33 downloaded production files match remote SHA256. Local scan
of6decoded HTML/JS/WASM/data payloads (Brotli decoded, UTF8/UTF16LE) finds none of
`typingHeroQA`, `typingHeroPerf`, `typingHeroReadFrames`, `TypingHeroQA_Frame`,
`GameLoopProbe`; the QA framework is a positive control for the buffer marker.
This verifies exclusion, not production FPS or a new production gameplay playtest.

Retain `qa-build/`, `production-build/`, raw diagnostic directories, exact
`locked-harness/` files (hash-verified against the final paired receipts), native
and Ubuntu adapters, manifests and runner logs under the durable root. The matching
two remote immutable result directories remain under the existing private runner
`/data/typinghero-game-loop/tasks/replaced-lighting-20261026-c590c73/results/`.
Keep the existing task branch/worktree for acceptance and eventual integration;
release these evidence resources after replacement/review and verified integration.
Task-created registered scratch is disposable after those deliverables are saved.
No task browser or Unity build remains running after the completed commands.

Resume with the retained native harness and fresh output directory. Set
`ARENA_BUILD` to the exact candidate player, `ARENA_HEADED=1`, `ARENA_CONTROLLED=1`,
`ARENA_BROWSER_EXE` to the recorded Chromium executable, `PERF_OUTPUT` to that output
directory, `PERF_MODE=normal` or `parry`, `PERF_DETAILED=1`, `PERF_DIAGNOSTIC=0` and
`PERF_GPU=0`; invoke `node native.cjs scenario.cjs`. Formal mode uses15s/60s and
draw counters with no GPU query/video. Re-establish a matched instrumented A/B pair
on the pinned post-lighting runtime before the prescribed ABBA; the old pre-lighting
player remains visual evidence only. The final driver also records its own hashes.

## Current integration checkpoint — not accepted/integrated (2026-10-26)

Task HEAD merged newer main4506bb1 as084b38ee, retaining its authored chest-attached
Slayer visual and first-word preparation. The single test conflict combines the
task's production-spawner path with main's child SpriteRenderer; runtime source
from main was preserved. **72/72focused Unity tests PASS**,0failed/skipped,
23.5608316s, Ubuntu `20261026T083245-quick`, input
`5595b34e9cbe343871d81bf96d6c61e5b3bd08d9320663bc8e358a5ed88603d2`.
Strict two-world repeated builder PASS at `20261026T084131-serialize` on the same
input; painted maps/custom bindings remain intact. Logs/XML/serialized outputs:
`ubuntu-merged-{tests,repeat}-20261026/` in the existing evidence root.

Inventory re-audit:34clips byte-identical after LF normalization between main24f
and4506; all six definitions differ only by the new hitAnchor reference. Controller,
idle/hurt/attack references, sequences and timings are unchanged:281cels/306keys
remain current. Static pose/white-light/shader checks can be reused; the old native
video does not certify the updated intro or chest VFX. Updated paired Ubuntu WebGL
builds and scoped native frame inspection have now passed as recorded below;
subjective timed playback and performance acceptance remain unverified.

Reachability correction: production round1 contains six encounters but only five
distinct definitions: Swordsman,DoubleStrike,Heavy,Guard,Elite/Swordsman,Captain.
`EndlessRun.RepeatAfterBoss=false`; Berserker is selected only in unreachable round2.
The earlier real-input run therefore establishes **six encounters, five unique
definitions**, not six unique definitions. A separate serialized Berserker fixture
was built for real-key combat without changing the production run or adding a
runtime mutation API. Production Berserker reachability is outside this task.

Host pacing remains unresolved: a scoped telemetry repeat stopped when its candidate
run could not include Slayer under1s frame cadence. Chromium153 on the same NVIDIA
GPU then measured parry ABBA74.52/74.56/**1.00**/73.54FPS, with p9513.495/13.505/
**1000.63**/13.520ms. No image/export work ran during that protocol. Preserve these
failures; no stable60FPS acceptance or code bottleneck attribution is claimed.
An asynchronous request asks for a foreground,unlocked,quiet host session before
another formal measurement. Continue independent Ubuntu/build/frame work meanwhile.

Historical native-video inspection is now complete for the declared export:
**1387/1387 frames and1370/1370 consecutive source-frame pairs**, retaining held
frames and source PTS across17ranges. Per-frame observations are versioned in
`evidence/replaced-lighting-20261025/premerge-runtime-frame-review.csv`, with
source hash and limits in `premerge-runtime-review-summary.json`. This is player
`eca5db436d689a34`, before the newer main merge. The initial intro range begins
with the player already visible; full preparation and original-speed subjective
playback remain unverified. These sheets do not certify the merged intro/chest VFX
or target-GPU performance. No light-direction jump, closed bright rim or moving
floor pool was observed within the inspected ranges; authored impact flashes,
Slayer presentation and scene-cover transitions are intentional.

Merged-main baseline WebGL build completed on Ubuntu at `20261026T084445-webgl`,
input `9356c4be2ad703a4fc1046036e31b641609a899856bab93a48ef5e7e64c139b3`.
All33 downloaded player files match their immutable remote SHA256 hashes; QA flag
is true. Candidate build `20261026T090032-webgl` also completed successfully on
Ubuntu, with all33 player-file hashes verified (48,211,165 bytes). Its input is the
same5595b34e canonical input as the72tests and repeated builder above; native build
identity is `03dc85d05f9853dd`. All six serialized fixture bindings, intensities,
ranges and positions match the authored configuration; see
`evidence/replaced-lighting-20261025/merged-player-light-bindings.json`.
First merged native run stopped on a harness scene-transition sentinel (`hp=-1`,
before the next scene's actors existed), after30HP in the preceding trace and
zero exceptions. The external driver now waits for positive actor HP/current word
after Continue; no runtime game code changed. The next run genuinely fell to Heavy
after missed parries. The external driver then sent Space at up to240ms before
contact, within the authored250ms pose window, and recorded input timestamps.
Both failed reports/videos are retained; no game timing or balance was changed.

Corrected merged-player native run **PASS1329 checks**, zero exceptions, Chromium
153.0.8010.12, ANGLE/NVIDIA GeForce GTX1650Ti D3D11,1280x720/world320x180.
Six production encounters defeated,15parries, final5HP; tutorial, full/short intro,
hurt, Pause/Resume, follow-up, Slayer entry/natural exit, result/retry/menu covered.
Firebase fixture intercepted1read/1write; no external requests. Evidence:
`ubuntu-merged-candidate-full-qa-20261026-retry2.json` and its original video.
Scoped output inspection: **1117/1117 frames,1108/1108 consecutive source pairs**,
nine exact ranges retaining source PTS/held frames. Full intro preparation is
included from the menu before scene cover, unlike the historical export. Entire
first Slayer interval, each encounter's first attack/parry/follow-up and retry/menu
were viewed at native320x180 and nearest enlargement. Versioned observations and
source hash: `evidence/replaced-lighting-20261025/merged-runtime-{frame-review.csv,review-summary.json}`.

Berserker diagnostic Ubuntu build `20261026T092421-webgl`, input
`f7817a613a8ffb0235312309ed06beb63792d1376df8b4e4670e8a8ea9761a64`, native
identity `f01f3c5af9361553`,33/33downloaded hashes verified. Build callback copied
exact authored Berserker serialized fields into the first resource entry after
the normal builder; production source/resources were not changed. Native actual
input **PASS216 checks**, zero exceptions, same Chromium/NVIDIA/view sizes:
Attack2/Attack1, received hit, parry/follow-up, hurt and death observed; final35HP.
This proves lighting behavior in a fixture, not production reachability.
**296/296 frames,294/294 source-adjacent pairs** inspected. Initial231-frame export
was retained; direct inspection required65preceding frames to include the final
axe preparation/Slayer restoration, joined at source764->765. No frames discarded.
Versioned observations: `evidence/replaced-lighting-20261025/berserker-runtime-{frame-review.csv,review-summary.json}`.

Within these inspected output ranges: directional lit edge/dark body, separated
bone/cloth/steel, stable foot contact, no closed bright rim or actor-following pool;
authored impact flashes and Slayer/scene-cover transitions remain intentional.
Original-speed subjective playback is still unverified. Per-frame inspection,
real-time input execution and a passing build do not substitute for that gate or
the failed/unstable performance protocol. Do not integrate main until acceptance
is resolved. Keep the task worktree/branch, private Ubuntu cache and immutable
players/logs/videos for review; remove owned cache/worktree only after integration
and keep evidence until review no longer needs it. Original dirty checkout stays intact.

## Target WebGL continuation — performance acceptance pending (2026-10-26)

Unity executed on Ubuntu only for these players, using the private pinned runner
below. Immutable baseline `20261026T070921-webgl`, canonical input
`e86f625e839a6d8770e3bea1e12440d2fef296e7ecc8173c12c93b80a79a6667`, native
player ID `7582b6de6542bc75`; candidate `20261026T073255-webgl`, canonical input
`96479702f012497b74836733d28d28e0f5e03779b0323309107e8cd5e00b2800`, player
`eca5db436d689a34`. Both builds exit0, use `BuildOptions.None`, and downloaded
33/33file SHA256 manifests match their immutable server outputs. Evidence paths
are `ubuntu-{baseline,candidate}-build-20261026/` under the existing evidence root.

Semantic serialized-scene inspection confirms all six original fixture names,
profile GUID bindings and local anchors survive the builder; final intensities/
ranges are front2/16,side key32/13,opposite4/10,rear4/15 twice,cool64/16.
Large text diffs arise from split-scene object ID serialization; no generated
scene wholesale replacement was imported. Receipt `serialized-light-bindings-20261026.json`.

Native Edge156.0.4314.42 reports actual NVIDIA GTX1650Ti/ANGLE D3D11 at1280x720;
world320x180,one world camera. Candidate real-key/mouse run
`ubuntu-candidate-full-qa-20261026-retry2.json`: **1706assert calls PASS**,0exceptions,
all six current opponents defeated,19parries,final HP14. Covers current reading
guide,Start Battle,full/short intro,received hit,Pause/Resume,parry/follow-up,
Slayer entry/natural expiry,enemy deaths,result,retry andMenu. One fixture read/
write is intercepted locally; no provider request or score write reaches Firebase.
The earlier browser-driver failures are retained: obsolete manual encounter-reading
gate and an incorrect TypingCard vertical padding assumption. Current source
starts encounters automatically; only the external driver changed.

First target-GPU ABBA:15s warm-up per run,60s active focused/visible combat per run,
no video,loading/intro/pause/transitions excluded at both interval endpoints.

| Mode | A1 FPS / p95 ms | B2 FPS / p95 ms | B3 FPS / p95 ms | A4 FPS / p95 ms |
| --- | --- | --- | --- | --- |
| Normal | 74.97 / 13.485 | 74.97 / 13.475 | 74.97 / 13.470 | 74.89 / 13.475 |
| Parry/Slayer | 73.17 / 13.555 | 61.05 / 26.775 | 63.90 / 26.730 | 67.03 / 26.700 |

**Parry/Slayer p95 fails the18ms target; no overall performance PASS.** No measured
consecutive28–40ms interval run reached1s, but raw frames include longer hitches.
Normal candidate GPU query mean2.20/2.31ms versus baseline3.59/2.72ms; parry candidate
6.38–7.40ms versus baseline4.04/5.09ms. CPU RAF callback and GPU timer are separate,
delayed observations, not additive or exactly synchronized. Draw counts are about
167normal,156–158parry on average; mean allocated memory about61.9MB baseline /
62.1MB candidate normal,62.1MB /62.2MB parry (decimal units). Raw logs,per-state
statistics and source samples: `abba-20261026.json`,`abba-summary-20261026.json`,
`timing-20261026-*.json`. Driver success establishes coverage, not FPS acceptance.

A bounded no-GPU-query diagnostic then showed1s engine/rAF cadence for part of a
focused/visible run before returning to75FPS. A subsequent empty focused page
measured66.54FPS,p9513.9ms,max213.4ms,31/663intervals>18ms. These host observations
do not prove the candidate's parry cost harmless. A single scoped parry ABBA repeat
with GPU telemetry and no image/export work is pending; preserve the failed runs.
Runtime video frame review is in progress; original-speed subjective playback
and overall visual/performance acceptance remain unverified. Do not integrate yet.

## Ubuntu verification continuation — acceptance pending (2026-10-26)

Per user instruction, Unity execution now uses Ubuntu `fly-server` only. The
task-owned Windows Editor was stopped before test execution; no result is claimed
from it. The original dirty checkout and other server jobs remain untouched.
Private runner: `/data/typinghero-game-loop/tasks/replaced-lighting-20261026-c590c73`;
pinned Unity2022.3.25f1 WebGL Docker digest
`cff6700de0d94127031a428e17c8c8df6dfd5c77850febf677ce39858ad34e4d`.
Its private license/cache must be removed after integration and evidence retention.

- Ubuntu focused lighting/depth/world/intro/Slayer tests:62 executed,60 passed,
  two failed in the existing mirrored Slayer tint test. The same test/runtime
  source exists on base main24f74e0. The test bypassed the production spawner's
  black-tint policy by directly emitting progression colors. The repair exercises
  actual `SpawnImpact` with captured progression tint and authored swing vectors;
  all original black-color/four-frame/angle/position/no-flip assertions remain.
- Fixed test: **2/2 PASS**,0failed/skipped,0.1310992s, Ubuntu result
  `20261026T065758-quick`. Local receipt `ubuntu-fixed-tests-20261026/tests.xml`
  under the existing evidence root. The preceding retry reused stale compiled
  code because snapshot tar timestamps were fixed; adapter timestamps now change
  on upload, and the successful rerun compiled the changed source. Runtime and
  artwork did not change for this test correction.
- Fresh focused empty-page Edge observation:73.47FPS,p9513.5ms,736/736 focused and
  visible samples. This supersedes the earlier host-pacing observation only;
  it is not game performance or acceptance against the60FPS target.
- Strict repeated builder **PASS**, Ubuntu `20261026T065938-serialize`, same input
  SHA256 `96479702f012497b74836733d28d28e0f5e03779b0323309107e8cd5e00b2800`.
  Includes painted-map/custom-binding preservation, two-world Preview/Undo/Redo,
  scene save/reopen and byte-stable authored maps through both builder passes.
  Log and serialized outputs retained at `ubuntu-repeat-20261026/`.
- Ubuntu four-direction white-light output **184/184 images actually inspected**,
  **168/168 internal adjacent pairs**,23Attack1/Die cels in both facings. Native
  actor crops and nearest2x, plus full320x180 Die0 output. Eye/rib cavities remain
  dark; crown/limbs/steel respond toward the selected direction after mirroring.
  Wide source swing smear becomes bright toward its facing light; source VFX and
  alpha remain unchanged. [Per-image notes](evidence/replaced-lighting-20261025/neutral-frame-review.csv),
  [swing matrix](evidence/replaced-lighting-20261025/neutral-swing.png).
  Full23sheets/raw184PNGs/manifest remain at `neutral-review-20261026/` and
  `ubuntu-neutral-20261026/` in the existing external evidence root.
- These are functional **llvmpipe LLVM15.0.7** renders on Ubuntu, not GTX performance.
  [Photometric fixture](evidence/replaced-lighting-20261025/ubuntu-photometric.txt):
  flat-depth parity0,missing-map parity0,mirror error0, main occlusion/additional
  fill/emission isolation PASS. Result `20261026T070816-serialize`, snapshot
  `ed71b6967079565ef3131ffb714a02857af0d5bcf262e253e941dbc94b0c8a8e`.
  Prior failed diagnostic had a temporary script in the wrong assembly; moving it
  into the test assembly fixed compilation without a game-source change.
- Ubuntu-built paired
  WebGL players, six-opponent real input/playback and GTX1650Ti ABBA measurements
  remain pending. Do not integrate while these acceptance gates are unverified.

## REPLACED quality/performance follow-up — candidate, not integrated (2026-10-25)

Task branch `micharel09/replaced-dungeon-lighting-2`, independent Orca worktree
`C:/Users/ADMIN/orca/workspaces/TypingHero/replaced-dungeon-lighting-2`, based on
`origin/main` `24f74e0901d51a44e7f58bcca0007ba8d4f0b31c`. Original dirty checkout
is preserved. Evidence root:
`C:/CodexTemp/TypingHero/replaced-lighting-final-20261025`.

Implementation:

- Current controller/world/scene inventory is **281 unique cels, 306 keys, 34 clips**;
  all six current Endless definitions are recorded in `inventory-scope.json`.
  Current tutorial is a reading guide with Start Battle; do not restore obsolete
  lessons to fit older QA drivers. Tutorial Attack1/Die remain in the inventory.
- Tutorial With_VFX Attack1/Die now have normal/material companions for **10/13
  cels**. Same bone/steel/wood recipe as idle/hurt; only their secondary-texture
  lists changed. GUIDs, rects, pivots, local IDs and depth bindings are preserved.
  Explicit Unity preparation generated/bound the two material maps successfully.
- `author_actor_normals.py` supports repeated `--source`, deduplicates sources,
  and retains read-only/default CLI behavior. A real default-run failure exposed
  the single-sprite parry_lift case; it now uses the full image rectangle.
- Floor1–8 keep their original albedos, surface masks and smoothness scalar **.5**.
  New linear Point normal maps keep broad tops/mortar flat and tilt chipped rims
  gently (slope .16); no luminance-to-height conversion. Versioned recipes, channel
  meanings, parameters and output hashes are under `tools/lighting/`.
  The old external floor recipe's prototype `.65` was corrected to `.5`, with its
  original bytes retained as `floor-surface-recipe-prototype-original.json`.
- Dungeon-only profiles: front lantern **2/range16**, left pillar side key
  **32/range13**, opposite pillar **4/range10**, both rear torches **4/range15**.
  Existing six fixture anchors/flames are retained; shared original profiles are
  unchanged. Fixed cool fill stays **64/range16**, original position/cone/color.
  Fog RGB **(.004,.009,.018)**; camera/exposure/focus, shadow resolution, scheduler,
  haze/rune/beam/reflection disabled states, gameplay and original timing unchanged.
- DungeonSprite clips alpha/intro first, returns directly at Slayer weight exactly
  1, and skips self-shadow probes for zero attenuation/back-facing lights.
  Partial weights retain the original full path; contributing lights retain all
  eight probes. No gameplay API, runtime CLI or quality tier was added.

Verified technical/static evidence:

- GPU: actual **NVIDIA GeForce GTX 1650 Ti**. Controlled shader A/B uses the old
  shader with the same current material/maps, camera, poses and frozen scene.
  **614 comparisons, maximum rendered RGB error 0/255**, including partial/full
  Slayer, hit flash and intro clipping. `final-poses/images/shader-ab.json`.
- Exported **562 pose images**; **624/624 key/reference rows and 566/566 consecutive
  or loop pairs actually inspected**, native crops and nearest2x. Includes held/
  repeated keys, both facings and **22 loop wraps**. Per-row notes:
  [static frame review](evidence/replaced-lighting-20261025/static-frame-review.csv),
  [scope/counts](evidence/replaced-lighting-20261025/static-review-summary.json).
  Mirrored axe metal is substantially dimmer when turned away, then catches the
  key during execution; this is recorded separately from facing0 observations.
  Contact-library cels use diagnostic actor staging, not runtime contact placement.
- Same-framing previews: [baseline](evidence/replaced-lighting-20261025/baseline.png)
  and [candidate](evidence/replaced-lighting-20261025/candidate.png). Nine controlled
  light variants were inspected before selecting side32/front2. Warm edges/dark
  cores remain separated; the room-fixed cold floor pool does not follow poses.
- **348 existing PNGs byte-identical; 59 animation/controller files identical after
  Git LF normalization**. All eight floor scalars remain .5. Tool dry-run/source
  selection/deduplication/outside-path rejection and eight output hashes passed.
- Original baseline WebGL build exited0. This is **not** a candidate build result.
  Prior REPLACED research remains in its separate reference-study record; this
  follow-up does not claim a new trailer-frame or original-speed playback review.

Acceptance blockers / remaining work:

- Two white-light GPU jobs could not execute to completion (0 diagnostic images).
  A subsequent headless Editor launched through Node also remained before log/test
  execution (CPU .03125s, no log). No Unity test, strict repeated-builder, candidate
  WebGL, original-speed playback, controller state-join or six-opponent runtime PASS
  is claimed. Interrupted jobs/owned temporary fixtures are tracked in the evidence
  root. The existing fly-server fallback SSH connection timed out.
- Edge on the selected NVIDIA GPU is available, but even an empty focused/visible
  page measured **32.000 FPS / p95 31.7ms** (321 samples over10s). This is a host
  pacing limitation observation, not a measured game bottleneck or proof of60FPS.
  Required15s warm-up/60s ABBA normal and parry/Slayer runs remain unverified.
  Prepared fixture-only drivers record CPU callback/GPU timer/draws/memory and
  intercept provider traffic; they have not been used to claim acceptance.
- Resume with four-direction white-light checks/both facings, targeted companion/
  world-switch/Slayer/intro checks, strict two-pass builder and final candidate build;
  then real-input current tutorial/all six opponents, full intro/retry/pause/Slayer/
  menu, original-speed playback and the specified GTX1650Ti performance protocol on
  a60Hz-capable session. Preserve target320x180/1280x720 and the original criteria.
- **Do not integrate into main while these gates are unverified.** Keep the task
  worktree/branch for completion and immutable baseline player, logs, captures and
  receipts for review. Remove only task-owned cache/worktree after verified remote
  integration and after no reader needs the cache; never touch the dirty checkout.

## REPLACED dungeon lighting - implementation checkpoint, 2026-10-25

User approved REPLACED-inspired floor pools, directional actor highlights and dark
body cores, keeping the existing HD-2D dungeon. Floor choice: damp stone with mild
specular response; planar reflection stays disabled. Preserve the rejected rune,
blue haze and beam-card disabled states, authored flames, camera and gameplay.

Independent Orca branch `micharel09/replaced-dungeon-lighting`, based on main
`7dc84f918390bb826d9eae25fa0012f7d89821fc`; original dirty checkout preserved.
Task evidence: `C:/CodexTemp/TypingHero/replaced-dungeon-lighting-20261025`.
Baseline is an isolated LF snapshot with its own Library, input SHA256
`ba6e904d535f6fd53c75b12b28696a69a1fb9d585862362bf7643037419e11af`.

Reference: official Steam REPLACED Gameplay Trailer (movie257284443),
https://store.steampowered.com/app/1663850/REPLACED/ . Interior source frames944..1088
(15.733333..18.133333s),145frames/144pairs; combat1934..2036
(32.233333..33.933333s),103frames/102pairs. All selected frames/pairs were inspected
at320px thumbnails, plus native1280x720 anchors960,1048,1978; source SHA256
12ac5730025e4e30f073624cb93df1d7e30a3354de326bf9da468323c8f80b9e.
Includes shot boundaries; not a whole-trailer, timed-playback or rendering-technique
claim. Original research record remains under the sibling lighting-reference-study
task root. This reference review does not certify the changed game's appearance.

Candidate uses one constant, shadowless cool Spot Light through existing
LightProfile/DungeonTorchLight bindings: color789BB5, intensity64/range16,
inner45/outer75degrees, environment-local(1.5,4,6), aimed at local floor origin.
MainLantern24->16 retains its anchor/shadows/flame; rear/pillar torches are unchanged.
GroundSurface actually uses Floor1..8, not the unused GroundStone material. Eight
16x16 linear Point surface masks distinguish stone tops, chipped bevels and dry
mortar using the source's explicit three-color palette. Metallic remains0;
smoothness scalar.5 gives stone tops.306..369, bevel.141 and mortar.012. The
original albedo PNGs and geometry remain unchanged; camera reflection stays off.

Scoped tests found a pre-existing missing _MaterialMap on parry_lift (same input
binding as main). Existing SurfaceNormalAssets.PrepareMaterials generated only its
missing companion; preserve original albedo/normal/depth/pivot/GUID and timing.
The initial10/11result remains a failure receipt. Final input
c5c6c21d2c8d177e3555478367f1a75de1ebe5c7e20c74c1bcfeb4a3b76dd1c6:
11/11LightingProfileTests/DungeonRenderTests PASS,0failed/skipped. World-switch
regression distinguishes five flame fixtures from the independent light-only fill.
Protected430existing art/animation/shader files remain unchanged.

Offscreen GPU tuning used NVIDIA GTX1650Ti, actual world receivers/actor material,
fixed poses and the native post volume. It excludes production environment blur,
presentation framing and gameplay; previews are not final WebGL acceptance.
Baseline native Edge156/Intel UHD/D3D11 observed Normal, parry/follow-up,
Pause/Resume, Slayer/restore, restart and player death without runtime exceptions.
Its pilot driver stopped on a wrong menu-control name before return; no full PASS
is claimed. Corrected driver waits for actual result controls and uses Main MenuButton.
Pre-integration verification completed:

- Strict preview/Undo/save-reopen/repeated builder PASS (`final-repeat`). Final QA
  WebGL `final-build/player`, bundle321b30e604a19551;33file hashes retained alongside
  baseline17eb20e48efd34b4. Build exited0 after Bee recovered a stale reference to
  the removed task-only capture script; this diagnostic remains in the log.
- Native Edge156/Intel UHD/D3D11,1280x720:16checks PASS (`candidate-qa-r2.json`),
  zero runtime exceptions. Real keys cover word/hurt, Pause/Resume, four parries,
  queued followup, Slayer entry/exit, retry, death/result and menu; world ID reused.
  Provider reads/writes are intercepted local fixtures, no real provider mutation.
  Earlier candidate pilot missed the input window while hurt-locked; driver now
  waits for unlocked state. Baseline pilot's menu name failure remains recorded.
- GPU pose inventory:281unique cels,292original keys,34clips;562pose images plus
  scene/control/restored and5translation images. All596ordered review rows and
  538within-clip transitions (including repeated/held keys and loop wraps) directly
  inspected at native crop and nearest2x on50sheets. `pose-review/manifest.json`
  records per-key findings. Static GPU diagnostics exclude gameplay blur/framing;
  Slayer clips remain colored here and contact-card staging overlaps the foe.
- Legacy tutorial Attack1/Die lack existing normal/material companions. Matched
  GPU controls show the same dim-vs-Idle issue under baseline lighting; no claim
  these inherited maps are fixed. Production Endless sword/axe/shield and the
  repaired lift retain visible material regions in both facings. Five translations
  keep the floor pool fixed as the player crosses it.
- Runtime video dca942cd334d29f4ceb08f03b6d8d94922097d1702369fb8346fce978e54d095:
  selected12..15.9s idle/word;16.7..19.5s hurt/pause;21..23.8s parry/followup;
  31.8..33.3s Slayer entry;41.8..44s exit;67.5..73.9s death through result/menu.
  Scope recorded before review; death-ending supplement extends the initial72s
  boundary to include complete dissolution.491/491 decoded frames and485/485
  consecutive pairs directly viewed, original25fps PTS/holds retained;21sheets at
  320px width plus11native actor/floor crops. Warm/cool hierarchy returns after
  flash/Slayer, contact stays on the stone, no moving player-attached floor glow.
  Not human original-timing playback, full-video review or sub-frame60Hz assurance.
- Matched Normal10s no-video ABBA (`timing-summary.json`):A p95=33.315/33.235ms,
  B=33.255/33.195ms; mean delta-0.15%, within10% guard.304/306/306/305 samples,
  all focused/visible, median166draws. Real words, no parry to avoid changing mode.
  Earlier timing pilot entered Slayer and another menu input failed; retained and
  excluded, driver corrected. Short rAF pacing samples are not GPU profiling or a
  stable60FPS claim.

Integration checkpoint:8ab634c contains task data/tests/docs. Latest origin/main
2b07f3f brought intro, Slayer recovery/flash and presentation-clock updates; merged
without conflict. Merged input e858775f4ef26a6358dc4677ac611ccc1666e3549f1dc91d69464bde0cdd750e:
11/11focused cases PASS. Merged WebGL player961c878991127356 exits0 and strict
repeated builder PASS. Native merged-qa-r2:16checks PASS,0exceptions; all real-input
flows above repeated successfully. Initial merged driver clicked Practice before
tutorial intro unlocked input and timed out; corrected to await intro/transition
completion, with the failed receipt retained.22read-only intro samples cover
Tutorial/Practice and retry short variants; no full2.4s returning-player reacceptance
claim. Merged Normal10s sample:306frames, p95=33.275ms, all focused/visible, within
the original baseline guard. Same local provider fixture isolation.

Merged-motion scope recorded before review:9.8..13.1s tutorial/Practice entry,
42.4..44.3s Slayer restore,44.4..46.8s retry.191/191frames and188/188pairs actually
viewed on8sheets plus6native actor/floor crops. Original25fps PTS/holds preserved;
source SHA256 6deb9021dc8196a608469bf217a0616f72843e353ca19ffefdb60279a650aff7. World fill stays fixed through actor/dust reveal,
HUD handoff and Slayer restoration. Static pose coverage remains the named
pre-merge library; newer main's Slayer clip changes are retained, not reauthored.

Build provenance distinguishes initial LF input from the existing builder's
reserialization/derived outputs. Post-build audit: all task asset payloads unchanged
apart from scene ordering/lighting-settings binding and Unity float serialization
(rotation precision under2e-8); fill profile, light, controller and additional data
remain equal. Both builder passes are stable. No task capture script ships.
Final diff/UTF-8/relative links/whitespace checked. Implementation8ab634c and
verification a632b29 were fast-forwarded to origin/main and both ancestry plus
`ls-remote refs/heads/main` confirmed a632b29c32f130f4793555de2a22ff997f8f94f0.
Delivery/cleanup receipts remain in the task evidence root (`integration.json`,
`cleanup.json`). Remove only this clean Orca worktree/local branch and isolated
source/Library after verification. Retain logs, immutable players, review manifests,
images/videos and helper recipes there for visual review/reproduction; they can be
removed once review is accepted and evidence is archived. Original dirty checkout,
other task resources and installed Unity are preserved.

## Reusable world lighting profiles - integrated, 2026-10-23

User requested reusable scene/map profiles for all 3D lighting, rune and combat
pulses. No hardware quality tiers or map-selection UI. Five authored types:
FlameProfile, LightProfile, WorldLightingProfile, CombatLightingProfile and
WorldDefinition. Native materials/VolumeProfile/LightingSettings/URP own their
values; clocks, phases, anchors and UV meshes remain local. Four semantic roles
replace generated object-array indices. [Authoring guide](../game/lighting-profiles.md).

Task branch micharel09/world-lighting-profiles, independent worktree
C:/Users/ADMIN/orca/workspaces/TypingHero/world-lighting-profiles, based on5f371fa.
Original dirty parent checkout preserved. Evidence root
E:/CodexTemp/TypingHero/lighting-profiles-20261022-3vlshh3g.

- Unity2022.3.25f1 / URP14.0.11:20/20 scoped checks,0fail/skip, input
  b23958f4e36fc7a22909bfcd94c1ad64b27397ef800f830279e893b3d2208cfb,
  run20261022T162012-quick. Covers private UV/source-mesh/clock lifecycle, rune,
  real-controller companion maps, viewport release, direct/menu entry, three gameplay
  cycles, compact/legacy/overlap pulses and standalone MetalClash light snapshots.
- Second world: arbitrary names, four independent fixtures, intensity12 variation,
  256x144 target and independent fog. A-to-B-to-A releases prior camera/RT/meshes
  without changing profile JSON. Native Test Framework prebuild/cleanup registration
  independently passed2/2 in20261022T163704-quick, input
  4efb2852409bcf522d66cb36a6190af05cd46517dc35c1b9b3dc054e4ecab79f.
  Fixture scene remains excluded from production Build Settings.
- Authoring audit: intensity19/projected height9, material palette and bloom.14
  survive two full builder applications byte-for-byte; baseline restored afterward.
  Prefab overrides are recorded. No one-time migration seeder ships.
- Before camera integration: QA20261022T162758-webgl, snapshot
  ff11735dc6734c53f9c048857670582f9933f95131f7fbce6a315597173ad790,
  immutable player f72a8fe34ee9b768, WebGL/strict repeated builder PASS. Native Edge155,
  Playwright1.58.2, NVIDIA GTX1650Ti/ANGLE D3D11,1280x720:34/34 checks,
  0exceptions/provider reads/writes. All10frames per lamp;288active samples each.
  Front22.5599995..25.2784576, left15.6800003..16.2807598,
  right15.6800003..16.2647190. Emission gain, fixed roots, actual Pause frame/age/light
  and hidden Slayer phase pass. Actual word attack/parry/following attack/Slayer/menu.
- Pre-integration video hash95bdba41bc13660ed4dec180862c4a87e503868c5a55a1d1287d9a19edbe85f0:
  exported563/563 encoded frames with exact timestamps. Directly viewed72/72frames
  and70/70pairs, native320x180 and nearest2x,18paired sheets. Normal F64..91
  (7.219..8.786s), including incoming hit/recovery; parry F137..180
  (11.452..14.397s), preparation/contact/counter/following word attack/recovery.
  Wall/floor pools and player readability were observed; cage/root position retained.
  Palette acceptance from that review is superseded: later source/material audit
  found zero migration thresholds, confirmed by reopening after.png (pale-only fire).
  Earlier claims of preserved red/orange layers were incorrect. Native after/parry/restore stills and
  real1x/quarter playback checked. VFR lossy capture does not prove every engine
  frame, all563frames, pixel-identical GPU output or performance. Per-frame notes
  remain in selected/frame-evidence.csv. Source atlas remains unchanged:
  539be2b093203e571c5d30f49c8ece31f8769f07c0fa08496c9a97e6d1878912;
  reuse prior complete10source-cell/10transition review above the historical record.
- New main3710883 MetalClash: preserve ImpactVfxProfile as sole standalone pulse
  tuning owner. World combat profile owns templates/overlap/decay/ordinary+legacy
  events; parameters are snapshotted when emitted. New main2e77c70 camera impact:
  preserve its contact scheduler/priority/foot anchor/Pause behavior. Camera framing,
  pitch and recoil pixel size now read the active world profile; legacy no-arena
  camera fallback uses its actual pixelHeight, without a fixed dungeon target.
- Integrated input01543b8b82adc03a484b212287866b318802b54473b31e301a1df419fe86466a,
  run20261022T164856-quick:19/19 passed,0fail/skip. CameraImpactTests14,
  direct-entry3, projection/RT1 and three-cycle lifecycle1. Repeated-builder authoring
  audit passed again. Scene record IDs preserved: World1030original+1rune light-data,
  MainMenu477, SampleScene1024. Reordered serialized records retain equivalent data.

Final integrated evidence (supersedes earlier palette approval):

- Additional native fixture checks20261022T165821-quick, input
  e151dd0e4b406155aa6f04b43181ab08776e42ee59ee8425676e513b6dd3e976:
  4/4 passed,0fail/skip. Light-only profile hides the existing card without cloning
  a mesh; irregular frames, world switching and existing pixel-lantern lifecycle pass.
- Final QA20261022T165927-webgl, input
  c25b03c0040fe852621521f45b6d41d93354f427a77f2e7f6802252e2c425626,
  immutable player a484cfa7f3a82e38. WebGL and strict repeated builder PASS.
  Material thresholds .03/.12/.3 match the original shader and survive serialization.
  Local scenes match generated records exactly after whitespace/order normalization;
  all original IDs retained. Profiles/prefabs/material equal the built inputs.
- Native Edge155.0.4283.62 / Playwright1.58.2 / NVIDIA GTX1650Ti / ANGLE D3D11,
  1280x720:34/34 real-input checks PASS,0exceptions/provider reads/writes. Actual
  pause/resume, word hit/parry/following hit, Slayer/restore and menu return pass.
  All10frames each lamp;289active samples each. Front22.5599995..25.2697334,
  left15.6821041..16.2718716, right15.7007704..16.2819023. Emission, fixed roots,
  Pause frame/clock/light and hidden Slayer phase pass (1e-5float boundary tolerance).
- Final video SHA2563b24553c9e9c2f07f106ab4e182f26ebe911f3a1693c61c976ef0870b7f31dc6:
  exported651/651 encoded frames with original timestamps. Directly inspected all
  108selected frames and106consecutive within-segment pairs in28sheets, native320x180
  and nearest2x640x360. Normal F70..108(5.277..6.869s) includes enemy preparation,
  incoming hit and recovery; parry F165..233(9.187..12.106s) includes prior word hit,
  complete wind-up/guard/contact/counter/recovery, following word hit and return.
  Red/orange tongues and bright cores change locally; cage/root stays fixed, brick
  and floor pools persist, player remains readable through every reviewed recovery.
  Native after/parry/restored stills inspected. Playback controls ran at1x/.25x and
  advanced time; visual evidence is the complete selected sheets/stills. No whole651
  frame/every-engine-frame, pixel-identical GPU or performance claim.
  Per-frame notes/timing: integrated/selected/frame-evidence.csv and scope.json.
- QA observer removed from checkout. Production Build Settings retain only the
  three production scenes. No package, gameplay balance, UI or reflection change.
  Atlas/mesh/animation/controller files and existing GUID metadata remain unchanged.

Remote integration verified at73528adb3df60e2b148ed6bf6539f02070bbfff5 on
micharel09/TypingHero main: task HEAD equals fetched origin/main and the task commit
is its ancestor. Main2e77c70 is preserved. This final receipt changes documentation
only. Remove the clean task-created worktree/local branch after receipt ancestry
is verified; retain the remote branch and original dirty parent checkout.

Keep local evidence root for replay/reproduction; cleanup is safe when it is no
longer needed. Sixteen owned completed runner result directories were archived
locally then removed after confirming no active Docker mounts. All task jobs and
headless browser sessions exited; no new preview server was started. Shared
source/cache/toolchain, older preview server8228 and pre-existing resources remain.

Earlier failed migration/verification attempts are archived separately: missing
initial bake reference; Editor assembly refs; stale shared C# Bee graph; fixture
registration/prefab override errors; Editor-only bakeType player assignment.
The initial camera merge compile referenced removed WorldHeight; replaced it with
active profile height / actual no-arena pixelHeight. Partial/failing runs are not
claimed as passing. Shared cache/source and other jobs are preserved.

## Approved integration checkpoint - 2026-10-22

User approved final fire commit297c0c5 and requested integration into main. Merge current main916c752, preserving the new separate tutorial callouts and longer parry ember floor hold. Only documentation conflicted: retain current tutorial/VFX descriptions and approved fire lighting description. DungeonWorld merge changes only blank-line normalization outside the fire records. Fire runtime, shader, atlas, material, geometry, builder and focused tests are unchanged: reuse the recorded2/2Unity,35/35native and complete selected-frame reviews; no additional player build is needed. Run diff whitespace/conflict checks before push. Asset author/license remain unknown; user approval does not establish distribution rights. No game-host deployment requested.

Remove the clean task worktree/local branch after remote ancestor verification. Preserve remote branch, original dirty checkout and local evidence under E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t. Keep local demo server8228 available for replay; it can be stopped when replay is no longer needed.

# Combat hanging lantern ? restored final glass

## Pinterest flame - user approved, main integration, 2026-10-22

User approved https://www.pinterest.com/pin/1135681231035570556/ after rejecting both the unreadable NYKNCK runtime and the Pixie composite's art style. Continue the same isolated task worktree. This supersedes the visual direction below; earlier technical receipts do not verify this replacement. User approved the final real Unity demo and explicitly requested main integration. Source attribution/license remain unverified.

- Source GIF SHA256 `ac07257c81d688591ebbe67e309f80f37aeee4adcd06a1ccd75407b492e26d37`:10frames x100ms, complete1sloop, transparent500x500 image, exact5x expansion of100x100. All10frames and10transitions including wrap visually compared at100/200px, plus full source F0. Per-frame observations/source metadata: `E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t/pinterest1135681231035570556/reference-review.csv` and `source.json`. Original GIF timing is recorded from its100ms frame metadata; final runtime comparison and playback-control receipts follow below.
- Uniform source crop(22,21)-(76,101),54x80cells,5x2atlas270x160. Source colors/order/durations retained; material maps its four colors to brick red873B2B,dark orangeB6532C,warm orangeE48B3A,pale yellowF4CC79. Fixed bottom,80percent width, projected maximum height front10/rear12native world pixels at the existing camera.
- Reuse existing actor draw pass after environment blur with depth occlusion. Front24/6percent, rear16/2percent, Perlin/emission/shared lifecycle clock, per-lamp phase offsets unchanged. Builder and saved scene must agree before handoff. Runtime private UV meshes release on destroy; authored source assets/GUIDs remain intact.
- Scoped Unity20261022T132647-quick input18830d886955398a5f9a2885a620e7d9ba16910fec7660b6120467ecbc6dcd90: strict repeat builder and2/2existing focused light tests PASS. All10final raw cells/10loop transitions inspected at54x80and2x; Final GPU palette/runtime review is recorded below. Saved scene retains1022original IDs:7modified records+8new rear-card records equal verified builder after local-ID normalization; all other saved records retained. Material/quad equal builder ignoring trailing whitespace. Initial immutable QA build completed; final size-corrected evidence follows. No publication claim. Pin attribution/license absent; local review candidate only until external distribution rights are established.

### Final local verification

Pinterest size correction: initial QA player b2f131fd1d776174 passed35real-input/telemetry checks,0exceptions/provider writes on Edge155/Intel UHD/D3D11/1280x720, but direct native/4x closeup review found7/8native-pixel flames too small to read their silhouettes. Preserve this failed visual candidate in pinterest1135681231035570556/small-first-candidate. Increase only projected geometry to10/12native pixels; source atlas/timing/palette, runtime clock and physical lights unchanged. Reuse passing2Unity lifecycle tests; rebuild and recapture only affected visual integration.

Final local candidate (2026-10-22):
- QA20261022T140347-webgl, input70136ae1860409e6a97567e3d27d31993f318e71fc62b6d519d2f1aa72dd0050, immutable player5b494ce8e2ac2083. Strict repeated builder PASS. Reuse2/2focused Unity lifecycle checks from132647: runtime code, source atlas/timing/palette unchanged; only Editor projected geometry changed from7/8to10/12pixels after the initial visual failure.
- Native Edge155.0.4283.62/Intel UHD/ANGLE D3D11/1280x720:35/35PASS,0exceptions/provider reads/writes. Actual keyboard pause, parry, following word attack, Slayer/restore and menu return. All10cels observed on each lamp; active samples282each. Intensity main22.5599995..25.2148552,left15.6839170..16.2656498,right15.6800003..16.2859631;1e-5float boundary tolerance. Emission, fixed root, actual pause frame/clock and hidden-Slayer phase checks pass for all3.
- Full canvas export634/634frames with exact source timestamps. Selected complete normal-camera segmentF64..102(5.68129..7.302195s), including incoming hit/recovery, and parry preparation/contact/recovery/following attackF162..226(9.774305..12.774305s):104/104frames,102/102adjacent pairs viewed at320x180and nearest2x640x360. Also39/39normal native-capture2x three-lamp crops and1280normal/parry/restore stills viewed. Per-frame notes: pinterest1135681231035570556/selected/frame-evidence.csv. Video SHA256461cce307e4f1a1441df3a1d22ac224595bc26357a3e83ca04258706d72ddfc6.
- Red tongues/core change locally while fixture roots and warm wall/floor pools persist; player returns to normal illumination after hit/parry. Cage depth occlusion retained. F101 shows brief capture pixel-grid resampling across scene/cage; no claim of general shimmer absence. VFR25FPS lossy recording does not prove every engine frame, whole634frame clip review, or FPS/performance. User approved this final demo and requested main integration on2026-10-22.
- Final saved scene:1022original records/IDs retained;7changed+8new task records match final builder after local-ID normalization; material/quad equal generated values ignoring trailing whitespace. No package/camera/UI/floor-reflection changes. Disposable QA observer removed from checkout and retained only in evidence.
- Actual Unity demo: http://127.0.0.1:8228/pinterest1135681231035570556/demo.html (Orca page b275cbdb-b38f-48b6-93cf-b2724696e722). Video/closeup media and1x/quarter playback controls verified; default restored to1x. Retain local review server8228 and evidence root E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t until review finishes.
- Retain worktree C:/Users/ADMIN/orca/workspaces/TypingHero/lantern-pixel-fire and branch micharel09/lantern-pixel-fire for user review. Main integration remains held, and external asset distribution rights remain unverified; no new branch push/main merge/publication. Original dirty parent checkout preserved.

Resource cleanup: owned result directories132647-quick and140347-webgl removed only after local full-archive snapshot/player validation and confirming no active container mount. Full archives remain under the Pinterest evidence root; shared source/cache/toolchain, other jobs and prior resources preserved. All task check/build/capture jobs have exited; retain only the review server8228/browser tab/worktree/evidence until user review and source-rights resolution.


## All three pixel flames — review candidate, 2026-10-22

**User review rejected this NYKNCK candidate:** flame is not visibly readable in the playing camera. Prior technical/frame checks below do not establish accepted art quality. On2026-10-22 the user selected `https://pixie.haus/pixelart/20470/warm-glowing-ember-flame-animation` as the replacement direction. Reuse the existing branch/worktree; do not commit/push/integrate the rejected candidate as final. New target: a clearly readable upward flame with bright yellow core and orange/red tongues on all three fixtures, visible at the current playing camera. Inspect the complete linked animation and keep its source/output review separate. Main integration remains held for a new demo.

- User extended the NYKNCK free flame to both rear torches; retain the current task worktree and hold main integration until the expanded demo is reviewed.
- Shared unchanged five-cel atlas, 100ms/cel. Private animation offsets: front0s, left0.17s, right0.33s. Front24/range16/6percent, rear16/range15/2percent; all use continuous Perlin and synchronized per-lamp emission.
- Rear visible bodies reuse the existing fire-free shadow mesh; original FBX/materials and existing shadow proxies remain intact. Each new flame card fits its original fire-only world bounds, with a fixed bottom pivot. Shared atlas/quad/material, private runtime UV mesh per torch.
- Builder excludes existing PixelFlame renderers from torch-source extraction, avoiding recursive shadow children on repeat builds. No gameplay/UI/camera/floor reflection or public API changes.
- Evidence: `E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t/all-torches`. Scoped Unity2/2 and strict repeat builder PASS; QA playerf6eb222b78eb434e completed real input flows with zero exceptions/provider writes. Corrected read-only telemetry validation passes all3lamps (483samples each); raw aggregate-harness limitations are recorded in game/verification. Reviewed109/109frames and107/107pairs, native world and2x, plus all40normal three-lamp native-capture2x crops. Source atlas unchanged; prior5cel review reused. Full receipts/limits are in game/verification. The additional exact-saved-scene Unity audit is waiting for the shared runner; static seven-record/eight-node patch retains all original IDs.

## Current checkpoint ? 2026-10-21

Remote integration verified at `f1ab52b70eac8b2441abc1926b7dc0ec2d2d1e03`.
Both task303eeaa and latest main e36b02d are ancestors of remote main. Task branch
matches remote main. Parent dirty checkout remains untouched. No game publication.

Cleanup:28Unity logs/snapshots/XML archived in
`E:/CodexTemp/TypingHero/combat-hanging-light/lowpoly/rework/glass-final/integrated/retained-unity-receipts.tgz`.
Removed `/data/typinghero-game-loop/task-combat-hanging-light` only after acquiring
its exclusive lock and confirming0active owned containers; shared unity-home and
toolchain symlink targets remain. No task8217/8237preview listener remains. Keep
local players, images, selected video/frames and audit JSONs for review/reproduction.
The clean restored task worktree and its unused auxiliary restored branch are
removed after this receipt is verified on remote. The pre-existing task branch
and remote branch remain; their history is preserved on main.

This checkpoint supersedes historical candidates below. User approved thin glass,
one additional QA WebGL and integration into main; no game publication.

- Original worktree was removed externally during PlayMode. User authorized recovery.
  Restored from `E:/CodexTemp/TypingHero/combat-hanging-light/lowpoly/rework/interrupted-105914-source.tgz`
  into `C:/Users/ADMIN/orca/workspaces/TypingHero/combat-hanging-light-restored`,
  continuing branch `micharel09/combat-hanging-light`. Parent checkout untouched.
- Source FBX remains 472 triangles. Derived frame 446 triangles, glass reuses 40
  window triangles: total 486 submitted lantern triangles (beam separate).
- Glass: URP Lit, black base alpha .12, emission (1,.38,.08), no specular,
  alpha blending, ZWrite off, no cast/receive shadows. Child of LanternModel,
  inside environment hiding/restoration; excluded from flame flicker bindings.
- Key24/range16/flicker0; rear16/range15/+/-2percent. Iron/wood emission0.
  No camera/floor/post/gameplay/public API tuning in the lantern change.
- Correct HDR PlayMode contract: actual camera/presenter, 320x180 HDR Point world RT,
  1280x720, active Volume. Final glass7stills reviewed on110516/inputbfa57a81;
  settled off/on0changed pixels outside590,140..690,275cage ROI.
  First recovery pilot found Unity import removing emission and enabling premultiply;
  builder now fixes GI flags and PreserveSpecular. First pilot is not acceptance.
- QA110751/input046160c165646a5c3788e3b9cf90e90cfb9736e63224afcff7ee2ee742997e34
  player028bdab132b5e1c9: build and strict twice-run full builder PASS.
  Edge155/Intel UHD/D3D11/headed1280x720:22gameplay +9isolated camera checks PASS,
  0exceptions/provider reads/writes. Real attack,hit/parry,pause,Slayer/restore;
  glass hides/restores and retains fixed emission. All9camera endpoints inspected.
  Selected complete pan12.48..18.96s:163/163frames and162/162transitions viewed
  across11sheets; native0/80/162 inspected. No observed alpha pop/transparency
  overlap/floor outline/source drift. Source25fps lossy; no sub40ms/whole-video/
  realtime playback review claim. PTS preserved in motion-review/frames.json.
  Fresh matched20s baseline on current Intel UHD: n245/mean32.12816/p5032.350/
  p9533.225ms; final n250/mean31.38584/p5031.300/p9532.575ms. One run each/RNG/
  observer overhead; not isolated GPU cost, equivalence or FPS guarantee.
  Scene audit:10changed,228removed procedural blocks,8new model/glass blocks;
  no differences outside approved fixture blocks;0existing meta/GUID changes.
- Reuse lifecycle071929:2/2PASS, production runtime components unchanged so far.
  Earlier no-glass QA091233 had19gameplay/9camera checks PASS, visual cage too dark.
  Its 163frame/162transition pan is historical; final glass needs its own review.
- Timing baseline: same Edge155/GTX1650Ti/ANGLE D3D11/headed/1280x720,20s,
  rejected KayKit472/playered2d0d86653698d7: n559,mean13.67229,p5013.340,p9513.525ms.
  One run/RNG/observer overhead; no isolated GPU cost or performance equivalence claim.
- Evidence root: `E:/CodexTemp/TypingHero/combat-hanging-light/lowpoly/rework/glass-final`.
  Remote task cache: `/data/typinghero-game-loop/task-combat-hanging-light`.
  Preserve evidence; remove only task-owned cache after verified integration.
- User integration decision: latest main owns gameplay/UI; this task owns the full
  dungeon3D/camera/lighting/lantern. Preserve newest parry/impact systems, HUD/font,
  score rules and typing snapshots. Keep only camera-coordinate adapters in UI.
- Integration boundary: main f16ca37 plus subsequent1004724 documentation; world
  from303eeaa. Main nonworld scene blocks preserved, including stripped prefab
  records. Replace only old3D roots/cameras/global3D lighting, then run the main UI
  builder plus task dungeon pass. Keep main transient ParryContactLight binding
  because newest parry presentation consumes it; three steady sources unchanged.
- Integration source requires existing URP Core Runtime/Universal Editor assembly
  references from the dungeon baseline. Initial compile failures found omitted
  assembly refs; corrected. No new package dependency.
- Strict builder113353PASS, then final QA114427 strict builder PASS on
  input9a9081df02bf07286e197c4cf9ec1e5c3f0c49008fbc838b46fc8d47455dbc97.
  Snapshot canonicalizes YAML/shader CRLF to Git/Linux LF; font protection unchanged.
  Five existing main GUIDs preserved and seven serialized references updated.
  Earlier114008QA attempt was deliberately stopped before completion for this fix.
- Targeted offline results:1136554/4PASS (camera/RT lifecycle, fixed main, bounded
  rear and parry pulse),113808HUD lifecycle PASS but HP old231-width expectation
  failed against the approved171-unit rail. Main CombatR7Build explicitly owns171;
  updated only that exact width expectation, retaining0/1/50/100percent fill checks.
  Corrected HP rerun1/1PASS. Six distinct scoped checks PASS, no full-suite claim.
- Run strict full builder, targeted3dungeon lifecycle/projection tests and one
  integrated QA native camera/gameplay/HUD/projection smoke. Prior final material,
  163frame pan and20s timing remain evidence for the original isolated candidate;
  do not relabel them as final merged-source evidence without identity checks.



## Final integrated verification

- QA114427/input9a9081df02bf07286e197c4cf9ec1e5c3f0c49008fbc838b46fc8d47455dbc97,
  playerfdc8253ed7a62b1f: strict twice-run builder and WebGL build PASS.
- Edge155.0.4283.62/Intel UHD/ANGLE D3D11/headed1280x720:25real-input checks PASS,
  0exceptions/provider reads/writes. Attack/hit/parry, pause, Slayer/restore,
  three steady sources, fixed glass emission and glass hide/restore verified.
  Returning through the actual pause/MainMenu button releases all owned3D RTs
  and presentation cameras. Seven native state/menu stills directly inspected.
- Final scene, actor prefabs, UI/VFX outputs match actual QA serialized output
  after newline/trailing-whitespace canonicalization;0semantic differences.
  Existing main asset GUID changes0. Gameplay/UI/data/prefab/MainMenu paths match
  latest main exactly; only viewport/projection adapters change UI source.
- Main advanced during QA with documentation-only commits through e36b02d;
  preserve those receipts. No gameplay/render input change since QA.
- Reuse final glass material/pan evidence: active3D assets/settings/render code
  match303eeaa after GUID preservation and whitespace canonicalization, except
  sub6e-8legacy _Color round-trip values (URP uses unchanged _BaseColor).
  Root/camera/lantern transforms are exact matches. Prior163frame pan and20s
  timing remain isolated-candidate evidence; no integrated performance claim.
- Latest gameplay/main parry presentation retained, including its transient
  ParryContactLight; the steady lantern/rear lighting remains task-owned.
  No production build, itch upload or game publication.
- Integrated source committedc58beed, latest main e36b02d merged as1fa5ee2.
  Automatic merge added a duplicate, identical groundY prefab override; kept the
  already-verified single override. Runtime/build input diff c58beed..1fa5ee2 is0;
  only documentation changed, so passing QA is reused. Final diff versus main
  has0unexpected gameplay/UI paths and passes whitespace checks. Push/remote
  verification and task-resource cleanup follow these checks.

## Historical checkpoint (superseded)

# M?t ??n ch?nh cho v?ng combat

## Ph?m vi v? baseline

- Worktree ri?ng: `C:/Users/ADMIN/orca/workspaces/TypingHero/combat-hanging-light`.
- Branch: `micharel09/combat-hanging-light`; baseline snapshot `fc1bbfd` l?y t? to?n b? ngu?n dungeon ?ang c?. Kh?ng s?a worktree ngu?n ho?c main; kh?ng push/publish.
- K? ho?ch: thay ?u?c tr??c b?ng ??n khung s?t h?, treo x?ch v?o d?m tr?n hai c?t. T?m l?a t?i t?m ngang combat, tr??c nh?n v?t 2 WU, cao 3.65 WU t? s?n. T?i d?ng holder/node ngu?n s?ng v? GUID c? s?n.
- Ch?nh: realtime point 40/range16, rung 0%; ph?: 16/range15, rung t?i ?a ?2%. Emission c?ng h? s?, pause gi? phase, cinematic disable/enable kh?i ph?c phase.
- Gi? camera, floor, exposure/bloom, gameplay/public API, shadow atlas2048/tier512 ch?nh v?256 ph?; kh?ng ngu?n ?n.

## Ki?m ch?ng ?ang th?c hi?n

- Unity2022.3.25f1/pinned container, cache ri?ng `/data/typinghero-game-loop/task-combat-hanging-light`.
- Hai test ?nh s?ng c? li?n quan PASS (20261021T045916-quick): fixed key v? bounded rear/pause/hiding/emission. Kh?ng ch?y suite r?ng.
- M?t QA WebGL v?i strict repeat builder v? diagnostic clone scene ri?ng, kh?ng th?m mutating API v?o gameplay bridge.
- Native Edge155/D3D11/GTX1650Ti ?1280?720: baseline camera/attack/pause/Slayer11checks v? render timing4checks PASS. Baseline player20261021T041655-webgl/inpute1cf4516; c?c kh?c bi?t serialized scene/post so v?i input l? ??u ra c?a builder.
- C?n: build, ?nh c?ng pose/t?ch ngu?n, camera sweep, runtime final v? so timing; ch?a k?t lu?n visual/performance PASS.

## T?i nguy?n

- Preview/captures/scripts: `E:/CodexTemp/TypingHero/combat-hanging-light`.
- Gi? player/?nh/checkpoint ?? review. Cache/server task ri?ng x?a sau khi archive receipts, kh?ng ?i theo symlink unity-home/toolchain.

## ?i?u ch?nh sau ?nh runtime

- Pilot050027 build/repeat PASS nh?ng superseded sau s?a bounds l?a: ch? d?ng tam gi?c l?a ?? ??t t?m3.65, kh?ng d?ng c?c ??nh c?n ?u?c ch?a x?a kh?i vertex buffer.
- Candidate050651/inputeab0836e build/repeat v?13?nh ngu?n/camera PASS v? capture, nh?ng visual FAIL: v?ng ngang d??i ch?n tia s?ng t?i m?t, t?o d?i t?i; cube UV tr?i c? b?ng m?u l?m d?m nhi?u m?u. Kh?ng d?ng b?n n?y ?? b?n giao.
- ?? m? v?ng d??i (ch? gi? hai c?nh d?c ngo?i v?ng tia t?i nh?n v?t), n?i khung1.35WU v? gi? to?n b? caster s?t/g? th?t; th?m UV mesh d?ng ??ng ? palette s?t/g? trong atlas hi?n c?.
- Candidate050651 camera/gameplay checks ??t tr??c b??c telemetry, nh?ng observer ph? kh?ng g?i samples; kh?ng coi to?n b? script PASS. Observer read-only chuy?n v?o c?ng class diagnostic ?? ho?t ??ng. Kh?ng th?m API gameplay.
- Builder hi?n c? t? ??i38font refs trong scene baseline c?; gi? nguy?n38component UI ?? serialize c?a baseline khi nh?p output. Code font/UI v? baseline/runtime typography kh?ng ??i; builder t?i t?o c?c refs n?y ? c? hai b?n. Scene diff task ch? gi? lighting/khung treo.

## K?t qu? cu?i

- QA052033, input `b1602adbb0abaaaef224521bc109c6fc530fee550a4970d64619e9a968437e20`, player `2ee55b9f1b0faeae`: build v? strict repeat builder PASS. Pilot052033 ch? s?a timestamp observer: tar member tr??c ?? c?mtime0 n?n Unity cache gi? script ch?n ?o?n c?; kh?ng ??i ?nh s?ng/h?nh h?c.
- Native Edge155.0.4283.62/GTX1650Ti/ANGLE D3D11,1280?720:17checks PASS;240samples, key40..40, left15.68462..16.31872/right15.68202..16.30644; pause gi? phase, ngu?n kh?ng d?ch chuy?n, Slayer ?n/kh?i ph?c. Main world position(-.17452,.72245,-1.99105), t?c3.65WU tr?n floor hi?n t?i; kh?ng nh?m v?i world-y3.65.
- Camera b?nh th??ng(640,360), FOV35.000004, actor-scale delta0%; actual hit/attack/3parries/pause/Slayer/restore ?? ki?m.0exceptions/fixture reads/writes, external blocking. Unity2testPASS t?045916 ???c t?i d?ng v? runtime light source kh?ng ??i sau ??.
- Visual ab0035a181492c9a:13?nh (9pose +4t? h?p ngu?n) v?36sample ?2fps trong18s ?? xem. Kh?ng th?y b?ng d?i b?t th??ng/h? c?nh trong m?u ?? xem. Kh?ng tuy?n b? ?? xem m?i frame video. `visual-reuse-proof.json`:287assets byte-identical, scene lighting/geometry identical;38TMP refs v? renderer c?a AttackCue c? ?ang inactive ???c gi? theo baseline trong source. Baseline/candidate ??u d?ng builder font code c? kh?ng ??i.
- C?ng ROI face: hero meanRGB47.21?67.01, skeleton50.48?98.47; enemy weapon21.50?41.15. ROI v? kh? hero51.02?40.90 (background t?i h?n, silhouette v?n r?): kh?ng ??ng nh?t ?? s?ng v?i ?? d? ??c. Full-black world ROI RGB0.
- Combat20s m?i b?n: baseline n488/mean15.798/p5013.350/p9526.765ms; final n428/mean18.262/p5013.400/p9526.865ms. Mean t?ng15.6%; p50/p95 kho?ng0.4%. Final3sample>50ms, baseline0. M?t l??t/RNG/timing combat/observer QA kh?c nhau n?n ch?a quy nguy?n nh?n ho?c ch?ng minh chi ph? t??ng ???ng; kh?ng tuy?n b?60FPS. ?? ho?n th?nh ph?p so theo y?u c?u, kh?ng m? r?ng stress/soak/suite.
- Gi?1578/1578sceneIDs g?c; ch? thay block ??n/mesh/transform/khung m?i. Kh?ng ??i renderer settings/camera/floor/postprocessing/GUID g?c/public API. Hai worktree ngu?n/main kh?ng s?a b?i task.

## Preview v? t?i nguy?n b?n giao

- Ch?i `http://127.0.0.1:8217/`; ?nh/video `http://127.0.0.1:8217/review.html`.
- Local `E:/CodexTemp/TypingHero/combat-hanging-light`: immutable player, before/after,comparison,sources,camera-sweep,preview.mp4,36sample frames, JSON v?Unity receipts; gi? ?? review, d?n khi review/t?ch h?p ???c ch?p nh?n. Preview ch? local, ch?n k?t n?i ngo?i; kh?ng ph?t h?nh.
- Cache remote task ???c d?n sau khi archive receipts v? package v? local; shared unity-home/toolchain v? dungeon-3d cache kh?ng ???c theo symlink ?? x?a.

- Cleanup ?? th?c hi?n: x?c minh kh?ng c?n container d?ng task root r?i x?a `/data/typinghero-game-loop/task-combat-hanging-light`; c?c symlink tr? shared home/toolchain kh?ng b? ?i theo. Local preview server session29053 gi? cho review; d?ng server v? x?a local temp sau khi review ???c ch?p nh?n. Branch/worktree gi? ?? ti?p t?c/t?ch h?p theo y?u c?u m?i.
- HTTP preview200, t?t c? review routes200/no-store/version2ee55b9f1b0faeae; actual HTTP output.data SHA256 kh?p immutable player. Diff whitespace PASS; source Settings/post/floor/meta GUID g?c kh?ng ??i.

- Cu?i c?ng b? trailing spaces c?a8file YAML asset/meta m?i do Unity xu?t ra; ch? kho?ng tr?ng cu?i d?ng, d? li?u mesh/GUID kh?ng ??i. Ki?m `git diff --cached --check` l?i sau khi stage.


## NYKNCK free pixel flame ? 2026-10-22

- User selected the free five-frame Fire Set, not paid Bonfire. Source: https://nyknck.itch.io/pixel-fire-set; credit @nyk_nck; source archive SHA256 08b78ec0fe4ee9f4d5627e57f0a7013ea7edbc3fb279cb320f9f83ee0ecd091b.
- Task branch micharel09/lantern-pixel-fire, base origin/main 4e62ea2. Preserve the dirty parent checkout.
- Isolate each largest 8-connected alpha body on the authored native pixel grid, bottom-anchor in 48x34 cels, keep original colors and 100ms timing. Main lamp only: private runtime quad/UV flipbook and continuous bounded Perlin noise; base24, amplitude6percent, emission synchronized. Rear sine/+/-2percent behavior unchanged.
- Original lantern/mesh/material GUIDs retained. No gameplay/UI/floor reflection changes. Full source and final output frame review remain separate.
- Original acceptance (waived by user): scoped front/rear Unity tests, strict repeated builder and persisted DungeonWorld configuration, all five output frames/transitions at game size/2x, one short real-input gameplay capture including parry/recovery and pause/Slayer restoration. No broad suite/multi-viewport/stress acceptance.
- Local evidence: E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t. Source five frames/transitions inspected; paid promotional preview is excluded from output. Runtime verification is not yet complete.

### Pixel flame verification

- Unity2022.3.25f1 scoped Quick20261022T081730: **2/2 PASS**, front lifecycle/mesh/emission/clock test and directly affected rear torch regression. Strict repeated builder PASS on input195216041bd12cdd99291d0b9827c5fa31c4ea79ab69565cad5f940cf8034c3b.
- Initial Windows CRLF-only font protection failure was resolved in the disposable Linux snapshot (canonical text LF; binary bytes retained), not by changing font assets or weakening guards. Earlier material GI/alpha-mask and room renderer-enable idempotence failures were fixed in the new flame builder; failed receipts retained locally.
- QA20261022T082242, inputcf8366fc823770ecf1ef786e66facf8a37bef5ff0fb648a78c55a65350ae00f4; player5f849262aed3f42e. Native Edge155.0.4283.62 / NVIDIA GTX1650Ti / ANGLE D3D11 at1280x720: **21/21 PASS**,0exceptions/provider reads/writes,481read-only lamp samples,186hidden. Observed active intensity22.5750618..25.2119675, all5cels; shared emission, root, pause and Slayer phase verified. Shipping runtime/assets unchanged by the QA-only observer, which is excluded from the commit.
- First browser pilot is INVALID for gameplay acceptance: tutorial skip reloaded the encounter after its early Enter. Fixed only the harness settling/wait; reused the same player for the passing run.
- Source GIF original timing is5x100ms. Final adapted atlas240x34 SHA25614b26a8dc4566e7c7079107054cfbcc9056432b0b33c3fe3a9cb857cf7cce910; no interpolated colors. Source reference review and output review remain separate.

| Output cel | Duration | Source | Observation at native48x34/nearest2x | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|
| F0 |100ms|1.png, largest alpha body|Hai c?m ch?nh n?i ch?o t?o th?n l?a nghi?ng; palette v?ng/cam g?c, ??y ? h?ng32.|Sang F1 th?n ng?n l?i theo ngu?n; pivot ??y kh?ng ??i.|??t|PixelFire.png cell0; local output-frame-0.png|
| F1 |100ms|2.png, largest alpha body|Th?n nh? nh?t, d?ng n?m nghi?ng ph?i; m?p pixel v? ch?n c?n trong canvas.|F1?F2 k?o d?i th?nh gi?t; gi? ??y33 v? t? l? pixel chung.|??t|PixelFire.png cell1; local output-frame-1.png|
| F2 |100ms|3.png, largest alpha body|Th?n gi?t d?i m?u v?ng cam, ?u?i d??i nh?n; kh?ng clipping.|F2?F3 r?ng th?m, c? v?ng t?i gi?a th?n theo ngu?n; ch?n kh?ng d?ch.|??t|PixelFire.png cell2; local output-frame-2.png|
| F3 |100ms|4.png, largest alpha body|Th?n r?ng c? l? t?i v? ??m ch?n n?i ch?o theo alpha g?c; kh?ng v? th?m pixel.|F3?F4 k?o cao theo frame ngu?n; v?n n?m trong envelope ??n.|??t|PixelFire.png cell3; local output-frame-3.png|
| F4 |100ms|5.png, largest alpha body|Th?n cao nh?t d?ng cong ch? S v? ?u?i cam; ??y c?ng h?ng32, gi? palette g?c.|Loop F4?F0 ??i silhouette theo ngu?n, kh?ng reset pha ?nh s?ng ho?c ??i pivot.|??t|PixelFire.png cell4; local output-frame-4.png|

- Actual canvas SHA256c4b65aa7482de36f7d93ee8a9144fc012c4eb02b6443e0d50006333bbeec86a3;640decoded/exported with exact timestamp agreement. Before review selected normal6.212475..7.830410s(F77..116) and first parry9.834410..12.234410s(F166..223), covering windup/contact/recovery and a following normal hit. **98/98selected frames and96/96adjacent transitions actually inspected** at logical320x180 and nearest2x640x360; all40normal lamp crops additionally inspected at2x capture. Actual1280x720 after/parry/post-hit/restored stills also opened.
- Warm light remains on 3D floor/actors, fire varies inside a stationary cage; no full-room flash or whole-player recovery blackout in those segments. Source five-cel silhouette changes are retained. VFR25FPS capture is not every engine frame/high-frequency shimmer or performance/FPS evidence. No full suite, extra viewport or stress run.
- Full builder also regenerates unrelated legacy scene records; committed DungeonWorld instead preserves all1018original IDs and every out-of-scope record. Only old front renderer, front holder child list and front torch binding/settings change, plus4new pixel-node records. New-node transforms match generated output; original materials/meshes/GUIDs remain unchanged.
- Local frame-level runtime observations: selected-review/frame-evidence.csv (98rows), source/output hashes and scope.json; source animation review has its own source-frame-review.csv. Retain local evidence until integrated result is reviewed; no public game publication.

- Direct committed-scene load audit20261022T084400-serialize PASS (input5e6eb9b5d04ad1a0020033c53c55a8b9f303f60d94f031d2c4ac109913b49f95): Unity opens the exact scoped DungeonWorld scene and asserts3torches, front24/.06, rear.02/no pixel binding,4vertex private-quad template, renderer enable/array links, old flame disabled,240x34 Point/no-mip atlas. This is a read-only scene audit, NOT another full repeated-builder run despite the stock runner completion label. Initial optional audit wait timed out on the shared lock; retry passed, other jobs preserved.


## Complete 3D configuration - 2026-10-23

- Scope: blur, haze/rune, cloth, camera impact and multi-world builder. Preserve
  source atlases/meshes, combat and Point/HDR/MSAA. Orca branch
  `micharel09/finish-world-configuration`, base `3253274` from latest origin/main;
  original dirty checkout preserved. Retain main changes after plan baseline bab5ee6.
- Blur migration is explicit in production/fixture assets: offset .75 -> center 0,
  half-width .75, depth edge .5; unchanged Gaussian/taps, no builder migration.
  Haze/rune use existing materials. Cloth captures profile values on enable.
- CameraImpactProfile retains approved defaults; scalar envelopes/duration snapshot
  at emission, curve copy at binding, no objects per hit. Gameplay binds selected
  virtual lens/profile before activation; two-profile A -> B -> A test added.
- Builder uses gate definition and serialized owner/references, applies worlds
  reachable from enabled scenes, supports assets outside the lighting folder and
  ignores unused templates. Repeat/export include actual dependencies. Preserve
  authored camera/room placement and restore scene setup.
- Initial Unity `20261023T031358-quick`, input
  `06f699768bfb430ccf0bf808e186fe40540ede4a88d3b1c1744b98a4b6d4dcd7`:
  42/43 PASS, 1 FAIL. PlayerControllerCelsHaveAlignedNormalAndMaterialBindings
  still expects old male_hero-combo_2_end.png recovery, removed by current main;
  no normal-map/controller/atlas change or weaker assertion in this task.
  Relevant camera/environment/bindings/session cases pass, including zero managed
  bytes after warm-up. The aggregate run is not PASS.
- Editor pilot `20261023T031700-serialize` failed restoring empty scene setup in
  batch mode. Empty setup handling fixed; additive scene setup/placement checks added.
  Final Editor/filtered runtime/WebGL/native visual acceptance remain pending.
- Local evidence/adapters: `E:/CodexTemp/TypingHero/finish-world-configuration-20261023`.
  Remote temporary source/build/results: `/tmp/typinghero-world-configuration-20261023`.
  Shared lock/cache/images/license preserved. Only this task's doctor source/archive
  removed after marker/mount verification; old results untouched, thresholds unchanged.
  External adapter uses testFilter and canonical text LF without changing font content.
  Waiting on other jobs holding the shared runner lock; do not stop unrelated jobs.
- Integration and cleanup are recorded in the final local `acceptance.json` receipt;
  historical failed pilots below/above do not count as acceptance.

- Second Editor pilot `20261023T032714-webgl`, canonical input
  `6a3dd2b2fdaa63562e4a038100393fb19fc01c5230478a7ce271a733af5ea322`,
  exposed legacy HUD camera Y overwrite (+.110096929). Authored room stayed fixed.
  Default world pose is now captured before UI passes and restored by the 3D pass;
  no camera pose is computed from dungeon-specific geometry. Preview Undo now also
  records affected combat rig components. Final repeat/runtime/WebGL rerun queued.

- Final source input `6b1258a53256b28cc07ab6bf716e8cf40ca994514d8ad31d31c7172f665299fa`,
  run `20261023T033400-webgl`: Editor authoring/additive setup/arbitrary naming/
  selected outside-folder assets/unused-template/placement checks PASS.
  Filtered PlayMode39/39 PASS,0failed/skipped,9.9696019s. Strict repeated builder
  PASS; WebGL linker and native acceptance still pending. Runtime/config source
  is frozen for this candidate; reuse scoped evidence while it stays unchanged.

- Final WebGL/strict repeated builder PASS on the same6b1258a5 input. Downloaded
  immutable player33/33 SHA256 matches,48,078,641bytes. Native Edge155.0.4283.62 /
  Playwright1.58.2 / NVIDIA GTX1650Ti ANGLE D3D11,1280x720:53/53 PASS in37.7076506s,
  zero exceptions/provider reads/writes/external requests. Includes accepted normal/
  received/parry, positive active-pulse Pause freeze, Slayer/lethal, actual typed
  post-parry follow-up, Restart and Menu restoration. First driver attempt paused
  too late (pulse already0); retained as failed, timing corrected using real keyboard.
- Direct visual review COMPLETE:46/46 sheets;355/355 frame observations and350/350
  adjacent pairs across5windows (340unique frames/336unique pairs due to overlapping
  normal/received windows). Raw video1048frames; source PTS windows0-1.148s(F0-18),
  .163-1.682s(F4-43),13.201-14.974s(F507-581),18.897-20.673s(F735-805),
  22.827-25.997s(F898-1047). Native320x180 and nearest2x
  640x360 shown side-by-side; held/repeated frames and sheet-boundary pairs retained.
  No observed change to warm flame colors, anchored fixtures, floor/wall pools,
  focal actor clarity or room restore. Cloth is dark/subpixel; deformation/pinning/
  independent local direction/private mesh/phase rely on the scoped Unity lifecycle test.
- VFR timing limit: source F10(.302s)->F11(.950s) has648ms gap; keep it, do not
  interpolate or infer every-engine-frame/FPS behavior. Warm F1008-1031 provides
  full normal-hit/recovery motion. Reuse unchanged source atlas/mesh/clip review;
  baseline camera-impact native sheet and Pause image compared. No full-video,
  new reference-match, audio-signal or performance-improvement claim.
- Evidence root: `E:/CodexTemp/TypingHero/finish-world-configuration-20261023`.
  Final receipt `acceptance.json`; scoped Unity XML/editor/build logs under
  `20261023T033400-webgl`;53-check report/video under `native`; per-frame observations
  and exact source windows under `native/frame-review/manifest.json` and
  `frame-evidence.csv`. Video SHA256
  `089b3c6e2638053c487a852bb560baad7e74c0971d803ea068e3d56debbf5619`.
  Keep these evidence/player artifacts for reproduction/review; delete after review
  when no longer needed. Disposable task source/worktree/profile cleanup is required
  after verified integration; final receipt records actual completion.

- Final read-through found multi-scene Preview wrote RenderSettings into the active
  scene. Fixed only Preview scene selection/Undo recording and restored active scene
  in finally. Standalone VerifyPreviewScenes `20261023T035720-serialize` PASS on
  input530e3293: each world's fog correct, active scene retained, Undo restores both.
  Generic runner completion wording does not represent another repeated-builder run.
  Subsequent indentation-only adjustment has no semantic change. Production Apply
  method and all runtime/shader/asset inputs are unchanged from6b1258a5; reuse the
  passing39Unity/53native/WebGL/visual results. `preview-isolation-proof.json`,
  `preview-unity.log` and `preview-snapshot.json` retain exact evidence.

## Preview ownership and complete scene Undo follow-up (2026-10-23)

- Independent Orca worktree `fix-world-preview-undo`, branch
  `micharel09/fix-world-preview-undo`, base `9acdb4e`. Dirty primary checkout
  preserved. No runtime, shader, profile, material, geometry or GUID changes.
- Preview/Apply resolve combat rigs from the world scene plus explicit roots of
  gates selecting that exact definition. Distinct targets are shared with Undo;
  unrelated gates/unbound rigs retain their profiles. Changed gameplay scenes
  are marked dirty and prefab modifications recorded.
- One Preview creates one Undo group including each scene's native
  LightmapSettings reference, RenderSettings and affected combat rigs. Null
  profile baking retains the existing scene reference. The cached internal
  LightmapEditorSettings.GetLightmapSettings contract is pinned to Unity2022.3.
- New Editor checks copy world/gameplay scenes and styles into a temporary folder,
  then restore scene setup and delete only those copies in finally. Covers A/B
  gate selection, detached/duplicate roots, local/unbound rigs, grouped Undo/Redo,
  prior and null bake references, unchanged authored JSON, scene dirty/setup and
  save/reopen persistence. Existing multi-scene fog/active-scene test is retained.
- Reproduction `20261023T063030-serialize`, input
  `b3b76993724dd4b2a66c740098ed40a0531bf50f125e7a9d6fa8a6820ce2a859`,
  executes the unchanged base authoring code with the regression checks: FAIL as
  expected, confirming missed split rigs and missing bake-reference Undo.
- First fixed-source pilot `20261023T063235-serialize` still executed the cached
  old Editor DLL: CombatRigs was absent despite the new source. Not acceptance.
  The external snapshot adapter advances only this source file's timestamp after
  baseline injection; no source/test assertion or build threshold was weakened.
- Accepted pre-integration input
  `98a91cc6109419f190e5046bca6003b2fd22c7ce33063477d7541d73e5a00c2c`,
  run `20261023T063454-serialize`: new Preview regression checks PASS, existing
  multi-scene fog/Undo PASS, strict PixelUIBuild repeat PASS with both worlds
  selected. The compiled Editor DLL contains CombatRigs. No runtime/full-suite,
  WebGL, frame-review or performance claim; those paths are unchanged by this fix.
- Evidence: `E:/CodexTemp/TypingHero/fix-world-preview-undo-20261023`, including
  `repro`, `fixed-pilot`, `accepted` logs/snapshot receipts and external adapters.
  Server private source/cache/Library/HOME/results are under
  `/tmp/typinghero-preview-undo-20261023`; shared runner lock/images/license/cache
  preserved. Private writable copies avoid the shared filesystem's <3GiB free
  condition while retaining the original 3GiB threshold on the actual filesystem.
- Newer main `94911a8` changes impact pulse lifetime/legacy-light cleanup. Merge
  and scoped Editor/repeat verification are required before remote integration.

- Integrated main94911a8 as42ef2a5 without conflicts in source; both authoring-guide
  additions are retained. Post-merge run20261023T063803 (eb875c2e input) passes
  the two Editor entrypoints and strict repeated builder.
- Final fixture explicitly uses the real CombatLightingRig prefab for the detached
  gate root and asserts prefab identity plus the selected profile after reopening.
  This closes the earlier fixture's plain-object-only persistence coverage.
- Final input `00654f10733f1ef8d8d740f3638135e98cb7de7f222b332153663ee7b219876c`,
  run `20261023T064045-serialize`: split/multiple-world Preview/Undo/Redo/null bake/
  unchanged assets/save-reopen including actual prefab override PASS; existing
  multi-scene fog/active-scene/Undo PASS; strict repeated builder with both worlds
  enabled PASS (TYPINGHERO_QA=1). No exceptions or regression markers in final log.
  Final evidence is under `final`; earlier passes/pilot remain attributed to their
  own snapshots. Documentation-only receipt updates do not change tested inputs.
- Remote verification and resource cleanup are recorded in the evidence root's
  `acceptance.json`. Retain logs, snapshot/patch/audit receipts and the remote task
  branch for review/reproduction; they may be deleted when review is complete.

## Camera prefab overrides and strict QA precondition (2026-10-23)

- Independent Orca worktree/branch `finish-camera-prefab-preview` /
  `micharel09/finish-camera-prefab-preview`, based on remote main95f978b.
  Dirty primary checkout preserved. Scope is Editor/test/docs only; existing
  asset GUIDs, runtime, shaders, profiles, materials and world geometry unchanged.
- Apply records prefab overrides for Camera, DungeonArenaCamera and native URP
  camera data even outside environment. The same three camera targets are used
  by Preview Undo; duplicate targets in the environment hierarchy are removed.
- VerifyPreviewAndRepeatBuilder rejects every QA value other than exactly1
  before calling scene/profile/builder code. New checks exercise unset and0,
  compare scene setup/Build Settings and restore the original environment.
- New temporary camera-prefab fixture changes FOV/near/far/half-height, arena
  profile and renderer slot; swaps the two existing renderer references/indices
  only in a cloned profile to keep validation valid. Tests Preview, Undo/Redo,
  save/reopen and direct Apply without Undo; source prefab bytes and cloned
  profile JSON must remain unchanged. Temporary assets/scenes are deleted in finally.
- Fixture pilot20261023T071305, cd096ba0 input: ArgumentNullException before
  camera assertions. Creating a scene after cloning unreferenced temporary assets
  allowed Unity to unload them; scene creation now precedes cloning. Not acceptance.
- Baseline production code95f978b with corrected fixture, run20261023T071522,
  input `bd5ad160038706b2e8d059c3d11768edb469cc4eb805d04bb82a9b129d7702fd`:
  expected FAIL for the combined arena-profile/renderer assertion on Redo and
  subsequent Preview save/reopen. Lens and direct-Apply checks did not fail in
  this environment; do not claim that every camera field was lost on baseline.
- Accepted pre-integration run20261023T071834, input
  `7f41e55138b4b67f732e6c5167c30947e218df2c1d4d9389b7ee7acb5b0e1353`:
  QA unset/zero rejection PASS; camera prefab persistence/Undo/Redo/direct Apply
  PASS; existing split-world combat/bake/fog/Undo checks PASS; strict repeated
  builder with both worlds selected PASS. TYPINGHERO_QA=1 for the valid path.
- Evidence root `E:/CodexTemp/TypingHero/camera-prefab-preview-20261023` contains
  fixture pilot (`repro`), genuine baseline (`repro-initialized`), accepted logs,
  snapshot receipts and external adapters. Private server root
  `/tmp/typinghero-camera-prefab-preview-20261023` uses private copies of Library,
  cache/HOME (unused npm cache omitted) and the shared exclusive runner lock.
  Images/activation/shared data are preserved; original3GiB threshold remains on
  the actual private filesystem. No full suite, PlayMode/WebGL, visual or FPS claim.
- Main advanced to15d538c with combat-light pulse clock changes. Integrate this
  source and recheck the scoped Editor acceptance before remote publication.

- Integrated main15d538c asb4fbb7d without source conflicts; retained both guide
  additions. Final scoped rerun20261023T072145, input
  `2d28112b52ea6befbbb4558b733326a8da35d25f88507416f6fdc5e473b7c9c8`:
  all camera prefab/QA guard/previous Preview/Undo checks PASS and strict repeated
  builder with both selected worlds PASS. No regression markers or unhandled
  exceptions. Only receipt documentation changes after this tested snapshot.
- Final evidence/logs/snapshot are in `final`; `acceptance.json` records actual
  remote integration and cleanup. Retain evidence and the remote task branch for
  review/reproduction; evidence can be deleted when no longer needed, and the
  remote branch only when requested. Task-created local worktree/private server
  root must be removed after verified remote integration.

## Dynamic sprite shadows - strong contrast, 2026-10-23

User selected strong contrast. Base main `1f22565`; independent Orca worktree
`dynamic-sprite-shadows` preserves the dirty primary checkout. Hybrid route:
alpha-clipped sprite cards cast into the existing 3D scene; semantic/form normal
planes relight their surfaces. No limb-on-body volumetric self-shadow claim.
Gameplay, contact timing, albedo/atlas, existing GUIDs, geometry and pixel camera
settings are unchanged.

- Key stays intensity24/hard512. Rear fills become intensity8/shadows off.
  Parry uses hard256 shadows with bias0.015/normalBias0.02; eight additional
  lights/object admit authored world lights plus the one selected combat light.
  Existing strongest-pulse scheduling, snapshots, clocks, holds and release stay.
  Light fills shadows spatially; no global shadowStrength animation was added.
- Re-authored29 existing normal PNGs/261 sliced poses using semantic material
  regions and broad form planes, never luminance-as-height. Ordinary Prepare now
  preserves painted bytes and custom secondary bindings. The destructive draft
  replacement is an explicit Editor menu command; offline recipe `--write` is
  also explicit. See [authoring guide](../game/lighting-profiles.md).
- Reference: [Acquire combat lighting](https://www.unrealengine.com/spotlights/octopath-traveler-s-hd-2d-art-style-and-story-make-for-a-jrpg-dream-come-true)
  directly describes point lights with combat VFX casting character shadows on
  environments. [Dead Cells developer interview](https://80.lv/articles/interview-with-the-developers-of-dead-cells)
  discusses normal-map lighting. Prior research directly inspected129 GIF frames/
  127 pairs; this does not establish its cast-shadow algorithm or visual parity.

Evidence root: `E:/CodexTemp/TypingHero/dynamic-sprite-shadows-20261023`.
`review.html` links A/B videos, timestamp manifests, observations and measurements.
Retain this evidence and the remote task branch for review; remove only when no
longer needed/requested. No external publication or full-suite/stress/soak run.

| Acceptance | Actual evidence |
|---|---|
| Frozen baseline | main1f22565, input2d28112b52ea6befbbb4558b733326a8da35d25f88507416f6fdc5e473b7c9c8, WebGL run20261023T081906 |
| Scoped Unity | run20261023T082941:43 tests PASS across SpriteShadowTests, ImpactVfxClockTests, ImpactVfxTests, LightingProfileTests |
| Final warmed light tests | run20261023T083737:2 SpriteShadowTests PASS, including zero managed allocations after warm-up; overlaps the43 above |
| Authoring | run20261023T083209:custom binding/painted-byte preservation, Preview/Undo/Redo/save-reopen/fog, strict two-world repeat PASS |
| QA player | run20261023T083500, input626e54e88dcb554f27efec2a89b18b3e3f2a7b661f2a16deeac2993137a47a61:strict repeat + WebGL build PASS |
| Real input | baseline/candidate each14 checks PASS:normal hit, received hit, parry, Pause/Resume, follow-up, Slayer/victory, Menu cleanup; no exceptions; network fixtures/blocking enabled |
| GPU pose diagnostics | run20261023T083900:1044/1044 cells directly viewed,261 poses x left/right x facing; run20261023T084553:10/10 above/below cells, llvmpipe LLVM15; alpha-shaped floor casts, no rectangular card shadows |
| Normal previews | 261/261 poses viewed across33 sheets, albedo/normal/left/right/mirror; illustrations are not Unity PBR acceptance |
| Data | source-audit.json:29 normal dimensions and bytes match QA output; existing metas/albedo/animation unchanged. Only two rear Light records copied from tested serialized output, retaining original scene IDs/geometry |

Native GPU: Edge155/ANGLE D3D11, GTX1650Ti,1280x720. Selected encoded frames and
all consecutive transitions reviewed at internal native320x180 and nearest2x
actor/floor crops; full1280 stills also inspected. A/B captures use phase mapping,
not identical wall-clock inputs. Actual coverage:

| Recording | Complete selected windows | Viewed frames / transitions |
|---|---|---|
| baseline-visual | parry/follow-up F12-277; final guard-break preparation through Slayer restoration F605-849 | 266/265 +245/244 |
| candidate-visual | parry/follow-up F18-289; final guard-break preparation through Slayer restoration F613-861 | 272/271 +249/248 |
| baseline-continuous | normal preparation/contact/recovery F11-64 | 54/53 |
| candidate-continuous | normal preparation/contact/recovery F11-57 | 47/46 |

Manifests record exact requested/encoded PTS and source hashes; CSV rows record
observations per frame and preceding transition. Held/repeated encoded frames
remain included. Initial visual normal windows have a recording gap and are
superseded by continuous captures without screenshots. No claim to inspect
unrecorded engine frames, every full animation clip, or the whole source videos.
Static GPU pose coverage remains separate from temporal WebGL coverage. Native
and quarter-speed playback controls are verified in playback-controls.json;
frame inspection is the visual evidence. Warm torch color, actor readability,
spatial floor light, cloth/blur and return from Slayer showed no selected-scope
regression. Intentional Slayer black/red silhouettes and source hurt flashes stay.

Separate non-recording runs, matched active/unpaused/non-Slayer samples401 each:
RAF p50/p95 baseline18.295/19.025ms, candidate18.290/19.020ms. Observed WebGL draws
p50/p95 277/281 to165/184; 256/512 shadow-tile viewports18/18 to6/12. These are
observational browser counts, not GPU-only timing or engine profiler pass counts.
No FPS improvement claim. No >10% p95 regression, so no ABBA escalation required.

Pilot/environment notes: initial baseline stopped before Unity below the3GiB
free-space threshold. Removed only unused private npm cache; shared toolchain,
license and caches preserved. Private tmpfs runner uses the shared exclusive
runner.lock. First offscreen diagnostic image needed URP shadow-resource warm-up;
recaptured warm full/vertical sets. Exactly1043/1044 and9/10 other images are
byte-identical to already viewed pilots; changed first images viewed separately.
Latest diagnostic inputa17b4bb5d7844a29f130f66f48a207bc82bb2c82bd4f292b52bde54aa987eed0
adds only Editor capture warm-up and test changes relative to QA runtime inputs.
Final source audit preserves that snapshot; only two serialized rear-light records
now mirror tested builder output. Remote integration/cleanup receipt: acceptance.json
in the evidence root, populated after publication.


## Per-frame depth self-shadow implementation (2026-10-23, in progress)

Task base origin/main `69ed62c`; visual baseline `1a7895c`. Independent Orca
worktree `sprite-self-shadow`, branch `micharel09/sprite-self-shadow`. Dirty
primary checkout preserved. Evidence root:
`E:/CodexTemp/TypingHero/sprite-self-shadow-20261023`.

Contract: authored linear R depth (front = local -Z), alpha occluder coverage,
Point/no mip/no compression, matching sprite atlas UV. Material defaults .65
strength/.15 local depth/.01 local bias. Eight probes/light, at most12 source
texels, bounded to the current frame. Ambient/emission and existing alpha/depth/
shadow-caster passes remain unchanged. URP14.0.11 installed Lighting.hlsl was read
from fly-server package cache; direct lighting uses its existing BRDF/AO/shadow
helpers. This is planar approximate self-occlusion, not character geometry.

Production is not enabled: world definitions have no depth library. Current
fixtures author only hero combo_3 frame1 and sword Attack1 frame2, with explicit
pose polygons recorded in `Assets/Tests/Fixtures/Lighting/SelfShadow/poses.json`.
These are prototypes, not full-clip or all-actor acceptance. Required next gates:
48 same-pose GPU captures (two actors, four white-light directions, two facings,
flat normal/off, authored normal/off, authored normal/on), prototype performance,
then controller-derived complete authoring and per-frame/transition/loop reviews;
real1280x720 WebGL input/lifecycle and separate same-build on/off p95 comparison.
If >10%, confirm ABBA, then only four-probe fallback is permitted. No visual,
performance, runtime or integration pass is claimed yet.

First focused test attempt stopped before Unity: private runner copy was still
in progress and activation state had not reached its private HOME. No test ran.
Private runner `/data/typinghero-game-loop/task-self-shadow-20261023` uses the
shared exclusive lock, pinned image, copied Library/cache/HOME; no shared data
is modified. Retain until verification completes, then remove only owned runner
and clean integrated worktree. Local A/B evidence must remain for review.


Prototype verification receipts (not full acceptance):

- Focused Unity run `20261023T115125-quick`, input
  `3aab147d599954c4b49d59687614bb1a69ab3074db721f57439aa9a658ad4d93`:
  3/3 tests passed (SpriteDepthTests and two SpriteShadowTests), zero failed/skipped.
  Frame/facing refresh, missing-frame fallback, clear/rebind, combat flash/Slayer
  property preservation and256 warmed updates with zero managed bytes passed.
  The subsequent explicit second-library A/B/A assertion was not in this first run.
- GPU pilots `20261023T115449-serialize` and `20261023T120022-serialize` each
  exported48 images successfully. The first had incorrect skeleton ground placement;
  the second corrected it. These used hero combo_2 frame0, which is superseded by
  combo_3 frame1 (hand/hilt over waist). No full pilot visual pass is claimed.
  First-pilot hero sheet was opened but cropped the mirrored weapon; not acceptance.
- Current fixture PNG structural audit passes dimensions, exact selected-cel alpha,
  transparent unpainted cels and data channels; hashes in `prototype-inputs.json`.
  Existing source albedo/normal/importer files have not been edited. New GUID
  collision audit and `git diff --check` pass.
- Run `20261023T120901-serialize`, input
  `b96abc3c1272f4b01e9a4855cd26324cb5dd1bb902273bdd27ee1f2a6f14f2b2`,
  produced no capture directory after over5minutes. Unity was still using~103% CPU;
  log stopped after scene-manager initialization and contained a Unity TLS error.
  Cause is not established. Stopped exactly its container after verifying the
  private `/project` mount; exit143 is an interrupted failure, not a pass. Latest
  source is being retried without changing shared cache/other jobs.
- Orca-created default terminal was read, confirmed idle, closed. Worktree remains
  dirty intentionally because full feature/asset acceptance is incomplete; no commit,
  push, main integration or production activation has occurred.


Latest prototype receipt `20261023T121521-serialize`, input
`5815fa884768f373416632903daca28a49509b1441b753d0cee447f3db1b0a92`:

- Retry completed without changing shared data; prior interruption remains failed.
  Final prototype shader compiles/renders on llvmpipe LLVM15.0.7. Export48/48
  snapshots; directly inspected48/48 on4 sheets at native and nearest2x. Two
  source poses only: hero combo_3 F1 and sword Attack1 F2; four white-light
  directions, both facings, flat-normal/off, authored-normal/off and authored-
  normal/on. No animation sequence or transition acceptance (0 reviewed).
- Localized hand/chest/waist differences track the selected light/facing; face,
  blade and silhouette remain readable in these stills. No visible fringe in this
  selected scope. Subtle1-2pixel changes do not establish temporal stability,
  independent parry-fill behavior or adversarial neighboring-frame rejection.
- Latest scoped3/3 Unity tests PASS, zero failed/skipped, including the added
  second-library A -> B -> A property check and256 warmed binding updates with
  zero managed allocation. This is binding-level evidence, not full world-session
  A/B/A or native GPU performance evidence.
- `prototype-verified/receipt.json`, `visual-observations.csv`, four sheets,
 48 raw PNGs, exact input snapshot receipt, test XML and logs are retained in the
  task evidence root. Source/depth hashes are in `prototype-inputs.json`.
- After this receipt, only Editor inventory export and its CLI entry point were
  added; runtime, shader, maps and tests are unchanged. Inventory-only compilation
  is separate; it does not rerun or expand visual/PlayMode acceptance.

Remaining: complete production depth atlases and secondary bindings/library,
full frame/transition/loop review and original-speed/slowed playback, strict twice-
run builder preservation with complete depth data, one native1280x720 WebGL
lifecycle/input run, and separate same-build on/off p95 measurement (ABBA if>10%,
then only4 samples if confirmed). No production activation or main merge until
those gates pass. The retained dirty worktree and private runner remain task-owned
continuation resources; remove them after verified integration, retaining A/B evidence.


Final inventory-only compile/export `20261023T122418-serialize`, input
`28099165de958d4a4539c2e1bd6e2bde05b4161eb1ffba5ddb3e82b2027626f7`,
PASS:268 unique referenced sprites,279 serialized sprite keys across33 clips and
30 source atlases. The initial94-sprite scene-only inventory was incomplete:
`EndlessRun.Definition` loads the six `EndlessRole` resources dynamically. The
final collector includes exactly those role assets/dependencies, plus selected
scene/world references and direct hurt/parry clips, without scanning unused art
folders or hardcoding261. Original key times, held/repeated keys and clip loop
flags are retained in `inventory-final/depth-inventory.json`. This is an inventory,
not visual review of268 sprites. Only2 of268 have prototype depth data; no complete
production atlas/library is supplied yet. The final inventory compile adds no
runtime/shader/map change, so the previous3-test/48-static-image evidence is reused
only within its stated scope. Whitespace/GUID checks remain passing.

Next work remains authorized: establish the representative native performance
comparison, author all remaining referenced depth cels with pose-specific overlap,
then finish the full review/runtime/builder/performance gates above. Do not treat
this prototype checkpoint as completion or as permission to enable production.


## Self-shadow diagnosis and interactive two-pose demo (2026-10-23, in progress)

User rejected the visual distinction in the static viewer. This supersedes any
interpretation that the earlier two-pose images passed visual acceptance. The
agreed immediate scope is diagnosis, correction of the two existing poses, and
an interactive WebGL light demo; production/full-animation gates above remain.

Evidence: `E:/CodexTemp/TypingHero/sprite-self-shadow-20261023/diagnosis`.
`before.tar.gz` freezes the pre-diagnosis inputs. Source albedo, normal maps and
all existing Art importer bytes are unchanged; existing ShadowCaster/DepthOnly
pass text is unchanged after LF normalization. No GUID collisions found.

- Extracted the existing, byte-equivalent ray function to
  `SpriteDepthVisibility.hlsl` so diagnostic shaders exercise the real function.
  GPU run20261023T124740, input86a021217135b058ad6ca16e6d058cf48560364bb9b62f96c1edde417c0f1060:
  eight analytical cases pass (flat, four ledge directions, explicit uniform
  mirror, transparent blocker, hostile neighboring cel), plus3/3 scoped Unity
  binding/shadow tests. The uniform mirror case does NOT establish that an actual
  SpriteRenderer supplies that flip uniform; this gap is being investigated.
- Initial photometric run20261023T125714 failed because the diagnostic created
  transient textures before NewScene, then cleanup obscured their destroyed
  references. Fixed diagnostic resource ordering; not a runtime shader fix.
- Run20261023T130934, inpute7bdfb5597f0c3ed8b11e5d7ed652d1d47e753957b1cf3167668ae17296a3ebd:
  analytical cases and photometric checks pass. Flat-depth PBR and missing-map
  output errors are0. Occluded main-light receiver changes red0.222->0.078;
  independent fill blue0.432 and emission green0.040 remain unchanged.
  Scoped Unity3/3 pass. WebGL demo build succeeded; native inspection exposed
  an incorrectly elevated neutral floor (hiding the actor) and missing font
  glyphs. This candidate is not the accepted demo.
- Two numeric depth fixtures were re-authored: torso32, head165, foreground
  arm240, blade250, rear scarf80, other silhouette24 (0..255). Hero arm polygon
  now follows the upper gray forearm/grip instead of extending across the lower
  tunic. Material defaults remain0.65/0.15/0.01 and8 probes. Both selected alpha
  masks exactly match source; unpainted cels remain transparent. These are still
  ONLY two static pose fixtures, not production atlases.
- Viewed all48 new color captures on four native-2x sheets and all48 diagnostics
  on four sheets, plus the enlarged hero overhead comparison. Increased localized
  shade is visible under the arm/grip and on upper ribs; facial/weapon silhouettes
  remain. This is not yet a visual pass at gameplay conditions. Critically, the
  debug local-ray colors do not reverse X with SpriteRenderer.flipX. A direct
  SpriteRenderer photometric mirror regression was added and queued to distinguish
  a debug-only error from a production shader binding error. Do not ignore it.
- The isolated QA builder copies DungeonWorld and uses actual SampleScene actor
  transforms, existing arena/presentation cameras and profile; only two frozen
  sprites are installed. White-light/dungeon, facing, three normal/shadow modes,
  moving light, blue fill and diagnostic views are implemented. Demo code is
  guarded by UNITY_EDITOR or TYPINGHERO_DEPTH_DEMO; no gameplay bridge/API changed.
  No playable demo or native GPU acceptance is claimed until the build is opened
  and verified. Main integration remains blocked by the original feature gates.

Actual-SpriteRenderer regression run20261023T132711,
inputeeaa8b4f92c5bf65c6d27e303bf650ba57a776f93cdab88b61475f604de36361,
FAILED: mirrored ledge casts towards the light, maximum channel error0.1441458.
The earlier explicit-uniform mirror test missed this runtime binding defect.
The pending fix supplies `_DepthFacing` from SpriteRenderer.flipX/flipY through
the existing merged property block and uses it for mapped actors' rays and normals.
Unmapped actors retain the original normal path. Added real SpriteRenderer flip,
negative transform scale, combined scale/flip, and known tilted-normal regressions.
These additions require a new passing run; the earlier analytical pass is not
evidence that the fix works.

The next demo candidate places the neutral floor at the authored pose's lowest
opaque texel, freezes dungeon time for reproducible A/B, and uses ASCII labels
supported by the built-in font. Its source was reviewed after correcting a local
text-conversion error. The superseded queued snapshots were cancelled before
execution; `diagnosis/fixed-run-verified-source.log` tracks the fresh snapshot.

Verification continuation:

- Run20261023T134736 passed8 analytical GPU cases, then stopped progressing.
  The main Unity thread was waiting while its ALSA Mixer thread consumed99% CPU.
  Stopped only the verified task-owned container; this run is aborted, not passed.
- Retry20261023T135928 uses identical input
  `d5f0ca1e9e31dfe1c99cc939a399194909fa21f13685c33e6a3c0546c2c2fb08`.
  Eight analytical cases, real renderer flip/negative scale/double flip/tilted
  normal checks, and3/3 scoped Unity tests PASS. Mirror max error is now0;
  flat-depth and missing-map errors remain0; red receiver0.222->0.078 while
  blue fill0.432 and green emission0.040 remain unchanged. The retry included
  an unverified `-noaudio` flag; its effectiveness is not claimed. Future runner
  invocations use `-disableaudio`, found in the pinned Editor binary. No project
  audio configuration was changed.
- Downloaded48 color and48 diagnostic images into
  `diagnosis/fixed-20261023T135928`. Viewed every case on four color and four debug
  sheets (nearest3x crop containing the full actor). Ray X reverses correctly
  with facing; the head and silhouette remain readable and the mask stays local
  to overlaps. Color differences remain subtle in this soft offscreen capture;
  this is not native-gameplay visual acceptance. The WebGL build and native
  review are still pending at this checkpoint.
- Rechecked1304 existing Art files against the frozen input: zero byte changes.
  All1204 current asset GUIDs are unique. `git diff --check` passes.

Native two-pose review (candidate2):

- WebGL build20261023T135928 succeeded, including the existing strict repeat
  builder. Downloaded archive SHA256 matches the runner:
  `f89a3cf1a7ba882b0fc0d094dae80448a1b69a768f7b1ee1d336699a74078dd5`.
- Browser:1280x720 canvas/viewport, device scale1, WebGL2, ANGLE NVIDIA
  GeForce GTX1650Ti/D3D11. All resources are localhost; the demo server blocks
  external connections. Evidence is `diagnosis/native-final/`.
- Directly viewed all48 white-light captures (two poses, two SpriteRenderer
  facings, four directions, three modes) on nearest2x full-actor crops. Native
  images are crisp. Existing normals visibly shape scarf/skull/arms. Self-shadow
  adds a localized band beneath the hero's crossing gray forearm and darkens the
  sword skeleton's upper ribs beneath the foreground arm/weapon. It changes with
  light direction, retains faces/silhouettes, and does not darken the entire actor.
  Bottom/right cases can be subtle; the effect is not a large volumetric shadow.
- Also viewed both dungeon ON/OFF pairs and native depth/mask/ray views.
  Dungeon differences are subtler under the original warm lighting. Repeated
  paused white-light captures are pixel-identical; repeated frozen dungeon
  captures differ by one red-channel value of1/255 at one pixel per pose.
  Moving-light captures differ; fill ON adds light to the actor. These are render
  controls, not gameplay Pause/Resume or parry acceptance.
- Found two demo-only defects during browser QA: runtime CreatePrimitive logs
  missing MeshCollider after stripping, and hiding zoom leaves stale pixels
  outside the production combat viewport. The next build serializes only the
  built-in floor mesh and clears that unused right margin before drawing UI.
  Shader/binding/maps are unchanged and hash-match the passing render run.
  Build20261023T141723, input
  `c9a6d6bc16567d07814b9f5b12cb4744ef5abdf9c048f9e3243b597f2d459382`,
  is pending. It must pass native floor/zoom/console checks before demo handoff.

Technical and static-pose visual evidence above does not cover268 production
frames, transitions, combat lifecycle, or same-build native performance. Production
remains disabled; no main integration is claimed.

Final-demo follow-up: build20261023T141723 succeeded; archive SHA256
`ca1b810a0a3c904d830782a2e1fcea9297405744d439bcff999dec82e620eb71`.
The MeshCollider error is gone. Both poses' overhead-white-light OFF/ON world
captures match candidate2 pixel-for-pixel, confirming the serialized floor mesh
preserves the reviewed rendering. However, hiding zoom still leaves its image:
the initial clear used Unity's transparent-black texture. Corrected the margin
to draw the opaque white texture tinted Color.black and restore GUI.color.
See [Unity's blackTexture definition](https://docs.unity3d.com/ja/2022.3/ScriptReference/Texture2D-blackTexture.html).
Only this QA UI clear changed; next build input
`8064bfd3423e1786932b67b334fd81b48c8a9987dda1262c442cfeb4810ea608`
must verify show/change-actor/hide zoom and console before handoff. All132 normal,
surface/depth PNGs and metadata match source after the runner's LF normalization
for metadata; authored PNGs are byte-exact. The old static-viewer server was
stopped; its A/B evidence is retained. The interactive server remains on58936.

### Two-pose lab handoff (2026-10-23)

Build20261023T142920, input8064bfd3423e1786932b67b334fd81b48c8a9987dda1262c442cfeb4810ea608,
PASS. Downloaded archive matches runner SHA256
`27c3434d0f5e9c6596c3df1a5bf8076308d65fc632e4428dfd284be4264a3ad9`.
Live local lab: `http://127.0.0.1:58936/demo-review/demo/`.

- Rechecked native1280x720 floor and zoom show/change-actor/hide. Hidden zoom
  region has zero non-black pixels; no stale actor remains. Both poses' overhead
  OFF/ON world captures match the reviewed candidate2 exactly (zero changed pixels
  in all four comparisons). No MeshCollider error or script exception remains.
  Console still logs unsupported optional URP Edge Adaptive Spatial Upsampling
  and HDRDebugView shaders; these are not the actor shader or active lab passes.
  Do not describe this as an entirely warning-free URP console.
- Scope complete: diagnose binding/data/capture, correct two authored depth poses,
  fix real SpriteRenderer ray/normal facing, eight analytical GPU checks plus
  photometric/flip/scale tests and3 scoped Unity tests,48 offscreen color +48
  diagnostic +48 native white-light views, two native dungeon comparisons and
  interactive lab controls. See `diagnosis/native-final/receipt.json`, per-case
  observations, final-regressions, console and environment records.
- Runtime rendering acceptance is reused across the last two demo-only fixes;
  no normal/albedo image or material art default changed. After the final build,
  only trailing whitespace in three NEW .meta files and these documentation
  records changed; their GUID/importer values are unchanged.
- Full feature remains incomplete: only2/268 referenced sprites have depth,
  no animation transitions/loops/playback or full combat lifecycle/performance
  acceptance. Keep production disabled and do not merge main at this checkpoint.
- Retain task branch/worktree `micharel09/sprite-self-shadow` at
  `C:/Users/ADMIN/orca/workspaces/TypingHero/sprite-self-shadow` and private runner
  `/data/typinghero-game-loop/task-self-shadow-20261023` for the remaining feature
  gates. No task Unity job remains after the build. Remove those resources after
  verified full integration; preserve A/B evidence. Local demo serverPID2024 is
  retained for user review and can be stopped when that review is finished.

### Full gameplay depth expansion (2026-10-23, in progress)

- Merged origin/main e5bbc76 into the existing task branch, preserving the upstream
  hit-flash/Slayer composition and removed guard-break/body-fall VFX.
- Found and fixed a dead hookup: the active PixelUI pipeline calls
  DungeonSessionBuild, not WorldLightingBuild. The active builder now serializes
  exactly one SpriteDepthBinding on each actor. Run20261023T150555-serialize,
  input01b97e7e1ccdafd93f57f2f341e0d69f01d7f0b564a9e7c94cedc74cc24cf448,
  passed repeated builder, two serialized bindings, unassigned-world bind/clear,
  and13 SpriteDepthTests/SlayerTransitionTests. That run predates full-library bind
  assertions; it does not establish the later A-to-B-to-A result.
- Inventory now follows ParryContactLibrary replacements too:274 distinct cels
  on31 atlases, including six gameplay parry cels formerly missed by clip-only
  collection. No hardcoded expected frame count in the builder.
- Added draft anatomical masks and31 depth PNGs, plus ActorDepth.asset. Source
  cels were inspected to place head/arm/weapon/shield masks; final rendered frame,
  transition, loop and playback review remains pending. Hero's non-anatomical
  final soul fragments do not occlude light. The two approved prototype cels
  preserve every visible depth pixel (including their24-valued rear plane).
  Original31 source GUIDs, slicing, pivots/import settings and prior secondary
  bindings compare unchanged; only _DepthMap links were added. No albedo or
  normal PNG was edited. Draft recipe and asset hashes are under task evidence
  depth-authoring/draft-map-receipt.json (regenerate after mask edits).
- Full-library checks initially exposed incorrect empty-list secondary texture
  names; fixed. Full-library bind initially failed because scene opening unloaded
  the test's library asset, which deliberately is not a production dependency.
  Reacquiring it after both scenes open fixes the test without a runtime/shader
  change. Run20261023T154159-webgl input6d9099ac242a638df7a72bd7efd7da6d5453b2808a3c16b5170d13249a5d8e9b
  passes repeated builder, painted PNG byte preservation, full-library bind,
  clear, no-map bind and return. CaptureAll and the QA gameplay build are running;
  no full-game visual/performance acceptance yet. Production remains unassigned.
- Two-pose native lab RAF measurements, no capture: hero ON/OFF p95 both33.1ms,
  sword OFF31.6ms/ON31.7ms. Same existing lab build/viewport1280x720; raw samples
  are diagnosis/*-dungeon-*-perf.json. This is static-pose browser cadence, not
  final combat/GPU performance acceptance.
- Retain the task worktree, private runner, original two-pose lab and evidence
  roots listed above while full gameplay work continues. No main integration or
  complete-feature acceptance is claimed by this checkpoint.

### Full-map QA checkpoint, 2026-10-23

- Build20261023T155618-webgl, input
  `f15cd2ac15c99709af18110ac0276d6006fe1cdd60a9452d40f11998e4c6589a`,
  passed repeated builder, PNG preservation, selected-library bind/clear/no-map/
  return,6576 GPU captures and actual gameplay WebGL build. The previous154159
  build failed after compilation while restoring unloaded world editor objects;
  restore now reacquires assets by path. Downloaded player archive SHA256:
  `b5cc7a12ae979c108a517ea8d2f3c00af4646d683808dbe020ff5e1f5050f21a`.
- Inspected274 cels in both facings under top white light, native and nearest2x.
  Found a dark stripe on tutorial Attack1 F5/F6's non-anatomical smear; excluded
  that VFX from blocker coverage, rebuilt and re-inspected the full10-cel clip.
  All other captures compare byte-identical. No source albedo/normal PNG changed.
  Evidence: depth-authoring/frame-observations.csv (548 rows), review-wide,
  tutorial-revised and review-tutorial. Updated hashes: draft-map-receipt.json.
- Inspected all279 animation keys in33 clips, both facings, in actual clip order,
  including held/repeated keys and514 adjacent/loop pairs across both facings.
  Evidence: depth-authoring/clip-order and clip-observations.csv (558 rows).
  These are static adjacent-pose observations, not timed playback acceptance.
  Non-loop outgoing state joins are not inferred from isolated clip sheets.
- Serialize20261023T162033, input
  `9d52f4aca4abbbfd7532a2457fda867b67e5720ee964a66d2d96605f39c4f434`,
  passed full-library/editor checks and13 SpriteDepthTests/SlayerTransitionTests,
  including warmed binding zero-allocation and property-block preservation.
  Copied only the two tested added-component overrides into SampleScene;
  preserved existing serialized IDs and avoided unrelated builder churn.
- Native gameplay QA runs gameplay-native-03 and -04 passed14 checks each:
  real parry, typed follow-up, Slayer, Pause/Resume, restoration, managed errors,
  restart, menu unload and return. Recorded received hits too. Earlier01/02
  drivers failed to parry: they used an obsolete normalized animation contact
  formula. Switching the driver to actual enemyContactIn fixes input scheduling;
  no combat code was changed. User demo: http://127.0.0.1:8277 (F8 A/B).
- Native04 video exports788 decoded frames with exact timestamp agreement.
  Before visual review, selected0–3.2s (parry/slash),9.4–12s (Slayer entry and
  Pause/Resume),20.5–22.9s (exit, slash and hurt). Inspected every decoded frame
  in these intervals:220 frames,217 within-segment pairs,400x260 native crops.
  Observations: pulse lights contact/face and decays; flash survives depth binding;
  Slayer tint restores; hurt returns to lit idle. Selected crop/row evidence is
  gameplay-native-04/video-review/selected. The zoom crop excludes parts of the
  full silhouette. Native03 only frames0–44 were viewed before recapture.
- Capture limitation remains: even after warm-up, native04 has gaps0.337–0.809s,
  2.025–2.181s and21.807–21.963s. Recording was continuous with no screenshots;
  decoded-frame coverage does not prove coverage of every game-rendered frame.
  Do not claim full motion acceptance or infer images inside those gaps.
- Same-build native Intel UHD Graphics/D3D11, Edge155.0.4283.62,1280x720,
  no recording: gameplay-perf/browser.json contains480 OFF and481 ON combat
  samples. p95 OFF31.8451ms, ON31.7593ms (ratio0.9973). No >10% regression;
  ABBA/four-sample fallback were not triggered. This is player frame cadence,
  not isolated GPU timing. Existing interactive demo remained open in both runs.
- Timed1x/slow playback remains unverified. A review page at
  http://127.0.0.1:8277/depth-review/index.html preserves original clip key times,
  durations, non-loop endings and mirrored A/B images, plus the native video.
  Production is still unassigned; do not merge main or report full acceptance.
- origin/main advanced to b1a0df6 during QA with a changed parry follow-up and
  recovery clip. Integrate it and recheck inventory/affected parry motion before
  treating this candidate demo as current. Other static cel observations remain
  reusable only where source/depth/render configuration is unchanged.

### Updated-main candidate checkpoint, 2026-10-23

- Integrated b1a0df6 as 6f1754b. Its new queued parry follow-up references six
  additional cels. Authored those explicitly: combo2 F4/F5 and combo2_end F0-F3.
  Existing combo2 F0-F3 depth pixels remain byte-identical. Inventory now covers
  280 distinct cels,32 atlases,290 animation keys and34 clips. Production remains
  unassigned; these maps are still a QA candidate.
- Build20261023T163224-webgl, input
  `0b498d6dc72976333ddf92168acdd71e9061609c565b04b5afc087e9c55cfbc4`,
  passed repeated builder/preservation/binding checks,36 SpriteDepthTests and
  PlayerParryTests,6720 GPU captures and WebGL compilation. All6576 prior
  captures are byte-identical, recorded in reused-capture-comparison.json.
  Downloaded player archive SHA256:
  `486c3385b216e1a93751cc9fc714128850744d44b695fe938ed948f1731f52b1`.
  The earlier162840 build correctly rejected missing newly referenced maps.
- Inspected six added cels in both facings at native/nearest2x and all changed
  parry_success/parry_followup ordered sheets. Evidence under depth-authoring:
  current-map-receipt.json, current-frame-observations.csv (560 rows),
  current-clip-observations.csv (580 rows), review-current-new and
  current-clip-order. Combined static coverage is534 adjacent/loop pairs.
  Timed playback remains unverified; static sheets do not establish motion pass.
- Native gameplay-native-05 confirmed queued typing during successful parry and
  reaching male_hero-combo_2_4. The longer scenario failed to reach Slayer before
  death: measured input frames often took93-296ms during recording and parries
  were missed. Preserve this failed run/video; do not claim full current-build
  native acceptance. Earlier03/04 passes belong to the earlier gameplay source.
- Current review page includes34 clips and original key timing. Retain the
  independent worktree and private runner/evidence while motion acceptance and
  the current-build native scenario remain outstanding. No main integration.
- Current-build performance: gameplay-perf-current initially stopped when its
  input driver missed a short parry pose. gameplay-perf-current-02 used a100ms
  real key hold without requiring that transient pose to be observed. Both
  conditions contain living combat frames; no video or screenshots were taken.
  Initial OFF/ON p95 were54.8706/61.7752ms, triggering ABBA. Confirmation p95:
  OFF54.9164, ON55.5649, ON59.0973, OFF53.1464ms. Mean condition p95 is
  OFF54.0314/ON57.3311ms (+6.107%). This does not confirm a >10% regression,
  so the four-sample fallback was not triggered. Native environment/viewport
  match the earlier run, but absolute cadence is worse; these are browser frame
  timings, not isolated GPU measurements. Raw samples and abba-receipt.json are
  preserved. The driver completed11 checks; this is not full native motion QA.
- Current playable QA: http://127.0.0.1:8291, F8 toggles self-shadow.
  launch-gameplay-current.py serves gameplay-build-163224/player; keep it while the user
  tests. The older8277 demo remains unchanged. Current review page:
  http://127.0.0.1:8291/depth-review/index.html (34 clips; embedded combat video
  remains the earlier native04 capture, not proof of updated-main behavior).
  Review-page smoke check passed34 clips,320x180 images, non-loop ending,
  facing/speed controls and no JavaScript errors. The first launcher closed
  during startup; retry37072 reached ready and served the verified page.


### Production integration, 2026-10-24 (verification complete)

- User accepted demo8293 as visual acceptance, with rear rune/light and both blue
  haze layers disabled. Explicit scope: finish saved production configuration,
  scoped integration tests and one1280x720 real-input run without video; no full
  animation review. Earlier complete-playback gate is superseded by this choice,
  not reported as having passed. Existing evidence and failed runs are retained.
- Merged origin/main8a7b498 into the existing sprite-self-shadow worktree, retaining
  upstream trimmed parry follow-up cel and normal-attack-matched duration.
- DungeonWorld now selects ActorDepth; DungeonSprite saves strength0.65,
  depth scale0.15 and bias0.01. DepthPreview only adds QA controls to those saved
  assets. F8 captures per-material values and restores them on ON/disable/destroy.
- Named shader limits remain8samples/light and12source texels; probe sequence,
  light configuration and authored depth/normal images are unchanged. Reuse the
  existing performance evidence; this constant extraction is not a shader fix or
  lighting change. No new performance claim.
- Pending: current reference inventory, repeated builder/map/reference preservation,
  focused binding/F8/parry/Slayer tests, native integration, production QA exclusion,
  commit/push/remote verification and retained-demo handoff.

- Offline run20261023T180003, input9d1d29426c2fe2f41f603354c1563c233a60c5528e9b28c7dbc9a5c94b4d172d:
  49/49 SpriteDepthTests/PlayerParryTests/SlayerTransitionTests PASS,0failed/skipped;
  actual inventory280sprites, repeated builder, PNG/config/reference preservation,
  serialized world bind/clear/no-depth/return PASS. Reused for unchanged depth,
  parry and Slayer source below. WebGL compilation from this snapshot was stopped
  deliberately (exit143) after newer main arrived; it is not a passing build.
- Integrated mainb0534d5 (pause chrome/menu/encounter guidance cleanup). Two depth
  component overrides remain through the scene merge. Recheck current builder and
  actual menu/Pause/Resume/encounter flow on a new QA build; no full-suite expansion.

- Build queue: the first latest-main attempt expired waiting600s for the shared
  Unity lock; it executed no Unity stage and is not a failed gameplay result.
  QA and production were resubmitted with3600s lock waits, preserving other jobs.

- Final-main QA20261023T182529, input
  `d0403f7cf73aa00e821d13dd00d2a67955d1458ec565ac72611614739fd1ed8e`:
  repeated builder,280sprite metadata/map validation, painted-map and saved
  config/reference preservation, selected world bind/clear/no-map/return, and
  WebGL build PASS. Inventory289keys/34clips/32atlases; no fixed-count gate.
- Native integration-native-02: Edge/GTX1650Ti/ANGLE D3D11,1280x720,18/18PASS.
  Real keyboard/mouse parry -> queued combo2 follow-up, received hit, Pause/Resume,
  Slayer entry/restore, F8 OFF/ON, menu and world return passed; enemyFlash and
  expected follow-up/hurt cels observed in the read-only trace. No video recording,
  no full animation review,0exceptions/provider writes; fixture interception and
  external blocking remain enabled. Initial native launch01 did not start because
  port8295 was occupied; preserved that process and selected a free port for02.
- Downloaded33player files match the immutable server player. All204 exported
  lighting files match source (text line endings normalized); RuneLight,RearRune,
  RearHaze,LowHaze are inactive in serialized build output. Paused OFF/restored
  stills inspected only as integration evidence, not a new animation/style gate.
  Evidence: `E:/CodexTemp/TypingHero/sprite-self-shadow-20261023/`:
  integration-offline, integration-qa-build, integration-native-02 and their
  receipts. Production build and remote integration remain pending.

- Production20261023T183215 uses the same d0403f7 input; normal WebGL build PASS.
  Scanned48stripped managed assemblies: GameLoopProbe,SpriteDepthPreviewControl
  and SpriteDepthDemo absent; JS QA bridge absent. All33downloaded player files
  match the remote. Final local build-input digest also matches both builds.
- Normal-player native smoke: actual menu/name/tutorial skip/gameplay/pause opened
  with keyboard/mouse, no QA bridge or JS/managed exceptions,0provider writes.
  Gameplay and paused images directly inspected: actors render in the saved
  dungeon with rune/haze off. Pressing F8 leaves the paused canvas byte-identical.
  Production helper's first launch had a local tutorial-hash path error before
  browser startup; corrected only the external helper, without changing the player.
- Acceptance is complete for the user's scoped integration and approved demo;
  no new full-animation or performance claim. Final remote commit/ancestry and
  cleanup are recorded in `E:/CodexTemp/TypingHero/sprite-self-shadow-20261023/integration-receipt.json`.
  Retain evidence and the standalone integration-qa-build/player demo outside the
  worktree. Private runner results remain for reproducibility; remove its cache
  and source only when no longer needed, preserving evidence/player artifacts.


### Cinematic wall torches, 2026-10-24 (verification complete)

- User reported fixed-looking triangular pools below both wall flames and chose
  cinematic, clearly visible flicker. Keep central lantern, flame atlas/timing,
  actor depth/normal data, rune and blue haze settings unchanged.
- Branch/worktree: micharel09/cinematic-wall-torches, from main3cd7293.
  Retain current Point Lights (base8/range15/no shadows); author SinePair amplitude
  .18/frequencies5.8,13.7/weights.7,.3/phase factors1,2.37; retain instance phases
  .7,2.9. Disable two TorchBeam objects and remove their atmosphere update refs.
- Use disposable QA render-only controls for same-player baseline/candidate and
  independent beam/wall-light/key-shadow/fog/blur diagnosis. QA controls never
  mutate combat and are absent from production. No recording or full sprite review.
- Pending: source-of-dark-region diagnosis, scoped lighting lifecycle/builder checks,
  native10s visual samples and real-input lifecycle,15s/condition performance
  (ABBA confirmation only above10% p95), production and remote integration.
- Evidence/artifacts: E:/CodexTemp/TypingHero/cinematic-wall-torches-20261024.

- Initial snapshotf31be374 passed repeated builder/authored-byte preservation and
 16/16 LightingProfileTests/SlayerTransitionTests. Native A/B diagnosis and relative
 performance use that immutable player. Main then advanced to c36e591 with reviewed
 Space cue/HUD; rendering scripts, lighting profiles/assets and cameras are unchanged.
 Integrated that main and queued final QA builder/player for its actual input flow;
 reuse the16passing unchanged lighting/lifecycle tests.

- Diagnosis on immutable QA20261024T014122/inputf31be374:26checks PASS.
  Directly inspected candidate and all six same-phase diagnostic images.
  Baseline-no-beam removes the long diagonal/vertical pools; disabling the key
  shadow leaves them visible, and disabling wall lights still leaves the beam
  cards. Fog/blur changes do not remove their geometry. No room geometry, shadow
  proxy or ambient adjustment is needed; keep legitimate dark wall/floor regions.
- Inspected11paired wall crops at0.12..10.13s, both wall torches, plus full-scene
  diagnostic stills. Intensity ranges6.663..9.153 and6.685..9.285; actual emission
  equals each light multiplier, both phases differ, wall texture remains visible
  without the long beam cards. This is sampled lighting observation, not full
  animation review or video capture.
- Same-player native Edge/Intel UHD/ANGLE D3D11,1280x720:15s/condition live combat,
  no captures/recording during measurement. Baseline471samples/p95=33.3938599ms;
  candidate460samples/p95=33.3900452ms, ratio0.999886.11checks PASS,0exceptions/
  provider writes. ABBA not triggered. Browser cadence, not GPU/device-FPS claim.
  Relative torch comparison uses the immutable pre-cue-HUD player; final-main
  rendering/lighting sources are identical and final input/lifecycle is checked
  separately on the current player.

- Final-main QA20261024T015428/input8380b12e (base4b57eb6) passed the repeated
  builder, cinematic profile/disabled beam checks and authored lighting preservation.
 33downloaded QA files match remote; all232exported lighting files match current
 source after newline normalization. Both beam nodes plus RuneLight/RearRune/
 RearHaze/LowHaze are inactive in actual serialized output.
- Native lifecycle-final-02:16/16PASS on current cue/HUD player; real parry/queued
 slash, Slayer endpoint, Pause/Resume, no phase reset or pause catch-up, received
 hit and menu/world return. Actual pause holds age/frame/emission/intensity.
 Slayer composite endpoint and returned gameplay stills directly inspected;
 0exceptions/provider writes, no recording. First lifecycle-final fixture failed
 after5checks because it incorrectly expected inactive light GameObjects during
 Slayer. Source uses a full background composite and keeps world clocks active;
 corrected only the external assertion and clarified the authoring guide, without
 changing production behavior or weakening the no-reset/Pause acceptance.

- Final normal player from the same015428/8380b12e input built successfully;
  48stripped assemblies exclude GameLoopProbe,SpriteDepthPreviewControl,
  SpriteDepthDemo,TorchQaControl, and JS QA bridge is absent.33downloaded production
  files match remote. Native production smoke PASS: actual menu/name/tutorial skip,
  gameplay and Pause opened; gameplay image directly inspected, no long beam cards,
  F9 leaves paused canvas byte-identical,0exceptions/provider writes.
- Final local source plus the declared disposable QA fixture hashes match both final
  build inputs. Production repository changes are only RearTorch.asset and four
  serialized scene edits (two active flags/two update refs), plus documentation.
- Remote commit/ancestry, independent demo endpoint and clean-worktree cleanup are
  recorded in E:/CodexTemp/TypingHero/cinematic-wall-torches-20261024/receipt.json.
  Retain original reported images, immutable players, observations and failed fixture
  result. Reused private runner cache predates this task; preserve it and its results
  for reproducibility. Standalone demo/evidence stay outside the task worktree.


### Pillar torches, 2026-10-24 (implemented; QA waived by user)

- User selected two total torches: one at each outer front stone pillar, facing
  the arena. Retain cinematic yellow flame style and existing rear/main lamps.
- Independent branch micharel09/pillar-torches from ae1271d. Reuse authored
  holder mesh, flame atlas/material, and DungeonTorchLight; new PillarTorch profile
  starts intensity8/range10/no shadows, projected flame12 internal pixels,
  SinePair amplitude.18, phases1.6/4.2, flame offsets.17/.43 seconds.
- Fixtures are parented to FrontPillar_-1/1 with inverse parent-scale compensation.
  Room positions are x=+-8.55,y=2.7,z=-1.2, on the inner faces; holder yaw is
  -90/+90degrees, flame cards counter-rotate toward the camera. Lamps are at
  x=+-8.3,y=3.02666,z=-1.45. Flame scale1.26313648 follows the authoring projection
  formula at the saved35degree camera/180logical height; appearance is unverified.
- Keep ambient/exposure, old lamps, shaders, combat, rune/haze/beam disabled state,
  GUIDs and sprite animation unchanged. Repeated builder must retain both fixtures.
- Original acceptance (waived by user): repeated builder/profile preservation, native1280x720 A/B
  and10s temporal review, actual-input combat/Pause/Slayer/world return, conservative
 8light/object budget,15s on/off p95 (ABBA if >10%), production QA exclusion.
- External artifact root: E:/CodexTemp/TypingHero/pillar-torches-20261024.
  Disposable render-only QA fixture toggles only the new holders/lamps using F9;
  read-only F7 reports phase/emission/lights. No recording/full animation review.
  Reuse previous runner cache only under its global lock; preserve unrelated jobs.

- User explicitly requested "skip QA" after the shared runner queue remained busy.
  Canceled only this task's queued runner before it acquired the lock; no Unity
  checks/builds/browser/performance ran for this revision. Earlier snapshots were
  replaced in the queue to reduce upload and include the affected test assertion;
  all other jobs were preserved.
- Updated the existing main-world lifecycle expectation from3to5lamps; the distinct
  four-lamp fixture is unchanged. Test execution, repeated-builder preservation,
  visual placement/brightness, Pause/Slayer/world return, performance and production
  QA exclusion remain UNVERIFIED for this change. No passing result is claimed.
- Static diff/references/projection reviewed; preserve the old demo because it is
  the previous immutable build. New scene/profile data ships on the next build;
  current live demo does not contain the pillar torches. Remote/cleanup receipt
  stays in the external artifact root.


### Pillar visibility correction, 2026-10-24 (production image verified)

- User's actual gameplay screenshot showed no new torches in production025704,
  despite the two fixtures existing in serialized build data. Previous completion
  claim was too broad: data presence is not visibility acceptance.
- Root cause from source: fixtures were attached to hidden legacy KayKit
  FrontPillar_-1/1, while the visible room uses PixelRoom/FrontPier-1/FrontPier1
  at different positions. Old lights were placed outside/behind visible piers.
- Reparent holder/light pairs to actual pixel-pier transforms (unit scales,
  yaw-18/+18). Holder local x points .25units inward,y=-.25,z=-.82, outside the
  front face (.7half-depth). Lights y=.08,z=-1.08. Counter-rotate flame cards by
  pier yaw, preserve style/phase/offset and12pixel projected height.
- User waived QA suites; run only the production build and one directly inspected
  gameplay image with real menu input. No full tests/performance/recording.
- Retain rejected build/demo8332, user screenshot reported-pillar-invisible.png,
  new visibility-build/visibility-demo artifacts under existing external root.
  Do not integrate or report visible torches until the actual gameplay image passes.

- Production030724/input8cfaf3ac: build PASS, QA excluded; all2384source files
  matched the snapshot manifest and all33downloaded player files match remote.
  Actual serialized scene binds both holder/light pairs to the visible FrontPier
  transforms. Copied only Unity-rounded flame scales back to local scene data.
- Native Edge1280x720 demo8333 entered gameplay using real menu/name/tutorial
  inputs. Directly inspected visibility-demo/pillar-gameplay.png: both flames
  visible on the left/right pier front faces, warm light reveals stone tiles and
  nearby floor, center actors/UI remain readable. Held in Pause for user review.
  This is one still-image visibility check, not animation/lifecycle/performance QA.
- Keep the rejected025704build/demo8332 and the user's image as failure evidence.
  Integration commit, endpoint/PID and cleanup are in the external receipt.


### Clean letterbox, 2026-10-24 (production verification passed)

- User showed duplicated torch/pier strips at wide browser edges and selected
  black bars outside the existing16:9frame, not a wider scene/camera composition.
- Branch micharel09/clean-letterbox from9d41188. Pinned URP14FinalBlitPass uses
  Load on non-default viewports, preserving uncovered backbuffer pixels. Source
  presentation camera clears only its own rect; no full-output bar clear exists.
- Keep world RT/camera/profiles/point presentation unchanged. Add a backbuffer-only
  letterbox pass at AfterRendering+2, after FinalBlit(+1) and before overlay UI
  (+2,enqueued after features). One existing material's new black pass discards
  interior pixels, paints only outside PresentationRect, handlesUVorigin and
  restores viewport. Skip when rect is full. RTHandle wraps built-in camera target;
  no new camera/texture, public API, quality setting or lighting/torch changes.
- Reuse prior production030724for baseline. First menu navigation using cached
  coordinates missed Start; not a gameplay check. Corrected1280entry worked;
  paused1280->1920x900->960x900->1280resize did not reproduce edge residue. Check
  wide menu-to-game entry as the relevant viewport-shrink transition as well.
- Baseline wide menu-to-game entry reproduces the reported duplicate strips after
  waiting for menu readiness and using real Enter inputs. Reviewed image:
  baseline-entry/wide-entry-reviewed.png. Earlier startup inputs were too early;
  preparation-dialog captures are not gameplay evidence.
- First build033055 failed because URP14CameraData.pixelRect is internal; use
  public cameraData.camera.pixelRect. Production034449/inputd7f29cd2 passes,
  all2386snapshot inputs verified,33downloaded player hashes match remote and
  QA bridge/depth-preview types are excluded. Both changed rendering sources
  exactly match the build snapshot. Generated DungeonWorld/PillarTorch/meta
  exactly match the prior accepted030724production; builder serialization differs
  from raw authoring scene IDs, so raw scene byte equality is not claimed.
- Native Edge/D3D11 production: real Enter/Escape/M input,1280x720 gameplay,
  Pause,1920x900/960x900/1280x720resize, Resume, wide menu return/re-entry and
  Pause pass. Directly inspected gameplay, wide/tall/back/resumed/menu/re-entry
  images: bars contain no duplicate scene, actors/torches/HUD/typing remain visible.
  Four image pairs of bar interiors have0nonblack pixels, max channel0
  (135584pixels/side wide;163744top/bottom tall). Exclude the outer4px screenshot
  rim and4px gameplay boundary margin; raw first measurements included a dark
  outer screenshot rim, not leaked scene imagery.0captured runtime exceptions.
  Full QA/performance/video remain waived; this is scoped rendering acceptance.
- Artifact root E:/CodexTemp/TypingHero/clean-letterbox-20261024; keep original user
  screenshot, baseline attempts, immutable build/receipts and final demo separately.
- Integration pending: merged newer main9c48f5d into task e8aee8a. Rendering sources
  remain identical, but integrated production035339/inputedfe35fe is blocked by
  main65852c2's new male_hero-parry_lift sprite lacking authored depth. Validation
  was not bypassed. Ask whether to extend scope to depth authoring or retain the
  verified pre-parry demo. Demo8341 uses034449, not the failed integrated build.
  Keep this worktree/branch until that decision and verified remote integration.

### Automatic depth feature, 2026-10-24 (production verification passed)

- User approved automatic recognition of authored maps, not draft image generation.
  Missing depth is an allowed per-frame fallback; mapped frames retain self-shadow.
  Keep the clean-letterbox implementation and merge current main on this worktree.
- SpriteDepthAuthoring resolves existing source bindings, preserves legacy custom
  maps and discovers conventional GUID-named maps only when unbound. Conflicts,
  broken bindings, invalid maps and build atlas packing still fail; absent maps do
  not. Resolve/validate a library before writing bindings/index; preserve all PNGs.
- Synchronize through the same menu/Enter Play Mode/builder path. Builder sync is
  after clip/world authoring, validation callback -500 follows PixelUIBuild -1000.
  Runtime schema/shader/combat stay unchanged. Stable GUID/local-ID index ordering,
  pruning only unreachable metadata and write-on-change prevent repeat drift.
- Actor inventory follows actor prefab/opponent dependencies and contact libraries,
  without actor folder-name filters or fixed frame counts. Explicit coverage audit
  remains strict; CaptureAll still requires complete coverage rather than claiming
  a partial inventory as full visual acceptance.
- Save only the derived library with SaveAssetIfDirty; automatic Play Mode sync
  must not save unrelated dirty author assets. The Editor test covers this explicitly.
- Production050935/inputb2ddfed:5/5Editor (4.7034938s) and2/2runtime
  (0.0654234s) PASS,0failed/skipped. Actual serializer preserves painted PNGs,
  selected world/material configuration and stable index on repeat. Production
  succeeds with280mapped/1missing (parry_lift), proving missing art no longer
  blocks a normal player; QA bridge/depth preview types are excluded.
- Main advanced to168ae6b: retain its authored lift depth d6eb563 and terminal HUD
  suppression. Mergecd7db69 keeps both changes and final depth sync after builders.
  Synchronizer/validator/binding/focused test sources are unchanged; reuse the7
  passing tests and main's documented HUD checks, rerun actual serialization.
- Integrated production052952/inputaf65179 passes serialization/build/QA exclusion,
 281mapped/0missing. All33downloaded player files match remote. All99lighting
  PNG hashes match current source, including main's added map; no PNG generated or
  rewritten. All281sprite/depth references and float32 metadata values match input.
  Copy back only the verified generated index: one-time stable row order/Unity
  numeric serialization, with asset GUID, bindings and semantic values unchanged.
- Native Edge/D3D11,1280x720: real Enter/Space/Escape, one successful parry
  ([PARRY] OK, score30), recovery and Pause/Resume PASS;0captured exceptions.
  Gameplay/parry/recovery/pause/resume images inspected; bodies, warm pier torches,
  HUD and typing remain visible. No bridge/gameplay setters or video used. Reuse
  clean-letterbox wide/tall evidence because render code/camera contracts unchanged.
- Earlier compiler API-name, helper hashlib and fixture-import-type failures are
  retained in the external receipt. Correct the fixture to linear Default rather
  than weakening map validation; packing rejection now asserts its actual reason.
  Two stale queued jobs were withdrawn only after PID/helper-hash/lock-wait checks.
- Full QA, sprite-animation review and performance profiling remain outside scope.
  Current evidence/demo stay under the artifact root above; remote integration and
  clean worktree removal are recorded in receipt.json after push verification.


## Scene depth discovery and map replacement fix (2026-10-24)

- Approved follow-up: collect scene-specific actor references and treat a valid
  source `_DepthMap` binding as authoritative over stale derived library metadata.
- Preserve prefab/Endless discovery; read enabled build/world scenes, loaded
  instances in memory and unopened scenes through temporary additive scenes.
  Include loose actors, child components and explicitly referenced external
  SpriteRenderer/Animator components. Do not save scenes or unrelated dirty assets.
- Keep broken/duplicate bindings, invalid map imports/dimensions/packing and
  ambiguous unbound legacy maps as errors. Preserve authored PNGs/GUIDs, stable
  index ordering and write-on-change. No runtime/shader/combat/schema changes.
- Verification: result20261024T071611-quick, inputa3609b88,17/17Editor and
  2/2runtime PASS,0failed/skipped. Real QA-mode serializer/repeat builder PASS,
  two actor bindings and selected-world bind/clear/no-depth/restore PASS.
  Full QA, WebGL, animation review and profiling are outside this change.
- Task worktree: depth-scene-sync; external receipt/artifacts:
  E:/CodexTemp/TypingHero/depth-scene-sync-20261024.
- All10checked source/config/index files match the tested/generated snapshot.
  All99lighting PNG hashes match; ActorDepth, DungeonWorld and material bytes
  unchanged. Inventory now also sees the ContactShadow child sprite without a
  depth map. This was incorrectly classified as expected fallback: production
  remained usable, but strict audit/CaptureAll rejected it. Receiver-scope
  correction below addresses this; all authored rows must remain unchanged.
- Six map/inventory checks passed before scene fixtures could run; fix NUnit
  assertion compatibility and Unity Untitled scene restrictions in fixtures. Use
  only native scene-settings documents from a saved scene, then open additively.
  NewPreviewScene is not needed; opening a saved preview scene is not public in
  pinned Unity2022.3. Preserve loaded/unloaded entries and active/dirty state.
  Earlier compile/fixture failures and one safely withdrawn queued snapshot remain
  in the external receipt; they are not counted as passes.
- Main advanced only by startup-profile documentation213e67d; build inputs,
  dependencies and environment are unchanged, so the19tests/serializer remain valid.

## Depth receiver scope correction (2026-10-24)

- Main SpriteRenderer on PlayerHealth/SkeletonController is automatic; secondary
  and external receivers opt in via existing SpriteDepthBinding. No name, shader,
  path, enabled-state or material-strength classification.
- Direct/serialized/persistent renderer references share classification. Clip
  curves resolve SpriteRenderer.m_Sprite against the live Animator hierarchy
  before temporary scenes close; export retains asset/path records only.
  Controllers/overrides, direct hurt/recovery, Endless controller/idle/hurt and
  contact/defense cels retain coverage. One shared inventory serves sync/export/
  validation/audit. Production fallback and strict missing-map rejection remain.
- Unity2022.3.25f1/fly-server result20261024T074545-quick,
  input3d80507630403c0db3c5bcf5d2b1e2b9ed6861614dd85ab89cf4d231607d30df:
  31/31Editor tests PASS,0failed/skipped. Public strict audit and the exact
  CaptureAll strict validator PASS; missing actor maps still fail strict audit
  while production validation allows fallback. Bound/disabled/strength-zero and
  external/persistent receivers, mixed curves, shared sprites, overrides/hurt/
  contact/Endless and loaded/unloaded/dirty scene-state preservation all pass.
- Production inventory equals the saved authored index:281mapped/0missing;
  only the unbound ContactShadow sprite is removed from the previous inventory.
  Real QA-mode two-pass builder and selected-world bind/clear/no-depth/restore
  PASS. All99lighting PNGs and12checked source/config/index files match after
  serialization; library/world/material bytes unchanged. Reuse prior2/2runtime
  tests because runtime/test/config/index sources are unchanged. No WebGL build,
  full-frame rendering, full QA or animation-review claim.
- Remote integration/cleanup receipt is retained in the evidence root below.
- Worktree depth-receiver-scope; evidence root
  E:/CodexTemp/TypingHero/depth-receiver-scope-20261024.


## Renderer discovery and capture preflight (2026-10-24)

- Referenced prefab receivers include the nearest self/ancestor Animator even
  when the reference is unsaved and absent from disk dependencies. Excluded
  renderers still stop before following the Animator; receiver curve filtering
  and map selection/migration/pruning are unchanged.
- CaptureAll strict-validates the current scene state before checking every
  loaded scene for dirty changes. Capture/Investigate share that dirty-scene
  guard, including scenes outside Build Settings. No auto-save, library sync,
  GPU check, output or destructive scene replacement precedes preflight success.
- Unity2022.3.25f1/fly-server Editor20261024T101721-quick:38/38PASS,0failed/skipped,
  56.8565823s. Seven new cases cover unsaved prefab animation on self/inactive
  ancestor, no-binding exclusion, disabled/zero-strength authoring, strict missing
  map before dirty/GPU guards, actual CaptureAll/Capture/Investigate entry points,
  scene/override/output preservation and clean complete preflight without GPU.
  Existing31sync/discovery cases include production inventory equality against
  the saved index without a fixed frame count.
- Builder20261024T102114-quick/input
  942e01c8184ae0cb02c39a035d347a5b58d99ae094c95b3de914e07b2b2a93cb:
  QA-mode repeated builder and selected-world bind/clear/no-depth/restore PASS.
  All209protected files (including99lighting PNG and library/world/material)
  remain byte-identical. First builder failed existing font protection on CRLF
  archive input; canonical LF snapshot passes without changing that protection.
  The38passing Editor cases are reused across this normalization-only retry.
- Prior2/2runtime depth cases from071611 reused after Git comparison confirms
  runtime/test/shader/library/material/package/project inputs remain unchanged.
  No frame rendering, WebGL build, full QA or new animation/performance claim.
  XML/logs, source/artifact/protected SHA256 and runtime reuse evidence:
  E:/CodexTemp/TypingHero/depth-preflight-fix-20261024/receipt.json.
