using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class GamePauseController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode toggleKey = KeyCode.Escape;
    [SerializeField] KeyCode restartKey = KeyCode.R;
    [SerializeField] bool pauseOnLoseFocus = true;

    [Header("Camera Zoom (Manual Main Camera)")]
    [Tooltip("Main Camera đang render gameplay.")]
    [SerializeField] Camera targetCamera;
    [Tooltip("CinemachineBrain gắn trên Main Camera (sẽ tắt trong lúc tween).")]
    [SerializeField] CinemachineBrain cinemachineBrain;
    [Tooltip("Tâm cần zoom vào (đặt 1 empty ở giữa màn hình monitor).")]
    [SerializeField] Transform focusTarget;
    [SerializeField] Vector2 focusOffset = Vector2.zero;
    [SerializeField, Min(1f)] float zoomMultiplier = 2f;
    [SerializeField, Range(0.05f, 1.5f)] float zoomDuration = 0.15f;
    [SerializeField] AnimationCurve zoomCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("UI")]
    [Tooltip("Root của toàn bộ UI gameplay (Canvas chính). Trong pause sẽ ẩn.")]
    [SerializeField] RectTransform uiRoot;
    [Tooltip("Canvas/nhãn tách biệt chỉ dùng cho chữ PAUSE + hint.")]
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] TextMeshProUGUI pauseLabel;
    [SerializeField] string pauseText = "PAUSE";
    [SerializeField] Color pauseTextColor = Color.green;
    [SerializeField] TextMeshProUGUI restartHintLabel;
    [SerializeField] string restartHint = "Press R to Restart";

    [Header("Audio")]
    [SerializeField] bool pauseAudioListener = true;

    // runtime
    bool _paused;
    bool _restarting;                 // guard chuỗi restart
    Coroutine _co;
    Vector3 _savedCamPos;
    float _savedOrtho;
    bool _savedBrainEnabled;

    void Reset()
    {
        targetCamera = Camera.main;
        if (!targetCamera) return;
        cinemachineBrain = targetCamera.GetComponent<CinemachineBrain>();
    }

    void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (!cinemachineBrain && targetCamera) cinemachineBrain = targetCamera.GetComponent<CinemachineBrain>();

        if (pauseLabel)
        {
            pauseLabel.text = pauseText;
            pauseLabel.color = pauseTextColor;
        }
        if (restartHintLabel) restartHintLabel.text = restartHint;
        if (pauseCanvas) pauseCanvas.enabled = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!pauseOnLoseFocus) return;
        if (!hasFocus && !_paused) TogglePause(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePause(!_paused);

        if (_paused && Input.GetKeyDown(restartKey))
            TryRestart();
    }

    // ===================== Core =====================
    public void TogglePause(bool pause)
    {
        if (_paused == pause) return;

        // Hủy tween đang chạy (nếu có)
        if (_co != null) { StopCoroutine(_co); _co = null; }

        _paused = pause;

        if (pause)
        {
            // UI ngay lập tức
            if (uiRoot) uiRoot.gameObject.SetActive(false);
            if (pauseCanvas) pauseCanvas.enabled = true;
            if (pauseLabel) { pauseLabel.text = pauseText; pauseLabel.color = pauseTextColor; }
            if (restartHintLabel) restartHintLabel.enabled = true;

            // snapshot camera + tắt brain để tween tay
            if (targetCamera)
            {
                _savedCamPos = targetCamera.transform.position;
                _savedOrtho  = targetCamera.orthographicSize;
            }
            if (cinemachineBrain)
            {
                _savedBrainEnabled = cinemachineBrain.enabled;
                cinemachineBrain.enabled = false;
            }

            // ❗ĐÓNG BĂNG NGAY LẬP TỨC
            Time.timeScale = 0f;
            if (pauseAudioListener) AudioListener.pause = true;

            // Tween zoom-in bằng unscaled
            _co = StartCoroutine(CoTweenCamera(
                fromPos: _savedCamPos,
                toPos: CalcFocusPos(_savedCamPos),
                fromOrtho: _savedOrtho,
                toOrtho: Mathf.Max(0.01f, _savedOrtho / zoomMultiplier),
                dur: zoomDuration,
                curve: zoomCurve,
                then: null));                 // nothing: đã đóng băng ngay từ trên
        }
        else
        {
            // Zoom-out trước (unscaled), xong mới tháo pause
            _co = StartCoroutine(CoTweenCamera(
                fromPos: targetCamera ? targetCamera.transform.position : Vector3.zero,
                toPos: _savedCamPos,
                fromOrtho: targetCamera ? targetCamera.orthographicSize : 5f,
                toOrtho: _savedOrtho,
                dur: zoomDuration,
                curve: zoomCurve,
                then: () =>
                {
                    if (cinemachineBrain) cinemachineBrain.enabled = _savedBrainEnabled;
                    if (pauseCanvas) pauseCanvas.enabled = false;
                    if (uiRoot) uiRoot.gameObject.SetActive(true);

                    if (pauseAudioListener) AudioListener.pause = false;
                    Time.timeScale = 1f;      // ← tháo pause cuối cùng
                }));
        }
    }

    void TryRestart()
    {
        if (_restarting) return;
        _restarting = true;

        // Đang pause sẵn: giữ timeScale=0. Chỉ cần zoom-out như Unpause, sau đó reload scene.
        _co = StartCoroutine(CoTweenCamera(
            fromPos: targetCamera ? targetCamera.transform.position : Vector3.zero,
            toPos: _savedCamPos,
            fromOrtho: targetCamera ? targetCamera.orthographicSize : 5f,
            toOrtho: _savedOrtho,
            dur: zoomDuration,
            curve: zoomCurve,
            then: () =>
            {
                // Ẩn pause canvas để không thấy UI cũ chớp trước khi reset
                if (pauseCanvas) pauseCanvas.enabled = false;

                // Giữ đóng băng trong quá trình reload
                if (pauseAudioListener) AudioListener.pause = false; // tránh giữ pause âm thanh qua scene mới
                Time.timeScale = 1f; // bật về 1 trước khi load để không “mang” pause sang scene mới

                var active = SceneManager.GetActiveScene();
                SceneManager.LoadScene(active.name, LoadSceneMode.Single);
                _restarting = false;
            }));
    }

    Vector3 CalcFocusPos(Vector3 keepZFrom)
    {
        if (!targetCamera) return keepZFrom;
        if (!focusTarget) return keepZFrom;

        Vector3 f = focusTarget.position;
        f.x += focusOffset.x;
        f.y += focusOffset.y;
        f.z = keepZFrom.z; // giữ nguyên Z của camera
        return f;
    }

    IEnumerator CoTweenCamera(Vector3 fromPos, Vector3 toPos, float fromOrtho, float toOrtho,
                              float dur, AnimationCurve curve, System.Action then)
    {
        if (!targetCamera)
        {
            then?.Invoke();
            yield break;
        }

        float t = 0f;
        dur = Mathf.Max(0.0001f, dur);
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / dur;
            float k = curve.Evaluate(Mathf.Clamp01(t));

            targetCamera.transform.position = Vector3.LerpUnclamped(fromPos, toPos, k);
            targetCamera.orthographicSize   = Mathf.LerpUnclamped(fromOrtho, toOrtho, k);
            yield return null;
        }
        targetCamera.transform.position = toPos;
        targetCamera.orthographicSize   = toOrtho;
        then?.Invoke();
        _co = null;
    }
}
