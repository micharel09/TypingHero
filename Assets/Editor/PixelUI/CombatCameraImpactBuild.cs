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
        var source = (CinemachineImpulseSource)new SerializedObject(shake).FindProperty("impulseSource").objectReferenceValue;
        if (!source || source.m_ImpulseDefinition.m_ImpulseChannel != 2)
            throw new System.InvalidOperationException("Dedicated ParryRecoil channel missing");
        CinemachineImpulseListener impactListener = null;
        foreach (var listener in camera.GetComponents<CinemachineImpulseListener>())
            if (listener.m_ChannelMask == 2) impactListener = listener;
        if (!impactListener) impactListener = camera.gameObject.AddComponent<CinemachineImpulseListener>();
        impactListener.m_ApplyAfter = CinemachineCore.Stage.Noise;
        impactListener.m_ChannelMask = 2;
        impactListener.m_Gain = 1;
        impactListener.m_UseCameraSpace = false;
        impactListener.m_Use2DDistance = true;
        var settings = new SerializedObject(impactListener);
        settings.FindProperty("m_ReactionSettings.m_AmplitudeGain").floatValue = 0;
        settings.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(impactListener);
        Set(impact, "profile", arena.profile.cameraImpact);
        Set(impact, "arena", arena); Set(impact, "health", health); Set(impact, "recoil", source);
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
