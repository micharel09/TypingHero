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
    public static void VerifyPreviewBindingsAndUndo()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        string folder = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Fixtures/Lighting/PreviewUndoTemporary");
        AssetDatabase.CreateFolder("Assets/Tests/Fixtures/Lighting", Path.GetFileName(folder));
        var failures = new List<string>();
        void Check(bool valid, string message)
        { if (!valid) { failures.Add(message); Debug.Log("PREVIEW REGRESSION: " + message); } }
        T Save<T>(T asset, string name) where T : UnityEngine.Object
        { AssetDatabase.CreateAsset(asset, folder + "/" + name + ".asset"); return asset; }
        WorldDefinition CopyWorld(string source, string name)
        {
            var original = AssetDatabase.LoadAssetAtPath<WorldDefinition>(source);
            var definition = Save(UnityEngine.Object.Instantiate(original), name);
            string scenePath = folder + "/" + name + ".unity";
            if (!AssetDatabase.CopyAsset(original.scenePath, scenePath)) throw new InvalidOperationException("Cannot copy preview fixture");
            definition.scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath); definition.scenePath = scenePath;
            definition.lighting = Save(UnityEngine.Object.Instantiate(original.lighting), name + "Lighting");
            definition.combat = Save(UnityEngine.Object.Instantiate(original.combat), name + "Combat");
            definition.lighting.baking = Save(new LightingSettings { autoGenerate = false, bakedGI = false, realtimeGI = false }, name + "Bake");
            EditorUtility.SetDirty(definition); EditorUtility.SetDirty(definition.lighting);
            return definition;
        }
        try
        {
            var a = CopyWorld("Assets/Data/Lighting/DungeonWorld.asset", "A");
            var b = CopyWorld(LightingFixtureBuild.DefinitionPath, "B");
            var priorBake = Save(new LightingSettings { autoGenerate = false, bakedGI = false, realtimeGI = false }, "PriorBake");
            var first = EditorSceneManager.OpenScene(a.scenePath);
            first.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<DungeonWorld>(true)).Single().definition = a;
            var firstFog = Color.magenta; RenderSettings.fogColor = firstFog; Lightmapping.lightingSettings = priorBake;
            var local = new GameObject("World-local rig"); local.SetActive(false);
            var localRig = local.AddComponent<ClashLightingRig>(); localRig.profile = b.combat;
            var second = EditorSceneManager.OpenScene(b.scenePath, OpenSceneMode.Additive);
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(second);
            second.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<DungeonWorld>(true)).Single().definition = b;
            var secondFog = Color.green; RenderSettings.fogColor = secondFog; Lightmapping.lightingSettings = null;
            string gamePath = folder + "/Gameplay.unity";
            if (!AssetDatabase.CopyAsset("Assets/Scenes/SampleScene.unity", gamePath)) throw new InvalidOperationException("Cannot copy gameplay fixture");
            var game = EditorSceneManager.OpenScene(gamePath, OpenSceneMode.Additive);
            var gate = game.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<DungeonSessionBindings>(true)).Single();
            gate.worldDefinition = a;
            var rigs = gate.roots.SelectMany(x => x.GetComponentsInChildren<ClashLightingRig>(true)).Distinct().ToArray();
            if (rigs.Length == 0) throw new InvalidOperationException("Gameplay fixture has no combat rig");
            var originalProfiles = rigs.Select(x => x.profile).ToArray();
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(game);
            var otherHost = new GameObject("Unrelated gate"); otherHost.SetActive(false);
            var otherGate = otherHost.AddComponent<DungeonSessionBindings>(); otherGate.worldDefinition = b;
            var otherRoot = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Data/Lighting/CombatLightingRig.prefab"), game);
            otherRoot.name = "Explicit prefab root outside gate hierarchy"; otherRoot.SetActive(false);
            var otherRig = otherRoot.GetComponent<ClashLightingRig>(); otherRig.profile = a.combat;
            PrefabUtility.RecordPrefabInstancePropertyModifications(otherRig);
            otherGate.roots = new[] { otherRoot, otherRoot }; // Duplicate roots must not duplicate application/Undo.
            var orphan = new GameObject("Unbound rig"); orphan.SetActive(false);
            var orphanRig = orphan.AddComponent<ClashLightingRig>(); orphanRig.profile = originalProfiles[0];
            AssetDatabase.SaveAssets();
            foreach (var scene in new[] { first, second, game }) EditorSceneManager.SaveScene(scene);
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(first);
            var loaded = EditorSceneManager.GetSceneManagerSetup().Select(x => x.path + "|" + x.isActive + "|" + x.isLoaded).ToArray();
            var styles = new UnityEngine.Object[] { a, b, a.lighting, b.lighting, a.combat, b.combat, a.lighting.baking, b.lighting.baking };
            var authored = styles.Select(EditorJsonUtility.ToJson).ToArray();
            WorldLightingAuthoring.PreviewLoaded();
            Check(rigs.All(x => x.profile == a.combat), "Preview missed split gameplay rigs for A");
            Check(otherRig.profile == b.combat, "Preview missed explicit roots for B");
            Check(localRig.profile == a.combat && orphanRig.profile == originalProfiles[0], "Preview crossed world ownership");
            Check(first.isDirty && game.isDirty, "Preview did not dirty changed scenes");
            Check(loaded.SequenceEqual(EditorSceneManager.GetSceneManagerSetup().Select(x => x.path + "|" + x.isActive + "|" + x.isLoaded)), "Preview changed scene setup");
            Undo.FlushUndoRecordObjects(); Undo.PerformUndo();
            Check(RenderSettings.fogColor == firstFog && Lightmapping.GetLightingSettingsForScene(first) == priorBake, "Undo missed A fog/bake settings");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(second);
            Check(RenderSettings.fogColor == secondFog && Lightmapping.GetLightingSettingsForScene(second) == null, "Undo missed B fog/null bake settings");
            Check(rigs.Select(x => x.profile).SequenceEqual(originalProfiles) && localRig.profile == b.combat && otherRig.profile == a.combat, "Undo missed combat profiles");
            Undo.PerformRedo();
            Check(RenderSettings.fogColor == b.lighting.fogColor && Lightmapping.GetLightingSettingsForScene(second) == b.lighting.baking, "Redo missed B fog/bake settings");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(first);
            Check(RenderSettings.fogColor == a.lighting.fogColor && Lightmapping.GetLightingSettingsForScene(first) == a.lighting.baking, "Redo missed A fog/bake settings");
            Check(rigs.All(x => x.profile == a.combat) && localRig.profile == a.combat && otherRig.profile == b.combat, "Redo missed combat profiles");
            gate.worldDefinition = b;
            WorldLightingAuthoring.PreviewLoaded(); Undo.FlushUndoRecordObjects();
            Check(rigs.All(x => x.profile == b.combat) && localRig.profile == a.combat, "Switching gate to B leaked another world's combat profile");
            Undo.PerformUndo(); Check(rigs.All(x => x.profile == a.combat), "Undo missed gate-selected rig change");
            Undo.PerformRedo(); Check(rigs.All(x => x.profile == b.combat), "Redo missed gate-selected rig change");
            var bake = a.lighting.baking; a.lighting.baking = null;
            WorldLightingAuthoring.PreviewLoaded();
            Check(Lightmapping.GetLightingSettingsForScene(first) == bake, "Null profile bake changed existing scene bake settings");
            a.lighting.baking = bake;
            foreach (var scene in new[] { first, second, game }) EditorSceneManager.SaveScene(scene);
            EditorSceneManager.OpenScene(gamePath);
            var reopened = UnityEngine.Object.FindObjectsOfType<DungeonSessionBindings>(true).Single(x => x.worldDefinition == b && x.roots.Any(y => y && y.GetComponentsInChildren<PlayerHealth>(true).Length > 0));
            Check(reopened.roots.SelectMany(x => x.GetComponentsInChildren<ClashLightingRig>(true)).All(x => x.profile == b.combat), "Save/reopen lost gameplay rig profile or prefab overrides");
            var reopenedPrefab = UnityEngine.Object.FindObjectsOfType<DungeonSessionBindings>(true).Single(x => x.roots.Length == 2 && x.roots[0] == x.roots[1]).roots[0].GetComponent<ClashLightingRig>();
            Check(PrefabUtility.IsPartOfPrefabInstance(reopenedPrefab) && reopenedPrefab.profile == b.combat, "Save/reopen lost actual prefab profile override");
            EditorSceneManager.OpenScene(a.scenePath);
            Check(Lightmapping.GetLightingSettingsForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()) == bake, "Save/reopen lost A bake settings");
            EditorSceneManager.OpenScene(b.scenePath);
            Check(Lightmapping.GetLightingSettingsForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()) == b.lighting.baking, "Save/reopen lost B bake settings");
            Check(authored.SequenceEqual(styles.Select(EditorJsonUtility.ToJson)), "Preview mutated authored assets");
            if (failures.Count != 0) throw new InvalidOperationException(string.Join("; ", failures));
            Debug.Log("PASS: split/multiple-world combat Preview, grouped Undo/Redo, null bake, scene setup and save/reopen");
        }
        finally
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(folder);
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
    public static void VerifyPreviewAndRepeatBuilder()
    {
        VerifyPreviewBindingsAndUndo();
        VerifyPreviewScenes();
        var scenes = EditorBuildSettings.scenes;
        try
        {
            EditorBuildSettings.scenes = scenes.Where(x => x.path != LightingFixtureBuild.ScenePath)
                .Concat(new[] { new EditorBuildSettingsScene(LightingFixtureBuild.ScenePath, true) }).ToArray();
            PixelUIBuild.SerializeOnly();
            Debug.Log("PASS: Preview regressions and strict repeated builder with both selected worlds");
        }
        finally { EditorBuildSettings.scenes = scenes; }
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
