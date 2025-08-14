using UnityEngine;

public class PlayerAttackEvents : MonoBehaviour
{
    public PlayerWeaponHitbox hitbox;
    bool windowOpen = false;

    // gọi từ Animation Event
    public void OnAttackStart()
    {
        if (windowOpen) return;
        windowOpen = true;
        hitbox.BeginAttack();
    }

    // gọi từ Animation Event
    public void OnAttackEnd()
    {
        if (!windowOpen) return;
        windowOpen = false;
        hitbox.EndAttack();
    }

    // DÙNG KHI RESTART ĐÒN GIỮA CHỪNG
    public void ForceCloseWindow()
    {
        if (!windowOpen) return;
        windowOpen = false;
        hitbox.EndAttack();
    }
}
