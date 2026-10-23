# CLACKBLADE — Combat HUD design review

Bộ duyệt hình độc lập với Unity. Mở [index.html](index.html) bằng trình duyệt;
không cần server, đăng nhập hoặc mạng. Chưa áp dụng hai bố cục vào game.

## Xem và so sánh

- Chọn trạng thái, từ ngắn/dài, kích thước xuất 640×360 hoặc 1280×720.
- **Vùng bảo vệ** hiện bao thủ công cho frame dùng trong board, không phải
  kết quả dò va chạm HUD/nhân vật mỗi frame.
- **Chạy storyboard** minh họa cue/trạng thái; **Lưu PNG** xuất từng phương án.
- [Video 12 giây](exports/comparison-storyboard.mp4) cũng xem được ngay dưới hai hình.
- Ảnh native: [PA1 thường](exports/PA1-typing-640.png), [PA1 Slayer](exports/PA1-slayer-640.png),
  [PA2 thường](exports/PA2-typing-640.png), [PA2 Slayer](exports/PA2-slayer-640.png).
  Bản1280×720 nằm cùng thư mục `exports`.
- Bảng đủ 17 trạng thái: [PA1](exports/PA1-states.png), [PA2](exports/PA2-states.png).

## Quyết định trình bày

Tọa độ logical 320×180, gốc trái trên. Vẽ ở lưới gốc rồi scale nguyên 2×/4×,
không làm mượt ảnh pixel. Cobblestone, khung/keycap/icon lấy từ asset project;
không tạo art/font/audio mới cho game.

| Thành phần | PA1 — hai tầng | PA2 — ba ô |
|---|---|---|
| Khay | x56 y142,208×36 | x28 y152,264×26 |
| Cue | x116 y128,88×12 | x116 y136,88×12 |
| Từ chính |16 px, giữa x160 |16 px, giữa x90 |
| Preview |10 px, giữa x133/x187 |10 px, giữa x187/x259 |
| Timer | x62 y175, rộng 196 | x32 y174, rộng 112 |

Header chung: player x8–104, score x112–208, enemy x216–312; y4–42.
HP và Guard cùng nhóm; điểm thưởng có ô cạnh multiplier dưới tổng score.
Slayer hiện Guard rỗng/BREAK, tắt cue, giữ nguyên khay. Số HP đặt cạnh tên,
không chồng lên thanh. Cue PA1 hạ **2 px** so với plan để tăng khoảng hở;
không đổi cỡ chữ/camera/khay.

Màu: chữ #F2E7CF, preview #C5B9A7/#938B80, tiến độ #73B6AA, cue #E5BE72,
báo sai #F16B67, nền #14111E. Khung không đổi kích thước theo từ; clip màu
phần đã gõ trên cùng toàn từ để giữ nguyên tâm.

## Kết quả và giới hạn

- **15 kiểm tra trang review đạt**,0 script error: offline,17 trạng thái cả hai
  bố cục, chọn từ/kích thước, exportPNG, chạy/dừng, năm reference và không
  tràn ngang tại 640/1280. Xem [receipt](verification.json).
- Pool có **220 dòng,219 từ sau lowercase/deduplicate**: `I`→`i`, lặp `most`.
  Kiểm đủ 219 từ: rộng nhất `remember`,52 logical px ở cỡ chính và32.5 ở preview.
  `different` nhiều ký tự nhất vẫn dùng trong hình. Không autoshrink/bỏ preview.
- Đã xem nativePNG hai kích thước và bảng trạng thái. Frame mẫu không có chữ/cue
  đè nhân vật hoặc nhóm HUD khác. Preview10 px vẫn cần người chơi đánh giá tại
  kích thước thật; ảnh thu nhỏ trong trang không thay nghiệm thu.
- Đã xem **144/144 frame,143/143 chuyển tiếp** của MP4 cuối, riêng **60/60 frame,
 58/58 chuyển tiếp** hai đoạn camera vào/ra Slayer. Xem [visual review](visual-review.json).
  Đây là review UI/camera, không phải sửa hoặc nghiệm thu sprite animation.
- Bốn frame nền được giữ nguyên. Header/footer mới phủ vùng UI cũ; vùng giao
  chiến không retouch, dịch nhân vật hoặc sửa camera. Storyboard đổi giữa các
  frame tĩnh và minh họa UI; **không phải gameplay chạy cùng HUD mới**.
- Capture player cũ dùng keyboard/mouse, QA chỉ đọc, fixture/network isolation:
  sword/double strike/heavy/guard/elite, có sword/shield Attack1/2;
  không exception/provider write. Chỉ dùng làm nguồn hình. Classic và axe Attack2
  chưa có footage. Không tuyên bố đạt khoảng hở tối thiểu4 px trên mọi frame/clip.
- Browser dùng TTF gốc; raster/SDF chính xác, hitstop/Pause đồng bộ đòn, thrust,
  hủy đòn/lifecycle và bấm Space thật ở mốc180 ms chưa xác minh cho hai bố cục.
  Chỉ kiểm trong vòng Unity sau khi người dùng chọn thiết kế.

## Nguồn và tái tạo

[Reference evidence](reference-evidence.json) ghi nguồn chính thức, đoạn video,
frame/chuyển tiếp đã xem và khoảng trống. [Source evidence](source-evidence.json)
ghi snapshot player, PTS và SHA256. Video PTS là mốc chọn hình; QA gần nhất
chỉ cung cấp ngữ cảnh, không đồng bộ chính xác với frame video.

Artifacts đã đóng gói offline. Tái tạo bằng Playwright hiện có trên máy:

```powershell
$env:PLAYWRIGHT_MODULE = 'đường-dẫn-module-playwright-hiện-có'
node design/combat-hud-review/verify.cjs --video-frames
python design/combat-hud-review/render_media.py --ffmpeg <ffmpeg.exe> --review-cache <cache> --source-video <capture.webm>
```

Mặc định channel`msedge`; có thể đổi`PLAYWRIGHT_CHANNEL`.
`prepare.py --capture-root <root> --references <cache> --ffmpeg <ffmpeg.exe>`
đóng gói lại từ resource repo/capture gốc. Cache phiên này:
`E:/CodexTemp/TypingHero/typing-hud-ab-demo/design-review-source` và
`C:/Users/ADMIN/AppData/Local/Temp/clackblade-hud-reference-cy9ehna5`.
Không cần cache để xem artifacts. Giữ trong vòng duyệt, có thể dọn sau khi
chọn/hủy phương án. Không có server nền cần duy trì.
