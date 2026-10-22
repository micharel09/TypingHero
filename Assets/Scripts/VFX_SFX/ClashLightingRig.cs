using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(210)]
public sealed class ClashLightingRig : MonoBehaviour
{
    [SerializeField] Light2D clashLight;
    [SerializeField] Light combatLight;
    [SerializeField] Light parryLight;
    public CombatLightingProfile profile;
    struct PulseState { public Component owner; public Vector3 point; public float strength, age, duration, hold; public bool compact; public CombatLightingProfile.Event kind; }
    PulseState[] pulses = System.Array.Empty<PulseState>();
    void Prepare()
    {
        if (!profile) throw new System.InvalidOperationException(name + ": missing CombatLightingProfile");
        if (pulses.Length == profile.overlapCapacity) return;
        profile.Validate();
        pulses = new PulseState[profile.overlapCapacity];
    }
    static readonly System.Collections.Generic.List<ClashLightingRig> rigs = new();
    Light2D key, fill, ambient;
    void OnEnable()
    {
        if (profile)
        {
            Prepare();
            if (combatLight) profile.combat.Apply(combatLight);
            if (parryLight) profile.parry.Apply(parryLight);
        }
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
        if (parryLight) parryLight.intensity = 0;
    }
    internal static void Pulse(Scene scene, Vector3 point, CombatLightingProfile.Event kind, ParryClashBurst owner)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.Add(point, kind, owner);
    }
    internal static void Impact(Scene scene, Vector3 point, CombatLightingProfile.Event kind, VoxelImpactBurst owner)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.Add(point, kind, owner);
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
    void Add(Vector3 point, CombatLightingProfile.Event kind, Component owner)
    {
        Prepare(); var settings = profile.Get(kind);
        AddProfile(new PulseState { owner = owner, point = point, strength = settings.intensity,
            duration = settings.duration, hold = settings.hold, kind = kind,
            compact = kind != CombatLightingProfile.Event.LegacyParry });
    }
    float Intensity(PulseState p)
    {
        if (!p.owner) return 0;
        var settings = profile.Get(p.kind);
        float t = Mathf.Clamp01((p.age - p.hold) / (p.duration - p.hold));
        return Mathf.Min(settings.maximum, p.strength * settings.decay.Evaluate(t));
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
        if (!clashLight && !combatLight && !parryLight) return;
        float strongest = 0; Vector3 point = transform.position; ParryClashBurst parry = null;
        foreach (var pulse in pulses) if (pulse.owner)
        {
            float intensity = Intensity(pulse);
            if (intensity > strongest) { strongest = intensity; point = pulse.point;
                parry = pulse.compact ? pulse.owner as ParryClashBurst : null; }
        }
        if (clashLight) { clashLight.transform.position = point; clashLight.intensity = strongest; }
        if (combatLight)
        {
            combatLight.transform.position = point + profile.combatOffset;
            combatLight.intensity = parry && parryLight ? 0 : strongest;
        }
        if (parryLight)
        {
            var camera = Camera.main;
            float units = parry ? parry.PixelUnitsPerMeter : profile.defaultPixelUnits;
            parryLight.transform.position = point - (camera ? camera.transform.forward : Vector3.forward) * (profile.parryDepthOffset * units);
            parryLight.range = profile.parryRangeScale * units; parryLight.intensity = parry ? strongest : 0;
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
