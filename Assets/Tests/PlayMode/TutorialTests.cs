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
        overlay.ShowStep(1);
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
    public IEnumerator MissingAtlasShowsErrorWithoutCompletingAndBackStillWorks()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var lessons = (TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay);
        lessons[0].atlasPaths[0] = "Tutorial/missing-atlas";
        Object.FindObjectOfType<TutorialDirector>().Continue();
        var panel = overlay.transform.Find("Reference960x540/ParryModal/Panel");
        var caption = panel.Find("Guidance/Caption").GetComponent<TMPro.TMP_Text>();
        float deadline = Time.realtimeSinceStartup + 15;
        while (!caption.text.Contains("Unable to load") && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(caption.text, Does.Contain("Unable to load"));
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        var pause = Object.FindObjectOfType<GamePauseController>();
        pause.TogglePause(true); yield return null;
        Assert.That(GamePauseController.IsPaused, Is.True);
        pause.Resume();
        while (GamePauseController.IsPaused && Time.realtimeSinceStartup < deadline) yield return null;
        panel.Find("BackButton").GetComponent<Button>().onClick.Invoke();
        Assert.That(panel.parent.gameObject.activeSelf, Is.False);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        yield return null;
        Object.FindObjectOfType<TutorialDirector>().Continue();
        deadline = Time.realtimeSinceStartup + 30;
        while (!caption.text.Contains("Unable to load") && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(caption.text, Does.Contain("Unable to load"));
        pause.TogglePause(true);
        pause.ReturnToMenu();
        while ((!SceneManager.GetSceneByName("MainMenu").isLoaded || DungeonSession.I.Transitioning) &&
            Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(SceneManager.GetSceneByName("MainMenu").isLoaded, Is.True, "Menu remains usable after a resource failure");
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LoadingPauseRapidNavigationAndDisableNeverPublishStaleFrames()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var clock = typeof(TutorialOverlay).GetField("clipTime", flags);
        var cache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache", flags).GetValue(overlay);
        var image = overlay.transform.Find("Reference960x540/ParryModal/Panel/Clip").GetComponent<Image>();
        var pause = Object.FindObjectOfType<GamePauseController>();
        overlay.ShowStep(1);
        Assert.That(image.sprite, Is.Null);
        Assert.That(image.enabled, Is.False);
        pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(image.sprite, Is.Null, "completion while paused must not publish a frame");
        Assert.That((double)clock.GetValue(overlay), Is.Zero);
        overlay.ShowStep(2); overlay.ShowStep(3); overlay.ShowStep(4);
        Assert.That(cache.SlotCount, Is.LessThanOrEqualTo(2));
        pause.Resume();
        float deadline = Time.realtimeSinceStartup + 15;
        while (!image.sprite && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(image.sprite.name, Does.StartWith("slayer_"));
        // Seek beyond the prefetched page to guarantee an outstanding disk request.
        var lesson = ((TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay))[3];
        int distant = System.Array.FindIndex(lesson.frameAtlas, atlas => atlas == 4);
        double blockedTime = lesson.durations.Take(distant).Sum(value => (double)value);
        var held = image.sprite;
        int dirty = 0;
        UnityEngine.Events.UnityAction onDirty = () => dirty++;
        image.RegisterDirtyVerticesCallback(onDirty);
        var caption = overlay.transform.Find("Reference960x540/ParryModal/Panel/Guidance/Caption").GetComponent<TMPro.TMP_Text>();
        string heldCaption = caption.text;
        clock.SetValue(overlay, blockedTime);
        var advance = typeof(TutorialOverlay).GetMethod("AdvanceClip", flags);
        advance.Invoke(overlay, new object[] { .01f });
        advance.Invoke(overlay, new object[] { 10f });
        image.UnregisterDirtyVerticesCallback(onDirty);
        Assert.That(dirty, Is.Zero, "waiting ticks preserve Image without dirtying it");
        Assert.That((double)clock.GetValue(overlay), Is.EqualTo(blockedTime), "waiting never accumulates wall time");
        Assert.That(image.sprite, Is.SameAs(held));
        Assert.That(caption.text, Is.EqualTo(heldCaption));
        Assert.That(cache.SlotCount, Is.LessThanOrEqualTo(2));
        pause.TogglePause(true);
        var loading = typeof(TutorialAtlasCache).GetField("loading", flags);
        while (loading.GetValue(cache) != null && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(loading.GetValue(cache), Is.Null);
        // Simulate the first playback tick after a long wait; the pending candidate wins.
        advance.Invoke(overlay, new object[] { 10f });
        Assert.That((double)clock.GetValue(overlay), Is.EqualTo(blockedTime + (double).01f));
        Assert.That(image.sprite.name, Is.EqualTo("slayer_" + distant.ToString("D3")));
        overlay.ShowStep(0);
        while (cache.SlotCount > 0 && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(cache.SlotCount, Is.Zero);
        Assert.That(image.sprite, Is.Null);
        overlay.ShowStep(5);
        overlay.gameObject.SetActive(false);
        while (cache.SlotCount > 0 && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(cache.SlotCount, Is.Zero);
        Assert.That(image.sprite, Is.Null, "late completion after disable is cleanup only");
        overlay.gameObject.SetActive(true);
        overlay.ShowStep(2);
        Object.Destroy(overlay.gameObject);
        yield return null;
        while (cache.SlotCount > 0 && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(cache.SlotCount, Is.Zero, "completion owns cleanup after destruction");
    }

    [UnityTest, Category("Quick")]
    public IEnumerator EveryLessonUsesCapturedDurationsHoldsPauseAndResetsOnBack()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var director = Object.FindObjectOfType<TutorialDirector>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var clock = typeof(TutorialOverlay).GetField("clipTime", flags);
        var render = typeof(TutorialOverlay).GetMethod("RenderClip", flags);
        var modal = overlay.transform.Find("Reference960x540/ParryModal");
        Assert.That(modal.gameObject.activeSelf, Is.False);
        director.Continue(); director.Continue();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        Assert.That(modal.gameObject.activeSelf, Is.True, "same-frame confirmation cannot skip the second step");
        var panel = (RectTransform)modal.Find("Panel");
        Assert.That(panel.sizeDelta, Is.EqualTo(new Vector2(928, 516)));
        var image = panel.Find("Clip").GetComponent<Image>();
        var rule = panel.Find("Rule").GetComponent<TMPro.TMP_Text>();
        var caption = panel.Find("Guidance/Caption").GetComponent<TMPro.TMP_Text>();
        var focus = (RectTransform)panel.Find("Clip/Focus");
        var label = (RectTransform)panel.Find("Guidance");
        Assert.That(image.GetComponentsInChildren<Graphic>(true).All(g => !g.raycastTarget), Is.True);
        var lessons = (TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay);
        using var capture = TutorialFrameCapture.Open(overlay);
        var atlasCache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache", flags).GetValue(overlay);
        Assert.That(lessons.Length, Is.EqualTo(5));
        int frameChecks = 0, atlasBoundaries = 0, loopJoins = 0;
        for (int lesson = 0; lesson < lessons.Length; lesson++)
        {
            if (lesson > 0) { yield return null; director.Continue(); }
            var frames = lessons[lesson].frameAtlas;
            string SpriteName(int index) => lessons[lesson].atlasPaths[0].Substring("Tutorial/".Length).Replace("-0", "") + "_" + index.ToString("D3");
            bool TryRenderAt(double value)
            {
                clock.SetValue(overlay, value);
                return (bool)render.Invoke(overlay, null);
            }
            IEnumerator RenderAt(double value)
            {
                float deadline = Time.realtimeSinceStartup + 15;
                do
                {
                    if (TryRenderAt(value)) yield break;
                    yield return null;
                } while (Time.realtimeSinceStartup < deadline);
                Assert.Fail("Tutorial load timed out");
            }
            var durations = lessons[lesson].durations;
            Assert.That(frames.Length, Is.GreaterThan(1));
            Assert.That(durations.Length, Is.EqualTo(frames.Length));
            Assert.That(lessons[lesson].captions.Length, Is.EqualTo(frames.Length));
            Assert.That(lessons[lesson].guidance.Length, Is.EqualTo(frames.Length));
            Assert.That(rule.text, Is.EqualTo(lessons[lesson].rule));
            Assert.That(label.parent, Is.SameAs(panel));
            Assert.That(label.anchoredPosition, Is.EqualTo(new Vector2(144, -414)));
            Assert.That(!label.GetComponent<Image>() || !label.GetComponent<Image>().enabled, Is.True);
            Assert.That(image.rectTransform.sizeDelta, Is.EqualTo(new Vector2(640, 360)));

            var pause = Object.FindObjectOfType<GamePauseController>();
            pause.TogglePause(true); // Freeze automatic playback while exhaustive lookups await disk loads.
            Assert.That(GamePauseController.IsPaused, Is.True);
            double time = 0;
            for (int i = 0; i < frames.Length; i++)
            {
                if (!TryRenderAt(time + durations[i] * .5)) yield return RenderAt(time + durations[i] * .5);
                Assert.That(atlasCache.SlotCount, Is.LessThanOrEqualTo(2), "loaded plus pending atlas slots");
                Assert.That(image.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
                Assert.That(image.sprite.rect.size, Is.EqualTo(new Vector2(320, 180)));
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(i)), "captured frame/duration " + i);
                Assert.That(caption.text, Is.EqualTo(lessons[lesson].captions[i]));
                capture?.Save(lesson, i, image.rectTransform);
                var guide = lessons[lesson].guidance[i];
                Assert.That(guide.focus.width, Is.GreaterThan(0));
                Assert.That(guide.focus.height, Is.GreaterThan(0));
                Assert.That(focus.sizeDelta, Is.EqualTo(guide.focus.size));
                Assert.That(focus.anchoredPosition, Is.EqualTo(new Vector2(guide.focus.x, -guide.focus.y)));
                Assert.That(label.anchoredPosition, Is.EqualTo(new Vector2(144, -414)));
                Assert.That(guide.focus.xMin, Is.GreaterThanOrEqualTo(0));
                Assert.That(guide.focus.yMin, Is.GreaterThanOrEqualTo(0));
                Assert.That(guide.focus.xMax, Is.LessThanOrEqualTo(640));
                Assert.That(guide.focus.yMax, Is.LessThanOrEqualTo(360));
                // Both sides of every boundary must select the same sprite and guidance index.
                if (!TryRenderAt(time + .000001)) yield return RenderAt(time + .000001);
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(i)));
                Assert.That(caption.text, Is.EqualTo(lessons[lesson].captions[i]));
                Assert.That(durations[i], Is.GreaterThan(0));
                time += durations[i];
                if (!TryRenderAt(time - .000001)) yield return RenderAt(time - .000001);
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(i)), "before boundary " + i);
                if (!TryRenderAt(time)) yield return RenderAt(time);
                int next = (i + 1) % frames.Length;
                frameChecks++;
                if (next == 0) loopJoins++;
                else if (frames[i] != frames[next]) atlasBoundaries++;
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(next)), $"exact boundary {i}, requested={time:R}, clock={clock.GetValue(overlay)}, paused={GamePauseController.IsPaused}");
                Assert.That(caption.text, Is.EqualTo(lessons[lesson].captions[next]));
            }
            if (!TryRenderAt(time + .001)) yield return RenderAt(time + .001);
            Assert.That(image.sprite.name, Is.EqualTo(SpriteName(0)), "loop boundary");
            Assert.That(caption.text, Is.EqualTo(lessons[lesson].captions[0]));
            if (!TryRenderAt(1.5)) yield return RenderAt(1.5);
            var held = image.sprite;
            string heldCaption = caption.text;
            var heldFocus = focus.anchoredPosition;
            pause.TogglePause(true);
            yield return new WaitForSecondsRealtime(.15f);
            Assert.That((double)clock.GetValue(overlay), Is.EqualTo(1.5));
            Assert.That(image.sprite, Is.SameAs(held));
            Assert.That(caption.text, Is.EqualTo(heldCaption));
            Assert.That(focus.anchoredPosition, Is.EqualTo(heldFocus));
            pause.Resume();
            float resumeDeadline = Time.realtimeSinceStartup + 3;
            while ((double)clock.GetValue(overlay) <= 1.5 && Time.realtimeSinceStartup < resumeDeadline)
                yield return null;
            Assert.That((double)clock.GetValue(overlay), Is.GreaterThan(1.5));
            Assert.That(Time.timeScale, Is.Zero);
            Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
            yield return null;
            panel.Find("BackButton").GetComponent<Button>().onClick.Invoke();
            Assert.That(modal.gameObject.activeSelf, Is.EqualTo(lesson > 0));
            Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
            yield return null;
            director.Continue();
            Assert.That((double)clock.GetValue(overlay), Is.Zero);
            pause.TogglePause(true);
            if (!TryRenderAt(0)) yield return RenderAt(0);
            Assert.That(image.sprite.name, Is.EqualTo(SpriteName(0)));
            pause.Resume();
            float unpauseDeadline = Time.realtimeSinceStartup + 3;
            while (GamePauseController.IsPaused && Time.realtimeSinceStartup < unpauseDeadline) yield return null;
            Assert.That(GamePauseController.IsPaused, Is.False);
        }
        Assert.That(frameChecks, Is.EqualTo(1995));
        Assert.That(atlasBoundaries, Is.EqualTo(28));
        Assert.That(loopJoins, Is.EqualTo(5));
        Debug.Log("Tutorial lookup PASS: 1995 frames, 28 atlas boundaries, 5 loops");
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
        director.Continue(); director.Continue();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        float battleRequestedAt = 0;
        for (int step = 2; step <= 6; step++)
        {
            yield return null;
            Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
            if (step == 6) battleRequestedAt = Time.realtimeSinceStartup;
            director.Continue(); director.Continue();
        }
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Completed));
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
