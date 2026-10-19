# Typing SFX + HUD Slayer + Endless release

## Approved scope and acceptance

Publish the complete dirty checkout to https://micharel09.itch.io/typing-hero,
game3857509. Preserve existing edits, GUIDs, scoring/input/combat/Firebase contracts.
Reuse [HUD receipt](combat-hud.md) and [Endless receipt](endless-duel.md).
Keep Slayer48/24px,4px gap,220ms transitions.

TypingInputSfx subscribes only to TypingManager.OnInputResult while enabled and
uses existing SfxPlayer/pool/mixer/preferences/transition gain. Kenney Interface
Sounds CC0 original tick_002.ogg/error_004.ogg only. Correct .16 gain,0 cooldown,
4voices; error .22 gain,.12s cooldown,1voice. Zero jitter/base pitch,2D,
ignoreListenerPause=false. No word/timeout sounds or input/scoring changes.
ShogunTheme owns idempotent binding/event generation; Unity serialized assets
must be synchronized before freezing candidate.

## Required gates

- Real ProcessInput regression: case/error/type-ahead/Slayer retyping/locks;
  binder lifecycle, source settings, limits, slider and pause policy. Final Quick
  must execute tests without skips. QA repeat-builder preserves prior references.
- Native Edge real keys/mouse, slow/10–15cps/errors/normal/Slayer x2/x3. Capture
  actual WebAudio destination signal with QA-only tap; music0, SFX0/intermediate/1,
  combat overlap, no clipping, typing quieter than combat. No speaker claim.
- Six Endless duels/Continue/round2/carry/boss heal/pause/death/restart/menu and
  reload audio; provider fixtures and external blocking, no real leaderboard writes.
  Final HUD640x360/1280x720; reuse four-size receipt if layout inputs unchanged.
- Freeze source/config/assets fingerprint; production from identical candidate,
  QA define/bridge absent in JS/WASM/data. Original HTML smoke in fresh profile.
- ZIP root index, full payload/credits, CRC/SHA256/per-file manifest, retained
  server releases archive. Verify owner/game and old playable upload, upload new
  ZIP, keep old uploads/store settings. Saved + public version/CDN manifest +
  anonymous menu/audio required. Roll back playable upload if public fails.
- Update affected game memory and this checkpoint, clean owned temporary resources.

## Checkpoint 2026-10-18 — implementation

Verified repository/root/Unity2022.3.25f1 and preserved pre-existing dirty HUD,
Endless and other checkout changes. No branch/worktree/commit created. Existing
receipts are evidence only for unchanged inputs, not this release's audio gates.
Added binder, builder event/binding configuration and packaged credits/license.
Downloaded original Kenney archive into
E:/CodexTemp/TypingHero/typing-sfx-release; retained only two original clips in
Assets/Audio/SFX/Clips/KenneyInterface. Temporary archive retained until verification.
Implementation and all release gates remain in progress; no publication yet.

### First implementation gate

- Quick20261018T160418:76/76 PASS, no skipped tests, input
  `2291f5ad5e6a24ac31a1ee6d6f62aff110f6ac19a650a6c0f0d04f73ccdd1c8e`.
  Includes real ProcessInput/binder lifecycle and pool limit/cooldown regression.
- QA20261018T160520 input
  `d516a72f34d6c5ba3914436e194819eab3cfda0f51c6433d95699aa132761c02`
  is building. Unity output synchronized to checkout: SampleScene adds exactly
  binder75867774 on existing GameManager75867769 and two event references;
  whitespace-normalized diff has no other delta. Existing scene IDs/references
  retained. Clip/event/meta serialization synchronized before final candidate.
- Added authored-event regression and scene reload single-subscription assertions
  after the first Quick; final Quick queued and remains unverified.
- Runner contention came from a separate parry-lighting checkout. Used temporary
  blocking-lock wrapper to queue safely; no other job/container interrupted.
- Orca page initially stopped at Cloudflare security verification, then vanished
  from tab list. No upload/owner/playable verification yet. User notified while
  independent implementation/QA continues. No production change has occurred.

### Candidate rejected — concurrent source migration

- Snapshot `23ebdb7bbc729eb9c2d927444b272fe23666cdd4c9feeff91bee2545abb93e4a`
  (1073inputs) and manifest are retained under the task temporary directory.
  Quick20261018T161133 executes78tests,75passed/3failed,0skipped. Failures:
  Endless actual-scene loop, role patterns and six-opponent test. Stack identifies
  EndlessRun.Definition/Enemy: concurrent work replaced the approved inline roster
  with Resources.Load while its required opponent assets were not in this snapshot.
  This is an inconsistent integration snapshot, not a typing SFX defect or PASS.
- **Do not release this candidate.** Its QA20261018T161229 build was stopped after
  confirming its owned container/result mount; no production build/upload was made.
  No concurrent source was reverted, overwritten or incorporated as an approved
  change. Other ongoing edits include Endless online provider/schema/UI changes,
  which this release plan does not authorize.
- Need a stable source baseline matching the approved Endless contract before
  final Quick, candidate QA and production. User asked asynchronously to coordinate
  the competing session and confirm the intended release baseline. Keep this gate
  open; do not infer approval from silence or substitute the new game rules.
- First QA20261018T160520 built/repeat-builder passed. Software browser stopped at
  intro after6checks; not a profile PASS. Its preserved player remains valid for
  investigating typing audio against the earlier approved Endless behavior, but
  it is not a substitute for a reproducible final source snapshot.
- Native audio-only run on that player:11checks passed; actual destination signal
  captured, stereo peak0.8942(no clipping), Music0/SFX0 window32255samples all zero.
  Intermediate/full windows contain combat and cannot establish isolated gain;
  controlled tutorial audio run and full native Slayer/Endless remain in progress.
  First harness attempts exposed slider key-repeat throttling, transient empty word
  during promotion, and lost QA tap connection after Unity reconnects its gain node.
  Fixed the temporary QA driver/tap only; no gameplay/timing thresholds changed.

### Audio findings and mute correction

- Native combined run74checks then FAIL at the global clipping check during
  Slayer capture: peak1.0988642 at14.04696s, preceding the first typing buffer
  at14.12063s. Actual typing/overlap window through16.42177s peaks.9178221,
  0clipped samples. Do not misattribute the pre-typing combat peak to new typing
  sounds, and do not call the whole audio acceptance PASS.
- Controlled tutorial single clips: tick peaks.0670586 at SFX.5 and.1341172 at1
  (exact1:2 gain); error.1878652. No enemy/word-completion sounds in those windows.
  A mute-after-pause window has exactly1nonzero sample(-.3772047), so strict mute
  still failed despite the earlier completely silent normal window.
- Unity framework/graph evidence: channel.configurePanningNodes reconnects its
  gain to destination for each buffer. First muted tick connects at recycled UI
  gain.45; its replacement connects at0. Temporary tap now reattaches on reconnect,
  retaining original playback route. Buffer replacements0–12ms apart are kept in
  diagnostics, rather than incorrectly equated with duplicated binder voices.
- Corrected TypingInputSfx to avoid creating a voice when AudioPreferences.Sfx=0;
  real ProcessInput regression verifies no new voice and restores preference.
  Focused GameplayTests/TypingInputSfxTests queued from current source. This changes
  the candidate; no final Quick/WebGL/production PASS may be reused for this fix.
- Full native six-duel/reload capture on the preserved earlier QA player is running
  separately. Its result can inform behavior/audio investigation, not certify the
  unbuilt mute correction or replace final candidate verification.

- Focused Quick20261018T162525 **33/33PASS,0skips**, input
  `377ded72b681b88407a43140e204f37899fdc1bb8f937f01fbcd3f0dc8ac4380`.
  GameplayTests and TypingInputSfxTests executed, including muted real input creating
  no voice, correct/error/type-ahead/Slayer retyping, lifecycle, limits, original
  clip/routing/gain/pause contract. Does not satisfy the required full Quick gate.

### Native earlier-player completion and handoff state

- Separate native Endless capture completed all6victories, mouse Continue, round2,
  boss heal30→40, preserved4014points/22parries, Slayer/pause, death/stat freeze,
  restart, fresh no-letter death/best retention and return menu.39behavior checks,
  250.562s,0fixture writes/JS or managed exceptions. **Overall FAIL** at global
  destination audio peak1.4233700, not a full profile PASS. Retained201.27s actual
  float capture and WAV; nearest typing start to max is117ms, so isolate combat
  and typing contributions before changing the approved authored gains.
- Inspected actual normal640x360/1280x720 and Slayer x3 frame; approved layout
  retained. All this applies only to earlier player160520, not the mute correction
  or the concurrently changed Endless source. Do not reuse as final-candidate QA.
- No production, release ZIP, upload or public change. Pending decisions/blocks:
  stable approved Endless baseline (exclude unapproved online/schema/rule work),
  final full Quick, WebGL verification of the mute fix and clipping investigation,
  then production/public gates from one immutable candidate. Cloudflare still
  blocks the default-profile itch manager before target/old-upload verification.
- Needed temporary evidence/source/player/scripts remain at
  `E:/CodexTemp/TypingHero/typing-sfx-release/` for resumption. Server owned receipts:
  `/data/typinghero-game-loop/results/20261018T160418-quick`, `20261018T160520-webgl`
  (earlier valid player), `20261018T161133-quick` (rejected integration),
  `20261018T161229-webgl` (stopped rejected build), `20261018T162525-quick`
  (focused mute regression). Keep until final source is agreed, evidence reconciled
  and release/rollback artifacts safely retained; remove only these owned results.
  Shared source/cache/build directories and other sessions' results must stay.
- Owned test processes have finished; rejected QA container was stopped. Retain
  Orca page `ccb3d70f-5444-4701-b209-03a5e965cd2f` at the itch manager for the user
  to finish Cloudflare. No credentials were exported and no real score was written.
- Removed the3owned native Edge profiles after all browser processes exited;
  retained only task evidence/player/scripts/snapshot inputs needed to resume.
  Final scoped whitespace review PASS; the scene still contains exactly1binder
  script reference. No commit/push/merge was made. Git has only main and the existing
  pixel-sparks worktree; its source does not contain the original untracked Endless
  implementation. Orca session search is unavailable(reason disabled), so the
  pre-migration source could not be recovered from that route. Source recovery/
  coordination remains pending; do not recreate the approved roster by guessing.
