using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class NormalStrikeEffectsTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    readonly List<GameObject> owned = new();
    static void Set(object target, string field, object value) => target.GetType().GetField(field, Flags).SetValue(target, value);
    static object Call(string type, string method, params object[] args)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var found = assembly.GetType(type);
            if (found != null) return found.GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, args);
        }
        throw new System.Exception(type);
    }
    [SetUp] public void Setup()
    {
        Time.timeScale = 1; GamePauseController.SetTutorialReading(false);
        Call("ParryClashAssets", "Prepare"); Call("CombatVoxelAssets", "Prepare"); Call("StrikeAccentAssets", "Prepare");
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        SlayerModeSignals.SetActive(false); Time.timeScale = 1; GamePauseController.SetTutorialReading(false);
        foreach (var go in owned) if (go) Object.Destroy(go);
        owned.Clear(); yield return null;
    }
    GameObject Load(string name)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/" + name + ".prefab");
#else
        return null;
#endif
    }
    HitSparkSpawner Emitter()
    {
        var go = new GameObject("strike test"); go.SetActive(false); owned.Add(go);
        var emitter = go.AddComponent<HitSparkSpawner>();
        Set(emitter, "sparkPrefab", Load("Normal")); Set(emitter, "normalCorePrefab", Load("StrikeCore"));
        Set(emitter, "slayerPrefabs", new[] { Load("Slayer") }); Set(emitter, "lethalPrefab", Load("Lethal"));
        go.SetActive(true); return emitter;
    }
    static List<GameObject> Live(HitSparkSpawner emitter) => (List<GameObject>)typeof(HitSparkSpawner).GetField("active", Flags).GetValue(emitter);
    [TestCase(-1f), TestCase(1f)]
    public void DustStaysAboveFloorMovesBackwardAndExpires(float direction)
    {
        var emitter = Emitter(); Vector3 point = new Vector3(3, -2, 0);
        emitter.SpawnAccent(Load("StrikeDust"), point, direction);
        var burst = Live(emitter)[0].GetComponent<StrikeAccentBurst>();
        var particles = new ParticleSystem.Particle[3];
        for (int frame = 0; frame < 18; frame++)
        {
            int count = burst.GetComponent<ParticleSystem>().GetParticles(particles);
            Assert.That(count, Is.EqualTo(3));
            foreach (var p in particles)
            {
                Assert.That(p.position.y - p.startSize3D.y / 2, Is.GreaterThanOrEqualTo(point.y - .0001f));
                Assert.That((p.position.x - point.x) * direction, Is.LessThanOrEqualTo(0));
                Assert.That(p.position.y, Is.LessThan(point.y + .1f));
            }
            burst.Advance(.01f);
        }
        burst.Advance(.001f); Assert.That(burst.ParticleCount, Is.Zero);
    }
    [Test] public void CoreUsesExactImpactPositionAndOnlyNonlethalNormalRoute()
    {
        var emitter = Emitter(); var point = new Vector2(2, 3);
        emitter.SpawnImpact(point, emitter.transform, false, false);
        Assert.That(Live(emitter).Count, Is.EqualTo(2));
        var core = Live(emitter)[1].GetComponent<StrikeAccentBurst>();
        var p = new ParticleSystem.Particle[1]; core.GetComponent<ParticleSystem>().GetParticles(p);
        Assert.That(p[0].position, Is.EqualTo((Vector3)point));
        Assert.That(p[0].startSize3D.x, Is.EqualTo(4f/16));
        core.Advance(.039f); Assert.That(core.ParticleCount, Is.EqualTo(1));
        core.Advance(.002f); Assert.That(core.ParticleCount, Is.Zero);
        emitter.ClearAccents();
        emitter.SpawnImpact(point, emitter.transform, true, false);
        emitter.SpawnImpact(point, emitter.transform, false, true);
        emitter.SpawnImpact(point, emitter.transform, true, true);
        foreach (var item in Live(emitter)) Assert.That(item.GetComponent<StrikeAccentBurst>(), Is.Null);
    }
    [UnityTest] public IEnumerator PauseReuseDisableAndSlayerClearAccentsWithoutClearingFragments()
    {
        var emitter = Emitter(); var binder = emitter.gameObject.AddComponent<NormalStrikeEffects>();
        Set(binder, "emitter", emitter);
        emitter.SpawnImpact(Vector2.up, emitter.transform, false, false);
        var core = Live(emitter)[1].GetComponent<StrikeAccentBurst>();
        Time.timeScale = 0; yield return null; yield return null;
        Assert.That(core.Age, Is.Zero); Assert.That(core.ParticleCount, Is.EqualTo(1));
        Time.timeScale = 1; GamePauseController.SetTutorialReading(true); yield return null;
        Assert.That(core.Age, Is.Zero); GamePauseController.SetTutorialReading(false);
        SlayerModeSignals.SetActive(true);
        Assert.That(Live(emitter).Count, Is.EqualTo(1)); Assert.That(core.ParticleCount, Is.Zero);
        SlayerModeSignals.SetActive(false);
        emitter.SpawnAccent(Load("StrikeCore"), Vector3.right, 1);
        Assert.That(Live(emitter)[1], Is.EqualTo(core.gameObject)); Assert.That(core.Age, Is.Zero);
        emitter.enabled = false; Assert.That(Live(emitter), Is.Empty);
    }
    [Test] public void IndependentLeaseCapsKeepExistingImpactBudget()
    {
        var emitter = Emitter();
        for (int i = 0; i < 20; i++)
        {
            emitter.SpawnImpact(Vector2.up, emitter.transform, false, false);
            emitter.SpawnAccent(Load("StrikeDust"), Vector3.zero, 1);
        }
        Assert.That(Live(emitter).FindAll(x => x.GetComponent<VoxelImpactBurst>()).Count, Is.EqualTo(8));
        Assert.That(Live(emitter).FindAll(x => x.GetComponent<StrikeAccentBurst>()).Count, Is.EqualTo(8));
    }
    [Test] public void BuilderTwiceBindsOneListenerAndMissStillEmitsDust()
    {
#if UNITY_EDITOR
        var root = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Characters/Player/Player.prefab");
        try
        {
            for (int i = 0; i < 2; i++) Call("CombatFeedbackBuild", "Configure", root, null, null, null);
            Assert.That(root.GetComponentsInChildren<NormalStrikeEffects>(true).Length, Is.EqualTo(1));
            var binder = root.GetComponentInChildren<NormalStrikeEffects>(true);
            var so = new UnityEditor.SerializedObject(binder);
            foreach (string field in new[] { "attackEvents", "slayer", "emitter", "body", "dustPrefab" })
                Assert.That(so.FindProperty(field).objectReferenceValue, Is.Not.Null, field);
        }
        finally { UnityEditor.PrefabUtility.UnloadPrefabContents(root); }
#endif
        var emitter = Emitter(); emitter.gameObject.SetActive(false);
        var events = emitter.gameObject.AddComponent<PlayerAttackEvents>();
        var body = emitter.gameObject.AddComponent<BoxCollider2D>();
        events.hitbox = emitter.gameObject.AddComponent<PlayerWeaponHitbox>();
        var accent = emitter.gameObject.AddComponent<NormalStrikeEffects>();
        Set(accent,"attackEvents",events); Set(accent,"emitter",emitter); Set(accent,"body",body); Set(accent,"dustPrefab",Load("StrikeDust"));
        emitter.gameObject.SetActive(true); events.OnAttackStart();
        Assert.That(Live(emitter).Count, Is.EqualTo(1));
        Assert.That(Live(emitter)[0].GetComponent<StrikeAccentBurst>().ParticleCount, Is.EqualTo(3));
        Assert.That(Live(emitter)[0].transform.position.x, Is.EqualTo(-1f / 16f), "one world pixel behind the planted-foot landmark, not a fraction of collider width");
        events.OnAttackStart(); Assert.That(Live(emitter).Count, Is.EqualTo(2));
        accent.enabled = false; accent.enabled = true; events.OnAttackStart();
        Assert.That(Live(emitter).Count, Is.EqualTo(1));
    }
}
