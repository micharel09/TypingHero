# TypingHero UI kit v2 — review

Mở [gallery](index.html), [contact sheet màn](contact-sheet-screens.png), [contact sheet asset](contact-sheet-assets.png) hoặc [ZIP](typinghero-ui-kit-v2.zip). Chỉ là asset và mockup review; chưa tích hợp Unity hoặc phát hành. V1 giữ nguyên.

116 PNG RGBA + nguồn SVG; 30 màn × 320×180, 640×360, 1280×720. Bản 2×/4× dùng nearest-neighbor chính xác. Sprite hero/skeleton/Captain crop frame đầu, giữ toàn bộ pixel nhìn thấy ở 1×; chỉ skeleton flip để đối đầu. Background final được dựng bằng khối pixel trên canvas 320×180; concept ImageGen gốc lưu riêng, không dùng downsample thay pixel source. Logo fantasy lettering riêng, text chức năng dùng VT323: body 10, phụ 8, title 12–14, từ gõ 16px. Chữ được raster ở native size, có alpha nét chữ; không raster lại ở bản lớn.

UI dùng mặt lõm, viền thép tím, cạnh sáng trên trái, vàng cũ và đinh tán góc. Boss dùng khung fixed 94×16, hai đầu chạm khắc, huy hiệu Captain tách riêng, fill đỏ ba lớp. Boss ở vùng đối thủ hiện có; Slayer giữ layout với badge/multiplier/viền vàng. Nhập sai dùng đỏ và copy `! MISS / TRY AGAIN`.

## Mapping và độ phủ

- Layout/action reuse từ v1 và các owner nêu trong README v1: menu có tên, Normal selector, Play, How to Play, Tutorial, Collection, Music/SFX; không đổi nghiệp vụ.
- `Assets/Scripts/UI/PlayerIdentity.cs`: tên 3–10. Có mockup tên rộng WWWWWWWWWW, tên lỗi và tên thường.
- `Assets/Editor/PixelUI/EndlessAssets.cs`: Captain dùng sword-shield. HUD có HP/combo bên trái, HP/stamina bên phải, score/combo giữa, typing dưới. Thanh boss thay HP đối thủ.
- Pause Resume/Restart/Main Menu; kết quả score/time/accuracy/parries/cleared; Continue giữ đối thủ kế và HP; How to Play giữ Continue.
- Sáu bài tutorial Attack/Accuracy/Parry/Stamina/Slayer/Combine, retry, passed và complete.
- Bốn Collection Endless/Classic/Opponents/Achievements; chỉ Endless có difficulty và round/kills; Classic chỉ Score. Loading/empty/error, opponent locked, achievement locked/unlocked.
- Controls: primary/secondary/input/tab, slider/scrollbar, checkbox/toggle, icons/keycaps; normal/hover/pressed/focus/disabled, selected/error/on/off theo loại. Primitive v1 được snapshot tại `source/basis.json`, recolor và chuyển về native size; frame/logo/boss/background là nguồn v2 mới.

Mockup dùng dữ liệu minh họa. Gallery chỉ có tương tác review, không nối Play, typing, online hay gameplay. Các màn Collection giữ dữ liệu và action hiện có, không tạo API mới.

## Import và chỉnh sửa

[metadata.json](metadata.json) chứa palette, rect atlas, pivot, border. Rect gốc trên trái; chuyển Y khi importer dùng gốc dưới trái. Pivot `[0.5,0.5]`; border Unity **left,bottom,right,top**. Point/Clamp, mipmaps off; native unit 1px. Atlas RGBA 1024² có extrusion 2px, khoảng cách 6px.

Frame border 6px giữ đinh tán và góc; phần trung tâm chỉ kéo mặt/viền thẳng. Không stretch icon, logo, badge hoặc boss track. Boss fill crop liên tục 68×3 tại offset `(13,7)` so với track, sau đó chồng `boss-ticks`; vạch chỉ trang trí, không chia nấc HP. Resource fill và track tách. Không dùng các PNG screen đã ghép chữ làm asset gameplay.

Nguồn chỉnh sửa chính: [build.py](source/build.py), [dungeon.svg](source/dungeon.svg), SVG từng asset. Rebuild bằng `python design/ui-kit/typinghero/v2/source/build.py` (Pillow). Gói tự chứa font, snapshot primitive và crop sprite; ngoài repo script dùng các crop đã bàn giao. Sửa SVG riêng không tự cập nhật Python; sửa source rồi rebuild để đồng bộ atlas/metadata. `--pack-only` đóng gói lại tài liệu mới.

## Intro

GIF 2400ms; gallery tự chạy: nền 0–400ms, sprite/foreground 400–850ms, logo 850–1500ms, menu crossfade 1650–2200ms, giữ đến 2400ms rồi ẩn Bỏ qua. Nút Bỏ qua hoặc Esc hoàn tất ngay; Chạy lại hủy lượt cũ. Không có bước Press Enter. GIF là mô phỏng raster; HTML có motion reveal, không phải animation Unity.

Nguồn tham khảo và giới hạn xem [CREDITS](CREDITS.md); kết quả thực chạy xem [QA](QA.md). Chưa kiểm chứng importer, binding, input/focus Unity, gameplay, hiệu năng hoặc online. Gói giữ tại v2 để review; không có worktree hay service mới.
