using UnityEngine;

// Retained editor entrypoint; all lighting/atmosphere materials are authored assets.
internal static class DungeonAtmosphereBuild
{
    internal static void Apply(GameObject room) => DungeonPixelFlameBuild.Apply(room);
}
