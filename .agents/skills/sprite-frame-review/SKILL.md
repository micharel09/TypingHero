---
name: sprite-frame-review
description: >-
  Tạo, sửa hoặc review mọi animation sprite trong TypingHero bằng kiểm trực quan
  từng frame và từng chuyển tiếp theo yêu cầu và reference ban đầu. Áp dụng cho
  idle, di chuyển, tấn công, trúng đòn và chết; không áp dụng UI motion hoặc chỉ
  chỉnh màu sprite tĩnh.
---

# Review animation sprite từng frame

Skill này dành riêng cho repo TypingHero. Nếu thay đổi gameplay hoặc hit timing,
áp dụng thêm [typinghero-game-loop](../typinghero-game-loop/SKILL.md).
Nghiên cứu video reference theo mục Game reference research trong `AGENTS.md`;
việc đó không thay thế review đầy đủ sprite đầu ra theo các bước dưới đây.

## Chốt nguồn đối chiếu

- Đọc yêu cầu, reference ban đầu và các quyết định đã chốt của task. Ghi đường
  dẫn/reference cụ thể, clip, phiên bản asset (hash hoặc revision xác định), số
  frame, thứ tự, duration, canvas, pivot/anchor, contact nếu có, loop hay trạng
  thái kết thúc và điểm nối sang trạng thái liên quan.
- Mở reference trực tiếp. Khi nguồn có frame, đối chiếu theo frame/thời điểm;
  khi khác số frame hoặc chỉ có key pose, ghi ánh xạ theo pha và giới hạn so sánh.
  Không tự đổi reference, động tác, timing hoặc tiêu chí để hợp thức hóa kết quả.
- Thiếu hoặc không mở được reference cần thiết: tiếp tục phần kiểm độc lập,
  nhưng ghi phần đối chiếu đó là **chưa kiểm chứng**, không suy ra khớp nguồn.

## Kiểm đủ frame và chuyển tiếp

1. Trích hoặc mở toàn bộ frame theo đúng thứ tự và duration, đánh số rõ ràng.
   **Thực sự xem từng frame**, ở kích thước chơi và zoom nearest-neighbor.
   Contact sheet chỉ hỗ trợ định vị; phải mở đủ lớn để đọc chi tiết từng frame,
   chia nhóm hoặc mở riêng khi cần. Không lấy mẫu, bỏ frame giữa hoặc chỉ xem
   key pose. Frame giữ/lặp vẫn phải kiểm vị trí và duration trong chuỗi.
2. Với từng frame, đối chiếu yêu cầu/reference: silhouette, tỷ lệ, hình dạng,
   palette theo contract, chi tiết bị mất/thừa, clipping, anchor, chân chạm đất,
   lớp trước/sau và pose. Với vũ khí, kiểm tay nối thân, grip, cán, hướng lưỡi/mũi
   và điểm contact. Phân biệt smear/biến dạng chủ ý với lỗi theo reference.
3. Kiểm **mọi cặp liên tiếp** F0→F1, F1→F2…: continuity của thân, chi, vũ khí,
   trọng tâm, quỹ đạo, grip, chân trụ, anchor và nhịp duration. Pose riêng đẹp
   không chứng minh chuyển tiếp đúng. Kiểm cuối→đầu nếu clip loop; kiểm điểm nối
   trạng thái có trong phạm vi task, không ép animation không loop quay về đầu.
4. Xem thêm playback đúng tốc độ và chậm để kiểm nhịp, lực và độ đọc động tác.
   Với các hướng/mirror và biến thể VFX có trong đầu ra, kiểm đủ frame/chuyển
   tiếp của từng biến thể; xem cả khi tắt VFX để hiệu ứng không che lỗi sprite.
   Ghi giới hạn preview so với runtime; chỉ nghiệm thu runtime khi đã kiểm thực tế
   nếu đó là yêu cầu của task.

## Bằng chứng bắt buộc và kiểm lại

Lưu bằng chứng trong artifact review hoặc checkpoint hiện có của task; task dài
dùng `docs/plans/`. Ghi phiên bản asset đã xem và link ảnh/frame đánh số có thể
truy lại. Không tạo thêm hệ thống log song song.

Mỗi clip/biến thể phải có **một dòng cho mỗi frame**, gồm:

| Clip / biến thể | Frame | Duration | Reference frame / thời điểm / pha | Nhận xét frame | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|---|---|

Điền nhận xét cụ thể theo phần đã quan sát; không thay bảng bằng câu “đã xem toàn
bộ” hoặc chỉ ghi PASS chung. Dòng cuối ghi chuyển tiếp loop, điểm nối trạng thái
đã kiểm hoặc “không áp dụng” kèm lý do theo contract. Trạng thái dùng đạt, lỗi
hoặc chưa kiểm chứng; ghi rõ lỗi, vị trí và phần chưa xem được. Chỉ điền sau khi
đã xem, không suy diễn kết quả từ script hoặc frame khác.

Sau khi sửa asset, **kiểm lại toàn bộ frame và chuyển tiếp của clip bị sửa**,
các biến thể chịu ảnh hưởng và điểm nối liên quan. Cập nhật bằng chứng theo bản
cuối; kết quả của bản cũ không chứng minh bản mới đạt. Nếu sửa nguồn dùng chung,
kiểm mọi clip/biến thể bị ảnh hưởng.

## Điều kiện kết luận

- Chỉ báo đạt khi đã kiểm đủ frame, chuyển tiếp, playback và đối chiếu yêu cầu/
  reference trong phạm vi nghiệm thu, không còn lỗi hoặc phần chưa kiểm chứng.
  Báo số frame/cặp chuyển tiếp đã kiểm trên tổng cần kiểm cho từng clip/biến thể.
- Tách kết quả kỹ thuật asset (alpha, bounds, thứ tự, duration, export/import)
  khỏi chất lượng hình và chuyển động. Build, export hoặc script PASS không
  thay bằng chứng quan sát từng frame, cũng không chứng minh khớp reference.
- Thiếu frame, thiếu reference, không xem được chi tiết, còn lỗi hoặc user phản
  hồi chưa đạt: giữ trạng thái lỗi/chưa kiểm chứng tương ứng, nêu phần còn lại;
  không báo hoàn tất hoặc tự hạ acceptance.
