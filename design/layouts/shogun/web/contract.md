# Shogun presentation contract

User-approved implementation plan, 2026-10-17: Shogun Showdown concept.
Reference: https://store.steampowered.com/app/2084000/Shogun_Showdown/
Use its Japanese layered environment, dark foreground, restrained UI and target-local feedback as visual direction. No source game code/art is copied.
Assets: https://lyniadesign.itch.io/tiny-pixel-japan-parallax-background and https://kenney.nl/assets/ui-audio.

Revision4 uses Type Knight as primary HUD reference:
https://chaikadev.itch.io/type-knight/devlog/70022/type-knight-demo-4
Approved user implementation plan2026-10-18; reference-only, existing project assets reused.
Unframed normal total font24 top/right24 at960x540, fixed screen scale;
combo font16 below, hidden x1, existing tiers/decay and120ms award pulse.
Status112px wide/12 below foot: no names, HP6 red at top, stamina4 Gold(#DCC47C)
with2px gap. Player status height6, enemy12; tray672x56 centered12px below lowest status.
Single +delta popup font18 at projected enemy head right24/up12, max rise18;
hold150ms/fade450ms after last award. New awards accumulate exact received delta,
restore alpha without creating another instance. Pause holds time; lethal freezes
last available anchor and lets fade complete; restart clears popup and accumulation.
No changes to score rules/events/dependencies. Geometry above is user-specified;
wireframe body positions are illustrative, not measured runtime coordinates.
Normal/Slayer share hierarchy; existing red/full-width black bars/body gap8 remain.
CombatHUDLayout owns positions; ShogunTheme migrates in place preserving bindings;
ScoreUI_Flash owns award pulse. Camera scale binder is disabled for this HUD.
FloatingScore owns elapsed/rise/alpha, ScoreSystem retains the single popup instance.
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

Revision5, approved supplied user plan (2026-10-18): Slayer is the explicit
layout exception. Center the total48/combo24 group in the upper black frame,
horizontally and vertically using actual glyph bounds including the120ms pulse.
Combo gap4, x1 hidden; decay centered below. Interpolate position/fonts with the
existing220ms bar reveal, including interrupted entry/exit. Restore normal24/16
and margins24. Canvas scale1, existing font/tier colors, enemy popup and tray.
Geometry is user-specified; body/frame positions in wireframes remain schematic.


## Endless difficulty selector (2026-10-19)

Reuse the existing Shogun flat Button/feedback, font and jade/coal/ivory palette.
MenuPanel is (264,24,432,364) on the960x540 reference. Name stays above Difficulty;
selector is (24,204,384,40), hint(24,246,384,24), Play(24,280,384,40).
The single keyboard stop is the center level Button: Tab/ShiftTab reaches it,
Left/Right cycles, pointer arrows return focus there. Selector actions keep menu open.
Hint describes enemy pace and effective combo seconds. Normal is the default.
The community modal panel is (180,0,600,540), selector(16,52,568,40), header y96,
list y128 with the existing404px viewport/28px rows. Filter is independent of menu/run
and hidden in Classic/profiles/achievements. Intro, combat run line and results show
the captured run difficulty. Required review sizes are960x540 and640x360.
