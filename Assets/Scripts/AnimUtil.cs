using System.Collections.Generic;
using UnityEngine;

public static class AnimUtil
{
    static readonly Dictionary<string, int> _cache = new();

    public static int Hash(string path)
    {
        if (string.IsNullOrEmpty(path)) return 0;
        if (_cache.TryGetValue(path, out var h)) return h;
        h = Animator.StringToHash(path);
        _cache[path] = h;
        return h;
    }

    public static void CrossFadePath(Animator anim, string path, float fade, float startNormalizedTime = 0f, int layer = 0)
    {
        if (!anim || string.IsNullOrEmpty(path)) return;
        anim.CrossFade(Hash(path), fade, layer, startNormalizedTime);
    }



    public static bool IsInState(Animator anim, string path, out AnimatorStateInfo info, int layer = 0)
    {
        info = default;
        if (!anim || string.IsNullOrEmpty(path)) return false;
        info = anim.GetCurrentAnimatorStateInfo(layer);
        return info.fullPathHash == Hash(path);
    }
}
