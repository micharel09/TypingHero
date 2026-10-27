using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(200)]
public sealed class EndlessDuel : MonoBehaviour
{
    internal static EndlessDuel I { get; private set; }
    internal EndlessRun Run { get; private set; }
    internal bool NewBest { get; private set; }
    internal int BestDefeated { get; private set; }
    internal int BestScore { get; private set; }
    GamePauseController pause;
    PlayerHealth player;
    bool ready, loading, enemyDefeated, resultSaved;
    string resultSummary;
    int encounterStartHp;

    void Awake() { I = this; }
    void Start()
    {
        Run = EndlessRun.EnterScene();
        player = FindObjectOfType<PlayerHealth>();
        pause = FindObjectOfType<GamePauseController>();
        var enemy = FindObjectOfType<SkeletonController>();
        enemy.ConfigureEndless(Run);
        DungeonSession.I.CaptureReadySpawn(enemy);
        if (Run.Health > 0) player.RestoreEncounterHealth(Run.Health);
        else Run.Health = player.Current;
        CombatSession.I.RestoreEndless(Run);
        if (ScoreSystem.I) ScoreSystem.I.RestoreRunScore(Run.Score);
        EndlessProfile.Encounter(Run.Role, Run.Slot == 4);
        encounterStartHp = player.Current;
        BeginEncounter();
    }

    public void Continue()
    {
        if (DungeonSession.InputSuppressed || !ready || loading || Run.Completed || GamePauseController.IsPaused || LeaderboardModal.BlocksShortcuts ||
            (pause && pause.IsRestarting)) return;
        if (CombatSession.Finished)
        {
            if (!player || player.IsDead) return;
            loading = true;
            EndlessRun.Continuing = true;
            DungeonSession.I.ReloadGameplay();
            return;
        }
        BeginEncounter();
    }

    void BeginEncounter()
    {
        ready = false;
        GamePauseController.SetEncounterReading(false);
    }

    internal void EnemyDefeated()
    {
        if (enemyDefeated || Run.Completed) return;
        enemyDefeated = true;
        Run.Capture(CombatSession.I, player.Current);
        EndlessProfile.Defeat(Run.Role, Run.Slot == 4);
        if (Run.Slot == 5)
        {
            EndlessProfile.Unlock(EndlessProfile.FirstBoss);
            if (player.Current == encounterStartHp) EndlessProfile.Unlock(EndlessProfile.PerfectBoss);
        }
        Run.Win(player.maxHealth);
    }

    internal void EnemyDeathFinished()
    {
        if (!player || player.IsDead) return;
        foreach (var bar in FindObjectsOfType<StaminaBarUI>()) bar.gameObject.SetActive(false);
        if (Run.Completed)
        {
            ready = false;
            if (pause && !pause.IsRestarting) pause.ShowVictory();
            return;
        }
        if (ready) return;
        ready = true;
        DungeonSession.I.BeginReady(FindObjectOfType<TypingManager>(), Continue);
    }

    internal string FinishRun(bool won = false)
    {
        if (resultSaved) return resultSummary;
        resultSaved = true;
        Run.Capture(CombatSession.I, won ? player.Current : 0);
        bool best = Run.SaveBest();
        var killer = FindObjectOfType<SkeletonController>();
        string cause = Run.Enemy.Name + " / " + (killer ? killer.LastAttack : "Attack");
        EndlessLeaderboard.Queue(Run.Defeated, Run.Score, Run.Difficulty);
        int cleared = PlayerPrefs.GetInt(EndlessDifficulties.StorageKey(Run.Difficulty, EndlessRun.BestDefeatedKey), 0);
        int score = PlayerPrefs.GetInt(EndlessDifficulties.StorageKey(Run.Difficulty, EndlessRun.BestScoreKey), 0);
        NewBest = best; BestDefeated = cleared; BestScore = score;
        string outcome = won ? "BOSS DEFEATED" : "FELL TO " + cause;
        return resultSummary = $"{Run.Difficulty.Label()} · {outcome}\nDEFEATED {Run.Defeated}\n{(best ? "NEW BEST" : "BEST")} {cleared} DEFEATED · {score} POINTS";
    }

    void OnDestroy() { if (I == this) I = null; }
}
