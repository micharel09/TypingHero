using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// Does not generate or rewrite painted maps. Only build-reachable art counts.
public static class SpriteDepthAuthoring
{
    static string[] ActorDependencies()
    {
        // EndlessRun.Definition selects these by Resources path at runtime;
        // serialized scene dependencies alone miss every swapped opponent.
        var roles = Enum.GetValues(typeof(EndlessRole)).Cast<EndlessRole>()
            .Select(role => Resources.Load<EndlessOpponent>("Endless/" + role)).Where(x => x)
            .Select(AssetDatabase.GetAssetPath).ToArray();
        return LightingBuildValidation.Dependencies().Concat(AssetDatabase.GetDependencies(roles, true))
            .Distinct().ToArray();
    }
    [Serializable] sealed class KeyRecord
    {
        public string clip, sprite, source, binding;
        public long localId;
        public float time, duration;
        public bool loop;
    }
    [Serializable] sealed class Inventory { public int uniqueSprites; public KeyRecord[] keys; }
    public static void ExportInventory(string destination)
    {
        var keys = new List<KeyRecord>();
        foreach (var clip in ActorDependencies()
            .Select(AssetDatabase.LoadAssetAtPath<AnimationClip>).Where(x => x))
        foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
        {
            var curve = AnimationUtility.GetObjectReferenceCurve(clip, binding);
            for (int i = 0; i < curve.Length; i++)
            {
                if (!(curve[i].value is Sprite sprite) || !IsActor(sprite)) continue;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long id);
                keys.Add(new KeyRecord { clip = AssetDatabase.GetAssetPath(clip), sprite = sprite.name,
                    source = AssetDatabase.GetAssetPath(sprite), localId = id, binding = binding.path,
                    time = curve[i].time, duration = (i + 1 < curve.Length ? curve[i+1].time : clip.length) - curve[i].time,
                    loop = AnimationUtility.GetAnimationClipSettings(clip).loopTime });
            }
        }
        System.IO.File.WriteAllText(destination, JsonUtility.ToJson(new Inventory {
            uniqueSprites = ReferencedSprites().Length, keys = keys.ToArray() }, true));
    }
    // An explicit authoring operation. Routine builders only validate the result.
    [MenuItem("CLACKBLADE/Lighting/Rebuild Selected Depth Library Metadata")]
    static void RebuildSelected()
    {
        if (!(Selection.activeObject is SpriteDepthLibrary library))
            throw new InvalidOperationException("Select a Sprite Depth Library asset");
        var frames = new List<SpriteDepthLibrary.Frame>();
        foreach (var sprite in ReferencedSprites())
        {
            string path = AssetDatabase.GetAssetPath(sprite);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var textures = importer.secondarySpriteTextures.ToList();
            var depth = textures.FirstOrDefault(x => x.name == "_DepthMap").texture;
            if (!depth)
            {
                string authored = "Assets/Art/Lighting/Depths/" + AssetDatabase.AssetPathToGUID(path) + ".png";
                depth = AssetDatabase.LoadAssetAtPath<Texture2D>(authored);
                if (!depth) throw new InvalidOperationException("Author depth before binding: " + authored);
                Describe(sprite, depth); // Reject incorrect import before writing the binding.
                textures.RemoveAll(x => x.name == "_DepthMap");
                textures.Add(new SecondarySpriteTexture { name = "_DepthMap", texture = depth });
                importer.secondarySpriteTextures = textures.ToArray();
                importer.SaveAndReimport();
            }
            frames.Add(Describe(sprite, depth));
        }
        Undo.RecordObject(library, "Rebuild depth frame metadata");
        library.frames = frames.ToArray(); EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
    }
    internal static Sprite[] ReferencedSprites()
    {
        var dependencies = ActorDependencies();
        // Include directly referenced hurt/parry clips as well as controller clips.
        var clips = dependencies.Select(AssetDatabase.LoadAssetAtPath<AnimationClip>).Where(x => x);
        var initial = dependencies.Select(AssetDatabase.LoadAssetAtPath<EndlessOpponent>)
            .Where(x => x).Select(x => x.idle);
        return clips
            .SelectMany(clip => AnimationUtility.GetObjectReferenceCurveBindings(clip)
                .SelectMany(binding => AnimationUtility.GetObjectReferenceCurve(clip, binding)))
            .Select(x => x.value as Sprite).Concat(initial).Where(x => x && IsActor(x)).Distinct().ToArray();
    }
    static bool IsActor(Sprite sprite)
    {
        var path = AssetDatabase.GetAssetPath(sprite).ToLowerInvariant();
        return path.Contains("male_hero") || path.Contains("skeleton");
    }
    public static SpriteDepthLibrary.Frame Describe(Sprite sprite, Texture2D depth)
    {
        var source = sprite.texture;
        var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(depth));
        if (source.width != depth.width || source.height != depth.height || importer.sRGBTexture ||
            importer.mipmapEnabled || importer.filterMode != FilterMode.Point ||
            importer.textureCompression != TextureImporterCompression.Uncompressed ||
            importer.textureType != TextureImporterType.Default || sprite.packed)
            throw new BuildFailedException(sprite.name + ": depth requires aligned unpacked linear Point uncompressed data");
        var rect = sprite.rect;
        return new SpriteDepthLibrary.Frame { sprite = sprite, depth = depth,
            uvRect = new Vector4(rect.xMin / source.width, rect.yMin / source.height,
                rect.xMax / source.width, rect.yMax / source.height),
            textureSize = new Vector2(source.width, source.height), pixelsPerUnit = sprite.pixelsPerUnit };
    }
    internal static void Validate(SpriteDepthLibrary library)
    {
        if (!library) return; // Production opt-in only after full authoring acceptance.
        var frames = library.CreateLookup();
        foreach (var sprite in ReferencedSprites())
        {
            if (!frames.TryGetValue(sprite, out var frame))
                throw new BuildFailedException("Missing authored depth: " + AssetDatabase.GetAssetPath(sprite) + "/" + sprite.name);
            var expected = Describe(sprite, frame.depth);
            var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite));
            if (!importer.secondarySpriteTextures.Any(x => x.name == "_DepthMap" && x.texture == frame.depth) ||
                frame.uvRect != expected.uvRect || frame.textureSize != expected.textureSize ||
                frame.pixelsPerUnit != expected.pixelsPerUnit)
                throw new BuildFailedException(sprite.name + ": stale depth metadata or secondary binding");
        }
    }
    internal static void Attach(GameObject actor)
    {
        var renderer = actor.GetComponent<SpriteRenderer>();
        if (renderer && !renderer.GetComponent<SpriteDepthBinding>()) actor.AddComponent<SpriteDepthBinding>();
    }
}
