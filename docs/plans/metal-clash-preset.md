# Reusable MetalClash impact VFX ? 2026-10-22

## Acceptance

- Reuse the approved 24-particle, 1px, floor-rest <=400ms, lifetime <=1.1s effect.
- ImpactVfxProfile owns physics, rendering prefab, cooling/fade and local pulse settings.
- ImpactVfxEmitter.Play(profile, context) works without player or ParrySystem.
- Point, direction, floor, seed and optional recipient are captured at emission.
- Preserve one-floor/one-bounce solver, pause, pooling, snapshot isolation and cleanup.
- Parry retains existing contacts, audio, damage, camera recoil and hitstop.
- Focused standalone/two-profile/seed/mirror/pool/pause/allocation tests; inspect every
  frame and transition of both directions against the approved version.

## Implementation checkpoint

Base: origin/main 916c752. Orca worktree
C:/Users/ADMIN/orca/workspaces/TypingHero/metal-clash-preset,
branch micharel09/metal-clash-preset. Parent dirty checkout is untouched.

ImpactVfxBurst/ImpactVfxPixel contain the existing renderer and historical presets.
ParryClashBurst is a serialized compatibility adapter retaining its script GUID.
MetalClash references the existing rendering prefab and ParryPhysics asset; their
existing GUIDs and particle data remain. Parry calls the shared emitter using its
existing leases; standalone callers use the emitter's eight-lease pool. Both paths
snapshot appearance and physical settings. No solver changes or new materials.
Builder fills missing references only.

## Verification checkpoint

Final scoped Unity72/72 PASS; both-direction frame comparison PASS. Feature integration
verified on remote main at7ef0da3069dc4b621f608de33bcf642d6c3319cd. Task artifacts and external filtered runner:
E:/CodexTemp/TypingHero/metal-clash-preset. Shared runner cache/license are preserved.

## Calling and authoring

Assign MetalClash to a caller's serialized ImpactVfxProfile field, then call:

```csharp
emitter.Play(profile, new ImpactVfxContext {
    point = contactPoint, direction = direction, floorY = floorY, seed = sequence
});
```

Optional recipient is PhysicalParticleProfile.RecipientSnapshot; unitsPerMeter
sets the pulse's world scale (default 1.25). A caller needs ImpactVfxEmitter only,
with the existing arena ClashLightingRig for local illumination. No ParrySystem or
player is required. Emitter returns the playing burst, limits active leases to
8 per emitter, and reclaims the oldest. Clear/disable releases all active leases;
destruction removes idle leases and its independent scene root.

Duplicate ImpactVfxProfile and, when changing trajectories, duplicate its referenced
PhysicalParticleProfile. Color/fade/pulse values and solver groups are copied when
playing. Shared materials remain prefab-authored. A newly emitted burst uses the
latest profile; living bursts retain their own values. Existing parry prefab and
script GUIDs are preserved through ParryClashBurst's compatibility subclass.

## Visual comparison sources selected

Approved full reference segments, selected for complete idle/windup, parry contact,
counter/recovery, falling/bounce, floor rest and expiry:

- E:/CodexTemp/TypingHero/parry-ember-rest/visible-player-left/parry-right:
  audio-video.webm 0.024515..2.217050s; input ef2e25d0;66frames/65pairs.
- E:/CodexTemp/TypingHero/parry-ember-rest/visible-player-right/parry-left:
  audio-video.webm 0.018250..2.162665s; input22038076;62frames/61pairs.

Both source scope.json files retain timestamps and exact input hashes. All128
numbered reference frames and126consecutive pairs directly inspected via every
review12 sheet, nearest-neighbor enlarged: held pre-contact guard, white/gold
Pimen contact, thin backward spray, orange tail, fixed floor dots then clean expiry.
No external-game research is part of this extraction. Output comparison is recorded below.

## Final technical input

Input4e59bdb41d6d22a5c113353f270154ad746213e95691f084dc9d2ff4f9a50c8f,
run20261022T144224-webgl, Unity2022.3.25f1:72passed,0failed/skipped,
15.0103695s. Selected ImpactVfxTests/ParryClashTests/PhysicalParticleTests;
VoxelCombatTests.cs is another partial of ParryClashTests, so both existing shared
pulse tests are included. Updated weak-pulse test verifies the inactive local
light retains its legacy range when the strongest pulse is a voxel impact.
New builder test verifies two calls preserve a custom impact profile/reference,
and only a null reference is filled with MetalClash. Tests/artifacts are external
runner snapshots, with no runner/dependency/product QA hooks committed.

The first final-run attempt stopped before test execution at the unchanged3GiB
runner free-space guard (a few MiB short). Verified32matching file hashes before
removing the task-owned duplicate build-qa; archived the intermediate result to
normal-interim and verified32local player hashes before deleting only its remote
player duplicate. Shared cache/license/source/other jobs were preserved. The retry
runs the exact same product input. No acceptance guard was lowered.

approved-source-audit.json verifies solver/profile, ParryPhysics, contact/Compact
materials and both actor prefabs match the approved ground-rest snapshot exactly.

## Final visual and integration evidence

- Final normal run20261022T144224-webgl / input4e59bdb4 and mirror
  run20261022T144950-webgl / input455ad0aa both pass WebGL build and strict
  repeated builder. Final prefab is byte-identical after LF normalization to the
  repeated builder's serialized output. All product inputs match the final archive;
  both32-file local players match their immutable server players by SHA256.
  Normal/mirror input archives differ only by PixelSparkMirrorQA.cs (editor-only
  transform mirror fixture). No product runtime/art/simulation difference.
- Native headless Edge / Playwright1.58.2 / Intel UHD ANGLE D3D11,1280x720:
  visible-right18/18 and visible-left18/18 PASS,0exceptions,0provider reads/writes.
  Actual menu/keyboard parry, unchanged successful-parry HP,24particle cap,
  recipient-facing flight, same-render counter hit/contact and full expiry pass.
- Full final selected source segments (recorded scope before inspection):
  visible-right/parry-right/audio-video.webm0.022600..2.220065s,62numbered frames,
  and visible-left/parry-left/audio-video.webm0.021835..2.142730s,100numbered frames.
  Idle before new windup through parry/counter/recovery and full floor-tail expiry;
  no segment narrowed after capture. All162/162frames and160/160consecutive pairs
  directly inspected in every review12 sheet at nearest2x. Per-frame observations,
  original durations, approved reference phases and transition notes are in each
  visual-review.csv; scope.json records actual inspected counts. Render frame IDs
  are consecutive throughout both segments; repeated cels and long frame holds
  remain. Reference coverage is separately128/128frames and126/126pairs above.
- Match findings: stable sword contact and held white/gold Pimen flash; distinct
  thin recipient-backward rays, no expanding ring; orange1px fall/floor rest,
  final fade and no residue. Both mirrored actor grips/feet and counter transitions
  remain coherent. Compare by phase/effect age because capture cadence and lesson
  words differ; no bitwise whole-scene/FPS or independent subjective-audio claim.
  Unchanged sprite sources/actor prefabs retain prior animation evidence; this
  extraction authors no new sprite poses. Captures cover these selected render
  sequences, not unobserved sessions or a performance benchmark.
- A premature mirror-driver launch ended before browser execution because the
  archive download was still running. After awaiting download, the actual mirror
  run passed18/18. This setup attempt is not reported as a gameplay PASS.

## Handoff resources

E:/CodexTemp/TypingHero/metal-clash-preset retains final players, immutable input
archives, XML/build/browser receipts, per-frame PNG/JSON/CSV/sheets, captured
WebM/audio and reproduction scripts. Keep for review/reproduction; disposable
when those comparisons are no longer needed. Task server root cleanup and clean
Orca worktree/local-branch cleanup follow verified remote integration; preserve
shared source/Library/cache/license and parent checkout changes. Remote task branch
is retained for review (no automatic deletion).

## Remote integration and cleanup

- Commit09a58f5 implements the preset. Newer origin/main5f371fa adds the approved
  dungeon torch flames; merged as7ef0da3. Resolved only the systems.md conflict,
  retaining the preset VFX row and newer torch-lighting row. All VFX/Combat/
  ParryClash/actor/SampleScene files remain identical to the verified task commit.
  Upstream torch assets/runtime and its prior visual evidence are preserved.
- Integrated input1eb1cbe4f3d13a52d8477e93dab37c8a69761c4381cbad67f2805c1c812175ce,
  run20261022T145953-quick:6/6PASS,0failed/skipped. Actual local profile pulse,
  standalone pool/pause/expiry/reuse, parry pulse pause, weak voxel overlap,
  default parry adapter and upstream pixel-lantern clock/source-mesh lifecycle.
  No broader runtime/build recheck: relevant VFX inputs are unchanged; full-frame
  captures remain attributed to their exact pre-torch-merge snapshots.
- Task branch and main pushed; fetched remote confirms09a58f5 and7ef0da3 are
  ancestors of origin/main, with main at7ef0da3 before this documentation closure.
  Parent dirty main was never checked out, reset, stashed or modified.
- Final integration XML/logs archived to tests-integration-result. Task server root
  /data/typinghero-game-loop/task-metal-clash-preset removed after verifying its
  final input marker and absence of any container mount in that target. Shared
  runner was occupied by another job, which was left untouched; isolated task-root
  cleanup required no shared-source mutation. Shared source/Library/cache/license
  remain. Clean Orca worktree/local-branch cleanup follows final remote verification;
  delivery.json in the retained artifact root records the outcome. Remote task
  branch stays for review.


## Reusable authoring follow-up — 2026-10-23 (in progress)

User approved implementing the optimization plan for particles, flashes and local
lighting. Base origin/main325327418a6c; independent Orca worktree
`C:/Users/ADMIN/orca/workspaces/TypingHero/impact-vfx-authoring`, branch
`micharel09/impact-vfx-authoring`. Parent dirty checkout is preserved.

Acceptance: configurable renderer IDs/material bindings, 0/1/3+ flash layers,
count/lifetime/tint/pixel size/world cap, grid/start/frame/color-alpha curves;
profile-owned local pulse enabled/color/intensity/hold/duration/decay/range/offset.
Scene rig still owns actual light templates and overlap budget. One public
`ImpactVfxEmitter.Play(profile, context)` path for standalone and parry, including
profile.prefab ownership. Historical presets and parry-only metadata belong to
the compatibility adapter. Preserve GUIDs, solver/RNG, fixed40 physical slots,
MetalClash24 particles/1px/400ms floor rest/1100ms lifetime, pause/pooling/cleanup,
zero warmed Update allocations and immutable living snapshots. Builders only fill
missing references; versioned migration never resets authored arrays/curves.
No extra production material preset, wall/stair physics or gameplay/audio changes.

Frozen pre-refactor snapshot0c42fde68500bdb4766722a0cd56fddf19bf01711280172145d9c0c812d89f51
captures both directions/seeds0,7 at68 consecutive1/60s steps across all5 renderers.
Independent JSON SHA256079ac532a82ecc8d521993391f19f5974fe10a22fc751956e3415f2e477f3360;
compressed fixture is `Assets/Tests/PlayMode/MetalClashBaseline.bytes`.
Artifacts/reproduction scripts: `E:/CodexTemp/TypingHero/impact-vfx-authoring`.
Server task root `/tmp/typinghero-impact-vfx-authoring`: task outputs relocated to
tmpfs because /data is just below the existing3GiB free guard. No guard reduction,
shared cache/library/license deletion, or other task cleanup. Shared runner lock
preserves concurrent jobs. Initial /data task snapshot remains task-owned pending
cleanup. Runtime/visual acceptance remains pending until results below.


### Follow-up verification checkpoint

- First implementation snapshot7c7166daff50:78/78 scoped Unity tests pass
  (`20261023T032601-quick`). Final runtime/test source snapshotdb94155fd2b6692296437179e45ba41e8ec131454ac68eeba2db55047644347d:
  **83/83 PASS**,0failed/skipped (`20261023T033251-quick`). Includes the frozen
  pre-refactor trace at every step, pure generic prefabs with0/1/3 flash layers
  and IDs77/10/20/30, custom grid/tint/curves/scales, immutable curve keys,
  concurrent profiles/pulse color/decay/overlap, profile.prefab selection by parry,
  invalid binding/overflow rejection, repeated migration preservation, seed/mirror,
  pool/pause/reuse/cleanup and zero warmed particle-update allocation. Three
  directly affected old PixelSpark parry direction/teardown/unload tests also pass.
- Frozen normal baseline build/strict repeat (`20261023T031901-webgl`) and mirror
  (`20261023T032949-webgl`) pass. The first old native driver failed its automatic
  counter assumption on unchanged main: main3253274 requires a completed word
  within800ms to queue a follow-up. Preserved failed receipt in baseline-right;
  updated only the external driver to type the current word after real parry.
  No gameplay mutation. Both corrected baseline runs pass18/18 native checks,
  no exceptions/provider calls. Their source segments were selected before capture:
  idle, complete fresh windup/parry/success/recovery, typed queued slash and full
  recovery, complete ember tail and empty endpoint. Full source video/timing in
  each scope.json. Baseline right178frames/177pairs, video1.36478..5.433445s;
  left124frames/123pairs, video0.01562..2.68949s. All302frames/300pairs directly
  inspected in numbered native/nearest2x sheets; per-frame/transition observations
  recorded in each visual-review.csv. Small blade-centered contacts, directional
  hot rays cooling to1px orange ground embers, fade and coherent recovery match
  the current source. Final output comparison remains pending.
- Final normal/mirror builds are queued under the existing shared runner lock;
  another job is left untouched. Reuse83 passing runtime results while their
  relevant inputs remain unchanged. Authoring docs now describe profile ownership,
  binding IDs/validation, public API, snapshot semantics, scene light templates
  and the unchanged Compact shader/one-floor solver contracts.


### Final implementation and acceptance

- Hạt/flash/local pulse are profile-owned. Generic runtime has no ParrySystem,
  Preset/BCompact, ParryContactSnapshot, Pimen slot0/3 or320px raster dependency.
  Fixed40-state capacity, RNG constants, first-visible-step timing and Compact
  shader UV/mask protocol remain algorithm/resource contracts. Existing prefab
  object IDs, GUIDs and all old references remain. Solver/ParryPhysics bytes are
  unchanged. Production still has only MetalClash; no gameplay/audio/camera edits.
- Late review found two necessary compatibility edges: infer a missing parry
  profile from its existing custom prefab instead of assigning MetalClash; resolve
  an emitter-added lease when an active generic prefab Awakes before that lease
  exists. The latter prevents expired inactive objects remaining in active leases.
  Dedicated tests added. Builder4/4 PASS (6135a1ff...,20261023T035557-quick).
- Final snapshotcbff319fd0c09a9cb26c5f4b5baeafb6d656f2e5e2640ad567588c16247420d8,
  run20261023T035820-quick: **85/85 PASS**,0failed/skipped. All2229 snapshot inputs
  match checkout after text-only LF normalization. Includes the complete earlier
  scoped set, custom builder references and active-prefab expiry/reuse. Metadata
  whitespace correction is the only nonbehavioral difference from earlier83 inputs.
- Normal d6a015205aa4d34ce16fda9ff3fae94b09a8ce1ed3cee59f339e92ceed639e4b
  (20261023T034250-webgl) and mirror33823a196b16484a4604a222d7e3677a84a8c4d2d15c2431188a53e58aa68d41
  (20261023T034946-webgl) pass WebGL/strict repeated builders. All33 files in each
  baseline/final normal/mirror player match local/server SHA256. Immutable source
  archives, XML, logs and source/static/generated audits retained in artifact root.
- Native Edge155.0.4283.62/Playwright1.58.2/NVIDIA GTX1650Ti ANGLE D3D11,
  real keyboard/mouse with fixture/external-network isolation: final36/36 PASS,
  0exceptions/provider calls. Right109frames/108pairs, video0.01893..2.532805s;
  left133frames/132pairs, video0.009915..2.54843s. Source segment selection recorded
  before capture. Inspected all242 output frames/240pairs and compared by effect
  age/hero phase to all302 baseline frames/300pairs: **544frames/540pairs total**.
  Every Unity render ID is consecutive; positive durations and held/repeated cels
  retained. Numbered native PNGs/nearest2x sheets, original WebM/audio, timing and
  explicit per-frame/transition notes remain under each capture directory.
- Both directions retain small white/yellow blade contacts, thin recipient-directed
  launch, independent orange1px cooling/fall/ground hold/final fade, no ring or
  residue, original success cels and queued-slash recovery. Words/cadence/encounter
  labels differ across sessions; no whole-scene pixel/FPS equality or subjective
  audio judgment is claimed. Actual audio-source/contact/damage alignment checks
  pass. Generated baseline/final serialized outputs differ only in MetalClash.asset,
  ParryClash.prefab bindings and Player.prefab profile reference. Copied only that
  one new player reference; unrelated scene/material/importer normalization stays out.
- Visual receipts intentionally remain attributed to their immutable players,
  before the final missing-lease guard and custom-reference Editor fix. MetalClash
  already has its lease, so that new branch is not used; render configuration,
  materials, physics and render evaluation are unchanged. The player reference
  equals the built value. Final85 tests verify changed generic/Editor paths and
  rerun frozen traces/pool/pause/allocation. No native missing-lease-fixture claim.
- Remote integration and scoped resource cleanup follow the final diff review.


### Follow-up integration

- Implementation commit1590347 pushed to origin/micharel09/impact-vfx-authoring.
  origin/main advanced to e491984 (authored world effects/camera impacts); merged
  as cb4c84e. Automatic merges preserve both source-backed memory updates and
  shared CombatFeedbackBuild behavior: upstream camera profile ownership plus
  this task's missing-reference migration. No VFX runtime/profile/prefab/player
  differences from the tested task commit. No parent checkout/index changes.
- Integration snapshotf4fa6f0c3fa360570ac856c537e686b01cbec31d27cb0346094c53848b0b08d6,
  run20261023T040800-quick: **12/12 PASS**,0failed/skipped. Frozen baseline trace,
  active generic lease expiry/reuse, custom reference migration, public parry
  profile.prefab route, pulse color/curve snapshots, parry pause/weak voxel overlap,
  selected-world lens/impact-profile rebinding and pixel-lantern mesh lifecycle.
  Passing85 task tests remain scoped to their original inputs; VFX render/physics
  inputs are unchanged by integration. No repeat of unrelated full suites/builds.
- Newer world authoring source has independent39Unity/53native and355frame/
  350pair observations in combat-hanging-light.md. Preserve that evidence and
  its explicit VFR/subjective/performance limits. This task's full-frame VFX
  comparisons remain attributed to their pre-integration immutable players.
- Evidence/player/scripts retained at E:/CodexTemp/TypingHero/impact-vfx-authoring
  for review/reproduction; disposable when no longer needed. Only task server
  roots and the clean task Orca worktree/local branch are removed after remote
  verification; shared Library/cache/license, other tasks and parent WIP remain.


### Verified follow-up delivery and cleanup

- Task branch and main pushed. Fetched origin/main36d6e27d49918e192ef3aab87a127c8711c4e692
  contains implementation1590347 and integration cb4c84e; both ancestry checks pass.
  This documentation-only closure follows that verification, with no runtime edits.
- Removed only /tmp/typinghero-impact-vfx-authoring and the initial
  /data/typinghero-game-loop/task-impact-vfx-authoring snapshot root. Verified exact
  resolved targets, expected tests-merged/baseline-normal markers and no running
  container mount in either target before deletion. server-cleanup.json retains
  receipts. Shared runner/cache/Library/license and other jobs remain untouched.
- Clean Orca worktree/local task-branch removal follows this closure's remote
  verification. Local delivery.json records final remote HEAD/ancestry and actual
  Orca cleanup. Remote task branch remains. E:/CodexTemp/TypingHero/impact-vfx-authoring
  retains immutable players, source archives, tests, per-frame/video evidence and
  reproduction scripts for review; delete those evidence artifacts only when no
  longer needed. Parent dirty checkout/index remains untouched.
