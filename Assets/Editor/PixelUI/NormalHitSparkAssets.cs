using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal static class NormalHitSparkAssets
{
    const string Source = "Assets/Art/VFX/Pimen/HitSpark/Hit Effect 01/Hit Effect 01 1.png";
    const string Path = CombatVoxelAssets.Root + "NormalHitSpark.prefab";
    internal static GameObject Load() => AssetDatabase.LoadAssetAtPath<GameObject>(Path);
    internal static void Prepare()
    {
        var frames = AssetDatabase.LoadAllAssetsAtPath(Source).OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
        if (frames.Length != 7) throw new System.InvalidOperationException("Pimen normal hit requires seven source frames");
        // Read a temporary copy; never change the source texture/importer or recenter individual cels.
        var texture = new Texture2D(2, 2);
        int widest = 0;
        try
        {
            texture.LoadImage(File.ReadAllBytes(Source));
            foreach (var frame in frames)
            {
                var rect = frame.rect; int left = (int)rect.width, right = -1;
                for (int y = 0; y < rect.height; y++) for (int x = 0; x < rect.width; x++)
                    if (texture.GetPixel((int)rect.x + x, (int)rect.y + y).a > 0) { left = Mathf.Min(left, x); right = Mathf.Max(right, x); }
                widest = Mathf.Max(widest, right - left + 1);
            }
        }
        finally { Object.DestroyImmediate(texture); }
        frames = frames.Skip(1).ToArray(); // Start at the white core; retain the transparent tail and total160ms.
        var root = Load() ? PrefabUtility.LoadPrefabContents(Path) : new GameObject("NormalHitSpark");
        bool existing = Load();
        try
        {
            root.SetActive(false);
            if (!root.GetComponent<PooledVFX>()) root.AddComponent<PooledVFX>();
            var image = root.GetComponent<SpriteRenderer>();
            if (!image) image = root.AddComponent<SpriteRenderer>();
            image.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat");
            image.sortingOrder = 3; image.color = Color.white; image.sprite = frames[0];
            var burst = root.GetComponent<NormalHitSpark>();
            if (!burst) burst = root.AddComponent<NormalHitSpark>();
            var so = new SerializedObject(burst);
            var list = so.FindProperty("frames"); list.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            so.FindProperty("image").objectReferenceValue = image;
            so.FindProperty("uniformScale").floatValue = 48f / 16f * frames[0].pixelsPerUnit / widest;
            so.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, Path);
        }
        finally { if (existing) PrefabUtility.UnloadPrefabContents(root); else Object.DestroyImmediate(root); }
    }
}
