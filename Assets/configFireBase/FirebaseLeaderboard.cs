using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public sealed class FirebaseLeaderboard : MonoBehaviour
{
    public enum AuthMode { PublicNoAuth, AnonymousAuth }

    [Header("Firebase")]
    [Tooltip("VD: https://your-project.asia-southeast1.firebasedatabase.app (KHÔNG có / ở cuối)")]
    [SerializeField] string databaseRootUrl = "";

    [Tooltip("Đường dẫn node chứa entries. VD: \"leaderboards/global\" hoặc \"leaderboard\"")]
    [SerializeField] string tablePath = "leaderboards/global";

    [Header("Field Keys (khớp với DB của bạn)")]
    [SerializeField] string nameKey = "name";
    [SerializeField] string scoreKey = "bestScore";
    [SerializeField] string timestampKey = "updatedAt";

    [Header("Auth Mode")]
    [SerializeField] AuthMode authMode = AuthMode.PublicNoAuth;
    [Tooltip("Chỉ cần khi AnonymousAuth")]
    [SerializeField] string webApiKey = "";
    [SerializeField] bool autoSignIn = false;

    [Header("Debug")]
    [SerializeField] bool logs = true;
    [SerializeField, Range(5, 60)] int httpTimeoutSec = 20;

    string _idToken;
    string _uid;

    public bool IsReady
        => !string.IsNullOrEmpty(databaseRootUrl) &&
           (authMode == AuthMode.PublicNoAuth || (!string.IsNullOrEmpty(_idToken) && !string.IsNullOrEmpty(_uid)));

    [Serializable] public sealed class SignInResp { public string idToken; public string localId; public string refreshToken; }

    public sealed class Row
    {
        public string uid;
        public string name;
        public int score;
        public long ts;
    }

    void Start()
    {
        if (authMode == AuthMode.AnonymousAuth && autoSignIn)
            StartCoroutine(SignInAnonymous());
        if (logs)
            Debug.Log($"[LB] Ready. Mode={authMode} Path={tablePath} Keys=({nameKey},{scoreKey},{timestampKey})");
    }

    // ============== PUBLIC API ==============

    public IEnumerator SubmitScore(string displayName, int score, Action<bool> done = null)
    {
        if (!CheckDbRoot(out var err)) { if (logs) Debug.LogError(err); done?.Invoke(false); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (authMode == AuthMode.AnonymousAuth && !IsReady) { done?.Invoke(false); yield break; }

        string uid = (authMode == AuthMode.PublicNoAuth) ? EnsureLocalUid() : _uid;

        // read old
        string path = CombinePath(tablePath, uid);
        string getUrl = DbUrl(path, AuthQuery());
        if (logs) Debug.Log("[LB] GET " + getUrl);
        var getReq = UnityWebRequest.Get(getUrl);
        getReq.timeout = httpTimeoutSec;
        yield return getReq.SendWebRequest();

        int oldScore = int.MinValue;
        if (getReq.result == UnityWebRequest.Result.Success &&
            !string.IsNullOrEmpty(getReq.downloadHandler.text) &&
            getReq.downloadHandler.text != "null")
        {
            var obj = MiniJson.Deserialize(getReq.downloadHandler.text) as Dictionary<string, object>;
            if (obj != null) oldScore = ReadInt(obj, scoreKey);
            if (logs) Debug.Log($"[LB] Old entry raw: {getReq.downloadHandler.text}");
        }

        if (oldScore >= score)
        {
            if (logs) Debug.Log($"[LB] Keep old score {oldScore} >= {score}");
            done?.Invoke(true);
            yield break;
        }

        // write new
        string putUrl = DbUrl(path, AuthQuery());
        string body = BuildEntryJson(displayName, score, NowUnixMillis());
        if (logs) Debug.Log("[LB] PUT " + putUrl + " body=" + body);
        var putReq = new UnityWebRequest(putUrl, "PUT");
        putReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        putReq.downloadHandler = new DownloadHandlerBuffer();
        putReq.SetRequestHeader("Content-Type", "application/json");
        putReq.timeout = httpTimeoutSec;

        yield return putReq.SendWebRequest();

        bool ok = putReq.result == UnityWebRequest.Result.Success;
        if (logs) Debug.Log(ok ? $"[LB] Submit OK: {putReq.downloadHandler.text}"
                               : $"[LB] Submit FAIL: {putReq.error} {putReq.downloadHandler.text}");
        done?.Invoke(ok);
    }

    public IEnumerator FetchTop(int count, Action<List<Row>> done)
    {
        if (!CheckDbRoot(out var err)) { if (logs) Debug.LogError(err); done?.Invoke(new List<Row>()); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (authMode == AuthMode.AnonymousAuth && !IsReady) { done?.Invoke(new List<Row>()); yield break; }

        count = Mathf.Max(1, count);

        string q = $"orderBy=%22{scoreKey}%22&limitToLast={count}";
        if (authMode == AuthMode.AnonymousAuth) q += $"&auth={_idToken}";
        var url = DbUrl(tablePath, q);
        if (logs) Debug.Log("[LB] GET " + url);

        var req = UnityWebRequest.Get(url);
        req.timeout = httpTimeoutSec;
        yield return req.SendWebRequest();

        var result = new List<Row>();
        if (req.result != UnityWebRequest.Result.Success ||
            string.IsNullOrEmpty(req.downloadHandler.text) ||
            req.downloadHandler.text == "null")
        {
            if (logs) Debug.LogWarning($"[LB] FetchTop empty/err: {req.error} {req.downloadHandler.text}");
            done?.Invoke(result);
            yield break;
        }

        var root = MiniJson.Deserialize(req.downloadHandler.text) as Dictionary<string, object>;
        if (root != null)
        {
            foreach (var kv in root)
            {
                var obj = kv.Value as Dictionary<string, object>;
                if (obj == null) continue;

                var row = new Row
                {
                    uid   = kv.Key,
                    name  = ReadStr(obj, nameKey) ?? "Player",
                    score = ReadInt(obj, scoreKey),
                    ts    = ReadLong(obj, timestampKey)
                };
                result.Add(row);
            }
            result.Sort((a, b) => b.score.CompareTo(a.score));
        }

        if (logs) Debug.Log($"[LB] FetchTop OK: {result.Count} rows (topScore={(result.Count>0 ? result[0].score : 0)})");
        done?.Invoke(result);
    }

    // ============== Auth (anonymous - optional) ==============

    public IEnumerator SignInAnonymous(Action<bool> done = null)
    {
        if (authMode != AuthMode.AnonymousAuth) { done?.Invoke(true); yield break; }
        if (!string.IsNullOrEmpty(_idToken) && !string.IsNullOrEmpty(_uid)) { done?.Invoke(true); yield break; }

        if (string.IsNullOrEmpty(webApiKey))
        {
            if (logs) Debug.LogError("[LB] Missing Web API Key for AnonymousAuth");
            done?.Invoke(false);
            yield break;
        }

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={webApiKey}";
        var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = httpTimeoutSec;

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var resp = JsonUtility.FromJson<SignInResp>(req.downloadHandler.text);
                _idToken = resp.idToken;
                _uid = resp.localId;
                if (logs) Debug.Log("[LB] Signed in (anonymous).");
                done?.Invoke(true);
            }
            catch
            {
                if (logs) Debug.LogError("[LB] SignIn parse failed");
                done?.Invoke(false);
            }
        }
        else
        {
            if (logs) Debug.LogError($"[LB] SignIn fail: {req.error} {req.downloadHandler.text}");
            done?.Invoke(false);
        }
    }

    // ============== Helpers ==============

    bool CheckDbRoot(out string error)
    {
        error = null;
        if (string.IsNullOrEmpty(databaseRootUrl)) { error = "[LB] Missing Database Root Url"; return false; }
        if (string.IsNullOrEmpty(tablePath)) { error = "[LB] Missing Table Path"; return false; }
        return true;
    }

    string AuthQuery() => (authMode == AuthMode.AnonymousAuth) ? $"auth={_idToken}" : null;

    string DbUrl(string path, string query = null)
    {
        var root = databaseRootUrl.TrimEnd('/');
        var p = string.IsNullOrEmpty(path) ? "" : ("/" + path.Trim('/'));
        var q = string.IsNullOrEmpty(query) ? "" : ("?" + query);
        return $"{root}{p}.json{q}";
    }

    static string CombinePath(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return b ?? "";
        if (string.IsNullOrEmpty(b)) return a ?? "";
        return a.TrimEnd('/') + "/" + b.TrimStart('/');
    }

    static long NowUnixMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    string EnsureLocalUid()
    {
        const string PrefKey = "LB_LocalUID";
        if (!PlayerPrefs.HasKey(PrefKey))
            PlayerPrefs.SetString(PrefKey, System.Guid.NewGuid().ToString("N"));
        return PlayerPrefs.GetString(PrefKey);
    }

    string BuildEntryJson(string displayName, int score, long ts)
    {
        var sb = new StringBuilder(128);
        sb.Append('{');
        AppendKV(sb, nameKey, string.IsNullOrEmpty(displayName) ? "Player" : displayName); sb.Append(',');
        AppendKV(sb, scoreKey, score); sb.Append(',');
        AppendKV(sb, timestampKey, ts);
        sb.Append('}');
        return sb.ToString();
    }

    static void AppendKV(StringBuilder sb, string key, string val)
    {
        sb.Append('"').Append(EscapeJson(key)).Append('"').Append(':')
          .Append('"').Append(EscapeJson(val)).Append('"');
    }
    static void AppendKV(StringBuilder sb, string key, long val)
    {
        sb.Append('"').Append(EscapeJson(key)).Append('"').Append(':').Append(val);
    }
    static void AppendKV(StringBuilder sb, string key, int val)
    {
        sb.Append('"').Append(EscapeJson(key)).Append('"').Append(':').Append(val);
    }
    static string EscapeJson(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    static string ReadStr(Dictionary<string, object> obj, string key)
        => obj != null && obj.TryGetValue(key, out var v) ? v as string : null;

    static int ReadInt(Dictionary<string, object> obj, string key)
    {
        if (obj == null || !obj.TryGetValue(key, out var v) || v == null) return 0;
        if (v is long l) return (int)l;
        if (v is double d) return (int)Math.Round(d);
        if (int.TryParse(v.ToString(), out var i)) return i;
        return 0;
    }
    static long ReadLong(Dictionary<string, object> obj, string key)
    {
        if (obj == null || !obj.TryGetValue(key, out var v) || v == null) return 0L;
        if (v is long l) return l;
        if (v is double d) return (long)Math.Round(d);
        if (long.TryParse(v.ToString(), out var i)) return i;
        return 0L;
    }

    // ====== Minimal JSON parser (deserialize only) ======
    static class MiniJson
    {
        public static object Deserialize(string json)
        {
            if (json == null) return null;
            return Parser.Parse(json);
        }

        sealed class Parser
        {
            readonly string _json; int _index;
            Parser(string json) { _json = json; _index = 0; }
            public static object Parse(string json) => new Parser(json).ParseValue();

            char Peek() => _index < _json.Length ? _json[_index] : '\0';
            char Next() => _index < _json.Length ? _json[_index++] : '\0';
            void SkipWs() { while (_index < _json.Length && char.IsWhiteSpace(_json[_index])) _index++; }

            object ParseValue()
            {
                SkipWs();
                char c = Peek();
                if (c == '"') return ParseString();
                if (c == '{') return ParseObject();
                if (c == '[') return ParseArray();
                if (c == '-' || char.IsDigit(c)) return ParseNumber();
                if (Match("true")) return true;
                if (Match("false")) return false;
                if (Match("null")) return null;
                return null;
            }

            bool Match(string s)
            {
                SkipWs();
                if (_index + s.Length > _json.Length) return false;
                for (int i = 0; i < s.Length; i++) if (_json[_index + i] != s[i]) return false;
                _index += s.Length; return true;
            }

            Dictionary<string, object> ParseObject()
            {
                var dict = new Dictionary<string, object>();
                Next(); // {
                while (true)
                {
                    SkipWs();
                    if (Peek() == '}') { Next(); break; }
                    string key = ParseString();
                    SkipWs();
                    if (Next() != ':') return dict;
                    object val = ParseValue();
                    dict[key] = val;
                    SkipWs();
                    char ch = Peek();
                    if (ch == ',') { Next(); continue; }
                    if (ch == '}') { Next(); break; }
                }
                return dict;
            }

            List<object> ParseArray()
            {
                var list = new List<object>();
                Next(); // [
                while (true)
                {
                    SkipWs();
                    if (Peek() == ']') { Next(); break; }
                    var v = ParseValue();
                    list.Add(v);
                    SkipWs();
                    char ch = Peek();
                    if (ch == ',') { Next(); continue; }
                    if (ch == ']') { Next(); break; }
                }
                return list;
            }

            string ParseString()
            {
                var sb = new StringBuilder();
                if (Next() != '"') return "";
                while (_index < _json.Length)
                {
                    char c = Next();
                    if (c == '"') break;
                    if (c == '\\')
                    {
                        char esc = Next();
                        switch (esc)
                        {
                            case '"': sb.Append('\"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'u':
                                var hex = new string(new[] { Next(), Next(), Next(), Next() });
                                if (ushort.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var cp))
                                    sb.Append((char)cp);
                                break;
                        }
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }

            object ParseNumber()
            {
                int start = _index;
                if (Peek() == '-') Next();
                while (char.IsDigit(Peek())) Next();
                if (Peek() == '.') { Next(); while (char.IsDigit(Peek())) Next(); }
                if (Peek() == 'e' || Peek() == 'E')
                {
                    Next(); if (Peek() == '+' || Peek() == '-') Next();
                    while (char.IsDigit(Peek())) Next();
                }
                string s = _json.Substring(start, _index - start);
                if (s.IndexOf('.') >= 0 || s.IndexOf('e') >= 0 || s.IndexOf('E') >= 0)
                {
                    if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d))
                        return d;
                }
                else
                {
                    if (long.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var l))
                        return l;
                }
                return 0;
            }
        }
    }
    // ==== NEW: Submit score OR update display name even when score doesn't improve ====
    public IEnumerator SubmitScoreOrUpdateName(string displayName, int score, Action<bool> done = null)
    {
        if (!CheckDbRoot(out var err)) { if (logs) Debug.LogError(err); done?.Invoke(false); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (authMode == AuthMode.AnonymousAuth && !IsReady) { done?.Invoke(false); yield break; }

        string uid = (authMode == AuthMode.PublicNoAuth) ? EnsureLocalUid() : _uid;
        string path = CombinePath(tablePath, uid);

        // 1) Read current entry
        string getUrl = DbUrl(path, AuthQuery());
        if (logs) Debug.Log("[LB] GET " + getUrl);
        var getReq = UnityWebRequest.Get(getUrl);
        getReq.timeout = httpTimeoutSec;
        yield return getReq.SendWebRequest();

        var nowTs = NowUnixMillis();

        // No entry yet -> write full
        if (getReq.result != UnityWebRequest.Result.Success || string.IsNullOrEmpty(getReq.downloadHandler.text) || getReq.downloadHandler.text == "null")
        {
            string firstBody = BuildEntryJson(displayName, score, nowTs);
            string putUrl = DbUrl(path, AuthQuery());
            if (logs) Debug.Log("[LB] PUT " + putUrl + " body=" + firstBody);
            var putReq = new UnityWebRequest(putUrl, "PUT");
            putReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(firstBody));
            putReq.downloadHandler = new DownloadHandlerBuffer();
            putReq.SetRequestHeader("Content-Type", "application/json");
            putReq.timeout = httpTimeoutSec;
            yield return putReq.SendWebRequest();
            bool ok0 = putReq.result == UnityWebRequest.Result.Success;
            if (logs) Debug.Log(ok0 ? "[LB] Created new entry" : $"[LB] Create FAIL: {putReq.error} {putReq.downloadHandler.text}");
            done?.Invoke(ok0);
            yield break;
        }

        // Has entry -> decide update
        var obj = MiniJson.Deserialize(getReq.downloadHandler.text) as Dictionary<string, object>;
        var oldName = ReadStr(obj, nameKey) ?? "Player";
        var oldScore = ReadInt(obj, scoreKey);

        if (score > oldScore)
        {
            // Improve score -> overwrite all
            string body = BuildEntryJson(displayName, score, nowTs);
            string putUrl = DbUrl(path, AuthQuery());
            if (logs) Debug.Log("[LB] PUT " + putUrl + " body=" + body);
            var putReq = new UnityWebRequest(putUrl, "PUT");
            putReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            putReq.downloadHandler = new DownloadHandlerBuffer();
            putReq.SetRequestHeader("Content-Type", "application/json");
            putReq.timeout = httpTimeoutSec;
            yield return putReq.SendWebRequest();
            bool ok = putReq.result == UnityWebRequest.Result.Success;
            if (logs) Debug.Log(ok ? "[LB] Score improved and updated." : $"[LB] PUT FAIL: {putReq.error} {putReq.downloadHandler.text}");
            done?.Invoke(ok);
            yield break;
        }

        // Score not improved -> update name only if changed
        if (!string.Equals(oldName, displayName, StringComparison.Ordinal))
        {
            // PATCH {"nameKey": "..."} to keep bestScore
            var patch = $"{{\"{EscapeJson(nameKey)}\":\"{EscapeJson(string.IsNullOrEmpty(displayName) ? "Player" : displayName)}\"}}";
            string patchUrl = DbUrl(path, AuthQuery());
            if (logs) Debug.Log("[LB] PATCH " + patchUrl + " body=" + patch);
            var patchReq = new UnityWebRequest(patchUrl, "PATCH");
            patchReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(patch));
            patchReq.downloadHandler = new DownloadHandlerBuffer();
            patchReq.SetRequestHeader("Content-Type", "application/json");
            patchReq.timeout = httpTimeoutSec;
            yield return patchReq.SendWebRequest();
            bool ok = patchReq.result == UnityWebRequest.Result.Success;
            if (logs) Debug.Log(ok ? "[LB] Name updated (score kept)." : $"[LB] PATCH FAIL: {patchReq.error} {patchReq.downloadHandler.text}");
            done?.Invoke(ok);
            yield break;
        }

        // Nothing to update
        if (logs) Debug.Log("[LB] No change (name same, score not higher).");
        done?.Invoke(true);
    }

}
