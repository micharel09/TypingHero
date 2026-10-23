using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class DungeonEditorPlayModeTests
{
    bool optionsEnabled;
    EnterPlayModeOptions options;
    SceneSetup[] scenes;

    [SetUp]
    public void SaveEditorSetup()
    {
        optionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
        options = EditorSettings.enterPlayModeOptions;
        scenes = EditorSceneManager.GetSceneManagerSetup();
        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
    }

    [UnityTearDown]
    public IEnumerator RestoreEditorSetup()
    {
        if (EditorApplication.isPlaying) yield return new ExitPlayMode();
        TutorialRun.End(); EndlessRun.Clear(); GamePauseController.ResetSessionState();
        EditorSettings.enterPlayModeOptionsEnabled = optionsEnabled;
        EditorSettings.enterPlayModeOptions = options;
        if (System.Array.Exists(scenes, setup => setup.isLoaded && setup.isActive))
            EditorSceneManager.RestoreSceneManagerSetup(scenes);
        else
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator ActualPlayTwiceWithoutDomainReloadPreservesEditsAndClearsRun()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var gate = Object.FindObjectOfType<DungeonSessionBindings>(true);
        var inspector = new SerializedObject(gate.GetComponentInChildren<PlayerHealth>(true));
        inspector.FindProperty("maxHealth").intValue = 53;
        inspector.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.MarkSceneDirty(scene);
        int progress = PlayerPrefs.GetInt("TutorialProgress_v1", 0);
        for (int entry = 0; entry < 2; entry++)
        {
            yield return new EnterPlayMode(expectDomainReload: false);
            yield return WaitForStartup();
            var session = DungeonSession.Current;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(DungeonSession.GameScene));
            Assert.That(Object.FindObjectOfType<PlayerHealth>().maxHealth, Is.EqualTo(53));
            Assert.That(Object.FindObjectOfType<PlayerHealth>().Current, Is.EqualTo(53));
            Assert.That(TutorialRun.Active, Is.False);
            Assert.That(EndlessRun.Current.Score, Is.Zero);
            Assert.That(GamePauseController.IsPaused, Is.False);
            Assert.That(GamePauseController.EncounterReading, Is.False);
            Assert.That(CombatSession.I.Seconds, Is.InRange(0f, 1f), "Fresh encounter clock starts immediately");
            Assert.That(Object.FindObjectsOfType<DungeonWorld>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsOfType<DungeonArenaCamera>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsOfType<DungeonPresentationCamera>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsOfType<AudioListener>().Length, Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetInt("TutorialProgress_v1", 0), Is.EqualTo(progress));
            var world = session.World;
            var texture = world.Arena.WorldTexture;
            EndlessDuel.I.Continue(); // No-op during the already active encounter.
            Assert.That(GamePauseController.InputBlocked, Is.False);
            var typing = Object.FindObjectOfType<TypingManager>();
            var word = (string)typeof(TypingManager).GetField("current", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(typing);
            Assert.That(word, Is.Not.Empty);
            typing.ProcessInput(word);
            Assert.That(CombatSession.I.Letters, Is.EqualTo(word.Length));
            int oldHandle = SceneManager.GetActiveScene().handle;
            var pause = Object.FindObjectOfType<GamePauseController>();
            pause.TogglePause(true); pause.Restart();
            float deadline = Time.realtimeSinceStartup + 30;
            while ((SceneManager.GetActiveScene().handle == oldHandle || session.Transitioning)
                && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(SceneManager.GetActiveScene().handle, Is.Not.EqualTo(oldHandle));
            Assert.That(session.Transitioning, Is.False);
            yield return null; yield return null;
            Assert.That(session.World, Is.SameAs(world));
            Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
            Assert.That(EndlessRun.Current.Score, Is.Zero);
            Assert.That(GamePauseController.EncounterReading, Is.False);
            pause = Object.FindObjectOfType<GamePauseController>();
            pause.TogglePause(true); pause.ReturnToMenu();
            deadline = Time.realtimeSinceStartup + 30;
            while ((!SceneManager.GetSceneByName("MainMenu").isLoaded || session.Transitioning)
                && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(SceneManager.GetSceneByName("MainMenu").isLoaded, Is.True);
            Assert.That(session.Transitioning, Is.False);
            Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
            Assert.That(Time.timeScale, Is.EqualTo(1));
            Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
            // Leave stale managed state deliberately; the next Play must reset it.
            TutorialRun.Begin();
            EndlessRun.Prepare();
            EndlessRun.Current.Score = 881;
            GamePauseController.SetTutorialReading(true);
            yield return new ExitPlayMode();
        }
        Assert.That(EditorSceneManager.GetActiveScene().isDirty, Is.True);
        Assert.That(Object.FindObjectOfType<DungeonSessionBindings>(true)
            .GetComponentInChildren<PlayerHealth>(true).maxHealth, Is.EqualTo(53));
    }

    [UnityTest]
    public IEnumerator ActualPlayWithMenuAndGameplayOpenLeavesGameplayInactive()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        var game = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Additive);
        SceneManager.SetActiveScene(game);
        yield return new EnterPlayMode(expectDomainReload: false);
        yield return WaitForStartup();
        Assert.That(SceneManager.GetSceneByName("MainMenu").isLoaded, Is.True);
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        Assert.That(CombatSession.I, Is.Null);
        Assert.That(Object.FindObjectOfType<DungeonSessionBindings>(true).gameObject.activeSelf, Is.False);
        yield return new ExitPlayMode();
    }

    static IEnumerator WaitForStartup()
    {
        float deadline = Time.realtimeSinceStartup + 30;
        while ((!DungeonSession.Current || !DungeonSession.Current.World || DungeonSession.Current.Transitioning)
            && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(DungeonSession.Current, Is.Not.Null);
        Assert.That(DungeonSession.Current.World, Is.Not.Null);
        Assert.That(DungeonSession.Current.Transitioning, Is.False);
        yield return null; yield return null;
    }
}
