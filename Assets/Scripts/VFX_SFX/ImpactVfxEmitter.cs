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
    [SerializeField, Min(1)] int maxActive = 8;
    Transform poolRoot;

    public ImpactVfxBurst Play(ImpactVfxProfile profile, ImpactVfxContext context)
    {
        if (!isActiveAndEnabled || !profile || !profile.prefab || !profile.physics ||
            GamePauseController.InputBlocked || Time.timeScale == 0 || GamePauseController.ResultOverlayVisible ||
            (CombatSession.Finished && context.completionPolicy == ImpactVfxCompletionPolicy.StopOnCombatEnd)) return null;
        var template = profile.prefab.GetComponent<ImpactVfxBurst>();
        if (!template) throw new System.InvalidOperationException(profile.name + ": prefab needs ImpactVfxBurst");
        template.Validate(profile);
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
            }, go => { if (go) Destroy(go); }, false, maxActive, maxActive);
            pool = created; pools.Add(prefab, pool);
        }
        if (active.Count >= maxActive) active[0].GetComponent<PooledVFX>().Release();
        var item = pool.Get();
        var burst = item.GetComponent<ImpactVfxBurst>();
        burst.Play(profile, context);
        return burst;
    }

    public void Clear()
    {
        while (active.Count > 0)
        {
            var go = active[active.Count - 1]; active.RemoveAt(active.Count - 1);
            if (go) go.GetComponent<PooledVFX>().Release();
        }
    }
    void OnEnable() => GamePauseController.FreezeChanged += OnFreeze;
    void OnFreeze(bool _) { if (GamePauseController.ResultOverlayVisible) Clear(); }
    void Update()
    {
        if (GamePauseController.ResultOverlayVisible) { Clear(); return; }
        if (!CombatSession.Finished) return;
        for (int i = active.Count - 1; i >= 0; i--)
            if (active[i].GetComponent<ImpactVfxBurst>().CompletionPolicy == ImpactVfxCompletionPolicy.StopOnCombatEnd)
                active[i].GetComponent<PooledVFX>().Release();
    }
    void OnDisable()
    {
        GamePauseController.FreezeChanged -= OnFreeze;
        Clear();
    }
    void OnDestroy()
    {
        Clear();
        foreach (var pool in pools.Values) pool.Clear();
        pools.Clear();
        if (poolRoot) Destroy(poolRoot.gameObject);
    }
}
