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
