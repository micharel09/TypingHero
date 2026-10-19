# HUD r7 — contract review

Phạm vi: review HTML tương tác; không đổi Unity/public API, không xuất atlas. R4–r6 giữ nguyên. Trang cần các revision nguồn trong cùng cây thư mục.

- Khung198×30 tại(61,146), lòng chữ190px, VT32316, chữ tại y151 như r6. Ba ô58px, gap8, trái65/131/197, tâm94/160/226. `left = cellLeft + round((58 - rasterWidth)/2)`; tâm layout giữ nguyên khi gõ, toàn từ chỉ đổi màu.
- Current/next/third giữ ivory/jade/error, muted và metal của r4/r6. Không thêm ô nền/divider/highlight. Slayer giữ cảnh, hai dải đen, Parry/timer và không bảng riêng.
- Handler queue/input từ r4: chữ không phân biệt hoa/thường; bỏ nonletter; sai reset progress, giữ queue, đỏ180ms; lỗi tiếp khởi động lại, đúng xóa lỗi ngay. Hoàn tất cập nhật queue đồng bộ, không khóa input.
- R7 trượt trái66px/100ms ease-out cubic; frame đầu queue mới offset+66, về0. Burst hoàn tất nhiều từ paint queue cuối không trượt. Hoàn tất tiếp trong animation bắt đầu chuyển queue mới ngay. System OR checkbox reduced motion chuyển ngay. R6 đối chứng dùng quãng trượt `wordWidth+8` của r4.
- Raster rộng hơn58px clip vào ô, giữ font/kích thước; trang ghi “stress ngoài pool”. Quy tắc chỉ thuộc review, không áp dụng Unity.

Lý do: thử điểm đọc cố định để giữ nhịp gõ, tìm current/đọc trước hai từ và trở lại combat. Pool219 có max54px; ba từ max54 cho gap tối thiểu12px. Từ ngắn tạo khoảng cách lớn hơn, có thể làm queue rời rạc: cần người chơi đánh giá. Không suy ra tọa độ tối ưu hoặc hiệu quả WPM từ nguyên tắc proximity/readability.
