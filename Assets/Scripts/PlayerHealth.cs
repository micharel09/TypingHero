using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int current;
    public Animator animator; // (tùy chọn) nếu có clip Hit/Die
    public bool IsDead => current <= 0;

    void Awake() => current = maxHealth;

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead) return;
        current = Mathf.Max(0, current - amount);
        if (animator) animator.SetTrigger(AnimationStrings.Hit);
        if (IsDead && animator) animator.SetTrigger(AnimationStrings.Die);
    }
}
