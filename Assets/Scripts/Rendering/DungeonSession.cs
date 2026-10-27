using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// A single owner serializes every internal scene transition and retains the world.
public sealed class DungeonSession : MonoBehaviour
{
    public WorldDefinition Definition { get; private set; }
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
    public static bool InputSuppressed => instance && (instance.Transitioning || instance.heldInput.Count > 0 || Time.frameCount <= instance.inputFrame);
    readonly System.Collections.Generic.List<KeyCode> heldInput = new System.Collections.Generic.List<KeyCode>();
    internal void ConsumeIntroInput()
    {
        heldInput.Clear();
        for (int key = (int)KeyCode.A; key <= (int)KeyCode.Z; key++)
            if (Input.GetKey((KeyCode)key)) heldInput.Add((KeyCode)key);
        foreach (var key in new[] { KeyCode.Space, KeyCode.Return, KeyCode.KeypadEnter })
            if (Input.GetKey(key)) heldInput.Add(key);
    }
    void Update()
    {
        for (int i = heldInput.Count - 1; i >= 0; i--)
            if (!Input.GetKey(heldInput[i])) heldInput.RemoveAt(i);
    }
    int inputFrame = -1;
    DungeonSessionBindings gameplay;
    DuelIntroPresentation intro;
    ReadyRitualPresentation readyRitual;
    Coroutine transitionRoutine;
    bool shortIntro;
    bool ritualIntro;

    internal void CaptureReadySpawn(SkeletonController enemy)
    {
        if (!readyRitual) readyRitual = gameObject.AddComponent<ReadyRitualPresentation>();
        readyRitual.CaptureSpawn(enemy, World.Arena, World.environment.position.y);
    }
    internal void BeginReady(TypingManager typing, System.Action onReady)
        => readyRitual.Begin(typing, onReady);
    internal void PaintReadySummon(float elapsed)
    {
        if (ritualIntro && readyRitual) readyRitual.PaintSummon(elapsed);
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEditorSession() => instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartEditorGameplay()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name == GameScene && !SceneManager.GetSceneByName("MainMenu").isLoaded)
            I.StartCoroutine(I.WaitForEditorStartup(scene));
    }

    IEnumerator WaitForEditorStartup(Scene scene)
    {
        // With multiple Editor scenes, AfterSceneLoad can precede the menu's load.
        // Let the initial scene set and MainMenu.Start settle before choosing an owner.
        yield return null;
        TryBeginEditorGameplay(scene);
    }

    internal bool TryBeginEditorGameplay(Scene scene)
    {
        if (Transitioning || gameplay || !scene.IsValid() || !scene.isLoaded ||
            scene.name != GameScene || scene != SceneManager.GetActiveScene() ||
            SceneManager.GetSceneByName("MainMenu").isLoaded) return false;
        var gate = FindGameplayGate(scene);
        if (!gate || gate.gameObject.activeSelf) return false;
        Transitioning = true;
        transitionRoutine = StartCoroutine(BeginEditorGameplay(scene));
        return true;
    }

    IEnumerator BeginEditorGameplay(Scene scene)
    {
        TutorialRun.End();
        EndlessRun.Clear();
        EndlessRun.Prepare();
        GamePauseController.ResetSessionState();
        Definition = FindGameplayGate(scene).worldDefinition;
        yield return EnsureWorld();
        World.ResetView();
        SceneManager.SetActiveScene(scene);
        World.ApplyEnvironment();
        // Adopt the live scene: reloading it would discard Inspector edits.
        yield return ActivateGameplay(scene);
        Transitioning = false;
    }
#endif

    public IEnumerator LoadWorld(WorldDefinition definition)
    {
        definition.Validate();
        if (World && Definition == definition) yield break;
        if (gameplay) throw new System.InvalidOperationException("Select a world before entering gameplay");
        if (Transitioning) { while (Transitioning) yield return null; yield return LoadWorld(definition); yield break; }
        Transitioning = true;
        if (World)
        {
            var oldScene = World.gameObject.scene;
            World = null;
            yield return SceneManager.UnloadSceneAsync(oldScene);
        }
        Definition = definition;
        yield return EnsureWorld();
        Transitioning = false;
    }
    IEnumerator EnsureWorld()
    {
        if (World) yield break;
        if (!Definition) throw new System.InvalidOperationException("DungeonSession requires a WorldDefinition");
        var path = Definition.scenePath;
        var scene = SceneManager.GetSceneByPath(path);
        if (!scene.isLoaded) yield return SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByPath(path);
        foreach (var root in scene.GetRootGameObjects())
            if (root.TryGetComponent<DungeonWorld>(out var world)) World = world;
        if (!World) throw new System.InvalidOperationException("DungeonWorld scene has no owner");
        if (World.definition != Definition) throw new System.InvalidOperationException("World scene/definition mismatch");
        World.ResetView();
    }
    public void BeginGame(string scene = GameScene) => Request(scene);
    public void ReloadGameplay() => Request(GameScene, true);
    public void ReturnToMenu(string scene = "MainMenu") => Request(scene);
    void Request(string scene, bool reload = false)
    {
        if (Transitioning)
        {
            // Only explicit navigation may cancel a live intro; duplicate Play is ignored.
            if (!intro || !intro.Running || (!reload && scene == GameScene)) return;
            if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        }
        GamePauseController.SetIntro(true);
        shortIntro = reload;
        ritualIntro = reload && readyRitual && readyRitual.Committed;
        if (readyRitual && !ritualIntro) readyRitual.Cancel();
        Transitioning = true;
        transitionRoutine = StartCoroutine(Transition(scene));
    }
    IEnumerator Transition(string destination)
    {
        if (!intro) intro = gameObject.AddComponent<DuelIntroPresentation>();
        yield return intro.Cover(shortIntro ? .2f : .25f, !shortIntro && destination == GameScene, ritualIntro);
        intro.ClearCovered();
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
        if (ritualIntro) readyRitual.BeginSummon();
        yield return SceneManager.LoadSceneAsync(destination, LoadSceneMode.Additive);
        var loaded = SceneManager.GetSceneByName(destination);
        SceneManager.SetActiveScene(loaded);
        World.ApplyEnvironment();
        if (destination == GameScene)
            yield return ActivateGameplay(loaded);
        if (destination != GameScene) yield return intro.Uncover(.2f);
        Transitioning = false;
    }

    static DungeonSessionBindings FindGameplayGate(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
            if (root.TryGetComponent<DungeonSessionBindings>(out var bindings)) return bindings;
        return null;
    }

    IEnumerator ActivateGameplay(Scene scene)
    {
        gameplay = FindGameplayGate(scene);
        if (!gameplay) throw new System.InvalidOperationException("Gameplay scene has no binding gate");
        gameplay.Bind(World);
        if (!intro) intro = gameObject.AddComponent<DuelIntroPresentation>();
        // Lock before Awake/OnEnable/Start, while preserving animation and VFX clocks.
        GamePauseController.SetIntro(true);
        intro.Prepare(gameplay, TutorialRun.Active);
        inputFrame = Time.frameCount + 1;
        gameplay.gameObject.SetActive(true);
        World.EnableGameplayCamera();
        yield return intro.Play(shortIntro, TutorialRun.Active);
        inputFrame = Time.frameCount + 1;
    }
    void OnDestroy()
    {
        if (readyRitual) readyRitual.Cancel();
        if (intro) intro.Cancel();
        if (instance == this) instance = null;
    }
}
