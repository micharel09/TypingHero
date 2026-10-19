using UnityEngine;
using System.Collections.Generic;

// Explicit emission after placement; simulation uses real time but obeys reading/pause locks.
[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
public sealed class ParryClashBurst : MonoBehaviour
{
    public enum Preset { Current, A, B, C, BCompact }
    [SerializeField] internal Preset preset;
    [SerializeField, HideInInspector] int lengthSupportVersion;
    [SerializeField, HideInInspector] int compactVersion;
    [SerializeField] Material compactMaterial;
    [System.Serializable]
    sealed class CompactSettings
    {
        public Vector2Int longPixels = new Vector2Int(12, 16), shortPixels = new Vector2Int(6, 9), nearPixels = new Vector2Int(4, 6);
        public Vector2 rayLifetime = new Vector2(.14f, .22f), fragmentLifetime = new Vector2(.12f, .18f);
    }
    [SerializeField] CompactSettings compact = new CompactSettings();
    Material[] originalMaterials;
    bool[] originalSizeEnabled, originalSize3D;
    ParticleSystemRenderMode[] originalModes;
    ParticleSystemRenderSpace[] originalAlignments;
    float[] originalLength, originalVelocity;
    ParticleSystem.MinMaxCurve[] originalGravity;
    bool[] originalCollision, originalColor, originalLimit;
    float[] originalSimulationSpeed;
    struct Debris
    {
        public Vector3 origin, launch, coastOrigin, coastVelocity, impactPosition, impactVelocity;
        public float floor, flight, bounce, total, initialLength, initialSpeed, mask, launchDragX;
        public int group;
        public bool fast;
        public ParticleSystem.Particle particle;
    }
    readonly Debris[] debris = new Debris[22];
    readonly ParticleSystem.Particle[] debrisParticles = new ParticleSystem.Particle[10];
    Vector3 screenRight = Vector3.right, screenUp = Vector3.up;
    float debrisAge;
    int debrisCount;
    const float BrakeTime = .12f, LaunchDrag = 20f, CoastDrag = 2f, Gravity = 9.81f;
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
    internal ParticleSystem[] Systems => systems;
    void Awake()
    {
        systems = new[] { contact, mainRays, front, back, embers }; lease = GetComponent<PooledVFX>();
        originalGravity = new ParticleSystem.MinMaxCurve[5]; originalCollision = new bool[5];
        originalColor = new bool[5]; originalLimit = new bool[5]; originalSimulationSpeed = new float[5];
        originalMaterials = new Material[5]; originalSizeEnabled = new bool[5]; originalSize3D = new bool[5];
        originalModes = new ParticleSystemRenderMode[5]; originalAlignments = new ParticleSystemRenderSpace[5];
        originalLength = new float[5]; originalVelocity = new float[5];
        for (int i = 0; i < systems.Length; i++)
        {
            originalGravity[i] = systems[i].main.gravityModifier;
            originalSimulationSpeed[i] = systems[i].main.simulationSpeed;
            originalCollision[i] = systems[i].collision.enabled;
            originalColor[i] = systems[i].colorOverLifetime.enabled;
            originalLimit[i] = systems[i].limitVelocityOverLifetime.enabled;
            var renderer = systems[i].GetComponent<ParticleSystemRenderer>();
            originalMaterials[i] = renderer.sharedMaterial; originalModes[i] = renderer.renderMode;
            originalAlignments[i] = renderer.alignment;
            originalLength[i] = renderer.lengthScale; originalVelocity[i] = renderer.velocityScale;
            originalSizeEnabled[i] = systems[i].sizeOverLifetime.enabled; originalSize3D[i] = systems[i].main.startSize3D;
        }
    }

    void ConfigurePreset()
    {
        for (int i = 1; i < systems.Length; i++)
        {
            bool pixel = emittedPreset == Preset.BCompact;
            var ps = systems[i]; var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = pixel ? compactMaterial : originalMaterials[i];
            renderer.renderMode = pixel ? ParticleSystemRenderMode.Billboard : originalModes[i];
            renderer.alignment = pixel ? ParticleSystemRenderSpace.View : originalAlignments[i];
            renderer.lengthScale = pixel ? 1 : originalLength[i]; renderer.velocityScale = pixel ? 0 : originalVelocity[i];
            var size = ps.sizeOverLifetime; size.enabled = pixel ? false : originalSizeEnabled[i];
            var main = ps.main; main.startSize3D = pixel || originalSize3D[i];
            main.gravityModifier = pixel ? new ParticleSystem.MinMaxCurve(0) : originalGravity[i];
            // The same analytical path supplies position, velocity and visible orientation.
            main.simulationSpeed = pixel ? 0 : originalSimulationSpeed[i];
            var collision = ps.collision; collision.enabled = !pixel && originalCollision[i];
            var color = ps.colorOverLifetime; color.enabled = !pixel && originalColor[i];
            var limit = ps.limitVelocityOverLifetime; limit.enabled = !pixel && originalLimit[i];
        }
    }

    internal void Emit(Vector3 point, float direction, float groundY = -10f, Preset? selection = null, int sequence = 0)
    {
        Clear();
        emittedPreset = selection ?? preset;
        variationIndex = sequence;
        ConfigurePreset();
        transform.SetPositionAndRotation(point, Quaternion.identity);
        transform.localScale = Vector3.one;
        if (ground) ground.SetPositionAndRotation(new Vector3(point.x, groundY, 0), Quaternion.identity);
        direction = direction < 0 ? -1 : 1;
        bool varied = emittedPreset != Preset.Current;
        float rotation = Random.Range(varied ? -15f : -fanRotation, varied ? 15f : fanRotation) * Mathf.Deg2Rad;
        float spread = Random.Range(fanSpread.x, fanSpread.y);
        if (emittedPreset == Preset.A) spread = (sequence % 2 == 0) ? Random.Range(.55f, .75f) : Random.Range(1.15f, 1.35f);
        float energyRange = varied ? .15f : energyVariation;
        float energy = Random.Range(1 - energyRange, 1 + energyRange);
        EmitGroup(contact, contactSettings, direction, 0, 0, rotation, spread, energy);
        if (emittedPreset == Preset.BCompact) EmitCompact(direction, rotation, energy, groundY);
        else
        {
            EmitGroup(mainRays, mainSettings, direction, 1, -.15f, rotation, spread, energy);
            EmitGroup(front, frontSettings, direction, 1, -.15f, rotation, spread, energy);
            EmitGroup(back, backSettings, direction, .72f, .15f, rotation, spread, energy);
            EmitGroup(embers, emberSettings, direction, .24f, 0, rotation, spread, energy);
        }
        foreach (var ps in systems) ps.Play(false);
        playing = true;
        ClashLightingRig.Pulse(gameObject.scene, point, pulseIntensity, this);
    }

    void EmitCompact(float direction, float rotation, float energy, float groundY)
    {
        var camera = Camera.main;
        screenRight = camera ? camera.transform.right : Vector3.right;
        screenUp = camera ? camera.transform.up : Vector3.up;
        debrisCount = 22;
        for (int i = 0; i < debrisCount; i++)
        {
            // Keep the original ten draws, then add six reverse sparks per depth layer.
            // Reverse only X; mirror preserves Y and camera-relative Z.
            bool fast = i == 2 || i == 3;
            bool reverse = i >= 10;
            int group = reverse ? (i < 16 ? 2 : 3) : (i < 2 ? 1 : (i < 6 ? 2 : 3));
            int fanSlot = reverse ? i - 10 : i;
            int cluster = i % 2;
            float degrees = (reverse ? Mathf.Lerp(-12f, 36f, fanSlot / 11f) :
                (cluster == 0 ? -16 : 45) + (fanSlot / 2 % 3 - 1) * 10) + Random.Range(-2f, 2f);
            float angle = degrees * Mathf.Deg2Rad + rotation;
            var bounds = fast ? compact.nearPixels : (cluster == variationIndex % 2 ? compact.longPixels : compact.shortPixels);
            float velocity = Random.Range(speed.x, speed.y) * energy;
            float velocityX = reverse ? Random.Range(32f, 44f) * energy : velocity;
            var launch = new Vector3(Mathf.Cos(angle) * direction * (reverse ? -1 : 1) * velocityX, Mathf.Sin(angle) * velocity,
                group == 2 ? -Random.Range(4f, 6f) : (group == 3 ? Random.Range(2f, 4f) : Random.Range(-1f, 1f)));
            var d = new Debris { origin = transform.position, launch = launch,
                floor = groundY + 1f / 32, group = group, fast = fast, launchDragX = reverse ? 10f : LaunchDrag,
                initialLength = Random.Range(bounds.x, bounds.y + 1), mask = Random.Range(0, 3) };
            d.initialSpeed = ProjectedSpeed(launch);
            LaunchPath(d, BrakeTime, out d.coastOrigin, out d.coastVelocity);
            if (fast) d.total = Random.Range(compact.fragmentLifetime.x, compact.fragmentLifetime.y);
            else
            {
                // Solve once at emission, including contacts inside the braking phase.
                float lo = 0, hi = 10;
                for (int n = 0; n < 28; n++)
                {
                    float mid = (lo + hi) * .5f;
                    FlightPath(d, mid, out var pos, out var vel);
                    if (pos.y > d.floor) lo = mid; else hi = mid;
                }
                d.flight = (lo + hi) * .5f;
                FlightPath(d, d.flight, out d.impactPosition, out d.impactVelocity);
                d.impactPosition.y = d.floor;
                d.bounce = Mathf.Max(0, -2 * d.impactVelocity.y * .22f / Gravity);
                d.total = d.flight + d.bounce + .18f;
            }
            systems[group].Emit(new ParticleSystem.EmitParams {
                position = d.origin, velocity = launch, startLifetime = d.total + .1f,
                startSize3D = Vector3.one / 16, startColor = Color.white
            }, 1);
            debris[i] = d;
        }
        for (int group = 1; group <= 3; group++)
        {
            systems[group].GetParticles(debrisParticles);
            int slot = 0;
            for (int i = 0; i < debrisCount; i++)
                if (debris[i].group == group) debris[i].particle = debrisParticles[slot++];
        }
        AdvanceDebris(0);
    }

    float ProjectedSpeed(Vector3 velocity) => new Vector2(Vector3.Dot(velocity, screenRight), Vector3.Dot(velocity, screenUp)).magnitude;

    static void LaunchPath(Debris d, float t, out Vector3 position, out Vector3 velocity)
    {
        float decay = Mathf.Exp(-LaunchDrag * t), travel = (1 - decay) / LaunchDrag;
        velocity = d.launch * decay + Vector3.down * (Gravity * travel);
        position = d.origin + d.launch * travel + Vector3.down * (Gravity / LaunchDrag * (t - travel));
        float decayX = Mathf.Exp(-d.launchDragX * t);
        velocity.x = d.launch.x * decayX;
        position.x = d.origin.x + d.launch.x * (1 - decayX) / d.launchDragX;
    }

    static void FlightPath(Debris d, float t, out Vector3 position, out Vector3 velocity)
    {
        if (t <= BrakeTime) { LaunchPath(d, t, out position, out velocity); return; }
        t -= BrakeTime;
        float decay = Mathf.Exp(-CoastDrag * t), travel = (1 - decay) / CoastDrag;
        velocity = new Vector3(d.coastVelocity.x * decay, d.coastVelocity.y - Gravity * t, d.coastVelocity.z * decay);
        position = d.coastOrigin + new Vector3(d.coastVelocity.x * travel,
            d.coastVelocity.y * t - .5f * Gravity * t * t, d.coastVelocity.z * travel);
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

    // Evaluate absolute age, so skipped frames cannot add bounces or move resting sparks.
    internal void AdvanceDebris(float delta)
    {
        if (debrisCount == 0) return;
        debrisAge += delta;
        bool alive = false;
        for (int group = 1; group <= 3; group++)
        {
            int count = 0; shapeData.Clear();
            for (int i = 0; i < debrisCount; i++)
            {
                var d = debris[i];
                if (d.group != group || debrisAge >= d.total) continue;
                alive = true;
                float t = debrisAge;
                var p = d.particle;
                FlightPath(d, t, out var position, out var velocity);
                if (!d.fast && t >= d.flight)
                {
                    float bounceAge = Mathf.Clamp(t - d.flight, 0, d.bounce);
                    float travel = (1 - Mathf.Exp(-CoastDrag * bounceAge)) / CoastDrag;
                    position = d.impactPosition + new Vector3(d.impactVelocity.x * .55f * travel,
                        .5f * Gravity * bounceAge * (d.bounce - bounceAge), d.impactVelocity.z * .55f * travel);
                    velocity = t >= d.flight + d.bounce ? Vector3.zero : new Vector3(
                        d.impactVelocity.x * .55f * Mathf.Exp(-CoastDrag * bounceAge),
                        -d.impactVelocity.y * .22f - Gravity * bounceAge,
                        d.impactVelocity.z * .55f * Mathf.Exp(-CoastDrag * bounceAge));
                }
                // Ortho depth cues use integer source pixels, never camera changes or blur.
                float depth = Mathf.Clamp(1 - (position.z - d.origin.z) * .7f, .75f, 1.5f);
                float motion = Mathf.Clamp01(ProjectedSpeed(velocity) / d.initialSpeed);
                int length = Mathf.Max(1, Mathf.RoundToInt(d.initialLength * motion * depth));
                int width = Mathf.Max(1, Mathf.RoundToInt((d.fast ? 4 : 3) * motion * depth));
                if (!d.fast)
                {
                    int clearance = Mathf.Max(1, Mathf.FloorToInt((position.y - d.floor) * 32));
                    length = Mathf.Min(length, clearance); width = Mathf.Min(width, clearance);
                }
                p.position = position; p.velocity = velocity;
                p.startSize3D = new Vector3(width / 16f, length / 16f, 1);
                p.rotation = width == 1 && length == 1 ? 0 : 90 - Mathf.Atan2(Vector3.Dot(velocity, screenUp), Vector3.Dot(velocity, screenRight)) * Mathf.Rad2Deg;
                float heat = Mathf.Clamp01(t / (d.fast ? d.total : d.flight + d.bounce));
                Color tint = heat < .5f ? Color.Lerp(new Color(1, .8667f, .5f), new Color(.8f, .25f, .03f), heat * 2) :
                    Color.Lerp(new Color(.8f, .25f, .03f), new Color(.18f, .015f, 0), heat * 2 - 1);
                tint.a = d.fast ? 1 - t / d.total : 1 - Mathf.Clamp01((t - d.flight) / (d.bounce + .18f));
                p.startColor = tint; p.remainingLifetime = d.total - t + .1f;
                debrisParticles[count++] = p;
                shapeData.Add(new Vector4(d.mask, 2f / 3f, width, length));
            }
            systems[group].SetParticles(debrisParticles, count);
            systems[group].SetCustomParticleData(shapeData, ParticleSystemCustomData.Custom1);
        }
        if (!alive) debrisCount = 0;
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
        AdvanceDebris(Time.unscaledDeltaTime);
        if (emittedPreset == Preset.BCompact)
        {
            if (debrisCount > 0 || contact.IsAlive(false)) return;
        }
        else foreach (var ps in systems) if (ps.IsAlive(false)) return;
        lease.Release();
    }

    internal void Clear()
    {
        if (systems != null) foreach (var ps in systems) if (ps) ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        debrisAge = 0; debrisCount = 0;
        System.Array.Clear(debris, 0, debris.Length); System.Array.Clear(debrisParticles, 0, debrisParticles.Length);
        playing = paused = false;
        ClashLightingRig.Cancel(this);
    }
    void OnDisable() => Clear();
}
