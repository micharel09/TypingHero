using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class LeaderboardSubmitter : MonoBehaviour
{
    [SerializeField] FirebaseLeaderboard leaderboard;
    [SerializeField] bool logs = true;

    bool _busy;

    void Reset()
    {
        leaderboard = FindObjectOfType<FirebaseLeaderboard>();
    }

    public void SubmitFinalScore(int finalScore)
    {
        if (leaderboard == null) leaderboard = FindObjectOfType<FirebaseLeaderboard>();
        if (leaderboard == null) { Debug.LogError("[LB_Submit] Missing FirebaseLeaderboard."); return; }
        if (_busy) return;

        string name = PlayerIdentity.HasName ? PlayerIdentity.Name : "Player";
        StartCoroutine(SubmitRoutine(name, finalScore));
    }

    IEnumerator SubmitRoutine(string name, int score)
    {
        _busy = true;
        try
        {
            bool done = false;
            bool ok = false;

            // Dùng SubmitScoreWithRetry thay vì SubmitScoreOrUpdateName
            yield return leaderboard.SubmitScoreWithRetry(name, score, r => { ok = r; done = true; });
            if (!done) yield break;

            if (logs)
            {
                Debug.Log(ok ? $"[LB_Submit] Uploaded: {name} -> {score}" : "[LB_Submit] Upload failed.");
            }

            // Tùy chọn: trigger UI refresh mà không gọi thêm API
            if (ok)
            {
                var uiBinder = FindObjectOfType<LeaderboardUIBinder>();
                if (uiBinder != null)
                {
                    uiBinder.ForceRefresh(); // Binder sẽ dùng cache/throttle, nhẹ hơn
                }
            }
        }
        finally
        {
            _busy = false;
        }
    }
}