using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;

public interface IScoreProvider { int CurrentScore { get; } }  // Nếu ScoreSystem implement thì tuyệt vời

[DisallowMultipleComponent]
public sealed class ScoreSubmitTrigger : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] LeaderboardSubmitter submitter;      // _Services(LeaderboardSubmitter)
    [SerializeField] MonoBehaviour scoreSystem;           // Component chứa điểm (ScoreSystem)
    [SerializeField] TMP_Text scoreText;                  // Fallback: UI đang hiển thị điểm

    [Header("Timing")]
    [Tooltip("Trễ trước khi đọc điểm (đảm bảo ScoreSystem đã cộng và UI đã cập nhật).")]
    [SerializeField, Range(0f, 0.5f)] float readDelaySeconds = 0.05f;

    [Tooltip("Khoảng cách tối thiểu giữa 2 lần submit để tránh spam.")]
    [SerializeField, Range(0f, 3f)] float minIntervalSeconds = 1.0f;

    [Header("Anti-Spam")]
    [Tooltip("Chỉ submit 1 lần cho mỗi round/session (reset khi game restart).")]
    [SerializeField] bool oncePerRound = false;

    [Header("Debug")]
    [SerializeField] bool log = true;

    float _lastSubmitTime = -999f;
    int _lastSubmittedValue = int.MinValue;
    bool _hasSubmittedThisRound = false; // Cho oncePerRound

    // === Gọi từ UnityEvent (Player die / Skeleton die) ===
    public void Submit() => StartCoroutine(SubmitCoro());

    // Reset flag khi bắt đầu round mới
    public void ResetRound()
    {
        _hasSubmittedThisRound = false;
        if (log) Debug.Log("[ScoreSubmitTrigger] Round reset - can submit again");
    }

    IEnumerator SubmitCoro()
    {
        if (submitter == null)
        {
            if (log) Debug.LogWarning("[ScoreSubmitTrigger] Missing LeaderboardSubmitter.");
            yield break;
        }

        // Chặn nếu đã submit trong round này
        if (oncePerRound && _hasSubmittedThisRound)
        {
            if (log) Debug.Log("[ScoreSubmitTrigger] Already submitted this round, skipping.");
            yield break;
        }

        // chặn spam quá sát nhau
        if (Time.unscaledTime - _lastSubmitTime < minIntervalSeconds)
            yield break;

        // chờ 1 nhịp nhỏ để ScoreSystem/UI kịp cập nhật
        if (readDelaySeconds > 0f)
            yield return new WaitForSecondsRealtime(readDelaySeconds);

        int score = ReadScoreSafe();

        // Clamp điểm âm về 0
        score = Mathf.Max(0, score);

        // vẫn cho phép submit cùng giá trị giữa các thời điểm khác nhau,
        // chỉ bỏ qua nếu vừa submit cùng giá trị ngay trước đó
        if (score == _lastSubmittedValue && Time.unscaledTime - _lastSubmitTime < minIntervalSeconds)
            yield break;

        _lastSubmittedValue = score;
        _lastSubmitTime = Time.unscaledTime;
        _hasSubmittedThisRound = true;

        if (log) Debug.Log($"[ScoreSubmitTrigger] Submit {score}");
        submitter.SubmitFinalScore(score);
    }

    int ReadScoreSafe()
    {
        // 1) Nếu hệ thống cung cấp interface IScoreProvider
        if (scoreSystem is IScoreProvider p) return Mathf.Max(0, p.CurrentScore);

        // 2) Reflection nhẹ: tìm property/method thường gặp
        if (scoreSystem != null)
        {
            var t = scoreSystem.GetType();
            // property
            foreach (var propName in new[] { "CurrentScore", "Score", "TotalScore", "Points" })
            {
                var pi = t.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (pi != null && pi.PropertyType == typeof(int))
                    return Mathf.Max(0, (int)pi.GetValue(scoreSystem));
            }
            // method GetScore()
            var mi = t.GetMethod("GetScore", BindingFlags.Public | BindingFlags.Instance);
            if (mi != null && mi.ReturnType == typeof(int))
                return Mathf.Max(0, (int)mi.Invoke(scoreSystem, null));
        }

        // 3) Fallback: parse từ Text
        if (scoreText != null)
        {
            var s = scoreText.text;
            int val = 0;
            if (!string.IsNullOrEmpty(s))
            {
                // giữ lại các ký tự số
                System.Text.StringBuilder sb = new System.Text.StringBuilder(s.Length);
                foreach (var c in s) if (char.IsDigit(c)) sb.Append(c);
                if (sb.Length > 0) int.TryParse(sb.ToString(), out val);
            }
            return Mathf.Max(0, val);
        }

        if (log) Debug.LogWarning("[ScoreSubmitTrigger] No score source bound.");
        return 0;
    }
}