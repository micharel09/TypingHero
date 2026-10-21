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
    struct PulseState { public bool active; public ParryClashBurst owner; public Vector3 point; public float strength, age; }
    readonly PulseState[] pulses = new PulseState[8];
    static readonly System.Collections.Generic.List<ClashLightingRig> rigs = new();
    void OnEnable() { rigs.Add(this); Clear(); }
    void OnDisable() { rigs.Remove(this); Clear(); }
    void Clear() { System.Array.Clear(pulses, 0, pulses.Length); if (clashLight) clashLight.intensity = 0; if (combatLight) { combatLight.intensity = 0; combatLight.enabled = false; } }
    internal static void Pulse(Scene scene, Vector3 point, float strength, ParryClashBurst owner)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene) rig.Add(point, strength, owner);
    }
    internal static void Impact(Scene scene, Vector3 point)
    {
        foreach (var rig in rigs) if (rig.gameObject.scene == scene && rig.combatLight) rig.Add(point, .9f, null);
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
            if (!pulses[i].active) { index = i; break; }
            if (pulses[i].age > pulses[index].age) index = i;
        }
        pulses[index] = new PulseState { active = true, owner = owner, point = point, strength = strength };
        Render();
    }
    void Update()
    {
        if (CombatSession.Finished) { Clear(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0f) return;
        for (int i = 0; i < pulses.Length; i++) if (pulses[i].active)
        {
            pulses[i].age += Time.unscaledDeltaTime;
            if (pulses[i].age >= duration) pulses[i] = default;
        }
        Render();
    }
    void Render()
    {
        if (!clashLight && !combatLight) return;
        float strongest = 0; Vector3 point = transform.position;
        foreach (var pulse in pulses) if (pulse.active)
        {
            float intensity = pulse.strength * Mathf.Pow(1 - pulse.age / duration, 2);
            if (intensity > strongest) { strongest = intensity; point = pulse.point; }
        }
        if (clashLight) { clashLight.transform.position = point; clashLight.intensity = Mathf.Min(maximumIntensity, strongest); }
        if (combatLight)
        {
            combatLight.transform.position = point;
            combatLight.intensity = Mathf.Min(maximumIntensity, strongest); combatLight.enabled = strongest > 0;
        }
    }
}
