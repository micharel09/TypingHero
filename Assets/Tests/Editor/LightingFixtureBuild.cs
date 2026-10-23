using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Test-only authored world: different names/count/resolution, shared fixture styles.
public static class LightingFixtureBuild
{
    static readonly System.Collections.Generic.List<Object> retained = new System.Collections.Generic.List<Object>();
    public const string ScenePath = "Assets/Tests/Fixtures/Lighting/OtherWorld.unity";
    public const string DefinitionPath = "Assets/Tests/Fixtures/Lighting/OtherWorld.asset";
    public static void Create()
    {
        if (AssetDatabase.LoadAssetAtPath<WorldDefinition>(DefinitionPath)) return;
        System.IO.Directory.CreateDirectory("Assets/Tests/Fixtures/Lighting"); AssetDatabase.Refresh();
        var source = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
        var definition = Object.Instantiate(source); definition.scene = null; definition.scenePath = ScenePath;
        retained.Add(source); retained.Add(definition);
        var profile = Object.Instantiate(source.lighting); retained.Add(profile); profile.fogColor = new Color(.05f, .1f, .2f);
        profile.ambientIntensity = .25f; profile.resolution = new Vector2Int(256, 144);
        AssetDatabase.CreateAsset(profile, "Assets/Tests/Fixtures/Lighting/OtherLighting.asset");
        var camera = Object.Instantiate(source.cameraImpact); retained.Add(camera);
        camera.presets[0].zoomPeak = .01f;
        AssetDatabase.CreateAsset(camera, "Assets/Tests/Fixtures/Lighting/OtherCamera.asset");
        definition.cameraImpact = camera;
        definition.lighting = profile; AssetDatabase.CreateAsset(definition, DefinitionPath);
        var scene = EditorSceneManager.OpenScene(source.scenePath);
        var world = Object.FindObjectOfType<DungeonWorld>(true); world.definition = definition;
        world.view.name = "Alternative Lens"; world.Arena.profile = profile;
        foreach (var motion in world.environment.GetComponentsInChildren<DungeonEnvironmentMotion>(true)) motion.profile = profile;
        int index = 0;
        foreach (var torch in world.environment.GetComponentsInChildren<DungeonTorchLight>(true)) torch.name = "Arbitrary Fixture " + index++;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Data/Lighting/WallTorch.prefab");
        var fourth = (GameObject)PrefabUtility.InstantiatePrefab(prefab, world.environment);
        var alternate = Object.Instantiate(fourth.GetComponentInChildren<DungeonTorchLight>(true).profile);
        alternate.intensity = 12; alternate.amplitude = .04f;
        AssetDatabase.CreateAsset(alternate, "Assets/Tests/Fixtures/Lighting/OtherLight.asset");
        var alternateTorch = fourth.GetComponentInChildren<DungeonTorchLight>(true);
        alternateTorch.profile = alternate;
        PrefabUtility.RecordPrefabInstancePropertyModifications(alternateTorch);
        EditorUtility.SetDirty(alternateTorch);
        fourth.name = "Fourth Reused Fixture"; fourth.transform.localPosition = new Vector3(0, 4, 9);
        EditorSceneManager.SaveScene(scene, ScenePath);
        definition.scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        EditorUtility.SetDirty(definition); AssetDatabase.SaveAssets();
        Debug.Log("Reusable lighting fixture created: four lights, independent world profile");
    }
}
