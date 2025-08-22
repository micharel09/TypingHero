using System.Collections.Generic;
using UnityEngine;

/// Helpers cho Animator: hash cache + tiện ích crossfade + string keys chuẩn.
public static class AnimatorUtil
{
    // -------- Hash cache ----------
    static readonly Dictionary<string, int> _cache = new();

    /// Lấy hash từ full path / param name, có cache.
    public static int Hash(string path)
    {
        if (string.IsNullOrEmpty(path)) return 0;
        if (_cache.TryGetValue(path, out var h)) return h;
        h = Animator.StringToHash(path);
        _cache[path] = h;
        return h;
    }

    // -------- Crossfade theo full path ----------
    /// CrossFade bằng full-path state (ví dụ: "Base Layer.player_hit").
    public static void CrossFadePath(
        Animator anim, string stateFullPath, float fade, float startNormalizedTime = 0f, int layer = 0)
    {
        if (!anim || string.IsNullOrEmpty(stateFullPath)) return;
        anim.CrossFade(Hash(stateFullPath), fade, layer, startNormalizedTime);
    }

    // -------- Trạng thái hiện tại ----------
    /// Kiểm tra có đang ở đúng full-path state không (layer mặc định = 0).
    public static bool IsInState(Animator anim, string stateFullPath, out AnimatorStateInfo info, int layer = 0)
    {
        info = default;
        if (!anim || string.IsNullOrEmpty(stateFullPath)) return false;
        info = anim.GetCurrentAnimatorStateInfo(layer);
        return info.fullPathHash == Hash(stateFullPath);
    }

    // -------- String keys chuẩn dùng chung ----------
    public static class Strings
    {
        public const string Attack = "Attack";
        public const string Hit = "Hit";
        public const string Die = "Die";
        // Mở rộng thêm khi cần (ví dụ: "Parry", "Stun"…) để tránh string literal rải rác.
    }
}
