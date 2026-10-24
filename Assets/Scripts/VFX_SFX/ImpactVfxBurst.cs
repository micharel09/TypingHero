using System;
using System.Collections.Generic;
using UnityEngine;

// Explicit emission after placement; the analytical solver retains absolute-age trajectories.
[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
public partial class ImpactVfxBurst : MonoBehaviour
{
    [Serializable] public sealed class RendererBinding
    {
        public int id;
        public ParticleSystem particles;
        public Material material;
    }
    [SerializeField] internal ImpactVfxProfile impactProfile;
    [SerializeField] RendererBinding[] rendererBindings = Array.Empty<RendererBinding>();
    [SerializeField] int directionalFlashChannel = -1;
    protected ParticleSystem[] systems;
    protected ParticlePresetState[] original;
    protected readonly List<Vector4> shapeData = new(64);
    protected PooledVFX lease;
    protected bool playing, paused;
    protected int debrisEmissionFrame = -1;
    bool analytical;
    bool[] flashChannels;
    internal ParticleSystem[] Systems => systems;
#if TYPINGHERO_AUTOMATION
    internal bool Analytical => analytical;
    internal bool IsPhysicalChannel(int index) => !flashChannels[index];
    internal int ChannelId(int index) => rendererBindings[index].id;
    internal float PixelsPerUnit => pixelsPerUnit;
#endif
    public int ParticleCount { get { int count = 0; foreach (var ps in systems) count += ps.particleCount; return count; } }
    protected virtual void Awake()
    {
        systems = new ParticleSystem[rendererBindings.Length];
        original = new ParticlePresetState[systems.Length]; flashChannels = new bool[systems.Length];
        lease = GetComponent<PooledVFX>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i] = rendererBindings[i].particles;
            original[i] = new ParticlePresetState(systems[i]);
        }
    }
    internal void Validate(ImpactVfxProfile profile)
    {
        if (!profile) throw new InvalidOperationException(name + ": missing ImpactVfxProfile");
        profile.Validate(DebrisCapacity);
        for (int i = 0; i < rendererBindings.Length; i++)
        {
            var binding = rendererBindings[i];
            if (!binding.particles || !binding.material) throw new InvalidOperationException(name + ": missing renderer binding");
            for (int j = 0; j < i; j++)
                if (binding.id == rendererBindings[j].id || binding.particles == rendererBindings[j].particles)
                    throw new InvalidOperationException(name + ": duplicate renderer binding");
        }
        foreach (var binding in rendererBindings)
        {
            int count = 0;
            foreach (var group in profile.physics.groups) if (group.channel == binding.id) count += group.count;
            if (count > binding.particles.main.maxParticles)
                throw new InvalidOperationException(name + ": physical count exceeds renderer capacity");
        }
        foreach (var group in profile.physics.groups) if (group.count > 0) RequireBinding(group.channel);
        foreach (var flash in profile.flashes)
            if (flash.count > rendererBindings[RequireBinding(flash.channel)].particles.main.maxParticles)
                throw new InvalidOperationException(name + ": flash count exceeds renderer capacity");
    }
    int RequireBinding(int id)
    {
        for (int i = 0; i < rendererBindings.Length; i++) if (rendererBindings[i].id == id) return i;
        throw new InvalidOperationException(name + ": missing renderer channel " + id);
    }
    internal void Play(ImpactVfxProfile profile, ImpactVfxContext context)
    {
        Validate(profile);
        // An active prefab can Awake before the emitter supplies its missing lease.
        if (!lease) lease = GetComponent<PooledVFX>();
        Clear(); OnPlay();
        appearance = profile.Capture(); analytical = true;
        pixelsPerUnit = profile.pixelsPerUnit; depthScale = profile.depthScale; depthLimits = profile.depthLimits;
        debrisEmissionFrame = Time.frameCount;
        transform.SetPositionAndRotation(context.point, Quaternion.identity); transform.localScale = Vector3.one;
        for (int i = 0; i < systems.Length; i++) original[i].Restore(systems[i]);
        ConfigurePixel(profile);
        EmitPixel(profile, context);
        foreach (var ps in systems) ps.Play(false);
        for (int i = 0; i < systems.Length; i++) if (flashChannels[i]) systems[i].Pause(false);
        playing = true;
        ClashLightingRig.Pulse(gameObject.scene, context.point, appearance, this);
    }
    protected virtual void OnPlay() { }
    protected void Update()
    {
        if (!playing) return;
        if (CombatSession.Finished) { Release(); return; }
        bool hold = GamePauseController.InputBlocked || Time.timeScale == 0f;
        if (hold != paused)
        {
            paused = hold;
            foreach (var ps in systems) { if (hold) ps.Pause(false); else ps.Play(false); }
        }
        if (hold) return;
        if (analytical && Time.frameCount != debrisEmissionFrame)
        {
            float delta = Time.unscaledDeltaTime;
            if (debrisEmissionFrame >= 0)
            {
                for (int i = 0; i < systems.Length; i++) if (flashChannels[i]) systems[i].Play(false);
                // Preserve the approved first visible step after a cold render.
                if (delta > 1f / 30) delta = 1f / 60;
            }
            debrisEmissionFrame = -1; AdvanceDebris(delta);
        }
        if (analytical)
        {
            if (debrisCount > 0) return;
            for (int i = 0; i < systems.Length; i++) if (flashChannels[i] && systems[i].particleCount > 0) return;
            // The rig owns the pulse clock; a longer local light still owns this lease.
            if (ClashLightingRig.HasPulse(this)) return;
        }
        else foreach (var ps in systems) if (ps.IsAlive(false)) return;
        Release();
    }
    void Release() { if (lease) lease.Release(); else { Clear(); gameObject.SetActive(false); } }
    internal virtual void Clear()
    {
        if (systems != null) foreach (var ps in systems) if (ps) ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        debrisAge = 0; debrisCount = 0; debrisEmissionFrame = -1;
        simulation.Clear(); Array.Clear(identities, 0, identities.Length); Array.Clear(debrisParticles, 0, debrisParticles.Length);
        playing = paused = analytical = false;
        ClashLightingRig.Cancel(this);
    }
    protected void OnDisable() => Clear();
}
