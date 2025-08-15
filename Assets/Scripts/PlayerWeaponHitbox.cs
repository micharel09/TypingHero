using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerWeaponHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;

    [Header("Who can be hit")]
    public LayerMask targetLayers; // đặt Enemy

    Collider2D col;
    readonly HashSet<IDamageable> hitTargets = new();

    bool active = false;
    bool suppressNextDamage = false;

    public bool Active => active;                                   // <-- dùng cho clash check
    public bool HasDealtDamageThisSwing => hitTargets.Count > 0;    // <-- tránh “đã đánh trúng trước đó”
    public Collider2D Collider => col;                               // <-- dùng overlap
    public void SuppressNextDamage() => suppressNextDamage = true;   // <-- chặn 1 hit

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void BeginAttack()
    {
        hitTargets.Clear();
        suppressNextDamage = false;
        active = true;
        col.enabled = true;
    }

    public void EndAttack()
    {
        active = false;
        col.enabled = false;
        hitTargets.Clear();
        suppressNextDamage = false;
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    void TryHit(Collider2D other)
    {
        if (!active) return;
        if (suppressNextDamage) { suppressNextDamage = false; return; }   // <-- nếu vừa clash thì bỏ 1 lần gây damage
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;

        if (!hitTargets.Add(dmg)) return;

        Vector2 p = other.ClosestPoint(transform.position);
        dmg.TakeDamage(damage, p);

        if (HitStopper.I) HitStopper.I.StopEnemyHit();
    }
}
