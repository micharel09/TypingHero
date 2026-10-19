using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Uses the same Button skin, focus, sound and press feedback as existing controls.
public sealed class EndlessDifficultySelector : Button
{
    [SerializeField] TMP_Text caption, description;
    [SerializeField] LeaderboardUIBinder table;
    [SerializeField] ScoreRules rules;
    EndlessDifficulty value = EndlessDifficulty.Normal;
    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(Next);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying) Display(table ? table.Difficulty : EndlessDifficulties.Selected);
    }
    public void Previous() => Change(-1);
    public void Next() => Change(1);
    void Change(int step)
    {
        if (!IsActive() || !IsInteractable()) return;
        value = EndlessDifficulties.Step(value, step);
        if (table) table.SelectDifficulty(value); else EndlessDifficulties.Selected = value;
        Display(value); Select();
    }
    public void Display(EndlessDifficulty difficulty)
    {
        value = difficulty;
        if (caption) caption.text = value.Label();
        if (description && rules) description.text = $"Enemy pace {value.Pace():0.##}x / combo {rules.comboWindowSeconds / value.Pace():0.###}s";
    }
    public override void OnMove(AxisEventData data)
    {
        if (data.moveDir == MoveDirection.Left || data.moveDir == MoveDirection.Right)
        { Change(data.moveDir == MoveDirection.Left ? -1 : 1); data.Use(); }
        else base.OnMove(data);
    }
}
