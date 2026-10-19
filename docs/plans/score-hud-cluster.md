# Gom UI tính điểm — hoàn tất local

## Đã làm

- Tổng điểm → hệ số × → decay thành một ScoreCluster giữa HUD. Player chỉ tên/HP; enemy stamina giữ nguyên. ×1 ẩn multiplier/decay, total giữ vị trí/pulse. Thanh mỏng reuse R7, không art mới.
- Layout960: ComboHUD là child(0,69,294,54) của ScoreCluster; multiplier(0,0,294,30), decay(12,36,270,12). Player status306×63. ROUND hạ native y38→46 để không đè decay. Decoration combo cũ inactive giữ ID; runtime tint chỉ còn fill.
- ScoreSystem/ScoreRules/input/damage/provider không đổi. Không sửa cơ chế pre-threshold/typo; không public API, dependency hoặc Firebase mới.

## Kiểm chứng

- Unity2022.3.25f1 serialize20261019T151210 strict repeat PASS; nhập SampleScene do Unity serialize.853/853 IDs, scripts và persistent event blocks giữ nguyên; art bytes không đổi.
- QA20261019T151409 build/repeat PASS, input `62da79d2242a0ad8691df7190c96ae2d271ee973c9171eedfa3ea7aa9222adf9`. Final native Edge155.0.4283.62/Playwright1.58.2/NVIDIA GTX1650Ti D3D11:209/209 PASS trong66.194s. Driver cuối `tools/game_loop/score_cluster.cjs` SHA256 `d380c6f0207f0ac08dec2e08818b8c170eb80f201677d3392636ea5caaaea249`.
- Đã xem normal/combo/Slayer ở320×180,640×360,1280×720,400×300, ảnh lúc chơi không có pause che. Gõ từ thứ5, parry gia hạn, pause/resume, expiry/timeout, restart, Slayer vào/ra tự nhiên và fill thường sau Slayer đạt.0exceptions/provider reads/writes; fixture interception + external blocking giữ nguyên. Normal activation đã quan sát ở pilot; lượt cuối dùng VeryEasy để có đủ thời gian capture live qua bốn kích thước.
- Software43checks FAIL tại tutorial parry timeout; pilot native FAIL do giả định/timing driver được giữ là fail, không tính PASS. Driver sửa theo contact Endless thật, chờ pause/zoom hoàn tất và quan sát mode/timeout thật; gameplay không đổi. Không chạy full Quick/balance/stress/soak/production/release.
- [Gameplay memory](../game/gameplay.md), [verification](../game/verification.md).

## Tài nguyên và bàn giao

- Repo micharel09/TypingHero; base main e20c4ab; origin https://github.com/micharel09/TypingHero.git. Parent C:/Users/ADMIN/orca/TypingHero có WIP ngoài scope, không sửa.
- Child `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/score-hud-cluster`, branch `micharel09/score-hud-cluster` giữ để review/tích hợp. Chưa merge main, push hoặc cập nhật itch.io trong task này. Dọn child/branch sau khi toàn bộ commit được tích hợp đúng đích.
- Đã dọn E:/CodexTemp/TypingHero/score-hud-cluster, hai kết quả server serialize151210/webgl151409 và task-score-hud-cluster sau khi ghi kết luận. Native browser/host của task đã đóng; không đụng shared runner/cache, bản public hoặc tài nguyên task khác.
