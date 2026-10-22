# Reusable 3D lighting and flames

The authoritative configuration is authored data. Runtime state belongs to each
instance; builders apply data and preserve existing assets. The default look is
the approved dungeon/Pinterest flame, not a runtime fallback.

## Ownership

| Data | Authoritative owner |
| --- | --- |
| World scene selection | `WorldDefinition` scene picker; runtime scene path is derived Editor metadata |
| Ambient, fog, camera, world resolution, depth blur, rune modulation | `WorldLightingProfile` |
| Exposure, bloom, grading | Referenced native `VolumeProfile` |
| Pipeline/shadow atlas limits | Referenced native URP asset and renderer assets |
| Bake settings | Referenced native `LightingSettings`; baking is explicit |
| Light type, enabled state, color, intensity, range, shadows, flicker | `LightProfile` |
| Flame material/atlas, source mesh/pivot, UV sequence, per-frame duration, content padding | `FlameProfile` and its native material/mesh assets |
| Palette remapping/thresholds, alpha cutoff, brightness | Flame material; disable remapping to retain arbitrary source colors |
| World combat-light templates, overlap, decay and ordinary/legacy impact pulses | `CombatLightingProfile` |
| Standalone MetalClash pulse intensity, duration, hold, range and offset | Existing `ImpactVfxProfile`; copied into each pulse at emission |
| Fixture placement, flame anchor, noise phase and animation offset | Serialized scene/prefab instance |
| Clock, current frame, pulse leases and private UV mesh | Runtime instance only |

Profiles have Create menu entries under **CLACKBLADE / Lighting**. Duplicate an
existing asset to make a variation. Materials, post profiles and meshes are shared:
duplicate those too when a variation should not affect other users of the asset.
There is no profile inheritance or implicit per-instance style override.

## Add a fixture or flame animation

1. Place a reusable hanging-lantern or wall-torch prefab from `Assets/Data/Lighting`.
   Move its transform and assign a `LightProfile`. Positions do not depend on object
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

The canonical UI builder still partitions the existing menu/gameplay/default-world
scenes. It preserves authored entrypoint selections and does not recreate other
maps. New maps use the generic bound lighting authoring pass. Root world geometry,
actor/gameplay placement and pixel-art presentation contracts remain scene/game
authoring responsibilities; this feature does not add procedural maps or a map UI.

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
