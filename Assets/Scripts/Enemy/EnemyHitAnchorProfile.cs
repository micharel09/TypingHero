using UnityEngine;

// Coordinates are pixels from the sprite rect's bottom-left, before pivot/flip.
[CreateAssetMenu(menuName = "Combat/Enemy Hit Anchor Profile")]
public sealed class EnemyHitAnchorProfile : ScriptableObject
{
    [System.Serializable]
    public struct Pose
    {
        public Sprite sprite;
        public Vector2 chest;
    }
    public Pose[] poses;

    public bool TryGet(Sprite sprite, out Vector2 point)
    {
        if (poses != null)
            foreach (var pose in poses)
                if (pose.sprite == sprite) { point = pose.chest; return true; }
        point = default;
        return false;
    }
}
