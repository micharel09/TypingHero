using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[DefaultExecutionOrder(100)]
public sealed class PixelSparkReadingLockDriver : MonoBehaviour
{
    public ParticleSystem source;
    public bool arm;
    public float heldLife;
    public Vector3 heldPosition;
    readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[56];
    void Update()
    {
        if (!arm) return;
        source.GetParticles(particles);
        heldLife = particles[0].remainingLifetime; heldPosition = particles[0].position;
        GamePauseController.SetTutorialReading(true); arm = false;
    }
}

public sealed class PixelSparkTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    readonly List<GameObject> owned = new();
    readonly List<Scene> scenes = new();
    static void Set(object o, string field, object value) => o.GetType().GetField(field, Flags).SetValue(o, value);
    GameObject Host(string name)
    {
        var go = new GameObject(name); go.SetActive(false); owned.Add(go); return go;
    }
    static GameObject Prefab(string role)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/PixelSparks/" + role + ".prefab");
#else
        return null;
#endif
    }
    PixelSparkBurst Burst(string role)
    {
        var prefab = Prefab(role); Assert.That(prefab, Is.Not.Null, role);
        var go = Object.Instantiate(prefab); owned.Add(go); return go.GetComponent<PixelSparkBurst>();
    }
    HitSparkSpawner Hits(GameObject host)
    {
        var spawner = host.AddComponent<HitSparkSpawner>();
        Set(spawner, "sparkPrefab", Prefab("Normal")); Set(spawner, "slayerPrefabs", new[] { Prefab("Slayer") });
        Set(spawner, "lethalPrefab", Prefab("Lethal")); Set(spawner, "groundY", -1.5f);
        Set(spawner, "minInterval", 0f); return spawner;
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        Time.timeScale = 1; GamePauseController.SetTutorialReading(false);
        foreach (var go in owned) if (go) Object.Destroy(go);
        owned.Clear(); yield return null;
        GamePauseController.SetTutorialReading(false); Time.timeScale = 1; AudioListener.pause = false;
        foreach (var scene in scenes) if (scene.IsValid() && scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        scenes.Clear();
        Assert.That(Object.FindObjectsOfType<PixelSparkBurst>(), Is.Empty);
    }

    [Test, Category("Quick")]
    public void ConfirmedHitProfilesShareAnEightBurstLimitAndFollowTargetContext()
    {
        var host = Host("hit owner"); host.transform.position = new Vector3(2, 0, 0);
        var target = Host("left target"); target.transform.position = new Vector3(-3, 0, 0);
        var spawner = Hits(host); host.SetActive(true);
        for (int i = 0; i < 20; i++)
        {
            spawner.SpawnImpact(new Vector2(i, 2), host.transform, i % 3 != 0, i % 3 == 2, target.transform);
            var bursts = Object.FindObjectsOfType<PixelSparkBurst>();
            Assert.That(bursts.Length, Is.EqualTo(Mathf.Min(i + 1, 8)));
            PixelSparkBurst latest = null;
            foreach (var burst in bursts)
            {
                Assert.That(burst.transform.position.x, Is.GreaterThanOrEqualTo(Mathf.Max(0, i - 7)), "oldest burst is recycled across profiles");
                Assert.That(burst.transform.localScale, Is.EqualTo(Vector3.one));
                Assert.That(burst.transform.rotation, Is.EqualTo(Quaternion.identity));
                Assert.That(burst.GroundY, Is.EqualTo(-1.5f).Within(.000001f));
                if (burst.transform.position.x == i) latest = burst;
            }
            var role = i % 3 == 0 ? PixelSparkBurst.Profile.Normal : i % 3 == 1 ? PixelSparkBurst.Profile.Slayer : PixelSparkBurst.Profile.Lethal;
            Assert.That(latest.Role, Is.EqualTo(role));
            Assert.That(latest.ParticleCount, Is.EqualTo(role == PixelSparkBurst.Profile.Normal ? 24 : role == PixelSparkBurst.Profile.Slayer ? 32 : 56));
            float velocity = 0;
            foreach (var ps in latest.Systems)
            {
                var particles = new ParticleSystem.Particle[56]; int count = ps.GetParticles(particles);
                for (int p = 0; p < count; p++) velocity += particles[p].velocity.x;
            }
            Assert.That(velocity, Is.LessThan(0), "target position, not impact point or attacker scale, owns force direction");
        }
        host.SetActive(false); Assert.That(Object.FindObjectsOfType<PixelSparkBurst>(), Is.Empty);
    }

    [Test, Category("Quick")]
    public void PublicHitSpawnReusesCleanParticlesAtTheContactPoint()
    {
        var host = Host("reuse owner"); var target = Host("right target"); target.transform.position = Vector3.right * 3;
        var spawner = Hits(host); host.SetActive(true);
        spawner.Spawn(new Vector2(1, 2), host.transform, target.transform);
        var first = Object.FindObjectOfType<PixelSparkBurst>();
        first.transform.localScale *= 7; first.transform.rotation = Quaternion.Euler(0, 0, 45);
        foreach (var ps in first.Systems)
        {
            var particles = new ParticleSystem.Particle[56]; int count = ps.GetParticles(particles);
            for (int i = 0; i < count; i++) particles[i].startColor = Color.red;
            ps.SetParticles(particles, count);
        }
        first.GetComponent<PooledVFX>().Release(); Assert.That(first.ParticleCount, Is.Zero);
        spawner.Spawn(new Vector2(2, 3), host.transform, target.transform);
        Assert.That(Object.FindObjectOfType<PixelSparkBurst>(), Is.SameAs(first));
        Assert.That(first.ParticleCount, Is.EqualTo(24)); Assert.That(first.transform.localScale, Is.EqualTo(Vector3.one));
        Assert.That(first.transform.rotation, Is.EqualTo(Quaternion.identity));
        foreach (var ps in first.Systems)
        {
            var particles = new ParticleSystem.Particle[56]; int count = ps.GetParticles(particles);
            for (int i = 0; i < count; i++)
            {
                Assert.That((Color)particles[i].startColor, Is.EqualTo(Color.white));
                Assert.That(particles[i].position, Is.EqualTo(new Vector3(2, 3, 0)));
            }
        }
    }

    [Test, Category("Quick")]
    public void ActualHitboxDeduplicatesAndKeepsLethalPriorityWhenSlayerEndsDuringDamage()
    {
        var host = Host("actual hit owner"); host.AddComponent<BoxCollider2D>();
        var slayer = host.AddComponent<PlayerSlayerMode>(); var spawner = Hits(host);
        var box = host.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        Set(box, "slayer", slayer); Set(box, "hitSpark", spawner); box.hitStopper = host.AddComponent<HitStopper>();
        host.SetActive(true); Set(slayer, "_active", true);
        var enemy = Host("actual hit target"); enemy.transform.position = Vector3.left * 3;
        var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var a = enemy.AddComponent<BoxCollider2D>(); var b = enemy.AddComponent<CircleCollider2D>(); enemy.SetActive(true);
        damage.onDamage = () => { damage.dead = true; Set(slayer, "_active", false); };
        box.BeginAttack(1);
        var hit = typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags);
        hit.Invoke(box, new object[] { a }); hit.Invoke(box, new object[] { b });
        Assert.That(damage.hits, Is.EqualTo(1));
        var bursts = Object.FindObjectsOfType<PixelSparkBurst>(); Assert.That(bursts.Length, Is.EqualTo(1));
        Assert.That(bursts[0].Role, Is.EqualTo(PixelSparkBurst.Profile.Lethal));
        Assert.That(bursts[0].ParticleCount, Is.EqualTo(56)); Assert.That(Time.timeScale, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void AuthoredProfilesUseFlatPixelsAndNativeWorldPhysics()
    {
        var roles = new[] { "Normal", "Slayer", "Parry", "Lethal" };
        var budgets = new[] { 24, 32, 40, 56 };
        var minimum = new[] { .45f, .35f, .6f, .75f };
        var maximum = new[] { .65f, .55f, .85f, .95f };
        for (int i = 0; i < roles.Length; i++)
        {
            var burst = Burst(roles[i]); Assert.That(burst.Role.ToString(), Is.EqualTo(roles[i]));
            Assert.That(burst.GetComponent<Animator>(), Is.Null, "particle lifetime owns release");
            burst.Emit(new Vector2(0, 2), 1, 0);
            Assert.That(burst.ParticleCount, Is.EqualTo(budgets[i]));
            Assert.That(burst.Systems.Length, Is.EqualTo(2));
            foreach (var ps in burst.Systems)
            {
                Assert.That(ps.main.loop || ps.main.playOnAwake, Is.False);
                Assert.That(ps.main.useUnscaledTime, Is.True);
                Assert.That(ps.main.simulationSpace, Is.EqualTo(ParticleSystemSimulationSpace.World));
                Assert.That(ps.emission.enabled || ps.trails.enabled, Is.False);
                Assert.That(ps.forceOverLifetime.y.constant, Is.EqualTo(-18));
                Assert.That(ps.collision.type, Is.EqualTo(ParticleSystemCollisionType.Planes));
                Assert.That(ps.collision.bounce.constant, Is.InRange(.3f, .45f));
                Assert.That(ps.collision.dampen.constant, Is.InRange(.35f, .5f));
                Assert.That(ps.colorOverLifetime.color.gradient.mode, Is.EqualTo(GradientMode.Fixed));
                var gradient = ps.colorOverLifetime.color.gradient;
                Assert.That(gradient.Evaluate(.1f), Is.EqualTo(Color.white), "bright opening");
                Assert.That(gradient.Evaluate(.2f), Is.EqualTo(gradient.colorKeys[1].color), "main palette holds through middle life");
                Assert.That(gradient.Evaluate(.9f).a, Is.EqualTo(1f), "solid until expiry");
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                Assert.That(renderer.renderMode, Is.EqualTo(ParticleSystemRenderMode.Billboard));
                Assert.That(renderer.sortingOrder, Is.EqualTo(30));
                Assert.That(renderer.sharedMaterial.shader.name, Is.EqualTo("TypingHero/PixelSpark"));
                Assert.That(renderer.sharedMaterial.mainTexture.filterMode, Is.EqualTo(FilterMode.Point));
                var particles = new ParticleSystem.Particle[56]; int count = ps.GetParticles(particles);
                for (int p = 0; p < count; p++) Assert.That(particles[p].startLifetime, Is.InRange(minimum[i], maximum[i]));
            }
            burst.GetComponent<PooledVFX>().Release();
        }
    }

    [Test, Category("Quick")]
    public void BuilderRetainsInspectorTuningAndPrefabGuid()
    {
#if UNITY_EDITOR
        var prefab = Prefab("Parry"); var burst = prefab.GetComponent<PixelSparkBurst>();
        var so = new UnityEditor.SerializedObject(burst);
        int count = so.FindProperty("count").intValue;
        string path = UnityEditor.AssetDatabase.GetAssetPath(prefab);
        string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
        System.Type builder = null;
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            if ((builder = assembly.GetType("PixelSparkAssets")) != null) break;
        Assert.That(builder, Is.Not.Null);
        try
        {
            so.FindProperty("count").intValue = count + 1; so.ApplyModifiedPropertiesWithoutUndo();
            UnityEditor.AssetDatabase.SaveAssets();
            builder.GetMethod("Prepare", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            so.Update(); Assert.That(so.FindProperty("count").intValue, Is.EqualTo(count + 1));
            Assert.That(UnityEditor.AssetDatabase.AssetPathToGUID(path), Is.EqualTo(guid));
        }
        finally
        {
            so.Update(); so.FindProperty("count").intValue = count;
            so.ApplyModifiedPropertiesWithoutUndo(); UnityEditor.AssetDatabase.SaveAssets();
        }
#else
        Assert.Fail("Builder verification requires Editor PlayMode");
#endif
    }

    [Test, Category("Quick")]
    public void ParryEmitsOnlyAfterPlacementWithIntegerPixelsAndMirroredForce()
    {
        var burst = Burst("Parry"); Assert.That(burst.ParticleCount, Is.Zero);
        foreach (float direction in new[] { -1f, 1f })
        {
            burst.Emit(new Vector2(3, 2), direction, .4f);
            Assert.That(burst.ParticleCount, Is.EqualTo(40));
            Assert.That(burst.GroundY, Is.EqualTo(.4f).Within(.000001f));
            int forward = 0, dots = 0, streaks = 0;
            foreach (var ps in burst.GetComponentsInChildren<ParticleSystem>())
            {
                var particles = new ParticleSystem.Particle[56]; int n = ps.GetParticles(particles);
                for (int i = 0; i < n; i++)
                {
                    var p = particles[i]; Assert.That(p.position, Is.EqualTo(new Vector3(3, 2, 0)));
                    Assert.That(p.rotation, Is.Zero);
                    var size = p.startSize3D * 16;
                    if (size.x == size.y) { dots++; Assert.That(size.x, Is.EqualTo(1f).Or.EqualTo(2f)); }
                    else { streaks++; Assert.That(size.x * size.y, Is.EqualTo(3)); }
                    if (p.velocity.x * direction > 0) forward++;
                    Assert.That(p.startLifetime, Is.InRange(.6f, .85f));
                }
            }
            Assert.That(dots, Is.EqualTo(28)); Assert.That(streaks, Is.EqualTo(12));
            Assert.That(forward, Is.EqualTo(32));
        }
    }

    [Test, Category("Quick")]
    public void ParryParticlesFallBounceAndStayAboveTheirFixedPlane()
    {
        var burst = Burst("Parry"); burst.Emit(new Vector2(0, .5f), 1, 0);
        bool falling = false, bounced = false;
        var previous = new Dictionary<uint, float>();
        foreach (var ps in burst.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Pause(false);
            var particles = new ParticleSystem.Particle[56];
            for (int frame = 0; frame < 55; frame++)
            {
                ps.Simulate(.01f, false, false, false);
                int n = ps.GetParticles(particles);
                for (int i = 0; i < n; i++)
                {
                    var p = particles[i]; Assert.That(p.position.y, Is.GreaterThanOrEqualTo(-.001f));
                    if (p.velocity.y < -.1f) falling = true;
                    if (previous.TryGetValue(p.randomSeed, out float vy) && vy < -.1f && p.velocity.y > .1f)
                    { bounced = true; Assert.That(p.velocity.y, Is.LessThan(-vy)); }
                    previous[p.randomSeed] = p.velocity.y;
                }
            }
        }
        Assert.That(falling, Is.True); Assert.That(bounced, Is.True);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ParryPausesWithoutAgingResumesDuringHitstopAndReleasesOnce()
    {
        var burst = Burst("Parry"); var lease = burst.GetComponent<PooledVFX>();
        int released = 0;
        var pool = new ObjectPool<GameObject>(() => burst.gameObject, actionOnRelease: go => { released++; go.SetActive(false); });
        pool.Get();
        lease.Setup(pool); burst.Emit(new Vector2(0, 2), 1, 0);
        yield return null;
        GamePauseController.SetTutorialReading(true); yield return null;
        var ps = burst.GetComponentsInChildren<ParticleSystem>()[0];
        var particles = new ParticleSystem.Particle[56]; ps.GetParticles(particles);
        float age = particles[0].remainingLifetime; var position = particles[0].position;
        yield return new WaitForSecondsRealtime(.15f);
        ps.GetParticles(particles);
        Assert.That(particles[0].remainingLifetime, Is.EqualTo(age)); Assert.That(particles[0].position, Is.EqualTo(position));
        Assert.That(burst.ParticleCount, Is.EqualTo(40));
        GamePauseController.SetTutorialReading(false); Time.timeScale = .0001f;
        yield return new WaitForSecondsRealtime(1.1f);
        Assert.That(released, Is.EqualTo(1)); Assert.That(burst.ParticleCount, Is.Zero);
        lease.Release(); lease.AnimEvent_Release(); Assert.That(released, Is.EqualTo(1));
        Assert.That(pool.Get(), Is.SameAs(burst.gameObject));
        burst.gameObject.SetActive(true); Assert.That(burst.ParticleCount, Is.Zero);
        burst.Emit(Vector2.one, -1, -.2f); Assert.That(burst.ParticleCount, Is.EqualTo(40));
        lease.Release(); Assert.That(released, Is.EqualTo(2)); Assert.That(burst.ParticleCount, Is.Zero);
        pool.Dispose();
    }

    [Test, Category("Quick")]
    public void ParryContextSetsDirectionBoundsPoolAndClearsOnDisable()
    {
        var host = Host("parry owner"); var spawner = host.AddComponent<ParrySparkVFX_Pooled>();
        var anchor = Host("anchor"); anchor.transform.position = new Vector3(3, 2, 0);
        var target = Host("enemy"); target.transform.position = new Vector3(-2, 0, 0);
        Set(spawner, "sparkPrefab", Prefab("Parry")); Set(spawner, "playerSparkAnchor", anchor.transform);
        Set(spawner, "groundY", .2f); host.SetActive(true);
        var spawn = typeof(ParrySparkVFX_Pooled).GetMethod("Spawn", Flags);
        for (int i = 0; i < 20; i++) spawn.Invoke(spawner, new object[] { new ParrySystem.ParryContext { targetRoot = target.transform } });
        var bursts = Object.FindObjectsOfType<PixelSparkBurst>(); Assert.That(bursts.Length, Is.EqualTo(8));
        foreach (var burst in bursts)
        {
            Assert.That(burst.ParticleCount, Is.EqualTo(40)); Assert.That(burst.GroundY, Is.EqualTo(.2f).Within(.000001f));
            Assert.That(burst.transform.position, Is.EqualTo(anchor.transform.position));
            int left = 0;
            foreach (var ps in burst.Systems)
            {
                var particles = new ParticleSystem.Particle[56]; int count = ps.GetParticles(particles);
                for (int p = 0; p < count; p++) if (particles[p].velocity.x < 0) left++;
            }
            Assert.That(left, Is.EqualTo(32), "parry follows target context rather than anchor scale");
        }
        host.SetActive(false); Assert.That(Object.FindObjectsOfType<PixelSparkBurst>(), Is.Empty);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator TutorialLockAtOrder100StopsTheSameSimulationFrame()
    {
        var burst = Burst("Parry"); burst.Emit(new Vector2(0, 2), 1, 0);
        var driverHost = Host("late reading lock"); var driver = driverHost.AddComponent<PixelSparkReadingLockDriver>();
        driver.source = burst.Systems[0]; driver.arm = true; driverHost.SetActive(true);
        yield return new WaitForSecondsRealtime(.1f);
        var particles = new ParticleSystem.Particle[56]; driver.source.GetParticles(particles);
        Assert.That(driver.arm, Is.False);
        Assert.That(particles[0].remainingLifetime, Is.EqualTo(driver.heldLife));
        Assert.That(particles[0].position, Is.EqualTo(driver.heldPosition));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SlayerExitKeepsItsBurstAndMatchEndClearsEarlierBursts()
    {
        var parry = Burst("Parry"); var slayer = Burst("Slayer"); var lethal = Burst("Lethal");
        parry.Emit(Vector2.up * 2, 1, 0); slayer.Emit(Vector2.up * 2, 1, 0); lethal.Emit(Vector2.up * 2, 1, 0);
        SlayerModeSignals.SetActive(true); SlayerModeSignals.SetActive(false); yield return null;
        Assert.That(slayer.Role, Is.EqualTo(PixelSparkBurst.Profile.Slayer)); Assert.That(slayer.ParticleCount, Is.GreaterThan(0));
        var session = Host("finished session"); var match = session.AddComponent<CombatSession>(); session.SetActive(true);
        match.Complete(true); yield return null; yield return null;
        Assert.That(parry == null && slayer == null, Is.True);
        Assert.That(lethal && lethal.ParticleCount > 0, Is.True, "final lethal remains visible");
        yield return new WaitForSecondsRealtime(1.1f); Assert.That(lethal == null, Is.True);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ResultsClearLethalParticlesWhileOrdinaryPauseKeepsThem()
    {
        var host = Host("results controller"); var pause = host.AddComponent<GamePauseController>();
        Set(pause, "autoGameOverOnPlayerDie", false); host.SetActive(true);
        var burst = Burst("Lethal"); burst.Emit(Vector2.up * 2, 1, 0);
        pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.1f);
        Assert.That(GamePauseController.ResultOverlayVisible, Is.False);
        Assert.That(burst && burst.ParticleCount == 56, Is.True, "ordinary pause preserves the lethal burst");
        pause.ShowThanksSimple(); yield return null; yield return null;
        Assert.That(GamePauseController.ResultOverlayVisible, Is.True);
        Assert.That(burst == null, Is.True, "result freeze cannot strand live particles");
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PoolRootCanBeDestroyedBeforeItsOwnerDuringSceneTeardown()
    {
        var host = Host("teardown owner"); var spawner = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(spawner, "sparkPrefab", Prefab("Parry")); Set(spawner, "playerSparkAnchor", host.transform);
        host.SetActive(true);
        typeof(ParrySparkVFX_Pooled).GetMethod("Spawn", Flags).Invoke(spawner, new object[] { new ParrySystem.ParryContext() });
        Object.Destroy(GameObject.Find("ParryImpactPool")); yield return null;
        Assert.DoesNotThrow(() => host.SetActive(false));
        Assert.That(Object.FindObjectsOfType<PixelSparkBurst>(), Is.Empty);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SceneUnloadReclaimsActiveAndIdleParryInstances()
    {
        var scene = SceneManager.CreateScene("PixelSparkUnloadTest"); scenes.Add(scene);
        var host = Host("scene owner"); SceneManager.MoveGameObjectToScene(host, scene);
        var spawner = host.AddComponent<ParrySparkVFX_Pooled>();
        var anchor = Host("scene anchor"); SceneManager.MoveGameObjectToScene(anchor, scene);
        Set(spawner, "sparkPrefab", Prefab("Parry")); Set(spawner, "playerSparkAnchor", anchor.transform);
        host.SetActive(true);
        var spawn = typeof(ParrySparkVFX_Pooled).GetMethod("Spawn", Flags);
        spawn.Invoke(spawner, new object[] { new ParrySystem.ParryContext() });
        Object.FindObjectOfType<PixelSparkBurst>().GetComponent<PooledVFX>().Release();
        spawn.Invoke(spawner, new object[] { new ParrySystem.ParryContext() });
        yield return SceneManager.UnloadSceneAsync(scene); yield return null;
        Assert.That(Object.FindObjectsOfType<PixelSparkBurst>(), Is.Empty);
        Assert.That(GameObject.Find("ParryImpactPool"), Is.Null);
    }
}
