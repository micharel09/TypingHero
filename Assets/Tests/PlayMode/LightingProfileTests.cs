using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class LightingProfileTests : IPrebuildSetup, IPostBuildCleanup
{
    const string OtherScene = "Assets/Tests/Fixtures/Lighting/OtherWorld.unity";
    [System.Serializable] sealed class SceneList { public string[] paths; public bool[] enabled; }
    const string SceneBackup = "CLACKBLADE.LightingTests.SceneList";
    void IPrebuildSetup.Setup()
    {
        var scenes = UnityEditor.EditorBuildSettings.scenes;
        UnityEditor.SessionState.SetString(SceneBackup, JsonUtility.ToJson(new SceneList {
            paths = scenes.Select(x => x.path).ToArray(), enabled = scenes.Select(x => x.enabled).ToArray()
        }));
        UnityEditor.EditorBuildSettings.scenes = scenes.Where(x => x.path != OtherScene)
            .Concat(new[] { new UnityEditor.EditorBuildSettingsScene(OtherScene, true) }).ToArray();
    }
    void IPostBuildCleanup.Cleanup()
    {
        var json = UnityEditor.SessionState.GetString(SceneBackup, "");
        if (string.IsNullOrEmpty(json)) return;
        var saved = JsonUtility.FromJson<SceneList>(json);
        UnityEditor.EditorBuildSettings.scenes = saved.paths.Select((path, i) => new UnityEditor.EditorBuildSettingsScene(path, saved.enabled[i])).ToArray();
        UnityEditor.SessionState.EraseString(SceneBackup);
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        var session = DungeonSession.Current;
        if (session && session.World) yield return SceneManager.UnloadSceneAsync(session.World.gameObject.scene);
        if (session) Object.Destroy(session.gameObject);
        GamePauseController.ResetSessionState(); LightingTestAssets.Cleanup();
        yield return null;
    }
    [Test, Category("Quick")]
    public void IrregularAtlasFramesKeepTheirOwnDurationsAndRejectInvalidData()
    {
        var profile = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<FlameProfile>("Assets/Data/Lighting/WarmFlame.asset"));
        try
        {
            profile.frames = new[] {
                new FlameProfile.Frame { uv = new Rect(.5f, .25f, .25f, .25f), seconds = .125f },
                new FlameProfile.Frame { uv = new Rect(0, .5f, .25f, .25f), seconds = .25f },
                new FlameProfile.Frame { uv = new Rect(.75f, 0, .25f, .25f), seconds = .5f }
            };
            profile.Validate();
            foreach (var sample in new[] { (0f, 0), (.124f, 0), (.125f, 1), (.374f, 1), (.375f, 2), (.874f, 2), (.875f, 0), (1f, 1) })
                Assert.That(profile.FrameAt(sample.Item1), Is.EqualTo(sample.Item2));
            profile.frames[1].seconds = 0; Assert.Throws<System.InvalidOperationException>(() => profile.Validate());
        }
        finally { Object.DestroyImmediate(profile); }
    }
    [UnityTest, Category("Quick")]
    public IEnumerator DifferentWorldNamesAndFourLightsReuseProfilesWithoutLeakingState()
    {
        var first = LightingTestAssets.Definition;
        var second = UnityEditor.AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Tests/Fixtures/Lighting/OtherWorld.asset");
        Assert.That(second, Is.Not.Null);
        string a = JsonUtility.ToJson(first.lighting), b = JsonUtility.ToJson(second.lighting);
        var session = DungeonSession.I;
        yield return session.LoadWorld(first);
        var previousWorld = session.World; var previousRT = previousWorld.Arena.WorldTexture;
        Assert.That(previousWorld.environment.GetComponentsInChildren<DungeonTorchLight>().Length, Is.EqualTo(3));
        yield return session.LoadWorld(second); yield return null;
        Assert.That(previousWorld == null, Is.True); Assert.That(previousRT == null, Is.True);
        var world = session.World;
        Assert.That(world.Profile, Is.SameAs(second.lighting));
        Assert.That(world.Arena.WorldTexture.width, Is.EqualTo(256));
        Assert.That(RenderSettings.fogColor, Is.EqualTo(second.lighting.fogColor));
        var torches = world.environment.GetComponentsInChildren<DungeonTorchLight>();
        Assert.That(torches.Length, Is.EqualTo(4));
        Assert.That(torches.Any(x => x.profile.intensity == 12), Is.True, "new fixture style comes from an independent asset");
        Assert.That(torches.Select(x => x.name), Does.Not.Contain("TorchGlow_Front"));
        var filters = torches.Select(x => x.Flame.sharedMesh).ToArray();
        Assert.That(filters.Distinct().Count(), Is.EqualTo(4), "shared style must not share mutable UV state");
        foreach (var torch in torches)
        {
            Assert.That(torch.GetComponent<Light>().intensity, Is.InRange(torch.profile.intensity * (1 - torch.profile.amplitude), torch.profile.intensity * (1 + torch.profile.amplitude)));
            Assert.That(torch.Flame.sharedMesh, Is.Not.SameAs(torch.profile.flame.mesh));
        }
        yield return session.LoadWorld(first); yield return null;
        foreach (var filter in filters) Assert.That(filter == null, Is.True);
        Assert.That(session.World.Arena.WorldTexture.width, Is.EqualTo(320));
        Assert.That(RenderSettings.fogColor, Is.EqualTo(first.lighting.fogColor));
        Assert.That(JsonUtility.ToJson(first.lighting), Is.EqualTo(a));
        Assert.That(JsonUtility.ToJson(second.lighting), Is.EqualTo(b));
    }
}
