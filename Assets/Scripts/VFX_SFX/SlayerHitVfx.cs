using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(200)]
public sealed class SlayerHitVfx : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    [SerializeField] SpriteRenderer image;
    [SerializeField] float uniformScale;
    [SerializeField] float[] frameDurations;
    PooledVFX lease;
    float age;
    float fadeAge = -1;
    int bornFrame;
    internal float Age => age;
    internal Transform Recipient { get; private set; }
    internal bool IsFading => fadeAge >= 0;
    void Awake() => lease = GetComponent<PooledVFX>();
    internal void Emit(Vector3 point, float clockwiseAngle, Color tint, Transform recipient = null)
    {
        age = 0;
        fadeAge = -1;
        Recipient = recipient;
        bornFrame = Time.frameCount;
        transform.SetPositionAndRotation(point, Quaternion.Euler(0, 0, -clockwiseAngle));
        transform.localScale = Vector3.one * uniformScale;
        tint.a = 1;
        image.color = tint;
        image.flipX = image.flipY = false;
        image.sprite = frames[0];
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
        for (int i = 0; i < frames.Length; i++)
        {
            end += frameDurations[i];
            if (age < end) { image.sprite = frames[i]; return; }
        }
        lease.Release();
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
        fadeAge = -1;
        Recipient = null;
        transform.localRotation = Quaternion.identity;
        if (image) { image.sprite = null; image.color = Color.white; image.flipX = image.flipY = false; }
    }
}
