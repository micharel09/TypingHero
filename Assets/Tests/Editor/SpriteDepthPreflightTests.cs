using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public sealed class SpriteDepthPreflightTests
{
    const string Root = "Assets/__DepthReviewRepro";
    EditorBuildSettingsScene[] originalSettings;
    Sprite first, current, animated;
    Scene scene;
    string scenePath;

    [SetUp] public void SetUp()
    {
        Assert.That(AssetDatabase.IsValidFolder(Root),Is.False);
        originalSettings = EditorBuildSettings.scenes;
        AssetDatabase.CreateFolder("Assets","__DepthReviewRepro");
        first = Sprite("first"); current = Sprite("current"); animated = Sprite("animatedOnly");
        scenePath = Root + "/actors.unity";
        string template = File.ReadAllText("Assets/Scenes/DungeonWorld.unity");
        int objects = Regex.Match(template,@"^--- !u!(?!(29|104|157|196) )",RegexOptions.Multiline).Index;
        Assert.That(objects,Is.GreaterThan(0));
        File.WriteAllText(scenePath,template.Substring(0,objects));
        AssetDatabase.ImportAsset(scenePath);
        scene = EditorSceneManager.OpenScene(scenePath,OpenSceneMode.Single);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath,true) };
    }
    [TearDown] public void TearDown()
    {
        Environment.SetEnvironmentVariable("TYPINGHERO_LIGHTING_CAPTURE",null);
        EditorBuildSettings.scenes = originalSettings;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        AssetDatabase.DeleteAsset(Root);
    }
    static Sprite Sprite(string name)
    {
        string path = Root + "/" + name + ".png";
        var texture = new Texture2D(8,8);
        texture.SetPixels(Enumerable.Repeat(Color.white,64).ToArray()); texture.Apply();
        File.WriteAllBytes(path,texture.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    static void Map(Sprite sprite)
    {
        string path = Root + "/" + sprite.name + "-depth.png";
        var texture = new Texture2D(8,8);
        File.WriteAllBytes(path,texture.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        var map = (TextureImporter)AssetImporter.GetAtPath(path);
        map.textureType = TextureImporterType.Default; map.sRGBTexture = false;
        map.mipmapEnabled = false; map.filterMode = FilterMode.Point;
        map.textureCompression = TextureImporterCompression.Uncompressed; map.SaveAndReimport();
        var source = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite));
        source.secondarySpriteTextures = new[] { new SecondarySpriteTexture {
            name = "_DepthMap", texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path) } };
        source.SaveAndReimport();
    }
    void SelectLibrary(SpriteDepthLibrary library)
    {
        var world = ScriptableObject.CreateInstance<WorldDefinition>(); world.spriteDepth = library;
        AssetDatabase.CreateAsset(world,Root + "/world.asset");
        var gate = new GameObject("selection",typeof(DungeonSessionBindings));
        gate.GetComponent<DungeonSessionBindings>().worldDefinition = world;
    }
    static AnimatorController Controller(Sprite value)
    {
        var clip = new AnimationClip();
        AnimationUtility.SetObjectReferenceCurve(clip,EditorCurveBinding.PPtrCurve("",typeof(SpriteRenderer),"m_Sprite"),
            new[] { new ObjectReferenceKeyframe { time=0,value=value } });
        AssetDatabase.CreateAsset(clip,Root + "/external.anim");
        var controller = AnimatorController.CreateAnimatorControllerAtPath(Root + "/external.controller");
        controller.AddMotion(clip); AssetDatabase.SaveAssetIfDirty(controller); return controller;
    }
    [TestCase(true, false)] [TestCase(false, false)] [TestCase(true, true)]
    public void UnsavedPrefabRendererReferenceIncludesAnimatorFrames(bool bound, bool ancestor)
    {
        Map(first); Map(current);
        first = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/first.png");
        current = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/current.png");
        var library = ScriptableObject.CreateInstance<SpriteDepthLibrary>();
        AssetDatabase.CreateAsset(library,Root + "/library.asset"); SelectLibrary(library);
        var actor = new GameObject("actor",typeof(SpriteRenderer),typeof(PlayerHealth));
        actor.GetComponent<SpriteRenderer>().sprite = first;
        var image = new SerializedObject(actor.AddComponent<NormalHitSpark>());
        EditorSceneManager.SaveScene(scene);
        var external = new GameObject("ContactShadow",typeof(SpriteRenderer),typeof(Animator));
        external.GetComponent<SpriteRenderer>().sprite = current;
        var animator = external.GetComponent<Animator>();
        if (ancestor)
        {
            var parent = new GameObject("inactive Animator",typeof(Animator));
            external.transform.SetParent(parent.transform);
            UnityEngine.Object.DestroyImmediate(animator);
            animator = parent.GetComponent<Animator>();
        }
        animator.runtimeAnimatorController = Controller(animated);
        var clip = animator.runtimeAnimatorController.animationClips.Single();
        if (ancestor)
        {
            AnimationUtility.SetObjectReferenceCurve(clip,EditorCurveBinding.PPtrCurve("",typeof(SpriteRenderer),"m_Sprite"),null);
            AnimationUtility.SetObjectReferenceCurve(clip,EditorCurveBinding.PPtrCurve(external.name,typeof(SpriteRenderer),"m_Sprite"),
                new[] { new ObjectReferenceKeyframe { time=0,value=animated } });
            AssetDatabase.SaveAssetIfDirty(clip);
        }
        var material = new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/KayKitDungeon/DungeonSprite.mat"));
        material.SetFloat("_SelfShadowStrength",0);
        AssetDatabase.CreateAsset(material,Root + "/disabled.mat");
        external.GetComponent<SpriteRenderer>().sharedMaterial = material;
        external.GetComponent<SpriteRenderer>().enabled = false;
        if (bound) external.AddComponent<SpriteDepthBinding>().enabled = false;
        string prefabPath = Root + "/external.prefab";
        var prefabRoot = external.transform.root.gameObject;
        prefabRoot.SetActive(false);
        var prefab = PrefabUtility.SaveAsPrefabAsset(prefabRoot,prefabPath);
        UnityEngine.Object.DestroyImmediate(prefabRoot);
        image.FindProperty("image").objectReferenceValue = prefab.GetComponentInChildren<SpriteRenderer>(true);
        image.ApplyModifiedPropertiesWithoutUndo(); EditorSceneManager.MarkSceneDirty(scene);
        Assert.That(AssetDatabase.GetDependencies(scenePath,true).Contains(prefabPath),Is.False,
            "The prefab is reachable only through an unsaved in-memory reference");
        var inventory = SpriteDepthAuthoring.ReferencedSprites();
        Assert.That(inventory.Contains(first),Is.True);
        Assert.That(inventory.Contains(current),Is.EqualTo(bound));
        Assert.That(inventory.Contains(animated),Is.EqualTo(bound));
        if (bound) Assert.Throws<BuildFailedException>(SpriteDepthAuthoring.AuditCoverage);
        else Assert.DoesNotThrow(SpriteDepthAuthoring.AuditCoverage);
        var beforeSave = inventory.ToArray();
        EditorSceneManager.SaveScene(scene);
        Assert.That(AssetDatabase.GetDependencies(scenePath,true).Contains(prefabPath),Is.True);
        inventory = SpriteDepthAuthoring.ReferencedSprites();
        Assert.That(inventory.Contains(animated),Is.EqualTo(bound),"Saving makes dependency discovery find the Animator");
        if (bound) Assert.Throws<BuildFailedException>(SpriteDepthAuthoring.AuditCoverage);
        else Assert.DoesNotThrow(SpriteDepthAuthoring.AuditCoverage);
        Assert.That(inventory,Is.EquivalentTo(beforeSave));
        Map(animated);
        Assert.DoesNotThrow(SpriteDepthAuthoring.AuditCoverage);
    }
    [Test]
    public void CaptureAllRejectsMissingMapBeforeDirtyGuardAndPreservesOverride()
    {
        var production = AssetDatabase.LoadAssetAtPath<SpriteDepthLibrary>("Assets/Data/Lighting/ActorDepth.asset");
        byte[] before = File.ReadAllBytes(AssetDatabase.GetAssetPath(production));
        var actor = new GameObject("actor",typeof(SpriteRenderer),typeof(PlayerHealth));
        var renderer = actor.GetComponent<SpriteRenderer>();
        renderer.sprite = production.frames[0].sprite;
        EditorSceneManager.SaveScene(scene);
        renderer.sprite = animated;
        EditorSceneManager.MarkSceneDirty(scene);
        WithNoCaptureEffects(() => {
            var error = Assert.Throws<BuildFailedException>(SpriteDepthRenderChecks.CaptureAll);
            Assert.That(error.Message,Does.Contain("animatedOnly"));
        });
        Assert.That(renderer.sprite,Is.EqualTo(animated));
        Assert.That(File.ReadAllBytes(AssetDatabase.GetAssetPath(production)),Is.EqualTo(before));
        renderer.sprite = production.frames[0].sprite;
        WithNoCaptureEffects(() => Assert.That(
            Assert.Throws<InvalidOperationException>(SpriteDepthRenderChecks.CaptureAll).Message,
            Does.Contain(scenePath)));
        EditorSceneManager.SaveScene(scene);
        Assert.DoesNotThrow(() => SpriteDepthRenderChecks.Preflight(true));
    }
    [TestCase("Capture")] [TestCase("Investigate")] [TestCase("CaptureAll")]
    public void EveryCaptureGuardsDirtySceneOutsideBuildSettings(string entry)
    {
        EditorSceneManager.SaveScene(scene);
        var extra = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
        var marker = new GameObject("unsaved override");
        SceneManager.MoveGameObjectToScene(marker,extra);
        EditorSceneManager.MarkSceneDirty(extra);
        Action capture = entry == "Capture" ? SpriteDepthRenderChecks.Capture :
            entry == "Investigate" ? SpriteDepthRenderChecks.Investigate : SpriteDepthRenderChecks.CaptureAll;
        WithNoCaptureEffects(() => Assert.That(Assert.Throws<InvalidOperationException>(() => capture()).Message,
            Does.Contain(extra.name).And.Contain("Save all open dirty scenes")));
        Assert.That(marker,Is.Not.Null);
        Assert.That(extra.isDirty,Is.True);
        EditorSceneManager.CloseScene(extra,true);
        Assert.DoesNotThrow(() => SpriteDepthRenderChecks.Preflight(true));
    }
    void WithNoCaptureEffects(Action assertion)
    {
        string output = Path.Combine(Path.GetTempPath(),"depth-preflight-" + Guid.NewGuid().ToString("N"));
        Environment.SetEnvironmentVariable("TYPINGHERO_LIGHTING_CAPTURE",output);
        var setup = EditorSceneManager.GetSceneManagerSetup();
        var active = SceneManager.GetActiveScene();
        byte[] savedScene = File.ReadAllBytes(scenePath);
        assertion();
        Assert.That(Directory.Exists(output),Is.False);
        Assert.That(EditorSceneManager.GetSceneManagerSetup().Select(x => (x.path,x.isLoaded,x.isActive)),
            Is.EqualTo(setup.Select(x => (x.path,x.isLoaded,x.isActive))));
        Assert.That(SceneManager.GetActiveScene(),Is.EqualTo(active));
        Assert.That(File.ReadAllBytes(scenePath),Is.EqualTo(savedScene));
        Assert.That(scene.isLoaded,Is.True);
    }
}
