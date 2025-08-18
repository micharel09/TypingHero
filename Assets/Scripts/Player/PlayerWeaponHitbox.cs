using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerWeaponHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;

    [Header("Who can be hit")]
    public LayerMask targetLayers;

    public enum HitStopTarget { None, Player, Enemy, Both }
    [Header("Hitstop (optional)")]
    public HitStopper hitStopper;
    public HitStopTarget hitStopTarget = HitStopTarget.Both;

    Collider2D col;
    readonly HashSet<IDamageable> hitTargets = new();
    bool active;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
        gameObject.tag = "PlayerWeapon";
    }

    public void BeginAttack(int attackId)
    {
        hitTargets.Clear();
        active = true;
        col.enabled = true;
    }

    public void EndAttack()
    {
        active = false;
        col.enabled = false;
        hitTargets.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;
        if (hitTargets.Contains(dmg)) return;

        hitTargets.Add(dmg);
        Vector2 p = other.ClosestPoint(transform.position);

        // Parry Success → nếu đang STUN thì giữ STUN, còn không thì ép Hit React
        Transform root = other.transform.root;
        if (UninterruptibleBypass.IsActiveFor(root))
        {
            if (root.TryGetComponent(out EnemyStunController stun) && stun.IsStunned)
                stun.ReassertNow();
            else if (root.TryGetComponent(out EnemyHitReactGate gate))
                gate.ForceInterrupt();
        }

        dmg.TakeDamage(damage, p);

        if (hitStopper)
        {
            float pStop = (hitStopTarget == HitStopTarget.Player || hitStopTarget == HitStopTarget.Both) ? hitStopper.playerHitStop : 0f;
            float eStop = (hitStopTarget == HitStopTarget.Enemy  || hitStopTarget == HitStopTarget.Both) ? hitStopper.enemyHitStop : 0f;
            hitStopper.Request(player: pStop, enemy: eStop);
        }
    }
}
