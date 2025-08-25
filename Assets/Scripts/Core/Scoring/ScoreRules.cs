using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "ScoreRules", menuName = "TypingHero/Score Rules", order = 0)]
public class ScoreRules : ScriptableObject
{
    [Header("Base Points")]
    public int parryPoint = 30;
    public int wordPoint = 10;
    public int charPointInSlayer = 2;

    [Header("Combo (Fruit-Ninja-like)")]
    public float startMultiplier = 1f;
    public float maxMultiplier = 3f;
    public float addPerHit = 0.15f;

    [Tooltip("Cửa sổ combo (giây). Có hit mới trước khi hết cửa sổ thì combo tiếp tục và multiplier tăng; hết cửa sổ thì rơi về startMultiplier.")]
    public float comboWindowSeconds = 1.2f;

    [Header("Enable Combo Threshold")]
    [Tooltip("Số TỪ liên tiếp giúp bật combo trong chế độ thường (>= 5 theo yêu cầu). Trước ngưỡng này multiplier vẫn x1.0.")]
    [Min(0)] public int minWordsForCombo = 5;

    [Tooltip("Số KÝ TỰ liên tiếp giúp bật combo trong SlayerMode. Mặc định 0 = bật ngay như trước.")]
    [Min(0)] public int minSlayerCharsForCombo = 0;

    [Header("Color Thresholds (for UI flash)")]
    public float tier2Threshold = 2f;
    public float tier3Threshold = 3f;
    public Color colorTier1 = Color.white;
    public Color colorTier2 = new Color(1f, 0.85f, 0.25f);
    public Color colorTier3 = new Color(1f, 0.35f, 0.35f);

    [Header("Formatting")]
    public string scoreFormat = "0";
}
