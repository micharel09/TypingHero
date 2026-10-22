using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// NYKNCK's five authored cels share one bottom pivot. Only this lamp uses the atlas.
internal static class DungeonPixelFlameBuild
{
    const string Art = "Assets/Art/Environment/HangingLantern/";

    internal static void Apply(GameObject room)
    {
        var original = room.transform.Find("Torch_Front/ModelMesh").GetComponent<MeshRenderer>();
        var bounds = original.bounds;
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Art + "PixelFire.png");
        var importer = (TextureImporter)AssetImporter.GetAtPath(Art + "PixelFire.png");
        if (importer.filterMode != FilterMode.Point || importer.mipmapEnabled ||
            importer.textureCompression != TextureImporterCompression.Uncompressed || importer.wrapMode != TextureWrapMode.Clamp)
        {
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }
        string materialPath = Art + "PixelFire.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (!material)
        {
            material = new Material(original.sharedMaterial) { name = "Lantern pixel fire" };
            AssetDatabase.CreateAsset(material, materialPath);
        }
        material.SetTexture("_BaseMap", texture);
        material.SetTexture("_EmissionMap", texture);
        material.SetColor("_BaseColor", Color.black);
        material.SetColor("_EmissionColor", Color.white * 1.4f);
        material.SetFloat("_AlphaClip", 1);
        material.SetFloat("_AlphaToMask", 1);
        material.SetFloat("_Cutoff", .5f);
        material.SetFloat("_Cull", 0);
        material.doubleSidedGI = true;
        material.EnableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_EMISSION");
        material.SetOverrideTag("RenderType", "TransparentCutout");
        material.renderQueue = (int)RenderQueue.AlphaTest;
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        material.SetShaderPassEnabled("ShadowCaster", false);
        EditorUtility.SetDirty(material);

        var parent = original.transform.parent;
        var node = parent.Find("PixelFlame");
        if (!node) { node = new GameObject("PixelFlame").transform; node.SetParent(parent, false); }
        node.position = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        node.localRotation = Quaternion.identity;
        node.localScale = Vector3.one;
        // All cels fit the old flame envelope. Atlas cels are 48x34, with a one-pixel floor margin.
        float pixel = Mathf.Min(bounds.size.x / 30f, bounds.size.y / 32f);
        string meshPath = Art + "PixelFireQuad.asset";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if (!mesh) { mesh = new Mesh { name = "Lantern pixel fire quad" }; AssetDatabase.CreateAsset(mesh, meshPath); }
        mesh.Clear();
        mesh.vertices = new[] { new Vector3(-24, -1, 0) * pixel, new Vector3(24, -1, 0) * pixel,
            new Vector3(-24, 33, 0) * pixel, new Vector3(24, 33, 0) * pixel };
        mesh.uv = new[] { new Vector2(0, 0), new Vector2(.2f, 0), new Vector2(0, 1), new Vector2(.2f, 1) };
        mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
        mesh.RecalculateNormals(); mesh.RecalculateBounds(); EditorUtility.SetDirty(mesh);
        var filter = node.GetComponent<MeshFilter>(); if (!filter) filter = node.gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        var renderer = node.GetComponent<MeshRenderer>(); if (!renderer) renderer = node.gameObject.AddComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = material; renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false; renderer.lightProbeUsage = LightProbeUsage.Off;
        renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        original.enabled = false;
        var torch = room.transform.Find("TorchGlow_Front").GetComponent<DungeonTorchLight>();
        var settings = new SerializedObject(torch);
        settings.FindProperty("pixelFlame").objectReferenceValue = filter;
        settings.FindProperty("flickerAmplitude").floatValue = .06f;
        var flames = settings.FindProperty("flameRenderers"); flames.arraySize = 1;
        flames.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
        settings.ApplyModifiedPropertiesWithoutUndo();
    }
}
