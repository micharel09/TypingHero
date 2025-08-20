using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public class SlayerAfterimagePool : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] SpriteRenderer source;
    [SerializeField] bool forceLocalSource = true;
    [SerializeField] bool debugLogs = false;

    [Header("Visual (base)")]
    [SerializeField] Color baseTint = new Color(1f, 1f, 1f, 0.55f);
    [SerializeField] float life = 0.18f;
    [SerializeField] AnimationCurve alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] int sortingOffset = -1;

    [Header("Burst")]
    [SerializeField] int burstCount = 1;
    [SerializeField] float burstInterval = 0f;
    [SerializeField, Range(0f, 1f)] float alphaFalloff = 0.7f;
    [SerializeField] float lifeStep = 0.03f;
    [SerializeField] Vector2 stepLocalOffset = new Vector2(-0.05f, 0f);

    [Header("Combo Tint")]
    [SerializeField] bool useComboTint = true;
    [SerializeField] int comboMax = 20;
    [SerializeField] Gradient comboGradient;

    [Header("Pool")]
    [SerializeField] int defaultCapacity = 8;
    [SerializeField] int maxSize = 32;

    IObjectPool<GameObject> _pool;

    void Awake()
    {
        BindLocalSourceIfNeeded();
        _pool = new ObjectPool<GameObject>(Create, OnGet, OnRelease, OnDestroyItem,
            collectionCheck: false, defaultCapacity, maxSize);
    }

    void OnValidate()
    {
        if (alphaCurve == null) alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
        if (comboGradient == null || comboGradient.colorKeys == null || comboGradient.colorKeys.Length == 0)
        {
            var g = new Gradient();
            g.SetKeys(
                new[]{
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.cyan,  0.33f),
                    new GradientColorKey(Color.yellow,0.66f),
                    new GradientColorKey(Color.red,    1f)},
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            comboGradient = g;
        }
        if (burstCount < 1) burstCount = 1;
        if (comboMax < 1) comboMax = 1;
    }

    void BindLocalSourceIfNeeded()
    {
        if (forceLocalSource || source == null || source.transform.root != transform.root)
        {
            var local = GetComponentInChildren<SpriteRenderer>(true);
            if (local != null) source = local;
            if (debugLogs)
                Debug.Log($"[Afterimage] Bound source = {source?.name} (root={source?.transform.root.name})", this);
        }
    }

    GameObject Create()
    {
        var go = new GameObject("Afterimage");
        go.hideFlags = HideFlags.HideAndDontSave;
        go.AddComponent<SpriteRenderer>();
        go.SetActive(false);
        return go;
    }

    // === POOL HOOKS ===
    void OnGet(GameObject go)
    {
        // Parent trực tiếp về Player để disable Player là tắt hết afterimage
        go.transform.SetParent(transform, worldPositionStays: true);
        go.SetActive(true);
    }

    void OnRelease(GameObject go)
    {
        if (!go) return;
        go.transform.SetParent(transform, true);   // giữ dưới Player cho dễ dọn
        go.SetActive(false);
    }

    void OnDestroyItem(GameObject go) { if (go) Destroy(go); }

    // === API ===
    public void SpawnBurstFromCurrentFrame(int combo)
    {
        // Không spawn nếu đã bị tắt
        if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

        BindLocalSourceIfNeeded();
        if (!source || !source.sprite) return;

        if (debugLogs)
            Debug.Log($"[Afterimage] Spawn from '{source.transform.root.name}/{source.name}' sprite={source.sprite.name}");

        if (burstCount <= 1 && Mathf.Approximately(burstInterval, 0f))
            SpawnOne(0, GetComboTint(combo), life);
        else
            StartCoroutine(BurstCo(combo));
    }
    public void SpawnBurstFromCurrentFrame() => SpawnBurstFromCurrentFrame(0);

    IEnumerator BurstCo(int combo)
    {
        float a = GetComboTint(combo).a;
        float l = life;
        Color c = GetComboTint(combo);
        for (int i = 0; i < burstCount; i++)
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) yield break;
            SpawnOne(i, c, l);
            a *= alphaFalloff; c.a = a; l += lifeStep;
            if (i < burstCount - 1 && burstInterval > 0f)
                yield return new WaitForSecondsRealtime(burstInterval);
        }
    }

    Color GetComboTint(int combo)
    {
        if (!useComboTint || comboMax <= 0 || comboGradient == null) return baseTint;
        float t = Mathf.Clamp01(combo / (float)comboMax);
        var col = comboGradient.Evaluate(t);
        col.a *= baseTint.a;
        return col;
    }

    void SpawnOne(int index, Color tint, float lifeSeconds)
    {
        var go = _pool.Get();
        var t = go.transform;

        float dirX = source.flipX ? -1f : 1f;
        Vector3 local = new Vector3(stepLocalOffset.x * dirX, stepLocalOffset.y, 0f) * index;

        t.position = source.transform.position + local;
        t.rotation = source.transform.rotation;
        t.localScale = source.transform.lossyScale;

        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = source.sprite;
        sr.flipX = source.flipX;
        sr.flipY = source.flipY;
        sr.sortingLayerID = source.sortingLayerID;
        sr.sortingOrder = source.sortingOrder + sortingOffset;
        sr.sharedMaterial = source.sharedMaterial;   // giữ shader/URP lit
        sr.color = tint;

        StartCoroutine(FadeAndRelease(sr, go, tint, lifeSeconds));
    }

    IEnumerator FadeAndRelease(SpriteRenderer sr, GameObject go, Color baseColor, float lifeSeconds)
    {
        float t = 0f;
        while (t < lifeSeconds)
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) break; // Player tắt -> dọn
            float a = alphaCurve.Evaluate(t / Mathf.Max(0.0001f, lifeSeconds));
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * a);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        _pool.Release(go);
    }

    // === CLEANUP GỌN ===
    void OnDisable()
    {
        // Dừng mọi fade đang chạy và thu hồi tất cả Afterimage (đang ở dưới transform)
        StopAllCoroutines();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (!child || child.name != "Afterimage") continue;
            if (_pool != null) _pool.Release(child.gameObject);
            else Destroy(child.gameObject);
        }

#if UNITY_2021_3_OR_NEWER
        if (_pool is ObjectPool<GameObject> op) op.Clear();
#endif
    }
}
