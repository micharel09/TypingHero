# Windows Unity adapter verification

Scope: stale XML rejection and retry-safe mirror QA builds in
`tools/game_loop/local_windows.py`; no runtime, builder, CLI or Unity changes.

Implementation: per-invocation UTC/UUID evidence directories, strict XML acceptance,
preflight before fixture creation, byte backups/restoration, staged output promotion,
and owned Editor termination/wait on Ctrl+C. Retain failed evidence and backups.

2026-10-25: seven Windows unittest methods passed, covering valid/invalid/stale XML,
failed/failed-to-launch/interrupted mirror builds, same-Library retry, existing
output/fixture/meta preservation, restoration errors and owned-process shutdown.
Actual Unity evidence is recorded below after completion.

Real Unity2022.3.25f1 (530ae0ba3889), existing local WebGL module:
- Workspace/evidence: `E:/CodexTemp/TypingHero/windows-adapter-verification`.
  Snapshot2452files, SHA256
  `bce17d965e9098a58da074f4355987c6b2c3293e4db71fbfb0b3b2c3c67b31c7`.
- Initial `20261025T013121.646128Z-d94236c8e94744818f9576c683513bca-tests`
  initialized a fresh Library, entered PlayMode, then emitted no XML/progress for
  several minutes. Owned Editor11948 was stopped after target verification;
  adapter exited1. This attempt is not a passing test receipt.
- Mirror QA `20261025T014303.192316Z-8971c0858244410b9a094fdd74f42089-build-left`
  exited0, created player-left/index.html and qa=true metadata. Strict repeated
  builder reports unchanged serialized objects, references and listeners.
  Scene and meta matched backups byte-for-byte after Editor exit; fixture/meta absent.
  Scene SHA256 `1256acddf9884e09bad0f808e24c4bb89937553246b1f0d0c6eb42b9381e4955`;
  meta `204f446bc13539151c15af875550a7fa6f791662e967572a1b4b06fd0005dad1`.
  External fixture saved the mirrored scene then logged ArgumentException at line23
  while reopening an empty previous scene. Unity still completed QA build. This is
  a fixture limitation, not an exception-free QA claim; fixture input retained beside log.
- Retry `20261025T020149.227217Z-594f880ba96345cab418c144cc3f541e-tests`
  on the same Library: rootPassed,44/44casesPassed,0failed/skipped/inconclusive;
  adapter exited0 and used only this fresh XML. Initial logs remain separate.
- No Parry video/browser/full gameplay suite, runtime or version changes.

After remote verification, remove only task-created source/Library and player-left;
retain results, scene backups, snapshot manifest and unittest receipt for review.

## Workspace lock and deferred cancellation follow-up (2026-10-25)

Acceptance: one adapter owns a workspace from preflight through source restoration
and output publication; SIGINT cannot interrupt restoration or leave a live owned
Editor writing to restored source. Existing XML acceptance and CLI stay unchanged.

Implementation uses a retained `.local-windows.lock` with a nonblocking Windows
byte-range lock for snapshot/tests/build. Busy invocations fail before source
inspection/mutation. Backup restoration is restricted to the invocation that
created the fixture. SIGINT records cancellation; the owned Editor is terminated
and awaited before cleanup, repeated SIGINT is deferred, and cancellation exits130
without publishing a player. Each source file is restored through a unique sibling
file, fsync, os.replace and byte readback; cleanup attempts both source files and
the owned fixture/meta even when one restore fails. Failures retain backups and
partial players. Two-file restoration is not a single atomic transaction.

Scoped adapter tests and actual Unity cancellation/retry receipts are recorded
below when complete. No gameplay suite, video or browser rerun is required.

- Windows adapter unittest19/19PASS, including subprocess ownership/lock contention,
  crashed lock owner, separate workspaces, four repeated-SIGINT boundaries,
  cancellation before launch/at publication/during XML validation, temp write/fsync/replace/readback
  errors, retained-temp reporting, failed fixture ownership and normal/right build.
  Receipt: `C:/CodexTemp/TypingHero/windows-adapter-cancel-lock/adapter-unittest.txt`.
- Actual Unity workspace: `C:/CodexTemp/TypingHero/windows-adapter-cancel-lock`.
  Snapshot2453files SHA256
  `cbacdf42d415f2e63d5e8fdb97bdb7ee3cde595ab8f9c9dc0112310bc1e9da18`.
  Unity2022.3.25f1/WebGL module reused; no versions/license/dependencies changed.
- Fixture verification copy guards empty previous scene before reopening it;
  original SHA256 `84a5aa814f39a9d9095ea7f0c591a8dedec9bcd257e6c28e2a38285e72b0bf5b`,
  copy `c84d8bb5c471dcc85a1b14524fe8433c128e4bf294c804bb57136b22b6c96c67`.
  Both files and fixture.diff remain in the evidence root; external original untouched.
- Cancelled mirror `20261025T021604.720002Z-c26ab1eb78ed40e5bec7624082e359d7-build-left`:
  fixture-save marker observed02:20:38.920855UTC, owned Editor36340 terminated,
  first restore02:20:39.379520UTC with confirmed Editor exit1. Adapter result130;
  scene/meta byte-identical to backups, fixture/meta absent, lock reacquired and
  no player-left published. `cancellation.json` and `cancel_unity.py` retain ordering
  and reproduction. A concurrent actual tests invocation was rejected as busy.

- Same-Library retry `20261025T022118.921171Z-0175ebcfc2ae4727bcb1a025bb47db53-build-left`:
  Editor/adapter exit0; player-left/index.html present and qa=true. Strict repeated
  builder and fixture-completion marker pass, no Exception/errorCS/BuildFailedException
  in the retry log. Scene/meta match their pre-build backups byte-for-byte;
  fixture/meta and restore temporary files absent. `retry.json` contains hashes.
- ImpactVfxTests/ImpactVfx implementation/profile and Unity/packages are unchanged
  from the prior44/44receipt; later main delta is the separately verified Slayer
  body-center path. This task reuses that scoped receipt rather than claiming a new
  gameplay test run. Current19adapter tests cover XML acceptance and cancellation.
- `git diff --check` passes. Final task changes are adapter, its Windows-only tests,
  verification guidance and this checkpoint; no runtime or builder changes.
- After verified remote integration, task-created source/Library and player-left
  are removed; results, backups, manifest, fixture originals/diff, cancellation and
  retry JSON plus harness/unittest receipt remain for review. The retained evidence
  root is an archive after cleanup; create a new workspace for another Unity run.
