## Reusable world lighting profiles - integrated, 2026-10-23

User requested reusable scene/map profiles for all 3D lighting, rune and combat
pulses. No hardware quality tiers or map-selection UI. Five authored types:
FlameProfile, LightProfile, WorldLightingProfile, CombatLightingProfile and
WorldDefinition. Native materials/VolumeProfile/LightingSettings/URP own their
values; clocks, phases, anchors and UV meshes remain local. Four semantic roles
replace generated object-array indices. [Authoring guide](../game/lighting-profiles.md).

Task branch micharel09/world-lighting-profiles, independent worktree
C:/Users/ADMIN/orca/workspaces/TypingHero/world-lighting-profiles, based on5f371fa.
Original dirty parent checkout preserved. Evidence root
E:/CodexTemp/TypingHero/lighting-profiles-20261022-3vlshh3g.

- Unity2022.3.25f1 / URP14.0.11:20/20 scoped checks,0fail/skip, input
  b23958f4e36fc7a22909bfcd94c1ad64b27397ef800f830279e893b3d2208cfb,
  run20261022T162012-quick. Covers private UV/source-mesh/clock lifecycle, rune,
  real-controller companion maps, viewport release, direct/menu entry, three gameplay
  cycles, compact/legacy/overlap pulses and standalone MetalClash light snapshots.
- Second world: arbitrary names, four independent fixtures, intensity12 variation,
  256x144 target and independent fog. A-to-B-to-A releases prior camera/RT/meshes
  without changing profile JSON. Native Test Framework prebuild/cleanup registration
  independently passed2/2 in20261022T163704-quick, input
  4efb2852409bcf522d66cb36a6190af05cd46517dc35c1b9b3dc054e4ecab79f.
  Fixture scene remains excluded from production Build Settings.
- Authoring audit: intensity19/projected height9, material palette and bloom.14
  survive two full builder applications byte-for-byte; baseline restored afterward.
  Prefab overrides are recorded. No one-time migration seeder ships.
- Before camera integration: QA20261022T162758-webgl, snapshot
  ff11735dc6734c53f9c048857670582f9933f95131f7fbce6a315597173ad790,
  immutable player f72a8fe34ee9b768, WebGL/strict repeated builder PASS. Native Edge155,
  Playwright1.58.2, NVIDIA GTX1650Ti/ANGLE D3D11,1280x720:34/34 checks,
  0exceptions/provider reads/writes. All10frames per lamp;288active samples each.
  Front22.5599995..25.2784576, left15.6800003..16.2807598,
  right15.6800003..16.2647190. Emission gain, fixed roots, actual Pause frame/age/light
  and hidden Slayer phase pass. Actual word attack/parry/following attack/Slayer/menu.
- Pre-integration video hash95bdba41bc13660ed4dec180862c4a87e503868c5a55a1d1287d9a19edbe85f0:
  exported563/563 encoded frames with exact timestamps. Directly viewed72/72frames
  and70/70pairs, native320x180 and nearest2x,18paired sheets. Normal F64..91
  (7.219..8.786s), including incoming hit/recovery; parry F137..180
  (11.452..14.397s), preparation/contact/counter/following word attack/recovery.
  Wall/floor pools and player readability were observed; cage/root position retained.
  Palette acceptance from that review is superseded: later source/material audit
  found zero migration thresholds, confirmed by reopening after.png (pale-only fire).
  Earlier claims of preserved red/orange layers were incorrect. Native after/parry/restore stills and
  real1x/quarter playback checked. VFR lossy capture does not prove every engine
  frame, all563frames, pixel-identical GPU output or performance. Per-frame notes
  remain in selected/frame-evidence.csv. Source atlas remains unchanged:
  539be2b093203e571c5d30f49c8ece31f8769f07c0fa08496c9a97e6d1878912;
  reuse prior complete10source-cell/10transition review above the historical record.
- New main3710883 MetalClash: preserve ImpactVfxProfile as sole standalone pulse
  tuning owner. World combat profile owns templates/overlap/decay/ordinary+legacy
  events; parameters are snapshotted when emitted. New main2e77c70 camera impact:
  preserve its contact scheduler/priority/foot anchor/Pause behavior. Camera framing,
  pitch and recoil pixel size now read the active world profile; legacy no-arena
  camera fallback uses its actual pixelHeight, without a fixed dungeon target.
- Integrated input01543b8b82adc03a484b212287866b318802b54473b31e301a1df419fe86466a,
  run20261022T164856-quick:19/19 passed,0fail/skip. CameraImpactTests14,
  direct-entry3, projection/RT1 and three-cycle lifecycle1. Repeated-builder authoring
  audit passed again. Scene record IDs preserved: World1030original+1rune light-data,
  MainMenu477, SampleScene1024. Reordered serialized records retain equivalent data.

Final integrated evidence (supersedes earlier palette approval):

- Additional native fixture checks20261022T165821-quick, input
  e151dd0e4b406155aa6f04b43181ab08776e42ee59ee8425676e513b6dd3e976:
  4/4 passed,0fail/skip. Light-only profile hides the existing card without cloning
  a mesh; irregular frames, world switching and existing pixel-lantern lifecycle pass.
- Final QA20261022T165927-webgl, input
  c25b03c0040fe852621521f45b6d41d93354f427a77f2e7f6802252e2c425626,
  immutable player a484cfa7f3a82e38. WebGL and strict repeated builder PASS.
  Material thresholds .03/.12/.3 match the original shader and survive serialization.
  Local scenes match generated records exactly after whitespace/order normalization;
  all original IDs retained. Profiles/prefabs/material equal the built inputs.
- Native Edge155.0.4283.62 / Playwright1.58.2 / NVIDIA GTX1650Ti / ANGLE D3D11,
  1280x720:34/34 real-input checks PASS,0exceptions/provider reads/writes. Actual
  pause/resume, word hit/parry/following hit, Slayer/restore and menu return pass.
  All10frames each lamp;289active samples each. Front22.5599995..25.2697334,
  left15.6821041..16.2718716, right15.7007704..16.2819023. Emission, fixed roots,
  Pause frame/clock/light and hidden Slayer phase pass (1e-5float boundary tolerance).
- Final video SHA2563b24553c9e9c2f07f106ab4e182f26ebe911f3a1693c61c976ef0870b7f31dc6:
  exported651/651 encoded frames with original timestamps. Directly inspected all
  108selected frames and106consecutive within-segment pairs in28sheets, native320x180
  and nearest2x640x360. Normal F70..108(5.277..6.869s) includes enemy preparation,
  incoming hit and recovery; parry F165..233(9.187..12.106s) includes prior word hit,
  complete wind-up/guard/contact/counter/recovery, following word hit and return.
  Red/orange tongues and bright cores change locally; cage/root stays fixed, brick
  and floor pools persist, player remains readable through every reviewed recovery.
  Native after/parry/restored stills inspected. Playback controls ran at1x/.25x and
  advanced time; visual evidence is the complete selected sheets/stills. No whole651
  frame/every-engine-frame, pixel-identical GPU or performance claim.
  Per-frame notes/timing: integrated/selected/frame-evidence.csv and scope.json.
- QA observer removed from checkout. Production Build Settings retain only the
  three production scenes. No package, gameplay balance, UI or reflection change.
  Atlas/mesh/animation/controller files and existing GUID metadata remain unchanged.

Remote integration verified at73528adb3df60e2b148ed6bf6539f02070bbfff5 on
micharel09/TypingHero main: task HEAD equals fetched origin/main and the task commit
is its ancestor. Main2e77c70 is preserved. This final receipt changes documentation
only. Remove the clean task-created worktree/local branch after receipt ancestry
is verified; retain the remote branch and original dirty parent checkout.

Keep local evidence root for replay/reproduction; cleanup is safe when it is no
longer needed. Sixteen owned completed runner result directories were archived
locally then removed after confirming no active Docker mounts. All task jobs and
headless browser sessions exited; no new preview server was started. Shared
source/cache/toolchain, older preview server8228 and pre-existing resources remain.

Earlier failed migration/verification attempts are archived separately: missing
initial bake reference; Editor assembly refs; stale shared C# Bee graph; fixture
registration/prefab override errors; Editor-only bakeType player assignment.
The initial camera merge compile referenced removed WorldHeight; replaced it with
active profile height / actual no-arena pixelHeight. Partial/failing runs are not
claimed as passing. Shared cache/source and other jobs are preserved.

## Approved integration checkpoint - 2026-10-22

User approved final fire commit297c0c5 and requested integration into main. Merge current main916c752, preserving the new separate tutorial callouts and longer parry ember floor hold. Only documentation conflicted: retain current tutorial/VFX descriptions and approved fire lighting description. DungeonWorld merge changes only blank-line normalization outside the fire records. Fire runtime, shader, atlas, material, geometry, builder and focused tests are unchanged: reuse the recorded2/2Unity,35/35native and complete selected-frame reviews; no additional player build is needed. Run diff whitespace/conflict checks before push. Asset author/license remain unknown; user approval does not establish distribution rights. No game-host deployment requested.

Remove the clean task worktree/local branch after remote ancestor verification. Preserve remote branch, original dirty checkout and local evidence under E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t. Keep local demo server8228 available for replay; it can be stopped when replay is no longer needed.

# Combat hanging lantern ? restored final glass

## Pinterest flame - user approved, main integration, 2026-10-22

User approved https://www.pinterest.com/pin/1135681231035570556/ after rejecting both the unreadable NYKNCK runtime and the Pixie composite's art style. Continue the same isolated task worktree. This supersedes the visual direction below; earlier technical receipts do not verify this replacement. User approved the final real Unity demo and explicitly requested main integration. Source attribution/license remain unverified.

- Source GIF SHA256 `ac07257c81d688591ebbe67e309f80f37aeee4adcd06a1ccd75407b492e26d37`:10frames x100ms, complete1sloop, transparent500x500 image, exact5x expansion of100x100. All10frames and10transitions including wrap visually compared at100/200px, plus full source F0. Per-frame observations/source metadata: `E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t/pinterest1135681231035570556/reference-review.csv` and `source.json`. Original GIF timing is recorded from its100ms frame metadata; final runtime comparison and playback-control receipts follow below.
- Uniform source crop(22,21)-(76,101),54x80cells,5x2atlas270x160. Source colors/order/durations retained; material maps its four colors to brick red873B2B,dark orangeB6532C,warm orangeE48B3A,pale yellowF4CC79. Fixed bottom,80percent width, projected maximum height front10/rear12native world pixels at the existing camera.
- Reuse existing actor draw pass after environment blur with depth occlusion. Front24/6percent, rear16/2percent, Perlin/emission/shared lifecycle clock, per-lamp phase offsets unchanged. Builder and saved scene must agree before handoff. Runtime private UV meshes release on destroy; authored source assets/GUIDs remain intact.
- Scoped Unity20261022T132647-quick input18830d886955398a5f9a2885a620e7d9ba16910fec7660b6120467ecbc6dcd90: strict repeat builder and2/2existing focused light tests PASS. All10final raw cells/10loop transitions inspected at54x80and2x; Final GPU palette/runtime review is recorded below. Saved scene retains1022original IDs:7modified records+8new rear-card records equal verified builder after local-ID normalization; all other saved records retained. Material/quad equal builder ignoring trailing whitespace. Initial immutable QA build completed; final size-corrected evidence follows. No publication claim. Pin attribution/license absent; local review candidate only until external distribution rights are established.

### Final local verification

Pinterest size correction: initial QA player b2f131fd1d776174 passed35real-input/telemetry checks,0exceptions/provider writes on Edge155/Intel UHD/D3D11/1280x720, but direct native/4x closeup review found7/8native-pixel flames too small to read their silhouettes. Preserve this failed visual candidate in pinterest1135681231035570556/small-first-candidate. Increase only projected geometry to10/12native pixels; source atlas/timing/palette, runtime clock and physical lights unchanged. Reuse passing2Unity lifecycle tests; rebuild and recapture only affected visual integration.

Final local candidate (2026-10-22):
- QA20261022T140347-webgl, input70136ae1860409e6a97567e3d27d31993f318e71fc62b6d519d2f1aa72dd0050, immutable player5b494ce8e2ac2083. Strict repeated builder PASS. Reuse2/2focused Unity lifecycle checks from132647: runtime code, source atlas/timing/palette unchanged; only Editor projected geometry changed from7/8to10/12pixels after the initial visual failure.
- Native Edge155.0.4283.62/Intel UHD/ANGLE D3D11/1280x720:35/35PASS,0exceptions/provider reads/writes. Actual keyboard pause, parry, following word attack, Slayer/restore and menu return. All10cels observed on each lamp; active samples282each. Intensity main22.5599995..25.2148552,left15.6839170..16.2656498,right15.6800003..16.2859631;1e-5float boundary tolerance. Emission, fixed root, actual pause frame/clock and hidden-Slayer phase checks pass for all3.
- Full canvas export634/634frames with exact source timestamps. Selected complete normal-camera segmentF64..102(5.68129..7.302195s), including incoming hit/recovery, and parry preparation/contact/recovery/following attackF162..226(9.774305..12.774305s):104/104frames,102/102adjacent pairs viewed at320x180and nearest2x640x360. Also39/39normal native-capture2x three-lamp crops and1280normal/parry/restore stills viewed. Per-frame notes: pinterest1135681231035570556/selected/frame-evidence.csv. Video SHA256461cce307e4f1a1441df3a1d22ac224595bc26357a3e83ca04258706d72ddfc6.
- Red tongues/core change locally while fixture roots and warm wall/floor pools persist; player returns to normal illumination after hit/parry. Cage depth occlusion retained. F101 shows brief capture pixel-grid resampling across scene/cage; no claim of general shimmer absence. VFR25FPS lossy recording does not prove every engine frame, whole634frame clip review, or FPS/performance. User approved this final demo and requested main integration on2026-10-22.
- Final saved scene:1022original records/IDs retained;7changed+8new task records match final builder after local-ID normalization; material/quad equal generated values ignoring trailing whitespace. No package/camera/UI/floor-reflection changes. Disposable QA observer removed from checkout and retained only in evidence.
- Actual Unity demo: http://127.0.0.1:8228/pinterest1135681231035570556/demo.html (Orca page b275cbdb-b38f-48b6-93cf-b2724696e722). Video/closeup media and1x/quarter playback controls verified; default restored to1x. Retain local review server8228 and evidence root E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t until review finishes.
- Retain worktree C:/Users/ADMIN/orca/workspaces/TypingHero/lantern-pixel-fire and branch micharel09/lantern-pixel-fire for user review. Main integration remains held, and external asset distribution rights remain unverified; no new branch push/main merge/publication. Original dirty parent checkout preserved.

Resource cleanup: owned result directories132647-quick and140347-webgl removed only after local full-archive snapshot/player validation and confirming no active container mount. Full archives remain under the Pinterest evidence root; shared source/cache/toolchain, other jobs and prior resources preserved. All task check/build/capture jobs have exited; retain only the review server8228/browser tab/worktree/evidence until user review and source-rights resolution.


## All three pixel flames — review candidate, 2026-10-22

**User review rejected this NYKNCK candidate:** flame is not visibly readable in the playing camera. Prior technical/frame checks below do not establish accepted art quality. On2026-10-22 the user selected `https://pixie.haus/pixelart/20470/warm-glowing-ember-flame-animation` as the replacement direction. Reuse the existing branch/worktree; do not commit/push/integrate the rejected candidate as final. New target: a clearly readable upward flame with bright yellow core and orange/red tongues on all three fixtures, visible at the current playing camera. Inspect the complete linked animation and keep its source/output review separate. Main integration remains held for a new demo.

- User extended the NYKNCK free flame to both rear torches; retain the current task worktree and hold main integration until the expanded demo is reviewed.
- Shared unchanged five-cel atlas, 100ms/cel. Private animation offsets: front0s, left0.17s, right0.33s. Front24/range16/6percent, rear16/range15/2percent; all use continuous Perlin and synchronized per-lamp emission.
- Rear visible bodies reuse the existing fire-free shadow mesh; original FBX/materials and existing shadow proxies remain intact. Each new flame card fits its original fire-only world bounds, with a fixed bottom pivot. Shared atlas/quad/material, private runtime UV mesh per torch.
- Builder excludes existing PixelFlame renderers from torch-source extraction, avoiding recursive shadow children on repeat builds. No gameplay/UI/camera/floor reflection or public API changes.
- Evidence: `E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t/all-torches`. Scoped Unity2/2 and strict repeat builder PASS; QA playerf6eb222b78eb434e completed real input flows with zero exceptions/provider writes. Corrected read-only telemetry validation passes all3lamps (483samples each); raw aggregate-harness limitations are recorded in game/verification. Reviewed109/109frames and107/107pairs, native world and2x, plus all40normal three-lamp native-capture2x crops. Source atlas unchanged; prior5cel review reused. Full receipts/limits are in game/verification. The additional exact-saved-scene Unity audit is waiting for the shared runner; static seven-record/eight-node patch retains all original IDs.

## Current checkpoint ? 2026-10-21

Remote integration verified at `f1ab52b70eac8b2441abc1926b7dc0ec2d2d1e03`.
Both task303eeaa and latest main e36b02d are ancestors of remote main. Task branch
matches remote main. Parent dirty checkout remains untouched. No game publication.

Cleanup:28Unity logs/snapshots/XML archived in
`E:/CodexTemp/TypingHero/combat-hanging-light/lowpoly/rework/glass-final/integrated/retained-unity-receipts.tgz`.
Removed `/data/typinghero-game-loop/task-combat-hanging-light` only after acquiring
its exclusive lock and confirming0active owned containers; shared unity-home and
toolchain symlink targets remain. No task8217/8237preview listener remains. Keep
local players, images, selected video/frames and audit JSONs for review/reproduction.
The clean restored task worktree and its unused auxiliary restored branch are
removed after this receipt is verified on remote. The pre-existing task branch
and remote branch remain; their history is preserved on main.

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
  1280x720, active Volume. Final glass7stills reviewed on110516/inputbfa57a81;
  settled off/on0changed pixels outside590,140..690,275cage ROI.
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
- User integration decision: latest main owns gameplay/UI; this task owns the full
  dungeon3D/camera/lighting/lantern. Preserve newest parry/impact systems, HUD/font,
  score rules and typing snapshots. Keep only camera-coordinate adapters in UI.
- Integration boundary: main f16ca37 plus subsequent1004724 documentation; world
  from303eeaa. Main nonworld scene blocks preserved, including stripped prefab
  records. Replace only old3D roots/cameras/global3D lighting, then run the main UI
  builder plus task dungeon pass. Keep main transient ParryContactLight binding
  because newest parry presentation consumes it; three steady sources unchanged.
- Integration source requires existing URP Core Runtime/Universal Editor assembly
  references from the dungeon baseline. Initial compile failures found omitted
  assembly refs; corrected. No new package dependency.
- Strict builder113353PASS, then final QA114427 strict builder PASS on
  input9a9081df02bf07286e197c4cf9ec1e5c3f0c49008fbc838b46fc8d47455dbc97.
  Snapshot canonicalizes YAML/shader CRLF to Git/Linux LF; font protection unchanged.
  Five existing main GUIDs preserved and seven serialized references updated.
  Earlier114008QA attempt was deliberately stopped before completion for this fix.
- Targeted offline results:1136554/4PASS (camera/RT lifecycle, fixed main, bounded
  rear and parry pulse),113808HUD lifecycle PASS but HP old231-width expectation
  failed against the approved171-unit rail. Main CombatR7Build explicitly owns171;
  updated only that exact width expectation, retaining0/1/50/100percent fill checks.
  Corrected HP rerun1/1PASS. Six distinct scoped checks PASS, no full-suite claim.
- Run strict full builder, targeted3dungeon lifecycle/projection tests and one
  integrated QA native camera/gameplay/HUD/projection smoke. Prior final material,
  163frame pan and20s timing remain evidence for the original isolated candidate;
  do not relabel them as final merged-source evidence without identity checks.



## Final integrated verification

- QA114427/input9a9081df02bf07286e197c4cf9ec1e5c3f0c49008fbc838b46fc8d47455dbc97,
  playerfdc8253ed7a62b1f: strict twice-run builder and WebGL build PASS.
- Edge155.0.4283.62/Intel UHD/ANGLE D3D11/headed1280x720:25real-input checks PASS,
  0exceptions/provider reads/writes. Attack/hit/parry, pause, Slayer/restore,
  three steady sources, fixed glass emission and glass hide/restore verified.
  Returning through the actual pause/MainMenu button releases all owned3D RTs
  and presentation cameras. Seven native state/menu stills directly inspected.
- Final scene, actor prefabs, UI/VFX outputs match actual QA serialized output
  after newline/trailing-whitespace canonicalization;0semantic differences.
  Existing main asset GUID changes0. Gameplay/UI/data/prefab/MainMenu paths match
  latest main exactly; only viewport/projection adapters change UI source.
- Main advanced during QA with documentation-only commits through e36b02d;
  preserve those receipts. No gameplay/render input change since QA.
- Reuse final glass material/pan evidence: active3D assets/settings/render code
  match303eeaa after GUID preservation and whitespace canonicalization, except
  sub6e-8legacy _Color round-trip values (URP uses unchanged _BaseColor).
  Root/camera/lantern transforms are exact matches. Prior163frame pan and20s
  timing remain isolated-candidate evidence; no integrated performance claim.
- Latest gameplay/main parry presentation retained, including its transient
  ParryContactLight; the steady lantern/rear lighting remains task-owned.
  No production build, itch upload or game publication.
- Integrated source committedc58beed, latest main e36b02d merged as1fa5ee2.
  Automatic merge added a duplicate, identical groundY prefab override; kept the
  already-verified single override. Runtime/build input diff c58beed..1fa5ee2 is0;
  only documentation changed, so passing QA is reused. Final diff versus main
  has0unexpected gameplay/UI paths and passes whitespace checks. Push/remote
  verification and task-resource cleanup follow these checks.

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


## NYKNCK free pixel flame ? 2026-10-22

- User selected the free five-frame Fire Set, not paid Bonfire. Source: https://nyknck.itch.io/pixel-fire-set; credit @nyk_nck; source archive SHA256 08b78ec0fe4ee9f4d5627e57f0a7013ea7edbc3fb279cb320f9f83ee0ecd091b.
- Task branch micharel09/lantern-pixel-fire, base origin/main 4e62ea2. Preserve the dirty parent checkout.
- Isolate each largest 8-connected alpha body on the authored native pixel grid, bottom-anchor in 48x34 cels, keep original colors and 100ms timing. Main lamp only: private runtime quad/UV flipbook and continuous bounded Perlin noise; base24, amplitude6percent, emission synchronized. Rear sine/+/-2percent behavior unchanged.
- Original lantern/mesh/material GUIDs retained. No gameplay/UI/floor reflection changes. Full source and final output frame review remain separate.
- Acceptance pending: scoped front/rear Unity tests, strict repeated builder and persisted DungeonWorld configuration, all five output frames/transitions at game size/2x, one short real-input gameplay capture including parry/recovery and pause/Slayer restoration. No broad suite/multi-viewport/stress acceptance.
- Local evidence: E:/CodexTemp/TypingHero/pixel-fire-review-20261022-m60wlc3t. Source five frames/transitions inspected; paid promotional preview is excluded from output. Runtime verification is not yet complete.

### Pixel flame verification

- Unity2022.3.25f1 scoped Quick20261022T081730: **2/2 PASS**, front lifecycle/mesh/emission/clock test and directly affected rear torch regression. Strict repeated builder PASS on input195216041bd12cdd99291d0b9827c5fa31c4ea79ab69565cad5f940cf8034c3b.
- Initial Windows CRLF-only font protection failure was resolved in the disposable Linux snapshot (canonical text LF; binary bytes retained), not by changing font assets or weakening guards. Earlier material GI/alpha-mask and room renderer-enable idempotence failures were fixed in the new flame builder; failed receipts retained locally.
- QA20261022T082242, inputcf8366fc823770ecf1ef786e66facf8a37bef5ff0fb648a78c55a65350ae00f4; player5f849262aed3f42e. Native Edge155.0.4283.62 / NVIDIA GTX1650Ti / ANGLE D3D11 at1280x720: **21/21 PASS**,0exceptions/provider reads/writes,481read-only lamp samples,186hidden. Observed active intensity22.5750618..25.2119675, all5cels; shared emission, root, pause and Slayer phase verified. Shipping runtime/assets unchanged by the QA-only observer, which is excluded from the commit.
- First browser pilot is INVALID for gameplay acceptance: tutorial skip reloaded the encounter after its early Enter. Fixed only the harness settling/wait; reused the same player for the passing run.
- Source GIF original timing is5x100ms. Final adapted atlas240x34 SHA25614b26a8dc4566e7c7079107054cfbcc9056432b0b33c3fe3a9cb857cf7cce910; no interpolated colors. Source reference review and output review remain separate.

| Output cel | Duration | Source | Observation at native48x34/nearest2x | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|
| F0 |100ms|1.png, largest alpha body|Hai c?m ch?nh n?i ch?o t?o th?n l?a nghi?ng; palette v?ng/cam g?c, ??y ? h?ng32.|Sang F1 th?n ng?n l?i theo ngu?n; pivot ??y kh?ng ??i.|??t|PixelFire.png cell0; local output-frame-0.png|
| F1 |100ms|2.png, largest alpha body|Th?n nh? nh?t, d?ng n?m nghi?ng ph?i; m?p pixel v? ch?n c?n trong canvas.|F1?F2 k?o d?i th?nh gi?t; gi? ??y33 v? t? l? pixel chung.|??t|PixelFire.png cell1; local output-frame-1.png|
| F2 |100ms|3.png, largest alpha body|Th?n gi?t d?i m?u v?ng cam, ?u?i d??i nh?n; kh?ng clipping.|F2?F3 r?ng th?m, c? v?ng t?i gi?a th?n theo ngu?n; ch?n kh?ng d?ch.|??t|PixelFire.png cell2; local output-frame-2.png|
| F3 |100ms|4.png, largest alpha body|Th?n r?ng c? l? t?i v? ??m ch?n n?i ch?o theo alpha g?c; kh?ng v? th?m pixel.|F3?F4 k?o cao theo frame ngu?n; v?n n?m trong envelope ??n.|??t|PixelFire.png cell3; local output-frame-3.png|
| F4 |100ms|5.png, largest alpha body|Th?n cao nh?t d?ng cong ch? S v? ?u?i cam; ??y c?ng h?ng32, gi? palette g?c.|Loop F4?F0 ??i silhouette theo ngu?n, kh?ng reset pha ?nh s?ng ho?c ??i pivot.|??t|PixelFire.png cell4; local output-frame-4.png|

- Actual canvas SHA256c4b65aa7482de36f7d93ee8a9144fc012c4eb02b6443e0d50006333bbeec86a3;640decoded/exported with exact timestamp agreement. Before review selected normal6.212475..7.830410s(F77..116) and first parry9.834410..12.234410s(F166..223), covering windup/contact/recovery and a following normal hit. **98/98selected frames and96/96adjacent transitions actually inspected** at logical320x180 and nearest2x640x360; all40normal lamp crops additionally inspected at2x capture. Actual1280x720 after/parry/post-hit/restored stills also opened.
- Warm light remains on 3D floor/actors, fire varies inside a stationary cage; no full-room flash or whole-player recovery blackout in those segments. Source five-cel silhouette changes are retained. VFR25FPS capture is not every engine frame/high-frequency shimmer or performance/FPS evidence. No full suite, extra viewport or stress run.
- Full builder also regenerates unrelated legacy scene records; committed DungeonWorld instead preserves all1018original IDs and every out-of-scope record. Only old front renderer, front holder child list and front torch binding/settings change, plus4new pixel-node records. New-node transforms match generated output; original materials/meshes/GUIDs remain unchanged.
- Local frame-level runtime observations: selected-review/frame-evidence.csv (98rows), source/output hashes and scope.json; source animation review has its own source-frame-review.csv. Retain local evidence until integrated result is reviewed; no public game publication.

- Direct committed-scene load audit20261022T084400-serialize PASS (input5e6eb9b5d04ad1a0020033c53c55a8b9f303f60d94f031d2c4ac109913b49f95): Unity opens the exact scoped DungeonWorld scene and asserts3torches, front24/.06, rear.02/no pixel binding,4vertex private-quad template, renderer enable/array links, old flame disabled,240x34 Point/no-mip atlas. This is a read-only scene audit, NOT another full repeated-builder run despite the stock runner completion label. Initial optional audit wait timed out on the shared lock; retry passed, other jobs preserved.


## Complete 3D configuration - 2026-10-23

- Scope: blur, haze/rune, cloth, camera impact and multi-world builder. Preserve
  source atlases/meshes, combat and Point/HDR/MSAA. Orca branch
  `micharel09/finish-world-configuration`, base `3253274` from latest origin/main;
  original dirty checkout preserved. Retain main changes after plan baseline bab5ee6.
- Blur migration is explicit in production/fixture assets: offset .75 -> center 0,
  half-width .75, depth edge .5; unchanged Gaussian/taps, no builder migration.
  Haze/rune use existing materials. Cloth captures profile values on enable.
- CameraImpactProfile retains approved defaults; scalar envelopes/duration snapshot
  at emission, curve copy at binding, no objects per hit. Gameplay binds selected
  virtual lens/profile before activation; two-profile A -> B -> A test added.
- Builder uses gate definition and serialized owner/references, applies worlds
  reachable from enabled scenes, supports assets outside the lighting folder and
  ignores unused templates. Repeat/export include actual dependencies. Preserve
  authored camera/room placement and restore scene setup.
- Initial Unity `20261023T031358-quick`, input
  `06f699768bfb430ccf0bf808e186fe40540ede4a88d3b1c1744b98a4b6d4dcd7`:
  42/43 PASS, 1 FAIL. PlayerControllerCelsHaveAlignedNormalAndMaterialBindings
  still expects old male_hero-combo_2_end.png recovery, removed by current main;
  no normal-map/controller/atlas change or weaker assertion in this task.
  Relevant camera/environment/bindings/session cases pass, including zero managed
  bytes after warm-up. The aggregate run is not PASS.
- Editor pilot `20261023T031700-serialize` failed restoring empty scene setup in
  batch mode. Empty setup handling fixed; additive scene setup/placement checks added.
  Final Editor/filtered runtime/WebGL/native visual acceptance remain pending.
- Local evidence/adapters: `E:/CodexTemp/TypingHero/finish-world-configuration-20261023`.
  Remote temporary source/build/results: `/tmp/typinghero-world-configuration-20261023`.
  Shared lock/cache/images/license preserved. Only this task's doctor source/archive
  removed after marker/mount verification; old results untouched, thresholds unchanged.
  External adapter uses testFilter and canonical text LF without changing font content.
  Waiting on other jobs holding the shared runner lock; do not stop unrelated jobs.
- Integration and cleanup are recorded in the final local `acceptance.json` receipt;
  historical failed pilots below/above do not count as acceptance.

- Second Editor pilot `20261023T032714-webgl`, canonical input
  `6a3dd2b2fdaa63562e4a038100393fb19fc01c5230478a7ce271a733af5ea322`,
  exposed legacy HUD camera Y overwrite (+.110096929). Authored room stayed fixed.
  Default world pose is now captured before UI passes and restored by the 3D pass;
  no camera pose is computed from dungeon-specific geometry. Preview Undo now also
  records affected combat rig components. Final repeat/runtime/WebGL rerun queued.

- Final source input `6b1258a53256b28cc07ab6bf716e8cf40ca994514d8ad31d31c7172f665299fa`,
  run `20261023T033400-webgl`: Editor authoring/additive setup/arbitrary naming/
  selected outside-folder assets/unused-template/placement checks PASS.
  Filtered PlayMode39/39 PASS,0failed/skipped,9.9696019s. Strict repeated builder
  PASS; WebGL linker and native acceptance still pending. Runtime/config source
  is frozen for this candidate; reuse scoped evidence while it stays unchanged.

- Final WebGL/strict repeated builder PASS on the same6b1258a5 input. Downloaded
  immutable player33/33 SHA256 matches,48,078,641bytes. Native Edge155.0.4283.62 /
  Playwright1.58.2 / NVIDIA GTX1650Ti ANGLE D3D11,1280x720:53/53 PASS in37.7076506s,
  zero exceptions/provider reads/writes/external requests. Includes accepted normal/
  received/parry, positive active-pulse Pause freeze, Slayer/lethal, actual typed
  post-parry follow-up, Restart and Menu restoration. First driver attempt paused
  too late (pulse already0); retained as failed, timing corrected using real keyboard.
- Direct visual review COMPLETE:46/46 sheets;355/355 frame observations and350/350
  adjacent pairs across5windows (340unique frames/336unique pairs due to overlapping
  normal/received windows). Raw video1048frames; source PTS windows0-1.148s(F0-18),
  .163-1.682s(F4-43),13.201-14.974s(F507-581),18.897-20.673s(F735-805),
  22.827-25.997s(F898-1047). Native320x180 and nearest2x
  640x360 shown side-by-side; held/repeated frames and sheet-boundary pairs retained.
  No observed change to warm flame colors, anchored fixtures, floor/wall pools,
  focal actor clarity or room restore. Cloth is dark/subpixel; deformation/pinning/
  independent local direction/private mesh/phase rely on the scoped Unity lifecycle test.
- VFR timing limit: source F10(.302s)->F11(.950s) has648ms gap; keep it, do not
  interpolate or infer every-engine-frame/FPS behavior. Warm F1008-1031 provides
  full normal-hit/recovery motion. Reuse unchanged source atlas/mesh/clip review;
  baseline camera-impact native sheet and Pause image compared. No full-video,
  new reference-match, audio-signal or performance-improvement claim.
- Evidence root: `E:/CodexTemp/TypingHero/finish-world-configuration-20261023`.
  Final receipt `acceptance.json`; scoped Unity XML/editor/build logs under
  `20261023T033400-webgl`;53-check report/video under `native`; per-frame observations
  and exact source windows under `native/frame-review/manifest.json` and
  `frame-evidence.csv`. Video SHA256
  `089b3c6e2638053c487a852bb560baad7e74c0971d803ea068e3d56debbf5619`.
  Keep these evidence/player artifacts for reproduction/review; delete after review
  when no longer needed. Disposable task source/worktree/profile cleanup is required
  after verified integration; final receipt records actual completion.

- Final read-through found multi-scene Preview wrote RenderSettings into the active
  scene. Fixed only Preview scene selection/Undo recording and restored active scene
  in finally. Standalone VerifyPreviewScenes `20261023T035720-serialize` PASS on
  input530e3293: each world's fog correct, active scene retained, Undo restores both.
  Generic runner completion wording does not represent another repeated-builder run.
  Subsequent indentation-only adjustment has no semantic change. Production Apply
  method and all runtime/shader/asset inputs are unchanged from6b1258a5; reuse the
  passing39Unity/53native/WebGL/visual results. `preview-isolation-proof.json`,
  `preview-unity.log` and `preview-snapshot.json` retain exact evidence.

## Preview ownership and complete scene Undo follow-up (2026-10-23)

- Independent Orca worktree `fix-world-preview-undo`, branch
  `micharel09/fix-world-preview-undo`, base `9acdb4e`. Dirty primary checkout
  preserved. No runtime, shader, profile, material, geometry or GUID changes.
- Preview/Apply resolve combat rigs from the world scene plus explicit roots of
  gates selecting that exact definition. Distinct targets are shared with Undo;
  unrelated gates/unbound rigs retain their profiles. Changed gameplay scenes
  are marked dirty and prefab modifications recorded.
- One Preview creates one Undo group including each scene's native
  LightmapSettings reference, RenderSettings and affected combat rigs. Null
  profile baking retains the existing scene reference. The cached internal
  LightmapEditorSettings.GetLightmapSettings contract is pinned to Unity2022.3.
- New Editor checks copy world/gameplay scenes and styles into a temporary folder,
  then restore scene setup and delete only those copies in finally. Covers A/B
  gate selection, detached/duplicate roots, local/unbound rigs, grouped Undo/Redo,
  prior and null bake references, unchanged authored JSON, scene dirty/setup and
  save/reopen persistence. Existing multi-scene fog/active-scene test is retained.
- Reproduction `20261023T063030-serialize`, input
  `b3b76993724dd4b2a66c740098ed40a0531bf50f125e7a9d6fa8a6820ce2a859`,
  executes the unchanged base authoring code with the regression checks: FAIL as
  expected, confirming missed split rigs and missing bake-reference Undo.
- First fixed-source pilot `20261023T063235-serialize` still executed the cached
  old Editor DLL: CombatRigs was absent despite the new source. Not acceptance.
  The external snapshot adapter advances only this source file's timestamp after
  baseline injection; no source/test assertion or build threshold was weakened.
- Accepted pre-integration input
  `98a91cc6109419f190e5046bca6003b2fd22c7ce33063477d7541d73e5a00c2c`,
  run `20261023T063454-serialize`: new Preview regression checks PASS, existing
  multi-scene fog/Undo PASS, strict PixelUIBuild repeat PASS with both worlds
  selected. The compiled Editor DLL contains CombatRigs. No runtime/full-suite,
  WebGL, frame-review or performance claim; those paths are unchanged by this fix.
- Evidence: `E:/CodexTemp/TypingHero/fix-world-preview-undo-20261023`, including
  `repro`, `fixed-pilot`, `accepted` logs/snapshot receipts and external adapters.
  Server private source/cache/Library/HOME/results are under
  `/tmp/typinghero-preview-undo-20261023`; shared runner lock/images/license/cache
  preserved. Private writable copies avoid the shared filesystem's <3GiB free
  condition while retaining the original 3GiB threshold on the actual filesystem.
- Newer main `94911a8` changes impact pulse lifetime/legacy-light cleanup. Merge
  and scoped Editor/repeat verification are required before remote integration.
