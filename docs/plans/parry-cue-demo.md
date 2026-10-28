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

### Whole gameplay frame revision (2026-10-26; in progress)

This revision supersedes the cropped/composited lesson assets above. The user
accidentally deleted the original worktree and requested a replacement. Continue
from remote commit2d589cb on `micharel09/tutorial-full-gameplay` at
`C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-full-gameplay`; keep it separate
from main. The dirty primary checkout is untouched. The earlier E: read failure
has resolved; capture/backups remain under the existing artifact root.

Approved geometry: panel928x516 at16,12, whole gameplay640x360 at160,64 on the
960x540 reference. Title/page above, beat label outside below gameplay, fixed
rule and Back/Next/Start Battle in the footer. One outline and short arrow remain
inside gameplay. No crop, HUD composition, masking, debug text, gameplay API,
dependency, save key, combat tuning or animation change.

Every source frame is resized once to320x180 nearest-neighbor, with original
chronological duration and holds. Atlases are six columns, at most11rows, two
pixel gutters, max2048, Point. Sources/events/hashes are in the five JSON manifests
and `full-review/selection.json`; reproducible packer is
`tools/game_loop/tutorial_pack.py`. Current candidates: typing106frames from
full-capture-21, mistakes160 from full-capture-3, parry-health303 from
full-capture-2, Slayer745 from full-capture-20, score681 from full-capture-23.
All captures use real keyboard/mouse on the clean normal player. Existing Very
Easy selection is used for the final score recording; no state mutation.

Review found and corrected premature typing-result guidance, two Pause frames
at the start of mistakes, delayed timeout guidance, and timer-outline overlap.
The earlier typing candidate began during enemy recovery; the earlier score
candidate began during hurt. They are superseded by21/23. Slayer19 failed actual
post-mode HP reduction;20 shows eight actual letter hits, complete mode exit,
normal word all with score146->156/enemy HP loss, complete counters/recovery and
terminal idle. Failed22 stopped after three words due external OCR;23 completes
those/public/answer/find/your/study, score0/10/20/30/40/52/65 and combo expiration.

Direct atlas review: typing106/106, mistakes160/160, parry303/303,
Slayer745/745, score681/681:1995cels,1990adjacent pairs and5idle loop joins.
All69numbered native320x180 sheets were directly inspected. Original long held
poses, transient source partial glyphs and
score pulse/flicker are retained visibly; no mask hides them. Per-frame notes
and pending UGUI/playback status are in `full-review/atlas-review/*-frames.csv`
and `full-review/manual-atlas-notes.json`. Old cropped927-frame review does not
certify this revision. Independent whole-source resize equality was rechecked
after the final score replacement: all1995frames match their full source after
the single permitted resize. Technical pixel equality is distinct from the
manual visual review; its receipt intentionally records visualReviewed:0.

The user explicitly requires all further Unity execution on Ubuntu. The owned
Windows prepare process25480 was stopped; no other task's Unity process was
stopped. Ubuntu pinned Unity2022.3.25f1 serialize result
`/data/typinghero-game-loop/results/20261026T062132-serialize` passes strict
twice-run builder. First061726 failed protected font byte equality because of
Windows CRLF; the external snapshot helper now canonicalizes LF while preserving
repository font contents. No check was relaxed. First focused run063923 passed
four lifecycle/provider cases but failed the all-lesson case because a fixed
350ms Resume wait elapsed before Linux rendered another frame. The test now
waits, bounded to3s, for actual clock advance; it retains the same assertion.
All-lesson reruns065753,072810 and073902 PASS. The last exports all1995UGUIframes
and checks every frame midpoint, both sides of every boundary, loop lookup,
Pause clock/frame hold, Resume and Back reset.

Direct UGUI review corrected invisible Space targets, result labels pointing to
sliding empty words, and a Slayer outline crossing wide sword trails. Final
targets use visible cue/word/timer/HP/Guard/score, with Slayer letter hits pointing
at score. All37final guide/pose variants were directly inspected on five sheets
in `full-review/ugui-events-final4`; final all-frame export hashes are in
`full-ubuntu-quick4/ugui-hashes.json`. Initial and intermediate exports are not
final guidance acceptance. Premature e2ddfb54 QA/normal jobs were intentionally
cancelled; neither counts as a passing build.

Final generated projection changes15existing tutorial records, adds/removes0,
preserves all1231scene IDs, both persistent button listeners,18existing atlas
GUIDs and1018existing named frame IDs. No unrelated generated scene edits were
copied. Receipts: `full-review/final-scene-audit.json` and
`full-review/guid-before-builder.json`.

Final QA Ubuntu build074535 PASS, input
8721070249f1634801da47a7e5ddbbc3a8a9e584af37aab222aaf09d9972c183.
Strict repeat builder PASS again; all82tutorial outputs and the full generated
SampleScene are byte-equal to quick4. Immutable33-file QA player hashes were
verified after transfer. Native browser104/104PASS at640x360/1280x720: all six
pages, glyph bounds, input/score locks, Back/reset, exact Pause frame/clock hold,
Enter release gate, restart/menu, viewed persistence/replay, clean40/180/100
battle and actual successful Space parry. All13screenshots directly inspected;
no exceptions/provider reads/writes. Evidence: `full-native-final/browser.json`,
`full-review/qa-layout`, `full-review/final-repeat-tutorial.json`.

The first browser invocation stopped before launch because another task's old
Playwright directory was gone. Existing cached Playwright1.62.1 was restored
offline under this task's `browser-tools`, using native Edge; no project package
or game change. The corrected104-check run is the acceptance result.

Normal Ubuntu build075759 PASS with the same immutable input. The33-file normal
player transfer matches its hashes, qa:false; all five JS/WASM/data payloads
exclude typingHeroQA/typingHeroCombatFrame in ASCII and UTF-16LE. Final normal
native6/6PASS,0exceptions/provider reads/writes; all11screenshots inspected.
Real keyboard input plays all six pages and complete five clip cycles, Pause /
Resume, final Start Battle, successful Space parry and return to menu.

The actual normal parry segment is registered before review at source frames
2121..2221, timestamps1793002183.345681..1793002186.786824:101/101frames and
100/100adjacent pairs directly inspected on seven sheets. Idle preparation,
enemy windup, real Space/blue cue, contact/+30/Guard loss, follow-through, full
recovery and terminal idle remain. Later natural word timeout and next warning
remain visible. Per-frame evidence: `full-review/normal-parry/frames.csv` and
`scope.json`; earlier95-frame normal review is not reused.

`Tutorial-Full-Gameplay.mp4`:73.902s,2222chronological full1280x720 source frames
plus explicit500ms terminal hold,2223encoded frames, silent H264 VFR, no crop or
composition. All frames decode and PTS strictly increase. The default null-muxer
rounding warnings were checked again with passthrough output/microsecond time
base: no decode errors. SHA256:
c0906507f91ef8a4345a1112beac8172b23746d0e35a55f2c20f41d35d000ec0.
Video/frame receipts: `full-review/clip.json`, `full-normal-final/modal-capture`.
Full atlas visual review, selected UGUI variants, browser playback and video
decode are distinct evidence; this does not claim direct inspection of every
encoded walkthrough frame or independent new character-animation acceptance.

Demo http://127.0.0.1:8303/ now serves `full-production/player`; video at
/Tutorial-Full-Gameplay.mp4. All31unmodified served files match the immutable
normal player. Index retains existing fullscreen CSS/title; ServiceWorker is the
existing local offline Firebase fixture, separately verified. Browser bootstrap
loads the new normal menu with active worker, no QA globals or exceptions.
Video GET/HEAD/type/hash checked. `Start-Demo.ps1` can restart the task-owned host;
only that host remains running. Receipt: `full-review/demo-http.json` and
`demo-browser.json`; final menu screenshot directly inspected.

Retain the worktree/branch `micharel09/tutorial-full-gameplay` and artifact root
`E:/CodexTemp/TypingHero/space-press-cue` for user review, including source PNGs,
manifests, per-frame notes, Ubuntu results, player hashes, failures and final
video/demo. Remove these only after review and preservation of needed evidence.
Main integration remains explicitly deferred. No combat, animation, dependency,
storage or gameplay API changes; final diff whitespace check is scoped to task
files. Consolidated receipt: `full-review/final-review.json`.


## Tutorial atlas streaming (2026-10-26; implementation in progress)

Scope: continue `micharel09/tutorial-full-gameplay` from `7c208a2e`, review only;
no main integration. Preserve 1,995 PNG cels, Sprite IDs/GUIDs, durations and six-page
layout. Target at most two tutorial atlas slots (including pending requests),
<=31 MiB live RGBA textures, and zero after cleanup on HUD/menu/normal encounters.
Existing demo8303/full-production and `Tutorial-Full-Gameplay.mp4` remain unchanged.

Implementation: per-atlas Resources descriptors, scene index/path mappings without
sprite preview, overlay-owned async cache with session invalidation and completion
cleanup, held clock/frame while unavailable, loading/error caption. Builder includes
atlas metadata and resource descriptors in strict repeat checks. Focused tests cover
frame lookup, unload/reload, obsolete completion, pause/loading, rapid navigation and
lifecycle. Ubuntu test/build and final WebGL verification are still pending.

Baseline evidence: `E:/CodexTemp/TypingHero/space-press-cue/atlas-baseline2/browser.json`
and `atlas-baseline3/browser.json`, normal immutable `full-production/player`, Edge
156.0.4314.42 native WebGL. Instrumented actual texStorage2D/texImage2D/deleteTexture
for tutorial PNG dimensions: menu0; HUD33; typing33, 479,312,640 bytes =457.108154 MiB.
Baseline2 typing screenshot directly inspected. Initial `atlas-baseline` stayed in
menu preparation and is not memory acceptance. Source dimensions independently
predict maximum two-atlas RGBA30.019043 MiB; this is not an optimized runtime result.

Task artifacts use existing evidence root with `atlas-*` prefixes. Current first
Ubuntu run: `/data/typinghero-game-loop/results/20261026T084415-quick`; its snapshot
predates additional lifecycle tests and fixture checks, so a final-source run is
required. No passing claim for this in-progress revision.


Atlas verification progress: first Ubuntu quick084415 strict builder PASS; imported
33 resource pages and projected exactly two tutorial scene records, no other scene
record changes. `atlas-review/preservation.json` verifies all tutorial PNG/JSON/meta
against7c208a2e (text newline normalization only). `atlas-review/mapping.json` verifies
all1,995 sprite fileID/GUID references in order plus exact serialized durations,
captions/guidance. Scene contains zero tutorial PNG GUIDs.

Quick084415 runtime3/5, not acceptance: async frame assertions raced automatic
playback; restart test assumed the replacement controller existed after300ms.
Tests now freeze playback for exhaustive lookup and wait for controller replacement.
Fixture run090609/input398caa7f: current fixture/two full builder passes PASS;
fresh construction failed on dangling DungeonSessionBindings roots after deleting
tutorial objects, before testing fresh builder behavior. Fixture now removes the
deleted roots/links; production dungeon code unchanged. Preserve current result,
rerun fresh/tuned separately. Logs: `atlas-fixtures-first`; first XML: `atlas-quick1`.
Latest tests additionally cover missing-resource UI/Back/Pause, held UI/clock during
a distant-page request, new cache overlapping a retired request, destruction and
empty encounter clone. Final runtime and browser acceptance remain pending.


Runner correction: the inherited `full-ubuntu-runner.py` tar override omitted mtimes
(default0). Quick093016 had latest source/inputa08c67de but stale compiled tests
(7 discovered, missing the eighth present in source); its runtime results are not
final-source evidence. Atlas wrappers now preserve each source mtime and retain
immutable archives under `atlas-inputs/<content-sha>.tar.gz`.

Correctly rebuilt quick094526/inputef65e369 discovered8 tests:6PASS/2FAIL. Cache native
residency/unload/fresh reload, overlapping retired/new cache request, rapid navigation,
pause/loading/disable/destroy, missing-resource UI/Back, viewed storage and provider
lock checks passed. Exhaustive lookup stopped on parry-health boundary78; fixture
now yields only for actual pending loads instead of a frame for every already-ready
lookup, retaining exact boundary assertions. Lifecycle reached clean battle but its
arbitrary<=1s clock assertion saw1.0947s on the slow editor; replacement compares the
clock with elapsed time since Start Battle and verifies advancement. Final-source
rerun is required for those changed assertions and the refined pending-clock logic.

Clock refinement: retain the first unavailable candidate time and retry it verbatim;
even the final waiting interval is excluded. A focused test injects10s stalled ticks
and checks that recovery uses the original10ms candidate. No global asset sweep,
static cache, gameplay clock or combat change.

Scoped final-source Ubuntu verification:

- `20261026T095946-quick`, input31756a0820b4068ef9abd385efffd4255e4c8a258b4999363d054edf815d4b21:
  four changed tests pass (40.46s), covering all1,995 lookups, exact boundaries,
  28 atlas transitions/five loops, pending-clock10s injection, pause/rapid navigation,
  destroy/disable cleanup, final-only completion, clean battle and encounter clone.
  `atlas-review/runtime-source.json` confirms the four runtime files match this input.
- `20261026T101732-quick`, input18d5570dfe112e2072b34d958070b1ebafe809c2bcc56302d65c7d813c042b47:
  the extended error test passes (16.09s), including Pause/Back/reentry/Menu while
  Unseen. Unchanged native residency/reload/overlap, reading pause, legacy viewed
  and provider-lock tests retain passing094526 results; eight unique tests covered.
- Current fixture090609 and fresh/tuned fixtures100251 each pass two complete
  builders. Tuned geometry and persistent listener survive. Exported117 resource,
  atlas metadata/manifest and scene files equal current source after whitespace
  normalization; resource GUIDs and all frame data remain stable.

Receipts: `atlas-quick3`, `atlas-quick4`, `atlas-quick5`, `atlas-fixtures-first`,
`atlas-fixtures-final` under the evidence root above. Earlier failed/stale runs are
retained for provenance, superseded only for the checks explicitly listed here.

### Atlas streaming acceptance (2026-10-26)

Normal Ubuntu WebGL result `20261026T102719-production` uses input18d5570d above.
All build inputs match the local implementation. The33 downloaded player files
match remote hashes; `game-loop-build.json` is qa:false, and JS/WASM/data exclude
the QA globals in ASCII/UTF16. Receipts: `atlas-production/verified.json` and
`atlas-review/final-source-tests.json`. Unity ran only on Ubuntu2022.3.25f1.

Native Edge156.0.4314.42, Windows, ANGLE/D3D11 NVIDIA GTX1650Ti:

| Measurement | Before7c208a2e | Streaming normal |
| --- | --- | --- |
| Tutorial peak live GPU atlases |33 |2 |
| Tutorial RGBA allocation |457.108154 MiB |30.019043 MiB |
| HUD1/6 |33 |0 |
| Menu and completed cleanup |0 at initial menu |0 |

Measured actual texStorage2D/texImage2D/deleteTexture, including transient peaks;
dimension classification independently matches exactly33 tutorial PNGs and no
other project PNG. These are tutorial texture allocations, not total browser/RAM
or download size. Normal1280x720 passes33 checks;640x360 passes17. All six pages
at both sizes were directly inspected. Held Enter stays on page2; Back, Pause,
replay, viewed Play and Start Battle work. Actual Space input lowers live Guard
and earns30points. HUD, Back-to-HUD, fresh battle, returned menu, viewed encounter
clone, battle restart, replay HUD, tutorial restart and exit-loading menu all
reach zero tutorial textures. No console/runtime exception or exposed QA bridge.

Visual boundary evidence: `atlas-review/boundaries/review.json`,33 contact sheets,
132/132 selected compositor frames and99/99 consecutive pairs directly inspected:
28 internal atlas changes plus five end-to-start loops. Complete gameplay remains
visible, captions/focus follow the displayed beat, and no blank or wrong-atlas
image was found. Four consecutive captures surround each boundary; timestamps
retain original holds and compositor gaps, sorted by timestamp rather than CDP
arrival order. Matching indices only locate the sheets; they do not replace direct
inspection. This is not an FPS guarantee or a claim that CDP captures every engine
tick. Existing1,995-cel review is reused because PNG pixels, SpriteRects/IDs and
timing are unchanged; offline tests resolve every frame and exact boundary.

New review demo: <http://127.0.0.1:8304/>.
Video: <http://127.0.0.1:8304/Tutorial-Atlas-Streaming.mp4>.
The normal-input capture contains5,501 frames; VFR H264 preserves original timing
and the final hold (5,502 encoded frames). Full decode and monotonic PTS pass;
SHA256 `313fb530ec7eaec8e0e2d8d5cd4b2a4fed5524aee1a0411d47522ad162396134`.
HTTP video hash/MIME, service-worker bootstrap, menu screenshot and QA absence pass
in `atlas-review/clip.json`, `demo-http.json`, `demo-browser.json`. Browser fixtures
block production score writes. Existing8303 player/video remain untouched.

Retain the review worktree `C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-full-gameplay`,
branch `micharel09/tutorial-full-gameplay`, evidence root and8304 host for user review.
Host PID is recorded in `atlas-host.pid`; restart with `Start-Atlas-Demo.ps1` in the
evidence root. Stop/remove only these task resources after review or integration;
do not remove the old demo before acceptance. No main merge is authorized for this
handoff. Implementation/testing are complete; user visual acceptance remains open.

### Playback hot-path follow-up (2026-10-26; review candidate)

Continue `micharel09/tutorial-full-gameplay` from `ce9b87a5` in the same clean
review worktree. Keep main unmerged and retain demos8303/8304. Candidate demo
target8305; do not replace an accepted player before candidate verification.

Implementation: `Get` protects the displayed atlas until the Image references its
destination; `CommitDisplay` then retires the old page and starts next prefetch.
The two-entry dictionary and reusable key buffer avoid temporary collections.
Request completion closures live in `BeginLoad`, outside steady `Pump` calls.
Unchanged frames return without Image/guidance/visibility writes; loading/error
captions change only when text changes. Modal playback no longer forces Canvas
rebuilds; HUD1/6 keeps its geometry-dependent rebuild. No clock-policy change yet:
only confirmed resource-work contamination authorizes the proposed clock fix.

QA-only trace uses16,384 struct records, monotonic timestamps, requested/accepted
clock, displayed frame/atlas, waiting/session/slot state and resource call events.
`Canvas.willRenderCanvases` records at most once per Unity frame. The observation
bridge exports after measurement and synchronizes Unity/browser timestamps;
overflow invalidates evidence. WebGL observer records upload/delete start/end
and fixed-buffer rAF cadence. Capture acknowledges before asynchronous file IO;
backlog/IO/ack failure invalidates capture rather than silently dropping images.

Evidence root remains `E:/CodexTemp/TypingHero/space-press-cue`:

- `streaming-review/preservation.json`:149 tracked tutorial PNG/manifest/importer/
  descriptor/scene files match ce9b87a5 (text checkout newlines normalized).
  Prior cel and builder-fixture evidence remains scoped to those unchanged files.
- `streaming-baseline/browser.json`:10/10 checks PASS on the old normal player,
  without screencast or synchronous PNG writes. Peak2 atlases/30.019043MiB,
  battle cleanup0; no JavaScript/Unity exceptions. Per-clip rAF/event traces in
  the same folder: Slayer max rAF20.07ms, Score20.40ms; largest wrapped WebGL
  upload/delete call0.07ms. The historical133–213ms capture gaps did not recur
  in this measurement. This suggests capture overhead but does not establish
  Unity playback continuity or explain every historical gap.
- Added focused tests for stable cache/render/tick allocation, Image dirty
  callbacks, displayed-atlas protection and waiting-tick dirty suppression.
  Supplemental Windows Unity2022.3.25f1: `streaming-local/tests.xml` passes8/9;
  all1,995 lookups/28 boundaries/five loops and existing tutorial cases pass.
  The new test passes its0-byte cache/render/tick and0-dirty assertions, then
  fails an incorrect C# null-after-UnloadAsset assertion. Corrected that fixture
  to inspect native residency by instance ID, matching the established cache
  lifecycle test. `streaming-local/stable.xml` passes1/1 on the same runtime:
  native old-sprite residency ends after Image handoff. Combined9 distinct
  cases pass across these two runs, not a single9/9 run. Input manifests and
  failed receipts are retained. First Ubuntu attempt timed out waiting for the
  shared runner lock without executing Unity. Longer-wait scoped test requests,
  QA and normal builds are queued; other jobs are preserved. Windows QA build
  is supplemental early trace preparation and does not replace Ubuntu acceptance.
- First queued Ubuntu input8bcebfae (`20261026T152903-quick`) passes8/9, with
  the new test failing because the prefetched destination is null. This is the
  intermediate `Get` implementation that evicted following when held==current,
  already corrected before final build requests. Retained in `streaming-ubuntu-first`;
  do not cite it as final runtime evidence. Current QA input728fddcf is building.
- Windows QA strict repeat builder/build PASS in `streaming-local/qa.log`.
  `streaming-local-trace/browser.json`:51/51 PASS, explicit coverage of all33
  atlas boundary/loop pairs, no buffer overflow, GPU bound and battle cleanup0.
  Each selected priority transition has adjacent render gap17.965–19.310ms:
  Slayer2→3=19.310,4→5=18.745,9→10=17.965; Score9→10=18.280,10→0=18.315.
  Synchronous resource calls across the measured clips peak0.440ms. Clip-only
  tick gaps peak32.455ms; export/previous-lesson records are excluded by step/session
  when computing these metrics, not silently discarded. Raw records remain.
  No133–213ms gap or resource-caused clock jump was observed; keep existing clock.
- `streaming-local-integration/browser.json`:104/104 real-input checks PASS,
  six pages at1280x720/640x360, Enter-release/Back/Pause/Resume/Start, clean battle,
  actual parry, viewed/replay/restart/navigation and no exceptions/provider IO.
  Directly inspected all13 page/parry screenshots; hashes/manual observations
  in `streaming-review/local-ui-review.json`. QA-only F6 overlay is expected.

Current accepted evidence:

- Ubuntu `20261026T160709-quick`, inputd0f5f956:9/9 tutorial tests PASS,
  0failed/skipped,29.4497702s. All1,995 lookups/28 boundaries/five loops,
 0-byte warmed cache/render/tick loops,0 Image dirty callbacks, safe native
  residency handoff and loading/navigation/error/lifecycle checks pass.
- Ubuntu QA `20261026T153705-webgl`, input728fddcf: strict repeat builder and
  build PASS. Initial invocation omitted the tutorial scenario and accidentally
  entered the default browser harness, which failed finding NameInput; that whole
  profile is NOT a pass. Preserved the still-unchanged build-qa output before any
  later QA build, downloaded and verified all33 file hashes in `streaming-ubuntu-qa`.
  Correct scoped native trace `streaming-ubuntu-trace/browser.json` passes51/51,
  with all33 explicit pairs, fixed buffers valid, two slots and battle cleanup0.
  Resource calls peak0.725ms; five priority render gaps16.875?22.965ms.
- Windows normal capture `streaming-normal/browser.json`:34/34 PASS,
  Edge156/D3D11; actual tutorial texture peak2/30.019043MiB. HUD, Back, battle,
  menu, viewed encounter clone, restart/replay and exit-loading cleanup all0.
  Real Space parry visibly reduces Guard; no QA globals/exceptions/provider IO.
  Capture validity: no backlog, disk or ack errors. `streaming-driver/streaming-normal.cjs`
  acknowledges before async writes; the tracked observer matches its deployed copy.
- Initial normal exclusion found the empty TutorialPlaybackTrace type name in
  metadata, despite no bridge/buffer. Wrapped the entire type in the QA define;
  `streaming-local/normal-final-exclusion.json` verifies all33 final normal files
  and excludes five QA/type markers in every JS/WASM/data payload (ASCII/UTF16).
  Post-packaging normal `streaming-normal-final-small` passes17/17; six640x360
  page images directly inspected. This packaging-only change does not alter
  active normal playback or serialized assets, so existing capture/input coverage
  remains applicable. One later QA-only provenance correction records the captured
  request epoch on completion; stable-session measured traces are unaffected.
  Final-source warmed allocation/dirty/handoff check passes again in
  `streaming-local/stable-final.xml` (1/1,3.8508793s); source hashes/variance in
  `streaming-review/final-source.json`.
- Complete registered33-segment visual review:528/528 compositor images,
  495/495 consecutive pairs directly inspected, including all held/repeated frames.
  Selection policy was recorded before capture/review; each interval spans150ms
  on either side of its boundary bracket midpoint, without sampling. Full-room
  gameplay, captions and guidance remain visible. Source loop resets are deliberate;
  original short cels may be skipped at real-time cadence, per agreed policy.
  Sources/timestamps/hashes and manual interval observations are in
  `streaming-review/boundaries/full-segment-review.json`. Existing1,995-cel art
  review is reused for unchanged pixels/SpriteRects/durations, not recertified.
- `streaming-review/priority-comparison.json` retains exact before/after values.
  Old selected-window maximum gaps:213.021/186.828/200.301/133.370/132.943ms;
  new maxima:19.065/19.137/19.229/19.780/19.439ms. The last old gap is AFTER
  Score loop0?8; its actual atlas10?0 pair is13.769ms. Current no-capture Unity
  traces and separate capture support continuous streaming in this measurement.
  The exact cause of every historical gap remains UNVERIFIED, because the original
  capture had no Unity render trace. Evidence suggests capture-path overhead;
  do not call it a proven historic root cause. No clock fix/clamp/slowdown added.
- New `Tutorial-Atlas-Playback.mp4`:4,991 chronological source PNGs,
  4,992 encoded VFR frames, complete1280x720 UI,500ms terminal hold, full decode
  and increasing PTS PASS; SHA256
  `4f524a8357b3b2c89cc2660eaea705d2f9223a14f311e5ccc834b477d0f4efb3`.
  `streaming-review/clip.json` records original timing. This video is a viewing aid;
  direct frame review above covers the33 complete selected intervals.
- Demo8305 now serves canonical Ubuntu160828/inputacf808ea normal and the new video.
  HTTP31 player payload hashes/video MIME+hash and the intentional existing offline
  fixture worker pass; browser bootstrap/worker/no QA globals/no exceptions pass.
  Receipts: `streaming-review/demo-http.json`, `demo-browser.json`; host PID6136,
  script `Start-Streaming-Demo.ps1`. Existing8303/8304/player/videos remain intact.
- Superseded Ubuntu normal160210/input728fddcf was stopped only after verifying its
  exact task container/result mount. It predates the metadata fix; exit143 is a
  deliberate cancellation, not a build pass. Final Ubuntu normal160828/inputacf808ea
  build PASS;33 downloaded hashes match remote, normal payload QA/type exclusion
  PASS. Native `streaming-ubuntu-normal-small`17/17 PASS; all six640x360 images
  directly inspected. The17-case smoke targets packaging/host change; unchanged
  playback/lifecycle coverage is reused from normal34/QA104 and the33-segment
  review. Other runner jobs/resources are preserved.

Final QA-only refinement: suspend the existing per-frame combat JSON publisher
while TutorialRun is active; the fixed tutorial trace and regular100ms state
observation remain. Resume combat observation after Start Battle. This keeps the
lightweight measurement from serializing unrelated combat JSON each tutorial frame.
Final Windows QA build/strict repeat PASS (`streaming-local/qa-final.log`).
`streaming-final-trace`58/58 PASS explicitly proves no per-frame combat publication
through all five lessons and resumed publication after Start. All33 pairs/buffers/
GPU cleanup pass; final priority render gaps18.455?19.955ms, synchronous resource
calls peak0.355ms. `streaming-final-integration`104/104 PASS again, including actual
parry after Start. No exceptions/provider IO. Final QA-only edits are compiled and
verified here; normal results remain applicable because these types are excluded.
The inherited local snapshot marker predates the last QA edits: reports explicitly
flag it rather than claiming it is current. `streaming-review/final-qa-source.json`
records actual final source hashes (matched to repository) and33 player hashes after
build; this is a recovered post-build receipt, not a fabricated pre-build snapshot.

Implementation and requested scoped checks are complete. Exact commit/push/remote
verification is recorded at handoff in `streaming-review/handoff.json`. Keep the review worktree/branch and the streaming-* evidence/demo
resources until user review/integration; remove only these owned resources afterward.
The retained `streaming-local/source/Library` is a junction to the pre-existing
`unity-local/source/Library` cache; preserve that older cache when cleaning the
new source copy after review. No main merge, full-game FPS guarantee, combat
optimization or external publication.


## New-player tutorial implementation (2026-10-27; in progress)

Approved six-lesson plan: click-to-watch once, explicit DEMO/YOUR TURN, required
real HIT attack and one real safe parry, visible Skip, isolated practice statistics.
Continue the existing tutorial-full-gameplay worktree from7b9f15eb; no main merge.
Reuse TutorialBuild's authored Back/button skin, gold clip-focus edges, existing
live HUD/actors and original clip data. New controls compose these components;
no new art, sprite timing, dependency or public gameplay contract.

Implementation adds TutorialPractice and internal practice gates/reset hooks.
Demo end holds and replay replace automatic joins. Intro focuses HP, word and
enemy HP separately. Only actual attack damage and parry success complete skills.
Practice releases only the reading lock; Pause remains authoritative. Skip uses
existing Skipped storage value and preserves an existing Completed on replay.

Verification pending. Initial Ubuntu serialize023114/input6b1b0370 failed protected
font byte checks due CRLF input; external snapshot adapter normalizes text to LF,
without changing font/checker. Quick023903/inputbbace8a8 failed strict builder
repeat before tests: new practice labels changed typography, cloned focus colors
changed, and Readiness/Guidance sibling order differed. These differences were
inspected and corrected in TutorialBuild; strict checker remains unchanged.
Quick025325/inputb97ad635 is running; it is not yet a passing test receipt.
Source changes after that snapshot require final-source verification.

New-player usability remains heuristic until an independent first-time player
tries the candidate without spoken coaching. Do not label automation as a user study.


### Verification correction / resume point (2026-10-27)

The external normalized-TAR adapter used zero mtimes. Unity reused an older
ScriptAssemblies cache despite changed source bytes. Therefore quick031445/input
554ae1fb and032258/input2f3ede28 (both15/16) are NOT final-source verification;
the Replay assertion belonged to the old test assembly. The verified-delta adapter
now validates every source file hash, sets real mtimes on changed files, and once
forces fresh script/scene import when upgrading from the old transport.

Quick032727/inputf85c6cb3 has fresh scripts:16/18, with typing completion failures.
The typing failure path now waits for the actual attack-window closure and recovery,
not merely an Animator idle/transition observation. PlayerAttackEvents exposes only
an internal read-only WindowActive to preserve its deferred physics tick. Added a
real missed-attack/retry fixture and telegraph-cancel/cue cleanup fixture. The cue
presenter hides live cues outside practice while retaining ordinary Pause behavior.
Quick033458/input644aac54:16/19, typing still pending; further diagnosis required.
Quick034137/inputa676c8f1 became idle before producing XML under -nographics;
its exact owned container was inspected and stopped, no unrelated job touched.
Rendered Xvfb scoped diagnostic quick035727/inputd5bce127 is running. No WebGL
candidate, normal-player receipt, new video or final push exists yet.

Serialized candidate preserves all1231 original object IDs/order:13existing records
changed and76new UI records. Avoid wholesale Unity scene reordering in the commit;
project final builder records back into the original order and verify equivalence.
Current task temp is registered as tutorial-new-player-20261027 for session
01a178b8-da00-7840-a0ba-0498bc9b6bee. Before completion, preserve receipts/player/
video outside CodexTemp, then Complete -Apply. External delta/native drivers are
in the registered directory; do not delete older8303/8304/8305 demos.


### Scope correction before final acceptance

Windows rendered single main flow passes. Combined final-tests:18/19, with only
the additional artificial no-target typing-attack retry fixture failing. That
experimental fallback was not part of the approved fixed-target HIT practice and
is removed with its fixture/internal WindowActive getter; it is NOT recorded as
verified. Original acceptance remains: wrong letters reset, a real damaging HIT
is required, genuine safe parry/retries, navigation/lifecycle/cache/lookup checks.
No completion is granted from merely completing a word. Back/Skip remain usable.
The no-target attack scenario remains unverified; do not claim support for it.
Final scoped matrix is18 cases, including telegraph cancellation, all1995 lookups,
28 atlas boundaries/five terminal holds, zero warmed cache/tick allocation/dirty,
and six directly affected normal combat/parry/scoring regressions.

### Current-source scoped verification, 2026-10-27

Windows Unity2022.3.25f1 rendered matrix `verified-tests.xml`:16/18 passed;
the two live-HIT fixtures failed with accepted word but no damage after synthetic
same-tick input. The helper now feeds each HIT character on a separate tick.
`typing-diagnostic.xml`:2/2 passed for the full genuine typing/parry/miss flow and
telegraphed-attack cancellation. Reuse the other16 unchanged passing cases; this
is combined evidence across two runs, not an18/18 single-run receipt. No runtime
combat timings, damage, collision masks or pass conditions were weakened.
The passing coverage includes1995 lookups/28 internal boundaries/five end holds,
zero warmed render/tick/cache allocation and Image dirty callbacks, lifecycle,
loading errors, navigation and six normal combat/scoring/parry regressions.
QA WebGL candidate is now building; strict repeat-builder/native/normal-player/
video/review-branch push remain pending. No independent novice study is claimed.

### WebGL candidate/build checkpoint

QA build PASS, pinned Windows Unity2022.3.25f1; strict repeat-builder PASS.
Input0aad8b4b5e6728c643214568636e866de115e3947f6f5384679d38594fc87acb;
post-builder receipt d3c30f55e8614f27a693b8dcbeb1b436dfebd61d1a35f1ec592caf8978005158.
Native Edge1280 scope in `qa-native-5`:90checks before replay-menu preparation
was omitted by the harness. The completed scope covers six pages, five full
one-pass clips/end holds/replays, genuine HIT/parry and safe miss, Back cancellation,
readiness, Start Battle, normal typing/parry, restart/menu and texture cleanup.
The report remains FAILED overall; replay/Skip and640 acceptance remain pending.
Earlier harness failures are retained: stale module name; mouse-down frame sampled
before cursor movement; an extra browser read delayed Space until~81ms before
contact. Fixed the observers/input driver, never the game's parry window/speed.
`qa-native-6` was closed through its exact owned Edge window to reduce memory
contention while normal Unity built; it is not a passing complete run.
Normal build is pending. It logged an AssetImportWorker font-metadata timestamp
version error while still continuing; preserve this log and inspect actual player
text/behavior. Source cleanup must remove generated-only serialization changes.
Scene audit:1231 original IDs retained,76added UI records,13meaningful old-record
changes. Two trailing-blank differences and three TMP sorting fields reordered
by Windows serialization are semantically unchanged and excluded from final diff.
Permanent receipts/deliverables live at
`C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-new-player-20261027`.


### Verified new-player candidate (2026-10-27; supersedes pending status above)

Implementation: six lessons with sequential HUD focus; optional one-pass demo,
terminal hold/Replay; required real HIT and one parry; safe original-timing retry;
Back cancellation, persistent Skip, session readiness and fresh selected-difficulty
battle. Practice awards no real-run score/statistics/unlocks. Completed replay is
never downgraded by Skip. No source PNG/GUID/SpriteRect/frame duration/caption/
guidance change. The artificial no-target fallback remains outside scope.

| Evidence | Result |
|---|---|
| Ubuntu rendered quick064043/inputdf152cc0 |18/18 scoped tests PASS; final252code/assembly/JS binding files match the tested snapshot |
| Warmed cache/render/tick,1000calls each |0allocated bytes;0Image dirty callbacks for held/loading frames; new sprite still dirties correctly |
| Current QA repeat builder |PASS: objects/references/listeners unchanged across repeats |
| Native QA640x360 |91checks PASS, all six pages; actual HIT/parry/misses/Back/Pause/Resume/Start/restart/menu/replay/Skip |
| Native normal1280x720 |34checks PASS,0capture errors, no QA globals or external provider writes |
| Normal actual tutorial GPU |Peak2textures,31,477,248bytes;0after practice/battle/restart/menu/replay/late-Skip |
| Normal QA exclusion/copy |qa:false,5payloads scanned ASCII/UTF16;33/33copied player hashes match |
| Source/scene audit |1231old IDs retained,76new UI records,13old records meaningfully changed; builder-equivalent; frame/caption/guidance data hash47a1f786c7deb03f75514b8210eddcfc4a0409175b82ef0431f6777c76c8153b |

The earlier Windows16+2 receipts are retained; the Ubuntu18/18 run is the final
single-run scoped result. The tested snapshot precedes only nonsemantic YAML
empty-mapping whitespace/checkout-EOL cleanup and documentation updates.
Normal build logged one metadata timestamp import-version error while Apply ran;
it still completed with exit0. Actual normal text/font/behavior checks passed.
This diagnostic is preserved, not presented as a clean no-diagnostic log.

The four still-internal priority atlas transitions were traced at both sizes:
Slayer2-to3/4-to5/9-to10 and Score9-to10 show13.8-40.9ms render intervals.
No historical133-213ms gap recurred there. Original clock policy stays; no fixed
clamp, clip slowdown or blanket exclusion of prefetch time. Score10-to0 is now
explicit Replay, with its loading/reset shown in the full new capture.
QA trace counts11438/9674, no overflow; at most2slots. Raw traces are preserved.

#### Direct capture review and video

Normal-native-1 correctly rejected its capture after asynchronous write backlog;
its functional observations are not substituted for a passing capture. The final
normal-native-2 uses a256MiB per-segment RAM threshold, early ACK and asynchronous
post-segment writes; maximum observed segment62,763,553bytes,0capture errors.
CDP PNG encoding delivered168adjacent timestamp regressions in arrival order.
All1465records have unique presentation timestamps: sort by original PTS, retain
all raw frame IDs/hashes and every repeated image; no frame was discarded.

| Selected complete capture segment | Frames inspected/total | Consecutive transitions inspected/total |
|---|---|---|
| Three-focus intro and lesson entry |130/130|129/129|
| Typing demo terminal/Replay |155/155|154/154|
| Real HIT: wrong letter, contact, recovery/result |175/175|174/174|
| Mistakes demo terminal/Replay |179/179|178/178|
| Parry demo terminal/Replay |144/144|143/143|
| Live parry: miss, Retry, preparation/contact/recovery/result |401/401|400/400|
| Slayer demo terminal/Replay |117/117|116/116|
| Score demo terminal/Replay |164/164|163/163|

Total1465/1465frames and1457/1457transitions, directly viewed on41overlapping
sheets, including cross-sheet pairs; all page/practice state PNGs also viewed at
native1280x720/640x360. Five terminal/Replay windows add to the unchanged28internal
boundary receipts:33boundary/end windows remain covered, with the new one-pass
policy replacing automatic loop transitions.

Capture intervals up to200-240ms in static intro/Finished/result holds are retained.
The first live-parry213ms interval spans preparation-to-strike; rAF stays continuous
(~13.7ms maximum using estimated clock alignment), but exact capture-vs-game-render
attribution is UNVERIFIED. Do not claim every Unity render was captured or whole-game
smoothness. This is live practice with no displayed tutorial atlas, not evidence
for changing streaming clock policy. No segment was narrowed to hide the interval.

Video `Tutorial-New-Player.mp4`:1280x752 (unchanged1280x720game plus32px title),
36.1066s before codec tail,1465source rasters plus one identical terminal flush
raster, variable timing, silent. All source frames retained; monotonic encoded PTS,
maximum timing error1.192ms, full decode PASS. SHA256:
`3a18a12d5842ec92188259860f9d625844b227d88eb8cb1daee0dfcf0eed63c0`.
Three encoded-state previews directly viewed. Old videos/demos8303/8304/8305 remain.

#### Review delivery

Normal demo: http://127.0.0.1:8306/ . Video: /Tutorial-New-Player.mp4 .
Permanent artifact root:
`C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-new-player-20261027`.
Contains player, raw accepted capture, reviewed sheets/manifests, XML/logs, source
and QA-exclusion audits, failed-run reports, host/Stop-Demo script and final git
receipt. Demo bootstrap controls the page with a local fixture service worker;
external leaderboard requests never reach the real provider.

Keep worktree `C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-full-gameplay`,
branch `micharel09/tutorial-full-gameplay`, for review; do not merge main.
Final commit/remote verification is recorded in artifact `delivery.json`.
Keep the owned8306host/player/video until review acceptance; stop only via its
Stop-Demo.ps1, then remove the artifact folder when no longer needed.
Independent novice usability remains untested; no whole-game FPS claim.

### Gothic tutorial buttons (2026-10-27; verified review candidate)

Source: [Gothic Pixel UI by AbyssOwl](https://abyssowl.itch.io/gothic-pixel-ui), free 1.0.2. Directly inspected the full supplied sheet and `split_assets/frames and bars/large_ornate_frame2.png` (Gold). All 80source PNGs remain byte-identical to the official archive; all 90new GUIDs are unique and do not collide with existing assets. Source license/credit live under `Assets/Art/UI/GothicPixelUI/`. The existing GameLoopBuild credit exporter appends AbyssOwl to output CREDITS.txt and ships GothicPixelUI-License.txt. No paid assets or newly painted/composited borders.

Mapping: one complete supplied Gold frame per TutorialOverlay button, separate existing TMP label, 12 px nine-slice corners with Point filtering/uncompressed texture. The author explicitly permits stretching/extending sections; only straight edges/solid center stretch. No dedicated states exist for this frame, so Unity ColorTint handles normal/hover/selected/pressed/disabled. TutorialBuild.Guide applies the final skin after shared migrations and removes DungeonControlSkin from all 8tutorial buttons, including inactive legacy controls. White labels fix the inherited gray hover contrast without changing font, bounds, events or navigation.

Verification:
- Windows Unity 2022.3.25f1: final strict repeat-builder and QA WebGL PASS, exit0. Both complete Apply passes preserve serialized objects/references/listeners. Normal WebGL PASS, exit0; all 4JS/WASM/data payloads exclude QA markers in ASCII and UTF16; qa:false marker verified. Runtime/config/package inputs and existing gameplay art are unchanged.
- Final native Edge 156.0.4314.42 / GTX 1650 Ti ANGLE D3D11: 374 checks PASS at actual 1280x720 and 640x360; 0 exceptions/provider reads/provider writes. Real Watch/Replay, Retry/Back/Next/Skip/Start, disabled click rejection, safe HIT/parry practice, pause/resume, restart, replay and clean battle covered. An earlier gray-label candidate also passed374functional checks; it is superseded visually by the white-label candidate.
- Directly inspected 20final UI stills covering all visible button roles, both layouts and normal/hover/pressed/disabled. Corners remain intact, labels fit and controls do not cover content. Solid white glyph-core contrast against the flat button face is 9.808 normal, 5.256 hover and 14.030 pressed at both sizes; previous gray hover contrast was 2.145. This is a measured button-face contrast, not a whole-game accessibility/performance claim.
- Normal demo smoke: local-fixture header verified; unseen Play enters the tutorial, Skip enters battle, viewed Play bypasses the tutorial and Pause remains intact. All 4 normal smoke stills directly inspected; no QA/debug overlay. Credit and exact license copy verified in the exported player.
- Source projection changes 34 tutorial records, removes 8 old skin components and preserves 978 non-tutorial scene blocks byte-for-byte. No RectTransform changed; standalone listeners/navigation match, and 54 changed prefab properties are appearance-only. No new dangling local references. Restored 148 unrelated builder-generated files; menu/HUD/shared assets and all runtime/config files remain unchanged.

Final QA data SHA256: `8af0adfe6b9c559edc8eb49e71258fbcc11a96d9d81343b9f76a6ea6a8b1b2da`. Normal data SHA256: `38116beb07e7fb941629cc3a3b289df533db2f07054dff8598582b682276344e`. The first/final QA WASM and framework JS are byte-identical; final UI data differs for label color. The private harness snapshot field reused the baseline source marker; actual emitted-player hashes, exact build root and the directly inspected white labels identify the final run. No complete final source-input hash is inferred from that legacy field.

Delivery: new normal demo http://127.0.0.1:8307/ . Permanent root `C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-gothic-buttons-20261027`; independent normal-player, fixture worker, host and Stop-Demo.ps1 have no dependency on task temp. Retain until review; stop only the recorded owned host with Stop-Demo.ps1 and remove this review folder when no longer needed. Old 8306 demo/artifacts/video remain untouched. Review branch/worktree: `micharel09/tutorial-full-gameplay` at `C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-full-gameplay`; keep for review and do not merge main. The delivered README records the pushed commit after remote verification.

Earlier attempts: two Ubuntu runner-lock waits timed out; run20261027T082357-serialize lost SSH before its result could be retrieved, so no Ubuntu PASS is claimed. User requested continuation after the Windows fallback was offered. Earlier Windows launches ended without a result; only explicit successful final runs above are accepted. No runtime test suite, animation authoring/review, stress/soak, performance benchmark, publishing or main integration was added. Temporary captures/profiles/logs are cleaned after recording this textual receipt; only the requested review demo/source archive and its operational files remain outside temp.

### Tutorial fixed footer (2026-10-27)

Approved layout: reuse AbyssOwl skin and existing control identities, one fixed footer and compact top statuses. TutorialBuild applies final positions after shared migrations. Reference960x540 footer y484/h40: Back x24/w128, Watch/Replay x168/w248, Skip x480/w160, primary x656/w280. Modal coordinates account for parent offset(16,12). Hidden controls leave empty slots; HUD Next and practice actions share the primary slot. Title/step/status share the header; practice status uses one centered line. Clip/caption/rule and typing bounds stay intact. EndlessOverlay retains TutorialPrompt/PracticeButton and resets its own bounds on Show. No assets/dependencies/public APIs added.

Verification: Windows Unity2022.3.25f1 strict repeat builder PASS; QA and final normal WebGL PASS/exit0. Edge156.0.4314.42:389 scoped checks at actual1280x720 and640x360 pass: fixed footer baseline, untruncated labels, Watch/Replay, disabled click rejection, real HIT, genuine parry after miss/Retry, Back, Next, completion/Start, fresh battle and Skip preserving completed progress. QA errors/browser exceptions/external requests:0. Directly viewed all30 QA stills covering both sizes and normal/hover/pressed/disabled states. Visual review caught old multiline terminal copy overflowing the compact header; final copy is FINISHED. Only this string changed after the functional run: unchanged behavior/builder results are reused, with the final production replay state directly inspected at both sizes. Seven final normal UI stills pass with no QA/debug overlay. No new animation/performance/full-suite/novice-usability claim.

Source audit:23 tutorial scene records change geometry/font size; other1276 records retain original bytes. All object IDs and Button components/events/navigation remain identical, as do QA/normal tutorial records. Restored149 unrelated builder outputs; removed only three known Unity-created metadata files. Pack/menu/HUD assets and learning/combat rules unchanged. Normal qa:false and all4 payload QA-marker scans pass (typingHeroQA, typingHeroCombatFrame, GameLoopProbe, TutorialStreamingTrace; ASCII/UTF16). Final normal data SHA256:`2aee6e290b8ff8d02ebdb9bcaf682562ce68d6378291fca09dd627c852292031`.

Attempts: initial private input driver clicked before menu transition completion; corrected driver waits for it. Early normal recordings had the same readiness issue and are rejected. QA export logged a transient font metadata import error but completed strict repeat/build with exit0. Intermediate normal build was canceled to incorporate the terminal badge fix; only the final successful normal player is delivered.

Delivery: http://127.0.0.1:8309/ and /Tutorial-Layout.webm under `C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-layout-20261027`. Retain independent player/video, fixture worker, recorded host and Start/Stop-Demo.ps1 until review; stop only the verified owned host using Stop-Demo.ps1 and remove this artifact folder when no longer needed. Old8306/8307 demos/videos untouched. Keep existing tutorial-full-gameplay worktree/branch for review; push current branch, do not merge main. Final remote receipt is in artifact README. Temporary captures/drivers/build logs are cleaned after these textual results are saved.


### Compact five-card tutorial (2026-10-27; verified review candidate)

Approved scope implemented: centered 600x240 card at (180,150), short rule left,
240x135 sprite clip right, title above, one Next / final Start Battle button and
small corner Skip. Five cards include HP in the first. No practice, pass gates,
Back/Watch/Replay/Retry, badges or beat guidance. TutorialDirector no longer
creates TutorialPractice. Clips autoplay and loop with the original timeline,
reset on entry and hold while paused/loading. TutorialBuild's final pass and
runtime share compact geometry; Endless retains TutorialPrompt/PracticeButton.
Original clip artwork, GUIDs, timings, dependencies and gameplay rules unchanged.

Verification:
- Windows Unity2022.3.25f1 strict repeat builder PASS: serialized objects,
  references/listeners unchanged over two complete passes, successful exit0.
- Normal WebGL build PASS, exit0, qa:false. Four JS/WASM/data payloads exclude
  typingHeroQA, typingHeroCombatFrame, GameLoopProbe, TutorialStreamingTrace and
  TutorialPlaybackTrace in ASCII/UTF16. Browser globals remain absent.
- Final projected scene: TutorialTests 7/7 PASS, 0failed/skipped,6.6656177s,
  exit0. Covers all five cards, actual async sprite loads, looping/reset,
  completion without practice, cache lifecycle/allocations, reading pause/input/
  audio/provider locks, restart, abandon, preferences, Skip and fresh battle.
- Edge normal player at actual1280x720 and640x360: all10card stills directly
  inspected. Short text fits; small clips remain visible; no old controls/badges/
  extra tutorial rim. Real Enter/mouse Next, final Start Battle, Pause/Resume,
  exit to Menu, unseen Skip, Menu Tutorial replay/Skip and viewed Play verified
  through direct flow captures. JS page exceptions0; external requests0 with
  provider requests intercepted locally. No QA/debug overlay.
- Scene projection changes16tutorial records, adds0, preserves1283other records.
  All original IDs retained. Restored148unrelated builder files and5editor/test
  writes; removed only3known Unity-created metadata files. Assets outside scope,
  menu, packages and the dirty primary checkout are preserved.
- New VP9 WebM:1280x720,25fps,27s,silent,1238264bytes. Full decode and HTTP200/
  video-webm delivery pass; four encoded previews directly inspected at1,12,24,
  26.5seconds. Original sprite review reused because art/timing are unchanged;
  no new animation authoring, performance, full-suite or novice-usability claim.
  Shogun Showdown source comparison remains unverified; layout follows the user.

Attempts: first editor startup was stopped after an apparent assembly-reload
stall, without a PASS claim; the established E:/Unity editor/two-worker retry
completed. Two normal Menu replay pilots used old coordinates and then omitted
its existing name-panel Start confirmation; their replay captures are rejected.
The corrected scoped rerun confirms the complete path at both sizes. The five
cards/Start/Pause evidence from the unchanged player remains valid.

Delivery: http://127.0.0.1:8310/ and /Tutorial-Small.webm . Permanent root:
C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-small-20261027 . Independent
normal player, fixture service worker, host and Start/Stop-Demo.ps1 are retained
until review; stop only that recorded host with Stop-Demo.ps1, then remove this
folder when no longer needed. Old8306/8307/8309demos/videos remain untouched.
Keep existing worktree C:/Users/ADMIN/orca/workspaces/TypingHero/tutorial-full-gameplay,
branch micharel09/tutorial-full-gameplay for review. Push current branch only;
do not merge main. Final commit/remote receipt is recorded in artifact README.
Temporary capture collections/drivers/logs are removed after these textual
results and hashes are retained.

Video SHA256: `87300221431679196e7a6d24584f334c50ed674e358141f951493ef5c6786023`.
Normal payload SHA256:
- `normal-player.data`: `50bdd8fe162507efa1921a8ec65641256fe394353bceeb28e016e792acbf8b96`
- `normal-player.framework.js`: `3a135de2226d0be04d56916641dc18decbf1d2e1c479de07f31848ee4c88d96a`
- `normal-player.loader.js`: `1d61ac36c38d8fa61f2e2e53974a6047ccc321d79b216b068930afc6a205ff76`
- `normal-player.wasm`: `840c8dd25741fbc28eadf006dbb5f8b28af9e3b90b71c4941c42ad2c6c6796ee`

### Centered tutorial follow-up (2026-10-28; in progress)

- Scope: five 600x240 cards, centered 240x135 clip, two-line rule beneath,
  title at top left and adjacent Skip/Next or Start Battle at top right. Shared
  builder/runtime layout and the existing Endless template bindings are retained.
- Worktree `tutorial-full-gameplay`, branch `micharel09/tutorial-full-gameplay`,
  base `fe44e68e`. The implementation and verification progress is recorded below.
- Inherited Unity serializer PID 24036 stopped after becoming orphaned and making
  no log progress for nearly two minutes. The E: volume briefly disappeared;
  its relationship to the stall is unconfirmed. No passing result is claimed.
  Copies of the inherited scenes and an isolated HEAD-plus-source snapshot are
  retained at `C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-centered-work-20261028`.
  Shared fly-server runner was busy; this task queued without interrupting it.
- Isolated snapshot `434bf6471e79a3eaa5b8a5962ea8552ebe5d134f7f4f526428133147814e0181`,
  serialize `20261028T021425`: FAIL before repeat, protected typing font
  `CobblestoneUI64.asset` changed during builder. A later copy of the shared
  server source matched HEAD, but another job may have replaced that source;
  it does not establish the failed run's final bytes or cause. Retrying the
  identical snapshot with the warmed import cache and unchanged assertions.
- Repeat `20261028T022626`: same protected font failure. A third diagnostic
  snapshot adds only before/after copies at the existing failure point; the
  assertion and builder conditions remain intact. No strict pass is claimed.
- Diagnostic `20261028T023705`: the protected before/after font files differ
  only in CRLF versus LF (6,014 line endings); content matches after newline
  normalization. `git archive` inherited Windows `core.autocrlf=true`. Recreated
  the snapshot with `core.autocrlf=false` and confirmed the font bytes equal the
  Git blob; strict verification is rerunning without diagnostic code or relaxed
  checks. The font asset itself was not changed.
- Clean-LF snapshot `69b8ed8b0cf92577da1b54377bf23d617fee0c4dd5ded2b609e3d43c1f15b0b6`,
  serialize `20261028T024342`: stopped after more than eight minutes without
  log progress following SampleScene load. The Unity main thread waited on a
  futex while ALSA Mixer consumed a CPU; causation is unknown. Exit 143 is not
  a pass. A local isolated Windows attempt was stopped after the server runner
  became available; that attempt also produced no result.
- Retry `20261028T025424-serialize` on the same clean-LF snapshot: strict repeat
  builder PASS, process exit 0. Projected the final scene to eight changed
  tutorial records matching the builder, retaining 1,291 other records byte for
  byte and all 1,299 original IDs; no missing local reference candidates. The
  inherited scene copy remains in the temporary evidence directory.
- Projected scene snapshot `c44478e61fb6361599838298c436c36d0b40b332c48e1c53b59a8e395470a1dd`,
  `20261028T031439-quick`: the three requested `TutorialTests` PASS, 3/3,
  zero failed/skipped, 3.7719508 seconds. Strict builder above ran before scene
  projection; the projected records match its output as noted.
- Same snapshot `20261028T031925-production`: normal WebGL PASS, exit 0. All 34
  player files match remote/local SHA256; four payloads exclude ASCII/UTF16 QA
  markers. Edge normal player: ten card screenshots directly inspected at
  1280x720/640x360, four mouse Next clicks, Start, Skip/reopen, held Enter,
  Pause/Resume and frozen reading checked. Correct real keyboard input after
  Start and Skip scored 10; timer advanced. Zero page exceptions/external
  requests; fixture service worker blocks real provider effects. First typing
  pilot sent input too soon during Resume and did not establish input; a later
  retry after transition succeeded.
- Demo http://127.0.0.1:8311/ and `/Tutorial-Centered.webm`: VP9 1280x720,
  25 fps, silent, 28 s, 1,010,625 bytes, SHA256
  `4599fe4b7f0122de582788f107257b0d27ba80f59725dc4613d5366752712036`.
  Complete decode and HTTP `video/webm` pass; six encoded previews inspected.
  Original recording's video clock differed from driver wall clock, so card
  transitions were located in the encoded frames before the five-second cuts.
- Outstanding: plan calls for an Endless intro with Fight. Current source starts
  the first encounter immediately and uses Continue after a win; no Fight intro
  exists. Combat has no tutorial overlay, but the post-win Continue screen has
  not been directly verified on this player: a real-input Very Easy attempt
  ended in Game Over at score 102 before the first enemy was defeated. The user
  accepted this review and requested the larger modal follow-up; no earlier
  remote push was made. Review receipt and Start/Stop scripts are in
  `C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-centered-20261028`.

### Larger tutorial modal follow-up (2026-10-28; verified review candidate)

User accepted the centered tutorial and requested a larger modal and GIF.
Retain the current behavior, text, positions relative to the card, 16:9 ratio,
review branch and previous demos. Proposed reference-space bounds: card
720x340 at (120,100), GIF 400x225 at (160,50) within it, rule 680x44 at
(20,290), action 176x32 at (520,10), Skip 60x26 at root (570,113).
Runtime/builder layout and the existing focused layout assertion were updated;
verification progress is recorded below.
- Unity Windows strict repeat PASS with exit 0 after clean Library import; both
  earlier copied-cache attempts failed before tutorial checks with missing scene
  binding/renderer features and are not passing evidence. Shared fly-server runner
  timed out waiting for its lock; other tasks were left intact. Compared to the
  previous projected scene, five layout records match builder output and 1,294
  other records remain byte-identical; all 1,299 IDs and local references remain.
- Three requested focused PlayMode tests PASS, 3/3, zero skipped, 5.0806809 s,
  exit 0 on the projected scene.
- Normal WebGL build PASS, exit 0; build manifest reports qa:false and all four
  player payloads exclude ASCII/UTF16 QA markers. The 34 player file hashes are
  recorded in the review receipt. On the actual normal player, all ten card
  states at 1280x720 and 640x360 were directly inspected. Mouse Next, Start,
  Skip/reopen, held Enter, Pause/Resume and the reading lock were exercised;
  page exceptions and external requests were zero. The previous candidate's
  real typing and timer checks are reused for unchanged combat behavior.
- Demo http://127.0.0.1:8312/ and /Tutorial-Larger.webm: VP9 1280x720, 25 fps,
  silent, 28 seconds, 1,106,017 bytes, SHA256
  bb206d3cd1664df9a6a6a4b823e8b84278dae387e7b7f739da1a2cce5ca0710b.
  Complete decode and HTTP video/webm passed; six encoded previews directly
  inspected in order. Capture clock drift required cuts based on decoded frames.
  Artwork, frame order and clip timing are unchanged. Existing Endless has no
  initial Fight intro; post-win Continue remains unverified as described above.
- Review artifacts and Start/Stop scripts remain at
  C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-larger-20261028.
  Keep the demo, evidence, branch and worktree until review ends; stop the
  task-owned host and remove only approved review resources afterward.

### Narrower tutorial modal follow-up (2026-10-28; verified review candidate)

User identified excess horizontal spacing in the larger modal and approved
reducing width to 560 while keeping the GIF 400x225. Reuse ApplyCompactLayout
and the current builder: panel 560x340 at (200,100), GIF at (80,50) within it,
title 240x32, rule 520x44, action at (360,10), Skip at root (480,113).
Horizontal GIF padding becomes 80 per side; the GIF stays at its previous
absolute canvas position. No sprite, playback or gameplay behavior changes.

- Continuing review branch micharel09/tutorial-full-gameplay from 0ac73059;
  existing unrelated dirty asset changes remain outside this follow-up.
- Shared fly-server Unity runner is occupied by another job. Strict repeat
  builder runs in a new isolated Windows Unity2022.3.25f1 clean-Library project,
  using canonical Git archive plus the two changed source/test files.
- Evidence: C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-trimmed-work-20261028.
  A fixture-isolated normal demo is prepared at tutorial-trimmed-20261028,
  port 8313; earlier demos are preserved. Results follow.
- Strict repeat builder PASS, exit 0: serialized objects, references and listeners
  unchanged across two complete passes. Initial Python CREATE_NO_WINDOW launches
  stalled at ILPP connectivity and were stopped; a hidden Start-Process launch
  after clearing the task's stale ilpp.pid completed. These two changes were
  applied together; the individual cause is not established. Remote retry timed
  out waiting for the shared runner lock and is not passing evidence.
- Projected six tutorial RectTransforms from the completed builder output. All
  1,299 scene IDs remain, 1,293 other records are byte-identical to 0ac73059 and no
  local references are missing. No listeners, navigation or template bindings
  changed. The focused CompactCardsAutoplayLoopAndFinishWithoutPractice test
  PASS on this final scene: 1/1, zero failed/skipped, 37.0413469 s, exit 0.
  Other lifecycle/pause tests from the previous candidate are reused for
  unchanged behavior.
- Normal WebGL PASS, exit 0; qa:false, all four player payloads exclude known
  ASCII/UTF16 QA markers. The final builder's six layout records match the
  projected scene. SHA256 for all 34 player files is in player-sha256.json.
- Actual Edge normal player: all ten cards directly inspected at 1280x720 and
  640x360. GIF keeps 400x225 and its previous absolute canvas bounds; horizontal
  modal padding halves to 80 per side. All titles and two-line rules fit,
  including Score & Combo. Four mouse Next clicks, Start, replay, held Enter
  advancing once and Skip were checked at both sizes. Battle screenshots show
  the tutorial cleared after Start and Skip. Zero page/console exceptions and
  external requests; QA global undefined at both sizes.
- New demo http://127.0.0.1:8313/ with the existing fixture worker/CSP and
  Start-Demo.ps1/Stop-Demo.ps1. Receipt and player hashes are in
  C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-trimmed-20261028.
  Previous demos/clips stay intact; no sprite or playback changes.
- Transfer archives and unused temporary runner were removed; Unity and browser
  checks finished. Retain the isolated cached project and evidence directory for
  subsequent review iterations, plus the demo and review worktree/branch.
  After review, stop only this demo host and remove task-owned artifacts/cache;
  remove the worktree only once unrelated dirty changes have been preserved and
  it is clean. Main integration remains deferred for review.

### 2026-10-28 ? exhaustive tutorial stutter audit

- User requested direct review of every frame after observing stutter.
  Candidate645eff0e and normal demo8313 were retained unchanged.
- Directly inspected all1,995 frames and1,995 outgoing transitions including five
  loop seams, using103 full-frame/nearest-neighbor zoom sheets: typing106,
  mistakes160, parry303, slayer745, score681. Per-frame CSV observations and
  evidence: C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-trimmed-work-20261028/frame-audit/README.md.
- Smoothness FAIL: typingF13/F16/F84 hold586.868/586.856/559.680ms during
  idle/windup, totaling47.3% of the3.665s clip. ScoreF220 holds240.1ms during
  idle. Every clip has an abrupt loop reset. These defects were not evaluated
  by the earlier layout/functional PASS.
- Source PNG hashes, atlas frame pixels, source timeline hashes and all durations
  match the original timestamp-sorted captures for all1,995 frames. Recent
  modal resizing did not introduce these source holds.
- SlayerF219..400 holds the actor/slash while HUD timers continue. Current
  Slayer code has explicit enemy hit-pose holding and player attack states with
  no outgoing transitions; historical captured player-slash intent is not proven.
- Fixture-isolated normal-player observation visited all five cards for two clip
  durations plus2s each; end screenshots directly inspected. Zero exceptions
  or external requests, QA undefined, two atlas textures peak. rAF maxima
 93.5..120.1ms show additional browser cadence gaps but do not isolate their
  cause. Normal/slow continuous visual playback and attribution of loop reload
  delays remain unverified; no smoothness PASS or fix is claimed.
- Recommended follow-up: stable-cadence recapture/repack using the existing
  pipeline, full replacement-frame review, and separate diagnosis of Slayer
  pose holding and loop loading. Do not shorten source durations to hide stalls.
- Browser review process ended; no new Unity process/build. Preserve audit
  artifacts/demo/review branch until findings are resolved or archived.
  Existing unrelated dirty files and main remain untouched.

### Tutorial smooth recapture and loop fade (2026-10-28; in progress)

- Continue `tutorial-full-gameplay`, `micharel09/tutorial-full-gameplay`, base
  `c7794ff486d343fd16d45c0f3df4792d90a534d6`. Review branch only; do not merge main.
  Preserve existing dirty animation, importer, scene and tutorial descriptor changes.
  Task evidence: `C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-smooth-work-20261028`.
- Scope: recapture five full-gameplay clips at real speed, target 30 fps; pilot
  gates >=28.5 fps, P95 <=50 ms, max <=100 ms, no missing frames. Keep modal
  560x340 / GIF 400x225, English copy, current combat and Slayer behavior.
  Original atlas assets remain unchanged until pilots pass.
- Runtime adaptation: `TutorialOverlay` reuses `TutorialAtlasCache.Get` / held-page
  protection and `CommitDisplay`, selecting the next atlas circularly. A local
  phase clock follows `PanelReveal` unscaled SmoothStep: final frame completes,
  first page readiness is checked, 100 ms fade-out, first-frame reset at alpha 0,
  100 ms fade-in. Release resets phase/alpha; existing pause gate freezes updates.
  Implementation is NOT yet accepted on a real player.
- Recorder: `tools/game_loop/tutorial_readback.cjs` adapts the existing default
  framebuffer capture and MDN non-blocking async readback:
  https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/WebGL_best_practices#use_non-blocking_async_data_readback
  Three preallocated PBOs/fences, request-time timestamps, ordered RGBA RAM
  storage, restored pack/read-framebuffer/read-buffer state, bottom-up conversion
  only for export/OCR. Limits 768 MiB and 24 s. Buffer/fence/context/export errors
  reject capture. PNG export is available only after complete drain.
  Node syntax check passes; cadence/image correctness remain unverified.
- Tests adapt the compact loop assertion and recover slow-load/stale-completion
  plus exhaustive frame-lookup coverage from `ce9b87a5:Assets/Tests/PlayMode/TutorialTests.cs`.
  Added circular-prefetch, terminal hold, two fade phases, pause hold and navigation
  alpha reset checks. No test result is claimed before matching its snapshot hash.
- Isolated Ubuntu snapshots are canonical HEAD inputs plus only task playback/test
  source, excluding inherited dirty files. First snapshot
  `e0a0400776d2a882a0ec155da9804660d29a5f1218c9fe82e68168eae020d11b`
  timed out waiting 120 s for the shared runner; retry waits up to 3600 s.
  Expanded-test snapshot
  `b026d3ed420c5d21b70522a7ffa2de7add766e5a56feb5b16af2bd4bf2d3bb05`
  is separately queued. Other jobs and shared cache are preserved.
- Ubuntu recorder preflight: container attempts with SwiftShader and ANGLE GL
  both selected SwiftShader. Existing host Chrome GL-EGL workflow was found in
  `E:/CodexTemp/TypingHero/parry-gold-qa-20261028-7246068222b3/browser-remote.py`;
  its verified prior renderer is Intel HD4600. Reuse its temporary `render` group
  launch without persistent permission changes. Fixture-isolated typing baseline
  and 8 s async pilot are queued under the shared lock, using the preserved QA
  player `20261028T040651-webgl` only for recorder diagnostics, not final assets.
  Remote task files: `/data/typinghero-game-loop/task-tutorial-smooth-20261028`.
- Remaining: both cadence pilots and no-record comparisons, OCR integration,
  five replacement captures/provenance/atlases, full frame/transition/playback
  review, final scoped tests, strict repeat builder, QA/normal players and native
  acceptance, demo8314/walkthrough, verified commit/push. No new demo or smoothness
  PASS exists yet. Retain existing demos and this review worktree.


#### Smooth tutorial checkpoint: implementation and recorder pilot receipts

- Preliminary playback input `b026d3ed420c5d21b70522a7ffa2de7add766e5a56feb5b16af2bd4bf2d3bb05`:
  Ubuntu `20261028T081340-quick` TutorialTests 9/9 PASS, no failed/skipped,
  15.9460633 s. XML copied to task evidence `tests-081340.xml`. A duplicate queued
  invocation ran the same expanded snapshot at `081652-quick`, also 9/9 PASS.
  Do not use unrelated `075701-quick` (55 tests / different hash) as task evidence.
- Later source review added a physical final-frame hold (`finalFrameElapsed`) to
  handle loading/seek-to-end before fading. Fractional render-tick overrun may
  enter fade; first-page loading time is discarded via `waitingForLoop`. These
  later changes require a new focused test run; preliminary 9/9 is NOT a final
  result for them. QA-only trace now exports numeric phase and image alpha.
- Canonical pre-final-hold QA input `f38a0bd191452872b52c9796479ccb0d010c78ad3ab76e5a8714035ffeb76a4c`:
  `084532-webgl` stopped with exit143 after unchanged import log/main-thread futex
  and Audio Mixer around95% CPU. Rendered retry `093820-webgl` repeated the same
  symptom and was also stopped. Both stopped containers were verified by their
  task result mounts/names; worker logs retained. No build/strict-builder PASS.
- Private Ubuntu build reuses server.py with task BASE/private Library/cache/HOME
  and a task Docker bridge (same pinned Unity identity). Root:
  `/data/typinghero-game-loop/task-tutorial-smooth-20261028/unity`.
  Initial private launch was deferred when the shared retry became available;
  after that retry stuck, fresh-Library QA `unity/results/20261028T095221-webgl`
  started on f38a0bd1. It is importing packages/assets; no successful build yet.
  Private activation files remain outside source snapshots under a mode700 root.
  Network `typinghero-tutorial-smooth-build-20261028` has task ownership label;
  remove it only after its containers stop and this build environment is retired.
- Recorder allocates CPU RGBA destinations before timed capture, and reuses export
  canvases. Existing GL errors are reported separately from recorder errors.
  Warm-up is a separate complete0.5s readback run while the game is Paused; its
  request/completion/timing receipt is retained. Release it, Resume, then start
  the actual source capture. No warm-up frames are presented as source frames.
- Normal-player typing cadence under the shared exclusive lock:
  `/data/typinghero-game-loop/task-tutorial-smooth-20261028/pilot-normal-idle`.
  Capture241/241 complete,30.040968 fps,P95 39.525ms,max51.34ms,no readback error
  or pre-existing GL error. Baseline57.388166 fps,P95 16.67ms,max99.995ms.
  Capture callback max21.085ms. First/final PNGs directly inspected at640x360:
  full frame, correct orientation/color/HUD; real typed word `late` scored10.
  Only endpoint images were exported for this pilot, not an exhaustive motion
  review. Cadence PASS does not certify final replacement assets.
- Earlier contended normal attempts FAILED cadence (27.75fps/P95 56.735/max254.89;
  preallocation27.96fps/P95 52.29/max185.92). Their baselines also contained gaps.
  Preserve their pilot.json/host.json/browser reports; do not lower the gate.
- Actual serialized parry stun is10s (Player.prefab), so the second pilot keeps
  complete entry/Slayer/exit/normal attack rather than cutting to8s. Limit24s and
  all cadence criteria stay. Guard is prepared through real Space inputs using
  the prior small framebuffer observer, stopped before timed async capture.
- Second-pilot controller basis recovered from
  `E:/CodexTemp/TypingHero/space-press-cue/full-review/slayer-capture20-driver.cjs`,
  full-cache.py and full-no-pause.py. Prototype driver:
  task evidence `slayer-pilot.cjs`; Ubuntu copy under task `runner/`.
  Lookahead OCR overlaps actual typing; >300ms inter-word delay or a word that
  fails to advance the real timer rejects the attempt. No gameplay/Slayer edit.
- OCR runs separately in task Python3.12 venv (existing Playwright image), pinned
  RapidOCR1.4.4/ONNX Runtime1.31.0; game Packages/dependencies unchanged. Loopback
  service metadata is task `ocr-host.json`. Actual text crops were warmed before
  readiness. Active WordPool GUID98cf087e matches WP_Common200; its normalized
 219-word set equals200tu.txt.txt. This is an OCR allowlist, not a game-state read.
- Failed Slayer pilot folders `pilot-slayer`, `pilot-slayer-queue`,
  `pilot-slayer-queue-normal`, `pilot-slayer-pad14-verified-word`,
  `pilot-slayer-lexicon` preserve marks/final diagnostic images and available
  capture-status. Reasons include pause-dimmed OCR, >300ms inter-word gaps and
  wrong recognition (`at`->`t`, `was`->`eas`, `will`->`i`). None is accepted media.
  Last recognition issue traced to max-channel threshold including red Slayer
  background (observed bbox240px vs font `will`64px). Current OCR adapts the
  original signature's RGB-neutral mask; this latest fix still needs verification.
- No tutorial PNG/manifest/descriptor or scene has been replaced. Existing dirty
  assets, old demos and main remain untouched. Full five-clip replacement,
  exhaustive final review, final tests/builds/native loop acceptance and new
  demo/commit/push remain pending. No completion or remote integration claimed.


#### Smooth tutorial checkpoint: second pilot and current test snapshot

- Current playback/test snapshot is
  `77b32c678e978c1abd54b6a06e5e06ffd3e03806d6bf4bdc649da3f45c50e237`.
  It includes physical terminal-frame hold, delayed-loop fresh fade, and the
  tenth TutorialTest covering a one-page clip without duplicate cache slots and
  a delayed first-page load. Private Ubuntu quick/TutorialTests is waiting for
  the task build lock; no result yet. Scoped `git diff --check` passes.
- Private f38 QA build progressed through import and shader compilation into
  WebGL IL2CPP compilation. It remains an old-source diagnostic build; even a
  successful result will not certify the current snapshot or final assets.
- OCR neutral mask corrected the observed red-background contamination.
  Narrow single-glyph correction `8` -> `a` requires the observed 20x24 pixel
  crop (tolerance2), checked against Cobblestone font metrics. Preserved real
  screenshot now recognizes `a / still / last`. No source game state is read.
- `pilot-slayer-glyph-a`: real Guard break, nine continuous Slayer words, natural
  exit, and subsequent normal word/HP reduction all completed. Capture464/464,
  28.543704fps,P95 53.575ms,max208.895ms,max callback174.595ms,no readback error.
  Cadence FAIL; NOT replacement media. All464 PNGs were exported after stopping
  for diagnosis. This capture overlapped the task shader build, so causation is
  not isolated. Full review of these failed frames has not been claimed.
- A no-full-frame-recording Slayer baseline adapts the original small synchronous
  HUD observer, reading only the OCR strip at word boundaries. Its rAF timing
  includes that observer's overhead. Task evidence `tutorial_baseline.cjs`,
  `slayer-baseline.cjs`, `run_idle_slayer.py` queues baseline then another async
  pilot under the same private runner lock, avoiding task build contention.
- Local Unity build/test permission was requested because AGENTS requires it;
  no affirmative authorization has been received and no local heavy job runs.
  Ubuntu work continues independently. No final assets/demo/commit/push yet.


#### Smooth tutorial checkpoint: all source cadence gates and materialized assets

- Corrected source-refresh receipt: private `112239-quick` contained source hash
  77b32c67 but ran only9 tests with stale compiled line numbers. It failed one old
  compact assertion and is invalid evidence for the latest source. Git archive
  preserves commit timestamps; replacements now receive fresh archive mtimes to
  force Unity refresh without changing source bytes/hash. `112727-quick` on the
  same byte hash ran10/10 PASS (36.2702277s), XML `tests-112727.xml`.
- Both pilots now meet the unchanged cadence gate. Regular idle pilot remains
  30.040968fps/P95 39.525ms/max51.34ms,241/241. Full parry/Slayer idle pilot
  `slayer-pilot-raf-idle`:30.055152fps/P95 42.415ms/max94.545ms,488/488,no error;
  max callback56.94ms. Nine continuous real words, real fourth Space/Guard break,
  natural exit and full normal word after exit. Separate no-full-frame baseline
  `slayer-baseline-raf-idle`:56.914490fps/P95 16.67ms/max83.33ms,941 rAF samples;
  observer reads small HUD plus380x32 word strip in rAF, PNG only on OCR request.
  Earlier non-rAF-strip baseline returned blank words and was rejected.
- Fresh accepted captures: `capture-typing-03`76frames,30.065784fps/P95 39.11ms/
  max56.595ms; `capture-mistakes-03`120frames,30.084616fps/P95 40.65ms/max49.54ms;
  `capture-parry-01`218frames,30.187398fps/P95 39.93ms/max97.485ms;
  `capture-score-03`401frames,30.007997fps/P95 41.34ms/max88.02ms. All request counts
  equal completion counts, no readback errors. Parry reduces Guard1->.76, released
  Space allows next HP loss. Score shows40->52 on word5,65 on word6, then combo
  expires while65 stays. Full raw PNGs, frames.json, reports and input marks kept.
- Packer unchanged (`tools/game_loop/tutorial_pack.py`). Final selection:
  typing72/2.4052s, mistakes117/3.882835s, parry-health214/7.117645s,
  slayer487/16.203545s, score261/8.70168s:1151 frames,20 atlases. Full640x360 source
  resized once to320x180 nearest. All selected frames/repeats/timestamps kept.
  Direct review caught the closing Pause menu in leading source frames. Excluded
  from selection: typing3,mistakes2,parry3,score3; Slayer0. First clean-frame to
  typing/prefix input stays~335-384ms. Final real source frame supplies the duration
  boundary. Score's unused trailing idle is shortened to1.2s after combo expiry.
  No interior frame is removed and the complete Slayer is retained.
- Before promotion,151 tutorial/scene files backed up in task artifact
  `before-asset-promotion.zip`, SHA256
  e38a7dab1b4c47473bcc7de09dfd95158be9e878d74d541c3ecbb4bb11f894eb.
  Script `promote_tutorial.py` adapts the previous record-preserving projector.
  Preserved1151 spriteID/internalID pairs and existing asset GUIDs. Only scene
  record1477722025 (overlay lessons) changed;1298 other SampleScene records, the
  pre-lessons bindings/layout/listeners and all local references were preserved.
  Only20 PNGs/metas,5 manifests,20 descriptors and SampleScene were promoted.
  Inherited unused meta/descriptor changes and dirty MainMenu/DungeonWorld remain.
- Strict two-pass builder PASS on final media: `115452-serialize` (f26818d9) and
  `120613-serialize` (04bb7379), each invokes the complete Apply twice and compares
  serialized outputs. Old provisional Pause-leading builder114646 was stopped,
  not counted. The old f38 QA build095221 timed out at90min/478 of514 C++ items;
  cache retained. No successful player from it.
- Materialized source snapshot
  `04bb7379fe216b313e2f70d79924f644f13025e30e1930a8e4173bd7cafe6ab4`:
  `120909-quick`10/10 PASS, no failed/skipped (XML `tests-120909.xml`). It includes
  direct per-frame duration/atlas/slot comparisons with the five manifests,
  Skip during fade-out and Start during fade-in assertions. Authoring fixture's
  obsolete fixed1995 total now comes from the five actual manifest frame counts.
  Same snapshot QA `121034-webgl` is building; normal player is not built yet.
- Final-media visual review is IN PROGRESS. `frame-review/frame-evidence.csv`
  records only actually inspected frames. Typing72/72,71/71 internal transitions
  and loop endpoints viewed at400x225 and640x360 nearest; actual loop fade and
  playback1x/.25x still unverified. Mistakes56/117 frames and55 transitions viewed.
  Remaining cards not exhaustively reviewed. Detail sheets adapt the previous
  native400px+actor-crop review template, widening the vertical crop to include
  legs/ground and magnifying nearest4x. Contact sheets do not certify playback.
- OCR service stopped after successful captures, browser contexts closed. Temporary
  venv/scripts/raw evidence remain task-owned under the Ubuntu task root. No local
  heavy job ran, no new demo exists, and no commit/push/merge has been performed.

## Smooth tutorial checkpoint - 2026-10-28, final1083 candidate

This section supersedes the1151-frame candidate and its review counts above.
Implementation/review branch remains `micharel09/tutorial-full-gameplay` from
`c7794ff4`; no main merge is authorized for this task. Worktree and inherited
dirty state are preserved. Task scratch: `C:/Users/ADMIN/orca/artifacts/TypingHero/
tutorial-smooth-work-20261028` (join the two path lines). Ubuntu task root:
`/data/typinghero-game-loop/task-tutorial-smooth-20261028`.

- Final clips: typing72/2.405200s/2pages; mistakes117/3.882835s/2pages;
  parry-health192/6.406265s/3pages; Slayer441/14.713350s/7pages;
  score261/8.701680s/4pages. No interior frames removed, interpolation, speed-up
  or artificial duplicates. Initial selection excludes Pause closure and excess
  idle only; parry cue starts371.67ms and Slayer cue397.61ms after clip start.
  Full cues, contacts, Slayer entry/continuous typing/exit and normal attacks remain.
- Gate passes: regular pilot30.040968fps/P9539.525ms/max51.340ms; complete
  parry/Slayer pilot30.055152fps/P9542.415ms/max94.545ms,488/488 requests.
  Final source captures: typing30.065784/P9539.110/max56.595;
  mistakes30.084616/40.650/49.540; parry30.187398/39.930/97.485;
  score30.007997/41.340/88.020 (fps/ms/ms). All requests completed, no buffer,
  readback or export error. Baselines without full-frame recording: regular
  57.388166fps; full parry/Slayer56.914490fps, P9516.67/max83.33ms. Small
  read-only HUD sampling remains in the baseline; it is not zero observer overhead.
- Reuse: real-input/fixture/OCR capture harness and unchanged `tutorial_pack.py`;
  async recorder `tools/game_loop/tutorial_readback.cjs` adapts the existing
  framebuffer observer and [MDN async readback](https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/WebGL_best_practices#use_non-blocking_async_data_readback).
  Three preallocated PBO/fences, request timestamps, bounded RAM, post-stop PNG
  export. No OCR/file-write wait in frame callback. Five manifests record source
  hashes and resized RGBA hashes for every frame. Raw1303 captured frames retained.
- Final scene projection changes only overlay record1477722025;1298 other records,
  layout/listener prefix, all record IDs and local references preserved.1083
  spriteID/internalID pairs preserved. Obsolete/inactive inherited descriptors and
  meta whitespace changes restored byte-for-byte from initial backup, not staged.
- Circular prefetch and a shared200ms transition clock implemented. A70+70+60ms
  regression test checks carry across the100ms midpoint. Compact test oracle now
  accounts for fractional entry tick, without dropping the wrap assertion.
- Strict repeat builder complete invocations `123030-serialize` and
  `124607-serialize` PASS on1083 media; each invokes full Apply twice and compares
  outputs. Latest materialized snapshot
  `a134fee506948f195ac109fd234d7ff0a429c3c273e76c30a937fb27710093f1`:
  `130218-quick`10/10 PASS. Test-only correction after the second builder does not
  affect its source/assets. QA `130345-webgl` PASS and preserved exact player.
- Direct visual asset review:1083/1083 frames,1078/1078 internal transitions and
  all five loop endpoint pairs viewed at400x225 and nearest zoom. Per-frame
  timestamp/duration/observation/transition/hash/image reference in
  [frame evidence](evidence/tutorial-smooth-20261028/frame-evidence.csv).
  This certifies frame review only; normal-speed and quarter-speed playback
  review remains UNVERIFIED, not replaced by sheets or telemetry.
- Native hardware Chrome GL-EGL/IntelHD4600, no video during cadence measurement:
  four wraps/card at1280x720 (three complete consecutive cycles between wraps),
  two/card at640x360 (one complete cycle). All atlas handoffs and final?first
  observed, no waiting tick after initial load. Independent trace audit checks
  every render lookup using actual serialized float durations; boundary changes
  occur on consecutive render ticks without extra clock hold; both monotonic
  fade phases and full terminal-frame hold pass. Peak2textures/31,477,248bytes
  (30.019MiB),0 after Start at both sizes.
  [Trace audit](evidence/tutorial-smooth-20261028/native-trace-audit.json).
- Original measurement wrapper completed112 scoped assertions, then wrongly ran
  the obsolete six-card Slayer test and failed on missing PracticeButton. Its
  failed report is retained unchanged; no game exception occurred. Corrected
  wrapper skips this unrelated flow. Separate native lifecycle23 assertions PASS:
  pause during fade, no tick/render while paused, Resume through fade-in, Next
  resets alpha/clock, held Enter once, Skip during fade releases0, reopen card0,
  Start during fade saves Completed/releases0, clone has no overlay and combat
  resumes.0 browser/Unity exceptions and0 fixture writes.
  [Lifecycle receipt](evidence/tutorial-smooth-20261028/lifecycle-receipt.json).
- Normal build `133537-production` is running on Ubuntu from the same a134
  snapshot. New demo8314, walkthrough, video review, final packaging and verified
  commit/push are still pending at this checkpoint. No local heavy job ran.

### Review delivery candidate - normal player and media

- Normal `133537-production` PASS, same a134 snapshot. All four JS/data/wasm
  payloads exclude QA symbols/globals;34 player files hashed;16 credits/license
  files preserved byte-for-byte. Normal keyboard/mouse walkthrough PASS6 checks,
  no exceptions/writes. Delivered host itself tested through SSH forwarding from
  Ubuntu:640x360 PASS7 checks, local fixture worker header verified, all five
  cards plus Start. Temporary tunnel closed afterward.
- Final normal stills of all five cards at1280x720 and640x360 directly viewed,
  with Pause, held Enter and started/skipped battle evidence retained. No QA/debug
  overlay, header/rule clipping or GIF bounds issue observed. These are still
  checks, not a claim to have watched real-time playback.
- New demo: http://127.0.0.1:8314/ . Retained independent directory
  `C:/Users/ADMIN/orca/artifacts/TypingHero/tutorial-smooth-20261028`. Start/Stop
  scripts validate owned host PID/path/start time; local fixture intercepts
  service traffic.8313 remains untouched. Keep demo/worktree/evidence until review.
- Walkthrough/review: `/Tutorial-Smooth-1x.mp4`,119.68s/2992frames;
  `/Tutorial-Smooth-025x.mp4`,478.68s/11967frames. Both1280x720,25fps, H264,
  no audio; complete decode PASS, order typing?mistakes?parry?Slayer?score,
  complete loops included. HTTP MIME video/mp4 and response SHA256 match files.
  [Video receipts](evidence/tutorial-smooth-20261028/video-receipts.json).
- `Tutorial-Smooth-Sources.tar.gz` retains1303 raw PNGs/timestamps, accepted
  capture reports, real-input/OCR/recorder/packer/selection sources and QA drivers.
  `Tutorial-Smooth-Frame-Review.tar.gz` retains the final review images and CSV.
  [Archive hashes](evidence/tutorial-smooth-20261028/archive-receipts.json).
  Packaging excludes Unity activation HOME/license, credentials, browser profiles
  and unrelated task data. Sources stay in retained Ubuntu task root as well.
- **UNVERIFIED acceptance: direct1x and0.25x playback viewing.** Available visual
  tools inspect still images; decode, full-frame review and native traces do not
  replace this requested viewing. Implementation and technical verification are
  ready for review, but the task is not declared fully accepted/Done.
- Review-branch commit/remote verification receipt is retained in delivery README;
  no main merge. Only task-specific source/assets/tests/docs are staged. Owned
  capture/QA browsers, OCR services, buffers, build containers and SSH test tunnel
  are closed; only the requested demo host remains running. Retain task worktree,
  private build cache and evidence until review ends, then clean only owned paths.
