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
    public IEnumerator ParryModalUsesCapturedDurationsHoldsPauseAndResetsOnBack()
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
        Assert.That(panel.sizeDelta, Is.EqualTo(new Vector2(816, 480)));
        var image = panel.Find("Clip").GetComponent<Image>();
        var frames = (Sprite[])typeof(TutorialOverlay).GetField("clipFrames", flags).GetValue(overlay);
        var durations = (float[])typeof(TutorialOverlay).GetField("clipDurations", flags).GetValue(overlay);
        Assert.That(frames.Length, Is.EqualTo(77));
        Assert.That(frames.All(f => f && f.texture.filterMode == FilterMode.Point), Is.True);
        double time = 0;
        for (int i = 0; i < frames.Length; i++)
        {
            clock.SetValue(overlay, time + durations[i] * .5); render.Invoke(overlay, null);
            Assert.That(image.sprite, Is.SameAs(frames[i]), "captured frame/duration " + i);
            Assert.That(durations[i], Is.GreaterThan(0));
            time += durations[i];
        }
        clock.SetValue(overlay, time + .001); render.Invoke(overlay, null);
        Assert.That(image.sprite, Is.SameAs(frames[0]), "loop boundary");
        clock.SetValue(overlay, 1.5); render.Invoke(overlay, null);
        var held = image.sprite;
        var pause = Object.FindObjectOfType<GamePauseController>();
        pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That((double)clock.GetValue(overlay), Is.EqualTo(1.5));
        Assert.That(image.sprite, Is.SameAs(held));
        pause.Resume();
        yield return new WaitForSecondsRealtime(.35f);
        Assert.That((double)clock.GetValue(overlay), Is.GreaterThan(1.5));
        Assert.That(Time.timeScale, Is.Zero);
        panel.Find("BackButton").GetComponent<Button>().onClick.Invoke();
        Assert.That(modal.gameObject.activeSelf, Is.False);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        yield return null;
        director.Continue();
        Assert.That((double)clock.GetValue(overlay), Is.Zero);
        Assert.That(image.sprite, Is.SameAs(frames[0]));
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
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(GamePauseController.InputBlocked, Is.True);
        pause.Restart();
        yield return new WaitForSecondsRealtime(.3f);
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Active, Is.True);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        Object.FindObjectOfType<GamePauseController>().ReturnToMenu();
        yield return new WaitForSecondsRealtime(.5f);
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        director = Object.FindObjectOfType<TutorialDirector>();
        director.Continue(); director.Continue();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Unseen));
        yield return null;
        director.Continue(); director.Continue();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Status, Is.EqualTo(TutorialRun.Progress.Completed));
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(DungeonSession.I.World, Is.SameAs(world));
        Assert.That(EndlessDuel.I.Run.Difficulty, Is.EqualTo(EndlessDifficulties.Selected));
        Assert.That(EndlessDuel.I.Run.Score, Is.Zero);
        Assert.That(EndlessDuel.I.Run.Slot, Is.Zero);
        Assert.That(CombatSession.I.Letters, Is.Zero);
        Assert.That(CombatSession.I.Seconds, Is.InRange(0f, 1f), "Fresh encounter clock starts immediately");
        Assert.That(GamePauseController.EncounterReading, Is.False);
        var encounter = Object.FindObjectOfType<EndlessOverlay>();
        var panel = encounter.transform.Find("Reference960x540/TutorialPrompt");
        foreach (string name in new[] { "HealthGuide", "EnemyGuide", "TypingGuide", "ScoreGuide", "Shortcuts", "SpaceExample" })
            Assert.That(panel.Find(name).gameObject.activeInHierarchy, Is.False, "guide must not leak into the encounter clone");
        Assert.That(encounter.transform.Find("Reference960x540/ParryModal").gameObject.activeSelf, Is.False);
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
