using UnityEditor;
using UnityEngine;

public class LightingAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Validate"))
        {
            if (target is WorldDefinition definition) definition.Validate();
            else if (target is WorldLightingProfile world) world.Validate();
            else if (target is CombatLightingProfile combat) combat.Validate();
            else if (target is LightProfile light) light.Validate();
            else if (target is FlameProfile flame) flame.Validate();
            Debug.Log(target.name + ": valid", target);
        }
        using (new EditorGUI.DisabledScope(Application.isPlaying))
            if (GUILayout.Button("Apply / Preview in Loaded Worlds")) WorldLightingAuthoring.PreviewLoaded();
    }
}
[CustomEditor(typeof(WorldDefinition))] public sealed class WorldDefinitionEditor : LightingAssetEditor { }
[CustomEditor(typeof(WorldLightingProfile))] public sealed class WorldLightingProfileEditor : LightingAssetEditor { }
[CustomEditor(typeof(CombatLightingProfile))] public sealed class CombatLightingProfileEditor : LightingAssetEditor { }
[CustomEditor(typeof(LightProfile))] public sealed class LightProfileEditor : LightingAssetEditor { }
[CustomEditor(typeof(FlameProfile))] public sealed class FlameProfileEditor : LightingAssetEditor { }
