using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public class HitSparkSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject sparkPrefab;

    [Header("Parenting")]
    [SerializeField] bool parentUnderAttacker = true;
    [Tooltip("Giữ kích thước trong world không đổi dù parent có scale.")]
    [SerializeField] bool keepWorldScale = true;

    [Header("Placement")]
    [SerializeField] Vector3 worldOffset = Vector3.zero;
    [SerializeField] bool faceFromAttacker = true;
    [SerializeField] float zRotationOffset = 0f;

    [Header("Randomize")]
    [SerializeField] Vector2 angleJitter = new(-2f, 2f);
    [SerializeField, Range(0, 1)] float flipXChance = 0.5f;
    [SerializeField, Range(0, 1)] float flipYChance = 0f;
    [SerializeField] Vector2 scaleRange = new(0.9f, 1.1f);
    [SerializeField] Vector2 verticalJitter = new(-0.08f, 0.08f);

    [Header("Limits")]
    [SerializeField] float minInterval = 0.02f;

    // pool
    IObjectPool<GameObject> _pool;
    Vector3 _prefabScale = Vector3.one;
    float _nextAt;

    void Awake()
    {
        if (!sparkPrefab) enabled = false;

        // lấy scale gốc của prefab để dùng lại mỗi lần spawn
        _prefabScale = sparkPrefab.transform.localScale;

        _pool = new ObjectPool<GameObject>(
            Create, OnGet, OnRelease, OnDestroyItem,
            collectionCheck: false, defaultCapacity: 8, maxSize: 32);
    }

    GameObject Create()
    {
        var go = Instantiate(sparkPrefab);
        // Đảm bảo scale khởi điểm đúng như prefab
        go.transform.localScale = _prefabScale;
        if (go.TryGetComponent<PooledVFX>(out var pvfx)) pvfx.Setup(_pool);
        return go;
    }

    void OnGet(GameObject go) { go.SetActive(true); }
    void OnRelease(GameObject go) { if (go) go.SetActive(false); }
    void OnDestroyItem(GameObject go) { if (go) Destroy(go); }

    public void Spawn(Vector2 worldPoint, Transform attacker, Transform targetRoot)
    {
        if (Time.time < _nextAt || _pool == null) return;
        _nextAt = Time.time + minInterval;

        var go = _pool.Get();
        var t = go.transform;

        // --- vị trí ---
        var pos = (Vector3)worldPoint + worldOffset;
        pos.y += Random.Range(verticalJitter.x, verticalJitter.y);

        // Parent trước để tính scale bù chính xác
        if (parentUnderAttacker && attacker)
            t.SetParent(attacker, worldPositionStays: true);
        else
            t.SetParent(null, worldPositionStays: true);

        t.position = pos;

        // --- hướng/rotation ---
        float faceSign = 1f;
        if (faceFromAttacker && attacker)
            faceSign = (attacker.lossyScale.x < 0f) ? -1f : 1f;

        float angle = Random.Range(angleJitter.x, angleJitter.y);
        t.rotation = Quaternion.Euler(0, 0, zRotationOffset + angle * faceSign);

        // --- scale tuyệt đối (KHÔNG cộng dồn) ---
        float s = Random.Range(scaleRange.x, scaleRange.y);
        Vector3 baseScale = _prefabScale * s;

        if (parentUnderAttacker && keepWorldScale && attacker)
        {
            // bù parent để world-scale ổn định
            Vector3 p = attacker.lossyScale;
            baseScale = new Vector3(
                SafeDiv(baseScale.x, p.x),
                SafeDiv(baseScale.y, p.y),
                SafeDiv(baseScale.z, p.z));
        }
        t.localScale = baseScale;

        // Flip ngẫu nhiên (theo localScale)
        if (Random.value < flipXChance) t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
        if (Random.value < flipYChance) t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
    }

    static float SafeDiv(float a, float b) => a / (Mathf.Abs(b) < 1e-5f ? 1f : b);
}
