# Khôi phục Slayer điện ảnh và hiệu ứng điểm

## Acceptance

Bố cục HUD góc trong task này đã được thay bằng [plan HUD dưới nhân vật](combat-hud.md) theo quyết định user 2026-10-18; phần nền đỏ, framing và hiệu ứng điểm vẫn được giữ.

Nền đỏ gốc, ẩn/khôi phục Shogun; hai dải đen hết ngang màn hình bám mốc thân người với khoảng 8 px tại 960×540 và camera zoom. Điểm/multiplier cạnh nhau trên dải trên, zoom và pulse chỉ khi cộng điểm; màu ScoreRules trắng/vàng x2/đỏ x3, popup cùng màu gần điểm. HP/stamina nhỏ ở góc, typing trên dải dưới, bỏ panel HUD khi Slayer, tutorial đọc được. Giữ GUID/public API/gameplay.

## Phạm vi hiện hành
User cập nhật: chỉ hoàn tất kiểm chứng rồi báo; chưa xuất ZIP. Không chạy production/Windows hoặc đóng gói. QA WebGL chỉ phục vụ kiểm chứng runtime.

## Thực hiện và checks
- SlayerScreenFX: framing theo collider với mốc đứng cố định, khoảng hở 8 px; camera mở hết ngang; khôi phục môi trường/panel/layout khi thoát, disable và unload.
- ScoreSystem: pulse ở AddScore; ScoreUI_ComboHUD: bám phần chữ điểm; binder zoom nhóm riêng, thoát về scale gốc. FloatingScore: sửa pivot để anchor không lệch 100 px.
- ShogunTheme builder: nhóm điểm + binder, bars ngoài mask/full canvas; theme giữ màu đen; tutorial highlight theo typing tray.
- Scene serialize bằng Unity: giữ đủ 652 ID cũ, thêm 7 ID; GUID/.meta không đổi. Lần cuối sẽ lấy thêm reference caption và các cấu hình mới.
- Doctor, Quick, WebGL keyboard x2/x3; 960×540,1280×720,1920×1080,640×360; tutorial/pause/exit/victory/restart; builder hai lần.

## Checkpoint
- Checkout dirty từ task trước được giữ; không tạo branch/worktree/commit. Temp: E:/CodexTemp/TypingHero/slayer-restore.
- Doctor PASS. Quick baseline44/44, hồi quy pulse/tier45/45; runner8/8 trên Ubuntu. Unittest cục bộ không chạy vì fcntl chỉ có Linux, đã chạy đúng trên fly-server.
- WebGL đầu dừng tại builder: helper Value không hỗ trợ Vector2; đã sửa bằng SerializedObject.vector2Value tại owner. Lượt 20261018T032617 vượt repeat-builder và tutorial đầy đủ, pause/retry/restart/clean battle, không có ghi điểm thật.
- Đã xem ảnh tutorial Slayer: nhãn SCORE chồng tổng điểm; popup lệch sang HP do pivot -0.5; collider có khoảng trống trên sọ/dưới đế. Sửa caption toggle, popup pivot, mốc đứng inset .28/.12 world units (đối chiếu ảnh ở 53.846 px/world unit), giữ collider/gameplay nguyên vẹn. Chưa chốt hình ảnh sau sửa.
- Lượt QA cũ đã chủ động dừng sau khi kiểm tra đúng container/mount, vì source đã có các sửa cuối; không tính là WebGL PASS. Quick cuối đang chạy, tiếp theo QA WebGL mới và xem ảnh thực tế. Chưa xuất ZIP.

- Quick cuối 20261018T033558: 46/46 PASS (snapshot bcd4c310...). WebGL 20261018T033726 đã dừng sớm trước kiểm thử để sửa phép đổi tọa độ mốc đầu; không tính PASS.
- WebGL hiện hành 20261018T033806, snapshot29dba92a...: inset đầu .28, chân .12; repeat-builder PASS, đang link WASM. Scene cuối đã lấy về: 652 ID cũ/659 tổng, caption reference và bodyVerticalInset đúng.
- Driver được chỉnh để chờ promotion trước từ tiếp theo, đo framing bốn cỡ trong tutorial reading (camera gameplay), đo điểm/resize riêng khi pause (camera focus vốn có). Nếu cần sẽ chạy lại driver trên cùng QA payload, không rebuild runtime.
- Resize 640×360 ban đầu bắt layout còn scale2 từ 1920, trong khi render đã640; state score x48 thay vì229. Chỉ sửa driver chờ4frame thực sau render-size change, không đổi runtime/acceptance. Chạy lại trên QA payload29dba92a..., đã qua cả4assert framing8px; ảnh640 và3cỡ còn lại đã xem, không clipping chữ/panel/nhân vật.
- Browser rerun ở /data/typinghero-game-loop/task-temp/slayer-restore/full với driver riêng (QA payload không đổi). Đang tiếp tutorial/normal/victory/restart và input x2/x3, chưa kết luận toàn bộ PASS.

### Hoàn tất acceptance
- WebGL broad hoàn thành134checks trước khi driver x3 hết thời gian Slayer do chờ/chụp giữa các từ; không ghi toàn profile là PASS. Chạy lại riêng cùng QA payload bằng input liên tục:30/30 PASS,50.575s, điểm287 tại x3/đỏRGB(1,0,0), đối chiếu đúng mọi điểm ký tự và rounding cũ. Không sửa runtime để đáp ứng driver.
- Đã xem x2/popup vàng, x3 đỏ, exit khôi phục nền/HUD/scale; pause giữ điểm, resize4cỡ và restart đúng. Broad đã qua tutorial, ba trận thắng, defeat/restart, modal/embedding/preferences. Không có Unity/JS exceptions, không ghi Firebase thật.
- Gaps4cỡ đều8px reference (sai số float lớn nhất.000016). Doctor,46Quick,8runner contracts và repeat-builder đạt. Bằng chứng và giới hạn ở docs/game/verification.md.
- User chưa yêu cầu ZIP: không production/Windows/đóng gói. Tiếp: diff/link checks và dọn temp/capture đã có receipt; giữ source/cache/QA build Ubuntu, checkout dirty gốc.
- Final Doctor PASS sau khi đồng bộ source/driver/scene cuối lên Ubuntu: snapshot6adde6ec8a9bf33e6a8d63b1abb3fe9022a86f80b080893c783a76b67bda7c5d. QA payload vẫn29dba92a...; chỉ khác scene serialize/whitespace và driver đã được kiểm lại, không đổi logic runtime.
- Slayer driver cuối SHA256f8ba00cb8c8c3cbe1852133929eaa3100eef5025c836dc80b11335ae2a3dcab3. Diff whitespace checks PASS;11ZIP cũ giữ nguyên SHA256. Không production/Windows/archive/commit/push.
- Cleanup hoàn tất:8thư mục kết quả/temp riêng trên Ubuntu và temp/capture/bytecode riêng local đã xóa sau receipt. Không còn container kiểm thử của task. Giữ source/Library/cache/QA player Ubuntu để tiếp tục khi cần; không có branch/worktree mới. Acceptance đã hoàn tất; ZIP/release đang hoãn theo user.
