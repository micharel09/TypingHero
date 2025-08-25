using UnityEngine;
using TMPro;
using System;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem I { get; private set; }

    [Header("Config & Refs")]
    [SerializeField] ScoreRules rules;
    [SerializeField] PlayerSlayerMode slayer;
    [SerializeField] ParrySystem parry;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] ScoreUI_ComboHUD comboHUD;
    [SerializeField] FloatingScore floatingPrefab;
    [SerializeField] Canvas floatingCanvas;

    [Header("Popup Anchor & Offset")]
    [SerializeField] RectTransform popupAnchor;
    [SerializeField] Vector2 popupOffset = new Vector2(-100f, 20f);

    [Header("Behaviour")]
    [SerializeField] bool stopDecayWhilePaused = true;

    public int Score { get; private set; }
    public float Multiplier { get; private set; }
    public float ComboRemain01 { get; private set; }

    public static event Action<int, float> OnScored;

    // ====== Gating combo ======
    int _streakWords;       // đếm từ liên tiếp trong chế độ thường
    int _streakChars;       // đếm ký tự liên tiếp trong Slayer
    bool _comboEnabled;     // chỉ khi true mới cộng multiplier

    float _windowRemain;
    bool _paused;
    Camera _cam;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        if (!slayer) slayer = FindObjectOfType<PlayerSlayerMode>(true);
        if (!parry) parry  = FindObjectOfType<ParrySystem>(true);
        _cam = Camera.main;

        ResetAllInternal();
    }

    void OnEnable()
    {
        if (parry != null) parry.OnParrySuccess += OnParry;
        TypingManager.OnWordCorrect += OnWordCorrect;
        TypingManager.OnCorrectChar += OnCharCorrect;
        TypingManager.OnWordTimeout += OnWordTimeout; // nếu bạn đã bắn event này
    }

    void OnDisable()
    {
        if (parry != null) parry.OnParrySuccess -= OnParry;
        TypingManager.OnWordCorrect -= OnWordCorrect;
        TypingManager.OnCorrectChar -= OnCharCorrect;
        TypingManager.OnWordTimeout -= OnWordTimeout;
    }

    void Update()
    {
        if (_paused && stopDecayWhilePaused) return;
        if (Multiplier <= rules.startMultiplier && !_comboEnabled) { ComboRemain01 = 0f; return; }

        _windowRemain -= Time.unscaledDeltaTime;
        if (_windowRemain <= 0f)
        {
            // Hết cửa sổ → rơi về trạng thái trước combo: reset multiplier & streak
            ResetComboOnly();
        }
        ComboRemain01 = Mathf.Clamp01(_windowRemain / Mathf.Max(0.001f, rules.comboWindowSeconds));
        UpdateUI();
    }

    // ==== Public ====
    public void SetPaused(bool v) => _paused = v;

    public void ResetScore()
    {
        Score = 0;
        ResetComboOnly();
        UpdateUI(true);
    }

    public void BreakCombo()  // timeout/miss
    {
        ResetComboOnly();
        UpdateUI();
    }

    // ==== Internals ====
    void ResetAllInternal()
    {
        Score = 0;
        Multiplier = rules ? rules.startMultiplier : 1f;
        _windowRemain = 0f;
        ComboRemain01 = 0f;
        _streakWords = _streakChars = 0;
        _comboEnabled = false;
        UpdateUI(true);
    }

    void ResetComboOnly()
    {
        Multiplier = rules ? rules.startMultiplier : 1f;
        _windowRemain = 0f;
        ComboRemain01 = 0f;
        _streakWords = _streakChars = 0;
        _comboEnabled = false;
    }

    void BumpComboWindow()
    {
        _windowRemain = rules.comboWindowSeconds;
    }

    // ===== Event handlers =====
    void OnParry(ParrySystem.ParryContext _)
    {
        // Parry chỉ cộng điểm; CHỈ tăng multiplier nếu combo đã bật.
        AddScore(rules.parryPoint, countAsComboUnit: false, hitKind: HitKind.Parry);
    }

    void OnWordCorrect()
    {
        if (slayer && slayer.IsActive) return; // Slayer tính theo ký tự

        _streakWords++;
        BumpComboWindow();

        // Bật combo khi đạt ngưỡng từ
        if (!_comboEnabled && _streakWords >= Mathf.Max(0, rules.minWordsForCombo))
            _comboEnabled = true;

        AddScore(rules.wordPoint, countAsComboUnit: true, hitKind: HitKind.Word);
    }

    void OnCharCorrect(char _)
    {
        if (!(slayer && slayer.IsActive)) return;

        _streakChars++;
        BumpComboWindow();

        // Bật combo khi đạt ngưỡng ký tự (mặc định 0 → bật ngay)
        if (!_comboEnabled && _streakChars >= Mathf.Max(0, rules.minSlayerCharsForCombo))
            _comboEnabled = true;

        AddScore(rules.charPointInSlayer, countAsComboUnit: true, hitKind: HitKind.SlayerChar);
    }

    void OnWordTimeout()
    {
        BreakCombo();
    }

    enum HitKind { Word, SlayerChar, Parry }

    void AddScore(int basePoint, bool countAsComboUnit, HitKind hitKind)
    {
        if (basePoint <= 0 || rules == null) return;

        // Nếu combo đã bật → mỗi hit gia hạn cửa sổ + tăng multiplier
        if (_comboEnabled)
        {
            BumpComboWindow();
            Multiplier = Mathf.Min(rules.maxMultiplier, Multiplier + rules.addPerHit);
        }
        else
        {
            // combo chưa bật: chỉ gia hạn cửa sổ khi là thành phần streak (Word/Char)
            if (hitKind != HitKind.Parry) BumpComboWindow();
        }

        int delta = Mathf.RoundToInt(basePoint * Multiplier);
        Score += delta;

        SpawnFloating(delta, Multiplier, worldPos: null);
        UpdateUI();
        OnScored?.Invoke(delta, Multiplier);
    }

    void UpdateUI(bool force = false)
    {
        if (scoreText)
        {
            scoreText.text = Score.ToString(rules.scoreFormat);
            scoreText.color = ColorByMultiplier(Multiplier);
            ScoreUI_Flash.Do(scoreText, Multiplier);
        }
        if (comboHUD)
        {
            // HUD chỉ hiện khi mul > 1.0 (đã xử lý sẵn trong ComboHUD) — behavior giữ nguyên
            comboHUD.SetMultiplier(Multiplier, rules.maxMultiplier, ColorByMultiplier(Multiplier));
            comboHUD.SetDecay(ComboRemain01);
        }
    }

    Color ColorByMultiplier(float mul)
    {
        if (rules == null) return Color.white;
        if (mul >= rules.tier3Threshold) return rules.colorTier3;
        if (mul >= rules.tier2Threshold) return rules.colorTier2;
        return rules.colorTier1;
    }

    void SpawnFloating(int delta, float mul, Vector3? worldPos)
    {
        if (!floatingPrefab || !floatingCanvas) return;

        var go = Instantiate(floatingPrefab, floatingCanvas.transform);
        go.Setup("+" + delta.ToString(rules.scoreFormat),
                 ColorByMultiplier(mul),
                 floatingCanvas,
                 popupAnchor,
                 popupOffset);
    }
}
