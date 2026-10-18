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
        foreach (var path in Directory.GetFiles(Root, "*.png", SearchOption.AllDirectories))
        {
            var import = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            import.textureType = TextureImporterType.Sprite;
            import.filterMode = FilterMode.Point; import.mipmapEnabled = false;
            import.textureCompression = TextureImporterCompression.Uncompressed;
            var web = import.GetPlatformTextureSettings("WebGL");
            web.overridden = true; web.format = TextureImporterFormat.RGBA32; import.SetPlatformTextureSettings(web);
            import.SaveAndReimport();
        }
        var orange = Sheet(Root + "HitSpark/Hit Effect 01/Hit Effect 01 1.png", 48, 48, 0, 7);
        var blue = Sheet(Root + "HitSpark/Hit Effect 01/Hit Effect 01 2.png", 48, 48, 0, 7);
        Effect("Normal", orange, .15f, 48, 45);
        Effect("Parry", blue, .18f, 56, 45);
        Effect("Lethal", orange, .18f, 64, 45);
        for (int i = 1; i <= 4; i++)
        {
            string source = Root + $"BattleEffects/Effect {i} - Sprite Sheet.png";
            string white = Generated + $"Slayer{i}-White.png";
            // Preserve author alpha/shape and original sheets; only the Slayer palette changes.
            var image = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            image.LoadImage(File.ReadAllBytes(source));
            var pixels = image.GetPixels32();
            for (int p = 0; p < pixels.Length; p++) pixels[p] = new Color32(255, 255, 255, pixels[p].a);
            image.SetPixels32(pixels);
            var bytes = image.EncodeToPNG();
            Object.DestroyImmediate(image);
            if (!File.Exists(white) || !File.ReadAllBytes(white).SequenceEqual(bytes)) File.WriteAllBytes(white, bytes);
            AssetDatabase.ImportAsset(white);
            var frames = Sheet(white, i <= 2 ? 64 : 32, 32, i == 1 ? 1 : 0, 4);
            Effect("Slayer" + i, frames, .1f, 28, i <= 2 ? 32 : i == 3 ? 31 : 26);
        }
        AssetDatabase.SaveAssets();
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
        for (int i = 0; i < frames.Length; i++) keys[i] = new ObjectReferenceKeyframe { time = duration * i / frames.Length, value = frames[i] };
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
