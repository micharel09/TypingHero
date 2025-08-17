using System.Collections.Generic;
using UnityEngine;

/// Trung gian gom tất cả “cờ khóa” input ở Player
public class PlayerInputGate : MonoBehaviour
{
    [Header("Debug")] public bool logs;

    // ---- nguồn khóa ----
    bool _hitLocked;                                // do state "player_hit"
    readonly HashSet<object> _parryPoseSources = new(); // do Anim_ParryOpen/Close
    float _parrySuccUntilUnscaled;                  // i-frame sau parry OK

    // ---- API set/clear từ các hệ khác ----
    public void SetHitLocked(bool v)
    {
        _hitLocked = v;
        if (logs) Debug.Log($"[GATE] HitLocked={_hitLocked}");
    }

    public void PushParryPose(object src) { _parryPoseSources.Add(src ?? this); }
    public void PopParryPose(object src) { _parryPoseSources.Remove(src ?? this); }

    public void BeginParrySuccessIFrame(float seconds)
    {
        if (seconds > 0f)
            _parrySuccUntilUnscaled = Time.unscaledTime + seconds;
    }

    // ---- trạng thái tổng hợp ----
    public bool IsHitLocked => _hitLocked;
    public bool IsParryPoseLocked => _parryPoseSources.Count > 0;
    public bool IsParrySuccessIFrame => Time.unscaledTime < _parrySuccUntilUnscaled;

    // Quy tắc cho phép/không cho phép
    public bool CanAttack => !_hitLocked && !IsParryPoseLocked;    // muốn khác thì đổi ở đây
    public bool CanParry => !_hitLocked && !IsParryPoseLocked;    // (parry thành công vẫn bị “pose lock” nên không spam)
}
