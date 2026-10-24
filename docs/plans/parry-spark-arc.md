# Parry spark and shock arc - 2026-10-24

Current status: implementation, integrated scoped tests and visual/media verification
PASS; verified main push and clean task-worktree removal pending.

## Intent and acceptance
- User selected clearer metal sparks plus a thin directional shock arc after the
  previous 40px flash made fast-cooling 1px embers difficult to see.
- Preserve 200ms flash, lighting, audio, camera, hitstop, particle count and launch
  trajectories. Separate ParryImpact physics/prefab; MetalClash unchanged.
- Leads 2px x10-14, cooling450-550ms/life550-700ms; secondary4-7px,
  cooling400-550ms, brighter orange embers. One hollow120-degree arc,8cels/320ms,
  64px canvas/16PPU, centered contact and mirrored toward opponent.
- Real keyboard parries both directions at1280x720; inspect every captured frame
  and adjacent transition, export audible1x/.25x clips; pool/pause/reuse checks.

## Implementation checkpoint
- Base origin/main0dc4511; independent Orca worktree parry-spark-arc, branch
  micharel09/parry-spark-arc. Original dirty checkout preserved.
- Private directional renderer channel uses one emission-time UV flip; existing
  ParticleSystem sheet animation and pool clocks own the arc. No new public API
  or per-frame handler. Preset restoration includes renderer flip.
- One-time builder creates separate prefab, then preserves authored configuration.
- Sprite source is the approved textual design (no external game reference).
  Local deterministic raster authoring is retained in task artifacts/author.py.

## Verification scope
- Sprite preview: all8right cels/7transitions and all8mirrored cels/
  7transitions viewed at1x/2x. Hollow center, increasing radius and disappearing final
  cel observed. Runtime scale, overlap, timing and playback now qualified below.
- Focused ImpactVfxTests, repeated builder, real input and final frame review PASS.
- Task artifacts E:/CodexTemp/TypingHero/parry-spark-arc retained for reproduction.
- No push/integration or itch.io upload yet.

## Runner receipts and current queue
- Initial serialize20261024T062239 on52f6081b: Unity compilation and strict repeated
  builder passed, new prefab/profile exported. External adapter then failed because
  its copy list contained ParryClash twice; deduplicated the task-local adapter.
  This is not a full runner PASS. Generated ParryImpact prefab/meta/profile copied
  back; unrelated canonicalized ParryContacts output not adopted.
- Extra serialize20261024T062628 on864b63ee stalled after opening SampleScene with
  unchanged log for6minutes/one CPU. Stopped only its verified owned container
  typinghero-loop-2323000-1792823188130282056; exit143, no PASS claim.
- Normal QA b7c11712 started20261024T063750-webgl with the preserved new prefab.
  Focused ImpactVfxTests and external mirrored QA are queued behind shared work.
- Final formatting strips trailing whitespace in new assets; material importer main
  object corrected to2100000. Final serialize/tests will qualify this exact source.
  Earlier visual build uses identical material/texture payloads; verify equivalence
  against final serialized output before reusing that visual evidence.

## Native right review
- QA20261024T063750-webgl, b7c1171279982cb84f5028702beaeb69f0d6511b87ac824e1c5615ccee8d4dd6:
  WebGL/strict repeated builder PASS. Input archive preserved inside this result.
- Final right capture Edge156/Intel UHD D3D11 at1280x720:11checks PASS,
  no exceptions/provider writes. Actual Space parry retains40HP and awards30;
  real Escape Pause/Resume succeeds. PNG capture records the320x180 world raster
  from the actual1280x720 render, with original displayed-frame timestamps.
- Directly reviewed151/151frames and150/150transitions across the complete5.507s
  displayed segment. F0-63idle/cue,64-72windup,73guard,74contact star,75core/arc,
  76-77contracting white streak/expanding arc tips,78-85orange flying streaks,
  86-94embers,95-98guard exit,99-102last embers/idle,103-150clear idle.
  Core briefly covers blade; player eye/body/feet remain legible. Arc bulges right
  with hollow center, then clears while orange sparks fall without visible clipping.
- Physical observations retain multi-pixel streaks through514ms (length2px), then
  orange1px embers through1066ms. This is observed sample coverage, not every RNG
  outcome or a performance/FPS guarantee. Audio/1x/.25x playback not yet reviewed.
- Earlier right attempt1 missed real input under redundant Playwright video capture;
  disabled only that extra recorder, retaining exact PNGs and destination audio.
  Attempt2 produced valid parry images but post-capture40ms Escape was missed;
  retained as failed-native-right-2, reviewed only F0-80, not final evidence.
  Final driver focuses canvas and holds Escape120ms; no game timing/input changed.
- Task artifacts contain per-cel and per-runtime-frame CSV observations. Six render/
  physics payloads equal final source after whitespace normalization; material
  importer main object metadata differs as documented above.

## Focused test correction
- Quick20261024T065619 on the same b7c11712 input executed41cases:40PASS/1FAIL,
  zero skipped,7.2694294s. Only new Pause/Resume fixture failed: it set timeScale0
  before SetTutorialReading(false), whose existing ApplyFreeze intentionally resets
  timeScale1. The test therefore resumed its own effect while claiming a hold.
- Corrected fixture order: leave tutorial reading first, then set timeScale0.
  Product runtime unchanged; final strict serialize plus all41focused tests queued
  against corrected test and final importer/whitespace source.
- Mirrored QA854d4967 started20261024T065830-webgl, adding only the external editor
  actor-mirror fixture to the original b7c11712 runtime input.

## Final acceptance receipts
- Final serialize20261024T070951, immutable input
  6331af8e49955345088499ef92351e3e3b92a6171836a265ba6eab413c4cc4ed:
  strict repeated builder and41/41ImpactVfxTests PASS,0failed/0skipped,8.7500326s.
  Before main integration, build inputs exactly matched this digest. All new rendered
  assets, physics and material importer match generated output after whitespace
  normalization; no authored settings changed after qualification.
- Left native11/11PASS,0exceptions/provider writes, same Edge156/Intel UHD/D3D11/
  1280x720 as right. Reviewed101/101frames and100/100transitions: F0-22idle/cue,
  23-32windup,33guard,34star,35core/left arc,36-37contracting streak/arc tips,
  38-41orange rays,42-49embers,50-54exit,55-100clear idle. All directly viewed;
  mirrors correctly, player eye/body/feet remain readable, no observed floor clipping.
- Total runtime252/252frames/250/250transitions and source16/16cels/14/14transitions
  directly reviewed. Per-frame observations and original durations are in the task
  artifact CSVs. Both real keyboard parries retain40HP/+30; post200ms multi-pixel
  sparks, tail expiry, actual Escape Pause/Resume and all fixture guards pass.
- Final preview ParrySparkArc-1280-1x-025x.mp4:
  SHA25601e7b0f93a215c638a94a657817ac2ae2e6bbfbf442cdeb840fcc9778c425ac7,
  1280x720/46.997s/H264level4.2/AAC. Directly inspected508/508decoded frames and
  507/507transitions plus40contact crops. Blocks: right1x152frames, right.25x152,
  left1x102,left.25x102. Cuts afterF151/F303/F405 are intentional take/speed changes;
  each block includes one terminal idle repeat. Full decode and strictly increasing
  video PTS pass; source displayed-frame timing retained to1ms, no interpolation.
- Original MediaRecorder video is only an audio carrier; exact displayed PNGs own
  visual timing. Leading Opus timestamps are padded as silence before trimming the
  first-PNG offset and applying tempo changes. Four decoded peaks0.47-0.53.
  Both directional clips played through in headed Edge at1x/.25x, unmuted/volume1/
  pitch preserved; combined final clip also opened and played through to46.997s.
  Independent subjective listening approval is not claimed.
- Export pilot rejected an incompatible FFmpeg -r/VFR option; corrected encoder
  FPS metadata without resampling and verified H264level4.2 plus508frame mapping.
  No product code changed for export. No broad performance/full suite, normal
  production build or itch.io upload; no new public API or per-frame handler.
- Origin/main first advanced to213e67d with startup documentation, then to c86d72f
  with the independently verified scene-aware depth editor fix5a663cd. Merged into
  task at d54c05f after code4309f64 was pushed to the task branch; retained both
  sections of the verification-document conflict. No shaders/runtime/assets were
  changed upstream. Re-running strict builder/41tests and comparing generated
  ActorDepth/Player/scene/VFX before final main push.
- Shared historical parry driver filtered only the old prefab name ParryClash;
  updated its private role predicate to accept ParryImpact too, retaining the same
  particle limit and rejection of other effects. Actual captured151right/101left
  frames (30/23live frames) pass; old ParryClash accepted and Normal rejected.
  Node syntax/VM checks PASS. This Node-only consumer correction is independent of
  Unity rendering/tests and does not require another player build or frame review.

## Integrated qualification
- Merged c86d72f then7694ba1; final code line includes4309f64 and both depth-editor
  fixes. Actual integration run20261024T074942-serialize on
  65e72571444019f0768dd8e9188d7fe2f82839361ea6d9198976bf16500f23f8:
  strict two-pass builder and41/41focused tests PASS,0failed/skipped,8.7472983s.
  ActorDepth/Player/VFX payloads match source, including untouched MetalClash.
- Generated scene IDs differ between independent builder processes. Canonical
  hierarchy/component identity comparison covers all1121records, every local/GUID
  reference/listener and float32 value:0added/0removed/0changed versus the reviewed
  normal QA scene. This is not acceptance based on selected scene fields.
- The later receiver-scope editor fix7694ba1 has verified31/31Editor tests and a
  real two-pass builder, preserving281mapped frames/0missing, library/world/material
  output and99lighting PNGs. Read its actual XML and independently matched all11
  listed source/config/index hashes to this checkout. ParticleSystem effects are
  outside SpriteRenderer receiver collection; our contact library/actors remain
  unchanged. Reuse the integrated41tests and reviewed images for unchanged runtime
  and render data; no extra WebGL/full profile is necessary for this editor-only fix.
- Private Node role-filter correction verified separately over all252captured
  frames, including53live-effect frames. It is the only tool-input delta after the
  integration snapshot; Unity source/assets tested by that run are unchanged apart
  from the independently qualified upstream editor fix above.
- Detailed machine receipts: integration-equivalence.json, scene-equivalence.json,
  latest-upstream-equivalence.json, upstream-editor-tests.xml and qa-consumer.json
  under the retained task artifact root. Final Git/cleanup receipt is delivery.json.

## Retained review resources
- E:/CodexTemp/TypingHero/parry-spark-arc contains final combined/per-direction
  audible clips, immutable QA players, raw PNG/audio captures, frame/cel review CSVs,
  decoded final images, driver/author/export scripts and receipts. Keep until user
  review/reproduction is no longer needed; then these task-owned artifacts may be
  removed. Final source/input archives remain in the named isolated server results.
- Temporary remote media-export directory, duplicate local transfer tarballs and
  four closed Edge profiles removed after verification of all five MP4 hashes;
  no owned Edge process remained. Original checkout/resources remain untouched.
  Task worktree/local branch will be removed after remote proof;
  remote task branch is retained under repository policy until explicitly cleaned.
