using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public sealed class HitSparkSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject sparkPrefab;

    [Header("Placement")]
    [SerializeField] Vector3 worldOffset = Vector3.zero;
    [SerializeField] bool faceFromAttacker = true;    // quay theo hướng từ attacker -> target
    [SerializeField] float zRotationOffset = 0f;      // cộng thêm vài độ nếu muốn

    [Header("Limits")]
    [SerializeField, Range(0f, 0.2f)] float minInterval = 0.02f; // chống spam quá dày

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
        Vector3 pos = hitPoint + worldOffset;
        Quaternion rot = Quaternion.identity;

        if (faceFromAttacker && attacker && target)
        {
            Vector2 dir = (Vector2)(target.position - attacker.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + zRotationOffset;
            rot = Quaternion.Euler(0, 0, angle);
        }

        go.transform.SetPositionAndRotation(pos, rot);
    }
}
