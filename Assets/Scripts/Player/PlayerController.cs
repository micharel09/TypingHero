using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerAttackEvents attackEvents;
    public AttackConfig attack;

    PlayerHealth _hp;                              

    void Awake()                                   
    {
        _hp = GetComponent<PlayerHealth>();
    }

    bool IsHitLocked()                             
    {
        return _hp && _hp.animator &&
               AnimUtil.IsInState(_hp.animator, _hp.hitStatePath, out _);
    }

    void OnEnable() => TypingManager.OnWordCorrect += DoAttack;
    void OnDisable() => TypingManager.OnWordCorrect -= DoAttack;

    void DoAttack()
    {
        if (!animator || attack == null) return;

        // ⛔ đang play player_hit thì không cho tấn công
        if (IsHitLocked()) return;                

        animator.CrossFade(attack.statePath, attack.crossfade, 0, attack.startTime);
    }
}
