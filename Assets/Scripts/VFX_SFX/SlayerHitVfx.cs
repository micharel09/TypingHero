using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(200)]
public sealed class SlayerHitVfx : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    [SerializeField] SpriteRenderer image;
    [SerializeField] float uniformScale;
    [SerializeField] float duration = .15f;
    PooledVFX lease;
    float age;
    int bornFrame;
    internal float Age => age;
    void Awake() => lease = GetComponent<PooledVFX>();
    internal void Emit(Vector3 point, float direction, Color tint)
    {
        age = 0;
        bornFrame = Time.frameCount;
        transform.SetPositionAndRotation(point, Quaternion.identity);
        transform.localScale = Vector3.one * uniformScale;
        tint.a = 1;
        image.color = tint;
        image.flipX = direction < 0;
        image.flipY = false;
        image.sprite = frames[0];
    }
    internal void Advance(float delta)
    {
        age += delta;
        if (age >= duration) { lease.Release(); return; }
        image.sprite = frames[Mathf.Min(frames.Length - 1, Mathf.FloorToInt(age * frames.Length / duration))];
    }
    void Update()
    {
        // A lethal contact can finish the session before its short visual tail ends.
        if (GamePauseController.ResultOverlayVisible) { lease.Release(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0 || bornFrame == Time.frameCount) return;
        Advance(Time.unscaledDeltaTime);
    }
    void OnDisable() { age = 0; if (image) { image.sprite = null; image.color = Color.white; image.flipX = image.flipY = false; } }
}
