# Space cue only at the press beat

## Implementation

Fresh task worktree from origin/main9e5252a0; dirty parent preserved.
Adapt ParryCuePresenter: valid attack telegraph AND (remaining <= existing
fillLeadSeconds200ms OR existing parry window active). Retain100ms tap cycles,
anchor/colors/lock, pause/hitstop and existing lifecycle guards. Input/combat
and tutorial clips unchanged. Existing presentation test now checks hidden
800/300ms, visible200ms, tap phases, open window, lock, cancel and next strike.
Gameplay/system memory reflects source; runtime acceptance remains unverified.

## Verification checkpoint

- Local git diff --check PASS.
- Ubuntu reachable; first run could not acquire shared lock, second timed out
  after180s with zero tests. Third retry was stopped while still waiting: existing Unity log did not
  advance for several minutes and read-only Docker stats also stalled. No tests
  ran for this task; do not interrupt the pre-existing lock owner.
- Test filter: ParryCuePresentationTests|ParryCueTests|PlayerParryTests.
- Player check pending. Temporary build-only adapter reuses repository run.py /
  server.py; browser driver adapts existing cue driver and fixture isolation at
  E:/CodexTemp/TypingHero/parry-cue-on-beat. Do not claim player PASS.
- User explicitly requested main integration and task closure despite the disclosed
  runtime verification gap. Static review and git diff --check pass; Unity tests
  and player acceptance remain unverified. Task temporary driver/build adapters
  are disposable; retain only this checkpoint and source/tests on main.
