# HUD r1 — review riêng

## Bản hiện hành — khung gõ gọn và ba cấp đối thủ

Nguồn quyết định: plan user bàn giao ngày 19/10/2026. Mở [gallery có preview gõ thật](index.html), [bảy màn](contact-sheet.png) và [component thường/elite/boss, bạc/vàng](component-sheet.png). Phần Pinterest và kết quả cũ bên dưới lưu lịch sử của revision; mô tả bản hiện hành ở mục này thay thế các thông số cũ.

- Khung gõ 158×30 tại (81,146), chỉ hiện nguyên từ. Phần đã đúng màu ngọc; phần còn lại sáng. Không nhãn, caret, dòng nhập lặp hoặc NEXT. `hud-correct` minh họa ba ký tự đầu đúng; `hud-error` đánh dấu ký tự thứ tư sai và reset tiến độ.
- Preview dùng `source/preview.html`, ghép nền riêng không có khung gõ với frame và overlay từ ở native. Sai: ký tự mục tiêu tại vị trí sai và viền đỏ 180ms; cả khung và từ dịch ngang tối đa ±1px nguồn trong 120ms, không cộng dồn. Lỗi mới thay thời điểm bắt đầu; hết rung về (81,146). Giảm chuyển động theo hệ thống hoặc checkbox chỉ báo màu. Nút/ô nhập review nằm ngoài HUD. Đây là mô phỏng review, không nối Unity.
- Thanh HP giữ frame/track/fill chung. Thường dùng socket tròn và tim; elite có hai nấc cao 2px, hai chevron và tên `ELITE SWORDSMAN`; boss có ba đỉnh cao 3px, Captain trong socket. Không badge trong lòng thanh. Socket rank 21×24 có gốc vòng tròn tại y=3, đặt lệch y=-3 so với frame. Slayer giữ silhouette/huy hiệu và vàng/x16.
- Mapping: `source/build.py` sở hữu geometry/rank/word và xuất 26 PNG RGBA + SVG, atlas, metadata, bảy màn × ba kích thước; `source/preview.html` sở hữu input/RAF của gallery. Overlay từ `png/word-progress-*.png`, `png/word-wrong-*.png` và `artwork/preview-background.png` chỉ phục vụ preview, không đưa vào atlas. Palette/font/nền/sprite kế thừa revision. Quy tắc reset đã đối chiếu `Assets/Scripts/Core/TypingManager/TypingManager.cs`, nhánh sai của `ProcessInput`: `current = originalCurrent`.

Kiểm chứng bản này: build và `source/verify.py` PASS cho 26 PNG/SVG, atlas/padding, 21 bản xuất nearest-neighbor, crop chiến đấu không đổi, HP 0/1/50/100% cho cả ba cấp bật/tắt Slayer, tên dài/điểm lớn/combo 999 x16/từ dài và góc 9-slice. `source/verify-browser.cjs` PASS trên Chrome với Playwright đã có: ảnh local decode, bảy màn, viewport 320/640/1280 không tràn; nhập đúng/sai/reset, hết từ, lỗi liên tiếp, RAF ±1px/về gốc, checkbox và media giảm chuyển động. Các lệnh chạy giữ nguyên ở mục Chỉnh sửa và import bên dưới.

Đã xem contact sheet ở native, preview browser và component chuyển grayscale: nhận diện hai nấc/ba đỉnh cùng huy hiệu vẫn tách biệt ở bạc và vàng. Đã xem chuỗi frame screencast browser của phản hồi lỗi; phát hiện nền preview còn khung tĩnh gây viền kép khi rung, đã sửa bằng nền riêng và chạy lại checks PASS. Capture QA chỉ dùng trong task-temp rồi dọn. Chưa kiểm chứng Unity; không đổi gameplay/API, âm thanh hoặc sprite, không chạy gameplay suite.

Mở [gallery và bảng đối chiếu](index.html), [sáu màn](contact-sheet.png), [lớp component và các mức HP](component-sheet.png).

Đã dựng 21 PNG RGBA + SVG tương ứng, atlas RGBA 512×256 và sáu màn × ba kích thước. Nguồn 320×180; bản 2×/4× nhân nearest-neighbor. Revision không ghi vào Unity, menu hoặc asset v2 gốc. Sau chỉ dẫn “tự tìm trên Pinterest rồi tự đánh giá”, đã xem thêm chín pin, tự chọn nguồn bổ sung và sửa lại socket/bevel/icon. Không còn chờ user tìm hoặc chốt nguồn bổ sung.

## Tự review Pinterest — nguồn chọn và kết luận

| Pin đã mở ảnh thực | Đánh giá | Quyết định |
| --- | --- | --- |
| [Pixel HUD Health Hearts & Bars](https://www.pinterest.com/pin/993184524050127407/) | Sheet có bevel bạc sáng trên, tối dưới; hàng giữa dưới có ổ tròn nhô khỏi thanh và track còn rõ khi cạn | Chọn cho kết cấu ổ tròn/cổ nối/bevel trong hướng plan. Giữ palette v2, fill đỏ liên tục; không lấy chia ô hoặc các vòng tài nguyên mới. |
| [Dark Fantasy v2 — Finnmercury](https://www.pinterest.com/pin/194499277651664276/) | Mặt bảng gần đen, nhiều sắc cạnh kim loại; ruột fill có các dải sáng/chính/tối | Chọn bổ sung bằng chứng vật liệu bảng tối; giữ khung dày/góc gia cố/nhãn riêng theo plan. Không thêm motif đầu lâu hoặc ornament. |
| [Dark Fantasy v2 — pin thứ hai](https://www.pinterest.com/pin/309904018134685144/) | Cùng bộ với pin trên, khác crop | Đã xem; không tính là hướng thiết kế độc lập. |
| [DVA Bars ’N’ Boxes — bảng](https://www.pinterest.com/pin/334321972354241632/) | Mặt trước rộng, cạnh sáng và bóng chân phân biệt được rõ; panel có ô nhãn riêng | Dùng đối chiếu độ nổi của mặt bảng; không lấy palette sáng, inventory hay ô chức năng. |
| [DVA Bars ’N’ Boxes — thanh](https://www.pinterest.com/pin/334321972354241623/) | Icon nhô khỏi thanh, bóng rõ nhưng fill chia ô, outline trắng rất nặng | Chỉ đối chiếu silhouette icon–thanh; loại kiểu fill chia ô và outline trắng. |
| [Twisted Tales — Damien Richard](https://www.pinterest.com/pin/655062708336253558/) | Ổ portrait lớn, dây gai/gỗ; nhiều texture nhỏ | Loại: vật liệu và mật độ ornament không hợp HUD 320×180 đã chốt. |
| [Magicopalypse — Philipp](https://www.pinterest.com/pin/347410558772835860/) | Bảng gỗ/menu với viền sáng mảnh | Loại: không đủ bằng chứng cho HUD kim loại dày; ngoài phạm vi menu. |
| [Thanh tim nhiều màu](https://www.pinterest.com/pin/271975264991985333/) | Outline đen, fill trắng hoặc chia ô; rất ít chiều sâu | Loại: thiếu ổ kim loại/rãnh lõm. |
| [Thanh tim đơn giản](https://www.pinterest.com/pin/696087686188126285/) | Lòng cạn xám dễ nhận, nhưng frame phẳng | Chỉ đối chiếu đọc mức HP; loại hình thức frame. |

Tự đánh giá r1 trước sửa: rãnh và fill đạt chức năng nhưng socket có góc chéo dài, gần bát giác; mặt viền hẹp làm kim loại trông như outline; icon HP vàng lẫn với combo. Đã sửa tại `bar_assets()` và `assets()`: ổ tròn pixel bằng các vòng ellipse native, bevel trên/dưới có diện tích và sắc khác nhau, icon HP đỏ phân lớp, icon stamina tăng sáng. Không thêm ornament, đổi bố cục hoặc dùng asset bên ngoài. [Gallery](index.html) có ảnh trước/sau và reference → component → HUD cạnh sprite thật.

Tự nghiệm thu sau sửa: ổ tròn nhô rõ, collar vẫn nối vào rãnh; mặt sáng ngoài/thân/mép tối/track và bóng chân phân biệt được ở native. HP 0% còn lòng rãnh, HP 1% còn một cột đỏ; tên 10 ký tự/điểm lớn/từ dài không tràn. Socket/icon/endcap không stretch; score/typing giữ góc cố định. Boss Captain ở vùng đối thủ, Slayer nhấn vàng/x16, không có ornament giữa sân. Đạt phạm vi artifact HUD theo mô tả và các nguồn bổ sung đã tự chọn; không khẳng định khôi phục đúng ảnh 1–3 chưa nhận diện được và không suy ra nghiệm thu Unity.

## Nguồn và mapping — kiểm tra ngày 19/10/2026

| Nguồn đã mở | Nội dung thực tế | Áp dụng / giới hạn |
| --- | --- | --- |
| [Pin 1025272671408964433](https://www.pinterest.com/pin/1025272671408964433/) | The Druid, thanh boss tím đen; endcap sáng; fill đỏ và vạch tách | Đối chiếu phân lớp fill/track/vạch và đầu thanh; giữ Captain trong vùng đối thủ. Không lấy ornament giữa thanh sang giữa sân. |
| [Pin 804666658453520783](https://www.pinterest.com/pin/804666658453520783/) | Nút PLAY, nút vuông và thanh có gỗ/lá | Đối chiếu mặt sáng/thân/bóng chân; không lấy chủ đề gỗ/lá. Đây không phải bảng tối mô tả ở plan. |
| [Pin 1089871178602252731](https://www.pinterest.com/pin/1089871178602252731/) từ top_pin_ids của link search | Năm mức thanh tim, khung vỡ dần khi cạn | Lòng thanh tối vẫn rõ khi mất HP. Không đưa cơ chế vỡ khung hoặc hình răng cưa vào revision. |
| [Cyangmou — HUD Designer Kit](https://cyangmou.itch.io/pixel-menu-gui-hud-health-bars) | Đã đọc trang tác giả: hệ modular nhiều loại bar và tài nguyên | Nghiên cứu cách tách thành phần. Ảnh đầu trang truy xuất được chỉ là emblem, không đủ để khẳng định đã đối chiếu toàn bộ kit. Không nhập asset. |
| [Otsoga — Dynamic Status Bars](https://otsoga.itch.io/dynamic-status-bars) | Đã đọc mô tả border/content tách, portrait và animation | Nghiên cứu thêm; ảnh demo không tải được ở phiên này, chưa đối chiếu hình. Không nhập asset hoặc thêm motion. |
| [Smelted Health](https://soup-boardgaming.itch.io/dark-fantasy-hud) | Đã đọc trang: thanh tùy biến, endcap, dividers và bản silver/gold | Nghiên cứu thêm; các ảnh chi tiết trả lỗi, chưa đối chiếu hình. Không lấy ornament hoặc đổi hướng theo bộ này. |

Pinterest đọc trực tiếp bằng web tool bị chặn; metadata OG và ba ảnh của đúng pin đã lấy được qua HTTP và quan sát trực tiếp. Bảng trong gallery dùng URL ảnh gốc, cần mạng để tải reference; asset revision mở offline. Không đóng gói ảnh/asset của các tác giả nghiên cứu vào atlas.

Plan user là nguồn cho ổ tròn nhô ra, cổ nối và bảng tối dày. User đã giao tự tìm/tự đánh giá; các pin chọn ở trên bổ sung bằng chứng để hoàn thiện đúng hướng đó. Các link gốc chưa xác định được ảnh 1–3 nên không kết luận pixel-match với ảnh chưa thấy.

## Thay đổi của revision

- Bỏ panel lớn sau các cụm tài nguyên. Socket tròn pixel 21×21 riêng, frame có cổ nối và đầu bậc, track 77×6 và fill ba vùng sáng/chính/bóng; bevel sáng trên/tối dưới và bóng chân lệch xuống.
- Boss cùng kết cấu thanh tài nguyên, badge Captain nằm trong HUD đối thủ. Slayer chỉ nhấn vàng và x16; không có badge giữa sân.
- Score/combo và vùng gõ có mặt tối, khung dày, góc gia cố và bảng nhãn riêng. Giữ bố cục, nền và vị trí sprite của v2.
- VT323 giữ cho score/từ gõ; nhãn rất nhỏ dùng glyph 3×5 định nghĩa trong source để đọc được ở native. Đây là adaptation typography của revision cần review cùng hình. Text đúng/sai tách riêng; từ mục tiêu giữ nguyên khi nhập sai.

## Chỉnh sửa và import

Nguồn chính: [source/build.py](source/build.py), snapshot icon [semantic-basis.json](source/semantic-basis.json), SVG từng asset và các crop artwork/font kèm theo. Build chỉ ghi trong `hud-r1`. Lần build đầu lấy crop từ v2 nếu thiếu; bản bàn giao đã đủ file để rebuild offline. Không sửa riêng SVG rồi chạy build: Python là nguồn sinh đồng bộ.

```powershell
python design/ui-kit/typinghero/v2/revisions/hud-r1/source/build.py
python design/ui-kit/typinghero/v2/revisions/hud-r1/source/verify.py
node design/ui-kit/typinghero/v2/revisions/hud-r1/source/verify-browser.cjs E:/codex-data/packages/rea/node_modules/playwright-core/index.js 'C:/Program Files/Google/Chrome/Application/chrome.exe'
```

[metadata.json](metadata.json): rect atlas gốc trên trái, pivot `[0.5,0.5]`, border theo Unity **left,bottom,right,top**. Point/Clamp, mipmaps off; khoảng cách sprite 6px, extrusion 2px. Track/fill tại `(20,7,77,6)` so với frame 102×21. Crop fill ngang liên tục; 0% không vẽ fill, giá trị dương nhỏ nhất hiện một cột pixel. Vạch chỉ là decoration.

Không stretch socket, icon, collar, badge hoặc endcap. Thanh hiện tại là kích thước cố định; đổi độ dài bằng source tại native rồi xuất lại. Frame score/typing hỗ trợ 9-slice border 8px, label 4px; ví dụ co giãn trong component sheet giữ nguyên góc.

## Kiểm chứng

Đã chạy build, verifier và headless Chrome gallery. Đã quan sát sáu màn contact sheet, normal ở native 320×180, stress 1280×720, error 640×360 và bảng component.

- PASS: 21 PNG alpha, SVG tái dựng đúng từng pixel; rect atlas khớp PNG, không chồng lấn; bản 2×/4× khớp nearest-neighbor từng pixel.
- PASS: HP 0/1/50/100% tương ứng 0/1/38/77 cột fill, phần còn lại giữ lòng rãnh. Góc frame giữ nguyên qua hai kích thước 9-slice.
- PASS: tên 10 ký tự `WWWWWWWWWW`, score `2,147,483,647`, combo `999 / x16`, từ dài `counterrevolutionary`, trạng thái đúng và `resxlve` sai. Các chuỗi nằm trong giới hạn đo của vùng chữ.
- PASS: crop vùng chiến đấu y65–132 không đổi so với nền/sprite v2; không thêm ornament trong vùng đó. Nhãn typing và thanh nằm tại vùng HUD hiện có.
- PASS sau sửa: 20 ảnh local trong gallery decode được; sáu màn, ảnh trước/sau, link anchor đối chiếu; không tràn ngang ở viewport 320/640/1280. Ảnh Pinterest ngoài mạng không nằm trong phép kiểm offline.
- Chưa kiểm chứng: pixel-match ảnh 1–3 gốc chưa nhận diện; importer, binding, hành vi và hiển thị Unity. Đây là artifact review tĩnh, không chạy gameplay suite.

Provenance background/sprite/icon/font tiếp nối [CREDITS v2](../../CREDITS.md); revision không dùng ImageGen. Không thay gameplay/API/dữ liệu.
