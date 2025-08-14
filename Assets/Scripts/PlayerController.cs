using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;

    [Header("Attack State Settings")]
    public string attackStateName = "attack1"; // ĐÚNG tên state trong Animator
    public string attackStateTag = "Attack";  // Tag của state attack1 (set trong Animator)
    public float restartCrossfade = 0.02f;     // thời gian crossfade khi restart
    public bool useQueueInsteadOfRestart = false; // bật nếu muốn QUEUE thay vì restart

    // Queue (tùy chọn)
    int queued = 0;

    void OnEnable() => TypingManager.OnWordCorrect += DoAttack;
    void OnDisable() => TypingManager.OnWordCorrect -= DoAttack;

    void DoAttack()
    {
        if (!animator) return;

        var info = animator.GetCurrentAnimatorStateInfo(0);
        bool inAttack = info.IsTag(attackStateTag) || info.IsName(attackStateName);

        if (useQueueInsteadOfRestart)
        {
            if (inAttack) { queued++; return; }          // đợi đòn hiện tại
            animator.ResetTrigger(AnimationStrings.Attack);
            animator.SetTrigger(AnimationStrings.Attack); // vào attack1
            return;
        }

        // === MODE RESTART NGAY ===
        if (inAttack)
        {
            // đóng hitbox hiện tại rồi restart state về 0
            if (attackEvents) attackEvents.ForceCloseWindow();
            animator.CrossFadeInFixedTime(attackStateName, restartCrossfade, 0, 0f);
        }
        else
        {
            animator.ResetTrigger(AnimationStrings.Attack);
            animator.SetTrigger(AnimationStrings.Attack);
        }
    }

    // Nếu bật Queue: gọi từ Animation Event OnAttackEnd cuối clip
    public void OnComboWindowEnd()
    {
        if (!useQueueInsteadOfRestart) return;
        if (queued > 0)
        {
            queued--;
            animator.SetTrigger(AnimationStrings.Attack);
        }
    }
}
