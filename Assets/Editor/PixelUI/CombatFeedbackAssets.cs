using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using UnityEngine;

internal static class CombatFeedbackAssets
{
    internal const string Root = "Assets/Art/VFX/Pimen/";
    internal const string Generated = Root + "Generated/";

    internal static GameObject Load(string role) => AssetDatabase.LoadAssetAtPath<GameObject>(Generated + role + ".prefab");

    internal static void Prepare()
    {
        Directory.CreateDirectory(Generated);
        AssetDatabase.Refresh();
        var small = Normalize("SmallHit", 532, 528, 0);
        var impact = Normalize("Impact", 291, 301, 0);
        var big = Normalize("BigHit", 557, 553, 0);
        Effect("Normal", small.frames, .15f, 64, small.extent);
        Effect("Parry", impact.frames, .18f, 72, impact.extent);
        Effect("Lethal", big.frames, .18f, 96, big.extent);
        for (int i = 0; i < 4; i++)
        {
            var variant = Normalize("SmallHit", 532, 528, i, "Slayer" + (i + 1));
            Effect("Slayer" + (i + 1), variant.frames, .1f, 56, variant.extent);
        }
        AssetDatabase.SaveAssets();
    }

    // Source sheets have six columns, chronological rows from the top.
    // One transform for the entire animation keeps the initial flash at contact.
    static (Sprite[] frames, int extent) Normalize(string source, int width, int height, int turns, string name = null)
    {
        var image = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        image.LoadImage(File.ReadAllBytes("Assets/Art/VFX/CodeManu/" + source + ".png"));
        var input = image.GetPixels32();
        var bounds = new RectInt[30];
        for (int f = 0; f < 30; f++)
        {
            int minX = width, minY = height, maxX = -1, maxY = -1;
            for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
                if (input[((4 - f / 6) * height + y) * image.width + f % 6 * width + x].a > 0)
                { minX = Mathf.Min(minX, x); minY = Mathf.Min(minY, y); maxX = Mathf.Max(maxX, x); maxY = Mathf.Max(maxY, y); }
            bounds[f] = maxX < 0 ? new RectInt() : new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }
        int first = System.Array.FindIndex(bounds, b => b.width > 0);
        int last = System.Array.FindLastIndex(bounds, b => b.width > 0);
        var center = bounds[first].center;
        var selected = Enumerable.Range(0, 8).Select(i => first + Mathf.RoundToInt((last - first) * i / 7f)).ToArray();
        float radius = selected.Max(f => Mathf.Max(Mathf.Abs(bounds[f].xMin - center.x), Mathf.Abs(bounds[f].xMax - center.x),
            Mathf.Abs(bounds[f].yMin - center.y), Mathf.Abs(bounds[f].yMax - center.y)));
        float ratio = 62f / radius;
        var output = new Color32[1024 * 128];
        int extent = 0;
        for (int i = 0; i < 8; i++)
        {
            int minX = 128, minY = 128, maxX = -1, maxY = -1;
            for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
            {
                var offset = new Vector2(x + .5f - 64, y + .5f - 64);
                for (int t = 0; t < turns; t++) offset = new Vector2(offset.y, -offset.x);
                int sx = Mathf.FloorToInt(center.x + offset.x / ratio), sy = Mathf.FloorToInt(center.y + offset.y / ratio);
                byte alpha = sx < 0 || sx >= width || sy < 0 || sy >= height ? (byte)0 :
                    input[((4 - selected[i] / 6) * height + sy) * image.width + selected[i] % 6 * width + sx].a;
                output[y * 1024 + i * 128 + x] = new Color32(255, 255, 255, alpha);
                if (alpha == 0) continue;
                minX = Mathf.Min(minX, x); minY = Mathf.Min(minY, y); maxX = Mathf.Max(maxX, x); maxY = Mathf.Max(maxY, y);
            }
            extent = Mathf.Max(extent, maxX - minX + 1, maxY - minY + 1);
        }
        Object.DestroyImmediate(image);
        var strip = new Texture2D(1024, 128, TextureFormat.RGBA32, false);
        strip.SetPixels32(output);
        var bytes = strip.EncodeToPNG(); Object.DestroyImmediate(strip);
        string path = "Assets/Art/VFX/CodeManu/" + (name ?? source) + "-White.png";
        if (!File.Exists(path) || !File.ReadAllBytes(path).SequenceEqual(bytes)) File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        return (Sheet(path, 128, 128, 0, 8), extent);
    }

    static Sprite[] Sheet(string path, int width, int height, int first, int count)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.textureShape = TextureImporterShape.Texture2D;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 100;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        var web = importer.GetPlatformTextureSettings("WebGL");
        web.overridden = true; web.format = TextureImporterFormat.RGBA32;
        importer.SetPlatformTextureSettings(web);
        importer.GetSourceTextureWidthAndHeight(out int sheetWidth, out int sheetHeight);
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaIsTransparency = false;
        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(textureSettings);
        importer.SaveAndReimport();
        var factory = new SpriteDataProviderFactories(); factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        var slices = new SpriteRect[count];
        for (int i = 0; i < count; i++)
        {
            int frame = first + i;
            slices[i] = new SpriteRect { name = "frame_" + i, alignment = SpriteAlignment.Center, pivot = Vector2.one * .5f,
                rect = new Rect(sheetWidth > width ? frame * width : 0,
                    sheetWidth > width ? 0 : sheetHeight - (frame + 1) * height, width, height) };
        }
        foreach (var slice in slices)
            slice.spriteID = previous.FirstOrDefault(s => s.name == slice.name)?.spriteID ?? GUID.Generate();
        provider.SetSpriteRects(slices);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(
            slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply();
        importer.SaveAndReimport();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();
        if (sprites.Length != count) throw new System.InvalidOperationException($"Expected {count} frames in {path}, got {sprites.Length}");
        return sprites;
    }

    static void Effect(string role, Sprite[] frames, float duration, float pixels, float maxExtent)
    {
        string path = Generated + role;
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + ".anim");
        if (!clip) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, path + ".anim"); }
        clip.frameRate = 1000;
        var keys = new ObjectReferenceKeyframe[frames.Length + 1];
        for (int i = 0; i < frames.Length; i++) keys[i] = new ObjectReferenceKeyframe { time = i == 0 ? 0 : .02f + (duration - .02f) * (i - 1) / (frames.Length - 1), value = frames[i] };
        keys[frames.Length] = new ObjectReferenceKeyframe { time = duration - .00001f, value = frames[frames.Length - 1] };
        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keys);
        AnimationUtility.SetAnimationEvents(clip, new[] { new AnimationEvent { time = duration, functionName = "AnimEvent_Release" } });
        var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false; settings.startTime = 0; settings.stopTime = duration;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path + ".controller");
        if (!controller)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(path + ".controller");
            controller.AddMotion(clip);
        }
        var go = new GameObject(role);
        try
        {
            // Preserve 42 screen pixels per world unit in the presentation camera.
            go.transform.localScale = Vector3.one * (pixels / 42f * 100f / maxExtent);
            var renderer = go.AddComponent<SpriteRenderer>(); renderer.sprite = frames[0]; renderer.sortingOrder = 30;
            renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            var animator = go.AddComponent<Animator>(); animator.runtimeAnimatorController = controller;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime; animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            go.AddComponent<PooledVFX>();
            PrefabUtility.SaveAsPrefabAsset(go, path + ".prefab");
        }
        finally { Object.DestroyImmediate(go); }
    }
}
