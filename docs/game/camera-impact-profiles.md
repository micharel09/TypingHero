# Camera impact profiles

1. Duplicate `Assets/Data/Lighting/CameraImpact.asset` in the Unity Project window.
2. Edit its six presets in the Inspector. `zoomPeak` is a fraction (`0.02` = 2%);
   all times are seconds. Attack curves run 0?1; release curves run 1?0.
   Shake envelopes use normalized time 0?1 and end at zero.
3. Select `WorldPixels` for logical pixels in the active world render texture,
   or `WorldUnits` for absolute world displacement. Contact directions are projected
   into the camera plane; fixed directions are camera-space X/Y vectors. Strength multiplies
   the envelope, which may contain signed values. Slayer entry retains its existing
   base lens tween; its impact preset controls entry displacement/priority only.
4. Assign the duplicate to `WorldDefinition.cameraImpact`. Every world must supply
   all six distinct events, including disabled presets. Click **Validate** on the
   profile and WorldDefinition to check completeness, times and curves.
5. Reload the scene/session. Binding prepares private curve copies for each
   controller. Edits do not hot-reload during an active impact. Builders validate
   and connect references without replacing authored preset values.

Only the highest-priority accepted impact owns the camera. Equal/weaker events are
ignored and never renew its time. `minimumPriorityTime` keeps ownership for at least
that many unpaused seconds; higher priority can still replace it. Received damage
uses zero zoom. Nonlethal Slayer contacts do not request a transient impact.

Zoom uses event timestamps on the unscaled clock excluding manual Pause; shake uses
the gameplay clock and its configured hitstop gate/delay. A skipped attack/hold still
renders the exact peak once, acknowledged only by the bound world camera. Release
starts no earlier than that frame's actual render completion, so a slow render cannot
consume recovery before the peak is shown. A later hitch cannot replay an acknowledged peak. Reading,
results, restart, menu, unbind and disable discard pending requests and displacement;
old render acknowledgments cannot confirm a replacement effect.

The default Slayer entry migrates the old CM2.9.7 Rumble curve (200ms), source
velocity magnitude 0.1 and listener gain 10 into a signed envelope of strength 1 world unit with
30ms delay. Other combat recoil preserves its100ms curve. Combat now applies the
sample at Cinemachine Noise through `CameraImpactExtension`; it emits no legacy
impulses. Other Cinemachine sources/listeners remain separate.
