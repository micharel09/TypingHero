using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount, Vector2 hitPoint);
    bool IsDead { get; }
}
