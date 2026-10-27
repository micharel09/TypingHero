using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(200)]
public sealed class SlayerHitVfx : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    [SerializeField] SpriteRenderer image;
    [SerializeField] float uniformScale;
    [SerializeField] float[] frameDurations;
    [SerializeField] Vector2[] frameCenters;
    [SerializeField] int contactFrame;
    int startFrame;
    EnemyHitAnchor anchor;
    uint anchorLifetime;
    PooledVFX lease;
    float age;
    float fadeAge = -1;
    int bornFrame;
    internal float Age => age;
    internal Transform Recipient { get; private set; }
    internal bool IsFading => fadeAge >= 0;
    internal bool IsPrimaryFor(Transform recipient, EnemyHitAnchor hitAnchor, uint lifetime) =>
        !IsFading && Recipient == recipient && anchor == hitAnchor && anchorLifetime == lifetime &&
        (!anchor || anchor.Lifetime == lifetime);
    void Awake() => lease = GetComponent<PooledVFX>();
    internal void Emit(Vector3 point, float clockwiseAngle, Color tint, Transform recipient = null, EnemyHitAnchor hitAnchor = null, uint lifetime = 0, bool startAtContact = false)
    {
        age = 0;
        startFrame = startAtContact ? contactFrame : 0;
        fadeAge = -1;
        Recipient = recipient;
        anchor = hitAnchor;
        anchorLifetime = lifetime;
        bornFrame = Time.frameCount;
        transform.SetPositionAndRotation(point, Quaternion.Euler(0, 0, -clockwiseAngle));
        transform.localScale = Vector3.one * uniformScale;
        image.color = Color.black;
        image.flipX = image.flipY = false;
        ApplyFrame(startFrame);
    }
    internal void Retire()
    {
        fadeAge = 0;
        var color = image.color; color.a = Mathf.Min(color.a, .35f); image.color = color;
    }
    internal void Advance(float delta)
    {
        age += delta;
        if (IsFading)
        {
            fadeAge += delta;
            if (fadeAge >= .03f) { lease.Release(); return; }
            var color = image.color; color.a = .35f * (1 - fadeAge / .03f); image.color = color;
        }
        float end = 0;
        for (int i = startFrame; i < frames.Length; i++)
        {
            end += frameDurations[i];
            if (age < end) { ApplyFrame(i); return; }
        }
        lease.Release();
    }
    void ApplyFrame(int index)
    {
        image.sprite = frames[index];
        image.transform.localPosition = -frameCenters[index];
    }
    void LateUpdate()
    {
        // Animator and hit-pose jitter finish before this world-space attachment.
        if (anchor && anchor.Lifetime != anchorLifetime) { lease.Release(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        if (anchor && anchor.TryGetWorldPoint(out var point)) transform.position = point;
    }
    void Update()
    {
        // A lethal contact can finish the session before its short visual tail ends.
        if (GamePauseController.ResultOverlayVisible) { lease.Release(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0 || bornFrame == Time.frameCount) return;
        Advance(Time.unscaledDeltaTime);
    }
    void OnDisable()
    {
        age = 0;
        startFrame = 0;
        fadeAge = -1;
        Recipient = null;
        anchor = null;
        anchorLifetime = 0;
        transform.localRotation = Quaternion.identity;
        if (image) { image.transform.localPosition = Vector3.zero; image.sprite = null; image.color = Color.white; image.flipX = image.flipY = false; }
    }
}
