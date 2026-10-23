using UnityEngine;

public struct ImpactVfxContext
{
    public Vector3 point;
    public float direction, floorY;
    public int seed;
    public PhysicalParticleProfile.RecipientSnapshot recipient;
    public float unitsPerMeter;
}
