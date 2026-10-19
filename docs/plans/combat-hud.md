# HUD theo Shogun Showdown

## Acceptance đã duyệt (r2, lịch sử)

Áp dụng trận thường và Slayer: cụm dưới từng chân 12 px, rộng 112, tên nhỏ/HP 6/stamina 4; điểm + multiplier căn giữa theo chữ, top 32; tray 672×56 dưới cụm thấp nhất 12 px, đủ ba từ/timer/error. Giữ đỏ/dải đen/gap 8, score zoom/pulse/tint/popup và tutorial. Không đổi gameplay/API/GUID; không production/ZIP.

## Mapping và implementation

Nguồn: plan user 2026-10-18 và [contract r2](../../design/layouts/shogun/web/contract.md), giữ r1 trong revisions/r1. Reuse slider/font/color/binding của ShogunTheme. CombatHUDLayout sở hữu vị trí status/tray/score/multiplier/decay/popup; collider landmarks .28/.12 dùng chung với SlayerScreenFX. Camera/bars/scale/pulse giữ owner cũ; follow multiplier cũ được tắt qua binding null. TutorialOverlay cho viền bám target mới.

## Checkpoint

- Giữ checkout dirty gốc, không tạo branch/worktree/commit. Temp: E:/CodexTemp/TypingHero/combat-hud; Ubuntu: /data/typinghero-game-loop/task-temp/combat-hud.
- Doctor PASS, Unity 2022.3.25f1/license/toolchain sẵn có, 23.7 GiB. Runner contracts 8/8 PASS.
- Quick 47/47 PASS, 20261018T041510, input 51fe66ab40ef29a8b787fe85a8a8d2f6c37727a7736510e458dc9f4606df6a84. Thêm regression projection/center với zoom 1/1.282 và pulse 1.2. Sau đó chỉ dọn intermediate layout trong editor builder; runtime sản phẩm không đổi.
- Bundle r1 check PASS trước sửa; r2 check PASS, fingerprint 902c0ea39127ccbe8d51208f1bca6fd1c1656b9cdc58dc9172f2dcfa2e690c35. Đã xem PNG không có editor che và ảnh reference chính thức.
- QA WebGL 20261018T041601, input d8cd8f99853ff500632ad4a23b1def76620d0ae7c147d3d0e2e0bf13a29f63cc: repeat-builder PASS. SampleScene giữ 659/659 ID cũ, tổng 664, thêm duy nhất GUID owner layout; 27 document cũ đổi đều thuộc UI/binding/layout. Local scene chỉ bỏ trailing whitespace từ output Unity.
- QA đã qua 142 checks trước lỗi: tutorial pilot và sáu bài học/retry/expiry/pause/restart/clean battle; normal/Slayer ở 960×540,1280×720,1920×1080,640×360. Đã đo status112/gap12, tray672×56/gap12, score center/top32, đủ ba từ/binding khớp; bars full width/gap8. Đã xem đủ tám ảnh và stamina highlight25/250; không clipping hay che thân.
- Lượt broad FAILED khi nhân vật bị hủy sau chết: QA probe gọi HighestHead/LowestFoot mà chưa kiểm tra hai collider còn sống, coroutine quan sát ném NullReference và state bị cũ, dẫn tới watchdog timeout. Đã thêm guard trong QA probe; đây chưa phải full browser PASS. Không đổi gameplay/watchdog/threshold để né lỗi.
- Driver bổ sung kiểm HP fill sau sát thương và một ảnh popup x2; input vẫn là bàn phím thật, fixture/network blocking giữ nguyên. Tiếp: QA build mới, kiểm thắng/thua/restart, x2/x3/popup/pulse, receipt/memory và cleanup.
- QA build mới 20261018T042543, input ac4c2952c7e5ca6e77e5995702a4549ff276fda7034e6acf32d41488096281cd; repeat-builder PASS. Chỉ guard QA và scene đã serialize/driver khác so với player đầu; runtime sản phẩm không đổi. R1/r2 bundle và link plan đều check PASS.

## Acceptance hoàn tất (r2)

- Broad mới qua 181 checks và một round/restart, 0 exception; HP/stamina sau sát thương, tám hit dẫn tới thua, kết quả đóng băng và restart đều đạt. Đã chủ động dừng khi đang vòng cân bằng đầu, không ghi full profile PASS. Việc không có log mới lúc đó bị đọc nhầm là ngừng tiến triển; report thực tế vẫn tiến triển tới enemy37HP và đã qua luồng chết. Không có lỗi sản phẩm mới được chứng minh.
- Ca thắng input thật đạt: enemy850HP, 79.9s, score2798, accuracy98.7%, parry9; đã xem result, restart trả đầy ba thanh/score0/HUD thường. Popup vàng x2 đúng vị trí đã xem. Ca này qua 12 checks trước khi driver x3 hết thời gian do chụp ảnh giữa chuỗi input; không tính toàn ca là PASS.
- Bỏ riêng bước chụp x2 giữa input; giữ ảnh đã xem và chạy lại Slayer liên tục trên cùng player: PASS27/27,46.307s, score275/x3 đỏ/zoom1.282051; điểm khớp từng ký tự/rounding, pulse/idle/pause4cỡ/exit/restart đều đạt. Đã xem x3 và exit với viền lỗi đỏ. 9fixture reads/0writes/0exceptions. Không đổi gameplay hoặc hạ tiêu chí.
- Source/scene cuối khớp Unity serialization sau chuẩn hóa whitespace: SHA2565cd400538351b13aae9669c9065cbc949ad8622c8014f9781d3d18ba9699fc36. Doctor cuối PASS, input1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588; chỉ QA drivers khác với build cuối, runtime sản phẩm giữ nguyên.
- [Receipt và giới hạn](../game/verification.md): Chromium153/SwiftShader,2CPU/8GiB,network blocked/Firebase fixtures; không claim native FPS/audio. Không production/Windows/ZIP/stress/soak/publish.
- Đã dọn bốn thư mục result, task-temp trên Ubuntu và temp/capture local sau receipt. Không còn container kiểm thử của task; source/Library/toolchain/QA player chuẩn trên Ubuntu được giữ để tái sử dụng. Diff whitespace và link checks đạt. Acceptance đã hoàn tất, không còn bước implementation/kiểm chứng HUD cần làm.

## Revision 3 — điểm gần vùng gõ (đã kiểm chứng)

User duyệt plan mới: normal/Slayer giữ status; hàng điểm cách status thấp nhất 8 px, cao cố định 48 px, font 24/gap 8/decay dưới; tray 672×56 sau 8 px. Popup phải 12 px, font 18, rise 12; giữ fade và bám camera. Không đổi gameplay/API/font/palette, không publish.

- Owner: CombatHUDLayout đặt vị trí, ShogunTheme cập nhật binding/serialization, FloatingScore bám anchor trong LateUpdate sau layout. Giữ checkout dirty gốc; không tạo branch/worktree/commit. Bundle r3 và r2 giữ nguyên đều check PASS; fingerprint r3 `49ad851305cd8144427738228e5136278d376a2644057e7d6a4b394ef0469df6`. Đã xem wireframe không có editor che.
- Doctor trước/sau PASS, pinned Unity/license có sẵn, 23.7 GiB. Quick cuối **52/52 PASS**, 20261018T063624, input `24893dfb136c6a6d19e372ff4413b6d7cca8784b3d333de555d6d98036790d84`. Có 0/6/10 chữ số, x1 ẩn/x2/x3, pulse 1/1.2, zoom 1/1.282, vị trí tray/decay/popup và fade không khởi động lại khi anchor di chuyển. Runner contracts 8/8 PASS.
- QA WebGL 20261018T063120, input `70c98ea4c61ba45e02061ec219cc459a290b6a5d9901a45ea1b6510952085d85`; repeat-builder PASS. Scene/prefab Unity xuất giữ GUID, đủ 664/664 và 5/5 object ID. Local chỉ chuẩn hóa trailing whitespace, delta thuộc UI yêu cầu.
- Các lần driver đầu thất bại: đo sau khi tutorial xóa từ đã hoàn thành; đo HUD ẩn dưới camera pause; và hết Slayer khi chụp ảnh/resume lần lượt. Không tính các lượt này là PASS. Camera pause có sẵn khiến tray cũ cũng vượt đáy dưới overlay; không đổi camera ngoài scope. Phép đo cuối dùng reading có đủ từ, Slayer được kích hoạt bằng parry thật rồi giữ trong reading; ca functional chạy riêng trong cùng scenario. Không đổi timing/threshold sản phẩm. Quick thử khi runner giữ lock bị từ chối, sau đó chạy lại khi lock được nhả.
- Final `score-layout` **PASS 166 checks / 171.817 s** trên cùng QA player: keyboard hits/parries, reading/pause locks, normal/Slayer đủ 640×360, 960×540, 1280×720, 1920×1080; x2 vàng/x3 đỏ, score 275 đúng rounding, popup font 18 bám anchor, pulse/idle/pause/resume/exit/restart. Chromium 153/SwiftShader, 2 CPU/8 GiB/shm 1 GiB; 45 fixture reads/0 writes/0 exceptions, network ngoài bị chặn.
- Đã xem đủ tám ảnh layout, popup x3 và exit. Khoảng cách tâm điểm–từ **70 px normal / 66.615 px Slayer**, top-to-top 60 px; tray cách đáy ít nhất 43.142/18.080 px. Baseline tính từ công thức r2 và cùng projection: tâm điểm–từ normal 368.466–368.858 px, Slayer 389.469–389.971 px. Đây là baseline hình học tái dựng, không phải ảnh trước sửa hay eye tracking; kết quả mới dưới 80 px.
- Memory cập nhật đúng phần HUD trong gameplay/systems/verification. Không production/Windows/release/itch/stress/soak; không claim FPS thiết bị hay audio. Doctor cuối input `e49808a9fe7f779d2bec93160e240e07362bdc002aadec015b3d30a7a56489db`: runtime/serialized UI giữ nguyên so với player đã kiểm; test/driver mới được đồng bộ.
- Đã dọn task-temp/capture/report trên cả hai host và bốn thư mục result của task sau receipt/review. Không còn container kiểm thử của task. Source/Library/toolchain/QA player dùng chung giữ để tái sử dụng; không tạo branch/worktree/commit. Syntax, runner contracts và whitespace cuối PASS. Acceptance hoàn tất, không còn bước implementation/kiểm chứng cần làm.
- User sau đó yêu cầu publish: bản production r3 đã lên itch.io, upload19840745, giữ bốn upload trước để rollback. Chi tiết build/checksum/smoke/CDN trong [itch-discovery](itch-discovery.md) và verification.md; không đổi runtime khi publish.


## Revision 4 - compact Type Knight HUD (verified)

Approved user plan/reference: https://chaikadev.itch.io/type-knight/devlog/70022/type-knight-demo-4
Reference only, no source game assets/code copied. Existing font/palette/UGUI reused.
Unframed total font24 at top/right24, screen scale1, immediate updates/120ms pulse;
combo font16 below/hidden x1. Hide actor names and disable PlayerNameHUD refresh;
red HP6, Gold(#DCC47C) stamina4/y8, player/enemy status heights6/12. Tray672x56,
gap12 below lowest status, no score row. One accumulated +delta popup font18 at
projected enemy head +(24,12), max rise18; hold150ms/fade450ms after last award,
restore alpha/add on fade interruption. Pause holds time; lethal keeps last anchor/
finishes fade; restart clears instance/accumulation. Rules/Score/OnScored unchanged.
Owners: CombatHUDLayout positions; ShogunTheme bindings/serialization; ScoreSystem
single presentation instance; FloatingScore lifetime/rise/alpha. No runtime dependency.

- Preserved existing dirty VFX/skeleton work; no branch/worktree/commit/push created.
- Bundle r3 retained/check PASS; r4 rendered/captured/reviewed in Edge with editor panel
  closed, check PASS. Fingerprint fa2062713e0aa8e844663fbaf35f372774d246fd8f8bf72d3a7a35c501445523.
  Body positions are schematic; contract geometry is explicitly user-specified.
- QA WebGL20261018T101114 PASS, input78e663f836356dddd5bfbf94889aa7965df9e5180af4679a761f54fe93815bf8;
  repeat-builder PASS. SampleScene664/664 and popup5/5 IDs retained, identical referenced
  GUID sets. Local copies match generated serialization after trailing-whitespace cleanup.
- Targeted score-layout PASS198 checks/311.200s: real keyboard hits/parries, reading/
  pause locks, normal/Slayer at640x360,960x540,1280x720,1920x1080; all eight frames,
  cue, popup, x3/pause, restored normal and victory frames reviewed. Top/right margins24px
  at all sizes (largest error<.0001 reference pixels); status/tray gap12, Gold stamina,
  names hidden, score scale1. Exact awards/capped multiplier, one accumulating popup,
  held popup time/alpha during pause, Slayer exit and restart pass. Victory86.1s,
  score2499/100% accuracy/9parries, lethal fade completes, restart restores all bars.
  Fixture leaderboard1000 rows/scroll/refresh reuse PASS. Chromium153/SwiftShader,
  2CPU/8GiB/shm1GiB;7fixture reads/1intercepted write/0exceptions; external network blocked.
- Native Edge155.0.4283.62, Playwright1.64.1: Slayer PASS50 checks/38.961s, x2/yellow,
  x3/red/score275/scale1, exact popup accumulation/pause/restart.0fixture reads/writes/
  exceptions. Same QA player; no gameplay mutation. Reviewed real parry-to-fade and
  rapid Slayer award sequences at100ms intervals, including yellow combo/popups and
  continued rise. Recorded37.24s WebM; actual playbackRate1 advances2.012s over2.0121s
  (ratio.9999503), no retiming. This is functional/visual evidence, not device FPS/audio.
- Final Quick20261018T102205 PASS53/53, no failures/skips, input
  6a6e68b810b1e7531268235820aff4f88a6e5364fe5532e8ebd75351114847e2.
  Tests explicitly cover0/6/10digits,x1/x2/x3,pulse1/1.2,camera normal/Slayer;
  anchor movement, accumulation during fade/alpha restore, actual CombatSession.Complete,
  pause, destroyed anchor, expiry and restart. Final Quick includes serialized UI and
  observation-driver correction; product C# matches QA runtime.
- Runner initially busy with another session. Nonblocking attempts safely refused;
  task-temp wrapper queued on the existing exclusive lock. An ordering error in the
  wrapper meant QA ran before Quick; corrected by running final Quick separately,
  not by claiming the earlier52-test pass covered new tests. No repository runner/CI
  change. Earlier task-owned waiting jobs were verified sleeping on the lock and stopped;
  other jobs were preserved. The native recording initially lacked Playwright ffmpeg;
  installed only that support binary in task-temp, then reran successfully.
- Final receipts are also in docs/game/verification.md. Only QA built; no production,
  Windows, ZIP/release, publication, stress/soak or real provider writes by this task.
- Final scoped diff/secret/GUID/whitespace/path review PASS; no out-of-scope runtime
  deltas or new dependencies. Runner contracts8/8 PASS on Ubuntu. Attempting that
  Linux-only suite locally first failed importing fcntl; rerun in its intended Ubuntu
  environment passed. JS syntax and current/preserved bundle checks PASS.
- Written receipts recorded before capture cleanup. Shared server source/Library/
  toolchain/QA build remain reusable; final product C# is unchanged after acceptance.
  Task captures, downloaded QA player, temporary wrappers/native profile/cache and
  three owned result directories were removed after receipts. Local task-temp no longer
  exists; other sessions/results/artwork were preserved. Acceptance complete; no blocker.


## Revision4 production publication (complete)

User authorized itch.io deployment after acceptance, then explicitly requested the
agent handle the remaining file-selection step. Published to
https://micharel09.itch.io/typing-hero, game3857509, upload19847268.

- Production20261018T130348 PASS from verified Quick53 snapshot
  6a6e68b810b1e7531268235820aff4f88a6e5364fe5532e8ebd75351114847e2.
  Runtime/builder hashes match verified local source. All JS/WASM/data exclude
  typingHeroQA/TypingHeroQA_Publish/GameLoopProbe markers; build qa=false.
- Original production HTML Edge155 smoke: native name/Enter/BLADE hit/score10 and
  enemy-head+10, hidden names/Gold stamina/corner score, pause/Resume/640x360 reviewed.
 0JS exceptions/0Firebase reads/writes. Unsupported shader console lines observed
  with correct render/no managed exception. Initial smoke helper pointed to an older
  player; discarded it, corrected root and used a fresh profile for final evidence.
- ZIP29members/15,984,116bytes, CRC32 5b3e086b, SHA256
  31799c809a460e4b1eec86d6fb7eb1d26ba5b57904c2c2209d6cd05eb6648921.
  All member CRC/local-server hash checks PASS. ZIP/release.json/per-file manifest/
  published.json retained at /data/typinghero-game-loop/releases/compact-hud-r4-20261018/.
- Orca authenticated edit page verified micharel09/game3857509. Detached input made
  direct CLI upload fail without upload; BrowserSkill alternate login timed out and
  its owned session was stopped. Orca later cleared Cloudflare. Native-window attempts
  did not establish a useful picker state; used the documented Orca CLI `get cdp-url`
  endpoint instead, with its exact scoped target, no endpoint scanning/cookie extraction.
  Standard Page.fileChooserOpened + DOM.setFileInputFiles selected the verified ZIP.
  No custom page JS submission or gameplay mutation. No second upload was created.
- New upload Success -> browser-playable checked; previous ScoreHUD upload hidden.
  Save via focused native Enter produced actual Saved confirmation. Earlier pointer
  Save attempts had no Saved/POST evidence; public remained old, so they were not
  treated as publication. Existing five uploads retained hidden; original fullscreen/
  copy/theme/pricing/gallery/draft preserved. Public placeholder now19847268 with
  version1792330953. Saved state and public target verified.
- Anonymous CDN checks: all28non-HTML file hashes match release; hosted HTML matches
  original after removing itch.io htmlgame.js and normalizing whitespace. Unversioned
  index request hit a cached404 from before extraction; actual versioned public URL
  returns the correct HTML. Anonymous Edge155 loads the actual menu, QA undefined,
  Firebase blocked/no public gameplay/provider writes. Initial capture was still Unity
  splash and rejected; later actual menu frame inspected. Public-page headless request
  hit Cloudflare; configured public URL verified through existing Orca session, while
  CDN payload/menu verification used a fresh anonymous browser.
- Final receipt/diff/whitespace review PASS. Local ZIP/client/captures and owned
  production result removed; server release/source/Library/toolchain/builds retained.
  Public task tab closed; edit tab was already absent at cleanup. Other sessions,
  artwork and prior uploads preserved. Publication acceptance complete.


## Revision 5 - centered Slayer score (verified)

Approved supplied user plan: Slayer total48/combo24, actual glyph-centered group in
upper black frame, gap4, x1 hidden; existing tiers/pulse120ms. Normal restores
corner top/right24 and fonts24/16. Position/fonts follow220ms bar reveal, canvas
scale1; popup/tray/scoring/events unchanged. CombatHUDLayout owns geometry,
SlayerScreenFX exposes reveal/full frame, ShogunTheme binds existing objects.

- Final Quick143053 **74/74 PASS**, input
  `1f15e8ec15222e1f2e1b91f3906e7bac8efc5bac05c99c1606d3e60342918cde`.
  Covers0/6/10digits,x1/x2/x3,pulse1/1.2,normal/Slayer camera, repeated reveal and
  restore, interrupted exit/re-entry, unscaled pause and disable reset.
- QA143344 reused safely:996/996 raw non-test product inputs (Assets/Packages/
  ProjectSettings) match current source, zero missing/different; input
  `0b72b04665673ec594fb6dc0d7323a53a9f3deb9fa5370c5275ad13122cae822`.
  Pinned Unity2022.3.25f1, QA=true and repeat-builder PASS. Scene normalized match,
  664/664 IDs and all referenced GUIDs retained; scene SHA256
  `1135e386c17c33a4208702dbdb5f87fd000a89158c41e70b04eb12eb5a23217f`.
  Shared runner contention led to reuse of this identical concurrent build;
  stopped only the task-owned duplicate while it was demonstrably blocked on
  FLOCK (PID171407), without touching the lock holder or other queued work.
- Software targeted run completed201 checks: normal/Slayer four sizes, real
  parries/rapid typing,x2/x3/exact score,popup/pause/resume/exit/restart. Then failed
  awaiting an unrelated Endless next-duel screen after lethal. NOT a full profile
  PASS. Final narrower HUD/lethal plus Slayer rerun **65 checks PASS/74.512s**:
  enemy180HP,score772,27.268combat seconds,HP40; fade completes after lethal,
  frame closes/fonts24/16 restore even after actor destruction, pause/R restarts
  with score0/full bars. Chromium153/SwiftShader,2CPU/8GiB/shm1GiB,0provider reads/
  writes/exceptions; fixture/network blocking retained. Next-duel progression is
  outside this HUD change and is not claimed verified.
- Same final player, Edge155 native: rapid Slayer **56 checks PASS/25.629s**,
  score155/x3red, exact awards/center during pulse, pause/resume/exit/restart;
  active Slayer four-size capture **25 checks PASS/19.754s**, all0provider IO and
  exceptions. Reviewed actual active640x360,960x540,1280x720,1920x1080 frames,
  all eight software normal/Slayer frames, cue/typing, lethal, and100ms entry/exit/
  rapid-award sequences. Tutorial reading panels obscure the underlying score;
  separate active native frames establish visibility without those panels.
- Independent screenshot pixel check960x540: visible gap exactly4px; score/combo
  centers480px; visible whole-group center104px equals208px upper-frame center.
  No clipping or score/tray/cue overlap in active gameplay. These are functional/
  visual results, not device-FPS or speaker-audio claims.
- Earlier failures: TMP fixture lacked Canvas; added it and the existing
  Cinemachine test-assembly reference. Initial textBounds used font line metrics,
  producing excessive visible gap despite matching its own bounds; replaced with
  per-glyph metrics excluding quad padding. Tests/probe independently use origin/
  baseline/bearings. Driver Skip/850HP assumptions were adapted to existing dirty
  Endless180HP intro; wait until it is ready, use native Enter, and test lethal
  restart through pause/R without changing gameplay or provider behavior.
- Wireframe r4 preserved/check PASS; r5 rendered/captured with editor closed,
  reviewed/check PASS. Fingerprint
  `75be18aa46b4bedddb86b186a079b53e7c64984f46d56c6cb60922b1366402c9`.
  Runtime/builders/GUIDs/diff/JS syntax/whitespace reviewed. Existing dirty Endless,
  gameplay and runner work preserved; no branch/worktree/commit/push/publication.
- Written receipts preceded cleanup. Removed local/server slayer-score task temp
  and7 snapshot-verified owned result directories. No task container remains.
  Other-session143344,shared source/Library/toolchain/builds and all other sessions
  preserved. Final whitespace/JS/source/binding review PASS. HUD acceptance complete;
  no production build/publication and no remaining HUD implementation/checks.
