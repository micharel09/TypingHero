# Duel opening presentation

Scope: Play -> arena/player -> opponent foot dust/reveal -> opponent name and
DUEL x/6 -> HUD/first word -> combat. No character sprite/clip, camera, balance,
provider, saved data or audio-asset changes. Existing cue/voice lifecycle is corrected for continuity. No skip/VS/READY/FIGHT/additional Enter.

## Accepted timing

Full Play: menu/cover 250ms before scene transition; after activation reveal450ms,
settle until850ms, foot dust850-1350ms with opponent reveal halfway, title1350-2000ms,
status HUD1850-2000ms, word tray1930-2100ms, readable stationary preview2100-2400ms.
Title exits1850-2000ms. Combat remains locked through the handoff frame.
Continue/retry: cover200ms before loading, reveal200ms, dust200-600ms,
title enters460-540ms, holds until750ms, exits750-900ms; status HUD540-640ms,
word tray620-700ms, stationary preview700-1000ms (1200ms plus scene load in total).
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

## Concurrent main integration

Fetched origin/main advanced from7dc84f9 to669df07 during finalization. Its Slayer
contact-first VFX, poses, shader, fixtures and capture-only receipt are retained.
The sole manual conflict was adjacent verification receipts; both are kept. The
probe auto-merge keeps both sets of read-only diagnostics. Native58 checks and the
509/504 visual review apply to player05832092f73446d6 before this merge, not a
recapture of the newer Slayer visuals. The intro implementation is unchanged.
Targeted merged-source intro/music/session/tutorial tests are recorded separately
in merged-consumers.xml; final outcome is appended below before integration.

Final main integration also retained5085b32 and its authored combo recovery/lighting
asset updates. Merged-source run merged-consumers.xml passed10/11: the focus test
assumed Resume's camera tween completed in300ms. The fixture now waits (max3s)
for the actual unpaused/progressing state, preserving both assertions. A subsequent
-nographics run stalled entering Play Mode and was terminated without result XML;
it is not a pass. Final D3D11 merged-d3d.xml:11/11 PASS, including all four intro,
two music, one shared-session and four tutorial cases. No runtime change was needed.
The other six scoped cases retain their earlier recorded evidence, not a claim of
17 cases in the final invocation. Source/doc links and git diff --check pass.

## Restrained motion polish ? accepted follow-up (2026-10-25)

User feedback: correct sequence feels rough/basic. Chosen direction: smooth and
restrained, same duration, no new audio, camera movement or actor sprite/clip edits.
Implementation: smoothstep cover/menu/reveal; private three-cluster foot dust;
120ms pixel-aligned world-height reveal1.04-1.16s/full,.34-.46s/short; default-off
per-renderer shader clip shared by forward/depth/shadow. Unclipped child shadow
waits until body completion. Existing property blocks retain lighting/hit values.
Name fades and moves6reference pixels in/out; existing HUD groups fade before
word tray by80ms. Same lock/handoff and pause/navigation lifecycle.

Acceptance checklist: full Play and short Continue/retry; partial reveal vs shadow;
no early HUD/word/cue; title pixel stability; pause/resume at spawn; focus loss;
Menu cancellation/restored property blocks; normal combat after handoff; tutorial
reading unchanged. Reuse existing Shogun research scope, without a new match claim.
New output evidence is distinct from the earlier accepted capture above.

fly-server doctor with ConnectTimeout8 failed SSH timeout; use installed pinned
Unity2022.3.25f1 locally, isolated task source/build under
C:/Users/ADMIN/AppData/Local/Temp/TypingHero-duel-intro-polish-20261025/.
Do not count unexecuted or failed checks as passes. Verification pending.

### Follow-up implementation and pilot findings

Initial Unity intro-tests.xml:3/8 passed; five failures exposed MaterialPropertyBlock
construction in a MonoBehaviour field initializer. Moved allocation to canvas
initialization. polish-tests.log then caught a misplaced test assertion before
execution; corrected it without changing product behavior. polish-final.xml:
10/10 PASS (five intro cases, four tutorial, one Slayer property-block consumer).

First WebGL attempt failed protected font byte equality: CobblestoneUI64 text was
identical after CRLF/LF normalization. Normalized only isolated YAML/meta inputs;
no repository asset/guard changes. The next attempt began from partial generated
state and failed scene idempotence; first/second scene diffs are retained. Restoring
pristine task inputs before build passed the strict repeated builder and produced
pilot player867010b35d382014. Native pilot39 checks PASS, zero exceptions/provider
reads/writes. All124 pilot frames/123 consecutive pairs were directly inspected on
seven sequential sheets, with original25fps decoder PTS/repeated frames retained.
Pilot visual acceptance FAILED: overlapping large dust quads formed a slab; broad
renderer bounds included invisible padding/other renderers and compressed the body
reveal into too few visible steps. No visual pass is claimed for that player.

Correction:18 small irregular quads in three clusters, staggered per-particle
lifetimes/heights and lower opacity; body-collider foot/head landmarks define the
120ms reveal. No change to the total timing or shader pass contract. Targeted
reveal-final.xml:2/2 PASS for preactivation lock/first attack and property restoration.
The final build/capture/review outcome is appended after actual verification.

### Final polish acceptance

Playerae7ca2579d0b544e passed65native assertions but its complete562frame/557pair
review exposed the intro curtain50 occluding Pause40 during early focus loss.
That is a visual FAIL despite the passing input driver. Lowered only the intro
canvas to39; the focus test now checks the actual authored layer relation.

Two driver failures are separate from product acceptance: startup-attempt.json
clicked before MainMenuUI.Start finished shared-world loading/EventSystem enable;
partial-observation-attempt.json polled a120ms interval through100ms QA snapshots
and missed it. The latter waited into a fixture-only game-over (one intercepted
read/write; no real provider traffic). Startup synchronization and timestamp-based
real Escape scheduling corrected the driver, without gameplay setters.

Final build-pause-layer-result.json exits0 with index.html; strict repeated builder
passes. player-manifest.json hashes33files. source-comparison.json verifies253
C#/shader/package/version inputs against the task worktree (BOM/CRLF normalized).
Final player46e41cff3648399e, Edge156.0.4314.42, Intel UHD/ANGLE D3D11,1280x720:
accepted.json61assertions PASS, zero exceptions/provider reads/writes. Counts vary
with repeated intro polling; the same complete scenario/explicit checks ran.
Unity latest passing results per10distinct cases are in unity-results.json; the
final two reveal/lock and one Pause/focus cases were rechecked after their changes.
No combined13-case invocation or new runtime behavior beyond this follow-up claimed.

Final output scopes below were declared before review, with all frames and every
adjacent pair directly inspected on31sequential sheets (page overlaps included).
Original25fps decoder PTS and held/repeated frames retained; encoded-frame visits
are not engine-frame/FPS measurements. Intentional overlap between scopes remains.

| Scope | Encoded first-last PTS | Frames inspected/total | Pairs inspected/total |
|---|---|---:|---:|
| full-play | 13.12-18.16s | 127/127 | 126/126 |
| short-continue | 26.56-30.36s | 96/96 | 95/95 |
| short-pause-resume | 17.40-21.64s | 107/107 | 106/106 |
| cancel-intro | 34.92-38.96s | 102/102 | 101/101 |
| focus-resume | 38.40-43.96s | 140/140 | 139/139 |

Totals:572frame visits/567pair visits. Per-frame ledger and observations are in
runtime-review/. Full Pause menu now remains visible over the frozen partial cover;
Resume continues at the held milestone. Enemy emerges in several visible steps,
small low dust has no slab, title is stable, HUD leads word tray, no intro hit flash,
shake or early cue/word. Manual Pause, combat cue and actual hit flashes in the
preserved test tails occur after handoff and are expected. Cancel keeps the partial
body until the cover closes, then leaves a clean menu. Same world10752/camera11162/
RT-458 across all observed navigation. No runtime changes after final native capture.

Delivery-only excerpts (subsets of the broader fully reviewed scopes):
E:/CodexTemp/TypingHero/duel-intro-polish/intro-play.mp4,86frames, original14.00<=t<17.44;
intro-continue.mp4,47frames, original27.68<=t<29.56. Both preserve decoded relative
PTS/counts, without interpolation/resampling; video-only. Full review scopes/tails
and source video remain at the isolated task root. No sprite/clip/prefab/font,
package, camera framing, balance, provider/save contract edits or itch.io release.
Integration/cleanup receipt is recorded externally after remote verification.

## First-word preparation follow-up (2026-10-26; verified)

User chose to retain the total2.4s/1.2s timing. The last300ms now show the prepared
first word at full authored opacity, using the existing next-word ink. At combat
handoff it returns to the authored active ink over100ms. No countdown, extra wait,
audio, geometry or word reinitialization. Accepted letters immediately supersede
the cue with existing correct/error feedback. Intro/pause gates remain authoritative;
Cancel clears preview without signaling combat. Tutorial does not enter preview.
Implementation and pending focused checks are on micharel09/duel-word-preview,
based on origin/main24f74e0; all Unity execution is on Ubuntu fly-server.

Reference evidence remains separate from product output. Predeclared scope and
per-frame PTS ledger: E:/CodexTemp/TypingHero/duel-word-preview/reference-review.json
(original research cache: Clackblade-start-cue-research-20261025 under local Temp).
Type Knight official Steam trailer5<=t<16s:660/660frames,659/659adjacent transitions
directly inspected sequentially; official screenshot and full-resolution7s still
also viewed. Words remain above approaching enemies; trailer is not a complete
menu/start flow and cannot prove engine input-enable timing. Small contact-sheet
glyph details do not establish a measured reading interval.
Source: https://store.steampowered.com/app/1006220/Type_Knight/ . Developer confirms
a word-appearance audio cue at https://itch.io/devlog/108069/type-knight-version-1001.amp .
Nidhogg2 source https://www.youtube.com/watch?v=36UDLPPD_U0 ,38<=t<50s:
720/720frames,719/719adjacent transitions directly inspected, including all page
overlaps. Customization, stage selection, covered transition, READY during arena
reveal, GO at44.983s, first exchange and continuation; source38s/44s stills viewed.
No numeric3-2-1 observed; no engine input-enable or retry variant claim.
osu! is a documentation-only comparator: https://osu.ppy.sh/wiki/en/Beatmap/Countdown
describes countdown as optional/useful for abrupt openings. No osu motion review.
The300ms preview is a CLACKBLADE design choice, not a duration copied from references.

Ubuntu Unity2022.3.25f1 scoped `quick --test-filter DuelIntroTests`:6/6PASS,
0failed/skipped, `/data/typinghero-game-loop/results/20261026T061302-quick`.
Input snapshot f255f209b8ead2dce840bf59fea704de5bb3339e2c0b0a69ce394442438e3235.
Shared runner was then occupied by another task; its source was preserved. The
build-only helper extracts the exact same snapshot to the independent Ubuntu
`/data/typinghero-game-loop/word-preview-20261026/source`, takes the shared Library
lock, and builds an immutable QA player. No Windows Unity invocation. Build/runtime
and visual acceptance remain pending; local evidence root is
E:/CodexTemp/TypingHero/duel-word-preview/.

First build failed font byte guard: CobblestoneUI64 differs only byCRLF/LF
(normalized equality verified). Fresh isolated serialized inputs normalized toLF
passed the unchanged strict repeat builder. Ubuntu build062625/player
eea80fc084776dbd passed62native Edge156/NVIDIA D3D11 assertions,0exceptions or
provider IO. Visual FAIL: all445frames/441pairs in the first four declared scopes
were viewed, revealing the live opponent label above the closed Menu navigation
cover at45.60-45.72s. Fifth focus scope unreviewed; no complete visual pass claimed.
Pre-cover-fix evidence retained separately. Correction brings the existing curtain
to the final sibling when Cover begins; Pause stays above the whole intro canvas.
Focused cancellation test now begins during readable preview and checks that the
cover occludes the heading. Final source7b86815e04628556532d9c7b077c8dd3bbcc0b84ddf6e2dcf6f59935564ea6bf
was retested and rebuilt on Ubuntu; final acceptance below supersedes pending notes.

### Final preparation acceptance

Ubuntu2022.3.25f1 final source:6/6DuelIntroTests PASS,0failed/skipped; strict repeat
builder and WebGL QA build PASS at word-preview-20261026/20261026T064955-build.
All Unity processes ran on Ubuntu; no Windows Unity invocation. Final281C#/shader/
package/version inputs match the worktree afterBOM/CRLF normalization. Final
player7a03a2f20df3f514:65native real-input assertions PASS,0exceptions/provider IO,
Edge156.0.4314.42/NVIDIA GTX1650Ti/ANGLE D3D11 at1280x720. Original prepared word,
full timer and statistics preserved until handoff; preview uses authored secondary
ink and returns to active ink; held Play/Continue Enter, letters/Space, retry,
pause/resume, real focus loss, Menu cancellation and tutorial reading covered.

All31sequential sheets below directly inspected, every frame and adjacent pair
including page overlaps. Original25fps decoder PTS/held frames retained. Counts
are encoded-frame visits, not engine FPS. Per-frame ledger and scope observations
are in runtime-review/manifest.json; source research remains separate above.

| Scope | Encoded first-last PTS | Frames inspected/total | Pairs inspected/total |
|---|---|---:|---:|
| full-play | 12.96-17.84s | 123/123 | 122/122 |
| short-continue | 27.72-31.36s | 92/92 | 91/91 |
| short-pause-resume | 17.16-20.96s | 96/96 | 95/95 |
| cancel-intro | 36.40-40.80s | 111/111 | 110/110 |
| focus-resume | 40.64-46.36s | 144/144 | 143/143 |

Total566frame visits/561pair visits. Full-size readable preview and closed
cancellation cover also viewed. Word geometry stays fixed; timer starts after
the preview; no early shadow/cue/word, extra confirmation, heading leak on closed
cover or activation flash during cancellation. Actual Pause overlays and combat
effects in preserved tails are expected. Existing `F6 Spark` OnGUI is conditional
on TYPINGHERO_AUTOMATION; it is not a product HUD leak.

Delivery excerpts of the broader fully reviewed scopes, with original relative
PTS/counts and no interpolation/resampling: intro-play.mp4,84frames,
13.76<=t<17.12; intro-continue.mp4,42frames,28.96<=t<30.64. Video-only. Source/output
hashes, tests and failed draft remain at E:/CodexTemp/TypingHero/duel-word-preview/.
No production build, itch.io publication, package/art/font/sprite changes or full
suite/performance claim. Integration and task-resource cleanup recorded externally.
