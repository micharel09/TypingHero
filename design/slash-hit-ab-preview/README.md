# Preview A/B vết nhận chém

## B đổi góc mỗi nhát — 25/10/2026

Phạm vi mới: chỉ player/template preview. Dùng lại nguồn B và SHA-256 đã khóa bên
 dưới; không mua asset, sửa GIF, tích hợp Unity hoặc đổi API game. Những nhận xét
không xoay ở phần kiểm bản gốc bên dưới được thay bằng contract sau.

- Mặc định Trắng, “Đổi góc mỗi nhát”; chọn đều tám góc 0°–315°, bước 45°,
  loại góc liền trước. Góc tính theo chiều kim đồng hồ trên canvas; 0° là artwork gốc.
- Chu kỳ B: F4 (80 ms) → F5 → F0 → F1 → F2 → F3 (mỗi frame còn lại 100 ms).
  Tổng 580 ms ở 1×, 2.320 ms ở 0,25×. Chỉ F3→F4 chọn góc mới khi phát/tiến frame.
  Bước lùi giữ góc để kiểm hình; dừng/tiếp tục giữ góc. Phát lại bắt đầu F4, nhát mới.
- Canvas B 384×384; ô nguồn 256×256 xoay quanh tâm (192,192), không nội suy.
  Góc chéo có răng cưa/hạt dạng hình thoi do nearest-neighbor, không làm mờ.
  “Góc gốc” luôn 0°. Montage tám màu vẽ nguyên nguồn, không xoay cả bảng.
- A giữ artwork, lựa chọn và thời gian cũ. Cả hai vùng xem cao 410 px để chứa B.

### Kiểm đầu ra và bằng chứng

Mở HTML offline bằng Microsoft Edge 156 headless, xác nhận controls được bật và
“Sẵn sàng · dữ liệu offline từ preview tác giả.”; không có page error. Ảnh UI:
[edge-ready.png](local/edge-ready.png). Dựng lại bằng `build.py`, SHA-256 cả ba nguồn
khớp; `node --check player.js` và `git diff --check` đạt.

Kiểm biên thời gian bằng đồng hồ chủ động: 16 chu kỳ mỗi tốc độ, giữ frame tới
đúng duration, đổi góc chỉ ở nhát mới. Kiểm đủ 56 cặp lựa chọn trước→sau, mỗi góc
có đúng bảy khả năng còn lại. Kiểm dừng, tiếp tục/phát lại, tiến/lùi, tám màu,
Góc gốc và montage. Kiểm riêng requestAnimationFrame thực trong Edge: 2,6 giây
mỗi tốc độ, đủ sáu frame, sai lệch từng khoảng hoàn chỉnh <50 ms; giữ góc giữa
nhát, đổi ở F3→F4. Số đo và script cục bộ: [verification.json](local/verification.json),
[verify.cjs](local/verify.cjs) (dùng playwright-core có sẵn của môi trường).

Trực tiếp mở tám bảng canvas Edge bên dưới, mỗi bảng có sáu ô 384×384, đọc theo
hàng F4→F5→F0→F1→F2→F3. Đã xem 48/48 frame và 48/48 cặp chuyển tiếp của tám góc,
gồm pha tan→nhát mới. Bảng ghi quan sát trực tiếp; số đo timing là kiểm kỹ thuật
riêng, không thay thế kiểm hình. Không thấy clipping hoặc pixel bị nội suy.
Giới hạn: đây là preview quảng bá trên nền đen; runtime Unity vẫn chưa kiểm chứng.
Media và bằng chứng nằm trong `local/`, không đưa lên remote. Có thể dựng lại HTML;
bảng ảnh/số đo là bằng chứng của lần kiểm này.

| Góc | Frame | Duration 1× / 0,25× | Đối chiếu | Quan sát frame | Chuyển tiếp kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|---|---|
| 0° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở trên phải, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [0° F4](local/angle-0.png) |
| 0° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục chéo lên phải, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [0° F5](local/angle-0.png) |
| 0° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [0° F0](local/angle-0.png) |
| 0° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục chéo lên phải. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [0° F1](local/angle-0.png) |
| 0° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [0° F2](local/angle-0.png) |
| 0° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [0° F3](local/angle-0.png) |
| 45° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở bên phải, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [45° F4](local/angle-45.png) |
| 45° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục ngang, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [45° F5](local/angle-45.png) |
| 45° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [45° F0](local/angle-45.png) |
| 45° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục ngang. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [45° F1](local/angle-45.png) |
| 45° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [45° F2](local/angle-45.png) |
| 45° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [45° F3](local/angle-45.png) |
| 90° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở dưới phải, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [90° F4](local/angle-90.png) |
| 90° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục chéo xuống phải, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [90° F5](local/angle-90.png) |
| 90° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [90° F0](local/angle-90.png) |
| 90° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục chéo xuống phải. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [90° F1](local/angle-90.png) |
| 90° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [90° F2](local/angle-90.png) |
| 90° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [90° F3](local/angle-90.png) |
| 135° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở phía dưới, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [135° F4](local/angle-135.png) |
| 135° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục dọc, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [135° F5](local/angle-135.png) |
| 135° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [135° F0](local/angle-135.png) |
| 135° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục dọc. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [135° F1](local/angle-135.png) |
| 135° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [135° F2](local/angle-135.png) |
| 135° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [135° F3](local/angle-135.png) |
| 180° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở dưới trái, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [180° F4](local/angle-180.png) |
| 180° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục chéo xuống trái, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [180° F5](local/angle-180.png) |
| 180° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [180° F0](local/angle-180.png) |
| 180° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục chéo xuống trái. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [180° F1](local/angle-180.png) |
| 180° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [180° F2](local/angle-180.png) |
| 180° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [180° F3](local/angle-180.png) |
| 225° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở bên trái, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [225° F4](local/angle-225.png) |
| 225° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục ngang, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [225° F5](local/angle-225.png) |
| 225° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [225° F0](local/angle-225.png) |
| 225° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục ngang. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [225° F1](local/angle-225.png) |
| 225° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [225° F2](local/angle-225.png) |
| 225° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [225° F3](local/angle-225.png) |
| 270° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở trên trái, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [270° F4](local/angle-270.png) |
| 270° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục chéo lên trái, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [270° F5](local/angle-270.png) |
| 270° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [270° F0](local/angle-270.png) |
| 270° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục chéo lên trái. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [270° F1](local/angle-270.png) |
| 270° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [270° F2](local/angle-270.png) |
| 270° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [270° F3](local/angle-270.png) |
| 315° | F4 | 80 / 320 ms | B nguồn F4, xoay tâm ô | Đầu vệt đặc ở phía trên, chấm nhỏ phía đuôi; còn khoảng đệm quanh ô. | F5 kéo dài từ cùng đầu vệt, góc giữ nguyên. | đạt | [315° F4](local/angle-315.png) |
| 315° | F5 | 100 / 400 ms | B nguồn F5, xoay tâm ô | Vệt dài hơn dọc trục dọc, lõi liền và đuôi nhọn. | F0 dài thêm và xuất hiện hạt; nối F5→F0 không đổi hướng. | đạt | [315° F5](local/angle-315.png) |
| 315° | F0 | 100 / 400 ms | B nguồn F0, xoay tâm ô | Vệt đạt chiều dài lớn, hạt tách cạnh đầu vệt; không chạm mép canvas. | F1 lõi đứt thành nhiều đoạn trên cùng trục. | đạt | [315° F0](local/angle-315.png) |
| 315° | F1 | 100 / 400 ms | B nguồn F1, xoay tâm ô | Lõi đã tắt, còn đoạn ngắn và hạt rời dọc trục dọc. | F2 các đoạn co ngắn, không trượt tâm hoặc đổi hướng. | đạt | [315° F1](local/angle-315.png) |
| 315° | F2 | 100 / 400 ms | B nguồn F2, xoay tâm ô | Chỉ còn mảnh nhỏ và chấm rời, cạnh sắc trên nền đen. | F3 giảm còn chấm thưa, giữ nguyên hướng. | đạt | [315° F2](local/angle-315.png) |
| 315° | F3 | 100 / 400 ms | B nguồn F3, xoay tâm ô | Hai chấm thưa, không còn vệt đặc; vùng quanh hoàn toàn tối. | F4 nhát mới hiện lại ở góc mới; giữ tâm ô, không xoay vệt cũ. | đạt | [315° F3](local/angle-315.png) |

Ngày kiểm: 25/10/2026. Phạm vi: preview quảng bá tác giả, không mua gói asset,
không đưa vào Unity, không dựng lại animation thiếu dữ liệu.

## Mở bản xem thử

Chạy từ gốc repo (Python có Pillow):

```powershell
python design/slash-hit-ab-preview/build.py
```

Mở [local/preview.html](local/preview.html). File HTML tự chứa dữ liệu và chạy
offline sau khi dựng. `local/` được Git bỏ qua; không đưa media của tác giả lên
remote. Cần Internet ở lần dựng đầu để tải ba GIF quảng bá công khai. Hash nguồn
đã kiểm được khóa trong `build.py`; nguồn đổi cần kiểm lại trước khi dựng.

Cách trình bày và điều khiển tái sử dụng cấu trúc bản xem thử
`design/player-parry-original/preview.html` và `preview.css`: controls, hai canvas,
caption và thông báo trạng thái. A/B cạnh nhau, tốc độ 1× hoặc 0,25×; dừng,
tiếp tục, phát lại, bước frame. Đổi mẫu/tốc độ đặt cả hai về đầu. Không đồng bộ
độ dài vòng lặp bằng cách thêm/bỏ frame. Bước frame tiến/lùi riêng mỗi clip.

## Nguồn và checklist trước đánh giá

- A: [Small slash FX — Sagak Art (Pururu)](https://sagak-art-pururu.itch.io/small-slash-fx).
  Checklist: đủ slash_hit 1, 2, 3; chuẩn bị/slash đi kèm, hit, tan, frame trống,
  chuyển mẫu và điểm nối lặp. Bảng ảnh gốc xác nhận nhãn/thứ tự ba mẫu.
- B: [Slash VFX — Met.Pxl](https://metpxl.itch.io/pixel-art-slash-vfx-8-color-animation-pack).
  Checklist: tám màu, nét chém chéo, hạt, tan và điểm nối lặp gốc.
- [A động](https://img.itch.zone/aW1hZ2UvMTk4NDQ5NC8xMTY2OTUzNC5naWY=/original/VlLIuI.gif):
  xem trọn 0–2.600 ms, 26 frame × 100 ms, canvas 376×314. Chọn toàn bộ vì
  chứa cả ba slash + hit và sting, bao gồm kết thúc, không cắt giấu frame.
  SHA-256 `73add428aa4e91c0f01107ff9235ef12dd08ffb1ee409ed0f4b8eade556ba5e1`.
- [A bảng ảnh](https://img.itch.zone/aW1hZ2UvMTk4NDQ5NC8xMTY2OTM5MS5naWY=/original/l7UxUi.gif):
  1000×1320, một frame tĩnh. Đã xem đủ bảng, gồm ba hàng slash_hit.
  SHA-256 `7967ca59d73611d1d9668ef2bff8b483b8f7659a48b84cce70b55770bae1feae`.
- [B động tám màu](https://img.itch.zone/aW1nLzI4MDAzMDI0LmdpZg==/original/1%2B4p2y.gif):
  xem trọn 0–580 ms, sáu frame: 100/100/100/100/80/100 ms, canvas 1260×1000.
  Chọn toàn bộ để giữ đúng thứ tự tan rồi hiện lại qua điểm nối vòng lặp.
  SHA-256 `9a6e0a01cefb1b10a7db3b6c250259a1a1b3f414861f136654395674ab5cb139`.

Nguồn chuyển động ở đây là GIF công khai, không phải video riêng. Không tuyên bố
đã nghiên cứu video khác hay gói trả phí. Bốn GIF màu phóng riêng trên trang B
được xác định nhưng không dùng làm nguồn đầu ra; montage đã đủ tám màu.
Frame giữ/trống được giữ nguyên. Không đoán timing từ bảng ảnh A.

## Kết quả và giới hạn

Đã trực tiếp xem A 26/26 frame và 26/26 chuyển tiếp (gồm cuối→đầu), B 6/6 frame
montage chứa tám màu và 6/6 chuyển tiếp cho mỗi màu. Kiểm thêm điểm nối ba đoạn
A: F5→F0, F11→F6, F18→F12. A1: F0–F5 (6/6 frame, 6/6 chuyển tiếp);
A2: F6–F11 (6/6, 6/6); A3: F12–F18 (7/7, 7/7). B mỗi màu: 6/6, 6/6.
Các bảng dưới ghi quan sát frame/chuyển tiếp của nguồn; đầu ra dùng chính hình
nguồn đã giải mã, không có sprite mới cần đánh giá tạo hình riêng.

A1 có tia chéo dốc; A2 có dấu X rõ; A3 trải ngang với nhiều mảnh nhỏ. B có nét
chéo từ dưới trái lên trên phải, tan thành hạt và hiện lại trong cùng chu kỳ.
Màu đen và tím B tương phản thấp trên nền đen nguồn; giữ nguyên màu khi so sánh.
A có slash đi kèm ở các frame đầu, không thể coi là hit tách riêng từ gói mua.

Nền đen chung, A giữ kích thước nguồn; B chỉ cắt ô 256×256 trong montage,
không vẽ lại. Nguồn đã có mức phóng khác nhau nên không khẳng định kích thước
asset trong game tương đương. Không thay pivot, không mirror hay thêm hiệu ứng.
Độ rõ trên Ske, alpha/pivot gói mua và runtime Unity **chưa kiểm chứng**.

## Kiểm kỹ thuật và UI

- Dựng HTML bằng Pillow; kiểm số frame, canvas, duration và SHA-256 nguồn.
- `node --check design/slash-hit-ab-preview/player.js` và `git diff --check`.
- Mở file cục bộ trong Orca, quan sát hai panel, nhãn tác giả, controls và nội dung
  giới hạn. Chọn A3, tốc độ 0,25×, dừng và bước trước/sau bằng controls thực.
- Đối chiếu pixel canvas trực tiếp với ảnh giải mã: mọi frame của bốn lựa chọn A
  và chín lựa chọn B khớp toàn bộ (B gồm tám màu và montage đầy đủ). Điều này
  kiểm bảo toàn artwork, không thay cho quan sát chuyển động.
- Đo playback độc lập trong Edge headless bằng profile tạm, mỗi tốc độ 2,5 giây:
  1× thấy đủ thứ tự F0→F5 qua nhiều vòng; 0,25× thấy đủ một vòng. Sai lệch khoảng
  thời gian từng frame trong ngưỡng 45 ms do lịch vẽ màn hình. Frame B F4
  80 ms được giữ (đo khoảng 80 ms ở 1×, 336 ms ở 0,25× so với danh định 320 ms).
  Orca eval cho lịch vẽ không đều khi đo, nên không dùng nó để kết luận timing.
- Không chạy Unity/WebGL vì không thay đổi game. Không có mua hoặc tích hợp asset.

## Bằng chứng từng frame

Để truy lại từng hình, mở HTML cục bộ, chọn toàn bộ montage A hoặc màu B,
dừng/phát lại và bước đến số `Frame nguồn F…` trên caption. Các hình là chính
frame trong GIF có URL và hash ở trên. Duration trong bảng là timing gốc;
chế độ 0,25× nhân bốn. Trạng thái “đạt” chỉ nói preview giữ được nguồn trong
phạm vi này, không nghiệm thu sử dụng trong game.

### A — toàn bộ GIF, gồm các frame của A1–A3

| Clip | Frame / thời điểm | Duration | Quan sát frame | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|---|
| A1 | F0 / 0 ms | 100 ms | Vệt slash ngắn phía trên phải | F1: cung dài hơn, lóe nhỏ xuất hiện | đạt | A động F0 |
| A1 | F1 / 100 ms | 100 ms | Cung dọc trắng và tia hit nhỏ bên phải | F2: cung quét xuống, hit kéo dài | đạt | A động F1 |
| A1 | F2 / 200 ms | 100 ms | Cung dưới trái, hit chéo dốc sáng | F3: cung co, hit rỗng giữa | đạt | A động F2 |
| A1 | F3 / 300 ms | 100 ms | Tia hit chéo phân nhánh, đuôi cung còn thấp | F4: cung mất, các nhánh tách | đạt | A động F3 |
| A1 | F4 / 400 ms | 100 ms | Các mảnh tia rời quanh đường chéo | F5: tắt sạch | đạt | A động F4 |
| A1 | F5 / 500 ms | 100 ms | Nền trống được giữ đủ 100 ms | F6 bắt đầu mẫu 2; đoạn A1 lặp về F0 từ trống | đạt | A động F5 |
| A2 | F6 / 600 ms | 100 ms | Vệt slash ngắn hơi ngang | F7: cung ngang nở, lóe tại đầu phải | đạt | A động F6 |
| A2 | F7 / 700 ms | 100 ms | Cung trắng ngang với hit nhỏ | F8: hit thành dấu X lớn | đạt | A động F7 |
| A2 | F8 / 800 ms | 100 ms | Dấu X đặc trắng, cung kéo trái | F9: X tách rỗng và cung thu | đạt | A động F8 |
| A2 | F9 / 900 ms | 100 ms | Bốn nhánh X mảnh, đuôi cung thấp trái | F10: chỉ còn đầu nhánh | đạt | A động F9 |
| A2 | F10 / 1000 ms | 100 ms | Mảnh rời ở bốn hướng, giữa đã tối | F11: tắt sạch | đạt | A động F10 |
| A2 | F11 / 1100 ms | 100 ms | Nền trống | F12 bắt đầu mẫu 3; đoạn A2 lặp về F6 từ trống | đạt | A động F11 |
| A3 | F12 / 1200 ms | 100 ms | Slash nhỏ phía trên trái | F13: cung kéo chéo xuống phải | đạt | A động F12 |
| A3 | F13 / 1300 ms | 100 ms | Cung dài, đầu trắng ở dưới phải | F14: đầu phải lóe hit ngang | đạt | A động F13 |
| A3 | F14 / 1400 ms | 100 ms | Cung mỏng kéo trái, hit sáng nhỏ tại phải | F15: hit mở thành nhiều tia ngang | đạt | A động F14 |
| A3 | F15 / 1500 ms | 100 ms | Cụm tia dài, trục hơi chéo xuống phải | F16: lõi vỡ, nhánh tách nhỏ | đạt | A động F15 |
| A3 | F16 / 1600 ms | 100 ms | Các nhánh rời, hai cụm sáng còn lại | F17: ít mảnh và tối hơn | đạt | A động F16 |
| A3 | F17 / 1700 ms | 100 ms | Mảnh nhỏ phân tán theo trục ngang | F18: nền trống | đạt | A động F17 |
| A3 | F18 / 1800 ms | 100 ms | Nền trống | F19 bắt đầu sting; đoạn A3 lặp về F12 từ trống | đạt | A động F18 |
| Sting nguồn | F19 / 1900 ms | 100 ms | Vệt đâm ngang từ trái sang phải | F20: mũi tên trắng mở rộng | đạt | A động F19 |
| Sting nguồn | F20 / 2000 ms | 100 ms | Mũi đâm nhọn sáng ở phải | F21: mũi ngắn lại, hit nhỏ hiện tại phải | đạt | A động F20 |
| Sting nguồn | F21 / 2100 ms | 100 ms | Mũi đâm rút, tia hit dọc nhỏ | F22: hit nở vòng dọc | đạt | A động F21 |
| Sting nguồn | F22 / 2200 ms | 100 ms | Vòng hit dọc trắng, giữa tối, đuôi đâm còn | F23: vòng tách nhiều nhánh | đạt | A động F22 |
| Sting nguồn | F23 / 2300 ms | 100 ms | Các nhánh sáng quanh vùng giữa rỗng | F24: nhánh mảnh và thưa | đạt | A động F23 |
| Sting nguồn | F24 / 2400 ms | 100 ms | Mảnh nhỏ thành vòng dọc | F25: tắt sạch | đạt | A động F24 |
| Sting nguồn | F25 / 2500 ms | 100 ms | Nền trống cuối montage | F0: bắt đầu lại slash 1 sau khoảng trống, không nối pose giả | đạt | A động F25 |

### B — từng màu trong montage

| Clip / màu | Frame / thời điểm | Duration | Quan sát frame | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|---|
| B / Đỏ | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải | F1: vệt đứt và ngắn đi | đạt | B động, ô Đỏ, F0 |
| B / Đỏ | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Đỏ, F1 |
| B / Đỏ | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Đỏ, F2 |
| B / Đỏ | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Đỏ, F3 |
| B / Đỏ | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Đỏ, F4 |
| B / Đỏ | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Đỏ, F5 |
| B / Đen | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải; tương phản thấp trên nền nguồn | F1: vệt đứt và ngắn đi | đạt | B động, ô Đen, F0 |
| B / Đen | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo; tương phản thấp trên nền nguồn | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Đen, F1 |
| B / Đen | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời; tương phản thấp trên nền nguồn | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Đen, F2 |
| B / Đen | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt; tương phản thấp trên nền nguồn | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Đen, F3 |
| B / Đen | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải; tương phản thấp trên nền nguồn | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Đen, F4 |
| B / Đen | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng; tương phản thấp trên nền nguồn | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Đen, F5 |
| B / Xanh dương | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải | F1: vệt đứt và ngắn đi | đạt | B động, ô Xanh dương, F0 |
| B / Xanh dương | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Xanh dương, F1 |
| B / Xanh dương | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Xanh dương, F2 |
| B / Xanh dương | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Xanh dương, F3 |
| B / Xanh dương | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Xanh dương, F4 |
| B / Xanh dương | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Xanh dương, F5 |
| B / Xanh lá | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải | F1: vệt đứt và ngắn đi | đạt | B động, ô Xanh lá, F0 |
| B / Xanh lá | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Xanh lá, F1 |
| B / Xanh lá | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Xanh lá, F2 |
| B / Xanh lá | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Xanh lá, F3 |
| B / Xanh lá | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Xanh lá, F4 |
| B / Xanh lá | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Xanh lá, F5 |
| B / Hồng | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải | F1: vệt đứt và ngắn đi | đạt | B động, ô Hồng, F0 |
| B / Hồng | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Hồng, F1 |
| B / Hồng | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Hồng, F2 |
| B / Hồng | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Hồng, F3 |
| B / Hồng | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Hồng, F4 |
| B / Hồng | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Hồng, F5 |
| B / Tím | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải; tương phản thấp trên nền nguồn | F1: vệt đứt và ngắn đi | đạt | B động, ô Tím, F0 |
| B / Tím | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo; tương phản thấp trên nền nguồn | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Tím, F1 |
| B / Tím | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời; tương phản thấp trên nền nguồn | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Tím, F2 |
| B / Tím | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt; tương phản thấp trên nền nguồn | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Tím, F3 |
| B / Tím | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải; tương phản thấp trên nền nguồn | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Tím, F4 |
| B / Tím | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng; tương phản thấp trên nền nguồn | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Tím, F5 |
| B / Trắng | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải | F1: vệt đứt và ngắn đi | đạt | B động, ô Trắng, F0 |
| B / Trắng | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Trắng, F1 |
| B / Trắng | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Trắng, F2 |
| B / Trắng | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Trắng, F3 |
| B / Trắng | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Trắng, F4 |
| B / Trắng | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Trắng, F5 |
| B / Vàng | F0 / 0 ms | 100 ms | Vệt chéo dài, vài hạt ở đầu trên phải | F1: vệt đứt và ngắn đi | đạt | B động, ô Vàng, F0 |
| B / Vàng | F1 / 100 ms | 100 ms | Vệt gãy thành các đoạn dọc đường chéo | F2: các đoạn co thành mảnh nhỏ | đạt | B động, ô Vàng, F1 |
| B / Vàng | F2 / 200 ms | 100 ms | Còn vài mảnh ngắn và hạt rời | F3: mảnh nhỏ còn chấm rời | đạt | B động, ô Vàng, F2 |
| B / Vàng | F3 / 300 ms | 100 ms | Chỉ còn chấm thưa, gần tắt | F4: vệt hiện lại theo đúng thứ tự nguồn | đạt | B động, ô Vàng, F3 |
| B / Vàng | F4 / 400 ms | 80 ms | Đầu vệt đặc hiện lại ở phía trên phải | F5: vệt kéo dài từ đầu trên xuống trái | đạt | B động, ô Vàng, F4 |
| B / Vàng | F5 / 480 ms | 100 ms | Vệt dài ra về dưới trái, đầu trên giữ hướng | F0: vệt dài hơn, hạt lại xuất hiện; giữ điểm nối nguồn | đạt | B động, ô Vàng, F5 |
