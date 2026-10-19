using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Use the existing cloud Unity builder. Migration also runs from the editor menu.
public sealed class PixelUIBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        try { Apply(); }
        catch (System.Exception error) { throw new BuildFailedException(error); }
        if (System.Environment.GetEnvironmentVariable("TYPINGHERO_QA") != "1") return;
        var paths = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity",
            PixelUIFactory.Generated + "Panel.prefab", PixelUIFactory.Generated + "Button.prefab",
            PixelUIFactory.Generated + "StatBar.prefab", PixelUIFactory.Generated + "LeaderboardRow.prefab",
            "Assets/Characters/Player/Player.prefab", "Assets/Characters/Enermy/Skeleton.prefab",
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_normal.asset",
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_slayer.asset",
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_lethal.asset",
            "Assets/Audio/SFX/Events/Combat/Player/Parry/ev_parry_clang.asset" };
        paths = paths.Concat(Directory.GetFiles(CombatFeedbackAssets.Generated, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Art/VFX/CodeManu", "*"))
            .Concat(Directory.GetFiles(PixelSparkAssets.Root, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(ParryClashAssets.Root, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(WorldLightingBuild.Root, "*", SearchOption.AllDirectories)).ToArray();
        paths = paths.Concat(Directory.GetFiles(ShogunEnvironment.Art, "*"))
            .Concat(Directory.GetFiles("Assets/Audio/SFX/Clips/KenneyUI", "*"))
            .Concat(Directory.GetFiles(ShogunTheme.Events, "*")).ToArray();
        var before = System.Array.ConvertAll(paths, File.ReadAllBytes);
        try { Apply(); }
        catch (System.Exception error) { throw new BuildFailedException(error); }
        for (int i = 0; i < paths.Length; i++)
            if (!before[i].SequenceEqual(File.ReadAllBytes(paths[i])))
                throw new BuildFailedException("UI builder is not idempotent: " + paths[i]);
        Debug.Log("Shogun repeat builder: serialized objects, references and listeners unchanged");
    }

    public static void SerializeOnly()
    {
        Apply();
        Debug.Log("Build succeeded!");
    }

    [MenuItem("Typing Hero/Apply Pixel UI")]
    public static void Apply()
    {
        var previousScene = EditorSceneManager.GetActiveScene().path;
        try
        {
            PixelUIAssets.Prepare();
            PixelUIAssets.SavePrimitives();
            PixelUIScenes.Menu();
            PixelUIScenes.Game();
            CoreMatchBuild.Apply();
            CombatFeedbackBuild.Apply();
            TutorialBuild.Apply();
            ShogunTheme.Apply();
            WorldLightingBuild.Apply();
            AssetDatabase.SaveAssets();
            // Return the actual Unity-serialized assets alongside the WebGL artifact.
            Directory.CreateDirectory("build/PixelUI");
            AssetDatabase.ExportPackage(new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity",
                PixelUIFactory.Generated.TrimEnd('/'), PixelUIFactory.Art.TrimEnd('/'),
                ShogunEnvironment.Art.TrimEnd('/'), "Assets/Audio/SFX/Clips/KenneyUI", ShogunTheme.Events.TrimEnd('/') }, "build/PixelUI/PixelUI.unitypackage",
                ExportPackageOptions.Recurse);
            Debug.Log("Pixel UI migration and serialization complete");
        }
        finally
        {
            if (!string.IsNullOrEmpty(previousScene) && File.Exists(previousScene))
                EditorSceneManager.OpenScene(previousScene);
        }
    }
}
