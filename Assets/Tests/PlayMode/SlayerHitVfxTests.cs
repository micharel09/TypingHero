#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public sealed class SlayerHitVfxTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    readonly List<UnityEngine.Object> owned = new();
    static void Set(object o, string f, object v) => o.GetType().GetField(f, Flags).SetValue(o, v);
    static T Get<T>(object o, string f) => (T)o.GetType().GetField(f, Flags).GetValue(o);
    GameObject Host(string name) { var go = new GameObject(name); owned.Add(go); go.SetActive(false); return go; }
    static GameObject Prefab(bool lethal) => AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/SlayerHits/" + (lethal ? "SlayerFinisher" : "SlayerHit") + ".prefab");
    static SlayerHitVfx[] Active() => UnityEngine.Object.FindObjectsOfType<SlayerHitVfx>();
    HitSparkSpawner Owner(out GameObject go)
    {
        go = Host("slash owner"); var spawner = go.AddComponent<HitSparkSpawner>();
        Set(spawner, "slayerPrefabs", new[] { Prefab(false) }); Set(spawner, "slayerLethalPrefab", Prefab(true));
        go.SetActive(true); return spawner;
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        GamePauseController.ResetSessionState(); SlayerModeSignals.SetActive(false);
        foreach (var o in owned) if (o) UnityEngine.Object.Destroy(o);
        owned.Clear(); yield return null; Assert.That(Active(), Is.Empty);
    }
    [TestCase(false, 32f), TestCase(true, 40f), Category("Quick")]
    public void WhiteBArtworkSlicePivotTimingAndVisibleScale(bool lethal, float width)
    {
        var effect = Prefab(lethal).GetComponent<SlayerHitVfx>();
        Assert.That(Prefab(lethal).GetComponentsInChildren<ParticleSystem>(true), Is.Empty);
        var frames = Get<Sprite[]>(effect, "frames"); Assert.That(frames.Length, Is.EqualTo(4));
        var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(frames[0]));
        Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point)); Assert.That(importer.mipmapEnabled, Is.False);
        Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
        Assert.That(importer.npotScale, Is.EqualTo(TextureImporterNPOTScale.None));
        Assert.That(frames[0].texture.width, Is.EqualTo(384)); Assert.That(frames[0].texture.height, Is.EqualTo(64));
        Assert.That(effect.GetComponent<SpriteRenderer>().sharedMaterial, Is.SameAs(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat")));
        var order = new[] { 0, 1, 2, 3 };
        for (int i = 0; i < 4; i++)
        {
            Assert.That(frames[i].name, Is.EqualTo("frame_" + order[i]));
            Assert.That(frames[i].rect, Is.EqualTo(new Rect(order[i] * 64, 0, 64, 64)));
            Assert.That(frames[i].pivot, Is.EqualTo(new Vector2(32, 32)));
            Assert.That(frames[i].pixelsPerUnit, Is.EqualTo(16));
        }
        Assert.That(Get<float[]>(effect, "frameDurations"), Is.EqualTo(lethal ? new[] { .06f, .04f, .03f, .03f } : new[] { .04f, .03f, .03f, .02f }));
        var texture = new Texture2D(2, 2); int widest = 0;
        try
        {
            var bytes = System.IO.File.ReadAllBytes(AssetDatabase.GetAssetPath(frames[0]));
            using (var hash = System.Security.Cryptography.SHA256.Create())
                Assert.That(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(), Is.EqualTo("62ad20c15fb52b44b4b49f6aa0bb5e583c1ddc5209964530724edc43350f7356"));
            texture.LoadImage(bytes);
            foreach (var frame in frames)
            {
                int left = 64, right = -1;
                for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
                {
                    var pixel = texture.GetPixel((int)frame.rect.x + x, (int)frame.rect.y + y);
                    Assert.That(pixel.r == 1 && pixel.g == 1 && pixel.b == 1 && (pixel.a == 0 || pixel.a == 1), Is.True);
                    if (pixel.a > 0) { left = Mathf.Min(left, x); right = Mathf.Max(right, x); }
                }
                widest = Mathf.Max(widest, right - left + 1);
            }
        }
        finally { UnityEngine.Object.DestroyImmediate(texture); }
        Assert.That(Get<float>(effect, "uniformScale") * widest, Is.EqualTo(width).Within(.0001));
    }
    [TestCase(false), TestCase(true), Category("Quick")]
    public void EveryFrameKeepsContactTintAndAngleUntilRelease(bool lethal)
    {
        var spawner = Owner(out var go);
        spawner.SpawnImpact(new Vector2(3, 4), go.transform, true, lethal, directionOverride: -1, capturedTint: Color.red);
        var effect = Active().Single(); var image = effect.GetComponent<SpriteRenderer>();
        Assert.That(effect.GetComponent<VoxelImpactBurst>(), Is.Null);
        var rotation = effect.transform.rotation;
        var durations = Get<float[]>(effect, "frameDurations");
        var order = new[] { 0, 1, 2, 3 };
        SlayerModeSignals.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            Assert.That(image.sprite.name, Is.EqualTo("frame_" + order[i]));
            effect.Advance(durations[i] - .001f);
            Assert.That(image.sprite.name, Is.EqualTo("frame_" + order[i]), "frame held for full source duration");
            Assert.That(image.color, Is.EqualTo(Color.red)); Assert.That(image.flipX || image.flipY, Is.False);
            Assert.That(effect.transform.position, Is.EqualTo(new Vector3(3, 4, 0)));
            Assert.That(effect.transform.rotation, Is.EqualTo(rotation));
            effect.Advance(.00101f);
        }
        Assert.That(Active(), Is.Empty);
        Assert.That(effect.transform.rotation, Is.EqualTo(Quaternion.identity));
        spawner.SpawnImpact(Vector2.zero, go.transform, true, lethal, directionOverride: 1, capturedTint: Color.white);
        Assert.That(Active().Single(), Is.SameAs(effect)); Assert.That(effect.Age, Is.Zero);
        Assert.That(image.color, Is.EqualTo(Color.white)); Assert.That(image.flipX || image.flipY, Is.False);
        Assert.That(effect.transform.rotation, Is.Not.EqualTo(rotation));
        Assert.That(image.sprite.name, Is.EqualTo("frame_0"));
    }
    [TestCase(0, 1, -1), TestCase(1, 1, 0), TestCase(2, 1, 1), Category("Quick")]
    public void AuthoredDirectionRepeatsWithTheSelectedSwing(int variant, float x, float y)
    {
        var direction = PlayerSlayerMode.LocalStrikeDirection(variant);
        Assert.That(direction, Is.EqualTo(new Vector2(x, y)));
        var owner = Owner(out var go);
        for (int i = 0; i < 3; i++)
        {
            owner.SpawnImpact(Vector2.zero, go.transform, true, false, slayerDirection: direction);
            var primary = Active().Single(v => !v.IsFading);
            float angle = Mathf.Repeat(45 - Mathf.Atan2(y, x) * Mathf.Rad2Deg, 360);
            Assert.That(Quaternion.Angle(primary.transform.rotation, Quaternion.Euler(0, 0, -angle)), Is.LessThan(.001f));
            Assert.That(Active().Length, Is.LessThanOrEqualTo(2));
        }
    }
    [Test, Category("Quick")]
    public void RecipientOwnsOnePrimaryAndOneThirtyMillisecondResidualAndFinisherReplacesBoth()
    {
        var owner = Owner(out var go);
        var target = Host("target"); target.SetActive(true);
        var other = Host("other target"); other.SetActive(true);
        owner.SpawnImpact(Vector2.zero, go.transform, true, false, target.transform);
        var first = Active().Single();
        owner.SpawnImpact(Vector2.zero, go.transform, true, false, target.transform);
        Assert.That(first.IsFading, Is.True);
        Assert.That(first.GetComponent<SpriteRenderer>().color.a, Is.EqualTo(.35f));
        first.Advance(.015f);
        Assert.That(first.GetComponent<SpriteRenderer>().color.a, Is.EqualTo(.175f).Within(.0001));
        owner.SpawnImpact(Vector2.zero, go.transform, true, false, other.transform);
        Assert.That(Active().Count(v => v.Recipient == target.transform), Is.EqualTo(2));
        owner.SpawnImpact(Vector2.zero, go.transform, true, true, target.transform);
        Assert.That(Active().Count(v => v.Recipient == target.transform), Is.EqualTo(1));
        Assert.That(Active().Single(v => v.Recipient == target.transform).name, Does.StartWith("SlayerFinisher"));
        Assert.That(Active().Single(v => v.Recipient == other.transform).IsFading, Is.False);
        owner.SpawnImpact(Vector2.zero, go.transform, true, false, other.transform);
        var residual = Active().Single(v => v.IsFading);
        residual.Advance(.03001f);
        Assert.That(residual.gameObject.activeSelf, Is.False);
    }
    [TestCase(false), TestCase(true), Category("Quick")]
    public void AuthoredMirroredAnglesAndProgressionTintKeepAllFourFrames(bool lethal)
    {
        var owner = Owner(out var go);
        var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player/Player.prefab");
        var gradient = Get<Gradient>(player.GetComponent<SlayerAfterimagePool>(), "comboGradient");
        foreach (int direction in new[] { 0, 1, 2, 4, 5, 6 }) for (int combo = 0; combo <= 20; combo++)
        {
            owner.SpawnImpact(Vector2.zero, go.transform, true, lethal);
            var effect = Active().Single();
            var tint = gradient.Evaluate(combo / 20f); tint.a = 1;
            effect.Emit(new Vector3(3, 4, 0), direction * 45, tint);
            var sprite = effect.GetComponent<SpriteRenderer>();
            foreach (int frame in new[] { 0, 1, 2, 3 })
            {
                Assert.That(sprite.sprite.name, Is.EqualTo("frame_" + frame));
                Assert.That(sprite.color, Is.EqualTo(tint));
                Assert.That(sprite.flipX || sprite.flipY, Is.False);
                Assert.That(Quaternion.Angle(effect.transform.rotation, Quaternion.Euler(0, 0, -direction * 45)), Is.LessThan(.001));
                Assert.That(effect.transform.position, Is.EqualTo(new Vector3(3, 4, 0)));
                effect.Advance(Get<float[]>(effect, "frameDurations")[frame] + .000001f);
            }
            Assert.That(Active(), Is.Empty);
        }
    }
    [UnityTest, Category("Quick")]
    public IEnumerator TutorialPauseHoldsAndResultClearsEvenWhenFinished()
    {
        var spawner = Owner(out var go); spawner.SpawnImpact(Vector2.zero, go.transform, true, true);
        var effect = Active().Single(); GamePauseController.SetTutorialReading(true);
        float age = effect.Age; var sprite = effect.GetComponent<SpriteRenderer>().sprite;
        yield return new WaitForSecondsRealtime(.22f);
        Assert.That(effect.Age, Is.EqualTo(age)); Assert.That(effect.GetComponent<SpriteRenderer>().sprite, Is.SameAs(sprite));
        spawner.SpawnImpact(Vector2.zero, go.transform, true, true); Assert.That(Active().Length, Is.EqualTo(1));
        GamePauseController.SetTutorialReading(false); yield return null; yield return null;
        Assert.That(effect.Age, Is.GreaterThan(age));
        typeof(GamePauseController).GetProperty("ResultOverlayVisible", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, true);
        yield return null; Assert.That(Active(), Is.Empty);
    }
    [TestCase(.05f), TestCase(.1f), TestCase(.2f), Category("Quick")]
    public void RapidHitsBoundPoolAndDisableReclaims(float interval)
    {
        var spawner = Owner(out var go);
        for (int i = 0; i < 20; i++)
        {
            foreach (var effect in Active()) effect.Advance(interval);
            spawner.SpawnImpact(Vector2.zero, go.transform, true, i % 2 == 0);
            Assert.That(Active().Length, Is.LessThanOrEqualTo(8));
        }
        foreach (var effect in Active()) effect.GetComponent<PooledVFX>().Release();
        for (int i = 0; i < 9; i++) spawner.SpawnImpact(Vector2.zero, go.transform, true, false);
        Assert.That(Active().Length, Is.EqualTo(2));
        go.SetActive(false); Assert.That(Active(), Is.Empty);
        go.SetActive(true); spawner.SpawnImpact(Vector2.zero, go.transform, true, false);
        Assert.That(Active().Single().Age, Is.Zero);
    }
    [TestCase(false, false), TestCase(false, true), TestCase(true, false), TestCase(true, true), Category("Quick")]
    public void RoleRoutingPreservesNonSlayerAndLegacyConsumers(bool slayer, bool lethal)
    {
        var spawner = Owner(out var go);
        var normal = Host("legacy normal"); normal.AddComponent<SpriteRenderer>(); normal.AddComponent<PooledVFX>();
        var death = Host("legacy lethal"); death.AddComponent<SpriteRenderer>(); death.AddComponent<PooledVFX>();
        Set(spawner, "sparkPrefab", normal); Set(spawner, "lethalPrefab", death);
        spawner.SpawnImpact(Vector2.zero, go.transform, slayer, lethal);
        var active = Get<List<GameObject>>(spawner, "active");
        Assert.That(active.Count, Is.EqualTo(1));
        Assert.That(active[0].name, Does.StartWith(slayer ? lethal ? "SlayerFinisher" : "SlayerHit" : lethal ? "legacy lethal" : "legacy normal"));
        active[0].GetComponent<PooledVFX>().Release();
        Set(spawner, "slayerLethalPrefab", null);
        spawner.SpawnImpact(Vector2.zero, go.transform, true, true);
        Assert.That(active.Single().name, Does.StartWith("legacy lethal"));
    }
    [Test, Category("Quick")]
    public void SlashAndNormalShareEightImpactLeasesAndReclaimOldest()
    {
        var spawner = Owner(out var go);
        Set(spawner, "sparkPrefab", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/NormalHitSpark.prefab"));
        for (int i = 0; i < 8; i++) spawner.SpawnImpact(Vector2.zero, go.transform, false, false);
        var active = Get<List<GameObject>>(spawner, "active"); var oldest = active[0]; var second = active[1];
        spawner.SpawnImpact(Vector2.zero, go.transform, true, false);
        Assert.That(active.Count, Is.EqualTo(8)); Assert.That(oldest.activeSelf, Is.False); Assert.That(second.activeSelf, Is.True);
        Assert.That(active.Last().GetComponent<SlayerHitVfx>(), Is.Not.Null);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator FinishedSessionAllowsTailAndSceneUnloadReclaimsActiveAndIdle()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.CreateScene("Slayer slash lifecycle");
        var spawner = Owner(out var go);
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, scene);
        // The pool root is created in Awake, so move its independent root as a scene owner would.
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(Get<Transform>(spawner, "poolRoot").gameObject, scene);
        spawner.SpawnImpact(Vector2.zero, go.transform, true, true);
        var idle = Active().Single(); idle.GetComponent<PooledVFX>().Release();
        spawner.SpawnImpact(Vector2.zero, go.transform, true, false);
        var effect = Active().Single();
        var session = Host("finished session"); session.SetActive(true); session.AddComponent<CombatSession>().Complete(true);
        yield return null; yield return null;
        Assert.That(effect.Age, Is.GreaterThan(0));
        Assert.That(effect.gameObject.activeSelf, Is.True);
        effect.Advance(.6f); Assert.That(Active(), Is.Empty);
        spawner.SpawnImpact(Vector2.zero, go.transform, true, true);
        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        Assert.That(go == null && idle == null && effect == null, Is.True);
        Assert.That(Active(), Is.Empty);
    }
    [Test, Category("Quick")]
    public void OrdinarySlayerContactRejectsClosedDeadAndReadingTargets()
    {
        var spawner = Owner(out var go); go.SetActive(false); go.AddComponent<BoxCollider2D>();
        var slayer = go.AddComponent<PlayerSlayerMode>();
        var box = go.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1; Set(box, "hitSpark", spawner); Set(box, "slayer", slayer);
        go.SetActive(true); Set(slayer, "_active", true);
        var enemy = Host("living recipient"); var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var a = enemy.AddComponent<BoxCollider2D>(); var b = enemy.AddComponent<BoxCollider2D>(); enemy.SetActive(true);
        var hit = typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags);
        hit.Invoke(box, new object[] { a }); Assert.That(Active(), Is.Empty);
        box.BeginAttack(1); GamePauseController.SetTutorialReading(true);
        hit.Invoke(box, new object[] { a }); Assert.That(Active(), Is.Empty);
        GamePauseController.SetTutorialReading(false);
        hit.Invoke(box, new object[] { a }); hit.Invoke(box, new object[] { b });
        Assert.That(damage.hits, Is.EqualTo(1)); Assert.That(Active().Single().name, Does.StartWith("SlayerHit"));
        box.EndAttack(); hit.Invoke(box, new object[] { a }); Assert.That(damage.hits, Is.EqualTo(1));
        damage.dead = true; box.BeginAttack(2); hit.Invoke(box, new object[] { a }); Assert.That(damage.hits, Is.EqualTo(1));
        Set(slayer, "_active", false); // Undo manual fixture activation; this fixture has no Animator.
    }
    [Test, Category("Quick")]
    public void LethalHitCapturesSlayerAndTintBeforeDamageAndDeduplicatesColliders()
    {
        var spawner = Owner(out var go); go.SetActive(false); go.AddComponent<BoxCollider2D>();
        var slayer = go.AddComponent<PlayerSlayerMode>(); var images = go.AddComponent<SlayerAfterimagePool>();
        var box = go.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1; Set(box, "hitSpark", spawner); Set(box, "slayer", slayer);
        go.SetActive(true); Set(slayer, "_active", true); Set(images, "<CurrentTint>k__BackingField", Color.red);
        var enemy = Host("recipient"); var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var a = enemy.AddComponent<BoxCollider2D>(); var b = enemy.AddComponent<BoxCollider2D>(); enemy.SetActive(true);
        damage.onDamage = () => { damage.dead = true; Set(slayer, "_active", false); Set(images, "<CurrentTint>k__BackingField", Color.white); };
        box.BeginAttack(1);
        var hit = typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags);
        hit.Invoke(box, new object[] { a }); hit.Invoke(box, new object[] { b });
        Assert.That(damage.hits, Is.EqualTo(1)); var effect = Active().Single();
        Assert.That(effect.name, Does.StartWith("SlayerFinisher")); Assert.That(effect.GetComponent<SpriteRenderer>().color, Is.EqualTo(Color.red));
        Assert.That(Time.timeScale, Is.EqualTo(1));
    }

    [TestCase(false), TestCase(true), Category("Quick")]
    public void HitboxCapturesBodyCenterBeforeDamageAndKeepsContactSeparate(bool lethal)
    {
        var spawner = Owner(out var go); go.SetActive(false); go.AddComponent<BoxCollider2D>();
        var slayer = go.AddComponent<PlayerSlayerMode>();
        var box = go.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1;
        Set(box, "hitSpark", spawner); Set(box, "slayer", slayer);
        go.SetActive(true); Set(slayer, "_active", true);
        var enemy = Host("offset body"); enemy.transform.position = new Vector3(5, 3, 0);
        enemy.transform.localScale = new Vector3(-1.5f, 1.2f, 1);
        var damage = enemy.AddComponent<FeedbackDamageTarget>();
        var body = enemy.AddComponent<BoxCollider2D>(); body.offset = new Vector2(.4f, .2f);
        body.size = new Vector2(2, 3); enemy.SetActive(true); Physics2D.SyncTransforms();
        Vector2 center = body.bounds.center, contact = body.ClosestPoint(go.transform.position);
        Assert.That(Vector2.Distance(center, contact), Is.GreaterThan(.5f));
        Assert.That(Vector2.Distance(center, enemy.transform.position), Is.GreaterThan(.1f));
        damage.onDamage = () =>
        {
            damage.dead = lethal;
            enemy.transform.position += Vector3.right * 7;
            body.enabled = false;
            Physics2D.SyncTransforms();
        };
        box.BeginAttack(1);
        typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags).Invoke(box, new object[] { body });
        var effect = Active().Single();
        Assert.That(effect.name, Does.StartWith(lethal ? "SlayerFinisher" : "SlayerHit"));
        Assert.That(damage.point, Is.EqualTo(contact)); Assert.That(box.LastContact, Is.EqualTo(contact));
        var rotation = effect.transform.rotation;
        foreach (float duration in Get<float[]>(effect, "frameDurations"))
        {
            Assert.That((Vector2)effect.transform.position, Is.EqualTo(center));
            Assert.That(effect.transform.rotation, Is.EqualTo(rotation));
            enemy.transform.position += Vector3.up;
            effect.Advance(duration);
        }
        Assert.That(Active(), Is.Empty);
        Set(slayer, "_active", false);
    }

    [TestCase(1, "0,1,2", 4)]
    [TestCase(2, "3,4,5", 4)]
    [TestCase(3, "11", 6)]
    [Category("Quick")]
    public void SlayerClipsUseFinalStrikeAndCompleteRecoveryIn280Milliseconds(int variant, string attackCels, int endFrames)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"Assets/Characters/Player/player_slayer/attackS{variant}.anim");
        var keys = AnimationUtility.GetObjectReferenceCurve(clip, AnimationUtility.GetObjectReferenceCurveBindings(clip).Single());
        var poses = attackCels.Split(',').Select(i => $"male_hero-combo_{variant}_{i}")
            .Concat(Enumerable.Range(0, endFrames).Select(i => $"male_hero-combo_{variant}_end_{i}")).ToArray();
        Assert.That(keys.Select(k => k.value.name), Is.EqualTo(poses.Concat(new[] { poses.Last() })));
        var times = new[] { 0f, .04f, .08f, .12f, .16f, .20f, .24f, .27f };
        for (int i = 0; i < keys.Length; i++) Assert.That(keys[i].time, Is.EqualTo(times[i]).Within(.0001));
        Assert.That(clip.length, Is.EqualTo(.28f).Within(.0001));
        Assert.That(clip.isLooping, Is.False);
    }
    [Test, Category("Quick")]
    public void CorrectCharacterAppliesContactPoseBeforeOpeningTheExistingDamageWindow()
    {
        var rig = Host("instant Slayer"); var sr = rig.AddComponent<SpriteRenderer>();
        var animator = rig.AddComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player/Player.prefab").GetComponent<Animator>().runtimeAnimatorController;
        var mode = rig.AddComponent<PlayerSlayerMode>(); var events = rig.AddComponent<PlayerAttackEvents>();
        var weapon = Host("weapon"); weapon.transform.SetParent(rig.transform); weapon.AddComponent<BoxCollider2D>();
        var box = weapon.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1; Set(box, "slayer", mode);
        events.hitbox = box; events.minOpenSeconds = 0;
        Set(mode, "animator", animator); Set(mode, "attackEvents", events);
        rig.SetActive(true); weapon.SetActive(true); Set(mode, "_active", true);
        typeof(PlayerSlayerMode).GetMethod("OnCorrectChar", Flags).Invoke(mode, new object[] { 'a' });
        int variant = Get<int>(mode, "selectedAttackIndex");
        var expected = new[] { "male_hero-combo_1_0", "male_hero-combo_2_3", "male_hero-combo_3_11" }[variant];
        Assert.That(sr.sprite.name, Is.EqualTo(expected));
        Assert.That(mode.StrikeDirection, Is.EqualTo(PlayerSlayerMode.LocalStrikeDirection(variant).normalized));
        var recipient = Host("damage recipient"); var damage = recipient.AddComponent<FeedbackDamageTarget>();
        var body = recipient.AddComponent<BoxCollider2D>(); recipient.SetActive(true);
        typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags).Invoke(box, new object[] { body });
        Assert.That(damage.hits, Is.EqualTo(1)); Assert.That(box.LastHitFrame, Is.EqualTo(Time.frameCount));
        Assert.That(sr.sprite.name, Is.EqualTo(expected));
        Set(mode, "_active", false);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator HeldSlayerCelExpiresItsTrailAndANewSwingRestartsItWhilePauseHoldsAge()
    {
        var rig = Host("trail rig"); rig.AddComponent<PlayerSlayerMode>();
        var sr = rig.AddComponent<SpriteRenderer>();
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Characters/Player/player_slayer/attackS1.anim");
        sr.sprite = (Sprite)AnimationUtility.GetObjectReferenceCurve(clip, AnimationUtility.GetObjectReferenceCurveBindings(clip).Single())[0].value;
        var volume = rig.AddComponent<SlashVolume>(); Set(volume, "source", sr);
        Set(volume, "library", AssetDatabase.LoadAssetAtPath<SlashLibrary>("Assets/Art/VFX/SlashVolume/SlashLibrary.asset"));
        rig.SetActive(true); SlayerModeSignals.SetActive(true); volume.BeginSlayerSwing();
        var late = typeof(SlashVolume).GetMethod("LateUpdate", Flags); late.Invoke(volume, null);
        Assert.That(rig.GetComponentsInChildren<MeshRenderer>().Any(r => r.enabled), Is.True);
        GamePauseController.SetTutorialReading(true); float age = Get<float>(volume, "slayerAge");
        yield return new WaitForSecondsRealtime(.16f);
        Assert.That(Get<float>(volume, "slayerAge"), Is.EqualTo(age));
        GamePauseController.SetTutorialReading(false); yield return new WaitForSecondsRealtime(.16f);
        Assert.That(rig.GetComponentsInChildren<MeshRenderer>().All(r => !r.enabled), Is.True);
        volume.BeginSlayerSwing(); late.Invoke(volume, null);
        Assert.That(rig.GetComponentsInChildren<MeshRenderer>().Any(r => r.enabled), Is.True, "same held cel starts a new trail");
    }
    [UnityTest, Category("Quick")]
    public IEnumerator SlayerBodyPulseSurvivesSilhouetteHoldsPauseAndRestoresBaselineAndPropertyBlock()
    {
        var rig = Host("pulse rig"); var sr = rig.AddComponent<SpriteRenderer>();
        sr.color = new Color(.7f, .6f, .5f, .4f);
        var material = new Material(Shader.Find("CLACKBLADE/Dungeon Sprite")); owned.Add(material); sr.sharedMaterial = material;
        var group = rig.AddComponent<SlayerSilhouetteGroup>();
        rig.SetActive(true); var original = sr.color;
        group.SetPresentationWeight(1); group.PulseSlayerHit();
        var block = new MaterialPropertyBlock(); sr.GetPropertyBlock(block); block.SetFloat("_UnrelatedProbe", .73f); sr.SetPropertyBlock(block);
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", Flags); late.Invoke(group, null);
        sr.GetPropertyBlock(block);
        Assert.That(block.GetFloat("_SlayerHitFlashAmount"), Is.EqualTo(1));
        Assert.That(block.GetFloat("_UnrelatedProbe"), Is.EqualTo(.73f));
        Assert.That(sr.color.a, Is.EqualTo(original.a));
        GamePauseController.SetTutorialReading(true); yield return new WaitForSecondsRealtime(.08f);
        Assert.That(group.HitFlashWeight, Is.EqualTo(1));
        GamePauseController.SetTutorialReading(false); yield return new WaitForSecondsRealtime(.09f);
        Assert.That(group.HitFlashWeight, Is.Zero);
        group.SetPresentationWeight(0); sr.GetPropertyBlock(block);
        Assert.That(block.GetFloat("_SlayerHitFlashAmount"), Is.Zero);
        Assert.That(sr.color, Is.EqualTo(original));
    }
    [TestCase(false), TestCase(true), Category("Quick")]
    public void BodyCenterOverrideDoesNotMoveNonSlayerOrLegacyImpacts(bool wasSlayer)
    {
        var spawner = Owner(out var go);
        var legacy = Host("legacy contact"); legacy.AddComponent<SpriteRenderer>(); legacy.AddComponent<PooledVFX>();
        Set(spawner, "sparkPrefab", legacy); Set(spawner, "slayerLethalPrefab", null); Set(spawner, "lethalPrefab", legacy);
        Set(spawner, "verticalJitter", Vector2.zero);
        var contact = new Vector2(2, 3);
        spawner.SpawnImpact(contact, go.transform, wasSlayer, true, slayerVisualPoint: new Vector2(8, 9));
        Assert.That(Get<List<GameObject>>(spawner, "active").Single().transform.position, Is.EqualTo((Vector3)contact));
    }
}
#endif
