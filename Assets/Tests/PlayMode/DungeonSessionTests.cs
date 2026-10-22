using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class DungeonSessionTests
{
    DungeonSession owned;
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        foreach (var name in new[] { "MainMenu", DungeonSession.GameScene, LightingTestAssets.WorldScene })
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        }
        if (owned) Object.Destroy(owned.gameObject);
        TutorialRun.End(); EndlessRun.Clear(); GamePauseController.ResetSessionState();
        yield return null;
    }
    [UnityTest, Category("Quick")]
    public IEnumerator WorldSurvivesThreeGameplayCyclesAndResetsSlayerPause()
    {
        var session = owned = DungeonSession.I;
        yield return session.LoadWorld(LightingTestAssets.Definition);
        var world = session.World;
        var room = world.environment;
        var camera = world.view;
        var texture = world.Arena.WorldTexture;
        var presenter = world.Arena.PresentationCamera;
        TutorialRun.End(); EndlessRun.Prepare();
        for (int cycle = 0; cycle < 3; cycle++)
        {
            session.BeginGame();
            session.BeginGame(); // A simultaneous duplicate request is ignored.
            yield return Wait(session);
            Assert.That(Object.FindObjectsOfType<PlayerHealth>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsOfType<SkeletonController>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectOfType<PlayerHealth>().transform.root,
                Is.Not.EqualTo(Object.FindObjectOfType<SkeletonController>().transform.root), "actor root contract");
            Assert.That(CombatSession.I, Is.Not.Null);
            Assert.That(CombatSession.I.Seconds, Is.EqualTo(0));
            var gate = Object.FindObjectOfType<DungeonSessionBindings>();
            Assert.That(gate.links.Length, Is.GreaterThan(0));
            foreach (var link in gate.links)
            {
                var owner = link.worldOwnsField ? world.Resolve(link.slot) : link.local;
                var field = owner.GetType().GetField(link.field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var actual = owner is Canvas canvas && link.field == "m_Camera" ? canvas.worldCamera : field.GetValue(owner);
                Assert.That(actual, Is.EqualTo(link.worldOwnsField ? link.local : world.Resolve(link.slot)), link.field);
            }
            EndlessRun.Restart();
            session.ReloadGameplay();
            yield return Wait(session);
            Assert.That(session.World, Is.SameAs(world));
            Assert.That(world.environment, Is.SameAs(room));
            Assert.That(world.view, Is.SameAs(camera));
            Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
            Assert.That(world.Arena.PresentationCamera, Is.SameAs(presenter));
            GamePauseController.SetEncounterReading(false);
            var slayer = Object.FindObjectOfType<PlayerSlayerMode>();
            slayer.Activate(10);
            Assert.That(slayer.IsActive, Is.True);
            Assert.That(world.environment.gameObject.activeSelf, Is.False);
            Assert.That(world.Arena.Cinematic, Is.True);
            GamePauseController.SetTutorialReading(true);
            session.ReturnToMenu();
            yield return Wait(session);
            Assert.That(world.environment.gameObject.activeSelf, Is.True);
            Assert.That(world.Arena.Cinematic, Is.False);
            Assert.That(world.Arena.PresentationRect, Is.EqualTo(new Rect(0, 0, 1, 1)));
            Assert.That(Time.timeScale, Is.EqualTo(1));
            Assert.That(AudioListener.pause, Is.False);
            Assert.That(GamePauseController.InputBlocked, Is.False);
            Assert.That(Object.FindObjectsOfType<PlayerHealth>(), Is.Empty);
            Assert.That(Object.FindObjectsOfType<SkeletonController>(), Is.Empty);
            Assert.That(Object.FindObjectsOfType<CombatHUDLayout>(), Is.Empty);
            Assert.That(Object.FindObjectsOfType<ClashLightingRig>(), Is.Empty);
            Assert.That(CombatSession.I, Is.Null);
            Assert.That(world.Arena.player, Is.Null);
            Assert.That(world.Arena.enemy, Is.Null);
            Assert.That(Object.FindObjectsOfType<DungeonArenaCamera>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsOfType<DungeonPresentationCamera>().Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsOfType<AudioListener>().Length, Is.EqualTo(1));
            Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
            Assert.That(world.view, Is.SameAs(camera));
            Assert.That(session.World, Is.SameAs(world));
        }

    }
    static IEnumerator Wait(DungeonSession session)
    {
        float deadline = Time.realtimeSinceStartup + 30;
        while (session.Transitioning && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(session.Transitioning, Is.False, "scene transition completed");
        yield return null;
    }
}
