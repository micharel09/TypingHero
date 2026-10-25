# Demo c?nh b?o Space c?nh khay g?

## Ph?m vi v? nh?nh

- Nh?nh th? `micharel09/parry-cue-demo`, t? `origin/main` b1a0df6. Gi? ri?ng ?? ng??i d?ng ??nh gi?; kh?ng merge/publish.
- Worktree: `C:/Users/ADMIN/orca/workspaces/TypingHero/parry-cue-demo`.
- Artifact task: `E:/CodexTemp/TypingHero/parry-cue-demo` (player, harness, b?o c?o v? clip; gi? ??n khi ??nh gi? xong).
- Cue m?c ??nh m?i; Esc ? F8 ? Esc ??i gi?a ph?m v? `!`, ch? trong phi?n. Kh?ng ??i TypingManager, ba t?, timer, camera, c?a s? parry, HP/stamina/score ho?c Animator.

## Acceptance

- Ph?m 32?16 t?i logical (23,153), c?ch tray 6 px; ??c l?p camera v? Slayer.
- ?n khi kh?ng c? ??n; vi?n h? ph?ch khi l?y ??; k?n ng? t? 200 ms tr??c va ch?m; k?n xanh khi c?a s? parry m?; vi?n x?m/X v? ?n ch? khi kh?ng th? m?.
- Gi? object/reference/?m c?a cue c?; ch? ??i Graphic.enabled v? kh?i ph?c ??ng. Kh?ng replay ?m khi ??i.
- API ch? ??c d?a event Anim_DealDamage, Animator time/speed v? hold 100 ms c?a thrust; cache theo controller.
- Pause gi? h?nh; hitstop gi? ti?n ??; contact/cancel/stun/Slayer/death/encounter d?n cue. Builder kh?ng tr?ng node v? gi? tuning.
- Tutorial c? m?u t?nh v? `Space: parry when the key fills.`.
- Ki?m Classic v? s?u attack, early/fill/late/locked Space th?t; mute t?i 640?360 v? 1280?720; l?i/t? d?i/timeout/Slayer/restart/menu/encounter. Review to?n b? frame v? chuy?n ti?p trong ?o?n capture ?? ch?n. C?m nh?n ph?n t?m c?n ng??i d?ng ch?i.

## Ngu?n asset v? ki?m tr?c quan

- https://kenney.nl/assets/input-prompts-pixel-1-bit ? Input Prompts Pixel 1-Bit 1.1; trang v? License.txt x?c nh?n CC0.
- Ch? import 5 tile tr?ng nguy?n b?n: 15,235,237,507,509; gh?p b?ng hai Image, kh?ng s?a pixel. License/source k?m d??i Assets/Art/UI/ParryCue.
- ?? tr?c ti?p xem c? 5 tile tr?n contact sheet `parry-cue-tiles.png`; border v? filled caps kh?p k? ho?ch. ??y l? ki?m asset t?nh, kh?ng ph?i nghi?n c?u video animation.

## B?ng ch?ng ?ang c?

- Doctor PASS: Unity 2022.3.25f1, license s?n, fly-server.
- Quick 20261023T165511: 1 test th?c thi/PASS, v?ng l?p 7 case (Classic + 6 attack); d? b?o kh?p damage th?t trong 45 ms, pause, speed?2, hold thrust v? clear sau impact/disable.
- Quick trong snapshot 20261023T165808: c?ng fixture PASS. WebGL d?ng ? protected Cobblestone font bytes do newline Windows; helper snapshot chu?n h?a LF ???c d?ng cho l?n k? ti?p. Kh?ng s?a asset font g?c.
- Build/presentation/native/capture acceptance v?n ?ang th?c hi?n; ch?a c? tuy?n b? ho?n th?nh.

### Ti?n ?? ki?m 2026-10-24

- QA 20261023T170922 / input c219b21e: build WebGL v? strict repeat-builder PASS. 39 serialized objects m?i, 0 object c? b? x?a; kh?c bi?t c? ? ngh?a ? object c? ch? g?m parent/reference cho hai key v? copy Shortcuts. C? th? lo?i whitespace serialize khi ch?p scene v?.
- Native pilot: Edge/MS Windows, mute b?ng browser flag. 640 ??t Space-at-fill (kh?ng m?t HP), early/late, kh?a, pause/freeze/F8, l?i g?/timeout, Slayer v? th?ng duel. L??t ??u failed v? harness ch? intro sau Continue d? game hi?n t? v?o duel k?; ?? ??i chi?u EndlessDuel.Start v? s?a harness.
- Pilot2 failed v? ch?a nh?n pose t? Space; pilot3 sau focus canvas ??t to?n b? c?c b??c tr?n v? Continue v?o DOUBLE STRIKE, r?i failed ? y?u c?u t?m t? ?7 k? t? tr??c khi th?ng. C?c failed report/video ???c gi? nguy?n; kh?ng t?nh l? full native PASS.
- Th?m case crossfade th?c t? attack sang Hurt. QA cu?i input 00adef3c: 2 Unity tests PASS (7 case timing + crossfade cancel); build/repeat-builder/native cu?i ?ang ch?y.
- S?a skin ch? b? qua subtree cue c? owner ri?ng; kh?ng thay theme chung. Builder gi? tuning v? th?m l?i child/reference thi?u.

## K?t qu? cu?i ???c ch?p nh?n

- QA `20261023T172219-webgl`, normal `20261023T172820-production`: c?ng input `00adef3c5b61a1d0a95dbfbb8e2c5ecfcbd529f492bdafb540489e845f78b893`. 2 Unity fixture PASS; strict repeat-builder v? c? hai build PASS. Normal marker qa=false; 5 JS/WASM/data payload kh?ng ch?a marker QA ? ASCII/UTF16. 33 player files t?i v? kh?p hash server.
- Scene cu?i gi? to?n b? ID c?, th?m39 object; ch?3 object c? ??i n?i dung: Shortcuts, parent child-list v? prefab added-child reference. Gi? th? t? document c? khi ch?p scene v?; map object chu?n h?a whitespace kh?p tuy?t ??i output Unity. Kh?ng ??i camera, combat config, controller/animation, queue/typing ho?c window250ms.
- Native mute: scope640 ho?n t?t to?n b? flow ? row ??u `native-final2/browser.json`, t?i `640-menu.png`. Raw report n?y v?n FAILED do polling kh?a1280 c?; kh?ng g?i c? report l? PASS. Reuse scope640 v? player/source b?t bi?n. Scope1280 ch?y ri?ng `native-1280-final/browser.json`:16checks PASS, completed=true. Hai scope0exceptions, Firebase fixture/external blocking gi? nguy?n,0ghi provider th?t.
- Space th?t t?i m?t k?n parry kh?ng m?t HP; early h?t c?a s? tr??c contact, late kh?ng h?i t? strike; m? c?a s? chuy?n xanh, kh?a chuy?n X/?n ch?. Pause gi? time/face; F8 kh?i ph?c ??ng legacy Graphic v? kh?ng g?i ?m. Hitstop c? cue ?ang ??c xu?t hi?n trong2frame640/4frame1280; ti?n ?? l?y t? Animator/scaled time. Typing error, timeout, Slayer, th?ng, Continue v?o DOUBLE STRIKE, restart v? menu ?? ?i qua b?ng input th?t.
- T? d?i `different` (9k? t?) ki?m ? c?640?360/1280?720 b?ng queue th?t v? ??i viewport khi pause; t?m b?ng27l?n Restart th?t, kh?ng s?a word pool/state b?ng QA. Vi?n cue kh?ng ch?m ch?/timer; geometry logical(23,153),32?16, c?ch tray6px. Tutorial v? m?u t?nh tr?c ti?p ki?m ?nh.
- Native sampled ready-filled lead kh?ng v??t200ms (max~189.7ms640/~193.4ms1280 do b??c frame). Kh?ng suy ra reaction time c?a ng??i, FPS hay kh?ng c? l?i d??i m?t frame t? s? n?y.
- Normal smoke `normal-smoke2`:5checks PASS c?ng ki?m tr?c quan menu/tutorial/combat/pause/??i mode/menu; QA state undefined. `normal-cue-proof.png` x?c nh?n cue tr?n normal player, kh?ng c? nh?n QA. L??t normal-smoke ??u ch?a v?o game n?n kh?ng d?ng l?m b?ng ch?ng gameplay.

### Review capture ??y ?? v? clip

Source video, timestamp v? ph?m vi ch?n ???c ghi trong manifest tr??c khi xem. Kh?ng l?y key pose thay review. ?? tr?c ti?p xem m?i frame v? m?i chuy?n ti?p li?n nhau, g?m qua bi?n sheet v? frame gi?/l?p:

| Capture/?o?n | PTS y?u c?u | Frame source | ?? xem / t?ng frame | ?? xem / t?ng chuy?n ti?p |
|---|---|---|---|---|
| native-final2/640, b?m t?i fill |16.925815?19.573580s|424?489|66/66|65/65|
| native-final2/640, early/kh?a |19.800910?21.913030s|496?547|52/52|51/51|
| native-1280-final, b?m t?i fill |19.000000?22.000000s|475?550|76/76|75/75|

T?ng194/194frame,191/191chuy?n ti?p. To?n b? preparation/contact/recovery/idle v? c?c ?o?n pause/restart ?i k?m ???c gi?. Cue ?n ??nh, vi?n?k?n?xanh??n ??ng; early xanh?x?m/X (kh?ng ch?)??n tr??c damage; kh?ng che tray/timer.

C?c sheet/manifest chu?n l? `frame-review-v2/640-*` v? `frame-review-v3/1280-filled`. C?c th? c?n clock t? duration/global correlation b? lo?i: clock trace/video1280 c? drift; ch?n theo native PTS quanh onset19.8s ?? ph? ?? chu k?, kh?ng s?a t?c ??/gi?u l?i. Derived sheet c?n clock sai kh?ng ???c t?nh l? review ??t. ??y l? review output UI capture, kh?ng ph?i nghi?n c?u reference-video hay s?a sprite animation.

Clip `SpaceCue-Demo.mp4`:7.76s,25fps,194frame,1280?752 (header32px ngo?i game);640 scale2? nearest,1280 gi? nguy?n. Gi? encoded timing/held frames, kh?ng audio. Full decode/frame-count PASS; ba preview tr?ng th?i/caption tr?c ti?p ki?m. SHA256 `6b5defdc399c13d5d4accdff3fc36d04f1db9e4661e7947042a509cadee98e6b`.

### B?n giao v? gi? t?i nguy?n

- Demo normal: http://127.0.0.1:8295/ ; clip: http://127.0.0.1:8295/SpaceCue-Demo.mp4 . Host loopback ph?c v? nguy?n player ?? ki?m, service worker d?ng fixture v? ch?n request ngo?i; kh?ng ??ng leaderboard th?t.
- Kh?i ??ng/d?ng: `E:/CodexTemp/TypingHero/parry-cue-demo/Start-Demo.ps1` / `Stop-Demo.ps1`. Gi? host/player/clip, snapshot hash, capture v? qualified/failed reports ?? ng??i d?ng ??nh gi?/reproduce; d?n sau khi ??nh gi? xong. Browser QA profiles ??ng/d?n khi k?t th?c; kh?ng d?ng server hay resource c?a task kh?c.
- Nh?nh/worktree gi? ri?ng theo y?u c?u; commit/push nh?nh th?, kh?ng merge main ho?c publish itch. T?nh ph?n t?m/?u ti?n gi?a hai cue v?n c?n ng??i d?ng ch?i ??nh gi?, kh?ng suy t? automation.

- Host bootstrap th?c ?8295 PASS: service worker active, v?o index/menu1280?720, QA globals undefined,0page errors/blocked external request; GET clip kh?p SHA256 file v? HTTP HEAD200/video/mp4. T?m profile Edge ?? ??ng/d?n; hai th? m?c sheet c?n clock sai ?? d?n, raw captures/reports gi? nguy?n.

## Follow-up: fixed HUD below fighters (2026-10-24)

- User chose fixed screen position. Tray moves from logical (61,146) to (61,126), size198x30; Space moves from (23,153) to (23,133), size32x16. Both rise20px, retaining6px horizontal gap.
- Scene and builders updated; camera, typing, timing and combat logic unchanged.
- Tutorial typing callout and shortcuts compacted to clear the raised tray; Start Battle moved below it. Copy/listeners and arrow targets retained.
- Normal/QA builds and targeted layout verification completed below; previous capture/clip records above describe the older lower HUD.

### Follow-up acceptance

- Immutable input `4933a167fa1e3bea25f4116d11424154fc89115fc2d685e7a67fcfd7b5da0495`; normal `20261023T180518-production`, QA `20261023T181516-webgl`. Both WebGL builds PASS; QA strict repeat-builder PASS. No runtime test suite repeated for this layout-only change. Both downloaded players:33files match remote SHA256 manifests.
- Native muted Edge: `hud-normal/browser.json`5checks PASS; `hud-qa/browser.json`19checks PASS. Real keyboard/mouse, fixture interception and external blocking retained. Tutorial arrows hit targets; guide copy fits; fixed tray geometry checked at both640x360/1280x720, including Slayer and pause/resume. Actual parries enter Slayer; real restart queues supply `remember` (8letters).
- Directly inspected normal combat/tutorial/pause at both sizes and QA Slayer, long word and Space cue screenshots. Text/timer remain clear; cue sits6logical px left of tray. This is static layout verification, not a new full-frame animation review. Previous194-frame review/clip describes the earlier lower HUD.
- Active demo host8295 now serves `hud-production-player`; old player/clip retained for comparison, not presented as updated layout evidence. Current screenshots/reports: `hud-normal/`, `hud-qa/`. Worktree/branch and task artifacts retained for user review; no main merge.

## Pre-main review and integration (2026-10-24)

- User accepted the raised HUD and requested thorough review before main. Integrated newer main3cd7293 into this task branch; preserve authored actor self-shadow, parry follow-up trim/speed and compact Pause/results. Scene tail conflict resolved by retaining all39 cue objects and both main SpriteDepthBinding components; documentation keeps both receipt histories.
- Review finding fixed: when rebound within one scene, presenter retained previous target Graphics indefinitely (the current encounter reload normally limits this lifetime). It now restores previous actor visibility and clears those references before binding the current target. New test walks20 encounter replacements and verifies bounded retention plus restoration on disable.
- Scoped cue timing/cancel/lifecycle tests:3/3 PASS on immutable inputa880ecef. Additional affected tutorial/HUD/result checks and QA/normal integration builds are in progress. Earlier demo screenshots/clip do not certify the newly merged rendering configuration.

### Integration review evidence

- Quick `20261024T011956-quick`:3/3 cue tests PASS. QA `20261024T012059-webgl`:7/7 affected tutorial/fixed-HUD/compact-result tests PASS, strict repeat-builder and WebGL PASS. Both input SHA256 `a880ecef93969efa868eee726fc7bd6dab0d81edacaa6ba40e4d0fdc899b5dd1`.
- Native `review-qa/browser.json`:16/16 PASS, Edge155.0.4283.62/D3D11,1280x720,925 observed state frames; zero exceptions/provider writes. Real keys verify fill/early/late/locked parry, Pause/F8 restoration, typing errors/timeout, Slayer/win, DOUBLE STRIKE continuation, restart/menu. Ten screenshots directly inspected. This is scoped integration/static UI review; no new full-frame sprite or performance claim. Earlier640x360 layout evidence remains valid because the tray/cue geometry is unchanged.
- Final source SampleScene matches every Unity-generated record, ignoring document order/trailing whitespace only. Canonicalization changed just min/max font-size24->18 on two tutorial labels with AutoSize disabled; same effective scene already used by the tested player. Receipt `review-scene-canonicalization.json` distinguishes this from the original input snapshot. Actor-depth/shader/lighting/animation files remain byte-for-byte main3cd7293.
- No remaining merge-blocking finding identified. Production/exclusion/smoke completed below; final remote integration verification follows.

- Production `20261024T013118-production` PASS on the same inputa880ecef; QA bridge excluded by runner. Both downloaded players match33remote hashes each. Native `review-normal`5checks PASS with zero exceptions/provider writes; tutorial/combat at640x360/1280x720, unframed Pause and mode-switch images directly inspected. Five JS/WASM/data payloads exclude ASCII/UTF16 QA and depth-preview markers. Unmodified normal video frame661 directly confirms the live Space cue at640x360 (`review-normal/normal-live-key-proof.png`); this is static presence proof only. Initial frame508 was rejected as live-cue evidence because it shows the static tutorial sample.

- Integration c36e591 pushed as a fast-forward to main after fetch confirmed base3cd7293 unchanged. Final remote verification follows the receipt correction commit. No game code changed after the validated integration.
- Retain loopback demo/player/captures/receipts under `E:/CodexTemp/TypingHero/parry-cue-demo` for follow-up playtest; clean after user review. Remove the clean task worktree/local branch after remote ancestry verification. Preserve the remote task branch and original dirty checkout.


## Per-strike one-second warning (2026-10-24, revised after user clarification)

- Authoritative latest instruction: show the **white outline and white SPACE label** about1s before each strike as advance warning. Only the existing ivory filled face means it is time to press. Latest placement instruction supersedes the tray position: put it above the enemy head. No early-input queue; the manual250ms parry window remains unchanged.
- Extend preparation using the existing first cel (Classic0ms, sword/shield Attack1 500ms, Attack2 800ms, axe Attack1 400ms, Attack2 600ms). Authored swing/contact/recovery/sprites unchanged. Replace old thrust100ms hold with the calculated hold; legacy cue/audio remains final300ms, once per strike. Fill lead stays200ms; teal window and gray/X lock remain.
- Worktree `C:/Users/ADMIN/orca/workspaces/TypingHero/parry-one-second`, branch `micharel09/parry-one-second`. Started from origin/main4b57eb6, then fast-forwarded toae1271d (torch assets/docs only) while preserving task edits. Original dirty checkout preserved. Artifacts `E:/CodexTemp/TypingHero/parry-one-second`; old demo/evidence untouched.
- The intermediate queued-input prototype was rejected by the clarification and removed in full. Its QA build873c5fb8 and22native checks are **superseded, not acceptance for this revision**. Earlier fixture error (forced chain index not reset) was corrected; intermediate receipts remain for traceability.
- Reusable enemy-only preview from `20261024T022053-webgl`, inputbd29fff9: source attack logic/assets match this revision. Directly viewed300/300 sampled frames and293/293 pairs at25fps, native/2x nearest, plus all76 source cels. Per-frame observations/hashes in artifact `frame-review/review.md` and `manifest.json`. This fixture has no parry/input behavior. Final drawn idle transitions, mirrored preview and selected live capture review remain pending.
- Current verification pending: real manual Space during outline vs fill on seven attacks; existing legacy cue/pause/dedup/chain recovery; final idle/manual-pose checks; muted native keyboard test and normal/QA builds on the revised behavior. No completion or main integration claim yet.


### Enemy-head placement follow-up

- Latest user instruction: move the same Space key above the enemy.32x16logical size and white-outline/final200ms-fill semantics remain. Stable BoxCollider2D head landmark,8logical-pixel clearance and3reference-unit snapping; reuse CombatViewportCamera projection (including DungeonArenaCamera presentation rect). Weapon frame bounds cannot move the cue. Pause holds its position. Tutorial sample stays static.
- Before this location change, phase revision `20261024T025930-webgl`, input02175c97d88bfb6aec879f27a4794910198e32c37f41983110eefe8b785d9394:5/5 scoped Unity tests PASS,0failed/skipped,26.4771956s. Includes14 real manual early/fill contacts on seven attacks and next-strike independence, prediction/speed/pause/cancel, presenter lifecycle, complete recovery/idle traces. Runtime parry code is byte-for-byte main. Reuse these behavior results because the follow-up changes presentation position only.
- Stopped only this task's superseded old-position build and canceled its queued normal build after confirming helper/profile/snapshot/container ownership; other runner jobs preserved. New anchor/viewport/weapon-bounds test and head-position builds/native checks pending.
- Task base advanced by fast-forward to main4e7ccc6 (pillar torch assets/world scene/docs); no parry runtime/UI overlap. Final demo will retain this newer world lighting.


### Final head anchor checkpoint

- Final clearance is8logical pixels: direct source inspection found the raised axe blade can touch a4px cue footprint. The larger gap keeps the button above the helmet/raised blade without changing size or tray position.
- Final anchor test `20261024T031855-webgl`, input3ee81546511ae82a7ba7394370a0399d0aaa9d71a470984ce4e5fa5a07b156ff:1/1 PASS. Head centering/clearance, weapon-bounds independence, enemy movement and camera viewport conversion. The first fixture omitted Physics2D.SyncTransforms after moving its collider in the same test tick; corrected the fixture. Intermediate4px builds were stopped/canceled only after ownership checks; no outside job interrupted.
- Complete left-facing enemy-only trace:314/314frames and307/307pairs directly viewed at native/2x nearest, including14additional drawn-idle frames; all11PNG sources exactly match baseline4b57eb6. Per-frame observations and hashes: artifact `final-frame-review/review.md`, `manifest.json`, `source-hashes.json`;28numbered sheets and normal/slow WebP. Earlier right-facing300frame preview remains source comparison. Lit-scene/VFX and cue-position acceptance awaits native capture; no all-seven lit/VFX claim.
- Merged newer main9d41188 after reviewing added result presentation/stat labels and pillar-torch placement. It does not alter parry rules or cue bindings. Final normal build targets this merged tree; scoped source behavior/head tests above are reused for unchanged consumers. Native integration and remote main verification pending.

### Final native head-cue acceptance

- QA031855 strict repeated builder and WebGL PASS, input3ee81546511ae82a7ba7394370a0399d0aaa9d71a470984ce4e5fa5a07b156ff. Downloaded33files match immutable remote SHA256 manifest. Source scenes match all1107SampleScene/440MainMenu generated records, ignoring record order and trailing line whitespace only; no scene edits required.
- `native-head/browser.json`:26/26 PASS, muted native Edge155.0.4283.62, Intel UHD/D3D11, actual Playwright1.58.2 (server manifest1.63 identifies the separate Linux harness). Actual keyboard/mouse verifies head position/size at640x360/1280x720, manual filled parry, held-key next-strike failure, early expiry, late input, typing, Pause/F8, Slayer, results, separate combo hits, restart/menu;0exceptions/provider writes. All9screenshots directly inspected.
- Native source video `807df7b2cc53d42a63057c546382cac7.webm`: four complete selected segments registered with sourceSHA/PTS/reasons before extraction. Directly viewed278/278encoded frames and274/274pairs in48sheets, preserving40ms timing/repeated frames. Read-only trace/video alignment has4mismatches/595comparedframes, offset-0.14s. Full observations: `native-frame-review/review.md`, `selection.json`. No cue/tray overlap or lingering cue at contact/restart found. The first initial strike was unpressed while screenshots ran; a mistaken original parry label was corrected without dropping its frames, and the complete actual second-strike parry was added.
- Short clip `SpaceCue-Demo.mp4` combines the complete successful parry and two-hit combo segments; SHA256541284c62aec0f654f30c30164c1247be8fa7a3a128c60e91205d3a490531d05. Lit capture covers representative sword/combo, not every weapon/VFX combination. All-seven enemy sprite/preparation coverage is separately recorded above; subjective distraction/rhythm still needs user play.
- Normal033136 PASS on the same3ee81546 input; all33downloaded files match remote hashes. Five JS/WASM/data payloads exclude ASCII/UTF16 QA markers. `native-normal/browser.json`:5/5 PASS,1280x720,0exceptions/provider writes; eight screenshots directly inspected, including the live overhead outline in `normal-new.png`, tutorial/menu/Pause. Corrected copied report viewport metadata (normal used1280 only; QA covers both sizes).
- Demo host8302 serves the exact verified normal player and new clip; old ports8301/8295 and their task resources are preserved. Bootstrap PASS: active fixture service worker, index/menu1280x720, both QA globals undefined,0page errors/blocked external requests. Clip HEAD200/video/mp4 and downloaded SHA match. Retain artifacts/player/clip/server under `E:/CodexTemp/TypingHero/parry-one-second` for user review, clean after acceptance; Start-Demo.ps1/Stop-Demo.ps1 manage only this host. Task worktree/local branch will be removed only after clean remote integration; keep remote branch and original dirty checkout.

## Compact SimpleKeys tap cue (2026-10-24)

- User requested palette/centering correction and a pixel Space asset with pressing animation. Clarification: keep the1s advance warning still; only the final200ms repeats press/release, not a single tap and not an early-input queue. Use100ms cycles; preserve the manual250ms defense window and all combat timing.
- Independent Orca worktree `space-press-cue`, branch `micharel09/space-press-cue`, base main9c48f5d (includes approved success sprite recovery). Original dirty checkout and all other worktrees/servers preserved. Artifacts: `E:/CodexTemp/TypingHero/space-press-cue`; proposed new demo8303, existing8302 remains the earlier revision.
- SimpleKeys Classic by beamedeighth: author page confirms free commercial/personal use, no asset resale, custom permission (not CC0), no generative AI. Direct download returned403 locally and on runner. Obtained original two-frame Dark/Light SPACE sheets from credited Mimic-Dungeon public project pinned180c911869bac33cdf90eb19e16371a58620c97a. Both native/5x sheets directly inspected: released and depressed face/shadow, same67x16canvas per frame, pressed position lowered. Source/license/hashes in `Assets/Art/UI/ParryCue/SimpleKeys/SOURCE.txt`. Only those two unchanged PNGs imported; use UGUI9-slice to compact the empty face to32x14logical, warm HUD tint and existing SPACE label. No source pixel redraw.
- Stable skull-top landmarks from directly inspected first idle cels (96x64,16PPU,pivot45.12/26.24): Classic(44.5,46), sword/axe(43.5,46), shield(39.5,48) in bottom-left source pixels. Presenter transforms corresponding local points, including actor mirroring, camera viewport and3reference-unit snapping;8logical-pixel gap. No weapon/collider bounds in cue positioning. Serialized controller-to-landmark bindings preserve authored tuning on repeat builder runs.
- Presenter uses actual remaining impact time to select still/up/down; active parry stays teal/pressed, locked gray/X hides label, pause/hitstop holds drawn frame, contact/cancel/Slayer/terminal/rebind clears as before. Tutorial sample uses the same new cap. Runtime input/HP/score/attack code unchanged. QA probe adds read-only drawn-frame and skull-point observations.
- Verification pending: scoped presentation/anchor/rebind tests, strict repeated builder and QA player; native muted real keyboard at640x360/1280x720, complete selected tap/transition capture review, normal build/exclusion/smoke and new demo/clip. One queued candidate was canceled before lock acquisition to correct active-window light-face contrast; no running external job interrupted. Do not infer completion from previous-player receipts.
- QA `20261024T042640-webgl`, input551e35837c29531ac6d34ad0e1e6eed660e0b4d4b05f7eaf4ae09ce79f16ca31:3/3 scoped Unity tests PASS,0failed/skipped,0.1872721s. Real authored countdown drives two tap cycles, freezes during hitstop, holds teal pressed during the actual manual window, shows lock and clears/restarts; geometry tests cover mirror/movement/viewport and independence from weapon/collider offsets;20-rebind restoration retained. Build and native checks remain in progress.
- First QA042640 and normal042942 builds on551e3583 were blocked by main9c48f5d's missing authored depth entry for new `male_hero-parry_lift`; repeated builder itself passed, but no player acceptance. Fetched and merged main4121c10, which already includes the focused authored-depth fixd6eb563 plus verified terminal-result HUD hiding. Preserved both; no task-owned lighting/sprite/timing changes. The old production helper completed its failure before a targeted cancellation could run; no outside container/process was touched. New merged build/native verification pending.

- Initial merged QA043840/input0ad8a37d:3/3 scoped tests and strict repeat-builder/WebGL PASS;33 downloaded player hashes match remote. Native pilot verified still warning, repeated up/down source states and head geometry, but manual-parry input arrived after contact and the pilot timed out. Its driver serialized a large observation before keydown; final driver reads only a small timing tuple and records the attempt before keydown. This is not a passing manual-input receipt.
- Direct native inspection also found the existing TMP label touching the original SPACE symbol. Superseded that overlay version: use untouched Dark/Light SPACEALTERNATIVE sheets with baked SPACE letters, plus blank Dark SPACE only for the lock. Three pinned Git blobs independently verified. Compact37x14logical cap uses Unity9-slice borders34/4/3/10 to preserve glyph pixels and source1px depression; old Label node retained inactive. Dark/light asset GUIDs preserved; warning sheet newly imported. New immutable input5b20fcb5 is queued for final verification. Earlier32x14 receipts do not prove final visuals.
- User requested a gameplay preview clip. Final capture will include complete single-strike and combo preparation/contact/recovery; diagnostic read-only frame barcode avoids the native recording clock drift found in the superseded pilot. Original PTS/repeated frames will be retained for selected frame review.

- Delivery scope remains the isolated demo/branch for user preview, as requested in the original plan and latest clip request; no main integration before this visual evaluation. Retain task worktree for follow-up tuning, alongside the artifact/host.

- Final-candidate QA051953/input5b20fcb5:3/3 scoped tests PASS,0failed/skipped,.1835948s, strict repeat-builder and WebGL PASS;33 player files match remote hashes. Serialized inspection caught a new warning-sheet import bug before visual acceptance: renamed sprite metadata inherited Dark names and Unity produced duplicate zero local IDs. This candidate is invalid for delivery despite passing behavior tests. Give warning frames correctly named, distinct nonzero persisted IDs and own sprite IDs while retaining the PNG GUID; corrected inputf4ac2a1d queued. Stopped only owned production helper2126444/container after verifying profile, helper marker and5b20 input. Another unrelated runner job remains untouched.
- The invalid-ID native pilot also missed initial menu input (1check, no gameplay); focus the canvas before Play and retry only if the name panel has not opened. Preserve its report/video as native-invalid-id-menu-pilot. No native pass claimed for this candidate.

- Latest user correction: button is too large; shrink only. Live/tutorial cap now28x11logical (84x33reference), about25% smaller than37x14; lock24x24reference centered inside it. Keep8logical-pixel skull gap, original assets/Point filtering, every timing/color/input rule and existing UI layout. Builder defaults match the new size and retain authored inactive-label nodes. New layout-only QA/normal outputs queued; prior source behavior checks are reused.
- QA053937/inputf4ac2a1d passed3/3 tests and repeated builder, but its qa:true player lacked the compiled observation bootstrap. Native startup timed out although the game menu rendered. Normal055352 onf4ac2a1d builds/payload exclusion33hashes/5JS-WASM-data markers pass; native visual runs confirm real menu/tutorial/combat/Pause and one successful parry, but repeated PNG polling caused capture/input latency and is not final timing evidence.
- External QA runner now applies an explicit WebGL-only TYPINGHERO_AUTOMATION project define after extracting its isolated snapshot, records the overlay, and rejects compiled players missing both observation globals. Shipping source/settings remain unchanged. QA060202/input202dc53d passes3/3, repeated builder and compiled bridge; all1121SampleScene/440MainMenu records match the tracked canonical scenes. Its26-check native run passes manual filled parry at both sizes, held-key failure, early/late, typing, Pause/F8 and Slayer. Combo acceptance remains pending: auto-input observations arrived with only67-73ms remaining and no window opened. The next driver compensates its observation round trip with a280ms trigger; this affects test inputs only, not cue200ms or combat250ms. Preserve incomplete captures/receipts; no complete-native pass claimed.

### Compact preview delivery — 2026-10-24

- Final live/tutorial size28x11logical; original asset Point filtering scales the supplied glyph/borders at this compact size. Timing, colors, head landmarks and combat unchanged by shrink.
- Final QA `20261024T063401-webgl`:3/3 scoped tests PASS, strict repeated builder/WebGL/compiled bridge PASS,33 downloaded file hashes verified. Final normal `20261024T064636-production`:build PASS,33 hashes verified, five JS/WASM/data payloads exclude QA markers. Framework/WASM byte-identical to preceding validated runtime; only serialized UI data changed. Canonical scene/metas match generated output.
- Native Edge156.0.4314.42, Playwright1.58.2, Intel UHD/ANGLE D3D11, muted: native-head has32 valid assertions before an incorrect harness Berserker-vs-Guard route expectation (not a complete profile pass). Focused native-guard17/17 PASS follows actual Sword/DoubleStrike/Heavy/Guard route, all skin head positions, restart/menu. Final native-direct-final19/19 PASS covers actual filled parry, early expiry, per-hit combo, Slayer and cleanup. Normal native-normal5/5 PASS; all8 screenshots directly inspected. Existing seven-attack timing/real-contact receipts above reused for unchanged combat. No new all-seven lit/VFX coverage claim.
- Old Playwright recording replays older frames and is superseded. Final direct CDP capture retains all2332 original PNGs ordered by original presentation timestamps; zero Unity frame regressions. Registered six complete segments and directly inspected all392frames/386pairs on67 sheets. `E:/CodexTemp/TypingHero/space-press-cue/native-direct-review/review.md` records observed transitions, source hashes and timing. Technical visuals pass; subjective distraction awaits user play.
- Clip `SpaceCue-Demo.mp4`:9.733s,960x540 crop,235 original frames/235 encoded frames, silent H.264; SHA256 `d1bfa7316e81ddb84ec2132a8c95e35a06961077d7d9f477a4a9c90eda3085f5`. Source timing retained. HTTP200/video-mp4 hash verified. Demo bootstrap PASS, active fixture service worker,1280x720, no QA globals/errors. Demo http://127.0.0.1:8303/; clip /SpaceCue-Demo.mp4. Esc -> F8 -> Esc compares modes for this session.
- Keep branch `micharel09/space-press-cue`, worktree `C:/Users/ADMIN/orca/workspaces/TypingHero/space-press-cue` and artifact/owned8303host for user preview. Main integration intentionally deferred by demo scope; newer main213e67d fetched but not merged into the reviewed trial. After visual acceptance/integration, stop owned host via artifact Stop-Demo.ps1 and remove only clean preserved task resources. Original checkout and unrelated servers preserved.

## Tutorial Space example continuation (2026-10-24)

User-approved scope: replace the static tutorial cap with an in-place loop,
reference960x540 (328,216),304x80. Keep cap28x11logical, all four HUD cards,
typing tray and Start Battle. Existing sprite assets only; no combat changes.
Timeline: warning800ms, light press/release200ms (100ms cycles), teal open250ms,
gray/X unavailable750ms, hidden release750ms; total2750ms. Teal means an open
parry window, not a successful parry. English captions follow the approved plan.
Unscaled tutorial clock; Pause holds, Resume continues, reopening/restart resets.
Builder adds missing nodes/bindings and retains existing example tuning. Encounter
clone disables TutorialOverlay and hides SpaceExample with the other guide nodes.

Acceptance checklist: five phases and wrap; input/score lock and viewed storage;
Pause/F8/Resume; restart/menu replay; Enter->fresh encounter with no guide leak;
real Space parry; repeated builder; readable640x360/1280x720; complete-loop and
Pause/Resume captures with all selected frames/consecutive transitions inspected.
Verification is in progress; no pass claimed yet. Continue on the existing
micharel09/space-press-cue worktree. Keep isolated demo before main integration.
Task artifacts: E:/CodexTemp/TypingHero/space-press-cue; earlier clips preserved.

- Builder first attempt072916 failed strict repeat comparison: global typography
  changed the new Heading/Caption18->48 on pass2. Owns now covers SpaceExample,
  preserving its authored labels. Corrected serialize074234/inputbd10f7ed PASS;
  only5existing SampleScene records changed and10added,0removed. Other records
  are preserved in original order; changed/new records have whitespace normalized.
- QA082204/inputa820ba0399c0ec4334a70fc781fd849001380ca55fd9c9a5b20200d45fd257c0:
  8/8Unity tests PASS,0failed/skipped,3.5245101s. Includes all5TutorialTests and
  3directly affected cue presentation/head-anchor/rebind tests. First180s serializer
  wait and first1800s QA wait expired before Unity ran; retries preserve other jobs.
  Player/native/frame-review acceptance remains pending at this checkpoint.

- QA082204 WebGL/compiled bridge/strict repeated builder PASS;33downloaded player
  hashes match remote. Current scene records match generated output ignoring only
  document order/trailing whitespace; no further source changes after tests.
- Native Edge156.0.4314.42/Intel UHD/ANGLE D3D11, muted: tutorial-native-final2
  22/22checks PASS at1280x720/640x360,56.5015s,0exceptions/provider reads/writes.
  Five phases, score lock, Pause/F8/Resume reading lock, restart, Enter/viewed,
  example absence in combat, one real parry per size and menu replay verified.
  Native-1 passed9checks but stopped at menu replay's preparation card, which the
  driver omitted. Final pilot then missed teal while synchronous screenshots
  blocked polling. Driver now observes every read-only QA publication during the
  same5.7s capture; all five captions remain required. Both failed reports retained.
- Full selected recordings directly reviewed on all13sheets:1280 180/180frames,
  179/179pairs,0..9.656381s;640 272/272frames,271/271pairs,0..8.531490s. Total452/452
  and450/450; no sampling, all holds and sheet boundaries included. Labels/key do
  not overlap cards/tray, wrap clears X, pause hides guide and restores same teal
  or gray/X state. Per-frame CSV observations/PTS/hashes and scope are in
  E:/CodexTemp/TypingHero/space-press-cue/tutorial-native-final2/review/review.md.
  This is UI capture review, not a new actor animation/performance/usability claim.
- Loop export uses complete warning->tap->open->lock->release frames26..94 at1280;
  original period2.750392s. Tutorial-Space-Example.mp4:69original frames +1terminal
  repeat to retain final hold,2.751s, SHA256
  6bf62ab888038f61bab86ba7e59c6809f86cd66593ccd8a18f100ad7bfdf5563.
  Companion Tutorial-Space-PauseResume.mp4:180original+1terminal repeat,9.689s,
  SHA256 d947ab3da7c5617d623aaccb9177dc43478e31085e547fd73ded2a36a6c9e964.
  Both decode completely; no source frames dropped, millisecond encoding timebase.
  Earlier B-frame export lost terminal hold in container duration and was replaced.
  Normal player/demo update remains pending at this checkpoint.

### Tutorial example delivery verified

- Normal083354 uses the same a820ba03 input as QA082204; WebGL build/QA exclusion
  PASS.33downloaded hashes match; all5JS/WASM/data payloads exclude ASCII/UTF16
  QA/depth markers. Native tutorial-normal5/5checks PASS,44.3385s,0exceptions/
  provider reads/writes. Actual tutorial/release phase, live combat without guide,
  Pause and mode/menu images directly inspected. No new full-suite run.
- Final encoded loops also directly inspected after decode:70/70frames69/69pairs
  and181/181frames180/180pairs on all8encoded sheets; terminal holds preserved.
- Demo8303 now serves immutable083354/player. Host bootstrap PASS: service worker
  active,1280x720, QA/combat globals undefined,0page errors; actual menu inspected.
  /Tutorial-Space-Example.mp4 and /Tutorial-Space-PauseResume.mp4 return200/video-mp4
  with exact local SHA256s above. Old SpaceCue-Demo.mp4 and old player are retained;
  old clip hash d1bfa7316e81ddb84ec2132a8c95e35a06961077d7d9f477a4a9c90eda3085f5.
- Keep worktree/branch micharel09/space-press-cue and
  E:/CodexTemp/TypingHero/space-press-cue (players, raw frames, reports, per-frame
  review, clips, host/helper) for requested preview. Stop-Demo.ps1 stops the owned
  8303host after review. Remove only clean task resources after acceptance and
  verified integration. Main integration is intentionally deferred by this plan;
  original dirty checkout and unrelated jobs/servers remain untouched.


## Two-step tutorial with captured parry modal (2026-10-24)

User plan supersedes the single-page example: HUD step 1/2 retains four cards/five arrows; Next or Enter opens step 2/2, an 816x480 modal on 960x540. Back restores HUD; only Start Battle completes existing TutorialProgress_v1. Held Enter/same-frame duplicate confirmations cannot skip a step. Esc/Pause retains reading/input/score lock. Restart/replay starts on step 1. No main integration; continue existing space-press-cue branch.

Capture checklist: idle >=500ms, enemy preparation, Space warning/light/press-release, player defense, successful contact, recovery of both actors, idle >=500ms, loop boundary. Source is the real keyboard-driven native-direct-final successful parry recording from this trial; no actor/combat changes since that capture. Source directory E:/CodexTemp/TypingHero/space-press-cue/native-direct-final/compositor, original frames.json timestamps. Registered prior full successful segment trace18.770230..21.519835s; expanded source window starts 500ms before its first original PNG and ends after >=500ms after its final PNG. Scope/selection recorded BEFORE review in modal-review/selection.json. All 77 chronological source PNGs frame-000173..frame-000247 (arrival order differs from timestamp order), no omitted in-window frame. Each original inter-frame duration retained, including long holds; last frame retains its full250.057ms original hold instead of truncating at the selection boundary. Total3926.166ms. Crop160,180..1120,500, nearest1/4 to240x80 native pixels; atlas1952x840 with2px gutters, no recoloring/sprite reconstruction. Source SHA256 and exact durations in Assets/Art/UI/TutorialParry/parry.json. At720x240 reference display, Space remains28x11logical pixels. No external-game/reference-match claim.

Directly inspected all77source frames/76pairs on sheets00..06, then every final atlas cel at3x nearest-neighbor on modal-review/final-00..12, including page boundaries and final->first. Both actors remain inside crop; no HUD fragment; original lighting/impact preserved. Source-capture cadence contains held frames (e.g. F47/F57); no synthetic in-between poses or timing smoothing. Last hold correction changes duration only; image unchanged, idle->idle join retains actor positions and scarf cycle. Below are manual per-frame visual observations, not script-derived asset checks. Runtime playback/pause/input/layout/builder verification pending.

| Clip | Frame | Duration ms | Reference | Frame observation | Next transition | Status | Evidence |
|---|---:|---:|---|---|---|---|---|
| Parry modal | 0 | 33.269 | frame-000173.png | Idle, scarf left and blade low | To F1: Idle held, boots fixed | đạt | modal-review/final-00.png |
| Parry modal | 1 | 33.012 | frame-000174.png | Idle held, boots fixed | To F2: Idle scarf lifts, feet fixed | đạt | modal-review/final-00.png |
| Parry modal | 2 | 33.099 | frame-000175.png | Idle scarf lifts, feet fixed | To F3: Idle torso settles | đạt | modal-review/final-00.png |
| Parry modal | 3 | 32.821 | frame-000176.png | Idle torso settles | To F4: Scarf extends left | đạt | modal-review/final-00.png |
| Parry modal | 4 | 32.473 | frame-000177.png | Scarf extends left | To F5: Scarf levels, enemy upright | đạt | modal-review/final-00.png |
| Parry modal | 5 | 32.745 | frame-000178.png | Scarf levels, enemy upright | To F6: Scarf raised left | đạt | modal-review/final-00.png |
| Parry modal | 6 | 32.834 | frame-000179.png | Scarf raised left | To F7: Scarf held high | đạt | modal-review/final-01.png |
| Parry modal | 7 | 32.461 | frame-000181.png | Scarf held high | To F8: Scarf begins lowering | đạt | modal-review/final-01.png |
| Parry modal | 8 | 33.356 | frame-000180.png | Scarf begins lowering | To F9: Scarf slants down | đạt | modal-review/final-01.png |
| Parry modal | 9 | 32.776 | frame-000183.png | Scarf slants down | To F10: Scarf low, legs stable | đạt | modal-review/final-01.png |
| Parry modal | 10 | 33.115 | frame-000182.png | Scarf low, legs stable | To F11: Low scarf hold | đạt | modal-review/final-01.png |
| Parry modal | 11 | 33.135 | frame-000184.png | Low scarf hold | To F12: Scarf low, head unobscured | đạt | modal-review/final-01.png |
| Parry modal | 12 | 32.844 | frame-000185.png | Scarf low, head unobscured | To F13: Scarf low continuation | đạt | modal-review/final-02.png |
| Parry modal | 13 | 32.754 | frame-000186.png | Scarf low continuation | To F14: Scarf reaches low point | đạt | modal-review/final-02.png |
| Parry modal | 14 | 32.909 | frame-000187.png | Scarf reaches low point | To F15: Low scarf, enemy sword upright | đạt | modal-review/final-02.png |
| Parry modal | 15 | 32.853 | frame-000188.png | Low scarf, enemy sword upright | To F16: Scarf rises left | đạt | modal-review/final-02.png |
| Parry modal | 16 | 32.865 | frame-000189.png | Scarf rises left | To F17: Scarf stretches left | đạt | modal-review/final-02.png |
| Parry modal | 17 | 32.749 | frame-000190.png | Scarf stretches left | To F18: Scarf rises, both grounded | đạt | modal-review/final-02.png |
| Parry modal | 18 | 33.220 | frame-000191.png | Scarf rises, both grounded | To F19: Scarf held up | đạt | modal-review/final-03.png |
| Parry modal | 19 | 32.939 | frame-000192.png | Scarf held up | To F20: Warning Space appears above skull | đạt | modal-review/final-03.png |
| Parry modal | 20 | 32.982 | frame-000193.png | Warning Space appears above skull | To F21: Raised cap and white SPACE readable | đạt | modal-review/final-03.png |
| Parry modal | 21 | 33.361 | frame-000194.png | Raised cap and white SPACE readable | To F22: Warning still, scarf lowers | đạt | modal-review/final-03.png |
| Parry modal | 22 | 32.539 | frame-000195.png | Warning still, scarf lowers | To F23: Warning still, arms connected | đạt | modal-review/final-03.png |
| Parry modal | 23 | 33.148 | frame-000196.png | Warning still, arms connected | To F24: Warning still, feet stable | đạt | modal-review/final-03.png |
| Parry modal | 24 | 33.006 | frame-000197.png | Warning still, feet stable | To F25: Warning still, scarf settling | đạt | modal-review/final-04.png |
| Parry modal | 25 | 32.797 | frame-000198.png | Warning still, scarf settling | To F26: Warning still, low scarf | đạt | modal-review/final-04.png |
| Parry modal | 26 | 33.118 | frame-000200.png | Warning still, low scarf | To F27: Warning still, low scarf held | đạt | modal-review/final-04.png |
| Parry modal | 27 | 33.161 | frame-000199.png | Warning still, low scarf held | To F28: Warning still, scarf near waist | đạt | modal-review/final-04.png |
| Parry modal | 28 | 33.538 | frame-000202.png | Warning still, scarf near waist | To F29: Warning still, enemy idle | đạt | modal-review/final-04.png |
| Parry modal | 29 | 32.404 | frame-000201.png | Warning still, enemy idle | To F30: Warning still, low scarf | đạt | modal-review/final-04.png |
| Parry modal | 30 | 33.426 | frame-000203.png | Warning still, low scarf | To F31: Warning still, low scarf held | đạt | modal-review/final-05.png |
| Parry modal | 31 | 33.322 | frame-000204.png | Warning still, low scarf held | To F32: Warning still, scarf begins rising | đạt | modal-review/final-05.png |
| Parry modal | 32 | 32.765 | frame-000206.png | Warning still, scarf begins rising | To F33: Warning still, scarf low left | đạt | modal-review/final-05.png |
| Parry modal | 33 | 32.542 | frame-000205.png | Warning still, scarf low left | To F34: Warning still, scarf raised | đạt | modal-review/final-05.png |
| Parry modal | 34 | 32.519 | frame-000207.png | Warning still, scarf raised | To F35: Warning still, scarf extends | đạt | modal-review/final-05.png |
| Parry modal | 35 | 33.187 | frame-000208.png | Warning still, scarf extends | To F36: Warning still, scarf stretched | đạt | modal-review/final-05.png |
| Parry modal | 36 | 33.205 | frame-000209.png | Warning still, scarf stretched | To F37: Warning still, torso upright | đạt | modal-review/final-06.png |
| Parry modal | 37 | 32.807 | frame-000210.png | Warning still, torso upright | To F38: Warning still before preparation | đạt | modal-review/final-06.png |
| Parry modal | 38 | 32.902 | frame-000211.png | Warning still before preparation | To F39: Enemy draws blade back, player idle | đạt | modal-review/final-06.png |
| Parry modal | 39 | 32.542 | frame-000212.png | Enemy draws blade back, player idle | To F40: Enemy preparation, hilt at waist | đạt | modal-review/final-06.png |
| Parry modal | 40 | 32.413 | frame-000213.png | Enemy preparation, hilt at waist | To F41: Enemy preparing, head and feet stable | đạt | modal-review/final-06.png |
| Parry modal | 41 | 32.466 | frame-000214.png | Enemy preparing, head and feet stable | To F42: Enemy blade across chest, raised cap | đạt | modal-review/final-06.png |
| Parry modal | 42 | 32.967 | frame-000215.png | Enemy blade across chest, raised cap | To F43: Enemy preparation held, depressed cap | đạt | modal-review/final-07.png |
| Parry modal | 43 | 32.767 | frame-000216.png | Enemy preparation held, depressed cap | To F44: Cap released, blade drawn across shoulder | đạt | modal-review/final-07.png |
| Parry modal | 44 | 32.425 | frame-000217.png | Cap released, blade drawn across shoulder | To F45: Cap lights, enemy extends lead leg | đạt | modal-review/final-07.png |
| Parry modal | 45 | 32.940 | frame-000218.png | Cap lights, enemy extends lead leg | To F46: Depressed cap, player still standing | đạt | modal-review/final-07.png |
| Parry modal | 46 | 32.666 | frame-000219.png | Depressed cap, player still standing | To F47: Teal pressed cap, player lifts defense blade | đạt | modal-review/final-07.png |
| Parry modal | 47 | 223.526 | frame-000220.png | Teal pressed cap, player lifts defense blade; original223.526ms hold | To F48: Contact spark at joined blades, +30 visible | đạt | modal-review/final-07.png |
| Parry modal | 48 | 93.628 | frame-000221.png | Contact spark at joined blades, +30 visible | To F49: Contact spark retained, original93.628ms prior hold | đạt | modal-review/final-08.png |
| Parry modal | 49 | 31.365 | frame-000223.png | Contact spark retained, original93.628ms prior hold | To F50: Player low recovery, enemy slash | đạt | modal-review/final-08.png |
| Parry modal | 50 | 31.481 | frame-000222.png | Player low recovery, enemy slash | To F51: Wide enemy slash, player crouch | đạt | modal-review/final-08.png |
| Parry modal | 51 | 155.943 | frame-000224.png | Wide enemy slash, player crouch | To F52: Enemy follow-through, particles left | đạt | modal-review/final-08.png |
| Parry modal | 52 | 31.278 | frame-000227.png | Enemy follow-through, particles left | To F53: Enemy returns blade diagonally | đạt | modal-review/final-08.png |
| Parry modal | 53 | 31.637 | frame-000225.png | Enemy returns blade diagonally | To F54: Enemy lowers sword, player remains low | đạt | modal-review/final-08.png |
| Parry modal | 54 | 93.532 | frame-000226.png | Enemy lowers sword, player remains low | To F55: Enemy upright, player recovery low | đạt | modal-review/final-09.png |
| Parry modal | 55 | 31.271 | frame-000232.png | Enemy upright, player recovery low | To F56: Enemy idle, player low, residual sparks | đạt | modal-review/final-09.png |
| Parry modal | 56 | 31.111 | frame-000229.png | Enemy idle, player low, residual sparks | To F57: Player low hold, original281.402ms duration | đạt | modal-review/final-09.png |
| Parry modal | 57 | 281.402 | frame-000228.png | Player low hold, original281.402ms duration | To F58: Player low, particles fade | đạt | modal-review/final-09.png |
| Parry modal | 58 | 31.125 | frame-000230.png | Player low, particles fade | To F59: Player low recovery endpoint | đạt | modal-review/final-09.png |
| Parry modal | 59 | 94.056 | frame-000231.png | Player low recovery endpoint | To F60: Player lifts torso and rear arm, enemy idle | đạt | modal-review/final-09.png |
| Parry modal | 60 | 31.090 | frame-000235.png | Player lifts torso and rear arm, enemy idle | To F61: Player upright recovery, scarf connected | đạt | modal-review/final-10.png |
| Parry modal | 61 | 31.181 | frame-000233.png | Player upright recovery, scarf connected | To F62: Player settles into idle | đạt | modal-review/final-10.png |
| Parry modal | 62 | 62.595 | frame-000239.png | Player settles into idle | To F63: Idle low scarf, no cue | đạt | modal-review/final-10.png |
| Parry modal | 63 | 62.417 | frame-000234.png | Idle low scarf, no cue | To F64: Idle scarf begins rising | đạt | modal-review/final-10.png |
| Parry modal | 64 | 187.544 | frame-000236.png | Idle scarf begins rising | To F65: Idle scarf extends left | đạt | modal-review/final-10.png |
| Parry modal | 65 | 30.932 | frame-000240.png | Idle scarf extends left | To F66: Idle scarf high left | đạt | modal-review/final-10.png |
| Parry modal | 66 | 62.970 | frame-000237.png | Idle scarf high left | To F67: Idle scarf high, original124.782ms hold | đạt | modal-review/final-11.png |
| Parry modal | 67 | 124.782 | frame-000238.png | Idle scarf high, original124.782ms hold | To F68: Idle scarf lowers | đạt | modal-review/final-11.png |
| Parry modal | 68 | 31.343 | frame-000241.png | Idle scarf lowers | To F69: Idle low scarf | đạt | modal-review/final-11.png |
| Parry modal | 69 | 31.279 | frame-000243.png | Idle low scarf | To F70: Idle low scarf continuation | đạt | modal-review/final-11.png |
| Parry modal | 70 | 31.407 | frame-000242.png | Idle low scarf continuation | To F71: Idle low, original193.607ms hold | đạt | modal-review/final-11.png |
| Parry modal | 71 | 93.607 | frame-000244.png | Idle low, original193.607ms hold | To F72: Idle scarf low, feet fixed | đạt | modal-review/final-11.png |
| Parry modal | 72 | 31.377 | frame-000245.png | Idle scarf low, feet fixed | To F73: Idle scarf starts rising | đạt | modal-review/final-12.png |
| Parry modal | 73 | 62.204 | frame-000246.png | Idle scarf starts rising | To F74: Idle scarf extends left | đạt | modal-review/final-12.png |
| Parry modal | 74 | 62.752 | frame-000249.png | Idle scarf extends left | To F75: Idle scarf high | đạt | modal-review/final-12.png |
| Parry modal | 75 | 31.130 | frame-000248.png | Idle scarf high | To F76: Idle scarf high, both actors at original positions | đạt | modal-review/final-12.png |
| Parry modal | 76 | 250.057 | frame-000247.png | Idle scarf high, both actors at original positions | Idle->idle loop: F0 scarf/feet align; preserve250.057ms terminal hold | đạt | modal-review/final-12.png |

### Implementation verification checkpoint

- First serialize102509 failed strict repeat: newly created buttons acquired skin/feedback only on pass2. Corrected by cloning the existing fully skinned PracticeButton, retaining the existing skin/listener conventions. Serialize102925/inputf6ee6664 passes strict repeated builder. Scoped scene projection changes5existing tutorial records, adds49records, removes0original IDs; MainMenu remains unchanged. Added explicit prefab-modification recording for newly created action labels/bindings before final runtime checks.
- QA103338/input494d9c0472226da9bab14c58f56469e84cba13dff1c022036912bf419f8d61ea:8/8actual focused Unity cases PASS,0failed/skipped,3.5205443s. Covers all77sprite/duration lookups and loop, unscaled pause/resume, Back/reentry reset, duplicate confirmation, reading/restart/menu/fresh encounter, viewed/provider lock plus directly affected live-cue consumers. WebGL/browser acceptance remains pending.
- Final caption-only correction: F42..44 stay Wait for the attack; Tap Space now starts at F45, the first bright cap. PNGs, source sprites and every duration unchanged. Final normal-player generated-record comparison must prove this three-string-only delta before reusing unchanged runtime/clock evidence. No new actor animation was authored; no independent VFX-off/mirrored variant is claimed.

- QA103338 strict repeat-builder and compiled WebGL build succeed. Caller omitted --browser-scenario tutorial; adapter then raised unassigned immutable after Unity exited successfully. Preserved only its separate build-qa output while the next job built production in build-production; verified latest QA result, output mtimes and all33file hashes before/after copy. Local download matches33remote hashes. Recovery receipt is adapter-recovery.json; no other job/source/cache/lock was modified. Two task-only preservation watchers were stopped after checking their exact command fingerprints.
- Native1 inadvertently invoked the previous tutorial-driver entry point;1check then timeout looking for obsolete SpaceExample. Corrected external entry path. Native2 passes20checks, both-size strict layout/caption/arrow/lock assertions, held Enter, complete automatic loop, exact Pause/Resume, Back/reset, restart and menu abandon. It stops at the second deliberate Enter: the driver released/repressed within one Unity frame, so the intended release guard rejected it. Added80ms up-time between separate test key presses; held-Enter and same-frame rejection assertions remain unchanged. No production/runtime change from either driver correction. Native3 continues on the identical immutable player.

- Native3 passes24checks including a real successful Space parry and durable viewed status, then its first Play click after page reload only focuses the newly created canvas. Native4 explicitly focuses an empty canvas corner and waits350ms for menu layout before its real menu click; no acceptance assertion removed. Native4:29/29PASS in39.704763s, zero exceptions/provider reads/writes/blocked external requests, fixture interception retained. Both640x360/1280x720 strict modal bounds, all five text/action bounds, four cards/five arrows and old-example absence pass. Held Enter does not skip step2; Back, exact pause frame/clock, Resume, reentry reset, restart, menu abandonment/replay, keyboard and mouse completion, persisted viewed/automatic bypass, fresh selected-difficulty40HP/180enemyHP/100guard/zero score, clean encounter and actual manual parry pass. All six current screenshots directly inspected (layout/parry/pause); no scene/card/modal leakage. Runtime source is unchanged; final normal caption-only delta/build/render and preview clip remain pending.

### Final preview acceptance

- Implementation fdf9f5e was committed/pushed to micharel09/space-press-cue; fresh fetch verified remote HEAD equality. Main integration remains explicitly deferred. Startup automation consumers use both tutorial actions; the difficulty-audio bootstrap now awaits the existing immediate live encounter rather than an obsolete Fight gate. No audio/gameplay behavior changed; the same fresh-encounter route is covered by native29checks, and changed JavaScript syntax checks pass.
- Final normal110858/input770b5a63a2a70032d3c116074162f53cf8b2bc06112c04bee460820b5b7f13b0 builds successfully, qa:false.33/33downloaded player files match remote SHA256. Five JS/WASM/data payloads exclude ASCII/UTF16 QA/depth markers. Native normal smoke5/5PASS,0exceptions/provider writes; all10menu/preparation/HUD/modal/pause/live/menu images directly inspected. QA globals are absent. No modal in live combat.
- runtime-input-delta.json compares2382Assets/Packages/ProjectSettings inputs: every script/other asset identical to the passing QA snapshot; only source scene and parry.json differ. final-data-delta.json proves every generated scene record, PNG/importer, frame order and duration unchanged apart from the three F42..44caption strings. Reuse the passing8Unity/29native behavior results for unchanged runtime and timing; final normal capture directly verifies corrected captions.
- Final atlas SHA256 675bfdb801027dd6802adef0560cfb7c63d53b039330f5841ec90e091cdb1434; manifest SHA256 3d90faa53a4612cc4367b98883d8ec45bbd89e0dec98b10f96f0bb9293557a8e. All77asset frames,76adjacent pairs and final->first join directly reviewed. Refreshed/reviewed final sheets07/12 after the three-caption and terminal-duration metadata corrections; image pixels unchanged.
- Normal recording selection registered before review in modal-review/normal/selection.json: entire HUD->modal capture,0..4.477539s,222/222original PNGs and221/221pairs directly inspected on all19native-resolution sheets, including sheet boundaries and internal wrapF200->201.56larger sheets and full1280x720 originals retained. Per-frame manual phase/transition evidence: modal-review/normal/frames.csv. All four captions, bright cue/defense/contact/+30/recovery/idle and wrap are visible; no clipping or detached parts observed. Asset detail review remains the77frame table above, distinct from these222rendered frames.
- Final Tutorial-Parry-Modal.mp4:1280x720,4.979s,silent H.264,VFR;222source frames plus1explicit terminal-repeat frame/500ms review hold. All223frames fully decode; terminal duplicate directly inspected. Initial B-frame encoding exposed MP4 duration shorter than final PTS; re-encoded with bf0, preserving source timestamps to1ms and the full terminal hold. Final SHA2564fe12691c92c434da157020ddce8712d732fe6fddc7aa154ae3721fcfc4385f9. Do not confuse encoded-count verification with the direct222source/77asset visual review.
- Existing demo8303 now serves the verified normal player and /Tutorial-Parry-Modal.mp4. Bootstrap PASS: active fixture service worker, no QA globals/errors,1280x720. Four served Unity JS/WASM/data hashes equal the verified player; served worker equals the intentional local fixture overlay. MP4 HTTP200/video-mp4/hash PASS. Earlier standalone demo clips remain available. No main push, public artifact/itch upload or workflow dispatch.

### Retained resources

Keep existing worktree C:/Users/ADMIN/orca/workspaces/TypingHero/space-press-cue and branch micharel09/space-press-cue for user preview before integration. Keep E:/CodexTemp/TypingHero/space-press-cue (normal player, original source/captures, per-frame evidence, clips, isolated drivers/profiles, owned8303host) until review is complete; Stop-Demo.ps1 stops only the verified owned host. Task-created remote results under /data/typinghero-game-loop/results are102509-serialize,102925-serialize,103338-webgl and110858-production (all prefixed20261024T); retain for reproducibility until review, then remove only these owned results after preserving evidence. Source/cache/license and other jobs/resources remain untouched. Both task-only preservation watchers and all test browsers exited. Normal build waited behind the shared pipeline; no other job interrupted. No full suite, extra viewport, performance/soak or new-player usability claim.


### Six-page extension - implementation candidate, 2026-10-25

User expanded the approved modal to all teaching mechanics. Final sequence: HUD (1/6), Type to Attack (2/6), Mistakes & Timer (3/6), Parry with Space (4/6), Break Guard / Slayer (5/6), Score & Combo (6/6). Every mechanic page uses captured frame atlases; only the last action persists the existing viewed key and reloads fresh gameplay. Back, unscaled playback, pause hold/resume, per-entry reset and Enter-release/same-frame gates are implemented. No gameplay tuning, storage key, public gameplay API, GIF decoder or combat simulation was added.

Builder imports17 Point-filtered atlas pages with original frame durations and frame-specific aspect ratios. It binds five lessons to the shared existing modal and preserves existing object names/GUIDs; the old default clip rectangle migrates once to720x270, keeping custom geometry. Tests and directly affected startup harnesses now traverse six pages. **The scene and importer metadata have not yet been regenerated in Unity. This candidate is not ready for play or release.**

Capture source is the prior immutable QA player103338/input494d9c0472226da9bab14c58f56469e84cba13dff1c022036912bf419f8d61ea, whose combat is unchanged here. Actual keyboard/mouse input and chronological CDP PNG capture; no state mutation. Sources selected before review in six-pack.py and six-review/selection.json; source.txt/manifests retain exact source paths/timestamps/hashes. six-type-3 verifies a completed word and enemy HP loss; six-score-2 verifies fifth/sixth words, multiplier expiry and retained score. The failed score portion of six-capture-1 was excluded; only its wrong-letter/timeout, missed-hit and full Slayer segments are used.

| Final clip | Frames directly inspected native +2x | Adjacent transitions inspected | Loop inspected | Duration |
|---|---:|---:|---:|---:|
| typing |67/67|66/66|1/1|2.112776995s|
| mistakes |119/119|118/118|1/1|3.712725163s|
| parry-health |163/163|162/162|1/1|6.597002926s|
| slayer |424/424|423/423|1/1|13.240350962s|
| score |157/157|156/156|1/1|5.611195803s|

All930 final cels and925 adjacencies plus5 loop joins were directly viewed on consecutive12-cel native and nearest2x sheets, including sheet boundaries. Per-frame manual observations, next transition, duration and sheet references: E:/CodexTemp/TypingHero/space-press-cue/six-review/{typing,mistakes,parry-health,slayer,score}/frames.csv; final atlas/manifest hashes and reviewed counts in six-review/selection.json. Slayer first432-frame review found8 leftover recovery cels from the preceding parry: trimmed those only, repacked, then directly reopened all36 native and36 zoom final sheets (424frames), including final-to-first. Loop actors now begin/end idle. Captured score/word/HP/Guard reset at replay is deliberate. Parry-health F76-to-77 and final-to-first explicitly switch between cropped success and full-HUD missed-hit examples; captions identify the separate examples. Black/red Slayer effect obscures interior sprite details, so no independent VFX-off or mirrored review is claimed. Typing includes the preceding may timeout before as succeeds; mistakes/score include real enemy attacks while their HUD behavior is explained. No claim that those secondary events were removed.

Technical check:930/930 atlas cells pixel-match their captured nearest resize or the exact approved77 parry cels; all positive durations, atlas bounds/max2048 and unique metadata GUIDs pass. Receipt: six-review/technical.json. Source durations are retained, including long holds and short terminal cels. These checks supplement direct visual inspection; they do not establish UGUI playback. node --check passes five changed browser harnesses; git diff --check passes. New scoped Unity tests cover all lesson frame/duration lookups, loops, pause/resume, Back/reset, viewed and fresh encounter lifecycle; browser tutorial covers six pages at640x360 and1280x720, held Enter, real parry, locks/replay/restart/persistence. **Neither runtime suite has run on this candidate.**

Blocker: fly-server/quanubutu100.115.189.84 is offline (SSH22 timed out before remote execution; Tailscale last seen5h), and this Windows machine has no Unity Editor. Asked user to reconnect the existing runner; no reply yet. The attempted webgl command ran no Unity tests/build. Do not reuse prior two-page8Unity/29browser passes as evidence for this code. Keep demo8303 and Tutorial-Parry-Modal.mp4 on the previous verified two-page build until replacement acceptance succeeds.

Resume: run serialize first and import only tutorial-scoped generated scene/atlas metadata; verify strict repeat builder and retained tuning. Then scoped Unity/webgl tests, native browser acceptance at both requested sizes, normal build QA exclusion, all-page screenshot review and actual parry after final Start. Produce/review the new playback clip, replace demo only after pass, update affected gameplay memory, commit/push micharel09/space-press-cue and verify remote HEAD. Keep separate from main as requested. No commit/push of this uncompiled candidate yet. Retain existing worktree and E:/CodexTemp/TypingHero/space-press-cue for evidence/resume; no additional long-running process remains from capture.


### Local Unity continuation after user-authorized installation

User authorized installing the pinned Editor/WebGL on this Windows machine while fly-server is powered off. Unity Hub headless installed2022.3.25f1/revision530ae0ba3889 and WebGL successfully under E:/Unity/Hub/Editor/2022.3.25f1. Installer receipt/log: E:/CodexTemp/TypingHero/space-press-cue/unity-install/hub-install.log (All Tasks Completed Successfully). Existing local entitlement resolves; no credential/license was copied or committed. Installation remains for future local work.

Isolated2426-file/80.5MiB LF-normalized input copy and hashes: E:/CodexTemp/TypingHero/space-press-cue/unity-local/source and input.json. Local Library/package cache is private to this copy. First real import found new minimal PNG metadata defaults to Cubemap, leaving zero Sprite subassets despite valid slices. Set Texture2D explicitly and use the repository's existing SpriteDataProviderFactories/name-file-ID convention, retaining existing slice IDs. Earlier force-sync experiment did not solve the root cause and was removed. Native/atlas pixels, frame order/durations unchanged.

Serialize5 completed both builder applications but its first-use scene delta diagnostic failed writing Linux-only /results on Windows. For isolated diagnostics only, redirected that output path to the task root; no repo checker/threshold changed. Serialize6 then passes strict repeat builder, exits0. Log: unity-local/serialize-6.log. Original PixelUIBuild.cs restored before tests/build. A transient deleted diagnostic-script reference in serialize5 triggered recompile; final focused test compilation succeeds with no diagnostic script remaining.

Local PlayMode tests.xml:8/8actual focused cases Passed,0failed/skipped (TutorialTests, ParryCuePresentationTests and two affected cue anchor/rebinding cases). Tests run with real Unity2022.3.25f1 in batch/nographics, no browser pass implied. Imported only three substantive tutorial records1548844259,1548844261,1477722025 from the generated scene, preserving every other original scene record and ID; no new/removed records. Brought back generated metadata for17atlas PNGs and5manifests with original GUIDs retained. All other generated scene/world/menu changes stay in the isolated copy. Receipt: unity-local/scene-projection.json. Source checks and git diff --check pass.

QA WebGL build started on the isolated candidate; log unity-local/webgl.log, owned PID in webgl.pid. Browser acceptance, normal player, replacement demo/review clip and commit/push still pending. Prior offline blocker is resolved for local testing. Keep unity-local source/Library/logs/evidence until acceptance; do not remove installed Editor/WebGL as temporary artifacts.


### Six-page final acceptance on local Windows

Final Texture2D atlas import uses existing Sprite Editor APIs and stable slice IDs. Strict twice-run builder passes in serialize6 and both QA WebGL builds. QA centered WebGL and normal build exit0; all scripts compile. Focused8/8PlayMode cases pass on the same runtime; final change after that is only centered Image pivot/anchored-position geometry. Source projection imports exactly the same3tutorial records (no additions/removals), retaining unrelated authored scene records. No new storage/public gameplay contract.

Native six-accept-3 and final six-accept-4 both pass74/74checks. Final run: actual keyboard/mouse,640x360/1280x720, all6pages, Back/reentry clock reset, exact Pause frame/clock and Resume on every page, held Enter, restart/menu abandon/replay, final-only viewed persistence across reload, fresh40HP/180enemyHP/100Guard/zero score/stats, encounter clone hidden, actual successful Space parry. Zero exceptions/provider reads/provider writes/blocked requests. Early driver1 referenced old two-page entry; driver2 used nonexistent glyph instead of glyphBounds. Corrected drivers without weakening checks. Final native screenshots: six-accept-4/tutorial-{hud,2,3,4,5,6}-{1280,640}.png, all12directly viewed. They found the earlier left-aligned preserveAspect issue; centered pivot correction keeps clip's bounds and dimensions, strict repeat and74checks pass afterward. Asset cels/timing unchanged;930-frame direct review and pixel equality receipts remain valid. Per-frame CSV status updated to distinguish asset visual review from Unity lookup/pause and native page playback evidence.

Normal production qa:false build succeeds. Five JS/WASM/data payloads exclude ASCII/UTF16 QA/depth markers (unity-local/normal-exclusion.json). six-normal-1 passes5/5normal smoke checks, no exceptions/provider writes. All11normal menu/HUD/five lesson/Pause/gameplay/return-menu images directly inspected: centered readable clips and captions, clean battle, no modal leakage, QA globals absent. This complements the QA74behavior proof; normal smoke alone does not assert every gameplay value.

Tutorial-Mechanics.mp4 is the complete normal HUD-to-five-modal-page recording:862chronological CDP PNGs,36.283360s source interval plus500ms terminal hold, VFR1280x720 silentH.264;863encoded frames fully decode. SHA256302d38fd07f329d476324f7cb6e931751b290e1fb0b228120db1cffaef08ba2d. Full source/PTS/encode receipt: six-review/clip.json; original capture six-normal-1/modal-capture. This video is a viewing aid;930atlas frames are the directly inspected final sprite sequences, distinct from the862screen-capture frame count. No independent frame-by-frame visual pass is claimed for the encoded UI recording.

Demo8303 now serves unity-local/production/player and /Tutorial-Mechanics.mp4. Four served player JS/WASM/data hashes equal verified build; MP4 HTTP200/type/hash pass (six-review/served.json). Existing earlier MP4s remain available. Keep installed Editor/WebGL at E:/Unity/Hub/Editor/2022.3.25f1 and the isolated unity-local source/Library/evidence for local work/reproducibility. All test browsers and owned Unity build/test processes exited; only the existing task-owned demo8303host remains. Its Stop-Demo.ps1 verifies the exact owned Python command before stopping. No main integration; retain trial branch/worktree for preview.

### Beat guidance follow-up (2026-10-25)

This checkpoint supersedes the earlier six-page assets and intermediate crop candidates above. Scope remains the trial branch `micharel09/space-press-cue`, separate from main. The dirty primary checkout is untouched. Evidence root: `E:/CodexTemp/TypingHero/space-press-cue` (paths below are relative to it).

Implemented HUD → Type → Mistakes/Timer → Parry → Slayer → Score/Combo. Five fixed rules remain below720x270 clips; DEMO · AUTO badge, one phase label, one focus outline and one arrow are synchronized to the same unscaled frame clock. Pause holds that clock and every guide; entering another lesson resets all to frame0. Modal816x480 at72,30; original skin/font/English, HUD four cards/five arrows, Enter release gate, Back/restart/menu/viewed/fresh encounter contracts remain. No public gameplay API, storage key, dependency, animation/controller or combat tuning change.

Builder preserves existing object/asset/slice GUIDs and listeners; migrates only matching old default geometry and preserves authored tuning. SampleScene projection changes only10 existing tutorial records and adds50 tutorial-owned records; the unrelated TMP generated sort change is excluded. Importer is Texture2D, Point, no mipmaps/lossy compression, max2048, gutter2. Seven used atlas pages, unused pre-existing pages retained.

Real capture sources and decisions:

- Typing: `beat-type-11/capture`, fresh part → of, actual10damage without a preceding timeout or received hit; existing Very Easy selector permits clean preparation/recovery under capture load.
- Mistakes: original `six-capture-1/capture`, correct prefix then error/reset, running timer and timeout/new word without score.
- Parry: all77 approved `parry.json` success cels and durations retained exactly; missed example comes from the original capture, same240x90 canvas/scale/actor positions, with HP strip from that same frame.
- Slayer: `beat-capture-tail-3/capture`, selected1792902572.125..1792902587.145. Real final Guard-breaking parry and eight letters spaced about1.08s through full Slayer. Enemy survives at161HP after exit; completed normal word deals10, leaving151HP. Complete enemy counter/hurt/recovery and >500ms idle retained. Held attack poses are source controller behavior (AttackS1/S2/S3 have no outgoing transition; subsequent characters crossfade attacks, exit restores normal); no animation/tuning edit. Fixed arena50,25..250,130 uniformly nearest-resized160x84 at80,6; masks remove irrelevant upper HUD outside actors, with exact coordinates/condition in manifest; word/Guard strips retain original same-frame pixels.
- Score: `beat-score-6/capture`, six actual completions0→10→20→30→40→52→65, fifth unlocks multiplier, sixth increases it; expiration retains65. Full enemy attack/recovery and idle preserved.

Rejected attempts remain evidence, not acceptance: earlier timeout/under-load typing captures; old315-frame Slayer ending during hurt; failed tail capture1/2; narrow450-frame crop clipping right weapon/head; widened upper crop exposing irrelevant HUD; stepped head-extension crop causing a background seam. Current rectangular scene crop supersedes those. `beat-pack-slayer.py` packs only final Slayer; other four reviewed clips remain unchanged. Source paths, hashes, timestamps, selected events, crop/composition and durations are in five committed manifests and `beat-review/selection.json`; approved success provenance maps to `native-direct-final/compositor`.

Direct final output review:

| Clip | Frames inspected/total | Adjacent pairs inspected/total | Loop joins | Duration |
|---|---:|---:|---:|---:|
| typing | 51/51 | 50/50 | 1/1 | 2.449949026s |
| mistakes | 119/119 | 118/118 | 1/1 | 3.712725163s |
| parry-health | 163/163 | 162/162 | 1/1 | 6.597002926s |
| slayer | 450/450 | 449/449 | 1/1 | 14.986407042s |
| score | 144/144 | 143/143 | 1/1 | 10.872750998s |

All927 final frames,922 consecutive transitions and5 loop joins directly inspected at actual480x180 UGUI playing size and nearest2x. No sampled-keypose or ordinary-playback substitution. Labels follow recorded events; arrows and outlines avoid word/HUD values/Space/action; whole characters/weapons retained. Slayer preview inspection is validated by RGB byte equality of all450 frames against actual final Unity UGUI exports (`beat-review/final-preview-equality.json`); other four final export PNG hashes exactly match their already inspected render. Source red/black Slayer VFX and authored held poses are preserved, not independently VFX-off reviewed. Demo loops deliberately reset captured word/HUD state after recovered idle.

Per-frame observations, transition notes, original durations/source timestamps/hashes: `beat-review/{typing,mistakes,parry-health,slayer,score}/frames.csv`. Actual UGUI exports: `beat-review/ugui-final/{0..4}`; receipt/hashes: `ugui-receipt.json`; manual observations: `manual-notes.json`; pixel composition/77-reference/atlas checks: `technical.json`. These technical checks supplement the visual review.

Final focused Unity PlayMode:5/5 PASS (`beat-tests.xml`, `beat-tests.log`), including every frame midpoint/exact boundary/pre-boundary/loop, fixed rules, nonraycast guides, pause/resume/reset and lifecycle/storage/input locks. Earlier stalled run is retained separately as `beat-tests-stalled.log`; it is not a pass. Isolated scenes restored from task checkout and builder prepared before successful final run. Final build input snapshot2435files SHA256 `97215012a91932bd5b443e8d5a1f0ddfc63475b370fa137469d8a7d0f9766b11` (`beat-review/build-inputs.json`). Two isolated test-init artifacts remain outside the build list for failure provenance.

Final QA build PASS (`beat-qa.log`), including strict twice-run builder and unchanged atlas metadata (`beat-review/repeat-atlas.json`). Final native browser94/94 PASS (`beat-native-accepted/browser.json`): six-page click/Enter/held-release gate, Back/reset, Pause/Resume, restart/menu/viewed, typing/score/provider locks, clean battle and real Space parry. Zero exceptions/provider reads/writes. All13 screenshots directly inspected (six pages at640x360/1280x720 plus real parry), with hashes in `beat-review/viewport-review.json`. Normal WebGL build PASS (`beat-normal.log`, same frozen input snapshot); `game-loop-build.json` is qa:false. Five JS/WASM/data payloads exclude ASCII/UTF16 QA markers (`beat-review/normal-exclusion.json`). Normal native7/7 PASS (`beat-normal-final/browser.json`), zero exceptions/provider reads/writes; all11 normal screenshots inspected (`beat-review/normal-pages.png`). First observer attempt in `beat-normal-accepted` sent no Space because it expected a cream face: actual ready face uses gray176/168/158 or128/120/115. Corrected only artifact pixel-observer thresholds, reran unchanged normal player into a fresh folder; no gameplay changes or waived assertion.

Delivered `Tutorial-Mechanics-Beats.mp4`:1280x720, silent H264 VFR,51.164s,1473 chronological original capture frames plus one terminal hold frame. Explicit2.5s HUD intro still makes page1 readable; subsequent capture timing unchanged, terminal hold500ms. Full frame decode and1474 strictly increasing PTS pass. SHA256 `1e6648b2e154a33ab6cf1753b44ce2f6b206d9c5c5b4ca44254a6991766db434`. Receipt: `beat-review/clip.json`, original `beat-normal-final/modal-capture/frames.json`, export manifest `beat-review/normal.ffconcat`. The video contains full UGUI guidance across all six pages, Pause/Resume then Start Battle and real Space parry. No independent all1474-encoded-frame visual-review claim: final927 lesson cels/guidance were reviewed separately above.

Actual normal parry video segment was registered before direct review: source frames1378..1472, timestamps1792905493.679923..1792905496.778286, complete Start Battle scene transition, fresh idle/preparation/cue, Space, contact/result and recovery/idle.95/95 frames and94/94 transitions directly inspected at640x360 and nearest2x. F1425 cue fills;1428 hero guard/teal cue;1431 spark/+30/score0->30/Guard falls with full heroHP;1457..1460 recover;1461..1472 idle. Source hashes, timestamps, selection reason and per-frame notes: `beat-review/normal-parry/{selection.json,frames.csv}`. No mirrored/VFX-off review claimed.

Demo http://127.0.0.1:8303/ now serves `beat-production/player`; video at http://127.0.0.1:8303/Tutorial-Mechanics-Beats.mp4.31 served player files match local hashes, qa:false verified, MP4 GET/HEAD/hash pass (`beat-review/demo-delivery.json`). Custom bootstrap/service worker and styled index retain the existing local fixture behavior. Old demos/video files remain for provenance.

Trial commit/push verification is recorded after delivery in `beat-review/git-delivery.json` (remote branch HEAD must equal local HEAD). Retain `C:/Users/ADMIN/orca/workspaces/TypingHero/space-press-cue`, branch `micharel09/space-press-cue`, installed pinned Unity and `E:/CodexTemp/TypingHero/space-press-cue` for user review/reproducibility. Keep only task demo8303 running; `Stop-Demo.ps1` verifies its exact owned command. Cleanup worktree/local branch after user review and remote integration, evidence/demo when no longer needed. Do not delete pre-existing resources or merge main in this task.
