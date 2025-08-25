using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public class SlayerAfterimagePool : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] SpriteRenderer source; // để trống -> tự lấy SR gần nhất

    [Header("Visual")]
    [SerializeField] Color baseTint = new(1, 1, 1, 0.55f);
    [SerializeField] float life = 0.18f;
    [SerializeField] AnimationCurve alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] int sortingOffset = -1;

    [Header("Burst")]
    [SerializeField] int burstCount = 1;
    [SerializeField] float burstInterval = 0f;
    [SerializeField, Range(0f, 1f)] float alphaFalloff = 0.7f;
    [SerializeField] float lifeStep = 0.03f;
    [SerializeField] Vector2 stepLocalOffset = new(-0.05f, 0f);

    [Header("Combo Tint")]
    [SerializeField] bool useComboTint = true;
    [SerializeField] int comboMax = 20;
    [SerializeField] Gradient comboGradient;

    [Header("Color FX")]
    [SerializeField] bool useSaturationBoost = true;
    [SerializeField, Range(0f, 1f)] float maxSaturationBoost = 0.6f;
    [SerializeField] AnimationCurve saturationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Pool")]
    [SerializeField] int defaultCapacity = 8;
    [SerializeField] int maxSize = 32;

    [Header("Material")]
    [SerializeField] bool forceUnlit = true; // <-- GIỮ ON để không bị tô đen trong Slayer
    static Material s_UnlitMat;              // share toàn scene

    [SerializeField] bool freezeFadeOnPause = true;
    IObjectPool<GameObject> pool;

    void Awake()
    {
        if (!source) source = GetComponentInChildren<SpriteRenderer>(true);

        // Chuẩn bị Unlit share một lần
        if (forceUnlit && s_UnlitMat == null)
        {
            var sh = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (sh) s_UnlitMat = new Material(sh) { name = "Afterimage_Unlit_Shared" };
        }

        pool = new ObjectPool<GameObject>(Create, OnGet, OnRelease, DestroyItem, false, defaultCapacity, maxSize);
    }

    void OnValidate()
    {
        if (alphaCurve == null) alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
        if (saturationCurve == null) saturationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        if (burstCount < 1) burstCount = 1;
        if (comboMax   < 1) comboMax   = 1;

        if (comboGradient == null || comboGradient.colorKeys == null || comboGradient.colorKeys.Length == 0)
        {
            var g = new Gradient();
            g.SetKeys(
                new[]{
                    new GradientColorKey(Color.white , 0f),
                    new GradientColorKey(Color.cyan  , 0.33f),
                    new GradientColorKey(Color.yellow, 0.66f),
                    new GradientColorKey(Color.red   , 1f)},
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            comboGradient = g;
        }
    }

    GameObject Create()
    {
        var go = new GameObject("Afterimage") { hideFlags = HideFlags.HideAndDontSave };
        go.AddComponent<SpriteRenderer>();
        go.SetActive(false);
        return go;
    }
    void OnGet(GameObject go) { go.transform.SetParent(transform, true); go.SetActive(true); }
    void OnRelease(GameObject go) { if (go) { go.transform.SetParent(transform, true); go.SetActive(false); } }
    void DestroyItem(GameObject go) { if (go) Destroy(go); }

    // API
    public void SpawnBurstFromCurrentFrame(int combo = 0)
    {
        if (Time.timeScale == 0f) return;
        if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;
        if (!source || !source.sprite) return;

        if (burstCount <= 1 && Mathf.Approximately(burstInterval, 0f))
            SpawnOne(0, Tint(combo), life);
        else
            StartCoroutine(BurstCo(combo));
    }

    IEnumerator BurstCo(int combo)
    {
        Color c = Tint(combo);
        float a = c.a, l = life;
        for (int i = 0; i < burstCount; i++)
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) yield break;
            SpawnOne(i, c, l);
            a *= alphaFalloff; c.a = a; l += lifeStep;
            if (i < burstCount - 1 && burstInterval > 0f) yield return new WaitForSecondsRealtime(burstInterval);
        }
    }

    Color Tint(int combo)
    {
        Color col = useComboTint && comboGradient != null
            ? comboGradient.Evaluate(Mathf.Clamp01(combo / (float)comboMax))
            : baseTint;
        col.a *= baseTint.a;

        if (useSaturationBoost)
        {
            float t = Mathf.Clamp01(combo / (float)comboMax);
            float k = Mathf.Clamp01(saturationCurve.Evaluate(t));
            Color.RGBToHSV(col, out float h, out float s, out float v);
            s = Mathf.Clamp01(s + (maxSaturationBoost * k) * (1f - s));
            col = Color.HSVToRGB(h, s, v); col.a = baseTint.a;
        }
        return col;
    }

    void SpawnOne(int index, Color tint, float lifeSeconds)
    {
        var go = pool.Get();
        var t = go.transform;

        float dirX = (source && source.flipX) ? -1f : 1f;
        Vector3 local = new(stepLocalOffset.x * dirX, stepLocalOffset.y, 0f);
        t.position   = source.transform.position + local * index;
        t.rotation   = source.transform.rotation;
        t.localScale = source.transform.lossyScale;

        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite         = source.sprite;
        sr.flipX          = source.flipX;
        sr.flipY          = source.flipY;
        sr.sortingLayerID = source.sortingLayerID;
        sr.sortingOrder   = source.sortingOrder + sortingOffset;

        // ✅ Tránh bị SilhouetteGroup tô đen
        sr.sharedMaterial = (forceUnlit && s_UnlitMat) ? s_UnlitMat : source.sharedMaterial;

        sr.color = tint;
        StartCoroutine(FadeAndRelease(sr, go, tint, lifeSeconds));
    }

    // sửa coroutine fade (đúng hàm bạn đang dùng để giảm alpha rồi trả pool)
    IEnumerator FadeAndRelease(SpriteRenderer sr, GameObject go, Color baseColor, float lifeSeconds)
    {
        float t = 0f;
        float life = Mathf.Max(0.0001f, lifeSeconds);
        sr.color = baseColor;

        while (t < life)
        {
            if (!(freezeFadeOnPause && Time.timeScale == 0f))
                t += Time.unscaledDeltaTime;       // dùng unscaled nhưng vẫn đứng khi pause

            float k = Mathf.Clamp01(t / life);
            float a = alphaCurve.Evaluate(k);
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * a);

            yield return null;
        }

        pool.Release(go);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i);
            if (c && c.name == "Afterimage") pool.Release(c.gameObject);
        }
#if UNITY_2021_3_OR_NEWER
        if (pool is ObjectPool<GameObject> op) op.Clear();
#endif
    }
}
