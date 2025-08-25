using System.Collections;
using System.Collections.Generic;   // NEW
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSlayerMode : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerAttackEvents attackEvents;
    [SerializeField] SlayerAfterimagePool afterimage;

    [Header("Enemy Bind")]
    [SerializeField] SkeletonController enemy;   // << kéo Skeleton vào đây

    [Header("Slayer Enter (optional)")]
    [SerializeField] string slayerEnterStatePath = "Base Layer.player_slayer";
    [SerializeField, Range(0f, 0.2f)] float enterCrossfade = 0.02f;

    [Header("RabbitAttack (3 clip trong sub-SM)")]
    [SerializeField]
    string[] slayerAttackStates = new string[3] {
        "Base Layer.player_slayer.attackS1",
        "Base Layer.player_slayer.attackS2",
        "Base Layer.player_slayer.attackS3"
    };
    [SerializeField, Range(0f, 0.2f)] float attackCrossfade = 0.02f;

    [Header("Tuning")]
    [SerializeField] float attackSpeedMultiplier = 3.5f;
    [SerializeField] float slayerMinOpenSeconds = 0.035f;
    [SerializeField] float inputDebounce = 0.015f;

    [Header("Combat Rules")]
    [SerializeField] bool ignoreHitstop = true;

    [Header("Animator Param")]
    [SerializeField] string speedParam = "AttackSpeedMul";

    [Header("Exit")]
    [SerializeField] string exitStatePath = "Base Layer.player_idle";
    [SerializeField, Range(0f, 0.2f)] float exitCrossfade = 0.02f;

    [Header("Typing guards")]
    [SerializeField] float postWordCooldown = 0.08f;

    [Header("Grace sau stun")]
    [SerializeField] float graceAfterStun = 0.08f;

    [Header("Combo rules")]
    [SerializeField] bool resetComboOnWord = true;
    [SerializeField] bool comboDecay = true;
    [SerializeField] float comboDecayIdleDelay = 0.4f;
    [SerializeField] float comboDecayPerSecond = 8f;

    bool Paused => Time.timeScale == 0f;

    [Header("Slayer Damage (relative by tint)")]
    [SerializeField] bool useSlayerRelativeDamage = true;
    [SerializeField] int stepsToMaxTint = 20;
    [SerializeField] float dmgMulWhite = 0.5f;
    [SerializeField] float dmgMulRed = 1.2f;
    [SerializeField] AnimationCurve dmgMulCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Auto-Sync with Afterimage")]
    [SerializeField] bool autoSyncStepsFromAfterimage = true;
    [SerializeField] string afterimageComboMaxFieldName = "comboMax";

    [Header("Debug")]
    [SerializeField] bool logs = false;
    [SerializeField] bool showDebugHUD = true;
    [SerializeField] Vector2 hudPosition = new Vector2(12, 12);

    [Header("Slayer ScreenFX (relative by tint)")]
    [SerializeField] bool autoPlayScreenFX = true;
    [SerializeField] SlayerScreenFX screenFX;
    [SerializeField] bool silhouetteOnInSlayer = true;

    // ===== NEW: Auto toggle objects in Slayer =====
    [Header("Objects Auto Toggle")]
    [Tooltip("Các object trong list sẽ tự tắt khi Enter Slayer và tự bật lại khi Exit Slayer.")]
    [SerializeField] GameObject[] disableDuringSlayer;
    readonly List<GameObject> _disabledBySlayer = new();   // chỉ lưu những object mà chính script đã tắt

    float _comboF;
    int ComboInt => Mathf.Max(0, Mathf.FloorToInt(_comboF));
    int _typedSteps = 0;

    public bool IsActive => _active;
    public bool IgnoreHitstop => ignoreHitstop;

    bool _active, _didFirstSwing;
    float _remain, _lastTypedAt, _savedMinOpen, _savedSpeedMul, _blockUntil;
    Coroutine _co;

    EnemyStunController _followStun;
    bool _windowOpen;
    bool _pendingExit;
    float _exitAt;

    // ===== Bind enemy events =====
    void OnEnable()
    {
        if (!enemy) enemy = FindObjectOfType<SkeletonController>();
        if (enemy)
        {
            enemy.OnDeathStarted -= OnEnemyDieStarted;
            enemy.OnDeathStarted += OnEnemyDieStarted;
        }

        TypingManager.OnWordCorrect += DoNothing; // giữ OnDisable an toàn nếu có logic khác
    }

    void OnDisable()
    {
        if (enemy) enemy.OnDeathStarted -= OnEnemyDieStarted;
        Exit(); // đảm bảo thoát hẳn khi disable
        TypingManager.OnWordCorrect -= DoNothing;
    }

    void OnEnemyDieStarted()
    {
        if (IsActive) Exit();      // << THOÁT SLAYER NGAY khi enemy bắt đầu die
    }

    void DoNothing() { } // placeholder để không thay đổi hệ khác

    // ===== API =====
    public float TintProgress => IsActive ? Mathf.Clamp01(_typedSteps / (float)Mathf.Max(1, stepsToMaxTint)) : 0f;

    public void Activate(float duration)
    {
        if (duration <= 0f) return;
        _remain += duration;
        if (_active) return;
        _followStun = null;
        Enter();
    }

    public void ActivateForStun(EnemyStunController stun)
    {
        if (!stun) return;
        if (_active && _followStun && _followStun != stun) Exit();
        _followStun = stun;
        if (!_active) Enter();
    }

    void Enter()
    {
        _comboF = 0f;
        _typedSteps = 0;

        if (!animator || !attackEvents) return;

        if (autoSyncStepsFromAfterimage && afterimage)
        {
            int comboMax;
            if (TryGetAfterimageComboMax(afterimage, out comboMax))
                stepsToMaxTint = comboMax;
        }

        _active = true;
        _didFirstSwing = false;
        _blockUntil = 0f;
        _pendingExit = false;
        _windowOpen = false;

        _savedMinOpen = attackEvents.minOpenSeconds;
        _savedSpeedMul = animator.GetFloat(speedParam);

        attackEvents.minOpenSeconds = slayerMinOpenSeconds;
        animator.SetFloat(speedParam, attackSpeedMultiplier);

        TypingManager.OnCorrectChar -= OnCorrectChar;
        TypingManager.OnWordCorrect -= OnWordCorrect;
        TypingManager.OnCorrectChar += OnCorrectChar;
        TypingManager.OnWordCorrect += OnWordCorrect;
        TypingManager.MuteWordCorrect = true;

        TypingManager.OnWordAdvanced -= OnWordAdvanced;
        TypingManager.OnWordAdvanced += OnWordAdvanced;

        attackEvents.OnWindowOpen += MarkOpen;
        attackEvents.OnWindowClose += MarkClose;

        if (_followStun)
        {
            _followStun.onStunEnd.RemoveListener(OnFollowStunEnd);
            _followStun.onStunEnd.AddListener(OnFollowStunEnd);
        }

        if (!string.IsNullOrEmpty(slayerEnterStatePath))
            animator.CrossFadeInFixedTime(slayerEnterStatePath, enterCrossfade, 0, 0f);

        if (autoPlayScreenFX && screenFX) screenFX.EnterSlayerFX();
        if (silhouetteOnInSlayer) SlayerModeSignals.SetActive(true);

        // NEW: tắt các object được cấu hình
        ToggleObjectsOnEnter();

        _co = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (true)
        {
            if (Paused) { yield return null; continue; }

            if (_followStun != null)
            {
                if (comboDecay && _comboF > 0f)
                {
                    float idle = Time.unscaledTime - _lastTypedAt;
                    if (idle >= comboDecayIdleDelay)
                        _comboF = Mathf.Max(0f, _comboF - comboDecayPerSecond * Time.unscaledDeltaTime);
                }

                if (!_followStun.IsStunned)
                {
                    if (!_pendingExit)
                    {
                        _pendingExit = true;
                        _exitAt = Time.unscaledTime + Mathf.Max(0f, graceAfterStun);
                    }
                    if (!_windowOpen && Time.unscaledTime >= _exitAt) break;
                }
            }
            else
            {
                _remain -= Time.unscaledDeltaTime;
                if (_remain <= 0f) break;
            }
            yield return null;
        }
        Exit();
    }

    void Exit()
    {
        if (!_active) return;
        _active = false;

        TypingManager.OnCorrectChar -= OnCorrectChar;
        TypingManager.OnWordCorrect -= OnWordCorrect;
        TypingManager.OnWordAdvanced -= OnWordAdvanced;
        TypingManager.MuteWordCorrect = false;

        _comboF = 0f;
        _typedSteps = 0;

        attackEvents.OnWindowOpen -= MarkOpen;
        attackEvents.OnWindowClose -= MarkClose;

        if (_followStun)
        {
            _followStun.onStunEnd.RemoveListener(OnFollowStunEnd);
            _followStun = null;
        }

        attackEvents.minOpenSeconds = _savedMinOpen;
        animator.SetFloat(speedParam, _savedSpeedMul);

        if (!string.IsNullOrEmpty(exitStatePath))
            animator.CrossFadeInFixedTime(exitStatePath, exitCrossfade, 0, 0f);

        if (_co != null) { StopCoroutine(_co); _co = null; }
        _remain = 0f;

        // NEW: bật lại những object mình đã tắt khi Enter
        ToggleObjectsOnExit();

        if (autoPlayScreenFX && screenFX) screenFX.ExitSlayerFX();
        if (silhouetteOnInSlayer) SlayerModeSignals.SetActive(false);
    }

    void OnFollowStunEnd() { }

    void OnWordCorrect() => _blockUntil = Time.unscaledTime + postWordCooldown;

    void OnWordAdvanced() { if (resetComboOnWord) _comboF = 0f; }

    void OnCorrectChar(char _)
    {
        if (Paused || !_active || _pendingExit) return;
        float now = Time.unscaledTime;
        if (now < _blockUntil) return;
        if (now - _lastTypedAt < inputDebounce) return;
        _lastTypedAt = now;

        _comboF += 1f;
        _typedSteps += 1;

        if (_didFirstSwing && afterimage)
            afterimage.SpawnBurstFromCurrentFrame(_typedSteps);

        var st = PickRandomAttackState();
        if (!string.IsNullOrEmpty(st))
            animator.CrossFadeInFixedTime(st, attackCrossfade, 0, 0f);

        attackEvents.OnAttackStart();
        attackEvents.OnAttackEnd();
        _didFirstSwing = true;
    }

    void MarkOpen() => _windowOpen = true;
    void MarkClose() => _windowOpen = false;

    string PickRandomAttackState()
    {
        if (slayerAttackStates == null || slayerAttackStates.Length == 0) return null;
        for (int i = 0; i < 3; i++)
        {
            var s = slayerAttackStates[Random.Range(0, slayerAttackStates.Length)];
            if (!string.IsNullOrEmpty(s)) return s;
        }
        return null;
    }

    void Awake()
    {
        if (!afterimage) afterimage = GetComponent<SlayerAfterimagePool>();
        if (!screenFX) screenFX = FindObjectOfType<SlayerScreenFX>();
    }

    void OnGUI()
    {
        if (!showDebugHUD || !IsActive) return;
        float t = stepsToMaxTint > 0 ? Mathf.Clamp01(_typedSteps / (float)stepsToMaxTint) : 0f;
        float mul = CurrentSlayerMultiplier;
        string text = $"<b>SLAYER</b>  steps {_typedSteps}/{stepsToMaxTint}  t={t:0.00}\nDMG x{mul:0.00}";
        var size = GUI.skin.label.CalcSize(new GUIContent(text));
        var rect = new Rect(hudPosition.x, hudPosition.y, Mathf.Max(180, size.x + 12), size.y + 10);
        var oc = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.6f); GUI.Box(rect, GUIContent.none);
        GUI.color = Color.white; GUI.Label(new Rect(rect.x+6, rect.y+4, rect.width-12, rect.height-8), text);
        GUI.color = oc;
    }

    public float CurrentSlayerMultiplier
    {
        get
        {
            if (!IsActive || !useSlayerRelativeDamage) return 1f;
            int maxSteps = Mathf.Max(1, stepsToMaxTint);
            float t = Mathf.Clamp01(_typedSteps / (float)maxSteps);
            float k = dmgMulCurve.Evaluate(t);
            return Mathf.Lerp(dmgMulWhite, dmgMulRed, k);
        }
    }

    bool TryGetAfterimageComboMax(SlayerAfterimagePool pool, out int comboMax)
    {
        comboMax = stepsToMaxTint;
        var prop = pool.GetType().GetProperty("ComboMax", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(int)) { comboMax = (int)prop.GetValue(pool); return true; }
        var field = pool.GetType().GetField(afterimageComboMaxFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(int)) { comboMax = (int)field.GetValue(pool); return true; }
        return false;
    }

    // ===== NEW: helpers =====
    void ToggleObjectsOnEnter()
    {
        _disabledBySlayer.Clear();
        if (disableDuringSlayer == null || disableDuringSlayer.Length == 0) return;

        for (int i = 0; i < disableDuringSlayer.Length; i++)
        {
            var go = disableDuringSlayer[i];
            if (!go) continue;
            if (go.activeSelf)
            {
                go.SetActive(false);
                _disabledBySlayer.Add(go);
            }
        }
    }

    void ToggleObjectsOnExit()
    {
        if (_disabledBySlayer.Count == 0) return;
        for (int i = 0; i < _disabledBySlayer.Count; i++)
        {
            var go = _disabledBySlayer[i];
            if (go) go.SetActive(true);
        }
        _disabledBySlayer.Clear();
    }
}
