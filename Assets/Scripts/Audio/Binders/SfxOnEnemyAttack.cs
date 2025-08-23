using UnityEngine;

[DisallowMultipleComponent]
public sealed class SfxOnEnemyAttack : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] SfxEvent sfxEnemyAttack;      // ev_enemy_attack_whoosh_light
    [SerializeField] Transform playAt;             // SFX_EnemyWhooshAnchor (tip vũ khí)
    [SerializeField, Min(0f)] float cooldown = 0.02f;

    [Header("Channel")]
    [SerializeField] string channelKey = "enemy_hand";
    [SerializeField] Transform ownerRoot;          // root của enemy này
    [SerializeField, Min(0f)] float crossfadeOutSeconds = 0.06f;

    float _nextAllowedAt;

    void Reset()
    {
        if (!playAt) playAt = transform;
        if (!ownerRoot) ownerRoot = transform.root;
    }

    // === GỌI TỪ ANIMATION EVENT (clip tấn công của enemy) ===
    public void Anim_EnemyAttackSfx()
    {
        if (!SfxPlayer.I || !sfxEnemyAttack) return;
        if (Time.unscaledTime < _nextAllowedAt) return;

        var t = playAt ? playAt : transform;
        var owner = ownerRoot ? ownerRoot : transform.root;

        // Crossfade các âm tay (hand) của enemy nếu động tác đổi đột ngột
        SfxPlayer.I.PlayOnChannel(sfxEnemyAttack, t.position, t, channelKey, owner, crossfadeOutSeconds);
        _nextAllowedAt = Time.unscaledTime + cooldown;
    }
}
