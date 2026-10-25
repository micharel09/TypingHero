# Duel opening presentation

Scope: Play -> arena/player -> opponent foot dust/reveal -> opponent name and
DUEL x/6 -> HUD/first word -> combat. No character sprite/clip, camera, balance,
provider, saved data or audio-asset changes. Existing cue/voice lifecycle is corrected for continuity. No skip/VS/READY/FIGHT/additional Enter.

## Accepted timing

Full Play: menu/cover 250ms before scene transition; after activation reveal450ms,
settle until850ms, foot dust850-1350ms with opponent reveal halfway, title1350-2000ms,
HUD/title handoff2000-2400ms. Combat remains locked through the handoff frame.
Continue/retry: cover200ms before loading, reveal200ms, dust200-600ms,
title600-800ms, HUD800-1000ms (1200ms plus scene load in total).
Tutorial only uses the shared reveal; its reading/lesson state remains owned by
TutorialDirector. Pause/focus loss hold the timeline; navigation cancels it.

## Implementation

DungeonSession owns DuelIntroPresentation, screen cover and cancellation. The
same DungeonWorld/camera/render texture are retained. Intro lock is set before
activating the gameplay gate. GamePauseController separates gameplay blocking
from presentation freeze/timeScale/audio. Renderer.forceRenderingOff hides enemy
and child shadow without disabling configuration/animator. Existing result HUD
CanvasGroups hide the word/tray/cue/status/score. The opponent heading clones the
authored tutorial heading style. The spawn variant uses StrikeDust's material and
particle renderer settings with private manually aged low puffs; strike prefab
and pool remain unchanged. No hit flash/light/shake/audio is emitted.

TypingManager.ProcessInput remains the letter input path. Intro input is dropped;
keys held across handoff must be released before typing/parry can consume them.
First enemy attack is scheduled500ms after the combat gate opens. Statistics,
typing timer and scoring use the existing shared gameplay block.

## Reference evidence (separate from output verification)

Reused existing captures, not downloaded new game artifacts:
E:/codex-data/artifacts/duel-entry-research-20261025/.
Source: https://www.youtube.com/watch?v=i6tP6bBTaZ0 (MasakoTeam,2024-09-06).
Predeclared segments in existing frame index files:

- 0-18s: menu upward exit, covered interval, arena reveal/player, tutorial movement.
  Directly inspected all540frames/539pairs on sheets000-004, including overlap.
- 94-108s: preparation/instruction, enemy spawn dust, exchange and defeated enemy.
  Directly inspected all420frames/419pairs on sheets000-003, including overlap.
- Directly inspected shogun-spawn.png and every image in the sequential sheets.

Original30fps PTS and held/repeated frames retained:960frames/958pairs reviewed.
Coverage: menu exit/arena staging/player/spawn/first exchange. Opponent title and
1200/2400ms timings are the user's CLACKBLADE design, not measured Shogun behavior.
Reference retry/actual input-unlock/audio are unverified and are not used as claims
of an exact game match. No sprite animation was authored/edited/reviewed in scope.

## Verification checkpoint

Targeted Unity: preactivation lock; no HP/timer/score/letters/stat time changes;
one handoff; enemy first attack delay; pause/focus hold; retry/menu cancellation;
shared world; tutorial reveal/reading ownership. Existing affected session/pause/
tutorial tests should also run. Real input: mouse Play/held Enter, letters/Space
inside intro; Continue/retry; pause/resume/focus; menu during intro. Native1280x720
video of full/short flow, every encoded frame and adjacent transition reviewed.

2026-10-25: origin/main7dc84f9; isolated Orca worktree micharel09/duel-intro.
fly-server SSH100.115.189.84:22 timeout; no server tests ran. Existing local
Unity2022.3.25f1 used with independent source/Library:
E:/CodexTemp/TypingHero/duel-intro/. No install/toolchain or unrelated Unity changes.
Tests and rendered acceptance are pending; no completion/integration claim yet.

## Scoped results before final-player review

- First intro run:3/4pass. The duration assertion exposed stale frame delta from
  scene initialization; fixed by starting a monotonic presentation clock after
  startup/configuration. Full opening then passes independently.
- Combined scoped run:13/14pass. Existing DungeonSession lifecycle fixture tried
  camera impact during the new deliberate handoff frame. It now waits both handoff
  frames, matching DungeonSceneTestFlow. Its old post-transition zero-stat assertion
  is replaced by ready-gate assertion; DuelIntroTests retains stronger continuous
  zero HP/timer/score/letters/combat-time-change assertions throughout the intro.
- session-retry.xml:1/1pass,19.277s; covers three complete shared-world cycles.
- music-retry.xml:2/2pass (full opening including every child renderer plus actual
  scheduled AudioSource voice/playhead continuity and menu-stopped restart).
  Together15distinct cases pass; not a full-suite claim. Test target is
  StandaloneWindows64/Editor. An attempted post-WebGL PlayMode run was stopped
  without XML/executed-case evidence; active Editor target was restored for retry.
- First native player7579764be242aea8: Edge156/Intel UHD/D3D11/1280x720.
  Draft input reached42checks across mouse/held Enter, letters/Space, retry,
  pause/resume, actual duel clear/Continue and mid-intro menu cancellation. Its
  final focus step timed out because Playwright forces focus emulation; a real
  timeout death generated one fixture read/write (external network stayed blocked).
  This draft is not an overall pass or music-continuity acceptance.
- Local Playwright crPage.js explicitly sends Emulation.setFocusEmulationEnabled
  enabled:true. A separate pilot disables that setting, switches real browser tabs,
  observes document.hasFocus=false and Unity paused at intro elapsed0.17307,
  timeScale0/audioPaused=true, then resumes the same intro:7/7checks pass,
  zero exceptions or provider requests. Final driver incorporates this environment
  correction; it does not inject gameplay state or dispatch synthetic focus events.
- First native stills directly inspected: clean player-only arena, opponent reveal,
  compact title/DUEL1/6 and existing HUD handoff. Complete final-frame review pending.
- MusicOnSceneStart previously restarted the same cue each scene. Its new internal
  live-cue check preserves an active voice and restarts normally after Menu stops it.
  Final player build is pending; gameplay/art/world/camera code otherwise unchanged.

## Live audio ownership correction

The second player ce609bad1f67d34a built successfully but its native music assertion
failed:MusicPlayer ID/voice ID/sample all0 across every scene. No runtime exceptions
or provider calls. This is a real acceptance failure, not a passing silent baseline.
Source inspection found the menu's persistent SfxPlayer kills the whole shared
Audio_Services GameObject when its gameplay SfxPlayer duplicate awakens, including
its MusicPlayer and MusicOnSlayerMode sibling. Both singleton duplicate branches now
remove only themselves when the other service shares their host; standalone
single-service duplicate hosts retain the existing whole-host cleanup. Static
singleton identity and pool/configuration remain unchanged.

audio-services.xml:4/4pass, including the strengthened full intro, live same-cue
voice/sample continuity and stopped restart, shared-host sibling/binder preservation
across reload, and existing UI-audio pause/reuse regression. Together17distinct
scoped Unity cases pass. Final integrated player and full frame review pending.

## Final integrated acceptance

Unity2022.3.25f1 integrated QA build PASS, including strict repeated authoring.
Player05832092f73446d6;252C#/package/version inputs match the verified worktree
(CRLF/BOM-only normalization),33immutable player file hashes recorded.
Native Edge156.0.4314.42/Intel UHD/ANGLE D3D11/1280x720:58checks PASS,
zero exceptions/provider reads/provider writes; fixture interception/external-network
blocking retained. Real keyboard/mouse covers tutorial, mouse Play/held Enter,
held letters/Space and no buffered first-word input, retry, live spawn pause/resume,
actual duel clear and held Continue, intro cancellation to Menu, actual focus loss
and resume. Same world10728/camera11138/RT-458 survives all observed transitions.
Music player15296/voice-688 persists: samples132978->204755 across retry and
495461->567114 across Continue. No direct speaker-listening claim.

Full output scopes are declared in E:/CodexTemp/TypingHero/duel-intro/runtime-review/manifest.json.
Every frame and consecutive pair was directly inspected on28sequential sheets,
with prior-frame page overlap; original decoder PTS and held/repeated frames retained.
These are encoded video frames, not a1:1 engine-frame trace or performance claim.
The Continue scope was extended earlier after locating video/wall-clock offset,
so the complete result-card preparation is included; no prior scope or tail removed.

| Scope | Original video interval | Frames inspected/total | Pairs inspected/total |
|---|---|---:|---:|
| Full Play and subsequent test tail |14.80-19.26s|112/112|111/111|
| Previous ending/result -> short Continue and tail |27.00-30.94s|99/99|98/98|
| Short retry with spawn Pause/Resume and combat tail |18.80-22.45s|92/92|91/91|
| Retry -> paused intro -> Menu cancellation/fresh menu |35.54-38.75s|80/80|79/79|
| Full Play -> actual focus loss -> resume/handoff and tail |38.93-43.99s|126/126|125/125|

Totals are509frame visits/504pair visits across scopes (overlaps are intentional).
No observed early enemy/shadow/cue/word reveal, stale title/dust/cover after Menu,
spawn hit flash or camera shake. Original character sprites/clips are unchanged.
Pause at short elapsed0.402 occurs after the midpoint enemy reveal; the visible
opponent behind Pause is expected, with held dust/clock. Focus Pause occurs before
spawn and keeps the opponent hidden until resume reaches its normal milestone.

Delivery excerpts intro-play.mp4(91frames) and intro-continue.mp4(46frames) are
presentation-only subsets of the broader fully reviewed scopes. Their decoded
frame counts and relative PTS match the original capture, without resampling;
review scopes/tails and per-frame ledger remain intact. No itch.io publication,
full-suite, broad performance, stress/soak or exact Shogun-copy claim.

After the shared audio-host correction, scene ownership/tutorial consumers passed
5/5 targeted PlayMode cases (audio-scene-consumers.xml). All 17 distinct scoped
cases pass across the recorded runs. Integration status is recorded externally in
RECEIPT.md after remote verification. No runtime changes after final player QA.
