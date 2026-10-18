using UnityEngine;

// Runs after pause input (-100) and tutorial reading (100), before particle simulation.
[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
public sealed class PixelSparkBurst : MonoBehaviour
{
    public enum Profile { Normal, Slayer, Parry, Lethal }
    [SerializeField] Profile profile;
    [SerializeField] ParticleSystem dots, streaks;
    [SerializeField] Transform ground;
    [SerializeField] int count = 40;
    [SerializeField] Vector2 lifetime = new Vector2(.6f, .85f);
    [SerializeField] Vector2 horizontalSpeed = new Vector2(3f, 7f);
    [SerializeField] Vector2 verticalSpeed = new Vector2(1f, 4f);
    PooledVFX lease;
    bool playing, paused;
    public Profile Role => profile;
    public int ParticleCount => dots.particleCount + streaks.particleCount;
    public float GroundY => ground.position.y;
    internal ParticleSystem[] Systems { get; private set; }

    void Awake() { lease = GetComponent<PooledVFX>(); Systems = new[] { dots, streaks }; }

    internal void Emit(Vector2 point, float direction, float groundY)
    {
        Clear();
        transform.SetPositionAndRotation(point, Quaternion.identity);
        transform.localScale = Vector3.one;
        ground.SetPositionAndRotation(new Vector3(point.x, groundY, 0), Quaternion.identity);
        direction = direction < 0 ? -1 : 1;
        int dotCount = Mathf.RoundToInt(count * .7f);
        for (int i = 0; i < count; i++)
        {
            bool dot = i < dotCount;
            float x = dot ? (Random.value < .5f ? 1 : 2) : (Random.value < .5f ? 3 : 1);
            float y = dot ? x : (x == 3 ? 1 : 3);
            var particle = new ParticleSystem.EmitParams
            {
                position = point,
                velocity = new Vector3(direction * (i < Mathf.RoundToInt(count * .8f) ? 1 : -1) *
                    Random.Range(horizontalSpeed.x, horizontalSpeed.y), Random.Range(verticalSpeed.x, verticalSpeed.y), 0),
                startLifetime = Random.Range(lifetime.x, lifetime.y),
                startColor = Color.white,
                startSize3D = new Vector3(x / 16f, y / 16f, 1f / 16f),
                rotation = 0f
            };
            (dot ? dots : streaks).Emit(particle, 1);
        }
        playing = true;
        dots.Play(false); streaks.Play(false);
    }

    void Update()
    {
        if (!playing) return;
        // The final confirmed impact is allowed to finish; earlier bursts stop at match end.
        if (GamePauseController.ResultOverlayVisible || (CombatSession.Finished && profile != Profile.Lethal))
        { lease.Release(); return; }
        bool hold = GamePauseController.InputBlocked || Time.timeScale == 0f;
        if (hold != paused)
        {
            paused = hold;
            if (hold) { dots.Pause(false); streaks.Pause(false); }
            else { dots.Play(false); streaks.Play(false); }
        }
        if (!hold && !dots.IsAlive(false) && !streaks.IsAlive(false)) lease.Release();
    }

    internal void Clear()
    {
        if (dots) dots.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (streaks) streaks.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        playing = paused = false;
    }
    void OnDisable() => Clear();
}
