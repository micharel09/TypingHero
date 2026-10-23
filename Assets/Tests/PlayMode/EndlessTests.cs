using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class EndlessTests
{
    [Test, Category("Quick")]
    public void DifficultyDefaultsPersistsAndSnapshotsAcrossContinueAndRestart()
    {
        bool had = PlayerPrefs.HasKey(EndlessDifficulties.SelectionKey);
        string previous = PlayerPrefs.GetString(EndlessDifficulties.SelectionKey);
        try
        {
            PlayerPrefs.DeleteKey(EndlessDifficulties.SelectionKey);
            Assert.That(EndlessDifficulties.Selected, Is.EqualTo(EndlessDifficulty.Normal));
            foreach (var difficulty in EndlessDifficulties.All)
            {
                EndlessRun.Clear(); EndlessDifficulties.Selected = difficulty;
                Assert.That(PlayerPrefs.GetString(EndlessDifficulties.SelectionKey), Is.EqualTo(difficulty.Id()));
                EndlessRun.Prepare();
                EndlessDifficulties.Selected = EndlessDifficulties.Step(difficulty, 1);
                var run = EndlessRun.EnterScene();
                Assert.That(run.Difficulty, Is.EqualTo(difficulty));
                run.Defeated = 5; run.Health = 17; run.Score = 500; run.Parries = 9;
                EndlessRun.Continuing = true;
                Assert.That(EndlessRun.EnterScene(), Is.SameAs(run));
                Assert.That(run.Health, Is.EqualTo(17)); Assert.That(run.Parries, Is.EqualTo(9));
                EndlessRun.Restart(); var restart = EndlessRun.EnterScene();
                Assert.That(restart.Difficulty, Is.EqualTo(difficulty));
                Assert.That(restart.Defeated + restart.Health + restart.Score + restart.Parries, Is.Zero);
            }
        }
        finally
        {
            if (had) PlayerPrefs.SetString(EndlessDifficulties.SelectionKey, previous);
            else PlayerPrefs.DeleteKey(EndlessDifficulties.SelectionKey);
            PlayerPrefs.Save(); EndlessRun.Clear();
        }
    }

    [Test, Category("Quick")]
    public void DifficultyScalesOnlyClampedRecoveryForEveryRoleEliteBossAndPressureCap()
    {
        float[] factors = { .6f, .8f, 1f, 1.25f, 1.5f };
        for (int level = 0; level < 5; level++)
        for (int defeated = 0; defeated < 90; defeated++)
        {
            var run = EndlessRun.Begin(EndlessDifficulties.All[level]); run.Defeated = defeated;
            var baseline = EndlessRun.Begin(); baseline.Defeated = defeated;
            Assert.That(run.Recovery, Is.EqualTo(baseline.Recovery / factors[level]).Within(.0001f));
            Assert.That(run.Enemy.Health, Is.EqualTo(baseline.Enemy.Health));
            Assert.That(run.Enemy.Stamina, Is.EqualTo(baseline.Enemy.Stamina));
            Assert.That(run.Definition, Is.SameAs(baseline.Definition));
        }
    }

    [Test, Category("Quick")]
    public void LocalBestsAreIsolatedAndNormalRetainsLegacyKeys()
    {
        var saved = new System.Collections.Generic.Dictionary<string, int?>();
        try
        {
            foreach (var difficulty in EndlessDifficulties.All)
            foreach (string legacy in new[] { EndlessRun.BestDefeatedKey, EndlessRun.BestScoreKey })
            {
                string key = EndlessDifficulties.StorageKey(difficulty, legacy);
                saved[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
                PlayerPrefs.DeleteKey(key);
            }
            Assert.That(EndlessDifficulties.StorageKey(EndlessDifficulty.Normal, EndlessRun.BestScoreKey), Is.EqualTo(EndlessRun.BestScoreKey));
            foreach (var difficulty in EndlessDifficulties.All)
            {
                var run = EndlessRun.Begin(difficulty); run.Defeated = (int)difficulty + 1; run.Score = 100;
                Assert.That(run.SaveBest(), Is.True);
                run.Score = 99; Assert.That(run.SaveBest(), Is.False);
            }
            foreach (var difficulty in EndlessDifficulties.All)
                Assert.That(PlayerPrefs.GetInt(EndlessDifficulties.StorageKey(difficulty, EndlessRun.BestDefeatedKey)), Is.EqualTo((int)difficulty + 1));
        }
        finally
        {
            foreach (var pair in saved)
                if (pair.Value.HasValue) PlayerPrefs.SetInt(pair.Key, pair.Value.Value); else PlayerPrefs.DeleteKey(pair.Key);
            PlayerPrefs.Save();
        }
    }
    static void AssertTypingAudioBinding()
    {
        var typing = Object.FindObjectOfType<TypingManager>();
        var binders = Object.FindObjectsOfType<TypingInputSfx>();
        Assert.That(binders.Length, Is.EqualTo(1));
        Assert.That(binders[0].gameObject, Is.EqualTo(typing.gameObject));
        var handlers = (System.Delegate)typeof(TypingManager).GetField("OnInputResult",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
        int count = 0;
        foreach (var handler in handlers.GetInvocationList())
            if (handler.Target is TypingInputSfx) count++;
        Assert.That(count, Is.EqualTo(1), "Reload must replace the typing audio subscription");
    }

    [TearDown]
    public void ResetRun()
    {
        EndlessRun.Clear();
        GamePauseController.SetEncounterReading(false);
    }

    [Test, Category("Quick")]
    public void SixOpponentsFinishWithoutBossHealingOrFurtherProgress()
    {
        var run = EndlessRun.Begin(); run.Health = 17;
        var names = new System.Collections.Generic.HashSet<string>();
        for (int i = 0; i < 6; i++)
        {
            Assert.That(run.Round, Is.EqualTo(1)); Assert.That(run.Slot, Is.EqualTo(i));
            names.Add(run.Enemy.Name); run.Win(40);
            Assert.That(run.Health, Is.EqualTo(17));
            Assert.That(run.Completed, Is.EqualTo(i == 5));
        }
        Assert.That(names.Count, Is.EqualTo(6));
        run.Win(40); run.Win(40);
        Assert.That(run.Defeated, Is.EqualTo(6));
        Assert.That(run.Health, Is.EqualTo(17));
    }

    [Test, Category("Quick")]
    public void BossCompletionIsIdempotentAndRestartRetainsEachDifficulty()
    {
        const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        Assert.That(Object.FindObjectOfType<FirebaseLeaderboard>(), Is.Null, "Offline fixture must never resolve a real provider");
        foreach (var difficulty in EndlessDifficulties.All)
        {
            string best = EndlessDifficulties.StorageKey(difficulty, EndlessRun.BestDefeatedKey);
            string score = EndlessDifficulties.StorageKey(difficulty, EndlessRun.BestScoreKey);
            string pending = EndlessDifficulties.StorageKey(difficulty, EndlessLeaderboard.PendingKey);
            string[] keys = { best, score, pending };
            var saved = new string[3];
            for (int i = 0; i < keys.Length; i++) saved[i] = PlayerPrefs.HasKey(keys[i]) ? PlayerPrefs.GetString(keys[i], PlayerPrefs.GetInt(keys[i]).ToString()) : null;
            var host = new GameObject("Finite run fixture");
            var player = host.AddComponent<PlayerHealth>();
            var session = host.AddComponent<CombatSession>();
            var duel = host.AddComponent<EndlessDuel>();
            try
            {
                PlayerPrefs.SetInt(best, 18); PlayerPrefs.SetInt(score, 9999); PlayerPrefs.DeleteKey(pending);
                var run = EndlessRun.Begin(difficulty); run.Defeated = 5; run.Health = player.Current;
                typeof(EndlessDuel).GetField("<Run>k__BackingField", flags).SetValue(duel, run);
                typeof(EndlessDuel).GetField("player", flags).SetValue(duel, player);
                session.Complete(true);
                var queued = PlayerPrefs.GetString(pending);
                Assert.That(EndlessLeaderboard.Pending(difficulty).defeated, Is.EqualTo(6));
                Assert.That(session.RunWon, Is.True);
                session.Complete(true); session.Complete(false); duel.EnemyDefeated();
                duel.EnemyDeathFinished(); duel.EnemyDeathFinished(); duel.Continue();
                Assert.That(duel.FinishRun(true), Does.Contain("BOSS DEFEATED"));
                Assert.That(PlayerPrefs.GetString(pending), Is.EqualTo(queued));
                Assert.That(run.Defeated, Is.EqualTo(6));
                Assert.That(run.Health, Is.EqualTo(player.Current));
                Assert.That(PlayerPrefs.GetInt(best), Is.EqualTo(18));
                Assert.That(PlayerPrefs.GetInt(score), Is.EqualTo(9999));
                EndlessRun.Restart();
                var fresh = EndlessRun.EnterScene();
                Assert.That(fresh.Difficulty, Is.EqualTo(difficulty));
                Assert.That(fresh.Defeated + fresh.Score + fresh.Health + fresh.Letters + fresh.Correct + fresh.Parries, Is.Zero);
                Assert.That(fresh.Seconds, Is.Zero);
            }
            finally
            {
                Object.DestroyImmediate(host);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (saved[i] == null) PlayerPrefs.DeleteKey(keys[i]);
                    else if (i < 2) PlayerPrefs.SetInt(keys[i], int.Parse(saved[i]));
                    else PlayerPrefs.SetString(keys[i], saved[i]);
                }
                PlayerPrefs.Save();
            }
        }
        Object.DestroyImmediate(EndlessLeaderboard.Get().gameObject);
    }

    [Test, Category("Quick")]
    public void RolesHaveDifferentPatternsAndPressureHasAFloor()
    {
        var run = EndlessRun.Begin();
        Assert.That(run.Role, Is.EqualTo(EndlessRole.Swordsman));
        run.Defeated = 1; Assert.That(run.Definition.Pattern(0, 1, 1), Is.EqualTo(new[] { 1, 2 }));
        run.Defeated = 2; Assert.That(run.Role, Is.EqualTo(EndlessRole.Heavy));
        run.Defeated = 5;
        Assert.That(run.Definition.Pattern(0, 1, 1).Length, Is.EqualTo(1));
        Assert.That(run.Definition.Pattern(1, 1, 1).Length, Is.EqualTo(2));
        Assert.That(run.Definition.Pattern(2, 1, .5f).Length, Is.EqualTo(3));
        Assert.That(run.Definition.Pattern(2, 1, 1).Length, Is.EqualTo(1));
        int hp = run.Enemy.Health;
        run.Defeated = 6005;
        Assert.That(run.Enemy.Health, Is.EqualTo(hp));
        Assert.That(run.Recovery, Is.GreaterThanOrEqualTo(.7f));
    }

    [Test, Category("Quick")]
    public void ContinueIsOneUseAndFreshSceneOrRestartDoesNotReuseRun()
    {
        var run = EndlessRun.EnterScene(); run.Defeated = 9; run.Health = 13;
        EndlessRun.Continuing = true;
        Assert.That(EndlessRun.EnterScene(), Is.SameAs(run));
        Assert.That(EndlessRun.Continuing, Is.False);
        Assert.That(EndlessRun.EnterScene().Defeated, Is.Zero);
        EndlessRun.Continuing = true; EndlessRun.Clear();
        Assert.That(EndlessRun.EnterScene().Health, Is.Zero);
    }

    [Test, Category("Quick")]
    public void ApprovedScheduleIntroducesAllRolesAndRotatesAtPressureCap()
    {
        EndlessRole[][] rounds = {
            new[] { EndlessRole.Swordsman, EndlessRole.DoubleStrike, EndlessRole.Heavy, EndlessRole.Guard, EndlessRole.Swordsman, EndlessRole.Captain },
            new[] { EndlessRole.Berserker, EndlessRole.DoubleStrike, EndlessRole.Guard, EndlessRole.Heavy, EndlessRole.Heavy, EndlessRole.Captain },
            new[] { EndlessRole.Swordsman, EndlessRole.DoubleStrike, EndlessRole.Heavy, EndlessRole.Berserker, EndlessRole.Swordsman, EndlessRole.Captain }
        };
        var run = EndlessRun.Begin();
        for (int round = 0; round < rounds.Length; round++)
        for (int slot = 0; slot < 6; slot++)
        {
            run.Defeated = round * 6 + slot;
            Assert.That(run.Role, Is.EqualTo(rounds[round][slot]));
            Assert.That(run.Enemy.Stamina, Is.EqualTo(slot == 5 ? 200 : slot == 4 ? 150 : 100));
        }
        run.Defeated = 8 * 6;
        float capped = run.Recovery;
        run.Defeated += 5 * 6;
        Assert.That(run.Recovery, Is.EqualTo(capped), "Same role five rounds later keeps capped pressure");
        Assert.That(run.Definition.Pattern(0, 2, 1), Is.Not.Empty);
        run.Defeated = 3;
        Assert.That(run.Definition.hitCost, Is.EqualTo(4));
        Assert.That(run.Definition.damage, Is.EqualTo(5));
    }

    [Test, Category("Quick")]
    public void EveryFormOwnsAllReactionStatesAndContactEvents()
    {
        foreach (var role in new[] { EndlessRole.Swordsman, EndlessRole.Heavy, EndlessRole.Guard })
        {
            var definition = Resources.Load<EndlessOpponent>("Endless/" + role);
            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.idle, Is.Not.Null);
            Assert.That(definition.hurt.length, Is.EqualTo(.5f).Within(.001f));
            foreach (var clip in definition.controller.animationClips)
            {
                if (!clip.name.StartsWith("Attack")) continue;
                int impacts = 0;
                foreach (var animationEvent in clip.events)
                {
                    if (animationEvent.functionName != "Anim_DealDamage") continue;
                    impacts++;
                    float contact = role == EndlessRole.Heavy ? (clip.name == "Attack1" ? .6f : .4f) : clip.name == "Attack1" ? .5f : .2f;
                    Assert.That(animationEvent.time, Is.EqualTo(contact).Within(.001f));
                }
                Assert.That(impacts, Is.EqualTo(1));
                Assert.That(clip.isLooping, Is.False);
            }
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator RiposteHurtCannotAccelerateSecondCueIntoParrySuccessPose()
    {
        var host = new GameObject("Riposte recovery fixture"); host.SetActive(false);
        var animator = host.AddComponent<Animator>(); host.AddComponent<SpriteRenderer>();
        host.AddComponent<AttackStrike>();
        var brain = host.AddComponent<SkeletonController>();
        var cue = new GameObject("Cue"); cue.transform.SetParent(host.transform); cue.SetActive(false);
        const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        typeof(SkeletonController).GetField("animator", flags).SetValue(brain, animator);
        typeof(SkeletonController).GetField("attackCue", flags).SetValue(brain, cue);
        host.SetActive(true); brain.ConfigureEndless(new EndlessRun { Defeated = 1 });
        // No receiver is needed: the first contact is observed through the cue.
        try
        {
            float deadline = Time.realtimeSinceStartup + 5;
            while (!cue.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(cue.activeSelf, Is.True);
            float firstCue = Time.time;
            while (cue.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
            animator.Play("Base Layer.skeleton_hit", 0, 0);
            animator.Update(0);
            while (!cue.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(cue.activeSelf, Is.True);
            Assert.That(brain.LastAttack, Is.EqualTo("Attack2"));
            Assert.That(Time.time - firstCue, Is.GreaterThanOrEqualTo(1.09f),
                "Hurt retains the authored first swing duration plus chain recovery");
        }
        finally { Object.DestroyImmediate(host); }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator AllSixRealAttacksKeepCueContactAndPauseWithoutDuplicateDamage()
    {
        int[] encounters = { 0, 12, 2, 6, 3, 3 };
        for (int test = 0; test < encounters.Length; test++)
        {
            var receiver = new GameObject("Cue fixture player").AddComponent<PlayerHealth>();
            receiver.maxHealth = 40; receiver.FullRestore();
            var host = new GameObject("Real form cue fixture"); host.SetActive(false);
            var animator = host.AddComponent<Animator>();
            host.AddComponent<SpriteRenderer>(); host.AddComponent<AttackStrike>();
            var brain = host.AddComponent<SkeletonController>();
            var cue = new GameObject("Cue"); cue.transform.SetParent(host.transform); cue.SetActive(false);
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            typeof(SkeletonController).GetField("animator", flags).SetValue(brain, animator);
            typeof(SkeletonController).GetField("attackCue", flags).SetValue(brain, cue);
            host.SetActive(true);
            var run = new EndlessRun { Defeated = encounters[test] };
            brain.ConfigureEndless(run);
            if (test == 1 || test == 5) typeof(SkeletonController).GetField("pattern", flags).SetValue(brain, 1);
            try
            {
                float deadline = Time.realtimeSinceStartup + 4;
                while (!cue.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(cue.activeSelf, Is.True, "Cue for case " + test);
                float cueTime = Time.time;
                GamePauseController.SetEncounterReading(true);
                float held = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                yield return new WaitForSecondsRealtime(.15f);
                Assert.That(receiver.Current, Is.EqualTo(40)); Assert.That(cue.activeSelf, Is.True);
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, Is.EqualTo(held).Within(.01f));
                GamePauseController.SetEncounterReading(false);
                while (receiver.Current == 40 && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(Time.time - cueTime, Is.InRange(.24f, .4f), "300ms cue, case " + test);
                int expected = test == 2 ? 32 : 35;
                Assert.That(receiver.Current, Is.EqualTo(expected));
                host.GetComponent<AttackStrike>().Anim_DealDamage();
                Assert.That(receiver.Current, Is.EqualTo(expected), "Impact is spent exactly once");
            }
            finally
            {
                GamePauseController.SetEncounterReading(false);
                Object.DestroyImmediate(host); Object.DestroyImmediate(receiver.gameObject);
            }
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator HeavyDamageIsSpentOnceAndNormalSwingRestoresItsOwnDamage()
    {
        var player = new GameObject("Endless damage receiver").AddComponent<PlayerHealth>();
        var attacker = new GameObject("Endless strike").AddComponent<AttackStrike>();
        try
        {
            attacker.BeginSwing(8); attacker.Anim_DealDamage(); attacker.Anim_DealDamage();
            Assert.That(player.Current, Is.EqualTo(player.maxHealth - 8));
            yield return new WaitForSecondsRealtime(.1f);
            attacker.BeginSwing(); attacker.Anim_DealDamage();
            Assert.That(player.Current, Is.EqualTo(player.maxHealth - 13));
        }
        finally { Object.Destroy(player.gameObject); Object.Destroy(attacker.gameObject); }
        yield return null;
    }

    [Test, Category("Quick")]
    public void BestRanksSurvivalBeforeScoreAndUsesSeparatePreferences()
    {
        string a = EndlessRun.BestDefeatedKey, b = EndlessRun.BestScoreKey;
        bool hasA = PlayerPrefs.HasKey(a), hasB = PlayerPrefs.HasKey(b);
        int oldA = PlayerPrefs.GetInt(a), oldB = PlayerPrefs.GetInt(b);
        try
        {
            PlayerPrefs.SetInt(a, 4); PlayerPrefs.SetInt(b, 100);
            var run = EndlessRun.Begin(); run.Defeated = 3; run.Score = 99999;
            Assert.That(run.SaveBest(), Is.False);
            run.Defeated = 5; run.Score = 10;
            Assert.That(run.SaveBest(), Is.True);
            run.Score = 20; Assert.That(run.SaveBest(), Is.True);
            Assert.That(PlayerPrefs.GetInt(a), Is.EqualTo(5)); Assert.That(PlayerPrefs.GetInt(b), Is.EqualTo(20));
        }
        finally
        {
            if (hasA) PlayerPrefs.SetInt(a, oldA); else PlayerPrefs.DeleteKey(a);
            if (hasB) PlayerPrefs.SetInt(b, oldB); else PlayerPrefs.DeleteKey(b);
            PlayerPrefs.Save();
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ActualSceneCarriesHealthAndStatisticsAndResetsCombatBindings()
    {
        TutorialRun.End(); EndlessRun.Clear();
        var previousMusic = MusicPlayer.I;
        var previousSfx = SfxPlayer.I;
        yield return DungeonSceneTestFlow.Start();
        yield return null;
        try
        {
            var duel = EndlessDuel.I;
            Assert.That(duel, Is.Not.Null);
            AssertTypingAudioBinding();
            Assert.That(GamePauseController.EncounterReading, Is.False, "Fresh encounters start without an intro gate");
            var player = Object.FindObjectOfType<PlayerHealth>();
            var enemy = Object.FindObjectOfType<SkeletonController>();
            Assert.That(enemy.Current, Is.EqualTo(180));
            duel.Continue();
            player.TakeDamage(5, Vector2.zero);
            Assert.That(player.Current, Is.EqualTo(35));
            ScoreSystem.I.RestoreRunScore(123);
            enemy.TakeDamage(10000, Vector2.zero);
            Assert.That(CombatSession.Finished, Is.True);
            Assert.That(duel.Run.Defeated, Is.EqualTo(1));
            Assert.That(duel.Run.Score, Is.EqualTo(123));
            yield return new WaitForSecondsRealtime(2);
            Assert.That(Object.FindObjectOfType<SkeletonController>(), Is.Null);
            Assert.That(Object.FindObjectOfType<StaminaBarUI>(), Is.Null, "No floating stamina bar after enemy cleanup");
            duel.Continue();
            yield return DungeonSceneTestFlow.Wait();
            yield return null; yield return null;
            Assert.That(GamePauseController.EncounterReading, Is.False);
            Assert.That(CombatSession.Finished, Is.False);
            AssertTypingAudioBinding();
            Assert.That(EndlessDuel.I.Run.Slot, Is.EqualTo(1));
            Assert.That(Object.FindObjectOfType<PlayerHealth>().Current, Is.EqualTo(35));
            Assert.That(ScoreSystem.I.Score, Is.EqualTo(123));
            Assert.That(Object.FindObjectOfType<EnemyStamina>().Current, Is.EqualTo(100));
            Assert.That(Object.FindObjectOfType<PlayerSlayerMode>().IsActive, Is.False);
            Assert.That(Object.FindObjectOfType<ParrySystem>().IsWindowActive, Is.False);
            Assert.That(TypingManager.MuteWordCorrect, Is.False);
            var pause = Object.FindObjectOfType<GamePauseController>();
            pause.TogglePause(true); pause.Resume();
            yield return new WaitForSecondsRealtime(.4f);
            Assert.That(GamePauseController.InputBlocked, Is.False, "Continue loads and starts combat without another Fight");
            EndlessDuel.I.Continue();
            Assert.That(GamePauseController.InputBlocked, Is.False);
            var heavy = new EndlessRun { Defeated = 2 };
            enemy = Object.FindObjectOfType<SkeletonController>();
            enemy.ConfigureEndless(heavy);
            var cue = enemy.transform.Find("AttackCue").gameObject;
            float cueTime = -1, impactTime = -1;
            float deadline = Time.realtimeSinceStartup + 5;
            while (Time.realtimeSinceStartup < deadline && impactTime < 0)
            {
                if (cue.activeInHierarchy && cueTime < 0) cueTime = Time.time;
                if (Object.FindObjectOfType<PlayerHealth>().Current < 35) impactTime = Time.time;
                yield return null;
            }
            Assert.That(Object.FindObjectOfType<PlayerHealth>().Current, Is.EqualTo(27));
            Assert.That(cueTime, Is.GreaterThan(0));
            Assert.That(impactTime - cueTime, Is.InRange(.24f, .4f), "Heavy preserves the learned parry cue lead");
            // Public entry point must reject Endless before even resolving a provider.
            Object.FindObjectOfType<LeaderboardSubmitter>().SubmitFinalScore(999);
        }
        finally
        {
            // Test scene cleanup must also happen if an assertion fails.
            var clean = SceneManager.CreateScene("EndlessTestCleanup");
            SceneManager.SetActiveScene(clean);
            SceneManager.UnloadSceneAsync("SampleScene");
            DungeonSceneTestFlow.Cleanup();
            if (!previousMusic && MusicPlayer.I) Object.Destroy(MusicPlayer.I.gameObject);
            if (!previousSfx && SfxPlayer.I) Object.Destroy(SfxPlayer.I.gameObject);
            AudioListener.pause = false;
        }
        yield return null; yield return null;
    }
}
