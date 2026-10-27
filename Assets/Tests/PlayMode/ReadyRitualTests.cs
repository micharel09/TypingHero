using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public sealed class ReadyRitualTests
{
    DungeonSession session;
    ReadyRitualPresentation ritual;
    TypingManager typing;
    string pendingKey, savedPending;
    static T Find<T>() where T : Object => Object.FindObjectOfType<T>();
    static T Field<T>(object owner, string name) => (T)owner.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(owner);
    string Phase => Field<object>(ritual, "phase").ToString();
    string Word => Field<string>(typing, "current");
    IEnumerator Until(Func<bool> predicate, float seconds = 15)
    {
        float end = Time.realtimeSinceStartup + seconds;
        while (!predicate() && Time.realtimeSinceStartup < end) yield return null;
        Assert.That(predicate(), Is.True, "Ritual milestone timed out");
    }
    [UnitySetUp] public IEnumerator Setup()
    {
        TutorialRun.End(); EndlessRun.Prepare(); GamePauseController.ResetSessionState();
        session = DungeonSession.I;
        yield return session.LoadWorld(LightingTestAssets.Definition);
        session.BeginGame(); yield return DungeonSceneTestFlow.Wait();
        ritual = Find<ReadyRitualPresentation>(); typing = Find<TypingManager>();
        // Terminal-victory coverage must never resolve the scene's real provider.
        foreach (var provider in Object.FindObjectsOfType<FirebaseLeaderboard>())
            typeof(FirebaseLeaderboard).GetField("databaseRootUrl", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(provider, "");
        pendingKey = EndlessDifficulties.StorageKey(EndlessDuel.I.Run.Difficulty, EndlessLeaderboard.PendingKey);
        savedPending = PlayerPrefs.HasKey(pendingKey) ? PlayerPrefs.GetString(pendingKey) : null;
        PlayerPrefs.DeleteKey(pendingKey);
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        if (session) Object.Destroy(session.gameObject);
        yield return null;
        foreach (var name in new[] { "SampleScene", "MainMenu", LightingTestAssets.WorldScene })
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        }
        TutorialRun.End(); EndlessRun.Clear(); GamePauseController.ResetSessionState();
        if (pendingKey != null)
        {
            if (savedPending == null) PlayerPrefs.DeleteKey(pendingKey); else PlayerPrefs.SetString(pendingKey, savedPending);
        }
        if (Find<EndlessLeaderboard>()) Object.Destroy(Find<EndlessLeaderboard>().gameObject);
        yield return null;
    }
    IEnumerator Defeat()
    {
        Find<SkeletonController>().TakeDamage(10000, Vector2.zero);
        yield return Until(() => Phase == "Waiting");
        yield return null;
    }
    [UnityTest, Category("Quick")]
    public IEnumerator CachedFootProgressErrorAndRenderedChargeCarryOnceIntoBoss()
    {
        var foot = Field<Vector3>(ritual, "foot");
        var world = session.World; var camera = world.Arena;
        EndlessDuel.I.Run.Defeated = 4; // The same nonterminal ritual leads into the captain.
        yield return Defeat();
        Assert.That(Find<SkeletonController>(), Is.Null);
        Assert.That(Field<Vector3>(ritual, "foot"), Is.EqualTo(foot));
        Assert.That(camera.EnvironmentPresentationGain, Is.EqualTo(.85f).Within(.0001f));
        var ring = Field<ParticleSystem>(ritual, "ring");
        var markSystem = Field<ParticleSystem>(ritual, "marks");
        var particles = new ParticleSystem.Particle[5];
        Assert.That(markSystem.GetParticles(particles), Is.EqualTo(5));
        foreach (var particle in particles)
            Assert.That(particle.position.y, Is.EqualTo(world.environment.position.y + .015f).Within(.00001f), "Every mark sits above the floor");
        foreach (var vertex in Field<Mesh>(ritual, "ringMesh").vertices)
            Assert.That(vertex.y, Is.Zero.Within(.00001f), "The entire seal is on the ground plane");
        Assert.That(ring.particleCount, Is.EqualTo(1));
        int hp = Find<PlayerHealth>().Current, score = ScoreSystem.I.Score, letters = CombatSession.I.Letters;
        float seconds = CombatSession.I.Seconds;
        typing.ProcessInput("\r\n"); yield return new WaitForSecondsRealtime(.2f);
        Assert.That(Word, Is.EqualTo("READY")); Assert.That(session.Transitioning, Is.False);
        typing.ProcessInput("re"); yield return new WaitForSecondsRealtime(.09f);
        var levels = Field<float[]>(ritual, "levels");
        Assert.That(levels[0], Is.EqualTo(1)); Assert.That(levels[1], Is.EqualTo(1)); Assert.That(levels[2], Is.Zero);
        EndlessDuel.I.EnemyDeathFinished(); Assert.That(Word, Is.EqualTo("ADY"));
        typing.ProcessInput("x"); yield return new WaitForSecondsRealtime(.13f);
        Assert.That(levels, Is.All.EqualTo(0)); Assert.That(Word, Is.EqualTo("READY"));
        typing.ProcessInput("rEaDyREADYmore");
        Assert.That(session.Transitioning, Is.False);
        yield return Until(() => Field<bool>(ritual, "chargePainted"));
        double painted = Time.realtimeSinceStartupAsDouble;
        Assert.That(levels, Is.All.EqualTo(1));
        typing.ProcessInput("READY");
        Assert.That(CombatSession.I.Letters, Is.EqualTo(letters));
        Assert.That(CombatSession.I.Seconds, Is.EqualTo(seconds));
        yield return Until(() => session.Transitioning);
        Assert.That(Time.realtimeSinceStartupAsDouble - painted, Is.GreaterThanOrEqualTo(.18 - .01));
        yield return Until(() => Phase == "Summoning");
        yield return Until(() => Find<DuelIntroPresentation>().Running && Find<DuelIntroPresentation>().Elapsed >= .2f);
        Assert.That(ring, Is.Not.Null); Assert.That(Find<DuelIntroPresentation>().Running, Is.True);
        yield return Until(() => Find<DuelIntroPresentation>().Elapsed >= .6f);
        Assert.That(Phase, Is.EqualTo("Dormant"));
        Assert.That(camera.EnvironmentPresentationGain, Is.EqualTo(1));
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(session.World, Is.SameAs(world));
        Assert.That(EndlessDuel.I.Run.Slot, Is.EqualTo(5));
        Assert.That(Find<PlayerHealth>().Current, Is.EqualTo(hp));
        Assert.That(ScoreSystem.I.Score, Is.EqualTo(score));
        Assert.That(CombatSession.I.Letters, Is.EqualTo(letters));
        // Reload supplied a fresh provider; disconnect it before the terminal queue.
        foreach (var provider in Object.FindObjectsOfType<FirebaseLeaderboard>())
            typeof(FirebaseLeaderboard).GetField("databaseRootUrl", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(provider, "");
        Find<SkeletonController>().TakeDamage(10000, Vector2.zero);
        yield return new WaitForSecondsRealtime(2);
        Assert.That(EndlessDuel.I.Run.Completed, Is.True);
        Assert.That(Phase, Is.EqualTo("Dormant"), "Terminal victory never creates a READY ritual");
    }
    [UnityTest, Category("Quick")]
    public IEnumerator PauseAndFocusHoldArrivalProgressAndCompletionThenResume()
    {
        Find<SkeletonController>().TakeDamage(10000, Vector2.zero);
        yield return Until(() => Phase == "Arriving");
        var pause = Find<GamePauseController>();
        foreach (string stage in new[] { "Arriving", "Waiting", "Charged" })
        {
            if (stage == "Waiting") { yield return Until(() => Phase == stage); yield return null; typing.ProcessInput("re"); }
            if (stage == "Charged") { typing.ProcessInput("ady"); yield return Until(() => Field<bool>(ritual, "chargePainted")); }
            pause.SendMessage("OnApplicationFocus", false);
            float elapsed = Field<float>(ritual, "elapsed");
            var levels = (float[])Field<float[]>(ritual, "levels").Clone();
            float gain = session.World.Arena.EnvironmentPresentationGain;
            yield return new WaitForSecondsRealtime(.3f);
            Assert.That(Phase, Is.EqualTo(stage));
            Assert.That(Field<float>(ritual, "elapsed"), Is.EqualTo(elapsed));
            Assert.That(Field<float[]>(ritual, "levels"), Is.EqualTo(levels));
            Assert.That(session.World.Arena.EnvironmentPresentationGain, Is.EqualTo(gain));
            Assert.That(session.Transitioning, Is.False);
            typing.ProcessInput("READY");
            pause.Resume(); yield return Until(() => !GamePauseController.IsPaused);
        }
        yield return Until(() => session.Transitioning);
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(EndlessDuel.I.Run.Slot, Is.EqualTo(1));
        Assert.That(session.World.Arena.EnvironmentPresentationGain, Is.EqualTo(1));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator MenuAndRestartCancelWaitProgressAndChargedCallback()
    {
        for (int action = 0; action < 2; action++)
        for (int stage = 0; stage < 3; stage++)
        {
            if (action + stage > 0)
            {
                if (SceneManager.GetActiveScene().name == "MainMenu") session.BeginGame();
                yield return DungeonSceneTestFlow.Wait();
                typing = Find<TypingManager>();
            }
            yield return Defeat();
            if (stage > 0) typing.ProcessInput(stage == 1 ? "re" : "READY");
            if (stage == 2) yield return Until(() => Field<bool>(ritual, "chargePainted"));
            var pause = Find<GamePauseController>();
            pause.TogglePause(true);
            if (action == 0) pause.ReturnToMenu();
            else typeof(GamePauseController).GetMethod("TryRestart", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(pause, null);
            yield return Until(() => Phase == "Dormant");
            Assert.That(Field<Action>(ritual, "continuation"), Is.Null);
            Assert.That(session.World.Arena.EnvironmentPresentationGain, Is.EqualTo(1));
            Assert.That(ritual.GetComponentsInChildren<ParticleSystem>(), Is.Empty);
            yield return Until(() => session.Transitioning);
            yield return DungeonSceneTestFlow.Wait();
            Assert.That(Phase, Is.EqualTo("Dormant"));
            if (action == 0) Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("MainMenu"));
            else Assert.That(EndlessDuel.I.Run.Slot, Is.Zero);
        }
    }
    [UnityTest, Category("Quick")]
    public IEnumerator SealGainRestoresBaselineWithoutWritingSlayerAndCueUsesSfxPreferences()
    {
        yield return Defeat();
        var arena = session.World.Arena;
        var slayer = new Color(.8f, .03f, .02f, .4f);
        arena.SetSlayerBackground(slayer);
        float shaderWeight = Shader.GetGlobalFloat("_DungeonSlayerWeight");
        ritual.Cancel();
        arena.EnvironmentPresentationGain = .93f;
        session.BeginReady(typing, () => Assert.Fail("Cancelled seal cannot continue"));
        yield return Until(() => Phase == "Waiting"); yield return null;
        Assert.That(arena.EnvironmentPresentationGain, Is.EqualTo(.93f * .85f).Within(.0001f));
        Assert.That(arena.SlayerBackground, Is.EqualTo(slayer));
        float savedSfx = AudioPreferences.Sfx;
        try
        {
            PlayerPrefs.SetFloat("SfxVolume", 0);
            typing.ProcessInput("READY");
            yield return Until(() => Field<bool>(ritual, "chargePainted"));
            Assert.That(Array.Exists(SfxPlayer.I.GetComponentsInChildren<AudioSource>(), s => s.clip && s.clip == Resources.Load<SfxEvent>("SummonSeal").PickClip()), Is.False);
            ritual.Cancel();
            Assert.That(arena.EnvironmentPresentationGain, Is.EqualTo(.93f));
            Assert.That(arena.SlayerBackground, Is.EqualTo(slayer));
            Assert.That(Shader.GetGlobalFloat("_DungeonSlayerWeight"), Is.EqualTo(shaderWeight));
        }
        finally { PlayerPrefs.SetFloat("SfxVolume", savedSfx); arena.SetSlayerBackground(Color.clear); }
    }
}
