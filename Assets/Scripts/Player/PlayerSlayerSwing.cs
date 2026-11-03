using UnityEngine;

// Shared execution path for full Slayer and the run-meter Semi Slayer.
internal static class PlayerSlayerSwing
{
    internal static bool Execute(
        Animator animator,
        PlayerAttackEvents attackEvents,
        SlashVolume trail,
        SlayerAfterimagePool afterimage,
        SpriteRenderer source,
        string statePath,
        string speedParam,
        float speedMultiplier,
        int directionVariant,
        int afterimageCombo,
        bool spawnAfterimage,
        bool semiSlayerTrail,
        PlayerAttackContext context,
        out Vector2 direction)
    {
        direction = Vector2.right;
        if (!animator || !attackEvents || string.IsNullOrEmpty(statePath)) return false;

        if (spawnAfterimage && afterimage)
            afterimage.SpawnBurstFromCurrentFrame(afterimageCombo);

        if (!string.IsNullOrEmpty(speedParam)) animator.SetFloat(speedParam, speedMultiplier);
        animator.Play(statePath, 0, 0f);
        animator.Update(0f);

        direction = PlayerSlayerMode.LocalStrikeDirection(directionVariant);
        if (source)
        {
            if (source.flipX) direction.x = -direction.x;
            if (source.flipY) direction.y = -direction.y;
            direction = source.transform.TransformVector(direction);
        }
        direction = direction.normalized;

        if (trail)
        {
            if (semiSlayerTrail) trail.BeginSemiSlayerSwing();
            else trail.BeginSlayerSwing();
        }
        attackEvents.OnAttackStart(context);
        attackEvents.OnAttackEnd();
        return true;
    }
}
