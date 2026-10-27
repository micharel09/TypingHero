# Rà soát độ đồng bộ pixel trên main — 2026-10-27

> **Audit đang mở; A1 có bản sửa nguồn trên nhánh riêng, chưa nghiệm thu runtime.** Bộ capture gõ `READY` thuộc revision **`66749da1`**, không phải giao diện main hiện tại.
> **Checkpoint đã kiểm:** ba clip READY có ledger 806 frame/803 cặp chuyển tiếp; tutorial 34/33 và normal 339/338 giữ riêng. Tổng catalog runtime đã ghi kiểm 1.186/1.180 trên 1.900/1.890.
> **Còn mở:** 714 frame/710 cặp chưa ghi kiểm trong catalog lịch sử (F682–F688 đã đồng bộ metadata và ghi chú), preview chi tiết bị che và playback người xem 1×/0.25×. Không dùng số frame đã xem để tuyên bố clip đạt toàn bộ.
> **Giới hạn áp dụng:** main đã đổi READY/intro và lighting/rendering sau R. Xem [checkpoint tích hợp](#checkpoint-tích-hợp-tài-liệu-và-phạm-vi-bằng-chứng).
> Các số liệu và kết luận trong phần lịch sử giữ theo revision của từng lượt; không cộng vào coverage hiện tại hoặc dùng chứng nhận main mới.

## Checkpoint tích hợp tài liệu và phạm vi bằng chứng

Đối chiếu R `66749da15890be3b8ae039a81228249acda0a4ed` với main đã fetch
`bca2a187e779685dfc8e3bdd7688806a19819045` trước commit checkpoint.
Phần hoàn tất được công bố ngay; audit tiếp tục mở, không đợi toàn bộ audit
để tích hợp tài liệu. Ledger vẫn mô tả revision/hash ghi kèm, không chuyển nhãn
thành bằng chứng runtime của main mới.

| Nhóm bằng chứng | Phần đã hoàn tất tại revision capture | Áp dụng sau main đổi |
|---|---|---|
| READY R | 463/462 kiếm, 162/161 rìu, 181/180 khiên; đủ ledger frame encode, resize đã chốt và trace input/stats | UI READY, intro và spawn đã đổi sang ReadyRitual/MagicCircle/compact prompt; chưa chứng nhận bố cục, nhập hoặc chuyển trận của main mới. |
| Tutorial + normal e5 | 34/33 tutorial reading khóa timeScale và 339/338 normal; đối chiếu e5→R đã ghi riêng | Main đổi DungeonWorld, profile ánh sáng, normal/surface, DungeonSprite/depth blur/camera; rendering/landmark/lighting chưa tái xác minh trên main mới. |
| Glyph không QA trên R | Đã quan sát input/HUD `Đặng_Á9` bằng bàn phím và native/nearest 4× | Là kết quả trên R; cần kiểm ảnh hưởng UI/render mới trước khi tuyên bố đạt main. |
| Skeleton nguồn/reference | Giữ lịch sử 64 key/61 cặp nguồn và 23/22 reference, không cộng runtime | Main đổi import metadata Sword Attack1/Die và normal/surface; chưa tái xác nhận import/render tương ứng. Giữ kết quả xem nguồn cũ, không coi là chứng nhận cấu hình mới. |
| A1 | Lỗi asset rìu đã xác nhận; không sửa trong task này | Tiếp tục lỗi xác nhận. Nhãn frame/PTS runtime lịch sử sau resize đã bị vô hiệu hóa bởi lỗi bộ lọc n; không dùng riêng F2392/99.008s để chứng nhận runtime mới. |
| Phần còn thiếu | Catalog còn 721 frame/716 cặp chưa ghi kiểm; F682–F688 có ghi chú chờ đồng bộ | Tiếp tục preview đúng cel bị che, kiểm biên/recovery, cập nhật manifest cuối và lấy playback 1×/0.25×; thu lại các phần bị ảnh hưởng trên main mới. |

Chỉ Markdown được commit; không sửa game/API/dependency/cấu hình và không chạy
suite game cho checkpoint tài liệu. Test đã dẫn là kết quả có sẵn tại revision
tương ứng. Media giữ để tiếp tục; bản lưu lượt tiếp tục nằm ngoài CodexTemp tại
`D:/Projects/TypingHero-evidence/pixel-audit-continue-20261027`.

## Phạm vi và revision

Revision capture **R** của lượt tiếp tục là
`66749da15890be3b8ae039a81228249acda0a4ed`, fetch ngày 2026-10-27 từ
`https://github.com/micharel09/TypingHero.git`. Đã merge vào nhánh task
`micharel09/pixel-audit-evidence` tại `8c04ec2a`, giữ checkpoint `97ae6819`.
Chỉ báo cáo này được sửa; không sửa A1, artwork, gameplay, API, dependency hay
cấu hình sản phẩm. Checkout gốc bẩn tiếp tục được giữ nguyên.

Các receipt lượt đầu dưới đây là lịch sử: audit bắt đầu trên `origin/main` tại
`4db567bada8c7fcc512c426540941a4b894052eb`.
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

## Kết luận lịch sử — đọc cùng checkpoint hiện tại

Lượt bổ sung lịch sử xác nhận **A1 ở nguồn và runtime Die rìu**: mảnh khiên của đối
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
  HUD trong ảnh e5 vẫn **chưa kiểm chứng**; xem bằng chứng bổ sung R không QA bên dưới. Không gọi label QA là lỗi sản phẩm.

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

### Các đoạn capture e5 — biên và khả năng áp dụng sau sửa phép trích

Mỗi hàng dưới đây vẫn **chưa kiểm chứng** toàn mục khi thiếu kiểm hình hoặc
playback. Số frame đã xuất không thay cho số đã xem. Crop actor là
(350,180)–(950,530), nearest 4× 2400×1400; full canvas 1280×720 và nearest
5120×2880 có pan riêng khi cần xem vùng ngoài crop.

| Đoạn | PTS chọn (s), giữ mọi frame | Global frame | Tổng frame / cặp | Đã xem full + nearest / cặp | Kết luận |
|---|---|---|---:|---:|---|
| `normal-idle-parry-hurt-slayer` | [0, 19.3) | F0–F338 | 339 / 338 | 339 / 338 | chưa kiểm chứng |
| `sword-death-cleanup` | [32.4, 35.8) | F571–F641 | 71 / 70 | 0 / 0 | lịch sử e5; loại khỏi nghiệm thu R; PNG/PTS không dùng sau phát hiện bộ lọc n reset |
| `sword-double-attacks` | [38, 44.9) | F682–F822 | 141 / 140 | 0 / 0 | chưa kiểm chứng |
| `axe-idle-attack-hurt` | [74.65, 79.98) | F1451–F1567 | 117 / 116 | 0 / 0 | chưa kiểm chứng |
| `axe-death-cleanup` | [118.4, 121.5) | F2359–F2431 | 73 / 72 | 0 / 0 | lịch sử e5; loại khỏi nghiệm thu R; PNG/PTS không dùng sau phát hiện bộ lọc n reset |
| `shield-idle-attacks-hurt` | [121.2, 132.5) | F2425–F2693 | 269 / 268 | 0 / 0 | chưa kiểm chứng |
| `shield-death-cleanup` | [153.25, 156.45) | F3247–F3329 | 83 / 82 | 0 / 0 | lịch sử e5; loại khỏi nghiệm thu R; PNG/PTS không dùng sau phát hiện bộ lọc n reset |
| `player-hurt-idle-new` | [1.1, 4.5) | F25–F112 | 88 / 87 | 0 / 0 | chưa kiểm chứng |
| `player-death-result-new` | [25.5, 29.723) | F634–F739 | 106 / 105 | 0 / 0 | chưa kiểm chứng |

Tổng sáu đoạn combat còn áp dụng: **1.060 frame / 1.054 cặp** sau mở biên; đã xem 339 / 338. Ba đoạn death/card cũ (227/224) chỉ lịch sử, không tính nghiệm thu R. Tutorial giữ riêng 34/33. Ba clip death→READY mới có 806/803; coverage trực tiếp cập nhật ở bảng READY bên dưới. Không cộng ảnh gán sai PTS hoặc output tool bị cắt.
Biên đã đối chiếu trace, còn phải xác nhận đủ recovery bằng hình.
Nếu thiếu biên phải mở rộng và kiểm lại, không cắt bỏ frame khó.

Đối chiếu actor bằng hình/controller/cel; role Run đổi trước cleanup.
Walk và Attack2 rìu vẫn asset-only theo nguồn R `66749da1`, không tạo runtime
coverage giả bằng cách ép animator/HP. Reflection inactive: **không áp dụng với
cấu hình R đã capture**. Không bật reflection để tạo coverage.

### Ready card — kết quả lịch sử e5, không áp dụng cho R

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

`Đặng_Á9` ở capture e5: input hiện đúng dấu qua bàn phím; HUD bị label QA che, nên ảnh lịch sử chưa đủ quan sát toàn glyph. Khoảng trống này đã được bổ sung bằng bản R không QA, input và HUD full/nearest 4× tại mục Main đổi bên dưới; không mở rộng sang Unicode khác.

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
8 clip nguồn, extraction reference F13–F35, tutorial, 6 đoạn combat giữ lại và
3 đoạn death→READY trên R. Đây là danh sách đang kiểm, chưa phải manifest nghiệm thu
cuối. Ba đoạn death/card cũ loại khỏi danh sách này; giữ manifest/hash/media lịch sử.
Ảnh trích nhầm do bộ đếm n reset được giữ trong `review/history-n-filter/`;
metadata trước sửa tại `review/clips-before-pts-repair.json`, không dùng xác nhận R.
Ảnh nguồn giữ key/duration/pivot/canvas; loop xem hai vòng để có cuối→đầu.
Video runtime dùng file gốc byte-identical, chọn biên theo PTS, không retime.
Frame trước/sau, full canvas, actor và nearest có pan; ghi chú lưu riêng theo
name/hash rồi xuất JSON. Tự chạy playback không thay xác nhận con người.

**Chưa nhận xác nhận người xem cho bất kỳ clip mới nào.** Mỗi clip cần xác nhận
xem hết ở 1× và 0.25×, nhịp/độ đọc, PTS nghi vấn và nhận xét cụ thể.
Chưa đóng audit, chưa tích hợp báo cáo như audit hoàn tất và chưa dọn media.
Ngoài playback, sáu đoạn giữ lại có 1.060/1.054, đã kiểm 339/338, còn 721/716; ba clip READY mới có 806/803, còn frame/cặp chưa xem theo bảng bên dưới. Chi tiết bị che và xác nhận recovery bằng hình vẫn mở. Không tính ba đoạn lịch sử đã thay thế vào tồn nghiệm thu R. Đây là công việc agent còn lại, không quy cho người xem.

Giữ `E:/CodexTemp/TypingHero/pixel-audit-evidence-20261027` (media/build/snapshot/
preview/driver/server) và worktree `D:/Projects/TypingHero-worktrees/pixel-audit-evidence`,
branch `micharel09/pixel-audit-evidence`. Server chỉ bind localhost 8239,
PID ghi tại `review-server.pid`. Điều kiện dọn: đã review đủ, phản hồi người xem
được ghi, báo cáo commit/push/tích hợp và xác minh remote. Không dọn checkout
gốc hoặc tài nguyên có từ trước. A1 tiếp tục **lỗi xác nhận**, sửa ở task riêng.

### Ledger runtime đầu đoạn — phần đã trực tiếp xem

`normal-idle-parry-hurt-slayer`, hash `combat.webm` ở receipt capture-5 phía trên.
Đã mở riêng F0–F338 tại full 1280×720; F0–F99 dùng actor nearest 2400×1400,
F100–F185 dùng pan (440,260)–(860,500) nearest 1680×960, đủ đầu/chân/vũ khí,
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
| F20 | 1.520000 / 0.015000 | Parry/phản kích đang hồi: player còn thấp, kiếm kéo sau trái; skeleton kiếm chéo dựng, grip liền. Hạt cam nằm trái actor, chân/bóng giữ gạch. | Từ F19: skeleton giữ đứng; player chưa thoát pose thấp, +30 mờ thêm. | chưa kiểm chứng |
| F21 | 1.535000 / 0.055000 | Giữ pose thấp/kiếm sau trái; skeleton tay sát chuôi, hai chân đủ trong canvas và crop; countdown vẫn ở khay. | Từ F20: hold 15ms giữ vị trí thân, sàn và grip; không bỏ frame encode. | chưa kiểm chứng |
| F22 | 1.590000 / 0.076000 | Player giữ kiếm thấp sau thân, skeleton đứng với kiếm chéo; hạt cam giảm bên trái, không che grip tại frame này. | Từ F21: hai actor giữ landmark; vệt +30 gần hết. | chưa kiểm chứng |
| F23 | 1.666000 / 0.045000 | Skeleton xoay thân/đầu sang phải, kiếm chéo thấp hơn qua ngực; tay vẫn sát chuôi. Player còn thấp, chân giữ mốc. | Từ F22: đổi cel skeleton nhưng bàn chân trụ phải và mốc sàn còn cùng vùng; chưa thấy vật rời thêm. | chưa kiểm chứng |
| F24 | 1.711000 / 0.092000 | Skeleton giữ thân xoay và kiếm qua ngực; player vẫn cúi, kiếm thấp sau trái; đầu/chân không cắt khung. | Từ F23: hold tư thế, hạt cam tiếp tục tản; không thấy đổi sorting actor/sàn. | chưa kiểm chứng |
| F25 | 1.803000 / 0.032000 | Skeleton trở lại thân thẳng với kiếm chéo lên trái; player giữ thấp. Grip skeleton nối chuôi và tay; bóng dưới chân. | Từ F24: skeleton hồi kiếm/tay; camera và sàn không nhảy rõ. | chưa kiểm chứng |
| F26 | 1.835000 / 0.093000 | Hai actor giữ pose của F25: player cúi với kiếm kéo sau, skeleton đứng/kiếm chéo; đuốc đổi sáng. | Từ F25: hold 32ms được giữ; phần actor không trượt theo nhấp nháy đuốc. | chưa kiểm chứng |
| F27 | 1.928000 / 0.017000 | Player nâng thân và kiếm lên sau vai trái, khăn sáng cong sau đầu; skeleton giữ kiếm chéo. Hai chân player vẫn trên gạch cũ. | Từ F26: bắt đầu hồi thân khỏi pose thấp; đường chuôi còn liền tay, không có flash che. | chưa kiểm chứng |
| F28 | 1.945000 / 0.058000 | Player gần đứng, cánh tay/kiếm thu sau trái, vài mảnh sáng của đường kiếm còn sau tay; skeleton idle, grip nhìn được. | Từ F27: 17ms vẫn giữ bước hồi; các mảnh sáng đi cùng quỹ đạo kiếm, không tự gọi là pixel rác. | chưa kiểm chứng |
| F29 | 2.003000 / 0.054000 | Player đứng thẳng, kiếm không còn kéo dài sát sàn; khăn ngang trái, tay hạ gần hông. Skeleton kiếm chéo lên trái. | Từ F28: vệt kiếm sau tay hết; thân/đầu về tư thế đứng, đáy chân giữ landmark. | chưa kiểm chứng |
| F30 | 2.057000 / 0.069000 | Player đứng/khăn hạ phía trái; tay cạnh hông và chân đầy đủ; skeleton đầu hơi nghiêng, giữ kiếm. | Từ F29: trở lại nhịp idle; không thấy dư cung/afterimage trong khoảng actor. | chưa kiểm chứng |
| F31 | 2.126000 / 0.074000 | Player idle với khăn cong sau vai; skeleton giữ đầu/ngực và kiếm chéo; chân/bóng hai actor không bị khay che. | Từ F30: khăn đổi cel lên nhẹ; sàn, grip skeleton và khay giữ vị trí. | chưa kiểm chứng |
| F32 | 2.200000 / 0.018000 | Player idle, khăn trải trái; skeleton hạ đầu/ngực nhẹ nhưng chuôi vẫn gắn tay. Không có flash/cung. | Từ F31: dao động idle nhỏ, không thấy trượt chân so với mạch gạch. | chưa kiểm chứng |
| F33 | 2.218000 / 0.043000 | Hai actor giữ cel gần F32, player khăn trái và skeleton kiếm chéo; đầu/chân/vũ khí trong khung; countdown tiếp tục giảm. | Từ F32: hold 18ms giữ mốc sàn và sorting. | chưa kiểm chứng |
| F34 | 2.261000 / 0.082000 | Idle: khăn cong sau vai trái, hai actor đứng; kiếm skeleton chéo/chuôi liền tay, bóng và mạch sàn nguyên vị trí. | Từ F33: giữ pose, chỉ đổi nhịp khăn/ánh đuốc; không thấy jump camera. | chưa kiểm chứng |
| F35 | 2.343000 / 0.022000 | Idle: khăn cuộn cao hơn thành vòng sau đầu player; skeleton đứng thẳng, kiếm chéo và chân đủ trong crop. | Từ F34: khăn nâng, chân/grip không nhảy cùng khăn. | chưa kiểm chứng |
| F36 | 2.365000 / 0.036000 | Idle: mặt player hướng trước hơn, khăn hạ nhẹ; skeleton giữ chuôi đỏ/kiếm ngà, hai bóng vẫn dưới chân. | Từ F35: đổi cel đầu/tay nhỏ; không xuất hiện cung hoặc flash. | chưa kiểm chứng |
| F37 | 2.401000 / 0.088000 | Idle: player nghiêng đầu, khăn lượn trái; skeleton giữ đầu/ngực và lưỡi kiếm chéo; không cắt đầu/chân. | Từ F36: dao động phần trên; đáy chân và mạch gạch vẫn cùng vùng. | chưa kiểm chứng |
| F38 | 2.489000 / 0.022000 | Idle: khăn player ngắn, đầu nâng; skeleton kiếm chéo lên trái, tay bám chuôi; khay còn rõ, countdown gần hết. | Từ F37: scarf đổi silhouette; sorting actor/sàn giữ. | chưa kiểm chứng |
| F39 | 2.511000 / 0.046000 | Idle: khăn player trải ngang trái, tay cạnh hông; skeleton đầu ngực thấp nhẹ, lưỡi kiếm không tách tay. | Từ F38: hạ/nâng idle nhỏ, không có dịch toàn bộ actor so sàn. | chưa kiểm chứng |
| F40 | 2.557000 / 0.079000 | Idle: hai actor giữ đứng; từ đầu khay biến mất và countdown đầy lại, find/give còn giữa/phải. | Từ F39: đổi từ/đầy countdown trong UI; chân và body không chạy theo khay. | chưa kiểm chứng |
| F41 | 2.636000 / 0.020000 | Idle: khăn player cong trái, skeleton kiếm chéo; khay hiện find/give/true đầy đủ, viền/padding không cắt chữ. | Từ F40: nhãn mới vào vị trí đầu, body/sàn giữ landmark. | chưa kiểm chứng |
| F42 | 2.656000 / 0.043000 | Idle: player khăn cong, skeleton tay nối chuôi, hai chân hiện trọn trên sàn xanh; ba từ nằm trong khay. | Từ F41: hold ngắn giữ actor, nhãn trượt nhẹ theo rail; không nhầm thành camera drift. | chưa kiểm chứng |
| F43 | 2.699000 / 0.018000 | Idle: player scarf trái/ngực đứng, skeleton kiếm chéo/đầu thẳng; không flash che. | Từ F42: settle nhãn find/give/true, đáy chân và sorting giữ. | chưa kiểm chứng |
| F44 | 2.717000 / 0.054000 | Idle: hai actor giữ pose, player scarf trái, skeleton grip đỏ dưới lưỡi; countdown giảm trong khay. | Từ F43: hold 18ms; không thấy vũ khí rời hoặc nhảy anchor. | chưa kiểm chứng |
| F45 | 2.771000 / 0.156000 | Idle: khăn player kéo xuống trái, skeleton đứng với kiếm chéo, chân phải ở cùng mạch gạch. | Từ F44: khăn hạ, các nhãn và khung HUD giữ bố cục. | chưa kiểm chứng |
| F46 | 2.927000 / 0.038000 | Idle: mặt player xoay trước nhẹ và khăn vòng phía trái; skeleton đầu/hai chân đủ khung, bóng vẫn dưới chân. | Từ F45: hold dài vẫn được xem riêng; sàn/cột và grip không bật vị trí. | chưa kiểm chứng |
| F47 | 2.965000 / 0.097000 | Idle: khăn player duỗi ngắn, skeleton giữ kiếm; countdown tiếp tục giảm, không có afterimage. | Từ F46: khăn đổi silhouette; chân player vẫn cùng vùng sáng. | chưa kiểm chứng |
| F48 | 3.062000 / 0.013000 | Idle: khăn player uốn lên trái; skeleton đầu hơi thấp, tay bám chuôi đỏ; chân/bóng không bị khay che. | Từ F47: dao động thân nhỏ, không thấy sàn dịch theo actor. | chưa kiểm chứng |
| F49 | 3.075000 / 0.087000 | Idle: cả hai giữ dáng, kiếm skeleton chéo; player đủ mặt/tóc/chân, không có spark/cung. | Từ F48: hold 13ms còn cùng grip và mốc chân; không bỏ frame lặp. | chưa kiểm chứng |
| F50 | 3.162000 / 0.028000 | Idle: khăn player cong trái, tay sát hông; skeleton giữ kiếm chéo, body và bóng nằm trên cùng gạch. | Từ F49: nhịp khăn thay, camera/cột/sàn không nhảy rõ. | chưa kiểm chứng |
| F51 | 3.190000 / 0.035000 | Idle: player mặt trước, khăn hạ; skeleton đầu/ngực và chuôi không bị flash che; toàn actor trong khung. | Từ F50: hold 28ms giữ bố cục và anchor. | chưa kiểm chứng |
| F52 | 3.225000 / 0.036000 | Idle: khăn player kéo xuống trái, tay sát hông; skeleton giữ kiếm chéo và grip liền; chân/bóng trên mạch gạch cũ. | Từ F51: phần trên dao động nhỏ, không thấy nhảy camera. | chưa kiểm chứng |
| F53 | 3.261000 / 0.059000 | Idle: player đầu nghiêng, khăn trải sau trái; skeleton thẳng, chuôi đỏ dưới lưỡi ngà; toàn actor trong crop. | Từ F52: hold tư thế, đuốc đổi sáng nhưng anchor chân giữ. | chưa kiểm chứng |
| F54 | 3.320000 / 0.044000 | Idle: khăn player và hai chân giữ dáng; skeleton kiếm chéo, tay không tách chuôi; không có flash/afterimage. | Từ F53: giữ silhouette gần cùng cel; countdown giảm trong khay, không kéo world. | chưa kiểm chứng |
| F55 | 3.364000 / 0.025000 | Idle: khăn vòng cao sau player; skeleton đầu/ngực đứng và kiếm còn chéo; bóng ở ngay dưới chân. | Từ F54: khăn nâng/thu; không thấy sorting actor/sàn đổi. | chưa kiểm chứng |
| F56 | 3.389000 / 0.063000 | Idle: player hướng mặt trước hơn, khăn cuộn trái; skeleton chuôi/kiếm liền tay; đầu/chân không cắt khung. | Từ F55: 25ms vẫn giữ mốc sàn, chỉ phần trên đổi nhẹ. | chưa kiểm chứng |
| F57 | 3.452000 / 0.018000 | Idle: khăn player cong trái, tay cạnh hông; skeleton giữ kiếm chéo, hai chân đủ và bóng giữ gạch. | Từ F56: hold tư thế. | chưa kiểm chứng |
| F58 | 3.470000 / 0.079000 | Khăn player cong trái, mặt trước; skeleton giữ kiếm chéo và chuôi liền tay; toàn đầu/chân trong khung. | Từ F57: hold 18ms vẫn giữ sàn và bóng; chưa có cue ở frame này. | chưa kiểm chứng |
| F59 | 3.549000 / 0.015000 | SPACE hiện viền trắng trên skeleton; player idle khăn trải trái; skeleton giữ kiếm chéo, grip/chân nhìn được. | Từ F58: cue xuất hiện độc lập phía trên, không che đầu/tay; actor giữ vị trí. | chưa kiểm chứng |
| F60 | 3.564000 / 0.063000 | Cue SPACE còn sáng; player khăn ngang trái, skeleton đầu/ngực đứng và kiếm giữ chéo; bóng bám mạch gạch. | Từ F59: hold 15ms, không có flash hoặc dời khay theo cue. | chưa kiểm chứng |
| F61 | 3.627000 / 0.054000 | Cue/chuẩn bị giữ: player mặt trước, khăn cong; skeleton tay bám chuôi, hai chân đủ; HUD và khay ổn định. | Từ F60: dao động khăn, không thấy camera/sàn nhảy. | chưa kiểm chứng |
| F62 | 3.681000 / 0.017000 | SPACE viền sáng nằm trên đầu; player khăn trải trái, skeleton giữ kiếm ngà/chuôi đỏ; không bị crop cắt vũ khí. | Từ F61: hold pose chuẩn bị, chân/bóng không trượt ra khỏi gạch. | chưa kiểm chứng |
| F63 | 3.698000 / 0.075000 | Cue còn hiện; mặt player quay trước, skeleton đứng kiếm chéo, tay không tách chuôi; ba từ trong khay đọc được. | Từ F62: hold 17ms vẫn xem riêng; không coi thành vòng idle đã kết thúc. | chưa kiểm chứng |
| F64 | 3.773000 / 0.101000 | Player khăn hạ trái, skeleton giữ kiếm chéo dưới SPACE; đầu/chân/vũ khí còn đủ khung, không có vệt mới. | Từ F63: cue giữ, nhịp khăn đổi; nền/cột giữ bố cục. | chưa kiểm chứng |
| F65 | 3.874000 / 0.022000 | Player khăn cuộn sau đầu, skeleton giữ thân/kiếm dưới cue; hai chân vẫn đứng ở cùng vùng gạch sáng. | Từ F64: 101ms giữ chuẩn bị; khăn thay silhouette nhưng grip skeleton liền. | chưa kiểm chứng |
| F66 | 3.896000 / 0.045000 | Player mặt trước và scarf vòng trái; skeleton đầu/ngực đứng, kiếm chéo; SPACE không chạm đầu ở full canvas. | Từ F65: hold 22ms vẫn giữ anchor và lớp actor/sàn. | chưa kiểm chứng |
| F67 | 3.941000 / 0.094000 | Cue vẫn sáng; player khăn trái/tay hông, skeleton giữ chuôi đỏ và hai chân; không có spark che. | Từ F66: dao động nhỏ phần trên; mạch sàn không đổi với chân. | chưa kiểm chứng |
| F68 | 4.035000 / 0.011000 | Player khăn ngắn, skeleton thân đứng/kiếm chéo; SPACE giữ phía trên; không cắt đầu/chân trong full/nearest. | Từ F67: khăn hạ; countdown giảm, toàn camera không dịch rõ. | chưa kiểm chứng |
| F69 | 4.046000 / 0.039000 | Cue/chuẩn bị vẫn giữ; player khăn ngắn trái và tay sát hông; skeleton kiếm chéo, grip nhìn được. | Từ F68: hold 11ms được giữ. | chưa kiểm chứng |
| F70 | 4.085000 / 0.077000 | Skeleton đưa kiếm dựng sát mặt/ngực dưới cue SPACE, đầu và vai xoay; player vẫn idle/khăn trái. Chân trụ giữ gạch. | Từ F69: bắt đầu nâng/thu kiếm, chuôi vẫn nối tay; không có flash che. | chưa kiểm chứng |
| F71 | 4.162000 / 0.028000 | Skeleton giữ kiếm dựng cạnh mặt, cẳng tay co gần ngực; player khăn cong trái và đứng đủ hai chân. | Từ F70: hold chuẩn bị, mốc chân/bóng và camera giữ. | chưa kiểm chứng |
| F72 | 4.190000 / 0.046000 | Skeleton còn kiếm dựng và tay co, SPACE trên đầu; player mặt trước/khăn trái; cả hai đủ trong crop. | Từ F71: hold 28ms giữ vị trí; không thấy tách vũ khí khỏi tay. | chưa kiểm chứng |
| F73 | 4.236000 / 0.079000 | Skeleton kéo kiếm chéo ra sau vai phải, thân xoay; player idle, khăn ngang trái. Cán/chuôi đi cùng tay. | Từ F72: kiếm chuyển từ dựng sang qua vai; chân phải còn trên vùng gạch cũ. | chưa kiểm chứng |
| F74 | 4.315000 / 0.019000 | Skeleton giữ kiếm sau vai và lùi thân, SPACE đổi nền ngà; player khăn cong, tay cạnh hông; không cắt đầu/vũ khí. | Từ F73: cue đổi màu và thân dồn nhẹ, không phải toàn nền/camera dịch. | chưa kiểm chứng |
| F75 | 4.334000 / 0.115000 | Skeleton giữ đòn sau vai, tay sát chuôi; player idle/khăn trái; viền cue ngà không che đầu, chân/bóng còn đủ. | Từ F74: hold 19ms giữ chuẩn bị. | chưa kiểm chứng |
| F76 | 4.449000 / 0.019000 | Player hạ thân và dựng kiếm trước mặt, khăn vòng sau; skeleton giữ kiếm sau vai, SPACE chuyển xanh. Đầu/chân/vũ khí đủ trong full và crop. | Từ F75: player vào parry; đáy chân vẫn ở vùng gạch cũ, không thấy grip rời chuôi. | chưa kiểm chứng |
| F77 | 4.468000 / 0.079000 | Giữ parry thấp, lưỡi player dựng; skeleton giữ kiếm sau vai dưới SPACE xanh; bóng vẫn sát chân. | Từ F76: hold 19ms được giữ; countdown giảm, không kéo camera/actor. | chưa kiểm chứng |
| F78 | 4.547000 / 0.081000 | Skeleton lao tay/kiếm ngang tới kiếm dựng player; flash trắng/vàng và tia cam tại điểm chạm che một phần hai lưỡi. +30 hiện, điểm 60, HP player còn đầy. | Từ F77: đòn tới/parry; nền cùng actor đổi vị trí nhẹ trong rung, không quy thành trượt chân. Vùng tiếp xúc bị che chưa chứng nhận grip. | chưa kiểm chứng |
| F79 | 4.628000 / 0.046000 | Player cúi phản kích, kiếm quét thấp ra sau trái, khăn vòng cao; skeleton duỗi tay ngang. Sao sáng lớn che điểm giao kiếm, chân và sàn vẫn đọc được. | Từ F78: kiếm player từ dựng sang sát sàn; camera rung và sao nở, không xuất hiện vũ khí rời ngoài quỹ đạo nhìn thấy. | chưa kiểm chứng |
| F80 | 4.674000 / 0.112000 | Player giữ thấp với kiếm sau trái; skeleton giữ tay ngang, flash thu/cam hóa, +30 còn trên đầu. Grip tại flash chưa nhìn được. | Từ F79: đỉnh flash giảm và tia đỏ tản; chân/bóng cùng landmark sàn sau rung. | chưa kiểm chứng |
| F81 | 4.786000 / 0.017000 | Giữ phản kích thấp; skeleton tay ngang, sao còn nhỏ quanh kiếm, hạt đỏ xa hơn về trái; khay đủ ba từ. | Từ F80: flash thu tiếp, camera gần bố cục trước; đầu/chân không cắt crop. | chưa kiểm chứng |
| F82 | 4.803000 / 0.081000 | Player giữ kiếm sát sàn bên trái, skeleton còn tay duỗi; flash cam che đoạn lưỡi gần tay, +30 mờ hơn. | Từ F81: hold 17ms, hạt cam tản; chân và bóng giữ mạch gạch. | chưa kiểm chứng |
| F83 | 4.884000 / 0.040000 | Skeleton co tay, đưa kiếm về chéo lên trái, thân dựng hơn; tia cam nhỏ còn trên lưỡi. Player còn pose thấp, scarf cong sau. | Từ F82: hồi kiếm skeleton, chuôi đỏ bắt đầu lộ; chân trụ cùng vùng gạch, không có nhảy toàn camera rõ. | chưa kiểm chứng |
| F84 | 4.924000 / 0.080000 | Skeleton giữ kiếm chéo, spark giữa hai actor đã hết, chuôi đỏ/tay nhìn liền; player thấp và kiếm kéo sau trái, hạt cam rải xa. | Từ F83: vùng grip hiện rõ khi flash tắt; không dùng frame này để chứng nhận vùng bị che của F78–F83. | chưa kiểm chứng |
| F85 | 5.004000 / 0.044000 | Skeleton hạ tay về gần hông, kiếm nghiêng dựng lên trái; player vẫn cúi, kiếm gần sàn; +30 rất mờ. | Từ F84: hồi tay skeleton, chân/bóng giữ vùng sáng; lưỡi đi cùng chuôi. | chưa kiểm chứng |
| F86 | 5.048000 / 0.047000 | Hai actor giữ pose hồi: player thấp/kiếm sau trái, skeleton đứng/kiếm dựng chéo; hạt cam thưa, đầu/chân đủ khung. | Từ F85: hold 44ms, +30 mờ tiếp; sorting và mạch sàn giữ. | chưa kiểm chứng |
| F87 | 5.095000 / 0.051000 | Pose thấp player còn giữ, skeleton kiếm dựng chéo và tay ở hông; +30 đã hết, còn vài hạt đỏ trái sàn. | Từ F86: hold hồi, hạt giảm; không còn flash giữa actor. | chưa kiểm chứng |
| F88 | 5.146000 / 0.043000 | Skeleton xoay đầu/thân sang phải, kiếm chéo thấp qua ngực; player còn thấp, khăn vòng cao. Tay vẫn sát chuôi đỏ. | Từ F87: đổi cel hồi skeleton, chân trụ phải giữ gạch; không lẫn thay silhouette với camera drift. | chưa kiểm chứng |
| F89 | 5.189000 / 0.069000 | Skeleton giữ thân xoay và kiếm qua ngực, player giữ kiếm thấp sau trái; hai bóng nằm dưới chân, countdown gần hết. | Từ F88: hold 43ms, không thấy grip/kiếm bật rời hoặc đảo sorting. | chưa kiểm chứng |
| F90 | 5.258000 / 0.042000 | Skeleton trở lại đứng trước, kiếm dựng chéo lên trái/tay gần hông; player vẫn trong hồi phản kích thấp. Hạt cam còn rất ít bên trái. | Từ F89: hồi hướng thân/kiếm skeleton; mốc chân và sàn không nhảy. Player chưa về idle tại frame này. | chưa kiểm chứng |
| F91 | 5.300000 / 0.017000 | Player giữ phản kích thấp/kiếm sau trái; skeleton đứng, tay tại hông và kiếm dựng chéo, còn vài hạt cam sát sàn trái. | Từ F90: hold hồi, bề mặt sàn và bóng không đổi vị trí rõ. | chưa kiểm chứng |
| F92 | 5.317000 / 0.032000 | Player còn thấp, khăn vòng sau đầu; skeleton kiếm dựng chéo, grip đỏ liền tay; đầu/chân/vũ khí đều trong crop. | Từ F91: hold 17ms, countdown giảm, không nhảy anchor. | chưa kiểm chứng |
| F93 | 5.349000 / 0.016000 | Player nâng thân và kéo kiếm ngang sau vai trái, scarf vòng cao; skeleton giữ đứng/kiếm chéo. | Từ F92: bắt đầu đứng dậy; chân trụ vẫn ở gạch cũ, lưỡi nối tay. | chưa kiểm chứng |
| F94 | 5.365000 / 0.075000 | Giữ bước hồi đứng dậy: kiếm player ngang sau trái, mặt nghiêng; skeleton đứng/kiếm chéo, không flash che grip. | Từ F93: hold 16ms, nền/cột và chân giữ landmark. | chưa kiểm chứng |
| F95 | 5.440000 / 0.050000 | Player đứng gần thẳng, kiếm thu về sau vai, vài mảnh sáng nằm dọc đường lưỡi sau tay; chữ fi của find xanh. | Từ F94: thân nâng/kiếm thu; các mảnh sáng đi theo lưỡi, chưa coi là pixel rác ngoài quỹ đạo. | chưa kiểm chứng |
| F96 | 5.490000 / 0.099000 | Player về dáng idle, tay hạ cạnh hông, kiếm kéo dài không còn; khăn ra trái, fin trong khay xanh. Skeleton giữ kiếm chéo. | Từ F95: kết hồi phản kích về đứng; mảnh sáng sau tay hết, chân giữ gạch. | chưa kiểm chứng |
| F97 | 5.589000 / 0.060000 | Player lao thấp chém sang phải, chuôi xanh/đỏ sát tay, cung ngà lớn và sao vàng/cam che ngực/tay skeleton; +10, điểm 70. | Từ F96: đòn thường bắt đầu; khay chuyển give/true/any và countdown đầy, nền cùng actor rung. Grip enemy sau cung chưa thấy. | chưa kiểm chứng |
| F98 | 5.649000 / 0.034000 | Cung ngà còn trước player và che skeleton, tia cam tản bên phải; player giữ thấp, tay/chuôi còn nhìn liền. | Từ F97: sao thu, cung giữ; UI settle trái, không nhầm rail UI với trượt camera. | chưa kiểm chứng |
| F99 | 5.683000 / 0.042000 | Player giữ chém thấp, cung ngà trước thân; skeleton đầu/rib/chân còn lộ nhưng tay/kiếm vẫn bị cung che; +10 còn rõ. | Từ F98: spark lớn hết, mốc sàn trở gần trước rung; chưa chứng nhận grip phía sau cung. | chưa kiểm chứng |
| F100 | 5.725000 / 0.064000 | Giữ cung lớn và chém thấp; lưỡi player/chuôi xanh nằm trước tay, skeleton đứng sau cung; toàn actor đủ full và pan4×. | Từ F99: hold 42ms, không có vật rời mới ngoài cung; chân/bóng bám vùng sáng. | chưa kiểm chứng |
| F101 | 5.789000 / 0.049000 | Skeleton sáng/nhạt hơn và nghiêng thân sang phải trong Hurt, đầu xoay; cung còn che phần bụng/tay. Player thấp, khăn ra sau trái. | Từ F100: skeleton vào phản ứng trúng đòn; nền/actor cùng dịch nhẹ, không kết luận trượt chân từ flash. | chưa kiểm chứng |
| F102 | 5.838000 / 0.020000 | Cung vỡ thành dải ngà quanh thân/chân skeleton; lưỡi player nằm thấp sang phải, chuôi còn liền tay; +10 trên đầu. | Từ F101: cung tan, skeleton hồi hướng thân; vùng che giảm nhưng không thay bằng chứng cel không VFX. | chưa kiểm chứng |
| F103 | 5.858000 / 0.084000 | Cung hết: skeleton đứng lại, kiếm chéo gần dựng, chuôi đỏ/tay rõ; player còn chém thấp với kiếm nằm trước bên phải. | Từ F102: hồi skeleton và hết cung; sàn/bóng giữ landmark, player chưa về idle. | chưa kiểm chứng |
| F104 | 5.942000 / 0.046000 | Hồi đòn thường: player nâng thân/kiếm thấp trước phải, scarf rủ sau; skeleton idle, grip đỏ hiện rõ, +10 mờ dần. | Từ F103: kiếm player thu gần tay, chân/bóng giữ mạch gạch, cung đã hết. | chưa kiểm chứng |
| F105 | 5.988000 / 0.043000 | Player gần đứng, lưỡi thu thành các mảnh ngà/tím trước tay; skeleton giữ kiếm chéo, hai actor đủ trong pan4×. | Từ F104: thân nâng tiếp, mảnh lưỡi theo quỹ đạo thu; không có flash che grip. | chưa kiểm chứng |
| F106 | 6.031000 / 0.051000 | Giữ bước hồi đứng, mảnh lưỡi trước tay player; skeleton đứng/kiếm chéo và +10 nhạt. | Từ F105: hold hồi, mốc chân/cột/sàn giữ; countdown giảm. | chưa kiểm chứng |
| F107 | 6.082000 / 0.071000 | Player còn dáng hồi với mảnh kiếm trước phải, scarf sau vai; skeleton grip liền, không có spark. | Từ F106: hold, +10 mờ hơn; không đổi sorting hoặc cắt chân. | chưa kiểm chứng |
| F108 | 6.153000 / 0.060000 | Player đứng thẳng hơn, tay về hông, mảnh ngà/tím còn trôi trước đùi phải; skeleton đứng, kiếm chéo. | Từ F107: kiếm thu/biến mất dần, chân trụ cùng vùng sáng, không kết luận mảnh sprite là lỗi khi chưa đối chiếu cel. | chưa kiểm chứng |
| F109 | 6.213000 / 0.041000 | Player đứng với tay hông, chỉ còn vài mảnh lưỡi phía phải; scarf lượn trái. Skeleton giữ grip/kiếm chéo. | Từ F108: mảnh lưỡi thưa hơn, +10 đã hết; anchor chân và nền giữ. | chưa kiểm chứng |
| F110 | 6.254000 / 0.053000 | Player idle, các mảnh lưỡi đã hết; scarf lượn ngắn sau trái, skeleton giữ kiếm; gi trong give xanh. | Từ F109: kết hồi về idle, không còn residue giữa actor; HUD/khay ổn định. | chưa kiểm chứng |
| F111 | 6.307000 / 0.076000 | Idle: player hơi quay mặt trước, scarf lượn trái, skeleton đầu/ngực đứng, grip đỏ rõ. | Từ F110: scarf/đầu dao động nhẹ, chân/bóng giữ mạch sàn. | chưa kiểm chứng |
| F112 | 6.383000 / 0.063000 | Idle: scarf player hạ trái, skeleton thân nhún nhẹ và kiếm chéo, chữ giv xanh trong khay. | Từ F111: thay cel nhỏ phần trên, không trượt chân hoặc nhảy camera rõ. | chưa kiểm chứng |
| F113 | 6.446000 / 0.112000 | Đòn thường tiếp: player thấp với chuôi sát tay, cung ngà/sao trắng vàng lớn che tay/ngực skeleton, điểm 80/+10. | Từ F112: attack từ idle, nền cùng actor rung; khay trượt true/any/write, chữ write đang vào biên rail, chưa đánh giá như chữ tĩnh. | chưa kiểm chứng |
| F114 | 6.558000 / 0.057000 | Player giữ chém thấp, cung lớn trước skeleton; flash đã hết, đầu/rib/chân enemy lộ nhưng grip còn bị cung che. | Từ F113: sao tắt, khay settle đủ true/any/write; floor/cột gần bố cục trước rung. | chưa kiểm chứng |
| F115 | 6.615000 / 0.019000 | Giữ chém thấp/cung lớn, chuôi player xanh liền tay; skeleton đứng sau cung, +10 rõ. | Từ F114: hold, không có pixel rời mới ngoài vùng cung; chân/bóng giữ vùng sáng. | chưa kiểm chứng |
| F116 | 6.634000 / 0.084000 | Skeleton sáng/nhạt và nghiêng phải trong Hurt; cung còn che bụng/tay, player thấp/kiếm trước phải. | Từ F115: enemy vào cel Hurt, mốc chân cùng gạch; phần bị cung che chưa chứng nhận. | chưa kiểm chứng |
| F117 | 6.718000 / 0.053000 | Cung vỡ thành dải ngà, skeleton còn nhạt/thoái thân; player giữ kiếm thấp, khăn vòng sau; +10 trên đầu. | Từ F116: cung tan dần, không bỏ frame 53ms; đầu/chân/vũ khí nhìn thấy đủ crop. | chưa kiểm chứng |
| F118 | 6.771000 / 0.106000 | Cung hết, skeleton đứng lại/kiếm gần dựng và grip đỏ liền; player vẫn thấp, lưỡi nằm trước phải. | Từ F117: skeleton hồi và hết flash/cung, camera giữ sàn; chưa kết hồi player. | chưa kiểm chứng |
| F119 | 6.877000 / 0.019000 | Player nâng thân/thu kiếm thấp, scarf rủ trái; skeleton giữ dáng và kiếm chéo, +10 mờ. | Từ F118: bắt đầu hồi player, chân/bóng vẫn ở mạch gạch cũ. | chưa kiểm chứng |
| F120 | 6.896000 / 0.051000 | Player gần đứng với mảnh lưỡi ngà/tím trước phải; skeleton đứng/kiếm chéo, +10 còn nhạt. | Từ F119: kiếm thu, không có VFX che tay; không quy mảnh lưỡi thành lỗi trước đối chiếu cel. | chưa kiểm chứng |
| F121 | 6.947000 / 0.095000 | Giữ pose hồi gần đứng và các mảnh lưỡi player; skeleton grip đỏ rõ, hai bóng dưới chân. | Từ F120: hold, +10 mờ tiếp, sàn/cột không nhảy theo đuốc. | chưa kiểm chứng |
| F122 | 7.042000 / 0.053000 | Player đứng thẳng/tay hông, mảnh lưỡi còn vài đoạn trước đùi; scarf cong sau, skeleton idle. | Từ F121: mảnh lưỡi thưa, +10 đã hết; khung HUD giữ vị trí. | chưa kiểm chứng |
| F123 | 7.095000 / 0.065000 | Player idle với vài mảnh cuối phía phải, scarf lượn trái; skeleton đứng/kiếm chéo, grip hiện rõ. | Từ F122: hold ngắn giữ chân/sàn, không đảo lớp actor. | chưa kiểm chứng |
| F124 | 7.160000 / 0.052000 | Player idle và mảnh lưỡi đã hết; SPACE viền trắng xuất hiện trên enemy; t của true xanh. | Từ F123: cue hiện riêng phía trên, không che đầu; hồi player kết thúc về idle. | chưa kiểm chứng |
| F125 | 7.212000 / 0.130000 | Cue SPACE còn trắng, player scarf ngắn/trước mặt, skeleton giữ kiếm chéo; tr trong khay xanh. | Từ F124: hold, cue và HUD giữ chỗ; chân/bóng tiếp tục trên cùng gạch. | chưa kiểm chứng |
| F126 | 7.342000 / 0.089000 | Đòn thường: player thấp/chuôi trước phải, cung và sao trắng/vàng che tay/ngực skeleton; SPACE còn trắng phía trên, điểm90/+10. | Từ F125: từ idle vào đòn, nền/actor cùng rung, khay bỏ true và chuyển any/write; grip enemy sau cung chưa thấy. | chưa kiểm chứng |
| F127 | 7.431000 / 0.014000 | Player giữ chém thấp; cung lớn che enemy, sao thu thành tia cam phải; SPACE trắng, any/write/up đã vào khay. | Từ F126: flash thu và UI settle; chân/bóng cùng landmark sau rung. | chưa kiểm chứng |
| F128 | 7.445000 / 0.050000 | Cung còn trước player và che tay skeleton; spark hết, +10 trên đầu, SPACE vẫn trắng; hai actor đủ khung. | Từ F127: hết tia cam lớn, không có vật rời mới ngoài cung; nền trở gần bố cục trước. | chưa kiểm chứng |
| F129 | 7.495000 / 0.099000 | Giữ chém thấp/cung ngà, skeleton đầu/rib/chân lộ nhưng grip bị che; player tay/chuôi xanh liền. | Từ F128: hold18ms giữ silhouette/anchor, không bỏ frame lặp. | chưa kiểm chứng |
| F130 | 7.594000 / 0.038000 | Cung vỡ thành dải ngà quanh thân/chân enemy; player thấp/kiếm trước phải, skeleton kiếm chéo hiện dần. | Từ F129: cung tan, vùng tay enemy bắt đầu lộ; không coi là đã nhìn được cel bị che trước đó. | chưa kiểm chứng |
| F131 | 7.632000 / 0.016000 | Cung hết, skeleton đứng/kiếm chéo và grip đỏ rõ; player còn thấp, lưỡi trước phải, +10 còn. | Từ F130: hồi skeleton về đứng, không đảo sorting hay nhảy chân rõ. | chưa kiểm chứng |
| F132 | 7.648000 / 0.048000 | Player giữ thấp/kiếm trước phải, skeleton giữ đứng dưới SPACE trắng; bóng hai actor ở vùng gạch sáng. | Từ F131: hold16ms, countdown giảm; camera/sàn giữ. | chưa kiểm chứng |
| F133 | 7.696000 / 0.037000 | Player nâng thân/thu kiếm sát sàn, scarf rủ trái; skeleton đứng/kiếm chéo và +10 nhạt dần. | Từ F132: bắt đầu hồi player, đáy chân vẫn cùng gạch. | chưa kiểm chứng |
| F134 | 7.733000 / 0.081000 | Giữ bước hồi nâng thân, kiếm player thấp trước tay; skeleton grip rõ, cue không che đầu. | Từ F133: hold37ms, không nhảy anchor hoặc dời khay theo actor. | chưa kiểm chứng |
| F135 | 7.814000 / 0.072000 | Player gần đứng, các mảnh lưỡi ngà/tím trước phải; skeleton đứng/kiếm chéo, +10 rất mờ. | Từ F134: kiếm thu dần, không có cung/flash mới. | chưa kiểm chứng |
| F136 | 7.886000 / 0.075000 | Player đứng hơn với vài mảnh lưỡi cuối; skeleton thu kiếm dựng gần mặt/ngực bắt đầu chuẩn bị, SPACE trắng. | Từ F135: tay/kiếm skeleton đổi hướng cùng nhau; chân trụ giữ gạch. | chưa kiểm chứng |
| F137 | 7.961000 / 0.018000 | Skeleton đưa kiếm qua vai phải, thân xoay; player gần idle, còn mảnh cuối trước phải, khăn trái. | Từ F136: chuẩn bị đòn, đầu/chân/vũ khí đủ pan; nền/cột giữ. | chưa kiểm chứng |
| F138 | 7.979000 / 0.049000 | Player idle, các mảnh lưỡi đã hết; skeleton giữ kiếm sau vai dưới cue trắng. | Từ F137: kết hồi player về idle; chân/bóng giữ landmark. | chưa kiểm chứng |
| F139 | 8.028000 / 0.014000 | Skeleton lùi thân/kiếm sau vai phải, SPACE nền ngà; player idle. Mở thêm crop600×350 nearest4× để thấy đủ mũi kiếm ngoài pan x860. | Từ F138: cue đổi màu, kiếm/chuôi vẫn liền tay; không nhầm phần mũi bị crop hẹp thành mất vũ khí. | chưa kiểm chứng |
| F140 | 8.042000 / 0.070000 | Giữ chuẩn bị kiếm sau vai, cue ngà; player scarf ngắn trái. Đã mở crop rộng nearest4×, mũi kiếm đủ và không tách. | Từ F139: hold14ms, tay/chân và sàn giữ. | chưa kiểm chứng |
| F141 | 8.112000 / 0.085000 | Player scarf lượn trái, skeleton giữ sau vai dưới cue ngà; crop rộng nearest4× cho thấy đủ mũi kiếm phải. | Từ F140: dao động idle player; skeleton giữ đòn, không đổi sorting. | chưa kiểm chứng |
| F142 | 8.197000 / 0.049000 | Skeleton dồn thân sau/giữ kiếm, player idle; cue ngà, đầu/chân và mũi kiếm đã xem đủ bằng crop rộng nearest4×. | Từ F141: hold85ms, không thấy trượt chân theo nhấp nháy đuốc. | chưa kiểm chứng |
| F143 | 8.246000 / 0.076000 | Skeleton duỗi tay/kiếm ngang, cung trắng dài quét trước mặt player; HP player giảm và viền HP cam, cue biến mất. | Từ F142: đòn tới; cung che một phần tóc/mặt player, chưa thể chứng nhận chi tiết ở vùng đó. | chưa kiểm chứng |
| F144 | 8.322000 / 0.069000 | Player đỏ toàn thân trong Hurt, ngửa trái/chân trước duỗi; enemy giữ cung ngang và tay duỗi. Sàn/nền lệch cùng rung. | Từ F143: vào phản ứng trúng đòn, không suy màu albedo từ flash đỏ; đầu/chân vẫn đủ crop. | chưa kiểm chứng |
| F145 | 8.391000 / 0.050000 | Player còn Hurt ngửa trái, flash đỏ giảm để lộ mặt/áo; enemy giữ cung ngang trước đầu player, HP giữ mức sau hit. | Từ F144: màu đỏ nhạt dần, body giữ phản ứng; nền trở gần trước rung. | chưa kiểm chứng |
| F146 | 8.441000 / 0.060000 | Cung ngang thu thành dải ngắn dọc lưỡi enemy; player vẫn Hurt/đỏ nhẹ, chân trước duỗi, tay co sau. | Từ F145: cung tan, grip enemy đỏ và tay duỗi nhìn liền; không xuất hiện mảnh vũ khí rời. | chưa kiểm chứng |
| F147 | 8.501000 / 0.077000 | Player hết flash đỏ, còn ngửa Hurt với chân trước duỗi và khăn vòng trái; enemy kiếm ngang còn dải trắng ngắn. | Từ F146: màu trở bình thường, không coi sự đổi màu là đổi outfit/cel nguồn; chân giữ vùng gạch. | chưa kiểm chứng |
| F148 | 8.578000 / 0.060000 | Dải cung hết, skeleton tay/kiếm ngang rõ; player thu chân và dựng thân hơn trong hồi Hurt, khăn cong trái. | Từ F147: hồi phản ứng, không còn che đầu player bởi cung; landmark sàn giữ. | chưa kiểm chứng |
| F149 | 8.638000 / 0.081000 | Player giữ hồi Hurt, thân gần đứng/chân thu; skeleton co tay đưa kiếm chéo lên trái, grip đỏ liền. | Từ F148: enemy hồi kiếm, player chưa idle; sorting/bóng không đổi rõ. | chưa kiểm chứng |
| F150 | 8.719000 / 0.039000 | Player cúi thân trong bước cuối hồi Hurt, tay lên gần vai, skeleton đứng/kiếm chéo; không flash hay cung. | Từ F149: player từ ngửa sang cúi hồi, bàn chân giữ gạch; camera về bố cục ổn định. | chưa kiểm chứng |
| F151 | 8.758000 / 0.040000 | Player trở lại idle với tay hông/khăn trái, skeleton đứng/kiếm chéo, HP giữ mức sau hit. | Từ F150: kết Hurt về idle, không còn residue đỏ/cung trên người. | chưa kiểm chứng |
| F152 | 8.798000 / 0.077000 | Hai actor giữ idle: player khăn trái, enemy tay nối chuôi đỏ; đầu/chân đủ pan. | Từ F151: hold40ms, sàn/bóng/khay giữ vị trí. | chưa kiểm chứng |
| F153 | 8.875000 / 0.069000 | Player idle hơi quay mặt trước, scarf ngắn trái; skeleton đứng kiếm chéo, không flash. | Từ F152: dao động đầu/khăn, không nhảy chân/camera rõ. | chưa kiểm chứng |
| F154 | 8.944000 / 0.084000 | Skeleton xoay đầu/ngực sang phải và kiếm chéo thấp qua ngực; player idle scarf cong trái. | Từ F153: đổi cel idle skeleton, tay vẫn sát chuôi, chân trụ cùng mạch gạch. | chưa kiểm chứng |
| F155 | 9.028000 / 0.039000 | Giữ thân xoay skeleton/kiếm qua ngực, player đứng với scarf trái, countdown giảm trong khay. | Từ F154: hold84ms, chân/bóng và sorting giữ; không có pixel rời mới. | chưa kiểm chứng |
| F156 | 9.067000 / 0.089000 | Skeleton quay về trước/kiếm chéo dựng; player idle, khăn vòng lên trái. Hai chân/bóng giữ vùng gạch và cột nền. | Từ F155: từ tư thế quay phải về đứng trước; kiếm và grip đỏ đi cùng tay. | chưa kiểm chứng |
| F157 | 9.156000 / 0.033000 | Player hướng mặt trước, khăn vòng trái; skeleton idle với grip đỏ nối lưỡi, đủ đầu/chân/vũ khí. | Từ F156: đổi cel idle, không nhảy chân hoặc bóng. | chưa kiểm chứng |
| F158 | 9.189000 / 0.033000 | Giữ idle hai actor, khăn vòng trái và kiếm skeleton chéo trước ngực; khay/countdown đọc được. | Từ F157: giữ pose, không bỏ frame lặp; sàn/cột ổn định. | chưa kiểm chứng |
| F159 | 9.222000 / 0.016000 | Khăn player ngắn về trái, skeleton đứng/kiếm trước; không có flash/cung che tay. | Từ F158: khăn đổi cel, thân/chân giữ cùng vị trí sàn. | chưa kiểm chứng |
| F160 | 9.238000 / 0.076000 | Giữ idle/khăn trái, tay và vũ khí skeleton liền; bóng đúng dưới hai actor. | Từ F159: giữ pose ở frame encode tiếp theo, không nhảy sorting. | chưa kiểm chứng |
| F161 | 9.314000 / 0.032000 | Khăn player lượn trái, đầu xoay phải; ngực skeleton đổi nhẹ theo idle, grip còn rõ. | Từ F160: dao động idle nhỏ, chân giữ gạch. | chưa kiểm chứng |
| F162 | 9.346000 / 0.048000 | Đầu/rib skeleton thấp hơn ở cel idle, player mặt trước/khăn ngắn; grip đỏ vẫn nối tay/lưỡi. | Từ F161: thay cel idle, không đổi anchor thân so với chân. | chưa kiểm chứng |
| F163 | 9.394000 / 0.053000 | Chữ a của any chuyển teal; player và skeleton vẫn idle, HUD không dời. | Từ F162: input đổi màu khay, không kích đòn trước khi hoàn tất từ. | chưa kiểm chứng |
| F164 | 9.447000 / 0.050000 | Chữ an teal, hai actor giữ idle; ánh đuốc đổi nhẹ trên nền. | Từ F163: tiến độ thêm một chữ, chữ/khay không nhảy vị trí. | chưa kiểm chứng |
| F165 | 9.497000 / 0.065000 | Đòn thường: điểm100/+10, player chém thấp/chuôi xanh trước; sao trắng/vàng và cung che tay/rib skeleton. write/up và từ mới trượt vào khay. | Từ F164: hoàn tất any kích chém; flash/camera cùng tác động cảnh, grip enemy bị che nên chưa kết luận cel đó. | chưa kiểm chứng |
| F166 | 9.562000 / 0.057000 | Flash thu thành tia cam, cung còn lớn; write/up/place vào vị trí khay, player thấp, HP enemy giảm. | Từ F165: sao thu, khay settle; chân/bóng theo cùng nền sau rung. | chưa kiểm chứng |
| F167 | 9.619000 / 0.034000 | Flash hết, cung vẫn che tay enemy; player giữ thấp, đầu/chân hai actor đủ khung. | Từ F166: bỏ tia cam, không thấy đảo sorting; grip enemy vẫn chưa quan sát được. | chưa kiểm chứng |
| F168 | 9.653000 / 0.042000 | Skeleton Hurt trắng/ngả sau phải, cung che phần tay; player thấp/chuôi trước, +10 trên đầu. | Từ F167: enemy vào Hurt sau đòn; màu trắng che chi tiết albedo, chân giữ vùng gạch. | chưa kiểm chứng |
| F169 | 9.695000 / 0.066000 | Giữ Hurt trắng/ngả sau với cung trước ngực/tay; chân skeleton và player đều còn rõ. | Từ F168: giữ pose Hurt, không bỏ frame encode lặp hoặc coi grip bị che là đã thấy. | chưa kiểm chứng |
| F170 | 9.761000 / 0.035000 | Cung đã hết; player kiếm thấp trước phải, skeleton đứng/grip đỏ lộ rõ; +10 còn trên đầu. | Từ F169: từ cung/Hurt sang không cung/đứng; không suy thêm cel tan cung không được encode ở giữa. | chưa kiểm chứng |
| F171 | 9.796000 / 0.082000 | Player giữ chém thấp, skeleton đứng/kiếm chéo, +10 nhạt dần; bóng và gạch ổn định. | Từ F170: giữ hồi đầu, không tách tay/kiếm. | chưa kiểm chứng |
| F172 | 9.878000 / 0.016000 | Player nâng thân/kiếm thấp trước tay, khăn rủ; skeleton đứng, chân hai actor giữ sàn. | Từ F171: bắt đầu hồi nâng thân, không đổi sorting hay mất bóng. | chưa kiểm chứng |
| F173 | 9.894000 / 0.038000 | Player gần đứng với mảnh lưỡi trước phải; skeleton sẵn sàng, +10 rất mờ; đầu/chân/kiếm đủ pan. | Từ F172: kiếm thu dần trong recovery, landmark chân giữ. | chưa kiểm chứng |
| F174 | 9.932000 / 0.050000 | Player giữ hồi gần đứng/mảnh kiếm trước phải, skeleton idle, +10 mờ; chân/bóng đủ khung. | Từ F173: giữ recovery, chuôi/lưỡi player còn cùng tay. | chưa kiểm chứng |
| F175 | 9.982000 / 0.068000 | Pose hồi player còn mảnh kiếm; skeleton đổi cel idle hơi thấp/trái, kiếm vẫn nối grip đỏ. | Từ F174: đổi cel idle enemy trong cùng vùng gạch, không có camera nhảy rõ. | chưa kiểm chứng |
| F176 | 10.050000 / 0.054000 | Player đứng, chỉ còn vài mảnh lưỡi trước phải; skeleton đứng, không flash/cung che. | Từ F175: player nâng thân về idle, chân trụ giữ. | chưa kiểm chứng |
| F177 | 10.104000 / 0.038000 | Player gần idle với mảnh lưỡi cuối rất nhỏ; skeleton giữ đứng/kiếm, +10 đã hết. | Từ F176: mảnh kiếm tan dần, không đổi sorting. | chưa kiểm chứng |
| F178 | 10.142000 / 0.071000 | Player idle, không còn mảnh kiếm; khăn kéo trái, skeleton vẫn đứng. | Từ F177: kết recovery về idle, đủ đầu/chân/vũ khí. | chưa kiểm chứng |
| F179 | 10.213000 / 0.078000 | Mặt player hướng trước/khăn ngắn; skeleton cầm kiếm chéo trước; khay write/up/place đọc rõ. | Từ F178: thay cel idle, nền và bóng giữ. | chưa kiểm chứng |
| F180 | 10.291000 / 0.056000 | Player mặt phải/khăn vòng trái, skeleton xoay nhẹ đầu sang phải; hai grip không bị VFX che. | Từ F179: dao động idle cả hai actor, chân vẫn cùng vùng gạch. | chưa kiểm chứng |
| F181 | 10.347000 / 0.064000 | Chữ w của write teal, player idle; skeleton đầu/rib thấp hơn, kiếm nối tay. | Từ F180: input thêm chữ, chưa phát đòn; khay không nhảy. | chưa kiểm chứng |
| F182 | 10.411000 / 0.057000 | Chữ wr teal, khăn player vòng trái; skeleton giữ idle với kiếm trước ngực. | Từ F181: thêm tiến độ một chữ, anchor hai actor giữ. | chưa kiểm chứng |
| F183 | 10.468000 / 0.129000 | Chữ wri teal; player idle/khăn vòng, skeleton đứng; countdown tiếp tục giảm. | Từ F182: tiến độ nhập tiếp, không che actor hoặc dịch khay. | chưa kiểm chứng |
| F184 | 10.597000 / 0.039000 | Slayer entry: điểm112/+12 và thanh x1.2 hiện, cảnh bắt đầu đỏ/zoom; player hạ thấp đưa kiếm ngang sàn, skeleton vẫn đủ grip. | Từ F183: hoàn tất write chuyển sang Slayer, actor/nền cùng zoom còn HUD/khay đứng yên; từ mới trượt vào. | chưa kiểm chứng |
| F185 | 10.636000 / 0.097000 | Slayer đỏ đậm hơn/zoom gần, player giữ thấp/kiếm ngang, skeleton đứng đủ đầu/kiếm/chân. Đã mở crop rộng600×350 nearest4× do pan hẹp cắt đầu enemy. | Từ F184: cảnh tiếp tục zoom trong entry, không coi cắt crop là mất đầu; grip vẫn nối tay, bóng theo sàn. | chưa kiểm chứng |
| F186 | 10.733000 / 0.056000 | Entry đỏ/zoom gần: player thấp/kiếm ngang, skeleton sáng xám/grip vẫn nối; xem thêm pan(380,100)–(980,510)4× để đủ đỉnh đầu. | Từ F185: camera nâng/phóng cảnh cùng actor và sàn, HUD/khay giữ vị trí; không kết luận nhảy anchor riêng sprite. | chưa kiểm chứng |
| F187 | 10.789000 / 0.053000 | Player giữ thấp/kiếm ngang, skeleton xám với đủ đầu/tay/lưỡi/chân; cảnh đỏ tăng, nền ít chi tiết. | Từ F186: camera đưa actor xuống so với F186, hai actor cùng đổi khung; thanh x1.2 giảm. | chưa kiểm chứng |
| F188 | 10.842000 / 0.088000 | Hai actor tối hơn trên nền đỏ; kiếm player sát sàn, skeleton đứng/kiếm chéo; chân đủ pan tới y510. | Từ F187: camera hạ tiếp, silhouette còn tách; chi tiết màu/grip bị tint tối hạn chế quan sát. | chưa kiểm chứng |
| F189 | 10.930000 / 0.053000 | Silhouette đen trên dải đỏ, player gập người hồi, skeleton cầm kiếm chéo. Đã mở pan tới y570 để đủ bàn chân ở gần y530. | Từ F188: palette về đen/đỏ, mất landmark gạch/bóng nên chưa đánh giá riêng bám sàn ở frame này. | chưa kiểm chứng |
| F190 | 10.983000 / 0.077000 | Player nâng thân, kiếm đen còn trước phải; skeleton giữ silhouette đứng, bàn chân đủ pan mở rộng. | Từ F189: hồi từ gập sang đứng, chân cùng đường đáy; chi tiết grip không đọc dưới tint đen. | chưa kiểm chứng |
| F191 | 11.060000 / 0.095000 | Giữ hồi player/kiếm trước phải; skeleton đổi nhẹ đầu/cánh tay, vẫn đủ silhouette trên dải đỏ. | Từ F190: đổi cel idle enemy, dải đỏ/camera gần settle; không coi silhouette là kiểm được albedo. | chưa kiểm chứng |
| F192 | 11.155000 / 0.017000 | Player gần đứng, mảnh kiếm đen trước phải; skeleton đứng/kiếm chéo, x1.2 giảm. | Từ F191: kiếm thu thành mảnh trong recovery, chân giữ đường đáy silhouette; sàn vẫn không thấy. | chưa kiểm chứng |
| F193 | 11.172000 / 0.055000 | Player đứng với mảnh kiếm cuối và khăn vòng ra trái; skeleton giữ kiếm. Đã mở riêng pan trái(300,250)–(500,490)4× cho đủ khăn. | Từ F192: thân nâng tiếp, khăn lượn, không nhầm phần khăn ngoài crop rộng ban đầu là bị cắt runtime. | chưa kiểm chứng |
| F194 | 11.227000 / 0.072000 | Player idle silhouette, kiếm thu hết/khăn hất trái; skeleton đứng. Đã mở pan trái4× đủ đầu khăn. | Từ F193: kết hồi về idle, không đổi vị trí chân rõ; tint đen vẫn che cấu trúc tay. | chưa kiểm chứng |
| F195 | 11.299000 / 0.022000 | Giữ idle silhouette player/khăn cong trái, skeleton kiếm chéo. Đã mở pan trái4×, đủ khăn. | Từ F194: thay cel khăn, thân/chân vẫn cùng đường đáy; landmark sàn không áp dụng khi nền đỏ trơn. | chưa kiểm chứng |
| F196 | 11.321000 / 0.043000 | Giữ idle đen/đỏ, khăn player cong trái, x1.2 tiếp tục giảm; đã mở pan trái4× đầy đủ. | Từ F195: giữ pose encode kế tiếp; không bỏ frame giữ và không suy chi tiết màu bị tint. | chưa kiểm chứng |
| F197 | 11.364000 / 0.047000 | Player idle/khăn ngang trái hơn, skeleton đứng/kiếm chéo; pan trái4× đã xem đủ khăn. | Từ F196: khăn thay cel, silhouette không dính enemy, HUD/khay giữ vị trí. | chưa kiểm chứng |
| F198 | 11.411000 / 0.077000 | Idle Slayer đen/đỏ: khăn player rủ trái, skeleton đứng/kiếm chéo; pan mới(300,100)–(980,570)4× đủ khăn và chân. | Từ F197: đổi cel khăn từ ngang sang rủ, actor vẫn tách; không thấy gạch/bóng dưới nền đỏ. | chưa kiểm chứng |
| F199 | 11.488000 / 0.049000 | Khăn player thành vòng trái, skeleton giữ silhouette đứng; x1.2 giảm, khay up/place/when. | Từ F198: vòng khăn thay hình, chân cùng đường đáy; grip chỉ thấy đường viền đen. | chưa kiểm chứng |
| F200 | 11.537000 / 0.034000 | Giữ idle với vòng khăn trái, enemy kiếm chéo; cả hai đủ đầu/chân. | Từ F199: giữ pose encode, thanh x1.2/countdown giảm; không bỏ frame giữ. | chưa kiểm chứng |
| F201 | 11.571000 / 0.056000 | Player mặt quay trước hơn, khăn trái góc nhọn; skeleton giữ đứng/kiếm. | Từ F200: đổi cel đầu/khăn, không thay khoảng cách hai actor rõ. | chưa kiểm chứng |
| F202 | 11.627000 / 0.072000 | Player mặt phải/khăn ngang đầu trái, skeleton giữ silhouette; HUD/khay vẫn đọc được. | Từ F201: dao động idle, bàn chân không đổi đường đáy. | chưa kiểm chứng |
| F203 | 11.699000 / 0.058000 | Khăn player hất lên trái, tay sát thân; enemy đứng/kiếm chéo, x1.2 gần cạn. | Từ F202: khăn đổi cel; cảnh đỏ trơn không cung cấp landmark sàn để kiểm trượt. | chưa kiểm chứng |
| F204 | 11.757000 / 0.040000 | Khăn player cong lên trái, hai actor idle đen; x1.2 và countdown giảm. | Từ F203: giữ thân/chân, thay nhẹ cel khăn; không có VFX mới. | chưa kiểm chứng |
| F205 | 11.797000 / 0.051000 | Giữ idle đen/đỏ, khăn cong trái; đủ silhouette hai actor và vũ khí enemy. | Từ F204: giữ pose encode kế tiếp; không suy chi tiết albedo bị tint. | chưa kiểm chứng |
| F206 | 11.848000 / 0.063000 | Slayer chém cung ngà lớn trước phải, skeleton flash sáng toàn thân; điểm115/+3, x1.3, chữ u teal. | Từ F205: từ idle vào chém thấp; cung/flash che tay-ngực enemy, chưa kiểm được grip ở cel này. | chưa kiểm chứng |
| F207 | 11.911000 / 0.119000 | Player đứng xoay/kiếm ngà ngang thấp, khăn dài trái; skeleton flash sáng, cung thu thành dải và tia ngang; điểm118/+6, x1.5, khay sang place. Mở riêng trái(200,250)–(400,430) và phải(960,280)–(1080,450)4× đủ khăn/tia ngoài crop. | Từ F206: hai input gần nhau đổi pose và mức nhân; cung thu, tay enemy vẫn lẫn flash, không suy recovery ở giữa nếu không encode. | chưa kiểm chứng |
| F208 | 12.030000 / 0.017000 | Player hồi đứng/kiếm mảnh đen bên trái, khăn hất; skeleton ngả Hurt silhouette, flash/cung đã hết; +6 trên đầu. | Từ F207: flash sáng về đen, các mảnh tia phải gần hết; đủ đầu/chân/kiếm, grip chi tiết không đọc vì tint. | chưa kiểm chứng |
| F209 | 12.047000 / 0.053000 | Giữ pose hồi player/kiếm mảnh trái, skeleton Hurt ngả sau với kiếm gần đứng; x1.5 giảm. | Từ F208: giữ frame encode liền trước, chân cùng đường đáy, không bỏ hold. | chưa kiểm chứng |
| F210 | 12.100000 / 0.085000 | Player hồi đứng, còn hai mảnh kiếm đen xa trái/khăn dựng; skeleton giữ Hurt ngả, kiếm gần đứng; +6 còn. | Từ F209: mảnh kiếm nhỏ hơn so với F209, chân giữ đáy; tint đen che chi tiết grip. | chưa kiểm chứng |
| F211 | 12.185000 / 0.033000 | Player đứng/tay sát thân, không còn mảnh kiếm xa trái; khăn chĩa lên trái. Skeleton vẫn giữ Hurt. | Từ F210: kết phần mảnh kiếm của recovery; đủ đầu/chân hai actor. | chưa kiểm chứng |
| F212 | 12.218000 / 0.023000 | Giữ player đứng/khăn chĩa trái và enemy Hurt silhouette; thanh x1.5 giảm, khay đọc rõ. | Từ F211: giữ pose encode kế tiếp, không bỏ frame lặp; nền đỏ không có landmark gạch. | chưa kiểm chứng |
| F213 | 12.241000 / 0.059000 | Player đứng/khăn chĩa trái, skeleton ngả/kiếm dựng; +6 nhạt dần; đủ silhouette trong pan. | Từ F212: giữ pose, HUD không dời; không suy cấu trúc tay từ silhouette. | chưa kiểm chứng |
| F214 | 12.300000 / 0.096000 | Hai actor giữ silhouette, player đứng và skeleton Hurt; +6 tối hơn, x1.5 còn trên HUD. | Từ F213: giữ phản ứng bị Slayer đánh, chân cùng đường đáy quan sát được. | chưa kiểm chứng |
| F215 | 12.396000 / 0.037000 | Player giữ đứng/khăn trái, skeleton giữ ngả/kiếm dựng; +6 rất mờ. | Từ F214: giữ pose encode, countdown/x1.5 tiếp tục giảm, không có cung mới. | chưa kiểm chứng |
| F216 | 12.433000 / 0.104000 | Giữ hai silhouette và kiếm enemy; +6 gần mất, khay place/when/had không bị actor che. | Từ F215: giữ frame tiếp theo, không có đổi sorting rõ từ đường viền. | chưa kiểm chứng |
| F217 | 12.537000 / 0.015000 | Player đứng/khăn trái và enemy Hurt giữ nguyên, +6 đã hết. | Từ F216: label điểm nổi tắt, silhouette không đổi; sàn/bóng vẫn bị nền đỏ che. | chưa kiểm chứng |
| F218 | 12.552000 / 0.044000 | Giữ pose sau Slayer, đủ đầu/chân hai actor; x1.5 giảm và countdown còn teal. | Từ F217: giữ frame lặp, không lấy giữ pose làm một vòng idle có chuyển cel. | chưa kiểm chứng |
| F219 | 12.596000 / 0.042000 | Player đứng/khăn trái; skeleton ngả/kiếm gần đứng, hai silhouette tách trên đỏ. | Từ F218: giữ pose encode liền trước, chân không đổi đường đáy rõ. | chưa kiểm chứng |
| F220 | 12.638000 / 0.094000 | Giữ silhouette player và Hurt enemy, điểm118/x1.5; khay chữ đứng yên. | Từ F219: tiếp hold, không bỏ thời lượng giữ; grip/albedo chưa đọc được vì tint. | chưa kiểm chứng |
| F221 | 12.732000 / 0.070000 | Giữ pose sau đòn, x1.5 tiếp tục giảm, actor đủ đầu/chân/vũ khí trong pan mở rộng. | Từ F220: không có nhảy silhouette rõ so với F220; không suy thêm cel không được encode. | chưa kiểm chứng |
| F222 | 12.802000 / 0.033000 | Slayer đỏ/đen; player đứng tay sát thân, khăn hướng lên trái; đối thủ Hurt ngửa, kiếm gần thẳng đứng. Đầu/chân trọn pan; grip/albedo vẫn bị tint che. | Từ F221: giữ silhouette F221, mốc chân không trượt; countdown tiếp tục giảm. | chưa kiểm chứng |
| F223 | 12.835000 / 0.062000 | Pose player và đối thủ giữ; khăn lên trái, hai cụm chân trên cùng đường sàn; khay x1.5 còn đọc được. | Từ F222: giữ F222, không lẫn frame giữ với vòng idle. | chưa kiểm chứng |
| F224 | 12.897000 / 0.072000 | Hai silhouette đen tiếp tục giữ, kiếm đối thủ dựng trước thân; biên vùng đỏ không đổi, landmark sàn bị Slayer phủ. | Từ F223: không thấy nhảy đầu/chân so F223; không kết luận chi tiết grip qua tint. | chưa kiểm chứng |
| F225 | 12.969000 / 0.064000 | Player đứng và đối thủ ngửa Hurt vẫn giữ; toàn đầu/chân nằm trong pan, không có flash mới; HUD chỉ đổi đồng hồ/gauge. | Từ F224: giữ F224, không có đổi sorting nhìn thấy. | chưa kiểm chứng |
| F226 | 13.033000 / 0.097000 | Silhouette và khăn giữ như F225; đường chân y528 ổn định, nền đỏ che cấu trúc sàn. | Từ F225: frame giữ tiếp; kiểm cả duration thay vì bỏ lặp. | chưa kiểm chứng |
| F227 | 13.130000 / 0.054000 | Đòn Slayer mới: score121/+3, x1.6; kiếm player màu ngà nằm thấp phía trước, khăn kéo xa trái x264; đã mở thêm pan trái (200,250)-(400,430) nearest4× để thấy hết đầu khăn. Đối thủ flash ngà, cung mảnh đứng trước ngực che grip; chân vẫn ở y528. | Từ F226: từ pose giữ F226 sang vung/flash, điểm chân giữ; chi tiết đối thủ bị flash/cung còn chưa kiểm chứng riêng. | chưa kiểm chứng |
| F228 | 13.184000 / 0.059000 | Cung Slayer ngà rộng quét từ sát chân player lên đầu/ngực đối thủ; đối thủ flash, player thấp người, hai chân trụ giữ; x1.8/+6. Cung che tay/kiếm và thân nên chưa đọc grip. | Từ F227: F227→228 cung mở lớn dọc chéo lên phải; đầu khăn co lại trong pan. | chưa kiểm chứng |
| F229 | 13.243000 / 0.067000 | Player xoay cúi sâu, kiếm/cung hạ tới chân đối thủ; flash ngà còn, x1.9/+10; thân/đầu đối thủ cao hơn F228 nhưng chân vẫn cùng đường sàn. | Từ F228: cung chuyển từ dọc sang chéo thấp, không thấy chân bật khỏi landmark. | chưa kiểm chứng |
| F230 | 13.310000 / 0.078000 | Cung dài cong thấp gần sàn từ sau player tới thân đối thủ; score136/x2.2/+18. Khăn kéo trái x272, đã xem pan trái bổ sung (200,250)-(400,430) 4×; flash che thân/grip đối thủ. | Từ F229: F229→230 xoay quét rộng ngang; mũi cung ngà ở trái không phải vũ khí rời chưa đối chiếu cel. | chưa kiểm chứng |
| F231 | 13.388000 / 0.016000 | Flash thân đối thủ tắt về đen; cung chỉ còn mảnh ngà dọc bụng/chân, player đứng hồi phục tay gần thân. Khăn trái x284 đã mở pan trái bổ sung 4×. | Từ F230: từ quét thấp F230 sang đứng, hai chân trụ không nhảy; mảnh cung nhỏ tan còn che một phần chân đối thủ. | chưa kiểm chứng |
| F232 | 13.404000 / 0.040000 | Cung ngà biến mất; player đứng với hai dải khăn sau trái, đối thủ giữ Hurt ngửa với kiếm dựng; đầu/chân trọn pan, nền đỏ/tint đen che albedo. | Từ F231: F231→232 recovery, không thấy đổi sorting; chỉ đánh giá silhouette qua Slayer. | chưa kiểm chứng |
| F233 | 13.444000 / 0.047000 | Player giữ tư thế đứng, dải khăn trái hạ thấp hơn F232; đối thủ giữ Hurt, x2.2/+18 và chân cùng mốc. | Từ F232: khăn đổi cel, thân và chân giữ từ F232; cần playback để kết luận nhịp. | chưa kiểm chứng |
| F234 | 13.491000 / 0.025000 | Recovery Slayer: player đứng tay thấp, hai dải khăn chĩa trái ở hai độ cao; đối thủ Hurt ngửa giữ kiếm dựng. Mốc chân hai bên giữ, grip bị tint đen. | Từ F233: từ F233 khăn hạ tiếp, thân/đầu không nhảy. | chưa kiểm chứng |
| F235 | 13.516000 / 0.027000 | Khăn player co ngắn về vai, hai nhánh trái ngắn; đối thủ giữ Hurt, +18 còn trên đầu; không có cung/flash. | Từ F234: F234→235 đổi cel khăn hồi phục; hai chân giữ. | chưa kiểm chứng |
| F236 | 13.543000 / 0.026000 | Giữ player đứng/khăn ngắn hai nhánh và đối thủ Hurt; countdown và gauge Slayer giảm; toàn silhouette nằm trong pan. | Từ F235: giữ F235, không thấy trượt chân hoặc đổi sorting. | chưa kiểm chứng |
| F237 | 13.569000 / 0.063000 | Pose hai bên giữ như F236, kiếm đối thủ nghiêng dựng sang trái; nền đỏ che mốc sàn, albedo/grip không đọc được. | Từ F236: giữ F236; không suy thành idle đủ vòng từ pose này. | chưa kiểm chứng |
| F238 | 13.632000 / 0.050000 | Player và đối thủ tiếp tục giữ silhouette đen; không có particle/cung mới; +18 mờ dần, x2.2 còn. | Từ F237: giữ F237; gauge giảm mà thân/chân không đổi vị trí. | chưa kiểm chứng |
| F239 | 13.682000 / 0.033000 | Silhouette đứng và Hurt giữ; chân ở cùng đường y528; khay when/had/take đọc đủ, countdown giảm. | Từ F238: giữ F238; vùng đỏ và scale nhìn không đổi. | chưa kiểm chứng |
| F240 | 13.715000 / 0.048000 | Player khăn ngắn hai nhánh và đối thủ ngửa giữ, không có flash; +18 giảm sáng thêm. | Từ F239: giữ F239; chưa thấy thay màu/zoom ra Slayer. | chưa kiểm chứng |
| F241 | 13.763000 / 0.044000 | Hai pose giữ; nền đỏ phẳng, mũi kiếm đối thủ dựng vẫn ở trái thân; UI countdown/gauge tiếp tục giảm. | Từ F240: giữ F240, chân và đầu giữ cùng tọa độ. | chưa kiểm chứng |
| F242 | 13.807000 / 0.014000 | Pose đứng/Hurt và khăn ngắn giữ; +18 rất mờ; toàn đầu/vũ khí/chân nằm trong pan. | Từ F241: giữ F241; grip và bóng vẫn chưa thể đánh giá qua tint. | chưa kiểm chứng |
| F243 | 13.821000 / 0.037000 | Slayer recovery giữ player đứng, khăn ngắn hai nhánh bên trái và đối thủ Hurt/kiếm dựng; +18 gần tan; chân không đổi. | Từ F242: giữ F242, countdown tiếp tục giảm. | chưa kiểm chứng |
| F244 | 13.858000 / 0.047000 | Hai silhouette đen giữ, không có cung/flash; +18 chỉ còn nét tối; đầu và chân trọn pan. | Từ F243: giữ F243; chưa đọc grip qua tint. | chưa kiểm chứng |
| F245 | 13.905000 / 0.058000 | +18 biến mất; player đứng khăn ngắn, đối thủ ngửa Hurt, nền đỏ và tỷ lệ nhân vật giữ. | Từ F244: F244→245 chỉ feedback score tan, không có đổi pose nhìn thấy. | chưa kiểm chứng |
| F246 | 13.963000 / 0.068000 | Player/đối thủ giữ silhouette, các chân trên cùng đường y528; khay when/had/take còn đủ chữ, x2.2 giữ. | Từ F245: giữ F245, gauge Slayer giảm. | chưa kiểm chứng |
| F247 | 14.031000 / 0.088000 | Không có cung/particle mới; khăn ngắn sau player và kiếm đối thủ dựng giữ nguyên; bóng/sàn bị tint che. | Từ F246: giữ F246, không thấy trượt chân. | chưa kiểm chứng |
| F248 | 14.119000 / 0.027000 | Hai pose tiếp tục giữ; toàn đầu/chân/vũ khí trong pan; UI gauge và countdown giảm, không có score mới. | Từ F247: giữ F247; không tính chuỗi giữ này là idle đủ vòng. | chưa kiểm chứng |
| F249 | 14.146000 / 0.045000 | Giữ player đứng khăn ngắn hai nhánh và đối thủ Hurt/kiếm dựng; không còn feedback +18. | Từ F248: giữ F248, countdown/gauge giảm, chân không trượt. | chưa kiểm chứng |
| F250 | 14.191000 / 0.069000 | Hai silhouette và mốc chân giữ; nền đỏ phẳng, khay when/had/take đọc đủ. | Từ F249: giữ F249; không có cung/flash mới. | chưa kiểm chứng |
| F251 | 14.260000 / 0.034000 | Player và đối thủ giữ pose, không đổi sorting nhìn thấy; đầu/chân trọn pan. | Từ F250: giữ F250, chưa đọc albedo/grip qua tint. | chưa kiểm chứng |
| F252 | 14.294000 / 0.045000 | Pose đứng/Hurt giữ, hai nhánh khăn player ngắn sau vai; x2.2 còn và gauge ngắn hơn. | Từ F251: giữ F251; không tính lặp là vòng idle. | chưa kiểm chứng |
| F253 | 14.339000 / 0.092000 | Giữ toàn silhouette như F252; kiếm đối thủ dựng nghiêng về trái, không có particle. | Từ F252: giữ F252, đường chân y528 ổn định. | chưa kiểm chứng |
| F254 | 14.431000 / 0.038000 | Player đứng, đối thủ Hurt ngửa giữ; chữ trong khay không nhảy, gauge tiếp tục giảm. | Từ F253: giữ F253; mốc sàn chi tiết bị Slayer phủ. | chưa kiểm chứng |
| F255 | 14.469000 / 0.068000 | Hai pose và khăn giữ, không flash mới; toàn đầu/kiếm/chân trong pan. | Từ F254: giữ F254, countdown giảm mà silhouette không đổi. | chưa kiểm chứng |
| F256 | 14.537000 / 0.128000 | Đòn Slayer mới: player nâng kiếm dựng cao trên đầu, cánh tay trước thân; đối thủ flash ngà, +5/x2.4; chân player giữ cùng đường. | Từ F255: F255→256 từ đứng sang chuẩn bị chém dọc, không cắt mũi kiếm trong pan. | chưa kiểm chứng |
| F257 | 14.665000 / 0.096000 | Player xoay cúi sâu, cung ngà từ trước ngực đối thủ xuống sát chân; đối thủ flash, +15/x2.7, bàn chân giữ đường sàn. | Từ F256: kiếm dựng F256 hạ thành cung chéo, grip bị silhouette/cung che. | chưa kiểm chứng |
| F258 | 14.761000 / 0.083000 | Player vẫn cúi với lưỡi/cung thấp; flash thân đối thủ tắt nhưng cung ngà còn phủ ngực/chân; +21/x2.8, khay chuyển had/take/is. | Từ F257: F257→258 thân đối thủ trở đen, cung chưa tan; hai chân giữ. | chưa kiểm chứng |
| F259 | 14.844000 / 0.061000 | Cung ngà đã tan; player cúi hồi phục, tay/kiếm thấp về trước; đối thủ giữ Hurt đen và +21 còn. | Từ F258: từ cung F258 sang silhouette, chân không bật; chi tiết grip chưa đọc qua tint. | chưa kiểm chứng |
| F260 | 14.905000 / 0.051000 | Player đứng dậy, khăn cong rủ trái và mảnh đen nhỏ bên phải vũ khí; đối thủ Hurt giữ, không flash. | Từ F259: F259→260 hồi thân lên thẳng, chân giữ trụ; các mảnh đen cần đối chiếu VFX/cel riêng. | chưa kiểm chứng |
| F261 | 14.956000 / 0.088000 | Player đứng, khăn hai dải kéo trái rõ hơn, mảnh vũ khí đen phía phải co lại; đối thủ giữ Hurt/kiếm dựng. | Từ F260: F260→261 khăn và recovery đổi cel, chân giữ. | chưa kiểm chứng |
| F262 | 15.044000 / 0.052000 | Hai pose đứng/Hurt giữ như F261, mảnh đen phía tay phải còn, +21 mờ thêm. | Từ F261: giữ F261, gauge/countdown giảm; không suy chi tiết bị tint che. | chưa kiểm chứng |
| F263 | 15.096000 / 0.061000 | Player khăn hai dải trái và đối thủ ngửa giữ; đầu/chân/kiếm trong pan, +21 tiếp tục tan. | Từ F262: giữ F262, mốc chân không nhảy. | chưa kiểm chứng |
| F264 | 15.157000 / 0.066000 | Player tóc nhọn, hai dải khăn rủ trái; mảnh smear đen phía phải và đối thủ Hurt ngửa/kiếm dựng còn nguyên, +21 mờ. | Từ F263: Giữ F263, chân hai bên cùng đường nền đỏ; không có cung mới. | chưa kiểm chứng |
| F265 | 15.223000 / 0.035000 | Hai silhouette giữ tư thế, +21 nhạt hơn; đầu, bàn chân và kiếm nằm đủ trong pan. | Từ F264: Giữ F264; không flash hay trượt chân. | chưa kiểm chứng |
| F266 | 15.258000 / 0.048000 | Khăn hai dải và mảnh đen cạnh vũ khí giữ; +21 gần chìm vào nền. | Từ F265: Giữ F265, chỉ chữ sát thương tiếp tục tan. | chưa kiểm chứng |
| F267 | 15.306000 / 0.087000 | +21 đã hết; player đứng và đối thủ Hurt ngửa với kiếm dựng còn giữ. | Từ F266: Từ F266 chữ biến mất, thân/khăn/chân không nhảy. | chưa kiểm chứng |
| F268 | 15.393000 / 0.033000 | Pose đứng/khăn trái và đối thủ ngửa giữ; gauge trên giảm, nền đen đỏ vẫn che albedo. | Từ F267: Giữ F267, không có bước chân hoặc cung chém mới. | chưa kiểm chứng |
| F269 | 15.426000 / 0.037000 | Đầu, chân và toàn kiếm của hai bên vẫn trong pan; player giữ hai dải khăn rủ. | Từ F268: Giữ F268; đường chân và sorting silhouette ổn định. | chưa kiểm chứng |
| F270 | 15.463000 / 0.017000 | Player đứng, đối thủ Hurt ngửa, x2.8 đọc được; không có flash trắng. | Từ F269: Giữ F269, chỉ gauge/countdown tiếp tục chạy. | chưa kiểm chứng |
| F271 | 15.480000 / 0.040000 | Hai dải khăn và mảnh smear đen bên phải giữ, kiếm đối thủ dựng. | Từ F270: Giữ F270; các landmark silhouette không nhảy. | chưa kiểm chứng |
| F272 | 15.520000 / 0.074000 | Player/đối thủ giữ đầy đủ bàn chân; nền đỏ cùng silhouette đen, không cung mới. | Từ F271: Giữ F271; chân và khăn không dịch. | chưa kiểm chứng |
| F273 | 15.594000 / 0.033000 | Tóc nhọn, khăn rủ hai nhánh và đối thủ ngửa vẫn giữ; gauge tiếp tục giảm. | Từ F272: Giữ F272; không VFX mới hay cắt đầu/chân. | chưa kiểm chứng |
| F274 | 15.627000 / 0.035000 | Pose đứng đối diện Hurt ngửa giữ; mảnh đen cạnh vũ khí còn nguyên. | Từ F273: Giữ F273, khoảng cách hai thân và đường chân không đổi. | chưa kiểm chứng |
| F275 | 15.662000 / 0.017000 | Khăn hai nhánh trái và kiếm đối thủ dựng giữ; nền đỏ không đổi vị trí. | Từ F274: Giữ F274; chưa có thay đổi pose recovery khác. | chưa kiểm chứng |
| F276 | 15.679000 / 0.048000 | Cả hai bàn chân và đầu đầy đủ trong pan, silhouette đứng/ngửa giữ. | Từ F275: Giữ F275, không flash hoặc trượt anchor. | chưa kiểm chứng |
| F277 | 15.727000 / 0.073000 | Player tóc nhọn đứng, khăn rủ và mảnh smear đen giữ; đối thủ Hurt ngửa. | Từ F276: Giữ F276; chữ x2.8 và gauge nằm nguyên vị trí. | chưa kiểm chứng |
| F278 | 15.800000 / 0.016000 | Hai silhouette giữ, thanh trên giảm; không có cung hay số damage mới. | Từ F277: Giữ F277; đường chân và khoảng cách không nhảy. | chưa kiểm chứng |
| F279 | 15.816000 / 0.045000 | Khăn trái hai dải giữ, kiếm đối thủ dựng và đầu ngửa vẫn trong khung. | Từ F278: Giữ F278; không có frame trắng che silhouette. | chưa kiểm chứng |
| F280 | 15.861000 / 0.036000 | Player đứng cạnh mảnh smear đen và đối thủ ngửa giữ; nền đen/đỏ vẫn hạn chế grip/albedo. | Từ F279: Giữ F279; chân chung đường nền, gauge giảm. | chưa kiểm chứng |
| F281 | 15.897000 / 0.054000 | Pose hai bên và hai nhánh khăn vẫn giữ trọn đầu/chân/vũ khí; không VFX mới. | Từ F280: Giữ F280; chưa thấy exit trả palette trong frame này. | chưa kiểm chứng |
| F282 | 15.951000 / 0.073000 | Hai silhouette đứng/ngửa, khăn hai dải và kiếm dựng giữ; x2.8 vẫn hiện, gauge còn ít. | Từ F281: Giữ F281; chân cùng đường nền đỏ, không flash/cung mới. | chưa kiểm chứng |
| F283 | 16.024000 / 0.029000 | Đầu/chân/kiếm đủ trong pan; khăn trái và mảnh đen bên vũ khí giữ, gauge nhỏ hơn. | Từ F282: Giữ F282; chỉ thanh trên và countdown giảm. | chưa kiểm chứng |
| F284 | 16.053000 / 0.034000 | Player tóc nhọn đứng, đối thủ Hurt ngửa giữ; x2.8 và đường chân không đổi. | Từ F283: Giữ F283; không nhảy silhouette hay sorting. | chưa kiểm chứng |
| F285 | 16.087000 / 0.034000 | Pose hai bên giữ, gauge vàng chỉ còn đoạn ngắn; nền đỏ vẫn che sàn/bóng. | Từ F284: Giữ F284; đầu, chân, kiếm chưa bị cắt. | chưa kiểm chứng |
| F286 | 16.121000 / 0.047000 | Hai dải khăn rủ trái giữ, đối thủ ngửa/kiếm dựng giữ; gauge gần cạn. | Từ F285: Giữ F285, không cung hoặc số damage mới. | chưa kiểm chứng |
| F287 | 16.168000 / 0.063000 | Gauge vàng còn vạch nhỏ, x2.8 còn; player/đối thủ giữ trọn silhouette. | Từ F286: Giữ F286; chân và khoảng cách không đổi. | chưa kiểm chứng |
| F288 | 16.231000 / 0.065000 | Gauge vàng đã cạn nhưng x2.8 còn; hai silhouette và khăn giữ. | Từ F287: Từ F287 thanh hết, chưa có đổi pose hoặc palette. | chưa kiểm chứng |
| F289 | 16.296000 / 0.032000 | x2.8 và thanh Slayer biến mất; sân vẫn đỏ, actor vẫn đen cùng tư thế. | Từ F288: Từ F288 HUD đổi pha, chân/đầu/khăn giữ nguyên. | chưa kiểm chứng |
| F290 | 16.328000 / 0.036000 | Hai silhouette giữ nguyên với nền đỏ; không còn chỉ báo Slayer phía giữa. | Từ F289: Giữ F289; countdown dưới tiếp tục rút. | chưa kiểm chứng |
| F291 | 16.364000 / 0.060000 | Player đứng/khăn hai dải và đối thủ ngửa/kiếm dựng giữ; không cung mới. | Từ F290: Giữ F290; chưa đọc được albedo/grip do silhouette. | chưa kiểm chứng |
| F292 | 16.424000 / 0.040000 | Cả hai giữ pose đầy đủ đầu/chân/vũ khí, nền đỏ vẫn đồng nhất. | Từ F291: Giữ F291; không trượt đường chân. | chưa kiểm chứng |
| F293 | 16.464000 / 0.036000 | Silhouette đứng và ngửa giữ, thanh dưới tiếp tục giảm; không flash trắng. | Từ F292: Giữ F292; mảnh đen bên vũ khí chưa đổi. | chưa kiểm chứng |
| F294 | 16.500000 / 0.102000 | Khăn hai dải và đối thủ ngửa giữ, HUD không còn x2.8; sàn/bóng chưa hiện. | Từ F293: Giữ F293; không jump vị trí. | chưa kiểm chứng |
| F295 | 16.602000 / 0.047000 | Biểu tượng shield xanh thay cảnh báo đỏ ở HUD đối thủ; pose/sân đen đỏ giữ. | Từ F294: Từ F294 chỉ đổi biểu tượng; hai bàn chân không nhảy. | chưa kiểm chứng |
| F296 | 16.649000 / 0.034000 | Thanh xanh đối thủ đầy trở lại; hai silhouette giữ cùng đường chân. | Từ F295: Từ F295 thanh xuất hiện, không cung chém mới. | chưa kiểm chứng |
| F297 | 16.683000 / 0.047000 | Khăn player chuyển lên trái thành một nhánh cong, mảnh smear cạnh phải hết; đối thủ ngửa giữ. | Từ F296: Từ F296 đổi cel khăn/đứng trong khi chân giữ. | chưa kiểm chứng |
| F298 | 16.730000 / 0.082000 | Sân đỏ sẫm hơn, viền khay bắt đầu hiện; camera lùi làm actor nhỏ lại, cả hai chân vẫn đủ. | Từ F297: Từ F297 thu zoom và đổi cel đối thủ; không cắt đầu/kiếm. | chưa kiểm chứng |
| F299 | 16.812000 / 0.060000 | Đuốc và chi tiết mặt/xương rất tối bắt đầu hiện; player/đối thủ nhỏ lại theo camera, nền vẫn đỏ. | Từ F298: Từ F298 tiếp lùi zoom, khay từ hiện rõ hơn, bàn chân cùng dịch theo sân. | chưa kiểm chứng |
| F300 | 16.872000 / 0.064000 | Thấy xương sườn, mắt player và scarf nhạt trong tint đỏ; kiếm đối thủ dựng đủ mũi, sàn xanh bắt đầu rõ. | Từ F299: Từ F299 albedo sáng dần và camera tiếp lùi, không mất chi. | chưa kiểm chứng |
| F301 | 16.936000 / 0.101000 | Chi tiết mặt/khăn/xương rõ hơn, chữ h của had xanh; hai bên đứng, sân đỏ còn nhạt. | Từ F300: Từ F300 camera tiếp lùi, chân theo landmark sàn; chưa có flash. | chưa kiểm chứng |
| F302 | 17.037000 / 0.087000 | Player hạ thấp chém sát sàn, cung ngà và impact vàng trắng che ngực/kiếm đối thủ, +10; nền đỏ còn. | Từ F301: Từ F301 vào attack thường, chân trụ thấp; không thể kết luận grip phần bị impact che. | chưa kiểm chứng |
| F303 | 17.124000 / 0.026000 | Giữ chém thấp/cung ngà, impact trắng tan thành tia cam; xương đối thủ lộ dần, +10 còn. | Từ F302: Từ F302 camera lùi và shake đổi vị trí toàn sân, đầu/chân không cắt. | chưa kiểm chứng |
| F304 | 17.150000 / 0.048000 | Cung giữ trước thân đối thủ, tia cam thưa hơn; player thấp và tay nối chuôi xanh thấy được. | Từ F303: Từ F303 impact tan, sân/camera tiếp hồi, không có frame mất chân. | chưa kiểm chứng |
| F305 | 17.198000 / 0.099000 | SPACE hiện trên đầu đối thủ; cung ngà còn, tia cam gần hết; nền về màu nâu gần thường. | Từ F304: Từ F304 giữ chém thấp, camera tiếp lùi; card không che thân. | chưa kiểm chứng |
| F306 | 17.297000 / 0.121000 | Palette/sân thường và bóng xanh đã về, cung biến mất; player recovery thấp, kiếm sát sàn và đối thủ đứng. | Từ F305: Từ F305 mất cung, toàn đầu/chân/kiếm còn đủ; khung camera rộng lại. | chưa kiểm chứng |
| F307 | 17.418000 / 0.016000 | Player nhấc thân và thu kiếm thấp, scarf rủ; đối thủ đứng kiếm dựng, SPACE và +10 mờ. | Từ F306: Từ F306 recovery nâng thân, chân trụ tại cùng gạch sàn. | chưa kiểm chứng |
| F308 | 17.434000 / 0.047000 | Player cao hơn, kiếm thu ngang thấp trước người, scarf rủ trái; đối thủ đứng giữ. | Từ F307: Từ F307 kéo kiếm về tay, không đứt grip; camera/sàn ổn định. | chưa kiểm chứng |
| F309 | 17.481000 / 0.089000 | Giữ recovery với kiếm thấp, scarf rủ và đầu/chân đủ; +10 tiếp mờ. | Từ F308: Giữ F308, chỉ countdown/tia đuốc đổi; không trượt chân. | chưa kiểm chứng |
| F310 | 17.570000 / 0.060000 | Player đứng gần thẳng, khăn nhấc lên trái; kiếm thu thêm thành đoạn ngắn ngang thấp. | Từ F309: Từ F309 đứng lên trên chân cũ; đối thủ giữ kiếm dựng. | chưa kiểm chứng |
| F311 | 17.630000 / 0.088000 | Player về dáng idle thẳng, scarf bay trái, kiếm gần thu; +10 đã hết, sân thường. | Từ F310: Từ F310 khép recovery, chân giữ landmark sàn, không flash. | chưa kiểm chứng |
| F312 | 17.718000 / 0.023000 | Kiếm player không còn smear phía phải, khăn cuộn trái; đối thủ đứng kiếm dựng, SPACE còn. | Từ F311: Từ F311 đổi cel idle/khăn, chân và camera giữ. | chưa kiểm chứng |
| F313 | 17.741000 / 0.070000 | Giữ idle với scarf cuộn trái, tay đỏ và mắt xanh đọc được; sàn/bóng thường. | Từ F312: Giữ F312, chỉ thanh countdown giảm. | chưa kiểm chứng |
| F314 | 17.811000 / 0.102000 | Đối thủ đưa kiếm lên gần mặt, player idle scarf bay; toàn kiếm/đầu trong pan. | Từ F313: Từ F313 bắt đầu chuẩn bị đòn đối thủ sau exit; chân trụ vẫn trên sàn. | chưa kiểm chứng |
| F315 | 17.913000 / 0.062000 | Kiếm đối thủ xoay chéo lên phải ngang vai, hai tay gần ngực; player idle giữ. | Từ F314: Từ F314 xoay kiếm qua vai, không mất chuôi; SPACE vẫn viền trắng. | chưa kiểm chứng |
| F316 | 17.975000 / 0.097000 | Đối thủ giữ kiếm chéo ngang vai, SPACE nền sáng; player idle, sân thường và bóng xanh rõ. | Từ F315: tiếp windup; biên đã mở tới F338 để kiểm đủ parry/phản kích và recovery kế tiếp. | chưa kiểm chứng |

| F317 | 18.072000 / 0.056000 | Player đưa kiếm dựng trước mặt vào guard, chuôi xanh nối tay; khăn rủ trái. Đối thủ giữ kiếm chéo trên vai/ngực, SPACE nền teal; đủ đầu/chân. | Từ F316 player nâng guard trong khi đối thủ giữ windup; chân vẫn trên cùng gạch sàn. | chưa kiểm chứng |
| F318 | 18.128000 / 0.092000 | Giữ guard kiếm dựng và kiếm đối thủ chéo trước ngực; SPACE teal, không cắt mũi kiếm hoặc đầu. | Giữ F317, thanh countdown rút; không nhảy grip/chân. | chưa kiểm chứng |
| F319 | 18.220000 / 0.018000 | Đối thủ duỗi tay/kiếm ngang trái chạm guard, tia vàng trắng và +30; player giữ kiếm dựng, đủ hai bàn chân. | Từ F318 vào contact, camera shake/zoom dịch cả sàn. Tia sáng che giao điểm lưỡi nên chưa chứng minh chi tiết grip bị che. | chưa kiểm chứng |
| F320 | 18.238000 / 0.066000 | Player hạ thân xoay phải chuẩn bị phản kích, kiếm kéo sau trái, khăn cuộn lên; đối thủ còn duỗi kiếm ngang, +30/tia vàng giữ. | Từ F319 player hạ guard, chân trụ theo sàn sau shake; lưỡi đối thủ vẫn nối tay. | chưa kiểm chứng |
| F321 | 18.304000 / 0.072000 | Impact sao trắng vàng trước đầu kiếm đối thủ, tia cam tản; player giữ thấp với kiếm sau trái và khăn cong lên. | Từ F320 impact mở rộng, camera tiếp hồi; vùng contact vẫn bị sáng che. | chưa kiểm chứng |
| F322 | 18.376000 / 0.067000 | Sao contact thu nhỏ, tia cam tản trái; đối thủ duỗi tay/kiếm, player giữ thấp và +30 còn. | Từ F321 lõi trắng mỏng hơn, không mất đầu/chân hoặc rời chuôi ở phần thấy được. | chưa kiểm chứng |
| F323 | 18.443000 / 0.057000 | Sao nhỏ/cam gần tắt phía trái, đối thủ giữ kiếm ngang; player thấp, khăn cong lên, kiếm kéo sau trái. | Từ F322 lõi trắng gần hết; pose và chân trụ giữ. | chưa kiểm chứng |
| F324 | 18.500000 / 0.050000 | Đối thủ co khuỷu kéo kiếm chéo dựng về sau; player còn thấp chuẩn bị phản kích, +30 mờ và ember bay trái. | Từ F323 bắt đầu recovery đối thủ, bàn chân giữ landmark sàn. | chưa kiểm chứng |
| F325 | 18.550000 / 0.057000 | Giữ kiếm chéo/khủy tay co của đối thủ và player thấp; ember trái thưa, +30 nhạt hơn. | Giữ F324; không có flash/contact mới. | chưa kiểm chứng |
| F326 | 18.607000 / 0.048000 | Đối thủ về idle kiếm dựng và tay thả hơn; player thấp với kiếm sau trái, ember chỉ còn vài hạt. | Từ F325 tay/kiếm enemy về idle; chân không nhảy theo thay cel. | chưa kiểm chứng |
| F327 | 18.655000 / 0.071000 | Giữ enemy idle/kiếm dựng, player thấp và khăn cuộn cao; phần lớn ember đã hết, +30 mờ. | Giữ F326; sàn và bóng dưới chân đọc được, không đổi bên. | chưa kiểm chứng |
| F328 | 18.726000 / 0.039000 | Player giữ thấp/kiếm sau trái, enemy đứng dựng kiếm; +30 gần mất và còn vài ember thấp trái. | Giữ F327; đầu/chân/vũ khí đủ pan, không trượt anchor. | chưa kiểm chứng |
| F329 | 18.765000 / 0.059000 | +30 đã mất; player thấp, khăn cao và lưỡi sau trái, đối thủ idle kiếm dựng; vài ember còn trên sàn. | Từ F328 label điểm tắt, actor không đổi tư thế rõ. | chưa kiểm chứng |
| F330 | 18.824000 / 0.057000 | Đối thủ đưa kiếm sát thân hơn và đổi hướng đầu/cánh tay; player giữ thấp, scarf cong lên. | Từ F329 đổi cel idle enemy, chân và khoảng cách vẫn giữ. | chưa kiểm chứng |
| F331 | 18.881000 / 0.015000 | Giữ pose enemy kiếm gần thân và player thấp; ember cam nhỏ còn ở sàn trái, countdown giảm. | Giữ F330; không thấy jump grip hoặc sorting. | chưa kiểm chứng |
| F332 | 18.896000 / 0.071000 | Giữ đủ đầu, bàn chân và hai lưỡi như F331; player thấp, enemy đứng, ember tiếp mờ. | Giữ F331; không bỏ frame encode ngắn này, chỉ countdown/ember đổi. | chưa kiểm chứng |
| F333 | 18.967000 / 0.081000 | Đối thủ lại dựng kiếm cao, player còn thấp với kiếm sau trái và khăn cuộn. | Từ F332 đổi cel idle enemy; chân không nhảy landmark. | chưa kiểm chứng |
| F334 | 19.048000 / 0.040000 | Player nâng thân nghiêng trước, kiếm ngang sau trái ở tầm vai, khăn cuộn cao; đối thủ đứng. | Từ F333 recovery phản kích nâng người, bàn chân trước giữ điểm trụ. | chưa kiểm chứng |
| F335 | 19.088000 / 0.034000 | Player đứng cao hơn, tay/kiếm thu thành smear ngắn sau trái, khăn vẫn cao; enemy giữ kiếm dựng. | Từ F334 lưỡi thu ngắn theo tay, không thấy mảnh rời ngoài silhouette. | chưa kiểm chứng |
| F336 | 19.122000 / 0.049000 | Giữ player nâng tay với smear ngắn sau trái và enemy idle; chữ ta/take xanh từng phần trong khay. | Giữ F335; chân và bóng ổn định, không flash mới. | chưa kiểm chứng |
| F337 | 19.171000 / 0.065000 | Player về idle tay hạ, scarf trái, smear đen đã hết; chữ ta xanh; enemy đứng đủ kiếm. | Từ F336 kết recovery phản kích, chân giữ landmark và camera/palette thường. | chưa kiểm chứng |
| F338 | 19.236000 / 0.065000 | Player idle khăn cong xuống trái, đối thủ kiếm dựng; toàn đầu/chân/vũ khí đủ, chữ tak xanh, sàn/bóng thường. | Từ F337 thay cel khăn/idle, không nhảy anchor. Cuối đoạn mở rộng không loop: đã về idle trước đòn kế ở trace 19.3427s; không nối về F0. | chưa kiểm chứng |

### Ledger kiểm trực tiếp — `sword-death-ready-R`

Media SHA256 `80c10132fd8e44b64ba0da8c8f16c722db1644e17d2de636676e9d7f120297bc`; revision `66749da1`.
Mọi frame dưới đây đã mở full canvas và nearest 4× không thu nhỏ; frame giữ vẫn tính.
Giới hạn: playback người xem và chi tiết bị che còn chờ, nên chưa chứng nhận toàn clip.

| Frame | PTS / duration (s) | Pha quan sát | Nhận xét full/nearest | Chuyển tiếp từ frame trước | Trạng thái | Bằng chứng tạm trong review/ |
|---|---|---|---|---|---|---|
| F0 | 0.000000 / 0.104000 | Recovery → idle | Player còn mảnh lưỡi ngắn phía trước thấp, khăn cong trái; skeleton kiếm dựng và +12 mờ, hai chân trên gạch/bóng xanh. | Đầu media, không có cặp trước; đủ đầu/chân/vũ khí. | chưa kiểm chứng | `sword-death-ready-R/000000.png` |
| F1 | 0.104000 / 0.066000 | Idle | Player tay sát thân, hết smear, khăn ngang trái; skeleton giữ kiếm chéo lên, +12 đã tắt. | Từ F0 thu hết kiếm/recovery; chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000001.png` |
| F2 | 0.170000 / 0.064000 | Idle | Khăn player vòng cao trái, hai tay gần eo; skeleton giữ dáng đứng/kiếm và bóng xanh. | Từ F1 đổi cel khăn/đầu, không đổi khoảng cách hay điểm chân. | chưa kiểm chứng | `sword-death-ready-R/000002.png` |
| F3 | 0.234000 / 0.069000 | Idle / input | Player đứng khăn vòng trái, skeleton kiếm dựng; chữ t trong two xanh. | Từ F2 đổi cel đầu enemy và tiến độ chữ, bàn chân không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000003.png` |
| F4 | 0.303000 / 0.069000 | Idle / input | Khăn player hạ ngang rồi cong trái; skeleton giữ kiếm dựng, chữ t xanh, countdown/gauge rút. | Từ F3 khăn đổi cel, các chi và lưỡi còn đủ trong pan. | chưa kiểm chứng | `sword-death-ready-R/000004.png` |
| F5 | 0.372000 / 0.015000 | Idle / input | Khăn cuộn lên trái, mắt player và xương enemy đọc được; cả hai giữ chân trên sàn, khay t xanh. | Từ F4 thay cel idle/khăn; không flash/sorting mới. | chưa kiểm chứng | `sword-death-ready-R/000005.png` |
| F6 | 0.387000 / 0.080000 | Idle / input | Giữ khăn vòng trái và skeleton kiếm dựng; chữ tw xanh, gauge/countdown còn. | Từ F5 tiến thêm chữ w, thân/chân không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000006.png` |
| F7 | 0.467000 / 0.073000 | Attack thường / Hurt | Player chém thấp, cung ngà dựng rộng trước skeleton, +13 và tia vàng cam che ngực/grip; hai chân còn trên sàn. | Từ F6 vào contact/chém, camera zoom/shake cùng sàn; không suy chi tiết bị cung/impact che. | chưa kiểm chứng | `sword-death-ready-R/000007.png` |
| F8 | 0.540000 / 0.076000 | Attack hold / flash | Player thấp/cung ngà giữ, skeleton sáng và tia cam thưa hơn, +13 còn. | Từ F7 impact tản; đầu/chân/kiếm đủ pan, giao điểm vẫn che. | chưa kiểm chứng | `sword-death-ready-R/000008.png` |
| F9 | 0.616000 / 0.056000 | Hurt / cung | Cung ngà vẫn che ngực/tay enemy, tia cam đã gần hết; player thấp và khăn kéo lên trái. | Từ F8 flash ngực nhạt hơn, không có mất chân hoặc đổi bên bóng. | chưa kiểm chứng | `sword-death-ready-R/000009.png` |
| F10 | 0.672000 / 1.629000 | Recovery / Hurt | Cung hết, player kéo kiếm thấp; skeleton ngửa với kiếm dựng và +13; sàn/bóng xanh đầy đủ. | Từ F9 tắt cung, lộ silhouette Hurt. PTS tiếp cách1.629s: giữ duration encode; không suy động tác chưa được encode trong khoảng trống. | chưa kiểm chứng | `sword-death-ready-R/000010.png` |
| F11 | 2.301000 / 0.059000 | Slayer idle | Nền đỏ/actor đen; player đứng khăn rủ trái, enemy kiếm chéo đứng, đủ đầu/chân; x1.5 và khay do/those/if. | Từ F10 qua khoảng encode1.629s tới Slayer đã vào; camera lớn hơn và sàn bị phủ, không nhận là chứng minh đủ chuyển động entry trong khoảng không có frame. | chưa kiểm chứng | `sword-death-ready-R/000011.png` |
| F12 | 2.360000 / 0.111000 | Slayer idle | Player đứng/khăn ngắn trái, skeleton đứng kiếm chéo; đen trên đỏ, gauge x1.5 gần cạn, HP enemy thanh tối. | Từ F11 đổi cel khăn/tay, hai chân cùng đáy; sàn/grip không đọc dưới tint. | chưa kiểm chứng | `sword-death-ready-R/000012.png` |
| F13 | 2.471000 / 0.020000 | Slayer idle | Khăn player cong lên trái, skeleton giữ đứng/kiếm, gauge còn vạch vàng nhỏ; đủ đầu/chân. | Từ F12 thay cel khăn; chân không đổi đường đáy silhouette. | chưa kiểm chứng | `sword-death-ready-R/000013.png` |
| F14 | 2.491000 / 0.069000 | Slayer hold | Giữ khăn cong trái và skeleton đứng; gauge nhỏ hơn, x1.5 còn. | Giữ F13, không cung hoặc flash mới; không suy chi tiết tay dưới đen. | chưa kiểm chứng | `sword-death-ready-R/000014.png` |
| F15 | 2.560000 / 0.087000 | Slayer exit | Gauge và x1.5 tắt, nền vẫn đỏ/actor đen; player khăn vòng trái, skeleton đứng. | Từ F14 chỉ bắt đầu đổi HUD/exit; hai silhouette vẫn cùng chân. | chưa kiểm chứng | `sword-death-ready-R/000015.png` |
| F16 | 2.647000 / 0.030000 | Slayer exit | Khăn player kéo dài ngang trái hơn; enemy giữ kiếm chéo, đủ chân/đầu, nền đỏ. | Từ F15 đổi cel khăn/đầu player, không mất actor hoặc đảo sorting. | chưa kiểm chứng | `sword-death-ready-R/000016.png` |
| F17 | 2.677000 / 0.046000 | Slayer exit hold | Giữ player đứng khăn ngang trái và skeleton kiếm chéo; countdown còn. | Giữ F16; sàn và albedo còn bị Slayer phủ, chưa chứng nhận grip. | chưa kiểm chứng | `sword-death-ready-R/000017.png` |
| F18 | 2.723000 / 0.065000 | Slayer exit | Khăn player hất lên trái, thân đứng; skeleton giữ kiếm chéo, hai silhouette đủ đầu/chân. | Từ F17 đổi cel khăn; nền đỏ che landmark sàn/grip. | chưa kiểm chứng | `sword-death-ready-R/000018.png` |
| F19 | 2.788000 / 0.053000 | Slayer exit hold | Giữ khăn lên trái và enemy đứng; khay do/those/if, countdown giảm. | Giữ F18; chân cùng đường đáy, không cung mới. | chưa kiểm chứng | `sword-death-ready-R/000019.png` |
| F20 | 2.841000 / 0.038000 | Slayer exit | Khăn player hạ ngang trái hơn; skeleton không đổi kiếm/đầu, đủ silhouette. | Từ F19 thay cel khăn, không trượt chân nhìn thấy. | chưa kiểm chứng | `sword-death-ready-R/000020.png` |
| F21 | 2.879000 / 0.041000 | Slayer exit hold | Giữ player đứng khăn ngang trái, enemy kiếm chéo, nền đỏ; HUD ổn vị trí. | Giữ F20; countdown rút, không flash. | chưa kiểm chứng | `sword-death-ready-R/000021.png` |
| F22 | 2.920000 / 0.049000 | Slayer exit | Khăn player cuộn trái thành vòng cao, enemy giữ dáng đứng; đầu/chân đủ. | Từ F21 đổi cel khăn/đầu, hai actor vẫn tách. | chưa kiểm chứng | `sword-death-ready-R/000022.png` |
| F23 | 2.969000 / 0.093000 | Slayer exit hold | Vòng khăn trái giữ, enemy kiếm chéo và chân giữ; score333. | Giữ F22; không suy cấu trúc tay dưới tint đen. | chưa kiểm chứng | `sword-death-ready-R/000023.png` |
| F24 | 3.062000 / 0.071000 | Slayer exit | Khăn vòng trái co thấp hơn, player đứng; enemy giữ kiếm dựng chéo, đủ chân. | Từ F23 đổi cel idle/khăn; camera chưa trả palette sàn. | chưa kiểm chứng | `sword-death-ready-R/000024.png` |
| F25 | 3.133000 / 0.057000 | Slayer exit | Khăn mở ngang trái thành nhánh dài, tay player sát thân; enemy giữ dáng đứng. | Từ F24 thay cel khăn; không đổi đường chân silhouette. | chưa kiểm chứng | `sword-death-ready-R/000025.png` |
| F26 | 3.190000 / 0.074000 | Slayer exit | Player hơi quay mặt, khăn dài trái; skeleton giữ đầu/kiếm, countdown ngắn hơn. | Từ F25 thay cel đầu/khăn; không có mất actor/chi. | chưa kiểm chứng | `sword-death-ready-R/000026.png` |
| F27 | 3.264000 / 0.043000 | Slayer exit | Khăn player hất lên trái, enemy đứng kiếm chéo; score333 và cảnh đỏ giữ. | Từ F26 khăn nâng, chân không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000027.png` |
| F28 | 3.307000 / 0.047000 | Slayer exit | Khăn hạ về ngang trái, hai actor đủ toàn đầu/chân trên nền đỏ. | Từ F27 đổi cel khăn, chưa lộ gạch/bóng để kiểm riêng bám sàn. | chưa kiểm chứng | `sword-death-ready-R/000028.png` |
| F29 | 3.354000 / 0.042000 | Slayer exit hold | Giữ khăn ngang trái và enemy kiếm chéo; không cung/flash mới. | Giữ F28, countdown tiếp rút; không coi hold là một vòng idle khác. | chưa kiểm chứng | `sword-death-ready-R/000029.png` |
| F30 | 3.396000 / 0.094000 | Slayer exit hold | Player khăn ngang trái và skeleton kiếm chéo giữ; đủ đầu/chân, score333. | Giữ F29; countdown giảm, sàn/grip vẫn bị phủ. | chưa kiểm chứng | `sword-death-ready-R/000030.png` |
| F31 | 3.490000 / 0.023000 | Slayer exit | Khăn player cuộn vòng trái cao hơn; skeleton giữ đứng, kiếm chéo. | Từ F30 đổi cel khăn; chân cùng đáy silhouette. | chưa kiểm chứng | `sword-death-ready-R/000031.png` |
| F32 | 3.513000 / 0.032000 | Slayer exit hold | Giữ vòng khăn trái, hai actor đứng tách trên nền đỏ; đủ mũi kiếm và chân. | Giữ F31; không flash/cung hoặc nhảy anchor. | chưa kiểm chứng | `sword-death-ready-R/000032.png` |
| F33 | 3.545000 / 0.036000 | Slayer exit | Vòng khăn player co thấp hơn, tay sát thân; enemy đứng/kiếm dựng chéo. | Từ F32 đổi cel khăn/đầu; HUD không dịch. | chưa kiểm chứng | `sword-death-ready-R/000033.png` |
| F34 | 3.581000 / 0.044000 | Slayer exit | Khăn player mở ra ngang trái, phần đầu/cuống khăn giữ đủ; enemy kiếm đứng chéo. | Từ F33 thay cel khăn, không mất chân hay đổi khoảng cách. | chưa kiểm chứng | `sword-death-ready-R/000034.png` |
| F35 | 3.625000 / 0.069000 | Slayer exit hold | Giữ khăn ngang trái và skeleton kiếm chéo; countdown dưới gần cuối. | Giữ F34, không suy chi tiết grip từ đen. | chưa kiểm chứng | `sword-death-ready-R/000035.png` |
| F36 | 3.694000 / 0.023000 | Slayer exit | Player xoay nhẹ đầu, khăn ngang trái có nhánh ngắn dưới; enemy đứng giữ. | Từ F35 đổi cel đầu/khăn; hai chân cùng đường đáy. | chưa kiểm chứng | `sword-death-ready-R/000036.png` |
| F37 | 3.717000 / 0.052000 | Slayer exit | Khăn player hất lên trái, skeleton đứng đủ kiếm và chân; nền đỏ còn. | Từ F36 khăn nâng, không có cung/contact mới. | chưa kiểm chứng | `sword-death-ready-R/000037.png` |
| F38 | 3.769000 / 0.056000 | Slayer exit hold | Giữ khăn lên trái, hai silhouette đứng và kiếm enemy trọn pan. | Giữ F37; thanh countdown rút, không trượt chân rõ. | chưa kiểm chứng | `sword-death-ready-R/000038.png` |
| F39 | 3.825000 / 0.098000 | Slayer exit hold | Player/khăn nâng và enemy kiếm chéo giữ; HUD333/khay do/those/if vẫn ổn. | Giữ F38; sàn chưa hiện để kiểm bóng. | chưa kiểm chứng | `sword-death-ready-R/000039.png` |
| F40 | 3.923000 / 0.014000 | Slayer exit | Khăn player hạ ngang trái, skeleton đứng/kiếm chéo; đủ đầu/chân. | Từ F39 đổi cel khăn; thân/đường chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000040.png` |
| F41 | 3.937000 / 0.055000 | Slayer exit | Khăn player hướng chéo xuống trái hơn; enemy giữ đứng, countdown còn đoạn rất ngắn. | Từ F40 thay cel khăn, không có jump silhouette hoặc mất chi. | chưa kiểm chứng | `sword-death-ready-R/000041.png` |
| F42 | 3.992000 / 0.056000 | Slayer exit hold | Khăn chéo trái, player đứng; enemy giữ kiếm chéo và đủ đầu/chân; countdown gần cạn. | Giữ F41, chỉ thanh dưới rút; không đọc grip dưới đen. | chưa kiểm chứng | `sword-death-ready-R/000042.png` |
| F43 | 4.048000 / 0.033000 | Slayer exit | Khăn cuộn vòng trái cao, enemy đứng giữ; thanh dưới còn vạch nhỏ. | Từ F42 đổi cel khăn, đường chân hai actor không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000043.png` |
| F44 | 4.081000 / 0.038000 | Slayer exit / timer | Player khăn vòng trái và enemy giữ; thanh dưới đầy teal trở lại. | Từ F43 timer đổi khi từ hết thời gian; actor không đổi pose rõ. | chưa kiểm chứng | `sword-death-ready-R/000044.png` |
| F45 | 4.119000 / 0.042000 | Slayer exit / tray | Khăn vòng trái giữ; khay chuyển those/if/but, không còn do. | Từ F44 chữ trượt vị trí theo đổi từ; silhouette/đầu/chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000045.png` |
| F46 | 4.161000 / 0.073000 | Slayer exit | Khăn player mở ngang trái thành hai nhánh, enemy kiếm chéo giữ; khay those/if/but. | Từ F45 thay cel khăn, chân giữ đáy; chưa thấy sàn/bóng. | chưa kiểm chứng | `sword-death-ready-R/000046.png` |
| F47 | 4.234000 / 0.053000 | Slayer exit | Khăn player hất lên trái; skeleton đủ kiếm/đầu/chân, nền đỏ. | Từ F46 khăn nâng, không có cung/flash mới. | chưa kiểm chứng | `sword-death-ready-R/000047.png` |
| F48 | 4.287000 / 0.059000 | Slayer exit | Khăn player hạ ngang trái, actor đen đủ silhouette; khay ổn vị trí. | Từ F47 đổi cel khăn; hai chân không đổi khoảng cách. | chưa kiểm chứng | `sword-death-ready-R/000048.png` |
| F49 | 4.346000 / 0.032000 | Slayer exit hold | Giữ khăn ngang trái, enemy đứng kiếm chéo; countdown rút. | Giữ F48, không mất limb/đảo sorting nhìn thấy. | chưa kiểm chứng | `sword-death-ready-R/000049.png` |
| F50 | 4.378000 / 0.042000 | Slayer exit hold | Hai actor giữ đứng, khăn ngang trái; đủ mũi kiếm/chân trong pan. | Giữ F49; HUD333/khay không dịch. | chưa kiểm chứng | `sword-death-ready-R/000050.png` |
| F51 | 4.420000 / 0.054000 | Slayer exit | Khăn player chéo xuống trái hơn, skeleton đứng giữ kiếm. | Từ F50 thay cel khăn; không nhảy chân so với đường đáy. | chưa kiểm chứng | `sword-death-ready-R/000051.png` |
| F52 | 4.474000 / 0.037000 | Slayer exit hold | Giữ khăn chéo trái, player tay sát thân; enemy kiếm dựng chéo. | Giữ F51, countdown tiếp giảm; sàn/grip chưa đọc được. | chưa kiểm chứng | `sword-death-ready-R/000052.png` |
| F53 | 4.511000 / 0.034000 | Slayer exit | Khăn player cuộn lên trái, enemy đứng và kiếm đủ; cảnh đỏ không đổi. | Từ F52 thay cel khăn; không contact mới. | chưa kiểm chứng | `sword-death-ready-R/000053.png` |
| F54 | 4.545000 / 0.044000 | Slayer exit | Vòng khăn trái mở rộng hơn, thân player đứng, enemy giữ kiếm chéo. | Từ F53 đổi cel khăn/đầu, chân cùng đáy silhouette. | chưa kiểm chứng | `sword-death-ready-R/000054.png` |
| F55 | 4.589000 / 0.066000 | Slayer exit hold | Giữ vòng khăn và enemy đứng; đủ đầu, chân và kiếm; khay those/if/but. | Giữ F54; countdown rút, không bỏ frame giữ. | chưa kiểm chứng | `sword-death-ready-R/000055.png` |
| F56 | 4.655000 / 0.036000 | Slayer exit hold | Khăn vòng trái và skeleton kiếm chéo giữ; HUD còn333. | Giữ F55, không jump silhouette hoặc clipping actor. | chưa kiểm chứng | `sword-death-ready-R/000056.png` |
| F57 | 4.691000 / 0.069000 | Slayer exit hold | Giữ player đứng/khăn vòng trái và enemy kiếm dựng chéo; không flash. | Giữ F56; sàn/bóng vẫn bị nền đỏ che. | chưa kiểm chứng | `sword-death-ready-R/000057.png` |
| F58 | 4.760000 / 0.144000 | Slayer, hai nhân vật đứng | Nền đỏ/đen; khăn cuộn gần vai, hai chân đủ trong pan, kiếm địch chéo lên trái; khay those/if/but còn countdown. | Từ F57 giữ tư thế thân/chân; mép khăn đổi nhẹ, không dịch cả actor. | chưa kiểm chứng | `sword-death-ready-R/000058.png` |
| F59 | 4.904000 / 0.059000 | Slayer idle | Khăn mở ngang trái rồi hất đầu lên; silhouette tay/kiếm địch vẫn liền; sàn và grip bị màu đen che chi tiết. | 58→59 mở vòng khăn; chân và camera giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000059.png` |
| F60 | 4.963000 / 0.090000 | Slayer idle | Đuôi khăn hạ chéo trái, thân người chơi nhô nhẹ; địch giữ kiếm, HP còn vạch rất mỏng. | 59→60 đầu khăn đi xuống; không mất mảng chân hay vũ khí. | chưa kiểm chứng | `sword-death-ready-R/000060.png` |
| F61 | 5.053000 / 0.083000 | Slayer idle | Khăn cuộn trái sát vai; cả hai vẫn đứng, không flash/afterimage; chữ those không bị cắt. | 60→61 đầu khăn cuộn lên; đế chân giữ cùng đường ngang. | chưa kiểm chứng | `sword-death-ready-R/000061.png` |
| F62 | 5.136000 / 0.078000 | Slayer idle | Khăn kéo ngang trái, có khe đỏ dưới phần cổ; đầu và tay địch còn đủ trong pan. | 61→62 vòng khăn mở, silhouette thân giữ gốc. | chưa kiểm chứng | `sword-death-ready-R/000062.png` |
| F63 | 5.214000 / 0.050000 | Slayer idle | Khăn trái hất góc lên; chân người chơi tách rõ trên nền đỏ, countdown tiếp tục co. | 62→63 đổi mép khăn và đầu; không có camera nhảy. | chưa kiểm chứng | `sword-death-ready-R/000063.png` |
| F64 | 5.264000 / 0.068000 | Slayer idle giữ | Giữ hình khăn hất lên và hai actor đứng; nền đỏ/đen tiếp tục che vật liệu/bóng sàn. | 63→64 giữ pose nhìn thấy; chỉ thanh thời gian ngắn hơn. | chưa kiểm chứng | `sword-death-ready-R/000064.png` |
| F65 | 5.332000 / 0.050000 | Slayer idle | Đỉnh đầu người chơi nhô lên, khăn cong lên trái rồi xuống cổ; kiếm địch chưa đổi vị trí. | 64→65 đổi cel idle nhẹ, chân vẫn bám cùng vị trí. | chưa kiểm chứng | `sword-death-ready-R/000065.png` |
| F66 | 5.382000 / 0.019000 | Slayer idle | Khăn cong ngang trái, đầu cel cao; hai chân và kiếm địch đầy đủ, không VFX mới. | 65→66 mở đuôi khăn; gốc chân/camera giữ. | chưa kiểm chứng | `sword-death-ready-R/000066.png` |
| F67 | 5.401000 / 0.071000 | Slayer idle giữ | Silhouette thân và khăn giữ F66; khay those/if/but vẫn nguyên căn. | 66→67 pose giữ, countdown co nhẹ; không mất pixel vũ khí. | chưa kiểm chứng | `sword-death-ready-R/000067.png` |
| F68 | 5.472000 / 0.068000 | Slayer idle | Khăn trái võng xuống; đầu người chơi hạ nhẹ, địch đứng cùng chỗ. | 67→68 đổi cel idle, không dịch đồng loạt cả hình. | chưa kiểm chứng | `sword-death-ready-R/000068.png` |
| F69 | 5.540000 / 0.020000 | Slayer idle | Khăn cuộn sát vai trái, khe đỏ giữa khăn/tay còn đọc được; sàn vẫn bị tông Slayer che. | 68→69 đầu khăn cuộn lên; chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000069.png` |
| F70 | 5.560000 / 0.081000 | Slayer idle giữ | Giữ vòng khăn gần vai và dáng địch; không flash hay bóng tách rời nhìn thấy. | 69→70 giữ pose, thanh thời gian ngắn hơn. | chưa kiểm chứng | `sword-death-ready-R/000070.png` |
| F71 | 5.641000 / 0.038000 | Slayer idle | Khăn mở vòng trái, có vùng đỏ trong vòng; thân người chơi thẳng hơn. | 70→71 mở vòng khăn, chân không trượt khỏi đường cũ. | chưa kiểm chứng | `sword-death-ready-R/000071.png` |
| F72 | 5.679000 / 0.048000 | Slayer idle | Đuôi khăn duỗi ngang trái; đầu/kiếm địch giữ đầy đủ trong pan. | 71→72 vòng mở thành dải, không có cắt đầu/chân. | chưa kiểm chứng | `sword-death-ready-R/000072.png` |
| F73 | 5.727000 / 0.066000 | Slayer idle | Đuôi khăn hất lên trái, thân người chơi nghiêng nhẹ; enemy HP vẫn vạch mỏng. | 72→73 đổi mép khăn và thân nhẹ; camera/palette giữ đỏ đen. | chưa kiểm chứng | `sword-death-ready-R/000073.png` |
| F74 | 5.793000 / 0.064000 | Slayer idle | Khăn dựng cong lên trái, hai actor đứng đen trên đỏ; khay và kiếm không bị cắt. | 73→74 đầu khăn đi lên; chân và khoảng cách hai actor giữ. | chưa kiểm chứng | `sword-death-ready-R/000074.png` |
| F75 | 5.857000 / 0.033000 | Slayer idle | Giữ dáng đầu cao, khăn trái hất nhẹ; tay trái địch nằm bên thân, kiếm liền với tay phải. | 74→75 đổi mép đầu/khăn nhỏ, không có flash mới. | chưa kiểm chứng | `sword-death-ready-R/000075.png` |
| F76 | 5.890000 / 0.056000 | Slayer idle giữ | Khăn vẫn hất trái, viền silhouette có sắc tím/đỏ rất tối; vật liệu bên trong chưa đọc được. | 75→76 pose gần giữ; không thấy lỗ hoặc tách chi mới. | chưa kiểm chứng | `sword-death-ready-R/000076.png` |
| F77 | 5.946000 / 0.051000 | Slayer idle | Đầu người chơi hạ, khăn võng ngang; full canvas vẫn chỉ có hai actor, HUD trên đen. | 76→77 chuyển cel idle, chân giữ mốc; không đổi zoom nhìn thấy. | chưa kiểm chứng | `sword-death-ready-R/000077.png` |
| F78 | 5.997000 / 0.035000 | Slayer idle giữ | Giữ thân và khăn võng của F77; thanh countdown co, those/if/but còn nguyên. | 77→78 giữ pose, không cắt mũi kiếm. | chưa kiểm chứng | `sword-death-ready-R/000078.png` |
| F79 | 6.032000 / 0.063000 | Slayer idle | Đuôi khăn hạ chéo trái; đầu gối người chơi còn phân biệt được với nền đỏ. | 78→79 đầu khăn đi xuống; đế chân và enemy giữ. | chưa kiểm chứng | `sword-death-ready-R/000079.png` |
| F80 | 6.095000 / 0.047000 | Slayer idle | Khăn cuộn lại sát vai, silhouette đầu và kiếm địch không đổi lớn. | 79→80 cuộn khăn lên; camera không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000080.png` |
| F81 | 6.142000 / 0.031000 | Slayer idle | Vòng khăn mở ở trái vai, khe đỏ bên trong rõ; không afterimage/impact mới. | 80→81 vòng khăn mở, chân vẫn cùng vị trí; chi tiết grip/sàn chưa đọc do tint. | chưa kiểm chứng | `sword-death-ready-R/000081.png` |
| F82 | 6.173000 / 0.037000 | Slayer idle | Khăn duỗi ngang trái, khe đỏ dưới cổ; kiếm địch chéo lên đủ mũi. | 81→82 vòng khăn mở thành dải; chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000082.png` |
| F83 | 6.210000 / 0.042000 | Slayer idle | Giữ dải khăn và thân F82, countdown ngắn hơn. | 82→83 giữ pose, không flash/camera đổi. | chưa kiểm chứng | `sword-death-ready-R/000083.png` |
| F84 | 6.252000 / 0.071000 | Slayer idle | Khăn trái hất góc lên, đầu người chơi nghiêng nhẹ; hai actor vẫn đủ chân. | 83→84 đổi cel idle nhẹ, không trượt gốc. | chưa kiểm chứng | `sword-death-ready-R/000084.png` |
| F85 | 6.323000 / 0.015000 | Slayer idle | Đầu người chơi nhô lên, khăn cong dựng trái; enemy HP còn vạch mỏng. | 84→85 đầu/khăn nâng, kiếm địch giữ cùng góc. | chưa kiểm chứng | `sword-death-ready-R/000085.png` |
| F86 | 6.338000 / 0.034000 | Slayer idle | Giữ đầu cao và khăn dựng, thân đen liền; those/if/but không đổi căn. | 85→86 pose giữ, thanh thời gian co. | chưa kiểm chứng | `sword-death-ready-R/000086.png` |
| F87 | 6.372000 / 0.059000 | Slayer idle | Khăn hất lên trái và thân người chơi giữ; không tia hay afterimage mới. | 86→87 giữ hình, camera/palette đỏ đen ổn định. | chưa kiểm chứng | `sword-death-ready-R/000087.png` |
| F88 | 6.431000 / 0.053000 | Slayer idle | Đầu cao, khăn ngắn trái hơi ngang hơn; hai đế chân không mất mảng. | 87→88 mép khăn hạ nhẹ; enemy vẫn đứng. | chưa kiểm chứng | `sword-death-ready-R/000088.png` |
| F89 | 6.484000 / 0.037000 | Slayer idle | Đầu người chơi hạ, khăn võng trái; mũi kiếm địch đầy đủ. | 88→89 đổi cel idle, vị trí chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000089.png` |
| F90 | 6.521000 / 0.103000 | Slayer idle | Đuôi khăn hạ chéo trái; địch giữ kiếm lên, nền/sàn bị Slayer che chi tiết. | 89→90 đầu khăn đi xuống, không mất chi. | chưa kiểm chứng | `sword-death-ready-R/000090.png` |
| F91 | 6.624000 / 0.055000 | Slayer idle | Khăn cuộn gần vai; khe đỏ nhỏ tách khăn khỏi tay người chơi. | 90→91 cuộn lên, không dịch toàn thân. | chưa kiểm chứng | `sword-death-ready-R/000091.png` |
| F92 | 6.679000 / 0.056000 | Slayer idle | Vòng khăn mở trái vai, đế chân hai actor vẫn cùng mốc; countdown gần cạn. | 91→92 vòng khăn mở, camera không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000092.png` |
| F93 | 6.735000 / 0.087000 | Slayer idle | Khăn duỗi ngang trái, gốc cổ vẫn liền; không impact mới. | 92→93 mở vòng thành dải; chân/kiếm giữ. | chưa kiểm chứng | `sword-death-ready-R/000093.png` |
| F94 | 6.822000 / 0.042000 | Slayer idle | Khăn hất góc lên, thân người chơi nghiêng nhẹ; khay chữ không bị cắt. | 93→94 đổi mép khăn, palette giữ. | chưa kiểm chứng | `sword-death-ready-R/000094.png` |
| F95 | 6.864000 / 0.031000 | Slayer idle | Khăn dựng cong trái; đầu player cao hơn, enemy vẫn đứng với kiếm chéo. | 94→95 nâng đầu/khăn; không đổi khoảng cách actor. | chưa kiểm chứng | `sword-death-ready-R/000095.png` |
| F96 | 6.895000 / 0.050000 | Slayer idle | Khăn cong trái hơi ngang, hai actor đen liền; countdown chỉ còn vạch ngắn. | 95→96 đổi mép khăn nhỏ, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000096.png` |
| F97 | 6.945000 / 0.051000 | Slayer idle | Đầu hạ nhẹ, khăn ngang cong; không flash/afterimage hay sprite ngoài pan. | 96→97 cel idle tiếp nối; grip/bóng sàn chưa đọc do tint. | chưa kiểm chứng | `sword-death-ready-R/000097.png` |
| F98 | 6.996000 / 0.033000 | Slayer idle | Khăn võng ngang trái, cả hai actor đứng; countdown còn vạch nhỏ cuối. | 97→98 hạ đầu/khăn nhẹ; không cắt mũi kiếm hoặc chân. | chưa kiểm chứng | `sword-death-ready-R/000098.png` |
| F99 | 7.029000 / 0.067000 | Slayer idle | Khăn hạ chéo trái; biểu tượng thanh dưới HP địch đổi từ cảnh báo sang khiên xanh. | 98→99 đổi cel idle và icon; thân/chân vẫn tại chỗ. | chưa kiểm chứng | `sword-death-ready-R/000099.png` |
| F100 | 7.096000 / 0.067000 | Slayer, đổi từ | those biến mất, if/but còn; countdown và thanh khiên địch đầy lại; hai actor chưa ra đòn. | 99→100 thay UI khi timer cạn, không flash hoặc dịch actor. | chưa kiểm chứng | `sword-death-ready-R/000100.png` |
| F101 | 7.163000 / 0.048000 | Slayer exit bắt đầu | Khay if/but/in và viền hiện lại; đỏ sẫm hơn, actor bắt đầu nhỏ lại; cả hai vẫn đầy chân. | 100→101 zoom lùi và khay trở lại, không cắt đầu/vũ khí. | chưa kiểm chứng | `sword-death-ready-R/000101.png` |
| F102 | 7.211000 / 0.052000 | Slayer exit | Địch về cel idle kiếm gần thẳng hơn; player đứng, nền đỏ tối; có đường viền khay dưới chân. | 101→102 thu camera thêm, địch đổi pose; gốc chân dịch cùng khung cảnh. | chưa kiểm chứng | `sword-death-ready-R/000102.png` |
| F103 | 7.263000 / 0.042000 | Slayer exit | Đã thấy mờ xương sườn, mặt player và đuốc; actor nhỏ hơn, hai chân không mất mảng. | 102→103 palette mở dần và zoom lùi; không actor/afterimage dư. | chưa kiểm chứng | `sword-death-ready-R/000103.png` |
| F104 | 7.305000 / 0.098000 | Slayer exit | Xương địch và tóc/khăn player rõ hơn; sàn tím/vết gạch hiện, bóng sát chân còn rất tối. | 103→104 tông đỏ giảm, khung cảnh và actor thu đều; không nhảy sorting. | chưa kiểm chứng | `sword-death-ready-R/000104.png` |
| F105 | 7.403000 / 0.056000 | Slayer exit | Thấy đầy mũi kiếm, xương sườn, bàn tay và hai đế chân; đuốc/sàn hiện rõ dưới lớp đỏ. | 104→105 zoom tiếp tục lùi, chân vẫn nằm trên sàn; palette chưa bình thường hoàn toàn. | chưa kiểm chứng | `sword-death-ready-R/000105.png` |
| F106 | 7.459000 / 0.087000 | Slayer exit | Khăn cuộn trái, xương và kiếm liền; camera lùi thêm, nền đỏ vẫn phủ nhưng gạch sàn rõ. | 105→106 giảm zoom và tăng ánh sáng; chân cùng nền, không mất actor. | chưa kiểm chứng | `sword-death-ready-R/000106.png` |
| F107 | 7.546000 / 0.082000 | Exit cuối / chuẩn bị đòn | SPACE xuất hiện trên địch; chữ i trong if xanh; player đứng, khay và viền rõ. | 106→107 màu gần dungeon thường, camera lùi; chỉ prompt bị chạm mép pan trên, full canvas thấy đủ. | chưa kiểm chứng | `sword-death-ready-R/000107.png` |
| F108 | 7.628000 / 0.057000 | Đòn kết liễu / Die sớm | Player lao thấp, kiếm ngang sát sàn liền tay/chuôi xanh; +10 trên đầu địch, HP hết và điểm343; xương địch bắt đầu tách tay phải phía ảnh. | 107→108 hạ người và ra kiếm; palette thường trở lại, chưa thấy cung che, sorting đòn trước chân. | chưa kiểm chứng | `sword-death-ready-R/000108.png` |
| F109 | 7.685000 / 0.074000 | Death, rã xương sớm | Player giữ thấp, khăn hất sau; cụm tay/xương rời sang phải, đầu/sườn vẫn phía trên; +10 còn. | 108→109 cụm xương rời đi phải, camera rung ngang; chân và kiếm player không cắt. | chưa kiểm chứng | `sword-death-ready-R/000109.png` |
| F110 | 7.759000 / 0.073000 | Death, rã xương | Tay rời cao hơn bên phải, hông tách khỏi sườn; player vẫn cầm kiếm thấp, vạch khay giữ đầy. | 109→110 xương tách tiếp trong khi camera hồi; không mảng kiếm đứt khỏi chuôi. | chưa kiểm chứng | `sword-death-ready-R/000110.png` |
| F111 | 7.832000 / 0.036000 | Death, rã xương | Cụm xương rời xoay ngoài bên phải thân; +10 mờ dần; player cúi, khăn ngắn lại. | 110→111 thân xương co lại, cụm tay dịch phải; chân player vẫn bám sàn nhìn thấy. | chưa kiểm chứng | `sword-death-ready-R/000111.png` |
| F112 | 7.868000 / 0.103000 | Death, rã xương | Đầu tách rõ khỏi sườn, xương tay ngoài phải; kiếm địch vẫn trên tay trái ảnh; không flash che đầu. | 111→112 đầu/hông rời, thân player bắt đầu hồi; actor đủ trong pan. | chưa kiểm chứng | `sword-death-ready-R/000112.png` |
| F113 | 7.971000 / 0.019000 | Death, rã xương | Sườn, hông, chân địch rời nhau; đầu nổi trên, cụm tay xa phải còn trọn ở full/pan; player nâng đầu nhưng kiếm vẫn thấp. | 112→113 khoảng cách các mảnh tăng, không teleport cả cụm; camera ổn định hơn. | chưa kiểm chứng | `sword-death-ready-R/000113.png` |
| F114 | 7.990000 / 0.059000 | Death, rã xương giữ | Đầu/sườn/hông địch tách, player còn thấp; pan phải bổ sung thấy hạt xương cuối sát x950, không actor khác. | 113→114 pose gần giữ, hạt rời đi phải; +10 mờ hơn. | chưa kiểm chứng | `sword-death-ready-R/000114.png` |
| F115 | 8.049000 / 0.054000 | Death / player hồi | Player nâng người, kiếm thu sát tay có vệt trắng ngắn; đầu địch giữ cao, chân/hông rơi; pan phải thấy hạt tách rải xuống. | 114→115 thu kiếm và nâng thân, hạt ngoài pan cũ không biến mất đột ngột. | chưa kiểm chứng | `sword-death-ready-R/000115.png` |
| F116 | 8.103000 / 0.081000 | Death, rơi xương | Giữ tư thế hồi, hông/chân đã gần sàn; pan phải thấy cụm hạt lan rộng và xuống, không flash che. | 115→116 đầu/sườn còn giữ, hạt phải rơi tiếp; +10 sẫm hơn. | chưa kiểm chứng | `sword-death-ready-R/000116.png` |
| F117 | 8.184000 / 0.049000 | Death / player gần idle | Player đứng thẳng hơn, kiếm thu thành vệt nhỏ bên phải tay; sườn/tay địch hạ, hông chạm sàn; hạt phải xoay và rơi. | 116→117 hạ sườn, hồi người chơi; đế player giữ mốc gạch. | chưa kiểm chứng | `sword-death-ready-R/000117.png` |
| F118 | 8.233000 / 0.036000 | Death / player idle | Player về dáng đứng, khăn quét trái, không còn kiếm ngang sàn; đầu địch vẫn lơ lửng, tay/sườn tách và bụi phải thấp dần. | 117→118 hết vệt kiếm hồi; thân không trượt, hạt ngoài pan cũ đã xem thêm. | chưa kiểm chứng | `sword-death-ready-R/000118.png` |
| F119 | 8.269000 / 0.056000 | Death, thân đổ | Đầu địch thấp hơn, hai tay duỗi ngang, sườn gần sàn; player đứng với khăn duỗi trái; pan phải thấy hạt nâu rải xuống. | 118→119 sườn/tay hạ cùng, đầu vẫn tách; không đổi camera lớn. | chưa kiểm chứng | `sword-death-ready-R/000119.png` |
| F120 | 8.325000 / 0.093000 | Death, thân đổ giữ | Giữ dáng tay ngang/sườn thấp; đầu trên thân, chân nằm sàn; các hạt ngoài phải tiếp tục hạ. | 119→120 pose thân giữ, bụi dịch phải/xuống; viền khay ổn định. | chưa kiểm chứng | `sword-death-ready-R/000120.png` |
| F121 | 8.418000 / 0.040000 | Death, đầu rơi | Đầu địch xuống gần đống sườn trên sàn; player đứng, khăn chéo trái; pan phải thấy hạt cuối dưới x1150 và trên sàn. | 120→121 đầu hạ rõ, thân/xương nằm lại; không cắt đầu hay bàn chân trong full canvas. | chưa kiểm chứng | `sword-death-ready-R/000121.png` |
| F122 | 8.458000 / 0.066000 | Death, đầu chạm đống xương | Đầu gần đống sườn; kiếm nằm trái, player idle khăn chéo trái; pan rộng thấy bụi phải tiếp tục xuống. | 121→122 đầu hạ, phần thân đã nằm giữ. | chưa kiểm chứng | `sword-death-ready-R/000122.png` |
| F123 | 8.524000 / 0.037000 | Death, đầu dựng trên sàn | Đầu dựng trên đống xương, kiếm nằm nguyên; khăn player cuộn gần vai; bụi phải thưa và thấp hơn. | 122→123 đầu giữ dựng, hạt rơi tiếp. | chưa kiểm chứng | `sword-death-ready-R/000123.png` |
| F124 | 8.561000 / 0.069000 | Death, đầu đổ | Đầu nghiêng xuống đống xương; player idle khăn cuộn, các hạt bên phải còn đọc được trong pan rộng. | 123→124 đầu xoay xuống, không trượt cả đống xương. | chưa kiểm chứng | `sword-death-ready-R/000124.png` |
| F125 | 8.630000 / 0.068000 | Death, đống xương giữ | Đầu nằm phẳng, kiếm/sườn sát sàn; player đứng, khăn ngắn; bụi phải ít và tối hơn. | 124→125 giữ đống xương, bụi mờ dần. | chưa kiểm chứng | `sword-death-ready-R/000125.png` |
| F126 | 8.698000 / 0.048000 | Death, đống xương giữ | Player nâng đầu, khăn cuộn lên; đống xương giữ, nhiều hạt phải đã biến mất. | 125→126 đổi cel idle nhỏ, bụi tắt dần. | chưa kiểm chứng | `sword-death-ready-R/000126.png` |
| F127 | 8.746000 / 0.048000 | Death, đống xương giữ | Player đầu cao, khăn duỗi trái đầu cong lên; xương và kiếm nằm cố định, bụi còn ít gần cột phải. | 126→127 khăn mở ra, hạt phải mờ hơn. | chưa kiểm chứng | `sword-death-ready-R/000127.png` |
| F128 | 8.794000 / 0.036000 | Death, đống xương giữ | Player đầu cao, khăn trái hướng lên; đống xương giữ, vài hạt phải rất tối. | 127→128 giữ tư thế, bụi tiếp tục tắt. | chưa kiểm chứng | `sword-death-ready-R/000128.png` |
| F129 | 8.830000 / 0.051000 | Death, đống xương giữ | Player đứng, khăn trái hướng lên; hạt nâu cuối thấp gần cột phải, khay vẫn but/in và countdown đầy. | 128→129 đống xương giữ, hạt cuối mờ; chưa READY. | chưa kiểm chứng | `sword-death-ready-R/000129.png` |
| F130 | 8.881000 / 0.064000 | Death, giữ trước cleanup | Đống xương/kiếm vẫn trên sàn; player đứng khăn trái, khay but/in, HP địch rỗng và score343. | 129→130 khăn hạ nhẹ, đống xương giữ. | chưa kiểm chứng | `sword-death-ready-R/000130.png` |
| F131 | 8.945000 / 0.042000 | Death, giữ trước cleanup | Xương/kiếm không đổi; khăn nâng rộng trái, player còn idle và countdown đầy. | 130→131 đổi cel khăn, không dịch chân hay camera. | chưa kiểm chứng | `sword-death-ready-R/000131.png` |
| F132 | 8.987000 / 0.054000 | Cleanup → READY | Xương và kiếm biến mất cùng; READY màu ngà nằm trái khay có padding, viền giữ, countdown ẩn; không card cũ. | 131→132 cleanup và đổi khay trong cùng frame quan sát; actor player giữ mốc sàn. | chưa kiểm chứng | `sword-death-ready-R/000132.png` |
| F133 | 9.041000 / 0.049000 | READY chờ | READY đủ năm chữ không cắt, viền trên/trái sáng dưới tối; player idle khăn cong gần vai, score343 giữ. | 132→133 chữ và khay giữ vị trí; khăn co lại, không tự chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000133.png` |
| F134 | 9.090000 / 0.073000 | READY chờ | Player đầu thấp, khăn tạo vòng rộng trái; READY ngà đủ chữ, padding và viền khay không cắt. | 133→134 khăn mở vòng, chân và chữ giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000134.png` |
| F135 | 9.163000 / 0.073000 | READY chờ | Khăn thu thành dải ngắn phía vai, đầu player hơi nâng; READY vẫn ngà, countdown không xuất hiện. | 134→135 khăn co, khay giữ nguyên. | chưa kiểm chứng | `sword-death-ready-R/000135.png` |
| F136 | 9.236000 / 0.051000 | READY chờ | Khăn vươn trái đầu hướng lên; player đứng, READY giữ rõ trong khay, score343 và thanh HP giữ. | 135→136 khăn mở trái, không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000136.png` |
| F137 | 9.287000 / 0.035000 | READY chờ | Giữ dáng khăn vươn trái, đầu cao; READY không đổi màu hoặc bị cắt, không card/nút giữa màn. | 136→137 giữ cel gần giống, chỉ ánh đuốc thay nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000137.png` |
| F138 | 9.322000 / 0.068000 | READY chờ | Player xoay mặt thấp hơn, khăn chéo trái; READY cùng baseline và padding, countdown ẩn. | 137→138 đầu/khăn đổi cel, mốc chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000138.png` |
| F139 | 9.390000 / 0.086000 | READY chờ | Player đầu cao trở lại, khăn gợn trái; chữ ngà đủ READY, viền khay thẳng, score343 giữ. | 138→139 đổi cel idle, chữ không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000139.png` |
| F140 | 9.476000 / 0.029000 | READY chờ | Khăn dài chéo trái, đầu player quay phải; READY đầy đủ, không thanh countdown dưới chữ. | 139→140 khăn nhấc, không đổi bố cục khay. | chưa kiểm chứng | `sword-death-ready-R/000140.png` |
| F141 | 9.505000 / 0.042000 | READY chờ | Player và khăn giữ gần F140; READY ngà, viền vàng trên/trái và đáy tối giữ nguyên. | 140→141 giữ pose, chỉ ánh đuốc nhấp nhẹ; không tự tiếp trận. | chưa kiểm chứng | `sword-death-ready-R/000141.png` |
| F142 | 9.547000 / 0.037000 | READY chờ | Khăn cuộn vòng lên trái, player đứng trên cùng gạch; READY giữ vị trí và khoảng lề. | 141→142 khăn cuộn, đầu hơi hạ; không xê dịch khay. | chưa kiểm chứng | `sword-death-ready-R/000142.png` |
| F143 | 9.584000 / 0.097000 | READY chờ | Khăn mở vòng rộng dưới vai; READY ngà đọc đủ, HP địch rỗng, score343. | 142→143 khăn hạ vòng, chữ và camera giữ. | chưa kiểm chứng | `sword-death-ready-R/000143.png` |
| F144 | 9.681000 / 0.132000 | READY chờ | Player đầu nâng, khăn ngắn bên vai; READY còn nguyên năm chữ, countdown ẩn. | 143→144 vòng khăn co, chân giữ mốc sàn. | chưa kiểm chứng | `sword-death-ready-R/000144.png` |
| F145 | 9.813000 / 0.122000 | READY chờ | Khăn vươn trái đầu cong lên, player đứng thẳng; khay READY giữ màu ngà và padding. | 144→145 khăn mở trái, không có intro hay card mới. | chưa kiểm chứng | `sword-death-ready-R/000145.png` |
| F146 | 9.935000 / 0.050000 | READY chờ | Player đầu cao, khăn chéo trái xuống; READY không cắt cạnh hoặc đổi baseline. | 145→146 khăn hạ, thân hơi đổi cel; khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000146.png` |
| F147 | 9.985000 / 0.173000 | READY chờ | Giữ tư thế đầu cao khăn chéo trái; READY đủ chữ trong khay, countdown ẩn và score343. | 146→147 pose gần giữ, viền và chữ không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000147.png` |
| F148 | 10.158000 / 0.159000 | READY chờ | Player mặt hạ, khăn cuộn trái; READY ngà nguyên chữ và padding, countdown ẩn, không intro. | 147→148 đầu hạ và khăn co; giữ duration encode, không nội suy frame. | chưa kiểm chứng | `sword-death-ready-R/000148.png` |
| F149 | 10.317000 / 0.048000 | READY chờ | Đầu player nâng, khăn vươn trái; READY giữ baseline, score343 không đổi. | 148→149 khăn mở, chân đứng đúng mốc. | chưa kiểm chứng | `sword-death-ready-R/000149.png` |
| F150 | 10.365000 / 0.141000 | READY chờ | Player đầu hơi quay xuống, khăn chéo trái; READY giữ màu ngà, viền khay không cắt. | 149→150 đổi mặt/khăn nhẹ, bố cục khay cố định. | chưa kiểm chứng | `sword-death-ready-R/000150.png` |
| F151 | 10.506000 / 0.055000 | READY chờ | Player đầu cao và khăn chéo xuống trái; READY đủ năm chữ, không countdown hoặc card. | 150→151 đầu nâng, khăn hạ nhẹ; không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000151.png` |
| F152 | 10.561000 / 0.067000 | READY chờ | Khăn cuộn vòng lên trái, player mặt phải; READY giữ vị trí và màu, HP giữ. | 151→152 khăn cuộn, chân giữ trên gạch. | chưa kiểm chứng | `sword-death-ready-R/000152.png` |
| F153 | 10.628000 / 0.044000 | READY chờ | Khăn mở thành vòng rộng bên vai; READY ngà đọc đủ, các cạnh khay giữ thẳng. | 152→153 khăn hạ vòng, không rung chữ hay camera. | chưa kiểm chứng | `sword-death-ready-R/000153.png` |
| F154 | 10.672000 / 0.152000 | READY chờ | Player mặt thấp, khăn cuộn ngắn; READY không thay đổi khoảng lề và baseline. | 153→154 khăn co, đầu hạ; khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000154.png` |
| F155 | 10.824000 / 0.119000 | READY chờ | Player đầu cao, khăn vươn trái đầu cong lên; READY còn nguyên, countdown tiếp tục ẩn. | 154→155 khăn mở trái; score343 và cảnh chờ giữ. | chưa kiểm chứng | `sword-death-ready-R/000155.png` |
| F156 | 10.943000 / 0.037000 | READY chờ | Player đầu cao, khăn gợn trái; READY ngà đủ chữ, viền và padding giữ, countdown ẩn. | 155→156 khăn hạ và đổi cel mặt; không tiếp trận. | chưa kiểm chứng | `sword-death-ready-R/000156.png` |
| F157 | 10.980000 / 0.204000 | READY chờ | Đầu player nâng, khăn chéo xuống trái; READY cùng baseline, score343 và HP không đổi. | 156→157 khăn nhấc lên vai, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000157.png` |
| F158 | 11.184000 / 0.076000 | READY chờ | Khăn cuộn vòng rộng trái, player đứng; READY giữ màu ngà và khoảng lề, không card. | 157→158 khăn mở vòng; chữ và camera giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000158.png` |
| F159 | 11.260000 / 0.174000 | READY chờ trước resize | Player đầu cao, khăn dạng dải trái; READY đủ chữ, không countdown, khay vẫn 1280×720. | 158→159 khăn co vòng; bố cục khay chưa thay. | chưa kiểm chứng | `sword-death-ready-R/000159.png` |
| F160 | 11.434000 / 0.028000 | READY / resize 1366×768 | Canvas lớn hơn, HUD và khay phân bố lại; READY ngà đủ chữ, padding trái/trên rõ, viền không cắt; player vẫn đứng trên sàn. | 159→160 resize làm cảnh/HUD đổi kích thước và vị trí đồng thời; không tự vào trận. | chưa kiểm chứng | `sword-death-ready-R/000160.png` |
| F161 | 11.462000 / 0.077000 | READY 1366×768 | Giữ player/khăn giống F160, chữ READY và khay ổn định, countdown ẩn; không nút cũ. | 160→161 giữ pose và bố cục sau resize, ánh đuốc thay nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000161.png` |
| F162 | 11.539000 / 0.044000 | READY 1366×768 | Player hạ đầu và khăn chéo xuống trái; READY đủ chữ không cắt, viền khay giữ. | 161→162 đổi cel idle, chân giữ mốc gạch mới sau resize. | chưa kiểm chứng | `sword-death-ready-R/000162.png` |
| F163 | 11.583000 / 0.052000 | READY 1366×768 | Player/khăn gần giữ F162; READY ngà cùng padding, HUD score343 giữ và không intro. | 162→163 giữ cel, không nhảy chữ hoặc khay. | chưa kiểm chứng | `sword-death-ready-R/000163.png` |
| F164 | 11.635000 / 0.072000 | READY 1366×768 | Đầu player thấp, khăn cuộn vòng rộng trái; READY đủ chữ ngà, padding và viền giữ, countdown ẩn. | 163→164 khăn từ chéo sang cuộn vòng, chân giữ mốc sàn. | chưa kiểm chứng | `sword-death-ready-R/000164.png` |
| F165 | 11.707000 / 0.080000 | READY 1366×768 | Mặt player thấp phía trước, vòng khăn hạ dưới vai trái; READY và HUD giữ vị trí. | 164→165 đầu hạ, vòng khăn hạ; không intro. | chưa kiểm chứng | `sword-death-ready-R/000165.png` |
| F166 | 11.787000 / 0.118000 | READY 1366×768 | Đầu nâng sang phải, khăn co thành dải ngắn trái; READY không cắt chữ. | 165→166 vòng khăn khép và đầu nâng, chân đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000166.png` |
| F167 | 11.905000 / 0.041000 | READY 1366×768 | Đầu cao, đuôi khăn cong lên trái; READY giữ màu và khoảng lề. | 166→167 khăn duỗi trái và cong lên, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000167.png` |
| F168 | 11.946000 / 0.044000 | READY 1366×768 | Đầu cao, khăn gợn ngang trái; READY ngà đầy đủ, countdown ẩn. | 167→168 đuôi khăn hạ phẳng, không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000168.png` |
| F169 | 11.990000 / 0.054000 | READY 1366×768 | Đầu cao, khăn chéo xuống trái; chân và chữ READY giữ mốc. | 168→169 khăn nâng tại vai và hạ ở đuôi. | chưa kiểm chứng | `sword-death-ready-R/000169.png` |
| F170 | 12.044000 / 1.008000 | READY 1366×768 | Pose đầu và khăn giữ; vùng sáng sàn sáng hơn và mép sáng đổi lấy mẫu; READY vẫn đọc rõ. | 169→170 giữ pose, ánh sáng sàn thay; không nhảy chữ. | chưa kiểm chứng | `sword-death-ready-R/000170.png` |
| F171 | 13.052000 / 0.158000 | READY / về 1280×720 | Canvas về 1280×720, HUD/khay trở lại bố cục trước F160; READY đủ chữ ngà, padding và viền giữ, countdown ẩn. Player đầu cao, khăn chéo trái. | 170→171 resize đồng thời cảnh/HUD; khoảng encode 1,008 s được giữ nguyên, không suy diễn frame trung gian. | chưa kiểm chứng | `sword-death-ready-R/000171.png` |
| F172 | 13.210000 / 0.015000 | READY chờ 1280×720 | Player đầu cao, khăn cuộn vòng rộng trái; READY ngà đủ chữ, baseline và padding giữ, countdown ẩn. | 171→172 khăn cuộn vòng; chân giữ gạch, không intro. | chưa kiểm chứng | `sword-death-ready-R/000172.png` |
| F173 | 13.225000 / 0.059000 | READY chờ 1280×720 | Pose đầu và vòng khăn giữ F172; khay/READY đứng yên, score343 giữ. | 172→173 giữ cel, ánh đuốc đổi nhỏ; không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000173.png` |
| F174 | 13.284000 / 0.054000 | READY chờ 1280×720 | Đầu player hạ, khăn co ngắn trái; READY đủ chữ, viền khay liên tục. | 173→174 vòng khăn khép, đầu hạ; chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000174.png` |
| F175 | 13.338000 / 0.039000 | READY chờ 1280×720 | Đầu và khăn ngắn giữ, READY cùng vị trí và màu, countdown ẩn. | 174→175 giữ cel idle; vùng sáng sàn đổi nhẹ. | chưa kiểm chứng | `sword-death-ready-R/000175.png` |
| F176 | 13.377000 / 0.100000 | READY chờ 1280×720 | Khăn kéo dài trái, đầu đuôi cong lên; đầu player vẫn thấp, READY đọc đủ. | 175→176 khăn duỗi sang trái, tay/chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000176.png` |
| F177 | 13.477000 / 0.046000 | READY chờ 1280×720 | Đầu nâng, khăn uốn thấp bên trái; READY và padding giữ. | 176→177 đầu nâng, vòng khăn hạ ở giữa; không intro. | chưa kiểm chứng | `sword-death-ready-R/000177.png` |
| F178 | 13.523000 / 0.042000 | READY chờ 1280×720 | Mặt player cao, khăn chéo xuống trái với nếp sáng gần vai; READY ngà không cắt. | 177→178 khăn nâng tại vai và thả đuôi trái; chân đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000178.png` |
| F179 | 13.565000 / 0.082000 | READY chờ 1280×720 | Khăn chéo xuống trái mở rộng, mặt player cao; READY cùng baseline, không countdown/card. | 178→179 khăn duỗi và đổi nếp, HUD/khay giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000179.png` |
| F180 | 13.647000 / 0.023000 | READY chờ | Mặt player cao, khăn chéo trái; READY ngà đầy đủ, khay và chân giữ vị trí. | 179→180 nếp khăn đổi nhỏ, countdown vẫn ẩn. | chưa kiểm chứng | `sword-death-ready-R/000180.png` |
| F181 | 13.670000 / 0.848000 | READY chờ | Khăn cong vòng trái ở dưới vai, đầu cao; READY cùng baseline, score343 giữ. | 180→181 khăn mở vòng; không intro hoặc card. | chưa kiểm chứng | `sword-death-ready-R/000181.png` |
| F182 | 14.518000 / 0.119000 | READY chờ | Đầu nâng, khăn uốn thấp trái; READY đủ chữ, padding giữ. | 181→182 đổi cel khăn/đầu sau khoảng encode 0,848 s; không suy diễn frame thiếu. | chưa kiểm chứng | `sword-death-ready-R/000182.png` |
| F183 | 14.637000 / 0.050000 | READY chờ | Player đầu cao, khăn chéo xuống trái; READY vẫn ngà và nằm trọn trong viền khay. | 182→183 khăn nâng gần vai, đuôi trái hạ; chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000183.png` |
| F184 | 14.687000 / 0.063000 | READY chờ | Pose và khăn chéo giữ F183, READY không đổi màu/vị trí, countdown ẩn. | 183→184 giữ cel, ánh đuốc dao động nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000184.png` |
| F185 | 14.750000 / 0.077000 | READY chờ | Đầu player hạ, khăn cuộn vòng rộng trái; READY đủ chữ, HUD giữ. | 184→185 khăn mở vòng và đầu hạ, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000185.png` |
| F186 | 14.827000 / 0.065000 | READY chờ | Mặt player nâng, vòng khăn rộng giữ; READY ngà cùng padding, không card cũ. | 185→186 đầu nâng và vòng khăn hạ nhẹ; không tiếp trận. | chưa kiểm chứng | `sword-death-ready-R/000186.png` |
| F187 | 14.892000 / 0.056000 | READY chờ | Khăn co thành dải ngắn trái, mặt player cao; READY vẫn đủ chữ và countdown ẩn. | 186→187 khăn khép vòng; khay và camera giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000187.png` |
| F188 | 14.948000 / 0.030000 | READY chờ | Đầu player cao, khăn dài trái cong lên ở đuôi; READY ngà đủ chữ và padding giữ. | 187→188 khăn kéo dài và cong lên, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000188.png` |
| F189 | 14.978000 / 0.050000 | READY chờ | Đầu và khăn cong trái giữ F188; countdown ẩn, khay không dịch. | 188→189 giữ cel, không tự vào trận. | chưa kiểm chứng | `sword-death-ready-R/000189.png` |
| F190 | 15.028000 / 0.053000 | READY chờ | Đầu nâng phía trước, khăn uốn thấp trái; READY vẫn đủ chữ, score343 giữ. | 189→190 đầu và nếp khăn đổi, chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000190.png` |
| F191 | 15.081000 / 0.058000 | READY chờ | Mặt player cao, khăn chéo xuống trái sáng gần vai; READY cùng baseline và màu. | 190→191 khăn nâng sát vai và thả đuôi; không intro. | chưa kiểm chứng | `sword-death-ready-R/000191.png` |
| F192 | 15.139000 / 0.066000 | READY chờ | Khăn chéo xuống trái mở rộng, mặt cao; READY không cắt, countdown ẩn. | 191→192 khăn duỗi, camera và HUD giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000192.png` |
| F193 | 15.205000 / 0.061000 | READY chờ | Khăn cuộn vòng trái, player đứng đầu cao; READY vẫn đủ chữ trong viền khay. | 192→193 khăn mở vòng dưới vai, chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000193.png` |
| F194 | 15.266000 / 0.053000 | READY chờ | Đầu player hạ, vòng khăn rộng giữ; READY ngà, padding không đổi. | 193→194 đầu hạ, vòng khăn hạ nhẹ, không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000194.png` |
| F195 | 15.319000 / 0.032000 | READY chờ | Đầu nâng, vòng khăn rộng giữ; READY ngà đầy đủ, score343 và HP không đổi. | 194→195 đầu nâng và khăn đổi nếp nhỏ, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000195.png` |
| F196 | 15.351000 / 0.050000 | READY chờ | Mặt player cao, khăn co ngắn trái; READY ngà và khay giữ padding, countdown ẩn. | 195→196 vòng khăn khép, chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000196.png` |
| F197 | 15.401000 / 0.036000 | READY chờ | Đầu và khăn ngắn giữ F196; READY cùng baseline, không intro. | 196→197 giữ cel, ánh đuốc đổi nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000197.png` |
| F198 | 15.437000 / 0.057000 | READY chờ | Khăn dài cong lên ở đuôi trái; mặt cao, READY vẫn đủ chữ trong khay. | 197→198 khăn duỗi trái, tay và chân giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000198.png` |
| F199 | 15.494000 / 0.062000 | READY chờ | Khăn cong trái và đầu giữ; READY ngà không cắt, countdown ẩn. | 198→199 giữ cel, khay không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000199.png` |
| F200 | 15.556000 / 0.067000 | READY chờ | Đầu nâng phía trước, khăn uốn thấp trái; READY vẫn đọc đủ, score343 giữ. | 199→200 đầu/khăn đổi cel, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000200.png` |
| F201 | 15.623000 / 0.046000 | READY chờ | Mặt player cao, khăn chéo xuống trái sáng gần vai; READY cùng baseline. | 200→201 khăn nâng gần vai và thả đuôi; không intro. | chưa kiểm chứng | `sword-death-ready-R/000201.png` |
| F202 | 15.669000 / 0.049000 | READY chờ | Khăn chéo trái mở rộng, mặt cao; READY đủ chữ ngà và padding giữ. | 201→202 khăn duỗi; camera/HUD không dịch. | chưa kiểm chứng | `sword-death-ready-R/000202.png` |
| F203 | 15.718000 / 0.042000 | READY chờ | Player đầu cao, khăn cuộn vòng rộng trái; READY không đổi màu/vị trí, countdown ẩn. | 202→203 khăn mở vòng dưới vai, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000203.png` |
| F204 | 15.760000 / 0.018000 | READY chờ | Đầu hạ, khăn vòng rộng trái; READY ngà đủ chữ, countdown ẩn. | 203→204 đầu và vòng khăn hạ, chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000204.png` |
| F205 | 15.778000 / 0.054000 | READY chờ | Đầu nâng, khăn vòng trái cao hơn; khay giữ padding và baseline. | 204→205 đầu/khăn nâng, camera đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000205.png` |
| F206 | 15.832000 / 0.050000 | READY chờ | Mặt hướng trước, khăn vòng trái thấp; READY không đổi màu, score343 giữ. | 205→206 mặt xoay trước, vòng khăn hạ. | chưa kiểm chứng | `sword-death-ready-R/000206.png` |
| F207 | 15.882000 / 0.037000 | READY chờ | Đầu cao, khăn co thành dải ngắn trái; READY đầy đủ, countdown ẩn. | 206→207 vòng khăn khép, chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000207.png` |
| F208 | 15.919000 / 0.046000 | READY chờ | Khăn dài trái với đuôi cong lên; READY cùng vị trí và viền khay. | 207→208 khăn duỗi và cong đuôi, tay giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000208.png` |
| F209 | 15.965000 / 0.059000 | READY chờ | Đầu và khăn giữ cel trước; READY ngà không cắt. | 208→209 giữ cel, ánh đuốc đổi nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000209.png` |
| F210 | 16.024000 / 0.050000 | READY chờ | Mặt cao phía trước, khăn uốn U thấp trái; khay không dịch, chưa intro. | 209→210 mặt xoay trước, khăn hạ thành U. | chưa kiểm chứng | `sword-death-ready-R/000210.png` |
| F211 | 16.074000 / 0.071000 | READY chờ | Giữ đầu/khăn U của F210; READY nguyên chữ và countdown ẩn. | 210→211 giữ cel, chân cố định trên gạch. | chưa kiểm chứng | `sword-death-ready-R/000211.png` |
| F212 | 16.145000 / 0.035000 | READY chờ | Mặt cao nghiêng phải, khăn chéo xuống trái; READY đủ chữ, score343 giữ. | 211→212 khăn nâng gần vai và thả chéo, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000212.png` |
| F213 | 16.180000 / 0.104000 | READY chờ | Giữ mặt và khăn chéo trái; READY cùng baseline, không intro. | 212→213 giữ cel, khay và camera đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000213.png` |
| F214 | 16.284000 / 0.029000 | READY chờ | Đầu cao, khăn mở vòng rộng trái; READY ngà và padding giữ. | 213→214 khăn mở vòng dưới vai, chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000214.png` |
| F215 | 16.313000 / 0.111000 | READY chờ | Đầu hạ phía trước, khăn vòng trái thấp; countdown vẫn ẩn, READY không cắt. | 214→215 đầu/vòng khăn hạ, không tự chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000215.png` |
| F216 | 16.424000 / 0.061000 | READY chờ | Mặt cao phía trước, khăn co dải ngắn trái; READY ngà đầy đủ, countdown ẩn. | 215→216 đầu nâng và vòng khăn khép, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000216.png` |
| F217 | 16.485000 / 0.041000 | READY chờ | Khăn dài trái với đuôi cong lên; READY giữ padding và baseline. | 216→217 khăn duỗi trái, tay giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000217.png` |
| F218 | 16.526000 / 0.052000 | READY chờ | Đầu và khăn cong trái giữ cel trước; READY không dịch, score343 giữ. | 217→218 giữ cel, ánh đuốc đổi nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000218.png` |
| F219 | 16.578000 / 0.047000 | READY chờ | Mặt nâng phía trước, khăn U thấp trái; READY không cắt, chưa intro. | 218→219 mặt xoay trước, khăn hạ U. | chưa kiểm chứng | `sword-death-ready-R/000219.png` |
| F220 | 16.625000 / 0.042000 | READY chờ | Khăn chéo trái mở rộng, đầu cao; READY ngà cùng viền khay. | 219→220 khăn nâng gần vai, duỗi chéo; chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000220.png` |
| F221 | 16.667000 / 0.049000 | READY chờ | Khăn chéo trái và mặt nghiêng phải; READY giữ baseline, countdown ẩn. | 220→221 mặt nghiêng phải, khăn mở rộng, camera đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000221.png` |
| F222 | 16.716000 / 0.075000 | READY chờ | Mặt cao và khăn chéo trái giữ; READY đủ chữ và padding. | 221→222 giữ cel, không intro hay card. | chưa kiểm chứng | `sword-death-ready-R/000222.png` |
| F223 | 16.791000 / 0.056000 | READY chờ | Khăn vòng rộng dưới vai trái, đầu cao; READY vẫn ngà đầy đủ. | 222→223 khăn mở vòng; chân và khay không trượt. | chưa kiểm chứng | `sword-death-ready-R/000223.png` |
| F224 | 16.847000 / 0.077000 | READY chờ | Đầu hạ phía trước, khăn vòng thấp trái; READY ngà và countdown ẩn. | 223→224 đầu/vòng khăn hạ, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000224.png` |
| F225 | 16.924000 / 0.059000 | READY chờ | Đầu cao, khăn ngắn trái; READY giữ padding, score343 giữ. | 224→225 đầu nâng, vòng khăn khép. | chưa kiểm chứng | `sword-death-ready-R/000225.png` |
| F226 | 16.983000 / 0.039000 | READY chờ | Khăn dài trái cong lên cuối, mặt cao; READY đầy đủ trong viền. | 225→226 khăn duỗi trái và cong đuôi. | chưa kiểm chứng | `sword-death-ready-R/000226.png` |
| F227 | 17.022000 / 0.076000 | READY chờ | Giữ mặt và khăn cong trái; countdown ẩn, không intro. | 226→227 giữ cel, ánh đuốc đổi nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000227.png` |
| F228 | 17.098000 / 0.052000 | READY chờ | Mặt hướng trước, khăn U thấp trái; READY không dịch baseline. | 227→228 mặt xoay trước, khăn hạ U. | chưa kiểm chứng | `sword-death-ready-R/000228.png` |
| F229 | 17.150000 / 0.050000 | READY chờ | Khăn chéo trái sáng gần vai; mặt cao, READY ngà đủ chữ. | 228→229 khăn nâng gần vai, thả chéo, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000229.png` |
| F230 | 17.200000 / 0.048000 | READY chờ | Đầu và khăn chéo trái giữ; khay giữ viền/padding, countdown ẩn. | 229→230 giữ cel, camera không dịch. | chưa kiểm chứng | `sword-death-ready-R/000230.png` |
| F231 | 17.248000 / 0.039000 | READY chờ | Khăn chéo trái mở rộng, mặt cao; READY không cắt, chưa intro. | 230→231 khăn duỗi trái, chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000231.png` |
| F232 | 17.287000 / 0.076000 | READY chờ | Khăn vòng rộng trái, đầu cao; READY nguyên chữ ngà, countdown ẩn. | 231→232 khăn mở vòng dưới vai; chân và camera giữ. | chưa kiểm chứng | `sword-death-ready-R/000232.png` |
| F233 | 17.363000 / 0.061000 | READY nhập R | R chuyển xanh ngọc, EADY ngà, đủ chữ cùng baseline/padding; khăn vòng trái và đầu cao. | 232→233 chỉ R đổi màu, chữ không nhảy; chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000233.png` |
| F234 | 17.424000 / 0.072000 | READY nhập R | R xanh và EADY ngà giữ vị trí; khăn co dải trái, mặt cao, score343 giữ. | 233→234 khăn khép vòng, trạng thái chữ giữ. | chưa kiểm chứng | `sword-death-ready-R/000234.png` |
| F235 | 17.496000 / 0.037000 | READY nhập R | Khăn dài trái cong lên cuối; R xanh và EADY ngà đầy đủ trong viền khay. | 234→235 khăn duỗi trái, chữ không dịch và countdown vẫn ẩn. | chưa kiểm chứng | `sword-death-ready-R/000235.png` |
| F236 | 17.533000 / 0.068000 | READY nhập R | Giữ mặt cao và khăn dài cong trái; R xanh, EADY ngà rõ trong khay. | 235→236 giữ cel, chữ không dịch. | chưa kiểm chứng | `sword-death-ready-R/000236.png` |
| F237 | 17.601000 / 0.048000 | READY nhập R | Mặt phía trước, khăn U thấp trái; R xanh và EADY ngà cùng baseline. | 236→237 mặt xoay trước và khăn hạ, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000237.png` |
| F238 | 17.649000 / 0.079000 | READY nhập R | Khăn chéo trái mở rộng, mặt cao; chữ giữ padding và viền, countdown ẩn. | 237→238 khăn nâng gần vai, duỗi chéo; không intro. | chưa kiểm chứng | `sword-death-ready-R/000238.png` |
| F239 | 17.728000 / 0.059000 | READY nhập R | Mặt nghiêng phải và khăn chéo trái; R xanh giữ, score343 giữ. | 238→239 mặt nghiêng, khăn mở rộng; camera không dịch. | chưa kiểm chứng | `sword-death-ready-R/000239.png` |
| F240 | 17.787000 / 0.038000 | READY nhập R | Giữ mặt/khăn chéo trái; R xanh và EADY ngà đầy đủ. | 239→240 giữ cel, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000240.png` |
| F241 | 17.825000 / 0.047000 | READY nhập R | Đầu cao, khăn vòng rộng trái; R xanh giữ baseline và countdown ẩn. | 240→241 khăn mở vòng dưới vai, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000241.png` |
| F242 | 17.872000 / 0.060000 | READY nhập R | Đầu cao và khăn vòng trái giữ; chữ không cắt, chưa intro. | 241→242 giữ cel, ánh đuốc đổi nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000242.png` |
| F243 | 17.932000 / 0.060000 | READY nhập R | Đầu nâng hướng trước, khăn vòng trái thấp; R xanh/EADY ngà giữ padding. | 242→243 mặt xoay trước, vòng khăn hạ, chân giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000243.png` |
| F244 | 17.992000 / 0.058000 | READY nhập R | Đầu cao hướng trước, khăn dải ngắn trái; R xanh EADY ngà rõ. | 243→244 khăn khép, chân và khay giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000244.png` |
| F245 | 18.050000 / 0.081000 | READY nhập R | Khăn dài trái cong đầu lên, đầu cao; chữ giữ baseline. | 244→245 khăn duỗi trái, không intro. | chưa kiểm chứng | `sword-death-ready-R/000245.png` |
| F246 | 18.131000 / 0.057000 | READY nhập R | Mặt phía trước cao, khăn U thấp; R xanh EADY ngà đủ padding. | 245→246 mặt xoay trước, khăn hạ. | chưa kiểm chứng | `sword-death-ready-R/000246.png` |
| F247 | 18.188000 / 0.018000 | READY nhập R | Mặt nghiêng phải cao, khăn chéo trái sáng gần vai; chữ giữ. | 246→247 khăn nâng chéo, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000247.png` |
| F248 | 18.206000 / 0.097000 | READY nhập R | Mặt và khăn chéo giữ; countdown ẩn, score343 giữ. | 247→248 giữ cel và màu chữ. | chưa kiểm chứng | `sword-death-ready-R/000248.png` |
| F249 | 18.303000 / 0.073000 | READY nhập RE | Khăn chéo trái rộng; RE xanh ADY ngà đọc đủ. | 248→249 E chuyển xanh, glyph không dịch; khăn mở. | chưa kiểm chứng | `sword-death-ready-R/000249.png` |
| F250 | 18.376000 / 0.041000 | READY nhập RE | Đầu cao, khăn vòng rộng trái; RE xanh ADY ngà cùng baseline. | 249→250 khăn mở vòng, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000250.png` |
| F251 | 18.417000 / 0.046000 | READY nhập RE | Đầu thấp hướng trước, vòng khăn hạ trái; chữ không cắt. | 250→251 đầu và vòng khăn hạ, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000251.png` |
| F252 | 18.463000 / 0.064000 | READY nhập RE | Đầu cao hướng trước, khăn dải ngắn trái; RE xanh ADY ngà rõ. | 251→252 đầu nâng, khăn khép, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000252.png` |
| F253 | 18.527000 / 0.103000 | READY nhập RE | Đầu cao, khăn dài cong trái lên ở đầu; chữ giữ padding. | 252→253 khăn duỗi trái, score343 giữ. | chưa kiểm chứng | `sword-death-ready-R/000253.png` |
| F254 | 18.630000 / 0.064000 | READY nhập RE | Mặt phía trước cao, khăn U thấp trái; countdown ẩn. | 253→254 khăn hạ thành U, glyph giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000254.png` |
| F255 | 18.694000 / 0.042000 | READY nhập RE | Mặt nghiêng phải cao, khăn chéo trái rộng; RE xanh ADY ngà đọc đủ. | 254→255 mặt nghiêng và khăn nâng chéo, khay không dịch. | chưa kiểm chứng | `sword-death-ready-R/000255.png` |
| F256 | 18.736000 / 0.082000 | READY nhập RE | Mặt nghiêng phải, khăn chéo trái rộng, RE xanh ADY ngà rõ. | 255→256 khăn duỗi rộng, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000256.png` |
| F257 | 18.818000 / 0.035000 | READY nhập RE | Giữ mặt và khăn chéo trái; countdown ẩn, score343 giữ. | 256→257 giữ cel, chữ không dịch. | chưa kiểm chứng | `sword-death-ready-R/000257.png` |
| F258 | 18.853000 / 0.032000 | READY nhập RE | Đầu cao, khăn vòng rộng trái; chữ đủ padding và baseline. | 257→258 khăn mở vòng, camera giữ. | chưa kiểm chứng | `sword-death-ready-R/000258.png` |
| F259 | 18.885000 / 0.048000 | READY nhập RE | Đầu thấp hướng trước, vòng khăn hạ; RE xanh ADY ngà giữ. | 258→259 đầu hạ, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000259.png` |
| F260 | 18.933000 / 0.034000 | READY nhập RE | Mặt phía trước nâng, khăn vòng thấp trái; khay đứng yên. | 259→260 đầu nâng, không intro. | chưa kiểm chứng | `sword-death-ready-R/000260.png` |
| F261 | 18.967000 / 0.076000 | READY nhập RE | Đầu cao, khăn dải ngắn trái; chữ đọc đủ, countdown ẩn. | 260→261 khăn khép về vai. | chưa kiểm chứng | `sword-death-ready-R/000261.png` |
| F262 | 19.043000 / 0.053000 | READY nhập RE | Đầu cao, khăn dài cong trái lên ở đầu; RE xanh ADY ngà giữ. | 261→262 khăn duỗi, chân và bóng giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000262.png` |
| F263 | 19.096000 / 0.054000 | READY nhập RE | Mặt phía trước, khăn U thấp trái; glyph giữ baseline và padding. | 262→263 khăn hạ và mặt xoay trước; score343 giữ. | chưa kiểm chứng | `sword-death-ready-R/000263.png` |
| F264 | 19.150000 / 0.086000 | READY nhập RE | Mặt nghiêng phải cao, khăn chéo trái mở; RE xanh ADY ngà rõ, countdown ẩn. | 263→264 mặt nghiêng, khăn nâng chéo, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000264.png` |
| F265 | 19.236000 / 0.016000 | READY nhập RE | Mặt và khăn chéo trái rộng giữ; score343 không đổi, chữ đủ padding. | 264→265 giữ cel, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000265.png` |
| F266 | 19.252000 / 0.035000 | READY nhập RE | Giữ đầu và khăn chéo trái; glyph giữ baseline, chưa intro. | 265→266 giữ cel, ánh đuốc đổi nhỏ. | chưa kiểm chứng | `sword-death-ready-R/000266.png` |
| F267 | 19.287000 / 0.022000 | READY nhập RE | Đầu cao, khăn vòng rộng trái; RE xanh ADY ngà đọc đủ. | 266→267 khăn mở vòng dưới vai, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000267.png` |
| F268 | 19.309000 / 0.040000 | READY nhập RE | Đầu cao và khăn vòng rộng trái giữ; RE xanh ADY ngà rõ. | 267→268 giữ cel, khay và chân đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000268.png` |
| F269 | 19.349000 / 0.046000 | READY nhập RE | Đầu cao, vòng khăn trái giữ; countdown ẩn, score343 giữ. | 268→269 giữ cel, ánh đuốc đổi nhẹ. | chưa kiểm chứng | `sword-death-ready-R/000269.png` |
| F270 | 19.395000 / 0.056000 | READY nhập RE | Đầu hạ hướng trước, vòng khăn thấp trái; chữ đủ padding. | 269→270 đầu và khăn hạ; chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000270.png` |
| F271 | 19.451000 / 0.018000 | READY nhập RE | Mặt phía trước nâng, khăn vòng thấp; glyph giữ baseline. | 270→271 đầu nâng, không intro. | chưa kiểm chứng | `sword-death-ready-R/000271.png` |
| F272 | 19.469000 / 0.069000 | READY nhập RE | Đầu cao và vòng khăn thấp giữ; RE xanh ADY ngà đầy đủ. | 271→272 giữ cel; khay không dịch. | chưa kiểm chứng | `sword-death-ready-R/000272.png` |
| F273 | 19.538000 / 0.065000 | READY nhập RE | Đầu cao, khăn dải ngắn trái; countdown vẫn ẩn. | 272→273 khăn khép về vai, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000273.png` |
| F274 | 19.603000 / 0.095000 | READY nhập sai: reset | READY toàn bộ đỏ, glyph vẫn nguyên vị trí và đọc đủ; khăn dài trái cong đầu lên. | 273→274 mất tiến độ RE, cả từ đỏ; viền và padding giữ. | chưa kiểm chứng | `sword-death-ready-R/000274.png` |
| F275 | 19.698000 / 0.080000 | READY feedback lỗi | READY đỏ giữ; mặt phía trước cao, khăn U thấp; score343 giữ. | 274→275 khăn hạ, chữ đỏ giữ baseline; không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000275.png` |
| F276 | 19.778000 / 0.018000 | READY hết feedback lỗi | READY trở lại toàn ngà, đọc đủ cùng vị trí; mặt nghiêng phải và khăn chéo trái. | 275→276 đỏ hết, chữ ngà không dịch; khăn nâng chéo. | chưa kiểm chứng | `sword-death-ready-R/000276.png` |
| F277 | 19.796000 / 0.061000 | READY chờ nhập lại | Đầu cao, khăn chéo trái rộng; READY toàn ngà đủ padding, countdown ẩn. | 276→277 khăn mở rộng, không intro. | chưa kiểm chứng | `sword-death-ready-R/000277.png` |
| F278 | 19.857000 / 0.060000 | READY chờ nhập lại | Giữ mặt và khăn chéo trái; chữ toàn ngà và score343 giữ. | 277→278 giữ cel, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000278.png` |
| F279 | 19.917000 / 0.038000 | READY chờ nhập lại | Đầu cao, khăn vòng rộng trái; READY toàn ngà rõ. | 278→279 khăn mở vòng, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000279.png` |
| F280 | 19.955000 / 0.076000 | READY chờ nhập lại | Đầu hạ hướng trước, vòng khăn thấp; chữ không cắt và countdown ẩn. | 279→280 đầu hạ, khay giữ baseline. | chưa kiểm chứng | `sword-death-ready-R/000280.png` |
| F281 | 20.031000 / 0.050000 | READY chờ nhập lại | Mặt phía trước nâng, vòng khăn thấp trái; READY toàn ngà giữ. | 280→281 đầu nâng, chân và camera giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000281.png` |
| F282 | 20.081000 / 0.033000 | READY chờ nhập lại | Đầu cao, khăn dải ngắn trái; glyph đủ padding. | 281→282 khăn khép về vai; score343 giữ. | chưa kiểm chứng | `sword-death-ready-R/000282.png` |
| F283 | 20.114000 / 0.045000 | READY chờ nhập lại | Khăn dài trái cong đầu lên, đầu cao; READY toàn ngà rõ. | 282→283 khăn duỗi, không intro hay countdown. | chưa kiểm chứng | `sword-death-ready-R/000283.png` |
| F284 | 20.159000 / 0.032000 | READY chờ nhập lại | Mặt trước nâng, khăn U thấp; READY ngà đủ padding, score343 giữ. | 283→284 mặt về trước, khăn hạ U; chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000284.png` |
| F285 | 20.191000 / 0.055000 | READY chờ nhập lại | Giữ mặt trước và khăn U thấp; khay đứng yên, countdown ẩn. | 284→285 giữ cel, glyph không dịch. | chưa kiểm chứng | `sword-death-ready-R/000285.png` |
| F286 | 20.246000 / 0.048000 | READY chờ nhập lại | Mặt nghiêng phải cao, khăn chéo trái sáng gần vai; READY ngà rõ. | 285→286 mặt nghiêng và khăn nâng chéo. | chưa kiểm chứng | `sword-death-ready-R/000286.png` |
| F287 | 20.294000 / 0.038000 | READY chờ nhập lại | Đầu cao, khăn chéo trái rộng; READY ngà giữ baseline. | 286→287 khăn mở rộng, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000287.png` |
| F288 | 20.332000 / 0.095000 | READY chờ nhập lại | Giữ mặt và khăn chéo rộng, score343 và HP giữ. | 287→288 giữ cel, không intro. | chưa kiểm chứng | `sword-death-ready-R/000288.png` |
| F289 | 20.427000 / 0.015000 | READY chờ nhập lại | Đầu cao, khăn mở vòng rộng trái; chữ đầy đủ trong khay. | 288→289 khăn mở vòng, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000289.png` |
| F290 | 20.442000 / 0.034000 | READY chờ nhập lại | Đầu hạ phía trước, vòng khăn thấp; READY ngà rõ. | 289→290 đầu hạ, countdown vẫn ẩn. | chưa kiểm chứng | `sword-death-ready-R/000290.png` |
| F291 | 20.476000 / 0.035000 | READY chờ nhập lại | Mặt trước nâng, vòng khăn thấp; glyph cùng baseline và padding. | 290→291 đầu nâng, chân và camera giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000291.png` |
| F292 | 20.511000 / 0.056000 | READY chờ nhập lại | Đầu cao, khăn dải ngắn trái; READY toàn ngà rõ, HP và score343 giữ. | 291→292 khăn khép gần vai, chân giữ gạch. | chưa kiểm chứng | `sword-death-ready-R/000292.png` |
| F293 | 20.567000 / 0.019000 | READY chờ nhập lại | Đầu cao, khăn dài trái cong đầu lên; chữ cùng baseline và padding. | 292→293 khăn duỗi trái, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000293.png` |
| F294 | 20.586000 / 0.038000 | READY chờ nhập lại | Giữ đầu cao, khăn dài cong trái; countdown ẩn. | 293→294 giữ cel, không intro. | chưa kiểm chứng | `sword-death-ready-R/000294.png` |
| F295 | 20.624000 / 0.085000 | READY chờ nhập lại | Giữ mặt và khăn cong trái, READY ngà không cắt. | 294→295 giữ cel, nền đuốc đổi sáng nhẹ. | chưa kiểm chứng | `sword-death-ready-R/000295.png` |
| F296 | 20.709000 / 0.041000 | READY chờ nhập lại | Mặt trước cao, khăn U thấp; khay và score343 giữ. | 295→296 khăn hạ U, mặt về trước; chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000296.png` |
| F297 | 20.750000 / 0.037000 | READY chờ nhập lại | Mặt nghiêng phải, khăn chéo trái rộng; READY ngà đủ padding. | 296→297 mặt nghiêng và khăn nâng chéo. | chưa kiểm chứng | `sword-death-ready-R/000297.png` |
| F298 | 20.787000 / 0.036000 | READY chờ nhập lại | Giữ mặt cao và khăn chéo trái rộng; countdown ẩn. | 297→298 giữ cel, camera giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000298.png` |
| F299 | 20.823000 / 0.025000 | READY chờ nhập lại | Giữ mặt và khăn chéo trái; READY toàn ngà rõ. | 298→299 giữ cel, không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000299.png` |
| F300 | 20.848000 / 0.035000 | READY chờ nhập lại | Giữ mặt cao và khăn chéo trái rộng; chữ ngà đủ padding. | 299→300 giữ cel; chân và khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000300.png` |
| F301 | 20.883000 / 0.076000 | READY chờ nhập lại | Đầu cao, khăn mở vòng trái; READY ngà rõ, countdown ẩn. | 300→301 khăn mở vòng, baseline giữ. | chưa kiểm chứng | `sword-death-ready-R/000301.png` |
| F302 | 20.959000 / 0.028000 | READY chờ nhập lại | Đầu hạ trước, vòng khăn thấp trái; score343 giữ. | 301→302 đầu hạ, chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000302.png` |
| F303 | 20.987000 / 0.041000 | READY chờ nhập lại | Giữ đầu thấp và vòng khăn trái; khay chữ không cắt. | 302→303 giữ cel, không intro. | chưa kiểm chứng | `sword-death-ready-R/000303.png` |
| F304 | 21.028000 / 0.048000 | READY chờ nhập lại | Mặt trước nâng, vòng khăn thấp; READY toàn ngà rõ. | 303→304 đầu nâng, camera giữ mốc. | chưa kiểm chứng | `sword-death-ready-R/000304.png` |
| F305 | 21.076000 / 0.049000 | READY chờ nhập lại | Đầu cao, khăn dải ngắn trái; chữ cùng baseline. | 304→305 khăn khép về vai, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000305.png` |
| F306 | 21.125000 / 0.039000 | READY chờ nhập lại | Đầu cao, khăn dài trái cong đầu lên; HP và score giữ. | 305→306 khăn duỗi trái, countdown vẫn ẩn. | chưa kiểm chứng | `sword-death-ready-R/000306.png` |
| F307 | 21.164000 / 0.054000 | READY chờ nhập lại | Giữ mặt cao và khăn cong trái; READY toàn ngà đủ padding. | 306→307 giữ cel, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000307.png` |
| F308 | 21.218000 / 0.056000 | READY chờ nhập lại | Mặt trước cao, khăn U thấp; READY ngà rõ, score343 giữ. | 307→308 mặt về trước, khăn hạ U; chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000308.png` |
| F309 | 21.274000 / 0.041000 | READY chờ nhập lại | Mặt nghiêng phải, khăn chéo trái rộng; chữ đủ padding. | 308→309 mặt nghiêng, khăn nâng chéo, khay không dịch. | chưa kiểm chứng | `sword-death-ready-R/000309.png` |
| F310 | 21.315000 / 0.090000 | READY chờ nhập lại | Giữ mặt cao và khăn chéo trái rộng; countdown ẩn. | 309→310 giữ cel, chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000310.png` |
| F311 | 21.405000 / 0.016000 | READY chờ nhập lại | Giữ mặt và khăn chéo trái; READY toàn ngà rõ, HP giữ. | 310→311 giữ cel, không intro. | chưa kiểm chứng | `sword-death-ready-R/000311.png` |
| F312 | 21.421000 / 0.055000 | READY chờ nhập lại | Đầu cao, khăn vòng rộng trái; READY ngà đủ padding. | 311→312 khăn mở vòng, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000312.png` |
| F313 | 21.476000 / 0.065000 | READY chờ nhập lại | Đầu hạ trước, vòng khăn thấp; chữ ngà đọc đủ. | 312→313 đầu hạ, baseline giữ. | chưa kiểm chứng | `sword-death-ready-R/000313.png` |
| F314 | 21.541000 / 0.051000 | READY nhập lại R | R xanh, EADY ngà cùng vị trí; mặt trước nâng và vòng khăn thấp. | 313→314 R đổi xanh, không dịch glyph; đầu nâng. | chưa kiểm chứng | `sword-death-ready-R/000314.png` |
| F315 | 21.592000 / 0.040000 | READY nhập lại R | R xanh/EADY ngà giữ baseline; đầu cao, khăn dải ngắn trái. | 314→315 khăn khép vai, countdown ẩn và score343 giữ. | chưa kiểm chứng | `sword-death-ready-R/000315.png` |
| F316 | 21.632000 / 0.085000 | READY nhập lại R | R xanh/EADY ngà; giữ khăn ngắn trái và đầu cao, score343 giữ. | 315→316 giữ cel, khay không dịch. | chưa kiểm chứng | `sword-death-ready-R/000316.png` |
| F317 | 21.717000 / 0.064000 | READY nhập lại R | R xanh giữ, khăn dài trái cong đầu lên; glyph không cắt. | 316→317 khăn duỗi, chân cố định. | chưa kiểm chứng | `sword-death-ready-R/000317.png` |
| F318 | 21.781000 / 0.052000 | READY nhập lại RE | RE xanh/ADY ngà, cùng baseline; mặt trước nâng và khăn U thấp. | 317→318 E đổi xanh, khăn hạ U, không intro. | chưa kiểm chứng | `sword-death-ready-R/000318.png` |
| F319 | 21.833000 / 0.055000 | READY nhập lại RE | RE xanh giữ vị trí, mặt nghiêng phải và khăn chéo trái rộng. | 318→319 mặt nghiêng, khăn nâng chéo; HP và score giữ. | chưa kiểm chứng | `sword-death-ready-R/000319.png` |
| F320 | 21.888000 / 0.138000 | READY nhập lại RE | RE xanh/ADY ngà giữ; mặt cao khăn chéo rộng; score343 giữ. | 319→320 giữ cel, chữ không dịch. | chưa kiểm chứng | `sword-death-ready-R/000320.png` |
| F321 | 22.026000 / 0.019000 | READY nhập lại RE | Giữ mặt và khăn chéo trái; glyph đủ padding, countdown ẩn. | 320→321 giữ cel, không intro. | chưa kiểm chứng | `sword-death-ready-R/000321.png` |
| F322 | 22.045000 / 0.057000 | READY nhập lại RE | Mặt trước cao, vòng khăn rộng; RE xanh đọc rõ, nền sàn đổi nét pixel nhẹ. | 321→322 khăn mở vòng; khay giữ vị trí, cần đối chiếu pha camera với trace. | chưa kiểm chứng | `sword-death-ready-R/000322.png` |
| F323 | 22.102000 / 0.057000 | READY nhập lại RE | Giữ mặt trước và vòng khăn trái; RE xanh/ADY ngà cùng baseline. | 322→323 giữ cel, score343 và HP giữ. | chưa kiểm chứng | `sword-death-ready-R/000323.png` |
| F324 | 22.159000 / 0.072000 | READY nhập lại RE | Đầu cao khăn dài cong trái; RE xanh/ADY ngà giữ vị trí, HP và score343 giữ. | 323→324 mặt lên và khăn duỗi trái; khay không dịch. | chưa kiểm chứng | `sword-death-ready-R/000324.png` |
| F325 | 22.231000 / 0.052000 | READY nhập lại RE | Mặt trước nâng, khăn U thấp; RE xanh rõ, countdown ẩn. | 324→325 mặt về trước, khăn hạ U; chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000325.png` |
| F326 | 22.283000 / 0.095000 | READY nhập lại RE | Mặt nghiêng phải, khăn chéo trái rộng; chữ đủ padding. | 325→326 mặt nghiêng và khăn nâng chéo, không intro. | chưa kiểm chứng | `sword-death-ready-R/000326.png` |
| F327 | 22.378000 / 0.039000 | Pause lúc nhập RE | Overlay Pause phủ tối; RE xanh/ADY ngà vẫn thấy trong khay, menu Resume/Restart/Settings/Top1000/MainMenu đọc được ở native. Nhân vật bị menu che. | 326→327 overlay xuất hiện, tiến độ RE còn nguyên; không suy diễn chi tiết nhân vật bị che. | chưa kiểm chứng | `sword-death-ready-R/000327.png` |
| F328 | 22.417000 / 0.099000 | Pause lúc nhập RE | Menu Pause hiện đủ viền vàng và nền nút tối; RE xanh/ADY ngà giữ trong khay dưới lớp tím. Nhân vật bị menu che phần lớn. | 327→328 opacity menu tăng; phần đầu lộ qua khe nút nhích lên, không suy diễn phần bị che. | chưa kiểm chứng | `sword-death-ready-R/000328.png` |
| F329 | 22.516000 / 0.022000 | Pause lúc nhập RE | Menu và tiến độ RE giữ vị trí; một phần đầu/miệng lộ giữa Settings và Top1000. | 328→329 phần đầu lộ hạ nhẹ; chữ và menu đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000329.png` |
| F330 | 22.538000 / 0.041000 | Pause lúc nhập RE | RE vẫn xanh, ADY ngà; không countdown hoặc intro. Phần đầu nhỏ còn thấy trong khe menu. | 329→330 phần đầu/nền dịch nhẹ xuống; menu và baseline chữ giữ. | chưa kiểm chứng | `sword-death-ready-R/000330.png` |
| F331 | 22.579000 / 0.037000 | Pause lúc nhập RE | Mắt xanh và mảnh đầu thấp trong khe Settings/Top1000; menu Pause giữ đủ nhãn, chưa có thêm chữ được nhận. | 330→331 nền và phần đầu lộ trôi nhẹ; RE/menu giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000331.png` |
| F332 | 22.616000 / 0.060000 | Pause lúc nhập RE | RE xanh/ADY ngà dưới lớp tối; đầu/mắt lộ thấp giữa Settings và Top1000, thân bị nút che. | 331→332 menu/khay giữ; mảnh đầu lộ thấp, không suy diễn thân. | chưa kiểm chứng | `sword-death-ready-R/000332.png` |
| F333 | 22.676000 / 0.053000 | Pause lúc nhập RE | Menu đủ năm nút; đầu và mắt lộ trong cùng khe, điểm343 và HP giữ. | 332→333 silhouette lộ và tiến độ RE giữ; không intro. | chưa kiểm chứng | `sword-death-ready-R/000333.png` |
| F334 | 22.729000 / 0.065000 | Pause lúc nhập RE | Chữ READY đủ padding dưới menu; phần thân vẫn bị che. | 333→334 menu, khay và phần đầu lộ giữ bố cục. | chưa kiểm chứng | `sword-death-ready-R/000334.png` |
| F335 | 22.794000 / 0.042000 | Pause lúc nhập RE | Viền vàng các nút liên tục; RE xanh giữ, countdown không thấy. | 334→335 chữ/viền đứng yên; không thêm tiến độ. | chưa kiểm chứng | `sword-death-ready-R/000335.png` |
| F336 | 22.836000 / 0.101000 | Pause lúc nhập RE | Đầu thấp trong khe nút, chân mờ phía dưới Top1000; glyph READY không cắt. | 335→336 mảnh đầu và menu giữ vị trí quan sát. | chưa kiểm chứng | `sword-death-ready-R/000336.png` |
| F337 | 22.937000 / 0.054000 | Pause lúc nhập RE | Chân đỏ mờ dưới Top1000 rõ hơn do sáng nền; menu và RE không đổi. | 336→337 vùng chân/nền sáng nhẹ, không có đổi chữ. | chưa kiểm chứng | `sword-death-ready-R/000337.png` |
| F338 | 22.991000 / 0.131000 | Pause lúc nhập RE | RE xanh, ADY ngà giữ baseline; đầu thấp còn lộ trong khe. | 337→338 menu, vùng chân và chữ giữ; không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000338.png` |
| F339 | 23.122000 / 0.053000 | Pause lúc nhập RE | Năm nhãn nút đủ ở native; READY không dịch, điểm343 giữ. | 338→339 phần lộ/menu giữ; không nhận thêm A. | chưa kiểm chứng | `sword-death-ready-R/000339.png` |
| F340 | 23.175000 / 0.036000 | Pause lúc nhập RE | Menu tối che phần lớn nhân vật; RE xanh còn nguyên trong khay. | 339→340 hình giữ ở các vùng thấy được; không suy diễn cel bị che. | chưa kiểm chứng | `sword-death-ready-R/000340.png` |
| F341 | 23.211000 / 0.097000 | Pause lúc nhập RE | Đầu và chân mờ lộ ở cùng khe; khay vẫn có RE xanh/ADY ngà. | 340→341 phần lộ và viền nút giữ; không countdown. | chưa kiểm chứng | `sword-death-ready-R/000341.png` |
| F342 | 23.308000 / 0.016000 | Pause lúc nhập RE | HUD tối vẫn đọc343; glyph READY giữ padding và màu tiến độ. | 341→342 menu/khay không dịch và chưa vào intro. | chưa kiểm chứng | `sword-death-ready-R/000342.png` |
| F343 | 23.324000 / 0.076000 | Pause lúc nhập RE | Pause còn đầy đủ; RE giữ nguyên, đầu thấp bị che phần lớn. | 342→343 bố cục và phần lộ giữ, không thêm tiến độ. | chưa kiểm chứng | `sword-death-ready-R/000343.png` |
| F344 | 23.400000 / 0.085000 | Pause giữ RE | Menu viền vàng giữ, đầu thấp trong khe và chân đỏ mờ; RE xanh/ADY ngà. | 343→344 các vùng lộ và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000344.png` |
| F345 | 23.485000 / 0.052000 | Pause giữ RE | Năm nút còn đủ nhãn; điểm343 và HP dưới lớp tối giữ. | 344→345 menu và chữ READY giữ, không intro. | chưa kiểm chứng | `sword-death-ready-R/000345.png` |
| F346 | 23.537000 / 0.052000 | Pause giữ RE | Chữ RE còn xanh; thân và vũ khí bị menu che. | 345→346 phần đầu lộ và menu giữ; chi tiết che chưa thấy. | chưa kiểm chứng | `sword-death-ready-R/000346.png` |
| F347 | 23.589000 / 0.064000 | Pause giữ RE | Khay đủ padding, countdown ẩn; đầu thấp trong khe nút. | 346→347 baseline/màu READY giữ. | chưa kiểm chứng | `sword-death-ready-R/000347.png` |
| F348 | 23.653000 / 0.059000 | Pause giữ RE | RE xanh/ADY ngà đứng yên dưới lớp tím; viền khay liên tục. | 347→348 không đổi tiến độ hoặc bố cục menu. | chưa kiểm chứng | `sword-death-ready-R/000348.png` |
| F349 | 23.712000 / 0.047000 | Pause giữ RE | Menu Pause giữ năm nút; đầu/mắt nhỏ còn lộ ở khe Settings/Top1000. | 348→349 phần lộ và glyph giữ. | chưa kiểm chứng | `sword-death-ready-R/000349.png` |
| F350 | 23.759000 / 0.046000 | Pause giữ RE | Điểm343 mờ ở HUD; READY giữ màu và vị trí. | 349→350 menu và khay giữ, không countdown. | chưa kiểm chứng | `sword-death-ready-R/000350.png` |
| F351 | 23.805000 / 0.042000 | Pause giữ RE | Chân đỏ mờ dưới Top1000 giữ; phần thân vẫn che. | 350→351 phần nhìn thấy giữ; không suy diễn animation che. | chưa kiểm chứng | `sword-death-ready-R/000351.png` |
| F352 | 23.847000 / 0.039000 | Pause giữ RE | RE vẫn xanh, ADY vẫn ngà; nhãn Resume đủ ở native. | 351→352 menu/khay đứng yên, chưa resume. | chưa kiểm chứng | `sword-death-ready-R/000352.png` |
| F353 | 23.886000 / 0.050000 | Pause giữ RE | Đầu thấp lộ trong khe; năm viền nút và chữ READY giữ. | 352→353 bố cục và phần lộ giữ. | chưa kiểm chứng | `sword-death-ready-R/000353.png` |
| F354 | 23.936000 / 0.081000 | Pause giữ RE | Countdown không hiện, điểm343 và tiến độ RE giữ dưới lớp tối. | 353→354 glyph/menu giữ, không intro. | chưa kiểm chứng | `sword-death-ready-R/000354.png` |
| F355 | 24.017000 / 0.057000 | Pause giữ RE | Menu còn phủ tối; RE xanh/ADY ngà không thêm tiến độ. | 354→355 phần đầu/chân lộ và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000355.png` |
| F356 | 24.074000 / 0.078000 | Pause giữ RE | Menu và phần đầu thấp lộ trong khe giữ; RE xanh/ADY ngà còn đủ. | 355→356 không đổi tiến độ, viền khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000356.png` |
| F357 | 24.152000 / 0.079000 | Pause giữ RE | Chân đỏ mờ dưới Top1000 và đầu trong khe giữ; thân che. | 356→357 phần lộ/menu giữ, không suy diễn thân. | chưa kiểm chứng | `sword-death-ready-R/000357.png` |
| F358 | 24.231000 / 0.015000 | Pause giữ RE | HUD tối giữ343; READY đủ padding, countdown ẩn. | 357→358 glyph và năm nút đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000358.png` |
| F359 | 24.246000 / 0.068000 | Pause giữ RE | RE xanh còn nguyên, ADY ngà; menu chưa tắt. | 358→359 khay và phần đầu lộ giữ. | chưa kiểm chứng | `sword-death-ready-R/000359.png` |
| F360 | 24.314000 / 0.029000 | Pause giữ RE | Viền vàng các nút giữ liên tục; nhân vật bị che phần lớn. | 359→360 menu/RE giữ, không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000360.png` |
| F361 | 24.343000 / 0.081000 | Pause giữ RE | Mảnh đầu/mắt vẫn thấp trong khe; READY giữ baseline. | 360→361 phần lộ và chữ giữ. | chưa kiểm chứng | `sword-death-ready-R/000361.png` |
| F362 | 24.424000 / 0.078000 | Pause giữ RE | Điểm343 và HP dưới lớp tối giữ; không thêm A xanh. | 361→362 menu/khay giữ, không intro. | chưa kiểm chứng | `sword-death-ready-R/000362.png` |
| F363 | 24.502000 / 0.057000 | Pause giữ RE | Pause còn đủ năm nút; RE xanh/ADY ngà giữ vị trí. | 362→363 phần đầu/chân lộ và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000363.png` |
| F364 | 24.559000 / 0.061000 | Pause giữ RE | Menu giữ, đầu thấp lộ trong khe, chân đỏ mờ; RE xanh/ADY ngà. | 363→364 phần lộ và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000364.png` |
| F365 | 24.620000 / 0.108000 | Pause / Resume đổi viền | Viền Resume chuyển cam sẫm hơn; RE giữ hai chữ xanh. | 364→365 chỉ viền Resume đổi rõ; không chuyển trận. | chưa kiểm chứng | `sword-death-ready-R/000365.png` |
| F366 | 24.728000 / 0.018000 | Pause / Resume đổi nét | Chữ Resume thành nét vuông hơn, viền cam; thân nhân vật vẫn che. | 365→366 nét nhãn đổi; READY giữ baseline. | chưa kiểm chứng | `sword-death-ready-R/000366.png` |
| F367 | 24.746000 / 0.042000 | Pause giữ RE | Nhãn Resume nét vuông/viền cam giữ; RE không thêm chữ. | 366→367 menu và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000367.png` |
| F368 | 24.788000 / 0.039000 | Pause giữ RE | Các nút và RE xanh/ADY ngà giữ; không countdown. | 367→368 phần lộ giữ, thân che. | chưa kiểm chứng | `sword-death-ready-R/000368.png` |
| F369 | 24.827000 / 0.030000 | Pause / Resume đổi nét | Resume trở lại nét có chân trang trí; viền cam giữ. | 368→369 nhãn đổi nét, khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000369.png` |
| F370 | 24.857000 / 0.136000 | Pause giữ RE | Resume nét trang trí giữ; điểm343 dưới lớp tối giữ. | 369→370 menu và tiến độ giữ. | chưa kiểm chứng | `sword-death-ready-R/000370.png` |
| F371 | 24.993000 / 0.063000 | Pause giữ RE | Năm nút còn nguyên, đầu thấp lộ trong khe; READY giữ. | 370→371 chưa tắt Pause, không intro. | chưa kiểm chứng | `sword-death-ready-R/000371.png` |
| F372 | 25.056000 / 0.141000 | Pause giữ RE | Viền Resume vàng nâu; RE xanh/ADY ngà giữ, đầu còn thấp. | 371→372 khay và menu giữ. | chưa kiểm chứng | `sword-death-ready-R/000372.png` |
| F373 | 25.197000 / 0.065000 | Pause bắt đầu mờ | Viền cả năm nút tối đi; đầu sau menu nhô lên, nền đổi vị trí. | 372→373 menu mờ, RE vẫn đủ hai chữ xanh. | chưa kiểm chứng | `sword-death-ready-R/000373.png` |
| F374 | 25.262000 / 0.137000 | Pause mờ / nền dịch | Viền tối giữ; phần đầu sau menu thấp hơn, đuốc/nền dịch; thân che. | 373→374 menu/khay đứng vị trí, nền dịch. | chưa kiểm chứng | `sword-death-ready-R/000374.png` |
| F375 | 25.399000 / 0.041000 | Pause mờ / nền dịch | Đầu gần như khuất trong khe; RE vẫn xanh, ADY ngà; menu còn đủ. | 374→375 nền tiếp tục dịch, chưa thấy intro hay countdown. | chưa kiểm chứng | `sword-death-ready-R/000375.png` |
| F376 | 25.440000 / 0.034000 | Resume / RE giữ | Menu và lớp tím tắt; đầu/tay/thân/chân lộ đủ, khăn chéo trái xuống; RE xanh/ADY ngà. | 375→376 nền sáng trở lại, chân giữ landmark, khay không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000376.png` |
| F377 | 25.474000 / 0.040000 | READY sau Resume | Pose đứng và khăn chéo trái xuống giữ; HP/343 giữ, không countdown. | 376→377 silhouette và tiến độ giữ. | chưa kiểm chứng | `sword-death-ready-R/000377.png` |
| F378 | 25.514000 / 0.095000 | READY sau Resume | Đầu thấp nhẹ, khăn trái mở rộng; RE xanh đủ padding. | 377→378 khăn mở, chân và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000378.png` |
| F379 | 25.609000 / 0.033000 | READY sau Resume | Đầu cao, khăn vòng lớn trái xuống; nền và ánh sáng sàn giữ. | 378→379 khăn đổi cel, tay vẫn sát thân, chân neo sàn. | chưa kiểm chứng | `sword-death-ready-R/000379.png` |
| F380 | 25.642000 / 0.208000 | READY sau Resume | Vòng khăn trái rộng giữ; RE/ADY đủ nét và baseline. | 379→380 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000380.png` |
| F381 | 25.850000 / 0.063000 | READY sau Resume | Đầu thấp hơn, khăn ngắn đầu cong lên trái; HP/điểm giữ. | 380→381 khăn thu ngắn, chân không trượt. | chưa kiểm chứng | `sword-death-ready-R/000381.png` |
| F382 | 25.913000 / 0.046000 | READY sau Resume | Đầu nâng, khăn đầu cong lên trái kéo dài hơn. | 381→382 khăn dài, chân và tray giữ. | chưa kiểm chứng | `sword-death-ready-R/000382.png` |
| F383 | 25.959000 / 0.047000 | READY sau Resume | Đầu ngang, khăn chéo xuống trái trở lại; RE còn xanh. | 382→383 khăn đổi cel, không intro/countdown. | chưa kiểm chứng | `sword-death-ready-R/000383.png` |
| F384 | 26.006000 / 0.164000 | READY sau Resume | Khăn chéo trái xuống, đầu ngang; RE xanh và ADY ngà đủ. | 383→384 pose/khay giữ, chân neo sàn. | chưa kiểm chứng | `sword-death-ready-R/000384.png` |
| F385 | 26.170000 / 0.188000 | READY sau Resume | Đầu cao, khăn vòng lớn bên trái. | 384→385 khăn mở vòng, tay/grip giữ sát thân. | chưa kiểm chứng | `sword-death-ready-R/000385.png` |
| F386 | 26.358000 / 0.108000 | READY sau Resume | Đầu hạ, khăn ngắn đầu cong lên trái; HUD343 giữ. | 385→386 khăn thu, khay giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000386.png` |
| F387 | 26.466000 / 0.131000 | READY sau Resume | Đầu cao, khăn gợn trái ngang; READY không thêm chữ xanh. | 386→387 đầu/khăn đổi cel, chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000387.png` |
| F388 | 26.597000 / 0.190000 | READY sau Resume | Khăn cong tròn trái xuống, đầu ngang; không countdown. | 387→388 khăn mở vòng, tray không nhảy. | chưa kiểm chứng | `sword-death-ready-R/000388.png` |
| F389 | 26.787000 / 0.137000 | READY sau Resume | Khăn ngắn chéo lên trái, đầu thấp; RE giữ. | 388→389 khăn thu ngắn, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000389.png` |
| F390 | 26.924000 / 0.076000 | READY sau Resume | Khăn đầu cong lên trái dài, đầu cao; HP343 giữ. | 389→390 khăn dài, khay/camera giữ. | chưa kiểm chứng | `sword-death-ready-R/000390.png` |
| F391 | 27.000000 / 0.128000 | READY sau Resume | Khăn chéo xuống trái, đầu ngang; RE xanh/ADY ngà. | 390→391 khăn đổi cel, không intro. | chưa kiểm chứng | `sword-death-ready-R/000391.png` |
| F392 | 27.128000 / 0.053000 | READY sau Resume | Khăn vòng lớn trái xuống, đầu ngang; RE xanh/ADY ngà. | 391→392 khăn mở vòng, chân/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000392.png` |
| F393 | 27.181000 / 0.108000 | READY sau Resume | Vòng khăn giữ, đầu cao hơn nhẹ; không countdown. | 392→393 đầu nâng, tray giữ. | chưa kiểm chứng | `sword-death-ready-R/000393.png` |
| F394 | 27.289000 / 0.064000 | Tiến độ REA | REA xanh, DY ngà; A đổi màu đúng vị trí, khăn ngắn chéo trái. | 393→394 thêm A xanh, glyph không dịch hay cắt. | chưa kiểm chứng | `sword-death-ready-R/000394.png` |
| F395 | 27.353000 / 0.051000 | Tiến độ REA | REA xanh/DY ngà giữ; khăn đầu cong lên trái dài. | 394→395 khăn dài, khay và chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000395.png` |
| F396 | 27.404000 / 0.058000 | Tiến độ REA | REA xanh/DY ngà, khăn cong lên trái; chân giữ sàn. | 395→396 pose và glyph giữ. | chưa kiểm chứng | `sword-death-ready-R/000396.png` |
| F397 | 27.462000 / 0.073000 | Tiến độ REA | Khăn gợn trái ngang, đầu cao; khay giữ. | 396→397 khăn đổi cel, không countdown. | chưa kiểm chứng | `sword-death-ready-R/000397.png` |
| F398 | 27.535000 / 0.075000 | Tiến độ REA | Khăn chéo xuống trái, đầu ngang; REA còn xanh. | 397→398 khăn hạ, chân neo sàn. | chưa kiểm chứng | `sword-death-ready-R/000398.png` |
| F399 | 27.610000 / 0.111000 | Tiến độ REA | Khăn chéo trái giữ; HP/343 và glyph giữ. | 398→399 silhouette/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000399.png` |
| F400 | 27.721000 / 0.073000 | Tiến độ REA | Đầu thấp, khăn vòng lớn trái xuống; không intro. | 399→400 khăn mở vòng, khay đứng yên. | chưa kiểm chứng | `sword-death-ready-R/000400.png` |
| F401 | 27.794000 / 0.063000 | Tiến độ REA | Đầu cao, khăn ngang trái ngắn; REA xanh đủ nét. | 400→401 khăn thu/đầu nâng, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000401.png` |
| F402 | 27.857000 / 0.056000 | Tiến độ REA | Khăn cong lên trái dài hơn, đầu nâng; DY ngà. | 401→402 khăn dài, tiến độ giữ. | chưa kiểm chứng | `sword-death-ready-R/000402.png` |
| F403 | 27.913000 / 0.038000 | Tiến độ REA | Khăn cong lên trái và đầu giữ; HUD không đổi. | 402→403 pose/khay giữ, không countdown. | chưa kiểm chứng | `sword-death-ready-R/000403.png` |
| F404 | 27.951000 / 0.106000 | Tiến độ REA | Khăn cong lên trái và đầu cao giữ; DY ngà đủ nét. | 403→404 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000404.png` |
| F405 | 28.057000 / 0.043000 | Tiến độ REA | Khăn chéo xuống trái, đầu ngang; REA xanh giữ. | 404→405 khăn hạ, chân/viền khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000405.png` |
| F406 | 28.100000 / 0.126000 | Tiến độ REA | Khăn chéo trái giữ, không countdown/intro. | 405→406 pose/khay giữ, HUD343 không đổi. | chưa kiểm chứng | `sword-death-ready-R/000406.png` |
| F407 | 28.226000 / 0.042000 | Tiến độ REA | Đầu cao, khăn vòng lớn trái; DY ngà giữ padding. | 406→407 khăn mở vòng, chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000407.png` |
| F408 | 28.268000 / 0.107000 | Tiến độ REA | Đầu cao, khăn vòng lớn trái giữ; DY ngà. | 407→408 pose và khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000408.png` |
| F409 | 28.375000 / 0.044000 | Tiến độ REA | Đầu hạ, khăn ngắn cong lên trái; DY ngà đủ nét. | 408→409 khăn thu, chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000409.png` |
| F410 | 28.419000 / 0.083000 | Tiến độ REA | Khăn cong lên trái giữ; countdown vẫn ẩn. | 409→410 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000410.png` |
| F411 | 28.502000 / 0.095000 | Tiến độ READ | D chuyển xanh tại chỗ; Y ngà, đầu cao khăn ngang lượn. | 410→411 D đổi màu, baseline/padding giữ. | chưa kiểm chứng | `sword-death-ready-R/000411.png` |
| F412 | 28.597000 / 0.048000 | Tiến độ READ | Khăn ngang lượn trái; Y ngà không cắt, HUD343. | 411→412 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000412.png` |
| F413 | 28.645000 / 0.130000 | Tiến độ READ | Đầu và khăn ngang giữ; không intro/countdown. | 412→413 khay và chân giữ vị trí. | chưa kiểm chứng | `sword-death-ready-R/000413.png` |
| F414 | 28.775000 / 0.037000 | Tiến độ READ | Khăn mở vòng trái xuống dưới; Y ngà đầy đủ. | 413→414 khăn mở, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000414.png` |
| F415 | 28.812000 / 0.118000 | Tiến độ READ | Vòng khăn trái và đầu cao giữ, READ xanh/Y ngà. | 414→415 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000415.png` |
| F416 | 28.930000 / 0.396000 | Tiến độ READ | Khăn ngắn cong lên trái, đầu cao; Y ngà. | 415→416 khăn thu, chân/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000416.png` |
| F417 | 29.326000 / 0.249000 | Tiến độ READ | Khăn mở vòng trái; READ xanh/Y ngà đầy đủ. | 416→417 khăn mở, HUD343 giữ. | chưa kiểm chứng | `sword-death-ready-R/000417.png` |
| F418 | 29.575000 / 0.289000 | Tiến độ READ | Đầu thấp, khăn lượn ngang trái; countdown ẩn. | 417→418 đầu hạ, khăn ngang, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000418.png` |
| F419 | 29.864000 / 0.050000 | Tiến độ READ | Đầu cao khăn vòng trái; chữ không dịch. | 418→419 khăn mở, padding/baseline giữ. | chưa kiểm chứng | `sword-death-ready-R/000419.png` |
| F420 | 29.914000 / 0.194000 | Tiến độ READ | Khăn ngắn cong lên trái; Y ngà rõ. | 419→420 khăn thu, khay/chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000420.png` |
| F421 | 30.108000 / 0.329000 | Tiến độ READ | Khăn chéo xuống trái và đầu cao; không intro. | 420→421 khăn hạ, chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000421.png` |
| F422 | 30.437000 / 0.068000 | Tiến độ READ | Khăn cong lên trái; READ xanh/Y ngà giữ. | 421→422 khăn nâng, chữ/viền giữ. | chưa kiểm chứng | `sword-death-ready-R/000422.png` |
| F423 | 30.505000 / 0.127000 | Tiến độ READ | Khăn cong lên trái và đầu cao giữ; countdown ẩn. | 422→423 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000423.png` |
| F424 | 30.632000 / 0.198000 | Tiến độ READ | Khăn chéo trái xuống, đầu cao; Y ngà giữ. | 423→424 khăn hạ, chân/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000424.png` |
| F425 | 30.830000 / 0.127000 | Tiến độ READ | Khăn vòng trái, đầu ngang; READ xanh đủ nét. | 424→425 khăn mở vòng, HUD343 giữ. | chưa kiểm chứng | `sword-death-ready-R/000425.png` |
| F426 | 30.957000 / 0.120000 | Tiến độ READ | Khăn ngắn cong lên trái; countdown ẩn. | 425→426 khăn thu, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000426.png` |
| F427 | 31.077000 / 0.018000 | Tiến độ READ | Đầu hạ, khăn ngang lượn trái; Y ngà. | 426→427 đầu thấp, chữ/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000427.png` |
| F428 | 31.095000 / 0.163000 | Tiến độ READ | Đầu thấp và khăn ngang giữ; không intro. | 427→428 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000428.png` |
| F429 | 31.258000 / 0.107000 | Tiến độ READ | Đầu cao khăn vòng lớn trái; chữ giữ padding. | 428→429 khăn mở, chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000429.png` |
| F430 | 31.365000 / 0.111000 | Tiến độ READ | Đầu ngang khăn vòng trái giữ; Y ngà không cắt. | 429→430 đầu hạ nhẹ, khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000430.png` |
| F431 | 31.476000 / 0.181000 | Tiến độ READ | Khăn ngắn cong lên trái, đầu ngang; countdown ẩn. | 430→431 khăn thu, HUD343/chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000431.png` |
| F432 | 31.657000 / 0.061000 | Tiến độ READ | Khăn chéo xuống trái, đầu ngang; Y ngà. | 431→432 khăn hạ, chân/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000432.png` |
| F433 | 31.718000 / 0.038000 | Tiến độ READ | Khăn chéo trái và đầu ngang giữ; countdown ẩn. | 432→433 pose/khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000433.png` |
| F434 | 31.756000 / 0.082000 | Tiến độ READ | Khăn vòng lớn trái, đầu ngang; glyph đủ nét. | 433→434 khăn mở vòng, chân giữ sàn. | chưa kiểm chứng | `sword-death-ready-R/000434.png` |
| F435 | 31.838000 / 0.104000 | Tiến độ READ | Đầu thấp khăn vòng trái, tay hạ nhẹ; Y ngà. | 434→435 đầu/tay hạ, chữ giữ baseline. | chưa kiểm chứng | `sword-death-ready-R/000435.png` |
| F436 | 31.942000 / 0.098000 | Tiến độ READ | Đầu cao khăn vòng trái; không intro. | 435→436 đầu/tay nâng, chân giữ. | chưa kiểm chứng | `sword-death-ready-R/000436.png` |
| F437 | 32.040000 / 0.082000 | Tiến độ READ | Khăn cong lên trái, đầu ngang; HUD343. | 436→437 khăn thu, khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000437.png` |
| F438 | 32.122000 / 0.097000 | Tiến độ READ | Đầu thấp khăn ngang trái; chữ Y ngà đủ nét. | 437→438 đầu hạ/khăn ngang, chân giữ landmark. | chưa kiểm chứng | `sword-death-ready-R/000438.png` |
| F439 | 32.219000 / 0.121000 | Tiến độ READ | Đầu cao khăn chéo xuống trái; countdown ẩn. | 438→439 khăn hạ, khay/padding giữ. | chưa kiểm chứng | `sword-death-ready-R/000439.png` |
| F440 | 32.340000 / 0.143000 | Chuyển trận sau READY | Màn đen che từ đáy đến mép trên khay; player/HUD cũ còn. Không có frame Y xanh đọc được. | 439→440 chữ/khay bị wipe che, đầu player hạ; không suy diễn pose bị che. | chưa kiểm chứng | `sword-death-ready-R/000440.png` |
| F441 | 32.483000 / 0.116000 | Chuyển trận | Đen toàn canvas trừ label QA; actor/HUD/khay không đọc được. | 440→441 wipe phủ phần cảnh còn lại. | chưa kiểm chứng | `sword-death-ready-R/000441.png` |
| F442 | 32.599000 / 0.128000 | Chuyển trận | Toàn cảnh vẫn đen, label QA giữ. | 441→442 trạng thái che toàn canvas giữ. | chưa kiểm chứng | `sword-death-ready-R/000442.png` |
| F443 | 32.727000 / 0.039000 | Chuyển trận | Canvas đen, không thấy actor/khay/countdown. | 442→443 trạng thái che giữ. | chưa kiểm chứng | `sword-death-ready-R/000443.png` |
| F444 | 32.766000 / 0.064000 | Mở cảnh kế | Đen còn gần hết; dải sàn rất mỏng xuất hiện đáy. | 443→444 wipe bắt đầu mở từ dưới. | chưa kiểm chứng | `sword-death-ready-R/000444.png` |
| F445 | 32.830000 / 0.137000 | Mở cảnh kế | Phần sàn dưới mở rõ, player còn bị che; HUD/khay ẩn. | 444→445 vùng sàn mở lên trên, chưa đánh giá actor. | chưa kiểm chứng | `sword-death-ready-R/000445.png` |
| F446 | 32.967000 / 0.103000 | Intro kế | Cảnh mở hoàn toàn, player đứng cùng landmark sàn, khăn vòng trái; HUD/khay ẩn. | 445→446 wipe hết, actor hiện đúng phía trái. | chưa kiểm chứng | `sword-death-ready-R/000446.png` |
| F447 | 33.070000 / 0.245000 | Intro kế | Hai cụm bụi ngà xuất hiện bên phải player, che vùng spawn đối thủ. | 446→447 bụi nở trên sàn; player/chân giữ, khay/HUD ẩn. | chưa kiểm chứng | `sword-death-ready-R/000447.png` |
| F448 | 33.315000 / 0.238000 | Intro kế | HUD hiện dần, score 343; DOUBLESTRIKE DUEL 2/6 giữa cảnh. Kiếm idle bên phải, bụi sàn nhỏ; khay ẩn. | 447→448 đối thủ hiện khỏi bụi, chân đứng trên sàn, player đầu thấp khăn cong. | chưa kiểm chứng | `sword-death-ready-R/000448.png` |
| F449 | 33.553000 / 0.067000 | Intro → combat | Intro rất nhạt; there then them trở lại đầy đủ trong khay, countdown cyan đầy. | 448→449 khay/HUD sáng lên; kiếm giữ grip, player đầu nâng. | chưa kiểm chứng | `sword-death-ready-R/000449.png` |
| F450 | 33.620000 / 0.143000 | Combat kế | Intro giữa cảnh đã hết; tên DOUBLESTRIKE trên HUD, ba từ ngà đủ và countdown đầy. | 449→450 chữ intro biến mất, actor/chân và bố cục khay giữ. | chưa kiểm chứng | `sword-death-ready-R/000450.png` |
| F451 | 33.763000 / 0.016000 | Combat kế | Đầu/sườn kiếm hạ theo idle; blade chéo trái vẫn nối tay, player đầu hạ khăn trải trái. | 450→451 cả hai bob idle, chân và landmark sàn giữ. | chưa kiểm chứng | `sword-death-ready-R/000451.png` |
| F452 | 33.779000 / 0.062000 | Combat kế | Kiếm giữ pose đầu thấp, player khăn võng chữ U; ba từ ngà, score 343. | 451→452 thân kiếm giữ, khăn player đổi cel; countdown gần đầy. | chưa kiểm chứng | `sword-death-ready-R/000452.png` |
| F453 | 33.841000 / 0.157000 | Combat kế | Player đầu nâng, khăn võng thấp trái; kiếm vẫn giữ blade và chân. | 452→453 player đổi idle tại chân cố định; countdown bắt đầu ngắn nhẹ. | chưa kiểm chứng | `sword-death-ready-R/000453.png` |
| F454 | 33.998000 / 0.103000 | Combat kế | Đầu kiếm nâng; khăn player vòm cao trước vai, grip kiếm liên tục; từ không đổi. | 453→454 idle đổi đầu/rib, countdown rút thêm từ phải. | chưa kiểm chứng | `sword-death-ready-R/000454.png` |
| F455 | 34.101000 / 0.072000 | Combat kế | Player đầu thấp khăn vòm; kiếm đầu cao, chân giữ; khay và HUD đủ. | 454→455 player bob xuống, countdown rút đều; score/HP nhìn không đổi. | chưa kiểm chứng | `sword-death-ready-R/000455.png` |
| F456 | 34.173000 / 0.091000 | Combat kế | Pose kiếm giữ, player đầu thấp khăn vòm, ba từ ngà. | 455→456 actor giữ cel, countdown rút một đoạn ngắn. | chưa kiểm chứng | `sword-death-ready-R/000456.png` |
| F457 | 34.264000 / 0.062000 | Combat kế | Khăn player trải ngang trái và đầu nâng; kiếm idle giữ. | 456→457 khăn đổi silhouette, chân không trượt; countdown tiếp tục rút. | chưa kiểm chứng | `sword-death-ready-R/000457.png` |
| F458 | 34.326000 / 0.164000 | Combat kế | Player khăn võng U trái, đầu nâng; blade kiếm vẫn chéo trái. | 457→458 khăn hạ, actor không đổi sorting; khay cố định. | chưa kiểm chứng | `sword-death-ready-R/000458.png` |
| F459 | 34.490000 / 0.102000 | Combat kế | Card SPACE xuất hiện trên kiếm; actor giữ khoảng cách, chữ khay đủ, score 343. | 458→459 cue SPACE hiện; khăn player vòm, countdown rút. | chưa kiểm chứng | `sword-death-ready-R/000459.png` |
| F460 | 34.592000 / 0.086000 | Combat kế | SPACE giữ trên kiếm, player đầu thấp khăn vòm; các thanh HP/HUD giữ. | 459→460 player bob xuống, countdown rút; grip và chân kiếm giữ. | chưa kiểm chứng | `sword-death-ready-R/000460.png` |
| F461 | 34.678000 / 0.101000 | Combat kế | SPACE giữ, hai actor cùng pose trước; there then them không cắt. | 460→461 pose giữ, countdown ngắn thêm ở mép phải. | chưa kiểm chứng | `sword-death-ready-R/000461.png` |
| F462 | 34.779000 / chưa xác định (cuối media) | Cuối media / combat kế | Player đầu nâng khăn trải trái; SPACE/kiếm idle giữ, score 343; duration cuối chưa xác định. | 461→462 khăn đổi cel và countdown rút; đây là biên clip, không phải loop hoặc recovery đòn mới. | chưa kiểm chứng | `sword-death-ready-R/000462.png` |
<!-- ledger-sword-death-ready-R-end -->

### Sửa định danh frame sau resize — bằng chứng kỹ thuật

Đối chiếu PNG/trace phát hiện `decode-scopes.py` dùng `select=between(n,...)`.
FFmpeg tái khởi tạo filter graph khi video đổi 1280×720→1366×768→1280×720;
biến `n` trong filter quay về 0. PNG gán F682/PTS38.010 thực tế là PTS68.984
(trích lại bằng n682 khớp từng pixel; chọn t≥38 cho hình khác, đúng khoảng intro).
Đây là lỗi định danh trong công cụ audit, không phải lỗi gameplay/animation.

Đã vô hiệu hóa PNG/PTS cũ của ba đoạn combat sau resize: Double Strike, rìu và
khiên. Trích lại bằng khoảng PTS tuyệt đối, giữ mọi frame encode và duration;
không coi sáu ảnh gán nhầm vừa xem là coverage các đoạn này. Các đoạn death/card
lịch sử cũng dùng phép trích đó nên không dùng nhãn frame của chúng cho R.
Nguồn video/hash vẫn giữ nguyên. `decode-index-correction.json` ghi phản chứng,
biên gốc/biên mới và số frame; ảnh trước sửa giữ nguyên trong history-n-filter.
Normal F0–F338 nằm trước resize đầu, tutorial và media Hurt/death riêng không
đổi kích thước nên không chịu lỗi reset này; bằng chứng đã xem của chúng giữ lại.

Biên normal mở [0,18)→[0,19.3) để bao parry/phản kích và recovery về idle19.2156;
rìu mở tới79.98 sau idle79.8718; khiên tới132.5 vì Attack1 đã bắt đầu trong lúc
Hurt hồi phục ở biên130, kết parry/counter132.3218. Hurt player mở đầu1.1 trước
windup1.23, death player mở25.5 trước windup25.7381. Không cắt bớt đoạn đã chốt.
Các biên này còn phải xác nhận trực tiếp qua toàn bộ hình trích đúng PTS.

### Ledger kiểm trực tiếp — `axe-death-ready-R`

Media SHA256 `86afe9c0aa5b9c1d65e88cdf3f38dd3d4618f07f7ea705a0deab36e29419cdf7`; revision `66749da1`.
Mọi frame dưới đây đã mở full canvas và nearest 4× không thu nhỏ; frame giữ vẫn tính.
Giới hạn: playback người xem và chi tiết bị che còn chờ, nên chưa chứng nhận toàn clip.

| Frame | PTS / duration (s) | Pha quan sát | Nhận xét full/nearest | Chuyển tiếp từ frame trước | Trạng thái | Bằng chứng tạm trong review/ |
|---|---|---|---|---|---|---|
| F0 | 0.000000 / 0.099000 | Trước kết liễu / recovery | Player thấp, kiếm sát sàn bên phải; rìu dựng ngà sáng, tay nối cán, +10 và SPACE phía trên. Khay would an every đủ. | Biên đầu media: không có cặp trước; phần chuẩn bị đòn trước nằm ngoài clip. | chưa kiểm chứng | `axe-death-ready-R/000000.png` |
| F1 | 0.099000 / 0.017000 | Recovery player | Player nâng thân, kiếm chéo thấp, khăn rủ sau; rìu/đối thủ giữ. Score 1582. | 0→1 player hồi thân, chân cùng vùng sàn; countdown ngắn thêm, chữ giữ. | chưa kiểm chứng | `axe-death-ready-R/000001.png` |
| F2 | 0.116000 / 0.121000 | Recovery player | Player giữ thân nửa đứng/kiếm thấp; +10 mờ và rìu dựng. | 1→2 pose giữ, countdown rút nhẹ; không bỏ frame lặp. | chưa kiểm chứng | `axe-death-ready-R/000002.png` |
| F3 | 0.237000 / 0.057000 | Idle / input combat | Player đứng idle, khăn vòm trái; w cyan trong would, đối thủ rìu dựng. | 2→3 player về idle, kiếm không còn vươn; khay giữ vị trí khi đổi màu w. | chưa kiểm chứng | `axe-death-ready-R/000003.png` |
| F4 | 0.294000 / 0.114000 | Rìu chuẩn bị Attack1 | Rìu chéo lên trái ngang ngực; tay và cán chạm, chân đối thủ giữ. wo cyan. | 3→4 cán xoay từ dựng sang chéo, ngực thu; player idle, countdown rút. | chưa kiểm chứng | `axe-death-ready-R/000004.png` |
| F5 | 0.408000 / 0.057000 | Rìu Attack1 chuẩn bị | Rìu ngang ngực, lưỡi trái; hai tay dọc cán, đầu và chân rõ. wou cyan. | 4→5 cán xoay ngang qua thân, grip đi cùng cán; khăn player võng trái. | chưa kiểm chứng | `axe-death-ready-R/000005.png` |
| F6 | 0.465000 / 0.050000 | Rìu Attack1 chuẩn bị | Rìu hạ chéo trái sát hông/gối, chân vẫn cùng landmark; woul cyan. | 5→6 tay/cán hạ, thân nghiêng; player chưa đổi vị trí, countdown rút. | chưa kiểm chứng | `axe-death-ready-R/000006.png` |
| F7 | 0.515000 / 0.129000 | Player đánh / che bởi VFX | Player chùng đánh, cung ngà và flash vàng/trắng che sườn/tay/rìu đối thủ; đầu còn thấy. Score 1592, would biến mất. | 6→7 cung/flash xuất hiện khi hoàn tất từ; không kết luận grip phần bị che, countdown được nạp lại. | chưa kiểm chứng | `axe-death-ready-R/000007.png` |
| F8 | 0.644000 / 0.099000 | Đánh / VFX | Flash trắng hết, cung ngà vẫn che tay trái/sườn rìu; hạt cam quanh thân. Khay an every some. | 7→8 flash tắt để lộ thêm đầu/rib, cung giữ; cảnh có shake nhẹ, HUD/khay cố định. | chưa kiểm chứng | `axe-death-ready-R/000008.png` |
| F9 | 0.743000 / 0.043000 | Parry | Player dựng kiếm trước mặt; rìu chéo xuống trái gần sàn, SPACE cyan. Score 1592. | 8→9 cung mất, kiếm player dựng, cán rìu lộ trở lại; chân giữ vùng sàn. | chưa kiểm chứng | `axe-death-ready-R/000009.png` |
| F10 | 0.786000 / 1.035000 | Parry giữ | Kiếm player dựng, rìu thấp chéo, SPACE cyan, +10 mờ. | 9→10 pose giữ, countdown rút; đầu đối thủ hạ nhẹ. | chưa kiểm chứng | `axe-death-ready-R/000010.png` |
| F11 | 1.821000 / 0.074000 | Sau khoảng encode dài | Score 1622; rìu dựng idle, player tay nâng trái; khay an every some. | 10→11 cách PTS 1,035 s: pose đổi qua khoảng giữ encode; không xác nhận mọi cel parry/phản kích trong khoảng này. | chưa kiểm chứng | `axe-death-ready-R/000011.png` |
| F12 | 1.895000 / 0.089000 | Idle / input | Player idle khăn võng; a cyan trong an, rìu dựng grip rõ. | 11→12 tay player hạ về idle, chân không trượt; khay giữ vị trí khi a đổi màu. | chưa kiểm chứng | `axe-death-ready-R/000012.png` |
| F13 | 1.984000 / 0.093000 | Đánh / VFX | Cung ngà che tay/sườn đối thủ, flash vàng phải; score 1632. Khay every some than. | 12→13 an hoàn tất và khay dịch từ; cung/flash xuất hiện, phần grip bị che chưa kết luận. | chưa kiểm chứng | `axe-death-ready-R/000013.png` |
| F14 | 2.077000 / 0.044000 | Đánh giữ cung | Flash tắt, cung ngà còn che nửa trái thân/cán; đầu và tay phải đối thủ đọc được. | 13→14 flash rút, cung giữ, +10 mờ; countdown bắt đầu rút. | chưa kiểm chứng | `axe-death-ready-R/000014.png` |
| F15 | 2.121000 / 0.032000 | Hurt / che cung | Đối thủ sáng ngà toàn thân, tay phải co; rìu dựng trên cung. Player giữ thế đánh thấp. | 14→15 flash vật liệu sáng lên và đầu/tay đổi; không suy diễn grip phía sau cung. | chưa kiểm chứng | `axe-death-ready-R/000015.png` |
| F16 | 2.153000 / 0.085000 | Hurt / recovery | Cung gần hết, player thấp kiếm sát sàn; đối thủ còn sáng ngà, cán dựng và tay nối. | 15→16 cung thu xuống, lộ thân/grip; chân hai actor giữ landmark. | chưa kiểm chứng | `axe-death-ready-R/000016.png` |
| F17 | 2.238000 / 0.015000 | Hurt → idle | Đối thủ hết trắng, tay phải còn co; player thấp, kiếm phải sát sàn. | 16→17 màu trở lại, thân đối thủ co nhẹ; +10 mờ hơn. | chưa kiểm chứng | `axe-death-ready-R/000017.png` |
| F18 | 2.253000 / 0.044000 | Recovery player | Player thân nâng, khăn rủ sau, kiếm thấp chéo phải; rìu giữ cán dựng. | 17→18 player đổi cel hồi thân; chân không trượt, chữ/khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000018.png` |
| F19 | 2.297000 / 0.042000 | Recovery giữ | Player giữ pose trước, đầu đối thủ nâng/rib mở; +10 mờ. | 18→19 đối thủ hồi idle, grip/cán giữ; countdown rút nhẹ. | chưa kiểm chứng | `axe-death-ready-R/000019.png` |
| F20 | 2.339000 / 0.075000 | Recovery / input | Player nâng thân thêm, kiếm chéo thấp; e cyan trong every; score 1632. | 19→20 e đổi màu tại chỗ, actor không đổi sorting; countdown rút. | chưa kiểm chứng | `axe-death-ready-R/000020.png` |
| F21 | 2.414000 / 0.069000 | Recovery giữ | Player cùng pose, đối thủ rìu dựng; ev cyan. | 20→21 v đổi màu, pose giữ và countdown rút; không bỏ frame lặp. | chưa kiểm chứng | `axe-death-ready-R/000021.png` |
| F22 | 2.483000 / 0.077000 | Idle / input | Player về idle khăn rủ cong trái; eve cyan. Rìu/đối thủ đứng tại landmark. | 21→22 kiếm player thu dần khỏi phía trước, thân đứng; khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000022.png` |
| F23 | 2.560000 / 0.084000 | Idle / input | Player idle khăn vòm trái, ever cyan và y ngà; rìu dựng/đầu cao. | 22→23 khăn đổi cel, r đổi màu; countdown rút đều, score/HP giữ. | chưa kiểm chứng | `axe-death-ready-R/000023.png` |
| F24 | 2.644000 / 0.079000 | Đòn kết liễu | Score 1642, HP địch rỗng; player thấp kiếm ngang sàn, rìu còn dựng; slot every trống, some/than ngà. | 23→24 không có cung/flash encode để xem; camera/sàn dịch nhẹ so với HUD cố định. | chưa kiểm chứng | `axe-death-ready-R/000024.png` |
| F25 | 2.723000 / 0.056000 | Death tách xương | Đầu nhấc khỏi cổ, rib tách pelvis, tay phải mở ra; cán rìu dựng bên trái. | 24→25 xuất hiện khe đen cổ/thân; player giữ pose thấp, countdown đầy. | chưa kiểm chứng | `axe-death-ready-R/000025.png` |
| F26 | 2.779000 / 0.057000 | Death giữ | Đầu/rib/pelvis rời, bàn tay phải mở thêm; tay trái còn sát cán, player kiếm sát sàn. | 25→26 phân mảnh tay phải tăng; khay và score giữ. | chưa kiểm chứng | `axe-death-ready-R/000026.png` |
| F27 | 2.836000 / 0.042000 | Death phân rã | Đầu nổi, rib bốn hàng trên hip; chân phải bắt đầu gập, xương tay phải rời sang phải. | 26→27 khoảng tách thân tăng; cán rìu vẫn dựng, player thấp giữ. | chưa kiểm chứng | `axe-death-ready-R/000027.png` |
| F28 | 2.878000 / 0.080000 | Death phân rã | Đầu nghiêng trái, rib rời và xương tay phải văng; rìu dựng chạm tay trái; player thấp. | 27→28 đầu/rib hạ nhẹ, mảnh phải tách xa; foot landmark player giữ. | chưa kiểm chứng | `axe-death-ready-R/000028.png` |
| F29 | 2.958000 / 0.038000 | Death giữ / recovery | Xương địch giữ dạng tách, player nâng thân và kiếm thấp chéo; some/than và score 1642 giữ. | 28→29 player hồi thân, xương tay phải đổi vị trí; countdown vẫn đầy. | chưa kiểm chứng | `axe-death-ready-R/000029.png` |
| F30 | 2.996000 / 0.040000 | Death giữ / recovery | Player đứng cao hơn, khăn rủ dài trái, kiếm thấp; đầu/rib/rìu địch còn dạng trước. | 29→30 đổi cel hồi player; phần chết giữ cel, không bỏ frame lặp. | chưa kiểm chứng | `axe-death-ready-R/000030.png` |
| F31 | 3.036000 / 0.098000 | Death rơi | Rib/đầu hạ, tay trái tách cán, chân rơi thành xương sát sàn; rìu hơi nghiêng phải ở đỉnh. | 30→31 mảnh chết hạ/rải rộng, player giữ recovery; HUD/khay không dịch. | chưa kiểm chứng | `axe-death-ready-R/000031.png` |
| F32 | 3.134000 / 0.071000 | Death rơi / hồi kiếm | Đầu và rib hạ xa nhau, cán nghiêng nhẹ, tay trái gãy thành đoạn; player nâng kiếm chéo thấp. | 31→32 rib sát sàn hơn, mảnh phải văng xa; chân player giữ landmark. | chưa kiểm chứng | `axe-death-ready-R/000032.png` |
| F33 | 3.205000 / 0.034000 | Death giữ / hồi idle | Player đứng thẳng, khăn ngang trái, kiếm thu về; rìu/đầu/rib giữ cel rời, bụi phải tản. | 32→33 player đổi cel, xương giữ và bụi rơi; countdown/khay cố định. | chưa kiểm chứng | `axe-death-ready-R/000033.png` |
| F34 | 3.239000 / 0.060000 | Death rìu đổ | Cán đổ chéo lên phải, đầu sát dưới lưỡi, rib chạm sàn; player idle khăn vòm. | 33→34 rìu xoay rõ quanh điểm gần sàn, xương hạ; không xuyên player. | chưa kiểm chứng | `axe-death-ready-R/000034.png` |
| F35 | 3.299000 / 0.072000 | Death gần sàn | Rìu chéo thấp gần ngang, sọ sát heap, rib/xương rải trên sàn; player idle khăn hất trái. | 34→35 rìu/đầu hạ tiếp, bụi phải tản; HUD 1642 và some/than giữ. | chưa kiểm chứng | `axe-death-ready-R/000035.png` |
| F36 | 3.371000 / 0.099000 | Death giữ | Rìu chéo thấp và sọ/heap giữ pose; player cùng idle khăn cong trái, bụi phải sát sàn. | 35→36 cel actor giữ; bụi đi xuống/phải, khay và HP không đổi. | chưa kiểm chứng | `axe-death-ready-R/000036.png` |
| F37 | 3.470000 / 0.018000 | Death chạm sàn | Cán nằm ngang, sọ nghiêng nằm trên heap thấp; player khăn vòm trái, chân giữ. | 36→37 rìu và sọ rơi nốt xuống sàn; không nhảy sorting qua player. | chưa kiểm chứng | `axe-death-ready-R/000037.png` |
| F38 | 3.488000 / 0.021000 | Death giữ sàn | Heap sọ/rìu nằm ngang, player giữ idle; some/than ngà, countdown đầy. | 37→38 giữ cel chết và player, bụi phải hạ; vẫn giữ frame encode lặp. | chưa kiểm chứng | `axe-death-ready-R/000038.png` |
| F39 | 3.509000 / 0.058000 | Death lún / settle | Sọ thấp hơn trên heap, cán ngang trải sàn; player idle cùng pose, bụi gần tan bên phải. | 38→39 sọ/heap hạ một bước, cán giữ hướng; HUD/khay không đổi. | chưa kiểm chứng | `axe-death-ready-R/000039.png` |
| F40 | 3.567000 / 0.095000 | Death settle giữ | Heap thấp/cán ngang giữ; player khăn vòm, score 1642, some/than ngà, countdown đầy. | 39→40 khăn đổi cel, bụi bên phải tản; xương không đổi landmark. | chưa kiểm chứng | `axe-death-ready-R/000040.png` |
| F41 | 3.662000 / 0.034000 | Death settle | Sọ nhô lên lại trên heap, cán ngang không đổi; player đầu hạ nhẹ, khăn vòm. | 40→41 sọ nâng một bước trong settle; chân player và khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000041.png` |
| F42 | 3.696000 / 0.081000 | Death settle giữ | Heap/sọ giữ cel, player khăn thu gọn ngang trái; bụi chỉ còn ít bên phải. | 41→42 đổi khăn, xương giữ; countdown vẫn đầy và HP địch rỗng. | chưa kiểm chứng | `axe-death-ready-R/000042.png` |
| F43 | 3.777000 / 0.068000 | Chờ cleanup | Sọ/cán giữ tại sàn, player khăn hất dài trái; bụi rất mờ; some/than còn trong khay. | 42→43 khăn đổi cel, bụi tan tiếp; chưa có READY, không đổi score/HP. | chưa kiểm chứng | `axe-death-ready-R/000043.png` |
| F44 | 3.845000 / 0.067000 | Cleanup → READY | Heap/rìu biến mất, READY ngà trong khay căn trái có padding; countdown dưới khay ẩn, không card/nút cũ. | 43→44 thay some/than bằng READY và cleanup cùng frame encode; player/landmark giữ, score 1642. | chưa kiểm chứng | `axe-death-ready-R/000044.png` |
| F45 | 3.912000 / 0.037000 | READY chờ | READY toàn chữ ngà, viền/padding giữ, địch trống; player đầu hạ và khăn cong. | 44→45 chỉ idle đổi cel; countdown vẫn ẩn, không tự intro. | chưa kiểm chứng | `axe-death-ready-R/000045.png` |
| F46 | 3.949000 / 0.069000 | READY giữ | Player đầu nâng, khăn vòm trái; READY/khay giữ vị trí, HP và score không đổi. | 45→46 idle nâng đầu, không dịch chân/camera; glyph không cắt. | chưa kiểm chứng | `axe-death-ready-R/000046.png` |
| F47 | 4.018000 / 0.047000 | READY giữ | Player cùng pose, READY ngà đọc đủ trong khay; sàn trống sau cleanup. | 46→47 pose giữ, torch đổi nhẹ; viền/padding/countdown ẩn không đổi. | chưa kiểm chứng | `axe-death-ready-R/000047.png` |
| F48 | 4.065000 / 0.050000 | READY idle | Player khăn vòng rộng trái, READY ngà giữ đủ nét; sàn trống và countdown ẩn. | 47→48 khăn đổi cel, chân/khay/camera cố định; score 1642. | chưa kiểm chứng | `axe-death-ready-R/000048.png` |
| F49 | 4.115000 / 0.016000 | READY idle đầu hạ | Player đầu hạ nhẹ với khăn vòng, READY giữ màu và vị trí. | 48→49 idle đầu hạ, chân không trượt; không intro hoặc countdown. | chưa kiểm chứng | `axe-death-ready-R/000049.png` |
| F50 | 4.131000 / 0.074000 | READY idle đầu nâng | Player đầu nâng lại, khăn vòng giữ; viền và padding khay nguyên. | 49→50 đầu nâng, chữ đứng yên; HP/score giữ. | chưa kiểm chứng | `axe-death-ready-R/000050.png` |
| F51 | 4.205000 / 0.053000 | READY idle giữ | Cùng pose player, READY ngà đủ năm glyph; sàn/khay và HUD không đổi. | 50→51 giữ pose/chữ, ánh đuốc thay nhẹ; frame lặp được giữ. | chưa kiểm chứng | `axe-death-ready-R/000051.png` |
| F52 | 4.258000 / 0.076000 | READY idle | Player khăn cong rộng trái, READY ngà và padding nguyên; HP/score giữ. | 51→52 khăn đổi cel; sàn trống, countdown ẩn. | chưa kiểm chứng | `axe-death-ready-R/000052.png` |
| F53 | 4.334000 / 0.052000 | READY idle | Khăn player kéo ngang trái và đầu khăn hướng lên; chữ/khay cố định. | 52→53 đổi cel khăn; chân không trượt, không tự chuyển trận. | chưa kiểm chứng | `axe-death-ready-R/000053.png` |
| F54 | 4.386000 / 0.033000 | READY idle giữ | Cùng pose khăn ngang, READY đọc đủ năm chữ ngà; score 1642. | 53→54 giữ pose, ánh đuốc thay nhẹ; glyph và viền không nhảy. | chưa kiểm chứng | `axe-death-ready-R/000054.png` |
| F55 | 4.419000 / 0.055000 | READY idle | Khăn cong hất lên trái, player đứng tại landmark; READY giữ vị trí và màu. | 54→55 đổi đầu khăn, thân/chân giữ; countdown tiếp tục ẩn. | chưa kiểm chứng | `axe-death-ready-R/000055.png` |
| F56 | 4.474000 / 0.056000 | READY idle | Khăn player trở lại vòm, READY ngà đọc đủ, khay không countdown. | 55→56 khăn hạ cong; chân/đầu và HUD giữ. | chưa kiểm chứng | `axe-death-ready-R/000056.png` |
| F57 | 4.530000 / 0.049000 | READY idle giữ | Player giữ khăn vòm và pose đứng, READY/viền/padding giữ nguyên. | 56→57 giữ cel, ánh đuốc thay nhẹ; score 1642. | chưa kiểm chứng | `axe-death-ready-R/000057.png` |
| F58 | 4.579000 / 0.047000 | READY idle | Khăn vòng rộng trái, đầu player cùng độ cao; sàn trống, READY ngà. | 57→58 khăn mở vòng; glyph không dịch/cắt, không intro. | chưa kiểm chứng | `axe-death-ready-R/000058.png` |
| F59 | 4.626000 / 0.039000 | READY idle đầu hạ | Đầu player hạ nhẹ, khăn vòng giữ; READY đủ chữ trong khay. | 58→59 đầu hạ một bước, chân đứng cùng landmark; countdown vẫn ẩn. | chưa kiểm chứng | `axe-death-ready-R/000059.png` |
| F60 | 4.665000 / 0.017000 | READY idle | Đầu player nâng lại, khăn vòng rộng; READY ngà giữ vị trí/padding, sàn trống. | 59→60 đầu nâng, chân và khay giữ; không countdown. | chưa kiểm chứng | `axe-death-ready-R/000060.png` |
| F61 | 4.682000 / 0.048000 | READY idle giữ | Player giữ pose khăn vòng, chữ READY ngà đủ nét; HP và score không đổi. | 60→61 cel giữ; torch thay nhẹ, không nhảy glyph hoặc camera. | chưa kiểm chứng | `axe-death-ready-R/000061.png` |
| F62 | 4.730000 / 0.040000 | READY idle | Khăn thu về vòm trái, đầu cùng độ cao; READY/viền khay giữ. | 61→62 đổi khăn, chân không trượt; không intro/countdown. | chưa kiểm chứng | `axe-death-ready-R/000062.png` |
| F63 | 4.770000 / 0.094000 | READY idle | Khăn cong dài xuống trái, player đứng nguyên; READY toàn ngà, HP/score giữ. | 62→63 khăn hạ, thân/chân và khay ổn định; chưa nhận chữ nào. | chưa kiểm chứng | `axe-death-ready-R/000063.png` |
| F64 | 4.864000 / 0.053000 | READY idle | Khăn ngang, đầu khăn nhọn lên trái; READY ngà và khay giữ, score 1642. | 63→64 khăn nâng; chân, glyph và camera giữ. | chưa kiểm chứng | `axe-death-ready-R/000064.png` |
| F65 | 4.917000 / 0.037000 | READY idle | Khăn ngắn cong dạng U bên trái, đầu/thân giữ; READY ngà. | 64→65 khăn thu, không countdown hay intro. | chưa kiểm chứng | `axe-death-ready-R/000065.png` |
| F66 | 4.954000 / 0.049000 | READY idle | Đầu player hạ, khăn hướng lên trái; chân và READY giữ. | 65→66 đầu hạ trong idle, không trượt chân hoặc chữ. | chưa kiểm chứng | `axe-death-ready-R/000066.png` |
| F67 | 5.003000 / 0.034000 | READY idle | Đầu nâng, khăn thành vòm trái; READY ngà đủ nét và padding. | 66→67 đầu nâng, khay/HUD giữ; không tự chuyển trận. | chưa kiểm chứng | `axe-death-ready-R/000067.png` |
| F68 | 5.037000 / 0.045000 | READY idle | Khăn duỗi ngang trái rồi cong xuống; đầu cao, chân giữ. READY ngà, HP/score giữ. | 67→68 khăn hạ; khay và glyph không dịch. | chưa kiểm chứng | `axe-death-ready-R/000068.png` |
| F69 | 5.082000 / 0.043000 | READY idle giữ | Pose đầu cao/khăn cong giữ; READY ngà và viền/padding nguyên. | 68→69 cel giữ, torch thay nhẹ; không countdown. | chưa kiểm chứng | `axe-death-ready-R/000069.png` |
| F70 | 5.125000 / 0.032000 | READY idle | Khăn vòng rộng, đầu cao; sàn trống và READY ngà rõ. | 69→70 khăn mở vòng; chân/camera và khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000070.png` |
| F71 | 5.157000 / 0.020000 | READY idle | Đầu player hạ, khăn vòng trái; READY giữ đủ nét. | 70→71 đầu hạ, chân không trượt; không intro hay countdown. | chưa kiểm chứng | `axe-death-ready-R/000071.png` |
| F72 | 5.177000 / 0.056000 | READY idle giữ | Đầu thấp, khăn vòng trái giữ; READY toàn ngà, score 1642. | 71→72 giữ cel; khay và chân không dịch. | chưa kiểm chứng | `axe-death-ready-R/000072.png` |
| F73 | 5.233000 / 0.095000 | READY idle | Đầu nâng, khăn ngắn hướng lên trái; READY/padding giữ. | 72→73 đầu nâng, khăn thu; không countdown. | chưa kiểm chứng | `axe-death-ready-R/000073.png` |
| F74 | 5.328000 / 0.016000 | READY idle | Khăn vươn lên trái dạng lưỡi cong, đầu cao; READY rõ. | 73→74 khăn đổi hướng, chân và khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000074.png` |
| F75 | 5.344000 / 0.061000 | READY idle giữ | Pose khăn cong lên trái giữ; HP/score và READY ngà không đổi. | 74→75 cel giữ; không tự vào trận. | chưa kiểm chứng | `axe-death-ready-R/000075.png` |
| F76 | 5.405000 / 0.038000 | READY idle | Đầu thấp, khăn cong lên trái; READY ngà và khay giữ. | 75→76 đầu hạ, khăn uốn; chân giữ. | chưa kiểm chứng | `axe-death-ready-R/000076.png` |
| F77 | 5.443000 / 0.031000 | READY idle | Đầu nâng, khăn dạng võng trái; READY ngà, score 1642. | 76→77 đầu nâng; khay và camera giữ. | chưa kiểm chứng | `axe-death-ready-R/000077.png` |
| F78 | 5.474000 / 0.073000 | READY idle giữ | Pose đầu cao/khăn võng giữ; glyph READY ngà không cắt. | 77→78 giữ cel; chưa có countdown. | chưa kiểm chứng | `axe-death-ready-R/000078.png` |
| F79 | 5.547000 / 0.048000 | READY nhập R | R chuyển cyan, EADY ngà tại chỗ; player khăn duỗi ngang rồi cong xuống. | 78→79 chỉ R đổi màu, glyph không nhảy/padding giữ; HP/score không đổi. | chưa kiểm chứng | `axe-death-ready-R/000079.png` |
| F80 | 5.595000 / 0.089000 | READY nhập R giữ | R cyan/EADY ngà; player đầu cao khăn cong xuống trái. | 79→80 pose và chữ giữ, HP/score không đổi. | chưa kiểm chứng | `axe-death-ready-R/000080.png` |
| F81 | 5.684000 / 0.036000 | READY nhập RE | E chuyển cyan cùng R; ADY ngà, khăn vòng trái. | 80→81 E đổi màu tại chỗ, chân/khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000081.png` |
| F82 | 5.720000 / 0.037000 | READY nhập RE | Đầu hạ, khăn vòng; RE cyan/ADY ngà rõ, padding giữ. | 81→82 đầu hạ trong idle, glyph không dịch. | chưa kiểm chứng | `axe-death-ready-R/000082.png` |
| F83 | 5.757000 / 0.040000 | READY nhập RE | Đầu nâng, khăn ngắn lên trái; RE cyan/ADY ngà. | 82→83 đầu nâng; không countdown hoặc intro, score giữ. | chưa kiểm chứng | `axe-death-ready-R/000083.png` |
| F84 | 5.797000 / 0.071000 | READY nhập RE giữ | Đầu cao khăn ngắn lên trái; RE cyan/ADY ngà giữ. | 83→84 giữ pose/chữ; sàn/camera giữ. | chưa kiểm chứng | `axe-death-ready-R/000084.png` |
| F85 | 5.868000 / 0.049000 | READY nhập REA | A chuyển cyan, DY ngà; khăn cong lên trái, HUD score 1642. | 84→85 A đổi màu tại chỗ; cấu trúc sáng sàn thay nhẹ, khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000085.png` |
| F86 | 5.917000 / 0.054000 | READY nhập REA | Đầu hạ, khăn võng lên trái; REA cyan/DY ngà. | 85→86 đầu hạ, chữ và padding không dịch. | chưa kiểm chứng | `axe-death-ready-R/000086.png` |
| F87 | 5.971000 / 0.069000 | READY nhập REA | Đầu nâng, khăn võng trái; REA cyan/DY ngà rõ. | 86→87 đầu nâng; chân và khay giữ, countdown ẩn. | chưa kiểm chứng | `axe-death-ready-R/000087.png` |
| F88 | 6.040000 / 0.070000 | READY nhập READ | D chuyển cyan, Y ngà; khăn hạ dài trái, player đứng nguyên. | 87→88 D đổi màu tại chỗ; HP/score giữ. | chưa kiểm chứng | `axe-death-ready-R/000088.png` |
| F89 | 6.110000 / 0.053000 | READY nhập READ giữ | READ cyan/Y ngà; khăn dài cong xuống trái giữ, chân/sàn giữ. | 88→89 pose/chữ giữ; countdown ẩn. | chưa kiểm chứng | `axe-death-ready-R/000089.png` |
| F90 | 6.163000 / 0.078000 | READY nhập READ | Khăn vòng rộng, đầu cao; READ cyan/Y ngà đủ nét. | 89→90 khăn mở vòng; glyph và padding giữ. | chưa kiểm chứng | `axe-death-ready-R/000090.png` |
| F91 | 6.241000 / 0.015000 | READY nhập READ giữ | Pose vòng khăn và READ giữ; HUD score 1642 không đổi. | 90→91 giữ cel, không tự chuyển trận. | chưa kiểm chứng | `axe-death-ready-R/000091.png` |
| F92 | 6.256000 / 0.036000 | READY nhập READ giữ | READ cyan/Y ngà, đầu cao khăn vòng trái; HP/score giữ. | 91→92 giữ cel; countdown ẩn. | chưa kiểm chứng | `axe-death-ready-R/000092.png` |
| F93 | 6.292000 / 0.034000 | READY nhập READ | Khăn thu ngắn lên trái, đầu cao; READ/Y giữ vị trí. | 92→93 khăn thu; chân và glyph giữ. | chưa kiểm chứng | `axe-death-ready-R/000093.png` |
| F94 | 6.326000 / 0.084000 | READY nhập READ giữ | Khăn ngắn và đầu cao giữ; READ cyan/Y ngà đủ nét. | 93→94 cel giữ, không intro. | chưa kiểm chứng | `axe-death-ready-R/000094.png` |
| F95 | 6.410000 / 0.019000 | READY nhập READ | Khăn cong lên trái dạng lưỡi, player đứng nguyên; READ/Y giữ. | 94→95 khăn nâng, khay/camera giữ; score 1642. | chưa kiểm chứng | `axe-death-ready-R/000095.png` |
| F96 | 6.429000 / 0.038000 | READY nhập READ | Đầu hạ, khăn cong lên trái; READ cyan/Y ngà giữ. | 95→96 đầu hạ, chân/khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000096.png` |
| F97 | 6.467000 / 0.111000 | READY nhập READ | Đầu nâng, khăn võng trái; chữ và HUD giữ. | 96→97 đầu nâng, không countdown. | chưa kiểm chứng | `axe-death-ready-R/000097.png` |
| F98 | 6.578000 / 0.013000 | READY nhập READ | Khăn duỗi ngang rồi cong xuống, đầu cao; READ/Y giữ. | 97→98 khăn hạ, glyph không dịch. | chưa kiểm chứng | `axe-death-ready-R/000098.png` |
| F99 | 6.591000 / 0.034000 | READY nhập READ giữ | Pose khăn cong xuống giữ; READ cyan/Y ngà, score 1642. | 98→99 cel giữ; không intro. | chưa kiểm chứng | `axe-death-ready-R/000099.png` |
| F100 | 6.625000 / 0.035000 | READY nhập READ | Khăn ngắn hướng lên trái, đầu cao; READ cyan/Y ngà giữ. | 99→100 khăn thu/nâng, chân và glyph giữ. | chưa kiểm chứng | `axe-death-ready-R/000100.png` |
| F101 | 6.660000 / 0.098000 | READY nhập READ | Khăn vòng rộng, đầu cao; READ cyan/Y ngà. | 100→101 khăn mở vòng, HUD/khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000101.png` |
| F102 | 6.758000 / 0.021000 | Hoàn tất READY / wipe | Wipe đen từ dưới lên che gần hết khay; phần đỉnh glyph còn thấy cyan, player/HUD cũ còn. | 101→102 bắt đầu wipe; không có frame toàn Y cyan đọc trọn, không suy diễn pose bị che. | chưa kiểm chứng | `axe-death-ready-R/000102.png` |
| F103 | 6.779000 / 0.035000 | Wipe lên | Mảng đen lên sát dưới chân player, khay bị che toàn; HUD score 1642 và actor trên còn. | 102→103 biên wipe đi lên; player đầu thấp, chân còn trên landmark sàn. | chưa kiểm chứng | `axe-death-ready-R/000103.png` |
| F104 | 6.814000 / 0.076000 | Wipe lên | Biên đen lên quá player; HUD 1642 và phần tường trên còn, actor/khay đã bị che. | 103→104 wipe che cả player và sàn. | chưa kiểm chứng | `axe-death-ready-R/000104.png` |
| F105 | 6.890000 / 0.062000 | Wipe kín | Canvas đen, chỉ label QA trắng góc trên còn. | 104→105 HUD/tường bị che hết. | chưa kiểm chứng | `axe-death-ready-R/000105.png` |
| F106 | 6.952000 / 0.027000 | Wipe kín | Canvas đen và label QA giữ; không actor/khay. | 105→106 giữ khung đen. | chưa kiểm chứng | `axe-death-ready-R/000106.png` |
| F107 | 6.979000 / 0.046000 | Wipe kín | Canvas đen và label QA giữ; không thấy trạng thái nội bộ. | 106→107 tiếp tục giữ đen. | chưa kiểm chứng | `axe-death-ready-R/000107.png` |
| F108 | 7.025000 / 0.015000 | Wipe kín | Toàn canvas đen; label QA cũng không hiện ở frame này. | 107→108 label QA biến mất; không suy diễn actor. | chưa kiểm chứng | `axe-death-ready-R/000108.png` |
| F109 | 7.040000 / 0.117000 | Wipe kín | Canvas đen, actor/HUD/khay chưa hiện. | 108→109 giữ đen. | chưa kiểm chứng | `axe-death-ready-R/000109.png` |
| F110 | 7.157000 / 0.037000 | Wipe kín | Label QA trắng trở lại trên canvas đen. | 109→110 label QA hiện lại; vùng game vẫn đen. | chưa kiểm chứng | `axe-death-ready-R/000110.png` |
| F111 | 7.194000 / 0.036000 | Mở wipe | Dải sàn rất tối lộ ở mép dưới canvas, phần trên còn đen; actor/khay chưa thấy. | 110→111 bắt đầu lộ nền dưới, chưa có pose để kiểm. | chưa kiểm chứng | `axe-death-ready-R/000111.png` |
| F112 | 7.230000 / 0.039000 | Mở wipe | Dải sàn lộ rộng hơn, actor/HUD/khay còn bị che. | 111→112 biên mở đi lên, landmark sàn hiện dần. | chưa kiểm chứng | `axe-death-ready-R/000112.png` |
| F113 | 7.269000 / 0.081000 | Mở wipe | Player idle một mình hiện toàn thân, khăn cong lên trái, chân trên landmark; HUD/khay chưa hiện. | 112→113 player và vùng sàn được lộ, không có di chuyển chân thấy được. | chưa kiểm chứng | `axe-death-ready-R/000113.png` |
| F114 | 7.350000 / 0.018000 | Intro nền | Wipe gần hết, player cùng chân cố định, khăn mở vòm; đuốc/tường hiện đầy đủ, chưa có đối thủ. | 113→114 nền trên mở; khăn đổi cel, không trượt actor. | chưa kiểm chứng | `axe-death-ready-R/000114.png` |
| F115 | 7.368000 / 0.051000 | Spawn chuẩn bị | Player giữ vòm khăn; vệt bụi sáng đầu tiên ở sàn phải, HUD/khay vẫn ẩn. | 114→115 bụi xuất hiện đúng phía đối thủ; landmark/chân player giữ. | chưa kiểm chứng | `axe-death-ready-R/000115.png` |
| F116 | 7.419000 / 0.081000 | Spawn bụi | Bụi ngà bẹt nở trên sàn phải; player đầu thấp, khăn vòng rộng, chân giữ. | 115→116 bụi sáng/nở, không che player. | chưa kiểm chứng | `axe-death-ready-R/000116.png` |
| F117 | 7.500000 / 0.040000 | Spawn bụi | Hai cuộn bụi cao ngà che toàn vùng chân đối thủ; player khăn ngắn lên trái. | 116→117 bụi nở cao; chưa kết luận pose đối thủ bị che. | chưa kiểm chứng | `axe-death-ready-R/000117.png` |
| F118 | 7.540000 / 0.041000 | Spawn Guard | Đầu/rib, khiên đỏ và kiếm chéo lên trái của Guard hiện trên bụi; chân/tay thấp vẫn che, player giữ. | 117→118 Guard xuất hiện phía sau bụi, sorting phần thấy được nhất quán. | chưa kiểm chứng | `axe-death-ready-R/000118.png` |
| F119 | 7.581000 / 0.087000 | Intro Guard | Guard giữ đầu/rib/kiếm, bụi thấp hơn; chữ GUARD/DUEL 4/6 rất mờ trên giữa native, HUD/khay chưa rõ. | 118→119 bụi hạ, khăn player cong lên; không đánh giá chân đang bị che. | chưa kiểm chứng | `axe-death-ready-R/000119.png` |
| F120 | 7.668000 / 0.042000 | Intro Guard | GUARD/DUEL 4/6 ngà rõ ở native; bụi tách ba cụm thấp, kiếm/khiên/grip hiện, chân dưới còn bụi; player đầu thấp. | 119→120 intro sáng, bụi hạ lộ thân/grip. | chưa kiểm chứng | `axe-death-ready-R/000120.png` |
| F121 | 7.710000 / 0.066000 | Intro / HUD vào | HUD 1642 và HP/meter đối thủ đầy đang mờ vào; chân Guard hiện qua bụi mỏng; player khăn cong ngắn. | 120→121 HUD fade vào, bụi mỏng; chân neo sàn. | chưa kiểm chứng | `axe-death-ready-R/000121.png` |
| F122 | 7.776000 / 0.044000 | Intro / HUD | HUD sáng đầy đủ, score1642, HP player giữ mức cũ; bụi hết, Guard hai chân cố định và kiếm chéo trong tay, khay còn ẩn. | 121→122 bụi tan lộ chân; khăn player dài xuống trái, actor không trượt. | chưa kiểm chứng | `axe-death-ready-R/000122.png` |
| F123 | 7.820000 / 0.045000 | Khay combat trở lại | Khay well/see/its ngà và countdown cyan đầy trở lại; intro còn; Guard giữ, player khăn vòm, HUD1642. | 122→123 khay hiện đúng viền/padding, chân/sàn/HUD giữ. | chưa kiểm chứng | `axe-death-ready-R/000123.png` |
| F124 | 7.865000 / 0.078000 | Intro kết | Player khăn vòm, Guard idle kiếm/khiên giữ; khay well/see/its và countdown đầy, score1642. | 123→124 khay sáng đầy; actor/landmark giữ. | chưa kiểm chứng | `axe-death-ready-R/000124.png` |
| F125 | 7.943000 / 0.049000 | Intro mờ đi | Intro giữa mờ, player đầu thấp khăn vòng; Guard/khay/HUD giữ. | 124→125 intro fade ra, khăn đổi cel không trượt chân. | chưa kiểm chứng | `axe-death-ready-R/000125.png` |
| F126 | 7.992000 / 0.033000 | HUD Guard | Intro giữa biến mất, tên GUARD góc phải đang mờ vào; player khăn ngắn trái, hai chân giữ, khay/countdown đầy. | 125→126 tên chuyển sang vị trí HUD; không có wipe lặp. | chưa kiểm chứng | `axe-death-ready-R/000126.png` |
| F127 | 8.025000 / 0.046000 | Combat idle | GUARD góc phải sáng rõ; player/Guard giữ pose gần trước, khay well/see/its không nhảy, HUD1642. | 126→127 tên HUD sáng; chân/grip/khay ổn định. | chưa kiểm chứng | `axe-death-ready-R/000127.png` |
| F128 | 8.071000 / 0.037000 | Combat idle | Player khăn ngắn lên trái; Guard đầu/rib hạ nhẹ, kiếm/grip đi cùng tay, chân giữ; khay countdown đầy. | 127→128 Guard đổi cel idle, không tách grip. | chưa kiểm chứng | `axe-death-ready-R/000128.png` |
| F129 | 8.108000 / 0.052000 | Combat idle | Hai actor giữ pose, score1642, well/see/its ngà giữ. | 128→129 pose giữ, đuốc đổi nhẹ; khay không đổi. | chưa kiểm chứng | `axe-death-ready-R/000129.png` |
| F130 | 8.160000 / 0.072000 | Combat idle | Player khăn cong ngắn trái, Guard giữ đầu thấp/kiếm, chân neo, HUD/khay giữ. | 129→130 khăn đổi cel, không dịch landmark. | chưa kiểm chứng | `axe-death-ready-R/000130.png` |
| F131 | 8.232000 / 0.020000 | Combat idle | Player đầu thấp khăn cong, Guard giữ; countdown vẫn đầy, score1642. | 130→131 player nhún đầu/thân trong idle, chân giữ. | chưa kiểm chứng | `axe-death-ready-R/000131.png` |
| F132 | 8.252000 / 0.061000 | Combat idle | Player đầu cao khăn dài xuống trái, Guard giữ; countdown bắt đầu rút bên phải, glyph vẫn ngà. | 131→132 khăn đổi cel và countdown rút, chân/grip giữ. | chưa kiểm chứng | `axe-death-ready-R/000132.png` |
| F133 | 8.313000 / 0.016000 | Combat idle | Khăn dài xuống trái giữ; countdown ngắn hơn, score1642, Guard idle giữ. | 132→133 pose giữ, countdown rút tiếp. | chưa kiểm chứng | `axe-death-ready-R/000133.png` |
| F134 | 8.329000 / 0.070000 | Combat idle | Actor giữ, well/see/its cùng vị trí; countdown rút thêm. | 133→134 chỉ thấy countdown/đuốc đổi nhẹ, không trượt actor. | chưa kiểm chứng | `axe-death-ready-R/000134.png` |
| F135 | 8.399000 / 0.099000 | Combat idle | Player khăn vòm cao, Guard giữ kiếm/khiên, chân cố định; countdown tiếp tục rút. | 134→135 khăn mở vòm, HUD/khay giữ. | chưa kiểm chứng | `axe-death-ready-R/000135.png` |
| F136 | 8.498000 / 0.018000 | Combat idle | Player đầu thấp khăn vòng, Guard giữ đầu/rib thấp và kiếm/khiên; countdown rút, score1642. | 135→136 player nhún, chân giữ. | chưa kiểm chứng | `axe-death-ready-R/000136.png` |
| F137 | 8.516000 / 0.041000 | Combat idle | Player đầu lên khăn vòng rộng, Guard giữ; well/see/its ngà, countdown ngắn thêm. | 136→137 đầu/thân player nâng, chân không trượt. | chưa kiểm chứng | `axe-death-ready-R/000137.png` |
| F138 | 8.557000 / 0.035000 | Combat idle | Player khăn vòng rộng giữ, Guard grip/kiếm/khiên không đổi; countdown rút. | 137→138 pose giữ, countdown/đuốc đổi. | chưa kiểm chứng | `axe-death-ready-R/000138.png` |
| F139 | 8.592000 / 0.075000 | Combat idle | Player khăn cong ngắn lên trái, Guard cùng pose; HUD1642 và khay giữ. | 138→139 khăn thu, chân/sàn giữ, countdown rút tiếp. | chưa kiểm chứng | `axe-death-ready-R/000139.png` |
| F140 | 8.667000 / 0.050000 | Combat idle | Player khăn cong ngắn giữ, Guard đầu/rib thấp, kiếm đi cùng tay; countdown rút, score1642. | 139→140 Guard hạ cel, chân/khiên neo. | chưa kiểm chứng | `axe-death-ready-R/000140.png` |
| F141 | 8.717000 / 0.034000 | Combat idle | Player đầu thấp, khăn cong; Guard giữ pose thấp; khay ngà và countdown rút. | 140→141 player nhún đầu, chân/grip giữ. | chưa kiểm chứng | `axe-death-ready-R/000141.png` |
| F142 | 8.751000 / 0.076000 | Cue combat | SPACE trắng xuất hiện trên Guard ở native; actor giữ, khay countdown rút. Phần đỉnh cue ngoài pan 4x. | 141→142 cue hiện; không che actor, giới hạn crop cue được ghi. | chưa kiểm chứng | `axe-death-ready-R/000142.png` |
| F143 | 8.827000 / 0.037000 | Cue combat | Player đầu lên khăn dài xuống trái, Guard giữ, SPACE còn; score1642/HP giữ. | 142→143 khăn đổi cel, chân/khay giữ, countdown rút tiếp. | chưa kiểm chứng | `axe-death-ready-R/000143.png` |
| F144 | 8.864000 / 0.015000 | Cue combat | Hai actor giữ pose; player khăn dài xuống trái, Guard đầu thấp; SPACE còn, score1642/HP giữ, countdown rút. | 143→144 pose giữ; thanh thời gian ngắn hơn. | chưa kiểm chứng | `axe-death-ready-R/000144.png` |
| F145 | 8.879000 / 0.037000 | Cue combat | Player khăn vòm, đầu bình thường; Guard giữ grip/khiên/chân; khay well see its ngà. | 144→145 khăn đổi cel, cue và khay neo. | chưa kiểm chứng | `axe-death-ready-R/000145.png` |
| F146 | 8.916000 / 0.058000 | Cue combat | Actor giữ pose, SPACE còn; countdown ngắn hơn, score1642. | 145→146 frame giữ actor, countdown tiếp tục rút. | chưa kiểm chứng | `axe-death-ready-R/000146.png` |
| F147 | 8.974000 / 0.049000 | Cue combat | Player đầu thấp khăn vòng, Guard giữ; SPACE và khay ổn định. | 146→147 player nhún, chân và grip neo. | chưa kiểm chứng | `axe-death-ready-R/000147.png` |
| F148 | 9.023000 / 0.037000 | Cue combat | Player đầu lên khăn ngắn trái; Guard đầu cao kiếm chéo lên trái, khiên đỏ; countdown rút, score1642/HP giữ. | 147→148 đầu player và Guard lên, chân neo. | chưa kiểm chứng | `axe-death-ready-R/000148.png` |
| F149 | 9.060000 / 0.052000 | Cue combat | Actor giữ pose, SPACE trắng viền đen; well see its ngà, thanh rút. | 148→149 pose giữ, countdown ngắn hơn. | chưa kiểm chứng | `axe-death-ready-R/000149.png` |
| F150 | 9.112000 / 0.084000 | Cue combat | Khăn player võng trái dài hơn, Guard giữ grip và chân; HUD1642. | 149→150 khăn đổi cel, cue/khay neo. | chưa kiểm chứng | `axe-death-ready-R/000150.png` |
| F151 | 9.196000 / 0.025000 | Cue combat | Player đầu thấp khăn võng; Guard giữ, countdown rút. | 150→151 player nhún đầu, chân không trượt. | chưa kiểm chứng | `axe-death-ready-R/000151.png` |
| F152 | 9.221000 / 0.039000 | Cue combat | Hai pose giữ, SPACE còn; thanh ngắn hơn dưới khay. | 151→152 frame giữ actor, countdown rút. | chưa kiểm chứng | `axe-death-ready-R/000152.png` |
| F153 | 9.260000 / 0.041000 | Cue combat | Player đầu lên khăn lượn lên trái; Guard giữ, HP/score không đổi. | 152→153 đầu và khăn đổi cel, grip neo. | chưa kiểm chứng | `axe-death-ready-R/000153.png` |
| F154 | 9.301000 / 0.047000 | Cue combat | Khăn player chéo lên trái rõ, Guard giữ kiếm/khiên; well see its ổn định. | 153→154 khăn đổi hướng đầu mút, chân neo. | chưa kiểm chứng | `axe-death-ready-R/000154.png` |
| F155 | 9.348000 / 0.041000 | Cue combat | Actor giữ pose, countdown ngắn tiếp; SPACE trắng còn. | 154→155 pose giữ, khay không nhảy. | chưa kiểm chứng | `axe-death-ready-R/000155.png` |
| F156 | 9.389000 / 0.037000 | Chuẩn bị attack mới | Guard thu tay kiếm về thân, lưỡi phía trái không còn lộ; khiên và chân giữ. Player khăn chéo trái. | 155→156 Guard đổi cel chuẩn bị; chưa có contact. | chưa kiểm chứng | `axe-death-ready-R/000156.png` |
| F157 | 9.426000 / 0.052000 | Chuẩn bị attack mới | Guard giữ tay thu, player khăn vòng trái; SPACE trắng, khay ngà. | 156→157 khăn đổi cel; chân/khiên neo. | chưa kiểm chứng | `axe-death-ready-R/000157.png` |
| F158 | 9.478000 / 0.030000 | Chuẩn bị attack mới | Guard đưa kiếm chéo lên phải ngang vai, tay đi cùng cán; player giữ khăn vòng. | 157→158 kiếm đổi silhouette cùng tay, chưa vung/contact. | chưa kiểm chứng | `axe-death-ready-R/000158.png` |
| F159 | 9.508000 / 0.051000 | Chuẩn bị attack mới | Guard giữ kiếm ngang vai chéo phải, player khăn chéo trái; countdown rút. | 158→159 Guard giữ, khăn đổi cel. | chưa kiểm chứng | `axe-death-ready-R/000159.png` |
| F160 | 9.559000 / 0.050000 | Chuẩn bị attack mới | Guard nghiêng thân, chân trái co, kiếm chéo phải; SPACE đổi nền ngà chữ tối. Player khăn ngắn trái. | 159→160 thân/chân Guard đổi cel, cue đổi màu; không suy ra contact. | chưa kiểm chứng | `axe-death-ready-R/000160.png` |
| F161 | 9.609000 / chưa xác định (cuối media) | Biên cuối media | Pose Guard và player giữ, score1642/HP giữ, well see its ngà; duration cuối chưa xác định. | 160→161 pose giữ, countdown rút. Clip kết thúc trong chuẩn bị đòn mới; không dùng đoạn cuối này chứng nhận recovery attack mới, không phải loop. | chưa kiểm chứng | `axe-death-ready-R/000161.png` |
<!-- ledger-axe-death-ready-R-end -->

### Ledger kiểm trực tiếp — `shield-death-ready-R`

Media SHA256 `1dfb40d57b2a37b13e154bc4f241c0e88df2b067ff2d9e0a64256f24e6dbb7e0`; revision `66749da1`.
Mọi frame dưới đây đã mở full canvas và nearest 4× không thu nhỏ; frame giữ vẫn tính.
Giới hạn: playback người xem và chi tiết bị che còn chờ, nên chưa chứng nhận toàn clip.

| Frame | PTS / duration (s) | Pha quan sát | Nhận xét full/nearest | Chuyển tiếp từ frame trước | Trạng thái | Bằng chứng tạm trong review/ |
|---|---|---|---|---|---|---|
| F0 | 0.000000 / 0.049000 | Combat attack/recovery | Player cúi thấp, cung ngà lớn che tay kiếm/sườn trái Guard, Guard Hurt trắng và +25; score2068, khay keep were use. Canvas/world có độ nhòe encode nhìn thấy. | Đầu media đã ở cuối đòn trước; không dùng đoạn này chứng nhận chuẩn bị đòn trước. | chưa kiểm chứng | `shield-death-ready-R/000000.png` |
| F1 | 0.049000 / 0.073000 | Recovery | Cung thu mảnh dọc trước Guard, màu xương/giáp đỏ trở lại; player vẫn thấp, cán sát sàn; +25 còn. | 0→1 flash hết, cung thu, chân giữ vùng gạch; grip còn che một phần. | chưa kiểm chứng | `shield-death-ready-R/000001.png` |
| F2 | 0.122000 / 0.086000 | Recovery | Cung biến mất, Guard kiếm chéo lên trái nối tay, khiên cạnh thân; player nâng đầu/khăn, kiếm thấp sát sàn. | 1→2 VFX mất để lộ grip; player bắt đầu đứng, camera dịch nhỏ còn HUD neo. | chưa kiểm chứng | `shield-death-ready-R/000002.png` |
| F3 | 0.208000 / 0.075000 | Recovery | Player đứng cao hơn, kiếm dài chéo xuống phải, khăn xuống trái; Guard đầu cao kiếm chéo/khiên cạnh. | 2→3 thân player lên, kiếm đi cùng tay, chân neo; +25 nhạt. | chưa kiểm chứng | `shield-death-ready-R/000003.png` |
| F4 | 0.283000 / 0.070000 | Recovery/input | Player giữ kiếm thấp, Guard khiên xoay ra mặt đỏ trước thân; k cyan trong keep, score2068. | 3→4 khiên đổi góc, k đổi màu tại chỗ, countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000004.png` |
| F5 | 0.353000 / 0.036000 | Recovery/input | Player đứng cao thêm, khăn cong trái, kiếm thu; ke cyan, ep ngà; Guard giữ chân/grip. | 4→5 player nâng thân/thu kiếm, e đổi cyan tại chỗ. | chưa kiểm chứng | `shield-death-ready-R/000005.png` |
| F6 | 0.389000 / 0.081000 | Recovery/input | Player gần đứng thẳng kiếm thu gần hông, khăn vòm; kee cyan p ngà, Guard idle kiếm chéo/khiên đỏ. | 5→6 kiếm thu cùng tay, ký tự thứ ba đổi cyan; chân giữ. | chưa kiểm chứng | `shield-death-ready-R/000006.png` |
| F7 | 0.470000 / 0.050000 | Combat attack | Player cúi vung cung ngà, flash vàng cam che grip/sườn Guard; +14/score2082/x1.5; were use let ngà, thanh nạp đầy. | 6→7 đòn mới và shake làm world dịch, HUD/khay neo; không suy ra chi tiết nằm sau flash. | chưa kiểm chứng | `shield-death-ready-R/000007.png` |
| F8 | 0.520000 / 0.107000 | Attack/cue | Cung ngà và hạt đỏ cam che tay kiếm/rib trái Guard; +14 còn, SPACE nền ngà xuất hiện; score2082, were use let. | 7→8 flash thu còn hạt, cue hiện; không nhìn xuyên cung. | chưa kiểm chứng | `shield-death-ready-R/000008.png` |
| F9 | 0.627000 / 0.016000 | Parry | Player kiếm dựng đứng nối hai tay, Guard kiếm ngang trái hướng player; SPACE nền cyan, +14; chân ở cùng landmark. | 8→9 cung mất, player dựng kiếm, Guard đổi kiếm ngang; camera trở nhẹ, HUD neo. | chưa kiểm chứng | `shield-death-ready-R/000009.png` |
| F10 | 0.643000 / 0.097000 | Parry hold | Hai actor giữ pose kiếm đứng/ngang, SPACE cyan; score2082, countdown rút. | 9→10 pose giữ, thanh thời gian ngắn hơn. | chưa kiểm chứng | `shield-death-ready-R/000010.png` |
| F11 | 0.740000 / 0.029000 | Parry contact | Spark vàng ở đầu kiếm player và đầu kiếm Guard ngang trái; score2130/+62/x1.6, khiên Guard khuất phía phải thân. | 10→11 kiếm Guard vươn ngang cùng tay; spark che điểm tiếp xúc, không chứng nhận chi tiết grip sau spark. | chưa kiểm chứng | `shield-death-ready-R/000011.png` |
| F12 | 0.769000 / 0.035000 | Parry contact hold | Player giữ kiếm đứng, Guard tay kiếm ngang trái; spark vàng kéo vệt trái, +62/2130; countdown rút. | 11→12 actor giữ, spark thay hình và hạt bay trái; grip contact vẫn che. | chưa kiểm chứng | `shield-death-ready-R/000012.png` |
| F13 | 0.804000 / 0.053000 | Phản kích prep | Player cúi thấp, kiếm sát sàn kéo trái, khăn dựng vòng; Guard vẫn kiếm ngang và khiên dang phải, spark ở đầu kiếm. | 12→13 player hạ thân/kiếm, Guard sáng hơn; chân giữ vùng gạch, world dịch nhỏ. | chưa kiểm chứng | `shield-death-ready-R/000013.png` |
| F14 | 0.857000 / 0.058000 | Phản kích prep hold | Pose player thấp và Guard ngang giữ, spark vàng nở trên tay kiếm; score2130, were use let. | 13→14 actor giữ, spark nở che lưỡi/điểm contact; HUD neo. | chưa kiểm chứng | `shield-death-ready-R/000014.png` |
| F15 | 0.915000 / 0.059000 | Phản kích prep hold | Hai pose giữ; spark còn, hạt vàng cam tản trái; countdown rút, HP player giữ. | 14→15 frame giữ actor, hạt dịch trái, không suy ra grip sau spark. | chưa kiểm chứng | `shield-death-ready-R/000015.png` |
| F16 | 0.974000 / 0.064000 | Phản kích prep hold | Player thấp kiếm sát sàn, Guard tay ngang giữ; spark thu cam đỏ, hạt tản trái; +62 nhạt. | 15→16 pose giữ, spark co nhỏ, countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000016.png` |
| F17 | 1.038000 / 0.033000 | Phản kích prep hold | Actor giữ, hạt đỏ cam xa hơn bên trái, spark nhỏ còn trên kiếm Guard. | 16→17 pose giữ, hạt tiếp tục tản; không bỏ frame giữ. | chưa kiểm chứng | `shield-death-ready-R/000017.png` |
| F18 | 1.071000 / 0.052000 | Enemy recovery | Guard thu kiếm chéo lên trái cùng tay, khiên trở cạnh thân; player vẫn cúi thấp khăn vòng, kiếm trên sàn. | 17→18 tay/kiếm Guard hạ về chéo, chân giữ; spark nhỏ ở đầu lưỡi. | chưa kiểm chứng | `shield-death-ready-R/000018.png` |
| F19 | 1.123000 / 0.095000 | Enemy recovery hold | Hai pose giữ, spark/hạt đỏ cam thưa, +62 rất nhạt; score2130, were use let ngà. | 18→19 frame giữ, hạt rơi và countdown rút; grip Guard lộ ngoài spark. | chưa kiểm chứng | `shield-death-ready-R/000019.png` |
| F20 | 1.218000 / 0.022000 | Idle / player thấp | Guard về idle kiếm chéo lên trái, khiên đỏ hướng trước; player còn thấp, kiếm sát sàn trái, khăn vòng lên. Spark hết, ít hạt rơi trái; score2130 HP10. | 19→20 Guard thu xong, grip lộ; chân và landmark sàn giữ. | chưa kiểm chứng | `shield-death-ready-R/000020.png` |
| F21 | 1.240000 / 0.055000 | Giữ pose | Hai actor giữ pose; +62 nhạt hơn, were use let ngà và countdown rút. | 20→21 frame giữ, chữ và khay giữ vị trí. | chưa kiểm chứng | `shield-death-ready-R/000021.png` |
| F22 | 1.295000 / 0.074000 | Giữ pose | Pose giữ, +62 đã hết; ít hạt cam gần sàn trái, score2130 HP10 không đổi. | 21→22 popup hết, hạt hạ thấp; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000022.png` |
| F23 | 1.369000 / 0.017000 | Giữ pose | Hai actor giữ, hạt thưa thấp hơn bên trái; were use let đọc được nhưng media mềm, countdown ngắn hơn. | 22→23 giữ pose và chân, hạt tiếp tục rơi. | chưa kiểm chứng | `shield-death-ready-R/000023.png` |
| F24 | 1.386000 / 0.041000 | Giữ pose thấp | Player thấp kiếm sàn trái khăn vòng; Guard idle kiếm chéo lên trái khiên trước, hạt cam gần sàn trái. | 23→24 actor giữ, countdown rút, landmark sàn giữ. | chưa kiểm chứng | `shield-death-ready-R/000024.png` |
| F25 | 1.427000 / 0.035000 | Giữ pose thấp | Pose giữ, hạt cam trái hạ thấp; were use let ngà, score2130 x1.6 HP10. | 24→25 không bỏ frame giữ; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000025.png` |
| F26 | 1.462000 / 0.048000 | Giữ pose thấp | Actor giữ, hạt thưa xa chân player; grip Guard lộ, media mềm. | 25→26 pose và chân giữ, hạt rơi; rail không dịch. | chưa kiểm chứng | `shield-death-ready-R/000026.png` |
| F27 | 1.510000 / 0.058000 | Giữ pose thấp | Hai pose giữ, ít hạt cam sát sàn trái; khay và HUD nguyên vị trí. | 26→27 frame giữ, countdown tiếp tục rút. | chưa kiểm chứng | `shield-death-ready-R/000027.png` |
| F28 | 1.568000 / 0.053000 | Giữ pose thấp | Player vẫn thấp, kiếm sàn trái khăn vòng, Guard idle với khiên đỏ và kiếm chéo; ít hạt cam sàn trái. | 27→28 pose giữ, countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000028.png` |
| F29 | 1.621000 / 0.080000 | Player nâng kiếm | Player đứng lên, tay và kiếm ngang ra sau trái ngang vai, khăn cong cao trái; Guard giữ idle. | 28→29 đầu/thân player lên cùng grip, chân neo sàn. | chưa kiểm chứng | `shield-death-ready-R/000029.png` |
| F30 | 1.701000 / 0.014000 | Player thu kiếm | Player gần đứng thẳng, kiếm sau vai hướng trái còn vài pixel sáng; khăn chéo lên trái, Guard giữ. | 29→30 thân đứng, tay/kiếm thu, chân không trượt. | chưa kiểm chứng | `shield-death-ready-R/000030.png` |
| F31 | 1.715000 / 0.038000 | Player recovery | Player đứng, tay hạ dọc thân, vài pixel lưỡi kiếm sau vai trái; khăn chéo trái, Guard không đổi. | 30→31 tay hạ, grip/kiếm thu theo; rail ngà và countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000031.png` |
| F32 | 1.753000 / 0.047000 | Player idle | Player tay hạ kiếm đã khuất sau vai, khăn ngắn cong lên trái; Guard idle cao, grip kiếm đỏ lộ. | 31→32 hoàn tất thu tay, chân giữ landmark; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000032.png` |
| F33 | 1.800000 / 0.064000 | Idle | Guard đầu và rib hạ nhẹ, kiếm/khiên cùng thân; player giữ, khăn ngắn. | 32→33 đầu Guard bob xuống, chân neo; không có flash. | chưa kiểm chứng | `shield-death-ready-R/000033.png` |
| F34 | 1.864000 / 0.092000 | Idle / gõ w | Player đầu hạ nhẹ khăn gợn trái; Guard giữ thấp, w cyan trong were, phần còn lại ngà. | 33→34 đổi màu w tại chỗ, actor idle không trượt. | chưa kiểm chứng | `shield-death-ready-R/000034.png` |
| F35 | 1.956000 / 0.058000 | Idle / gõ we | Guard đầu/rib lên, player khăn dài ngang trái; we cyan, re ngà, score2130 HP10. | 34→35 e đổi màu tại chỗ, Guard lên cùng kiếm/khiên, chân giữ. | chưa kiểm chứng | `shield-death-ready-R/000035.png` |
| F36 | 2.014000 / 0.071000 | Idle / gõ wer | wer cyan, e ngà, player khăn dài trái đầu thấp, Guard giữ idle cao. | 35→36 r đổi màu tại chỗ, chân hai actor giữ. | chưa kiểm chứng | `shield-death-ready-R/000036.png` |
| F37 | 2.085000 / 0.038000 | Attack / flash | Player thấp tay vung phải, cung ngà che tay/rib trái Guard; flash trắng vàng che khiên và thân, +17 score2147 x1.8, enemyHP rất ít. Khay trượt use let và mép th. | 36→37 attack và shake cảnh, HUD/khay neo; grip Guard bị che chưa kết luận. | chưa kiểm chứng | `shield-death-ready-R/000037.png` |
| F38 | 2.123000 / 0.080000 | Attack / flash tan | Cung ngà còn phủ grip/rib, flash co thành hạt cam đỏ, phần khiên lộ màu; player giữ thấp, khay use let these đầy đủ. | 37→38 flash nhỏ, hạt tản, chữ trượt xong; chân theo shake cảnh. | chưa kiểm chứng | `shield-death-ready-R/000038.png` |
| F39 | 2.203000 / 0.055000 | Attack / arc | Flash hết, Guard màu thường, cung còn che tay trái và rib; player giữ thấp, +17 trên đầu Guard. | 38→39 hạt hết, cung còn giữ; countdown rút, score2147 HP10. | chưa kiểm chứng | `shield-death-ready-R/000039.png` |
| F40 | 2.258000 / 0.078000 | Enemy Hurt / arc | Guard sáng ngà toàn thân, đầu quay và khiên nghiêng cạnh, cung còn che tay trái; player thấp, +17 giữ. | 39→40 Hurt sáng đổi pose, chân neo; grip bị cung che. | chưa kiểm chứng | `shield-death-ready-R/000040.png` |
| F41 | 2.336000 / 0.015000 | Arc tan / Hurt hết | Guard màu thường, rib lộ, cung thu mảnh trên kiếm player; player vẫn thấp, khăn vòng ngang. | 40→41 Hurt hết, cung tách mảnh rồi thu; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000041.png` |
| F42 | 2.351000 / 0.042000 | Recovery | Cung hết, Guard idle thấp với kiếm chéo grip đỏ, khiên cạnh phải; player thấp kiếm nằm ngang sát sàn phải khăn vòng. | 41→42 cung biến mất lộ grip, không thấy vũ khí rời tay. | chưa kiểm chứng | `shield-death-ready-R/000042.png` |
| F43 | 2.393000 / 0.018000 | Player recovery | Player nâng đầu/thân, kiếm vẫn thấp chéo phải, khăn rủ dọc trái; Guard giữ idle, +17 nhạt. | 42→43 player đứng dần, tay/kiếm theo thân, chân giữ. | chưa kiểm chứng | `shield-death-ready-R/000043.png` |
| F44 | 2.411000 / 0.092000 | Recovery hold | Player nửa đứng kiếm thấp chéo phải khăn rủ trái; Guard giữ idle thấp, +17 mờ, use let these ngà. | 43→44 frame giữ, countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000044.png` |
| F45 | 2.503000 / 0.049000 | Recovery | Player cao hơn, kiếm phẳng chéo xuống phải rõ, khăn dài rủ; Guard đầu/rib lên với kiếm/khiên, +17 rất nhạt. | 44→45 actor nâng thân cùng grip; chân giữ landmark. | chưa kiểm chứng | `shield-death-ready-R/000045.png` |
| F46 | 2.552000 / 0.047000 | Recovery hold | Hai pose giữ, tay player vẫn cầm kiếm thấp phải; rail ngà, HP10 score2147. | 45→46 frame giữ, popup mờ hơn và countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000046.png` |
| F47 | 2.599000 / 0.074000 | Recovery / gõ u | Player quay mặt trước và thu kiếm ngang phải, khăn cong trái; u cyan, se ngà. Guard giữ idle. | 46→47 tay/kiếm thu về thân, u đổi màu tại chỗ; chân giữ. | chưa kiểm chứng | `shield-death-ready-R/000047.png` |
| F48 | 2.673000 / 0.082000 | Thu kiếm / gõ us | Player đứng thu tay, vài pixel kiếm ngang phải còn thấy; us cyan e ngà. Guard idle và +17 gần hết. | 47→48 s đổi màu, kiếm thu theo tay, chân giữ. | chưa kiểm chứng | `shield-death-ready-R/000048.png` |
| F49 | 2.755000 / 0.044000 | Đòn kết liễu | Player thấp kiếm ngang sát sàn phải; Guard còn đứng kiếm chéo khiên đỏ, không có cung/flash ở frame encode này; +19 score2166 x1.9 enemyHP rỗng. Khay còn let these. | 48→49 attack thấp và shake cảnh; không suy diễn cel flash đã nhìn thấy. | chưa kiểm chứng | `shield-death-ready-R/000049.png` |
| F50 | 2.799000 / 0.063000 | Death bắt đầu | Guard đầu/rib còn thẳng, xương bàn tay hiện trên khiên, kiếm chéo giữ; player thấp; +19 và score2166 giữ. | 49→50 xương tay đổi vị trí trên khiên; sàn/cảnh còn shake, HUD neo. | chưa kiểm chứng | `shield-death-ready-R/000050.png` |
| F51 | 2.862000 / 0.079000 | Death / tay tách | Bàn tay xương trôi xa phải ra mép khiên, Guard đầu/rib đứng, kiếm chéo, player giữ thấp. | 50→51 tay tách tiếp qua mặt khiên; chân chưa đổ, +19 giữ, countdown đầy. | chưa kiểm chứng | `shield-death-ready-R/000051.png` |
| F52 | 2.941000 / 0.104000 | Death tách xương | Tay xương tách phải qua khiên, một mảnh xương trái gần kiếm; đầu/rib còn gần thân, player thấp. | 51→52 xương bay hai bên, khiên/kiếm còn cao, cảnh hết phần lớn shake. | chưa kiểm chứng | `shield-death-ready-R/000052.png` |
| F53 | 3.045000 / 0.034000 | Death phân rã | Đầu tách lên khỏi rib, hông và chân tách, kiếm nghiêng trái còn cùng bàn tay; khiên cao bên phải với mảnh xương. | 52→53 các khối tách rõ, player nâng thân; rail let these giữ. | chưa kiểm chứng | `shield-death-ready-R/000053.png` |
| F54 | 3.079000 / 0.047000 | Death phân rã | Kiếm xoay thấp về ngang trái, rib/hông cách nhau, hai chân gập rời; đầu treo trên, khiên đỏ giữa đám xương. | 53→54 kiếm xoay, chi rơi, player lên; không gọi đây idle. | chưa kiểm chứng | `shield-death-ready-R/000054.png` |
| F55 | 3.126000 / 0.048000 | Death phân rã | Đầu/rib/hông giữ phân tách, tay xương phải tản rộng gần biên pan; player gần đứng kiếm chéo phải. | 54→55 mảnh phải bay rộng cần pan bổ sung; native cho thấy chưa ra canvas. | chưa kiểm chứng | `shield-death-ready-R/000055.png` |
| F56 | 3.174000 / 0.064000 | Death phân rã | Đầu treo trên rib, kiếm chéo trái khiên giữa cao, chân gập rời sát sàn; xương phải tản rộng đã mở pan850,280–1120,510. Player thu kiếm. | 55→56 xương phải ra xa, các mảnh xoay, không bị cắt canvas; +19 mờ. | chưa kiểm chứng | `shield-death-ready-R/000056.png` |
| F57 | 3.238000 / 0.047000 | Death rơi | Khiên hạ dưới rib, chân rơi thấp, đầu vẫn cao; kiếm chéo trái và tay theo; xương phải tiếp tục tản. | 56→57 khiên/chi hạ, player gần idle, pan phải đủ cấu trúc. | chưa kiểm chứng | `shield-death-ready-R/000057.png` |
| F58 | 3.285000 / 0.054000 | Death rơi | Rib hạ và co, hai cánh tay dang, kiếm gần ngang trái, khiên nghiêng phải thấp; chân đổ thành đống. | 57→58 rib/arms rơi tách, đầu ở trên; +19 gần hết, pan phải thấy mảnh xoay xuống. | chưa kiểm chứng | `shield-death-ready-R/000058.png` |
| F59 | 3.339000 / 0.082000 | Death giữ pha | Đầu/rib/kiếm/khiên giữ pha, xương phải tản xa và thấp hơn; player thu kiếm gần hết, khăn cong trái. | 58→59 frame giữ các khối chính, mảnh phải hạ/xoay; +19 hết, HP10 score2166. | chưa kiểm chứng | `shield-death-ready-R/000059.png` |
| F60 | 3.421000 / 0.054000 | Death rơi | Đầu bắt đầu hạ, rib thấp, kiếm ngang trái, khiên gần sàn; mảnh phải vẫn trong canvas. Player idle khăn ngắn. | 59→60 đầu/rib hạ, khiên xoay mặt ra; pan rộng thấy các mảnh phải không cắt. | chưa kiểm chứng | `shield-death-ready-R/000060.png` |
| F61 | 3.475000 / 0.084000 | Death giữ pha | Các khối chính giữ, mảnh xương phải thấp hơn và tản rộng; khay let these, countdown đầy. | 60→61 đầu và heap giữ, mảnh phải đổi góc; player hạ đầu trong idle. | chưa kiểm chứng | `shield-death-ready-R/000061.png` |
| F62 | 3.559000 / 0.041000 | Death sát sàn | Đầu xuống gần độ cao đầu player; rib sát sàn, kiếm ngang và khiên nghiêng phẳng phải, xương rải bên phải. | 61→62 đầu/rib hạ rõ, khiên xoay phẳng; landmark sàn giữ. | chưa kiểm chứng | `shield-death-ready-R/000062.png` |
| F63 | 3.600000 / 0.054000 | Death sát sàn | Đầu xuống ngang torso cũ, helmet đỏ/face rỗng còn đọc được; kiếm trái, khiên và heap sát sàn. | 62→63 đầu hạ tiếp, mảnh phải hạ và xoay; pan650,280–1200,570 bao trọn heap. | chưa kiểm chứng | `shield-death-ready-R/000063.png` |
| F64 | 3.654000 / 0.075000 | Death sát sàn | Đầu giữ hơi nghiêng cao trên heap; kiếm ngang trái, khiên nghiêng phải, xương phải còn bay. Player idle khăn vòm trái. | 63→64 các khối chính giữ, mảnh phải đổi góc/hạ; pan rộng thấy trọn. | chưa kiểm chứng | `shield-death-ready-R/000064.png` |
| F65 | 3.729000 / 0.019000 | Death đổ đầu | Đầu lật xuống gần nằm ngang trên heap, khiên/kiếm giữ sát sàn; xương phải tản. | 64→65 đầu hạ và xoay rõ; player hạ đầu, chân giữ landmark. | chưa kiểm chứng | `shield-death-ready-R/000065.png` |
| F66 | 3.748000 / 0.045000 | Death giữ heap | Đầu/helmet nghiêng nằm cạnh rib, kiếm trái khiên phải, mảnh phải hạ tiếp. Khay let these countdown đầy. | 65→66 heap giữ, mảnh phải xoay; HP10 score2166 không đổi. | chưa kiểm chứng | `shield-death-ready-R/000066.png` |
| F67 | 3.793000 / 0.032000 | Death heap | Đầu và xương sát sàn, khiên nằm nghiêng phẳng phải, kiếm ngang trái; player ngẩng đầu khăn chéo trái. | 66→67 đầu/heap hạ nhẹ, mảnh phải hạ; không bị cắt canvas, pan rộng bao trọn. | chưa kiểm chứng | `shield-death-ready-R/000067.png` |
| F68 | 3.825000 / 0.077000 | Death heap | Đầu/heap giữ sát sàn; kiếm ngang trái khiên phẳng phải; player idle khăn chéo trái. | 67→68: Pan phải bổ sung nearest4× (650,280)–(1200,570): heap xương/kiếm trái và khiên đỏ phải sát sàn; vài mảnh sáng phải đang hạ. | chưa kiểm chứng | `shield-death-ready-R/000068.png` |
| F69 | 3.902000 / 0.016000 | Death heap | Đầu/kiếm giữ, khiên phẳng sát nền; player khăn vươn ngang trái, HP10 score2166. | 68→69: Pan phải bổ sung nearest4×: heap giữ, các mảnh phải hạ gần nền và ít hơn F68. | chưa kiểm chứng | `shield-death-ready-R/000069.png` |
| F70 | 3.918000 / 0.035000 | Death heap giữ | Đầu/heap/vũ khí giữ; player đầu cao khăn U trái; HUD/khay không đổi. | 69→70: Pan phải bổ sung nearest4×: heap giữ; mảnh phải ở cùng cấu trúc thấp như F69, không còn khối lớn ngoài crop. | chưa kiểm chứng | `shield-death-ready-R/000070.png` |
| F71 | 3.953000 / 0.048000 | Death heap giữ | Heap vẫn sát sàn, khiên đỏ thấp phải kiếm trái; player giữ đầu cao khăn U. | 70→71: Pan phải bổ sung nearest4×: heap và khiên thấp giữ; các mảnh phải thưa, gần nền. | chưa kiểm chứng | `shield-death-ready-R/000071.png` |
| F72 | 4.001000 / 0.080000 | Death heap cuối | Heap giữ sát sàn, còn mảnh nhỏ native phía phải; player đầu thấp khăn gập ngang trái. | 71→72: Pan phải bổ sung nearest4×: heap giữ trước cleanup F73; phần phải chỉ còn các chấm mờ sát sàn. | chưa kiểm chứng | `shield-death-ready-R/000072.png` |
| F73 | 4.081000 / 0.050000 | Cleanup → READY | Heap/kiếm/khiên đã biến mất; READY ngà nằm bên trái khay, padding rõ, countdown ẩn, không card/nút cũ. | 72→73 cleanup cùng đổi khay; world/camera đổi framing, HUD neo score2166 HP10. | chưa kiểm chứng | `shield-death-ready-R/000073.png` |
| F74 | 4.131000 / 0.083000 | Chờ READY | READY giữ ngà đầy đủ, viền vàng/trái và xám dưới, khoảng trống phải rộng; player đầu cao khăn vòm. | 73→74 camera/sàn thay đổi nhẹ còn READY và HUD giữ vị trí. | chưa kiểm chứng | `shield-death-ready-R/000074.png` |
| F75 | 4.214000 / 0.024000 | Chờ READY | READY không cắt chữ, countdown vẫn ẩn; player hạ đầu khăn U ngang trái. | 74→75 chỉ idle/torch thay đổi thấy rõ, HP10 score2166 giữ; chưa có intro. | chưa kiểm chứng | `shield-death-ready-R/000075.png` |
| F76 | 4.238000 / 0.048000 | Chờ READY | READY ngà toàn bộ, padding/viền giữ, không countdown; player đầu thấp khăn vòng trái. | 75→76 giữ pha idle, sàn/khay/HUD neo. | chưa kiểm chứng | `shield-death-ready-R/000076.png` |
| F77 | 4.286000 / 0.017000 | Chờ READY | Player ngẩng đầu khăn chéo ngắn trái; READY vẫn nguyên không đổi màu. | 76→77 đầu lên, khăn đổi cel, chân không trượt thấy rõ; score2166. | chưa kiểm chứng | `shield-death-ready-R/000077.png` |
| F78 | 4.303000 / 0.075000 | Chờ READY giữ | Player/khăn giữ cel, READY giữ vị trí và màu, cảnh không đối thủ. | 77→78 held pose, torch biến đổi nhẹ; chưa chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000078.png` |
| F79 | 4.378000 / 0.047000 | Chờ READY | Player khăn U ngang trái, đầu cao; READY ngà, viền khay đầy đủ. | 78→79 khăn đổi pha, glyph không nhảy/cắt; countdown ẩn HP10. | chưa kiểm chứng | `shield-death-ready-R/000079.png` |
| F80 | 4.425000 / 0.079000 | Chờ READY giữ | Player đầu cao khăn U ngang trái, READY ngà đầy đủ; khay và sàn ổn định. | 79→80 giữ pose, torch đổi nhẹ; không countdown/đối thủ. | chưa kiểm chứng | `shield-death-ready-R/000080.png` |
| F81 | 4.504000 / 0.038000 | Chờ READY | Player hạ đầu, khăn gập lên trái; READY giữ từng glyph ngà. | 80→81 idle đổi cel, chân giữ sàn, score2166 HP10. | chưa kiểm chứng | `shield-death-ready-R/000081.png` |
| F82 | 4.542000 / 0.060000 | Chờ READY | Khăn chéo xuống trái, player đầu thấp; chữ/viền/padding READY giữ. | 81→82 khăn đổi, glyph không di chuyển, không intro. | chưa kiểm chứng | `shield-death-ready-R/000082.png` |
| F83 | 4.602000 / 0.059000 | Chờ READY giữ | Pose player đầu thấp khăn chéo trái giữ; READY ngà và HUD không đổi. | 82→83 held pose, torch biến thiên, countdown vẫn ẩn. | chưa kiểm chứng | `shield-death-ready-R/000083.png` |
| F84 | 4.661000 / 0.017000 | Chờ READY | Player đầu cao khăn vòng trái; READY giữ màu ngà và padding, HUD2166/HP10. | 83→84 đầu lên và khăn cuộn, chân giữ sàn; không countdown. | chưa kiểm chứng | `shield-death-ready-R/000084.png` |
| F85 | 4.678000 / 0.054000 | Chờ READY giữ | Đầu cao, khăn vòng trái giữ; READY không đổi glyph hoặc căn vị trí. | 84→85 held pose, torch đổi nhẹ; không chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000085.png` |
| F86 | 4.732000 / 0.060000 | Chờ READY | Player đầu thấp khăn U trái; READY ngà toàn bộ, viền khay không cắt. | 85→86 idle hạ đầu, glyph giữ; score và HP không đổi. | chưa kiểm chứng | `shield-death-ready-R/000086.png` |
| F87 | 4.792000 / 0.038000 | Chờ READY | Player đầu cao khăn U dài trái; READY ngà, khoảng trống phải giữ. | 86→87 đầu lên, khăn duỗi; countdown ẩn, không đối thủ/intro. | chưa kiểm chứng | `shield-death-ready-R/000087.png` |
| F88 | 4.830000 / 0.016000 | Chờ READY | Player đầu cao khăn chéo ngắn trái; READY nguyên ngà và padding. | 87→88 khăn rút ngắn, chân giữ sàn, HUD không đổi. | chưa kiểm chứng | `shield-death-ready-R/000088.png` |
| F89 | 4.846000 / 0.050000 | Chờ READY giữ | Player/khăn giữ cel cao, READY ngà không cắt; không countdown. | 88→89 held pose, torch đổi nhỏ; không tự chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000089.png` |
| F90 | 4.896000 / 0.034000 | Chờ READY | Player khăn U dài trái, đầu cao; READY và viền giữ vị trí. | 89→90 khăn đổi pha, sàn/HP10/score2166 giữ. | chưa kiểm chứng | `shield-death-ready-R/000090.png` |
| F91 | 4.930000 / 0.071000 | Chờ READY giữ | Pose player cao khăn U giữ; READY ngà trọn vẹn, khay còn trống phải. | 90→91 held pose, không intro/countdown; glyph không đổi màu. | chưa kiểm chứng | `shield-death-ready-R/000091.png` |
| F92 | 5.001000 / 0.075000 | Chờ READY | Player ngẩng đầu, khăn gấp lên trái, READY ngà giữ trọn glyph. | 91→92 khăn đổi, chân và khay neo; không tự chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000092.png` |
| F93 | 5.076000 / 0.047000 | Chờ READY | Player hạ đầu khăn chéo xuống trái; READY giữ padding và viền. | 92→93 đầu xuống, glyph không nhảy; HP10 score2166. | chưa kiểm chứng | `shield-death-ready-R/000093.png` |
| F94 | 5.123000 / 0.047000 | Chờ READY giữ | Player đầu thấp khăn chéo trái giữ; READY ngà, countdown ẩn. | 93→94 held pose, nền torch đổi nhẹ; không intro. | chưa kiểm chứng | `shield-death-ready-R/000094.png` |
| F95 | 5.170000 / 0.034000 | Chờ READY | Player đầu cao khăn vòng trái; READY giữ vị trí/màu toàn từ. | 94→95 đầu lên, khăn cuộn; sàn và HUD giữ. | chưa kiểm chứng | `shield-death-ready-R/000095.png` |
| F96 | 5.204000 / 0.057000 | Chờ READY giữ | Player đầu cao khăn vòng trái giữ, READY ngà toàn từ, countdown ẩn. | 95→96 held pose, nền torch đổi nhẹ; không chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000096.png` |
| F97 | 5.261000 / 0.070000 | Chờ READY | Player đầu thấp khăn U trái; READY và khay giữ, HP10 score2166. | 96→97 idle hạ đầu, chân neo; glyph không nhảy. | chưa kiểm chứng | `shield-death-ready-R/000097.png` |
| F98 | 5.331000 / 0.019000 | Chờ READY | Player đầu cao khăn chéo ngắn trái; READY ngà trọn vẹn padding giữ. | 97→98 đầu lên, khăn đổi pha, không intro. | chưa kiểm chứng | `shield-death-ready-R/000098.png` |
| F99 | 5.350000 / 0.033000 | Chờ READY giữ | Pose cao khăn chéo ngắn giữ; READY và viền/padding không đổi. | 98→99 held pose, countdown ẩn; HUD giữ. | chưa kiểm chứng | `shield-death-ready-R/000099.png` |
| F100 | 5.383000 / 0.046000 | Chờ READY | Player đầu cao khăn chéo trái dài; READY ngà, countdown ẩn, HP10 score2166. | 99→100 khăn kéo dài, chân neo; glyph và viền khay giữ. | chưa kiểm chứng | `shield-death-ready-R/000100.png` |
| F101 | 5.429000 / 0.067000 | Chờ READY | Đầu cao khăn chéo dài giữ; READY ngà toàn từ, không đối thủ mới. | 100→101 pose giữ, torch đổi sáng nhẹ; không tự chuyển. | chưa kiểm chứng | `shield-death-ready-R/000101.png` |
| F102 | 5.496000 / 0.031000 | Nhập R | R cyan, EADY ngà; đầu player cao khăn vòng trái; padding khay giữ. | 101→102 đổi đúng màu R, chữ không dời/cắt; chân neo. | chưa kiểm chứng | `shield-death-ready-R/000102.png` |
| F103 | 5.527000 / 0.069000 | Giữ R | R cyan EADY ngà giữ; player đầu thấp khăn gập lên trái, HUD2166 HP10. | 102→103 idle hạ đầu, màu/landmark chữ và viền ổn định. | chưa kiểm chứng | `shield-death-ready-R/000103.png` |
| F104 | 5.596000 / 0.087000 | Giữ R | R cyan EADY ngà; player đầu thấp khăn chéo xuống trái, countdown ẩn. | 103→104 khăn hạ, chân neo; chữ giữ vị trí. | chưa kiểm chứng | `shield-death-ready-R/000104.png` |
| F105 | 5.683000 / 0.051000 | Nhập RE | RE cyan ADY ngà; player đầu thấp khăn chéo xuống trái giữ, HP10 score2166. | 104→105 E đổi cyan, khoảng cách và viền khay giữ. | chưa kiểm chứng | `shield-death-ready-R/000105.png` |
| F106 | 5.734000 / 0.044000 | Giữ RE | RE cyan ADY ngà; đầu thấp khăn vòng lớn bên trái, khay không cắt glyph. | 105→106 khăn cuộn vòng; nền torch đổi, chữ ổn định. | chưa kiểm chứng | `shield-death-ready-R/000106.png` |
| F107 | 5.778000 / 0.083000 | Giữ RE | RE cyan ADY ngà; đầu cao khăn U trái, chưa có intro/countdown. | 106→107 đầu lên khăn U, chân neo; không chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000107.png` |
| F108 | 5.861000 / 0.032000 | Nhập REA | REA cyan DY ngà; player đầu cao khăn chéo ngắn trái, countdown ẩn. | 107→108 A đổi cyan; glyph không dời, chân neo. | chưa kiểm chứng | `shield-death-ready-R/000108.png` |
| F109 | 5.893000 / 0.036000 | Giữ REA | REA cyan DY ngà giữ; đầu cao khăn chéo ngắn giữ; HP10 score2166. | 108→109 held pose, viền khay và màu chữ giữ. | chưa kiểm chứng | `shield-death-ready-R/000109.png` |
| F110 | 5.929000 / 0.037000 | Giữ REA | REA cyan DY ngà; đầu cao khăn chéo dài trái, không intro/đối thủ. | 109→110 khăn kéo dài; chân và chữ neo. | chưa kiểm chứng | `shield-death-ready-R/000110.png` |
| F111 | 5.966000 / 0.028000 | Giữ REA | Đầu cao khăn chéo dài giữ; REA cyan DY ngà đủ nét trong giới hạn encode mềm. | 110→111 held pose, nền torch đổi; khay không nhảy/cắt. | chưa kiểm chứng | `shield-death-ready-R/000111.png` |
| F112 | 5.994000 / 0.074000 | Giữ REA | REA cyan DY ngà; player đầu cao khăn vòng trái, countdown ẩn. | 111→112 khăn vòng, chân neo; chữ không dời. | chưa kiểm chứng | `shield-death-ready-R/000112.png` |
| F113 | 6.068000 / 0.051000 | Nhập READ | READ cyan Y ngà; player đầu thấp khăn gập lên trái; HP10 score2166. | 112→113 D đổi cyan, Y giữ ngà; khay/padding neo. | chưa kiểm chứng | `shield-death-ready-R/000113.png` |
| F114 | 6.119000 / 0.014000 | Giữ READ | READ cyan Y ngà; player đầu thấp khăn chéo xuống trái. | 113→114 khăn hạ, chân neo; không intro/countdown. | chưa kiểm chứng | `shield-death-ready-R/000114.png` |
| F115 | 6.133000 / 0.122000 | Giữ READ | READ cyan Y ngà và pose thấp khăn chéo giữ; viền khay đủ cạnh. | 114→115 held pose, nền torch đổi; chữ không cắt/nhảy. | chưa kiểm chứng | `shield-death-ready-R/000115.png` |
| F116 | 6.255000 / 0.019000 | Giữ READ | READ cyan Y ngà; player đầu thấp khăn vòng lớn trái, HP10 score2166. | 115→116 khăn cuộn vòng, chân neo; không chuyển trận. | chưa kiểm chứng | `shield-death-ready-R/000116.png` |
| F117 | 6.274000 / 0.059000 | Giữ READ | READ cyan Y ngà và pose thấp khăn vòng giữ; countdown ẩn. | 116→117 held pose; nền torch đổi nhẹ, glyph neo. | chưa kiểm chứng | `shield-death-ready-R/000117.png` |
| F118 | 6.333000 / 0.093000 | Giữ READ | READ cyan Y ngà; đầu cao khăn U trái, không đối thủ mới. | 117→118 đầu lên khăn U, chân neo; khay giữ. | chưa kiểm chứng | `shield-death-ready-R/000118.png` |
| F119 | 6.426000 / 0.034000 | Giữ READ | READ cyan Y ngà; đầu cao khăn chéo dài trái, HP10 score2166. | 118→119 khăn kéo dài, glyph và padding giữ; chưa wipe. | chưa kiểm chứng | `shield-death-ready-R/000119.png` |
| F120 | 6.460000 / 0.014000 | Giữ READ | READ cyan Y ngà; đầu cao khăn chéo dài giữ; countdown ẩn HP10 score2166. | 119→120 held pose; viền/glyph giữ. | chưa kiểm chứng | `shield-death-ready-R/000120.png` |
| F121 | 6.474000 / 0.095000 | Giữ READ | READ cyan Y ngà và pose cao khăn dài giữ; nền torch đổi nhẹ. | 120→121 held pose, không tự chuyển; chân neo. | chưa kiểm chứng | `shield-death-ready-R/000121.png` |
| F122 | 6.569000 / 0.033000 | Giữ READ | READ cyan Y ngà; đầu cao khăn vòng trái; khay và HUD neo. | 121→122 khăn vòng, chân giữ; glyph không nhảy. | chưa kiểm chứng | `shield-death-ready-R/000122.png` |
| F123 | 6.602000 / 0.054000 | Giữ READ | Đầu cao khăn vòng giữ; READ cyan Y ngà trọn vẹn, chưa wipe. | 122→123 held pose, torch đổi; countdown vẫn ẩn. | chưa kiểm chứng | `shield-death-ready-R/000123.png` |
| F124 | 6.656000 / 0.044000 | Giữ READ | READ cyan Y ngà; player đầu thấp khăn chéo xuống trái, countdown ẩn. | 123→124 đầu hạ khăn hạ, chân neo; chữ giữ. | chưa kiểm chứng | `shield-death-ready-R/000124.png` |
| F125 | 6.700000 / 0.068000 | Giữ READ | READ cyan Y ngà và pose thấp khăn chéo giữ; HP10 score2166. | 124→125 held pose; chưa wipe, glyph đủ. | chưa kiểm chứng | `shield-death-ready-R/000125.png` |
| F126 | 6.768000 / 0.091000 | Hoàn tất READY / wipe | READY toàn cyan đọc được; wipe đen bắt đầu từ dưới, che đáy khay; player đầu thấp khăn vòng. | 125→126 Y đổi cyan tại chỗ rồi wipe lên; không dùng phần bị che để kết luận viền đáy. | chưa kiểm chứng | `shield-death-ready-R/000126.png` |
| F127 | 6.859000 / 0.034000 | Wipe đóng | Wipe che actor/khay/sàn, chỉ dải HUD trên còn score2166 và thanh HP. | 126→127 cạnh wipe tiến lên đến dưới HUD; không quan sát pose bị che. | chưa kiểm chứng | `shield-death-ready-R/000127.png` |
| F128 | 6.893000 / 0.018000 | Wipe gần kín | Chỉ mép đỉnh card điểm/tên Guard và label QA còn ở dải trên; phần còn lại đen. | 127→128 cạnh wipe lên sát đỉnh; actor/glyph không còn quan sát. | chưa kiểm chứng | `shield-death-ready-R/000128.png` |
| F129 | 6.911000 / 0.069000 | Wipe kín | Canvas đen, còn label QA góc trái; actor/HUD/khay bị che hoàn toàn. | 128→129 dải HUD cuối bị wipe che. | chưa kiểm chứng | `shield-death-ready-R/000129.png` |
| F130 | 6.980000 / 0.066000 | Chuyển scene kín | Canvas đen, label QA góc trái biến mất; không thấy actor/khay. | 129→130 label QA mất, giữ màn đen; không suy ra pose. | chưa kiểm chứng | `shield-death-ready-R/000130.png` |
| F131 | 7.046000 / 0.131000 | Chuyển scene kín giữ | Canvas vẫn đen, không label QA, không glyph/HUD đọc được. | 130→131 held màn đen trong khoảng PTS; không phải cel idle. | chưa kiểm chứng | `shield-death-ready-R/000131.png` |
| F132 | 7.177000 / 0.015000 | Wipe kín / QA trở lại | Canvas đen; label QA góc trái trở lại, actor/HUD/khay chưa hiện. | 131→132 label hiện lại, không suy ra actor trong vùng đen. | chưa kiểm chứng | `shield-death-ready-R/000132.png` |
| F133 | 7.192000 / 0.033000 | Wipe mở bắt đầu | Dải sàn mảnh xuất hiện đáy native; QA giữ, actor/khay vẫn che. | 132→133 cạnh wipe mở từ dưới lên, chưa thấy chân. | chưa kiểm chứng | `shield-death-ready-R/000133.png` |
| F134 | 7.225000 / 0.045000 | Wipe mở sàn | Dải sàn rộng hơn với landmark gạch giữa; actor vẫn chưa thấy. | 133→134 cạnh wipe lên, sàn lộ thêm; không HUD/khay. | chưa kiểm chứng | `shield-death-ready-R/000134.png` |
| F135 | 7.270000 / 0.079000 | Wipe mở actor | Chân/thân dưới player lộ, đầu bị cạnh wipe che; sàn sáng giữa, khay/HUD chưa hiện. | 134→135 wipe lên qua thân; chân xuất hiện đúng landmark sàn, đầu chưa kiểm được. | chưa kiểm chứng | `shield-death-ready-R/000135.png` |
| F136 | 7.349000 / 0.048000 | Wipe mở hết | Player một mình, đầu thấp/khăn vòng; HUD/khay chưa hiện. | 135→136 cạnh wipe rời đầu, cảnh mở hết. | chưa kiểm chứng | `shield-death-ready-R/000136.png` |
| F137 | 7.397000 / 0.031000 | Spawn bắt đầu | Player giữ tư thế, vệt bụi phẳng mờ trên sàn phải. | 136→137 bụi bắt đầu, chưa thấy enemy. | chưa kiểm chứng | `shield-death-ready-R/000137.png` |
| F138 | 7.428000 / 0.079000 | Bụi spawn | Bụi kem phẳng sáng hơn bên phải, player giữ. | 137→138 bụi sáng/rộng thêm, chưa thấy enemy. | chưa kiểm chứng | `shield-death-ready-R/000138.png` |
| F139 | 7.507000 / 0.061000 | Bụi nở | Player đầu cao/khăn chéo ngắn; bụi nở cao che vùng spawn. | 138→139 bụi che silhouette; không suy ra cel bên trong. | chưa kiểm chứng | `shield-death-ready-R/000139.png` |
| F140 | 7.568000 / 0.062000 | Enemy xuất hiện | Đầu/rib/kiếm chéo trái enemy lộ trên bụi; chân/tay dưới bị bụi che, player đứng neo sàn. | 139→140 thân trên enemy hiện, bụi còn lớn; HUD/khay ẩn. | chưa kiểm chứng | `shield-death-ready-R/000140.png` |
| F141 | 7.630000 / 0.064000 | Intro | ELITE / SWORDSMAN, DUEL 5/6 hiện; bụi thấp hơn lộ hông, kiếm giữ trong tay đỏ. | 140→141 tiêu đề hiện, bụi hạ dần; chân còn bị che. | chưa kiểm chứng | `shield-death-ready-R/000141.png` |
| F142 | 7.694000 / 0.075000 | Intro / bụi tan | Enemy lộ chân dang, mũi kiếm chéo trái, bụi nhỏ sát sàn; player khăn ngang. | 141→142 bụi giảm, landmark chân hiện; intro sáng hơn. | chưa kiểm chứng | `shield-death-ready-R/000142.png` |
| F143 | 7.769000 / 0.102000 | HUD trở lại | HUD 2166/player HP thấp/enemy HP đầy hiện; intro giữ, bụi tan, khay chưa có chữ. | 142→143 HUD fade vào, chân enemy neo trên sàn; không READY còn sót. | chưa kiểm chứng | `shield-death-ready-R/000143.png` |
| F144 | 7.871000 / 0.018000 | Intro / khay trở lại | Khay or / new / learn màu ngà, countdown cyan đầy; HUD 2166/HP thấp giữ. Player khăn vòng lớn, enemy kiếm chéo trái. | 143→144 khay/countdown hiện đúng vùng, bụi hết; chân hai actor giữ landmark. | chưa kiểm chứng | `shield-death-ready-R/000144.png` |
| F145 | 7.889000 / 0.041000 | Intro giữ | Actor và khay giữ bố cục; or/new/learn không đổi màu; countdown đầy, 2166 giữ. | 144→145 pose giữ, không chuyển trận lần hai. | chưa kiểm chứng | `shield-death-ready-R/000145.png` |
| F146 | 7.930000 / 0.025000 | Intro mờ dần | Player đầu thấp/khăn vòng; enemy giữ kiếm, chân neo; tiêu đề mờ hơn, glyph khay giữ. | 145→146 đầu player hạ nhẹ, khăn đổi vòng; intro fade, HUD cố định. | chưa kiểm chứng | `shield-death-ready-R/000146.png` |
| F147 | 7.955000 / 0.035000 | Intro mờ dần / giữ | Player đầu thấp/khăn vòng, enemy kiếm chéo trái; khay/HP/2166 giữ. | 146→147 pose giữ, tiêu đề tối thêm, countdown còn đầy. | chưa kiểm chứng | `shield-death-ready-R/000147.png` |
| F148 | 7.990000 / 0.041000 | Intro kết thúc | Tiêu đề giữa biến mất; player đầu cao/khăn vòng, enemy kiếm chéo trái; or/new/learn ngà và countdown đầy. | 147→148 intro hết, head player nhấc; actor không đổi vị trí sàn. | chưa kiểm chứng | `shield-death-ready-R/000148.png` |
| F149 | 8.031000 / 0.034000 | Combat idle | ELITE / SWORDSMAN hiện góc phải; player khăn chéo dài, enemy giữ; HUD 2166/HP thấp không đổi. | 148→149 nhãn role hiện đúng HUD, khay giữ vị trí. | chưa kiểm chứng | `shield-death-ready-R/000149.png` |
| F150 | 8.065000 / 0.027000 | Combat idle | Enemy đầu/rib hạ nhẹ, kiếm nghiêng trái cùng tay đỏ; player khăn chéo giữ, chân hai bên neo. | 149→150 idle enemy hạ thân không trượt chân; countdown bắt đầu rút nhẹ. | chưa kiểm chứng | `shield-death-ready-R/000150.png` |
| F151 | 8.092000 / 0.070000 | Combat idle | Player khăn ngắn chéo lên trái, đầu cao; enemy giữ đầu thấp/kiếm; chữ khay ngà, 2166 giữ. | 150→151 khăn thu ngắn, silhouette thân giữ; không flash/afterimage còn sót. | chưa kiểm chứng | `shield-death-ready-R/000151.png` |
| F152 | 8.162000 / 0.041000 | Combat idle giữ | Player khăn chéo ngắn, enemy đầu thấp/kiếm trái giữ; or/new/learn ngà, HP/2166 giữ. | 151→152 pose giữ, ánh đuốc đổi nhẹ; không trượt chân. | chưa kiểm chứng | `shield-death-ready-R/000152.png` |
| F153 | 8.203000 / 0.068000 | Combat idle | Player khăn chữ U hướng trái; enemy đầu hạ thêm, kiếm theo tay; khay cố định. | 152→153 khăn cuộn U, enemy idle hạ thân; chân neo landmark. | chưa kiểm chứng | `shield-death-ready-R/000153.png` |
| F154 | 8.271000 / 0.054000 | Combat idle / countdown | Player đầu cao/khăn dài chéo xuống trái; enemy đầu thấp giữ, countdown rút rõ ở mép phải. | 153→154 khăn duỗi; countdown ngắn hơn, chữ không dịch. | chưa kiểm chứng | `shield-death-ready-R/000154.png` |
| F155 | 8.325000 / 0.050000 | Combat idle giữ | Player khăn dài chéo và enemy kiếm chéo giữ; countdown rút thêm, HP/2166 không đổi. | 154→155 pose giữ, mép cyan lùi trái; camera và chân ổn định. | chưa kiểm chứng | `shield-death-ready-R/000155.png` |
| F156 | 8.375000 / 0.038000 | Combat idle giữ | Player khăn vòng lớn, enemy đầu thấp/kiếm trái; or/new/learn ngà, countdown rút và 2166 giữ. | 155→156 khăn cuộn vòng, chân/camera giữ. | chưa kiểm chứng | `shield-death-ready-R/000156.png` |
| F157 | 8.413000 / 0.074000 | Combat idle | Enemy đầu/rib nhấc, kiếm theo tay đỏ; player khăn vòng giữ; HUD không đổi. | 156→157 idle enemy lên, chân không trượt; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000157.png` |
| F158 | 8.487000 / 0.050000 | Combat idle | Player đầu cao/khăn vòng; enemy đầu cao giữ kiếm chéo; toàn chữ khay ngà, HP/2166 giữ. | 157→158 đầu player nhấc, khăn giữ vòng; mép cyan lùi trái. | chưa kiểm chứng | `shield-death-ready-R/000158.png` |
| F159 | 8.537000 / 0.066000 | Combat idle | Player khăn chéo dài trái, enemy đầu cao/kiếm giữ; countdown ngắn thêm, HUD neo. | 158→159 khăn duỗi, silhouette enemy giữ; không có flash hay bụi còn sót. | chưa kiểm chứng | `shield-death-ready-R/000159.png` |
| F160 | 8.603000 / 0.050000 | Combat idle giữ | Player khăn chéo dài và enemy đầu cao/kiếm trái giữ; countdown rút, chữ khay/2166/HP giữ. | 159→160 pose giữ; mép cyan tiếp tục lùi, camera ổn định. | chưa kiểm chứng | `shield-death-ready-R/000160.png` |
| F161 | 8.653000 / 0.067000 | Combat idle | Player khăn chữ U thấp bên trái; enemy giữ tay đỏ/kiếm, chân hai bên neo. | 160→161 khăn cuộn xuống, đầu player giữ cao; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000161.png` |
| F162 | 8.720000 / 0.043000 | Combat idle / giữ | Player khăn U và enemy đầu cao giữ; or/new/learn ngà, countdown ngắn hơn. | 161→162 pose giữ; HUD và mép khay không nhảy. | chưa kiểm chứng | `shield-death-ready-R/000162.png` |
| F163 | 8.763000 / 0.073000 | Combat idle | Player khăn dài chéo trái, enemy giữ silhouette; HP thấp/2166 không đổi, countdown rút. | 162→163 khăn duỗi lên, chân/player/kiếm không đổi điểm neo. | chưa kiểm chứng | `shield-death-ready-R/000163.png` |
| F164 | 8.836000 / 0.074000 | Combat idle / cue | SPACE trắng nền đen xuất hiện trên enemy; player khăn vòng, enemy kiếm chéo giữ; countdown rút. | 163→164 cue hiện; actor/khay/HUD giữ vị trí. Phần trên cue ngoài pan actor, native thấy đủ. | chưa kiểm chứng | `shield-death-ready-R/000164.png` |
| F165 | 8.910000 / 0.027000 | Combat idle giữ | SPACE giữ trắng; player khăn vòng/enemy đầu cao giữ, or/new/learn ngà, 2166 giữ. | 164→165 pose giữ, countdown ngắn hơn; không flash. | chưa kiểm chứng | `shield-death-ready-R/000165.png` |
| F166 | 8.937000 / 0.037000 | Combat idle giữ | Cue trắng, enemy kiếm chéo, player khăn vòng giữ; chân/sàn neo và HP giữ. | 165→166 giữ pose; mép countdown lùi nhẹ, khay không đổi. | chưa kiểm chứng | `shield-death-ready-R/000166.png` |
| F167 | 8.974000 / 0.059000 | Combat idle | Player khăn vòng lớn hơn, đầu thấp; enemy đầu cao/kiếm giữ, SPACE trắng. | 166→167 khăn đổi vòng, head player hạ nhẹ; không trượt chân, countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000167.png` |
| F168 | 9.033000 / 0.066000 | Combat idle | Player đầu cao/khăn vòng, enemy kiếm trái; SPACE trắng giữ, countdown rút; HUD 2166 giữ. | 167→168 player nhấc đầu; không trượt chân, mép khay ổn định. | chưa kiểm chứng | `shield-death-ready-R/000168.png` |
| F169 | 9.099000 / 0.062000 | Combat idle giữ | Player khăn vòng và enemy đầu cao/kiếm giữ; or/new/learn ngà và HP giữ. | 168→169 pose giữ, countdown rút; SPACE không đổi màu. | chưa kiểm chứng | `shield-death-ready-R/000169.png` |
| F170 | 9.161000 / 0.050000 | Combat idle | Player khăn U thấp trái, enemy giữ; countdown ngắn hơn, 2166 giữ. | 169→170 khăn cuộn xuống, actor neo sàn; chữ khay không nhảy. | chưa kiểm chứng | `shield-death-ready-R/000170.png` |
| F171 | 9.211000 / 0.071000 | Combat idle giữ | Player khăn U và enemy kiếm chéo giữ, cue trắng; countdown tiếp tục rút. | 170→171 pose giữ; HUD/khay/camera ổn định, không flash. | chưa kiểm chứng | `shield-death-ready-R/000171.png` |
| F172 | 9.282000 / 0.092000 | Combat idle giữ | Player khăn dài chéo và enemy kiếm trái giữ; SPACE trắng, countdown rút, 2166/HP giữ. | 171→172 khăn duỗi; chân hai actor neo. | chưa kiểm chứng | `shield-death-ready-R/000172.png` |
| F173 | 9.374000 / 0.054000 | Chuẩn bị đòn mới | Enemy xoay đầu/thân, tay đỏ rút gần sườn, lưỡi kiếm trái không còn đọc rõ; player khăn vòng. | 172→173 silhouette enemy đổi sang chuẩn bị, chân vẫn neo; chưa có đòn/recovery. | chưa kiểm chứng | `shield-death-ready-R/000173.png` |
| F174 | 9.428000 / 0.072000 | Chuẩn bị / giữ | Enemy đầu xoay/tay rút giữ, player khăn vòng, cue trắng; chữ khay ngà/2166 giữ. | 173→174 pose chuẩn bị giữ; countdown rút, không trượt sàn. | chưa kiểm chứng | `shield-death-ready-R/000174.png` |
| F175 | 9.500000 / 0.037000 | Chuẩn bị / nâng kiếm | Enemy đưa kiếm chéo lên phải ngang vai, tay đỏ trước rib, thân xoay; player giữ khăn vòng. | 174→175 kiếm đi qua vai sang phải, grip đọc được; chân neo, chưa có tiếp xúc. | chưa kiểm chứng | `shield-death-ready-R/000175.png` |
| F176 | 9.537000 / 0.051000 | Chuẩn bị / giữ kiếm | Enemy giữ kiếm chéo lên phải qua vai; player khăn vòng, chân hai actor neo; điểm 2166 và HP giữ. | 175→176 tư thế chuẩn bị giữ, countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000176.png` |
| F177 | 9.588000 / 0.043000 | Chuẩn bị / khuỵu | Enemy hạ đầu, gối gập và thân nghiêng trước, kiếm vẫn chéo lên phải; SPACE đổi nền ngà chữ tối. | 176→177 hạ trọng tâm; grip còn sát rib, player khăn dài chéo trái. | chưa kiểm chứng | `shield-death-ready-R/000177.png` |
| F178 | 9.631000 / 0.040000 | Chuẩn bị / giữ thấp | Enemy giữ gối gập và kiếm vai phải, player đầu thấp khăn chéo trái; khay or/new/learn ngà. | 177→178 silhouette chính giữ, chân neo; countdown rút. | chưa kiểm chứng | `shield-death-ready-R/000178.png` |
| F179 | 9.671000 / 0.054000 | Chuẩn bị / giữ thấp | Enemy giữ pose thấp kiếm vai phải; player khăn cong ngắn lên trái, SPACE nền ngà. | 178→179 khăn player đổi, enemy giữ; HP và 2166 không đổi. | chưa kiểm chứng | `shield-death-ready-R/000179.png` |
| F180 | 9.725000 / chưa xác định (cuối media) | Biên cuối / chưa có recovery | Enemy vẫn chuẩn bị kiếm vai phải, player khăn cong; khay và cue đọc được, countdown tiếp tục rút. Duration cuối chưa xác định. | 179→180 giữ pose chính; clip kết thúc trước thực thi/recovery đòn mới, không phải loop. | chưa kiểm chứng | `shield-death-ready-R/000180.png` |
<!-- ledger-shield-death-ready-R-end -->

### Main đổi trong lúc thu bằng chứng — đã chốt audit luồng READY

Fetch lại origin ngày 2026-10-27 xác nhận main mới
`66749da15890be3b8ae039a81228249acda0a4ed`, commit
`Type READY in the word tray between duels`.
Diff từ revision capture `e5c48017` thay `TypingManager.cs`, `EndlessDuel.cs`
và hai file test liên quan. `EndlessDuel` bỏ tạo overlay/Update xử lý Enter,
chuyển sang `TypingManager.BeginReady(Continue)`. TypingManager hiển thị `READY`,
ẩn countdown và nhận từng chữ để tiếp tục; không còn nút `Ready · Enter`.
Không có delta artwork/shader/camera trong diff này, nhưng flow giữa trận
và typing đã thay đổi: ảnh click/Enter ở trên **không áp dụng cho main mới**.

Người dùng đã chọn audit main mới với gõ READY. Nút/click/Enter và card cũ
chỉ là bằng chứng lịch sử của `e5c48017`; không dùng nghiệm thu R. Không khôi
phục nút hoặc đổi sản phẩm để dùng lại recording. Các đoạn death/cleanup nối
sang màn chờ phải được thu lại trên R; phần combat trước điểm nối có thể giữ
khi đối chiếu nguồn chứng minh không chịu ảnh hưởng.

Snapshot LF sạch của R: **2.495 file**, SHA256
`d337b04e43ff1099440c85403c9e78bebbadddab6c10bdc3252f404029d85922`, tại
`E:/CodexTemp/TypingHero/pixel-audit-evidence-20261027/ready-R66749da1/source`.
Manifest so với snapshot capture cũ chỉ khác bốn file: `TypingManager.cs`,
`EndlessDuel.cs`, `GameplayTests.cs`, `EndlessTests.cs`. Mọi asset, metadata,
anim/controller, scene, material/shader, camera, package và cấu hình build có
hash chuẩn hóa LF không đổi; các đầu vào snapshot khớp Git R sau chuẩn hóa LF.
Unity đúng `2022.3.25f1 (530ae0ba3889)`, runner Windows `local_windows.py`.
Lần build QA đầu `20261027T064109.028585Z-c0a247c81f374438a112f696dbf95896`
thất bại ở Bee backend với access violation `-1073741819`; chưa có player,
không phải kết quả kiểm gameplay. Lần thứ hai cùng snapshot/cache,
`20261027T071346.161280Z-f570b723c762494680dca83131b582a5`, build thành công,
exit 0, `game-loop-build.json` xác nhận `qa=true`. Log cả hai lần được giữ
trong `ready-R66749da1/results/`. Chưa đồng nhất build thành công với audit đạt.

Pilot `ready-R66749da1/capture-1` đã chạy trên NVIDIA GTX 1650 Ti / ANGLE
Direct3D11, fixture Firebase và chặn provider thật. Driver dừng ở check so
sánh thời gian sau khi combat mới đã chạy: trước/chờ/sau prefix/intro đều
`42.18952178955078s`, mẫu combat kế là `42.27203369140625s`.
Nguồn `CombatSession.Update` tăng thời gian khi combat tiếp tục; đây là sai
mốc so sánh của driver. Giữ video/trace/stills và 13 check trước lỗi làm lịch
sử pilot; không dùng pilot này chốt bộ nghiệm thu. Driver ngoài repo đã đổi
sang đối chiếu thời gian tại intro còn khóa input, giữ so sánh HP/score/
letters/correct/parries khi vào combat. `capture-2` đã thu clip kiếm đủ tới combat
kế và qua 20 check trước khi pilot chết ở Double Strike sau điểm kết thúc clip.
Việc xuất media khi game vẫn chạy làm pilot bỏ phòng thủ; driver đã đổi sang
bấm Escape sau khi recorder xác nhận dừng, Resume sau khi xuất, ngoài đoạn thu.
`capture-3` dừng ở mốc đọc intro trong lúc SampleScene đã unload: bridge lúc đó
đọc DungeonWorld, HP=-1, thời gian=0; không phải chứng cứ mất thống kê của game.
Clip rìu pilot này thiếu điểm cuối nên chưa dùng nghiệm thu. Driver đã chờ
SampleScene khôi phục thống kê và còn khóa intro trước khi so sánh. `capture-4`
chưa qua tutorial do input quá sớm; không có clip nghiệm thu. `capture-5` đã hoàn tất rìu/khiên bằng input thường, 29 check kỹ thuật qua, không exception; giữ clip kiếm `capture-2` qua 20 check READY trước lỗi pilot sau clip. Các con số check không thay kiểm hình.

Đã mở trực tiếp ba still `capture-2/ready-1280.png`, `ready-1366.png`,
`ready-return.png` ở kích thước gốc: READY nằm trái trong khay, không có
card/nút cũ, countdown ẩn; chữ không cắt, viền/padding còn ở cả ba bước
resize. Đây là kiểm ba still, chưa thay review mọi frame trong clip liên tục
hoặc xác nhận playback của người xem.

Hai player R đều build Windows Unity thành công, mỗi bản 33 file. Digest manifest
QA: `eff250c693241a650b7b8a39ff091d8f26627ecf3f1a4daf91b5b137af85c817`;
không QA: `cbffa2d323e7ec97981e9f6133f187c0da1e1a8808d7e17b9a9cc84977fa57d7`.
Receipt `game-loop-build.json` bản sau ghi `qa=false`; runtime `typingHeroQA`
và `typingHeroCombatFrame` đều undefined. Build không QA dùng `EMCC_CORES=2`
ở môi trường runner, không đổi cấu hình sản phẩm. Không chạy thêm suite/profile.

Đã kiểm tên bằng phím CDP trên bản không QA (`glyph-2/receipt.json`):
`Đặng_Á9` hiện đủ trong input và HUD tutorial reading ở full 1280×720 và crop
nearest 4×. Đọc được nét ngang Đ, dấu ă và dấu nặng tách dưới, gạch dưới,
dấu sắc Á và số 9; không ô vuông/mất dấu/cắt biên nhìn thấy. Input screenshot
SHA256 `8ac98d8feb4d6c70ed6001954ae645dc25bf96f6850b04712e1e91c6129b9206`;
HUD reading `ea39e60671f2edabaef836fc8b3219f2cfbe21229618d80cb60bfd7895f9186d`.
Tên HUD không còn label QA che; padding trái/trên còn đủ ở mẫu này. Still mang tên
`hud-combat.png` thực tế là khoảng chuyển intro sau Enter, HUD chưa hiện; không
dùng still đó kết luận glyph. Phiên glyph không ghi exception/provider thật;
fixture vẫn bật nhưng không phát sinh request Firebase trong phạm vi nhập tên.
Phiên browser/host glyph đã đóng; giữ media phục vụ audit đang mở.

| Bằng chứng | Quyết định áp dụng cho R | Phần còn phải hoàn tất |
|---|---|---|
| 64 key / 61 cặp nguồn skeleton và rìu | Giữ: asset, timing, pivot, importer byte-equivalent sau LF; không có delta art trong Git. | Playback người xem từng clip 1×/0.25×. |
| GIF extraction F13–F35, 23 frame / 22 cặp | Giữ riêng reference, SHA256 `e41867a829ef06b3296e8778aa43739bc8c0605b9df77e4212dbd373c4ba522c`. | Playback reference, không cộng vào runtime. |
| Tutorial reading, 34 frame / 33 cặp | Giữ: controller/tutorial/rendering không đổi; gate của ProcessInput tương đương khi `readyPrompt=false`, `Finished=false`. | Playback; pose timeScale=0 không phải vòng idle. |
| Combat thường, parry, Slayer, Hurt và death player trong capture-5 | Giữ các đoạn trước màn chờ: phần xử lý combat của ProcessInput và HandleTimer không đổi khi `readyPrompt=false`; không đổi caller animation hay rendering. | Toàn bộ frame/cặp chưa xem, recovery/biên, VFX che và playback vẫn mở. |
| sword/axe/shield death → cleanup → màn chờ cũ | Không dùng toàn đoạn cho READY mới. | Đã thu và ghi ledger đủ 806 frame/803 cặp của ba clip R; phần che và playback còn mở, không dùng clip/card cũ. |
| Ready · Enter, click/Enter cũ | Chỉ lịch sử tại e5c48017. | Kiểm gõ READY; Enter/click vùng cũ không chuyển trận. |
| Tên Đặng_Á9/HUD bị QA label che | Đã bổ sung bản R không TYPINGHERO_AUTOMATION, phím thật, xem input và HUD trọn glyph ở native/nearest 4×. | Mẫu glyph này đủ quan sát; giới hạn QA label của ảnh cũ vẫn là lịch sử. |

Kiểm hành vi nội bộ dùng lại test đã có trên R, dẫn tại
[verification](../game/verification.md#typed-ready-replaces-the-inter-duel-card-2026-10-27-verified):
28/28 ban đầu, 3/3 ca READY cuối, và 17/17 check input native của task READY.
Đó là kết quả **đã có**, không phải suite vừa chạy trong audit này; không thay
kiểm hình hai viewport hoặc review frame của bộ capture audit.

Mốc nghiệm thu READY: chữ/viền/padding qua ba bước resize; countdown ẩn, chờ
không tự chạy; từng chữ giữ vị trí, sai sau prefix trở về READY và hết feedback;
chấp nhận chữ thường; Enter/click vùng cũ không tiếp tục; Pause chặn chữ và
Resume giữ prefix; hoàn tất chuyển đúng một lần, intro/HUD/tray/countdown trở
lại; HP/score/time/letter stats không tính READY như combat. Điều khiển bằng
phím/chuột; bridge chỉ đọc. Không ép HP/animator/callback hoặc thêm API/QA lâu dài.

Coverage frame và playback vẫn chưa đủ; audit tiếp tục mở. Media cũ được giữ
để đối chiếu và hoàn tất phần còn áp dụng, không coi main đổi là bằng chứng đạt.

Trang review đã có check kỹ thuật lịch sử trên danh sách 19 mục e5: bước F1 PTS0.062,
video playbackRate0.25 chạy, mục cuối bắt đầu F645 PTS26.008, nearest width5120px,
không có page error. Receipt tạm `review-smoke.json`. Đã sửa sự kiện timeupdate
ghi đè frame vừa chọn và bỏ preload mọi PNG runtime trong trang tạm.
Đây là check công cụ review, **không phải xác nhận playback người xem**.
Danh sách hiện tại có 19 mục sau bổ sung ba clip READY; các đoạn bị thay thế giữ ở manifest lịch sử. Chưa chốt bộ nghiệm thu cuối hoặc xác nhận playback.

### Clip READY trên R — manifest đang review

Mọi clip giữ trọn media từ trước đòn kết liễu qua cleanup/READY đến combat mới;
không loop/retime. Duration frame cuối **chưa xác định**, tách khỏi wall-clock stop.
Probe/trace ghi trước review: `ready-R66749da1/runtime-clips-manifest.json`.

| Clip / nguồn | SHA256 | PTS đầu–cuối encode / wall stop (s) | Frame/cặp | Đã kiểm trực tiếp frame/cặp |
|---|---|---|---:|---:|
| `sword-death-ready-R` / `capture-2` | `80c10132fd8e44b64ba0da8c8f16c722db1644e17d2de636676e9d7f120297bc` | 0.000–34.779; wall 35.851750 | 463/462 | 463/462 |
| `axe-death-ready-R` / `capture-5` | `86afe9c0aa5b9c1d65e88cdf3f38dd3d4618f07f7ea705a0deab36e29419cdf7` | 0.000–9.609; wall 9.704785 | 162/161 | 162/161 |
| `shield-death-ready-R` / `capture-5` | `1dfb40d57b2a37b13e154bc4f241c0e88df2b067ff2d9e0a64256f24e6dbb7e0` | 0.000–9.725; wall 9.880415 | 181/180 | 181/180 |

Kiếm có 11 frame1366×768 (F160–F170, PTS11.434–13.052 trước frame trở về),
các frame còn lại1280×720; rìu/khiên1280×720. Lần trích PNG toàn media ban đầu
giữ kích thước encoder1280 cho nhóm1366; đã trích riêng đúng khối timestamp và
kiểm kích thước từng PNG so với probe trước review. Không thu nhỏ hoặc đổi canvas
để che clipping. Trang review tạm dùng kích thước video intrinsic và nearest bằng
4× naturalWidth, không ép1366 về1280. Đây là sửa công cụ ngoài repo.

Capture2 kiếm: HP35/score343/letters59/correct59/parries5/words18/chars59,
combat39.9630241394043s giữ qua READY và intro. Capture5 rìu:
HP25/score1642/letters301/correct301/parries26/words74/chars301,
combat171.4067840576172s; khiên:
HP10/score2166/letters381/correct381/parries31/words95/chars381,
combat207.0516357421875s. Sau khi khóa intro kết thúc, đồng hồ combat tăng lại;
đòn enemy mới có thể trừ HP sau đó, không gán việc này cho nhập READY.
Các check kỹ thuật xác nhận READY không tính như combat và chỉ một lần handoff;
đọc chữ/pose/camera vẫn cần kiểm hình, không lấy trace thay quan sát.

Môi trường thực: Windows Unity2022.3.25f1, Edge native (Program Files x86),
ANGLE NVIDIA GTX1650Ti Direct3D11. Nhãn Linux/Docker trong browser.json là field
thừa kế của runner, **không phải môi trường đã dùng**. Fixture và chặn provider
thật bật; capture5 không exception và không request Firebase/provider trong phạm vi
đã thu (reads/writes/externalBlocked đều0). Host/browser capture và glyph đã đóng.

### Checkpoint khi ổ bằng chứng E: mất kết nối — 2026-10-27

Ba clip READY đã có ledger trực tiếp đủ 806 frame/803 cặp chuyển tiếp encode,
bao gồm pan phải nearest 4× bổ sung cho khiên F68–F72. Đây là tiến độ kiểm hình,
chưa phải chứng nhận toàn audit: phần combat tái dùng, preview chi tiết bị che và
xác nhận playback người xem vẫn chưa hoàn tất. A1 tiếp tục là lỗi xác nhận.

Khi tiếp tục `sword-double-attacks`, đã mở trực tiếp native full canvas và pan actor
nearest 4× F682–F688. Các quan sát chưa được đồng bộ vào catalog/ledger định lượng:

| Frame / PTS (s) | Quan sát trực tiếp | Chuyển tiếp |
|---|---|---|
| F682 / 38.010 | Canvas đen, chưa thấy actor/HUD. | Frame đầu phạm vi; chưa có cặp trước trong đoạn. |
| F683 / 38.110 | Canvas còn đen, label QA xuất hiện góc trái trên. | F682→F683 chỉ label QA hiện; actor vẫn bị che. |
| F684 / 38.137 | Mép mở màn lộ dải sàn ở đáy; actor chưa đọc được. | F683→F684 bắt đầu mở từ đáy. |
| F685 / 38.188 | Dải mở rộng, lộ chân và phần dưới player; đầu còn bị che. | F684→F685 mép che đi lên, landmark sàn hiện thêm. |
| F686 / 38.199 | Lộ toàn player trong pan, khăn kéo trái; phần trên scene vẫn bị che. | F685→F686 mép mở qua đầu player; HUD/khay chưa hiện. |
| F687 / 38.256 | Scene gần mở hết, player đứng trên sàn, chưa có enemy/HUD/khay đọc được. | F686→F687 mở thêm tường/đuốc, chân player giữ landmark. |
| F688 / 38.285 | Scene mở, player giữ dáng và khăn trái; enemy/HUD/khay chưa hiện. | F687→F688 actor giữ, phần che phía trên hết. |

Đã đối chiếu decoder độc lập với `-reinit_filter 0`: bốn frame toàn cục F682–F685
có PTS 38.010/38.110/38.137/38.188, vẫn là mở màn. Trace tại 38.0293 còn
DungeonWorld; 38.1293 chuyển SampleScene trước khi hiệu ứng mở màn lộ actor.
Vì vậy không lấy trạng thái trace làm bằng chứng rằng sprite đã nhìn thấy trong
cùng thời điểm; giữ các frame đen/che trong đoạn và tiếp tục review theo hình.

Lần mở F689 thất bại với `A device which does not exist was specified`
(`os error 433`). Kiểm tra volume ngay sau đó chỉ còn C: và D:, không còn E:.
Worktree D: vẫn truy cập được; `git diff --check` cho báo cáo không có lỗi.
Không suy diễn mất file thành file hỏng hoặc mất vĩnh viễn; chưa thể kiểm catalog,
hash hay dọn tài nguyên trên E:. Chưa commit/push/integrate checkpoint này.
Giữ worktree `D:/Projects/TypingHero-worktrees/pixel-audit-evidence` và nhánh
`micharel09/pixel-audit-evidence`; chỉ tiếp tục đọc bằng chứng và dọn tài nguyên tạm
sau khi volume E: được khôi phục và đã xác minh các đường dẫn/hash.

Kiểm tra lại sau sự cố: volume E: đã xuất hiện lại với HealthStatus Healthy,
OperationalStatus OK. Catalog `review/clips.json` đọc/parse được; ảnh
`sword-double-attacks/000689-review.png` truy cập được. SHA256 của cả ba media
READY khớp catalog, số frame đã ghi kiểm trực tiếp lần lượt 463/463, 162/162 và
181/181. Kết quả này gỡ trở ngại truy cập bằng chứng; không thay thế phần review
còn thiếu hoặc xác nhận playback. Chưa xác minh toàn bộ file trên volume.

Bàn giao tài nguyên lượt tiếp tục: đã lưu 825 file công cụ/pan đã tạo trong lượt này
ra `D:/Projects/TypingHero-evidence/pixel-audit-continue-20261027/review-work`,
kiểm SHA256 từng file nguồn/bản sao khớp; có `saved-files-manifest.json`,
`verification.txt`, bản sao báo cáo và `cleanup-receipt.json` cùng thư mục cha.
Đã chạy Complete cho đăng ký `pixel-audit-continue-20261027`: kết quả deleted /
verified-absent, không báo lỗi consumer hay xóa. Bộ bằng chứng audit cũ vẫn giữ
ở E:/CodexTemp/TypingHero/pixel-audit-evidence-20261027 để tiếp tục phần chưa kiểm;
không đưa bộ có từ trước này vào phạm vi dọn đăng ký của lượt tiếp tục.


## Checkpoint A1 và đối soát R0 — 2026-10-27

R0 đã fetch: `3f1b1ed720ce22a5815425f227903049410c4cd2`, remote
`https://github.com/micharel09/TypingHero.git`. Worktree A1 riêng:
`D:/Projects/TypingHero-worktrees/a1-shield-ownership`, nhánh
`micharel09/a1-shield-ownership`. Báo cáo tiếp tục ở worktree audit có sẵn;
checkout gốc và mọi thay đổi ngoài task được giữ nguyên.

### A1: nguồn đã sửa, runtime chưa kiểm

Đã trực tiếp mở GIF F19–F25, lưới nguồn nearest 4×. Khiên có cả núm kim loại,
không chỉ pixel nâu. Quy tắc riêng từng frame trong `owners` chuyển đúng
208 pixel từ owner 1 sang owner 2, không đổi lane split chung hoặc lọc màu.
Walk rìu F6–F11 bỏ 182 pixel; Die rìu F0 bỏ 26; Die kiếm–khiên F6–F12 nhận
đủ 208 pixel cùng RGB/tọa độ nguồn. Những số nâu trong lịch sử chỉ đếm một
phần palette, không phải tổng pixel chuyển ownership.

Ba atlas đã đồng bộ trong nhánh A1; recipe depth cập nhật hash và thêm mask
scanline chỉ tại phần khiên trả lại, depth 245 như lớp khiên hiện có. Normal
được tạo lại bằng công cụ hiện có; material sửa tại texel đổi coverage theo
quy tắc palette hiện hành. Không đổi GUID/rect/pivot/animation/controller.

Đã xem 36/36 cel ở native và nearest 4×, source và mirror X, cùng map chẩn đoán;
72 dòng frame/biến thể và 68/68 cặp nội bộ/seam Walk ghi trong ledger bên dưới.
Không có mảnh khiên rời ở rìu; khiên rơi/nảy/nằm trong đúng cel sword-shield.
Đây là kiểm nguồn, không phải render dưới ánh sáng runtime hoặc playback.
Walk tiếp tục asset-only, không ép Animator để tạo coverage.

Bằng chứng bền vững ngoài CodexTemp:
[D:/Projects/TypingHero-evidence/a1-shield-ownership](D:/Projects/TypingHero-evidence/a1-shield-ownership).
`cel-review.json` chứa từng frame/biến thể, duration 100 ms, frame GIF,
quan sát/chuyển tiếp, filename và hash preview. Các file `*-final.png` thay
preview map thử khi có; bản thử không được tính là map cuối. `technical-receipt.json`
đối soát pixel/nguồn/native và tính cục bộ map, tách khỏi kiểm hình.

Có lỗi trong script đồng bộ tạm: tên Die_N trùng giữa hai actor làm scanline
khiên ban đầu vào axe Die F6–F10. Đã sửa vùng áp dụng, tạo lại map và mở lại
bản cuối; không đưa map thử vào nghiệm thu. Build Windows snapshot đầu
`65fa48e2be3b479c71fc8c7ec281d26d41f42a7bfbe992d89b21f7d2f05a3618`
chứa map thử, không dùng chứng nhận runtime bản cuối dù build thành công.

**Chưa nghiệm thu:** runtime trước kết liễu→Die→cleanup→READY rìu/khiên,
render material/lighting hiện hành, playback từng clip 1×/0.25× và xác nhận
người xem. A1 chưa đổi thành “đã sửa” toàn phần. Checkpoint này chỉ tích hợp
tài liệu; asset chưa đủ runtime acceptance không được đưa lên main.

### Đồng bộ bằng chứng lịch sử

Catalog `review/clips.json` còn truy xuất được trên E:. Đã kiểm metadata F682–F688
của `sword-double-attacks`: PNG đều 1280×720; PTS lần lượt
38.010/38.110/38.137/38.188/38.199/38.256/38.285 s, khớp ghi chú trực tiếp
đã có ở checkpoint trước. Đã đồng bộ 7 quan sát và 6 cặp nội bộ; không tính
cặp F688→F689 chưa xem. Tổng lịch sử thành 1.186 frame/1.180 cặp trên
1.900/1.890; còn 714/710. Bản trước/sau catalog và hash bảy PNG nằm trong
`historical-catalog-{before,after}.json`, `historical-F682-F688.json` tại thư mục
bằng chứng A1. Việc đồng bộ không phải review mới hoặc acceptance của R0.

Bộ READY 806/803 giữ nguyên là lịch sử 66749da1. R0 khác R ở 75 đường dẫn
Assets/tools lighting; Magic Circle, compact READY và lighting hiện hành là
baseline phải thu lại. Các giới hạn áp dụng trong bảng đầu báo cáo vẫn giữ;
chưa đổi trạng thái glyph Đặng_Á9, reflection, animation bị VFX che hoặc các
clip chưa có nguồn truy xuất thành đạt. Không chạy suite game cho tài liệu.

### Ledger nguồn A1 cuối

Mỗi hàng dưới tóm tắt hai biến thể đã xem; ledger JSON có dòng riêng cho từng
biến thể. Map preview là RGB chẩn đoán, không phải render shader. Điểm nối
runtime và playback giữ trạng thái chưa kiểm chứng.

| Clip / biến thể | Frame / GIF | Duration | Quan sát trực tiếp | Chuyển tiếp | Bằng chứng |
|---|---|---|---|---|---|
| axe/Walk / source + mirror X | F0 / GIF F13 | 100 ms | Chân mở, tay phải nắm cán dựng; lưỡi đủ hai cạnh. | → F1: Đầu/thân hạ, chân thu bước; cán dựng cùng tay. | `axe-Walk-00.png` |
| axe/Walk / source + mirror X | F1 / GIF F14 | 100 ms | Đầu/thân hạ, chân thu bước; cán dựng cùng tay. | → F2: Gối nâng, đầu rìu lên cao; grip không tách. | `axe-Walk-01.png` |
| axe/Walk / source + mirror X | F2 / GIF F15 | 100 ms | Gối nâng, đầu rìu lên cao; grip không tách. | → F3: Chân thu hẹp dưới hông; đầu/cán rìu liền. | `axe-Walk-02.png` |
| axe/Walk / source + mirror X | F3 / GIF F16 | 100 ms | Chân thu hẹp dưới hông; đầu/cán rìu liền. | → F4: Gối đối diện nâng, tay sau duỗi; cán không mất. | `axe-Walk-03.png` |
| axe/Walk / source + mirror X | F4 / GIF F17 | 100 ms | Gối đối diện nâng, tay sau duỗi; cán không mất. | → F5: Chân mở rộng, rìu nghiêng nhẹ phải; đầu lưỡi đủ. | `axe-Walk-04.png` |
| axe/Walk / source + mirror X | F5 / GIF F18 | 100 ms | Chân mở rộng, rìu nghiêng nhẹ phải; đầu lưỡi đủ. | → F6: Chân thu, tay sau hạ; vùng phải sát đáy sạch mảnh khiên. | `axe-Walk-05.png` |
| axe/Walk / source + mirror X | F6 / GIF F19 | 100 ms | Chân thu, tay sau hạ; vùng phải sát đáy sạch mảnh khiên. | → F7: Thân hẹp, hai chân gần nhau; không còn mảnh khiên đáy. | `axe-Walk-06.png` |
| axe/Walk / source + mirror X | F7 / GIF F20 | 100 ms | Thân hẹp, hai chân gần nhau; không còn mảnh khiên đáy. | → F8: Đầu nâng, gối co; grip liền, nền phải đáy sạch. | `axe-Walk-07.png` |
| axe/Walk / source + mirror X | F8 / GIF F21 | 100 ms | Đầu nâng, gối co; grip liền, nền phải đáy sạch. | → F9: Chân chuyển về mở, rìu dựng; không có vật rời bên phải. | `axe-Walk-08.png` |
| axe/Walk / source + mirror X | F9 / GIF F22 | 100 ms | Chân chuyển về mở, rìu dựng; không có vật rời bên phải. | → F10: Chân mở rộng, thân hạ; rìu giữ nguyên, mảnh khiên đã bỏ. | `axe-Walk-09.png` |
| axe/Walk / source + mirror X | F10 / GIF F23 | 100 ms | Chân mở rộng, thân hạ; rìu giữ nguyên, mảnh khiên đã bỏ. | → F11: Chân gần khép, đầu cao; bên phải đáy sạch trước seam. | `axe-Walk-10.png` |
| axe/Walk / source + mirror X | F11 / GIF F24 | 100 ms | Chân gần khép, đầu cao; bên phải đáy sạch trước seam. | → F0: chân mở lại, cán dựng; không còn fragment nhảy mất ở seam. | `axe-Walk-11.png` |
| axe/Die / source + mirror X | F0 / GIF F25 | 100 ms | Sọ tách khỏi vai, rìu còn dựng; không mảnh khiên sát chân. | → F1: Sọ nâng, ngực/eo/chân rời; tay phải còn bám cán. | `axe-Die-00-final.png` |
| axe/Die / source + mirror X | F1 / GIF F26 | 100 ms | Sọ nâng, ngực/eo/chân rời; tay phải còn bám cán. | → F2: Ngực nâng tách eo, chân bắt đầu rã sát sàn; rìu dựng. | `axe-Die-01-final.png` |
| axe/Die / source + mirror X | F2 / GIF F27 | 100 ms | Ngực nâng tách eo, chân bắt đầu rã sát sàn; rìu dựng. | → F3: Tay trái rời xa thân, eo xuống sàn; rìu còn cạnh phải. | `axe-Die-02-final.png` |
| axe/Die / source + mirror X | F3 / GIF F28 | 100 ms | Tay trái rời xa thân, eo xuống sàn; rìu còn cạnh phải. | → F4: Sọ hạ, lồng ngực rời, cán nghiêng trái quanh tay phải. | `axe-Die-03-final.png` |
| axe/Die / source + mirror X | F4 / GIF F29 | 100 ms | Sọ hạ, lồng ngực rời, cán nghiêng trái quanh tay phải. | → F5: Rìu đổ chéo qua sọ; xương/tay hạ sát sàn. | `axe-Die-04-final.png` |
| axe/Die / source + mirror X | F5 / GIF F30 | 100 ms | Rìu đổ chéo qua sọ; xương/tay hạ sát sàn. | → F6: Lưỡi/cán nằm chéo thấp qua sọ, đống xương đáy; không khiên. | `axe-Die-05-final.png` |
| axe/Die / source + mirror X | F6 / GIF F31 | 100 ms | Lưỡi/cán nằm chéo thấp qua sọ, đống xương đáy; không khiên. | → F7: Rìu nằm ngang, sọ còn nhô giữa; nền cạnh phải sạch. | `axe-Die-06-final.png` |
| axe/Die / source + mirror X | F7 / GIF F32 | 100 ms | Rìu nằm ngang, sọ còn nhô giữa; nền cạnh phải sạch. | → F8: Giữ đống xương, lưỡi bật lên một pixel; cán ngang đủ. | `axe-Die-07-final.png` |
| axe/Die / source + mirror X | F8 / GIF F33 | 100 ms | Giữ đống xương, lưỡi bật lên một pixel; cán ngang đủ. | → F9: Lưỡi hạ, xương/sọ giữ đống thấp; rìu ngang. | `axe-Die-08-final.png` |
| axe/Die / source + mirror X | F9 / GIF F34 | 100 ms | Lưỡi hạ, xương/sọ giữ đống thấp; rìu ngang. | → F10: Lưỡi trở lại cao nhẹ, sọ/đống xương giữ; không loop. | `axe-Die-09-final.png` |
| axe/Die / source + mirror X | F10 / GIF F35 | 100 ms | Lưỡi trở lại cao nhẹ, sọ/đống xương giữ; không loop. | Không loop; death→cleanup runtime chưa kiểm. | `axe-Die-10-final.png` |
| sword-shield/Die / source + mirror X | F0 / GIF F13 | 100 ms | Khiên trước thân, kiếm chéo lên; đầu có mũ, chân mở. | → F1: Sọ nâng nhẹ, khiên hạ; tay phải giữ kiếm, chân nguyên. | `sword-shield-Die-00-final.png` |
| sword-shield/Die / source + mirror X | F1 / GIF F14 | 100 ms | Sọ nâng nhẹ, khiên hạ; tay phải giữ kiếm, chân nguyên. | → F2: Sọ tách vai, khiên lệch trái, kiếm dựng hơn; chân bắt đầu rời. | `sword-shield-Die-01-final.png` |
| sword-shield/Die / source + mirror X | F2 / GIF F15 | 100 ms | Sọ tách vai, khiên lệch trái, kiếm dựng hơn; chân bắt đầu rời. | → F3: Sọ/ngực/eo rời, khiên rơi trái còn nguyên đường viền. | `sword-shield-Die-02-final.png` |
| sword-shield/Die / source + mirror X | F3 / GIF F16 | 100 ms | Sọ/ngực/eo rời, khiên rơi trái còn nguyên đường viền. | → F4: Khiên hạ dưới ngực, kiếm chéo phải; tay trái và eo tách. | `sword-shield-Die-03-final.png` |
| sword-shield/Die / source + mirror X | F4 / GIF F17 | 100 ms | Khiên hạ dưới ngực, kiếm chéo phải; tay trái và eo tách. | → F5: Khiên tròn gần sàn, sọ giữ cao; kiếm gần ngang. | `sword-shield-Die-04-final.png` |
| sword-shield/Die / source + mirror X | F5 / GIF F18 | 100 ms | Khiên tròn gần sàn, sọ giữ cao; kiếm gần ngang. | → F6: Khiên nghiêng trái sát nền; viền trái trả đủ, sọ/ngực đang rơi. | `sword-shield-Die-05-final.png` |
| sword-shield/Die / source + mirror X | F6 / GIF F19 | 100 ms | Khiên nghiêng trái sát nền; viền trái trả đủ, sọ/ngực đang rơi. | → F7: Khiên ép thành dải mỏng đáy; viền trái/núm không bị cắt. | `sword-shield-Die-06-final.png` |
| sword-shield/Die / source + mirror X | F7 / GIF F20 | 100 ms | Khiên ép thành dải mỏng đáy; viền trái/núm không bị cắt. | → F8: Khiên nảy nghiêng lên, núm sáng giữa; sọ rơi gần đống xương. | `sword-shield-Die-07-final.png` |
| sword-shield/Die / source + mirror X | F8 / GIF F21 | 100 ms | Khiên nảy nghiêng lên, núm sáng giữa; sọ rơi gần đống xương. | → F9: Khiên nghiêng lại phía sàn, sọ/mũ về đống; kiếm ngang. | `sword-shield-Die-08-final.png` |
| sword-shield/Die / source + mirror X | F9 / GIF F22 | 100 ms | Khiên nghiêng lại phía sàn, sọ/mũ về đống; kiếm ngang. | → F10: Khiên nằm thấp, núm lệch phải; kiếm/xương nằm sát đáy. | `sword-shield-Die-09-final.png` |
| sword-shield/Die / source + mirror X | F10 / GIF F23 | 100 ms | Khiên nằm thấp, núm lệch phải; kiếm/xương nằm sát đáy. | → F11: Khiên gần nằm phẳng, viền trái đầy đủ, núm sáng mỏng. | `sword-shield-Die-10-final.png` |
| sword-shield/Die / source + mirror X | F11 / GIF F24 | 100 ms | Khiên gần nằm phẳng, viền trái đầy đủ, núm sáng mỏng. | → F12: Giữ khiên phẳng cạnh đống xương, viền trái đủ; trạng thái kết thúc. | `sword-shield-Die-11-final.png` |
| sword-shield/Die / source + mirror X | F12 / GIF F25 | 100 ms | Giữ khiên phẳng cạnh đống xương, viền trái đủ; trạng thái kết thúc. | Không loop; death→cleanup runtime chưa kiểm. | `sword-shield-Die-12-final.png` |



### Checkpoint đối soát glyph và reflection trên R0

Checkpoint tài liệu `9df678c53c3d2d60f5330082ba6f51284cca9c79` đã lên main
và được fetch xác minh ancestry. Bản sửa A1 `1a2b6d4` đã commit/push trên
`micharel09/a1-shield-ownership`; asset chưa tích hợp main.

Mẫu không QA `Đặng_Á9` đủ điều kiện tái dùng **chỉ cho input và HUD reading**.
Hai PNG được kiểm lại theo receipt R: `name-input.png` SHA256
`8ac98d8feb4d6c70ed6001954ae645dc25bf96f6850b04712e1e91c6129b9206`;
`hud-reading.png` SHA256
`ea39e60671f2edabaef836fc8b3219f2cfbe21229618d80cb60bfd7895f9186d`.
Hai probe trên bản không QA đều undefined. Đã mở lại native và nearest 4×:
chữ Đ, ặ/dấu nặng, gạch dưới, Á/dấu sắc và 9 đọc được, không thấy ô vuông/cắt glyph.

Git object từ R66749da1 tới A1/1a2b6d4 của MainMenu.unity, SampleScene.unity,
Assets/Art/UI, Assets/Scripts/UI, Packages và ProjectSettings đồng nhất.
Delta thuộc world/READY và skeleton; không đổi text/font/material/canvas input/HUD
của mẫu này. Không dùng world cũ chứng nhận lighting mới. Bản sao và receipt
Git object ở thư mục bằng chứng A1 `glyph-reuse/`, ngoài CodexTemp.
Đây là tái dùng có điều kiện, không phải phiên nhập mới trên A1.

Reflection: DungeonWorld.unity R0, WetStoneReflection fileID 1632546716 vẫn
`m_IsActive: 0`; A1 không đổi scene/caller rendering hoặc bật reflection.
Reflection không áp dụng trong baseline này; không chứng nhận chất lượng khi bật.

Build thử đầu chứa map tạm đã dừng; giữ log/manifest riêng.
Snapshot cuối có 2.543 file, SHA256
`4a4538ea2810cf81b226ebb47d98da8217fa6ffda6f30dd5c07fe2f04f8f203d`.
Không đổi trạng thái nghiệm thu chỉ dựa trên build.

## Checkpoint thu R0+A1 — runtime hình còn mở

A1 vẫn ở commit `1a2b6d4` trên nhánh riêng. QA Windows Unity 2022.3.25f1
(530ae0ba3889), snapshot trên, build thành công tại
`runtime/results/20261027T142929.162161Z-3f0c4ef0828e45529ada23825d1c82d3-build-right`.
Strict repeated builder xác nhận serialized objects/references/listeners không đổi;
`game-loop-build.json` ghi qa=true. Cả 12 PNG albedo/normal/depth/material khớp
byte với commit sau import/build, theo `runtime-map-preservation.json`.
Không chạy full suite, stress, soak, profiling hoặc viewport ngoài hai cỡ yêu cầu.

### Hành vi và media đã thu

`capture-1` thu tutorial reading, combat kiếm/Double Strike/rìu/khiên và ba
điểm nối death→cleanup→READY→combat tiếp. Driver dùng phím/chuột; bridge chỉ đọc,
fixture Firebase và chặn provider thật/service worker.
Edge 156.0.4314.42, **Intel UHD 0x9BC4 / ANGLE D3D11**.
Nhãn Linux/Docker kế thừa trong toolchain.json không phải môi trường capture này.

45 check kỹ thuật đã qua trước lỗi ở phần player bổ sung. Luồng kiếm kiểm chờ,
Enter/click vị trí nút cũ không tiếp tục, từng chữ thường, sai sau prefix,
Pause chặn chữ/Resume giữ prefix, resize 1280×720→1366×768→1280×720,
carry và một handoff. Rìu/khiên chỉ kiểm điểm nối/carry.
Combat time tại chờ/intro lần lượt 41.68546676635742 / 152.0548095703125 /
194.65335083007812 s; HP40, score/letters/correct giữ qua READY→intro.
Combat kế tăng lại từ thời gian mang sang.
Contract hold 180 ms được đối chiếu ReadyRitualPresentation.LateUpdate và
ReadyRitualTests hiện có; không dùng số frame video làm phép đo engine chính xác,
không nhận test lịch sử là lần chạy mới.

Ba still resize đã mở native: compact frame, TYPE READY TO CONTINUE, READY
có đủ glyph/viền/padding, countdown ẩn, vòng trên sàn. Still không thay review
frame liên tục. QA label che tên player; mẫu glyph không QA được tái dùng riêng.

Phiên 1 gặp `Target page, context or browser has been closed` trong phần thu
player Hurt/death bổ sung; nguyên nhân đóng chưa xác định. Status toàn phiên
vẫn **failed**; 45 check không chứng nhận toàn phiên PASS. Clip trước đó đã
stop/export hoàn chỉnh. Generic fixtureReads/Writes bằng 0 nhưng fixture
Endless riêng có 1 read/1 write được intercept ở result, không ghi provider thật.
Không có Unity/JS exception trong report. Không phân loại thiếu quan sát/lỗi
công cụ thành lỗi game.

`capture-2` thu lại riêng player trên cùng build bằng input thường, không ép
HP/Animator: idle chạy thật, Hurt→idle và death→result; 4 check harness PASS,
0 exception. Điều kiện chờ của công cụ đổi thành overlay && hp<=0 vì bridge
trả sentinel âm sau cleanup actor tại result; không sửa game.
Media chính `capture-2/player-hurt-death-result.webm`; context video bổ sung
không cộng coverage. Clip/trace/still và `progress-receipt.json` ở thư mục
bằng chứng A1 ngoài CodexTemp, có SHA256/source/build và event carry.

### PTS, biên và phần đã xem

`decode-pts.py` ngoài repo chọn **PTS integer gốc**, chia run cùng kích thước,
giữ ordinal toàn media; không dùng filter n reset khi resize.
Đã đối soát số PNG/PTS/kích thước từng PNG.
F1217/43.541 s là 1280×720; F1218/43.590 s tới F1232/44.066 s là 1366×768;
F1233/44.103 s trở về 1280×720.
Duration frame cuối chưa xác định giữ null; không retime/nội suy/bỏ frame lặp.

Biên rìu ban đầu 64.531 s lộ giữa Slayer strike/recovery ở F1842, đã mở rộng
về 50.5 s. Trace có idle50.433, Hurt51.306, Attack1 preparation53.185,
Slayer entry54.226; đoạn tiếp tục đủ burst/exit, lethal, Die/cleanup/READY và
combat mới. Giữ hash/ghi chú/global ordinal của frame đã xem; đoạn hẹp bị thay thế.
F1436 xác nhận bối cảnh trước preparation kế tiếp, không nghiệm thu preparation
của động tác trước đoạn. Media toàn trận vẫn giữ.

Catalog `runtime-review/clips.json` là **ứng viên đang review**, chưa phải
manifest nghiệm thu cuối: 1.590 frame/1.584 cặp.
Tại checkpoint thu ban đầu đã mở 7 frame/5 cặp; checkpoint preview bên dưới
cập nhật thành 10/7. Không cộng với catalog lịch sử 1.900/1.890.
Raw Double Strike/combat vẫn giữ; cần chốt đoạn trước review, chưa khép toàn ma trận.

| Đoạn mới | Frame/cặp | Đã mở frame/cặp hiện tại | Giới hạn |
|---|---:|---:|---|
| axe-death-cleanup-ready-current |641/640|10/7|F1436–1439, F1842–1844, F1920–1922. VFX và source cel còn cần đối chiếu.|
| shield-death-cleanup-ready-current |247/246|0/0|Chưa mở frame encode.|
| sword-death-ready-current |475/474|0/0|Ba still resize không thay frame encode.|
| player-idle-hurt-recovery |79/78|0/0|Chưa kiểm đủ vòng/khớp source.|
| player-death-result |123/122|0/0|Chưa mở frame encode.|
| tutorial-reading-current |25/24|0/0|timeScale khóa; không phải vòng idle.|

Từng frame đã mở có phase/quan sát/chuyển tiếp/giới hạn/hash trong catalog.
Không tính cặp nối sang frame chưa xem. Vệt kiếm rời cần đối chiếu source,
chưa kết luận lỗi. F1920–1922 có kiếm player che vùng sàn A1, chưa đủ chứng nhận
không còn fragment trong toàn death.

### Preview dưới lighting hiện hành — checkpoint Walk

Hai bộ thử `lit-preview/` và `lit-preview-current/` không được nghiệm thu:
helper cũ thiếu presentation/post, CopyFrom kế thừa viewport serialized khác
runtime. Mỗi bộ chỉ mở một ảnh chẩn đoán, không tính 72 ảnh xuất thành đã xem.
Tool cuối tái hiện rect(0,0,1,1) của DungeonArenaCamera.OnEnable, pose camera
từ trace, pitch/profile, WorldTexture, world renderer và presentation/post R0.
Không đổi camera/lighting/source game để kiểm đạt.
Preview offline không controller/VFX/contact-shadow actor, dùng preset ánh sáng
cơ sở, không tái hiện chính xác flicker. Editor **NVIDIA GTX1650Ti** khác Intel
runtime; không chứng nhận pixel tương đương hoặc tạo caller runtime.

Bộ cuối `lit-preview-final/frames.json` có 72 cel/biến thể, mỗi biến thể gồm
native 1280×720 và crop nearest 4×. Ảnh Walk F0 source đầu có lỗi khởi tạo renderer
(dải đỏ trần). Đã giữ bản loại ở `rejected-initial/`, render lại sau ba warmup,
mở lại native/4× rồi thay canonical. Receipt `warmup-replacement.json` ghi
6.533 pixel khác trong ROI actor; không tuyên bố phần actor giữ nguyên render.

Đã xem đủ **Walk 12/12 cel × hai hướng = 24 cặp ảnh native/4×**, mọi chuyển tiếp
và seam F11→F0. Các frame F6–F11 không còn fragment khiên cạnh chân;
cán/đầu rìu và grip còn liền. Mặt thép phía xa tối nâu ở source và mặt gần sáng
ngà ở mirror là giới hạn ánh sáng/hướng được ghi riêng.
Walk tiếp tục **asset-only**, playback chưa xác nhận.
Ledger ghi từng cel/hướng, filename/SHA256/quan sát/chuyển tiếp/giới hạn.

Die rìu đang xem: F0–F4 cả hai hướng đã mở native/4×, chưa kiểm hết dải.
Tổng preview cuối đã xem 34/72; không nhận phần còn lại là đạt.
Runtime vẫn chỉ 10/1.590 frame, 7/1.584 cặp; còn 1.580 frame/1.577 cặp chưa xem.
Chưa chốt manifest nghiệm thu, playback 1×/0.25× hoặc xác nhận người xem.
A1 vẫn **nguồn đã sửa / runtime hình chưa kiểm đủ**, audit chưa đóng.

Ghi chú văn bản: hai checkpoint sau A1 từng bị shell chuyển dấu tiếng Việt
thành `?`. Nội dung tại đây đã khôi phục có dấu; không đổi hash ảnh, kết quả
kỹ thuật hoặc biến phần chưa xem thành đạt.

### Main tiến thêm khi tích hợp checkpoint

Lượt fetch sau kiểm trả origin/main `a70c1a29d0e9749bbca82c2ca53d721e6c2b02ec`,
mới hơn R0 ở HUD score/combo và flash parry ngang (72ab039d, 979f04a0,
cùng checkpoint tài liệu liên quan). Đã đọc diff SampleScene, ScoreUI_ComboHUD,
ImpactVfxPixel và ImpactVfxProfile: thay đổi layout/fit score, multiplier,
decay cùng flash parry có kích thước 2D và khóa variation.
Capture R0+A1 **không chứng nhận layout score/combo hoặc flash parry mới**.
Cần thu/kiểm lại đúng phần chịu ảnh hưởng; không xem tiếp clip cũ chỉ để đủ số
hoặc nhận bằng chứng task khác là đã trực tiếp xem trong audit này.

Nguồn A1, atlas/maps, DungeonWorld, actor material, camera/profile, READY và
tutorial không đổi trong delta này; source review và preview offline không VFX
giữ giới hạn đã ghi. Nhận xét input/HUD tên Đặng_Á9 chỉ áp dụng text/font và
reading của mẫu, không mở rộng sang score/combo mới.
Checkpoint tài liệu này được tích hợp độc lập; audit/rendering hiện hành vẫn mở.

### Checkpoint preview A1 đủ 36 cel — chưa đóng runtime

Đã mở trực tiếp toàn bộ `lit-preview-final/`: **72/72 biến thể cel**, mỗi
biến thể gồm native full canvas và nearest 4×; axe Walk 12, axe Die 11,
sword-shield Die 13, cả source và mirror. Đã ghi **68/68 cặp chuyển tiếp**
gồm seam Walk của hai hướng. Mỗi ảnh giữ SHA256 trong frames.json;
đã đối soát lại tất cả 144 file native/nearest theo ledger.

Rìu giữ đầu/cán/grip, không có mảnh khiên rời ở vùng đã sửa.
Die kiếm–khiên F6–F12 giữ phần viền/núm trả lại khi rơi, ép mỏng, nảy và nằm.
Die rìu F7–F10 và Die kiếm–khiên F7–F12 có chi tiết rất thấp/tối sát sàn:
nearest 4× đọc được thêm viền/cán, nhưng native không đủ để nhận mọi texel.
Không tự phân loại giới hạn tương phản thành lỗi mất pixel hoặc sửa lighting.

Đây là preview offline không VFX với giới hạn transform/preset/GPU đã nêu;
không thay clip runtime, source GIF, bản map chẩn đoán hoặc xác nhận playback.
Bằng chứng nguồn 36 cel và preview lighting đủ cel được giữ riêng.
Runtime vẫn 10/1.590 frame và 7/1.584 cặp; playback 1×/0.25× và người xem còn mở.
Asset A1 vẫn ở nhánh riêng, chưa đưa lên main.

### Current-main capture checkpoint: main321 + A1

Capture source: main `321ab65c` plus A1 HEAD
`762a3e92e280e2a99d74e2edaed229f0183327c8`; immutable 2,547-file snapshot SHA256
`fbedb17c05479be298464bda6cca6ba70f714406ead1d61ba2ff4692f5b112d0`.
Fresh-Library Windows Unity 2022.3.25f1 build exited 0; repeated builder unchanged,
all 12 A1 PNG files byte-identical after build (`runtime-map-preservation-current.json`).
Two attempts using a transferred Library failed with MonoScript.GetClass null;
diagnostic logs and rejected cache are preserved outside the repository.
No game change was made to pass the cache failure.

Evidence root: `D:/Projects/TypingHero-evidence/a1-shield-ownership`.
`capture-current-1/browser.json`: passed 46 functional checks, exceptions empty;
tutorial, four combats and three sword/axe/shield handoffs, detailed READY input
matrix on sword. `capture-current-2/browser.json`: passed 4 checks, exceptions
empty; player idle/Hurt/death/result with no input in the first sword fight.
Normal input, read-only QA bridge, fixture interception and real-provider blocking.
Actual runtime is native Edge 156 / Intel UHD D3D11; inherited Linux/container
labels in toolchain metadata do not describe this execution environment.
These results establish behavior/capture, not full visual acceptance.

CombatSeconds remained identical Waiting to Intro at 40.56234359741211,
136.7529754638672 and 177.50416564941406, increasing only in the next combat.
Video frame count is not an exact engine hold measurement. Existing READY hold
contract/test evidence is reused only where its relevant source is unchanged.

Directly inspected the entire new tutorial reading clip: **26/26 frames and
25/25 consecutive transitions**, full native canvas plus actor crop nearest 4x;
verified all 52 PNG hashes. Ledger:
`runtime-review-current/current-tutorial-scopes-clips.json`.
Video `capture-current-1/tutorial-reading.webm` SHA256
`003f94efea67ec96f897f09854e11bdbbecd893b55c2b74aead9afe28321e0f5`.
Original PTS 0 to 1.304 seconds and held durations retained; final duration unknown.
Pose/grip, reading cards/arrows/SPACE, score 0 and real/another/all tray remain
stable. Trace timeScale 0: this is not a complete idle cycle. Score/Combo card
covers the enemy skull top and Typing/Timer covers the player's trailing lower
body; mandatory hidden-detail previews remain open. QA label overlap is not
non-QA glyph acceptance. Lossy WebM does not prove identical source texels.

R0 catalog 1,590 frames / 1,584 pairs, with 10 / 7 inspected, remains historical;
it is not added to the current-main catalog. New combat/READY/player clips are
not yet fully inspected. Final acceptance manifest and human 1x / 0.25x playback
remain pending. Source/maps and all 72 offline A1 previews retain separate
results. A1 assets remain on the task branch until runtime visual acceptance.
Audit remains open; this checkpoint establishes no additional proven game bug.

### Tutorial hidden-detail supplement

Opened both no-card source-cel previews at native full canvas and nearest4x,
plus native/4x albedo, depth, normal and material diagnostics for
`male_hero-idle_0` and `Skeleton_01_White_Idle_0`.
Evidence: `tutorial-hidden-preview-final/review.json` and `maps.json` under the
permanent A1 evidence root. Source paths, rectangles, atlas hashes, preview
hashes, original scene transforms/flip and camera trace are retained.
Player trailing bent leg/boot remains continuous below the hip, front shin
and scarf-to-neck connection complete. Skeleton crown has its full stepped
curve above the eye socket; jaw/neck, sword grip and rib cage remain readable.

The preview uses the current DungeonSprite material, depth lookup, 320x180
world rendering and 1280x720 presentation/post pipeline, original actor
positions/scales and the tutorial camera pose. No UI, controller, VFX or actor
contact shadow; preset torch preview differs from exact runtime flicker.
Editor NVIDIA GTX1650Ti differs from Intel runtime, so this supplements hidden
source detail without asserting identical runtime pixels or a full idle cycle.
Mirrored diagnostic normal RGB is not vector-reencoded; actual transform
lighting is evaluated in the separate lit preview. The first attempt pointed
at an unsliced Without_VFX texture and failed; it is retained outside acceptance.
The final With_VFX imported cel succeeds and matches the trace's named cel.

Tutorial remains 26 frames/25 pairs directly inspected; this adds two static
cel previews and their maps, not additional video frames/transitions.
Human 1x/0.25x playback remains unconfirmed. Current handoff extraction is
complete: axe326/325, shield254/253, sword507/506 frames/pairs, including all
three native resolution runs across sword resize. Their visual review remains
in progress; these totals are availability, not acceptance. No asset integration
or audit closure is implied by this documentation checkpoint.
