# Shogun presentation contract

User-approved implementation plan, 2026-10-17: Shogun Showdown concept.
Reference: https://store.steampowered.com/app/2084000/Shogun_Showdown/
Use its Japanese layered environment, dark foreground, restrained UI and target-local feedback as visual direction. No source game code/art is copied.
Assets: https://lyniadesign.itch.io/tiny-pixel-japan-parallax-background and https://kenney.nl/assets/ui-audio.

Geometry comes from the user plan, not measurements of Shogun: 960x540, HUD60,
arena928x340, tray672x88. Arena and tray have distinct regions; the leaderboard
is absent from the combat HUD. Keep uniform centered scaling for 640x360,
1280x720,1920x1080 and iframe/fullscreen; expand letterbox outside 16:9.
Leaderboard reuses the existing 232px panel, columns, row pool and provider.
Open from menu/pause, modal Close returns to prior screen/focus; pause remains paused.
Escape closes the modal without reaching pause, R/M remain blocked; refresh and
scroll retain existing data semantics and all1000 rows. Menu/help/result/pause
reuse current controls and layouts, jade/coal/ivory with vermilion HP/error.
Buttons: visible focus/hover, label depresses1px; panel reveal120ms unscaled.
Typing border flashes180ms on incorrect input/timeout; text does not move.

Owner: Assets/Editor/PixelUI/ShogunTheme.cs migrates existing UGUI in place;
runtime owners LeaderboardModal, UIButtonFeedback, PanelReveal,
TypingPanelFeedback. Reuse PixelUI primitives and existing Vietnamese font.
Combat geometry, player/skeleton animation/colliders,40/850HP,typing/scoring and
Firebase schema remain owned by their current modules.
