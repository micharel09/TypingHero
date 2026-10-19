# Hoàn thiện core Typing Hero trước roguelike

## Acceptance đã chốt
- Một trận skeleton người mới, mục tiêu thắng 60–120 giây. Giữ pixel UI, công thức điểm/damage người chơi, cửa sổ parry, Firebase/name/leaderboard. Player 40 HP, skeleton damage 5, bắt đầu 1000 HP.
- Sai khôi phục cả từ và giữ timer, không trừ HP/combo. Type-ahead cùng luật. Slayer thưởng từng vị trí tối đa một lần cho mỗi lượt từ.
- Một slash chờ sau parry; xóa khi chết/Slayer/rời/restart; pause không phát. Skeleton animation từ 0, một damage/swing, parry/damage cùng impact. Cue vàng/swing trước khoảng 300ms và dừng khi pause.
- Hướng dẫn lần đầu/mở menu/lưu đã xem; Music/SFX 0–100%, mặc định 100%, gain riêng crossfade/duck, lưu trình duyệt.
- Chốt score/combat time/accuracy/parry ngay lethal hit, giữ death animation trước Restart/Main Menu. Accuracy đúng/tổng chữ kể cả gõ lại; không chữ hiển thị —. Menu/hướng dẫn/pause không tính thời gian.
- Quick + WebGL; Edge 61a46c05; embed/fullscreen; 960×540, 1280×720, 1920×1080. Ba lượt input thật ~45 WPM / 95% accuracy / 80% parry. Chọn HP bước 50 đưa median gần 90s. Kiểm tra tham chiếu, chưa thay playtest người mới. Không stress/soak mặc định. Bàn giao source, ZIP QA, checkpoint.

## Implementation
- TypingManager.ProcessInput: original word, reward set theo occurrence, OnInputResult(bool), giữ event cũ. Promotion mang reward history, timeout bỏ từ cũ.
- PlayerController: một pending slash khi pose parry khóa attack.
- SkeletonController/AttackStrike: arm mỗi swing, Animator.Play từ 0, frame sau mới xét kết thúc. Unity sửa clip thật cue .7s / impact 1s / không loop. Parry trước damage, chặn lặp.
- CombatSession sở hữu thống kê, health chốt tại lethal hit.
- CoreMatchBuild reuse factory/panel/bar/button pixel, Unity serialize scene/clip; camera nền xóa ngoài viewport combat.
- AudioPreferences/MusicPlayer/SfxPlayer tách user gain khỏi envelope, lưu PlayerPrefs. Pause giữ stamina/stun/hitstop/typing/queue.

## Checkpoint kết quả — 2026-10-16
- Source hoàn tất; giữ checkout/staging dirty đầu vào, không tạo branch/worktree, commit, push hay publish. Không thêm dependency hoặc đổi Firebase/name/leaderboard contract.
- Skeleton chốt **850 HP**, player40HP, damage5. Unity đã serialize scenes/character prefabs/clip; GUID giữ nguyên, bỏ override clock/debounce đã nghỉ dùng. Camera nền sửa residual menu ngoài combat viewport; cue RectTransform ở(0,1.8,0), size1x1.
- `python tools/game_loop/run.py quick`: **26/26** PlayMode tests đạt, snapshot `4e1dce5e4580dd356fd45273217ef22f4183dfae2804acb7719a6eeb5fc5f08f`. Sau đó chỉ đổi enemy HP bằng editor build và lấy Unity assets về, không đổi runtime C#.
- `python tools/game_loop/run.py webgl`: **54 checks đạt**, snapshot `c0ecf7d502deb7718350596766730c74ffbb925609ccb0a3ee4441a948c45a5f`, 378.594 giây wall clock. Có reset từ/timer, type-ahead, Slayer, parry/queue qua pause, 8 impact5HP, kết quả thắng/thua sau animation, input/stat freeze, restart, audio/help lưu qua menu/reload. Không JavaScript/Unity exception.
- Focus kiểm riêng trên Chromium headed qua Xvfb: **7 checks đạt**, document.hasFocus=false thật, combat/time/stamina giữ nguyên và quay lại vẫn chờ Resume. Snapshot `629df64b8b7a77123a8898b65103658ae1d9f41ff79376d63aac33f28a9a0055`; code focus/pause không đổi sau check này. Headless giữ focus emulation nên không dùng nó làm bằng chứng mất focus.
- Môi trường: Unity2022.3.25f1, TestFramework1.1.33; fly-server Ubuntu26.04, Unity image digest trong toolchain; containers tuần tự 2CPU/8GiB, browser shm1GiB/network none, Chromium153.0.8010.12 + SwiftShader. Probe chỉ đọc; toàn bộ gameplay bằng keyboard/mouse, Firebase fixture và chặn external network. Frame p50~109ms/p95~175ms trong mẫu full-flow có probe; không suy ra FPS máy người chơi.

## Ba lượt tham chiếu cuối ở 850 HP
| Lượt | Combat (s) | WPM | Accuracy | Parry thành công | Điểm cuối |
| --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 86.524 | 44.935 | 95.062% | 12/15 = 80.000% | 2181 |
| 2 | 94.404 | 44.871 | 94.334% | 13/16 = 81.250% | 1685 |
| 3 | 90.706 | 44.716 | 95.266% | 11/14 = 78.571% | 1769 |
- Cả ba thắng, median **90.706s**, nằm60–120s. Đây là kiểm tra cân bằng tham chiếu, **chưa thay playtest người mới**.
- Bắt đầu1000HP; pilot95.399s nhưng bộ ba chưa hợp lệ nên không chốt từ pilot. Ba lượt hợp lệ tiếp ở950HP:100.277/99.093/104.588s, median100.277s (WPM44.862–44.995, accuracy94.149–94.629%, parry75–83.333%).
- Chọn bước50 bằng thời điểm cumulative damage trong ba trace950HP (damage không phụ thuộc HP địch): 750→median82.951s, 800→86.796s, **850→92.364s**, 900→96.958s, 950→100.277s. 850 gần90 nhất; ba trận thực tế850HP ở trên xác nhận lại. Chỉ HP địch thay đổi trong cân bằng.
- Cue clip thật ở.7s, impact1s; Quick xác minh một impact/swing và cùng parry/damage event. Browser950HP median lead~.291s; probe100ms/frame chậm làm lượng tử sai số, không đổi gameplay theo sai số đo.

## Edge/UI đã kiểm
- Đúng Edge instance **61a46c05**, Edge155. Pause/menu/game/result đã inspect ở960×540,1280×720,1920×1080; iframe960×600 và native iframe fullscreen1280×720 (document.fullscreenElement=true), Enter bắt đầu trận và không mở lại hướng dẫn đã xem.
- Native queue: hoàn thành từ4chữ trong parry→pending=true; pause giữ combatSeconds=.848885 và HP địch1000; resume→pending=false, HP990 đúng một slash. Native defeat8đòn:25.555s, SCORE0/ACCURACY—/PARRIES0, panel sau animation, errors0. Cue vàng đã inspect trên đầu skeleton, trong combat viewport.
- QA5 runtime4e1dce…: first-help/Continue/40HP/950HP/pause đạt, errors0. Trước chơi: serviceWorker.controller=null, registrations0, Firebase fetch trả fixture{} và mock rule hit1/failures0. Các layout/input owner không đổi khi chốt850HP.

## Lệch đã sửa và bằng chứng
- Attack Update đọc Animator state cũ ngay sau Play và EndAttack cùng frame: return sau StartAttack, regression clip thật đạt.
- Production chưa có owner PushParryPose dù test dùng: ParrySystem nay execution order-50, latch ngay request, giữ gate theo pose/success, pop khi thoát/disable. Không nới window0–.25s/grace.02. Test real Animator+ProcessInput và native queue đạt.
- Clip bị hit ngắt có thể bỏ qua event close: đóng window khi hit/dead/rời pose; chặn late open. Quick interrupted-window đạt.
- Stun coroutine tiếp tục khi Slayer disable component, sau lethal hit CrossFade idle làm mất death cleanup/panel: đăng ký OnDeathStarted từ Awake đến OnDestroy, stop coroutine mà không unlock/idle. Quick real death clip + victory overlay đạt; sáu lượt thắng950/850 đều có panel.
- Audio fade bị LateUpdate ghi đè và restore duck trong fade-in giữ gain thấp: tách raw/user/scene gain, đọc duck target hiện hành; Quick zero/crossfade/duck/disable binder đạt.
- Driver chuột quá nhanh/TMP nhập tên chưa kịp frame, hotkeyR/M trong pause, phase snapshot trễ: sửa timing/input và bù tuổi snapshot bằng quan sát read-only, không sửa game để giúp driver. Các lượt parry46.7/66.7% bị loại, không dùng chọn HP.

## Sự cố fixture đã khôi phục
- PWA Service Worker bypass page-level BrowserSkill interception ở origin QA cũ. Hai record CoreQA/0 tạo ngoài ý muốn: `ed79dd2304924cda87e3d9a19475683f` lúc1792140739793 và `1a412064df92460bab78fe2454db1612` lúc1792142604176.
- Đã đối chiếu name/score/time và GET-null trước create (capture1a412), backup tạm, conditional DELETE bằng ETag đúng từng record, GET-null lại; query bestScore0/CoreQA trong task cho0 record còn lại. Không sửa schema/rules/dữ liệu người chơi khác. Conditional REST theo https://firebase.google.com/docs/database/rest/save-data#section-conditional-requests .
- QA host sau đó bỏ riêng dòng đăng ký SW trong HTML được serve; artifact/template production không đổi. Native PATCH200 đã chứng minh mock intervention applied/hit1; không submit thật tiếp. Năm QA origins8167/8171/8173/8175/8177 đã CacheAPI rỗng và registrations0; đóng sessions và dừng đúng năm Python hosts. Giữ shared bsk daemon.

## Production và bàn giao
- `python tools/game_loop/run.py production`: **đạt**, snapshot `61ba8e37bdda95125cb1d376a198b00a2d5c612e9111f6dd960f4582b1347e14`, runtime/HP giống QA cuối; source snapshot khác do lấy Unity assets về và chuẩn hóa trailing whitespace. qa=false, mọi JS không có typingHeroQA; final Unity assets normalize so khớp checkout.
- ZIP `C:/Users/ADMIN/Downloads/TypingHero-WebGL-core-match-20261016.zip`: 15,847,525 bytes,22files, CRC **đạt**, rootindex + CREDITS/Crusenho/Kenney/VT323licenses; không có QA cleanup HTML/bridge. SHA256 `a5720eabf6a5650dd2c8297275b8948f33399ad6ceb25d6165691c344b9c7c5c`. Không chạy stress/soak; không deploy/publish.
- Production Edge61a46c05 smoke **đạt**: bridge undefined, canvas1280×720, first-help→Continue→pause/resume→GAME OVER25.5s/SCORE0/ACCURACY—/PARRIES0→Restart HP đầy/score0→Main Menu giữ tên. Trước chơi fixture{} hit1, SWnull/registrations0; PATCH200 intervention mock applied. Không runtime exception; favicon.ico404 chỉ phát sinh khi chuyển trang cleanup QA. Không đổi source sau checks runtime. Diff/whitespace, new-script metas và final Unity serialization khớp đã kiểm đạt.
- **Đã dọn** toàn bộ capture/temp local của task và35 thư mục output QA/build/result trên server sau khi ghi receipts. CacheAPI sáu QA origins rỗng/registrations0; sessions đóng, sáu Python hosts dừng. Shared bsk daemon và cache/license/Library/toolchain server giữ dùng lại. ZIP bàn giao nằm ngoài temp. Không còn blocker/acceptance chưa đạt trong phạm vi; playtest người mới và stress/soak chưa thực hiện như đã giới hạn.
