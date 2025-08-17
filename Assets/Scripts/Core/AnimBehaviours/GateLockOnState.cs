using UnityEngine;

/// Gắn vào state 'player_hit' của Player Animator:
/// OnEnter -> khóa, OnExit -> mở
public class GateLockOnState : StateMachineBehaviour
{
    public enum Kind { Hit }
    public Kind kind = Kind.Hit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var gate = animator.GetComponentInParent<PlayerInputGate>();
        if (!gate) return;
        if (kind == Kind.Hit) gate.SetHitLocked(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var gate = animator.GetComponentInParent<PlayerInputGate>();
        if (!gate) return;
        if (kind == Kind.Hit) gate.SetHitLocked(false);
    }
}
