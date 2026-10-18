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

    [Test, Category("Quick")]
    public void CombatLayoutTracksBodiesAndCentersZoomedPulsingScore()
    {
        var root = new GameObject("layout", typeof(RectTransform));
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
            var a = Box("player status", bounds, 112, 30); var b = Box("enemy status", bounds, 112, 30);
            var tray = Box("tray", bounds, 672, 56); var cluster = Box("score", bounds, 220, 48);
            var score = Box("total", cluster, 144, 24).gameObject.AddComponent<TextMeshProUGUI>();
            var combo = Box("combo", cluster, 64, 28);
            var multiplier = Box("multiplier", combo, 64, 24).gameObject.AddComponent<TextMeshProUGUI>();
            score.text = "128450"; multiplier.text = "x3.0";
            score.fontSize = 24; multiplier.fontSize = 24;
            Set(layout, "targetCamera", camera); Set(layout, "playerBody", player.GetComponent<BoxCollider2D>());
            Set(layout, "enemyBody", enemy.GetComponent<BoxCollider2D>());
            Set(layout, "playerStatus", a); Set(layout, "enemyStatus", b); Set(layout, "typingTray", tray);
            Set(layout, "scoreRoot", cluster); Set(layout, "scoreText", score); Set(layout, "multiplierText", multiplier);
            var decay = Box("decay", bounds, 112, 4); var popup = Box("popup", bounds, 1, 1);
            Set(layout, "comboRoot", combo); Set(layout, "comboDecay", decay);
            Set(layout, "popupAnchor", popup);
            foreach (string digits in new[] { "0", "128450", "2147483647" })
            foreach (string tier in new[] { "x1.0", "x2.0", "x3.0" })
            foreach (float pulse in new[] { 1f, 1.2f })
            foreach (float zoom in new[] { 1f, 1.282051f })
            {
                score.text = digits; multiplier.text = tier;
                multiplier.gameObject.SetActive(tier != "x1.0");
                camera.orthographicSize = 5 / zoom; cluster.localScale = Vector3.one * zoom;
                score.transform.localScale = Vector3.one * pulse;
                typeof(CombatHUDLayout).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(layout, null);
                var foot = camera.WorldToScreenPoint(layout.Foot(player.GetComponent<BoxCollider2D>()));
                Assert.That(a.position.x + 56, Is.EqualTo(foot.x).Within(.001f));
                Assert.That(a.position.y, Is.EqualTo(foot.y - 12).Within(.001f));
                Assert.That(tray.position.y, Is.EqualTo(Mathf.Min(a.position.y, b.position.y) - 94).Within(.001f));
                float width = (score.preferredWidth * pulse + (multiplier.gameObject.activeSelf ? 8 + multiplier.preferredWidth : 0)) * zoom;
                Assert.That(cluster.position.x + width * .5f, Is.EqualTo(bounds.position.x).Within(.001f));
                Assert.That(cluster.position.y, Is.EqualTo(Mathf.Min(a.position.y, b.position.y) - 38).Within(.001f));
                Assert.That(decay.position.y, Is.LessThan(cluster.position.y - 24 * pulse * zoom));
                Assert.That(decay.position.y - 4, Is.GreaterThan(tray.position.y + 8));
                Assert.That(popup.position.x, Is.EqualTo(cluster.position.x + width + 12).Within(.001f));
            }
        }
        finally
        {
            Object.DestroyImmediate(root); Object.DestroyImmediate(cameraHost);
            Object.DestroyImmediate(player); Object.DestroyImmediate(enemy);
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
            Set(popup, "_t0", Time.unscaledTime - .5f);
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
        }
        finally { Object.DestroyImmediate(popupHost); Object.DestroyImmediate(anchor.gameObject); Object.DestroyImmediate(canvasHost); }
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
