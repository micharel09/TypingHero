## Full GIF source copy (58 frames)

[Preview](gif-pack/review.html) / [Source, clip ranges and verification](gif-pack/README.md).
New isolated set: sword, axe, sword + armor/shield; 18 PNG strips and three
58-cel natives, 100ms per cel. Prior variants below remain preserved.

# Skeleton / hero art

Full design pack: [kiếm, rìu và thương — preview](full-pack/review.html),
with72 PNG sheets and six layered/tagged LibreSprite natives.
See [full-pack notes and rebuild commands](full-pack/README.md) for source
provenance, frame counts, timing, verification and reconstruction limits.

New art review: [axe and spear samples](weapon-milestone/review.html), with
Idle0 and16 axe /10 spear Attack cels, side by side with the approved sword and hero.
The two `weapon-milestone/*.aseprite` files have17 axe /11 spear timeline frames with Idle/Attack
tags and separate `Body + weapon` / `VFX` layers. The28 individual PNGs are96×64.
These are review samples only; no runtime integration or gameplay approval.
The revised axe follows the [user's blade reference](references/axe-blade-user.png):
a broad cheek, flared cutting edge, narrow neck and a deep bearded recess;
the spear has a long, narrow leaf point with a metal socket and a continuous rib.
The motion follows row two of the [user chopping sheet](references/axe-chop-user.png),
with a [mirrored copy](references/axe-chop-user-mirrored.png) for the skeleton's direction.
Cels1–3 lift overhead, cel4 folds forward with low hands and a broad blade-sweep
smear reconstructed from intermediate shaft/head positions, and cel5 contacts
with a horizontal shaft, bent elbows and low hands, folded thorax and bent knees.
Cels6–7 compress, then8–9 settle without lifting the head. The actual bright cutting
rim touches y63 and the entire weapon head stays pinned. Fixed grips remain7/15px
along the32px shaft. Idle0 and all spear/hero/sword assets remain unchanged.
Cels10–15 brace, lift the blade, draw inward and return to exact Idle0.
Use **Cel9 · ghim đất**, **Cel10 · hồi thế** and **Cel15 · về Idle** to inspect transitions.
Attack plays once and holds;
Play at the end restarts from zero, while mid-clip pause/resume preserves time.
The source drawing is `weapons-pixel.js`, with cel coordinates and exporter in
`prepare_weapons.py`. Rebuild with the same retained LibreSprite:

```powershell
python design/skeleton-hero/prepare_weapons.py --libresprite E:/CodexTemp/TypingHero/skeleton-hero-art/libresprite/libresprite.exe
```

`verification.json` records native/PNG and body-layer roundtrips, timeline,
binary alpha, bounds, fixed foot contacts and unchanged input hashes.
Native durations use whole milliseconds (axe Attack2053ms / spear Attack1333ms); the preview uses the
original float clip times through cel9, then100/120/140/140/120/100ms recovery, including.7s telegraph and1s impact. Sword and spear hold their final cels during axe recovery. Toggle VFX,
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
