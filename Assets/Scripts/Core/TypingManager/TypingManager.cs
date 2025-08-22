using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TypingManager : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] TextMeshProUGUI wordText;        // current word
    [SerializeField] TextMeshProUGUI nextWordText;    // next preview
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Slider countdownSlider;

    [Header("Timing")]
    [SerializeField, Range(0.8f, 3f)] float attackTimeLimit = 2.2f;

    [Header("Word Source")]
    [SerializeField] WordPoolConfig wordPool;
    [SerializeField, Tooltip("Nếu pool rỗng thì dùng từ này (để không bao giờ 'đơ').")]
    string fallbackWord = "type";

    [Header("Typing Test Mode")]
    [SerializeField] bool startActive = true;
    [SerializeField] KeyCode toggleTestKey = KeyCode.BackQuote;
    [SerializeField] bool hideUIWhenInactive = true;

    [Header("Animation (feel)")]
    [SerializeField] float charPopScale = 1.08f;
    [SerializeField] float charPopDuration = 0.06f;
    [SerializeField] float advanceSlidePixels = 24f;
    [SerializeField] float advanceSlideDuration = 0.08f;
    [SerializeField] float advanceFadeDuration = 0.06f;
    [SerializeField] bool uiUseUnscaledTime = true;

    [Header("Debug")]
    [SerializeField] bool logs;

    // Events (giữ nguyên API)
    public static bool MuteWordCorrect = false;
    public static event Action OnWordCorrect;
    public static event Action OnWordTimeout;
    public static event Action OnWordAdvanced;
    public static event Action<char> OnCorrectChar;

    bool typingTestActive;
    bool isTransitioning;
    string current = "";
    string next = "";
    float timer = 0f;

    // anim helpers
    RectTransform _curRT, _nextRT;
    CanvasGroup _curCG, _nextCG;
    Vector3 _curBaseScale, _nextBaseScale;
    Vector2 _curBaseAnchored, _nextBaseAnchored;
    Coroutine _advanceCo, _popCo;

    void Awake()
    {
        if (wordPool) wordPool.BuildCache();

        if (wordText) _curRT = wordText.rectTransform;
        if (nextWordText) _nextRT = nextWordText.rectTransform;

        _curCG = EnsureCanvasGroup(wordText);
        _nextCG = EnsureCanvasGroup(nextWordText);

        if (_curRT) { _curBaseScale = _curRT.localScale; _curBaseAnchored = _curRT.anchoredPosition; }
        if (_nextRT) { _nextBaseScale = _nextRT.localScale; _nextBaseAnchored = _nextRT.anchoredPosition; }
    }

    void Start()
    {
        typingTestActive = startActive;
        ApplyUIVisibility();
        if (typingTestActive)
        {
            ForceInitialWords();
            FocusInput();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleTestKey))
        {
            typingTestActive = !typingTestActive;
            ApplyUIVisibility();

            if (typingTestActive)
            {
                ForceInitialWords();
                FocusInput();
            }
            else if (inputField) inputField.DeactivateInputField();
        }

        if (!typingTestActive) return;

        HandleTimer();
        if (!isTransitioning) HandleTyping();   // khóa input khi đang chuyển cảnh
    }

    // ================= Core =================
    void HandleTimer()
    {
        timer -= Time.deltaTime;

        if (countdownSlider)
            countdownSlider.value = Mathf.Clamp01(timer / attackTimeLimit);

        if (timer <= 0f)
        {
            if (logs) Debug.Log("[Typing] TIMEOUT");
            OnWordTimeout?.Invoke();
            AdvanceToNext();
        }
    }

    void HandleTyping()
    {
        var s = Input.inputString;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (!char.IsLetter(c)) continue;
            if (string.IsNullOrEmpty(current)) break;

            // luôn đánh ký tự đầu
            if (char.ToLowerInvariant(c) == char.ToLowerInvariant(current[0]))
            {
                OnCorrectChar?.Invoke(c);

                // UI punch nhỏ cho current
                if (_popCo != null) StopCoroutine(_popCo);
                _popCo = StartCoroutine(PunchScale(_curRT, _curBaseScale, charPopScale, charPopDuration));

                // Xoá ký tự đầu
                current = (current.Length > 1) ? current.Substring(1) : "";
                if (wordText) wordText.text = current;

                if (current.Length == 0)
                {
                    if (!MuteWordCorrect) OnWordCorrect?.Invoke();
                    OnWordAdvanced?.Invoke();
                    AdvanceToNext();
                }
            }
        }
    }

    void ForceInitialWords()
    {
        current = GetWordOrFallback();
        next = GetWordOrFallback();

        timer = attackTimeLimit;

        if (wordText) wordText.text = current;
        if (nextWordText) nextWordText.text = next;

        ResetCurrentVisual();
        ResetNextVisual();

        if (inputField) inputField.text = "";
    }

    void AdvanceToNext()
    {
        // đảm bảo có next trước khi chuyển
        if (string.IsNullOrEmpty(next))
            next = GetWordOrFallback();

        // Animate chuyển cảnh
        if (_advanceCo != null) StopCoroutine(_advanceCo);
        _advanceCo = StartCoroutine(AdvanceTransitionSafe());
    }

    IEnumerator AdvanceTransitionSafe()
    {
        isTransitioning = true;

        // prep: next ở phải + mờ
        if (_nextRT)
        {
            _nextRT.anchoredPosition = _nextBaseAnchored + new Vector2(advanceSlidePixels, 0f);
            _nextRT.localScale = _nextBaseScale;
        }
        if (_nextCG) _nextCG.alpha = 0f;

        float t = 0f;
        float slideT = Mathf.Max(0.0001f, advanceSlideDuration);
        float fadeT = Mathf.Max(0.0001f, advanceFadeDuration);

        while (t < Mathf.Max(slideT, fadeT))
        {
            float dt = uiUseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;

            float kSlide = Mathf.Clamp01(t / slideT);
            float kFade = Mathf.Clamp01(t / fadeT);

            if (_curRT)
                _curRT.anchoredPosition = Vector2.Lerp(_curBaseAnchored, _curBaseAnchored + new Vector2(-advanceSlidePixels, 0f), Smooth(kSlide));
            if (_curCG)
                _curCG.alpha = 1f - kFade;

            if (_nextRT)
                _nextRT.anchoredPosition = Vector2.Lerp(_nextBaseAnchored + new Vector2(advanceSlidePixels, 0f), _nextBaseAnchored, Smooth(kSlide));
            if (_nextCG)
                _nextCG.alpha = kFade;

            yield return null;
        }

        // Swap
        current = next;
        if (wordText) wordText.text = current;

        // Prefetch next mới (bất kể pool có hay không → luôn có fallback)
        next = GetWordOrFallback();
        if (nextWordText) nextWordText.text = next;

        // Reset visuals về base (để chắc chắn không bị 'mờ đứng')
        ResetCurrentVisual();
        ResetNextVisual();

        // Reset timer & input
        timer = attackTimeLimit;
        if (inputField) inputField.text = "";
        FocusInput();

        isTransitioning = false;
        _advanceCo = null;
    }

    // ================= Helpers =================
    string GetWordOrFallback()
    {
        string w = null;
        if (wordPool != null) w = wordPool.GetRandomWord();
        if (string.IsNullOrEmpty(w)) w = string.IsNullOrEmpty(fallbackWord) ? "type" : fallbackWord;
        return w;
    }

    IEnumerator PunchScale(RectTransform rt, Vector3 baseScale, float peakMul, float duration)
    {
        if (!rt || duration <= 0f || peakMul <= 1f) yield break;

        float half = duration * 0.5f;
        float t = 0f;

        // up
        while (t < half)
        {
            float dt = uiUseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;
            float k = Mathf.Clamp01(t / half);
            float s = Mathf.Lerp(1f, peakMul, Smooth(k));
            rt.localScale = baseScale * s;
            yield return null;
        }

        // down
        t = 0f;
        while (t < half)
        {
            float dt = uiUseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;
            float k = Mathf.Clamp01(t / half);
            float s = Mathf.Lerp(peakMul, 1f, Smooth(k));
            rt.localScale = baseScale * s;
            yield return null;
        }
        rt.localScale = baseScale;
    }

    static float Smooth(float x) => x * x * (3f - 2f * x); // smoothstep

    CanvasGroup EnsureCanvasGroup(TextMeshProUGUI tmp)
    {
        if (!tmp) return null;
        if (!tmp.TryGetComponent<CanvasGroup>(out var cg))
            cg = tmp.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    void ResetCurrentVisual()
    {
        if (_curRT)
        {
            _curRT.localScale = _curBaseScale;
            _curRT.anchoredPosition = _curBaseAnchored;
        }
        if (_curCG) _curCG.alpha = 1f;
    }

    void ResetNextVisual()
    {
        if (_nextRT)
        {
            _nextRT.localScale = _nextBaseScale;
            _nextRT.anchoredPosition = _nextBaseAnchored;
        }
        if (_nextCG) _nextCG.alpha = 1f;
    }

    void FocusInput()
    {
        if (!inputField) return;
        inputField.ActivateInputField();
        inputField.caretPosition = inputField.text.Length;
    }

    void ApplyUIVisibility()
    {
        bool show = typingTestActive || !hideUIWhenInactive;
        if (wordText) wordText.gameObject.SetActive(show);
        if (nextWordText) nextWordText.gameObject.SetActive(show);
        if (countdownSlider) countdownSlider.gameObject.SetActive(show);
        if (inputField) inputField.gameObject.SetActive(show);
    }
}
