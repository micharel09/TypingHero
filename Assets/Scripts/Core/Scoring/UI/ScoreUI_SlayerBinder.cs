using UnityEngine;

[DisallowMultipleComponent]
public sealed class ScoreUI_SlayerBinder : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Camera targetCam;           // Main Camera (để trống sẽ tự lấy)
    [SerializeField] PlayerSlayerMode slayer;    // PlayerSlayerMode (để trống sẽ tự tìm)
    [SerializeField] RectTransform scoreUIRoot;  // Parent bao ScoreText + ScoreUI_ComboHUD

    [Header("Scale by Camera Ortho")]
    [SerializeField] float scaleMul = 1.00f;     // nhân thêm nếu muốn chữ lớn hơn khi zoom
    [SerializeField] float enterTween = 0.15f;   // tween vào slayer
    [SerializeField] float exitTween = 0.15f;   // tween ra slayer
    [SerializeField] bool liveFollow = true;     // true: cập nhật scale mỗi frame theo cam (zoom động)

    float _baseOrtho;
    Vector3 _baseScale;
    float _tweenUntil;
    Vector3 _fromScale;
    Vector3 _toScale;
    bool _active;

    void Awake()
    {
        if (!targetCam) targetCam = Camera.main;
        if (!slayer) slayer = FindObjectOfType<PlayerSlayerMode>(true);
        if (!scoreUIRoot) scoreUIRoot = transform as RectTransform;

        _baseScale = scoreUIRoot ? scoreUIRoot.localScale : Vector3.one;
        _baseOrtho = targetCam ? targetCam.orthographicSize : 5f;

        SlayerModeSignals.OnSetActive -= OnSlayerToggle;
        SlayerModeSignals.OnSetActive += OnSlayerToggle;

        // set ngay trạng thái ban đầu
        OnSlayerToggle(slayer && slayer.IsActive);
    }

    void OnDestroy() => SlayerModeSignals.OnSetActive -= OnSlayerToggle;

    void OnSlayerToggle(bool on)
    {
        _active = on;
        // mục tiêu scale tức thời (không chờ frame)
        float k = CalcScaleNow();
        Vector3 target = _baseScale * k;

        _fromScale = scoreUIRoot ? scoreUIRoot.localScale : Vector3.one;
        _toScale   = target;

        float d = on ? Mathf.Max(0.01f, enterTween) : Mathf.Max(0.01f, exitTween);
        _tweenUntil = Time.unscaledTime + d;
        enabled = true;
    }

    void Update()
    {
        if (!scoreUIRoot) return;

        // Option: follow liên tục theo camera (khi zoom động hoặc shake)
        if (liveFollow && _active)
        {
            float kLive = CalcScaleNow();
            _toScale = _baseScale * kLive;
        }

        float remain = _tweenUntil - Time.unscaledTime;
        if (remain <= 0f)
        {
            scoreUIRoot.localScale = _toScale;
            enabled = liveFollow && _active; // nếu follow sống, giữ Update; còn không thì tắt
            return;
        }

        float total = (_active ? enterTween : exitTween);
        float t = 1f - Mathf.Clamp01(remain / Mathf.Max(0.0001f, total));
        t = 1f - (1f - t) * (1f - t); // easeOutQuad
        scoreUIRoot.localScale = Vector3.LerpUnclamped(_fromScale, _toScale, t);
    }

    float CalcScaleNow()
    {
        if (!targetCam) return 1f;
        float cur = Mathf.Max(0.0001f, targetCam.orthographicSize);
        // zoom in (orthographicSize giảm) → UI to ra; zoom out → UI nhỏ lại
        return (_baseOrtho / cur) * scaleMul;
    }
}
