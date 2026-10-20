using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(210)]
public sealed class ClashLightingRig : MonoBehaviour
{
    [SerializeField] Light2D clashLight;
    [SerializeField] Light combatLight;
    [SerializeField] float duration = .14f;
    [SerializeField] float maximumIntensity = 3f;
    // A fixed array bounds overlap without allocating in the combat loop.
    struct PulseState { public Component owner; public Vector3 point; public float strength, age, duration, hold; public bool compact; }
    readonly PulseState[] pulses = new PulseState[8];
    static readonly System.Collections.Generic.List<ClashLightingRig> rigs = new();
    Light2D key, fill, ambient;
    void OnEnable()
    {
        rigs.Add(this); Clear();
        foreach (var light in GetComponentsInChildren<Light2D>())
        {
            if (light.name == "Key") key = light;
            else if (light.name == "Fill") fill = light;
            else if (light.name == "Ambient") ambient = light;
        }
    }
    void OnDisable() { rigs.Remove(this); Clear(); if (combatLight) Shader.SetGlobalFloat("_VoxelWorld3D", 0); }
    void Clear()
    {
        System.Array.Clear(pulses, 0, pulses.Length);
        if (clashLight) clashLight.intensity = 0;
        if (combatLight) combatLight.intensity = 0;
    }
    internal static void Pulse(Scene scene, Vector3 point, float strength, ParryClashBurst owner)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.Add(point, strength, owner);
    }
    internal static void Impact(Scene scene, Vector3 point, float strength, float hold, float seconds, VoxelImpactBurst owner)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene)
            rig.AddProfile(new PulseState {owner = owner, point = point, strength = strength, duration = seconds, hold = hold, compact = true});
    }
    internal static void Cancel(Component owner)
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
        AddProfile(new PulseState { owner = owner, point = point, strength = strength, compact = owner.EmittedPreset == ParryClashBurst.Preset.BCompact,
            duration = owner.EmittedPreset == ParryClashBurst.Preset.BCompact ? .28f : duration,
            hold = owner.EmittedPreset == ParryClashBurst.Preset.BCompact ? .07f : 0 });
    }
    float Intensity(PulseState p)
    {
        float t = Mathf.Clamp01((p.age - p.hold) / (p.duration - p.hold));
        return Mathf.Min(p.compact ? 5f : maximumIntensity, p.strength * (p.compact ? 1 - t * t * (3 - 2 * t) : Mathf.Pow(1 - t, 2)));
    }
    void AddProfile(PulseState pulse)
    {
        int index = 0;
        for (int i = 0; i < pulses.Length; i++)
        {
            if (!pulses[i].owner) { index = i; break; }
            if (Intensity(pulses[i]) < Intensity(pulses[index])) index = i;
        }
        if (pulses[index].owner && Intensity(pulses[index]) > pulse.strength) return;
        pulses[index] = pulse;
        Render();
    }
    void Update()
    {
        if (GamePauseController.ResultOverlayVisible) { Clear(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0f) return;
        for (int i = 0; i < pulses.Length; i++) if (pulses[i].owner)
        {
            if (CombatSession.Finished && !(pulses[i].owner is VoxelImpactBurst impact && impact.Lethal)) { pulses[i] = default; continue; }
            pulses[i].age += Time.unscaledDeltaTime;
            if (pulses[i].age >= pulses[i].duration) pulses[i] = default;
        }
        Render();
    }
    void Render()
    {
        if (!clashLight && !combatLight) return;
        float strongest = 0; Vector3 point = transform.position;
        foreach (var pulse in pulses) if (pulse.owner)
        {
            float intensity = Intensity(pulse);
            if (intensity > strongest) { strongest = intensity; point = pulse.point; }
        }
        if (clashLight) { clashLight.transform.position = point; clashLight.intensity = strongest; }
        if (combatLight)
        {
            combatLight.transform.position = point + new Vector3(0, 2, -4);
            combatLight.intensity = strongest;
        }
    }
    void LateUpdate()
    {
        // Forward-rendered arena meshes use the same actual lights as the actors and floor.
        Shader.SetGlobalFloat("_VoxelWorld3D", combatLight ? 1 : 0);
        if (combatLight) return;
        // Preserve the shared light bridge for the legacy 2D renderer and menu.
        if (key) { Shader.SetGlobalVector("_VoxelKeyPosition", key.transform.position + Vector3.back * 3); Shader.SetGlobalColor("_VoxelKeyColor", key.color * key.intensity); }
        if (fill) { Shader.SetGlobalVector("_VoxelFillPosition", fill.transform.position + Vector3.back * 2); Shader.SetGlobalColor("_VoxelFillColor", fill.color * fill.intensity); }
        if (ambient) Shader.SetGlobalColor("_VoxelAmbient", ambient.color * ambient.intensity);
        if (clashLight) { Shader.SetGlobalVector("_VoxelPulsePosition", clashLight.transform.position + Vector3.back); Shader.SetGlobalColor("_VoxelPulseColor", clashLight.color * clashLight.intensity); }
    }
}
