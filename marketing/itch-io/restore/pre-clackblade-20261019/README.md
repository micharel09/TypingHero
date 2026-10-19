# Pre-CLACKBLADE rollback state

Captured read-only from the authenticated editor and public page, 2026-10-19.
Owner micharel09; game3857509; https://micharel09.itch.io/typing-hero.
Current playable19865461; cover31068886; banner30983627 (960x394).
`page-state.json` contains non-secret project field values, media URLs and upload
visibility; `description.html` preserves the actual current public description.
Older uploads remain in itch.io. No remote changes were made when this backup was taken.

Rollback after a future release: restore title `Typing Hero: Type to Slash` and
slug `typing-hero` if available; select upload19865461 as playable, keep the newer
upload retained/hidden, restore description and cover/banner referenced in JSON.
Restore cover from `../../../ItchIO_Optimization/Thumbnails/cover-a-630x500.png`
only after checking it still matches image31068886. Backed-up `banner.png`
is the old banner; check960x394 before replacing. Restore theme/pricing/tags only
if the saved receipt proves they changed. Save and verify the public page, actual
playable upload and old/new URLs; a successful click is insufficient.
