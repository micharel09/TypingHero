using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static class DungeonSceneTestFlow
{
    internal static IEnumerator Start()
    {
        yield return DungeonSession.I.LoadWorld(LightingTestAssets.Definition);
        DungeonSession.I.BeginGame();
        yield return Wait();
    }
    internal static IEnumerator Wait()
    {
        float deadline = Time.realtimeSinceStartup + 30;
        while (DungeonSession.I.Transitioning && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(DungeonSession.I.Transitioning, Is.False);
        yield return null; yield return null;
    }
    internal static void Cleanup()
    {
        var session = DungeonSession.I;
        var world = SceneManager.GetSceneByName(LightingTestAssets.WorldScene);
        if (world.isLoaded) SceneManager.UnloadSceneAsync(world);
        Object.Destroy(session.gameObject);
    }
}
