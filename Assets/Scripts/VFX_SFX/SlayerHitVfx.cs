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
    int bornFrame;
    internal float Age => age;
    void Awake() => lease = GetComponent<PooledVFX>();
    internal void Emit(Vector3 point, float clockwiseAngle, Color tint)
    {
        age = 0;
        bornFrame = Time.frameCount;
        transform.SetPositionAndRotation(point, Quaternion.Euler(0, 0, -clockwiseAngle));
        transform.localScale = Vector3.one * uniformScale;
        tint.a = 1;
        image.color = tint;
        image.flipX = image.flipY = false;
        image.sprite = frames[0];
    }
    internal void Advance(float delta)
    {
        age += delta;
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
        transform.localRotation = Quaternion.identity;
        if (image) { image.sprite = null; image.color = Color.white; image.flipX = image.flipY = false; }
    }
}
