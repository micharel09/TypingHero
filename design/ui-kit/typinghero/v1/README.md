# TypingHero UI kit v1 — gói review

Ngày: 19/10/2026. Phạm vi: thiết kế asset và mockup; chưa tích hợp Unity, chưa phát hành itch.io.

Mở [index.html](index.html) trực tiếp bằng Chrome/Edge. Có tìm kiếm, lọc nhóm, xem kích thước/pivot/9-slice, tải PNG/SVG và intro có nút bỏ qua. Tải toàn bộ qua [ZIP](typinghero-ui-kit-v1.zip). Xem [contact sheet asset](contact-sheet-assets.png) và [contact sheet màn hình](contact-sheet-screens.png).

## Đã bàn giao

- 113 asset PNG RGBA và 113 nguồn SVG: logo chính, logo gọn, đơn sắc, game icon; controls; frames; icons/keycaps; HUD.
- 30 mockup ở cả 640×360 và 1280×720. Bản lớn là nearest-neighbor 2×, không nội suy mờ.
- Artwork ImageGen gốc và bản chuẩn hóa pixel: backdrop, nhân vật lấy từ sprite hiện có, foreground alpha; có phương án foreground vector riêng.
- Sprite atlas 1024×1024, metadata rect/pivot/border/filter; font VT323 và OFL.
- Intro không tiếng 2,4 giây: GIF, MP4 và AVI; HTML mô phỏng reveal và bỏ qua.
- Nguồn Python tái tạo kit, nguồn kiểm chứng artifact và gallery. Không ghi vào `Assets`, `Packages` hay `ProjectSettings`.

## Asset hoàn chỉnh và mockup

`png/`, `svg/`, `sheets/` và ba lớp `artwork/01-backdrop.png`, `02-character.png`, `03-foreground.png` là đầu ra asset thiết kế v1. Artwork gốc giữ trong `artwork/*-original.png` để truy nguồn. Foreground SVG là **phương án khác** được vẽ bằng pixel/vector, không phải vector hóa artwork ImageGen.

`screens/`, hai contact sheet và `intro/` là bản trình bày thiết kế. Tên, từ gõ, điểm, HP, stamina và thống kê chỉ được ghép trong mockup. Không có chữ động trong background, frame hoặc fill asset. Keycap và logo chứa ký tự tĩnh đúng chức năng.

Gallery chỉ tương tác để review asset và intro. Play, navigation game, typing và leaderboard trong ảnh chưa nối gameplay. Không suy ra kiểm chứng Unity, gameplay, performance, online hay importer từ gói này.

## Hướng hình thức và nguồn

Plan user bàn giao là nguồn quyết định: pixel fantasy, than đen/trắng ngà/đỏ son/ngọc trầm; title có chiều sâu; menu luôn hiện tên và Play; HUD hai bên phía trên, score/combo giữa, vùng gõ phía dưới. Không cần duyệt lại trước khi tạo toàn bộ v1; v1 này chờ review chung.

Tham khảo chính: [Shogun Showdown](https://game.udn.com/game/story/122088/8224224), đã mở 19/10/2026, dùng hierarchy tương phản mạnh và silhouette nhiều lớp. Tham khảo phụ: [Type Knight](https://chaikadev.itch.io/type-knight), đã mở cùng ngày, dùng ý tưởng lettering có cá tính và thông tin ngắn, rõ. Không sao chép asset hoặc mã của hai game. Đây là sáng tác theo hướng đã chốt, không phải tái dựng website.

Màu nền/text giữ cơ sở từ `Assets/Editor/PixelUI/ShogunTheme.cs`: jade `#528D83`, paper `#F2E7CF`, vermilion `#C65140`. Coal được làm tối thành `#111B20` theo brief; warning dùng `#FF685C` + icon/copy; Slayer dùng `#DFB965` + frame/multiplier. Đỏ lỗi không đồng nhất với vàng Slayer. [metadata.json](metadata.json) lưu palette và giá trị dùng thực tế.

Logo vẽ từ ma trận pixel góc cạnh riêng, không dùng VT323 thay logo. Chữ chức năng dùng đúng VT323 đang dùng trong source. Tất cả hình học control có cùng góc bậc 3px, viền 2px, shadow chân 3px, phần nhấn ngắn ở góc trên.

## Mapping source → kit

Các đường dẫn dưới tính từ root repo; đọc trên dirty checkout hiện hành, không thay đổi source.

| Source | Consumer trong kit / adaptation |
| --- | --- |
| `Assets/Editor/PixelUI/PixelUIAssets.cs`, `PixelUIFactory.cs` | Font VT323 và text layer riêng; các primitive được vẽ lại theo brief, không claim reuse prefab Unity |
| `Assets/Editor/PixelUI/ShogunTheme.cs`, `EndlessDifficultyBuild.cs`, `TutorialBuild.cs` | Menu giữ ô tên, Play, selector Normal, How to Play, Tutorial, Collection, Music và SFX; title/art được bố trí lại theo plan |
| `Assets/Scripts/UI/PlayerIdentity.cs` | min 3/max 10; thử tên rộng `WWWWWWWWWW`; tên trống có helper/error riêng |
| `Assets/Editor/PixelUI/PixelUIScenes.cs`, `CoreMatchBuild.cs` | Pause: Resume/Restart/Main Menu; kết quả giữ thống kê và bỏ Resume; How to Play có Continue |
| `Assets/Scripts/GameController/EndlessDuel.cs` | Round cleared có difficulty/cleared/HP, đối thủ tiếp theo, Continue; không thêm cơ chế |
| `Assets/Scripts/GameController/TutorialDirector.cs` | Sáu bài Attack/Accuracy/Parry/Stamina/Slayer/Combine, Practice/Next/Start Battle và retry; tái dùng cùng modal/HUD asset |
| `Assets/Editor/PixelUI/EndlessCollectionBuild.cs`, `Assets/configFireBase/LeaderboardUIBinder.cs` | Đủ Endless/Classic/Opponents/Achievements; độ khó chỉ ở Endless; cột Classic chỉ Score; loading/empty/error dùng cùng shell |
| `Assets/Editor/PixelUI/EndlessAssets.cs` | Sáu thẻ đối thủ Swordsman/Double Strike/Heavy/Berserker/Guard/Captain; locked chưa gặp; không vẽ lại sprite chiến đấu |
| `Assets/Scripts/GameController/CombatSession.cs` | Kết quả gồm score/time/accuracy/parries và cleared; không phát sinh số liệu gameplay mới |

Các thẻ Opponents/Achievements là đề xuất trình bày nội dung hiện có. Source hiện dùng text detail, kit tách thành card theo phạm vi plan; chưa thay đổi contract data hay render Unity. Chi tiết hint đối thủ cần bind từ definition hiện có khi tích hợp, không lấy text mẫu từ ảnh làm nguồn nghiệp vụ.

## Thành phần và trạng thái

| Nhóm | Trạng thái / file |
| --- | --- |
| Button primary/secondary | normal, hover, pressed, focus, disabled |
| Input | normal, hover, pressed, focus, disabled, error |
| Tab | normal, hover, pressed, focus, disabled, selected |
| Slider / scrollbar | normal, hover, pressed, focus, disabled |
| Checkbox / toggle | on/off × normal, hover, pressed, focus, disabled |
| Leaderboard row / opponent / achievement | normal, selected, disabled/locked |
| Panel / modal / tooltip | normal; label/content đặt riêng |
| Resource | track/fill riêng HP, stamina, combo, timer, Slayer |
| Typing | normal/correct/error frame; Slayer và cue frame riêng |
| Icon | HP, stamina, score, combo, audio on/off, music, pause, restart, Continue, close/back, warning, check, Slayer, trophy, lock, sword/shield/axe, timer, chevrons |
| Keycap | Enter, Esc, Space, R, M, A, Backspace |

Focus dùng biên ivory và dấu gạch chân; pressed đổi nền/insert; disabled giảm sắc. Correct dùng ivory/jade + copy, error coral + copy; không dùng màu làm tín hiệu duy nhất. Không thêm animation gameplay hoặc cơ chế input mới.

## Quy ước bàn giao

- Canvas logic 640×360; hiển thị 1280×720 bằng scale nguyên 2×. Không scale lẻ nếu cần giữ pixel đều.
- PNG straight RGBA, RGB của pixel alpha 0 được làm sạch. Shape/UI alpha nhị phân; artwork gốc AI giữ nguyên để truy nguồn.
- `metadata.json`: kích thước px, pivot normalized `[0.5,0.5]`, border thứ tự Unity **left,bottom,right,top**; rect atlas dùng gốc **trên trái**. Khi tích hợp phải chuyển Y sang convention importer Unity nếu cần.
- Nine-slice 6px ở frame/button/input/tab/card; không scale méo corner. Bar track dùng 3px; fill crop theo tỷ lệ, không nine-slice. Icon/keycap/logo giữ tỷ lệ và Point filter.
- Atlas có gap và extrusion 2px; không mipmap, Clamp. Pivot foreground `[0.5,0.5]`, layer full canvas đặt chồng cùng gốc. Nhân vật trong layer là sprite có sẵn, không phải sprite mới để thay animation.
- Chỉ ba lớp số 01/02/03 tạo composition intro; logo PNG đặt riêng ở `(29,24)`. SVG foreground alternative không được chồng thêm vào cùng composition mặc định.

## Timing intro (mô phỏng thiết kế)

| Khoảng | Reveal |
| --- | --- |
| 0–0,40s | Backdrop fade từ coal |
| 0,40–0,85s | Character + foreground; HTML translate 8px theo 8 nấc |
| 0,85–1,50s | Logo reveal; HTML opacity theo 10 nấc |
| 1,50–1,65s | Giữ logo |
| 1,65–2,20s | Crossfade sang menu với ô tên/Play |
| 2,20–2,40s | Giữ menu; tự kết thúc |

Nút Bỏ qua hoặc Esc hoàn tất animation đang chạy; replay hủy animation cũ rồi bắt đầu lại. Không có bước “Press Enter” sau intro. GIF/AVI/MP4 dùng 60 frame × 40ms = 2,4s; video là mô phỏng raster, HTML bổ sung chuyển động nấc. Không có âm thanh. Fade/video encode có màu trung gian; các PNG asset giữ biên pixel gốc.

## Kiểm chứng và giới hạn

Xem [QA.md](QA.md) cho kết quả thực chạy. Acceptance chỉ ở chất lượng và tính đầy đủ của bộ thiết kế. Chưa kiểm chứng binding, 9-slice importer, keyboard focus Unity, localization Unicode ngoài glyph VT323 hiện có hoặc gameplay. Không chạy suite gameplay vì không thay source game.

Tái tạo: `python design/ui-kit/typinghero/v1/source/build.py` từ repo. Cần Pillow; giữ hai artwork AI gốc. MP4 dùng ffmpeg có sẵn; không có ffmpeg thì vẫn xuất GIF/AVI. Kiểm artifact: `python design/ui-kit/typinghero/v1/source/verify.py`. Nguồn SVG được sinh cùng hình học PNG, sửa `source/build.py` để tái tạo đồng bộ. Chỉnh riêng SVG sẽ không tự cập nhật PNG.

Gói được giữ tại đúng thư mục này phục vụ review; không tạo child worktree, service, terminal nền lâu dài hay artifact tạm cần giữ.
