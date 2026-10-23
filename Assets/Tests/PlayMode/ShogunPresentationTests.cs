using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class ShogunPresentationTests
{
    static void Set(object target, string name, object value) => target.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    static Bounds VisibleGlyphs(TMP_Text text)
    {
        Bounds result = default;
        bool first = true;
        for (int i = 0; i < text.textInfo.characterCount; i++)
        {
            var c = text.textInfo.characterInfo[i];
            if (!c.isVisible) continue;
            var m = c.textElement.glyph.metrics;
            var topLeft = new Vector3(c.origin + m.horizontalBearingX * c.scale,
                c.baseLine + m.horizontalBearingY * c.scale, 0);
            var bottomRight = topLeft + new Vector3(m.width, -m.height, 0) * c.scale;
            if (first) { result = new Bounds(topLeft, Vector3.zero); first = false; }
            result.Encapsulate(topLeft); result.Encapsulate(bottomRight);
        }
        return result;
    }

    [Test, Category("Quick")]
    public void CombatLayoutKeepsAuthoredHudFixedAndTracksOnlyWorldPopup()
    {
        var root = new GameObject("layout", typeof(RectTransform), typeof(Canvas));
        root.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        var cameraHost = new GameObject("camera", typeof(Camera));
        var player = new GameObject("player", typeof(BoxCollider2D));
        var enemy = new GameObject("enemy", typeof(BoxCollider2D));
        RectTransform Box(string name, Transform parent, float width, float height)
        {
            var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rt.SetParent(parent, false); rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1); rt.sizeDelta = new Vector2(width, height); return rt;
        }
        try
        {
            var bounds = root.GetComponent<RectTransform>(); bounds.sizeDelta = new Vector2(960, 540);
            var camera = cameraHost.GetComponent<Camera>(); camera.orthographic = true;
            camera.pixelRect = new Rect(0, 0, 960, 540);
            player.transform.position = new Vector3(-2, 0, 0); enemy.transform.position = new Vector3(2, -.4f, 0);
            Physics2D.SyncTransforms();
            var layout = root.AddComponent<CombatHUDLayout>(); layout.enabled = false;
            var a = Box("player status", bounds, 112, 6); var b = Box("enemy status", bounds, 112, 12);
            var tray = Box("tray", bounds, 672, 56); var cluster = Box("score", bounds, 220, 48);
            var score = Box("total", cluster, 144, 24).gameObject.AddComponent<TextMeshProUGUI>();
            var combo = Box("combo", cluster, 64, 28);
            var multiplier = Box("multiplier", combo, 64, 24).gameObject.AddComponent<TextMeshProUGUI>();
            score.text = "128450"; multiplier.text = "x3.0";
            score.fontSize = 24; multiplier.fontSize = 16;
            Set(layout, "targetCamera", camera); Set(layout, "playerBody", player.GetComponent<BoxCollider2D>());
            Set(layout, "enemyBody", enemy.GetComponent<BoxCollider2D>());
            Set(layout, "playerStatus", a); Set(layout, "enemyStatus", b); Set(layout, "typingTray", tray);
            Set(layout, "scoreRoot", cluster); Set(layout, "scoreText", score); Set(layout, "multiplierText", multiplier);
            var decay = Box("decay", bounds, 112, 4); var popup = Box("popup", bounds, 1, 1);
            Set(layout, "comboRoot", combo); Set(layout, "comboDecay", decay);
            Set(layout, "popupAnchor", popup);
            var fx = root.AddComponent<SlayerScreenFX>(); fx.enabled = false;
            var bar = Box("top bar", bounds, 960, 160);
            Set(fx, "topBar", bar); Set(fx, "_topFrameHeight", 160f);
            Set(layout, "slayerFX", fx);
            foreach (float reveal in new[] { 0f, .5f, 1f, .5f, 0f, 1f, 0f })
            foreach (string digits in new[] { "0", "128450", "2147483647" })
            foreach (string tier in new[] { "x1.0", "x2.0", "x3.0" })
            foreach (float pulse in new[] { 1f, 1.2f })
            foreach (float zoom in new[] { 1f, 1.282051f })
            {
                Set(fx, "_barReveal", reveal);
                score.text = digits; multiplier.text = tier;
                multiplier.gameObject.SetActive(tier != "x1.0");
                camera.orthographicSize = 5 / zoom; cluster.localScale = Vector3.one * zoom;
                score.transform.localScale = Vector3.one * pulse;
                typeof(CombatHUDLayout).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(layout, null);
                Assert.That(a.anchoredPosition, Is.EqualTo(Vector2.zero), "HUD stays fixed when actors/camera move");
                Assert.That(b.anchoredPosition, Is.EqualTo(Vector2.zero));
                Assert.That(tray.anchoredPosition, Is.EqualTo(Vector2.zero));
                Assert.That(cluster.localScale, Is.EqualTo(Vector3.one));
                Assert.That(score.fontSize, Is.EqualTo(24));
                Assert.That(multiplier.fontSize, Is.EqualTo(16));
                var head = camera.WorldToScreenPoint(layout.Head(enemy.GetComponent<BoxCollider2D>()));
                Assert.That(popup.position.x, Is.EqualTo(head.x + 24).Within(.001f));
                Assert.That(popup.position.y, Is.EqualTo(head.y + 12).Within(.001f));
            }
        }
        finally
        {
            Object.DestroyImmediate(root); Object.DestroyImmediate(cameraHost);
            Object.DestroyImmediate(player); Object.DestroyImmediate(enemy);
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SlayerBarRevealSurvivesInterruptedExitPauseAndDisable()
    {
        var host = new GameObject("screen FX"); host.SetActive(false);
        var camera = host.AddComponent<Cinemachine.CinemachineVirtualCamera>();
        var fx = host.AddComponent<SlayerScreenFX>();
        var top = new GameObject("top", typeof(RectTransform)).GetComponent<RectTransform>();
        var bottom = new GameObject("bottom", typeof(RectTransform)).GetComponent<RectTransform>();
        Set(fx, "vcam", camera); Set(fx, "topBar", top); Set(fx, "bottomBar", bottom);
        try
        {
            host.SetActive(true);
            fx.EnterSlayerFX();
            yield return new WaitForSecondsRealtime(.06f);
            float partial = fx.BarReveal;
            Assert.That(partial, Is.InRange(.001f, .99f));
            // The projected frame can be much taller than serialized barHeight64.
            top.sizeDelta = new Vector2(960, 180);
            fx.ExitSlayerFX();
            Assert.That(fx.BarReveal, Is.LessThanOrEqualTo(partial), "exit must start at reveal, not projected pixels");
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(.7f);
            Assert.That(fx.BarReveal, Is.Zero);
            fx.EnterSlayerFX();
            yield return new WaitForSecondsRealtime(.7f);
            Assert.That(fx.BarReveal, Is.EqualTo(1));
            fx.ExitSlayerFX();
            yield return new WaitForSecondsRealtime(.06f);
            partial = fx.BarReveal;
            fx.EnterSlayerFX();
            Assert.That(fx.BarReveal, Is.GreaterThanOrEqualTo(partial));
            yield return new WaitForSecondsRealtime(.7f);
            Assert.That(fx.BarReveal, Is.EqualTo(1));
            fx.enabled = false;
            Assert.That(fx.BarReveal, Is.Zero);
        }
        finally
        {
            Time.timeScale = 1;
            Object.DestroyImmediate(host); Object.DestroyImmediate(top.gameObject); Object.DestroyImmediate(bottom.gameObject);
        }
    }

    [Test, Category("Quick")]
    public void FloatingAwardFollowsMovingAnchorWithoutRestartingFade()
    {
        var canvasHost = new GameObject("canvas", typeof(RectTransform), typeof(Canvas));
        var anchor = new GameObject("anchor", typeof(RectTransform)).GetComponent<RectTransform>();
        var popupHost = new GameObject("popup", typeof(RectTransform), typeof(TextMeshProUGUI));
        try
        {
            var canvas = canvasHost.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            anchor.SetParent(canvas.transform, false); popupHost.transform.SetParent(canvas.transform, false);
            var popup = popupHost.AddComponent<FloatingScore>();
            popup.Setup("+18", Color.yellow, canvas, anchor, new Vector2(3, 4));
            Set(popup, "life", 1f); Set(popup, "fadeDelay", 0f); Set(popup, "riseDistance", 0f);
            Set(popup, "_elapsed", .5f);
            popup.SetPaused(true);
            var update = typeof(FloatingScore).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            update.Invoke(popup, null);
            var initial = popupHost.GetComponent<RectTransform>().anchoredPosition;
            anchor.position += new Vector3(37, -23, 0);
            update.Invoke(popup, null);
            var moved = popupHost.GetComponent<RectTransform>().anchoredPosition - initial;
            Assert.That(moved.x, Is.EqualTo(37).Within(.001f));
            Assert.That(moved.y, Is.EqualTo(-23).Within(.001f));
            Assert.That(popupHost.GetComponent<TMP_Text>().text, Is.EqualTo("+18"));
            Assert.That(popupHost.GetComponent<TMP_Text>().color.a, Is.EqualTo(.5f).Within(.01f));
            popup.AddAward(18, "0", Color.yellow);
            popup.AddAward(7, "0", Color.red);
            Assert.That(popupHost.GetComponent<TMP_Text>().text, Is.EqualTo("+25"));
            Assert.That(popupHost.GetComponent<TMP_Text>().color.a, Is.EqualTo(1));
            Set(popup, "_elapsed", .5f);
            update.Invoke(popup, null);
            Assert.That(popupHost.GetComponent<TMP_Text>().color.a, Is.EqualTo(.5f).Within(.01f));
            var last = popupHost.GetComponent<RectTransform>().anchoredPosition;
            Object.DestroyImmediate(anchor.gameObject);
            update.Invoke(popup, null);
            Assert.That(popupHost.GetComponent<RectTransform>().anchoredPosition, Is.EqualTo(last));
        }
        finally { Object.DestroyImmediate(popupHost); if (anchor) Object.DestroyImmediate(anchor.gameObject); Object.DestroyImmediate(canvasHost); }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ScoreAwardsShareOnePopupPauseAndRestartClearIt()
    {
        var canvasHost = new GameObject("canvas", typeof(RectTransform), typeof(Canvas));
        var prefab = new GameObject("award prefab", typeof(RectTransform), typeof(TextMeshProUGUI));
        var scoreHost = new GameObject("score owner"); scoreHost.SetActive(false);
        var rules = ScriptableObject.CreateInstance<ScoreRules>();
        var matchHost = new GameObject("match");
        var match = matchHost.AddComponent<CombatSession>();
        var anchor = new GameObject("anchor", typeof(RectTransform)).GetComponent<RectTransform>();
        try
        {
            var canvas = canvasHost.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            anchor.SetParent(canvas.transform, false);
            var template = prefab.AddComponent<FloatingScore>(); template.enabled = false;
            var system = scoreHost.AddComponent<ScoreSystem>();
            Set(system, "rules", rules); Set(system, "floatingPrefab", template);
            Set(system, "floatingCanvas", canvas); Set(system, "popupAnchor", anchor);
            scoreHost.SetActive(true);
            var spawn = typeof(ScoreSystem).GetMethod("SpawnFloating", BindingFlags.Instance | BindingFlags.NonPublic);
            spawn.Invoke(system, new object[] { 10, 1f, null });
            spawn.Invoke(system, new object[] { 30, 2f, null });
            var popup = canvas.GetComponentInChildren<FloatingScore>();
            Assert.That(canvas.GetComponentsInChildren<FloatingScore>().Length, Is.EqualTo(1));
            Assert.That(popup.GetComponent<TMP_Text>().text, Is.EqualTo("+40"));
            system.SetPaused(true);
            yield return new WaitForSecondsRealtime(.7f);
            Assert.That(popup, Is.Not.Null);
            Assert.That(popup.GetComponent<TMP_Text>().color.a, Is.EqualTo(1));
            system.SetPaused(false);
            yield return new WaitForSecondsRealtime(.3f);
            Assert.That(popup.GetComponent<TMP_Text>().color.a, Is.InRange(.1f, .9f));
            spawn.Invoke(system, new object[] { 6, 3f, null });
            Assert.That(canvas.GetComponentsInChildren<FloatingScore>().Length, Is.EqualTo(1));
            Assert.That(popup.GetComponent<TMP_Text>().text, Is.EqualTo("+46"));
            Assert.That(popup.GetComponent<TMP_Text>().color.a, Is.EqualTo(1));
            system.ResetScore();
            Assert.That(canvas.GetComponentsInChildren<FloatingScore>().Length, Is.EqualTo(0));
            yield return null;
            spawn.Invoke(system, new object[] { 10, 1f, null });
            popup = canvas.GetComponentInChildren<FloatingScore>();
            Assert.That(popup.GetComponent<TMP_Text>().text, Is.EqualTo("+10"));
            match.Complete(true);
            Object.DestroyImmediate(anchor.gameObject);
            yield return new WaitForSecondsRealtime(.7f);
            Assert.That(canvas.GetComponentsInChildren<FloatingScore>().Length, Is.EqualTo(0));
        }
        finally
        {
            Object.DestroyImmediate(matchHost); Object.DestroyImmediate(scoreHost); Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(canvasHost); Object.DestroyImmediate(rules);
        }
    }

#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void SerializedUICuesHaveOriginalClipsMixerAndPauseRouting()
    {
        var host = new GameObject("audio routing"); var source = host.AddComponent<AudioSource>();
        try
        {
            foreach (var name in new[] { "Confirm", "Back" })
            {
                var ev = UnityEditor.AssetDatabase.LoadAssetAtPath<SfxEvent>("Assets/Audio/SFX/Events/UI/" + name + ".asset");
                Assert.That(ev, Is.Not.Null); Assert.That(ev.PickClip(), Is.Not.Null);
                Assert.That(ev.PickClip().name, Is.EqualTo(name == "Confirm" ? "click1" : "switch2"));
                ev.ApplyToSource(source);
                Assert.That(source.ignoreListenerPause, Is.True);
                Assert.That(source.outputAudioMixerGroup, Is.Not.Null);
                Assert.That(source.volume, Is.EqualTo(.45f).Within(.001f));
            }
            var combat = UnityEditor.AssetDatabase.LoadAssetAtPath<SfxEvent>("Assets/Audio/SFX/Events/Combat/Player/ev_impact_normal.asset");
            var group = source.outputAudioMixerGroup; combat.ApplyToSource(source);
            Assert.That(source.ignoreListenerPause, Is.False);
            Assert.That(source.outputAudioMixerGroup, Is.EqualTo(group));
        }
        finally { Object.DestroyImmediate(host); }
    }
#endif

    [UnityTest, Category("Quick")]
    public IEnumerator ScoreZoomRestoresBaseScaleBeforeCameraFinishesExiting()
    {
        var cameraHost = new GameObject("score camera", typeof(Camera));
        var camera = cameraHost.GetComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 5;
        var host = new GameObject("score cluster", typeof(RectTransform)); host.SetActive(false);
        var binder = host.AddComponent<ScoreUI_SlayerBinder>();
        Set(binder, "targetCam", camera); Set(binder, "scoreUIRoot", host.GetComponent<RectTransform>());
        try
        {
            SlayerModeSignals.SetActive(false); host.SetActive(true);
            SlayerModeSignals.SetActive(true); camera.orthographicSize = 2.5f;
            yield return new WaitForSecondsRealtime(.2f);
            Assert.That(host.transform.localScale.x, Is.EqualTo(2).Within(.001f));
            SlayerModeSignals.SetActive(false);
            // The screen FX camera has its own longer exit tween. UI must return to baseline independently.
            yield return new WaitForSecondsRealtime(.2f);
            Assert.That(host.transform.localScale, Is.EqualTo(Vector3.one));
        }
        finally
        {
            SlayerModeSignals.SetActive(false);
            Object.Destroy(host); Object.Destroy(cameraHost);
        }
        yield return null;
    }

    static GameObject PausePanel(Transform parent)
    {
        var panel = new GameObject("pause panel", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        panel.SetActive(false); panel.transform.SetParent(parent, false);
        foreach (var name in new[] { "PauseLabel", "ResumeButton", "RestartButton", "SettingsButton", "Top 1000Button", "Main MenuButton", "AudioPreferences", "KeyHint" })
        {
            var child = new GameObject(name, typeof(RectTransform)); child.transform.SetParent(panel.transform, false);
            if (name == "PauseLabel") child.AddComponent<TextMeshProUGUI>();
            if (name.EndsWith("Button"))
            {
                child.AddComponent<Button>();
                var label = new GameObject("label", typeof(RectTransform), typeof(TextMeshProUGUI)); label.transform.SetParent(child.transform, false);
            }
        }
        var key = new GameObject("key-r"); key.transform.SetParent(panel.transform.Find("RestartButton"), false);
        panel.AddComponent<CompactPauseLayout>(); panel.AddComponent<PanelReveal>(); panel.SetActive(true);
        return panel;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PauseModalsHideRevealPanelAndRestoreAudioFocus()
    {
        var events = new GameObject("events", typeof(EventSystem));
        var canvas = new GameObject("canvas", typeof(RectTransform), typeof(Canvas));
        var previous = PausePanel(canvas.transform);
        var panel = new GameObject("modal panel", typeof(RectTransform));
        var card = new GameObject("SettingsPanel", typeof(RectTransform)); card.transform.SetParent(panel.transform);
        var close = new GameObject("close", typeof(RectTransform), typeof(Button)); close.transform.SetParent(panel.transform);
        var host = new GameObject("modal owner");
        var settings = host.AddComponent<SettingsModal>(); var leaderboard = host.AddComponent<LeaderboardModal>();
        var audio = (RectTransform)previous.transform.Find("AudioPreferences");
        audio.anchoredPosition = new Vector2(12, 34); audio.sizeDelta = new Vector2(56, 78); audio.gameObject.SetActive(false);
        Set(settings, "panel", panel); Set(settings, "audio", audio); Set(settings, "previousScreen", previous.GetComponent<CanvasGroup>()); Set(settings, "backButton", close.GetComponent<Button>());
        Set(leaderboard, "panel", panel); Set(leaderboard, "previousScreen", previous.GetComponent<CanvasGroup>()); Set(leaderboard, "closeButton", close.GetComponent<Button>());
        panel.SetActive(false);
        try
        {
            Time.timeScale = 0; AudioListener.pause = true;
            var opener = previous.transform.Find("SettingsButton").GetComponent<Button>(); opener.gameObject.SetActive(true); opener.Select();
            settings.Open(); yield return new WaitForSecondsRealtime(.16f);
            Assert.That(previous.activeSelf, Is.False); Assert.That(panel.activeInHierarchy, Is.True);
            Assert.That(audio.IsChildOf(panel.transform), Is.True); Assert.That(audio.gameObject.activeInHierarchy, Is.True);
            Assert.That(Time.timeScale, Is.Zero); Assert.That(AudioListener.pause, Is.True);
            settings.Close(); Assert.That(SettingsModal.BlocksShortcuts, Is.True);
            Assert.That(previous.activeSelf, Is.True); Assert.That(audio.parent, Is.EqualTo(previous.transform));
            Assert.That(audio.anchoredPosition, Is.EqualTo(new Vector2(12, 34))); Assert.That(audio.sizeDelta, Is.EqualTo(new Vector2(56, 78)));
            Assert.That(audio.gameObject.activeSelf, Is.False); Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(opener.gameObject));
            yield return null;
            opener = previous.transform.Find("Top 1000Button").GetComponent<Button>(); opener.Select();
            leaderboard.Open(); yield return new WaitForSecondsRealtime(.16f);
            Assert.That(previous.activeSelf, Is.False); Assert.That(LeaderboardModal.IsOpen, Is.True);
            leaderboard.Close(); Assert.That(LeaderboardModal.BlocksShortcuts, Is.True);
            Assert.That(previous.activeSelf, Is.True); Assert.That(previous.GetComponent<CanvasGroup>().interactable, Is.True);
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(opener.gameObject));
            Assert.That(Time.timeScale, Is.Zero); Assert.That(AudioListener.pause, Is.True);
        }
        finally { Object.DestroyImmediate(host); Object.DestroyImmediate(panel); Object.DestroyImmediate(canvas); Object.DestroyImmediate(events); GamePauseController.ResetSessionState(); }
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ResumeKeepsCompactLayoutFrozenUntilCameraExitAndRejectsRepeatedActions()
    {
        var events = new GameObject("events", typeof(EventSystem));
        var canvas = new GameObject("canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        var panel = PausePanel(canvas.transform); var rect = panel.GetComponent<RectTransform>(); rect.sizeDelta = new Vector2(600, 500);
        var cameraHost = new GameObject("camera", typeof(Camera)); var camera = cameraHost.GetComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 5;
        var host = new GameObject("pause owner"); host.SetActive(false);
        var pause = host.AddComponent<GamePauseController>(); Set(pause, "pauseCanvas", canvas.GetComponent<Canvas>()); Set(pause, "targetCamera", camera);
        Set(pause, "pauseLabel", panel.transform.Find("PauseLabel").GetComponent<TextMeshProUGUI>()); Set(pause, "zoomDuration", .2f); Set(pause, "pauseOnLoseFocus", false);
        try
        {
            host.SetActive(true); pause.TogglePause(true);
            Assert.That(panel.GetComponent<Image>().enabled, Is.False, "Pause has no enclosing panel");
            pause.TogglePause(true);
            Assert.That(panel.GetComponent<Image>().enabled, Is.False);
            panel.transform.Find("ResumeButton").GetComponent<Button>().Select();
            pause.Resume(); pause.Resume(); pause.Restart(); pause.ReturnToMenu(); pause.TogglePause(true);
            Assert.That(pause.IsRestarting, Is.False);
            Assert.That(rect.sizeDelta, Is.EqualTo(new Vector2(420, 340)));
            Assert.That(canvas.GetComponent<CanvasGroup>().interactable, Is.False);
            yield return null;
            Assert.That(canvas.GetComponent<Canvas>().enabled, Is.True); Assert.That(GamePauseController.InputBlocked, Is.True);
            Assert.That(Time.timeScale, Is.Zero); Assert.That(AudioListener.pause, Is.True);
            GamePauseController.SetTutorialReading(true);
            yield return new WaitForSecondsRealtime(.3f);
            Assert.That(canvas.GetComponent<Canvas>().enabled, Is.False); Assert.That(rect.sizeDelta, Is.EqualTo(new Vector2(600, 500)));
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.Null); Assert.That(camera.orthographicSize, Is.EqualTo(5));
            Assert.That(GamePauseController.InputBlocked, Is.True); GamePauseController.SetTutorialReading(false);
            Assert.That(GamePauseController.InputBlocked, Is.False); Assert.That(Time.timeScale, Is.EqualTo(1));
            pause.ShowVictory(); Assert.That(rect.sizeDelta, Is.EqualTo(new Vector2(600, 500)));
            Assert.That(panel.GetComponent<Image>().enabled, Is.True, "Results retain the authored panel");
            Assert.That(canvas.GetComponent<Canvas>().enabled, Is.True);
        }
        finally { Object.DestroyImmediate(host); Object.DestroyImmediate(canvas); Object.DestroyImmediate(cameraHost); Object.DestroyImmediate(events); GamePauseController.ResetSessionState(); }
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ResumeClearsSelectionAndDisablesHiddenPauseButtons()
    {
        var events = new GameObject("events", typeof(EventSystem));
        var canvas = new GameObject("pause", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        var button = new GameObject("hidden button", typeof(RectTransform), typeof(Button));
        button.transform.SetParent(canvas.transform);
        int calls = 0; button.GetComponent<Button>().onClick.AddListener(() => calls++);
        var host = new GameObject("pause owner"); host.SetActive(false);
        var pause = host.AddComponent<GamePauseController>(); Set(pause, "pauseCanvas", canvas.GetComponent<Canvas>());
        host.SetActive(true); pause.TogglePause(true); button.GetComponent<Button>().Select();
        Assert.That(button.GetComponent<Button>().IsInteractable(), Is.True);
        pause.Resume(); yield return null;
        Assert.That(canvas.GetComponent<CanvasGroup>().interactable, Is.False);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.Null);
        ExecuteEvents.Execute(button, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        Assert.That(calls, Is.Zero, "Space/Submit must not invoke a hidden pause action");
        Assert.That(Time.timeScale, Is.EqualTo(1)); Assert.That(GamePauseController.InputBlocked, Is.False);
        Object.Destroy(host); Object.Destroy(canvas); Object.Destroy(events); AudioListener.pause = false;
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ModalRestoresFocusAndKeepsPausedMatchFrozen()
    {
        var events = new GameObject("events", typeof(EventSystem));
        var previous = new GameObject("previous", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup), typeof(Button));
        var host = new GameObject("modal"); var panel = new GameObject("panel");
        var close = new GameObject("close", typeof(Button));
        close.transform.SetParent(panel.transform);
        var modal = host.AddComponent<LeaderboardModal>();
        Set(modal, "panel", panel); Set(modal, "closeButton", close.GetComponent<Button>());
        Set(modal, "previousScreen", previous.GetComponent<CanvasGroup>());
        panel.SetActive(false); previous.GetComponent<Button>().Select();
        Time.timeScale = 0; AudioListener.pause = true;
        modal.Open(); yield return null;
        Assert.That(LeaderboardModal.IsOpen, Is.True);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(close));
        Assert.That(previous.GetComponent<CanvasGroup>().interactable, Is.False);
        modal.Close();
        Assert.That(LeaderboardModal.BlocksShortcuts, Is.True, "Closing a modal must consume the current frame's shortcuts");
        yield return null;
        Assert.That(LeaderboardModal.BlocksShortcuts, Is.False);
        Assert.That(LeaderboardModal.IsOpen, Is.False);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(previous));
        Assert.That(previous.GetComponent<CanvasGroup>().blocksRaycasts, Is.True);
        Assert.That(Time.timeScale, Is.Zero); Assert.That(AudioListener.pause, Is.True);
        previous.GetComponent<Canvas>().enabled = false;
        modal.Open();
        Assert.That(LeaderboardModal.IsOpen, Is.False, "Hidden pause controls cannot open leaderboard during combat");
        previous.GetComponent<Canvas>().enabled = true;
        modal.Open(); modal.Close();
        Assert.That(previous.GetComponent<CanvasGroup>().interactable, Is.True);
        Time.timeScale = 1; AudioListener.pause = false;
        Object.Destroy(host); Object.Destroy(panel); Object.Destroy(previous); Object.Destroy(events);
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator UIAudioReleasesDuringPauseAndCombatReuseResetsPausePolicy()
    {
        var host = new GameObject("sfx"); host.SetActive(false);
        var player = host.AddComponent<SfxPlayer>();
        Set(player, "initialVoices", 1); Set(player, "maxVoices", 1); host.SetActive(true);
        var clip = AudioClip.Create("cue", 4410, 1, 44100, false);
        var ui = ScriptableObject.CreateInstance<SfxEvent>();
        var combat = ScriptableObject.CreateInstance<SfxEvent>();
        Set(ui, "clips", new[] { clip }); Set(combat, "clips", new[] { clip });
        Set(ui, "ignoreListenerPause", true);
        AudioListener.pause = true;
        var voice = player.Play(ui, Vector3.zero);
        Assert.That(voice.ignoreListenerPause, Is.True);
        yield return new WaitForSecondsRealtime(.25f);
        Assert.That(voice.clip, Is.Null, "UI cue must return to pool while listener is paused");
        var hit = player.Play(combat, Vector3.zero);
        Assert.That(hit, Is.SameAs(voice), "Exercise the same pooled AudioSource");
        Assert.That(hit.ignoreListenerPause, Is.False);
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(hit.clip, Is.EqualTo(clip), "Combat voice must remain held during pause");
        AudioListener.pause = false;
        Object.Destroy(host); Object.Destroy(ui); Object.Destroy(combat); Object.Destroy(clip);
        yield return null;
    }
}
