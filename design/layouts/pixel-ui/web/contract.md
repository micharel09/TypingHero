# Pixel UI layout, revision 1

Source: the user-provided implementation plan, 2026-10-16. Desktop keyboard WebGL, Unity 2022.3.25f1 UGUI/TextMeshPro. Geometry below is specified by that plan; wireframe font sizes and placeholder bars are illustrative, not production tokens.

Reference assets: [Crusenho UI Essential Flat](https://crusenho.itch.io/complete-ui-essential-pack), free Flat variant, CC BY 4.0; [Kenney Input Prompts Pixel](https://kenney.nl/assets/input-prompts-pixel); [VT323](https://fonts.google.com/specimen/VT323). These are selected sources, not assets already imported or verified in Unity.

Fixed layout: 960×540 reference canvas, 16 px outer margins, 8 px gap, 232 px leaderboard column. Combat occupies the remaining 688 px. Camera viewport and Slayer effects stay in the combat region. HUD does not follow camera zoom. Leaderboard remains visible beside combat and has separate rank, name and score columns with truncation and a visible scrollbar.

HUD order: player name/HP; score/combo; opponent HP/stamina. Each health/stamina label identifies its owner. Typing rail sits above characters, with the timer adjacent. Current word uses teal, next words use decreasing brightness. Existing arena, characters and combat effects are retained; image blocks in the wireframe indicate their reserved viewport, not replacement artwork.

Menu: one centered cluster containing title, framed name input, validation error and Play. Preserve existing naming rules and saved identity. Overlay: a shared panel in the combat region; Resume appears only while paused, Restart/Main Menu use existing controller actions. Game over and demo completion use the same panel with their existing titles and no Resume.

Small windows: uniformly scale the reference composition; do not move the leaderboard over combat. 640×360 is the small-window preview. 1280×720 and 1920×1080 retain identical proportions. All preview content is illustrative fixture data and has no Firebase connection.

Palette intent: charcoal background, blue-gray panels, ivory text, red HP, yellow stamina, teal typing. Final pixel sprites must use Point filtering, no mipmaps and 9-slice borders. Vietnamese atlas coverage must be verified after importing VT323; these HTML wireframes do not prove font coverage.

Approval: draft; the plan explicitly requests one preview review before scene edits. Scene implementation and runtime verification have not started.
