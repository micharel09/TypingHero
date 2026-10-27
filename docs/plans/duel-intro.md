# Duel opening presentation

Scope: Play -> arena/player -> opponent foot dust/reveal -> opponent name and
DUEL x/6 -> HUD/first word -> combat. No character sprite/clip, camera, balance,
provider, saved data or audio-asset changes. Existing cue/voice lifecycle is corrected for continuity. No skip/VS/READY/FIGHT/additional Enter.

## Accepted timing

Full Play: menu/cover250ms before loading. After activation: reveal0-450ms,
settle to850ms, smoke850-1350ms, intact enemy/shadow1030ms; title enters1100-1200ms,
holds to1720ms and exits1720-1880ms. Status HUD1820-1980ms; words1900-2100ms;
stationary readable preview2100-2400ms. HUD enemy name fades1880-1960ms only after
the intro label has fully faded. Combat remains locked through the handoff frame.
Continue/retry: cover200ms before loading; reveal0-200ms; smoke200-600ms, intact
reveal344ms; title enters420-500ms, holds to680ms, exits680-820ms. Status540-640ms,
words620-700ms, preview700-1000ms; HUD name820-900ms. Total1200ms plus scene load.
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


## Smoke anticipation and identity handoff (2026-10-26; verification in progress)

Task branch micharel09/intro-smoke-rhythm from origin/main4506bb1. Preserve
Play2.4s and short1.2s including200ms pre-load cover; no Windows Unity or itch
publication. Reference checklist: menu exit/covered arena entry; smoke anticipation,
whole enemy/shadow reveal, dissipation; opponent label/status/word handoff; full
and short variants; preview/input lock; Pause/focus/Resume and Menu/Restart cleanup.

Implementation replaces the private18quad spawn with18stepped elliptical mesh
puffs (6core/12edge), fixed lobe positions, collider-relative scale and existing
StrikeAccent material. Shared shader/prefab remain untouched. Smoke peaks at
u.28-.48; renderer states restore together once at u.36, without any property-block
writes. Name30/DUEL18 stays stationary in700x72; full title1.10-1.88s, short.42-.82s.
Enemy HUD name stays hidden until title ends, then fades80ms independently from
HP/guard/score. Status full1.82-1.98/short.54-.64; words full1.90-2.10/short.62-.70.
Existing300ms readable preview and100ms ink handoff remain unchanged.

Reference records reused and all9sequential original sheets directly reinspected:
Shogun https://www.youtube.com/watch?v=i6tP6bBTaZ0, menu0<=t<18 (540/540frames,
539/539pairs), preparation/spawn/first exchange/ending94<=t<108 (420/420frames,
419/419pairs). Native96.8/96.9stills and baselineb68b02015.28still viewed.
No reference retry/input-enable timing or exact visual-copy claim. Sources/checklist
and new output evidence live at E:/CodexTemp/TypingHero/intro-smoke-rhythm/;
reference evidence remains separate from new output review.

Ubuntu Unity2022.3.25f1 focused6/6DuelIntroTests PASS,0failed/skipped,17.494s,
input4363b1fda1bb9d22087e7162a6614099254f29c0255c156fa7cdeb67e5142b88.
Isolated server root /data/typinghero-game-loop/intro-smoke-rhythm-20261026/,
result20261026T081303-build. Native build/capture, frame review and integration
remain pending; these test results alone do not establish visual acceptance.


Pilot44ceaa7f4bf22ee8:33native real-input checks PASS,0exceptions/provider IO.
All129pilot frames/128pairs directly inspected on7sequential overlapping sheets;
smoke clearly anticipates intact enemy and has stepped lobes, then dissipates low.
Pilot mesh world alignment is superseded by final camera-plane alignment; it is
not the final acceptance player. Scope and observations: pilot-review/manifest.json.

Expanded Pause fixture initially failed: alpha was read before presentation
LateUpdate and compared against the committed end-of-frame alpha. It now observes
after the pause frame's LateUpdate; assertions/thresholds are retained. Final
Ubuntu result20261026T082603-build:6/6DuelIntroTests PASS, including freezes at
smoke rise, intact reveal and identity handoff. Snapshot
64d60443caa66e3e61281ce316f6e2bcc78f68b8ae2ce7175b12cbf9c509a6b2;
284C#/shader/package/version inputs match the worktree afterBOM/CRLF normalization.
Final WebGL/native and visual acceptance pending. Earlier lock-wait timeout
preserved running jobs and the isolated final source; it is not a test pass.


Playere31eaef7358e2aa6:57native real-input checks PASS,0exceptions/provider IO;
33player files match Ubuntu output. All601frames/596pairs in32sequential sheets
were directly inspected across the five declared output scopes. Visual FAIL:
Menu cancellation re-shows the hidden paused title at43.56-43.96s, before the
closing curtain reaches it. This does not weaken the no-heading-flash criterion.
The complete failed review/ledger is retained at pre-navigation-fix-review/.

Correction keeps the heading disabled while Pause navigates and throughout Cover,
then resets that suppression only in Prepare for a new intro. No timeline, alpha,
input, camera or audio behavior changes. The existing cancellation test now uses
the real pause-owner Menu path and asserts no heading resurrection before cover;
the observation-only QA bridge reports actual label visibility for native checks.
Final rebuild and affected visual review pending.

Driver failures remain separate from product acceptance: original other-tab focus
was not observed; the narrow retry-preview polling window was missed; a generic
focus click hit Menu Play. The final57-check driver schedules real Escape from
early intro timestamps and prepares a separate native window before timed focus.
Diagnostics confirm document.hasFocus true -> false without gameplay setters.
Each failed report/video is retained; one earlier timeout produced fixture-only
leaderboard traffic (no production requests/writes). No product behavior changed
for these driver corrections.


Final handoff boundary audit: the old loop could exit after its last yielded frame
even if Pause/navigation began during that yield. It now retains the final frame
while PresentationFrozen or the pause owner is navigating. A LateUpdate fixture
pauses exactly at2.4s and asserts preview/intro remain active until Resume. Seven
scoped intro tests are selected; no broader profile is added. Snapshot
767edb7fc019d162189ca3dbf27e86a7658ed17ae77e682c6faecac7af06844d,
server source-handoff-final. The superseded waiting worker (only this task's PID,
verified by source path and exact snapshot in its command) was stopped before
Unity began; source-cancel-final is retained. Other jobs/locks/cache were preserved.


The shared runner remained occupied by other work. The task switched to its own
Library/cache and runner lock under /data/typinghero-game-loop/intro-smoke-rhythm-20261026/,
keeping the same pinned image, shared activation HOME,2CPU/8GiB, source and7-case
filter. Own waiting PID674845 was verified by source path/exact snapshot and
terminated before execution; other jobs and shared caches were preserved. Private
fresh-cache run20261026T090134-build is active; this is not yet a passing receipt.


Private cold-cache090134 timed out after1200s in import with no result XML; no
executed-test PASS is inferred. The same cache resumed at092416:7cases executed,
6PASS/1FAIL,29.848s. Both new navigation/last-frame handoff regressions PASS. The
old cue Resume test's fixed400ms wait left14.3ms of its100ms cue after the camera
Resume tween; it now waits (max3s) for actual resumed/cue-complete behavior, keeping
the zero-remaining assertion. Only this fixture changed after runtime767edb7.
Final source5b897298ce34316af5a533e34f2e91ca7e20e3064aed43d50d9e3bc362cd7141,
source-handoff-verified, private warm run20261026T093228-build is in progress.


Final private-cache Ubuntu2022.3.25f1 source5b897298:7/7DuelIntroTests PASS,
0failed/skipped,40.336s, result20261026T093228-build/tests.xml. Exact284C#/shader/
package/version inputs match afterBOM/CRLF normalization. No runtime changes after
these checks; final WebGL/native and visual acceptance remain pending below.


### Final smoke/identity acceptance

Final private-cache Ubuntu build093228 exits0, strict repeat-builder PASS;
player10a1876ff58157a5,33/33downloaded file hashes match Ubuntu. Source5b897298
matches284C#/shader/package/version inputs. Unity7/7PASS; native83checks PASS,
0exceptions/provider reads/writes, Edge156.0.4314.42/Playwright1.64.1/GTX1650Ti
ANGLE D3D11 at1280x720. No runtime changes after the passing tests/build/capture.
Actual focus diagnostics report document.hasFocus true -> false in a separate
native window; Pause and Resume hold/continue the same milestone. World10762,
camera11172 and RT-458 persist across the observed navigation.

All31sequential sheets were directly inspected, every frame and adjacent pair,
including page overlap. Original25fps decoder PTS/held/repeated frames remain;
these are encoded-frame visits, not an engine-frame/FPS measurement. New counts
happen to equal the old first-word follow-up totals; this is a distinct video,
source/player/hash and per-frame ledger at runtime-review/manifest.json.

| Scope | Original first-last PTS | Frames inspected/total | Pairs inspected/total |
|---|---|---:|---:|
| Full Play and handoff/input tail |14.36-19.48s|129/129|128/128|
| Previous ending/result -> Continue and tail |28.40-32.00s|91/91|90/90|
| Retry, preview Pause/Resume and combat tail |18.80-22.64s|97/97|96/96|
| Retry -> preview Pause -> Menu cancellation |36.72-40.96s|107/107|106/106|
| Full Play -> actual focus loss -> Resume/handoff |41.12-46.76s|142/142|141/141|

Total566frames/561pairs. Native-size pre/post intact reveal, readable preview and
post-Pause navigation were also viewed. The title stays absent through the Menu
exit tween/closing cover; no resurrection or closed-cover leak remains. Smoke is
clearly visible before the enemy, has stepped lobes rather than a rectangular slab,
peaks below half-body and dissipates low without touching the hero. Pose/shadow
switch together, no shader sweep; label/HUD identity never compete. First word
stays readable/fixed for300ms with a full timer until handoff. Existing SPACE cues,
real combat hits/death poses and Pause zooms in preserved tails are expected.

A/B: b68b020's dust was difficult to distinguish on the floor and the body scanned
upward. New smoke anticipates intact emergence and starts the title while fading;
short HUD bars appear without duplicate identity. This follows the selected Shogun
order/emphasis qualitatively; custom label/timing/preview remain CLACKBLADE design.
Reference9-sheet960/958review stays in reference-review.json, separately from output.
No exact game-copy, reference retry/input-enable timing or human-reading claim.

Delivery-only excerpts are subsets of these complete reviewed scopes, without
resampling/interpolation and with original relative PTS/counts checked:
intro-play.mp4,14.88<=t<17.96,77frames; intro-continue.mp4,29.28<=t<30.76,37frames.
Video only. Failed player/review, cold-import and fixture failures remain separate.
All evidence stays at E:/CodexTemp/TypingHero/intro-smoke-rhythm/. No itch release.
Integration, cleanup and retained-resource receipt is written externally after
remote verification; shared source/cache and other tasks are preserved.


### Actual 300ms preview under frame stalls (2026-10-26; verified)

Task branch `micharel09/intro-preview-300ms`, base `480ad7039d0379d2194a6750f5dba938aa58fa24`.
Reading time is independent of timeline Elapsed. The first LateUpdate applying
full tray alpha anchors the clock without crediting that frame's preceding stall.
Later visible intervals count, including stalls; freeze changes invalidate the
anchor and preserve accumulated reading time. Handoff waits for timeline end and
300ms reading time. Prepare/Finish/cancel reset the clock; tutorial is excluded.
Smooth Play stays about2.4s and short flow about1.2s plus scene loading, with frame
quantization. Late appearance extends only the final preview. ProcessInput, held
input barriers,100ms color cue and first attack500ms after handoff stay unchanged.

Acceptance: full/short pre-alpha stalls both below and beyond timeline end; visible
post-alpha stall; pause/focus preserving reading credit; pause at final boundary;
Menu/Restart during extra preview; HP/score/timer/word/input/first attack and tutorial.
Unity runner initially busy; queued filtered DuelIntroTests and focused QA build.
No passing evidence, native input or full frame review claimed at this checkpoint.
No itch publication. Final clips/review will be retained outside task temp.

Filtered Ubuntu Unity2022.3.25f1 run20261026T145850-quick:10/10PASS,0fail/skip,
76.6094381s. Snapshot90a0a643d2c7973ebf7e292d4555d5446057f031c7ba3ccfa801054ace22d48e.
This includes180ms and500ms real thread stalls before full alpha for both Play and
retry; timestamps originate after actual presentation LateUpdate. The old220ms
threshold is removed. Focus holds reading credit; a350ms already-visible stall
counts without restarting it. Menu/Restart during extra preview, final-frame Pause,
HP/score/timer/word/held-input path, first-attack scheduling and tutorial pass.
QA build queued after these results; native input/full-frame review remain pending.

Shared QA wait expired after900s while unrelated queued jobs retained the runner.
Only this task's queued QA processes were stopped after checking exact command
arguments and absence of child processes. Build moved to private source/Library/
cache/lock at `/data/typinghero-game-loop/tasks/intro-preview-300ms-20261026`.
Pinned image and existing shared activation HOME are reused; no credentials copied.
Private QA20261026T153054-webgl reports the exact90a0a643 input already tested.
Build/native/review are still pending. Remove this task-owned server root only
after final receipts/player/media are saved, with no active task consumer.

Private cold build153054 stopped before player generation: protected
`CobblestoneUI64.asset` changed on first serialization. Compared archived input
2217448bytes/6014CRLF with serialized2211434bytes/0CRLF: exact byte equality after
CRLF->LF normalization. Initial task launcher had omitted that established Ubuntu
normalization step. The launcher now normalizes only Unity serialized text files
in its private server copy and writes per-file original/normalized hashes, without
altering repo inputs or bypassing the font guard. Warm QA160646 has the same raw
90a0a643 snapshot; build/native/review remain pending.

Warm Ubuntu QA160646 exits0: strict repeat-builder PASS, QA=true,33/33 downloaded
player file hashes match the immutable Ubuntu output; player52899c8cfddf3ca0.
All1708 serialized-text normalizations are recorded with before/after hashes.
Native Edge156/GTX1650Ti/D3D11 first attempt passes92checks,0exceptions/provider IO,
then fails waiting for a fresh paused snapshot while another window owns focus.
Document.hasFocus true->false is observed, but Unity stops publishing while away.
Driver now brings the real game window back before polling paused state; no game
change. Failed report/video retained separately; final native/review pending.

#### Final300ms acceptance

No runtime change after passing snapshot90a0a643. Unity10/10PASS,0fail/skip;
Ubuntu QA160646 exits0, strict repeat-builder/font protection PASS; downloaded
33/33files match. Native Edge156.0.4314.42/GTX1650Ti/ANGLE D3D11 at1280x720:
125checks PASS,0exceptions/provider reads/writes. Real Play/Continue/retry, held
letters/Space/Enter, focus loss/Resume,700ms pre-alpha stalls on full/short flows,
350ms already-visible stall, and Pause/Menu/Restart during extra preview pass.
The initial focus-driver timeout remains a failed attempt, separately retained.

Directly inspected all593frames/583within-scope consecutive transitions in33
sequential sheets, with overlap across sheet boundaries and additional native-size
full-word frames. Final scopes were corrected for the visually observed encoder
origin offset (~800ms) with conservative margins. Extraction selects exact original
frame indices, avoiding one floating-point endpoint omission in the first attempt.
No dropped/repeated/held encoded frames were removed or interpolated. These video
counts are visual evidence only; the300ms minimum is established by Unity runtime
timestamps anchored after the actual full-alpha LateUpdate, not encoded frame counts.

| Scope | Original first-last PTS | Inspected frames | Inspected transitions |
|---|---|---:|---:|
| normal-play | 17.68-19.08s | 36/36 | 35/35 |
| normal-continue | 28.20-29.44s | 32/32 | 31/31 |
| normal-retry-pause | 20.24-22.52s | 58/58 | 57/57 |
| normal-menu-cancel | 35.32-37.64s | 59/59 | 58/58 |
| stalled-play | 46.44-48.40s | 50/50 | 49/49 |
| stalled-retry | 49.08-51.04s | 50/50 | 49/49 |
| post-alpha-stall | 52.00-53.44s | 37/37 | 36/36 |
| extra-pause | 54.20-57.12s | 74/74 | 73/73 |
| extra-menu | 57.76-61.28s | 89/89 | 88/88 |
| extra-restart | 65.64-69.92s | 108/108 | 107/107 |

Visual PASS: stationary readable words/full timer through the extra wait and Pause;
visible post-alpha stalls count without restarting; Resume holds remaining reading
credit; Menu/Restart suppress old heading through cover and clear old effects.
Active ink/timer appears only after the preview; no earlier combat or double handoff
observed. Smooth authored rhythm remains frame-quantized; a late reveal lengthens
only final preview. ProcessInput, held-input barriers,100ms ink cue and500ms first
attack schedule remain unchanged. Tutorial retains its separate path.

Retained deliverables (outside CodexTemp):
`D:/Projects/TypingHero-evidence/intro-preview-300ms-20261026/`:
normal-play.mp4, normal-continue.mp4, stalled-play.mp4, stalled-retry.mp4, complete
accepted.webm, failed focus-attempt.webm/report, Unity XML, build/player/hash receipts,
normalization receipt, native driver/report, all33sheets and per-frame review ledger.
Video is25fps encoded output with original timing; no engine-FPS/human-reading or
sub-frame visual guarantee is claimed. No broader profiles or itch publication.
Integration/cleanup receipt is written there after remote verification.

### Production release of approved 300ms fix (2026-10-27)

Release source is pinned to `3bfb4f3f32398ed72d97106ebb662031ffe3592c`,
not the dirty original checkout or later main changes. Ubuntu Unity2022.3.25f1
production20261027T020231 finished successfully on the identical90a0a643 snapshot.
Only1708 serialized-text CRLF normalizations were applied in the private build
copy; asset/font protections remain enabled. Reused Unity10/10 and QA125 checks
from the unchanged input; no new full profile, gameplay/API change or sprite claim.

Production ZIP33files/14,811,142bytes, root index and CRC PASS; qa=false, decoded
JS/WASM/data exclude the QA markers. SHA256
`902f8a2f880279e8173e3937ae0d90de9c6c4c71b5e678de9fbe6ddd32b2b888`.
Original production HTML at1280x720, Edge156/GTX1650Ti/D3D11: menu, name/Play,
Start Battle confirmation, real words/combo/Slayer, Pause/Resume, first duel win,
Continue to Double Strike, Restart to Swordsman/score0 and Menu inspected. No
exceptions, console errors, fixture reads/writes or external requests in the
complete final local smoke. Driver failures/interruptions remain separately saved.

Public https://micharel09.itch.io/clackblade now selects upload20065750. Actual
public Run game loads the menu without added cache queries. Same public CDN player
was opened directly for real canvas input because nested iframe screenshot refs
were unavailable: Play/guide/combat/Pause/Menu inspected. No captured exceptions
or provider requests; Firebase blocking enabled. Worker targets are outside
BrowserSkill capture, and public smoke did not finish/submit a run. All32non-HTML
payloads match:29exact original bytes,3exact decoded Brotli contents. Index differs
only by the standard static.itch.io/htmlgame.js integration script.

First candidate20065590 returned cached404 for Build payloads. Source ZIP was
valid; cause remains unproven. Restored19943928, waited for Save and verified old
public menu. Only then retried the identical ZIP as20065750, allowing first-time
processing to finish before public Run game. itch.io superseded the failed candidate on the same-filename retry; it is no longer
listed, and no delete action was issued. Previous19943928 remains hidden/retained
for rollback. Fresh final editor confirms playable20065750 and identical unrelated
form fields/gallery; theme untouched. Remote receipt/resource cleanup are recorded
in the release manifest.

Artifact, clips, compact reports and SHA manifests are retained outside CodexTemp
at `D:/Projects/TypingHero-evidence/release-preview-300ms-20261027/`.
Canonical receipt: [release manifest](../../marketing/itch-io/preview-300ms-release-20261027.json).

Cleanup: removed the private server root (no running owned containers), leaving
shared Library/cache/license untouched; removed the expanded local player and
closed the owned BrowserSkill session (registry confirms it is gone after an
initial cleanup timeout). Keep the clean release worktree/local branch at
`D:/Projects/TypingHero-worktrees/release-preview-300ms`: its terminal hosts the
shared browser daemon, which another session uses. Remove it only after that
daemon can be safely retired/relocated and remote receipt remains verified.

### Typed READY between opponents (2026-10-27; verified)

The user corrected the earlier button interpretation: remove the CLEARED/HP/NEXT
card and Ready button entirely. After the nonterminal death animation finishes,
show only READY in the existing current-word slot, with next/third words blank and
no countdown. Type the whole word (case insensitive) to run the existing Continue
callback once; Enter has no action. Mistakes reset READY with normal typing
feedback; partial input survives Pause/Resume. Waiting and Ready attempts must not
change combat HP, score, elapsed time, letters, parries, or trigger attacks/Slayer.
Same-frame/held input and trailing batch text must not reach the next encounter.
The short intro, actual300ms preview, terminal boss results, tutorial and Play
remain owned by their existing controllers.

Source mapping: EndlessDuel.EnemyDeathFinished -> TypingManager.BeginReady -> the
existing ProcessInput/word paint/error path -> EndlessDuel.Continue. EndlessOverlay
is no longer instantiated by the duel; its source/GUID is preserved. No new panel,
font, scene, save/provider contract, dependency or sprite change.

Verified: initial28/28 relevant Unity typing/Endless/intro cases; final3/3 Ready
cases (including the final modal-close guard) on snapshot5d608c59; same-input QA
build/repeated builder PASS; exact33-file player3d4a99d4211d7887; native Edge156/
GTX1650Ti/D3D11/1280x72017/17 real-input checks,0exceptions/provider IO. Two real
wins, held R at death, Enter/wait, partial/mistake/case-insensitive Ready,
Pause/Resume, held Y/Enter through short intro and Menu while waiting all pass.
READY/full partial-progress and next-combat stills directly inspected: original
tray retained, obsolete cards/button/countdown absent, original combat words/timer
restored. No sprite/animation assets were changed or newly frame-reviewed.

Native observation only: empty TMP glyph bounds made the first QA JSON invalid;
external reader normalizes nonfinite geometry to null without modifying player
bytes or game state. Second pilot missed the short intro while sequential typing
continued after it; final observer starts before the last key and holds Y/Enter.
Same-batch trailing-input rejection remains verified offline. Both failed pilots
are retained distinctly. Runner load/import delays and initial build lock timeout
do not count as failed gameplay checks. See the exact snapshots/results in
[verification](../game/verification.md).

Requested21.499s MP4, full recording, three stills, scripts/XML/payload hashes and
separate pilot reports are retained for review/reproduction at
D:/Projects/TypingHero-evidence/type-ready-20261027/ outside the repository.
Delivery and scoped cleanup receipt is written there after remote integration;
expanded preview/temporary worktree can be removed while preserving the clip,
reports and source receipts. No production build, deployment or itch update.

### Pixel summoning ritual — implementation checkpoint (2026-10-27)

Accepted user plan: retain unlimited typed READY in the original word tray, with
no result/NEXT card or confirmation button. After death, soften the environment
to85% over240ms. An amber#E5BE72 pixel ellipse (.9body-height wide/.22high)
at the cached spawn foot has five clockwise marks. Idle opacity breathes1.6s;
each correct letter charges one mark80ms, errors reset120ms, reduced motion
disables only breathing. A batch need not wait for individual mark animations.
The complete seal must render before its180ms hold starts, then invoke Continue
once. Use a200ms charcoal fade for this transition; existing short intro smoke
starts200ms, intact opponent/shadow344ms, seal ends600ms. Preserve the actual
300ms word preview, tutorial, terminal victory, health/score/save/public contracts.

Session owns the private meshes/particles/clock/audio channel through reload;
TypingManager.ProcessInput remains the only letter-input path. Environment gain
composes after the Slayer background and before actors/transparents. Pause/focus
hold all ritual clocks/progress; navigation cancels callbacks, sound and VFX.
SummonSeal reuses the existing light-whoosh clip at-7semitones with SFX gain/mute.
No character sprite or authored animation asset changes.

Reference record carried from the user-supplied plan: Blasphemous Sacred Sorrows
https://www.youtube.com/watch?v=_zH38erDvmI,96.997–107.007s, reported301frames/
300pairs inspected previously. Used for prepare→intact appearance→settle→combat.
Dead Cells https://deadcells.wiki.gg/wiki/Boss_Rush and TowerFall
https://towerfall.wiki.gg/wiki/Co-op inform only rest/commit/wave structure.
This implementation session has not re-inspected those frames or obtained their
raw image ledger; web fetches returned cache-miss/403. No new complete reference
research or exact motion-match claim. Reference evidence is distinct from the
forthcoming review of the actual CLACKBLADE output.

Base origin/main66749da1; Orca branch micharel09/duel-summoning-ritual at
D:/Projects/TypingHero-worktrees/duel-summoning-ritual. Existing dirty main intact.
Acceptance scope: complete death→wait→wrong→READY→spawn video; inspect every
encoded frame/adjacent transition at original timing. Focused input/state tests,
Pause/focus at arrival/partial/charge, Menu/Restart at wait/partial/charge,
Slayer-gain restoration, SFX mute, normal→next and elite→boss/terminal; real-input
normal/boss and reduced-motion browser checks at1280x720. Full suites, stress,
additional viewports, production build and external publication are out of scope.
Evidence directory: D:/Projects/TypingHero-evidence/duel-summoning-20261027/.
Doctor verified pinned2022.3.25f1/toolchain/license and35.2GiB free on fly-server.
Implementation and tests are pending execution; no acceptance/integration claim.

Initial scoped run20261027T071640-quick, snapshot3ff967d8:49/51PASS,0skips,
121.556s. All10intro cases, both READY-input cases and the actual carry scene
pass. One new ritual fixture falsely accepted stale Elapsed2.4 from the previous
intro during scene loading; it now waits for Running before checking new timing.
The other failure is the pre-existing music-gain fixture creating a duplicate
MusicPlayer after scene-based tests left the persistent service alive; duplicate
Awake returns before its voices initialize, so its fixture Play dereferences null.
No music runtime change or passing claim for that fixture. The broad GameplayTests
selection was unnecessary; the follow-up targets4ritual,2READY and1actual-carry
case. Also explicitly clears TypingManager's pending READY delegate on cancellation,
and navigates after the charged seal has painted (including its playing cue).
Prior failure XML/snapshot are retained as initial-tests.xml/initial-snapshot.json.

Run072842-quick (88656633) was stopped before tests, while still in Editor import,
because the added terminal-victory fixture could resolve SampleScene's production
provider. Its exact owned container was removed; exit137 is cancellation, not a
test pass/failure. The fixture now clears databaseRootUrl before terminal queueing
(FirebaseEndless returns before creating any request) and restores the pending
preference. Source copy/Library remain isolated. QA build's first360s lock wait
expired without a build; it is an environment queue delay, not a build result.
Subsequent runs normalize only serialized text line endings in the isolated
snapshot, as the existing native-preview adapter does; runtime/source/asset
semantics and all builder/font protections are preserved.

Targeted0323e5ac snapshot,073756-quick:7/7PASS,0failed/skipped,65.854659s.
This covers4ritual cases,2READY input cases and the actual carry/reset consumer.
Includes cached dead-enemy foot, clockwise progress/error, batch/trailing input,
post-render180ms hold, shared world/HP/score/letters, elite→Captain→terminal,
Pause/focus at arrival/partial/charge, both Menu/Restart at three READY phases,
Slayer/gain baseline restoration and mute. Initial10/10intro regressions are reused
for unchanged intro behavior; no full-suite claim. Final source additionally
centers READY in the whole existing rail (combat placement unchanged). The final
QA adapter runs just the2READY input cases on that exact snapshot before building.
The second native-build360s lock wait also expired before execution; both attempts
produced no player. Final build is queued after the passing7-case run.

Final centered snapshotc546b712,080232-webgl:2/2READY cases PASS before the
build. Runtime delta from0323 is only READY's horizontal Paint position; combat
layout/input/state/renderer logic is identical. WebGL platform import/build is
running. Final input hashes (2469files; binary hashes raw, text normalized only
for CRLF) are in final-source-files.json outside Git. Current external native
driver covers five real wins, wrong/partial/held READY, Pause/real-tab-focus,
reduced motion and entering Captain, with fixture interception and blocked
external traffic. export-review.py predeclares complete normal/boss scopes before
frame inspection. None of this pending browser/video work is claimed as passed.

080232 QA/repeated builder PASS; all33downloaded files match. Player82399f79935faa94,
Edge156/NVIDIA GTX1650Ti/D3D11/1280x720 pilot:60checks before a focus-observer
timeout,0runtime exceptions/provider IO. Complete first ritual, centered READY,
progress/error/Pause/reduced-motion/heldY and HP/score/letters carry pass. Direct
ready.png/partial.png inspection FAILS the floor composition: view-facing ellipse
extends below the opaque floor, hiding its lower arc/two marks. No visual pass.
Pilot moved to pilot-floor-and-tab/ (including exact player/audio/full recording).
The same-window tab change did not deliver application focus loss; this is not
counted as a focus pass. A separate-window preflight and final rerun are planned.

Correction: inverse-project the existing pixel ellipse/mark mesh onto the authored
world floor,15mm above it, using the live camera only at creation. Keep cached foot,
.9H/.22H screen footprint, five fixed clockwise offsets, existing material/depth
and all clocks/input/300ms-preview behavior. No ZTest override or actor occlusion
bypass. Added assertions that every particle and ring vertex share the floor
plane. Next snapshot reruns the4affected ritual cases before QA build. Browser
and complete final-frame review remain pending; integration withheld.

Floor-fix Ubuntu submission lost SSH while waiting/uploading, then two read-only
SSH probes timed out. No final-floor test/build is claimed from that submission.
Continued using the existing Windows Unity2022.3.25f1 and existing WebGL module
at E:/Unity/Hub/Editor/2022.3.25f1, without installation or editing another project.
Task-private source/Library/results: evidence-root/local-unity; normalized snapshot
c818fa0f,2501build/tool files. It runs exactly4ReadyRitualTests then QA/repeat build.
local-verify.py owns/waits/terminates only its Editor and guards disk headroom.

Focus observer diagnosis: Playwright's default focus emulation makes every page
report focused, so switching tabs/windows does not test real focus. Reused the
existing intro-smoke recipe: disable Emulation.setFocusEmulationEnabled on both
windows, then bring the other window forward. Pilot focus-window-3 passes6checks,
actual document focus false→application paused,0runtime exceptions/provider IO.
No gameplay setter, synthetic focus event or OS/Computer Use was used. Earlier
preflights remain separate (one missed the tutorial Enter; the other diagnosed
focus emulation). Final native driver uses the real Start Battle button and the
qualified window-focus method.

Local Windows first import was interrupted by the execution-session boundary at
15:50 before tests began; no XML/pass exists for it. Retained import-interrupted.log.
Continued the exact c818fa0f source/Library with local-verify.py (owned Editor3808),
without replacing source or accepting stale XML. C# recompile passed; remaining
package/resource imports are still running. Final floor geometry remains unverified.

Windows final-floor snapshotc818fa0f:4/4ReadyRitualTests PASS,0failed/skipped,
69.4304656s (2026-10-27 09:09:31Z-09:10:40Z). XML/log in local-unity. Includes
floor-plane assertions, cached dead foot, rendered180ms hold, one carry into
Captain/terminal exclusion, pause/focus, six Menu/Restart cancellation paths,
Slayer baseline restoration and muted completion cue. QA/repeat WebGL build
started from the same snapshot; final browser and frame review still pending.

Final Windows QA/repeated builder PASS (strict font/repeat checks intact),33player
files hashed in player-manifest.json; source snapshotc818fa0f matches2501worktree
inputs. Player74fd14c7306f372d, Edge156.0.4314.42, GTX1650Ti/ANGLE D3D11,1280x720:
178real keyboard/mouse checks PASS,0runtime exceptions,0provider reads/writes.
Covers five wins into Captain, death-frame heldR, Enter/indefinite rest, two marks,
wrong reset, Pause/Resume, real separate-window focus loss, reduced-motion preference,
heldY through handoff, HP/score/letters/shared-world carry and clean Menu.

Final direct output review (not a new reference-video research claim):
- complete.webm16.041-29.667s -> ready-summoning.mp4,13.626s selection:
  340/340frames and339/339consecutive transitions directly inspected.
- complete.webm81.414-87.665s -> captain-summoning.mp4,6.251s selection:
  156/156frames and155/155consecutive transitions directly inspected.
- Scope declared in review-scopes.json before inspection. All22contact sheets viewed
  at original320x180 world resolution, including previous-page boundary overlap;
  native1280x720 ready/partial/Captain stills also directly inspected. Source-selected
  frame counts match exported counts exactly; no interpolation/retiming, held frames
  and PTS preserved. Aligned captured WebAudio accompanies both MP4s.
- PASS: complete death/end hold precedes ritual, full ellipse/five clockwise marks
  above floor (lower arc no longer clipped), centered original tray, partial/error,
  Pause holds, steady reduced-motion charge, completed hold then alpha charcoal
  cover, seal into smoke, whole normal/Captain silhouette and shadow together,
  seal gone and first word readable before combat. First scope includes existing
  preceding Slayer exit; second ends with explicit Pause after Captain entry.
- review-ledger.json records inspected counts, every sheet and frame-range findings;
  each frames.json records every output frame/PTS/hash. Both clips fully decode.
  Existing reference evidence remains distinguished as recorded above; no claim of
  re-inspecting the prior301Blasphemous frames or exact reference-motion match.

Evidence retained at D:/Projects/TypingHero-evidence/duel-summoning-20261027/.
No production publication, full suite, stress/soak or sprite asset edit.

Final resource audit: SSH recovered; docker ps shows no Unity/game-loop task
container, and no later shared WebGL result than080232 exists. Lost floor-fix
submission produced no acceptance result. Existing remote audit results are retained;
shared source/cache/other task resources remain untouched. Remote main fetched
again and remains66749da1; no integration conflict or retest-triggering main delta.
