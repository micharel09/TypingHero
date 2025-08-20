using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class SlayerScreenFX : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] RectTransform topBar;
    [SerializeField] RectTransform bottomBar;
    [SerializeField] SpriteRenderer redOverlay;

    [Header("Zoom")]
    [SerializeField] float normalOrtho = 5f;
    [SerializeField] float slayerOrtho = 3.9f;
    [SerializeField, Range(0.05f, 1.5f)] float zoomDuration = 0.35f;
    [SerializeField] AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Filmframe Bars (pixel)")]
    [SerializeField] float barHeight = 64f;
    [SerializeField, Range(0.05f, 1.0f)] float barDuration = 0.22f;
    [SerializeField] AnimationCurve barCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Background Red")]
    [SerializeField, Range(0f, 1f)] float redAlpha = 1f;
    [SerializeField, Range(0.05f, 1.0f)] float redFade = 0.18f;

    [Header("General")]
    [SerializeField] bool useUnscaledTime = true;
    [SerializeField] bool logs = false;
    [SerializeField] CinemachineImpulseSource enterShake;
    [SerializeField, Range(0f, 0.2f)] float shakeDelay = 0.03f;

    // ---------- Simple Toggle Group ----------
    [Header("Editor Toggle Group")]
    [Tooltip("Kéo vào 1 object (vd: ScreenFX_Root) để tắt/bật cả nhóm. Có thể kéo nhiều object.")]
    [SerializeField] List<GameObject> toggleGroup = new List<GameObject>();

    [Tooltip("Tự bật nhóm khi EnterSlayerFX()")]
    [SerializeField] bool autoShowOnEnter = false;
    [Tooltip("Tự tắt nhóm khi ExitSlayerFX()")]
    [SerializeField] bool autoHideOnExit = false;

#if UNITY_EDITOR
    [ContextMenu("Group/Show")]
    void __ShowGroup() => SetGroup(true);
    [ContextMenu("Group/Hide")]
    void __HideGroup() => SetGroup(false);
    [ContextMenu("Group/Toggle")]
    void __ToggleGroup()
    {
        bool turnOn = false;
        foreach (var go in toggleGroup) if (go && !go.activeSelf) { turnOn = true; break; }
        SetGroup(turnOn);
    }

    [ContextMenu("Group/Auto-Fill (Top/Bottom/Overlay)")]
    void __AutoFill()
    {
        AddIfMissing(topBar ? topBar.gameObject : null);
        AddIfMissing(bottomBar ? bottomBar.gameObject : null);
        AddIfMissing(redOverlay ? redOverlay.gameObject : null);
        EditorUtility.SetDirty(this);
    }
#endif

    void SetGroup(bool on)
    {
        foreach (var go in toggleGroup) if (go) go.SetActive(on);
        if (!on && redOverlay)
        {
            var c = redOverlay.color; c.a = 0f; redOverlay.color = c;
            redOverlay.enabled = false;
        }
#if UNITY_EDITOR
        EditorApplication.RepaintHierarchyWindow();
        SceneView.RepaintAll();
#endif
    }
    void AddIfMissing(GameObject go)
    {
        if (!go) return;
        if (toggleGroup == null) toggleGroup = new List<GameObject>();
        if (!toggleGroup.Contains(go)) toggleGroup.Add(go);
    }
    // ---------- End Toggle Group ----------

    Camera _main;

    void Awake()
    {
        if (!enterShake) enterShake = GetComponent<CinemachineImpulseSource>();
        if (!vcam) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        _main = Camera.main;

        SetBarsInstant(0f);
        SetRedInstant(0f);
        SetZoomInstant(normalOrtho);
    }

    // Public API
    public void EnterSlayerFX()
    {
        if (autoShowOnEnter) SetGroup(true);

        StopAllCoroutines();
        StartCoroutine(CoZoom(vcam.m_Lens.OrthographicSize, slayerOrtho, zoomDuration, zoomCurve));
        StartCoroutine(CoBars(GetBarHeight(topBar), barHeight, barDuration, barCurve));
        StartCoroutine(CoRed(GetRedAlpha(), redAlpha, redFade));
        if (enterShake) StartCoroutine(CoShake());
        if (logs) Debug.Log("[SlayerScreenFX] Enter");
    }

    IEnumerator CoShake()
    {
        if (shakeDelay > 0f) yield return new WaitForSecondsRealtime(shakeDelay);
        enterShake.GenerateImpulse();
    }

    public void ExitSlayerFX()
    {
        StopAllCoroutines();
        StartCoroutine(CoZoom(vcam.m_Lens.OrthographicSize, normalOrtho, zoomDuration, zoomCurve));
        StartCoroutine(CoBars(GetBarHeight(topBar), 0f, barDuration, barCurve));
        StartCoroutine(CoRed(GetRedAlpha(), 0f, redFade));
        if (autoHideOnExit) StartCoroutine(CoDisableAfter(redFade));
        if (logs) Debug.Log("[SlayerScreenFX] Exit");
    }

    IEnumerator CoDisableAfter(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        SetGroup(false);
    }

    // Coroutines
    IEnumerator CoZoom(float from, float to, float dur, AnimationCurve curve)
    {
        if (!vcam) yield break;
        float t = 0f;
        while (t < 1f)
        {
            t += Dt(dur);
            float k = curve.Evaluate(Mathf.Clamp01(t));
            vcam.m_Lens.OrthographicSize = Mathf.LerpUnclamped(from, to, k);
            yield return null;
        }
        vcam.m_Lens.OrthographicSize = to;
    }

    IEnumerator CoBars(float from, float to, float dur, AnimationCurve curve)
    {
        if (!topBar || !bottomBar) yield break;
        float t = 0f;
        while (t < 1f)
        {
            t += Dt(dur);
            float k = curve.Evaluate(Mathf.Clamp01(t));
            float h = Mathf.LerpUnclamped(from, to, k);
            SetBarsInstant(h);
            yield return null;
        }
        SetBarsInstant(to);
    }

    IEnumerator CoRed(float from, float to, float dur)
    {
        if (!redOverlay) yield break;
        if (!redOverlay.enabled) redOverlay.enabled = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Dt(dur);
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t));
            var c = redOverlay.color; c.a = a; redOverlay.color = c;
            yield return null;
        }
        var cf = redOverlay.color; cf.a = to; redOverlay.color = cf;
        if (to <= 0.001f) redOverlay.enabled = false;
    }

    // Helpers
    float Dt(float dur) => (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / Mathf.Max(0.0001f, dur);

    void SetBarsInstant(float height)
    {
        if (topBar) { var s = topBar.sizeDelta; s.y = height; topBar.sizeDelta = s; }
        if (bottomBar) { var s = bottomBar.sizeDelta; s.y = height; bottomBar.sizeDelta = s; }
    }
    void SetRedInstant(float a)
    {
        if (!redOverlay) return;
        var c = redOverlay.color; c.a = a; redOverlay.color = c;
        redOverlay.enabled = a > 0.001f;
    }
    void SetZoomInstant(float ortho)
    {
        if (vcam) vcam.m_Lens.OrthographicSize = ortho;
        if (_main && _main.orthographic) _main.orthographicSize = ortho;
    }
    float GetRedAlpha() => redOverlay ? redOverlay.color.a : 0f;
    float GetBarHeight(RectTransform rt) => rt ? rt.sizeDelta.y : 0f;
}
