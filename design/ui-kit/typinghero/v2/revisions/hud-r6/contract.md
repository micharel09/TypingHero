# HUD r6 — contract review tĩnh

Nguồn intent: plan user HUD r6 ngày 19/10/2026. Chỉ nâng chiều sâu HUD, giữ r5 để so sánh. Không xuất atlas sản phẩm, tích hợp Unity, đổi public API, font, dependency hoặc animation.

Canvas 320×180; các ảnh 640×360 và 1280×720 dùng nearest-neighbor. Reuse trực tiếp r4/source/build.py, primitives.py và r5/source/propose.py; không ghi vào các revision nguồn.

| Vật liệu | Mapping r6 | Ràng buộc |
| --- | --- | --- |
| Khung HP/combo/stamina | metal/bevel phía trên/trái, steel/shadow phía dưới/phải | Alpha, socket/endcap, fill và track giữ nguyên |
| Socket | mặt vát cùng hướng sáng, lòng shadow/inner; icon nằm trên lòng tối | Không thêm đinh, nhiễu, glow; giữ tim/chevron/Captain |
| Score | coal, mép metal/bevel, bóng trong shadow, đáy inner | Chữ sáng, tọa độ và kích thước giữ nguyên |
| Khung gõ thường | rim duy nhất r5 đổi thành metal trên/trái và steel dưới/phải; lỗi giữ error ở cạnh sáng | Không thêm vòng viền; coal, footprint 198×30 tại (61,146) |

Bóng nằm trong alpha/footprint cũ, cạnh tối 1px; không dịch component. HP fill 77×6, phụ 84×4; số cột HP 0/1/50/100% là 0/1/38/77, phụ 0/1/42/84. Không che fill bằng bevel. Glyph origin (65,151), clip (65,150,190,22), VT323 16px/gap8. Màu ivory/muted/metal, tiến độ jade và lỗi error giữ nguyên.

Slayer không có nền/viền/shadow riêng quanh ba từ. Hai dải đen giữ công thức head−padding và foot+padding từ r5: landmark foot138, padding8 → top82/bottom146. Không sửa projection Unity. Parry/timer và cảnh/actor ngoài material mask không đổi giữa trước/sau.

Lỗi/reset180ms và chuyển từ100ms cubic tiếp tục kế thừa contract r4/r5. Preview này tĩnh, không xác nhận lại timing tương tác, WPM, Parry hoặc runtime Unity.

Trang review có normal/correct/error/Slayer/elite/boss/HP thấp và ma trận ba cấp × Slayer bật/tắt × HP0/1/50/100%, phóng1×/2×/4× và grayscale. Các ca ngoài bốn preview r5 gốc là baseline mở rộng dựng từ cùng renderer r4 và khung r5, không phải asset r5 đã xuất trước đó.
