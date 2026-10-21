using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParryClashBurst
{
    uint pixelRandom;
    float pixelUnit;
    internal float PixelUnitsPerMeter { get; private set; } = 1.25f;
    float Sample(float min, float max)
    {
        pixelRandom ^= pixelRandom << 13; pixelRandom ^= pixelRandom >> 17; pixelRandom ^= pixelRandom << 5;
        return Mathf.Lerp(min, max, (pixelRandom & 0xffffff) / 16777216f);
    }

    static readonly List<ParticleSystemVertexStream> contactStreams = new() {
        ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV };
    static readonly List<ParticleSystemVertexStream> compactStreams = new() {
        ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV,
        ParticleSystemVertexStream.UV2, ParticleSystemVertexStream.Custom1XYZW };
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

    void ConfigurePixel()
    {
        for (int i = 0; i < systems.Length; i++)
        {
            var ps = systems[i]; bool contactLayer = i == 0 || i == 3;
            var main = ps.main;
            // Only the two approved Pimen contacts use Unity's clock. The remaining
            // particles retain their identities along an absolute-age analytical path.
            main.simulationSpeed = contactLayer ? 1 : 0;
            main.startSize3D = !contactLayer; main.startRotation3D = false; main.gravityModifier = 0;
            var size = ps.sizeOverLifetime; size.enabled = false;
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = contactLayer ? pixelContactMaterial : compactMaterial;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.View;
            renderer.lengthScale = 1; renderer.velocityScale = 0; renderer.sortingOrder = 0;
            renderer.SetActiveVertexStreams(contactLayer ? contactStreams : compactStreams);
            var color = ps.colorOverLifetime; color.enabled = contactLayer;
            if (contactLayer)
            {
                var gradient = new Gradient();
                gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
                    new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, i == 0 ? .35f : .45f), new GradientAlphaKey(0, 1) });
                color.color = gradient;
            }
            var limit = ps.limitVelocityOverLifetime; limit.enabled = false;
            var collision = ps.collision; collision.enabled = false;
            var sheet = ps.textureSheetAnimation; sheet.enabled = contactLayer;
            if (contactLayer)
            {
                sheet.mode = ParticleSystemAnimationMode.Grid;
                sheet.numTilesX = 6; sheet.numTilesY = 1; sheet.animation = ParticleSystemAnimationType.WholeSheet;
                sheet.startFrame = i == 0 ? 0 : 1f / 6;
                sheet.frameOverTime = i == 0 ? new ParticleSystem.MinMaxCurve(0) :
                    new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, 0, 1, 5f / 6));
                sheet.cycleCount = 1; sheet.uvChannelMask = UnityEngine.Rendering.UVChannelFlags.UV0;
            }
        }
    }

    void EmitPixel(float direction, float groundY, ParryContactSnapshot? visual)
    {
        var camera = Camera.main;
        pixelUnit = camera && camera.orthographic ? Mathf.Max(1f / 16, camera.orthographicSize * 2 * camera.aspect / 320) : 1f / 16;
        var snapshot = visual.GetValueOrDefault();
        PixelUnitsPerMeter = snapshot.valid ? snapshot.unitsPerMeter : 1.25f;
        pixelRandom = unchecked((uint)(variationIndex + 1) * 747796405u + 2891336453u);
        float rotation = Sample(-15f, 15f) * Mathf.Deg2Rad;
        float energy = Sample(.85f, 1.15f);
        for (int group = 0; group < systems.Length; group++)
        {
            systems[group].useAutoRandomSeed = false;
            systems[group].randomSeed = pixelRandom + (uint)group + 1;
        }
        // Exact c12c3de pivot, size, tint, flipbook, alpha and lifetime.
        contact.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = Vector3.zero,
            startSize = Mathf.Min(1, pixelUnit * 16), startLifetime = .2f, startColor = Color.white }, 1);
        back.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = Vector3.zero,
            startSize = pixelUnit * 16, startLifetime = .2f, startColor = Color.white }, 1);
        EmitCompact(direction, rotation, energy, groundY);
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
            int group = reverse ? (i < 16 ? 2 : 4) : (i < 2 ? 1 : (i < 6 ? 2 : 4));
            int fanSlot = reverse ? i - 10 : i;
            int cluster = i % 2;
            float degrees = (reverse ? Mathf.Lerp(-12f, 36f, fanSlot / 11f) :
                (cluster == 0 ? -16 : 45) + (fanSlot / 2 % 3 - 1) * 10) + Sample(-2f, 2f);
            float angle = degrees * Mathf.Deg2Rad + rotation;
            var bounds = fast ? compact.nearPixels : (cluster == variationIndex % 2 ? compact.longPixels : compact.shortPixels);
            float velocity = Sample(speed.x, speed.y) * energy;
            float velocityX = reverse ? Sample(32f, 44f) * energy : velocity;
            var launch = new Vector3(Mathf.Cos(angle) * direction * (reverse ? -1 : 1) * velocityX, Mathf.Sin(angle) * velocity,
                group == 2 ? -Sample(4f, 6f) : (group == 4 ? Sample(2f, 4f) : Sample(-1f, 1f)));
            var d = new Debris { origin = transform.position, launch = launch,
                floor = groundY + 1f / 32, group = group, fast = fast, launchDragX = reverse ? 10f : LaunchDrag,
                initialLength = Mathf.Floor(Sample(bounds.x, bounds.y + 1)), mask = Mathf.Floor(Sample(0, 3)) };
            d.initialSpeed = ProjectedSpeed(launch);
            LaunchPath(d, BrakeTime, out d.coastOrigin, out d.coastVelocity);
            if (fast) d.total = Sample(compact.fragmentLifetime.x, compact.fragmentLifetime.y);
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
        for (int group = 1; group < systems.Length; group++)
        {
            if (group == 3) continue; // Back now owns the authored Pimen outer cels.
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

    // Evaluate absolute age, so skipped frames cannot add bounces or move resting sparks.
    internal void AdvanceDebris(float delta)
    {
        if (debrisCount == 0) return;
        debrisAge += delta;
        bool alive = false;
        for (int group = 1; group < systems.Length; group++)
        {
            if (group == 3) continue;
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

    // Save values, not module handles (module structs retain a live ParticleSystem reference).
    sealed class ParticlePresetState
    {
        readonly Material material;
        readonly Mesh mesh;
        readonly ParticleSystemRenderMode mode;
        readonly ParticleSystemRenderSpace alignment;
        readonly List<ParticleSystemVertexStream> streams = new();
        readonly float length, velocity, simulation, radius;
        readonly int order, cycles, tilesX, tilesY;
        readonly bool size3D, rotation3D, size, collision, color, limit, sheet, autoSeed, dragSize, dragVelocity;
        readonly uint seed;
        readonly ParticleSystem.MinMaxCurve gravity, drag, bounce, dampen, loss, frame, startFrame;
        readonly ParticleSystem.MinMaxGradient gradient;
        readonly ParticleSystemCollisionType collisionType;
        readonly ParticleSystemCollisionMode collisionMode;
        readonly Transform plane;
        readonly ParticleSystemAnimationMode animationMode;
        readonly ParticleSystemAnimationType animation;
        readonly UnityEngine.Rendering.UVChannelFlags uv;
        internal ParticlePresetState(ParticleSystem ps)
        {
            var r = ps.GetComponent<ParticleSystemRenderer>(); material = r.sharedMaterial; mesh = r.mesh;
            mode = r.renderMode; alignment = r.alignment; r.GetActiveVertexStreams(streams);
            length = r.lengthScale; velocity = r.velocityScale; order = r.sortingOrder;
            var m = ps.main; size3D = m.startSize3D; rotation3D = m.startRotation3D;
            gravity = m.gravityModifier; simulation = m.simulationSpeed; size = ps.sizeOverLifetime.enabled;
            autoSeed = ps.useAutoRandomSeed; seed = ps.randomSeed;
            var c = ps.collision; collision = c.enabled; collisionType = c.type; collisionMode = c.mode;
            plane = c.GetPlane(0); bounce = c.bounce; dampen = c.dampen; loss = c.lifetimeLoss; radius = c.radiusScale;
            color = ps.colorOverLifetime.enabled; gradient = ps.colorOverLifetime.color;
            var l = ps.limitVelocityOverLifetime; limit = l.enabled; drag = l.drag;
            dragSize = l.multiplyDragByParticleSize; dragVelocity = l.multiplyDragByParticleVelocity;
            var t = ps.textureSheetAnimation; sheet = t.enabled; animationMode = t.mode;
            frame = t.frameOverTime; startFrame = t.startFrame; cycles = t.cycleCount; uv = t.uvChannelMask;
            tilesX = t.numTilesX; tilesY = t.numTilesY; animation = t.animation;
        }
        internal void Restore(ParticleSystem ps)
        {
            var r = ps.GetComponent<ParticleSystemRenderer>(); r.sharedMaterial = material; r.renderMode = mode;
            r.mesh = mesh; r.alignment = alignment; r.SetActiveVertexStreams(streams);
            r.lengthScale = length; r.velocityScale = velocity; r.sortingOrder = order;
            var m = ps.main; m.startSize3D = size3D; m.startRotation3D = rotation3D;
            m.gravityModifier = gravity; m.simulationSpeed = simulation;
            var s = ps.sizeOverLifetime; s.enabled = size;
            ps.randomSeed = seed; ps.useAutoRandomSeed = autoSeed;
            var c = ps.collision; c.enabled = collision; c.type = collisionType; c.mode = collisionMode;
            c.SetPlane(0, plane); c.bounce = bounce; c.dampen = dampen; c.lifetimeLoss = loss; c.radiusScale = radius;
            var co = ps.colorOverLifetime; co.enabled = color; co.color = gradient;
            var l = ps.limitVelocityOverLifetime; l.enabled = limit; l.drag = drag;
            l.multiplyDragByParticleSize = dragSize; l.multiplyDragByParticleVelocity = dragVelocity;
            var t = ps.textureSheetAnimation;
            t.mode = animationMode; t.numTilesX = tilesX; t.numTilesY = tilesY; t.animation = animation;
            t.frameOverTime = frame; t.startFrame = startFrame; t.cycleCount = cycles; t.uvChannelMask = uv; t.enabled = sheet;
        }
    }
}
