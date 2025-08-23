using UnityEngine;

/// Gây damage đúng frame bằng Animation Event (dùng chung)
public class AttackStrike : MonoBehaviour
{
    [Tooltip("Component implement IDamageable (ưu tiên PlayerDamageSfx nếu có).")]
    public MonoBehaviour damageReceiver;

    public int damage = 5;

    public enum HitStopWho { None, Player, Enemy }

    [Header("Hitstop (optional)")]
    public HitStopWho hitStopTarget = HitStopWho.None;

    IDamageable _target;

    void Awake()
    {
        _target = ResolveTarget();
        Debug.Assert(_target != null, "[AttackStrike] damageReceiver phải implement IDamageable hoặc trong scene phải có IDamageable hợp lệ.");
    }

    IDamageable ResolveTarget()
    {
        // 1) Nếu ô inspector đã gán PlayerDamageSfx → dùng luôn
        if (damageReceiver is PlayerDamageSfx sfxRelay) return sfxRelay;

        // 2) Nếu gán PlayerHealth mà cùng GO có PlayerDamageSfx → ưu tiên Sfx
        if (damageReceiver is PlayerHealth ph)
        {
            var relay = ph.GetComponent<PlayerDamageSfx>();
            if (relay != null) return relay;
            return ph;
        }

        // 3) Nếu chưa gán hoặc gán thứ khác: tìm PlayerDamageSfx trong scene
        var foundRelay = Object.FindObjectOfType<PlayerDamageSfx>();
        if (foundRelay != null) return foundRelay;

        // 4) Fallback: tìm PlayerHealth
        var foundHealth = Object.FindObjectOfType<PlayerHealth>();
        if (foundHealth != null) return foundHealth;

        // 5) Fallback cuối: bất kỳ IDamageable nào
        foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>(true))
            if (mb is IDamageable id) return id;

        return null;
    }

    // Animation Event sẽ gọi hàm này đúng frame
    public void Anim_DealDamage()
    {
        if (_target == null || _target.IsDead) return;

        // CHUẨN CHỮ KÝ: interface dùng Vector2
        Vector2 hitPoint2D = (Vector2)transform.position;
        _target.TakeDamage(damage, hitPoint2D);

        // Hitstop nếu có
        if (HitStopper.I != null)
        {
            if (hitStopTarget == HitStopWho.Player) HitStopper.I.StopPlayerHit();
            else if (hitStopTarget == HitStopWho.Enemy) HitStopper.I.StopEnemyHit();
        }
    }
}
