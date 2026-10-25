using System;
using System.Collections.Generic;
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
    EndlessRun endless;
    int pattern, remainingSwings, swingIndex;
    int[] chain;
    float prepareUntil = -1;
    bool preparing, attackPending, cueAnnounced, thrustCue;
    public int AttackSequenceId { get; private set; }
    const float WarningSeconds = 1f;
    float swingEndsAt;
    float attackOneLength, attackTwoLength;
    int chainParries;
    internal string LastAttack { get; private set; } = "Attack1";
    EndlessOpponent definition;

    internal void ConfigureEndless(EndlessRun run)
    {
        endless = run;
        maxHealth = Current = run.Enemy.Health;
        attackInterval = run.Recovery;
        var stamina = GetComponent<EnemyStamina>();
        if (stamina) { stamina.max = run.Enemy.Stamina; stamina.PreparePractice(stamina.max); }
        definition = run.Definition;
        if (stamina) { stamina.hitCost = definition.hitCost; stamina.parryCost = 25; }
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite) { sprite.color = Color.white; sprite.sprite = definition.idle; }
        animator.runtimeAnimatorController = definition.controller;
        CacheAttackEvents();
        foreach (var clip in definition.controller.animationClips)
        {
            if (clip.name == "Attack1") attackOneLength = clip.length;
            if (clip.name == "Attack2") attackTwoLength = clip.length;
        }
        animator.speed = 1;
        animator.Play("Base Layer.skeleton_idle", 0, 0);
        animator.Update(0);
        var slayerPose = GetComponent<EnemySlayerHitPose>();
        if (slayerPose) slayerPose.ConfigureHurt(definition.hurt);
        pattern = remainingSwings = 0; preparing = false; HideCue();
        _attacking = false; _nextAttackAt = Now + .5f;

    }

    RuntimeAnimatorController telegraphController;
    readonly List<AnimatorClipInfo> telegraphClips = new List<AnimatorClipInfo>();
    readonly Dictionary<AnimationClip, float> impactTimes = new Dictionary<AnimationClip, float>();
    void CacheAttackEvents()
    {
        telegraphController = animator ? animator.runtimeAnimatorController : null;
        impactTimes.Clear();
        if (!telegraphController) return;
        foreach (var clip in telegraphController.animationClips)
            foreach (var evt in clip.events)
                if (evt.functionName == "Anim_DealDamage")
                { impactTimes[clip] = evt.time; break; }
    }

    // Read-only: seconds of game time until the current attack's authored strike event.
    public bool TryGetAttackTelegraph(out float secondsToImpact)
    {
        secondsToImpact = 0;
        if (!isActiveAndEnabled || !_attacking || IsDead || CombatSession.Finished || !attackPending || !animator) return false;
        if (telegraphController != animator.runtimeAnimatorController) CacheAttackEvents();
        if (!AnimatorUtil.IsInState(animator, attackStatePath, out var state)) return false;
        if (animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).fullPathHash != state.fullPathHash) return false;
        animator.GetCurrentAnimatorClipInfo(0, telegraphClips);
        if (telegraphClips.Count == 0 || !impactTimes.TryGetValue(telegraphClips[0].clip, out var impact)) return false;
        var clip = telegraphClips[0].clip;
        float remaining = impact - state.normalizedTime * clip.length;
        if (remaining < 0) return false;
        float speed = Mathf.Abs(state.speed * state.speedMultiplier * (preparing ? 1 : animator.speed));
        if (speed <= 0) return false;
        secondsToImpact = remaining / speed + (preparing ? Mathf.Max(0, prepareUntil - Now) : 0);
        return true;
    }

    internal GameObject AttackCueObject => attackCue;
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
        RevealCue();
    }
    void RevealCue()
    {
        if (cueAnnounced || !attackPending) return;
        cueAnnounced = true;
        if (attackCue) attackCue.SetActive(true);
        var sound = GetComponent<SfxOnEnemyAttack>();
        if (sound) sound.Anim_EnemyAttackSfx();
    }
    internal bool CueActive => attackCue && attackCue.activeInHierarchy;
    public void HideCue() { attackPending = false; if (attackCue) attackCue.SetActive(false); }
    void OnDisable()
    {
        _attacking = false; remainingSwings = 0; preparing = false;
        var strike = GetComponent<AttackStrike>();
        if (strike) strike.CancelSwing();
        if (animator) animator.speed = 1;
        HideCue();
    }

    void Awake()
    {
        if (Current <= 0) Current = maxHealth;
        CacheAttackEvents();
    }

    bool introHandoff;
    internal void BeginIntroCombat() { introHandoff = true; _nextAttackAt = Now + .5f; }

    void OnEnable() { _attacking = false; _nextAttackAt = Now + 0.5f; }

    void Update()
    {
        if (practiceIdle || IsDead || CombatSession.Finished || GamePauseController.InputBlocked) return;
        if (introHandoff) { introHandoff = false; _nextAttackAt = Now + .5f; }
        if (preparing && (!AnimatorUtil.IsInState(animator, attackStatePath, out var heldState) ||
            (animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).fullPathHash != heldState.fullPathHash)))
        {
            preparing = false; animator.speed = 1;
            HideCue();
            var canceledStrike = GetComponent<AttackStrike>();
            if (canceledStrike) canceledStrike.CancelSwing();
        }
        // Thrust has no telegraph event; retain its original final-300ms sound/cue.
        if (thrustCue && !cueAnnounced && TryGetAttackTelegraph(out float contactIn) && contactIn <= .3f)
            RevealCue();
        if (preparing)
        {
            if (Now < prepareUntil) return;
            preparing = false; animator.speed = 1;
        }

        if (!_attacking && Now >= _nextAttackAt) { StartAttack(); return; }

        if (_attacking)
        {
            bool inAttack = AnimatorUtil.IsInState(animator, attackStatePath, out var info);
            if (!inAttack && endless != null)
            {
                // A parry riposte may play Hurt. It must not accelerate the next
                // strike into the player's still-locked success pose.
                HideCue();
                var strike = GetComponent<AttackStrike>();
                if (strike) strike.CancelSwing();
                if (Now < swingEndsAt) return;
            }
            if (!inAttack || info.normalizedTime >= (endless != null ? 1f : .98f))
                EndAttack();
        }
    }

    void StartAttack()
    {
        if (endless != null)
        {
            if (remainingSwings == 0)
            {
                chain = definition.Pattern(pattern++, endless.Round, (float)Current / maxHealth);
                remainingSwings = chain.Length; swingIndex = 0; chainParries = 0;
            }
            int attack = chain[swingIndex++];
            attackStatePath = attack == 1 ? "Base Layer.skeleton_attack_heavy" : "Base Layer.skeleton_attack2";
            LastAttack = "Attack" + attack;
        }
        _attacking = true;
        _queuedHitReact = false;
        attackPending = true; cueAnnounced = false;
        AttackSequenceId++;
        if (attackCue) attackCue.SetActive(false);
        thrustCue = endless != null && LastAttack == "Attack2" &&
            definition.role != EndlessRole.Heavy && definition.role != EndlessRole.Berserker;
        if (animator && !string.IsNullOrEmpty(attackStatePath))
        {
            animator.speed = 1;
            animator.Play(attackStatePath, 0, 0f);
            animator.Update(0);
            if (endless != null) swingEndsAt = Now + (LastAttack == "Attack1" ? attackOneLength : attackTwoLength);
            // Extend preparation only. Authored swing frames, speed and event offsets stay intact.
            if (TryGetAttackTelegraph(out float authoredContact))
            {
                float hold = Mathf.Max(0, WarningSeconds - authoredContact);
                preparing = hold > 0;
                if (preparing)
                {
                    prepareUntil = Now + hold;
                    animator.speed = 0;
                    swingEndsAt += hold;
                }
            }
        }
        var strike = GetComponent<AttackStrike>();
        if (strike)
        {
            if (endless != null) strike.BeginSwing(definition.damage);
            else strike.BeginSwing();
        }
    }

    internal void RecordParry()
    {
        if (endless == null || chain == null || !_attacking) return;
        chainParries++;
        if (chain.Length == 3 && chainParries == 3) EndlessProfile.Unlock(EndlessProfile.TripleParry);
    }

    void EndAttack()
    {
        _attacking = false;
        HideCue();
        if (endless != null)
        {
            remainingSwings = Mathf.Max(0, remainingSwings - 1);
            if (animator) animator.speed = 1;
        }
        _nextAttackAt = Now + (remainingSwings > 0 ? .12f : attackInterval);
        if (endless != null && animator) animator.Play("Base Layer.skeleton_idle", 0, 0);

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
        remainingSwings = 0; preparing = false;
        var strike = GetComponent<AttackStrike>();
        if (strike) strike.CancelSwing();
        if (animator) animator.speed = 1;
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

        if (EndlessDuel.I) EndlessDuel.I.EnemyDeathFinished();
        else if (gamePause && !gamePause.IsRestarting)
            gamePause.ShowThanksSimple();

        OnDeathFinished?.Invoke();
        Destroy(gameObject);
    }
}
