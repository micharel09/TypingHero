using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class LightingAuthoringChecks
{
    static UnityEngine.Object[] retained;
    static Dictionary<string, byte[]> Snapshot() => Directory.GetFiles("Assets/Data/Lighting", "*", SearchOption.AllDirectories)
        .Concat(Directory.GetFiles("Assets/Tests/Fixtures/Lighting", "*.asset"))
        .Concat(Directory.GetFiles("Assets/Art/Environment/PixelDungeon", "*.mat"))
        .Concat(new[] { "Assets/Art/Environment/HangingLantern/PixelFire.mat", "Assets/Art/Environment/KayKitDungeon/DungeonPost.asset",
            "Assets/Settings/UniversalRP.asset", "Assets/Settings/DungeonForward.asset" }).ToDictionary(x => x, File.ReadAllBytes);
    public static void PrepareAndVerify()
    {
        const string cameraPath = "Assets/Data/Lighting/WorldCamera.prefab";
        if (!AssetDatabase.LoadAssetAtPath<GameObject>(cameraPath))
        {
            var definition = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
            EditorSceneManager.OpenScene(definition.scenePath);
            var world = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true);
            PrefabUtility.SaveAsPrefabAsset(world.view.gameObject, cameraPath);
        }
        const string combatPath = "Assets/Data/Lighting/CombatLightingRig.prefab";
        if (!AssetDatabase.LoadAssetAtPath<GameObject>(combatPath))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            var rig = UnityEngine.Object.FindObjectsOfType<ClashLightingRig>(true).Single(x => x.profile);
            PrefabUtility.SaveAsPrefabAsset(rig.gameObject, combatPath);
        }
        LightingFixtureBuild.Create();
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(x => x.path != LightingFixtureBuild.ScenePath)
            .Concat(new[] { new EditorBuildSettingsScene(LightingFixtureBuild.ScenePath, true) }).ToArray();
        Verify();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }
    public static void VerifyPreviewScenes()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            var a = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
            var b = AssetDatabase.LoadAssetAtPath<WorldDefinition>(LightingFixtureBuild.DefinitionPath);
            var first = EditorSceneManager.OpenScene(a.scenePath);
            Color firstFog = RenderSettings.fogColor;
            var second = EditorSceneManager.OpenScene(b.scenePath, OpenSceneMode.Additive);
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(second);
            Color secondFog = RenderSettings.fogColor;
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(first);
            Undo.IncrementCurrentGroup();
            WorldLightingAuthoring.PreviewLoaded();
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() != first || RenderSettings.fogColor != a.lighting.fogColor)
                throw new InvalidOperationException("Preview changed the active scene or wrote another world's fog into it");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(second);
            if (RenderSettings.fogColor != b.lighting.fogColor) throw new InvalidOperationException("Preview missed alternate world fog");
            Undo.FlushUndoRecordObjects(); Undo.PerformUndo();
            if (RenderSettings.fogColor != secondFog) throw new InvalidOperationException("Preview Undo missed alternate world fog");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(first);
            if (RenderSettings.fogColor != firstFog) throw new InvalidOperationException("Preview Undo missed first world fog");
            Debug.Log("PASS: multi-scene Preview writes each world's fog, retains active scene and supports Undo");
        }
        finally
        {
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
    public static void Verify()
    {
        var definition = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
        var light = AssetDatabase.LoadAssetAtPath<LightProfile>("Assets/Data/Lighting/MainLantern.asset");
        var material = light.flame.material;
        var haze = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/PixelDungeon/LowHaze.mat");
        var rune = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/PixelDungeon/RearRune.mat");
        var profile = definition.lighting; var impact = definition.cameraImpact;
        var other = AssetDatabase.LoadAssetAtPath<WorldDefinition>(LightingFixtureBuild.DefinitionPath);
        var styles = new UnityEngine.Object[] { profile, impact, haze, rune, other.lighting };
        var originals = styles.Select(x => EditorJsonUtility.ToJson(x)).ToArray();
        var unused = ScriptableObject.CreateInstance<WorldDefinition>();
        const string unusedPath = "Assets/Tests/Fixtures/Lighting/UnusedIncompleteTemplate.asset";
        definition.lighting.post.TryGet<Bloom>(out var bloom);
        retained = new UnityEngine.Object[] { definition, light, material, bloom }.Concat(styles).ToArray();
        string beforeLight = EditorJsonUtility.ToJson(light), beforeMaterial = EditorJsonUtility.ToJson(material), beforeBloom = EditorJsonUtility.ToJson(bloom);
        try
        {
            AssetDatabase.CreateAsset(unused, unusedPath);
            profile.focusOffset = 1.1f; profile.focusHalfWidth = 2.2f; profile.depthEdgeDistance = .9f;
            profile.clothAmplitude = .07f; profile.clothDirection = Vector3.right;
            var normal = impact.presets.Single(x => x.trigger == CombatCameraProfile.Event.Normal);
            normal.zoomPeak = .06f; normal.attack = .04f; normal.hold = .05f; normal.release = .21f;
            foreach (var preset in impact.presets)
                if (preset.trigger != CombatCameraProfile.Event.SlayerEnter) preset.shakeDuration = .19f;
            haze.SetFloat("_Taper", 2.7f); haze.SetFloat("_DriftSpeed", .9f);
            rune.SetFloat("_RingRadius", .48f); rune.SetFloat("_ScanFrequency", 17);
            other.lighting.focusHalfWidth = 3.1f;
            foreach (var style in styles) EditorUtility.SetDirty(style);
            new LightingBuildValidation().OnPreprocessBuild(null);
            light.intensity = 19; light.projectedFlameHeight = 9;
            material.SetColor("_FlameOrange", new Color(.4f, .5f, .6f)); bloom.intensity.Override(.14f);
            EditorUtility.SetDirty(light); EditorUtility.SetDirty(material); EditorUtility.SetDirty(bloom); AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(definition.scenePath);
            var originalWorld = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true);
            Vector3 roomPosition = originalWorld.environment.position, cameraPosition = originalWorld.view.transform.position;
            EditorSceneManager.OpenScene(other.scenePath, OpenSceneMode.Additive);
            var setup = EditorSceneManager.GetSceneManagerSetup().Select(x => x.path + "|" + x.isLoaded + "|" + x.isActive).ToArray();
            var authored = Snapshot();
            for (int pass = 0; pass < 2; pass++)
            {
                PixelUIBuild.Apply();
                if (!setup.SequenceEqual(EditorSceneManager.GetSceneManagerSetup().Select(x => x.path + "|" + x.isLoaded + "|" + x.isActive)))
                    throw new InvalidOperationException("Builder did not restore the additive scene setup");
            }
            foreach (var file in authored)
                if (!file.Value.SequenceEqual(File.ReadAllBytes(file.Key))) throw new InvalidOperationException("Builder overwrote authored data: " + file.Key);
            EditorSceneManager.OpenScene(definition.scenePath);
            var world = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true);
            if (world.environment.position != roomPosition || world.view.transform.position != cameraPosition)
                throw new InvalidOperationException("Builder moved authored room/camera placement");
            var instance = world.environment.GetComponentsInChildren<DungeonTorchLight>(true).Single(x => x.profile == light);
            if (instance.GetComponent<Light>().intensity != 19) throw new InvalidOperationException("Builder did not apply the edited profile");
            EditorSceneManager.OpenScene(other.scenePath);
            var alternate = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true);
            if (alternate.environment.name != "Alternate Authored Hall") throw new InvalidOperationException("Alternate authored room name changed");
            if (alternate.Arena.profile != other.lighting) throw new InvalidOperationException("Alternate world profile was not applied");
            Debug.Log("PASS: edited blur/cloth/impact/haze/rune/light/post survive two full builders; alternate world outside default folder applied; unused template ignored");
        }
        finally
        {
            AssetDatabase.DeleteAsset(unusedPath);
            for (int i = 0; i < styles.Length; i++)
            { EditorJsonUtility.FromJsonOverwrite(originals[i], styles[i]); EditorUtility.SetDirty(styles[i]); }
            EditorJsonUtility.FromJsonOverwrite(beforeLight, light); EditorJsonUtility.FromJsonOverwrite(beforeMaterial, material);
            EditorJsonUtility.FromJsonOverwrite(beforeBloom, bloom);
            EditorUtility.SetDirty(light); EditorUtility.SetDirty(material); EditorUtility.SetDirty(bloom); AssetDatabase.SaveAssets();
            PixelUIBuild.Apply();
            retained = null;
        }
    }
}
