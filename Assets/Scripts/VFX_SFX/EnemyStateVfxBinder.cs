using UnityEngine;

// Gameplay selects the cue; profiles own every visual setting.
[DisallowMultipleComponent]
public sealed class EnemyStateVfxBinder : MonoBehaviour
{
    [SerializeField] SkeletonController health;
    [SerializeField] EnemyStamina stamina;
    [SerializeField] EnemyStunController stun;
    [SerializeField] BoxCollider2D body;
    [SerializeField] Transform deathAnchor;
    [SerializeField] ImpactVfxEmitter guardEmitter, deathEmitter;
    [SerializeField] ImpactVfxProfile guardBreak, bodyFallDust;
    [SerializeField] float floorY;
    bool stunSeen, pendingGuard, pendingDeath, deathStarted, deathImpactSeen;
    Vector3 deathPoint;
    int sequence;

    void OnEnable()
    {
        // Enabling during an existing stun must not replay its start cue.
        stunSeen = stun && stun.IsStunned;
        pendingGuard = pendingDeath = false; deathImpactSeen = false;
        deathStarted = false; // Joining an existing death must not invent a new contact.
        if (stun) { stun.onStunStart?.AddListener(StunStarted); stun.onStunEnd?.AddListener(StunEnded); }
        if (health) health.OnDeathStarted += DeathStarted;
    }
    void OnDisable()
    {
        if (stun) { stun.onStunStart?.RemoveListener(StunStarted); stun.onStunEnd?.RemoveListener(StunEnded); }
        if (health) health.OnDeathStarted -= DeathStarted;
        pendingGuard = pendingDeath = stunSeen = deathStarted = deathImpactSeen = false;
        if (guardEmitter) guardEmitter.Clear();
        // The scene owns deathEmitter: actor destruction must not cancel its tail.
    }
    void StunStarted()
    {
        if (stunSeen) return;
        stunSeen = true;
        if (!health || health.IsDead || !stamina || stamina.Current != 0 || CombatSession.Finished) return;
        pendingGuard = true;
        TryGuard();
    }
    void StunEnded() { stunSeen = pendingGuard = false; }
    void Update()
    {
        if (GamePauseController.ResultOverlayVisible || !health || health.IsDead || CombatSession.Finished)
            pendingGuard = false;
        else if (pendingGuard && stun && stun.IsStunned) TryGuard();
        if (GamePauseController.ResultOverlayVisible) pendingDeath = false;
        else if (pendingDeath) TryDeath();
    }
    void TryGuard()
    {
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        pendingGuard = false;
        if (guardEmitter) guardEmitter.Play(guardBreak, Context(BodyCenter(), ImpactVfxCompletionPolicy.StopOnCombatEnd));
    }
    Vector3 BodyCenter() => body ? body.transform.TransformPoint(body.offset) : transform.position;
    void DeathStarted()
    {
        pendingGuard = stunSeen = false; deathImpactSeen = false; deathStarted = true;
        deathPoint = deathAnchor ? deathAnchor.position : BodyCenter();
        deathPoint.y = floorY;
        if (guardEmitter) guardEmitter.Clear();
    }
    ImpactVfxContext Context(Vector3 point, ImpactVfxCompletionPolicy policy) => new ImpactVfxContext {
        point = point, direction = transform.lossyScale.x < 0 ? -1 : 1, floorY = floorY,
        seed = sequence++, completionPolicy = policy
    };
    // Authored on the first torso/floor contact, never on the later head bounce.
    internal void DeathImpact()
    {
        if (!isActiveAndEnabled || !deathStarted || !health || !health.IsDead || deathImpactSeen || GamePauseController.ResultOverlayVisible) return;
        deathImpactSeen = true;
        pendingDeath = true;
        TryDeath();
    }
    void TryDeath()
    {
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        pendingDeath = false;
        if (deathEmitter) deathEmitter.Play(bodyFallDust, Context(deathPoint, ImpactVfxCompletionPolicy.AllowUntilResults));
    }
}
