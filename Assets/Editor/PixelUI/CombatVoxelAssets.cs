using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

internal static class CombatVoxelAssets
{
    internal const string Root = "Assets/Art/VFX/CombatVoxel/";
    internal static readonly string[] Names = { "Normal", "Slayer", "Lethal", "Received", "ReceivedLethal" };
    internal static GameObject Load(string name) => AssetDatabase.LoadAssetAtPath<GameObject>(Root + name + ".prefab");
    internal static void Prepare()
    {
        Directory.CreateDirectory(Root); AssetDatabase.Refresh();
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "Fragments.mat");
        if (!material)
        {
            material = new Material(Shader.Find("TypingHero/ParryVoxel")); material.SetFloat("_Thermal", 1); material.SetFloat("_Emission", 0);
            AssetDatabase.CreateAsset(material, Root + "Fragments.mat");
        }
        for (int role = 0; role < Names.Length; role++)
        {
            if (Load(Names[role])) continue;
            var go = new GameObject("Voxel" + Names[role]); go.SetActive(false);
            try
            {
                go.AddComponent<PooledVFX>(); var burst = go.AddComponent<VoxelImpactBurst>();
                var so = new SerializedObject(burst); so.FindProperty("profile").intValue = role;
                so.FindProperty("fragments").objectReferenceValue = Group(go, "Front", material, 2);
                so.FindProperty("rear").objectReferenceValue = Group(go, "Back", material, -1); so.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(go, Root + Names[role] + ".prefab");
            }
            finally { Object.DestroyImmediate(go); }
        }
        PrepareNormalRays();
    }
    static void PrepareNormalRays()
    {
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(Root + "NormalRay.asset");
        if (!mesh)
        {
            mesh = new Mesh { name = "Normal pointed ray", vertices = new[] {
                new Vector3(-.5f, 0, 0), new Vector3(-.25f, .5f, 0),
                new Vector3(.5f, 0, 0), new Vector3(-.25f, -.5f, 0) },
                triangles = new[] { 0, 1, 2, 0, 2, 3 },
                uv = new[] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right } };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh, Root + "NormalRay.asset");
        }
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "NormalImpact.mat");
        if (!material)
        {
            material = new Material(Shader.Find("TypingHero/NormalImpact"));
            AssetDatabase.CreateAsset(material, Root + "NormalImpact.mat");
        }
        var shader = Shader.Find("TypingHero/NormalImpact");
        if (material.shader != shader) { material.shader = shader; EditorUtility.SetDirty(material); }
        string path = Root + "Normal.prefab";
        var root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            bool changed = false;
            foreach (var renderer in root.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                // PlayerAttackSorting raises the player to1, so its front trail is2.
                if (renderer.mesh == mesh && renderer.sharedMaterial == material && renderer.sortingOrder == 3) continue;
                renderer.mesh = mesh; renderer.sharedMaterial = material; renderer.sortingOrder = 3; changed = true;
            }
            if (changed) PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }
    static ParticleSystem Group(GameObject owner, string name, Material material, int order)
    {
        var child = new GameObject(name); child.transform.SetParent(owner.transform, false);
        var ps = child.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main; main.loop = false; main.playOnAwake = false; main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.useUnscaledTime = true; main.startSize3D = true; main.startRotation3D = true; main.maxParticles = 14; main.simulationSpeed = 0;
        var emission = ps.emission; emission.enabled = false; var shape = ps.shape; shape.enabled = false;
        var renderer = ps.GetComponent<ParticleSystemRenderer>(); renderer.renderMode = ParticleSystemRenderMode.Mesh;
        renderer.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(ParryClashAssets.Root + "CompactVoxel.asset");
        renderer.sharedMaterial = material; renderer.alignment = ParticleSystemRenderSpace.World; renderer.sortingOrder = order;
        renderer.SetActiveVertexStreams(new List<ParticleSystemVertexStream> { ParticleSystemVertexStream.Position,
            ParticleSystemVertexStream.Normal, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV,
            ParticleSystemVertexStream.UV2, ParticleSystemVertexStream.Custom1XYZW });
        return ps;
    }
}
