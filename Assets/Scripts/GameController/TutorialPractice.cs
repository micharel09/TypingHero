using System;
using UnityEngine;

// Owns only the current tutorial attempt; combat still resolves through authored events.
[DefaultExecutionOrder(1200)]
public sealed class TutorialPractice : MonoBehaviour
{
    internal event Action<bool, string> Finished;
    readonly string[] words = { "HIT" };
    TypingManager typing;
    SkeletonController enemy;
    ParrySystem parry;
    PlayerController player;
    bool running, wordAccepted, succeeded, attackStarted;
    float leadIn;
    int initialHealth;

    void Awake()
    {
        typing = FindObjectOfType<TypingManager>();
        enemy = FindObjectOfType<SkeletonController>();
        parry = FindObjectOfType<ParrySystem>();
        player = FindObjectOfType<PlayerController>();
        TypingManager.OnWordCorrect += WordCompleted;
        parry.OnParrySuccess += Parried;
        enemy.HoldPracticeAttacks(true);
    }

    internal void Begin(bool isParry)
    {
        Cancel();
        enemy.ConfigureEndless(EndlessRun.ForPractice(EndlessDifficulties.Selected));
        enemy.HoldPracticeAttacks(true);
        FindObjectOfType<PlayerHealth>().FullRestore();
        TutorialRun.Practicing = isParry ? TutorialRun.Practice.Parry : TutorialRun.Practice.Typing;
        typing.SetPractice(words, false, !isParry);
        initialHealth = enemy.Current;
        running = true;
        leadIn = 1;
        GamePauseController.SetTutorialReading(false);
    }

    void WordCompleted()
    {
        if (!running || TutorialRun.Practicing != TutorialRun.Practice.Typing) return;
        wordAccepted = true;
        typing.StopPracticeInput();
    }

    void Parried(ParrySystem.ParryContext context)
    {
        if (running && attackStarted && TutorialRun.Practicing == TutorialRun.Practice.Parry) succeeded = true;
    }

    void LateUpdate()
    {
        if (!running || GamePauseController.InputBlocked) return;
        if (TutorialRun.Practicing == TutorialRun.Practice.Typing)
        {
            if (wordAccepted && enemy.Current < initialHealth && !PlayerBusy()) End(true, "Hit landed.");
            return;
        }
        if (!attackStarted)
        {
            // The first attack cannot consume a Space already held when Try/Retry was clicked.
            if (Input.GetKey(KeyCode.Space)) return;
            leadIn -= Time.deltaTime;
            if (leadIn <= 0) attackStarted = enemy.StartPracticeAttack();
            return;
        }
        if (!enemy.IsAttacking && !parry.PoseLocked)
            End(succeeded, succeeded ? "Parry successful - Guard reduced." :
                "Not parried. Watch the key fill, then tap Space.");
    }

    bool PlayerBusy()
    {
        var state = player.animator.GetCurrentAnimatorStateInfo(0);
        return player.animator.IsInTransition(0) || state.fullPathHash != Animator.StringToHash("Base Layer.player_idle");
    }

    void End(bool passed, string message)
    {
        running = false;
        typing.StopPracticeInput();
        TutorialRun.Practicing = TutorialRun.Practice.None;
        GamePauseController.SetTutorialReading(true);
        Finished?.Invoke(passed, message);
    }

    internal void Cancel()
    {
        running = wordAccepted = succeeded = attackStarted = false;
        TutorialRun.Practicing = TutorialRun.Practice.None;
        if (typing) typing.StopPracticeInput();
        if (enemy) { enemy.HoldPracticeAttacks(true); enemy.CancelPracticeAttack(); }
        if (parry) parry.ResetPracticePose();
        if (player) player.ResetPracticeAttack();
    }

    void OnDisable() => Cancel();
    void OnDestroy()
    {
        TypingManager.OnWordCorrect -= WordCompleted;
        if (parry) parry.OnParrySuccess -= Parried;
    }
}
