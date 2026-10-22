using UnityEngine;

// Kept as the existing editor entrypoint; style/atlas data belongs to profiles.
internal static class DungeonPixelFlameBuild
{
    internal static void Apply(GameObject room)
    {
        var world = Object.FindObjectOfType<DungeonWorld>(true);
        if (!world || world.environment != room.transform) throw new System.InvalidOperationException("World binding missing");
        WorldLightingAuthoring.Apply(world);
    }
}
