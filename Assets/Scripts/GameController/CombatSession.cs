using TMPro;
using UnityEngine;

// Owns statistics from scene start until the first lethal hit, before death cleanup.
public sealed class CombatSession : MonoBehaviour
{
    public static CombatSession I { get; private set; }
    public static bool Finished => I && I.finished;
    [SerializeField] TMP_Text resultLabel;
    bool finished;
    float seconds;
    int letters, correct, parries, finalScore;
    ParrySystem parry;
    internal bool RunWon { get; private set; }
    public float Seconds => seconds;
    public int Letters => letters;
    public int Correct => correct;
    public int Parries => parries;
    public int FinalScore => finalScore;

    void Awake()
    {
        I = this;
        if (!TutorialRun.Active && gameObject.scene.name == "SampleScene")
            gameObject.AddComponent<EndlessDuel>();
    }
    internal void RestoreEndless(EndlessRun run)
    {
        seconds = run.Seconds; letters = run.Letters; correct = run.Correct; parries = run.Parries;
    }
    void OnEnable()
    {
        TypingManager.OnInputResult += InputResult;
        parry = FindObjectOfType<ParrySystem>();
        if (parry) parry.OnParrySuccess += Parried;
    }
    void OnDisable()
    {
        TypingManager.OnInputResult -= InputResult;
        if (parry) parry.OnParrySuccess -= Parried;
        if (I == this) I = null;
    }
    void Update()
    {
        if (!TutorialRun.Active && !finished && !GamePauseController.InputBlocked) seconds += Time.unscaledDeltaTime;
    }
    void InputResult(bool hit)
    {
        if (TutorialRun.Active || finished) return;
        letters++;
        if (hit) correct++;
    }
    void Parried(ParrySystem.ParryContext _)
    {
        if (TutorialRun.Active || finished) return;
        parries++;
    }
    public void Complete(bool won)
    {
        if (TutorialRun.Active || finished) return;
        finished = true;
        finalScore = ScoreSystem.I ? ScoreSystem.I.Score : 0;
        if (ScoreSystem.I) ScoreSystem.I.SetPaused(true);
        string survival = "";
        if (EndlessDuel.I)
        {
            if (won)
            {
                EndlessDuel.I.EnemyDefeated();
                RunWon = EndlessDuel.I.Run.Completed;
                if (RunWon) survival = EndlessDuel.I.FinishRun(true) + "\n";
            }
            else survival = EndlessDuel.I.FinishRun() + "\n";
        }
        string accuracy = letters == 0 ? "—" : (100f * correct / letters).ToString("0.0") + "%";
        if (resultLabel)
        {
            resultLabel.text = survival + $"SCORE {finalScore}    TIME {seconds:0.0}s\nACCURACY {accuracy}    PARRIES {parries}";
            var view = resultLabel.GetComponentInParent<CombatResultView>(true);
            if (view)
            {
                var duel = EndlessDuel.I;
                view.Present(new CombatResultData
                {
                    Context = duel ? $"{duel.Run.Difficulty.Label()} · {(RunWon ? "Boss defeated" : "Defeated by " + duel.Run.Enemy.Name)}"
                        : won ? "Encounter cleared" : "Run ended",
                    Score = finalScore, Defeated = duel ? duel.Run.Defeated : won ? 1 : 0,
                    Seconds = seconds, Letters = letters, Correct = correct, Parries = parries,
                    NewBest = duel && duel.NewBest,
                    BestDefeated = duel ? duel.BestDefeated : 0, BestScore = duel ? duel.BestScore : 0
                });
            }
        }
    }
}
