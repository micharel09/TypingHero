using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class LeaderboardRowView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image background;

    public void Bind(int rank, string name, int score)
    {
        if (nameText  != null) nameText.text  = $"{rank}. {name}";
        if (scoreText != null) scoreText.text = score.ToString();
    }

    public void SetBackground(Color c)
    {
        if (background != null) background.color = c;
    }
}
