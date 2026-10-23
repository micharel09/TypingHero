# Reusable 3D lighting and flames

The authoritative configuration is authored data. Runtime state belongs to each
instance; builders apply data and preserve existing assets. The default look is
the approved dungeon/Pinterest flame, not a runtime fallback.

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

Pause and Slayer hiding preserve each instance's animation/flicker phase. Resume
continues it; source meshes/materials are never animated in place. World unload
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
cycles, normalized start frame/frame curve, height/width ratio (`heightScale`,
default1), sorting order and native color/alpha gradient are
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
profiles and pure generic prefabs. Production also ships GuardBreak and
BodyFallDust under `Assets/Art/VFX/EnemyState/`: original Pimen flipbooks, empty
physics groups and local light disabled. Guard uses a byte-identical2D copy of
Effect2 because the original pack importer is a cubemap; its original GUID/meta
are retained. Dust retains all eight cells (six visible, two transparent).

Combat end stops bursts by default. Set context.completionPolicy to
`ImpactVfxCompletionPolicy.AllowUntilResults` only for the unlit death tail:
its scene-owned emitter can emit after CombatSession.Finished and lives until
natural expiry or results. Result overlay, Clear, disable and scene unload always
clear it; pause/reading hold it. Shared combat lights retain their session owner.
The EnemyStateVfxBinder chooses events/anchors; appearance stays in the profiles.
Its actor emitter and independent scene death emitter each have two active leases.

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
