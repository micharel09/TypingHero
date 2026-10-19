using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dedicated Endless result/queue; never encodes progression into Classic scores.
public sealed class EndlessLeaderboard : MonoBehaviour
{
    [Serializable] public sealed class Entry
    {
        public string name;
        public int defeated, score;
        public long updatedAt;
        [NonSerialized] public string uid;
        public int Round => defeated / 6 + 1;
    }
    public sealed class Result
    {
        public List<Entry> entries;
        public string error;
        public bool Success => error == null;
    }
    internal const string PendingKey = "EndlessPending_v1";
    static EndlessLeaderboard instance;
    bool submitting;
    FirebaseLeaderboard Provider => FindObjectOfType<FirebaseLeaderboard>();

    public static int Compare(Entry a, Entry b)
    {
        int value = b.defeated.CompareTo(a.defeated);
        if (value == 0) value = b.score.CompareTo(a.score);
        if (value == 0) value = a.updatedAt.CompareTo(b.updatedAt);
        return value == 0 ? StringComparer.Ordinal.Compare(a.uid, b.uid) : value;
    }
    internal static bool Better(Entry candidate, Entry old) => old == null || candidate.defeated > old.defeated ||
        (candidate.defeated == old.defeated && candidate.score > old.score);
    public static EndlessLeaderboard Get()
    {
        if (!instance)
        {
            instance = new GameObject("EndlessLeaderboard").AddComponent<EndlessLeaderboard>();
            DontDestroyOnLoad(instance.gameObject);
        }
        return instance;
    }
    internal static void Queue(int defeated, int score, EndlessDifficulty difficulty = EndlessDifficulty.Normal)
    {
        var entry = new Entry { name = PlayerIdentity.HasName ? PlayerIdentity.Name : "Player", defeated = defeated,
            score = score, updatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
        var pending = Pending(difficulty);
        if (Better(entry, pending)) { PlayerPrefs.SetString(EndlessDifficulties.StorageKey(difficulty, PendingKey), JsonUtility.ToJson(entry)); PlayerPrefs.Save(); }
        Get().StartCoroutine(Get().RetryPending());
    }
    internal static Entry Pending(EndlessDifficulty difficulty = EndlessDifficulty.Normal)
    {
        string key = EndlessDifficulties.StorageKey(difficulty, PendingKey);
        return PlayerPrefs.HasKey(key) ? JsonUtility.FromJson<Entry>(PlayerPrefs.GetString(key)) : null;
    }
    public IEnumerator RetryPending(Action<bool> done = null)
    {
        if (submitting) { done?.Invoke(false); yield break; }
        var provider = Provider;
        if (!provider) { done?.Invoke(false); yield break; }
        submitting = true;
        bool success = true;
        try
        {
            foreach (var difficulty in EndlessDifficulties.All)
            {
                var pending = Pending(difficulty);
                if (pending == null) continue;
                bool submitted = false;
                yield return provider.SubmitEndless(pending, ok => submitted = ok, difficulty);
                success &= submitted;
                if (!submitted) continue;
                var current = Pending(difficulty);
                if (current != null && !Better(current, pending))
                { PlayerPrefs.DeleteKey(EndlessDifficulties.StorageKey(difficulty, PendingKey)); PlayerPrefs.Save(); }
            }
        }
        finally { submitting = false; }
        done?.Invoke(success);
    }
    public IEnumerator Fetch(Action<Result> done, EndlessDifficulty difficulty = EndlessDifficulty.Normal)
    {
        // Submission belongs to this persistent service even if the modal closes.
        yield return StartCoroutine(RetryPending());
        var provider = Provider;
        if (!provider) { done(new Result { error = "Leaderboard unavailable. Retry." }); yield break; }
        yield return provider.FetchEndless(done, difficulty);
    }
}
