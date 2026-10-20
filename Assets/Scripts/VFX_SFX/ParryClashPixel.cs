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

    void ConfigurePixel()
    {
        for (int i = 0; i < systems.Length; i++)
        {
            var ps = systems[i]; bool contactLayer = i == 0 || i == 3;
            var main = ps.main; main.simulationSpeed = 1; main.startSize3D = !contactLayer;
            main.startRotation3D = false; main.gravityModifier = contactLayer ? 0 : 1;
            var size = ps.sizeOverLifetime; size.enabled = false;
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = contactLayer ? pixelContactMaterial : (i == 4 ? pixelEmberMaterial : pixelRayMaterial);
            renderer.renderMode = contactLayer || i == 4 ? ParticleSystemRenderMode.Billboard : ParticleSystemRenderMode.Stretch;
            renderer.alignment = ParticleSystemRenderSpace.View;
            renderer.lengthScale = 1; renderer.velocityScale = 0;
            renderer.sortingOrder = 0;
            renderer.SetActiveVertexStreams(new List<ParticleSystemVertexStream> {
                ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV });
            var color = ps.colorOverLifetime; color.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(contactLayer ? new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) } :
                new[] { new GradientColorKey(new Color(1, .94f, .72f), 0), new GradientColorKey(new Color(1, .45f, .07f), .4f),
                    new GradientColorKey(new Color(.35f, .035f, .002f), 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, i == 0 ? .35f : .45f), new GradientAlphaKey(0, 1) });
            color.color = gradient;
            var limit = ps.limitVelocityOverLifetime; limit.enabled = !contactLayer;
            limit.drag = i == 4 ? .6f : 1.5f; limit.multiplyDragByParticleSize = false; limit.multiplyDragByParticleVelocity = true;
            var collision = ps.collision; collision.enabled = !contactLayer;
            collision.type = ParticleSystemCollisionType.Planes; collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.SetPlane(0, ground); collision.bounce = .18f; collision.dampen = .6f;
            collision.lifetimeLoss = 0; collision.radiusScale = .25f;
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

    void EmitPixel(float direction, ParryContactSnapshot? visual)
    {
        var camera = Camera.main;
        pixelUnit = camera && camera.orthographic ? Mathf.Max(1f / 16, camera.orthographicSize * 2 * camera.aspect / 320) : 1f / 16;
        var snapshot = visual.GetValueOrDefault();
        float units = snapshot.valid ? snapshot.unitsPerMeter : 1.25f;
        PixelUnitsPerMeter = units;
        Vector3 slide = snapshot.valid ? snapshot.slide : new Vector3(direction, .6f, 0).normalized;
        Vector3 reflected = snapshot.valid ? snapshot.reflected : new Vector3(-direction, .35f, 0).normalized;
        pixelRandom = unchecked((uint)(variationIndex + 1) * 747796405u + 2891336453u);
        float energy = Sample(.85f, 1.15f);
        for (int group = 0; group < systems.Length; group++)
        {
            systems[group].useAutoRandomSeed = false;
            systems[group].randomSeed = pixelRandom + (uint)group + 1;
            var main = systems[group].main;
            main.gravityModifier = group == 0 || group == 3 ? 0 : units * 9.81f / Mathf.Max(.001f, Mathf.Abs(Physics.gravity.y));
        }
        // The fixed-pivot core and authored outer cels are two particles, not layered fog.
        contact.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = Vector3.zero,
            startSize = Mathf.Min(1, pixelUnit * 16), startLifetime = .2f, startColor = Color.white }, 1);
        back.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = Vector3.zero,
            startSize = pixelUnit * 16, startLifetime = .2f, startColor = Color.white }, 1);
        for (int i = 0; i < 22; i++)
        {
            bool ember = i >= 18;
            Vector3 axis = i < 14 ? slide : reflected;
            Vector3 velocity = Quaternion.AngleAxis(Sample(-25, 25), camera ? camera.transform.forward : Vector3.forward) * axis;
            velocity += (camera ? camera.transform.forward : Vector3.forward) * Sample(-.3f, .3f);
            velocity = velocity.normalized * Sample(ember ? 1.5f : 4, ember ? 3 : 7) * units * energy;
            if (ember) velocity.y = Mathf.Abs(velocity.y) * .35f;
            var ps = ember ? embers : (i < 14 ? mainRays : front);
            ps.Emit(new ParticleSystem.EmitParams { position = transform.position, velocity = velocity,
                startLifetime = Sample(ember ? .8f : .14f, ember ? 1.4f : .26f),
                startSize3D = new Vector3(pixelUnit, ember ? pixelUnit : Mathf.Floor(Sample(3, 9)) * pixelUnit, 1),
                startColor = Color.white, randomSeed = pixelRandom }, 1);
        }
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
