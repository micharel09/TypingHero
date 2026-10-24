# Reusable 3D lighting and flames

The authoritative configuration is authored data. Runtime state belongs to each
instance; builders apply data and preserve existing assets. The default look is
the approved dungeon/Pinterest flame, not a runtime fallback.

The selected DungeonWorld disables the RearRune decoration and its RuneLight.
The user rejected that blue rear glow; keep the existing torch lighting as the
environment light sources. Shared rune assets remain available to other worlds.
RearHaze and LowHaze are also disabled: their blue decorative overlays produced
the remaining horizontal glow after the rune was removed.

## Ownership

| Data | Authoritative owner |
| --- | --- |
| World scene selection | `WorldDefinition` scene picker; runtime scene path is derived Editor metadata |
| Ambient, fog, camera, world resolution, depth blur, pinned cloth, rune modulation | `WorldLightingProfile` |
| Exposure, bloom, grading | Referenced native `VolumeProfile` |
| Pipeline/shadow atlas limits | Referenced native URP asset and renderer assets |
| Bake settings | Referenced native `LightingSettings`; baking is explicit |
| Light type, enabled state, color, intensity, range, shadows, flicker | `LightProfile` |
| Flame material/atlas, source mesh/pivot, UV sequence, per-frame duration, content padding | `FlameProfile` and its native material/mesh assets |
| Palette remapping/thresholds, alpha cutoff, brightness | Flame material; disable remapping to retain arbitrary source colors |
| Haze taper/fades/ellipse/drift and rune size/strokes/scan | Existing atmosphere and rune materials |
| Contact zoom envelopes, recoil duration/curve and Slayer priority hold | `CombatCameraProfile`, referenced by `WorldDefinition.cameraImpact` |
| World combat-light templates, overlap, decay and ordinary/legacy impact pulses | `CombatLightingProfile` |
| Impact pulse enabled, color, intensity, duration, hold, decay, range and offset | Existing `ImpactVfxProfile`; copied into each pulse at emission |
| Fixture placement, flame anchor, noise phase and animation offset | Serialized scene/prefab instance |
| Clock, current frame, pulse leases and private UV mesh | Runtime instance only |

Profiles have Create menu entries under **CLACKBLADE / Lighting**. Duplicate an
existing asset to make a variation. Materials, post profiles and meshes are shared:
duplicate those too when a variation should not affect other users of the asset.
There is no profile inheritance or implicit per-instance style override.

## Blur, atmosphere, cloth and camera impacts

`focusOffset` is the signed eye-depth offset of the **center** of the sharp region
from the player. `focusHalfWidth` controls its half-width independently;
`depthEdgeDistance` is the depth difference where a blur sample receives zero
weight. Existing production and test profiles have been migrated once from
`focusOffset=.75` to `0`, with half-width `.75` and edge distance `.5`.
The builder never migrates or resets these fields. Gaussian weights/tap count are unchanged.

Edit haze and rune geometry/drift/scan directly on their existing materials.
The original values are explicitly serialized on every current material.
`_DungeonAge` and `_DungeonPulse` remain per-instance property-block values;
materials contain no shared moving clock.

Cloth uses world-profile amplitude `.025`, speed `1.25`, length frequency `2`,
and local Z direction by default. Instances capture these on enable, retain the
pinned upper edge and own their mesh; reload/re-enable to apply structural edits.

Assign a `CombatCameraProfile` through `WorldDefinition.cameraImpact`. Six presets
cover normal, parry, received, Slayer entry, lethal and Slayer-lethal. They own
priority, zoom curves/times, shake units/direction/envelope, delay and hitstop gate.
Defaults retain the existing zoom/pixel strengths and Slayer entry curve/gain.
Binding prepares private data once; world changes clear old requests. Explicit
`Configure` prepares future requests while accepted requests keep their snapshot.
Combat displacement uses a direct Noise extension rather than the old channel2
impulse path. See the [camera guide](camera-impact-profiles.md) for Inspector steps.

## Dynamic sprite shadows and authored normals

The default dungeon uses one hard-shadow key (front lantern24,512px faces), two
rear fill lights (8, no shadows), and at most one active spatial combat light
(256px faces). Parry now casts shadows as well as illuminating receivers.
The2048 shadow atlas and Point/HDR/MSAA presentation are unchanged. The shared
URP per-object additional-light limit is8, covering the current selected worlds'
fixtures, rune and combat pulse without dropping a light at contact.

Actor albedo/alpha/UVs drive the same clipped silhouette in color, depth and shadow
passes. This is a flat animated sprite casting onto3D receivers, not volumetric
character geometry or limb-on-body self-shadowing. Other light contributions can
brighten a region shadowed from the key; no event fades every world's shadowStrength.
The existing strongest-pulse scheduler, contact positions, pause clock and decay
remain authoritative. Intensity/hold/duration/color remain in their existing profiles.

Secondary normal PNGs are authored data. Routine surface preparation preserves an
existing map and custom binding. To deliberately replace a selected **source sprite**
normal with the old contour draft, use **CLACKBLADE / Lighting / Replace Selected
Normal With Generated Draft**. This is destructive for that binding/map; builders
never invoke the replacement. Keep source albedo and sprite slicing unchanged.

The production actor maps use the explicit offline recipe in
[author_actor_normals.py](../../tools/lighting/author_actor_normals.py): semantic
skin/cloth/steel/bone/wood regions, broad plane biases and bounded contour curvature,
not albedo luminance as height. With the existing Pillow tooling, run without
arguments to list targets; **--write replaces all listed actor normal PNGs in place**.
Edit the recipe or paint the normal PNGs directly, then review every affected pose,
mirror, loop and transition under moving light before accepting a revision.
There is no normal generation or material lookup added to gameplay updates.

## Add a fixture or flame animation

1. Place a reusable hanging-lantern or wall-torch prefab from `Assets/Data/Lighting`.
   Move its transform and assign a `LightProfile`. A light-only profile with no
   flame hides the existing card and creates no private mesh. Positions do not depend on object
   names, scene names or a fixed number of lights.
2. Change style in a duplicated light profile. Noise phase and animation offset
   remain instance fields; using one style does not share clocks or mutable UVs.
3. For a different animation, duplicate `FlameProfile`, assign a compatible material
   containing the atlas and an authored common-pivot mesh, then enter each frame's
   normalized UV rectangle and duration in order. Repeated frames and unequal
   durations are supported. `meshUV` describes the source mesh's own UV rectangle;
   it is remapped into each target frame. The mesh preserves aspect/pivot; the
   visible-height/canvas-height ratio accounts for transparent padding.
4. Set the projected flame height in logical world pixels on the light profile.
   **Apply / Preview** computes its scene scale at the authored camera. It does not
   rescale the fixture each gameplay frame.
5. Use **Validate**, then **Apply / Preview in Loaded Worlds** outside Play Mode.
   Scene changes support Undo. Apply validates first and never overwrites profile,
   material, atlas or post-process values.

### Cinematic wall torch preset

The two wall sources share RearTorch: Point Light, base intensity8, range15,
shadows disabled, with synchronized flame emission. Flicker uses SinePair,
amplitude0.18, angular frequencies(5.8,13.7)rad/s, weights(0.7,0.3) and phase
factors(1,2.37). Instance phases0.7/2.9 remain independent. Noise speed/row apply
only to Perlin mode; no runtime constants replace these authored settings.
Intensity stays within6.56..9.44; the central MainLantern preset is unchanged.

TorchBeam1/TorchBeam-1 are saved inactive and removed from the atmosphere update
array. Their old additive cards caused the long fixed-looking pools under the
flames. Retain their assets for authoring; enabling a card again also requires an
explicit atmosphere binding if it should animate. The Point Lights illuminate the
wall directly. No extra lights, shadow maps, RTs, shader passes or volumetrics are
introduced; flame atlas/palette/UV timing remain the authored10x100ms sequence.
Pause and Slayer preserve the existing phase clock; this preset changes data only.
Evidence and final integration status: [lighting checkpoint](../plans/combat-hanging-light.md).

## Add a world/map without changing C#

1. Save a copy of the default world scene. Author the new room layout and fixtures
   using normal scene/prefab tools; the old procedural dungeon art passes are not
   part of rebuilds.
2. Create a `WorldDefinition`, select the new scene, and assign lighting/combat
   profiles. Share the existing profiles or duplicate the required styles.
3. Assign this definition to the scene's root `DungeonWorld` owner. Its environment
   and camera are explicit references; the camera needs `DungeonArenaCamera` and a
   Cinemachine brain. The reusable world-camera prefab supplies the camera setup.
4. Apply/preview the loaded world and enable its scene in Build Settings. Camera,
   arena, brain and environment are the four semantic gameplay binding roles;
   adding/reordering lights does not change the contract.
5. Assign the definition to the menu entrypoint and gameplay binding gate for
   direct Editor entry. `DungeonSession.LoadWorld(definition)` selects another
   world before gameplay. The selected world survives gameplay reload/menu return.
   During binding, actors, viewport, Slayer and combat lights receive that world's
   profiles before gameplay activation.

Maps share the selected project URP asset; the lighting authoring pass validates
the profile reference and renderer slots against that pipeline.

The canonical UI builder keeps MainMenu/SampleScene as application entrypoints.
It gets the default world scene from the gameplay gate's `worldDefinition`, the
room/camera from `DungeonWorld`, and combat lights from the serialized Slayer
binding. Names and lighting asset folders are not selection rules. It partitions
that authored world, then applies profiles to all definitions reachable from enabled
Build Settings scenes, including menu/gameplay selections. Referenced world scenes
must be enabled; unused templates do not block a build. Selected worlds' actual
asset dependencies and metadata participate in repeat checks and dependency export.
The builder preserves authored room geometry/camera positions and restores the
previous scene setup. Preview changes remain undoable. Gameplay binding sets each
selected world's virtual lens and camera-impact profile before activation. Root world geometry,
actor/gameplay placement and pixel-art presentation contracts remain scene/game
authoring responsibilities; this feature does not add procedural maps or a map UI.

With world and gameplay scenes loaded additively, Preview also updates combat rigs
in the explicit `roots` of gates whose `worldDefinition` matches that world.
Other gates and unbound gameplay rigs retain their profiles. One Preview is one
Undo/Redo step for all loaded worlds, including fog, scene bake-settings references
and affected combat rigs. A null profile `baking` leaves the scene's existing bake
settings unchanged; Preview does not bake or edit the referenced settings assets.
The Unity 2022.3 scene `LightmapSettings` object is obtained through the cached
internal `LightmapEditorSettings.GetLightmapSettings` method for Undo; recheck this
Editor-only contract when upgrading Unity.

Camera prefab instances may sit outside the environment hierarchy. Apply records
overrides for Camera, DungeonArenaCamera and UniversalAdditionalCameraData using
the same camera-target list as Preview Undo, so lens/profile/renderer changes
survive saving and reopening without editing the source prefab.
`LightingAuthoringChecks.VerifyPreviewAndRepeatBuilder` requires
`TYPINGHERO_QA=1`; unset/other values fail before scene or Build Settings changes.
The strict PASS message is emitted only after the QA two-pass check returns.

## Lifecycle and checks

Pause freezes each instance's animation/flicker clock. Current Slayer presentation
covers the world with its composite while the world objects and their clocks remain
active; exiting reveals the continuing phase without resetting it. Disabling and
re-enabling a fixture retains its accumulated age. Source meshes/materials are never
animated in place. World unload
releases owned meshes, cameras and render textures. Structural profile changes use
Editor apply and scene/session reload, not a runtime settings menu.

URP 14 exposes additional-light shadow tier through a read-only getter. The pinned
serialized package field is configured once at activation using cached reflection;
the scene authoring pass also serializes it. Package upgrades must recheck this
contract. Render algorithms and shader pass/property contracts remain code; artistic
values are authored data. Point pixel presentation remains the game's renderer
contract. Hardware quality tiers are outside this revision.

Focused checks cover authored edits surviving rebuilds, irregular frame sequences,
independent instances, pause/hide/restore, pulse snapshots and two different worlds.
The test-only second world and profiles live under `Assets/Tests/Fixtures/Lighting`
and are not enabled in production Build Settings. Results and visual evidence belong
to [the lighting checkpoint](../plans/combat-hanging-light.md).


## Author reusable impact effects

Duplicate `Assets/Art/VFX/ParryClash/MetalClash.asset` to tune another effect.
Assign its `physics` and `prefab`. A new prefab needs `ImpactVfxBurst`, particle
systems with emission disabled/play-on-awake off/world simulation/unscaled time,
and renderer bindings containing unique IDs, systems and appropriate materials.
`PooledVFX` is supplied by the emitter when absent. No ParrySystem or
ParryClashBurst is required. Legacy Current/A/B/C remain only on the old adapter.

Each physical group's `channel` and each flash's `channel` names a binding ID,
not its array index. Physical groups can share a renderer; a flash must have its
own renderer. An empty flash array means no flash. Counts, lifetime, tint,
pixel size, optional world size cap (`0` means uncapped), grid columns/rows,
cycles, normalized start frame/frame curve and native color/alpha gradient are
editable per layer. Bind materials suitable for each layer; Compact physics
materials retain their shader's Custom1/UV stream contract.

`pixelsPerUnit` defines physical pixel geometry and floor clearance. Flash size
uses the greater of one authored pixel and the orthographic camera's actual
world-raster pixel; perspective uses the authored pixel size. `unitsPerMeter`
defines the default local-light world scale; a positive context scale overrides
it for that emission. Local light range/offset are multiplied by that scale.
Enabled/color/intensity/hold/duration/decay/range/offset belong to the impact
profile; the scene's ClashLightingRig supplies the real light templates and
shared overlap budget. No rig means particles/flashes still play without light.

```csharp
emitter.Play(profile, new ImpactVfxContext {
    point = contactPoint, direction = facingRight ? 1 : -1,
    floorY = floorHeight, seed = sequence++
});
```

Optional recipient bounds are captured with
`PhysicalParticleProfile.RecipientSnapshot.FromBounds(body.bounds)` and assigned
to context.recipient. The snapshot remains fixed after emission. Material/prefab
references keep normal Unity asset ownership; copied profile values, physics
settings and native curve keys remain independent of later profile edits.

The emitter validates the fixed40-state physics budget and renderer bindings
before leasing. `maxActive` defaults to8 across prefab pools. Pause/reading holds
age and native flash clocks; hitstop keeps unscaled VFX running; Clear/disable/
scene unload cancel owned particles and pulses. Sound/damage/camera/hitstop are
caller responsibilities. Rebuilds preserve authored references/arrays/curves;
one-time legacy MetalClash migration is versioned. Tests use alternate temporary
profiles and pure generic prefabs; production still ships only MetalClash.

Local light may outlive all particles/flashes, including a light-only profile.
The burst waits for its registered pulse in active rigs before returning to the
pool; no rig, disabled lighting, or a rejected/evicted pulse adds no delay.
The rig owns the clock, so pause and emission snapshots remain shared with pulse
rendering. Explicit Clear/disable/unload and the emitter's capacity eviction still
cancel immediately. Pool return may follow pulse expiry by one Update frame.
Clearing a legacy 2D rig restores its captured base color; edits made while the
rig is disabled become the base color on its next enable.

Pulse age uses each rig's private double-precision realtime clock and the captured
emission timestamp, not the whole receiving frame's delta. Pause/reading transitions
exclude their exact interval through FreezeChanged; direct timeScale0 changes are
observed conservatively without resume catch-up. Positive-scale hitstop still uses
unscaled elapsed time. Existing pulses are sampled before overlap admission, so
expired or weakened pulses cannot incorrectly block a new pulse. Disable unsubscribes
freeze events; enable starts a fresh clock. Post-emission stalls count toward the
authored duration: this is not a render-acknowledged peak-hold system. No new profile
field or migration is required; MetalClash retains5intensity/70ms hold/280ms duration.

Camera impact authoring, render acknowledgment and profile reload rules: [camera guide](camera-impact-profiles.md).


### Sprite depth self-shadow

`WorldDefinition.spriteDepth` selects a `SpriteDepthLibrary`. DungeonWorld uses
`ActorDepth.asset` in normal builds. The user accepted demo `8293` as the visual
baseline, with the rear rune/light and both blue haze overlays disabled. Acceptance
for this integration is the approved demo plus scoped integration checks; it does
not claim a new complete animation review. The builder collects current gameplay
references and rejects missing/stale metadata or maps, without a fixed cel count.
This approximates occlusion on a planar sprite; it does not add character geometry
or replace the existing silhouette shadow caster. Leave `spriteDepth` unassigned
for a world that should use the previous rendering path.

Author an RGBA atlas matching the original dimensions/slicing: R is depth towards
local -Z (0 back,1 front), A is valid opaque coverage. Use linear Default texture,
Point, no mipmaps, no compression. Paint anatomical/weapon overlap per pose; do
not convert albedo brightness into depth. Keep source albedo, pivot, PPU and GUIDs.
Bind it as `_DepthMap`, or put the authored map at
`Assets/Art/Lighting/Depths/<source-texture-guid>.png`. Select the depth library and
use **CLACKBLADE / Lighting / Rebuild Selected Depth Library Metadata** to bind
existing maps and serialize frame UV rect/texture dimensions/PPU. This command
never generates or overwrites painted PNGs. Routine builds validate selected
world libraries and do not regenerate maps; incomplete unselected libraries do
not opt into validation. Packed/rotated runtime atlases are not supported by this
original-UV metadata path.

The actor material saves `_SelfShadowStrength` = 0.65, `_DepthScale` = 0.15
local units and `_SelfShadowBias` = 0.01 local units. Authors may tune each
material independently. `SELF_SHADOW_SAMPLES_PER_LIGHT` = 8 and
`SELF_SHADOW_MAX_DISTANCE_TEXELS` = 12 are fixed technical limits in
`SpriteDepthVisibility.hlsl`. The shader probes 1,2,3,4,6,8,10,12 source texels
towards each light, staying inside the current frame. Only direct diffuse/specular from that light is attenuated;
ambient/emission and other light contributions remain independent. A sprite
without a bound frame uses the previous URP PBR path. World bind prepares one
lookup, and clear removes the prior lookup; `SpriteDepthBinding` merges its
properties with combat tint/flash via `GetPropertyBlock`.
The active `DungeonSessionBuild` attaches the component before serializing the
gameplay roots; the retired world-lighting builder is not the attachment path.
Mapped actors also receive `_DepthFacing` from SpriteRenderer flip flags so the
ray and sampled normal follow the source UV orientation. Transform scale is
handled by the object/world matrices; do not apply its sign a second time.

For the isolated two-pose lab, run Editor entry point
`SpriteDepthDemoBuild.WebGL` with `TYPINGHERO_QA=1` and
`TYPINGHERO_BUILD_PATH` pointing to a task-owned output directory. It copies the
world into a temporary scene, builds with `TYPINGHERO_DEPTH_DEMO`, then removes
that scene. Normal players exclude the lab component. Use the native1280x720
WebGL view for visual judgment; `SpriteDepthRenderChecks.Investigate` provides
analytical/per-light checks and offscreen diagnostics, not gameplay presentation
or performance acceptance.

The explicit offline authoring command is
`python tools/lighting/author_actor_depth.py --recipe tools/lighting/actor_depth_poses.json --write`.
It rasterizes per-cel anatomical masks after checking each source PNG hash; only
source alpha supplies coverage. Without `--write` it reports targets. Builders do
not call it. Edit masks or paint the depth PNGs directly and re-review affected
frames/transitions; rerunning `--write` deliberately replaces the listed maps.

`GameLoopBuild.DepthPreview` builds the real enabled gameplay scenes using the
saved production world/material configuration and requires the isolated QA runner.
It never selects a library by path or rewrites the world. Its QA-only F8 key toggles
self-shadow for same-player A/B, restoring each material's captured strength when
turned on. Disabling/destroying the preview control restores all captured values;
scene loads while OFF retain the original values. Normal player builds exclude
that control and the F8 handler. `SpriteDepthRenderChecks.CaptureAll`
exports flat/OFF/ON, four white-light directions and both facings for every
referenced cel; export alone is not visual acceptance.

Existing static A/B, adjacent-pose observations and same-build performance remain
scoped to their recorded source. The user's demo acceptance supersedes the earlier
pending full-playback gate; no complete timed-animation review is claimed. See
[the lighting checkpoint](../plans/combat-hanging-light.md) for evidence and current
integration status.
