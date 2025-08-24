using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "ScoreRules", menuName = "TypingHero/Score Rules", order = 10)]
public sealed class ScoreRules : ScriptableObject
{
    [Header("Base Points")]
    public int pointsParrySuccess = 30;
    public int pointsWordComplete = 50;
    public int pointsCharSlayer = 10;

    [Header("Combo / Multiplier")]
    [Tooltip("Mức tăng multiplier cho mỗi đơn vị combo (word thường, char trong Slayer).")]
    [Range(0.05f, 1f)] public float multiplierStep = 0.25f;
    [Tooltip("Giới hạn multiplier tối đa.")]
    [Range(1f, 10f)] public float multiplierMax = 3.0f;

    [Header("Threshold Colors")]
    [Tooltip("x < level2 → dùng color1,  level2 ≤ x < level3 → color2,  x ≥ level3 → color3")]
    public float level2 = 2.0f;
    public float level3 = 3.0f;
    public Color color1 = Color.white;     // x1.x
    public Color color2 = new Color(1f, 0.84f, 0f, 1f); // vàng
    public Color color3 = Color.red;       // đỏ

    [Header("UI / Effects")]
    [Tooltip("Thời gian flash/punch trên ScoreText khi cộng điểm.")]
    public float scoreFlashDuration = 0.08f;
    [Tooltip("Độ lớn punch trên ScoreText khi cộng điểm.")]
    public float scorePunchScale = 1.08f;

    [Header("Floating Popup")]
    public float popupRiseDistance = 48f;
    public float popupLifetime = 0.6f;
    public AnimationCurve popupEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Time")]
    [Tooltip("UI dùng unscaled time để không bị ảnh hưởng hitstop/pause?")]
    public bool uiUseUnscaledTime = true;

    public Color ColorByMultiplier(float mul)
    {
        if (mul >= level3) return color3;
        if (mul >= level2) return color2;
        return color1;
    }
}
