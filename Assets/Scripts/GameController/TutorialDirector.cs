using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(100)]
public sealed class TutorialDirector : MonoBehaviour
{
    [SerializeField] TutorialOverlay overlay;
    GamePauseController pause;
    bool initialized, starting, waitForEnterRelease;
    int step;
    int changedFrame = -1;
    bool ControlsBlocked => DungeonSession.InputSuppressed || !initialized || starting ||
        GamePauseController.IsPaused || LeaderboardModal.IsOpen || (pause && pause.IsRestarting);

    void OnEnable()
    {
        if (initialized && !starting && TutorialRun.Active) ChangeStep(0);
    }

    void Start()
    {
        if (!TutorialRun.Active) { overlay.gameObject.SetActive(false); enabled = false; return; }
        initialized = true;
        pause = FindObjectOfType<GamePauseController>();
        GamePauseController.SetTutorialReading(true);
        overlay.BindLearning(null, Skip);
        overlay.Unavailable += OverlayUnavailable;
        ChangeStep(0);
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
        if (step < overlay.StepCount - 1) ChangeStep(step + 1);
        else StartBattle(false);
    }

    void Skip()
    {
        if (!ControlsBlocked && changedFrame != Time.frameCount) StartBattle(true);
    }

    void StartBattle(bool skipped)
    {
        starting = true;
        GamePauseController.SetTutorialReading(true);
        overlay.ReleaseClip();
        if (skipped) TutorialRun.Skip(); else TutorialRun.Complete();
        TutorialRun.End();
        EndlessRun.Prepare();
        DungeonSession.I.ReloadGameplay();
    }

    void MarkChanged()
    {
        changedFrame = Time.frameCount;
        waitForEnterRelease = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        overlay.ClearSelection();
    }

    void ChangeStep(int next)
    {
        GamePauseController.SetTutorialReading(true);
        step = next;
        overlay.ShowStep(step);
        overlay.SetPrimary(step == overlay.StepCount - 1 ? "Start Battle" : "Next", true);
        MarkChanged();
    }

    void OnDisable()
    {
        if (initialized) GamePauseController.SetTutorialReading(true);
    }
    void OverlayUnavailable()
    {
        if (!initialized || starting) return;
        step = 0;
        GamePauseController.SetTutorialReading(true);
        MarkChanged();
    }
    void OnDestroy()
    {
        if (overlay) overlay.Unavailable -= OverlayUnavailable;
        if (initialized) GamePauseController.SetTutorialReading(false);
    }
}
