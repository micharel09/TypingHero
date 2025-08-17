using UnityEngine;

/// Gây damage ở đúng frame bằng Animation Event (dùng chung cho mọi enemy / mọi clip)
public class AttackStrike : MonoBehaviour
{
    [Tooltip("Nơi nhận damage (kéo component implement IDamageable vào, ví dụ PlayerHealth)")]
    public MonoBehaviour damageReceiver;

    public int damage = 5;

    IDamageable _target;

    void Awake()
    {
        if (damageReceiver != null) _target = damageReceiver as IDamageable;
        if (_target == null)
            Debug.LogWarning("[AttackStrike] Chưa gán damageReceiver hoặc component không implement IDamageable.");
    }

    // Animation Event: đặt ở frame gây sát thương

    public void Anim_DealDamage()
    {
        if (_target == null || _target.IsDead) return;

        _target.TakeDamage(damage, transform.position);
    }
}
