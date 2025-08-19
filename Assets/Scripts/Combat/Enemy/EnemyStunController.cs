using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class EnemyStunController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator animator;
    [SerializeField] string stunStatePath = "Base Layer.skeleton_stun";
    [SerializeField, Range(0f, 0.2f)] float stunCrossfade = 0.02f;
    [SerializeField] string exitStatePath = "Base Layer.skeleton_idle";
    [SerializeField, Range(0f, 0.2f)] float exitCrossfade = 0.02f;

    [Header("Auto disable trong lúc stun")]
    [SerializeField] bool autoDisableAttackStrike = true;
    [SerializeField] bool autoDisableSkeletonController = true;

    AttackStrike _attack;
    SkeletonController _brain;

    [Header("Events")]
    public UnityEvent onStunStart;   // <-- NEW
    public UnityEvent onStunEnd;     // <-- NEW

    [Header("Debug")]
    [SerializeField] bool logs;

    bool _isStunned;
    public bool IsStunned => _isStunned;

    void Awake()
    {
        if (autoDisableAttackStrike) _attack = GetComponent<AttackStrike>();
        if (autoDisableSkeletonController) _brain = GetComponent<SkeletonController>();
    }

    public void TriggerStun(float seconds)
    {
        if (seconds <= 0f) return;
        StopAllCoroutines();
        StartCoroutine(StunRoutine(seconds));
    }

    IEnumerator StunRoutine(float seconds)
    {
        _isStunned = true;
        SetDisabled(true);

        Crossfade(stunStatePath, stunCrossfade);
        onStunStart?.Invoke(); // <-- NEW
        if (logs) Debug.Log($"[Stun] {name} START {seconds:0.00}s");

        yield return new WaitForSecondsRealtime(seconds);

        _isStunned = false;
        SetDisabled(false);
        Crossfade(exitStatePath, exitCrossfade);
        onStunEnd?.Invoke();   // <-- NEW
        if (logs) Debug.Log($"[Stun] {name} END");
    }

    void SetDisabled(bool off)
    {
        if (_attack) _attack.enabled = !off;
        if (_brain) _brain.enabled = !off;
    }

    void Crossfade(string path, float cf)
    {
        if (!animator || string.IsNullOrEmpty(path)) return;
        animator.CrossFadeInFixedTime(path, cf, 0, 0f);
    }

    public void ReassertNow()
    {
        if (!_isStunned) return;
        Crossfade(stunStatePath, stunCrossfade);
        if (logs) Debug.Log("[Stun] Reassert");
    }
}
