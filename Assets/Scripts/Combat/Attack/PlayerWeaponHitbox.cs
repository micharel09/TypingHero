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
    [SerializeField] int staminaOnHit = -1;
    [SerializeField] float hitStunSeconds = 6f;
    Collider2D col;
    readonly HashSet<IDamageable> hitTargets = new();
    bool active;

    //HitSpark
    [Header("VFX (optional)")]
    [SerializeField] HitSparkSpawner hitSpark;

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

    // ... giữ nguyên phần khai báo ở trên

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled || !active || GamePauseController.InputBlocked || CombatSession.Finished) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;
        if (hitTargets.Contains(dmg)) return;

        hitTargets.Add(dmg);
        Vector2 p = other.ClosestPoint(transform.position);
        Transform root = other.transform.root;

        bool wasSlayer = slayer && slayer.IsActive;
        bool ignoreHitstop = wasSlayer && slayer.IgnoreHitstop;

        // === Reaction rules ===
        if (wasSlayer)
        {
            // QUAN TRỌNG: KHÔNG reassert stun trong Slayer để nhường hit-pose
            if (root.TryGetComponent(out EnemyHitReactGate gate))
                gate.ForceInterrupt();
        }
        else
        {
            if (UninterruptibleBypass.IsActiveFor(root) && root.TryGetComponent(out EnemyHitReactGate g2))
                g2.ForceInterrupt();
        }

        // === DAMAGE ===
        int applyDamage = damage;
        if (wasSlayer)
        {
            float mul = Mathf.Max(0f, slayer.CurrentSlayerMultiplier);
            applyDamage = Mathf.RoundToInt(damage * mul);
        }
        dmg.TakeDamage(applyDamage, p);
        if (wasSlayer && !dmg.IsDead && root.TryGetComponent(out EnemySlayerHitPose pose))
        {
            pose.OnSlayerHit();
        }

        // (sau đó mới tới HitFlash, HitSpark, Stamina, Hitstop…)

        // === HIT FLASH (tùy chọn) ===
        if (root.TryGetComponent<EnemyHitFlash>(out var flash))
        {
            if (!wasSlayer) flash.Tick();
        }

        // === HIT SPARK (tùy chọn) ===
        if (hitSpark) hitSpark.SpawnImpact(p, transform.root, wasSlayer, dmg.IsDead, root);



        // === STAMINA DRAIN ===
        if (root.TryGetComponent(out EnemyStamina stam))
        {
            int before = stam.Current;
            if (staminaOnHit >= 0) stam.ConsumeHit(staminaOnHit);
            else stam.ConsumeHit();

            if (before > 0 && stam.Current == 0 && hitStunSeconds > 0f &&
                root.TryGetComponent(out EnemyStunController stun))
            {
                stun.TriggerStun(hitStunSeconds);
                if (slayer) slayer.ActivateForStun(stun);
            }
        }

        if (hitStopper && !ignoreHitstop)
        {
            float pStop = (hitStopTarget == HitStopTarget.Player || hitStopTarget == HitStopTarget.Both) ? hitStopper.playerHitStop : 0f;
            float eStop = (hitStopTarget == HitStopTarget.Enemy || hitStopTarget == HitStopTarget.Both) ? hitStopper.enemyHitStop : 0f;
            hitStopper.Request(player: pStop, enemy: eStop);
        }
    }

}
