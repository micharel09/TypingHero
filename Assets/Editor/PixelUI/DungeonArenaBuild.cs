using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Canonical gameplay integration. Room meshes/materials are authored assets.
internal static class DungeonArenaBuild
{
    internal const string Art = "Assets/Art/Environment/KayKitDungeon/";
    internal static void Apply(Pose cameraPose)
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var world = Object.FindObjectOfType<DungeonWorld>(true);
        if (!world) throw new System.InvalidOperationException("Missing authored world");
        WorldLightingAuthoring.Validate(world);
        var p = world.Profile; var camera = world.view;
        // Legacy UI passes may reposition Camera.main; retain the world authored pose.
        camera.transform.SetPositionAndRotation(cameraPose.position, cameraPose.rotation);
        var layout = Object.FindObjectOfType<CombatHUDLayout>(true);
        var position = camera.transform.position;
        var settings = new SerializedObject(world.Arena);
        settings.FindProperty("framingLayout").objectReferenceValue = layout; settings.ApplyModifiedPropertiesWithoutUndo();
        foreach (var vcam in Object.FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>(true))
        {
            vcam.transform.SetPositionAndRotation(position, camera.transform.rotation);
            vcam.m_Lens.ModeOverride = Cinemachine.LensSettings.OverrideModes.Perspective;
            vcam.m_Lens.OrthographicSize = p.normalOrtho; vcam.m_Lens.FieldOfView = p.fieldOfView;
            vcam.m_Lens.NearClipPlane = p.nearClip; vcam.m_Lens.FarClipPlane = p.farClip;
        }
        foreach (var viewport in Object.FindObjectsOfType<CombatViewportCamera>(true))
        {
            var so = new SerializedObject(viewport); so.FindProperty("normalOrtho").floatValue = p.normalOrtho;
            so.FindProperty("anchorFloorDuringZoom").boolValue = false; so.ApplyModifiedPropertiesWithoutUndo();
        }
        var screenFx = Object.FindObjectOfType<SlayerScreenFX>(true);
        screenFx.MigratePresentationDefaults();
        var fx = new SerializedObject(screenFx);
        fx.FindProperty("normalOrtho").floatValue = p.normalOrtho; fx.FindProperty("slayerOrtho").floatValue = p.slayerOrtho;
        fx.FindProperty("environment").objectReferenceValue = world.environment; fx.ApplyModifiedPropertiesWithoutUndo();
        foreach (var actor in new Component[] { Object.FindObjectOfType<PlayerHealth>(true), Object.FindObjectOfType<SkeletonController>(true) })
        foreach (var renderer in actor.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (renderer.name == "ContactShadow") { renderer.enabled = false; continue; }
            if (!renderer.sprite) continue;
            renderer.sharedMaterial = p.actorMaterial; renderer.shadowCastingMode = ShadowCastingMode.TwoSided; renderer.receiveShadows = true;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            EditorUtility.SetDirty(renderer);
            if (PrefabUtility.IsPartOfPrefabInstance(renderer)) PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
        }
        foreach (var old in Object.FindObjectsOfType<CombatR7Backdrop>(true))
        {
            old.enabled = false;
            if (old.TryGetComponent<Renderer>(out var renderer)) renderer.enabled = false;
        }
        WorldLightingAuthoring.Apply(world);
        EditorSceneManager.SaveScene(scene);
    }
}
