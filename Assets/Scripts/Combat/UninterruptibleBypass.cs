using System.Collections.Generic;
using UnityEngine;

/// Bật "xuyên Uninterruptible" cho TỪNG enemy trong một khoảng thời gian ngắn.
/// Dùng unscaledTime để không bị lệch bởi hitstop.
public static class UninterruptibleBypass
{
    static readonly Dictionary<int, float> _untilById = new Dictionary<int, float>();
    static float Now => Time.unscaledTime;

    public static void ActivateFor(Component target, float seconds)
    {
        if (!target) return;
        int id = target.GetInstanceID();
        float until = Now + Mathf.Max(0f, seconds);
        if (_untilById.TryGetValue(id, out float cur)) _untilById[id] = Mathf.Max(cur, until);
        else _untilById[id] = until;
    }

    public static bool IsActiveFor(Component target)
    {
        if (!target) return false;
        int id = target.GetInstanceID();
        if (_untilById.TryGetValue(id, out float until))
        {
            if (Now < until) return true;
            _untilById.Remove(id); // hết hạn thì dọn
        }
        return false;
    }

    public static void ClearFor(Component target)
    {
        if (!target) return;
        _untilById.Remove(target.GetInstanceID());
    }
}
