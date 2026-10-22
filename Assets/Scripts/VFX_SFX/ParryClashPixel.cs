using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParryClashBurst
{
    [SerializeField] PhysicalParticleProfile physicalProfile;
    float pixelUnit;
    internal float PixelUnitsPerMeter { get; private set; } = 1.25f;
    const int DebrisCapacity = 40;
    readonly PhysicalParticleSimulation simulation = new PhysicalParticleSimulation(DebrisCapacity);
    readonly ParticleSystem.Particle[] debrisParticles = new ParticleSystem.Particle[DebrisCapacity];
    readonly ParticleSystem.Particle[] identities = new ParticleSystem.Particle[DebrisCapacity];
    Vector3 screenRight = Vector3.right, screenUp = Vector3.up;
    float debrisAge;
    int debrisCount;
#if TYPINGHERO_AUTOMATION
    internal float DebrisAge => debrisAge;
#endif

    static readonly List<ParticleSystemVertexStream> contactStreams = new() {
        ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV };
    static readonly List<ParticleSystemVertexStream> compactStreams = new() {
        ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV,
        ParticleSystemVertexStream.UV2, ParticleSystemVertexStream.Custom1XYZW };
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

    void EmitPixel(float direction, float groundY, ParryContactSnapshot? visual, float playerDirection, PhysicalParticleProfile.RecipientSnapshot recipient)
    {
        var camera = Camera.main;
        pixelUnit = camera && camera.orthographic ? Mathf.Max(1f / 16, camera.orthographicSize * 2 * camera.aspect / 320) : 1f / 16;
        var snapshot = visual.GetValueOrDefault();
        PixelUnitsPerMeter = snapshot.valid ? snapshot.unitsPerMeter : 1.25f;
        uint seed = unchecked((uint)(variationIndex + 1) * 747796405u + 2891336453u);
        for (int group = 0; group < systems.Length; group++)
        {
            systems[group].useAutoRandomSeed = false;
            systems[group].randomSeed = seed + (uint)group + 1;
        }
        // Exact c12c3de pivot, size, tint, flipbook, alpha and lifetime.
        contact.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = Vector3.zero,
            startSize = Mathf.Min(1, pixelUnit * 16), startLifetime = .2f, startColor = Color.white }, 1);
        back.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = Vector3.zero,
            startSize = pixelUnit * 16, startLifetime = .2f, startColor = Color.white }, 1);
        EmitPhysical(direction, playerDirection, groundY, recipient);
    }

    void EmitPhysical(float direction, float playerDirection, float groundY, PhysicalParticleProfile.RecipientSnapshot recipient)
    {
        var camera = Camera.main;
        screenRight = camera ? camera.transform.right : Vector3.right;
        screenUp = camera ? camera.transform.up : Vector3.up;
        simulation.Emit(physicalProfile, transform.position, direction, playerDirection, groundY + 1f / 32, variationIndex, recipient);
        debrisCount = simulation.Count;
        for (int i = 0; i < debrisCount; i++)
        {
            var d = simulation[i];
            // Stable local identities: pool reuse and deterministic seeds never consume gameplay Random.
            identities[i] = new ParticleSystem.Particle {
                randomSeed = (uint)i + 1, startLifetime = d.total + .1f,
                remainingLifetime = d.total + .1f, startColor = Color.white
            };
        }
        AdvanceDebris(0);
    }

    float ProjectedSpeed(Vector3 velocity) => new Vector2(Vector3.Dot(velocity, screenRight), Vector3.Dot(velocity, screenUp)).magnitude;

    internal void AdvanceDebris(float delta)
    {
        if (debrisCount == 0) return;
        debrisAge += delta;
        bool alive = false;
        for (int group = 1; group < systems.Length; group++)
        {
            if (group == 3) continue; // The approved Pimen outer contact retains this renderer.
            int count = 0; shapeData.Clear();
            for (int i = 0; i < simulation.Count; i++)
            {
                var d = simulation[i];
                if (d.channel != group) continue;
                var sample = simulation.Evaluate(i, debrisAge);
                if (!sample.alive) continue;
                alive = true;
                var p = identities[i];
                var position = sample.position; var velocity = sample.velocity;
                float motion = Mathf.Clamp01(ProjectedSpeed(velocity) / Mathf.Max(.00001f, ProjectedSpeed(d.launch)));
                float coolingTime = d.coolingDuration > 0 ? d.coolingDuration : Mathf.Min(d.total, d.flight + d.bounce);
                float heat = Mathf.Clamp01(debrisAge / Mathf.Max(.00001f, coolingTime));
                float depth = d.pixelScaleMode == PhysicalParticleProfile.PixelScaleMode.VelocityOnly ? 1 :
                    Mathf.Clamp(1 - (position.z - d.origin.z) * .7f, .75f, 1.5f);
                int length = Mathf.Max(1, Mathf.RoundToInt(d.length * motion * depth));
                int width = Mathf.Max(1, Mathf.RoundToInt(d.width * motion * depth));
                if (d.coolingDuration > 0)
                    length = Mathf.Min(length, Mathf.Max(1, Mathf.RoundToInt(Mathf.Lerp(d.length, 1, heat))));
                if (d.small) { length = Mathf.Clamp(Mathf.RoundToInt(d.length), 1, 2); width = length; }
                if (d.physics.collideWithFloor)
                {
                    int clearance = Mathf.Max(1, Mathf.FloorToInt((position.y - d.floor) * 32));
                    length = Mathf.Min(length, clearance); width = Mathf.Min(width, clearance);
                }
                p.position = position; p.velocity = velocity;
                p.startSize3D = new Vector3(width / 16f, length / 16f, 1);
                p.rotation = d.small || (width == 1 && length == 1) ? 0 :
                    90 - Mathf.Atan2(Vector3.Dot(velocity, screenUp), Vector3.Dot(velocity, screenRight)) * Mathf.Rad2Deg;
                // Cooling shortens the streak, but the remaining ember must stay
                // readable through its physical fall and bounce on the dark floor.
                Color cooled = d.coolingDuration > 0 ? new Color(.65f, .2f, .025f) : new Color(.18f, .015f, 0);
                Color tint = heat < .5f ? Color.Lerp(new Color(1, .8667f, .5f), new Color(.8f, .25f, .03f), heat * 2) :
                    Color.Lerp(new Color(.8f, .25f, .03f), cooled, heat * 2 - 1);
                tint.a = d.coolingDuration > 0 ? Mathf.Clamp01((d.total - debrisAge) / Mathf.Min(.08f, d.total)) :
                    d.physics.restDuration > 0 && !float.IsPositiveInfinity(d.flight) ?
                    1 - Mathf.Clamp01((debrisAge - d.flight) / (d.bounce + d.physics.restDuration)) : 1 - debrisAge / d.total;
                p.startColor = tint; p.remainingLifetime = d.total - debrisAge + .1f;
                debrisParticles[count++] = p;
                shapeData.Add(new Vector4(d.mask, 2f / 3f, width, length));
            }
            systems[group].SetParticles(debrisParticles, count);
            systems[group].SetCustomParticleData(shapeData, ParticleSystemCustomData.Custom1);
        }
        if (!alive) debrisCount = 0;
    }

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
