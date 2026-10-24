using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Only worlds reachable from enabled scenes participate. Unused templates are free to be incomplete.
public sealed class LightingBuildValidation : IPreprocessBuildWithReport
{
    // PixelUIBuild (-1000) updates clips and synchronizes depth metadata first.
    public int callbackOrder => -500;
    internal static WorldDefinition[] SelectedWorlds()
    {
        var paths = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
        return AssetDatabase.GetDependencies(paths, true)
            .Select(AssetDatabase.LoadAssetAtPath<WorldDefinition>).Where(x => x).Distinct().ToArray();
    }
    internal static string[] Dependencies()
    {
        var paths = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path)
            .Concat(SelectedWorlds().Select(AssetDatabase.GetAssetPath)).ToArray();
        return AssetDatabase.GetDependencies(paths, true).Where(System.IO.File.Exists).ToArray();
    }
    public void OnPreprocessBuild(BuildReport report)
    {
        foreach (var definition in SelectedWorlds())
        {
            definition.Validate();
            SpriteDepthAuthoring.Validate(definition.spriteDepth);
            if (!definition.scene || AssetDatabase.GetAssetPath(definition.scene) != definition.scenePath ||
                !EditorBuildSettings.scenes.Any(x => x.enabled && x.path == definition.scenePath))
                throw new BuildFailedException(definition.name + ": select the current world scene and enable it in Build Settings");
        }
    }
    internal static void ApplySelected()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            foreach (var definition in SelectedWorlds())
            {
                var scene = EditorSceneManager.OpenScene(definition.scenePath);
                var worlds = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<DungeonWorld>(true)).ToArray();
                if (worlds.Length != 1 || worlds[0].definition != definition)
                    throw new InvalidOperationException(definition.name + ": scene must bind its selected world definition");
                WorldLightingAuthoring.Apply(worlds[0]);
                EditorSceneManager.SaveScene(scene);
            }
        }
        finally
        {
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
