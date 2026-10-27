# Horizontal parry flash

## Acceptance and reference scope

- Successful sword parry only, at the existing contact point, in both directions.
- Horizontal 72 world-resolution pixels; warm white core 1px, gold halo 3px, tapered ends.
- Rise 0–20ms, hold 20–40ms, fade 40–120ms. Existing flash, sparks, light and gameplay unchanged.
- Inspect output preparation, contact, expiry; Pause/Restart/Menu and pooled reuse; typing tray clearance.

## Reference selection (before frame review)

Steam app3489340, ReBlade: The Death Spiral, SNF Trailer (EN), movie257457478.
Source: https://store.steampowered.com/app/3489340/
Video: https://video.akamai.steamstatic.com/store_trailers/3489340/1492852456/bee6c8072e420aa2221843bb4e0d636828d19217/1791775482/hls_264_3_video.m3u8
Overview 0–63s at1fps used only to locate candidate; not counted as frame review.
Selected segment42.0–46.0s,30fps, expected120frames/119transitions: stationary
sword encounter before, during and after gold contact flashes, before the scene cut.
Review pending. Exact ReBlade match remains unverified; requested dimensions are independent art direction.

## Reference review

Directly inspected120/120frames and119/119consecutive transitions, including
held/repeated poses, at original30fps timestamps42+n/30 seconds. Six numbered
contact sheets (0–19,20–39,40–59,60–79,80–99,100–119); no skipped frames.
0–6: preparation/closing;7–10: contact warm-up;11–19: gold/white flare and long
lateral rays;20–34: held contact with diminishing sparks;35–47: recovery and next
preparation;48–69: second contact, narrow lateral ray and held white/gold center;
70–99: release/separation and following approach;100–119: next sword strike/contact.
Transitions19?20,39?40,59?60,79?80,99?100 directly compared across sheets.
The segment ends during the next strike, so its final follow-through is unverified;
the first two full contact/recovery cycles are covered. Images are the extracted
video frames themselves; no independent still proves opposite-facing behavior.
No source assets were imported. No exact reproduction or exhaustive game-reference
coverage is claimed. Requested72px/120ms values remain independent art direction.

## Implementation

- ParryImpact adds channel5, a stationary camera-plane quad using an original
  procedural shader:72×3pixel envelope,1pixel warm-white center, gold halo,
  tapered8pixel ends. Shader uses the existing particle Color/UV stream.
- FlashLayer.sizePixels2D defaults to zero and lockVariation defaults false.
  Only the new layer opts into72×3 and locks rotation/variation. Existing source
  flash bytes, physical profile, parry contact path, flash RNG, lighting, damage,
  hitstop/camera and pause ownership are preserved.
- Gradient alpha at normalized0,1/6,1/3,1 is0,1,1,0; lifetime120ms.
  Reuses emission/pause/lease cleanup; no gameplay API or save-data changes.
- Missing-stock-prefab builder creates the same sixth binding and preserves
  existing authored/custom references on repeat.

## Verification checkpoint

Unity2022.3.25f1, Ubuntu runner,45/45filteredImpactVfxTests PASS,0skip;
result /data/typinghero-game-loop/results/20261027T151249-quick,
input9bc1dc0e68bb8568e05db05771265c7b4d267ec99cea4c28a979a2119850ddad.
Includes48direction/seed/pool-reuse combinations, fixed dimensions/rotation,
20/40/120ms alpha envelope, expiry with old flash still alive, pause/read holds,
shared-profile consumers and missing-prefab/repeated-builder tests. Existing
flash serialized bytes identical; only existing prefab root's children/bindings
changed, plus four cloned particle objects. git diff --check PASS.

Initial QA build stopped at unrelated protected Cobblestone font asset CRLF/LF
serialization. External task adapter normalizes YAML/meta line endings in the
immutable source archive; no checker/toolchain/repository font changes.
Final native input/render checks and integration pending. No itch.io release.

Native right pilot:10input/lifecycle checks PASS,0exceptions/provider writes.
Canvas readback inside the render callback yielded62black images; these are
rejected as visual evidence. The compositor video is valid. Before frame review,
select compositor segment10.0–13.0s (25fps), covering enemy preparation, first
successful parry and disappearance before Restart transition. Review pending.

The right segment was extended to10.0–14.5s to include recovery and Pause/Restart,
not shortened:113/113recorded frames and112/112pairs directly inspected, preserving
25fps held/repeated frames. Cross-sheet pairs19?20,39?40,59?60,79?80,99?100 checked.
Frames0–46 cover reveal/idle;47–69 warning/preparation;70 raised parry;71–74
contact and thin fixed-horizontal flare;75–88 flare gone, old core/sparks recover;
89–100 Pause;101–112 Resume/Restart cover, no lingering line. Full-size contact
frames at12.80–13.04s also inspected. Warm center, tapered gold line and clear
vertical gap above typing tray; no random angle. Recorder25fps cannot independently
resolve20ms phases; exact envelope/expiry is established by Unity tests, not video.

Before reviewing mirrored output, select compositor segment10.0–14.5s from
native-left/cb36759848fab16eb5e4d3c59ae34658.webm, expected113frames/112pairs.
Covers complete enemy preparation, accepted real Space, new flare and physical-tail
recovery through pause. Mirror fixture changes initial actor positions/facing only,
outside repository/bridge; no direct success/spawn setters. Input/lifecycle10checks
PASS,0exceptions/provider writes. Left frame review pending.

## Native visual acceptance

Left10.0–14.5s:113/113frames and112/112pairs directly inspected, including cross-sheet
pairs19?20,39?40,59?60,79?80,99?100.0–36reveal/idle;37–57warning/preparation;
58–60raised parry;61–64contact and stationary horizontal line;65–84existing flash
and particles recover;85–101idle with line absent;102–112Pause, no residue.
Both directions visibly clear the typing tray. Total226frames/224pairs, no sampling.
Selected recordings include held/repeated frames.25fps source limits sub-frame
claims; this is compositor-video coverage, not every internal Unity render.

Final right QA:20261027T151747-webgl, source1d67ca00b76d3ddf5f0e600c6aa557ea73a3ec016cc6ffe5279a1c8c0fb7c3d0.
Mirrored QA:20261027T152503-webgl, source1f9022b9b086f8de7552ae168fdc5b2e684f4b98819d31e4700f3ea421cac09f.
Both builds retain all strict builder checks. Native Edge reports10checks each,
0exceptions and0provider writes. First obsolete start helper clicked during scene
transition and timed out in the guide; corrected external driver waits for scene
settlement before actual Practice click. No game change for this harness issue.

Durable evidence/demos/adapters:
E:/codex-data/artifacts/TypingHero/parry-horizontal-flash-20261027.
right.mp4/left.mp4 preserve all113selected frames at25fps; original compositor
videos and source receipts retained separately. Black render-callback readbacks
are excluded. Canonical code/checkpoint remains in this repository.

Integration with latest main pending. New main score-frame layout is separate
from impact/typing-tray/camera code; inspect the merge and rerun scoped tests.
