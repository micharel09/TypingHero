using UnityEngine;

// Small unlit accents; the existing impact fragments and lights own their own lifetime.
[DefaultExecutionOrder(200)]
public sealed class StrikeAccentBurst : MonoBehaviour
{
    [SerializeField] bool dust;
    [SerializeField] ParticleSystem particles;
    readonly ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[3];
    PooledVFX lease;
    Vector3 origin;
    float age, direction;
    int bornFrame;
    internal float Age => age;
    internal int ParticleCount => particles.particleCount;
    internal bool IsDust => dust;
    internal float Duration => dust ? .18f : .06f;

    void Awake() => lease = GetComponent<PooledVFX>();
    internal void Emit(Vector3 point, float facing)
    {
        Clear(); origin = point; direction = facing < 0 ? -1 : 1;
        transform.SetPositionAndRotation(point, Quaternion.identity);
        transform.localScale = Vector3.one;
        bornFrame = Time.frameCount;
        particles.Play(false);
        Advance(0);
    }
    internal void Advance(float delta)
    {
        age += delta;
        int count = age >= Duration ? 0 : dust ? 3 : 1;
        float t = Mathf.Clamp01(age / Duration);
        for (int i = 0; i < count; i++)
        {
            // Dimensions are native world pixels (16 PPU), independent of actor scale.
            float width = dust ? (3 - i * .5f) / 16f : 8f / 16f;
            float height = dust ? (i == 0 ? 2 : 1) / 16f : 8f / 16f;
            Vector3 position = origin;
            if (dust)
            {
                position.x -= direction * (i * 2f / 16f + t * (3 + i) / 16f);
                position.y += height * .5f + Mathf.Sin(t * Mathf.PI) / 32f;
            }
            buffer[i] = new ParticleSystem.Particle {
                position = position, startSize3D = new Vector3(width, height, 1),
                startColor = dust ? new Color(.48f, .48f, .46f, 1 - t) : new Color(1, .96f, .82f, Mathf.Clamp01((Duration - age) / .02f)),
                startLifetime = Duration, remainingLifetime = Duration - age + .1f,
                randomSeed = (uint)(i + 1)
            };
        }
        particles.SetParticles(buffer, count);
    }
    void Update()
    {
        if (GamePauseController.ResultOverlayVisible || CombatSession.Finished) { lease.Release(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0 || bornFrame == Time.frameCount) return;
        Advance(Time.unscaledDeltaTime);
        if (age >= Duration) lease.Release();
    }
    internal void Clear()
    {
        if (particles) particles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        age = 0;
    }
    void OnDisable() => Clear();
}
