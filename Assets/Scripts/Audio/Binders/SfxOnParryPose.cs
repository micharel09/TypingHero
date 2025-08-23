using UnityEngine;

[DisallowMultipleComponent]
public sealed class SfxOnParryPose : MonoBehaviour
{
    [SerializeField] SfxEvent sfxParryPose;   // ev_parry_pose.asset
    [SerializeField] Transform playAt;        // neo phát (ví d?: ParrySparkAnchor)
    [SerializeField, Min(0f)] float cooldown = 0.03f;

    float _nextAllowedAt;

    void Reset()
    {
        if (!playAt) playAt = transform;
    }

    // G?I T? ANIMATION EVENT trong clip parry pose
    public void Anim_ParryPoseSfx()
    {
        if (Time.unscaledTime < _nextAllowedAt) return;
        if (!SfxPlayer.I || !sfxParryPose) return;

        var t = playAt ? playAt : transform;
        SfxPlayer.I.Play(sfxParryPose, t.position, t);
        _nextAllowedAt = Time.unscaledTime + cooldown;
    }
}
