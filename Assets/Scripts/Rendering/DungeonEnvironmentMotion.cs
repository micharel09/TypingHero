using UnityEngine;

// One scaled clock for atmosphere, hanging cloth and runes; no shader _Time drift.
public sealed class DungeonEnvironmentMotion : MonoBehaviour
{
    [SerializeField] Renderer[] atmosphere = new Renderer[0];
    [SerializeField] ParticleSystem[] weather = new ParticleSystem[0];
    [SerializeField] MeshFilter banner;
    [SerializeField] Light runeLight;
    Mesh original, deformed;
    Vector3[] rest, vertices;
    MaterialPropertyBlock block;
    float age;
    bool paused;
    ParticleSystem.Particle[][] particles;
    ParticleSystem.PlaybackState[] playback;
    int[] counts;
    bool savedWeather;
    bool restoreWeather;
    void OnEnable()
    {
        block = new MaterialPropertyBlock();
        if (banner && banner.sharedMesh)
        {
            original = banner.sharedMesh; deformed = Instantiate(original);
            deformed.name = "Dungeon pinned cloth instance"; banner.sharedMesh = deformed;
            rest = original.vertices; vertices = new Vector3[rest.Length];
        }
        if (particles == null)
        {
            particles = new ParticleSystem.Particle[weather.Length][];
            playback = new ParticleSystem.PlaybackState[weather.Length]; counts = new int[weather.Length];
            for (int i=0;i<weather.Length;i++)
                if (weather[i]) particles[i] = new ParticleSystem.Particle[weather[i].main.maxParticles];
        }
        restoreWeather = savedWeather;
        paused = false;
        for (int i=0;i<weather.Length;i++) if (weather[i])
        {
            weather[i].Play();
        }
        Apply();
    }
    void Update()
    {
        RestoreWeather();
        bool freeze = GamePauseController.InputBlocked || Time.timeScale == 0;
        if (freeze != paused)
        {
            foreach (var system in weather) if (system)
            { if (freeze) system.Pause(); else system.Play(); }
            paused = freeze;
        }
        if (freeze) return;
        age += Time.deltaTime; Apply();
    }
    void RestoreWeather()
    {
        if (!restoreWeather) return;
        paused = GamePauseController.InputBlocked || Time.timeScale == 0;
        for (int i=0;i<weather.Length;i++) if (weather[i])
        {
            weather[i].Play(); weather[i].SetPlaybackState(playback[i]);
            weather[i].SetParticles(particles[i],counts[i]);
            if (paused) weather[i].Pause();
        }
        restoreWeather=false;
    }
    void LateUpdate()
    {
        // Child particle native callbacks can clear data before the parent's
        // OnDisable. Retain the last visible state, and restore only after all
        // child OnEnable callbacks have completed.
        RestoreWeather();
        for (int i=0;i<weather.Length;i++) if (weather[i])
        {
            playback[i]=weather[i].GetPlaybackState();
            counts[i]=weather[i].GetParticles(particles[i]);
        }
        savedWeather=true;
    }
    void Apply()
    {
        float noise = Mathf.Clamp01(Mathf.PerlinNoise(11.17f + age * .11f, 29.31f + age * .07f));
        float envelope = .5f + .5f * Mathf.Sin(2 * Mathf.PI * age / 12);
        float pulse = .76f + .18f * noise + .06f * envelope;
        foreach (var renderer in atmosphere) if (renderer)
        {
            renderer.GetPropertyBlock(block);
            block.SetFloat("_DungeonAge", age); block.SetFloat("_DungeonPulse", pulse);
            renderer.SetPropertyBlock(block);
        }
        if (runeLight) runeLight.intensity = .7f * pulse;
        if (!deformed) return;
        float top = original.bounds.max.y, span = Mathf.Max(.001f, original.bounds.size.y);
        for (int i = 0; i < vertices.Length; i++)
        {
            float weight = Mathf.Clamp01((top - rest[i].y) / span);
            vertices[i] = rest[i] + Vector3.forward * (.025f * weight * weight * Mathf.Sin(age * 1.25f + rest[i].y * 2));
        }
        deformed.vertices = vertices; deformed.RecalculateBounds(); deformed.RecalculateNormals();
    }
    void OnDisable()
    {
        foreach (var system in weather) if (system) system.Pause();
        if (banner && original) banner.sharedMesh = original;
        if (deformed) Destroy(deformed);
        deformed = null;
    }
}
