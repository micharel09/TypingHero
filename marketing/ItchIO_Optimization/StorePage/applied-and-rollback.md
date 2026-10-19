# Publication receipt and exact update procedure

Verified 2026-10-19, approximately 06:00 UTC / 13:00 Asia/Saigon.

Owner editor: https://itch.io/game/edit/3857509
Public: https://micharel09.itch.io/typing-hero

The user authorized direct store improvement. Native editor saves were used. The public page subsequently rendered the new title, opening `TYPE TO ATTACK. SPACE TO PARRY. BREAK STAMINA TO UNLEASH SLAYER.`, explicit desktop Run game CTA and complete credits. Source-mode edits needed a native keyboard input in the rich editor to synchronize the saved description; public text was checked after saving, so this receipt is not based only on an unsaved editor preview.

- Cover A accepted as image **31068886**. The actual search card displayed the new cover, title, tagline, Action and Play in browser.
- New gallery: **31068982** type attack, **31068983** parry cue, **31068984** tutorial, **31068981** typing GIF. Old gallery images 30982787/88/89/90 removed from this page's gallery. Originals remain in existing `marketing/itch-io/` assets.
- Same GIF in description: https://img.itch.zone/aW1hZ2UvMzg1NzUwOS8zMTA2ODk4MS5naWY=/original/nQ9RJP.gif
- All three gallery PNGs, gallery GIF and inline GIF reported loaded with nonzero natural width; page and search screenshot visually inspected. Final cover B also inspected at 315x250.
- More information publicly reports Action, HTML5, Keyboard, English, A few minutes, Interactive tutorial, nine configured tags, AI Assisted / Text. Duration is a creator category, not measured average retention.
- Existing banner, theme and donation settings retained. No devlog or social post published. No creator contacted.
- **Playable unchanged:** https://html-classic.itch.zone/html/19865461/index.html?v=1792376251 . Native embed remains click-to-launch maximized. A generated itch owner notification mentions the existing ZIP; no ZIP was uploaded, replaced or activated in this task.

## Reapply manually if needed

1. Open the editor above; copy title and tagline from adjacent `.txt` files.
2. Paste `description.html` through the native HTML toolbar; return to rich text, make a harmless native edit/undo if necessary, then Save and verify the public opening and credits.
3. Replace cover with `../Thumbnails/cover-a-630x500.png`. Use B only as a separate deliberate experiment.
4. Add screenshots in the order above. The included GIF is in `../SocialMedia/typing-live-preview.gif`. Use the upload's actual CDN address if it changes; do not invent one.
5. Select genre/tags from `configuration.json`. Metadata > Classification: A few minutes, Keyboard, English, 1-1 players, Interactive tutorial. Keep HTML/Public/current playable. Leave trailer field blank until hosted.
6. Save, inspect public page, search card, image loading and unchanged embed; do not repeatedly launch your live build for marketing QA.

## Rollback

Paste `before-description.html`, restore `before-cover.png`, title `Typing Hero`, old tagline `Type to slash, press Space to parry, and chase the high score.`; replace High Score with Indie if reverting tags. Old media references and assets remain in `marketing/itch-io/`; restore those only if intentionally reverting to the old visual snapshot. Previous duration was A few seconds and accessibility was empty. Do not reinstate inaccurate No-AI disclosure for the new AI-assisted text. No game-file rollback is necessary: the playable was unchanged.

Search presence was rechecked, but no ranking improvement, CTR uplift, additional strangers or retention improvement has been measured.

Local validation: nine PNG/GIF files fully decoded with Pillow; both 630x500 and 315x250 export sizes verified, three screenshots 960x540. GIF optimizes 31 source samples to 17 encoded frames, total 2520 ms. JSON parsed, local Markdown/HTML references resolved, and new text passed trailing-whitespace checks. Offline capture report: zero leaderboard reads/writes, one blocked external request, no JavaScript exceptions. No claim of successful fresh Slayer capture. Browser session mwoo stopped after one bounded cleanup retry; shared daemon retained.

Task-owned temporary capture directory E:/CodexTemp/TypingHero/itch-optimization was checked for path/reparse safety, removed, and confirmed absent. Product exports remain in this package. Earlier tasks' QA directories and shared tooling were preserved.
