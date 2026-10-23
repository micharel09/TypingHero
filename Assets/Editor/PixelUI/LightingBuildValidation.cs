using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public sealed class LightingBuildValidation : IPreprocessBuildWithReport
{
    public int callbackOrder => -2000;
    public void OnPreprocessBuild(BuildReport report)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:WorldDefinition", new[] { "Assets/Data/Lighting" }))
        {
            var definition = AssetDatabase.LoadAssetAtPath<WorldDefinition>(AssetDatabase.GUIDToAssetPath(guid));
            definition.Validate();
            if (!definition.scene || AssetDatabase.GetAssetPath(definition.scene) != definition.scenePath ||
                !EditorBuildSettings.scenes.Any(x => x.enabled && x.path == definition.scenePath))
                throw new BuildFailedException(definition.name + ": select the current world scene and enable it in Build Settings");
        }
    }
}
