# Skeleton full pack

Mở [review.html](review.html) để chọn vũ khí, màu, animation, VFX, mirror,
onion và overlay; xem 1280×720, 640×360 hoặc nearest 4×/8×. Idle/Walk lặp,
các clip còn lại phát một lượt rồi giữ cuối. Replay, pause/resume, scrub và bước
cel dùng số cel/duration thật, không dùng chung timeline giữa các animation.

- 72 PNG: `sword|axe|spear/White|Yellow/Without_VFX|With_VFX/Idle|Walk|Attack1|Attack2|Hurt|Die.png`.
- 6 native LibreSprite: `sword|axe|spear-White|Yellow.aseprite`, mỗi file có
  hai lớp `Body + weapon`/`VFX` và sáu tag animation liên tiếp.
- Canvas mỗi cel 96×64, alpha 0/255. Idle rìu9, kiếm/thương8; Walk10, Hurt5, Die13;
  Attack1 rìu16, kiếm/thương10; Attack2 đều9. Mỗi native rìu62 cel;
  tổng344 cel trong sáu native. Idle rìu9×100ms =900ms.
- Walk100ms/cel; Attack2 duration83/84ms tương ứng12fps. Idle/Hurt/Die và
  phần đầu Attack1 theo timeline nguồn. Attack1 rìu giữ contact trong khoảng1s,
  smear chỉ cel4, đầu rìu ghim cel5–10; recovery100/120/140/140/120/100ms.
  Đây là timing thiết kế, chưa đổi contract combat.

Nguồn quan sát là [pack MonoPixelArt](https://monopixelart.itch.io/skeletons-pack)
và hai GIF lưu nguyên tại [references](../references/). Kiếm dùng pose gốc:
composite White của Idle/Attack1/Hurt/Die khớp pixel bộ đã duyệt; Walk/Attack2
giữ pose/alpha nguồn và áp palette cùng bộ. Idle White rìu trích đủ9 cel từ GIF
overview: khôi phục lưới4× đồng nhất, loại nền và hai skeleton bên cạnh,
giữ nguyên RGB/thứ tự/cỡ pixel, đặt chân tại y63. Một tay giữ cán, tay phụ thả.
Preview đối chiếu bản xuất/GIF/overlay silhouette cùng cel và cùng tỷ lệ.
Yellow đổi riêng vật liệu xương; highlight thép dùng chung RGB với xương được
phân loại theo vùng vật liệu. Cán nguồn khoảng49px thay cán32px cũ; head/socket
và cán các pose khác lấy từ raster nguồn, inverse nearest theo trục vũ khí.
GIF công khai không có trọn các
clip rìu và không có skeleton thương; những phần đó được dựng lại theo plan,
không phải bản trích từng pixel từ Premium. Giữ thương theo yêu cầu ban đầu.

VFX kiếm được tách từ chênh lệch sheet nguồn có/không VFX. Cel5 Attack1 nguồn
có một pixel xương (39,36) bị VFX thay bằng alpha0; lớp Body giữ envelope
transparent này để composite native khớp nguyên bản đã duyệt. Vì vậy Body
không được gọi là bản Without_VFX nguồn khớp từng pixel. White/Yellow giữ
cùng alpha/pose; chỉ palette xương thay đổi, vật liệu vũ khí giữ nguyên.

Nguồn chỉnh sửa là [axe_reference.py](../axe_reference.py),
[full_pack_poses.py](../full_pack_poses.py),
[full-pack-pixel.js](../full-pack-pixel.js),
[prepare_full_pack.py](../prepare_full_pack.py) và
[preview template](../full-pack-preview.template.html). Primitive pixel/native
được dùng lại từ exporter/renderer hiện có; việc vẽ cel chạy trong LibreSprite.

```powershell
python -B design/skeleton-hero/prepare_full_pack.py --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe
# Chỉ xuất rìu, giữ nguyên byte PNG/native kiếm và thương:
python -B design/skeleton-hero/prepare_full_pack.py --weapon axe --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe
node design/skeleton-hero/verify_full_pack.cjs
```

Browser check dùng Playwright đã có qua `NODE_PATH` và Edge. Exporter kiểm
roundtrip từng lớp/composite, tag, duration, alpha, grip, contact, chân và hash
nguồn; [manifest.json](manifest.json) lưu pose/timing/provenance,
[verification.json](verification.json) lưu kết quả kỹ thuật. Kiểm browser
chứng minh controls/playback, không thay đánh giá cảm giác lực. Đây là asset
thiết kế và preview, chưa tích hợp Assets hoặc kiểm Unity/WebGL.
