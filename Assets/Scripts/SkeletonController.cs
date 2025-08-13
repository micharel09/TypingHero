using UnityEngine;
using System.Collections;
using TMPro;

public class SkeletonController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform target; // Player
    public int health = 50;

    [Header("Attack Settings")]
    public float attackDamage = 5f;
    public float attackCooldown = 2f;

    private float lastAttackTime;
    private bool isDead = false;

    [Header("Damage Settings")]
    public int damageTakenPerHit = 10; // Số máu trừ khi bị player đánh

    void OnEnable()
    {
        TypingManager.OnWordTimeout += TryAttackPlayer;
    }

    void OnDisable()
    {
        TypingManager.OnWordTimeout -= TryAttackPlayer;
    }

    void Update()
    {
        if (isDead) return; // Không làm gì khi đã chết
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Đã chết thì bỏ qua

        health -= damage;
        Debug.Log($"Skeleton bị trúng đòn! Máu còn: {health}");

        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger(AnimationStrings.Hit);
        }
    }

    void TryAttackPlayer()
    {
        if (isDead) return;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger(AnimationStrings.Attack);
            Debug.Log("Skeleton tấn công Player!");
            lastAttackTime = Time.time;
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger(AnimationStrings.Die);
        Debug.Log("Skeleton chết!");

        Destroy(gameObject, 2f);
    }

    // Va chạm vũ khí của player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra tag của vũ khí
        if (collision.CompareTag("PlayerWeapon"))
        {
            TakeDamage(damageTakenPerHit);
        }
    }
}
