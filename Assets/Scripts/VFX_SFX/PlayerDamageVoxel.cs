using UnityEngine;

public sealed class PlayerDamageVoxel : MonoBehaviour
{
    [SerializeField] PlayerHealth health;
    [SerializeField] HitSparkSpawner emitter;
    [SerializeField] BoxCollider2D body;
    void OnEnable() { if (health) health.DamageAccepted += Spawn; }
    void OnDisable() { if (health) health.DamageAccepted -= Spawn; }
    void Spawn(Vector2 attacker, bool lethal)
    {
        // AttackStrike supplies the attacker's root, not a weapon contact position.
        Vector2 probe = body ? new Vector2(attacker.x, body.bounds.center.y + body.bounds.extents.y * .25f) : attacker;
        Vector2 point = body ? body.ClosestPoint(probe) : transform.position;
        emitter.SpawnImpact(point, null, false, lethal, transform, transform.position.x - attacker.x);
    }
}
