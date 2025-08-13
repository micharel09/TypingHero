using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public SkeletonController boss; // Tham chiếu đến Boss
    public int damage = 10; // Sát thương mỗi đòn
    

    void OnEnable()
    {
        TypingManager.OnWordCorrect += DoAttack;
    }

    void OnDisable()
    {
        TypingManager.OnWordCorrect -= DoAttack;
    }

    void DoAttack()
    {
        if (animator) animator.SetTrigger(AnimationStrings.Attack);
    }
}
