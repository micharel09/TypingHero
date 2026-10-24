using TMPro;
using UnityEngine;

internal struct CombatResultData
{
    internal string Context;
    internal int Score, Defeated, Letters, Correct, Parries, BestDefeated, BestScore;
    internal float Seconds;
    internal bool NewBest;
}

// Presentation only: statistics and record decisions are owned by the completed run.
public sealed class CombatResultView : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] TMP_Text context, score, defeated, time, accuracy, parries, best;

    internal void Present(CombatResultData data)
    {
        context.text = data.Context;
        score.text = data.Score.ToString();
        defeated.text = data.Defeated.ToString();
        time.text = $"{data.Seconds:0.0}s";
        accuracy.text = data.Letters == 0 ? "—" : $"{100f * data.Correct / data.Letters:0.0}%";
        parries.text = data.Parries.ToString();
        best.text = data.NewBest ? "NEW BEST" : $"BEST  {data.BestDefeated} defeated · {data.BestScore} pts";
        content.SetActive(true);
    }
}
