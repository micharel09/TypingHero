# Full GIF source copy

Approved by the user on 2026-10-18. This `gif-pack` is the approved design set
for the three source skeletons; gameplay integration remains a separate task.

[Open preview](review.html). Three characters: sword, axe, sword with armor/shield.
White visible composite only, including baked VFX. No Yellow or reconstructed
pixels under effects. Existing `full-pack/` and other variants remain separate.

Source: [original 1100×600 GIF](https://img.itch.zone/aW1hZ2UvMjU2ODY4OC8xNTMwODk3OC5naWY=/original/VboK7e.gif),
saved byte-for-byte at [references/monopixelart-full.gif](../references/monopixelart-full.gif).
58 source frames, each 100ms; full timeline 5.8s, including repeated poses.
The earlier conclusion that the public GIFs lack full axe animation applied to
the nine-frame overview; this complete GIF supplies the full visible timeline.

Each character has six horizontal PNG strips and one 58-cel `.aseprite` native
with six tags in source timeline order. One layer: `Visible White composite + VFX`.
All cels are 96×64. Source-native offsets are (0,18), (64,18), (128,18);
source ground y82 is excluded, standing feet y81 map to canvas y63.
Body, weapons and effects share the same fixed translation. No rescaling,
recoloring, deduplication, independent weapon repositioning or timing changes.

| Character | Idle | Hurt | Walk | Die | Attack1 | Attack2 |
|---|---|---|---|---|---|---|
| Sword / sword-shield | 0–7 | 8–12 | 26–35 | 13–25 | 36–47 | 48–57 |
| Axe | 0–7 | 8–12 | 13–24 | 25–35 | 36–47 | 48–57 |

`ownership.png` is a lossless label map: each 220×120 tile is one source frame;
labels 0/1/2 identify the characters above, 255 identifies scenery. The
frame-specific rules in `prepare_gif_pack.py` remove background, border and
ground while retaining disconnected death fragments and weapon/VFX pixels.

The GIF is already composited. Frame41 sword VFX meets the axe shaft; frame42
axe VFX overlaps shield and forearm; frames50–51 axe head/hand cover the sword
thrust; frame52 axe sweep overlaps the shield; frames53–54 axe head touches the
shield region. Same-colored white intersections cannot reveal their original
layer ownership or hidden silhouette. Masks follow the visible edges; no hidden
pixels are filled. Sword-shield attack VFX at frames41–42 and50–52 also reaches
the source's right border: any pixels outside the GIF are unavailable.
Roundtrip equality proves fidelity to the declared masks, not recovery of the
original premium layers or certainty at same-color intersections.

The preview uses the same source frame for source/export/50% overlay, mirror,
4×/8× nearest zoom, clip/full timeline, 1×/0.25×, pause/resume, replay, scrub
and cel steps. Source view intentionally shows neighboring pixels and the border
for auditing. Playback loops the selected interval, including Die.

From repository root:

```powershell
python design/skeleton-hero/prepare_gif_pack.py
python design/skeleton-hero/verify_gif_pack.py --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe
$env:NODE_PATH='E:/CodexTemp/TypingHero/itch-shogun/pw158/node_modules'
node design/skeleton-hero/verify_gif_pack.cjs
```

Verification: all source 5× blocks uniform; 174 masked cels have exact source RGB,
binary alpha, silhouette, fixed canvas and ordering. All 18 PNG strips roundtrip
through the three LibreSprite natives with six tags and 100ms per cel. Browser
checks cover 168 combinations at1280×720/640×360 and4×/8×, plus controls and both
speeds. Contact sheets were inspected for attack ownership and death fragments;
this is a design preview, with no Unity/gameplay/API/Assets integration.
