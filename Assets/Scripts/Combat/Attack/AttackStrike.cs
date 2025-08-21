using UnityEngine;

/// Gây damage đúng frame bằng Animation Event (dùng chung)
public class AttackStrike : MonoBehaviour
{
    [Tooltip("Nơi nhận damage (kéo component implement IDamageable vào, ví dụ PlayerHealth)")]
    public MonoBehaviour damageReceiver;

    public int damage = 5;

    public enum HitStopWho { None, Player, Enemy }

    [Header("Hitstop (optional)")]
    public HitStopWho hitStopTarget = HitStopWho.None;

    IDamageable _target;

    void Awake()
    {
        if (damageReceiver is IDamageable id) _target = id;
        else Debug.LogWarning("[AttackStrike] Chưa gán damageReceiver hoặc component không implement IDamageable.");
    }

    // Animation Event: đặt ở frame gây sát thương
    public void Anim_DealDamage()
    {
        if (_target == null || _target.IsDead) return;

        _target.TakeDamage(damage, transform.position);

        // Gọi hitstop nếu có
        if (HitStopper.I != null)
        {
            if (hitStopTarget == HitStopWho.Player) HitStopper.I.StopPlayerHit();
            else if (hitStopTarget == HitStopWho.Enemy) HitStopper.I.StopEnemyHit();
        }
    }
}
