using UnityEngine;
using UnityEngine.Events;

public class EnemyHitReactGate : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Hit React")]
    public string hitStatePath = "Base Layer.skeleton_hit";
    [Range(0f, 0.2f)] public float hitCrossfade = 0.02f;
    public int layerIndex = 0;

    [Header("Safety")]
    public float minInterval = 0.05f;

    [Header("Events")]
    public UnityEvent onForcedInterrupt;

    float _nextAllowedAt;

    public void ForceInterrupt()
    {
        // đang STUN → giữ STUN, không ép hit
        if (TryGetComponent(out EnemyStunController stun) && stun.IsStunned)
        {
            stun.ReassertNow();
            onForcedInterrupt?.Invoke();
            return;
        }

        if (!animator || string.IsNullOrEmpty(hitStatePath)) return;
        if (Time.unscaledTime < _nextAllowedAt) return;

        animator.CrossFadeInFixedTime(hitStatePath, hitCrossfade, layerIndex, 0f);
        _nextAllowedAt = Time.unscaledTime + minInterval;
        onForcedInterrupt?.Invoke();
    }
}
