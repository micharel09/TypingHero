using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Baking is an explicit authoring action using the selected world's native settings.
public static class DungeonLightingBake
{
    [MenuItem("CLACKBLADE/Lighting/Bake Loaded World")]
    public static void Bake()
    {
        var world = Object.FindObjectOfType<DungeonWorld>(true);
        if (!world || !world.Profile.baking) throw new System.InvalidOperationException("Open a world with authored LightingSettings before baking");
        WorldLightingAuthoring.Validate(world);
        Lightmapping.lightingSettings = world.Profile.baking;
        if (!Lightmapping.Bake()) throw new System.InvalidOperationException("World lighting bake failed");
        EditorSceneManager.SaveScene(world.gameObject.scene);
    }
}
