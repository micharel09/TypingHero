using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Applies authored data to explicit scene bindings. Never writes style assets.
public static class WorldLightingAuthoring
{
    // Unity 2022.3 keeps the scene's lighting-settings reference on this native object.
    static readonly System.Reflection.MethodInfo GetLightmapSettings = typeof(LightmapEditorSettings)
        .GetMethod("GetLightmapSettings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
    static ClashLightingRig[] CombatRigs(DungeonWorld world) => UnityEngine.Object.FindObjectsOfType<ClashLightingRig>(true)
        .Where(x => x.gameObject.scene == world.gameObject.scene)
        .Concat(UnityEngine.Object.FindObjectsOfType<DungeonSessionBindings>(true)
            .Where(x => x.worldDefinition == world.definition)
            .SelectMany(x => x.roots).Where(x => x)
            .SelectMany(x => x.GetComponentsInChildren<ClashLightingRig>(true)))
        .Distinct().ToArray();
    [MenuItem("CLACKBLADE/Lighting/Validate Loaded Worlds")]
    public static void ValidateLoaded()
    {
        foreach (var world in UnityEngine.Object.FindObjectsOfType<DungeonWorld>(true)) Validate(world);
        Debug.Log("Loaded world lighting profiles valid");
    }
    public static void Validate(DungeonWorld world)
    {
        if (!world.definition || !world.environment || !world.view || !world.Arena)
            throw new InvalidOperationException(world.name + ": incomplete world bindings");
        world.definition.Validate();
        var p = world.Profile;
        if (GraphicsSettings.currentRenderPipeline != p.pipeline)
            throw new InvalidOperationException(p.name + ": select its URP asset in Graphics/Quality Settings");
        var renderers = new SerializedObject(p.pipeline).FindProperty("m_RendererDataList");
        if (p.worldRendererIndex < 0 || p.worldRendererIndex >= renderers.arraySize ||
            renderers.GetArrayElementAtIndex(p.worldRendererIndex).objectReferenceValue != p.worldRenderer ||
            p.presentationRendererIndex < 0 || p.presentationRendererIndex >= renderers.arraySize ||
            renderers.GetArrayElementAtIndex(p.presentationRendererIndex).objectReferenceValue != p.presentationRenderer)
            throw new InvalidOperationException(p.name + ": renderer references do not match the pipeline slots");
        foreach (var torch in world.environment.GetComponentsInChildren<DungeonTorchLight>(true))
        {
            if (!torch.profile) throw new InvalidOperationException(torch.name + ": missing LightProfile");
            torch.profile.Validate();
            if (torch.profile.flame && !torch.Flame) throw new InvalidOperationException(torch.name + ": missing flame anchor/mesh filter");
        }
    }
    [MenuItem("CLACKBLADE/Lighting/Apply Preview to Loaded Worlds")]
    public static void PreviewLoaded()
    {
        if (Application.isPlaying) throw new InvalidOperationException("Stop Play Mode before applying authored scene changes");
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Apply lighting profiles");
        try
        {
            foreach (var world in UnityEngine.Object.FindObjectsOfType<DungeonWorld>(true))
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(world.gameObject.scene);
                var objects = world.environment.GetComponentsInChildren<Component>(true).Where(x => x).Cast<UnityEngine.Object>()
                    .Concat(CombatRigs(world).Cast<UnityEngine.Object>())
                    .Concat(new UnityEngine.Object[] { world, world.view, world.view.transform, world.Arena, world.view.GetUniversalAdditionalCameraData(),
                        Unsupported.GetRenderSettings(), (UnityEngine.Object)GetLightmapSettings.Invoke(null, null) }).Distinct().ToArray();
                Undo.RecordObjects(objects, "Apply lighting profiles");
                Apply(world);
                EditorSceneManager.MarkSceneDirty(world.gameObject.scene);
            }
        }
        finally
        {
            Undo.CollapseUndoOperations(undoGroup);
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
        }
    }

    public static void Apply(DungeonWorld world)
    {
        Validate(world);
        var p = world.Profile; var view = world.view;
        world.ApplyEnvironment();
        world.Arena.profile = p;
        view.allowHDR = true; view.allowMSAA = false;
        view.orthographic = false; view.orthographicSize = p.normalOrtho; view.fieldOfView = p.fieldOfView;
        view.nearClipPlane = p.nearClip; view.farClipPlane = p.farClip;
        view.aspect = (float)p.resolution.x / p.resolution.y;
        view.backgroundColor = p.background; view.clearFlags = CameraClearFlags.SolidColor;
        var cameraData = view.GetUniversalAdditionalCameraData(); cameraData.SetRenderer(p.worldRendererIndex);
        cameraData.renderPostProcessing = false; cameraData.antialiasing = AntialiasingMode.None;
        var volume = world.environment.GetComponent<Volume>();
        if (volume) volume.sharedProfile = p.post;
        if (p.baking) Lightmapping.lightingSettings = p.baking;
        foreach (var motion in world.environment.GetComponentsInChildren<DungeonEnvironmentMotion>(true))
        { motion.profile = p; if (motion.RuneLight && p.rune) p.rune.Apply(motion.RuneLight); }
        foreach (var torch in world.environment.GetComponentsInChildren<DungeonTorchLight>(true))
        {
            torch.Preview();
            var data = new SerializedObject(torch.GetComponent<Light>().GetUniversalAdditionalLightData());
            data.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue = torch.profile.shadowResolutionTier;
            data.ApplyModifiedPropertiesWithoutUndo();
            if (!torch.Flame || !torch.profile.flame) continue;
            var node = torch.Flame.transform; var flame = torch.profile.flame;
            float depth = Vector3.Dot(node.position - view.transform.position, view.transform.forward);
            float height = 2 * depth * Mathf.Tan(view.fieldOfView * .5f * Mathf.Deg2Rad) * torch.profile.projectedFlameHeight /
                (p.resolution.y * Vector3.Dot(Vector3.up, view.transform.up));
            float parentScale = node.parent ? node.parent.lossyScale.y : 1;
            node.localScale = Vector3.one * (height / flame.contentHeightRatio / flame.mesh.bounds.size.y / parentScale);
            EditorUtility.SetDirty(node); EditorUtility.SetDirty(torch);
        }
        foreach (var rig in CombatRigs(world))
            if (rig.profile != world.definition.combat)
            {
                rig.profile = world.definition.combat;
                if (PrefabUtility.IsPartOfPrefabInstance(rig)) PrefabUtility.RecordPrefabInstancePropertyModifications(rig);
                EditorUtility.SetDirty(rig); EditorSceneManager.MarkSceneDirty(rig.gameObject.scene);
            }
        foreach (var component in world.environment.GetComponentsInChildren<Component>(true))
            if (component && PrefabUtility.IsPartOfPrefabInstance(component))
            { PrefabUtility.RecordPrefabInstancePropertyModifications(component); EditorUtility.SetDirty(component); }
        EditorUtility.SetDirty(world.Arena); EditorUtility.SetDirty(view);
    }
}
