using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(100)]
public sealed class TutorialDirector : MonoBehaviour
{
    [SerializeField] TutorialOverlay overlay;
    GamePauseController pause;
    bool initialized, starting, parryStep, waitForEnterRelease;
    int changedFrame = -1;
    bool ControlsBlocked => DungeonSession.InputSuppressed || !initialized || starting ||
        GamePauseController.IsPaused || LeaderboardModal.IsOpen || (pause && pause.IsRestarting);

    void Start()
    {
        if (!TutorialRun.Active) { overlay.gameObject.SetActive(false); enabled = false; return; }
        initialized = true;
        pause = FindObjectOfType<GamePauseController>();
        GamePauseController.SetTutorialReading(true);
        overlay.BindBack(Back);
        overlay.ShowStep(false);
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter)) waitForEnterRelease = false;
        if (!ControlsBlocked && !waitForEnterRelease && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            overlay.SubmitContinue();
    }

    public void Continue()
    {
        if (ControlsBlocked || changedFrame == Time.frameCount || waitForEnterRelease) return;
        if (!parryStep) { ChangeStep(true); return; }
        starting = true;
        TutorialRun.Complete();
        TutorialRun.End();
        // DungeonSession consumes held Enter before activating the fresh encounter.
        // Keep the reading lock until that session tears down this scene.
        DungeonSession.I.ReloadGameplay();
    }

    void Back()
    {
        if (!ControlsBlocked && changedFrame != Time.frameCount) ChangeStep(false);
    }

    void ChangeStep(bool parry)
    {
        parryStep = parry;
        changedFrame = Time.frameCount;
        waitForEnterRelease = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        overlay.ShowStep(parry);
    }

    void OnDestroy()
    {
        if (initialized) GamePauseController.SetTutorialReading(false);
    }
}
