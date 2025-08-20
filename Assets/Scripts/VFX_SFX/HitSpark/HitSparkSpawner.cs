using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public sealed class HitSparkSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject sparkPrefab;

    [Header("Parenting")]
    [Tooltip("Parent spark dưới attacker để SilhouetteGroup bắt được.")]
    [SerializeField] bool parentUnderAttacker = true;

    [Header("Placement")]
    [SerializeField] Vector3 worldOffset = Vector3.zero;
    [SerializeField] bool faceFromAttacker = true;
    [SerializeField] float zRotationOffset = 0f;

    [Header("Randomize")]
    [SerializeField] Vector2 angleJitter = new Vector2(-25f, 25f);
    [SerializeField, Range(0f, 1f)] float flipXChance = 0.5f;
    [SerializeField, Range(0f, 1f)] float flipYChance = 0f;
    [SerializeField] Vector2 scaleRange = new Vector2(0.9f, 1.1f);
    [SerializeField] Vector2 verticalJitter = new Vector2(-0.08f, 0.08f);

    [Header("Limits")]
    [SerializeField, Range(0f, 0.2f)] float minInterval = 0.02f;

    IObjectPool<GameObject> _pool;
    float _lastSpawnAt = -999f;

    void Awake()
    {
        Debug.Assert(sparkPrefab != null, "[HitSparkSpawner] Chưa gán sparkPrefab.");
        _pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var go = Instantiate(sparkPrefab);
                if (go.TryGetComponent<PooledVFX>(out var pvfx))
                    pvfx.Setup(_pool);
                return go;
            },
            actionOnGet: go => go.SetActive(true),
            actionOnRelease: go => go.SetActive(false),
            actionOnDestroy: go => Destroy(go),
            collectionCheck: false, defaultCapacity: 8, maxSize: 64
        );
    }

    public void Spawn(Vector3 hitPoint, Transform attacker, Transform target)
    {
        if (Time.unscaledTime - _lastSpawnAt < minInterval) return;
        _lastSpawnAt = Time.unscaledTime;

        var go = _pool.Get();

        // Parent vào attacker để SilhouetteGroup tô đen khi Slayer bật
        if (parentUnderAttacker && attacker)
            go.transform.SetParent(attacker, worldPositionStays: true);
        else
            go.transform.SetParent(null, true);

        // --- vị trí + jitter dọc ---
        float yJit = Random.Range(verticalJitter.x, verticalJitter.y);
        Vector3 pos = hitPoint + worldOffset + new Vector3(0f, yJit, 0f);

        // --- hướng base ---
        float angle = 0f;
        if (faceFromAttacker && attacker && target)
        {
            Vector2 dir = (Vector2)(target.position - attacker.position);
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        angle += zRotationOffset + Random.Range(angleJitter.x, angleJitter.y);

        go.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angle));

        // --- flip + scale ---
        var s = go.transform.localScale;
        float baseScale = Random.Range(scaleRange.x, scaleRange.y);
        bool flipX = Random.value < flipXChance;
        bool flipY = Random.value < flipYChance;
        s.x = Mathf.Abs(s.x) * (flipX ? -1f : 1f) * baseScale;
        s.y = Mathf.Abs(s.y) * (flipY ? -1f : 1f) * baseScale;
        go.transform.localScale = s;
    }
}
