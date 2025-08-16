using UnityEngine;

/// Đối tượng nhận sát thương có thể bị tấn công (Player, Enemy…)
public interface IDamageable
{
    /// Nhận sát thương lượng `amount`, tại điểm va chạm `hitPoint` (để VFX/knockback nếu cần)
    void TakeDamage(int amount, Vector2 hitPoint);

    /// Đã chết chưa (để các hệ thống khác bỏ qua)
    bool IsDead { get; }
}
