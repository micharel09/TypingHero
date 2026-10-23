using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CameraImpactAuthoringChecks
{
    static UnityEngine.Object[] retained;
    public static void VerifyIntegrated() { Verify(); LightingAuthoringChecks.PrepareAndVerify(); }
    public static void Verify()
    {
        var world = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
        var other = AssetDatabase.LoadAssetAtPath<WorldDefinition>(LightingFixtureBuild.DefinitionPath);
        world.Validate(); other.Validate();
        var profile = world.cameraImpact;
        retained = new UnityEngine.Object[] { world, other, profile };
        string before = EditorJsonUtility.ToJson(profile);
        string path = AssetDatabase.GetAssetPath(profile);
        try
        {
            profile.presets.Single(x => x.trigger == CombatCameraProfile.Event.Normal).zoomPeak = .027f;
            EditorUtility.SetDirty(profile); AssetDatabase.SaveAssets();
            var edited = File.ReadAllBytes(path);
            for (int pass = 0; pass < 2; pass++)
            {
                PixelUIBuild.Apply();
                if (!edited.SequenceEqual(File.ReadAllBytes(path))) throw new InvalidOperationException("Builder overwrote edited camera profile");
                EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
                var impacts = UnityEngine.Object.FindObjectsOfType<CombatCameraImpact>(true);
                var extensions = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).Where(x => x.GetType().Name == "CameraImpactExtension").ToArray();
                if (impacts.Length != 1 || extensions.Length != 1 ||
                    new SerializedObject(extensions[0]).FindProperty("impact").objectReferenceValue != impacts[0])
                    throw new InvalidOperationException("Camera migration duplicated or disconnected components");
                if (impacts[0].GetComponents<MonoBehaviour>().Any(x => x.GetType().Name == "CinemachineImpulseListener" && new SerializedObject(x).FindProperty("m_ChannelMask").intValue == 2))
                    throw new InvalidOperationException("Legacy combat listener still active");
            }
            if (other.cameraImpact == profile || other.lighting.resolution != new Vector2Int(256, 144))
                throw new InvalidOperationException("Independent camera fixture missing");
            Debug.Log("PASS camera authoring: edited presets survive two builders, unique connected extension/controller, independent 256x144 fixture");
        }
        finally
        {
            profile = AssetDatabase.LoadAssetAtPath<CombatCameraProfile>(path);
            EditorJsonUtility.FromJsonOverwrite(before, profile); EditorUtility.SetDirty(profile); AssetDatabase.SaveAssets();
            PixelUIBuild.Apply(); retained = null;
        }
    }
}
