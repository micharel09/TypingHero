# Compact Pause, Settings and Top 1000

## Scope and acceptance

Continue `micharel09/compact-tutorial` and preserve its existing tutorial WIP.
Use the existing Cobblestone/coal/copper/ivory controls. Local type sizes only:
heading 30, body 27, actions/table 24. Pause is centered 420x340 with five
36-high actions separated by 8. Settings is centered 420x224, opened from
Pause and Main Menu, with Music/SFX/Back. Top 1000 occupies 840x444 with
176-wide actions and a 288-high viewport containing eight 36-high rows.
Keep result geometry, audio preferences/keys, providers, schemas and virtual list.
Back/Escape restores the prior screen/focus; modal input cannot reach below it.

## Reuse and implementation

- `BlackletterUILayout.cs`: existing menu/results/collection typography pass;
  compact collection geometry and row-prefab dimensions. Global font constants
  and tutorial WIP remain unchanged by this task.
- `CompactMenusBuild.cs`: clone the existing skinned button and copy the existing
  panel image/font into Settings. Persistent callbacks target SettingsModal.
  Reapply local sizes after the common typography pass for repeatable builds.
- `SettingsModal.cs`: temporarily move the same AudioPreferences and sliders;
  preserve and restore geometry/text/visibility. AudioPreferences remains the
  sole owner of MusicVolume/SfxVolume PlayerPrefs and listeners.
- `CompactPauseLayout.cs`: snapshot/restore only compact Pause geometry, labels,
  audio/hint visibility; result presentation retains its authored layout.
- Pause/MainMenu shortcut guards reuse the existing leaderboard modal contract,
  including a closing-frame guard and focus restoration.

## Verification checkpoint

Only scoped native WebGL interaction/layout checks at 1280x720 and 640x360.
Reuse `/data/typinghero-game-loop/task-compact-tutorial` Library/build cache.
No six-lesson tutorial run, full gameplay suite, or itch deployment.

First build 20261021T142702 failed because the inherited scene omitted all 254
Unity stripped records. Restored those records from HEAD (also present in the
previous Unity-generated scene), preserving all other inherited scene content.
This is serialized-reference repair, not a fix for tutorial actor clearance.
Second build 142933 found Settings font/label-height drift on the repeat builder;
added the local post-typography style pass. The final build and scoped acceptance below supersede these failed attempts.

The inherited tutorial card covering the enemy head remains unresolved. Do not
claim tutorial visual acceptance or completion from these menu checks.


## Final scoped acceptance ? 2026-10-21

Unity 2022.3.25f1 on fly-server: final QA build 20261021T144302-webgl PASS,
strict repeated-builder PASS. Input db377c103f65db0b2ceaa5e95166953d1bb9a71d33dd0ddcd773cf7397248a43.
Reused the existing tutorial Library/cache; no gameplay suite or tutorial lesson run.
Final palette reuses CombatR7 DarkFantasy modal-normal/secondary-frame sprites,
including focused variants, and ivory f2e7cf. No new raster assets or audio keys.

Native Edge 155.0.4283.62, Intel UHD/ANGLE D3D11, Playwright 1.58.2:
60/60 checks PASS in 46.107 seconds at 1280x720 and 640x360. Tested opening
Settings from Main Menu and Pause, adjusting both original sliders, Back/Escape,
reopening with the same saved values, persistence across scene return, focus
restoration and blocked R/M/underlying controls. Settings Escape leaves Pause
frozen. Resume and Main Menu work. Classic and Endless retain 1000 fixture rows,
36-high rows / 288-high viewport = eight visible rows, bounded pool 12; real wheel,
difficulty filtering, Refresh, Opponents/Achievements and modal Escape pass.
Inspected the eight final Pause/Settings/table images at both sizes: no text/button
overflow, expected coal/copper/ivory geometry and right-aligned one-line scores.
No JavaScript/managed exceptions and zero provider writes; fixture interception
and external blocking stayed enabled. Audio application continues through the
unchanged AudioPreferences/MusicPlayer/SfxPlayer path; no acoustic measurement.

Two earlier native attempts failed in the driver: sampling before the last held
arrow-key frame (13 checks), then reopening Pause before the existing Resume zoom
finished (44 checks). The final driver observes settled input/zoom; product input,
audio behavior and acceptance thresholds were not changed to pass those attempts.

Final serialized scenes imported from Unity; all initial 1894 scene records remain,
41 new records. Task-only SampleScene commit changes 31 pre-existing records and
adds 41, excluding the inherited tutorial presentation changes. Tutorial source
files, tutorial editor method and its scene changes remain uncommitted WIP.
Result panel geometry remains authored; runtime Pause snapshots restore the
original geometry/text sizes/audio/hints before resume. Source API/provider/schema
and global typography constants remain unchanged. Scoped diff whitespace PASS.

Preview retained at http://127.0.0.1:8238 in a visible native Edge window.
Player/driver/report: E:/CodexTemp/TypingHero/compact-tutorial/menus-build,
`driver/compact_menus.cjs`, `menus-qa/browser.json`; preview helper PID 32016.
Keep these with the existing compact-tutorial worktree/cache until review is over;
then close the task preview and remove task-only menu artifacts. Existing tutorial
preview/materials are not owned by this task. No itch deployment performed.


## Integration and retained resources

Implementation commit `1509028` pushed to `origin/micharel09/compact-tutorial`
and fast-forwarded to `origin/main`; remote SHA and ancestry verified. Only task
changes were committed. The worktree retains exactly the original nine changed
and 28 added tutorial scene records plus the pre-existing tutorial source/docs WIP.
Removed the three task-owned superseded remote build results and intermediate
menu captures/generated staging copies. Kept compact-tutorial Library/cache,
final immutable remote result 20261021T144302-webgl, local menus-build/player,
final QA/failure JSON receipts and live preview helper PID 32016/port 8238.
The earlier task menu preview on port 8237 was closed; original tutorial resources
remain untouched. Keep the worktree/branch and final preview resources until the
user finishes review and the outstanding tutorial actor-clearance work is resolved.


## Pause modal / Resume follow-up (2026-10-22)

Scope: Settings/Top 1000 deactivate the CompactPauseLayout panel while preserving
dungeon, pause/time/audio locks. Close returns audio geometry, activates Pause,
and restores opener focus; the closing frame still consumes shortcuts.
Resume keeps compact geometry and pause until camera exit, hides the canvas
before restoring result geometry, and rejects Resume/Restart/Menu while exiting.
The preceding camera tween is cancelled to handle Resume immediately after Pause.
No scene/GUID, public API, camera parameter, volume or gameplay rule changes.

Isolated Orca worktree `fix-pause-modal-resume`, branch
`micharel09/fix-pause-modal-resume`, based on origin/main `4c33780`.
Parent dirty checkout is preserved. No full suite, stress/soak or itch upload.

Focused Unity receipt `20261022T074625-quick`, input
`d953da04972cf86830623fef125b5549f578afc894f92363a4d1d663db892dc0`:
5/5 PASS, 0 failed/skipped, 0.8417408s, Unity 2022.3.25f1/fly-server.
Two new tests cover PanelReveal/audio/focus and 0.2s camera exit/repeated actions,
compact geometry until hidden, input/time/audio lock and restored Victory layout.
Three existing tests cover hidden selection, modal closing-frame/focus and tutorial
reading. Task-only adapter uses Unity testFilter; repository runner is unchanged.

First QA `20261022T074847-webgl` failed the pre-existing protected-font byte check:
checkout CRLF versus Linux Unity LF in CobblestoneUI64.asset. Download comparison
proves equality after only newline normalization (2217448 vs 2211434 bytes,
6014 CRLFs). Retried snapshot with Git-canonical LF YAML; restored checkout
line endings immediately. Font/checker/assets are unchanged in the task diff.

Final QA `20261022T075105-webgl`, input
`c320c91534c08c8ce08689e40a60a46a1568a9190f2be3a8919722b1c637c23b`:
WebGL/strict repeated-builder PASS. Changed runtime/test files SHA256 match the
integrated checkout 4/4. Merged latest upstream parry tuning without conflicts;
final integrated scoped Unity `20261022T080125-quick`: 5/5 PASS,0fail/skip,
0.8485045s, input36fffcf31b5f40a0c57c8e43a7e43eed594c3358000d6aa83444f1a9a7600298.

Native final `qa-verified/browser.json`:31/31 PASS,58.252276s,1280x720,
Edge155.0.4283.62/Playwright1.58.2/GTX1650Ti ANGLE D3D11. Main-menu Settings;
Pause Settings Back/Escape; Top1000 Close/Escape; opener focus and unchanged audio
values; inactive underlying panel; frozen match; Resume rapid Escape/R/M and
encounter reading retention; Restart; natural Game Over after Resume; return from
results/Pause to MainMenu all pass. Unity covers Victory result geometry too.
Inspected all9final PNGs, including disabled compact menu during Resume and
restored Game Over results; trace records compact PAUSE through locked transition
and absence of label after canvas hide. No result-layout flash observed.
0exceptions,5fixture reads,1intercepted ended-run score write,0production writes.
Fixture interception/external blocking stay on; no bridge gameplay mutations.

Failed driver receipts remain distinct: first25checks timed out on hp0 after actor
was disabled (hp-1); use existing finished/overlay state. Second attempt missed a
menu click and hung closing its already-exited browser; stopped only verified task
Python/Node processes. Final driver holds actual mouse input90ms, unchanged game.
No full suite/stress/soak, production build or itch upload.

Implementation commit `bc962f1`; task resources/evidence root:
`E:/CodexTemp/TypingHero/pause-modal-resume/`. Final QA player,9PNGs/video/trace,
Unity XML/snapshot receipts and reproducible driver are retained for review.
Remote final QA result/source input: `/data/typinghero-game-loop/results/20261022T075105-webgl/`.
Keep QA/evidence until review ends; cleanup then requires a separately reviewed
scope that preserves anything still needed. Automatic approval review rejected
recursive cleanup including QA directories/player.tar.gz because it could remove
required review evidence. Used a narrower safe cleanup: removed only the temporary
font comparison asset and scoped/build execution adapters. All QA directories,
archives, failed-run diagnostics and remote results remain as review evidence;
shared cache and pre-existing resources stay untouched. Final extracted QA player
was verified byte-identical to all32archive members before cleanup was considered.
The QA listener8254 is closed and no live preview is retained.

Remote integration verified: task branch pushed and HEAD `c8e74a1` fast-forwarded
`origin/main` from4e62ea2; fetched remote SHA equals local HEAD and implementation
`bc962f1` is an ancestor. This final receipt is a documentation-only follow-up;
verify its remote HEAD before removing the clean task worktree/local branch.
Remote task branch stays retained. No parent checkout/WIP modification.


## Itch release follow-up (2026-10-22; published)

User authorized upload after Pause acceptance. Release source `db1936d`, from
verified origin/main, in isolated `release-pause-itch` / `micharel09/release-pause-itch`.
Parent dirty checkout/WIP is excluded. Existing scoped Unity5/5 and native31/31
pause results reused; no runtime changes or full suite/stress/soak.

Unity2022.3.25f1/fly-server production `20261022T082936-production` PASS,
input
`6dd75a9b32f7e6baefc5b85f8fec6a95d9df83272dc6b1a115ff7aa0654a85a8`.
Snapshot adapter only normalizes YAML CRLF to canonical LF; no checker/asset edits.
Production runs the normal builder once; the strict repeat was performed by the
preceding Pause QA build, not by this production invocation.
qa=false; ASCII/UTF16LE typingHeroQA/GameLoopProbe/TYPINGHERO_AUTOMATION exclusion
across all JS/WASM/data PASS. ZIP32members/18,777,511bytes, SHA256
`7d30660e33cf70adb76167a28a6d5afcd32ff0b88f71d66d71a059afb4d157a4`,CRC/local transfer PASS.
Original HTML/native Edge155: Guest/Play, tutorial reading, Pause/Settings/Escape,
Resume with reading retained and Pause/M return inspected; bridge undefined,
0exceptions/provider reads/writes, fixture/external blocking enabled. No audio or
performance measurement. Initial interactive runner had closed stdin; stopped only
its verified task process tree and reran in a PTY, without touching the game.

Published https://micharel09.itch.io/clackblade, game3857509, playable19943928,
`CLACKBLADE-WebGL-Pause-20261022-db1936d.zip`. BrowserSkill native upload succeeded,
new playable selected, previous19915823 hidden/retained, Save returned Saved.
38non-upload fields,5page images and every other upload flag are unchanged.
No page text/gallery/pricing/tag edits, devlog/social posts or provider changes.
Actual public Run game iframe selects19943928 and renders the new dungeon menu.
All31non-HTML anonymous CDN SHA256s match ZIP; index adds only standard itch
htmlgame.js. Direct anonymous Edge player also renders the same menu, bridge
undefined,0exceptions/writes. Raw anonymous HTTP access to the root page returned
Cloudflare403; no anonymous-root HTTP PASS claimed. Browser/public-player checks
above establish the published runtime; no challenge bypass.

Canonical receipt: [pause-release-20261022.json](../../marketing/itch-io/pause-release-20261022.json).
Rollback: select retained19915823, hide19943928 and Save; other settings unchanged.
BrowserSkill session stopped; task smoke browser/server8256 closed. Earlier idle
BrowserSkill sessions were already absent and were not reused/guessed.
Retain final ZIP/player, smoke images/reports, immutable source snapshot and CDN/
form verification receipts at `E:/CodexTemp/TypingHero/pause-itch-release/` and
`/data/typinghero-game-loop/results/20261022T082936-production/` until release review
ends. Shared cache and previous artifacts stay untouched. The clean release
worktree/local branch can be removed after these documentation commits are pushed,
merged and verified on origin/main; retain the remote release branch.


## Pause and redundant guidance cleanup ? 2026-10-24

Approved scope: remove the enclosing Pause face/rim, retain its five actions;
keep the single current tutorial, remove How to Play and the initial Fight gate;
remove DUEL x/6 from HUD and between-encounter copy, retain Continue/results.
No combat/animation/provider/save-schema changes or itch publication.

Implementation: CompactPauseLayout snapshots/restores the shared panel Image.enabled
alongside geometry. Settings/results keep their own authored surfaces. CoreMatchBuild
removes the legacy help canvas/button/component; menu navigation contains four actions.
All layout passes stop addressing retired help/round nodes. CombatR7Build removes both
round label/backplate; the QA observer checks the actual hierarchy without retired fields.
EndlessDuel starts after initialization, preserving DungeonSession transition input
suppression; Continue still advances only a completed encounter. The result shows the
next opponent name without tactical hints or duel numbering. Tutorial guide/template,
replay, completion and persisted preferences remain intact.

Worktree: C:/Users/ADMIN/orca/workspaces/TypingHero/clean-pause-guidance,
branch micharel09/clean-pause-guidance, based on origin/main 8a7b498.
Evidence/helpers: E:/CodexTemp/TypingHero/clean-pause-guidance.
Verification: Unity2022.3.25f1/fly-server result20261023T174743-webgl,
input d90abc079611723cb0556589b74770f96563c3e29dee9a94cc796a4ca6be0922:
12/12 filtered PlayMode PASS, zero failures/skips, 10.564191s. Strict repeated builder
and QA WebGL PASS; all2231 input files match before generated scene import. Initial
builder caught a retired Round node in ApplyCompactStatus; corrected before acceptance.
Generated scene records match final checkout after ignoring ordering/trailing whitespace;
37 legacy help records and8 round records removed, zero new records, all other IDs kept.
Original record ordering retained to avoid unrelated Unity serialization churn.

Native Edge/ANGLE D3D11, Playwright1.58.2,1280x720:22/22 PASS with actual keyboard/mouse,
read-only probe and fixture interception. Tutorial first-run/replay/Start Battle,
pause during tutorial/combat, Resume, Settings/R/M guards/Back, Top1000/Escape,
real typing defeat, Continue to next opponent, Restart, death/results and Main Menu.
Nine screenshots directly inspected: menu,tutorial,combat,pause,pause-tutorial,
pause-restored,settings,continue,defeat. Pause shows title/buttons without enclosing
face/rim; Settings/results retain their surfaces; no Help/DUEL x/6/initial Fight gate.
Zero JS/managed exceptions or production writes. No full balance, sprite or performance
suite; actual Editor EnterPlayMode tests updated but not rerun (direct-entry PlayMode
coverage passes). No merge-conflict cause established for the original visible panel.


Preview retained: http://127.0.0.1:8291/ with its player, fixture service worker,
source snapshot,12-test XML,22-check native report and nine screenshots under
E:/CodexTemp/TypingHero/clean-pause-guidance. Keep until user review is complete;
then stop only its preview host and remove that task artifact directory. Shared
Unity cache/source and pre-existing previews remain untouched.


## Compact Game Over / Victory ? 2026-10-24

User requested review and shrink of the result card after the Pause cleanup.
Prior results used864x540 with48-unit summary text, embedded volume sliders and
full-width actions. New authored result card600x366 at(180,87) on960x540, title30,
summary24 with176-high wrap area, two268x36 action columns: Restart/Main Menu,
Top1000/Settings. Audio preferences use the existing Settings modal. Redundant
R/M footer and restart keycap are hidden; keyboard shortcuts remain owned by
GamePauseController. Same result bounds for Game Over and Victory. Pause continues
its unframed420x340 runtime layout and restores the compact result geometry.

Base origin/main b0534d5; task branch micharel09/compact-results. Scoped checks:
two real-scene result layout/long-statistics/Settings round-trip tests plus existing
pause/modal restoration cases, strict repeated builder, and one1280x720 native
visual/real-input flow for defeat and actual six-opponent victory. No timing,
progression, damage, save/provider/public API, asset GUID or animation changes.
Artifacts/helpers: E:/CodexTemp/TypingHero/compact-results.
Unity2022.3.25f1/fly-server result20261023T184327-webgl, input
43e1f3b6a5dafd1609a3f9e3ad48e569b9c310191a56f2bf16869970d18d730a:
4/4 filtered PlayMode PASS,0failed/skipped,2.6974057s; strict repeated builder PASS.
Tests use both real result callbacks, long five-line summaries, glyph/card bounds,
four active actions/no Resume, Settings/Back/hidden sliders, and Pause restoration.
Initial run183939 passed3/4 but Victory teardown reused the first test cleanup scene
name; fixed only the test scene name and reran. No product layout correction needed.
QA WebGL/strict repeated builder PASS. Native image review still pending; shared runner contention accounted for
most waiting before the scoped tests. No other task processes were stopped.


Final native UI review: Edge155/ANGLE D3D11/NVIDIA GTX1650Ti, Playwright1.58.2,
1280x720, native-complete/browser.json PASS191 checks, zero JS/managed exceptions.
Real mouse/keyboard: natural death, result Settings/R/M guards/Back, Top1000/Escape,
Restart, unframed Pause/Resume, six actual Very Easy opponent defeats and Victory,
Victory Settings/Escape restoration, Main Menu. Six full screenshots directly
inspected: game-over, game-over-restored, result-settings, pause-unchanged, victory,
victory-restored. Both result titles, five summary lines and four actions fit;
embedded sliders/key hints absent. Fixture interception retained; no production writes.
Earlier native pilots are not overall PASS: one assumed HP stayed0 after corpse
cleanup (probe correctly returns-1), then conservative typing pilots lost before
Victory. Corrected only the real-input driver (allow legal Slayer/Parry typing,
acknowledge held character input); no game rules/timing/assets were changed to win.

Integrated newer origin/main cb54e5f authored actor shadows without conflicts.
Result UI code and all18 modified scene records are unchanged from the reviewed
QA player43e1f3...; UGUI remains separate from the actor shadow material.
Integrated Quick20261023T185318/input
d1188082e78510a576d2d56a5e03bc27cf92038a6981b304e7b57a903811bc7f:
4/4 same scoped PlayMode tests PASS,0fail/skip,2.887987s; strict repeat builder PASS.
The complete integrated SampleScene matches Unity's generated records, ignoring
record order/trailing whitespace only. No actor-shadow visual certification is
claimed by this UI task; its existing main receipts remain authoritative.

UI review page: http://127.0.0.1:8293/review.html (links to the UI QA player).
Retain E:/CodexTemp/TypingHero/compact-results player, screenshots and receipts
until user review is complete; then stop only its preview host and remove the task
artifact directory. This UI player predates the concurrent shadow merge; its UI
records match the integrated source exactly. No itch publication.


## Result hierarchy revision - 2026-10-24

User rejected dense centered summary in both result screenshots. Approved layout:
600x366 card; red/gold outcome title, short difficulty/outcome context without
internal Attack1 state, prominent score40, four label/value stats in two columns,
separate NEW BEST or historical best line, Restart primary / Main Menu secondary,
smaller Top1000 and Settings footer. Existing pixel font/palette and handlers stay.
No combat/sprite/tutorial/Continue redesign. Read-only best metadata feeds the new
CombatResultView directly; diagnostic MatchResult string/reference retained hidden
for existing QA consumers. Pause hides/restores the result group.

Reference checklist: win/loss heading; grouped stats and label/value alignment;
record placement; primary/secondary actions; complete result entry/settling/exit.
Directly inspected Brotato win screenshot (Steam guide2896126855, image9E244DD2),
loss screenshot https://note.com/osama/n/nd5034b2948d6 and Hades screenshot
https://sixohthree.com/media/2020/11/hades.jpg. Brotato motivates left labels/right
values and bottom actions; Hades motivates grouping records with result statistics.
No assets copied. Motion reference selected BEFORE exhaustive review:
https://www.youtube.com/watch?v=PvU_HBp2D48, 01:14:53?01:15:05 (4493-4505s):
final combat, win banner, full summary entry/settling, Return to Main Menu/settling.
Original-rate60fps stream requested; 30fps tail used only to locate this segment,
not counted as review. Frame/transition inspection and loss motion still pending.
Research artifacts: E:/CodexTemp/TypingHero/result-layout-research.
Implementation/check artifacts: E:/CodexTemp/TypingHero/result-layout.

Reference review completed for the downloaded60fps stream:900/900frames and
899/899consecutive transitions directly inspected in30ordered sheets, including
held/repeated frames. Stream-copy request4493-4505s retained approximately3s of
keyframe preroll (15s local clip); exact source alignment of frame0 is not certified.
Sheet timestamps initially assumed no preroll; frame numbers are authoritative.
Frames1-297 final combat;298-330 RUN WON letter reveal;331-478 banner hold and
coin collection;479-867 grouped result panel with tooltip/button hover;
868-900 return and settled main menu. No result motion copied. Static win/loss
layout and Hades grouping inspected; loss-motion/Hades-motion comparison remains
unverified and is not a timing target for this UI-only revision.


Initial Unity result20261024T015153-webgl / inputf06e458b: strict repeated builder
PASS. Five selected PlayMode cases ran:3pass,2result-layout failures. Both detected
DefeatedLabel ink atx=-1.25 against the left boundary tolerance; added2units of
horizontal text padding in the new result group, leaving test bounds unchanged.
Endless terminal save/queue idempotency, Pause modal/focus and Resume sequencing
passed. Rerun on eb86454c is queued. Long shared-runner waits preserved other tasks;
only the task's cancelled lock-waiter1637770 was stopped after verifying its
command marker and locks_lock_inode_wait state. No other task was interrupted.


Body layout verification020755 / inputeb86454c:5/5scoped PlayMode PASS in3.1103252s,
0fail/skip; strict repeat builder and QA WebGL PASS. New text uses2-unit horizontal
padding for negative font bearings. Scene changes remain within the result subtree:
10existing records updated,51new records for the12labels/group/presenter; all old
IDs retained and all semantic records match Unity serialization.

Additional primary-action check022158/input97e12566 ran5cases:3pass,2fail because
the attempted gold sprite still saved as the common secondary frame, including
with explicit SerializedObject/SetDirty. Removed that styling attempt. Final
primary emphasis uses authored width304 for Restart versus232 for Main Menu,
retaining the common skin and existing hover/focus states; footer controls180x32.
The check now proves that intended size hierarchy survives serialization and
retains the existing card/glyph/action/modal checks. This is a static visual choice,
not a runtime fallback or a claimed fix to shared skin behavior. Final rerun
inputd11258b8 passed; final receipt follows. No runtime source changed since the verified body build.


Final UI build023638 / input
`d11258b89f9b209e18a3c8af7cfb14a775f0907651e484ec928da267f9dc1da2`:
5/5selected Unity2022.3.25f1 PlayMode cases PASS,0failed/skipped,3.0599973s;
strict repeated builder and QA WebGL PASS. Committed scene keeps all old IDs,
changes10existing UI records/adds51, and matches every generated semantic record;
MainMenu scene unchanged. No sprite/animation/public-provider contract changes.

Native Edge155.0.4283.62 / Playwright1.58.2 / Intel UHD Graphics ANGLE D3D11,
1280x720:203checks PASS in124.9356567s,0JS/managed exceptions,2intercepted
Endless fixture writes and0production writes. Actual input: no-word natural death,
all four result actions, Settings Back/Escape/shortcut guards, leaderboard Escape,
Restart, unframed Pause/Resume, real six-opponent victory and Main Menu return.
Victory3629points,6defeated,63.9s,100.0%accuracy,10parries,40HP; NEW BEST shown
once without repeating score/count. Six complete screenshots directly inspected:
game-over,game-over-restored,result-settings,pause-unchanged,victory,victory-restored.
All text/buttons fit; labels/values align in two columns; score dominant and
Restart visibly wider; no internal Attack1 or legacy multiline summary visible.
The helper's legacy scenario field says tutorial; the dedicated compact-results
module and captured per-image states establish the actual scope above.

UI review http://127.0.0.1:8295/review.html, playable QA at root. Artifacts and
receipts E:/CodexTemp/TypingHero/result-layout; reference research in sibling
result-layout-research. Keep until user review completes, then stop only preview
terminalterm_d3ab3d3f-1825-462d-86f1-2c9c19c52b9f and remove these task artifacts.
Merged newer cue/torch main4e7ccc6; this player certifies
result UI only, not the newer main world/cue appearance. No itch publication.


Integrated newer main4e7ccc6 (Space cue HUD and torch profiles) via merge27639ad.
Only a verification-document append conflicted; retained both evidence sections.
All61result UI records and result UI code remain identical to the native-reviewed
player. Integrated Quick030507 / input
`1b30dee76923dbebf6de5590e080489913fd5e569c9746f69169b1f0aa5f59a4`:
5/5same scoped PlayMode cases PASS,0fail/skip,3.3917055s; strict repeat PASS.
Entire merged SampleScene and MainMenu match generated semantic records exactly.
Native203-check UI evidence is reused for the unchanged UI; newer cue/torch visuals
retain their own main receipts. UI QA preview is intentionally scoped to results;
no new production/world/performance certification or itch upload.


## Result-only HUD suppression (2026-10-24)

User approved hiding the surrounding combat HUD on Game Over/Victory. Keep the
world/dimmer and compact result card, and preserve ordinary Pause presentation.
GamePauseController hides thirteen explicitly bound CanvasGroups once when a
terminal overlay opens; logic objects stay active. Visibility remains latched
through Settings/Top1000 and camera exit, then scene reload supplies fresh authored
groups. No per-frame work, public API, scoring/input or provider changes.
CombatR7Build binds the existing visual nodes after ParryCueBuild; no reparenting
or GUID replacement. Initial implementation is awaiting scoped Unity and native
checks; do not treat the earlier result-layout receipts as proof of this change.
Task evidence: E:/CodexTemp/TypingHero/result-hide-hud.

Initial input401cbca3 / run034053: strict builder completed, but PlayMode3/5
passed and both result cases rejected null HUD references. Serialized scene
contained thirteen null entries and no added CanvasGroups. Replaced the native
Unity-object null-coalescing expression with its boolean existence check, and
explicitly mark/save the authored scene. Tests and required reference count remain
unchanged. Intermediate346a646b waiter was stopped only after verifying the
task-specific helper marker and locks_lock_inode_wait; it executed no Unity work.
Final candidatea86a182f is awaiting checks. All other runner jobs were preserved.

Candidatea86a182f / run035006 serialized all thirteen valid groups and passed
strict repeat; result cases then exposed an incorrect new test wait:250ms versus
the authored295ms Resume camera exit. The result call was correctly rejected
while resuming, so HUD remained visible. Replaced that fixed test delay with a
bounded wait for IsPaused=false plus an explicit result-visible assertion. No
product transition timing or HUD acceptance was relaxed. Candidate4a9a713b pending.

Final Unity candidate4a9a713b / run20261024T035514-webgl:5/5selected PlayMode
cases PASS,0failed/skipped,3.7247909s, and strict repeated builder PASS. Native
image/integration review remains pending while the QA player compiles.

Final4a9a713b QA WebGL PASS;33downloaded player hashes match the immutable remote
artifact. Both authored scenes match every generated semantic record. SampleScene
preserves all old IDs, changing15records/adding14 (thirteen CanvasGroups and one
stripped prefab identity). Native Edge155.0.4283.62/Playwright1.58.2/Intel UHD ANGLE
D3D11 at1280x720:229checks PASS in121.9313421s,0exceptions,2intercepted Endless
fixture writes. Real no-input death, result Settings/Top1000, Restart, Pause/Resume,
six-opponent victory, result restore, Menu and new gameplay covered. Seven full
images directly inspected: game-over, game-over-restored, result-settings,
pause-unchanged, victory, victory-restored, menu-new-game. HUD is absent at results
and result Settings, restored for gameplay and retained in ordinary Pause. Victory
3814points/6defeated/62.4s/99.7%/9parries/40HP matches the captured run.

Native startup initially lacked copied toolchain/helper metadata (no game started).
First actual native attempt stopped at3checks because its new assertion also
matched legacy zero-scale PlayerHP controls exposed by the read-only probe.
Corrected the external driver to identify the unique PauseIconR7; all thirteen
groups remain asserted hidden/noninteractive in Unity and all seven images were
inspected directly. Failed attempt preserved separately as native-initial-probe.
No product change or threshold relaxation followed this probe-identification fix.

Evidence retained at E:/CodexTemp/TypingHero/result-hide-hud until user review is
complete; then remove that task artifact directory. Native contexts/server exited.
No performance measurement, production build, sprite-motion certification or itch
publication is claimed. Native player precedes newer main parry/cue revisions;
those retain their own upstream receipts. Integrated UI checks follow separately.

Integrated main9c48f5d through merge51c9e7b, retaining upstream parry recovery and
enemy-head Space warning. Only the verification append conflicted; both receipts
were retained. GamePauseController, HUD builder/test code and both full UI scenes
are byte-identical to the native-reviewed UI commit10b7f0f. Integrated Quick042228
/inputb9eacfd1:5/5scoped PlayMode PASS,0failed/skipped,3.6638901s, strict
repeat PASS; both complete scenes match every generated semantic record. Native
229-check evidence is reused for unchanged HUD behavior. Newer upstream parry/cue
visuals retain their own receipts; this task adds no sprite review certification.
