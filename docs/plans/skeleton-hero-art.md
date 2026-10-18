# Skeleton lấy cảm hứng Shogun Showdown: art và chuyển động

## Checkpoint trọn đòn rìu và recovery — 2026-10-18

- Phạm vi đã triển khai: chỉ mẫu thiết kế. Dựng lại cel1–14 bằng landmark vai,
  khuỷu, cổ tay, ngực, hông và gối; bỏ solver hai đoạn tay12px. Thorax theo trục
  ngực–hông thay vì chuyển tức thời sang một hình gập riêng, trở về tạo hình
  Idle ở cuối recovery. Giữ reference hàng hai, hình đầu rìu dùng chung, canvas96×64,
  chân neo, cán32px/grip7–15px và cel0–9 timing gốc/contact1s.
- Cel10 còn ghim đất và chuyển hông về; cel11 tách lưỡi; cel12 kéo gần thân;
  cel13 dựng người; cel14 giảm tốc; cel15 trùng RGBA Idle0. Recovery bắt đầu
  1.3333334s, duration100/120/140/140/120/100ms; tổng2.0533334s (native2053ms).
  Native rìu17frame, Attack tag1–16, hai lớp Body/VFX. Thương vẫn11frame.
- Đã sửa exporter, renderer, preview và browser checks. Timeline riêng cho rìu;
  kiếm/thương giữ cel cuối trong recovery. Có nút ghim đất/hồi thế/về Idle;
  scrub, step, playback dùng số cel thực tế. Overlay thêm trục cán, đường lưỡi,
  vai–khuỷu–tay và đầu–ngực–hông. Smear chỉ cel4, dựng lại theo đường lưỡi mới.
- Vòng sửa hình: lần đầu né cán bằng cách dịch sọ ra sau khiến cổ dài ở cel2–3;
  đã thay bằng nâng cán và hai tay qua sọ, giữ đầu gần ngực. Đường quét mới có
  mẫu trung gian chạm y0; sửa đường hạ tay trong sweep (không clamp/hạ check).
  Cel14→15 từng đổi kiểu ribcage rõ; đưa cel13–14 về patch ribcage Idle để nối.
- Export LibreSprite PASS28PNG, alpha/bounds/chân/cán/grip/timing/contact PASS;
  roundtrip riêng Body, VFX và composite exact. Tag Attack bao gồm recovery;
  toàn đầu rìu cố định cel5–10, không VFX recovery; Idle0/Attack0 giữ hash.
  1132 file được guard trong lần export cuối, gồm Assets, complete, cluster, thương.
- Browser Edge headless lần cuối: **72 checks PASS**, không page/console error;
  1280×720/640×360 và nearest4×/8×. Scrub giải đúng cả16cel; timeline riêng,
  giữ cuối/replay/pause/resume và giữ cel cuối kiếm/thương PASS. Tám lượt phát
  đầy đủ (1×/0.25× × mirror bật/tắt × VFX bật/tắt) đều ghi nhận cel0–15, không
  bỏ cel trong lần cuối; đo tốc độ1.00019× và0.24411×. Các lượt trước đôi lúc
  không ghi nhận một cel; nguyên nhân stall chưa xác định. Kiểm mới quan sát
  trực tiếp render, ghi rõ cel bị bỏ nếu có, tách khỏi kiểm resolve keyframe;
  không thay timing để ép browser hiển thị. Không coi kết quả này là duyệt lực.
  Python AST, JS syntax và git diff --check trong phạm vi task PASS.
- Đã soi16cel không VFX và mirror, cặp chuyển tiếp3→4→5,9→10→11,
  14→15→Idle; xem key pose Body/VFX/overlay/mirror từ canvas browser,
  và contact ở1280×720/640×360. Hông thu về trước khi rìu rời đất, ngực nâng
  dần trong recovery; không thấy đứt grip hoặc nhảy đầu rìu trong key pose.
  Đây là đánh giá hình và chuỗi cel; **quan sát playback liên tục bằng mắt chưa
  kiểm chứng**. Không suy kiểm tốc độ/thu thập frame tự động thành duyệt chuyển động.
- Phản hồi tay/thân và cảm giác lực **chưa được user xác nhận đạt**; mẫu đầy đủ
  tiếp tục chờ đánh giá cảm giác. Không tích hợp runtime, không Unity/WebGL/API;
  giữ nguyên các thay đổi checkout của công việc khác. Tool LibreSprite portable
  cũ được giữ cho lần sửa tiếp theo; capture/build QA của lượt này dọn sau kiểm tra.

## Lưu hướng dẫn animation cận chiến — 2026-10-18

Trạng thái hiện hành: **mẫu rìu hiện tại chưa đạt theo phản hồi user** và vẫn chờ
sửa. Các kết quả kiểm kỹ thuật và nhận xét của mốc trước được giữ nguyên bên dưới;
alpha/bounds/roundtrip và preview PASS không đồng nghĩa animation đã được duyệt.

- Đã lưu skill dùng chung
  [pixel-melee-animation](E:/codex-data/skills/pixel-melee-animation/SKILL.md),
  nguồn chính [SLYNYRD — Melee Attacks](https://www.slynyrd.com/blog/2018/9/8/pixelblog-9-melee-attacks),
  và nối định tuyến trong [AGENTS.md](../../AGENTS.md).
- Skill hướng dẫn chọn reference đúng đòn/số tay/góc nhìn, kiểm key pose và grip
  trước cel hoàn thiện, phân biệt cầm cố định với trượt cán, tách kiểm kỹ thuật
  khỏi đánh giá chuyển động. Giữ contract project; không lấy canvas/số frame/timing
  của TypingHero làm quy định chung.
- Kiểm chứng hướng dẫn: validator `skill-creator/scripts/quick_validate.py` trả
  `Skill is valid!`; frontmatter/YAML, `allow_implicit_invocation: true`, path/link
  và whitespace PASS. Đã review phạm vi kích hoạt, loại trừ UI motion và chỉnh màu
  sprite tĩnh; nội dung plan cũ giữ nguyên byte. Không chạy Unity/WebGL ở lượt này.
- Phạm vi lượt này chỉ lưu hướng dẫn; không sửa asset rìu, runtime hoặc gameplay.
  Bước tiếp của task art: sửa mẫu rìu theo phản hồi và đối chiếu key pose, cách cầm,
  playback, từng cel, mirror, không VFX và kích thước chơi trước khi kết luận đạt.

## Rìu theo hình user và kết thúc bổ xuống đất — 2026-10-18

Yêu cầu mới thay phần tương ứng của mốc trước: lưỡi theo hình user gửi và cel
cuối Attack chạm đất để có sức nặng. Lưu nguồn duyệt tại
`design/skeleton-hero/references/axe-blade-user.png`: mặt thép rộng, vai lưỡi
xòe, mép cong dài và khoảng lõm sâu dưới cổ. Giữ thương, hero, kiếm, canvas96×64,
Idle0 + Attack10, shaft32px, mốc impact1s và clip1.3333334s.

- Đã vẽ lại đầu rìu từng cel trong LibreSprite bằng `prepare_weapons.py` và
  `weapons-pixel.js`; không xoay bitmap. Giữ nguyên thân/tay/cán ở cel0–4.
- Attack5 hạ lưỡi xuống y63 tại impact;6–7 nén đầu/ngực/chậu và giữ tiếp xúc;
  8–9 ổn định thấp, kết thúc vẫn chạm đất. Hai tay tính theo cùng trục cán32px.
  VFX rìu riêng kéo cung bổ tới điểm tiếp xúc; không thêm VFX che pose cuối.
  **Yêu cầu cũ Attack9=Idle0 được thay bằng kết thúc chạm đất cho rìu**;
  thương vẫn Attack9=Idle0. Exporter kiểm chân cố định và tiếp xúc lưỡi thật
  ở5–9, không bỏ kiểm bounds/alpha/roundtrip để chấp nhận pose mới.
- Preview thêm nút `Cel cuối · chạm đất` chọn đúng Attack9, đồng thời cập nhật
  mô tả kết thúc. Đã xem tất cả Attack ở4×, mirror và không VFX, soi đầu rìu ở8×;
  đầu lưỡi rộng hơn, vai nhô khỏi cổ và mặt cắt lõm về cán theo hình user.
- Export cuối PASS: 22 PNG 96×64, hai native 11 cel với tags/lớp tách riêng;
  alpha0/255, feet cố định, không cắt vũ khí/VFX, RGBA native/body-layer tái xuất
  trùng PNG. Lưỡi rìu thực chạm y63 ở5–9, cả khi tắt VFX; body8=body9 để giữ
  pose cuối ổn định. `verification.json` ghi `groundContactFrames=[5,6,7,8,9]`.
- Audit so trước sửa PASS: 1101 file Assets/complete/cluster-milestone giữ hash;
  cả12 file PNG/native thương giữ nguyên byte, dữ liệu pose thương và ảnh
  hero/kiếm giữ nguyên. Timing float/native không đổi; thân/khớp/cán/tay0–4
  giữ nguyên; cán32px, hai tay thẳng trục, đầu lưỡi không xuyên sọ ở đủ10pose.
  Bản reference trong project trùng byte hình user cung cấp.
- Preview **29/29 PASS**, gồm hai kiểm mới chọn đúng cel9 và pixel tiếp xúc đất
  thật ở cả5–9 không VFX; console/pageerror rỗng. Đủ1280×720,640×360,4×/8×,
  mirror/onion/VFX/scrub/step/pause. Lấy mẫu tốc độ~520ms:1× đo.997514×,
  .25× đo.247432×, giữ ngưỡng sai số8% hiện có; không coi đây là đo frame pacing.
- Phát cả vòng không VFX bằng Playwright/Edge: thấy đủ10cel ở4×/1× và
  8×/.25× mirror, nearest sampling, không lỗi browser. Đã xem Idle/lấy đà/impact/
  cel cuối ở hai kích thước, cả mirror: đầu/ngực/chậu hạ sau impact rồi ổn định,
  rìu giữ tiếp xúc đất tới cuối; tay vẫn bám cán. Preview phát lặp Attack theo
  cơ chế có sẵn; rìu kết thúc chạm đất theo yêu cầu mới, không còn khớp Idle0.
- JS syntax, Python compile, whitespace/diff review và link/path PASS. Không thêm
  dependency. Chỉ sửa mẫu thiết kế, chưa tích hợp runtime/gameplay; không chạy
  Unity/WebGL cho mốc này. Hai mẫu và nút xem cel cuối nằm trong preview hiện có.
- Đã dọn bản trước, audit/capture QA tạm và scratch exporter sau khi ghi kết quả;
  giữ source/native/PNG/reference trong project, LibreSprite portable dùng lại.

## Sửa tạo hình rìu / thương — 2026-10-18

Yêu cầu hiện hành: giữ animation hai mẫu đã chốt; chỉ sửa đầu vũ khí từng cel.
Rìu một lưỡi: cổ hẹp, mặt thép xòe một phía, mép cong và phần lưỡi kéo xuống
tạo khoảng lõm với cán. Thương mũi lá dài/hẹp, mũi nhọn và cổ kim loại liên tục.
Giữ canvas96×64, Idle0 + Attack10 mỗi loại, VFX riêng; không đổi hero/kiếm/gameplay.
Nguồn nghiên cứu đã chốt: [karsiori](https://karsiori.itch.io/pixel-art-warrior-with-axe),
[Royal Spearman](https://elthen.itch.io/2d-pixel-art-royal-spearman-sprites),
[Frogfolk Spearman](https://elthen.itch.io/2d-pixel-art-frogfolk-spearman-sprites),
[Viking Axe](https://sanctumpixel.itch.io/viking-axe-pixel-art-character).

- Đã sửa Idle0, Attack3 và5 rồi so cạnh bản trước khi tắt VFX. Bản rìu đầu
  còn mảnh như lưỡi liềm; đã mở rộng mặt thép và phần vai, giữ khoảng lõm ở gót.
  Sau kiểm ba pose, áp dụng đủ22cel trong `prepare_weapons.py` / `weapons-pixel.js`.
  Mọi polygon được đặt trong tọa độ cel và vẽ ở native resolution bằng LibreSprite;
  không xoay bitmap. Mép rìu giữ phía dẫn của cung bổ; đầu thương theo trục cán.
- Đã sửa thêm gót rìu impact 1 pixel để giữ chiều dài ổn định qua góc raster;
  viền thương dùng màu thép sáng hơn để đọc được trên nền, bỏ pixel bóng cán
  thừa sau mũi nhọn. Đã xem lại đủ 22 cel ở 4×/8×, gồm mirror và không VFX;
  mép rìu theo chiều bổ, thương thẳng cán, hai tay bám cán, không thấy xuyên sọ.
- Export cuối PASS: 22 PNG 96×64, hai native 11 cel có tags Idle/Attack và VFX
  riêng; alpha 0/255, bounds không cắt vũ khí/VFX, chân y63 cố định, Attack9
  nối Idle0. Native tái xuất RGBA trùng PNG và lớp thân/vũ khí trùng từng cel.
  Guard 1101 file Assets/complete/cluster-milestone trùng trước sửa.
- Audit riêng so bản trước: mọi tọa độ thân/khớp/hai tay/cán, pose lean, timeline
  float và duration native giữ nguyên; ảnh hero/kiếm và VFX cả hai native trùng
  byte RGBA. Tất cả pixel khác chỉ nằm trong vùng đầu vũ khí/cổ lắp cán. Cụm
  đầu vũ khí có kích thước ổn định theo trục cán trong sai số raster; đầu thương
  dài khoảng 14 px, rộng khoảng 4–5 px. Không sửa runtime hoặc gameplay.
- `verify_weapons.cjs` cuối **27/27 PASS**, console/pageerror rỗng; đủ 1280×720,
  640×360, 4×/8×, mirror, VFX/onion, scrub/step, telegraph/impact và pause.
  Playback lấy mẫu khoảng 528 ms: 1× đo .975961×, .25× đo .247064×; giữ ngưỡng
  sai số tương đối 8% hiện có. Đây không phải đo frame pacing.
- Kiểm toàn vòng bằng Playwright/Edge đã có: cả 10 cel xuất hiện khi phát 4×/1×
  bình thường và 8×/.25× mirror, qua điểm nối Idle; không VFX, nearest sampling,
  không lỗi browser. Đã xem Idle/lấy đà/impact ở hai kích thước, cả mirror; rìu
  có cổ hẹp và lưỡi cong lệch một phía, thương gọn/nhọn và tách được thép/cán.
  Nhận xét trực quan này không thay user duyệt tạo hình.
- JS syntax, Python compile và whitespace các file sửa PASS; không thêm dependency.
  Unity/WebGL không chạy vì mốc này chỉ sửa nguồn thiết kế, chưa tích hợp gameplay.
  Hai mẫu cùng lúc tại `design/skeleton-hero/weapon-milestone/review.html`; native,
  PNG, `poses.json`, `verification.json` và README đã cập nhật. Chờ user duyệt
  tạo hình; giữ animation đã chốt, không mở rộng sang các state hay enemy khác.
- Đã dọn bản trước/capture/audit tạm của lượt sửa và scratch exporter; giữ nguồn
  thiết kế trong project cùng LibreSprite portable dùng lại theo checkpoint cũ.

## Mốc mới — mẫu rìu / thương (2026-10-18)

Phạm vi đã chốt: hai enemy, mỗi loại Idle0 + 10 cel Attack, canvas96×64,
native LibreSprite có timeline và VFX riêng. Chỉ design, chưa tích hợp gameplay.
Giữ nguyên hero và bộ kiếm đã duyệt; không chạy lại Unity/WebGL ở mốc này.

### Áp dụng giáo trình trước khi dựng cel

Nguồn Raymond Schlitter: [Intro to Animation](https://www.slynyrd.com/blog/2018/8/19/pixelblog-8-intro-to-animation)
và [Melee Attacks](https://www.slynyrd.com/blog/2018/9/8/pixelblog-9-melee-attacks).
Đã đọc bài và xem bảng pose `17-Sword_Attacks.gif` (stance, anticipation,
smear, hit, follow-through, recover, overshoot); không đưa hình tutorial vào deliverable.

| Nguyên tắc | Pose minh họa / nguồn | Cách áp dụng và kiểm mẫu |
|---|---|---|
| Key pose phải tự kể được động tác | Ví dụ wind-up / extension trong bài8 | So Idle, Attack3 và5 không VFX trước chuyển tiếp |
| Cụm pixel ổn định | Idle/run trong bài8 | Tái dùng sọ đã duyệt, soi sáng/tối4×/8× từng cel |
| Stance theo trọng lượng vũ khí | Stance hai tay và anticipation trong bảng kiếm bài9 | Rìu giơ quá đầu; thương thu cán ngang thân, cả hai tay trên cán |
| Smear ngắn, theo quỹ đạo | Smear1 và hit trong bảng kiếm bài9 | Rìu cung bổ xuống, thương vệt thẳng; lớp VFX riêng, tắt để kiểm pose |
| Follow-through và hồi thế có lực | Follow-through/recover/overshoot bài9 | Rìu chìm chậu sau bổ; thương chuyển trọng tâm rồi rút; frame9 khớp Idle |

Đối chiếu từng frame kiếm đã duyệt: 0 guard,1 nhấc kiếm,2 cuộn ra sau,
3 giữ lấy đà,4 nén cuối,5 burst ngang,6 follow-through,7 duỗi cuối,
8 thu kiếm,9 trở lại guard. Sọ khoảng11×13px, đầu tại y18 ở Idle,
chân đến y63. Key times lấy trực tiếp clip: 0/.083333336/.16666667/
.41666666/.9166667/.98333335/1.0833334/1.15/1.25/1.3166667;
telegraph.7s ở frame3, impact1s ở frame5, kết thúc1.3333334s.
Mốc thời gian là ràng buộc của mẫu; đánh giá art không suy ra balance.

### Kết quả mẫu và kiểm chứng

- Đã dựng và xuất cả hai mẫu qua LibreSprite1.3: `weapons-pixel.js` vẽ từng
  cel ở native resolution; `prepare_weapons.py` sở hữu tọa độ pose/vũ khí,
  đóng gói ASE rồi mở/lưu lại bằng LibreSprite. Không xoay bitmap hoặc nội suy.
  Sọ, ngực và chậu lấy cụm pixel từ Idle0 đã duyệt; chi/vũ khí/VFX được vẽ riêng.
- `design/skeleton-hero/weapon-milestone/`:22PNG96×64, `axe.aseprite`,
  `spear.aseprite`, `poses.json`, `verification.json`, `review.html` tự chứa ảnh.
  Mỗi native có11cel, tags Idle/Attack, lớp `Body + weapon` và `VFX` tách rời.
  Native duration là millisecond nguyên: Attack1333ms, sai số mốc<.334ms;
  preview giữ float clip gốc. Không đổi clip/gameplay để khớp native.
- Exporter PASS:22PNG; alpha0/255; không cắt biên vũ khí/VFX; chân cố định
  y63; cán rìu32px/thương58px với sai số raster<1px; frame9 khớp Idle0;
  native tái xuất trùng RGBA từng cel, kể cả xuất riêng lớp thân/vũ khí.
  Hash1101 file đầu vào (Assets, complete, cluster-milestone) không đổi trong
  lượt xuất cuối. Không sửa PNG runtime, hero, kiếm đã duyệt, prefab hay combat.
- Preview mới liên kết từ preview/README hiện có: heroIdle0, kiếm đã duyệt,
  rìu và thương; cùng timeline, scrub/step, mirror, onion skin, VFX và1×/.25×.
  Kiếm giữ VFX baked; toggle chỉ áp dụng mẫu mới. Không giả là render Unity.
- QA đã phát hiện/sửa: mũi thương bị cắt ở mép preview kích thước chơi;
  dời các cột, kiểm bounds tất cả cel có/không VFX và mirror. Cán rìu của
  wind-up2/3 ban đầu đi qua sọ; đã dời cả hai tay/cán lên trên trán và chỉnh
  silhouette/edge từng cel. Đã xem lại ba cel overhead phóng6×.
- `verify_weapons.cjs` dùng Playwright/Edge đã có: **27/27 PASS**, console/pageerror
  rỗng;1280×720,640×360,4×/8× nearest; telegraph.7→3, impact1→5,
  scrub/step/mirror/onion/VFX, pause và trở lại Idle. Đo lượt cuối trong cửa sổ
  khoảng525ms:1× thực.977727×, .25× thực.245810×. Không là đo frame pacing.
  Probe đầu dùng cửa sổ~80ms bị lệch do pha timer16ms; chuyển sang cửa sổ≥500ms,
  giữ kiểm sai số tương đối8%, không sửa tốc độ hay hạ acceptance art.
- Đã soi contact sheet đủ20 Attack và hai Idle, cùng canvas kích thước chơi
  và4×/8× không VFX. Nhận xét: rìu có overhead/cung bổ và chậu hạ sau contact;
  thương có thu cán/đẩy thân/duỗi rồi rút theo trục gần ngang. Hai tay giữ cán,
  đầu và màu xương cùng bộ kiếm; không thấy rung sọ hay đổi thể tích bất chợt
  trong chuỗi đã xem. Đây là đánh giá định tính, không thay user duyệt art.
  Điểm cần chú ý khi duyệt: hồi rìu khá gọn vì khung còn~.25s; đầu thương nhỏ
  ở640×360. Không tự kéo dài clip hoặc tăng weapon scale ngoài mẫu đã chốt.
- `python -m py_compile`, `node --check`, kiểm whitespace các file task PASS.
  Unity/WebGL không chạy vì chưa tích hợp gameplay. Không thêm dependency.
  Đã dọn scratch/capture của mốc này; giữ LibreSprite portable dùng chung tại
  `E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/` theo checkpoint cũ.

Trạng thái: **đã có hai mẫu để user duyệt, chưa được duyệt art**. Bước tiếp chỉ
sau duyệt: lập phạm vi Idle/Walk/Hurt/Die/đòn phụ và tích hợp riêng; không suy
ra đã duyệt balance/spawn. Checkout dirty khác được giữ nguyên, không commit gộp.

## Tiếp tục sau duyệt 2026-10-18 — hoàn thiện và tích hợp36 frame

User đã duyệt rõ bộ11 frame bằng “oke ổn, duyệt”. Giữ mẫu đó nguyên byte,
ghi hash PNG/native/mask trong `cluster-milestone/approval.json`. Quyền tiếp theo
theo plan đã có: hoàn thiện25 frame, tích hợp4PNG, Doctor/Quick/WebGL đúng hai
kích thước; không release/stress/soak.

- Doctor PASS: Unity2022.3.25f1 pinned, licensePresent=true, Docker29.7.2,
  free23.7GiB; snapshot trước tích hợp1769bf938797f93ebc8a93b368f1adb3460d10db34ed9b8f5435c836195aa588.
- Nguồn mới `remaining-poses.json`: tọa độ nét theo pose/chi tiết xương gốc;
  `original-pixel-edit.js` dùng cùng quy tắc màu/nét đã duyệt, hỗ trợ chậu rời
  và sọ ép ở cuối Die. `prepare_complete.py` xuất36 PNG/native và4sheet tại
  `design/skeleton-hero/complete/`, khóa11frame đã duyệt theo hash.
- Kiểm asset PASS: alpha từng pixel trùng originals, RGBA kiếm/grip/VFX trùng,
  native tái xuất trùng PNG, sheet giữ kích thước/thứ tự ô. Mask phần còn lại
  bao vật liệu kiếm với padding2px và toàn VFX diff. Hurt1 là **flash VFX baked
  sẵn trên toàn cel**, giữ nguyên RGBA cả frame theo yêu cầu khóa VFX; không
  đổi flash thành bóng thường. Đã soi đủ Idle8/Hurt5/Die13 ở4×: cụm sáng/tối
  theo bộ phận, Die giữ tách rời/ép xương gốc, chưa thấy lệch đường bao.
- Đã tích hợp **chỉ4PNG runtime**; meta/GUID/spriteID/pivot/PPU/filter và toàn
  clip/event/prefab/combat của task giữ nguyên. Quick cuối **51/51 PASS**, snapshot
  a1a06d81537fe6c96d123f1e47fa393be48b768d50cc2012224f6c80aa54f1b6,
  run20261018T052446-quick. Preview đủ36frame và sáu clip theo timeline gốc.
- Thêm trường quan sát tên sprite/texture trong bridge QA có sẵn (read-only,
  vẫn dưới TYPINGHERO_AUTOMATION) và driver `tools/game_loop/skeleton_art.cjs`.
  Lượt `python tools/game_loop/run.py webgl --browser-scenario skeleton-art`
  dùng harness/fixture/chặn mạng/giới hạn container hiện có, kiểm đúng1280×720
  và640×360; default full profile không đổi. Runner contracts8/8 và JS syntax PASS.
  WebGL snapshot c17c44b021207980a97430e999ef78e0fdf7f51d2e17337a2797fdd2c32c84b2,
  run20261018T050425-webgl FAIL ở pause sau4checks, chưa là bằng chứng PASS.
  Driver đầu so pause ngay publication đầu, chưa bảo đảm lượt Animator đã xong;
  chưa đủ dữ liệu tách trường nào lệch trong assert gộp. Đã lưu before/after và
  chờ hai frame render rồi mới so hold, vẫn yêu cầu HP/timer/sprite bằng tuyệt đối. Capture
  ban đầu đã để một impact xảy ra (HP35), nên lượt cue/impact tiếp theo đo từ
  HP thực tại cue thay vì giả định còn40; vẫn đúng một impact5HP và lead<.6s.
- Kiểm loop phát hiện9cel gốc giống hệt nhau (Idle0/1/6/7,Attack0/9,Hurt0/4,
  Die0) có2biến thể màu. Đã sửa exporter chỉ reuse **cel gốc trùng toàn RGBA**
  sang đúng paint đã duyệt, không đóng dấu lên pose khác; assert mọi nhóm cel
  gốc giống nhau phải có paint giống nhau. Tái xuất/tích hợp và kiểm lại PASS.
- Trong checkout có task `player-attack-sorting` đang thêm component vào
  Player.prefab và tag Attack cho player. Đối chiếu snapshot trước build xác
  nhận đây là thay đổi ngoài task skeleton, không reset. Prefab mới SHA256
  4a3c4c617fcf934a58dd8eee981f159d04c474dfc94659893d4b7a5b34e85dc3
  được ghi nhận riêng; hashguard trong lượt export vẫn kiểm mọi input không đổi.
  Bridge QA giữ cả quan sát sorting của task đó và tên sprite/texture của task này.
- Các lượt SwiftShader2CPU/8GiB không được tính PASS: snapshot a1a06d8
  run052605 còn capture trong khoảng đo cue→impact; đã tách capture sang swing
  khác. Lượt0fa4d3e/run053031 đo5HP nhưng lead.604345s với frame409.355ms,
  không đạt giới hạn.6s. Không nới giới hạn hoặc sửa gameplay để khớp môi trường.
- QA cuối dùng đúng player build0fa4d3e9c89ea6606e4a2b2aa5c03c2627cdefaccd53c725c7cecf4b4620c45d,
  tải dưới runner lock và đối chiếu cả4PNG source với runtime cuối. Tái dùng
  Playwright1.58.2 đã có, Edge155.0.4283.62/Intel UHD Graphics D3D11 cục bộ;
  host/harness/fixture giữ nguyên, thêm đường dẫn build cục bộ bằng env.
- **WebGL skeleton-art32/32 PASS,122.675s**, render thực1280×720 và640×360.
  Cả hai đều kiểm pause giữ HP/timer/sprite; cue→impact đúng5HP (lead lấy mẫu
  .261480/.260160s); typing thật→Hurt→Idle; parry→Stun/Slayer dùng Hurt;
  Die→event cleanup thật, restart về40/850HP/score0.28fixture reads/2mock writes,
  0exception; mọi request ngoài local/Firebase fixture bị chặn. Không là full
  broad WebGL/performance profile và không là đo frame pacing chính xác.
- Driver đầu gặp frame đổi từ `current=""` và có thể gõ khi hit gate đang khóa.
  Trace xác nhận; bản cuối chờ từ có chữ, gate mở, đủ thời gian gõ và đầu khoảng
  nghỉ (Hurt không ngắt attack gốc), xác nhận số phím thực nhận. Không sửa combat.
  Driver skeleton SHA256 b0a3c67081ad9241b824c407c77f24a7bbe613cc6f7f124e09b50d4fabdd7468;
  browser SHA256047c1eee025d5edb4f129015e1fbb95b04608ea93f2cb5edc4a8b2e54350d425.
- Đã xem16capture runtime của hai kích thước: xương sáng và khe xương đọc được,
  mirror/grip/mũi kiếm/slash đúng dáng gốc; Hurt1 trắng là flash nguyên gốc;
  Stun/Slayer giữ silhouette đen vốn có; Die bắt đầu rã và cleanup đúng event.
  Source đã soi đủ36frame ở4×/8× và playback sáu clip; cel gốc giống nhau có
  paint giống nhau, Idle cuối→đầu/Attack9/Hurt4/Die0 không thêm nhấp màu.
- Hoàn tất acceptance. Đã dọn capture/report/build/profile QA cục bộ và năm
  result directory của task trên runner sau khi ghi receipt; giữ source/build/cache
  dùng lại của runner và dữ liệu task khác. Diff/whitespace, frozen approval,
  đủ36frame preview và4PNG runtime khớp nguồn cuối đều PASS.
  Giữ LibreSprite portable tạiE:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/
  cho sửa nguồn sau này. Không release/stress/soak, không gộp commit checkout dirty.

## Mốc đã duyệt — chỉnh cụm pixel Idle0 và Attack1

Yêu cầu hiện hành: giữ nguyên alpha từng pixel, dáng, khoảng rỗng và RGBA
kiếm/grip/slash; chỉnh nét và cách chia sáng/tối bên trong xương. Hoàn thiện
Idle0 + đủ10 Attack1 cùng một mốc duyệt. Hai lần “oke” trước không duyệt tạo hình
mới. Chưa được nhân rộng25 frame hoặc thay runtime trước khi user duyệt bộ này.

- Nguồn chỉnh: `design/skeleton-hero/original-pixel-edit.js`, exporter hiện có
  `prepare_preserved.py`; đầu ra mới `cluster-milestone/`:11 native +11 PNG96×64,
  native/PNG sheet Attack1 960×64, diff màu so với palette và mask vùng khóa.
  `preserved-original-milestone/` giữ nguyên làm bản **chỉ đổi màu trung gian**,
  không phải bộ được duyệt. Không tái dùng hình khối của các study đã loại.
- Chỉnh cel originals trong LibreSprite1.3 bằng nét chọn theo tọa độ từng pose:
  nối sáng vòm/trán và gò má, gom bóng hàm; gom đầu sườn phía tối, giữ khe phía
  sáng; chia mặt chậu và nối dải sáng chân xa. Không dựng lại đường bao hoặc
  đóng dấu toàn đầu/thân. Attack9 dùng cùng chỉnh với Attack0 vì hai cel gốc
  trùng nhau. RGB250 giữ; màu lạnh từ giáp hero, tím trầm chỉ ở hốc tối.
- Exporter PASS đủ11 frame: alpha0/255 và alpha từng pixel trùng originals,
  RGBA toàn mask kiếm/grip/VFX trùng; xuất lại từng native trùng PNG;
  sheet trùng các frame; Idle0/Attack0/Attack9 trùng nhau. Kiểm partition màu
  xác nhận đây có sửa cụm bên trong, không chỉ một ánh xạ palette. Kiểm cấu
  trúc này không kết luận chất lượng art bằng số pixel đổi.
- Review mask phát hiện envelope cũ Attack1 frame1 chỉ bao grip, chưa bao kín
  lưỡi kiếm đứng qua sọ. Các pixel cạnh kiếm vốn vẫn trùng gốc; đã mở envelope
  bảo thủ từ đỉnh kiếm qua sọ/ngực tới grip để khóa rõ toàn vùng chồng lấp,
  giữ thêm pixel xương cạnh kiếm. Tái xuất đủ11 frame và kiểm native/alpha/RGBA
  lại PASS; các frame khác giữ nguyên chỉnh nét. Đã xem lại frame1 ở cả bốn
  tỷ lệ và mask8× trực tiếp tại sọ/ngực/grip; không thấy lệch cạnh kiếm.
- Lệnh: `python design/skeleton-hero/prepare_preserved.py --libresprite
  E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe`, rồi
  `python design/skeleton-hero/build_preview.py`. Hash input không đổi trong
  lượt xuất. Baseline cũ vẫn chỉ lệch hai file đã biết `TutorialOverlay.cs`
  và `SlayerScreenFX.cs`; không reset baseline hoặc sửa thay đổi task khác.
  Hero, runtime PNG/meta, clip/event, controller/prefab và combat được theo
  dõi vẫn khớp baseline. Không đổi camera/API/dependency.
- Preview hiện có bốn cột nhãn riêng: hero, originals, chỉ đổi màu, chỉnh cụm.
  Scrub/step/playback dùng chung timeline Unity; overlay, mirror, diff màu và
  mask hồng vùng khóa, zoom4×/8× và canvas1280×720/640×360. Không giả nhận đây
  là render Unity. Event telegraph.7s→frame3, impact1s→frame5, clip1.3333334s.
- Đã xem đủ10 keyframe trong Orca ở1280×720,640×360,4×/8×: xương sáng, đọc
  được chân/khớp; cụm sọ, đầu sườn và chậu khác bản palette. Không thấy lệch
  pose/chân/kiếm/slash; không thấy bóng mới nhảy giữa các bộ phận trong các
  frame đã soi. Idle0 có pixel trùng Attack0 nên cùng bằng chứng hình ở bốn tỷ lệ.
- Playback1×/.25× đã chạy cả bốn tỷ lệ. Lượt đầu8× có lấy mẫu thiếu frame ngắn
  và tiến thời gian bị chậm ở thời điểm đọc; không hạ timing gốc. Sửa preview
  để cache nền ghép theo kích thước và không reset canvas mỗi tick. Hình impact
  sau sửa **trùng từng pixel** trước sửa ở cả bốn tỷ lệ. Kiểm lại8×:1× wall6.5133s,
  tiến clip6.5124s; .25× wall6.5002s, tiến clip1.623475s (kỳ vọng1.62505s).
  Hai lượt quan sát đủ10 frame; smoothing=false. Đây là lấy mẫu endpoint trong
  browser, không phải đo frame pacing chính xác hoặc bằng chứng combat Unity.
- Đã dùng control thật kiểm overlay+mirror ở impact, diff màu, mask impact và
  frame1 chồng sọ; scrub bằng phím Home→frame0, End→frame9. Phát lại bộ cuối
  sau sửa mask quan sát đủ10 frame. Console trống. Đã dọn64 capture QA tạm;
  preview bàn giao trở về Idle0,1280×720, dừng, tắt overlay/diff/mask/mirror.
- **Chưa duyệt hình, chưa tích hợp runtime.** Sau duyệt: Idle7→Hurt5→Die13,
  kiểm loop/chuyển trạng thái kể cả Stun/hitBySlayer dùng Hurt; từng sheet đạt
  mới thay PNG. Doctor + Quick>0 + WebGL input thật1280×720/640×360 theo skill,
  fixture/chặn mạng kể cả score writes; không release/stress/soak.
- Giữ LibreSprite portable tại `E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/`
  để tiếp tục sau duyệt; nguồn bàn giao nằm trong design. Không commit gộp
  checkout dirty.

## Mốc trước — chỉ đổi màu, đã được thay thế bởi chỉnh cụm pixel

Mẫu `attack-milestone/` bị user loại: cơ thể dựng bằng khối cố định và đoạn
thẳng làm lệch tỷ lệ/pose; slash cánh cung mới không được giữ. Checkpoint bên
dưới về quyền vẽ lại kiếm/slash đã bị thay thế bởi yêu cầu mới.

- Phạm vi duyệt: Idle 0 + đủ 10 frame Attack1. Giữ silhouette/alpha, tỷ lệ,
  nghiêng thân, tay chân, grip, quỹ đạo kiếm và slash từng frame originals.
  Chưa sửa 25 frame còn lại, chưa thay PNG runtime.
- Nguồn mới: `design/skeleton-hero/original-pixel-edit.js`,
  `prepare_preserved.py`, `preserved-original-milestone/` (11 PNG/native,
  sheet Attack1, mask vùng khóa và kết quả kiểm tra). Script sửa cel originals
  trong LibreSprite1.3; không dựng đầu/thân/chân mới. Giữ cụm sáng/tối gốc,
  dùng ba bậc màu giáp hero cho bóng xương, tím trầm ở vùng tối nhất;
  highlight250 giữ nguyên. Đây là chỉnh palette bảo toàn cụm pixel, không phải
  một bản dựng pose mới.
- Xuất bằng `python design/skeleton-hero/prepare_preserved.py --libresprite
  E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe`:
  PASS 11 canvas96×64, Attack1 960×64, alpha0/255 và toàn bộ alpha trùng gốc;
  pixel trong envelope kiếm/grip, vật liệu blade/hilt và VFX diff trùng RGBA.
  Đổi203–284 pixel/frame. Export lại native trùng PNG; hash các input được
  theo dõi không đổi trong lượt chạy. Mask bảo thủ giữ cả pixel xương gần kiếm.
- Baseline cũ đã lệch trước khi sửa tại `TutorialOverlay.cs` và
  `SlayerScreenFX.cs` do checkout khác đang thay đổi. Không reset baseline,
  không sửa hai file đó; không tuyên bố toàn workspace trùng baseline cũ.
  Hero, runtime PNG, metadata, clip/event, controller/prefab và combat còn
  trùng baseline tại lần kiểm tra. Mốc .7s/frame3, 1s/frame5 và tổng clip
  1.3333334s giữ nguyên.
- Preview hiện lấy nguồn mới, originals và hero chạy đồng bộ; Idle1–7 vẫn
  ghi chưa chỉnh. Bố cục nền chỉ tái dựng để duyệt art, không phải Unity.
  Đã xem so sánh đủ11 frame: hình kiếm/slash/pose giữ nguyên, xương sáng hơn,
  độ thay đổi màu có chủ ý nhẹ. Chất lượng art cuối cùng còn chờ user duyệt.
- Orca preview: canvas1280×720/640×360 và zoom4×/8×, smoothing=false;
  đã xem hình cạnh hero và chuỗi chuyển động lấy đà/chém/thu. Không thấy rung
  hình hay nhấp nháy màu mới trong phần xem; đây là nhận xét định tính,
  không thay duyệt art. Hai bản dùng chung keyframe và thời gian phát.
  RAF trong browser nhúng ngừng callback dù visibility=visible; sửa preview
  dùng interval60Hz với elapsed time. Sau sửa, Attack1 phát1× tăng .5112s
  trong .5108s wall time; .25× có tiến thời gian và đi hết vòng. Lấy mẫu chậm
  có jitter khi render, chưa chứng minh frame pacing chính xác. Console trống.
  Telegraph frame3 được đối chiếu tại .7s; impact frame5 tại1s.
- Quick/WebGL chưa chạy vì chưa tích hợp runtime. Sau duyệt mới triển khai
  Idle7/Hurt5/Die13, kiểm loop/chuyển trạng thái, tích hợp từng sheet đạt,
  Doctor + Quick>0 + WebGL input thật1280×720/640×360 với fixture/chặn mạng.
  Không đổi hero/API/metadata/clip/prefab/camera/combat, không release/stress/soak.
- Giữ `E:/CodexTemp/TypingHero/skeleton-hero-art/` cho LibreSprite portable
  và lượt sửa sau duyệt; capture QA của lượt này đã dọn sau khi ghi kết luận.

## Lịch sử mẫu bị loại — không dùng làm hướng hiện hành

## Checkpoint mới 2026-10-18 — Idle 0 và toàn bộ Attack1

Yêu cầu user mới thay thế phạm vi ba pose và khóa pixel kiếm/VFX bên dưới.
Được vẽ lại cơ thể, đường kiếm và slash; giữ hero, combat, keyframe/event và tổng clip.
Mốc duyệt bắt buộc: Idle 0 + Attack1 10 frame, sau đó mới làm Idle 7/Hurt 5/Die 13.
Chưa có duyệt mẫu mới; chưa thay PNG runtime.

- Đã dựng 11 frame bằng `attack-sample.js` và `prepare_attack.py`, vẽ/xuất qua
  LibreSprite 1.3. Nguồn `.aseprite`, PNG từng frame, sheet Attack1 và landmark ở
  `design/skeleton-hero/attack-milestone/`. Dùng chung skull/ribcage/pelvis;
  chân giữ vị trí, trọng tâm qua hông/gối; kiếm mới nối grip, slash ngoài thân.
- Attack 0 chuẩn bị → 1–2 lấy đà → 3 giữ thế qua telegraph .7s → 4 mở kiếm →
  5 chém qua damage 1s → 6 follow-through → 7–9 thu đòn. Giữ nguyên 10 key time
  thực tế từ .0 tới 1.3166667, tổng 1.3333334s. Frame 9 bằng pose idle mẫu.
- `python design/skeleton-hero/prepare_attack.py --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe`
  PASS: 11 canvas96×64, Attack1 960×64, alpha0/255, không cắt biên trên/trái/phải,
  chân x37–40/50–53 ở y63, không component rời nhỏ hơn4 pixel; export native đọc lại
  trùng sheet. Toàn bộ hash baseline (hero, metadata, sáu clip, controller, prefab,
  gameplay/config và PNG runtime) không đổi. Đây là kiểm tra cấu trúc, không chứng minh
  chất lượng chuyển động hay hitbox trong Unity.
- `build_preview.py` cập nhật `review.html`: toàn Attack1 mới, chỉ Idle0 mới;
  ghi rõ 25 frame chưa sửa. Có hero/gốc/mẫu, nền layer+tint từ ShogunEnvironment
  với bố cục tái dựng, canvas1280×720/640×360, zoom4×/8×, overlay, mirror,
  landmark mẫu, scrub/step và playback1×/.25×. Không giả định tương đương render Unity.
- Orca browser eval kiểm tra ready, đủ10 keyframe, canvas hai kích thước,
  smoothing=false, telegraph.7→frame3/impact1→frame5, overlay/mirror, bật phát .25×,
  dừng và nhãn Idle1 chưa chỉnh; console trống. Đã xem canvas impact và sheet.
  Screenshot API lỗi helper; đọc canvas PNG để kiểm tra hình. Chưa đo định lượng
  tốc độ playback, chưa đối chiếu đầy đủ chuyển trạng thái vì các sheet khác chưa vẽ.
- Tham chiếu chính thức: https://goblinzstudio.com/game/shogun-showdown/ và gallery,
  https://roboatino.itch.io/shogunshowdown . Đã đối chiếu chuỗi hình trailer chính thức
  Steam “Full Launch Trailer”, movie257052703, đoạn12–18s lấy mẫu6fps: pose gọn,
  giữ thế trước burst ngắn, vệt trắng đọc rõ trên nền tối, sau impact trả tư thế nhanh.
  Áp dụng nhịp giữ Attack3, mở/chém4–5 và thu6–9; không sao chép nhân vật/trang phục.
  Lấy mẫu này chỉ chứng minh đối chiếu định tính, không đo timing animation gốc.
  Fetch itch local bị403; server không resolve CDN, đã tải hai segment qua Windows
  rồi dùng ffmpeg sẵn có trên server để trích hình. Không cài tool/dependency mới.
  `shogun-motion-reference.png` là nghiên cứu imagegen mới, không phải ảnh game chính thức
  hay runtime. Prompt: skeleton ba pose idle/coiled wind-up/horizontal slash, ivory,
  muted plum outline/cool shadows, chunky clusters, không áo/phụ kiện, slash ngoài thân.
- Doctor PASS: Unity2022.3.25f1 pinned, Docker29.7.2, licensePresent=true,
  free23.7GiB. Quick/WebGL cho art mới **chưa chạy**, đợi duyệt và tích hợp runtime.
- Bước tiếp: user duyệt/sửa Idle0+Attack1;
  sau duyệt mới vẽ25 frame, tích hợp từng sheet đạt, Quick>0 và WebGL input thật,
  fixture/chặn mạng ngoài, hai kích thước, typing/telegraph/impact/parry/hurt/stun/
  Slayer/death-cleanup/pause-resume/restart. Không release/stress/soak.
- Giữ `E:/CodexTemp/TypingHero/skeleton-hero-art/` cho LibreSprite và tiếp tục task;
  giữ nguồn/originals trong design. Không commit gộp checkout dirty của các task khác.

## Lịch sử trước khi đổi hướng — không còn là phạm vi hiện hành

## Tiếp tục 2026-10-18 — mốc ba frame chờ duyệt

- Điểm duyệt bắt buộc theo plan user: hoàn thiện và trình Idle0, Attack1 3/5 cạnh
  hero trước khi nhân rộng. **Chưa có duyệt tạo hình; chưa thay PNG runtime.**
- Đã bảo toàn bốn PNG + metadata tại `design/skeleton-hero/originals/`. Baseline
  `design/skeleton-hero/baseline.json` ghi kích thước 768/960/480/1248×64, 36 tên
  sprite, GUID/spriteID/internalID và hash. Sáu clip lấy theo motion GUID thực
  của AC_Skeleton, giữ đầy đủ YAML keyframe/event, kể cả thay đổi sẵn trong checkout.
  Hash hero/controller/prefab/gameplay được ghi từ working tree, không từ HEAD.
- Built-in imagegen tạo `pose-style-reference.png` để nghiên cứu; không dùng làm
  sheet. Ba mẫu vẽ lại bằng script pixel trong LibreSprite1.3, lưu native và PNG
  tại `design/skeleton-hero/samples/`. Palette lấy màu outline/giáp của hero;
  skull/ribcage/pelvis dùng chung tạo hình, limb theo pose, không scale nhân vật.
- `prepare_samples.py` xuất rồi đọc lại: ba canvas96×64, alpha0/255, footprint
  alpha tại y63 trùng gốc; 129/112/565 pixel trong mask hình học kiếm/tay cầm và
  slash trùng RGBA gốc. Script kiểm tra ảnh thực sự đổi và lỗi LibreSprite stdout
  vì batch có thể exit0 khi script lỗi. Lần đầu activeImage=null dẫn đến xuất
  nguyên ảnh gốc; đã sửa sang activeSprite.layer(0).cel(0).image và kiểm tra lại.
- Preview `design/skeleton-hero/review.html` tự chứa ảnh và timing, resolve frame
  bằng internalID từ importer: Idle0, Attack1 frame3 tại .7s và frame5 tại1s.
  Có 1×/4×/8× nearest, scale chơi tái dựng3.5/1.75px mỗi texel, overlay gốc,
  mask, pivot/grip/tip, mirror, scrub, bước keyframe và playback1×/0.25×.
  Những frame chưa chỉnh được ghi rõ; preview mixed chưa chứng minh cả loop mượt.
- Orca browser đã kiểm tra load, chọn ba pose, mốc .7/1s, 4×/8×/1.75×,
  overlay thay canvas, mask, tắt image smoothing, playback/quarter-speed và dừng
  giữ thời gian; console không có message. Đã xem trực tiếp Idle và anticipation
  cạnh hero. Đây là QA preview, không phải QA Unity combat.
- Doctor ban đầu fail `[Errno11] Resource temporarily unavailable`; không xóa
  lock/dừng process khác. Khi runner rảnh, chạy lại
  `python tools/game_loop/run.py doctor` đạt, Unity2022.3.25f1 image pinned,
  license có, Docker29.7.2, trống23.7GiB.
- Chưa chạy Quick/WebGL cho art mới vì chưa có bốn runtime sheet được duyệt và
  tích hợp. Không kết luận timing/combat mới đã được kiểm chứng trong game.
- Còn lại: user duyệt/sửa ba mẫu → đánh dấu landmark/mask từng frame còn lại →
  Idle8, Attack1 10, Hurt5, Die13 → kiểm palette/anchor/kiếm/vòng lặp → thay chỉ
  PNG từng sheet đạt → Quick>0 tests và WebGL sáu trạng thái1280×720/640×360
  với fixture/chặn mạng ngoài. Giữ mọi clip/event, metadata/pivot/PPU, hero,
  controller/prefab/collider/hitbox/combat. Không release/stress/soak.
- Hướng dẫn rebuild, landmark ba mẫu và prompt imagegen ở
  `design/skeleton-hero/README.md`. Temp cần giữ để tiếp tục:
  `E:/CodexTemp/TypingHero/skeleton-hero-art/` (LibreSprite portable, script đầu
  vào và capture QA); dọn capture khi nghiệm thu, giữ nguồn thiết kế và originals.

## Checkpoint sửa rìu theo chuỗi bổ — 2026-10-18

- Yêu cầu mới: lấy chuỗi bổ hàng hai làm reference; cel1–3 nâng quá đầu,
  cel4 vung nhanh với smear ngắn; cel5 cán gần ngang, tay thấp phía trước,
  thân gập và gối khuỵu; cel6–9 nén rồi giữ mép cắt ghim y63. Grip cố định,
  không bắt buộc trượt tay. Giữ Idle0 + Attack10, 96×64, cán32px và timing gốc.
- Đã nhận và lưu nguyên Image #2 tại `design/skeleton-hero/references/axe-chop-user.png`
  cùng bản mirror; blocker thiếu ảnh đã giải quyết. Hàng hai làm chuẩn, hàng ba
  chỉ hỗ trợ chịu lực, không lấy đoạn nhấc rìu. Đã sửa cel1–9 theo mô tả trên,
  vẽ thorax gập riêng, giữ grip7/15px và đầu rìu cố định từ contact đến cuối.
- Đã sửa preview phát Attack một lần, giữ cuối và Play chạy lại từ đầu;
  pause/resume giữa clip giữ thời gian. Đã xuất pose/PNG/native mới; không đổi Assets.
- Kiểm chứng: exporter LibreSprite PASS22PNG, hai native11frame, exact body/
  composite roundtrip và1113 input giữ hash, gồm12file thương. Mép cắt sáng thật
  chạm y63 qua cel5–9; toàn đầu rìu cố định, cán ngang và không VFX sau contact.
  Idle0/Attack0 rìu giữ nguyên, smear chỉ ở cel4. Cán32px sai số raster dưới1px.
  Đối chiếu git hash riêng:1115file protected (Assets, complete, cluster, thương,
  Idle0/Attack0 rìu) khớp HEAD; không chỉ so hash trong một lần export.
- Playwright Edge headless:40 checks PASS, không browser error; 1280×720,
  640×360, 4×/8×; giữ cuối/replay/pause/resume ở cả hai kích thước chơi.
  Playback đo được1×≈0.9869 và0.25×≈0.2456. Python AST, JS syntax và whitespace PASS.
- Đánh giá hình: đã soi10cel không VFX ở4×, anticipation/final mirror và contact
  ở1280×720/640×360. Pose mới gập thấp, tay duỗi trước, chân giữ anchor, sọ giữ
  kích thước và cán nằm phía trước thân. Đây là kiểm pose và playback tự động;
  cảm giác chuyển động vẫn cần user đánh giá, không khẳng định đã được duyệt.
- Giữ impact1s, tổng clip1.3333334s/native1333ms. Không chạy Unity/WebGL vì chưa
  tích hợp runtime. Capture/build tạm dọn sau khi ghi kết quả; giữ tool LibreSprite
  portable sẵn có để tiếp tục sửa nguồn khi cần.

## Checkpoint cel4 bổ có lực — 2026-10-18

- Phản hồi lượt trước: kỹ thuật PASS nhưng cel4 đứng và giữ tay cao, vệt chéo
  mảnh đọc thành quét ngang; chuyển động chưa đạt, không coi checkpoint cũ là duyệt.
- Đã dựng lại cel4 trong `prepare_weapons.py`: vai/chest y40, đầu y27,
  hông y47, gối khuỵu; hai tay kéo xuống trước, cán từ (54,41) đến (85,49).
  Dùng thorax gập, giữ lưỡi và grip7/15px. Lần soi đầu tiên phát hiện cán đi qua
  phần dưới sọ; hạ cán/tay thêm6px trước khi xuất bản mẫu cuối.
- `weapons-pixel.js`: thay vệt mảnh bằng mảng cung quạt rộng, mặt sáng H/L,
  dải ngoài S theo palette có sẵn; chỉ cel4. Mép VFX thấp nhất y60, chưa chạm
  đất y63; cel5–9 vẫn không smear, toàn đầu rìu ghim cố định và mép sáng chạm y63.
  Giữ canvas96×64, số cel, cán32px, timing, contact và các pose khác.
- Kỹ thuật: rebuild LibreSprite PASS22PNG, hai native11frame, body/composite
  roundtrip exact, alpha/bounds/chân/grip/timing PASS;1118 input file giữ hash
  trong lần export cuối. Bổ sung assert smear chỉ cel4 và không chạm đất.
  Playwright Edge40checks PASS, không browser error;1280×720,640×360,4×/8×.
  Playback đo1×=0.9968,0.25×=0.2494; lấy mẫu playback từ telegraph xác nhận
  đi qua cel3→4→5 ở cả hai tốc độ; giữ cuối/replay/pause/resume PASS.
- Nhận xét hình: so cel3→4→5 không VFX rồi có VFX, cả mirror, với hàng hai
  reference cùng hướng. Cel4 hạ đầu/vai và gập thân rõ, cán trước thân dưới sọ;
  cung rộng đi xuống sát nền, cel5 lộ contact. Soi nearest và capture browser
  ở kích thước chơi. Đây là kiểm pose và playback lấy mẫu, chưa có user duyệt
  cảm giác lực; giữ mẫu chờ đánh giá, không kết luận chuyển động đã đạt.
- Không tích hợp runtime, không Unity/WebGL/API. Trong lúc làm có thay đổi
  Assets và `docs/plans/combat-feedback.md` từ công việc khác; giữ nguyên.
  Capture/build tạm được dọn sau khi ghi kết quả, giữ LibreSprite portable cũ.

## Phạm vi và acceptance

Chỉnh tỷ lệ, nét và đổ bóng skeleton theo hero; giữ kiếm và chuyển động.
Làm mẫu idle, anticipation và contact để duyệt trước khi triển khai đủ Idle,
Attack1, Hurt, Die thuộc Skeleton_White/Skeleton_With_VFX.
Giữ nguyên hero, ô sprite, thứ tự frame, pivot, PPU, filtering, GUID,
animation timing/events/transitions, collider/hitbox, damage và parry.
Không đổi API/leaderboard hay xuất bản lên itch.

## Thực hiện và kiểm chứng

1. Đối chiếu mẫu cạnh hero ở kích thước chơi thật và phóng lớn, phát timing gốc.
2. Sau khi mẫu đạt, dùng imagegen và LibreSprite hoàn thiện bốn sheet;
   lưu bản gốc để khôi phục và kiểm tra cấu trúc import không đổi.
3. Xem idle/attack/hurt/stun/Slayer/death: chân, kiếm và tỷ lệ ổn định.
4. Unity Quick và WebGL với leaderboard fixture: typing/parry/Slayer/death/
   pause/restart. Không dùng static checks thay bằng chứng hành vi.

## Checkpoint 2026-10-18

- Workspace hiện có nhiều thay đổi staged/unstaged/untracked của task khác;
  giữ nguyên, không commit gộp hay ghi đè prefab/animation.
- Xác minh Unity 2022.3.25f1; Attack1 GUID
  `1a1b37ac36b89394492c859d829adbcd`, Hurt GUID
  `7ec67ad34b59fcd418a1e7cf7aae7d13`, Idle GUID
  `fb5c7de69732e054bbea856089b9c6ee`.
- Attack telegraph .7s, damage 1s; Attack1 10 frame, idle 8 frame;
  importer Attack1 point filtering, PPU16. Không chỉnh các giá trị này.
- Đã xem hero idle, skeleton idle và Attack1 gốc. Đang tạo mẫu 3 pose
  bằng built-in imagegen, hero là style reference, skeleton là pose reference.
- `python tools/game_loop/run.py doctor`: FAIL, `[Errno 11] Resource
  temporarily unavailable`; Unity/WebGL chưa kiểm chứng.
- Chưa thay asset runtime. Cần kiểm tra mẫu sinh ra, khả năng LibreSprite,
  và duyệt mẫu trước khi mở rộng.

### Kết quả mẫu và giới hạn hiện tại

- Built-in imagegen lần 1: ba pose có màu ivory/violet hợp hướng tham chiếu,
  nhưng anticipation bị đổi thành thrust, hình kiếm/chân lệch. Loại khỏi runtime.
- Lần 2: yêu cầu edit nguyên Attack1 960x64/10 ô96x64, hero chỉ làm style
  reference; khóa pose, chân, kiếm, slash, yêu cầu ivory/violet/charcoal và pixel
  sắc nét. Output thực tế2171x724, vị trí/kích thước sprite và VFX thay đổi,
  có viền màu thừa. Đây là nghiên cứu màu, KHÔNG đạt acceptance sprite sheet.
- Lưu mẫu lần2 tại `design/skeleton-hero/attack-style-study.png` và bản LibreSprite
  `attack-style-study.aseprite`; không có Unity reference tới hai file này.
- LibreSprite1.3 portable đã chạy batch chuyển mẫu sang native thành công;
  nằm tại `E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite`.
  Giữ tool cho bước sửa pixel tiếp theo; các hình zoom thử không phải deliverable.
- Doctor thất bại do runner khác đang giữ `/data/typinghero-game-loop/runner.lock`
  (xác minh bằng lslocks, PID3137557 rồi3141046). Không dừng hoặc chiếm runner
  của task khác. Quick/WebGL chưa chạy; không có bằng chứng combat mới.
- Chưa triển khai bốn sheet: cần hoàn thiện mẫu có đúng hình học/kiếm/VFX,
  đối chiếu hero và phát timing gốc; sau đó duyệt tạo hình rồi mới mở rộng.
- Không sửa hero, bốn texture runtime, metadata, animation, prefab hay gameplay.


## Checkpoint h?nh h?c r?u v? chu?i pose b? ? 2026-10-18

- Ba ph?n h?i tr??c (h?nh r?u/tr?c c?n, chu?i pose ch?u l?c, cung smear) **ch?a ??t theo user**; c?c checkpoint k? thu?t c? kh?ng ph?i duy?t chuy?n ??ng.
- ?? d?ng m?t m?u blade/light/edge/socket cho cel1?9, bi?n ??i t?a ?? theo tr?c c?n th?c t? r?i raster; kh?ng xoay bitmap. Idle0/Attack0 gi? ngu?n c?. M?p c?t k?o t?i toe ? m?i g?c. Grip7/15px, c?n32px, contact cel5/y63 v? ??u r?u c? ??nh cel5?9 gi? nguy?n.
- Landmark cel1?9 d?ng c?ng c?nh tay/c?ng tay12px; khu?u gi?i theo vai v? grip. Cel4 h? h?ng/ng?c/??u, tay k?o tr??c th?n; cel5?7 khu?u c?n g?p, h?ng v? g?i nh?n l?c; cel8?9 gi? ??t. Canvas, ch?n neo, s? cel v? timing kh?ng ??i.
- Smear ch? cel4, n?i c?c m?p c?t trung gian t? cel3 t?i cel4; tr?c quay ?i c?ng butt/tay, c? nh?p h? tay cong trong kho?ng chuy?n ti?p ?? l??i kh?ng v??t ??nh canvas. Bi?n cu?i ch?nh l? edge cel4; to?n ???ng qu?t kh?ng th?p h?n toe cel4. B? cung v? s?n.
- K? thu?t: rebuild LibreSprite PASS22PNG/hai native11frame, body/composite roundtrip exact, alpha/bounds/ch?n/grip/timing/contact PASS;1118 file input gi? hash trong l?n export. JS syntax v? git diff --check PASS. Playwright Edge40checks PASS, kh?ng browser error,1280?720/640?360/4?/8?; playback ?o1?=0.9977,0.25?=0.25005; cu?i/replay/pause/resume PASS.
- Nh?n x?t h?nh: ?? soi to?n b? cel body v? smear ri?ng, overlay tr?c c?n/socket/???ng l??i cel3?4?5 ?4? v? mirror, ??i chi?u reference h?ng hai c?ng h??ng. Tr?c/socket li?n, v?t qu?t n?i ??ng cel4; chu?i h? th?n v? khu?u g?p ??c r? h?n. Browser x?c minh controls/t?c ??/k?ch th??c; ch?a ph?i quan s?t playback li?n t?c b?ng m?t ho?c QA runtime. Kh?ng d?ng k?t qu? n?y ?? tuy?n b? c?m gi?c l?c ??t: **m?u ti?p t?c ch? user ??nh gi?**, c? ba ph?n h?i ch?a ???c user x?c nh?n ?? gi?i quy?t.
- Kh?ng ch?y Unity/WebGL; kh?ng s?a runtime/API/th??ng/hero/ki?m. Gi? c?c thay ??i checkout c? s?n c?a task kh?c. D?n output build/overlay t?m sau khi ghi checkpoint; tool LibreSprite portable c? v?n gi? cho s?a m?u ti?p.

## Checkpoint full-pack — 2026-10-18

- Phạm vi mới theo plan user: 72 sheet PNG (3 vũ khí × 2 màu × 6 animation × 2 VFX), 6 native hai lớp và preview chung tại `design/skeleton-hero/full-pack/`. Chỉ asset thiết kế; không tích hợp Assets, Unity/WebGL hoặc itch.io. Acceptance/timing/pose trong plan user giữ nguyên và thay phạm vi runtime cũ ở trên cho lượt này.
- Đã xác minh nguồn Walk/Attack2 kiếm và Yellow trong pack; nguồn kiếm đã duyệt ở `complete/` và `cluster-milestone/` phải bảo toàn. Checkout dirty có sẵn của task khác được giữ nguyên.
- Đã mở rộng `prepare_weapons.native(..., animation_tags=None)` nhận danh sách tag liên tiếp, không chồng lấn, phủ đủ timeline; kiểm duration uint16 và tên tag. Caller cũ giữ mặc định Idle/Attack. Đây là chuẩn bị exporter, chưa xuất full-pack.
- Kiểm chứng bằng LibreSprite portable: roundtrip native cũ 3 cel và native thử 61 cel/6 tag Idle/Walk/Attack1/Attack2/Hurt/Die, đúng tag/duration/composite RGBA. Input thử dùng lặp Idle chỉ để kiểm đóng gói, KHÔNG là animation bàn giao. Reject tag thiếu/sai range/trùng tên; Python AST PASS. 1165 file Assets/nguồn/kiếm/milestone giữ hash trước và sau check. File thử được dọn tự động.
- Thiếu nguồn bắt buộc: ảnh dáng nghỉ rìu mới mà plan gọi “ảnh vừa gửi” không có trong context hiện tại hoặc thư mục references. Đã xem `axe-blade-user.png`: chỉ có đầu rìu/cán, không có skeleton/tay/dáng nghỉ. Hai file còn lại là reference bổ cũ. Không tìm thấy ảnh PNG/JPG/WebP trong Downloads. Không thể dùng ảnh cũ làm bằng chứng cho tỷ lệ/cách cầm/dáng nghỉ mới.
- Cần user đính kèm lại ảnh dáng nghỉ mới hoặc chỉ đường dẫn chính xác. Chưa dựng/sửa chuỗi rìu, chưa tạo 72 PNG/6 native/preview; chưa kiểm chuyển động. Bước tiếp: lưu reference chuẩn mới, đối chiếu toàn chuỗi rìu trước khi nhân Yellow/VFX, rồi hoàn thiện và QA toàn bộ bộ theo plan user. Không coi checkpoint kỹ thuật này là animation đã đạt.

## Checkpoint nguồn MonoPixelArt — 2026-10-18

- User chỉ định https://monopixelart.itch.io/skeletons-pack làm chuẩn và yêu cầu copy giống animation nguồn. Đã tải/xem GIF công khai; lưu nguyên byte tại `design/skeleton-hero/references/monopixelart-pack-overview.gif` (960×400, 9 frame) và `monopixelart-sword-preview.gif` (300×192, 55 frame). Đây là reference gốc, không phải sheet xuất hoặc bằng chứng toàn bộ animation rìu đã có.
- GIF overview cung cấp dáng nghỉ rìu một tay: cán đứng, tay phụ thả. Blocker thiếu ảnh Idle mới ở checkpoint trước đã được giải quyết bằng nguồn user vừa chỉ định; không yêu cầu lại ảnh đó.
- Overview cho thấy ba loại kiếm, rìu, kiếm+khiên; không thấy thương trong overview. Trang mô tả Free chỉ có kiếm; Premium gồm đủ ba skeleton và đang niêm yết từ $3. Workspace chỉ tìm thấy Skeletons_Free_Pack, không thấy ZIP skeleton trong Downloads. Không mua hoặc truy cập nội dung Premium.
- GIF minh họa riêng chứa skeleton kiếm; GIF overview chỉ 9 frame Idle, không cung cấp trọn Attack/Hurt/Die/Walk rìu. Để sao đúng toàn bộ cel/timing/palette/alpha của rìu cần ZIP Premium do user cung cấp. Cần chốt loại thứ ba theo pack (kiếm+khiên) hay tiếp tục thương theo plan cũ. Chưa xuất full-pack hoặc thay đổi runtime.

## Checkpoint bàn giao full-pack quan sát GIF — 2026-10-18

- User xác nhận không có ZIP và yêu cầu tự quan sát/copy. Đã tiếp tục bằng GIF công khai và nguồn có sẵn, không còn đặt ZIP hoặc ảnh Idle làm điều kiện triển khai. Giữ thương theo plan ban đầu; không tự thay bằng kiếm+khiên.
- Đã xuất `design/skeleton-hero/full-pack/`: 72 PNG theo vũ khí/màu/VFX/animation; 6 native LibreSprite, hai lớp Body + weapon/VFX, sáu tag đúng thứ tự; 342 cel tổng cộng. `review.html` có selector vũ khí/màu/clip, VFX/mirror/onion/overlay, timeline duration thực, replay/pause/resume/scrub/bước cel và bốn kích thước xem.
- Nguồn mới: `full_pack_poses.py` sở hữu pose/hình học, `full-pack-pixel.js` vẽ trong LibreSprite bằng primitive dùng lại từ `weapons-pixel.js`, `prepare_full_pack.py` đóng gói/export/kiểm chứng; `full-pack-preview.template.html` và `verify_full_pack.cjs` sở hữu player/QA. README chính dẫn tới preview mới; README của full-pack mô tả nguồn và giới hạn.
- Kiếm White Idle/Attack1/Hurt/Die composite khớp RGBA từng cel trong `complete/`; Walk10/Attack2 đâm9 theo nguồn gốc. Yellow đổi màu xương theo palette pack. VFX kiếm tách từ hai sheet nguồn; riêng Attack1 cel5 có một pixel (39,36) bị source VFX xóa, nên Body giữ alpha0 ở đó để composite không đổi. Không tuyên bố Without_VFX khớp từng pixel sheet gốc.
- Rìu Idle một tay/cán đứng/tay phụ thả theo GIF; Walk có support/lift và sway nhẹ. Các Attack/Hurt/Die dựng lại toàn tay/thân/vũ khí. Cán32px cùng head/socket xuyên suốt; Attack1 cel5–10 đầu rìu ghim và mép sáng thật y63, smear chỉ cel4; cel14 thả tay phụ và cel15 khớp Idle0. Recovery100/120/140/140/120/100ms; giữ contact khoảng1s. Attack2 rìu quét ngang hai tay; thương đâm chính/đâm thấp. Hurt phản lực; Die buông và đặt vũ khí theo ngã, không khóa stance.
- Kỹ thuật PASS: exporter LibreSprite kiểm toàn bộ 6 native/từng lớp/composite roundtrip exact; tag/duration, alpha0/255, canvas96×64, bounds, grip/cán, chân chống/neo theo trạng thái, mép sáng chạm đất, palette vật liệu vũ khí White/Yellow và silhouette/alpha tương ứng. 1165 file Assets/complete/cluster/originals/weapon-milestone giữ hash. `full-pack/verification.json` lưu hash và `manifest.json` lưu pose/timing/provenance.
- Đã sửa hai lỗi phát hiện trong lúc kiểm: sọ Die vượt đáy ở cel7 (hạ vị trí đầu, không cắt pixel); cung smear rìu đi trên canvas (sửa quỹ đạo bàn tay trung gian, bỏ clamp). Kiếm Attack2 nguồn chủ ý dùng pixel x95, giữ đúng envelope nguồn; margin nghiêm ngặt chỉ áp dụng cel mới, không crop hoặc đổi pose nguồn để ép margin.
- Edge headless PASS10418 checks/288 tổ hợp: cả hai màu, mọi clip/vũ khí, mirror/VFX và1×/.25×; reset, biên bước cel, scrub, pause/resume/replay, loop/giữ cuối, VFX trên swing, overlay/onion,1280×720/640×360/nearest4×/8×. Matrix dùng tick kiểm deterministic mọi tổ hợp; đo rAF thực riêng trên Attack1 rìu:1×≈0.99196,0.25×≈0.24890, pause/resume và giữ cuối PASS. Không browser error.
- Đánh giá hình riêng: đã soi contact sheet mọi cel rìu/thương không VFX, Walk/Attack2 kiếm, Yellow thương, chuỗi bắt cán/contact/rời đất/thả tay; xem capture contact hai hướng ở kích thước chơi và nearest. Không nhận kết quả browser làm bằng chứng cảm giác lực đã được user duyệt. GIF công khai chỉ đủ dáng nghỉ rìu và động tác kiếm: các clip rìu ngoài Idle và toàn bộ thương là bản dựng theo plan/reference, không có bằng chứng khớp từng pixel Premium hoặc được quan sát playback liên tục bằng mắt.
- Python AST, JS syntax, HTML script syntax và git diff --check PASS; đã review diff trong phạm vi. Không sửa Assets, không Unity/WebGL/itch.io, không mua pack, không push/commit gộp checkout dirty. Dọn thư mục build/capture tạm của lượt này sau kiểm; giữ tool LibreSprite portable sẵn có. Bản thiết kế có đủ đầu ra và kiểm chứng kỹ thuật; giới hạn nguồn/chuyển động được ghi rõ trong README và preview.

## Checkpoint đồng bộ rìu với GIF gốc — 2026-10-18

- Yêu cầu được triển khai: chỉ bộ rìu thiết kế và preview; Idle White copy đúng9 cel×100ms từ skeleton giữa trong GIF overview. Các clip còn lại dùng cùng nguồn xương/vũ khí, giữ Walk10/Attack1 16/Attack2 9/Hurt5/Die13 và timing đã chốt. Không tích hợp gameplay. Checkpoint này thay mô tả Idle8/cán32px của rìu ở checkpoint trước.
- `axe_reference.py` kiểm từng khối4× trong vùng nguồn `(400,112)–(576,324)` đồng màu, bỏ nền RGB104/83/65 và hai skeleton bên cạnh; chuyển native về96×64 với chân y63, không resize nhân vật. Idle White cả hai VFX khớp RGBA/thứ tự từng cel với nguồn sau tách nền. Yellow phân loại riêng xương/thép/gỗ: mask đầu rìu đi theo độ nhún từng cel, tránh đổi highlight thép dùng cùng RGB250 với xương.
- `full_pack_poses.py` bỏ nhánh rìu32px và head đa giác cũ. Cán nguồn khoảng49px; các pose mới dựng cả vai/khuỷu/tay/thân/chân, raster đầu/cán/socket lấy từ GIF và inverse nearest theo trục. Idle/Walk một tay; Attack bắt tay phụ trước phát lực, thả trong recovery, cel cuối Attack1/Attack2/Hurt khớp Idle0 mới. Attack1 giữ contact khoảng1s, đầu rìu cố định cel5–10, thép sáng thật y63, smear chỉ cel4. Die thả tay cel4, vũ khí rơi riêng sang trước chân, giữ cuối.
- Đã sửa lệch trong quá trình QA: cán dài xuyên vùng sọ ở pose lấy đà cũ → dựng lại tư thế giơ tay/cúi thân; smear vượt mép trên → sửa quỹ đạo bàn tay trung gian, không clamp; vũ khí khi ngã đi qua vùng đầu → dựng lại đường rơi sau thả. Kiểm độc lập pixel vật liệu với raster native, envelope sọ xoay và bounds đều PASS. Pose thương trước/sau bỏ code rìu cũ được so cấu trúc và giữ nguyên.
- `prepare_full_pack.py --weapon axe` tái sử dụng manifest/PNG/native kiếm và thương; chỉ xuất24 PNG rìu và hai native62 cel, hai lớp Body + weapon/VFX, sáu tag. Bộ chung vẫn72 PNG/sáu native, tổng344 cel. Manifest cập nhật Idle9/900ms. Preview có ba vùng bản xuất/GIF/overlay silhouette cùng cel, cùng tỷ lệ, theo mirror; báo RGB/alpha và silhouette lệch0 pixel cho cả9 cel White.
- Kỹ thuật PASS với LibreSprite1.3: roundtrip composite và từng lớp, tag/duration, alpha0/255, canvas/bounds, grip, nguồn Idle, vật liệu vũ khí White/Yellow, sọ không giao vũ khí, contact/ghim đất và recovery. Mỗi lượt xuất giữ hash1217 file được bảo vệ gồm Assets, deliverable cũ và52 PNG/native kiếm/thương. So riêng52 file kiếm/thương với HEAD: byte-identical; metadata kiếm/thương và timing ngoài Idle của rìu giữ nguyên. Các thay đổi Assets/task khác đang diễn ra trong checkout được giữ nguyên, không đưa vào phạm vi này.
- Browser Edge headless:10506 checks/288 tổ hợp PASS, không page/console error; cả màu/clip/vũ khí, VFX/mirror,1×/0,25×; replay/pause/resume/scrub/bước cel/loop/giữ cuối và1280×720/640×360/nearest4×/8×. rAF thực Attack1 rìu đo1×≈0,99732;0,25×≈0,24360. Python AST, JS/HTML script syntax và whitespace PASS.
- Đánh giá hình tách riêng: soi cel sáu clip White không VFX, Yellow/mirror với VFX, key pose bắt cán/contact/giữ đất/nhấc/thả/ngã và capture contact hai hướng ở kích thước chơi/nearest. Cán, head/socket và tay nối theo pose; không thấy cắt vũ khí hay xuyên sọ trong các cel kiểm. Playback/tốc độ đã kiểm bằng browser; không tuyên bố đã quan sát liên tục bằng mắt hoặc cảm giác lực đã được user duyệt. Chỉ Idle có reference đủ để kết luận khớp chuyển động nguồn; không suy browser PASS ra các clip khác giống GIF/Premium.
- Đã cập nhật README full-pack và review diff trong phạm vi. Build/capture tạm của lượt đồng bộ được dọn sau QA; giữ LibreSprite portable/cache có sẵn. Không sửa runtime, không build Unity/WebGL, không commit/push hoặc mở PR.


## Checkpoint copy GIF đầy đủ — 2026-10-18

- User đã duyệt bộ `gif-pack` ngày 2026-10-18: “oke lưu lại đi tôi duyệt”. Lưu bộ này làm bản thiết kế đã duyệt cho ba skeleton; chưa yêu cầu tích hợp gameplay/Assets. Lưu commit riêng phần GIF và ghi nhận duyệt; các thay đổi sẵn có khác giữ ngoài commit.

- Hoàn thiện plan mới tại `design/skeleton-hero/gif-pack/`: kiếm, rìu, kiếm giáp/khiên; 18 strip PNG và ba native một lớp White composite/VFX. Mỗi native có 58 cel × 100ms, sáu tag theo thứ tự nguồn, giữ hold pose. Nguồn nguyên byte: `references/monopixelart-full.gif`, 1100×600, native grid 220×120. Không tạo Yellow, không dùng pose/timing tự dựng cũ.
- Đính chính kết luận thiếu nguồn rìu: overview 9 frame chỉ có Idle; GIF đầy đủ user cung cấp có toàn timeline rìu. Không cần ZIP để copy phần nhìn thấy. GIF composite không cung cấp layer gốc hoặc phần đã bị che.
- `prepare_gif_pack.py` sở hữu mask và đóng gói; `gif-pack-preview.template.html` dùng pattern controls project. `ownership.png` lưu nhãn từng pixel nguồn. Canvas 96×64, origin (0,18)/(64,18)/(128,18), chân y63. Giữ RGB và một phép dịch cho thân/vũ khí/VFX. Đã sửa ranh giới cho đầu vũ khí vượt lane, mảnh chết frame 13–35 và loại viền/đất/nền.
- Vùng che: frame41 VFX kiếm/cán rìu;42 VFX rìu/khiên/tay;50–51 đầu rìu/tay che kiếm;52 sweep rìu che khiên;53–54 đầu rìu chạm vùng khiên. VFX kiếm+khiên ở41–42/50–52 chạm mép phải GIF, thiếu dữ liệu ngoài ảnh. Giao trắng cùng màu không cho phép phân định layer duy nhất; mask theo mép nhìn thấy, không vẽ bù. Không tuyên bố khớp layer Premium chưa có.
- `verify_gif_pack.py --libresprite ...` PASS: toàn bộ khối5× đồng màu, 174 cel RGB/alpha/silhouette exact theo mask nguồn; alpha0/255, canvas, thứ tự100ms. LibreSprite roundtrip ba native/18 strip/sáu tag. Export giữ SHA256 của397 file thiết kế/công cụ có sẵn; manifest ghi số file và digest tập hash. README chính sửa riêng có chủ ý; deliverable cũ giữ nguyên.
- `verify_gif_pack.cjs` với Edge headless PASS168 tổ hợp viewport1280×720/640×360, ba nhân vật, bảy lựa chọn clip/timeline, nearest4×/8×, mirror. Scrub/step/replay/pause/resume/cả hai tốc độ đạt, không page error. Đã soi contact sheet attack/death và screenshot preview; source/mask roundtrip không chứng minh ownership duy nhất ở giao trắng hoặc phục hồi phần bị che.
- Không sửa Assets/gameplay/API, không chạy Unity/WebGL, không commit/push. Giữ checkout dirty có sẵn. Output QA tạm dọn sau verification; LibreSprite/Playwright dùng lại tool có sẵn.
