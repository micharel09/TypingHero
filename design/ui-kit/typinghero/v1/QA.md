# Nghiệm thu bộ thiết kế v1

19/10/2026 — **đạt phạm vi gói asset và mockup để review**. Chưa tích hợp hoặc kiểm chứng trong Unity.

## Kiểm tra thực chạy

| Hạng mục | Kết quả và bằng chứng |
| --- | --- |
| Độ phủ | 113 asset có PNG + SVG; 30 mockup × 2 kích thước. Source mapping nằm trong README. Đủ bốn Collection, sáu bài tutorial, pause, kết quả, Continue và menu/HUD |
| PNG/SVG/atlas | `source/verify.py` PASS: kích thước khớp metadata; XML hợp lệ; từng rect atlas trùng byte với PNG; border nằm trong kích thước asset |
| Alpha | 113 PNG UI alpha chỉ 0/255, RGB tại alpha 0 bằng 0. Các layer artwork dùng trong mockup cũng sạch RGB trong suốt. Hai artwork AI gốc giữ nguyên, không dùng làm bản importer |
| Pixel | 30 cặp 1280×720 trùng byte với nearest-neighbor 2× của 640×360. Đã xem menu, HUD, Slayer, stress, bảng điểm, Opponents, Achievements, pause/results và contact sheet |
| Panel co giãn | Mockup dùng cùng hàm nine-slice: modal 128×96 → 346×286; row 160×28 → 563×29; frame HUD → 316×92; góc và viền giữ độ dày, không thấy nứt/méo ở ảnh review. Đây là kiểm chứng renderer của kit, chưa phải Unity importer |
| Nội dung biên | Tên rộng `WWWWWWWWWW` (10 ký tự) ở menu/row/HUD; điểm `2,147,483,647`; 999 combo/x16; HP 1/100; từ dài `counterstrike`; error tên trống và nhập sai. Text fitter kiểm rộng trong quá trình build |
| Async/locked | Collection loading/empty/error; opponent chưa gặp; achievement locked/unlocked. Error không thêm Retry vào luồng source, hướng dẫn đóng/mở lại |
| Trạng thái control | Contact sheet và nguồn đối chiếu normal/hover/pressed/focus/disabled, on/off, selected/error. Viền ivory cho focus, insert/nền cho pressed, giảm sắc disabled |
| Intro media | GIF tổng 2400ms. `ffprobe` MP4: H.264 640×360, 25/1 fps, 60 frame, duration 2.400000s; chỉ một video stream, không có audio |
| Gallery Chrome | `source/verify-browser.cjs` PASS: mở bằng file URL, font/ảnh/JS tải đủ; intro tự kết thúc; replay; nút và Esc bỏ qua; search ra đúng 5 primary state; lọc brand đúng 4 asset; mở/đóng dialog; không tràn ngang ở viewport 640 và 1280 |
| Liên kết/ZIP | `source/verify.py` kiểm mọi href/src local trong gallery và ZIP byte parity với toàn bộ file bàn giao, không nhúng chính ZIP vào ZIP |

Lệnh artifact: `python design/ui-kit/typinghero/v1/source/verify.py`.

Lệnh browser đã chạy trên Windows, dùng playwright-core có sẵn và Chrome headless riêng:

```powershell
node design/ui-kit/typinghero/v1/source/verify-browser.cjs E:/codex-data/packages/rea/node_modules/playwright-core/index.js 'C:/Program Files/Google/Chrome/Application/chrome.exe'
```

Không cài dependency, không dùng phiên browser đăng nhập của user. Các check không gửi dữ liệu game ra mạng.

## Sửa sau lượt kiểm đầu

- Làm sạch RGB vô hình trên bản crop sprite hero/skeleton; giữ nguyên pixel nhìn thấy và source gốc.
- Crop skeleton theo đúng import rect 96×64.
- Error tên trống giữ selector độ khó, đưa helper dưới Play; tên 10 ký tự có mockup riêng.
- Keycap Esc/R/M/A dùng glyph đúng; Backspace/Space/Enter dùng symbol.
- Gallery bổ sung rule `[hidden]` và giữ escape trong JavaScript để filter/skip hoạt động đúng. Kiểm lại Chrome đã PASS.
- Script QA giải mã ảnh lazy bằng cách chuyển chúng sang eager trước `decode`; không thay hành vi gallery.

## Phần chưa kiểm chứng

Binding/navigation/typing/leaderboard thật, importer 9-slice Unity, focus/input Unity, hiệu năng GPU/WebGL và gameplay đều thuộc bước tích hợp sau review. Không dùng ảnh/clip này làm bằng chứng các phần đó. Artwork AI gốc có pixel cluster và kích thước khác canvas yêu cầu; bản chuẩn hóa 640×360 mới là đầu vào layer của kit. Sprite nhân vật không được vẽ lại.

Gói để user review chung; không sửa scene, GUID hoặc serialized reference, không push/deploy. Không có service hoặc tài nguyên tạm cần giữ để xem gói.
