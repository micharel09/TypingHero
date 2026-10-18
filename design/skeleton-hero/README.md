# Skeleton / hero art

New art review: [axe and spear samples](weapon-milestone/review.html), each with
Idle0 and ten Attack cels, side by side with the approved sword and hero.
The two `weapon-milestone/*.aseprite` files have11 timeline frames with Idle/Attack
tags and separate `Body + weapon` / `VFX` layers. The22 individual PNGs are96×64.
These are review samples only; no runtime integration or gameplay approval.
The revised axe follows the [user's blade reference](references/axe-blade-user.png):
a broad cheek, flared cutting edge, narrow neck and a deep bearded recess;
the spear has a long, narrow leaf point with a metal socket and a continuous rib.
Head silhouettes, steel faces and cutting edges are drawn per cel, including the
wind-up and impact angles. The axe now contacts the ground at impact (cel5),
compresses and holds at6–7, then settles at8–9 with the blade still on y63.
Its final cel intentionally ends grounded. Both grips follow the32px shaft;
the axe trail reaches the new ground contact on its separate VFX layer. Original
timing, the first five body poses and every spear/hero/sword image are preserved.
Use **Cel cuối · chạm đất** to inspect the actual ending pose.
The source drawing is `weapons-pixel.js`, with cel coordinates and exporter in
`prepare_weapons.py`. Rebuild with the same retained LibreSprite:

```powershell
python design/skeleton-hero/prepare_weapons.py --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe
```

`verification.json` records native/PNG and body-layer roundtrips, timeline,
binary alpha, bounds, fixed foot contacts and unchanged input hashes.
Native durations use whole milliseconds (Attack1333ms); the preview uses the
original float clip times including.7s telegraph and1s impact. Toggle VFX,
onion skin, mirror, scrub/step and1×/.25×; view1280×720,640×360,4×/8×.
VFX toggle applies to the new samples; the approved sword retains baked effects.
The hero is the approved static Idle0 reference. Final art remains subject to
user review. The existing sword history below is unchanged.

Preview QA: `node design/skeleton-hero/verify_weapons.cjs` with the already
available Playwright module on `NODE_PATH` and installed Edge. An optional final
argument saves canvas captures to a chosen scratch directory. No server is needed.

The user approved **Idle0 + all10 Attack1 frames** on2026-10-18.
Their PNG/native/mask hashes are locked in `cluster-milestone/approval.json`.
The full36-frame set is exported and integrated into the four runtime PNGs.
Runtime verification is recorded in the [existing plan](../../docs/plans/skeleton-hero-art.md).

Open [review.html](review.html) locally: all36 frames, six original Unity clips
(Idle, Attack1, Hurt, Die, Stun, hitBySlayer), scrub/step,1x/.25x playback,
overlay/mirror, masks and color differences at1280x720,640x360,4x/8x.
The arena is reconstructed for comparison; this preview is not a Unity render.
Column3 retains the intermediate palette-only reference for the11 approved samples;
for the other25 it explicitly shows originals. Diff always compares against column3.

`complete/` contains36 editable LibreSprite sources,36 PNGs96x64, protected masks,
four native/PNG sheets. The exporter verifies that all alpha pixels and locked
weapon/grip/VFX RGBA match originals. Hurt1 is an original baked full-body flash:
its entire RGBA is preserved. Stun and hitBySlayer consume the same Hurt sheet.
Skull/rib/joint light planes follow each original pose; death fragments and the
compressed skull remain in their original positions. Bone highlights use RGB250,
cool shadows use hero armor colors and deep cavities use restrained plum.

Rebuild from the repository root with the retained portable LibreSprite1.3:

```powershell
python design/skeleton-hero/prepare_complete.py --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe
python design/skeleton-hero/build_preview.py
```

`prepare_complete.py --integrate` additionally copies only the four verified PNGs
to their existing runtime paths. It checks the historical baseline without resetting
it, rejects unexpected drift and verifies every native re-export. Two unrelated
scripts already differ from that baseline: TutorialOverlay.cs and SlayerScreenFX.cs.
The concurrent player-attack-sorting task's Player.prefab addition is separately
verified by its exact hash; it is retained without resetting the historical baseline.
Metadata, animation/events, prefab, hero, camera and combat are not edited.

`remaining-poses.json` owns brush coordinates for the distinct remaining poses;
`original-pixel-edit.js` paints inside source alpha in LibreSprite. The exporter
copies approved cels unchanged, reuses their paint for exact RGBA-identical originals
(including matching Idle/Hurt/Die start/end cels), pads weapon materials at overlaps
and locks visible VFX differences. `originals/` and `baseline.json` retain originals.
`preserved-original-milestone/` is the intermediate palette-only reference.
`attack-milestone/`, `samples/` and imagegen studies are rejected historical studies.

No release, stress or soak is part of this task. Portable LibreSprite remains at
`E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/` for future source edits.
