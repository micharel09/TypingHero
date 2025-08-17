using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;
    public AttackConfig attack;

    public PlayerInputGate gate;  

    void OnEnable() => TypingManager.OnWordCorrect += DoAttack;
    void OnDisable() => TypingManager.OnWordCorrect -= DoAttack;

    void DoAttack()
    {
        if (!animator || attack == null) return;
        if (gate && !gate.CanAttack) return;   // <-- khóa khi đang bị hit/parry

        animator.CrossFade(attack.statePath, attack.crossfade, 0, attack.startTime);
    }
}
