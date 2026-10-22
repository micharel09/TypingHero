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

    [System.Serializable]
    public sealed class Group
    {
        public string name;
        [Min(0)] public int count;
        [Tooltip("Renderer channel supplied by the effect adapter. Parry: 1 fragments, 2 near, 4 far; 0/3 reserved for contacts.")]
        public int channel;
        public bool small;
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
