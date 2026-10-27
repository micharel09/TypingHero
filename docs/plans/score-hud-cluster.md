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


## HUD production publication (2026-10-20)

Published https://micharel09.itch.io/clackblade, game3857509, upload19892576. Source6e626f5 on micharel09/hud-itch-release preserves approved public palette/UI input64de5b3a plus score/HUD changes; parent dirty WIP is untouched. No new reduced-motion handling.

- Unity2022.3.25f1/fly-server strict repeat serialize20261020T030558 PASS; imported only MainMenu/SampleScene builder output. Production20261020T031135 PASS, input1f2550f0a8a0d4f226417dc926801c0eba4983146500a238b106ded8fb43134c. QA bridge excluded, scanned ASCII and UTF16 payloads. ZIP CRC PASS; 16,491,939 bytes, SHA2567333b9de781abb84606769dcee8580c6c453452962005b22ce045bd29449f679.
- Native Edge155/D3D11 using original production HTML: tutorial, Endless intro/combat, pause/resume, four real Space inputs, successful parries, gold cue/teal success and Slayer warning icon visually inspected. Two smoke assertions PASS; no exceptions or provider writes. Initial local menu capture caught Unity splash; actual public menu subsequently inspected after waiting8s. Guard-break notice text was not captured in this production smoke; previous detailed scoped HUD QA covers it. No full balance/stress or repeated multi-size matrix.
- Authenticated browser-skill upload/Save and editor reload: new playable19892576, previous19891356 hidden and retained for rollback.31 non-upload business controls and15 unrelated uploads unchanged. Anonymous CDN29/29 non-HTML SHA256s match; index.html differs only by standard static.itch.io/htmlgame.js injection. Anonymous native Edge public menu renders, QA bridge undefined,0 exceptions/provider writes. Platform itch.io/html-callback POST was blocked; first smoke predicate wrongly counted this telemetry as a provider write, then corrected the predicate and reran PASS.
- Release manifest: [hud-release-20261020.json](../../marketing/itch-io/hud-release-20261020.json). Retained product ZIP in release child Builds/CLACKBLADE-WebGL-HUD-20261020.zip; canonical ZIP/source-input.tar.gz/receipt under fly-server /data/typinghero-game-loop/releases/hud-feedback-20261020/. Rollback reselects19891356, hides19892576 and saves.

Cleanup complete: task captures/profiles/helpers removed, browser session qegv stopped and owned server results20261020T030558-serialize/20261020T031135-production removed after canonical artifact verification. Shared cache/builds/other jobs preserved. Keep both task worktrees for source review/integration into main; remove only after integration. No Git push/merge performed.

## Remove redundant GUARD BREAK announcement (2026-10-20)

User approved removing only the text because guard break immediately enters Slayer. Continue on the clean existing hud-itch-release child/branch; parent WIP and earlier published release remain intact.

- Removed announcement duration/fade/first-break state. Existing GuardBreakR7 node and binding retain IDs, with empty text and disabled label in scene/builder; runtime reset keeps it disabled. Shield/warning transitions, real depletion/stun validation, Slayer and parry/HP feedback are unchanged.
- Updated existing HUDFeedback tests and native driver to require no announcement on every break and retain pause/recovery checks for the warning icon. Removed stale QA reflection for the deleted timer.
- Unity2022.3.25f1/fly-server result20261020T034519-quick, inputc156dca188c61db60603c42f539c95033b64852683f066f3e6f3bb28a326cb24: strict repeated SerializeOnly PASS; generated SampleScene equals checkout, all scene IDs retained. Scoped HUDFeedback PlayMode4/4 PASS (actual damage/parry/pause/first and repeated real break/icon recovery/disable lifecycle, plus three existing asset/fill checks). Existing browser driver syntax, source/docs whitespace and relative links PASS. The old QA timer reflection was removed alongside the timer.
- No geometry/font/combat changes and no new WebGL/browser matrix or itch.io publication in this follow-up. Previously published upload19892576 remains unchanged; branch holds this local follow-up for review/integration.
- Cleanup complete: removed owned result20261020T034519-quick and task helpers/XML/scene copies under E:/CodexTemp/TypingHero/remove-guard-break. Shared runner/cache and unrelated jobs preserved; retained existing child because changes are not integrated into main.


## HUD readability for side-view 2.5D (2026-10-20; in progress)

- Authorized base `8fa55dd`; branch `micharel09/hud-25d-readability`, child `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/workspaces/TypingHero/hud-25d-readability`, parent `e14b56e3-5bc0-49e9-bfdc-c342284235fc::C:/Users/ADMIN/orca/TypingHero`, origin `https://github.com/micharel09/TypingHero.git`. Parent WIP preserved. Keep child for review until verified integration; no push/merge/deploy requested.
- Presentation only: four flat #14111E/.88 backplates at native player(3,0,102,11), enemy(215,0,102,11), difficulty(108,45,104,13), clock(256,145,32,14). Behind HUD/tray/labels, no raycasts. Difficulty background shares CombatR7Status visibility.
- CombatR7Build owns static VT323 bitmap atlases8/10/12, one-pixel gutter, Point/Clamp. HP/difficulty/multiplier8, clock10, score12. BlackletterTitleBuild protects concrete TypingManager, CombatR7Status and CombatHUDLayout label bindings. Names/PARRY/menu and all three complete typing labels retain existing treatment. Score-frame image tint(.75,.75,.75,1); score text/pulse and feedback remain unchanged.
- Unity2022.3.25f1/fly-server serialize `20261020T055929-serialize`, input `3bc2189d043874c5978410e72a54850fb02b5ca9597564738e153a0a5100841b`: strict two-pass serialization and temporary final-font/content/bounds/backplate validator PASS. Imported actual SampleScene and three atlas assets/metas. All858 old scene IDs retained; exactly16 records for four Images/GameObjects/RectTransforms/CanvasRenderers added. Three complete typing text components byte-equivalent after newline normalization. Independent actual atlas-alpha check:97 characters each at8/10/12, required glyph pixels present; one-pixel gutter/Point/Clamp/static verified.
- Earlier serialize attempts054536/054731 failed sibling ordering; corrected insertion index preserves backgrounds before HUD across repeats.054949 passed repeat serialization but its temporary verifier selected a legacy inactive combo; final verifier uses the current HUD path and branding protects CombatHUDLayout bindings. Those incomplete attempts are not acceptance PASS.
- Final QA build `20261020T060631-webgl` PASS on input `f7f98d70df12176141ff3c76e5813833fa68b1659901f52835278f54d1da122b`; strict repeated builder PASS, QA=true. QA-only temporary runner reused run.py snapshot/lock/build/preservation and skipped the software browser; native checks used that immutable player. Final generated SampleScene equals checkout; all three atlas assets and GUID metas match build output byte-for-byte. No runtime/editor C# or assets changed after the build; later changes are driver acknowledgement/capture timing and documentation only.
- Edge155.0.4283.62 / Playwright1.58.2 / GTX1650Ti ANGLE D3D11, real keyboard/mouse only. Full feedback attempt completed all five difficulty/intro checks and six Normal duels, victory10HP/4259score/34parries; observed all eleven feedback flags including ready/cue/window/success/pose-lock/low-HP/Slayer/warning/shield/results/Continue. It then FAILED at15070checks/280.739s because restart's old identical40HP/intro state satisfied the driver before scene reload, so Enter was sent too early. This is not a full-profile PASS.
- Fixed the existing driver to wait for paused=false and ResumeButton removal after restart, and hold Enter70ms. Reused unchanged completed six-duel coverage. Focused same-player restart/death rerun PASS25/25 in36.788s: intro, pause/resume, completed restart, death/results; difficulty label/backplate both hidden at all four sizes. Existing native score-cluster profile PASS209/209 in63.055s: actual tutorial parry/Slayer, live multiplier/decay four sizes, parry renewal, pause/expiry/timeout/restart/natural Slayer exit. No gameplay timing or acceptance threshold changed.
- Visually inspected normal low-HP, active normal combo, Slayer at320x180/640x360/1280x720/400x300; intro, pause, restarted intro, victory and death/results samples. New text is sharp and contained; score/multiplier/decay stay ordered, clock background remains behind typing frame, original tray words/colors/materials remain. HUD position is stable across the existing Slayer zoom; sampled gold cue and popup clear important HUD/tray content. All reports have0JS/managed exceptions. Firebase fixtures/external blocking stayed enabled; two Endless writes were intercepted across feedback/lifecycle runs, zero production writes. No full balance/stress/soak/performance/production/release check.
- Final driver SHA256 combat_hud_feedback5fd07b26433ee53a23cbbdefdde4ffba21a580e9b71f1b6a580292d9ade2486f; score_clusterd380c6f0207f0ac08dec2e08818b8c170eb80f201677d3392636ea5caaaea249. Temporary lifecycle helper6fbe368d2b8c9099a9e484ae3d09e66bbfa5e9d1185c3f1eaa66ffde5f7199ba reused browser.cjs fixtures/state/start/keyboard helpers. Code/prose/driver whitespace/syntax and new relative links checked; Unity-generated YAML empty-field spaces retained as canonical output.
- Cleanup complete: removed task temporary player/captures/helpers/profiles/reports under `E:/CodexTemp/TypingHero/hud-25d-readability`, all five owned server results054536/054731/054949/055929-serialize and060631-webgl, and the verified idle task-created Orca shell. Three local QA listeners closed. Shared source/cache/toolchain/builds and other tasks preserved. Keep this child/branch for review; remove only after verified integration. No push/merge/deploy. Local commit contains only this task.
- Current scene only. Representative 2.5D scene does not exist in this baseline: bright/dark lighting, parallax, foreground/VFX cue occlusion and popup clearance require later verification. Screen-space HUD/floor native y138/tray y146 and existing camera/input/gameplay remain unchanged.


## Approved main integration and task closure (2026-10-20)

- User reviewed the headed native preview and approved merging to main and closing the task. Main fast-forwarded frome20c4ab to6d4243a; its complete tree equals the verified HUD branch. This also integrates the required previously published8fa55dd baseline. No implementation changed during integration; existing builder/glyph/native receipts remain applicable.
- Preserved all458 original parent WIP files byte-for-byte through a verified temporary stash/backup; index contains only committed main. WIP remains uncommitted and is excluded from the push. Original parent checkout and all other task worktrees/branches are retained.
- Preview rebuild063848 PASS, snapshoteba9f8b0733f32450970b5ea817923c1e28a4ffa42b063aabef1085c91d003a0, generated SampleScene equals6d4243a. Actual headed Edge opened Normal intro with Firebase fixtures/external write blocking. User approval closes this current-scene HUD task; representative2.5D lighting/parallax/foreground checks remain future work.
- Closure cleanup: preview process/window had already exited; task preview cache/helpers/profiles and owned server063848 result are removed. Temporary preservation stash/backup are removed only after byte-for-byte restoration verification. Delete only hud-25d-readability child/local branch after main integration; no remote feature branch was created. Shared runner/cache/builds and all other tasks stay intact. No itch.io deployment in this closure.


## Combo inside score frame / 2026-10-27

- Request: preserve the 294x66 score frame at (333,9); center score alone at x1,
  center score plus smaller multiplier on one row when active; decay inside bottom.
  Preserve score rules/timing/tints/pulse and character-side popup. No itch release.
- Worktree: D:/Projects/TypingHero-worktrees/score-combo-frame,
  branch micharel09/score-combo-frame, based on origin/main ee2811b.
  Original dirty checkout remains untouched.
- Builder reuses ScoreText/ComboHUD/ComboDecay, fonts and bindings. Row center y27;
  decay (15,51,264,6). Runtime reserves 1.2x pulse width and scales font display
  for long totals without dropping digits. CombatHUDLayout still owns popup anchor.
- Acceptance completed below: focused Unity geometry/scoring checks, strict repeated
  builder, direct WebGL review at1280x720/400x300 and combo lifecycle.
- Initial serialize stopped at unsupported Vector2 assignment in builder helper;
  removed unused assignment. First focused invocation selected zero cases because
  the new test lacked Quick category; not counted as passing evidence.

- Focused Unity 2022.3.25f1/fly-server result20261027T145616-quick:3/3 PASS
  (long totals/pulse, combo activation/timeout, score award/tier pulse). Geometry
  fixture spans4totals through20digits x3multiplier states x4pulse sizes.
- QA build20261027T145722 initially stopped at protected CobblestoneUI64 asset:
  server/local bytes are identical after CRLF-to-LF normalization. Normalized
  branding asset line endings only in this task checkout, with zero Git content diff;
  protected-asset assertions stay enabled.
- QA20261027T145930, input882e88e000ed107ebdb465d8659812bf95b7b107eaf20f0938a3d8d52f51aee1:
  strict repeated builder PASS. All1121original scene IDs retained. Imported only
  six affected Unity-serialized blocks (score/combo/decay geometry and follow binding);
  excluded unrelated fog/whitespace/block-order serialization.

- Final-font fixture20261027T151518-quick, inputb3044b14fb58292db85872307ac09b5cb6b80068ea371c00291e38010d6f4b24:
  1/1 PASS using the actual final CobblestoneUI64 font and48/42sizes. Earlier
  3/3 includes the two unchanged combo-expiry/award-tier regressions; its geometry
  fixture used the earlier R7 font and is superseded by this final-font result.
- Native final5: Edge156.0.4314.42 / Playwright1.62.1 / Intel UHD D3D11,
  72/72 PASS,51.406s,0exceptions/provider writes. Existing fixture/network blocking
  remained enabled. All33downloaded player hashes match immutable QA145930.
  Runtime/builder code did not change after that build; later edits affect tests,
  browser driver and documentation only. Driver SHA256:
  60fddc344f7c562a3e566f67a29cf5ad59c588995d36a7c20ef0af7747a1ee63.
- Directly inspected x1 and active-combo views at1280x720/400x300, Slayer x3 at
  both sizes, natural exit and normal combo after Slayer. Text/decay remain inside
  the unchanged frame; HP/tray stay fixed; popup remains by the enemy head.
  Native lifecycle also verifies pause/resume, expiry, timeout, real parry renewal
  and restart. Long totals/max1.2pulse use the48case offline geometry fixture.
- Browser pilots were not counted as full passes: old menu preparation/tutorial/
  encounter-reading waits, stale contact estimates, waiting for reward on each
  individual character, and a menu-transition click were corrected in the scoped
  driver. At x3 the existing renewal path can show one-update slider latency:
  remaining2.5s/slider.987436 at31.41ms. Timer assertion permits exactly one
  observed frame plus.002 normalized tolerance; game timing code is unchanged.
  Software pilots stopped before gameplay acceptance; final result is native GPU.
- No full suite, stress/soak, production build or itch.io release. Immutable QA
  player/Unity result receipts remain under /data/typinghero-game-loop/results/
  for reproduction; task-local browser/capture artifacts are cleaned after integration.
