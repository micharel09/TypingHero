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
- Original acceptance (waived by user): scoped front/rear Unity tests, strict repeated builder and persisted DungeonWorld configuration, all five output frames/transitions at game size/2x, one short real-input gameplay capture including parry/recovery and pause/Slayer restoration. No broad suite/multi-viewport/stress acceptance.
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

- Integrated main94911a8 as42ef2a5 without conflicts in source; both authoring-guide
  additions are retained. Post-merge run20261023T063803 (eb875c2e input) passes
  the two Editor entrypoints and strict repeated builder.
- Final fixture explicitly uses the real CombatLightingRig prefab for the detached
  gate root and asserts prefab identity plus the selected profile after reopening.
  This closes the earlier fixture's plain-object-only persistence coverage.
- Final input `00654f10733f1ef8d8d740f3638135e98cb7de7f222b332153663ee7b219876c`,
  run `20261023T064045-serialize`: split/multiple-world Preview/Undo/Redo/null bake/
  unchanged assets/save-reopen including actual prefab override PASS; existing
  multi-scene fog/active-scene/Undo PASS; strict repeated builder with both worlds
  enabled PASS (TYPINGHERO_QA=1). No exceptions or regression markers in final log.
  Final evidence is under `final`; earlier passes/pilot remain attributed to their
  own snapshots. Documentation-only receipt updates do not change tested inputs.
- Remote verification and resource cleanup are recorded in the evidence root's
  `acceptance.json`. Retain logs, snapshot/patch/audit receipts and the remote task
  branch for review/reproduction; they may be deleted when review is complete.

## Camera prefab overrides and strict QA precondition (2026-10-23)

- Independent Orca worktree/branch `finish-camera-prefab-preview` /
  `micharel09/finish-camera-prefab-preview`, based on remote main95f978b.
  Dirty primary checkout preserved. Scope is Editor/test/docs only; existing
  asset GUIDs, runtime, shaders, profiles, materials and world geometry unchanged.
- Apply records prefab overrides for Camera, DungeonArenaCamera and native URP
  camera data even outside environment. The same three camera targets are used
  by Preview Undo; duplicate targets in the environment hierarchy are removed.
- VerifyPreviewAndRepeatBuilder rejects every QA value other than exactly1
  before calling scene/profile/builder code. New checks exercise unset and0,
  compare scene setup/Build Settings and restore the original environment.
- New temporary camera-prefab fixture changes FOV/near/far/half-height, arena
  profile and renderer slot; swaps the two existing renderer references/indices
  only in a cloned profile to keep validation valid. Tests Preview, Undo/Redo,
  save/reopen and direct Apply without Undo; source prefab bytes and cloned
  profile JSON must remain unchanged. Temporary assets/scenes are deleted in finally.
- Fixture pilot20261023T071305, cd096ba0 input: ArgumentNullException before
  camera assertions. Creating a scene after cloning unreferenced temporary assets
  allowed Unity to unload them; scene creation now precedes cloning. Not acceptance.
- Baseline production code95f978b with corrected fixture, run20261023T071522,
  input `bd5ad160038706b2e8d059c3d11768edb469cc4eb805d04bb82a9b129d7702fd`:
  expected FAIL for the combined arena-profile/renderer assertion on Redo and
  subsequent Preview save/reopen. Lens and direct-Apply checks did not fail in
  this environment; do not claim that every camera field was lost on baseline.
- Accepted pre-integration run20261023T071834, input
  `7f41e55138b4b67f732e6c5167c30947e218df2c1d4d9389b7ee7acb5b0e1353`:
  QA unset/zero rejection PASS; camera prefab persistence/Undo/Redo/direct Apply
  PASS; existing split-world combat/bake/fog/Undo checks PASS; strict repeated
  builder with both worlds selected PASS. TYPINGHERO_QA=1 for the valid path.
- Evidence root `E:/CodexTemp/TypingHero/camera-prefab-preview-20261023` contains
  fixture pilot (`repro`), genuine baseline (`repro-initialized`), accepted logs,
  snapshot receipts and external adapters. Private server root
  `/tmp/typinghero-camera-prefab-preview-20261023` uses private copies of Library,
  cache/HOME (unused npm cache omitted) and the shared exclusive runner lock.
  Images/activation/shared data are preserved; original3GiB threshold remains on
  the actual private filesystem. No full suite, PlayMode/WebGL, visual or FPS claim.
- Main advanced to15d538c with combat-light pulse clock changes. Integrate this
  source and recheck the scoped Editor acceptance before remote publication.

- Integrated main15d538c asb4fbb7d without source conflicts; retained both guide
  additions. Final scoped rerun20261023T072145, input
  `2d28112b52ea6befbbb4558b733326a8da35d25f88507416f6fdc5e473b7c9c8`:
  all camera prefab/QA guard/previous Preview/Undo checks PASS and strict repeated
  builder with both selected worlds PASS. No regression markers or unhandled
  exceptions. Only receipt documentation changes after this tested snapshot.
- Final evidence/logs/snapshot are in `final`; `acceptance.json` records actual
  remote integration and cleanup. Retain evidence and the remote task branch for
  review/reproduction; evidence can be deleted when no longer needed, and the
  remote branch only when requested. Task-created local worktree/private server
  root must be removed after verified remote integration.

## Dynamic sprite shadows - strong contrast, 2026-10-23

User selected strong contrast. Base main `1f22565`; independent Orca worktree
`dynamic-sprite-shadows` preserves the dirty primary checkout. Hybrid route:
alpha-clipped sprite cards cast into the existing 3D scene; semantic/form normal
planes relight their surfaces. No limb-on-body volumetric self-shadow claim.
Gameplay, contact timing, albedo/atlas, existing GUIDs, geometry and pixel camera
settings are unchanged.

- Key stays intensity24/hard512. Rear fills become intensity8/shadows off.
  Parry uses hard256 shadows with bias0.015/normalBias0.02; eight additional
  lights/object admit authored world lights plus the one selected combat light.
  Existing strongest-pulse scheduling, snapshots, clocks, holds and release stay.
  Light fills shadows spatially; no global shadowStrength animation was added.
- Re-authored29 existing normal PNGs/261 sliced poses using semantic material
  regions and broad form planes, never luminance-as-height. Ordinary Prepare now
  preserves painted bytes and custom secondary bindings. The destructive draft
  replacement is an explicit Editor menu command; offline recipe `--write` is
  also explicit. See [authoring guide](../game/lighting-profiles.md).
- Reference: [Acquire combat lighting](https://www.unrealengine.com/spotlights/octopath-traveler-s-hd-2d-art-style-and-story-make-for-a-jrpg-dream-come-true)
  directly describes point lights with combat VFX casting character shadows on
  environments. [Dead Cells developer interview](https://80.lv/articles/interview-with-the-developers-of-dead-cells)
  discusses normal-map lighting. Prior research directly inspected129 GIF frames/
  127 pairs; this does not establish its cast-shadow algorithm or visual parity.

Evidence root: `E:/CodexTemp/TypingHero/dynamic-sprite-shadows-20261023`.
`review.html` links A/B videos, timestamp manifests, observations and measurements.
Retain this evidence and the remote task branch for review; remove only when no
longer needed/requested. No external publication or full-suite/stress/soak run.

| Acceptance | Actual evidence |
|---|---|
| Frozen baseline | main1f22565, input2d28112b52ea6befbbb4558b733326a8da35d25f88507416f6fdc5e473b7c9c8, WebGL run20261023T081906 |
| Scoped Unity | run20261023T082941:43 tests PASS across SpriteShadowTests, ImpactVfxClockTests, ImpactVfxTests, LightingProfileTests |
| Final warmed light tests | run20261023T083737:2 SpriteShadowTests PASS, including zero managed allocations after warm-up; overlaps the43 above |
| Authoring | run20261023T083209:custom binding/painted-byte preservation, Preview/Undo/Redo/save-reopen/fog, strict two-world repeat PASS |
| QA player | run20261023T083500, input626e54e88dcb554f27efec2a89b18b3e3f2a7b661f2a16deeac2993137a47a61:strict repeat + WebGL build PASS |
| Real input | baseline/candidate each14 checks PASS:normal hit, received hit, parry, Pause/Resume, follow-up, Slayer/victory, Menu cleanup; no exceptions; network fixtures/blocking enabled |
| GPU pose diagnostics | run20261023T083900:1044/1044 cells directly viewed,261 poses x left/right x facing; run20261023T084553:10/10 above/below cells, llvmpipe LLVM15; alpha-shaped floor casts, no rectangular card shadows |
| Normal previews | 261/261 poses viewed across33 sheets, albedo/normal/left/right/mirror; illustrations are not Unity PBR acceptance |
| Data | source-audit.json:29 normal dimensions and bytes match QA output; existing metas/albedo/animation unchanged. Only two rear Light records copied from tested serialized output, retaining original scene IDs/geometry |

Native GPU: Edge155/ANGLE D3D11, GTX1650Ti,1280x720. Selected encoded frames and
all consecutive transitions reviewed at internal native320x180 and nearest2x
actor/floor crops; full1280 stills also inspected. A/B captures use phase mapping,
not identical wall-clock inputs. Actual coverage:

| Recording | Complete selected windows | Viewed frames / transitions |
|---|---|---|
| baseline-visual | parry/follow-up F12-277; final guard-break preparation through Slayer restoration F605-849 | 266/265 +245/244 |
| candidate-visual | parry/follow-up F18-289; final guard-break preparation through Slayer restoration F613-861 | 272/271 +249/248 |
| baseline-continuous | normal preparation/contact/recovery F11-64 | 54/53 |
| candidate-continuous | normal preparation/contact/recovery F11-57 | 47/46 |

Manifests record exact requested/encoded PTS and source hashes; CSV rows record
observations per frame and preceding transition. Held/repeated encoded frames
remain included. Initial visual normal windows have a recording gap and are
superseded by continuous captures without screenshots. No claim to inspect
unrecorded engine frames, every full animation clip, or the whole source videos.
Static GPU pose coverage remains separate from temporal WebGL coverage. Native
and quarter-speed playback controls are verified in playback-controls.json;
frame inspection is the visual evidence. Warm torch color, actor readability,
spatial floor light, cloth/blur and return from Slayer showed no selected-scope
regression. Intentional Slayer black/red silhouettes and source hurt flashes stay.

Separate non-recording runs, matched active/unpaused/non-Slayer samples401 each:
RAF p50/p95 baseline18.295/19.025ms, candidate18.290/19.020ms. Observed WebGL draws
p50/p95 277/281 to165/184; 256/512 shadow-tile viewports18/18 to6/12. These are
observational browser counts, not GPU-only timing or engine profiler pass counts.
No FPS improvement claim. No >10% p95 regression, so no ABBA escalation required.

Pilot/environment notes: initial baseline stopped before Unity below the3GiB
free-space threshold. Removed only unused private npm cache; shared toolchain,
license and caches preserved. Private tmpfs runner uses the shared exclusive
runner.lock. First offscreen diagnostic image needed URP shadow-resource warm-up;
recaptured warm full/vertical sets. Exactly1043/1044 and9/10 other images are
byte-identical to already viewed pilots; changed first images viewed separately.
Latest diagnostic inputa17b4bb5d7844a29f130f66f48a207bc82bb2c82bd4f292b52bde54aa987eed0
adds only Editor capture warm-up and test changes relative to QA runtime inputs.
Final source audit preserves that snapshot; only two serialized rear-light records
now mirror tested builder output. Remote integration/cleanup receipt: acceptance.json
in the evidence root, populated after publication.


## Per-frame depth self-shadow implementation (2026-10-23, in progress)

Task base origin/main `69ed62c`; visual baseline `1a7895c`. Independent Orca
worktree `sprite-self-shadow`, branch `micharel09/sprite-self-shadow`. Dirty
primary checkout preserved. Evidence root:
`E:/CodexTemp/TypingHero/sprite-self-shadow-20261023`.

Contract: authored linear R depth (front = local -Z), alpha occluder coverage,
Point/no mip/no compression, matching sprite atlas UV. Material defaults .65
strength/.15 local depth/.01 local bias. Eight probes/light, at most12 source
texels, bounded to the current frame. Ambient/emission and existing alpha/depth/
shadow-caster passes remain unchanged. URP14.0.11 installed Lighting.hlsl was read
from fly-server package cache; direct lighting uses its existing BRDF/AO/shadow
helpers. This is planar approximate self-occlusion, not character geometry.

Production is not enabled: world definitions have no depth library. Current
fixtures author only hero combo_3 frame1 and sword Attack1 frame2, with explicit
pose polygons recorded in `Assets/Tests/Fixtures/Lighting/SelfShadow/poses.json`.
These are prototypes, not full-clip or all-actor acceptance. Required next gates:
48 same-pose GPU captures (two actors, four white-light directions, two facings,
flat normal/off, authored normal/off, authored normal/on), prototype performance,
then controller-derived complete authoring and per-frame/transition/loop reviews;
real1280x720 WebGL input/lifecycle and separate same-build on/off p95 comparison.
If >10%, confirm ABBA, then only four-probe fallback is permitted. No visual,
performance, runtime or integration pass is claimed yet.

First focused test attempt stopped before Unity: private runner copy was still
in progress and activation state had not reached its private HOME. No test ran.
Private runner `/data/typinghero-game-loop/task-self-shadow-20261023` uses the
shared exclusive lock, pinned image, copied Library/cache/HOME; no shared data
is modified. Retain until verification completes, then remove only owned runner
and clean integrated worktree. Local A/B evidence must remain for review.


Prototype verification receipts (not full acceptance):

- Focused Unity run `20261023T115125-quick`, input
  `3aab147d599954c4b49d59687614bb1a69ab3074db721f57439aa9a658ad4d93`:
  3/3 tests passed (SpriteDepthTests and two SpriteShadowTests), zero failed/skipped.
  Frame/facing refresh, missing-frame fallback, clear/rebind, combat flash/Slayer
  property preservation and256 warmed updates with zero managed bytes passed.
  The subsequent explicit second-library A/B/A assertion was not in this first run.
- GPU pilots `20261023T115449-serialize` and `20261023T120022-serialize` each
  exported48 images successfully. The first had incorrect skeleton ground placement;
  the second corrected it. These used hero combo_2 frame0, which is superseded by
  combo_3 frame1 (hand/hilt over waist). No full pilot visual pass is claimed.
  First-pilot hero sheet was opened but cropped the mirrored weapon; not acceptance.
- Current fixture PNG structural audit passes dimensions, exact selected-cel alpha,
  transparent unpainted cels and data channels; hashes in `prototype-inputs.json`.
  Existing source albedo/normal/importer files have not been edited. New GUID
  collision audit and `git diff --check` pass.
- Run `20261023T120901-serialize`, input
  `b96abc3c1272f4b01e9a4855cd26324cb5dd1bb902273bdd27ee1f2a6f14f2b2`,
  produced no capture directory after over5minutes. Unity was still using~103% CPU;
  log stopped after scene-manager initialization and contained a Unity TLS error.
  Cause is not established. Stopped exactly its container after verifying the
  private `/project` mount; exit143 is an interrupted failure, not a pass. Latest
  source is being retried without changing shared cache/other jobs.
- Orca-created default terminal was read, confirmed idle, closed. Worktree remains
  dirty intentionally because full feature/asset acceptance is incomplete; no commit,
  push, main integration or production activation has occurred.


Latest prototype receipt `20261023T121521-serialize`, input
`5815fa884768f373416632903daca28a49509b1441b753d0cee447f3db1b0a92`:

- Retry completed without changing shared data; prior interruption remains failed.
  Final prototype shader compiles/renders on llvmpipe LLVM15.0.7. Export48/48
  snapshots; directly inspected48/48 on4 sheets at native and nearest2x. Two
  source poses only: hero combo_3 F1 and sword Attack1 F2; four white-light
  directions, both facings, flat-normal/off, authored-normal/off and authored-
  normal/on. No animation sequence or transition acceptance (0 reviewed).
- Localized hand/chest/waist differences track the selected light/facing; face,
  blade and silhouette remain readable in these stills. No visible fringe in this
  selected scope. Subtle1-2pixel changes do not establish temporal stability,
  independent parry-fill behavior or adversarial neighboring-frame rejection.
- Latest scoped3/3 Unity tests PASS, zero failed/skipped, including the added
  second-library A -> B -> A property check and256 warmed binding updates with
  zero managed allocation. This is binding-level evidence, not full world-session
  A/B/A or native GPU performance evidence.
- `prototype-verified/receipt.json`, `visual-observations.csv`, four sheets,
 48 raw PNGs, exact input snapshot receipt, test XML and logs are retained in the
  task evidence root. Source/depth hashes are in `prototype-inputs.json`.
- After this receipt, only Editor inventory export and its CLI entry point were
  added; runtime, shader, maps and tests are unchanged. Inventory-only compilation
  is separate; it does not rerun or expand visual/PlayMode acceptance.

Remaining: complete production depth atlases and secondary bindings/library,
full frame/transition/loop review and original-speed/slowed playback, strict twice-
run builder preservation with complete depth data, one native1280x720 WebGL
lifecycle/input run, and separate same-build on/off p95 measurement (ABBA if>10%,
then only4 samples if confirmed). No production activation or main merge until
those gates pass. The retained dirty worktree and private runner remain task-owned
continuation resources; remove them after verified integration, retaining A/B evidence.


Final inventory-only compile/export `20261023T122418-serialize`, input
`28099165de958d4a4539c2e1bd6e2bde05b4161eb1ffba5ddb3e82b2027626f7`,
PASS:268 unique referenced sprites,279 serialized sprite keys across33 clips and
30 source atlases. The initial94-sprite scene-only inventory was incomplete:
`EndlessRun.Definition` loads the six `EndlessRole` resources dynamically. The
final collector includes exactly those role assets/dependencies, plus selected
scene/world references and direct hurt/parry clips, without scanning unused art
folders or hardcoding261. Original key times, held/repeated keys and clip loop
flags are retained in `inventory-final/depth-inventory.json`. This is an inventory,
not visual review of268 sprites. Only2 of268 have prototype depth data; no complete
production atlas/library is supplied yet. The final inventory compile adds no
runtime/shader/map change, so the previous3-test/48-static-image evidence is reused
only within its stated scope. Whitespace/GUID checks remain passing.

Next work remains authorized: establish the representative native performance
comparison, author all remaining referenced depth cels with pose-specific overlap,
then finish the full review/runtime/builder/performance gates above. Do not treat
this prototype checkpoint as completion or as permission to enable production.


## Self-shadow diagnosis and interactive two-pose demo (2026-10-23, in progress)

User rejected the visual distinction in the static viewer. This supersedes any
interpretation that the earlier two-pose images passed visual acceptance. The
agreed immediate scope is diagnosis, correction of the two existing poses, and
an interactive WebGL light demo; production/full-animation gates above remain.

Evidence: `E:/CodexTemp/TypingHero/sprite-self-shadow-20261023/diagnosis`.
`before.tar.gz` freezes the pre-diagnosis inputs. Source albedo, normal maps and
all existing Art importer bytes are unchanged; existing ShadowCaster/DepthOnly
pass text is unchanged after LF normalization. No GUID collisions found.

- Extracted the existing, byte-equivalent ray function to
  `SpriteDepthVisibility.hlsl` so diagnostic shaders exercise the real function.
  GPU run20261023T124740, input86a021217135b058ad6ca16e6d058cf48560364bb9b62f96c1edde417c0f1060:
  eight analytical cases pass (flat, four ledge directions, explicit uniform
  mirror, transparent blocker, hostile neighboring cel), plus3/3 scoped Unity
  binding/shadow tests. The uniform mirror case does NOT establish that an actual
  SpriteRenderer supplies that flip uniform; this gap is being investigated.
- Initial photometric run20261023T125714 failed because the diagnostic created
  transient textures before NewScene, then cleanup obscured their destroyed
  references. Fixed diagnostic resource ordering; not a runtime shader fix.
- Run20261023T130934, inpute7bdfb5597f0c3ed8b11e5d7ed652d1d47e753957b1cf3167668ae17296a3ebd:
  analytical cases and photometric checks pass. Flat-depth PBR and missing-map
  output errors are0. Occluded main-light receiver changes red0.222->0.078;
  independent fill blue0.432 and emission green0.040 remain unchanged.
  Scoped Unity3/3 pass. WebGL demo build succeeded; native inspection exposed
  an incorrectly elevated neutral floor (hiding the actor) and missing font
  glyphs. This candidate is not the accepted demo.
- Two numeric depth fixtures were re-authored: torso32, head165, foreground
  arm240, blade250, rear scarf80, other silhouette24 (0..255). Hero arm polygon
  now follows the upper gray forearm/grip instead of extending across the lower
  tunic. Material defaults remain0.65/0.15/0.01 and8 probes. Both selected alpha
  masks exactly match source; unpainted cels remain transparent. These are still
  ONLY two static pose fixtures, not production atlases.
- Viewed all48 new color captures on four native-2x sheets and all48 diagnostics
  on four sheets, plus the enlarged hero overhead comparison. Increased localized
  shade is visible under the arm/grip and on upper ribs; facial/weapon silhouettes
  remain. This is not yet a visual pass at gameplay conditions. Critically, the
  debug local-ray colors do not reverse X with SpriteRenderer.flipX. A direct
  SpriteRenderer photometric mirror regression was added and queued to distinguish
  a debug-only error from a production shader binding error. Do not ignore it.
- The isolated QA builder copies DungeonWorld and uses actual SampleScene actor
  transforms, existing arena/presentation cameras and profile; only two frozen
  sprites are installed. White-light/dungeon, facing, three normal/shadow modes,
  moving light, blue fill and diagnostic views are implemented. Demo code is
  guarded by UNITY_EDITOR or TYPINGHERO_DEPTH_DEMO; no gameplay bridge/API changed.
  No playable demo or native GPU acceptance is claimed until the build is opened
  and verified. Main integration remains blocked by the original feature gates.

Actual-SpriteRenderer regression run20261023T132711,
inputeeaa8b4f92c5bf65c6d27e303bf650ba57a776f93cdab88b61475f604de36361,
FAILED: mirrored ledge casts towards the light, maximum channel error0.1441458.
The earlier explicit-uniform mirror test missed this runtime binding defect.
The pending fix supplies `_DepthFacing` from SpriteRenderer.flipX/flipY through
the existing merged property block and uses it for mapped actors' rays and normals.
Unmapped actors retain the original normal path. Added real SpriteRenderer flip,
negative transform scale, combined scale/flip, and known tilted-normal regressions.
These additions require a new passing run; the earlier analytical pass is not
evidence that the fix works.

The next demo candidate places the neutral floor at the authored pose's lowest
opaque texel, freezes dungeon time for reproducible A/B, and uses ASCII labels
supported by the built-in font. Its source was reviewed after correcting a local
text-conversion error. The superseded queued snapshots were cancelled before
execution; `diagnosis/fixed-run-verified-source.log` tracks the fresh snapshot.

Verification continuation:

- Run20261023T134736 passed8 analytical GPU cases, then stopped progressing.
  The main Unity thread was waiting while its ALSA Mixer thread consumed99% CPU.
  Stopped only the verified task-owned container; this run is aborted, not passed.
- Retry20261023T135928 uses identical input
  `d5f0ca1e9e31dfe1c99cc939a399194909fa21f13685c33e6a3c0546c2c2fb08`.
  Eight analytical cases, real renderer flip/negative scale/double flip/tilted
  normal checks, and3/3 scoped Unity tests PASS. Mirror max error is now0;
  flat-depth and missing-map errors remain0; red receiver0.222->0.078 while
  blue fill0.432 and green emission0.040 remain unchanged. The retry included
  an unverified `-noaudio` flag; its effectiveness is not claimed. Future runner
  invocations use `-disableaudio`, found in the pinned Editor binary. No project
  audio configuration was changed.
- Downloaded48 color and48 diagnostic images into
  `diagnosis/fixed-20261023T135928`. Viewed every case on four color and four debug
  sheets (nearest3x crop containing the full actor). Ray X reverses correctly
  with facing; the head and silhouette remain readable and the mask stays local
  to overlaps. Color differences remain subtle in this soft offscreen capture;
  this is not native-gameplay visual acceptance. The WebGL build and native
  review are still pending at this checkpoint.
- Rechecked1304 existing Art files against the frozen input: zero byte changes.
  All1204 current asset GUIDs are unique. `git diff --check` passes.

Native two-pose review (candidate2):

- WebGL build20261023T135928 succeeded, including the existing strict repeat
  builder. Downloaded archive SHA256 matches the runner:
  `f89a3cf1a7ba882b0fc0d094dae80448a1b69a768f7b1ee1d336699a74078dd5`.
- Browser:1280x720 canvas/viewport, device scale1, WebGL2, ANGLE NVIDIA
  GeForce GTX1650Ti/D3D11. All resources are localhost; the demo server blocks
  external connections. Evidence is `diagnosis/native-final/`.
- Directly viewed all48 white-light captures (two poses, two SpriteRenderer
  facings, four directions, three modes) on nearest2x full-actor crops. Native
  images are crisp. Existing normals visibly shape scarf/skull/arms. Self-shadow
  adds a localized band beneath the hero's crossing gray forearm and darkens the
  sword skeleton's upper ribs beneath the foreground arm/weapon. It changes with
  light direction, retains faces/silhouettes, and does not darken the entire actor.
  Bottom/right cases can be subtle; the effect is not a large volumetric shadow.
- Also viewed both dungeon ON/OFF pairs and native depth/mask/ray views.
  Dungeon differences are subtler under the original warm lighting. Repeated
  paused white-light captures are pixel-identical; repeated frozen dungeon
  captures differ by one red-channel value of1/255 at one pixel per pose.
  Moving-light captures differ; fill ON adds light to the actor. These are render
  controls, not gameplay Pause/Resume or parry acceptance.
- Found two demo-only defects during browser QA: runtime CreatePrimitive logs
  missing MeshCollider after stripping, and hiding zoom leaves stale pixels
  outside the production combat viewport. The next build serializes only the
  built-in floor mesh and clears that unused right margin before drawing UI.
  Shader/binding/maps are unchanged and hash-match the passing render run.
  Build20261023T141723, input
  `c9a6d6bc16567d07814b9f5b12cb4744ef5abdf9c048f9e3243b597f2d459382`,
  is pending. It must pass native floor/zoom/console checks before demo handoff.

Technical and static-pose visual evidence above does not cover268 production
frames, transitions, combat lifecycle, or same-build native performance. Production
remains disabled; no main integration is claimed.

Final-demo follow-up: build20261023T141723 succeeded; archive SHA256
`ca1b810a0a3c904d830782a2e1fcea9297405744d439bcff999dec82e620eb71`.
The MeshCollider error is gone. Both poses' overhead-white-light OFF/ON world
captures match candidate2 pixel-for-pixel, confirming the serialized floor mesh
preserves the reviewed rendering. However, hiding zoom still leaves its image:
the initial clear used Unity's transparent-black texture. Corrected the margin
to draw the opaque white texture tinted Color.black and restore GUI.color.
See [Unity's blackTexture definition](https://docs.unity3d.com/ja/2022.3/ScriptReference/Texture2D-blackTexture.html).
Only this QA UI clear changed; next build input
`8064bfd3423e1786932b67b334fd81b48c8a9987dda1262c442cfeb4810ea608`
must verify show/change-actor/hide zoom and console before handoff. All132 normal,
surface/depth PNGs and metadata match source after the runner's LF normalization
for metadata; authored PNGs are byte-exact. The old static-viewer server was
stopped; its A/B evidence is retained. The interactive server remains on58936.

### Two-pose lab handoff (2026-10-23)

Build20261023T142920, input8064bfd3423e1786932b67b334fd81b48c8a9987dda1262c442cfeb4810ea608,
PASS. Downloaded archive matches runner SHA256
`27c3434d0f5e9c6596c3df1a5bf8076308d65fc632e4428dfd284be4264a3ad9`.
Live local lab: `http://127.0.0.1:58936/demo-review/demo/`.

- Rechecked native1280x720 floor and zoom show/change-actor/hide. Hidden zoom
  region has zero non-black pixels; no stale actor remains. Both poses' overhead
  OFF/ON world captures match the reviewed candidate2 exactly (zero changed pixels
  in all four comparisons). No MeshCollider error or script exception remains.
  Console still logs unsupported optional URP Edge Adaptive Spatial Upsampling
  and HDRDebugView shaders; these are not the actor shader or active lab passes.
  Do not describe this as an entirely warning-free URP console.
- Scope complete: diagnose binding/data/capture, correct two authored depth poses,
  fix real SpriteRenderer ray/normal facing, eight analytical GPU checks plus
  photometric/flip/scale tests and3 scoped Unity tests,48 offscreen color +48
  diagnostic +48 native white-light views, two native dungeon comparisons and
  interactive lab controls. See `diagnosis/native-final/receipt.json`, per-case
  observations, final-regressions, console and environment records.
- Runtime rendering acceptance is reused across the last two demo-only fixes;
  no normal/albedo image or material art default changed. After the final build,
  only trailing whitespace in three NEW .meta files and these documentation
  records changed; their GUID/importer values are unchanged.
- Full feature remains incomplete: only2/268 referenced sprites have depth,
  no animation transitions/loops/playback or full combat lifecycle/performance
  acceptance. Keep production disabled and do not merge main at this checkpoint.
- Retain task branch/worktree `micharel09/sprite-self-shadow` at
  `C:/Users/ADMIN/orca/workspaces/TypingHero/sprite-self-shadow` and private runner
  `/data/typinghero-game-loop/task-self-shadow-20261023` for the remaining feature
  gates. No task Unity job remains after the build. Remove those resources after
  verified full integration; preserve A/B evidence. Local demo serverPID2024 is
  retained for user review and can be stopped when that review is finished.

### Full gameplay depth expansion (2026-10-23, in progress)

- Merged origin/main e5bbc76 into the existing task branch, preserving the upstream
  hit-flash/Slayer composition and removed guard-break/body-fall VFX.
- Found and fixed a dead hookup: the active PixelUI pipeline calls
  DungeonSessionBuild, not WorldLightingBuild. The active builder now serializes
  exactly one SpriteDepthBinding on each actor. Run20261023T150555-serialize,
  input01b97e7e1ccdafd93f57f2f341e0d69f01d7f0b564a9e7c94cedc74cc24cf448,
  passed repeated builder, two serialized bindings, unassigned-world bind/clear,
  and13 SpriteDepthTests/SlayerTransitionTests. That run predates full-library bind
  assertions; it does not establish the later A-to-B-to-A result.
- Inventory now follows ParryContactLibrary replacements too:274 distinct cels
  on31 atlases, including six gameplay parry cels formerly missed by clip-only
  collection. No hardcoded expected frame count in the builder.
- Added draft anatomical masks and31 depth PNGs, plus ActorDepth.asset. Source
  cels were inspected to place head/arm/weapon/shield masks; final rendered frame,
  transition, loop and playback review remains pending. Hero's non-anatomical
  final soul fragments do not occlude light. The two approved prototype cels
  preserve every visible depth pixel (including their24-valued rear plane).
  Original31 source GUIDs, slicing, pivots/import settings and prior secondary
  bindings compare unchanged; only _DepthMap links were added. No albedo or
  normal PNG was edited. Draft recipe and asset hashes are under task evidence
  depth-authoring/draft-map-receipt.json (regenerate after mask edits).
- Full-library checks initially exposed incorrect empty-list secondary texture
  names; fixed. Full-library bind initially failed because scene opening unloaded
  the test's library asset, which deliberately is not a production dependency.
  Reacquiring it after both scenes open fixes the test without a runtime/shader
  change. Run20261023T154159-webgl input6d9099ac242a638df7a72bd7efd7da6d5453b2808a3c16b5170d13249a5d8e9b
  passes repeated builder, painted PNG byte preservation, full-library bind,
  clear, no-map bind and return. CaptureAll and the QA gameplay build are running;
  no full-game visual/performance acceptance yet. Production remains unassigned.
- Two-pose native lab RAF measurements, no capture: hero ON/OFF p95 both33.1ms,
  sword OFF31.6ms/ON31.7ms. Same existing lab build/viewport1280x720; raw samples
  are diagnosis/*-dungeon-*-perf.json. This is static-pose browser cadence, not
  final combat/GPU performance acceptance.
- Retain the task worktree, private runner, original two-pose lab and evidence
  roots listed above while full gameplay work continues. No main integration or
  complete-feature acceptance is claimed by this checkpoint.

### Full-map QA checkpoint, 2026-10-23

- Build20261023T155618-webgl, input
  `f15cd2ac15c99709af18110ac0276d6006fe1cdd60a9452d40f11998e4c6589a`,
  passed repeated builder, PNG preservation, selected-library bind/clear/no-map/
  return,6576 GPU captures and actual gameplay WebGL build. The previous154159
  build failed after compilation while restoring unloaded world editor objects;
  restore now reacquires assets by path. Downloaded player archive SHA256:
  `b5cc7a12ae979c108a517ea8d2f3c00af4646d683808dbe020ff5e1f5050f21a`.
- Inspected274 cels in both facings under top white light, native and nearest2x.
  Found a dark stripe on tutorial Attack1 F5/F6's non-anatomical smear; excluded
  that VFX from blocker coverage, rebuilt and re-inspected the full10-cel clip.
  All other captures compare byte-identical. No source albedo/normal PNG changed.
  Evidence: depth-authoring/frame-observations.csv (548 rows), review-wide,
  tutorial-revised and review-tutorial. Updated hashes: draft-map-receipt.json.
- Inspected all279 animation keys in33 clips, both facings, in actual clip order,
  including held/repeated keys and514 adjacent/loop pairs across both facings.
  Evidence: depth-authoring/clip-order and clip-observations.csv (558 rows).
  These are static adjacent-pose observations, not timed playback acceptance.
  Non-loop outgoing state joins are not inferred from isolated clip sheets.
- Serialize20261023T162033, input
  `9d52f4aca4abbbfd7532a2457fda867b67e5720ee964a66d2d96605f39c4f434`,
  passed full-library/editor checks and13 SpriteDepthTests/SlayerTransitionTests,
  including warmed binding zero-allocation and property-block preservation.
  Copied only the two tested added-component overrides into SampleScene;
  preserved existing serialized IDs and avoided unrelated builder churn.
- Native gameplay QA runs gameplay-native-03 and -04 passed14 checks each:
  real parry, typed follow-up, Slayer, Pause/Resume, restoration, managed errors,
  restart, menu unload and return. Recorded received hits too. Earlier01/02
  drivers failed to parry: they used an obsolete normalized animation contact
  formula. Switching the driver to actual enemyContactIn fixes input scheduling;
  no combat code was changed. User demo: http://127.0.0.1:8277 (F8 A/B).
- Native04 video exports788 decoded frames with exact timestamp agreement.
  Before visual review, selected0–3.2s (parry/slash),9.4–12s (Slayer entry and
  Pause/Resume),20.5–22.9s (exit, slash and hurt). Inspected every decoded frame
  in these intervals:220 frames,217 within-segment pairs,400x260 native crops.
  Observations: pulse lights contact/face and decays; flash survives depth binding;
  Slayer tint restores; hurt returns to lit idle. Selected crop/row evidence is
  gameplay-native-04/video-review/selected. The zoom crop excludes parts of the
  full silhouette. Native03 only frames0–44 were viewed before recapture.
- Capture limitation remains: even after warm-up, native04 has gaps0.337–0.809s,
  2.025–2.181s and21.807–21.963s. Recording was continuous with no screenshots;
  decoded-frame coverage does not prove coverage of every game-rendered frame.
  Do not claim full motion acceptance or infer images inside those gaps.
- Same-build native Intel UHD Graphics/D3D11, Edge155.0.4283.62,1280x720,
  no recording: gameplay-perf/browser.json contains480 OFF and481 ON combat
  samples. p95 OFF31.8451ms, ON31.7593ms (ratio0.9973). No >10% regression;
  ABBA/four-sample fallback were not triggered. This is player frame cadence,
  not isolated GPU timing. Existing interactive demo remained open in both runs.
- Timed1x/slow playback remains unverified. A review page at
  http://127.0.0.1:8277/depth-review/index.html preserves original clip key times,
  durations, non-loop endings and mirrored A/B images, plus the native video.
  Production is still unassigned; do not merge main or report full acceptance.
- origin/main advanced to b1a0df6 during QA with a changed parry follow-up and
  recovery clip. Integrate it and recheck inventory/affected parry motion before
  treating this candidate demo as current. Other static cel observations remain
  reusable only where source/depth/render configuration is unchanged.

### Updated-main candidate checkpoint, 2026-10-23

- Integrated b1a0df6 as 6f1754b. Its new queued parry follow-up references six
  additional cels. Authored those explicitly: combo2 F4/F5 and combo2_end F0-F3.
  Existing combo2 F0-F3 depth pixels remain byte-identical. Inventory now covers
  280 distinct cels,32 atlases,290 animation keys and34 clips. Production remains
  unassigned; these maps are still a QA candidate.
- Build20261023T163224-webgl, input
  `0b498d6dc72976333ddf92168acdd71e9061609c565b04b5afc087e9c55cfbc4`,
  passed repeated builder/preservation/binding checks,36 SpriteDepthTests and
  PlayerParryTests,6720 GPU captures and WebGL compilation. All6576 prior
  captures are byte-identical, recorded in reused-capture-comparison.json.
  Downloaded player archive SHA256:
  `486c3385b216e1a93751cc9fc714128850744d44b695fe938ed948f1731f52b1`.
  The earlier162840 build correctly rejected missing newly referenced maps.
- Inspected six added cels in both facings at native/nearest2x and all changed
  parry_success/parry_followup ordered sheets. Evidence under depth-authoring:
  current-map-receipt.json, current-frame-observations.csv (560 rows),
  current-clip-observations.csv (580 rows), review-current-new and
  current-clip-order. Combined static coverage is534 adjacent/loop pairs.
  Timed playback remains unverified; static sheets do not establish motion pass.
- Native gameplay-native-05 confirmed queued typing during successful parry and
  reaching male_hero-combo_2_4. The longer scenario failed to reach Slayer before
  death: measured input frames often took93-296ms during recording and parries
  were missed. Preserve this failed run/video; do not claim full current-build
  native acceptance. Earlier03/04 passes belong to the earlier gameplay source.
- Current review page includes34 clips and original key timing. Retain the
  independent worktree and private runner/evidence while motion acceptance and
  the current-build native scenario remain outstanding. No main integration.
- Current-build performance: gameplay-perf-current initially stopped when its
  input driver missed a short parry pose. gameplay-perf-current-02 used a100ms
  real key hold without requiring that transient pose to be observed. Both
  conditions contain living combat frames; no video or screenshots were taken.
  Initial OFF/ON p95 were54.8706/61.7752ms, triggering ABBA. Confirmation p95:
  OFF54.9164, ON55.5649, ON59.0973, OFF53.1464ms. Mean condition p95 is
  OFF54.0314/ON57.3311ms (+6.107%). This does not confirm a >10% regression,
  so the four-sample fallback was not triggered. Native environment/viewport
  match the earlier run, but absolute cadence is worse; these are browser frame
  timings, not isolated GPU measurements. Raw samples and abba-receipt.json are
  preserved. The driver completed11 checks; this is not full native motion QA.
- Current playable QA: http://127.0.0.1:8291, F8 toggles self-shadow.
  launch-gameplay-current.py serves gameplay-build-163224/player; keep it while the user
  tests. The older8277 demo remains unchanged. Current review page:
  http://127.0.0.1:8291/depth-review/index.html (34 clips; embedded combat video
  remains the earlier native04 capture, not proof of updated-main behavior).
  Review-page smoke check passed34 clips,320x180 images, non-loop ending,
  facing/speed controls and no JavaScript errors. The first launcher closed
  during startup; retry37072 reached ready and served the verified page.


### Production integration, 2026-10-24 (verification complete)

- User accepted demo8293 as visual acceptance, with rear rune/light and both blue
  haze layers disabled. Explicit scope: finish saved production configuration,
  scoped integration tests and one1280x720 real-input run without video; no full
  animation review. Earlier complete-playback gate is superseded by this choice,
  not reported as having passed. Existing evidence and failed runs are retained.
- Merged origin/main8a7b498 into the existing sprite-self-shadow worktree, retaining
  upstream trimmed parry follow-up cel and normal-attack-matched duration.
- DungeonWorld now selects ActorDepth; DungeonSprite saves strength0.65,
  depth scale0.15 and bias0.01. DepthPreview only adds QA controls to those saved
  assets. F8 captures per-material values and restores them on ON/disable/destroy.
- Named shader limits remain8samples/light and12source texels; probe sequence,
  light configuration and authored depth/normal images are unchanged. Reuse the
  existing performance evidence; this constant extraction is not a shader fix or
  lighting change. No new performance claim.
- Pending: current reference inventory, repeated builder/map/reference preservation,
  focused binding/F8/parry/Slayer tests, native integration, production QA exclusion,
  commit/push/remote verification and retained-demo handoff.

- Offline run20261023T180003, input9d1d29426c2fe2f41f603354c1563c233a60c5528e9b28c7dbc9a5c94b4d172d:
  49/49 SpriteDepthTests/PlayerParryTests/SlayerTransitionTests PASS,0failed/skipped;
  actual inventory280sprites, repeated builder, PNG/config/reference preservation,
  serialized world bind/clear/no-depth/return PASS. Reused for unchanged depth,
  parry and Slayer source below. WebGL compilation from this snapshot was stopped
  deliberately (exit143) after newer main arrived; it is not a passing build.
- Integrated mainb0534d5 (pause chrome/menu/encounter guidance cleanup). Two depth
  component overrides remain through the scene merge. Recheck current builder and
  actual menu/Pause/Resume/encounter flow on a new QA build; no full-suite expansion.

- Build queue: the first latest-main attempt expired waiting600s for the shared
  Unity lock; it executed no Unity stage and is not a failed gameplay result.
  QA and production were resubmitted with3600s lock waits, preserving other jobs.

- Final-main QA20261023T182529, input
  `d0403f7cf73aa00e821d13dd00d2a67955d1458ec565ac72611614739fd1ed8e`:
  repeated builder,280sprite metadata/map validation, painted-map and saved
  config/reference preservation, selected world bind/clear/no-map/return, and
  WebGL build PASS. Inventory289keys/34clips/32atlases; no fixed-count gate.
- Native integration-native-02: Edge/GTX1650Ti/ANGLE D3D11,1280x720,18/18PASS.
  Real keyboard/mouse parry -> queued combo2 follow-up, received hit, Pause/Resume,
  Slayer entry/restore, F8 OFF/ON, menu and world return passed; enemyFlash and
  expected follow-up/hurt cels observed in the read-only trace. No video recording,
  no full animation review,0exceptions/provider writes; fixture interception and
  external blocking remain enabled. Initial native launch01 did not start because
  port8295 was occupied; preserved that process and selected a free port for02.
- Downloaded33player files match the immutable server player. All204 exported
  lighting files match source (text line endings normalized); RuneLight,RearRune,
  RearHaze,LowHaze are inactive in serialized build output. Paused OFF/restored
  stills inspected only as integration evidence, not a new animation/style gate.
  Evidence: `E:/CodexTemp/TypingHero/sprite-self-shadow-20261023/`:
  integration-offline, integration-qa-build, integration-native-02 and their
  receipts. Production build and remote integration remain pending.

- Production20261023T183215 uses the same d0403f7 input; normal WebGL build PASS.
  Scanned48stripped managed assemblies: GameLoopProbe,SpriteDepthPreviewControl
  and SpriteDepthDemo absent; JS QA bridge absent. All33downloaded player files
  match the remote. Final local build-input digest also matches both builds.
- Normal-player native smoke: actual menu/name/tutorial skip/gameplay/pause opened
  with keyboard/mouse, no QA bridge or JS/managed exceptions,0provider writes.
  Gameplay and paused images directly inspected: actors render in the saved
  dungeon with rune/haze off. Pressing F8 leaves the paused canvas byte-identical.
  Production helper's first launch had a local tutorial-hash path error before
  browser startup; corrected only the external helper, without changing the player.
- Acceptance is complete for the user's scoped integration and approved demo;
  no new full-animation or performance claim. Final remote commit/ancestry and
  cleanup are recorded in `E:/CodexTemp/TypingHero/sprite-self-shadow-20261023/integration-receipt.json`.
  Retain evidence and the standalone integration-qa-build/player demo outside the
  worktree. Private runner results remain for reproducibility; remove its cache
  and source only when no longer needed, preserving evidence/player artifacts.


### Cinematic wall torches, 2026-10-24 (verification complete)

- User reported fixed-looking triangular pools below both wall flames and chose
  cinematic, clearly visible flicker. Keep central lantern, flame atlas/timing,
  actor depth/normal data, rune and blue haze settings unchanged.
- Branch/worktree: micharel09/cinematic-wall-torches, from main3cd7293.
  Retain current Point Lights (base8/range15/no shadows); author SinePair amplitude
  .18/frequencies5.8,13.7/weights.7,.3/phase factors1,2.37; retain instance phases
  .7,2.9. Disable two TorchBeam objects and remove their atmosphere update refs.
- Use disposable QA render-only controls for same-player baseline/candidate and
  independent beam/wall-light/key-shadow/fog/blur diagnosis. QA controls never
  mutate combat and are absent from production. No recording or full sprite review.
- Pending: source-of-dark-region diagnosis, scoped lighting lifecycle/builder checks,
  native10s visual samples and real-input lifecycle,15s/condition performance
  (ABBA confirmation only above10% p95), production and remote integration.
- Evidence/artifacts: E:/CodexTemp/TypingHero/cinematic-wall-torches-20261024.

- Initial snapshotf31be374 passed repeated builder/authored-byte preservation and
 16/16 LightingProfileTests/SlayerTransitionTests. Native A/B diagnosis and relative
 performance use that immutable player. Main then advanced to c36e591 with reviewed
 Space cue/HUD; rendering scripts, lighting profiles/assets and cameras are unchanged.
 Integrated that main and queued final QA builder/player for its actual input flow;
 reuse the16passing unchanged lighting/lifecycle tests.

- Diagnosis on immutable QA20261024T014122/inputf31be374:26checks PASS.
  Directly inspected candidate and all six same-phase diagnostic images.
  Baseline-no-beam removes the long diagonal/vertical pools; disabling the key
  shadow leaves them visible, and disabling wall lights still leaves the beam
  cards. Fog/blur changes do not remove their geometry. No room geometry, shadow
  proxy or ambient adjustment is needed; keep legitimate dark wall/floor regions.
- Inspected11paired wall crops at0.12..10.13s, both wall torches, plus full-scene
  diagnostic stills. Intensity ranges6.663..9.153 and6.685..9.285; actual emission
  equals each light multiplier, both phases differ, wall texture remains visible
  without the long beam cards. This is sampled lighting observation, not full
  animation review or video capture.
- Same-player native Edge/Intel UHD/ANGLE D3D11,1280x720:15s/condition live combat,
  no captures/recording during measurement. Baseline471samples/p95=33.3938599ms;
  candidate460samples/p95=33.3900452ms, ratio0.999886.11checks PASS,0exceptions/
  provider writes. ABBA not triggered. Browser cadence, not GPU/device-FPS claim.
  Relative torch comparison uses the immutable pre-cue-HUD player; final-main
  rendering/lighting sources are identical and final input/lifecycle is checked
  separately on the current player.

- Final-main QA20261024T015428/input8380b12e (base4b57eb6) passed the repeated
  builder, cinematic profile/disabled beam checks and authored lighting preservation.
 33downloaded QA files match remote; all232exported lighting files match current
 source after newline normalization. Both beam nodes plus RuneLight/RearRune/
 RearHaze/LowHaze are inactive in actual serialized output.
- Native lifecycle-final-02:16/16PASS on current cue/HUD player; real parry/queued
 slash, Slayer endpoint, Pause/Resume, no phase reset or pause catch-up, received
 hit and menu/world return. Actual pause holds age/frame/emission/intensity.
 Slayer composite endpoint and returned gameplay stills directly inspected;
 0exceptions/provider writes, no recording. First lifecycle-final fixture failed
 after5checks because it incorrectly expected inactive light GameObjects during
 Slayer. Source uses a full background composite and keeps world clocks active;
 corrected only the external assertion and clarified the authoring guide, without
 changing production behavior or weakening the no-reset/Pause acceptance.

- Final normal player from the same015428/8380b12e input built successfully;
  48stripped assemblies exclude GameLoopProbe,SpriteDepthPreviewControl,
  SpriteDepthDemo,TorchQaControl, and JS QA bridge is absent.33downloaded production
  files match remote. Native production smoke PASS: actual menu/name/tutorial skip,
  gameplay and Pause opened; gameplay image directly inspected, no long beam cards,
  F9 leaves paused canvas byte-identical,0exceptions/provider writes.
- Final local source plus the declared disposable QA fixture hashes match both final
  build inputs. Production repository changes are only RearTorch.asset and four
  serialized scene edits (two active flags/two update refs), plus documentation.
- Remote commit/ancestry, independent demo endpoint and clean-worktree cleanup are
  recorded in E:/CodexTemp/TypingHero/cinematic-wall-torches-20261024/receipt.json.
  Retain original reported images, immutable players, observations and failed fixture
  result. Reused private runner cache predates this task; preserve it and its results
  for reproducibility. Standalone demo/evidence stay outside the task worktree.


### Pillar torches, 2026-10-24 (implemented; QA waived by user)

- User selected two total torches: one at each outer front stone pillar, facing
  the arena. Retain cinematic yellow flame style and existing rear/main lamps.
- Independent branch micharel09/pillar-torches from ae1271d. Reuse authored
  holder mesh, flame atlas/material, and DungeonTorchLight; new PillarTorch profile
  starts intensity8/range10/no shadows, projected flame12 internal pixels,
  SinePair amplitude.18, phases1.6/4.2, flame offsets.17/.43 seconds.
- Fixtures are parented to FrontPillar_-1/1 with inverse parent-scale compensation.
  Room positions are x=+-8.55,y=2.7,z=-1.2, on the inner faces; holder yaw is
  -90/+90degrees, flame cards counter-rotate toward the camera. Lamps are at
  x=+-8.3,y=3.02666,z=-1.45. Flame scale1.26313648 follows the authoring projection
  formula at the saved35degree camera/180logical height; appearance is unverified.
- Keep ambient/exposure, old lamps, shaders, combat, rune/haze/beam disabled state,
  GUIDs and sprite animation unchanged. Repeated builder must retain both fixtures.
- Original acceptance (waived by user): repeated builder/profile preservation, native1280x720 A/B
  and10s temporal review, actual-input combat/Pause/Slayer/world return, conservative
 8light/object budget,15s on/off p95 (ABBA if >10%), production QA exclusion.
- External artifact root: E:/CodexTemp/TypingHero/pillar-torches-20261024.
  Disposable render-only QA fixture toggles only the new holders/lamps using F9;
  read-only F7 reports phase/emission/lights. No recording/full animation review.
  Reuse previous runner cache only under its global lock; preserve unrelated jobs.

- User explicitly requested "skip QA" after the shared runner queue remained busy.
  Canceled only this task's queued runner before it acquired the lock; no Unity
  checks/builds/browser/performance ran for this revision. Earlier snapshots were
  replaced in the queue to reduce upload and include the affected test assertion;
  all other jobs were preserved.
- Updated the existing main-world lifecycle expectation from3to5lamps; the distinct
  four-lamp fixture is unchanged. Test execution, repeated-builder preservation,
  visual placement/brightness, Pause/Slayer/world return, performance and production
  QA exclusion remain UNVERIFIED for this change. No passing result is claimed.
- Static diff/references/projection reviewed; preserve the old demo because it is
  the previous immutable build. New scene/profile data ships on the next build;
  current live demo does not contain the pillar torches. Remote/cleanup receipt
  stays in the external artifact root.


### Pillar visibility correction, 2026-10-24 (production image verified)

- User's actual gameplay screenshot showed no new torches in production025704,
  despite the two fixtures existing in serialized build data. Previous completion
  claim was too broad: data presence is not visibility acceptance.
- Root cause from source: fixtures were attached to hidden legacy KayKit
  FrontPillar_-1/1, while the visible room uses PixelRoom/FrontPier-1/FrontPier1
  at different positions. Old lights were placed outside/behind visible piers.
- Reparent holder/light pairs to actual pixel-pier transforms (unit scales,
  yaw-18/+18). Holder local x points .25units inward,y=-.25,z=-.82, outside the
  front face (.7half-depth). Lights y=.08,z=-1.08. Counter-rotate flame cards by
  pier yaw, preserve style/phase/offset and12pixel projected height.
- User waived QA suites; run only the production build and one directly inspected
  gameplay image with real menu input. No full tests/performance/recording.
- Retain rejected build/demo8332, user screenshot reported-pillar-invisible.png,
  new visibility-build/visibility-demo artifacts under existing external root.
  Do not integrate or report visible torches until the actual gameplay image passes.

- Production030724/input8cfaf3ac: build PASS, QA excluded; all2384source files
  matched the snapshot manifest and all33downloaded player files match remote.
  Actual serialized scene binds both holder/light pairs to the visible FrontPier
  transforms. Copied only Unity-rounded flame scales back to local scene data.
- Native Edge1280x720 demo8333 entered gameplay using real menu/name/tutorial
  inputs. Directly inspected visibility-demo/pillar-gameplay.png: both flames
  visible on the left/right pier front faces, warm light reveals stone tiles and
  nearby floor, center actors/UI remain readable. Held in Pause for user review.
  This is one still-image visibility check, not animation/lifecycle/performance QA.
- Keep the rejected025704build/demo8332 and the user's image as failure evidence.
  Integration commit, endpoint/PID and cleanup are in the external receipt.


### Clean letterbox, 2026-10-24 (production verification passed)

- User showed duplicated torch/pier strips at wide browser edges and selected
  black bars outside the existing16:9frame, not a wider scene/camera composition.
- Branch micharel09/clean-letterbox from9d41188. Pinned URP14FinalBlitPass uses
  Load on non-default viewports, preserving uncovered backbuffer pixels. Source
  presentation camera clears only its own rect; no full-output bar clear exists.
- Keep world RT/camera/profiles/point presentation unchanged. Add a backbuffer-only
  letterbox pass at AfterRendering+2, after FinalBlit(+1) and before overlay UI
  (+2,enqueued after features). One existing material's new black pass discards
  interior pixels, paints only outside PresentationRect, handlesUVorigin and
  restores viewport. Skip when rect is full. RTHandle wraps built-in camera target;
  no new camera/texture, public API, quality setting or lighting/torch changes.
- Reuse prior production030724for baseline. First menu navigation using cached
  coordinates missed Start; not a gameplay check. Corrected1280entry worked;
  paused1280->1920x900->960x900->1280resize did not reproduce edge residue. Check
  wide menu-to-game entry as the relevant viewport-shrink transition as well.
- Baseline wide menu-to-game entry reproduces the reported duplicate strips after
  waiting for menu readiness and using real Enter inputs. Reviewed image:
  baseline-entry/wide-entry-reviewed.png. Earlier startup inputs were too early;
  preparation-dialog captures are not gameplay evidence.
- First build033055 failed because URP14CameraData.pixelRect is internal; use
  public cameraData.camera.pixelRect. Production034449/inputd7f29cd2 passes,
  all2386snapshot inputs verified,33downloaded player hashes match remote and
  QA bridge/depth-preview types are excluded. Both changed rendering sources
  exactly match the build snapshot. Generated DungeonWorld/PillarTorch/meta
  exactly match the prior accepted030724production; builder serialization differs
  from raw authoring scene IDs, so raw scene byte equality is not claimed.
- Native Edge/D3D11 production: real Enter/Escape/M input,1280x720 gameplay,
  Pause,1920x900/960x900/1280x720resize, Resume, wide menu return/re-entry and
  Pause pass. Directly inspected gameplay, wide/tall/back/resumed/menu/re-entry
  images: bars contain no duplicate scene, actors/torches/HUD/typing remain visible.
  Four image pairs of bar interiors have0nonblack pixels, max channel0
  (135584pixels/side wide;163744top/bottom tall). Exclude the outer4px screenshot
  rim and4px gameplay boundary margin; raw first measurements included a dark
  outer screenshot rim, not leaked scene imagery.0captured runtime exceptions.
  Full QA/performance/video remain waived; this is scoped rendering acceptance.
- Artifact root E:/CodexTemp/TypingHero/clean-letterbox-20261024; keep original user
  screenshot, baseline attempts, immutable build/receipts and final demo separately.
- Integration pending: merged newer main9c48f5d into task e8aee8a. Rendering sources
  remain identical, but integrated production035339/inputedfe35fe is blocked by
  main65852c2's new male_hero-parry_lift sprite lacking authored depth. Validation
  was not bypassed. Ask whether to extend scope to depth authoring or retain the
  verified pre-parry demo. Demo8341 uses034449, not the failed integrated build.
  Keep this worktree/branch until that decision and verified remote integration.

### Automatic depth feature, 2026-10-24 (production verification passed)

- User approved automatic recognition of authored maps, not draft image generation.
  Missing depth is an allowed per-frame fallback; mapped frames retain self-shadow.
  Keep the clean-letterbox implementation and merge current main on this worktree.
- SpriteDepthAuthoring resolves existing source bindings, preserves legacy custom
  maps and discovers conventional GUID-named maps only when unbound. Conflicts,
  broken bindings, invalid maps and build atlas packing still fail; absent maps do
  not. Resolve/validate a library before writing bindings/index; preserve all PNGs.
- Synchronize through the same menu/Enter Play Mode/builder path. Builder sync is
  after clip/world authoring, validation callback -500 follows PixelUIBuild -1000.
  Runtime schema/shader/combat stay unchanged. Stable GUID/local-ID index ordering,
  pruning only unreachable metadata and write-on-change prevent repeat drift.
- Actor inventory follows actor prefab/opponent dependencies and contact libraries,
  without actor folder-name filters or fixed frame counts. Explicit coverage audit
  remains strict; CaptureAll still requires complete coverage rather than claiming
  a partial inventory as full visual acceptance.
- Save only the derived library with SaveAssetIfDirty; automatic Play Mode sync
  must not save unrelated dirty author assets. The Editor test covers this explicitly.
- Production050935/inputb2ddfed:5/5Editor (4.7034938s) and2/2runtime
  (0.0654234s) PASS,0failed/skipped. Actual serializer preserves painted PNGs,
  selected world/material configuration and stable index on repeat. Production
  succeeds with280mapped/1missing (parry_lift), proving missing art no longer
  blocks a normal player; QA bridge/depth preview types are excluded.
- Main advanced to168ae6b: retain its authored lift depth d6eb563 and terminal HUD
  suppression. Mergecd7db69 keeps both changes and final depth sync after builders.
  Synchronizer/validator/binding/focused test sources are unchanged; reuse the7
  passing tests and main's documented HUD checks, rerun actual serialization.
- Integrated production052952/inputaf65179 passes serialization/build/QA exclusion,
 281mapped/0missing. All33downloaded player files match remote. All99lighting
  PNG hashes match current source, including main's added map; no PNG generated or
  rewritten. All281sprite/depth references and float32 metadata values match input.
  Copy back only the verified generated index: one-time stable row order/Unity
  numeric serialization, with asset GUID, bindings and semantic values unchanged.
- Native Edge/D3D11,1280x720: real Enter/Space/Escape, one successful parry
  ([PARRY] OK, score30), recovery and Pause/Resume PASS;0captured exceptions.
  Gameplay/parry/recovery/pause/resume images inspected; bodies, warm pier torches,
  HUD and typing remain visible. No bridge/gameplay setters or video used. Reuse
  clean-letterbox wide/tall evidence because render code/camera contracts unchanged.
- Earlier compiler API-name, helper hashlib and fixture-import-type failures are
  retained in the external receipt. Correct the fixture to linear Default rather
  than weakening map validation; packing rejection now asserts its actual reason.
  Two stale queued jobs were withdrawn only after PID/helper-hash/lock-wait checks.
- Full QA, sprite-animation review and performance profiling remain outside scope.
  Current evidence/demo stay under the artifact root above; remote integration and
  clean worktree removal are recorded in receipt.json after push verification.
