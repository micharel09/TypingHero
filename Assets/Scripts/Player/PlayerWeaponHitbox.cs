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

    [Header("Slayer (optional)")]
    [SerializeField] PlayerSlayerMode slayer;

    [Header("Stamina drain (normal hit)")]
    [Tooltip("Nếu < 0 sẽ dùng HitCost trong EnemyStamina. Nếu >= 0 sẽ override theo vũ khí này.")]
    [SerializeField] int staminaOnHit = -1;
    [Tooltip("Thời lượng stun khi stamina tụt 0 bởi đòn đánh thường (ngắn hơn Parry).")]
    [SerializeField] float hitStunSeconds = 6f;

    Collider2D col;
    readonly HashSet<IDamageable> hitTargets = new();
    bool active;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
        gameObject.tag = "PlayerWeapon";
        if (!slayer) slayer = GetComponentInParent<PlayerSlayerMode>();
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

        Transform root = other.transform.root;

        // === Reaction rules ===
        if (slayer && slayer.IsActive)
        {
            // Đang stun → giữ stun; nếu không stun → ép hit-react
            if (root.TryGetComponent(out EnemyStunController st) && st.IsStunned)
                st.ReassertNow();
            else if (root.TryGetComponent(out EnemyHitReactGate gate))
                gate.ForceInterrupt();
        }
        else
        {
            // Parry-success bypass bình thường
            if (UninterruptibleBypass.IsActiveFor(root) && root.TryGetComponent(out EnemyHitReactGate g2))
                g2.ForceInterrupt();
        }

        // === DAMAGE (theo tint afterimage trong Slayer) ===
        int applyDamage = damage;
        if (slayer && slayer.IsActive)
        {
            float mul = Mathf.Max(0f, slayer.CurrentSlayerMultiplier);
            applyDamage = Mathf.RoundToInt(damage * mul);
        }
        dmg.TakeDamage(applyDamage, p);

        // === STAMINA DRAIN (đòn đánh thường) ===
        if (root.TryGetComponent(out EnemyStamina stam))
        {
            int before = stam.Current;

            if (staminaOnHit >= 0) stam.ConsumeHit(staminaOnHit);
            else stam.ConsumeHit(); // dùng HitCost trong EnemyStamina

            // Tụt về 0 lần này → Stun ngắn (nhỏ hơn Parry)
            if (before > 0 && stam.Current == 0 && hitStunSeconds > 0f &&
                root.TryGetComponent(out EnemyStunController stun))
            {
                stun.TriggerStun(hitStunSeconds);
                if (slayer) slayer.ActivateForStun(stun);
            }
        }

        if (hitStopper && !(slayer && slayer.IsActive && slayer.IgnoreHitstop))
        {
            float pStop = (hitStopTarget == HitStopTarget.Player || hitStopTarget == HitStopTarget.Both) ? hitStopper.playerHitStop : 0f;
            float eStop = (hitStopTarget == HitStopTarget.Enemy || hitStopTarget == HitStopTarget.Both) ? hitStopper.enemyHitStop : 0f;
            hitStopper.Request(player: pStop, enemy: eStop);
        }
    }
}
