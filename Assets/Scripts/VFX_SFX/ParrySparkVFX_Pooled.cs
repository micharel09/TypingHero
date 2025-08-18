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

    void OnEnable()
    {
        if (_pool == null) BuildPool();
        if (parrySystem) parrySystem.OnParrySuccess += Spawn;
    }

    void OnDisable()
    {
        if (parrySystem) parrySystem.OnParrySuccess -= Spawn;
    }

    void BuildPool()
    {
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
        var go = Instantiate(sparkPrefab);
        go.SetActive(false);

        // gắn PooledVFX để animation-end có thể trả về pool
        var pooled = go.GetComponent<PooledVFX>();
        if (!pooled) pooled = go.AddComponent<PooledVFX>();
        pooled.Setup(_pool);

        return go;
    }

    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    void OnRelease(GameObject go)
    {
        // thu gọn trước khi cất vào pool
        if (parentToAnchor == false) go.transform.SetParent(null);
        go.SetActive(false);
    }

    void OnDestroyItem(GameObject go)
    {
        if (go) Destroy(go);
    }

    void Spawn(ParrySystem.ParryContext ctx)
    {
        if (_pool == null || sparkPrefab == null || !playerSparkAnchor) return;

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
