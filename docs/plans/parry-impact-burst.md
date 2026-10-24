# ParryImpact burst - 2026-10-24

## Acceptance

Create a parry-only copy of MetalClash: both flash layers40px, core clamp2.5world units, existing Pimen sprite sheet and200ms lifetime. Alpha holds through100ms then falls to zero by200ms. Preserve physical particles/seeded trajectories, lighting, camera, audio and hitstop. Player uses ParryImpact; standalone MetalClash remains. Builder migrates the stock reference, preserves custom Inspector profiles/prefabs and repeats without duplication. No public API or new per-frame work.

Required evidence: measured post-clamp size/contact/expiry; pause/resume/pool; same-seed comparison;1280x720 real keyboard parry in both directions; every frame/transition from preparation through tail/end; new audio clip opened at1x/.25x. No itch upload.

## Implementation checkpoint

- Base origin/main168ae6b, independent Orca worktree `C:/Users/ADMIN/orca/workspaces/TypingHero/parry-impact-burst`, branch `micharel09/parry-impact-burst`. Original dirty checkout preserved.
- Added ParryImpact asset/meta, Player reference and private builder binding. MetalClash/prefab/physics/sprites remain byte-identical.
- Added focused ImpactVfxTests for measured sizes (unclamped and clamped camera scales), contact/200ms expiry, same-seed physical renderer traces in both directions, repeated default/custom binding, pause/resume/lease release.
- Unity2022.3.25f1 runner on fly-server initially busy: immediate lock unavailable and doctor60s wait expired. Focused test job queued; no runtime/visual PASS yet.
- Runtime/clip/frame review and remote integration pending. Do not treat source checks as visual acceptance.

## Normal-direction QA

- QA/repeated builder `20261024T045935-webgl` PASS, input716cfe568c579fddf6734fd8d4e5fa1dd1b543a6367966c2bcf3eec22423c5c8. Input archive retained in that result; generated Player confirms ParryImpact, scene inherits the prefab. Focused tests use this exact same snapshot.
- Local immutable player and capture: `E:/CodexTemp/TypingHero/parry-impact-burst`. Native Edge156.0.4314.42/Intel UHD ANGLE D3D11, actual1280x720 viewport/render, world-raster320x180 capture. First full-size PNG capture attempt missed parry and is retained as `failed-native-right-1`; it is not a PASS. Reduced capture overhead only; product timing unchanged.
- Final `native-right/browser.json`:9checks PASS,0exceptions/provider reads/writes. Real Space succeeds, HP40 retained, +30; complete tail expires; Pause/Resume. Actual destination WebAudio mixed into `parry-audio.webm`. No subjective listening claim yet.
- Scope registered before capture: idle before fresh windup, complete preparation, contact, all flash/particle tail, recovery and idle. Full displayed sequence38.4..4095.8ms:95/95frames and94/94transitions directly viewed in all11sheets, plus contact enlargement. Per-frame observations/durations/references: `native-right/review/frame-review.csv`.
- Visual: F28/29 blade raised without premature flash; F30 warm contact star; F31 large white star/halo stays at sword crossing. Contact blade is briefly obscured; player eye/face/body/feet remain readable. F32/33 core clears and white front streak/red rear fragments continue; F34..41 no lingering core; F42..46 exit; F47..94 idle and no retained VFX. Native recording contains stalls/held frames; original timestamps retained, no FPS or hidden subframe claim. Numerical200ms/fade acceptance still awaits focused Unity tests.
- Focused tests/mirror build remain queued behind another task's production job using the same shared lock. No unrelated job stopped. Integration pending.

## Mirror and final media review

- Mirror QA/repeated builder `20261024T051736-webgl` PASS, input9b6a53a1c4ff67488fbef90ba58f8b197fec9fa3e1cf769daf9e92d8dbb05632. Only the external prebuild actor-position/facing fixture differs from normal; no product mirror API. Native `native-left/browser.json`:9checks PASS,0exceptions/provider reads/writes, actual1280x720 real Space parry, HP retained and tail expiry/Pause/Resume.
- Full mirror capture57.5..3406.7ms reviewed:102/102frames and101/101transitions,12sheets and per-frame CSV. F35..39 prepare, F40 contact star, F41..43 white core, F44..46 retreating white streak, F47..64 cooling tail, F65..69 exit, F70..101 idle. Core obscures the crossing blade/near head edge briefly; torso/feet remain readable. No contact drift or retained white core observed.
- Initial MediaRecorder-only video export was rejected: it omitted the normal peak frame. Final movie is reconstructed from **every** displayed PNG and original VFR intervals (no interpolation), nearest-neighbor320->1280, both directions at1x/.25x. Last image repeats to author a40ms terminal hold. Original runtime PNGs/timestamps and WebM destination audio remain authoritative.
- Final `ParryImpact-1280-1x-025x.mp4`:398frames/397transitions, **398/398 and397/397 directly inspected** across all25final-export-review sheets. Encoded peak frames31/127 and233..235/336..338 retain the large contact flare. Blocks right1x/right.25x/left1x/left.25x; block transitions are intentional take/speed changes. Per-frame CSV includes phase observations and mapping to every source image. Rejected export-review sheets are superseded, not complete evidence.
- Original destination audio peaks: right.45636886/left.46738282, nonzero and below1. Audio-only remux preserves Opus leading timestamps as silence, trims only the first-PNG offset, slows the complete timeline with atempo, and pads blocks to video duration. All398video bitstreams/PTS unchanged by the audio correction; full final decode/strictly increasing displayed PTS PASS.
- Final movie3150194bytes, SHA2569770bbcb20a0762a849e5075b0c9332544265e3b649d26b7da9869786c790c6b. Final per-side MP4s opened in headed Edge with audio unmuted/volume1 at playbackRate1/.25 and played to ended; receipts/screenshots in playback.json. This proves playback and audio routing, not a human listening approval. Visual force: bright impulse centered on crossing, readable body and short white obstruction.

## Focused test correction checkpoint

- `20261024T051621-quick`, exact normal input716cfe56:38/40 PASS. One legacy custom-prefab test caught an overbroad migration: final builder now changes MetalClash only with the stock prefab, retaining explicitly authored MetalClash on a custom prefab. All other custom profiles also remain.
- New size fixture initially omitted renderer pixel-unit floor `max(1/PPU, projectionPixelUnit)` and expected.6667WU instead of2.5WU. Corrected formula, pinned actual MainCamera orthographic/target settings with finally restoration. At6/40 orthographic half-heights, tests distinguish minimum pixel geometry and core2.5WU clamp from the larger outer flash. Intermediate queued run predates the floor correction; final focused rerun and strict final serialize are queued.
- Product runtime/render code, profile data and stock-prefab generated output do not change after the two reviewed builds; final Editor delta only preserves custom-prefab migration. Final serialization will be compared before reusing visual evidence. No remote integration claimed until final checks pass.

## Cache correction and upstream integration

- Intermediate `20261024T053647-quick` still reported39/40, an impossible old expectation. Archived source already contains the PPU floor, but the failure PDB points to the preceding assertion line. External adapter had normalized contents with all archive mtimes0; Unity reused the old test assembly. Preserve actual source mtimes, matching the canonical runner. No test threshold or product formula was changed to conceal the failure.
- Fresh `20261024T055545-quick`, same final input09902e4856e750cd63a82d4f6a07f5486a3472f75e6575c82ded01f7318c315e: **40/40PASS,0failed/0skipped**,8.4003645s. All size/fade/expiry/contact/seed/mirror/custom-binding/pause/pool checks now execute the final source. XML retained as tests-preintegration.xml. Pre-integration strict serialize053740 also PASS; whole Player prefab and SampleScene equal the reviewed normal build byte-for-byte after newline normalization.
- Fetched newer main d6d99dd and fast-forwarded this task branch, preserving scoped WIP. Upstream only adds exterior letterbox clearing and automatic depth authoring; all281sprite/depth references and float32 metadata values equal the reviewed base, independently checked in upstream-depth-equivalence.json. Existing shader pass0/VFX simulation/profile/prefab/camera geometry are unchanged; new pass clips inside the gameplay rectangle. Reuse the complete contact/particle visual review and upstream's native1280 parry receipt; no broader QA/performance sweep.
- Final integrated strict serialize + focused40tests are queued on the merged snapshot with real mtimes. Builder/test evidence will be recorded before commit/push/main integration.
- Closed task Edge profiles removed only after checking no task browser processes remain. Players, exact PNGs, audio, final movies and receipts retained under E:/CodexTemp/TypingHero/parry-impact-burst for user review/reproduction; remove after review when no reproduction is needed. No QA listener/browser left running. Original dirty checkout remains untouched.

## Final integrated acceptance

- `20261024T055845-serialize`, merged inputb5a3b7ab9460c7e453252c2a5dfd7e971262b53adf5fb12672332e12747228fd: strict repeated builder PASS, followed by focused ImpactVfxTests **40/40PASS,0failed/0skipped**,8.361289s. XML/snapshot/builder log retained in final-result; exact input archive retained in the server result. Real source mtimes force correct compilation; no shared cache deletion or runner/toolchain/package edits.
- Whole generated Player prefab and SampleScene match reviewed normal outputs exactly after newline normalization; final profile equals reviewed source. All281upstream depth entries remain semantically identical. This establishes reuse of both full visual reviews after the Editor-only custom-binding correction and upstream integration; no new gameplay/render changes are hidden by a historical clip.
- Final source comparison: MetalClash, physics/render prefab/sprites/light/camera/audio/hitstop unchanged by this task. Profile clone differs only in name, two sizes, core clamp and alpha hold keys. Existing GUIDs/serialized references retained apart from Player's intentional new profile reference. One fixed asset path, no duplicated assets/public API/per-frame work.
- Per-frame evidence is stored as legible UTF-8 CSV with English observations; every source/render index, duration and final movie mapping preserved. Review counts: runtime95+102frames/94+101transitions; final movie398frames/397transitions. Final media checksum and playback receipts are in delivery.json/playback.json/audio-export.json. Movie audio is actual captured destination output; subjective human listening approval is not claimed.
- Source/docs/diff checks complete. Git commit/push/main verification and clean task worktree removal follow; no itch upload.

## Remote integration and retained review artifacts

- Code commit9b3b36965467f1633b8ad85d40eb4a1504e606b0 pushed to the task branch and fast-forwarded to origin/main. A fresh fetch confirms exact remote HEAD and task ancestry. No conflicts, protection bypass, force push or itch upload.
- This receipt-only follow-up changes no build inputs; reuse integrated40/40/strict-builder and full visual/media evidence. Remove the clean task-created worktree/local branch after this receipt reaches main. Actual cleanup receipt is written to E:/CodexTemp/TypingHero/parry-impact-burst/delivery.json.
- Keep local players/raw PNGs/audio/final clip/CSVs/helper receipts and owned server results055845-serialize,045935-webgl,051736-webgl for review/reproduction. Cleanup after review when reproduction is no longer needed. Shared runner/cache/toolchain and original dirty checkout are preserved. Remote task branch remains per repository policy; delete only on explicit request.
