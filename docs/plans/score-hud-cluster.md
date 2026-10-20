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


## Combat HUD feedback ? ho?n t?t local / 20-10-2026

### Thay ??i v? mapping

- Base b260470, reuse child/branch ? tr?n; WIP parent gi? nguy?n. ?? b?n m?c user ch?t, gi? score ? multiplier ? decay, v? tr? HP/typing tray, VT323 v? palette v2/R7.
- `CombatR7Status` snapshot run difficulty/slot khi Start: `DIFFICULTY ? DUEL n/6` ho?c `TUTORIAL`; ?n khi InputBlocked/results. Slayer kh?ng ??i d?ng n?y. `EndlessOverlay` ch? hi?n heading trong intro/Continue, gi? nguy?n handler/prompt.
- `CombatHUDFeedback` l? n?i duy nh?t ghi tint Space, s?/vi?n HP, icon stamina v? GUARD BREAK. Ready d?ng c?ng predicate n?i b? v?i TryBeginParry; cue ??c GameObject th?t; window ??c IsWindowActive. Kh?a gi?m s?ng40%; cue/window DCC18A; OnParrySuccess t?588B8C trong120ms. Success iframe serialized v?n80ms; kh?ng s?a window/grace/input/SFX.
- HP s?ng?25% ??i s? sang F16B67. OnDamaged n?i b? ch? ph?t khi TakeDamage th?t s? gi?m HP; kh?ng ph?t khi init/restore. Vi?n d?ng Color.Lerp v? m?u g?c180ms; hit m?i reset timer, kh?ng coroutine. Timer b? tr? frame v?a nh?n event ?? kh?ng m?t flash khi frame tr??c d?i. User20/10 x?c nh?n kh?ng c?n reduced-motion: ?? b? nh?nh x? l? ri?ng m?i th?m; HP lu?n d?ng Lerp180ms v? notice lu?n fade180ms theo normal path ?? ki?m.
- `design/ui-kit/typinghero/v2/png/icon-{shield,warning}.png` ? `Assets/Art/UI/CombatR7/`, byte nguy?n b?n10?10, Point/Clamp/no-mips/uncompressed/border0. Shield SHA256 `16f18723e96b0594deeb5be2e1327a4bc2a12e935f28426b7ed5dd63754faa77`; warning `7b48a6153de5884e3ff2cd16e89263c9fa9dd757107698faf6d4d69dad145cc9`.
- OnDepleted n?i b? ch? ph?t t? cost th?t>0?0; PreparePractice/regen kh?ng ph?t. HUD c?n ki?m enemy s?ng v? onStunStart th?t c?ng frame. Warning gi? t?i h?t stun; GUARD BREAK c?n gi?a102native d??i thanh, font8native, hold720/fade180ms, m?t l?n m?i duel. Pause gi? timers; results/death x?a. Disable h?y ??ng k?/reset; Continue/restart t?o l?i tr?ng th?i.

### Ki?m ch?ng cu?i

- Unity2022.3.25f1/fly-server: QA022355 compile/build/strict repeated-builder PASS, frozen input `fb80c4104397ce860097599777f60ba6c81020895d67a5da802c7782546a250a`. Scene v? to?n b? CombatR7 serialized output b?ng checkout;853/853 ID c?, script v? persistent listener blocks gi? nguy?n,858 ID cu?i. Kh?ng component/object/listener tr?ng; hai PNG kh?p source/import settings. Unity empty serialized fields gi? whitespace chu?n; source/docs diff whitespace PASS.
- Scoped PlayMode category HUDFeedback:4/4 PASS tr?n c?ng input022316; b?n test b? sung parry gate/cue locked/window/pose, iframe80ms v? Tutorial row ti?p t?c4/4 PASS t?i022957, input `a4282a59ec4fe432ba8d2e20277b1448de23595ca23bc9b31627f17417bbfc7a`. C? health11/10/9, hit li?n ti?p, pause, restore, setup0+stun kh?ng b?o, cost+stun th?t, refill/icon restore, notice l?n hai v? disable/re-enable. Sau QA b? sung tests/driver/docs; sau ?? user b? reduced-motion, ch? x?a hai nh?nh BrowserMotion.Reduced trong feedback, gi? nguy?n normal path ?? ki?m v? serialized product.
- Final native Edge155.0.4283.62 / GTX1650Ti ANGLE D3D11 / Playwright1.58.2:2353/2353 PASS,169.701s. N?m difficulty qua menu; Normal finite6duels g?m elite/boss, gi?10HP qua Continue t?i victory(2961points/8parries). Real Space ch?ng minh ready/hit-pose lock/cue/window/success, pause gi?a hit/success/notice; Continue reset kh?ng flash do carry HP; restart/death clear. Reduced motion: hit m?u t?c th?i, b?n parry th?t ph? guard, notice kh?ng fade, restart v? death th?t.
- L??t QA tr??c quy?t ??nh b? reduced-motion ?? xem normal, Slayer v? reduced motion (c? reduced-Slayer) ?320?180,640?360,1280?720,400?300. D?ng difficulty ?n ??nh/kh?ng l?p heading; GUARD BREAK kh?ng ?? thanh, icon gi? pixel v? keycap kh?ng che tray. Overlay F6 c?a QA c? v?n c? trong capture; kh?ng thu?c thay ??i n?y.
-0JS/managed exceptions;6Endless fixture reads/2writes b? intercept,0Classic writes/0production writes, external blocking b?t. Driver `combat_hud_feedback.cjs` SHA256 `26f85e88d086c28b03475550465769f46dff316da5fd814513c7fee983bca98f`; shared Endless driver `40fdea83afff6d75d9a631463096656e754b970f1543806dc0858dd7a689ac23`.
- Build: `python tools/game_loop/run.py webgl --browser-scenario score-cluster --wait-seconds 3600`; native d?ng browser.cjs v?i `TYPINGHERO_NATIVE=1`, `TYPINGHERO_HUD_FEEDBACK_ONLY=1`, immutable player v? runner/results roots. Scoped Unity d?ng runner chu?n v?i `-runTests -testPlatform PlayMode -testCategory HUDFeedback`; helper ch?n category ch? n?m trong artifact t?m, kh?ng thay runner public.

### Gi?i h?n v? c?c l??t kh?ng ??t

- Software score-cluster c?a c? ba QA candidates d?ng53checks ? tutorial parry/six retries; kh?ng tuy?n b? software profile PASS. Nghi?m thu HUD d?ng native th?t v? scoped Unity ? tr?n. Kh?ng ch?y full Quick, balance matrix, stress/soak, production build, release ho?c Firebase deployment; kh?ng c? k?t lu?n performance hay thay ??i gameplay balance.
- Native pilots ??u fail do tu?i snapshot/??i resume tween ch?a ??; driver cu?i ki?m tu?i snapshot v? ??i ResumeButton bi?n m?t. Offline ??u3/4 fail v? flash b? tr? frame load d?i; fix timer c?4/4 regression PASS. M?t native candidate qua6duels r?i fail v? kh?ng thu ???c recovery-clock sample c?a balance gate; HUD mode b? y?u c?u ?o balance ??, ch? ?? Endless/balance chu?n v?n gi? nguy?n assert. Kh?ng gi?m acceptance HUD ho?c s?a combat ?? ph?c v? driver.
- Gi? child `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/score-hud-cluster`, branch `micharel09/score-hud-cluster` ?? review/t?ch h?p; d?n sau khi t?ch h?p ??y ?? ??ng ??ch. Kh?ng merge/push main ho?c c?p nh?t itch.io. Capture/build QA t?m kh?ng b?n giao; k?t qu? ???c ghi b?ng ch? t?i ??y.

- Quy?t ??nh user sau QA: kh?ng c?n reduced-motion ri?ng. ?? b? hai nh?nh BrowserMotion.Reduced kh?i CombatHUDFeedback v? c?c b??c QA d?nh ri?ng cho n? kh?i driver. Kh?ng s?a h? tr? motion c? c?a typing/UI kh?c. Native receipt2353 ? tr?n v?n ch?ng minh normal path kh?ng ??i; kh?ng ch?y l?i build WebGL ch? ?? x?a nh?nh ngo?i scope. Normal path gi? nguy?n ch?nh c?c bi?u th?c ?? compile/ki?m trong Unity4/4 v? native2353; kh?ng tuy?n b? ?? build l?i source sau khi x?a conditional. L??t Unity b? sung b? h?y an to?n tr??c khi ch?y (helper c?n ch? shared lock), kh?ng t?o result m?i.
- ?? d?n artifact QA local v?8k?t qu? server ban ??u(020307serialize,020550/021619/022355webgl,021356/022241/022316/022957quick), gi? shared source/cache/builds. Helper c?a l??t check b? sung ?? h?y v? ???c d?n c?ng task temp; kh?ng ??ng job ?ang ch?y kh?c.

- Driver hi?n t?i ?? b? reduced-only cases, SHA256 `21fca2980d324c446c965043ef82c3914df1c4f125a26b0eb984fa0fafccb995`; syntax PASS. Hash26f85e88 ? native receipt l? driver ?? ch?y tr??c quy?t ??nh scope cu?i, kh?ng ph?i claim ch?y l?i driver m?i.
