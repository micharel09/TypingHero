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
        const string path = Root + "pixel_art_sword_slash_sprites.png";
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
        importer.maxTextureSize = 256;
        var web = importer.GetPlatformTextureSettings("WebGL");
        web.overridden = true; web.format = TextureImporterFormat.RGBA32; web.maxTextureSize = 256;
        importer.SetPlatformTextureSettings(web);
        var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect; importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
        var factory = new SpriteDataProviderFactories(); factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer); provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        var slices = new SpriteRect[9];
        for (int i = 0; i < slices.Length; i++)
            slices[i] = new SpriteRect { name = "frame_" + i, alignment = SpriteAlignment.Center, pivot = Vector2.one * .5f,
                rect = new Rect(i % 3 * 64, 141 - (i / 3 + 1) * 47, 64, 47),
                spriteID = previous.FirstOrDefault(s => s.name == "frame_" + i)?.spriteID ?? GUID.Generate() };
        provider.SetSpriteRects(slices);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply(); importer.SaveAndReimport();
        var frames = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();
        if (frames.Length != 9) throw new System.InvalidOperationException("Slayer slash requires all nine source frames");
        var texture = new Texture2D(2, 2); int widest = 0;
        try
        {
            texture.LoadImage(File.ReadAllBytes(path));
            foreach (var frame in frames)
            {
                var rect = frame.rect; int left = 64, right = -1;
                for (int y = 0; y < 47; y++) for (int x = 0; x < 64; x++)
                    if (texture.GetPixel((int)rect.x + x, (int)rect.y + y).a > 0) { left = Mathf.Min(left, x); right = Mathf.Max(right, x); }
                widest = Mathf.Max(widest, right - left + 1);
            }
        }
        finally { Object.DestroyImmediate(texture); }
        Build(false, frames, 32f / widest, .15f);
        Build(true, frames, 40f / widest, .18f);
    }
    static void Build(bool lethal, Sprite[] frames, float scale, float duration)
    {
        var path = Root + (lethal ? "SlayerFinisher" : "SlayerHit") + ".prefab";
        bool existing = Load(lethal);
        var root = existing ? PrefabUtility.LoadPrefabContents(path) : new GameObject(lethal ? "SlayerFinisher" : "SlayerHit");
        try
        {
            root.SetActive(false);
            if (!root.GetComponent<PooledVFX>()) root.AddComponent<PooledVFX>();
            var image = root.GetComponent<SpriteRenderer>(); if (!image) image = root.AddComponent<SpriteRenderer>();
            image.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat");
            image.sortingOrder = 3; image.color = Color.white; image.sprite = frames[0];
            var effect = root.GetComponent<SlayerHitVfx>(); if (!effect) effect = root.AddComponent<SlayerHitVfx>();
            var so = new SerializedObject(effect); var list = so.FindProperty("frames"); list.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            so.FindProperty("image").objectReferenceValue = image;
            so.FindProperty("uniformScale").floatValue = scale;
            so.FindProperty("duration").floatValue = duration;
            so.ApplyModifiedPropertiesWithoutUndo(); PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally { if (existing) PrefabUtility.UnloadPrefabContents(root); else Object.DestroyImmediate(root); }
    }
}
