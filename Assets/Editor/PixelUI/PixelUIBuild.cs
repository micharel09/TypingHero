using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Use the existing cloud Unity builder. Migration also runs from the editor menu.
public sealed class PixelUIBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report) => Apply();

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
            AssetDatabase.SaveAssets();
            // Return the actual Unity-serialized assets alongside the WebGL artifact.
            Directory.CreateDirectory("build/PixelUI");
            AssetDatabase.ExportPackage(new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity",
                PixelUIFactory.Generated, PixelUIFactory.Art }, "build/PixelUI/PixelUI.unitypackage",
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
