using UnityEngine;
using System.Collections.Generic;

// Explicit emission after placement; simulation uses real time but obeys reading/pause locks.
[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
public sealed partial class ParryClashBurst : MonoBehaviour
{
    public enum Preset { Current, A, B, C, BCompact }
    [SerializeField] internal Preset preset;
    [SerializeField, HideInInspector] int lengthSupportVersion;
    [SerializeField, HideInInspector] int compactVersion;
    [SerializeField] Material compactMaterial;
    [SerializeField] Material compactContactMaterial;
    [SerializeField, HideInInspector] int contactVersion;
    [SerializeField] Mesh compactVoxelMesh;
    [SerializeField] Material compactVoxelMaterial;
    [SerializeField, HideInInspector] int voxelVersion;
    [SerializeField, HideInInspector] int pixelVersion;
    [SerializeField] Material pixelContactMaterial, pixelRayMaterial, pixelEmberMaterial;
    [SerializeField] Sprite pixelCore;
    [SerializeField] Sprite[] pixelBurst;
    // Retain serialized historical compact tuning when migrating existing prefabs.
    [System.Serializable] sealed class CompactSettings
    {
        public Vector2Int longPixels = new Vector2Int(12,16), shortPixels = new Vector2Int(6,9), nearPixels = new Vector2Int(4,6);
        public Vector2 rayLifetime = new Vector2(.14f,.22f), fragmentLifetime = new Vector2(.12f,.18f);
    }
    [SerializeField] CompactSettings compact = new CompactSettings();
    ParticlePresetState[] original;
    Preset emittedPreset;
    int variationIndex;
    [SerializeField] ParticleSystem contact, mainRays, front, back, embers;
    [SerializeField] Transform ground;
    [SerializeField] Vector2 speed = new Vector2(14f, 24f);
    [SerializeField] float pulseIntensity = 2.4f;
    [System.Serializable]
    sealed class Layer
    {
        public int count;
        public float width, brightness;
        public Vector2 lifetime;
        public Layer(int count, float width, Vector2 lifetime, float brightness)
        { this.count = count; this.width = width; this.lifetime = lifetime; this.brightness = brightness; }
    }
    [SerializeField] Layer contactSettings = new Layer(2, .42f, new Vector2(.04f, .07f), 3f);
    [SerializeField] Layer mainSettings = new Layer(12, .1875f, new Vector2(.12f, .28f), 1f);
    [SerializeField] Layer frontSettings = new Layer(28, .09375f, new Vector2(.12f, .28f), .85f);
    [SerializeField] Layer backSettings = new Layer(16, .03125f, new Vector2(.10f, .24f), .75f);
    [SerializeField] Layer emberSettings = new Layer(8, .025f, new Vector2(.25f, .45f), 1f);
    [SerializeField, HideInInspector] int rayShapeVersion;
    [Header("Fan variation")]
    [SerializeField, Range(0, 6)] float fanRotation = 6;
    [SerializeField] Vector2 fanSpread = new Vector2(.9f, 1.1f);
    [SerializeField, Range(0, .08f)] float energyVariation = .08f;
    [SerializeField, Range(0, .15f)] float lengthVariation = .15f;
    [Header("Toward camera (negative world Z)")]
    [SerializeField] int cameraRayCount = 3;
    [SerializeField] float cameraRayWidth = .25f;
    [SerializeField] Vector2 cameraRayLength = new Vector2(.55f, .70f);
    [SerializeField] Vector2 cameraRaySpeed = new Vector2(4, 6);
    readonly List<Vector4> shapeData = new List<Vector4>(64);
    ParticleSystem[] systems;
    PooledVFX lease;
    bool playing, paused;
    public int ParticleCount { get { int count = 0; foreach (var ps in systems) count += ps.particleCount; return count; } }

    internal Preset EmittedPreset => emittedPreset;
    internal ParryContactSnapshot Visual { get; private set; }
    internal ParticleSystem[] Systems => systems;
    void Awake()
    {
        systems = new[] { contact, mainRays, front, back, embers }; lease = GetComponent<PooledVFX>();
        original = new ParticlePresetState[systems.Length];
        for (int i = 0; i < systems.Length; i++) original[i] = new ParticlePresetState(systems[i]);
    }

    void ConfigurePreset()
    {
        for (int i = 0; i < systems.Length; i++) original[i].Restore(systems[i]);
        if (emittedPreset == Preset.BCompact) ConfigurePixel();
    }

    internal void Emit(Vector3 point, float direction, float groundY = -10f, Preset? selection = null, int sequence = 0, ParryContactSnapshot? visual = null)
    {
        Clear();
        emittedPreset = selection ?? preset;
        variationIndex = sequence;
        Visual = visual.GetValueOrDefault();
        ConfigurePreset();
        transform.SetPositionAndRotation(point, Quaternion.identity);
        transform.localScale = Vector3.one;
        if (ground) ground.SetPositionAndRotation(new Vector3(point.x, groundY, 0), Quaternion.identity);
        direction = direction < 0 ? -1 : 1;
        if (emittedPreset == Preset.BCompact) EmitPixel(direction, visual);
        else
        {
            bool varied = emittedPreset != Preset.Current;
            float rotation = Random.Range(varied ? -15f : -fanRotation, varied ? 15f : fanRotation) * Mathf.Deg2Rad;
            float spread = Random.Range(fanSpread.x, fanSpread.y);
            if (emittedPreset == Preset.A) spread = (sequence % 2 == 0) ? Random.Range(.55f, .75f) : Random.Range(1.15f, 1.35f);
            float energyRange = varied ? .15f : energyVariation;
            float energy = Random.Range(1 - energyRange, 1 + energyRange);
            EmitGroup(contact, contactSettings, direction, 0, 0, rotation, spread, energy);

            EmitGroup(mainRays, mainSettings, direction, 1, -.15f, rotation, spread, energy);
            EmitGroup(front, frontSettings, direction, 1, -.15f, rotation, spread, energy);
            EmitGroup(back, backSettings, direction, .72f, .15f, rotation, spread, energy);
            EmitGroup(embers, emberSettings, direction, .24f, 0, rotation, spread, energy);
        }
        foreach (var ps in systems) ps.Play(false);
        playing = true;
        ClashLightingRig.Pulse(gameObject.scene, point, emittedPreset == Preset.BCompact ? 5f : pulseIntensity, this);
    }

    void EmitGroup(ParticleSystem ps, Layer layer, float direction, float force, float z,
        float rotation, float spread, float energy)
    {
        bool ray = ps == mainRays || ps == front || ps == back;
        shapeData.Clear();
        int forwardCount = Mathf.CeilToInt(layer.count * .85f);
        for (int i = 0; i < layer.count; i++)
        {
            // Stratify each lobe independently; mirror X only, never near/far Z.
            int index = i < forwardCount ? i : i - forwardCount;
            int count = i < forwardCount ? forwardCount : layer.count - forwardCount;
            float angle = Mathf.Lerp(-.48f, 1f, (index + Random.value) / count);
            angle = (angle - .26f) * spread + .26f + rotation + (i < forwardCount ? 0 : Mathf.PI);
            float velocity = Random.Range(speed.x, speed.y) * force * energy;
            bool near = ps == mainRays && i < cameraRayCount;
            // Spread the three short fragments across the forward fan.
            if (near) angle = Mathf.Lerp(-.48f, 1f, (i + Random.value) / cameraRayCount) * spread + rotation;
            if (emittedPreset == Preset.B && ray && !near)
            {
                angle = ((i % 2 == 0 ? -16f : 45f) + Random.Range(-8f, 8f)) * Mathf.Deg2Rad + rotation;
                if (i >= forwardCount) angle += Mathf.PI;
            }
            float mask = Random.value;
            float length = Random.Range(1 - lengthVariation, 1 + lengthVariation);
            if (ray && !near)
            {
                if (emittedPreset == Preset.A) length = Random.Range(.65f, 1.30f);
                if (emittedPreset == Preset.B) length = (i % 2 == variationIndex % 2) ? Random.Range(.95f, 1.30f) : Random.Range(.45f, .80f);
                if (emittedPreset == Preset.C)
                {
                    if (ps == mainRays) length = ((i - cameraRayCount + variationIndex) % 3 == 0) ? Random.Range(1.10f, 1.30f) : Random.Range(.45f, .75f);
                    else if (ps == front) length = Random.Range(.40f, .75f);
                }
            }
            if (near) length *= Random.Range(cameraRayLength.x, cameraRayLength.y);
            // mask / body-to-head ratio / head source pixels / visible length.
            shapeData.Add(new Vector4(mask < .5f ? 0 : (mask < .8f ? 1 : 2),
                near ? .75f : 2f / 3f, near ? 4 : 3, length));
            ps.Emit(new ParticleSystem.EmitParams {
                position = transform.position + Vector3.forward * z,
                velocity = new Vector3(Mathf.Cos(angle) * direction * velocity, Mathf.Sin(angle) * velocity,
                    force == 0 ? 0 : (near ? -Random.Range(cameraRaySpeed.x, cameraRaySpeed.y) :
                        (ps == back ? Random.Range(2f, 4f) : Random.Range(-2f, 2f)))),
                startLifetime = Random.Range(layer.lifetime.x, layer.lifetime.y),
                startSize = near ? cameraRayWidth : layer.width,
                startColor = new Color(layer.brightness, layer.brightness * .8667f, layer.brightness * .5f, 1)
            }, 1);
        }
        if (ray) ps.SetCustomParticleData(shapeData, ParticleSystemCustomData.Custom1);
    }

    void Update()
    {
        if (!playing) return;
        if (CombatSession.Finished) { lease.Release(); return; }
        bool hold = GamePauseController.InputBlocked || Time.timeScale == 0f;
        if (hold != paused)
        {
            paused = hold;
            foreach (var ps in systems) { if (hold) ps.Pause(false); else ps.Play(false); }
        }
        if (hold) return;
        foreach (var ps in systems) if (ps.IsAlive(false)) return;
        lease.Release();
    }

    internal void Clear()
    {
        if (systems != null) foreach (var ps in systems) if (ps) ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        playing = paused = false;
        Visual = default;
        ClashLightingRig.Cancel(this);
    }
    void OnDisable() => Clear();
}
