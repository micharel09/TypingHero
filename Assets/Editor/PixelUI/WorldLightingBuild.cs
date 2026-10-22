using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

internal static class WorldLightingBuild
{
    internal const string Root = "Assets/Art/Lighting/";
    internal static void Apply()
    {
        Directory.CreateDirectory(Root); AssetDatabase.Refresh();
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "WorldLit.mat");
        if (!material)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
            AssetDatabase.CreateAsset(material, Root + "WorldLit.mat");
        }
        var prepared = new System.Collections.Generic.HashSet<string>();
        foreach (string folder in new[] { "Assets/Characters/Player", "Assets/Characters/Enermy" })
            foreach (string guid in AssetDatabase.FindAssets("t:AnimationClip", new[] { folder }))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                    foreach (var key in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                        if (key.value is Sprite sprite) PrepareSurface(AssetDatabase.GetAssetPath(sprite.texture), prepared);
            }
        foreach (string path in new[] { "Assets/Scenes/SampleScene.unity", "Assets/Scenes/MainMenu.unity" })
        {
            var scene = EditorSceneManager.OpenScene(path);
            foreach (var oldLight in Object.FindObjectsOfType<Light2D>(true))
                if (!oldLight.GetComponentInParent<ClashLightingRig>()) oldLight.enabled = false;
            PrepareRig();
            var rig = Object.FindObjectOfType<ClashLightingRig>(true);
            if (!rig) PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "WorldLighting.prefab"), scene);
            if (scene.name == "MainMenu") MenuBackdrop(material, prepared);
            foreach (var renderer in Object.FindObjectsOfType<SpriteRenderer>(true))
            {
                // UI, cues, trails and flashes retain their existing presentation materials.
                bool actor = renderer.GetComponentInParent<PlayerHealth>() || renderer.GetComponentInParent<SkeletonController>();
                bool near = renderer.transform.parent && renderer.transform.parent.name == "ShogunEnvironment" && renderer.sortingOrder >= -65;
                if (!actor && !near) continue;
                if (!renderer.sprite || renderer.name == "ContactShadow") continue;
                PrepareSurface(AssetDatabase.GetAssetPath(renderer.sprite.texture), prepared);
                renderer.sharedMaterial = material;
                EditorUtility.SetDirty(renderer);
                if (PrefabUtility.IsPartOfPrefabInstance(renderer)) PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
            }
            foreach (var body in Object.FindObjectsOfType<PlayerHealth>(true)) ContactShadow(body.gameObject);
            foreach (var body in Object.FindObjectsOfType<SkeletonController>(true)) ContactShadow(body.gameObject);
            if (Camera.main)
            {
                Camera.main.allowHDR = true;
                Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            }
            EditorSceneManager.SaveScene(scene);
        }
    }
    static void MenuBackdrop(Material lit, System.Collections.Generic.HashSet<string> prepared)
    {
        var source = GameObject.Find("PixelUI/Reference960x540/MenuEnvironment");
        if (!source) return;
        var canvas = GameObject.Find("PixelUI").GetComponent<Image>(); if (canvas) canvas.enabled = false;
        var previous = Object.FindObjectOfType<WorldMenuBackdrop>(true);
        var root = previous ? previous.gameObject : new GameObject("WorldMenuBackdrop");
        var driver = root.GetComponent<WorldMenuBackdrop>();
        if (!driver) driver = root.AddComponent<WorldMenuBackdrop>();
        var images = source.GetComponentsInChildren<Image>(true);
        var so = new SerializedObject(driver);
        so.FindProperty("targetCamera").objectReferenceValue = Camera.main;
        var sources = so.FindProperty("sources"); sources.arraySize = images.Length;
        var layers = so.FindProperty("layers"); layers.arraySize = images.Length;
        for (int i = 0; i < images.Length; i++)
        {
            var image = images[i]; image.enabled = false;
            var child = root.transform.Find(image.name);
            if (!child) { child = new GameObject(image.name).transform; child.SetParent(root.transform, false); }
            var renderer = child.GetComponent<SpriteRenderer>();
        if (!renderer) renderer = child.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = image.sprite; renderer.color = image.color; renderer.sortingOrder = -100 + i;
            bool near = image.name == "House" || image.name == "Ground" || image.name == "Trees";
            renderer.sharedMaterial = near ? lit : AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            if (near && image.sprite) PrepareSurface(AssetDatabase.GetAssetPath(image.sprite.texture), prepared);
            sources.GetArrayElementAtIndex(i).objectReferenceValue = image.rectTransform;
            layers.GetArrayElementAtIndex(i).objectReferenceValue = renderer;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }
    static void ContactShadow(GameObject actor)
    {
        var body = actor.GetComponentInChildren<BoxCollider2D>(); if (!body) return;
        var child = actor.transform.Find("ContactShadow");
        if (!child) { child = new GameObject("ContactShadow").transform; child.SetParent(actor.transform, false); }
        string path = Root + "ContactShadow.png";
        if (!File.Exists(path))
        {
            var texture = new Texture2D(64, 16, TextureFormat.RGBA32, false);
            for (int y = 0; y < 16; y++) for (int x = 0; x < 64; x++)
            {
                float radius = new Vector2((x-31.5f)/32, (y-7.5f)/8).sqrMagnitude;
                texture.SetPixel(x,y,new Color(0,0,0,Mathf.Pow(Mathf.Clamp01(1-radius),2)*.42f));
            }
            texture.Apply(); File.WriteAllBytes(path, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spritePixelsPerUnit = 32;
            importer.mipmapEnabled = false; importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        Vector3 foot = body.transform.TransformPoint(body.offset) - Vector3.up * body.size.y * Mathf.Abs(body.transform.lossyScale.y) * .5f;
        child.position = foot + Vector3.up * .04f;
        child.localScale = new Vector3(.6f,.45f,1);
        var renderer = child.GetComponent<SpriteRenderer>();
        if (!renderer) renderer = child.gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path); renderer.sortingOrder = -2;
        renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
    }
    static void PrepareSurface(string path, System.Collections.Generic.HashSet<string> prepared)
    {
        if (path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) && prepared.Add(path)) SurfaceNormalAssets.Prepare(path);
    }
    static void PrepareRig()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(Root + "WorldLighting.prefab")) return;
        var root = new GameObject("WorldLighting");
        try
        {
            var rig = root.AddComponent<ClashLightingRig>();
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(Root + "WorldBloom.asset");
            if (!profile)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, Root + "WorldBloom.asset");
                var bloom = profile.Add<Bloom>(); bloom.threshold.Override(1.25f);
                bloom.intensity.Override(.18f); bloom.scatter.Override(.35f);
                AssetDatabase.AddObjectToAsset(bloom, profile);
            }
            var volume = root.AddComponent<Volume>(); volume.isGlobal = true; volume.sharedProfile = profile;
            Light(root, "Ambient", Light2D.LightType.Global, new Color(.68f,.75f,.88f), .65f, Vector3.zero, 0);
            Light(root, "Key", Light2D.LightType.Point, new Color(1,.86f,.66f), .8f, new Vector3(-3,5,0), 18);
            Light(root, "Fill", Light2D.LightType.Point, new Color(.4f,.65f,1), .35f, new Vector3(5,2,0), 12);
            var pulse = Light(root, "Clash", Light2D.LightType.Point, new Color(1,.65f,.2f), 0, Vector3.zero, 4);
            var so = new SerializedObject(rig); so.FindProperty("clashLight").objectReferenceValue = pulse; so.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, Root + "WorldLighting.prefab");
        }
        finally { Object.DestroyImmediate(root); }
    }
    static Light2D Light(GameObject root, string name, Light2D.LightType type, Color color, float intensity, Vector3 position, float radius)
    {
        var go = new GameObject(name); go.transform.SetParent(root.transform, false); go.transform.localPosition = position;
        var light = go.AddComponent<Light2D>(); light.lightType = type; light.color = color; light.intensity = intensity;
        if (type != Light2D.LightType.Global)
        {
            light.pointLightOuterRadius = radius; light.pointLightInnerRadius = radius * .12f;
            var so = new SerializedObject(light);
            so.FindProperty("m_NormalMapQuality").intValue = (int)Light2D.NormalMapQuality.Accurate;
            so.FindProperty("m_NormalMapDistance").floatValue = 2;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        return light;
    }
}
