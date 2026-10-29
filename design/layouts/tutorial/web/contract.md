# Interactive tutorial - new-player review candidate

Current authority: user-approved six-lesson implementation plan, 2026-10-27.
This section supersedes the historical contract below. Native visual and real-input checks at1280x720 and640x360 passed on2026-10-27.
This records implementation acceptance, not an independent novice usability study.

## Flow and controls

Keep six logical pages: arena, typing, mistakes/timer, parry, Guard/Slayer, score.
Arena introduces HP, the word tray and enemy HP one at a time using Next; only one
callout/arrow is visible. Guard/Slayer/score explanations belong to later pages.
Demo playback is opt-in, one pass, with a terminal hold and Replay. DEMO / WATCH
ONLY stays outside the captured gameplay. Demo letters and Space are blocked.

Typing and parry have required YOUR TURN phases. HIT uses the real typing/attack
path, no timer and an idle opponent; completion requires a real damaging hit and
finished recovery. Parry uses one real first-Swordsman strike per Try/Retry, original
cue/window/recovery, no HP loss and no letter input. Misses remain retryable; three
parry misses retain the release/wait-for-fill reminder. Success never auto-advances.

Enter activates the primary action and requires release across state changes.
Space belongs only to live parry practice. Back cancels the attempt and resets the
page; passed skills remain in this session. Restart begins again at page1.
Skip tutorial is a separate persistent action and loads a fresh battle. Completed
is saved only by final Start Battle with both skills passed. Skip does not downgrade
an earlier Completed replay. Menu abandonment does not save progress.

## Composition / reuse mapping

TutorialBuild's existing Back/button skin -> Watch, Skip and practice buttons;
existing clip-focus gold edges -> one live tray/Guard outline; real game HUD/actors
-> practice arena. Keep the project font/material, original clip PNGs, GUIDs,
SpriteRects, captions, guidance and durations. No new visual library or art assets.

All bounds use the existing960x540 reference and CanvasScaler:

- Demo panel: (16,12),928x516; complete640x360 clip at global(160,64).
- Footer: Back(48,492),130x36; Watch/Replay(196,492),250x36;
  primary(464,492),280x36; Skip(760,492),180x36.
- Practice hides the video modal and reveals the real arena. Goal text:
  (204,100),552x72,26em; mode/safety badge:(24,110),160x100,20em.
- Practice Back:(24,370),160x48; primary:(776,370),160x48. These occupy the
  outer gutters above the typing tray. Common Skip stays in the footer.
- Live parry cue keeps its authored head anchor. No practice control covers it.
  Live focus follows actual HUD geometry; it is hidden during a parry attempt
  and highlights Guard only after success. Cancelled attempts hide the live cue.
- Rules use24em rather than18em; short lines explain correct letters in Slayer
  and points plus the five-word multiplier. The last page shows both skill checks.

Validate actual glyph bounds, cue/tray clearance, controls and six pages at1280x720
and640x360. Do not infer legibility from reference dimensions alone.

## State / resource ownership

Director owns lesson, phase transitions and session skill flags. TutorialPractice
owns real combat events and cancellation. Overlay owns presentation and streamed
clip resources. Practice releases only the reading lock; ordinary Pause is retained.
Practice does not accrue real score/statistics, records or unlocks. Final Start/Skip
reloads a clean selected-difficulty encounter. No public gameplay/storage migration.

Clip resources retain two slots, one outstanding request, safe Image/CommitDisplay
handoff and zero owned textures after cleanup/late completion. Do not prefetch a
loop at the terminal atlas. Held playback/loading frames do not dirty Image again.
Normal builds exclude the read-only QA observation/trace bridge.

## Historical contract (superseded)

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
