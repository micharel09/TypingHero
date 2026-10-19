# HUD r6 — kim loại tối, chiều sâu vừa

Mở [review trước/sau r5–r6](index.html) và [contract vật liệu](contract.md). Phạm vi chỉ preview tĩnh theo plan user ngày 19/10/2026; r5 và các thay đổi Unity có sẵn được giữ nguyên.

Reuse palette/primitive r4 và khung gõ/Slayer r5. `source/build.py::material` chỉ đổi sắc độ các asset khung/socket/score khi render trong bộ nhớ; fill, track, icon và renderer chữ dùng nguyên nguồn. Mặt trên/trái metal/bevel sáng tiết chế, dưới/phải steel/shadow; socket giữ lòng tối và bóng tiếp xúc quanh icon. Score chuyển deep sang coal, có bóng trong mỏng; chữ sáng hơn khung. Khung gõ đổi sắc độ trên đường viền r5 duy nhất. Không sinh asset component, atlas, font, dependency hoặc animation mới.

Trang có 31 cặp preview, mỗi cặp ba kích thước: normal/correct/error/Slayer, elite/boss/HP thấp và ma trận ba cấp × Slayer bật/tắt × HP0/1/50/100%. Bốn baseline đầu khớp từng byte với ảnh r5 hiện có; các ca bổ sung dựng bằng r4 + khung r5 cho phép so cùng dữ liệu. Controls chọn trạng thái, phóng1×/2×/4× và grayscale hoạt động trên cả hai ảnh. Trang mở trực tiếp bằng file URL, không cần server.

Kiểm ngày 19/10/2026:

- Python/Pillow PASS 31 cặp × 3 kích thước. So pixel ngoài alpha footprint material; cảnh/actor và vùng ngoài HUD không đổi giữa trước/sau. Alpha/silhouette component không đổi. Các vùng fill HP/combo/stamina, glyph/clip, Parry/timer khớp từng byte. HP0/1/50/100% giữ 0/1/38/77 cột; fill phụ giữ nguyên. PNG ghi/đọc và nearest-neighbor khớp từng byte.
- Chrome headless qua Playwright PASS186 tổ hợp trạng thái × scale × grayscale: chọn controls thật, decode cả hai ảnh, natural/display dimensions đúng, CSS pixelated, grayscale đúng; không page/request error. Browser tự đóng sau kiểm.
- Đã xem native Slayer, normal2×, boss HP1%4× và trang so sánh elite grayscale2×. Ba từ đọc rõ, cạnh sáng không lấn chữ; tim/chevron/Captain tiếp tục phân biệt bằng hình, HP1% vẫn có cột fill. Score giữ tương phản chữ cao hơn mặt/khung; Slayer không xuất hiện bảng riêng quanh từ. Capture QA tạm được dọn sau khi ghi kết quả.

Chạy lại:

```powershell
python design/ui-kit/typinghero/v2/revisions/hud-r6/source/build.py
node design/ui-kit/typinghero/v2/revisions/hud-r6/source/verify-browser.cjs E:/codex-data/packages/rea/node_modules/playwright-core 'C:/Program Files/Google/Chrome/Application/chrome.exe'
```

Giữ VT32316/gap8, palette tiến độ, lỗi180ms và chuyển từ100ms trong contract. Preview tĩnh không xác nhận lại timing/input, WPM/Parry hoặc projection/hiệu năng Unity. Chưa tích hợp Unity, xuất atlas sản phẩm hay đổi public API. Các ảnh preview là tài liệu review được yêu cầu bàn giao.
