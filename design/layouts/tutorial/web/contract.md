# Interactive tutorial

Approved source: user implementation plan, 2026-10-17, six English lessons and
safe practice. [Shogun Showdown opening](https://www.youtube.com/watch?v=i6tP6bBTaZ0&t=29s)
is the selected short-instruction / arena-practice reference. Web fetch returned
cache miss in this session; the supplied plan owns the exact lesson flow.
Existing Shogun layout/theme and UGUI factory own geometry, colors and controls.

The existing 960x540 HUD, 928x340 arena and 672x88 typing tray stay anchored.
The tutorial prompt sits below the HUD at (144,80), 672x124. Its heading,
two-line instruction and Practice/Next/Start Battle plus Skip Tutorial actions
occupy the empty upper arena, above the existing characters. These dimensions
are an implementation estimate within the approved below-HUD placement.
Gold highlights the typing tray, actual yellow cue or enemy stamina for the
current lesson. No full-screen shade covers the characters. All elements scale
uniformly with the existing CanvasScaler at 640x360 through 1920x1080.

The menu's existing secondary row contains How to Play, Tutorial and Top 1000;
the existing name validation applies to Play and tutorial replay. No new form.
Reading stops combat and typing with a distinct reading lock. Enter/click begins
practice or advances a completed lesson. SPACE belongs to Parry. Pause/focus
loss can interrupt any phase and Resume preserves the reading lock. UI sounds
reuse the existing pause-safe SFX events. No new spatial motion is introduced.

Owner: TutorialBuild/TutorialOverlay/TutorialDirector. TutorialRun retains only
the current session checkpoint through clean scene reloads for retry/next lesson;
the real stamina-break to Slayer transition remains in the same live scene.
No lesson condition may be completed by clicking Next alone.

Revision2: actual native-GPU cue capture showed the opaque124px prompt covered
the upper part of the existing yellow exclamation mark. The dark background now
covers only the top70px behind heading/instruction; the existing two buttons keep
their own backgrounds and unchanged positions. Their middle gap reveals the real
cue and its highlight. This fixes the approved cue visibility without moving the
prompt,HUD,characters,actions or typing tray. Geometry and lesson flow are unchanged.
