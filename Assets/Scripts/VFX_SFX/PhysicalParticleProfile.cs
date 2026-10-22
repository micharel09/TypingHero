using UnityEngine;

[CreateAssetMenu(menuName = "TypingHero/VFX/Physical Particle Profile")]
public sealed class PhysicalParticleProfile : ScriptableObject
{
    [System.Serializable]
    public struct PhysicsSettings
    {
        [Min(0)] public float gravity, launchDuration, launchDrag;
        [Tooltip("Horizontal/depth drag after launch; vertical motion follows gravity.")]
        [Min(0)] public float flightDrag;
        public bool collideWithFloor;
        [Range(0, 1)] public float restitution, tangentRetention;
        [Min(0)] public float restDuration;
    }

    public enum SpawnRegion { Contact, RecipientBody }

    public struct RecipientSnapshot
    {
        public bool valid;
        public Vector3 center;
        public Vector2 halfAxes;
        public static RecipientSnapshot FromBounds(Bounds bounds) => new RecipientSnapshot {
            valid = true, center = bounds.center + Vector3.up * (bounds.size.y * .2f),
            halfAxes = new Vector2(bounds.extents.x + 1f / 16, bounds.size.y * .3f)
        };
    }

    [System.Serializable]
    public sealed class Group
    {
        public string name;
        [Min(0)] public int count;
        [Tooltip("Renderer channel supplied by the effect adapter. Parry: 1 fragments, 2 near, 4 far; 0/3 reserved for contacts.")]
        public int channel;
        public bool small;
        public SpawnRegion spawnRegion;
        public bool towardPlayer;
        public Vector2 angles = new Vector2(-20, 50);
        [Min(0)] public float angleJitter = 3;
        public Vector2 speed = new Vector2(14, 24), depthSpeed;
        [Min(0)] public float spawnRadius, playerOffset;
        public Vector2Int longPixels = new Vector2Int(12, 16), shortPixels = new Vector2Int(6, 9);
        [Min(1)] public int widthPixels = 3;
        public Vector2 lifetime = new Vector2(5, 5);
        public PhysicsSettings physics;
    }

    public Group[] groups = System.Array.Empty<Group>();
}
