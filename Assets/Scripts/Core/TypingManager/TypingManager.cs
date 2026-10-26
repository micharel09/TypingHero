using System;
using System.Collections;
using System.Collections.Generic;
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

    // Retained for source/serialized compatibility; r7 uses fixed palette and timing.
    [Range(0f, 1f)] public float nextIdleAlpha = 1f;
    [Range(0f, 1f)] public float thirdIdleAlpha = .42f;
    public enum Ease { CubicOut, QuintOut, ExpoOut }

    [Header("Error feedback")]
    [SerializeField] bool enableErrorFlash = true;
    [SerializeField] Color errorFlashColor = new Color(1f, .25f, .25f, 1f);

    // ===================== Events (giữ API) =====================
    public static bool MuteWordCorrect = false;
    public static event Action OnWordCorrect;
    public static event Action OnWordTimeout;
    public static event Action OnWordAdvanced;
    public static event Action<char> OnCorrectChar;
    public static event Action<bool> OnInputResult;
    string originalCurrent = "";
    HashSet<int> rewardedCurrent = new HashSet<int>(), rewardedNext = new HashSet<int>();

    void AcceptChar(char c, int position, HashSet<int> rewarded)
    {
        OnInputResult?.Invoke(true);
        if (!MuteWordCorrect || rewarded.Add(position)) OnCorrectChar?.Invoke(c);
    }

    string current = "", next = "", third = "";
    float timer, slideRemaining, errorRemaining;
    int completedThisFrame;
    string[] practiceWords;
    int practiceIndex;
    bool practiceTimer = true, practiceInput = true;
    bool pendingAdvance, isTransitioning;
    int _typeAheadCount;
    RectTransform _curRT, _nextRT, _thirdRT;
    [SerializeField] float referencePixel = 3f;
    [SerializeField] Color completedColor = new Color(.44f, .66f, .62f, 1);
    Color currentColor, nextColor, thirdColor;
    bool introPreview;
    float introStartRemaining;

    internal void BeginIntroPreview()
    {
        introPreview = true;
        introStartRemaining = 0;
    }

    internal void EndIntroPreview(bool combatHandoff)
    {
        introStartRemaining = introPreview && combatHandoff ? .1f : 0;
        introPreview = false;
        Paint();
    }

    float DT => GamePauseController.InputBlocked || CombatSession.Finished ? 0 : Time.unscaledDeltaTime;

    void Awake()
    {
        if (wordPool) wordPool.BuildCache();
        _curRT = wordText.rectTransform;
        _nextRT = nextWordText.rectTransform;
        _thirdRT = thirdWordText.rectTransform;
        currentColor = wordText.color; nextColor = nextWordText.color; thirdColor = thirdWordText.color;
    }

    void Start() { ForceInitial(); }
    void Update()
    {
        completedThisFrame = 0;
        slideRemaining = Mathf.Max(0, slideRemaining - DT);
        errorRemaining = Mathf.Max(0, errorRemaining - DT);
        introStartRemaining = Mathf.Max(0, introStartRemaining - DT);
        ProcessInput(Input.inputString);
        if (practiceTimer && !GamePauseController.InputBlocked && !CombatSession.Finished) HandleTimer();
        Paint();
    }

    internal void SetPractice(string[] words, bool timed, bool acceptsLetters)
    {
        practiceWords = words; practiceIndex = 0;
        practiceTimer = timed; practiceInput = acceptsLetters;
        ForceInitial();
    }

    void HandleTimer()
    {
        timer -= Time.deltaTime;
        if (countdownSlider) countdownSlider.value = Mathf.Clamp01(timer / attackTimeLimit);
        if (timer > 0) return;
        OnWordTimeout?.Invoke();
        AdvanceToNext();
    }

    internal void ProcessInput(string s)
    {
        if (!practiceInput || GamePauseController.InputBlocked || DungeonSession.InputSuppressed || CombatSession.Finished || string.IsNullOrEmpty(s)) return;
        int completed = 0;
        foreach (char c in s)
        {
            if (!practiceInput || GamePauseController.InputBlocked || CombatSession.Finished) break;
            if (!char.IsLetter(c) || current.Length == 0) continue;
            // Correct/error feedback owns the first accepted letter immediately.
            introStartRemaining = 0;
            if (char.ToLowerInvariant(c) == char.ToLowerInvariant(current[0]))
            {
                AcceptChar(c, originalCurrent.Length - current.Length, rewardedCurrent);
                errorRemaining = 0;
                current = current.Substring(1);
                if (current.Length == 0)
                {
                    if (!MuteWordCorrect) OnWordCorrect?.Invoke();
                    OnWordAdvanced?.Invoke();
                    AdvanceToNext();
                    completed++;
                }
            }
            else
            {
                OnInputResult?.Invoke(false);
                current = originalCurrent;
                errorRemaining = enableErrorFlash ? .18f : 0;
            }
        }
        completedThisFrame += completed;
        if (completed > 1 || completedThisFrame > 1) slideRemaining = 0;
        Paint();
    }

    void ForceInitial()
    {
        pendingAdvance = isTransitioning = false;
        completedThisFrame = 0;
        current = originalCurrent = GetWord(); next = GetWord(); third = GetWord();
        rewardedCurrent.Clear(); rewardedNext.Clear();
        _typeAheadCount = 0; slideRemaining = errorRemaining = 0;
        timer = attackTimeLimit;
        if (countdownSlider) countdownSlider.value = 1;
        if (inputField) inputField.text = "";
        Paint();
    }

    void AdvanceToNext()
    {
        current = originalCurrent = next; next = third; third = GetWord();
        rewardedCurrent.Clear(); rewardedNext.Clear();
        timer = attackTimeLimit; errorRemaining = 0;
        if (countdownSlider) countdownSlider.value = 1;
        slideRemaining = BrowserMotion.Reduced ? 0 : .1f;
        pendingAdvance = isTransitioning = false;
        Paint();
    }

    void Paint()
    {
        if (!wordText) return;
        int progress = originalCurrent.Length - current.Length;
        wordText.color = Color.white;
        string before = originalCurrent.Substring(0, Mathf.Clamp(progress, 0, originalCurrent.Length));
        string after = originalCurrent.Substring(before.Length);
        Color activeInk = introPreview ? nextColor : Color.Lerp(currentColor, nextColor, introStartRemaining / .1f);
        string ink = ColorUtility.ToHtmlStringRGB(errorRemaining > 0 ? errorFlashColor : activeInk);
        wordText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(completedColor) + ">" + before + "</color><color=#" + ink + ">" + after + "</color>";
        nextWordText.text = next; thirdWordText.text = third;
        nextWordText.color = nextColor; thirdWordText.color = thirdColor;
        if (BrowserMotion.Reduced) slideRemaining = 0;
        float offset = 66 * Mathf.Pow(slideRemaining / .1f, 3);
        Place(wordText, 29 + offset); Place(nextWordText, 95 + offset); Place(thirdWordText, 161 + offset);
    }

    void Place(TextMeshProUGUI label, float center)
    {
        // Full-word geometry is independent of typed prefix and error feedback.
        float width = Mathf.Round(label.GetPreferredValues().x / referencePixel);
        float shift = Mathf.Round(center - width / 2) * referencePixel;
        label.rectTransform.anchoredPosition = new Vector2(shift, 0);
        label.rectTransform.localScale = Vector3.one;
    }

    string GetWord()
    {
        if (practiceWords != null && practiceWords.Length > 0)
            return practiceWords[practiceIndex++ % practiceWords.Length];
        string w = wordPool ? wordPool.GetRandomWord() : null;
        return string.IsNullOrEmpty(w) ? (string.IsNullOrEmpty(fallbackWord) ? "type" : fallbackWord) : w;
    }

}
