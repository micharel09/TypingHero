# Combat hanging lantern ? restored final glass

## Current checkpoint ? 2026-10-21

This checkpoint supersedes historical candidates below. User approved thin glass,
one additional QA WebGL and integration into main; no game publication.

- Original worktree was removed externally during PlayMode. User authorized recovery.
  Restored from `E:/CodexTemp/TypingHero/combat-hanging-light/lowpoly/rework/interrupted-105914-source.tgz`
  into `C:/Users/ADMIN/orca/workspaces/TypingHero/combat-hanging-light-restored`,
  continuing branch `micharel09/combat-hanging-light`. Parent checkout untouched.
- Source FBX remains 472 triangles. Derived frame 446 triangles, glass reuses 40
  window triangles: total 486 submitted lantern triangles (beam separate).
- Glass: URP Lit, black base alpha .12, emission (1,.38,.08), no specular,
  alpha blending, ZWrite off, no cast/receive shadows. Child of LanternModel,
  inside environment hiding/restoration; excluded from flame flicker bindings.
- Key24/range16/flicker0; rear16/range15/+/-2percent. Iron/wood emission0.
  No camera/floor/post/gameplay/public API tuning in the lantern change.
- Correct HDR PlayMode contract: actual camera/presenter, 320x180 HDR Point world RT,
  1280x720, active Volume. Final glass image/effect review in progress.
  First recovery pilot found Unity import removing emission and enabling premultiply;
  builder now fixes GI flags and PreserveSpecular. First pilot is not acceptance.
- QA110751/input046160c165646a5c3788e3b9cf90e90cfb9736e63224afcff7ee2ee742997e34
  player028bdab132b5e1c9: build and strict twice-run full builder PASS.
  Edge155/Intel UHD/D3D11/headed1280x720:22gameplay +9isolated camera checks PASS,
  0exceptions/provider reads/writes. Real attack,hit/parry,pause,Slayer/restore;
  glass hides/restores and retains fixed emission. All9camera endpoints inspected.
  Selected complete pan12.48..18.96s:163/163frames and162/162transitions viewed
  across11sheets; native0/80/162 inspected. No observed alpha pop/transparency
  overlap/floor outline/source drift. Source25fps lossy; no sub40ms/whole-video/
  realtime playback review claim. PTS preserved in motion-review/frames.json.
  Fresh matched20s baseline on current Intel UHD: n245/mean32.12816/p5032.350/
  p9533.225ms; final n250/mean31.38584/p5031.300/p9532.575ms. One run each/RNG/
  observer overhead; not isolated GPU cost, equivalence or FPS guarantee.
  Scene audit:10changed,228removed procedural blocks,8new model/glass blocks;
  no differences outside approved fixture blocks;0existing meta/GUID changes.
- Reuse lifecycle071929:2/2PASS, production runtime components unchanged so far.
  Earlier no-glass QA091233 had19gameplay/9camera checks PASS, visual cage too dark.
  Its 163frame/162transition pan is historical; final glass needs its own review.
- Timing baseline: same Edge155/GTX1650Ti/ANGLE D3D11/headed/1280x720,20s,
  rejected KayKit472/playered2d0d86653698d7: n559,mean13.67229,p5013.340,p9513.525ms.
  One run/RNG/observer overhead; no isolated GPU cost or performance equivalence claim.
- Evidence root: `E:/CodexTemp/TypingHero/combat-hanging-light/lowpoly/rework/glass-final`.
  Remote task cache: `/data/typinghero-game-loop/task-combat-hanging-light`.
  Preserve evidence; remove only task-owned cache after verified integration.
- Integration audit: baseline snapshot fc1bbfd includes unrelated obsolete UI/gameplay
  compared with current main. Preserve current main UI/gameplay. User clarification
  pending on carrying the existing dungeon baseline versus adapting only the lamp.

## Historical checkpoint (superseded)

# M?t ??n ch?nh cho v?ng combat

## Ph?m vi v? baseline

- Worktree ri?ng: `C:/Users/ADMIN/orca/workspaces/TypingHero/combat-hanging-light`.
- Branch: `micharel09/combat-hanging-light`; baseline snapshot `fc1bbfd` l?y t? to?n b? ngu?n dungeon ?ang c?. Kh?ng s?a worktree ngu?n ho?c main; kh?ng push/publish.
- K? ho?ch: thay ?u?c tr??c b?ng ??n khung s?t h?, treo x?ch v?o d?m tr?n hai c?t. T?m l?a t?i t?m ngang combat, tr??c nh?n v?t 2 WU, cao 3.65 WU t? s?n. T?i d?ng holder/node ngu?n s?ng v? GUID c? s?n.
- Ch?nh: realtime point 40/range16, rung 0%; ph?: 16/range15, rung t?i ?a ?2%. Emission c?ng h? s?, pause gi? phase, cinematic disable/enable kh?i ph?c phase.
- Gi? camera, floor, exposure/bloom, gameplay/public API, shadow atlas2048/tier512 ch?nh v?256 ph?; kh?ng ngu?n ?n.

## Ki?m ch?ng ?ang th?c hi?n

- Unity2022.3.25f1/pinned container, cache ri?ng `/data/typinghero-game-loop/task-combat-hanging-light`.
- Hai test ?nh s?ng c? li?n quan PASS (20261021T045916-quick): fixed key v? bounded rear/pause/hiding/emission. Kh?ng ch?y suite r?ng.
- M?t QA WebGL v?i strict repeat builder v? diagnostic clone scene ri?ng, kh?ng th?m mutating API v?o gameplay bridge.
- Native Edge155/D3D11/GTX1650Ti ?1280?720: baseline camera/attack/pause/Slayer11checks v? render timing4checks PASS. Baseline player20261021T041655-webgl/inpute1cf4516; c?c kh?c bi?t serialized scene/post so v?i input l? ??u ra c?a builder.
- C?n: build, ?nh c?ng pose/t?ch ngu?n, camera sweep, runtime final v? so timing; ch?a k?t lu?n visual/performance PASS.

## T?i nguy?n

- Preview/captures/scripts: `E:/CodexTemp/TypingHero/combat-hanging-light`.
- Gi? player/?nh/checkpoint ?? review. Cache/server task ri?ng x?a sau khi archive receipts, kh?ng ?i theo symlink unity-home/toolchain.

## ?i?u ch?nh sau ?nh runtime

- Pilot050027 build/repeat PASS nh?ng superseded sau s?a bounds l?a: ch? d?ng tam gi?c l?a ?? ??t t?m3.65, kh?ng d?ng c?c ??nh c?n ?u?c ch?a x?a kh?i vertex buffer.
- Candidate050651/inputeab0836e build/repeat v?13?nh ngu?n/camera PASS v? capture, nh?ng visual FAIL: v?ng ngang d??i ch?n tia s?ng t?i m?t, t?o d?i t?i; cube UV tr?i c? b?ng m?u l?m d?m nhi?u m?u. Kh?ng d?ng b?n n?y ?? b?n giao.
- ?? m? v?ng d??i (ch? gi? hai c?nh d?c ngo?i v?ng tia t?i nh?n v?t), n?i khung1.35WU v? gi? to?n b? caster s?t/g? th?t; th?m UV mesh d?ng ??ng ? palette s?t/g? trong atlas hi?n c?.
- Candidate050651 camera/gameplay checks ??t tr??c b??c telemetry, nh?ng observer ph? kh?ng g?i samples; kh?ng coi to?n b? script PASS. Observer read-only chuy?n v?o c?ng class diagnostic ?? ho?t ??ng. Kh?ng th?m API gameplay.
- Builder hi?n c? t? ??i38font refs trong scene baseline c?; gi? nguy?n38component UI ?? serialize c?a baseline khi nh?p output. Code font/UI v? baseline/runtime typography kh?ng ??i; builder t?i t?o c?c refs n?y ? c? hai b?n. Scene diff task ch? gi? lighting/khung treo.

## K?t qu? cu?i

- QA052033, input `b1602adbb0abaaaef224521bc109c6fc530fee550a4970d64619e9a968437e20`, player `2ee55b9f1b0faeae`: build v? strict repeat builder PASS. Pilot052033 ch? s?a timestamp observer: tar member tr??c ?? c?mtime0 n?n Unity cache gi? script ch?n ?o?n c?; kh?ng ??i ?nh s?ng/h?nh h?c.
- Native Edge155.0.4283.62/GTX1650Ti/ANGLE D3D11,1280?720:17checks PASS;240samples, key40..40, left15.68462..16.31872/right15.68202..16.30644; pause gi? phase, ngu?n kh?ng d?ch chuy?n, Slayer ?n/kh?i ph?c. Main world position(-.17452,.72245,-1.99105), t?c3.65WU tr?n floor hi?n t?i; kh?ng nh?m v?i world-y3.65.
- Camera b?nh th??ng(640,360), FOV35.000004, actor-scale delta0%; actual hit/attack/3parries/pause/Slayer/restore ?? ki?m.0exceptions/fixture reads/writes, external blocking. Unity2testPASS t?045916 ???c t?i d?ng v? runtime light source kh?ng ??i sau ??.
- Visual ab0035a181492c9a:13?nh (9pose +4t? h?p ngu?n) v?36sample ?2fps trong18s ?? xem. Kh?ng th?y b?ng d?i b?t th??ng/h? c?nh trong m?u ?? xem. Kh?ng tuy?n b? ?? xem m?i frame video. `visual-reuse-proof.json`:287assets byte-identical, scene lighting/geometry identical;38TMP refs v? renderer c?a AttackCue c? ?ang inactive ???c gi? theo baseline trong source. Baseline/candidate ??u d?ng builder font code c? kh?ng ??i.
- C?ng ROI face: hero meanRGB47.21?67.01, skeleton50.48?98.47; enemy weapon21.50?41.15. ROI v? kh? hero51.02?40.90 (background t?i h?n, silhouette v?n r?): kh?ng ??ng nh?t ?? s?ng v?i ?? d? ??c. Full-black world ROI RGB0.
- Combat20s m?i b?n: baseline n488/mean15.798/p5013.350/p9526.765ms; final n428/mean18.262/p5013.400/p9526.865ms. Mean t?ng15.6%; p50/p95 kho?ng0.4%. Final3sample>50ms, baseline0. M?t l??t/RNG/timing combat/observer QA kh?c nhau n?n ch?a quy nguy?n nh?n ho?c ch?ng minh chi ph? t??ng ???ng; kh?ng tuy?n b?60FPS. ?? ho?n th?nh ph?p so theo y?u c?u, kh?ng m? r?ng stress/soak/suite.
- Gi?1578/1578sceneIDs g?c; ch? thay block ??n/mesh/transform/khung m?i. Kh?ng ??i renderer settings/camera/floor/postprocessing/GUID g?c/public API. Hai worktree ngu?n/main kh?ng s?a b?i task.

## Preview v? t?i nguy?n b?n giao

- Ch?i `http://127.0.0.1:8217/`; ?nh/video `http://127.0.0.1:8217/review.html`.
- Local `E:/CodexTemp/TypingHero/combat-hanging-light`: immutable player, before/after,comparison,sources,camera-sweep,preview.mp4,36sample frames, JSON v?Unity receipts; gi? ?? review, d?n khi review/t?ch h?p ???c ch?p nh?n. Preview ch? local, ch?n k?t n?i ngo?i; kh?ng ph?t h?nh.
- Cache remote task ???c d?n sau khi archive receipts v? package v? local; shared unity-home/toolchain v? dungeon-3d cache kh?ng ???c theo symlink ?? x?a.

- Cleanup ?? th?c hi?n: x?c minh kh?ng c?n container d?ng task root r?i x?a `/data/typinghero-game-loop/task-combat-hanging-light`; c?c symlink tr? shared home/toolchain kh?ng b? ?i theo. Local preview server session29053 gi? cho review; d?ng server v? x?a local temp sau khi review ???c ch?p nh?n. Branch/worktree gi? ?? ti?p t?c/t?ch h?p theo y?u c?u m?i.
- HTTP preview200, t?t c? review routes200/no-store/version2ee55b9f1b0faeae; actual HTTP output.data SHA256 kh?p immutable player. Diff whitespace PASS; source Settings/post/floor/meta GUID g?c kh?ng ??i.

- Cu?i c?ng b? trailing spaces c?a8file YAML asset/meta m?i do Unity xu?t ra; ch? kho?ng tr?ng cu?i d?ng, d? li?u mesh/GUID kh?ng ??i. Ki?m `git diff --cached --check` l?i sau khi stage.
