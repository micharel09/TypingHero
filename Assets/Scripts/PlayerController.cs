using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;

    [Header("Attack Config")]
    public AttackConfig attack;   // gán SO trong Inspector

    void OnEnable() => TypingManager.OnWordCorrect += DoAttack;
    void OnDisable() => TypingManager.OnWordCorrect -= DoAttack;

    void DoAttack()
    {
        attackEvents?.ForceCloseWindow(); // reset cửa sổ đòn cũ

        // đồng bộ hitbox theo config (1 nơi quản lý)
        if (attackEvents && attack != null && attackEvents.hitbox)
        {
            attackEvents.hitbox.damage = attack.damage;
            attackEvents.hitbox.targetLayers = attack.targetLayers;
        }

        animator.CrossFadeSafe(attack.statePath, attack.crossfade, attack.startTime);
    }
}
