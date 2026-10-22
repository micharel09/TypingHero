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
        Verify(); LightingFixtureBuild.Create();
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(x => x.path != LightingFixtureBuild.ScenePath)
            .Concat(new[] { new EditorBuildSettingsScene(LightingFixtureBuild.ScenePath, true) }).ToArray();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }
    public static void Verify()
    {
        var definition = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
        var light = AssetDatabase.LoadAssetAtPath<LightProfile>("Assets/Data/Lighting/MainLantern.asset");
        var material = light.flame.material;
        definition.lighting.post.TryGet<Bloom>(out var bloom);
        retained = new UnityEngine.Object[] { definition, light, material, bloom };
        string beforeLight = EditorJsonUtility.ToJson(light), beforeMaterial = EditorJsonUtility.ToJson(material), beforeBloom = EditorJsonUtility.ToJson(bloom);
        try
        {
            light.intensity = 19; light.projectedFlameHeight = 9;
            material.SetColor("_FlameOrange", new Color(.4f, .5f, .6f)); bloom.intensity.Override(.14f);
            EditorUtility.SetDirty(light); EditorUtility.SetDirty(material); EditorUtility.SetDirty(bloom); AssetDatabase.SaveAssets();
            var authored = Snapshot();
            PixelUIBuild.Apply(); PixelUIBuild.Apply();
            foreach (var file in authored)
                if (!file.Value.SequenceEqual(File.ReadAllBytes(file.Key))) throw new InvalidOperationException("Builder overwrote authored data: " + file.Key);
            EditorSceneManager.OpenScene(definition.scenePath);
            var world = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true);
            var instance = world.environment.GetComponentsInChildren<DungeonTorchLight>(true).Single(x => x.profile == light);
            if (instance.GetComponent<Light>().intensity != 19) throw new InvalidOperationException("Builder did not apply the edited profile");
            Debug.Log("PASS: edited intensity/size/material/post survive two full builder applications");
        }
        finally
        {
            EditorJsonUtility.FromJsonOverwrite(beforeLight, light); EditorJsonUtility.FromJsonOverwrite(beforeMaterial, material);
            EditorJsonUtility.FromJsonOverwrite(beforeBloom, bloom);
            EditorUtility.SetDirty(light); EditorUtility.SetDirty(material); EditorUtility.SetDirty(bloom); AssetDatabase.SaveAssets();
            PixelUIBuild.Apply();
            retained = null;
        }
    }
}
