using UnityEngine;

public enum ImpactVfxCompletionPolicy { StopOnCombatEnd, AllowUntilResults }

public struct ImpactVfxContext
{
    public Vector3 point;
    public float direction, floorY;
    public int seed;
    public ImpactVfxCompletionPolicy completionPolicy;
    public PhysicalParticleProfile.RecipientSnapshot recipient;
    public float unitsPerMeter;
}
