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
    [TestCase(false, .15f, 32f), TestCase(true, .18f, 40f), Category("Quick")]
    public void OriginalArtworkSlicePivotTimingAndVisibleScale(bool lethal, float duration, float width)
    {
        var effect = Prefab(lethal).GetComponent<SlayerHitVfx>(); Assert.That(effect, Is.Not.Null);
        Assert.That(Prefab(lethal).GetComponentsInChildren<ParticleSystem>(true), Is.Empty);
        var frames = Get<Sprite[]>(effect, "frames"); Assert.That(frames.Length, Is.EqualTo(9));
        var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(frames[0]));
        Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point)); Assert.That(importer.mipmapEnabled, Is.False);
        Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
        Assert.That(importer.npotScale, Is.EqualTo(TextureImporterNPOTScale.None));
        Assert.That(frames[0].texture.width, Is.EqualTo(192)); Assert.That(frames[0].texture.height, Is.EqualTo(141));
        Assert.That(effect.GetComponent<SpriteRenderer>().sharedMaterial, Is.SameAs(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat")));
        for (int i = 0; i < 9; i++)
        {
            Assert.That(frames[i].rect, Is.EqualTo(new Rect(i % 3 * 64, 141 - (i / 3 + 1) * 47, 64, 47)));
            Assert.That(frames[i].pivot, Is.EqualTo(new Vector2(32, 23.5f)));
            Assert.That(frames[i].pixelsPerUnit, Is.EqualTo(16));
        }
        Assert.That(Get<float>(effect, "duration"), Is.EqualTo(duration));
        var texture = new Texture2D(2, 2); int widest = 0;
        try
        {
            var bytes = System.IO.File.ReadAllBytes(AssetDatabase.GetAssetPath(frames[0]));
            using (var hash = System.Security.Cryptography.SHA256.Create())
                Assert.That(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(), Is.EqualTo("7642aa4698828f6cc908b3fd2873c3b3d8555138bae188991c6b775ae0c96915"));
            texture.LoadImage(bytes);
            foreach (var frame in frames)
            {
                int left = 64, right = -1;
                for (int y = 0; y < 47; y++) for (int x = 0; x < 64; x++)
                    if (texture.GetPixel((int)frame.rect.x + x, (int)frame.rect.y + y).a > 0) { left = Mathf.Min(left, x); right = Mathf.Max(right, x); }
                widest = Mathf.Max(widest, right - left + 1);
            }
        }
        finally { UnityEngine.Object.DestroyImmediate(texture); }
        Assert.That(Get<float>(effect, "uniformScale") * widest, Is.EqualTo(width).Within(.0001));
    }
    [TestCase(false, 1f), TestCase(false, -1f), TestCase(true, 1f), TestCase(true, -1f), Category("Quick")]
    public void EveryFrameKeepsContactTintAndMirrorUntilRelease(bool lethal, float direction)
    {
        var spawner = Owner(out var go);
        spawner.SpawnImpact(new Vector2(3, 4), go.transform, true, lethal, directionOverride: direction, capturedTint: Color.red);
        var effect = Active().Single(); var image = effect.GetComponent<SpriteRenderer>();
        Assert.That(effect.GetComponent<VoxelImpactBurst>(), Is.Null);
        float duration = lethal ? .18f : .15f;
        SlayerModeSignals.SetActive(false);
        effect.Advance(duration / 18);
        for (int i = 0; i < 9; i++)
        {
            if (i > 0) effect.Advance(duration / 9);
            Assert.That(image.sprite.name, Is.EqualTo("frame_" + i));
            Assert.That(image.color, Is.EqualTo(Color.red)); Assert.That(image.flipX, Is.EqualTo(direction < 0));
            Assert.That(effect.transform.position, Is.EqualTo(new Vector3(3, 4, 0)));
            Assert.That(effect.transform.rotation, Is.EqualTo(Quaternion.identity));
        }
        effect.Advance(duration / 9 + .00001f); Assert.That(Active(), Is.Empty);
        spawner.SpawnImpact(Vector2.zero, go.transform, true, lethal, directionOverride: -direction, capturedTint: Color.white);
        Assert.That(Active().Single(), Is.SameAs(effect)); Assert.That(effect.Age, Is.Zero);
        Assert.That(image.color, Is.EqualTo(Color.white)); Assert.That(image.flipX, Is.EqualTo(direction > 0));
        Assert.That(image.sprite.name, Is.EqualTo("frame_0"));
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
        Assert.That(Active().Length, Is.EqualTo(8));
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
        effect.Advance(.2f); Assert.That(Active(), Is.Empty);
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
}
#endif
