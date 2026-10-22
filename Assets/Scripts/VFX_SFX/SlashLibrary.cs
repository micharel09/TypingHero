using UnityEngine;

public sealed class SlashLibrary : ScriptableObject
{
    [System.Serializable]
    internal struct Frame
    {
        [SerializeField] internal Sprite sprite;
        [SerializeField] internal Mesh front, back;
        [SerializeField] internal bool hasContact;
        [SerializeField] internal Vector2 contactA, contactB;
    }
    [SerializeField] internal Frame[] frames;
    [SerializeField] internal Material material;

    internal bool TryContact(Sprite cel, SpriteRenderer source, Collider2D target, out Vector2 point)
    {
        foreach (var frame in frames)
        {
            if (frame.sprite != cel || !frame.hasContact) continue;
            Vector2 Local(Vector2 pixel) {
                var local = (pixel - cel.pivot) / cel.pixelsPerUnit;
                if (source.flipX) local.x = -local.x;
                if (source.flipY) local.y = -local.y;
                return local;
            }
            Vector2 a = source.transform.TransformPoint(Local(frame.contactA));
            Vector2 b = source.transform.TransformPoint(Local(frame.contactB));
            // Minimise distance to the authored segment on the convex target collider.
            float lo = 0, hi = 1;
            for (int i = 0; i < 20; i++) {
                float l = (2 * lo + hi) / 3, r = (lo + 2 * hi) / 3;
                var lp = Vector2.Lerp(a, b, l); var rp = Vector2.Lerp(a, b, r);
                float left = (target.ClosestPoint(lp) - lp).sqrMagnitude;
                float right = (target.ClosestPoint(rp) - rp).sqrMagnitude;
                if (left < right) hi = r;
                else if (left > right) lo = l;
                else { lo = l; hi = r; } // A contained/flat segment contacts at its centre, not its low endpoint.
            }
            point = target.ClosestPoint(Vector2.Lerp(a, b, (lo + hi) * .5f));
            return true;
        }
        point = default; return false;
    }
}
