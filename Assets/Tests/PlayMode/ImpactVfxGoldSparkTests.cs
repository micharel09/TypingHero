// Adapted from the reviewed GoldSparkTrialTests fixture (2026-10-29), now
// exercising the production AssetDatabase profile rather than QA Resources copies.
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
public sealed partial class ImpactVfxTests
{
    [Test, Category("Quick")]
    public void GoldSparksKeepApprovedPaletteCoolingAndRendererCapacity()
    {
        var profile = Parry;
        Assert.That(profile.hot, Is.EqualTo(Metal.hot));
        Assert.That(profile.warm, Is.EqualTo(new Color(1, .72f, .16f)));
        Assert.That(profile.cooled, Is.EqualTo(new Color(1, .5f, .08f)));
        Assert.That(profile.fadeDuration, Is.EqualTo(.08f));
        Assert.That(profile.lightDuration, Is.EqualTo(.22f));
        Assert.That(profile.flashes[0].lifetime, Is.EqualTo(.48f));
        Assert.That(profile.flashes[1].lifetime, Is.EqualTo(.22f));
        Assert.That(profile.flashes[0].tint, Is.EqualTo(new Color(1, .9f, .35f, .8f)));
        int[] counts = { 2, 3, 3, 9, 9, 4 };
        for (int i = 0; i < counts.Length; i++)
        {
            var group = profile.physics.groups[i];
            var baseline = Metal.physics.groups[i];
            Assert.That(group.count, Is.EqualTo(counts[i]));
            Assert.That(group.coolingDuration, Is.EqualTo(i >= 1 && i <= 4
                ? new Vector2(i <= 2 ? .63f : .56f, .77f) : baseline.coolingDuration));
            Assert.That(group.angles, Is.EqualTo(baseline.angles));
            Assert.That(group.speed, Is.EqualTo(baseline.speed));
            Assert.That(group.depthSpeed, Is.EqualTo(baseline.depthSpeed));
            Assert.That(group.physics, Is.EqualTo(baseline.physics));
        }
        var emitter = Emitter();
        foreach (float direction in new[] { -1f, 1f })
        {
            var burst = emitter.Play(profile, Context(direction));
            Assert.That(burst.ParticleCount, Is.EqualTo(32));
            var simulation = Simulation(burst);
            Assert.That(simulation.Count, Is.EqualTo(30));
            for (int step = 0; step <= 75; step++)
            {
                if (step > 0) burst.AdvanceDebris(1f / 60);
                float age = step / 60f;
                foreach (int channel in new[] { 1, 2, 3, 4 })
                {
                    var particles = Particles(burst, channel);
                    Assert.That(particles.Length, Is.LessThanOrEqualTo(burst.Systems[channel].main.maxParticles));
                    foreach (var particle in particles)
                    {
                        var state = simulation[(int)particle.randomSeed - 1];
                        float heat = Mathf.Clamp01(age / Mathf.Max(.00001f, state.coolingDuration));
                        var expected = heat < .5f ? Color.Lerp(profile.hot, profile.warm, heat * 2)
                            : Color.Lerp(profile.warm, profile.cooled, heat * 2 - 1);
                        expected.a = Mathf.Clamp01((state.total - age) / Mathf.Min(profile.fadeDuration, state.total));
                        var color = (Color32)expected;
                        Assert.That((int)particle.startColor.r, Is.EqualTo((int)color.r).Within(1));
                        Assert.That((int)particle.startColor.g, Is.EqualTo((int)color.g).Within(1));
                        Assert.That((int)particle.startColor.b, Is.EqualTo((int)color.b).Within(1));
                        Assert.That((int)particle.startColor.a, Is.EqualTo((int)color.a).Within(1));
                        Assert.That(state.coolingDuration, Is.LessThanOrEqualTo(state.total));
                        if (!state.small && state.coolingDuration > 0 && age >= state.coolingDuration)
                            Assert.That(particle.startSize3D.y, Is.LessThanOrEqualTo(1 / profile.pixelsPerUnit + .000001f));
                    }
                }
            }
            emitter.Clear();
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator GoldSparksPauseExpireAndReuseBoundedPool()
    {
        var profile = Parry;
        var emitter = Emitter();
        var first = emitter.Play(profile, Context());
        GamePauseController.SetTutorialReading(true); yield return null;
        var before = Particles(first, 2); Assert.That(before.Length, Is.EqualTo(12));
        yield return new WaitForSecondsRealtime(.2f);
        var held = Particles(first, 2); Assert.That(held.Length, Is.EqualTo(before.Length));
        for (int i = 0; i < held.Length; i++)
        {
            Assert.That(held[i].position, Is.EqualTo(before[i].position));
            Assert.That(held[i].startColor, Is.EqualTo(before[i].startColor));
        }
        Assert.That(emitter.Play(profile, Context()), Is.Null);
        GamePauseController.SetTutorialReading(false);
        for (int i = 0; i < 12; i++) emitter.Play(profile, Context());
        Assert.That(emitter.active.Count, Is.EqualTo(8));
        int count = 0;
        foreach (var item in emitter.active) count += item.GetComponent<ImpactVfxBurst>().ParticleCount;
        Assert.That(count, Is.EqualTo(256));
        emitter.enabled = false; Assert.That(emitter.active, Is.Empty);
        emitter.enabled = true;
        var reused = emitter.Play(profile, Context()); Assert.That(reused.ParticleCount, Is.EqualTo(32));
        yield return new WaitForSecondsRealtime(1.4f); Assert.That(emitter.active, Is.Empty);
        emitter.Play(Metal, Context()); emitter.Clear(); Assert.That(emitter.active, Is.Empty);
    }
}
#endif
