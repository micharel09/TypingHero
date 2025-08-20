using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public sealed class HitSparkSlashPool : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] HitSparkSlash prefab;

    [Header("Angle (vertical family)")]
    [SerializeField] float angleCenter = 90f;      // dọc
    [SerializeField] float angleVariance = 14f;    // ± độ quanh dọc

    [Header("Burst")]
    [SerializeField, Range(1, 3)] int sparksPerHit = 2;
    [SerializeField] Vector2 lengthMulRange = new Vector2(0.9f, 1.2f);

    [Header("Sorting Override (đảm bảo nằm trên lớp tối Slayer)")]
    [SerializeField] bool overrideSortingFromPool = true;
    [SerializeField] string sortingLayerName = "VFX_Overlay";
    [SerializeField] int sortingOrder = 100;

    IObjectPool<HitSparkSlash> _pool;

    void Awake()
    {
        _pool = new ObjectPool<HitSparkSlash>(
            createFunc: () =>
            {
                var inst = Instantiate(prefab, transform);
                inst.gameObject.SetActive(false);
                inst.Setup(_pool);
                if (overrideSortingFromPool)
                    ForceSorting(inst);
                return inst;
            },
            actionOnGet: (hs) => { hs.gameObject.SetActive(true); },
            actionOnRelease: (hs) => { hs.gameObject.SetActive(false); },
            actionOnDestroy: (hs) => { if (hs) Destroy(hs.gameObject); },
            collectionCheck: false, defaultCapacity: 16, maxSize: 64
        );
    }

    void ForceSorting(HitSparkSlash inst)
    {
        // ép sorting layer/order cho tất cả SR con (kể cả chỉ có core)
        var srs = inst.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in srs)
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }
    }

    public void Spawn(Vector2 worldPos)
    {
        int n = sparksPerHit;
        for (int i = 0; i < n; i++)
        {
            float ang = angleCenter + Random.Range(-angleVariance, angleVariance);
            float mul = Random.Range(lengthMulRange.x, lengthMulRange.y);

            var hs = _pool.Get();
            // PlayAt sẽ tự thêm jitter/scale/alpha
            hs.PlayAt(worldPos, ang, mul);
        }
        Debug.Log($"[SparkPool] Spawned {n} at {worldPos}");
    }
}
