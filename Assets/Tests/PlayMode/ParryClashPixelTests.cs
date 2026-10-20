using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TestTools;

[DefaultExecutionOrder(-60)]
public sealed class ParryClashEmissionProbe : MonoBehaviour
{
    internal ParryClashBurst burst;
    internal Vector3 point;
    internal bool pending;
    internal int frame = -1, birthCount;
    internal float birthDelta, birthAge, nextDelta, nextAge = -1, nextDisplacement;
    internal readonly Vector3[] birthPositions = new Vector3[24];
    readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[10];
    static readonly System.Reflection.FieldInfo Age = typeof(ParryClashBurst).GetField("debrisAge",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
    void Update()
    {
        if (!pending) return;
        pending = false; frame = Time.frameCount; birthDelta = Time.unscaledDeltaTime;
        burst.Emit(point, 1, 0, ParryClashBurst.Preset.BCompact, 7);
    }
    void LateUpdate()
    {
        if (frame < 0 || nextAge >= 0) return;
        if (Time.frameCount == frame)
        {
            birthAge = (float)Age.GetValue(burst); birthCount = 0;
            foreach (var ps in burst.Systems)
                for (int i = 0, count = ps.GetParticles(particles); i < count; i++)
                    birthPositions[birthCount++] = particles[i].position;
            // Reproduce a cold first render stalling before the following frame.
            System.Threading.Thread.Sleep(100);
        }
        else
        {
            nextDelta = Time.unscaledDeltaTime;
            nextAge = (float)Age.GetValue(burst); nextDisplacement = 0;
            foreach (var ps in burst.Systems)
                for (int i = 0, count = ps.GetParticles(particles); i < count; i++)
                    nextDisplacement = Mathf.Max(nextDisplacement, Vector3.Distance(particles[i].position, point));
        }
    }
}

public sealed partial class ParryClashTests
{
    [UnityTest, Category("Quick")]
    public IEnumerator EarlyUpdateEmissionRendersAtContactBeforeFlightEvenAfterSlowFrameAndReuse()
    {
        var burst = Burst();
        var host = new GameObject("early parry emission"); owned.Add(host);
        var probe = host.AddComponent<ParryClashEmissionProbe>(); probe.burst = burst;
        foreach (bool reuse in new[] { false, true })
        {
            if (reuse) { burst.gameObject.SetActive(false); burst.gameObject.SetActive(true); }
            probe.point = reuse ? new Vector3(-3, 2, -.1f) : new Vector3(3, 2, .1f);
            probe.frame = -1; probe.nextAge = -1; probe.pending = true;
            // The next Update receives elapsed time that happened before emission.
            System.Threading.Thread.Sleep(100);
            while (probe.nextAge < 0) yield return null;
            Assert.That(probe.birthDelta, Is.GreaterThan(.08f), "exercise a genuinely slow emission frame");
            Assert.That(probe.birthAge, Is.Zero, "pre-emission time must not move a newborn particle");
            Assert.That(probe.birthCount, Is.EqualTo(24));
            foreach (var position in probe.birthPositions) Assert.That(position, Is.EqualTo(probe.point));
            Assert.That(probe.nextAge, Is.GreaterThan(0), "flight starts on the following rendered frame");
            Assert.That(probe.nextDisplacement, Is.GreaterThan(0));
            Assert.That(probe.nextDelta, Is.GreaterThan(.08f), "exercise a stall after the birth render too");
            Assert.That(probe.nextDisplacement, Is.LessThanOrEqualTo(1),
                "the first visible flight step must stay in the contact footprint despite a cold-render stall");
        }
    }

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
        int[] expected = { 1, 2, 10, 1, 10 };
        bool near = false, far = false;
        for (int g = 0; g < 5; g++)
        {
            var ps = burst.Systems[g]; Assert.That(ps.particleCount, Is.EqualTo(expected[g]));
            Assert.That(ps.main.simulationSpeed, Is.EqualTo(g == 0 || g == 3 ? 1 : 0));
            Assert.That(ps.GetComponent<ParticleSystemRenderer>().renderMode, Is.EqualTo(ParticleSystemRenderMode.Billboard));
            var particles = new ParticleSystem.Particle[32]; int count = ps.GetParticles(particles);
            for (int i = 0; i < count; i++)
            {
                Assert.That(particles[i].position, Is.EqualTo(point));
                if (g == 0 || g == 3) continue;
                Assert.That(ps.GetComponent<ParticleSystemRenderer>().sharedMaterial.name, Is.EqualTo("Compact"));
                Assert.That(ps.main.gravityModifier.constant, Is.Zero);
                Assert.That(ps.collision.enabled || ps.colorOverLifetime.enabled || ps.sizeOverLifetime.enabled || ps.limitVelocityOverLifetime.enabled, Is.False);
                Assert.That(particles[i].startSize3D.x, Is.GreaterThanOrEqualTo(1f / 16));
                near |= particles[i].velocity.z < 0; far |= particles[i].velocity.z > 0;
            }
        }
        Assert.That(near && far, Is.True, "depth comes from 3D motion, not voxel meshes");
        burst.Clear(); Assert.That(burst.ParticleCount, Is.Zero);
    }

    [Test, Category("Quick")]
    public void PixelContactsRetainApprovedCelsSizeColorAndTwoHundredMillisecondTiming()
    {
        var burst = Burst(); burst.Emit(new Vector3(0, 2, 0), 1, 0, ParryClashBurst.Preset.BCompact);
        var core = burst.Systems[0]; var outer = burst.Systems[3];
        Assert.That(outer.textureSheetAnimation.numTilesX, Is.EqualTo(6));
        Assert.That(outer.textureSheetAnimation.startFrame.constant, Is.EqualTo(1f / 6).Within(.00001f));
        var p = new ParticleSystem.Particle[1];
        for (int group = 0; group <= 3; group += 3)
        {
            var ps = burst.Systems[group]; ps.GetParticles(p);
            Assert.That(p[0].startLifetime, Is.EqualTo(.2f));
            Assert.That(p[0].startSize, Is.EqualTo(1f));
            Assert.That(p[0].startColor, Is.EqualTo((Color32)Color.white));
            Assert.That(p[0].velocity, Is.EqualTo(Vector3.zero));
            Assert.That(ps.GetComponent<ParticleSystemRenderer>().sharedMaterial.name, Is.EqualTo("PixelContact"));
            ps.Simulate(.069f, false, false, false); ps.GetParticles(p);
            Assert.That(p[0].GetCurrentColor(ps).a, Is.GreaterThanOrEqualTo(253));
            ps.Simulate(.1311f, false, false, false);
            Assert.That(ps.particleCount, Is.Zero);
        }
        burst.AdvanceDebris(.42f);
        Assert.That(burst.Systems[1].particleCount + burst.Systems[2].particleCount + burst.Systems[4].particleCount,
            Is.EqualTo(20), "falling particles must survive the discarded 220-420ms ray lifetimes");
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
                foreach (int g in new[] { 2, 4 })
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
                foreach (int g in new[] { 2, 4 })
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
                    foreach (int g in new[] { 1, 2, 4 })
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
                Assert.That(bounces.Count, Is.EqualTo(20)); Assert.That(rests.Count, Is.EqualTo(20)); Assert.That(burst.Systems[1].particleCount + burst.Systems[2].particleCount + burst.Systems[4].particleCount, Is.Zero);
                burst.Clear(); burst.AdvanceDebris(.1f); Assert.That(burst.ParticleCount, Is.Zero);
                // Absolute-age integration must agree even across skipped contacts.
                Random.InitState(42); burst.Emit(new Vector3(0, 1, 0), direction, 0, ParryClashBurst.Preset.BCompact);
                burst.AdvanceDebris(1);
                var single = new Dictionary<int, Vector3>();
                foreach (int g in new[] { 1, 2, 4 }) { int n = burst.Systems[g].GetParticles(buffer); for (int i = 0; i < n; i++) single.Add(g * 10 + i, buffer[i].position); }
                Random.InitState(42); burst.Emit(new Vector3(0, 1, 0), direction, 0, ParryClashBurst.Preset.BCompact);
                for (int n = 0; n < 100; n++) burst.AdvanceDebris(.01f);
                foreach (int g in new[] { 1, 2, 4 }) { int n = burst.Systems[g].GetParticles(buffer); for (int i = 0; i < n; i++) Assert.That(Vector3.Distance(buffer[i].position, single[g * 10 + i]), Is.LessThan(.00001f)); }
            }
        }
        finally { Random.state = saved; }
    }

    [Test, Category("Quick")]
    public void AnalyticalParticleUpdatesReuseFixedBuffersWithoutManagedAllocation()
    {
        var burst = Burst(); burst.Emit(Vector3.up * 2, 1, 0, ParryClashBurst.Preset.BCompact);
        burst.AdvanceDebris(.001f);
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100; i++) burst.AdvanceDebris(.001f);
        long allocated = System.GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.That(allocated, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CompletedDebrisWaitsForAuthoredContactBeforeReturningLease()
    {
        var burst = Burst(); int releases = 0;
        var pool = new ObjectPool<GameObject>(() => null, null, go => { releases++; go.SetActive(false); });
        burst.GetComponent<PooledVFX>().Setup(pool);
        burst.Emit(Vector3.up, 1, 0, ParryClashBurst.Preset.BCompact);
        burst.AdvanceDebris(20);
        yield return null;
        Assert.That(releases, Is.Zero, "completed debris cannot truncate the approved contact");
        Assert.That(burst.Systems[0].particleCount + burst.Systems[3].particleCount, Is.EqualTo(2));
        float deadline = Time.realtimeSinceStartup + 1;
        while (releases == 0 && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(releases, Is.EqualTo(1));
        pool.Clear();
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
