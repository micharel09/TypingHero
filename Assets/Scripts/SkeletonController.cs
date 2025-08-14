using UnityEngine;

public class SkeletonController : MonoBehaviour, IDamageable
{
    [Header("References")] public Animator animator; public Transform target;
    [Header("Stats")] public int health = 500; public bool IsDead { get; private set; }
    [Header("Attack Settings")] public float attackDamage = 5f; public float attackCooldown = 4f;
    float lastAttackTime;
    public PlayerHealth playerHealth;   

    void OnEnable() => TypingManager.OnWordTimeout += TryAttackPlayer;
    void OnDisable() => TypingManager.OnWordTimeout -= TryAttackPlayer;

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead) return;
        health -= amount;
        Debug.Log($"Skeleton hit {amount}, HP: {health}");
        if (health <= 0) Die();
        else animator.SetTrigger(AnimationStrings.Hit);
    }

    void TryAttackPlayer()
    {
        if (IsDead) return;
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;
        animator.SetTrigger(AnimationStrings.Attack);
        // Gọi damage đúng frame bằng Animation Event:
        // thêm event "EnemyDealDamage" trong clip attack của skeleton
    }


    // Animation Event trên clip skeleton_attack1
    public void EnemyDealDamage()
    {
        if (playerHealth != null && !playerHealth.IsDead)
            playerHealth.TakeDamage(Mathf.RoundToInt(attackDamage), (Vector2)playerHealth.transform.position);
        if (HitStopper.I) HitStopper.I.StopPlayerHit();
    }

    void Die()
    {
        IsDead = true;
        animator.SetTrigger(AnimationStrings.Die);
        Destroy(gameObject, 2f);
    }
}
