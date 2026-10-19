# Player attack sorting

## Accepted scope

Raise the combined player/body/sword SpriteRenderer to order 1 only during the
normal attack and all three Slayer attacks, using the Animator Attack tag.
Keep the existing sorting layer and skeleton order 0. Restore original order
outside attacks and on disable; transition destination wins, pause holds order.
Preserve damage, timing, sprites, GUIDs and scene/prefab references. No itch.io update.

## Acceptance and checkpoint

- Added PlayerAttackSorting on the existing player prefab; tagged Slayer states.
- Added actual-controller PlayMode checks for all four attacks, interruption,
  crossfade destination/consecutive attacks, pause/resume and disable/re-enable.
- Doctor initially blocked by another running job holding the shared runner lock
  (`Errno 11`, confirmed active Unity container). No other job was interrupted.
- Doctor PASS. Quick PASS **51/51**, including four new sorting tests, input
  `aacc162b49e95ee8d0856ddda79571b93b5efc4d57146f4cbffdf59b30499bfd`,
  result `20261018T051652-quick`. Initial Quick47 ignored the new test due to a
  malformed metadata GUID; corrected and reran all tests. Runner contracts8/8 PASS.
- Baseline1280x720 SwiftShader was too slow for the existing parry driver
  (~224ms/frame, zero parries), so it is not Slayer evidence. Baseline640x360
  succeeds with real parries/typing. Capture before opening pause: the pause menu
  covers the actors. Hold typed keys long enough for the actual render frame.
- Existing dirty checkout changes are preserved. Gameplay/system/verification
  memory updated only for this change.
- QA build input `6e29a49a4108e59a08ea5b4d58271053b55d2143d9ee3bc73ac0fda3df0e4948`
  built successfully. Targeted run `20261018T051908-webgl` reached all three Slayer
  states (mask14), order1 versus skeleton0, zero frame mismatches/exceptions.
  Actual normal overlap and all three Slayer/trail frames reviewed against baseline
  `c17c44b021207980a97430e999ef78e0fdf7f51d2e17337a2797fdd2c32c84b2`.
  Baseline final real-input run6checks/33.479s,8fixture reads/0writes.
- First QA run stopped at the driver's premature VFX assertion after881checks:
  red overlay legitimately fades after Slayer ends. Corrected driver to wait for
  `redAlpha==0` and restored environment; no product/timing change. Death/restart
  was pending at that point. Other jobs temporarily reacquired the shared lock;
  preserved them and resumed after completion.

## Completed acceptance

- Final `20261018T052807-webgl`: **924 checks PASS**,69.609s, input
  `a1a06d81537fe6c96d123f1e47fa393be48b768d50cc2012224f6c80aa54f1b6`.
  Real normal attack, parries, all three Slayer states/captures, repeated attacks,
  pause/resume, VFX restore, hit, death and restart; zero sorting mismatches and
  zero JS/Unity exceptions.17fixture reads/1intercepted write; external network
  blocked. Final four actual-render frames reviewed against the baseline.
- Chromium153.0.8010.12/SwiftShader,640x360,2CPU/8GiB/shm1GiB. Native device FPS,
  audio, stress/soak and production build were outside this task's verification.
- Final source/diff review, syntax/metadata and whitespace PASS. No changes to
  damage/timing, asset GUIDs or scene bindings. No itch.io publication.
- Cleanup completed: removed only this task's local capture directory
  `E:/CodexTemp/TypingHero/player-attack-sorting`, server task-temp directory and
  generated results051400/051652-quick,051908/052807-webgl after these receipts.
  Retain shared source/Library/toolchain/latest QA build for later tasks. No branch
  or worktree created.

## Spark clarity and parry sorting - accepted continuation (2026-10-18)

- Reference canvas960x540: Normal48px, Parry56px, Lethal64px, Slayer28px.
  Preserve shapes/palette,150/180/100ms clips/events/audio, GUIDs, pools/anchors,
  and spark order30. Regenerate seven prefabs through CombatFeedbackAssets.
- Tag player_parry/player_parry_success as Parry; Attack or Parry raises the
  combined player/sword/trail to1 over skeleton0. Destination wins; pause holds;
  idle/hit/death and disable restore captured original order. No public API,
  damage/hitbox/parry-window/gameplay/camera change. No production/itch.io build.
- Verify actual controller transitions/interruption/pause/disable and actual
  prefab sizes, durations and reused scales. Quick has no per-test filtering;
  run its supported category. Real keyboard WebGL must cover normal hits,
  parry success, Slayer chains/lethal, pause/resume/restart with all Firebase
  writes intercepted. Inspect post-LateUpdate sorting/contact frames and normal/
  parry visibility at960x540 and640x360 against cue/tray.

### Verified continuation

- CombatFeedbackAssets and7generated prefab scales updated; PlayerAttackSorting
  now recognizes Parry, and the2real controller states carry that tag. Extended
  existing PlayMode tests and the conditional read-only post-LateUpdate observer.
- Doctor PASS. Final source equals Quick52/52 input
  e4f99c581d3125aa640dcac9403ae6fc9d983598aa03cad7a9a4591206d4760d
  (20261018T072736-quick). All4actual-controller sorting tests pass with Parry,
  including destination/interruption/pause/disable and nonzero-original restore.
  All7actual prefab profiles pass size/order/clip/event/reuse checks.
- QA build20261018T072017-webgl/repeat-builder passed; exact input reconstruction
  proves it differs from final Quick only by its7regenerated prefab scales.
  Clips/controllers/textures/meta bytes, GUIDs and scene references are preserved;
  all other generated file changes are whitespace only and were not integrated.
- Focused native acceptance5314checks/171.829s PASS: real normal/parry input at
  960x540 and640x360, parry pose/success1 above skeleton0,0sorting mismatches;
  all3Slayer attack states, all4Slayer variants and lethal, pause/resume, actual
  Slayer exit and fresh restart. Final40/0HP/11parries, spark mask127.
  26fixture reads/1intercepted write,0exceptions; no live leaderboard writes.
- Reviewed960/640normal/parry collision video and cue frames: sparks at existing
  contact locations, sword/trail in front, cue/tray clear. White Slayer impact
  frames and lethal reviewed. Direct speaker audio/device FPS/full software
  WebGL are not claimed. Environment, commands/hashes: docs/game/verification.md.
- Failed attempts: software browser driver stopped before its first attack while
  typing an expired/missed word; no sorting mismatch/exception, not a profile PASS.
  An initial native load preceded WASM output and timed out; completed-build
  native rerun passes. No product input/timing change or relaxed acceptance.
- Final review PASS:17task files only; source/metadata/diff/whitespace/document
  links checked, unrelated dirty checkout preserved. No runtime acceptance remains.
  Cleanup PASS: task local/server temp and captures/drivers/reports removed;
  owned Edge contexts and QA host8127 closed, results072017-webgl/072736-quick
  removed after snapshot/test/build-log provenance checks. No task temp remains.
  Shared source/Library/toolchain/latest QA build stay. No branch/worktree created.
