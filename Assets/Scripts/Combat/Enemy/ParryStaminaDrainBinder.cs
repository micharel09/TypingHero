using UnityEngine;

/// Nghe Parry SUCCESS → trừ stamina, nếu về 0 thì stun N giây.
public sealed class ParryStaminaDrainBinder : MonoBehaviour
{
    [SerializeField] ParrySystem parry; // kéo ParrySystem (Player)
    [SerializeField] float stunDurationSeconds = 5f; // === MỚI ===
    [SerializeField] bool logs;

    void OnEnable() { if (parry) parry.OnParrySuccess += OnParrySuccess; }
    void OnDisable() { if (parry) parry.OnParrySuccess -= OnParrySuccess; }

    void OnParrySuccess(ParrySystem.ParryContext ctx)
    {
        if (!ctx.targetRoot) return;

        if (ctx.targetRoot.TryGetComponent(out EnemyStamina stam))
        {
            stam.ConsumeParry();     // trừ stamina
            stam.NotifyParried();    // reset timer regen (4s)
            if (stam.Current == 0 && stunDurationSeconds > 0f &&
                ctx.targetRoot.TryGetComponent(out EnemyStunController stun))
                stun.TriggerStun(stunDurationSeconds);
        }

    }

}
