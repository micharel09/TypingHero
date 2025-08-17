using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ParryTarget : MonoBehaviour, IParryTarget
{
    public event Action OnStrike;

    [Header("Refs (tuỳ chọn)")]
    [Tooltip("Component có IDamageable để đọc IsDead (nếu để trống sẽ tự tìm trên chính GameObject).")]
    public MonoBehaviour damageable;

    [Tooltip("Controller có hàm Parried(float). Với skeleton: kéo SkeletonController vào đây.")]
    public SkeletonController skeleton;

    IDamageable _hp;

    void Awake()
    {
        if (damageable != null) _hp = damageable as IDamageable;
        if (_hp == null) _hp = GetComponent<IDamageable>();
        if (skeleton == null) skeleton = GetComponent<SkeletonController>();
        if (_hp == null)
            Debug.LogWarning("[ParryTarget] Không tìm thấy IDamageable để đọc IsDead (không bắt buộc).");
    }

    public bool IsDead => _hp != null && _hp.IsDead;

    // Animation Event: đặt đúng strike-frame trong clip tấn công của enemy
    public void Anim_StrikeFrame() => OnStrike?.Invoke();
}
