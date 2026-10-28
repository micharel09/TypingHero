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
    struct PulseState { public Component owner; public Vector3 point; public float strength, age, duration, hold; public double startedAt; public bool compact; public CombatLightingProfile.Event kind; public float maximum, range, offset, rise, backgroundMinimum; public AnimationCurve decay; public bool local, cinematic; public Color color; }
    PulseState[] pulses = System.Array.Empty<PulseState>();
    // Per-rig unscaled time. Polling an unannounced freeze transition conservatively
    // discards that interval; controller events account up to the actual transition.
    internal struct PulseClock
    {
        internal double Time { get; private set; }
        double lastTime;
        bool held;
        internal void Reset(double now, bool frozen) { Time = 0; lastTime = now; held = frozen; }
        internal void Sample(double now, bool frozen)
        {
            now = System.Math.Max(lastTime, now);
            if (!held && !frozen) Time += now - lastTime;
            lastTime = now; held = frozen;
        }
        internal void FreezeAt(double now, bool frozen)
        {
            now = System.Math.Max(lastTime, now);
            if (!held) Time += now - lastTime;
            lastTime = now; held = frozen;
        }
    }
    PulseClock clock;
    static bool Frozen => GamePauseController.InputBlocked || Time.timeScale == 0f;
    void Prepare()
    {
        if (!profile) throw new System.InvalidOperationException(name + ": missing CombatLightingProfile");
        if (pulses.Length == profile.overlapCapacity) return;
        profile.Validate();
        pulses = new PulseState[profile.overlapCapacity];
    }
    static readonly System.Collections.Generic.List<ClashLightingRig> rigs = new();
    DungeonArenaCamera arena;
    Camera arenaCamera;
    internal void BindArena(DungeonArenaCamera value)
    {
        Clear();
        arena = value;
        arenaCamera = value ? value.GetComponent<Camera>() : null;
        SetBackgroundGain(1);
    }
    internal static void ClearArena(DungeonArenaCamera value)
    {
        foreach (var rig in rigs) if (rig.arena == value)
        {
            for (int i = 0; i < rig.pulses.Length; i++)
                if (rig.pulses[i].cinematic) rig.pulses[i] = default;
            rig.Render();
        }
    }
    Light2D key, fill, ambient;
    Color clashColor;
    bool clashColorCaptured;
    void OnEnable()
    {
        if (profile)
        {
            Prepare();
            if (combatLight) profile.combat.Apply(combatLight);
            if (parryLight) profile.parry.Apply(parryLight);
        }
        clock.Reset(Time.realtimeSinceStartupAsDouble, Frozen);
        GamePauseController.FreezeChanged += FreezeChanged;
        if (clashLight) { clashColor = clashLight.color; clashColorCaptured = true; }
        rigs.Add(this); Clear();
        foreach (var light in GetComponentsInChildren<Light2D>())
        {
            if (light.name == "Key") key = light;
            else if (light.name == "Fill") fill = light;
            else if (light.name == "Ambient") ambient = light;
        }
    }
    void OnDisable()
    {
        GamePauseController.FreezeChanged -= FreezeChanged;
        rigs.Remove(this); Clear(); if (combatLight) Shader.SetGlobalFloat("_VoxelWorld3D", 0);
    }
    void FreezeChanged(bool frozen) => clock.FreezeAt(Time.realtimeSinceStartupAsDouble, frozen);
    void Clear()
    {
        System.Array.Clear(pulses, 0, pulses.Length);
        SetBackgroundGain(1);
        if (clashLight) { clashLight.intensity = 0; if (clashColorCaptured) clashLight.color = clashColor; }
        if (combatLight) combatLight.intensity = 0;
        if (parryLight) parryLight.intensity = 0;
    }
    internal static void Pulse(Scene scene, Vector3 point, CombatLightingProfile.Event kind, ImpactVfxBurst owner)
    {
        double now = Time.realtimeSinceStartupAsDouble;
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.PulseAt(point, kind, owner, now, Frozen);
    }
    internal static void Pulse(Scene scene, Vector3 point, ImpactVfxProfile.Appearance settings, ImpactVfxBurst owner)
    {
        if (settings.lightIntensity <= 0) return;
        double now = Time.realtimeSinceStartupAsDouble;
        foreach (var rig in rigs) if (rig.gameObject.scene == scene)
            rig.PulseAt(point, settings, owner, now, Frozen);
    }
    // Runtime and deterministic tests use the same timestamped emission path.
    internal void PulseAt(Vector3 point, ImpactVfxProfile.Appearance settings, ImpactVfxBurst owner, double now, bool frozen)
    {
        if (settings.lightIntensity <= 0) return;
        Prepare();
        AddProfile(new PulseState { owner = owner, point = point, strength = settings.lightIntensity,
            maximum = settings.lightIntensity, duration = settings.lightDuration, hold = settings.lightHold,
            decay = settings.lightDecay, range = settings.lightRange, offset = settings.lightOffset,
            rise = settings.lightRise, backgroundMinimum = settings.backgroundMinimumGain, color = settings.lightColor, cinematic = settings.cinematicPulse, local = true, compact = true }, now, frozen);
    }
    internal static void Impact(Scene scene, Vector3 point, CombatLightingProfile.Event kind, VoxelImpactBurst owner)
    {
        double now = Time.realtimeSinceStartupAsDouble;
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.PulseAt(point, kind, owner, now, Frozen);
    }
    internal static bool HasPulse(Component owner)
    {
        foreach (var rig in rigs)
            foreach (var pulse in rig.pulses)
                if (pulse.owner == owner) return true;
        return false;
    }
    internal static void Cancel(Component owner)
    {
        double now = Time.realtimeSinceStartupAsDouble;
        foreach (var rig in rigs) rig.CancelAt(owner, now, Frozen);
    }
    internal void CancelAt(Component owner, double now, bool frozen)
    {
        SampleAt(now, frozen);
        for (int i = 0; i < pulses.Length; i++) if (pulses[i].owner == owner) pulses[i] = default;
        Render();
    }
    internal void PulseAt(Vector3 point, CombatLightingProfile.Event kind, Component owner, double now, bool frozen)
    {
        Prepare();
        var settings = profile.Get(kind);
        if (settings.intensity <= 0) return;
        if (settings.duration <= settings.hold) throw new System.InvalidOperationException("Light pulse duration must exceed hold");
        AddProfile(new PulseState { owner = owner, point = point, strength = settings.intensity,
            duration = settings.duration, hold = settings.hold, kind = kind, maximum = settings.maximum, decay = settings.decay,
            range = profile.parryRangeScale,
            offset = profile.parryDepthOffset,
            compact = kind != CombatLightingProfile.Event.LegacyParry }, now, frozen);
    }
    float Intensity(PulseState p)
    {
        if (!p.owner) return 0;
        if (p.cinematic) return p.strength * CinematicEnvelope(p);
        float t = Mathf.Clamp01((p.age - p.hold) / (p.duration - p.hold));
        return Mathf.Min(p.maximum, p.strength * p.decay.Evaluate(t));
    }
    void AddProfile(PulseState pulse, double now, bool frozen)
    {
        SampleAt(now, frozen);
        pulse.startedAt = clock.Time;
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
    void Update() => SampleAt(Time.realtimeSinceStartupAsDouble, Frozen);
    internal void SampleAt(double now, bool frozen)
    {
        clock.Sample(now, frozen);
        if (GamePauseController.ResultOverlayVisible) { Clear(); return; }
        if (frozen) return;
        for (int i = 0; i < pulses.Length; i++) if (pulses[i].owner)
        {
            if (CombatSession.Finished && !(pulses[i].owner is VoxelImpactBurst impact && impact.Lethal)) { pulses[i] = default; continue; }
            double age = clock.Time - pulses[i].startedAt;
            pulses[i].age = (float)age;
            if (age >= pulses[i].duration) pulses[i] = default;
        }
        Render();
    }
    static float CinematicEnvelope(PulseState p) => p.age < p.rise ? Mathf.Clamp01(p.age / p.rise) :
        p.age <= p.hold ? 1 : Mathf.Clamp01((p.duration - p.age) / (p.duration - p.hold));
    void SetBackgroundGain(float gain)
    {
        if (arena) arena.ParryBackgroundGain = gain;
    }
    void Render()
    {
        float background = 1;
        foreach (var pulse in pulses) if (pulse.owner && pulse.cinematic)
            background = Mathf.Min(background, Mathf.Lerp(1, pulse.backgroundMinimum, CinematicEnvelope(pulse)));
        SetBackgroundGain(background);
        if (!clashLight && !combatLight && !parryLight) return;
        float strongest = 0; Vector3 point = transform.position; ImpactVfxBurst parry = null; float range = profile ? profile.parryRangeScale : 0, offset = profile ? profile.parryDepthOffset : 0;
        Color tint = profile ? profile.parry.color : Color.white;
        bool local = false;
        foreach (var pulse in pulses) if (pulse.owner)
        {
            float intensity = Intensity(pulse);
            if (intensity > strongest) { strongest = intensity; point = pulse.point; range = pulse.range; offset = pulse.offset;
                parry = pulse.compact ? pulse.owner as ImpactVfxBurst : null; local = pulse.local; tint = pulse.local ? pulse.color : profile.parry.color; }
        }
        if (clashLight) { clashLight.transform.position = point; clashLight.intensity = strongest; clashLight.color = local ? tint : clashColor; }
        if (combatLight)
        {
            combatLight.transform.position = point + profile.combatOffset;
            combatLight.color = local ? tint : profile.combat.color;
            combatLight.intensity = parry && parryLight ? 0 : strongest;
        }
        if (parryLight)
        {
            var camera = arenaCamera ? arenaCamera : Camera.main;
            float units = parry ? parry.PixelUnitsPerMeter : profile.defaultPixelUnits;
            parryLight.transform.position = point - (camera ? camera.transform.forward : Vector3.forward) * (offset * units);
            parryLight.color = tint;
            parryLight.range = range * units; parryLight.intensity = parry ? strongest : 0;
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
