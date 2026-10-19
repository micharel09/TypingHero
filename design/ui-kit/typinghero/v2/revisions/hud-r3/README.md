# HUD r3 — đọc trước, chuyển từ tức thời

Bản **draft review**, chưa tích hợp Unity. Mở [gallery](index.html), [native/contact sheet](contact-sheet.png), [wireframe](wireframe/native.wireframe.html), [contract](wireframe/contract.md) hoặc [r2 gốc](../hud-r2/index.html).

Nguồn: plan “Khung gõ r3 — đọc trước, chuyển từ tức thời” user bàn giao ngày 19/10/2026. Snapshot `hud-r2/source/build.py`, `primitives.py`, semantic-basis, artwork/font → source và asset r3; camera, HP, cấp đối thủ, Slayer, grayscale, responsive giữ từ r2. Chỉ thay khung gõ và mô hình queue trong gallery. `Assets/Scripts/Core/TypingManager/TypingManager.cs:203` xác nhận ProcessInput bỏ nonletter, current/next/third và type-ahead; gallery là mô phỏng review riêng, không đổi contract hoặc code Unity. Không cập nhật memory gameplay vì không thay game.

Khung158×36 tại(81,140), đáy176, border LBRT[4,4,4,4], nền kim loại tối/viền bạc gọn. Current VT32316 vẽ y144, preview VT32312 vẽ y160, cùng tâm x160. Preview dùng palette muted, không opacity toàn lớp. Toàn từ luôn giữ vị trí; chữ đúng đổi màu tại chỗ. Không shake, slide/fade/zoom/caret/NEXT. Slayer giữ bạc. Actor feet y138; Parry/timer giữ nguyên.

Normal lặp resolve/honor/courage/guard/strike; stress lặp a/to/counterrevolutionary/i/resolve. Hoàn tất chuyển queue đồng bộ ngay trong vòng xử lý input; event nhiều từ xử lý tiếp ký tự trên từ mới. Nonletter bị bỏ qua; không cần Space. Sai reset current, giữ next, đỏ vị trí sai/viền180ms; lỗi mới restart, chữ đúng kết thúc lỗi cũ. Reset về đầu queue. Camera/giảm chuyển động theo r2, chữ luôn đứng yên.

Checkbox “Đối chiếu khung r2 (cùng chuỗi)” dùng frame gốc r2 và vị trí một hàng trong gallery r3, dùng chung queue/input để so cùng chuỗi; đây là so geometry, không sửa hoặc mô phỏng lại feedback rung/input lặp của gallery r2 gốc. Tắt checkbox để xem lại hai hàng r3. Link r2 và so sánh ảnh native vẫn giữ nguyên.

Wireframe JSON/HTML/PNG và contract được lưu trước khi dựng asset. Bundle giữ draft; helper check chỉ dừng tại approval.status=draft, không ghi duyệt giả. JSON grouping3× vì renderer flex; geometry chính xác nằm trong contract/metadata. Không dùng wireframe làm runtime layout.

`source/build.py` sinh 24 component PNG/SVG pixel-run, atlas512²/extrusion2px, 48 overlay HUD, 10 màn×3 kích thước nearest-neighbor, các lớp chữ từng tiến độ/sai và preview cho queue. Source raster VT323 giữ nguyên cỡ16/12 cả từ dài. PNG chữ không đưa vào atlas component. Không thêm dependency hoặc mạng. Sửa nguồn trong source rồi rebuild, không sửa riêng index.html.

Lệnh kiểm từ repo root:

```powershell
python.exe design/ui-kit/typinghero/v2/revisions/hud-r3/source/build.py
python.exe design/ui-kit/typinghero/v2/revisions/hud-r3/source/verify.py
node design/ui-kit/typinghero/v2/revisions/hud-r3/source/verify-browser.cjs
python.exe E:/codex-data/skills/wireframe/scripts/wireframe.py validate design/ui-kit/typinghero/v2/revisions/hud-r3/wireframe
python.exe E:/codex-data/skills/wireframe/scripts/wireframe.py check design/ui-kit/typinghero/v2/revisions/hud-r3/wireframe
```

Browser verifier mặc định Chrome local/Playwright có sẵn tại E:/codex-data/packages/rea/node_modules/playwright-core; tham số lần lượt override module, Chrome, task-temp. Capture/recording chỉ vào E:/CodexTemp/TypingHero/hud-r3, dọn sau khi ghi kết quả. Không lưu capture QA vào repo; PNG wireframe và màn thiết kế là deliverable được yêu cầu.

Kiểm chứng ngày19/10/2026, Windows/Chrome headless: artifact PASS cho PNG/SVG từng pixel, atlas/extrusion cả góc, alpha component, nearest-neighbor, rank/Slayer/HP0/1/50/100, border và scene isolation. Bounds font xác nhận gy và counterrevolutionary không cắt ở cả16/12. Đã xem native/stress/hai từ dài/contact sheet: hàng phân biệt, preview đọc được, không chồng chữ hoặc che chân actor; không scale cỡ chữ theo lượt.

Browser PASS: gõ150/100/60ms hoàn tất đủ5 từ, một event chứa nhiều từ và nonletter, reset, stress queue, đúng sau lỗi, sai đầu/giữa/cuối, lỗi liên tiếp, preview đúng current tiếp theo. Fixture tạm dùng cùng handler với hai counterrevolutionary liên tiếp trong một event, ký tự g tiếp theo vào guard; không đổi bộ stress chính thức. Chuyển từ và preview được cập nhật trước RAF kế tiếp, không frame rỗng. Đỏ đo182.0ms sau lần restart trong lượt đầu, nằm trong180ms cộng một frame; offset luôn0. Camera hoạt động cùng gõ; checkbox/system reduced motion, grayscale và viewport320/640/1280 PASS. Đã mở recording playbackRate1 và xem frame trung gian; kết luận chỉ cho bản review Chrome, không chứng minh tăng WPM, gameplay hay hiệu năng Unity. R2 giữ nguyên để review.
