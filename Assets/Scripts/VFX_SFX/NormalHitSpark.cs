using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(200)]
public sealed class NormalHitSpark : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    [SerializeField] SpriteRenderer image;
    [SerializeField] float uniformScale;
    PooledVFX lease;
    float age;
    int bornFrame;
    internal const float Duration = .16f;
    internal float Age => age;
    void Awake() => lease = GetComponent<PooledVFX>();
    internal void Emit(Vector3 point, float direction)
    {
        age = 0;
        bornFrame = Time.frameCount;
        transform.SetPositionAndRotation(point, Quaternion.identity);
        transform.localScale = Vector3.one * uniformScale;
        image.color = Color.white;
        image.flipX = direction < 0;
        image.flipY = false;
        image.sprite = frames[0];
    }
    internal void Advance(float delta)
    {
        age += delta;
        if (age >= Duration) { lease.Release(); return; }
        image.sprite = frames[Mathf.Min(frames.Length - 1, Mathf.FloorToInt(age * frames.Length / Duration))];
    }
    void Update()
    {
        if (GamePauseController.ResultOverlayVisible || CombatSession.Finished) { lease.Release(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0 || bornFrame == Time.frameCount) return;
        Advance(Time.unscaledDeltaTime);
    }
    void OnDisable() { age = 0; if (image) image.sprite = null; }
}
