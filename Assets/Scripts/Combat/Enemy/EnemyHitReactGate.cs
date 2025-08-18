using UnityEngine;
using UnityEngine.Events;

/// Cổng ép enemy nhảy vào Hit React — dùng chung cho mọi controller.
/// ĐẶT TRÊN ROOT CỦA ENEMY để truy cập nhanh.
public class EnemyHitReactGate : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Hit React (data)")]
    [Tooltip("Full path của state bị trúng. VD: Base Layer.skeleton_hit")]
    public string hitStatePath = "Base Layer.skeleton_hit";
    [Range(0f, 0.2f)] public float hitCrossfade = 0.02f;
    public int layerIndex = 0;

    [Header("Safety")]
    [Tooltip("Khoảng cách tối thiểu giữa 2 lần ép crossfade.")]
    public float minInterval = 0.05f;

    [Header("Events")]
    public UnityEvent onForcedInterrupt;

    float _nextAllowedAt;
    int _hitStateHash; // cache

    void Awake() => Rehash();
    void OnValidate() { if (Application.isEditor) Rehash(); }

    void Rehash()
    {
        // Animator hỗ trợ hash theo tên path. Hash 1 lần để tránh so chuỗi nhiều lần.
        _hitStateHash = Animator.StringToHash(hitStatePath);
    }

    /// Ép vào Hit React ngay lập tức (xuyên Uninterruptible nếu có bypass).
    public void ForceInterrupt()
    {
        if (!animator || _hitStateHash == 0) return;
        if (Time.unscaledTime < _nextAllowedAt) return;

        animator.CrossFadeInFixedTime(_hitStateHash, hitCrossfade, layerIndex, 0f);
        _nextAllowedAt = Time.unscaledTime + minInterval;
        onForcedInterrupt?.Invoke();
    }
}

