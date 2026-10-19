using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;
    public AttackConfig attack;

    public PlayerInputGate gate;  

    void OnEnable() => TypingManager.OnWordCorrect += DoAttack;
    bool pendingAttack;
    void OnDisable() { TypingManager.OnWordCorrect -= DoAttack; pendingAttack = false; }

    void Update()
    {
        if (CombatSession.Finished || (gate && gate.IsDeadLocked) || TypingManager.MuteWordCorrect)
        { pendingAttack = false; return; }
        if (GamePauseController.InputBlocked) return;
        if (pendingAttack && (!gate || gate.CanAttack)) { pendingAttack = false; DoAttack(); }
    }

    void DoAttack()
    {
        if (!animator || attack == null) return;
        if (CombatSession.Finished || GamePauseController.InputBlocked) return;
        if (gate && !gate.CanAttack) { if (gate.IsParryPoseLocked && !gate.IsDeadLocked) pendingAttack = true; return; }   // <-- khóa khi đang bị hit/parry

        animator.CrossFade(attack.statePath, attack.crossfade, 0, attack.startTime);
    }
}
