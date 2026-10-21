# Player full palette

## Acceptance

Idle approved: charcoal hair, ivory scarf, dark clothing/boots; exact original skin and eyes. Extend to all 25 sheets / 166 frames and matching 2944×3200 master atlas. Preserve pixels/alpha/pose/order, all metadata/normal maps, sword/slash/VFX and authored hit/death overlays. Work from original source; reuse approved idle verbatim.

## Implementation and result

Child worktree: `C:/Users/ADMIN/orca/workspaces/TypingHero/hero-idle-palette`, branch `micharel09/hero-idle-palette`, base main/e20c4ab9db39471642a76a59c02328acd860e902, parent `C:/Users/ADMIN/orca/TypingHero`, origin `https://github.com/micharel09/TypingHero.git`.

Complete output: `design/player-palette/`, containing 25 PNG strips, 2944×3200 master atlas, 166 explicit L masks, source locks, material palettes, builder, standalone preview and PNG ZIP. Approved reference remains `design/hero-idle-palette/`. Source and destination atlas/strips retain exact geometry/alpha; approved idle is byte-identical after RGBA decode. Metadata, clips, secondary normal textures and all other Assets files are untouched.

Masks were reviewed across all 166 frames. Shared brown boot/skin components are fixed per pose in overrides; crouched Death boot/skin joins use explicit source-coordinate regions. White armor is separated from eyes, blade and slash components. Two red Hurt cels and three Death fade cels keep their source overlay contribution; only underlying selected material changes. Death 14–23 silhouette/dissolve cels are entirely unchanged. Extra movement-only scarf highlight and three original purple edge shades use the same ivory/gray hierarchy; one blue cuff edge pixel follows clothing gray.

Preview contains 9 real Unity sprite timelines, including cross-sheet Attack/Slayer/Parry Success, and 25 separately labeled 8-fps sheet inspections. Empty Player/player_parry_success.anim is not given a fictitious sprite timeline; actual art-folder timeline is available with its source path shown.

## Verification (2026-10-19)

- `python design/player-palette/build.py --author-masks`, then normal `build.py`: PASS, 25 strips / 166 cels; 48,615 changed pixels. Build asserts protected base colors, unchanged unmasked/hidden pixels and alpha; exact source checksums; same input cel cannot have inconsistent output; idle equals approved reference.
- Independent Pillow pass over saved files: 48,615 changed + 2,671,129 unchanged pixels, exact alpha, original skin highlights/eyes/weapon-only colors, exact Death 14–23, all 166 master cells equal corresponding output strips, empty cells unchanged. ZIP CRC and every archived file match outputs. This pass ran from task-temp; its conclusions are preserved here before cleanup.
- Viewed all 166 original/new cels in seven review contact sheets. Reviewed hands versus boots in attack/run/slide/climb and crouched Death; corrected pose selections and reviewed final frames. Reviewed actual dungeon browser rendering.
- Orca browser, fresh local file tab: all 50 source/output images decoded; all 34 sequences exercised with 236 frame visits. Passed sequence lengths, every timing boundary, scrub, stepping/wrap, non-loop Death stop and loop option. Deterministic delta200ms gives phase200ms at1×/50ms at0.25×. Reflected canvas matches every mirrored pixel;6× zoom is768×768 and smoothing is disabled; all three backgrounds pass. Browser captures are QA only and removed at handoff.
- `git diff --exit-code -- Assets`: PASS. No runtime logic/config/import/normal/light change. No Unity/game tests, build or release: separate asset-preview scope; Unity lighting/integration remains unverified.

## Handoff

User asked full set after idle approval. Optional preview-versus-integration question had no answer during independent work; retained prior preview-first workflow. Asset set is complete and ready for review; not integrated into game. Preserve this child worktree/branch because outputs await review and are not in main; no commit/push/merge/release performed. Remove task child only after approved outputs are preserved/integrated or task is explicitly canceled. Temporary review files under `E:/CodexTemp/TypingHero/player-full-palette` are cleaned before delivery.

## Integration and itch release — authorized, in progress

User explicitly requested replacing in-game sprites and publishing to itch.io. All 26 PNGs replaced in child; originals and original metadata now retained under `design/player-palette/originals/` for reproducible generation/rollback. Builders use this frozen source after integration. GUIDs, rects, pivots, curves, normals and lighting are preserved.

Parent integration is also complete: only the26 approved player PNGs were replaced after checking each original checksum and metadata against rollback copies. Both design source folders, this checkpoint and release ZIP are preserved in parent. Existing dirty font/UI/marketing work remains untouched. Parent sources now include the approved palette for any subsequent build; previous published uploads still have old colors until successful release.

Latest separately verified UI baseline: immutable `ed738d4e8bb83aeb61b66a4d5bd8ff8b8365648affa3dababd93228b6cf71e4a`, production154213 and native font QA documented in the parent page checkpoint. Captured before shared runner source replacement. The final player-color input `64de5b3ad3a3df3bc79ff56c159713ba777cd1ab25ffa0f08a81834ba7d6fcd7` differs from that baseline by exactly 26 approved player PNGs and nothing else. Source title-only candidate3bc764fd/production154730 was deliberately canceled and never published when the newer font baseline became available.

Final Unity2022.3.25f1 production **20261019T154825-production PASS**, qa=false. Native Edge155.0.4283.62 original-HTML smoke: four automated checks PASS, zero JS/managed exceptions and zero provider writes. Actual name PaletteQA, tutorial idle, real BLADE attack/score10/enemy850→840, original sword/slash, Skip Tutorial→Normal Swordsman, pause/resume and new palette inspected. Initial loader screenshot was Unity splash and was rejected as menu evidence. Existing unsupported-shader console messages match historical renderer behavior; actual lit sprite/menu/combat render is inspected.

ZIP: `Builds/CLACKBLADE-WebGL-PlayerPalette-20261019.zip`, 16,515,439bytes, SHA256 `b9c90c93ec9fee949d0192f4191fbb1696f9d7669aaac1b4996ca6bd1dee0c87`, 30members/CRC PASS. JS/WASM/data exclude typingHeroQA/GameLoopProbe/TYPINGHERO_AUTOMATION. Existing font provenance retained in CREDITS. Input and player are frozen; no full Quick/balance/performance or redundant QA build was needed for pixel-only delta.

Target verified: game3857509, https://micharel09.itch.io/clackblade. Last confirmed playable19877153. Original native upload input can be exposed from the site's own Upload files handler and selected using Orca upload; no private upload API or credential extraction. Actual upload attempt returned **Server error403**, and fresh native editor tab530e5d21-4a59-44c9-bffc-ab49d80c8f33 displays Cloudflare verification. No uploaded player ID or Save/publication claimed. Human verification requested asynchronously; do not retry upload before user confirms. Preserve prior upload for rollback and compare protected fields before Save.

The parallel font-only release uploaded candidate19878019 but its Save also hit Cloudflare, according to the parent checkpoint. Read fresh server/editor state after human verification. The player-palette candidate64de already contains the same verified font baselineed738d4e plus26approved images, so it is the complete current candidate; publishing the older font-only ZIP afterward would revert the new player colors. Do not blindly submit any stale editor form or reselect19878019 once the palette build is published.

Resume resources: `E:/CodexTemp/TypingHero/player-palette-release/` contains frozen source input, exact production player, ZIP receipt, actual native screenshots/report, safe editor before-state and helpers. Native smoke process closed normally. Orca editor14d7f609-da07-498a-a1f7-e9f96b3a9f2d still has native upload input and failed upload row; fresh editor530e5d21 has Cloudflare. Public tab07d648a8-9d7d-4b55-b76d-788e61b4e749. Browser-skill daemon was started in managed session12860 but has zero connected browsers and no browser session was created; upload continues through Orca's native input. Do not install/change browser settings or use Computer Use.


### Publication completed ? 2026-10-20

Published approved full player palette with existing all-UI font baseline to https://micharel09.itch.io/clackblade using BrowserSkill and the user-connected Edge profile. Native upload and Save succeeded: upload19891315; public iframe selects19891315. Fresh editor confirms persistence and previous19877153 is hidden, retained for rollback. All29 non-HTML CDN files match the verified release ZIP SHA256s; actual public Unity menu rendered correctly. Prior Cloudflare/pending notes above are historical and superseded. Receipt: `marketing/itch-io/player-palette-release-20261019.json`. No further build or source changes were needed.
