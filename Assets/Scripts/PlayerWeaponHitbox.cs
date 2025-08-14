using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerWeaponHitbox : MonoBehaviour
{
    [Header("Damage")] public int damage = 10;
    [Header("Who can be hit")] public LayerMask targetLayers; // Enemy

    Collider2D col;
    readonly HashSet<IDamageable> hitTargets = new();
    bool active = false;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    // gọi từ Animation Event
    public void BeginAttack()
    {
        hitTargets.Clear();
        active = true;
        col.enabled = true;
    }

    // gọi từ Animation Event
    public void EndAttack()
    {
        active = false;
        col.enabled = false;
        hitTargets.Clear();
    }

    void OnTriggerEnter2D(Collider2D other) { TryHit(other); }
    void OnTriggerStay2D(Collider2D other) { TryHit(other); }

    void TryHit(Collider2D other)
    {
        if (!active) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;
        if (!hitTargets.Add(dmg)) return; // đã trúng trong nhát này rồi

        Vector2 p = other.ClosestPoint(transform.position);
        dmg.TakeDamage(damage, p);
        if (HitStopper.I) HitStopper.I.StopEnemyHit();
    }

}
