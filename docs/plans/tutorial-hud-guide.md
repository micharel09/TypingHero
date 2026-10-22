# Single HUD tutorial guide

## Acceptance

Replace six lessons with one English guide screen. The follow-up below replaces its shared background with four separate callout cards. All four groups are visible together: Your HP, Enemy HP / Guard, Typing / Timer and Score / Combo. Five static gold arrows follow live RectTransforms after HUD layout, including separate enemy HP and guard destinations. Reuse the dungeon, HUD, fonts, panel and button. Include Space/Esc copy and Start Battle / Enter. No practice, retry, animation, illustrations or extra page. How to Play stays separate.

First Play for TutorialProgress_v1=0 opens the guide; saved legacy values 1 and 2 bypass it. Tutorial reopens it. Only explicit Start Battle persists completion. Reading freezes gameplay/input/time/audio and cannot submit scores. Pause/Resume retains the reading lock; Restart reopens it; Menu abandons without marking viewed. Repeated confirmation is guarded; DungeonSession waits for Enter release before activating the new selected-difficulty encounter intro.

## Implementation checkpoint

- Independent Orca worktree: `C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-hud-guide`, branch `micharel09/tutorial-hud-guide`, base origin/main f82c012.
- Runtime lesson logic removed. Continue listener and original control/script GUIDs retained; obsolete controls hidden. Final builder wires live HUD references after skin/typography/layout passes.
- Read-only QA observations expose panel, arrow tips/targets and audio pause. Tutorial drivers now use real guide/encounter transitions; former practice consumers use ordinary combat.
- Doctor passed with pinned Unity2022.3.25f1, server activation and browser image available. Server has roughly3.1GiB free; preserve other tasks/resources.
- Initial serialize091318 failed on an obsolete staminaHighlight binding in CombatR7Build. Corrected. No passing runtime or visual acceptance claimed yet.

## Verification pending

Focused TutorialTests / DungeonEditorEntryTests; strict repeated serialization; real keyboard/mouse at1280x720 and640x360 with fixture interception and external blocking. Directly inspect captures for readability, clearance and arrow routes. Record actual counts and failures here and in affected game memory. Commit/push/integrate only after checks pass; no itch.io upload.

## Unity checkpoint 2026-10-22

- Serialize093736 / input80eac3948f82be7299743990e4c4054e45c78b44cace9edaa74e4363806e83e0: strict repeated builder PASS; seven actual TutorialTests / DungeonEditorEntryTests PASS.
- MainMenu remains byte-identical to HEAD. Generic builder output also materializes unrelated dormant dungeon objects in SampleScene; the committed scene is projected to the tutorial subtree only. All925 original scene IDs remain,80 new blocks belong to20 guide/arrow nodes; every non-tutorial block remains byte-identical. All local scene references resolve.
- Rechecking the same seven tests against the scoped scene before the focused tutorial WebGL build. This extra run is required because the asset under test changed after projection. Native/visual acceptance remains pending.

- Direct-source recheck094725 / input5e5ad45d:3/7 passed,4 scene-load tests failed on an existing malformed DungeonWorld GameObject1784889890 block. Unity reported its Transform, LightProbeGroup and two MonoBehaviours missing from the component list despite unchanged field references. That block in origin/main contains blank lines between every YAML line. Removed only those blank lines; IDs, fields and configuration remain identical. Recheck required; no log suppression or weakened assertion.

## Visual/consumer iteration

- Source9d1fc47c: scoped seven Unity tests PASS after the one-block World formatting repair. QA095149 build/repeat PASS; strict browser stopped after5checks because Cobblestone has negative leading bearings and Score's glyph descender exceeded its44-unit label rect. Actual text was inside the panel. Added6reference-unit leading padding and increased Score label height to48; retain strict label-bound checks.
- Native Edge155/GTX1650Ti diagnostic on that immutable player:23checks PASS,0exceptions/provider reads/writes; only diagnostic text containment used panel bounds. All guide/pause captures reviewed at1280x720 and640x360. This is not final strict layout acceptance.
- Direct review of the fresh-encounter capture found that EndlessOverlay clones the tutorial panel and inherited the new guide copy/background plus reduced action typography. Fix the existing clone consumer to activate only its original intro fields, restore864-wide intro bounds/action size and leave no panel/copy during combat. Added Unity and browser regression assertions plus a real typed attack after Fight. Rerun the seven scoped tests and final QA after this runtime consumer change.

- Final-runtime input0c8b3a88988177676262b280baf67aed56b21a66f67c1089ab4c740c6a205844 / run101452:7/7 Unity PASS including no cloned guide copy in intro/live combat. WebGL strict builder passes, but incremental IL2CPP link fails with duplicate EqualityComparer symbols in GameAssembly.a. Retry rebuilds only that generated archive while holding the existing exclusive runner lock. Reuse passing tests while runtime/config/dependencies stay unchanged.
- Padding-only100437 build/repeat succeeded; server browser preflight failed just below3GiB. Removed two owned redundant server players095149/100437 after matching all32file hashes of the diagnostic player to its retained local copy. Free capacity returned to3.154GiB; other jobs/cache/toolchain preserved.

## Final acceptance

- Runtime input0c8b3a88988177676262b280baf67aed56b21a66f67c1089ab4c740c6a205844:7/7 actual Unity tests PASS (101452), strict builder/build PASS (103149) after rebuilding only generated GameAssembly.a. Source/runtime/config/dependencies remain the same across that cache recovery.
- Software profile103149 remains FAIL after15checks: missed Play input on the roughly193ms software renderer. Its strict two-size glyph/arrow/clearance assertions had passed. No software-profile success claimed.
- Same immutable player, Edge155.0.4283.62 / Playwright1.58.2 / GTX1650Ti ANGLE D3D11:26/26 final checks PASS;0exceptions/provider reads/writes, fixtures/external blocking retained. Hard selected; initial/unread menu exit, pause/resume/restart, held Enter, persisted completion, automatic bypass, replay, mouse confirmation and real typed attack pass. All guide, pause, fresh intro and live-hit captures directly inspected; no cloned guide in intro/combat. All32player file SHA256s match server/local.
- Final scene projection:925original IDs retained,1005total;168tutorial blocks match the final builder apart from trailing blank-field whitespace. All non-tutorial blocks remain byte-identical; all local references resolve. Scene SHA256baa59c3fc7aa271d8e6ed561971e8b731f9f771bdc21f813f8c89db1a4f1cbff. MainMenu remains byte-identical to base. World repair removes only19blank lines in GameObject1784889890, with identical fields/IDs.
- Current gameplay/systems/verification memory updated; no itch.io upload or manual workflow dispatch. Git workflow is manual-only and has no itch deploy step. Integration and final resource cleanup follow verified acceptance.

## Resource cleanup

After written evidence, all eight owned server result directories and the task-only local temp tree (players, captures, diagnostic/final Edge profiles, manifests and helpers) were removed. Native QA processes exited and port8267 has no listener. Shared source, import/cache/toolchain/builds and other tasks remain. After verified remote integration, remove the clean task worktree/local branch; retain the remote task branch under repository policy for comparison until no longer needed.

## Remote integration receipt

Source commit680ba53ab5c9fcb49d84a046c76c391534dc2a0d was pushed to the task branch and fast-forwarded to origin/main fromf82c012. A fresh fetch confirmed HEAD equals origin/main and task ancestry is preserved. All scoped checks and artifact cleanup above are complete. This documentation receipt is pushed through the same fast-forward workflow; the clean task worktree/local branch are removed afterward. Remote branch `micharel09/tutorial-hud-guide` is retained under repository policy for comparison and may be deleted when no longer needed. No itch upload or workflow dispatch occurred.


## Follow-up: separate HUD callouts (2026-10-22)

User supplied an image showing annotations distributed beside the actual HUD and requested separate boxes instead of one crowded panel. Keep the four groups visible together, existing copy/font/colors, five real-HUD arrows, one Start Battle button and all lifecycle/storage/provider behavior. Replace only the large background with four smaller existing-style panels. Keep HP left, enemy right, score above center and typing below left; shortcuts/action sit separately above the typing tray. No reference animation or art recreation is in scope.

Independent Orca worktree `C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-hud-cards`, branch `micharel09/tutorial-hud-cards`, base c666fd5. Builder and scoped layout assertion updated. Strict serialization, two-size visual/input QA and scene projection passed as recorded below. Shared runner contention was handled without interrupting other jobs. Remote integration receipt follows verified acceptance.


### Follow-up verified result

- Integrated origin/main fd3f21d before the final WebGL snapshot, preserving the separately verified parry updates. Canceled only this task's superseded queued serialize jobs; no other job interrupted. Final inputa0f2690bf90397d5fbccf510d5b01502acecbe9eb2703140b1ebd7b73f65bc3f / run20261022T134816-webgl: build and strict repeat builder PASS. No runtime/API change or new dependency.
- Four existing-style dark cards, transparent shared root, HP left/enemy right/score above center/typing below left. Same copy/font, five HUD-bound arrows, separate shortcut line and retained Start button. Final serialized scene equals the captured builder output; scoped projection retains1005 original IDs, adds16 blocks for four cards, and leaves837 non-tutorial blocks byte-identical. All local references resolve; SHA25647c0d23ce881d5eb1be7192c84c45dc501d114971380456e05213795c3828b7e. Existing listener and GUIDs retained.
- Software browser FAIL after15 checks on missed menu Play at184.63ms/frame; its strict two-size copy/card-clearance/arrow checks had passed. Native Edge155.0.4283.62 / Playwright1.58.2 / Intel UHD D3D11 on the same32-file hash-matched immutable player:26/26 PASS,0exceptions/provider reads/writes. No assertions weakened. Both guide sizes, pause, fresh intro and live-hit images directly inspected. No guide/card leakage into encounter or combat. Reuse existing unit lifecycle evidence: runtime code is unchanged; new scene/input behavior is covered by this focused real-input run.
- Interactive fixture-isolated Edge demo opened at127.0.0.1:8251, in the guide after transition completion. Retain `E:/CodexTemp/TypingHero/tutorial-hud-cards-20261022` (player, evidence, independent preview driver/profile/helpers) while the user reviews; close the demo and remove this directory when review is finished. Previous requested demo at8250 remains untouched. No itch upload.


### Follow-up integration and cleanup receipt

Source commit9927025a547b82006bb0edf0530d8f25751dd31d was pushed to `micharel09/tutorial-hud-cards` and fast-forwarded to origin/main fromfd3f21d. A fresh fetch confirmed HEAD equals origin/main and task ancestry is preserved. Owned server result20261022T134816-webgl and its `/tmp` transfer archive were removed after copying logs, captures, serialized scene, snapshot and32-file hash evidence to the retained local review directory. Shared source/cache/build and other jobs remain untouched. This receipt is documentation-only; remove the clean task worktree/local branch after its remote verification. Retain remote branch `micharel09/tutorial-hud-cards` for comparison under repository policy; delete it only when no longer needed. No itch upload.
