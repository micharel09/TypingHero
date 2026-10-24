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
