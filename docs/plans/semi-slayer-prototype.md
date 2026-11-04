# Semi Slayer prototype — checkpoint

## Mục tiêu và phạm vi

Prototype tách nhánh để thử độ đa dạng giữa các trận và thêm một kỹ năng chủ động
cho tuyến Endless hiện tại. Không thay đổi `main`; không ghi kỷ lục Endless cục bộ
hay đưa điểm vào hàng chờ Firebase khi `SemiSlayerMode` gắn trên người chơi.

| Hành vi | Cấu hình đã chốt |
|---|---|
| Kích hoạt | Nhấn Left Shift khi đủ 100; chỉ bắt cạnh nhấn, không buffer |
| Nạp | Từ hoàn tất trong combat thường +10; parry thành công +20; tối đa 100 |
| Vòng đời | Bắt đầu rỗng; giữ qua Continue; Restart/Menu tạo hoặc xóa run thì rỗng |
| Chuỗi | 10 đòn tự động, S1 → S2 → S3 lặp lại; mở hitbox 35 ms, cách nhau 100 ms |
| Sát thương | 5 HP mỗi đòn; không cộng điểm/nạp lại/tháo stamina; không hitstop |
| Địch | Hủy swing/chuỗi đang chuẩn bị, giữ địch, chờ recovery hiện tại rồi đánh lại |
| Gõ | Giữ word/progress, timer và combo; timer combat vẫn chạy, trừ Pause |
| Hình ảnh | Dùng clip và afterimage hiện có; không vào global Slayer/đổi nền |

## Cơ sở tái sử dụng và thay đổi

- `PlayerSlayerMode`, `PlayerAttackEvents`, `PlayerWeaponHitbox`, `SlashVolume` và
  `SlayerAfterimagePool` là đường chém, hitbox, stamina, trail và VFX đang có.
  `PlayerSlayerSwing` tách thao tác một nhát để Slayer đầy đủ và Semi Slayer dùng
  chung clip S1/S2/S3; ngữ cảnh Semi đặt 5 damage, không tháo stamina và bỏ hitstop.
- `SkeletonController` nhận owner lock riêng, dùng `AttackStrike.CancelSwing()` và
  reaction hiện tại; không gọi stun/guard-break để khóa địch.
- `TypingManager` và `ScoreSystem` tạm giữ timer/progress trong kỹ năng; `EndlessRun`
  giữ charge xuyên scene reload Continue. Charge chỉ tồn tại trong run.
- `CombatR7Build`/`CombatR7Status` dùng secondary row và thanh stamina đã có; label
  SHIFT nằm cạnh thanh và sprite vàng có sẵn báo đủ 100. Không thêm art/dependency.
- `GameLoopProbe` chỉ đọc state/hit frames trong build QA; `semi_slayer.cjs` dùng
  keyboard input thật qua harness WebGL hiện có.
- `EndlessDuel.FinishRun()` bỏ qua `SaveBest()` và `EndlessLeaderboard.Queue()` khi
  người chơi có SemiSlayerMode; score vẫn nằm trong run/session để trình bày.

## Nghiệm thu và giới hạn

- PASS — WebGL charge trace nhận +10 ở mười lần hoàn tất từ combat thường và dừng ở
  100. Quick test của `EndlessRun` xác nhận giữ 70 qua Continue và reset về 0 sau
  Restart. Quick `SuccessfulParryAddsTwentySemiSlayerCharge` gọi event thành công
  của `ParrySystem` trên scene gameplay thật và xác nhận charge 40→60.
- PASS — WebGL quan sát 10 attack ID và frame riêng; địch mất đúng 50 HP tổng
  (120→70), stamina giữ 50, score không đổi, hitstop và global Slayer presentation
  đều tắt. Giữ word/timer/combo trong chuỗi; combat clock chạy. Pause giữ chuỗi ở
  swing 6, không catch-up; Shift trong Pause không buffer và giữ Shift không tự dùng
  lại kỹ năng. Kết thúc chuỗi nhả lock gõ/địch.
- PASS — HUD được kiểm tra ở 320×180 và 640×360; ảnh report không cho thấy chồng lên
  HP hoặc typing tray. Bài Quick hiện có xác nhận prototype không ghi local best hay
  pending leaderboard.
- LIMIT — Code lên lịch swing cách nhau 0.1s. WebGL SwiftShader đo interval ngoài
  Pause 0.156–0.715s; quãng Pause là 1.294s. Check tự động xác nhận không có hai hit
  dưới 75ms, nhưng nhịp 100ms thực tế chưa được chứng minh trên runner này.
- UNVERIFIED — Browser không ghi nhận lần parry thành công: frame của SwiftShader dài
  khoảng 307ms ở snapshot cuối và không lấy được sample trong cửa sổ. Test Quick xác
  nhận event-to-charge +20, nhưng timing parry bằng input thật chưa được xác minh.
- UNVERIFIED — Gameplay hiện không có điều khiển quay trái; WebGL chỉ ghi hướng hiện
  tại. `PlayerSlayerSwing` và `SlashVolume` đọc `SpriteRenderer.flipX`, nhưng kết quả
  runtime khi quay trái chưa được kiểm tra.
- PASS — Đã xem trực tiếp sáu sheet `male_hero-combo_{1,1_end,2,2_end,3,3_end}.png`
  không có VFX và video runtime 50.50–58.76s với VFX. Review đủ 206 frame và 205
  transition, giữ nguyên frame lặp và các frame Pause/Resume. Slow-motion playback
  riêng chưa được ghi.
- UNVERIFIED — Chưa có video/reference ngoài cho motion Semi Slayer. Trang public
  CLACKBLADE chỉ mô tả Slayer hiện tại; không cung cấp động tác cho mode mới. So khớp
  motion với nguồn ngoài chưa kết luận được.
- PASS — Khi `SemiSlayerMode` tồn tại, kết thúc Endless không ghi local best hoặc
  pending leaderboard theo Quick test.

## Evidence log

- 2026-11-04 resumption: the prototype branch/worktree is unchanged at
  `feat/semi-slayer-prototype`; the pre-existing Cobblestone asset edit remains
  outside this task. Regression fixtures are being added for Semi trail handoff,
  synchronous lethal cleanup, player hit/death animation and teardown. Current
  source still lacks a Semi trail-end call; enemy death still requests abort
  cleanup, and the idle gate rejects completed combat. Run the new focused tests
  before changing runtime code to record the baseline failure.
- RED baseline — Ubuntu pinned Unity `20261104T021600-quick`, snapshot
  `044899febf8139d023d8ef5a5048c354f908004609d13a3a7ce9d60a874a84d8`:
  both Semi trail handoff cases fail because `slayerSignalBound` remains false.
  `20261104T022358-quick` reproduces a real lethal first hit; target dies and
  `CompletedSwings=1`, but cleanup leaves `swingDelay=0.1`. The first scene test
  stopped at that assertion before checking idle. An attempted `Assert.Multiple`
  aggregation did not compile because this project's NUnit lacks that API; the
  next attempt now checks idle first using supported ordered assertions.
  `20261104T023102-quick` fails that assertion: the live player's current state
  never returns to `player_idle` after lethal enemy damage. Unity emitted
  IL postprocessor `/tmp/ilpp.sock` warnings, but both runs produced test XML and
  executed the selected cases. Container exit code 2 reflects failed assertions.
- Nhánh/worktree: `feat/semi-slayer-prototype` tại
  `D:/Projects/TypingHero-worktrees/semi-slayer-prototype`; tạo từ `origin/main`
  `652ad93b26dc65ade1c91d22b4e2ee68e7314e24`. Checkout gốc được giữ nguyên.
- Reference research: [trang public CLACKBLADE](https://micharel09.itch.io/clackblade)
  ghi mô tả Slayer thường, không có clip motion Semi Slayer; link TYPECAST/The
  Textorcist trong quyết định tính năng cũng không phải reference cho động tác này.
  Vì vậy đối chiếu motion ngoài vẫn **unverified**.
- 2026-11-03 Ubuntu Quick baseline: 15/15 PASS ở
  `/data/typinghero-game-loop/results/20261103T073603-quick`. Test bổ sung
  `SuccessfulParryAddsTwentySemiSlayerCharge` PASS 1/1 với
  `python tools/game_loop/run.py quick --test-filter SuccessfulParryAddsTwentySemiSlayerCharge --wait-seconds 900`.
- 2026-11-03 WebGL baseline `/data/typinghero-game-loop/results/20261103T084708-webgl`
  PASS 52 checks. Lượt thử đo parry bằng browser bị fail ở assertion mới; receipt
  `/data/typinghero-game-loop/results/20261103T091449-webgl/browser.json` cho thấy
  hai lần bấm tại impactIn 0.64–0.66s do frame dài, nên kết quả đó không dùng để
  kết luận gameplay. Sau khi chuyển +20 sang Quick event test và giới hạn frame
  compensation của browser QA, WebGL PASS 52 checks ở
  `/data/typinghero-game-loop/results/20261103T094158-webgl/browser.json`; bản sao
  report lưu tại `D:/Projects/TypingHero-evidence/semi-slayer-prototype-20261103/`.
- Visual ledger: video `page@a05c6aa63928da0cfb6ae15a5e9b7fb2.webm`, đoạn
  50.50–58.76s; 206 frame/205 transition đã xem. Ảnh theo frame nằm trong
  `D:/Projects/TypingHero-evidence/semi-slayer-prototype-20261103/sprite-review-segment-50.50-to-end/`.
- Report WebGL cuối: 10 hit ID riêng; hit frame `[150,151,153,154,155,157,161,162,163,165]`;
  intervals `[0.212,0.156,0.210,0.397,0.335,1.294,0.378,0.599,0.715]` giây.
  Pause tại swing 6 chiếm gap 1.294s. Word/combo delta 0; enemy HP 120→70; stamina
  50→50; hitstop 0; parry browser count 0; Continue browser count 0.
- 2026-11-04 focused lifecycle Quick receipts on the pinned Ubuntu Unity runner:
  `/data/typinghero-game-loop/results/20261104T025300-quick` SemiSlayer filter PASS 9;
  `20261104T025536-quick` player hit/death animation PASS 1;
  `20261104T025741-quick` existing Slayer expiry and Pause/Resume PASS 1. The added
  scene cases cover lethal swings 1/5/10 and boss completion, one synchronous cleanup
  with no next swing delay, closed hitbox, released locks, restored hit window/speed and
  idle for a living player; damage, death and teardown preserve their own animation.
  Trail fixtures verify the next ordinary cel regains the normal material after Semi.
- Browser receipt `/data/typinghero-game-loop/results/20261104T062654-webgl-retry23/browser.json`
  is **failed** (64 checks): actual keyboard READY advanced from charge 100 to a fresh
  encounter at enemy HP 200 while retaining charge 100. Player had 20 HP before READY,
  then died during target setup with charge 100 and enemy HP 108; the terminal was recorded
  and the scenario stopped before the lethal skill. The 1× recording is
  `D:/Projects/TypingHero-evidence/semi-slayer-prototype-20261104/semi-slayer-retry23-1x.webm`;
  its 0.5× playback is saved beside it. These clips document a terminal run, not the full
  lethal acceptance sequence. An earlier retry reported `passed` after silently returning
  on player death; that result is excluded. The driver now fails and records terminal
  states instead of counting them as a completed scenario.
- Visual review of the 1× recording `semi-slayer-1x.webm` is complete for both selected
  affected segments: 54.80–66.76s (300 frames/299 consecutive transitions; Semi trail,
  Pause/Resume, expiry and ordinary-trail handoff) and 78.00–94.24s (407 frames/406
  transitions; typed READY, scene load, fresh encounter and subsequent player states).
  All 707 frames and 705 transitions were directly reviewed in order; held frames were
  retained. Contact sheets and one-row-per-frame ledger are in
  `D:/Projects/TypingHero-evidence/semi-slayer-prototype-20261104/frame-review/` and
  the evidence root. The 0.5× p15 clip preserves the same source frames; it does not add
  another gameplay run. The p23 terminal recording is not represented as fully reviewed.
- p23 SwiftShader rendered about 311 ms per sampled frame. Its first skill still reported
  ten distinct hits, enemy HP120→70 and the ordinary follow-up at60 HP with one active
  slash part. Hit gaps were `[0.483,0.212,0.425,0.350,1.239,0.388,0.605,0.398,0.314]`
  seconds; the 1.239s gap includes Pause. This does not verify the configured 100 ms cadence.
  A match parry count of one and repeated missed keyboard timing samples do not establish
  reliable parry timing. Facing-left and external Semi Slayer motion comparison remain
  unverified.

## Trạng thái

Ba lỗi lifecycle đã được sửa và focused Quick checks pass. Browser xác nhận nonlethal
trail và READY giữ charge qua encounter mới, nhưng chưa xác nhận lethal skill trên video:
player chết khi chuẩn bị target. Nhịp100ms thực tế, parry timing bằng input thật, hướng
trái và reference motion ngoài còn **unverified**. Không tích hợp `main`.
