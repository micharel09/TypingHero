using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

#if UNITY_EDITOR
public sealed class EnemyStateVfxTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    readonly List<Object> owned = new();
    static ImpactVfxProfile Profile(string name) => UnityEditor.AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(
        "Assets/Art/VFX/EnemyState/" + name + ".asset");
    static void Set(object owner, string name, object value) => owner.GetType().GetField(name, Flags).SetValue(owner, value);
    T Host<T>(string name) where T : Component { var go = new GameObject(name); owned.Add(go); return go.AddComponent<T>(); }
    static ImpactVfxContext Context(bool tail = false) => new ImpactVfxContext {
        point = Vector3.up, direction = 1, floorY = 0, seed = 7,
        completionPolicy = tail ? ImpactVfxCompletionPolicy.AllowUntilResults : ImpactVfxCompletionPolicy.StopOnCombatEnd
    };
    [UnityTearDown] public IEnumerator Cleanup()
    {
        foreach (var item in owned) if (item) Object.Destroy(item);
        owned.Clear(); yield return null;
        GamePauseController.ResetSessionState(); Time.timeScale = 1;
        Assert.That(Object.FindObjectsOfType<ImpactVfxBurst>(), Is.Empty);
    }
    [Test, Category("Quick")]
    public void StockProfilesAreStandaloneAndPreserveAspectAndAuthoredFrames()
    {
        var emitter = Host<ImpactVfxEmitter>("standalone");
        var guard = emitter.Play(Profile("GuardBreak"), Context());
        var dust = emitter.Play(Profile("BodyFallDust"), Context());
        Assert.That(guard.GetComponent<ParrySystem>(), Is.Null);
        Assert.That(guard.ParticleCount, Is.EqualTo(1)); Assert.That(dust.ParticleCount, Is.EqualTo(1));
        var particles = new ParticleSystem.Particle[1]; guard.Systems[0].GetParticles(particles);
        Assert.That(particles[0].startSize3D.y / particles[0].startSize3D.x, Is.EqualTo(.5f).Within(.0001));
        Assert.That(guard.Systems[0].main.startSize3D, Is.True);
        Assert.That(dust.Systems[0].main.startSize3D, Is.False);
        Assert.That(guard.Systems[0].GetComponent<ParticleSystemRenderer>().sharedMaterial.GetTexture("_MainTex").width, Is.EqualTo(64));
        Assert.That(dust.Systems[0].GetComponent<ParticleSystemRenderer>().sharedMaterial.GetTexture("_MainTex").width, Is.EqualTo(256));
        foreach (var name in new[] { "GuardBreak", "BodyFallDust" })
        {
            var profile = Profile(name); var layer = profile.flashes[0];
            Assert.That(profile.physics.groups, Is.Empty); Assert.That(profile.lightEnabled, Is.False);
            int count = layer.columns * layer.rows;
            for (int i = 0; i < count; i++)
                Assert.That(Mathf.FloorToInt(layer.frameOverTime.Evaluate((i + .5f) / count) * count), Is.EqualTo(i));
            Assert.That(layer.frameOverTime.Evaluate(.9999f), Is.EqualTo((count - 1f) / count).Within(.0001));
        }
    }
    [Test, Category("Quick")]
    public void ConcurrentStockAndTemporaryProfilesSnapshotAppearanceAndReuseRestoresSquareSize()
    {
        var emitter = Host<ImpactVfxEmitter>("concurrent");
        var profile = Object.Instantiate(Profile("GuardBreak")); owned.Add(profile);
        profile.flashes[0].heightScale = .25f; profile.flashes[0].tint = Color.cyan;
        profile.flashes[0].lifetime = .6f;
        var custom = emitter.Play(profile, Context()); var original = emitter.Play(Profile("GuardBreak"), Context());
        profile.flashes[0].heightScale = 1; profile.flashes[0].tint = Color.magenta; profile.flashes[0].lifetime = .1f;
        var p = new ParticleSystem.Particle[1]; custom.Systems[0].GetParticles(p);
        Assert.That(p[0].startSize3D.y / p[0].startSize3D.x, Is.EqualTo(.25f).Within(.0001));
        Assert.That(p[0].startColor, Is.EqualTo((Color32)Color.cyan)); Assert.That(p[0].startLifetime, Is.EqualTo(.6f));
        original.Systems[0].GetParticles(p); Assert.That(p[0].startColor, Is.EqualTo((Color32)Color.white));
        emitter.Clear(); var reused = emitter.Play(profile, Context());
        Assert.That(reused.Systems[0].main.startSize3D, Is.False);
        reused.Systems[0].GetParticles(p); Assert.That(p[0].startColor, Is.EqualTo((Color32)Color.magenta));
        profile.flashes[0].heightScale = 0;
        Assert.Throws<System.InvalidOperationException>(() => emitter.Play(profile, Context()));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator CompletionPolicyIsPerLeasePauseSafeAndResultAlwaysClears()
    {
        var emitter = Host<ImpactVfxEmitter>("scene owned"); var session = Host<CombatSession>("test session");
        var guard = emitter.Play(Profile("GuardBreak"), Context());
        var tail = emitter.Play(Profile("BodyFallDust"), Context(true));
        session.Complete(true); yield return null;
        Assert.That(guard.gameObject.activeSelf, Is.False); Assert.That(tail.gameObject.activeSelf, Is.True);
        Assert.That(emitter.Play(Profile("GuardBreak"), Context()), Is.Null);
        Assert.That(emitter.Play(Profile("BodyFallDust"), Context(true)), Is.Not.Null);
        GamePauseController.SetTutorialReading(true); yield return null;
        float age = tail.Systems[0].time;
        yield return new WaitForSecondsRealtime(.12f); Assert.That(tail.Systems[0].time, Is.EqualTo(age));
        GamePauseController.SetTutorialReading(false);
        yield return new WaitForSecondsRealtime(.35f); Assert.That(emitter.active, Is.Empty);
        emitter.Play(Profile("BodyFallDust"), Context(true));
        var pause = Host<GamePauseController>("result overlay"); pause.ShowThanksSimple();
        Assert.That(GamePauseController.ResultOverlayVisible, Is.True); Assert.That(emitter.active, Is.Empty);
        Assert.That(emitter.Play(Profile("BodyFallDust"), Context(true)), Is.Null);
        Object.DestroyImmediate(pause.gameObject); GamePauseController.ResetSessionState();
        Set(session, "finished", false);
        var reused = emitter.Play(Profile("BodyFallDust"), Context());
        Assert.That(reused.CompletionPolicy, Is.EqualTo(ImpactVfxCompletionPolicy.StopOnCombatEnd));
        session.Complete(true); yield return null; Assert.That(emitter.active, Is.Empty);
    }
    EnemyStateVfxBinder Actor(out SkeletonController health, out EnemyStamina stamina, out EnemyStunController stun,
        out ImpactVfxEmitter guard, out ImpactVfxEmitter dust)
    {
        var go = new GameObject("fixture enemy"); owned.Add(go); go.SetActive(false);
        health = go.AddComponent<SkeletonController>(); health.HoldPracticeAttacks(true);
        stamina = go.AddComponent<EnemyStamina>();
        stun = go.AddComponent<EnemyStunController>(); stun.onStunStart = new UnityEvent(); stun.onStunEnd = new UnityEvent();
        guard = go.AddComponent<ImpactVfxEmitter>(); dust = Host<ImpactVfxEmitter>("scene death emitter");
        var body = go.AddComponent<BoxCollider2D>(); body.offset = new Vector2(.2f, .4f);
        var binder = go.AddComponent<EnemyStateVfxBinder>();
        Set(binder, "health", health); Set(binder, "stamina", stamina); Set(binder, "stun", stun); Set(binder, "body", body);
        Set(binder, "guardEmitter", guard); Set(binder, "deathEmitter", dust);
        Set(binder, "guardBreak", Profile("GuardBreak")); Set(binder, "bodyFallDust", Profile("BodyFallDust"));
        Set(binder, "floorY", -2f); go.SetActive(true); return binder;
    }
    [Test, Category("Quick")]
    public void RealNormalHitDrainEmitsGuardWithoutChangingDamageOrStaminaRules()
    {
        Actor(out var health, out var stamina, out var stun, out var guard, out _);
        stamina.PreparePractice(stamina.hitCost);
        var attacker = Host<BoxCollider2D>("normal attacker").gameObject.AddComponent<PlayerWeaponHitbox>(); attacker.targetLayers = 1;
        attacker.BeginAttack(1);
        typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Flags).Invoke(attacker, new object[] { health.GetComponent<BoxCollider2D>() });
        Assert.That(health.Current, Is.EqualTo(health.maxHealth - attacker.damage));
        Assert.That(stamina.Current, Is.Zero); Assert.That(stun.IsStunned, Is.True);
        Assert.That(guard.active.Count, Is.EqualTo(1));
        SlayerModeSignals.SetActive(true); SlayerModeSignals.SetActive(false);
        Assert.That(guard.active.Count, Is.EqualTo(1), "Entering Slayer does not swallow the guard cue");
    }
    [Test, Category("Quick")]
    public void RealParryDrainEmitsOneGuardOnAcceptedStunAndRefreshDoesNotReplay()
    {
        Actor(out var health, out var stamina, out var stun, out var guard, out _);
        stamina.PreparePractice(stamina.parryCost);
        var host = new GameObject("parry owner"); owned.Add(host); host.SetActive(false);
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var drain = host.AddComponent<ParryStaminaDrainBinder>(); Set(drain, "parry", parry);
        host.SetActive(true);
        var success = typeof(ParrySystem).GetMethod("ForceSuccess", Flags);
        success.Invoke(parry, new object[] { health });
        Assert.That(stamina.Current, Is.Zero); Assert.That(stun.IsStunned, Is.True);
        Assert.That(guard.active.Count, Is.EqualTo(1)); var burst = guard.active[0];
        success.Invoke(parry, new object[] { health });
        Assert.That(guard.active.Count, Is.EqualTo(1)); Assert.That(guard.active[0], Is.SameAs(burst));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator AcceptedStunDeduplicatesRefreshAndPendingPauseAndEnableDoesNotReplay()
    {
        var binder = Actor(out var health, out var stamina, out var stun, out var guard, out _);
        stamina.PreparePractice(0); stun.TriggerStun(.15f);
        Assert.That(health.enabled, Is.False, "Adapter works while the brain is disabled");
        Assert.That(guard.active.Count, Is.EqualTo(1)); var first = guard.active[0];
        stamina.PreparePractice(10); stamina.ConsumeHit(10); stun.TriggerStun(.15f);
        Assert.That(guard.active.Count, Is.EqualTo(1)); Assert.That(guard.active[0], Is.SameAs(first));
        binder.enabled = false; binder.enabled = true; stun.TriggerStun(.15f);
        Assert.That(guard.active, Is.Empty);
        yield return new WaitForSecondsRealtime(.25f);
        stamina.PreparePractice(0); GamePauseController.SetTutorialReading(true); stun.TriggerStun(.15f);
        Assert.That(guard.active, Is.Empty);
        yield return new WaitForSecondsRealtime(.05f); GamePauseController.SetTutorialReading(false); yield return null;
        Assert.That(guard.active.Count, Is.EqualTo(1));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator ActualSlayerActivationPreservesGuardAndDeathExitsWithoutCancellingSceneDust()
    {
        var binder = Actor(out var health, out var stamina, out var stun, out var guard, out var dust);
        var player = new GameObject("real Slayer consumer"); owned.Add(player); player.SetActive(false);
        var animator = player.AddComponent<Animator>();
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        var attacks = player.AddComponent<PlayerAttackEvents>();
        var mode = player.AddComponent<PlayerSlayerMode>();
        Set(mode, "animator", animator); Set(mode, "attackEvents", attacks); Set(mode, "enemy", health);
        player.SetActive(true); animator.Update(0);
        stamina.PreparePractice(0); stun.TriggerStun(.3f); mode.ActivateForStun(stun);
        Assert.That(mode.IsActive, Is.True); Assert.That(guard.active.Count, Is.EqualTo(1));
        stun.enabled = false; // Same component lifetime as the Slayer hit-pose path.
        health.TakeDamage(9999, Vector2.zero); health.Anim_DeathImpact();
        Assert.That(mode.IsActive, Is.False); Assert.That(guard.active, Is.Empty);
        Assert.That(dust.active.Count, Is.EqualTo(1));
        yield return new WaitForSecondsRealtime(.35f); Assert.That(dust.active, Is.Empty);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator LethalHitHasNoGuardAndDeathContactIsOnceAndSurvivesActorDestruction()
    {
        var binder = Actor(out var health, out var stamina, out var stun, out var guard, out var dust);
        var session = Host<CombatSession>("session");
        var expected = binder.transform.TransformPoint(new Vector3(.2f, .4f, 0)); expected.y = -2;
        health.TakeDamage(9999, Vector2.zero); stamina.ConsumeHit(9999); stun.TriggerStun(1);
        Assert.That(health.IsDead, Is.True); Assert.That(CombatSession.Finished, Is.True); Assert.That(guard.active, Is.Empty);
        binder.GetComponent<BoxCollider2D>().enabled = false; binder.transform.position += Vector3.right * 10;
        health.Anim_DeathImpact(); health.Anim_DeathImpact();
        Assert.That(dust.active.Count, Is.EqualTo(1));
        Assert.That(dust.active[0].transform.position, Is.EqualTo(expected));
        Object.Destroy(binder.gameObject); yield return null;
        Assert.That(dust.active.Count, Is.EqualTo(1));
        yield return new WaitForSecondsRealtime(.35f); Assert.That(dust.active, Is.Empty);
    }
    [Test, Category("Quick")]
    public void EveryDeathClipKeepsCleanupAndHasExactlyOneFirstTorsoContact()
    {
        foreach (var item in new[] { ("Assets/Art/EndlessSkeletons/sword/Die.anim", .7f, 1.3f),
            ("Assets/Art/EndlessSkeletons/sword-shield/Die.anim", .7f, 1.3f),
            ("Assets/Art/EndlessSkeletons/axe/Die.anim", .5f, 1.1f), ("Assets/Scripts/skeleton_die.anim", .48333332f, .8333333f) })
        {
            var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(item.Item1);
            var events = UnityEditor.AnimationUtility.GetAnimationEvents(clip);
            Assert.That(events.Count(e => e.functionName == "Anim_DeathImpact"), Is.EqualTo(1));
            Assert.That(events.Single(e => e.functionName == "Anim_DeathImpact").time, Is.EqualTo(item.Item2).Within(.00001));
            Assert.That(events.Single(e => e.functionName == "Anim_DieCleanup").time, Is.EqualTo(item.Item3).Within(.00001));
        }
    }
}
#endif
