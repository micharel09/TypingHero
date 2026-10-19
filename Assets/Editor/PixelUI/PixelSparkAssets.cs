using System.IO;
using UnityEditor;
using UnityEngine;

internal static class PixelSparkAssets
{
    internal const string Root = "Assets/Art/VFX/PixelSparks/";
    internal static GameObject Load(string role) => AssetDatabase.LoadAssetAtPath<GameObject>(Root + role + ".prefab");

    internal static void Prepare()
    {
        Directory.CreateDirectory(Root);
        string texturePath = Root + "Pixel.png";
        if (!File.Exists(texturePath))
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white); texture.Apply();
            File.WriteAllBytes(texturePath, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
        }
        AssetDatabase.Refresh();
        var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
        importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        var web = importer.GetPlatformTextureSettings("WebGL");
        web.overridden = true; web.format = TextureImporterFormat.RGBA32; importer.SetPlatformTextureSettings(web);
        importer.SaveAndReimport();
        string materialPath = Root + "Pixel.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (!material) { material = new Material(Shader.Find("TypingHero/PixelSpark")); AssetDatabase.CreateAsset(material, materialPath); }
        material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        EditorUtility.SetDirty(material);
        Profile("Parry", PixelSparkBurst.Profile.Parry, 40, new Vector2(.6f, .85f), new Color32(109, 214, 255, 255), material);
        Profile("Normal", PixelSparkBurst.Profile.Normal, 24, new Vector2(.45f, .65f), new Color32(255, 164, 48, 255), material);
        Profile("Slayer", PixelSparkBurst.Profile.Slayer, 32, new Vector2(.35f, .55f), new Color32(255, 245, 208, 255), material);
        Profile("Lethal", PixelSparkBurst.Profile.Lethal, 56, new Vector2(.75f, .95f), new Color32(255, 164, 48, 255), material);
        AssetDatabase.SaveAssets();
    }

    static void Profile(string name, PixelSparkBurst.Profile role, int count, Vector2 life, Color color, Material material)
    {
        // Generated once; subsequent builds retain authored Inspector tuning and GUIDs.
        if (Load(name)) return;
        var root = new GameObject(name);
        try
        {
            var ground = new GameObject("Ground").transform; ground.SetParent(root.transform, false);
            var dots = Group(root.transform, "Dots", ground, life, color, material);
            var streaks = Group(root.transform, "Streaks", ground, life, color, material);
            root.AddComponent<PooledVFX>();
            var burst = root.AddComponent<PixelSparkBurst>();
            var so = new SerializedObject(burst);
            so.FindProperty("profile").enumValueIndex = (int)role;
            so.FindProperty("count").intValue = count;
            so.FindProperty("lifetime").vector2Value = life;
            so.FindProperty("horizontalSpeed").vector2Value = role == PixelSparkBurst.Profile.Slayer ? new Vector2(5, 7) :
                role == PixelSparkBurst.Profile.Lethal ? new Vector2(3, 7) : new Vector2(3, 6);
            so.FindProperty("verticalSpeed").vector2Value = role == PixelSparkBurst.Profile.Normal ? new Vector2(1, 3) : new Vector2(1, 4);
            so.FindProperty("dots").objectReferenceValue = dots;
            so.FindProperty("streaks").objectReferenceValue = streaks;
            so.FindProperty("ground").objectReferenceValue = ground;
            so.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, Root + name + ".prefab");
        }
        finally { Object.DestroyImmediate(root); }
    }

    static ParticleSystem Group(Transform root, string name, Transform ground, Vector2 life, Color color, Material material)
    {
        var go = new GameObject(name); go.transform.SetParent(root, false);
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.loop = false; main.playOnAwake = false; main.duration = life.y;
        main.startLifetime = new ParticleSystem.MinMaxCurve(life.x, life.y);
        main.startSpeed = 0; main.startSize3D = true; main.startColor = Color.white;
        main.simulationSpace = ParticleSystemSimulationSpace.World; main.useUnscaledTime = true;
        main.maxParticles = 56; main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
        var emission = ps.emission; emission.enabled = false;
        var shape = ps.shape; shape.enabled = false;
        var force = ps.forceOverLifetime; force.enabled = true; force.space = ParticleSystemSimulationSpace.World;
        force.y = -18f;
        var collision = ps.collision; collision.enabled = true;
        collision.type = ParticleSystemCollisionType.Planes; collision.SetPlane(0, ground);
        collision.mode = ParticleSystemCollisionMode.Collision3D;
        collision.bounce = .4f; collision.dampen = .4f; collision.lifetimeLoss = 0f;
        collision.minKillSpeed = .5f; collision.radiusScale = .5f;
        var overLife = ps.colorOverLifetime; overLife.enabled = true;
        var gradient = new Gradient { mode = GradientMode.Fixed };
        // Fixed gradients select the next key; times mark each color's end.
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, .15f), new GradientColorKey(color, .7f),
            new GradientColorKey(color * .45f, 1f) }, new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
        overLife.color = gradient;
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = material; renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.View; renderer.sortingOrder = 30;
        renderer.maxParticleSize = 1; renderer.minParticleSize = 0;
        return ps;
    }
}
