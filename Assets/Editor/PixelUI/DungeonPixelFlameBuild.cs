using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// The complete reference flipbook shares one bottom pivot across all three torches.
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
        material.shader = Shader.Find("CLACKBLADE/Pixel Flame");
        material.SetTexture("_BaseMap", texture);
        material.SetTexture("_EmissionMap", texture);
        material.SetColor("_BaseColor", Color.black);
        material.SetColor("_EmissionColor", Color.white);
        material.SetColor("_FlameRed", new Color32(135, 59, 43, 255));
        material.SetColor("_FlameDarkOrange", new Color32(182, 83, 44, 255));
        material.SetColor("_FlameOrange", new Color32(228, 139, 58, 255));
        material.SetColor("_FlameYellow", new Color32(244, 204, 121, 255));
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
        // Uniform 54x80 cells preserve all ten source poses and their common bottom pivot.
        float pixel = PixelHeight(node.position, 10) / 76f / node.lossyScale.y;
        string meshPath = Art + "PixelFireQuad.asset";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if (!mesh) { mesh = new Mesh { name = "Lantern pixel fire quad" }; AssetDatabase.CreateAsset(mesh, meshPath); }
        mesh.Clear();
        mesh.vertices = new[] { new Vector3(-27 * .8f, -2, 0) * pixel, new Vector3(27 * .8f, -2, 0) * pixel,
            new Vector3(-27 * .8f, 78, 0) * pixel, new Vector3(27 * .8f, 78, 0) * pixel };
        mesh.uv = new[] { new Vector2(0, .5f), new Vector2(.2f, .5f), new Vector2(0, 1), new Vector2(.2f, 1) };
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
        settings.FindProperty("flameTimeOffset").floatValue = 0;
        settings.FindProperty("flameFrames").intValue = 10;
        settings.FindProperty("flameColumns").intValue = 5;
        settings.FindProperty("flameFrameSeconds").floatValue = .1f;
        var flames = settings.FindProperty("flameRenderers"); flames.arraySize = 1;
        flames.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
        settings.ApplyModifiedPropertiesWithoutUndo();
        Rear(room, -1, .17f, pixel, mesh, material);
        Rear(room, 1, .33f, pixel, mesh, material);
    }

    static void Rear(GameObject room, int side, float offset, float frontPixel, Mesh quad, Material material)
    {
        var holder = room.transform.Find("Torch_" + side);
        var model = holder.Find("ModelMesh");
        var filter = model.GetComponent<MeshFilter>();
        var source = filter.sharedMesh;
        var uv = source.uv; var vertices = source.vertices; var indices = source.triangles;
        Bounds bounds = new Bounds(); bool found = false;
        for (int i = 0; i < indices.Length; i += 3)
        {
            var center = (uv[indices[i]] + uv[indices[i + 1]] + uv[indices[i + 2]]) / 3;
            if (center.x < .875f || center.x > 1 || center.y < .25f || center.y >= .5f) continue;
            for (int j = 0; j < 3; j++)
            {
                var point = model.TransformPoint(vertices[indices[i + j]]);
                if (!found) { bounds = new Bounds(point, Vector3.zero); found = true; }
                else bounds.Encapsulate(point);
            }
        }
        if (!found) throw new System.InvalidOperationException("Rear flame region missing: " + side);
        // This existing derived mesh already contains only wood/metal. Keep the original FBX intact.
        filter.sharedMesh = model.Find("TorchSolidShadow").GetComponent<MeshFilter>().sharedMesh;
        var node = holder.Find("PixelFlame");
        if (!node) { node = new GameObject("PixelFlame").transform; node.SetParent(holder, false); }
        node.position = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        node.localRotation = Quaternion.identity;
        node.localScale = Vector3.one;
        node.localScale = Vector3.one * (PixelHeight(node.position, 12) / 76f / node.lossyScale.y / frontPixel);
        var flame = node.GetComponent<MeshFilter>(); if (!flame) flame = node.gameObject.AddComponent<MeshFilter>();
        flame.sharedMesh = quad;
        var renderer = node.GetComponent<MeshRenderer>(); if (!renderer) renderer = node.gameObject.AddComponent<MeshRenderer>();
        renderer.enabled = true; renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
        renderer.lightProbeUsage = LightProbeUsage.Off; renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        var settings = new SerializedObject(room.transform.Find("TorchGlow_" + side).GetComponent<DungeonTorchLight>());
        settings.FindProperty("pixelFlame").objectReferenceValue = flame;
        settings.FindProperty("flameTimeOffset").floatValue = offset;
        settings.FindProperty("flameFrames").intValue = 10;
        settings.FindProperty("flameColumns").intValue = 5;
        settings.FindProperty("flameFrameSeconds").floatValue = .1f;
        var flames = settings.FindProperty("flameRenderers"); flames.arraySize = 1;
        flames.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
        settings.ApplyModifiedPropertiesWithoutUndo();
    }

    static float PixelHeight(Vector3 point, int pixels)
    {
        var camera = Camera.main;
        float depth = Vector3.Dot(point - camera.transform.position, camera.transform.forward);
        return 2 * depth * Mathf.Tan(camera.fieldOfView * .5f * Mathf.Deg2Rad) * pixels /
            (DungeonArenaCamera.WorldHeight * Vector3.Dot(Vector3.up, camera.transform.up));
    }
}
