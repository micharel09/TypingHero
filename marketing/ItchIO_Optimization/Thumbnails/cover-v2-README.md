# Cover v2 — gameplay điện ảnh

Ngày: 2026-10-19. Chỉ thay cover; đây là bố cục quảng bá từ asset thật,
không phải screenshot gameplay nguyên trạng. Không dùng ảnh AI.

## Nguồn và bố cục đã chốt

- [Type Knight](https://chaikadev.itch.io/type-knight): không khí pixel art;
  [TYPECAST](https://gutter-arcade.itch.io/typecast): tỷ lệ nhân vật/logo;
  [TYPE:CAST](https://not-jam.itch.io/typecast): ít chi tiết, điểm nhấn rõ.
  Không đưa artwork của reference vào sản phẩm.
- Đối chiếu `../Screenshots/01-type-to-attack.png` và provenance trong
  `../Screenshots/README.md`: hero khăn tím, skeleton trắng và nền Nhật Bản.
- Hero: `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, cel đầu
  128×128, alpha bounds `(54,42)-(112,80)`, phóng 7 lần.
- Skeleton: `Assets/Art/Skeletons_Free_Pack/Skeleton_Sword/Skeleton_White/`
  `Skeleton_With_VFX/Skeleton_01_White_Attack1.png`, cel đầu 96×64, alpha bounds
  `(34,18)-(68,64)`, phóng 6 lần, mirror ngang để đối mặt hero.
- Giữ nguyên pose, vũ khí, màu và tỷ lệ của từng cel; cung kiếm nằm sẵn trong
  sprite hero. Không vẽ lại grip, không ghép vũ khí hay tự thêm VFX.
- Nền: Sky, Mountain_Back, Mountain_Middle, Shrine_Multiple, Trees trong
  `Assets/Art/Environment/TinyPixelJapan/`; scale 3 lần, crop khung và chuyển tông
  xanh đen/giảm sáng riêng nền. Font VT323 có sẵn trong project.
- `cover-v2.html` là nguồn HTML/SVG chỉnh sửa được; mọi đường dẫn tương đối
  tới asset gốc. Cảnh tràn khung, nhân vật ở hai phần ba dưới, logo ở trên.

## Xuất và kiểm tra

Render HTML trong Chromium/Edge ở viewport 630×500, deviceScaleFactor=1;
chờ `document.fonts.ready`, chụp đúng khung 630×500. Không screenshot ở DPR khác
rồi thu nhỏ. Không cài dependency. Phiên này dùng Playwright đã có trong runtime
Codex và Edge headless. Hai bản nhỏ xuất từ PNG chính bằng Pillow đã có:

```python
from PIL import Image
from pathlib import Path
p = Path('Marketing/ItchIO_Optimization/Thumbnails')
im = Image.open(p / 'cover-v2-630x500.png')
for size in [(315, 250), (158, 125)]:
    im.resize(size, Image.Resampling.NEAREST).save(
        p / ('cover-v2-%dx%d.png' % size))
```

Đã xem cả ba kích thước: nhận ra hai nhân vật đấu kiếm, đọc được TYPING HERO,
khăn tím và lưỡi kiếm tách khỏi nền; không HUD, tutorial, phím, footer hoặc viền.
Nearest-neighbor giữ cạnh pixel; các sprite dùng scale nguyên, không kéo méo.
Đã xem cạnh hình nhận diện từ ba trang reference trong cùng vùng 315×250,
giữ tỷ lệ ảnh reference (contain); đây không phải phép đo hiệu quả thumbnail.
Font load thành công; DOM chỉ có TYPING HERO và TYPE TO SLASH.
Không chạy Unity tests vì không thay file game. Không có dữ liệu CTR mới.

## Khôi phục

Cover trước task là `cover-a-630x500.png`, image ID **31068886**; giữ nguyên
HTML/PNG của A và B. Để rollback, vào editor game **3857509** của **micharel09**,
chọn **Replace Cover Image**, upload `cover-a-630x500.png`, **Save**, rồi kiểm tra
card tìm kiếm. Không dùng `StorePage/before-cover.png`: đó là mốc cũ hơn cover A.

Trạng thái lưu và kiểm tra public được ghi trong `docs/plans/itch-discovery.md`.
