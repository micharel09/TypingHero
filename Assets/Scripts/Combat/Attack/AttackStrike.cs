using UnityEngine;

public class AttackStrike : MonoBehaviour
{
    [Tooltip("Component implement IDamageable (ưu tiên PlayerDamageSfx nếu có).")]
    public MonoBehaviour damageReceiver;

    public int damage = 5;

    public enum HitStopWho { None, Player, Enemy }
    [Header("Hitstop (optional)")] public HitStopWho hitStopTarget = HitStopWho.None;

    IDamageable _target;

    void Awake()
    {
        _target = ResolveTarget();
        Debug.Assert(_target != null,
            "[AttackStrike] damageReceiver phải implement IDamageable hoặc trong scene phải có IDamageable hợp lệ.");
    }

    IDamageable ResolveTarget()
    {
        // 1) Inspector đã gán → nếu là PlayerDamageSfx dùng luôn
        if (damageReceiver is PlayerDamageSfx relay) return relay;

        // 2) Nếu gán PlayerHealth mà cùng GO có PlayerDamageSfx → ưu tiên relay
        if (damageReceiver is PlayerHealth ph)
        {
            var r = ph.GetComponent<PlayerDamageSfx>();
            if (r != null) return r;
            return ph;
        }

        // 3) Chưa gán hoặc gán khác → tìm trong scene
        var foundRelay = Object.FindObjectOfType<PlayerDamageSfx>();
        if (foundRelay != null) return foundRelay;

        var foundHealth = Object.FindObjectOfType<PlayerHealth>();
        if (foundHealth != null) return foundHealth;

        foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>(true))
            if (mb is IDamageable id) return id;

        return null;
    }

    // Animation Event gọi đúng frame va chạm
    public void Anim_DealDamage()
    {
        if (_target == null || _target.IsDead) return;

        Vector2 hitPoint2D = (Vector2)transform.position;
        _target.TakeDamage(damage, hitPoint2D);

        if (HitStopper.I != null)
        {
            if (hitStopTarget == HitStopWho.Player) HitStopper.I.StopPlayerHit();
            else if (hitStopTarget == HitStopWho.Enemy) HitStopper.I.StopEnemyHit();
        }
    }
}
