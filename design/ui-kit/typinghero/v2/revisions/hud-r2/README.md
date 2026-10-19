# HUD r2 — draft review trên cảnh chiều sâu

Mở [gallery offline](index.html), [10 màn native](contact-sheet.png), [component](component-sheet.png), [component grayscale](component-sheet-grayscale.png), [wireframe](wireframe/native.wireframe.html) và [contract](wireframe/contract.md). Giữ nguyên [r1](../hud-r1/index.html) để so sánh. Đây là bản review độc lập, chưa tích hợp hoặc nghiệm thu hiển thị/hiệu năng Unity.

Nguồn quyết định: plan HUD r2 user giao ngày 19/10/2026. Các tọa độ trong plan là nguồn chính xác; không tự đổi gameplay, API, âm thanh, sprite hoặc camera game. Bundle wireframe giữ `approval.status=draft` đúng yêu cầu; `validate`/`render` thành công, `check` xác nhận receipt/artifact rồi dừng ở điều kiện chưa duyệt. Không ghi approval giả để vượt helper. JSON wireframe biểu diễn grouping tại 3× vì renderer flex không hỗ trợ tọa độ tuyệt đối; contract và metadata giữ geometry nguồn 320×180. Task cho phép dựng asset/gallery review từ plan cụ thể này, chưa cho phép tích hợp sản phẩm.

## Quyết định và mapping nguồn

| Nguồn → owner r2 | Tái sử dụng / thay đổi |
| --- | --- |
| `hud-r1/source/build.py` → `source/primitives.py` | Snapshot hàm Pillow, palette, glyph 3×5, socket/frame HP, frame gõ, raster VT323, actor; không import hoặc ghi vào r1 khi rebuild. |
| `hud-r1/source/semantic-basis.json`, artwork/font → bản copy cùng tên tại r2 | Giữ pixel, kích thước và màu sprite; tim bạc/đỏ, badge Captain, pause/Space. Font VT323 và OFL giữ nguyên. Nguồn/gán công artwork kế thừa [CREDITS v2](../../CREDITS.md). |
| Plan r2 → `source/build.py` / `metadata.json` | HP giữ (3,11)/(215,11), 102×21, fill 77×6. Thanh phụ xuống (3,34)/(215,34), 102×13, socket13, icon7, fill84×4 tại (14,4). Bỏ góc/glint phụ. |
| Plan r2 → score/combo/status | Score 98×22 tại (111,3), VT323 12px, bỏ nhãn SCORE/đinh sáng; combo glyph top y28 và status top y40. Giữ đủ `2,147,483,647`, `999 COMBO / x16`. |
| Plan r2 + pattern gõ r1 → `source/gallery.html` | Frame 158×30 tại (81,146), chỉ một từ; overlay tiến độ/sai từ Pillow. Sai reset; đỏ180ms, rung ngang ±1px/120ms, lỗi mới restart. Đúng/hoàn tất/reset hoạt động trong lúc camera chạy. |
| Nền/sprite r1 → `scene_assets()` và scene layers | Tách nền xa/cột giữa từ pixel có sẵn, vẽ sàn phối cảnh y112–145 điểm tụ(160,80), chỉ giữ foreground ở hai mép18px. Actor feet y138. |
| Plan r2 → RAF camera gallery | Sinusoid 4000ms, integer round: xa±1, giữa/actor±2, sàn/foreground±4; không zoom/xoay. HUD và typing tách khỏi scene. Phát/Dừng ở ngoài HUD, mặc định dừng. |

Slayer chỉ đổi thanh combo trái, combo/hệ số và `SLAYER / x16` sang vàng; HP đỏ/viền bạc, stamina nền tảng, score tối, typing bạc. Lỗi typing ưu tiên đỏ. Thường tim trong socket bạc; Elite hai nấc2px, hai chevron đặc cách một hàng tối; Boss ba đỉnh3px, badge Captain có khoảng nền tối. Track và số HP không bị badge che. Không thêm pulse/flash.

Giảm chuyển động là **checkbox OR thiết lập hệ thống**: dừng camera về gốc, tắt rung, giữ màu lỗi. Nút camera disabled khi giảm chuyển động; bỏ giảm chuyển động không tự phát. Dừng camera bình thường giữ vị trí hiện tại. Điều khiển gallery reflow ngoài HUD; preview chỉ scale số nguyên1/2/4 theo chỗ trống, không scale phân số hoặc đổi bố cục HUD.

## Nguồn và đầu ra

`source/build.py` sinh 24 component PNG alpha/SVG pixel-run khớp nhau, atlas512×512 có extrusion2px, metadata, 10 màn×3 kích thước và gallery. `source/gallery.html` là nguồn HTML/CSS/JS; không sửa riêng `index.html`. `source/primitives.py` là snapshot primitive r1, đã đặt ROOT tại r2. Chỉ Python/Pillow và Playwright/Chrome sẵn có, không thêm dependency hoặc mạng.

`artwork/scene-{far,mid,floor,foreground}.png` là các lớp cảnh320×180 riêng; sprite nguồn trong cùng thư mục. `artwork/hud-<rank>-<slayer>-<stress>-<hp>.png` là 48 overlay HUD alpha để gallery đổi trạng thái lossless. `png/word-*.png` là lớp từ/tiến độ/sai cho hai độ dài, không đưa vào atlas component. Metadata ghi rect, pivot/anchor, border LBRT, atlas rect, fill crop và thông số phản hồi/camera. HP/socket cố định không stretch; score border[3,4,3,3], secondary[2,2,2,2], typing[8,8,8,8]. Không áp border8px cho panel mỏng.

## Lệnh kiểm

```powershell
python design/ui-kit/typinghero/v2/revisions/hud-r2/source/build.py
python design/ui-kit/typinghero/v2/revisions/hud-r2/source/verify.py
node design/ui-kit/typinghero/v2/revisions/hud-r2/source/verify-browser.cjs E:/codex-data/packages/rea/node_modules/playwright-core/index.js 'C:/Program Files/Google/Chrome/Application/chrome.exe' E:/CodexTemp/TypingHero/hud-r2
python E:/codex-data/skills/wireframe/scripts/wireframe.py validate design/ui-kit/typinghero/v2/revisions/hud-r2/wireframe
python E:/codex-data/skills/wireframe/scripts/wireframe.py check design/ui-kit/typinghero/v2/revisions/hud-r2/wireframe
```

Đối số cuối browser verifier chỉ định task-temp để lưu screencast/timing/capture khi kiểm; bỏ đối số này để chỉ chạy assertion. `check` trả exit1 ở draft là dự kiến; không dùng kết quả đó làm chứng nhận approval. Capture wireframe là nguồn thiết kế được yêu cầu nên giữ trong bundle; capture QA chỉ ở task-temp rồi dọn sau khi ghi kết luận.

## Kiểm chứng và giới hạn

Artifact verifier PASS: 24 PNG/SVG khớp từng pixel và alpha nhị phân; atlas đúng rect, padding và extrusion2px kể cả bốn góc; 30 màn xuất đúng nearest-neighbor. Kiểm mọi cấp×Slayer×HP0/1/50/100: HP có0/1/38/77 cột, thanh phụ0/1/42/84 cột; số cột không chịu ảnh hưởng icon/socket. So crop xác nhận Slayer không đổi HP/stamina/score/typing. Border từng frame và floor/foreground isolation đúng metadata; nội dung dài không vượt limit.

Đã xem contact sheet và native `depth-elite`: score giảm43→22px, bỏ bảng nhãn nhô, thanh phụ thấp21→13px, combo/status lên trên, vùng giữa sân có thêm chỗ cho actor. Từ gõ vẫn giữ vị trí và kích thước r1. Đã xem component grayscale cho tất cả mức fill: thường/Elite/Boss tách nhau bằng tim/hai chevron/Captain và hai/ba nấc; Slayer không làm mất nhận diện. Đường sàn tối hơn nhân vật và chữ; foreground chỉ ở mép.

Browser: đang kiểm; chưa ghi PASS khi chưa có kết quả. Kiểm hình và browser review chỉ xác nhận HUD trên cảnh chiều sâu minh họa; không xác nhận gameplay, Unity, trình duyệt khác hoặc frame-rate thiết bị.
