# Dungeon main menu

## Accepted scope

Continue the existing compact-tutorial worktree and cache. Preserve all tutorial
WIP, including the known card/enemy-head overlap. Do not deploy itch.io.
The supplied plan is the design contract: static full-screen dungeon, small
Clackblade logo top left, unboxed menu bottom right. The original image 2 is
not attached to this fresh context; its composition is implemented from the
plan, without claiming a direct image match.

At reference 960x540: menu x624/y272, width240, five 36-high items with8 gaps,
right96/bottom56. Order Play/Tutorial/Top1000/Settings/How to Play; Cobblestone30,
ivory/gold with a small selection mark. Brand font56 at32/24. Play and Tutorial
open the same centered420x320 preparation card, with existing name input,
difficulty, description, validation, Back and Start. Only Start depends on name
validity. Initial focus Play; arrows/Tab and Enter; card focuses name, Back/Escape
returns to opener. Existing tutorial selection and saved difficulty/name rules
remain authoritative. Settings/Top1000 retain the accepted compact layouts.

## Source mapping

- `DungeonMainMenuBuild.cs` is the final menu composition called by the existing
  `BlackletterUILayout.Menu` pass. Legacy passes may temporarily reposition the
  same objects; the final pass restores this contract on every build.
- Existing `MenuPanel`/name input/difficulty/PlayButton become the preparation
  card and Start. Existing tutorial/collection/settings/help buttons retain their
  identities; the menu Play action is separate. `MainMenuUI` owns only UI state,
  focus, shortcut guards, validation display and existing launch dispatch.
- `BlackletterTitleBuild` identifies the brand at `Reference960x540/Logo`;
  the card title uses the existing Cobblestone UI font. Existing prefab, font,
  UIButtonFeedback/audio and DarkFantasy panel assets are reused.
- Existing CanvasGroup modal blocking is reused. The card's CanvasGroup ignores
  the disabled menu parent, allowing its controls while blocking underlying menu.
- No gameplay/provider/schema/PlayerPrefs key or difficulty rule changes.

## Raster asset provenance

Built-in imagegen, stylized-concept direction; game palette reference directly
inspected at `Assets/Art/UI/CombatR7/scene-mid.png`. No sprite animation generated.
Final asset `Assets/Art/UI/DungeonV2/menu-dungeon.png`,320x180, nearest-neighbor
size conversion from generated1672x941 image; Point/Clamp/no mipmaps/uncompressed.
Inspected final native image: stepped pixel clusters, stone arches/corridor depth,
copper torch contrast, distant blue light, quiet dark right region. Runtime
composition/legibility acceptance is recorded separately below.

Final generation prompt (built-in tool):

> Use case: stylized-concept. Create a production pixel art background for the Unity game Clackblade main menu. 16:9 landscape, intended final native resolution 320x180. Deliberate crisp low-resolution pixel clusters on an exact uniform coarse grid, limited palette and hard stair-stepped silhouettes, no anti-aliasing, no smooth painted surfaces. Dungeon interior with deep perspective: large stone arch and worn stone pillars dominating LEFT half, dark corridor receding to distant cool blue/cyan light near left-center, small copper-orange torch illumination on stone, dark desaturated purple charcoal masonry and floor. Right third especially lower right must be very dark, quiet low-detail negative space for ivory menu lettering. Top left must allow a small logo. Strong depth and atmospheric silhouettes, readable large stone shapes. No characters, creatures, text, logos, UI, frames, watermarks. The supplied game art reference uses purple coal stone outlines with broad clean pixel clusters; preserve that muted palette and coarse grain while adding perspective. Output a clean wide game environment, no mockup.

## Verification checkpoint

Targeted cached Unity WebGL build with strict repeat-builder check, then real
keyboard/mouse integration and visual review at1280x720 and640x360. No full Quick
suite, six tutorial lessons, performance/soak profile or release upload.
First build150853 stopped at compiler error (local variable shadowing); corrected
the variable name before the next build. Intermediate repeat-build differences
were fixed by recording prefab overrides and explicitly setting new button
colors/sprite states/skin. Native review caught the new texture's cubemap shape;
the final pass explicitly imports Texture2D and requires a valid Sprite. Mouse
focus uses empty explicit navigation links; MainMenuUI retains arrow/Tab movement.
The card is the last design-root sibling, covering the underlying selection mark.

## Final scoped acceptance — 2026-10-21

Unity2022.3.25f1 on fly-server: QA153008 WebGL PASS, strict repeat-builder PASS,
input `b2b63f11963989f3e233cdbd4da8c43faddf1952287260c2a6f5069b9f37b020`.
Reused the existing compact-tutorial Library/cache. Imported Unity's final menu
scene and background metadata; the two removed original records are obsolete
DungeonControlSkin components on text menu buttons. No SampleScene/tutorial WIP
was imported into the task changes. Only new diff-line trailing whitespace is
removed from Unity's generated menu serialization; object values are unchanged.

Native Edge155.0.4283.62 / ANGLE D3D11 / Playwright1.58.2:83/83 checks PASS in29.777s
at1280x720 and640x360, zero JavaScript/managed exceptions and zero provider writes.
Real input verified initial focus, Up/Down/Tab/Shift-Tab/Enter, valid/invalid names,
all five actions remaining available with an invalid name, input focus on opening,
blocked underlying actions, difficulty changes and retention, Back/Escape/opener
focus, Settings and Top1000 mouse/keyboard open/close, quick-help Escape, and name
persistence after scene return. Tutorial Start enters the existing tutorial;
first Play preserves the Unseen rule; after Skip, Play enters an ordinary run.
Two additional hover checks PASS on the same player; directly inspected gold hover/focus text and selection marks at both sizes without stealing keyboard focus. No six-lesson execution or combat/balance/performance/release claim.

Directly inspected final menu, Play/Tutorial cards, Settings, Top1000 and help
at both sizes: coherent320x180 pixel grain, no distortion/overflow, clear ivory
text on the quiet right region, correct card and unchanged compact modal geometry.
Fixture interception/external blocking stayed enabled. The inherited tutorial
card/enemy-head overlap remains outside this task and unresolved.

Preview http://127.0.0.1:8240, local final player/driver/report under
`E:/CodexTemp/TypingHero/compact-tutorial/dungeon-final-build` and `driver/dungeon_*`.
QA/hover captures, browser profiles and reports were removed after recording
the results above. Remote immutable player/log/snapshot:
`/data/typinghero-game-loop/task-compact-tutorial/results/20261021T153008-webgl`.
Retain the preview/player and existing worktree for user review and tutorial WIP;
close/remove these task resources after review. Earlier menu/tutorial resources
are not owned by this task.

## Integration and cleanup

Implementation `d9be2c8` pushed to `origin/micharel09/compact-tutorial` and
fast-forwarded into `origin/main`; fetched remote SHA equals the task commit and
ancestry verification passes. The original seven tutorial WIP files remain dirty,
including only the original tutorial hunk in BlackletterUILayout; SampleScene
still has its original806 added/42 removed lines. No tutorial changes entered
the menu commit. No itch deployment.

Removed the task's six failed/superseded remote result directories and local
superseded white-background player/captures/reports. Retained final immutable
153008 player/log/snapshot, final local player, task preview on8240 and its driver.
Keep the pre-existing worktree/branch for the user's review and unfinished
tutorial work; remove the task preview/player only after review. Shared cache,
toolchain, source, earlier menu previews and other agents' jobs remain untouched.


## Name validation, Escape draft and background focus - 2026-10-22

Accepted follow-up: names use 3-10 characters; Back/Escape retain the in-menu
text/difficulty draft and only Start persists identity. Clicking empty background
restores the last selected main action. Preserve existing visuals, tutorial WIP,
PlayerPrefs keys, provider and tutorial dispatch. No itch.io deployment.

Implementation: PlayerIdentity exposes shared length constants without changing
normalization/API/storage. MainMenuUI checks raw length before normalization in
both preview and Start, sets the input limit and disables TMP Escape restoration.
Legacy saved names load intact with Start disabled until corrected. Main-menu
selection memory only tracks actual EventSystem selection, excludes hover, and
restores null selection only while the main menu is interactable and unblocked.
DungeonMainMenuBuild applies the same input settings on every build.

Sources inspected: [TMP 3.0.6 input field](https://docs.unity.cn/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_InputField.html),
[Unity focus discussion](https://discussions.unity.com/t/keyboard-navigation-doesnt-work-if-you-click-off-the-ui/635261),
[TMP text](https://docs.unity.cn/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html).
Installed TMP 3.0.6 source confirms DeactivateInputField restores the original
text only when m_WasCanceled and m_RestoreOriginalTextOnEscape are both true.
Glyph bounds and actual rendered images are required for Overflow text; absence
of isTextTruncated alone is not acceptance.

Filtered Unity PlayMode PlayerIdentityTests: 15/15 PASS, no skipped/failed,
result `/data/typinghero-game-loop/task-compact-tutorial/results/20261022T015748-quick`,
input `2eb55c66bee8884f055007d0587ba78104567ef9b394179c8be163bb36c1724a`.
The task-local runner replaces only the Unity category argument with testFilter
PlayerIdentityTests; no repository runner changes or full Quick run. Tests cover
0/1/2/3/9/10 boundaries, unchanged legacy16 preference, UI raw-overlimit rejection,
legacy16 display/disabled Start and TMP configuration, existing normalization and
reload persistence. QA WebGL `20261022T015918-webgl` PASS on the same input; Unity2022.3.25f1,
strict repeated-builder reports unchanged serialized objects/references/listeners.
No generated tutorial/scene output was imported into the patch.

Native Edge155 / ANGLE D3D11, real keyboard/mouse, fixture interception and
external blocking: 279 assertions PASS for typed boundaries0/1/2/3/9/10/11/16,
selection replacement at10, valid/invalid Back/Escape drafts from input/selector/
Back, both openers, difficulty retention, each main action after background
click plus Up/Down/Tab/Shift-Tab, Enter, modal background blocking/return,
Tutorial/Unseen/Skipped dispatch and saved name into gameplay/menu.
Two widths1280x720 and640x360; no six-lesson/balance/stress/soak coverage.
Read-only bridge observes only; all input uses actual keyboard/mouse.
Additional browser-storage legacy16 fixture:17 assertions PASS at both widths,
intact display/error/disabled Start, no change after Back/Escape/reload, actual
keyboard correction to fixed hero, normalized Fixed Hero in gameplay/menu/reload.
Zero JS/managed exceptions and zero provider writes in both qualified runs.
Directly inspected name10, name2 error and legacy16 error images at both widths;
name10 glyph bounds fit the input rectangle. No truncation-only visual claim.

Clipboard acceptance is NOT passed: real Ctrl+V from browser clipboard leaves
the field empty. Both new player and original153008 player reproduce this;
baseline observation4 assertions PASS confirms it predates this patch. No WebGL
clipboard bridge exists in the project. A scope question was sent about adding
one; the three-fix patch deliberately leaves clipboard support unchanged pending
that decision. Do not report paste11/16 as passing name-limit tests.
One early selection-replacement assertion read the preceding QA frame; waiting
for the rendered frame passes without product changes. The legacy fixture driver
also needed to wait for QA initialization after reload. These driver failures do
not establish product failures. Earlier paste assertion failures remain failures.

Qualified native runs: menu-fix-qa279 in149.697s, menu-legacy-qa17 in26.758s,
clipboard baseline4 in6.838s, Edge155.0.4283.62. Their captures/profiles and the
failed clipboard capture directory were removed after textual evidence recording.
Final immutable player/snapshot/log remains in remote015918; local menu-fix-build
holds the same player.

### Follow-up integration and retained resources

Implementation fc06acd pushed to origin/micharel09/compact-tutorial. Integrated
new origin/main particle-physics commits through2c3a742 in merge dedfe16; the
verification document append conflict was resolved by preserving both records.
All four menu runtime/editor/test files are unchanged from the tested snapshot.
Filtered integrated identity run20261022T021754-quick:15/15 PASS, input
3f11a598c427567a85ec2f0dc40a023f8689bee9d4a8f4e6aed52b1cf83138a6.
Reused native menu evidence because affected menu inputs/configuration are
unchanged; no second WebGL build for unrelated, upstream-verified VFX changes.
Pushed dedfe16 to origin/main as a fast-forward; fetched ancestry confirms both
fc06acd and dedfe16 on main. Seven original tutorial WIP files remain byte-identical;
none entered the task commits. No itch deployment.

Removed the superseded11-test remote result and four completed native capture/
profile directories; shared caches, earlier resources and other jobs preserved.
Preview http://127.0.0.1:8246 is open at1280x720; task process26524,
menu-fix-preview and menu-fix-build under E:/CodexTemp/TypingHero/compact-tutorial.
Keep this final player/preview plus menu_fix/menu_legacy drivers for review and
reproduction. After review, close the preview browser/process and remove these
specific task-created preview/player directories. Keep the pre-existing
compact-tutorial worktree/branch until tutorial WIP is resolved and preserved.
The original8240 preview and earlier assets were not created by this follow-up
and remain untouched. Clipboard support remains a separate pending scope decision.

## Shared3D world continuation ? 2026-10-22

The static menu backdrop is replaced by the same retained3D room used by gameplay.
All96original menu RectTransforms and existing scene GUID/meta are unchanged.
Strict repeated builder,22focused Unity checks and native129+7checks pass.
The same room/camera/RT survive all transitions and3measured repeat cycles. Integration
and final preview are tracked in [shared dungeon checkpoint](shared-dungeon-session.md).
