using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(210)]
public sealed class ClashLightingRig : MonoBehaviour
{
    [SerializeField] Light2D clashLight;
    [SerializeField] float duration = .14f;
    [SerializeField] float maximumIntensity = 3f;
    // A fixed array bounds overlap without allocating in the combat loop.
    struct PulseState { public ParryClashBurst owner; public Vector3 point; public float strength, age, duration, hold; public bool compact; }
    readonly PulseState[] pulses = new PulseState[8];
    static readonly System.Collections.Generic.List<ClashLightingRig> rigs = new();
    void OnEnable() { rigs.Add(this); Clear(); }
    void OnDisable() { rigs.Remove(this); Clear(); }
    void Clear() { System.Array.Clear(pulses, 0, pulses.Length); if (clashLight) clashLight.intensity = 0; }
    internal static void Pulse(Scene scene, Vector3 point, float strength, ParryClashBurst owner)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.Add(point, strength, owner);
    }
    internal static void Cancel(ParryClashBurst owner)
    {
        foreach (var rig in rigs)
        {
            for (int i = 0; i < rig.pulses.Length; i++)
                if (rig.pulses[i].owner == owner) rig.pulses[i] = default;
            rig.Render();
        }
    }
    void Add(Vector3 point, float strength, ParryClashBurst owner)
    {
        int index = 0;
        for (int i = 0; i < pulses.Length; i++)
        {
            if (!pulses[i].owner) { index = i; break; }
            if (pulses[i].age > pulses[index].age) index = i;
        }
        pulses[index] = new PulseState { owner = owner, point = point, strength = strength, compact = owner.EmittedPreset == ParryClashBurst.Preset.BCompact,
            duration = owner.EmittedPreset == ParryClashBurst.Preset.BCompact ? .28f : duration,
            hold = owner.EmittedPreset == ParryClashBurst.Preset.BCompact ? .07f : 0 };
        Render();
    }
    void Update()
    {
        if (CombatSession.Finished) { Clear(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0f) return;
        for (int i = 0; i < pulses.Length; i++) if (pulses[i].owner)
        {
            pulses[i].age += Time.unscaledDeltaTime;
            if (pulses[i].age >= pulses[i].duration) pulses[i] = default;
        }
        Render();
    }
    void Render()
    {
        if (!clashLight) return;
        float strongest = 0; Vector3 point = transform.position;
        foreach (var pulse in pulses) if (pulse.owner)
        {
            float t = Mathf.Clamp01((pulse.age - pulse.hold) / (pulse.duration - pulse.hold));
            float intensity = pulse.strength * (pulse.compact ? 1 - t * t * (3 - 2 * t) : Mathf.Pow(1 - t, 2));
            intensity = Mathf.Min(pulse.compact ? 5f : maximumIntensity, intensity);
            if (intensity > strongest) { strongest = intensity; point = pulse.point; }
        }
        clashLight.transform.position = point;
        clashLight.intensity = strongest;
    }
}
