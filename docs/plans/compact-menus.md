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
