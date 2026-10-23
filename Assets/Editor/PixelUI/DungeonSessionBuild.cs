using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

internal static class DungeonSessionBuild
{
    internal static string WorldPath { get; private set; }
    const string GamePath = "Assets/Scenes/SampleScene.unity";
    // Existing passes author a complete room. Reunite it before those passes, then
    // partition the final serialized result once. No room/presenter is cloned.
    internal static Pose RestoreForBuilders()
    {
        var game = EditorSceneManager.OpenScene(GamePath);
        var gate = Object.FindObjectOfType<DungeonSessionBindings>(true);
        if (!gate) throw new InvalidOperationException("Split scene missing binding gate");
        if (!gate.worldDefinition) throw new InvalidOperationException("Gameplay gate requires a world definition");
        WorldPath = gate.worldDefinition.scenePath;
        var worldScene = EditorSceneManager.OpenScene(WorldPath, OpenSceneMode.Additive);
        var world = Object.FindObjectOfType<DungeonWorld>(true);
        var cameraPose = new Pose(world.view.transform.position, world.view.transform.rotation);
        foreach (var root in worldScene.GetRootGameObjects()) SceneManager.MoveGameObjectToScene(root, game);
        gate.Bind(world);
        foreach (var link in gate.links)
        {
            var owner = link.worldOwnsField ? world.Resolve(link.slot) : link.local;
            EditorUtility.SetDirty(owner);
            if (PrefabUtility.IsPartOfPrefabInstance(owner)) PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
        }
        foreach (Transform child in gate.transform.Cast<Transform>().ToArray()) child.SetParent(null, true);
        gate.gameObject.SetActive(true);
        EditorSceneManager.CloseScene(worldScene, true);
        EditorSceneManager.SaveScene(game);
        return cameraPose;
    }
    internal static void Apply()
    {
        var game = EditorSceneManager.OpenScene(GamePath);
        var gate = Object.FindObjectOfType<DungeonSessionBindings>(true);
        var world = Object.FindObjectOfType<DungeonWorld>(true);
        if (!gate || !world || !world.environment || !world.view)
            throw new InvalidOperationException("Authored world/session bindings missing");
        foreach (var root in game.GetRootGameObjects())
        {
            foreach (var actor in root.GetComponentsInChildren<PlayerHealth>(true))
                SpriteDepthAuthoring.Attach(actor.gameObject);
            foreach (var actor in root.GetComponentsInChildren<SkeletonController>(true))
                SpriteDepthAuthoring.Attach(actor.gameObject);
        }
        var environment = world.environment.gameObject;
        var camera = world.view;
        WorldPath = gate.worldDefinition.scenePath;
        var cinematic = new SerializedObject(Object.FindObjectOfType<SlayerScreenFX>(true));
        var combat = cinematic.FindProperty("combatLighting").objectReferenceValue as GameObject;
        if (!combat || !combat.GetComponent<ClashLightingRig>())
            throw new InvalidOperationException("Serialized combat lighting binding missing");
        var backgroundCamera = GameObject.Find("PixelBackgroundCamera");
        if (backgroundCamera) backgroundCamera.GetComponent<Camera>().enabled = false;
        var roots = new[] { environment, camera.gameObject, world.gameObject };
        var worldSet = new HashSet<Object>();
        foreach (var root in roots)
        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            worldSet.Add(transform.gameObject);
            foreach (var component in transform.GetComponents<Component>()) if (component) worldSet.Add(component);
        }
        // Scene root order changes when reuniting additive scenes. Serialize IDs
        // first, then order links by stable local ID rather than traversal order.
        EditorSceneManager.SaveScene(game);
        var objects = new List<Object>();
        var links = new List<DungeonSessionBindings.Link>();
        foreach (var component in game.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Component>(true))
            .Where(component => component)
            .OrderBy(component => GlobalObjectId.GetGlobalObjectIdSlow(component).targetObjectId))
        {
            if (!component || component == world || component == gate) continue;
            var serialized = new SerializedObject(component);
            var property = serialized.GetIterator();
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                var value = property.objectReferenceValue;
                if (!value || EditorUtility.IsPersistent(value)) continue;
                if (!(value is GameObject) && !(value is Component)) continue;
                bool owns = worldSet.Contains(component), refers = worldSet.Contains(value);
                if (owns == refers) continue;
                if (!(component is MonoBehaviour) && !(component is Canvas && property.propertyPath == "m_Camera"))
                    throw new InvalidOperationException("Unsupported native cross-scene binding: " + component + "." + property.propertyPath);
                if (property.propertyPath.Contains("."))
                    throw new InvalidOperationException("Unsupported nested cross-scene binding: " + component + "." + property.propertyPath);
                var item = owns ? (Object)component : value;
                var slot = world.SlotFor(item);
                links.Add(new DungeonSessionBindings.Link { local = owns ? value : component,
                    field = property.propertyPath, slot = slot, worldOwnsField = owns });
                property.objectReferenceValue = null;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
        gate.links = links.ToArray();
        var worldScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        // Copy authored fog/ambient/lighting via the scene settings before moving roots.
        var previousActive = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(game);
        var settings = Unsupported.GetRenderSettings();
        var settingsJson = EditorJsonUtility.ToJson(settings);
        SceneManager.SetActiveScene(worldScene);
        EditorJsonUtility.FromJsonOverwrite(settingsJson, Unsupported.GetRenderSettings());
        SceneManager.SetActiveScene(previousActive);
        foreach (var root in roots) SceneManager.MoveGameObjectToScene(root, worldScene);
        gate.roots = game.GetRootGameObjects().Where(root => root != gate.gameObject).ToArray();
        foreach (var root in gate.roots) root.transform.SetParent(gate.transform, true);
        gate.gameObject.SetActive(false);
        EditorSceneManager.SaveScene(worldScene, WorldPath);
        EditorSceneManager.SaveScene(game);
        EditorSceneManager.CloseScene(worldScene, true);
        var scenes = EditorBuildSettings.scenes.ToList();
        if (!scenes.Any(scene => scene.path == WorldPath)) scenes.Add(new EditorBuildSettingsScene(WorldPath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        var menu = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        foreach (var menuCamera in Object.FindObjectsOfType<Camera>(true)) Object.DestroyImmediate(menuCamera.gameObject);
        foreach (var backdrop in Object.FindObjectsOfType<WorldMenuBackdrop>(true)) backdrop.gameObject.SetActive(false);
        foreach (var lighting in Object.FindObjectsOfType<ClashLightingRig>(true))
        {
            foreach (var light in lighting.GetComponentsInChildren<UnityEngine.Rendering.Universal.Light2D>(true))
            {
                light.enabled = false;
                PrefabUtility.RecordPrefabInstancePropertyModifications(light);
            }
            lighting.gameObject.SetActive(false);
            PrefabUtility.RecordPrefabInstancePropertyModifications(lighting.gameObject);
        }
        var canvas = GameObject.Find("PixelUI");
        if (canvas && canvas.TryGetComponent<UnityEngine.UI.Image>(out var image)) image.enabled = false;
        var background = GameObject.Find("PixelUI/DungeonMenu");
        if (background) background.SetActive(false);
        EditorSceneManager.SaveScene(menu);
    }
}
