using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // thêm dòng này ở đầu file
using TMPro;
using Cinemachine;

[DisallowMultipleComponent]
public sealed class GamePauseController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode toggleKey = KeyCode.Escape;
    [SerializeField] KeyCode restartKey = KeyCode.R;
    [SerializeField] KeyCode menuKey = KeyCode.M;           // NEW
    [SerializeField] bool pauseOnLoseFocus = true;

    [Header("Camera Zoom (Manual Main Camera)")]
    [SerializeField] Camera targetCamera;
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Transform focusTarget;
    [SerializeField] Vector2 focusOffset = Vector2.zero;
    [SerializeField, Min(1f)] float zoomMultiplier = 2f;
    [SerializeField, Range(0.05f, 1.5f)] float zoomDuration = 0.15f;
    [SerializeField] AnimationCurve zoomCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("UI (Pause Canvas)")]
    [SerializeField] RectTransform uiRoot;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] TextMeshProUGUI pauseLabel;
    [SerializeField] TextMeshProUGUI restartHintLabel;
    [SerializeField] TextMeshProUGUI menuHintLabel;            // NEW
    [SerializeField] string pauseText = "PAUSE";
    [SerializeField] Color pauseTextColor = Color.green;

    [Header("Main Menu")]
    [SerializeField] string menuSceneName = "MainMenu";        // NEW

    [Header("Game Over")]
    [SerializeField] bool autoGameOverOnPlayerDie = true;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] string gameOverText = "GAME OVER";
    [SerializeField] Color gameOverTextColor = new Color(1f, .2f, .2f, 1f);

    [Header("Thanks (Enemy Die)")]
    [SerializeField] TextMeshProUGUI thanksLabel;
    [SerializeField] string thanksText = "THANKS FOR PLAYING DEMO";
    [SerializeField] Color thanksTextColor = new Color(0.25f, 1f, 0.25f, 1f);

    [Header("Audio")]
    [SerializeField] bool pauseAudioListener = true;          // đã có
    [SerializeField] bool stopAudioOnReturnMenu = true;        // NEW
    [SerializeField, Range(0f, 1f)] float stopAudioFade = 0.15f; // NEW

    bool _paused;
    bool _overlayLocked;
    bool _restarting;
    Vector3 _camPosSaved;
    float _orthoSaved;
    bool _brainSaved;

    public bool IsRestarting => _restarting;

    // ---- NEW: helper guard để tránh chạy logic trên instance đã destroy/disabled
    bool Usable()
    {
        // Unity-null check
        if (!this) return false;
        if (!isActiveAndEnabled) return false;
        return true;
    }

    void Reset()
    {
        targetCamera = Camera.main;
        if (targetCamera) cinemachineBrain = targetCamera.GetComponent<CinemachineBrain>();
        if (string.IsNullOrEmpty(menuSceneName)) menuSceneName = "MainMenu";
    }

    void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (!cinemachineBrain && targetCamera) cinemachineBrain = targetCamera.GetComponent<CinemachineBrain>();

        if (pauseCanvas) pauseCanvas.enabled = false;
        if (pauseLabel) { pauseLabel.text = pauseText; pauseLabel.color = pauseTextColor; pauseLabel.gameObject.SetActive(false); }
        if (restartHintLabel) restartHintLabel.enabled = false;
        if (menuHintLabel) menuHintLabel.enabled = false;      // NEW
        if (thanksLabel) { thanksLabel.text = thanksText; thanksLabel.color = thanksTextColor; thanksLabel.gameObject.SetActive(false); }

        // đảm bảo có playerHealth và subscribe
        BindPlayerHealthIfNeeded();
    }

    void OnDestroy()
    {
        if (autoGameOverOnPlayerDie && playerHealth)
            playerHealth.OnDied -= ShowGameOverAfterPlayerDie;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (_overlayLocked) return;
        if (!pauseOnLoseFocus) return;
        if (!hasFocus && !_paused) TogglePause(true);
    }

    void Update()
    {
        if (_overlayLocked)
        {
            if (Input.GetKeyDown(restartKey)) TryRestart();
            if (Input.GetKeyDown(menuKey)) { ReturnToMenu(); return; }   // đã OK cho GameOver/Thanks
            return;
        }

        if (Input.GetKeyDown(toggleKey)) TogglePause(!_paused);

        if (_paused)
        {
            if (Input.GetKeyDown(restartKey)) TryRestart();
            if (Input.GetKeyDown(menuKey)) ReturnToMenu();               // 🔴 THÊM DÒNG NÀY
        }
    }


    // ===== PUBLIC API =====
    public void ShowGameOverAfterPlayerDie()
    {
        // ---- NEW GUARD ----
        if (!Usable()) return;
        if (_overlayLocked || _restarting) return;
        _overlayLocked = true;
        ShowOverlay_PauseLabel(gameOverText, gameOverTextColor, showRestart: true);
    }

    public void ShowThanksSimple()
    {
        // ---- NEW GUARD ----
        if (!Usable()) return;
        if (_overlayLocked || _restarting) return;
        _overlayLocked = true;
        ShowOverlay_ThanksLabel(showRestart: true);
    }

    public void ReturnToMenu()
    {
        if (_restarting) return;         // dùng chung flag tránh spam
        _restarting = true;

        ZoomOutThen(() =>
        {
            // khôi phục trạng thái như Restart
            if (pauseCanvas) pauseCanvas.enabled = false;
            if (pauseAudioListener) AudioListener.pause = false;
            Time.timeScale = 1f;
            if (cinemachineBrain) cinemachineBrain.enabled = _brainSaved;

            // fade/stop audio trước khi load menu hoặc load trực tiếp
            if (stopAudioOnReturnMenu)
                StartCoroutine(CoFadeStopThenLoad());
            else
                LoadMenuNow();
        });
    }

    // ===== PAUSE CORE =====
    public void TogglePause(bool pause)
    {
        if (_overlayLocked || _restarting) return;
        if (_paused == pause) return;

        _paused = pause;
        if (pause)
            ShowOverlay_PauseLabel(pauseText, pauseTextColor, showRestart: true);
        else
            ZoomOutThen(() =>
            {
                if (cinemachineBrain) cinemachineBrain.enabled = _brainSaved;
                if (pauseCanvas) pauseCanvas.enabled = false;
                if (uiRoot) uiRoot.gameObject.SetActive(true);

                if (thanksLabel) thanksLabel.gameObject.SetActive(false);
                if (pauseLabel) pauseLabel.gameObject.SetActive(false);
                if (restartHintLabel) restartHintLabel.enabled = false;
                if (menuHintLabel) menuHintLabel.enabled = false; // NEW

                if (pauseAudioListener) AudioListener.pause = false;
                Time.timeScale = 1f;
            });
    }

    // ===== OVERLAY HELPERS =====
    void ShowOverlay_PauseLabel(string text, Color color, bool showRestart)
    {
        if (uiRoot) uiRoot.gameObject.SetActive(false);
        if (pauseCanvas) pauseCanvas.enabled = true;

        if (thanksLabel) thanksLabel.gameObject.SetActive(false);
        if (pauseLabel) { pauseLabel.text = text; pauseLabel.color = color; pauseLabel.gameObject.SetActive(true); }
        if (restartHintLabel) restartHintLabel.enabled = true;
        if (menuHintLabel) menuHintLabel.enabled    = true; // NEW

        SnapshotCam(); Freeze(true); ZoomIn();
    }

    void ShowOverlay_ThanksLabel(bool showRestart)
    {
        if (uiRoot) uiRoot.gameObject.SetActive(false);
        if (pauseCanvas) pauseCanvas.enabled = true;

        if (pauseLabel) pauseLabel.gameObject.SetActive(false);
        if (thanksLabel) { thanksLabel.text = thanksText; thanksLabel.color = thanksTextColor; thanksLabel.gameObject.SetActive(true); }
        if (restartHintLabel) restartHintLabel.enabled = true;
        if (menuHintLabel) menuHintLabel.enabled    = true; // NEW

        SnapshotCam(); Freeze(true); ZoomIn();
    }

    // ===== RESTART =====
    void TryRestart()
    {
        if (_restarting) return;
        _restarting = true;

        ZoomOutThen(() =>
        {
            if (pauseCanvas) pauseCanvas.enabled = false;
            if (pauseAudioListener) AudioListener.pause = false;

            SceneManager.sceneLoaded += OnSceneLoaded;
            var sc = SceneManager.GetActiveScene();
            SceneManager.LoadScene(sc.name, LoadSceneMode.Single);
        });
    }

    void OnSceneLoaded(Scene sc, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Bind lại playerHealth (trường hợp object cũ bị Destroy hoặc dùng DontDestroyOnLoad)
        BindPlayerHealthIfNeeded();
        if (playerHealth) playerHealth.FullRestore();     // reset HP về max

        Time.timeScale = 1f;
        _restarting = false;
        _overlayLocked = false;
        _paused = false;
        if (cinemachineBrain) cinemachineBrain.enabled = _brainSaved;
    }

    // ===== AUDIO HELPERS =====
    IEnumerator CoFadeStopAllAudio(float dur)
    {
        var sources = FindObjectsOfType<AudioSource>(true);
        int n = sources.Length;
        var vols = new float[n];
        for (int i = 0; i < n; i++)
        {
            if (!sources[i]) continue;
            vols[i] = sources[i].volume;
            sources[i].ignoreListenerPause = false; // đảm bảo chịu ảnh hưởng pause/resume
        }

        if (dur > 0f)
        {
            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float k = 1f - Mathf.Clamp01(t / dur);
                for (int i = 0; i < n; i++) if (sources[i]) sources[i].volume = vols[i] * k;
                yield return null;
            }
        }

        for (int i = 0; i < n; i++)
        {
            if (!sources[i]) continue;
            sources[i].Stop();
            sources[i].volume = vols[i]; // trả về volume cũ để lần sau phát đúng mức
        }
    }

    void LoadMenuNow()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(0, LoadSceneMode.Single); // fallback: index 0
    }

    IEnumerator CoFadeStopThenLoad()
    {
        yield return CoFadeStopAllAudio(stopAudioFade);
        LoadMenuNow();
    }

    // ===== CAMERA & FREEZE =====
    void SnapshotCam()
    {
        if (targetCamera)
        {
            _camPosSaved = targetCamera.transform.position;
            _orthoSaved  = targetCamera.orthographicSize;
        }
        if (cinemachineBrain)
        {
            _brainSaved = cinemachineBrain.enabled;
            cinemachineBrain.enabled = false;
        }
    }

    void Freeze(bool on)
    {
        if (on) { Time.timeScale = 0f; if (pauseAudioListener) AudioListener.pause = true; }
        else { if (pauseAudioListener) AudioListener.pause = false; Time.timeScale = 1f; }
    }

    void ZoomIn()
    {
        StartCoroutine(CoTweenCam(_camPosSaved, CalcFocus(_camPosSaved),
            _orthoSaved, Mathf.Max(0.01f, _orthoSaved / zoomMultiplier), zoomDuration, zoomCurve, null));
    }

    void ZoomOutThen(System.Action then)
    {
        StartCoroutine(CoTweenCam(
            targetCamera ? targetCamera.transform.position : Vector3.zero,
            _camPosSaved,
            targetCamera ? targetCamera.orthographicSize : 5f,
            _orthoSaved,
            zoomDuration, zoomCurve,
            () => { Freeze(false); then?.Invoke(); }));
    }

    Vector3 CalcFocus(Vector3 keepZFrom)
    {
        if (!targetCamera || !focusTarget) return keepZFrom;
        var f = focusTarget.position;
        f.x += focusOffset.x; f.y += focusOffset.y; f.z = keepZFrom.z;
        return f;
    }

    IEnumerator CoTweenCam(Vector3 fromPos, Vector3 toPos, float fromOrtho, float toOrtho,
                           float dur, AnimationCurve curve, System.Action then)
    {
        if (!targetCamera) { then?.Invoke(); yield break; }
        dur = Mathf.Max(0.0001f, dur);
        float t = 0f;
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
    }

    // ===== UTIL =====
    void BindPlayerHealthIfNeeded()
    {
        if (!autoGameOverOnPlayerDie) return;

        // nếu thiếu reference, tìm trong scene
        if (!playerHealth)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null) return;

        // tránh double-subscribe
        playerHealth.OnDied -= ShowGameOverAfterPlayerDie;
        playerHealth.OnDied += ShowGameOverAfterPlayerDie;
    }
}