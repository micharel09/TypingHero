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
            var expected = legacy.GetComponent<ParryClashBurst>();
            expected.Emit(context.point, context.direction, context.floorY, ParryClashBurst.Preset.BCompact,
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
        var rig = host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;
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
    ImpactVfxProfile GenericProfile(int flashCount)
    {
        var profile = Object.Instantiate(Metal); owned.Add(profile);
        profile.physics = Object.Instantiate(Metal.physics); owned.Add(profile.physics);
        profile.physics.groups = new[] { profile.physics.groups[0] };
        profile.physics.groups[0].channel = 77; profile.physics.groups[0].count = 2;
        profile.pixelsPerUnit = 32; profile.lightEnabled = false;
        var prefab = new GameObject("generic impact prefab"); owned.Add(prefab); prefab.SetActive(false);
        var burst = prefab.AddComponent<ImpactVfxBurst>();
        var serialized = new UnityEditor.SerializedObject(burst);
        var bindings = serialized.FindProperty("rendererBindings"); bindings.arraySize = flashCount + 1;
        profile.flashes = new ImpactVfxProfile.FlashLayer[flashCount];
        for (int i = 0; i <= flashCount; i++)
        {
            int id = i == 0 ? 77 : i * 10;
            var child = new GameObject("renderer " + id); child.transform.SetParent(prefab.transform, false);
            var ps = child.AddComponent<ParticleSystem>();
            var main = ps.main; main.playOnAwake = false; main.loop = false;
            main.useUnscaledTime = true; main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = ps.emission; emission.enabled = false;
            var binding = bindings.GetArrayElementAtIndex(i);
            binding.FindPropertyRelative("id").intValue = id;
            binding.FindPropertyRelative("particles").objectReferenceValue = ps;
            binding.FindPropertyRelative("material").objectReferenceValue =
                UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/ParryClash/" + (i == 0 ? "Compact.mat" : "PixelContact.mat"));
            if (i == 0) continue;
            var gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) });
            profile.flashes[i - 1] = new ImpactVfxProfile.FlashLayer {
                channel = id, count = i + 1, lifetime = .6f, tint = Color.cyan,
                sizePixels = 9, columns = 2, rows = 3, cycles = 2, startFrame = .25f,
                frameOverTime = AnimationCurve.Linear(0, .2f, 1, .8f), colorOverLifetime = gradient };
        }
        serialized.ApplyModifiedPropertiesWithoutUndo(); profile.prefab = prefab; return profile;
    }

    [TestCase(0), TestCase(1), TestCase(3), Category("Quick")]
    public void GenericPrefabUsesArbitraryBindingsFlashCountsAndSnapshots(int flashCount)
    {
        var profile = GenericProfile(flashCount); var emitter = Emitter();
        var burst = emitter.Play(profile, Context());
        Assert.That(burst.GetComponent<ParryClashBurst>(), Is.Null);
        Assert.That(burst.Systems.Length, Is.EqualTo(flashCount + 1));
        Assert.That(burst.Systems[0].particleCount, Is.EqualTo(2));
        for (int i = 1; i <= flashCount; i++)
        {
            var ps = burst.Systems[i]; var sample = Particles(burst, i);
            Assert.That(sample.Length, Is.EqualTo(i + 1));
            Assert.That(sample[0].startLifetime, Is.EqualTo(.6f));
            Assert.That(sample[0].startColor, Is.EqualTo((Color32)Color.cyan));
            Assert.That(sample[0].startSize, Is.EqualTo(9f / 32).Within(.000001f));
            var sheet = ps.textureSheetAnimation;
            Assert.That(sheet.numTilesX, Is.EqualTo(2)); Assert.That(sheet.numTilesY, Is.EqualTo(3));
            Assert.That(sheet.cycleCount, Is.EqualTo(2)); Assert.That(sheet.startFrame.constant, Is.EqualTo(.25f));
            Assert.That(sheet.frameOverTime.curve.Evaluate(.5f), Is.EqualTo(.5f).Within(.000001f));
            profile.flashes[i - 1].frameOverTime.MoveKey(1, new Keyframe(1, 0));
            profile.flashes[i - 1].colorOverLifetime.alphaKeys = new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(0, 1) };
            profile.flashes[i - 1].tint = Color.magenta; profile.flashes[i - 1].lifetime = 5;
            Assert.That(sheet.frameOverTime.curve.Evaluate(.5f), Is.EqualTo(.5f).Within(.000001f));
            Assert.That(ps.colorOverLifetime.color.gradient.Evaluate(.5f).a, Is.EqualTo(.5f).Within(.0001f));
        }
        profile.pixelsPerUnit = 8; profile.depthScale = 99;
        burst.AdvanceDebris(.05f);
        foreach (var particle in Particles(burst, 0)) Assert.That(particle.startSize3D.x, Is.EqualTo(1f / 32));
        var other = emitter.Play(Metal, Context());
        Assert.That(other.ParticleCount, Is.EqualTo(24));
        Assert.That(burst.Systems.Length, Is.EqualTo(flashCount + 1));
        emitter.Clear(); Assert.That(burst.ParticleCount, Is.Zero);
    }

    [Test, Category("Quick")]
    public void InvalidBindingAndPhysicalOverflowFailBeforeLeasing()
    {
        var profile = GenericProfile(1); var emitter = Emitter();
        profile.flashes[0].channel = 900;
        Assert.Throws<System.InvalidOperationException>(() => emitter.Play(profile, Context()));
        Assert.That(emitter.active, Is.Empty);
        profile.flashes[0].channel = 10; profile.physics.groups[0].count = 41;
        StringAssert.Contains("capacity", Assert.Throws<System.InvalidOperationException>(() => emitter.Play(profile, Context())).Message);
        Assert.That(emitter.active, Is.Empty);
        profile.physics.groups[0].count = 2; profile.flashes[0].channel = 77;
        StringAssert.Contains("share", Assert.Throws<System.InvalidOperationException>(() => emitter.Play(profile, Context())).Message);
    }

    [Test, Category("Quick")]
    public void ParryUsesProfilePrefabThroughPublicEmitter()
    {
        var profile = GenericProfile(1);
        var host = new GameObject("parry with generic profile"); owned.Add(host); host.SetActive(false);
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var fx = host.AddComponent<ParrySparkVFX_Pooled>();
        foreach (var field in new[] { ("parrySystem", (Object)parry), ("sparkPrefab", Metal.prefab),
            ("impactProfile", profile), ("playerSparkAnchor", host.transform) })
            typeof(ParrySparkVFX_Pooled).GetField(field.Item1, Flags).SetValue(fx, field.Item2);
        host.SetActive(true); parry.ParrySuccess();
        var emitter = host.GetComponent<ImpactVfxEmitter>(); Assert.That(emitter.active.Count, Is.EqualTo(1));
        var burst = emitter.active[0].GetComponent<ImpactVfxBurst>();
        Assert.That(burst.GetComponent<ParryClashBurst>(), Is.Null);
        Assert.That(burst.Systems.Length, Is.EqualTo(2));
        fx.enabled = false; Assert.That(emitter.active, Is.Empty);
    }

    [Test, Category("Quick")]
    public void PulseColorDecayAndOverlapUseIndependentProfileSnapshots()
    {
        var first = Object.Instantiate(Metal); owned.Add(first);
        first.lightIntensity = 4; first.lightColor = Color.red;
        first.lightHold = 0; first.lightDuration = 1;
        first.lightDecay = AnimationCurve.Linear(0, 1, 1, 0);
        var second = Object.Instantiate(first); owned.Add(second);
        second.lightIntensity = 2; second.lightColor = Color.blue;
        second.lightDecay = AnimationCurve.Constant(0, 1, 1);
        var host = new GameObject("profile pulse overlap"); owned.Add(host); host.SetActive(false);
        var light = host.AddComponent<Light>();
        var rig = host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;
        typeof(ClashLightingRig).GetField("parryLight", Flags).SetValue(rig, light); host.SetActive(true);
        var emitter = Emitter(); var a = emitter.Play(first, Context()); emitter.Play(second, Context());
        Assert.That(light.color, Is.EqualTo(Color.red)); Assert.That(light.intensity, Is.EqualTo(4));
        first.lightDecay.MoveKey(1, new Keyframe(1, 1)); first.lightColor = Color.green;
        second.lightDecay.MoveKey(1, new Keyframe(1, 0)); second.lightColor = Color.magenta;
        var pulses = (System.Array)typeof(ClashLightingRig).GetField("pulses", Flags).GetValue(rig);
        var pulse = pulses.GetValue(0); pulse.GetType().GetField("age").SetValue(pulse, .25f); pulses.SetValue(pulse, 0);
        typeof(ClashLightingRig).GetMethod("Render", Flags).Invoke(rig, null);
        Assert.That(light.intensity, Is.EqualTo(3)); Assert.That(light.color, Is.EqualTo(Color.red));
        a.GetComponent<PooledVFX>().Release();
        Assert.That(light.intensity, Is.EqualTo(2)); Assert.That(light.color, Is.EqualTo(Color.blue));
        emitter.Clear(); Assert.That(light.intensity, Is.Zero);
        first.lightEnabled = false; emitter.Play(first, Context()); Assert.That(light.intensity, Is.Zero);
    }

    [System.Serializable] sealed class BaselineParticle
    {
        public Vector3 position, velocity, size;
        public Color32 color;
        public float rotation, lifetime;
        public uint id;
    }
    [System.Serializable] sealed class BaselineFrame
    {
        public int side, seed, step, channel;
        public BaselineParticle[] particles;
        public Vector4[] shape;
    }
    [System.Serializable] sealed class BaselineTrace { public BaselineFrame[] frames; }

    // Independent pre-refactor trace: origin/main 325327418a6c, frozen snapshot 0c42fde68500.
    // Four 68-step traces (both sides, seeds 0/7), every renderer and every live particle.
    [Test, Category("Quick")]
    public void MetalClashMatchesFrozenPreRefactorTraceAtEveryStep()
    {
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Tests/PlayMode/MetalClashBaseline.bytes");
        using var bytes = new System.IO.MemoryStream(asset.bytes);
        using var zip = new System.IO.Compression.GZipStream(bytes, System.IO.Compression.CompressionMode.Decompress);
        using var reader = new System.IO.StreamReader(zip);
        var trace = JsonUtility.FromJson<BaselineTrace>(reader.ReadToEnd());
        Assert.That(trace.frames.Length, Is.EqualTo(2 * 2 * 68 * 5));
        var emitter = Emitter(); ImpactVfxBurst burst = null;
        var particles = new ParticleSystem.Particle[40]; var shapes = new List<Vector4>();
        foreach (var frame in trace.frames)
        {
            if (frame.step == 0 && frame.channel == 0)
            {
                emitter.Clear(); var context = Context(frame.side); context.seed = frame.seed;
                context.recipient = PhysicalParticleProfile.RecipientSnapshot.FromBounds(
                    new Bounds(new Vector3(-frame.side, 1, 0), new Vector3(1, 2, 1)));
                burst = emitter.Play(Metal, context);
            }
            var ps = burst.Systems[frame.channel];
            int count = ps.GetParticles(particles);
            string at = "side=" + frame.side + " seed=" + frame.seed + " step=" + frame.step + " channel=" + frame.channel;
            Assert.That(count, Is.EqualTo(frame.particles.Length), at);
            for (int i = 0; i < count; i++)
            {
                var actual = particles[i]; var expected = frame.particles[i];
                Assert.That(Vector3.Distance(actual.position, expected.position), Is.LessThan(.000001f), at);
                Assert.That(Vector3.Distance(actual.velocity, expected.velocity), Is.LessThan(.000001f), at);
                Assert.That(Vector3.Distance(actual.startSize3D, expected.size), Is.LessThan(.000001f), at);
                Assert.That(actual.startColor, Is.EqualTo(expected.color), at);
                Assert.That(actual.rotation, Is.EqualTo(expected.rotation).Within(.00001f), at);
                Assert.That(actual.remainingLifetime, Is.EqualTo(expected.lifetime).Within(.000001f), at);
                Assert.That(actual.randomSeed, Is.EqualTo(expected.id), at);
            }
            shapes.Clear();
            if (frame.channel != 0 && frame.channel != 3) ps.GetCustomParticleData(shapes, ParticleSystemCustomData.Custom1);
            Assert.That(shapes.Count, Is.EqualTo(frame.shape.Length), at);
            for (int i = 0; i < shapes.Count; i++) Assert.That(Vector4.Distance(shapes[i], frame.shape[i]), Is.LessThan(.000001f), at);
            if (frame.channel == 4)
            {
                burst.AdvanceDebris(1f / 60);
                burst.Systems[0].Simulate(1f / 60, false, false, false);
                burst.Systems[3].Simulate(1f / 60, false, false, false);
            }
        }
    }

    [Test, Category("Quick")]
    public void VersionedMigrationPreservesAuthoredProfileAndBindings()
    {
        var custom = Object.Instantiate(Metal); owned.Add(custom);
        custom.flashes = System.Array.Empty<ImpactVfxProfile.FlashLayer>();
        custom.lightColor = Color.green; custom.lightDecay = AnimationCurve.Constant(0, 1, .3f);
        var instance = Object.Instantiate(Metal.prefab); owned.Add(instance);
        var serialized = new UnityEditor.SerializedObject(instance.GetComponent<ParryClashBurst>());
        serialized.FindProperty("impactProfile").objectReferenceValue = custom;
        serialized.FindProperty("rendererBindings").GetArrayElementAtIndex(0).FindPropertyRelative("id").intValue = 901;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        System.Type builder = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.GetType("ParryClashAssets") != null) builder = assembly.GetType("ParryClashAssets");
        var migrate = builder.GetMethod("MigrateImpactAuthoring", BindingFlags.Static | BindingFlags.NonPublic);
        for (int i = 0; i < 2; i++)
        {
            Assert.That((bool)migrate.Invoke(null, new object[] { serialized }), Is.False);
            Assert.That(custom.flashes, Is.Empty);
            Assert.That(custom.lightColor, Is.EqualTo(Color.green));
            Assert.That(custom.lightDecay.Evaluate(.5f), Is.EqualTo(.3f));
            Assert.That(serialized.FindProperty("rendererBindings").GetArrayElementAtIndex(0).FindPropertyRelative("id").intValue, Is.EqualTo(901));
        }
    }

    [Test, Category("Quick")]
    public void ParryBuilderMigratesExistingPrefabProfileWithoutReplacingAuthoredReferences()
    {
        var custom = GenericProfile(1);
        var template = new UnityEditor.SerializedObject(custom.prefab.GetComponent<ImpactVfxBurst>());
        template.FindProperty("impactProfile").objectReferenceValue = custom; template.ApplyModifiedPropertiesWithoutUndo();
        var host = new GameObject("custom serialized parry"); owned.Add(host); host.SetActive(false);
        host.AddComponent<BoxCollider2D>(); var fx = host.AddComponent<ParrySparkVFX_Pooled>();
        typeof(ParrySparkVFX_Pooled).GetField("sparkPrefab", Flags).SetValue(fx, custom.prefab);
        System.Type builder = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.GetType("CombatFeedbackBuild") != null) builder = assembly.GetType("CombatFeedbackBuild");
        var configure = builder.GetMethod("Configure", BindingFlags.Static | BindingFlags.NonPublic);
        for (int i = 0; i < 2; i++)
        {
            configure.Invoke(null, new object[] { host, null, null, null });
            Assert.That(typeof(ParrySparkVFX_Pooled).GetField("sparkPrefab", Flags).GetValue(fx), Is.SameAs(custom.prefab));
            Assert.That(typeof(ParrySparkVFX_Pooled).GetField("impactProfile", Flags).GetValue(fx), Is.SameAs(custom));
        }
        typeof(ParrySparkVFX_Pooled).GetField("impactProfile", Flags).SetValue(fx, Metal);
        configure.Invoke(null, new object[] { host, null, null, null });
        Assert.That(typeof(ParrySparkVFX_Pooled).GetField("impactProfile", Flags).GetValue(fx), Is.SameAs(Metal));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ActiveGenericPrefabWithoutAuthoredLeaseExpiresAndReusesItsPoolItem()
    {
        var profile = GenericProfile(1); profile.prefab.SetActive(true);
        Assert.That(profile.prefab.GetComponent<PooledVFX>(), Is.Null);
        var emitter = Emitter(); var first = emitter.Play(profile, Context());
        profile.prefab.SetActive(false);
        Assert.That(first.GetComponent<PooledVFX>(), Is.Not.Null);
        yield return new WaitForSecondsRealtime(1.3f);
        Assert.That(emitter.active, Is.Empty);
        Assert.That(emitter.Play(profile, Context()), Is.SameAs(first));
        emitter.Clear();
    }

}

#endif
