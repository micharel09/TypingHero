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
