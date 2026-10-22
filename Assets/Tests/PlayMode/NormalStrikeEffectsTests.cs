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
        Call("ParryClashAssets", "Prepare"); Call("CombatVoxelAssets", "Prepare"); Call("StrikeAccentAssets", "Prepare"); Call("NormalHitSparkAssets", "Prepare");
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
        Set(emitter, "sparkPrefab", Load("NormalHitSpark"));
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
    [TestCase(-1f), TestCase(1f)]
    public void PimenSixEqualFramesFixedContactMirrorAndExactRelease(float direction)
    {
        var emitter = Emitter(); var point = new Vector2(2, 3);
        emitter.SpawnImpact(point, emitter.transform, false, false, null, direction);
        Assert.That(Live(emitter).Count, Is.EqualTo(1));
        var go = Live(emitter)[0]; var burst = go.GetComponent<NormalHitSpark>();
        var image = go.GetComponent<SpriteRenderer>();
        Assert.That(image.sortingOrder, Is.EqualTo(3));
        Assert.That(image.sharedMaterial.shader.name, Is.EqualTo("TypingHero/PixelSpark"));
        Assert.That(image.flipX, Is.EqualTo(direction < 0));
        Assert.That(go.GetComponentsInChildren<ParticleSystem>(true), Is.Empty);
        Vector3 scale = go.transform.localScale;
        Assert.That(scale.x, Is.EqualTo(48f / 16f * 100f / 36f).Within(.00001f));
        Assert.That(scale.y, Is.EqualTo(scale.x));
        for (int i = 0; i < 6; i++)
        {
            Assert.That(image.sprite.name, Is.EqualTo("frame_" + (i + 1)));
            Assert.That(image.color, Is.EqualTo(Color.white));
            Assert.That(go.transform.position, Is.EqualTo((Vector3)point));
            Assert.That(go.transform.rotation, Is.EqualTo(Quaternion.identity));
            Assert.That(go.transform.localScale, Is.EqualTo(scale));
            Assert.That(image.sprite.pivot, Is.EqualTo(new Vector2(24, 24)));
            if (i < 5) burst.Advance(NormalHitSpark.Duration / 6f + (i == 0 ? .000001f : 0));
        }
        burst.Advance(.159999f - burst.Age); Assert.That(go.activeSelf, Is.True);
        burst.Advance(.000002f); Assert.That(Live(emitter), Is.Empty);
        emitter.SpawnImpact(point, emitter.transform, false, false, null, -direction);
        Assert.That(Live(emitter)[0], Is.EqualTo(go));
        Assert.That(image.sprite.name, Is.EqualTo("frame_1"));
        emitter.SpawnImpact(point, emitter.transform, false, false);
        Assert.That(Live(emitter).Count, Is.EqualTo(2), "two confirmed hits each emit once");
        emitter.ClearAccents(); Assert.That(Live(emitter), Is.Empty);
        emitter.SpawnImpact(point, emitter.transform, true, false);
        emitter.SpawnImpact(point, emitter.transform, false, true);
        foreach (var item in Live(emitter)) Assert.That(item.GetComponent<NormalHitSpark>(), Is.Null);
    }
    [UnityTest] public IEnumerator PauseReuseDisableAndSlayerClearNormalSpark()
    {
        var emitter = Emitter(); var binder = emitter.gameObject.AddComponent<NormalStrikeEffects>();
        Set(binder, "emitter", emitter);
        emitter.SpawnImpact(Vector2.up, emitter.transform, false, false);
        var spark = Live(emitter)[0].GetComponent<NormalHitSpark>();
        spark.Advance(.05f); var sprite = spark.GetComponent<SpriteRenderer>().sprite;
        Time.timeScale = 0; yield return null; yield return null;
        Assert.That(spark.Age, Is.EqualTo(.05f)); Assert.That(spark.GetComponent<SpriteRenderer>().sprite, Is.EqualTo(sprite));
        Time.timeScale = 1; GamePauseController.SetTutorialReading(true); yield return null;
        Assert.That(spark.Age, Is.EqualTo(.05f)); GamePauseController.SetTutorialReading(false);
        SlayerModeSignals.SetActive(true); Assert.That(Live(emitter), Is.Empty);
        SlayerModeSignals.SetActive(false);
        emitter.SpawnImpact(Vector2.right, emitter.transform, false, false);
        Assert.That(Live(emitter)[0], Is.EqualTo(spark.gameObject)); Assert.That(spark.Age, Is.Zero);
        emitter.enabled = false; Assert.That(Live(emitter), Is.Empty);
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
#if UNITY_EDITOR
    [Test] public void MissingRequiredContactFailsAuthoringWithoutOverwritingAuthoredData()
    {
        var library = UnityEditor.AssetDatabase.LoadAssetAtPath<SlashLibrary>("Assets/Art/VFX/SlashVolume/SlashLibrary.asset");
        var contact = library.frames[0].contactA;
        Call("SlashVolumeAssets", "Prepare"); Call("SlashVolumeAssets", "Prepare");
        Assert.That(library.frames[0].contactA, Is.EqualTo(contact));
        var broken = Object.Instantiate(library);
        try
        {
            broken.frames[0].hasContact = false;
            var error = Assert.Throws<TargetInvocationException>(() => Call("SlashVolumeAssets", "ValidateContacts", broken));
            Assert.That(error.InnerException, Is.TypeOf<System.InvalidOperationException>());
            StringAssert.Contains("Missing authored slash contact", error.InnerException.Message);
        }
        finally { Object.DestroyImmediate(broken); }
    }
#endif

    [Test] public void IndependentLeaseCapsKeepExistingImpactBudget()
    {
        var emitter = Emitter();
        for (int i = 0; i < 20; i++)
        {
            emitter.SpawnImpact(Vector2.up, emitter.transform, false, false);
            emitter.SpawnAccent(Load("StrikeDust"), Vector3.zero, 1);
        }
        Assert.That(Live(emitter).FindAll(x => x.GetComponent<NormalHitSpark>()).Count, Is.EqualTo(8));
        Assert.That(Live(emitter).FindAll(x => x.GetComponent<StrikeAccentBurst>()).Count, Is.EqualTo(8));
    }
    [Test] public void BuilderTwiceBindsOneListenerAndMissStillEmitsDust()
    {
#if UNITY_EDITOR
        for (int i = 0; i < 2; i++) Call("CombatVoxelAssets", "Prepare");
        for (int i = 0; i < 2; i++) Call("NormalHitSparkAssets", "Prepare");
        Assert.That(Load("NormalHitSpark").GetComponents<NormalHitSpark>().Length, Is.EqualTo(1));
        Assert.That(Load("NormalHitSpark").GetComponents<SpriteRenderer>().Length, Is.EqualTo(1));
        var root = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Characters/Player/Player.prefab");
        try
        {
            for (int i = 0; i < 2; i++) Call("CombatFeedbackBuild", "Configure", root, null, null, null);
            Assert.That(root.GetComponentsInChildren<NormalStrikeEffects>(true).Length, Is.EqualTo(1));
            var emitterData = new UnityEditor.SerializedObject(root.GetComponentInChildren<HitSparkSpawner>(true));
            Assert.That(emitterData.FindProperty("sparkPrefab").objectReferenceValue, Is.EqualTo(Load("NormalHitSpark")));
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
