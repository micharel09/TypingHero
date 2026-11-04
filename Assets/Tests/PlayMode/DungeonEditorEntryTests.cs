#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class DungeonEditorEntryTests
{
    const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    static void SetPrivate(object target, string name, object value)
        => target.GetType().GetField(name, PrivateInstance).SetValue(target, value);

    static T GetPrivate<T>(object target, string name)
        => (T)target.GetType().GetField(name, PrivateInstance).GetValue(target);

    static void SetPrivateProperty(object target, string name, object value)
    {
        var property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.GetSetMethod(true).Invoke(target, new[] { value });
    }

    static void BeginSemiSlayer(SemiSlayerMode mode)
        => typeof(SemiSlayerMode).GetMethod("Begin", PrivateInstance).Invoke(mode, null);

    IEnumerator VerifyLethalSemiSlayerSwing(int lethalSwing, bool bossEnd)
    {
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        yield return DungeonSceneTestFlow.Wait();

        var mode = Object.FindObjectOfType<SemiSlayerMode>();
        var player = Object.FindObjectOfType<PlayerHealth>();
        var enemy = Object.FindObjectOfType<SkeletonController>();
        var typing = Object.FindObjectOfType<TypingManager>();
        var attackEvents = player.GetComponent<PlayerAttackEvents>();
        var animator = player.GetComponent<Animator>();
        var run = EndlessDuel.I.Run;
        float introDeadline = Time.realtimeSinceStartup + 5f;
        while ((DungeonSession.InputSuppressed || GamePauseController.InputBlocked || typing.IsReadyPrompt) &&
               Time.realtimeSinceStartup < introDeadline) yield return null;
        Assert.That(DungeonSession.InputSuppressed || GamePauseController.InputBlocked || typing.IsReadyPrompt, Is.False);

        if (bossEnd) run.Defeated = EndlessRun.DuelsPerRound - 1;
        run.SemiSlayerCharge = 100;
        SetPrivateProperty(enemy, "Current", lethalSwing * 5);
        float originalSpeed = animator.GetFloat("AttackSpeedMul");
        float originalHitWindow = attackEvents.minOpenSeconds;
        BeginSemiSlayer(mode);

        float deadline = Time.realtimeSinceStartup + 3f;
        while (mode.IsActive && Time.realtimeSinceStartup < deadline) yield return null;
        float idleDeadline = Time.realtimeSinceStartup + 1f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_idle") &&
               Time.realtimeSinceStartup < idleDeadline) yield return null;
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_idle"), Is.True,
            "a live player returns to idle after an enemy's lethal hit, even when combat is finished");
        Assert.That(mode.IsActive, Is.False, $"lethal swing {lethalSwing} must end the skill");
        Assert.That(enemy.IsDead, Is.True, $"lethal swing {lethalSwing} must hit the live target");
        Assert.That(mode.CompletedSwings, Is.EqualTo(lethalSwing), "the synchronous lethal hit counts once and stops the chain");
        Assert.That(GetPrivate<float>(mode, "swingDelay"), Is.Zero, "the lethal callback cannot leave a next-swing delay scheduled");
        Assert.That(attackEvents.hitbox.GetComponent<Collider2D>().enabled, Is.False, "lethal cleanup closes the weapon hitbox");
        Assert.That(enemy.SemiSlayerHeld, Is.False, "lethal cleanup releases the enemy lock");
        Assert.That(GetPrivate<bool>(typing, "semiSlayerHeld"), Is.False, "lethal cleanup releases the typing lock");
        Assert.That(attackEvents.minOpenSeconds, Is.EqualTo(originalHitWindow), "lethal cleanup restores the original hit window");
        Assert.That(animator.GetFloat("AttackSpeedMul"), Is.EqualTo(originalSpeed), "lethal cleanup restores attack speed");
        Assert.That(CombatSession.Finished, Is.True);
        if (bossEnd) Assert.That(run.Completed, Is.True, "the last boss finishes the run");
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        foreach (var name in new[] { DungeonSession.GameScene, "MainMenu", LightingTestAssets.WorldScene })
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        }
        if (DungeonSession.Current) Object.Destroy(DungeonSession.Current.gameObject);
        TutorialRun.End(); EndlessRun.Clear(); GamePauseController.ResetSessionState();
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator AdoptsLoadedGameplayAndInspectorEditsIntoAFreshFight()
    {
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var gate = Object.FindObjectOfType<DungeonSessionBindings>(true);
        Assert.That(gate.gameObject.activeSelf, Is.False, "direct scene was previously inert");
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        var player = gate.GetComponentInChildren<PlayerHealth>(true);
        player.maxHealth = 53; // An unsaved Inspector edit must survive bootstrap.
        TutorialRun.Begin();
        EndlessRun.Begin().Score = 123;
        GamePauseController.SetTutorialReading(true);
        int progress = PlayerPrefs.GetInt("TutorialProgress_v1", 0);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        Assert.That(session.TryBeginEditorGameplay(scene), Is.False, "duplicate startup");
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(SceneManager.GetActiveScene(), Is.EqualTo(scene));
        Assert.That(player.maxHealth, Is.EqualTo(53));
        Assert.That(player.Current, Is.EqualTo(53));
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(PlayerPrefs.GetInt("TutorialProgress_v1", 0), Is.EqualTo(progress));
        Assert.That(EndlessDuel.I.Run.Score, Is.Zero);
        Assert.That(EndlessDuel.I.Run.Difficulty, Is.EqualTo(EndlessDifficulties.Selected));
        Assert.That(CombatSession.I.Seconds, Is.InRange(0f, 1f), "Fresh encounter clock starts immediately");
        Assert.That(GamePauseController.IsPaused, Is.False);
        Assert.That(GamePauseController.EncounterReading, Is.False, "combat starts without Fight");
        Assert.That(Object.FindObjectsOfType<PlayerHealth>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<SkeletonController>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<AudioListener>().Length, Is.EqualTo(1));
        Assert.That(session.TryBeginEditorGameplay(scene), Is.False, "already activated");
        EndlessDuel.I.Continue();
        yield return null;
        Assert.That(GamePauseController.InputBlocked, Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SuccessfulParryAddsTwentySemiSlayerCharge()
    {
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        yield return DungeonSceneTestFlow.Wait();

        var mode = Object.FindObjectOfType<SemiSlayerMode>();
        var parry = Object.FindObjectOfType<ParrySystem>();
        var typing = Object.FindObjectOfType<TypingManager>();
        Assert.That(mode, Is.Not.Null);
        Assert.That(parry, Is.Not.Null);
        Assert.That(typing, Is.Not.Null);
        float deadline = Time.realtimeSinceStartup + 5f;
        while ((DungeonSession.InputSuppressed || GamePauseController.InputBlocked || typing.IsReadyPrompt) &&
               Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(DungeonSession.InputSuppressed || GamePauseController.InputBlocked || typing.IsReadyPrompt, Is.False);

        var run = EndlessDuel.I.Run;
        run.SemiSlayerCharge = 40;
        parry.ParrySuccess();
        Assert.That(run.SemiSlayerCharge, Is.EqualTo(60));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SemiSlayerLethalFirstSwingReturnsIdleAndStopsChain()
        => VerifyLethalSemiSlayerSwing(1, false);

    [UnityTest, Category("Quick")]
    public IEnumerator SemiSlayerLethalFifthSwingReturnsIdleAndStopsChain()
        => VerifyLethalSemiSlayerSwing(5, false);

    [UnityTest, Category("Quick")]
    public IEnumerator SemiSlayerLethalTenthSwingReturnsIdleAndStopsChain()
        => VerifyLethalSemiSlayerSwing(10, false);

    [UnityTest, Category("Quick")]
    public IEnumerator SemiSlayerLethalBossSwingReturnsIdleAndStopsChain()
        => VerifyLethalSemiSlayerSwing(1, true);

    [UnityTest, Category("Quick")]
    public IEnumerator PlayerDamageAndDeathPreserveTheirOwnAnimations()
    {
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        yield return DungeonSceneTestFlow.Wait();

        var mode = Object.FindObjectOfType<SemiSlayerMode>();
        var player = Object.FindObjectOfType<PlayerHealth>();
        var enemy = Object.FindObjectOfType<SkeletonController>();
        var typing = Object.FindObjectOfType<TypingManager>();
        var animator = player.GetComponent<Animator>();
        float introDeadline = Time.realtimeSinceStartup + 5f;
        while ((DungeonSession.InputSuppressed || GamePauseController.InputBlocked || typing.IsReadyPrompt) &&
               Time.realtimeSinceStartup < introDeadline) yield return null;
        Assert.That(DungeonSession.InputSuppressed || GamePauseController.InputBlocked || typing.IsReadyPrompt, Is.False);

        SetPrivateProperty(enemy, "Current", 500);
        player.postHitIFrames = 0f;
        EndlessDuel.I.Run.SemiSlayerCharge = 100;
        BeginSemiSlayer(mode);
        Assert.That(mode.IsActive, Is.True);
        player.TakeDamage(1, Vector2.zero);
        Assert.That(mode.IsActive, Is.False, "nonlethal damage cancels the chain");
        float hitDeadline = Time.realtimeSinceStartup + 1f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_hit") &&
               Time.realtimeSinceStartup < hitDeadline) yield return null;
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_hit"), Is.True,
            "cleanup does not overwrite the hit animation");

        EndlessDuel.I.Run.SemiSlayerCharge = 100;
        BeginSemiSlayer(mode);
        player.TakeDamage(player.Current, Vector2.zero);
        Assert.That(mode.IsActive, Is.False, "lethal damage cancels the chain");
        Assert.That(player.IsDead, Is.True);
        float deathDeadline = Time.realtimeSinceStartup + 1f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_die") &&
               Time.realtimeSinceStartup < deathDeadline) yield return null;
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_die"), Is.True,
            "cleanup does not overwrite the death animation");
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SemiSlayerTeardownReleasesLocksWithoutForcingIdle()
    {
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        yield return DungeonSceneTestFlow.Wait();

        var mode = Object.FindObjectOfType<SemiSlayerMode>();
        var player = Object.FindObjectOfType<PlayerHealth>();
        var enemy = Object.FindObjectOfType<SkeletonController>();
        var typing = Object.FindObjectOfType<TypingManager>();
        var attackEvents = player.GetComponent<PlayerAttackEvents>();
        var animator = player.GetComponent<Animator>();
        SetPrivateProperty(enemy, "Current", 500);
        EndlessDuel.I.Run.SemiSlayerCharge = 100;
        float originalSpeed = animator.GetFloat("AttackSpeedMul");
        float originalHitWindow = attackEvents.minOpenSeconds;
        BeginSemiSlayer(mode);
        mode.enabled = false;

        Assert.That(mode.IsActive, Is.False);
        Assert.That(attackEvents.hitbox.GetComponent<Collider2D>().enabled, Is.False);
        Assert.That(enemy.SemiSlayerHeld, Is.False);
        Assert.That(GetPrivate<bool>(typing, "semiSlayerHeld"), Is.False);
        Assert.That(attackEvents.minOpenSeconds, Is.EqualTo(originalHitWindow));
        Assert.That(animator.GetFloat("AttackSpeedMul"), Is.EqualTo(originalSpeed));
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_idle"), Is.False,
            "teardown must not replace the in-flight animation with idle");
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ReusesOpenWorldThroughDirectStartRestartAndMenuStart()
    {
        var session = DungeonSession.I;
        yield return session.LoadWorld(LightingTestAssets.Definition);
        var world = session.World;
        var camera = world.view;
        var texture = world.Arena.WorldTexture;
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        yield return DungeonSceneTestFlow.Wait();
        EndlessRun.Restart(); session.ReloadGameplay();
        yield return DungeonSceneTestFlow.Wait();
        session.ReturnToMenu();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        Assert.That(Time.timeScale, Is.EqualTo(1));
        TutorialRun.End(); EndlessRun.Prepare(); session.BeginGame();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(session.World, Is.SameAs(world));
        Assert.That(world.view, Is.SameAs(camera));
        Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
        Assert.That(Object.FindObjectsOfType<DungeonWorld>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<DungeonArenaCamera>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<DungeonPresentationCamera>().Length, Is.EqualTo(1));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LoadedMenuRetainsOwnershipOfStart()
    {
        yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        yield return null; // Let MainMenu.Start acquire the world-load lock.
        yield return DungeonSceneTestFlow.Wait();
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.False);
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        Assert.That(CombatSession.I, Is.Null);
        TutorialRun.Begin();
        session.BeginGame();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Active, Is.True, "normal menu dispatch is preserved");
        Assert.That(Object.FindObjectsOfType<TutorialDirector>().Length, Is.EqualTo(1));
    }
}
#endif
