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
