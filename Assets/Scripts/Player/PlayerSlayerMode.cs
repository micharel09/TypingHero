using System.Collections;
using System.Reflection;               // for auto-sync via reflection
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSlayerMode : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerAttackEvents attackEvents;
    [SerializeField] SlayerAfterimagePool afterimage;

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
    [SerializeField] float graceAfterStun = 0.08f;   // 0 = tắt ngay

    [Header("Combo rules")]
    [SerializeField] bool resetComboOnWord = true;     // reset combo nội bộ khi sang từ mới
    [SerializeField] bool comboDecay = true;           // decay combo nếu ngừng gõ
    [SerializeField] float comboDecayIdleDelay = 0.4f;
    [SerializeField] float comboDecayPerSecond = 8f;


    // ===================== Slayer Damage (relative by tint) =====================
    [Header("Slayer Damage (relative by tint)")]
    [SerializeField] bool useSlayerRelativeDamage = true;
    [Tooltip("Phải khớp với Combo Max trong SlayerAfterimagePool để damage và màu đồng bộ.")]
    [SerializeField] int stepsToMaxTint = 20;          // sẽ auto-sync từ Afterimage khi Enter()
    [SerializeField] float dmgMulWhite = 0.5f;         // trắng ~ 0.5x (vd base 10 -> 5)
    [SerializeField] float dmgMulRed = 1.2f;         // đỏ max ~ 1.2x (vd base 10 -> 12)
    [SerializeField]
    AnimationCurve dmgMulCurve
        = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Auto-Sync with Afterimage")]
    [SerializeField] bool autoSyncStepsFromAfterimage = true;
    [Tooltip("Tên property/field trong Afterimage để lấy ComboMax (ưu tiên property 'ComboMax', fallback field 'comboMax').")]
    [SerializeField] string afterimageComboMaxFieldName = "comboMax";

    [Header("Debug")]
    [SerializeField] bool logs = false;
    [SerializeField] bool showDebugHUD = true;
    [SerializeField] Vector2 hudPosition = new Vector2(12, 12);

    // ===================== Slayer ScreenFX (relative by tint) =====================
    [SerializeField] bool autoPlayScreenFX = true;
    [SerializeField] SlayerScreenFX screenFX;
    [SerializeField] bool silhouetteOnInSlayer = true;



    // ---- runtime ----
    float _comboF;                                // vẫn giữ nếu cần dùng logic combo khác
    int ComboInt => Mathf.Max(0, Mathf.FloorToInt(_comboF));

    // số ký tự đã gõ trong phiên Slayer (để sync đúng với afterimage tint)
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

    // ===== API =====
    public float TintProgress
    {
        get
        {
            if (!IsActive) return 0f;
            int maxSteps = Mathf.Max(1, stepsToMaxTint);
            // _typedSteps là bộ đếm kí tự đã gõ trong phiên Slayer (bạn đã có)
            return Mathf.Clamp01(_typedSteps / (float)maxSteps);
        }
    }
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

        // Auto-sync StepsToMaxTint từ Afterimage
        if (autoSyncStepsFromAfterimage && afterimage)
        {
            int comboMax;
            if (TryGetAfterimageComboMax(afterimage, out comboMax))
            {
                stepsToMaxTint = comboMax;
                if (logs) Debug.Log($"[Slayer] Sync stepsToMaxTint = {stepsToMaxTint} from Afterimage.");
            }
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

        // sự kiện typing
        TypingManager.OnCorrectChar -= OnCorrectChar;
        TypingManager.OnWordCorrect -= OnWordCorrect;
        TypingManager.OnCorrectChar += OnCorrectChar;
        TypingManager.OnWordCorrect += OnWordCorrect;
        TypingManager.MuteWordCorrect = true;

        TypingManager.OnWordAdvanced -= OnWordAdvanced;
        TypingManager.OnWordAdvanced += OnWordAdvanced;

        // windows
        attackEvents.OnWindowOpen += MarkOpen;
        attackEvents.OnWindowClose += MarkClose;

        if (_followStun)
        {
            _followStun.onStunEnd.RemoveListener(OnFollowStunEnd);
            _followStun.onStunEnd.AddListener(OnFollowStunEnd);
        }

        if (!string.IsNullOrEmpty(slayerEnterStatePath))
            animator.CrossFadeInFixedTime(slayerEnterStatePath, enterCrossfade, 0, 0f);

        if (logs) Debug.Log(_followStun ? "[Slayer] ENTER (follow STUN)" : $"[Slayer] ENTER {_remain:0.00}s");
        if (autoPlayScreenFX && screenFX) screenFX.EnterSlayerFX();
        if (silhouetteOnInSlayer) SlayerModeSignals.SetActive(true);
        _co = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (true)
        {
            if (_followStun != null)
            {
                // DECAY COMBO (không ảnh hưởng _typedSteps)
                if (comboDecay && _comboF > 0f)
                {
                    float idle = Time.unscaledTime - _lastTypedAt;
                    if (idle >= comboDecayIdleDelay)
                        _comboF = Mathf.Max(0f, _comboF - comboDecayPerSecond * Time.unscaledDeltaTime);
                }

                // Stun END -> chờ grace + đợi cửa sổ attack đóng rồi mới Exit
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
        if (autoPlayScreenFX && screenFX) screenFX.ExitSlayerFX();
        if (silhouetteOnInSlayer) SlayerModeSignals.SetActive(false);
        if (logs) Debug.Log("[Slayer] EXIT");
    }

    // ===== Events / Handlers =====
    void OnFollowStunEnd() { /* Run() sẽ xử lý grace + window-close */ }

    void OnWordCorrect() => _blockUntil = Time.unscaledTime + postWordCooldown;

    void OnWordAdvanced() { if (resetComboOnWord) _comboF = 0f; /* _typedSteps giữ nguyên */ }

    void OnCorrectChar(char _)
    {
        if (!_active) return;
        if (_pendingExit) return;
        float now = Time.unscaledTime;
        if (now < _blockUntil) return;
        if (now - _lastTypedAt < inputDebounce) return;
        _lastTypedAt = now;

        _comboF += 1f;
        _typedSteps += 1;                        // đồng bộ afterimage tint

        // Afterimage (burst) → dùng typedSteps để tint
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


    void OnDisable() => Exit();

    // ================== Public API cho Hitbox ==================
    public float CurrentSlayerMultiplier
    {
        get
        {
            if (!IsActive || !useSlayerRelativeDamage) return 1f;
            int maxSteps = Mathf.Max(1, stepsToMaxTint);
            float t = Mathf.Clamp01(_typedSteps / (float)maxSteps);   // đúng theo afterimage
            float k = dmgMulCurve.Evaluate(t);
            return Mathf.Lerp(dmgMulWhite, dmgMulRed, k);
        }
    }

    // ================== Auto-sync helper ==================
    bool TryGetAfterimageComboMax(SlayerAfterimagePool pool, out int comboMax)
    {
        comboMax = stepsToMaxTint;

        // Ưu tiên property public "ComboMax"
        var prop = pool.GetType().GetProperty("ComboMax", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(int))
        {
            comboMax = (int)prop.GetValue(pool);
            return true;
        }

        // Fallback: field private "comboMax" (đúng tên bản mình đã gửi trước)
        var field = pool.GetType().GetField(afterimageComboMaxFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(int))
        {
            comboMax = (int)field.GetValue(pool);
            return true;
        }

        return false;
    }

    // ================== Debug HUD ==================
    void OnGUI()
    {
        if (!showDebugHUD || !IsActive) return;

        float t = stepsToMaxTint > 0 ? Mathf.Clamp01(_typedSteps / (float)stepsToMaxTint) : 0f;
        float mul = CurrentSlayerMultiplier;

        string text = $"<b>SLAYER</b>  steps {_typedSteps}/{stepsToMaxTint}  t={t:0.00}\nDMG x{mul:0.00}";
        var size = GUI.skin.label.CalcSize(new GUIContent(text));
        var rect = new Rect(hudPosition.x, hudPosition.y, Mathf.Max(180, size.x + 12), size.y + 10);

        var oldColor = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.6f);
        GUI.Box(rect, GUIContent.none);
        GUI.color = Color.white;
        GUI.Label(new Rect(rect.x + 6, rect.y + 4, rect.width - 12, rect.height - 8), text);
        GUI.color = oldColor;
    }
}
