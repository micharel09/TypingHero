# Typing Hero — itch.io discovery

## Cover v2 — gameplay điện ảnh — 2026-10-19

- Theo kế hoạch user giao: thay riêng cover bằng cảnh đấu kiếm tràn khung,
  asset thật, logo TYPING HERO và TYPE TO SLASH. Hướng/reference đã chốt;
  không sửa game, banner, copy, metadata hay upload playable.
- Nguồn chỉnh sửa và provenance/rollback:
  [cover-v2.html](../../marketing/ItchIO_Optimization/Thumbnails/cover-v2.html),
  [hướng dẫn](../../marketing/ItchIO_Optimization/Thumbnails/cover-v2-README.md).
  Xuất PNG 630×500, 315×250 và 158×125 cùng thư mục; A/B cũ giữ nguyên.
- Đối chiếu screenshot production 01-type-to-attack và sprite gốc: hero combo1
  cel đầu ×7, skeleton trắng sword Attack1 cel đầu mirror ×6, background Tiny
  Pixel Japan ×3. Chỉ crop alpha bounds; giữ nguyên pose/weapon/palette sprite;
  nền giảm sáng/chuyển tông xanh đen. Không AI, dependency mới hay sửa asset gốc.
- Edge headless/Playwright có sẵn, viewport630×500/DPR1/font ready: đã xem ảnh
  chính và hai bản nhỏ nearest-neighbor. Logo đọc được, hai nhân vật/khăn tím/
  cung kiếm rõ, không HUD/viền/tutorial/footer; không kéo méo. Đã xem cạnh hình
  nhận diện của ba reference trong vùng315×250, giữ tỷ lệ reference. Đây là
  graphic quảng bá, không phải screenshot nguyên trạng hay kết quả thử CTR.
- 8/8 đường dẫn asset/font tồn tại; ba PNG decode và kích thước đúng; không
  trailing whitespace trong source/hướng dẫn. Không chạy Unity tests.
- SHA256 PNG630: `d0df90e7f2366df283b16bdd6f8227ab48734d45265d8bfb6f33f821e6f2cf2f`;
  PNG315: `125d412405d3f85886fc78fb1da22fc74103a461c6731b18836adb8488471be3`;
  PNG158: `67e8c9a107585696e1a2489fa11f601516931d81424f145c372cffc2f0c10978`.
- Publication chưa nghiệm thu: đã xác minh owner micharel09/game3857509.
  Upload đầu tạo image31069803 và preview315×250 loaded, nhưng native Save gặp
  lỗi CDP frame graph; fresh editor xác nhận vẫn cover31068886 và28 game fields
  giống trước task. Phiên Edge mới pjsm upload lại, sau đó Save timeout với
  `effect_state=unknown`; chưa kết luận lưu thành công. Đã gọi human help60s
  theo browser-skill sau các lỗi thao tác, không suy timeout là quyền mới.
- Còn lại: xác minh saved cover và search card; chỉ báo publish hoàn tất khi có
  bằng chứng. Rollback về `cover-a-630x500.png` (cover31068886), không dùng mốc
  before-cover cũ hơn. Không branch/worktree/commit/push; giữ dirty checkout.
- Task temp `E:/CodexTemp/TypingHero/cover-v2`: giữ field receipts để đối chiếu
  khi tiếp tục và render.cjs để tái xuất. Human help đã timeout, không có xác nhận
  thao tác; publication/search card vẫn chưa kiểm chứng. Dừng phiên browser riêng;
  dọn comparison/reference/capture tạm sau khi ghi kết luận, giữ bản PNG sản phẩm.

## Guest-name and 14-day campaign ? 2026-10-19

- Implemented: PlayerIdentity.EnsureLoaded saves valid GuestXXXX via TrySetName
  only when no name exists. Existing names, validation, PlayerPrefs key, UID,
  provider/schema, menu controls, six tutorial lessons/Skip and combat stay intact.
  Existing dirty Endless/HUD/audio changes preserved; no branch/worktree/commit/push.
- Final frozen input:
  `d391738b0cb5fd9163c3e8b287810f7412c4a6e3927cfebbee8755393293cbe7`.
  QA input24ab98cb3c87e0c96eba1f52ba26c0cdfd76ae1d9016375f4f1bfc501b3dd375
  differs only in the corrected test expectation, not runtime/assets/config.
  Current product inputs match; later edits to difficulty_ui.cjs/endless.cjs
  are unrelated harness changes and remain untouched.
- Unity Quick044730:96/97pass; only new test expected `New hero` instead of
  existing `New Hero` normalization. Corrected test; final focused045715:4/4pass.
  Reused93 other unchanged passing cases. No full-final-Quick pass claimed.
- Native Edge155/D3D11,960x540: fresh guest/Enter/Play, first tutorial/Skip,
  match restart/menu/reload, edited empty/short-name validation, custom name
  persistence and replay verified. Final tutorial/restart/Skip/menu segment9checks
  PASS. Guest7279/301 and NewHero/10 payloads observed in intercepted fixtures;
  no real provider writes. Equal-best runs correctly need not write a new payload.
  Failed driver attempts exposed timing assumptions: recording past Slayer,
  result reveal before Menu, and stale pre-reload tutorial state. Corrected the
  harness, without changing gameplay. Do not call those combined attempts passes.
- Production050328 PASS; JS/WASM/data ASCII/UTF-16LE QA-marker scans PASS.
  Original HTML guest/Enter/tutorial BLADE hit/score10 inspected; QA undefined,
  zero exceptions/writes. Deliverable:
  [WebGL ZIP](../../Builds/TypingHero-WebGL-Guest-20261019.zip),30files/16,457,704bytes.
  SHA256 `061cc7f2e6af3236a5f0115e762c82c8486e4e9f49f796b9683f42bb4fd678d7`;
  server/local SHA256 and ZIP CRC agree. Build metadata:Builds/guest-build.json.
  Broader existing Endless/audio release limits remain in their own checkpoints;
  no new balance/performance/audio-quality claim.
- Three Shorts from one continuous35s native-keyboard run of the verified QA
  runtime, not old storefront footage. Type cut0?12.48s; parry3?15.48s; Slayer
  3.644?16.124s includes its real parry/stamina break around6.14s. Overlapping cuts
  are intentional. Passive CDP full-frame JPEG capture,25fps resampling, silent
  VP8 intermediate/H.264 final; no gameplay manipulation, synthetic action/audio
  or score editing. Full frame plus enlarged unaltered tray detail; captions
  outside HUD. Each1080?1920/yuv420p/312frames/12.48s; ffprobe/full decode PASS.
- Clip SHA256:
  - short-01-type.mp4:8bb2d81de6a7d73e41387e70cdf70a78852d85ac6b83cff6168f0fefe9fa60c3
  - short-02-parry.mp4:608b33fce9001941d1fea29b736cf670cefec4afe9748b49b4478c5f54f6173f
  - short-03-slayer.mp4:0bdb0114e835542798d726e72a9a0175cea53ed532f33222eb23c787818618db
- [Campaign kit](../../marketing/itch-io/discovery-14-days.md): three English
  titles/captions/desktop-keyboard CTAs, Get Feedback draft, day0/3/7/10/14 manual
  schedule and measurement table. Local devlog now accurately describes the guest
  candidate. Cover/layout untouched; no posts, creator contact, remote devlog
  edit or itch upload. YouTube/Get Feedback guidance checked2026-10-19.
- Supplied observation211Views/45BrowserPlays,29Facebook referrals/30days and no
  ratings/collections/comments/followers retained, not freshly checked/day0.
  First actual Short starts day0; learning target50non-QA visits/+10plays, not a
  promise or unique-player/retention measurement. No14-day results invented.
- All three final MP4s played from start to ended on native Edge at360?640;
  each reports12.48s/1080?1920/error=null. Full FFmpeg decode passed; five timeline
  frames per clip plus full-size/phone detail inspected: words/CTA readable,
  typing/parry/Slayer match the game and captions do not cover HUD. Downloaded
  MP4 hashes match server exports. No audio-quality claim (exports are silent).
- Local `E:/CodexTemp/TypingHero/itch-discovery-14d` and server
  `/data/typinghero-game-loop/task-temp/itch-discovery-14d` removed after receipts.
  All task browser/processes exited. Build ZIP/manifest and product MP4s retained.
- Auto-review rejected deleting broader test/production result records, including
  a retry after inventory and escalation. The safer deletion of task-temp only
  succeeded. Retain these four directories under `/data/typinghero-game-loop/results/`
  until explicit user approval to delete their XML/log/snapshot/QA-player records:
  `20261019T044730-quick`, `20261019T045130-webgl`, `20261019T045715-quick`,
  `20261019T050328-production`. No captures were committed. Shared runner/cache,
  other tasks, original checkout and all pre-existing dirty changes remain.
  No deployment or community publication.

## Score-near-tray publication — 2026-10-18

- User yêu cầu đẩy bản HUD r3 lên itch.io. Đích giữ nguyên https://micharel09.itch.io/typing-hero, game3857509; giữ mọi upload cũ để rollback, cấu hình maximized/copy/theme/pricing và devlog Draft.
- Input production `e49808a9fe7f779d2bec93160e240e07362bdc002aadec015b3d30a7a56489db` khớp snapshot cuối task HUD: Quick52/52 và targeted WebGL166 checks còn hiệu lực. Production20261018T064806 PASS; QA-exclusion, smoke production, ZIP/CRC/hash, upload/save và đối chiếu payload public đã hoàn tất.
- Temp task `E:/CodexTemp/TypingHero/itch-score-release`, server `/data/typinghero-game-loop/task-temp/itch-score-release`. BrowserSkill dùng Edge61a46c05, sessionehel; daemon foreground do host quản lý. Không branch/worktree/commit.
- Production PASS, JS/WASM/data không có QA markers. ZIP28files/15,957,828bytes, CRC và server/local SHA256 khớp `237151c63e41baa2a64676d026d2e1b19fd6e6383d8f70a61500251c8272d558`; release giữ tại `/data/typinghero-game-loop/releases/score-hud-20261018/` cùng manifest.
- Exact production HTML smoke: Chromium153/SwiftShader,2CPU/8GiB/shm1GiB/network none; native tên/Enter/BLADE tạo hit thật và score10/ATTACK COMPLETE; pause/resume/next lesson và render640×360 được xem ảnh. QA bridge undefined,5fixture reads/0writes/0exceptions.
- Đã upload và chọn `TypingHero-WebGL-ScoreHUD-20261018.zip`, ẩn bản HUD trước, giữ cả bốn upload cũ. Native Save hiển thị alert Saved; form sau save xác nhận bản mới playable, bốn bản cũ hidden và embed maximized. Không thao tác copy/theme/pricing/devlog.
- Public Run game mở menu thật ở https://html-classic.itch.zone/html/19840745/index.html, ảnh menu đã xem. Anonymous CDN verification: 27/27 file ngoài HTML khớp release.json SHA256; HTML chỉ thêm script `static.itch.io/htmlgame.js`. Receipt `published.json` giữ cùng release ZIP/manifest trên server. Không gõ/chơi hay ghi provider trên bản public.
- Owned BrowserSkill session đã đóng; giữ shared daemon. Cleanup task-temp/captures và result production sau receipt; release đã publish giữ nguyên để rollback. Không branch/worktree/commit/push Git.

## HUD build publication — 2026-10-18

- User requested publishing the completed HUD update to the existing Typing Hero itch.io project. This authorizes the production WebGL build, ZIP/upload and saved playable selection; the earlier HUD task's release deferral is superseded for this publication.
- Target: https://micharel09.itch.io/typing-hero, game3857509; verify owner/current uploads before writing. Preserve previous uploads for rollback and retain native maximized embed/theme/copy/pricing. Devlog1712170 remains Draft; no community/media redesign scope.
- Build input1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588 matches verified HUD product source. Production runner started20261018T044548; Quick47 and HUD keyboard/four-size receipts remain valid from the preceding task.
- Temp E:/CodexTemp/TypingHero/itch-hud-release and /data/typinghero-game-loop/task-temp/itch-hud-release. Keep dirty checkout; no branch/worktree/commit/push needed.
- BrowserSkill restored existing Edge instance61a46c05 in owned Agent Window sessionpemx. CAPTCHA help timed out; a fresh observation then reached the authenticated editor and verified micharel09/game3857509. Publication proceeded on the user's explicit request, independent of the help timeout.
- Production20261018T044548 PASS, QA bridge excluded; all Build JS/WASM/data scanned for QA markers. ZIP28members/15,981,634bytes, server CRC PASS and downloaded SHA256 match: be81e04bb9163ee4cda8c46e16fac12e50e8d976e27761d8986fc947e2cbdeee. Server release: /data/typinghero-game-loop/releases/hud-20261018/TypingHero-WebGL-HUD-20261018.zip; release.json holds per-file hashes. Local upload copy is in the task temp above.
- Exact production HTML on Chromium153/SwiftShader,2CPU/8GiB/network none: menu/name/first tutorial/real BLADE hit (score10, ATTACK COMPLETE), pause and actual640×360 render inspected; QA bridge undefined. Resume/small HUD smoke completed below.
- Production smoke complete: original HTML renders1280×800 and640×360; menu/name, real BLADE attack/score10/ATTACK COMPLETE, pause/resume and next lesson's three-word compact HUD inspected.17fixture reads/0writes/0exceptions, bridge undefined throughout. Browser/container stopped after writing receipt. Runtime captures removed after recording receipts; server release ZIP/manifest retained as the published artifact.
- Publication complete: uploaded TypingHero-WebGL-HUD-20261018.zip, selected browser-playable, saved and observed native Saved confirmation. All three earlier uploads retained and hidden. Maximized embed, theme, copy, pricing and draft1712170 preserved.
- Public Run game now loads https://html-classic.itch.zone/html/19839230/index.html. Anonymous CDN verification: 27 non-HTML files match release.json SHA256, including all four Build payloads; index.html differs only by itch.io's appended static.itch.io/htmlgame.js script. Hosted menu rendered and visually inspected at733x485; no gameplay/provider writes performed on the public build. Gameplay evidence remains the exact production smoke above.
- Owned BrowserSkill session closed; shared daemon retained. Local/server task-temp and this run's generated result directory cleaned. Server releases/hud-20261018 ZIP/manifest retained for rollback/reproduction. Existing storefront narrow/200% acceptance remains outside this build publication scope.

## Approved Shogun storefront refresh — 2026-10-17

- User approved implementing the Shogun Showdown itch reference: native theme
  editor, black page/content, white Lato, red #ed4231 buttons/links, hidden sidebar,
  960x394 original Typing Hero banner, matching 630x500 cover, centered 640x360
  click-to-run/expand game with fullscreen. One-column copy: typing/parry/Slayer,
  free desktop/keyboard, genuine gameplay GIF, controls/Top1000, development/credits.
- Publish only `TypingHero-WebGL-shogun-20261017.zip`; keep old uploads for rollback.
  SHA256 b5e56079314c73fac75e20495db47a4a2e77897fa793f46a81aab21ebced53b3,
  28 files / 15945248 bytes / ZIP CRC PASS. No gameplay/API/leaderboard change.
- Reference CSS inspected live: same native itch theme variables, hidden right
  column, large banner and click-to-run frame. Reuse itch's editor and embed
  behavior; adapt with licensed Tiny Pixel Japan layers and this game's footage.
  No Shogun Showdown art/logo/characters. Existing tagline/pricing/tags retained.
- Rollback product media copied to `marketing/itch-io/restore/pixel-ui-20261017`;
  current public CSS/media/description also saved. Old playable is pixel-ui,
  iframe 19793957. Devlog 1712170 must remain Draft; no community posting.
- Exact production ZIP loaded locally with unchanged HTML/CSS. Native name entry,
  Enter/first help, typing attacks, successful parry, Slayer, pause/resume, restart,
  menu return, menu/result Top1000 open/close and 640x360 rendering observed.
  A complete captured round ends at score1793 / 89.05s / 10 successful parries;
  these are QA observations, not a representative player-duration measurement.
  QA bridge undefined. Existing Firebase fixtures intercept every provider read
  and write, service workers disabled, all other external traffic blocked.
  Original verification session:83 fixture reads / 6 intercepted writes / zero
  page exceptions. Doctor passed,24.4GiB free. No runtime source changed.
- Native reference Run game was clicked: its640x360 preview expands into the
  full1440x900 browser viewport; DOM reports `start_maximized ... maximized`.
  Therefore the matching native embed option is `maximized` (Click to launch in
  fullscreen), rather than an in-page fixed-size running iframe. Stored in
  `marketing/itch-io/theme.json`; dimensions describe the native launch preview.
- Local product media complete: banner960x394, cover630x500 (actual itch315x250
  thumbnail visually inspected and title readable), three unaltered960x540
  screenshots;15.000s silent H.264 MP4960x540/yuv420p/25fps and GIF640x360,
  180frames/12fps/15000ms. Full15-frame contact sheet reviewed; complete gameplay
  frame retained, including HUD and typing tray. ffmpeg complete decode PASS.
- Final clip uses a single native-keyboard run captured through passive CDP JPEG
  frames at about10fps, resampled to25fps for MP4. Nine Space inputs,5 fixture
  reads / 0writes / zero page exceptions. Typing, parry and Slayer silhouette/zoom
  are visible. The Shogun environment stays jade during Slayer; no artificial red
  background was added. The earlier screenshot-clip watcher disturbed Chromium's
  screencast dimensions and produced gray video frames; those exports were rejected.
  Passive full-frame capture fixed the recording without modifying the game.
- Current product SHA256: MP4
  `66fcc404eef88cd06a5a8683945e8f1ccbc9704447ff6b610232747e6804fca9`;
  GIF `5c4e95ef8c83b9a864a4b02d9331715ae5f35a2fc8d237c7ae28d18351dd5992`.
  Saved CDN GIF was downloaded and matches the local GIF byte-for-byte.
- Upload19819691 is `TypingHero-WebGL-shogun-20261017.zip`, separate from both old
  uploads. Initial cover30982747 (superseded by30983602); new PNGs30982787/30982789/30982788 and GIF30982790
  uploaded successfully. page.html contains the exact new CDN GIF URL and credits
  from the shipped Shogun CREDITS.txt (LyniaDesign/Pimen/Kenney/Peter Hull).
  Retained hero/skeleton/music provenance remains explicitly unverified.
- Owner preview confirms devlog1712170 still **Draft**, with the new six-paragraph
  body saved. Its old media/playable attachments were replaced in the published checkpoint below.
- Image uploads initially returned Cloudflare403 (`Just a moment...`), not an
  invalid PNG/CSRF response. BrowserSkill human-help timed out; no approval was
  inferred. The same Edge session later accepted uploads after normal endpoint
  navigation and a fresh editor load. No credentials extracted or backend bypass.
- One mistaken native button click removed the cover only in the unsaved editor.
  Reload discarded that form, restored cover30979664, and was verified before the
  new cover was uploaded successfully. No page save occurred during that mistake.
- Published checkpoint (2026-10-17): Edge reconnected; fresh owned session `ekur`
  inspected saved state before writes. Saved upload19819691 as playable with
  `maximized`; uploads19793957/19792304 retained hidden. Per-file price overrides
  are off; free/donation pricing, tagline and metadata retained. Saved new page.html,
  cover30983602, banner30983627, black/white/red Lato Large theme and hidden sidebar.
  Social image30983638 is the new banner; public og:image and tagline verified.
- Desktop owner/public URL renders the960x394 banner, centered640x360 launch
  preview, one-column copy/new GIF and hidden screenshot sidebar. Native Run game
  expands to1416x767 viewport and loads iframe19819691. Devlog1712170 saved with
  Shogun upload and four new image/GIF attachments; owner preview explicitly DRAFT.
- Hosted build checked in an isolated Playwright1.58.2 browser with every Firebase
  request intercepted, service workers blocked. Loaded name/menu, native name/Enter,
  typing, result/menu Top1000 and scroll, restart, pause/resume, menu return and
  640x360 rendering.11 fixture reads / 1 intercepted write / zero page exceptions;
  QA bridge undefined. The15s hosted pilot registered3 Space inputs but0 successful
  parries; successful parry/Slayer remain covered by the earlier exact-ZIP run.
  Downloaded hosted loader/framework/wasm/data all HTTP200 and SHA256-identical
  to the extracted release, so no divergent hosted build was observed.
- Acceptance remains OPEN for anonymous narrow viewport and actual200% browser
  zoom/native fullscreen (viewport expansion is verified separately). Owner400px
  viewport has600px-wide primary column inside itch's appearance-editor flex wrapper,
  causing horizontal overflow and cropped banner. This must not be called a pass
  or assumed to affect anonymous users without evidence. Anonymous Playwright and
  web fetch hit Cloudflare; browser-native Ctrl+= dispatched by BrowserSkill did
  not change DPR/viewport, so it is not proof of actual zoom. No false mobile-friendly
  declaration or gameplay changes were used to hide the issue.
- Next: verify the anonymous page in an accessible browser at400px and true200%
  zoom, plus native fullscreen. If overflow is also present for visitors, determine
  a native itch setting that fixes it while retaining desktop-keyboard requirements;
  custom CSS/mobile-support claims are not approved substitutes for the native-theme
  plan. The page is published, but storefront acceptance is not yet complete.
- Cleanup: hosted QA browser/static host stopped, fresh task session closed. Original
  afve session is not registered, so old Agent Window1350802780 cannot be closed
  by session lifecycle. Shared daemon PID3312 remains running. Server temp was
  already removed; local helper scripts/exact build/pinned tooling remain at
  `E:/CodexTemp/TypingHero/itch-shogun` solely for outstanding browser acceptance.
  Remove these after completion. Runtime captures removed after recording findings.
  Rollback assets under marketing/itch-io/restore remain product deliverables.
- No community posts, source ZIP/layer uploads, paid ads, schedule, gameplay/API/
  leaderboard changes, commit, branch or worktree. Fourteen-day discovery results
  remain unmeasured; existing outreach measurement table is preserved.

## Intent and scope

Implement the approved international-discovery plan on the existing
https://micharel09.itch.io/typing-hero page. Improve accurate classification,
English copy, genuine current-build media and sourced credits. Prepare a devlog
and two community pitches. No gameplay/API/build changes, paid advertising,
renaming, community posting or scheduled automation.

Acceptance: Action genre, Keyboard, English and single-player accurately saved;
tagline explains typing/parry; description explains free desktop-browser play,
controls, Slayer and leaderboard; cover visibly communicates typing and combat;
three genuine screenshots of the published build appear in the screenshot
gallery; credits identify verified creators without inventing missing authors;
short typing → parry → Slayer clip, profile bio and devlog draft are prepared.
Verify a run's duration before changing the current length classification.
Verify saved page/profile publicly and review the local diff.

## Decisions

- Tagline: “Type to slash, press Space to parry, and chase the high score.”
- Keep existing accurate tags, project URL, name, pricing and playable upload.
- Distinguish the currently published build from newer local deliverables.
- Only prepare the devlog, as requested. Community publication needs an explicit
  channel and approved message. The 14-day experiment starts after actual
  outreach, with day 0/7/14 deltas; do not fabricate future measurements.
- Original investigation: 206 views, 43 browser plays, no rating/collection/comment,
  0 followers; 24 external referrals in 30 days, all Facebook. These are historical
  observations, not current analytics or unique players.
- Reference: https://itch.io/docs/creators/quality-guidelines (read 2026-10-17).

## Checkpoint — investigation, 2026-10-17

- Existing checkout on main is extensively dirty with earlier gameplay/UI work;
  preserve all existing changes. No new branch/worktree/agent.
- Authenticated BrowserSkill Edge instance 61a46c05, session comt, confirms owner
  micharel09. Public page still announces TypingHero-WebGL-pixel-ui.zip (2026-10-16);
  newer Shogun build is only recorded as a local delivery. Capture published game.
- BrowserSkill and Orca CLI instructions read. Orca tab
  cad10d97-aca6-4a77-ba5d-815773c02d87 stalled at Cloudflare; authenticated Edge
  loads the page normally. Use Edge for this account, close task-owned surfaces.
- Temporary research/QA/download files: E:/CodexTemp/TypingHero/itch-discovery.
- Project verifies Crusenho UI (CC BY 4.0), Kenney Input Prompts Pixel (CC0),
  VT323 by Peter Hull (OFL). Newer-build-only credits require checking against
  the actual published archive before being attributed on the current page.
- Next: inspect owner metadata/analytics, run published build, capture media,
  prepare and save accurate copy/metadata/gallery, verify and record results.

## Checkpoint — page copy and classification saved

- Confirmed game ID3857509 and public iframe19793957. Hosted loader, framework,
  WASM, data and CREDITS.txt all match the local pixel-ui ZIP byte-for-byte (SHA256).
  Media capture serves this exact artifact without gameplay/HTML changes.
- Current analytics rechecked before changes:206 views,43 Browser Plays,
  ratings/collections/comments/followers all0. Facebook referral rows17+3+2+1+1
  for2026-09-18–2026-10-17. Stored the measurement protocol in
  `marketing/itch-io/outreach.md`; outreach/future counters remain pending.
- Saved Keyboard and English, retained single-player1–1 and all nine existing
  tags. Saved Action and the approved tagline. Public page and profile confirm
  new copy/bio; the updated page describes the actual published pixel-ui build.
- Redactor HTML mode changes were initially visible but its hidden submitted
  textarea remained stale. Native editor keyup/blur synchronized it; saving again
  and reloading the public page confirmed every new paragraph and source link.
- Credits identify only the three verified UI creators. Legacy character,
  environment and music provenance is still unknown; state this honestly rather
  than attributing newer-build-only assets or inventing an author.
- Devlog1712170 saved and owner preview explicitly shows DRAFT:
  https://micharel09.itch.io/typing-hero/devlog/1712170/type-to-slash-the-current-typing-hero-browser-update
  Media attachments still pending. Both community messages remain local drafts.
- Native HTML/SVG cover uses a crop of actual gameplay and the existing VT323
  font; no fabricated gameplay or generated art.630×500 PNG rendered and visually
  inspected. Uploaded cover; final save/public image verification pending.
- Local Playwright1.58.2/cached Chromium1243 capture uses native keys/mouse and raster OCR
  only; never injects Unity state. Service workers blocked, provider GET allowed,
  every other provider method intercepted, other external traffic blocked.
  Initial short key presses were missed by Unity; corrected native hold to80ms.
  Capture/clip selection still pending. No player score is intentionally written.
- Average-duration metadata remains unchanged: failed automated runs are not a
  representative estimate of time to complete a run. No unsupported new estimate.

## Checkpoint — genuine media captured

- Captured typing, successful parry and red-background Slayer with actual
  silhouette attacks, floating points and the real public leaderboard. Product
  screenshots are960×540 in `marketing/itch-io/`. All gameplay uses native keys;
  the final capture records one intercepted score write and zero page exceptions.
- Blind fixed-cadence input and sprite matching were unreliable for this published
  build. Reading the game's existing console impact messages and timing native
  Space presses before the following strike produced the required Slayer footage.
  No source, memory, score, HP or stamina was modified to obtain it.
- Cover630×500 and social preview are saved; public profile card uses image30979664,
  and public og:image/twitter:image use30979772. Downloaded social image was
  visually compared with the local cover and matches the intended composition.
- BrowserSkill daemon became unavailable; status with automatic startup disabled
  confirmed no process/listener. Restored it in managed foreground session89870,
  verified IPC and reconnected original Edge61a46c05. Old sessioncomt no longer
  exists. New sessionmboc controls only newly created task tabs. No user tab borrowed.
- First screenshot and devlog attachment saved; remaining two gallery uploads,
  short video/GIF export, final draft attachments and public verification pending.
- Video conversion uses existing ffmpeg on fly-server; task temp is
  `/home/quan/CodexTemp/TypingHero/itch-discovery`. The attempted `/data/codex-tmp`
  path was not writable; no data changed there. Remove local/server temporary
  captures after final verification and retain only requested product media.

## Final acceptance — 2026-10-17

- Public page: Action, Keyboard, English, existing single-player 1–1, nine existing
  tags, free/donation pricing and pixel-ui upload preserved. Approved tagline,
  controls, Slayer explanation, Top 1000, development/feedback text and linked UI
  credits verified on the actual public page. Profile bio also verified publicly.
- Gallery: three current-build images, IDs 30979831/30979971/30979992. Existing
  theme's Auto setting hides screenshots for this browser game; explicitly saved
  Sidebar, preserving colors/font/embed. All three loaded at 347×195 in the page;
  clicked the first and visually verified the full gameplay image in the lightbox.
- Cover image 30979664 and social image 30979772 show the same native composition.
  Public og:image/twitter:image explicitly reference the new social image.
- Media in `marketing/itch-io/`: 630×500 cover and editable HTML, three 960×540
  screenshots, 15.000-second MP4 (960×600, H.264, yuv420p, 25fps, 844798 bytes)
  and matching GIF (640×400, 12fps, 180 frames, 1334994 bytes). Whole clip visually
  reviewed as a 15-frame contact sheet; typing attack, parries, Slayer transition,
  rapid silhouette attacks and final URL are present. ffmpeg full MP4 decode PASS.
  Footer labels do not cover the original game frame. The montage is silent.
- Export SHA256: MP4
  `62cf250fe00938296c1a5d0797a2738796184e5804f3cfa63e674a3534ac73b0`;
  GIF `f89f7470e8ae768465adae98dbabc8692c48365565ec985ba06e2f76313499f7`.
  Server/local hashes match. The GIF downloaded from the saved draft's itch CDN
  URL matches the local export byte-for-byte.
- Devlog 1712170 remains **Draft**, General Update, English, with all three
  screenshots, the animated GIF and the existing playable upload attached.
  Owner preview verified Draft and four images. Corrected editor-induced hard
  line breaks into five complete paragraphs and verified the saved text.
- Final capture uses only native keyboard/mouse, screenshot OCR and existing
  console impact messages for timing. One attempted score write intercepted,
  zero page exceptions. No Unity state or production leaderboard record changed.
- HTML parsed, cover image/font paths resolved, dimensions/GIF frames verified,
  new text files passed `git diff --no-index --check -- NUL <file>`; no credentials
  in deliverables. Diff/file scope reviewed: only this plan and `marketing/itch-io`
  added by this task. Existing dirty gameplay/UI files preserved; no new commit,
  branch, worktree, Unity build, PR or game-build deployment.
- Remaining deliberately unclaimed: representative average session duration
  (unchanged), original character/environment/audio provenance (clearly stated
  as unverified). No unsupported bug list or attribution was invented.
- Prepared both international community pitches and day 0/7/14 measurement table
  in outreach.md. No community destination/message authorized for sending, no
  outreach performed, no paid ads or future results/schedule fabricated.
  Trailer field remains empty because it requires a YouTube/Vimeo destination;
  the requested clip is delivered locally and as the draft's GIF attachment.
- Cleanup complete: stopped task browser session mboc (session list empty),
  closed the task-owned Orca tab (tab list empty), removed verified local/server
  temp directories after checking their exact resolved paths and absence of
  symlinks/reparse points. Requested product media remain in `marketing/itch-io`.
  Shared BrowserSkill daemon remains available per its lifecycle instructions.

## Reference comparison and public preview — 2026-10-17

The user requested a confidence check and comparison with other creators. The
initial implementation verified correctness and media delivery; it did not include
a systematic comparison with other typing-game storefronts. That distinction
must remain explicit when discussing confidence in the result.

Read the creators' live pages and inspected their rendered desktop pages through
the existing Edge profile at a 1416×827 viewport. Compared page presentation;
did not play these games or obtain their private traffic/conversion analytics.

| Primary reference | Observed presentation | Decision for Typing Hero |
| --- | --- | --- |
| [Type Knight — chaikadev](https://chaikadev.itch.io/type-knight) | Paid Windows/Linux title; themed pixel-art header/background, trailer beside the overview and gameplay screenshots. The creator's comment replies also describe parrying. | Use actual combat footage to explain timing. Typing combat or parry alone is not an established unique selling point. Do not treat this paid game's storefront as a browser conversion benchmark. |
| [TYPE:CAST — Not Jam](https://not-jam.itch.io/typecast) | Browser play, concise narrative-typing premise, explicit web-only leaderboard and version information; Action, English and Keyboard metadata. | Retain accurate classification, a concrete opening premise and an explicit leaderboard description. Avoid copying its feature set or assuming its presentation caused its ratings. |
| [Hey, I'm typing here! — KennyDev3](https://kennydev3.itch.io/hey-im-typing-here) | Opens with the combined typing/running challenge, then explains three modes, including a 3-minute score challenge. | Retain Typing Hero's typing/parry challenge and score invitation. Do not invent a run-duration estimate from another game's timed mode. |

[itch.io's quality guidelines](https://itch.io/docs/creators/quality-guidelines)
explicitly support gameplay GIFs as screenshots, including for embedded browser
games. This offers a public preview without assigning a video-hosting account.

Concrete gap found: the prepared animated preview was only attached to the draft
devlog, so visitors to the public game page could not see that preview. Uploaded
the existing genuine gameplay GIF to Screenshots and used the editor's native
Move up buttons to place it first. Saved and verified the public gallery order:
GIF 30980850, then PNGs 30979831/30979971/30979992. The three original screenshots,
description, cover, playable build and draft status are preserved.

The public GIF loads from
`https://img.itch.zone/aW1hZ2UvMzg1NzUwOS8zMDk4MDg1MC5naWY=/original/xiSdpb.gif`
at native 640×400 and renders 347px wide in the sidebar. Effectiveness remains
unproven: no controlled thumbnail test, acquisition uplift or conversion result
has been measured. The planned 14-day outreach comparison is still pending
authorized community destinations and posting.

Verification: the saved public page reports four images; the GIF is the first
loaded image and all three stills remain. Two viewport captures at the same scroll
position visibly show different GIF frames (typing attacks and Slayer), confirming
that the gallery renders the animation. New text changes pass whitespace checks.
The reference comparison establishes presentation patterns, not causal evidence
that these patterns increase discovery or plays.

## 2026-10-19 - store discovery and conversion execution

Latest user scope prioritizes actual discovery/store presentation and forbids
unrelated gameplay changes. Earlier Guest/Endless code and candidate Shorts are
preserved but were not published or represented as the currently playable build.

Delivered `marketing/ItchIO_Optimization/`: two genuine-gameplay cover concepts
(630x500 plus 315x250 checks and editable HTML), three current-production canvas
screenshots, an actual BLADE-to-sword GIF, ready English store copy, accurate
configuration, five hooks, three short-video concepts, Reddit/Get Feedback/devlog/
creator drafts, Shorts/TikTok captions, audit and day 0/7/14 measurement worksheet.
No community post, creator message or video-channel upload was performed. Three
new finished MP4 Shorts are not part of this package; current Slayer footage must
be captured before making the third concept into a new production-matched clip.

Published through the authorized owner editor and verified on the public page:
`Typing Hero: Type to Slash`, explicit typing/parry/stamina CTA, cover A image
31068886, gallery 31068982/83/84/81 and inline real typing GIF. High Score replaces
Indie; expected-session category is A few minutes and Interactive tutorial is
selected. AI-assisted store text is disclosed as Text. Theme/banner, pricing and
playable upload 19865461 remain in use. Public exact-name search showed the new
cover/title/tagline; this does not establish improved ranking or CTR.

Fresh dashboard observation: 211 Views, 45 Browser Plays, zero ratings/collections/
comments. Referrer window September 20-October 19 shows 28 external Facebook-domain
visits plus 1 self-referral with fbclid, correcting the earlier blanket 29-Facebook
interpretation. Header totals are not confirmed as the same cohort/window as the
referrer table. Impressions, itch CTR, unique players and retention remain unknown.
The first loaded general typing search/browse cards did not include the target,
while exact-name indexing worked. No growth uplift is claimed.

Verification: exact published binary booted locally with keyboard/mouse controls
and blocked/fixture leaderboard traffic; no production score writes. Actual word
completion/hit captured; final GIF fully decoded, contact frames and both small
covers visually reviewed. Published description/metadata/images/search card and
unchanged iframe source checked. No gameplay scripts changed by this marketing
task; no Unity suite was needed for copy/media edits. Full evidence, limitations,
rollback and sources: `marketing/ItchIO_Optimization/StorePage/applied-and-rollback.md`
and `MarketingPlan/audit.md` beneath that package. Next: take a fresh day-0 baseline
before outbound posting and run the supplied 14-day experiment; no success claim
until real external arrivals and matched-period starts are measured.
