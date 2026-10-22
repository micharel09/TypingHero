#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class DungeonEditorEntryTests
{
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        foreach (var name in new[] { DungeonSession.GameScene, "MainMenu", DungeonSession.WorldScene })
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        }
        if (DungeonSession.Current) Object.Destroy(DungeonSession.Current.gameObject);
        TutorialRun.End(); EndlessRun.Clear(); GamePauseController.ResetSessionState();
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator AdoptsLoadedGameplayAndInspectorEditsIntoAFreshFight()
    {
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var gate = Object.FindObjectOfType<DungeonSessionBindings>(true);
        Assert.That(gate.gameObject.activeSelf, Is.False, "direct scene was previously inert");
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        var player = gate.GetComponentInChildren<PlayerHealth>(true);
        player.maxHealth = 53; // An unsaved Inspector edit must survive bootstrap.
        TutorialRun.Begin();
        EndlessRun.Begin().Score = 123;
        GamePauseController.SetTutorialReading(true);
        int progress = PlayerPrefs.GetInt("TutorialProgress_v1", 0);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        Assert.That(session.TryBeginEditorGameplay(scene), Is.False, "duplicate startup");
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(SceneManager.GetActiveScene(), Is.EqualTo(scene));
        Assert.That(player.maxHealth, Is.EqualTo(53));
        Assert.That(player.Current, Is.EqualTo(53));
        Assert.That(TutorialRun.Active, Is.False);
        Assert.That(PlayerPrefs.GetInt("TutorialProgress_v1", 0), Is.EqualTo(progress));
        Assert.That(EndlessDuel.I.Run.Score, Is.Zero);
        Assert.That(EndlessDuel.I.Run.Difficulty, Is.EqualTo(EndlessDifficulties.Selected));
        Assert.That(CombatSession.I.Seconds, Is.Zero);
        Assert.That(GamePauseController.IsPaused, Is.False);
        Assert.That(GamePauseController.EncounterReading, Is.True, "wait for Fight");
        Assert.That(Object.FindObjectsOfType<PlayerHealth>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<SkeletonController>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<AudioListener>().Length, Is.EqualTo(1));
        Assert.That(session.TryBeginEditorGameplay(scene), Is.False, "already activated");
        EndlessDuel.I.Continue();
        yield return null;
        Assert.That(GamePauseController.InputBlocked, Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ReusesOpenWorldThroughDirectStartRestartAndMenuStart()
    {
        var session = DungeonSession.I;
        yield return session.LoadWorld();
        var world = session.World;
        var camera = world.view;
        var texture = world.Arena.WorldTexture;
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        Assert.That(session.TryBeginEditorGameplay(scene), Is.True);
        yield return DungeonSceneTestFlow.Wait();
        EndlessRun.Restart(); session.ReloadGameplay();
        yield return DungeonSceneTestFlow.Wait();
        session.ReturnToMenu();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        Assert.That(Time.timeScale, Is.EqualTo(1));
        TutorialRun.End(); EndlessRun.Prepare(); session.BeginGame();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(session.World, Is.SameAs(world));
        Assert.That(world.view, Is.SameAs(camera));
        Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
        Assert.That(Object.FindObjectsOfType<DungeonWorld>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<DungeonArenaCamera>().Length, Is.EqualTo(1));
        Assert.That(Object.FindObjectsOfType<DungeonPresentationCamera>().Length, Is.EqualTo(1));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LoadedMenuRetainsOwnershipOfStart()
    {
        yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        yield return null; // Let MainMenu.Start acquire the world-load lock.
        yield return DungeonSceneTestFlow.Wait();
        yield return SceneManager.LoadSceneAsync(DungeonSession.GameScene, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(DungeonSession.GameScene);
        SceneManager.SetActiveScene(scene);
        var session = DungeonSession.I;
        Assert.That(session.TryBeginEditorGameplay(scene), Is.False);
        Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
        Assert.That(CombatSession.I, Is.Null);
        TutorialRun.Begin();
        session.BeginGame();
        yield return DungeonSceneTestFlow.Wait();
        Assert.That(TutorialRun.Active, Is.True, "normal menu dispatch is preserved");
        Assert.That(Object.FindObjectsOfType<TutorialDirector>().Length, Is.EqualTo(1));
    }
}
#endif
