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
        foreach (var world in UnityEngine.Object.FindObjectsOfType<DungeonWorld>(true))
        {
            var objects = world.environment.GetComponentsInChildren<Component>(true).Where(x => x).Cast<UnityEngine.Object>()
                .Concat(new UnityEngine.Object[] { world, world.view, world.view.transform, world.Arena }).ToArray();
            Undo.RecordObjects(objects, "Apply lighting profiles");
            Apply(world);
            EditorSceneManager.MarkSceneDirty(world.gameObject.scene);
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
        foreach (var rig in UnityEngine.Object.FindObjectsOfType<ClashLightingRig>(true))
            if (rig.gameObject.scene == world.gameObject.scene || rig.GetComponentInParent<DungeonSessionBindings>(true))
            { rig.profile = world.definition.combat; EditorUtility.SetDirty(rig); }
        EditorUtility.SetDirty(world.Arena); EditorUtility.SetDirty(view);
    }
}
