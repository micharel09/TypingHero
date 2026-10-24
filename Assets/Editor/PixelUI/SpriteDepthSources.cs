using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only source discovery. Scene instances own overrides; prefabs alone cannot describe them.
internal sealed class SpriteDepthSources
{
    internal readonly HashSet<string> paths = new HashSet<string>(StringComparer.Ordinal);
    internal readonly HashSet<Sprite> sprites = new HashSet<Sprite>();

    internal static SpriteDepthSources Collect()
    {
        var result = new SpriteDepthSources();
        var dependencies = LightingBuildValidation.Dependencies();
        foreach (var prefab in dependencies.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(x => x))
            result.ReadActors(prefab);
        // These opponents are selected by Resources path rather than serialized scene references.
        foreach (var role in Enum.GetValues(typeof(EndlessRole)).Cast<EndlessRole>())
            result.AddAsset(Resources.Load<EndlessOpponent>("Endless/" + role));
        foreach (var path in dependencies.Where(x => x.EndsWith(".unity", StringComparison.Ordinal)).Distinct())
        {
            var scene = SceneManager.GetSceneByPath(path);
            if (scene.IsValid() && scene.isLoaded) result.ReadScene(scene);
            else
            {
                // Opening saved preview scenes is not a public API in Unity 2022.3.
                // Use a temporary additive scene; preserve any existing unloaded entry.
                bool existingEntry = scene.IsValid();
                var active = SceneManager.GetActiveScene();
                var temporary = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try { result.ReadScene(temporary); }
                finally
                {
                    EditorSceneManager.CloseScene(temporary, !existingEntry);
                    if (active.IsValid() && active.isLoaded) SceneManager.SetActiveScene(active);
                }
            }
        }
        return result;
    }

    void ReadScene(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects()) ReadActors(root);
    }

    void ReadActors(GameObject root)
    {
        var actors = root.GetComponentsInChildren<PlayerHealth>(true).Cast<Component>()
            .Concat(root.GetComponentsInChildren<SkeletonController>(true));
        var visited = new HashSet<Component>();
        foreach (var actor in actors)
        foreach (var component in actor.GetComponentsInChildren<Component>(true))
            ReadComponent(component, visited);
    }

    void ReadComponent(Component component, HashSet<Component> visited)
    {
        if (!component || !visited.Add(component)) return;
        if (component is SpriteRenderer renderer) AddAsset(renderer.sprite);
        using (var serialized = new SerializedObject(component))
        {
            var property = serialized.GetIterator();
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                var reference = property.objectReferenceValue;
                if (EditorUtility.IsPersistent(reference)) AddAsset(reference);
                // Explicit renderer/animator references can live outside the actor's subtree.
                else if (reference is Animator || reference is SpriteRenderer)
                    ReadComponent((Component)reference, visited);
            }
        }
    }

    void AddAsset(UnityEngine.Object asset)
    {
        if (!asset) return;
        var path = AssetDatabase.GetAssetPath(asset);
        if (!string.IsNullOrEmpty(path)) paths.Add(path);
        if (asset is Sprite sprite) sprites.Add(sprite);
    }
}
