# HUD r7 — spacing cân, điểm đọc ổn định

Kiểm ngày19/10/2026: Python PASS pool219/max54/cell58/gap tối thiểu12; baseline normal khớp byte r6, ngoài typing Slayer khớp r6. Chrome/Playwright PASS tốc độ150/100/60ms, burst, queue mới trong animation, slide đầu66/về0, lỗi liên tiếp180ms/đúng xóa lỗi, reset, stable positions, toàn pool và các fixture, reduced motion system/checkbox;12 tổ hợp normal/Slayer ×1/2/4 ×màu/grayscale, pixel isolation ngoài typing, replay/đảo thứ tự; không page/request error. R6 đối chứng là renderer tương tác dùng raster r4 và vật liệu r6; Slayer tĩnh r6 từng paste alpha hai lần, review tương tác dùng raster trực tiếp một lần. Không claim byte-match glyph alpha Slayer với ảnh tĩnh.

Đã xem normal native/so sánh2×, longest triple native, Slayer grayscale2× và màu4×: không cắt/chồng từ trong pool, không lấn Parry/timer, Slayer không có bảng riêng. Capture QA tạm đã dọn sau khi ghi kết quả. `source/inspect.cjs` tái tạo các góc nhìn này. Cảm giác spacing/nhịp chuyển và hiệu quả đối với người chơi chưa kiểm chứng.

Mở [review r6/r7](index.html), xem [contract](contract.md). Chỉ bàn giao bản review để đánh giá cảm giác gõ; không tích hợp Unity. Reuse r6 `source/build.py::render/frame` cho vật liệu/cảnh/actor/Slayer và r4 PNG raster, pool/widths, queue/input/reset/easing. Handler chung cấp cùng state cho cả hai canvas; replay và đảo thứ tự không tạo hai queue lệch nhau.

R7 dùng ô58/gap8, tâm94/160/226, raster nguyên pixel. Giữ frame198×30 tại61,146, chữ y151/font16 và phân cấp màu. Tiến độ không đổi điểm căn từ. Quãng trượt cố định66 thay cho `wordWidth+8`; giữ100ms cubic. Không thêm visual HUD. Ba từ dài nhất trong pool là different54, question48, remember48 (các từ đồng hạng48 khác cũng vừa ô).

Controls: normal/Slayer, đúng/sai, reset/replay, pool219, longest triple, short, mixed và stress ngoài pool; scale1×/2×/4× nearest-neighbor, grayscale, reduced motion checkbox/system; đảo thứ tự r6/r7. Gõ cùng chuỗi ở hai thứ tự, ghi nhận tìm current, đọc nhầm preview, cảm giác khoảng cách và nhịp chuyển. Việc đánh giá với người chơi chưa thực hiện; chưa có dữ liệu WPM hoặc phân tâm.

Nguồn định hướng do plan user cung cấp: [NN/g proximity](https://www.nngroup.com/articles/gestalt-proximity/) và [Xbox XAG101](https://learn.microsoft.com/en-gb/gaming/accessibility/xbox-accessibility-guidelines/101). Đây là cơ sở giả thuyết, không phải nghiên cứu người chơi TypingHero hay bằng chứng cho geometry tối ưu. Đánh đổi: khoảng cách chữ ngắn lớn hơn; stress ngoài pool clip trong ô review.

Chạy lại từ repo root (Python/Pillow, Chrome và Playwright đã có):

```powershell
python design/ui-kit/typinghero/v2/revisions/hud-r7/source/build.py
New-Item -ItemType Directory -Force E:/CodexTemp/TypingHero/hud-r7 | Out-Null
node design/ui-kit/typinghero/v2/revisions/hud-r7/source/verify-browser.cjs
```

Verifier mở file URL; Chrome test cho phép đọc file để so pixel canvas (mặc định browser taint canvas file URL nhưng vẫn hiển thị/gõ được). Browser tự đóng. Capture tạm phải dọn sau khi xem và ghi kết quả. Source revision r7 không sửa các nguồn r4/r5/r6 và không chạy Unity suite vì không đổi game.
