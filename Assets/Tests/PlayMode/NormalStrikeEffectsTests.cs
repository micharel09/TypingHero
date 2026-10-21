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
        Assert.That(core.GetComponent<ParticleSystemRenderer>().sortingOrder, Is.EqualTo(4));
        var p = new ParticleSystem.Particle[1]; core.GetComponent<ParticleSystem>().GetParticles(p);
        Assert.That(p[0].position, Is.EqualTo((Vector3)point));
        Assert.That(p[0].startSize3D.x, Is.EqualTo(8f/16));
        core.Advance(.039f); Assert.That(core.ParticleCount, Is.EqualTo(1));
        core.Advance(.011f); core.GetComponent<ParticleSystem>().GetParticles(p);
        Assert.That(p[0].startColor.a / 255f, Is.EqualTo(.5f).Within(.005f));
        core.Advance(.011f); Assert.That(core.ParticleCount, Is.Zero);
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
    [TestCase(-1f), TestCase(1f)]
    public void NormalRaysHoldAtContactThenSeparateShrinkAndExpire(float direction)
    {
        var go = Object.Instantiate(Load("Normal")); owned.Add(go); go.SetActive(true);
        var burst = go.GetComponent<VoxelImpactBurst>();
        var point = new Vector3(2, 3, 0);
        burst.Emit(point, direction, 0, 7, Color.clear);
        var p = new ParticleSystem.Particle[14];
        var initial = new ParticleSystem.Particle[6];
        for (int group = 0; group < 2; group++)
        {
            var system = burst.Systems[group];
            int n = system.GetParticles(p);
            Assert.That(n, Is.EqualTo(3));
            for (int j = 0; j < n; j++)
            {
                int ray = group + j * 2;
                initial[ray] = p[j];
                float length = ray == 2 ? 20 : ray == 5 ? 12 : 14;
                float width = ray == 0 || ray == 4 ? 2 : 3;
                float angle = (ray < 5 ? -50 + ray * 25 : 180) * Mathf.Deg2Rad;
                var axis = new Vector3(direction * Mathf.Cos(angle), Mathf.Sin(angle), 0);
                Assert.That(p[j].startSize3D, Is.EqualTo(new Vector3(length / 16f, width / 16, 1f / 16)));
                Assert.That(Vector3.Distance(p[j].position - axis * (length / 32f), point), Is.LessThan(.0001f), "all ray bases meet at the supplied contact");
                Assert.That(p[j].velocity, Is.EqualTo(Vector3.zero), "peak is held for 40ms");
            }
        }
        burst.Advance(.039f);
        for (int group = 0; group < 2; group++)
        {
            burst.Systems[group].GetParticles(p);
            for (int j = 0; j < 3; j++)
            {
                var peak = initial[group + j * 2];
                Assert.That(p[j].position, Is.EqualTo(peak.position));
                Assert.That(p[j].startSize3D, Is.EqualTo(peak.startSize3D));
            }
        }
        burst.Advance(.021f);
        for (int group = 0; group < 2; group++)
        {
            burst.Systems[group].GetParticles(p);
            for (int j = 0; j < 3; j++)
            {
                var peak = initial[group + j * 2];
                Assert.That(p[j].velocity.magnitude, Is.EqualTo(2.5f).Within(.0001f));
                Assert.That(Vector3.Distance(p[j].position, peak.position + p[j].velocity * .02f), Is.LessThan(.0001f));
                Assert.That(p[j].rotation3D, Is.EqualTo(peak.rotation3D));
                Assert.That(p[j].startSize3D.x, Is.LessThan(peak.startSize3D.x));
            }
        }
        burst.Advance(.02f); burst.Systems[0].GetParticles(p);
        Assert.That(p[0].startColor.a / 255f, Is.EqualTo(.5f).Within(.005f));
        burst.Advance(.021f); Assert.That(burst.ParticleCount, Is.EqualTo(4));
        burst.Advance(.02f); Assert.That(burst.ParticleCount, Is.EqualTo(2));
        burst.Advance(.02f); Assert.That(burst.ParticleCount, Is.EqualTo(1));
        burst.Advance(.02f); Assert.That(burst.ParticleCount, Is.Zero);
    }
    [UnityTest] public IEnumerator SlashKeepsCelMeshesAndRestoresMaterialAcrossSlayer()
    {
#if UNITY_EDITOR
        Call("SlashVolumeAssets", "Prepare");
        var library = UnityEditor.AssetDatabase.LoadAssetAtPath<SlashLibrary>("Assets/Art/VFX/SlashVolume/SlashLibrary.asset");
        var material = (Material)Call("SlashVolumeAssets", "NormalPlayerMaterial");
        var go = new GameObject("normal player slash"); go.SetActive(false); owned.Add(go);
        var source = go.AddComponent<SpriteRenderer>(); source.sprite = library.frames[0].sprite;
        var slash = go.AddComponent<SlashVolume>(); Set(slash,"source",source); Set(slash,"library",library); Set(slash,"normalPlayerMaterial",material);
        go.SetActive(true); yield return null;
        var meshes = go.GetComponentsInChildren<MeshFilter>();
        Assert.That(meshes[0].sharedMesh, Is.EqualTo(library.frames[0].front));
        Assert.That(meshes[1].sharedMesh, Is.EqualTo(library.frames[0].back));
        foreach (bool active in new[] { false, true, false })
        {
            SlayerModeSignals.SetActive(active); yield return null;
            foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>())
                Assert.That(renderer.sharedMaterial, Is.EqualTo(active ? library.material : material));
        }
        Set(slash,"normalPlayerMaterial",null); yield return null;
        foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>())
            Assert.That(renderer.sharedMaterial, Is.EqualTo(library.material), "enemy retains the shared material");
#else
        yield return null;
#endif
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
        for (int i = 0; i < 2; i++) Call("CombatVoxelAssets", "Prepare");
        var rayMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/VFX/CombatVoxel/NormalRay.asset");
        var rayMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/CombatVoxel/NormalImpact.mat");
        Assert.That(rayMesh, Is.Not.Null);
        Assert.That(rayMaterial.shader.name, Is.EqualTo("TypingHero/NormalImpact"));
        foreach (var renderer in Load("Normal").GetComponentsInChildren<ParticleSystemRenderer>(true))
        {
            Assert.That(renderer.mesh, Is.EqualTo(rayMesh));
            Assert.That(renderer.sharedMaterial, Is.EqualTo(rayMaterial));
            Assert.That(renderer.sortingOrder, Is.EqualTo(3), "the impact must draw above the active attack trail at order2");
        }
        foreach (string role in new[] { "Slayer", "Lethal" })
            foreach (var renderer in Load(role).GetComponentsInChildren<ParticleSystemRenderer>(true))
                Assert.That(renderer.sharedMaterial, Is.Not.EqualTo(rayMaterial));
        var root = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Characters/Player/Player.prefab");
        try
        {
            for (int i = 0; i < 2; i++) Call("CombatFeedbackBuild", "Configure", root, null, null, null);
            Assert.That(root.GetComponentsInChildren<NormalStrikeEffects>(true).Length, Is.EqualTo(1));
            var slash = root.GetComponentInChildren<SlashVolume>(true);
            var slashData = new UnityEditor.SerializedObject(slash);
            var normal = (Material)slashData.FindProperty("normalPlayerMaterial").objectReferenceValue;
            Assert.That(normal, Is.Not.Null);
            Assert.That(normal.GetVector("_Tint"), Is.EqualTo(new Vector4(.75f,.75f,.75f,.55f)));
            Assert.That(normal.GetFloat("_Emission"), Is.EqualTo(.05f));
            Assert.That(normal.shader.name, Is.EqualTo("TypingHero/NormalPlayerSlash"));
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
