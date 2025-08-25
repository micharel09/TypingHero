// === AttackStrike.cs (drop-in) ===
using UnityEngine;

public class AttackStrike : MonoBehaviour
{
    [Tooltip("Component implement IDamageable (ưu tiên PlayerDamageSfx nếu có).")]
    [SerializeField] MonoBehaviour damageReceiver;     // optional, có thể để trống

    [Header("Damage")]
    [SerializeField] int damage = 5;

    public enum HitStopWho { None, Player, Enemy }
    [Header("Hitstop (optional)")]
    public HitStopWho hitStopTarget = HitStopWho.None;

    [Header("Debug")]
    [SerializeField] bool logs;

    IDamageable _target;
    float _parryLockUntilUnscaled = -1f;

    // ---------------- API ----------------

    /// <summary> Khóa gây damage trong ttl (giây, unscaled), gọi khi parry thành công. </summary>
    public void ParryLock(float ttl)
    {
        float until = Time.unscaledTime + Mathf.Max(0f, ttl);
        if (until > _parryLockUntilUnscaled) _parryLockUntilUnscaled = until;
        if (logs) Debug.Log($"[AttackStrike] ParryLock for {ttl:0.###}s (until={_parryLockUntilUnscaled:0.###})");
    }

    /// <summary> Alias rõ nghĩa khi nhận event parry. </summary>
    public void OnParried(float invulnerableSeconds) => ParryLock(invulnerableSeconds);

    public bool IsParryLocked => Time.unscaledTime < _parryLockUntilUnscaled;

    // -------------- Unity lifecycle --------------

    void Awake()
    {
        // Không assert cứng để tránh fail trong prefab/scene load order.
        TryResolveTarget();
        if (_target == null && logs)
            Debug.LogWarning("[AttackStrike] Chưa tìm được IDamageable ở Awake; sẽ thử lại khi gây damage.");
    }

    // -------------- Core --------------

    // Animation Event gọi đúng frame va chạm
    public void Anim_DealDamage()
    {
        // BỊ KHÓA DO PARRY → BỎ QUA
        if (IsParryLocked)
        {
            if (logs) Debug.Log("[AttackStrike] Damage blocked by ParryLock.");
            return;
        }

        // Đảm bảo có target (lazy resolve nếu cần)
        if (_target == null) TryResolveTarget();
        if (_target == null) { if (logs) Debug.LogWarning("[AttackStrike] Không có target IDamageable."); return; }
        if (_target.IsDead) { if (logs) Debug.Log("[AttackStrike] Target đã chết, bỏ qua."); return; }

        // Gây damage
        Vector2 hitPoint2D = (Vector2)transform.position;
        _target.TakeDamage(damage, hitPoint2D);

        // Hitstop (nếu có)
        if (HitStopper.I != null)
        {
            if (hitStopTarget == HitStopWho.Player) HitStopper.I.StopPlayerHit();
            else if (hitStopTarget == HitStopWho.Enemy) HitStopper.I.StopEnemyHit();
        }

        if (logs) Debug.Log($"[AttackStrike] DealDamage={damage} at {hitPoint2D}.");
    }

    // -------------- Helpers --------------

    void TryResolveTarget()
    {
        _target = ResolveTarget(damageReceiver);
    }

    static IDamageable ResolveTarget(MonoBehaviour receiverHint)
    {
        // 1) Inspector đã gán → nếu là PlayerDamageSfx dùng luôn (ưu tiên SFX relay)
        if (receiverHint is PlayerDamageSfx relayFromHint) return relayFromHint;

        // 2) Nếu gán PlayerHealth mà cùng GO có PlayerDamageSfx → ưu tiên relay
        if (receiverHint is PlayerHealth ph)
        {
            var r = ph.GetComponent<PlayerDamageSfx>();
            if (r != null) return r;
            return ph;
        }

        // 3) Chưa gán hoặc gán khác → tìm trong scene (ưu tiên PlayerDamageSfx)
        var foundRelay = Object.FindObjectOfType<PlayerDamageSfx>();
        if (foundRelay != null) return foundRelay;

        var foundHealth = Object.FindObjectOfType<PlayerHealth>();
        if (foundHealth != null) return foundHealth;

        // 4) Fallback: bất kỳ MonoBehaviour nào implement IDamageable (kể cả inactive)
        foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>(true))
            if (mb is IDamageable id) return id;

        return null;
    }
}
