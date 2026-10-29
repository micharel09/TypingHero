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
            Assert.That(GamePauseController.InputBlocked, Is.False, "intro and its handoff frame completed");
            var gate = Object.FindObjectOfType<DungeonSessionBindings>();
            Assert.That(gate.links.Length, Is.GreaterThan(0));
            foreach (var link in gate.links)
            {
                var owner = link.worldOwnsField ? world.Resolve(link.slot) : link.local;
                var field = owner.GetType().GetField(link.field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var actual = owner is Canvas canvas && link.field == "m_Camera" ? canvas.worldCamera : field.GetValue(owner);
                Assert.That(actual, Is.EqualTo(link.worldOwnsField ? link.local : world.Resolve(link.slot)), link.field);
            }
            var priorImpact = Object.FindObjectOfType<CombatCameraImpact>();
            Assert.That(priorImpact, Is.Not.Null);
            GamePauseController.SetEncounterReading(false);
            priorImpact.Hit(Vector3.zero, Vector3.right, false, false); priorImpact.Advance(.03f);
            Assert.That(priorImpact.Zoom, Is.EqualTo(.02f).Within(.00001f));
            EndlessRun.Restart();
            session.ReloadGameplay();
            yield return Wait(session);
            Assert.That(session.World, Is.SameAs(world));
            Assert.That(world.environment, Is.SameAs(room));
            Assert.That(world.view, Is.SameAs(camera));
            Assert.That(world.Arena.WorldTexture, Is.SameAs(texture));
            Assert.That(world.Arena.PresentationCamera, Is.SameAs(presenter));
            var impact = Object.FindObjectOfType<CombatCameraImpact>();
            Assert.That(impact, Is.Not.Null);
            Assert.That(impact.Zoom, Is.Zero);
            var listeners = impact.GetComponents<Cinemachine.CinemachineImpulseListener>();
            Assert.That(listeners.Length, Is.EqualTo(1), "only the unrelated legacy channel remains");
            Assert.That(listeners[0].m_ChannelMask, Is.EqualTo(1));
            Assert.That(listeners[0].m_Gain, Is.EqualTo(10));
            Assert.That(impact.GetComponents<CameraImpactExtension>().Length, Is.EqualTo(1), "one direct combat Noise extension");
            GamePauseController.SetEncounterReading(false);
            impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
            Assert.That(impact.Zoom, Is.EqualTo(.02f).Within(.00001f));
            var slayer = Object.FindObjectOfType<PlayerSlayerMode>();
            var virtualCamera = Object.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            var pause = Object.FindObjectOfType<GamePauseController>();
            var fx = Object.FindObjectOfType<SlayerScreenFX>();
            var fixtures = room.GetComponentsInChildren<DungeonTorchLight>();
            Assert.That(fixtures.Length, Is.EqualTo(5), "baseline fixtures");
            var profiles = System.Array.ConvertAll(fixtures, f => f.profile);
            var owners = System.Array.ConvertAll(fixtures, f => f.GetInstanceID());
            var colors = System.Array.ConvertAll(fixtures, f => f.GetComponent<Light>().color);
            var shadows = System.Array.ConvertAll(fixtures, f => f.GetComponent<Light>().shadows);
            var ranges = System.Array.ConvertAll(fixtures, f => f.GetComponent<Light>().range);
            var volume = room.GetComponent<UnityEngine.Rendering.Volume>();
            float volumeBaseline = volume ? volume.weight : 0;
            float normalLens = virtualCamera.m_Lens.OrthographicSize;
            float entry = Read<float>(fx, "enterDuration"), exit = Read<float>(fx, "exitDuration");
            Assert.That(SlayerScreenFX.PresentationWeight, Is.Zero);
            Assert.That(world.Arena.SlayerBackground.a, Is.Zero);
            Assert.That(Read<float>(world.Arena, "<EnvironmentPresentationGain>k__BackingField"), Is.EqualTo(1));
            TestContext.WriteLine($"cycle={cycle}: {(cycle == 2 ? "NATURAL_EXPIRY" : "MANUAL_SCREEN_FX_EXIT")}, entry={entry:R}, exit={exit:R}, baselineLens={normalLens:R}, volume={volumeBaseline:R}");
            slayer.Activate(cycle == 2 ? 2f : 10f);
            Assert.That(slayer.IsActive, Is.True);
            Assert.That(world.environment.gameObject.activeSelf, Is.True);
            if (cycle == 2)
            {
                yield return Until(() => SlayerScreenFX.PresentationWeight > 0, entry + .25f, "entry started");
                Assert.That(SlayerScreenFX.PresentationWeight, Is.LessThan(1), "pause inside entry");
                yield return HoldPresentation(pause, virtualCamera);
                yield return Until(() => SlayerScreenFX.PresentationWeight == 1, entry + .25f, "entry completed");
                yield return Until(() => !slayer.IsActive, 2f + .25f, "natural timer expiry");
                yield return Until(() => SlayerScreenFX.PresentationWeight < 1, exit + .25f, "exit started");
                Assert.That(SlayerScreenFX.PresentationWeight, Is.GreaterThan(0), "pause inside exit");
                yield return HoldPresentation(pause, virtualCamera);
                yield return Until(() => SlayerScreenFX.PresentationWeight == 0, exit + .25f, "natural exit completed");
                Assert.That(slayer.IsActive, Is.False);
                Assert.That(Shader.GetGlobalFloat("_DungeonSlayerWeight"), Is.Zero);
                Assert.That(world.Arena.SlayerBackground.a, Is.Zero);
                Assert.That(Read<float>(world.Arena, "<EnvironmentPresentationGain>k__BackingField"), Is.EqualTo(1));
                Assert.That(virtualCamera.m_Lens.OrthographicSize, Is.EqualTo(world.Arena.profile.normalOrtho).Within(.00001f));
                if (volume) Assert.That(volume.weight, Is.EqualTo(volumeBaseline));
                TestContext.WriteLine($"NATURAL_RESTORATION before cleanup: active={slayer.IsActive}, presentation={SlayerScreenFX.PresentationWeight:R}, global={Shader.GetGlobalFloat("_DungeonSlayerWeight"):R}, alpha={world.Arena.SlayerBackground.a:R}, lens={virtualCamera.m_Lens.OrthographicSize:R}, volume={(volume ? volume.weight : 0):R}");
            }
            else
            {
                yield return HoldPresentation(pause, virtualCamera);
                yield return new WaitForSecondsRealtime(entry + .25f);
                fx.ExitSlayerFX();
                GamePauseController.SetEncounterReading(true);
                yield return Until(() => SlayerScreenFX.PresentationWeight == 0, exit + .25f, "manual presentation exit");
                Assert.That(virtualCamera.m_Lens.OrthographicSize, Is.EqualTo(normalLens).Within(.00001f));
            }
            var restoredLights = room.GetComponentsInChildren<DungeonTorchLight>();
            Assert.That(restoredLights.Length, Is.EqualTo(5), "Slayer exit retains all five authored fixtures");
            for (int i = 0; i < restoredLights.Length; i++)
            {
                var fixture = restoredLights[i];
                Assert.That(fixture.GetInstanceID(), Is.EqualTo(owners[i]), "fixture owner");
                Assert.That(fixture.profile, Is.SameAs(profiles[i]), "fixture profile");
                Assert.That(fixture.GetComponent<Light>().color, Is.EqualTo(colors[i]), fixture.name);
                Assert.That(fixture.GetComponent<Light>().shadows, Is.EqualTo(shadows[i]), fixture.name);
                Assert.That(fixture.GetComponent<Light>().range, Is.EqualTo(ranges[i]), fixture.name);
                var light = fixture.GetComponent<Light>();
                Assert.That(light.enabled, Is.True, fixture.name);
                Assert.That(light.range, Is.EqualTo(fixture.profile.range), fixture.name);
                Assert.That(light.intensity, Is.InRange(fixture.profile.intensity * (1 - fixture.profile.amplitude), fixture.profile.intensity * (1 + fixture.profile.amplitude)), fixture.name);
                TestContext.WriteLine($"Slayer exit cycle={cycle}, modeActive={slayer.IsActive}, {fixture.name}: profile={fixture.profile.name}, intensity={light.intensity:R}, range={light.range:R}");
            }
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
            Assert.That(Object.FindObjectsOfType<CombatCameraImpact>(), Is.Empty);
            Assert.That(world.Arena.ImpactZoom, Is.Zero);
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
    static T Read<T>(object owner, string field) => (T)owner.GetType()
        .GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(owner);
    static IEnumerator Until(System.Func<bool> condition, float seconds, string phase)
    {
        float deadline = Time.realtimeSinceStartup + seconds;
        while (!condition() && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(condition(), Is.True, phase);
    }
    static IEnumerator HoldPresentation(GamePauseController pause, Cinemachine.CinemachineVirtualCamera camera)
    {
        float lens = camera.m_Lens.OrthographicSize, weight = SlayerScreenFX.PresentationWeight;
        pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.12f);
        Assert.That(camera.m_Lens.OrthographicSize, Is.EqualTo(lens).Within(.00001f), "paused lens");
        Assert.That(SlayerScreenFX.PresentationWeight, Is.EqualTo(weight), "paused presentation");
        pause.Resume();
        Assert.That(GamePauseController.IsPaused, Is.False);
    }
    static IEnumerator Wait(DungeonSession session)
    {
        float deadline = Time.realtimeSinceStartup + 30;
        while (session.Transitioning && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(session.Transitioning, Is.False, "scene transition completed");
        yield return null; yield return null;
    }
}
