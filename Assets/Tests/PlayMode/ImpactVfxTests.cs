using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
public sealed partial class ImpactVfxTests
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

    ImpactVfxProfile Parry => UnityEditor.AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(
        "Assets/Art/VFX/ParryClash/ParryImpact.asset");

    [Test, Category("Quick")]
    public void HorizontalParryFlashKeepsDimensionsRotationAndEnvelopeAcrossSeedsAndPoolReuse()
    {
        var layer = Parry.flashes[1];
        Assert.That(layer.channel, Is.EqualTo(5));
        Assert.That(layer.lifetime, Is.EqualTo(.12f));
        foreach (var sample in new[] { new Vector2(0, 0), new Vector2(.02f, 1),
            new Vector2(.04f, 1), new Vector2(.08f, .5f), new Vector2(.12f, 0) })
            Assert.That(layer.colorOverLifetime.Evaluate(sample.x / layer.lifetime).a,
                Is.EqualTo(sample.y).Within(.0001f));
        var emitter = Emitter();
        ImpactVfxBurst previous = null;
        for (int seed = 0; seed < 24; seed++)
        foreach (float direction in new[] { -1f, 1f })
        {
            var context = Context(direction); context.seed = seed;
            var burst = emitter.Play(Parry, context);
            if (previous) Assert.That(burst, Is.SameAs(previous));
            var horizontal = Particles(burst, 5);
            Assert.That(horizontal.Length, Is.EqualTo(1));
            Assert.That(horizontal[0].position, Is.EqualTo(context.point));
            Assert.That(horizontal[0].rotation, Is.Zero);
            // Use the existing square core to infer the current camera pixel scale.
            float width = horizontal[0].startSize3D.x;
            Assert.That(width, Is.GreaterThanOrEqualTo(72 / Parry.pixelsPerUnit));
            Assert.That(horizontal[0].startSize3D.y, Is.EqualTo(width * 3 / 72).Within(.00001f));
            burst.Systems[5].Simulate(.121f, false, false, false);
            Assert.That(Particles(burst, 5), Is.Empty);
            Assert.That(Particles(burst, 0).Length, Is.EqualTo(1), "Existing flash retains its lifetime");
            emitter.Clear();
            Assert.That(Particles(burst, 5), Is.Empty);
            previous = burst;
        }
    }

    [Test, Category("Quick")]
    public void ParryImpactPreservesFlashAndSeededTrajectoriesWhileExtendingSparks()
    {
        Assert.That(Parry.physics, Is.Not.SameAs(Metal.physics));
        Assert.That(Parry.prefab, Is.Not.SameAs(Metal.prefab));
        Assert.That(Parry.lightDuration, Is.EqualTo(Metal.lightDuration));
        Assert.That(Parry.lightIntensity, Is.EqualTo(Metal.lightIntensity));
        var camera = Camera.main;
        if (!camera)
        {
            var cameraHost = new GameObject("parry size camera"); owned.Add(cameraHost);
            cameraHost.tag = "MainCamera"; camera = cameraHost.AddComponent<Camera>();
        }
        float previousSize = camera.orthographicSize;
        bool previousOrthographic = camera.orthographic; camera.orthographic = true;
        var previousTarget = camera.targetTexture;
        var texture = new RenderTexture(1280, 720, 0); owned.Add(texture);
        camera.targetTexture = texture;
        try
        {
            var emitter = Emitter();
            foreach (float size in new[] { 6f, 40f })
            foreach (float direction in new[] { -1f, 1f })
            {
                camera.orthographicSize = size;
                var context = Context(direction);
                var original = emitter.Play(Metal, context);
                var actual = emitter.Play(Parry, context);
                Assert.That(actual.Systems.Length, Is.EqualTo(6));
                Assert.That(Parry.flashes.Length, Is.EqualTo(2));
                var layer = Parry.flashes[0];
                Assert.That(layer.sizePixels, Is.EqualTo(40));
                Assert.That(layer.lifetime, Is.EqualTo(.48f));
                float expected = Mathf.Min(2.5f, Mathf.Max(1 / Parry.pixelsPerUnit,
                    size * 2 * camera.aspect / 1280) * 40);
                float minimum = Mathf.Min(2.5f, Mathf.Max(1 / Parry.pixelsPerUnit,
                    size * 2 * camera.aspect / 1280) * 38);
                Assert.That(Particles(actual, 0)[0].startSize, Is.InRange(minimum - .00001f, expected + .00001f));
                Assert.That(Particles(actual, 0)[0].position, Is.EqualTo(context.point));
                Assert.That(layer.colorOverLifetime.Evaluate(.99f).a, Is.EqualTo(1));
                actual.Systems[0].Simulate(.479f, false, false, false);
                Assert.That(Particles(actual, 0).Length, Is.EqualTo(1));
                actual.Systems[0].Simulate(.002f, false, false, false);
                Assert.That(Particles(actual, 0), Is.Empty);
                for (int step = 0; step <= 66; step++)
                {
                    var a = Simulation(actual); var b = Simulation(original);
                    Assert.That(a.Count, Is.EqualTo(b.Count));
                    for (int n = 0; n < a.Count; n++)
                    {
                        Assert.That(a[n].origin, Is.EqualTo(b[n].origin));
                        Assert.That(a[n].launch, Is.EqualTo(b[n].launch));
                        var x = a.Evaluate(n, step / 60f); var y = b.Evaluate(n, step / 60f);
                        if (x.alive && y.alive)
                        {
                            Assert.That(Vector3.Distance(x.position, y.position), Is.LessThan(.00002f));
                            Assert.That(Vector3.Distance(x.velocity, y.velocity), Is.LessThan(.00002f));
                        }
                    }
                    actual.AdvanceDebris(1f / 60); original.AdvanceDebris(1f / 60);
                }
                emitter.Clear();
            }
        }
        finally { camera.orthographicSize = previousSize; camera.targetTexture = previousTarget; camera.orthographic = previousOrthographic; }

    }

    [Test, Category("Quick")]
    public void ParryVariationIsRepeatableMirroredAndLeavesPhysicsAndGameplayRandomUnchanged()
    {
        var baseline = Object.Instantiate(Parry); owned.Add(baseline);
        var serialized = new UnityEditor.SerializedObject(baseline);
        serialized.FindProperty("flashSizeReductionPixels").intValue = 0;
        serialized.FindProperty("flashRotationDegrees").floatValue = 0;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        var emitter = Emitter();
        emitter.Play(baseline, Context()); emitter.Play(Parry, Context()); emitter.Clear();
        var savedRandom = Random.state;
        var rotations = new HashSet<float>(); var pixelSizes = new HashSet<float>();
        var launches = new HashSet<Vector3>();
        for (int seed = 0; seed < 24; seed++)
        {
            emitter.Clear(); var context = Context(); context.seed = seed;
            var old = emitter.Play(baseline, context);
            var actual = emitter.Play(Parry, context);
            var core = Particles(actual, 0)[0];
            float ratio = core.startSize / Particles(old, 0)[0].startSize;
            Assert.That(ratio, Is.InRange(.95f - .00001f, 1f + .00001f));
            Assert.That(core.startSize, Is.LessThanOrEqualTo(2.5f + .00001f));
            Assert.That(core.rotation, Is.InRange(-3.00001f, 3.00001f));
            Assert.That(core.position, Is.EqualTo(context.point));
            rotations.Add(core.rotation);
            float pixels = 40;
            Assert.That(Parry.TryVaryFlash(seed, 0, ref pixels, out _), Is.True);
            pixelSizes.Add(pixels);
            var a = Simulation(actual); var b = Simulation(old);
            Assert.That(a.Count, Is.EqualTo(22)); launches.Add(a[2].launch);
            for (int particle = 0; particle < a.Count; particle++)
            for (int step = 0; step <= 66; step++)
            {
                var x = a.Evaluate(particle, step / 60f); var y = b.Evaluate(particle, step / 60f);
                Assert.That(x.alive, Is.EqualTo(y.alive));
                Assert.That(x.position, Is.EqualTo(y.position));
                Assert.That(x.velocity, Is.EqualTo(y.velocity));
            }
            emitter.Clear();
            var repeated = emitter.Play(Parry, context);
            Assert.That(Particles(repeated, 0)[0].startSize, Is.EqualTo(core.startSize));
            Assert.That(Particles(repeated, 0)[0].rotation, Is.EqualTo(core.rotation).Within(.00001f));
            emitter.Clear(); context.direction = -1;
            var mirrored = emitter.Play(Parry, context);
            Assert.That(Particles(mirrored, 0)[0].startSize, Is.EqualTo(core.startSize));
            Assert.That(Particles(mirrored, 0)[0].rotation, Is.EqualTo(-core.rotation).Within(.00001f));
            emitter.Clear();
            var unvaried = emitter.Play(baseline, context);
            Assert.That(Particles(unvaried, 0)[0].rotation, Is.EqualTo(0).Within(.00001f));
            Assert.That(Random.state, Is.EqualTo(savedRandom));
        }
        // Camera caps may collapse size choices; angle must still vary on every impact.
        Assert.That(rotations.Count, Is.EqualTo(24));
        Assert.That(launches.Count, Is.EqualTo(24));
        CollectionAssert.AreEquivalent(new[] { 38f, 39f, 40f }, pixelSizes);
    }

    [Test, Category("Quick")]
    public void ParryBuilderMigratesDefaultAndPreservesCustomReferencesOnRepeat()
    {
        System.Type builder = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.GetType("CombatFeedbackBuild") != null) builder = assembly.GetType("CombatFeedbackBuild");
        var bind = builder.GetMethod("BindParryProfile", BindingFlags.Static | BindingFlags.NonPublic);
        var host = new GameObject("parry owner"); owned.Add(host); host.SetActive(false);
        var owner = host.AddComponent<ParrySparkVFX_Pooled>();
        var custom = Object.Instantiate(Metal); owned.Add(custom);
        var so = new UnityEditor.SerializedObject(owner);
        so.FindProperty("sparkPrefab").objectReferenceValue = Metal.prefab;
        foreach (var input in new[] { null, Metal, Parry, custom })
        {
            so.FindProperty("impactProfile").objectReferenceValue = input; so.ApplyModifiedPropertiesWithoutUndo();
            for (int i = 0; i < 2; i++)
            {
                bind.Invoke(null, new object[] { owner }); so.Update();
                Assert.That(so.FindProperty("impactProfile").objectReferenceValue, Is.SameAs(input == custom ? custom : Parry));
            }
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ParryImpactPauseResumeReturnsLease()
    {
        var emitter = Emitter(); var burst = emitter.Play(Parry, Context());
        GamePauseController.SetTutorialReading(true);
        yield return null;
        var before = Particles(burst, 0)[0];
        var horizontalBefore = Particles(burst, 5)[0];
        yield return new WaitForSecondsRealtime(.25f);
        Assert.That(Particles(burst, 5)[0].remainingLifetime, Is.EqualTo(horizontalBefore.remainingLifetime));
        Assert.That(Particles(burst, 0)[0].remainingLifetime, Is.EqualTo(before.remainingLifetime));
        Assert.That(Particles(burst, 0)[0].rotation, Is.EqualTo(before.rotation));
        Assert.That(Particles(burst, 0)[0].startSize, Is.EqualTo(before.startSize));
        GamePauseController.SetTutorialReading(false);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(Particles(burst, 5)[0].remainingLifetime, Is.EqualTo(horizontalBefore.remainingLifetime));
        Assert.That(Particles(burst, 0)[0].remainingLifetime, Is.EqualTo(before.remainingLifetime));
        Assert.That(Particles(burst, 0)[0].rotation, Is.EqualTo(before.rotation));
        Assert.That(Particles(burst, 0)[0].startSize, Is.EqualTo(before.startSize));
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(.50f);
        Assert.That(Particles(burst, 0), Is.Empty);
        Assert.That(Particles(burst, 3), Is.Empty);
        Assert.That(Particles(burst, 5), Is.Empty);
        yield return new WaitForSecondsRealtime(1.1f);
        Assert.That(emitter.active, Is.Empty);
        Assert.That(emitter.Play(Parry, Context(-1)), Is.SameAs(burst));
        Assert.That(burst.Systems[0].GetComponent<ParticleSystemRenderer>().flip.x, Is.EqualTo(1));
        emitter.Clear();
        Assert.That(emitter.Play(Parry, Context(1)), Is.SameAs(burst));
        Assert.That(burst.Systems[0].GetComponent<ParticleSystemRenderer>().flip.x, Is.Zero);
    }

    [Test, Category("Quick")]
    public void MissingParryPrefabBuilderCreatesSixBindingsAndPreservesCustomOnRepeat()
    {
        const string path = "Assets/Art/VFX/ParryClash/ParryImpact.prefab";
        const string backup = "Assets/Art/VFX/ParryClash/__ParryImpactBuilderFixture.prefab";
        Assert.That(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(backup), Is.Null);
        System.Type builder = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.GetType("ParryImpactAssets") != null) builder = assembly.GetType("ParryImpactAssets");
        var prepare = builder.GetMethod("Prepare", BindingFlags.Static | BindingFlags.NonPublic);
        var profile = Parry; var originalPrefab = profile.prefab;
        Assert.That(UnityEditor.AssetDatabase.MoveAsset(path, backup), Is.Empty);
        try
        {
            profile.prefab = null;
            prepare.Invoke(null, null);
            var generated = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(generated, Is.Not.Null);
            Assert.That(profile.prefab, Is.SameAs(generated));
            var so = new UnityEditor.SerializedObject(generated.GetComponent<ImpactVfxBurst>());
            var bindings = so.FindProperty("rendererBindings");
            Assert.That(bindings.arraySize, Is.EqualTo(6));
            Assert.That(so.FindProperty("directionalFlashChannel").intValue, Is.Zero);
            string[] names = { "PimenCore", "PimenFragment", "PimenRay", "PimenSpeck", "PimenRay", "HorizontalFlash" };
            for (int i = 0; i < 6; i++)
            {
                var binding = bindings.GetArrayElementAtIndex(i);
                Assert.That(binding.FindPropertyRelative("id").intValue, Is.EqualTo(i));
                Assert.That(binding.FindPropertyRelative("material").objectReferenceValue.name, Is.EqualTo(names[i]));
            }
            Assert.That(generated.transform.Find("ShockArc"), Is.Null);
            var before = System.IO.File.ReadAllBytes(path);
            for (int repeat = 0; repeat < 2; repeat++)
            {
                prepare.Invoke(null, null);
                Assert.That(profile.prefab, Is.SameAs(generated));
                CollectionAssert.AreEqual(before, System.IO.File.ReadAllBytes(path));
            }
            profile.prefab = originalPrefab;
            prepare.Invoke(null, null); prepare.Invoke(null, null);
            Assert.That(profile.prefab, Is.SameAs(originalPrefab), "Custom Inspector prefab must survive");
        }
        finally
        {
            profile.prefab = originalPrefab;
            UnityEditor.EditorUtility.SetDirty(profile);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(profile);
            UnityEditor.AssetDatabase.DeleteAsset(path);
            Assert.That(UnityEditor.AssetDatabase.MoveAsset(backup, path), Is.Empty);
        }
    }

    [Test, Category("Quick")]
    public void PimenCoreMirrorsAtContactAndHotStreaksRemainAfterFlash()
    {
        var emitter = Emitter();
        foreach (float side in new[] { -1f, 1f })
        for (int seed = 0; seed < 24; seed++)
        {
            var context = Context(side); context.seed = seed;
            var burst = emitter.Play(Parry, context);
            Assert.That(burst.ParticleCount, Is.EqualTo(24));
            var core = Particles(burst, 0)[0];
            Assert.That(core.position, Is.EqualTo(context.point));
            Assert.That(core.startLifetime, Is.EqualTo(.48f).Within(.000001f));
            Assert.That(burst.Systems[0].GetComponent<ParticleSystemRenderer>().flip.x, Is.EqualTo(side < 0 ? 1 : 0));
            var sheet = burst.Systems[0].textureSheetAnimation;
            Assert.That(sheet.numTilesX, Is.EqualTo(7));
            for (int frame = 0; frame < 6; frame++)
                Assert.That(Mathf.FloorToInt(sheet.frameOverTime.curve.Evaluate((frame + .5f) / 6) * 7), Is.EqualTo(frame));
            burst.AdvanceDebris(.49f);
            bool readable = false;
            foreach (int channel in new[] { 2, 4 })
            foreach (var p in Particles(burst, channel))
                readable |= p.startSize3D.y >= 1 / Parry.pixelsPerUnit && p.startColor.a > 200;
            Assert.That(readable, Is.True, "Visible physical tail after 480ms core, seed=" + seed);
            burst.Systems[0].Simulate(.481f, false, false, false);
            Assert.That(Particles(burst, 0), Is.Empty);
            emitter.Clear();
        }
    }

    [Test, Category("Quick")]
    public void PimenMaterialsUseOriginalTexturesAndSpecksKeepTheOldTrajectory()
    {
        var emitter = Emitter(); var burst = emitter.Play(Parry, Context());
        string[] names = { "PimenCore", "PimenFragment", "PimenRay", "PimenSpeck", "PimenRay" };
        int[] widths = { 336, 4, 3, 1, 3 }, heights = { 48, 3, 5, 1, 5 };
        for (int i = 0; i < names.Length; i++)
        {
            var material = burst.Systems[i].GetComponent<ParticleSystemRenderer>().sharedMaterial;
            Assert.That(material.name, Is.EqualTo(names[i]));
            Assert.That(material.shader.name, Is.EqualTo("Universal Render Pipeline/Particles/Unlit"));
            Assert.That(material.GetColor("_BaseColor"), Is.EqualTo(Color.white));
            var texture = material.GetTexture("_BaseMap");
            Assert.That(texture.width, Is.EqualTo(widths[i]));
            Assert.That(texture.height, Is.EqualTo(heights[i]));
            Assert.That(texture.filterMode, Is.EqualTo(FilterMode.Point));
            Assert.That(texture.wrapMode, Is.EqualTo(TextureWrapMode.Clamp));
            var importer = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(UnityEditor.AssetDatabase.GetAssetPath(texture));
            Assert.That(importer.mipmapEnabled, Is.False);
        }
        Assert.That(Particles(burst, 3).Length, Is.EqualTo(4));
        Assert.That(burst.Systems[3].main.maxParticles, Is.GreaterThanOrEqualTo(4));
        Assert.That(burst.transform.Find("ShockArc"), Is.Null);
        var baseline = Object.Instantiate(Parry.physics); owned.Add(baseline);
        baseline.groups[5].channel = 2;
        var old = new PhysicalParticleSimulation(40);
        var context = Context();
        old.Emit(baseline, context.point, context.direction, 0, .5f / Parry.pixelsPerUnit, context.seed);
        var current = Simulation(burst);
        Assert.That(current.Count, Is.EqualTo(22));
        for (int i = 0; i < 22; i++)
        for (int step = 0; step <= 66; step++)
        {
            var a = current.Evaluate(i, step / 60f); var b = old.Evaluate(i, step / 60f);
            Assert.That(a.alive, Is.EqualTo(b.alive));
            Assert.That(a.position, Is.EqualTo(b.position));
            Assert.That(a.velocity, Is.EqualTo(b.velocity));
        }
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
        first.lightEnabled = second.lightEnabled = false;
        var emitter = Emitter(); var a = emitter.Play(first, Context()); var b = emitter.Play(second, Context());
        first.lightEnabled = second.lightEnabled = true;
        var host = new GameObject("profile pulse overlap"); owned.Add(host); host.SetActive(false);
        var light = host.AddComponent<Light>();
        var rig = host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;
        typeof(ClashLightingRig).GetField("parryLight", Flags).SetValue(rig, light); host.SetActive(true);
        ResetPulseClock(rig, 10);
        rig.PulseAt(Context().point, first.Capture(), a, 10, false);
        rig.PulseAt(Context().point, second.Capture(), b, 10, false);
        Assert.That(light.color, Is.EqualTo(Color.red)); Assert.That(light.intensity, Is.EqualTo(4));
        first.lightDecay.MoveKey(1, new Keyframe(1, 1)); first.lightColor = Color.green;
        second.lightDecay.MoveKey(1, new Keyframe(1, 0)); second.lightColor = Color.magenta;
        rig.SampleAt(10.25, false);
        Assert.That(light.intensity, Is.EqualTo(3)); Assert.That(light.color, Is.EqualTo(Color.red));
        rig.CancelAt(a, 10.25, false);
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

    ImpactVfxProfile PulseProfile(bool lightOnly = false)
    {
        var profile = GenericProfile(lightOnly ? 0 : 1);
        if (lightOnly) profile.physics.groups = System.Array.Empty<PhysicalParticleProfile.Group>();
        foreach (var group in profile.physics.groups) group.lifetime = Vector2.one * .2f;
        foreach (var flash in profile.flashes) flash.lifetime = .2f;
        profile.lightEnabled = true; profile.lightIntensity = 2;
        profile.lightHold = 0; profile.lightDuration = 1;
        profile.lightDecay = AnimationCurve.Constant(0, 1, 1);
        return profile;
    }

    ClashLightingRig PulseRig(out Light light, int capacity = 8)
    {
        var host = new GameObject("pulse lifecycle rig"); owned.Add(host); host.SetActive(false);
        light = host.AddComponent<Light>();
        var rig = host.AddComponent<ClashLightingRig>();
        rig.profile = Object.Instantiate(LightingTestAssets.Definition.combat); owned.Add(rig.profile);
        rig.profile.overlapCapacity = capacity;
        typeof(ClashLightingRig).GetField("parryLight", Flags).SetValue(rig, light);
        host.SetActive(true); return rig;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LongPulseOutlivesParticlesPausesAndKeepsItsSnapshot()
    {
        var profile = PulseProfile(); PulseRig(out var light);
        var emitter = Emitter(); var burst = emitter.Play(profile, Context());
        yield return new WaitForSecondsRealtime(.35f);
        Assert.That(burst.ParticleCount, Is.Zero);
        Assert.That(emitter.active.Count, Is.EqualTo(1));
        Assert.That(light.intensity, Is.EqualTo(2));
        profile.lightDuration = .01f; profile.lightColor = Color.magenta;
        profile.lightDecay.MoveKey(0, new Keyframe(0, 0));
        GamePauseController.SetTutorialReading(true);
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(emitter.active.Count, Is.EqualTo(1));
        Assert.That(light.intensity, Is.EqualTo(2));
        Assert.That(light.color, Is.EqualTo(burst.appearance.lightColor));
        GamePauseController.SetTutorialReading(false);
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That(emitter.active.Count, Is.EqualTo(1));
        Assert.That(light.intensity, Is.EqualTo(2));
        yield return new WaitForSecondsRealtime(.65f);
        Assert.That(light.intensity, Is.Zero);
        Assert.That(emitter.active, Is.Empty);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LightOnlyPulseExpiresAndReusesItsLease()
    {
        var profile = PulseProfile(true); profile.lightDuration = .2f;
        PulseRig(out var light); var emitter = Emitter(); var first = emitter.Play(profile, Context());
        yield return null; yield return null;
        Assert.That(first.ParticleCount, Is.Zero);
        Assert.That(emitter.active.Count, Is.EqualTo(1));
        Assert.That(light.intensity, Is.EqualTo(2));
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(emitter.active, Is.Empty); Assert.That(light.intensity, Is.Zero);
        Assert.That(emitter.Play(profile, Context()), Is.SameAs(first));
        Assert.That(light.intensity, Is.EqualTo(2));
        emitter.Clear(); Assert.That(light.intensity, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator MissingDisabledRejectedAndEvictedPulsesDoNotRetainEmptyBursts()
    {
        var profile = PulseProfile(true); var emitter = Emitter();
        emitter.Play(profile, Context());
        yield return null; yield return null;
        Assert.That(emitter.active, Is.Empty, "No rig must not retain a light-only lease");
        var rig = PulseRig(out var light, 1);
        profile.lightEnabled = false; emitter.Play(profile, Context());
        yield return null; yield return null;
        Assert.That(emitter.active, Is.Empty);
        profile.lightEnabled = true;
        var stronger = Object.Instantiate(profile); owned.Add(stronger); stronger.lightIntensity = 4;
        var first = emitter.Play(profile, Context());
        var second = emitter.Play(stronger, Context());
        var rejected = emitter.Play(profile, Context());
        yield return null; yield return null;
        Assert.That(first.gameObject.activeSelf, Is.False);
        Assert.That(rejected.gameObject.activeSelf, Is.False);
        Assert.That(emitter.active.Count, Is.EqualTo(1));
        Assert.That(emitter.active[0], Is.SameAs(second.gameObject));
        Assert.That(light.intensity, Is.EqualTo(4));
        rig.enabled = false;
        yield return null; yield return null;
        Assert.That(emitter.active, Is.Empty); Assert.That(light.intensity, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LongPulseCapacityDisableAndClearCancelOwnersImmediately()
    {
        var profile = PulseProfile(true); profile.lightDuration = 10;
        PulseRig(out var light); var emitter = Emitter();
        var first = emitter.Play(profile, Context());
        for (int i = 0; i < 8; i++) emitter.Play(profile, Context());
        Assert.That(emitter.active.Count, Is.EqualTo(8));
        emitter.enabled = false;
        Assert.That(emitter.active, Is.Empty); Assert.That(light.intensity, Is.Zero);
        emitter.enabled = true; var reused = emitter.Play(profile, Context());
        yield return null; yield return null;
        Assert.That(emitter.active.Count, Is.EqualTo(1));
        Assert.That(light.intensity, Is.EqualTo(2));
        emitter.Clear(); Assert.That(light.intensity, Is.Zero);
        Assert.That(reused.ParticleCount, Is.Zero); Assert.That(first.ParticleCount, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SceneUnloadReclaimsLightOnlyLeaseAndRig()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.CreateScene("long impact pulse unload");
        var profile = PulseProfile(true); var rig = PulseRig(out var light);
        var emitter = Emitter();
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(rig.gameObject, scene);
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(emitter.gameObject, scene);
        var burst = emitter.Play(profile, Context());
        yield return null; yield return null;
        Assert.That(light.intensity, Is.EqualTo(2));
        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        Assert.That(burst == null, Is.True); Assert.That(rig == null, Is.True);
    }

    [Test, Category("Quick")]
    public void WaitingForLocalPulseAllocatesNothingAfterWarmup()
    {
        var profile = PulseProfile(true); PulseRig(out _);
        var emitter = Emitter(); var burst = emitter.Play(profile, Context());
        var update = (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), burst,
            typeof(ImpactVfxBurst).GetMethod("Update", Flags));
        update();
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100; i++) update();
        long allocated = System.GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.That(allocated, Is.Zero);
        Assert.That(emitter.active.Count, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void RigReenableRestoresLegacyColorAndAcceptsInactiveAuthoring()
    {
        var host = new GameObject("2D pulse color lifecycle"); owned.Add(host); host.SetActive(false);
        var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        var light = host.AddComponent(lightType);
        var color = lightType.GetProperty("color"); var intensity = lightType.GetProperty("intensity");
        var baseline = Color.yellow; color.SetValue(light, baseline);
        var rig = host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;
        typeof(ClashLightingRig).GetField("clashLight", Flags).SetValue(rig, light); host.SetActive(true);
        var profile = PulseProfile(true); profile.lightColor = Color.cyan;
        var emitter = Emitter();
        var legacy = Object.Instantiate(Metal.prefab); owned.Add(legacy); legacy.SetActive(true);
        var owner = legacy.GetComponent<ParryClashBurst>();
        for (int i = 0; i < 3; i++)
        {
            emitter.Play(profile, Context()); Assert.That(color.GetValue(light), Is.EqualTo(Color.cyan));
            rig.enabled = false;
            Assert.That(intensity.GetValue(light), Is.EqualTo(0f)); Assert.That(color.GetValue(light), Is.EqualTo(baseline));
            if (i == 1) color.SetValue(light, baseline = Color.green);
            rig.enabled = true;
            ClashLightingRig.Pulse(host.scene, Vector3.zero, CombatLightingProfile.Event.LegacyParry, owner);
            Assert.That((float)intensity.GetValue(light), Is.GreaterThan(0)); Assert.That(color.GetValue(light), Is.EqualTo(baseline));
            ClashLightingRig.Cancel(owner); emitter.Clear();
        }
    }
}

#endif
