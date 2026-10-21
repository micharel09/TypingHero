# CLACKBLADE profile — implementation receipt

2026-10-19. Target: https://micharel09.itch.io/. Status: published; acceptance partially verified.

## Implemented

- [x] Backed up original bio, empty display name, default avatar state, theme, layout, collection visibility; no banner/background existed.
- [x] Native account editor: display name CLACKBLADE, 256×256 sword avatar. Username/URL unchanged. Fresh profile navigation shows both in account header.
- [x] Native theme editor: CombatR7 dungeon banner 960×300, CLACKBLADE / TYPE TO SLASH, dark palette, Lato, Sidebar retained.
- [x] Exact approved English introduction, play link, Enter the duel and Follow development sections; responsive gameplay GIF from released game page, 560px maximum.
- [x] Saved bio verified by fresh navigation. Redactor HTML source required a real key event before toggling back to rich text; checked backing textarea before final Save.
- [x] Desktop 1440×1000: banner whole, bio left, game card right, CTA and GIF visible. Mobile 390×844: banner whole, GIF fits 350px content area, text wraps, game card follows bio. Viewed screenshots directly; avatar remains readable at 32×32.
- [x] Play href points to `/clackblade`. Comment href matches native game Comments link `/clackblade#comments`; existing game has its comments widget. No comment submitted.
- [ ] Guest runtime: fresh cookie-free Playwright Chromium on fly-server encountered Cloudflare `Just a moment...`; Follow, guest links and community avatar/name propagation remain unverified. No challenge bypass attempted. Public web reader also could not access target.
- [ ] Native browser zoom 200%: synthetic Ctrl zoom shortcuts did not change browser zoom, so native zoom remains unverified. Desktop/mobile checks do not substitute for it.

## Sources, files and recovery

Approved references: [Terry Cavanagh](https://terrycavanagh.itch.io/) and [scumhead](https://scumhead.itch.io/). Source mapping, asset rebuild and rollback steps: [profile README](../../marketing/itch-io/profile/README.md). Published asset URLs and SHA-256 receipts: [published.json](../../marketing/itch-io/profile/published.json). Original settings: [backup](../../marketing/itch-io/profile/backup/account.json) and [theme](../../marketing/itch-io/profile/backup/theme.json).

No game-page editor, playable, pricing, tags, Firebase, social or devlog writes. Existing dirty `docs/plans/endless-duel.md` preserved. No push/PR/deploy of repository changes. Runtime gameplay tests are outside this profile-only change.

Remaining acceptance: check the public profile in a guest browser past the Cloudflare challenge, confirm native Follow and play/comments navigation; confirm avatar/name on community surfaces; inspect native 200% zoom. Restore old state through native editors using README if rollback is needed.

Validation: PNG dimensions and JSON parse passed; sanitized backup contains no CSRF fields. Browser session closed and task captures/scripts removed from E:/CodexTemp/TypingHero/itch-profile. Native zoom and final reflow capture attempts did not provide usable evidence; existing desktop/mobile screenshots were reviewed before cleanup.
