using UnityEngine;

public static class AnimUtil
{
    public static void CrossFadeSafe(this Animator anim, string statePath, float fade, float startTime, int layer = 0)
    {
        int hash = Animator.StringToHash(statePath);
        if (!anim.HasState(layer, hash))
        {
            Debug.LogWarning($"[Anim] State not found: {statePath} on {anim.runtimeAnimatorController.name}");
            return;
        }
        anim.CrossFadeInFixedTime(hash, fade, layer, startTime);
    }
}