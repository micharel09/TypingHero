using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Explicit authoring step; ordinary/repeated builds reuse the baked assets.
public static class DungeonLightingBake
{
    public static void Bake()
    {
        PixelUIBuild.Apply();
        var scene = EditorSceneManager.OpenScene(DungeonSessionBuild.WorldPath);
        var meshes = GameObject.Find("DungeonArena3D").GetComponentsInChildren<MeshFilter>();
        foreach (var guid in AssetDatabase.FindAssets("t:Model",new[] { DungeonArenaBuild.Art.TrimEnd('/') }))
        {
            var modelPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = (ModelImporter)AssetImporter.GetAtPath(modelPath);
            var scales = meshes.Where(mesh => AssetDatabase.GetAssetPath(mesh.sharedMesh) == modelPath)
                .Select(mesh => Mathf.Min(Mathf.Abs(mesh.transform.lossyScale.x), Mathf.Abs(mesh.transform.lossyScale.y), Mathf.Abs(mesh.transform.lossyScale.z))).ToArray();
            float minimumScale = scales.Length > 0 ? scales.Min() : 1;
            if (!importer.generateSecondaryUV || importer.secondaryUVMarginMethod != ModelImporterSecondaryUVMarginMethod.Calculate ||
                importer.secondaryUVMinLightmapResolution != 16 || !Mathf.Approximately(importer.secondaryUVMinObjectScale, minimumScale))
            {
                importer.generateSecondaryUV = true; importer.secondaryUVMarginMethod = ModelImporterSecondaryUVMarginMethod.Calculate;
                importer.secondaryUVMinLightmapResolution = 16; importer.secondaryUVMinObjectScale = minimumScale;
                importer.SaveAndReimport();
            }
            Debug.Log("Dungeon UV2 " + modelPath + " minimum object scale=" + minimumScale + " at16texels/unit");
        }
        const string path = DungeonArenaBuild.Art + "DungeonLighting.asset";
        var settings = AssetDatabase.LoadAssetAtPath<LightingSettings>(path);
        if (!settings) { settings = new LightingSettings(); AssetDatabase.CreateAsset(settings, path); }
        settings.bakedGI = true; settings.realtimeGI = false;
        settings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
        settings.mixedBakeMode = MixedLightingMode.IndirectOnly;
        settings.lightmapResolution = 16; settings.lightmapMaxSize = 1024;
        settings.directSampleCount = 32; settings.indirectSampleCount = 128; settings.environmentSampleCount = 64;
        settings.ao = true; settings.aoMaxDistance = 1; settings.aoExponentIndirect = .8f; settings.aoExponentDirect = .3f;
        Lightmapping.lightingSettings = settings; Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
        EditorUtility.SetDirty(settings); AssetDatabase.SaveAssets();
        if (!Lightmapping.Bake()) throw new System.InvalidOperationException("Dungeon GI bake failed");
        if (LightmapSettings.lightmaps.Length == 0 || !LightmapSettings.lightProbes || LightmapSettings.lightProbes.count == 0)
            throw new System.InvalidOperationException("Dungeon bake produced no lightmap or light probe data");
        var probe = GameObject.Find("DungeonArena3D/BakedReflection").GetComponent<ReflectionProbe>();
        const string reflectionPath = DungeonArenaBuild.Art + "DungeonReflection.exr";
        // Reflection cameras use the default renderer; the game's menu keeps slot0 (2D).
        var pipeline = new SerializedObject(UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline);
        var renderer = pipeline.FindProperty("m_DefaultRendererIndex"); int previous = renderer.intValue;
        try
        {
            renderer.intValue = 1; pipeline.ApplyModifiedPropertiesWithoutUndo();
            if (!Lightmapping.BakeReflectionProbe(probe, reflectionPath))
                throw new System.InvalidOperationException("Dungeon reflection bake failed");
        }
        finally { renderer.intValue = previous; pipeline.ApplyModifiedPropertiesWithoutUndo(); }
        AssetDatabase.ImportAsset(reflectionPath);
        probe.customBakedTexture = AssetDatabase.LoadAssetAtPath<Cubemap>(reflectionPath);
        if (!probe.customBakedTexture) throw new System.InvalidOperationException("Dungeon reflection must import as a Cubemap");
        EditorUtility.SetDirty(probe); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Debug.Log("Dungeon GI, contact AO, light probes and 64px reflection baked; lightmaps=" + LightmapSettings.lightmaps.Length);
    }
}
