using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

// Scene-local bounded leases. No player, parry, audio, damage, camera or hitstop dependency.
[DisallowMultipleComponent]
public sealed class ImpactVfxEmitter : MonoBehaviour
{
    readonly Dictionary<GameObject, ObjectPool<GameObject>> pools = new();
    internal readonly List<GameObject> active = new(8);
    Transform poolRoot;

    public ImpactVfxBurst Play(ImpactVfxProfile profile, ImpactVfxContext context)
    {
        if (!isActiveAndEnabled || !profile || !profile.prefab || !profile.physics ||
            GamePauseController.InputBlocked || Time.timeScale == 0 || CombatSession.Finished) return null;
        if (!poolRoot)
        {
            poolRoot = new GameObject("ImpactVfxPool").transform;
            SceneManager.MoveGameObjectToScene(poolRoot.gameObject, gameObject.scene);
        }
        var prefab = profile.prefab;
        if (!pools.TryGetValue(prefab, out var pool))
        {
            ObjectPool<GameObject> created = null;
            created = new ObjectPool<GameObject>(() => {
                var go = Instantiate(prefab, poolRoot); go.SetActive(false);
                var lease = go.GetComponent<PooledVFX>();
                if (!lease) lease = go.AddComponent<PooledVFX>();
                lease.Setup(created); return go;
            }, go => { active.Add(go); go.SetActive(true); }, go => {
                active.Remove(go);
                if (go) { go.transform.SetParent(poolRoot, true); go.SetActive(false); }
            }, go => { if (go) Destroy(go); }, false, 8, 8);
            pool = created; pools.Add(prefab, pool);
        }
        if (active.Count >= 8) active[0].GetComponent<PooledVFX>().Release();
        var item = pool.Get();
        var burst = item.GetComponent<ImpactVfxBurst>();
        Play(profile, context, burst);
        return burst;
    }

    // Adapter overload accepts an existing lease, retaining legacy pool ownership.
    internal void Play(ImpactVfxProfile profile, ImpactVfxContext context, ImpactVfxBurst burst,
        ParryContactSnapshot? contact = null)
    {
        var visual = contact ?? new ParryContactSnapshot {
            valid = context.unitsPerMeter > 0, unitsPerMeter = context.unitsPerMeter };
        burst.Emit(context.point, context.direction, context.floorY, ImpactVfxBurst.Preset.BCompact,
            context.seed, visual, context.recipient.valid ? context.recipient.center.x - context.point.x : 0,
            context.recipient, profile);
    }

    public void Clear()
    {
        while (active.Count > 0)
        {
            var go = active[active.Count - 1]; active.RemoveAt(active.Count - 1);
            if (go) go.GetComponent<PooledVFX>().Release();
        }
    }
    void Update() { if (CombatSession.Finished) Clear(); }
    void OnDisable() => Clear();
    void OnDestroy()
    {
        Clear();
        foreach (var pool in pools.Values) pool.Clear();
        pools.Clear();
        if (poolRoot) Destroy(poolRoot.gameObject);
    }
}
