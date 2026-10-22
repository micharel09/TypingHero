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
        public int worldIndex;
        public bool worldOwnsField;
    }
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
        foreach (var link in links)
        {
            var owner = link.worldOwnsField ? world.objects[link.worldIndex] as Component : link.local as Component;
            // Death cleanup may already have removed a gameplay-side owner.
            if (!owner && clear && !link.worldOwnsField) continue;
            if (!owner) throw new InvalidOperationException("Missing dungeon binding owner: " + link.field);
            if (owner is Canvas canvas && link.field == "m_Camera")
            {
                canvas.worldCamera = clear ? null : world.objects[link.worldIndex] as Camera;
                continue;
            }
            var field = owner.GetType().GetField(link.field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null) throw new InvalidOperationException("Missing dungeon binding field: " + link.field);
            field.SetValue(owner, clear ? null : link.worldOwnsField ? link.local : world.objects[link.worldIndex]);
        }
    }
}
