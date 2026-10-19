# HUD r4 — một hàng, ba từ

**Draft review, chưa tích hợp Unity.** Mở [gallery](index.html), [native](screens/320x180/hud-normal.png), [contact sheet](contact-sheet.png), [wireframe](wireframe/native.wireframe.html), [contract](wireframe/contract.md) hoặc [r3 gốc](../hud-r3/index.html).

Nguồn: plan HUD r4 user bàn giao ngày19/10/2026. Snapshot r3 source/primitives/artwork/font; giữ palette, HP, rank, Slayer, scene/camera và actor feet138. Chỉ thay khung gõ và queue hiển thị bản review. Không sửa Unity/public API/pool game. R2/r3 giữ nguyên.

Khung198×30 tại(61,146), border4; current bắt đầu(65,151), next/third cách8px, cả ba VT32316. Current ivory, chữ đúng jade tại chỗ; next muted, third metal, không fade lớp. Khi gõ giữ toàn từ và điểm bắt đầu; không co hàng, rung hoặc phóng chữ. Parry/timer giữ nguyên.

Queue/input chuyển ngay khi hoàn tất. Hàng mới bắt đầu tại vị trí next cũ rồi trượt trái100ms ease-out cubic. Input vẫn xử lý trong animation; hoàn tất tiếp kết thúc chuyển cũ rồi bắt đầu chuyển mới. Burst nhiều từ cùng event chỉ paint trạng thái cuối và chuyển tức thời. Nonletter bỏ qua, không cần Space. Sai reset current/giữ queue, đỏ vị trí sai và border180ms; lỗi tiếp restart; đúng xóa lỗi. Reset về đầu queue. Giảm chuyển động qua checkbox hoặc hệ thống chuyển tức thời và dừng camera.

Pool từ Assets/Data/Words/WP_Common200.asset, GUID gắn SampleScene98cf087e4b378b64a9a4756bbebf1d6d:220 mục,219 từ duy nhất sau lowercase/dedup, snapshot tại [source/pool.json](source/pool.json). Từ dài nhất different54px; lặp ba lần cần178px, vừa lòng190px. Gallery có checkbox pool thật, normal và stress; stress ưu tiên khi cùng bật. Không đổi pool để ép vừa. Stress giữ current đầy đủ, preview có thể cắt mép phải hoặc ngoài vùng nhìn; không giảm font/xuống hàng/marquee. Chuỗi stress có counterrevolutionary114px; không bảo đảm current cho chuỗi tùy ý dài hơn lòng190px.

Đối chiếu r3 dùng chung queue/input và geometry hai hàng/frame/preview12 từ r3; link gallery r3 gốc vẫn giữ. Camera mặc định dừng; chu kỳ4s, offset nguyên1/2/4px chỉ áp scene. Scale1×/2×/4× nearest-neighbor.

Wireframe/contract lưu trước khi sinh asset. Bundle draft; helper check chỉ dừng tại approval.status=draft, không ghi approval giả. Wireframe grouping3×; geometry chính xác thuộc contract/metadata. Deliverable:24 component PNG/SVG pixel-run, atlas512²/extrusion2px,48 overlay,10 màn×3 kích thước, glyph raster theo state, metadata và gallery offline. Chạy lại từ repo root:

```powershell
python design/ui-kit/typinghero/v2/revisions/hud-r4/source/build.py
python design/ui-kit/typinghero/v2/revisions/hud-r4/source/verify.py
node design/ui-kit/typinghero/v2/revisions/hud-r4/source/verify-browser.cjs
python E:/codex-data/skills/wireframe/scripts/wireframe.py validate design/ui-kit/typinghero/v2/revisions/hud-r4/wireframe
python E:/codex-data/skills/wireframe/scripts/wireframe.py check design/ui-kit/typinghero/v2/revisions/hud-r4/wireframe
```

QA19/10/2026 Windows/Pillow/Chrome headless: artifact PASS từng pixel PNG/SVG/alpha, atlas/extrusion cả góc, nearest-neighbor30 exports, border, rank/Slayer/HP0/1/50/100, scene isolation và bounds pool219/worst triple. Browser PASS lỗi đầu/giữa/cuối, lỗi liên tiếp/restart180ms, đúng sau lỗi/reset; gõ150/100/60ms đủ5 từ, uppercase/nonletter, burst, queue/third đúng thứ tự, hoàn tất tiếp trong animation100ms và gõ hết pool219 không mất/lặp. RAF kiểm layer chữ decode sẵn, không frame trắng; offset cubic giảm về0. Camera, checkbox/system reduced motion, grayscale, viewport320/640/1280 PASS.

Đã xem native/stress/ba different, contact sheet, frame chuyển hàng và recording playbackRate1. Ba từ pool rõ ở native, không đụng Parry/timer/chân actor; stress cắt preview đúng giới hạn. Capture/video QA tạm đã dọn sau khi ghi kết quả. Chỉ nghiệm thu hành vi/khả năng đọc bản review Chrome.100ms là giá trị thử; chưa đo WPM/Parry hoặc nghiệm thu gameplay/hiệu năng Unity. Cơ sở lựa chọn do plan cung cấp: [preview research](https://pubmed.ncbi.nlm.nih.gov/27576520/), [Monkeytype](https://monkeytype.com/settings?highlight=minWpm); không suy ra phương án tối ưu cho TypingHero.
