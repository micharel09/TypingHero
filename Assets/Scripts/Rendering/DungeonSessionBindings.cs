using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Build-authored links; scene assets never retain references into another scene.
[Preserve]
[DefaultExecutionOrder(-32000)]
public sealed class DungeonSessionBindings : MonoBehaviour
{
    [Serializable] public struct Link
    {
        public UnityEngine.Object local;
        public string field;
        [UnityEngine.Serialization.FormerlySerializedAs("worldIndex")] public DungeonWorld.Slot slot;
        public bool worldOwnsField;
    }
    public WorldDefinition worldDefinition;
    public Link[] links = Array.Empty<Link>();
    public GameObject[] roots = Array.Empty<GameObject>();
    void Awake()
    {
        // Activation is atomic under this inactive gate. Detach before gameplay
        // Awake: combat uses Transform.root to distinguish the two actors.
        foreach (var root in roots) root.transform.SetParent(null, true);
    }
    public void Deactivate()
    {
        foreach (var root in roots)
            if (root && root.scene == gameObject.scene) root.SetActive(false);
        gameObject.SetActive(false);
    }
    public void Bind(DungeonWorld world, bool clear = false)
    {
        if (!clear)
        foreach (var root in roots)
        {
            foreach (var camera in root.GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>(true))
            {
                var p = world.Profile;
                camera.transform.SetPositionAndRotation(world.view.transform.position, world.view.transform.rotation);
                camera.m_Lens.ModeOverride = Cinemachine.LensSettings.OverrideModes.Perspective;
                camera.m_Lens.OrthographicSize = p.normalOrtho; camera.m_Lens.FieldOfView = p.fieldOfView;
                camera.m_Lens.NearClipPlane = p.nearClip; camera.m_Lens.FarClipPlane = p.farClip;
            }
            foreach (var impact in root.GetComponentsInChildren<CombatCameraImpact>(true)) impact.Configure(world.Profile.cameraImpact);
            foreach (var viewport in root.GetComponentsInChildren<CombatViewportCamera>(true)) viewport.lighting = world.Profile;
            foreach (var fx in root.GetComponentsInChildren<SlayerScreenFX>(true)) fx.lighting = world.Profile;
            foreach (var rig in root.GetComponentsInChildren<ClashLightingRig>(true)) rig.profile = world.definition.combat;
            foreach (var actor in root.GetComponentsInChildren<PlayerHealth>(true)) ApplyActorMaterial(actor, world.Profile);
            foreach (var actor in root.GetComponentsInChildren<SkeletonController>(true)) ApplyActorMaterial(actor, world.Profile);
        }
        foreach (var link in links)
        {
            var owner = link.worldOwnsField ? world.Resolve(link.slot) as Component : link.local as Component;
            // Death cleanup may already have removed a gameplay-side owner.
            if (!owner && clear && !link.worldOwnsField) continue;
            if (!owner) throw new InvalidOperationException("Missing dungeon binding owner: " + link.field);
            if (owner is Canvas canvas && link.field == "m_Camera")
            {
                canvas.worldCamera = clear ? null : world.Resolve(link.slot) as Camera;
                continue;
            }
            var field = owner.GetType().GetField(link.field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null) throw new InvalidOperationException("Missing dungeon binding field: " + link.field);
            field.SetValue(owner, clear ? null : link.worldOwnsField ? link.local : world.Resolve(link.slot));
        }
    }
    static void ApplyActorMaterial(Component actor, WorldLightingProfile profile)
    {
        foreach (var renderer in actor.GetComponentsInChildren<SpriteRenderer>(true))
            if (renderer.name != "ContactShadow") renderer.sharedMaterial = profile.actorMaterial;
    }

}
