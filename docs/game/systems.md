# System and configuration map

| Owner | Source/config | Consumers and verification |
| --- | --- | --- |
| Typing | `Assets/Scripts/Core/TypingManager/` and `Assets/Data/Words/WP_Common200.asset` | PlayerController, Slayer, scoring subscribe to static events; quick typing/timeout/type-ahead |
| Combat | `Assets/Scripts/Combat/`, `Assets/Data/Attack/` | Animation events, hitboxes, gates, parry target and health; quick + WebGL |
| Player | `Assets/Scripts/Player/`, `Assets/Characters/Player/Player.prefab` | Slayer, health, attack controller and Attack/Parry-tag sprite sorting; Quick actual-controller lifecycle checks + keyboard WebGL |
| Enemy | `Assets/Scripts/Enemy/`, `Assets/Characters/Enermy/Skeleton.prefab` | Stamina, stun, death, parry; quick + WebGL |
| Skeleton art | `design/skeleton-hero/gif-pack`, `Assets/Art/EndlessSkeletons`, `EndlessAssets` |18 unchanged strips/174 cels/100ms, three controllers with both attacks and own Hurt/Die; role assets switch the existing actor. Original tutorial art/controller GUIDs retained. Technical import and all six real cue/contact/pause tests pass; final browser receipt in progress. |
| Score | `Assets/Scripts/Core/Scoring/`, `Assets/Data/Score/ScoreRules.asset` | UI, reset hooks, Firebase submit trigger; quick combo + restart |
| Combat HUD | `CombatR7Build`, `CombatR7Status`, `CombatHUDLayout`, `TutorialOverlay`, `Assets/Art/UI/CombatR7/` | r6 components/r7 fixed320x180 layout, live status/score/clock and three58px word cells; Point/Clamp components/static bitmap font. Existing IDs/bindings retained; popup alone follows the real enemy head. Quick104 and strict repeat serialization pass; final WebGL/public acceptance pending in ui-kit-v2 checkpoint. |
| Slayer presentation | `SlayerScreenFX`, `CombatViewportCamera`, `ScoreUI_ComboHUD`, `FloatingScore`, `CombatR7Build` | Real body landmarks/eight-native-pixel bands; fixed HUD/typing geometry, hidden typing board and restored environment. Lens correction anchors the floor at native138 using the brain state, preserving impulses and pause camera ownership. Actual rendered glyph/foot browser checks are in progress; see ui-kit-v2 checkpoint. |
| Match statistics | `Assets/Scripts/GameController/CombatSession.cs` | Letter-attempt event, parry success, lethal health paths and result label; Quick + keyboard browser rounds |
| Endless survival | `EndlessRun`, `EndlessDifficulty`, `EndlessDifficultySelector`, `EndlessDuel`, `EndlessOpponent`, `EndlessProfile`, `EndlessOverlay`, `SkeletonController` | Five roles/known elite/three-phase captain, one-Continue checkpoint, bounded recovery, boss heal, local records/achievements and dedicated online queue; tutorial and Classic remain isolated. See `docs/plans/endless-duel.md`. |
| Audio preferences | `Assets/Scripts/Audio/AudioPreferences.cs`, `MusicPlayer`, `SfxPlayer` | Menu/pause sliders, PlayerPrefs and active/future voice gain; before-scene listener gain .5 for output headroom. Frozen CLACKBLADE native signal gate passed; latest CombatR7/Firebase release remains pending. |
| Typing audio | `Assets/Scripts/Audio/Binders/TypingInputSfx.cs`, `ShogunTheme`, `Assets/Audio/SFX/Clips/KenneyInterface/`, `Events/UI/TypingCorrect` and `TypingError` | OnInputResult only; existing mixer/pool/preferences, no voice creation while SFX0; authored-asset/input/lifecycle regressions. Exact mute and output headroom verified on frozen CLACKBLADE candidate (15+1511 native checks); final CombatR7/Firebase release is gated. See clackblade-rebrand plan. |
| Pause/menu | `Assets/Scripts/GameController/`, `Assets/Scripts/UI/` | Scaled/unscaled time, camera/audio, scene loads; browser flow |
| Player identity | `PlayerIdentity`, `MainMenuUI` | Existing PlayerPrefs key/validation; absent names become persisted GuestXXXX. Menu, tutorial, restart and leaderboard consumers verified with native input and intercepted payloads; no UID/provider/schema changes. |
| Tutorial | `TutorialRun`, `TutorialDirector`, `TutorialOverlay`, `Assets/Editor/PixelUI/TutorialBuild.cs` | Six real-action lessons in SampleScene; in-session reload checkpoints, separate reading/pause lock, independent PlayerPrefs status,clean battle and submission guard. Quick + actual keyboard WebGL; source-backed details in gameplay and verification. |
| VFX | `ParryClashBurst`, `ClashLightingRig`, impact/parry spawners, `ParryClashAssets`, `CombatFeedbackBuild` | Parry Current/A/B/C remain unchanged; BCompact uses24particles (2/2/10/10/0), twenty-two fixed spark states, twenty continuous launch/brake/fall/one-bounce/rest180ms paths plus two fast front fragments; twelve added falling sparks reverse X at32-44speed/10per-second launch X drag, split6near/6far in a -12..36degree fan. Integer16PPU silhouette follows projected velocity/depth (.75-1.5), minimum1x1; Front/Back preserve actor sorting and mirror only X. Manual unscaled/pause-held simulation restores modules/materials on reuse; guarded return, eight leases/192particles and strongest single-light pulse. Quick79 passes including24seed/two-direction300ms reach/length acceptance; native24particle QA pending (prior40clashes/B/compact lifecycle cover16particle revision). Earlier mirror p95 concerns remain unresolved. Current/camera/gameplay/itch unchanged; legacy assets/licenses retained. |
| World lighting | `WorldLightingBuild`, `SurfaceNormalAssets`, `WorldMenuBackdrop`, `Assets/Art/Lighting/` | Post-environment lit materials, shared rig/bloom, fixed-foot shadows and menu world backdrop. Draft secondary normals retain source sprite metadata; full surface art/frame review awaits approval. Repeated-builder and prototype native evidence only. |
| Leaderboard | `Assets/configFireBase/`, `EndlessCollectionBuild`, `Assets/Prefabs/PixelUI/LeaderboardRow.prefab` | Separate Endless result/provider/ETag queue and Classic API; shared virtualized table and collection modal. Loopback conflict/boundary-tie/error tests pass; browser fixtures block production writes. |
| UI presentation | `ShogunTheme`, `ShogunEnvironment`, `CombatR7Build`, `CombatR7Backdrop` | Existing jade/coal menus, tutorial, Collection and overlays retained. Combat uses four approved purple dungeon layers; preserved Japan root stays inactive in combat and is reused by repeated migration. Point pixel components, persistent prefab overrides and source-backed r6/r7 asset mapping; final native matrix pending. |
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

Endless Normal retains `leaderboards/endless_v1/{uid}`; Very Easy/Easy/Hard/Very Hard
use `leaderboards/endless_v2/{very_easy|easy|hard|very_hard}/{uid}` with `name`, `defeated`, `score`,
`updatedAt`; uid uses the existing LB_LocalUID. `EndlessLeaderboard` owns result
ordering and durable best-pending queues partitioned by difficulty (legacy keys
mean Normal). `EndlessDifficulty_v1` stores the menu ID; run owns a snapshot.
Recovery and effective combo divide their existing bases by pace, without mutating
assets. Collection filter owns its own request version and selected level; `FirebaseEndless` reuses provider auth/URL/
parser/headers while keeping Classic score semantics untouched. Modal mode0 is
Endless,1 Classic,2 opponent profiles,3 achievements. Rows remain virtualized at
28px/404px viewport; community rows show defeated/score. Versioned
profile keys remain local. The index fragment [endless-firebase-index.json](endless-firebase-index.json)
is preparation ONLY: merge its `.indexOn` into existing rules in a separate approved
production configuration task. Each of the five paths indexes defeated; it adds
no read/write permission and has not been deployed. Never deploy the fragment as a replacement ruleset.
