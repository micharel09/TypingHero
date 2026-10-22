using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;
    public AttackConfig attack;

    public PlayerInputGate gate;  

    ParrySystem parry;
    bool followUpQueued, pendingFromSuccess, releasedFollowUp;

    internal bool ConsumeParryFollowUp()
    {
        bool released = releasedFollowUp;
        releasedFollowUp = false;
        return released;
    }
    void OnEnable()
    {
        parry = GetComponent<ParrySystem>();
        if (parry) parry.OnParrySuccess += OnParrySuccess;
        TypingManager.OnWordCorrect += DoAttack;
    }
    bool pendingAttack;
    void OnDisable()
    {
        TypingManager.OnWordCorrect -= DoAttack;
        if (parry) parry.OnParrySuccess -= OnParrySuccess;
        pendingAttack = pendingFromSuccess = followUpQueued = releasedFollowUp = false;
    }

    void OnParrySuccess(ParrySystem.ParryContext _)
    {
        pendingAttack = pendingFromSuccess = followUpQueued = releasedFollowUp = false;
    }

    void Update()
    {
        if (CombatSession.Finished || (gate && (gate.IsDeadLocked || gate.IsHitLocked)) || TypingManager.MuteWordCorrect)
        { pendingAttack = pendingFromSuccess = releasedFollowUp = false; return; }
        if (GamePauseController.InputBlocked) return;
        if (pendingFromSuccess && (!parry || (!parry.InSuccessPose && !parry.SuccessPoseCompleted)))
            pendingAttack = pendingFromSuccess = false;
        if (pendingAttack && (!gate || gate.CanAttack))
        {
            bool followUp = pendingFromSuccess;
            pendingAttack = pendingFromSuccess = false;
            DoAttack();
            releasedFollowUp = followUp;
        }
    }

    void DoAttack()
    {
        if (!animator || attack == null) return;
        if (CombatSession.Finished || GamePauseController.InputBlocked) return;
        if (gate && !gate.CanAttack)
        {
            if (gate.IsParryPoseLocked && !gate.IsDeadLocked && !gate.IsHitLocked)
            {
                if (parry && parry.InSuccessPose)
                {
                    if (parry.CanQueueFollowUp && !followUpQueued)
                        pendingAttack = pendingFromSuccess = followUpQueued = true;
                }
                else pendingAttack = true;
            }
            return;
        }

        releasedFollowUp = false;
        animator.CrossFade(attack.statePath, attack.crossfade, 0, attack.startTime);
    }
}
