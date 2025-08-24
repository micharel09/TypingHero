using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TypingManager : MonoBehaviour
{
    // ===================== Refs =====================
    [Header("Rail (container ở giữa) + UI Refs")]
    [SerializeField] RectTransform rail;              // mốc giữa (0,0)
    [SerializeField] TextMeshProUGUI wordText;        // current
    [SerializeField] TextMeshProUGUI nextWordText;    // next
    [SerializeField] TextMeshProUGUI thirdWordText;   // third
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Slider countdownSlider;

    [Header("Timing")]
    [SerializeField, Range(0.8f, 3f)] float attackTimeLimit = 2.2f;

    [Header("Word Source")]
    [SerializeField] WordPoolConfig wordPool;
    [SerializeField] string fallbackWord = "type";

    [Header("Layout")]
    [SerializeField, Tooltip("Khoảng cách chuẩn giữa đuôi từ trước → đầu từ sau (px)")]
    float nextGapPixels = 36f;
    [SerializeField, Tooltip("Cộng thêm khi current còn chữ (anti-overlap)")]
    float guardGapWhileTyping = 6f;
    [SerializeField] bool pixelSnap = true;

    [Header("Follow while typing")]
    [SerializeField, Tooltip("Thời gian SmoothDamp (giảm = bám nhanh hơn)")]
    float followSmoothTime = 0.06f;

    [Header("Visual (alpha idle)")]
    [Range(0f, 1f)] public float nextIdleAlpha = 1f;
    [Range(0f, 1f)] public float thirdIdleAlpha = 0.42f; // third mờ hơn

    [Header("Promote (next → current)")]
    [SerializeField] float promoteHold = 0.05f;     // giữ mắt 1 nhịp
    [SerializeField] float promoteDuration = 0.16f; // chậm, dễ đọc
    [SerializeField] float promoteScalePunch = 1.05f;

    [Header("Parallax spawn (sau swap)")]
    [SerializeField] float spawnDistNext = 60f;     // next gần
    [SerializeField] float spawnDistThird = 160f;   // third xa
    [SerializeField] float spawnFadeDur = 0.14f;

    [Header("Spawn throttle by WIDTH (px, sqrt)")]
    [SerializeField] bool spawnThrottleByWidth = true;
    [SerializeField, Tooltip("Base duration (s)")]
    float spawnBase = 0.16f;
    [SerializeField, Tooltip("Hệ số cho sqrt(width)")]
    float spawnCoefPxSqrt = 0.04f;
    [SerializeField, Tooltip("Clamp min/max cho spawn duration")]
    Vector2 spawnDurClamp = new Vector2(0.12f, 0.48f);

    public enum Ease { CubicOut, QuintOut, ExpoOut }
    [Header("Spawn easing")]
    [SerializeField] Ease spawnEaseNext = Ease.CubicOut;
    [SerializeField] Ease spawnEaseThird = Ease.QuintOut;

    [Header("Disable Follow trong tween")]
    [SerializeField] bool disableFollowDuringPromote = true;
    [SerializeField] bool disableFollowDuringSpawn = true;

    [Header("Hiệu ứng khác")]
    [SerializeField] float charPopScale = 1.08f;
    [SerializeField] float charPopDuration = 0.06f;
    [SerializeField] float currentFadeOut = 0.06f;  // khi kết thúc current
    [SerializeField] float currentSlideLeft = 24f;
    [SerializeField] bool uiUseUnscaledTime = true;

    [Header("Type-ahead (gõ trước khi promote xong)")]
    [SerializeField] bool allowTypeAhead = true;

    [Header("Error Feedback (flash đỏ khi sai)")]
    [SerializeField] bool enableErrorFlash = true;
    [SerializeField] Color errorFlashColor = new Color(1f, 0.25f, 0.25f, 1f);
    [SerializeField, Tooltip("Tổng thời gian nháy (đỏ rồi trả về)")]
    float errorFlashDuration = 0.12f;

    [Header("Debug")][SerializeField] bool logs;

    // ===================== Events (giữ API) =====================
    public static bool MuteWordCorrect = false;
    public static event Action OnWordCorrect;
    public static event Action OnWordTimeout;
    public static event Action OnWordAdvanced;
    public static event Action<char> OnCorrectChar;

    // ===================== Runtime =====================
    bool isTransitioning, pendingAdvance;
    string current = "", next = "", third = "";
    float timer;

    RectTransform _curRT, _nextRT, _thirdRT;
    CanvasGroup _curCG, _nextCG, _thirdCG;
    Vector3 _curBaseScale, _nextBaseScale, _thirdBaseScale;

    // colors gốc để flash/khôi phục
    Color _curBaseColor, _nextBaseColor, _thirdBaseColor;

    // Follow state
    float _nextVelX, _thirdVelX;
    Vector2 _nextTarget, _thirdTarget;
    bool _suspendFollowNext, _suspendFollowThird;

    // Type-ahead state
    int _typeAheadCount = 0;

    // coroutines
    Coroutine _advanceCo, _popCo, _spawn2Co, _spawn3Co;
    Coroutine _errCoCur, _errCoNext;

    float DT => uiUseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    void Awake()
    {
        if (wordPool) wordPool.BuildCache();

        _curRT   = wordText.rectTransform;
        _nextRT  = nextWordText.rectTransform;
        _thirdRT = thirdWordText.rectTransform;

        Debug.Assert(_curRT.parent == rail && _nextRT.parent == rail && _thirdRT.parent == rail,
            "[Typing] 3 TMP phải là child của 'rail' (container giữa).");

        _curCG   = EnsureCG(wordText);
        _nextCG  = EnsureCG(nextWordText);
        _thirdCG = EnsureCG(thirdWordText);

        _curBaseScale   = _curRT.localScale;
        _nextBaseScale  = _nextRT.localScale;
        _thirdBaseScale = _thirdRT.localScale;

        _curBaseColor   = wordText.color;
        _nextBaseColor  = nextWordText.color;
        _thirdBaseColor = thirdWordText.color;
    }

    void Start() { ForceInitial(); }

    void Update()
    {
        // CHẠY LUÔN để type-ahead hoạt động cả khi promote/spawn
        HandleTyping();

        if (!isTransitioning) HandleTimer();
        if (!isTransitioning) FollowTargets();
    }

    // ===================== Core =====================
    void HandleTimer()
    {
        timer -= Time.deltaTime;
        if (countdownSlider) countdownSlider.value = Mathf.Clamp01(timer / attackTimeLimit);

        if (timer <= 0f && !pendingAdvance)
        {
            pendingAdvance = true;
            OnWordTimeout?.Invoke();
            timer = attackTimeLimit;
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

            // Nếu current còn chữ: xử lý current
            if (!string.IsNullOrEmpty(current))
            {
                if (char.ToLowerInvariant(c) == char.ToLowerInvariant(current[0]))
                {
                    OnCorrectChar?.Invoke(c);

                    if (_popCo != null) StopCoroutine(_popCo);
                    _popCo = StartCoroutine(PunchScale(_curRT, _curBaseScale, charPopScale, charPopDuration));

                    current = (current.Length > 1) ? current.Substring(1) : "";
                    wordText.text = current;

                    UpdateTargets(); // để next/third bám theo width mới

                    if (current.Length == 0 && !pendingAdvance)
                    {
                        if (!MuteWordCorrect) OnWordCorrect?.Invoke();
                        OnWordAdvanced?.Invoke();
                        pendingAdvance = true;
                        AdvanceToNext();
                    }
                }
                else
                {
                    // Sai ký tự khi đang gõ current
                    TriggerErrorFlash(wordText, ref _errCoCur, _curBaseColor);
                }
                continue;
            }

            // current đã hết -> thử ăn input cho NEXT (type-ahead)
            if (!TryConsumeTypeAhead(c))
            {
                // Sai ký tự khi type-ahead next
                TriggerErrorFlash(nextWordText, ref _errCoNext, _nextBaseColor);
            }
        }
    }

    bool TryConsumeTypeAhead(char c)
    {
        if (!allowTypeAhead) return false;
        if (string.IsNullOrEmpty(next)) return false;

        int idx = _typeAheadCount;
        if (idx >= next.Length) return false;

        if (char.ToLowerInvariant(c) == char.ToLowerInvariant(next[idx]))
        {
            _typeAheadCount++;
            OnCorrectChar?.Invoke(c);
            return true;
        }
        return false;
    }

    void ForceInitial()
    {
        pendingAdvance = isTransitioning = false;

        current = GetWord();
        next    = GetWord();
        third   = GetWord();

        timer = attackTimeLimit;

        wordText.text      = current;
        nextWordText.text  = next;
        thirdWordText.text = third;

        _curRT.anchoredPosition = Vector2.zero;
        _curRT.localScale       = _curBaseScale;

        if (_curCG) _curCG.alpha   = 1f;
        if (_nextCG) _nextCG.alpha  = nextIdleAlpha;
        if (_thirdCG) _thirdCG.alpha = thirdIdleAlpha;

        // reset flash if any
        StopErrorFlash(ref _errCoCur, wordText, _curBaseColor);
        StopErrorFlash(ref _errCoNext, nextWordText, _nextBaseColor);

        _nextVelX = _thirdVelX = 0f;
        _typeAheadCount = 0;

        UpdateTargets(immediate: true);

        if (inputField) { inputField.text = ""; inputField.ActivateInputField(); }
    }

    // ===================== Advance (3 từ) =====================
    void AdvanceToNext()
    {
        if (_advanceCo != null) StopCoroutine(_advanceCo);
        _advanceCo = StartCoroutine(Advance3());
    }

    IEnumerator Advance3()
    {
        isTransitioning = true;

        // 1) CURRENT fade-out + lùi trái nhẹ
        Vector2 curFrom = _curRT.anchoredPosition;
        Vector2 curTo = curFrom + new Vector2(-currentSlideLeft, 0f);
        float t = 0f, fadeT = Mathf.Max(0.001f, currentFadeOut);
        while (t < fadeT)
        {
            t += DT;
            float k = Smooth01(t / fadeT);
            _curRT.anchoredPosition = LerpSnap(curFrom, curTo, k);
            if (_curCG) _curCG.alpha = 1f - k;
            yield return null;
        }

        // 2) HOLD nhẹ
        if (promoteHold > 0f) yield return new WaitForSecondsRealtime(promoteHold);

        // 3) PROMOTE: next -> giữa; third -> vị trí next mới
        if (disableFollowDuringPromote) { _suspendFollowNext = true; _suspendFollowThird = true; }

        Vector2 secFrom = _nextRT.anchoredPosition;
        Vector2 secTo = Vector2.zero; // giữa rail

        float wNext = Measure(nextWordText);
        float wThird = Measure(thirdWordText);

        Vector2 thirdFrom = _thirdRT.anchoredPosition;
        Vector2 thirdTo = new Vector2(
            (wNext * 0.5f) + nextGapPixels + (wThird * 0.5f),
            0f);

        Vector3 secBaseScale = _nextBaseScale;

        float pT = 0f, pDur = Mathf.Max(0.001f, promoteDuration);
        while (pT < pDur)
        {
            pT += DT;
            float k = EaseOutCubic(pT / pDur);

            _nextRT.anchoredPosition  = LerpSnap(secFrom, secTo, k);
            _thirdRT.anchoredPosition = LerpSnap(thirdFrom, thirdTo, Smooth01(k));

            if (_nextCG) _nextCG.alpha  = Mathf.Lerp(nextIdleAlpha, 1f, k);
            if (_thirdCG) _thirdCG.alpha = thirdIdleAlpha;

            _nextRT.localScale = secBaseScale * Mathf.Lerp(promoteScalePunch, 1f, k);
            yield return null;
        }

        if (disableFollowDuringPromote) { _suspendFollowNext = false; _suspendFollowThird = false; }

        // 4) SWAP nội dung (second đang ở giữa)
        int carry = _typeAheadCount;              // số ký tự đã gõ của 'next'
        _typeAheadCount = 0;

        current = next;
        next    = third;
        third   = GetWord();

        // ÁP dụng tiến độ type-ahead lên current mới
        if (allowTypeAhead && carry > 0)
        {
            if (carry >= (current?.Length ?? 0))
            {
                // gõ xong toàn bộ 'next' trong lúc promote -> chain advance ngay
                current = "";
                wordText.text = "";

                if (!MuteWordCorrect) OnWordCorrect?.Invoke();
                OnWordAdvanced?.Invoke();

                pendingAdvance = true;
                isTransitioning = false;
                AdvanceToNext();
                yield break;
            }
            else
            {
                current = current.Substring(carry);
            }
        }

        // update text sau khi áp carry
        wordText.text      = current;
        nextWordText.text  = next;
        thirdWordText.text = third;

        // reset color nếu có flash dang dở
        StopErrorFlash(ref _errCoCur, wordText, _curBaseColor);
        StopErrorFlash(ref _errCoNext, nextWordText, _nextBaseColor);

        // Reset current ở giữa
        _curRT.anchoredPosition = Vector2.zero;
        _curRT.localScale       = _curBaseScale;
        if (_curCG) _curCG.alpha = 1f;

        // 5) Đồng bộ TARGET mới (Follow & Spawn dùng chung đích)
        Vector2 nextTargetNew = CalcNextTarget();
        Vector2 thirdTargetNew = CalcThirdTarget(nextTargetNew);
        _nextTarget  = PixelSnap(nextTargetNew);
        _thirdTarget = PixelSnap(thirdTargetNew);

        // 6) Parallax spawn (throttle theo width sqrt + tắt Follow trong Spawn)
        if (_spawn2Co != null) StopCoroutine(_spawn2Co);
        if (_spawn3Co != null) StopCoroutine(_spawn3Co);

        float durNext = ComputeSpawnDurationPx(nextWordText, spawnBase, spawnCoefPxSqrt, spawnDurClamp);
        float durThird = ComputeSpawnDurationPx(thirdWordText, spawnBase, spawnCoefPxSqrt, spawnDurClamp);

        _spawn2Co = StartCoroutine(SpawnIn(
            _nextRT, _nextCG, _nextTarget,
            new Vector2(spawnDistNext, 0f),
            durNext, nextIdleAlpha,
            true,     // isNextLabel
            spawnEaseNext
        ));

        _spawn3Co = StartCoroutine(SpawnIn(
            _thirdRT, _thirdCG, _thirdTarget,
            new Vector2(spawnDistThird, 0f),
            durThird, thirdIdleAlpha,
            false,    // isNextLabel
            spawnEaseThird
        ));

        // 7) Reset state
        timer = attackTimeLimit;
        if (inputField) { inputField.text = ""; inputField.ActivateInputField(); }
        pendingAdvance  = false;
        isTransitioning = false;
        _advanceCo = null;
    }

    // ===================== SpawnIn (parallax + easing riêng + khóa Follow) =====================
    IEnumerator SpawnIn(RectTransform rt, CanvasGroup cg,
                        Vector2 target, Vector2 fromOffset,
                        float slideDur, float fadeTarget,
                        bool isNextLabel, Ease easing)
    {
        if (!rt) yield break;

        if (disableFollowDuringSpawn)
        {
            if (isNextLabel) _suspendFollowNext = true;
            else _suspendFollowThird = true;
        }

        // đảm bảo Follow (nếu có) cũng nhắm đúng đích
        if (isNextLabel) _nextTarget  = PixelSnap(target);
        else _thirdTarget = PixelSnap(target);

        if (cg) cg.alpha = 0f;
        rt.anchoredPosition = PixelSnap(target + fromOffset);

        float t = 0f; slideDur = Mathf.Max(0.001f, slideDur);
        float fadeDur = Mathf.Max(0.001f, spawnFadeDur);

        while (t < slideDur)
        {
            t += DT;
            float k = Mathf.Clamp01(t / slideDur);
            float e = EvaluateEase(easing, k);

            rt.anchoredPosition = LerpSnap(target + fromOffset, target, e);

            if (cg)
            {
                float kf = Mathf.Clamp01(t / fadeDur);
                cg.alpha = Mathf.Lerp(0f, fadeTarget, kf);
            }

            yield return null;
        }

        rt.anchoredPosition = PixelSnap(target);
        if (cg) cg.alpha = fadeTarget;

        if (disableFollowDuringSpawn)
        {
            if (isNextLabel) _suspendFollowNext = false;
            else _suspendFollowThird = false;
        }
    }

    // ===================== Targets & Follow =====================
    void UpdateTargets(bool immediate = false)
    {
        Vector2 nextT = CalcNextTarget();
        Vector2 thirdT = CalcThirdTarget(nextT);

        if (immediate)
        {
            _nextRT.anchoredPosition  = PixelSnap(nextT);
            _thirdRT.anchoredPosition = PixelSnap(thirdT);
        }
        _nextTarget  = PixelSnap(nextT);
        _thirdTarget = PixelSnap(thirdT);

        if (_nextCG && !isTransitioning) _nextCG.alpha  = nextIdleAlpha;
        if (_thirdCG && !isTransitioning) _thirdCG.alpha = thirdIdleAlpha;
    }

    Vector2 CalcNextTarget()
    {
        float wCur = Measure(wordText);
        float guard = (current.Length > 0) ? guardGapWhileTyping : 0f;
        float nextLeft = wCur * 0.5f + nextGapPixels + guard;
        float nextCenterX = nextLeft + Measure(nextWordText) * 0.5f;
        return new Vector2(nextCenterX, 0f);
    }

    Vector2 CalcThirdTarget(Vector2 nextCenter)
    {
        float wNext = Measure(nextWordText);
        float thirdLeft = (nextCenter.x - wNext * 0.5f) + wNext + nextGapPixels;
        float thirdCenterX = thirdLeft + Measure(thirdWordText) * 0.5f;
        return new Vector2(thirdCenterX, 0f);
    }

    void FollowTargets()
    {
        float dt = DT;

        if (_nextRT && !_suspendFollowNext)
        {
            float x = Mathf.SmoothDamp(_nextRT.anchoredPosition.x, _nextTarget.x,
                                       ref _nextVelX, followSmoothTime, Mathf.Infinity, dt);
            _nextRT.anchoredPosition = PixelSnap(new Vector2(x, 0f));
        }

        if (_thirdRT && !_suspendFollowThird)
        {
            float x = Mathf.SmoothDamp(_thirdRT.anchoredPosition.x, _thirdTarget.x,
                                       ref _thirdVelX, followSmoothTime, Mathf.Infinity, dt);
            _thirdRT.anchoredPosition = PixelSnap(new Vector2(x, 0f));
        }
    }

    // ===================== Helpers =====================
    string GetWord()
    {
        string w = wordPool ? wordPool.GetRandomWord() : null;
        return string.IsNullOrEmpty(w) ? (string.IsNullOrEmpty(fallbackWord) ? "type" : fallbackWord) : w;
    }

    float Measure(TextMeshProUGUI tmp)
    {
        if (!tmp) return 0f;
        tmp.enableWordWrapping = false;
        var pv = tmp.GetPreferredValues(tmp.text, Mathf.Infinity, Mathf.Infinity);
        return pv.x;
    }

    float ComputeSpawnDurationPx(TextMeshProUGUI label, float baseDur, float coefSqrt, Vector2 clamp)
    {
        if (!spawnThrottleByWidth || label == null) return baseDur;
        float w = Mathf.Max(0f, Measure(label));
        float dur = baseDur + Mathf.Sqrt(w) * coefSqrt;
        return Mathf.Clamp(dur, clamp.x, clamp.y);
    }

    // ---- Error flash helpers ----
    void TriggerErrorFlash(TextMeshProUGUI label, ref Coroutine token, Color baseColor)
    {
        if (!enableErrorFlash || label == null) return;
        if (token != null) StopCoroutine(token);
        token = StartCoroutine(FlashColorCo(label, baseColor, errorFlashColor, errorFlashDuration));
    }

    void StopErrorFlash(ref Coroutine token, TextMeshProUGUI label, Color baseColor)
    {
        if (token != null) StopCoroutine(token);
        token = null;
        if (label) label.color = baseColor;
    }

    IEnumerator FlashColorCo(TextMeshProUGUI label, Color baseColor, Color flashColor, float duration)
    {
        if (!label || duration <= 0f) yield break;
        float half = duration * 0.5f, t = 0f;

        // lên đỏ
        while (t < half)
        {
            t += DT;
            float k = Smooth01(Mathf.Clamp01(t / half));
            label.color = Color.Lerp(baseColor, flashColor, k);
            yield return null;
        }
        // về màu gốc
        t = 0f;
        while (t < half)
        {
            t += DT;
            float k = Smooth01(Mathf.Clamp01(t / half));
            label.color = Color.Lerp(flashColor, baseColor, k);
            yield return null;
        }
        label.color = baseColor;
    }

    Vector2 PixelSnap(Vector2 v) => pixelSnap ? new Vector2(Mathf.Round(v.x), Mathf.Round(v.y)) : v;
    Vector2 LerpSnap(Vector2 a, Vector2 b, float t) => PixelSnap(Vector2.LerpUnclamped(a, b, t));
    static float Smooth01(float x) => x * x * (3f - 2f * x);
    static float EaseOutCubic(float x) { x = Mathf.Clamp01(x); return 1f - Mathf.Pow(1f - x, 3f); }

    float EvaluateEase(Ease e, float k)
    {
        k = Mathf.Clamp01(k);
        switch (e)
        {
            case Ease.CubicOut: return 1f - Mathf.Pow(1f - k, 3f);
            case Ease.QuintOut: return 1f - Mathf.Pow(1f - k, 5f);
            case Ease.ExpoOut: return (k >= 1f) ? 1f : 1f - Mathf.Pow(2f, -10f * k);
            default: return k;
        }
    }

    IEnumerator PunchScale(RectTransform rt, Vector3 baseScale, float peakMul, float duration)
    {
        if (!rt || duration <= 0f || peakMul <= 1f) yield break;
        float half = duration * 0.5f, t = 0f;
        while (t < half) { t += DT; float k = Smooth01(t / half); rt.localScale = baseScale * Mathf.Lerp(1f, peakMul, k); yield return null; }
        t = 0f;
        while (t < half) { t += DT; float k = Smooth01(t / half); rt.localScale = baseScale * Mathf.Lerp(peakMul, 1f, k); yield return null; }
        rt.localScale = baseScale;
    }

    CanvasGroup EnsureCG(TextMeshProUGUI t)
    {
        if (!t) return null;
        if (!t.TryGetComponent<CanvasGroup>(out var cg)) cg = t.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }
}
