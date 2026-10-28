# Score multiplier and combo time clarity

## Scope and acceptance

- Preserve real scoring, +0.15 growth, x3 cap, Unity rounding, combo timing,
  Slayer colors, popup, public APIs and dependencies.
- Format the multiplier with invariant `0.##`; retain the fitted score row,
  pulse reserve and decay inside the existing frame.
- Explain score multiplier and combo expiry in the existing Score / Combo card;
  keep its arrow aimed at ScoreCluster.
- Show SCORE MULTIPLIER for 2.4 unpaused seconds on first activation per match.
  Expiry/re-activation does not repeat it. EndlessRun retains the presentation-only
  flag across duel reloads; Restart creates a fresh run. Non-Endless ResetScore
  re-arms the local notice.
- Verify requested multipliers/actual awards, first activation, typing/parry renewal,
  expiry, Slayer, Pause/Resume, Restart, long scores and maximum pulse.
- Directly inspect gameplay and guide at1280x720 and400x300; produce a short clip.
  Run strict repeated builder, preserve scene IDs/bindings, update affected memory.
  Commit/push/integrate to origin/main; no itch.io release.

## Workspace

- Base origin/main29b9029c; remote https://github.com/micharel09/TypingHero.git.
- Worktree D:/Projects/TypingHero-worktrees/score-combo-clarity;
  branch micharel09/score-combo-clarity. Original dirty main untouched.
- Unity2022.3.25f1/fly-server doctor passed, license present,39.7GiB free.
- Implementation, focused verification, direct review, clip and remote integration
  complete. Resource cleanup and final documentation receipt follow below.

## Reuse basis

- ScoreUI_ComboHUD retains its existing width fitting, placement and1.2x pulse
  reserve; ScoreUI_Flash remains unchanged. Only the displayed format changes.
- CombatR7Build.LabelOnce and BlackletterUILayout.Size author the new child label
  using the current font/color system. TutorialBuild.Guide reuses Card/Text and
  the existing ScoreCluster arrow; only this card's copy/height/font size change.
- The short notice follows CombatHUDFeedback's unscaled remaining-time pattern,
  first-frame protection and pause/input-lock gating. Existing ScoreSystem
  SetPaused/ResetScore paths control it; existing EndlessRun.Begin/Restart own
  the lifetime of the retained presentation flag. No new lifecycle owner or API.

## Focused Unity verification

- Unity2022.3.25f1/fly-server result20261028T021251-quick:5/5passed,0skips,
  duration3.6109s; input95fa9b95b8aa386d046b6619b6a8481a5391a7d90ace20806e5edec4898faa24.
- Final Cobblestone48/42 geometry:128combinations of four totals through20digits,
  eight multiplier values (including1.15/1.3/1.45/1.5/2.95/3), four pulse sizes.
- Real production typing path verifies actual multiplier/award/cap without rounding
  the stored multiplier. In fr-FR, labels still use a dot and trim trailing zeros.
  Word5/6/7 award12/13/14; third raw multiplier is1.44999993, displayedx1.45.
  Unity midpoint-to-even14.5->14 and15.5->16 are retained.
- Notice checks cover hiddenx1, fifth-word activation, unscaled time through slow
  time, explicit Pause and reading lock, auto-hide, no repeat after combo reset,
  fresh non-Endless reset, retained Endless run, Restart and disable cleanup.
  Existing threshold/timeout and award-only tier pulse regressions also pass.
- First immediate runner request encountered the existing shared lock; queued retry
  waited without interrupting another job. No failed game test was counted as pass.

## WebGL continuation

- First build20261028T021703-webgl uses the same input95fa9b95. Editor stopped
  logging after loading SampleScene while consuming one CPU for over five minutes.
  No build/strict-repeat/browser PASS is claimed for that attempt. After verifying
  the /results mount, stopped only its owned container (exit143); log retained.
  A fresh Editor retry is queued on the existing shared runner; other jobs/cache
  and the passing focused tests are preserved.
- Retry20261028T022932-webgl progressed through scene loading but logged CS2001
  for TutorialPractice/TutorialAtlasCache/TutorialAtlasPage/TutorialPlaybackTrace,
  absent from this snapshot. A stale shared compilation cache is suspected;
  no successful build is claimed. Stopped only its verified owned container143.
- Switched to the existing repository local_windows.py adapter and installed
  Unity2022.3.25f1/WebGL module, using a fresh task-owned Library, without changing
  dependencies or source. Snapshot2547files, LF-normalized input
  6f96da77085592384aa29af8b56dcaf0feb638f7a25d27acd18a2b5284bd3a0d.
  Local workspace:E:/CodexTemp/TypingHero/score-combo-clarity/local-unity;
  invocation20261028T023430.711731Z-913d89db31e141ad9c2f474226ee71ef-build-right.
  Initial package resolution completed in112.528s; import/build still pending.

## Verified result

- Local Windows build completed successfully with qa=true. Strict two-pass
  builder reports serialized objects, references and listeners unchanged.
  Imported five scoped scene blocks and four new hint blocks; all 1121 old IDs
  remain, 1116 unrelated blocks are byte-identical. Four empty YAML field spaces
  were trimmed. Task blocks otherwise match builder output. All 33 player hashes
  are recorded in the retained verification receipt.
- Native Edge156.0.4314.50 / Playwright1.63.0 / NVIDIA GTX1650Ti ANGLE D3D11:
  89/89 PASS in54.582s, zero JavaScript/managed exceptions and provider IO.
  Fixtures and external blocking remained enabled. Real typing, first activation,
  renewal by typing/parry, expiry/timeout, pause-held notice, no repeat in the same
  match, Restart, Slayer x3/natural exit and normal combo after Slayer all pass.
- Directly inspected all 11 final captures: guide, x1, x1.15 and Slayer x3 each
  at 1280x720/400x300, plus real parry renewal, natural Slayer exit and later combo.
  Copy fits the enlarged card and arrow points at the score frame. Text/decay stay
  inside the frame, the hint sits below it, HP/tray/popup preserve their positions.
- Short native-input demo passes 7 checks with zero exceptions/provider IO.
  Final requested clip: E:/codex-data/artifacts/TypingHero/score-combo-clarity-20261028/Score-Combo-Clarity.mp4,
  H.264,1280x720,25fps,341frames,13.64s,1,617,840bytes;
  SHA2560df1d3b5ad49a7a4ac0e5078d805a39762fb5616034973f625602a1d70ab7825.
  Full decode passes. Inspected guide, live gameplay/notice, pause and expiry
  preview frames; no sprite-animation/reference comparison is claimed.
- Minimal Playwright FFmpeg lacked the required filters/VP8 decoder, so final
  export reused the already installed full FFmpeg9.0.2. No project dependency changed.
- Current origin/main2339b65d adds independent actor-art/audit changes; retain
  them during integration. Relevant HUD/scoring/font code is unchanged upstream.
  No full suite, stress/soak, production build or itch.io release.

## Remote integration and cleanup

- Source commit e50ddfcfc3c6f3add5c045c0cefbe2b43899e397 was pushed to
  micharel09/score-combo-clarity. Integrated newer main art, audit documentation
  and execution-host guidance without conflicts; none of the tested code, scene,
  data, UI fonts, packages, settings or runner files changed. Passing focused
  results remain applicable; no broader runtime suite was repeated.
- Initial main push encountered a concurrent documentation commit. Fetched and
  merged its 28-line audit note, rechecked affected paths and whitespace, then
  fast-forwarded origin/main from 2a2cdb51 to
  6bbef57e1ed443246587c9ffbad78f8e590d8498. Fresh fetch confirms HEAD equals
  origin/main and the source commit is an ancestor. Original dirty main untouched.
- Requested MP4 and compact evidence remain under
  E:/codex-data/artifacts/TypingHero/score-combo-clarity-20261028/;
  retain these for user review, remove when no longer needed. Native contexts,
  QA servers and Editor exited; task QA ports 54296/50364 have no listener.
- Removed only owned server results 20261028T021251-quick,
  20261028T021703-webgl and 20261028T022932-webgl after receipt preservation
  and snapshot/mount checks. Shared source/cache/toolchain/builds and other jobs
  remain untouched. Local task source/Library/player/profiles/helpers/captures
  were removed after preserving the requested clip and receipts.
- Remove the clean task worktree and its task-created local branch after remote
  verification of this documentation-only receipt. Retain remote branch
  micharel09/score-combo-clarity under repository policy; it may be deleted when
  comparison is no longer needed. No itch.io upload or workflow dispatch occurred.
