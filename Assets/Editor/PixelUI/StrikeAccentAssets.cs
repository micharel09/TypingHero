using UnityEditor;
using UnityEngine;

internal static class StrikeAccentAssets
{
    internal static GameObject Load(bool dust) => AssetDatabase.LoadAssetAtPath<GameObject>(
        CombatVoxelAssets.Root + (dust ? "StrikeDust" : "StrikeCore") + ".prefab");
    internal static void Prepare()
    {
        string path = CombatVoxelAssets.Root + "StrikeAccent.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!material)
        {
            material = new Material(Shader.Find("TypingHero/PixelSpark"));
            AssetDatabase.CreateAsset(material, path);
        }
        foreach (bool dust in new[] { true, false })
        {
            if (Load(dust))
            {
                if (!dust)
                {
                    string corePath = CombatVoxelAssets.Root + "StrikeCore.prefab";
                    var core = PrefabUtility.LoadPrefabContents(corePath);
                    try
                    {
                        var renderer = core.GetComponent<ParticleSystemRenderer>();
                        if (renderer.sortingOrder != 4) { renderer.sortingOrder = 4; PrefabUtility.SaveAsPrefabAsset(core, corePath); }
                    }
                    finally { PrefabUtility.UnloadPrefabContents(core); }
                }
                continue;
            }
            var go = new GameObject(dust ? "StrikeDust" : "StrikeCore"); go.SetActive(false);
            try
            {
                go.AddComponent<PooledVFX>();
                var burst = go.AddComponent<StrikeAccentBurst>();
                var ps = go.AddComponent<ParticleSystem>();
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = ps.main; main.loop = false; main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.World; main.simulationSpeed = 0;
                main.startSize3D = true; main.maxParticles = dust ? 3 : 1;
                var emission = ps.emission; emission.enabled = false;
                var shape = ps.shape; shape.enabled = false;
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.sharedMaterial = material; renderer.sortingOrder = dust ? 0 : 4;
                var so = new SerializedObject(burst);
                so.FindProperty("dust").boolValue = dust;
                so.FindProperty("particles").objectReferenceValue = ps;
                so.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(go, CombatVoxelAssets.Root + go.name + ".prefab");
            }
            finally { Object.DestroyImmediate(go); }
        }
    }
}
