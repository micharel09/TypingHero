# Nghiệm thu thiết kế v2

19/10/2026. Đã kiểm gói asset/mockup để review. Không phải nghiệm thu Unity/gameplay.

- Artifact verifier PASS: 116 PNG/SVG đúng kích thước, XML hợp lệ, alpha UI 0/255 và RGB trong suốt bằng 0; 116 rect atlas khớp PNG. Góc của mọi asset nine-slice giữ nguyên byte khi kéo rộng/cao. Badge và boss endcap không nine-slice.
- 30 màn × 3: bản 2× và 4× trùng byte với nearest-neighbor của 320×180. Visible pixels của hero/skeleton/Captain khớp frame nguồn 1×; không đổi pose hoặc màu.
- Text bounds trong build PASS, không tự giảm font: tên WWWWWWWWWW, điểm 2,147,483,647, combo 999/x16, từ counterstrike. Low HP 1/100; lỗi tên/typing; loading/empty/error, locked/unlocked có màn riêng.
- Đã xem menu/HUD boss/Slayer/stress/Collection/Achievements/pause ở export 4×. Cùng nhịp pixel, vùng giữa tối, sprite có không gian riêng; badge và thanh boss nằm trong cụm đối thủ. Chữ phụ 8px có ít chi tiết hơn body 10px; cần user review ở native 320×180. Không claim đã đối chiếu reference ảnh bị thiếu.
- Chrome headless PASS: file URL/font/JS, auto menu, replay, nút/Esc skip, search đúng 5 primary state, group đúng 4 brand, dialog mở/đóng, decode mọi ảnh, không lỗi JS/request. Kiểm overflow tại 320/640/1280.
- GIF tổng duration 2400ms; gallery intro giữ menu đến 2400ms rồi ẩn skip. HTML links local tồn tại; ZIP CRC và byte parity khớp toàn bộ file bàn giao.
- Nguồn chính là Python/SVG; basis snapshot và font/crop nằm trong ZIP. Không tạo service, worktree, hoặc sửa file Unity. Không chạy gameplay suite vì phạm vi chỉ là thiết kế.

Lệnh:

```powershell
python design/ui-kit/typinghero/v2/source/build.py
python design/ui-kit/typinghero/v2/source/verify.py
node design/ui-kit/typinghero/v2/source/verify-browser.cjs E:/codex-data/packages/rea/node_modules/playwright-core/index.js 'C:/Program Files/Google/Chrome/Application/chrome.exe'
```

Giới hạn: ảnh reference đính kèm của phiên trước không hiện diện, Pinterest không truy cập được. Chưa đối chiếu trực tiếp các ảnh đó. Mockup không chứng minh Unity importer, binding/gameplay, hiệu năng hoặc online.
