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
