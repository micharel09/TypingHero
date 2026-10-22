using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent, DefaultExecutionOrder(100)]
public sealed class TutorialDirector : MonoBehaviour
{
    enum Lesson { Attack, Accuracy, Parry, Stamina, Slayer, Combine }
    enum Phase { Reading, Practice, Passed }
    static readonly string[] Titles = { "ATTACK", "ACCURACY", "PARRY", "STAMINA", "SLAYER", "COMBINE" };
    static readonly string[] Instructions = {
        "Type BLADE to attack.",
        "Mistakes reset the word. Finish before time runs out.",
        "Press SPACE when the yellow cue appears.",
        "One more parry will break its stamina.",
        "SLAYER! Each new letter strikes. Type FIRE.",
        "Type to attack. Watch the cue. Parry."
    };
    [SerializeField] TutorialOverlay overlay;
    TypingManager typing;
    SkeletonController enemy;
    PlayerHealth player;
    ParrySystem parry;
    PlayerSlayerMode slayer;
    EnemyStunController stun;
    EnemyStamina stamina;
    GamePauseController pause;
    Lesson lesson;
    Phase phase;
    int words, parries, slayerStartHealth;
    bool timedOut, reloading, initialized;
    internal int LessonIndex => (int)lesson;
    internal bool Practicing => phase == Phase.Practice;
    internal bool Passed => phase == Phase.Passed;
    bool ControlsBlocked => DungeonSession.InputSuppressed || !initialized || reloading || GamePauseController.IsPaused ||
        LeaderboardModal.IsOpen || (pause && pause.IsRestarting);

    void Start()
    {
        if (!TutorialRun.Active) { overlay.gameObject.SetActive(false); enabled = false; return; }
        initialized = true;
        typing = FindObjectOfType<TypingManager>();
        enemy = FindObjectOfType<SkeletonController>();
        player = FindObjectOfType<PlayerHealth>();
        parry = FindObjectOfType<ParrySystem>();
        slayer = FindObjectOfType<PlayerSlayerMode>();
        stun = enemy.GetComponent<EnemyStunController>();
        stamina = enemy.GetComponent<EnemyStamina>();
        pause = FindObjectOfType<GamePauseController>();
        lesson = (Lesson)TutorialRun.Checkpoint;
        enemy.HoldPracticeAttacks(lesson == Lesson.Attack || lesson == Lesson.Accuracy);
        if (lesson == Lesson.Stamina) stamina.PreparePractice(25);
        SetWords();
        TypingManager.OnWordAdvanced += Word;
        TypingManager.OnWordTimeout += Timeout;
        parry.OnParrySuccess += Parried;
        GamePauseController.SetTutorialReading(true);
        Render();
    }

    void SetWords()
    {
        bool timed = lesson == Lesson.Accuracy || lesson == Lesson.Combine;
        bool acceptsLetters = lesson != Lesson.Parry && lesson != Lesson.Stamina;
        typing.SetPractice(lesson == Lesson.Slayer ? new[] { "FIRE" } : new[] { "BLADE", "STEEL", "FIRE" }, timed, acceptsLetters);
    }

    void Word() { if (Practicing) { words++; Render(); } }
    void Timeout() { if (Practicing) timedOut = true; }
    void Parried(ParrySystem.ParryContext context)
    {
        if (!Practicing || context.targetRoot != enemy.transform.root) return;
        parries++;
        Render();
    }

    void Update()
    {
        if (ControlsBlocked) return;
        if (!Practicing && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            overlay.SubmitContinue();
        if (!Practicing || reloading) return;
        if (player.Current < player.maxHealth || timedOut)
        {
            Reload(lesson == Lesson.Slayer ? Lesson.Stamina : lesson, true);
            return;
        }
        bool passed = false;
        switch (lesson)
        {
            case Lesson.Attack: passed = words >= 1 && enemy.Current < enemy.maxHealth; break;
            case Lesson.Accuracy: passed = words >= 2; break;
            case Lesson.Parry: passed = parries >= 2; break;
            case Lesson.Stamina:
                if (parries >= 1 && stun.IsStunned && slayer.IsActive && stamina.Current == 0) BeginSlayer();
                return;
            case Lesson.Slayer:
                passed = words >= 1 && slayer.IsActive && enemy.Current < slayerStartHealth;
                if (!passed && !slayer.IsActive) { Reload(Lesson.Stamina, true); return; }
                break;
            case Lesson.Combine: passed = words >= 2 && parries >= 1; break;
        }
        if (!passed) return;
        phase = Phase.Passed;
        GamePauseController.SetTutorialReading(true);
        if (lesson == Lesson.Combine) TutorialRun.Complete();
        Render();
    }

    void BeginSlayer()
    {
        // Preserve the live stun; restart must reconstruct it by parrying again.
        lesson = Lesson.Slayer;
        phase = Phase.Reading;
        words = parries = 0;
        slayerStartHealth = enemy.Current;
        SetWords();
        GamePauseController.SetTutorialReading(true);
        Render();
    }

    void Render()
    {
        string progress = lesson == Lesson.Accuracy ? $" · {Mathf.Min(words, 2)}/2 WORDS" :
            lesson == Lesson.Parry ? $" · {Mathf.Min(parries, 2)}/2 PARRIES" :
            lesson == Lesson.Stamina ? $" · {stamina.Current}/{stamina.max}" :
            lesson == Lesson.Combine ? $" · {Mathf.Min(words, 2)}/2 WORDS  {Mathf.Min(parries, 1)}/1 PARRY" : "";
        string status = Passed ? " · COMPLETE" : TutorialRun.Retrying ? " · TRY AGAIN" : "";
        string action = Practicing ? "" : Passed ? (lesson == Lesson.Combine ? "Start Battle" : "Next · Enter") : "Practice · Enter";
        var focus = lesson == Lesson.Stamina ? TutorialOverlay.Focus.Stamina :
            lesson == Lesson.Parry ? TutorialOverlay.Focus.Cue :
            lesson == Lesson.Combine ? TutorialOverlay.Focus.Combine : TutorialOverlay.Focus.Typing;
        overlay.Show($"{(int)lesson + 1} / 6 · {Titles[(int)lesson]}{progress}{status}", Instructions[(int)lesson], action, focus);
    }

    public void Continue()
    {
        if (ControlsBlocked || Practicing) return;
        if (Passed)
        {
            if (lesson == Lesson.Combine) StartBattle();
            else Reload((Lesson)((int)lesson + 1), false);
            return;
        }
        TutorialRun.Retrying = false;
        phase = Phase.Practice;
        Render();
        GamePauseController.SetTutorialReading(false);
    }

    void Reload(Lesson checkpoint, bool retry)
    {
        reloading = true;
        TutorialRun.Checkpoint = (int)checkpoint;
        TutorialRun.Retrying = retry;
        GamePauseController.SetTutorialReading(true);
        DungeonSession.I.ReloadGameplay();
    }

    public void Skip()
    {
        if (ControlsBlocked) return;
        TutorialRun.Skip();
        StartBattle();
    }

    void StartBattle()
    {
        reloading = true;
        GamePauseController.SetTutorialReading(false);
        TutorialRun.End();
        DungeonSession.I.ReloadGameplay();
    }

    void OnDestroy()
    {
        TypingManager.OnWordAdvanced -= Word;
        TypingManager.OnWordTimeout -= Timeout;
        if (parry) parry.OnParrySuccess -= Parried;
        if (initialized) GamePauseController.SetTutorialReading(false);
    }
}
