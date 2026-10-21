# Player: tóc đen, khăn trắng

Đủ 25 sheet / 166 frame và atlas tổng `male_hero.png`. Mở `preview.html` trực tiếp để so sánh gốc/mới. Gói PNG nằm trong `player-dark-pack.zip`. Đã thay 26 PNG trong Unity; bản WebGL production đã kiểm tra, upload itch đang chờ Cloudflare sau lỗi403.

Màu theo idle đã duyệt: tóc đen than, khăn trắng ngà, áo/giày đen–xám; giữ nguyên sắc da và mắt. Màu highlight khăn `e74088` chỉ có ở một số sheet di chuyển được đổi sang `f5f1e5`, cùng thứ tự sáng tối với khăn idle. Ba biến thể viền tím rất gần nhau ở Combo 1/Dash chuyển sang các sắc xám tương ứng; một pixel viền xanh trong Ledge Hang chuyển xám đậm. Idle khớp RGBA với bản duyệt.

Nguồn: `Assets/Art/male_hero/`. Sheet 128 px/cel, atlas 2944×3200. Không sửa GUID, metadata, normal map, pivot, timing, pose hoặc alpha. Không dùng AI, không đổi logic gameplay.

## Tạo lại

Từ root repo, dùng Python và Pillow có sẵn:

```
python design/player-palette/build.py
python design/player-palette/preview.py
```

Build đọc mask L trong `masks/<sheet>/<frame>.png`: 0 giữ nguyên, 1 tóc, 2 khăn, 3 áo, 4 giày. Các màu nâu dùng chung giữa da/giày chỉ đổi ở mask giày. Màu trắng áo tách khỏi mắt, kiếm và vệt chém. `sources.json` khóa checksum sheet gốc; `palette.json` ghi bảng màu vật liệu.

`materials.py` hỗ trợ soạn mask ban đầu bằng nhóm màu và vùng liên thông. `overrides.json` cố định lựa chọn vùng giày cho toàn bộ frame còn vật liệu nhìn thấy ngoài idle; có vùng chữ nhật tách phần giày/da liền nhau ở Death co chân. `build.py --author-masks` tạo lại mask từ những quyết định đó; build thường chỉ đọc mask đã lưu. Sau mọi sửa mask phải rà lại hình và pixel.

Hai frame Hurt đỏ và ba frame Death chuyển trắng có màu flash được vẽ sẵn. Decode bảng màu vật liệu dưới flash, chỉ đổi pixel được chọn rồi áp mức ảnh hưởng gốc: 40%/70% màu nền cho Hurt và 77%/45%/16% cho Death. Giữ pixel da/mắt nguồn, kể cả khi đã tint. Từ frame Death 14 trở đi giữ nguyên toàn bộ silhouette trắng và dissolve VFX.

## Preview

- 9 clip có sprite curves được đọc từ Unity, gồm clip nối nhiều sheet và clip parry success dưới thư mục art. Clip parry success trống trong thư mục Player không tạo một timeline giả.
- 25 lựa chọn sheet dùng 8 fps để kiểm hình; đây không phải timing game. Nhãn trên preview phân biệt rõ hai chế độ.
- Gốc/mới dùng chung timeline; tốc độ 1×/0.25×, lặp tùy chọn, kéo frame, bước frame, lật hướng, zoom nguyên nearest-neighbor và nền dungeon/tối/caro.
- Nền dungeon là artwork của project, chưa kiểm ánh sáng runtime Unity.

## Trạng thái kiểm chứng

Đã rà hình đủ 166 frame, gồm điểm nối idle/run/walk, các pha combo và chuyển Hurt/Death. Kiểm độc lập PNG: 48.615 pixel đổi màu, 2.671.129 pixel còn lại trong 25 sheet giữ nguyên RGBA; alpha toàn bộ không đổi, da/mắt và màu riêng của kiếm giữ nguyên. Death 14–23 giữ nguyên toàn frame. Atlas khớp đủ 166 ô với sheet; ô rỗng giữ nguyên. Các cel trùng nguồn phải có kết quả giống nhau. ZIP khớp từng file sản phẩm và kiểm CRC qua.

Browser Orca local: 50 ảnh decode, 34 lựa chọn (9 clip + 25 sheet), 236 lượt frame; kiểm biên timing, kéo/đổi frame, bước vòng, dừng cuối Death và tùy chọn lặp. Đồng hồ xác định 200 ms tiến 200 ms ở 1× và 50 ms ở 0.25×. Lật canvas khớp phản chiếu từng pixel, zoom 6× đúng kích thước và tắt smoothing; ba nền hoạt động. Đã xem screenshot thực trên dungeon. Không đo FPS/performance, không build hay kiểm ánh sáng trong Unity. Không thay file dưới `Assets`.

Chi tiết bàn giao/worktree tại `docs/plans/player-palette.md`. Bộ asset đã tích hợp vào game. Trạng thái phát hành và bước Cloudflare còn chờ được ghi ở checkpoint.


Release status 2026-10-20: full player palette is integrated and published at https://micharel09.itch.io/clackblade (upload19891315). All29 non-HTML CDN hashes match the verified production ZIP; public menu rendered. Earlier pending/Cloudflare status is superseded.
