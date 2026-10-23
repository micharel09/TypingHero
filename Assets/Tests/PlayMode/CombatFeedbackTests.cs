using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class FeedbackDamageTarget : MonoBehaviour, IDamageable
{
    public bool dead;
    public int hits, amount;
    public Vector2 point;
    public Action onDamage;
    public bool IsDead => dead;
    public void TakeDamage(int damage, Vector2 point) { hits++; amount += damage; this.point = point; onDamage?.Invoke(); }
}

public sealed class CombatFeedbackTests
{
    readonly List<UnityEngine.Object> owned = new();
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Set(object o, string f, object v) => o.GetType().GetField(f, Flags).SetValue(o, v);
    static T Get<T>(object o, string f) => (T)o.GetType().GetField(f, Flags).GetValue(o);
    T Own<T>(T value) where T : UnityEngine.Object { owned.Add(value); return value; }
    GameObject Host(string name) { var go = Own(new GameObject(name)); go.SetActive(false); return go; }
    GameObject Spark(string name)
    {
        var go = Host(name); go.AddComponent<SpriteRenderer>(); go.AddComponent<PooledVFX>(); return go;
    }
    static PooledVFX[] Active() => UnityEngine.Object.FindObjectsOfType<PooledVFX>();
    static void Hit(PlayerWeaponHitbox box, Collider2D target)
        => typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags).Invoke(box, new object[] { target });

    [UnityTearDown] public IEnumerator Cleanup()
    {
        Time.timeScale = 1; AudioListener.pause = false; SlayerModeSignals.SetActive(false);
        foreach (var o in owned) if (o) UnityEngine.Object.Destroy(o);
        owned.Clear(); yield return null;
        Assert.That(Active(), Is.Empty);
    }

#if UNITY_EDITOR
    [UnityTest, Category("Quick")]
    public IEnumerator ImpactBanksLoadBeforeFirstHitWithoutPlayingOrConsumingRandom()
    {
        var medium = new AudioClip[3]; var heavy = new AudioClip[3];
        for (int i = 0; i < 3; i++)
        {
            string root = "Assets/Audio/SFX/Clips/KenneyImpact/impactWood_";
            medium[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(root + $"medium_{i:000}.ogg");
            heavy[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(root + $"heavy_{i:000}.ogg");
            Assert.That(medium[i].UnloadAudioData(), Is.True);
            Assert.That(heavy[i].UnloadAudioData(), Is.True);
            Assert.That(medium[i].loadState, Is.EqualTo(AudioDataLoadState.Unloaded));
            Assert.That(heavy[i].loadState, Is.EqualTo(AudioDataLoadState.Unloaded));
        }
        var normal = Own(ScriptableObject.CreateInstance<SfxEvent>()); Set(normal, "clips", medium);
        var slayer = Own(ScriptableObject.CreateInstance<SfxEvent>()); Set(slayer, "clips", medium);
        var lethal = Own(ScriptableObject.CreateInstance<SfxEvent>()); Set(lethal, "clips", heavy);
        var owner = Host("preload impact banks"); var spawner = owner.AddComponent<HitSparkSpawner>();
        Set(spawner, "normalImpact", normal); Set(spawner, "slayerImpact", slayer); Set(spawner, "lethalImpact", lethal);
        var random = UnityEngine.Random.state;
        int voices = UnityEngine.Object.FindObjectsOfType<AudioSource>().Length;
        owner.SetActive(true);
        Assert.That(UnityEngine.Random.state, Is.EqualTo(random));
        Assert.That(UnityEngine.Object.FindObjectsOfType<AudioSource>().Length, Is.EqualTo(voices));
        Assert.That(normal.CanPlayNow() && slayer.CanPlayNow() && lethal.CanPlayNow(), Is.True);
        float deadline = Time.realtimeSinceStartup + 3;
        bool loading;
        do
        {
            loading = false;
            foreach (var clip in medium) loading |= clip.loadState == AudioDataLoadState.Loading;
            foreach (var clip in heavy) loading |= clip.loadState == AudioDataLoadState.Loading;
            if (loading) yield return null;
        } while (loading && Time.realtimeSinceStartup < deadline);
        foreach (var clip in medium) Assert.That(clip.loadState, Is.EqualTo(AudioDataLoadState.Loaded));
        foreach (var clip in heavy) Assert.That(clip.loadState, Is.EqualTo(AudioDataLoadState.Loaded));
    }

    [TestCase("Assets/Characters/Player/attack1.anim", 1f, false)]
    [TestCase("Assets/Characters/Player/attack1.anim", -1.5f, true)]
    [TestCase("Assets/Characters/Player/parry_followup.anim", 1f, false)]
    [TestCase("Assets/Characters/Player/parry_followup.anim", -1.5f, true)]
    public void ActualAnimationEventConfirmsDamageAndBrightSparkAtAuthoredContact(string path, float scale, bool flip)
    {
        var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        var opening = System.Array.Find(UnityEditor.AnimationUtility.GetAnimationEvents(clip), e => e.functionName == "OnAttackStartCel");
        Assert.That(opening, Is.Not.Null);
        if (path.Contains("parry")) Assert.That(opening.time, Is.EqualTo(.1f).Within(.000001));
        var cel = (Sprite)opening.objectReferenceParameter;
        var library = UnityEditor.AssetDatabase.LoadAssetAtPath<SlashLibrary>("Assets/Art/VFX/SlashVolume/SlashLibrary.asset");
        var player = Host("animated contact"); player.transform.localScale = new Vector3(scale, 1.2f, 1);
        player.AddComponent<BoxCollider2D>().size = Vector2.one * 20;
        var source = player.AddComponent<SpriteRenderer>(); source.flipX = flip;
        var box = player.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        Set(box, "slashLibrary", library); Set(box, "slashSource", source);
        var events = player.AddComponent<PlayerAttackEvents>(); events.hitbox = box;
        var emitter = player.AddComponent<HitSparkSpawner>();
        Set(emitter, "sparkPrefab", UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/NormalHitSpark.prefab"));
        Set(box, "hitSpark", emitter);
        var animator = player.AddComponent<Animator>();
        var controller = Own(new UnityEditor.Animations.AnimatorController()); controller.AddLayer("Base Layer");
        var machine = controller.layers[0].stateMachine;
        var state = machine.AddState("swing"); state.motion = clip; machine.defaultState = state;
        animator.runtimeAnimatorController = controller;
        player.AddComponent<ParrySystem>().logs = false;
        var enemy = Host("contact recipient"); var target = enemy.AddComponent<FeedbackDamageTarget>();
        var collider = enemy.AddComponent<BoxCollider2D>(); collider.size = new Vector2(1, 2);
        var row = System.Array.Find(library.frames, f => f.sprite == cel);
        var local = ((row.contactA + row.contactB) * .5f - cel.pivot) / cel.pixelsPerUnit;
        if (flip) local.x = -local.x;
        enemy.transform.position = player.transform.TransformPoint(local) + new Vector3(.15f, .1f, 0);
        enemy.SetActive(true); player.SetActive(true); Physics2D.SyncTransforms();
        Assert.That(library.TryContact(cel, source, collider, out var expected), Is.True);
        target.onDamage = () => source.sprite = null; // Hurt/Die must not change the already captured point.
        animator.Update(0); animator.Update(.0001f);
        if (!path.Contains("parry")) Assert.That(target.hits, Is.EqualTo(1), "first displayed slash cel must already confirm damage");
        animator.Update(opening.time + .001f);
        Assert.That(target.hits, Is.EqualTo(1)); Assert.That(target.amount, Is.EqualTo(10));
        Assert.That(Vector2.Distance(target.point, expected), Is.LessThan(.0001f));
        Assert.That(box.LastHitFrame, Is.EqualTo(Time.frameCount));
        var spark = UnityEngine.Object.FindObjectOfType<NormalHitSpark>(); Assert.That(spark, Is.Not.Null);
        Assert.That(spark.GetComponent<SpriteRenderer>().sprite.name, Is.EqualTo("frame_1"));
        Assert.That(Vector2.Distance(spark.transform.position, expected), Is.LessThan(.0001f));
        Hit(box, collider); Assert.That(target.hits, Is.EqualTo(1));
    }
#endif

    [Test, Category("Quick")]
    public void ContainedContactSegmentUsesItsCentreInsteadOfLowEndpoint()
    {
        var texture = Own(new Texture2D(16, 16));
        var sprite = Own(Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero, 1));
        var library = Own(ScriptableObject.CreateInstance<SlashLibrary>());
        library.frames = new[] { new SlashLibrary.Frame { sprite = sprite, hasContact = true,
            contactA = new Vector2(10, 0), contactB = new Vector2(10, 10) } };
        var source = Host("contact source").AddComponent<SpriteRenderer>(); source.sprite = sprite;
        var target = Host("contact geometry"); target.transform.position = new Vector3(10, 5, 0);
        var collider = target.AddComponent<BoxCollider2D>(); collider.size = new Vector2(2, 12);
        target.SetActive(true); Physics2D.SyncTransforms();
        Assert.That(library.TryContact(sprite, source, collider, out var point), Is.True);
        Assert.That(Vector2.Distance(point, new Vector2(10, 5)), Is.LessThan(.0001f));
    }

    [Test, Category("Quick")]
    public void OpeningRespectsPairIgnoreAndTriggerStillAcceptsLaterEntry()
    {
        var player = Host("ignore player"); var ownCollider = player.AddComponent<BoxCollider2D>();
        var box = player.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        var events = player.AddComponent<PlayerAttackEvents>(); events.hitbox = box; player.SetActive(true);
        var enemy = Host("ignored enemy"); var target = enemy.AddComponent<FeedbackDamageTarget>();
        var collider = enemy.AddComponent<BoxCollider2D>(); enemy.SetActive(true);
        Physics2D.IgnoreCollision(ownCollider, collider); events.OnAttackStartCel(null); Assert.That(target.hits, Is.Zero);
        Physics2D.IgnoreCollision(ownCollider, collider, false); Hit(box, collider); Assert.That(target.hits, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void OpeningRespectsPhysicsLayerIgnoreDespiteTargetMask()
    {
        var player = Host("layer player"); player.AddComponent<BoxCollider2D>();
        var box = player.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1 << 8;
        var events = player.AddComponent<PlayerAttackEvents>(); events.hitbox = box; player.SetActive(true);
        var enemy = Host("layer enemy"); enemy.layer = 8;
        var target = enemy.AddComponent<FeedbackDamageTarget>(); enemy.AddComponent<BoxCollider2D>(); enemy.SetActive(true);
        bool before = Physics2D.GetIgnoreLayerCollision(0, 8);
        try
        {
            Physics2D.IgnoreLayerCollision(0, 8, true); events.OnAttackStartCel(null); Assert.That(target.hits, Is.Zero);
            Physics2D.IgnoreLayerCollision(0, 8, false); events.OnAttackStartCel(null); Assert.That(target.hits, Is.EqualTo(1));
        }
        finally { Physics2D.IgnoreLayerCollision(0, 8, before); }
    }

    [Test, Category("Quick")]
    public void AnimationOpeningConfirmsOverlapImmediatelyAndDeduplicatesLaterTrigger()
    {
        var player = Host("opening player"); player.AddComponent<BoxCollider2D>();
        var box = player.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        var events = player.AddComponent<PlayerAttackEvents>(); events.hitbox = box;
        player.SetActive(true);
        var enemy = Host("opening enemy"); var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var a = enemy.AddComponent<BoxCollider2D>(); var b = enemy.AddComponent<CircleCollider2D>();
        enemy.SetActive(true);
        events.OnAttackStart();
        Assert.That(damage.hits, Is.Zero, "Slayer entrypoint retains trigger cadence");
        events.OnAttackStartCel(null);
        Assert.That(damage.hits, Is.EqualTo(1), "animation callback must confirm before a physics step");
        Hit(box, a); Hit(box, b);
        Assert.That(damage.hits, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void HitboxDeduplicatesTargetsAndRejectsClosedDeadWrongLayerAndLockedMatch()
    {
        var player = Host("player"); player.AddComponent<BoxCollider2D>();
        var box = player.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        var spawner = player.AddComponent<HitSparkSpawner>();
        Set(spawner, "sparkPrefab", Spark("normal")); Set(box, "hitSpark", spawner);
        var cameraImpact = player.AddComponent<CombatCameraImpact>(); Set(box, "cameraImpact", cameraImpact);
        var cameraProfile = Own(UnityEngine.Object.Instantiate(LightingTestAssets.Definition.cameraImpact));
        foreach (var preset in cameraProfile.presets) preset.units = CombatCameraProfile.ShakeUnits.WorldUnits;
        cameraImpact.Bind(cameraProfile);
        player.SetActive(true);
        var enemy = Host("enemy"); var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var a = enemy.AddComponent<BoxCollider2D>(); var b = enemy.AddComponent<CircleCollider2D>(); enemy.SetActive(true);
        Hit(box, a); Assert.That(damage.hits, Is.Zero);
        box.BeginAttack(1); Hit(box, a); Hit(box, b); Assert.That(damage.hits, Is.EqualTo(1));
        cameraImpact.Advance(.03f);
        Assert.That(cameraImpact.NormalCount, Is.EqualTo(1));
        Assert.That(Active().Length, Is.EqualTo(1));
        box.EndAttack(); Hit(box, a); Assert.That(damage.hits, Is.EqualTo(1));
        box.BeginAttack(2); box.enabled = false; Hit(box, a); Assert.That(damage.hits, Is.EqualTo(1));
        box.enabled = true; damage.dead = true; Hit(box, a); Assert.That(damage.hits, Is.EqualTo(1));
        damage.dead = false; enemy.layer = 2; Hit(box, a); Assert.That(damage.hits, Is.EqualTo(1));
        enemy.layer = 0;
        var session = Own(new GameObject("session")).AddComponent<CombatSession>(); session.Complete(true);
        Hit(box, a); Assert.That(damage.hits, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void LethalSlayerUsesCapturedProfileAndDoesNotRequestHitstop()
    {
        var player = Host("player"); player.AddComponent<BoxCollider2D>();
        var slayer = player.AddComponent<PlayerSlayerMode>();
        var spawner = player.AddComponent<HitSparkSpawner>();
        Set(spawner, "sparkPrefab", Spark("normal")); Set(spawner, "lethalPrefab", Spark("lethal"));
        var box = player.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        Set(box, "hitSpark", spawner); Set(box, "slayer", slayer);
        box.hitStopper = player.AddComponent<HitStopper>();
        var cameraImpact = player.AddComponent<CombatCameraImpact>(); Set(box, "cameraImpact", cameraImpact);
        var cameraProfile = Own(UnityEngine.Object.Instantiate(LightingTestAssets.Definition.cameraImpact));
        foreach (var preset in cameraProfile.presets) preset.units = CombatCameraProfile.ShakeUnits.WorldUnits;
        cameraImpact.Bind(cameraProfile);
        player.SetActive(true); Set(slayer, "_active", true);
        var enemy = Host("enemy"); var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var collider = enemy.AddComponent<BoxCollider2D>(); enemy.SetActive(true);
        damage.onDamage = () => { damage.dead = true; Set(slayer, "_active", false); };
        box.BeginAttack(1); Hit(box, collider); Hit(box, collider);
        cameraImpact.Advance(.04f);
        Assert.That(cameraImpact.LethalCount, Is.EqualTo(1));
        Assert.That(cameraImpact.Zoom, Is.EqualTo(.02f).Within(.00001f));
        Assert.That(damage.hits, Is.EqualTo(1));
        Assert.That(Active().Length, Is.EqualTo(1)); Assert.That(Active()[0].name, Does.StartWith("lethal"));
        Assert.That(Time.timeScale, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void ImpactProfilesRotateAndReuseWithoutScaleOrColorAccumulation()
    {
        var host = Host("pool"); var spawner = host.AddComponent<HitSparkSpawner>();
        var normal = Spark("normal"); normal.transform.localScale = Vector3.one * 2;
        Set(spawner, "sparkPrefab", normal);
        Set(spawner, "slayerPrefabs", new[] { Spark("s0"), Spark("s1"), Spark("s2"), Spark("s3") });
        Set(spawner, "angleJitter", Vector2.zero); Set(spawner, "flipXChance", 0f);
        Set(spawner, "verticalJitter", Vector2.zero); host.SetActive(true);
        for (int i = 0; i < 8; i++)
        {
            spawner.SpawnImpact(Vector2.one, host.transform, true, false);
            Assert.That(Active().Length, Is.EqualTo(1));
            var effect = Active()[0]; Assert.That(effect.name, Does.StartWith("s" + i % 4));
            Assert.That(effect.transform.position, Is.EqualTo(Vector3.one - Vector3.forward));
            effect.AnimEvent_Release(); effect.AnimEvent_Release();
        }
        spawner.SpawnImpact(Vector2.zero, host.transform, false, false);
        var first = Active()[0]; int id = first.GetInstanceID();
        first.transform.localScale *= 8; first.GetComponent<SpriteRenderer>().color = Color.red;
        first.AnimEvent_Release();
        spawner.SpawnImpact(Vector2.zero, host.transform, false, false);
        Assert.That(Active()[0].GetInstanceID(), Is.EqualTo(id));
        Assert.That(first.transform.localScale, Is.EqualTo(Vector3.one * 2));
        Assert.That(first.GetComponent<SpriteRenderer>().color, Is.EqualTo(Color.white));
        host.transform.position = Vector3.one * 10;
        Assert.That(first.transform.position, Is.EqualTo(Vector3.zero));
        host.SetActive(false); Assert.That(Active(), Is.Empty);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator AfterimagesHoldScheduleAndFadeOnPauseAndCancelPendingBurst()
    {
        var host = Host("afterimages"); var source = host.AddComponent<SpriteRenderer>();
        var texture = Own(new Texture2D(8, 8));
        source.sprite = Own(Sprite.Create(texture, new Rect(0, 0, 8, 8), Vector2.one * .5f));
        var pool = host.AddComponent<SlayerAfterimagePool>();
        Set(pool, "source", source); Set(pool, "forceUnlit", false); Set(pool, "useComboTint", false);
        Set(pool, "useSaturationBoost", false); host.SetActive(true);
        pool.SpawnBurstFromCurrentFrame();
        var active = Get<HashSet<GameObject>>(pool, "activeImages");
        Assert.That(active.Count, Is.EqualTo(1));
        SpriteRenderer image = null; foreach (var go in active) image = go.GetComponent<SpriteRenderer>();
        Assert.That(image.color.a, Is.LessThanOrEqualTo(.4f));
        Color before = image.color;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.1f);
        Assert.That(active.Count, Is.EqualTo(1)); Assert.That(image.color, Is.EqualTo(before));
        Time.timeScale = 1;
        float deadline = Time.realtimeSinceStartup + .2f;
        while (active.Count < 3 && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(active.Count, Is.EqualTo(3));
        pool.CancelBurst(); Assert.That(active.Count, Is.Zero);
        SlayerModeSignals.SetActive(true);
        pool.SpawnBurstFromCurrentFrame(); SlayerModeSignals.SetActive(false);
        yield return new WaitForSecondsRealtime(.1f); Assert.That(active.Count, Is.Zero);
        pool.SpawnBurstFromCurrentFrame();
        var session = Own(new GameObject("finished session")).AddComponent<CombatSession>();
        session.Complete(false); yield return null; Assert.That(active.Count, Is.Zero);
        UnityEngine.Object.Destroy(session.gameObject); yield return null;
        pool.SpawnBurstFromCurrentFrame(); host.SetActive(false);
        yield return new WaitForSecondsRealtime(.1f); Assert.That(active.Count, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ActualAnimationEventReturnsVfxDuringHitstopButHoldsOnPause()
    {
#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/Pimen/Generated/Parry.prefab");
        var host = Host("real pool"); var spawner = host.AddComponent<HitSparkSpawner>();
        Set(spawner, "sparkPrefab", prefab); host.SetActive(true);
        spawner.Spawn(Vector2.zero, host.transform, null);
        var effect = Active()[0]; var animator = effect.GetComponent<Animator>();
        yield return null; Time.timeScale = 0; yield return null;
        float frozen = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        yield return new WaitForSecondsRealtime(.1f);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, Is.EqualTo(frozen));
        Time.timeScale = .0001f;
        float deadline = Time.realtimeSinceStartup + 2;
        while (effect.gameObject.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(effect.gameObject.activeSelf, Is.False);
        Time.timeScale = 1; spawner.Spawn(Vector2.zero, host.transform, null);
        Assert.That(Active()[0], Is.SameAs(effect));
        host.SetActive(false); Assert.That(Active(), Is.Empty);
#else
        Assert.Fail("Actual project prefab requires Editor PlayMode"); yield break;
#endif
    }
    [UnityTest, Category("Quick")]
    public IEnumerator EveryCodeManuProfileReleasesOnceAndResetsItsActualAnimation()
    {
#if UNITY_EDITOR
        foreach (var role in new[] { "Normal", "Parry", "Lethal", "Slayer1", "Slayer2", "Slayer3", "Slayer4" })
        {
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/Pimen/Generated/" + role + ".prefab");
            Assert.That(prefab, Is.Not.Null, role);
            float pixels = role == "Normal" ? 64 : role == "Parry" ? 72 : role == "Lethal" ? 96 : 56;
            var spritePath = UnityEditor.AssetDatabase.GetAssetPath(prefab.GetComponent<SpriteRenderer>().sprite);
            string sheetName = role == "Normal" ? "SmallHit" : role == "Parry" ? "Impact" : role == "Lethal" ? "BigHit" : role;
            Assert.That(spritePath, Is.EqualTo("Assets/Art/VFX/CodeManu/" + sheetName + "-White.png"));
            var texture = Own(new Texture2D(2, 2));
            texture.LoadImage(System.IO.File.ReadAllBytes(spritePath));
            Assert.That(texture.width, Is.EqualTo(1024)); Assert.That(texture.height, Is.EqualTo(128));
            int extent = 0;
            for (int f = 0; f < 8; f++)
            {
                int minX = 128, minY = 128, maxX = -1, maxY = -1;
                bool white = true;
                foreach (var pixel in texture.GetPixels(f * 128, 0, 128, 128))
                    if (pixel.a > 0 && (pixel.r != 1 || pixel.g != 1 || pixel.b != 1)) white = false;
                Assert.That(white, Is.True, role + " palette");
                for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
                {
                    if (texture.GetPixel(f * 128 + x, y).a == 0) continue;
                    minX = Mathf.Min(minX, x); minY = Mathf.Min(minY, y); maxX = Mathf.Max(maxX, x); maxY = Mathf.Max(maxY, y);
                }
                Assert.That(maxX, Is.GreaterThanOrEqualTo(minX), "no transparent end frames");
                Assert.That(minX, Is.GreaterThan(0)); Assert.That(maxX, Is.LessThan(127));
                Assert.That(minY, Is.GreaterThan(0)); Assert.That(maxY, Is.LessThan(127));
                extent = Mathf.Max(extent, maxX - minX + 1, maxY - minY + 1);
            }
            float expectedScale = pixels / 42f * 100f / extent;
            Assert.That(prefab.transform.localScale.x, Is.EqualTo(expectedScale).Within(.00001f), role);
            Assert.That(prefab.transform.localScale.y, Is.EqualTo(expectedScale).Within(.00001f), role);
            Assert.That(prefab.transform.localScale.z, Is.EqualTo(expectedScale).Within(.00001f), role);
            Assert.That(prefab.GetComponent<SpriteRenderer>().sortingOrder, Is.EqualTo(30), role);
            var clip = prefab.GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
            float duration = role == "Normal" ? .15f : role.StartsWith("Slayer") ? .1f : .18f;
            Assert.That(clip.length, Is.EqualTo(duration).Within(.001f), role);
            var keys = UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,
                UnityEditor.EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"));
            Assert.That(keys.Length, Is.EqualTo(9));
            Assert.That(keys[1].time, Is.EqualTo(.02f).Within(.00001f));
            for (int i = 0; i < 8; i++)
            {
                var frame = (Sprite)keys[i].value;
                Assert.That(frame.rect.size, Is.EqualTo(new Vector2(128, 128)));
                Assert.That(frame.pivot, Is.EqualTo(new Vector2(64, 64)));
                if (i > 0) Assert.That(keys[i].time, Is.EqualTo(.02f + (duration - .02f) * (i - 1) / 7).Within(.00001f));
            }
            var events = UnityEditor.AnimationUtility.GetAnimationEvents(clip);
            Assert.That(events.Length, Is.EqualTo(1)); Assert.That(events[0].functionName, Is.EqualTo("AnimEvent_Release"));
            Assert.That(events[0].time, Is.EqualTo(duration).Within(.001f));
            Assert.That(UnityEditor.AnimationUtility.GetAnimationClipSettings(clip).loopTime, Is.False);
            var sprite = prefab.GetComponent<SpriteRenderer>().sprite;
            var importer = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(UnityEditor.AssetDatabase.GetAssetPath(sprite));
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point)); Assert.That(importer.mipmapEnabled, Is.False);
            Assert.That(importer.GetPlatformTextureSettings("WebGL").format, Is.EqualTo(UnityEditor.TextureImporterFormat.RGBA32));
            var host = Host(role + " owner"); var spawner = host.AddComponent<HitSparkSpawner>();
            Set(spawner, "sparkPrefab", prefab); Set(spawner, "scaleRange", Vector2.one); Set(spawner, "flipXChance", 0f);
            host.SetActive(true); spawner.SpawnImpact(Vector2.zero, host.transform, false, false);
            var effect = Active()[0]; var renderer = effect.GetComponent<SpriteRenderer>(); var animator = effect.GetComponent<Animator>();
            var scale = effect.transform.localScale; renderer.color = Color.red; effect.transform.localScale *= 3;
            Assert.That(scale, Is.EqualTo(prefab.transform.localScale), role + " uses the authored size");
            Time.timeScale = .0001f;
            float deadline = Time.realtimeSinceStartup + duration + .3f;
            while (effect.gameObject.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(effect.gameObject.activeSelf, Is.False, role + " did not return via event");
            effect.AnimEvent_Release(); Time.timeScale = 1;
            spawner.SpawnImpact(Vector2.one, host.transform, false, false);
            Assert.That(Active()[0], Is.SameAs(effect)); Assert.That(renderer.color, Is.EqualTo(Color.white));
            Assert.That(effect.transform.localScale, Is.EqualTo(scale)); Assert.That(animator.speed, Is.EqualTo(1));
            host.SetActive(false); Assert.That(Active(), Is.Empty);
        }
#else
        Assert.Fail("Actual CodeManu prefabs require Editor PlayMode"); yield break;
#endif
    }

    [UnityTest, Category("Quick")]
    public IEnumerator IvoryFlashHoldsThroughPauseAndRestoresOnDisable()
    {
        var host = Host("flash"); var sr = host.AddComponent<SpriteRenderer>(); sr.color = Color.gray;
        var flash = host.AddComponent<EnemyHitFlash>();
        var ivory = new Color(1, .96f, .85f, 1); Set(flash, "flashColor", ivory);
        host.SetActive(true); flash.Tick(.06f);
        Assert.That(sr.color, Is.EqualTo(ivory));
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.1f); Assert.That(sr.color, Is.EqualTo(ivory));
        Time.timeScale = 1; yield return new WaitForSecondsRealtime(.08f);
        Assert.That(sr.color, Is.EqualTo(Color.gray));
        flash.Tick(); flash.enabled = false; Assert.That(sr.color, Is.EqualTo(Color.gray));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ImpactVoicesUsePreferencesAndPauseWithoutReplacingWhooshOrParry()
    {
        bool had = PlayerPrefs.HasKey("SfxVolume"); float saved = AudioPreferences.Sfx;
        var host = Own(new GameObject("audio")); var player = host.AddComponent<SfxPlayer>();
        var impact = Own(ScriptableObject.CreateInstance<SfxEvent>());
        Set(impact, "clips", new[] { Own(AudioClip.Create("impact", 44100, 1, 44100, false)) });
        Set(impact, "volume", .65f); Set(impact, "volumeJitter", 0f); Set(impact, "pitchJitterSemitone", .5f);
        var clang = Own(ScriptableObject.CreateInstance<SfxEvent>());
        Set(clang, "clips", new[] { Own(AudioClip.Create("clang", 44100, 1, 44100, false)) });
        var whoosh = Own(ScriptableObject.CreateInstance<SfxEvent>());
        Set(whoosh, "clips", new[] { Own(AudioClip.Create("whoosh", 44100, 1, 44100, false)) });
        try
        {
            PlayerPrefs.SetFloat("SfxVolume", 0);
            var voice = player.Play(impact, Vector3.zero);
            var parry = player.PlayOnChannel(clang, Vector3.zero, null, "player_parry", host.transform);
            var swing = player.PlayOnChannel(whoosh, Vector3.zero, null, "player_attack", host.transform);
            Assert.That(voice.volume, Is.Zero); Assert.That(voice, Is.Not.SameAs(parry)); Assert.That(voice, Is.Not.SameAs(swing));
            Assert.That(voice.pitch, Is.InRange(Mathf.Pow(2, -.5f / 12), Mathf.Pow(2, .5f / 12)));
            PlayerPrefs.SetFloat("SfxVolume", .5f);
            typeof(SfxPlayer).GetMethod("LateUpdate", Flags).Invoke(player, null);
            Assert.That(voice.volume, Is.EqualTo(.325f).Within(.001));
            AudioListener.pause = true;
            yield return new WaitForSecondsRealtime(1.15f);
            Assert.That(voice.clip, Is.Not.Null, "paused voices retain their lease beyond the clip duration");
            AudioListener.pause = false;
            float end = Time.realtimeSinceStartup + 2;
            while (voice.clip && Time.realtimeSinceStartup < end) yield return null;
            Assert.That(voice.clip, Is.Null);
        }
        finally
        {
            AudioListener.pause = false;
            if (had) PlayerPrefs.SetFloat("SfxVolume", saved); else PlayerPrefs.DeleteKey("SfxVolume");
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SceneUnloadReclaimsActiveAndIdleEffectsAndParryListener()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.CreateScene("combat feedback lifecycle");
        var host = Host("scene owner");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
        var prefab = Spark("scene spark");
        var spawner = host.AddComponent<HitSparkSpawner>(); Set(spawner, "sparkPrefab", prefab);
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var parryFx = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(parryFx, "parrySystem", parry); Set(parryFx, "sparkPrefab", prefab);
        Set(parryFx, "playerSparkAnchor", host.transform);
        host.SetActive(true);
        spawner.SpawnImpact(Vector2.zero, host.transform, false, false);
        var idle = Active()[0];
        spawner.SpawnImpact(Vector2.one, host.transform, false, false);
        idle.AnimEvent_Release();
        parry.ParrySuccess(); Assert.That(Active().Length, Is.EqualTo(2));
        parryFx.enabled = false; Assert.That(Active().Length, Is.EqualTo(1));
        parry.ParrySuccess(); Assert.That(Active().Length, Is.EqualTo(1), "disabled listener must be removed");
        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        Assert.That(host == null, Is.True); Assert.That(idle == null, Is.True);
        Assert.That(Active(), Is.Empty);
    }

}
