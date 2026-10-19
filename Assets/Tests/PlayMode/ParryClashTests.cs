using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TestTools;

public sealed class ParryClashTests
{
    readonly List<GameObject> owned = new();
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Set(object o, string field, object value) => o.GetType().GetField(field, Flags).SetValue(o, value);
    GameObject Prefab()
    {
#if UNITY_EDITOR
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType("ParryClashAssets");
            if (type != null) { type.GetMethod("Prepare", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null); break; }
        }
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/ParryClash/ParryClash.prefab");
#else
        return null;
#endif
    }
    ParryClashBurst Burst()
    {
        var go = Object.Instantiate(Prefab()); owned.Add(go); go.SetActive(true);
        return go.GetComponent<ParryClashBurst>();
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        Time.timeScale = 1; GamePauseController.SetTutorialReading(false);
        foreach (var go in owned) if (go) Object.Destroy(go);
        owned.Clear(); yield return null;
        Assert.That(Object.FindObjectsOfType<ParryClashBurst>(), Is.Empty);
    }
    [Test, Category("Quick")]
    public void ExplicitEmissionPlacesAllGroupsAndMirrorsForceWithDepth()
    {
        var burst = Burst(); Assert.That(burst.ParticleCount, Is.Zero);
        foreach (float direction in new[] { -1f, 1f })
        {
            burst.Emit(new Vector3(3, 2, 0), direction);
            Assert.That(burst.ParticleCount, Is.EqualTo(66));
            Assert.That(burst.GetComponentsInChildren<ParticleSystem>().Length, Is.EqualTo(5));
            foreach (var expected in new[] { ("Contact", 2, .42f), ("MainRays", 12, .1875f),
                ("Front", 28, .09375f), ("Back", 16, .03125f), ("Embers", 8, .025f) })
            {
                var ps = burst.transform.Find(expected.Item1).GetComponent<ParticleSystem>();
                Assert.That(ps.particleCount, Is.EqualTo(expected.Item2));
                var sample = new ParticleSystem.Particle[64]; ps.GetParticles(sample);
                Assert.That(sample[expected.Item1 == "MainRays" ? 3 : 0].startSize, Is.EqualTo(expected.Item3));
            }
            int forward = 0, depth = 0;
            var particles = new ParticleSystem.Particle[64];
            foreach (var ps in burst.GetComponentsInChildren<ParticleSystem>())
            {
                Assert.That(ps.main.useUnscaledTime, Is.True); Assert.That(ps.main.playOnAwake, Is.False);
                Assert.That(ps.emission.enabled, Is.False);
                int count = ps.GetParticles(particles);
                for (int i = 0; i < count; i++)
                {
                    var p = particles[i];
                    Assert.That(p.position.x, Is.EqualTo(3)); Assert.That(p.position.y, Is.EqualTo(2));
                    if (p.velocity.x * direction > 0) forward++;
                    if (Mathf.Abs(p.velocity.z) > .001f) depth++;
                }
            }
            Assert.That(forward, Is.InRange(51, 58)); Assert.That(depth, Is.GreaterThan(50));
        }
    }
    [Test, Category("Quick")]
    public void ShapesStayStableAndMirrorPreservesCameraDepthOnReuse()
    {
        var burst = Burst();
        var rays = burst.transform.Find("MainRays").GetComponent<ParticleSystem>();
        var back = burst.transform.Find("Back").GetComponent<ParticleSystem>();
        var data = new List<Vector4>(); var later = new List<Vector4>();
        var particles = new ParticleSystem.Particle[64];
        var randomState = Random.state;
        try
        {
            for (int seed = 0; seed < 32; seed++)
            {
                Random.InitState(seed);
                float rotation = Random.Range(-6f, 6f) * Mathf.Deg2Rad;
                float spread = Random.Range(.9f, 1.1f);
                float energy = Random.Range(.92f, 1.08f);
                Random.InitState(seed); burst.Emit(Vector3.zero, 1);
                rays.GetCustomParticleData(data, ParticleSystemCustomData.Custom1);
                Assert.That(data.Count, Is.EqualTo(12));
                rays.GetParticles(particles);
                var original = (ParticleSystem.Particle[])particles.Clone();
                int near = 0;
                for (int i = 0; i < 12; i++)
                {
                    var p = particles[i]; var d = data[i];
                    Assert.That(d.x, Is.InRange(0, 2));
                    bool camera = p.startSize == .25f;
                    if (camera) { near++; Assert.That(p.velocity.z, Is.InRange(-6f, -4f)); }
                    Assert.That(d.y, Is.EqualTo(camera ? .75f : 2f / 3f));
                    Assert.That(d.z, Is.EqualTo(camera ? 4 : 3));
                    Assert.That(d.w, Is.InRange(camera ? .85f * .55f : .85f, camera ? 1.15f * .7f : 1.15f));
                    Assert.That(new Vector2(p.velocity.x, p.velocity.y).magnitude, Is.InRange(14f * energy - .00001f, 24f * energy + .00001f));
                    float angle = Mathf.Atan2(p.velocity.y, p.velocity.x);
                    if (i == 11 && angle < 0) angle += 2 * Mathf.PI;
                    float slot = camera ? i / 3f : (i < 11 ? i / 11f : 0);
                    float next = camera ? (i + 1) / 3f : (i < 11 ? (i + 1) / 11f : 1);
                    float low = Mathf.Lerp(-.48f, 1, slot) * spread + rotation;
                    float high = Mathf.Lerp(-.48f, 1, next) * spread + rotation;
                    if (!camera) { low += .26f * (1 - spread); high += .26f * (1 - spread); }
                    if (i == 11) { low += Mathf.PI; high += Mathf.PI; }
                    Assert.That(angle, Is.InRange(low - .00001f, high + .00001f));
                }
                Assert.That(near, Is.EqualTo(3));
                var shapesBySeed = new Dictionary<uint, Vector4>();
                for (int i = 0; i < 12; i++) shapesBySeed.Add(original[i].randomSeed, data[i]);
                for (int step = 0; step < 10; step++)
                {
                    rays.Simulate(.03f, false, false);
                    int count = rays.GetParticles(particles);
                    rays.GetCustomParticleData(later, ParticleSystemCustomData.Custom1);
                    Assert.That(later.Count, Is.EqualTo(count));
                    for (int i = 0; i < count; i++)
                        Assert.That(later[i], Is.EqualTo(shapesBySeed[particles[i].randomSeed]),
                            "mask/ratios/length must survive shrink and particle compaction until death");
                }
                Random.InitState(seed); burst.Emit(Vector3.zero, -1);
                rays.GetParticles(particles);
                for (int i = 0; i < 12; i++)
                {
                    Assert.That(particles[i].velocity.x, Is.EqualTo(-original[i].velocity.x));
                    Assert.That(particles[i].velocity.y, Is.EqualTo(original[i].velocity.y));
                    Assert.That(particles[i].velocity.z, Is.EqualTo(original[i].velocity.z));
                }
                back.GetParticles(particles);
                for (int i = 0; i < back.particleCount; i++) Assert.That(particles[i].velocity.z, Is.InRange(2f, 4f));
            }
        }
        finally { Random.state = randomState; }
    }
#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void RepeatedBuilderRetainsPrefabIdentityReferencesAndInspectorTuning()
    {
        const string path = "Assets/Art/VFX/ParryClash/ParryClash.prefab";
        var prefab = Prefab();
        string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
        byte[] original = System.IO.File.ReadAllBytes(path);
        try
        {
            var root = UnityEditor.PrefabUtility.LoadPrefabContents(path);
            try
            {
                var so = new UnityEditor.SerializedObject(root.GetComponent<ParryClashBurst>());
                // Re-enter migration with all references and particle objects already present.
                so.FindProperty("rayShapeVersion").intValue = 0;
                so.FindProperty("compactVersion").intValue = 0;
                so.FindProperty("compactMaterial").objectReferenceValue = null;
                so.ApplyModifiedPropertiesWithoutUndo();
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { UnityEditor.PrefabUtility.UnloadPrefabContents(root); }
            Prefab();
            root = UnityEditor.PrefabUtility.LoadPrefabContents(path);
            try
            {
                var so = new UnityEditor.SerializedObject(root.GetComponent<ParryClashBurst>());
                Assert.That(so.FindProperty("rayShapeVersion").intValue, Is.EqualTo(1));
                Assert.That(so.FindProperty("mainSettings").FindPropertyRelative("width").floatValue, Is.EqualTo(.1875f));
                Assert.That(root.GetComponentsInChildren<ParticleSystem>(true).Length, Is.EqualTo(5));
                so.FindProperty("mainSettings").FindPropertyRelative("width").floatValue = .14f;
                so.FindProperty("fanRotation").floatValue = 4;
                so.FindProperty("cameraRayLength").vector2Value = new Vector2(.6f, .65f);
                Assert.That(so.FindProperty("compactVersion").intValue, Is.EqualTo(1));
                Assert.That(so.FindProperty("compactMaterial").objectReferenceValue, Is.Not.Null);
                so.FindProperty("compact").FindPropertyRelative("longPixels").vector2IntValue = new Vector2Int(13, 15);
                so.ApplyModifiedPropertiesWithoutUndo();
                root.transform.Find("MainRays").GetComponent<ParticleSystemRenderer>().lengthScale = 2.1f;
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { UnityEditor.PrefabUtility.UnloadPrefabContents(root); }
            byte[] tuned = System.IO.File.ReadAllBytes(path);
            Prefab(); Prefab();
            Assert.That(UnityEditor.AssetDatabase.AssetPathToGUID(path), Is.EqualTo(guid));
            Assert.That(System.IO.File.ReadAllBytes(path), Is.EqualTo(tuned));
            var serialized = new UnityEditor.SerializedObject(Prefab().GetComponent<ParryClashBurst>());
            foreach (string field in new[] { "contact", "mainRays", "front", "back", "embers", "ground" })
                Assert.That(serialized.FindProperty(field).objectReferenceValue, Is.Not.Null);
        }
        finally
        {
            System.IO.File.WriteAllBytes(path, original);
            UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
        }
    }
#endif
    [Test, Category("Quick")]
    public void PresetsKeepBudgetsLengthsMirrorAndLifetimeData()
    {
        var saved = Random.state;
        try
        {
            var burst = Burst();
            var ps = burst.transform.Find("MainRays").GetComponent<ParticleSystem>();
            var particles = new ParticleSystem.Particle[64];
            var data = new List<Vector4>();
            var later = new List<Vector4>();
            foreach (var preset in new[] { ParryClashBurst.Preset.A, ParryClashBurst.Preset.B, ParryClashBurst.Preset.C })
            for (int sequence = 0; sequence < 24; sequence++)
            {
                Random.InitState(sequence);
                burst.Emit(Vector3.zero, 1, -10, preset, sequence);
                Assert.That(burst.ParticleCount, Is.EqualTo(66));
                ps.GetParticles(particles); ps.GetCustomParticleData(data, ParticleSystemCustomData.Custom1);
                int longCount = 0;
                var bySeed = new Dictionary<uint, Vector4>();
                var original = new ParticleSystem.Particle[12];
                for (int i = 0; i < 12; i++)
                {
                    original[i] = particles[i]; bySeed.Add(particles[i].randomSeed, data[i]);
                    if (i < 3) { Assert.That(particles[i].velocity.z, Is.InRange(-6f, -4f)); continue; }
                    float lo = .45f, hi = 1.30f;
                    if (preset == ParryClashBurst.Preset.A) lo = .65f;
                    if (preset == ParryClashBurst.Preset.C)
                    {
                        bool isLong = (i - 3 + sequence) % 3 == 0;
                        lo = isLong ? 1.10f : .45f; hi = isLong ? 1.30f : .75f;
                        if (isLong) longCount++;
                    }
                    if (preset == ParryClashBurst.Preset.B)
                    { lo = i % 2 == sequence % 2 ? .95f : .45f; hi = i % 2 == sequence % 2 ? 1.30f : .80f; }
                    Assert.That(data[i].w, Is.InRange(lo, hi));
                }
                if (preset == ParryClashBurst.Preset.C) Assert.That(longCount, Is.EqualTo(3));
                for (int step = 0; step < 10; step++)
                {
                    ps.Simulate(.03f, false, false);
                    int count = ps.GetParticles(particles); ps.GetCustomParticleData(later, ParticleSystemCustomData.Custom1);
                    for (int i = 0; i < count; i++) Assert.That(later[i], Is.EqualTo(bySeed[particles[i].randomSeed]));
                }
                Random.InitState(sequence); burst.Emit(Vector3.zero, -1, -10, preset, sequence); ps.GetParticles(particles);
                for (int i = 0; i < 12; i++)
                {
                    Assert.That(particles[i].velocity.x, Is.EqualTo(-original[i].velocity.x));
                    Assert.That(particles[i].velocity.y, Is.EqualTo(original[i].velocity.y));
                    Assert.That(particles[i].velocity.z, Is.EqualTo(original[i].velocity.z));
                }
            }
        }
        finally { Random.state = saved; }
    }
    [Test, Category("Quick")]
    public void CompactPixelBudgetMirrorLifetimeAndPresetRestoration()
    {
        var burst = Burst(); var saved = Random.state;
        var original = new Material[5]; var gravity = new float[5];
        var collision = new bool[5]; var color = new bool[5]; var limit = new bool[5];
        var size = new bool[5]; var simulation = new float[5];
        for (int g = 1; g < 5; g++)
        {
            var ps = burst.Systems[g]; original[g] = ps.GetComponent<ParticleSystemRenderer>().sharedMaterial;
            gravity[g] = ps.main.gravityModifier.constant; collision[g] = ps.collision.enabled;
            color[g] = ps.colorOverLifetime.enabled; limit[g] = ps.limitVelocityOverLifetime.enabled;
            size[g] = ps.sizeOverLifetime.enabled; simulation[g] = ps.main.simulationSpeed;
        }
        try
        {
            for (int seed = 0; seed < 24; seed++)
            {
                var forward = new Dictionary<int, ParticleSystem.Particle>();
                foreach (float direction in new[] { 1f, -1f })
                {
                    Random.InitState(seed); burst.Emit(new Vector3(3, 2, 0), direction, 0, ParryClashBurst.Preset.BCompact, seed);
                    Assert.That(burst.ParticleCount, Is.EqualTo(24));
                    for (int g = 0; g < 5; g++) Assert.That(burst.Systems[g].particleCount, Is.EqualTo(new[] { 2, 2, 10, 10, 0 }[g]));
                    for (int g = 1; g < 5; g++)
                    {
                        var ps = burst.Systems[g];
                        Assert.That(ps.main.gravityModifier.constant, Is.Zero);
                        Assert.That(ps.main.simulationSpeed, Is.Zero);
                        Assert.That(ps.collision.enabled || ps.colorOverLifetime.enabled || ps.sizeOverLifetime.enabled || ps.limitVelocityOverLifetime.enabled, Is.False);
                        var particles = new ParticleSystem.Particle[10]; int count = ps.GetParticles(particles);
                        for (int i = 0; i < count; i++)
                        {
                            var p = particles[i];
                            bool reverse = (g == 2 || g == 3) && i >= 4;
                            Assert.That(p.velocity.x * direction * (reverse ? -1 : 1), Is.GreaterThan(0), "twelve added sparks travel opposite the original fan");
                            Assert.That(p.position, Is.EqualTo(new Vector3(3, 2, 0)), "all twenty-two sparks begin at the contact in the same frame");
                            if (reverse) Assert.That(Mathf.Abs(p.velocity.x), Is.InRange(32 * .85f * Mathf.Cos(53 * Mathf.Deg2Rad), 44 * 1.15f));
                            else Assert.That(new Vector2(p.velocity.x, p.velocity.y).magnitude, Is.InRange(14 * .85f, 24 * 1.15f));
                            if (g == 2) Assert.That(p.velocity.z, Is.InRange(-6, -4));
                            if (g == 3) Assert.That(p.velocity.z, Is.InRange(2, 4));
                            Assert.That(p.startSize3D.x * 16, Is.EqualTo(Mathf.Round(p.startSize3D.x * 16)));
                            Assert.That(p.startSize3D.y * 16, Is.EqualTo(Mathf.Round(p.startSize3D.y * 16)));
                            if (direction > 0) forward.Add(g * 10 + i, p);
                            else
                            {
                                var f = forward[g * 10 + i];
                                Assert.That(p.velocity, Is.EqualTo(new Vector3(-f.velocity.x, f.velocity.y, f.velocity.z)));
                                Assert.That(p.startLifetime, Is.EqualTo(f.startLifetime));
                                Assert.That(p.startSize3D, Is.EqualTo(f.startSize3D));
                            }
                        }
                    }
                    burst.AdvanceDebris(.2f);
                    Assert.That(burst.Systems[1].particleCount + burst.Systems[2].particleCount + burst.Systems[3].particleCount, Is.EqualTo(20));
                    burst.Emit(Vector3.one, direction, 0, ParryClashBurst.Preset.B);
                    Assert.That(burst.ParticleCount, Is.EqualTo(66));
                    for (int g = 1; g < 5; g++)
                    {
                        var ps = burst.Systems[g];
                        Assert.That(ps.GetComponent<ParticleSystemRenderer>().sharedMaterial, Is.SameAs(original[g]));
                        Assert.That(ps.main.gravityModifier.constant, Is.EqualTo(gravity[g]));
                        Assert.That(ps.main.simulationSpeed, Is.EqualTo(simulation[g]));
                        Assert.That(ps.collision.enabled, Is.EqualTo(collision[g])); Assert.That(ps.colorOverLifetime.enabled, Is.EqualTo(color[g]));
                        Assert.That(ps.sizeOverLifetime.enabled, Is.EqualTo(size[g])); Assert.That(ps.limitVelocityOverLifetime.enabled, Is.EqualTo(limit[g]));
                    }
                }
            }
        }
        finally { Random.state = saved; }
    }

    [Test, Category("Quick")]
    public void CompactReverseFanClearsPlayerAndRetainsVisibleStreaks()
    {
        var burst = Burst(); var saved = Random.state;
        try
        {
            for (int seed = 0; seed < 24; seed++)
            foreach (float direction in new[] { 1f, -1f })
            {
                Random.InitState(seed);
                burst.Emit(new Vector3(0, 1, 0), direction, 0, ParryClashBurst.Preset.BCompact, seed);
                var reverse = new HashSet<uint>();
                var particles = new ParticleSystem.Particle[10];
                for (int g = 2; g <= 3; g++)
                {
                    int count = burst.Systems[g].GetParticles(particles);
                    Assert.That(count, Is.EqualTo(10));
                    int longCount = 0;
                    for (int i = 4; i < count; i++)
                    {
                        reverse.Add(particles[i].randomSeed);
                        if (particles[i].startSize3D.y * 16 >= 12) longCount++;
                    }
                    Assert.That(longCount, Is.EqualTo(3), "three long and three short reverse rays per depth layer");
                }
                burst.AdvanceDebris(.3f);
                int beyond = 0, streaks = 0;
                for (int g = 2; g <= 3; g++)
                {
                    int count = burst.Systems[g].GetParticles(particles);
                    for (int i = 0; i < count; i++)
                    {
                        var p = particles[i];
                        if (!reverse.Contains(p.randomSeed)) continue;
                        if (-p.position.x * direction >= 2.5f) beyond++;
                        if (p.startSize3D.y * 16 >= 2) streaks++;
                    }
                }
                Assert.That(beyond, Is.GreaterThanOrEqualTo(8), "seed " + seed + " direction " + direction);
                Assert.That(streaks, Is.GreaterThanOrEqualTo(6), "seed " + seed + " direction " + direction);
            }
        }
        finally { Random.state = saved; }
    }

    [Test, Category("Quick")]
    public void CompactDebrisFallsBouncesOnceRestsFadesAndClearsOnReuse()
    {
        var burst = Burst(); var saved = Random.state;
        try
        {
            foreach (float direction in new[] { 1f, -1f })
            {
                Random.InitState(42); burst.Emit(new Vector3(0, 1, 0), direction, 0, ParryClashBurst.Preset.BCompact);
                var previous = new Dictionary<uint, ParticleSystem.Particle>();
                var bounces = new HashSet<uint>(); var rests = new HashSet<uint>();
                var buffer = new ParticleSystem.Particle[10];
                for (int step = 0; step <= 250; step++)
                {
                    for (int g = 1; g <= 3; g++)
                    {
                        int count = burst.Systems[g].GetParticles(buffer);
                        for (int i = 0; i < count; i++)
                        {
                            var p = buffer[i]; bool fast = p.startLifetime < .3f;
                            Assert.That(p.position.y, Is.GreaterThanOrEqualTo(1f / 32 - .000001f));
                            Assert.That(p.startSize3D.x * 16, Is.GreaterThanOrEqualTo(1));
                            Assert.That(p.startSize3D.y * 16, Is.GreaterThanOrEqualTo(1));
                            Assert.That(p.startSize3D.y * 16, Is.EqualTo(Mathf.Round(p.startSize3D.y * 16)));
                            if (previous.TryGetValue(p.randomSeed, out var old))
                            {
                                Assert.That(Vector3.Distance(p.position, old.position), Is.LessThan(.52f), "no replacement or discontinuous teleport");
                                Assert.That(p.startColor.a, Is.LessThanOrEqualTo(old.startColor.a));
                                if (!fast && old.velocity.y < 0 && p.velocity.y > 0)
                                    Assert.That(bounces.Add(p.randomSeed), Is.True, "only one bounce");
                                if (rests.Contains(p.randomSeed)) Assert.That(p.position, Is.EqualTo(old.position), "rest stops X/Y/Z");
                            }
                            if (!fast && p.velocity == Vector3.zero)
                            {
                                rests.Add(p.randomSeed);
                                Assert.That(p.position.y, Is.EqualTo(1f / 32).Within(.000001f));
                                Assert.That(p.startSize3D.x, Is.EqualTo(1f / 16)); Assert.That(p.startSize3D.y, Is.EqualTo(1f / 16));
                                Assert.That(p.startColor.r, Is.LessThan(60));
                            }
                            previous[p.randomSeed] = p;
                        }
                    }
                    burst.AdvanceDebris(.01f);
                }
                Assert.That(bounces.Count, Is.EqualTo(20)); Assert.That(rests.Count, Is.EqualTo(20)); Assert.That(burst.Systems[1].particleCount + burst.Systems[2].particleCount + burst.Systems[3].particleCount, Is.Zero);
                burst.Clear(); burst.AdvanceDebris(.1f); Assert.That(burst.ParticleCount, Is.Zero);
                // Absolute-age integration must agree even across skipped contacts.
                Random.InitState(42); burst.Emit(new Vector3(0, 1, 0), direction, 0, ParryClashBurst.Preset.BCompact);
                burst.AdvanceDebris(1);
                var single = new Dictionary<int, Vector3>();
                for (int g = 1; g <= 3; g++) { int n = burst.Systems[g].GetParticles(buffer); for (int i = 0; i < n; i++) single.Add(g * 10 + i, buffer[i].position); }
                Random.InitState(42); burst.Emit(new Vector3(0, 1, 0), direction, 0, ParryClashBurst.Preset.BCompact);
                for (int n = 0; n < 100; n++) burst.AdvanceDebris(.01f);
                for (int g = 1; g <= 3; g++) { int n = burst.Systems[g].GetParticles(buffer); for (int i = 0; i < n; i++) Assert.That(Vector3.Distance(buffer[i].position, single[g * 10 + i]), Is.LessThan(.00001f)); }
            }
        }
        finally { Random.state = saved; }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PauseHoldsAgeAndPositionWhileHitstopContinuesAndReleaseIsSingle()
    {
        var burst = Burst(); var ps = burst.transform.Find("MainRays").GetComponent<ParticleSystem>();
        int releases = 0;
        var pool = new ObjectPool<GameObject>(() => null, null, go => { releases++; go.SetActive(false); });
        burst.GetComponent<PooledVFX>().Setup(pool);
        burst.Emit(Vector3.zero, 1, -10, ParryClashBurst.Preset.BCompact);
        Time.timeScale = .001f; yield return new WaitForSecondsRealtime(.035f);
        var particles = new ParticleSystem.Particle[64]; ps.GetParticles(particles);
        Assert.That(particles[0].position.sqrMagnitude, Is.GreaterThan(.001f));
        GamePauseController.SetTutorialReading(true); yield return null;
        var heldGroups = new Dictionary<ParticleSystem, ParticleSystem.Particle[]>();
        foreach (var group in burst.GetComponentsInChildren<ParticleSystem>())
        {
            var held = new ParticleSystem.Particle[group.particleCount]; group.GetParticles(held);
            heldGroups.Add(group, held);
        }
        yield return new WaitForSecondsRealtime(.09f);
        foreach (var pair in heldGroups)
        {
            Assert.That(pair.Key.particleCount, Is.EqualTo(pair.Value.Length));
            var current = new ParticleSystem.Particle[pair.Value.Length]; pair.Key.GetParticles(current);
            for (int i = 0; i < current.Length; i++)
            {
                Assert.That(current[i].position, Is.EqualTo(pair.Value[i].position));
                Assert.That(current[i].remainingLifetime, Is.EqualTo(pair.Value[i].remainingLifetime));
            }
        }
        GamePauseController.SetTutorialReading(false); Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(3f);
        Assert.That(releases, Is.EqualTo(1));
        burst.GetComponent<PooledVFX>().Release(); Assert.That(releases, Is.EqualTo(1));
        burst.gameObject.SetActive(true); Assert.That(burst.ParticleCount, Is.Zero);
        burst.Emit(Vector3.one, -1, -10, ParryClashBurst.Preset.BCompact); Assert.That(burst.ParticleCount, Is.EqualTo(24));
        pool.Clear();
    }
#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void ProductionBuilderSelectsCompactForExistingSerializedCurrentOwner()
    {
        var host = new GameObject("serialized parry owner"); host.SetActive(false); owned.Add(host);
        host.AddComponent<BoxCollider2D>();
        var fx = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(fx, "sparkPreset", ParryClashBurst.Preset.Current);
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var builder = assembly.GetType("CombatFeedbackBuild");
            if (builder == null) continue;
            var configure = builder.GetMethod("Configure", BindingFlags.Static | BindingFlags.NonPublic);
            for (int repeat = 0; repeat < 2; repeat++)
            {
                configure.Invoke(null, new object[] { host, null, null, null });
                var selected = typeof(ParrySparkVFX_Pooled).GetField("sparkPreset", Flags).GetValue(fx);
                Assert.That(selected, Is.EqualTo(ParryClashBurst.Preset.BCompact));
            }
            return;
        }
        Assert.Fail("CombatFeedbackBuild was not found");
    }
#endif

    [Test, Category("Quick")]
    public void DefaultParryEmitsCompactPhysicsWithoutPresetSelectionAndAfterOwnerReuse()
    {
        var host = new GameObject("default parry owner"); host.SetActive(false); owned.Add(host);
        host.transform.position = Vector3.up;
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var fx = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(fx, "parrySystem", parry); Set(fx, "sparkPrefab", Prefab());
        Set(fx, "playerSparkAnchor", host.transform); Set(fx, "groundY", 0f);
        for (int emission = 0; emission < 2; emission++)
        {
            host.SetActive(true); parry.ParrySuccess();
            var active = (List<GameObject>)typeof(ParrySparkVFX_Pooled).GetField("active", Flags).GetValue(fx);
            Assert.That(active.Count, Is.EqualTo(1));
            var burst = active[0].GetComponent<ParryClashBurst>();
            Assert.That(burst.EmittedPreset, Is.EqualTo(ParryClashBurst.Preset.BCompact));
            Assert.That(burst.ParticleCount, Is.EqualTo(24));
            burst.AdvanceDebris(.3f);
            Assert.That(burst.Systems[2].particleCount + burst.Systems[3].particleCount, Is.EqualTo(18));
            host.SetActive(false);
            Assert.That(Object.FindObjectsOfType<ParryClashBurst>(), Is.Empty);
        }
    }

    [Test, Category("Quick")]
    public void SuccessfulParryUsesEightLeasesAndOwnerDisableClearsThem()
    {
        var host = new GameObject("parry owner"); host.SetActive(false); owned.Add(host);
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var fx = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(fx, "parrySystem", parry); Set(fx, "sparkPrefab", Prefab()); Set(fx, "playerSparkAnchor", host.transform);
        Set(fx, "groundY", -2f); // A real contact is above the floor; preserve initial ray lengths.
        host.SetActive(true); Assert.That(Object.FindObjectsOfType<ParryClashBurst>(), Is.Empty);
        Set(fx, "sparkPreset", ParryClashBurst.Preset.C);
        for (int sequence = 0; sequence < 12; sequence++)
        {
            parry.ParrySuccess();
            var active = (List<GameObject>)typeof(ParrySparkVFX_Pooled).GetField("active", Flags).GetValue(fx);
            var rays = active[active.Count - 1].transform.Find("MainRays").GetComponent<ParticleSystem>();
            var shapes = new List<Vector4>(); rays.GetCustomParticleData(shapes, ParticleSystemCustomData.Custom1);
            for (int i = 3; i < 12; i++)
                Assert.That(shapes[i].w > 1f, Is.EqualTo((i - 3 + sequence) % 3 == 0),
                    "owner sequence must continue when the eight-lease pool recycles");
        }
        Set(fx, "sparkPreset", ParryClashBurst.Preset.BCompact);
        for (int sequence = 0; sequence < 12; sequence++)
        {
            parry.ParrySuccess();
            var active = (List<GameObject>)typeof(ParrySparkVFX_Pooled).GetField("active", Flags).GetValue(fx);
            var burst = active[active.Count - 1].GetComponent<ParryClashBurst>();
            Assert.That(burst.ParticleCount, Is.EqualTo(24));
            var shapes = new List<Vector4>(); burst.Systems[1].GetCustomParticleData(shapes, ParticleSystemCustomData.Custom1);
            for (int i = 0; i < 2; i++) Assert.That(shapes[i].w >= 12, Is.EqualTo(i == sequence % 2));
            int live = 0;
            foreach (var go in active) live += go.GetComponent<ParryClashBurst>().ParticleCount;
            if (sequence >= 7) Assert.That(live, Is.EqualTo(192));
        }
        Assert.That(Object.FindObjectsOfType<ParryClashBurst>().Length, Is.EqualTo(8));
        host.SetActive(false); Assert.That(Object.FindObjectsOfType<ParryClashBurst>(), Is.Empty);
    }
    [Test, Category("Quick")]
    public void CompactContactAndPulseHaveReadableHoldAndBoundedFade()
    {
        var burst = Burst(); burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        var contact = burst.Systems[0];
        var advance = typeof(ParryClashBurst).GetMethod("AdvanceContact", Flags);
        var particles = new ParticleSystem.Particle[2];
        foreach (float age in new[] { 0f, .06f, .09f, .14f, .2f, .22f, .26f, .28f })
        {
            advance.Invoke(burst, new object[] { age });
            int count = contact.GetParticles(particles);
            Assert.That(count, Is.EqualTo(age < .2f ? 2 : age < .26f ? 1 : 0));
            if (age <= .06f) Assert.That(particles[0].startColor.a, Is.EqualTo(255));
            if (age < .2f) Assert.That(particles[0].startSize, Is.EqualTo(.5f).Within(.001f));
            if (age >= .09f && age < .26f) Assert.That(particles[count - 1].startSize, Is.EqualTo(1.5f).Within(.001f));
        }
        burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.B);
        Assert.That(contact.main.simulationSpeed, Is.GreaterThan(0));
        Assert.That(contact.colorOverLifetime.enabled, Is.True);
        Assert.That(contact.GetComponent<ParticleSystemRenderer>().sharedMaterial.name, Is.Not.EqualTo("CompactContact"));
        burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        Assert.That(contact.GetComponent<ParticleSystemRenderer>().sharedMaterial.name, Is.EqualTo("CompactContact"));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CompactContactClockIsIndependentOfCompletedSparkSimulation()
    {
        var burst = Burst(); int releases = 0;
        var pool = new ObjectPool<GameObject>(() => null, null, go => { releases++; go.SetActive(false); });
        burst.GetComponent<PooledVFX>().Setup(pool);
        burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        burst.AdvanceDebris(10); // Complete the spark simulation without advancing contact time.
        yield return null;
        Assert.That(releases, Is.Zero);
        Assert.That(burst.Systems[0].particleCount, Is.EqualTo(2));
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(releases, Is.EqualTo(1));
        Assert.That(burst.ParticleCount, Is.Zero);
        pool.Clear();
    }

    [Test, Category("Quick")]
    public void CompactPulseHolds70msHalfAt175AndExpires280AlongsideLegacy()
    {
        var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        var host = new GameObject("compact pulse rig"); host.SetActive(false); owned.Add(host);
        var light = host.AddComponent(lightType);
        var rig = host.AddComponent<ClashLightingRig>(); Set(rig, "clashLight", light); host.SetActive(true);
        var burst = Burst(); burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        var pulses = (System.Array)typeof(ClashLightingRig).GetField("pulses", Flags).GetValue(rig);
        var render = typeof(ClashLightingRig).GetMethod("Render", Flags);
        var intensity = lightType.GetProperty("intensity");
        foreach (var sample in new[] { new Vector2(0, 5), new Vector2(.07f, 5), new Vector2(.175f, 2.5f), new Vector2(.28f, 0) })
        {
            var pulse = pulses.GetValue(0);
            pulse.GetType().GetField("age").SetValue(pulse, sample.x); pulses.SetValue(pulse, 0);
            render.Invoke(rig, null);
            Assert.That((float)intensity.GetValue(light), Is.EqualTo(sample.y).Within(.001f));
        }
        burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        var legacy = Burst(); legacy.Emit(Vector3.right, 1, 0, ParryClashBurst.Preset.B);
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(5));
        burst.gameObject.SetActive(false);
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(2.4f).Within(.001f));
        host.SetActive(false); Assert.That((float)intensity.GetValue(light), Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CompactPulsePauseAndLeaseReuseRetainTheirOwnProfile()
    {
        var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        var host = new GameObject("paused compact light"); host.SetActive(false); owned.Add(host);
        var light = host.AddComponent(lightType);
        var rig = host.AddComponent<ClashLightingRig>(); Set(rig, "clashLight", light); host.SetActive(true);
        var burst = Burst(); burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.BCompact);
        var intensity = lightType.GetProperty("intensity");
        GamePauseController.SetTutorialReading(true);
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(5));
        GamePauseController.SetTutorialReading(false);
        yield return new WaitForSecondsRealtime(.31f);
        Assert.That((float)intensity.GetValue(light), Is.Zero);
        burst.Emit(Vector3.one, 1, 0, ParryClashBurst.Preset.B);
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(2.4f).Within(.001f));
        yield return new WaitForSecondsRealtime(.16f);
        Assert.That((float)intensity.GetValue(light), Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PulseUsesStrongestOverlapHoldsOnPauseAndClearsWithOwner()
    {
        var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        Assert.That(lightType, Is.Not.Null);
        var host = new GameObject("pulse rig"); host.SetActive(false); owned.Add(host);
        var light = host.AddComponent(lightType);
        var rig = host.AddComponent<ClashLightingRig>(); Set(rig,"clashLight",light);
        host.SetActive(true);
        var first = Burst(); first.Emit(Vector3.one,1);
        var second = Burst(); second.Emit(Vector3.right*2,1);
        var intensity = lightType.GetProperty("intensity");
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(2.4f).Within(.001f));
        GamePauseController.SetTutorialReading(true);
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(2.4f).Within(.001f));
        first.gameObject.SetActive(false);
        Assert.That((float)intensity.GetValue(light), Is.EqualTo(2.4f).Within(.001f));
        second.gameObject.SetActive(false);
        Assert.That((float)intensity.GetValue(light), Is.Zero);
        GamePauseController.SetTutorialReading(false);
        first.gameObject.SetActive(true); first.Emit(Vector3.zero,1);
        yield return new WaitForSecondsRealtime(.18f);
        Assert.That((float)intensity.GetValue(light), Is.Zero);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator SceneUnloadReclaimsClashAndPulse()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.CreateScene("clash unload");
        var host = new GameObject("scene parry owner"); host.SetActive(false); owned.Add(host);
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host,scene);
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var fx = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(fx,"parrySystem",parry); Set(fx,"sparkPrefab",Prefab()); Set(fx,"playerSparkAnchor",host.transform);
        host.SetActive(true); parry.ParrySuccess();
        Assert.That(Object.FindObjectsOfType<ParryClashBurst>().Length,Is.EqualTo(1));
        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        Assert.That(Object.FindObjectsOfType<ParryClashBurst>(),Is.Empty);
    }
    [Test, Category("Quick")]
    public void MissingHitPrefabsPermitNormalSlayerAndLethalWithoutParticles()
    {
        var host = new GameObject("audio only impacts"); owned.Add(host);
        var spawner = host.AddComponent<HitSparkSpawner>();
        spawner.SpawnImpact(Vector2.zero, host.transform, false, false);
        spawner.SpawnImpact(Vector2.zero, host.transform, true, false);
        spawner.SpawnImpact(Vector2.zero, host.transform, false, true);
        Assert.That(Object.FindObjectsOfType<PooledVFX>(), Is.Empty);
    }
}
