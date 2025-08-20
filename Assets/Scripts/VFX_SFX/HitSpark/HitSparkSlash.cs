using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public sealed class HitSparkSlash : MonoBehaviour
{
    [Header("Renderers (assign in prefab)")]
    [SerializeField] SpriteRenderer core;
    [SerializeField] SpriteRenderer glow1;
    [SerializeField] SpriteRenderer glow2;

    [Header("Force Sorting (đảm bảo nổi trên màn tối Slayer)")]
    [SerializeField] bool forceSorting = true;
    [SerializeField] string sortingLayerName = "VFX_Overlay";
    [SerializeField] int sortingOrder = 100;

    [Header("Lifespan")]
    [SerializeField, Range(0.03f, 0.20f)] float life = 0.09f;
    [SerializeField] AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Shape (local scale)")]
    [SerializeField] Vector2 lengthRange = new Vector2(0.9f, 1.5f);
    [SerializeField] Vector2 widthRange = new Vector2(0.06f, 0.12f);
    [SerializeField, Range(0f, 1f)] float coreWidthFactor = 0.6f;

    [Header("Color")]
    [SerializeField] Color coreColor = Color.white;
    [SerializeField] Color glow1Color = new Color(1f, 0.95f, 0.6f, 0.9f);
    [SerializeField] Color glow2Color = new Color(1f, 0.55f, 0.35f, 0.8f);

    [Header("Jitter")]
    [SerializeField] float posJitter = 0.04f;

    IObjectPool<HitSparkSlash> _pool;
    float _endAt;
    Color _c0, _c1, _c2;

    MaterialPropertyBlock _mpb0, _mpb1, _mpb2;

    public void Setup(IObjectPool<HitSparkSlash> pool) => _pool = pool;

    void Awake()
    {
        // Chỉ chấp nhận SR là CON của prefab
        ValidateLocal(ref core, nameof(core));
        ValidateLocal(ref glow1, nameof(glow1));
        ValidateLocal(ref glow2, nameof(glow2));

        // Auto-find nếu để trống
        if (!core)
        {
            var srs = GetComponentsInChildren<SpriteRenderer>(true);
            if (srs.Length > 0) core = srs[0];
            if (!glow1 && srs.Length > 1) glow1 = srs[1];
            if (!glow2 && srs.Length > 2) glow2 = srs[2];
        }

        // Nếu core chưa có Sprite → tự tạo 1×16 trắng (thấy ngay)
        if (core && core.sprite == null)
        {
            var tex = new Texture2D(1, 16, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var px = new Color[16]; for (int i = 0; i < px.Length; i++) px[i] = Color.white;
            tex.SetPixels(px); tex.Apply();
            core.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 16), new Vector2(0.5f, 0.5f), 16);
        }

        if (forceSorting) ApplySortingAll();

        _mpb0 ??= new MaterialPropertyBlock();
        _mpb1 ??= new MaterialPropertyBlock();
        _mpb2 ??= new MaterialPropertyBlock();

        SetColor(core, coreColor, 0f, _mpb0);
        SetColor(glow1, glow1Color, 0f, _mpb1);
        SetColor(glow2, glow2Color, 0f, _mpb2);

        enabled = false;
    }

    void ValidateLocal(ref SpriteRenderer sr, string field)
    {
        if (sr && !sr.transform.IsChildOf(transform))
        {
            Debug.LogWarning($"[HitSparkSlash] '{field}' trỏ tới SpriteRenderer ngoài prefab. Bỏ qua để khỏi sửa nhầm.");
            sr = null;
        }
    }

    void ApplySortingAll()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>(true))
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }
    }

    void SetColor(SpriteRenderer sr, Color baseC, float a, MaterialPropertyBlock mpb)
    {
        if (!sr) return;
        var c = new Color(baseC.r, baseC.g, baseC.b, baseC.a * a);
        sr.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", c);
        sr.SetPropertyBlock(mpb);
    }

    public void PlayAt(Vector2 worldPos, float angleDeg, float lengthMul = 1f)
    {
        transform.position = worldPos + Random.insideUnitCircle * posJitter;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        float len = Random.Range(lengthRange.x, lengthRange.y) * Mathf.Max(0.6f, lengthMul);
        float wid = Random.Range(widthRange.x, widthRange.y);
        transform.localScale = new Vector3(wid, len, 1f);

        if (core) core.transform.localScale = new Vector3(wid * coreWidthFactor, 1f, 1f);

        _c0 = coreColor; _c1 = glow1Color; _c2 = glow2Color;
        _endAt = Time.unscaledTime + life;

        ApplyAlpha(1f);
        enabled = true;
    }

    void Update()
    {
        float remain = _endAt - Time.unscaledTime;
        if (remain <= 0f) { Release(); return; }

        float t = 1f - (remain / life);
        float a = Mathf.Clamp01(alphaCurve.Evaluate(t));
        ApplyAlpha(a);

        float shrink = 1f - t * 0.35f;
        var s = transform.localScale;
        transform.localScale = new Vector3(s.x, s.y * shrink, 1f);
    }

    void ApplyAlpha(float a)
    {
        SetColor(core, _c0, a, _mpb0);
        SetColor(glow1, _c1, a * 0.9f, _mpb1);
        SetColor(glow2, _c2, a * 0.8f, _mpb2);
    }

    void Release()
    {
        enabled = false;
        ApplyAlpha(0f); // trả alpha về 0
        if (_pool != null) _pool.Release(this);
        else Destroy(gameObject);
    }
}
