# Typing Hero pixel UI refresh

## Accepted intent

Implement the user's 2026-10-16 plan: replace retro computer framing with dark pixel UI across menu, fixed HUD, separate Top 1000 sidebar and pause/game-over/demo-completion panels. Preserve characters, arena, combat, scoring, identity rules and Firebase contracts. Use free Crusenho Flat assets (CC BY 4.0 credit), Kenney Input Prompts Pixel, and VT323 with verified Vietnamese glyph coverage.

Reference geometry: 960×540; margins 16, gap 8, leaderboard width 232. Fixed HUD identifies owners of HP/stamina. Typing rail and timer stay above combatants. Shared UGUI prefabs; camera viewport and Slayer effects constrained to combat area. Preserve `LeaderboardRowView.Bind(rank, name, score)` and add a separate rank binding. Build/import/serialize with cloud Unity; deliver a verified WebGL ZIP.

## Milestones and acceptance

1. Preview menu/gameplay/pause/small-window composition; one user review before scene edits, as explicitly requested in the plan. Source: `design/layouts/pixel-ui/web/`.
2. Import licensed assets/font; build shared panel/button/bar/row prefabs and update MainMenu/SampleScene through Unity serialization. Preserve existing controller actions and business APIs.
3. Run Unity/WebGL QA at 960×540, 1280×720, 1920×1080, itch-style embed and fullscreen. Exercise name validation/saved/Vietnamese identity/Enter/disabled Play; correct/wrong typing/timeouts/long combo/Slayer; pause/resume/focus/restart/game-over/demo end with no input leakage; empty/long-name/1000-row leaderboard and retained scroll after 8-second refresh.
4. Review diff and deliver verified ZIP. Successful compilation alone does not satisfy runtime acceptance.

## Checkpoint — 2026-10-16

- Repository: `C:/Users/ADMIN/orca/TypingHero`, origin `micharel09/TypingHero`; working tree was clean on entry. Existing `leaderboard-top1000.md` describes an earlier completed task, not this UI refresh.
- Unity version: 2022.3.25f1. Existing `.github/workflows/build-webgl.yml` uses game-ci/unity-builder and repository Unity secrets; GitHub CLI authentication is available. No new build service or local Unity installation is needed. Current changes have not been pushed or built.
- UI implementation owners inspected: MainMenuUI, PlayerIdentity, GamePauseController, TypingManager, HealthBarUI, StaminaBarUI, ScoreUI_ComboHUD, LeaderboardRowView and LeaderboardUIBinder. Separate rank binding is absent; score/combo currently uses positional following. Identity validation/normalization must be preserved.
- Draft wireframe variants created for menu, gameplay, pause at 960×540 and gameplay at 640×360. Contract distinguishes illustrative geometry/content from final artwork. No scenes/runtime code changed pending the preview review explicitly required by the plan.
- Required BrowserSkill Edge instance `61a46c05` is not connected. `bsk browsers` lists only Chrome instances `9c92c27f` and `857ab518`. Asked user to connect the specified Edge instance; do not substitute profiles. WebGL QA remains unverified.
- Next: render/inspect previews, obtain the requested layout review, then implement assets/prefabs/scenes and cloud build. Asset URLs verified as sources; downloaded/imported assets and font atlas remain pending.
- Preview validation: helper `validate` and `render` pass. Opened all four rendered HTML files in headless Chromium through Playwright and visually inspected screenshots of `#wf-root`; measured roots are exactly 960×540 (menu/gameplay/pause) and 640×360 (small). Fixed initial renderer minimum-height overflow before capturing the current revision. These are monochrome structure previews, not final pixel skins or Unity runtime evidence.
- Helper `check` reaches the approval gate and reports `approval.status: bundle chưa duyệt (draft)`; do not mark it approved without user review. All screenshot receipts match the current rendered revision. Scene/code/build acceptance remains pending.
- Temporary rendering tools remain at `E:/CodexTemp/TypingHero/pixel-ui/tools` for the next preview/QA iteration. Product layout previews/PNGs are intentional design sources under `design/layouts/pixel-ui/web/`, not Unity QA captures.

## Implementation checkpoint — user approved layout

- User approved the four previews and requested immediate pixel skin implementation: “oke áp skin pixel vào cho hợp, triển khai liền đi”. Bundle approval recorded; helper `check` passes.
- Crusenho free Flat archive downloaded through Edge; `frame.png` is the unmodified Frame01a sprite. Kenney Enter/R sprites and license included. VT323 source cmap contains all Vietnamese U+1EA0–U+1EF9 and required punctuation. The Unity atlas still needs runtime verification.
- Editor migration under `Assets/Editor/PixelUI/` constructs shared panel/button/stat-bar/leaderboard-row prefabs and updates both scenes through Unity APIs. Build preprocessing exports `build/PixelUI/PixelUI.unitypackage` so cloud-serialized scene/assets can be restored locally. Old canvases are disabled; their controllers and referenced objects remain alive.
- Runtime changes: separate optional rank binding preserving Bind API; empty leaderboard label; overlay button wiring; fixed HUD and combat viewport camera; pause/ending input guard for typing/parry and paused combo decay. Menu submits via TMP onSubmit, with a scene-loading guard; leaving the input field no longer starts gameplay accidentally.
- Existing concurrent game-loop changes (runtime assembly/test/automation files and ProcessInput extraction) are preserved and excluded from the UI cloud snapshot. The snapshot stages an equivalent guard in original HandleTyping; local guard remains in the new ProcessInput path, preserving that task's tests.
- `python tools/game_loop/run.py doctor` failed with licensePresent=false and missing Unity image (setup requires 35 GiB; 21.4 GiB available). No runner setup attempted. Use the existing authenticated GitHub Unity builder for this task.
- Edge instance `61a46c05` is connected; UI task session `pfxo`. Asset downloading completed. WebGL runtime checks/build are pending.
- Build snapshots are on task-owned branches `codex/pixel-ui` and `codex/pixel-ui-qa`; the working checkout remains on `main` with concurrent game-loop edits preserved. QA snapshot reuses only the existing conditional read-only GameLoopProbe/jslib, not the infrastructure task's other changes. No publication to itch.io is authorized or performed.
- First cloud run `38843135999` failed with C# ambiguity between static Font/Text fields and Unity types; renamed those fields to Typeface/Foreground. Also replaced the old combo controller/label sharing one GameObject with a new controller and child label. Restarted production run `38843608472` and QA run `38843611633` (artifacts/runtime pending).
- Additional source review: center the fixed 960×540 reference frame inside an expanded Canvas for non-16:9 embeds, with camera alignment read from the actual combat RectTransform. Align leaderboard header columns to rows. These later edits and distributable credit copying must be included in the final build snapshot; the currently running builds precede those edits.
- UI browser session is now `qqyc` on the required Edge; previous asset session is closed. Local QA server is process session `13787` at http://127.0.0.1:8146. Browser debug capture `dbc9e02fdcf7d` has task-local mock rules for Firebase reads/writes (fixtures, no production submissions). The one full 1000-row rule exceeded the CLI's 80 KiB option limit; a compact fixture now fits and is installed.
- Temporary QA scripts and all forthcoming runtime captures/results stay under `E:/CodexTemp/TypingHero/pixel-ui/`. Final Unity package restoration and ZIP delivery remain pending runtime acceptance.
- Production run `38843608472` succeeded (10m49s), including a Unity-serialized package. Edge runtime review at 960×540 found invisible input text: the old TMP_InputField global font/point size and text rect were overriding child styling. Fixed input.fontAsset/pointSize, reset text viewport/child rects, and restored opaque selectable colors. Do not deliver this first ZIP.
- Edge `svlm`, capture `de5dadbe9ffcd`: verified actual gameplay/pause UI at 960×540, 1280×720, 1920×1080 and 640×360; actors and typing are confined to combat, sidebar stays separate. Selected the actual scrollbar through a screenshot-bound click and used native ArrowDown to reach ranks 987–1000. Rank 1000 remained visible after repeated eight-second mocked refreshes (42+ root rule hits, no rule failures). Game over reached through normal idle gameplay; Resume is hidden and Restart/Main Menu remain visible. These checks precede the input/embed fixes.
- QA run `38843611633` built successfully in Unity but GameCI rejected the custom adapter's output because it lacked the documented literal `Build succeeded!` signal. Added that signal only after BuildReport reports Succeeded in the QA-only adapter (no suppressed failure). Subsequent runs `38845113500`/`38845116551` failed compilation due to a prefab-local `root` variable shadowing the new design root; renamed it to `prefabRoot`.
- Current builds: production `38845448744`, QA `38845451555`. Both include input fixes, centered reference root, aligned headers and distributable credits. Await actual results before restoring serialized assets or delivering ZIP. Original package stays in `serialized-first/` for inspection only; do not import its older layout over the final implementation.
- The above Unity builds succeeded, but the packaging step failed because the root-owned container output directory rejected the host runner's credit copy. Use sudo only for copying credits into this task-owned build output. Artifact upload is now conditioned on the Unity step succeeding, so packaging errors remain visible without losing a successfully compiled artifact.
- Runtime blocker reproduced on the first ZIP in both Edge and isolated Chrome: 1000 visible row objects + resizing through 1280/1920/640/960 + game over + return to menu triggers native WASM memory access out of bounds. Controls: 20 rows + resize passes, 1000 rows without resize passes, 20 rows without resize passes. The underlying Unity native failure is not yet identified; do not claim it is fixed until the exact case passes.
- Replaced full allocation in the new UI with a visible-row pool (viewport + two rows overscan each side); retain all 1000 records and full content height. Old scene consumers default to the existing full-layout behavior. Refresh rebinds the current window and preserves content position; rank binding remains unchanged. The new scene sets virtualizeRows=true and removes the conflicting layout fitter/group. QA observer reports _rows.Count separately from view pool.Count; bounded pool expectation replaces the implementation-specific 1000-view expectation in this task's temporary test.
- Builds with the pool fix and packaging permissions: production `38846652619`, QA `38846656042`. Final package and ZIP remain pending acceptance. Native browser tests must wait for scene/input activation frames; the first headless lifecycle attempt that never left the menu was rejected as invalid evidence and rerun with visible before/after screenshots.

## Final verification ? 2026-10-16

- Implementation complete: Unity-serialized MainMenu/SampleScene, shared panel/button/stat-bar/leaderboard-row/floating-score prefabs and VT323 atlas restored from cloud package. The final package contains 18 asset paths; the earlier scenes-only export was corrected by removing trailing slashes from ExportPackage folder paths. Both scenes have unique serialized object IDs. All 90 Vietnamese U+1EA0?U+1EF9 glyphs are present in the serialized font atlas; Vietnamese names were also entered and rendered in native WebGL.
- Successful production build: GitHub Actions run 38846652619, snapshot 988e227; QA build: 38846656042, snapshot bd54b30. Complete serialization-only export: 38848215412, snapshot f9811a4. The export-only change did not alter runtime or UI inputs, so the verified production artifact is reused.
- Actual WebGL keyboard/mouse checks passed: invalid/disabled Play, Enter submission, saved and Vietnamese names, correct/wrong typing, timeout without score, combo/Slayer, pause/input blocking, Resume/Restart/Main Menu, game over and demo completion. Demo completion was reached through real typing/parry input, without gameplay setters (enemy 2000 HP to zero; player survived). Earlier combat-driver attempts failed due to parry timing; the corrected driver used observed animation timing and reached completion.
- Leaderboard: full 1000-record fixture with a bounded 19-view pool, long-name ellipsis, native wheel movement and scrollbar drag to rank 1000, eight-second refresh retaining position, empty state with zero views. Fixture requests and score writes were intercepted; no production data was submitted. Rank/name/score binding contract remains unchanged.
- Exact native WASM regression now passes: 1000 rows, resize 960?1280?1920?640?960, then return to menu from pause and from actual game over. Both production tests report no runtime errors. Earlier full allocation failed this sequence; bounded virtualization is the verified fix, without claiming a Unity-internal root cause.
- Layout reviewed at 960?540, 1280?720, 1920?1080, 640?360 and a 960?600 non-16:9 frame. A real 960?540 iframe in a 1280?720 wrapper and native fullscreen both render correctly. Clicking outside the iframe causes actual document focus loss and pause after refocusing; synthetic/headless tab-focus attempts were rejected as evidence. The live itch.io host itself was not modified or tested.
- Slayer visual check passed: sidebar pixels remain identical before/during Slayer; effects stay inside combat. Pause/ending screens retain the separate sidebar; demo ending hides Resume.
- Final BrowserSkill smoke used required Edge instance 61a46c05, session bmuf: visible menu, native name entry/Enter, gameplay, pause and scrollbar keyboard navigation to rank 1000. Production has no window.typingHeroQA. Earlier Edge checks cover all requested desktop dimensions; final exact resize/lifecycle regression also passed in isolated Chromium.
- Offline Unity doctor remains unavailable (license/image/storage prerequisites); native cloud Unity builds and real WebGL interaction checks provide runtime evidence. No claim is made that the unrelated offline suite passed. UI test drivers that stopped early are not reported as entire-suite successes; independent passing checks above are the evidence.
- Deliverable: C:/Users/ADMIN/Downloads/TypingHero-WebGL-pixel-ui.zip, 21 files, index.html at root, CREDITS.txt and three licenses included; ZIP CRC verification passes. This is the native production artifact, without the QA-only automation adapter. No itch.io publication performed.
- Concurrent game-loop changes remain preserved and excluded from staged UI changes. In TypingManager the index holds only the pause guard in the original HandleTyping; the worktree retains the other task's ProcessInput extraction with the equivalent guard. Build snapshot branches codex/pixel-ui and codex/pixel-ui-qa are retained until these source changes are integrated into the target branch; they are reproducible build references, not additional active worktrees.
- Final source review and git diff --check pass. Temporary captures/drivers/downloads are removed after recording these conclusions; approved product wireframes remain in design/layouts/pixel-ui/web/.

## Published follow-up - 2026-10-16

- User explicitly requested itch.io update. Uploaded the verified 12,606,816-byte TypingHero-WebGL-pixel-ui.zip to the existing micharel09/typing-hero project through authenticated Edge 61a46c05 (BrowserSkill session ykrh).
- Selected the new ZIP as browser-playable and saved. The previous top1000-scroll upload is hidden, retained for rollback. Public visibility, pricing, description and other page settings remain unchanged.
- Public page confirms the new upload; actual itch.io iframe renders the new pixel menu and credits. Hosted build URL is https://html-classic.itch.zone/html/19793957/index.html . CREDITS.txt and Build/WebGL.loader.js SHA-256 match the verified local ZIP byte for byte.
- Native Enter starts gameplay on the hosted build; Escape opens the new pause panel, and real Firebase leaderboard data renders in the separate sidebar. Write methods were mocked during the smoke test to avoid submitting a test score. Browser debug console inspection reported zero errors. Temporary release captures removed after this checkpoint; BrowserSkill session closed.
- Published game: https://micharel09.itch.io/typing-hero .

## Dark Ages migration — 2026-10-16 (active)

Approved source: Dark Ages UI v1.0 by Hypnobius, downloaded ZIP in Downloads.
Preserve 960×540 geometry, VT323/Kenney, characters/arena, controllers/listeners,
leaderboard columns/virtualization/scroll, all typing/combat/scoring and 40/850 HP.
Import original tilesheet/license/README to Assets/Art/UI/DarkAges; Unity role slices,
Point/no mipmaps/uncompressed, 9-slice. Apply in place after CoreMatchBuild, including
inactive help/pause/results. No dependencies, schema/API changes or publication.

Acceptance remaining: repeat builder preserves serialized identity/references/listeners;
Quick + WebGL flow; visual menu/help/HUD/typing/pause/results/button/bar checks at
960×540,1280×720,1920×1080,640×360 plus iframe/fullscreen; 1,000-row fixture scroll.
Production ZIP excludes QA bridge and includes credits/licenses with valid CRC.
No stress/soak. Retain core-match ZIP for comparison.

Checkpoint: role importer + post-core skin pass implemented; Quick 26/26 passed
(snapshot f9a862e4f6d9a2f45a7d63c5d27eac4c5b7ba377c76c28a3f7f7af11ff871947).
This verifies unchanged gameplay and compilation; scene migration/visual QA still pending.
Task temp: E:/CodexTemp/TypingHero/dark-ages. Existing checkout/staging preserved.
Doctor passes; server free space 3.3 GiB. Required Edge61a46c05 is disconnected;
user notified, no replacement profile selected. Continue independent server checks.

Migration diagnostics: initial spritesheet API did not yield usable sprites; switched
to the installed Unity.2D.Sprite.Editor provider (explicit editor assembly reference,
no package/dependency addition). Diagnostic showed imported asset was a Cubemap:
set Texture2D explicitly before slicing. Imported 11 role slices now resolve.
The existing typing rail also retains disabled legacy sliders without root Images;
post-pass targets the named skin-owned sliders instead of those retired controls.
Build callback failures now raise BuildFailedException, preventing false packaging.
Repeat-apply gate passed in Unity (20261016T121926-webgl): exact scene and four
shared-prefab contents unchanged on second Apply, including serialized IDs/references/listeners.
Earlier failed builds are diagnostic only, not accepted artifacts.

WebGL regression passed: 54 checks, 381.562s, snapshot
886bc0f608d0602e34d81348c327b4309f5b0aff1af1edaa6ec66fe45ef77421,
Chromium153.0.8010.12/SwiftShader, sequential containers2CPU/8GiB/shm1GiB.
Three850HP reference wins:88.402/91.465/93.900s; median91.465s.
No JS/Unity exceptions;107 fixture reads and5 fixture writes intercepted, network
isolated. Covered actual keyboard typing/error/timeout/Slayer, parry queue,
pause/resume, defeat/victory result freeze, restart/menu, help/audio persistence.
No runtime C# or gameplay changes were made by the skin task. Subsequent editor-only
changes map the old muted/rank colors to the approved warm palette and make the
second Apply failure fail the build too. Final visual build and Quick now pending.
Unity-serialized assets restored; original scene object-ID sets and scene/character
GUIDs preserved. Original DarkAgesPNG/README/license bytes match downloaded ZIP.

Visual QA rejected the first skin build: Sliced edge art stretched because Panel
border12 cut through the full corner ornament; scene instances also reverted to
shared panel/blue-bar defaults because direct component changes lacked
PrefabUtility.RecordPrefabInstancePropertyModifications. Fix: full24px panel
corners, tiled9-slice edges at native scale (sidebar half-scale to preserve existing
padding), quiet dedicated row sprite, and recorded image/button/text overrides.
HUD/typing must remain Inset and actual HP must render RedFill after scene reload.
Restore original combat camera background101820; skin changes only the outer
PixelBackgroundCamera, preserving the arena.
The first custom UI driver reached firstVisible978 (scroll .0052) but its drag ended
short of the track bottom; extend native drag below the track to test rank1000.
This is a driver correction, not a game/scroll behavior change. That partial run
is not reported as an acceptance pass. Updated visual build is running.

Final skin visual profile passed21 checks (20261016T124232-webgl), snapshot
6ff902abd1477b6266314f91b2343e3491f41f36a8cf2b951f965fa7c5dcedc3.
Observed native960/1280/1920/640 composition, full corner art and tiled edge detail,
quiet HUD/typing frame, red HP/full/half/empty, gold stamina/full/empty in Slayer,
blue timer/combo, button normal/hover/pressed/disabled, help and both result states.
Native Unicode char keyboard events rendered/persisted Nguyễn; insertText alone
had not generated Unity legacy keypresses, so that earlier attempt was rejected.
Actual typing/parry reached victory89.3s; no gameplay setters. No runtime exceptions.
1,000 long Vietnamese-name fixtures:19-view pool, native wheel+thumb drag to rank1000,
9s refresh retaining bottom; name ellipsis and three columns inspected.
All original scene object IDs, existing UI-prefab GUIDs and12 role imports verified.
Title GAME OVER now gold to match the approved result palette.
The first iframe capture occurred during Unity splash despite a published QA state;
not accepted as iframe visual evidence. Separate native iframe input/fullscreen check
is running. Edge61a46c05 remains disconnected; that exact-profile acceptance is pending.

Separate iframe/native-fullscreen profile passed5 checks on the same QA build:
real name entry EmbedQA→Enter→first-help Continue→game→Escape; actual iframe960×540
and fullscreen1280×720 pause UI inspected. Host fullscreen-button focus initially
left Unity's iframe player suspended; a native canvas click restored focus and
actual render dimensions. No runtime setting was changed to accommodate the driver.
No JS/Unity exceptions. Final source/Unity assets retained; final Quick running,
then production build/CRC. Required Edge instance still unavailable.

Final Quick passed26/26, snapshot1108a5ebd9b76c1b25b6afb2a82ec3ba9d2cc9dfa3d1a11c49c0d6bb34c3b9ad.
Production building from that exact snapshot. No runtime/source changes after these
checks. GitHub packaging now also copies Hypnobius credits/license; YAML parse and
source paths pass. Core-match ZIP retained unchanged in Downloads.

## Dark Ages delivery — production ready, native Edge pending

- Production passed from final Quick snapshot
  1108a5ebd9b76c1b25b6afb2a82ec3ba9d2cc9dfa3d1a11c49c0d6bb34c3b9ad,
  result20261016T125353-production. qa=false and all generated JS exclude typingHeroQA.
  Production Unity scenes/shared prefabs normalize identically to checkout.
- Deliverable:C:/Users/ADMIN/Downloads/TypingHero-WebGL-dark-ages-20261016.zip,
  15,859,154bytes,23files, root index.html, Hypnobius CREDITS + DarkAges/Crusenho/
  Kenney/VT323 licenses. Server and local CRC/SHA checks pass. SHA256:
  b7b6ea706fbed516d91223e6cb95fb0fce9c26d52fae38365c205ff0ca48b118.
- Core-match ZIP untouched:15,847,525bytes; SHA256 still
  a5720eabf6a5650dd2c8297275b8948f33399ad6ceb25d6165691c344b9c7c5c.
- Source changes are scoped to DarkAges art/importer/post-core skin, four UI prefabs,
  two existing scenes, packaging credits, design contract and affected memory.
  No runtime C#, gameplay configuration, public API, events, Firebase schema, package version,
  character/arena asset or new package. The editor assembly references the already-installed 2D Sprite editor package. Existing dirty/staged work preserved.
  No branch/worktree, commit, push, release upload, stress or soak.
- Remaining acceptance: exact native Edge61a46c05 QA. Final BrowserSkill status after
  restoring its absent daemon still exposes only857ab518/9c92c27f; no substitute used.
  All actual game UI evidence above is isolated Chromium, not native Edge evidence.
- Retained for that check: fly-server:/data/typinghero-game-loop/dark-ages-qa (verified
  compiled QA copy + snapshot/toolchain); E:/CodexTemp/TypingHero/dark-ages contains
  only the reusable UI/embed drivers and runner helpers after cleanup. Remove these
  task artifacts once the required Edge check passes and no follow-up needs them.
  Shared Unity Library/cache/toolchain/license remain untouched. BrowserSkill shared
  daemon is kept running in managed exec session62854 (PID17452); probe status before
  reuse, do not assume the task ID is still live. No browser session was created.
- Captures and failed/intermediate result outputs are removed after recording these
  conclusions. Final whitespace/path/GUID/source diff checks pass. This checkpoint
  intentionally remains pending native Edge; do not report that profile as passed.
