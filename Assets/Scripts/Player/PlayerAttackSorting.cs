using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public sealed class PlayerAttackSorting : MonoBehaviour
{
    static readonly int AttackTag = Animator.StringToHash("Attack");
    static readonly int ParryTag = Animator.StringToHash("Parry");
    Animator animator;
    SpriteRenderer sprite;
    int originalOrder;

    void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        originalOrder = sprite.sortingOrder;
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0) return;
        // The destination owns presentation as soon as a crossfade starts.
        var state = animator.IsInTransition(0)
            ? animator.GetNextAnimatorStateInfo(0)
            : animator.GetCurrentAnimatorStateInfo(0);
        sprite.sortingOrder = state.tagHash == AttackTag || state.tagHash == ParryTag ? 1 : originalOrder;
    }

    void OnDisable()
    {
        if (sprite) sprite.sortingOrder = originalOrder;
    }
}
