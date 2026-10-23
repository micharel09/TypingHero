using Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

internal static class CombatCameraImpactBuild
{
    internal static void Apply()
    {
        var camera = Object.FindObjectOfType<CinemachineVirtualCamera>(true);
        var arena = Object.FindObjectOfType<DungeonArenaCamera>(true);
        var shake = Object.FindObjectOfType<ParryCameraShake>(true);
        var health = Object.FindObjectOfType<PlayerHealth>(true);
        if (!camera || !arena || !shake || !health)
            throw new System.InvalidOperationException("Camera impact authoring references missing");
        var impact = camera.GetComponent<CombatCameraImpact>();
        if (!impact) impact = camera.gameObject.AddComponent<CombatCameraImpact>();
        var world = Object.FindObjectOfType<DungeonWorld>(true);
        if (!world || !world.definition) throw new System.InvalidOperationException("Camera impact world definition missing");
        world.definition.Validate();
        // Assets are authored migration results. Never replace user-edited preset data.
        foreach (var listener in camera.GetComponents<CinemachineImpulseListener>())
            if (listener.m_ChannelMask == 2) Object.DestroyImmediate(listener);
        var extension = camera.GetComponent<CameraImpactExtension>();
        if (!extension) extension = camera.gameObject.AddComponent<CameraImpactExtension>();
        Set(extension, "impact", impact);
        Set(impact, "arena", arena); Set(impact, "health", health);
        Set(arena, "impact", impact); Set(shake, "cameraImpact", impact);
        foreach (var box in Object.FindObjectsOfType<PlayerWeaponHitbox>(true)) Set(box, "cameraImpact", impact);
        foreach (var fx in Object.FindObjectsOfType<SlayerScreenFX>(true)) Set(fx, "cameraImpact", impact);
        EditorSceneManager.SaveScene(camera.gameObject.scene);
    }
    static void Set(Object owner, string field, Object value)
    {
        var serialized = new SerializedObject(owner);
        serialized.FindProperty(field).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(owner);
        PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
    }
}
