# System and configuration map

| Owner | Source/config | Consumers and verification |
| --- | --- | --- |
| Typing | `Assets/Scripts/Core/TypingManager/` and `Assets/Data/Words/WP_Common200.asset` | PlayerController, Slayer, scoring subscribe to static events; quick typing/timeout/type-ahead |
| Combat | `Assets/Scripts/Combat/`, `Assets/Data/Attack/` | Animation events, hitboxes, gates, parry target and health; quick + WebGL |
| Player | `Assets/Scripts/Player/`, `Assets/Characters/Player/Player.prefab` | Slayer, health, attack controller and Attack/Parry-tag sprite sorting; Quick actual-controller lifecycle checks + keyboard WebGL |
| Enemy | `Assets/Scripts/Enemy/`, `Assets/Characters/Enermy/Skeleton.prefab` | Stamina, stun, death, parry; quick + WebGL |
| Skeleton art | `design/skeleton-hero/`, four `Skeleton_White/Skeleton_With_VFX` PNGs |36 original-alpha frames; approved11 frozen, source-identical cels reuse paint; Hurt shared by Stun/Slayer. LibreSprite export checks + native WebGL1280x720/640x360. |
| Score | `Assets/Scripts/Core/Scoring/`, `Assets/Data/Score/ScoreRules.asset` | UI, reset hooks, Firebase submit trigger; quick combo + restart |
| Combat HUD | `CombatHUDLayout`, `ShogunTheme`, `TutorialOverlay` | Foot-local nameless status, screen-fixed corner total/combo, tray gap12 and single accumulated enemy-head award; shared fixed-body landmarks for Slayer framing, camera-following tutorial highlights. See combat-hud checkpoint. |
| Slayer presentation | `SlayerScreenFX`, `ScoreUI_SlayerBinder`, `ScoreUI_ComboHUD`, `FloatingScore`, final `ShogunTheme` migration | Fixed body landmarks/full-width bars, environment and HUD restore, fixed corner score/award pulse/tier tint and tutorial tray highlight; Quick53 plus keyboard WebGL/four render sizes. See combat-hud/slayer-restore checkpoints. |
| Match statistics | `Assets/Scripts/GameController/CombatSession.cs` | Letter-attempt event, parry success, lethal health paths and result label; Quick + keyboard browser rounds |
| Audio preferences | `Assets/Scripts/Audio/AudioPreferences.cs`, `MusicPlayer`, `SfxPlayer` | Menu/pause sliders, PlayerPrefs and active/future voice gain; Quick + browser persistence |
| Pause/menu | `Assets/Scripts/GameController/`, `Assets/Scripts/UI/` | Scaled/unscaled time, camera/audio, scene loads; browser flow |
| Tutorial | `TutorialRun`, `TutorialDirector`, `TutorialOverlay`, `Assets/Editor/PixelUI/TutorialBuild.cs` | Six real-action lessons in SampleScene; in-session reload checkpoints, separate reading/pause lock, independent PlayerPrefs status,clean battle and submission guard. Quick + actual keyboard WebGL; source-backed details in gameplay and verification. |
| VFX | `Assets/Scripts/VFX_SFX/PixelSparkBurst.cs`, impact/parry spawners, `Assets/ScreenFX/`, `Assets/Editor/PixelUI/PixelSparkAssets.cs` and `CombatFeedbackBuild.cs` | Four physical pixel profiles under `Assets/Art/VFX/PixelSparks`, two world Particle Systems each;16PPU/order30, fixed-foot planes, gravity/bounce, eight bursts per owner. PooledVFX owns guarded release; PixelSparkBurst owns emission/pause/clear/completion. Existing flash/slash/afterimage/audio remain. Quick66 covers lifecycle/profile/damage context and builder retention; native receipts in verification. Original Pimen source/licenses retained. |
| Leaderboard | `Assets/configFireBase/`, `Assets/Prefabs/PixelUI/LeaderboardRow.prefab` | Firebase REST and LeaderboardUIBinder row pool/scroll; offline fixtures only |
| UI presentation | `Assets/Editor/PixelUI/ShogunTheme.cs`, `ShogunEnvironment.cs`, `Assets/Art/Environment/TinyPixelJapan/` | Final post-core migration of existing/inactive UGUI; jade/coal/ivory flat controls, original Japan layers/tints/parallax, recorded prefab overrides; repeat-builder and WebGL/native checks |
| UI interaction | `Assets/Scripts/UI/LeaderboardModal.cs`, `UIButtonFeedback.cs`, `PanelReveal.cs`, `TypingPanelFeedback.cs` | Modal focus/shortcut ownership, existing SFX pool for pause-safe UI audio, unscaled panel reveal and stable typing error border. GamePauseController disables hidden pause interaction and clears selection. |
| Desktop packaging | `Assets/Automation/Editor/GameLoopBuild.cs`, `tools/game_loop/` | windows profile builds production Windows64 Mono on Ubuntu with pinned module; windowed1280x720/resizable and restores project settings. Complete player ZIP, credits and QA exclusion; Wine64 startup verified, real Windows interaction/audio unverified. |

Scene serialized values override class defaults. Preserve `.meta` GUIDs when editing
or moving assets. Runtime assembly owns Assets scripts; editor automation and tests
have separate assembly boundaries. Package references: TextMeshPro, UGUI,
Cinemachine, URP; do not move those dependencies into a parallel architecture.

Firebase scene keys: `leaderboards/global`, `name`, `bestScore`, `updatedAt`.
LeaderboardUIBinder fetches up to 1,000, refreshes while visible and reuses row
instances. Pixel UI enables virtualization: the full dataset stays in `_rows`,
while the pool covers the viewport plus four overscan rows (19 objects at the
current 404/28 modal geometry). Top1000 is opened from menu/pause; its existing
provider and binder refresh only while enabled by the modal. Active object count is not dataset count. Refresh starts
at top once and should preserve subsequent scroll position.
FirebaseLeaderboard caches fetch results and throttles requests; tests must account
for cooldown rather than bypassing it in browser gameplay.
