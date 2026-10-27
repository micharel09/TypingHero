using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;
    public AttackConfig attack;
    public AttackConfig parryFollowUp;

    public PlayerInputGate gate;  

    ParrySystem parry;
    bool followUpQueued, pendingFromSuccess, releasedFollowUp;
    static readonly int FollowUpParameter = Animator.StringToHash("ParryFollowUpQueued");

    internal void OnParryFollowUpEntered()
    {
        if (!pendingFromSuccess) return;
        pendingAttack = pendingFromSuccess = false;
        releasedFollowUp = true;
        animator.SetBool(FollowUpParameter, false);
    }

    void ClearFollowUpRequest()
    {
        if (followUpQueued && animator) animator.SetBool(FollowUpParameter, false);
    }

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
    internal void ResetPracticeAttack()
    {
        ClearFollowUpRequest();
        pendingAttack = pendingFromSuccess = followUpQueued = releasedFollowUp = false;
        if (attackEvents) attackEvents.CancelAttack();
        if (animator) { animator.Play("Base Layer.player_idle", 0, 0); animator.Update(0); }
    }
    void OnDisable()
    {
        TypingManager.OnWordCorrect -= DoAttack;
        if (parry) parry.OnParrySuccess -= OnParrySuccess;
        ClearFollowUpRequest();
        pendingAttack = pendingFromSuccess = followUpQueued = releasedFollowUp = false;
        if (attackEvents) attackEvents.CancelAttack();
    }

    void OnParrySuccess(ParrySystem.ParryContext _)
    {
        ClearFollowUpRequest();
        pendingAttack = pendingFromSuccess = followUpQueued = releasedFollowUp = false;
    }

    void Update()
    {
        if (CombatSession.Finished || (gate && (gate.IsDeadLocked || gate.IsHitLocked)) || TypingManager.MuteWordCorrect)
        { ClearFollowUpRequest(); pendingAttack = pendingFromSuccess = followUpQueued = releasedFollowUp = false; return; }
        if (GamePauseController.InputBlocked) return;
        if (pendingFromSuccess && (!parry || (!parry.InSuccessPose && !parry.SuccessPoseCompleted)))
        { ClearFollowUpRequest(); pendingAttack = pendingFromSuccess = false; }
        if (pendingAttack && (!gate || gate.CanAttack))
        {
            bool followUp = pendingFromSuccess;
            pendingAttack = pendingFromSuccess = false;
            PlayAttack(followUp);
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
                    {
                        pendingAttack = pendingFromSuccess = followUpQueued = true;
                        animator.SetBool(FollowUpParameter, true);
                    }
                }
                else pendingAttack = true;
            }
            return;
        }

        PlayAttack(false);
    }

    void PlayAttack(bool followUp)
    {
        var config = followUp ? parryFollowUp : attack;
        if (!animator || !config) return;
        releasedFollowUp = followUp;
        if (followUp) animator.CrossFadeInFixedTime(config.statePath, config.crossfade, 0, config.startTime);
        else animator.CrossFade(config.statePath, config.crossfade, 0, config.startTime);
    }
}
