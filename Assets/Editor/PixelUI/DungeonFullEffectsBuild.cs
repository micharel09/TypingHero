using UnityEngine;

// Retained editor entrypoint; all lighting/atmosphere materials are authored assets.
internal static class DungeonFullEffectsBuild
{
    internal static void Apply(GameObject room) => DungeonPixelFlameBuild.Apply(room);
}
