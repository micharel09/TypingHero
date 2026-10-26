using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

internal static class SlayerHitVfxAssets
{
    internal const string Root = "Assets/Art/VFX/SlayerHits/";
    internal static GameObject Load(bool lethal) => AssetDatabase.LoadAssetAtPath<GameObject>(Root + (lethal ? "SlayerFinisher" : "SlayerHit") + ".prefab");
    internal static void Prepare()
    {
        const string path = Root + "metpxl-white.png";
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaIsTransparency = false;
        importer.maxTextureSize = 512;
        var web = importer.GetPlatformTextureSettings("WebGL");
        web.overridden = true; web.format = TextureImporterFormat.RGBA32; web.maxTextureSize = 512;
        importer.SetPlatformTextureSettings(web);
        var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect; importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
        var factory = new SpriteDataProviderFactories(); factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer); provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        var slices = new SpriteRect[6];
        for (int i = 0; i < slices.Length; i++)
            slices[i] = new SpriteRect { name = "frame_" + i, alignment = SpriteAlignment.Center, pivot = Vector2.one * .5f,
                rect = new Rect(i * 64, 0, 64, 64),
                spriteID = previous.FirstOrDefault(s => s.name == "frame_" + i)?.spriteID ?? GUID.Generate() };
        provider.SetSpriteRects(slices);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply(); importer.SaveAndReimport();
        var frames = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();
        if (frames.Length != 6) throw new System.InvalidOperationException("Slayer B requires all six source frames");
        var texture = new Texture2D(2, 2); int widest = 0; var centers = new Vector2[frames.Length];
        try
        {
            texture.LoadImage(File.ReadAllBytes(path));
            for (int i = 0; i < frames.Length; i++)
            {
                var frame = frames[i];
                Vector2 sum = Vector2.zero; float weight = 0;
                var rect = frame.rect; int left = 64, right = -1;
                for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
                {
                    float alpha = texture.GetPixel((int)rect.x + x, (int)rect.y + y).a;
                    if (alpha <= 0) continue;
                    left = Mathf.Min(left, x); right = Mathf.Max(right, x);
                    sum += new Vector2(x + .5f, y + .5f) * alpha; weight += alpha;
                }
                if (weight == 0) throw new System.InvalidOperationException("Empty Slayer B frame: " + frame.name);
                centers[i] = (sum / weight - frame.pivot) / frame.pixelsPerUnit;
                widest = Mathf.Max(widest, right - left + 1);
            }
        }
        finally { Object.DestroyImmediate(texture); }
        var sequence = new[] { frames[0], frames[1], frames[2], frames[3] };
        Build(false, sequence, 32f / widest, centers.Take(4).ToArray());
        Build(true, sequence, 40f / widest, centers.Take(4).ToArray());
    }
    static void Build(bool lethal, Sprite[] frames, float scale, Vector2[] centers)
    {
        var path = Root + (lethal ? "SlayerFinisher" : "SlayerHit") + ".prefab";
        bool existing = Load(lethal);
        var root = existing ? PrefabUtility.LoadPrefabContents(path) : new GameObject(lethal ? "SlayerFinisher" : "SlayerHit");
        try
        {
            root.SetActive(false);
            if (!root.GetComponent<PooledVFX>()) root.AddComponent<PooledVFX>();
            var oldImage = root.GetComponent<SpriteRenderer>();
            if (oldImage) Object.DestroyImmediate(oldImage);
            var child = root.transform.Find("Image");
            if (!child) { child = new GameObject("Image").transform; child.SetParent(root.transform, false); }
            var image = child.GetComponent<SpriteRenderer>(); if (!image) image = child.gameObject.AddComponent<SpriteRenderer>();
            image.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat");
            image.sortingOrder = 3; image.color = Color.white; image.sprite = frames[0];
            var effect = root.GetComponent<SlayerHitVfx>(); if (!effect) effect = root.AddComponent<SlayerHitVfx>();
            var so = new SerializedObject(effect); var list = so.FindProperty("frames"); list.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            var offsets = so.FindProperty("frameCenters"); offsets.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) offsets.GetArrayElementAtIndex(i).vector2Value = centers[i];
            so.FindProperty("image").objectReferenceValue = image;
            so.FindProperty("uniformScale").floatValue = scale;
            var durations = so.FindProperty("frameDurations"); durations.arraySize = frames.Length;
            var timing = lethal ? new[] { .06f, .04f, .03f, .03f } : new[] { .04f, .03f, .03f, .02f };
            for (int i = 0; i < frames.Length; i++) durations.GetArrayElementAtIndex(i).floatValue = timing[i];
            so.ApplyModifiedPropertiesWithoutUndo(); PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally { if (existing) PrefabUtility.UnloadPrefabContents(root); else Object.DestroyImmediate(root); }
    }
}
