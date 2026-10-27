# Rà soát độ đồng bộ pixel trên main — 2026-10-27

## Phạm vi và revision

Audit bắt đầu trên `origin/main` tại `4db567bada8c7fcc512c426540941a4b894052eb`.
Fetch cuối trước tích hợp: `a66580cfdfba5d66b15771f11060878b3d2ac773`.
Delta chỉ gồm tài liệu duel-intro và receipt marketing; `git diff 4db567b
a66580cf -- Assets Packages ProjectSettings` rỗng. Vì vậy bằng chứng build/render
bên dưới vẫn áp dụng cho mã và cấu hình game của main mới nhất tại lúc chốt audit.
Chỉ báo cáo; không sửa game, artwork, public API hoặc chuẩn thiết kế.
Checkout đang sửa ở `C:/Users/ADMIN/orca/TypingHero` được giữ nguyên.
Snapshot LF dùng cho Unity: 2.495 file, SHA256
`0c94ea6c64c47de94c19842ec6599ace5281b27459c5780c98b8e995c6c9cbc7`.
Nguồn tham chiếu là thiết kế và asset hiện hành tại revision trên; không đánh giá
độ giống một game khác. Ảnh/clip/log chỉ dùng tạm; sản phẩm lưu giữ duy nhất là
báo cáo Markdown này. Định danh revision, asset, key và thời điểm dưới đây thay
cho liên kết ảnh tạm sẽ bị xóa.

Checklist nghiệm thu:

- Menu, nhập tên, tutorial hiện hành, intro, combat, pause và chuyển màn.
- Sprite player và ba bộ kiếm/rìu/kiếm–khiên; idle, hai attack của đối thủ,
  Hurt, death; normal, parry, Slayer, sorting và che lấp.
- World 320×180, presentation, letterbox; follow, zoom, shake và anchor.
- Canvas 960×540, viền/padding, title/UI/typing/fallback và độ sắc nét.
- Normal/depth/material map, mirror, bóng, reflection, blur và khả năng đọc.
- 1280×720 là kích thước chính, 1366×768 chỉ bổ sung kiểm phóng hình không nguyên.

## Kết luận hiện tại

Không phát hiện lỗi đồng bộ pixel trong gameplay đã được chứng minh qua nguồn và
17 ảnh runtime đã trực tiếp xem. Có một nghi vấn asset rìu (A1), chưa chứng minh
xuất hiện trong gameplay. Những khác biệt phối cảnh, palette, SDF, bloom, blur và
reflection có căn cứ thiết kế; không đề nghị tắt chúng chỉ để đồng nhất lưới pixel.

**Chưa đủ bằng chứng để kết luận toàn bộ phạm vi animation/camera đạt**: chưa
review video runtime từng frame, toàn bộ mirror, playback và state nối. Các
khoảng trống được liệt kê riêng; không dùng build hoặc receipt lịch sử để lấp chúng.

## Những khác biệt có chủ ý, không tự coi là lỗi

| Thành phần | Nguồn và hành vi | Đánh giá |
|---|---|---|
| World/presentation | `Assets/Data/Lighting/DungeonLighting.asset`: 320×180, ARGBHalf, pitch 5°, FOV 35°, normalOrtho 5.625. `DungeonArenaCamera.cs` tạo RT Point/Clamp, không mip, MSAA 1, không dynamic scale. `DungeonPresent.shader` dùng `sampler_PointClamp`. | Pixel world cố định; phối cảnh là thiết kế 2.5D. |
| Camera | `DungeonArenaCamera.cs` dùng SmoothDamp follow giới hạn 0.5, smooth 0.35; FOV thay theo impact/Slayer, giữ landmark chân khi impact zoom. | Không có snapping world toàn cục. Đây chưa phải bằng chứng rung sai thiết kế. |
| Viewport | `CombatR7Build.cs` phủ combat 960×540; `DungeonArenaBuild.cs` áp profile/phối cảnh sau legacy theme. `CombatViewportCamera.cs` lấy góc UI đổi sang rect presentation. | Không dùng cấu hình 928×340 của bước Shogun cũ để kết luận sai tỉ lệ. |
| Hai lưới world/UI | `PixelUIFactory.cs`: canvas overlay, pixelPerfect, ScaleWithScreenSize/Expand 960×540; world 320×180. | UI có độ phân giải cao hơn world theo thiết kế, không bắt font/viền bằng kích thước texel nhân vật. |
| Font | `BlackletterTitleBuild.cs`: title specimen SDF cạnh cứng; UI/typing CobblestoneUI64 + CobblestoneVT323Fallback dùng TMP Mobile Distance Field/Bilinear. `BlackletterPixelSnap.cs` chỉ snap dấu `+` của specimen. | UI/font fallback mềm hơn title có nguồn cấu hình rõ ràng; không gọi Bilinear của SDF là lỗi lọc sprite. |
| Blur | `DungeonWorldRender.cs` blur environment sau skybox, rồi mới vẽ `DungeonActor`; radius 0.65, focusHalfWidth 0.75, depthEdgeDistance 0.5. | Không suy ra nhân vật bị blur từ tên pass. |
| Reflection | `DungeonPlanarReflection.cs` capture half resolution, Bilinear, không post/shadow, ẩn receiver tránh recursion, tắt khi cinematic. | Phản chiếu mềm có chủ ý. |
| Bloom | `DungeonPresentationRender.cs` present trước hậu xử lý, letterbox sau FinalBlit và trước overlay. | Bloom có thể phá tính đồng màu trong block pixel; không mặc định coi đó là lỗi upscale. |
| Mirror | `DungeonSprite.shader` đổi basis normal bằng `_DepthFacing` khi có depth, `_Flip` khi chưa có depth. | Có xử lý mirror trong nguồn; chưa chứng minh mọi cel/mirror đúng bằng quan sát. |

## Rà asset và đường tham chiếu

Lần GUID, gồm secondary texture trong metadata, từ MainMenu, SampleScene, DungeonWorld và các Resources asset: 661 asset
có thể tới được, gồm 222 texture nguồn; đây là tập bao phủ rộng, có cả đối tượng
legacy bị tắt, không phải 222 texture đều xuất hiện trên màn hình. Trong tập này
222/222 TextureImporter có Point và mipmap tắt; PPU có 16/32/100. Không tìm thấy
SpriteAtlas trong đồ thị GUID này. Kết quả không bao gồm texture nhúng trong font
asset, mesh/material tải qua cơ chế khác hoặc kết quả packing thực tế trong player.

Không có texture trong tập trên vượt giới hạn maxTextureSize WebGL hiệu lực theo
metadata (0/222 có kích thước nguồn lớn hơn giới hạn; kiểm bằng kích thước ảnh và
override đang bật, không chỉ đọc default). Đây không thay kiểm imported texture
thực tế, compression format do Unity chọn hoặc lỗi bleeding ở UV khi chạy.

99 binding secondary texture trong tập này trỏ tới file tồn tại, có kích thước
nguồn bằng ảnh chính; không có mismatch kích thước. Build preprocess đồng bộ
281 sprite depth, 0 missing, và qua kiểm tra dựng lặp. Hai kết quả này không
chứng minh normal/depth đẹp hoặc đúng giải phẫu ở mọi cel và hướng mirror.

`EndlessAssets.cs` nối sáu role vào ba controller kiếm/rìu/kiếm–khiên, cắt cel
96×64, PPU16, pivot (0.47, 0.41), 10fps; override WebGL/Standalone RGBA32.
Attack1 contact index: kiếm/khiên 5, rìu 6; Attack2: kiếm/khiên 2, rìu 4.
Hurt dùng chung cho hit/stun/hitBySlayer. Không đánh giá một cel flash trắng,
vệt kiếm rời hoặc skeleton rã xương là lỗi chỉ vì silhouette khác idle.

Đã mở trực tiếp đủ 265 key-cel của 28 clip nguồn, cùng 244 cặp liên tiếp/loop;
giữ key lặp và duration cuối, không gộp theo PNG giống nhau. Chi tiết từng key ở
phụ lục. Đây là review hình nguồn nearest 4×, chưa phải review đầy đủ mọi frame
rendered, playback và state nối. Các variant mirror, VFX và ánh sáng runtime chưa
được chứng nhận bằng số đếm này.

## Nghi vấn cần phân biệt với lỗi trong gameplay

### A1 — Mảng nâu rời trong nguồn rìu Walk (asset quan sát được, runtime chưa chứng minh)

- Vị trí: `Assets/Art/EndlessSkeletons/axe/Walk.png`, key/cel F6–F11,
  clip `Walk.anim` tại 0.6–1.2s. Một mảng nâu tách khỏi chân và cán rìu xuất hiện
  bên phải, lớn dần rồi nằm ngang sát đáy cel. `axe/Die.png` F0 cũng có mảnh nâu
  riêng phía phải. Quan sát trực tiếp toàn bộ cel, không suy ra từ thống kê alpha.
- Điều kiện: phát clip nguồn trên canvas đủ 96×64, không crop riêng thân.
- Tác động có thể có: vật thể thừa nhấp nháy sát chân nếu Walk được phát; chưa có
  bằng chứng nó xuất hiện trong menu/tutorial/combat hiện tại.
- Giới hạn: `axe/Combat.controller` có state `skeleton_walk` nhưng không có
  transition tới Walk; tìm trong runtime C# không thấy lệnh gọi Walk. Không xếp
  thành lỗi gameplay đã tái hiện. Mảnh Die cần kiểm cả trình tự runtime trước khi
  gọi là pixel rác thay vì vật rơi từ nguồn.
- Hướng xử lý nếu được xác nhận: đối chiếu strip gốc và rect/canvas extraction;
  sửa riêng phần extraction/cel gây lẫn nếu mảng không thuộc reference. Chưa sửa.

## Kiểm chứng runtime

Unity 2022.3.25f1 Windows: QA WebGL build thành công, exit 0, strict repeat-builder
qua kiểm tra. Invocation `20261027T025039.475518Z-095fa6437ce04e518f0f0089b58912e6-build-right`.
Player QA 33 file; SHA256 manifest được tính bằng nối các dòng đã sort
`relative-path + NUL + hex-SHA256(file) + LF`:
`43c9c144df6f8f07811c3b088cf6a919c6a55f496e4e7d7885ae00e38e215963`.
Không tạo production build hoặc phát hành game.

Native Edge `156.0.4314.42`, Playwright từ runtime hiện có; ANGLE NVIDIA GTX1650Ti
D3D11. Lượt driver cuối hoàn tất 136.9996s, 4 assertion qua (render size, cú gõ
thật tạo score, không JS exception, không runtime exception), 0 lỗi Unity/console,
0 Firebase đọc/ghi, 0 external request phải chặn; fixture/network isolation vẫn bật.
Đây là luồng quan sát hình có phạm vi, không phải full gameplay/balance suite.
Đi tới SWORDSMAN → DOUBLE STRIKE → HEAVY → GUARD → ELITE/SWORDSMAN bằng bàn phím/
chuột; dừng sau 4 đối thủ, HP25, score2185. Đã có parry thật và Slayer thật.
Không suy ra boss/victory hoặc mọi combo animation đã đạt từ kết quả này.

Hai pilot không được tính là lượt qua: pilot đầu chờ encounterReading theo luồng
intro cũ, trong khi revision hiện tại tự vào trận; pilot thứ hai xong phần scoped
nhưng harness tự chạy thêm luồng Slayer cũ rồi timeout. Chỉ sửa driver tạm để theo
luồng hiện hành và tắt phần chạy thêm; không đổi game/timing để làm test qua.

Đã mở trực tiếp 17/17 ảnh final sau đây. Cột frame là probe đọc **sau capture**,
không phải bằng chứng frame-exact của một clip. Chữ `F6 Spark: BCompact` ở góc
trên trái thuộc QA, không phải lỗi UI production.

| Capture tạm | Frame probe | Quan sát trực tiếp |
|---|---:|---|
| menu-1280 | 54 | Title và bốn action rõ, khoảng trống không cắt chữ; phòng tối/ánh lửa mềm. |
| menu-1366 | 90 | Giữ bố cục, không lộ dải nội dung cũ ở cạnh; nhịp texel thay đổi theo phóng không nguyên. |
| name-1280 | 146 | Input, difficulty, Back/Start trong panel; padding/viền không đè chữ. |
| guide-1280 | 318 | Các card/hướng mũi tên và typing tray còn đọc được; card che world theo chủ ý overlay. |
| guide-1366 | 363 | Không tràn copy hoặc chồng card; viền UI nhỏ hơn texel world là hai hệ render khác nhau. |
| combat-1280 | 504 | Chân/khăn, kiếm và skeleton còn đọc được; UI/tray không che thân ở pose này. |
| pause-1280 | 549 | Các action và PAUSE rõ; nền game được phủ tối, không thấy cắt nhãn. |
| pause-1366 | 585 | Cấu trúc/padding tương tự, không xuất hiện seam nền ngoài viewport. |
| combat-1366 | 657 | Cue SPACE, thân và chân còn tách được; không coi cột 4/5px là lỗi artwork. |
| hit-1366 | 737 | Score10/cú gõ thật; pose cúi vẫn trong arena, tray không dời theo người. |
| opponent-0 | 800 | Swordsman và player cùng cảnh, silhouette đọc được. |
| parry-1280 | 1004 | Flash sáng ở điểm tiếp xúc; body/kiếm vẫn nhận dạng được ngoài lõi flash, HUD đọc được. |
| slayer-1280 | 1794 | Hai silhouette đen trên nền đỏ, thanh đen/HUD/tray còn đọc được; không gọi mất shading là lỗi. |
| opponent-1 | 4196 | Double Strike dùng cùng bộ kiếm, chân/pivot không có sai lệch rõ ở ảnh này. |
| opponent-2 | 5919 | Heavy có đầu rìu sáng do ánh kim/bloom; cán và thân tách được, không thấy mảng Walk rời. |
| opponent-3 | 8185 | Guard có khiên trước thân, mũ và kiếm rõ; không có vật rời quanh chân ở pose này. |
| opponent-4 | 9522 | Elite/Swordsman, tên dài và icon elite vẫn nằm trong HUD; không cắt nhãn. |

World RT thực tế trong cả 17 snapshot là 320×180, renderScale1, một presentation
camera. 1280×720 có scale world 4×; 1366×768 giữ 16:9 nên hệ số khoảng 4.2667×,
các cột/hàng nearest có bề rộng luân phiên 4/5px. Chưa thấy biến dạng cần sửa trong
các ảnh đã xem. Không bảo đảm mọi texel cuối cùng là một block đồng màu vì có
hậu xử lý sau Point presentation.

## Phần chưa kiểm chứng và bước xác nhận cần thiết

| Phần | Bằng chứng còn thiếu | Cách xác nhận trong lượt tiếp theo nếu cần chốt toàn phạm vi |
|---|---|---|
| Rung cạnh/follow/zoom/shake, trượt anchor | Ảnh tĩnh và nguồn không chứng minh continuity. | Ghi đoạn đầy đủ idle → normal hit/received hit/parry → hồi phục, entry/exit Slayer; xem mọi frame/chuyển tiếp ở kích thước chơi và nearest zoom, giữ frame lặp/timing. |
| Mọi animation/mirror/VFX | Đã xem 265 cel nguồn một hướng; chưa playback 1×/chậm, không có video runtime được review từng frame; các clip tutorial gốc ngoài bảng và state nối chưa review đủ. Số frame/transition video đã kiểm: 0; không có đoạn video nào được chọn làm bằng chứng. | Ghi rõ clip/biến thể, mốc đầu/cuối toàn hành vi và tổng frame/transition trước khi review; bổ sung từng hàng cho output cuối. Không dùng 17 screenshot để gọi animation đạt. |
| Normal/depth/bóng/phản chiếu | Binding/kích thước khớp, ảnh sample không chứng minh mọi hướng hoặc mọi pose đúng. | Review đủ cel/mirror trong cùng ánh sáng; phân biệt highlight có chủ ý với normal đảo hoặc bóng sai hình. |
| Atlas bleeding và imported compression | Nguồn không có dấu hiệu mip/filter sai; chưa đọc mọi imported texture/packing trong player. | Chẩn đoán imported rect/format/UV đúng asset khi có ảnh hoặc frame tái hiện seam/halo. |
| Font fallback và mọi nhãn động | Đã xem nhãn hiện ở 17 capture; chưa ép đầy bộ fallback/ký tự hoặc mọi trạng thái chữ động. | Dùng input được game cho phép để ép glyph cần kiểm, xem glyph thật và clipping tại hai kích thước; không đổi bộ font chỉ vì khác shader. |

Không đề nghị sửa game trong audit này. Ưu tiên xác nhận A1 nếu Walk sẽ được dùng;
ưu tiên video runtime nếu cần quyết định về shimmer/anchor hoặc nghiệm thu animation
toàn bộ. Không nâng các nghi vấn/phần chưa xem thành lỗi đã chứng minh.

## Phụ lục: quan sát cel nguồn, không phải nghiệm thu animation runtime
Nguồn đối chiếu là asset tại revision audit; không có game/reference bên ngoài được yêu cầu. Các sheet nearest 4× đã mở trực tiếp đủ từng cel. Dấu hồng trên sheet là pivot do công cụ review vẽ, không thuộc asset. Chưa xem playback 1×/chậm, mirror, toàn bộ state nối và mọi tổ hợp VFX; vì thế trạng thái nghiệm thu từng hàng giữ **chưa kiểm chứng**. Nhận xét dưới đây chỉ mô tả phần hình đã trực tiếp xem; không suy ra từ script. Bằng chứng được định danh bằng đường asset + key index + thời điểm tại revision, ảnh tạm không giữ sau audit.

### Assets/Characters/Player/attack1.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.166667 | contact thấp, kiếm kéo phải | → F1: giữ kiếm thấp | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, key 0 |
| F1 | 0.166667 / 0.0666667 | giữ kiếm thấp | → F2: kiếm ngắn lại | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, key 1 |
| F2 | 0.233333 / 0.05 | kiếm ngắn lại | → F3: thu kiếm chéo | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, key 2 |
| F3 | 0.283333 / 0.0833333 | thu kiếm chéo | → F4: thu về hông | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 3 |
| F4 | 0.366667 / 0.1 | thu về hông | → F5: mảnh sáng còn bên phải | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 4 |
| F5 | 0.466667 / 0.0833333 | mảnh sáng còn bên phải | → F6: mảnh sáng cuối, thân đứng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 5 |
| F6 | 0.55 / 0.0166667 | mảnh sáng cuối, thân đứng | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 6 |

### Assets/Characters/Player/parry_followup.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.0548387 | thân nghiêng, chuẩn bị phản kích | → F1: tay nâng gần mặt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 0 |
| F1 | 0.0548387 / 0.0548387 | tay nâng gần mặt | → F2: kiếm quét phía sau | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 1 |
| F2 | 0.109677 / 0.0548387 | kiếm quét phía sau | → F3: tay dang ngang | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 2 |
| F3 | 0.164516 / 0.0548387 | tay dang ngang | → F4: kiếm nâng chéo | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 3 |
| F4 | 0.219355 / 0.0548387 | kiếm nâng chéo | → F5: thu kiếm sau vai | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 4 |
| F5 | 0.274194 / 0.0731183 | thu kiếm sau vai | → F6: kiếm thành vệt ngắn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 5 |
| F6 | 0.347312 / 0.0731183 | kiếm thành vệt ngắn | → F7: vệt còn hai đoạn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 6 |
| F7 | 0.42043 / 0.0731183 | vệt còn hai đoạn | → F8: về đứng, khăn trái | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 7 |
| F8 | 0.493548 / 0.0564516 | về đứng, khăn trái | → F9: giữ đứng cuối | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 8 |
| F9 | 0.55 / 0.0166667 | giữ đứng cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 9 |

### Assets/Characters/Player/player_die.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.0666667 | co gối, tay trước ngực | → F1: cúi đầu | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 0 |
| F1 | 0.0666667 / 0.0833333 | cúi đầu | → F2: cuộn người | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 1 |
| F2 | 0.15 / 0.0666667 | cuộn người | → F3: thân hạ | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 2 |
| F3 | 0.216667 / 0.0833333 | thân hạ | → F4: giữ co | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 3 |
| F4 | 0.3 / 0.0333333 | giữ co | → F5: khăn cuộn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 4 |
| F5 | 0.333333 / 0.116667 | khăn cuộn | → F6: chuẩn bị bật lên | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 5 |
| F6 | 0.45 / 0.05 | chuẩn bị bật lên | → F7: ngửa đầu nâng chân | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 6 |
| F7 | 0.5 / 0.0833333 | ngửa đầu nâng chân | → F8: giữ pose ngửa | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 7 |
| F8 | 0.583333 / 0.05 | giữ pose ngửa | → F9: thân sáng hơn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 8 |
| F9 | 0.633333 / 0.1 | thân sáng hơn | → F10: bắt đầu trắng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 9 |
| F10 | 0.733333 / 0.05 | bắt đầu trắng | → F11: tăng trắng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 10 |
| F11 | 0.783333 / 0.0666666 | tăng trắng | → F12: gần trắng toàn thân | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 11 |
| F12 | 0.85 / 0.1 | gần trắng toàn thân | → F13: silhouette trắng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 12 |
| F13 | 0.95 / 0.05 | silhouette trắng | → F14: giữ silhouette | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 13 |
| F14 | 1 / 0.0833334 | giữ silhouette | → F15: silhouette kéo dọc | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 14 |
| F15 | 1.08333 / 0.0499999 | silhouette kéo dọc | → F16: tách vệt dọc | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 15 |
| F16 | 1.13333 / 0.116667 | tách vệt dọc | → F17: vệt dài | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 16 |
| F17 | 1.25 / 0.0333333 | vệt dài | → F18: vệt rỗng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 17 |
| F18 | 1.28333 / 0.0833334 | vệt rỗng | → F19: vỡ mảnh | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 18 |
| F19 | 1.36667 / 0.0666666 | vỡ mảnh | → F20: mảnh thưa | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 19 |
| F20 | 1.43333 / 0.0833334 | mảnh thưa | → F21: mảnh nhỏ | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 20 |
| F21 | 1.51667 / 0.0666667 | mảnh nhỏ | → F22: mảnh cuối | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 21 |
| F22 | 1.58333 / 0.0166666 | mảnh cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-death.png`, key 22 |

### Assets/Characters/Player/player_hit.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.0833333 | tint đỏ, bật lùi | → F1: đỏ giảm, chân co | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-hurt.png`, key 0 |
| F1 | 0.0833333 / 0.0833333 | đỏ giảm, chân co | → F2: hết đỏ, chân còn nâng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-hurt.png`, key 1 |
| F2 | 0.166667 / 0.1 | hết đỏ, chân còn nâng | → F3: hạ chân, tay mở | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-hurt.png`, key 2 |
| F3 | 0.266667 / 0.0833333 | hạ chân, tay mở | → F4: cúi thân phục hồi | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-hurt.png`, key 3 |
| F4 | 0.35 / 0.0833333 | cúi thân phục hồi | → F5: đứng lại | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-hurt.png`, key 4 |
| F5 | 0.433333 / 0.0166667 | đứng lại | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-hurt.png`, key 5 |

### Assets/Characters/Player/player_idle.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.0666667 | đứng, khăn cong lên | → F1: khăn hạ nhẹ | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 0 |
| F1 | 0.0666667 / 0.05 | khăn hạ nhẹ | → F2: khăn ngang | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 1 |
| F2 | 0.116667 / 0.05 | khăn ngang | → F3: khăn quét xuống | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 2 |
| F3 | 0.166667 / 0.05 | khăn quét xuống | → F4: khăn nâng gốc | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 3 |
| F4 | 0.216667 / 0.0666667 | khăn nâng gốc | → F5: khăn cuộn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 4 |
| F5 | 0.283333 / 0.05 | khăn cuộn | → F6: đuôi khăn tách hai | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 5 |
| F6 | 0.333333 / 0.05 | đuôi khăn tách hai | → F7: khăn xòe trái | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 6 |
| F7 | 0.383333 / 0.05 | khăn xòe trái | → F8: khăn co | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 7 |
| F8 | 0.433333 / 0.0666667 | khăn co | → F9: trở về khăn ngang | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 8 |
| F9 | 0.5 / 0.0166667 | trở về khăn ngang | → F0: đứng, khăn cong lên | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, key 9 |

### Assets/Characters/Player/player_parry.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.25 | kiếm chắn trước mặt | → F1: kiếm hạ nhẹ | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 0 |
| F1 | 0.25 / 0.0666667 | kiếm hạ nhẹ | → F2: kiếm tan thành vệt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 1 |
| F2 | 0.316667 / 0.0666667 | kiếm tan thành vệt | → F3: vệt rời, thân đứng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 2 |
| F3 | 0.383333 / 0.0166667 | vệt rời, thân đứng | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 3 |

### Assets/Characters/Player/player_parry_success.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.8 | cúi thấp giữ kiếm, held 800ms | → F1: kiếm ngang sau vai | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3.png`, key 0 |
| F1 | 0.8 / 0.0333333 | kiếm ngang sau vai | → F2: kiếm dựng chéo | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-parry_lift.png`, key 1 |
| F2 | 0.833333 / 0.0333334 | kiếm dựng chéo | → F3: tay quét sau | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3.png`, key 2 |
| F3 | 0.866667 / 0.0333333 | tay quét sau | → F4: kiếm thành vệt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 3 |
| F4 | 0.9 / 0.0166667 | kiếm thành vệt | → F5: vệt còn hai đoạn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 4 |
| F5 | 0.916667 / 0.0166666 | vệt còn hai đoạn | → F6: đứng, hết vệt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 5 |
| F6 | 0.933333 / 0.0166667 | đứng, hết vệt | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 6 |

### Assets/Characters/Player/player_slayer/attackS1.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.04 | contact thấp, kiếm phải | → F1: giữ kiếm thấp | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, key 0 |
| F1 | 0.04 / 0.04 | giữ kiếm thấp | → F2: kiếm co | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, key 1 |
| F2 | 0.08 / 0.04 | kiếm co | → F3: thu kiếm chéo | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1.png`, key 2 |
| F3 | 0.12 / 0.04 | thu kiếm chéo | → F4: kiếm về hông | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 3 |
| F4 | 0.16 / 0.04 | kiếm về hông | → F5: mảnh sáng bên phải | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 4 |
| F5 | 0.2 / 0.04 | mảnh sáng bên phải | → F6: mảnh cuối | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 5 |
| F6 | 0.24 / 0.03 | mảnh cuối | → F7: lặp mảnh cuối, held 10ms | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 6 |
| F7 | 0.27 / 0.01 | lặp mảnh cuối, held 10ms | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_1_end.png`, key 7 |

### Assets/Characters/Player/player_slayer/attackS2.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.04 | kiếm ngang sau vai | → F1: kiếm nâng nhẹ | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 0 |
| F1 | 0.04 / 0.04 | kiếm nâng nhẹ | → F2: kiếm dựng chéo | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 1 |
| F2 | 0.08 / 0.04 | kiếm dựng chéo | → F3: tay dang ngang | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2.png`, key 2 |
| F3 | 0.12 / 0.04 | tay dang ngang | → F4: kiếm thành vệt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 3 |
| F4 | 0.16 / 0.04 | kiếm thành vệt | → F5: vệt ngắn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 4 |
| F5 | 0.2 / 0.04 | vệt ngắn | → F6: đứng, hết vệt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 5 |
| F6 | 0.24 / 0.03 | đứng, hết vệt | → F7: lặp đứng, held 10ms | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 6 |
| F7 | 0.27 / 0.01 | lặp đứng, held 10ms | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_2_end.png`, key 7 |

### Assets/Characters/Player/player_slayer/attackS3.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.04 | kiếm chéo trước mặt | → F1: nâng thẳng kiếm | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3.png`, key 0 |
| F1 | 0.04 / 0.04 | nâng thẳng kiếm | → F2: kiếm thẳng, thân xoay | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 1 |
| F2 | 0.08 / 0.04 | kiếm thẳng, thân xoay | → F3: về pose chắn | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 2 |
| F3 | 0.12 / 0.04 | về pose chắn | → F4: kiếm hạ | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 3 |
| F4 | 0.16 / 0.04 | kiếm hạ | → F5: kiếm thành vệt | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 4 |
| F5 | 0.2 / 0.04 | kiếm thành vệt | → F6: vệt rời, thân đứng | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 5 |
| F6 | 0.24 / 0.03 | vệt rời, thân đứng | → F7: lặp vệt cuối, held 10ms | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 6 |
| F7 | 0.27 / 0.01 | lặp vệt cuối, held 10ms | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png`, key 7 |

### Assets/Art/EndlessSkeletons/axe/Attack1.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | rìu thẳng | → F1: xoay rìu chéo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 0 |
| F1 | 0.1 / 0.1 | xoay rìu chéo | → F2: rìu nằm ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 1 |
| F2 | 0.2 / 0.1 | rìu nằm ngang | → F3: rìu sau thấp | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 2 |
| F3 | 0.3 / 0.1 | rìu sau thấp | → F4: rìu chéo xuống | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 3 |
| F4 | 0.4 / 0.1 | rìu chéo xuống | → F5: giữ thấp | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 4 |
| F5 | 0.5 / 0.1 | giữ thấp | → F6: bổ ngược lên, vệt trắng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 5 |
| F6 | 0.6 / 0.1 | bổ ngược lên, vệt trắng | → F7: rìu cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 6 |
| F7 | 0.7 / 0.1 | rìu cao | → F8: thu rìu chéo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 7 |
| F8 | 0.8 / 0.1 | thu rìu chéo | → F9: rìu gần thẳng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 8 |
| F9 | 0.9 / 0.1 | rìu gần thẳng | → F10: về đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 9 |
| F10 | 1 / 0.1 | về đứng | → F11: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 10 |
| F11 | 1.1 / 0.1 | giữ đứng | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack1.png`, key 11 |

### Assets/Art/EndlessSkeletons/axe/Attack2.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | rìu chéo trước đầu | → F1: kéo rìu sau | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 0 |
| F1 | 0.1 / 0.1 | kéo rìu sau | → F2: xoay rìu thấp | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 1 |
| F2 | 0.2 / 0.1 | xoay rìu thấp | → F3: rìu lùi trái | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 2 |
| F3 | 0.3 / 0.1 | rìu lùi trái | → F4: quét ngang, vệt dài | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 3 |
| F4 | 0.4 / 0.1 | quét ngang, vệt dài | → F5: giữ rìu ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 4 |
| F5 | 0.5 / 0.1 | giữ rìu ngang | → F6: rìu thu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 5 |
| F6 | 0.6 / 0.1 | rìu thu | → F7: nâng rìu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 6 |
| F7 | 0.7 / 0.1 | nâng rìu | → F8: gần thẳng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 7 |
| F8 | 0.8 / 0.1 | gần thẳng | → F9: đứng lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 8 |
| F9 | 0.9 / 0.1 | đứng lại | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Attack2.png`, key 9 |

### Assets/Art/EndlessSkeletons/axe/Die.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | đứng, mảnh nâu phải | → F1: sọ và ngực rời | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 0 |
| F1 | 0.1 / 0.1 | sọ và ngực rời | → F2: eo rơi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 1 |
| F2 | 0.2 / 0.1 | eo rơi | → F3: chân đổ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 2 |
| F3 | 0.3 / 0.1 | chân đổ | → F4: sọ hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 3 |
| F4 | 0.4 / 0.1 | sọ hạ | → F5: rìu nghiêng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 4 |
| F5 | 0.5 / 0.1 | rìu nghiêng | → F6: rìu đổ ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 5 |
| F6 | 0.6 / 0.1 | rìu đổ ngang | → F7: sọ sát sàn | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 6 |
| F7 | 0.7 / 0.1 | sọ sát sàn | → F8: sọ nằm | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 7 |
| F8 | 0.8 / 0.1 | sọ nằm | → F9: giữ xương | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 8 |
| F9 | 0.9 / 0.1 | giữ xương | → F10: held đống xương cuối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 9 |
| F10 | 1 / 0.1 | held đống xương cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Die.png`, key 10 |

### Assets/Art/EndlessSkeletons/axe/Hurt.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | đứng | → F1: flash trắng, lùi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Hurt.png`, key 0 |
| F1 | 0.1 / 0.1 | flash trắng, lùi | → F2: hết flash, rìu thẳng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Hurt.png`, key 1 |
| F2 | 0.2 / 0.1 | hết flash, rìu thẳng | → F3: đầu hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Hurt.png`, key 2 |
| F3 | 0.3 / 0.1 | đầu hạ | → F4: đứng lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Hurt.png`, key 3 |
| F4 | 0.4 / 0.1 | đứng lại | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Hurt.png`, key 4 |

### Assets/Art/EndlessSkeletons/axe/Idle.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | rìu thẳng, chân trụ | → F1: thân nhích | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 0 |
| F1 | 0.1 / 0.1 | thân nhích | → F2: đầu hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 1 |
| F2 | 0.2 / 0.1 | đầu hạ | → F3: thân hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 2 |
| F3 | 0.3 / 0.1 | thân hạ | → F4: đầu nâng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 3 |
| F4 | 0.4 / 0.1 | đầu nâng | → F5: về cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 4 |
| F5 | 0.5 / 0.1 | về cao | → F6: giữ cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 5 |
| F6 | 0.6 / 0.1 | giữ cao | → F7: về pose đầu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 6 |
| F7 | 0.7 / 0.1 | về pose đầu | → F0: rìu thẳng, chân trụ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Idle.png`, key 7 |

### Assets/Art/EndlessSkeletons/axe/Walk.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | chân mở | → F1: chân chuyển | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 0 |
| F1 | 0.1 / 0.1 | chân chuyển | → F2: nhấc gối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 1 |
| F2 | 0.2 / 0.1 | nhấc gối | → F3: đổi chân trụ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 2 |
| F3 | 0.3 / 0.1 | đổi chân trụ | → F4: chân khép | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 3 |
| F4 | 0.4 / 0.1 | chân khép | → F5: chân mở rộng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 4 |
| F5 | 0.5 / 0.1 | chân mở rộng | → F6: chân bắt chéo, mảnh nâu phải | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 5 |
| F6 | 0.6 / 0.1 | chân bắt chéo, mảnh nâu phải | → F7: chân thẳng, mảnh nâu rộng hơn | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 6 |
| F7 | 0.7 / 0.1 | chân thẳng, mảnh nâu rộng hơn | → F8: chuyển chân, mảnh nâu to | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 7 |
| F8 | 0.8 / 0.1 | chuyển chân, mảnh nâu to | → F9: chân mở, mảnh nâu to | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 8 |
| F9 | 0.9 / 0.1 | chân mở, mảnh nâu to | → F10: giữ bước, mảnh nâu sát sàn | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 9 |
| F10 | 1 / 0.1 | giữ bước, mảnh nâu sát sàn | → F11: về chân mở, mảnh nâu ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 10 |
| F11 | 1.1 / 0.1 | về chân mở, mảnh nâu ngang | → F0: chân mở | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/axe/Walk.png`, key 11 |

### Assets/Art/EndlessSkeletons/sword/Attack1.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | kiếm dựng chéo | → F1: kéo kiếm về đầu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 0 |
| F1 | 0.1 / 0.1 | kéo kiếm về đầu | → F2: kiếm chéo sau gáy | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 1 |
| F2 | 0.2 / 0.1 | kiếm chéo sau gáy | → F3: gồng ngang vai | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 2 |
| F3 | 0.3 / 0.1 | gồng ngang vai | → F4: giữ chuẩn bị | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 3 |
| F4 | 0.4 / 0.1 | giữ chuẩn bị | → F5: chém ngang, vệt trắng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 4 |
| F5 | 0.5 / 0.1 | chém ngang, vệt trắng | → F6: tay duỗi, kiếm ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 5 |
| F6 | 0.6 / 0.1 | tay duỗi, kiếm ngang | → F7: kiếm co vệt | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 6 |
| F7 | 0.7 / 0.1 | kiếm co vệt | → F8: kiếm dựng lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 7 |
| F8 | 0.8 / 0.1 | kiếm dựng lại | → F9: về đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 8 |
| F9 | 0.9 / 0.1 | về đứng | → F10: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 9 |
| F10 | 1 / 0.1 | giữ đứng | → F11: kiếm thấp hơn chút | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 10 |
| F11 | 1.1 / 0.1 | kiếm thấp hơn chút | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack1.png`, key 11 |

### Assets/Art/EndlessSkeletons/sword/Attack2.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | kiếm ngang eo | → F1: giữ ngang eo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 0 |
| F1 | 0.1 / 0.1 | giữ ngang eo | → F2: đâm thẳng, vệt tách | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 1 |
| F2 | 0.2 / 0.1 | đâm thẳng, vệt tách | → F3: giữ duỗi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 2 |
| F3 | 0.3 / 0.1 | giữ duỗi | → F4: thu kiếm còn dài | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 3 |
| F4 | 0.4 / 0.1 | thu kiếm còn dài | → F5: kiếm dựng lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 4 |
| F5 | 0.5 / 0.1 | kiếm dựng lại | → F6: về đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 5 |
| F6 | 0.6 / 0.1 | về đứng | → F7: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 6 |
| F7 | 0.7 / 0.1 | giữ đứng | → F8: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 7 |
| F8 | 0.8 / 0.1 | giữ đứng | → F9: giữ đứng cuối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 8 |
| F9 | 0.9 / 0.1 | giữ đứng cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Attack2.png`, key 9 |

### Assets/Art/EndlessSkeletons/sword/Die.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | đứng | → F1: đầu nâng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 0 |
| F1 | 0.1 / 0.1 | đầu nâng | → F2: sọ rời thân | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 1 |
| F2 | 0.2 / 0.1 | sọ rời thân | → F3: ngực rời eo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 2 |
| F3 | 0.3 / 0.1 | ngực rời eo | → F4: chân gập | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 3 |
| F4 | 0.4 / 0.1 | chân gập | → F5: chân đổ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 4 |
| F5 | 0.5 / 0.1 | chân đổ | → F6: tay, sườn rơi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 5 |
| F6 | 0.6 / 0.1 | tay, sườn rơi | → F7: sọ rơi thấp | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 6 |
| F7 | 0.7 / 0.1 | sọ rơi thấp | → F8: sọ gần đất | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 7 |
| F8 | 0.8 / 0.1 | sọ gần đất | → F9: sọ nằm | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 8 |
| F9 | 0.9 / 0.1 | sọ nằm | → F10: xương trải ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 9 |
| F10 | 1 / 0.1 | xương trải ngang | → F11: giữ đống xương | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 10 |
| F11 | 1.1 / 0.1 | giữ đống xương | → F12: held đống xương cuối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 11 |
| F12 | 1.2 / 0.1 | held đống xương cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Die.png`, key 12 |

### Assets/Art/EndlessSkeletons/sword/Hurt.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | đứng | → F1: flash trắng, lùi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Hurt.png`, key 0 |
| F1 | 0.1 / 0.1 | flash trắng, lùi | → F2: hết flash, kiếm thẳng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Hurt.png`, key 1 |
| F2 | 0.2 / 0.1 | hết flash, kiếm thẳng | → F3: đầu hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Hurt.png`, key 2 |
| F3 | 0.3 / 0.1 | đầu hạ | → F4: đứng lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Hurt.png`, key 3 |
| F4 | 0.4 / 0.1 | đứng lại | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Hurt.png`, key 4 |

### Assets/Art/EndlessSkeletons/sword/Idle.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | kiếm dựng chéo, chân trụ | → F1: thân nhích | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 0 |
| F1 | 0.1 / 0.1 | thân nhích | → F2: đầu hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 1 |
| F2 | 0.2 / 0.1 | đầu hạ | → F3: thân hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 2 |
| F3 | 0.3 / 0.1 | thân hạ | → F4: đầu nâng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 3 |
| F4 | 0.4 / 0.1 | đầu nâng | → F5: về cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 4 |
| F5 | 0.5 / 0.1 | về cao | → F6: giữ cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 5 |
| F6 | 0.6 / 0.1 | giữ cao | → F7: về pose đầu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 6 |
| F7 | 0.7 / 0.1 | về pose đầu | → F0: kiếm dựng chéo, chân trụ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Idle.png`, key 7 |

### Assets/Art/EndlessSkeletons/sword/Walk.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | chân mở | → F1: chân chuyển | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 0 |
| F1 | 0.1 / 0.1 | chân chuyển | → F2: nhấc gối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 1 |
| F2 | 0.2 / 0.1 | nhấc gối | → F3: đổi chân trụ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 2 |
| F3 | 0.3 / 0.1 | đổi chân trụ | → F4: chân khép | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 3 |
| F4 | 0.4 / 0.1 | chân khép | → F5: chân mở rộng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 4 |
| F5 | 0.5 / 0.1 | chân mở rộng | → F6: chân bắt chéo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 5 |
| F6 | 0.6 / 0.1 | chân bắt chéo | → F7: chân gần thẳng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 6 |
| F7 | 0.7 / 0.1 | chân gần thẳng | → F8: chuyển chân | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 7 |
| F8 | 0.8 / 0.1 | chuyển chân | → F9: về chân mở | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 8 |
| F9 | 0.9 / 0.1 | về chân mở | → F0: chân mở | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword/Walk.png`, key 9 |

### Assets/Art/EndlessSkeletons/sword-shield/Attack1.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | khiên trước thân | → F1: kiếm kéo về đầu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 0 |
| F1 | 0.1 / 0.1 | kiếm kéo về đầu | → F2: kiếm qua vai, khiên hẹp | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 1 |
| F2 | 0.2 / 0.1 | kiếm qua vai, khiên hẹp | → F3: gồng ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 2 |
| F3 | 0.3 / 0.1 | gồng ngang | → F4: giữ chuẩn bị | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 3 |
| F4 | 0.4 / 0.1 | giữ chuẩn bị | → F5: chém ngang, khiên hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 4 |
| F5 | 0.5 / 0.1 | chém ngang, khiên hạ | → F6: khiên nghiêng, tay duỗi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 5 |
| F6 | 0.6 / 0.1 | khiên nghiêng, tay duỗi | → F7: khiên mở lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 6 |
| F7 | 0.7 / 0.1 | khiên mở lại | → F8: kiếm dựng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 7 |
| F8 | 0.8 / 0.1 | kiếm dựng | → F9: về đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 8 |
| F9 | 0.9 / 0.1 | về đứng | → F10: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 9 |
| F10 | 1 / 0.1 | giữ đứng | → F11: kiếm hạ nhẹ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 10 |
| F11 | 1.1 / 0.1 | kiếm hạ nhẹ | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack1.png`, key 11 |

### Assets/Art/EndlessSkeletons/sword-shield/Attack2.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | khiên nghiêng, kiếm ngang eo | → F1: giữ chuẩn bị | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 0 |
| F1 | 0.1 / 0.1 | giữ chuẩn bị | → F2: đâm thẳng, khiên ra trái | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 1 |
| F2 | 0.2 / 0.1 | đâm thẳng, khiên ra trái | → F3: giữ duỗi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 2 |
| F3 | 0.3 / 0.1 | giữ duỗi | → F4: thu kiếm, khiên nghiêng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 3 |
| F4 | 0.4 / 0.1 | thu kiếm, khiên nghiêng | → F5: dựng kiếm | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 4 |
| F5 | 0.5 / 0.1 | dựng kiếm | → F6: về đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 5 |
| F6 | 0.6 / 0.1 | về đứng | → F7: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 6 |
| F7 | 0.7 / 0.1 | giữ đứng | → F8: giữ đứng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 7 |
| F8 | 0.8 / 0.1 | giữ đứng | → F9: giữ đứng cuối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 8 |
| F9 | 0.9 / 0.1 | giữ đứng cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Attack2.png`, key 9 |

### Assets/Art/EndlessSkeletons/sword-shield/Die.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | đứng | → F1: sọ nâng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 0 |
| F1 | 0.1 / 0.1 | sọ nâng | → F2: sọ rời | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 1 |
| F2 | 0.2 / 0.1 | sọ rời | → F3: ngực rời eo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 2 |
| F3 | 0.3 / 0.1 | ngực rời eo | → F4: chân gập | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 3 |
| F4 | 0.4 / 0.1 | chân gập | → F5: khiên nghiêng rơi | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 4 |
| F5 | 0.5 / 0.1 | khiên nghiêng rơi | → F6: sọ hạ, khiên chạm sàn | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 5 |
| F6 | 0.6 / 0.1 | sọ hạ, khiên chạm sàn | → F7: khiên nằm | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 6 |
| F7 | 0.7 / 0.1 | khiên nằm | → F8: sọ sát đất | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 7 |
| F8 | 0.8 / 0.1 | sọ sát đất | → F9: sọ nằm | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 8 |
| F9 | 0.9 / 0.1 | sọ nằm | → F10: xương trải ngang | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 9 |
| F10 | 1 / 0.1 | xương trải ngang | → F11: giữ xương | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 10 |
| F11 | 1.1 / 0.1 | giữ xương | → F12: held đống xương cuối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 11 |
| F12 | 1.2 / 0.1 | held đống xương cuối | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Die.png`, key 12 |

### Assets/Art/EndlessSkeletons/sword-shield/Hurt.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | đứng, khiên tròn | → F1: flash trắng cả khiên | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Hurt.png`, key 0 |
| F1 | 0.1 / 0.1 | flash trắng cả khiên | → F2: hết flash, khiên cạnh | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Hurt.png`, key 1 |
| F2 | 0.2 / 0.1 | hết flash, khiên cạnh | → F3: khiên hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Hurt.png`, key 2 |
| F3 | 0.3 / 0.1 | khiên hạ | → F4: khiên tròn lại | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Hurt.png`, key 3 |
| F4 | 0.4 / 0.1 | khiên tròn lại | Không ép loop; nối state/cleanup runtime chưa xem | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Hurt.png`, key 4 |

### Assets/Art/EndlessSkeletons/sword-shield/Idle.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | khiên tròn trước thân | → F1: khiên và thân nhích | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 0 |
| F1 | 0.1 / 0.1 | khiên và thân nhích | → F2: đầu hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 1 |
| F2 | 0.2 / 0.1 | đầu hạ | → F3: thân hạ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 2 |
| F3 | 0.3 / 0.1 | thân hạ | → F4: đầu nâng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 3 |
| F4 | 0.4 / 0.1 | đầu nâng | → F5: khiên về cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 4 |
| F5 | 0.5 / 0.1 | khiên về cao | → F6: giữ cao | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 5 |
| F6 | 0.6 / 0.1 | giữ cao | → F7: về pose đầu | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 6 |
| F7 | 0.7 / 0.1 | về pose đầu | → F0: khiên tròn trước thân | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Idle.png`, key 7 |

### Assets/Art/EndlessSkeletons/sword-shield/Walk.anim
| Frame | t / duration (s) | Pha / nhận xét hình | Chuyển tiếp đã đối chiếu trên cel | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.1 | khiên trước, chân mở | → F1: chân chuyển | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 0 |
| F1 | 0.1 / 0.1 | chân chuyển | → F2: nhấc gối | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 1 |
| F2 | 0.2 / 0.1 | nhấc gối | → F3: đổi chân trụ | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 2 |
| F3 | 0.3 / 0.1 | đổi chân trụ | → F4: chân khép | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 3 |
| F4 | 0.4 / 0.1 | chân khép | → F5: chân mở rộng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 4 |
| F5 | 0.5 / 0.1 | chân mở rộng | → F6: chân bắt chéo | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 5 |
| F6 | 0.6 / 0.1 | chân bắt chéo | → F7: chân gần thẳng | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 6 |
| F7 | 0.7 / 0.1 | chân gần thẳng | → F8: chuyển chân | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 7 |
| F8 | 0.8 / 0.1 | chuyển chân | → F9: về chân mở | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 8 |
| F9 | 0.9 / 0.1 | về chân mở | → F0: khiên trước, chân mở | chưa kiểm chứng | `Assets/Art/EndlessSkeletons/sword-shield/Walk.png`, key 9 |
