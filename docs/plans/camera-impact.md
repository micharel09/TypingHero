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
