using System.Collections.Generic;
using UnityEngine;

internal static class LightingTestAssets
{
    static readonly List<Object> owned = new List<Object>();
    internal static WorldDefinition Definition => UnityEditor.AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
    internal static string WorldScene => System.IO.Path.GetFileNameWithoutExtension(Definition.scenePath);
    internal static LightProfile Light(float intensity, float amplitude)
    {
        var p = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<LightProfile>("Assets/Data/Lighting/MainLantern.asset"));
        owned.Add(p); p.intensity = intensity; p.amplitude = amplitude; p.flame = null;
        return p;
    }
    internal static FlameProfile Flame(Mesh mesh, Material material)
    {
        var p = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<FlameProfile>("Assets/Data/Lighting/WarmFlame.asset"));
        owned.Add(p); p.mesh = mesh; p.material = material;
        material.SetTexture("_BaseMap", Texture2D.whiteTexture); return p;
    }
    internal static void Cleanup() { foreach (var p in owned) if (p) Object.DestroyImmediate(p); owned.Clear(); }
}
