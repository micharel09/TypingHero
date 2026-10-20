using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public sealed partial class ParryClashTests
{
    [Test, Category("Quick")]
    public void ParryPointAndLegacySpotUseOneWinningPulseAndClearTogether()
    {
        var host = new GameObject("separate parry point"); host.SetActive(false); owned.Add(host);
        var spot = host.AddComponent<Light>();
        var child = new GameObject("Point"); child.transform.SetParent(host.transform);
        var point = child.AddComponent<Light>(); point.type = LightType.Point;
        var rig = host.AddComponent<ClashLightingRig>(); Set(rig, "combatLight", spot); Set(rig, "parryLight", point); host.SetActive(true);
        var compact = Burst(); compact.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        Assert.That(point.intensity, Is.EqualTo(5)); Assert.That(spot.intensity, Is.Zero);
        Assert.That(point.range, Is.EqualTo(3.75f).Within(.001f));
        var legacy = Burst(); legacy.Emit(Vector3.up, 1, 0, ParryClashBurst.Preset.B);
        Assert.That(point.intensity, Is.EqualTo(5)); Assert.That(spot.intensity, Is.Zero);
        compact.gameObject.SetActive(false);
        Assert.That(point.intensity, Is.Zero); Assert.That(spot.intensity, Is.EqualTo(2.4f).Within(.001f));
        host.SetActive(false); Assert.That(point.intensity + spot.intensity, Is.Zero);
    }
    [Test, Category("Quick")]
    public void PixelClashHasOneOriginTwentyFourParticlesAndIsolatedRandom()
    {
        var burst = Burst(); var saved = Random.state;
        var point = new Vector3(3, 2, .1f);
        burst.Emit(point, 1, 0, ParryClashBurst.Preset.BCompact, 7);
        Assert.That(Random.state, Is.EqualTo(saved), "presentation must not consume gameplay randomness");
        Assert.That(burst.ParticleCount, Is.EqualTo(24));
        int[] expected = { 1, 14, 4, 1, 4 };
        bool near = false, far = false;
        for (int g = 0; g < 5; g++)
        {
            var ps = burst.Systems[g]; Assert.That(ps.particleCount, Is.EqualTo(expected[g]));
            Assert.That(ps.main.simulationSpeed, Is.EqualTo(1));
            Assert.That(ps.GetComponent<ParticleSystemRenderer>().renderMode, Is.EqualTo(g == 1 || g == 2 ? ParticleSystemRenderMode.Stretch : ParticleSystemRenderMode.Billboard));
            var particles = new ParticleSystem.Particle[32]; int count = ps.GetParticles(particles);
            for (int i = 0; i < count; i++)
            {
                Assert.That(particles[i].position, Is.EqualTo(point));
                if (g == 0 || g == 3) continue;
                Assert.That(particles[i].startLifetime, Is.InRange(g == 4 ? .8f : .14f, g == 4 ? 1.4f : .26f));
                Assert.That(particles[i].startSize3D.x, Is.GreaterThanOrEqualTo(1f / 16));
                near |= particles[i].velocity.z < 0; far |= particles[i].velocity.z > 0;
            }
        }
        Assert.That(near && far, Is.True, "depth comes from 3D motion, not voxel meshes");
        burst.Clear(); Assert.That(burst.ParticleCount, Is.Zero);
    }

    [Test, Category("Quick")]
    public void PixelContactsHoldThenEndAndStockEmbersFallBounceAndExpire()
    {
        var burst = Burst(); burst.Emit(new Vector3(0, .4f, 0), 1, 0, ParryClashBurst.Preset.BCompact);
        var core = burst.Systems[0]; var outer = burst.Systems[3];
        Assert.That(outer.textureSheetAnimation.numTilesX, Is.EqualTo(6));
        Assert.That(outer.textureSheetAnimation.startFrame.constant, Is.EqualTo(1f / 6).Within(.00001f));
        var p = new ParticleSystem.Particle[4];
        core.Simulate(.069f, false, false, false); core.GetParticles(p);
        Assert.That(p[0].GetCurrentColor(core).a, Is.GreaterThanOrEqualTo(253));
        core.Simulate(.1311f, false, false, false); outer.Simulate(.2001f, false, false, false);
        Assert.That(core.particleCount + outer.particleCount, Is.Zero);
        var embers = burst.Systems[4]; bool fell = false, bounced = false;
        var previous = new Dictionary<uint, float>();
        for (int step = 0; step < 75; step++)
        {
            embers.Simulate(.02f, false, false, false); int count = embers.GetParticles(p);
            for (int i = 0; i < count; i++)
            {
                fell |= p[i].velocity.y < -.5f;
                if (previous.TryGetValue(p[i].randomSeed, out float y) && y < 0 && p[i].velocity.y > 0) bounced = true;
                previous[p[i].randomSeed] = p[i].velocity.y;
                Assert.That(p[i].position.y, Is.GreaterThanOrEqualTo(-.01f));
            }
        }
        Assert.That(fell && bounced, Is.True); Assert.That(embers.particleCount, Is.Zero);
    }

    [Test, Category("Quick")]
    public void PixelToLegacyRestoresMaterialRendererCollisionColorDragAndFlipbook()
    {
        var burst = Burst();
        var originals = new string[5];
        string State(ParticleSystem ps)
        {
            var r = ps.GetComponent<ParticleSystemRenderer>(); var streams = new List<ParticleSystemVertexStream>(); r.GetActiveVertexStreams(streams);
            return r.sharedMaterial.name + "/" + r.renderMode + "/" + r.alignment + "/" + r.sortingOrder + "/" + string.Join(",", streams) +
                "/" + ps.main.simulationSpeed + "/" + ps.main.gravityModifier.constant + "/" + ps.main.startSize3D +
                "/" + ps.collision.enabled + "/" + ps.collision.bounce.constant + "/" + ps.collision.dampen.constant +
                "/" + ps.collision.lifetimeLoss.constant + "/" + ps.collision.radiusScale +
                "/" + ps.limitVelocityOverLifetime.enabled + "/" + ps.limitVelocityOverLifetime.drag.constant +
                "/" + ps.limitVelocityOverLifetime.multiplyDragByParticleVelocity + "/" + ps.sizeOverLifetime.enabled +
                "/" + ps.colorOverLifetime.enabled + "/" + ps.textureSheetAnimation.enabled + "/" + ps.textureSheetAnimation.spriteCount;
        }
        for (int i = 0; i < 5; i++) originals[i] = State(burst.Systems[i]);
        for (int reuse = 0; reuse < 3; reuse++)
        {
            burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact, reuse);
            Assert.That(burst.Systems[0].GetComponent<ParticleSystemRenderer>().sharedMaterial.name, Is.EqualTo("PixelContact"));
            burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.B);
            for (int i = 0; i < 5; i++) Assert.That(State(burst.Systems[i]), Is.EqualTo(originals[i]));
        }
    }

    [Test, Category("Quick")]
    public void BladeContactUsesTransformedPixelsAndSnapshotPrecedesPoseMutation()
    {
#if UNITY_EDITOR
        ParryContactLibrary library = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var builder = assembly.GetType("ParryPixelAssets");
            if (builder != null) library = (ParryContactLibrary)builder.GetMethod("Contacts", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, null);
        }
        Assert.That(library, Is.Not.Null);
        var host = new GameObject("parry pose"); owned.Add(host);
        var player = host.AddComponent<SpriteRenderer>(); player.sprite = library.frames[0].sprite;
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false; parry.playerAnimator = host.AddComponent<Animator>();
        Set(parry, "contactLibrary", library);
        var enemy = new GameObject("attack pose"); owned.Add(enemy);
        var attacker = enemy.AddComponent<SpriteRenderer>();
        foreach (var f in library.frames) if (!f.defense) { attacker.sprite = f.sprite; break; }
        enemy.transform.position = new Vector3(3, .1f, 0); attacker.flipX = true;
        var expected = library.Capture(player, attacker, null);
        ParryContactSnapshot actual = default;
        parry.OnParrySuccess += ctx => { actual = ctx.contact; player.sprite = null; attacker.sprite = null; };
        typeof(ParrySystem).GetMethod("ForceSuccess", Flags).Invoke(parry, new object[] { enemy.transform });
        Assert.That(actual.valid, Is.True); Assert.That(actual.point, Is.EqualTo(expected.point));
        Assert.That(actual.slide.sqrMagnitude, Is.EqualTo(1).Within(.001f));
        ParryContactLibrary.Closest(Vector2.zero, Vector2.one, Vector2.up, Vector2.right, out float u, out float v);
        Assert.That(u, Is.EqualTo(.5f).Within(.001f)); Assert.That(v, Is.EqualTo(.5f).Within(.001f));
#endif
    }
}
