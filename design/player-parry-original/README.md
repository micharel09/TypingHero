# Parry bằng sprite gốc

Integration note: this copy targets latest main `fd3f21d`; source contract protects
unchanged main assets. New integration evidence: [checkpoint](../../docs/plans/parry-main-demo.md).
Earlier runtime receipts below apply to the previous parent player.

[Preview](preview.html) dùng trực tiếp PNG gốc, giữ pixel và tỉ lệ; không tạo sheet,
native hoặc normal thay thế. Layout/CSS và cách hủy timer kế thừa preview parry trước.
Chọn clip, 1×/0,5×, hướng, zoom; phát một lần giữ idle, phát lặp nghỉ 450 ms ở idle.
Scrub theo thời gian hoặc chuyển từng cel, kể cả idle cuối chuỗi.

| Clip | Sprite và mốc bắt đầu, ms | Về idle |
| --- | --- | --- |
| Parry | 2: 0; 3: 250; 4: 316⅔; 5: 383⅓ | 400 ms |
| Đỡ trúng | 2: 0; 3: 800; 4: 866⅔; 5: 933⅓ | 950 ms |

Hai clip vào thẳng `male_hero-combo_3_end_2`; không đi qua 0–1. Các khoảng hồi
67/66/17 ms trong brief là số làm tròn của 4/4/1 tick tại 60 FPS. Cửa sổ đỡ
vẫn mở ở 0 và đóng ở 250 ms, có grace hiện có. Các mốc clip là thời gian animation;
pause/hitstop hiện có giữ timeline. Success không tự mở hitbox hoặc phát damage.

Hoàn tất một từ trong **[0, 800) ms** từ success sẽ giữ tối đa một follow-up, phát
bằng attack hiện có sau khi success kết thúc ở 950 ms. 799 ms được nhận; đúng
800 ms bị loại. Từ trước success không chuyển thành follow-up mới. Pause giữ
thời hạn và hàng đợi; hit/death/Slayer/disable hoặc chuyển state sớm hủy hàng đợi.
VFX va chạm vẫn phát từ `OnParrySuccess` qua hệ thống gameplay hiện có.
Follow-up được nhận giữ quyền ngắt đòn của đúng mục tiêu qua thời gian hồi;
quyền này tiêu thụ ở lần mở hitbox đầu tiên, không tăng cửa sổ hoàn tất từ 800 ms.

Sprite source GUID `ab0383fb37a41114dba343cfa0c6e919`, PPU16, pivot (0.49, 0.51),
normal GUID `3a38644ea0de6fb619aeec1b7705cff4`. [Bảng contact](contacts.json) chỉ
trỏ rectangle/fileID gốc. [Source contract](source-contract.json) giữ SHA-256
nguồn/import và GUID của cả ba clip. Controller giữ state/clip GUID; clip success
đang dùng nằm ở `Assets/Art/male_hero/individual_sheets`, bản cùng tên trong
`Assets/Characters/Player` được đồng bộ để tránh pose rỗng. Editor menu
`CLACKBLADE/Apply Original Player Parry` và builder chỉ dùng sprite gốc.

Kiểm dữ liệu và timer:

```text
python -B design/player-parry-original/verify.py
node design/player-parry-original/test_preview.cjs
```

Tình trạng nghiệm thu, phiên bản Unity, input hash và giới hạn quan sát được ghi
riêng trong [checkpoint](../../docs/plans/hornet-parry-pose.md). Kết quả kiểm bản
tự vẽ trước đây không được tái sử dụng để nghiệm thu bản này.

Kiểm bản hiện hành: 56 hash nguồn/import/GUID, timer preview, Edge preview 30 kiểm,
Unity 24/24, builder lặp và WebGL input thật 42 kiểm đều đạt. Timeline là thời gian
animation; hitstop 50 ms hiện có và pause kéo dài thời gian thực. Cảm nhận chuyển
động liên tục chưa được nghiệm thu.
