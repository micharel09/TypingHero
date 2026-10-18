using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public sealed class ParrySparkVFX_Pooled : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] ParrySystem parrySystem;       // kéo ParrySystem (Player)
    [SerializeField] GameObject sparkPrefab;        // kéo prefab VFX_HitSpark_Parry (prefab, không để trong Scene)
    [SerializeField] Transform playerSparkAnchor;   // kéo Player/Attack1/ParrySparkAnchor

    [Header("Tuning")]
    [SerializeField] Vector2 offset;
    [SerializeField] bool flipTowardEnemy = true;
    [SerializeField] bool parentToAnchor = false;

    [Header("Pool")]
    [SerializeField] int defaultCapacity = 8;
    [SerializeField] int maxSize = 32;

    IObjectPool<GameObject> _pool;
    readonly HashSet<GameObject> active = new();
    Transform poolRoot;

    void OnEnable()
    {
        if (_pool == null) BuildPool();
        if (parrySystem) parrySystem.OnParrySuccess += Spawn;
    }

    void OnDisable()
    {
        if (parrySystem) parrySystem.OnParrySuccess -= Spawn;
        foreach (var go in new List<GameObject>(active))
            if (go) go.GetComponent<PooledVFX>().AnimEvent_Release();
    }

    void BuildPool()
    {
        poolRoot = new GameObject("ParryImpactPool").transform;
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(poolRoot.gameObject, gameObject.scene);
        _pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    GameObject CreateItem()
    {
        var go = Instantiate(sparkPrefab, poolRoot);
        go.SetActive(false);

        // gắn PooledVFX để animation-end có thể trả về pool
        var pooled = go.GetComponent<PooledVFX>();
        if (!pooled) pooled = go.AddComponent<PooledVFX>();
        pooled.Setup(_pool);

        return go;
    }

    void OnGet(GameObject go)
    {
        active.Add(go);
        go.SetActive(true);
    }

    void OnRelease(GameObject go)
    {
        // thu gọn trước khi cất vào pool
        active.Remove(go);
        go.transform.SetParent(poolRoot, true);
        go.SetActive(false);
    }

    void OnDestroyItem(GameObject go)
    {
        if (go) Destroy(go);
    }

    void OnDestroy()
    {
        if (_pool is ObjectPool<GameObject> pool) pool.Clear();
        if (poolRoot) Destroy(poolRoot.gameObject);
    }

    void Spawn(ParrySystem.ParryContext ctx)
    {
        if (_pool == null || sparkPrefab == null || !playerSparkAnchor || GamePauseController.InputBlocked || CombatSession.Finished || Time.timeScale == 0f) return;

        var go = _pool.Get();
        var t = go.transform;

        // đặt vị trí
        t.position = (Vector2)playerSparkAnchor.position + offset;
        if (parentToAnchor) t.SetParent(playerSparkAnchor, worldPositionStays: true);

        // lật hướng về enemy cho đẹp (tuỳ chọn)
        if (flipTowardEnemy && ctx.targetRoot)
        {
            float dir = Mathf.Sign(ctx.targetRoot.position.x - playerSparkAnchor.position.x);
            var s = t.localScale;
            t.localScale = new Vector3(Mathf.Abs(s.x) * (dir == 0 ? 1 : dir), s.y, s.z);
        }
    }
}
