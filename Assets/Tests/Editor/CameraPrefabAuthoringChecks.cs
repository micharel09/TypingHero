using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class CameraPrefabAuthoringChecks
{
    public static void Verify()
    {
        string original = Environment.GetEnvironmentVariable("TYPINGHERO_QA");
        var setup = EditorSceneManager.GetSceneManagerSetup();
        var scenes = EditorBuildSettings.scenes;
        string SceneSetup() => string.Join(";", EditorSceneManager.GetSceneManagerSetup().Select(x => x.path + "|" + x.isLoaded + "|" + x.isActive));
        string before = SceneSetup();
        try
        {
            foreach (string value in new string[] { null, "0" })
            {
                Environment.SetEnvironmentVariable("TYPINGHERO_QA", value);
                bool rejected = false;
                try { LightingAuthoringChecks.VerifyPreviewAndRepeatBuilder(); }
                catch (InvalidOperationException error) when (error.Message == "Strict repeat-builder requires TYPINGHERO_QA=1.") { rejected = true; }
                if (!rejected || SceneSetup() != before || !scenes.Select(x => x.path + "|" + x.enabled)
                    .SequenceEqual(EditorBuildSettings.scenes.Select(x => x.path + "|" + x.enabled)))
                    throw new InvalidOperationException("QA precondition did not reject before changing scenes/Build Settings");
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable("TYPINGHERO_QA", original);
            if (!scenes.Select(x => x.path + "|" + x.enabled).SequenceEqual(EditorBuildSettings.scenes.Select(x => x.path + "|" + x.enabled)))
                EditorBuildSettings.scenes = scenes;
            if (SceneSetup() != before)
            {
                if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
                else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }
        Debug.Log("PASS: unset/zero QA rejected before scene or Build Settings changes; environment restored");
        VerifyPrefab();
        LightingAuthoringChecks.VerifyPreviewAndRepeatBuilder();
    }
    public static void VerifyPrefab()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        const string prefabPath = "Assets/Data/Lighting/WorldCamera.prefab";
        byte[] prefabBefore = File.ReadAllBytes(prefabPath);
        string folder = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Fixtures/Lighting/CameraPreviewTemporary");
        AssetDatabase.CreateFolder("Assets/Tests/Fixtures/Lighting", Path.GetFileName(folder));
        var failures = new List<string>();
        void Check(bool valid, string message) { if (!valid) { failures.Add(message); Debug.Log("CAMERA PREFAB REGRESSION: " + message); } }
        int RendererIndex(Camera camera) => new SerializedObject(camera.GetUniversalAdditionalCameraData()).FindProperty("m_RendererIndex").intValue;
        WorldLightingProfile profile = null;
        try
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var original = AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Data/Lighting/DungeonWorld.asset");
            var definition = UnityEngine.Object.Instantiate(original);
            profile = UnityEngine.Object.Instantiate(original.lighting);
            AssetDatabase.CreateAsset(profile, folder + "/Lighting.asset");
            definition.lighting = profile; AssetDatabase.CreateAsset(definition, folder + "/World.asset");
            profile.fieldOfView = 47; profile.nearClip = .27f; profile.farClip = 129; profile.normalOrtho = 6.4f; profile.baking = null;
            // Both existing renderer slots stay valid; choose the other slot to expose lost overrides.
            var renderer = profile.worldRenderer; profile.worldRenderer = profile.presentationRenderer; profile.presentationRenderer = renderer;
            int index = profile.worldRendererIndex; profile.worldRendererIndex = profile.presentationRendererIndex; profile.presentationRendererIndex = index;
            string scenePath = folder + "/World.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            definition.scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath); definition.scenePath = scenePath;
            var world = new GameObject("Prefab camera world").AddComponent<DungeonWorld>();
            world.definition = definition; world.environment = new GameObject("Separate environment").transform;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Camera NewCamera() => ((GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene())).GetComponent<Camera>();
            world.view = NewCamera();
            float oldFov = world.view.fieldOfView, oldNear = world.view.nearClipPlane, oldFar = world.view.farClipPlane, oldOrtho = world.view.orthographicSize;
            var oldProfile = world.Arena.profile; int oldRenderer = RendererIndex(world.view);
            if (oldFov == profile.fieldOfView || oldNear == profile.nearClip || oldFar == profile.farClip || oldRenderer == profile.worldRendererIndex || oldProfile == profile)
                throw new InvalidOperationException("Camera fixture must differ from all tested authored values");
            EditorUtility.SetDirty(profile); EditorUtility.SetDirty(definition); AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene);
            string authored = EditorJsonUtility.ToJson(profile);
            void Applied(string phase)
            {
                Check(PrefabUtility.IsPartOfPrefabInstance(world.view) && !world.view.transform.IsChildOf(world.environment), phase + ": expected separate camera prefab instance");
                Check(Mathf.Approximately(world.view.fieldOfView, profile.fieldOfView) && Mathf.Approximately(world.view.nearClipPlane, profile.nearClip) &&
                    Mathf.Approximately(world.view.farClipPlane, profile.farClip) && Mathf.Approximately(world.view.orthographicSize, profile.normalOrtho), phase + ": camera lens lost");
                Check(world.Arena.profile == profile && RendererIndex(world.view) == profile.worldRendererIndex, phase + ": arena profile/renderer override lost");
            }
            WorldLightingAuthoring.PreviewLoaded(); Applied("Preview");
            Undo.FlushUndoRecordObjects(); Undo.PerformUndo();
            Check(Mathf.Approximately(world.view.fieldOfView, oldFov) && Mathf.Approximately(world.view.nearClipPlane, oldNear) && Mathf.Approximately(world.view.farClipPlane, oldFar) &&
                Mathf.Approximately(world.view.orthographicSize, oldOrtho) && world.Arena.profile == oldProfile && RendererIndex(world.view) == oldRenderer, "Undo missed camera components");
            Undo.PerformRedo(); Applied("Redo");
            EditorSceneManager.SaveScene(scene); EditorSceneManager.OpenScene(scenePath);
            world = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true); Applied("Preview save/reopen");
            UnityEngine.Object.DestroyImmediate(world.view.gameObject); world.view = NewCamera();
            WorldLightingAuthoring.Apply(world); Applied("Apply without Undo");
            EditorSceneManager.SaveScene(world.gameObject.scene); EditorSceneManager.OpenScene(scenePath);
            world = UnityEngine.Object.FindObjectOfType<DungeonWorld>(true); Applied("Apply save/reopen");
            Check(authored == EditorJsonUtility.ToJson(profile) && prefabBefore.SequenceEqual(File.ReadAllBytes(prefabPath)), "Authoring changed profile or source prefab");
            if (failures.Count != 0) throw new InvalidOperationException(string.Join("; ", failures));
            Debug.Log("PASS: camera prefab Preview/Undo/Redo and direct Apply persist lens/arena/renderer overrides; source assets unchanged");
        }
        finally
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(folder);
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
}
