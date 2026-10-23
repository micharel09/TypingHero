# Combat camera impact ? 2026-10-22

## Accepted behavior

World-only transient zoom/kick; fixed HUD/typing. Normal2%/140ms; parry3%/180ms plus1native pixel/100ms; received damage (including death)1pixel/100ms without zoom; outgoing lethal5%/250ms (2% if captured Slayer) plus2pixels/100ms. Guard break uses existing Slayer5.625?3.3/350ms and entry shake; Slayer rapid hits add no camera motion. Zoom attack/hold/release:20/20/100,20/30/130,30/40/180ms. Kick waits for existing hitstop except existing Slayer entry30ms. Priority lethal>Slayer entry>received>parry>normal; one bounded envelope, equal/weaker contacts do not extend it. No combat/audio/balance/input/provider contract change. No itch upload.

## Implementation checkpoint

- Independent Orca branch micharel09/camera-impact, base5f371fa (includes latest approved torch work); dirty parent preserved.
- CombatCameraImpact owns pending/active envelope and owned Cinemachine events, uses actual projection/native-pixel conversion. Dedicated channel2/gain1 listener; channel1/gain10 preserved. Existing parry component is a forwarding adapter. Confirmed hitbox, DamageAccepted and Slayer presentation feed the coordinator; outgoing lethal spawner no longer duplicates shake.
- DungeonArenaCamera composes perspective FOV from untouched base half-height plus transient zoom; foot anchor compensation is recalculated, never accumulated. Session bindings connect the gameplay coordinator and persistent camera. Pause holds, reading/results/reset clear; Slayer tween clock holds on pause.
- Read-only automation fields added to frame and periodic probes; focused tests cover profiles, priority, real hitbox dedup/lethal capture, projection, pause/read/reset and impulse-pool ownership.
- Final integrated verification:32/32 focused Unity cases and86/86 native browser checks PASS. Final receipts below supersede the intermediate runs.

## Research retained

Reference evidence: E:/CodexTemp/TypingHero/camera-impact-research-20261022/research-evidence.json. Official Steam First Cut14.75?17.50s, Nine Sols19.50?25.50s, Samurai Shodown43.75?45.50s:630selected frames/627transitions inspected, all holds preserved. Selected-scope research only; First Cut finisher and ordinary-hit camera generalization from edited Samurai Shodown footage remain unverified. Numeric profiles above are design defaults, not measurements from reference games.

## Verification/resources

- Local task artifacts: E:/CodexTemp/TypingHero/camera-impact-20261022. Keep player/demo/evidence for user review; remove disposable failed captures/remote task source after final verification.
- Shared runner capacity initially3176636416bytes (<3GiB). Removed only camera task failed snapshot003e4617/source and its duplicate archive under the runner lock; other task resources preserved. Temporary scoped runner uses /dev/shm/typinghero-camera-impact-{source,incoming,input.tar.gz}, shared pinned toolchain/library and unchanged capacity thresholds.
- Initial serialize150102 failed protected Cobblestone font byte comparison: generated/source text diff0; Windows CRLF vs Unity LF. Temporary Linux snapshot now canonicalizes text newlines without changing font content or protection assertions. Stale cached CS2001 import diagnostics recovered during refresh; not a passing test run.
- Scoped runner filters CombatCameraImpactTests, CombatFeedbackTests, owned world-texture projection, DungeonSessionTests and compact Pause Resume. Native browser scope: real keyboard/mouse1280x720 and640x360, all Firebase reads/writes intercepted, other network blocked; no full suite/stress/soak.

## Integration checkpoint

- Fetched and fast-forwarded task base to3710883, retaining upstream MetalClash extraction and all camera WIP without conflicts. Integrated32/32 cases PASS on canonical snapshotd0fede39 (includes lifecycle/bar compatibility); final QA player uses this source.
- Pilot QA152739/inputb03a1fe3 built/repeated builder PASS. Native Edge155/GTX1650Ti/D3D11 at1280x720 pilot-r2:45/45 PASS,1026observed render frames, zero exceptions/provider calls; normal peak2%,parry3%,received0%,captured Slayer lethal2%. Actual correction peaks?.987/.976/1.964native pixels for parry/received/lethal. Pilot first attempt failed a driver assumption: an intermediate Endless summary uses finished+removed enemy+Continue, not the final pause overlay/encounterReading flag. Driver corrected; product thresholds unchanged.
- Final source retains entry source world position, rejects post-finish entry requests and freezes Slayer tweens only for manual Pause; results/encounter unscaled exits still complete. Superseded own intermediate builds were stopped/queued requests cancelled after identifying exact dedicated RAM-source mount/profile; other jobs preserved. Remote integration receipt is reported with the final task commit.

## Final acceptance

- Canonical input `d0fede39b9c7db8c593c3ae618f256b4888a25f1a163df390e0389c573ccb4d1`, integrated base3710883. Unity2022.3.25f1 scoped run `20261022T160203-quick`:32/32 PASS,0failed/skipped. Includes actual contact consumers, channel/gain isolation, priority, foot projection, recycled event ownership,32 warmed parry/clear cycles allocating0 managed bytes, Pause/Resume and three dungeon lifecycle cycles. This is not a general performance claim.
- Final WebGL and strict repeated builder `20261022T160608-webgl` PASS on the same input; all33 local player files match server SHA256 receipts. SampleScene retains all1021 original records, adds3; DungeonWorld retains1030 records with only its arena binding field changed.
- Native Edge155.0.4283.62, Playwright1.58.2, GTX1650Ti ANGLE D3D11:86/86 real mouse/keyboard checks PASS at1280x720 and640x360;0exceptions/provider reads/writes. Fixtures intercept providers and block other external traffic. Covers normal/parry/received/guardbreak/Slayer/lethal, fixed HUD/typing, priority, actual pixel correction bounds, Pause/Resume/Restart/Menu and baseline recovery.
- Direct visual review:37/37 sheets,400/400 encoded source frames and390/390 consecutive transitions across ten selected camera windows, including sheet boundaries. Full original WebM and PTS preserve timing/held spans; convenience MP4 excerpts may truncate held tails and are not timing authority. Normal zoom/return, received translation without zoom, parry recoil, Slayer entry/stable rapid hits and lethal exit retain framing; HUD/typing stay fixed. No sprite-animation authorship or full-video/frame-rate claim.
- Local receipts: `E:/CodexTemp/TypingHero/camera-impact-20261022/{integrated-tests.xml,integrated-snapshot.json,integrated-unity.log,player-hashes.json,native-final/browser.json,native-final/frame-review/manifest.json}`. Final immutable server results: `/data/typinghero-game-loop/results/20261022T160203-quick` and `/data/typinghero-game-loop/results/20261022T160608-webgl`.
- Isolated review demo remains open in Edge on127.0.0.1:8261, same final player, fixture routing retained; Enter starts battle. Keep local player/demo/video/receipts until user review is finished. Remote task branch retained as integration history. No itch.io upload, full suite, stress or soak run.

## Reusable impact timeline / hitch regression ? 2026-10-23 (acceptance passed)

Task: replace frame-delta envelope/owned impulses with profile-driven timeline,
world binding and Noise-stage displacement. Worktree
`C:/Users/ADMIN/orca/workspaces/TypingHero/camera-impact-timeline`, branch
`micharel09/camera-impact-timeline`, base `3253274`. Existing dirty primary preserved.
No itch upload. Acceptance: six editable presets; exact hit timestamp; render-acknowledged
peak and release after a skipped attack/hold; priority, pause, hitstop and teardown;
independent worlds/controllers; zero warmed hit/update allocations; repeated builders
preserve edits. Focused Unity consumers/binding/projection and final WebGL real-input
checks at 1280x720 and 640x360, plus 256x144 world fixture and 180ms final-key stall.

Migration source: previous CombatCameraImpact constants; Player.prefab custom recoil
keys (0,0),(.2,1),(1,0), all zero tangents,100ms; SampleScene Slayer source Uniform,
Rumble CM2.9.7 standard curve,200ms,default velocity(0,-.1,0), listener gain10 in camera space,
delay30ms. DungeonCamera.asset preserves that composite1world-unit amplitude and
all11rumble keys. Existing Slayer base lens tween remains its owner. Dedicated
channel2 listener is replaced by CameraImpactExtension; combat no longer emits any
Cinemachine impulse. Channel1 listener/source references remain available without
combat emission, preserving unrelated users. Curves are cloned/prepared on binding;
no runtime default table or alternate missing-profile selection.

Implementation underway; no passing verification claimed yet. Task evidence/runner:
`E:/CodexTemp/TypingHero/camera-impact-timeline-20261023`. External filtered adapter
reuses the exclusive server runner lock and pinned Unity; it canonicalizes text
newlines and confines its disposable source to `/dev/shm/typinghero-camera-timeline-*`.
Shared runner currently has another task's world-configuration build; preserved.

Verification pilots (not acceptance): initial Quick `20261023T033158` / input
`5c3f62dd` stopped at an ambiguous Object identifier in the new consumer fixture;
qualified UnityEngine.Object. Quick `035442` / `5b3181fd` and authoring `035521` /
`30466006` stopped at a missing Cinemachine reference in the Editor test assembly;
removed that test-only dependency and inspect component identity/serialized fields
instead. Two queue waits expired while unrelated existing builds held the exclusive
runner lock. Current retries wait up to3600seconds without changing limits or
interfering with other jobs. No tests from these failed compiles are claimed as pass.
Runtime is frozen for verification; pixel conversion follows the composed lens and
actual world RT each sample, including base Slayer zoom changes. Added full-cycle
allocation coverage (attack, acknowledged peak, release), adapter WorldUnits/WorldPixels
warm loops, two-controller shared asset isolation and real A?B?A session binding.

### Offline acceptance and final QA preparation

- Quick `20261023T035931`, input `de45a36b`:53cases,52pass/1fail/0skip,
  8.9164846s. All camera/config/consumer/two-world cases passed. The lifecycle
  assertion still expected two impulse listeners; updated it to require the
  preserved channel1/gain10 listener plus exactly one direct extension.
- Authoring `20261023T040038`, same input:PASS. Edited normal peak0.027 survived
  two full builder calls byte-for-byte after save; one controller/extension,
  no dedicated channel2 listener, independent256x144 fixture. Finally restored
  original profile and rebuilt. `authoring-unity.log` and source hashes retained.
- Unity-serialized record comparison: DungeonWorld1031/1031records, no changed,
  added or removed records. SampleScene1024?1023: no changed/added records;
  only the now-unreferenced stripped recoil proxy3482038794572103695 disappears.
  Removed that orphan from source while preserving original record order/IDs.
  Profile numeric values/curves unchanged (only0.0/1.0 and whitespace canonicalization).
- Noise must read this frame's CameraState lens, since the brain copies it to
  Camera later. Shared projection math now handles that explicitly. Lens Quick
  `20261023T040245`, input `3b6502b4`:37/37PASS,0fail/skip; includes current lens,
  144/180 RT heights, per-controller asset isolation, full attack/peak/ack/release
  allocation loop and adapter WorldPixels/WorldUnits warm loops (0managed bytes).
- Lifecycle `20261023T040344` exposed a real destroyed-root access in the new unbind
  loop. Added the same live-object guard used by existing Deactivate; surviving
  controllers still clear, destroyed controllers have already cleared on disable.
  Focused rerun after this fix PASS1/1 on input `0c3fc9d2` (three gameplay cycles,
  Pause/Resume, Slayer exit through results, restart/menu and RT/controller ownership).
  Latest test receipt path and durations will be recorded with final QA.
- Final native QA build requested from the verified runtime. Scope remains real
  input1280x720/640x360, final-key180ms main-thread stall, camera acknowledgment,
  actual RT dimensions and full selected-camera-window frame review. No WebGL
  acceptance claim until that run and review complete.

### Integration with concurrent world/VFX authoring

Remote main advanced to `dac65f2` (world configuration `e491984`, impact VFX
`1590347` and receipts). Local checkpoint `bbc49d1` preserves the verified initial
implementation. Merge keeps the incoming world/VFX changes, virtual-camera lens
binding, selected-world validation and authoring checks. Their simpler camera
schema still used the old frame-delta/impulse runner; the requested timeline
supersedes that runner and the duplicate schema.

`Assets/Data/Lighting/CameraImpact.asset` retains incoming GUID
`4bca0a930c3f4bd0abaf68135084484e`, now with six CombatCameraProfile presets. All
five incoming envelope peak/attack/hold/release/pixel values, recoil duration/curve
and Slayer priority hold transfer unchanged; entry curve/gain are retained from
the earlier migration. The task's initial DungeonCamera asset is consolidated
into this asset. WorldDefinition owns the sole binding reference; obsolete
WorldLightingProfile camera pointers and the old CameraImpactProfile/default
C# table are removed. Other world/VFX settings remain incoming main values.
`integration-migration.json` verifies envelope equality/GUID and shows exactly two
changed SampleScene camera records plus the removed orphan proxy versus main.

The public Configure hook prepares future requests without altering accepted
snapshots; world Bind clears state. Adapted the incoming snapshot test and lens
binding test to the rich schema. Integrated Quick `20261023T042510`, input
`9da1fad7b03dbc1f305925ce818e80c6c81183dbf139197069da373c964bdb7a`:
**55/55PASS**,0fail/skip,12.3059049s. Receipts `integrated-{tests.xml,snapshot.json,unity.log}`.
Initial integrated authoring `042616` passed both camera builder assertions but
failed test cleanup because scene restoration unloaded the stack-only asset
reference. The harness now retains assets and reloads by path before restore,
matching existing lighting test ownership. This is not a passing whole run.
The pre-merge WebGL `040906` compiled/built successfully on input `0c3fc9d2` but
is superseded for final integrated native acceptance; no obsolete-player pass claim.

### Slow-render follow-up (resolved; final receipts below)

Integrated authoring retry `20261023T042916`, input `6f8a7c54`:PASS, including the
broader selected-world lighting/impact/material preservation and unused-template
validation. Generated SampleScene1023/1023 records match source exactly modulo
record order/whitespace. DungeonWorld1031records differ only in LightmapSettings
record3 linking the already-authored bake asset, applied by incoming main's builder;
no camera/geometry/style record changes. Keep source world records unchanged.
WebGL `043257` built on that input;33files/48,099,362bytes downloaded and SHA256
matched. Native first matrix102/102passed at1280x720/640x360, 180ms actual-key
stall with receiving contact deltas197.015/190.630ms. Captured parry/received/lethal
peaks about0.9963/0.9986/2.0028logical pixels; no exceptions/provider IO. This run
is superseded for final slow-render timing acceptance below.

The1280capture has long initial held spans. Extended segment boundaries to include
bracketing encoded frames (no narrowing). Directly inspected81window-frames and
78pairs in normal/parry/received before stopping this superseded review; no claim
of reviewing the remaining segments on that player. A follow-up wrapper initially
launched the original driver due an incomplete external path replacement. Its
300ms baseline assertion failed while the peak frame was rendering. Raw samples
show contact116, peak118, then acknowledgment118/baseline119 after a330ms render.
This exposed a real uncovered timing gap: release started at peak sampling rather
than actual render completion and could be consumed before the peak acknowledgment.

EndRender now updates the pause-excluded double clock and acknowledges with the
actual completion timestamp. Zoom release begins no earlier than that acknowledgment.
Shake partitions the next published gameplay delta around the render timestamp,
including clamped frames, then continues on gameplay time. A later post-acknowledgment
hitch still never replays the peak. No asset tuning or gameplay consumers changed.
New slow-render zoom and shake/post-ack regressions pass in the focused58/58 run
(input `947be5f7`,0fail/skip); latest receipt to be appended. Harness now waits for a
render-confirmed baseline with a deadline, warms using real typing and normal UI
Restart before capture, and verifies the actual receiving-frame delta. New final
player/capture requested; earlier102checks are not final timing acceptance.

### Final reusable-timeline acceptance

- Unity2022.3.25f1 run `20261023T044944-quick`, canonical input
  `947be5f7029eef2f1f740e122b3b440ad663abbeca56a4522c0fe9bc25e154af`:
  **58/58 PASS**,0failed/skipped,12.9840066s. Covers140/180/200/500ms receiving
  frames, hitches across phases and actual slow-render acknowledgment, priority,
  cancellation/old serials, pause/hitstop, real consumers, shared-asset instance
  isolation, A/B/A binding and256x144 projection. Warm timeline/adapter request
  and update loops allocate0managed bytes. Receipt `final-tests.xml`.
- Final QA WebGL/strict builder `20261023T045111-webgl`, input
  `09c8255daa374728bc9dcca5382074c29aff740860c767eedf3e9d33df8061eb` PASS.
  Runtime matches the58-case run; input difference is the final real-input driver
  and Editor verification helper. All33 downloaded player files match server
  SHA256 (`final-player-hashes.json`). Authoring preservation receipts above remain
  applicable: subsequent changes affect timing only, not profile/builder data.
- Native Edge155.0.4283.62 / Playwright1.58.2 / NVIDIA GTX1650Ti ANGLE D3D11:
  **110/110 PASS**,60.1598037s,1280x720 and640x360,0exceptions/provider IO/external
  requests. Real keyboard final-key blocks180.005/180.000ms; actual receiving
  contact deltas194.315/190.495ms (frames205/203, serials14/13). One HP reduction,
  one normal request, exact2%peak acknowledged once, then baseline. Parry/received/
  lethal displacement peaks approximately0.9963/0.9986/2.0028logical pixels.
  Normal/parry/received/Slayer/lethal and Pause/Resume/Restart/Menu pass; HUD and
  typing tray stay fixed. Actual world RT320x180 is recorded by the read-only probe.
- MediaRecorder startup dropped the1280normal/received segment; no visual pass
  is claimed from that gap. Same immutable player compositor follow-up passes
  **16/16** real-input checks. Its76original timestamped PNG frames are retained
  with matching76-frame lossless FFV1; largest185.508ms gap is the intentional
  final-key stall. Raw compositor timestamps, not convenience excerpts, own timing.
- Directly inspected all49sheets: **512/512 window-frame inspections and502/502
  consecutive pairs** over10selected complete camera windows, including boundaries,
  held/repeated frames and preparation/contact/recovery. Overlapping windows count
  twice; these are captured frames, not a claim to inspect every engine frame.
  Source/timing/per-frame notes are in `final-frame-review.csv` and both final
  `frame-review/manifest.json` files. No duplicate pulse, missing recovery or HUD
  movement observed. Slayer base lens persists during rapid hits and exits smoothly
  after lethal; damage translates without zoom. Existing reference research above
  remains separate; this task changes no sprites or animations.
- Evidence root: `E:/CodexTemp/TypingHero/camera-impact-timeline-20261023`.
  Keep final player, source videos/PNGs, logs, manifests and receipts for review;
  removable after user review. Remote integration/owned-source cleanup is recorded
  in the delivery receipt. No itch.io upload.
