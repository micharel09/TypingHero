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
`E:/CodexTemp/TypingHero/compact-tutorial/dungeon-final-build`, `driver/dungeon_*`,
and `dungeon-qa/browser.json`. Remote immutable player/log/snapshot:
`/data/typinghero-game-loop/task-compact-tutorial/results/20261021T153008-webgl`.
Retain the preview/player and existing worktree for user review and tutorial WIP;
close/remove these task resources after review. Earlier menu/tutorial resources
are not owned by this task. Remote integration verification follows below.
