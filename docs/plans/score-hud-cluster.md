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


## Combat HUD feedback — hoàn tất local / 20-10-2026

- Đủ bốn mục trên base b260470: gộp difficulty/DUEL, Space theo trạng thái parry thật, icon shield/warning và GUARD BREAK, HP thấp/hit flash. Giữ score → multiplier → decay, vị trí HP/tray, damage/input/SFX và success iframe80ms.
- CombatR7Status snapshot `DIFFICULTY · DUEL n/6` hoặc TUTORIAL, giữ khi Slayer và ẩn khi InputBlocked/results. EndlessOverlay chỉ sở hữu intro/Continue, giữ handler/prompt.
- CombatHUDFeedback sở hữu tint keycap, số/viền HP, icon stamina và notice. Ready dùng cùng predicate nội bộ với TryBeginParry; cue từ GameObject thật, window từ IsWindowActive, success từ OnParrySuccess120ms. Khóa giảm sáng40%, cue/window DCC18A, success588B8C.
- HP sống≤25% đổi số F16B67; OnDamaged chỉ phát khi TakeDamage thật giảm HP. Viền Lerp về màu gốc180ms, hit mới reset; bỏ trừ frame vừa nhận event. User bỏ yêu cầu reduced-motion riêng sau QA: đã xóa hai conditional mới, luôn dùng normal Lerp/fade đã kiểm. Motion cũ ngoài HUD không đổi.
- Hai PNG v2 `icon-{shield,warning}.png` copy nguyên byte vào Assets/Art/UI/CombatR7,10×10, Point/Clamp/no-mips/uncompressed/border0. Shield SHA25616f18723e96b0594deeb5be2e1327a4bc2a12e935f28426b7ed5dd63754faa77; warning7b48a6153de5884e3ff2cd16e89263c9fa9dd757107698faf6d4d69dad145cc9.
- OnDepleted nội bộ chỉ phát từ cost thật>0→0; HUD xác nhận enemy sống và onStunStart cùng frame. Setup/regen/death không báo. Warning tới hết stun; GUARD BREAK căn giữa102native, font8native, hold720/fade180ms, một lần/duel. Pause giữ timers; results/death clear; disable hủy event/reset; Continue/restart tạo lại.

### Kiểm chứng

- Unity2022.3.25f1/fly-server QA022355 compile/build/strict repeat PASS, input fb80c4104397ce860097599777f60ba6c81020895d67a5da802c7782546a250a. Scene/CombatR7 bằng checkout;853/853 ID cũ, scripts và persistent listeners giữ nguyên,858 ID cuối. Asset bytes/import/bindings và source/docs whitespace PASS; Unity blank serialized fields giữ chuẩn.
- Scoped HUDFeedback4/4 PASS022316 cùng input; test bổ sung gate/cue locked/window/pose, iframe80ms và Tutorial row4/4 PASS022957, input a4282a59ec4fe432ba8d2e20277b1448de23595ca23bc9b31627f17417bbfc7a. Kiểm HP11/10/9, hit liên tiếp, pause/restore, setup0+stun không báo, cost+stun thật, lần break thứ hai, icon restore và disable/re-enable.
- Native Edge155.0.4283.62/GTX1650Ti ANGLE D3D11/Playwright1.58.2:2353/2353 PASS,169.701s. Năm difficulty, real Space ready/lock/cue/window/success/pause, Normal6duels gồm elite/boss giữ10HP tới victory(2961points/8parries), Continue/reset, restart/death. Normal/Slayer và các reduced-only case lúc đó đều xem ở320×180,640×360,1280×720,400×300.0exceptions;6fixture reads/2intercepted writes,0production writes, external blocking bật. Overlay F6 cũ chỉ có trong QA capture.
- Native receipt dùng driver26f85e88d086c28b03475550465769f46dff316da5fd814513c7fee983bca98f; sau user bỏ reduced-motion, driver hiện tại21fca2980d324c446c965043ef82c3914df1c4f125a26b0eb984fa0fafccb995 (syntax PASS). Shared Endless driver40fdea83afff6d75d9a631463096656e754b970f1543806dc0858dd7a689ac23.
- Build bằng run.py webgl --browser-scenario score-cluster; native chọn TYPINGHERO_NATIVE=1/TYPINGHERO_HUD_FEEDBACK_ONLY=1 và immutable player/runner/results roots. Scoped Unity chọn testCategory HUDFeedback bằng helper tạm, không đổi runner public.

### Giới hạn và tài nguyên

- Software score-cluster của ba candidates dừng53checks ở tutorial parry/six retries, không tính PASS. Native pilots đầu fail vì snapshot cũ/resume tween chưa xong; driver cuối kiểm tuổi snapshot và đợi ResumeButton biến mất. Offline đầu3/4 fail vì flash mất thời gian frame load; timer fix có4/4 regression PASS.
- Một native candidate qua6duels rồi fail vì không thu được recovery-clock sample của balance gate. HUD mode không yêu cầu đo balance; Endless/balance chuẩn vẫn giữ assert. Không chạy full Quick/balance/stress/soak/performance hoặc production/release trong giai đoạn triển khai local.
- Sau khi bỏ hai conditional reduced-motion, normal path giữ nguyên biểu thức đã compile/kiểm ở trên. Không tuyên bố build/test lại source sau thao tác xóa nhánh. Lượt Unity bổ sung hủy an toàn khi còn chờ shared lock; không tạo result mới.
- Đã dọn task temp và8result server020307serialize,020550/021619/022355webgl,021356/022241/022316/022957quick. Giữ shared source/cache/builds. Giữ score-hud-cluster child/branch để review/tích hợp; chưa merge/push main hoặc cập nhật itch.io ở giai đoạn local. Giai đoạn publish mới được ghi dưới đây.

## Itch.io HUD release / 2026-10-20

User authorized publishing the completed HUD to itch.io. Release child: `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/hud-itch-release`, branch `micharel09/hud-itch-release`, parent original TypingHero/main (dirty WIP preserved), origin https://github.com/micharel09/TypingHero.git.

Restored the exact retained approved public palette source archive, verified complete input SHA256 `64de5b3ad3a3df3bc79ff56c159713ba777cd1ab25ffa0f08a81834ba7d6fcd7`, as local98465d8; cherry-picked HUD commits b260470/751de45 as a9add62/f66c090. This baseline is the published UI code/branding plus the26 approved player palette PNGs; unpublished parent font/marketing work is excluded. No changes to main or parent WIP.

Authenticated itch.io game3857509 editor is available in browser-skill Edge session wzuv. Initial current playable is19891356; previous palette19891315 remains listed. Backed up38 non-upload business controls and16 upload rows without hidden/auth fields under task temp. Preserve existing page/gallery/pricing and retain old playable for rollback. Target https://micharel09.itch.io/clackblade.

Temporary artifacts: E:/CodexTemp/TypingHero/hud-release. Serialization is queued on the shared Unity runner; no upload performed yet. Planned verification: strict repeat/import/bindings on integrated source, production build/QA exclusion and ZIP CRC/hashes, native original-HTML menu/combat/HUD/pause smoke with production network blocked, then upload/Save and anonymous CDN hash/public render checks. Reuse prior detailed HUD checks; no full balance/stress/reduced-motion work.
