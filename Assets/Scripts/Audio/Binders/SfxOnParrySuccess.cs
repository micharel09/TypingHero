using UnityEngine;

[DisallowMultipleComponent]
public sealed class SfxOnParrySuccess : MonoBehaviour
{
    [SerializeField] ParrySystem parry;
    [SerializeField] SfxEvent sfxParry;                  // ev_parry_clang
    [SerializeField] Transform playAt;                   // ParrySparkAnchor
    [SerializeField] bool useEnemyRoot = false;

    [Header("Ducking")]
    [SerializeField] Transform ownerRoot;                // KÉO Player vào
    [SerializeField] string channelToDuck = "player_hand";
    [SerializeField, Min(0f)] float duckSeconds = 0.08f;

    [Header("Clang channel riêng")]
    [SerializeField] string clangChannel = "player_parry";

    [Header("Debug")][SerializeField] bool logs = false;

    void Reset()
    {
        if (!parry) parry = GetComponentInParent<ParrySystem>();
        if (!playAt) playAt = transform;
        if (!ownerRoot) ownerRoot = transform.root;
    }

    void OnEnable() { if (parry) parry.OnParrySuccess += Handle; }
    void OnDisable() { if (parry) parry.OnParrySuccess -= Handle; }

    void Handle(ParrySystem.ParryContext ctx)
    {
        if (!SfxPlayer.I || !sfxParry) return;

        var owner = ownerRoot ? ownerRoot : transform.root;
        if (!string.IsNullOrEmpty(channelToDuck))
            SfxPlayer.I.FadeOutChannel(owner, channelToDuck, duckSeconds);

        Transform t = (useEnemyRoot && ctx.targetRoot) ? ctx.targetRoot : (playAt ? playAt : transform);

        // PHÁT BẤT CHẤP LIMITS
        SfxPlayer.I.PlayOnChannelImportant(sfxParry, ctx.contact.valid ? ctx.contact.point : t.position, ctx.contact.valid ? null : t, clangChannel, owner, 0f);

        if (logs) Debug.Log("[PARRY] clang forced play.");
    }
}
