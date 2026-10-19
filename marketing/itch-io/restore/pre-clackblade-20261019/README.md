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

## Applied release / rollback

Released CLACKBLADE at /clackblade, upload19871730. Previous upload19865461 remains
hidden, not deleted. Cover31073302, banner31073353, social image31073415.
To roll back: select19865461 playable, unhide it if necessary, hide the newer
upload, restore original title/description/cover/banner and social image30983638.
Restore original slugtyping-hero if available, then verify both URLs/public CDN.
Profile-bio.json and old three MP4s are also preserved here. Existing old remote
media URLs remain available. Theme comparison proves only banner image ID changed;
pricing remains0-or-donate/suggested1USD and tags remain unchanged.
Full applied manifest: ../../clackblade-release-20261019.json.
