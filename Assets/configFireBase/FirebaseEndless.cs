using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed partial class FirebaseLeaderboard
{
    internal IEnumerator SubmitEndless(EndlessLeaderboard.Entry candidate, Action<bool> done,
        EndlessDifficulty difficulty = EndlessDifficulty.Normal)
    {
        if (!CheckDbRoot(out _)) { done(false); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (!IsReady) { done(false); yield break; }
        string uid = EnsureLocalUid();
        PlayerPrefs.Save();
        string url = DbUrl(CombinePath(EndlessDifficulties.Path(difficulty), uid), AuthQuery());
        for (int attempt = 0; attempt < 3; attempt++)
        {
            string tag;
            using (var read = UnityWebRequest.Get(url))
            {
                read.timeout = httpTimeoutSec; SetStandardHeaders(read);
                read.SetRequestHeader("X-Firebase-ETag", "true");
                yield return read.SendWebRequest();
                if (read.result != UnityWebRequest.Result.Success) { done(false); yield break; }
                string json = read.downloadHandler.text;
                EndlessLeaderboard.Entry previous = null;
                if (json != "null")
                {
                    if (!TryEndlessEntry(uid, json, out previous)) { done(false); yield break; }
                }
                if (!EndlessLeaderboard.Better(candidate, previous)) { done(true); yield break; }
                tag = read.GetResponseHeader("ETag");
                if (string.IsNullOrEmpty(tag)) { done(false); yield break; }
            }
            using (var write = new UnityWebRequest(url, "PUT"))
            {
                write.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(candidate)));
                write.downloadHandler = new DownloadHandlerBuffer(); write.timeout = httpTimeoutSec;
                write.SetRequestHeader("Content-Type", "application/json"); write.SetRequestHeader("if-match", tag);
                yield return write.SendWebRequest();
                if (write.result == UnityWebRequest.Result.Success) { done(true); yield break; }
                if (write.responseCode != 412) { done(false); yield break; }
            }
        }
        done(false);
    }

    internal IEnumerator FetchEndless(Action<EndlessLeaderboard.Result> done,
        EndlessDifficulty difficulty = EndlessDifficulty.Normal)
    {
        if (!CheckDbRoot(out _)) { done(new EndlessLeaderboard.Result { error = "Leaderboard unavailable. Retry." }); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (!IsReady) { done(new EndlessLeaderboard.Result { error = "Connection failed. Retry." }); yield break; }
        var entries = new List<EndlessLeaderboard.Entry>();
        string query = "orderBy=%22defeated%22&limitToLast=1000";
        bool ok = false;
        yield return ReadEndless(difficulty, query, rows => { ok = rows != null; if (ok) entries = rows; });
        if (!ok) { done(new EndlessLeaderboard.Result { error = "Could not load Endless. Retry." }); yield break; }
        if (entries.Count == 1000)
        {
            int boundary = entries.Min(e => e.defeated);
            // Fetch the COMPLETE tie group before ranking, rather than Firebase's uid-selected tail.
            yield return ReadEndless(difficulty, "orderBy=%22defeated%22&equalTo=" + boundary, rows =>
            {
                ok = rows != null;
                if (ok) { entries.RemoveAll(e => e.defeated == boundary); entries.AddRange(rows); }
            });
            if (!ok) { done(new EndlessLeaderboard.Result { error = "Could not load tied results. Retry." }); yield break; }
        }
        entries.Sort(EndlessLeaderboard.Compare);
        if (entries.Count > 1000) entries.RemoveRange(1000, entries.Count - 1000);
        done(new EndlessLeaderboard.Result { entries = entries });
    }

    IEnumerator ReadEndless(EndlessDifficulty difficulty, string query, Action<List<EndlessLeaderboard.Entry>> done)
    {
        string auth = AuthQuery();
        using (var request = UnityWebRequest.Get(DbUrl(EndlessDifficulties.Path(difficulty), query + (string.IsNullOrEmpty(auth) ? "" : "&" + auth))))
        {
            request.timeout = httpTimeoutSec; SetStandardHeaders(request);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) { done(null); yield break; }
            if (request.downloadHandler.text == "null") { done(new List<EndlessLeaderboard.Entry>()); yield break; }
            var root = MiniJson.Deserialize(request.downloadHandler.text) as Dictionary<string, object>;
            if (root == null) { done(null); yield break; }
            var rows = new List<EndlessLeaderboard.Entry>();
            foreach (var pair in root)
            {
                var fields = pair.Value as Dictionary<string, object>;
                if (!TryEndlessFields(pair.Key, fields, out var row)) { done(null); yield break; }
                rows.Add(row);
            }
            done(rows);
        }
    }
    static bool TryEndlessEntry(string uid, string json, out EndlessLeaderboard.Entry entry) =>
        TryEndlessFields(uid, MiniJson.Deserialize(json) as Dictionary<string, object>, out entry);
    static bool TryEndlessFields(string uid, Dictionary<string, object> fields, out EndlessLeaderboard.Entry entry)
    {
        entry = null;
        if (fields == null || !fields.TryGetValue("name", out var name) || !(name is string) ||
            !fields.TryGetValue("defeated", out var defeated) || !(defeated is long) ||
            !fields.TryGetValue("score", out var score) || !(score is long) ||
            !fields.TryGetValue("updatedAt", out var time) || !(time is long) ||
            (long)defeated < 0 || (long)defeated > int.MaxValue || (long)score < 0 || (long)score > int.MaxValue || (long)time < 0) return false;
        entry = new EndlessLeaderboard.Entry { uid = uid, name = (string)name, defeated = (int)(long)defeated,
            score = (int)(long)score, updatedAt = (long)time };
        return true;
    }
}
