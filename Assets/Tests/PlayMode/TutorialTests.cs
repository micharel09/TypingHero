using System.Collections;
using System.Linq;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public sealed class TutorialTests
{
    int progress, helpSeen;
    bool hadProgress, hadHelp;
    [SetUp]
    public void Setup()
    {
        hadProgress = PlayerPrefs.HasKey("TutorialProgress_v1");
        hadHelp = PlayerPrefs.HasKey("HowToPlaySeen");
        progress = PlayerPrefs.GetInt("TutorialProgress_v1", 0);
        helpSeen = PlayerPrefs.GetInt("HowToPlaySeen", 0);
        PlayerPrefs.DeleteKey("TutorialProgress_v1");
        TutorialRun.End();
    }

    [TearDown]
    public void Cleanup()
    {
        TutorialRun.End();
        if (hadProgress) PlayerPrefs.SetInt("TutorialProgress_v1", progress);
        else PlayerPrefs.DeleteKey("TutorialProgress_v1");
        if (hadHelp) PlayerPrefs.SetInt("HowToPlaySeen", helpSeen);
        else PlayerPrefs.DeleteKey("HowToPlaySeen");
        PlayerPrefs.Save();
        GamePauseController.SetTutorialReading(false);
    }

    [Test, Category("Quick")]
    public void QuickHelpAndAbandonDoNotCompleteGuideAndLegacyViewedStatesRemainViewed()
    {
        PlayerPrefs.SetInt("HowToPlaySeen", 1);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        TutorialRun.Begin(); TutorialRun.End();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        foreach (int saved in new[] { 1, 2 })
        {
            PlayerPrefs.SetInt("TutorialProgress_v1", saved);
            TutorialRun.Begin(); TutorialRun.End();
            Assert.That((int)TutorialRun.Status, Is.EqualTo(saved));
        }
        TutorialRun.Begin(); TutorialRun.Skip(); TutorialRun.End();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Completed), "skip replay preserves completed");
        TutorialRun.Begin(); TutorialRun.Complete(); TutorialRun.End();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Completed));
    }

    [UnityTearDown]
    public IEnumerator UnloadOwnedScene()
    {
        foreach (string name in new[] { "MainMenu", DungeonSession.GameScene, LightingTestAssets.WorldScene })
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        }
        if (DungeonSession.Current) Object.Destroy(DungeonSession.Current.gameObject);
        EndlessRun.Clear(); GamePauseController.ResetSessionState();
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator StablePlaybackAndCacheAllocateNothingAndDoNotDirtyImage()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var render = (System.Func<bool>)System.Delegate.CreateDelegate(typeof(System.Func<bool>), overlay,
            typeof(TutorialOverlay).GetMethod("RenderClip", flags));
        var advance = (System.Action<float>)System.Delegate.CreateDelegate(typeof(System.Action<float>), overlay,
            typeof(TutorialOverlay).GetMethod("AdvanceClip", flags));
        var cache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache", flags).GetValue(overlay);
        var image = overlay.transform.Find("Reference960x540/ParryModal/Panel/Clip").GetComponent<Image>();
        Object.FindObjectOfType<GamePauseController>().TogglePause(true);
        overlay.ShowStep(0);
        float deadline = Time.realtimeSinceStartup + 15;
        while (!render() && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(image.sprite, Is.Not.Null);
        // Wait for prefetch too; resource request allocations are outside the measured loop.
        var loading = typeof(TutorialAtlasCache).GetField("loading", flags);
        while (loading.GetValue(cache) != null && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(loading.GetValue(cache), Is.Null);
        int dirty = 0;
        UnityEngine.Events.UnityAction onDirty = () => dirty++;
        image.RegisterDirtyVerticesCallback(onDirty);
        image.RegisterDirtyMaterialCallback(onDirty);
        image.RegisterDirtyLayoutCallback(onDirty);
        try
        {
            for (int i = 0; i < 32; i++) { render(); advance(0); cache.Get("Tutorial/typing-0", "Tutorial/typing-1", "Tutorial/typing-0"); }
            long before = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++) render();
            long renderBytes = System.GC.GetAllocatedBytesForCurrentThread() - before;
            before = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++) advance(0);
            long tickBytes = System.GC.GetAllocatedBytesForCurrentThread() - before;
            before = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++) cache.Get("Tutorial/typing-0", "Tutorial/typing-1", "Tutorial/typing-0");
            long cacheBytes = System.GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.That(renderBytes, Is.Zero, "same-frame render bytes");
            Assert.That(tickBytes, Is.Zero, "same-frame tick bytes");
            Assert.That(cacheBytes, Is.Zero, "stable cache bytes");
            Assert.That(dirty, Is.Zero, "same frame never dirties Image");
            var held = image.sprite;
            int heldId = held.GetInstanceID();
            var destination = cache.Get("Tutorial/typing-1", null, "Tutorial/typing-0");
            Assert.That(destination, Is.Not.Null);
            Assert.That(held && held.texture, Is.True, "Get protects displayed atlas even when destination is ready");
            Assert.That(image.sprite, Is.SameAs(held));
            Assert.That(dirty, Is.Zero);
            image.sprite = destination.sprites[0];
            Assert.That(dirty, Is.GreaterThan(0), "new sprite updates Image");
            cache.CommitDisplay("Tutorial/typing-1");
            Assert.That(image.sprite.texture, Is.SameAs(destination.texture));
            Assert.That(Resources.FindObjectsOfTypeAll<Sprite>().Any(sprite => sprite.GetInstanceID() == heldId),
                Is.False, "old sprite is no longer resident after handoff");
        }
        finally
        {
            image.UnregisterDirtyVerticesCallback(onDirty);
            image.UnregisterDirtyMaterialCallback(onDirty);
            image.UnregisterDirtyLayoutCallback(onDirty);
            overlay.ReleaseClip();
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator AtlasCacheReloadsAndRetiresObsoleteRequestsWithinTwoSlots()
    {
        var cache = new TutorialAtlasCache();
        IEnumerator Drain()
        {
            float deadline = Time.realtimeSinceStartup + 15;
            while (cache.SlotCount != 0 && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(cache.SlotCount, Is.Zero);
        }
        IEnumerator Load(string path, System.Action<TutorialAtlasPage> receive)
        {
            float deadline = Time.realtimeSinceStartup + 15;
            TutorialAtlasPage page;
            while (!(page = cache.Get(path, path)) && Time.realtimeSinceStartup < deadline)
            {
                Assert.That(cache.SlotCount, Is.LessThanOrEqualTo(2));
                yield return null;
            }
            Assert.That(page, Is.Not.Null);
            receive(page);
        }
        try
        {
            TutorialAtlasPage first = null;
            yield return Load("Tutorial/typing-0", page => first = page);
            int pageId = first.GetInstanceID(), spriteId = first.sprites[0].GetInstanceID(), textureId = first.texture.GetInstanceID();
            cache.Clear(); yield return Drain();
            Assert.That(Resources.FindObjectsOfTypeAll<TutorialAtlasPage>().Any(page => page.GetInstanceID() == pageId), Is.False, "page no longer resident");
            Assert.That(Resources.FindObjectsOfTypeAll<Sprite>().Any(sprite => sprite.GetInstanceID() == spriteId), Is.False, "sprite no longer resident");
            Assert.That(Resources.FindObjectsOfTypeAll<Texture2D>().Any(texture => texture.GetInstanceID() == textureId), Is.False, "texture no longer resident");
            yield return Load("Tutorial/typing-0", page => {
                Assert.That(ReferenceEquals(first, page), Is.False, "reload obtains a fresh descriptor");
                Assert.That(page.sprites[0].texture, Is.SameAs(page.texture));
            });
            cache.Clear();
            cache.Get("Tutorial/slayer-0", "Tutorial/slayer-1");
            cache.Clear();
            cache.Get("Tutorial/score-0", "Tutorial/score-1");
            cache.Clear();
            yield return Load("Tutorial/mistakes-0", page => Assert.That(page.sprites[0].name, Is.EqualTo("mistakes_000")));
            cache.Clear(); yield return Drain();
            cache.Get("Tutorial/parry-health-0", "Tutorial/parry-health-1");
            cache.Clear(); yield return Drain();
            Assert.That(cache.Error, Is.Null);
            // A scene replacement owns a different cache while the old request retires.
            cache.Get("Tutorial/typing-0", "Tutorial/typing-1");
            cache.Clear();
            var replacement = new TutorialAtlasCache();
            try
            {
                float replacementDeadline = Time.realtimeSinceStartup + 15;
                TutorialAtlasPage reloaded;
                while (!(reloaded = replacement.Get("Tutorial/typing-0", "Tutorial/typing-0")) &&
                    Time.realtimeSinceStartup < replacementDeadline) yield return null;
                Assert.That(reloaded, Is.Not.Null);
                Assert.That(reloaded.sprites[0].texture, Is.SameAs(reloaded.texture));
                Assert.That(replacement.Error, Is.Null);
            }
            finally { replacement.Clear(); }
            yield return Drain();
            cache.Get("Tutorial/missing-atlas", null);
            float deadline = Time.realtimeSinceStartup + 15;
            while (cache.Error == null && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(cache.Error, Does.Contain("Unable to load"));
        }
        finally { cache.Clear(); }
        yield return Drain();
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CompactCardsAutoplayLoopAndFinishWithoutPractice()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var director = Object.FindObjectOfType<TutorialDirector>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var step = typeof(TutorialOverlay).GetField("step", flags);
        var clock = typeof(TutorialOverlay).GetField("clipTime", flags);
        var duration = typeof(TutorialOverlay).GetField("totalDuration", flags);
        var advance = typeof(TutorialOverlay).GetMethod("AdvanceClip", flags);
        var render = typeof(TutorialOverlay).GetMethod("RenderClip", flags);
        var panel = (RectTransform)overlay.transform.Find("Reference960x540/ParryModal/Panel");
        Assert.That(overlay.StepCount, Is.EqualTo(5));
        Assert.That(panel.anchoredPosition, Is.EqualTo(new Vector2(200, -100)));
        Assert.That(panel.sizeDelta, Is.EqualTo(new Vector2(560, 340)));
        var clip = (RectTransform)panel.Find("Clip");
        Assert.That(clip.anchoredPosition, Is.EqualTo(new Vector2(80, -50)));
        Assert.That(clip.sizeDelta, Is.EqualTo(new Vector2(400, 225)));
        Assert.That(Object.FindObjectOfType<TutorialPractice>(), Is.Null);
        for (int index = 0; index < 5; index++)
        {
            Assert.That((int)step.GetValue(overlay), Is.EqualTo(index));
            Assert.That(GamePauseController.InputBlocked, Is.True);
            Assert.That(Time.timeScale, Is.Zero);
            Assert.That(panel.Find("BackButton").gameObject.activeInHierarchy, Is.False);
            Assert.That(overlay.transform.Find("Reference960x540/WatchDemo").gameObject.activeInHierarchy, Is.False);
            float deadline = Time.realtimeSinceStartup + 20;
            while (!(bool)render.Invoke(overlay, null) && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(panel.Find("Clip").GetComponent<Image>().sprite, Is.Not.Null);
            double end = (double)duration.GetValue(overlay);
            clock.SetValue(overlay, end - .01);
            advance.Invoke(overlay, new object[] { .02f });
            Assert.That((double)clock.GetValue(overlay), Is.LessThan(.02), "clip wraps without a replay click");
            yield return null;
            director.Continue();
            if (index < 4)
            {
                Assert.That((double)clock.GetValue(overlay), Is.Zero, "new card restarts clip");
                Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
            }
        }
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Completed));
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(EndlessDuel.I.Run.Score, Is.Zero);
        Assert.That(CombatSession.I.Letters, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ReadingRestartMenuAndRepeatedConfirmationUseFreshDungeonLifecycle()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var typing = Object.FindObjectOfType<TypingManager>();
        var director = Object.FindObjectOfType<TutorialDirector>();
        var pause = Object.FindObjectOfType<GamePauseController>();
        var world = DungeonSession.I.World;
        typing.ProcessInput("BLADE");
        Assert.That(CombatSession.I.Letters, Is.Zero);
        Assert.That(CombatSession.I.Seconds, Is.Zero);
        Assert.That(Time.timeScale, Is.Zero);
        Assert.That(AudioListener.pause, Is.True);
        Assert.That(EndlessDuel.I, Is.Null, "guide has no live encounter");
        pause.TogglePause(true);
        director.Continue();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen), "paused confirmation rejected");
        pause.Resume();
        float resumeDeadline = Time.realtimeSinceStartup + 10;
        while (GamePauseController.IsPaused && Time.realtimeSinceStartup < resumeDeadline) yield return null;
        Assert.That(GamePauseController.IsPaused, Is.False);
        Assert.That(GamePauseController.InputBlocked, Is.True);
        pause.Restart();
        float restartDeadline = Time.realtimeSinceStartup + 30;
        while ((pause || !Object.FindObjectOfType<GamePauseController>() || DungeonSession.I.Transitioning) &&
            Time.realtimeSinceStartup < restartDeadline) yield return null;
        Assert.That(pause == null, Is.True, "restart tears down the old controller");
        Assert.That(Object.FindObjectOfType<GamePauseController>(), Is.Not.Null);
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Active, Is.True);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        Object.FindObjectOfType<GamePauseController>().ReturnToMenu();
        float menuDeadline = Time.realtimeSinceStartup + 30;
        while ((!SceneManager.GetSceneByName("MainMenu").isLoaded || DungeonSession.I.Transitioning) &&
            Time.realtimeSinceStartup < menuDeadline) yield return null;
        Assert.That(SceneManager.GetSceneByName("MainMenu").isLoaded, Is.True);
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        director = Object.FindObjectOfType<TutorialDirector>();
        float battleRequestedAt = Time.realtimeSinceStartup;
        yield return null;
        Object.FindObjectOfType<TutorialOverlay>().transform.Find("Reference960x540/SkipTutorial").GetComponent<Button>().onClick.Invoke();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Skipped));
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(DungeonSession.I.World, Is.SameAs(world));
        Assert.That(EndlessDuel.I.Run.Difficulty, Is.EqualTo(EndlessDifficulties.Selected));
        Assert.That(EndlessDuel.I.Run.Score, Is.Zero);
        Assert.That(EndlessDuel.I.Run.Slot, Is.Zero);
        Assert.That(CombatSession.I.Letters, Is.Zero);
        float firstSeconds = CombatSession.I.Seconds;
        Assert.That(firstSeconds, Is.InRange(0f, Time.realtimeSinceStartup - battleRequestedAt + .01f),
            "fresh encounter time cannot include tutorial reading time");
        yield return null;
        Assert.That(CombatSession.I.Seconds, Is.GreaterThan(firstSeconds), "fresh encounter clock advances");
        Assert.That(GamePauseController.EncounterReading, Is.False);
        var encounter = Object.FindObjectOfType<EndlessOverlay>();
        var panel = encounter.transform.Find("Reference960x540/TutorialPrompt");
        foreach (string name in new[] { "HealthGuide", "EnemyGuide", "TypingGuide", "ScoreGuide", "Shortcuts", "SpaceExample" })
            Assert.That(panel.Find(name).gameObject.activeInHierarchy, Is.False, "guide must not leak into the encounter clone");
        Assert.That(encounter.transform.Find("Reference960x540/ParryModal").gameObject.activeSelf, Is.False);
        Assert.That(encounter.transform.Find("Reference960x540/ParryModal/Panel/Clip").GetComponent<Image>().sprite, Is.Null);
        var clone = encounter.GetComponent<TutorialOverlay>();
        var cloneCache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(clone);
        Assert.That(cloneCache.SlotCount, Is.Zero, "encounter clone carries no tutorial cache");
        EndlessDuel.I.Continue();
        yield return null;
        Assert.That(panel.GetComponentsInChildren<Graphic>().Any(g => g.isActiveAndEnabled), Is.False,
            "encounter template must not cover live combat");
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PauseResumeCannotReleaseReadingLock()
    {
        var host = new GameObject("pause");
        var pause = host.AddComponent<GamePauseController>();
        GamePauseController.SetTutorialReading(true);
        pause.TogglePause(true); pause.Resume();
        yield return null;
        Assert.That(GamePauseController.IsPaused, Is.False);
        Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(Time.timeScale, Is.Zero);
        Assert.That(AudioListener.pause, Is.True);
        GamePauseController.SetTutorialReading(false);
        Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(Time.timeScale, Is.EqualTo(1));
        Object.Destroy(host); yield return null;
    }

    [Test, Category("Quick")]
    public void TutorialSubmissionStopsBeforeResolvingAnyFirebaseProvider()
    {
        var host = new GameObject("submit");
        try
        {
            TutorialRun.Begin();
            host.AddComponent<LeaderboardSubmitter>().SubmitFinalScore(999);
            LogAssert.NoUnexpectedReceived();
        }
        finally { Object.DestroyImmediate(host); }
    }
}
