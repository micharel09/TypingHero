using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal static class SlayerHitVfxAssets
{
    internal const string Root = "Assets/Art/VFX/SlayerHits/";
    internal static GameObject Load(bool lethal) => AssetDatabase.LoadAssetAtPath<GameObject>(Root + (lethal ? "SlayerFinisher" : "SlayerHit") + ".prefab");
    internal static void Prepare()
    {
        // Reuse the exact contact-spark sequence used by NormalHitSpark.
        // The source importer and its stable sprite IDs belong to the existing asset.
        const string path = "Assets/Art/VFX/Pimen/HitSpark/Hit Effect 01/Hit Effect 01 1.png";
        var sourceFrames = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
        if (sourceFrames.Length != 7) throw new System.InvalidOperationException("Pimen contact spark requires seven source frames");
        var frames = sourceFrames.Skip(1).ToArray(); // White contact core through transparent tail.
        var texture = new Texture2D(2, 2); int widest = 0;
        try
        {
            texture.LoadImage(File.ReadAllBytes(path));
            foreach (var frame in frames)
            {
                var rect = frame.rect; int left = (int)rect.width, right = -1;
                for (int y = 0; y < rect.height; y++) for (int x = 0; x < rect.width; x++)
                    if (texture.GetPixel((int)rect.x + x, (int)rect.y + y).a > 0) { left = Mathf.Min(left, x); right = Mathf.Max(right, x); }
                widest = Mathf.Max(widest, right - left + 1);
            }
        }
        finally { Object.DestroyImmediate(texture); }
        Build(false, frames, 32f / 16f * frames[0].pixelsPerUnit / widest, .15f);
        Build(true, frames, 40f / 16f * frames[0].pixelsPerUnit / widest, .18f);
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
