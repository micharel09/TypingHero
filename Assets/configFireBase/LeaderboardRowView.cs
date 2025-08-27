using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class LeaderboardRowView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image background;

    [Header("Name Display")]
    [SerializeField, Range(8, 32)] int maxNameLength = 20;

    public void Bind(int rank, string name, int score)
    {
        if (nameText != null)
        {
            // Truncate tên dài để tránh vỡ layout
            string displayName = TruncateName(name);
            nameText.text = $"{rank}. {displayName}";
        }

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void SetBackground(Color c)
    {
        if (background != null) background.color = c;
    }

    string TruncateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "Player";

        if (name.Length <= maxNameLength)
            return name;

        // Cắt và thêm ellipsis
        return name.Substring(0, maxNameLength - 5) + "…";
    }
}