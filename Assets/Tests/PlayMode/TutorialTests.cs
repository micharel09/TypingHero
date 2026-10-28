using System.Collections;
using System.Linq;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public sealed class TutorialTests
{
    [System.Serializable] sealed class CaptureFrame { public float duration; public int atlas; }
    [System.Serializable] sealed class CaptureManifest { public string[] atlases; public CaptureFrame[] frames; }
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
        var lessons = (TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay);
        int multiPage = System.Array.FindIndex(lessons, lesson => lesson.atlasPaths.Length > 1);
        Assert.That(multiPage, Is.GreaterThanOrEqualTo(0));
        overlay.ShowStep(multiPage);
        string firstPath = lessons[multiPage].atlasPaths[0], nextPath = lessons[multiPage].atlasPaths[1];
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
            for (int i = 0; i < 32; i++) { render(); advance(0); cache.Get(firstPath, nextPath, firstPath); }
            long before = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++) render();
            long renderBytes = System.GC.GetAllocatedBytesForCurrentThread() - before;
            before = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++) advance(0);
            long tickBytes = System.GC.GetAllocatedBytesForCurrentThread() - before;
            before = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++) cache.Get(firstPath, nextPath, firstPath);
            long cacheBytes = System.GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.That(renderBytes, Is.Zero, "same-frame render bytes");
            Assert.That(tickBytes, Is.Zero, "same-frame tick bytes");
            Assert.That(cacheBytes, Is.Zero, "stable cache bytes");
            Assert.That(dirty, Is.Zero, "same frame never dirties Image");
            var held = image.sprite;
            int heldId = held.GetInstanceID();
            var destination = cache.Get(nextPath, null, firstPath);
            Assert.That(destination, Is.Not.Null);
            Assert.That(held && held.texture, Is.True, "Get protects displayed atlas even when destination is ready");
            Assert.That(image.sprite, Is.SameAs(held));
            Assert.That(dirty, Is.Zero);
            image.sprite = destination.sprites[0];
            Assert.That(dirty, Is.GreaterThan(0), "new sprite updates Image");
            cache.CommitDisplay(nextPath);
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
        overlay.ShowStep(2); overlay.ShowStep(4); overlay.ShowStep(3);
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
        var caption = overlay.transform.Find("Reference960x540/ParryModal/Panel/Guidance/Caption").GetComponent<TMPro.TMP_Text>();
        string heldCaption = caption.text;
        clock.SetValue(overlay, blockedTime);
        var advance = typeof(TutorialOverlay).GetMethod("AdvanceClip", flags);
        advance.Invoke(overlay, new object[] { .01f });
        advance.Invoke(overlay, new object[] { 10f });
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
        overlay.ReleaseClip();
        while (cache.SlotCount > 0 && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(cache.SlotCount, Is.Zero);
        Assert.That(image.sprite, Is.Null);
        overlay.ShowStep(4);
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
    public IEnumerator EveryCapturedFrameAndBoundaryRetainsTimingAndLoopFadeHoldsPause()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var clock = typeof(TutorialOverlay).GetField("clipTime", flags);
        var phase = typeof(TutorialOverlay).GetField("loopPhase", flags);
        var render = (System.Func<bool>)System.Delegate.CreateDelegate(typeof(System.Func<bool>), overlay,
            typeof(TutorialOverlay).GetMethod("RenderClip", flags));
        var advance = (System.Action<float>)System.Delegate.CreateDelegate(typeof(System.Action<float>), overlay,
            typeof(TutorialOverlay).GetMethod("AdvanceClip", flags));
        var lessons = (TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay);
        var cache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache", flags).GetValue(overlay);
        var image = overlay.transform.Find("Reference960x540/ParryModal/Panel/Clip").GetComponent<Image>();
        var pause = Object.FindObjectOfType<GamePauseController>();
        pause.TogglePause(true);
        int checkedFrames = 0;
        for (int index = 0; index < lessons.Length; index++)
        {
            overlay.ShowStep(index);
            var lesson = lessons[index];
            string clipName = System.IO.Path.GetFileNameWithoutExtension(lesson.atlasPaths[0]).Replace("-0", "");
            var manifest = JsonUtility.FromJson<CaptureManifest>(System.IO.File.ReadAllText(
                "Assets/Art/UI/TutorialParry/" + clipName + ".json"));
            Assert.That(lesson.durations.Length, Is.EqualTo(manifest.frames.Length), "serialized frame count matches source manifest");
            Assert.That(lesson.atlasPaths.Length, Is.EqualTo(manifest.atlases.Length));
            double time = 0;
            for (int frame = 0; frame < lesson.durations.Length; frame++)
            {
                Assert.That(lesson.durations[frame], Is.EqualTo(manifest.frames[frame].duration).Within(.0000001f));
                Assert.That(lesson.frameAtlas[frame], Is.EqualTo(manifest.frames[frame].atlas));
                Assert.That(lesson.frameSprite[frame], Is.EqualTo(frame % 66));
                Assert.That(lesson.durations[frame], Is.GreaterThan(0));
                foreach (double at in new[] { time + .000001, time + lesson.durations[frame] * .5,
                    time + lesson.durations[frame] - .000001 })
                {
                    clock.SetValue(overlay, at);
                    float deadline = Time.realtimeSinceStartup + 15;
                    while (!render() && Time.realtimeSinceStartup < deadline) yield return null;
                    string prefix = lesson.atlasPaths[0].Substring("Tutorial/".Length).Replace("-0", "");
                    Assert.That(image.sprite.name, Is.EqualTo(prefix + "_" + frame.ToString("D3")));
                    Assert.That(image.sprite.rect.size, Is.EqualTo(new Vector2(320, 180)));
                    Assert.That(image.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
                    Assert.That(cache.SlotCount, Is.LessThanOrEqualTo(2));
                }
                time += lesson.durations[frame]; checkedFrames++;
            }
            // The last page must prefetch the first without retiring the displayed frame.
            string first = lesson.atlasPaths[0];
            var pages = (System.Collections.Generic.Dictionary<string, TutorialAtlasPage>)
                typeof(TutorialAtlasCache).GetField("pages", flags).GetValue(cache);
            float readyDeadline = Time.realtimeSinceStartup + 15;
            while (!pages.ContainsKey(first) && Time.realtimeSinceStartup < readyDeadline) yield return null;
            Assert.That(pages.ContainsKey(first), Is.True, "circular prefetch before loop boundary");
            var last = image.sprite;
            clock.SetValue(overlay, time);
            advance(0);
            Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("Playing"), "seek/load must still display the full final frame duration");
            advance(lesson.durations[lesson.durations.Length - 1]); advance(.05f);
            Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("FadeOut"));
            float heldAlpha = image.color.a;
            yield return new WaitForSecondsRealtime(.15f);
            Assert.That(image.color.a, Is.EqualTo(heldAlpha), "Pause holds fade alpha");
            Assert.That(image.sprite, Is.SameAs(last));
            Assert.That((double)clock.GetValue(overlay), Is.EqualTo(time));
            advance(.05f);
            Assert.That(image.color.a, Is.Zero);
            Assert.That((double)clock.GetValue(overlay), Is.Zero);
            Assert.That(image.sprite, Is.SameAs(pages[first].sprites[lesson.frameSprite[0]]));
            advance(.05f);
            overlay.ShowStep(index);
            Assert.That(image.color.a, Is.EqualTo(1), "navigation cancels fade-in");
            Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("Playing"));
        }
        Assert.That(checkedFrames, Is.EqualTo(lessons.Sum(lesson => lesson.durations.Length)));
        overlay.ReleaseClip();
        float exitDeadline = Time.realtimeSinceStartup + 15;
        while (cache.SlotCount > 0 && Time.realtimeSinceStartup < exitDeadline) yield return null;
        Assert.That(cache.SlotCount, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator OneAtlasLoopsWithoutDuplicateSlotsAndDelayedLoopStartsFreshFade()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var lessons = (TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay);
        var cache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache", flags).GetValue(overlay);
        var clock = typeof(TutorialOverlay).GetField("clipTime", flags);
        var phase = typeof(TutorialOverlay).GetField("loopPhase", flags);
        var render = (System.Func<bool>)System.Delegate.CreateDelegate(typeof(System.Func<bool>), overlay,
            typeof(TutorialOverlay).GetMethod("RenderClip", flags));
        var advance = (System.Action<float>)System.Delegate.CreateDelegate(typeof(System.Action<float>), overlay,
            typeof(TutorialOverlay).GetMethod("AdvanceClip", flags));
        var image = overlay.transform.Find("Reference960x540/ParryModal/Panel/Clip").GetComponent<Image>();
        Object.FindObjectOfType<GamePauseController>().TogglePause(true);
        IEnumerator Ready()
        {
            float deadline = Time.realtimeSinceStartup + 15;
            while (!render() && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(image.sprite, Is.Not.Null);
        }
        // Derive a one-page fixture from the real first page, retaining actual sprite identities/timing.
        var original = lessons[0];
        int count = System.Array.FindIndex(original.frameAtlas, page => page != 0);
        if (count < 0) count = original.frameAtlas.Length;
        lessons[0] = new TutorialOverlay.Lesson {
            title = original.title, rule = original.rule, atlasPaths = new[] { original.atlasPaths[0] },
            frameAtlas = original.frameAtlas.Take(count).ToArray(), frameSprite = original.frameSprite.Take(count).ToArray(),
            durations = original.durations.Take(count).ToArray(), captions = original.captions.Take(count).ToArray(),
            guidance = original.guidance.Take(count).ToArray()
        };
        overlay.ShowStep(0); yield return Ready();
        Assert.That(cache.SlotCount, Is.EqualTo(1));
        Assert.That(typeof(TutorialAtlasCache).GetField("next", flags).GetValue(cache), Is.Null);
        clock.SetValue(overlay, lessons[0].durations.Sum(value => (double)value));
        yield return Ready();
        advance(lessons[0].durations[count - 1]);
        advance(.1f); advance(.1f);
        Assert.That((double)clock.GetValue(overlay), Is.Zero);
        Assert.That(image.sprite.name, Is.EqualTo("typing_000"));
        Assert.That(cache.SlotCount, Is.EqualTo(1), "one atlas never requests a duplicate next page");
        clock.SetValue(overlay, lessons[0].durations.Sum(value => (double)value));
        yield return Ready();
        advance(lessons[0].durations[count - 1]);
        advance(.07f); advance(.07f);
        Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("FadeIn"));
        Assert.That(image.color.a, Is.EqualTo(.352f).Within(.0001f), "100 ms boundary retains the render-tick remainder");
        Assert.That((double)clock.GetValue(overlay), Is.Zero);
        advance(.06f);
        Assert.That(image.color.a, Is.EqualTo(1));
        Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("Playing"), "irregular ticks still complete the full fade at 200 ms");
        lessons[0] = original;
        int index = System.Array.FindIndex(lessons, lesson => lesson.atlasPaths.Length > 2);
        Assert.That(index, Is.GreaterThanOrEqualTo(0));
        overlay.ShowStep(index);
        var lesson = lessons[index];
        double end = lesson.durations.Sum(value => (double)value);
        clock.SetValue(overlay, end);
        yield return Ready();
        string lastPath = lesson.atlasPaths[lesson.atlasPaths.Length - 1];
        cache.Get(lastPath, null, lastPath); // Evict circular prefetch while retaining the actual displayed page.
        var loading = typeof(TutorialAtlasCache).GetField("loading", flags);
        float readyDeadline = Time.realtimeSinceStartup + 15;
        while (loading.GetValue(cache) != null && Time.realtimeSinceStartup < readyDeadline) yield return null;
        Assert.That(loading.GetValue(cache), Is.Null);
        var last = image.sprite;
        advance(lesson.durations[lesson.durations.Length - 1]);
        advance(10);
        Assert.That(image.sprite, Is.SameAs(last));
        Assert.That(last && last.texture, Is.True);
        Assert.That((double)clock.GetValue(overlay), Is.EqualTo(end));
        Assert.That(image.color.a, Is.EqualTo(1));
        Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("Playing"));
        while (loading.GetValue(cache) != null && Time.realtimeSinceStartup < readyDeadline) yield return null;
        advance(10); // The loading delta must not skip the fade-out.
        Assert.That(image.color.a, Is.EqualTo(1));
        Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("FadeOut"));
        advance(.05f);
        Assert.That(image.color.a, Is.EqualTo(.5f).Within(.0001f));
        Assert.That(image.sprite, Is.SameAs(last));
        advance(.05f);
        Assert.That(image.color.a, Is.Zero);
        Assert.That((double)clock.GetValue(overlay), Is.Zero);
        Assert.That(image.sprite.name, Does.EndWith("_000"));
        Assert.That(cache.SlotCount, Is.LessThanOrEqualTo(2));
        overlay.ReleaseClip();
        while (cache.SlotCount > 0 && Time.realtimeSinceStartup < readyDeadline) yield return null;
        Assert.That(cache.SlotCount, Is.Zero);
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
            var pause = Object.FindObjectOfType<GamePauseController>();
            pause.TogglePause(true);
            float deadline = Time.realtimeSinceStartup + 20;
            while (!(bool)render.Invoke(overlay, null) && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(panel.Find("Clip").GetComponent<Image>().sprite, Is.Not.Null);
            double end = (double)duration.GetValue(overlay);
            clock.SetValue(overlay, end - .01);
            advance.Invoke(overlay, new object[] { .02f });
            var image = clip.GetComponent<Image>();
            deadline = Time.realtimeSinceStartup + 20;
            var phase = typeof(TutorialOverlay).GetField("loopPhase", flags);
            while (phase.GetValue(overlay).ToString() == "Playing" && Time.realtimeSinceStartup < deadline)
            {
                advance.Invoke(overlay, new object[] { .02f });
                yield return null;
            }
            Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("FadeOut"));
            Assert.That((double)clock.GetValue(overlay), Is.EqualTo(end), "last frame duration is complete before fading");
            var last = image.sprite;
            float initialFadeOffset = (float)typeof(TutorialOverlay).GetField("loopElapsed", flags).GetValue(overlay);
            advance.Invoke(overlay, new object[] { .05f });
            Assert.That(image.color.a, Is.InRange(.01f, .99f));
            Assert.That(image.sprite, Is.SameAs(last));
            float alpha = image.color.a;
            advance.Invoke(overlay, new object[] { 0f });
            Assert.That(image.color.a, Is.EqualTo(alpha), "held clock holds fade");
            advance.Invoke(overlay, new object[] { .05f - initialFadeOffset });
            Assert.That(image.color.a, Is.EqualTo(0).Within(.000001f));
            Assert.That((double)clock.GetValue(overlay), Is.Zero);
            Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("FadeIn"));
            advance.Invoke(overlay, new object[] { .05f });
            Assert.That(image.color.a, Is.InRange(.01f, .99f));
            advance.Invoke(overlay, new object[] { .05f });
            Assert.That(image.color.a, Is.EqualTo(1));
            Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("Playing"));
            pause.Resume();
            float resumeDeadline = Time.realtimeSinceStartup + 10;
            while (GamePauseController.IsPaused && Time.realtimeSinceStartup < resumeDeadline) yield return null;
            Assert.That(GamePauseController.IsPaused, Is.False);
            yield return null;
            if (index == 4)
            {
                // Exercise Start through the real director while a fade-in owns the GIF alpha.
                phase.SetValue(overlay, System.Enum.Parse(phase.FieldType, "FadeIn"));
                typeof(TutorialOverlay).GetField("loopElapsed", flags).SetValue(overlay, .15f);
                var fading = image.color; fading.a = .5f; image.color = fading;
            }
            director.Continue();
            if (index == 4)
            {
                Assert.That(phase.GetValue(overlay).ToString(), Is.EqualTo("Playing"), "Start cancels fade");
                Assert.That(image.color.a, Is.EqualTo(1));
                Assert.That(image.sprite, Is.Null);
            }
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
        var exitingOverlay = Object.FindObjectOfType<TutorialOverlay>();
        var fadeFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var fadePhase = typeof(TutorialOverlay).GetField("loopPhase", fadeFlags);
        var exitingImage = exitingOverlay.transform.Find("Reference960x540/ParryModal/Panel/Clip").GetComponent<Image>();
        float fadeReadyDeadline = Time.realtimeSinceStartup + 15;
        while (!exitingImage.sprite && Time.realtimeSinceStartup < fadeReadyDeadline) yield return null;
        Assert.That(exitingImage.sprite, Is.Not.Null, "Skip fixture has a visible GIF before fade-out");
        fadePhase.SetValue(exitingOverlay, System.Enum.Parse(fadePhase.FieldType, "FadeOut"));
        typeof(TutorialOverlay).GetField("loopElapsed", fadeFlags).SetValue(exitingOverlay, .05f);
        var fadingColor = exitingImage.color; fadingColor.a = .5f; exitingImage.color = fadingColor;
        battleRequestedAt = Time.realtimeSinceStartup;
        exitingOverlay.transform.Find("Reference960x540/SkipTutorial").GetComponent<Button>().onClick.Invoke();
        Assert.That(fadePhase.GetValue(exitingOverlay).ToString(), Is.EqualTo("Playing"), "Skip cancels fade");
        Assert.That(exitingImage.color.a, Is.EqualTo(1));
        Assert.That(exitingImage.sprite, Is.Null);
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
