# Combat HUD A/B — checkpoint

## Trạng thái hiện tại — 2026-10-23

Người dùng bác bố cục B trên đầu skeleton: chữ, điểm thưởng và nhân vật/vũ khí
tranh chỗ; thứ bậc và chất liệu chưa phù hợp. Người dùng đã duyệt plan **làm bộ
thiết kế xem trước, chưa áp dụng gameplay** và yêu cầu implement.

Đã dựng [bộ review](../../design/combat-hud-review/README.md): hai phương án cố định
dưới màn hình,17 trạng thái chọn được, năm nguồn tham khảo, tám PNG native
640×360/1280×720, hai bảng trạng thái, ảnh vùng bảo vệ và MP4 storyboard.
Trang mở offline, exportPNG và chạy/dừng. Giai đoạn này không sửa Assets/scene/runtime.
Không merge main trước khi người dùng chọn bố cục.

## Bằng chứng thiết kế

- [Verification](../../design/combat-hud-review/verification.json):15 kiểm tra trang
  review đạt,0 script error.219 từ chuẩn hóa từ220 dòng vừa cả hai bố cục ở cỡ
  chính16/preview10 logical px, không autoshrink.
- [Visual review](../../design/combat-hud-review/visual-review.json):144/144 frame,
 143/143 chuyển tiếp MP4; hai cửa sổ camera vào/ra Slayer tổng 60/60 frame,
 58/58 chuyển tiếp. NativePNG và trạng thái đã xem trực tiếp.
- [Reference evidence](../../design/combat-hud-review/reference-evidence.json):
  Type Knight 00:08–00:19,660 frame/659 chuyển tiếp; Furi 00:47–01:00,
 390 frame/389 chuyển tiếp, đã xem đầy đủ. NecroDancer mới xác minh ảnh lane tĩnh.
- Capture mới từ player cũ bằng input thật: sword/double strike/heavy/guard/elite;
  sword/shield có Attack1/2,0 exception/provider write. Chỉ là nguồn hình.
- Classic, axe Attack2, khoảng hở mọi frame, telegraph/Pause/hitstop/thrust và
  Space thật180 ms chưa xác minh cho hai bố cục mới. TTF browser không thay kiểm SDF Unity.
  Thông số/giới hạn và cách tái tạo nằm trong README của bộ review.

## Demo B cũ — giữ riêng, chưa được duyệt hình

Worktree`C:/Users/ADMIN/orca/workspaces/TypingHero/typing-hud-ab-demo`, nhánh
`micharel09/typing-hud-ab-demo`, base`origin/main` e5bbc76.
Thay đổi runtime A/B cũ còn riêng trong working tree, không vào commit thiết kế.
Không tự reset/stash hoặc thay giao diện mặc định.

Demo cũ gồm`TypingHudProfile`, presenter B đọc queue/timer từ TypingManager,
API đọc`TryGetTelegraph`, projection ổn định và F8 khi Pause/development build.
Giữ camera/input/reward/damage/scoring/audio/cửa sổ parry. Test hành vi đạt
không đồng nghĩa bố cục đạt. B tutorial còn bị guide panel che.

Bằng chứng cũ giữ nguyên phạm vi:

- Unity 68/68, snapshot`c11fcab210a5732b3fb1fff80142a24e42e5e3d1c6ace47d8265290fd662d65e`,
  `/data/typinghero-game-loop/results/20261023T132225-quick/tests.xml`.
- Development build/repeated builder:
  `de7d18c2eb42ef5289e0d9a6f0fd2a3127430e68e7a878c2d289d0dce2384f60`,
  result`/data/typinghero-game-loop/results/20261023T132859-webgl`.
- Native-final 35 kiểm tra hành vi đạt,640×360/1280×720,0 exception/provider write;
  **không có visual acceptance**.
- Coverage-draft từng fail assert nhầm vai trò cuối, không tính pass toàn lượt.
  Berserker/axe Attack2 không xuất hiện trong vòng finite đầu, không có footage.

## Bước tiếp theo và tài nguyên giữ lại

Người dùng xem PA1/PA2, chọn hoặc chỉnh thiết kế. Chỉ sau đó mới dựng vòng Unity
và kiểm font/collision/timing. Không nới cửa sổ parry, giảm chữ, bỏ preview hoặc
đổi camera để ép pass.

Giữ worktree/nhánh thử để duyệt, cùng`E:/CodexTemp/TypingHero/typing-hud-ab-demo`
chứa player/capture/cache/driver fixture. Không có server mới cần giữ chạy.
Chỉ dọn tài nguyên task sau khi chọn/hủy và kết quả cần giữ đã được lưu.
Không chạm checkout gốc đang dirty.
