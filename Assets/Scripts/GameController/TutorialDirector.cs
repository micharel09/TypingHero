using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(100)]
public sealed class TutorialDirector : MonoBehaviour
{
    [SerializeField] TutorialOverlay overlay;
    GamePauseController pause;
    bool initialized, starting;
    bool ControlsBlocked => DungeonSession.InputSuppressed || !initialized || starting ||
        GamePauseController.IsPaused || LeaderboardModal.IsOpen || (pause && pause.IsRestarting);

    void Start()
    {
        if (!TutorialRun.Active) { overlay.gameObject.SetActive(false); enabled = false; return; }
        initialized = true;
        pause = FindObjectOfType<GamePauseController>();
        GamePauseController.SetTutorialReading(true);
        overlay.ClearSelection();
    }

    void Update()
    {
        if (!ControlsBlocked && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            overlay.SubmitContinue();
    }

    public void Continue()
    {
        if (ControlsBlocked) return;
        starting = true;
        TutorialRun.Complete();
        TutorialRun.End();
        // DungeonSession consumes held Enter before activating the fresh encounter.
        // Keep the reading lock until that session tears down this scene.
        DungeonSession.I.ReloadGameplay();
    }

    void OnDestroy()
    {
        if (initialized) GamePauseController.SetTutorialReading(false);
    }
}
