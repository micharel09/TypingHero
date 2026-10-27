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
        yield return EnterTypingLesson();
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
        yield return EnterTypingLesson();
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
    public IEnumerator EveryLessonUsesCapturedDurationsHoldsPauseAndStopsAtEnd()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var clock = typeof(TutorialOverlay).GetField("clipTime", flags);
        var render = typeof(TutorialOverlay).GetMethod("RenderClip", flags);
        var lessons = (TutorialOverlay.Lesson[])typeof(TutorialOverlay).GetField("lessons", flags).GetValue(overlay);
        var cache = (TutorialAtlasCache)typeof(TutorialOverlay).GetField("atlasCache", flags).GetValue(overlay);
        var panel = overlay.transform.Find("Reference960x540/ParryModal/Panel");
        var image = panel.Find("Clip").GetComponent<Image>();
        var caption = panel.Find("Guidance/Caption").GetComponent<TMPro.TMP_Text>();
        var focus = (RectTransform)panel.Find("Clip/Focus");
        var pause = Object.FindObjectOfType<GamePauseController>();
        int checks = 0, boundaries = 0, ends = 0;
        for (int lesson = 0; lesson < lessons.Length; lesson++)
        {
            overlay.ShowStep(lesson + 1);
            var data = lessons[lesson];
            string SpriteName(int i) => data.atlasPaths[0].Substring("Tutorial/".Length).Replace("-0", "") + "_" + i.ToString("D3");
            bool TryRenderAt(double time)
            {
                clock.SetValue(overlay, time);
                return (bool)render.Invoke(overlay, null);
            }
            IEnumerator RenderAt(double time)
            {
                float deadline = Time.realtimeSinceStartup + 15;
                while (!TryRenderAt(time) && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(image.sprite, Is.Not.Null);
            }
            yield return RenderAt(0);
            yield return new WaitForSecondsRealtime(.1f);
            Assert.That((double)clock.GetValue(overlay), Is.Zero, "click-to-play never advances on entry");
            pause.TogglePause(true);
            double time = 0;
            for (int i = 0; i < data.frameAtlas.Length; i++)
            {
                if (!TryRenderAt(time + data.durations[i] * .5)) yield return RenderAt(time + data.durations[i] * .5);
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(i)));
                Assert.That(caption.text, Is.EqualTo(data.captions[i]));
                Assert.That(focus.sizeDelta, Is.EqualTo(data.guidance[i].focus.size));
                Assert.That(image.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
                Assert.That(image.sprite.rect.size, Is.EqualTo(new Vector2(320, 180)));
                Assert.That(cache.SlotCount, Is.LessThanOrEqualTo(2));
                if (!TryRenderAt(time + .000001)) yield return RenderAt(time + .000001);
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(i)));
                time += data.durations[i];
                if (!TryRenderAt(time - .000001)) yield return RenderAt(time - .000001);
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(i)));
                if (!TryRenderAt(time)) yield return RenderAt(time);
                int next = Mathf.Min(i + 1, data.frameAtlas.Length - 1);
                Assert.That(image.sprite.name, Is.EqualTo(SpriteName(next)), "boundary " + i);
                if (i == next) ends++;
                else if (data.frameAtlas[i] != data.frameAtlas[next]) boundaries++;
                checks++;
            }
            yield return RenderAt(time + 1);
            Assert.That(image.sprite.name, Is.EqualTo(SpriteName(data.frameAtlas.Length - 1)), "holds terminal frame");
            pause.Resume();
            float resumeDeadline = Time.realtimeSinceStartup + 10;
            while (GamePauseController.IsPaused && Time.realtimeSinceStartup < resumeDeadline) yield return null;
            Assert.That(GamePauseController.IsPaused, Is.False);
            overlay.ShowStep(lesson + 1);
            overlay.PlayDemo();
            yield return RenderAt(0);
            yield return new WaitForSecondsRealtime(.1f);
            Assert.That((double)clock.GetValue(overlay), Is.GreaterThan(0), "explicit replay advances");
            pause.TogglePause(true);
            double held = (double)clock.GetValue(overlay);
            yield return new WaitForSecondsRealtime(.1f);
            Assert.That((double)clock.GetValue(overlay), Is.EqualTo(held));
            pause.Resume();
            resumeDeadline = Time.realtimeSinceStartup + 10;
            while (GamePauseController.IsPaused && Time.realtimeSinceStartup < resumeDeadline) yield return null;
            Assert.That(GamePauseController.IsPaused, Is.False);
        }
        Assert.That(checks, Is.EqualTo(1995));
        Assert.That(boundaries, Is.EqualTo(28));
        Assert.That(ends, Is.EqualTo(5));
    }

    static IEnumerator EnterTypingLesson()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return null;
            Object.FindObjectOfType<TutorialDirector>().Continue();
        }
        yield return null;
    }

    static IEnumerator CompleteTyping()
    {
        var director = Object.FindObjectOfType<TutorialDirector>();
        director.Continue(); yield return null;
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.Typing));
        var typing = Object.FindObjectOfType<TypingManager>();
        typing.ProcessInput("x");
        foreach (char letter in "hit")
        { typing.ProcessInput(letter.ToString()); yield return null; }
        float deadline = Time.realtimeSinceStartup + 8;
        while (TutorialRun.Practicing != TutorialRun.Practice.None && Time.realtimeSinceStartup < deadline) yield return null;
        var actor = Object.FindObjectOfType<PlayerController>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var exercise = Object.FindObjectOfType<TutorialPractice>();
        string diagnostic = " state=" + actor.animator.GetCurrentAnimatorStateInfo(0).fullPathHash +
            " idle=" + Animator.StringToHash("Base Layer.player_idle") + " speed=" + actor.animator.speed +
            " active=" + actor.animator.isActiveAndEnabled + " controller=" + actor.isActiveAndEnabled +
            " scale=" + Time.timeScale + " transition=" + actor.animator.IsInTransition(0) +
            " hp=" + Object.FindObjectOfType<SkeletonController>().Current +
            " hitFrame=" + actor.attackEvents.hitbox.LastHitFrame +
            " player=" + actor.transform.position + " enemy=" + Object.FindObjectOfType<SkeletonController>().transform.position +
            " gate=" + actor.gate.CanAttack +
            " mute=" + TypingManager.MuteWordCorrect + " blocked=" + GamePauseController.InputBlocked +
            " word=" + typeof(TutorialPractice).GetField("wordAccepted", flags).GetValue(exercise) +
            " config=" + actor.attack.statePath +
            " listeners=" + string.Join(",", ((System.Action)typeof(TypingManager).GetField("OnWordCorrect", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null)).GetInvocationList().Select(d => d.Target.GetType().Name + "." + d.Method.Name));
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.None), "real hit must finish" + diagnostic);
        Assert.That(Object.FindObjectOfType<SkeletonController>().Current, Is.EqualTo(170), "exactly one real 10-damage attack");
        Assert.That(CombatSession.I.Letters, Is.Zero);
        Assert.That(ScoreSystem.I.Score, Is.Zero);
        yield return null;
    }

    static IEnumerator CompleteParry()
    {
        Object.FindObjectOfType<TutorialDirector>().Continue();
        var enemy = Object.FindObjectOfType<SkeletonController>();
        var parry = Object.FindObjectOfType<ParrySystem>();
        bool pressed = false;
        int strikeCount = 0, successCount = 0;
        float pressedContact = -1;
        System.Action<IParryTarget> strike = _ => strikeCount++;
        System.Action<ParrySystem.ParryContext> success = _ => successCount++;
        parry.enemy.OnStrike += strike;
        parry.OnParrySuccess += success;
        float deadline = Time.realtimeSinceStartup + 10;
        while (Time.realtimeSinceStartup < deadline)
        {
            if (!pressed && enemy.TryGetAttackTelegraph(out float contact) && contact <= .2f)
            {
                pressed = parry.TryBeginParry();
                if (pressed) pressedContact = contact;
                if (pressed) { parry.playerAnimator.Update(0); parry.playerAnimator.Update(.000001f); }
            }
            if (pressed && TutorialRun.Practicing == TutorialRun.Practice.None) break;
            yield return null;
        }
        parry.enemy.OnStrike -= strike;
        parry.OnParrySuccess -= success;
        Assert.That(pressed, Is.True);
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.None));
        Assert.That(Object.FindObjectOfType<EnemyStamina>().Current, Is.LessThan(100),
            "strikes=" + strikeCount + " successes=" + successCount + " contact=" + pressedContact +
            " message=" + Object.FindObjectOfType<TutorialOverlay>().transform.Find("Reference960x540/Practice/PracticeMessage").GetComponent<TMPro.TMP_Text>().text);
        Assert.That(CombatSession.I.Parries, Is.Zero);
        Assert.That(ScoreSystem.I.Score, Is.Zero);
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PracticeRequiresRealHitAndParryAndMissesAreSafe()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        yield return EnterTypingLesson();
        yield return CompleteTyping();
        var director = Object.FindObjectOfType<TutorialDirector>();
        director.Continue(); yield return null; // mistakes
        director.Continue(); yield return null; // parry read
        director.Continue(); yield return null; // first attempt: no input
        var health = Object.FindObjectOfType<PlayerHealth>();
        int hp = health.Current;
        var pause = Object.FindObjectOfType<GamePauseController>();
        pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.Parry));
        pause.Resume();
        float deadline = Time.realtimeSinceStartup + 10;
        while (TutorialRun.Practicing != TutorialRun.Practice.None && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(health.Current, Is.EqualTo(hp));
        var text = Object.FindObjectOfType<TutorialOverlay>().transform.Find("Reference960x540/Practice/PracticeMessage").GetComponent<TMPro.TMP_Text>();
        Assert.That(text.text, Does.StartWith("Not parried"));
        yield return null;
        var parry = Object.FindObjectOfType<ParrySystem>();
        var enemy = Object.FindObjectOfType<SkeletonController>();
        for (int attempt = 0; attempt < 2; attempt++)
        {
            director.Continue(); yield return null;
            if (attempt == 0) Assert.That(parry.TryBeginParry(), Is.True, "early tap uses real pose");
            else
            {
                bool approaching = false;
                deadline = Time.realtimeSinceStartup + 8;
                while (Time.realtimeSinceStartup < deadline)
                {
                    bool contactPending = enemy.TryGetAttackTelegraph(out _);
                    approaching |= contactPending;
                    if (approaching && !contactPending) break;
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(.15f);
                Assert.That(parry.TryBeginParry(), Is.True, "late tap uses real pose");
            }
            deadline = Time.realtimeSinceStartup + 10;
            while (TutorialRun.Practicing != TutorialRun.Practice.None && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(health.Current, Is.EqualTo(hp));
            Assert.That(text.text, Does.StartWith("Not parried"));
            yield return null;
        }
        Assert.That(text.text, Does.Contain("Release Space"), "persistent help after three misses");
        yield return CompleteParry();
        Assert.That(health.Current, Is.EqualTo(hp));
        Assert.That(CombatSession.I.Seconds, Is.Zero);
        director.Continue(); yield return null; // Slayer
        director.Continue(); yield return null; // score
        director.Continue();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Completed));
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(CombatSession.I.Letters, Is.Zero);
        Assert.That(EndlessRun.Current.Score, Is.Zero);
        Assert.That(EndlessRun.Current.Slot, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CancellingTelegraphedPracticeAttackHidesCueAndPendingStrike()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        yield return EnterTypingLesson();
        yield return CompleteTyping();
        var director = Object.FindObjectOfType<TutorialDirector>();
        director.Continue(); yield return null;
        director.Continue(); yield return null;
        director.Continue(); yield return null;
        var enemy = Object.FindObjectOfType<SkeletonController>();
        float deadline = Time.realtimeSinceStartup + 8;
        while (!enemy.TryGetAttackTelegraph(out _) && Time.realtimeSinceStartup < deadline) yield return null;
        yield return null;
        var presenter = Object.FindObjectOfType<ParryCuePresenter>();
        var key = (RectTransform)typeof(ParryCuePresenter).GetField("key", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(presenter);
        Assert.That(key.gameObject.activeSelf, Is.True);
        Object.FindObjectOfType<TutorialOverlay>().transform.Find("Reference960x540/Practice/Back").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.None));
        Assert.That(enemy.IsAttacking, Is.False);
        Assert.That(key.gameObject.activeSelf, Is.False, "reading must not retain a cancelled live cue");
        Assert.That(GamePauseController.InputBlocked, Is.True);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator BackAndSkipCancelPracticeAndRestoreNormalDamage()
    {
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        yield return EnterTypingLesson();
        var director = Object.FindObjectOfType<TutorialDirector>();
        var overlay = Object.FindObjectOfType<TutorialOverlay>();
        director.Continue(); yield return null;
        Object.FindObjectOfType<TypingManager>().ProcessInput("hi");
        overlay.transform.Find("Reference960x540/Practice/Back").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.None));
        Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(Object.FindObjectOfType<SkeletonController>().IsAttacking, Is.False);
        yield return EnterTypingLesson();
        director.Continue(); yield return null;
        overlay.gameObject.SetActive(false); yield return null;
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.None));
        Assert.That(GamePauseController.InputBlocked, Is.True);
        overlay.gameObject.SetActive(true); yield return null;
        yield return EnterTypingLesson();
        director.Continue(); yield return null;
        overlay.transform.Find("Reference960x540/SkipTutorial").GetComponent<Button>().onClick.Invoke();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Skipped));
        Assert.That(TutorialRun.Practicing, Is.EqualTo(TutorialRun.Practice.None));
        var hp = Object.FindObjectOfType<PlayerHealth>();
        Assert.That(Object.FindObjectOfType<ParrySystem>().CanBeginParry, Is.True);
        int before = hp.Current;
        hp.TakeDamage(5, Vector2.zero);
        Assert.That(hp.Current, Is.EqualTo(before - 5), "tutorial safety cannot leak into real battle");
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
