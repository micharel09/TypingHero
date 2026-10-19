using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class SkeletonController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] GamePauseController gamePause;

    public UnityEvent onDied;
    [Header("Stats")]
    public int maxHealth = 500;
    public int Current { get; private set; }
    public bool IsDead { get; private set; }

    [Header("Attack Clock")]
    [SerializeField] float attackInterval = 1.5f;

    [Header("Attack Animation")]
    [SerializeField] string attackStatePath = "Base Layer.skeleton_attack_heavy";

    [Header("Damage")]
    [SerializeField] int attackDamage = 5;

    [Header("Hit React")]
    [SerializeField] bool uninterruptibleDuringAttack = true;
    [SerializeField] string hitStatePath = "Base Layer.skeleton_hit";
    [SerializeField, Range(0f, .2f)] float hitCrossfade = 0.02f;
    [SerializeField] bool queueHitReactAfterAttack = false;

    [Header("Death (Animator)")]
    [SerializeField] string dieStatePath = "Base Layer.skeleton_die";
    [SerializeField] string dieBoolParam = "isDead";
    [SerializeField, Range(0f, .2f)] float dieCrossfade = 0.02f;

    [Header("Debug")][SerializeField] bool logs;

    // == EVENTS ==
    public event Action OnDeathStarted;
    public event Action OnDeathFinished;

    float _nextAttackAt;
    bool _attacking;
    bool _queuedHitReact;
    bool _deathEventFired;

    float Now => Time.time;
    public bool IsAttacking => _attacking;
    bool practiceIdle;
    internal void HoldPracticeAttacks(bool hold)
    {
        practiceIdle = hold;
        _nextAttackAt = Now + .5f;
    }
    [SerializeField] GameObject attackCue;
    public void Anim_Telegraph()
    {
        if (!isActiveAndEnabled || !_attacking || IsDead || CombatSession.Finished || GamePauseController.InputBlocked) return;
        if (attackCue) attackCue.SetActive(true);
        var sound = GetComponent<SfxOnEnemyAttack>();
        if (sound) sound.Anim_EnemyAttackSfx();
    }
    public void HideCue() { if (attackCue) attackCue.SetActive(false); }
    void OnDisable() { _attacking = false; HideCue(); }

    void Awake()
    {
        if (Current <= 0) Current = maxHealth;
    }

    void OnEnable() { _attacking = false; _nextAttackAt = Now + 0.5f; }

    void Update()
    {
        if (practiceIdle || IsDead || CombatSession.Finished || GamePauseController.InputBlocked) return;

        if (!_attacking && Now >= _nextAttackAt) { StartAttack(); return; }

        if (_attacking)
        {
            if (!AnimatorUtil.IsInState(animator, attackStatePath, out var info) || info.normalizedTime >= 0.98f)
                EndAttack();
        }
    }

    void StartAttack()
    {
        _attacking = true;
        _queuedHitReact = false;
        if (animator && !string.IsNullOrEmpty(attackStatePath))
            animator.Play(attackStatePath, 0, 0f);
        var strike = GetComponent<AttackStrike>();
        if (strike) strike.BeginSwing();
    }

    void EndAttack()
    {
        _attacking = false;
        HideCue();
        _nextAttackAt = Now + attackInterval;

        if (_queuedHitReact && !IsDead && animator && !string.IsNullOrEmpty(hitStatePath))
        {
            _queuedHitReact = false;
            AnimatorUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
        }
    }

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead || CombatSession.Finished || GamePauseController.InputBlocked) return;

        Current = Mathf.Max(0, Current - amount);

        if (Current <= 0) { Die(); return; }

        bool inAttack = AnimatorUtil.IsInState(animator, attackStatePath, out _);
        if (uninterruptibleDuringAttack && inAttack)
        {
            if (queueHitReactAfterAttack) _queuedHitReact = true;
            return;
        }

        if (animator && !string.IsNullOrEmpty(hitStatePath))
            AnimatorUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
    }

    void Die()
    {
        if (IsDead || CombatSession.Finished || GamePauseController.InputBlocked) return;
        IsDead = true;
        HideCue();
        if (CombatSession.I) CombatSession.I.Complete(true);
        onDied?.Invoke();
        if (!string.IsNullOrEmpty(dieBoolParam) && animator) animator.SetBool(dieBoolParam, true);
        if (animator && !string.IsNullOrEmpty(dieStatePath))
            AnimatorUtil.CrossFadePath(animator, dieStatePath, dieCrossfade, 0f);

        OnDeathStarted?.Invoke(); // << báo cho SlayerMode thoát ngay
    }

    // Gọi bằng Animation Event ở FRAME CUỐI clip skeleton_die
    public void Anim_DieCleanup()
    {
        if (_deathEventFired) return;
        _deathEventFired = true;

        if (gamePause && !gamePause.IsRestarting)
            gamePause.ShowThanksSimple();

        OnDeathFinished?.Invoke();
        Destroy(gameObject);
    }
}
