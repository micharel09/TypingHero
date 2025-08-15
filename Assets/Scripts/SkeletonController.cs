using UnityEngine;
using System.Collections;

public class SkeletonController : MonoBehaviour, IDamageable
{
    [Header("References")]
    public Animator animator;
    public Transform target;
    public PlayerHealth playerHealth;
    [Tooltip("Collider thân của skeleton (BoxCollider2D/Hurtbox)")]
    public Collider2D hurtbox;                         // <-- drag BoxCollider2D vào đây trong Inspector

    [Header("Stats")]
    public int health = 500;
    public bool IsDead { get; private set; }

    [Header("Attack Settings")]
    public float attackDamage = 5f;
    public float attackCooldown = 4f;
    float lastAttackTime;

    [Header("Anim States")]
    public string hitStatePath = "Base Layer.skeleton_hit";
    [Range(0f, 0.1f)] public float hitCrossfade = 0.02f;

    // ===== Clash/Parry flags & event =====
    public event System.Action OnStrikeFrame;          // <-- bắn đúng frame chém
    bool cancelThisStrike = false;
    bool stunned = false;
    // =====================================

    void OnEnable() => TypingManager.OnWordTimeout += TryAttackPlayer;
    void OnDisable() => TypingManager.OnWordTimeout -= TryAttackPlayer;

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead) return;
        health -= amount;
        animator.CrossFadeSafe(hitStatePath, hitCrossfade, 0f);
        if (health <= 0) Die();
    }

    void TryAttackPlayer()
    {
        if (IsDead || stunned) return;
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;
        animator.SetTrigger(AnimationStrings.Attack);
    }

    // === Animation Event: đặt đúng frame vung trúng (strike) ===
    public void Anim_StrikeFrame()
    {
        OnStrikeFrame?.Invoke();                    // cho hệ clash kiểm tra trước

        if (cancelThisStrike)
        {                     // nếu đã bị clash, huỷ gây damage
            cancelThisStrike = false;
            return;
        }

        if (playerHealth != null && !playerHealth.IsDead)
            playerHealth.TakeDamage(Mathf.RoundToInt(attackDamage), (Vector2)playerHealth.transform.position);

        if (HitStopper.I) HitStopper.I.StopPlayerHit();
    }

    // được gọi bởi hệ clash khi va chạm vũ khí thành công
    public void CancelCurrentStrikeAndStun(float stunDuration)
    {
        cancelThisStrike = true;
        if (gameObject.activeInHierarchy) StartCoroutine(Stun(stunDuration));
        animator.CrossFadeSafe(hitStatePath, hitCrossfade, 0f);
    }

    IEnumerator Stun(float d)
    {
        stunned = true;
        yield return new WaitForSeconds(d);
        stunned = false;
    }

    void Die()
    {
        IsDead = true;
        animator.SetTrigger(AnimationStrings.Die);
        Destroy(gameObject, 2f);
    }
}
