using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

// Does not generate or rewrite painted maps. Only build-reachable art counts.
public static class SpriteDepthAuthoring
{
    const string DepthRoot = "Assets/Art/Lighting/Depths/";
    const string MapName = "_DepthMap";
    [InitializeOnLoadMethod]
    static void Initialize() => EditorApplication.playModeStateChanged += BeforePlay;
    static void BeforePlay(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode) return;
        try { SynchronizeSelectedWorlds(); }
        catch (Exception error) { EditorApplication.isPlaying = false; Debug.LogException(error); }
    }
    [Serializable] sealed class KeyRecord
    {
        public string clip, sprite, source, binding;
        public long localId;
        public float time, duration;
        public bool loop;
    }
    [Serializable] sealed class FrameRecord
    {
        public string sprite, source;
        public long localId;
        public Rect rect;
        public float pixelsPerUnit;
    }
    [Serializable] sealed class Inventory { public int uniqueSprites; public KeyRecord[] keys; public FrameRecord[] frames; }
    public static void ExportInventory(string destination)
    {
        var sources = SpriteDepthSources.Collect();
        var keys = new List<KeyRecord>();
        foreach (var source in sources.curves)
        {
            var clip = source.clip;
            var binding = source.binding;
            var curve = AnimationUtility.GetObjectReferenceCurve(clip, binding);
            for (int i = 0; i < curve.Length; i++)
            {
                if (!(curve[i].value is Sprite sprite)) continue;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long id);
                keys.Add(new KeyRecord { clip = AssetDatabase.GetAssetPath(clip), sprite = sprite.name,
                    source = AssetDatabase.GetAssetPath(sprite), localId = id, binding = binding.path,
                    time = curve[i].time, duration = (i + 1 < curve.Length ? curve[i+1].time : clip.length) - curve[i].time,
                    loop = AnimationUtility.GetAnimationClipSettings(clip).loopTime });
            }
        }
        var frames = sources.sprites.Select(sprite => {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long id);
            return new FrameRecord { sprite = sprite.name, source = AssetDatabase.GetAssetPath(sprite),
                localId = id, rect = sprite.rect, pixelsPerUnit = sprite.pixelsPerUnit };
        }).ToArray();
        System.IO.File.WriteAllText(destination, JsonUtility.ToJson(new Inventory {
            uniqueSprites = frames.Length, keys = keys.ToArray(), frames = frames }, true));
    }
    public sealed class Coverage
    {
        public int mapped;
        public string[] missing;
        public override string ToString() => "Sprite depth: " + mapped + " mapped, " + missing.Length +
            " missing (renders without self-shadow)" + (missing.Length == 0 ? "" : "\n" + string.Join("\n", missing));
    }

    [MenuItem("CLACKBLADE/Lighting/Rebuild Selected Depth Library Metadata")]
    static void RebuildSelected()
    {
        if (!(Selection.activeObject is SpriteDepthLibrary library))
            throw new InvalidOperationException("Select a Sprite Depth Library asset");
        Debug.Log(Synchronize(library, ReferencedSprites()));
    }
    [MenuItem("CLACKBLADE/Lighting/Sync Gameplay Depth")]
    public static void SynchronizeSelectedWorlds()
    {
        var libraries = LightingBuildValidation.SelectedWorlds().Select(x => x.spriteDepth).Where(x => x).Distinct().ToArray();
        if (libraries.Length == 0) return;
        var sprites = ReferencedSprites();
        foreach (var library in libraries) Debug.Log(Synchronize(library, sprites), library);
    }
    [MenuItem("CLACKBLADE/Lighting/Audit Complete Gameplay Depth")]
    public static void AuditCoverage()
    {
        SynchronizeSelectedWorlds();
        foreach (var library in LightingBuildValidation.SelectedWorlds().Select(x => x.spriteDepth).Where(x => x).Distinct())
            Validate(library, true);
    }

    // The source texture binding owns the map. The library is a deterministic runtime index.
    // Resolve and validate the entire input before writing any binding or metadata.
    public static Coverage Synchronize(SpriteDepthLibrary library, IEnumerable<Sprite> referenced)
    {
        var sprites = referenced.Where(x => x).Distinct().ToArray();
        var maps = new Dictionary<string, Texture2D>();
        var bindings = new List<TextureImporter>();
        var missing = new List<string>();
        foreach (var group in sprites.GroupBy(AssetDatabase.GetAssetPath).OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            string path = group.Key;
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var secondary = importer.secondarySpriteTextures.Where(x => x.name == MapName).ToArray();
            if (secondary.Length > 1 || (secondary.Length == 1 && !secondary[0].texture))
                throw new BuildFailedException(path + ": duplicate or broken " + MapName + " binding");
            var depth = secondary.Length == 1 ? secondary[0].texture : null;
            var legacy = library.frames.Where(x => x.sprite && AssetDatabase.GetAssetPath(x.sprite) == path).ToArray();
            if (!depth && (legacy.Any(x => !x.depth) || legacy.Select(x => x.depth).Distinct().Count() > 1))
                throw new BuildFailedException(path + ": conflicting or broken depth library/binding; resolve the authored map explicitly");
            if (!depth)
            {
                // Preserve a pre-existing custom map before considering the conventional path.
                string authored = DepthRoot + AssetDatabase.AssetPathToGUID(path) + ".png";
                depth = legacy.Select(x => x.depth).FirstOrDefault() ?? AssetDatabase.LoadAssetAtPath<Texture2D>(authored);
                if (!depth && System.IO.File.Exists(authored))
                    throw new BuildFailedException(authored + ": authored depth exists but cannot be imported as a texture");
                if (depth) bindings.Add(importer);
            }
            if (depth) foreach (var sprite in group) Describe(sprite, depth);
            else missing.AddRange(group.Select(x => path + "/" + x.name));
            maps.Add(path, depth);
        }
        var keys = new HashSet<string>(sprites.Select(SpriteKey));
        foreach (var importer in bindings)
        {
            var textures = importer.secondarySpriteTextures.ToList();
            textures.Add(new SecondarySpriteTexture { name = MapName, texture = maps[importer.assetPath] });
            importer.secondarySpriteTextures = textures.ToArray(); importer.SaveAndReimport();
        }
        // Reacquire sprites after import; use stable asset GUID/local ID rather than names.
        var frames = maps.Where(x => x.Value).SelectMany(pair => AssetDatabase.LoadAllAssetsAtPath(pair.Key)
            .OfType<Sprite>().Where(x => keys.Contains(SpriteKey(x))).Select(x => Describe(x, pair.Value)))
            .OrderBy(x => SpriteKey(x.sprite), StringComparer.Ordinal).ToArray();
        if (!library.frames.SequenceEqual(frames))
        {
            Undo.RecordObject(library, "Sync depth frame metadata");
            library.frames = frames; EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssetIfDirty(library);
        }
        return new Coverage { mapped = frames.Length, missing = missing.OrderBy(x => x, StringComparer.Ordinal).ToArray() };
    }
    static string SpriteKey(Sprite sprite)
    {
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long id);
        return guid + ":" + id;
    }
    public static Sprite[] ReferencedSprites() => SpriteDepthSources.Collect().sprites.ToArray();
    public static SpriteDepthLibrary.Frame Describe(Sprite sprite, Texture2D depth)
    {
        var source = sprite.texture;
        var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(depth));
        string label = AssetDatabase.GetAssetPath(sprite) + "/" + sprite.name + ": depth " + AssetDatabase.GetAssetPath(depth);
        if (sprite.packed || PackedSource(sprite))
            throw new BuildFailedException(label + " does not support sprite/atlas packing");
        if (source.width != depth.width || source.height != depth.height || importer.sRGBTexture ||
            importer.mipmapEnabled || importer.filterMode != FilterMode.Point ||
            importer.textureCompression != TextureImporterCompression.Uncompressed ||
            importer.textureType != TextureImporterType.Default)
            throw new BuildFailedException(label + " requires aligned linear Default/Point/no-mips/uncompressed data; got " +
                source.width + "x" + source.height + " vs " + depth.width + "x" + depth.height +
                ", type=" + importer.textureType + ", sRGB=" + importer.sRGBTexture + ", mips=" + importer.mipmapEnabled +
                ", filter=" + importer.filterMode + ", compression=" + importer.textureCompression);
        var rect = sprite.rect;
        return new SpriteDepthLibrary.Frame { sprite = sprite, depth = depth,
            uvRect = new Vector4(rect.xMin / source.width, rect.yMin / source.height,
                rect.xMax / source.width, rect.yMax / source.height),
            textureSize = new Vector2(source.width, source.height), pixelsPerUnit = sprite.pixelsPerUnit };
    }
    public static void Validate(SpriteDepthLibrary library, bool requireComplete = false)
        => Validate(library, ReferencedSprites(), requireComplete);
    public static void Validate(SpriteDepthLibrary library, IEnumerable<Sprite> sprites, bool requireComplete = false)
    {
        if (!library) return; // Production opt-in only after full authoring acceptance.
        var frames = library.CreateLookup();
        foreach (var sprite in sprites)
        {
            if (!frames.TryGetValue(sprite, out var frame))
            {
                if (requireComplete) throw new BuildFailedException("Missing authored depth: " + AssetDatabase.GetAssetPath(sprite) + "/" + sprite.name);
                continue;
            }
            var expected = Describe(sprite, frame.depth);
            var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite));
            if (!importer.secondarySpriteTextures.Any(x => x.name == "_DepthMap" && x.texture == frame.depth) ||
                frame.uvRect != expected.uvRect || frame.textureSize != expected.textureSize ||
                frame.pixelsPerUnit != expected.pixelsPerUnit)
                throw new BuildFailedException(sprite.name + ": stale depth metadata or secondary binding");
        }
    }
    static bool PackedSource(Sprite sprite)
    {
        string path = AssetDatabase.GetAssetPath(sprite);
        if (!string.IsNullOrEmpty(((TextureImporter)AssetImporter.GetAtPath(path)).spritePackingTag)) return true;
        return AssetDatabase.FindAssets("t:SpriteAtlas").Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<SpriteAtlas>).Where(x => x && x.IsIncludeInBuild())
            .SelectMany(x => x.GetPackables()).Any(x => x == sprite || AssetDatabase.GetAssetPath(x) == path ||
                (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(x)) &&
                 path.StartsWith(AssetDatabase.GetAssetPath(x) + "/", StringComparison.Ordinal)));
    }
    internal static void Attach(GameObject actor)
    {
        var renderer = actor.GetComponent<SpriteRenderer>();
        if (renderer && !renderer.GetComponent<SpriteDepthBinding>()) actor.AddComponent<SpriteDepthBinding>();
    }
}
