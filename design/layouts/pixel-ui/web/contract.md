# Pixel UI layout, revision 2 — Dark Ages

Source: the user-provided implementation plan, 2026-10-16. Desktop keyboard WebGL, Unity 2022.3.25f1 UGUI/TextMeshPro. Geometry below is specified by that plan; wireframe font sizes and placeholder bars are illustrative, not production tokens.

Reference assets: [Dark Ages UI v1.0 by Hypnobius](https://hypnobius.itch.io/dark-ages-ui), original 384×352 tilesheet and license in Assets/Art/UI/DarkAges; [Kenney Input Prompts Pixel](https://kenney.nl/assets/input-prompts-pixel); [VT323](https://fonts.google.com/specimen/VT323). Dark Ages use is authorized for commercial/personal games; do not redistribute it as an asset pack.

Fixed layout: 960×540 reference canvas, 16 px outer margins, 8 px gap, 232 px leaderboard column. Combat occupies the remaining 688 px. Camera viewport and Slayer effects stay in the combat region. HUD does not follow camera zoom. Leaderboard remains visible beside combat and has separate rank, name and score columns with truncation and a visible scrollbar.

HUD order: player name/HP; score/combo; opponent HP/stamina. Each health/stamina label identifies its owner. Typing rail sits above characters, with the timer adjacent. Current word uses teal, next words use decreasing brightness. Existing arena, characters and combat effects are retained; image blocks in the wireframe indicate their reserved viewport, not replacement artwork.

Menu: one centered cluster containing title, framed name input, validation error and Play. Preserve existing naming rules and saved identity. Overlay: a shared panel in the combat region; Resume appears only while paused, Restart/Main Menu use existing controller actions. Game over and demo completion use the same panel with their existing titles and no Resume.

Small windows: uniformly scale the reference composition; do not move the leaderboard over combat. 640×360 is the small-window preview. 1280×720 and 1920×1080 retain identical proportions. All preview content is illustrative fixture data and has no Firebase connection.

Skin: charcoal brown #2E322A, ivory text #F4EEDA, gold titles #DCC47C. Menu/help/pause/result and leaderboard use ornamented gold frames; HUD, typing and rows use the quieter dark inset. Keep current/next typing colors and alpha. Dedicated button normal/selected/pressed sprites, gold bar endcaps with red HP/gold stamina/blue timer and combo fills, and dedicated vertical scrollbar track/thumb. Original slices use Point, no mipmaps/compression, and full 9-slice borders. Ornamented panels and the gold fill tile their edge/center texture instead of stretching the pattern; native scale is retained, with the sidebar at half scale to fit its existing padding. Quiet row backgrounds use a separate source slice. No inventory or additional feature from the source pack.

Apply skin after core-match construction to existing objects including inactive help/results, then persist scenes and shared prefabs. Preserve controller identities, serialized references/listeners, focus/navigation, validation and leaderboard scroll/virtualization. Repeated application must be idempotent.

Approval: user approved the Dark Ages dark background/gold-bronze preview and implementation plan on 2026-10-16. Geometry is retained from the approved prior layout. Dark Ages isolated Chromium runtime/layout checks passed; exact native Edge61a46c05 acceptance remains pending because the instance is disconnected. Evidence and production ZIP receipt are in docs/plans/pixel-ui-refresh.md.
