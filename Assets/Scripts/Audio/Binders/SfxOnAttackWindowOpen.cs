using UnityEngine;

[DisallowMultipleComponent]
public sealed class SfxOnAttackWindowOpen : MonoBehaviour
{
    [SerializeField] PlayerAttackEvents attackEvents;
    [SerializeField] Transform playAt;                 // neo phát (mặc định: this)
    [SerializeField] Transform ownerRoot;              // chủ sở hữu channel (thường là Player root)
    [SerializeField] SfxEvent sfxWhoosh;

    [Header("Channel")]
    [SerializeField] string channelKey = "player_hand";
    [SerializeField, Min(0f)] float crossfadeOutSeconds = 0.05f;

    void Reset()
    {
        if (!attackEvents) attackEvents = GetComponentInParent<PlayerAttackEvents>();
        if (!playAt) playAt = transform;
        if (!ownerRoot) ownerRoot = transform.root;
    }

    void OnEnable() { if (attackEvents) attackEvents.OnWindowOpen += HandleOpen; }
    void OnDisable() { if (attackEvents) attackEvents.OnWindowOpen -= HandleOpen; }

    void HandleOpen()
    {
        if (!SfxPlayer.I || !sfxWhoosh) return;
        var t = playAt ? playAt : transform;
        var owner = ownerRoot ? ownerRoot : transform.root;
        SfxPlayer.I.PlayOnChannel(sfxWhoosh, t.position, t, channelKey, owner, crossfadeOutSeconds);
    }
}
