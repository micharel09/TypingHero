using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;

public sealed class SpriteDepthSyncTests
{
    const string Root = "Assets/__DepthSyncTests";
    string source, conventional;
    SpriteDepthLibrary library;
    Sprite[] Sprites => AssetDatabase.LoadAllAssetsAtPath(source).OfType<Sprite>().OrderBy(x => x.name).ToArray();
    [SetUp]
    public void SetUp()
    {
        Assert.That(AssetDatabase.IsValidFolder(Root), Is.False, "Never overwrite an existing fixture folder");
        AssetDatabase.CreateFolder("Assets", "__DepthSyncTests");
        source = Root + "/actor.png";
        WriteTexture(source, 16, 8);
        var importer = (TextureImporter)AssetImporter.GetAtPath(source);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.spritesheet = new[] {
            new SpriteMetaData { name = "left", rect = new Rect(0,0,8,8), pivot = Vector2.one * .5f },
            new SpriteMetaData { name = "right", rect = new Rect(8,0,8,8), pivot = Vector2.one * .5f }
        };
        importer.SaveAndReimport();
        conventional = "Assets/Art/Lighting/Depths/" + AssetDatabase.AssetPathToGUID(source) + ".png";
        Assert.That(File.Exists(conventional), Is.False);
        library = ScriptableObject.CreateInstance<SpriteDepthLibrary>();
        AssetDatabase.CreateAsset(library, Root + "/library.asset");
    }
    [TearDown]
    public void TearDown()
    {
        if (!string.IsNullOrEmpty(conventional)) AssetDatabase.DeleteAsset(conventional);
        AssetDatabase.DeleteAsset(Root);
    }
    static Texture2D WriteTexture(string path, int width, int height)
    {
        var texture = new Texture2D(width, height);
        File.WriteAllBytes(path, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = false; importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    void Bind(Texture2D depth)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(source);
        importer.secondarySpriteTextures = new[] { new SecondarySpriteTexture { name = "_DepthMap", texture = depth } };
        importer.SaveAndReimport();
    }
    [Test]
    public void MissingThenConventionalMapUpdatesAllFramesAndSurvivesMoveWithoutRewriting()
    {
        var unrelated = ScriptableObject.CreateInstance<SpriteDepthLibrary>();
        string unrelatedPath = Root + "/unrelated.asset";
        AssetDatabase.CreateAsset(unrelated, unrelatedPath);
        byte[] unrelatedBefore = File.ReadAllBytes(unrelatedPath);
        unrelated.name = "unsaved author edit"; EditorUtility.SetDirty(unrelated);
        var report = SpriteDepthAuthoring.Synchronize(library, Sprites);
        Assert.That(File.ReadAllBytes(unrelatedPath), Is.EqualTo(unrelatedBefore));
        Assert.That(EditorUtility.IsDirty(unrelated), Is.True, "Sync must not save unrelated author edits");
        Assert.That(report.missing.Length, Is.EqualTo(2));
        Assert.That(library.frames, Is.Empty);
        Assert.DoesNotThrow(() => SpriteDepthAuthoring.Validate(library, Sprites));
        Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Validate(library, Sprites, true));
        WriteTexture(conventional, 16, 8);
        byte[] painted = File.ReadAllBytes(conventional);
        var importer = (TextureImporter)AssetImporter.GetAtPath(source);
        importer.secondarySpriteTextures = new[] { new SecondarySpriteTexture {
            name = "_NormalMap", texture = AssetDatabase.LoadAssetAtPath<Texture2D>(conventional) } };
        importer.SaveAndReimport();
        report = SpriteDepthAuthoring.Synchronize(library, Sprites);
        Assert.That(report.missing, Is.Empty);
        Assert.That(report.mapped, Is.EqualTo(2));
        var frames = library.frames.OrderBy(x => x.sprite.name).ToArray();
        Assert.That(frames[0].uvRect, Is.EqualTo(new Vector4(0,0,.5f,1)));
        Assert.That(frames[1].uvRect, Is.EqualTo(new Vector4(.5f,0,1,1)));
        Assert.That(frames.All(x => x.textureSize == new Vector2(16,8) && x.pixelsPerUnit == 16), Is.True);
        string metadata = File.ReadAllText(Root + "/library.asset");
        byte[] binding = File.ReadAllBytes(source + ".meta");
        SpriteDepthAuthoring.Synchronize(library, Sprites.Reverse());
        Assert.That(File.ReadAllText(Root + "/library.asset"), Is.EqualTo(metadata));
        Assert.That(File.ReadAllBytes(source + ".meta"), Is.EqualTo(binding));
        Assert.That(((TextureImporter)AssetImporter.GetAtPath(source)).secondarySpriteTextures.Length, Is.EqualTo(2));
        string guid = AssetDatabase.AssetPathToGUID(source);
        string moved = Root + "/renamed.png";
        Assert.That(AssetDatabase.MoveAsset(source, moved), Is.Empty); source = moved;
        Assert.That(AssetDatabase.AssetPathToGUID(source), Is.EqualTo(guid));
        SpriteDepthAuthoring.Synchronize(library, Sprites);
        SpriteDepthAuthoring.Validate(library, Sprites, true);
        Assert.That(File.ReadAllBytes(conventional), Is.EqualTo(painted));
        SpriteDepthAuthoring.Synchronize(library, Sprites.Take(1));
        Assert.That(library.frames.Length, Is.EqualTo(1));
        Assert.That(File.Exists(conventional), Is.True, "Pruning the index must not delete authored art");
    }
    [Test]
    public void CustomBindingWinsOverConventionalAndConflictingLegacyMapIsRejected()
    {
        var custom = WriteTexture(Root + "/custom.png",16,8);
        WriteTexture(conventional,16,8); Bind(custom);
        SpriteDepthAuthoring.Synchronize(library, Sprites);
        Assert.That(library.frames.All(x => x.depth == custom), Is.True);
        Bind(AssetDatabase.LoadAssetAtPath<Texture2D>(conventional));
        Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
        Assert.That(library.frames.All(x => x.depth == custom), Is.True);
    }
    [Test]
    public void LegacyCustomBindingMigratesAndInvalidMapsCannotSilentlyFallBack()
    {
        var custom = WriteTexture(Root + "/custom.png",16,8);
        library.frames = Sprites.Select(x => SpriteDepthAuthoring.Describe(x,custom)).ToArray();
        SpriteDepthAuthoring.Synchronize(library, Sprites);
        Assert.That(((TextureImporter)AssetImporter.GetAtPath(source)).secondarySpriteTextures.Single().texture, Is.EqualTo(custom));
        var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "/custom.png");
        importer.sRGBTexture = true; importer.SaveAndReimport();
        Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
        importer.sRGBTexture = false; importer.SaveAndReimport();
        var wrong = WriteTexture(Root + "/wrong.png",8,8);
        library.frames = Array.Empty<SpriteDepthLibrary.Frame>(); Bind(wrong);
        Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
        Bind(null);
        Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
    }
    [Test]
    public void AuthoredMapRejectsBuildAtlasPackingBeforeSpriteIsPacked()
    {
        WriteTexture(conventional,16,8);
        var atlas = new SpriteAtlas();
        atlas.Add(new UnityEngine.Object[] { AssetDatabase.LoadAssetAtPath<Texture2D>(source) });
        atlas.SetIncludeInBuild(true); AssetDatabase.CreateAsset(atlas,Root + "/test.spriteatlas");
        var error = Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
        Assert.That(error.Message, Does.Contain("packing"));
        Assert.That(library.frames, Is.Empty);
        Assert.That(((TextureImporter)AssetImporter.GetAtPath(source)).secondarySpriteTextures, Is.Empty);
    }
    [Test]
    public void InventoryIncludesReachablePlayerRecoveryAndAuthoredContactSprites()
    {
        var sprites = SpriteDepthAuthoring.ReferencedSprites();
        var playerClips = AssetDatabase.GetDependencies("Assets/Characters/Player/Player.prefab",true)
            .Select(AssetDatabase.LoadAssetAtPath<AnimationClip>).Where(x => x);
        var frames = playerClips.SelectMany(clip => AnimationUtility.GetObjectReferenceCurveBindings(clip)
            .SelectMany(binding => AnimationUtility.GetObjectReferenceCurve(clip,binding)))
            .Select(x => x.value as Sprite).Where(x => x).Distinct().ToArray();
        Assert.That(frames, Is.Not.Empty);
        Assert.That(frames.All(sprites.Contains), Is.True, "Inventory must cover actor dependencies regardless of folder names");
        Assert.That(sprites.Any(x => AssetDatabase.GetAssetPath(x).Contains("PixelWeaponContacts")), Is.True);
    }
}
