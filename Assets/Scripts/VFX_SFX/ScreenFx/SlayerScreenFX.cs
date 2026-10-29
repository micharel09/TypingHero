using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
public class SlayerScreenFX : MonoBehaviour
{
    public WorldLightingProfile lighting;
    float NormalOrtho => lighting ? lighting.normalOrtho : normalOrtho;
    float SlayerOrtho => lighting ? lighting.slayerOrtho : slayerOrtho;

    [Header("Refs")]
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] RectTransform topBar;
    [SerializeField] RectTransform bottomBar;
    [SerializeField] SpriteRenderer redOverlay;

    [Header("Zoom")]
    [SerializeField] float normalOrtho = 5f;
    [SerializeField] float slayerOrtho = 3.9f;
    [SerializeField, Range(0.05f, 1.5f)] [HideInInspector] float zoomDuration = 0.35f;
    [SerializeField] [HideInInspector] AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Filmframe Bars (pixel)")]
    [SerializeField] float barHeight = 64f;
    [SerializeField, Range(0.05f, 1.0f)] [HideInInspector] float barDuration = 0.22f;
    [SerializeField] [HideInInspector] AnimationCurve barCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Background Red")]
    [SerializeField, Range(0f, 1f)] float redAlpha = 1f;
    [SerializeField, Range(0.05f, 1.0f)] [HideInInspector] float redFade = 0.18f;

    [Header("General")]
    [SerializeField] [HideInInspector] bool useUnscaledTime = true;
    [SerializeField] bool logs = false;
    [SerializeField] CinemachineImpulseSource enterShake;
    [SerializeField] CombatCameraImpact cameraImpact;
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

    [Header("Cinematic presentation")]
    [SerializeField] Transform environment;
    [SerializeField] GameObject combatLighting;
    [SerializeField] BoxCollider2D playerBody, enemyBody;
    [SerializeField] CombatHUDLayout hudLayout;
    [SerializeField] RectTransform scoreRoot, typingTray, popupAnchor;
    [SerializeField] Image hudPanel, typingPanel;
    [SerializeField] GameObject scoreCaption;
    [SerializeField] float bodyPadding = 8f;
    bool _cinematic;
    float _barReveal;
    Camera _main;
    float _topFrameHeight, _bottomFrameHeight;

    public float BarReveal => _barReveal;
    // Full destination frame in canvas coordinates, independent of the animated reveal.
    public Rect TopFrame => new Rect(topBar.rect.xMin, -_topFrameHeight,
        topBar.rect.width, _topFrameHeight);
    public RectTransform TopBar => topBar;

    // Runs after Cinemachine and the viewport camera: use the actual rendered projection.
    void LateUpdate()
    {
        if ((!_cinematic && _barReveal <= 0) || !_main || !hudLayout) return;
        if (!playerBody || !enemyBody)
        {
            // Lethal may remove actors during the closing tween; retain the last frame.
            ApplyFrameHeights();
            return;
        }
        var viewport = CombatViewportCamera.Viewport(_main);
        CombatViewportCamera.SetViewport(_main, new Rect(0, viewport.y, 1, viewport.height));
        var parent = topBar.parent as RectTransform;
        var head = hudLayout.HighestHead;
        var foot = hudLayout.LowestFoot;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
            CombatViewportCamera.WorldToScreenPoint(_main, head), null, out var top);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
            CombatViewportCamera.WorldToScreenPoint(_main, foot), null, out var bottom);
        float topHeight = Mathf.Max(0, parent.rect.yMax - top.y - bodyPadding);
        _topFrameHeight = topHeight;
        float bottomHeight = Mathf.Max(0, bottom.y - parent.rect.yMin - bodyPadding);
        _bottomFrameHeight = bottomHeight;
        ApplyFrameHeights();

    }

    void ApplyFrameHeights()
    {
        topBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _topFrameHeight * _barReveal);
        bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _bottomFrameHeight * _barReveal);
    }

    [SerializeField, HideInInspector] int presentationVersion;
    public void MigratePresentationDefaults()
    {
        if (presentationVersion >= 1) return;
        // Old authored custom zoom durations survive the one-time migration.
        if (!Mathf.Approximately(zoomDuration, .35f))
        {
            if (Mathf.Approximately(enterDuration, .45f)) enterDuration = zoomDuration;
            if (Mathf.Approximately(exitDuration, .65f)) exitDuration = zoomDuration;
        }
        presentationVersion = 1;
    }

    [Header("Coordinated transition")]
    [SerializeField, Min(.001f)] float enterDuration = .45f;
    [SerializeField, Min(.001f)] float exitDuration = .65f;
    [SerializeField] AnimationCurve enterZoomCurve = new AnimationCurve(new Keyframe(0, 0, 3, 3), new Keyframe(1, 1, 0, 0));
    [SerializeField] AnimationCurve exitZoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] AnimationCurve presentationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public static float PresentationWeight { get; private set; }
    static SlayerScreenFX _owner;
    public static bool PresentationDriven => _owner != null;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { PresentationWeight = 0; _owner = null; Shader.SetGlobalFloat("_DungeonSlayerWeight", 0); }
    bool _target;
    float _weight, _zoomWeight, _fromWeight, _fromZoom, _elapsed;
    bool _transitioning;
    Color _hudColor, _trayColor;
    CanvasGroup _caption;
    float _captionAlpha;
    DungeonArenaCamera _arena;
    void BeginPresentation()
    {
        if (_cinematic) return;
        _cinematic = true;
        _hudColor = hudPanel ? hudPanel.color : Color.white;
        _trayColor = typingPanel ? typingPanel.color : Color.white;
        if (scoreCaption)
        {
            _caption = scoreCaption.GetComponent<CanvasGroup>();
            if (!_caption) _caption = scoreCaption.AddComponent<CanvasGroup>();
            _captionAlpha = _caption.alpha;
        }
    }

    void EndPresentation()
    {
        if (!_cinematic) return;
        _cinematic = false;
        if (hudPanel) hudPanel.color = _hudColor;
        if (typingPanel) typingPanel.color = _trayColor;
        if (_caption) _caption.alpha = _captionAlpha;
    }

    void OnEnable() { _owner = this; }

    void OnDisable()
    {
        if (cameraImpact) cameraImpact.ExitSlayer();
        if (_owner == this) _owner = null;
        _target = _transitioning = false;
        _weight = _zoomWeight = PresentationWeight = 0;
        if (_arena) _arena.SetSlayerBackground(Color.clear);
        Shader.SetGlobalFloat("_DungeonSlayerWeight", 0);
        foreach (var group in FindObjectsOfType<SlayerSilhouetteGroup>()) group.SetPresentationWeight(0);
        EndPresentation(); SetBarsInstant(0); SetRedInstant(0); SetZoomInstant(NormalOrtho);
    }

    void Awake()
    {
        MigratePresentationDefaults();
        if (!enterShake) enterShake = GetComponent<CinemachineImpulseSource>();
        if (!vcam) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        _main = Camera.main;
        _arena = _main ? _main.GetComponent<DungeonArenaCamera>() : null;
        _owner = this;
        PresentationWeight = 0;

        SetBarsInstant(0f);
        SetRedInstant(0f);
        SetZoomInstant(NormalOrtho);
    }

    // Gameplay keeps its boolean signals; presentation can finish after they turn off.
    public void EnterSlayerFX() => Request(true);
    public void ExitSlayerFX() => Request(false);

    void Request(bool on)
    {
        if (!isActiveAndEnabled || _target == on) return;
        _target = on;
        BeginPresentation();
        if (on && autoShowOnEnter) SetGroup(true);
        _fromWeight = _weight; _fromZoom = _zoomWeight; _elapsed = 0;
        _transitioning = true;
        if (cameraImpact)
        {
            if (on) cameraImpact.EnterSlayer(enterShake, shakeDelay);
            else cameraImpact.ExitSlayer();
        }
    }

    void Update()
    {
        if (GamePauseController.IsPaused && !GamePauseController.ResultOverlayVisible) return;
        AdvancePresentation(Time.unscaledDeltaTime);
    }

    // Explicit delta makes hitch, reversal and duplicate-request behavior testable.
    internal void AdvancePresentation(float delta)
    {
        if (!_transitioning) return;
        _elapsed += Mathf.Max(0, delta);
        float t = Mathf.Clamp01(_elapsed / Mathf.Max(.001f, _target ? enterDuration : exitDuration));
        float destination = _target ? 1 : 0;
        _weight = Mathf.Lerp(_fromWeight, destination, presentationCurve.Evaluate(t));
        _zoomWeight = Mathf.Lerp(_fromZoom, destination, (_target ? enterZoomCurve : exitZoomCurve).Evaluate(t));
        if (t >= 1) { _weight = _zoomWeight = destination; _transitioning = false; }
        PresentationWeight = _weight;
        Shader.SetGlobalFloat("_DungeonSlayerWeight", _weight);
        SetZoomInstantForTween(Mathf.Lerp(NormalOrtho, SlayerOrtho, _zoomWeight));
        SetBarsInstant(barHeight * _weight);
        if (_arena) _arena.SetSlayerBackground(Color.clear);
        SetRedInstant(0);
        if (hudPanel) { var c = _hudColor; c.a *= 1 - _weight; hudPanel.color = c; }
        if (typingPanel) { var c = _trayColor; c.a *= 1 - _weight; typingPanel.color = c; }
        if (_caption) _caption.alpha = _captionAlpha * (1 - _weight);
        if (!_transitioning && !_target)
        {
            EndPresentation();
            if (autoHideOnExit) SetGroup(false);
        }
    }

    void SetZoomInstantForTween(float ortho)
    {
        // Cinemachine/impact retain ownership of the composed rendered camera.
        if (vcam) vcam.m_Lens.OrthographicSize = ortho;
    }

    // Helpers
    void SetBarsInstant(float height)
    {
        _barReveal = Mathf.Clamp01(height / Mathf.Max(.001f, barHeight));
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
}
