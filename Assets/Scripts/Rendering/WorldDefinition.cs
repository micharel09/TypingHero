using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/World Definition")]
public sealed class WorldDefinition : ScriptableObject
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset scene;
#endif
    [HideInInspector] public string scenePath;
    public WorldLightingProfile lighting;
    public CombatLightingProfile combat;
    public CombatCameraProfile cameraImpact;
    public void Validate()
    {
        if (string.IsNullOrEmpty(scenePath) || !lighting || !combat || !cameraImpact) throw new InvalidOperationException(name + ": missing world scene/profiles");
        lighting.Validate(); combat.Validate(); cameraImpact.Validate();
    }
#if UNITY_EDITOR
    void OnValidate() { if (scene) scenePath = UnityEditor.AssetDatabase.GetAssetPath(scene); }
#endif
}
