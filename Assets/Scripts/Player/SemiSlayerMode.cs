using UnityEngine;

[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class SemiSlayerMode : MonoBehaviour
{
    const int ChargeToActivate = 100;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerAttackEvents attackEvents;
    [SerializeField] PlayerSlayerMode slayer;
    [SerializeField] SlayerAfterimagePool afterimage;
    [SerializeField] SlashVolume slashTrail;
    [SerializeField] PlayerHealth player;
    [SerializeField] PlayerInputGate gate;
    [SerializeField] ParrySystem parry;

    [Header("Sequence")]
    [SerializeField] string[] attackStates =
    {
        "Base Layer.player_slayer.attackS1",
        "Base Layer.player_slayer.attackS2",
        "Base Layer.player_slayer.attackS3"
    };
    [SerializeField] string speedParam = "AttackSpeedMul";
    [SerializeField] string exitStatePath = "Base Layer.player_idle";
    [SerializeField, Min(0f)] float attackSpeedMultiplier = 3.5f;
    [SerializeField, Min(0f)] float minHitWindowSeconds = 0.035f;
    [SerializeField, Min(0.001f)] float secondsBetweenSwings = 0.1f;
    [SerializeField, Range(1, 10)] int swingsPerUse = 10;

    TypingManager typing;
    ScoreSystem score;
    SkeletonController enemy;
    PlayerHealth boundPlayer;
    ParrySystem boundParry;
    SkeletonController boundEnemy;
    bool active;
    int completedSwings;
    float swingDelay;
    float savedSpeedMultiplier;
    float savedMinHitWindow;

    public bool IsActive => active;
    internal int CompletedSwings => completedSwings;
    public int Charge => EndlessRun.Current != null ? Mathf.Clamp(EndlessRun.Current.SemiSlayerCharge, 0, ChargeToActivate) : 0;
    public float Charge01 => Charge / (float)ChargeToActivate;

    internal static bool IsActiveOn(Animator target)
    {
        var mode = target ? target.GetComponentInParent<SemiSlayerMode>() : null;
        return mode && mode.IsActive;
    }

    void Awake() => ResolveReferences();

    void OnEnable()
    {
        TypingManager.OnWordCorrect += OnWordCorrect;
        ResolveReferences();
        BindSignals();
    }

    void OnDisable()
    {
        TypingManager.OnWordCorrect -= OnWordCorrect;
        UnbindSignals();
        Finish(false);
    }

    void Update()
    {
        ResolveReferences();
        BindSignals();

        if (active)
        {
            if (MustInterrupt()) { Finish(false); return; }
            if (GamePauseController.InputBlocked || Time.timeScale == 0f) return;

            swingDelay -= Time.unscaledDeltaTime;
            if (swingDelay > 0f) return;
            if (completedSwings >= swingsPerUse) { Finish(true); return; }

            ExecuteNextSwing();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && CanActivate()) Begin();
    }

    void ResolveReferences()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!attackEvents) attackEvents = GetComponent<PlayerAttackEvents>();
        if (!slayer) slayer = GetComponent<PlayerSlayerMode>();
        if (!afterimage) afterimage = GetComponent<SlayerAfterimagePool>();
        if (!slashTrail) slashTrail = GetComponentInChildren<SlashVolume>(true);
        if (!player) player = GetComponent<PlayerHealth>();
        if (!gate) gate = GetComponent<PlayerInputGate>();
        if (!parry) parry = GetComponent<ParrySystem>();
        if (!typing) typing = FindObjectOfType<TypingManager>();
        if (!score) score = ScoreSystem.I ? ScoreSystem.I : FindObjectOfType<ScoreSystem>();
        if (!enemy) enemy = FindObjectOfType<SkeletonController>();
    }

    void BindSignals()
    {
        if (boundPlayer != player)
        {
            if (boundPlayer) boundPlayer.OnDamaged -= OnPlayerDamaged;
            boundPlayer = player;
            if (boundPlayer) boundPlayer.OnDamaged += OnPlayerDamaged;
        }
        if (boundParry != parry)
        {
            if (boundParry) boundParry.OnParrySuccess -= OnParrySuccess;
            boundParry = parry;
            if (boundParry) boundParry.OnParrySuccess += OnParrySuccess;
        }
        if (boundEnemy != enemy)
        {
            if (boundEnemy) boundEnemy.OnDeathStarted -= OnEnemyDeathStarted;
            boundEnemy = enemy;
            if (boundEnemy) boundEnemy.OnDeathStarted += OnEnemyDeathStarted;
        }
    }

    void UnbindSignals()
    {
        if (boundPlayer) boundPlayer.OnDamaged -= OnPlayerDamaged;
        if (boundParry) boundParry.OnParrySuccess -= OnParrySuccess;
        if (boundEnemy) boundEnemy.OnDeathStarted -= OnEnemyDeathStarted;
        boundPlayer = null;
        boundParry = null;
        boundEnemy = null;
    }

    bool CanActivate()
    {
        var run = EndlessRun.Current;
        return run != null && run.SemiSlayerCharge >= ChargeToActivate && !TutorialRun.Active &&
            !CombatSession.Finished && !GamePauseController.InputBlocked && Time.timeScale != 0f &&
            !DungeonSession.InputSuppressed && !LeaderboardModal.BlocksShortcuts &&
            typing && !typing.IsReadyPrompt && player && player.Current > 0 &&
            attackEvents && attackEvents.hitbox && animator && enemy && !enemy.IsDead &&
            (!gate || gate.CanAttack) && (!parry || !parry.PoseLocked) &&
            (!slayer || !slayer.IsActive) && !TypingManager.MuteWordCorrect && !active;
    }

    bool CanGainCharge()
    {
        ResolveReferences();
        return EndlessRun.Current != null && !TutorialRun.Active && !active &&
            !CombatSession.Finished && !GamePauseController.InputBlocked &&
            !DungeonSession.InputSuppressed && typing && !typing.IsReadyPrompt &&
            player && player.Current > 0 && enemy && !enemy.IsDead &&
            (!slayer || !slayer.IsActive) && !TypingManager.MuteWordCorrect;
    }

    void OnWordCorrect()
    {
        if (CanGainCharge()) AddCharge(10);
    }

    void OnParrySuccess(ParrySystem.ParryContext _)
    {
        if (CanGainCharge()) AddCharge(20);
    }

    void AddCharge(int amount)
    {
        var run = EndlessRun.Current;
        if (run != null) run.SemiSlayerCharge = Mathf.Min(ChargeToActivate, run.SemiSlayerCharge + amount);
    }

    void Begin()
    {
        var run = EndlessRun.Current;
        if (run == null) return;
        run.SemiSlayerCharge = Mathf.Max(0, run.SemiSlayerCharge - ChargeToActivate);
        active = true;
        completedSwings = 0;
        swingDelay = 0f;
        savedSpeedMultiplier = animator.GetFloat(speedParam);
        savedMinHitWindow = attackEvents.minOpenSeconds;
        attackEvents.minOpenSeconds = minHitWindowSeconds;
        attackEvents.CancelAttack();
        if (afterimage) afterimage.CancelBurst();
        if (typing) typing.SetSemiSlayerHeld(true);
        if (score) score.SetSemiSlayerHeld(true);
        enemy.HoldForSemiSlayer(this);
        ExecuteNextSwing();
    }

    void ExecuteNextSwing()
    {
        if (attackStates == null || attackStates.Length == 0) { Finish(false); return; }
        string state = attackStates[completedSwings % attackStates.Length];
        bool spawnAfterimage = completedSwings > 0;
        if (afterimage) afterimage.CaptureImpactTint(0);
        if (!PlayerSlayerSwing.Execute(animator, attackEvents, slashTrail, afterimage,
            GetComponentInChildren<SpriteRenderer>(true), state, speedParam,
            attackSpeedMultiplier, completedSwings % 3, 0, spawnAfterimage, true,
            PlayerAttackContext.SemiSlayer, out _))
        {
            Finish(false);
            return;
        }
        completedSwings++;
        if (active) swingDelay = secondsBetweenSwings;
    }

    bool MustInterrupt() => CombatSession.Finished || DungeonSession.InputSuppressed ||
        !player || player.Current <= 0 || !enemy || enemy.IsDead;

    void OnPlayerDamaged() { if (active) Finish(false); }
    void OnEnemyDeathStarted()
    {
        if (!active) return;
        if (afterimage) afterimage.CancelBurst();
        Finish(true);
    }

    void Finish(bool returnToIdle)
    {
        if (!active) return;
        active = false;
        swingDelay = 0f;
        if (attackEvents)
        {
            attackEvents.CancelAttack();
            attackEvents.minOpenSeconds = savedMinHitWindow;
        }
        if (slashTrail) slashTrail.EndSemiSlayerSwing();
        if (!returnToIdle && afterimage) afterimage.CancelBurst();
        if (animator)
        {
            animator.SetFloat(speedParam, savedSpeedMultiplier);
            if (returnToIdle && player && player.Current > 0 && (!gate || gate.CanAttack) &&
                !DungeonSession.InputSuppressed && !string.IsNullOrEmpty(exitStatePath))
                animator.CrossFadeInFixedTime(exitStatePath, 0.02f, 0, 0f);
        }
        if (enemy) enemy.ReleaseSemiSlayer(this);
        if (typing) typing.SetSemiSlayerHeld(false);
        if (score) score.SetSemiSlayerHeld(false);
    }
}
