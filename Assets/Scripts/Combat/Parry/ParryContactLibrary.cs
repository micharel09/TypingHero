using UnityEngine;

// Presentation snapshot, taken while both attack/defence cels are still displayed.
internal struct ParryContactSnapshot
{
    internal bool valid;
    internal Vector3 point, slide, incoming, reflected;
    internal float unitsPerMeter;
    internal float separation;
    internal Sprite defenseCel, attackCel;
}

internal sealed class ParryContactLibrary : ScriptableObject
{
    [System.Serializable]
    internal struct Frame
    {
        public Sprite sprite;
        public Sprite contactCel;
        public Sprite defenseCel;
        // Source-cel pixels, measured from the bottom left (before pivot/flip/scale).
        public Vector2 heel, tip, incoming;
        public bool defense;
    }
    [SerializeField] internal Frame[] frames;
    [SerializeField] internal int version;

    internal ParryContactSnapshot Capture(SpriteRenderer player, SpriteRenderer enemy, Camera camera)
    {
        if (!player) return default;
        if (!TryFrame(player, true, out var defense))
        {
            if (frames == null || frames.Length == 0) return default;
            defense = frames[0];
        }
        bool hasAttack = TryFrame(enemy, false, out var attack);
        if (hasAttack && attack.defenseCel)
            foreach (var candidate in frames)
                if (candidate.defense && candidate.sprite == attack.defenseCel) { defense = candidate; break; }
        Vector3 a = World(player, defense.heel, defense.sprite), b = World(player, defense.tip, defense.sprite);
        Vector3 point = Vector3.Lerp(a, b, .6f), incoming = Vector3.left;
        float separation = 0; Sprite attackCel = null;
        if (hasAttack)
        {
            var cel = attack.contactCel ? attack.contactCel : attack.sprite;
            Vector3 c = World(enemy, attack.heel, cel), d = World(enemy, attack.tip, cel);
            // Orthographic screen-plane geometry also works with the tilted arena camera.
            Vector3 right = camera ? camera.transform.right : Vector3.right;
            Vector3 up = camera ? camera.transform.up : Vector3.up;
            Vector2 Project(Vector3 v) => new Vector2(Vector3.Dot(v, right), Vector3.Dot(v, up));
            Closest(Project(a), Project(b), Project(c), Project(d), out float u, out float v);
            point = (Vector3.Lerp(a, b, u) + Vector3.Lerp(c, d, v)) * .5f;
            separation = Vector2.Distance(Vector2.Lerp(Project(a), Project(b), u), Vector2.Lerp(Project(c), Project(d), v));
            attackCel = cel;
            incoming = World(enemy, attack.heel + attack.incoming, cel) - c;
        }
        Vector3 blade = (b - a).normalized;
        if (Vector3.Dot(blade, incoming) < 0) blade = -blade;
        // Deflected along the blade, with a small outward component; never radial around the actor.
        Vector3 normal = Vector3.Cross(blade, camera ? camera.transform.forward : Vector3.forward).normalized;
        Vector3 reflected = Vector3.Reflect(incoming.normalized, normal);
        Vector3 slide = (blade + reflected * .3f).normalized;
        var body = player.GetComponent<BoxCollider2D>();
        return new ParryContactSnapshot { valid = true, point = point, slide = slide,
            incoming = incoming.normalized, unitsPerMeter = body ? body.bounds.size.y / 1.75f : 1.25f,
            reflected = reflected,
            separation = separation, defenseCel = defense.sprite, attackCel = attackCel };
    }

    bool TryFrame(SpriteRenderer renderer, bool defense, out Frame frame)
    {
        if (renderer && renderer.sprite && frames != null)
            foreach (var candidate in frames)
                if (candidate.sprite == renderer.sprite && candidate.defense == defense) { frame = candidate; return true; }
        frame = default; return false;
    }

    internal static Vector3 World(SpriteRenderer renderer, Vector2 pixel, Sprite cel = null)
    {
        if (!cel) cel = renderer.sprite;
        Vector2 local = (pixel - cel.pivot) / cel.pixelsPerUnit;
        if (renderer.flipX) local.x = -local.x;
        if (renderer.flipY) local.y = -local.y;
        return renderer.transform.TransformPoint(local);
    }

    // Segment intersection, otherwise the nearest endpoint projection pair.
    internal static void Closest(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out float u, out float v)
    {
        Vector2 ab = b - a, cd = d - c, ac = c - a;
        float Cross(Vector2 x, Vector2 y) => x.x * y.y - x.y * y.x;
        float determinant = Cross(ab, cd);
        if (Mathf.Abs(determinant) > .000001f)
        {
            float x = Cross(ac, cd) / determinant, y = Cross(ac, ab) / determinant;
            if (x >= 0 && x <= 1 && y >= 0 && y <= 1) { u = x; v = y; return; }
        }
        float best = float.PositiveInfinity, bu = 0, bv = 0;
        void Pair(float x, float y)
        {
            float distance = (Vector2.Lerp(a, b, x) - Vector2.Lerp(c, d, y)).sqrMagnitude;
            if (distance < best) { best = distance; bu = x; bv = y; }
        }
        float Project(Vector2 p, Vector2 start, Vector2 delta) => Mathf.Clamp01(Vector2.Dot(p - start, delta) / Mathf.Max(.000001f, delta.sqrMagnitude));
        Pair(0, Project(a, c, cd)); Pair(1, Project(b, c, cd));
        Pair(Project(c, a, ab), 0); Pair(Project(d, a, ab), 1);
        u = bu; v = bv;
    }
}
