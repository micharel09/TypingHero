using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

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
    public void ValidReplacementBindingUpdatesDerivedIndexAndIsStable()
    {
        var custom = WriteTexture(Root + "/custom.png",16,8);
        WriteTexture(conventional,16,8); Bind(custom);
        SpriteDepthAuthoring.Synchronize(library, Sprites);
        Assert.That(library.frames.All(x => x.depth == custom), Is.True);
        var replacement = AssetDatabase.LoadAssetAtPath<Texture2D>(conventional);
        byte[] original = File.ReadAllBytes(Root + "/custom.png");
        Bind(replacement);
        SpriteDepthAuthoring.Synchronize(library, Sprites);
        Assert.That(library.frames.All(x => x.depth == replacement), Is.True);
        string index = File.ReadAllText(Root + "/library.asset");
        byte[] binding = File.ReadAllBytes(source + ".meta");
        SpriteDepthAuthoring.Synchronize(library, Sprites.Reverse());
        Assert.That(File.ReadAllText(Root + "/library.asset"), Is.EqualTo(index));
        Assert.That(File.ReadAllBytes(source + ".meta"), Is.EqualTo(binding));
        Assert.That(File.ReadAllBytes(Root + "/custom.png"), Is.EqualTo(original));
    }
    [Test]
    public void InvalidReplacementDoesNotWriteIndexAndUnboundLegacyConflictIsRejected()
    {
        var custom = WriteTexture(Root + "/custom.png",16,8); Bind(custom);
        SpriteDepthAuthoring.Synchronize(library, Sprites);
        string index = File.ReadAllText(Root + "/library.asset");
        Bind(WriteTexture(Root + "/wrong.png",8,8));
        var error = Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
        Assert.That(error.Message, Does.Contain("requires aligned"));
        Assert.That(File.ReadAllText(Root + "/library.asset"), Is.EqualTo(index));
        var second = WriteTexture(Root + "/second.png",16,8);
        library.frames[1].depth = second;
        var importer = (TextureImporter)AssetImporter.GetAtPath(source);
        importer.secondarySpriteTextures = Array.Empty<SecondarySpriteTexture>(); importer.SaveAndReimport();
        error = Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Synchronize(library, Sprites));
        Assert.That(error.Message, Does.Contain("conflicting or broken"));
        Assert.That(importer.secondarySpriteTextures, Is.Empty);
        Assert.That(File.ReadAllText(Root + "/library.asset"), Is.EqualTo(index));
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

    [Test]
    public void ProductionInventoryMatchesAuthoredIndexAndPassesPublicStrictAudit()
    {
        var production = AssetDatabase.LoadAssetAtPath<SpriteDepthLibrary>("Assets/Data/Lighting/ActorDepth.asset");
        var expected = production.frames.Select(x => x.sprite).ToArray();
        var inventory = SpriteDepthAuthoring.ReferencedSprites();
        Assert.That(inventory, Is.EquivalentTo(expected), "Keep every authored frame, without adding unbound effects");
        foreach (var role in Enum.GetValues(typeof(EndlessRole)).Cast<EndlessRole>())
        {
            var opponent = Resources.Load<EndlessOpponent>("Endless/" + role);
            Assert.That(inventory.Contains(opponent.idle), Is.True);
            var clips = opponent.controller.animationClips.Concat(new[] { opponent.hurt }).Where(x => x);
            foreach (var clip in clips)
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip)
                .Where(x => x.path == "" && x.type == typeof(SpriteRenderer) && x.propertyName == "m_Sprite"))
            foreach (var frame in AnimationUtility.GetObjectReferenceCurve(clip,binding).Select(x => x.value as Sprite).Where(x => x))
                Assert.That(inventory.Contains(frame), Is.True, role + "/" + clip.name);
        }
        Assert.DoesNotThrow(SpriteDepthAuthoring.AuditCoverage);
        // This is the strict validation invoked by CaptureAll before rendering.
        Assert.DoesNotThrow(() => SpriteDepthAuthoring.Validate(production,true));
    }

    [TestCase("excludedChild", false)]
    [TestCase("excludedChild", true)]
    [TestCase("boundChild", false)]
    [TestCase("boundChild", true)]
    [TestCase("excludedExternal", false)]
    [TestCase("prefabExternal", false)]
    [TestCase("excludedPrefabExternal", false)]
    [TestCase("boundCurves", false)]
    [TestCase("mixedCurves", false)]
    [TestCase("mixedCurves", true)]
    [TestCase("overrideController", false)]
    [TestCase("hurt", true)]
    [TestCase("shared", false)]
    [TestCase("controller", false)]
    [TestCase("controller", true)]
    [TestCase("unloaded", false)]
    [TestCase("sprite", false)]
    [TestCase("sprite", true)]
    [TestCase("contact", false)]
    [TestCase("contact", true)]
    [TestCase("externalAnimator", true)]
    [TestCase("externalRenderer", false)]
    [TestCase("loose", false)]
    [TestCase("loose", true)]
    public void InventoryFollowsSceneActorReferencesWithoutChangingSceneState(string variant, bool loaded)
    {
        var settings = EditorBuildSettings.scenes;
        var active = SceneManager.GetActiveScene();
        string scenePath = Root + "/actors.unity";
        // NewScene(Additive) rejects the test runner's unsaved Untitled scene.
        // Reuse only the native settings header of a valid saved scene, without any objects.
        string template = File.ReadAllText("Assets/Scenes/DungeonWorld.unity");
        int objects = Regex.Match(template,@"^--- !u!(?!(29|104|157|196) )",RegexOptions.Multiline).Index;
        Assert.That(objects, Is.GreaterThan(0));
        File.WriteAllText(scenePath,template.Substring(0,objects));
        AssetDatabase.ImportAsset(scenePath);
        var scene = EditorSceneManager.OpenScene(scenePath,OpenSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        try
        {
            var actor = new GameObject("actor",typeof(SpriteRenderer),typeof(Animator),typeof(PlayerHealth));
            SceneManager.MoveGameObjectToScene(actor, scene);
            actor.GetComponent<SpriteRenderer>().sprite = Sprites[0];
            actor.GetComponent<Animator>().runtimeAnimatorController = Controller("base",Sprites[0]);
            var prefab = PrefabUtility.SaveAsPrefabAsset(actor,Root + "/actor.prefab");
            UnityEngine.Object.DestroyImmediate(actor);
            actor = (GameObject)PrefabUtility.InstantiatePrefab(prefab,scene);
            var world = ScriptableObject.CreateInstance<WorldDefinition>();
            world.spriteDepth = library;
            AssetDatabase.CreateAsset(world,Root + "/world.asset");
            var gate = new GameObject("depth selection",typeof(DungeonSessionBindings));
            SceneManager.MoveGameObjectToScene(gate,scene);
            gate.GetComponent<DungeonSessionBindings>().worldDefinition = world;
            EditorSceneManager.SaveScene(scene,scenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath,true) };
            Assert.That(SpriteDepthAuthoring.ReferencedSprites().Contains(Sprites[1]), Is.False,
                "An unreferenced sprite in the same sheet must not enter the inventory");
            if (variant == "controller" || variant == "unloaded") actor.GetComponent<Animator>().runtimeAnimatorController = Controller("override",Sprites[1]);
            else if (variant == "sprite") actor.GetComponent<SpriteRenderer>().sprite = Sprites[1];
            else if (variant == "contact")
            {
                var contacts = ScriptableObject.CreateInstance<ParryContactLibrary>();
                contacts.frames = new[] { new ParryContactLibrary.Frame { sprite = Sprites[0], contactCel = Sprites[1], defenseCel = Sprites[1] } };
                AssetDatabase.CreateAsset(contacts,Root + "/contacts.asset");
                var parry = new SerializedObject(actor.AddComponent<ParrySystem>());
                parry.FindProperty("contactLibrary").objectReferenceValue = contacts;
                parry.ApplyModifiedPropertiesWithoutUndo();
            }
            else if (variant == "loose")
            {
                var loose = new GameObject("loose",typeof(SpriteRenderer),typeof(PlayerHealth));
                SceneManager.MoveGameObjectToScene(loose,scene);
                loose.GetComponent<SpriteRenderer>().sprite = Sprites[1];
            }
            else if (variant == "overrideController")
            {
                var animator = actor.GetComponent<Animator>();
                var replacement = Controller("replacement", Sprites[1]).animationClips.Single();
                var controller = new AnimatorOverrideController(animator.runtimeAnimatorController);
                controller[animator.runtimeAnimatorController.animationClips.Single()] = replacement;
                AssetDatabase.CreateAsset(controller,Root + "/override.overrideController");
                animator.runtimeAnimatorController = controller;
            }
            else if (variant == "hurt")
            {
                var pose = new SerializedObject(actor.AddComponent<EnemySlayerHitPose>());
                pose.FindProperty("hitClip").objectReferenceValue = Controller("hurt",Sprites[1]).animationClips.Single();
                pose.ApplyModifiedPropertiesWithoutUndo();
            }
            else if (variant == "excludedChild" || variant == "boundChild" || variant == "mixedCurves" || variant == "boundCurves" || variant == "shared")
            {
                var child = new GameObject(variant == "boundChild" ? "ContactShadow" : "arbitrary effect",typeof(SpriteRenderer));
                child.transform.SetParent(actor.transform);
                child.GetComponent<SpriteRenderer>().sprite = (variant == "mixedCurves" || variant == "boundCurves") ? null : Sprites[1];
                if (variant == "boundChild" || variant == "boundCurves")
                {
                    child.AddComponent<SpriteDepthBinding>().enabled = false;
                    var material = new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/KayKitDungeon/DungeonSprite.mat"));
                    material.SetFloat("_SelfShadowStrength",0);
                    AssetDatabase.CreateAsset(material,Root + "/disabled.mat");
                    child.GetComponent<SpriteRenderer>().sharedMaterial = material;
                    child.GetComponent<SpriteRenderer>().enabled = false;
                }
                if (variant == "shared") actor.GetComponent<SpriteRenderer>().sprite = Sprites[1];
                if (variant == "mixedCurves" || variant == "boundCurves")
                {
                    var clip = actor.GetComponent<Animator>().runtimeAnimatorController.animationClips.Single();
                    AnimationUtility.SetObjectReferenceCurve(clip,EditorCurveBinding.PPtrCurve(child.name,typeof(SpriteRenderer),"m_Sprite"),
                        new[] { new ObjectReferenceKeyframe { time = 0, value = Sprites[1] } });
                    AssetDatabase.SaveAssetIfDirty(clip);
                }
            }
            else
            {
                var external = new GameObject("ContactShadow",typeof(Animator),typeof(SpriteRenderer));
                SceneManager.MoveGameObjectToScene(external,scene);
                if (variant != "excludedExternal" && variant != "excludedPrefabExternal")
                    external.AddComponent<SpriteDepthBinding>().enabled = false;
                if (variant == "externalAnimator")
                {
                    external.GetComponent<Animator>().runtimeAnimatorController = Controller("external",Sprites[1]);
                    actor.GetComponent<PlayerHealth>().animator = external.GetComponent<Animator>();
                }
                else
                {
                    external.GetComponent<SpriteRenderer>().sprite = Sprites[1];
                    var target = actor.AddComponent<NormalHitSpark>();
                    var serialized = new SerializedObject(target);
                    var receiver = external.GetComponent<SpriteRenderer>();
                    if (variant == "prefabExternal" || variant == "excludedPrefabExternal")
                        receiver = PrefabUtility.SaveAsPrefabAsset(external,Root + "/external.prefab").GetComponent<SpriteRenderer>();
                    serialized.FindProperty("image").objectReferenceValue = receiver;
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                }
            }
            // Persist overrides only for the unopened-scene case; loaded scenes deliberately remain unsaved.
            foreach (var component in actor.GetComponents<Component>())
                if (PrefabUtility.IsPartOfPrefabInstance(component)) PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!loaded)
            {
                EditorSceneManager.SaveScene(scene);
                if (active.IsValid() && active.isLoaded) SceneManager.SetActiveScene(active);
                EditorSceneManager.CloseScene(scene,variant != "unloaded");
            }
            if (active.IsValid() && active.isLoaded) SceneManager.SetActiveScene(active);
            var setup = EditorSceneManager.GetSceneManagerSetup();
            int previewCount = EditorSceneManager.previewSceneCount;
            bool dirty = loaded && scene.isDirty;
            byte[] savedScene = File.ReadAllBytes(scenePath);
            var inventory = SpriteDepthAuthoring.ReferencedSprites();
            bool included = !variant.StartsWith("excluded",StringComparison.Ordinal) && variant != "mixedCurves";
            Assert.That(inventory.Contains(Sprites[1]), Is.EqualTo(included), "Scene-specific " + variant);
            Assert.That(inventory.Count(x => x == Sprites[1]), Is.EqualTo(included ? 1 : 0));
            string export = Root + "/inventory.json";
            SpriteDepthAuthoring.ExportInventory(export);
            if (variant == "mixedCurves") Assert.That(File.ReadAllText(export), Does.Not.Contain("arbitrary effect"));
            Assert.That(EditorSceneManager.GetSceneManagerSetup().Select(SceneState), Is.EqualTo(setup.Select(SceneState)));
            Assert.That(File.ReadAllBytes(scenePath), Is.EqualTo(savedScene));
            if (loaded)
            {
                Assert.That(scene.isDirty, Is.EqualTo(dirty));
                Assert.That(actor, Is.Not.Null);
            }
            Assert.That(EditorSceneManager.previewSceneCount, Is.EqualTo(previewCount));
            var scoped = inventory.Where(x => AssetDatabase.GetAssetPath(x) == source).ToArray();
            SpriteDepthAuthoring.Synchronize(library,scoped);
            Assert.DoesNotThrow(() => SpriteDepthAuthoring.Validate(library,scoped));
            Assert.Throws<BuildFailedException>(() => SpriteDepthAuthoring.Validate(library,scoped,true));
            Assert.Throws<BuildFailedException>(SpriteDepthAuthoring.AuditCoverage);
            WriteTexture(conventional,16,8);
            SpriteDepthAuthoring.Synchronize(library,scoped);
            Assert.That(library.CreateLookup().ContainsKey(Sprites[1]), Is.EqualTo(included));
            Assert.DoesNotThrow(() => SpriteDepthAuthoring.Validate(library,scoped,true));
            Assert.DoesNotThrow(SpriteDepthAuthoring.AuditCoverage);
            Assert.DoesNotThrow(() => SpriteDepthAuthoring.Validate(library,true));
        }
        finally
        {
            EditorBuildSettings.scenes = settings;
            if (scene.IsValid()) EditorSceneManager.CloseScene(scene,true);
            if (active.IsValid() && active.isLoaded) SceneManager.SetActiveScene(active);
        }
    }
    static string SceneState(SceneSetup scene) => scene.path + ":" + scene.isLoaded + ":" + scene.isActive;
    static AnimatorController Controller(string name, Sprite sprite)
    {
        var clip = new AnimationClip();
        AnimationUtility.SetObjectReferenceCurve(clip,EditorCurveBinding.PPtrCurve("",typeof(SpriteRenderer),"m_Sprite"),
            new[] { new ObjectReferenceKeyframe { time = 0, value = sprite } });
        AssetDatabase.CreateAsset(clip,Root + "/" + name + ".anim");
        var controller = AnimatorController.CreateAnimatorControllerAtPath(Root + "/" + name + ".controller");
        controller.AddMotion(clip);
        AssetDatabase.SaveAssetIfDirty(controller);
        return controller;
    }
}
