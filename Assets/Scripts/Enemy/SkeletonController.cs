using System;
using UnityEngine;

public class SkeletonController : MonoBehaviour, IDamageable
{
    [Header("References")]
    public Animator animator;
    public Transform target;
    public PlayerHealth playerHealth;

    [Header("Stats")]
    public int health = 500;
    public bool IsDead { get; private set; }

    [Header("Attack Clock")]
    public float attackInterval = 1.5f;
    public bool useUnscaledClock = true;

    [Header("Attack Animation")]
    public string attackStatePath = "Base Layer.skeleton_attack_heavy";
    [Range(0f, .2f)] public float attackCrossfade = 0.02f;
    public float attackStartTime = 0f;

    [Header("Damage")]
    public int attackDamage = 5;

    [Header("Hit React")]
    public bool uninterruptibleDuringAttack = true;
    public string hitStatePath = "Base Layer.skeleton_hit";
    [Range(0f, .2f)] public float hitCrossfade = 0.02f;
    public bool queueHitReactAfterAttack = true;

    [Header("Debug")]
    public bool logs = false;

    float _nextAttackAt;
    bool _attacking;
    bool _queuedHitReact;

    float Now => useUnscaledClock ? Time.unscaledTime : Time.time;

    void OnEnable()
    {
        _nextAttackAt = Now + 0.5f;
    }

    void Update()
    {
        if (IsDead) return;

        if (!_attacking && Now >= _nextAttackAt)
            StartAttack();

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

        if (animator)
            AnimatorUtil.CrossFadePath(animator, attackStatePath, attackCrossfade, attackStartTime);

        if (logs) Debug.Log("[SKE] Start attack");
    }

    void EndAttack()
    {
        _attacking = false;
        _nextAttackAt = Now + attackInterval;

        if (_queuedHitReact && !IsDead)
        {
            if (TryGetComponent(out EnemyStunController stun) && stun.IsStunned)
            {
                _queuedHitReact = false;
                if (logs) Debug.Log("[SKE] Skip queued hit-react (STUN)");
            }
            else
            {
                _queuedHitReact = false;
                if (animator) AnimatorUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
                if (logs) Debug.Log("[SKE] Play queued hit-react");
            }
        }
        if (logs) Debug.Log("[SKE] End attack");
    }

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead) return;

        health -= amount;
        if (logs) Debug.Log($"[SKE] Hit {amount}, HP: {health}");
        if (health <= 0) { Die(); return; }

        if (TryGetComponent(out EnemyStunController stun) && stun.IsStunned)
        {
            if (logs) Debug.Log("[SKE] Damage while STUN → keep stun");
            return;
        }

        bool inAttackNow = AnimatorUtil.IsInState(animator, attackStatePath, out _);
        if (uninterruptibleDuringAttack && inAttackNow)
        {
            if (queueHitReactAfterAttack) _queuedHitReact = true;
            if (logs) Debug.Log("[SKE] Got hit mid-swing → queued hit-react");
            return;
        }
        if (animator) AnimatorUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
    }

    void Die()
    {
        IsDead = true;
        if (logs) Debug.Log("[SKE] Dead");
        Destroy(gameObject, 2f);
    }
}
