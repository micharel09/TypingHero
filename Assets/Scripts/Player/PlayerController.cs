using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;
    public AttackConfig attack;

    void OnEnable() => TypingManager.OnWordCorrect += DoAttack;
    void OnDisable() => TypingManager.OnWordCorrect -= DoAttack;

    void DoAttack()
    {
        if (!animator || attack == null) return;
        animator.CrossFade(attack.statePath, attack.crossfade, 0, attack.startTime);
    }
}
