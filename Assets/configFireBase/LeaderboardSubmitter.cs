using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class LeaderboardSubmitter : MonoBehaviour
{
    [SerializeField] FirebaseLeaderboard leaderboard;

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
        bool done = false;
        bool ok = false;
        yield return leaderboard.SubmitScoreOrUpdateName(name, score, r => { ok = r; done = true; });
        if (!done) yield break;
        Debug.Log(ok ? $"[LB_Submit] Uploaded: {name} -> {score}" : "[LB_Submit] Upload failed.");
        _busy = false;
    }
}
