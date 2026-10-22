using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// A single owner serializes every internal scene transition and retains the world.
public sealed class DungeonSession : MonoBehaviour
{
    public const string WorldScene = "DungeonWorld";
    public const string GameScene = "SampleScene";
    static DungeonSession instance;
    public static DungeonSession Current => instance;
    public static DungeonSession I
    {
        get
        {
            if (!instance)
            {
                var host = new GameObject("Dungeon Session");
                instance = host.AddComponent<DungeonSession>(); DontDestroyOnLoad(host);
            }
            return instance;
        }
    }
    public DungeonWorld World { get; private set; }
    public bool Transitioning { get; private set; }
    public static bool InputSuppressed => instance && (instance.Transitioning || Time.frameCount <= instance.inputFrame);
    int inputFrame = -1;
    DungeonSessionBindings gameplay;

    public IEnumerator LoadWorld()
    {
        if (World) yield break;
        if (Transitioning) { while (Transitioning) yield return null; yield break; }
        Transitioning = true;
        yield return EnsureWorld();
        Transitioning = false;
    }
    IEnumerator EnsureWorld()
    {
        if (World) yield break;
        var scene = SceneManager.GetSceneByName(WorldScene);
        if (!scene.isLoaded) yield return SceneManager.LoadSceneAsync(WorldScene, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByName(WorldScene);
        foreach (var root in scene.GetRootGameObjects())
            if (root.TryGetComponent<DungeonWorld>(out var world)) World = world;
        if (!World) throw new System.InvalidOperationException("DungeonWorld scene has no owner");
        World.ResetView();
    }
    public void BeginGame(string scene = GameScene) => Request(scene);
    public void ReloadGameplay() => Request(GameScene);
    public void ReturnToMenu(string scene = "MainMenu") => Request(scene);
    void Request(string scene)
    {
        if (Transitioning) return;
        Transitioning = true;
        StartCoroutine(Transition(scene));
    }
    IEnumerator Transition(string destination)
    {
        yield return EnsureWorld();
        // Disable first: Slayer restores the environment while its actors still exist.
        if (gameplay) { gameplay.Deactivate(); gameplay.Bind(World, true); gameplay = null; }
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == GameScene || scene.name == "MainMenu")
                yield return SceneManager.UnloadSceneAsync(scene);
        }
        GamePauseController.ResetSessionState();
        World.ResetView();
        yield return SceneManager.LoadSceneAsync(destination, LoadSceneMode.Additive);
        var loaded = SceneManager.GetSceneByName(destination);
        SceneManager.SetActiveScene(loaded);
        World.ApplyEnvironment();
        if (destination == GameScene)
        {
            foreach (var root in loaded.GetRootGameObjects())
                if (root.TryGetComponent<DungeonSessionBindings>(out var bindings)) gameplay = bindings;
            if (!gameplay) throw new System.InvalidOperationException("Gameplay scene has no binding gate");
            gameplay.Bind(World);
            // Consume the launch/Continue key before any controller gets Awake/Start/Update.
            while (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) yield return null;
            inputFrame = Time.frameCount + 1;
            gameplay.gameObject.SetActive(true);
            World.EnableGameplayCamera();
        }
        Transitioning = false;
    }
    void OnDestroy() { if (instance == this) instance = null; }
}
