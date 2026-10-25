using UnityEngine;

[DisallowMultipleComponent]
public sealed class NormalStrikeEffects : MonoBehaviour
{
    [SerializeField] PlayerAttackEvents attackEvents;
    [SerializeField] PlayerSlayerMode slayer;
    [SerializeField] HitSparkSpawner emitter;
    [SerializeField] BoxCollider2D body;
    [SerializeField] GameObject dustPrefab;
    [SerializeField] float groundY;

    internal GameObject IntroDustTemplate => dustPrefab;

    void OnEnable()
    {
        if (attackEvents) attackEvents.OnWindowOpen += Open;
        SlayerModeSignals.OnSetActive += SlayerChanged;
    }
    void OnDisable()
    {
        if (attackEvents) attackEvents.OnWindowOpen -= Open;
        SlayerModeSignals.OnSetActive -= SlayerChanged;
        if (emitter) emitter.ClearAccents();
    }
    void SlayerChanged(bool active)
    {
        if (active && emitter) emitter.ClearAccents();
    }
    void Open()
    {
        if (!emitter || !body || (slayer && slayer.IsActive) || CombatSession.Finished) return;
        float direction = transform.lossyScale.x < 0 ? -1 : 1;
        Vector3 point = body.transform.TransformPoint(body.offset);
        // The standing-body foot landmark is stable across all attack cels.
        point.x -= direction / 16f;
        point.y = groundY;
        emitter.SpawnAccent(dustPrefab, point, direction);
    }
}
