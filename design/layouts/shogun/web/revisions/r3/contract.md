# Shogun presentation contract

User-approved implementation plan, 2026-10-17: Shogun Showdown concept.
Reference: https://store.steampowered.com/app/2084000/Shogun_Showdown/
Use its Japanese layered environment, dark foreground, restrained UI and target-local feedback as visual direction. No source game code/art is copied.
Assets: https://lyniadesign.itch.io/tiny-pixel-japan-parallax-background and https://kenney.nl/assets/ui-audio.

Revision 3 moves score to the typing region; revision2 is preserved in revisions/r2.
Geometry explicitly approved in user implementation plan2026-10-18: 960x540,
arena928x340; status112px wide/top12px below foot, name16/HP6/stamina4.
Score row top8px below the lowest status, fixed48px tall independent of text/pulse.
Score/multiplier font24, gap8; decay below. Tray672x56 centered8px below row.
Popup12px to the right, font18/rise12; existing lifetime/fade/tints/zoom preserved.
Score and tray follow rendered camera; floating awards follow their current anchor.
Normal and Slayer share this hierarchy; Slayer retains red/full-width black bars
and8px body gaps. Wireframe body positions are illustrative; runtime projects the
fixed collider landmarks through the rendered camera. Long names auto-fit within
112px; no gameplay text is removed. All three words/timer/error feedback remain.
Reference for grouping: https://goblinzstudio.com/wp-content/uploads/2022/12/Sho_1-768x430.jpg
User explicitly approved both variants in the implementation plan.
CombatHUDLayout owns group/tray/score/popup positions; SlayerScreenFX owns bars,
red/environment and camera, sharing CombatHUDLayout standing-body landmarks.
ScoreUI_SlayerBinder and ScoreUI_Flash keep scale ownership; existing sliders,
font/colors and bindings are reused. Tutorial highlights follow relocated targets.
Arena and tray have distinct regions; the leaderboard
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
