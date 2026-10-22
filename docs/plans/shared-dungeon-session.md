# Shared dungeon session

## Scope and acceptance

Source of intent: approved menu/gameplay shared 3D dungeon plan. Preserve room,
materials, lighting, native 320x180 presentation, camera angle, menu layout/logo,
modals and Play/Tutorial preparation. No camera transition, new backdrop, provider,
PlayerPrefs schema, clipboard or gameplay-rule changes. No itch.io deployment.

Acceptance: one retained DungeonWorld/environment/camera/presenter/RT; gameplay
created only after Start and bound before Awake; reloads for Restart, tutorial
lesson/Skip and Endless Continue; pause/Slayer return restores world and locks.
Three cycles must retain IDs and bounded resource counts. Focused Unity lifecycle,
binding/reset and identity tests; repeated builder; QA WebGL real input at1280x720,
background/text/transitions at640x360, fixture interception and external blocking.
Bridge observation only. No full gameplay suite or stress/soak.

## Implementation checkpoint - completed and verified

- Verified origin https://github.com/micharel09/TypingHero.git; fetched main.
- Base1e921327dc46847a9701dcd76611490d725bff5d.
- Orca worktree C:/Users/ADMIN/orca/workspaces/TypingHero/shared-dungeon,
  branch micharel09/shared-dungeon. Parent dirty checkout and compact-tutorial
  are preserved; none of their WIP is copied into this task.
- Added serialized additive-world ownership, inactive gameplay binding gate,
  session transition serialization and internal additive routes. Builder restores
  authoring ownership before existing passes, splits final output afterwards;
  combat lighting remains gameplay-owned, world camera outside Slayer environment.
- Added lifecycle/binding/three-cycle tests and observation counters. Final acceptance is recorded below.
- Task adapter E:/CodexTemp/TypingHero/shared-dungeon/run.py selects existing Unity
  testFilter DungeonSessionTests;DungeonRenderTests;PlayerIdentityTests; uses shared
  runner lock/toolchain/cache and immutable snapshots. Build-only output followed
  by scoped native browser checks; no repository runner expansion.
- Shared runner was initially occupied by another authorized task. Own serializations
  waited for its existing lock; other jobs/resources remain untouched.

## Retained resources

Keep task worktree/branch, adapter and runner receipts until implementation,
verification and remote integration are complete. Remove only clean task-created
worktree/local branch after verified integration; keep final preview/player for
review with its exact path/process and cleanup condition in final receipt.

## Builder/runner diagnostics

- Shared serialize20261022T025542, b85ff8fc: failed the existing font byte guard
  because Windows CRLF normalized under Unity; no guard was weakened. Task adapter
  now canonicalizes serialized text to LF, as the prior project adapter does.
- Serialize20261022T025806,03d9c439: repeat MainMenu failed; inactive legacy backdrop
  was not found by GameObject.Find and was duplicated. WorldLightingBuild now
  reuses inactive backdrop/rig objects. Combat-light authoring similarly reuses its
  inactive root and existing component rather than cloning it every pass.
- Superseded waiting own serializations were canceled after exact process/helper
  checks; running other jobs were preserved.
- Snapshot6849dcc1 was rejected before Unity: shared data disk2.95GiB vs3GiB gate.
  Task scratch moved to /tmp/typinghero-shared-dungeon (existing7.6GiB tmpfs), with
  symlinks to shared Library/cache/license/toolchain/image records/runner.lock.
  All capacity gates remain unchanged. Repository runner is unchanged.
- Current adapter selects the three directly affected scene-backed tests in
  addition to lifecycle/render/identity tests. Their scene loads now use the
  asynchronous session owner. QA bridge uses Current (no creation side effects).
- Preserve distinct actor Transform.root identities: gate activates all roots
  atomically, then detaches them in earliest Awake before gameplay controllers.
  Persistent audio roots are not disabled during gameplay teardown.
- Slayer still disables both room and combat lighting. Combat lights are now
  gameplay-owned and are destroyed on reload; world camera/presenter stay active.

## Verified offline ? 2026-10-22

- Serialize034609, inputdf81fda091bc93538458392a7a7ca58453c0ffb9283c9002a0ed77dca29bc14a:
  strict repeat PASS, including DungeonWorld scene/meta and EditorBuildSettings.
- [Scene audit](shared-dungeon-scene-audit.json):12checks PASS;96original menu
  RectTransforms byte-identical, existing scene GUID/meta unchanged, all original
  gameplay GameObjects retained, no dangling local refs in all three scenes.
  Only relocated combat-rig component changes its local ID. Final IDs:menu477,
  gameplay925, world997. Exactly one3D room/world camera; retired gameplay
  background camera is disabled, no menu camera.
- Red Quick035012/input8dea9d2c:21/22; actual Continue exposed a destroyed enemy
  collider still read through the persistent camera's old framing layout.
- Green Quick035342/input7635fb6a11e200ffb5c43959e0312daf2116a2d5d56c0ce5afd60ea1ec23cb88:
  22/22PASS,0failed/skipped. Camera stops querying dead actor framing; framing
  origin now initializes after collider activation. Tests cover same world,
  camera/presenter/RT over3cycles, bindings/nativeCanvas, separate actor roots,
  duplicate transition requests, pause/reading/Slayer reset, absence of combat in
  menu, existing identity15, rendering3, actual HUD/damage and Continue data.
- All receipts currently under /tmp/typinghero-shared-dungeon/results on fly-server.
  Local immutable inputs and drivers: E:/CodexTemp/TypingHero/shared-dungeon.
- QA WebGL build queued on exactly green runtime input; native/visual acceptance
  and integration remain pending. No full suite/stress/soak or production upload.

## Final acceptance ? 2026-10-22

- Retained native failures: legacy FX-backed visibility incorrectly returned false
  in menu; driver confused same-menu drafts with fresh-menu canonical names and
  Continue's reused PracticeButton. Corrected driver reached real Continue/Slayer,
  exposing4Unity errors from starting tweens on inactive ScreenFX_Root during return.
- ExitSlayerFX now uses instant teardown when inactive. The lifecycle test activates
  real PlayerSlayerMode before all3returns. Observer reads environment from world
  owner and combat light from the gameplay rig; no creation/mutation through bridge.
- Final Unity Quick041809:22/22PASS,0failed/skipped. QA WebGL042021 plus strict
  repeated builder PASS on the same immutable input:
  `1dc641a5e34fb745b3a71ef8d5ef913c4dbcd17fc6cf49073ac05e531283cc8d`.
  Final MainMenu/SampleScene/DungeonWorld/World.meta/EditorBuildSettings equal
  checkout after Git newline normalization; static scene audit remains valid.
- `native-final-1`:129/129PASS in68.897s, Edge155.0.4283.62,
  GTX1650Ti/ANGLE D3D11, DPR1,1280x720/640x360. Real Play/Tutorial preparation/Start,
  lesson transition, Skip, Restart, win, Continue without another Fight, real parries
  into Slayer, pause/M return, official-name reload and Enter consumption pass.
  Settings/Top1000/help and all relevant1280state images directly inspected.
- `native-640-final`:7/7PASS, same player, fresh Play/Unseen, Skip/Fight and pause/menu.
  Unobstructed640combat, HUD, characters/background and return screenshots inspected.
- Required flows warm first-use assets/audio caches: initial menu578GameObjects,
  after tutorial592, after win/Continue/Slayer594. Then3identical measured
  Menu->Start->Restart->Menu cycles remain594/594/594 and1/1/1dungeon RT. Same room10230,
  camera10640and RT-448from initial menu across every flow. World camera/presenter/
  AudioListener1each; menu actors/HUD/timer0, environment true, cinematic false,
  combat pulse0/disabled, full viewport and time scale1. No per-cycle accumulation
  after relevant warmup; no broader memory/performance claim.
- Both final reports:0Unity/JS exceptions and0provider writes. Full run has2intercepted
  Endless reads; fixture interception/external blocking preserved. Conditional bridge
  remains observation-only. No full gameplay suite/stress/soak or itch deployment.
- Player/serialized outputs: E:/CodexTemp/TypingHero/shared-dungeon/build-final.
  Reports/screens: native-final-1 and native-640-final under that task directory;
  exact source snapshots, adapter, drivers and failed receipts retained for review.
  Delete task artifacts after review/replacement, preserving shared toolchain/license/
  cache and compact-tutorial. Final preview and Git/cleanup receipt follows below.

Native driver module SHA256: `0bba8ba9265e897a82715113184069d96a01edd703e17099476e80f9e5d5aeab`.

## Integration and handoff receipt

- Task implementation commit0757ed77706e12cacce54b21666b08e97a8e091a pushed to
  origin/micharel09/shared-dungeon, then fast-forwarded to origin/main from1e921327.
  Fresh fetch verified task commit ancestry on remote main. Workflow is manual-only;
  no itch deployment was invoked. Dirty local main and compact-tutorial WIP untouched.
- Final QA preview opened and actual ready image inspected at1280x720:
  http://127.0.0.1:8251, hidden Python helper PID8568, isolated Edge profile under
  E:/CodexTemp/TypingHero/shared-dungeon/preview. Fixture/external blocking retained.
  Close the preview browser page to end the helper; remove this task artifact
  directory after review/replacement. Keep the pre-existing8246preview untouched.
- All runner receipts archived locally in runner-receipts.tar.gz (814entries),
  early shared-runner failures in failed-shared-runner-receipts.tar.gz, immutable
  final source in snapshots/webgl-1dc641a5*.tar.gz. Exact129-check driver was archived
  and its SHA256 matched the report. Final player and captures remain for review.
- Removed archived /tmp/typinghero-shared-dungeon scratch and its shared-resource
  symlinks, plus only the2verified task-owned failed results025542/025806. Shared
  Unity Library/cache/license/toolchain and other jobs preserved. Completed native
  profiles/downloads/traces removed; preview profile remains active intentionally.
  Owned stale diagnostic SSH process19192closed. No build job/lock remains owned.
- Source/docs/JSON/config whitespace and affected links pass. Unity-generated scene
  YAML and World.meta retain canonical blank-value trailing spaces; those assets
  are verified by strict repeated serialization and final byte identity.
- After this documentation receipt is pushed and verified, remove only the clean
  task worktree/local branch. Keep the remote task branch and final preview/evidence
  for review; no automatic deletion of pre-existing resources or remote branches.
