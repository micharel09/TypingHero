using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyHitAnchor : MonoBehaviour
{
    [SerializeField] SpriteRenderer body;
    [SerializeField] EnemyHitAnchorProfile profile;
    public SpriteRenderer Body => body;
    public EnemyHitAnchorProfile Profile => profile;
    internal uint Lifetime { get; private set; }

    void OnEnable() => Lifetime++;

    internal void Configure(EnemyHitAnchorProfile value)
    {
        profile = value;
        Lifetime++;
    }

    internal bool TryGetWorldPoint(out Vector3 point)
    {
        point = default;
        if (!isActiveAndEnabled || !body || !body.enabled || !body.gameObject.activeInHierarchy ||
            !body.sprite || !profile || !profile.TryGet(body.sprite, out var pixel)) return false;
        var local = (pixel - body.sprite.pivot) / body.sprite.pixelsPerUnit;
        if (body.flipX) local.x = -local.x;
        if (body.flipY) local.y = -local.y;
        point = body.transform.TransformPoint(local);
        return true;
    }
}
