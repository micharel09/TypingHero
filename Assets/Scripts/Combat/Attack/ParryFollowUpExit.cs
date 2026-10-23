using UnityEngine;

public sealed class ParryFollowUpExit : StateMachineBehaviour
{
    public Sprite openingCel;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var events = animator.GetComponent<PlayerAttackEvents>();
        if (events) events.CancelAttack(openingCel);
    }
}
