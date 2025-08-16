using System;
using UnityEngine;

public class SkeletonController : MonoBehaviour, IDamageable, IParryTarget
{
    [Header("References")]
    public Animator animator;
    public Transform target;              // dùng cho quay mặt/di chuyển nếu sau này cần
    public PlayerHealth playerHealth;     // nơi nhận damage khi skeleton chém trúng (có thể thay bằng hệ khác)

    [Header("Stats")]
    public int health = 500;
    public bool IsDead { get; private set; }

    [Header("Attack Clock")]
    [Tooltip("Khoảng thời gian giữa 2 lần ra đòn (giây).")]
    public float attackInterval = 1.5f;
    [Tooltip("Dùng đồng hồ không bị ảnh hưởng bởi timeScale (đề phòng hitstop).")]
    public bool useUnscaledClock = true;

    [Header("Attack Animation")]
    [Tooltip("FullPath của clip tấn công trong Animator (vd: Base Layer.skeleton_attack1)")]
    public string attackStatePath = "Base Layer.skeleton_attack1";
    [Range(0f, .2f)] public float attackCrossfade = 0.02f;
    [Tooltip("Bắt đầu phát clip ở normalized time bao nhiêu (0 = từ đầu).")]
    public float attackStartTime = 0f;

    [Header("Damage")]
    public int attackDamage = 5;

    [Header("Hit React")]
    [Tooltip("Đang vung thì không bị ngắt bởi Hit React.")]
    public bool uninterruptibleDuringAttack = true;
    [Tooltip("FullPath clip bị trúng (vd: Base Layer.skeleton_hit)")]
    public string hitStatePath = "Base Layer.skeleton_hit";
    [Range(0f, .2f)] public float hitCrossfade = 0.02f;
    [Tooltip("Nếu không ngắt được khi đang vung thì sẽ xếp hàng để phát Hit React sau khi vung xong.")]
    public bool queueHitReactAfterAttack = true;

    [Header("Debug")]
    public bool logs = false;

    // ================== internal state ==================
    float _nextAttackAt;
    bool _attacking;
    bool _queuedHitReact;
    float _stunnedUntil;

    public event Action OnStrike;   // IParryTarget

    float Now => useUnscaledClock ? Time.unscaledTime : Time.time;
    bool Stunned => Now < _stunnedUntil;

    void OnEnable()
    {
        // bắt đầu đếm cho lần vung đầu tiên
        _nextAttackAt = Now + 0.5f;
    }

    void Update()
    {
        if (IsDead) return;

        // nếu đang stun thì thôi
        if (Stunned) return;

        // tự ra đòn theo đồng hồ
        if (!_attacking && Now >= _nextAttackAt)
            StartAttack();

        // xác định đã ra khỏi state attack hay chưa để biết khi nào kết thúc lượt
        if (_attacking)
        {
            if (!AnimUtil.IsInState(animator, attackStatePath, out var info) || info.normalizedTime >= 0.98f)
                EndAttack();
        }
    }

    // --------- Attack lifecycle ----------
    void StartAttack()
    {
        _attacking = true;
        _queuedHitReact = false;

        if (animator)
            AnimUtil.CrossFadePath(animator, attackStatePath, attackCrossfade, attackStartTime);

        if (logs) Debug.Log("[SKE] Start attack");
    }   

    void EndAttack()
    {
        _attacking = false;
        _nextAttackAt = Now + attackInterval;

        if (_queuedHitReact && !Stunned && !IsDead)
        {
            // phát hit-react đã xếp hàng
            _queuedHitReact = false;
            if (animator)
                AnimUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
            if (logs) Debug.Log("[SKE] Play queued hit-react");
        }
        if (logs) Debug.Log("[SKE] End attack");
    }

    // --------- Animation Events ----------
    // đặt trong clip tấn công đúng frame vung kiếm
    public void Anim_StrikeFrame()
    {
        if (logs) Debug.Log("[SKE] Strike frame!");
        OnStrike?.Invoke();
    }

    // đặt tại frame gây damage (có thể trùng strike-frame nếu bạn muốn)
    // Gọi từ Animation Event trên clip skeleton_attack1 đúng frame vung kiếm
    public void Anim_DealDamage()
    {
        if (!playerHealth || playerHealth.IsDead) return;
        playerHealth.TakeDamage(attackDamage, (Vector2)transform.position);
    }


    // --------- IParryTarget ----------
    public void Parried(float stunSeconds)
    {
        if (IsDead) return;

        _stunnedUntil = Now + Mathf.Max(0f, stunSeconds);

        // ngắt attack hiện tại nếu cho phép
        _attacking = false;
        _nextAttackAt = Now + attackInterval;

        if (animator)
            AnimUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);

        if (logs) Debug.Log($"[SKE] Parried! Stun {stunSeconds:0.00}s");
    }

    // --------- IDamageable ----------
    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead) return;

        health -= amount;
        if (logs) Debug.Log($"[SKE] Hit {amount}, HP: {health}");

        if (health <= 0)
        {
            Die();
            return;
        }

        // → KHÓA CHẶT: đang ở state attack thì không cho ngắt
        bool inAttackNow = AnimUtil.IsInState(animator, attackStatePath, out _);
        if (uninterruptibleDuringAttack && inAttackNow)
        {
            if (queueHitReactAfterAttack) _queuedHitReact = true;   // để xả hit-react sau khi vung xong
            if (logs) Debug.Log("[SKE] Got hit mid-swing → queued hit-react");
            return; // QUAY RA, KHÔNG crossfade sang hit
        }

        // không ở state attack thì được phép play hit-react ngay
        if (animator) AnimUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
    }
    void Die()
    {
        IsDead = true;
        // Nếu bạn có clip die riêng, bạn có thể crossfade tại đây.
        // Ở bản tối giản: chỉ disable và xoá sau 2 giây để không làm rối project.
        if (logs) Debug.Log("[SKE] Dead");
        Destroy(gameObject, 2f);
    }
}
