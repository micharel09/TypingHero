using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(100)]
public sealed class TutorialDirector : MonoBehaviour
{
    [SerializeField] TutorialOverlay overlay;
    GamePauseController pause;
    TutorialPractice practice;
    bool initialized, starting, waitForEnterRelease, practicing;
    readonly bool[] passed = new bool[2];
    int step, failures, intro;
    int changedFrame = -1;
    bool Core => step == 1 || step == 3;
    int Skill => step == 1 ? 0 : 1;
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
        practice = gameObject.AddComponent<TutorialPractice>();
        practice.Finished += PracticeFinished;
        GamePauseController.SetTutorialReading(true);
        overlay.BindBack(Back);
        overlay.BindLearning(Watch, Skip);
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
        if (ControlsBlocked || changedFrame == Time.frameCount || waitForEnterRelease || practicing) return;
        if (step == 0 && intro < 2) { overlay.ShowIntroFocus(++intro); MarkChanged(); return; }
        if (Core && !passed[Skill])
        {
            practicing = true;
            overlay.ShowPractice(step, step == 1 ? "Type HIT to attack." : "Tap Space when the key fills.", false);
            MarkChanged();
            practice.Begin(step == 3);
            return;
        }
        if (step < overlay.StepCount - 1) { ChangeStep(step + 1); return; }
        if (passed[0] && passed[1]) StartBattle(false);
    }

    void PracticeFinished(bool success, string message)
    {
        if (!practicing || starting) return;
        practicing = false;
        if (success) passed[Skill] = true;
        else failures++;
        if (!success && step == 3 && failures >= 3) message = "Not parried. Wait for the filled key.\nRelease Space before each retry.";
        overlay.ShowPractice(step, message, true);
        overlay.SetPrimary(success ? "Next \u00b7 Enter" : "Retry \u00b7 Enter", true);
        MarkChanged();
    }

    void Watch()
    {
        if (ControlsBlocked || practicing || step == 0) return;
        practice.Cancel();
        GamePauseController.SetTutorialReading(true);
        overlay.ShowStep(step);
        overlay.SetReadiness(passed[0], passed[1]);
        UpdatePrimary();
        overlay.PlayDemo();
        MarkChanged();
    }

    void Skip()
    {
        if (!ControlsBlocked && changedFrame != Time.frameCount) StartBattle(true);
    }

    void StartBattle(bool skipped)
    {
        starting = true;
        practice.Cancel();
        GamePauseController.SetTutorialReading(true);
        overlay.ReleaseClip();
        if (skipped) TutorialRun.Skip(); else TutorialRun.Complete();
        TutorialRun.End();
        EndlessRun.Prepare();
        DungeonSession.I.ReloadGameplay();
    }

    void Back()
    {
        if (!ControlsBlocked && changedFrame != Time.frameCount && step > 0) ChangeStep(step - 1);
    }

    void MarkChanged()
    {
        changedFrame = Time.frameCount;
        waitForEnterRelease = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        overlay.ClearSelection();
    }

    void ChangeStep(int next)
    {
        practice.Cancel();
        GamePauseController.SetTutorialReading(true);
        step = next; practicing = false; failures = intro = 0;
        overlay.ShowStep(step);
        overlay.SetReadiness(passed[0], passed[1]);
        UpdatePrimary();
        MarkChanged();
    }

    void UpdatePrimary() => overlay.SetPrimary(Core && !passed[Skill] ? "Try it \u00b7 Enter" :
        step == 5 ? "Start Battle \u00b7 Enter" : "Next \u00b7 Enter", step != 5 || (passed[0] && passed[1]));

    void OnDisable()
    {
        if (practice) practice.Cancel();
        practicing = false;
        if (initialized) GamePauseController.SetTutorialReading(true);
    }
    void OverlayUnavailable()
    {
        if (!initialized || starting) return;
        practice.Cancel();
        practicing = false;
        step = intro = failures = 0;
        GamePauseController.SetTutorialReading(true);
        MarkChanged();
    }
    void OnDestroy()
    {
        if (overlay) overlay.Unavailable -= OverlayUnavailable;
        if (practice) { practice.Finished -= PracticeFinished; practice.Cancel(); }
        if (initialized) GamePauseController.SetTutorialReading(false);
    }
}
