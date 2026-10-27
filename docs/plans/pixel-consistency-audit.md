# Rà soát độ đồng bộ pixel trên main — 2026-10-27

> **Chưa hoàn tất; nguồn main đã đổi UI giữa trận:** `66749da1` dùng gõ `READY`, bỏ nút `Ready · Enter`. Đang chờ chốt mốc nghiệm thu.
> **Checkpoint đang thực hiện:** xem [Capture mới và trạng thái còn mở](#capture-mới-và-trạng-thái-còn-mở).
> Các revision, 1.432 frame và kết luận Ready chưa có ảnh ở phần lịch sử bên dưới
> thuộc lượt trước. Không cộng chúng vào coverage mới; audit chưa hoàn tất.

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

Lượt bổ sung xác nhận **A1 ở nguồn và runtime Die rìu**: mảnh khiên của đối
tượng khác lẫn vào rìu Walk F6–F11 và Die F0; runtime F2392/99.008s thấy vạch
ngà rời sát sàn khi flash Slayer nhuộm trắng mảnh nâu. Walk chỉ là lỗi asset,
không có đường gọi runtime hiện hành. Đã xem 1.432 frame encode / 1.419 cặp
trong các đoạn đã chốt, ghi từng frame bên dưới. Những khác biệt phối cảnh, palette, SDF, bloom, blur và
reflection có căn cứ thiết kế; không đề nghị tắt chúng chỉ để đồng nhất lưới pixel.

**Chưa nghiệm thu toàn bộ animation/camera**: thiếu quan sát playback 1×/chậm,
zoom nearest bổ sung cho mọi frame runtime và chi tiết bị VFX/overlay che.
Các khoảng trống được giữ riêng; không dùng build hoặc số lượng frame để lấp.
Fetch chốt bổ sung tới `29c032856460007055d6926cac2d9284704cd6d5`; thay đổi
duy nhất trong runtime từ bản capture `5d637df` là nhãn giữa trận thành
`Ready · Enter`. Ở checkpoint lịch sử này nhãn mới chưa có ảnh runtime; kết quả mới được ghi riêng ở cuối báo cáo.

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

### A1 — Mảng nâu rời trong nguồn rìu Walk (kết quả lượt đầu)

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

## Kiểm chứng runtime lượt đầu

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

## Phần chưa kiểm chứng của lượt đầu

Bảng này giữ checkpoint trước lượt bổ sung; số đếm video bằng 0 chỉ mô tả
lượt đầu. Coverage và giới hạn mới nằm trong phần bổ sung cuối báo cáo.

| Phần | Bằng chứng còn thiếu | Cách xác nhận trong lượt tiếp theo nếu cần chốt toàn phạm vi |
|---|---|---|
| Rung cạnh/follow/zoom/shake, trượt anchor | Ảnh tĩnh và nguồn không chứng minh continuity. | Ghi đoạn đầy đủ idle → normal hit/received hit/parry → hồi phục, entry/exit Slayer; xem mọi frame/chuyển tiếp ở kích thước chơi và nearest zoom, giữ frame lặp/timing. |
| Mọi animation/mirror/VFX | Đã xem 265 cel nguồn một hướng; chưa playback 1×/chậm, không có video runtime được review từng frame; các clip tutorial gốc ngoài bảng và state nối chưa review đủ. Số frame/transition video đã kiểm: 0; không có đoạn video nào được chọn làm bằng chứng. | Ghi rõ clip/biến thể, mốc đầu/cuối toàn hành vi và tổng frame/transition trước khi review; bổ sung từng hàng cho output cuối. Không dùng 17 screenshot để gọi animation đạt. |
| Normal/depth/bóng/phản chiếu | Binding/kích thước khớp, ảnh sample không chứng minh mọi hướng hoặc mọi pose đúng. | Review đủ cel/mirror trong cùng ánh sáng; phân biệt highlight có chủ ý với normal đảo hoặc bóng sai hình. |
| Atlas bleeding và imported compression | Nguồn không có dấu hiệu mip/filter sai; chưa đọc mọi imported texture/packing trong player. | Chẩn đoán imported rect/format/UV đúng asset khi có ảnh hoặc frame tái hiện seam/halo. |
| Font fallback và mọi nhãn động | Đã xem nhãn hiện ở 17 capture; chưa ép đầy bộ fallback/ký tự hoặc mọi trạng thái chữ động. | Dùng input được game cho phép để ép glyph cần kiểm, xem glyph thật và clipping tại hai kích thước; không đổi bộ font chỉ vì khác shader. |

Kết luận lượt đầu (được cập nhật bởi phần bổ sung): không sửa game trong audit này. Khi đó ưu tiên xác nhận A1 nếu Walk sẽ được dùng;
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

## Bổ sung khoảng trống — lượt 2026-10-27

### Revision và controller thực sự

Bắt đầu từ `5d637dfc7cd881ecb20b92b2057679cf5c555b1f` sau fetch origin.
Snapshot LF mới: 2.495 file, SHA256 `0c94ea6c64c47de94c19842ec6599ace5281b27459c5780c98b8e995c6c9cbc7`,
trùng snapshot audit đầu. Không dùng checkout bẩn làm đầu vào.

`Assets/Characters/Enermy/Skeleton.prefab:183` nối `AC_Skeleton.controller`
(GUID `28c1720a47527d049a98c7f82b523ad4`). `CombatSession.Awake` chỉ thêm
`EndlessDuel` khi `!TutorialRun.Active`; `EndlessDuel.Start` gọi
`SkeletonController.ConfigureEndless`, gán `definition.controller` và phát
`skeleton_idle`. Vì vậy tutorial giữ controller gốc; combat Endless dùng
`sword/axe/sword-shield/Combat.controller`. Không được dùng bảng Endless để
chứng nhận toàn bộ tutorial. Tutorial hiện hành là màn đọc, khóa input/combat;
các state có trong controller không đồng nghĩa tất cả được gọi trong tutorial.
`EndlessAssets.Prepare` tạo Walk nhưng không tạo transition tới Walk; tìm
đường gọi runtime không thấy yêu cầu phát Walk. Giữ phân loại Walk ở mức asset.

Giới hạn reachability khác: `EndlessRun.RepeatAfterBoss=false`, kết thúc ở sáu
duel của Round 1. Rìu dùng Heavy, `opening=[1]`, `alternate=[1]`; Berserker
chỉ nằm trong nhánh Round 2 không tới được qua run hiện hành. Vì vậy rìu
Attack2 có asset/controller nhưng không có encounter hiện hành phát nó qua
input bình thường. Không sửa run hay dùng bridge để ép state cho audit.
Skeleton prefab mirror bằng `localScale.x=-1`, `m_FlipX=0`; cần phân biệt
mirror transform này với shader `_DepthFacing` cập nhật theo SpriteRenderer.flipX.
Không suy ra sai normal chỉ từ hai giá trị đó; phải xem shading của pose thực.

Reflection hiện hành: `DungeonWorld.unity:34909` có node `WetStoneReflection`,
`m_IsActive:0`. Mô tả half-resolution/Bilinear ở bảng đầu là contract của code
khi được bật, không chứng minh reflection đang chạy. Audit không tự bật lại
reflection để lấy ảnh; không coi không có phản chiếu là lỗi so với cấu hình này.

### Bảng nguồn skeleton gốc còn thiếu

Trực tiếp mở sáu sheet nearest 4×, giữ key lặp và duration từ `.anim`.
Các hàng dưới là quan sát cel nguồn; chưa chứng nhận playback, runtime, mirror
hoặc state nối. Tổng bổ sung 41 key, 39 cặp nội bộ/loop (Die và Attack không loop).
Mỗi bằng chứng được định danh bởi revision trên + clip/hash + tên cel; không
giữ ảnh tạm sau audit.

#### `Assets/Scripts/skeleton_die.anim`

SHA256 `f5a612bc073058be54d91bd3c605e6010941e09722dc3b486ea347b9ad9cd914`; loop `False`; stop 0.84999996s.

| Frame | t / duration (s) | Reference / nhận xét cel | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.06666667 | đứng, kiếm cao | → F1: sọ nâng, chân nguyên | chưa kiểm chứng | `Skeleton_01_White_Die_0` |
| F1 | 0.06666667 / 0.06666667 | sọ nâng, chân nguyên | → F2: sọ rời cổ | chưa kiểm chứng | `Skeleton_01_White_Die_1` |
| F2 | 0.13333334 / 0.08333333 | sọ rời cổ | → F3: eo rời, gối gập | chưa kiểm chứng | `Skeleton_01_White_Die_2` |
| F3 | 0.21666667 / 0.06666666 | eo rời, gối gập | → F4: xương sườn hạ, kiếm chéo | chưa kiểm chứng | `Skeleton_01_White_Die_3` |
| F4 | 0.28333333 / 0.06666667 | xương sườn hạ, kiếm chéo | → F5: kiếm nằm thấp, chân chạm đất | chưa kiểm chứng | `Skeleton_01_White_Die_4` |
| F5 | 0.35 / 0.06666666 | kiếm nằm thấp, chân chạm đất | → F6: tay rời, ngực sát sàn | chưa kiểm chứng | `Skeleton_01_White_Die_5` |
| F6 | 0.41666666 / 0.06666666 | tay rời, ngực sát sàn | → F7: tay/kiếm rơi ngang, sọ còn treo | chưa kiểm chứng | `Skeleton_01_White_Die_6` |
| F7 | 0.48333332 / 0.06666668 | tay/kiếm rơi ngang, sọ còn treo | → F8: sọ hạ, kiếm tách bên phải | chưa kiểm chứng | `Skeleton_01_White_Die_7` |
| F8 | 0.55 / 0.0833333 | sọ hạ, kiếm tách bên phải | → F9: sọ chạm đống xương, kiếm ngang | chưa kiểm chứng | `Skeleton_01_White_Die_8` |
| F9 | 0.6333333 / 0.0666667 | sọ chạm đống xương, kiếm ngang | → F10: giữ đống xương thấp | chưa kiểm chứng | `Skeleton_01_White_Die_9` |
| F10 | 0.7 / 0.06666665 | giữ đống xương thấp | → F11: giữ sọ/xương, kiếm sát sàn | chưa kiểm chứng | `Skeleton_01_White_Die_10` |
| F11 | 0.76666665 / 0.06666665 | giữ sọ/xương, kiếm sát sàn | → F12: đống xương cuối, không còn thân đứng | chưa kiểm chứng | `Skeleton_01_White_Die_11` |
| F12 | 0.8333333 / 0.01666666 | đống xương cuối, không còn thân đứng | không ép loop; state nối runtime chưa kiểm | chưa kiểm chứng | `Skeleton_01_White_Die_12` |

#### `Assets/Art/VFX/skeleton_hitBySlayer.anim`

SHA256 `a3b3db4db16902b444d0a12c9f83cc1f2ca577d09fc3ab2d68c95405680580d4`; loop `True`; stop 0.06666667s.

| Frame | t / duration (s) | Reference / nhận xét cel | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.05 | Hurt_1, flash trắng toàn thân | → F1: Hurt_3, hết flash, kiếm chéo | chưa kiểm chứng | `Skeleton_01_White_Hurt_1` |
| F1 | 0.05 / 0.01666667 | Hurt_3, hết flash, kiếm chéo | → F0: Hurt_1, flash trắng toàn thân | chưa kiểm chứng | `Skeleton_01_White_Hurt_3` |

#### `Assets/Scripts/skeleton_hit.anim`

SHA256 `d9b57c2ead60fa1afad3260dc307f9eaf50e147d821351cecca92ea9c27127c0`; loop `True`; stop 0.38333336s.

| Frame | t / duration (s) | Reference / nhận xét cel | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.083333336 | đứng, kiếm cao | → F1: flash trắng cả xương và kiếm, thân lệch | chưa kiểm chứng | `Skeleton_01_White_Hurt_0` |
| F1 | 0.083333336 / 0.1 | flash trắng cả xương và kiếm, thân lệch | → F2: hết flash, kiếm gần dọc | chưa kiểm chứng | `Skeleton_01_White_Hurt_1` |
| F2 | 0.18333334 / 0.09999999 | hết flash, kiếm gần dọc | → F3: thân đứng lại, kiếm chéo | chưa kiểm chứng | `Skeleton_01_White_Hurt_2` |
| F3 | 0.28333333 / 0.08333334 | thân đứng lại, kiếm chéo | → F4: giữ đứng, chân khép hơn | chưa kiểm chứng | `Skeleton_01_White_Hurt_3` |
| F4 | 0.36666667 / 0.01666669 | giữ đứng, chân khép hơn | → F0: đứng, kiếm cao | chưa kiểm chứng | `Skeleton_01_White_Hurt_4` |

#### `Assets/Scripts/skeleton_attack_heavy.anim`

SHA256 `79d0811c8499de273ab079256e7a9a64a30018e3fe891c6f82c71a90630ca8c7`; loop `False`; stop 1.3333334s.

| Frame | t / duration (s) | Reference / nhận xét cel | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.083333336 | đứng chuẩn bị | → F1: kiếm dựng sát đầu | chưa kiểm chứng | `Skeleton_01_White_Attack1_0` |
| F1 | 0.083333336 / 0.083333334 | kiếm dựng sát đầu | → F2: tay cuộn trước ngực, kiếm sau vai | chưa kiểm chứng | `Skeleton_01_White_Attack1_1` |
| F2 | 0.16666667 / 0.24999999 | tay cuộn trước ngực, kiếm sau vai | → F3: giữ kiếm sau vai, mũi kéo trái | chưa kiểm chứng | `Skeleton_01_White_Attack1_2` |
| F3 | 0.41666666 / 0.50000004 | giữ kiếm sau vai, mũi kéo trái | → F4: giữ kiếm sau vai, chân sau co nâng | chưa kiểm chứng | `Skeleton_01_White_Attack1_3` |
| F4 | 0.9166667 / 0.06666665 | giữ kiếm sau vai, chân sau co nâng | → F5: chém phải, smear trắng rộng, chân duỗi | chưa kiểm chứng | `Skeleton_01_White_Attack1_4` |
| F5 | 0.98333335 / 0.10000005 | chém phải, smear trắng rộng, chân duỗi | → F6: smear thu ngắn, tay vẫn duỗi | chưa kiểm chứng | `Skeleton_01_White_Attack1_5` |
| F6 | 1.0833334 / 0.0666666 | smear thu ngắn, tay vẫn duỗi | → F7: mũi nhọn kéo dài, thân thu | chưa kiểm chứng | `Skeleton_01_White_Attack1_6` |
| F7 | 1.15 / 0.1 | mũi nhọn kéo dài, thân thu | → F8: tay mở, kiếm chéo phải | chưa kiểm chứng | `Skeleton_01_White_Attack1_7` |
| F8 | 1.25 / 0.0666667 | tay mở, kiếm chéo phải | → F9: về đứng, kiếm cao | chưa kiểm chứng | `Skeleton_01_White_Attack1_8` |
| F9 | 1.3166667 / 0.0166667 | về đứng, kiếm cao | không ép loop; state nối runtime chưa kiểm | chưa kiểm chứng | `Skeleton_01_White_Attack1_9` |

#### `Assets/Characters/Enermy/skeleton_stun.anim`

SHA256 `9257920ffb6d3593c887a3f2e5eefcc9957a92173cd5f9d2d36df2659feab237`; loop `True`; stop 0.71666664s.

| Frame | t / duration (s) | Reference / nhận xét cel | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.35 | Hurt_3, thân đứng, kiếm chéo | → F1: Hurt_4, ngực nâng nhẹ | chưa kiểm chứng | `Skeleton_01_White_Hurt_3` |
| F1 | 0.35 / 0.35 | Hurt_4, ngực nâng nhẹ | → F2: lặp Hurt_3, trở lại đầu | chưa kiểm chứng | `Skeleton_01_White_Hurt_4` |
| F2 | 0.7 / 0.01666664 | lặp Hurt_3, trở lại đầu | → F0: Hurt_3, thân đứng, kiếm chéo | chưa kiểm chứng | `Skeleton_01_White_Hurt_3` |

#### `Assets/Characters/Enermy/skeleton_idle.anim`

SHA256 `af7522d518783ee3647ce901bd3cb8c573b3567fe4801ad02f73e905608c9672`; loop `True`; stop 0.59999996s.

| Frame | t / duration (s) | Reference / nhận xét cel | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0 / 0.083333336 | đứng, kiếm chéo lên phải | → F1: đầu giữ cao, tay kiếm hạ nhẹ | chưa kiểm chứng | `Skeleton_01_White_Idle_0` |
| F1 | 0.083333336 / 0.083333334 | đầu giữ cao, tay kiếm hạ nhẹ | → F2: đầu hạ, thân nén | chưa kiểm chứng | `Skeleton_01_White_Idle_1` |
| F2 | 0.16666667 / 0.08333333 | đầu hạ, thân nén | → F3: thân thấp, kiếm cùng hướng | chưa kiểm chứng | `Skeleton_01_White_Idle_2` |
| F3 | 0.25 / 0.08333334 | thân thấp, kiếm cùng hướng | → F4: đầu nâng, gối mở | chưa kiểm chứng | `Skeleton_01_White_Idle_3` |
| F4 | 0.33333334 / 0.08333332 | đầu nâng, gối mở | → F5: ngực nâng, tay về cao | chưa kiểm chứng | `Skeleton_01_White_Idle_4` |
| F5 | 0.41666666 / 0.08333334 | ngực nâng, tay về cao | → F6: giữ cao, tay không tách | chưa kiểm chứng | `Skeleton_01_White_Idle_5` |
| F6 | 0.5 / 0.0833333 | giữ cao, tay không tách | → F7: trở lại độ cao đầu | chưa kiểm chứng | `Skeleton_01_White_Idle_6` |
| F7 | 0.5833333 / 0.01666666 | trở lại độ cao đầu | → F0: đứng, kiếm chéo lên phải | chưa kiểm chứng | `Skeleton_01_White_Idle_7` |

### A1 bổ sung — xác nhận lẫn chiếc khiên trong extraction rìu

Nguồn: `design/skeleton-hero/references/monopixelart-full.gif`, SHA256
`e41867a829ef06b3296e8778aa43739bc8c0605b9df77e4212dbd373c4ba522c`.
Phạm vi đối chiếu: GIF F13–F35, [1.3, 3.6)s, toàn bộ Walk và Die của rìu,
100ms mỗi frame, 23/23 frame và 22/22 cặp trong đoạn trực tiếp xem trên
sheet nearest 2× lưới gốc; đối chiếu 23/23 cel extraction nearest 4×.
Không xem toàn bộ GIF, không gọi đây là review playback 1×/chậm.

Vị trí lỗi: `prepare_gif_pack.py:owners`, lane giữa dùng `right_split=140`
ở F13–25; gốc extraction rìu (64,18), canvas 96×64. Khi khiên của skeleton
bên phải rơi sang x134–139, các pixel nâu được gán cho rìu. Mảnh này không
phải cán/đầu rìu hay smear có chủ ý. GIF F25 vẫn có khiên nằm ở sàn, còn rìu
đứng; vì thế Die F0 kế thừa mảnh đó rồi biến mất ở F1 khi đổi lane split.

Kiểm hỗ trợ: mọi pixel opaque của 23 cel giữ đúng RGB và tọa độ gốc GIF;
điều này xác nhận nguồn gốc pixel, không chứng nhận ownership đúng. Vùng
x≥70,y≥52 của cel có lần lượt 11/22/45/37/20/17 pixel nâu ở Walk F6–F11,
17 pixel ở Die F0; chỉ dùng số đếm để định vị, kết luận dựa trên hình đã xem.

- **Lỗi asset xác nhận**: Walk F6–F11 và Die F0 chứa mảnh khiên của đối tượng khác.
- Điều kiện tái hiện nguồn: mở nguyên cel 96×64, hoặc phát Walk [0.6,1.2)s /
  Die [0,0.1)s; mảnh ở phía phải sát đáy, độc lập với tay và cán rìu.
- Tác động: Walk có vật thể rời ở chân và nhảy mất khi loop; Die có thể lóe
  mảnh ngoài thân trong key đầu. Walk chưa có đường gọi runtime đã tìm thấy.
- Hướng xử lý: chỉnh ownership của vùng khiên tại các source frame tương ứng,
  xuất lại riêng cel bị lẫn, bảo toàn phần rìu; review lại toàn clip và điểm nối.
  Audit này không sửa extraction, PNG hoặc game.
- Runtime Die và khả năng thấy trong ánh sáng/che lấp: ghi ở phần runtime của
  lượt bổ sung; không suy ra lỗi gameplay đã tái hiện chỉ từ bảng nguồn.

| Clip | Frame | t / duration (s) | GIF / quan sát trực tiếp | Chuyển tiếp | Kết quả nguồn |
|---|---|---|---|---|---|
| Walk | F0 | 0.0 / 0.1 | GIF F13 / chân trái mở, rìu dọc | → F1: thân hạ, chân đổi trụ | quan sát nguồn; runtime chưa kiểm chứng |
| Walk | F1 | 0.1 / 0.1 | GIF F14 / thân hạ, chân đổi trụ | → F2: chân chéo, tay gần cán | quan sát nguồn; runtime chưa kiểm chứng |
| Walk | F2 | 0.2 / 0.1 | GIF F15 / chân chéo, tay gần cán | → F3: gối nâng, cán còn dọc | quan sát nguồn; runtime chưa kiểm chứng |
| Walk | F3 | 0.3 / 0.1 | GIF F16 / gối nâng, cán còn dọc | → F4: đầu rìu xoay nhẹ, thân thấp | quan sát nguồn; runtime chưa kiểm chứng |
| Walk | F4 | 0.4 / 0.1 | GIF F17 / đầu rìu xoay nhẹ, thân thấp | → F5: chân duỗi, cán nghiêng nhẹ | quan sát nguồn; runtime chưa kiểm chứng |
| Walk | F5 | 0.5 / 0.1 | GIF F18 / chân duỗi, cán nghiêng nhẹ | → F6: chân thu, mảnh khiên nâu bắt đầu bên phải | quan sát nguồn; runtime chưa kiểm chứng |
| Walk | F6 | 0.6 / 0.1 | GIF F19 / chân thu, mảnh khiên nâu bắt đầu bên phải | → F7: thân hẹp, mảnh khiên chạm đáy | lỗi ownership |
| Walk | F7 | 0.7 / 0.1 | GIF F20 / thân hẹp, mảnh khiên chạm đáy | → F8: đứng cao, mảnh khiên rộng hơn | lỗi ownership |
| Walk | F8 | 0.8 / 0.1 | GIF F21 / đứng cao, mảnh khiên rộng hơn | → F9: đầu rìu trở lại, khiên nằm cạnh chân | lỗi ownership |
| Walk | F9 | 0.9 / 0.1 | GIF F22 / đầu rìu trở lại, khiên nằm cạnh chân | → F10: chân gần pose đầu, khiên sát đáy | lỗi ownership |
| Walk | F10 | 1.0 / 0.1 | GIF F23 / chân gần pose đầu, khiên sát đáy | → F11: pose đứng, khiên vẫn còn | lỗi ownership |
| Walk | F11 | 1.1 / 0.1 | GIF F24 / pose đứng, khiên vẫn còn | → F0: khiên biến mất, chân mở lại | lỗi ownership |
| Die | F0 | 0.0 / 0.1 | GIF F25 / rìu đứng, mảnh khiên rời bên phải | → F1: sọ nâng, mảnh khiên mất | lỗi ownership |
| Die | F1 | 0.1 / 0.1 | GIF F26 / sọ nâng, mảnh khiên mất | → F2: sọ rời, rìu dọc | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F2 | 0.2 / 0.1 | GIF F27 / sọ rời, rìu dọc | → F3: eo/xương rời, cán nghiêng nhẹ | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F3 | 0.3 / 0.1 | GIF F28 / eo/xương rời, cán nghiêng nhẹ | → F4: ngực hạ, sọ trái cán | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F4 | 0.4 / 0.1 | GIF F29 / ngực hạ, sọ trái cán | → F5: rìu nghiêng xuống, chân thành đống | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F5 | 0.5 / 0.1 | GIF F30 / rìu nghiêng xuống, chân thành đống | → F6: rìu quét nằm ngang trên xương | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F6 | 0.6 / 0.1 | GIF F31 / rìu quét nằm ngang trên xương | → F7: sọ thấp, rìu sát sàn | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F7 | 0.7 / 0.1 | GIF F32 / sọ thấp, rìu sát sàn | → F8: giữ xương và rìu ngang | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F8 | 0.8 / 0.1 | GIF F33 / giữ xương và rìu ngang | → F9: giữ đống xương, đầu rìu nằm trái | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F9 | 0.9 / 0.1 | GIF F34 / giữ đống xương, đầu rìu nằm trái | → F10: giữ pose kết thúc | quan sát nguồn; runtime chưa kiểm chứng |
| Die | F10 | 1.0 / 0.1 | GIF F35 / giữ pose kết thúc | không ép loop; cleanup runtime chưa kiểm | quan sát nguồn; runtime chưa kiểm chứng |

SHA256 asset đã đối chiếu:

- `Assets/Art/EndlessSkeletons/axe/Walk.png`: `8cae0407a21d25fbe1898bd41df59d6c6bbd786a29e29ca467f054ba845d316e`.
- `Assets/Art/EndlessSkeletons/axe/Die.png`: `7bdf2c0ab75caf8277818faeb9d3ce406e4afa7a94a345c57db44f5ac169b507`.


### Phạm vi video runtime chọn trước khi review bổ sung

Capture chỉ dùng bàn phím/chuột; probe và observer chỉ đọc. Firebase qua fixture,
mọi host ngoài bị chặn; không ép animator, HP, mirror hoặc timeScale bằng bridge.
Đoạn đầu: `tutorial-idle.webm`, SHA256
`f47a5d00c42661fc4405d500fb0469ff77dfa8053ff7b7b0653882185c9e0f9c`,
toàn đoạn PTS 0–1.771s và hold kết thúc tới recorder stop; 25 frame, 24 cặp.
Lý do: kiểm idle gốc và idle player trong màn đọc có card che lấp, không cắt
bỏ frame giữa hoặc thay sampling. Xuất PNG bằng ffmpeg fps passthrough, giữ
PTS gốc không ép CFR, không loại frame lặp. Đây là frame video được encode,
không phải bảo đảm ghi mọi engine frame; captureStream(30) không chứng minh 30FPS.


Đã trực tiếp xem 25/25 frame và 24/24 cặp của đoạn tutorial trên năm sheet
tuần tự ở crop native 450×270, vùng (400,230)–(850,500), cùng toàn màn
1280/1366/1280. Chân giữ cùng landmark, kiếm không đổi grip, card che đầu
skeleton và phần thân theo overlay. Trace `timeScale=0`, player
`male_hero-idle_0`, enemy `Skeleton_01_White_Idle_0`: reading lock giữ pose,
không phải đã xem ba vòng animation. PTS F10→F11 giữ 809ms; không chèn frame
giả hoặc rút ngắn duration. Chưa xem playback 1×/chậm; không chứng nhận idle
động hoặc continuity engine giữa các frame video.

| Clip / frame | PTS / duration (s) | Reference / nhận xét | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| tutorial-idle / F0 | 0.000 / 0.068000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F1: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F0 |
| tutorial-idle / F1 | 0.068 / 0.067000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F2: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F1 |
| tutorial-idle / F2 | 0.135 / 0.074000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F3: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F2 |
| tutorial-idle / F3 | 0.209 / 0.077000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F4: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F3 |
| tutorial-idle / F4 | 0.286 / 0.028000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F5: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F4 |
| tutorial-idle / F5 | 0.314 / 0.029000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F6: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F5 |
| tutorial-idle / F6 | 0.343 / 0.037000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F7: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F6 |
| tutorial-idle / F7 | 0.380 / 0.050000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F8: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F7 |
| tutorial-idle / F8 | 0.430 / 0.049000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F9: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F8 |
| tutorial-idle / F9 | 0.479 / 0.028000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F10: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F9 |
| tutorial-idle / F10 | 0.507 / 0.809000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F11: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F10 |
| tutorial-idle / F11 | 1.316 / 0.026000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F12: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F11 |
| tutorial-idle / F12 | 1.342 / 0.048000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F13: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F12 |
| tutorial-idle / F13 | 1.390 / 0.042000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F14: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F13 |
| tutorial-idle / F14 | 1.432 / 0.041000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F15: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F14 |
| tutorial-idle / F15 | 1.473 / 0.033000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F16: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F15 |
| tutorial-idle / F16 | 1.506 / 0.030000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F17: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F16 |
| tutorial-idle / F17 | 1.536 / 0.034000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F18: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F17 |
| tutorial-idle / F18 | 1.570 / 0.022000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F19: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F18 |
| tutorial-idle / F19 | 1.592 / 0.042000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F20: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F19 |
| tutorial-idle / F20 | 1.634 / 0.045000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F21: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F20 |
| tutorial-idle / F21 | 1.679 / 0.024000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F22: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F21 |
| tutorial-idle / F22 | 1.703 / 0.024000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F23: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F22 |
| tutorial-idle / F23 | 1.727 / 0.044000 | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | → F24: chân/kiếm và mép card giữ vị trí | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F23 |
| tutorial-idle / F24 | 1.771 / chưa xác định | reading Idle_0; chân đứng, kiếm chéo; đầu skeleton bị card che | kết thúc video; hold cuối chưa xác định; resize sau đoạn | quan sát pose giữ; animation chưa kiểm chứng | clip hash trên, F24 |


Các scope combat được chốt theo trace trước khi mở sheet review. Giữ toàn
bộ frame encode trong mỗi khoảng, cả hold và frame lặp; không chọn riêng
key pose. Probe gắn theo thời gian gần nhất chỉ giúp định vị, không bảo đảm
frame-exact với PNG. Quan sát hình là căn cứ về pose, không lấy tên role làm
bằng chứng actor (Run đổi role trước cleanup). Crop native review (400,200)–
(900,500), toàn frame gốc vẫn được xuất tạm để kiểm context khi cần.

Nguồn capture chính:

- `combat.webm`: SHA256 `7c12c34ae32a4c32ebd4b48e02dbb7786a2b696022ff142d0a5f344e40e06c06`,
  6.980 frame, PTS cuối 275.316s; chỉ review các scope đã chọn dưới đây.
- `hurt-death.webm`: SHA256 `a065e78d6ac3cda113582b2d70222cc70adfbf2c75579ebe4c4af9d3aa90aaca`,
  782 frame, PTS cuối 28.061s; chỉ review các scope đã chọn dưới đây.
- Recorder stop của capture chính được ghi sau serialize Blob nên có overhead;
  không dùng nó để suy ra hold cuối hoặc FPS. PTS giữa các frame giữ nguyên.
  Đặc biệt duration cuối tutorial phải giữ **chưa xác định chính xác**, không
  lấy 102ms tính từ stop sau serialize làm duration đã chứng minh.

| Scope | Video / PTS cần xem (s) | Frame / cặp cần xem | Lý do chọn |
|---|---|---|---|
| normal-parry | combat / 0.000–3.696 | 75 / 74 | đứng → attack thường, parry/phản kích, Hurt kiếm và hồi phục |
| sword-death | combat / 23.805–26.179 | 72 / 71 | chuẩn bị đòn kết liễu → Die kiếm → cleanup |
| sword-attack2-idle | combat / 27.620–32.637 | 148 / 147 | Attack1 → Attack2 → idle kiếm trọn vòng, mirror runtime |
| axe-idle-attack-hurt | combat / 64.037–68.860 | 108 / 107 | idle rìu → Attack1/parry → Hurt → hồi phục |
| axe-death | combat / 98.430–100.627 | 61 / 60 | đòn kết liễu → Die rìu đủ key → cleanup |
| shield-attacks-hurt | combat / 101.022–108.232 | 174 / 173 | idle → Attack1, Hurt, Attack2 → hồi phục khiên |
| shield-death | combat / 131.405–133.676 | 69 / 68 | đòn kết liễu → Die khiên → cleanup |
| player-hurt-idle | hurt-death / 0.000–3.673 | 105 / 104 | idle → enemy attack → player Hurt → idle đủ vòng |
| player-death | hurt-death / 24.127–27.995 | 114 / 113 | chuẩn bị đòn lethal → death player → result |


Scope bổ sung chốt trước khi mở sheet: giãn phím Slayer 1.05s để cho clip
chạy hết recovery; tái hiện Die_0 rìu bằng Pause thật; idle khiên dùng đoạn
capture chính có đủ vòng không bị intro che. Lượt target chết sau khi đã lưu
hai clip, trong lúc chờ khiên; không tính là lượt driver pass. Không bỏ các
scope đã chọn ở capture chính. Ảnh Pause rìu có `f.enemyCel=Die_0` và
`after.enemyCel=Die_0`, nhưng overlay che body nên ảnh này riêng lẻ không
chứng minh mảnh nâu nhìn thấy. Clip phải được xem độc lập.

| Scope | Video / SHA256 | Khoảng yêu cầu PTS (s) | Frame / cặp cần xem |
|---|---|---|---|
| shield-idle | `combat.webm` / `7c12c34ae32a4c32ebd4b48e02dbb7786a2b696022ff142d0a5f344e40e06c06` | 121.100–125.000 | 115 / 114 |
| slayer-complete | `slayer-complete.webm` / `474b60c33650f6447f1526dad0c79607a34e4b818ec819976bbca5617814bf15` | 4.300–16.077 (mở rộng đuôi) | 309 / 308 |
| axe-die-target | `axe-die-repro.webm` / `699b4e681202bce7e14dfac7192c6ab8ebafb5ab28dd7fa7f8aa7705889fa9f8` | 25.585–28.263 (mở rộng đuôi) | 57 / 56 |


### Biên review runtime và kết quả quan sát

Mở rộng Slayer tới hết video F413/16.077s (thêm 14 frame) để thấy camera,
palette và cue trở về bình thường; mở rộng Die rìu target tới F661/28.263s
(thêm 11 frame) vì Pause kéo dài đoạn chết. Không thu hẹp đoạn để bỏ lỗi.
Đã trực tiếp xem 1.407/1.407 frame encode, 1.395/1.395 cặp liên tiếp của 12
scope dưới đây; cộng tutorial là 1.432 frame / 1.419 cặp. Đây **không phải**
tuyên bố đã ghi được mọi frame Unity: MediaRecorder có khoảng PTS biến thiên.
Frame giữ/lặp vẫn nằm riêng trong bảng. Duration là PTS kế trừ PTS hiện tại;
frame cuối video không suy diễn duration từ thời gian serialize.

Review từng ảnh theo thứ tự và đối chiếu cả biên giữa các sheet, native crop
500×300. Với sword/axe/shield death và Slayer, đã mở lại **toàn bộ** ảnh bằng
crop rộng 768×465 để không mất chân/vũ khí khi zoom. Entry Slayer F133–138,
exit F400–413 và death player F750–759 được mở thêm full 1280×720 vì crop che
đầu/đỉnh hiệu ứng. A1 F2392 được mở nearest 4× vùng (560,410)–(800,530).
Chưa review mọi frame runtime ở zoom nearest bổ sung; không dùng vài crop
phóng to để nhận đủ yêu cầu đó. Normal/depth ở vùng flash/silhouette, thân sau
overlay và grip sau cung sáng được giữ chưa kiểm chứng.

**Playback đúng tốc độ/chậm chưa được quan sát trực tiếp**: công cụ phiên này
chỉ trả ảnh tĩnh; không thay việc xem playback bằng chạy video tự động, số
frame, contact sheet hoặc timestamp. Theo
[sprite-frame-review](../../.agents/skills/sprite-frame-review/SKILL.md):
“Chỉ báo đạt khi đã kiểm đủ frame, chuyển tiếp, playback và đối chiếu yêu cầu/”.
Vì thiếu playback, các hàng không có lỗi vẫn là **chưa kiểm chứng nghiệm thu
chuyển động**, dù hình và chuyển tiếp đã được xem. Sáu clip nguồn bổ sung và
đoạn GIF extraction cũng chưa có playback quan sát; không kết luận khớp toàn
bộ reference hay nghiệm thu animation.

Probe chỉ gắn gần thời gian để định vị pha; quan sát trong cột nhận xét là
ghi sau khi mở ảnh, không sinh từ tên state. Role của Run có thể chuyển sớm
trước actor cleanup: ảnh `axe-death-paused` lượt chính thực ra thuộc Double
Strike, bị loại khỏi bằng chứng rìu. Ảnh Pause ở lượt target đúng Heavy nhưng
card che thân, không riêng nó chứng minh A1. Mỗi hàng dưới gắn video/hash ở
bảng scope + số F encode + PTS; ảnh tạm được dọn theo yêu cầu, không để link hỏng.

#### Runtime `normal-parry` — 75 frame / 74 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0.000 / 0.047000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F1: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F0 |
| F1 | 0.047 / 0.057000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F2: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1 |
| F2 | 0.104 / 0.037000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F3: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2 |
| F3 | 0.141 / 0.036000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F4: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3 |
| F4 | 0.177 / 0.060000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F5: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F4 |
| F5 | 0.237 / 0.033000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F6: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F5 |
| F6 | 0.270 / 0.045000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F7: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F6 |
| F7 | 0.315 / 0.080000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F8: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F7 |
| F8 | 0.395 / 0.078000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F9: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F8 |
| F9 | 0.473 / 0.060000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F10: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F9 |
| F10 | 0.533 / 0.084000 | Player đứng thấp, khăn quét trái; skeleton giữ kiếm chéo rồi gồng; hai chân còn trên landmark sàn. | → F11: Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F10 |
| F11 | 0.617 / 0.050000 | Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | → F12: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F11 |
| F12 | 0.667 / 0.026000 | Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | → F13: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F12 |
| F13 | 0.693 / 0.029000 | Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | → F14: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F13 |
| F14 | 0.722 / 0.050000 | Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | → F15: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F14 |
| F15 | 0.772 / 0.039000 | Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | → F16: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F15 |
| F16 | 0.811 / 0.028000 | Player hạ thân chém phải, cung trắng phủ ngực skeleton; +10 và spark sáng; chân trụ không tách khỏi sàn. | → F17: Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F16 |
| F17 | 0.839 / 0.042000 | Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | → F18: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F17 |
| F18 | 0.881 / 0.059000 | Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | → F19: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F18 |
| F19 | 0.940 / 0.046000 | Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | → F20: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F19 |
| F20 | 0.986 / 0.061000 | Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | → F21: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F20 |
| F21 | 1.047 / 0.036000 | Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | → F22: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F21 |
| F22 | 1.083 / 0.047000 | Cung thu ngắn về kiếm thấp; skeleton xoay vai chuẩn bị đánh; ánh thân trở lại, không thấy đổi vị trí chân đột ngột. | → F23: Player dựng kiếm trước mặt vào parry; skeleton kéo kiếm ngang sau vai; grip nối tay, không cắt mũi kiếm. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F22 |
| F23 | 1.130 / 0.040000 | Player dựng kiếm trước mặt vào parry; skeleton kéo kiếm ngang sau vai; grip nối tay, không cắt mũi kiếm. | → F24: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F23 |
| F24 | 1.170 / 0.030000 | Player dựng kiếm trước mặt vào parry; skeleton kéo kiếm ngang sau vai; grip nối tay, không cắt mũi kiếm. | → F25: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F24 |
| F25 | 1.200 / 0.054000 | Player dựng kiếm trước mặt vào parry; skeleton kéo kiếm ngang sau vai; grip nối tay, không cắt mũi kiếm. | → F26: Flash vàng tại hai lưỡi, +30; body skeleton tối trong flash, còn thấy chân và bóng; không chứng nhận chi tiết bị flash che. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F25 |
| F26 | 1.254 / 0.051000 | Flash vàng tại hai lưỡi, +30; body skeleton tối trong flash, còn thấy chân và bóng; không chứng nhận chi tiết bị flash che. | → F27: Player cúi phản kích, kiếm/vệt sáng thấp; kiếm skeleton duỗi qua đầu; spark tỏa rồi thu, lớp trước sau còn đọc được. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F26 |
| F27 | 1.305 / 0.049000 | Player cúi phản kích, kiếm/vệt sáng thấp; kiếm skeleton duỗi qua đầu; spark tỏa rồi thu, lớp trước sau còn đọc được. | → F28: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F27 |
| F28 | 1.354 / 0.074000 | Player cúi phản kích, kiếm/vệt sáng thấp; kiếm skeleton duỗi qua đầu; spark tỏa rồi thu, lớp trước sau còn đọc được. | → F29: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F28 |
| F29 | 1.428 / 0.146000 | Player cúi phản kích, kiếm/vệt sáng thấp; kiếm skeleton duỗi qua đầu; spark tỏa rồi thu, lớp trước sau còn đọc được. | → F30: Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F29 |
| F30 | 1.574 / 0.014000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F31: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F30 |
| F31 | 1.588 / 0.039000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F32: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F31 |
| F32 | 1.627 / 0.023000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F33: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F32 |
| F33 | 1.650 / 0.050000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F34: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F33 |
| F34 | 1.700 / 0.022000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F35: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F34 |
| F35 | 1.722 / 0.023000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F36: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F35 |
| F36 | 1.745 / 0.027000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F37: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F36 |
| F37 | 1.772 / 0.052000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F38: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F37 |
| F38 | 1.824 / 0.015000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F39: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F38 |
| F39 | 1.839 / 0.045000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F40: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F39 |
| F40 | 1.884 / 0.051000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F41: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F40 |
| F41 | 1.935 / 0.025000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F42: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F41 |
| F42 | 1.960 / 0.033000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F43: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F42 |
| F43 | 1.993 / 0.023000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F44: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F43 |
| F44 | 2.016 / 0.027000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F45: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F44 |
| F45 | 2.043 / 0.088000 | Player giữ cúi thấp cùng kiếm dài sát sàn; skeleton thu tay và kiếm về đứng; spark đỏ tàn, chân không nhảy theo flash. | → F46: Player nâng kiếm rồi đứng lại; skeleton idle kiếm chéo; bóng dưới chân giữ cùng bên sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F45 |
| F46 | 2.131 / 0.034000 | Player nâng kiếm rồi đứng lại; skeleton idle kiếm chéo; bóng dưới chân giữ cùng bên sàn. | → F47: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F46 |
| F47 | 2.165 / 0.064000 | Player nâng kiếm rồi đứng lại; skeleton idle kiếm chéo; bóng dưới chân giữ cùng bên sàn. | → F48: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F47 |
| F48 | 2.229 / 0.066000 | Player nâng kiếm rồi đứng lại; skeleton idle kiếm chéo; bóng dưới chân giữ cùng bên sàn. | → F49: Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F48 |
| F49 | 2.295 / 0.044000 | Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | → F50: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F49 |
| F50 | 2.339 / 0.060000 | Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | → F51: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F50 |
| F51 | 2.399 / 0.041000 | Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | → F52: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F51 |
| F52 | 2.440 / 0.065000 | Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | → F53: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F52 |
| F53 | 2.505 / 0.047000 | Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | → F54: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F53 |
| F54 | 2.552 / 0.063000 | Đòn chém thường nở cung trắng phải; skeleton flash trắng rồi co thân Hurt; cung che một phần tay/ngực, chân vẫn đọc được. | → F55: Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F54 |
| F55 | 2.615 / 0.043000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F56: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F55 |
| F56 | 2.658 / 0.033000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F57: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F56 |
| F57 | 2.691 / 0.065000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F58: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F57 |
| F58 | 2.756 / 0.045000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F59: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F58 |
| F59 | 2.801 / 0.042000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F60: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F59 |
| F60 | 2.843 / 0.076000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F61: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F60 |
| F61 | 2.919 / 0.076000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F62: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F61 |
| F62 | 2.995 / 0.068000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F63: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F62 |
| F63 | 3.063 / 0.073000 | Player thu cung về kiếm thấp rồi đứng; skeleton hết trắng trở lại idle; highlight ấm trở lại trên xương, không lật sáng rõ. | → F64: Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F63 |
| F64 | 3.136 / 0.036000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F65: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F64 |
| F65 | 3.172 / 0.018000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F66: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F65 |
| F66 | 3.190 / 0.046000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F67: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F66 |
| F67 | 3.236 / 0.044000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F68: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F67 |
| F68 | 3.280 / 0.063000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F69: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F68 |
| F69 | 3.343 / 0.030000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F70: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F69 |
| F70 | 3.373 / 0.120000 | Lặp chém thường với cung/spark; Hurt trắng của skeleton có nhịp riêng; chân trụ và khu vực bóng vẫn liền sàn. | → F71: Cung tan, player thu kiếm thấp và đứng lên; skeleton trở về tư thế kiếm chéo, không còn lõi flash. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F70 |
| F71 | 3.493 / 0.020000 | Cung tan, player thu kiếm thấp và đứng lên; skeleton trở về tư thế kiếm chéo, không còn lõi flash. | → F72: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F71 |
| F72 | 3.513 / 0.091000 | Cung tan, player thu kiếm thấp và đứng lên; skeleton trở về tư thế kiếm chéo, không còn lõi flash. | → F73: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F72 |
| F73 | 3.604 / 0.092000 | Cung tan, player thu kiếm thấp và đứng lên; skeleton trở về tư thế kiếm chéo, không còn lõi flash. | → F74: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F73 |
| F74 | 3.696 / 0.051000 | Cung tan, player thu kiếm thấp và đứng lên; skeleton trở về tư thế kiếm chéo, không còn lõi flash. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F74 |

#### Runtime `sword-death` — 72 frame / 71 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F583 | 23.805 / 0.043000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F584: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F583 |
| F584 | 23.848 / 0.037000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F585: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F584 |
| F585 | 23.885 / 0.027000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F586: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F585 |
| F586 | 23.912 / 0.026000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F587: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F586 |
| F587 | 23.938 / 0.045000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F588: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F587 |
| F588 | 23.983 / 0.021000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F589: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F588 |
| F589 | 24.004 / 0.044000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F590: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F589 |
| F590 | 24.048 / 0.024000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F591: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F590 |
| F591 | 24.072 / 0.043000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F592: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F591 |
| F592 | 24.115 / 0.026000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F593: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F592 |
| F593 | 24.141 / 0.042000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F594: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F593 |
| F594 | 24.183 / 0.022000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F595: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F594 |
| F595 | 24.205 / 0.054000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F596: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F595 |
| F596 | 24.259 / 0.014000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F597: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F596 |
| F597 | 24.273 / 0.052000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F598: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F597 |
| F598 | 24.325 / 0.021000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F599: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F598 |
| F599 | 24.346 / 0.040000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F600: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F599 |
| F600 | 24.386 / 0.041000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F601: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F600 |
| F601 | 24.427 / 0.013000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F602: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F601 |
| F602 | 24.440 / 0.049000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F603: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F602 |
| F603 | 24.489 / 0.023000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F604: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F603 |
| F604 | 24.512 / 0.027000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F605: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F604 |
| F605 | 24.539 / 0.043000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F606: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F605 |
| F606 | 24.582 / 0.039000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F607: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F606 |
| F607 | 24.621 / 0.027000 | Slayer nền đỏ, player/skeleton silhouette đen; cung trắng và flash body xen kẽ; chân còn đủ trong arena rộng, shading bị mode che theo thiết kế. | → F608: Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F607 |
| F608 | 24.648 / 0.024000 | Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | → F609: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F608 |
| F609 | 24.672 / 0.052000 | Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | → F610: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F609 |
| F610 | 24.724 / 0.030000 | Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | → F611: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F610 |
| F611 | 24.754 / 0.024000 | Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | → F612: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F611 |
| F612 | 24.778 / 0.026000 | Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | → F613: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F612 |
| F613 | 24.804 / 0.046000 | Đòn lethal làm body skeleton sáng; vệt đen ngang ngực là hiệu ứng contact, player hạ kiếm rồi thu; không dùng vùng bị phủ để kết luận anatomy. | → F614: Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F613 |
| F614 | 24.850 / 0.022000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F615: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F614 |
| F615 | 24.872 / 0.039000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F616: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F615 |
| F616 | 24.911 / 0.034000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F617: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F616 |
| F617 | 24.945 / 0.033000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F618: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F617 |
| F618 | 24.978 / 0.026000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F619: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F618 |
| F619 | 25.004 / 0.043000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F620: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F619 |
| F620 | 25.047 / 0.024000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F621: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F620 |
| F621 | 25.071 / 0.052000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F622: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F621 |
| F622 | 25.123 / 0.023000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F623: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F622 |
| F623 | 25.146 / 0.040000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F624: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F623 |
| F624 | 25.186 / 0.020000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F625: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F624 |
| F625 | 25.206 / 0.047000 | Slayer tắt dần, camera lùi và sàn hiện trở lại; sọ tách, ngực/eo rã; chân player vẫn ở sàn trong chuyển zoom. | → F626: Ngực/chi hạ về sàn, kiếm rơi ngang; player về idle, màu/normal shading trở lại dần, không thấy actor đổi thứ tự lớp bất thường. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F625 |
| F626 | 25.253 / 0.025000 | Ngực/chi hạ về sàn, kiếm rơi ngang; player về idle, màu/normal shading trở lại dần, không thấy actor đổi thứ tự lớp bất thường. | → F627: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F626 |
| F627 | 25.278 / 0.037000 | Ngực/chi hạ về sàn, kiếm rơi ngang; player về idle, màu/normal shading trở lại dần, không thấy actor đổi thứ tự lớp bất thường. | → F628: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F627 |
| F628 | 25.315 / 0.023000 | Ngực/chi hạ về sàn, kiếm rơi ngang; player về idle, màu/normal shading trở lại dần, không thấy actor đổi thứ tự lớp bất thường. | → F629: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F628 |
| F629 | 25.338 / 0.046000 | Ngực/chi hạ về sàn, kiếm rơi ngang; player về idle, màu/normal shading trở lại dần, không thấy actor đổi thứ tự lớp bất thường. | → F630: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F629 |
| F630 | 25.384 / 0.042000 | Ngực/chi hạ về sàn, kiếm rơi ngang; player về idle, màu/normal shading trở lại dần, không thấy actor đổi thứ tự lớp bất thường. | → F631: Sọ hạ xuống đống xương, kiếm ngang sàn; camera trở lại bố cục thường, bóng tiếp xúc thấp đi cùng phần rơi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F630 |
| F631 | 25.426 / 0.021000 | Sọ hạ xuống đống xương, kiếm ngang sàn; camera trở lại bố cục thường, bóng tiếp xúc thấp đi cùng phần rơi. | → F632: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F631 |
| F632 | 25.447 / 0.030000 | Sọ hạ xuống đống xương, kiếm ngang sàn; camera trở lại bố cục thường, bóng tiếp xúc thấp đi cùng phần rơi. | → F633: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F632 |
| F633 | 25.477 / 0.040000 | Sọ hạ xuống đống xương, kiếm ngang sàn; camera trở lại bố cục thường, bóng tiếp xúc thấp đi cùng phần rơi. | → F634: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F633 |
| F634 | 25.517 / 0.027000 | Sọ hạ xuống đống xương, kiếm ngang sàn; camera trở lại bố cục thường, bóng tiếp xúc thấp đi cùng phần rơi. | → F635: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F634 |
| F635 | 25.544 / 0.038000 | Sọ hạ xuống đống xương, kiếm ngang sàn; camera trở lại bố cục thường, bóng tiếp xúc thấp đi cùng phần rơi. | → F636: Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F635 |
| F636 | 25.582 / 0.022000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F637: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F636 |
| F637 | 25.604 / 0.045000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F638: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F637 |
| F638 | 25.649 / 0.036000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F639: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F638 |
| F639 | 25.685 / 0.026000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F640: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F639 |
| F640 | 25.711 / 0.027000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F641: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F640 |
| F641 | 25.738 / 0.045000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F642: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F641 |
| F642 | 25.783 / 0.021000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F643: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F642 |
| F643 | 25.804 / 0.043000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F644: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F643 |
| F644 | 25.847 / 0.030000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F645: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F644 |
| F645 | 25.877 / 0.036000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F646: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F645 |
| F646 | 25.913 / 0.027000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F647: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F646 |
| F647 | 25.940 / 0.038000 | Đống xương và kiếm giữ trên sàn, player idle; không ép animation death loop, không thấy residue bay độc lập. | → F648: Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F647 |
| F648 | 25.978 / 0.028000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | → F649: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F648 |
| F649 | 26.006 / 0.039000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | → F650: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F649 |
| F650 | 26.045 / 0.027000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | → F651: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F650 |
| F651 | 26.072 / 0.041000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | → F652: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F651 |
| F652 | 26.113 / 0.031000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | → F653: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F652 |
| F653 | 26.144 / 0.035000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | → F654: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F653 |
| F654 | 26.179 / 0.026000 | Overlay chuyển đối thủ che vùng actor sau cleanup; không chứng nhận hình bị overlay che, nhãn Double Strike vẫn đọc được trong context rộng. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F654 |

#### Runtime `sword-attack2-idle` — 148 frame / 147 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F697 | 27.620 / 0.026000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F698: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F697 |
| F698 | 27.646 / 0.040000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F699: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F698 |
| F699 | 27.686 / 0.045000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F700: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F699 |
| F700 | 27.731 / 0.009000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F701: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F700 |
| F701 | 27.740 / 0.039000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F702: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F701 |
| F702 | 27.779 / 0.027000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F703: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F702 |
| F703 | 27.806 / 0.040000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F704: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F703 |
| F704 | 27.846 / 0.026000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F705: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F704 |
| F705 | 27.872 / 0.048000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F706: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F705 |
| F706 | 27.920 / 0.024000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F707: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F706 |
| F707 | 27.944 / 0.035000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F708: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F707 |
| F708 | 27.979 / 0.029000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F709: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F708 |
| F709 | 28.008 / 0.040000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F710: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F709 |
| F710 | 28.048 / 0.040000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F711: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F710 |
| F711 | 28.088 / 0.025000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F712: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F711 |
| F712 | 28.113 / 0.032000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F713: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F712 |
| F713 | 28.145 / 0.037000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F714: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F713 |
| F714 | 28.182 / 0.024000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F715: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F714 |
| F715 | 28.206 / 0.041000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F716: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F715 |
| F716 | 28.247 / 0.026000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F717: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F716 |
| F717 | 28.273 / 0.050000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F718: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F717 |
| F718 | 28.323 / 0.029000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F719: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F718 |
| F719 | 28.352 / 0.032000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F720: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F719 |
| F720 | 28.384 / 0.031000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F721: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F720 |
| F721 | 28.415 / 0.038000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F722: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F721 |
| F722 | 28.453 / 0.022000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F723: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F722 |
| F723 | 28.475 / 0.040000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F724: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F723 |
| F724 | 28.515 / 0.033000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F725: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F724 |
| F725 | 28.548 / 0.022000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F726: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F725 |
| F726 | 28.570 / 0.038000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F727: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F726 |
| F727 | 28.608 / 0.038000 | Hai actor đứng, player có nhịp khăn/tay; skeleton giữ kiếm chéo trong cue, chân và bóng cùng sàn; không thấy mảnh rời quanh chân. | → F728: Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F727 |
| F728 | 28.646 / 0.039000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F729: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F728 |
| F729 | 28.685 / 0.028000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F730: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F729 |
| F730 | 28.713 / 0.026000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F731: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F730 |
| F731 | 28.739 / 0.031000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F732: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F731 |
| F732 | 28.770 / 0.038000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F733: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F732 |
| F733 | 28.808 / 0.025000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F734: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F733 |
| F734 | 28.833 / 0.047000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F735: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F734 |
| F735 | 28.880 / 0.020000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F736: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F735 |
| F736 | 28.900 / 0.040000 | Skeleton xoay kiếm qua vai vào Attack1, chân mở trụ; player vẫn chuẩn bị thấp, mũi kiếm không cắt mép crop. | → F737: Player dựng kiếm parry; skeleton gồng kiếm sau vai, chân không đổi landmark riêng khỏi sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F736 |
| F737 | 28.940 / 0.041000 | Player dựng kiếm parry; skeleton gồng kiếm sau vai, chân không đổi landmark riêng khỏi sàn. | → F738: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F737 |
| F738 | 28.981 / 0.025000 | Player dựng kiếm parry; skeleton gồng kiếm sau vai, chân không đổi landmark riêng khỏi sàn. | → F739: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F738 |
| F739 | 29.006 / 0.042000 | Player dựng kiếm parry; skeleton gồng kiếm sau vai, chân không đổi landmark riêng khỏi sàn. | → F740: Flash vàng ở lưỡi và +30; skeleton tối tạm, tay/điểm contact bị flash che, không suy ra lỗi normal từ màu tối này. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F739 |
| F740 | 29.048 / 0.029000 | Flash vàng ở lưỡi và +30; skeleton tối tạm, tay/điểm contact bị flash che, không suy ra lỗi normal từ màu tối này. | → F741: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F740 |
| F741 | 29.077 / 0.042000 | Flash vàng ở lưỡi và +30; skeleton tối tạm, tay/điểm contact bị flash che, không suy ra lỗi normal từ màu tối này. | → F742: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F741 |
| F742 | 29.119 / 0.025000 | Flash vàng ở lưỡi và +30; skeleton tối tạm, tay/điểm contact bị flash che, không suy ra lỗi normal từ màu tối này. | → F743: Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F742 |
| F743 | 29.144 / 0.037000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F744: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F743 |
| F744 | 29.181 / 0.034000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F745: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F744 |
| F745 | 29.215 / 0.034000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F746: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F745 |
| F746 | 29.249 / 0.038000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F747: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F746 |
| F747 | 29.287 / 0.043000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F748: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F747 |
| F748 | 29.330 / 0.026000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F749: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F748 |
| F749 | 29.356 / 0.055000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F750: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F749 |
| F750 | 29.411 / 0.041000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F751: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F750 |
| F751 | 29.452 / 0.044000 | Player cúi phản kích, skeleton duỗi kiếm qua trái; spark vàng/đỏ giảm dần, thứ tự lưỡi trước thân đọc được. | → F752: Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F751 |
| F752 | 29.496 / 0.021000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F753: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F752 |
| F753 | 29.517 / 0.027000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F754: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F753 |
| F754 | 29.544 / 0.041000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F755: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F754 |
| F755 | 29.585 / 0.024000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F756: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F755 |
| F756 | 29.609 / 0.027000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F757: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F756 |
| F757 | 29.636 / 0.038000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F758: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F757 |
| F758 | 29.674 / 0.040000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F759: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F758 |
| F759 | 29.714 / 0.026000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F760: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F759 |
| F760 | 29.740 / 0.046000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F761: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F760 |
| F761 | 29.786 / 0.025000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F762: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F761 |
| F762 | 29.811 / 0.036000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F763: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F762 |
| F763 | 29.847 / 0.026000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F764: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F763 |
| F764 | 29.873 / 0.028000 | Skeleton thu kiếm về chéo và idle, player giữ kiếm thấp; hạt đỏ tan, highlight xương ấm trở lại. | → F765: Player thu/nâng kiếm, skeleton chuyển sang tư thế Attack2 giữ kiếm ngang eo; không thấy lệch grip hoặc nhảy chân. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F764 |
| F765 | 29.901 / 0.053000 | Player thu/nâng kiếm, skeleton chuyển sang tư thế Attack2 giữ kiếm ngang eo; không thấy lệch grip hoặc nhảy chân. | → F766: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F765 |
| F766 | 29.954 / 0.027000 | Player thu/nâng kiếm, skeleton chuyển sang tư thế Attack2 giữ kiếm ngang eo; không thấy lệch grip hoặc nhảy chân. | → F767: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F766 |
| F767 | 29.981 / 0.027000 | Player thu/nâng kiếm, skeleton chuyển sang tư thế Attack2 giữ kiếm ngang eo; không thấy lệch grip hoặc nhảy chân. | → F768: Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F767 |
| F768 | 30.008 / 0.046000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F769: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F768 |
| F769 | 30.054 / 0.020000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F770: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F769 |
| F770 | 30.074 / 0.032000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F771: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F770 |
| F771 | 30.106 / 0.034000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F772: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F771 |
| F772 | 30.140 / 0.043000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F773: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F772 |
| F773 | 30.183 / 0.028000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F774: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F773 |
| F774 | 30.211 / 0.022000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F775: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F774 |
| F775 | 30.233 / 0.041000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F776: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F775 |
| F776 | 30.274 / 0.040000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F777: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F776 |
| F777 | 30.314 / 0.026000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F778: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F777 |
| F778 | 30.340 / 0.045000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F779: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F778 |
| F779 | 30.385 / 0.038000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F780: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F779 |
| F780 | 30.423 / 0.044000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F781: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F780 |
| F781 | 30.467 / 0.026000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F782: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F781 |
| F782 | 30.493 / 0.034000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F783: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F782 |
| F783 | 30.527 / 0.050000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F784: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F783 |
| F784 | 30.577 / 0.033000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F785: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F784 |
| F785 | 30.610 / 0.041000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F786: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F785 |
| F786 | 30.651 / 0.030000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F787: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F786 |
| F787 | 30.681 / 0.022000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F788: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F787 |
| F788 | 30.703 / 0.042000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F789: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F788 |
| F789 | 30.745 / 0.037000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F790: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F789 |
| F790 | 30.782 / 0.045000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F791: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F790 |
| F791 | 30.827 / 0.025000 | Skeleton giữ chuẩn bị đâm ngang, chân mở; player idle/khăn dao động, cue giữ; bóng/landmark không trượt riêng. | → F792: Player đưa kiếm đứng trước mặt để parry đâm; skeleton còn giữ kiếm ngang, hai body còn tách nhau. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F791 |
| F792 | 30.852 / 0.023000 | Player đưa kiếm đứng trước mặt để parry đâm; skeleton còn giữ kiếm ngang, hai body còn tách nhau. | → F793: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F792 |
| F793 | 30.875 / 0.047000 | Player đưa kiếm đứng trước mặt để parry đâm; skeleton còn giữ kiếm ngang, hai body còn tách nhau. | → F794: Flash parry và +30, thân skeleton tối; vùng lõi sáng che contact nên chỉ xác nhận outline và chân ngoài lõi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F793 |
| F794 | 30.922 / 0.023000 | Flash parry và +30, thân skeleton tối; vùng lõi sáng che contact nên chỉ xác nhận outline và chân ngoài lõi. | → F795: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F794 |
| F795 | 30.945 / 0.027000 | Flash parry và +30, thân skeleton tối; vùng lõi sáng che contact nên chỉ xác nhận outline và chân ngoài lõi. | → F796: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F795 |
| F796 | 30.972 / 0.034000 | Flash parry và +30, thân skeleton tối; vùng lõi sáng che contact nên chỉ xác nhận outline và chân ngoài lõi. | → F797: Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F796 |
| F797 | 31.006 / 0.042000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F798: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F797 |
| F798 | 31.048 / 0.030000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F799: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F798 |
| F799 | 31.078 / 0.041000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F800: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F799 |
| F800 | 31.119 / 0.021000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F801: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F800 |
| F801 | 31.140 / 0.040000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F802: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F801 |
| F802 | 31.180 / 0.038000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F803: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F802 |
| F803 | 31.218 / 0.018000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F804: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F803 |
| F804 | 31.236 / 0.038000 | Đâm Attack2 duỗi sang player cúi, spark dọc lưỡi thu nhỏ; tay nối cán, shadow cùng phần chân trụ. | → F805: Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F804 |
| F805 | 31.274 / 0.042000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F806: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F805 |
| F806 | 31.316 / 0.025000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F807: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F806 |
| F807 | 31.341 / 0.028000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F808: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F807 |
| F808 | 31.369 / 0.084000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F809: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F808 |
| F809 | 31.453 / 0.023000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F810: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F809 |
| F810 | 31.476 / 0.038000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F811: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F810 |
| F811 | 31.514 / 0.030000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F812: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F811 |
| F812 | 31.544 / 0.039000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F813: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F812 |
| F813 | 31.583 / 0.025000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F814: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F813 |
| F814 | 31.608 / 0.039000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F815: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F814 |
| F815 | 31.647 / 0.034000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F816: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F815 |
| F816 | 31.681 / 0.019000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F817: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F816 |
| F817 | 31.700 / 0.041000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F818: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F817 |
| F818 | 31.741 / 0.039000 | Skeleton thu kiếm về chéo rồi đứng; player còn cúi thấp, hạt đỏ tan, không giữ lại vệt sáng sau recovery. | → F819: Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F818 |
| F819 | 31.780 / 0.030000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F820: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F819 |
| F820 | 31.810 / 0.027000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F821: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F820 |
| F821 | 31.837 / 0.051000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F822: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F821 |
| F822 | 31.888 / 0.012000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F823: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F822 |
| F823 | 31.900 / 0.041000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F824: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F823 |
| F824 | 31.941 / 0.054000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F825: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F824 |
| F825 | 31.995 / 0.022000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F826: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F825 |
| F826 | 32.017 / 0.034000 | Player nâng/thu kiếm và đứng; skeleton idle với thay đổi cao thấp đầu nhỏ, chân giữ sàn. | → F827: Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F826 |
| F827 | 32.051 / 0.038000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F828: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F827 |
| F828 | 32.089 / 0.016000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F829: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F828 |
| F829 | 32.105 / 0.037000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F830: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F829 |
| F830 | 32.142 / 0.028000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F831: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F830 |
| F831 | 32.170 / 0.039000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F832: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F831 |
| F832 | 32.209 / 0.039000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F833: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F832 |
| F833 | 32.248 / 0.033000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F834: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F833 |
| F834 | 32.281 / 0.036000 | Một đòn thường nở cung trắng vào skeleton; ngực/tay bị cung che một phần, chân và outline đầu còn đọc được. | → F835: Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F834 |
| F835 | 32.317 / 0.026000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F836: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F835 |
| F836 | 32.343 / 0.045000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F837: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F836 |
| F837 | 32.388 / 0.056000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F838: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F837 |
| F838 | 32.444 / 0.039000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F839: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F838 |
| F839 | 32.483 / 0.019000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F840: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F839 |
| F840 | 32.502 / 0.044000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F841: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F840 |
| F841 | 32.546 / 0.024000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F842: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F841 |
| F842 | 32.570 / 0.039000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F843: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F842 |
| F843 | 32.609 / 0.028000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | → F844: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F843 |
| F844 | 32.637 / 0.072000 | Cung thu thành kiếm thấp, player đứng lại; skeleton về kiếm chéo, màu sáng trên xương ổn định trong các frame cuối. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F844 |

#### Runtime `axe-idle-attack-hurt` — 108 frame / 107 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F1565 | 64.037 / 0.063000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1566: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1565 |
| F1566 | 64.100 / 0.028000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1567: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1566 |
| F1567 | 64.128 / 0.030000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1568: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1567 |
| F1568 | 64.158 / 0.049000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1569: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1568 |
| F1569 | 64.207 / 0.027000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1570: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1569 |
| F1570 | 64.234 / 0.034000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1571: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1570 |
| F1571 | 64.268 / 0.039000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1572: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1571 |
| F1572 | 64.307 / 0.062000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1573: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1572 |
| F1573 | 64.369 / 0.058000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1574: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1573 |
| F1574 | 64.427 / 0.082000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1575: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1574 |
| F1575 | 64.509 / 0.046000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1576: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1575 |
| F1576 | 64.555 / 0.054000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1577: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1576 |
| F1577 | 64.609 / 0.037000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1578: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1577 |
| F1578 | 64.646 / 0.053000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1579: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1578 |
| F1579 | 64.699 / 0.040000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1580: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1579 |
| F1580 | 64.739 / 0.016000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1581: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1580 |
| F1581 | 64.755 / 0.043000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1582: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1581 |
| F1582 | 64.798 / 0.025000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1583: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1582 |
| F1583 | 64.823 / 0.046000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1584: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1583 |
| F1584 | 64.869 / 0.027000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1585: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1584 |
| F1585 | 64.896 / 0.036000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1586: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1585 |
| F1586 | 64.932 / 0.042000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1587: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1586 |
| F1587 | 64.974 / 0.034000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1588: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1587 |
| F1588 | 65.008 / 0.058000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1589: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1588 |
| F1589 | 65.066 / 0.029000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1590: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1589 |
| F1590 | 65.095 / 0.047000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1591: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1590 |
| F1591 | 65.142 / 0.014000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1592: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1591 |
| F1592 | 65.156 / 0.037000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1593: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1592 |
| F1593 | 65.193 / 0.030000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1594: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1593 |
| F1594 | 65.223 / 0.094000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1595: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1594 |
| F1595 | 65.317 / 0.054000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1596: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1595 |
| F1596 | 65.371 / 0.065000 | Rìu giữ cán dọc, đầu có highlight/bloom trắng, skeleton thở nâng hạ nhỏ; player idle và khăn chuyển; chân và vùng bóng không tách nhau. | → F1597: Skeleton kéo rìu chéo qua ngực, hai tay nối cán; đầu rìu đổi hướng liên tục, không thấy miếng Walk rời cạnh chân. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1596 |
| F1597 | 65.436 / 0.020000 | Skeleton kéo rìu chéo qua ngực, hai tay nối cán; đầu rìu đổi hướng liên tục, không thấy miếng Walk rời cạnh chân. | → F1598: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1597 |
| F1598 | 65.456 / 0.043000 | Skeleton kéo rìu chéo qua ngực, hai tay nối cán; đầu rìu đổi hướng liên tục, không thấy miếng Walk rời cạnh chân. | → F1599: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1598 |
| F1599 | 65.499 / 0.048000 | Skeleton kéo rìu chéo qua ngực, hai tay nối cán; đầu rìu đổi hướng liên tục, không thấy miếng Walk rời cạnh chân. | → F1600: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1599 |
| F1600 | 65.547 / 0.035000 | Skeleton kéo rìu chéo qua ngực, hai tay nối cán; đầu rìu đổi hướng liên tục, không thấy miếng Walk rời cạnh chân. | → F1601: Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1600 |
| F1601 | 65.582 / 0.041000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1602: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1601 |
| F1602 | 65.623 / 0.036000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1603: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1602 |
| F1603 | 65.659 / 0.030000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1604: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1603 |
| F1604 | 65.689 / 0.041000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1605: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1604 |
| F1605 | 65.730 / 0.020000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1606: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1605 |
| F1606 | 65.750 / 0.045000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1607: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1606 |
| F1607 | 65.795 / 0.013000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1608: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1607 |
| F1608 | 65.808 / 0.050000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1609: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1608 |
| F1609 | 65.858 / 0.020000 | Rìu quét xuống thấp bên trái rồi giữ chuẩn bị; chân mở trụ, player vẫn thấp, cán và tay còn nối. | → F1610: Player dựng kiếm parry; rìu còn thấp trong held pose, không nhầm thời gian giữ với frame mất. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1609 |
| F1610 | 65.878 / 0.067000 | Player dựng kiếm parry; rìu còn thấp trong held pose, không nhầm thời gian giữ với frame mất. | → F1611: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1610 |
| F1611 | 65.945 / 0.014000 | Player dựng kiếm parry; rìu còn thấp trong held pose, không nhầm thời gian giữ với frame mất. | → F1612: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1611 |
| F1612 | 65.959 / 0.043000 | Player dựng kiếm parry; rìu còn thấp trong held pose, không nhầm thời gian giữ với frame mất. | → F1613: Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1612 |
| F1613 | 66.002 / 0.050000 | Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | → F1614: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1613 |
| F1614 | 66.052 / 0.054000 | Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | → F1615: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1614 |
| F1615 | 66.106 / 0.018000 | Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | → F1616: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1615 |
| F1616 | 66.124 / 0.051000 | Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | → F1617: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1616 |
| F1617 | 66.175 / 0.015000 | Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | → F1618: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1617 |
| F1618 | 66.190 / 0.041000 | Flash vàng và +30 khi rìu nâng lên; body tối tạm rồi sáng, spark che đầu rìu/contact nhưng chân ngoài lõi vẫn đọc được. | → F1619: Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1618 |
| F1619 | 66.231 / 0.040000 | Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | → F1620: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1619 |
| F1620 | 66.271 / 0.039000 | Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | → F1621: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1620 |
| F1621 | 66.310 / 0.037000 | Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | → F1622: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1621 |
| F1622 | 66.347 / 0.097000 | Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | → F1623: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1622 |
| F1623 | 66.444 / 0.044000 | Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | → F1624: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1623 |
| F1624 | 66.488 / 0.075000 | Rìu thu lên qua vai về dọc, spark nhỏ dần; player cúi phản kích với kiếm thấp, thứ tự lớp không đảo rõ. | → F1625: Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1624 |
| F1625 | 66.563 / 0.035000 | Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | → F1626: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1625 |
| F1626 | 66.598 / 0.028000 | Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | → F1627: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1626 |
| F1627 | 66.626 / 0.048000 | Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | → F1628: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1627 |
| F1628 | 66.674 / 0.044000 | Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | → F1629: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1628 |
| F1629 | 66.718 / 0.045000 | Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | → F1630: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1629 |
| F1630 | 66.763 / 0.066000 | Skeleton về idle cán dọc, player còn thấp; hạt đỏ tàn, highlight đầu rìu trở lại cùng pose đứng. | → F1631: Player nâng kiếm và thu về, skeleton idle; không có mảng nâu độc lập bên chân trong các frame này. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1630 |
| F1631 | 66.829 / 0.027000 | Player nâng kiếm và thu về, skeleton idle; không có mảng nâu độc lập bên chân trong các frame này. | → F1632: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1631 |
| F1632 | 66.856 / 0.044000 | Player nâng kiếm và thu về, skeleton idle; không có mảng nâu độc lập bên chân trong các frame này. | → F1633: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1632 |
| F1633 | 66.900 / 0.030000 | Player nâng kiếm và thu về, skeleton idle; không có mảng nâu độc lập bên chân trong các frame này. | → F1634: Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1633 |
| F1634 | 66.930 / 0.027000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1635: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1634 |
| F1635 | 66.957 / 0.052000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1636: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1635 |
| F1636 | 67.009 / 0.062000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1637: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1636 |
| F1637 | 67.071 / 0.021000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1638: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1637 |
| F1638 | 67.092 / 0.021000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1639: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1638 |
| F1639 | 67.113 / 0.058000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1640: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1639 |
| F1640 | 67.171 / 0.017000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1641: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1640 |
| F1641 | 67.188 / 0.056000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1642: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1641 |
| F1642 | 67.244 / 0.011000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1643: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1642 |
| F1643 | 67.255 / 0.056000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1644: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1643 |
| F1644 | 67.311 / 0.019000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1645: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1644 |
| F1645 | 67.330 / 0.017000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1646: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1645 |
| F1646 | 67.347 / 0.057000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1647: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1646 |
| F1647 | 67.404 / 0.098000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1648: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1647 |
| F1648 | 67.502 / 0.089000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1649: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1648 |
| F1649 | 67.591 / 0.072000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1650: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1649 |
| F1650 | 67.663 / 0.089000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1651: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1650 |
| F1651 | 67.752 / 0.077000 | Cả hai idle, đầu rìu sáng theo ánh kim/bloom; score nổi rồi nhạt; chân giữ cùng vùng gạch. | → F1652: Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1651 |
| F1652 | 67.829 / 0.055000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1653: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1652 |
| F1653 | 67.884 / 0.043000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1654: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1653 |
| F1654 | 67.927 / 0.031000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1655: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1654 |
| F1655 | 67.958 / 0.047000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1656: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1655 |
| F1656 | 68.005 / 0.071000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1657: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1656 |
| F1657 | 68.076 / 0.056000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1658: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1657 |
| F1658 | 68.132 / 0.012000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1659: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1658 |
| F1659 | 68.144 / 0.065000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1660: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1659 |
| F1660 | 68.209 / 0.079000 | Chém thường với cung trắng, skeleton Hurt flash trắng cả rìu rồi hồi; cung che ngực nhưng outline chân còn liền sàn. | → F1661: Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1660 |
| F1661 | 68.288 / 0.083000 | Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | → F1662: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1661 |
| F1662 | 68.371 / 0.091000 | Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | → F1663: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1662 |
| F1663 | 68.462 / 0.029000 | Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | → F1664: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1663 |
| F1664 | 68.491 / 0.036000 | Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | → F1665: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1664 |
| F1665 | 68.527 / 0.052000 | Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | → F1666: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1665 |
| F1666 | 68.579 / 0.047000 | Đòn thường kế tiếp tích +20, lặp Hurt trắng rồi hết flash; cán dọc không rời grip khi hạ thân. | → F1667: Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1666 |
| F1667 | 68.626 / 0.012000 | Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | → F1668: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1667 |
| F1668 | 68.638 / 0.061000 | Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | → F1669: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1668 |
| F1669 | 68.699 / 0.038000 | Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | → F1670: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1669 |
| F1670 | 68.737 / 0.043000 | Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | → F1671: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1670 |
| F1671 | 68.780 / 0.080000 | Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | → F1672: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1671 |
| F1672 | 68.860 / 0.063000 | Player thu cung và đứng; skeleton về idle sáng ấm với đầu rìu trắng, không thấy residue của cung/spark. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F1672 |

#### Runtime `axe-death` — 61 frame / 60 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F2378 | 98.430 / 0.031000 | Hai silhouette trong Slayer đỏ giữ pose, rìu dọc; chân nguyên trong arena rộng, không đánh giá normal khi đang silhouette. | → F2379: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2378 |
| F2379 | 98.461 / 0.029000 | Hai silhouette trong Slayer đỏ giữ pose, rìu dọc; chân nguyên trong arena rộng, không đánh giá normal khi đang silhouette. | → F2380: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2379 |
| F2380 | 98.490 / 0.051000 | Hai silhouette trong Slayer đỏ giữ pose, rìu dọc; chân nguyên trong arena rộng, không đánh giá normal khi đang silhouette. | → F2381: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2380 |
| F2381 | 98.541 / 0.058000 | Hai silhouette trong Slayer đỏ giữ pose, rìu dọc; chân nguyên trong arena rộng, không đánh giá normal khi đang silhouette. | → F2382: Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2381 |
| F2382 | 98.599 / 0.048000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2383: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2382 |
| F2383 | 98.647 / 0.041000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2384: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2383 |
| F2384 | 98.688 / 0.042000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2385: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2384 |
| F2385 | 98.730 / 0.012000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2386: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2385 |
| F2386 | 98.742 / 0.067000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2387: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2386 |
| F2387 | 98.809 / 0.016000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2388: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2387 |
| F2388 | 98.825 / 0.048000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2389: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2388 |
| F2389 | 98.873 / 0.013000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2390: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2389 |
| F2390 | 98.886 / 0.052000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2391: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2390 |
| F2391 | 98.938 / 0.070000 | Đòn Slayer có cung trắng và flash xương/rìu, rồi tắt; player thu vũ khí, chân đứng trên đáy arena đỏ. | → F2392: Die F0: vạch ngà rời sát sàn bên trái cán rìu, tách khỏi player và chân; khớp mảnh khiên nâu trong extraction A1, thấy rõ ở crop nearest 4×. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2391 |
| F2392 | 99.008 / 0.078000 | Die F0: vạch ngà rời sát sàn bên trái cán rìu, tách khỏi player và chân; khớp mảnh khiên nâu trong extraction A1, thấy rõ ở crop nearest 4×. **A1: vạch ngà rời sát sàn giữa player và cán rìu, biến mất ở F2393.** | → F2393: Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | lỗi A1 | `combat` F2392 |
| F2393 | 99.086 / 0.034000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2394: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2393 |
| F2394 | 99.120 / 0.035000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2395: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2394 |
| F2395 | 99.155 / 0.038000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2396: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2395 |
| F2396 | 99.193 / 0.015000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2397: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2396 |
| F2397 | 99.208 / 0.038000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2398: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2397 |
| F2398 | 99.246 / 0.028000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2399: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2398 |
| F2399 | 99.274 / 0.063000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2400: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2399 |
| F2400 | 99.337 / 0.057000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2401: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2400 |
| F2401 | 99.394 / 0.033000 | Vạch rời mất ngay khi xương bắt đầu tách; sọ/ngực/eo rã, flash và nền đỏ nhạt dần; camera lùi cùng sàn, không thấy chân player trượt riêng. | → F2402: Sọ và sườn hạ, rìu nghiêng từ dọc; sàn/ánh sáng thường trở lại, thanh HP/word tray giữ ổn định ngoài world. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2401 |
| F2402 | 99.427 / 0.031000 | Sọ và sườn hạ, rìu nghiêng từ dọc; sàn/ánh sáng thường trở lại, thanh HP/word tray giữ ổn định ngoài world. | → F2403: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2402 |
| F2403 | 99.458 / 0.024000 | Sọ và sườn hạ, rìu nghiêng từ dọc; sàn/ánh sáng thường trở lại, thanh HP/word tray giữ ổn định ngoài world. | → F2404: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2403 |
| F2404 | 99.482 / 0.038000 | Sọ và sườn hạ, rìu nghiêng từ dọc; sàn/ánh sáng thường trở lại, thanh HP/word tray giữ ổn định ngoài world. | → F2405: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2404 |
| F2405 | 99.520 / 0.048000 | Sọ và sườn hạ, rìu nghiêng từ dọc; sàn/ánh sáng thường trở lại, thanh HP/word tray giữ ổn định ngoài world. | → F2406: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2405 |
| F2406 | 99.568 / 0.019000 | Sọ và sườn hạ, rìu nghiêng từ dọc; sàn/ánh sáng thường trở lại, thanh HP/word tray giữ ổn định ngoài world. | → F2407: Rìu đổ xuống, cán xoay qua đống xương và về sát sàn; fragment rã xương thuộc Die, không gọi mọi mảnh rời là lỗi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2406 |
| F2407 | 99.587 / 0.024000 | Rìu đổ xuống, cán xoay qua đống xương và về sát sàn; fragment rã xương thuộc Die, không gọi mọi mảnh rời là lỗi. | → F2408: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2407 |
| F2408 | 99.611 / 0.036000 | Rìu đổ xuống, cán xoay qua đống xương và về sát sàn; fragment rã xương thuộc Die, không gọi mọi mảnh rời là lỗi. | → F2409: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2408 |
| F2409 | 99.647 / 0.034000 | Rìu đổ xuống, cán xoay qua đống xương và về sát sàn; fragment rã xương thuộc Die, không gọi mọi mảnh rời là lỗi. | → F2410: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2409 |
| F2410 | 99.681 / 0.031000 | Rìu đổ xuống, cán xoay qua đống xương và về sát sàn; fragment rã xương thuộc Die, không gọi mọi mảnh rời là lỗi. | → F2411: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2410 |
| F2411 | 99.712 / 0.028000 | Rìu đổ xuống, cán xoay qua đống xương và về sát sàn; fragment rã xương thuộc Die, không gọi mọi mảnh rời là lỗi. | → F2412: Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2411 |
| F2412 | 99.740 / 0.041000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2413: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2412 |
| F2413 | 99.781 / 0.041000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2414: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2413 |
| F2414 | 99.822 / 0.016000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2415: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2414 |
| F2415 | 99.838 / 0.036000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2416: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2415 |
| F2416 | 99.874 / 0.048000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2417: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2416 |
| F2417 | 99.922 / 0.017000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2418: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2417 |
| F2418 | 99.939 / 0.045000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2419: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2418 |
| F2419 | 99.984 / 0.024000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2420: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2419 |
| F2420 | 100.008 / 0.039000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2421: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2420 |
| F2421 | 100.047 / 0.038000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2422: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2421 |
| F2422 | 100.085 / 0.034000 | Đống xương/rìu giữ thấp, player idle; shadow nằm cùng đống xương, không còn vạch ngà A1. | → F2423: Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2422 |
| F2423 | 100.119 / 0.028000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2424: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2423 |
| F2424 | 100.147 / 0.033000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2425: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2424 |
| F2425 | 100.180 / 0.028000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2426: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2425 |
| F2426 | 100.208 / 0.039000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2427: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2426 |
| F2427 | 100.247 / 0.033000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2428: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2427 |
| F2428 | 100.280 / 0.023000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2429: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2428 |
| F2429 | 100.303 / 0.038000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2430: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2429 |
| F2430 | 100.341 / 0.040000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2431: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2430 |
| F2431 | 100.381 / 0.040000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2432: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2431 |
| F2432 | 100.421 / 0.027000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2433: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2432 |
| F2433 | 100.448 / 0.028000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2434: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2433 |
| F2434 | 100.476 / 0.027000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2435: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2434 |
| F2435 | 100.503 / 0.051000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2436: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2435 |
| F2436 | 100.554 / 0.020000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2437: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2436 |
| F2437 | 100.574 / 0.053000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | → F2438: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2437 |
| F2438 | 100.627 / 0.029000 | Overlay Guard phủ arena sau cleanup; các frame giữ overlay được giữ đủ timing, không chứng nhận body bị che. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2438 |

#### Runtime `shield-attacks-hurt` — 174 frame / 173 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F2450 | 101.022 / 0.013000 | Khói spawn che actor; chưa đánh giá chi tiết thân/chân phía sau khói. | → F2451: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2450 |
| F2451 | 101.035 / 0.040000 | Khói spawn che actor; chưa đánh giá chi tiết thân/chân phía sau khói. | → F2452: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2451 |
| F2452 | 101.075 / 0.025000 | Khói spawn che actor; chưa đánh giá chi tiết thân/chân phía sau khói. | → F2453: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2452 |
| F2453 | 101.100 / 0.046000 | Khói spawn che actor; chưa đánh giá chi tiết thân/chân phía sau khói. | → F2454: Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2453 |
| F2454 | 101.146 / 0.029000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2455: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2454 |
| F2455 | 101.175 / 0.044000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2456: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2455 |
| F2456 | 101.219 / 0.049000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2457: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2456 |
| F2457 | 101.268 / 0.033000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2458: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2457 |
| F2458 | 101.301 / 0.036000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2459: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2458 |
| F2459 | 101.337 / 0.060000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2460: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2459 |
| F2460 | 101.397 / 0.025000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2461: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2460 |
| F2461 | 101.422 / 0.030000 | Khiên đỏ và kiếm hiện dần qua khói xám; chân xuất hiện trên sàn khi khói tan. | → F2462: Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2461 |
| F2462 | 101.452 / 0.044000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2463: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2462 |
| F2463 | 101.496 / 0.017000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2464: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2463 |
| F2464 | 101.513 / 0.029000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2465: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2464 |
| F2465 | 101.542 / 0.052000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2466: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2465 |
| F2466 | 101.594 / 0.024000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2467: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2466 |
| F2467 | 101.618 / 0.028000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2468: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2467 |
| F2468 | 101.646 / 0.032000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2469: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2468 |
| F2469 | 101.678 / 0.062000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2470: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2469 |
| F2470 | 101.740 / 0.043000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2471: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2470 |
| F2471 | 101.783 / 0.076000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2472: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2471 |
| F2472 | 101.859 / 0.010000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2473: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2472 |
| F2473 | 101.869 / 0.060000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2474: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2473 |
| F2474 | 101.929 / 0.014000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2475: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2474 |
| F2475 | 101.943 / 0.060000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2476: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2475 |
| F2476 | 102.003 / 0.048000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2477: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2476 |
| F2477 | 102.051 / 0.027000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2478: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2477 |
| F2478 | 102.078 / 0.053000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2479: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2478 |
| F2479 | 102.131 / 0.023000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2480: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2479 |
| F2480 | 102.154 / 0.038000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2481: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2480 |
| F2481 | 102.192 / 0.103000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2482: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2481 |
| F2482 | 102.295 / 0.045000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2483: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2482 |
| F2483 | 102.340 / 0.029000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2484: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2483 |
| F2484 | 102.369 / 0.047000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2485: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2484 |
| F2485 | 102.416 / 0.045000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2486: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2485 |
| F2486 | 102.461 / 0.028000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2487: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2486 |
| F2487 | 102.489 / 0.043000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2488: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2487 |
| F2488 | 102.532 / 0.030000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2489: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2488 |
| F2489 | 102.562 / 0.009000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2490: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2489 |
| F2490 | 102.571 / 0.056000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2491: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2490 |
| F2491 | 102.627 / 0.034000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2492: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2491 |
| F2492 | 102.661 / 0.024000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2493: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2492 |
| F2493 | 102.685 / 0.053000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2494: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2493 |
| F2494 | 102.738 / 0.058000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2495: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2494 |
| F2495 | 102.796 / 0.022000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2496: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2495 |
| F2496 | 102.818 / 0.038000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2497: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2496 |
| F2497 | 102.856 / 0.032000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2498: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2497 |
| F2498 | 102.888 / 0.055000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2499: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2498 |
| F2499 | 102.943 / 0.015000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2500: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2499 |
| F2500 | 102.958 / 0.055000 | Khiên trước thân, kiếm chéo, chân giữ landmark; khăn player dao động và cue chuẩn bị xuất hiện. | → F2501: Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2500 |
| F2501 | 103.013 / 0.055000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2502: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2501 |
| F2502 | 103.068 / 0.041000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2503: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2502 |
| F2503 | 103.109 / 0.074000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2504: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2503 |
| F2504 | 103.183 / 0.040000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2505: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2504 |
| F2505 | 103.223 / 0.036000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2506: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2505 |
| F2506 | 103.259 / 0.044000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2507: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2506 |
| F2507 | 103.303 / 0.066000 | Skeleton kéo kiếm sau vai; khiên thu theo xoay thân, grip còn nối cẳng tay. | → F2508: Player dựng kiếm parry rồi flash vàng; thân skeleton tối, chi tiết contact bị che. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2507 |
| F2508 | 103.369 / 0.039000 | Player dựng kiếm parry rồi flash vàng; thân skeleton tối, chi tiết contact bị che. | → F2509: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2508 |
| F2509 | 103.408 / 0.044000 | Player dựng kiếm parry rồi flash vàng; thân skeleton tối, chi tiết contact bị che. | → F2510: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2509 |
| F2510 | 103.452 / 0.058000 | Player dựng kiếm parry rồi flash vàng; thân skeleton tối, chi tiết contact bị che. | → F2511: Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2510 |
| F2511 | 103.510 / 0.062000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2512: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2511 |
| F2512 | 103.572 / 0.041000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2513: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2512 |
| F2513 | 103.613 / 0.014000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2514: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2513 |
| F2514 | 103.627 / 0.038000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2515: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2514 |
| F2515 | 103.665 / 0.044000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2516: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2515 |
| F2516 | 103.709 / 0.036000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2517: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2516 |
| F2517 | 103.745 / 0.062000 | Phản kích thấp và spark; kiếm đối thủ duỗi trái, khiên hẹp theo hướng thân; không thấy grip đứt. | → F2518: Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2517 |
| F2518 | 103.807 / 0.042000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2519: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2518 |
| F2519 | 103.849 / 0.043000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2520: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2519 |
| F2520 | 103.892 / 0.033000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2521: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2520 |
| F2521 | 103.925 / 0.041000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2522: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2521 |
| F2522 | 103.966 / 0.030000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2523: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2522 |
| F2523 | 103.996 / 0.028000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2524: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2523 |
| F2524 | 104.024 / 0.026000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2525: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2524 |
| F2525 | 104.050 / 0.052000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2526: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2525 |
| F2526 | 104.102 / 0.059000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2527: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2526 |
| F2527 | 104.161 / 0.025000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2528: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2527 |
| F2528 | 104.186 / 0.047000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2529: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2528 |
| F2529 | 104.233 / 0.041000 | Đối thủ thu kiếm, khiên trở mặt trước; player giữ cúi thấp; spark tan trên sàn. | → F2530: Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2529 |
| F2530 | 104.274 / 0.026000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2531: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2530 |
| F2531 | 104.300 / 0.038000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2532: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2531 |
| F2532 | 104.338 / 0.029000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2533: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2532 |
| F2533 | 104.367 / 0.027000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2534: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2533 |
| F2534 | 104.394 / 0.033000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2535: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2534 |
| F2535 | 104.427 / 0.030000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2536: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2535 |
| F2536 | 104.457 / 0.043000 | Player nâng kiếm rồi đứng, skeleton idle; chân/bóng còn cùng mặt sàn. | → F2537: Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2536 |
| F2537 | 104.500 / 0.046000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2538: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2537 |
| F2538 | 104.546 / 0.043000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2539: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2538 |
| F2539 | 104.589 / 0.024000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2540: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2539 |
| F2540 | 104.613 / 0.027000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2541: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2540 |
| F2541 | 104.640 / 0.046000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2542: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2541 |
| F2542 | 104.686 / 0.054000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2543: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2542 |
| F2543 | 104.740 / 0.044000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2544: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2543 |
| F2544 | 104.784 / 0.053000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2545: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2544 |
| F2545 | 104.837 / 0.037000 | Chém thường +10, cung che ngực; Hurt trắng cả thân và khiên rồi màu đỏ trở lại. | → F2546: Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2545 |
| F2546 | 104.874 / 0.057000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2547: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2546 |
| F2547 | 104.931 / 0.018000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2548: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2547 |
| F2548 | 104.949 / 0.070000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2549: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2548 |
| F2549 | 105.019 / 0.048000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2550: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2549 |
| F2550 | 105.067 / 0.056000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2551: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2550 |
| F2551 | 105.123 / 0.066000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2552: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2551 |
| F2552 | 105.189 / 0.069000 | Player thu kiếm thấp và hồi đứng; đối thủ giữ khiên trước. | → F2553: Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2552 |
| F2553 | 105.258 / 0.023000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2554: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2553 |
| F2554 | 105.281 / 0.047000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2555: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2554 |
| F2555 | 105.328 / 0.046000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2556: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2555 |
| F2556 | 105.374 / 0.045000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2557: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2556 |
| F2557 | 105.419 / 0.035000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2558: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2557 |
| F2558 | 105.454 / 0.052000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2559: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2558 |
| F2559 | 105.506 / 0.011000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2560: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2559 |
| F2560 | 105.517 / 0.039000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2561: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2560 |
| F2561 | 105.556 / 0.036000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2562: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2561 |
| F2562 | 105.592 / 0.032000 | Cung chém +10, Hurt trắng rồi phục hồi; chân skeleton còn đọc được dưới cung. | → F2563: Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2562 |
| F2563 | 105.624 / 0.034000 | Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | → F2564: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2563 |
| F2564 | 105.658 / 0.025000 | Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | → F2565: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2564 |
| F2565 | 105.683 / 0.058000 | Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | → F2566: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2565 |
| F2566 | 105.741 / 0.019000 | Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | → F2567: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2566 |
| F2567 | 105.760 / 0.024000 | Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | → F2568: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2567 |
| F2568 | 105.784 / 0.056000 | Thu kiếm/hồi đứng, hai actor chưa đổi bên; highlight khiên trở lại. | → F2569: Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2568 |
| F2569 | 105.840 / 0.048000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2570: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2569 |
| F2570 | 105.888 / 0.054000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2571: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2570 |
| F2571 | 105.942 / 0.062000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2572: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2571 |
| F2572 | 106.004 / 0.054000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2573: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2572 |
| F2573 | 106.058 / 0.028000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2574: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2573 |
| F2574 | 106.086 / 0.048000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2575: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2574 |
| F2575 | 106.134 / 0.032000 | Chém +20 và Hurt trắng trước khi đối thủ chuyển sang chuẩn bị đâm. | → F2576: Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2575 |
| F2576 | 106.166 / 0.025000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2577: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2576 |
| F2577 | 106.191 / 0.033000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2578: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2577 |
| F2578 | 106.224 / 0.056000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2579: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2578 |
| F2579 | 106.280 / 0.049000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2580: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2579 |
| F2580 | 106.329 / 0.014000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2581: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2580 |
| F2581 | 106.343 / 0.041000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2582: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2581 |
| F2582 | 106.384 / 0.043000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2583: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2582 |
| F2583 | 106.427 / 0.020000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2584: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2583 |
| F2584 | 106.447 / 0.050000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2585: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2584 |
| F2585 | 106.497 / 0.029000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2586: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2585 |
| F2586 | 106.526 / 0.012000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2587: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2586 |
| F2587 | 106.538 / 0.045000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2588: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2587 |
| F2588 | 106.583 / 0.051000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2589: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2588 |
| F2589 | 106.634 / 0.026000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2590: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2589 |
| F2590 | 106.660 / 0.031000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2591: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2590 |
| F2591 | 106.691 / 0.035000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2592: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2591 |
| F2592 | 106.726 / 0.041000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2593: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2592 |
| F2593 | 106.767 / 0.040000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2594: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2593 |
| F2594 | 106.807 / 0.017000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2595: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2594 |
| F2595 | 106.824 / 0.050000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2596: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2595 |
| F2596 | 106.874 / 0.016000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2597: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2596 |
| F2597 | 106.890 / 0.039000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2598: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2597 |
| F2598 | 106.929 / 0.042000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2599: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2598 |
| F2599 | 106.971 / 0.036000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2600: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2599 |
| F2600 | 107.007 / 0.096000 | Attack2 giữ kiếm ngang, khiên nghiêng hẹp; player hồi phục/idle; giữ nguyên pha chờ dài. | → F2601: Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2600 |
| F2601 | 107.103 / 0.038000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2602: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2601 |
| F2602 | 107.141 / 0.069000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2603: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2602 |
| F2603 | 107.210 / 0.062000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2604: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2603 |
| F2604 | 107.272 / 0.073000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2605: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2604 |
| F2605 | 107.345 / 0.065000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2606: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2605 |
| F2606 | 107.410 / 0.057000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2607: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2606 |
| F2607 | 107.467 / 0.030000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2608: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2607 |
| F2608 | 107.497 / 0.037000 | Parry flash rồi kiếm đâm duỗi qua đầu player; spark che mũi kiếm lúc contact, grip đọc được sau flash. | → F2609: Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2608 |
| F2609 | 107.534 / 0.044000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2610: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2609 |
| F2610 | 107.578 / 0.077000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2611: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2610 |
| F2611 | 107.655 / 0.048000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2612: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2611 |
| F2612 | 107.703 / 0.053000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2613: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2612 |
| F2613 | 107.756 / 0.038000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2614: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2613 |
| F2614 | 107.794 / 0.043000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2615: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2614 |
| F2615 | 107.837 / 0.046000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2616: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2615 |
| F2616 | 107.883 / 0.037000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2617: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2616 |
| F2617 | 107.920 / 0.054000 | Kiếm thu, khiên trở mặt trước; player giữ cúi, spark đỏ tàn. | → F2618: Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2617 |
| F2618 | 107.974 / 0.018000 | Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | → F2619: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2618 |
| F2619 | 107.992 / 0.050000 | Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | → F2620: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2619 |
| F2620 | 108.042 / 0.043000 | Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | → F2621: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2620 |
| F2621 | 108.085 / 0.086000 | Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | → F2622: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2621 |
| F2622 | 108.171 / 0.061000 | Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | → F2623: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2622 |
| F2623 | 108.232 / 0.075000 | Player nâng kiếm rồi đứng, skeleton idle; không thấy lớp khiên nhảy qua thân ngoài xoay pose. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2623 |

#### Runtime `shield-death` — 69 frame / 68 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F3244 | 131.405 / 0.040000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3245: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3244 |
| F3245 | 131.445 / 0.039000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3246: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3245 |
| F3246 | 131.484 / 0.032000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3247: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3246 |
| F3247 | 131.516 / 0.041000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3248: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3247 |
| F3248 | 131.557 / 0.030000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3249: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3248 |
| F3249 | 131.587 / 0.044000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3250: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3249 |
| F3250 | 131.631 / 0.024000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3251: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3250 |
| F3251 | 131.655 / 0.032000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3252: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3251 |
| F3252 | 131.687 / 0.023000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3253: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3252 |
| F3253 | 131.710 / 0.026000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3254: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3253 |
| F3254 | 131.736 / 0.049000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3255: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3254 |
| F3255 | 131.785 / 0.021000 | Slayer silhouette nền đỏ; cung và flash trắng che thân khiên; chân hai actor còn tách được. | → F3256: Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3255 |
| F3256 | 131.806 / 0.040000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3257: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3256 |
| F3257 | 131.846 / 0.026000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3258: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3257 |
| F3258 | 131.872 / 0.053000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3259: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3258 |
| F3259 | 131.925 / 0.020000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3260: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3259 |
| F3260 | 131.945 / 0.038000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3261: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3260 |
| F3261 | 131.983 / 0.023000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3262: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3261 |
| F3262 | 132.006 / 0.042000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3263: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3262 |
| F3263 | 132.048 / 0.023000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3264: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3263 |
| F3264 | 132.071 / 0.040000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3265: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3264 |
| F3265 | 132.111 / 0.040000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3266: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3265 |
| F3266 | 132.151 / 0.028000 | Cung kế tiếp, khiên xoay hẹp trong Hurt trắng; không dùng màu trắng này để kết luận normal lỗi. | → F3267: Đòn lethal +60, kiếm ngang và afterimage qua ngực; camera bắt đầu lùi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3266 |
| F3267 | 132.179 / 0.026000 | Đòn lethal +60, kiếm ngang và afterimage qua ngực; camera bắt đầu lùi. | → F3268: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3267 |
| F3268 | 132.205 / 0.039000 | Đòn lethal +60, kiếm ngang và afterimage qua ngực; camera bắt đầu lùi. | → F3269: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3268 |
| F3269 | 132.244 / 0.028000 | Đòn lethal +60, kiếm ngang và afterimage qua ngực; camera bắt đầu lùi. | → F3270: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3269 |
| F3270 | 132.272 / 0.054000 | Đòn lethal +60, kiếm ngang và afterimage qua ngực; camera bắt đầu lùi. | → F3271: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3270 |
| F3271 | 132.326 / 0.017000 | Đòn lethal +60, kiếm ngang và afterimage qua ngực; camera bắt đầu lùi. | → F3272: Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3271 |
| F3272 | 132.343 / 0.046000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3273: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3272 |
| F3273 | 132.389 / 0.019000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3274: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3273 |
| F3274 | 132.408 / 0.038000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3275: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3274 |
| F3275 | 132.446 / 0.027000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3276: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3275 |
| F3276 | 132.473 / 0.052000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3277: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3276 |
| F3277 | 132.525 / 0.016000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3278: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3277 |
| F3278 | 132.541 / 0.036000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3279: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3278 |
| F3279 | 132.577 / 0.028000 | Nền đỏ rút, xương/giáp hiện lại; đầu và xương tách dần, chân rã đúng pha chết. | → F3280: Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3279 |
| F3280 | 132.605 / 0.044000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3281: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3280 |
| F3281 | 132.649 / 0.025000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3282: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3281 |
| F3282 | 132.674 / 0.040000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3283: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3282 |
| F3283 | 132.714 / 0.029000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3284: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3283 |
| F3284 | 132.743 / 0.035000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3285: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3284 |
| F3285 | 132.778 / 0.027000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3286: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3285 |
| F3286 | 132.805 / 0.038000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3287: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3286 |
| F3287 | 132.843 / 0.049000 | Thân rơi, khiên rơi phải và kiếm hạ trái; đầu hạ theo, bóng nằm dưới đống xương. | → F3288: Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3287 |
| F3288 | 132.892 / 0.020000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3289: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3288 |
| F3289 | 132.912 / 0.034000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3290: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3289 |
| F3290 | 132.946 / 0.038000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3291: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3290 |
| F3291 | 132.984 / 0.023000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3292: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3291 |
| F3292 | 133.007 / 0.041000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3293: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3292 |
| F3293 | 133.048 / 0.024000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3294: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3293 |
| F3294 | 133.072 / 0.040000 | Đầu đáp sàn, xương và khiên nằm thấp; không thấy mảnh rìu lạ trong bộ khiên. | → F3295: Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3294 |
| F3295 | 133.112 / 0.028000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3296: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3295 |
| F3296 | 133.140 / 0.044000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3297: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3296 |
| F3297 | 133.184 / 0.025000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3298: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3297 |
| F3298 | 133.209 / 0.035000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3299: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3298 |
| F3299 | 133.244 / 0.029000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3300: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3299 |
| F3300 | 133.273 / 0.037000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3301: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3300 |
| F3301 | 133.310 / 0.034000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3302: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3301 |
| F3302 | 133.344 / 0.037000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3303: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3302 |
| F3303 | 133.381 / 0.025000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3304: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3303 |
| F3304 | 133.406 / 0.041000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3305: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3304 |
| F3305 | 133.447 / 0.033000 | Đống xương giữ trên sàn, player idle; landmark không trượt khi camera trở về. | → F3306: Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3305 |
| F3306 | 133.480 / 0.034000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | → F3307: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3306 |
| F3307 | 133.514 / 0.027000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | → F3308: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3307 |
| F3308 | 133.541 / 0.046000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | → F3309: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3308 |
| F3309 | 133.587 / 0.019000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | → F3310: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3309 |
| F3310 | 133.606 / 0.040000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | → F3311: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3310 |
| F3311 | 133.646 / 0.030000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | → F3312: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3311 |
| F3312 | 133.676 / 0.052000 | Overlay Elite/Swordsman che actor sau cleanup; chưa đánh giá chi tiết bị card che. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3312 |

#### Runtime `player-hurt-idle` — 105 frame / 104 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F0 | 0.000 / 0.048000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F1: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F0 |
| F1 | 0.048 / 0.018000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F2: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F1 |
| F2 | 0.066 / 0.039000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F3: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F2 |
| F3 | 0.105 / 0.028000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F4: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F3 |
| F4 | 0.133 / 0.050000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F5: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F4 |
| F5 | 0.183 / 0.022000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F6: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F5 |
| F6 | 0.205 / 0.033000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F7: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F6 |
| F7 | 0.238 / 0.027000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F8: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F7 |
| F8 | 0.265 / 0.038000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F9: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F8 |
| F9 | 0.303 / 0.033000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F10: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F9 |
| F10 | 0.336 / 0.038000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F11: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F10 |
| F11 | 0.374 / 0.030000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F12: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F11 |
| F12 | 0.404 / 0.034000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F13: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F12 |
| F13 | 0.438 / 0.027000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F14: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F13 |
| F14 | 0.465 / 0.040000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F15: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F14 |
| F15 | 0.505 / 0.038000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F16: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F15 |
| F16 | 0.543 / 0.033000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F17: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F16 |
| F17 | 0.576 / 0.023000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F18: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F17 |
| F18 | 0.599 / 0.050000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F19: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F18 |
| F19 | 0.649 / 0.032000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F20: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F19 |
| F20 | 0.681 / 0.031000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F21: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F20 |
| F21 | 0.712 / 0.023000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F22: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F21 |
| F22 | 0.735 / 0.049000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F23: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F22 |
| F23 | 0.784 / 0.026000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F24: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F23 |
| F24 | 0.810 / 0.022000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F25: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F24 |
| F25 | 0.832 / 0.057000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F26: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F25 |
| F26 | 0.889 / 0.025000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F27: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F26 |
| F27 | 0.914 / 0.037000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F28: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F27 |
| F28 | 0.951 / 0.028000 | Hai actor idle: khăn/đầu chuyển nhịp, chân giữ sàn và kiếm chéo giữ grip; highlight xương ấm, bóng dưới chân. | → F29: Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F28 |
| F29 | 0.979 / 0.046000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F30: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F29 |
| F30 | 1.025 / 0.024000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F31: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F30 |
| F31 | 1.049 / 0.031000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F32: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F31 |
| F32 | 1.080 / 0.028000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F33: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F32 |
| F33 | 1.108 / 0.049000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F34: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F33 |
| F34 | 1.157 / 0.019000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F35: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F34 |
| F35 | 1.176 / 0.031000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F36: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F35 |
| F36 | 1.207 / 0.037000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F37: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F36 |
| F37 | 1.244 / 0.061000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F38: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F37 |
| F38 | 1.305 / 0.040000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F39: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F38 |
| F39 | 1.345 / 0.054000 | Skeleton kéo kiếm lên vai rồi giữ windup; player idle chưa né; chân không dịch theo cue. | → F40: Kiếm đối thủ quét ngang qua đầu player, cung trắng che contact. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F39 |
| F40 | 1.399 / 0.024000 | Kiếm đối thủ quét ngang qua đầu player, cung trắng che contact. | → F41: Player flash đỏ ngả sau trong Hurt; sàn/camera dịch nhẹ theo impact, không coi flash là đổi palette asset. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F40 |
| F41 | 1.423 / 0.033000 | Player flash đỏ ngả sau trong Hurt; sàn/camera dịch nhẹ theo impact, không coi flash là đổi palette asset. | → F42: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F41 |
| F42 | 1.456 / 0.027000 | Player flash đỏ ngả sau trong Hurt; sàn/camera dịch nhẹ theo impact, không coi flash là đổi palette asset. | → F43: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F42 |
| F43 | 1.483 / 0.052000 | Player flash đỏ ngả sau trong Hurt; sàn/camera dịch nhẹ theo impact, không coi flash là đổi palette asset. | → F44: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F43 |
| F44 | 1.535 / 0.045000 | Player flash đỏ ngả sau trong Hurt; sàn/camera dịch nhẹ theo impact, không coi flash là đổi palette asset. | → F45: Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F44 |
| F45 | 1.580 / 0.046000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F46: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F45 |
| F46 | 1.626 / 0.029000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F47: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F46 |
| F47 | 1.655 / 0.014000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F48: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F47 |
| F48 | 1.669 / 0.046000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F49: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F48 |
| F49 | 1.715 / 0.033000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F50: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F49 |
| F50 | 1.748 / 0.028000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F51: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F50 |
| F51 | 1.776 / 0.044000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F52: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F51 |
| F52 | 1.820 / 0.022000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F53: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F52 |
| F53 | 1.842 / 0.062000 | Hết đỏ, player ngả rồi co thấp và hồi phục; skeleton thu kiếm; chân về landmark cùng sàn. | → F54: Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F53 |
| F54 | 1.904 / 0.052000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F55: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F54 |
| F55 | 1.956 / 0.037000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F56: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F55 |
| F56 | 1.993 / 0.035000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F57: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F56 |
| F57 | 2.028 / 0.051000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F58: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F57 |
| F58 | 2.079 / 0.037000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F59: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F58 |
| F59 | 2.116 / 0.023000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F60: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F59 |
| F60 | 2.139 / 0.035000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F61: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F60 |
| F61 | 2.174 / 0.032000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F62: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F61 |
| F62 | 2.206 / 0.034000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F63: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F62 |
| F63 | 2.240 / 0.042000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F64: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F63 |
| F64 | 2.282 / 0.023000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F65: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F64 |
| F65 | 2.305 / 0.026000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F66: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F65 |
| F66 | 2.331 / 0.054000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F67: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F66 |
| F67 | 2.385 / 0.016000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F68: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F67 |
| F68 | 2.401 / 0.042000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F69: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F68 |
| F69 | 2.443 / 0.024000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F70: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F69 |
| F70 | 2.467 / 0.041000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F71: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F70 |
| F71 | 2.508 / 0.027000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F72: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F71 |
| F72 | 2.535 / 0.036000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F73: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F72 |
| F73 | 2.571 / 0.027000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F74: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F73 |
| F74 | 2.598 / 0.045000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F75: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F74 |
| F75 | 2.643 / 0.024000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F76: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F75 |
| F76 | 2.667 / 0.044000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F77: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F76 |
| F77 | 2.711 / 0.027000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F78: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F77 |
| F78 | 2.738 / 0.035000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F79: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F78 |
| F79 | 2.773 / 0.038000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F80: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F79 |
| F80 | 2.811 / 0.046000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F81: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F80 |
| F81 | 2.857 / 0.010000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F82: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F81 |
| F82 | 2.867 / 0.043000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F83: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F82 |
| F83 | 2.910 / 0.046000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F84: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F83 |
| F84 | 2.956 / 0.036000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F85: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F84 |
| F85 | 2.992 / 0.035000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F86: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F85 |
| F86 | 3.027 / 0.033000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F87: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F86 |
| F87 | 3.060 / 0.041000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F88: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F87 |
| F88 | 3.101 / 0.031000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F89: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F88 |
| F89 | 3.132 / 0.040000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F90: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F89 |
| F90 | 3.172 / 0.052000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F91: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F90 |
| F91 | 3.224 / 0.022000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F92: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F91 |
| F92 | 3.246 / 0.024000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F93: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F92 |
| F93 | 3.270 / 0.047000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F94: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F93 |
| F94 | 3.317 / 0.027000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F95: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F94 |
| F95 | 3.344 / 0.040000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F96: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F95 |
| F96 | 3.384 / 0.026000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F97: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F96 |
| F97 | 3.410 / 0.036000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F98: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F97 |
| F98 | 3.446 / 0.041000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F99: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F98 |
| F99 | 3.487 / 0.025000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F100: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F99 |
| F100 | 3.512 / 0.051000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F101: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F100 |
| F101 | 3.563 / 0.041000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F102: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F101 |
| F102 | 3.604 / 0.032000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F103: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F102 |
| F103 | 3.636 / 0.037000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | → F104: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F103 |
| F104 | 3.673 / 0.060000 | Player trở lại idle đủ vòng khăn/đầu; skeleton idle đủ vòng, sáng tối trên xương không đảo đột ngột trong phần nhìn thấy. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F104 |

#### Runtime `player-death` — 114 frame / 113 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F666 | 24.127 / 0.048000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F667: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F666 |
| F667 | 24.175 / 0.043000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F668: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F667 |
| F668 | 24.218 / 0.015000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F669: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F668 |
| F669 | 24.233 / 0.040000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F670: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F669 |
| F670 | 24.273 / 0.028000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F671: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F670 |
| F671 | 24.301 / 0.040000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F672: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F671 |
| F672 | 24.341 / 0.024000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F673: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F672 |
| F673 | 24.365 / 0.046000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F674: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F673 |
| F674 | 24.411 / 0.022000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F675: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F674 |
| F675 | 24.433 / 0.039000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F676: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F675 |
| F676 | 24.472 / 0.029000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F677: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F676 |
| F677 | 24.501 / 0.042000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F678: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F677 |
| F678 | 24.543 / 0.023000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F679: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F678 |
| F679 | 24.566 / 0.041000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F680: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F679 |
| F680 | 24.607 / 0.030000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F681: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F680 |
| F681 | 24.637 / 0.056000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F682: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F681 |
| F682 | 24.693 / 0.025000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F683: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F682 |
| F683 | 24.718 / 0.036000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F684: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F683 |
| F684 | 24.754 / 0.041000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F685: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F684 |
| F685 | 24.795 / 0.036000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F686: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F685 |
| F686 | 24.831 / 0.043000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F687: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F686 |
| F687 | 24.874 / 0.041000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F688: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F687 |
| F688 | 24.915 / 0.035000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F689: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F688 |
| F689 | 24.950 / 0.018000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F690: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F689 |
| F690 | 24.968 / 0.038000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F691: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F690 |
| F691 | 25.006 / 0.033000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F692: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F691 |
| F692 | 25.039 / 0.036000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F693: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F692 |
| F693 | 25.075 / 0.025000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F694: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F693 |
| F694 | 25.100 / 0.026000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F695: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F694 |
| F695 | 25.126 / 0.053000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F696: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F695 |
| F696 | 25.179 / 0.030000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F697: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F696 |
| F697 | 25.209 / 0.031000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F698: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F697 |
| F698 | 25.240 / 0.067000 | Trước lethal: player idle, khăn dao động; skeleton giữ kiếm chéo, hai chân trên sàn. | → F699: Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F698 |
| F699 | 25.307 / 0.059000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F700: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F699 |
| F700 | 25.366 / 0.044000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F701: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F700 |
| F701 | 25.410 / 0.030000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F702: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F701 |
| F702 | 25.440 / 0.021000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F703: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F702 |
| F703 | 25.461 / 0.041000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F704: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F703 |
| F704 | 25.502 / 0.049000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F705: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F704 |
| F705 | 25.551 / 0.021000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F706: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F705 |
| F706 | 25.572 / 0.049000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F707: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F706 |
| F707 | 25.621 / 0.026000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F708: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F707 |
| F708 | 25.647 / 0.044000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F709: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F708 |
| F709 | 25.691 / 0.018000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F710: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F709 |
| F710 | 25.709 / 0.045000 | Đối thủ đưa kiếm lên vai rồi vung ngang; player bắt đầu co thấp lúc trúng. | → F711: Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F710 |
| F711 | 25.754 / 0.036000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F712: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F711 |
| F712 | 25.790 / 0.026000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F713: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F712 |
| F713 | 25.816 / 0.048000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F714: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F713 |
| F714 | 25.864 / 0.012000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F715: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F714 |
| F715 | 25.876 / 0.042000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F716: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F715 |
| F716 | 25.918 / 0.024000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F717: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F716 |
| F717 | 25.942 / 0.036000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F718: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F717 |
| F718 | 25.978 / 0.029000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F719: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F718 |
| F719 | 26.007 / 0.032000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F720: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F719 |
| F720 | 26.039 / 0.019000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F721: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F720 |
| F721 | 26.058 / 0.042000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F722: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F721 |
| F722 | 26.100 / 0.040000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F723: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F722 |
| F723 | 26.140 / 0.026000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F724: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F723 |
| F724 | 26.166 / 0.045000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F725: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F724 |
| F725 | 26.211 / 0.025000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F726: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F725 |
| F726 | 26.236 / 0.028000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F727: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F726 |
| F727 | 26.264 / 0.058000 | Player gập người và giữ thấp, đầu cúi/khăn cao; skeleton thu kiếm; không ép clip chết quay về idle. | → F728: Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F727 |
| F728 | 26.322 / 0.025000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F729: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F728 |
| F729 | 26.347 / 0.046000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F730: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F729 |
| F730 | 26.393 / 0.030000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F731: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F730 |
| F731 | 26.423 / 0.014000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F732: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F731 |
| F732 | 26.437 / 0.023000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F733: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F732 |
| F733 | 26.460 / 0.039000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F734: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F733 |
| F734 | 26.499 / 0.043000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F735: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F734 |
| F735 | 26.542 / 0.024000 | Player ngửa đầu, nâng thân vào death; khăn rơi sau, chân còn trên sàn. | → F736: Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F735 |
| F736 | 26.566 / 0.041000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F737: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F736 |
| F737 | 26.607 / 0.028000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F738: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F737 |
| F738 | 26.635 / 0.025000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F739: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F738 |
| F739 | 26.660 / 0.040000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F740: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F739 |
| F740 | 26.700 / 0.039000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F741: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F740 |
| F741 | 26.739 / 0.036000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F742: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F741 |
| F742 | 26.775 / 0.033000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F743: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F742 |
| F743 | 26.808 / 0.029000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F744: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F743 |
| F744 | 26.837 / 0.025000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F745: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F744 |
| F745 | 26.862 / 0.037000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F746: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F745 |
| F746 | 26.899 / 0.028000 | Thân sáng dần thành silhouette ngà, chi tiết mất theo hiệu ứng chết; không chứng nhận normal ở pha trắng. | → F747: Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F746 |
| F747 | 26.927 / 0.039000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F748: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F747 |
| F748 | 26.966 / 0.045000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F749: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F748 |
| F749 | 27.011 / 0.028000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F750: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F749 |
| F750 | 27.039 / 0.020000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F751: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F750 |
| F751 | 27.059 / 0.044000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F752: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F751 |
| F752 | 27.103 / 0.035000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F753: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F752 |
| F753 | 27.138 / 0.027000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F754: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F753 |
| F754 | 27.165 / 0.042000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F755: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F754 |
| F755 | 27.207 / 0.026000 | Silhouette kéo cao và tách mảnh; xem lại full frame F750–755: đầu hiệu ứng không bị canvas cắt. | → F756: Mảnh sáng thưa dần rồi biến mất; full frame F756–759 giữ đủ phần trên hiệu ứng. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F755 |
| F756 | 27.233 / 0.026000 | Mảnh sáng thưa dần rồi biến mất; full frame F756–759 giữ đủ phần trên hiệu ứng. | → F757: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F756 |
| F757 | 27.259 / 0.042000 | Mảnh sáng thưa dần rồi biến mất; full frame F756–759 giữ đủ phần trên hiệu ứng. | → F758: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F757 |
| F758 | 27.301 / 0.039000 | Mảnh sáng thưa dần rồi biến mất; full frame F756–759 giữ đủ phần trên hiệu ứng. | → F759: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F758 |
| F759 | 27.340 / 0.034000 | Mảnh sáng thưa dần rồi biến mất; full frame F756–759 giữ đủ phần trên hiệu ứng. | → F760: Result fade che sân, chữ/số xuất hiện dần; không suy ra lỗi glyph từ fade giữa chừng. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F759 |
| F760 | 27.374 / 0.021000 | Result fade che sân, chữ/số xuất hiện dần; không suy ra lỗi glyph từ fade giữa chừng. | → F761: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F760 |
| F761 | 27.395 / 0.039000 | Result fade che sân, chữ/số xuất hiện dần; không suy ra lỗi glyph từ fade giữa chừng. | → F762: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F761 |
| F762 | 27.434 / 0.039000 | Result fade che sân, chữ/số xuất hiện dần; không suy ra lỗi glyph từ fade giữa chừng. | → F763: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F762 |
| F763 | 27.473 / 0.026000 | Result fade che sân, chữ/số xuất hiện dần; không suy ra lỗi glyph từ fade giữa chừng. | → F764: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F763 |
| F764 | 27.499 / 0.041000 | Result fade che sân, chữ/số xuất hiện dần; không suy ra lỗi glyph từ fade giữa chừng. | → F765: Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F764 |
| F765 | 27.540 / 0.029000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F766: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F765 |
| F766 | 27.569 / 0.030000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F767: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F766 |
| F767 | 27.599 / 0.033000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F768: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F767 |
| F768 | 27.632 / 0.048000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F769: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F768 |
| F769 | 27.680 / 0.020000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F770: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F769 |
| F770 | 27.700 / 0.040000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F771: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F770 |
| F771 | 27.740 / 0.032000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F772: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F771 |
| F772 | 27.772 / 0.037000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F773: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F772 |
| F773 | 27.809 / 0.025000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F774: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F773 |
| F774 | 27.834 / 0.026000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F775: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F774 |
| F775 | 27.860 / 0.042000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F776: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F775 |
| F776 | 27.902 / 0.039000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F777: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F776 |
| F777 | 27.941 / 0.041000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F778: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F777 |
| F778 | 27.982 / 0.013000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | → F779: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F778 |
| F779 | 27.995 / 0.038000 | Result giữ nền đen và nhãn score/best/restart; actor đã khuất, không đánh giá thân phía sau. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `hurt-death` F779 |

#### Runtime `shield-idle` — 115 frame / 114 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F2959 | 121.109 / 0.053000 | Player thu kiếm sau đòn, khiên đỏ trước thân đối thủ; chân giữ sàn. | → F2960: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2959 |
| F2960 | 121.162 / 0.024000 | Player thu kiếm sau đòn, khiên đỏ trước thân đối thủ; chân giữ sàn. | → F2961: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2960 |
| F2961 | 121.186 / 0.038000 | Player thu kiếm sau đòn, khiên đỏ trước thân đối thủ; chân giữ sàn. | → F2962: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2961 |
| F2962 | 121.224 / 0.021000 | Player thu kiếm sau đòn, khiên đỏ trước thân đối thủ; chân giữ sàn. | → F2963: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2962 |
| F2963 | 121.245 / 0.025000 | Player thu kiếm sau đòn, khiên đỏ trước thân đối thủ; chân giữ sàn. | → F2964: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2963 |
| F2964 | 121.270 / 0.056000 | Player thu kiếm sau đòn, khiên đỏ trước thân đối thủ; chân giữ sàn. | → F2965: Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2964 |
| F2965 | 121.326 / 0.013000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2966: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2965 |
| F2966 | 121.339 / 0.039000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2967: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2966 |
| F2967 | 121.378 / 0.045000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2968: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2967 |
| F2968 | 121.423 / 0.022000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2969: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2968 |
| F2969 | 121.445 / 0.024000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2970: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2969 |
| F2970 | 121.469 / 0.038000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2971: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2970 |
| F2971 | 121.507 / 0.040000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2972: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2971 |
| F2972 | 121.547 / 0.043000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2973: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2972 |
| F2973 | 121.590 / 0.015000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2974: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2973 |
| F2974 | 121.605 / 0.049000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2975: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2974 |
| F2975 | 121.654 / 0.028000 | Cung chém thường +10 rồi thu; cung che một phần khiên, đầu/giáp không tách sai. | → F2976: Player đứng lại, khiên trước thân và kiếm chéo; score mờ dần. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2975 |
| F2976 | 121.682 / 0.033000 | Player đứng lại, khiên trước thân và kiếm chéo; score mờ dần. | → F2977: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2976 |
| F2977 | 121.715 / 0.029000 | Player đứng lại, khiên trước thân và kiếm chéo; score mờ dần. | → F2978: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2977 |
| F2978 | 121.744 / 0.069000 | Player đứng lại, khiên trước thân và kiếm chéo; score mờ dần. | → F2979: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2978 |
| F2979 | 121.813 / 0.032000 | Player đứng lại, khiên trước thân và kiếm chéo; score mờ dần. | → F2980: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2979 |
| F2980 | 121.845 / 0.048000 | Player đứng lại, khiên trước thân và kiếm chéo; score mờ dần. | → F2981: Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2980 |
| F2981 | 121.893 / 0.017000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2982: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2981 |
| F2982 | 121.910 / 0.038000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2983: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2982 |
| F2983 | 121.948 / 0.041000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2984: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2983 |
| F2984 | 121.989 / 0.035000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2985: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2984 |
| F2985 | 122.024 / 0.032000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2986: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2985 |
| F2986 | 122.056 / 0.030000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2987: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2986 |
| F2987 | 122.086 / 0.032000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2988: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2987 |
| F2988 | 122.118 / 0.034000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2989: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2988 |
| F2989 | 122.152 / 0.029000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2990: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2989 |
| F2990 | 122.181 / 0.032000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2991: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2990 |
| F2991 | 122.213 / 0.031000 | Đòn +20 và spark vàng, Hurt trắng; sau flash skeleton trở lại xương/giáp đỏ. | → F2992: Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2991 |
| F2992 | 122.244 / 0.050000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2993: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2992 |
| F2993 | 122.294 / 0.023000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2994: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2993 |
| F2994 | 122.317 / 0.039000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2995: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2994 |
| F2995 | 122.356 / 0.028000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2996: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2995 |
| F2996 | 122.384 / 0.031000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2997: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2996 |
| F2997 | 122.415 / 0.027000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2998: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2997 |
| F2998 | 122.442 / 0.038000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F2999: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2998 |
| F2999 | 122.480 / 0.043000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3000: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F2999 |
| F3000 | 122.523 / 0.012000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3001: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3000 |
| F3001 | 122.535 / 0.042000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3002: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3001 |
| F3002 | 122.577 / 0.047000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3003: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3002 |
| F3003 | 122.624 / 0.023000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3004: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3003 |
| F3004 | 122.647 / 0.022000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3005: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3004 |
| F3005 | 122.669 / 0.038000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3006: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3005 |
| F3006 | 122.707 / 0.049000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3007: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3006 |
| F3007 | 122.756 / 0.023000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3008: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3007 |
| F3008 | 122.779 / 0.024000 | Player hồi đứng, skeleton idle rồi kéo kiếm chuẩn bị; khăn và mũ giữ liên kết đầu/thân. | → F3009: Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3008 |
| F3009 | 122.803 / 0.040000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3010: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3009 |
| F3010 | 122.843 / 0.040000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3011: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3010 |
| F3011 | 122.883 / 0.027000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3012: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3011 |
| F3012 | 122.910 / 0.027000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3013: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3012 |
| F3013 | 122.937 / 0.044000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3014: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3013 |
| F3014 | 122.981 / 0.038000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3015: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3014 |
| F3015 | 123.019 / 0.036000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3016: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3015 |
| F3016 | 123.055 / 0.033000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3017: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3016 |
| F3017 | 123.088 / 0.032000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3018: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3017 |
| F3018 | 123.120 / 0.045000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3019: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3018 |
| F3019 | 123.165 / 0.022000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3020: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3019 |
| F3020 | 123.187 / 0.045000 | Windup Attack1, parry dựng kiếm rồi đâm ngang; khiên xoay hẹp có chủ ý, spark che contact. | → F3021: Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3020 |
| F3021 | 123.232 / 0.054000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3022: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3021 |
| F3022 | 123.286 / 0.025000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3023: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3022 |
| F3023 | 123.311 / 0.040000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3024: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3023 |
| F3024 | 123.351 / 0.035000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3025: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3024 |
| F3025 | 123.386 / 0.039000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3026: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3025 |
| F3026 | 123.425 / 0.019000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3027: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3026 |
| F3027 | 123.444 / 0.032000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3028: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3027 |
| F3028 | 123.476 / 0.034000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3029: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3028 |
| F3029 | 123.510 / 0.024000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3030: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3029 |
| F3030 | 123.534 / 0.040000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3031: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3030 |
| F3031 | 123.574 / 0.042000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3032: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3031 |
| F3032 | 123.616 / 0.025000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3033: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3032 |
| F3033 | 123.641 / 0.045000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3034: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3033 |
| F3034 | 123.686 / 0.033000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3035: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3034 |
| F3035 | 123.719 / 0.018000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3036: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3035 |
| F3036 | 123.737 / 0.037000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3037: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3036 |
| F3037 | 123.774 / 0.041000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3038: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3037 |
| F3038 | 123.815 / 0.034000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3039: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3038 |
| F3039 | 123.849 / 0.021000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3040: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3039 |
| F3040 | 123.870 / 0.040000 | Đối thủ thu kiếm về idle, spark tan; player giữ cúi thấp, chân/bóng không đổi bên. | → F3041: Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3040 |
| F3041 | 123.910 / 0.028000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3042: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3041 |
| F3042 | 123.938 / 0.039000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3043: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3042 |
| F3043 | 123.977 / 0.031000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3044: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3043 |
| F3044 | 124.008 / 0.036000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3045: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3044 |
| F3045 | 124.044 / 0.036000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3046: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3045 |
| F3046 | 124.080 / 0.033000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3047: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3046 |
| F3047 | 124.113 / 0.039000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3048: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3047 |
| F3048 | 124.152 / 0.027000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3049: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3048 |
| F3049 | 124.179 / 0.046000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3050: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3049 |
| F3050 | 124.225 / 0.034000 | Idle khiên đủ nhịp đầu/vai, chân giữ sàn; player nâng kiếm hồi đứng. | → F3051: Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3050 |
| F3051 | 124.259 / 0.010000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3052: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3051 |
| F3052 | 124.269 / 0.049000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3053: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3052 |
| F3053 | 124.318 / 0.032000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3054: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3053 |
| F3054 | 124.350 / 0.029000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3055: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3054 |
| F3055 | 124.379 / 0.024000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3056: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3055 |
| F3056 | 124.403 / 0.052000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3057: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3056 |
| F3057 | 124.455 / 0.028000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3058: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3057 |
| F3058 | 124.483 / 0.026000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3059: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3058 |
| F3059 | 124.509 / 0.028000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3060: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3059 |
| F3060 | 124.537 / 0.039000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3061: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3060 |
| F3061 | 124.576 / 0.030000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3062: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3061 |
| F3062 | 124.606 / 0.037000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3063: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3062 |
| F3063 | 124.643 / 0.033000 | Chém +10 rồi thu cung; khiên vẫn trước thân và đầu không đổi tỉ lệ rõ. | → F3064: Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3063 |
| F3064 | 124.676 / 0.049000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3065: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3064 |
| F3065 | 124.725 / 0.010000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3066: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3065 |
| F3066 | 124.735 / 0.039000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3067: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3066 |
| F3067 | 124.774 / 0.044000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3068: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3067 |
| F3068 | 124.818 / 0.025000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3069: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3068 |
| F3069 | 124.843 / 0.051000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3070: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3069 |
| F3070 | 124.894 / 0.025000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3071: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3070 |
| F3071 | 124.919 / 0.019000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3072: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3071 |
| F3072 | 124.938 / 0.036000 | Hai actor trở idle; khăn player chuyển nhịp, khiên/kiếm giữ grip. | → F3073: Đòn kế bắt đầu với spark; đây là ranh đoạn, không tính là toàn bộ đòn kế. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3072 |
| F3073 | 124.974 / 0.045000 | Đòn kế bắt đầu với spark; đây là ranh đoạn, không tính là toàn bộ đòn kế. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `combat` F3073 |

#### Runtime `slayer-complete` — 309 frame / 308 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F105 | 4.312 / 0.034000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F106: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F105 |
| F106 | 4.346 / 0.029000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F107: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F106 |
| F107 | 4.375 / 0.026000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F108: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F107 |
| F108 | 4.401 / 0.042000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F109: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F108 |
| F109 | 4.443 / 0.026000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F110: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F109 |
| F110 | 4.469 / 0.037000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F111: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F110 |
| F111 | 4.506 / 0.043000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F112: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F111 |
| F112 | 4.549 / 0.027000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F113: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F112 |
| F113 | 4.576 / 0.027000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F114: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F113 |
| F114 | 4.603 / 0.048000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F115: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F114 |
| F115 | 4.651 / 0.038000 | Trước entry: player thu kiếm thấp rồi đứng, skeleton idle; chân tách nhau trên sàn. | → F116: Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F115 |
| F116 | 4.689 / 0.028000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F117: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F116 |
| F117 | 4.717 / 0.029000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F118: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F117 |
| F118 | 4.746 / 0.049000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F119: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F118 |
| F119 | 4.795 / 0.022000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F120: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F119 |
| F120 | 4.817 / 0.027000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F121: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F120 |
| F121 | 4.844 / 0.037000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F122: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F121 |
| F122 | 4.881 / 0.041000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F123: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F122 |
| F123 | 4.922 / 0.024000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F124: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F123 |
| F124 | 4.946 / 0.039000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F125: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F124 |
| F125 | 4.985 / 0.015000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F126: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F125 |
| F126 | 5.000 / 0.052000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F127: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F126 |
| F127 | 5.052 / 0.016000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F128: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F127 |
| F128 | 5.068 / 0.048000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F129: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F128 |
| F129 | 5.116 / 0.057000 | Cue chuyển sáng, skeleton kéo kiếm sau vai, player idle; giữ windup nguyên timing. | → F130: Player dựng kiếm parry; mũi đối thủ sát đầu; grip còn nối tay. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F129 |
| F130 | 5.173 / 0.014000 | Player dựng kiếm parry; mũi đối thủ sát đầu; grip còn nối tay. | → F131: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F130 |
| F131 | 5.187 / 0.027000 | Player dựng kiếm parry; mũi đối thủ sát đầu; grip còn nối tay. | → F132: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F131 |
| F132 | 5.214 / 0.037000 | Player dựng kiếm parry; mũi đối thủ sát đầu; grip còn nối tay. | → F133: Parry +48 rồi +51/flash ngà; điểm chạm sáng vàng che chi tiết hai lưỡi. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F132 |
| F133 | 5.251 / 0.047000 | Parry +48 rồi +51/flash ngà; điểm chạm sáng vàng che chi tiết hai lưỡi. | → F134: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F133 |
| F134 | 5.298 / 0.010000 | Parry +48 rồi +51/flash ngà; điểm chạm sáng vàng che chi tiết hai lưỡi. | → F135: Cung vòng thấp, player nhấc kiếm sau vai; zoom vào và nền đỏ nổi dần, full frame xác nhận đầu không bị crop game. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F134 |
| F135 | 5.308 / 0.038000 | Cung vòng thấp, player nhấc kiếm sau vai; zoom vào và nền đỏ nổi dần, full frame xác nhận đầu không bị crop game. | → F136: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F135 |
| F136 | 5.346 / 0.042000 | Cung vòng thấp, player nhấc kiếm sau vai; zoom vào và nền đỏ nổi dần, full frame xác nhận đầu không bị crop game. | → F137: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F136 |
| F137 | 5.388 / 0.048000 | Cung vòng thấp, player nhấc kiếm sau vai; zoom vào và nền đỏ nổi dần, full frame xác nhận đầu không bị crop game. | → F138: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F137 |
| F138 | 5.436 / 0.019000 | Cung vòng thấp, player nhấc kiếm sau vai; zoom vào và nền đỏ nổi dần, full frame xác nhận đầu không bị crop game. | → F139: Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F138 |
| F139 | 5.455 / 0.022000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F140: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F139 |
| F140 | 5.477 / 0.032000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F141: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F140 |
| F141 | 5.509 / 0.037000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F142: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F141 |
| F142 | 5.546 / 0.029000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F143: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F142 |
| F143 | 5.575 / 0.030000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F144: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F143 |
| F144 | 5.605 / 0.039000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F145: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F144 |
| F145 | 5.644 / 0.085000 | Chi tiết thân tối dần thành silhouette đỏ/đen; spark thu; chân còn tách được và tray chữ không zoom cùng world. | → F146: Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F145 |
| F146 | 5.729 / 0.020000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F147: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F146 |
| F147 | 5.749 / 0.040000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F148: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F147 |
| F148 | 5.789 / 0.027000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F149: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F148 |
| F149 | 5.816 / 0.037000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F150: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F149 |
| F150 | 5.853 / 0.039000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F151: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F150 |
| F151 | 5.892 / 0.046000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F152: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F151 |
| F152 | 5.938 / 0.048000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F153: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F152 |
| F153 | 5.986 / 0.046000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F154: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F153 |
| F154 | 6.032 / 0.031000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F155: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F154 |
| F155 | 6.063 / 0.034000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F156: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F155 |
| F156 | 6.097 / 0.039000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F157: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F156 |
| F157 | 6.136 / 0.063000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F158: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F157 |
| F158 | 6.199 / 0.008000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F159: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F158 |
| F159 | 6.207 / 0.049000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F160: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F159 |
| F160 | 6.256 / 0.022000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F161: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F160 |
| F161 | 6.278 / 0.067000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F162: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F161 |
| F162 | 6.345 / 0.038000 | Silhouette giữ idle với khăn, kiếm và chân rõ đường bao; normal/depth nội thân bị cinematic che. | → F163: Slayer quét vòng thấp, cung ngà chia đôi quanh thân đối thủ; kiếm rút về vai, chân trụ không đổi bên. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F162 |
| F163 | 6.383 / 0.020000 | Slayer quét vòng thấp, cung ngà chia đôi quanh thân đối thủ; kiếm rút về vai, chân trụ không đổi bên. | → F164: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F163 |
| F164 | 6.403 / 0.045000 | Slayer quét vòng thấp, cung ngà chia đôi quanh thân đối thủ; kiếm rút về vai, chân trụ không đổi bên. | → F165: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F164 |
| F165 | 6.448 / 0.028000 | Slayer quét vòng thấp, cung ngà chia đôi quanh thân đối thủ; kiếm rút về vai, chân trụ không đổi bên. | → F166: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F165 |
| F166 | 6.476 / 0.034000 | Slayer quét vòng thấp, cung ngà chia đôi quanh thân đối thủ; kiếm rút về vai, chân trụ không đổi bên. | → F167: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F166 |
| F167 | 6.510 / 0.047000 | Slayer quét vòng thấp, cung ngà chia đôi quanh thân đối thủ; kiếm rút về vai, chân trụ không đổi bên. | → F168: Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F167 |
| F168 | 6.557 / 0.030000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F169: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F168 |
| F169 | 6.587 / 0.053000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F170: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F169 |
| F170 | 6.640 / 0.015000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F171: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F170 |
| F171 | 6.655 / 0.022000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F172: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F171 |
| F172 | 6.677 / 0.061000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F173: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F172 |
| F173 | 6.738 / 0.010000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F174: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F173 |
| F174 | 6.748 / 0.038000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F175: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F174 |
| F175 | 6.786 / 0.045000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F176: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F175 |
| F176 | 6.831 / 0.038000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F177: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F176 |
| F177 | 6.869 / 0.038000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F178: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F177 |
| F178 | 6.907 / 0.030000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F179: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F178 |
| F179 | 6.937 / 0.040000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F180: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F179 |
| F180 | 6.977 / 0.023000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F181: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F180 |
| F181 | 7.000 / 0.068000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F182: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F181 |
| F182 | 7.068 / 0.047000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F183: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F182 |
| F183 | 7.115 / 0.072000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F184: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F183 |
| F184 | 7.187 / 0.031000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F185: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F184 |
| F185 | 7.218 / 0.039000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F186: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F185 |
| F186 | 7.257 / 0.023000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F187: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F186 |
| F187 | 7.280 / 0.042000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F188: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F187 |
| F188 | 7.322 / 0.027000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F189: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F188 |
| F189 | 7.349 / 0.027000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F190: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F189 |
| F190 | 7.376 / 0.028000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F191: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F190 |
| F191 | 7.404 / 0.046000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F192: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F191 |
| F192 | 7.450 / 0.019000 | Recovery về idle silhouette; score mờ, giữ toàn bộ frame chờ trước đòn tiếp. | → F193: Cung vòng thấp kế tiếp rồi thu về vai; đường bao tay/kiếm liên tục trong phần không bị cung che. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F192 |
| F193 | 7.469 / 0.039000 | Cung vòng thấp kế tiếp rồi thu về vai; đường bao tay/kiếm liên tục trong phần không bị cung che. | → F194: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F193 |
| F194 | 7.508 / 0.051000 | Cung vòng thấp kế tiếp rồi thu về vai; đường bao tay/kiếm liên tục trong phần không bị cung che. | → F195: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F194 |
| F195 | 7.559 / 0.026000 | Cung vòng thấp kế tiếp rồi thu về vai; đường bao tay/kiếm liên tục trong phần không bị cung che. | → F196: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F195 |
| F196 | 7.585 / 0.031000 | Cung vòng thấp kế tiếp rồi thu về vai; đường bao tay/kiếm liên tục trong phần không bị cung che. | → F197: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F196 |
| F197 | 7.616 / 0.027000 | Cung vòng thấp kế tiếp rồi thu về vai; đường bao tay/kiếm liên tục trong phần không bị cung che. | → F198: Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F197 |
| F198 | 7.643 / 0.042000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F199: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F198 |
| F199 | 7.685 / 0.024000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F200: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F199 |
| F200 | 7.709 / 0.039000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F201: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F200 |
| F201 | 7.748 / 0.036000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F202: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F201 |
| F202 | 7.784 / 0.026000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F203: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F202 |
| F203 | 7.810 / 0.030000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F204: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F203 |
| F204 | 7.840 / 0.039000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F205: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F204 |
| F205 | 7.879 / 0.051000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F206: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F205 |
| F206 | 7.930 / 0.035000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F207: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F206 |
| F207 | 7.965 / 0.022000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F208: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F207 |
| F208 | 7.987 / 0.039000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F209: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F208 |
| F209 | 8.026 / 0.031000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F210: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F209 |
| F210 | 8.057 / 0.012000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F211: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F210 |
| F211 | 8.069 / 0.065000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F212: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F211 |
| F212 | 8.134 / 0.014000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F213: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F212 |
| F213 | 8.148 / 0.027000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F214: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F213 |
| F214 | 8.175 / 0.039000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F215: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F214 |
| F215 | 8.214 / 0.030000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F216: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F215 |
| F216 | 8.244 / 0.048000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F217: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F216 |
| F217 | 8.292 / 0.030000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F218: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F217 |
| F218 | 8.322 / 0.031000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F219: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F218 |
| F219 | 8.353 / 0.032000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F220: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F219 |
| F220 | 8.385 / 0.031000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F221: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F220 |
| F221 | 8.416 / 0.038000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F222: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F221 |
| F222 | 8.454 / 0.023000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F223: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F222 |
| F223 | 8.477 / 0.059000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F224: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F223 |
| F224 | 8.536 / 0.030000 | Idle silhouette giữ pose; tray đổi từ how sang while, world không kéo tray theo camera. | → F225: Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F224 |
| F225 | 8.566 / 0.053000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F226: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F225 |
| F226 | 8.619 / 0.040000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F227: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F226 |
| F227 | 8.659 / 0.024000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F228: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F227 |
| F228 | 8.683 / 0.026000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F229: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F228 |
| F229 | 8.709 / 0.039000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F230: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F229 |
| F230 | 8.748 / 0.028000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F231: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F230 |
| F231 | 8.776 / 0.052000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F232: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F231 |
| F232 | 8.828 / 0.021000 | Slayer chém dựng cung trước đối thủ rồi cúi rất thấp; kiếm duỗi ngang sàn và hồi đứng. | → F233: Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F232 |
| F233 | 8.849 / 0.050000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F234: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F233 |
| F234 | 8.899 / 0.023000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F235: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F234 |
| F235 | 8.922 / 0.037000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F236: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F235 |
| F236 | 8.959 / 0.043000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F237: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F236 |
| F237 | 9.002 / 0.074000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F238: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F237 |
| F238 | 9.076 / 0.055000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F239: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F238 |
| F239 | 9.131 / 0.044000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F240: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F239 |
| F240 | 9.175 / 0.011000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F241: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F240 |
| F241 | 9.186 / 0.045000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F242: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F241 |
| F242 | 9.231 / 0.050000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F243: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F242 |
| F243 | 9.281 / 0.015000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F244: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F243 |
| F244 | 9.296 / 0.022000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F245: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F244 |
| F245 | 9.318 / 0.060000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F246: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F245 |
| F246 | 9.378 / 0.028000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F247: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F246 |
| F247 | 9.406 / 0.087000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F248: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F247 |
| F248 | 9.493 / 0.026000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F249: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F248 |
| F249 | 9.519 / 0.059000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F250: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F249 |
| F250 | 9.578 / 0.046000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F251: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F250 |
| F251 | 9.624 / 0.038000 | Recovery/idle, khăn chuyển sau; giữ pha chờ, chân đứng cùng mặt sàn. | → F252: Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F251 |
| F252 | 9.662 / 0.022000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F253: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F252 |
| F253 | 9.684 / 0.024000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F254: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F253 |
| F254 | 9.708 / 0.034000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F255: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F254 |
| F255 | 9.742 / 0.041000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F256: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F255 |
| F256 | 9.783 / 0.021000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F257: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F256 |
| F257 | 9.804 / 0.063000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F258: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F257 |
| F258 | 9.867 / 0.024000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F259: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F258 |
| F259 | 9.891 / 0.034000 | Lặp cung dựng và cúi thấp, đối thủ flash trắng rồi tối; không chứng nhận ngực bị cung che. | → F260: Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F259 |
| F260 | 9.925 / 0.035000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F261: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F260 |
| F261 | 9.960 / 0.031000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F262: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F261 |
| F262 | 9.991 / 0.024000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F263: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F262 |
| F263 | 10.015 / 0.052000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F264: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F263 |
| F264 | 10.067 / 0.047000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F265: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F264 |
| F265 | 10.114 / 0.042000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F266: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F265 |
| F266 | 10.156 / 0.020000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F267: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F266 |
| F267 | 10.176 / 0.027000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F268: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F267 |
| F268 | 10.203 / 0.043000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F269: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F268 |
| F269 | 10.246 / 0.027000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F270: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F269 |
| F270 | 10.273 / 0.040000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F271: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F270 |
| F271 | 10.313 / 0.022000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F272: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F271 |
| F272 | 10.335 / 0.059000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F273: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F272 |
| F273 | 10.394 / 0.009000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F274: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F273 |
| F274 | 10.403 / 0.092000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F275: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F274 |
| F275 | 10.495 / 0.035000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F276: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F275 |
| F276 | 10.530 / 0.045000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F277: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F276 |
| F277 | 10.575 / 0.040000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F278: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F277 |
| F278 | 10.615 / 0.016000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F279: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F278 |
| F279 | 10.631 / 0.034000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F280: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F279 |
| F280 | 10.665 / 0.028000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F281: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F280 |
| F281 | 10.693 / 0.051000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F282: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F281 |
| F282 | 10.744 / 0.019000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F283: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F282 |
| F283 | 10.763 / 0.050000 | Idle silhouette; score tàn, phần nền đỏ giữ ổn định. | → F284: Slayer nâng kiếm cao rồi rút chéo qua ngực đối thủ; flash trắng, grip nối ở phía player đọc được. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F283 |
| F284 | 10.813 / 0.063000 | Slayer nâng kiếm cao rồi rút chéo qua ngực đối thủ; flash trắng, grip nối ở phía player đọc được. | → F285: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F284 |
| F285 | 10.876 / 0.064000 | Slayer nâng kiếm cao rồi rút chéo qua ngực đối thủ; flash trắng, grip nối ở phía player đọc được. | → F286: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F285 |
| F286 | 10.940 / 0.048000 | Slayer nâng kiếm cao rồi rút chéo qua ngực đối thủ; flash trắng, grip nối ở phía player đọc được. | → F287: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F286 |
| F287 | 10.988 / 0.067000 | Slayer nâng kiếm cao rồi rút chéo qua ngực đối thủ; flash trắng, grip nối ở phía player đọc được. | → F288: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F287 |
| F288 | 11.055 / 0.017000 | Slayer nâng kiếm cao rồi rút chéo qua ngực đối thủ; flash trắng, grip nối ở phía player đọc được. | → F289: Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F288 |
| F289 | 11.072 / 0.073000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F290: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F289 |
| F290 | 11.145 / 0.029000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F291: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F290 |
| F291 | 11.174 / 0.064000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F292: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F291 |
| F292 | 11.238 / 0.028000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F293: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F292 |
| F293 | 11.266 / 0.041000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F294: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F293 |
| F294 | 11.307 / 0.033000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F295: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F294 |
| F295 | 11.340 / 0.083000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F296: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F295 |
| F296 | 11.423 / 0.050000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F297: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F296 |
| F297 | 11.473 / 0.053000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F298: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F297 |
| F298 | 11.526 / 0.053000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F299: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F298 |
| F299 | 11.579 / 0.015000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F300: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F299 |
| F300 | 11.594 / 0.027000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F301: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F300 |
| F301 | 11.621 / 0.050000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F302: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F301 |
| F302 | 11.671 / 0.037000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F303: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F302 |
| F303 | 11.708 / 0.032000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F304: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F303 |
| F304 | 11.740 / 0.034000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F305: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F304 |
| F305 | 11.774 / 0.028000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F306: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F305 |
| F306 | 11.802 / 0.038000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F307: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F306 |
| F307 | 11.840 / 0.030000 | Hồi về silhouette idle, scarf quét trái; nền không bật normal ngẫu nhiên. | → F308: Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F307 |
| F308 | 11.870 / 0.054000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F309: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F308 |
| F309 | 11.924 / 0.023000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F310: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F309 |
| F310 | 11.947 / 0.069000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F311: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F310 |
| F311 | 12.016 / 0.021000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F312: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F311 |
| F312 | 12.037 / 0.038000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F313: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F312 |
| F313 | 12.075 / 0.078000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F314: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F313 |
| F314 | 12.153 / 0.012000 | Cung dựng rồi crouch thấp, kiếm sát sàn và thu; flash kết thúc sau cung. | → F315: Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F314 |
| F315 | 12.165 / 0.031000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F316: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F315 |
| F316 | 12.196 / 0.043000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F317: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F316 |
| F317 | 12.239 / 0.059000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F318: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F317 |
| F318 | 12.298 / 0.044000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F319: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F318 |
| F319 | 12.342 / 0.033000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F320: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F319 |
| F320 | 12.375 / 0.040000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F321: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F320 |
| F321 | 12.415 / 0.046000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F322: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F321 |
| F322 | 12.461 / 0.026000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F323: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F322 |
| F323 | 12.487 / 0.062000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F324: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F323 |
| F324 | 12.549 / 0.044000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F325: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F324 |
| F325 | 12.593 / 0.024000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F326: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F325 |
| F326 | 12.617 / 0.110000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F327: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F326 |
| F327 | 12.727 / 0.035000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F328: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F327 |
| F328 | 12.762 / 0.050000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F329: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F328 |
| F329 | 12.812 / 0.018000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F330: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F329 |
| F330 | 12.830 / 0.077000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F331: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F330 |
| F331 | 12.907 / 0.013000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F332: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F331 |
| F332 | 12.920 / 0.072000 | Idle giữ chân; thời gian chờ trước đòn kế không bị lược bỏ. | → F333: Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F332 |
| F333 | 12.992 / 0.059000 | Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | → F334: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F333 |
| F334 | 13.051 / 0.031000 | Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | → F335: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F334 |
| F335 | 13.082 / 0.017000 | Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | → F336: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F335 |
| F336 | 13.099 / 0.026000 | Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | → F337: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F336 |
| F337 | 13.125 / 0.034000 | Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | → F338: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F337 |
| F338 | 13.159 / 0.042000 | Cung vòng thấp rồi kiếm lên vai, đối thủ Hurt trắng; afterimage ngang ngực thu dần. | → F339: Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F338 |
| F339 | 13.201 / 0.026000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F340: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F339 |
| F340 | 13.227 / 0.044000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F341: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F340 |
| F341 | 13.271 / 0.046000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F342: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F341 |
| F342 | 13.317 / 0.055000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F343: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F342 |
| F343 | 13.372 / 0.061000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F344: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F343 |
| F344 | 13.433 / 0.034000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F345: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F344 |
| F345 | 13.467 / 0.022000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F346: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F345 |
| F346 | 13.489 / 0.039000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F347: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F346 |
| F347 | 13.528 / 0.040000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F348: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F347 |
| F348 | 13.568 / 0.013000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F349: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F348 |
| F349 | 13.581 / 0.048000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F350: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F349 |
| F350 | 13.629 / 0.014000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F351: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F350 |
| F351 | 13.643 / 0.042000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F352: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F351 |
| F352 | 13.685 / 0.026000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F353: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F352 |
| F353 | 13.711 / 0.027000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F354: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F353 |
| F354 | 13.738 / 0.040000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F355: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F354 |
| F355 | 13.778 / 0.030000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F356: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F355 |
| F356 | 13.808 / 0.034000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F357: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F356 |
| F357 | 13.842 / 0.104000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F358: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F357 |
| F358 | 13.946 / 0.019000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F359: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F358 |
| F359 | 13.965 / 0.014000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F360: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F359 |
| F360 | 13.979 / 0.067000 | Idle silhouette, score mờ; tray chuyển dark/make/them độc lập world. | → F361: Cung vòng thấp tiếp, kiếm thu vai; flash chuyển ngà rồi tối. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F360 |
| F361 | 14.046 / 0.057000 | Cung vòng thấp tiếp, kiếm thu vai; flash chuyển ngà rồi tối. | → F362: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F361 |
| F362 | 14.103 / 0.074000 | Cung vòng thấp tiếp, kiếm thu vai; flash chuyển ngà rồi tối. | → F363: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F362 |
| F363 | 14.177 / 0.036000 | Cung vòng thấp tiếp, kiếm thu vai; flash chuyển ngà rồi tối. | → F364: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F363 |
| F364 | 14.213 / 0.055000 | Cung vòng thấp tiếp, kiếm thu vai; flash chuyển ngà rồi tối. | → F365: Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F364 |
| F365 | 14.268 / 0.052000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F366: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F365 |
| F366 | 14.320 / 0.030000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F367: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F366 |
| F367 | 14.350 / 0.105000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F368: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F367 |
| F368 | 14.455 / 0.012000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F369: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F368 |
| F369 | 14.467 / 0.054000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F370: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F369 |
| F370 | 14.521 / 0.061000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F371: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F370 |
| F371 | 14.582 / 0.026000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F372: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F371 |
| F372 | 14.608 / 0.079000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F373: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F372 |
| F373 | 14.687 / 0.020000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F374: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F373 |
| F374 | 14.707 / 0.047000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F375: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F374 |
| F375 | 14.754 / 0.037000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F376: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F375 |
| F376 | 14.791 / 0.037000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F377: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F376 |
| F377 | 14.828 / 0.024000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F378: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F377 |
| F378 | 14.852 / 0.042000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F379: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F378 |
| F379 | 14.894 / 0.021000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F380: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F379 |
| F380 | 14.915 / 0.075000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F381: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F380 |
| F381 | 14.990 / 0.017000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F382: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F381 |
| F382 | 15.007 / 0.038000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F383: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F382 |
| F383 | 15.045 / 0.049000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F384: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F383 |
| F384 | 15.094 / 0.017000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F385: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F384 |
| F385 | 15.111 / 0.030000 | Recovery/idle cùng silhouette; không thấy đảo lớp chân so với mặt sàn. | → F386: Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F385 |
| F386 | 15.141 / 0.038000 | Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | → F387: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F386 |
| F387 | 15.179 / 0.060000 | Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | → F388: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F387 |
| F388 | 15.239 / 0.007000 | Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | → F389: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F388 |
| F389 | 15.246 / 0.033000 | Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | → F390: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F389 |
| F390 | 15.279 / 0.046000 | Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | → F391: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F390 |
| F391 | 15.325 / 0.034000 | Cung dựng cuối rồi crouch rất thấp, kiếm sát sàn; trắng đối thủ rút dần. | → F392: Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F391 |
| F392 | 15.359 / 0.031000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F393: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F392 |
| F393 | 15.390 / 0.037000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F394: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F393 |
| F394 | 15.427 / 0.031000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F395: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F394 |
| F395 | 15.458 / 0.027000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F396: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F395 |
| F396 | 15.485 / 0.042000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F397: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F396 |
| F397 | 15.527 / 0.010000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F398: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F397 |
| F398 | 15.537 / 0.044000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F399: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F398 |
| F399 | 15.581 / 0.039000 | Exit Slayer: player đứng lại, nền đỏ tối dần, camera lùi và xương hiện lại; chưa kết thúc ở đoạn cũ 15.6s. | → F400: Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F399 |
| F400 | 15.620 / 0.026000 | Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | → F401: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F400 |
| F401 | 15.646 / 0.026000 | Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | → F402: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F401 |
| F402 | 15.672 / 0.050000 | Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | → F403: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F402 |
| F403 | 15.722 / 0.050000 | Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | → F404: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F403 |
| F404 | 15.772 / 0.032000 | Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | → F405: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F404 |
| F405 | 15.804 / 0.057000 | Đuôi mở rộng full frame: camera lùi tiếp, chân theo landmark sàn, chi tiết khăn/xương trở lại dần. | → F406: Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F405 |
| F406 | 15.861 / 0.009000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F407: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F406 |
| F407 | 15.870 / 0.040000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F408: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F407 |
| F408 | 15.910 / 0.027000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F409: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F408 |
| F409 | 15.937 / 0.040000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F410: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F409 |
| F410 | 15.977 / 0.050000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F411: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F410 |
| F411 | 16.027 / 0.021000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F412: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F411 |
| F412 | 16.048 / 0.029000 | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | → F413: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F412 |
| F413 | 16.077 / chưa xác định | Về khung thường, cue SPACE và khung tray đúng vị trí; đỏ rút hết, player/enemy idle trên sàn. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `slayer-complete` F413 |

#### Runtime `axe-die-target` — 57 frame / 56 cặp đã xem

| Frame | PTS / duration (s) | Pha / nhận xét trực quan | Chuyển tiếp sang frame kế | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| F605 | 25.604 / 0.104000 | Player thu kiếm rồi đứng, rìu idle dọc; sàn và ánh sáng thường, không có mode silhouette. | → F606: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F605 |
| F606 | 25.708 / 0.059000 | Player thu kiếm rồi đứng, rìu idle dọc; sàn và ánh sáng thường, không có mode silhouette. | → F607: Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F606 |
| F607 | 25.767 / 0.038000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F608: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F607 |
| F608 | 25.805 / 0.064000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F609: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F608 |
| F609 | 25.869 / 0.031000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F610: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F609 |
| F610 | 25.900 / 0.041000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F611: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F610 |
| F611 | 25.941 / 0.036000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F612: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F611 |
| F612 | 25.977 / 0.045000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F613: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F612 |
| F613 | 26.022 / 0.100000 | Chém thường, cung/spark và Hurt trắng của rìu; phần chân còn đọc được, đầu rìu sáng theo highlight. | → F614: Cung rút về kiếm thấp, skeleton hết trắng và đứng lại; player còn thấp, cán rìu dọc. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F613 |
| F614 | 26.122 / 0.078000 | Cung rút về kiếm thấp, skeleton hết trắng và đứng lại; player còn thấp, cán rìu dọc. | → F615: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F614 |
| F615 | 26.200 / 0.036000 | Cung rút về kiếm thấp, skeleton hết trắng và đứng lại; player còn thấp, cán rìu dọc. | → F616: Đòn lethal bắt đầu Die, sọ nâng; vạch thấp gần cán trùng vùng kiếm player nên riêng các frame này không tách chắc được A1 bằng màu. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F615 |
| F616 | 26.236 / 0.051000 | Đòn lethal bắt đầu Die, sọ nâng; vạch thấp gần cán trùng vùng kiếm player nên riêng các frame này không tách chắc được A1 bằng màu. | → F617: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F616 |
| F617 | 26.287 / 0.098000 | Đòn lethal bắt đầu Die, sọ nâng; vạch thấp gần cán trùng vùng kiếm player nên riêng các frame này không tách chắc được A1 bằng màu. | → F618: Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F617 |
| F618 | 26.385 / 0.037000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F619: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F618 |
| F619 | 26.422 / 0.058000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F620: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F619 |
| F620 | 26.480 / 0.039000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F621: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F620 |
| F621 | 26.519 / 0.034000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F622: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F621 |
| F622 | 26.553 / 0.054000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F623: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F622 |
| F623 | 26.607 / 0.031000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F624: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F623 |
| F624 | 26.638 / 0.073000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F625: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F624 |
| F625 | 26.711 / 0.027000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F626: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F625 |
| F626 | 26.738 / 0.035000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F627: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F626 |
| F627 | 26.773 / 0.031000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F628: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F627 |
| F628 | 26.804 / 0.035000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F629: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F628 |
| F629 | 26.839 / 0.039000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F630: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F629 |
| F630 | 26.878 / 0.049000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F631: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F630 |
| F631 | 26.927 / 0.042000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F632: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F631 |
| F632 | 26.969 / 0.041000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F633: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F632 |
| F633 | 27.010 / 0.043000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F634: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F633 |
| F634 | 27.053 / 0.038000 | Pause overlay giữ pose death; probe báo Die_0 nhưng overlay/độ trễ render giới hạn đối chiếu cel ảnh; không chứng nhận chi tiết bị action button che. | → F635: Resume, sọ/ngực/eo rã tiếp và chi rơi; sàn không đổi vị trí độc lập với camera, player thu kiếm. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F634 |
| F635 | 27.091 / 0.037000 | Resume, sọ/ngực/eo rã tiếp và chi rơi; sàn không đổi vị trí độc lập với camera, player thu kiếm. | → F636: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F635 |
| F636 | 27.128 / 0.029000 | Resume, sọ/ngực/eo rã tiếp và chi rơi; sàn không đổi vị trí độc lập với camera, player thu kiếm. | → F637: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F636 |
| F637 | 27.157 / 0.044000 | Resume, sọ/ngực/eo rã tiếp và chi rơi; sàn không đổi vị trí độc lập với camera, player thu kiếm. | → F638: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F637 |
| F638 | 27.201 / 0.028000 | Resume, sọ/ngực/eo rã tiếp và chi rơi; sàn không đổi vị trí độc lập với camera, player thu kiếm. | → F639: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F638 |
| F639 | 27.229 / 0.059000 | Resume, sọ/ngực/eo rã tiếp và chi rơi; sàn không đổi vị trí độc lập với camera, player thu kiếm. | → F640: Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F639 |
| F640 | 27.288 / 0.080000 | Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | → F641: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F640 |
| F641 | 27.368 / 0.062000 | Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | → F642: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F641 |
| F642 | 27.430 / 0.040000 | Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | → F643: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F642 |
| F643 | 27.470 / 0.039000 | Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | → F644: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F643 |
| F644 | 27.509 / 0.105000 | Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | → F645: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F644 |
| F645 | 27.614 / 0.066000 | Rìu nghiêng rồi rơi ngang đống xương, hạt bụi nâu bay bên phải; bụi chuyển động khác mảnh A1 tĩnh sát sàn. | → F646: Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F645 |
| F646 | 27.680 / 0.029000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F647: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F646 |
| F647 | 27.709 / 0.037000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F648: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F647 |
| F648 | 27.746 / 0.043000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F649: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F648 |
| F649 | 27.789 / 0.059000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F650: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F649 |
| F650 | 27.848 / 0.040000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F651: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F650 |
| F651 | 27.888 / 0.056000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F652: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F651 |
| F652 | 27.944 / 0.026000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F653: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F652 |
| F653 | 27.970 / 0.062000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F654: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F653 |
| F654 | 28.032 / 0.043000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F655: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F654 |
| F655 | 28.075 / 0.029000 | Đống xương/rìu giữ trên sàn, hạt bụi tan; player idle; đoạn đuôi được mở rộng tới cleanup để không bỏ frame cuối. | → F656: Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F655 |
| F656 | 28.104 / 0.029000 | Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | → F657: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F656 |
| F657 | 28.133 / 0.032000 | Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | → F658: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F657 |
| F658 | 28.165 / 0.040000 | Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | → F659: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F658 |
| F659 | 28.205 / 0.029000 | Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | → F660: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F659 |
| F660 | 28.234 / 0.029000 | Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | → F661: tiếp pha trên; giữ cả frame lặp/hold. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F660 |
| F661 | 28.263 / chưa xác định | Overlay Guard xuất hiện sau cleanup và giữ; duration frame encode cuối chưa xác định, không dùng stop sau serialize để chứng nhận nó. | Kết thúc scope; không ép loop; biên sau ngoài phạm vi. | chưa kiểm chứng playback/zoom đầy đủ | `axe-die-repro` F661 |

## Chốt lượt bổ sung: phát hiện, phạm vi và giới hạn

### Lỗi xác nhận A1 — mảnh khiên lẫn vào rìu

| Mục | Bằng chứng / tác động / hướng xử lý |
|---|---|
| Vị trí nguồn | `Assets/Art/EndlessSkeletons/axe/Walk.png` F6–F11 và `axe/Die.png` F0; SHA256 và ánh xạ GIF ở phần extraction. `design/skeleton-hero/prepare_gif_pack.py` lấy lane rìu nhưng ngưỡng `right_split=140` ở source F13–F25 giữ nhầm mảnh khiên đang rơi của actor bên phải. |
| Tái hiện runtime | Bản `5d637df`, Normal, thắng Heavy bằng đòn Slayer; xem Die bắt đầu ở `combat.webm` F2392, PTS 99.008s (duration encode 78ms). Trong crop (560,410)–(800,530), vạch ngà nằm sát sàn, trái cán rìu và tách khỏi chân player/skeleton. F2393/99.086s mất vạch khi thân bắt đầu rã. Transform mirror đưa mảnh source sang bên đối diện; flash nhuộm nâu thành ngà. |
| Tác động | Pixel ngoài nhân vật hiện ngắn lúc Die, đọc như vệt/vật thừa rồi bật mất; lỗi ownership extraction, không phải cán rìu, bóng hoặc chủ ý xương rơi. Walk có cùng lỗi nguồn nhưng chưa ảnh hưởng đường gameplay đã lần. Không có bằng chứng lỗi logic combat. |
| Đối chiếu độc lập | Trực tiếp xem source GIF F13–F35 và cel đầu ra: mảnh thuộc chiếc khiên actor phải, không thuộc actor rìu. Clip target Pause đúng Heavy hỗ trợ diễn tiến Die nhưng overlay/vũ khí player che vùng mảnh; không lấy nó thay bằng chứng F2392. |
| Hướng xử lý cho task sửa riêng | Sửa mask ownership/lane khi extraction, giữ canvas/pivot/timing và phần rìu; xuất lại Walk/Die cùng maps bị ảnh hưởng, bảo toàn GUID. Sau đó review lại mọi frame/cặp của hai clip, các maps, mirror runtime Die và chỗ nối. Audit này không áp dụng sửa. |

### Ma trận bao phủ

| Yêu cầu | Đã quan sát / căn cứ | Kết luận có giới hạn |
|---|---|---|
| Controller tutorial/combat | Prefab gốc → nhánh TutorialRun; combat thường thay controller theo definition; sáu clip gốc bổ sung 41 key / 39 cặp, mỗi key có duration/nhận xét | Đã lần nguồn; tutorial reading khóa timeScale nên clip tutorial không chứng minh vòng idle hay mọi state legacy chạy. |
| Player idle/normal/parry/Hurt/death | `normal-parry`, `player-hurt-idle`, `player-death`: chuẩn bị, thực hiện, recovery/result; giữ idle đủ vòng | Đã xem từng frame encode/cặp; không thấy lỗi anchor/grip rõ trong vùng nhìn được. Chưa nghiệm thu timing playback hoặc phần VFX che. |
| Ba đòn Slayer, entry/exit | Observer ghi `player_slayer.attackS1`, `attackS2`, `attackS3`; clip target giãn phím 1.05s và toàn bộ đuôi tới khung thường. Quan sát riêng vòng thấp, cung dựng/crouch và kiếm cao/chém chéo | Đã xem toàn bộ scope 309 frame / 308 cặp; không coi mỗi phím là một clip hoàn chỉnh hoặc probe gần thời gian là ánh xạ frame-exact. Thiếu playback. |
| Kiếm, rìu, khiên | Idle, Attack1, Hurt, Die của ba bộ; Attack2 kiếm/khiên; mirror đối thủ thực dùng là localScale.x âm, không phải flipX | Đã xem các scope liệt kê. A1 lỗi ở rìu Die. Không ép thêm hướng không dùng trong flow. |
| Rìu Attack2 / Walk | Controller/definition không gọi Walk. Heavy vòng 1 chỉ Attack1; rìu Attack2 thuộc Berserker vòng 2 trong khi RepeatAfterBoss=false kết thúc vòng 1 | Chỉ đánh giá asset; runtime **không áp dụng cho flow hiện tại**, không tạo coverage bằng bridge ép state. |
| Follow/zoom/shake/chân | Scope normal/parry, nhận đòn, ba death, Slayer vào/ra; soi lại crop rộng khi zoom. Sàn và actor cùng dịch với camera, chân không trượt độc lập rõ ở phần nhìn được | Không phát hiện lỗi xác nhận ngoài A1; không đủ cơ sở chứng nhận mọi frame engine hoặc nhịp shake. Probe world luôn 320×180, FOV khoảng 20.6973–35.0000 là bằng chứng cấu hình chạy, không phải pass thẩm mỹ. |
| Sorting/flash/afterimage | Kiếm/cung trước ngực, khiên thu hẹp khi xoay, spark tại contact, flash Hurt/Slayer và afterimage thu; bộ đếm mismatch sorting tối đa 0 trong 20.525 probe world | Hình và probe không thấy lỗi sorting rõ; thân/grip sau cung/flash vẫn chưa kiểm chứng. Đường ngang afterimage và silhouette đỏ/đen có nguồn thiết kế, không mặc định gọi là mất normal. |
| Normal/depth/bóng | Highlight ấm trên xương và áo khi thường, bóng dưới chân/đống xương ở mặt sàn; review mirror có điều kiện nguồn `_DepthFacing` so với transform | Không xác nhận lỗi normal bằng quan sát hiện có. Chưa kiểm độc lập mọi map/cel/mirror, không bật/tắt VFX bằng bridge để thay flow. |
| Phản chiếu | `WetStoneReflection` inactive trong scene hiện tại | Không có runtime reflection để nghiệm thu; half-resolution/Bilinear là contract code khi bật, không báo không thấy reflection là lỗi. |
| 1366×768 / quay lại 1280×720 | Menu, input tên, tutorial, combat Pause, Victory resize rồi trả về; xem full screenshot, không chỉ crop sprite | Các màn đã xem không thấy viền/padding/chữ bị cắt hoặc bar cũ lưu sau resize. Độ rộng texel không nguyên ở 1366 là hệ quả phóng point, không tự đổi chuẩn 2.5D. Không suy rộng sang mọi màn/nhãn. |
| Glyph input hợp lệ / nhãn động | Nhập `Đặng_Á9` bằng CDP keyDown có text/key/unmodifiedText và keyUp vào Unity đang focus; input, tên HUD và fixture giữ đúng chuỗi. Đ/ặ/Á hiển thị không có ô vuông; nhãn score/combo/+damage, role, kết quả và “—” Accuracy đã nhìn | Đây là đường bàn phím thực, không sửa DOM/input hay gọi API game để chèn tên. Không chứng minh toàn Unicode/font fallback, font QA overlay hoặc các glyph chưa xuất hiện. |

Ảnh full được xem cho lượt này: `menu-1280`, `menu-1366`, `menu-return`;
`name-fallback-1280`, `name-fallback-1366`, `name-return`; `tutorial-1280`,
`tutorial-1366`, `tutorial-return`; `combat-paused-1366`,
`combat-paused-return`; `combat-end`, `combat-1366`, `combat-return`;
`death-end` (đều PNG tạm). Name-return còn dấu và caret; Victory có các giá trị
6/98.0%/49/258.7s vừa ô; death-end có `Normal · Defeated by SWORDSMAN`, 0,
25.9s, dấu “—” và best 6/4849 vừa khung. F6 QA overlay che một phần tên HUD;
không coi chữ debug chồng này là lỗi sản phẩm. Đây là kiểm các ảnh nêu tên,
không phải tuyên bố đã review toàn video combat 6.980 frame.

### Receipt kỹ thuật và delta main

- Snapshot sạch từ `5d637dfc7cd881ecb20b92b2057679cf5c555b1f`, hash LF ở đầu
  phần bổ sung. Unity native Windows đúng `2022.3.25f1`, executable
  `E:/Unity/Hub/Editor/2022.3.25f1/Editor/Unity.exe`; build invocation
  `20261027T034047.496298Z-d40f5870f4c24b13a979d2c0854196a7-build-right`, exit 0.
  Manifest player gồm 33 file, SHA256
  `6e076ba163d750ffa3278761f95f6a7231c45bc76490b830c23d02147ac115f7`
  (sort path tương đối, mỗi dòng path + NUL + SHA256 nội dung + LF).
- Driver audit riêng dùng server/fixtures của runner, chỉ kịch bản đã nêu;
  không tự chạy thêm profile Slayer, stress hoặc viewport khác. Bridge và
  observer chỉ đọc, điều khiển gameplay bằng phím/chuột. Không sửa HP,
  animator, mirror, map hay game API. WebGL Edge `156.0.4314.42`, ANGLE GTX
  1650 Ti/D3D11. Các tên container/toolchain có sẵn trong schema browser.json
  không mô tả môi trường thực; đây là lần chạy native Windows, không Docker.
- Lượt capture chính `capture-3`: 4 check kỹ thuật pass, không exception/pageerror;
  Victory 6 đối thủ, 4849 điểm, 49 parry, HP27; restart rồi không nhập để nhận
  8 đòn và chết. Đây là bằng chứng đi qua flow, không chứng minh hình đẹp.
- Firebase bị fixture bắt: endless GET 2, PUT 1, global fixture 0; **0 request
  tới provider thật**, externalBlocked 0 vì không host lạ phát sinh. Không dùng
  nhầm bộ đếm fixture global=0 thành không có request mock leaderboard.
- Hai thử input trước đó không dùng làm bằng chứng glyph: insertText không vào
  Unity; `keyboard.press('Đ')` báo Unknown key. Lượt target lưu được hai clip
  rồi chết lúc chờ idle khiên do HP thấp; status driver failed, không báo pass.
  Idle khiên lấy đoạn đủ vòng trong capture chính; mọi scope đã chọn vẫn giữ.
- Fetch cuối thấy `29c032856460007055d6926cac2d9284704cd6d5` mới hơn bản capture:
  `EndlessDuel.cs:93` đổi duy nhất `Continue · Enter` → `Ready · Enter`; test và
  tài liệu liên quan cũng cập nhật. Đã đọc diff toàn `Assets Packages
  ProjectSettings`; không thay asset/controller/camera/shader hoặc runtime
  behavior khác. Bằng chứng nguồn/animation ở trên vẫn áp dụng; **layout
  Ready mới chưa kiểm chứng runtime**. Không dùng build cũ để nhận nhãn mới đạt.

### Phần còn lại trước khi nghiệm thu hình/chuyển động

1. Quan sát trực tiếp playback 1× và chậm, giữ PTS/hold, của các scope runtime,
   sáu clip skeleton gốc và nguồn extraction đã chọn. Công cụ trả ảnh tĩnh ở
   phiên này không cung cấp bằng chứng xem playback; việc tự phát video rồi
   đợi hết cũng không giải quyết giới hạn đó.
2. Zoom nearest bổ sung cho mọi frame runtime; kiểm các chi tiết bị VFX/card
   che bằng preview có căn cứ và tách kết luận preview khỏi runtime. Chưa có
   so sánh đầy đủ tất cả cel normal/depth ở mọi biến thể VFX/hướng.
3. Render/quan sát nhãn `Ready · Enter` trên main mới tại hai kích thước và
   trở lại 1280×720. Các glyph/nhãn chưa xuất hiện không được tính là đã kiểm.
4. Khi làm task sửa A1, kiểm lại đầy đủ nguồn Walk/Die, maps và runtime Die;
   báo cáo này chỉ xác định lỗi, không sửa game.

Chỉ lưu Markdown này trong repo. Kết quả tài liệu được soát bảng đếm, đường
dẫn nguồn, diff và `git diff --check`; không chạy thêm suite game cho thay đổi
báo cáo. Snapshot, player, script audit, ảnh/clip/log tạm dưới
`E:/CodexTemp/TypingHero/pixel-audit-gaps-20261027` được dọn sau khi ghi kết quả;
định danh revision/hash/F/PTS ở đây là receipt lưu giữ, không phải link tới
artifact còn tồn tại. Không báo toàn audit đạt khi các khoảng trống trên còn mở.


## Tiếp tục đóng khoảng trống — 2026-10-27 (đang thực hiện)

### Revision, cách ly và phạm vi

Worktree Orca riêng `D:/Projects/TypingHero-worktrees/pixel-audit-evidence`, branch
`micharel09/pixel-audit-evidence`, từ `origin/main`
`e5c48017201d4d5ea780cc01b0b9039f1fe3569b`; checkout bẩn ban đầu giữ nguyên.
Đã fetch trước khi tạo. Diff sản phẩm từ `5d637df` chỉ có nhãn
`Ready · Enter` trong `EndlessDuel.cs` và test tương ứng; không đổi art,
controller, camera, map hoặc shader. Các receipt trước là lịch sử, không phải
media còn mở được. A1 tiếp tục là **lỗi**, không áp dụng sửa trong task này.

Snapshot LF sạch: 2.495 file, SHA256
`7b98f4e83a5e8b4b76d6b3e9ab3695a0e1d212eb252a157ed583c34eac9d9d73`.
Lệnh chuẩn bị: `python -B tools/game_loop/local_windows.py snapshot --workspace
E:/CodexTemp/TypingHero/pixel-audit-evidence-20261027`;
build dùng cùng workspace và action `build`. Invocation build
`20261027T051349.678039Z-c72f1304c2e34715bb81207c18f618dc-build-right`;
Unity Windows `E:/Unity/Hub/Editor/2022.3.25f1/Editor/Unity.exe`, đúng revision
`530ae0ba3889`. Chưa lấy tiến trình build đang chạy làm bằng chứng pass.
Không chạy suite gameplay, profile mặc định, stress hoặc thêm viewport ngoài
1280×720 và 1366×768 theo yêu cầu.

### Nguồn đã mở lại; playback chờ người xem

Các hash clip và cel trong bảng từng frame phía trên khớp byte với nguồn main
mới; giữ nguyên các hàng nhận xét từng frame, không thay chúng bằng pass chung.
Lần này đã mở trực tiếp đủ 19 sheet, mỗi sheet tối đa bốn cel với ảnh 96×64
và ảnh nearest 384×256 cạnh nhau; không giảm kích thước zoom. Đã so từng cặp
liên tiếp qua ranh giới sheet và cuối→đầu cho clip loop.

| Clip nguồn | Frame đã xem / tổng | Cặp đã xem / tổng, gồm loop | Quan sát bổ sung | Kết luận hiện tại |
|---|---:|---:|---|---|
| skeleton_die | 13/13 | 12/12 | F0–F1 còn thân đứng; F2 sọ tách; F3–F5 eo/chân hạ; F6–F8 tay/kiếm rơi; F9–F12 đống xương thấp và kiếm ngang. Không ép nối cuối→đầu. | **chưa kiểm chứng** playback 1×/0.25×; hình nguồn/nearest đã xem |
| skeleton_hitBySlayer | 2/2 | 2/2 | Hurt_1 trắng nằm trong chính albedo; Hurt_3 trở lại palette. Biên loop đổi flash theo hai key 50ms/16.66667ms. | **chưa kiểm chứng** playback 1×/0.25× |
| skeleton_hit | 5/5 | 5/5 | F1 flash và thân lệch; F2 hết trắng/kiếm dọc; F3–F4 thu tay, biên loop trở về đứng. Chân/grip nhìn được ở mọi key. | **chưa kiểm chứng** playback 1×/0.25× |
| skeleton_attack_heavy | 10/10 | 9/9 | F1 kiếm gần cạnh đầu; F2–F4 kéo sau vai; F5 smear lớn vẫn nối tay; F6–F7 thu cung; F8–F9 về kiếm cao. Key giữ 250ms/500ms không bị bỏ. | **chưa kiểm chứng** playback 1×/0.25× và chỗ nối runtime |
| skeleton_stun | 3/3 | 3/3 | Hurt_3→Hurt_4→Hurt_3 giữ chân, kiếm chéo; key cuối lặp vẫn được kiểm, loop về cùng Hurt_3. | **chưa kiểm chứng** playback 1×/0.25× |
| skeleton_idle | 8/8 | 8/8 | Đầu/ngực hạ rồi nâng, bàn tay bám chuôi; F7→F0 giữ hướng kiếm và hai chân. | **chưa kiểm chứng** playback 1×/0.25× |
| axe Walk | 12/12 | 12/12 | Chân chéo/đổi trụ, cán dựng; F6 mảnh nâu xuất hiện bên phải, F7–F11 rơi/nằm sàn; F11→F0 mảnh biến mất. | **lỗi** A1; playback còn thiếu; asset-only |
| axe Die | 11/11 | 10/10 | F0 mảnh nâu nằm tách bên phải cán; F1 mất mảnh trong khi sọ/eo bắt đầu rời; F5–F7 rìu đổ; F8–F10 đống xương giữ. | **lỗi** A1; playback còn thiếu |

Tổng nguồn đầu ra: **64/64 key, 61/61 cặp** (sáu clip gốc: 41/41 và
39/39; Walk/Die rìu: 23/23 và 22/22). Đây là review nguồn, không cộng vào
1.432 frame runtime lịch sử và không chứng minh đã nhìn xuyên VFX trong game.
Hash Walk anim `543ad7099b22d01d13ead2504a8874e2e777730c931739eaefdab75c2ce704b0`;
Die anim `d7a8229f8ddcce3e0ab42d6ac1950e50344c64e8242f3e36e3bf816f8096a2de`.

Nguồn GIF `e41867a829ef06b3296e8778aa43739bc8c0605b9df77e4212dbd373c4ba522c`:
đã mở lại **23/23 frame F13–F35, 22/22 cặp**, phạm vi PTS [1.3,3.6)s,
mỗi hold 100ms. Xem riêng cả ảnh nguyên 1100×600 và logical 220×120 + nearest
880×480. Script xác minh ảnh nguyên là phép nhân nearest 5× chính xác của
logical trước khi dùng logical; không suy diễn chi tiết sau nội suy.
F13–F18 khiên còn bên actor phải và bắt đầu rơi; F19–F25 mảnh khiên chạm
vùng lane rìu rồi nằm sàn; F26–F35 actor rìu rã và đổ rìu trong khi actor
khiên đã đứng lại. Hình nguồn khẳng định ownership A1; playback hai tốc độ
vẫn **chưa kiểm chứng**. Không tuyên bố đã xem toàn GIF ngoài đoạn này.

### Công cụ review tạm và điều kiện giữ tài nguyên

Root tạm: `E:/CodexTemp/TypingHero/pixel-audit-evidence-20261027`.
Trang local `http://127.0.0.1:8239/` cung cấp preview theo timestamp/duration
nguồn, hai tốc độ 1×/0.25×, hai vòng cho clip loop để xem biên cuối→đầu,
frame trước/sau, PTS/duration, ảnh nguyên và nearest 4× có thanh cuộn.
Preview chỉ trình bày albedo; chưa chứng minh material/normal/depth runtime.
Ghi chú người xem tách theo tên clip/hash, hai checkbox phải tự xác nhận,
nhận xét nhịp/độ đọc/PTS và xuất JSON; trình phát tự chạy hết không được tính
là xác nhận con người. Technical smoke: chín mục nguồn, đổi mục, bước frame,
PTS/duration và tải ảnh đúng kích thước; không có JS pageerror. Không dùng
technical smoke thay review playback.

Đã gửi yêu cầu xem riêng từng clip ở cả hai tốc độ; **chưa nhận xác nhận**.
Giữ server loopback, media, script, snapshot/build và worktree cho tới khi
thu đủ bằng chứng cùng phản hồi. Chỉ Markdown được đưa vào Git. Sau nghiệm
thu mới dọn tài nguyên task; không dọn media cũ của task khác hoặc checkout
ban đầu. Audit đang mở; Ready/glyph bản mới, runtime mới và preview độc lập
normal/depth/material còn **chưa kiểm chứng**.


### Bổ sung cấu trúc map A1 (không thay preview shader)

Đã trực tiếp mở bốn cel dưới đây, mỗi cel gồm albedo, depth, normal và
material-map lấy theo secondary texture GUID trong metadata, cùng hàng mirror X.
Ảnh source RGB nearest 4×, không có flash/cung/card; hàng mirror chỉ đổi tọa độ
ảnh để đối chiếu ownership, không mô phỏng phép biến đổi normal trong shader.
Material runtime được lần từ `DungeonLighting.asset.actorMaterial` qua
`DungeonSessionBindings.ApplyActorMaterial` tới renderer; việc lần tham chiếu
không phải bằng chứng đã render preview với material đó.

| Cel | Nhận xét trực tiếp | Chuyển tiếp | Trạng thái |
|---|---|---|---|
| axe Die F0 | Mảnh ngang nằm ngoài chân/cán trong albedo; depth, normal và surface cũng chứa vùng tương ứng. Mirror đưa mảnh sang phía trái. | → F1: cả vùng mảnh biến mất, sọ/eo tách tiếp. | **lỗi** A1 nguồn; shader preview chưa kiểm chứng |
| axe Die F1 | Không có mảnh rời cạnh cán; đầu/eo rời phù hợp albedo; các map đi cùng cel. | → F2: đã kiểm albedo ở bảng nguồn, chưa mở map F2. | **chưa kiểm chứng** toàn chuyển tiếp map/lighting |
| axe Walk F6 | Mảnh nâu mảnh dọc bên phải cán, tách chân; map đi cùng giữ vùng mảnh. | → F7: đã kiểm albedo; chưa mở map F7. | **lỗi** A1 nguồn; runtime không áp dụng |
| axe Walk F11 | Mảnh ngang sát đáy ngoài chân; mirror nằm bên trái; map có vùng tương ứng. | → F0 loop: albedo mất mảnh; chưa mở map F0. | **lỗi** A1 nguồn; playback còn thiếu |

Đã chuẩn bị 174 cel-map diagnostic trong thư mục tạm nhưng mới **xem 4/174**;
không dùng số file sinh ra làm coverage. Chưa thực hiện preview shader độc lập
với transform/material hiện hành; các chi tiết runtime bị che vẫn giữ
**chưa kiểm chứng**. Reflection inactive tiếp tục **không áp dụng với cấu hình
hiện tại**, không bật nó để tạo coverage.

Các atlas map đã mở được định danh như sau (hash nội dung PNG):

Assets/Art/EndlessSkeletons/axe/Die.png:

- `albedo`: `Assets\Art\EndlessSkeletons\axe\Die.png`, SHA256 `7bdf2c0ab75caf8277818faeb9d3ce406e4afa7a94a345c57db44f5ac169b507`.
- `_DepthMap`: `Assets\Art\Lighting\Depths\e81eb20d99accb0b6a215ec0644c0b97.png`, SHA256 `aa3a53f99454e411688dd8429facef03382fc1eadc5af711f42e01acdca3cb19`.
- `_NormalMap`: `Assets\Art\Lighting\Normals\e81eb20d99accb0b6a215ec0644c0b97.png`, SHA256 `7cf813a693620a07a539a0c4dca66e71bc713b08a1f63354de48c31cf48cd7a8`.
- `_MaterialMap`: `Assets\Art\Lighting\Normals\e81eb20d99accb0b6a215ec0644c0b97-surface.png`, SHA256 `27221ed8a49b3f2ea096afa9b231aa6c3cee613ca05607fb6f04e5d1f9c5189a`.

Assets/Art/EndlessSkeletons/axe/Walk.png:

- `albedo`: `Assets\Art\EndlessSkeletons\axe\Walk.png`, SHA256 `8cae0407a21d25fbe1898bd41df59d6c6bbd786a29e29ca467f054ba845d316e`.
- `_DepthMap`: `Assets\Art\Lighting\Depths\fbde5baff1333e402a40e42323e71fe1.png`, SHA256 `b3390c232c87b0d8fa55ba85d49051a000003676db6da7f5abbc38981a660f10`.
- `_NormalMap`: `Assets\Art\Lighting\Normals\fbde5baff1333e402a40e42323e71fe1.png`, SHA256 `82e3974c53c2dbd898a62592fded514c70a7163ba9021d0cfbf07c4e69f329cc`.
- `_MaterialMap`: `Assets\Art\Lighting\Normals\fbde5baff1333e402a40e42323e71fe1-surface.png`, SHA256 `293154c15602bc8ba1474d4f4e5f99fc8197792fd9aedb351e8c76808b7c6bbb`.


### Receipt build mới và Ready (đang bổ sung Enter)

Build Windows invocation nêu trên hoàn tất **exit 0**, player có `qa=true`,
33 file; manifest SHA256
`135fecd6b062447757f01c5515dc9b971f516635609b258ef703c27dae1e0acb`
(theo sort path tương đối + NUL + hash file + LF). Chỉ build phục vụ audit;
không có deployment hoặc thay đổi sản phẩm trong repo.

Lượt `capture-4`: Edge **156.0.4314.42**, NVIDIA GTX1650Ti / ANGLE D3D11,
điều khiển phím/chuột, fixture Firebase và chặn host ngoài giữ nguyên.
Pilot chết ở Double Strike nên toàn lượt **failed**, không lấy kết quả riêng
bên dưới làm pass toàn flow.

- Đã mở trực tiếp full PNG `ready-1-1280`, `ready-1-1366`, `ready-1-return`.
  Probe trước/sau xác nhận đúng 1280×720 → 1366×768 → 1280×720.
  `Ready · Enter` nằm trọn trong mặt nút, có khoảng trống quanh chữ; viền và
  góc không cắt, card giữ trong viewport. Không còn bar/kích thước 1366 sau
  khi trở về 1280. Phóng không nguyên tại 1366 vẫn theo phong cách hiện hành.
- Click thực tại center `PracticeButton` (x=0.2541666627, y=0.6351852417 của
  canvas, khoảng 325.333/457.333px tại 1280) đi sang Double Strike. Vùng click
  đã thử là tâm nút; không suy ra đã click mọi pixel/biên hit area.
- Caption/viền/padding/resize: **đạt** trong ba ảnh và revision này.
  Click tâm: **đạt**; Enter lần xuất hiện khác: **chưa kiểm chứng** ở checkpoint
  này. Giữ việc kiểm Enter riêng, không dùng chữ trong caption để chứng minh.
- Input `Đặng_Á9` được nhập bằng CDP keyDown có text/key/unmodifiedText + keyUp
  qua đường bàn phím Unity. Ảnh input `name-1280` không có ô vuông hoặc mất dấu
  nhìn thấy; QA state giữ nguyên chuỗi và HUD hiển thị tên. QA label
  `F6 Spark: BCompact` che một phần mép trên chữ HUD, nên toàn chi tiết glyph
  HUD vẫn **chưa kiểm chứng**; không gọi label QA là lỗi sản phẩm.

### Tutorial mới — từng frame và từng cặp

Bản `capture-4/tutorial-reading.webm`, SHA256
`f03c9ae446b25def0803b2ae2636d233320422b4cfd430070903e5172c85f1e5`.
Phạm vi toàn recording, F0–F33, PTS 0–2.590s; stop được chốt trước serialize
ở 2.676335s. Container báo duration 2.590164s, không có duration packet cuối;
không dùng chênh wall-clock để chứng nhận hold cuối. Đã trực tiếp xem
**34/34 frame, 33/33 cặp** ở full1280×720 và crop(400,230)–(850,500), nearest
1800×1080 không thu nhỏ; đã mở riêng từng frame sau sheet định vị.
Controller/cel reading: enemy `Skeleton_01_White_Idle_0`, player
`male_hero-idle_0`, `timeScale=0` trong toàn trace. Đây là pose giữ khi đọc,
không phải vòng idle chạy. Màn che intro mở lên ở đầu clip; card và Space
che vùng grip/thân nhất định. Encoded VP9 không phải pixel-identical PNG;
không lấy nhiễu/độ mềm của encode làm lỗi artwork hoặc chứng nhận texel ẩn.
Playback con người 1×/0.25× vẫn **chưa kiểm chứng**.

| Clip / frame | PTS / duration (s) | Reference / quan sát trực tiếp | Chuyển tiếp | Trạng thái | Bằng chứng |
|---|---|---|---|---|---|
| tutorial-reading-new / F0 | 0.000 / 0.075000 | Idle_0 / Màn che đen còn phủ đầu/thân; chỉ phần sàn/chân thấp và mép card nhìn được. | → F1: màn che mở tiếp; pose actor giữ. | chưa kiểm chứng | hash trên; F0; `tutorial-reading-new/000000.png` và nearest tạm |
| tutorial-reading-new / F1 | 0.075 / 0.043000 | Idle_0 / Màn che mở lên, thân player và skeleton hiện; đầu skeleton còn sát card trên, Space che vùng giữa. | → F2: màn che mở tiếp; pose actor giữ. | chưa kiểm chứng | hash trên; F1; `tutorial-reading-new/000001.png` và nearest tạm |
| tutorial-reading-new / F2 | 0.118 / 0.034000 | Idle_0 / Che đen rút tiếp lên trên Score/Combo; tay cầm và kiếm vẫn cùng hướng, sàn giữ mốc. | → F3: màn che mở tiếp; pose actor giữ. | chưa kiểm chứng | hash trên; F2; `tutorial-reading-new/000002.png` và nearest tạm |
| tutorial-reading-new / F3 | 0.152 / 0.031000 | Idle_0 / Màn che còn ở hàng trên HUD; hai actor giữ cùng pose, không trượt chân rõ. | → F4: màn che mở tiếp; pose actor giữ. | chưa kiểm chứng | hash trên; F3; `tutorial-reading-new/000003.png` và nearest tạm |
| tutorial-reading-new / F4 | 0.183 / 0.066000 | Idle_0 / Score/HUD lộ thêm; card và Space giữ trước actor, kiếm hướng chéo không đổi. | → F5: màn che mở tiếp; pose actor giữ. | chưa kiểm chứng | hash trên; F4; `tutorial-reading-new/000004.png` và nearest tạm |
| tutorial-reading-new / F5 | 0.249 / 0.053000 | Idle_0 / Màn che gần hết; đầu/kiếm nhìn rõ hơn, chân và bóng vẫn trên cùng landmark. | → F6: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F5; `tutorial-reading-new/000005.png` và nearest tạm |
| tutorial-reading-new / F6 | 0.302 / 0.047000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F7: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F6; `tutorial-reading-new/000006.png` và nearest tạm |
| tutorial-reading-new / F7 | 0.349 / 0.053000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F8: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F7; `tutorial-reading-new/000007.png` và nearest tạm |
| tutorial-reading-new / F8 | 0.402 / 0.047000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F9: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F8; `tutorial-reading-new/000008.png` và nearest tạm |
| tutorial-reading-new / F9 | 0.449 / 0.056000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F10: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F9; `tutorial-reading-new/000009.png` và nearest tạm |
| tutorial-reading-new / F10 | 0.505 / 1.064000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F11: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F10; `tutorial-reading-new/000010.png` và nearest tạm |
| tutorial-reading-new / F11 | 1.569 / 0.046000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F12: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F11; `tutorial-reading-new/000011.png` và nearest tạm |
| tutorial-reading-new / F12 | 1.615 / 0.048000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F13: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F12; `tutorial-reading-new/000012.png` và nearest tạm |
| tutorial-reading-new / F13 | 1.663 / 0.046000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F14: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F13; `tutorial-reading-new/000013.png` và nearest tạm |
| tutorial-reading-new / F14 | 1.709 / 0.058000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F15: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F14; `tutorial-reading-new/000014.png` và nearest tạm |
| tutorial-reading-new / F15 | 1.767 / 0.036000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F16: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F15; `tutorial-reading-new/000015.png` và nearest tạm |
| tutorial-reading-new / F16 | 1.803 / 0.057000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F17: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F16; `tutorial-reading-new/000016.png` và nearest tạm |
| tutorial-reading-new / F17 | 1.860 / 0.036000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F18: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F17; `tutorial-reading-new/000017.png` và nearest tạm |
| tutorial-reading-new / F18 | 1.896 / 0.035000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F19: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F18; `tutorial-reading-new/000018.png` và nearest tạm |
| tutorial-reading-new / F19 | 1.931 / 0.048000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F20: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F19; `tutorial-reading-new/000019.png` và nearest tạm |
| tutorial-reading-new / F20 | 1.979 / 0.046000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F21: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F20; `tutorial-reading-new/000020.png` và nearest tạm |
| tutorial-reading-new / F21 | 2.025 / 0.034000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F22: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F21; `tutorial-reading-new/000021.png` và nearest tạm |
| tutorial-reading-new / F22 | 2.059 / 0.044000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F23: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F22; `tutorial-reading-new/000022.png` và nearest tạm |
| tutorial-reading-new / F23 | 2.103 / 0.033000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F24: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F23; `tutorial-reading-new/000023.png` và nearest tạm |
| tutorial-reading-new / F24 | 2.136 / 0.045000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F25: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F24; `tutorial-reading-new/000024.png` và nearest tạm |
| tutorial-reading-new / F25 | 2.181 / 0.081000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F26: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F25; `tutorial-reading-new/000025.png` và nearest tạm |
| tutorial-reading-new / F26 | 2.262 / 0.018000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F27: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F26; `tutorial-reading-new/000026.png` và nearest tạm |
| tutorial-reading-new / F27 | 2.280 / 0.051000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F28: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F27; `tutorial-reading-new/000027.png` và nearest tạm |
| tutorial-reading-new / F28 | 2.331 / 0.072000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F29: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F28; `tutorial-reading-new/000028.png` và nearest tạm |
| tutorial-reading-new / F29 | 2.403 / 0.048000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F30: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F29; `tutorial-reading-new/000029.png` và nearest tạm |
| tutorial-reading-new / F30 | 2.451 / 0.047000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F31: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F30; `tutorial-reading-new/000030.png` và nearest tạm |
| tutorial-reading-new / F31 | 2.498 / 0.063000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F32: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F31; `tutorial-reading-new/000031.png` và nearest tạm |
| tutorial-reading-new / F32 | 2.561 / 0.029000 | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | → F33: giữ anchor/chân và lớp card; vẫn có giới hạn encode/che lấp. | chưa kiểm chứng | hash trên; F32; `tutorial-reading-new/000032.png` và nearest tạm |
| tutorial-reading-new / F33 | 2.590 / chưa xác định | Idle_0 / Reading giữ pose: kiếm chéo skeleton và tay/kiếm player không đổi vị trí rõ; card/Space tiếp tục che phần thân/grip, bóng/chân giữ landmark. | Kết thúc recording trong reading; không ép loop, duration cuối chưa xác định. | chưa kiểm chứng | hash trên; F33; `tutorial-reading-new/000033.png` và nearest tạm |

## Capture mới và trạng thái còn mở

Checkpoint tiếp tục trên `e5c48017201d4d5ea780cc01b0b9039f1fe3569b`.
Đây là **tiến độ audit chưa hoàn tất**, không phải kết luận game đạt.
Chỉ `docs/plans/pixel-consistency-audit.md` thay đổi trong worktree task;
A1, artwork, gameplay, API, package và cấu hình sản phẩm giữ nguyên.

### Receipt runtime mới

`capture-5/browser.json` báo `passed`, **3 check điều khiển/thu bằng chứng**;
không phải pass chất lượng hình ảnh. Không có exception/page error.
Firebase dùng fixture: Endless GET 2 / PUT 1; fixture global 0 / 0.
`externalBlocked=0` nghĩa không phát sinh request bị chặn trong lượt này,
không phải bằng chứng đã gửi request tới provider thật.
Môi trường thực là Windows, Edge 156.0.4314.42, ANGLE D3D11 / GTX 1650 Ti,
Unity Windows 2022.3.25f1. Các tên image Linux trong trường toolchain kế thừa
không phải môi trường đã chạy. QA bridge chỉ đọc; input bằng bàn phím/chuột.

| Media | SHA256 | Frame toàn recording | Stop trước serialize (s) |
|---|---|---:|---:|
| `capture-5/combat.webm` | `06e5ed8c3ecc8837f1662db20f13fa941d56f8fe0b1d57e483279273b74a623c` | 7.763 | 312.823770 |
| `capture-5/player-hurt-death.webm` | `0e0854d6c92a1db860fe685b1d7e6e0a7740705e38018785cd2ae34ea5906fe9` | 740 | 29.722370 |

Recording combat dừng sau defeated=6 trong cel đầu Die của Captain,
**không có cleanup Captain hoặc Victory**. Không dùng đuôi này để chứng minh
cleanup. Đoạn Guard bên dưới giữ trọn death/cleanup của bộ khiên.
Lượt chết player riêng có lethal hit ~26.860s, result ~28.408s, stop 29.722370s.
Duration của media và hold frame cuối được giữ riêng với wall-clock stop.
VP9 VFR giữ frame lặp và PTS gốc; không suy ra FPS render hoặc pixel-identical.

### Chín đoạn mới — biên chốt từ trace trước khi xem hình

Mỗi hàng dưới đây vẫn **chưa kiểm chứng** toàn mục khi thiếu kiểm hình hoặc
playback. Số frame đã xuất không thay cho số đã xem. Crop actor là
(350,180)–(950,530), nearest 4× 2400×1400; full canvas 1280×720 và nearest
5120×2880 có pan riêng khi cần xem vùng ngoài crop.

| Đoạn | PTS chọn (s), giữ mọi frame | Global frame | Tổng frame / cặp | Đã xem full + nearest / cặp | Kết luận |
|---|---|---|---:|---:|---|
| `normal-idle-parry-hurt-slayer` | [0, 18) | F0–F316 | 317 / 316 | 20 / 19 | chưa kiểm chứng |
| `sword-death-cleanup` | [32.4, 35.8) | F571–F641 | 71 / 70 | 0 / 0 | chưa kiểm chứng |
| `sword-double-attacks` | [38, 44.9) | F682–F822 | 141 / 140 | 0 / 0 | chưa kiểm chứng |
| `axe-idle-attack-hurt` | [74.65, 79.8) | F1451–F1564 | 114 / 113 | 0 / 0 | chưa kiểm chứng |
| `axe-death-cleanup` | [118.4, 121.5) | F2359–F2431 | 73 / 72 | 0 / 0 | chưa kiểm chứng |
| `shield-idle-attacks-hurt` | [121.2, 130.0) | F2425–F2638 | 214 / 213 | 0 / 0 | chưa kiểm chứng |
| `shield-death-cleanup` | [153.25, 156.45) | F3247–F3329 | 83 / 82 | 0 / 0 | chưa kiểm chứng |
| `player-hurt-idle-new` | [1.8, 4.5) | F41–F112 | 72 / 71 | 0 / 0 | chưa kiểm chứng |
| `player-death-result-new` | [26.0, 29.723) | F645–F739 | 95 / 94 | 0 / 0 | chưa kiểm chứng |

Tổng chín đoạn: **1.180 frame / 1.171 cặp**, hiện trực tiếp xem đủ full/nearest
**20 frame / 19 cặp** của đoạn đầu. Tutorial mới tách riêng: **34/34 frame,
33/33 cặp** đã xem như ledger trên. Tổng runtime mới đã xem: **54 frame,
52 cặp**; không tính ảnh xem lướt hoặc output tool bị cắt vào coverage.
Chưa khẳng định biên chín đoạn đủ recovery cho tới khi đã trực tiếp kiểm hết.
Nếu thiếu biên phải mở rộng và kiểm lại, không cắt bỏ frame khó.

Đối chiếu actor bằng hình/controller/cel; role Run đổi trước cleanup.
Walk và Attack2 rìu vẫn asset-only theo nguồn revision mới, không tạo runtime
coverage giả bằng cách ép animator/HP. Reflection inactive: **không áp dụng với
cấu hình hiện tại**. Không bật reflection để tạo coverage.

### Ready — kết quả mới thay trạng thái Enter còn mở phía trên

Hai lần xuất hiện riêng ở `capture-5`: lần 1 click sau defeated=1, HP35;
lần 2 Enter sau defeated=2, HP35. Trace xác nhận vào Double Strike rồi Heavy.
Ba ảnh lần 2 đã mở trực tiếp 1280×720 → 1366×768 → 1280×720; caption
`Ready · Enter` đầy đủ, viền/padding nằm trong card, trở về đúng viewport.
Kết hợp ảnh lần 1 của capture-4 đã kiểm và input center click thực: caption,
resize và hai cách kích hoạt **đạt tại e5c48017** trong phạm vi đã thử; không khẳng định
mọi pixel biên hit area. Không tái sử dụng ảnh cũ của nhãn Continue.

| Ảnh lần Enter, cùng player/revision mới | SHA256 |
|---|---|
| `capture-5/ready-2-1280.png` | `43c762273053e4429995ecd9e5b430505a39f1eb95bc4b07f3c7b0faf07916b4` |
| `capture-5/ready-2-1366.png` | `2259e0727485e56cb14ebc343b82c38a26d4dd68cd87fac88676fcf4ee574827` |
| `capture-5/ready-2-return.png` | `8b1767c2dba1cbf2f8fc6c79b7cadf989aec2f27f6b87c3e2def17d759ccbb80` |

`Đặng_Á9`: input hiện đúng dấu qua đường bàn phím; HUD có chuỗi đúng nhưng
mép trên bị label QA che. Input **đạt** trong ảnh đã xem; toàn nét glyph HUD
**chưa kiểm chứng**. Không mở rộng kết luận sang Unicode hoặc màn khác.

### Preview độc lập cho phần che — giới hạn quan sát

`shader-preview-2/receipt.json`: bốn capture, GPU GTX 1650 Ti,
shader `CLACKBLADE/Dungeon Sprite`, material
`Assets/Art/Environment/KayKitDungeon/DungeonSprite.mat`.
Preview snapshot dùng vị trí template (1.55999994,-1.38999999,0),
scale (-1,1,1), rotation identity; DungeonWorld, camera/torch và actor pass
hiện hành, tắt renderer khác/VFX trong preview riêng, không sửa player audit.
Render 320×180 trước presentation post; đã xem cả bốn ảnh nearest 4×.
Vùng tối chỉ lộ một số phần ấm/sáng: **chưa kiểm chứng toàn chi tiết ẩn**.
Không dùng preview này chứng nhận chi tiết đã nhìn thấy ở runtime.
Map diagnostic riêng chỉ xem 4/174 cel (Die_0, Die_1, Walk_6, Walk_11 rìu)
như bảng trước; số 174 được tạo không có nghĩa đã review 174.

### Playback người xem và tài nguyên giữ lại

Bộ review localhost [mở tại đây](http://127.0.0.1:8239/) hiện có **19 mục**:
6 skeleton nguồn, Walk/Die rìu, extraction F13–F35, tutorial và 9 đoạn mới.
Ảnh nguồn giữ key/duration/pivot/canvas; loop xem hai vòng để có cuối→đầu.
Video runtime dùng file gốc byte-identical, chọn biên theo PTS, không retime.
Frame trước/sau, full canvas, actor và nearest có pan; ghi chú lưu riêng theo
name/hash rồi xuất JSON. Tự chạy playback không thay xác nhận con người.

**Chưa nhận xác nhận người xem cho bất kỳ clip mới nào.** Mỗi clip cần xác nhận
xem hết ở 1× và 0.25×, nhịp/độ đọc, PTS nghi vấn và nhận xét cụ thể.
Chưa đóng audit, chưa tích hợp báo cáo như audit hoàn tất và chưa dọn media.
Ngoài playback còn thiếu 1.160 frame / 1.152 cặp runtime trong chín đoạn mới,
đối chiếu mọi chi tiết bị che và xác nhận biên recovery từ hình.
Các thiếu hụt này là công việc agent còn lại, không quy cho người xem.

Giữ `E:/CodexTemp/TypingHero/pixel-audit-evidence-20261027` (media/build/snapshot/
preview/driver/server) và worktree `D:/Projects/TypingHero-worktrees/pixel-audit-evidence`,
branch `micharel09/pixel-audit-evidence`. Server chỉ bind localhost 8239,
PID ghi tại `review-server.pid`. Điều kiện dọn: đã review đủ, phản hồi người xem
được ghi, báo cáo commit/push/tích hợp và xác minh remote. Không dọn checkout
gốc hoặc tài nguyên có từ trước. A1 tiếp tục **lỗi xác nhận**, sửa ở task riêng.

### Ledger runtime đầu đoạn — phần đã trực tiếp xem

`normal-idle-parry-hurt-slayer`, hash `combat.webm` ở receipt capture-5 phía trên.
Đã mở riêng F0–F19 tại full 1280×720 và actor nearest 2400×1400,
không thu nhỏ ảnh zoom. PTS/duration dưới đây lấy từ media, giữ cả hold.
Các hàng **chưa kiểm chứng** toàn mục: chưa playback người xem, chưa đối chiếu
xong controller/cel, phần grip bị flash che không được coi là đã nhìn thấy.

| Frame | PTS / duration (s) | Quan sát full/nearest | Chuyển từ frame trước | Trạng thái |
|---|---|---|---|---|
| F0 | 0.000000 / 0.062000 | Hai actor ở pose idle, khăn player ra sau trái; kiếm skeleton chéo lên; chân/bóng bám vùng gạch sáng. | Đầu đoạn; chưa có cặp trước. | chưa kiểm chứng |
| F1 | 0.062000 / 0.073000 | Khăn player nhấc nhẹ, kiếm/grip skeleton vẫn liền; sàn và chân giữ mốc. | Idle thay cel, không thấy dịch toàn camera rõ. | chưa kiểm chứng |
| F2 | 0.135000 / 0.067000 | Silhouette tóc/khăn đổi nhẹ; hai chân player giữ đáy; skeleton giữ kiếm chéo. | Khăn tiếp tục dao động; nền giữ mốc. | chưa kiểm chứng |
| F3 | 0.202000 / 0.098000 | Khăn player hạ nhẹ; tay skeleton vẫn gắn chuôi; chưa có flash. | Dao động idle, không thấy đảo sorting actor. | chưa kiểm chứng |
| F4 | 0.300000 / 0.056000 | Đuôi khăn cong sau trái; hai actor đứng trên cùng landmark sàn. | Pose idle tiếp tục, môi trường có nhấp nháy đuốc. | chưa kiểm chứng |
| F5 | 0.356000 / 0.052000 | Khăn/tóc player thay nhẹ; skeleton giữ thân và đường kiếm chéo. | Không thấy nhảy anchor chân so với gạch sàn. | chưa kiểm chứng |
| F6 | 0.408000 / 0.074000 | Tay player sát thân, khăn ra trái; bóng chân vẫn dưới actor. | Idle tiếp tục; chưa có afterimage. | chưa kiểm chứng |
| F7 | 0.482000 / 0.055000 | Đuôi khăn nhấc; skeleton chuẩn bị thay hướng tay, chân giữ mốc. | Chuyển pose nhỏ; full canvas không thấy clip đầu/chân. | chưa kiểm chứng |
| F8 | 0.537000 / 0.048000 | Player idle, khăn trải sau; skeleton kiếm chéo lên trái, grip nhìn liền. | Khăn hạ nhẹ, nền và bóng giữ landmark. | chưa kiểm chứng |
| F9 | 0.585000 / 0.087000 | Skeleton thu tay/kiếm dựng gần mặt trong chuẩn bị đòn; player vẫn idle. | Kiếm đổi từ chéo sang dựng, không tách khỏi tay rõ. | chưa kiểm chứng |
| F10 | 0.672000 / 0.042000 | Skeleton giữ kiếm dựng gần mặt; khăn player đổi nhẹ; chân đứng nguyên. | Hold tư thế chuẩn bị; không coi là frame bị bỏ. | chưa kiểm chứng |
| F11 | 0.714000 / 0.039000 | Skeleton đưa kiếm chéo lên phải qua vai, thân xoay; player idle. | Tay/kiếm đổi hướng chuẩn bị, chân còn trên vùng gạch cũ. | chưa kiểm chứng |
| F12 | 0.753000 / 0.053000 | Skeleton giữ kiếm qua vai; player giữ tư thế và khăn sau trái. | Hold chuẩn bị; không thấy đổi lớp trước/sau sàn. | chưa kiểm chứng |
| F13 | 0.806000 / 0.073000 | SPACE chuyển nền sáng; skeleton giữ kiếm qua vai, đầu hạ nhẹ. | Prompt đổi màu, actor tiếp tục chuẩn bị; HUD không dịch. | chưa kiểm chứng |
| F14 | 0.879000 / 0.080000 | Player hạ thân, rút kiếm dựng trước mặt; skeleton kiếm qua vai. | Player vào pose phòng thủ, khăn vòng sau; đầu/chân/kiếm nằm đủ trong crop. | chưa kiểm chứng |
| F15 | 0.959000 / 0.172000 | Player giữ kiếm dựng và thân thấp; skeleton giữ đòn chuẩn bị. | Hold phòng thủ; không suy ra mọi frame render giữa hai PTS. | chưa kiểm chứng |
| F16 | 1.131000 / 0.042000 | Spark trắng/vàng che vùng va chạm; player thấp, kiếm quét sát sàn; skeleton duỗi tay ngang. | Đòn tới/parry và camera rung; sàn/nền cùng dịch, không quy ngay là chân trượt. Grip ở spark chưa thấy. | chưa kiểm chứng |
| F17 | 1.173000 / 0.230000 | Spark thu thành tia cam; skeleton còn tay duỗi, player giữ thấp; chân vẫn trên vùng sàn sáng. | Rung đổi vị trí nền/actor; spark giảm; chưa chứng minh chi tiết grip bị che. | chưa kiểm chứng |
| F18 | 1.403000 / 0.048000 | Spark trung tâm hết, hạt cam tản; skeleton thu kiếm chéo, player vẫn thấp với kiếm sát sàn. | Skeleton hồi tay, camera về gần bố cục trước; số +30 mờ dần. | chưa kiểm chứng |
| F19 | 1.451000 / 0.069000 | Skeleton trở lại kiếm chéo cao và thân đứng; player còn pose thấp; hạt cam tiếp tục tản. | Hồi phục skeleton, landmark sàn giữ; player chưa về idle tại frame này. | chưa kiểm chứng |

### Main đổi trong lúc thu bằng chứng — quyết định nguồn còn chờ

Fetch lại origin ngày 2026-10-27 xác nhận main mới
`66749da15890be3b8ae039a81228249acda0a4ed`, commit
`Type READY in the word tray between duels`.
Diff từ revision capture `e5c48017` thay `TypingManager.cs`, `EndlessDuel.cs`
và hai file test liên quan. `EndlessDuel` bỏ tạo overlay/Update xử lý Enter,
chuyển sang `TypingManager.BeginReady(Continue)`. TypingManager hiển thị `READY`,
ẩn countdown và nhận từng chữ để tiếp tục; không còn nút `Ready · Enter`.
Không có delta artwork/shader/camera trong diff này, nhưng flow giữa trận
và typing đã thay đổi: ảnh click/Enter ở trên **không áp dụng cho main mới**.

Yêu cầu kế hoạch đòi nhãn/click/Enter nay không tồn tại trên main mới; audit-only
không cho phép khôi phục nó. Đã hỏi người dùng chọn audit main mới với gõ READY
hoặc giữ revision e5c48017. Chưa có trả lời; chưa build/capture lại main mới,
chưa dùng capture cũ chứng nhận flow mới, chưa tích hợp báo cáo như hoàn tất.
Việc kiểm 1.160 frame / 1.152 cặp còn lại của bản e5c48017 chưa xong; giữ nguyên
media để tiếp tục sau khi chốt mốc, không coi thay đổi main là bằng chứng pass.

Trang review kiểm kỹ thuật sau khi thêm đủ 19 mục: bước F1 PTS0.062,
video playbackRate0.25 chạy, mục cuối bắt đầu F645 PTS26.008, nearest width5120px,
không có page error. Receipt tạm `review-smoke.json`. Đã sửa sự kiện timeupdate
ghi đè frame vừa chọn và bỏ preload mọi PNG runtime trong trang tạm.
Đây là check công cụ review, **không phải xác nhận playback người xem**.
