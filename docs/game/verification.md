## READY task closed; release canceled by user (2026-10-27)

The user accepted the merged READY change, canceled the pending itch.io release,
and requested deletion of task temp/logs. Gameplay/assets remain preserved in
commit2f2f3da and main; no game/API changes in this closure. No itch.io upload,
form save or publication occurred. Historical validation summaries below remain;
raw receipts and build artifacts are no longer retained.

Removed both local task evidence directories ready-magic-circle-20261027 and
ready-release-20261027 under D:/Projects/TypingHero-evidence/ (about424MiB),
including production ZIP, players, videos, PNGs, scripts, manifests and logs.
Removed the9exact task result directories on fly-server after snapshot identity
checks and confirming no active container mounted them. Shared source/build/cache/
toolchain, other task resources, committed asset/license and remote branches remain.
Old task worktrees/local branches were already removed; Orca tab list was empty.
Closure changes are documentation only: review/diff whitespace checks, no runtime
suite. [Full closure record](../plans/duel-intro.md#ready-task-closure--2026-10-27).

## Dungeon task closed by user (2026-10-27)

User approved the current source for main integration and requested task log/temp
cleanup. Earlier FPS/foreground/final-review gates are waived for this integration;
their missing evidence remains unverified. Lighting QA100208/397smoke is reused
because lighting inputs are unchanged; current-main READY is retained, integration
input/frame contracts38/38PASS. No integrated player rebuild or performance pass.
Raw task artifacts are removed after remote verification; historical paths below
are no longer retained evidence. [Closure record](../plans/combat-hanging-light.md).

## READY production release candidate (2026-10-27; release canceled, task closed)

User accepted2f2f3da and requested release. Pinned Unity production131422,
input105615a0, strict builder/bridge-exclusion PASS. All33downloaded hashes match;
qa=false; decoded JS/WASM/data scan excludes QA/probe markers. ZIP33members,
14872340bytes, CRC/root-index PASS, SHA256 ec37cbf92fe01298cd8fa9479bccdcf97662b055fa7aed4dc89a76bef1eec4a1.
Native Edge156/Intel UHD D3D11/1280x720 production smoke8checks PASS,0exceptions,
console errors or provider IO: menu/name/Start Battle, Pause/Resume, real correct
word advances change->see and score0->10, return Menu; bridge undefined. Images
viewed; isolated browser/static/control servers closed. Prior gameplay/frame QA reused.

Publication was blocked before upload by browser availability/Cloudflare. The user
subsequently canceled release and closed the task. No itch.io write/save occurred;
the qualified ZIP/resume checkpoint and raw evidence were deleted as requested.
The old artifact paths below are historical, not available downloads. No public release claim.
See [release checkpoint](../plans/duel-intro.md).

## Licensed Magic circle and compact READY (2026-10-27; verified)

- Original Magic circle final build frame11 from Pixel Magic Circles, by
  a-small-smart-potato; archive LICENSE.txt read and image directly inspected.
  Personal/commercial game use and modification permitted; no asset-pack redistribution.
  Imported PNG is byte-identical64x64RGBA; Point/Clamp, mipmaps off, uncompressed.
  Procedural ellipse/five marks removed; entire ring has five opacity steps,
  120ms wrong reset, steady rest and existing rendered180ms completion hold.
- Complete-word READY frame reuses the typing-frame sprite at native9-slice scale,
  fixed8px/4px padding; steady cream TYPE READY TO CONTINUE6px above. Existing
  font/word size, combat tray, ProcessInput, public/save APIs and stats preserved.
- Ubuntu Unity2022.3.25f1:6/6focused cases PASS,0failed/skipped,56.8918669s,
  20261027T101311-quick, source878f12b7. After a visual rejection of the original
  tiny seal footprint, only the quad-size consumer case reran:1/1PASS,
  13.7639109s,20261027T103211-quick, sourcece6ddb5d. The other5cases reused.
  Covers actual texture/floor/64pixel projection, each opacity/reset/hold,
  padding/gap/fixed box, batch/trailing/Enter, Pause/focus, Menu/Restart,
  stat exclusion, one carry into Captain, restored combat tray and terminal exclusion.
- Same final ce6ddb5d source QA/repeated builder103545-webgl PASS;33immutable
  player files downloaded with matching SHA256. Native player4fe249aa7e98cbfb,
  Edge156.0.4314.42/GTX1650Ti/ANGLE D3D11/1280x720:184real keyboard/mouse checks
  PASS,0exceptions/provider reads/writes. Five wins into Captain, rest/Enter,
  wrong/partial/completed/held READY, Pause/real separate-window focus loss,
  reduced motion, HP/score/letters/world carry, clean Menu. No gameplay setters.
- Direct final visual review: source complete.webm16.234-28.871s gives316/316frames
  and315/315pairs;69.267-75.704s gives161/161frames and160/160pairs. All21sheets
  inspected at native320x180 world resolution with overlap at page boundaries,
  preserving source timing/held frames. Every frame/clip hash matches its ledger.
  Native ready/partial stills also inspected. PASS: complete death before rest,
  fixed prompt/box, steady ring, whole-image progress/error, held charge, one cover,
  floor seal into smoke, intact opponent/shadow, original tray/no residual seal.
  Reference is the static authored final PNG; no source-animation/reference-video
  motion match or whole-game/performance claim. Existing preview timing tests reused.
- Windows FFmpeg child-start waits prevented local export; stopped only task-owned
  processes and exported the identical recording with existing server FFmpeg.
  One source Opus packet-header warning was emitted; clips decode successfully.
  Audio was not changed by this task; no new audio-fidelity acceptance claim.
- Checkpoint/provenance, rejected pilot and scope details: [duel intro](../plans/duel-intro.md).
  Final evidence: D:/Projects/TypingHero-evidence/ready-magic-circle-20261027/final-run/;
  clips/frames/ledger in review-output/. Retain for user review/reproduction; clean
  task evidence after it is no longer needed. No live task browser/Editor/server.

## READY summoning ritual (2026-10-27; verified)

- New private session presenter adds five pixel marks, environment gain, rendered
  completion hold and duel-only alpha cover. ProcessInput still owns letters;
  stats, public/save contracts, tutorial and terminal victory remain unchanged.
- Windows Unity2022.3.25f1, exact normalized snapshot
  `c818fa0ff4c7211e5a20d5f9acf76165c82ac0d217156b528ce4fe944a8dfcb7`:
  **4/4 ReadyRitualTests PASS**,0failed/skipped,69.4304656s. Covers cached spawn,
  ground-plane geometry, input/error/batch/Enter/stat exclusion,180ms painted hold,
  one carry into Captain, terminal exclusion, pause/focus and six Menu/Restart
  cancellations, independent Slayer restoration and SFX mute.
- Earlier snapshot0323e5ac passed7/7 focused cases (four ritual/two READY/actual
  carry); initial10intro cases also passed and are reused for unchanged preview
  behavior. One unrelated music fixture in the wider first selection failed;
  no full-suite pass is claimed. Final source matches all2501snapshot inputs.
- Final-floor QA WebGL/strict repeated builder PASS;33immutable player files hashed.
  Player74fd14c7306f372d, native Edge156/GTX1650Ti D3D11/1280x720:
  **178 real-input checks PASS**,0exceptions/provider reads/writes. Five wins into
  Captain, partial/error/Enter/held keys, Pause/real-window focus loss, reduced motion,
  carry and clean Menu. Earlier pilot rejected floor clipping; retained separately.
- Direct visual acceptance: all340frames/339pairs of complete death/rest/error/READY/
  summon scope and156frames/155pairs into Captain inspected,22sheets with boundary
  overlap, original timing/held frames preserved. Source/export frame counts match;
  native stills also viewed. Existing300ms preview tests reused; video frame counts
  are visual evidence, not a substitute for runtime timing assertions.
  No production/stress/soak or publication is in scope.
- Full receipts, attempted runs and reference/output-review distinctions:
  [existing checkpoint](../plans/duel-intro.md). Evidence root:
  `D:/Projects/TypingHero-evidence/duel-summoning-20261027/`.

## Invisible dungeon fill removed (2026-10-27; final acceptance pending)

Ubuntu2022.3.25f1 QA100208 and strict repeated builder PASS on canonical input
`6bed25a62764729c94188f299e3bedc666bd4281d4bc7cc532119428e8b09ade`.
505downloaded files match server SHA256. Serialized world has5fixture components,
front lantern base16 and no removed fill/GUID/child link. Native Chromium153/NVIDIA
GTX1650Ti/D3D11,1280x720, player61b46a73877b0e0f:397real-input checks PASS,
menu/combat/two wins/Pause/Resume/Restart and Slayer flags enter/exit;
0exceptions/provider IO, context closed. Menu/combat stills directly inspected:
former blue floor pool absent. Complete restoration playback, matched-pose A/B,
central-source visual dominance and numeric post-OnEnable observation remain
unverified. Old players are history; new performance baseline is unqualified.
Foreground/FPS/ABBA/production/final-review gates remain closed; no main merge or
itch.io publication. [Checkpoint](../plans/combat-hanging-light.md).

## Dungeon typed READY / foreground checkpoint (2026-10-27; NOT accepted)

New Ubuntu2022.3.25f1 QA072142 includes typed READY; build and strict repeated
builder pass. All33player +472serialized downloads match server hashes,2495game
inputs match the continued source, and the retained observer remains990105b5.
38Node contracts pass. Native fresh-profile smoke297checks passes two wins,
full/partial READY, Pause/Resume and Restart,0exceptions/provider IO. Input uses
real observed key/mouse events; final receipt/report precede bounded teardown.

Performance remains UNQUALIFIED. Long nominal Unity-marker runs fail p95 or
cadence; configuration pairs are incomplete and lack actual Windows foreground
proof. New PID sampling proves DOM focus can stay true while Orca/Edge owns
foreground: three final75FPS canvas controls all pass metrics but lose the real
foreground to Edge. Three new foreground attempts produce no complete qualified
round. Gate closed pending a stable foreground task-Chromium window; no speculative
runtime optimization, B selection, official ABBA, new accepted production/visual
review or main integration. Required direct1x/0.25x video inspection also lacks a
supported tool in this session. See the existing [checkpoint](../plans/combat-hanging-light.md)
for manifests, raw/failing runs, exact environment evidence and resume conditions.

## Dungeon QA observer encoding (2026-10-27; formal acceptance pending)

QA-only particle encoding sends all192slots on first observation and whenever
ObserveRenderParry writes a slot. The bridge reparses its cached JSON each frame;
public nested snapshots remain independent, including stale slots. No game,
render, asset, animation, input, timing or provider source changed.
Diagnostic `PERF_VERIFY=1` compares the final published object with the original
complete JsonUtility snapshot every frame; it is disabled in formal FPS runs.
20Node contracts PASS, including changed/stale slots, independent mutation,
verification failures, required frame count and error/closure precedence.

After merging main29c03285 as85bd1706, Ubuntu2022.3.25f1 A043436/B045018 QA builds
and strict repeated builders PASS.33downloaded hashes per player match server;
472serialized files match byte-for-byte;2493game inputs are unchanged except the
2QA observer files. Ready carry/reset test044828-quick passes1/1.
Final-object equality:1233/1233observed frames PASS in real parry/Slayer.
Comparison-OFF diagnostics pass: normal74.832FPS/p9517.730ms/min1s73;
parry74.802FPS/p9515.755ms/min1s71,0hitches>50ms,correct coverage and0exceptions.
These are short diagnostics, not ABBA or production performance acceptance.
Rejected managed-string candidate and historical failures remain retained.
Formal ABBA, production checks, final capture/playback/frame review and main
integration remain pending. See [checkpoint](../plans/combat-hanging-light.md).

## Automation frame buffer and dungeon performance diagnostics (2026-10-26)

`TYPINGHERO_AUTOMATION` alone contains the fixed16384-row Unity LateUpdate buffer.
Set `window.typingHeroPerfOptions={enabled:true,detailed:false}` for minimal frame
observation; the default retains the existing detailed QA behavior. Batch reader
`typingHeroReadFrames()` exposes loss/focus epochs; it never controls gameplay.
The100ms state is sampled memory/driver context, not individual Unity frame timing.
`tools/game_loop/frame_timing.cjs` keeps Unity/rAF/CPU/GPU clocks distinct and
invalidates missing/focus-lost traces without dropping combat hitches. Run its
16 targeted contracts with `node --test tools/game_loop/test_frame_timing.cjs`.

Harness checkpoint 2026-10-27: final drain precedes verdict; interval ownership
uses the first combat frame, retaining exit hitches. Sliding 1-second combat
windows below40FPS fail alongside FPS>=59/p95<=18ms. Parry/Slayer coverage uses
measured combat only; counter resets and scene entry do not manufacture parries.
Parry protocol requires `PERF_DETAILED=1`; detailed-off parry is unimplemented.
16 Node contracts, JavaScript syntax and diff whitespace checks pass. No player
rebuild or remeasurement; ABBA, final playback and FPS acceptance remain pending.
See [checkpoint](../plans/combat-hanging-light.md) for scope and continuation.

Ubuntu QA20261026T144542/player7cd77c5fb88da486:33file hashes verified;
2495current player inputs match. Four real-input1280x720/world320x180 NVIDIA1650Ti
diagnostics finish with0exceptions/provider traffic, correct normal/parry/Slayer
coverage and no lost samples. All fail the≥59FPS/p95≤18ms target. GPU query is
diagnostic only; detailed-off still retains the100ms state publisher. No runtime
optimization, formal ABBA, new frame-review or production FPS pass is claimed.
Final same-harness normal pair also fails at53.97/54.02FPS,p9520.55/19.53ms;
light changing-WebGL control53.03rAF-FPS,p9519.2ms supports the unresolved
browser/environment pacing concern. No actor-coverage gaps in retained traces.
Ubuntu production20261026T150148 exits0;33downloaded hashes match,2495player
inputs equal QA. Six decoded HTML/JS/WASM/data payloads exclude existing and new
QA/performance markers in UTF8/UTF16LE, with a QA positive control. This is bridge
exclusion evidence, not a production performance/playtest pass.
Exact metrics, retained traces and pending gates: [lighting checkpoint](../plans/combat-hanging-light.md).
## Typed READY replaces the inter-duel card (2026-10-27; verified)

- Supersedes the Ready-button entry below. Nonterminal enemy death finishes before
  READY appears in the existing current-word slot; next/third text is blank and
  countdown is hidden. EndlessDuel no longer creates the CLEARED/HP/NEXT overlay
  or an action button. Full case-insensitive typing invokes the existing Continue
  callback once; Enter does nothing. Wrong letters reset the word using existing
  feedback, without word/character reward events. Waiting is unlimited.
- Unity2022.3.25f1/fly-server: initial scoped typing/Endless/intro regression
  **28/28 PASS**,0failed/skipped,64.063s, snapshot
  `e7e7e18c8c56afeef3c3eef6b194fa96f4f27acda93880d72ba03589c8fc1f61`,
  result `/data/typinghero-game-loop/results/20261027T050430-quick`.
- The final modal/closing-frame guard and its real modal fixture are additionally
  verified by **3/3 PASS**,0failed/skipped,16.738s, on final snapshot
  `5d608c594235e04ee9269608ec9f97c765a277845c12a70b74ea7749627263d1`.
  Ready tests cover activation-frame rejection, full/partial/wrong/case-insensitive
  input, Enter, one callback/trailing batch discard, timer exclusion, modal and
  Pause/Resume. The actual scene covers no overlay, post-death wait, unchanged
  HP/score/time/letter stats, next-duel carry/reset and duplicate death callbacks.
  Other initial regressions are reused: the only later runtime delta guards Ready
  input while a modal owns it; ordinary combat/tutorial/intro code is unchanged.
- Final result `/data/typinghero-game-loop/results/20261027T052010-quick`.
  Same-input Unity QA build/repeat-builder PASS, immutable33-file player at
  `/data/typinghero-game-loop/results/20261027T053003-webgl/player`; all downloaded
  payload hashes match. Native player `3d4a99d4211d7887`, Edge156.0.4314.42,
  GTX1650Ti/ANGLE D3D11,1280x720: **17/17 real-input checks PASS**,0exceptions or
  provider reads/writes. Includes two actual duel wins, held death-frame R,
  indefinite wait/Enter, partial/wrong/case-insensitive READY, Pause/Resume, held
  Y/Enter across the short intro, unchanged HP/score/letters and Menu from Ready.
- Direct still inspection: READY alone in the original typing tray, no result/NEXT
  card or button/countdown, blank next/third labels; typed RE uses existing teal
  feedback; new enemy/normal words and full timer restore after transition.
  Requested MP4 is21.499s, trimmed realtime recording with no retiming. No new
  sprite-animation, whole-game/performance, production build or itch release claim.
- First native pilot stopped on existing QA telemetry emitting Infinity for empty
  TMP glyph bounds. The external observer normalizes nonfinite geometry to null
  only in QA JSON (111messages on the accepted run); immutable player/gameplay
  bytes are untouched. Second pilot missed the short intro while a sequential
  multi-key automation action continued beyond it. Final observer starts before
  the last key and tests held Y/Enter; same-batch trailing text is covered offline.
  Failed reports/videos are retained separately, not counted as accepted runs.
- Source mapping and acceptance: [intro checkpoint](../plans/duel-intro.md).
  External scripts/XML/receipts and the requested preview are retained at
  D:/Projects/TypingHero-evidence/type-ready-20261027/ for review/reproduction.
  Shared-runner load/import and lock waits are recorded as environment delays,
  not test failures or passes.

## Ready between duels (2026-10-27)

- Runtime delta: only the result action caption changes from Continue to Ready · Enter;
  the existing callback, indefinite wait, transition and input guards are unchanged.
- Unity2022.3.25f1/fly-server, snapshot
  `66c3f6019780c3ba104cc47356f0ff2f109970955a045ec51030ed4854a07699`:
  focused `EndlessTests.ActualSceneCarriesHealthAndStatisticsAndResetsCombatBindings`
  **1/1 PASS**,0failed/skipped,9.888s. Reads the live Ready caption, checks frozen
  HP35/score123/combat seconds during the result wait, invokes the button callback,
  and verifies next-duel carry/reset and existing Pause/Resume behavior.
- Reuses unchanged Continue/input evidence from the smoke-led intro and actual300ms
  follow-up below and [intro checkpoint](../plans/duel-intro.md): real Enter/held-input,
  short transition, minimum preview, Pause/Resume and Menu. No new native browser
  click/keyboard, visual, WebGL build, performance or publication claim.
- XML/log retained for audit at
  `/data/typinghero-game-loop/results/20261027T040237-quick` on fly-server;
  disposable after this verification record is no longer needed for reproduction.
  Initial runner-lock attempt executed no tests; queued retry passed.

## Slayer B — completed and user accepted (2026-10-27)

Integrated and remotely verified on main83d6e0a6. Clean scoped Unity81/81,
QA/repeated builder and six technical playback checks passed. Direct still review
covered1535frames/1532pairs. Integrated latest-main intro/Slayer transitions passed
22/22cases,0failed/skipped. The user accepted the viewed visual result; sampling
and occlusion limitations remain documented, without an automated continuous
subjective-playback claim.

At the user's request, external clips/player/log/XML/archive/frame evidence and
task-only runner results were deleted. Only the in-repository Markdown completion
record remains; historical references below do not imply retained raw artifacts.
[Completed task and verification summary](../plans/slayer-slash-vfx.md).

## Smoke-led duel intro and identity handoff (2026-10-26; verified)

- Ubuntu fly-server, pinned Unity2022.3.25f1,2CPU/8GiB. Final source
  5b897298ce34316af5a533e34f2e91ca7e20e3064aed43d50d9e3bc362cd7141:
  7/7DuelIntroTests PASS,0failed/skipped,40.336s. Private-cache093228 build/repeated
  builder PASS;284C#/shader/package/version inputs match afterBOM/CRLF normalization.
- Immutable player10a1876ff58157a5:33/33files match Ubuntu. Edge156.0.4314.42,
  Playwright1.64.1, GTX1650Ti/ANGLE D3D11,1280x720:83real-input checks PASS,
  0exceptions/provider reads/writes, fixtures/external blocking retained. Full/short
  intro, held letters/Space/Enter, actual duel clear/Continue, retry, Pause/Resume,
  real window focus loss, Menu cancellation and tutorial covered. Same world10762,
  camera11172/RT-458 throughout. Observer-only bridge; no gameplay setters.
- All31sequential sheets directly viewed:566/566encoded frames and561/561adjacent
  pairs across five predeclared full scopes, original25fps PTS/held frames retained.
  Smoke anticipates intact pose/shadow, stepped lobes dissipate low, one identity
  at a time, stationary300ms preview, no navigation-title flash or residual effect.
  Supplementary native-size smoke/preview/cancellation images viewed. Video counts
  are not a1:1engine-frame/performance claim. Reference evidence stays separate.
- Earlier57-check player passed functionally but its601-frame review FAILED because
  Menu re-showed the paused heading before cover. Final presenter suppresses it
  through navigation; a last-frame pause now also defers combat handoff. Cold-cache
  import timeout and two fixture observation/wait corrections are retained, without
  counting unexecuted/failed checks as passes. No Windows Unity, full suite,
  performance profile, production build or itch.io release.
- Timing, exact source/reference scopes, failed drafts, final ledger and cleanup:
  [intro checkpoint](../plans/duel-intro.md), evidence root
  E:/CodexTemp/TypingHero/intro-smoke-rhythm/. Remote integration receipt is recorded
  there after verification; unrelated chest-anchor evidence qualifications remain.


## Shared chest anchor for Slayer B (acceptance reopened 2026-10-26)

The playback-quality audit supersedes the earlier overall acceptance claim.
Geometry and scoped visible-frame observations remain reusable, but completion
of a video does not establish playback quality. New reports separate callback
count, presented-frame metadata, decode quality, focus/visibility and elapsed
time from direct motion observations. Acceptance remains **unverified** until
all six playback passes qualify technically and have direct observer findings.
See [the playback audit](../plans/slayer-slash-vfx.md#playback-quality-audit-2026-10-26).

Ubuntu Unity2022.3.25f1:20unique scoped PlayMode cases PASS,0failed/skipped
(14unchanged cases from062003 +6final geometry/callback cases from063715).
Repairs the five inactive actor fixtures with explicit body/ske profile. Real
prefab/profile checks cover all217poses, seven held-parry mappings, pivot/flip/
parent transforms, fixed world angle/scale, original pixel alpha centroids,
120/160ms lifetime, hide/destroy/death, re-enable/ConfigureEndless, pool reuse,
pre-damage point capture and pause/resume. No public API or production fallback added.

Final Ubuntu WebGL070505 and strict repeated/protected-font builder PASS. LF input
15bd6781c5fc751631c7ae323a7eea1b03f577796d3d5256e301a831535e55bf;
2494inputs/252C# match pre-integration source,33player hashes retained;
player104dd2400c6dba53.
B PNG/GUID, blackRGB,120/160ms, overlap and combo_end remain unchanged.

Native Edge156/GTX1650Ti/D3D11/1280x720 final capture: four duels, HP32,17parries,
0exceptions/provider reads/writes.1455impact samples have root-anchor distance0,
all4B cels, maximum2overlap. Pause invariants hold for48/52/55observations across
sword/axe/shield. One miss is diagnosed as driver Space delivery after already
observed damage; no unexplained parry fault or gameplay timing change.

Direct final output review789/789frames,786/786pairs; original variable PTS and
held frames preserved, including one explicitly declared extra ending hold. Three
MP4 clips match source frame counts/PTS. Historical Edge playback reached the
end at1x/0.25x, but its callback counter was mislabeled `presentedFrames` and
its driver did not qualify interruptions, focus or dropped frames.
B center often merges into black torso/shield or is covered by ivory arcs/Pause UI;
those exact frame limits are recorded separately from centroid/root measurements.
No full suite, stress/soak, multiple viewports, full raw-video review, performance
claim or publication. Final receipts, prior failures, authoring guidance and
integration status: [existing checkpoint](../plans/slayer-slash-vfx.md).

Merged current mainb68b020 as b975485; Ubuntu074357 passes3/3affected intro-lock,
pause/cancel and accepted-character damage-window cases (23distinct scoped cases
overall). All VFX/anchor/profile/render inputs are identical; only four upstream
intro/typing/probe/test inputs differ. Pre-integration three-form capture and20
cases are reused for unchanged B behavior, not represented as a merged-player
build. Verified fast-forward main integration and final receipts are in the checkpoint.

## Restrained duel intro polish (2026-10-25)

Menu/cover ease upward together;18small staggered dust puffs replace overlapping
large quads;120ms body-landmark reveal shares the actor color/depth/shadow clip.
Opponent heading moves6reference pixels; HUD precedes word tray by80ms. Intro
canvas39 stays below Pause40. Same full/short timing and input/combat contracts.
Unity2022.3.25f1:10distinct scoped cases PASS across recorded targeted runs;
latest reveal/lock2/2 and final Pause-layer/focus1/1 pass. QA WebGL strict repeated
builder PASS;253C#/shader/package/version inputs match the task source.
Final native Edge156/Intel UHD/D3D11/1280x720 player46e41cff3648399e:61assertions
PASS, zero exceptions/provider reads/writes. Actual keys/mouse cover Play, held
Enter/letters/Space, Continue/retry, partial-reveal Pause/Resume, focus and Menu.
All572/572encoded frame visits and567/567consecutive pair visits directly reviewed
on31sequential sheets, original25fps PTS/held frames preserved. Covers full Play,
short Continue, retry/pause, partial reveal cancellation and actual focus loss.
Shared world10752/camera11162/RT-458 remain stable. No FPS/performance, speaker
listening, full-suite or itch.io claim. Earlier failed builds/drivers and visually
rejected pilots are retained separately in [the checkpoint](../plans/duel-intro.md).

## REPLACED-inspired dungeon lighting (2026-10-25)

Warm key16 plus constant shadowless cool Spot64/range16, and Floor1..8 palette
surface masks (metallic0, stone smoothness .306..369, mortar .012). Existing actors,
flames, camera, disabled rune/haze/beams/reflection and gameplay are preserved.
Missing parry-lift material companion restored through the existing palette recipe.
Unity focused11/11 and strict repeated builder PASS. Pre-integration native WebGL
321b30e604a19551:16checks,0exceptions; real word/parry/followup, Pause, Slayer restore,
retry/death/menu.596/596 static pose rows,538/538 adjacent key pairs, plus491/491
selected runtime video frames and485/485 adjacent pairs directly reviewed.
Normal10s ABBA rAF p95 mean33.275->33.225ms (-0.15%, within10% guard); short samples,
not GPU timing or60FPS assurance. Legacy tutorial missing-map dim poses remain an
explicit inherited limitation; no exact REPLACED or timed-playback match claimed.
Merged origin/main2b07f3f:11/11 focused cases and strict builder PASS; final player
961c878991127356 repeats16checks with0exceptions.191/191selected merged frames
and188/188pairs reviewed for tutorial/short entry, retry and Slayer restore;10s
p95=33.275ms. Detailed scope/limits in the [lighting checkpoint](../plans/combat-hanging-light.md).

## Duel entry presentation (2026-10-25)

Full Play uses the 2.4s player -> dust/enemy -> opponent label -> HUD sequence;
Continue/retry use the short sequence. Intro locks combat/statistics/input without
freezing idle or music; Pause/focus holds the presentation and Menu cancels it.
Unity2022.3.25f1:17 distinct scoped PlayMode cases PASS across targeted runs,
including 5/5 scene/tutorial consumers after the audio-host correction.
After integrating origin/main5085b32,11/11 intro/music/session/tutorial cases
PASS on D3D11 (merged-d3d.xml).
Pre-merge QA WebGL strict repeated build PASS;252 source/config inputs matched
the captured revision. Later origin/main Slayer changes are retained; the clip
and native checks below describe that pre-merge player.
Native Edge156/Intel UHD/D3D11/1280x720 player05832092f73446d6:58 checks PASS,
zero exceptions/provider reads/writes. Music voice/sample continuity confirmed.
Output:509/509 encoded frame visits and504/504 consecutive-pair visits directly
reviewed across five declared scopes; original PTS/held frames retained.
Reference research and output evidence are separate; no performance, speaker
listening, full-suite or itch.io release claim. Full scope/limitations and evidence
paths: [duel intro checkpoint](../plans/duel-intro.md).

## Contact-first Slayer B (2026-10-25; tests waived)

Final pinned Unity WebGL build exits0, input3272ecc39ea954d4e9e55850b31d3c8acb5dbc68a2ffb6e6476a72710e10aaaa,
player03a0033e57fdf3a2. Capture-only real keys reach enemyHP0/playerHP40 with
50/51correct letters and4parries,0observed exceptions/provider reads/writes.
Final canvas/audio clip and1x/0.25x viewer opened in Edge; paths and immutable
receipts are in the checkpoint. Source gameplay retains the original collision
window/event ordering. Feature6227d17 is verified on remote main. No test-pass,
frame-review, reverse-facing or subjective game-feel acceptance is claimed.

User requested: finish and open the clip immediately, with no tests. Scoped test
startup was cancelled during project initialization before any result XML; authored
cases remain for future use, and no pass count is claimed. Build and capture are
required to produce the requested gameplay clip, not substitutes for behavioral or
frame-review acceptance. Older receipts below qualify their original revisions.
See [the Slayer checkpoint](../plans/slayer-slash-vfx.md) for current status.

## Slayer body-center correction (2026-10-25)

User rejected the B contact-edge anchor. Slayer hit/finisher now capture struck
body collider center before damage/hurt/death and keep that world position;
damage/camera/audio and legacy/non-Slayer consumers retain contact coordinates.
Unity2022.3.25f1:51/51scoped PlayMode PASS,0failed/skipped,5.2562155s, including
exact center capture when damage moves/disables a mirrored/scaled offset body.
QA WebGL/strict repeated builder PASS;249C#/package/version inputs match source.
Native Edge156/Intel UHD/D3D11/1280x720 playerb450043660a03328:2772checks PASS,
0exceptions/provider reads/writes,all8angles and139stable leases observed.
Existing0.2WU body hit jitter remains; frozen VFX does not follow later jitter.

Static96/96source cels and80/80internal pairs reviewed around Hurt_0 body center;
native460/460selected encoded frames and457/457pairs directly reviewed with held
frames/originalPTS retained. Covers hit/pause/resume, exit/restart, rapid gold/red
hits and lethal/death/result. Pale first-hit core is partly occluded by menu/old
weapon trail; unoccluded rapid hits and read-only overlay establish placement.
Edge1x/0.25x playback clock check PASS; no performance/full-suite/itch claim.
Evidence: E:/CodexTemp/TypingHero/slayer-center-fix/; full scopes, hashes,
per-frame ledgers and limitations in [existing checkpoint](../plans/slayer-slash-vfx.md).

## Windows-local startup follow-up (2026-10-25)

Frozen18e03a2, local Unity2022.3.25f1/WebGL Support, Edge156/GTX1650Ti/D3D11,
1280x720:84formal navigations, per-navigation adapter matching, HTTPS local and
20Mbps/50ms, fresh/reload separate, no timing screenshots/readbacks, all samples
retained and no page/console errors/provider writes. Compiler ABCABCABC+ABCCBA
rejects DiskSize/DiskSizeLTO: neither reaches5%and100ms improvement. Separate
ADADAD rejects Medium: local fresh4219.0 ->4190.6ms(-0.67%), network10246.8 ->
10021.9ms(-2.19%). Keep original production configuration; **no additional
optimization accepted;20%target not achieved**. Editor reports BuildTimes/Low,
HasPro=False/splash on; no splash disable trial or license/toolchain change.

4337packed entries/48assemblies audited; six Endless Resources assets present;
serialized Slayer comboMax and URP shadow-resolution field survive Medium.
No resource removal or broad link.xml preservation. Clean production input
ce98020bed7f2fbe5f5d065132a4a9f867ff62b732fea4d27b454a476baa6994 PASS;
33immutable file hashes verified and decoded HTML/JS/WASM/data exclude UTF8/UTF16
QA/startup markers. Runtime C# and builder match frozen Git exactly after
CRLF-only normalization; manifest/package lock match. Original dirty checkout
preserved. No runtime/build configuration changes retained.

Real-input baseline/clean Normal and Slayer flows execute without errors or
provider writes: preparation, guide, Normal, Pause/Resume, four Space parries
(120points), menu return; separate clean Slayer expiry returns colored Normal.
Matched10s Normal p9526.8 ->26.6ms, not a general FPS claim. No new Build asset
requests after entering gameplay. First-complete-menu proxy independently
captured and inspected: full dungeon/title/four actions.

Acceptance gaps are explicit: both frozen baseline and clean show black Slayer
silhouettes over the dungeon instead of the requested red field. This is not
claimed as a visual pass or newly introduced regression. World loads once;
generic camera/RT markers appear three times. Separate read-only owner-ID P
diagnostic confirms one world camera/RT creation and the same world/camera/arena/
RT across menu, guide, gameplay and menu return; two extra RTs belong to separate
SampleScene cameras. Identity sources are archived/removed and all33clean hashes
remain unchanged. No rendering fix is included in this performance scope, and
the requested red-field visual acceptance is not claimed.
[Full results, attempts and retained evidence](../plans/startup-render-profile.md).

## Slayer B integration (2026-10-25)

Existing local Unity2022.3.25f1/WebGL module; independent snapshot/Library at
E:/CodexTemp/TypingHero/slayer-b-integration/unity-source. fly-server was offline;
no toolchain/package installation or changes to other running Unity projects.
serialize-retry.log: strict two-pass builder PASS. tests.xml:52/52PlayMode PASS,
0failed/skipped,6.210376s (20Slayer cases plus32directly affected regressions).
Covers original B white pixels/hash/native64x64 slices/Point/pivot/32–40px scale,
source80/100/100/100/100/100ms schedule, all56nonrepeat choices, all8angles and
21prefab progression tints, fixed contact/tint/rotation, role routing, pooling/reuse,
pause/reading/result/unload and lethal pre-damage capture.

QA WebGL build/strict repeat PASS;33player hashes saved, version9b813909fdd2dab1.
All C# inputs match the accepted worktree after LF normalization; package locks/
manifest/Unity version unchanged. Native Edge156.0.4314.42/Intel UHD/D3D11 at
1280x720:3153checks PASS,0exceptions/provider requests/writes; all8angles observed,
179stable lease samples and maximum8active Slayer effects. Real keyboard/mouse
covers overlap, live pause/resume, natural exit, restart, lethal hit and clean result.
All20emitted progression colors match the scene's authored white→gold→red ramp;
the scene overrides the prefab's cyan key. Both21-color static matrices inspected:
4032/4032cels,3360/3360within-effect pairs. Scene matrix alone2016/1680.
198/198selected encoded runtime frames and194/194pairs reviewed at original PTS,
including held frames; expanded crop encloses all observed B bounds. Browser video
clock check at1×/0.25× differs by<15ms over2.2/8.8seconds; presented callbacks may
coalesce held frames and are not the per-frame inspection evidence. No runtime,
source or asset changes after these passing checks; no broad suite/performance/
production release/itch claim. [Receipt and retained evidence](../plans/slayer-slash-vfx.md).

## Previous Slayer hit slash replacement (2026-10-24)

Unity2022.3.25f1/fly-server20261024T113433-webgl, input
`dc32bba70c576eaf074b00dc3e38f6ba9e675fe7ddf5f4f45c610270ee5d24f1`:
50/50scoped PlayMode PASS,0failed/skipped,8.8784652s; strict repeated builder
including new metadata PASS before tests and during QA WebGL build. Both roles
use original nine-cel tbbk CC0 artwork; source PNG SHA256 verified unchanged.
33/33immutable player hashes match local/server, version9b76edc5beddd984.
Native Edge156/GTX1650Ti/ANGLE D3D11/1280x720 real-input Slayer/rapid hits/
Pause-Resume/normal return/lethal ending PASS,0exceptions/provider calls.
72/72static cels/64pairs and407/407encoded output frames/403pairs directly
inspected, preserving held frames and original PTS. Production right-facing
runtime plus full static mirror/tint/duration matrix; no full animation,
performance, broad-suite or itch publication claim. Pilot fixture teardown and
late-pause driver failures remain distinguished from accepted evidence.
[Full receipt, limits and retained demo](../plans/slayer-slash-vfx.md).

## Production Brotli delivery (2026-10-24)

- Frozen main46d4dda, Unity2022.3.25f1: production104607 A,105711 diagnostic B,
  clean110109/input87b8f073c025a34cc1bac7580dee6cd397db4493eb20df6419974cc027f0f665
  PASS. Decode-aware production bridge guard and four raw/Brotli scanner cases pass;
  clean JS/WASM/data excludes ASCII/UTF16 QA/startup markers. No broad QA suite.
- Edge156/D3D11/1280x720, ABABAB+ABBA: five fresh/five reload per build per
  condition.20Mbps/50ms navigation-to-interactive-menu median14282.60 ->10377.70ms
  (-27.34%); HTTPS localhost4614.50 ->4411.80ms(-4.39%). Encoded bytes-39.92%.
  No timing screenshots/readbacks. Local longest-task and pooled menu p95 gates pass.
- Real-input A/B Normal10s p9519.70 ->19.50ms; world/camera/RT reused on return.
  Four real parries trigger Slayer in A/clean production. First complete menu,
  Normal/Pause, Slayer and return inspected; zero errors/provider writes.
  Decoded A/B executable/framework and serialized assets match, apart from build GUID.
- Production uses br/native decompression and requires HTTPS,Content-Encoding:br
  and underlying MIME(application/wasm). QA stays plain; editor settings restored.
  No Play asset deferral or rendering change. Public CDN inspection403; no itch
  publication or public-load-time claim. [Evidence and limits](../plans/startup-render-profile.md).

## Shared lantern shader startup optimization (2026-10-24)

- Frozen c86d72f production A/B + ABBA, Edge156/Intel UHD D3D11/1280x720:
  five fresh and five reload observations per build. Fresh world-to-background
  median 2337.01 -> 1829.30ms (-21.72%); per-run longest-task median 1931 -> 1409ms
  (-27.03%), largest observed 2373 -> 2014ms. Reload 761.81 -> 507.19ms (-33.42%).
  No screenshots/readbacks during timing; phase markers are diagnostic only.
- Pooled menu-frame p95 +4.74%fresh/+1.56%reload; matched10s normal gameplay
  33.19 -> 35.35ms (+6.51%), within10%. No general FPS/stress/soak claim.
- Fixed-light forward reflection fixture094239 passes direct alpha/emission/
  shadow/reflection comparison; repeated capture identical. Actual menu/normal/
  Slayer and Play -> gameplay -> menu inspected on A/B; world/camera/RT reused.
  A/B WASM/framework bytes match; only shader/material data differs.
- Clean integrated main3d6b74a production090837, input
  e59a86dcb2f4d9bf30c3965f3182f912dd6c5cabfe0fff736847f6e7381209f4,
  build/QA-startup marker exclusion/native real-input smoke PASS; zero errors or
  provider writes. Temporary diagnostic/fixture sources removed. No itch release.
- [Full diagnostic report](../plans/startup-render-profile.md) contains phase and
  raw-run evidence, source/build identities, excluded pilots and retained resources.

## Final current-main parry demo (2026-10-24)

Production043100/source4121c10/input92dc65f7 PASS, Unity2022.3.25f1. Includes the
upstream terminal-result HUD change plus parry lift depth registration; only docs
change after this frozen build. QA=false/JS bridge exclusion and33immutable local
player SHA256 checks PASS. Final menu, Play setup, live Swordsman combat/received
hit and manual Pause inspected in Orca using real input. Demo8151 retained paused;
retired kaluga8148 removed. No full animation/shadow/performance or new terminal-
result regression claim. [Version, attempts and resources](../plans/parry-main-demo.md).

## Latest-main parry demo and depth completion (2026-10-24)

Main9c48f5d + authored lift depth registration: production041305/input023fb6b8
PASS, Unity2022.3.25f1; normal QA=false/JS bridge exclusion and33downloaded file
hashes verified. Original failure was missing depth on the newly reachable lift;
only its map/import/binding/library/recipe added. No core, scene, shader or albedo
change. Font CRLF checkout corrected to canonical Git bytes, without a content diff.
Actual menu/tutorial/live combat/overhead white Space cue/Pause inspected on8151.
No new full animation, performance or broad-suite claim. Receipt, failed attempts,
limits and retained demo resources: [latest-main demo](../plans/parry-main-demo.md).

## Approved parry art integration (2026-10-24)

Test base origin/main9d41188; then merged latest main87aa6da (enemy warning/cue).
Parry runtime/controller/config and the tested player paths are unchanged by that
upstream merge; its warning behavior and verification receipts are retained.
No rerun or rendered-motion claim for the combined full player. Only success sprite keys, dedicated lift/normal assets,
asset builder and matching assertions changed. Runtime scripts, scene, controller,
normal parry, attack configuration and original character PNGs remain byte-identical.
Success remains950ms/800ms hold; defense250ms and existing70ms contact presentation,
queued follow-up, damage and cancellation contracts are preserved.

Unity2022.3.25f1/fly-server quick034322, input
`dcc4cdea6f01d00e6832d2087d82ef419c88fd49deb5a04c60f0fc95e025fb89`:
35/35PlayerParryTests PASS,0failed/skipped,0.9614194s. Covers seven sprite keys,
real Animator idle/direct follow-up exits, mirror,1x/.5x, pause, queue/deadline,
contact snapshot, interruption and damage-window lifecycle. Initial031703 had33/35:
two obsolete sprite-name assertions mistook recovery combo_2_end for the attack;
changed recognition to the actual Animator state, without runtime edits. One later
lock wait timed out before executing Unity; final retry passed. No new WebGL build,
performance measurement or full latest-main rendered-motion review claimed.

All seven cels directly inspected at native/3x nearest-neighbor and compared to the
user-approved local demo sheet: visible RGBA equal (transparent hidden RGB excluded).
Lift PNG also matches the approved bridge byte-for-byte. Both success clip events
and settings match main; source PNGs and asset GUIDs unchanged. Per-cel observations
below describe the accepted artwork, not a new redesign or a new motion-quality pass.

| Cel | Source | Hold ms | Observed drawing / next transition |
|---|---|---:|---|
| F0 | combo_3_1 | 800 | Low left blade, forward torso, both boots intact; F1 raises forearms/blade while feet remain planted. |
| F1 | parry_lift | 33.333 | Left horizontal blade, connected grip/forearms and waist; F2 rotates blade upward at the hands. |
| F2 | combo_3_0 | 33.333 | Original diagonal upper-left sword, small face/hair and connected shoulder; F3 straightens the chest. |
| F3 | combo_2_end_0 | 33.333 | Original raised rear arm, diminishing blade behind shoulder, two grounded legs; F4 lowers the forearm. |
| F4 | combo_2_end_1 | 16.667 | Scarf remains connected, original short blade trail, upright torso; F5 continues arm lowering. |
| F5 | combo_2_end_2 | 16.667 | Original residual trail, waist/boots remain intact; F6 settles the scarf and hand. |
| F6 | combo_2_end_3 | 16.667 | Original upright recovery, no detached limbs; Animator idle/direct queued-follow-up boundary verified by tests. |

No independent slash-effect variant was added. User approved the earlier playable
demo; latest-main visual scope here is exact cel preservation/static adjacency.
Transient review image and both task-owned remote test result folders were removed
after recording this receipt; original kaluga WIP and its playable demo are retained.

## Pillar visibility correction (2026-10-24)

Production030724/input8cfaf3ac PASS;2384source manifest hashes and33downloaded
player hashes match. Rebound fixtures from hidden legacy FrontPillar nodes to
actual PixelRoom/FrontPier meshes, outside their front faces. Native1280x720
production gameplay image directly inspected: both new flames and lit stone
faces visible, central actors/UI readable. Demo8333 retained paused. One targeted
visibility still only; QA suites/performance remain waived, no video.
[Checkpoint](../plans/combat-hanging-light.md).

## Pillar torches (2026-10-24; QA waived)

Two authored inner-face fixtures added at FrontPillar_-1/1 with independent
PillarTorch style (8intensity/10range/no shadows), cinematic SinePair phases and
shared original flame assets. Existing lighting lifecycle test expects5main-world
lamps; alternate four-lamp world retained. User explicitly waived QA;
task runner canceled while queued, before Unity work. Static diff/bindings checked;
no runtime, builder, visual, performance or production verification for this
revision. Existing demo remains the old build. Details: [checkpoint](../plans/combat-hanging-light.md).

## Cinematic wall torches (2026-10-24)

- Data-only change: RearTorch SinePair amplitude.18, frequencies5.8/13.7,
  weights.7/.3, phase factors1/2.37; two TorchBeam cards/update refs disabled.
  Unity2022.3.25f1/URP14.0.11, world320x180/presentation1280x720 retained.
- QA014122/inputf31be374: repeated builder/authored preservation and16/16focused
  LightingProfileTests/SlayerTransitionTests PASS,0failed/skipped. Native diagnosis
  26checks PASS; all7same-phase variants and11paired wall samples directly inspected.
  Beam removal fixes reported long pools; shadow/fog/blur toggles do not. No geometry,
  ambient, flame frame/timing or actor depth/normal change; no recording.
- Edge155.0.4283.62/Intel UHD/ANGLE D3D11,15s living-combat conditions on the same
  immutable player: baseline471samples/p95=33.39386ms; candidate460/p95=33.39005ms,
  ratio.999886.11checks PASS; no ABBA trigger, exceptions or provider writes.
  This is browser cadence and a relative torch comparison on pre-cue-HUD source;
  lighting/rendering sources remain identical after the reviewed HUD integration.
- Final4b57eb6-base QA/production015428 share complete input
  `8380b12e410423a53d5e234e96b9decca2d5206047a04510cd8424ea7099134f`.
  Final builder/authored preservation PASS;33QA and33production player hashes match
  remote.232exported lighting files match source; both beams/rune/light/hazes stay off.
  Current-player native lifecycle16/16PASS: real parry/follow-up, Pause/Resume,
  Slayer composite/continuous phase, received hit and menu/world return,0exceptions/
  provider writes. Initial fixture's wrong GameObject-hide assertion is retained
  separately; only that external assertion was corrected to actual composite behavior.
- Normal WebGL build PASS;48stripped assemblies exclude QA probe/depth demo/controls
  and TorchQaControl; JS bridge absent. Normal player menu/gameplay/pause inspected,
  F9 paused-canvas bytes identical,0exceptions/provider writes. No device-FPS,
  full animation review, stress/soak or publication claim.
  [Checkpoint and receipt](../plans/combat-hanging-light.md).

## Production sprite self-shadow integration (2026-10-24)

- User accepted demo8293 with rune/light and both blue haze layers disabled;
  scope is saved production configuration and integration, without full animation
  review. DungeonWorld selects ActorDepth; material saves0.65/0.15/0.01.
- Unity2022.3.25f1:49/49focused depth/F8/parry/Slayer tests PASS,0failed/skipped.
  Final-main QA182529/inputd0403f7:280sprites/289keys/34clips/32atlases validated;
  repeated builder, map/config/reference preservation and world/no-depth/return PASS.
- Native Edge/GTX1650Ti/D3D11,1280x720:18/18real-input integration checks PASS in
  38.25s,0exceptions/provider writes, no video. Parry->queued slash, hurt,
  Pause/Resume, Slayer/restore, F8 and menu/world return covered.204exported lighting
  files match source; all four rejected rune/haze objects remain inactive.
- QA and production183215 use the same complete input SHA256
  `d0403f7cf73aa00e821d13dd00d2a67955d1458ec565ac72611614739fd1ed8e`.
  Both WebGL builds PASS;33files per downloaded player match remote hashes.
  Production48assembly scan excludes QA probe/depth controls/demo; native production
  gameplay/pause directly inspected, bridge absent, F8 canvas unchanged,0exceptions.
- Historical visual/performance evidence remains scoped to its recorded source;
  unchanged probe sequence and lighting need no new measurement. Failed lock wait,
  port conflict and helper startup are recorded separately, not gameplay passes.
  [Checkpoint and receipt location](../plans/combat-hanging-light.md).

## Latest-main original parry demo (2026-10-22; local)


## Space cue demo, isolated review branch (2026-10-24)

- Immutable input00adef3c: QA172219, normal172820, Unity2022.3.25f1/fly-server.
  Two scoped Unity tests PASS: seven real Classic/sword/shield/axe attacks with
  pause/speed/thrust/contact/disable and crossfade cancellation. Strict repeat-builder
  PASS;39new scene objects,0old IDs removed; existing changes only tutorial text/parents.
- Native mute: completed640scope reused from first row of native-final2 (whole report
  retains its1280polling failure); fresh native-1280-final16checks PASS. Real Space
  fill/early/late/locked, pause/F8, typing error/timeout, different9-letter queue at
  both sizes, Slayer/win/next duel/restart/menu;0exceptions/production writes.
- All194selected capture frames and191adjacent transitions directly inspected,
  including holds/repeats.7.76s/25fps clip full decode/count PASS. Normal smoke2
  five checks plus actual menu/tutorial/combat/pause/mode/menu image review and
  visible cue proof; qa=false, five JS/WASM/data payloads exclude ASCII/UTF16 QA markers.
- No full-suite/balance/stress/performance or human-attention claim. Fixtures block
  external writes. Exact sources, valid/failed receipts, timestamps, clock-calibration
  caveat, clip hash and retained resources: [checkpoint](../plans/parry-cue-demo.md).

## Pause and redundant guidance cleanup (2026-10-24)

Unity2022.3.25f1 result20261023T174743-webgl/input
d90abc079611723cb0556589b74770f96563c3e29dee9a94cc796a4ca6be0922:
12/12 scoped PlayMode PASS,0failed/skipped,10.564191s; strict repeat builder and QA
WebGL PASS. Native Edge/ANGLE D3D11 at1280x720:22/22 real-input checks PASS,
0JS/managed exceptions, fixture interception/external blocking enabled.
Nine screenshots inspected: unframed Pause, four-action menu, preserved tutorial,
Settings/results, clean combat and Continue without duel numbering. Tutorial entry/
replay, pause/modals, direct battle start, typing defeat/Continue, Restart/death/menu
verified. No balance/performance/sprite suite or itch publication.
Source scenes semantically match generated records; retained original ordering and
unrelated values/GUIDs. [Receipt and resources](../plans/compact-menus.md).

Base origin/main fd3f21d; original2->3->4->5->idle integration on a separate child.
Pinned Unity2022.3.25f1/fly-server141745: strict builder/font protection and31/31
scoped parry/contact/slash cases PASS,0failed/skipped,2.4495452s. WebGL build PASS
on e17c70958e4e7cf0ac08e71d65ff7c03293c623fe23bf5eea878717eed6a7382.
32downloaded player file hashes match. Native Edge155/Playwright1.58.2/Intel UHD
D3D11,640x360:17/17PASS,15.865s, two real parries/two words/one10HP follow-up,
pause/resume and no automatic no-word attack;0exceptions/provider writes.
Dense frame/direction/speed/boundary coverage is Unity, not browser. Local demo
only; original pixels retained, source contact table matches tested generated
asset. Detailed failed attempts, font CRLF diagnosis, evidence limits and retained
resources: [checkpoint](../plans/parry-main-demo.md).
## Reusable world lighting profiles (2026-10-23)

- Pinned Unity2022.3.25f1 / URP14.0.11:20/20 scoped profile/lifecycle/lighting checks,
  independent native fixture registration2/2, camera-main integration19/19, final
  light-only/fixture/lifecycle4/4;0fail/skip in these accepted runs. These overlap;
  they are not one combined suite. Edited light/size/material/post values survive
  two complete builder applications. A-to-B-to-A validates arbitrary fixture names,
  four lights, independent style/fog and256x144 presentation without leaked UV/RT state.
- Final input c25b03c0040fe852621521f45b6d41d93354f427a77f2e7f6802252e2c425626,
  QA20261022T165927-webgl: WebGL and strict repeat builder PASS. Native player
  a484cfa7f3a82e38, Edge155 / GTX1650Ti / ANGLE D3D11 /1280x720:34/34 actual-input
  checks,0exceptions/provider reads/writes. Lamp bounds/emission/phase, Pause,
  parry/following attack, Slayer/restore and menu pass.
- Direct final visual review108/108selected encoded frames,106/106pairs in28native/2x
  sheets. Colored local fire, fixed fixtures, warm3D surface pools and readable
  player recovery pass. Earlier palette migration had zero thresholds; corrected
  to original .03/.12/.3 before final player. Earlier palette acceptance is superseded.
  No full suite, extra viewport, stress/soak or general performance claim.
  Exact inputs/counts/segments/limitations and retained evidence: [checkpoint](../plans/combat-hanging-light.md).
  Usage: [profile authoring](lighting-profiles.md).

## Combat camera impact (2026-10-22)

- Integrated input `d0fede39b9c7db8c593c3ae618f256b4888a25f1a163df390e0389c573ccb4d1`: pinned Unity2022.3.25f1 focused32/32 PASS,0failed/skipped (`20261022T160203-quick`). Confirms real hit consumers, bounded priority, channel isolation, projection/foot anchor, pooled-event ownership, warmed request allocation and Pause/dungeon lifecycle behavior.
- Final WebGL/strict repeated builder PASS (`20261022T160608-webgl`);33/33 immutable player file hashes match local/server. Native Edge155 / Playwright1.58.2 / GTX1650Ti D3D11:86/86 checks PASS across1280x720 and640x360,0exceptions/provider calls, real keyboard/mouse and fixture/external-network isolation. Camera profiles, actual kick bounds, fixed HUD/typing and Pause/Resume/Restart/Menu pass.
- Directly reviewed400/400 encoded frames and390/390 transitions in ten selected output windows,37/37 sheets; framing and recovery pass. Original WebM/PTS remain timing authority, including held spans; derived MP4 tails are not authoritative. Reference research remains separate. No full-suite/stress/soak, general performance or itch release claim. Sources, limits and retained demo: [camera checkpoint](../plans/camera-impact.md).

## Impact profile authoring follow-up (2026-10-23)

- Final source snapshotcbff319fd0c09a9cb26c5f4b5baeafb6d656f2e5e2640ad567588c16247420d8:
  **85/85 scoped Unity PASS**,0failed/skipped (`20261023T035820-quick`), all2229
  input files match the pre-integration task checkout after text-only LF normalization.
  Filters: ImpactVfxTests, ParryClashTests, PhysicalParticleTests and three directly
  affected PixelSpark parry direction/teardown/unload cases. Covers pure generic
  prefabs,0/1/3 flashes/arbitrary IDs, varied flipbook/color/physics/pixel scale,
  public parry/profile.prefab routing, immutable values/curve keys, concurrent
  pulses, bounded pool/pause/reuse/cleanup and zero warmed particle-update
  allocations. Invalid IDs/overlapping roles/>40 states fail before leasing.
  Includes active prefabs without an authored lease, and repeated custom-reference
  migration. Earlier83/83 and supplementary builder4/4 receipts remain attributed
  to their own snapshots, superseded by this complete focused85-case result.
- Independent frozen pre-refactor origin/main3253274 fixture compares every
  particle on every renderer over68 consecutive1/60s steps, both directions and
  seeds0,7. JSON SHA256079ac532a82ecc8d521993391f19f5974fe10a22fc751956e3415f2e477f3360;
  gzip TextAsset fixture is under PlayMode tests. Solver and physics asset unchanged.
- Normal/mirror WebGL + strict repeated builder PASS (20261023T034250 and034946).
  Native Edge155/Playwright1.58.2/NVIDIA GTX1650Ti ANGLE D3D11:36/36 PASS,
  0exceptions/provider calls. Real parry plus typed queued slash, HP preservation,
  contact/core/damage/audio-source alignment and pool caps pass. Frozen baseline
  builds/native36 also pass after correcting the external driver's superseded
  automatic-counter assumption; main requires real typing for the queued follow-up.
- All242 output frames/240 consecutive pairs directly compared with all302 baseline
  frames/300pairs, preserving native render timestamps and held frames, no gaps.
  Per-frame/transition notes in visual-review.csv; total544frames/540pairs. Both
  directions retain blade-centered contacts, narrow directional launch, orange1px
  fall/rest/fade, clear endpoint and coherent guard/queued-slash recovery. Compare
  by effect age/hero phase: cadence, words and encounter labels vary. No whole-scene
  bitwise, FPS benchmark, or independent subjective-audio claim.
- Visual players predate the final active-prefab missing-lease guard and Editor-only
  custom-reference migration. Reuse is limited to unchanged MetalClash rendering:
  its prefab already has PooledVFX, so the new missing-lease branch does not execute;
  its authored profile/materials/physics/render code are unchanged. Its new player
  reference equals the already-built serialized value. Final85 tests cover the
  changed generic ownership/migration paths and rerun frozen-trace/pooling tests.
  Native coverage of a generic active prefab missing a lease is not claimed.
- All33 files of each of four immutable players match local/server SHA256 hashes.
  Generated baseline/output assets differ only in the intended MetalClash profile,
  prefab bindings and player profile reference; unrelated builder normalization is
  not copied into source. [Checkpoint](../plans/metal-clash-preset.md) records handoff.
- Artifacts/reproduction: E:/CodexTemp/TypingHero/impact-vfx-authoring. Full suites,
  broad WebGL profiles and stress runs are outside this focused change.

- Integrated newer main e491984 as cb4c84e. VFX runtime/profile/prefab/player inputs
  remain identical to task1590347; the shared builder preserves upstream authored
  camera impacts and the custom impact-profile migration. Focused integration
 12/12 PASS (`20261023T040800-quick`, f4fa6f0c3fa360570ac856c537e686b01cbec31d27cb0346094c53848b0b08d6):
  frozen trace, generic lease expiry, custom builder refs, public parry route,
  pulse snapshots/pause/weak overlap, selected-world/camera-profile rebinding and
  lantern lifecycle. Upstream39Unity/53native/world-frame evidence stays attributed
  to its own checkpoint; task visual receipts remain pre-integration snapshots.

## Reusable MetalClash preset (2026-10-22)

- Integrated newer main5f371fa torch flames as7ef0da3, preserving all VFX/actor/
  ParryClash/SampleScene inputs exactly. Integration145953 input1eb1cbe4 passes6/6
  focused pulse/pause/pool/adapter/voxel-overlap/torch-lifecycle checks,0fail/skip.
  Feature and merge commits verified ancestors of remote main7ef0da3. Existing
  images below remain qualified by frozen inputs; no new torch-art claim.
- Focused Unity2022.3.25f1/fly-server:72/72 PASS,0failed/skipped,
  run20261022T144224-webgl input4e59bdb41d6d22a5c113353f270154ad746213e95691f084dc9d2ff4f9a50c8f.
  ImpactVfxTests + ParryClashTests + PhysicalParticleTests only, using the existing
  external filtered runner and shared exclusive lock. Standalone emitter, two
  concurrent profiles, color/physics/fade/pulse snapshots, actual local light,
  seed/recipient/both directions, pool cap/expiry, pause/reuse/cleanup and zero
  managed allocations in warmed render updates pass. Initial70/70 and71/71 passes predate
  the final builder-reference test and inactive voxel-light range assertion; final72
  includes them. No full-suite/performance claim.
- Solver/profile, approved physical asset and materials unchanged from916c752;
  existing prefab/script GUIDs retained. Final normal144224 and mirror144950 WebGL/strict
  repeated builder PASS; both32-file player hashes equal server artifacts. Native
  Edge/Playwright1.58.2/Intel UHD D3D11 at1280x720:36/36 real-input checks PASS,
  0exceptions/provider reads/writes. Direct162/162output frames/160/160pairs versus
  128/128reference frames/126/126pairs retain contact, thin spray, orange floor tail
  and expiry. Per-frame notes/timing/scope and artifacts: [preset checkpoint](../plans/metal-clash-preset.md).

## Separate tutorial HUD callouts (2026-10-22)

- Replaces the single shared background with four independent existing-style cards beside HP, enemy HP/guard, score/combo and typing/timer. Copy, font, HUD, input/lifecycle/storage and provider contracts remain unchanged. Transparent shared root; existing Start control/listener retained.
- Input `a0f2690bf90397d5fbccf510d5b01502acecbe9eb2703140b1ebd7b73f65bc3f`, run `20261022T134816-webgl`: pinned Unity2022.3.25f1 build and strict repeated builder PASS. Original1005 scene IDs retained; tutorial168?184 blocks (four new cards),1021 total. All837 non-tutorial blocks remain byte-identical; local references resolve. Projected scene SHA256 `47c0d23ce881d5eb1be7192c84c45dc501d114971380456e05213795c3828b7e`; tutorial matches final serialized builder apart from empty-field trailing whitespace.
- Same immutable player,32/32 local/server file hashes equal: native Edge155.0.4283.62 / Playwright1.58.2 / Intel UHD ANGLE D3D11 **26/26 PASS**,0exceptions,0provider reads/writes. At1280?720 and640?360 all glyphs fit, four cards clear each other and real HUD, and five arrow tips touch targets. Keyboard/mouse, frozen reading, Pause/Resume/Restart/Menu, held Enter, completion persistence/bypass/replay and typed battle hit pass. Guide images at both sizes, pause, fresh intro and live hit directly inspected; no cloned guide cards during intro/live combat. Fixtures and external blocking retained.
- Software-renderer run remains FAIL after15checks: missed menu Play at184.63ms/frame. Its two-size layout checks passed; no software success claimed. Native run used the exact same player and unchanged strict assertions. No balance, sprite, full-suite or itch.io upload scope. Details and retained review demo: [checkpoint](../plans/tutorial-hud-guide.md).

## Single HUD tutorial guide (2026-10-22)

- Unity2022.3.25f1 / fly-server:7/7 scoped TutorialTests and DungeonEditorEntryTests
  PASS,0failed/skipped, input `0c8b3a88988177676262b280baf67aed56b21a66f67c1089ab4c740c6a205844`,
  run101452. Covers existing viewed values, abandon/confirm, reading input/time/audio,
  pause/resume/restart/menu, repeated confirmation, provider guard, fresh dungeon
  lifecycle and no cloned guide copy/background in intro/live combat.
- QA103149 on the same input: WebGL build and strict repeated builder PASS after
  recreating only the generated GameAssembly.a archive to resolve stale duplicate
  IL2CPP symbols. No runtime/config/dependency change or weakened build threshold.
- Edge155.0.4283.62 / GTX1650Ti / ANGLE D3D11: final26/26 real keyboard/mouse checks
  PASS at1280x720 and640x360,0JS/managed exceptions,0provider reads/writes. Fixture
  interception and external blocking enabled. Strict text glyph containment,
  HP/guard/typing clearance, five real-RectTransform arrow tips, unread menu exit,
  pause/resume/restart, held Enter consumption, persistence/replay, selected Hard
  fresh intro and actual typed hit (score10) pass. Both guide sizes, pause, clean
  intro and live hit captures directly inspected. All32local player hashes match
  the immutable server player. This is scoped functional/layout acceptance.
- Software browser remains FAIL:15checks then a missed menu Play input at roughly
  193ms/frame. Earlier5check glyph-bound failure, preflight-capacity failure and
  stale-link failure are recorded in the checkpoint; none are reported as passing.
- Committed SampleScene contains1005IDs:925original IDs retained plus80blocks for
  20guide/arrow nodes. All168tutorial subtree blocks match the final builder;
  non-tutorial blocks remain byte-identical and local references resolve. Only
  blank lines were removed from one pre-existing malformed DungeonWorld GameObject
  to allow direct-source loading; every field/ID is unchanged. MainMenu, fonts,
  art, prefabs, How to Play, gameplay/provider APIs and TutorialProgress_v1 remain.
- No itch.io upload. Source/cleanup/integration record: [checkpoint](../plans/tutorial-hud-guide.md).

## Player recovery lighting / reflection removal (2026-10-22; playback approved)

- Root cause: actual player controller references the parry-success clip outside
  the folder scan. Four combo_2_end cels lacked both companion bindings. Matched
  48-state native A/B shows normal-flat/shadow-off do not remove the blackout;
  metallic0 restores the same recovery cel. Controller-derived map preparation
  fixes the missing bindings; shader, lights, original artwork and combat unchanged.
- QA20261022T044230/input3baf37bc3fce07853acb13b608f2b129dbbe6fd12143b20a66823c5c3d40024a,
  playerb2723a4a20fe5423:strict twice-run builder/WebGL PASS. All1928scene record
  contents/references match local result (serialization order differs). Five binding
  assets match immutable build properties/PNG payloads (new meta whitespace normalized);70actual-controller keys now have both maps;
  18original albedo/clip hashes unchanged and new GUIDs unique.
- Scoped Unity20261022T045507:9/9PASS,0fail/skip, including actual-controller map
  dimensions/bindings, environment lifecycle and parry70/175/280ms/pause/reuse.
  Final native r3:17/17real-input checks,0exceptions/provider reads/writes. Attack,
  received hit,consecutive parries,attack after parry,pause,Slayer/restore,menu passed.
  All reflection/capture-camera samples0;last menu world/reflection/scratch/cameras0.
- Final gameplay source563frames exported with exact PTS; actually inspected113
  frames/111pairs at native and nearest2x:153..213 (20.593..25.295s),234..285
  (26.505..29.970s). Recovered cels retain face/armor/waist brightness through pulse
  fade and return to idle. Whole-scene attack/parry/post-parry/restored stills also
  opened: no reflected figures/lamps;floor light and ground contact remain.
- Failed runs retained separately:one cold Edge WebGL-context loss at menu;one
  driver waited for immediate HP loss rather than observing the actual attack
  state and timed out. r3 observes correct-word attack animation directly; no
  gameplay change. First test transport truncated before Unity; retry hash checked.
  Disk failures did not execute tests or count as PASS. No performance/FPS claim.
- Supplementary no-VFX matrix:8cels x16variants actually inspected at native/2x,
  including atlas switch and both facings; no recovery blackout. User subsequently
  narrowed verification to the reported defect:additional VFX fixture build stopped,
  its inactive-clone captures are not accepted. No further broad QA required.
  User approved1x/quarter playback and requested main integration. Shared-world
  integration preserves27lighting records and main's game/menu scene contents;
  static references/Transform/reflection flags audit PASS. Additional filtered Unity
  attempt stalled before tests,not PASS;integrated runtime/builder not rerun.
  Approved defect gameplay evidence retained; no publication.
  [Checkpoint](../plans/dungeon-3d.md).

## Rune noise completion candidate (2026-10-22, acceptance pending)

Scoped DungeonRenderTests run20261022T015303-quick:5/5 PASS,0fail/skip,
Unity2022.3.25f1. Actual property-block/light gain checked over24s, bounds.76..1,
deterministic seek and gradual variation; pause/hide/restore preserves gain,
cloth and exact particle state. No serialized asset changes/duplicate GUIDs.
QA20261022T015412/player8d537e103ab97c01 passes WebGL/strict repeat builder;
all1928scene record contents match. Final native real-input gameplay-r2 passes16/16,
including LAST post-menu sample(world/reflection/scratch/cameras all0). Matched30s
ABBA RAF p95 A1/B1/B2/A2=13.445/13.525/26.735/26.685ms,mean draws~331 throughout;
both late runs slow similarly,not isolated GPU/FPS/equivalence evidence.
Native moving-light777selected frames/776pairs viewed at logical-native size;
247-key static evidence retained by605-input hash audit. Direct Canvas quarter
now logs247/247 with matching capture duration;48consecutive attackS3/context
frames supplement missing native key61. Direct Canvas gameplay586/586frames
viewed; legacy pause panel appears8.102..8.360s,so visual/UI acceptance remains
open. QA-only isolation build20261022T030011/player3ed75542c5ec83bf succeeded with
production source/art unchanged. All30A/B stills,261noise/camera frames and
134selected new-A/B frames inspected; exact source PTS retained. Low VFR cadence
limits high-frequency shimmer assessment. User playback approval remains pending.
No full visual PASS or main integration.
See [checkpoint](../plans/dungeon-3d.md) for source identity and receipts.

## Dungeon full-effects candidate (2026-10-21, acceptance pending)

- Focused DungeonRenderTests140215:4/4PASS, including exact particle count/time/
  position/lifetime through pause and hide/restore, pinned cloth and owned world RT.
- Final QA153315/input75e96989e28a55bfae019e71c6490fec05722c841c86bcfbb2aade776f50186b:
  strict twice-run builder and WebGL PASS; player0ecd1b6a6c221b30. Native Edge155,
  GTX1650Ti/D3D11,1280x720:15real-input/render checks PASS,zero exceptions/provider
  reads/writes. Attack,received hit,parry,pause,Slayer/restore,menu and RT cleanup.
- Static sprite review:247authored keys x16variants,219consecutive pairs +9loops
  per variant, native+nearest2x dark; all247keys also checked on bright background
  at native scale. Original albedo,animation timing,slicing,pivots and GUIDs retained.
- Independent A/B26states and all nine camera stills inspected. Selected camera
  video22.000-38.080s exported without resampling and checked frame by frame:
  402frames/401transitions,including holds; full video952frames/timestamps match.
  Real-time1x/quarter playback perception remains unverified; user review requested
  because available image tools only display static frames. Do not mark visual PASS.
- Matched pre-integration ABBA combat RAF p95 A20.135/20.270ms,B20.220/20.585ms;
  same GTX1650Ti,Edge155,1280x720,input driver. Draws266.8->331.4; triangles~12421->14417.
  Browser wrappers are observable submitted load,not GPU timings. No universal FPS claim.
- Evidence and review.html retained under E:/CodexTemp/TypingHero/dungeon-full-lighting.
  No game publication. Integrated main f7bfc67 in87eb373; QA160528/input47af0723,
  player2ac005eb7f4b60d3: repeat builder/WebGL PASS,16native checks PASS,
  zero exceptions/provider effects, newer Play/preparation and compact pause verified.
  Focused Unity162136/input1272a6a7:4/4PASS. Scene1928objects and main menu482objects
  unchanged by builder (only trailing blank line differs). Real-time playback and
  remote-main integration remain pending; task branch is a verified technical candidate.

## Dungeon and thin-glass lantern integrated (2026-10-21)

## Shared3D dungeon lifecycle ? 2026-10-22

Editor direct-play follow-up: Quick062143 on Unity2022.3.25f1/fly-server passes
2/2 actual EnterPlayMode tests and25/25 filtered PlayMode tests, zero failed/skipped,
on input `c6f931fa01cfc9228a58914d327cc2e205eae229f84579d409226db9f24eb931`.
Actual Play twice with domain reload disabled retains unsaved Inspector edits,
resets transient run/tutorial state and exercises Fight/ProcessInput/pause/Restart/
menu callbacks. Mixed MainMenu+SampleScene startup stays actor-free; direct entry
reuses an open world and retains camera/RT across reload and menu Start. Prior22
checks rerun with three new direct-entry cases. Nine source/scene/editor-setting
comparisons match the immutable input after testing. Headless controller/input-path
evidence; no new browser/physical-keyboard or visual/performance claim. Scenes,
builders, packages and project settings are unchanged. Receipts/diagnostics and
retained task artifacts: [checkpoint](../plans/shared-dungeon-session.md).

Unity2022.3.25f1/fly-server Quick041809:22/22PASS,0failed/skipped: real Slayer teardown
across3cycles, binding/nativeCanvas, actor roots, duplicate transitions, rendering,
identity15and3directly affected HUD/damage/Continue consumers. QA042021 strict
repeated builder PASS on identical input
`1dc641a5e34fb745b3a71ef8d5ef913c4dbcd17fc6cf49073ac05e531283cc8d`.
All3serialized scenes/world.meta/build settings match final checkout;96original
menu RectTransforms and old scene GUID/meta unchanged, no dangling local references.

Edge155/GTX1650Ti/D3D11/DPR1 native129/129PASS at1280x720/640x360; focused fresh
Play/Unseen/Skip/Fight/return640supplement7/7PASS. Actual menu/preparation/modals,
tutorial/next lesson, win/Continue, Slayer/restore and unobstructed640combat images
inspected. Real keyboard/mouse only. After required-flow warmup,3identical
Menu->Start->Restart->Menu cycles remain594GameObjects and1dungeon RT; room/camera/
RT IDs fixed across all flows, camera/presenter/listener1each. Menu has0actors/HUD/
timer, world restored, combat light0/disabled and time scale1. Both final reports
0Unity/JS exceptions/provider writes;2Endless reads intercepted, external blocking
and observation-only conditional bridge retained. No full gameplay/stress/soak/
performance or itch publication claim. [Checkpoint](../plans/shared-dungeon-session.md).


- Remote main verified at f1ab52b, retaining latest main gameplay/HUD/Pimen systems
  with the task's full dungeon3D/camera/lighting/lantern. No game publication.
- QA114427/input9a9081df02bf07286e197c4cf9ec1e5c3f0c49008fbc838b46fc8d47455dbc97,
  playerfdc8253ed7a62b1f: WebGL and strict twice-run builder PASS.
- Edge155/Intel UHD/D3D11/headed1280x720:25real-input checks PASS; actual attack,
  hit/parry,pause,Slayer/restore, three steady sources, glass fixed emission and
  hide/restore. MainMenu return releases all owned3D RT/presentation cameras.
  Seven state/menu images inspected;0exceptions/provider reads/writes.
- Six selected offline contracts PASS: world RT/projection lifecycle, fixed key,
  bounded rear phase/emission, parry pulse, HUD lifecycle and HP fills0/1/50/100%.
  Initial HP test expected obsolete231-unit rail; corrected to main's approved171,
  preserving exact size/fill assertions, then rerun PASS. Failure XML retained.
- Main gameplay/UI/data/prefab/MainMenu files remain identical to main; UI source
  changes only camera-coordinate adapters. All existing main GUIDs retained.
  Final serialized scene/actor/UI/VFX outputs match source after whitespace
  canonicalization; no runtime input difference after the final main merge.
- Prior isolated glass HDR7stills/9camera poses/163frames162transitions and20s
  timing remain scoped evidence, with source-identity proof. They are not an
  integrated performance/FPS claim. No full suite or production build.
-28Unity receipts archived locally before deleting the isolated remote task cache;
  shared Unity home/toolchain preserved. [Checkpoint](../plans/combat-hanging-light.md).

## HUD integration with current main (2026-10-21)

Merged HUD8179a0c with maindbe663f (approved3D arena/parry/combat). Two conflicts
resolved by retaining the main arena build and objects plus the final HUD pass.
Production082906/3fce8615 strict repeat and normal bridge exclusion PASS; focused
combat/damage4captures320/640 PASS with0JS/shader errors/external requests. Actual
3D images inspected;169UI RectTransforms equal the approved HUD, score interior
pixels identical at320/640, and all671new main arena objects unchanged.1545scene
IDs; no full suite rerun. [Receipt](../../design/hud-dark-fantasy/main-integration.json).
Earlier local8245/ZIP receipts below retain the approved2D preview; the integration
artifact is preserved separately as recorded in the checkpoint.

## Uniform side HUD scale — local only (2026-10-21)

Production075801/input0f97006d: strict repeat and production QA exclusion PASS.
Six original-HTML combat/damage captures at320/640/400x300 DPR1 PASS, zero JS/shader
errors/external requests. Actual images inspected: both HP digit labels hidden,
both side status groups/names/icons/stamina at80%, outer corners maintained and
fills continue updating under damage. Score region pixel-identical to77f90925 at
320/640. Complete serialized scene audit retains874IDs and all fields outside
explicit HUD changes, including the entire score/combo cluster and typing tray.
Stamina highlight serialized scale also80%, matching its existing TransformPoint
follow contract. No gameplay code edits, full tutorial/suite or upload.
Latest player/ZIP/hash: [delivery](../../design/hud-dark-fantasy/delivery.json).

## Compact health HUD follow-up — local only (2026-10-21)

Production070959/input77f90925: strict repeated builder and normal QA exclusion
PASS. Four original-HTML combat/damage captures at320/640 DPR1 PASS, zero JS/shader
errors; actual screenshots inspected. Enemy HP numbers and difficulty/DUEL line
stay hidden, player HP35/40to20/40 and its fill update correctly, rails102to82px
retain their height/corner anchors and sliced rim pixels. Complete scene audit
permits only these explicit geometry/visibility changes beyond the prior palette;
all874IDs, fonts and bindings retained. No gameplay code changes or full suite.
Latest local ZIP/hash: [delivery](../../design/hud-dark-fantasy/delivery.json);
[before/after](../../design/hud-dark-fantasy/compact.html). No upload.

## Dark fantasy HUD — local only (2026-10-21)

Source56433a4 on `micharel09/cobblestone-itch-release`. QA062108/72528e35 strict
repeat PASS; native focused11 checks/30.014s on Edge155/GTX1650Ti D3D11 at320/640
DPR1, actual normal/combo/low-HP inspected,0exceptions/provider writes. Final
production064056/eb36b3b1 includes coal tracks and pause palette; separate strict
repeat PASS,5JS/WASM/data payloads exclude QA markers, qa=false and original HTML
production combat/pause4 captures plus resume PASS with0JS/shader errors. Final
production screenshots inspected;874IDs and normalized complete scene prove only
palette/sprite/two HP material bindings changed, menu unchanged.26PNG dimensions
and alpha unchanged, text>=4.5:1 with hard HP coal underlay, resources>=3:1.

The user explicitly limited testing to changed parts: no final full gameplay,
tutorial, difficulty, DPR matrix, stress or soak rerun. Prior evidence remains
scoped to unchanged behavior; final material is visually verified atDPR1.
Local ZIP30members/16,983,695bytes, CRC PASS; receipt, paired native/nearest previews,
rejected candidate history and resources: [checkpoint](../plans/hud-dark-fantasy.md),
[delivery](../../design/hud-dark-fantasy/delivery.json). No itch.io upload.

## Typography role synchronization - local preview (2026-10-21)

Final94c4adbd: QA035043/production040103 strict repeat/bridge exclusion pass, matching serialized scenes/fonts/prefabs and unchanged complete typing components/non-text serialized scripts/scene IDs. Source fonts/prefabs unchanged. Native698checks plus259ink-clearance checks pass, four sizes and DPR1/2/1.25, fixture/blocking and zero real provider writes. Original-production HTML52viewport-state captures pass, including defeat/results; actual full live cue and wrapped COMPLETE title inspected. Hierarchy56/48/42;20native-pixel name row clears stacked accents, HP/round stacks shift9pixels, score/typing fixed. No full balance/performance or publication claim; previous public build unchanged. Source/limits/resources: [font checkpoint](../plans/itch-game-page-dark-fantasy.md).

## Cobblestone UI public release (2026-10-21)

Approved main b36b55a plus supplied Cobblestone UI/typing, Blackbit title only, hidden PARRY/key/clock. Exact input640a9871: QA020049 repeat-builder and complete typing behavior guard PASS; production021408 QA exclusion PASS. Native scoped633checks across four viewports and DPR1/2/1.25; six lessons/parry/Slayer/pause/restart/first win/Continue and current score cluster pass, Firebase fixture/blocking and zero real writes. Normal player48viewport-state captures pass; public Run game actual menu plus8native CDN combat/pause captures and Space pose,29payload hashes exact. Upload19915823 selected after native Save/reload;19893288 retained hidden for rollback. No full balance/performance claim. See [release receipt](../../marketing/itch-io/cobblestone-release-20261021.json) and [font checkpoint](../plans/itch-game-page-dark-fantasy.md). Older all-UI Blackbit font-quality conclusions are superseded by the user-approved Cobblestone choice; their behavioral evidence stays scoped to its frozen source.

# Verification and performance memory

## Duel first-word preparation (2026-10-26)

- All Unity execution on Ubuntu fly-server, pinned2022.3.25f1/2CPU/8GiB. Final
  focused DuelIntroTests6/6PASS,0failed/skipped; strict repeat builder and QA build
  pass at word-preview-20261026/20261026T064955-build under the shared runner root.
- Source7b86815e04628556532d9c7b077c8dd3bbcc0b84ddf6e2dcf6f59935564ea6bf;
  281C#/shader/package/version inputs match the worktree afterBOM/CRLF normalization.
  Serialized build-copy LF normalization resolves an unchanged font byte guard;
  no repository assets/guard edits. Final player7a03a2f20df3f514,33file manifest.
- Edge156.0.4314.42/NVIDIA GTX1650Ti/ANGLE D3D11,1280x720:65real-input checks PASS,
  0exceptions/provider IO. First-word preview/color/timer/input locks, held keys,
  Continue/retry, pause/focus, Menu cancellation and tutorial reading covered.
- All566frames/561adjacent pairs in five complete scopes directly reviewed at
  original25fps PTS, including held frames/page overlaps. Pilot label-over-cover
  visual failure corrected; final closed cover hides the heading. Existing F6
  Spark debug label is automation-only. No new FPS/audio/full-suite/release claim.
- Receipts, original video/ledger and84/42frame Play/Continue excerpts:
  E:/CodexTemp/TypingHero/duel-word-preview/ . [Checkpoint](../plans/duel-intro.md).

## Impact pulse emission clock (2026-10-23)

- Base95f978b. Red20261023T070541-quick reproduces1/1 expected failure: an actual
  >280ms receiving frame emits at Update205 before rig210; old light intensity is0
  instead of5 before that frame completes. Source15af9a4c9e0839520aed31b92a725fbd3ca56917c5fa3086e777945b93ac7eb5.
- Final inputa708356bb9940acb20eabc5b67470a58e4a83164fa7baedc6834b0a30e7f2fac,
  Unity2022.3.25f1/fly-server20261023T071650-quick: **44/44 PASS**,0failed/skipped.
  Includes13new clock cases and the31affected impact/parry/Point-Spot checks.
  Runtime and deterministic tests share timestamped emission/sample/cancel paths.
- Covers before/after rig Update, one real320ms receiving-frame stall, subsequent
  stalls expiring normally, exact MetalClash hold/midpoint/expiry, pause-event fractions,
  direct scale0 resume, positive-scale hitstop, repeated samples, expiry/current
  intensity before overlap, two independent rigs and disable/unsubscribe/re-enable.
  Clock sampling and pulse-only burst waiting allocate0managed bytes after warmup.
- First fixed pilot20261023T071159-quick passed41/44. Three existing fixtures assumed
  no wall time elapsed during construction or sequential emission. They now use the
  timestamp seams for simultaneous/precise samples; intensity/timing/owner assertions
  are preserved. Full final44case rerun passes; no threshold was relaxed.
- Frozen MetalClash particle trace and seed/mirror/snapshot/pool/color/unload regressions
  pass. Assets/GUIDs, particles, solver and profile parameters are unchanged. Headless
  script order and Light state are verified; no native WebGL/rendered peak or new
  animation-frame inspection is claimed. User chose authored expiry after emission,
  rather than extending the pulse to await a render.
- Immutable source/results and external filtered runner remain at
  E:/CodexTemp/TypingHero/impact-pulse-clock for audit, disposable when no longer needed.
  Delivery and scoped cleanup receipts are stored there after integration.
  See the [preset checkpoint](../plans/metal-clash-preset.md).

## Impact pulse lifecycle follow-up (2026-10-23)

- On base9acdb4e, focused red run20261023T062644-quick reproduced both findings:
  a0.2s particle/flash burst released before its1s light expired, and disabling a
  cyan2D pulse retained cyan instead of the authored base color (2/2 expected failures).
- Fixed snapshotb5bf5c663fa93a77aa4ff0105f322f8eaf22665c00a34e913bbb7ae3598a6615:
  Unity2022.3.25f1/fly-server20261023T062925-quick **30/30 PASS**,0failed/skipped;
  same-input20261023T063139-quick **1/1 PASS** for the shared Point/Spot arena route.
  Includes7new lifecycle/color/allocation regressions, frozen MetalClash trace,
  seed/mirror/recipient, concurrent snapshots, parry overlap/pause/reuse and unload.
- Pulse-only waiting allocates0managed bytes after warmup. Missing/disabled rigs,
  rejected/evicted pulses and explicit cleanup do not retain leases. Runtime uses
  the rig's existing pulse clock; no new profile field or duplicated timer.
- Render evaluation, solver, assets/GUIDs and approved MetalClash parameters are
  unchanged. Prior visual receipts retain their original attribution; no new
  WebGL build or sprite-frame inspection is claimed for this lifecycle-only fix.
- Results, immutable source archives and external filtered runner are retained at
  E:/CodexTemp/TypingHero/impact-vfx-lifecycle for reproduction; disposable when no
  longer needed. Delivery/cleanup receipts are stored there after integration.
  See the [preset checkpoint](../plans/metal-clash-preset.md).

## Near-zero physical particle drag (2026-10-22)

- Unity2022.3.25f1/fly-server red20261022T014230-quick reproduces1/1failure on old
  runtime: drag.0001/age.12s gives Y=-19.0962753WU instead of-.07063172WU.
- Green20261022T014352-quick inputa78eb11385d22f5183173e21d9b5ccf220d13580c0401937eaf46a8cec0d50fe:
  focused PhysicalParticleTests + ParryClashTests53/53PASS,0failed/skipped,
  13.2167476s. Adds16numeric regression cases; prior pause/pool/direction/budget
  and zero-allocation tests pass. All1992build/test inputs match the archive.
- External adapter retains the shared runner lock/cache/license and selects tests
  through Unity's existing filter; repository runner/toolchain/preset assets unchanged.
  No new WebGL/native/visual/performance claim. Exact scope and retained receipts:
  [physics checkpoint](../plans/parry-physical-particles.md#near-zero-drag-correction-2026-10-22).

## Dungeon main menu — 2026-10-21

Cached Unity2022.3.25f1 WebGL153008 and strict repeat-builder PASS on input
`b2b63f11963989f3e233cdbd4da8c43faddf1952287260c2a6f5069b9f37b020`.
Native Edge155/ANGLE D3D11:83/83 scoped checks PASS at1280x720 and640x360 in29.777s,
zero JS/managed exceptions/provider writes. Final menu/background/preparation and
unchanged Settings/Top1000/help visually inspected at both sizes. Real mouse and
keyboard, invalid/valid names, saved name/difficulty, blocking and opener focus,
Play/Tutorial dispatch and existing Unseen/Skipped rule verified. No six-lesson,
full gameplay/performance suite or itch deployment. Tutorial WIP/head overlap
remains unresolved. Asset prompt, mapping, corrections, evidence and preview:
[checkpoint](../plans/dungeon-main-menu.md).

## Final thin-glass lantern candidate ? 2026-10-21

- QA110751/input046160c165646a5c3788e3b9cf90e90cfb9736e63224afcff7ee2ee742997e34,
  player028bdab132b5e1c9: build + strict twice-run full builder PASS.
- Correct PlayMode HDR110516: actual320x180HDR Point world RT/1280x720 presenter,
  active Volume; all7glass/emission/shadow/bloom stills inspected. Held-pose glass
  off/on0changed pixels outside cage ROI590,140..690,275. No floor outline return.
- Edge155.0.4283.62/Intel UHD/ANGLE D3D11/1280x720/headed: native22gameplay checks
  and9camera/source checks PASS,0exceptions/provider reads/writes. Real attack,
  hit/parry,pause,Slayer/restore and glass environment lifecycle/fixed emission verified.
- All9pan/FOV endpoints and selected complete pan12.48..18.96s,163/163frames,
  162/162transitions inspected across11consecutive sheets; native0/80/162 inspected.
  No observed alpha popping/transparency overlap/light drift/large floor outline.
  Source25fps lossy capture cannot exclude sub40ms artifacts; no whole-video or
  realtime playback review claim. Original PTS and held frames retained.
- Fresh20s same native driver/no-video baseline472/playered2d0d86653698d7:
  n245/mean32.12816/p5032.350/p9533.225ms. Final486: n250/mean31.38584/
  p5031.300/p9532.575ms. Single runs/RNG/observer overhead; no GPU-only/FPS/
  performance equivalence claim. Earlier NVIDIA measurements are separate.
- Reused071929lifecycle2/2PASS, no runtime component changes. Existing meta/GUIDs
  retained. Final source scene changes only approved fixture blocks plus glass.
  Integrated QA114427/playerfdc8253ed7a62b1f: strict builder/native25 PASS,
  6scoped offline checks PASS, main gameplay/UI retained with task dungeon3D. No production build/publication. [Checkpoint](../plans/combat-hanging-light.md).



## Player received particles removed (2026-10-21, local)

- Orca child `micharel09/remove-player-damage-particles`, origin/main95767447 base.
  Only the builder, retired prefab wiring, its scene override and regression tests
  change; all runtime scripts, animation/art/GUIDs and gameplay settings remain.
- Frozen input `fe8c8c2e09129e0b36d910e78a0665dad8a6fa04d663f44d65351ea31b4874f5`,
  run20261021T041018-webgl: pinned Unity2022.3.25f1 scoped42/42 PASS,0skip/fail,
  17.685s. Both legacy/clean roots survive two configurations without received
  routes; outgoing normal/Slayer/lethal and BCompact references remain connected.
  QA build and strict repeat-builder PASS. Generated prefab/scene match the local
  patch; only Compact.mat's empty-property trailing space differs elsewhere.
- Edge155.0.4283.62/Playwright1.58.2/GTX1650Ti ANGLE D3D11:178checks PASS in325.623s.
  Both1280x720/640x360: eight sword5HP and five axe8HP deductions40to0, normal Hurt
  recovery, parry without damage, outgoing hit, pause/restart, real Continue to axe,
  both deaths/results. No Received/ReceivedLethal observed.0JS/managed exceptions;
  four fixture reads/four intercepted writes,0production writes; external blocking on.
- Reviewed all29unchanged source cels/27pairs plus301render captures/293within-window
  pairs: red recoil returns to idle, white sprite dissolves into authored flecks,
  no received burst. Sound exports at1x/.25x in both sizes use actual48kHz stereo;
  raw peaks.57030/.59371. Clips25fps and QA labels have sampling limits; no claim
  of every engine frame, sub-frame timing, subjective audio listening or performance.
- User then narrowed verification to the changed behavior; no further QA added.
  Preview/clip retention, missing supplementary melee guide and cleanup are in the
  [combat checkpoint](../plans/combat-feedback.md). No stress/soak/production/deploy. User-approved source commit8e840a0 was subsequently pushed to origin/main; task resources were cleaned after acceptance.

## Approved parry main integration (2026-10-21)

- User approved previewd866c5b and requested push/main closure. Mergeec30ed9 preserves newer main HUD/palette and excludes the unbuilt distributed-launch candidate. Final canonical source4dc0cbfe2fa75c97394e7aa5ec95d12f25be9d320ca65f23f3b8073909da854d: pinned Unity2022.3.25f1, repeat builder unchanged, all1564 checkout inputs match, zero final C# errors.
- Unity31parry plus4HUD/asset checks pass. Canonical Edge/native HUD damage confirmation6checks and final sword/axe/Slayer1280x720/640x360 run93checks/98.9857342s pass, including real damage/parry HUD pulses, pause and Continue cleanup. Zero JS/managed exceptions, provider reads/writes or external requests; no cue retry. No new performance/stress/soak/mirror or publication claim.
- First integration player020742 is unqualified despite a successful final build: intermediate script-import CS0246 and missed HUD pulse observation. Rebuilt canonical assets022158 without that error and confirmed behavior before push. Detailed provenance, retained final/reference clips, excluded candidate recovery and cleanup are in the [combat checkpoint](../plans/combat-feedback.md). Parent dirty main remains atb36b55a; remote main contains the merge.

## HUD readability for side-view 2.5D preparation (2026-10-20)

- Base8fa55dd, isolated child `micharel09/hud-25d-readability`; parent WIP preserved. Unity2022.3.25f1/fly-server serialize055929 PASS: strict repeat plus final-branding font/material/settings/glyph/content/bounds checks. Three new VT323 bitmap atlases8/10/12 each contain97 characters with actual nonblank required glyph pixels, static/Point/Clamp/one-pixel gutter. All858 existing scene IDs retained; only four backplates/16records added. Three complete typing text components are byte-equivalent after newline normalization.
- Final QA060631 PASS, input `f7f98d70df12176141ff3c76e5813833fa68b1659901f52835278f54d1da122b`, QA=true/repeat builder PASS. Frozen generated scene and all six new font asset/meta files equal checkout. A temporary QA-only runner reused the normal snapshot/lock/build and immutable-player path, skipping software execution in favor of native Edge.
- Edge155.0.4283.62/Playwright1.58.2/GTX1650Ti ANGLE D3D11, real input. Feedback run covered five levels, six Normal duels/victory(10HP/4259score/34parries), all eleven feedback flags and normal/Slayer four-size captures. Full run remains FAILED15070checks/280.739s at the final restart: the driver's identical old intro state matched before reload and Enter was lost. Fixed only driver acknowledgement/hold timing; reused completed unchanged coverage.
- Focused lifecycle same-player PASS25/25/36.788s, intro/pause/resume/completed restart/death/results and synchronized hidden difficulty background at320x180/640x360/1280x720/400x300. Existing score-cluster native profile PASS209/209/63.055s: real tutorial parry/Slayer, active multiplier/decay all four sizes, renewal/pause/expiry/timeout/restart/natural exit. Inspected normal/Slayer/active-combo images at all sizes and modal samples: sharp pixels, unchanged tray, no observed HUD/frame overlap or HUD shift with existing zoom. Current-scene cue/popup samples clear important HUD content.
- All three native reports0JS/managed exceptions; fixture interception/external blocking enabled throughout. Two Endless writes intercepted, zero production writes. No full balance/stress/soak/performance or production/publication claim. Representative2.5D bright/dark lighting, parallax, foreground/VFX occlusion and popup/character/tray clearance remain unverified until such a scene exists. Receipts, helper hashes and cleanup: [HUD checkpoint](../plans/score-hud-cluster.md).


## Contact-origin frame correction (2026-10-20, local preview ready)

- WebGL092035 sourceec52436251fc2a7766d9dc33a4b77f9f5783e00c767ca3f6aadaf6460ef354d4
  built with pinned Unity2022.3.25f1; strict repeated builder reports serialized
  objects/references/listeners unchanged. Only runtime change is the birth/first
  flight clock in ParryClashBurst. No physics/material/asset/GUID/API/package change.
- Focused Quick092648 PASS31/31,0failed/skipped,14.7551432s. Its frozenf2a2145f
  archive matches every build executable/test/asset/package/project input byte;
  only one explanatory comment differs. New early-Update/LateUpdate test stalls
  100ms before and after birth:24particles remain at contact/age0, first movement
  stays within1WU, and disable/reuse repeats correctly. Existing pause/hitstop,
  24/192, long travel,20floor/bounce/rest/expiry, no allocation, preset restoration
  and contact+tail completion tests pass.
- Native r4 PASS86/106.2375723s: Edge155/Playwright1.58.2, GTX1650Ti ANGLE D3D11,
  defaultBCompact/noF6, sword/axe/Slayer at1280x720 and640x360, intervening Double
  Strike, pause and Continue. Zero JS/managed exceptions/provider reads/writes/
  external requests. One logged automation cue miss at1280 (40->35HP) was retried;
  accepted-parry damage checks pass. This is scoped visual/integration QA, not an
  all-input timing or performance guarantee. No additional perf/deep/mirror run.
- Direct post-render canvas captures show contact birth followed by outward
  motion and the original far travel. Inspected both sizes, axe smooth-frame
  progression, cold sword first-step handling, floor tail and Slayer. Readback
  can itself cause stalls; this capture is not used to measure frame performance.
  QA publication100ms and native video25fps cannot prove the birth frame alone.
- Both actual-sound exports are42.021328s, H264/yuv420p25fps plus AAC48k stereo,
  full decode PASS. Sword,axe and axe-Slayer at1x/.25x, with full flight/bounce/
  expiry; PCM peaks.548333/.535968, no microphone. Nominal onset uses observed
  particle age and400ms lead, not frame-perfect synchronization or locked RNG.
  CLACKBLADE-Parry-ContactOrigin-1280.mp4 SHA256 a3bdd8628238331d4ed4ef4117ff9c8fb2780b0ff74590c13d03f896f163b7bc;
  640 SHA256 6374266022160c2219f940bafeb2cae72dbc967fdd5411c1b06fbad7f8db6e02.
  Previous approved/physics clips remain unchanged.
- Intermediateadf run passed30/31 and exposed native contact expiry on the birth
  frame; fixed without relaxing24. Obsoleteadf/c7fc WebGL jobs were canceled.
  Native r2 failed25checks on demanding a parry effect after an intervening duel
  had finished; r3 failed11 on a cue miss under capture. These remain failed.
  Temporary-driver corrections only; final player was not rebuilt for them.
- Headed1280 preview opened paused on loopback8152 from contact-origin/player.
  Keep the existing child and E:/CodexTemp/TypingHero/parry-clash-lighting/
  physics-pixel24/contact-origin player, launcher, clips and receipts for visual
  review. Esc resumes; Space parries; closing the page ends its helper/server.
  No push/merge/publication. Earlier552f p95 below belongs to that earlier source.

## Approved flash with restored physics (2026-10-20, local preview ready)

- Frozen source552f67ad130a6e5c988122fad484fb3bcecd2b18625f2340f39136062c0d03fd.
  c12c3de Pimen flash/material/pivot/size/color/timing, Point5/70/280 and recoil1px/
  100ms unchanged; cc06385 analytical launch/brake/fall/one-bounce/rest180ms paths
  restored. Existing Compact masks and real Z/depth tests;24/192, systems1/2/10/1/10.
  Uncommitted48 revision discarded.1356protected asset/package/project files match
  c12c3de after newline normalization; all27old MP4 hashes remain unchanged.
- Unity2022.3.25f1 on fly-server: WebGL082208 build/strict repeated-builder PASS;
  focused Quick083302 PASS30/30,0failed/skipped,14.18s on the same input. Includes
  common origin/local random,20continuous single-bounce/rest/expiry paths, fixed
  buffer zero managed allocations in100updates, pause/reading/hitstop/reuse/disable,
  192cap, full legacy restoration, exact200ms contact and completion of both clocks.
- Native qa-r2 PASS83/97.185s: Edge155/Playwright1.58.2/GTX1650Ti ANGLE D3D11,
  keyboard/defaultBCompact/noF6.18real parries across Swordsman/DoubleStrike/Heavy
  at1280x720/640x360; DoubleStrike is the required intervening duel to reach axe.
  Sword/axe contact/tail and Slayer at both sizes, pause holds state, Continue clears
  roots, sampled Y>=-2.896402 (dungeon floor-2.9275508 plus half pixel1/32).
  Zero JS/managed exceptions/provider reads/writes/external requests; observation-
  only bridge and network fixtures retained. First qa attempt failed5checks on a
  capture-helper variable shadowing/TDZ; renamed only that helper variable, retained
  failed report, no gameplay change or product rebuild.
- Separate no-video/no-screenshot perf PASS14. All-state p95ms:1280=13.455(n636),
  640=13.540(n636); live13.520(n340)/13.575(n344). Approved baseline PASS14:
  13.505(n630)/13.560(n646), live13.515(n307)/13.575(n304). Same native flags,
  profiling OFF; restored-physics run had the interactive review window open.
  These short RAF samples are not a controlled equivalence/GPU/device-FPS claim.
  No deep/stress/soak/mirrored QA was added.
- Two42.021328s clips, native25fps H264/yuv420p plus AAC48k stereo, full decode PASS:
  sword,axe,and axe-Slayer, each at1x then0.25x with actual game PCM (no microphone).
  Raw PCM peaks.557749/.587604. Each segment retains400ms nominal lead and2.4s
  after the observed onset; full launch-to-expiry is present. Onset age estimates
  are not frame-perfect video sync: first visible flashes inspected around10.000s
  (1280) and7.36s(640),40ms sampling. Compared with approved flash source2158 and
  the retained Strong24 physics reference; different RNG/pose phases, no pixel-
  identical replay claim. Contact cels/properties are proven by unchanged inputs
  and offline assertions. Small ray-to-pixel/floor/bounce/rest fade inspected; no
  observed floor penetration/pink/voxel parry blocks in these samples.
- Interactive1280 preview opened before remaining native/AV/perf checks, per user
  steering. Keep child micharel09/parry-contact-flash and task temp
  E:/CodexTemp/TypingHero/parry-clash-lighting/physics-pixel24 for visual review.
  Launcher open-preview.ps1, player/, clips and receipts remain; closing the Edge
  page ends its helper/server. Legacy approved/physics clips remain untouched.

## Cinematic pixel parry (2026-10-20, approved flash baseline)

- Approved baseline input `2158fcdf07c678134da9bc716996019c855294d2e1be57fe996ebf8a6327410a`,
  child `micharel09/parry-contact-flash`. Quick20261020T063136 PASS26/26;
  WebGL063220 build and strict repeated-builder PASS. Pixel material/module
  restoration,24/192budget, common origin/isolated random, stock fall/collision,
  contact snapshot, pulse timing/overlap/pause/disable/reuse and mixed Point/Spot
  arbitration covered. Generated GUIDs preserved; no duplicate asset GUIDs,
  missing new metas, actor source PNG/importer/clip changes or package changes.
- Native `parry-cinematic-final-qa` PASS66: Edge155.0.4283.62, Playwright1.58.2,
  ANGLE NVIDIA GTX1650Ti/D3D11. Actual keyboard/defaultBCompact/noF6;
  Swordsman/DoubleStrike/Heavy/Guard at1280x720,640x360Slayer; normal/received/
  Slayer/lethal smoke, pause and Continue cleanup. Zero JS/managed exceptions,
  fixture writes or external requests. Read-only QA/network fixtures retained.
  All6 clean weapon cels equal original RGBA crops. Blade centerline gaps sampled
  sword1=0,sword2=.084WU,axe1=.071WU,shield~0, within visible blade thickness.
- No-capture `parry-cinematic-final-perf` PASS14 on the same native configuration:
  1280 p95 13.435→13.485ms(n646→633), live13.420→13.520(n341→309);
  640 p95 13.510→13.505(n640→646), live13.515→13.505(n350→314).
  Baseline is the prior757f20fb voxel player. Short RAF samples, no performance
  guarantee; no increase above10%, so no ABBA run.
- Actual AV `parry-cinematic-final-audio` PASS12, native25fps1280x720 plus
  stereo48k game PCM, peak.516182; no microphone. MP4 H264/yuv420p+AAC fully
  decodes. `CLACKBLADE-Parry-Pixel-Cinematic.mp4` shows three real parries at1x
  then a0.25x replay; `CLACKBLADE-Parry-Pixel-BeforeAfter.mp4` compares the prior
  voxel appearance with the final pixel hybrid at both speeds, with actual sound.
  Files/receipt and interactive launcher are under
  `E:/CodexTemp/TypingHero/parry-clash-lighting`. Visual onset alignment uses
  native40ms samples, not deterministic or RNG-locked replay. Reviewed onset,
  40/80/140/220/280ms and falling-tail frames; this is not human art approval.
  Final cinematic SHA256 `ac9b996339e30ea230306283d3074b47564fc625f738f4ffdf71b7d60bc5475a`;
  comparison `f4392f7151ba6521d59825a9e4a0b6958681299a693e3dd5bb3d53415e169629`.
- Temporary BakeMesh UV diagnostic Quick065007, unique immutable input6bf8d739,
  executes1case and confirms normalized startFrame1/6 starts at tile1;0starts
  tile0 and.9999starts tile5. Earlier tile-index1 interpretation was wrong and
  its98f7986f player failed25/26; never use it as the candidate. Diagnostic code
  stays outside the repository. Previous empty/silent AV attempts are unqualified.
- Preview only: no stress/soak/mirrored QA, production build/publication/CDN check,
  push or merge requested. Keep final player/clips/reports and launcher while
  review is active; close preview to stop its helper/server. Prior physics clips
  remain byte-identical. Historical voxel receipts below qualify their technical
  behavior only; the user rejected that visual direction.

## Full VFX with concurrent3D arena (2026-10-20, scoped preview ready)

- Focused Quick030739:27/27PASS, zero failed/skipped, input `cddebed9d46c512da9204e9272c4173d844a7e029cba3d209fe7cd22bc7879a1`. Includes existing BCompact/legacy/pause/material/lease checks, all five impact profiles/local-random/floor support, accepted damage/iframe/lethal ordering, captured Slayer tint, afterimage generations/cap, actual-cel slash mapping and shared real3D-light timing/disable. No broad gameplay suite, stress/soak or new performance measurement.
- Candidate WebGL030935 on `c639e3d9eaca0c146d847ac0dfe6e454a5e0adc1f0a4eaa08f90f93e55c20d79` failed repeat serialization at the imported stone material. Diagnostic serialize031638 identified only URP's late `_Color` alias update. Explicit aliases/GetColor and dirty persistence fixed it; WebGL032020 on094aba33 passed build/repeat but is not visually qualified because its atlas binding was null.
- Final WebGL033055 input `757f20fb2074ae8641221db06b80fd410ff9a18b913190c87b81a3acb47988f2`: build/strict repeat PASS. It gives the original1024x1024 atlas explicit Texture2D/sRGB/Point/Clamp/no-mips metadata, preserves GUID/PNG bytes and errors if import is absent. Actual BaseMap fileID2800000/GUID54fcf6fd51724a4b8c2776ebeb709ffd binding verified. Runtime/tests are unchanged from Quick27; all deltas are presentation builder/import metadata. Parent/sibling WIP remains untouched.
- Earlier Quick025745/WebGL025824 failed compilation before any tests because the imported editor builder needed an existing Cinemachine assembly reference. Added that reference without a package change. Prior QA023116 also failed repeat serialization of the newly extended hot material; persisted explicit defaults and initialize new material properties only at creation, preserving later tuning. Failed attempts are not passing receipts.
- Static assets: no duplicate GUIDs; all12 original KayKit model/atlas hashes match durable SOURCE.json; all9 actor sheet metadata files match70d789d by content. Durable slash layers recompose original RGBA, with no changes outside authored masks; body/weapon source composite reviewed. Final new-world samples are described below. Old physics clips retained; no publication performed.
- Native Edge155.0.4283.62/Playwright1.58.2 on the final757f20fb player:70scoped checks passed and `preview-ready`. Actual keyboard/mouse, default BCompact/noF6; Swordsman/DoubleStrike/Heavy/Guard, accepted damage/normal/Slayer/lethal profiles, held pause positions/ages, scene Continue cleanup,1280x720 plus640x360Slayer sample. Zero JS/managed exceptions and fixture writes; external blocking/read-only QA retained. Inspected contact/cubes/slash poses, enemy axe grip/trail, lethal palette, red silhouettes/640 and the fresh1280paused review window. This is sampled visual QA, not human art approval, broad game/performance acceptance or all-RNG proof.
- Driver-only failures r1-r3 retained in the checkpoint: Continue reloads straight into combat, Run.Win advances the next-opponent label before Continue, and the QA spark snapshot can lag the state by one frame during reload. Corrected observation to wait for live HP/not-finished and two fresh frames before asserting cleanup. R4 passes without any product change to satisfy the driver. Copied112generated assets preserving GUID pairs; original407menu/853combat IDs and all9source sprite metadata preserved. MainMenu bytes restored after proving its generated diff was whitespace-only. Preview/player/helpers and prior physics clips remain task-temp for review; no source push/merge/publication.
- R4's interactive page closed at10:51local; final browser report is `passed`,72/72checks,0exceptions/fixture reads/writes/external requests,250.243s including the paused review interval. Wrapper32008/server8142 exited normally. Retain final player/screenshots/launcher for visual feedback; obsolete r1-r3 browser profiles/captures and the closed r4 profile can be removed after these receipts. Original Strong24 physics MP4 hashes rechecked unchanged (6405ff54e05…/128070df4057…). No further runtime checks launched.

## Non-combat dungeon skin (2026-10-19; publication pending)

- Final QA141923/repeated-builder and production142518 PASS on the same `781c881452f74d06b79ee7f47efb4efe4e6f09f46e3078ee613c94a88cf6e8a0` input. Pinned Unity2022.3.25f1 on fly-server. Original367menu/834combat IDs, persistent listeners/scripts retained; all93 HUD objects/overrides and CombatR7 art bytes unchanged. Four v2 PNGs match source bytes, Point/Clamp/no-mips/borders verified; R7 rim subrects reuse original textures with no raster generation.
- Edge155.0.4283.62/Intel UHD D3D11: final81/81 scoped checks PASS in133.708s. Menu/Collection/help at320x180,640x360,1280x720,400x300, valid/error/long name, Tab/arrows/Enter/Escape, wheel/scrollbar/loading/error/empty/retry, volumes, six tutorial lessons/retry/completion, pause/death/restart/Continue.0exceptions; Classic4fixture reads/0writes,Endless13reads/1intercepted write, external blocking on.
- Same final player additional modal-size run6/6 PASS: tutorial/intro/pause/results/two-line Continue captured and inspected at all four requested sizes. Prompt background80 now encloses existing text without moving content/actions; R7 metal rims use3reference units per source pixel. No production writes. Earlier3c24 finite Normal victory/unlocked UI run1172/1172 PASS remains valid for unchanged runtime/controllers:6defeated/35HP/3177points/14parries; victory960/640 and unlocked text lists inspected. No balance matrix or performance claim.
- Final normal player: qa=false, all JS/WASM/data exclude bridge markers; original-HTML native menu/name/tutorial skip/Fight/pause/resume smoke2checks PASS,0exceptions/external requests and bridge undefined. ZIP30members/16,575,624bytes, CRC PASS, SHA256 `5907f752234508235aa17e13a4468e82a1329f6eb0f9cde4a8f2ed0c9288a212`. Published upload19876564 at https://micharel09.itch.io/clackblade; previous19874589 hidden/retained.29/29non-HTML anonymous CDN hashes match ZIP; index adds only standard itch htmlgame.js. Fresh anonymous Edge155 loads actual centered dungeon menu, bridge undefined,0exceptions/provider writes. Other page configuration identical after saved-form reload. Source commit1bedcff; [release manifest](../../marketing/itch-io/ui-dungeon-release-20261019.json). Known Endless401 unchanged. See [skin checkpoint](../plans/ui-dungeon-skin.md).


## Full merged HUD/Endless public release (2026-10-19)

- Main source/publication commit b243545; final production102505 input5a0a621a730dd828d6fc7a33004256ffbc67808cd0f4f132fb38b5e17b7b9454. ZIP30files/16,595,641bytes, CRC/hash/JS-WASM-data QA exclusion PASS. Artifact hash and payload hashes: [release manifest](../../marketing/itch-io/full-release-20261019.json).
- Gameplay Quick101341 input70312dec:116/116PASS,0skip; strict builder repeat102201 PASS. Final delta is only the Endless combat-label position, corrected after smoke found overlap with r7 HP. Both scenes retain every prior ID/GUID and committed bytes equal build-generated scenes.
- Native final production:4automated checks,0JS/managed exceptions,0production writes. Actual menu/name, tutorial skip?Endless Fight, HP40?35, label clearance and pause/resume visually inspected. Earlier133checks certify their frozen R7/tutorial input; no new full difficulty/audio or human-WPM result inferred.
- Published upload19872500 at https://micharel09.itch.io/clackblade; typing-hero redirects there. Public actual menu loads and29non-HTML CDN files match final ZIP exactly. Previous19872290 hidden/retained for rollback. Fresh read-only Classic200/Endless normal401; Firebase untouched. See [release checkpoint](../plans/ui-kit-v2.md) for scope/resources/rollback.

## First-boss finite run (2026-10-19)

- Fresh QA094932/repeat-builder PASS on
  ca6cf60cdb52fb54a53a9c1710dd40885ac6b57dd8837424c48fbec7f952fc54.
  Entire current Assets/Packages/ProjectSettings matches this frozen input after QA.
- Native Edge155/GTX1650Ti/ANGLE D3D11/Playwright1.58.2: five-level matrix
  4413/4413 PASS,30wins,603.107s. Victory after boss death, repeated Enter/no duel7,
  no healing (35HP retained), final defeated6/exact-score payload, frozen stats,
  same-level Restart40HP/zero stats, early death/best retention, tables/Classic and
  error/filter/empty/retry pass. All20 result/table captures have the requested
  960x540/640x360 sizes; actual images/hints reviewed, no round column or heal copy.
- Same-player pending503/reload/original-partition retry and six tutorial lessons
  86/86 PASS,98.299s. Five matrix fixture writes/zero Classic writes;0exceptions,
  external blocking on, zero production Firebase writes. Functional QA only.
- Exact-input Quick101506:116/116 PASS,0failed/skipped,46.073s. Includes final
  terminal/callback/restart/best/pending regressions and shared UI/VFX consumers.
  Previous Quick105/native4155/pending85 receipts are superseded.
  Software pilots died early; no software-profile PASS. Driver input-observation
  race was fixed without changing game behavior or thresholds. Runner8/8 PASS.
- Task temporary captures/reports/players and four owned server results removed
  after written receipts; QA listeners closed, shared source/cache/build-qa retained.
  Details in [Endless checkpoint](../plans/endless-duel.md). No production
  build, release, Firebase config change or fixes to the two deferred issues by
  this task. Current QA preserves concurrent UI/input/VFX changes.


## CLACKBLADE public release (2026-10-19)

Final input0fd9445887f60e2a7ba4c9e2578c22371f57e995079cc7c662bf9cfbd82c7a0b:
production092049, QA exclusion/CRC/hash PASS, native focused combat-r7/tutorial133PASS,
exact original production HTML smoke9PASS (name/reload/menu640/960, BLADE attack,
pause/resume/menu/Classic fixtures, no probe/exceptions/score writes).
Software combat-r7 run passed125 scoped checks then failed its extra legacy
cinematic duel before Slayer; not claimed overallPASS. Task-local native focused
run omits only that separate scenario and preserves all R7/tutorial Slayer checks.
Public upload19871730 verified through Run game/menu and29matching non-HTML CDN
hashes; index only adds standard itch.io htmlgame.js. Source has all current dirty
Guest/Endless/HUD/audio inputs. No new full five-level/audio matrix claim.
https://micharel09.itch.io/clackblade; typing-hero URL navigates there. Old upload
19865461 hidden/retained. Theme/pricing/tags preserved; no social/devlog/Git push.
Online Endless read still401; Firebase unchanged. User instructed publication
with that limitation. Do not claim old public-browser data migration.
See [release checkpoint](../plans/clackblade-rebrand.md) for receipt/rollback.

## Combat HUD r7 integration (2026-10-19; earlier frozen candidate)

- Component exporter27PNG PASS; pool219 unique, maximum54px in58px cell.
- Quick085843 input5304b09b00eefb63ef67141a79df7fd3bcefeceadd4e09280b6a50892de64836:104/104PASS,0skip. Covers synchronous queue/input cadence, burst, reset/error, event/reward behavior, Point/Clamp sprites, bitmap gutter and actual HP0/1/50/100% fill.
- Serialize090126 input2757a5420ca6a9b3ed3731a55e218d4fc9a9434d913f9c9943e243219f12948a: strict repeat-builder PASS. Scene827IDs retains706original IDs and original GUID references; component bytes/GUIDs preserved.
- Latest compatibility declarations and native harness callback postdate these receipts. Final recheck attempts ended at shared runner lock timeout, so latest-source acceptance is pending.
- Pilot091145 input0fd9445887f60e2a7ba4c9e2578c22371f57e995079cc7c662bf9cfbd82c7a0b:125checks then cinematic failure,0exceptions/0fixtureWrites. Native Edge headless61checks then six Parry retries; headed real Parry succeeds but focus/resume check times out after56checks. Tutorial/Slayer/Continue/restart are not accepted. Native320/1280 images visually confirm clean glyphs and dungeon composition; this does not prove complete flow.
- Production Endless reads401; no backend changes/writes. No release build, ZIP, upload or public-payload verification completed. Exact pending decisions, receipts and retained failure artifacts: docs/plans/ui-kit-v2.md.


## CLACKBLADE rename and audio headroom (2026-10-19; release gated)

- Source/menu/product metadata/owned credits/current marketing use CLACKBLADE;
  TYPE TO SLASH retained. Cover630x500/315x250/158x125 and banner960x394 exported
  from existing HTML/assets and visually checked. GUIDs/assemblies/Firebase/save
  keys preserved. Existing MP4 Shorts still need re-export after final source freeze.
- Frozen brand Quick99/99; QA072050 player matches all30 files of QA071020.
  Native brand11 and storage13 checks PASS: menu960x540/640x360, Guest/custom name
  through play/menu/reload, Classic, compiled old/new products at one URL.
  A different hosting directory creates a separate fresh preference scope and keeps
  the old files. No claim of legacy public-browser data migration.
- Existing same-player five-level matrix22063 checks/66wins PASS in Endless plan.
  This does not certify the later concurrent CombatR7/TypingManager changes.
- Actual pre-fix destination stereo peak1.43137753 reproduced. Rejected mixer-only
  snapshot experiment had no WebGL payload/signal effect; mixer asset restored.
  AudioPreferences before-scene initializer now sets AudioListener.volume=.5.
  Event gains/clips/prefs/ratios/timing untouched. Frozen input
  a3848574cdbcb9f6791b9b1e06a9ba98a988c9ab782057cfa1562d82c6e9feb7:
  Quick99/99/0skip and QA075530/repeat-builder PASS.
- Native Edge155/GTX1650Ti/ANGLE D3D11/Playwright1.62.1: focused15 PASS;
  normal41470/tutorial18687 mute samples exactly0, full tick.0757400/error.0947887.
  VeryHard six wins, nominal70ms/letter, actual Music1/SFX1:1511 checks PASS,
  230.403s, stereo peak.79853839,22parries/3678points, death/best/restart/tables/menu.
  Zero exceptions, one intercepted Endless fixture write, zero production writes.
  A one-sample intermediate-volume pool transient remains below clipping; exact
  peak-linearity is not claimed. Signal QA only, no speaker/human/FPS claim.
- Production reads: Classic200; endless_v1 and endless_v2/very_easy401 Permission
  denied. No Firebase configuration/auth/writes changed. Latest-source CombatR7
  acceptance and backend decision are required before release. No production ZIP,
  itch mutation or new slug claimed. Checkpoint/rollback/resources:
  [CLACKBLADE plan](../plans/clackblade-rebrand.md).

## Guest identity and discovery candidate (2026-10-19)

- Final immutable input `d391738b0cb5fd9163c3e8b287810f7412c4a6e3927cfebbee8755393293cbe7`.
  Unity2022.3.25f1, pinned GameCI image, Ubuntu fly-server,2CPU/8GiB.
- Quick044730:97 cases,96pass/1fail/0skip. The only failure was a new test's
  incorrect expectation of `New hero`; existing normalization returns `New Hero`.
  Corrected expectation only; focused final Quick045715:4/4pass/0skip.
  Other93 existing cases had passed and their inputs are unchanged.
- QA045130 build/repeat-builder passed on input24ab98cb3c87e0c96eba1f52ba26c0cdfd76ae1d9016375f4f1bfc501b3dd375.
  Per-file archive comparison proves the only difference from final input is that
  test expectation; all runtime/config/assets are identical. Current checkout
  runtime also matches; two later unrelated browser-driver edits are preserved.
- Native Edge155.0.4283.62/ANGLE D3D11,960x540: valid fresh GuestXXXX,
  Enter/Play, first tutorial, Skip, match restart, guest reload, invalid edited
  names, custom name save/reload, tutorial replay, actual BLADE hit, tutorial
  restart and menu checked with actual keyboard/mouse. Final targeted tutorial
  segment9checks PASS. Earlier passing identity/fixture segments were reused;
  failed combined attempts are not claimed as full-profile passes.
- Fixtures observed Guest7279/score301 and NewHero/score10 payloads. External
  traffic blocked; no production score writes. Equal-to-best score correctly
  does not force a write; payload verification used a genuinely higher score.
  Driver corrections: capture transitions continuously, wait for actual scene
  restart rather than the old reading panel, allow result reveal before clicking
  Menu. No game timing, leaderboard rules or gameplay changed to satisfy drivers.
- Production050328 PASS on final input.30files/16,457,704-byte ZIP; local/server
  SHA256 and CRC PASS:061cc7f2e6af3236a5f0115e762c82c8486e4e9f49f796b9683f42bb4fd678d7.
  All JS/WASM/data exclude typingHeroQA/GameLoopProbe/TYPINGHERO_AUTOMATION in
  ASCII and UTF-16LE. Original HTML native smoke: guest name, Enter, tutorial
  BLADE actual hit/score10/ATTACK COMPLETE inspected; bridge undefined,
  zero exceptions and zero writes. Build: `Builds/TypingHero-WebGL-Guest-20261019.zip`.
- Build includes the pre-existing Endless/HUD/audio checkout. This task verifies
  the identity/tutorial integration, not new balance/performance/audio acceptance;
  earlier release limits remain in their existing checkpoints. No itch upload.
- Clip/export receipts and task cleanup are tracked in docs/plans/itch-discovery.md.


## Endless difficulty QA (2026-10-19)

- Frozen QA071020/input
  `956fec7a879998b7c018f0a0b21a71cc8d4932d0c03cc03ea2c834269decffa1`:
  build/repeat-builder PASS; exact archived-input Quick081526 **99/99PASS**, no
  failures/skips. Covers configs/persistence/run carry/reset, role/elite/boss/capped
  recovery, combo renewal/boundaries/pause/tutorial, five local/provider partitions,
  legacy Normal, ETag/conflict/read-error/pending and >1000-row boundary ties.
- Same player native Edge155/ANGLE D3D11/GTX1650Ti/Playwright1.58.2: five-level
  matrix **22063checks/66winsPASS**, rounds3/1/3/1/3 for VeryEasy/Easy/Normal/Hard/
  VeryHard. Each actual death/best/Restart/table/filter-race/error/Classic flow
  passed. Five fixture Endless writes, zero Classic writes/unhandled exceptions.
- Same player pending503/reload/original-partition retry and six tutorial lessons
  **129PASS**; five-level pointer/keyboard/focus/death/table/Restart **40PASS**;
  tick/error versus reading/pause/mute audio routing **11PASS**. Menu, intro,
  Slayer, results and tables inspected at960x540/640x360. Audio routing is not an
  isolated loudness/clipping measurement. Fixtures block external Firebase traffic.
- Combo samples721/794/834/240/248 and recovery samples9/9/8/2/4 in order
  VeryHard/Normal/VeryEasy/Easy/Hard. Median frame13.36â€“13.38ms; largest combo
  decay-minus-clock error <.00003s, recovery deadlines within sampled frames.
  Per-win role/elite/boss/round formula, HP/stat carry and10HP boss heal checked.
- Post-Slayer lock reproduced on QA054721: outgoing SlayerS2 frozen at state
  speedMultiplier0 while Animator/timeScale/gameplay clock advance. Slayer saved
  and restored the authored AttackSpeedMul default0. Changed neutral default to1;
  active Slayer3.5/clips/contact/cue/parry unchanged. Actual lifecycle regression
  fails on old0 and passes on1. Fixed full matrix includes positive state-speed
  checks and deliberate hits. Latest optional-hit driver cancels observations at
  encounter end: focused Normal6wins2476checksPASS, actual Heavy hit35â†’27.
- Frozen source archive/player/reproduction commands:
  `Builds/EndlessDifficulty-20261019/README.md` (ignored QA artifact).
  `python harness/verify_frozen.py` reruns exact-input Quick through shared runner
  lock. Standard source commands: `python tools/game_loop/run.py quick` and
  `python tools/game_loop/run.py webgl --browser-scenario endless`.
  Native harness flags and focused NETWORK/UI/AUDIO profiles are in artifact README.
  Native keys acknowledge accepted letters and reread after parry/promotion;
  probe is read-only. Latest harness bookkeeping change was independently checked.
- Later concurrent TypingManager/UI edits remain preserved but are outside this
  candidate. This task has not built production, uploaded itch, written production
  scores or deployed rules/index. Source research and remaining measurement limits:
  [Endless checkpoint](../plans/endless-duel.md). Agent operation is not a human
  playtest; Wine/software rendering is not a real-device performance result.

## Typing SFX release in progress (2026-10-18)

- Binder/builder/two original Kenney Interface clips and packaged CC0 notices
  integrated. Quick160418 **76/76 PASS**,0skips, input
  `2291f5ad5e6a24ac31a1ee6d6f62aff110f6ac19a650a6c0f0d04f73ccdd1c8e`.
- QA160520 build/repeat-builder PASS, input
  `d516a72f34d6c5ba3914436e194819eab3cfda0f51c6433d95699aa132761c02`.
  Scene retains664previous IDs/all GUID references and adds one binder75867774
  with two event refs. No other non-whitespace scene delta. Inspected native
  active normal HUD640x360/1280x720; prior four-size HUD receipt remains applicable.
  Software browser stops at intro after6checks; no full software profile PASS.
- Native Edge155/Playwright1.58.2, actual destination-node WebAudio tap with
  original route retained: audio-only11checks PASS, muted normal window32255samples
  exactly0, peak stereo.8942. Controlled tutorial tick at SFX.5/1 peaks
  .0670586/.1341172, error.1878652. These are signal measurements, not speaker QA.
- **Final native profile failed**,74checks executed before the audio gate failed
  during Slayer: stereo pre-destination peak1.0988642. Maximum occurs at14.04696s,
  before first typing buffer14.12063s; the14.12063â€“16.42177s typing/overlap window
  peaks.9178221 with0samples>=1. This distinguishes existing combat mix from typing;
  it does not establish an overall no-clipping PASS. x2/x3/exit behavior checks
  completed, but clipping invalidates the combined acceptance. Controlled tutorial
  mute-after-pause window additionally contains one nonzero sample(-.3772047),
  not a strict silence PASS. A separate native six-duel run subsequently completes
 39behavior checks, Continue/round2/boss heal/death/restart/menu,0fixture writes/
  exceptions,250.56s, but FAILS global audio peak1.4233700. Defeat6 carried30HP/
 4014points/22parries before boss heal. Captured201.27s of destination audio;
  overall profile and final-source reload/mute verification remain unpassed.
  Gain-node trace shows the first muted buffer briefly connects with the recycled
  UI gain.45 before its replacement connects at0. Binder now skips voice creation
  when SFX0, with regression added. Focused Quick16252533/33PASS,0skips, input
  `377ded72b681b88407a43140e204f37899fdc1bb8f937f01fbcd3f0dc8ac4380`.
  This is GameplayTests/TypingInputSfxTests only, not the required final full Quick.
  WebGL mute correction remains unverified.
  No authored volume/mix contract was changed.
- Final Quick161133 executes78tests:75PASS/3FAIL/0skips. A concurrent source
  migration changed EndlessRun to Resources before matching assets were present,
  causing NullReference in roster/role/actual-scene tests. Reject snapshot
  `23ebdb7bbc729eb9c2d927444b272fe23666cdd4c9feeff91bee2545abb93e4a`;
  its QA161229 was stopped after verifying the owned container/result mount.
  Concurrent edits preserved. Need a stable approved Endless baseline; new online
  schema/provider/UI changes are outside this release plan.
- No production build, ZIP, upload, public payload verification or release claim.
  Orca default profile is stopped by Cloudflare before owner/upload verification.
  Source decisions, failed harness approaches, retained temporary evidence and
  next steps: [typing release checkpoint](../plans/typing-sfx-release.md).

## Endless Duel representative loop (2026-10-18)

- Final gameplay Quick74/74, no failures/skips, source
  `6bf307dc0f686dd407084ecfe65470241147c53a64dc29f98ed6c4dd5a545d7b`.
  Final result typography Quick74/74, source
  `c65964ff9d500fe713121c04620f7bf1cc659a1c35bb47b7896366a858f7c200`.
  Includes actual scene transition35HP/123points, fresh enemy/parry/Slayer, real
  heavy8HP impact/cue240â€“400ms under frame sampling, hidden dead-enemy stamina,
  boss-heal cap, bounded patterns and progress-before-score record ordering.
- QA WebGL20261018T143344, source
  `0b72b04665673ec594fb6dc0d7323a53a9f3deb9fa5370c5275ad13122cae822`:
  build and repeat-builder pass. Native Edge155.0.4283.62, Playwright1.58.2,
  NVIDIA GTX1650Ti/ANGLE D3D11: **41checks PASS**,217.24s. Real letters, Space,
  mouse Continue, intro/pause locks, Slayer pause, six victories, boss heal35â†’40,
  round2 death, restart, fresh zero-letter death retaining best6/4412, return menu.
  Zero JS/managed exceptions and zero fixture writes; external blocking retained.
- Inspected actual normal, blue double-strike, larger warm heavy, Slayer and
  cleared-opponent frames; death result960x540 and640x360 shows all four lines
  at font20 without overlapping Restart. Confirmed progressed result ROUND2/
  DEFEATED6/4412points/100.0%/26parries, then fresh ROUND1/DEFEATED0/0points/
  no-letter accuracy while best6/4412 remains. No old floating stamina after cleanup.
- Automated reference cadence >=200ms/letter plus action time: normal duels
  19.18/16.98/20.25/16.48s, elite26.16s, boss48.32s. Earlier software Chromium153/
  SwiftShader40checks passed the same six-opponent loop with normals21â€“29s and
  boss61.83s. These are controlled reference runs, not human WPM or replay-desire
  evidence. Human recognition, preference and desire to retry still need playtest.
- Final software attempt on143344 stopped at intro: its60ms key hold could fall
  between ~94ms render frames. Not a software-profile PASS. Native final driver
  holds action keys until two observed frames; gameplay timing/windows unchanged.
  Driver SHA256 `8b3932374757be7474519303ed18317d12cb9affd02ed3acd7427092a28250fb`.
  The only later product edit generalizes Swordsman hint wording so it remains
  accurate after round3; no mechanics, numbers, layout or font change. Copy fits
  the same label already checked with longer Heavy/Boss hints; no suite repeated
  for that literal-only change.
- Earlier failures: first native heavy cue was447ms after slow playback, beyond
  the250ms parry window; delayed cue now retains300ms and measured scene test passes.
  Initial scene test left persistent audio objects; fixture cleanup fixed it.
  One editor hung in native asset GC; owned container stopped and fresh editor
  passed. Concurrent HUD compile snapshots/runner contention were isolated by
  preserving other edits and waiting on the existing exclusive runner lock.
- Source/checkpoint: [Endless plan](../plans/endless-duel.md). Temporary local
  artifacts,11 owned result folders and the server player copy were removed after
  these receipts; shared source/cache and other tasks' results remain. No release.

## Centered Slayer score, revision5 (2026-10-18)

- Quick143053 **74/74 PASS**, input
  `1f15e8ec15222e1f2e1b91f3906e7bac8efc5bac05c99c1606d3e60342918cde`.
  Glyph-based0/6/10digits,x1/x2/x3,pulse1/1.2,zoom/restore and interrupted220ms
  entry/exit, pause/unscaled completion and disable reset. Canvas fixture and
  independent origin/baseline measurements avoid the earlier textBounds line-metric
  error; actual glyph ink, not its advance/ascender box, determines the Slayer group.
- QA143344 build/repeat-builder PASS, input
  `0b72b04665673ec594fb6dc0d7323a53a9f3deb9fa5370c5275ad13122cae822`.
  Reused a concurrent build after raw archive comparison:996/996 non-test product
  inputs match current Assets/Packages/ProjectSettings. Pinned Unity2022.3.25f1,
  QA=true; scene normalized match/664 IDs/GUID sets preserved. Duplicate queued
  task build stopped only after verifying blocked FLOCK state. Other jobs preserved.
- Software score-layout completed201 checks (four normal/Slayer sizes, input,
  exact awards, tiers, pulse, popup, pause/resume/exit/restart), then failed waiting
  for an Endless next-duel screen after a real lethal. Not a full profile PASS.
  Remaining HUD/lethal path plus Slayer rerun **65 checks PASS/74.512s**, score772,
  27.268combat seconds,HP40 vs180HP enemy; fade/closed frame/restored24+16 fonts,
  destroyed actor and pause/R restart with full bars/zero score verified.
  Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB,0fixture IO/exceptions,
  external network blocked. Next-duel progression remains outside HUD acceptance.
- Same immutable player Edge155.0.4283.62: rapid Slayer **56 checks/25.629s PASS**,
  score155/x3red/exact awards/48px centered during pulse/pause/restore/restart;
  actual active Slayer640x360,960x540,1280x720,1920x1080 **25 checks/19.754s PASS**.
  Both0provider IO/exceptions. Reviewed all active native sizes, eight software
  normal/Slayer frames, cue/tray, lethal and100ms entry/exit/rapid-award frames.
  Reading overlays obscure the underlying tutorial score; active native frames
  separately prove actual gameplay visibility. No device-FPS/audio claim.
- Independent960x540 screenshot pixels: visible glyph gap4, both centers480,
  whole visible group center104 equals upper black frame208/2. Fonts48/24, normal
 24/16/margins24, canvas scale1; score rules/events/popup/tray unchanged by this task.
- Wireframe r4 retained/check PASS, r5/check PASS, fingerprint
  `75be18aa46b4bedddb86b186a079b53e7c64984f46d56c6cb60922b1366402c9`.
  Source, failed approaches, scope and cleanup receipt:docs/plans/combat-hud.md.
  QA only; no production/release/publication/stress/soak or real provider writes.

## BCompact 12-particle revision (2026-10-19, review sample; performance caveat)

- Final Quick20261019T055443 **77/77 PASS**,0failed/skipped; input
  dd83bea99d6378e96deffb4c3cffd832db008e885584292ba535b4a452c2b6e5.
  Covers12particles/clash,96over eight leases, six main slots with shared rotation
  and +/-2degree jitter,12owner emissions, mirror X/preserved Y/Z, fixed dimensions
  and lifetime/custom data, B/compact restoration and pooled lifecycle.
- Immutable normal055551/mirror055822 QA builds PASS; normal input matches Quick.
  Mirror adds only the existing disposable editor actor-mirroring fixture.
- Native Edge155.0.4283.62/GTX1650Ti ANGLE D3D11: normal1602/mirror1782checks PASS,
 10real keyboard parries per size/direction,40total. No exceptions/provider writes.
  Mirror first430/149check runs failed after missed input; capture harness polling5ms
  and40ms earlier key timing resolved that run without changing product timing.
- Inspected four40ms-spaced frames and native peak at each size/direction. Rays
  separate on travel; sword edge remains readable through brief contact overlap.
  Short rays at640are less distinct during contact than after travel. Art judgment
  is based on these recorded samples; it is not a deterministic RNG guarantee or
  human approval.
- Two10vs12 MP4s retained at E:/CodexTemp/TypingHero/parry-clash-lighting,
  44.8s/25fps/1120frames each: both directions,1x and0.25x nearest-neighbor closeups,
  native-size panes1280x360/2560x720; full decode PASS. Existing10particle clips kept.
- Lifecycle PASS1241checks: both B<->compact directions/sizes, paused emitted
  state preserved, next emission changed and restart cleared leases/restored Current.
- Initial sampled p95 exceeded10% at1280 in both directions; all eight ordered
 10/12/12/10 repeat runs PASS. Mean of per-run p95s (10->12, milliseconds):

  | Direction/size |10 particles|12 particles|Change|
  |---|---|---|---|
  |normal1280|16.3175|16.9275|+3.74%|
  |normal640|15.8325|15.6600|-1.09%|
  |mirror1280|15.2500|20.0025|+31.16%|
  |mirror640|14.9300|17.9950|+20.53%|

  Mirror performance concern remains; do not claim equivalent performance. Four
  keyboard parries/row,94-101all-state samples, only5-7live-clash frames, visible/
  focused tab and no recording/screenshots. Same QA observer/player environment;
  tail variation does not establish GPU cause or a no-spike guarantee.
- Updated Edge QA is paused at1280x720 with BCompact selected on loopback8139;
  owned PowerShell18504/Node17856. Ready screenshot inspected.
- Current remains serialized default, no production/itch/integration change.
  See [checkpoint](../plans/combat-feedback.md#bcompact-12-particles-2026-10-19-review-sample-performance-caveat).

## BCompact verification (2026-10-19, ready for art review)

- Final Quick20261019T050959 **77/77 PASS**,0failed/skipped; input
  c40ca3d7188dacc63a7718d61629946d4725de2506db391d1638ae9b10a8538c.
  Tests cover integer sizes/lifetimes/mirror/hot-head orientation, stable Custom1
  and width until death, B/compact renderer/material/module restoration,
  12owner emissions over8leases/80particles, forced compact migration and two
  tuning-preserving builder reruns, compact reading/hitstop/single release and
  existing unload/light behavior. Earlier76/77 measured the deliberately empty
  Front system after switching the lifecycle test to compact; corrected to MainRays.
- Normal QA044126 input3da8b6d3b176dcc40b2e4799efb77715cf2db27267830cf82056a431988961d8;
  mirror QA044505 inputf5f6c1a4ce5b886ca226890aef6035a575f9d51a3631803f8844d519a13c053d.
  Both build/repeated-builder and serialized migration pass. Reverting only the
  final lifecycle test's Front->MainRays selector reproduces the normal player
  input exactly; runtime/editor/shader/material bytes are unchanged after build.
- Native normal3424checks PASS/189.047s: B/BCompact each10real keyboard clashes
  at1280x720/640x360, ordinary/Slayer hits without particles. Edge155.0.4283.62,
  Playwright1.58.2, actual NVIDIA GTX1650Ti/ANGLE D3D11;0exceptions/provider writes.
  Firebase interception/external blocking and observation-only bridge retained.
- Qualified mirror rows each have10clashes: B1280/640 from partial1613check
  run; compact1280 from partial731check run; compact640 from PASS806/51.720s.
  Failed full profiles remain failed; their already-completed rows are reused.
  This establishes80art clashes across2presets/2directions/2sizes, with no runtime
  exceptions/provider writes. Normal/Slayer particle exclusion checks pass.
- F6/pause/restart passes both B->compact and compact->B at both sizes. Qualified
  rows are B1280/640 from partial373check run, compact1280 from partial205check
  run, compact640 from PASS170/19.514s. Existing particles retain preset/count/
  age/Y bounds through paused F6; next clash uses the selected preset; restart
  removes old roots and the new parry uses Current. Partial profiles are not
  described as full PASS. Compact640 capture requires5ms polling and immediate
  real Escape; the earlier100ms observer pause could arrive after particles died.
- Controlled native headless no-video performance: normal1506/mirror1569checks
  PASS, four real parries per row, visible/focused tab and same player/machine.
  All-particle-state sampled frame p95 (milliseconds; B->compact):

  | Direction/viewport | Samples B/compact | p95 B/compact | Change |
  |---|---:|---:|---:|
  | Normal1280 |97/97|17.710/18.670|+5.42%|
  | Normal640 |99/97|15.360/14.575|-5.11%|
  | Mirror1280 |100/100|17.975/15.550|-13.49%|
  | Mirror640 |98/98|16.685/14.735|-11.69%|

  Live-only counts12/5,13/7,13/6,13/6; corresponding p95 B/compact are
  19.360/20.040,16.065/14.335,19.375/13.600,16.710/13.255ms. Maximum live increase
  +3.51%. No positive increase exceeded10%, so the conditional balanced repeat
  was not triggered. Short sampled QA deltas include observer/idle overhead;
  live samples are sparse and durations differ. No GPU/device-FPS guarantee or
  general optimization/no-spike claim; no stress/soak or production rebuild.
- Review MP4s ParrySpark-BCompact-640.mp4 and ParrySpark-BCompact-1280.mp4:
  44.8s/25fps/1120frames, two native-size panes at1280x360/2560x720 output,
  both directions with20s1x+2.4s0.25x nearest-neighbor close-up per direction.
  Full decode/local-server SHA256 PASS. Actual body/leading-head/cluster frames
  inspected at both sizes/directions, including640compact17.88/17.92/17.96s.
  Different RNG contacts are not a deterministic replay. Art choice remains open.
- Software112/128/143check attempts missed timed parries; no software PASS.
  Native retries also missed real inputs or captured too late. Helpers corrected
  profile flags/scenario before tests, SSH stdin EOF, video-path-before-close,
  real canvas focus after resume and fast transient polling. No product timing,
  fixture or acceptance threshold changed; no failed full profile is a PASS.
- Current/itch and prior clips/players are preserved. Final captures, canonical
  players/reports and interactive Edge stay in the existing task temp root for
  requested art review. Hashes/reopen/cleanup are in the
  [checkpoint](../plans/combat-feedback.md#bcompact-implementation-2026-10-19-ready-for-art-review).


## Current / A / B / C spark review (2026-10-19)

- Quick032249 **76/76 PASS**, snapshot8ac5244b36c022e077329936b6fa8487847271e248a6972bdde68a90145f6ab7.
  Preset bounds/mirror/lifetime Custom1, owner sequence across12emissions/eight
  leases, migration reruns/tuning/identity and existing lifecycle/light tests pass.
- Normal QA031618/mirror032014 build and repeated-builder PASS. Native headed
  Edge155.0.4283.62/Playwright1.58.2/NVIDIA GTX1650Ti D3D11:9068/10501checks PASS,
  ten real keyboard clashes/preset/direction/size (160total), normal/Slayer no burst.
  F6 while paused/next-clash semantics/restart:1718checks PASS. All0exceptions/
  provider writes, Firebase fixture and external blocking retained, bridge read-only.
- Sampled p95 required investigation: headed idle spikes and an invalid diagnostic
  Current1006ms baseline prevent a clean initial comparison. Final focused/visible
  headless no-recording runs:normal3789/mirror2689checks PASS, four clashes/row.
  Normal p95 Current18.685/14.230ms(1280/640),A13.705/14.335,B13.500/14.945,
  C14.115/14.525 (largest positive+5.03%). Mirror A640+16.26% triggered balanced
  Current/A/A/Current:1862checks PASS,8clashes per preset,208/207samples,
  p9515.160/14.645ms(-3.40%), live15.845/14.030 and idle14.845/14.385ms.
  Sparse live-only C640normal spike21.370versus15.035ms (13samples each) did not
  repeat in mirror17.910versus24.010ms. No stable >10% regression established;
  these short observed QA deltas do not establish whole-device/GPU FPS or no spikes.
- Normal032354 build PASS on final Quick source; qa=false, bridge markers and F6
  label absent from JS/WASM/data. Generated prefab retains26IDs/ordered GUIDs/meta,
  normalized SHA2569a7d954cc829d892a2f7838b2a8017b4b6997ab028b46d31328112080dd719c1.
  Local runtime/editor/test/shader inputs match verified source; only generated
  prefab empty-field whitespace normalized. No publish/merge/push/default change.
- Two final four-pane MP4s in E:/CodexTemp/TypingHero/parry-clash-lighting:
  ParrySpark-ABC-640.mp4 and ParrySpark-ABC-1280.mp4,44.8s/25fps, both directions,
  1x plus0.25x nearest-neighbor close-up. Render sizes640x360/1280x720; output grids
  1280x720/2560x1440. Decode/local-server hashes PASS, actual contact/body frames
  reviewed after rejecting cue-selected close-ups. Art choice remains pending.
- Edge interactive host8138 is kept paused with Current/F6 label for user review.
  Reopen/cleanup/PIDs/source hashes and diagnostic failures are in
  [combat-feedback checkpoint](../plans/combat-feedback.md#interactive-qa-and-retention).


## Thicker rays / camera fragments art sample (2026-10-19)

- Quick20261019T025524 **75/75 PASS**,0failures/skips, diagnostic input
  `e216a12e95dcbdfa0ffa7026b806dcb780533cb61a039219178c29e0bd2d98ea`.
  Tests cover32seeded bursts mirrored in X, stratified angle and burst-energy
  limits, three near fragments/Z4?6, back Z2?4, length/mask/body/head data stable
  through shrink/particle-array compaction until death, clean reuse and two builder
  reruns retaining tuning/references after forced shape migration. Existing eight-
  lease/single light/pause/tutorial/hitstop/disable/unload tests also pass.
- Normal QA024122 (input `ed929399dd2986bee5ac5fcb1af8d17401ae7988153cc0e4ba5ca4c1d9eb9d24`)
  and mirror QA025210 (`9f82692511765f73b42c744dda205e49c619bbb18e420df01057888e8deee4d1`)
  build/repeat-builder PASS. Software browser fails94checks in each from a missed
  timed parry/HP loss; neither is a software-profile PASS. Immutable players were
  preserved before lock release. Normal/mirror product code is identical; mirror
  adds only the existing disposable editor actor-mirroring fixture. Final Quick
  adds only the stronger lifetime/compaction test to that product source.
- Serialized migration copied back in place: all26old object IDs and ordered GUID
  references identical, prefab.meta byte-identical. Changes are two head widths,
  three renderer scales/vertex stream lists and nine new serialized settings.
  Local prefab matches generated semantics, stripping only trailing empty-field
  whitespace. No camera, normal-map, animation, gameplay or material changes.
- Native Windows Edge155.0.4283.62/Playwright1.58.2, requested ANGLE D3D11,
  actual1280x720 and640x360: normal2197checks/89.168s and mirror2412/85.412s PASS,
  each ten real keyboard clashes per size. Initial driver reaches Slayer at7;
  supplemental driver types during Slayer, restarts, then finishes10. It asserts
  normal/Slayer hits own no burst. Bridge stays read-only, Firebase fixture/external
  network blocking retained; all runs0JS/managed exceptions/provider writes.
  Same-player live-clash pause120ms/restart/new parry **256checks/24.054s PASS**.
- Same-machine/driver fresh old-player baselines: normal2533checks/86.869s,
  mirror2545/83.001s, ten clashes per size. Normal p95(old?new repeat) at1280:
  14.495?14.750ms(+1.8%);640:14.345?14.410ms(+0.5%). Mirror:
  14.160?14.335ms(+1.2%);14.295?14.105ms(-1.3%). First new normal1280p95
  17.060ms(+17.7%) triggered investigation/repeat: idle p95 was16.735ms as well;
  repeat2369checks/84.474s PASS did not reproduce the >10% increase. Median
  observed allocation582B/frame unchanged. These are sampled QA delta-time values,
  not GPU timings or a whole-device performance guarantee. Server Unity is pinned
 2022.3.25f1,2CPU/8GiB; software browser1GiBshm. Native runs use this same Windows
  machine/adapters (Intel UHD/GTX1650Ti); active adapter was not separately queried.
- New before/after clips both28.800s/25fps/2560x720: each contains right+left,
 12s at1x plus0.6s slowed to0.25x per direction, nearest-neighbor close-up.
  Old/new use the same state-driven keyboard scenario/camera, not a locked RNG
  replay. Reviewed direct PNG/contact sequences and exported comparison frames:
  main bodies read at640, stepped hot heads lead both directions, short larger
  foreground fragments and actor-occluded back layer remain visible, no observed
  mask flicker/pink or continuous hair-width shrink. Human art approval remains.
  Video is25fps and compressed; this does not prove absence of sub-frame artifacts.
- Both MP4s decode cleanly; local/server SHA256 match.1280:8,275,718bytes,
  `88698d491e8790720456ea37f631f57efee9cc8de4fc7498aa6dca5609e43ad5`;
 640:7,937,651bytes,
  `eea577591adeafc98586645c563c11fd5d497c45e9b646077dc0e442ceb11b8a`.
  Review files, old/new immutable players and native receipts remain under
  `E:/CodexTemp/TypingHero/parry-clash-lighting` for art review. Earlier review
  clips/players preserved. Current thickness revision has no production build,
  merge/push or itch upload. B/C and parent integration remain separate.

## Parry pixel prototype publication (2026-10-19)

- User explicitly requested publishing the reviewed ray revision. Shipped the
  verified child prototype; B/C normal authoring and parent integration remain
  separate. Parent Endless/HUD/provider WIP was excluded, no merge/push.
- Production20261019T015346 PASS, same verified diagnostic input
  50d2b168619477d87596fd32ffcc5b441b7a5d763c612354e30d3aea5cfa46bd;
  raw child46f55c0090cfc4411cc8e8b49269d562afe9b4472fa7bb6b7a9cf96e6da461be.
  All JS/WASM/data lack typingHeroQA/TypingHeroQA_Publish/GameLoopProbe.
- ZIP29members/16,398,742bytes, local/server SHA256 and member CRC PASS:
  06415cfd0d002d60e526d994124ac35ea83388a822bfdac03458d39ebde23f6a.
- Exact production HTML, native Edge155/Intel UHD D3D11: real name/Enter,
  BLADE score10/ATTACK COMPLETE, accuracy2/2, cue-timed parry2/2/COMPLETE,
  visible clash material/light, pause and native Resume after640x360 resize.
  Independently inspected resumed640 frame. QA undefined,0JS exceptions/provider
  writes. Initial blind Space/full-frame delayed cue attempts missed the lesson;
  final cropped visual-cue capture pressed immediately and succeeded. No product
  timing/threshold/input or test expectation changed.
- Native helper corrections: first HTTP load preceded completed ZIP extraction
  and got404; rerun only after hash/CRC/extraction passed. A screenshot/OCR helper
  returned500 after two successful parry inputs; a fresh actual-canvas OCR read
  and images confirmed2/2 COMPLETE. Initial Escape after resize did not resume;
  corrected QA uses the actual Resume button and asserts overlay disappearance.
  Existing video.path-before-close helper replaced a hanging saveAs-after-close.
  These were harness issues; final independent behavior checks pass.
- Initial Orca manager hit Cloudflare. Restored existing BrowserSkill daemon with
  original OS user/default home, reused verified Edge61a46c05 login, own Agent
  Window only. No credentials/cookies exported or browser settings changed.
  Idle session expired before upload; re-bound Edge and verified account, same
  seven old uploads and unchanged settings before publishing.
- Saved new playable upload19865461 to https://micharel09.itch.io/typing-hero,
  game3857509. Seven prior uploads retained hidden, including previous playable
  19847423. Storefront/embed/pricing/gallery settings hash unchanged:
  1fabaea4decac5ab34647f4249f96700ea29cbd34bd6a7b6a561b844062d445f.
- Public page reports the new ZIP and Run game. Anonymous CDN28non-HTML hashes
  match release; HTML differs only by itch htmlgame.js integration. New iframe:
  https://html-classic.itch.zone/html/19865461/index.html?v=1792376251.
  Same hosted payload menu visually verified1280x720 after Unity splash, QA absent,
  external/provider routes blocked/fixtured,0exceptions/writes. No public gameplay
  or real leaderboard writes. Earlier one-second hosted capture was splash only;
  discarded after waiting past the splash and checking the actual menu.
- Release ZIP/release.json/published.json retained for rollback under
  /data/typinghero-game-loop/releases/parry-pixel-20261019/. Prior release retained.
  Child feat/pixel-hit-sparks and pre-existing review artifacts remain for B/C and
  later verified parent integration; remove child only after that work is complete.

## Pixel parry ray revision (2026-10-18)

- Supersedes the ray shape/count split of the earlier prototype below; world
  lighting, draft normals, camera and gameplay are unchanged. Art approval remains
  required before B/C or integration/publication.
- Quick74/74 PASS, own20261018T160100-quick; normal QA155853 build/repeat-builder
  PASS, same input50d2b168619477d87596fd32ffcc5b441b7a5d763c612354e30d3aea5cfa46bd.
  Tests now verify five-system count/width split, every group's pause age/position,
  and two builder runs preserving tuned width/renderer values, GUID and references.
- Mirrored QA160147 build/repeat-builder PASS, input
  654d8f6e25ed5cef6eb7976e1214619ec75f457f8708b12a53805826d75ababa;
  disposable actor-position/scale fixture only, excluded from product checkout.
- Native Edge155/Intel UHD ANGLE D3D11/Playwright1.58.2, Development allocation
  players: normal2304checks/67.374s, mirrored2330checks/67.418s PASS at1280x720 and
  640x360. Each has0exceptions/0provider reads/writes; external block/fixtures on.
  Observed forward meanX6.025/6.806 and mirrored-8.427/-4.830, nonzero Z velocity;
  only successful parry bursts, normal/Slayer no hit burst, both reach Slayer.
- Reviewed native contact sequences and full frames at both sizes/directions:
  main rays now remain visible beside slash at640; hard edges, stepped narrower
  body/tail and leading hot heads in both directions, readable swords/pose/cue/HUD,
  no pink edge or wide added halo. Initial UV0-leading assumption was reversed;
  final shader uses UV y=1 for the leading end. No camera/screen snapping.
- Fresh same-device baseline2134checks/66.345s PASS, same input scenario and flags;
  independent stochastic bursts. Unpaused/non-Slayer distinct published samples:

  |Render|Frames before/after|Frame ms p50 before/after|Frame ms p95 before/after|Allocation B/frame p95 before/after|Max GameObjects before/after|
  |---|---|---|---|---|---|
  |1280x720|168/161|31.533/32.525|33.280/33.465|1038/894|227/229|
  |640x360|164/160|32.653/32.630|33.585/33.380|894/814|227/229|

  Allocation median582B/frame both. p95 changes+0.56%/-0.61%, no10% trigger.
  One new child per lease (+prefab and its live copy in this sample);8lease cap,
  66particles/clash (528max) and single pulse light unchanged. These instrumented
  short samples do not establish full milestone C or device FPS guarantees.
- Migration preserves22/22 existing prefab IDs and all existing GUIDs; adds only
  four IDs (one GameObject/Transform/ParticleSystem/Renderer), plus two role
  materials. First revision Quick73/normal native2276 passed but UV was reversed;
  superseded for art. Software browser120checks(normal)/62(mirror) failed on missed
  defensive input, not exceptions: no full software profile PASS claimed.
- Review clips ParrySpark-BeforeAfter-1280.mp4 and ParrySpark-BeforeAfter-640.mp4
  remain in E:/CodexTemp/TypingHero/parry-clash-lighting with approved prior clips,
  native evidence/players and helpers. Side-by-side before/after, both directions,
  actual25fps,1x and0.25x nearest-neighbor crops without interpolation. Keep for art
  review/continuation, clean after approved integration per combat-feedback plan.


- Same immutable normal player: supplemental real-keyboard live-clash pause120ms,
  held particle count/age/world bounds, restart removes roots and new parry works,
  both1280x720/640x360:313checks PASS20.937s,0exceptions/provider writes.
- Input identity reconciliation PASS: raw checkout build inputs
  46f55c0090cfc4411cc8e8b49269d562afe9b4472fa7bb6b7a9cf96e6da461be;
  existing isolated wrapper adds only editor stage-log statements, yielding the
  verified50d2b168... Quick/normal marker. Adding only the disposable mirror editor
  fixture to raw inputs yields654d8f6e..., matching mirror marker exactly. No
  diagnostic or mirror fixture was added to product source.
- Both side-by-side exports decode cleanly,25fps/7.040s/2560x720; each half shows
  the labeled native render size enlarged with nearest-neighbor when needed.
  1280 clip1941603bytes SHA256204f58e2f5f27c77ce4f06ceb16394c07acda4da7db078c7052fce2c774703b2;
  640 clip1599056bytes SHA25698e6b5d9b9b738b91600dcfd21863ac4fdaa68ccce70ac0f311199eaffa71995.
  Local/server hashes match. Prior two review MP4 hashes remain unchanged.

## 2.5D lighting / parry clash prototype (2026-10-18)

- Milestone A only, awaiting user art approval. Feature child remains isolated on
  feat/pixel-hit-sparks, base main701830b. Parent Endless/HUD/QA WIP untouched.
- Quick73/73 PASS before generated assets6497f555..., then **73/73 PASS** with
  imported Unity assets, inputc352ac1237c8e406dbd994289484254555e1d6dce8b2e983239258ad1753d183.
  Tests cover explicit emission, mirrored/depth force, eight leases, hitstop/pause,
  release/reuse, strongest/capped pulse hold/cancel and unload. Runner contracts8/8.
- Own normal QA144811-webgl input94ac9c144bf1045008b98934a6b69c52ae256a59eaece1b18796f6df3e04c263:
  build and repeated-builder PASS; includes temporary editor stage logs only in
  diagnostic snapshot. Mirrored QA145731 inputf0945b7991079db5d51b34ea63df5dcf6873642d56b0acc48f2d88fda03715eb
  adds only disposable editor actor-position/scale fixture. Runtime gameplay code
  is unchanged between normal/mirror players. Both software browser attempts
  took damage before the art sequence completed; not full software profile PASS.
- Native Edge155.0.4283.62 / Intel UHD ANGLE D3D11 / Playwright1.58.2, Development
  allocation build, actual1280x720 and640x360: forward **1767checks PASS/70.047s**,
  mirrored **2223checks PASS/67.548s**, zero JS/Unity exceptions. Read-only bridge,
  fixture routing/external blocking active; zero provider reads/writes in these
  short samples. Restart isolates normal-hit case from parry case. No real scores.
- Actual live clash samples have66/65 particles, nonzero mean depth speed and
  forward X velocity8.127/6.811; mirrored X-4.701/-4.581. Only ParryClash roots
  observed; normal and Slayer have no hit bursts. Both samples reach Slayer.
- Reviewed idle/menu/parry/Slayer frames and contact-frame sequences: white-gold
  pulse on both actors, thin directed streaks, foreground/background occlusion,
  readable cue/text, retained slash/silhouette and no pink materials. Exported
  1x and0.25x crops from actual25fps videos; no interpolation. This does not verify
  every frame/animation, all tutorial/results/transition paths or final surface art.
- All69 imported generated files passed retained-GUID checks; reviewed sprite
  metadata diff adds secondary texture only. New-line whitespace normalization
  preserves baseline matching lines and asset semantics; no behavior changes.
- No GPU regression/allocation acceptance is claimed yet: comparable baseline and
  full milestone C measurement remain after art approval. No merge/production/
  itch upload/push. Owned captures/player/helpers remain in
  E:/CodexTemp/TypingHero/parry-clash-lighting and isolated server task root for
  art review/continuation; see combat-feedback checkpoint for cleanup conditions.


## Pixel spark integration/release preparation (2026-10-18)

- User authorized local main merge and itch.io update. Reviewed/preserved completed
  design and legacy CodeManu foundation in main610352b, merged into the feature at
  372b9f0; current rendering remains physical pixel sparks. Design-only assets are
  excluded from WebGL snapshots and have no runtime links. Legacy notices retained.
- Post-foundation Quick66/66 PASS20261018T095638, source input
  `39d1ff867e100a65ffafddc764561681ce8276372ac49b4c3e25d1302fbdcc44`.
  QA095748 build/repeated-builder PASS. Software browser again failed before first
  hit due slow rendering; no full software profile PASS claimed.
- Same immutable player, native Edge155/Intel UHD ANGLE D3D11: **25,619 checks PASS**,
  207.352s,1280x720/640x360, all four live particle roles, normal/Slayer camera,
  actual keyboard, pause/resume, lethal/results/restart.37fixture reads/two mocked
  writes, zero JS/Unity exceptions. Reviewed final640lethal/normal and retained
  previous art/motion evidence (runtime pixels/physics unchanged by legacy merge).
- Production095953 PASS on that same input. QA=false; JS/WASM/data exclude all
  three bridge markers. Exact original HTML smoke on native Edge155 passes name,
  Enter/tutorial BLADE/ATTACK COMPLETE/score10, real battle word `it`, orange pixel
  impact (visually reviewed, no pink material), pause/resume and actual640x360
  canvas.11fixture reads/zero writes/zero JS or Unity exceptions. Startup logs
  identify unsupported optional URP EASU/HDR-debug shaders on this GPU; the actual
  game and pixel material render correctly. No pipeline change made for these.
- ZIP29members/16,311,433bytes, local/server SHA256 and CRC PASS:
  `c58174cc5e84498e41151a3c006b853b528117a69dbff91a8c720a02c88eee08`.
  Candidate ZIP/manifest retained under server releases/pixel-sparks-20261018.
  Existing score-hud release remains untouched. Production smoke tool corrected
  OCR control-character JSON parsing; final explicit context shutdown and Node
  exit confirmed. A saveAs after persistent-context close failed, so non-QA
  evidence uses inspected native frames/screenshots, not a finalized export claim.
- Publication is pending: main has further active HUD/scoring/art edits outside
  this feature; do not commit or publish their unfinished source. BrowserSkill
  daemon/IPC works, but extension doctor reports zero connected browsers. User was
  asked to pause concurrent writes and connect the logged-in browser. No target
  main merge or itch upload claimed. See combat-feedback checkpoint for resumption.

## Physical pixel impacts (2026-10-18)

- Final Quick **66/66 PASS**, result20261018T093521-quick, input
  `c6d86c7acee7b370d7185a9eeb1bfa11c8e7578df58706c921771876753a7837`.
  Covers four real prefabs, integer pixel sizes/contact/force, actual gravity/bounce,
  hitbox collider dedup and lethal priority after Slayer exit, same-frame tutorial
  lock, hitstop, pause/results distinction, single release, clean reuse, eight-burst
  owner limits, disable/unload and Inspector tuning/GUID retention. Contracts8/8,
  browser/driver syntax and diff whitespace checks PASS.
- WebGL20261018T093620 build and repeated-builder checks PASS on the same input.
  Serialized outputs match source semantically; all PixelSparks assets/metas matched
  byte-for-byte before removing Unity-generated trailing whitespace for commit. Software input driver failed before first hit at ~198ms/frame;
  this is not a full software WebGL profile PASS. No timing thresholds relaxed.
- Native Edge155/Intel UHD ANGLE D3D11, existing Playwright1.58.2, QA Development
  allocation counters and recorded video: **25,242 checks PASS**,196.756s, exact
  1280x720 and640x360. Real keyboard Normal/Parry/Slayer/Lethal at both sizes,
  ordinary and Slayer camera, pause age/position hold, resume, victory results
  cleanup and restart. All four live particle roles observed; zero JS/Unity
  exceptions,36fixture reads/two intercepted writes, external blocking retained.
  Driver SHA256 `b3b07bf5904b60b0d00946871d7d67367a5c3d296ba2eb9bbee5db3e408fc2b1`.
- Forward parry motion151checks and mirrored parry142checks PASS at both sizes.
  Mirror fixture only changes actor positions/scales during disposable QA build;
  observed negative mean force -2.6764/-2.7063WU/s, with readable normal HUD.
  Actual live/video frames and nearest-neighbor4x crops reviewed: sharp flat
  squares/streaks, blue/orange/ivory distinction, fall/bounce and natural release,
  no halo/pink shader, no old animated impact stacked, preserved slash/flash/
  afterimages and readable cues/poses/text. Direct speaker listening unverified.
- Relative measurements on the same machine/Development flags and keyboard
  sequence, unpaused distinct published frames (QA capture/transition spikes kept):

  | Render | Frame ms p50/p95, before ? after | Allocation B/frame p50/p95, before ? after | Objects p50/p95, before ? after |
  | --- | --- | --- | --- |
  |1280x720|31.280/32.840 ?31.235/31.825|1618/12006 ?636/3980|233/238 ?274/280|
  |640x360|31.260/32.395 ?31.255/32.000|588/11706 ?636/11818|233/239 ?272/279|

  Final maxima243/237particles and8active bursts; no unbounded accumulation.
  Extra objects are bounded pooled roots/two systems/plane per burst, versus one
  animated object previously. Median memory rises ~226KB/231KB; no material frame
  or allocation regression observed. These are instrumented headless recordings,
  not a device-FPS promise. Baseline inputd1898611ff53e4c0c3ce7dd050bcb11961dcb42333fc8df76bcd18763e0263b0,
  player090104: both victories/zero exceptions, but overall3796check status failed
  because640poll missed its180ms legacy lethal (render mask127/video confirmed).
  Its measurement samples remain valid; do not label baseline a full profile PASS.
- No production build, push, release, stress or soak. Main integration remains
  gated by independent uncommitted main edits; see [checkpoint](../plans/combat-feedback.md).

## White CodeManu sparks (2026-10-18)

- Quick **52/52 PASS**, no failed/skipped tests, `20261018T081430-quick`,
  input `6122b54f59213aed994fde9bff82ac1b99970bf1a1ebe19fa3ba8cfe7792de00`.
  EveryCodeManuProfileReleasesOnceAndResetsItsActualAnimation and actual pause/
  hitstop animation-event tests executed and passed. All seven profiles verify
  source, white RGB, alpha-derived size,128cell/pivot64,first20ms,clip/event/reuse.
- Independent asset check confirms64/72/96/56px, exact quarter-turn permutations,
  unchanged retained prefab/controller/clip metadata/GUIDs. Original archive SHA256
  matches the accepted plan; CodeManu CC0 source/license/credits are retained.
- QA WebGL `20261018T081946-webgl` PASS; input
  `2b5fd2a683dbae905469418dbaf6e6c9304585ae0726caaf1c49185630ff648f`.
  Repeat-builder PASS; every generated asset matches local checkout. This input
  adds Unity-generated strips/metas/clips/prefabs and removes4unused Pimen strips
  after Quick; relevant runtime/test source is unchanged. Packaged CodeManu notice
  and license match source. A stale shared Library initially referenced the other
  session's PixelSparkBurst.cs; Unity refreshed/recompiled and completed the build.
- Focused native WebGL **8640checks PASS** in175.171s, exact960x540/640x360;
  Edge155.0.4283.62, Intel UHD Graphics/ANGLE D3D11, Playwright1.63.0. Actual keys
  reach normal/parry/all4Slayer/lethal at each size (mask127/127), fast combos,
  active normal+Slayer spark pause/resume and clean restart. All observed leases
  keep authored scale/order30; post-LateUpdate sorting has0mismatches (player
  Attack/Parry1, skeleton0). Each restart restores40/850HP,score0,no active sparks.
- Reviewed native25fps contact sequences and full frames at both sizes: white
  flash/rays/dispersal at existing contact, Slayer spark above silhouette, gold
  cue and typing words clear. All observed sprite rectangles avoid typing words.
  Recording playbackRate1 advances at .999956x/.999967x over2s; no time stretching.
  This is functional/visual evidence, not a device FPS benchmark or audio review.
- Fixtures:34reads/2intercepted writes;0JS/Unity exceptions, error count0. Existing
  harness blocks other external requests. No real leaderboard write. Final helper
  SHA256 `dcb801b65f56c3ecc36e7b359d46476834111b68994466db646c5209ec2114d8` (temporary harness using
  existing session/start/state/fixture helpers and real keyboard input).
- Earlier native attempts:960covered all7profiles but driver expected enemyHp0
  after destroyed enemy reports-1; another attempt sampled a spark near its end
  before pause arrived. Driver now accepts the documented terminal sentinel with
  finished/victory and retries missed pause contacts, requiring an actual held
  lease before passing. Product/timing/acceptance unchanged; final both-size run
  passes. No full WebGL/stress/soak/production build or itch publication claimed.
- Final scoped diff/whitespace/path review PASS; task captures/videos/client/cache,
  copied QA player and both owned server result folders removed after these
  receipts. Shared source/Library/toolchain/builds and other sessions preserved.
  No branch/worktree/commit/push; see combat-feedback checkpoint.

## Spark clarity and parry sorting (2026-10-18)

- Doctor PASS. `python tools/game_loop/run.py quick` PASS **52/52**,
  result20261018T072736-quick, input
  `e4f99c581d3125aa640dcac9403ae6fc9d983598aa03cad7a9a4591206d4760d`.
  Actual-controller checks cover Attack/Parry tags, attack/parry/success crossfades,
  idle/hit/death interruptions, pause/resume, disable/re-enable and captured order7.
  All7real prefabs verify48/56/64/28px scales, order30, unchanged clip duration/
  release event, actual animation completion and reused scale/color/speed reset.
- QA build20261018T072017-webgl and repeat-builder PASS on
  `2b7009e7dc7c6fbccbd28b47d695a96d4535f2798de18832fd47d2018a783553`.
  Final Quick inputs differ only by the7prefabs regenerated by that builder;
  their output bytes match local source. Other generated deltas are whitespace.
  Generated clip/controller/texture/meta bytes and GUIDs remain unchanged.
- Focused native WebGL **5314checks PASS**,171.829s, exact960x540 and640x360,
  Edge155.0.4283.62/Intel UHD Graphics/ANGLE D3D11, Playwright1.63.0. The temporary
  task driver reused browser.cjs session/start/state/fixture helpers, with
  TYPINGHERO_NATIVE=1 and TYPINGHERO_ATTACK_SORTING_ONLY=1; task helper SHA256
  `1390c33165f35871e9961a84af36973727de6039a6e3aad9fb590f800f3e3fc6`.
  Real keyboard normal hits and successful parries at both sizes; post-LateUpdate
  sorting has0mismatches (Attack/Parry1,skeleton0). Both parry states render.
  Parry pause holds order1; resume/restarts restore presentation. At640, all3
  Slayer attack states/all4spark variants, Slayer pause/resume/exit and lethal
  victory pass, ending40/0HP after11parries, spark mask127; restart resets all.
- Reviewed actual video collision frames at both sizes: orange normal/blue parry
  remain at contact; parry sword/trail draws in front of skeleton; gold cue and
  typing tray remain clear. Inspected white Slayer impact frames and lethal.
  All observed active sparks retain order30 and expected scale, with no tray
  overlap.26fixture reads/1intercepted write,0JS/Unity exceptions; all external
  page requests remain blocked by the existing harness. No real leaderboard write.
- The existing software attack-sorting driver stopped before its first normal hit
  after typing across a stale/expired word; this is not a full WebGL profile PASS.
  Native acceptance above uses the same completed QA player. An initial native
  load started before WASM output existed and timed out; rerun began after build.
  No gameplay/input/timing/camera changes or relaxed thresholds. No production
  build/itch.io publication, stress/soak or device-FPS/audio claim.
- Source diff/whitespace/GUID review and task cleanup are recorded in the existing
  [sorting checkpoint](../plans/player-attack-sorting.md). QA captures/temporary
  drivers/reports are disposable; shared Unity source/Library/toolchain/build stay.

## Player attack sorting (2026-10-18)

- Doctor PASS. Quick **51/51**, including four actual-controller/prefab sorting
  tests: all four attacks, interruption/parry/death/idle, transition destination,
  consecutive attacks, pause/resume, disable/re-enable and original-order restore.
  Input `aacc162b49e95ee8d0856ddda79571b93b5efc4d57146f4cbffdf59b30499bfd`.
- Targeted WebGL `--browser-scenario attack-sorting`: **924 checks PASS**,69.609s,
  input `a1a06d81537fe6c96d123f1e47fa393be48b768d50cc2012224f6c80aa54f1b6`.
  Real keyboard normal attack/parries/all three Slayer states (mask14), repeated
  attacks, pause/resume, Slayer exit/VFX fade, hit/death and restart. Post-LateUpdate
  observer recorded zero sorting mismatches: attacks1, skeleton0, restored player0.
- Reviewed actual normal sword/skeleton overlap before/after and all three Slayer
  poses/trails. Chromium153.0.8010.12/SwiftShader,640x360,2CPU/8GiB/shm1GiB;
  17fixture reads/1intercepted write,zero exceptions. No real leaderboard writes.
  Baseline1280x720 was too slow for parry timing (~224ms/frame); use the verified
  640x360 run as functional evidence, not player-device performance evidence.
- Initial test metadata GUID was corrected before final Quick. Initial browser
  driver checked red overlay before its exit fade ended; final driver waits for
  actual restoration. No gameplay timing changed. Runner contracts8/8 and syntax/
  whitespace/GUID checks PASS. No stress/soak/production/native run or publication.
- Task captures/generated reports cleaned after receipts; shared source/Library/
  toolchain/latest QA build remain reusable. See player-attack-sorting checkpoint.

## Impact-based check selection

Identify the changed behavior and affected dependent flows before testing. Select
the smallest checks that prove acceptance; the commands below are available
profiles, not a checklist to execute on every task.

- Do not default to full Quick followed by full WebGL. Use offline tests for
  affected gameplay contracts and real-input browser checks when the changed
  integration or user flow needs them. Add viewports only when layout, camera,
  or responsive behavior is affected.
- The `quick` runner supports `--test-filter` for Unity test names; use it for
  directly affected cases instead of the entire Quick category. WebGL supports `--browser-scenario skeleton-art` and
  `--browser-scenario attack-sorting` and `--browser-scenario score-layout`
  (four-size near-tray HUD plus keyboard Slayer/popup/pause/restart). These do not
  cover arbitrary changes.
  When no suitable filter exists, state that limitation and choose a supported
  targeted verification path that covers acceptance. Run a broader profile only
  when its coverage is needed, not merely because it is convenient. Do not invent
  filter flags or claim a partial run passed the whole profile. Runner changes
  are not an automatic prerequisite for ordinary tasks.
- Expand coverage only for affected shared logic, a relevant failure, a concrete
  unresolved concern, or an explicit request. Briefly explain the reason before
  expanding. Heavy stress/soak needs a relevant performance, pooling, or lifecycle
  question or an explicit request, within the user's current test scope.
- Reuse passing results while relevant source, config, dependencies, and
  environment remain unchanged. Rerun affected checks after changes; stop when
  acceptance and relevant checks pass with no concrete unresolved concern.
- Use doctor when runner/environment verification is needed; setup only when
  necessary. Documentation-only changes need format/path/link and diff checks,
  not Unity or browser profiles. Report unmet acceptance as unverified; profiles
  outside the task scope do not become automatic follow-up work.

Use Python 3 from the repository root:

```text
python tools/game_loop/run.py doctor
python tools/game_loop/run.py setup
python tools/game_loop/run.py quick
python tools/game_loop/run.py webgl
python tools/game_loop/run.py stress
python tools/game_loop/run.py soak
python tools/game_loop/run.py production
python tools/game_loop/run.py windows
```

Runner targets SSH alias `fly-server`. All source snapshots (including uncommitted
tracked/untracked source), toolchain, Library/import cache and results live under
`/data/typinghero-game-loop`. Secrets are excluded: supply an activated Unity
activation state outside the repo in `/data/typinghero-game-loop/unity-home`.
Account licenses use `.config/unity3d/Unity/licenses/*.xml`; legacy ULF files use
`.local/share/unity3d/Unity/Unity_lic.ulf`. An explicit `--license` can mount an
existing matching server ULF instead. The existing GitHub WebGL pipeline uses Unity
account secrets; those are not readable from repository source and are not copied.
Do not log secret contents. Docker containers run sequentially with 2 CPUs/8 GiB.
Chromium has a private 1 GiB shared-memory mount within that memory limit.
Snapshots replace only the task source directory; persistent Library is separate.
Disk checks precede setup, import/build and browser stages. Lock prevents concurrent runs.

Setup measures and verifies the pinned image layers before import; the containerd
storage budget includes compressed content and unpacked snapshots, plus 3 GiB
headroom. Cache bytes are released as the import consumes them. Each execution
stage requires 3 GiB free initially; the task container is stopped if free space
drops below 1 GiB while running. These replace earlier unmeasured 35/12 GiB
estimates. Do not remove unrelated projects or Docker images to gain capacity.

When account activation is needed, `tools/game_loop/activate.py` runs on the server
in a trusted ephemeral Actions job with the pipeline's existing Unity account
secrets. Credentials stay in the job environment; only the resulting license is
persisted in a private task HOME. The official GameCI CLI is pinned to 0.1.72.
Administrative bootstrap helpers remain task artifacts, not a public service.
Activation and runtime use the same configured container hostname/MAC on this server.

Quick runs offline PlayMode NUnit tests with cleanup; test XML must exist and report
nonzero passing tests. Browser runs Chromium on a locally served QA WebGL build.
State comes from the conditional read-only bridge; interactions use actual input.
The QA embedding host sizes the canvas to the requested viewport and verifies Unity's
actual render dimensions. It does not establish responsiveness of the production
HTML template. Leaderboard refresh checks actual row instance IDs and Top fetches,
as well as pool size and retained scroll position.
Firebase reads/writes are fulfilled by fixtures; other external requests abort.
Stress uses large pool/1,000 word/VFX tests and leaderboard fixtures 500/1,000 rows
at 960Ã—600 and 400Ã—600. Soak defaults to 1,800 elapsed seconds, repeatedly completes
words and restarts rounds, checks progress deadlines and records frame time,
memory and object count. Exceptions, missing state/results, timeouts and zero tests fail.
Production builds without `TYPINGHERO_AUTOMATION` and the build hook checks exclusion.

The `windows` profile builds Windows x64 Mono using the existing Ubuntu editor
and an official2022.3.25f1 Windows Mono module mounted read-only from
`~/typinghero-game-loop/toolchain/windows-mono-2022.3.25f1/WindowsStandaloneSupport`.
Output is `/data/typinghero-game-loop/build-windows/CLACKBLADE.exe` with its Data,
DLL and MonoBleedingEdge directories. It uses production defines and verifies
the runtime assembly excludes GameLoopProbe. Do not distribute the executable
alone; ZIP the complete player directory with its credits/licenses.

## Windows portable verification (2026-10-17)

- Doctor, runner contracts8/8 and Quick39/39 passed. Final Windows production
  build: results/20261017T080628-windows, snapshot
  251293113ae167f93a14a1377d31b07a5e6f3b38afa63d7ba8ab09d584472027.
- x64 PE headers/player/Mono dependencies, both enabled scenes, production flag,
  all118managed DLL QA-marker scans and restored player settings passed. Ten
  notices match prior verified WebGL release byte-for-byte. All951 prior local
  Assets/Packages/ProjectSettings inputs unchanged except the editor build helper.
- Wine9.0/Xvfb startup47.21s, software D3D11: live1280x720 player, MainMenu ready,
  actual frame inspected,0managed exceptions. Networks disconnected before game
  launch; no real Firebase writes. Native keyboard/combat/audio and real Windows
  hardware not tested. Wine ALSA/no-device,DXGI/touch/vsync limitations observed;
  direct speaker listening and device performance remain unverified.
- ZIP173files/33,657,976bytes; server/local entryCRC and archiveCRC PASS:
  CRC32 7aac5c0b, SHA256
  7b7720461515d3cd2ea09996cc7f00c7ec3362435e81fdffa4865f87dc36bb0c.
  Filename TypingHero-Windows-x64-20261017.zip; extract all files before launch.
  Asset/toolchain/cache/build data stay on Ubuntu; only requested final ZIPs are
  delivered locally. Previous ZIPs retained. No publishing or stress/soak.

## Skeleton art verification (2026-10-18)

- Original-alpha/weapon/VFX/native re-export checks PASS on36frames;11 approved
  PNG/native/masks remain byte-identical. Four runtime PNGs integrated, importers
  and skeleton clips/events/prefab/combat unchanged. Hurt1 baked flash unchanged.
- Doctor PASS; final Quick51/51 PASS, input
  `a1a06d81537fe6c96d123f1e47fa393be48b768d50cc2012224f6c80aa54f1b6`.
- QA WebGL build input
  `0fa4d3e9c89ea6606e4a2b2aa5c03c2627cdefaccd53c725c7cecf4b4620c45d`;
  subsequent edits are observation/driver/host-path changes, not player code/assets.
  Final targeted native WebGL32/32 PASS in122.675s: exact1280x720 and640x360,
  real typing/Hurt, cue/5HP impact, parry/Stun/Slayer, death/cleanup, pause/restart.
  Edge155.0.4283.62/Intel UHD Graphics D3D11, Playwright1.58.2 reused locally;
  28fixture reads/2mock writes,0exceptions; external page requests blocked.
- Runtime16frames reviewed: bright bone/gaps, mirror, weapon/slash, original
  flash/Slayer silhouettes and Die/restart. All36 source frames reviewed separately.
  Driver SHA256`b0a3c67081ad9241b824c407c77f24a7bbe613cc6f7f124e09b50d4fabdd7468`.
- SwiftShader2CPU/8GiB attempts are failed measurements, not a full WebGL PASS:
  a409.355ms frame yielded sampled cue lead.604345s, exceeding.6s. Thresholds
  unchanged; final native lead samples.261480/.260160s. Driver fixes synchronized
  pause after Animator, removed capture from timing measurement and avoided empty
  promotion/hit-locked input. No device-FPS, release,stress or soak claim.
- Full receipt, source ownership and temporary cleanup: skeleton-hero-art plan.

## Baseline

No completed soak baseline. The user stopped heavy testing during the post-fix
soak at approximately 1,355 elapsed browser seconds (51 rounds, 750 completed
words observed). This is partial diagnostic evidence, not a 1,800-second pass.
Before the core-match task, the production QA-exclusion build was cancelled before starting; the completed core-match production result is recorded below.
Select checks using the impact-based guidance above; full Quick/WebGL are not
defaults. Heavy profiles are opt-in or justified by the affected behavior,
within the user's current scope.
First successful full soak, when requested,
must record commit/snapshot hash, image digest, CPU/RAM limits, Chromium version,
viewport, elapsed duration, samples/progress, frame percentiles, memory/object
range and exceptions. Headless software rendering measures only regression under
the same environment; it does not define player-device FPS acceptance.

The QA probe samples frame time, allocated Unity memory and loaded GameObject count
every 100 ms; browser reports retain approximately one sample per second. Its scene
queries and allocations affect measurements. Compare instrumented runs with the
same toolchain/limits rather than extrapolating to a production build.

## Initial environment (2026-10-16)

fly-server: Ubuntu 26.04 LTS, kernel 7.0.0-34-generic, Intel Core i5-4570 at 3.20 GHz
(4 logical CPUs), Docker 29.7.2, host RAM 15 GiB, `/data` free 22 GiB initially.
Docker-managed files use `/data/docker`; containerd image contents/snapshots use
`/data/containerd` (verified from active config), on the same filesystem as task cache.
Unity 2022.3.25f1 and Test Framework 1.1.33 unchanged. Unity image imported and
account activation succeeded in the trusted ephemeral Actions job:
https://github.com/micharel09/TypingHero/actions/runs/38856603799
License remains only in the private server HOME. The ephemeral runner deregistered
and its binaries were removed after the job. GameCI requires a disposable empty
Git repository in the activation snapshot; the user checkout is never initialized
or committed by this bootstrap.

Quick passed all 10 actual PlayMode cases after the hitstop regression fix.
The new regression failed before the fix (expected original scale 0.75, actual
0.0001) and passed afterward with stacking both enabled and disabled.
WebGL passed 13 checks after that fix, including Slayer and round restart.
Stress passed 3 Unity tests and 34 browser checks across all four dataset/viewport
combinations. Its typing/VFX/leaderboard inputs are unchanged by the hitstop fix.
Latest verified WebGL/partial-soak snapshot:
`c9c37e2ebb05beaf51bb92de531a7644dc313548bafa5fb769b9d3d91b2c5fdc`.
WebGL initially stopped at the disk preflight (2.5 GiB available, 3 GiB required).
Removed only task-owned duplicate extracted global package cache after verifying
all packages exist in retained Library/PackageCache, plus the completed ephemeral
runner. This restored 4.2 GiB; WebGL resumed. npm download archives, Library, Unity
image and toolchain remain cached. Full soak and production remain unverified,
following the user's instruction to stop heavy testing.

Infrastructure checks passed: skill validator, relative links, asmdef/metadata,
Python/Node syntax, whitespace and eight Ubuntu runner-result contract tests.
Pinned image storage measured: 4.79 GiB compressed + 9.94 GiB uncompressed layer
tar bytes. Actual Docker 29 OCI import validates canonical image name and exact
manifest ID. Public transfer caches are no longer served; no service was installed.

Browser toolchain smoke passed with the actual 2 CPU / 8 GiB / 1 GiB shared-memory
limits and network disabled: Playwright 1.63.0, Chromium 153.0.8010.12, WebGL2 via
ANGLE Vulkan 1.3.0 SwiftShader (Subzero). Browser image digest:
`sha256:eff16c30e6f3f4af0a03fa4b706120d5e9b0891c344a27d64559aff5900a4a27`.
This smoke verifies browser/graphics prerequisites; actual game profile results
are recorded above. Temporary test reports/build captures were cleaned after
recording conclusions here. Persistent server license/toolchain/Library and the
captured source remain under `/data/typinghero-game-loop` for future runs.
See `docs/plans/typinghero-game-loop.md` for the resume checkpoint and retained server directory.


## Core match acceptance (2026-10-16)

Quick: 26/26 passed on runtime snapshot
`4e1dce5e4580dd356fd45273217ef22f4183dfae2804acb7719a6eeb5fc5f08f`.
Final WebGL: 54 checks passed on snapshot
`c0ecf7d502deb7718350596766730c74ffbb925609ccb0a3ee4441a948c45a5f`.
Only enemy HP/editor serialization changed after the Quick runtime checks.
Three actual keyboard rounds at850HP won in86.524/94.404/90.706 combat seconds,
median90.706s,44.716â€“44.935WPM,94.334â€“95.266%accuracy,78.571â€“81.250%successful parries.
These are reference balance runs, not novice human playtests.

Chromium153.0.8010.12/SwiftShader, 2CPU/8GiB, shm1GiB, network none;
Firebase fixtures cover reads/writes. Read-only probe/frame sampling affects timings.
Real focus loss was checked separately in headed Xvfb Chromium (7 checks passed);
headless focus emulation is not evidence of real focus loss.
Edge61a46c05/155 was inspected at960Ã—540,1280Ã—720,1920Ã—1080,
iframe960Ã—600 and native iframe fullscreen1280Ã—720. No stress/soak rerun for this task.
See [core-match checkpoint](../plans/core-match.md) for acceptance, fixes, HP selection,
fixture incident/remediation and production artifact status.

Production `61ba8e37bdda95125cb1d376a198b00a2d5c612e9111f6dd960f4582b1347e14` passed QA bridge exclusion (qa=false and all JS scanned). ZIP CRC passed; artifact path/hash are in the checkpoint.

Production smoke on Edge61a46c05 passed first-help, pause/resume, defeat25.5s/no-letter accuracy, restart and menu return. Bridge is undefined in browser; Firebase PATCH had mock intervention applied. The only capture error was favicon.ico404 on the QA cleanup page, not a runtime exception. Task captures/hosts/output directories were cleaned after writing receipts.

## Dark Ages UI verification (2026-10-16)

- Final Quick:26/26 PlayMode cases, source snapshot
  `1108a5ebd9b76c1b25b6afb2a82ec3ba9d2cc9dfa3d1a11c49c0d6bb34c3b9ad`.
  Final serialized scenes/prefabs include the role-specific skin and preserved
  original scene IDs, prefab GUIDs and40/850HP values.
- Full WebGL regression:54 checks,381.562s, snapshot
  `886bc0f608d0602e34d81348c327b4309f5b0aff1af1edaa6ec66fe45ef77421`.
  Runtime C# and gameplay configuration unchanged by this skin task; three850HP
  reference wins88.402/91.465/93.900s. This predates appearance-only serialization
  fixes; the final visual integration checks below verify those fixes.
- Final visual/interaction profile:21 checks, QA source
  `6ff902abd1477b6266314f91b2343e3491f41f36a8cf2b951f965fa7c5dcedc3`.
  Reviewed960Ã—540,1280Ã—720,1920Ã—1080,640Ã—360, quiet HUD/typing, native ornate
  corners/tiled edges, button states, full/half/empty bars, help, defeat/victory,
  restart/menu and actual Unicode keyboard entry/persistence of Nguyá»…n.
  Visible-row pool19 for1,000 long-name fixtures; native wheel/thumb to rank1000,
  ellipsis, column alignment and refresh retaining scroll verified.
- Supplementary iframe/fullscreen:5 checks on the same QA build; native name entry,
  Enter/help/game/pause at960Ã—540 and native fullscreen1280Ã—720, images inspected.
  Host fullscreen-button focus must be returned by a native canvas click so the
  iframe player resumes publishing actual render dimensions. An earlier splash
  capture and an earlier insertText-only attempt were rejected as visual/input evidence.
- All browser profiles used Chromium153.0.8010.12/SwiftShader on the existing
  isolated2CPU/8GiB/shm1GiB runner. Firebase reads/writes fulfilled by fixtures,
  external network blocked; no JS/Unity exceptions. Measurements are not device FPS.
  QA embedding host supplies viewport sizing; production HTML template unchanged.
- Repeat builder passed exact serialized scene/four-prefab comparison after two
  Apply calls. Direct prefab changes now record overrides, and reopened visual
  inspection confirms HP remains red and HUD/typing use the dark inset.
- Required native Edge instance61a46c05 is disconnected in BrowserSkill; exact-profile
  QA remains unverified. No stress/soak or itch.io publication in this task.
- Production passed from the final Quick snapshot above; qa=false and all JS exclude the bridge. ZIP C:/Users/ADMIN/Downloads/TypingHero-WebGL-dark-ages-20261016.zip:23 files,15,859,154 bytes, local/server CRC passed, SHA256 7b6ea706fbed516d91223e6cb95fb0fce9c26d52fae38365c205ff0ca48b118. Credits/four licenses included; original core-match ZIP unchanged. See the existing pixel-ui-refresh checkpoint for retained native-Edge QA resources.

## Combat feedback verification (2026-10-17)

- Final build-input SHA256: `f3146419f10dcfd1f9e42bb9237c61913d3958c112ef59945374b68d08b3111e`. Quick **35/35 PASS**;
  WebGL **54 checks PASS**, 378.643 seconds. Repeat builder
  passed exact serialized comparisons, including all new Pimen role assets/meta.
- Final server regression: Chromium153 / SwiftShader, 2CPU/8GiB/shm1GiB,
  viewport960x600; 104 fixture reads / 5 fixture writes, zero JS/Unity exceptions.
  Three 850HP reference wins: 90.295 / 92.321 / 89.434 combat seconds.
  These are functional automation results, not player-device FPS or human feel.
- Native Windows Chromium153, Intel UHD Graphics via ANGLE D3D11: **16 checks PASS**.
  960x540,1280x720,1920x1080,640x360 plus actual iframe/fullscreen, real typing/
  parry/Slayer victory, pause/resume, three restarts at40/850 and menu return.
  20 fixture reads / 1 fixture write, zero exceptions. All network writes intercepted.
- Actual original Pimen sheets, seven one-shot prefab clips and four white Slayer
  derivatives verified: timing100/150/180ms, Point/uncompressed WebGL import,
  alpha preserved, pool release/reset and pause/hitstop/exit/unload behavior.
  Native frame review corrected the old parry anchor to the sword crossing.
  Normal ivory flash/contact, Slayer silhouettes and larger lethal burst verified.
- Impact mute/gain/pitch/pause and separate whoosh/parry voices passed PlayMode.
  **Direct speaker listening remains unverified.** No stress/soak or publication.
- Production PASS, qa=false; QA markers absent from JS/wasm/data. Credits and all
  licenses included. ZIP CRC passed locally and on server. Production ZIP SHA256:
  `f4e64cdbf60ec47616bff549046c6a1984c47e3f2709b62685b07f5d71f97cdd`.
  [WebGL production](C:/Users/ADMIN/Downloads/TypingHero-WebGL-combat-feedback-20261017.zip) and
  [Unity source ZIP](C:/Users/ADMIN/Downloads/TypingHero-Source-combat-feedback-20261017.zip); adjacent SHA256SUMS file
  covers both archives. Prior Dark Ages ZIP retained byte-identically.
- Portable Quick/WebGL/native reports and four actual-render reference frames are
  included under `Verification/` in the source ZIP. Persistent runner evidence:
  `/data/typinghero-game-loop/results/20261017T052707-quick`,
  `/data/typinghero-game-loop/results/20261017T052751-webgl`,
  `/data/typinghero-game-loop/results/20261017T053514-production`. Task temp captures/baselines are removed after packaging;
  reusable runner source/Library/toolchain and result history are retained.

## Shogun presentation verification (2026-10-17)

- Runtime Quick39/39 PASS,snapshot
  fa0f612f9db3a323d0e692e5432ddea4d88b3ee10a4e96d50483471ee82d7592.
  Includes original35 cases,modal focus/hidden-canvas guard,actual UI clip/mixer
  routing,pause-safe voice return/reuse,and hidden pause Submit regression.
- Full WebGL66/66 PASS,390.482s,input
  1f59c5f2c967aadf36b1c6bb090fbfb4045cacecd6d62ad62be4f9a718175379.
  Chromium153/SwiftShader,2CPU/8GiB/shm1GiB,960x600;63fixture reads/5writes,
  zero JS/Unity exceptions. Native scrollbar reaches rank1000; refresh retains
  bottom scroll/pool; modal R/M/Escape/focus ownership and pause hold verified.
  Three850HP reference wins90.505/94.278/83.489s; no balance change.
- Native Windows Chromium151.0.7922.34,Intel UHD ANGLE D3D11:34checks PASS,
  23fixture reads/2writes,zero exceptions. Real typing/parry/Slayer victory,
  defeat,3restarts40/850,errors/timeout,menu/help/pause,4requested resolutions,
  actual focus loss/return,modal1000rows,iframe/fullscreen. Additional6checks
  verify actual gameplay960x540 iframe,1280x720 fullscreen,resume and a keyboard
  slash causing damage.4fixture reads/0writes,zero exceptions.
- Native frames verified jade/coal layers,dark natural edge trees,clear contact
  orange/blue and retained sword trails,cue,typing hierarchy and modal row1000.
  Original sprite profile timing/scale/pool tests remain passing. Direct speaker
  listening is UNVERIFIED; no player-device FPS claim,no stress/soak or publication.
- Final presentation-only deltas after full regression:zero fill inset fixes thin
  bars and obsolete SpriteSwap references are cleared from ColorTint controls.
  Runtime/tests are unchanged. Final serialized builder ran twice and compared
  103 files byte-for-byte:PASS. Input99161950a389a82e1af8792a59fed22077b21a045a5752a9608da59f7e86873b.
- Production inputc5066e5a6d455c309d17e8f1767aff660872ef5c3acddddacfecbc1556c092a1:
  PASS,qa=false; bridge markers absent from JS/WASM/data. Native normal-build
  smoke8checks PASS without a bridge,3fixture reads/0writes,zero exceptions;
  menu/help,4render sizes,pause/modal/close/resume,combat input and menu return.
  Real frame pixels confirm visible HP/stamina/timer fills after inset correction.
  A first smoke driver acted before Unity's first UI frame; it was corrected to
  wait for actual rendered title/HUD text,without changing product code.
- Extra iframe driver initially assumed default2x pause zoom; the serialized scene
  owns1.1x/295ms. Corrected the driver to wait for actual iframe focus/transition;
  product pause settings unchanged. The final gameplay iframe6checks passed.
- Production ZIP28files/15,945,248bytes,server/local ZIP CRC PASS; archive CRC32
  b64fbfdb;SHA256b5e56079314c73fac75e20495db47a4a2e77897fa793f46a81aab21ebced53b3.
  Deliveries: C:/Users/ADMIN/Downloads/TypingHero-WebGL-shogun-20261017.zip and
  TypingHero-Source-shogun-20261017.zip; adjacent
  TypingHero-shogun-20261017-checksums.txt records SHA256/CRC32 for both.
  Source includes credits/licenses,approved wireframes and these written receipts;
  runtime captures are temporary and are not copied into the source archive.
- Persistent runner receipts: /data/typinghero-game-loop/results/20261017T065257-quick,
  20261017T065419-webgl,20261017T070900-serialization,20261017T071107-production.
  Reusable toolchain/Library/results retained. No new branch/worktree/commit.

## Interactive tutorial verification (2026-10-17)

- Doctor PASS with pinned Unity2022.3.25f1 and existing activated Ubuntu toolchain.
  Final runtime Quick44/44 PASS, input
  `957b6cfdee5109b03eb4f43f8f2c9727e7016510022828b06979a9b1034e6935`.
  Runner contracts8/8 PASS on Ubuntu; JS driver syntax checks PASS.
- The same QA build passed the repeat builder's exact serialized comparison.
  Full browser rerun120checks PASS in548.093s: all6lessons, actual hits/parries,
  mistakes/timeout/hit retries, Slayer expiry/restart, skip/replay/preferences,
  reading/pause locks, clean Start Battle, ordinary defeat/victory, full1000-row
  modal and native iframe/fullscreen keyboard interaction.117fixture reads/5normal
  match writes,0tutorial writes,0JS/Unity exceptions. Three850HP reference wins
  91.709/90.535/86.424s. Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB;
  these functional timings are not player-device FPS or human-playtest evidence.
- An earlier final browser attempt passed118checks, then clicked stale Resume
  coordinates immediately after fullscreen resize. The corrected driver waits
  3actual layout frames and the end of Resume's camera transition. Accuracy's
  wrong-key assertion likewise waits for observed input. No game timing, window,
  expectation or passing threshold changed to accommodate the driver.
- Native Edge155.0.4283.45/Intel UHD ANGLE D3D11:55checks PASS, including3actual
  browser focus losses during Parry/Slayer.38fixture reads/0writes,0exceptions.
  Four requested render sizes and actual960x540 iframe/1280x720fullscreen were
  reviewed: full yellow cue, gold focus borders, complete secondary menu labels
  and unobscured characters. Additional same-build native27checks PASS in203.771s:
  fresh-context real BLADE input causes exactly10damage at960x540,1280x720,
  1920x1080and640x360; reading blocks Space/typing at each size.8fixture reads,
  0writes,0exceptions. All four actual hit frames were inspected.
- Serialized MainMenu/SampleScene retain all262/563original document IDs and
  unchanged GUIDs, adding5/89IDs. Other10generated deltas were whitespace only
  and were excluded. Unity/package versions and normal40/850HP/250stamina remain.
  Wireframe r2fingerprint:
  `1e1148f8566a16a8172d1ec0cf8f9d8a71c408f3bed0c8292ef3962b2ec8afa0`.
  Both saved wireframe revisions passed their render/check workflow.
- WebGL production and Windows64 build PASS from identical input
  `ef5da24af7d8f9a5623472d76841f960d9d0d6008f8cb7e0d755637874519ed9`.
  This adds the already-tested serialized scenes and driver-only changes to the
  Quick/QA input; runtime C# is unchanged. Production flags are false; all5WebGL
  JS/WASM/data files and118Windows managed DLLs exclude the QA markers. PE64
  checks pass for executable,UnityPlayer and Mono runtime. Ten credits/licenses
  match byte-for-byte across both players.
- Production browser smoke without a bridge:5checks PASS,7fixture reads/0writes,
  0exceptions. Actual reading/BLADE-complete frames, ordinary pause,1000-row
  leaderboard and returned menu reviewed. First menu capture preceded the first
  UI frame and is rejected as visual evidence; returned menu verifies the layout.
- WebGL ZIP28members,15,953,468bytes,CRC32b89efbbe,SHA256
  `12ec4286fafd69d5a0c1e49272cca9530ea6ec4a6221950c8b0db06118a0c1f0`.
  Windows ZIP174members,33,666,777bytes,CRC32b35fa287,SHA256
  `4aaa37dfc6279104842b1a662536f997391e0ce5faed03f4b235cbdedc53a634`.
  Server member CRC checks passed for both. Downloaded Windows ZIP matches SHA256,
  archive CRC32 and all174member CRCs; the complete executable/Data/Mono/DLL set
  and production metadata were verified locally without extracting assets here.
- Windows ZIP was extracted and launched offline under Wine6.0.3/Xvfb1600x1000:
  actual1280x720 window,D3D11featurelevel11.1,menu/first practice/BLADE-hit frames
  reviewed; score10and ATTACK COMPLETE verify the real hit.0managed exceptions.
  All container networks disconnected before launch; the disposable container
  was removed.233.462s includes Wine package setup and fresh-prefix initialization,
  not player startup time. This does not establish real Windows hardware QA,
  device FPS or speaker output. Direct speaker listening remains unverified.
- Delivery: new Windows ZIP and TypingHero-tutorial-20261017-checksums.txt in
  C:/Users/ADMIN/Downloads. Ten existing TypingHero ZIPs are preserved,including
  the earlier Windows ZIP SHA2567b7720461515d3cd2ea09996cc7f00c7ec3362435e81fdffa4865f87dc36bb0c.
  WebGL and Unity source archives remain only on Ubuntu under
  /data/typinghero-game-loop/releases/tutorial-20261017. The adjacent checksum
  receipt records SHA256/CRC32/bytes for all3archives. Source contains project
  files,credits,approved wireframes and written verification; no caches,builds,
  secrets or runtime QA captures. No itch.io publication or stress/soak.
- Run identifiers: Quick20261017T113032,QA build20261017T113623,production20261017T120740,
  Windows20261017T121315; browser rerun used the unchanged QA build with corrected
  observation-only helpers. Task captures/generated reports/clients and temporary
  hosts are cleaned after these written receipts. Normal source/Library/toolchain/
  build output and release archives stay on Ubuntu; no new branch/worktree/commit.

## Slayer cinematic verification (2026-10-18)

- Doctor and runner contracts8/8 PASS. Quick46/46 at20261018T033558 includes
  score award-only pulse/configured tiers and zoom exit before camera completion.
  Quick snapshot bcd4c310d0f27d734e031e9ae8d42943cffd3a46494dac7fe893354e7747721c.
- QA WebGL20261018T033806 built input
  29dba92a69cbd61c5a6866c7fe982be49549cfdf12bdd0c2c1deb7600f865186.
  Final standing-head inset .28 (versus earlier Quick .55) and serialized bindings
  were verified in this player. Repeat-builder PASS; all652 original scene IDs and
  GUIDs retained,659 total IDs. Local scene matches Unity output after whitespace
  normalization. Subsequent C# delta is trailing whitespace only.
- Browser broad rerun completed134 checks: six tutorial lessons/retries, pause,
  restart/clean battle, ordinary combat/defeat, preferences, modal/embedding and
  three850HP wins91.195/84.933/88.018s. Then its x3 driver expired Slayer while
  waiting/capturing between words, so its overall status is failed, not a full
  profile PASS. That driver-only failure was rechecked separately below; none of
  the already passing gameplay paths or their acceptance thresholds changed.
- Body gap assertions and actual images reviewed at960x540,1280x720,1920x1080,
  640x360: top/bottom gaps8 reference pixels (largest float error .000016), full
  render-width bars, original red background, readable tutorial/words/corner HUD.
  Initial640 resize assertion sampled stale1920 canvas scale; driver now waits
  four actual frames after the observed render-size change. Same QA payload passes.
- Corrected standalone Slayer browser PASS30/30 in50.575s: real keyboard parry/
  typing reaches x2 yellow and x3 RGB(1,0,0), total287 at x3 matches each observed
  character award using existing float32 multiplier and ties-to-even rounding.
  Zoom1.282051; award pulse observed, idle combo does not pulse; pause keeps actual
  score, bars span all4sizes, exit restores environment/panels/scale and restart
  resets the score/layout. Reviewed x2 popup, x3 and restored normal frames.
- Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB. Broad123fixture reads/6mock
  writes; targeted10reads/0writes; no JS/Unity exceptions. All external networking
  blocked; tutorial produces0writes. These are functional checks, not device FPS.
- User deferred release/ZIP. No new production/Windows build or release archive;
  previous ZIPs remain untouched. No native Windows/audio or stress/soak claim.
  Runtime build/source/cache remain on Ubuntu. Task QA captures/reports are removed
  after recording this receipt; corrected drivers remain in tools/game_loop.
- Final source synchronization/Doctor snapshot:
  `6adde6ec8a9bf33e6a8d63b1abb3fe9022a86f80b080893c783a76b67bda7c5d`.
  This includes the actual serialized scene and corrected test drivers; runtime
  logic matches the QA player above. Final Slayer driver SHA256:
  `f8ba00cb8c8c3cbe1852133929eaa3100eef5025c836dc80b11335ae2a3dcab3`.
  All11prior local ZIP SHA256 values unchanged; diff whitespace checks PASS.

## Foot-local combat HUD verification (2026-10-18)

- Doctor PASS before/after; Quick47/47 PASS20261018T041510, snapshot
  `51fe66ab40ef29a8b787fe85a8a8d2f6c37727a7736510e458dc9f4606df6a84`.
  Includes camera-projected feet and centered zoom1/1.282 plus pulse1.2 score.
  Product runtime is unchanged after Quick; subsequent changes are editor layout
  cleanup, actual scene serialization and QA-only observation/driver fixes.
  Runner contracts8/8, JS syntax, whitespace and wireframe r1/r2 checks PASS.
- Final QA player20261018T042543, input
  `ac4c2952c7e5ca6e77e5995702a4549ff276fda7034e6acf32d41488096281cd`.
  Repeat-builder PASS on both builds. SampleScene retains all659 previous IDs,
  adds5/664total, removes no referenced GUID and adds only CombatHUDLayout's GUID.
  Normalized local scene matches the final Unity serialization, SHA256
  `5cd400538351b13aae9669c9065cbc949ad8622c8014f9781d3d18ba9699fc36`.
- Both normal/Slayer at960x540,1280x720,1920x1080,640x360: measured112px status,
  foot gap12, centered672x56 tray/gap12, measured score/multiplier center/top32,
  three words inside tray and HP/stamina fills matching state. Eight real frames
  plus stamina25/250 highlight reviewed; no text clipping/body overlap. Full-width
  bars retain8px gaps; largest measured error.000016 reference pixels.
- Broad player run completed181checks, including six lessons/retries/Slayer
  expiry, pause/restart, ordinary real HP damage/fills, eight-hit defeat and result
  freeze/restart.86fixture reads/2mock writes,0JS/Unity exceptions. It was explicitly
  stopped during the first balance round; it is NOT a full WebGL profile PASS.
  A previous142-check attempt exposed a QA observer dereference after actor
  destruction. Guarding both colliders fixed it; later defeat/victory/restart
  observation continues without exceptions. No product timing/threshold changed.
- Targeted real-input victory against850HP:79.9combat seconds,score2798,
  accuracy98.7%,9parries; actual result frame reviewed. Restart restores all three
  bars,zero score and normal HUD. Targeted run completed12checks before its later
  x3 driver timed out;28fixture reads/2mock writes,0exceptions. The observed yellow
  x2 popup is correctly placed beside the centered score. Capturing it inside the
  short typing window delayed input; remove that capture from the live input loop.
- Same-player standalone Slayer rerun PASS27/27 in46.307s: x2 yellow,x3 pure red,
  score275 matches every observed character award/float rounding,zoom1.282051,
  award-only pulse,idle no pulse,pause at all four render sizes,full-width bars,
  exit restores environment/tray/white score and restart resets presentation.
  Reviewed x3 and exit/error-border frames.9fixture reads/0writes,0exceptions.
  Final slayer.cjs SHA256:
  `880bac734fb635dba2fd0c84926e049a93b047f784f0dc41b3a0b775a2583c4d`.
- Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB,external networking blocked
  and Firebase fixtures retained throughout. These are functional/layout results,
  not device FPS or native/audio verification. No stress/soak/release/ZIP build.
- Final synchronized Doctor snapshot
  `1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588`:
  same product runtime/serialized scene,final QA drivers. Wireframe r2 fingerprint
  `902c0ea39127ccbe8d51208f1bca6fd1c1656b9cdc58dc9172f2dcfa2e690c35`.
  See combat-hud checkpoint for scope and cleanup; source/Library/toolchain/QA
  player remain reusable on Ubuntu. No branch/worktree/commit/push created.
  Four task result directories and combat-hud task-temp/captures on both hosts
  were removed after recording these receipts; design source revisions remain.

## HUD production publication (2026-10-18)

- User subsequently authorized itch.io publication. Production20261018T044548
  built verified input1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588.
  Normal-build QA exclusion PASS; every Build JS/WASM/data file scanned for
  typingHeroQA/TypingHeroQA_Publish/GameLoopProbe markers: absent.
- Original production HTML smoke on Chromium153/SwiftShader,2CPU/8GiB/shm1GiB,
  no network: native name/Enter,real BLADE hit and score10/ATTACK COMPLETE,
  pause/resume,next lesson and640x360 HUD reviewed. Also renders1280x800.
  17fixture reads/0writes/0exceptions; bridge undefined. No product code changed.
- ZIP28members/15,981,634bytes,CRC PASS; downloaded SHA256 matches server:
  `be81e04bb9163ee4cda8c46e16fac12e50e8d976e27761d8986fc947e2cbdeee`.
  Release and per-file manifest retained at
  `/data/typinghero-game-loop/releases/hud-20261018/`.
- Published and saved to https://micharel09.itch.io/typing-hero, game3857509;
  new browser-playable upload19839230. Three prior uploads retained and hidden;
  existing maximized embed/theme/copy/pricing and draft devlog preserved.
- Public Run game renders the menu. Anonymous hosted payload checks match all27
  non-HTML file hashes; index.html only adds itch.io's standard htmlgame.js script.
  No public gameplay/provider writes. Task captures/results cleaned; actual
  server release ZIP/manifest retained. See itch-discovery for publication receipts.

## Score near typing tray (2026-10-18)

- Quick52/52 PASS20261018T063624, input
  `24893dfb136c6a6d19e372ff4413b6d7cca8784b3d333de555d6d98036790d84`.
  Layout regression covers0/6/10digits,x1 hidden/x2/x3,pulse1/1.2,zoom1/1.282;
  fixed tray,centered text,decay within48px and popup gap12. Moving-anchor award
  regression also confirms fade remains half elapsed after relocation.
- QA WebGL20261018T063120,input
  `70c98ea4c61ba45e02061ec219cc459a290b6a5d9901a45ea1b6510952085d85`;
  repeat-builder PASS. Local normalized serialization matches its SampleScene
  (664/664IDs) and floating prefab(5/5IDs),with original GUIDs intact.
- Final targeted score-layout browser PASS166checks in171.817s on the same player;
  corrected JS drivers, no runtime rebuild. Actual tutorial keyboard hits/parries,
  read/pause input locks,normal/Slayer projection at640x360,960x540,1280x720,
  1920x1080; all eight layout frames reviewed. Real ordinary Slayer typing reaches
  x2 yellow/x3red,score275 matches character awards/rounding,pulse then idle,
  font18 award follows anchor,pause holds score,exit restores/restart resets.
  Actual popup and restored normal frames reviewed; no HUD/text/timer overlap or
  gameplay tray overflow. Fixed row reserve holds at maximum tested pulse/zoom.
- Measured center-to-center score-word gap70px normal/66.615px Slayer at all sizes
  (top-to-top60px). Minimum tray bottom43.142px normal/18.080px Slayer.
  Calculated old-layout center gaps from the same observed camera/body projection
  and previous top32/tray-gap12 formulas:368.466-368.858px normal,
  389.469-389.971px Slayer. This baseline is reconstructed geometry,not a before
  screenshot or eye-tracking measurement. New gap is below80px.
- Chromium153.0.8010.12/SwiftShader,2CPU/8GiB/shm1GiB,45fixture reads/0writes,
  zero JS/Unity exceptions; external networking blocked. Functional/layout evidence,
  no device-FPS/native/audio claim. No production build,release or itch publication.
- Earlier browser attempts failed on driver sampling completed tutorial words,
  measuring hidden HUD under the pause camera,then letting Slayer expire while
  taking active screenshots. Final geometry is measured in populated reading states
  after real combat activation; functional Slayer runs separately within the same
  scenario. Pause camera's existing zoom/focus can put the underlying foot-following
  HUD outside the frame under its overlay; resume restores gameplay projection.
  No gameplay timing,fixture,acceptance threshold or product camera changed.
- Wireframe r3 and preserved r2 checks PASS; r3 fingerprint
  `49ad851305cd8144427738228e5136278d376a2644057e7d6a4b394ef0469df6`.
  Runner contracts8/8,driver syntax and task whitespace checks PASS. Written
  receipts remain here/ combat-hud plan; task captures/reports are removed after
  review. Reusable server source/Library/toolchain/QA player remain.

## Score HUD production publication (2026-10-18)

- User subsequently requested itch.io deployment. Production20261018T064806 PASS
  from verified input `e49808a9fe7f779d2bec93160e240e07362bdc002aadec015b3d30a7a56489db`.
  All Build JS/WASM/data exclude typingHeroQA/TypingHeroQA_Publish/GameLoopProbe.
- Exact production HTML, Chromium153/SwiftShader,2CPU/8GiB/shm1GiB/network none:
  real name/Enter/BLADE hit,score10/ATTACK COMPLETE,pause/resume,next lesson and
  640x360 render reviewed. QA bridge undefined;5fixture reads/0writes/0exceptions.
- ZIP28members/15,957,828bytes,CRC PASS; local/server SHA256
  `237151c63e41baa2a64676d026d2e1b19fd6e6383d8f70a61500251c8272d558`.
  Published to https://micharel09.itch.io/typing-hero,game3857509,new upload19840745;
  native Saved confirmation and saved playable/maximized state verified. Four
  previous uploads retained hidden; storefront copy/theme/pricing/devlog untouched.
- Public Run game renders the actual menu; anonymous CDN check matches all27
  non-HTML files. HTML differs only by itch.io's htmlgame.js integration script.
  No public gameplay/provider writes. Release ZIP/release.json/published.json stay
  at `/data/typinghero-game-loop/releases/score-hud-20261018/` for rollback.
  Task captures/reports/results cleaned after written receipts; owned browser
  session closed,shared source/cache/toolchain/builds and browser daemon retained.


## Compact Type Knight HUD (2026-10-18)

- Quick20261018T102205 PASS53/53,0failed/skipped; input
  `6a6e68b810b1e7531268235820aff4f88a6e5364fe5532e8ebd75351114847e2`.
  Layout covers0/6/10digits,x1/x2/x3,pulse1/1.2,normal/Slayer camera; popup tests cover
  live anchor, accumulated deltas/fade interruption, pause, actual lethal Complete,
  destroyed anchor, expiry and restart. Earlier52/52 preceded these final tests.
- QA20261018T101114/repeat-builder PASS; input
  `78e663f836356dddd5bfbf94889aa7965df9e5180af4679a761f54fe93815bf8`.
  Generated scene664/664 and popup5/5 IDs/GUID sets preserved. Normalized serialized
  scene SHA2568f6ec5eb22f8019518d920eda49603a9e31fc3ede0598e17f75ba6f75f443157;
  popup4534fc479780ec79e7afaca9f88981317bb08997363f288181314853af6abe29.
- Targeted score-layout PASS198checks/311.200s, real keyboard normal/parry/rapid
  Slayer,x2yellow/x3red,exact deltas/single popup,pause/resume,lethal/restart and
 1000-row fixture leaderboard/scroll/refresh. All8normal/Slayer layout frames at
 640x360,960x540,1280x720,1920x1080 plus cue/popup/exit/victory reviewed. Top/right24px
  (error<.0001reference pixels),tray gap12,names hidden,Gold stamina,score scale1.
  Victory86.1s,score2499,accuracy100%,9parries. Chromium153.0.8010.12/SwiftShader,
 2CPU/8GiB/shm1GiB;7fixture reads/1intercepted write/0exceptions, external blocking on.
- Same-player native Edge155.0.4283.62/Playwright1.64.1 Slayer PASS50checks/38.961s,
  score275/x3red/scale1, accumulated popup/pause/restart;0fixture reads/writes/errors.
 37.24s recording and100ms frame sequences reviewed: rise/fade, interrupted accumulation,
  tier tints,score pulse,cue/tray clearance. PlaybackRate1:2.012s/2.0121s elapsed,
  ratio.9999503; no retiming. No device FPS or speaker-audio claim.
- Bundle r4/preserved r3 check PASS; r4 fingerprint
  `fa2062713e0aa8e844663fbaf35f372774d246fd8f8bf72d3a7a35c501445523`.
  Runner contracts8/8 PASS on Ubuntu; JS syntax/scoped whitespace review PASS.
  Sources, resolved runner queue/wrapper issue and cleanup receipt:combat-hud plan.
  No production/release/publication,stress/soak or real Firebase writes by this task.
- After written receipts, all task captures/videos/reports/downloaded player/native cache/
  wrappers and3owned server results were removed. Shared source/Library/toolchain/QA
  player and other tasks remain; no new branch/worktree/commit/push.


## Expanded Endless Duel verification (2026-10-19, partial acceptance)

- Quick20261018T164341:88/88 PASS on
  da5b7eaa907fd9aee4cfa7e3f808f0c8df091dfa8649b8351b188dc2b887eb54.
  Covers six real attack clips,300ms cue/pause/single impact, role scheduling/cap,
  riposte recovery, continuation, modal shortcut isolation and local records.
  Loopback HTTP fixtures cover >1000entries/boundary ties, read failures,
  three-conflict limit, concurrent better records and pending-best retry.
- QA20261018T164534-webgl compile/repeat-builder passes on that snapshot.
  Production20261018T165155-production passes on
  4afa1031b50e4a1daaffbf3f932a251ce8100a15250b462d37c40a594127f697;
  ZIP33members,16,481,782bytes, integrity PASS, SHA256
  6acf4d41e41c52dd137ee9500db0a54cf6d127f51e3b439c5eadaa3dc09b9327.
  No QA bridge markers in player files. No production Firebase writes/config edits.
- Native Edge155 real input clears18duels/3rounds with95parries; HP carry/boss heal,
  death/restart/best/menu, profiles/achievements, Classic and network/error/empty/retry
  at960x540/640x360 exercised. Report fails after8934checks only at the final
  invalid fast-input comparison (stale word snapshots,129incorrect letters).
  Corrected driver targeted comparison9checks PASS:12.725s versus17.726s,
  correct input and no submit when abandoning after a win. No runtime code change.
  Normal reference15.01-19.38s, elite25.21-26.54s, boss44.99-46.63s.
- Prior post-Slayer idle failure remains unconfirmed; no reproduction in18wins.
  These receipts do not claim a full-suite pass or resolve that observation.
  See [Endless checkpoint](../plans/endless-duel.md) for exact receipts and limits.
  Prepared build/evidence: Builds/EndlessDuel-20261019/, ignored from Git.
- Three-person human recognition/balance/retry observations remain unverified.

## Compact HUD production publication (2026-10-18)

- Production20261018T130348 PASS on final Quick53 snapshot
 6a6e68b810b1e7531268235820aff4f88a6e5364fe5532e8ebd75351114847e2.
  qa=false; all JS/WASM/data exclude QA markers. Original HTML Edge155 real name/
  Enter/BLADE hit gives10 and enemy+10; pause/Resume/640x360 reviewed,0JS exceptions/
  Firebase writes. Runtime/builder match verified HUD. Existing unsupported-shader
  console messages did not prevent actual correct render; no managed exception.
- ZIP29members/15,984,116bytes/CRC5b3e086b, SHA256
  `31799c809a460e4b1eec86d6fb7eb1d26ba5b57904c2c2209d6cd05eb6648921`.
  Local/server hash and member CRC PASS. Retained release/manifest/published receipt:
  `/data/typinghero-game-loop/releases/compact-hud-r4-20261018/`.
- Published to https://micharel09.itch.io/typing-hero, game3857509, upload19847268.
  Native Saved confirmation; new playable file and five hidden prior uploads verified.
  Existing fullscreen/storefront/theme/pricing/gallery/draft preserved. Standard CDP
  file chooser reached through documented Orca endpoint resolved detached-input upload;
  focused Enter resolved pointer Save attempts with no effect. See combat-hud checkpoint.
- Public page points to19847268?v=1792330953. Anonymous CDN verifies all28non-HTML
  hashes; HTML adds only itch htmlgame.js. Fresh anonymous Edge155 loads actual menu,
  QA undefined, Firebase blocked/no public gameplay/provider writes. Splash capture
  rejected before actual menu; unversioned cached404 and headless Cloudflare responses
  were not used as successful evidence. No new gameplay changes or broadened tests.
- Written receipts recorded before cleanup; local task captures/ZIP/client and owned
  production result removed. Owned browser tabs no longer remain; server release,
  shared source/cache/toolchain/builds and other sessions preserved. Final whitespace PASS.

## Remote branch historical verification receipts

- User requested itch deployment. Production20261018T130348 PASS on the same final
  Quick53 snapshot6a6e68b810b1e7531268235820aff4f88a6e5364fe5532e8ebd75351114847e2.
  qa=false; all JS/WASM/data exclude QA markers. Original-HTML Edge155 real name/
  Enter/BLADE hit gives10 and enemy+10; pause/resume/640x360 reviewed,0JS exceptions/
  Firebase writes. Build and runtime source match verified HUD.
- Retained release compact-hud-r4-20261018:ZIP29members/15,984,116bytes/CRC5b3e086b,
  SHA25631799c809a460e4b1eec86d6fb7eb1d26ba5b57904c2c2209d6cd05eb6648921.
  Local/server hash and member CRC PASS. No upload/save/publication: authenticated
  Orca edit page is ready but its detached native file picker needs user selection.
  Exact blocker, owned tab/local ZIP and resume/cleanup conditions are in combat-hud plan.


## BCompact debris tail (2026-10-19, functional PASS; performance caveat)

- Quick78/78 PASS,0failed/skipped, immutable input
  `63171dbeb36002ddd417fc7b37fdb8830ecc2f807f6d97cb48515aebd4b09efb`,
  `/data/typinghero-game-loop/results/20261019T063719-quick`, pinned Unity2022.3.25f1 on fly-server.
  Includes compact ballistic floor/one-bounce/rest/cooling/alpha/expiry, both directions,
  preset module restoration, pause/hitstop/reuse and96particles/eight leases.
- QA normal063931 and mirror064919 build/repeat-builder PASS. Software browser
  failed106/111checks due missed real-keyboard parry; neither is a full-profile PASS.
- Same immutable players, Edge155.0.4283.62/GTX1650Ti/ANGLE D3D11:
  normal3475/mirror2867checks PASS,10parries per size/direction (40total).
  Lifecycle B/BCompact normal789/mirror685 PASS; tail-frame normal100/mirror120 PASS.
  Real keys only,640x360/1280x720, pause holds, next-emission F6, restart clears;
  fixed small debris arc/low bounce/rest fade inspected with raw videos and PNGs.
  All native reports0fixture reads/writes/JS or managed exceptions; external blocking retained.
- No-video separate performance A=old12/B=tail12. Normal1280/640 p95ms:
  21.600/25.310 ->19.385/17.050. Mirror ABBA1280:21.855/19.155/16.745/14.675;
 640:16.650/21.810/14.030/14.260. Mean per-run p95 delta-1.72%/+15.95%.
  Mirror640 remains above10%; no performance equivalence claim. Earlier10vs12 mirror
  +31.16%/+20.53% is a separate unresolved measurement. No unrelated optimization.
- Two49.6s before/after12particle clips include normal/mirror at1x/.25x; retained
  under task temp with reports/players and a paused Edge QA window.
  Current/default, camera/gameplay and itch unchanged. See combat-feedback checkpoint.

## BCompact continuous 3D sparks (2026-10-19)

- Final Quick073726:78/78 PASS,0failed/skipped; input
  `28e395dab45e1ac3e6c05d2779a47e7b56ccf97878ecf0f5da4322ecd10339de`.
  Tests cover12total/96over8leases, same-frame contact2/middle2/front4/back4/embers0,
  mirror X/preserved Z, integer projected sizes,8continuous falling trajectories,
  one bounce, full3-axis rest, cooling/alpha/expiry, skipped-frame equivalence,
  pause/reading/hitstop/clear/reuse/unload and module/preset restoration.
- Normal QA073121/mirror073528 build and repeated-builder PASS. All prefab/meta
  bytes, object IDs and ordered GUID references are unchanged. Normal player
  input8621b782 differs from Quick only by the final test ground fixture line;
  mirror57872d58 adds only the existing editor actor-mirror fixture.
- Edge155.0.4283.62/NVIDIA GTX1650Ti/ANGLE D3D11: normal3318/mirror4034checks PASS,
  ten real-keyboard parries per640x360/1280x720/direction (40total), normal/Slayer;
  hit particle exclusion remains. B<->compact pause/restart normal1062/mirror1139
  PASS; additional report checks prove restart clears roots and restores Current.
- Native tail normal117 PASS; mirror1280 completed four age samples in a failed57
  check run before a missed640parry. Only640was retried,48checks PASS, unchanged
  gameplay/timing. Failed full profile remains failed. Qualified ages.55/.75/.95/1.1
  show floor center >=-2.8713007 (scene groundY-2.9025507 +1/32). Motion grids/
  contact/tail PNGs reviewed at both sizes/directions: rays become pixels, fall,
  low bounce/rest/fade; sword contact remains readable. No blur/pink/observed
  ground penetration. These are sampled visuals, not human art approval.
  All native art/lifecycle/tail reports have0JS/managed exceptions/provider reads/writes;
  fixture interception/external blocking and observation-only bridge retained.
- Two final score30-parry-aligned before/after12particle MP4s, both directions,
  1x plus0.25x nearest-neighbor full-flight closeups;54.4s/25fps/1360frames each.
  Render sizes640x360/1280x720, side-by-side output1280x360/2560x720. Full decode
  and local/server SHA256 match; labels/aspect/contact frames inspected. RNG is
  not locked and25fps compressed video cannot rule out sub-frame artifacts.
  640 SHA2569baafa820f6687ff3215b5765e28414f60d48eb34b18a8b979f758e334d89ec7;
  1280 SHA256ebdd51988b556fe937e8c1d3d38b5f7dc04a22a5515791e9ad157992eb2686b0.
  Retained under E:/CodexTemp/TypingHero/parry-clash-lighting; earlier clips preserved.
- Matched no-video/no-screenshot performance is complete. A=previous12particle tail,
  B=continuous8falling sparks. Four keyboard clashes/size/run, visible/focused tab,
  same observer/machine. Normal uses allocation profiling OFF on both players
  (every sample allocatedBytes=-1); mirror uses profiling ON on both players.
  Initial normal profiling-mismatched diagnostics are excluded from conclusions.
  Final normal QA080156 uses final Quick input28e395da with --no-profile; build and
  repeated-builder PASS. Fresh ordered normal ABBA checks1275/1388/1428/1259 PASS;
  mirror A/B checks977/1307 PASS. Nearest-rank sampled frame p95ms:

  | Side/size | A1/B1/B2/A2 p95ms | Samples | Mean A ->B | Delta |
  |---|---|---|---|---|
  | normal1280 |14.050/14.175/13.925/15.520|101/101/102/99|14.785 ->14.050|-4.97%|
  | normal640 |14.385/14.500/13.670/13.785|102/101/102/102|14.085 ->14.085|0.00%|
  | mirror1280 |15.145/14.135 (AB only)|100/100|15.145 ->14.135|-6.67%|
  | mirror640 |19.250/14.200 (AB only)|96/100|19.250 ->14.200|-26.23%|

  Normal means are means of the two per-run p95s, not a pooled percentile. Live
  samples/row:normal34-45,mirror33-42. Matched normal live-only mean p95 changes
  15.1625->13.7175ms at1280 and14.3975->14.2275ms at640; mirror15.955->14.795 /
  16.265->13.730ms. No matched increase exceeds10%. Short sampled QA deltas include
  observer/idle/background overhead; they do not establish GPU cause, no spikes,
  whole-device FPS or a fix for the earlier mirror+31.16%/+20.53%/+15.95% concerns.
- Updated Edge QA was opened at1280x720, paused BCompact, score0/40-850HP;
  ready PNG inspected. The viewer closed normally at08:07UTC before the matched
  normal ABBA (08:18-08:20UTC). No8139listener remains; players and open_compact.ps1
  are retained for reopening. Current/default/camera/gameplay/itch unchanged. No merge/push/deploy.

## BCompact reverse sparks (2026-10-19, verified 16-particle review revision)

- Approved16/clash,128over8leases supersedes12/96: groups2/2/6/6/0; original10moving
  sparks plus4reverse-X falling sparks, two near/two far. Fourteen fixed states,
  six-particle scratch;12fall/one-bounce/rest180ms paths plus2fast front fragments.
  No shader/prefab/public API/default/camera/gameplay/itch changes.
- Quick20261019T084732:78/78 PASS,0failed/skipped. NormalQA20261019T083644 build/
  repeated-builder PASS; both inputc8d34d84840756d629276ee8e012b43c056e0908dbbf40daa1461d5f498f6c03.
  MirrorQA20261019T084910 PASS, input7eea5996085debd8da4f7249e75ff61d1aed962bdb9da777cc03ed447012b1ed
  adds only disposable editor actor mirror fixture. All13serialized parry assets
  retain identical bytes/IDs/ordered GUID references;614local prefab/scene/meta bytes unchanged.
- Edge155.0.4283.62/Playwright1.58.2/GTX1650Ti ANGLE D3D11: normal6274/mirror retry5608
  checks PASS,40keyboard parries (10per640x360/1280x720/direction), normal/Slayer hit
  exclusion. First mirror58check run missed keyboard timing after1parry; retained FAIL.
  B<->compact pause/restart normal1274/mirror1179 and tail135/146 PASS. Postchecks
  confirm restart clears roots/restores Current, sampled centerY>=-2.8713007 at
  ages.55/.75/.95/1.1. Motion grids at both sizes/directions show both branches,
  pixel shrink/fall/cooling and readable sword contact, no observed ground penetration,
  blur or pink. Quick covers12single bounces, X/Y/Z rest/fade, spawn/mirror and128cap.
  All native reports0exceptions/provider reads/writes; external blocking and read-only
  bridge preserved. Sampled visuals are not every RNG outcome, sub-frame proof or human approval.
- Matched12->16 no-video/no-screenshot p95, focused visible tab,4parries/size/run,
  profiling ON both players, actual counters>=0. Legacy baseline marker predates
  profile flag; exact prior volume input hashes checked instead. Normal AB1420/1386
  PASS. Initial mirror AB1377/1365 PASS:640live p95+13.40% triggered fresh ABBA,
  checks1392/1393/1422/1497 PASS. ABBA means below average per-run p95s, not pooled p95.

| Direction/size | All-state p95 A -> B (ms) | Change | Live p95 A -> B (ms) | Change |
|---|---|---|---|---|
|normal1280 AB|13.5750 -> 13.5600|-0.11%|14.3550 -> 13.9200|-3.03%|
|normal640 AB|13.7600 -> 14.0600|+2.18%|13.6400 -> 13.7100|+0.51%|
|mirror1280 ABBA|13.6925 -> 14.5950|+6.59%|13.6700 -> 15.0050|+9.77%|
|mirror640 ABBA|14.2050 -> 13.7450|-3.24%|14.6125 -> 13.6800|-6.38%|

- Normal all/live n1280=101/99 and41/43;640=100/100 and44/40. Mirror ABBA
  A1/B1/B2/A2 all1280 p95=13.685/15.380/13.810/13.700ms,n100/102/100/103;
 640=14.780/13.755/13.735/13.630ms,n100/100/101/101; live n1280=43/41/42/43,
 640=40/43/43/44. Repeat means below10%; live1280+9.77% is close to threshold.
  Short sampled deltas include observer/idle overhead, not GPU cause/device FPS/
  no-spike/equivalence evidence. Prior mirror+31.16%/+20.53%/+15.95% concerns remain unresolved.
- Two new12->16 clips, both directions,1x and.25x nearest-neighbor closeups;
  54.4s/25fps/1360frames each. Full decode and local/server SHA256 PASS; labels,
  aspect/contact inspected. First score30parry aligns onset; RNG/animation phase not locked.
  `ParrySpark-BCompact-Reverse-BeforeAfter-640.mp4`: 4682308bytes, SHA25654f4b35918a7b9a66f0fd47f4710bed47035653371547e383bdccb11d45d1719.
  `ParrySpark-BCompact-Reverse-BeforeAfter-1280.mp4`: 11606552bytes, SHA256c21e984af3b67d27560d2d09cb5afe17890a8c93d9ce521ff6767dff1df1a0a6.
- Review resources retained at E:/CodexTemp/TypingHero/parry-clash-lighting:
  reverse-normal/mirror-player, raw captures, qualified/failed reports, source backup,
  helpers and all old/new clips. Completed reverse QA profiles15 and duplicate local
  archives3 removed (18targets verified); no persistent Edge viewer retained.
  Keep child feat/pixel-hit-sparks until review and verified integration; no commit/push/merge/deploy.

- Final cleanup also removed12duplicate remote transfer/segment/concat files; final
  clips/raw videos/players remain. Code hashes match qualified Quick/QA snapshot,
 614asset bytes and unrelated UTF-8 document prefixes verified unchanged; scoped
  whitespace checks PASS. Task QA listener8137 closed. Review reopen command:
  `powershell -NoProfile -File E:/CodexTemp/TypingHero/parry-clash-lighting/open_compact.ps1 -Name reverse-interactive -Player E:/CodexTemp/TypingHero/parry-clash-lighting/reverse-normal-player`.


### BCompact24 Quick receipt (2026-10-19)

- Unity2022.3.25f1 on fly-server: Quick20261019T093602 passes79/79 actual tests.
  Immutable input fe4c72f5d87ef91a8fa511865f57fd95d778caba5b562745ae87598ed282ce23.
 24/192budget/groups, same-frame launch/mirror X/preserved Y/Z,20single bounces,
  full-axis rest/fade/expiry, pause/reading/hitstop/reuse/module restoration pass.
  New24seed/two-direction1unit-high fixture passes300ms >=8/12reverse particles
  beyond2.5Xunits and>=6streaks at least2pixels. Repeated builder/tuning test passes.
- Local623serialized/meta files match HEAD content after Git newline normalization;
  SHA256 inventory retained in strong24-asset-identity.json. Native QA/build/clip/
  performance acceptance remains pending; do not infer visual PASS from Quick.


### BCompact24 normal QA, performance and release steering (2026-10-19)

- Normal immutable QA20261019T093707 build/repeated-builder PASS, same fe4c72f5 input
  as Quick79. Edge155/Playwright1.58.2/GTX1650Ti ANGLE D3D11: art3336checks,
 20keyboard parries (10per640x360/1280x720), normal/Slayer; lifecycle967 and tail118
  checks PASS. No exceptions/provider reads/writes; fixture/external blocking kept.
  Recorded motion grids show reverse sparks beyond the player's rear silhouette,
  readable contact and no observed floor penetration; sampled floor>=-2.8713007.
- First71check art attempt failed after missed real-keyboard parry. QA-only harness
  now holds Space until observed parryPose/window, rather than only a newer frame;
  product timing unchanged. Failed attempt retained as failed.
- Matched no-capture profiling-ON16->24 normal AB checks1936/2162 PASS;1280 live
  p9517.305->19.670ms (+13.67%) triggered freshABBA. Four ordered runs pass
  checks2078/1955/2209/2029, four keyboard clashes/size/run, focused visible tab.
  All-state p95 A1/B1/B2/A2:1280=14.450/14.845/13.750/14.375ms;
 640=14.175/16.845/13.650/16.410ms. Means A->B14.4125->14.2975 (-0.80%) /
 15.2925->15.2475 (-0.29%). Live means14.2475->15.0825 (+5.86%) /
 15.185->14.720 (-3.06%). All sample counts99..103;live40..43 per run/size.
  Means average per-run p95s, not pooled percentiles. No repeated increase>10%;
  short sampled data does not establish GPU cause/equivalence/device FPS or resolve
  previous mirror+31.16%/+20.53%/+15.95% concerns.
- User explicitly canceled further mirrored QA and requested Git push plus itch.io
  publication. Mirror build094745/repeated-builder already PASS (input8eee591e),
  but running native mirror art/process tree was stopped; do not claim mirrorQA or
  mirrorperformance PASS. Offline24seed/two-direction Quick evidence remains valid.
  Clips now cover normal only at both sizes,1x/.25x full viewport; previous clips kept.
- Remote main advanced tobf98c27 with approved HUDR7/Endless/full-release work while
  this task ran. Committed spark changes e018961, merged latest origin/main cleanly
  into feature child before release to preserve those features. Parent WIP untouched.
  Integrated Quick and production build are queued; upload not yet performed.
  Current remains the serialized default, as requested by original plan.


### BCompact24 published ? 2026-10-19

- User canceled mirrored native QA/performance and authorized push plus itch.io.
  Integrated Quick20261019T095939:117/117 PASS,0failed/skipped. Production100117
  PASS on the same63e6532d5a2c69a00e43af0e4544943bd3c34731b06681d2559baafb703f0c43
  input; qa=false, all JS/WASM/data exclude bridge markers. Original-HTML native
  smoke3checks PASS,0exceptions/requests/writes; menu/Fight/pause/resume screenshots
  inspected. This scoped production smoke does not replace prior full-mode QA.
- Published https://micharel09.itch.io/clackblade, game3857509, new playable19872290.
  Saved previous19871730 hidden; other uploads/settings/gallery/pricing unchanged
  (settings SHA25602f423e4de577aa9926d0f7fa8e4757d228a6cb6109264a2b9593f477a4a4a90).
  Public Run game renders CLACKBLADE;29non-HTML CDN files match the ZIP exactly,
  index.html contains original HTML plus expected itch wrapper. No Firebase change.
- ZIP CLACKBLADE-WebGL-BCompact24-20261019.zip:30members/16,595,759bytes, CRC PASS,
  SHA256e50893f0081ac5715f1215c8a76d27938c0b4276ff80c9a3ed09f96a4512b0d1.
  Canonical receipt marketing/itch-io/clackblade-release-20261019.json; ignored ZIP
  under Builds/. Source pushed to origin/feat/pixel-hit-sparks at93f8eb7 before upload;
  final receipt/doc commit follows. No push to main; parent checkout/WIP preserved.
- Review clips normal only (mirrored continuation canceled):640/1280,27.2s each,
 16->24 at1x/.25x full viewport. Full decode/hash/labels/aspect checked; RNG/contact
  animation phase are not locked.640 SHA2565ff54e05fec76103dad202934cdfe5a7d2e591ec7933ce867f9af480fd6fb100;
 1280 SHA25670df40579895bf543eef8ac265c00bb75725de01d8d3842f15584a64f87486a9.
  Old clips retained. New files ParrySpark-BCompact-Strong24-BeforeAfter-640.mp4 and
  -1280.mp4 under E:/CodexTemp/TypingHero/parry-clash-lighting.
- Keep child feat/pixel-hit-sparks, strong24-normal/mirror/production-player, clips,
  qualified/failed reports and immutable source inputs for requested review and
  reproduction.14completed task Edge profiles already removed; production-smoke
  profiles are cleaned at final handoff. Mirror native process tree stopped on request.
  Earlier mirror performance concerns remain separate/unresolved. Current stays default.
  Feature is published from its pushed branch, not integrated into main; retain child
  until requested review and verified integration. Authenticated browser session is
  stopped after public verification. No devlog/social posts or unrelated changes.

## Unified score cluster acceptance (2026-10-19; local only)

- Unity2022.3.25f1/fly-server: serialize20261019T151210 strict repeat PASS on input787a50f32b27808fb84641307aaa8ff32b4d8d8f30676d938bff996a120a6853. QA20261019T151409 compiles/builds/repeats PASS on input`62da79d2242a0ad8691df7190c96ae2d271ee973c9171eedfa3ea7aa9222adf9`. Imported Unity's SampleScene serialization;853/853 IDs, scripts and persistent event blocks retained. All art, ScoreSystem, ScoreRules and typing core bytes unchanged.
- Final Edge155.0.4283.62/Playwright1.58.2, NVIDIA GTX1650Ti ANGLE D3D11: **209/209 PASS in66.194s** on that exact QA player with the final worktree score-cluster driver. Actual keyboard/mouse only; bridge observes binding/visibility/geometry/value. Normal/Slayer states at320×180,640×360,1280×720,400×300; fifth-word activation, hidden×1, live fill, unchanged total position/pulse, pause/resume, expiry, timeout, normal/Slayer restart, actual parry renewal, natural Slayer exit and subsequent normal fill. Native live captures inspected at all four sizes; gold Slayer and restored normal fill are clear. Zero managed/JS exceptions; fixture/provider reads0/writes0, interception/external blocking retained. Battle uses VeryEasy pacing for multi-size live capture; earlier Normal activation also observed. No performance/balance claim.
- Software profile FAILED43checks at tutorial parry timeout; do not label it passed. Earlier native pilot runs also failed on driver cue timing, rapid pause/zoom transitions, encounter completion or assumed stun/timeout lifecycle. Corrected driver observes actual Endless contact, fully completed pause transitions and natural mode exit; no gameplay changes or relaxed product acceptance. Final native PASS above supersedes those incomplete pilots.
- `python tools/game_loop/run.py webgl --browser-scenario score-cluster` selects the focused profile and preserves an immutable QA player; native execution uses the existing browser.cjs fixture harness with `TYPINGHERO_NATIVE=1`, `TYPINGHERO_SCORE_CLUSTER_ONLY=1`, explicit build/runner/results paths. Runtime C#/scene did not change after that build; subsequent edits were the browser driver only. No full Quick/balance/stress/soak or production/release run. [Checkpoint](../plans/score-hud-cluster.md).


## Combat HUD clarity and feedback — local implementation (2026-10-20)

- QA022355 Unity2022.3.25f1/fly-server compile/build/strict repeat PASS, input fb80c4104397ce860097599777f60ba6c81020895d67a5da802c7782546a250a. Scene/CombatR7 bằng checkout,853 old IDs/scripts/listeners giữ nguyên,858 final. Hai PNG10×10 khớp v2, Point/Clamp/no-mips/border0; iframe80ms không đổi.
- Scoped HUDFeedback4/4 PASS022316; gate/cue/window/Tutorial assertions4/4 PASS022957, input a4282a59ec4fe432ba8d2e20277b1448de23595ca23bc9b31627f17417bbfc7a. Kiểm damage/restore, HP11/10/9, pause, setup-zero vs cost+stun, lần break thứ hai, icon recovery và lifecycle. Không full Quick.
- Native Edge155.0.4283.62/GTX1650Ti ANGLE D3D11/Playwright1.58.2:2353/2353 PASS,169.701s. Năm difficulty, Space thật, finite Normal6duels giữ10HP tới victory(2961points/8parries), Continue/reset, restart/death. Bốn kích thước320×180,640×360,1280×720,400×300 đã xem normal/Slayer và các reduced-only case lúc đó.0exceptions,6fixture reads/2intercepted writes,0production writes; external blocking bật.
- Software score-cluster dừng53checks tại tutorial parry retries, không tính PASS. Các pilot snapshot/tween và lỗi timer frame đầu đã ghi trong checkpoint; timer fix có4/4 regression PASS. HUD mode không yêu cầu balance recovery-sample gate; chế độ Endless/balance gốc vẫn giữ assert. Không balance matrix/stress/soak/performance claim.
- Sau QA user bỏ reduced-motion riêng: chỉ xóa hai conditional BrowserMotion.Reduced, giữ nguyên normal Lerp/fade đã kiểm. Lượt Unity bổ sung hủy khi còn chờ lock; không tuyên bố build lại sau thao tác xóa. Native receipt dùng driver26f85e88d086c28b03475550465769f46dff316da5fd814513c7fee983bca98f; driver mới bỏ reduced-only cases21fca2980d324c446c965043ef82c3914df1c4f125a26b0eb984fa0fafccb995 syntax PASS.
- Native selector: TYPINGHERO_NATIVE=1/TYPINGHERO_HUD_FEEDBACK_ONLY=1 với immutable build/runner/results roots. [Mapping, commands và cleanup](../plans/score-hud-cluster.md). Kết quả publication ghi riêng khi hoàn tất release được user yêu cầu sau đó.

## HUD production publication (2026-10-20)

Published https://micharel09.itch.io/clackblade, game3857509, upload19892576. Source6e626f5 on micharel09/hud-itch-release preserves approved public palette/UI input64de5b3a plus score/HUD changes; parent dirty WIP is untouched. No new reduced-motion handling.

- Unity2022.3.25f1/fly-server strict repeat serialize20261020T030558 PASS; imported only MainMenu/SampleScene builder output. Production20261020T031135 PASS, input1f2550f0a8a0d4f226417dc926801c0eba4983146500a238b106ded8fb43134c. QA bridge excluded, scanned ASCII and UTF16 payloads. ZIP CRC PASS; 16,491,939 bytes, SHA2567333b9de781abb84606769dcee8580c6c453452962005b22ce045bd29449f679.
- Native Edge155/D3D11 using original production HTML: tutorial, Endless intro/combat, pause/resume, four real Space inputs, successful parries, gold cue/teal success and Slayer warning icon visually inspected. Two smoke assertions PASS; no exceptions or provider writes. Initial local menu capture caught Unity splash; actual public menu subsequently inspected after waiting8s. Guard-break notice text was not captured in this production smoke; previous detailed scoped HUD QA covers it. No full balance/stress or repeated multi-size matrix.
- Authenticated browser-skill upload/Save and editor reload: new playable19892576, previous19891356 hidden and retained for rollback.31 non-upload business controls and15 unrelated uploads unchanged. Anonymous CDN29/29 non-HTML SHA256s match; index.html differs only by standard static.itch.io/htmlgame.js injection. Anonymous native Edge public menu renders, QA bridge undefined,0 exceptions/provider writes. Platform itch.io/html-callback POST was blocked; first smoke predicate wrongly counted this telemetry as a provider write, then corrected the predicate and reran PASS.
- Release manifest: [hud-release-20261020.json](../../marketing/itch-io/hud-release-20261020.json). Retained product ZIP in release child Builds/CLACKBLADE-WebGL-HUD-20261020.zip; canonical ZIP/source-input.tar.gz/receipt under fly-server /data/typinghero-game-loop/releases/hud-feedback-20261020/. Rollback reselects19891356, hides19892576 and saves.

- Cleanup complete: task captures/profiles/helpers removed, browser session qegv stopped and owned server results20261020T030558-serialize/20261020T031135-production removed after canonical artifact verification. Shared cache/builds/other jobs preserved. Keep both task worktrees for source review/integration into main; remove only after integration. No Git push/merge performed.

## Remove GUARD BREAK text (2026-10-20, local follow-up)

User removed the redundant text because Slayer already communicates guard break. Label node/IDs/binding remain, but text is empty and disabled; announcement timer/first-break flag are removed. Shield/warning and combat rules remain unchanged.

- Unity2022.3.25f1/fly-server result20261020T034519-quick, inputc156dca188c61db60603c42f539c95033b64852683f066f3e6f3bb28a326cb24: strict repeated SerializeOnly PASS; generated SampleScene equals checkout, all scene IDs retained. Scoped HUDFeedback PlayMode4/4 PASS (actual damage/parry/pause/first and repeated real break/icon recovery/disable lifecycle, plus three existing asset/fill checks). Existing browser driver syntax, source/docs whitespace and relative links PASS. The old QA timer reflection was removed alongside the timer.
- No geometry/font/combat changes and no new WebGL/browser matrix or itch.io publication in this follow-up. Previously published upload19892576 remains unchanged; branch holds this local follow-up for review/integration.
- Cleanup complete: removed owned result20261020T034519-quick and task helpers/XML/scene copies under E:/CodexTemp/TypingHero/remove-guard-break. Shared runner/cache and unrelated jobs preserved; retained existing child because changes are not integrated into main.

### Physical parry default correction published (2026-10-19)

- Root cause: prior default Current remained selected in production; earlier compact
  tests used F6, and the first production/public smoke only proved menu/Fight/pause.
  User's report was valid. Latest separate full release19872500 also used Current.
  Preserved its main933dbbd fixes; owner default, serialized Player prefab and
  CombatFeedbackBuild now all select BCompact4 without F6. Current stays explicit.
- Focused Quick123645:14/14 PASS,0skips; QA123737 build/repeated-builder and native
  default/r2:16checks PASS. Fresh/restarted game emits BCompact without selection;
  20particles remain at300/550/800ms,12at1050ms, minY reaches-2.8713007 floor.
  Initial10check native attempt failed on stale post-reload probe data, retained FAIL;
  corrected observer waits3fresh frames/empty roots. No gameplay timing changes.
- Production124717 PASS on the same8ba1dda11a10270a96b8f096c30df977ee6f4e7c085b0c1340196a52216400a2
  input. qa=false/all JS/WASM/data scanned. Real-keyboard production parry smoke5
  checks PASS,0JS exceptions/writes,1successful parry,0F6. Launch/fall458ms/
  bounce704ms/rest966ms images visually verified. Current-baseline parry lacks tail.
- Published playable19874589 at https://micharel09.itch.io/clackblade; old19872500
  retained hidden. Public page points to that exact iframe;29non-HTML CDN hashes
  match ZIP and HTML adds only expected itch wrapper. Same public CDN actual game
  independently passes5checks/1real keyboard parry/0F6/0writes/0JS exceptions;
  inspected launch,fall457ms,bounce703ms,rest964ms images show physical particles.
  This directly verifies default public combat, not just a menu or QA-only preset.
- ZIP CLACKBLADE-Physics24-20261019.zip:30members/16,596,608bytes,CRC PASS,SHA256
 541e806b5557e7091198b1d269f5104149f1135bf65d254c06a5cb545c907e1c.
  Receipt marketing/itch-io/full-release-20261019.json, brand receipt reconciled;
  ignored ZIP in Builds/. Storefront settings hashcb998ee631f065a1c43833b8836b490e6300ecc4ebfc0a8e304f3330bd9bab27
  unchanged. No Firebase/schema/other gameplay changes; mirrored QA not requested.
- Correction e79c185 pushed atomically to origin/main and feat/pixel-hit-sparks;
  receipt commit follows. Parent local checkout/WIP untouched. Keep requested child,
  compact-default-qa/production players, screenshots/reports, source input and all
  earlier review clips under E:/CodexTemp/TypingHero/parry-clash-lighting; temporary
  Edge caches cleaned after receipt. Browser session stopped after public check.
  Prior mirror performance concerns remain unresolved; this correction changes routing,
  not the already-reviewed24particle trajectory/appearance. No new broad performance claim.


## BCompact flat contact flash (2026-10-19, historical technical QA; visual rejected)

- Final focused Quick13595918/18PASS,0skip. QA140934 build/repeat PASS; both input c594476614434566fda7309073d7b78ce88a550c99f0a3b5923be10dd6229db3. Core60/200ms, ring90/260ms, pulse70/175/280ms, strongest mixed overlap, pause/disable/reuse, independent contact clock, legacy140ms, material/modules and24/192 budgets covered. Local source commit725b5ad. Earlier17test snapshot is superseded.
- Edge155/Playwright1.58.2/Intel UHD0x9BC4 ANGLE D3D11 final-after26PASS,6real default noF6 parries at640/1280, Slayer/restart,0exceptions/provider reads/writes. Native25fps frames and comparison clips inspected: readable white-gold point/stepped orange ring, local fading light, intact face/sword/silhouette and falling sparks. Sampling40ms is not precise endpoint evidence. No mirror or human-art approval claim.
- Matched no-capture A2/B2 each20PASS,3parries/size, same visible/focused tab and allocation profiling.1280 all/live p95=33.500->31.940ms(-4.657%,n57/60)/33.425->31.640(-5.340%,n26/25);640 all/live33.340->33.595(+.765%,n57/57)/33.340->33.170(-.510%,n25/26). No10% threshold crossing; ABBA unnecessary. Sampled observations are not GPU/FPS/equivalence proof or an asserted speedup; prior mirror concerns remain unresolved.
- Final15.2s1x/.25x comparison clips decode/hash PASS; old physics clips retained. Full receipts, corrected observer/export attempts and pending production/public acceptance in combat-feedback checkpoint. Parent Ronin9sprites/8normals are excluded (separate task, no publication requested there).

## BCompact voxel preview (2026-10-19, visual review pending)

- Focused ParryClashTests Quick20261019T152205:19/19PASS. QA WebGL20261019T152304 build PASS; source input8470def229e035afaf4bd833c139d3c103d4c32689a8909f921e5b611f7f1da8. Mesh bounds/normals, three-axis spin, floor/rest rotation, legacy renderer/streams/modules reuse, pause, 24/192 counts and existing trajectory lifecycle covered. LaunchPath/FlightPath are byte-identical to prior source.
- Edge155.0.4283.62 preview12checks PASS: one real keyboard parry each1280x720/640x360, BCompact default/noF6, pause,0exceptions/fixture reads/writes. Impact/flight images inspected at both sizes: closed voxel forms have distinct lit/dark faces, no expanding ring; normal live control remains available. This is a focused preview, not human art approval, deep QA or a p95 measurement.
- Copied only generated CompactVoxel mesh/material+metas and ParryClash prefab from this build. Prefab diff verified to contain exactly two new asset references plus voxelVersion1; all prior IDs/GUID refs/tuning are unchanged. YAML trailing whitespace normalized without behavior changes. No itch publication; keep older physics clips. Temporary preview player/helper/captures retained under E:/CodexTemp/TypingHero/parry-clash-lighting while visual review continues.


## Normal strike accents (2026-10-21, verified local preview)

- Focused Unity `20261021T060457-quick`:9/9 PASS,0failed/skipped,3.2342438s;
  source `e95e1fc99ac76deadc972d4312af82761734b556a0db647880f2a1f340c6823d`.
  Both dust directions/floor/180ms, core position/4pixels/40ms and route selection,
  pause/reading/reuse/disable/Slayer clear, independent caps, twice-run builder,
  one opening listener, and preserved outgoing/parry/removed received route.
- Same-source main WebGL `20261021T060548-webgl` build/repeat PASS. Native Edge155,
  GTX1650Ti ANGLE D3D11 at1280x720:18checks main plus10checks corrected miss fixture,
  zero JS/managed exceptions/provider reads/writes. Miss build `20261021T063059-webgl`
  moves only the QA enemy +10world-X before building; no fixture in the checkout.
- Three matched-input samples per side: miss, normal hit and two hits. Inspected
  all388selected render frames/382consecutive transitions, including held frames.
  Silent1x/.25x exports preserve timestamps; full decode/count/PTS and playback
  checked. Native cadence and tutorial holds are not exact lifetime measurements;
  focused tests establish the180/40ms age limits. Second paired hit ends with
  enemy counter-hit/tutorial reset; natural expiry is established separately.
- Preview/local commit only; no new performance, full-profile, production or
  publication claim. Detailed receipts, rejected fixture/harness attempts,
  per-frame evidence paths and resource retention: [combat checkpoint](../plans/combat-feedback.md).


## Large golden normal impact (2026-10-21, verified local preview)

Scoped Unity2022.3.25f1 run20261021T085852-quick:12/12PASS,0fail/skip.
Clean WebGL20261021T092100 passes build and builder twice; same runtime source.
`large-impact-final-native`:18keyboard checks PASS at1280x720,0exceptions/provider
reads/writes. Normal110frames109pairs and two-hit178frames177pairs all directly
reviewed; four1x/0.25x clips play to ended. Miss10checks/95frames94pairs reused
because miss-route inputs are unchanged.272protected sprite/clip/controller/audio/
attack-hitstop files match5bf8629. Reference research, failed incremental startup,
clean-build adapter, source provenance, per-frame CSVs and preview are recorded in
`docs/plans/combat-feedback.md`. Local-only handoff, without push/deploy.


## Pimen normal hit (2026-10-21, verified local preview)

- Scoped Unity20261021T101930-quick:11/11PASS,0fail/skip,4.5620254s. Both mirrors,
  seven equal160/7ms cels, fixed pivot/contact/48world-pixel width, original tint,
 160ms release/reuse/pause/reading/Slayer/disable,8impact/8dust caps, miss opening,
  repeated builder and preserved other routes. Initial fake-null builder failure
  was fixed and rerun; failed intermediate players are excluded.
- Clean WebGL100352 build/strict repeat PASS. Same runtime source as final tests;
  clean-build adapter only, generated normal prefab/Player/scene match checkout
  ignoring YAML whitespace. Native22keyboard checks PASS at1280x720,0exceptions/
  provider reads/writes/external requests. Prior10-check out-of-range miss evidence
  reused because miss contact routing is unchanged; current miss opening test passes.
- Direct14source-direction frames/12pairs and264native frames/262pairs reviewed.
  Four1x/.25x exports retain all capture timestamps; decode/count/PTS and playback
  to ended pass. Source F6 is transparent: art ends137.142857ms, lease ends160ms.
  Native scenario faces right; left output has focused Unity and full mirrored
  source-image evidence. No exact endpoint claim from10Hz probe or performance claim.
- Original Pimen PNG/importer and288protected inputs unchanged. Preview8159,
  comparison videos and per-frame evidence/retention are in the existing
  [combat checkpoint](../plans/combat-feedback.md). Local only, no push/deploy.


## Pimen and current HUD integration (2026-10-21)

Merge aa14b24 combines99ac6da with main a83bd90. Focused Unity110107 passes11/11;
clean WebGL110357 passes build and strict twice-run builder. Native1280x720
integration-native-r2 passes22/22 real-input hit/pause/restart/Slayer checks,
zero exceptions/provider reads/writes/external requests. Peak Pimen and Slayer
images inspected: HP/score/tray remain readable. Original animation review is
reused because VFX/art are unchanged. Imported main builder's existing resource
frame border21/0/3/0. CRLF-only failed build and stale-restart pilot are excluded;
see [combat checkpoint](../plans/combat-feedback.md). No itch.io deployment.


## Menu name/Escape/focus follow-up - 2026-10-22

Filtered PlayerIdentityTests15/15 PASS; QA WebGL015918 and strict repeat-builder
PASS, same input2eb55c66bee8884f055007d0587ba78104567ef9b394179c8be163bb36c1724a.
Native Edge155/D3D11:279 focused assertions plus17 legacy16 fixture assertions
PASS at1280x720/640x360; real input, read-only QA bridge, zero exceptions/provider
writes. Name10 glyph bounds and actual name/error/legacy images inspected.
Three fixes verified: shared3-10 UI limits, Back/Escape drafts with Start-only
persistence, bounded main-menu selection restoration after background clicks.
WebGL OS clipboard paste remains unsupported in both original and patched players;
paste11/16 acceptance is not passed. No six-lesson/full Quick/gameplay/stress/soak
or itch deployment. Details and scope decision: [checkpoint](../plans/dungeon-main-menu.md).


## Attacker-facing parry and reusable floor particles (2026-10-21)

- Orca task micharel09/parry-physical-particles from origin/main edfc3b6; parent
  dirty checkout preserved. Canonical input8dfc8fc887f21b993b5cbbfaa8aa81841c123391479b0a1373a5208eb2a7d360,
  Unity2022.3.25f1 run20261021T145852-webgl: scoped37/37 PASS,0failed/skipped,
  13.2257312s; QA build/strict repeated builder PASS on the same snapshot.
  ParryClash.prefab and ParryPhysics.asset match generated Unity bytes after LF
  normalization. External runner adapter selects the existing test filter and LF
  normalization; repository runner/toolchain/packages unchanged.
- Mirror run20261021T150937-webgl builds/repeats PASS, inputa7b17ae6293b1e452d8de7a41a78837442b3e28d8ac2563744727879e0c978ed.
  Every canonical build input matches; only disposable editor fixture mirrors
  actor positions/facing. No fixture in committed product source.
- Native Edge155.0.4283.62/Playwright1.58.2/Intel UHD ANGLE D3D11,1280x720:
  normal-battle-r3 and mirror-battle each15/15 PASS. Real Space, attacker-directed
  flight,24cap, complete expiry, unchanged HP, pause and second-parry cleanup.
  Zero JS/managed exceptions, provider reads/writes or external requests.
  Additional normal-native-r2 tutorial15checks PASS; tutorial overlay occluded
  the upper fan, so final visual acceptance uses the unobstructed battle captures.
- Directly inspected every frame/consecutive transition: right55/55 and54/54,
  left55/55 and54/54, including held frames, birth, spread, fall and dark floor
  tail. No observed long reverse rays, visible floor penetration or residue.
  Readback/probe timing is limited to captured frames; no every-engine-frame,
  performance/FPS, new balance/stress/soak or publication claim.
- Startup missing snapshot, missed second cue from fixed2.2s driver wait, pending
  encounter intro, and a too-narrow10Hz observation window were harness pilots;
  their overall runs are not PASS. Only harness timing/setup changed; same game
  source/build reused. Exact sources, per-frame evidence, retained clips and
  resource cleanup are in [checkpoint](../plans/parry-physical-particles.md).

- Integration after ee5bd8f compact menus/pause: run20261021T154930-quick,
  input66299c77545a38c5508e4f14765b7c207d32e7450fd6459acc1ba77e12986566,
  five focused cases PASS/0skip in4.4420984s, including actual TogglePause/Resume,
  particle/pulse pause/reuse, attacker context and profile binding. Subsequent
  1611145 dungeon MainMenu merge leaves all37 relevant runtime/config/test inputs
  identical; scoped results reused. No integrated menu redesign or publication
  claim by this task. Final resource/provenance record is in the physics checkpoint.


Menu follow-up integrated as dedfe16 on origin/main after preserving incoming
particle physics changes. Integrated filtered identity021754:15/15 PASS,
input3f11a598c427567a85ec2f0dc40a023f8689bee9d4a8f4e6aed52b1cf83138a6.
Menu source/config unchanged; existing native evidence reused. Task captures/
profiles cleaned; final preview8246/player retained for review, tutorial WIP intact.
Clipboard acceptance remains pending as recorded above.


Parry/counter synchronization follow-up (2026-10-22): scoped CombatFeedback,
NormalStrikeEffects, PhysicalParticle, ParryClash and ParryClashPixel tests82/82,
normal062419-webgl inputc5d5e327; mirrored063127-webgl inputd7f66218 build/repeat
PASS. Native real-input normal/parry/full-hidden both directions PASS;474 frames
and468 consecutive pairs directly inspected, plus42 source cels/36 pairs.
Functional Slayer/lethal28checks PASS, final playerHP40/enemyHP0. Six fresh clips
retain actual destination audio and render holds. Browser audio-bus timing is
measured separately from display submission and does not measure device latency.
Exact source, timestamps, failed cold-decode/link diagnostics, per-frame records
and integration/cleanup are in [combat checkpoint](../plans/combat-feedback.md).


## Pause modal isolation and Resume transition (2026-10-22)

- Scoped Unity20261022T074625-quick:5/5 PASS,0fail/skip,0.8417408s,
  inputd953da04972cf86830623fef125b5549f578afc894f92363a4d1d663db892dc0.
  PanelReveal panel deactivation, audio parent/geometry/active state, focus and
  closing-frame shortcuts; 0.2s camera exit, compact-until-hidden geometry,
  rejected repeated Resume/Restart/Menu, hidden selection and Victory layout;
  existing modal/selection/tutorial reading regressions.
- Integrated Unity20261022T080125-quick after latest parry upstream merge:
  5/5 PASS,0fail/skip,0.8485045s, input36fffcf31b5f40a0c57c8e43a7e43eed594c3358000d6aa83444f1a9a7600298.
  Existing Unity testFilter selected by external task adapter; repo runner unchanged.
- QA20261022T075105-webgl inputc320c91534c08c8ce08689e40a60a46a1568a9190f2be3a8919722b1c637c23b:
  compile/build/strict repeated-builder PASS, qa=true. Three changed runtime files
  and focused test file match integrated checkout byte-for-byte (4/4 SHA256).
  Later upstream delta is unrelated parry particle tuning, preserved by merge;
  scoped integrated Unity acceptance above covers the final checkout.
- First QA074847 failed only the protected font's Windows CRLF/Linux LF byte check.
  Downloaded font is identical after newline normalization; canonical LF snapshot
  passes without changing fonts, checker or scene/GUIDs. Checkout endings restored.
- Final native Edge155.0.4283.62 / Playwright1.58.2 / GTX1650Ti ANGLE D3D11:
  31/31 PASS in58.252276s at1280x720. Main-menu Settings unchanged; Pause Settings
  Back/Escape and Top1000 Close/Escape restore focus and frozen state. Actual
  inactive-panel text observations plus inspected images show no underlying Pause.
  Resume screenshot/trace holds compact PAUSE while controls are locked, then hides;
  rapid Escape/R/M causes no second transition and encounter reading stays frozen.
  Real Restart, reading release, natural Game Over after Resume, result actions and
  returns to menu from results/Pause pass. Unity also checks restored Victory layout.
  Nine final PNGs inspected, including Resume transition;0exceptions,5fixture reads,
  1intercepted ended-run score write,0production writes. All external requests remain
  blocked; bridge only observes. No audio-signal/performance claim.
- First25checks reached actual
  Game Over but driver incorrectly waited for hp0 (disabled actor reports hp-1).
  Corrected to existing finished/overlay state. A subsequent driver click was not
  observed at MainMenu and shutdown hung; task-only processes stopped, mouse hold
  made explicit in the external driver. No runtime changes to satisfy drivers.
- Final evidence and cleanup: [compact menu checkpoint](../plans/compact-menus.md).
  No full suite/stress/soak, normal production build or itch upload in this task.


## Pause fix published (2026-10-22)

Source db1936d; production20261022T082936/input6dd75a9b32f7e6baefc5b85f8fec6a95d9df83272dc6b1a115ff7aa0654a85a8
passes the normal build and complete ASCII/UTF16LE QA-marker exclusion.
Production invokes the builder once; the earlier Pause QA performed strict repeat.
Original-HTML native Guest/Play/tutorial reading/Pause/Settings/Escape/Resume/Menu
inspected, bridge undefined,0exceptions/provider writes. Existing scoped5Unity
and31native acceptance reused; no full suite, audio or performance remeasurement.
Published itch playable19943928; prior19915823 hidden/retained. Save and actual
public iframe/menu verified,31non-HTML CDN hashes match ZIP and index adds only
standard htmlgame.js. Fresh anonymous direct player menu loads with no bridge or
exceptions/writes. Raw anonymous root HTTP403 is not a passing guest-page check.
38non-upload fields,5page images,other upload flags unchanged. ZIP32members,
18,777,511bytes,CRC/hash PASS. BrowserSkill and smoke sessions closed.
Receipt/rollback/resources: [pause release](../../marketing/itch-io/pause-release-20261022.json),
[checkpoint](../plans/compact-menus.md). No devlog/social/provider mutations.


## Latest main demo verified (2026-10-22)

Local integration includes main916c75291250c34aed774db1c7ecc489b435b1b0.
Frozen source662223b4e2dc26cddad814d0668c6a2f6e258e6e41dc858493e1f69fd3122cf7:
33/33 scoped Unity cases passed, zero failed/skipped,2.6048547s, including
both cooled-floor-ember cases. Strict builder repeat/font protection and WebGL
build passed on pinned Unity2022.3.25f1. Native Edge155.0.4283.62,
Playwright1.58.2,640x360, NVIDIA GTX1650Ti ANGLE D3D11:20/20 checks passed
in16.998s. Real keyboard/mouse confirmed latest guide cards/text/arrows,
original frame2 on press/contact, no-word recovery without attack, two typed
words queue exactly one10-damage follow-up, pause ignores letters and retains
queue, two parries retain40HP, enemy180->170, valid sorting, zero exceptions
and scoreboard writes. Menu, guide and success entry screenshots inspected.
This is technical/static evidence; continuous perceived motion/force and a full
performance matrix remain unverified. Previous e17 evidence remains separate.
Downloaded32 player files/48,026,728bytes; every SHA256 matched the immutable
build. Reopened user page e98327a8-914c-45c1-8810-75f3e56380f7 at
http://127.0.0.1:8136/; loading finished, no warning, actual menu observed.
Owned private server workspace/results were removed after verifying task marker,
snapshot and no active Docker mounts. Shared Library/cache/source/home preserved.
## NYKNCK free main-lantern pixel flame (2026-10-22)

Scoped Quick20261022T081730 passes2/2tests (front lifecycle/mesh/emission/clock and rear regression), strict repeated builder PASS; no full Quick profile claim. QA20261022T082242/player5f849262aed3f42e native Edge155/NVIDIA GTX1650Ti/ANGLE D3D11 at1280x720 passes21/21real-input checks,0exceptions/provider IO.481read-only samples, active intensity22.5750618..25.2119675, all5cels, stationary root, pause and Slayer phase verified.

Source/output each5/5cels and5/5loop transitions reviewed separately; output native48x34/nearest2x. Actual canvas640frames decoded/exported with exact timestamps; selectedF77..116 andF166..223 reviewed98/98frames,96/96pairs at logical320x180/nearest2x640x360 plus40lamp detail crops. No room flash or whole-player parry/recovery blackout in those samples. Capture is25FPS VFR, not every engine frame or performance/FPS evidence. Earlier intro-race pilot is not acceptance. Direct-scene load audit20261022T084400 verifies the committed scene reference graph, front24/6percent, rear2percent, atlas/renderer bindings and disabled original flame. [Full checkpoint](../plans/combat-hanging-light.md).

## All three NYKNCK flames — demo candidate (2026-10-22)

- User extended the accepted central sprite approach to both rear torches. Main integration remains held for review; task branch `micharel09/lantern-pixel-fire` is retained.
- Scoped Unity20261022T091031-quick: **2/2 PASS**, one lifecycle test now exercises all three intensity/offset configurations plus existing legacy flicker regression; strict repeated builder PASS. Input96cdf597d5b1589172ac543c058871e9b9f3f4c5f1df8095f51f7a5f6d318c4a.
- QA WebGL20261022T091817, input195e9efb11661a6c2b14fb34307c7d4a1968f88e05298debbb3c395549177292, immutable playerf6eb222b78eb434e. Native Edge155/NVIDIA GTX1650Ti/ANGLE D3D11/1280x720:17 native action/assertion checks before the old aggregate telemetry assertion, zero exceptions/provider writes; completed real pause, parry, immediate following word attack, Slayer and menu return.
- Raw harness status is retained as failed: it interleaved same-named dormant source-room age0 samples with the playing room. An earlier run also rejected valid float32 intensity22.559999465 at the22.56 boundary. Corrected read-only `all-torches/check-runtime.py` separates the advancing/active room, validates the recorded manual-pause and Slayer windows, and permits1e-5 float tolerance: **all3PASS**,483 samples each (301active/182hidden), all5cels, synchronized emission, fixed roots, frozen age/cel during pause/hiding. No product-code changes were needed for these harness issues.
- Observed intensity: main22.5599995–25.2892056, rear-left15.6800003–16.2812004, rear-right15.6800003–16.2817173, within24+/-6percent and16+/-2percent modulo float representation.
- Final video SHA256c61d35d0fe5752a381d1e4a79ca04220e84b752e3b8a6acf5e0eaecacbb554d6. Export641/641 frames with exact source timestamps; directly viewed normalF69–108 (5.483595–7.101765s) and parryF166–234 (9.438540–12.238540s):109/109frames,107/107within-segment adjacent pairs at320x180 and nearest2x640x360. Also viewed all40normal frames as three separate native-capture2x lamp crops, and1280x720 normal/guard/post-parry/restored stills. Per-frame observations and transition notes: `all-torches/selected/frame-evidence.csv`.
- Roots stay attached to bodies/cage; local flame clusters change independently, warm wall/floor pools persist, and actors remain readable through parry/recovery without a scene-wide flash. Rear detail remains deliberately soft under existing DOF/bloom; VFR25FPS capture does not prove every engine frame, high-frequency shimmer absence or FPS/performance.
- Atlas hash14b26a8dc4566e7c7079107054cfbcc9056432b0b33c3fe3a9cb857cf7cce910 unchanged: prior source/final-cel5/5 plus loop-transition review is reused. No new art or reference-match claim. Original scene records/IDs preserved; only7existing records changed and8new rear-card records added, with values copied from the verified builder output.
- Evidence/demo root: `E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t/all-torches`; local review URL `http://127.0.0.1:8228/all-torches/demo.html`. WebGL initially hit the3GiB free-space guard; only six task-owned older results were removed after checking their archived snapshots and absence of active mounts. Shared cache/toolchain and other jobs preserved. No full suite, extra viewport, stress run or publication.


## Pinterest flame replacement - local checks passed, user demo pending (2026-10-22)

User rejected NYKNCK runtime visibility and the Pixie composite style; their earlier
receipts above are historical and do not accept the current art. Approved direction:
https://www.pinterest.com/pin/1135681231035570556/, 10x100ms, brick red/warm orange/
pale yellow, compact fixed-base flames for all three fixtures. Technical atlas identity
check passes all10cells, colors/alpha/order/timing preserved,270x160 Point atlas.
Final raw10/10cells/transitions reviewed; Unity2/2focused lifecycle checks and final
QA140347/player5b494ce8e2ac2083 native35/35PASS on Edge155/Intel UHD/D3D11/1280x720.
Zero exceptions/provider IO. Selected104/104frames and102/102pairs viewed at native
world320x180and2x;39three-lamp native-capture2x crops also viewed. Normal-camera
segment includes incoming hit/recovery; parry segment includes full preparation/contact/
recovery and following word attack. Exact634frame export retained; no whole-video,
every-engine-frame or FPS claim. Final size10/12pixels follows a rejected7/8pixel pilot.
Source atlas/timing/runtime code unchanged, so2/2unit results reused. Final saved task
records/material/quad match verified builder. User art review and main integration
remain pending; no external distribution claim. Current source, per-frame reference observations,
runner receipts and limitations belong to the existing hanging-light checkpoint.


## Authored 3D configuration completion (2026-10-23)

Canonical input `6b1258a53256b28cc07ab6bf716e8cf40ca994514d8ad31d31c7172f665299fa`,
Unity2022.3.25f1/fly-server, task run `20261023T033400-webgl`:

- Editor check PASS: edited blur center/half-width/depth edge, cloth, camera impact,
  haze/rune material controls, existing light/material/post survive two full UI
  builders. Selected alternate world has arbitrary room/owner names and lives outside
  Assets/Data/Lighting; incomplete unused template does not block validation/build.
  Additive scene setup and authored room/camera placement remain unchanged.
- Filtered PlayMode **39/39 PASS**, zero failed/skipped, 9.9696019s. Camera impact,
  real contact consumers, environment clock/cloth/rune, target projection, binding
  and session lifecycle; includes two-profile A -> B -> A, pulse snapshots,
  Pause/Slayer, independent local cloth motion and zero managed allocation across
  32 warmed parry/clear cycles. No general FPS/performance claim.
- External task adapter retains the shared runner lock and pinned images/license,
  uses the existing Unity testFilter, canonical LF text snapshots, and task-owned
  /tmp source/build/results. Repository runner/toolchain/packages unchanged.
- Final WebGL/strict repeat PASS; immutable player33files/48,078,641bytes all match
  server SHA256 receipts. Native Edge155.0.4283.62/Playwright1.58.2/NVIDIA GTX1650Ti
  ANGLE D3D11, one1280x720 viewport:53/53 real-input checks PASS in37.7076506s,
  zero exceptions/provider reads/writes/external requests. Normal/received/parry,
  active-pulse Pause/Resume, Slayer/lethal, actual post-parry word impact, Restart
  and Menu baseline restore pass. Raw shared report viewport header is stale;
  actual cameraImpact rows and render dimensions prove the single-size scope.
- Direct review46/46 sheets,355/355 frame observations and350/350 within-window
  transitions at320x180 and nearest2x640x360. Overlapping normal/received windows
  yield340unique source frames/336unique pairs. Selected windows: F0-18, F4-43,
  F507-581, F735-805, F898-1047 of the1048-frame VFR WebM. Source PTS retained,
  including648ms F10->11 gap; no every-engine-frame or FPS claim. Warm follow-up
  contains full normal contact/recovery. Cloth is dark/subpixel at approved exposure;
  pinning/private mesh/local direction and pause/hide phases have focused Unity evidence.
- No observed selected-scope regression in warm flame colors/anchors, wall/floor
  lighting, actor focal clarity, parry release or restored room. Baseline native
  camera-impact sheet/Pause image inspected; source atlas/mesh/clip checks reused
  because these inputs are unchanged. No new reference-game/full-video match claim.
- First native pilot reached7checks but requested Pause after the180ms pulse expired;
  trace retained. External driver now sends real Escape key-down promptly and still
  requires a positive active envelope before asserting freeze. Product unchanged.
  Canonical build recovered a stale shared-cache missing diagnostic-script compiler
  message during refresh and completed successfully; no diagnostic script added.
- Exact sources, per-frame CSV, receipt and cleanup belong to the existing
  [lighting checkpoint](../plans/combat-hanging-light.md). No full suite, stress/soak,
  production publication or hardware quality tier claim.


Final Editor-only Preview follow-up `20261023T035720-serialize`, input
`530e3293c47fd5e0955600a083e759d6982e604f51e066de804829c733f00286`:
standalone VerifyPreviewScenes PASS. Two loaded worlds receive their own fog;
original active scene retained, Undo restores both original fog values. Preview
now selects each owning scene before recording/applying RenderSettings. Record
also covers camera data/combat rigs. Only Preview plus its Editor check changed
since the passing QA input (then indentation only); production Apply method,
runtime/shaders/profiles/materials/scenes are byte-identical. Reuse39Unity/53native/
strict WebGL/visual evidence. Stock runner serialize completion label is generic;
this follow-up does not claim another full builder repeat. Proof/receipt in checkpoint.


Dynamic sprite shadow acceptance (2026-10-23): scoped43 Unity tests PASS; final
2 SpriteShadowTests rerun includes zero warmed managed allocations. Editor
custom-normal preservation and strict two-world repeat PASS. Frozen main1f22565
and candidate QA WebGL builds passed; native GPU1280x720 each14 real-input checks
cover hits/parry/follow-up/Pause/Slayer/Menu. All261 poses were inspected in1044
left/right/mirror GPU cells, plus10 above/below diagnostics. Complete selected
normal/parry/Slayer windows have per-frame native/2x review; initial normal video
gap was replaced with continuous recording. Static pose coverage does not claim
every full runtime clip. Non-recording401-sample active comparisons: RAF p95
19.025 to19.020ms; observed draws median277 to165; no FPS gain claim. Exact snapshots,
timestamps, limitations and retained evidence are in the existing
[lighting checkpoint](../plans/combat-hanging-light.md).


## Coordinated Slayer transitions (2026-10-23)

- Scoped Unity20261023T090657:8/8PASS,0fail/skip. Entry450/exit650, curves,
  duplicate/reversal,180/500ms deltas, Pause/results, disable/re-enable, weather/Volume
  property restoration, dynamic silhouette/exclusions, actual scene lifecycle and
  camera-impact composition. No full Quick/stress/soak claim.
- Final QA20261023T092633: strict repeated builder and protected typing assets PASS,
  input668e6246dcced708e7a562f06ea85bb849117e68da0bed7d17bd79dbdbf8f46b.
  Native Edge155/GTX1650Ti ANGLE D3D11:32/32PASS at1280x720/640x360; real keyboard
  entry/exit,180ms hitch, Pause/timer/Resume, lethal/results with500ms hitch,
  Restart/Menu.0exceptions/provider reads/writes; fixture interception and external
  blocking remain. Nominal durations finish on the next rendered frame, not a
  sub-frame scheduling guarantee. No device-performance claim.
- Direct visual review: all479frames/471consecutive transitions in8complete selected
  segments. Original VFR timestamps/held intervals preserved, no sampling; source
  ranges/counts/manual notes are in the existing Slayer checkpoint and retained
  review-inventory.json. No observed transition pop or stuck state.
- Intermediate font gate failure was CRLF/LF only, proven byte-equal after
  normalization; adapter normalizes text snapshots without changing repo files or
  weakening checks. Initial HDR red181 was rejected; final red117 versus legacy118
  is a1/255 quantization difference with unchanged authored color. Failed attempts
  remain identified; no bit-identical palette or full-game/reference-match claim.
- Merged origin/main1a7895c as13d7304, preserving authored normals/shadows.
  Integrated QA20261023T094605, inputfeafdf96f9f5a77e39f7e07a502671f3be22c51c677ab89b12a4f9f8b2ec0bc6:
  strict repeat/protected typing assets PASS;32/32native checks again at both sizes,
  0exceptions/provider IO. All29normal PNGs remain byte-identical after builder.
  Directly re-reviewed483/483frames and475/475transitions in8complete integrated
  clips, including both hitches/Pause/results and sheet boundaries. Prior479frame
  receipt is historical; final integrated source/frame counts are in the checkpoint.
  Reuse Unity8/8: no changed timing/tint/lifecycle logic after its passing run.
  No itch.io publication.


## Final cue-integration acceptance

Current runtime c6f6802 preserves main668d3c2 lighting and guard-break/body-fall cues.
QA20261023T100031-webgl strict repeat/protected-font checks PASS; normalized input
5613e279864d13272436f8a143f5d1692ba5f8f0e1558e39d2adc0f24b49fce1.
Native32/32PASS at1280x720 and640x360, zero exceptions/provider IO.
Directly inspected488/488frames and480/480consecutive transitions in all8windows
selected above, preserving original timestamps/held frames and sheet boundaries.
1280:53/52entry,65/64exit,99/98hitch+Pause,32/31lethal exit.
640:50/49entry,63/62exit,90/89hitch+Pause,36/35lethal exit.
No observed background/tint pop or stuck bars; guard-break sparks and body-fall
particles remain separate from the background blend. Pause holds, Resume settles,
results exit and Restart/Menu restore baseline. Camera impact composition passes.
All29authored normal PNGs remain byte-identical after builder. Reuse scoped Unity8/8
because the merged cues do not change the tested presentation clock/lifecycle logic;
the affected render consumers were checked in this final player. No gameplay timing,
input, damage or scoring changes; no itch.io upload.

Authoritative current artifacts: E:/CodexTemp/TypingHero/slayer-soft-transitions/
acceptance.json and native-latest/review/review-inventory.json (per-source SHA256,
frame ranges, counts, observations). Retain these artifacts and immutable server
result20261023T100031-webgl for review; remove after review when no longer needed.
Earlier483/479frame receipts are historical, superseded by this488frame receipt.


Enemy state cue removal (2026-10-23): user deferred GuardBreak/BodyFallDust.
Reverted e882aba runtime/assets/clip events and builder wiring; preserved newer
Slayer transitions and lighting. Historical integrated VFX test receipts above
describe the removed candidate, not current enabled features.
Byte comparison confirms restored runtime, clips, prefab and original VFX assets
match pre-cue e882aba parent; remaining SampleScene edits exactly match newer
main scene changes. No references to deleted asset GUIDs remain in serialized
assets/source. Reuse prior original MetalClash/death evidence; no runtime or
visual behavior change beyond restoring that version, no new runtime test run.


## Hit-flash composition acceptance (2026-10-23)

Runtime5d82f96 on main9bd927b; immutable QA20261023T120206-webgl,
input7e88519966b671fbe3751f85539c5a6d36235d27c27434a69a3fcd58defc8e5e.
Before-fix two regression cases fail; after-fix14 distinct scoped Unity cases pass
(13+1 runs above),0fail/skip. Public APIs, shader/material and450/650ms clocks unchanged.
The group composes active flash RGB with silhouette and preserves live alpha; flash
stores untinted baseline. Endpoint hands back an active flash; disable cancels old state.

Native Edge155/Intel UHD ANGLE D3D11,1280x720:23/23PASS,0exceptions/provider IO.
Four keyboard-driven exit attempts captured; first normal-hit flash at weights
.600952/.529466, fourth at.004261 then0, proving it survives the endpoint.
Pause/Resume and Restart/Menu pass. No gameplay mutation through QA bridge.
Direct review:75/75encoded frames and73/73within-segment consecutive pairs across
both complete windows selected above:38/37middle and37/36endpoint. Original PTS,
held intervals and page-boundary pairs preserved. All four sheets inspected.
Flash is visible in the returning scene; neither endpoint erase nor residual dark
baseline observed. Local slash/sparks stay distinct; bars and room recover normally.
This is color/presentation QA, not sprite animation authoring or reference research.

Current artifacts: E:/CodexTemp/TypingHero/slayer-flash-composition/acceptance.json,
native/browser.json, original video and native/review/review-inventory.json with
source SHA256, frame ranges and manual observations. Keep for review; removable
after review. Immutable server result120206 and three scoped test receipts remain.
No full suite, extra viewport, stress/soak, performance claim or itch.io upload.
Implementation/acceptance complete; pending remote integration verification/cleanup.


## Dedicated parry combo_2 follow-up (2026-10-23)

Pinned Unity2022.3.25f1/fly-server final result20261023T144919-webgl, input
8be3e31c25b53c32b7ec40ca8b47cbb138b5aeee48b42374489a12e689a1f050:
strict repeated builder/WebGL PASS;89distinct focused PlayMode cases covered
(PlayerParry33, CombatFeedback18, CombatCameraImpact38). Runtime-final source
passed87/89 in144229; exact800ms fixture arithmetic and the existing timing-sensitive
180ms camera case failed. Final9/9targeted rerun passes both; only the fixture's
arithmetic grouping changed, with camera assertion/production code unchanged.
This combines scoped runs, not a single89/89 suite; prior0c74 passed88/88.
Final immutable33-file player hashes match local demo. Native Edge155.0.4283.62,
Playwright1.58.2/Intel UHD ANGLE D3D11, one1280x720:47functional +13queued
Restart/Menu checks PASS,0exceptions/provider writes, fixture blocking enabled.
Full10cels, one10-damage hit, early/late/no-input/pause, actual contact/hit flash/
camera/spark/audio verified. Source cels20/20,22/22key positions,18/18pose pairs +2/2hold pairs plus4joins;
reviewed0c74 runtime segment71/71frames and70/70pairs (six final authored asset
hashes unchanged); final-player native receipts are native-final/ and native-final-lifecycle/.
Mirrored VFX71/70 is a pixel
preview, not an actual runtime flipX lighting capture. Full per-frame evidence,
initial truncated-length/ownership regressions, scope and resources are in
[parry checkpoint](../plans/parry-main-demo.md). No full performance/balance or release claim.


## Remove user-image parry recovery pose (2026-10-23)

Image identifies combo_3_end_5/fileID1786144713. Both success clips now2/3/4/4;
950ms recovery and full combo_2 remain. Incorrect combo_2_0 removal was corrected
before main integration. Unity2022.3.25f1 result20261023T152926-webgl/input
673572ba16354a7df6a61b18584d23257f22461aa6494f3b9d148cc7c4d594a2:
37/37focused PASS, strict repeat/WebGL PASS,33player hashes match. Native1280x720/
Edge155/Intel UHD D3D11:21/21PASS,0exceptions/provider writes, all10combo_2 cels,
one10damage and explicit user-pose5 absence. Direct final38/38normal +38/38pixel
mirror frames and37/37pairs each; source30key rows. Existing idle0 transition and
pixel-mirror lighting limits are documented in the [parry checkpoint](../plans/parry-main-demo.md).

## Remove intermediate idle before queued follow-up (2026-10-23)

Pinned Unity42/42, strict repeated builder and WebGL build PASS; immutable result
`20261023T160209-webgl`, snapshot `de5bee8c4376827ead46f1f39cc54c6a4ebec84d0466bf6a0cc876c78978e09e`.
Native Edge155/Intel UHD ANGLE D3D11 at1280x720 passes22/22 real-input checks,
including no idle between success and combo_2, ten cels, one10-damage hit and idle tail.
Runtime video selected3.548090?6.329125s before added-frame review: sourceF34?74,
41/41encoded frames and40/40pairs inspected normal and pixel-mirror preview.
Pixel reflection does not verify mirrored runtime lighting. Source sprite/clip assets
are unchanged; prior source/VFX-off review remains applicable. See checkpoint.

## Space cue HUD proximity follow-up (2026-10-24)

Input4933a167: normal180518 and QA181516 WebGL builds PASS; strict repeat-builder PASS. Scoped native layout: normal5checks, QA19checks PASS; both640x360/1280x720, tutorial target arrows/copy, fixed tray bounds, real-parry Slayer, pause/resume, actual8-letter queue and cue switching. Direct screenshots inspected, no runtime suite repeated; [checkpoint](../plans/parry-cue-demo.md) distinguishes earlier animation clips from this layout update.

## Explicit first follow-up cel removal (2026-10-24)

User requested a quick removal of combo_2_0. Nine retained cels now start at
combo_2_1, finish516.667ms; contact events shift50ms earlier (100/150/200ms).
Pinned Unity42/42 PASS (`20261023T171621-webgl`); strict builder and
source normal/mirror VFX-off review10keys/9pairs each complete. WebGL build,
new real-input recording and final runtime join/playback were cancelled at the
user's speed request and remain unverified. Existing demo is the preceding build.
See [checkpoint](../plans/parry-main-demo.md).

Tempo follow-up (2026-10-24): retained nine cels stretched34/31 to566.667ms;
static serialized-reference/event/key/duration checks pass. Tests updated but
not rerun in this quick preview iteration; prior42/42 is not new-build evidence.
New sprite preview provided; runtime WebGL and complete timing visual acceptance
remain unverified. [Checkpoint](../plans/parry-main-demo.md).


## Compact Game Over / Victory (2026-10-24)

QA20261023T184327/input43e1f3b6a5dafd1609a3f9e3ad48e569b9c310191a56f2bf16869970d18d730a:
Unity2022.3.25f1 strict repeat and4/4 focused result/pause PlayMode PASS.
Native Edge155/NVIDIA GTX1650Ti/D3D11 at1280x720:191 checks PASS, zero exceptions/production
writes, actual typing/parry through six opponents to Victory plus natural death,
result Settings/modal/Restart/Menu and Pause/Resume. Six final UI images inspected.
Card600x366/title30/summary24, four actions, audio in existing Settings, no clipping.
After integrating main cb54e5f, Quick185318/input
d1188082e78510a576d2d56a5e03bc27cf92038a6981b304e7b57a903811bc7f:
4/4 PASS,2.887987s and strict repeat PASS. Full SampleScene semantic records match;
18 result records and all UI code unchanged from native-reviewed player. Actor
shadow visuals retain their separate main receipts. Initial test teardown name
collision and unsuccessful native pilots are superseded by these final runs.
[Scope, failed-attempt attribution and retained UI preview](../plans/compact-menus.md).

## Space cue pre-main integration review (2026-10-24)

Integrated main3cd7293 with cue/HUD. Inputa880ecef: Quick0119563/3 and QA0120597/7 scoped tests PASS; strict repeat-builder, QA WebGL and normal013118 PASS. Native integrated1280x72016checks PASS (real parry/early/late/lock, Pause/F8, errors/timeout, Slayer/win/next encounter/restart/menu). Normal5checks PASS and both-size images directly inspected;33remote hashes match each player,5payloads exclude QA/depth-preview markers. Scene records match generated output after canonicalizing four inactive font-bound values. Fixed bounded legacy-Graphic ownership on target rebind;20-target regression covered. No remaining merge blocker found; [checkpoint](../plans/parry-cue-demo.md) records evidence and limits.


## Result hierarchy revision (2026-10-24)

Unity2022.3.25f1 result023638/inputd11258b8:5/5focused result/Pause/Resume/terminal
save tests PASS,0failed/skipped,3.0599973s; strict repeated builder and QA WebGL PASS.
Both scenes semantically match generated records; only the result subtree changed
(10existing records,51new,all old IDs retained). Native Edge155/Intel UHD/D3D11,
1280x720:203checks PASS in124.94s,0exceptions or production writes. Natural defeat,
actual six-opponent victory, four actions, Settings/leaderboard roundtrips,
Pause/Resume/Restart/Menu verified. Six full screenshots inspected; primary width
304versus232 and aligned stat rows are readable. Victory3629/6/63.9s/100%/10parries.
Reference source/video review and failed candidate checks remain in the
[existing checkpoint](../plans/compact-menus.md). UI preview and retained receipts:
E:/CodexTemp/TypingHero/result-layout, http://127.0.0.1:8295/review.html.


Integrated cue/torch main4e7ccc6: Quick030507/input1b30dee7 passes the same5/5
scoped tests in3.3917055s,0failed/skipped, with strict repeat builder PASS. Both
complete merged scenes match generated semantic records; all61result records and
UI code match the native-reviewed d11258b8 player. Main's cue/lighting evidence is
preserved separately; UI preview does not certify those newer visuals.

## Clean gameplay letterbox (2026-10-24)

Normal WebGL034449/inputd7f29cd2 passes build and QA exclusion;33downloaded
player hashes match remote. Native Edge/D3D11 real-input gameplay/Pause/Resume,
1280x720 ->1920x900 ->960x900 ->1280x720, wide MainMenu return/re-entry pass.
Directly reviewed images preserve16:9actors/torches/HUD/typing; bar interiors in
four wide/tall image pairs contain0nonblack pixels,0captured runtime exceptions.
Old normal030724reproduces stale menu pier/torch strips on wide gameplay entry.
Final generated world/profile/meta match that baseline; only presentation render
code/shader changed. Full QA/performance/video waived. Snapshot receipts, pixel
measurement bounds and retained demo: [lighting checkpoint](../plans/combat-hanging-light.md).


## Result HUD suppression (2026-10-24)

Unity2022.3.25f1/fly-server run035514-webgl/input4a9a713b:5/5scoped PlayMode
checks PASS,0failed/skipped,3.7247909s; strict repeated builder and QA build PASS.
Thirteen valid authored groups hide visuals/interaction at terminal results,
retaining logic and ordinary Pause. All generated scene records and33downloaded
player hashes match. Native Edge155/Intel UHD ANGLE D3D11,1280x720:229checks PASS
in121.9313421s,0exceptions,2intercepted Endless fixture writes. Seven complete
images inspected; result/modals hide HUD, Restart/Menu/new gameplay restore it.
Earlier builder-null and test-wait failures, external probe false positive, scope
and retained artifacts are recorded in the [checkpoint](../plans/compact-menus.md).
No performance/full production/sprite-motion or publication claim.

## Automatic authored sprite depth (2026-10-24)

Production050935/inputb2ddfed:5/5Editor synchronization tests and2/2runtime depth
tests PASS,0failed/skipped. Covers missing->mapped discovery, custom/migrated
bindings, invalid maps/packing, stable frame metadata/rename/prune, unrelated dirty
asset preservation, facing/world clear/restore, combat property blocks and warmed
allocation guard. Normal build succeeds with280mapped/1missing; QA excluded.

Integrated main168ae6b adds the authored lift map and verified result HUD behavior.
Unchanged feature/test sources reuse those7tests; actual serializer and normal
052952/inputaf65179 PASS with281mapped/0missing.33player hashes match remote;
99lighting PNGs and all281mapping references/float32 values are preserved. The
saved generated index matches production output; only stable order/number format
changes. Native Edge/D3D11 production1280x720 real-input parry success/recovery,
Pause/Resume and5inspected images PASS,0exceptions, no QA setters/video. Reuse
unchanged renderer's wide/tall letterbox evidence. Full suites/animation/performance
remain waived; [checkpoint and retained receipts](../plans/combat-hanging-light.md).

## Per-strike white Space warning above enemy (2026-10-24)

Phase025930/input02175c97:5/5 scoped Unity tests PASS, including14 real manual
early/fill contacts across Classic and all six weapon attacks, independent next
press, prediction/speed/pause/cancel and20rebinds. Head031855/input3ee81546:
1/1 anchor/viewport/weapon-bounds test PASS; strict repeated builder and QA WebGL
PASS. Normal033136 builds the same3ee81546 tree and excludes the QA bridge.
Native muted Edge155/D3D11/Playwright1.58.2:26/26 checks PASS, real keyboard/mouse,
640x360/1280x720 head geometry, parry/early/late/held input, Pause/F8, typing,
Slayer/results/combo/restart/menu;0exceptions/provider writes. All9screenshots
directly inspected. Full selected live capture278/278frames and274/274transitions
directly reviewed; separate seven-attack mirrored314frames/307pairs and original
300frames/293pairs reviewed. Authored sprites unchanged. Representative lit-scene
coverage and subjective playtest limits are explicit in the
[checkpoint](../plans/parry-cue-demo.md); no all-seven VFX or performance claim.
Normal native1280x720 smoke5/5 PASS,0exceptions/provider writes; eight images
directly inspected, including the live head cue. Both players match33remote hashes
each; five normal JS/WASM/data payloads exclude ASCII/UTF16 QA/depth markers.

Integrated main9c48f5d through merge51c9e7b, retaining upstream parry recovery and
enemy-head Space warning. Only the verification append conflicted; both receipts
were retained. GamePauseController, HUD builder/test code and both full UI scenes
are byte-identical to the native-reviewed UI commit10b7f0f. Integrated Quick042228
/inputb9eacfd1:5/5scoped PlayMode PASS,0failed/skipped,3.6638901s, strict
repeat PASS; both complete scenes match every generated semantic record. Native
229-check evidence is reused for unchanged HUD behavior. Newer upstream parry/cue
visuals retain their own receipts; this task adds no sprite review certification.


## ParryImpact burst (2026-10-24)

- Integrated main d6d99dd + scoped ParryImpact change: final immutable input
  b5a3b7ab9460c7e453252c2a5dfd7e971262b53adf5fb12672332e12747228fd,
  Unity2022.3.25f1/fly-server result20261024T055845-serialize. Strict repeated
  builder PASS; focused ImpactVfxTests40/40 PASS,0failed/0skipped,8.361289s.
  Whole generated Player/SampleScene and profile equal reviewed inputs.
- Both40px flash sizes/core2.5WU clamp,100+100ms alpha/200ms expiry, contact,
  pause/resume/pool and same-seed physical traces in both directions pass.
  MetalClash standalone and custom-profile/custom-prefab binding remain covered.
- Native Edge156/Intel UHD D3D11: actual1280x720 real Space parry both directions,
 9checks each PASS,0exceptions/provider writes.197/197displayed PNGs and195/195
  transitions reviewed. Final audible1x/.25x movie398/398frames/397/397transitions
  inspected and played through; original timestamps/held frames retained.
- Later main's281depth entries are float32/reference-equivalent; new letterbox
  pass clears only outside the viewport. Reuse scoped visual evidence after exact
  final generated-output comparison. No performance/full-suite/production build
  or itch upload. [Receipts, cache correction and clip](../plans/parry-impact-burst.md).

## Parry sparks and directional shock arc (2026-10-24)

- Final input6331af8e49955345088499ef92351e3e3b92a6171836a265ba6eab413c4cc4ed,
  Unity2022.3.25f1/fly-server20261024T070951-serialize: strict repeated builder PASS;
  ImpactVfxTests41/41PASS,0failed/skipped,8.7500326s. Covers same-seed trajectories,
  24seeds/two directions with readable streaks after350ms, flash/arc size and expiry,
  tutorial/timeScale pause, reuse/mirror reset, MetalClash trace and custom bindings.
- Normal QA063750/b7c11712 and mirrored QA065830/854d4967 build/repeat PASS.
  Both have identical VFX payloads. Final rendered assets/importer match source;
  qualification delta is whitespace/native main-object metadata and corrected test
  fixture order, with unchanged runtime code/material/texture/physics values.
- Native Edge156.0.4314.42/Intel UHD D3D11 at1280x720:11/11checks per direction,
  actual Space/40HP/+30, post200ms multi-pixel streaks, tail expiry and real Escape
  Pause/Resume pass;0exceptions/provider writes, fixture interception retained.
  Direct review covers252/252runtime frames and250/250transitions plus8/8arc cels
  and7/7transitions per direction. Hollow arc opens toward enemy; longer orange
  streaks/embers remain after core clears, player eye/body/feet stay readable.
- Audible1280x720 preview retains508/508decoded frames/507/507transitions;
  all directly inspected, including40contact crops. Original displayed timestamps
  retained to1ms, terminal idle repeated40ms per1x block, no interpolation. Both
  directional files played through in headed Edge at1x/.25x, unmuted/volume1;
  measured decoded audio peaks0.47-0.53. Independent subjective listening approval
  is not claimed. No performance/full-suite/production build or itch upload.
  [Detailed receipts, failed pilots and artifact paths](../plans/parry-spark-arc.md).

- Integrated c86d72f: serialize074942/input65e72571, strict builder and41/41PASS,
  8.7472983s. ActorDepth/Player/VFX match source; all1121generated scene records,
  GUID/local/listener refs and float32 values match reviewed QA after canonical ID
  mapping. Later editor-only7694ba1 qualification reused after reading31/31actual
  XML and matching11source/config/index hashes;281mapped frames/render data unchanged,
  ParticleSystem effects excluded from receiver discovery. Shared Node parry-role
  filter accepts old/new prefab names; all252captured frames/53live frames pass.


## Scene-aware depth synchronization (2026-10-24)

- Isolated pinned Unity2022.3.25f1, result20261024T071611-quick,
  inputa3609b88b8b993edeb99baffa493be50ce882eaa29e060e42644904658395a95:
  17/17Editor +2/2runtime PASS,0failed/skipped. Controller/sprite/contact
  overrides, non-prefab actors, external animator/renderer references, saved and
  unsaved scenes and unloaded scene entries retain scene setup/active/dirty state.
- Valid replacement `_DepthMap` updates the derived index without editing it;
  invalid replacements and ambiguous unbound legacy maps still reject without
  index writes. Existing migration/pruning/import/packing/map-preservation checks
  and runtime frame/facing/flash/tint/world switch/missing-map checks pass.
- Real QA-mode serializer/repeat builder PASS; two actor bindings, world clear,
  world without depth and return PASS. All99lighting PNGs and10checked
  source/config/index files match.281mapped actor frames unchanged; the newly
  discovered ContactShadow child had no map. This overbroad discovery allowed
  production fallback but broke strict audit/CaptureAll input validation; the
  receiver-scope correction below supersedes that classification.
- Editor-only change; no shader/runtime/combat/schema/data-image edits. Full QA,
  WebGL, animation review and profiling were not run. Earlier compile/fixture
  failures, exact XML/logs, source identity and integration receipt are retained at
  E:/CodexTemp/TypingHero/depth-scene-sync-20261024. Details in the
  [lighting checkpoint](../plans/combat-hanging-light.md).

## Receiver-scoped depth audit (2026-10-24)

- Unity2022.3.25f1/fly-server20261024T074545-quick, input3d80507630403c0db3c5bcf5d2b1e2b9ed6861614dd85ab89cf4d231607d30df:
  31/31Editor PASS,0failed/skipped. Main actor renderers auto-qualify before
  builder binding; secondary/external renderers require SpriteDepthBinding.
  Disabled binding/renderer and zero strength retain authoring requirements.
  Mixed actor/VFX curves, persistent prefab references, shared sprite identity,
  controller/override/hurt/contact/Endless and scene-state checks pass.
- Public AuditCoverage and CaptureAll's exact strict Validate(library,true) PASS.
  Missing actor map still rejects strictly while production permits fallback.
  Production frame set equals authored index,281mapped/0missing; previous
  unbound ContactShadow is excluded, with no lost authored frame.
- Actual QA-mode two-pass builder and world bind/clear/no-depth/restore PASS.
  All99lighting PNGs, library/world/material and12checked source/config/index
  files match after serialization. Runtime/test/config/index unchanged, reusing
  prior2/2runtime tests from071611; no new runtime suite, WebGL build, all-frame
  render, full QA or sprite-review claim. Exact XML/logs/hash/inventory delta and
  integration receipt: E:/CodexTemp/TypingHero/depth-receiver-scope-20261024.
  See [lighting checkpoint](../plans/combat-hanging-light.md).


## Renderer discovery and capture preflight (2026-10-24)

- Referenced prefab receivers include the nearest self/ancestor Animator even
  when the reference is unsaved and absent from disk dependencies. Excluded
  renderers still stop before following the Animator; receiver curve filtering
  and map selection/migration/pruning are unchanged.
- CaptureAll strict-validates the current scene state before checking every
  loaded scene for dirty changes. Capture/Investigate share that dirty-scene
  guard, including scenes outside Build Settings. No auto-save, library sync,
  GPU check, output or destructive scene replacement precedes preflight success.
- Unity2022.3.25f1/fly-server Editor20261024T101721-quick:38/38PASS,0failed/skipped,
  56.8565823s. Seven new cases cover unsaved prefab animation on self/inactive
  ancestor, no-binding exclusion, disabled/zero-strength authoring, strict missing
  map before dirty/GPU guards, actual CaptureAll/Capture/Investigate entry points,
  scene/override/output preservation and clean complete preflight without GPU.
  Existing31sync/discovery cases include production inventory equality against
  the saved index without a fixed frame count.
- Builder20261024T102114-quick/input
  942e01c8184ae0cb02c39a035d347a5b58d99ae094c95b3de914e07b2b2a93cb:
  QA-mode repeated builder and selected-world bind/clear/no-depth/restore PASS.
  All209protected files (including99lighting PNG and library/world/material)
  remain byte-identical. First builder failed existing font protection on CRLF
  archive input; canonical LF snapshot passes without changing that protection.
  The38passing Editor cases are reused across this normalization-only retry.
- Prior2/2runtime depth cases from071611 reused after Git comparison confirms
  runtime/test/shader/library/material/package/project inputs remain unchanged.
  No frame rendering, WebGL build, full QA or new animation/performance claim.
  XML/logs, source/artifact/protected SHA256 and runtime reuse evidence:
  E:/CodexTemp/TypingHero/depth-preflight-fix-20261024/receipt.json.


## Pimen parry core and physical depth (2026-10-24)

- Scoped Unity ImpactVfxTests:43cases qualified,42/43 first run plus corrected float
  assertion1/1 retry;0skips. Only1e-6 lifetime assertion tolerance changed between
  immutable inputs;2433file comparison confirms runtime/assets identical.
  Covers480ms core/cell timing/mirror/size cap, materials/importers, all22physical
  position/velocity seed traces, pause/hitstop/pool/restart, missing-default/repeated
  builder/custom Inspector and independent MetalClash. No broad suite added.
- Normal20261024T112344 and mirror20261024T114327 WebGL/strict repeated builder PASS;
 33files/player hash-verified; same compiled WASM/framework and68VFX files.
- Native1280x720 real-input right16/left18checks PASS,0exceptions; GPU10checks PASS
  establish actor depth-write before Pimen LEqual against same depth attachment.
- Direct original-frame review147+131frames/146+130pairs, full and4x logical crop;
  final video148+132frames/147+131pairs decoded and directly inspected. Final VP9
  exports preserve all source timestamps and original Opus payloads, with1ms terminal
  held duplicate only. Clip playback to end at1x/0.25x unmuted has nonzero audio;
  subjective human listening not claimed. Viewer presentation drops are recorded,
  while separate decoding verifies every encoded frame. Supplemental diagnostic
 254normal frames/253pairs and48zoom frames includes earlier missed strike explicitly.
- Actual tail visible after480ms, no observed floor penetration or abrupt expiry;
  guard/counter readable. Pimen source/crops remain exact RGBA and source SHA-backed;
  no synthetic arc. Existing MetalClash/runtime clocks/camera/audio unchanged.
- Full sources, qualifications, hashes, per-frame evidence and exclusions:
  [Pimen/depth checkpoint](../plans/parry-pimen-depth.md). Artifacts retained at
  `E:/CodexTemp/TypingHero/parry-pimen-depth`. No itch.io upload or performance claim.

- Integration checkpoint:34ef5ee merged newer origin/main903344c in2d346ce; all26
  task build-input delta paths unchanged. Additional integrated ImpactVfxTests/
  strict-builder checks could not execute: fly-server SSH timeout, peer offline.
  Integrated revision remains **UNVERIFIED** for these checks; main push withheld.
  Pre-integration scoped results above remain valid only for their immutable inputs.
  Resume commands and retained worktree are in the checkpoint.


### Parry variation candidate (2026-10-25; runtime unverified)

Per-impact core38/39/40logical pixels and +/-3degrees implemented on task branch.
Local compiled sampling check2050seeds PASS, physics/seed producer/MetalClash inputs
unchanged; focused Unity coverage added for deterministic replay/mirror/pool and
all22unchanged trajectories. Runner remains offline: Unity44cases, strict builder,
QA build and three consecutive real-input parries per orientation have NOT executed.
Zero new variation runtime frames inspected. Prior unvaried results are not evidence
for this candidate. Main push withheld. [Resume/checkpoint](../plans/parry-pimen-depth.md).


### Parry variation local acceptance (2026-10-25)

Supersedes the candidate runtime block above. Existing Windows Unity2022.3.25f1
(530ae0ba3889), WebGL module and valid local Personal entitlement reused.
ImpactVfxTests ran once:44/44passed,0skipped. Normal and mirror QA builds both
passed their strict repeated builder. Frozen Unity inputs remain identical.
Each orientation:24browser checks,0exceptions,0fixture writes, three consecutive
Space parries and real typed counters in one fresh fight at1280x720.
Source visual review:330/330frames+329/329pairs right;331/331+330/330 left,
at logical size and4xnearest crop. Encoded review:331/331+330/330 right,
332/332+331/331 left, including1ms final held duplicate. All source timestamps
and original Opus payloads preserved. Both clips ended at1x/0.25x unmuted with
nonzero audio output; viewer presentation drops recorded separately from complete
decode inspection. No subjective human-listening or performance claim.
Contact/mirror/tails/clear intervals pass;22physical paths,480ms/six80ms cells,
38-40px core and+/-3degrees unchanged. No broader suite or production build.
[Final evidence, diagnostics and reproduction](../plans/parry-pimen-depth.md#final-local-verification-2026-10-25).


- New main79f5052 integrated as9556fd3:44/44focused tests and both strict QA builds
  pass again. Parry source/assets unchanged by12Unity-input Slayer delta, so full
  reviewed Parry clips above remain valid for their exact recorded inputs. Additional
  native24checks/orientation verify three consecutive real parries after integration;
  preparation includes HP40->35 hit, then35preserved across each parry. A failed
  left input attempt is retained. Only affected ending fully reviewed: right82frames/
  81pairs, left88/87, logical+4x; supplementary captures are not full-reviewed exports.
  Exact hashes, review exclusions and remote receipt are in the same checkpoint.

## Local Windows adapter isolation and retry

`local_windows.py` prints a fresh `results/<UTC timestamp>-<UUID>-<action>`
for every invocation. Tests accept only that invocation's XML, requiring root
`Passed`, exactly 44 cases and every case `Passed`; missing/malformed XML fails
regardless of Editor exit status. Earlier results remain untouched.

Builds stage under the invocation's `player` directory. Existing `player-right`,
`player-left`, mirror fixture or fixture meta causes rejection before source edits.
Mirror builds back up SampleScene and its meta byte-for-byte, then restore both and
remove the created fixture/meta after the owned Editor exits, including failures
and Ctrl+C. A successful build is promoted only after restoration succeeds.
Failed partial players/logs/backups remain in that invocation's directory; retry
with the same workspace and Library. Restoration errors are failures: use the
retained `.backup` files to recover source before retrying. Preserve an existing
successful player or select a new workspace; the adapter never overwrites it.

All actions take an exclusive, nonblocking Windows byte-range lock on
`.local-windows.lock` before source preflight; a busy workspace fails immediately.
The lock stays held until the owned Editor exits, source cleanup finishes and
successful output is published. Keep the lock file: its presence is normal and the
OS releases the actual lock when its owner exits. It does not coordinate manually
launched Editors or older adapter versions; use one adapter version per workspace.

Ctrl+C is deferred through source cleanup. The owned Editor is terminated and
awaited (30seconds before kill), scene/meta are restored through sibling temporary
files with atomic per-file replace and byte verification, then the created
fixture/meta are removed. A cancelled run exits130 and keeps partial output under
its result directory. Repeated Ctrl+C does not interrupt cleanup. If terminate/kill
fails, the adapter keeps waiting with the workspace locked until the Editor exits.
Cancellation after completed output publication does not undo a finished run.

Restore errors return failure and list the retained backup paths; recover both
source files from their invocation's backups before retrying. A failed temporary
file deletion reports its exact path. Scene and meta are restored separately, not
as a two-file transaction. Forced termination/power loss cannot guarantee cleanup;
inspect backups and fixture state before reuse. Cancellation during snapshot may
leave an incomplete source directory without a manifest; use a fresh workspace.

Adapter contracts: `python -B -m unittest tools/game_loop/test_local_windows.py`.

Adapter follow-up (2026-10-25): seven adapter unittest methods passed, real Unity
retry passed44/44ImpactVfxTests using fresh XML; mirror QA build and strict repeated
builder completed, scene/meta restoration and fixture cleanup verified. Initial
PlayMode attempt produced no XML and was stopped; external mirror fixture logged
an empty-previous-scene exception after saving mirror, despite build success.
Exact receipts and limitations: [adapter checkpoint](../plans/windows-unity-adapter.md).


Workspace/cancellation follow-up:19/19Windows adapter tests pass. Actual Unity
mirror cancelled after fixture-save exits130, waits for the owned Editor and
restores source exactly; same-Library retry builds QA successfully with strict
repeat and fixture-completion markers and no fixture exception. Both attempts
restore scene/meta byte-for-byte and remove the fixture/meta. Prior44/44ImpactVfx
receipt is reused; no new gameplay-suite claim. Exact evidence and cleanup are in
[the adapter checkpoint](../plans/windows-unity-adapter.md#workspace-lock-and-deferred-cancellation-follow-up-2026-10-25).

## Horizontal parry flash (2026-10-27)

- Added72×3logical-pixel procedural quad only to ParryImpact, at existing contact,
  with1px warm-white core/gold tapered halo. Alpha20/40/120ms; old flash/sparks/
  light and gameplay clocks unchanged. Optional2D size/variation lock default off.
- Unity2022.3.25f1 filteredImpactVfxTests45/45PASS/0skip; both directions over24seeds,
  pool reuse, expiry, exact alpha envelope, pause/read hold and old profile consumers.
- Native Edge640×360 real Space:10checks per direction PASS,0exceptions/provider
  writes. Mirrored actor-geometry fixture remains external. Both QA builds and strict
  repeated builder PASS after LF-only snapshot normalization, with checkers retained.
- Direct visual review226/226recorded frames/224pairs across two complete selected
  10.0–14.5s compositor segments: horizontal contact line, gold/white/taper, recovery,
  typing-tray clearance, pause/cleanup. Recorder25fps is not20ms phase evidence;
  precise envelope is covered by Unity tests. No full profile, production or itch
  release. Source/scope/receipts: [checkpoint](../plans/parry-horizontal-flash.md).

## Score/combo inside existing frame (2026-10-27)

- CombatR7Build retains the294x66frame at(333,9), reuses all scene IDs/bindings,
  and puts score/multiplier on one centered row with decay(15,51,264,6) inside.
  ScoreUI_ComboHUD fits long totals while preserving all digits and the1.2x pulse.
- Unity2022.3.25f1/fly-server: focused145616-quick3/3 PASS for combo expiry,
  award-tier pulse and initial geometry; final Cobblestone48/42geometry fixture
  151518-quick1/1 PASS,48cases through20digits and maxpulse. Final fixture replaces
  the initial R7-font geometry evidence. Strict repeated builder145930-webgl PASS;
  all1121original scene IDs retained; only six affected serialized blocks imported.
- Same immutable player native Edge156.0.4314.42 / Playwright1.62.1 / Intel UHD
  D3D11:72/72 PASS in51.406s; all33player files match server hashes. Real input
  covers x1, normal combo, maxx3, parry renewal, expiry/timeout, pause/resume,
  restart and natural Slayer exit. Direct views at1280x720/400x300 confirm text/
  decay remain in frame, HP/tray preserved and popup beside character.
- Zero exceptions/provider writes; fixture/external-network isolation enabled.
  Timer geometry assertion permits existing one-update renewal latency, measured
  as slider.987436 versus remaining2.5s at31.41ms. Scoring/timing/tints/pulse code
  unchanged. No full suite/stress/soak/production/itch.io release.
- Build input882e88e000ed107ebdb465d8659812bf95b7b107eaf20f0938a3d8d52f51aee1;
  final-font test inputb3044b14fb58292db85872307ac09b5cb6b80068ea371c00291e38010d6f4b24.
  Failed driver pilots, source normalization and receipts: [checkpoint](../plans/score-hud-cluster.md).
