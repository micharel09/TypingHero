using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
public sealed class ImpactVfxTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    readonly List<Object> owned = new();
    ImpactVfxProfile Metal => UnityEditor.AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(
        "Assets/Art/VFX/ParryClash/MetalClash.asset");
    ImpactVfxEmitter Emitter()
    {
        var host = new GameObject("standalone impact fixture"); owned.Add(host);
        return host.AddComponent<ImpactVfxEmitter>();
    }
    static ImpactVfxContext Context(float direction = 1) => new ImpactVfxContext {
        point = Vector3.up * 2, direction = direction, floorY = 0, seed = 7
    };
    static ParticleSystem.Particle[] Particles(ImpactVfxBurst burst, int channel = 2)
    {
        var result = new ParticleSystem.Particle[40];
        int count = burst.Systems[channel].GetParticles(result);
        System.Array.Resize(ref result, count); return result;
    }
    static PhysicalParticleSimulation Simulation(ImpactVfxBurst burst) =>
        (PhysicalParticleSimulation)typeof(ImpactVfxBurst).GetField("simulation", Flags).GetValue(burst);

    [UnityTearDown] public IEnumerator Cleanup()
    {
        Time.timeScale = 1; GamePauseController.SetTutorialReading(false);
        foreach (var item in owned) if (item) Object.Destroy(item);
        owned.Clear(); yield return null;
        Assert.That(Object.FindObjectsOfType<ImpactVfxBurst>(), Is.Empty);
    }

    [Test, Category("Quick")]
    public void MetalClashPlaysWithoutPlayerOrParryAndRetainsApprovedLimits()
    {
        var emitter = Emitter(); var burst = emitter.Play(Metal, Context());
        Assert.That(emitter.GetComponent<ParrySystem>(), Is.Null);
        Assert.That(burst.GetComponentInChildren<ParrySystem>(), Is.Null);
        Assert.That(burst.ParticleCount, Is.EqualTo(24));
        var simulation = Simulation(burst);
        for (int i = 0; i < simulation.Count; i++)
        {
            Assert.That(simulation[i].width, Is.EqualTo(1));
            Assert.That(simulation[i].total, Is.LessThanOrEqualTo(1.1f));
            Assert.That(simulation[i].physics.restDuration, Is.LessThanOrEqualTo(.4f));
        }
        emitter.Clear(); Assert.That(emitter.active, Is.Empty);
        Assert.That(burst.ParticleCount, Is.Zero);
    }

    [Test, Category("Quick")]
    public void ConcurrentProfilesReadTheirOwnColorPhysicsFadeAndSnapshot()
    {
        var custom = Object.Instantiate(Metal); owned.Add(custom);
        custom.physics = Object.Instantiate(Metal.physics); owned.Add(custom.physics);
        custom.hot = custom.warm = custom.cooled = Color.cyan;
        custom.fadeDuration = .4f; custom.lightIntensity = 1.2f; custom.lightRange = 7;
        foreach (var group in custom.physics.groups) { group.speed = Vector2.one * 3; group.lifetime = Vector2.one; }
        var emitter = Emitter();
        var first = emitter.Play(Metal, Context()); var second = emitter.Play(custom, Context());
        var firstState = Simulation(first)[0]; var secondState = Simulation(second)[0];
        Assert.That(secondState.launch.magnitude, Is.Not.EqualTo(firstState.launch.magnitude));
        Assert.That((Color)Particles(second)[0].startColor, Is.EqualTo((Color)(Color32)Color.cyan));
        Assert.That(Particles(first)[0].startColor, Is.Not.EqualTo(Particles(second)[0].startColor));
        custom.hot = custom.warm = custom.cooled = Color.magenta;
        custom.fadeDuration = .01f; custom.lightIntensity = 4; custom.lightRange = 2;
        foreach (var group in custom.physics.groups) group.physics.gravity = 999;
        second.AdvanceDebris(.1f); first.AdvanceDebris(.1f);
        Assert.That((Color)Particles(second)[0].startColor, Is.EqualTo((Color)(Color32)Color.cyan));
        Assert.That(Simulation(second)[0].physics.gravity, Is.EqualTo(secondState.physics.gravity));
        Assert.That(second.appearance.fadeDuration, Is.EqualTo(.4f));
        Assert.That(second.appearance.lightIntensity, Is.EqualTo(1.2f));
        Assert.That(second.appearance.lightRange, Is.EqualTo(7));
        Assert.That(first.appearance.fadeDuration, Is.EqualTo(.08f));
        var third = emitter.Play(custom, Context());
        Assert.That(Particles(third)[0].startColor, Is.EqualTo((Color32)Color.magenta));
        Assert.That(Simulation(third)[0].physics.gravity, Is.EqualTo(999));
    }

    [Test, Category("Quick")]
    public void SeedMirrorAndRecipientMatchLegacyRendererExactly()
    {
        var emitter = Emitter();
        foreach (float direction in new[] { -1f, 1f })
        {
            var context = Context(direction);
            context.recipient = PhysicalParticleProfile.RecipientSnapshot.FromBounds(
                new Bounds(new Vector3(-direction, 1, 0), new Vector3(1, 2, 1)));
            var actual = emitter.Play(Metal, context);
            var legacy = Object.Instantiate(Metal.prefab); owned.Add(legacy); legacy.SetActive(true);
            var expected = legacy.GetComponent<ImpactVfxBurst>();
            expected.Emit(context.point, context.direction, context.floorY, ImpactVfxBurst.Preset.BCompact,
                context.seed, null, context.recipient.center.x - context.point.x, context.recipient);
            for (int step = 0; step <= 66; step++)
            {
                for (int channel = 1; channel < 5; channel++)
                {
                    if (channel == 3) continue;
                    var a = Particles(actual, channel); var b = Particles(expected, channel);
                    Assert.That(a.Length, Is.EqualTo(b.Length));
                    for (int i = 0; i < a.Length; i++)
                    {
                        Assert.That(a[i].position, Is.EqualTo(b[i].position));
                        Assert.That(a[i].velocity, Is.EqualTo(b[i].velocity));
                        Assert.That(a[i].startSize3D, Is.EqualTo(b[i].startSize3D));
                        Assert.That(a[i].startColor, Is.EqualTo(b[i].startColor));
                    }
                }
                actual.AdvanceDebris(1f / 60); expected.AdvanceDebris(1f / 60);
            }
            emitter.Clear(); legacy.SetActive(false);
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator StandalonePoolPauseExpiryDisableAndReuseAreBounded()
    {
        var emitter = Emitter(); var first = emitter.Play(Metal, Context());
        GamePauseController.SetTutorialReading(true);
        var before = Particles(first);
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That(Particles(first)[0].position, Is.EqualTo(before[0].position));
        Assert.That(emitter.Play(Metal, Context()), Is.Null);
        GamePauseController.SetTutorialReading(false);
        for (int i = 0; i < 12; i++) emitter.Play(Metal, Context());
        Assert.That(emitter.active.Count, Is.EqualTo(8));
        Assert.That(Object.FindObjectsOfType<ImpactVfxBurst>().Length, Is.EqualTo(8));
        emitter.enabled = false;
        Assert.That(emitter.active, Is.Empty);
        Assert.That(Object.FindObjectsOfType<ImpactVfxBurst>(), Is.Empty);
        emitter.enabled = true;
        var reused = emitter.Play(Metal, Context()); Assert.That(reused.ParticleCount, Is.EqualTo(24));
        yield return new WaitForSecondsRealtime(1.3f);
        Assert.That(emitter.active, Is.Empty);
    }

    [Test, Category("Quick")]
    public void LocalPulseReadsProfileAndKeepsItsEmissionSnapshot()
    {
        var custom = Object.Instantiate(Metal); owned.Add(custom);
        custom.lightIntensity = 1.7f; custom.lightHold = .1f; custom.lightDuration = .5f;
        custom.lightRange = 7; custom.lightOffset = .25f;
        var host = new GameObject("standalone lighting fixture"); owned.Add(host); host.SetActive(false);
        var light = host.AddComponent<Light>();
        var rig = host.AddComponent<ClashLightingRig>();
        typeof(ClashLightingRig).GetField("parryLight", Flags).SetValue(rig, light);
        host.SetActive(true);
        var emitter = Emitter(); var context = Context(); context.unitsPerMeter = 2;
        emitter.Play(custom, context);
        Assert.That(light.intensity, Is.EqualTo(1.7f).Within(.00001f));
        Assert.That(light.range, Is.EqualTo(14));
        Assert.That(light.transform.position, Is.EqualTo(context.point + Vector3.back * .5f));
        custom.lightIntensity = 4; custom.lightHold = 0; custom.lightDuration = .1f; custom.lightRange = 1;
        var pulses = (System.Array)typeof(ClashLightingRig).GetField("pulses", Flags).GetValue(rig);
        var pulse = pulses.GetValue(0); pulse.GetType().GetField("age").SetValue(pulse, .3f);
        pulses.SetValue(pulse, 0); typeof(ClashLightingRig).GetMethod("Render", Flags).Invoke(rig, null);
        Assert.That(light.intensity, Is.EqualTo(.85f).Within(.00001f));
        Assert.That(light.range, Is.EqualTo(14));
        emitter.Clear(); Assert.That(light.intensity, Is.Zero);
    }

    [Test, Category("Quick")]
    public void BuilderOnlyFillsMissingImpactReferenceAcrossRepeatedCalls()
    {
        var custom = Object.Instantiate(Metal); owned.Add(custom); custom.fadeDuration = .37f;
        var instance = Object.Instantiate(Metal.prefab); owned.Add(instance);
        var serialized = new UnityEditor.SerializedObject(instance.GetComponent<ImpactVfxBurst>());
        serialized.FindProperty("impactProfile").objectReferenceValue = custom;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        System.Type builder = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.GetType("ParryClashAssets") != null) builder = assembly.GetType("ParryClashAssets");
        Assert.That(builder, Is.Not.Null);
        var bind = builder.GetMethod("BindPhysics", BindingFlags.Static | BindingFlags.NonPublic);
        for (int i = 0; i < 2; i++)
        {
            Assert.That((bool)bind.Invoke(null, new object[] { serialized }), Is.False);
            Assert.That(serialized.FindProperty("impactProfile").objectReferenceValue, Is.SameAs(custom));
            Assert.That(custom.fadeDuration, Is.EqualTo(.37f));
        }
        serialized.FindProperty("impactProfile").objectReferenceValue = null;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        Assert.That((bool)bind.Invoke(null, new object[] { serialized }), Is.True);
        Assert.That(serialized.FindProperty("impactProfile").objectReferenceValue, Is.SameAs(Metal));
    }

    [Test, Category("Quick")]
    public void StandaloneRendererUpdatesAllocateNothingAfterWarmup()
    {
        var emitter = Emitter(); var burst = emitter.Play(Metal, Context());
        burst.AdvanceDebris(.001f);
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100; i++) burst.AdvanceDebris(.001f);
        long allocated = System.GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.That(allocated, Is.Zero);
    }
}

#endif
