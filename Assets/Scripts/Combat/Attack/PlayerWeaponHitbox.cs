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
    [SerializeField] SlashLibrary slashLibrary;
    [SerializeField] SpriteRenderer slashSource;
    Sprite contactCel;
    internal void SetContactCel(Sprite cel) => contactCel = cel;
    internal int AttackId { get; private set; }
    internal int LastHitFrame { get; private set; } = -1;
    internal Vector2 LastContact { get; private set; }
    bool active;
    readonly List<Collider2D> overlaps = new List<Collider2D>(32);

    internal void ConfirmOverlaps()
    {
        var filter = new ContactFilter2D(); filter.SetLayerMask(targetLayers); filter.useTriggers = true;
        col.OverlapCollider(filter, overlaps);
        for (int i = 0; i < overlaps.Count; i++) TryHit(overlaps[i]);
        overlaps.Clear();
    }

    //HitSpark
    [Header("VFX (optional)")]
    [SerializeField] HitSparkSpawner hitSpark;
    [SerializeField] CombatCameraImpact cameraImpact;

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
        AttackId = attackId;
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

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);

    void TryHit(Collider2D other)
    {
        if (!isActiveAndEnabled || !active || GamePauseController.InputBlocked || CombatSession.Finished) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        if (Physics2D.GetIgnoreLayerCollision(col.gameObject.layer, other.gameObject.layer) || Physics2D.GetIgnoreCollision(col, other)) return;
        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;
        if (hitTargets.Contains(dmg)) return;

        hitTargets.Add(dmg);
        Vector2 p = other.ClosestPoint(transform.position);
        if (slashLibrary && slashSource && contactCel && slashLibrary.TryContact(contactCel, slashSource, other, out var contact)) p = contact;
        Transform root = other.transform.root;

        bool wasSlayer = slayer && slayer.IsActive;
        // Capture before damage/hit poses can move or disable the recipient.
        Vector2? slayerVisualPoint = wasSlayer ? (Vector2)other.bounds.center : (Vector2?)null;
        bool ignoreHitstop = wasSlayer && slayer.IgnoreHitstop;
        var images = wasSlayer ? transform.root.GetComponentInChildren<SlayerAfterimagePool>() : null;
        Color impactTint = images ? images.CurrentTint : Color.clear;
        Vector2? slayerDirection = wasSlayer ? slayer.StrikeDirection : (Vector2?)null;

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
        LastHitFrame = Time.frameCount; LastContact = p;
        dmg.TakeDamage(applyDamage, p);
        if (wasSlayer && !dmg.IsDead && root.TryGetComponent(out EnemySlayerHitPose pose))
        {
            pose.OnSlayerHit();
        }
        if (wasSlayer && root.TryGetComponent<SlayerSilhouetteGroup>(out var silhouette))
            silhouette.PulseSlayerHit();

        // (sau đó mới tới HitFlash, HitSpark, Stamina, Hitstop…)

        // === HIT FLASH (tùy chọn) ===
        if (root.TryGetComponent<EnemyHitFlash>(out var flash))
        {
            if (!wasSlayer) flash.Tick();
        }

        // === HIT SPARK (tùy chọn) ===
        if (hitSpark) hitSpark.SpawnImpact(p, transform.root, wasSlayer, dmg.IsDead, root, capturedTint: wasSlayer ? impactTint : (Color?)null, slayerVisualPoint: slayerVisualPoint, slayerDirection: slayerDirection);



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

        if (cameraImpact && applyDamage > 0)
            cameraImpact.Hit(p, root.position - transform.root.position, wasSlayer, dmg.IsDead);

        if (hitStopper && !ignoreHitstop)
        {
            float pStop = (hitStopTarget == HitStopTarget.Player || hitStopTarget == HitStopTarget.Both) ? hitStopper.playerHitStop : 0f;
            float eStop = (hitStopTarget == HitStopTarget.Enemy || hitStopTarget == HitStopTarget.Both) ? hitStopper.enemyHitStop : 0f;
            hitStopper.Request(player: pStop, enemy: eStop);
        }
    }

}
