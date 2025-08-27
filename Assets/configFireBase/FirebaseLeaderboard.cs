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

    [Header("Retry Settings")]
    [SerializeField] int maxRetries = 3;
    [SerializeField] float retryDelay = 2f;

    [Header("Fetch Throttling")]
    [Tooltip("Khoảng cách tối thiểu giữa các lần fetch (giây) - ngăn spam request")]
    [SerializeField] float minFetchInterval = 1.0f;

    [Tooltip("Cập nhật timestamp khi PATCH tên (không chỉ khi tăng điểm)")]
    [SerializeField] bool updateTimestampOnNameChange = false;

    [Header("Display Name Settings")]
    [SerializeField, Range(2, 32)] int minNameLength = 2;
    [SerializeField, Range(8, 64)] int maxNameLength = 20;

    [Header("Debug")]
    [SerializeField] bool logs = true;
    [SerializeField, Range(5, 60)] int httpTimeoutSec = 15;
    [SerializeField] bool checkInternetConnection = false; // Tắt cho WebGL

    // Internal state
    string _idToken;
    string _uid;
    bool _fetching; // Chặn fetch chồng chéo
    long _lastFetchAt; // Cooldown cho FetchTop
    List<Row> _lastResult; // Cache kết quả gần nhất

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
        // Sanitize URL - Tránh lỗi do nhập nhầm /
        databaseRootUrl = (databaseRootUrl ?? "").TrimEnd('/');
        tablePath = (tablePath ?? "").Trim('/');
        _lastResult = new List<Row>();

        // Chỉ check connection nếu không phải WebGL
#if !UNITY_WEBGL || UNITY_EDITOR
        if (checkInternetConnection)
        {
            StartCoroutine(CheckConnection());
        }
#endif

        if (authMode == AuthMode.AnonymousAuth && autoSignIn)
            StartCoroutine(SignInAnonymous());
        if (logs)
            Debug.Log($"[LB] Ready. Mode={authMode} Path={tablePath} Keys=({nameKey},{scoreKey},{timestampKey})");
    }

    // ============== CONNECTION CHECK (chỉ cho non-WebGL) ==============
#if !UNITY_WEBGL || UNITY_EDITOR
    IEnumerator CheckConnection()
    {
        var req = UnityWebRequest.Get("https://www.google.com");
        req.timeout = 5;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            if (logs) Debug.LogWarning("[LB] No internet connection detected");
        }
        else
        {
            if (logs) Debug.Log("[LB] Internet connection OK");
        }
    }
#endif

    // ============== MAIN SUBMIT WITH RETRY (recommended) ==============
    public IEnumerator SubmitScoreWithRetry(string displayName, int score, Action<bool> done = null)
    {
        // Sanitize display name
        displayName = SanitizeDisplayName(displayName);

        int attempts = 0;
        bool ok = false;

        do
        {
            attempts++;
            yield return SubmitScoreOrUpdateName(displayName, score, r => ok = r);

            if (ok) break;

            if (attempts < maxRetries)
            {
                if (logs) Debug.Log($"[LB] Retry attempt {attempts}/{maxRetries} in {retryDelay}s");
                yield return new WaitForSecondsRealtime(retryDelay);
            }
        }
        while (attempts < maxRetries);

        if (logs) Debug.Log(ok ? $"[LB] Submit successful after {attempts} attempts"
                               : $"[LB] Submit failed after {maxRetries} attempts");

        done?.Invoke(ok);
    }

    public IEnumerator FetchTop(int count, Action<List<Row>> done)
    {
        // Cooldown check - ngăn spam request với tính toán thời gian minh bạch
        var now = NowUnixMillis();
        long thresholdMs = (long)(minFetchInterval * 1000f);

        if (now - _lastFetchAt < thresholdMs)
        {
            if (logs) Debug.Log($"[LB] FetchTop throttled (cooldown: {minFetchInterval}s) - returning cached data");
            // Trả về cache thay vì rỗng để UI không bị xóa
            done?.Invoke(new List<Row>(_lastResult));
            yield break;
        }

        // Chặn fetch chồng chéo
        if (_fetching)
        {
            if (logs) Debug.Log("[LB] FetchTop already running - returning cached data");
            // Nhất quán với trường hợp throttle
            done?.Invoke(new List<Row>(_lastResult));
            yield break;
        }

        _fetching = true;

        try
        {
            if (!CheckDbRoot(out var err))
            {
                if (logs) Debug.LogError(err);
                done?.Invoke(new List<Row>());
                yield break;
            }

            if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
            if (authMode == AuthMode.AnonymousAuth && !IsReady)
            {
                done?.Invoke(new List<Row>());
                yield break;
            }

            count = Mathf.Max(1, count);

            // Cache-buster + query chuẩn
            var ts = NowUnixMillis();
            string q = $"orderBy=%22{scoreKey}%22&limitToLast={count}&__cb={ts}";
            if (authMode == AuthMode.AnonymousAuth) q += $"&auth={_idToken}";

            var url = DbUrl(tablePath, q);
            if (logs) Debug.Log("[LB] GET " + url);

            var req = UnityWebRequest.Get(url);
            req.timeout = httpTimeoutSec;

            // Set headers với helper
            SetStandardHeaders(req);

            yield return req.SendWebRequest();

            var result = new List<Row>();
            if (req.result != UnityWebRequest.Result.Success ||
                string.IsNullOrEmpty(req.downloadHandler.text) ||
                req.downloadHandler.text == "null")
            {
                // Log với responseCode để debug
                if (logs) Debug.LogWarning($"[LB] FetchTop err {(long)req.responseCode}: {req.error} | {req.downloadHandler.text}");
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
                        uid = kv.Key,
                        name = ReadStr(obj, nameKey) ?? "Player",
                        score = ReadInt(obj, scoreKey),
                        ts = ReadLong(obj, timestampKey)
                    };
                    result.Add(row);
                }
                result.Sort((a, b) => b.score.CompareTo(a.score));
            }

            // Cache kết quả thành công
            _lastResult = result;

            if (logs) Debug.Log($"[LB] FetchTop OK: {result.Count} rows (topScore={(result.Count > 0 ? result[0].score : 0)})");
            done?.Invoke(result);
        }
        finally
        {
            // Update cooldown timestamp
            _lastFetchAt = NowUnixMillis();
            _fetching = false;
        }
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
        req.timeout = httpTimeoutSec;

        SetStandardHeaders(req);
        req.SetRequestHeader("Content-Type", "application/json");

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
            if (logs) Debug.LogError($"[LB] SignIn fail {(long)req.responseCode}: {req.error} | {req.downloadHandler.text}");
            done?.Invoke(false);
        }
    }

    // ============== CORE: Submit score OR update display name ====
    IEnumerator SubmitScoreOrUpdateName(string displayName, int score, Action<bool> done = null)
    {
        if (!CheckDbRoot(out var err)) { if (logs) Debug.LogError(err); done?.Invoke(false); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (authMode == AuthMode.AnonymousAuth && !IsReady) { done?.Invoke(false); yield break; }

        string uid = (authMode == AuthMode.PublicNoAuth) ? EnsureLocalUid() : _uid;
        string path = CombinePath(tablePath, uid);

        // 1) Read current entry với cache-buster
        var ts = NowUnixMillis();
        string q = (authMode == AuthMode.AnonymousAuth)
            ? $"auth={_idToken}&__cb={ts}"
            : $"__cb={ts}";
        string getUrl = DbUrl(path, q);

        if (logs) Debug.Log("[LB] GET " + getUrl);
        var getReq = UnityWebRequest.Get(getUrl);
        getReq.timeout = httpTimeoutSec;
        SetStandardHeaders(getReq);
        yield return getReq.SendWebRequest();

        var nowTs = ts; // Reuse timestamp

        // No entry yet -> write full
        if (getReq.result != UnityWebRequest.Result.Success ||
            string.IsNullOrEmpty(getReq.downloadHandler.text) ||
            getReq.downloadHandler.text == "null")
        {
            // Log rõ ràng: đây là decide-by-design, không phải bug
            if (logs) Debug.Log($"[LB] No existing entry (GET {(long)getReq.responseCode}: {getReq.error}) -> Creating new");

            string firstBody = BuildEntryJson(displayName, score, nowTs);
            string putUrl = DbUrl(path, AuthQuery());
            if (logs) Debug.Log("[LB] PUT " + putUrl + " body=" + firstBody);

            var putReq = new UnityWebRequest(putUrl, "PUT");
            putReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(firstBody));
            putReq.downloadHandler = new DownloadHandlerBuffer();
            putReq.timeout = httpTimeoutSec;
            SetStandardHeaders(putReq);
            putReq.SetRequestHeader("Content-Type", "application/json");
            yield return putReq.SendWebRequest();

            bool ok0 = putReq.result == UnityWebRequest.Result.Success;
            if (logs)
            {
                if (ok0) Debug.Log("[LB] Created new entry");
                else Debug.LogWarning($"[LB] Create FAIL {(long)putReq.responseCode}: {putReq.error} | {putReq.downloadHandler.text}");
            }
            done?.Invoke(ok0);
            yield break;
        }

        // Has entry -> decide update
        var obj = MiniJson.Deserialize(getReq.downloadHandler.text) as Dictionary<string, object>;
        var oldName = ReadStr(obj, nameKey) ?? "Player";
        var oldScore = ReadInt(obj, scoreKey);

        if (score > oldScore)
        {
            // Score improved -> overwrite all
            string body = BuildEntryJson(displayName, score, nowTs);
            string putUrl = DbUrl(path, AuthQuery());
            if (logs) Debug.Log("[LB] PUT " + putUrl + " body=" + body);

            var putReq = new UnityWebRequest(putUrl, "PUT");
            putReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            putReq.downloadHandler = new DownloadHandlerBuffer();
            putReq.timeout = httpTimeoutSec;
            SetStandardHeaders(putReq);
            putReq.SetRequestHeader("Content-Type", "application/json");
            yield return putReq.SendWebRequest();

            bool ok = putReq.result == UnityWebRequest.Result.Success;
            if (logs)
            {
                if (ok) Debug.Log("[LB] Score improved and updated.");
                else Debug.LogWarning($"[LB] PUT FAIL {(long)putReq.responseCode}: {putReq.error} | {putReq.downloadHandler.text}");
            }
            done?.Invoke(ok);
            yield break;
        }

        // Score not improved -> update name only if changed
        if (!string.Equals(oldName, displayName, StringComparison.Ordinal))
        {
            // PATCH name (+ optionally timestamp)
            string patch;
            if (updateTimestampOnNameChange)
            {
                // Update both name and timestamp
                patch = $"{{\"{EscapeJson(nameKey)}\":\"{EscapeJson(displayName)}\",\"{EscapeJson(timestampKey)}\":{nowTs}}}";
            }
            else
            {
                // Update name only, keep old timestamp
                patch = $"{{\"{EscapeJson(nameKey)}\":\"{EscapeJson(displayName)}\"}}";
            }

            string patchUrl = DbUrl(path, AuthQuery());
            if (logs) Debug.Log("[LB] PATCH " + patchUrl + " body=" + patch);

            var patchReq = new UnityWebRequest(patchUrl, "PATCH");
            patchReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(patch));
            patchReq.downloadHandler = new DownloadHandlerBuffer();
            patchReq.timeout = httpTimeoutSec;
            SetStandardHeaders(patchReq);
            patchReq.SetRequestHeader("Content-Type", "application/json");
            yield return patchReq.SendWebRequest();

            bool ok = patchReq.result == UnityWebRequest.Result.Success;
            if (logs)
            {
                if (ok) Debug.Log("[LB] Name updated (score kept).");
                else Debug.LogWarning($"[LB] PATCH FAIL {(long)patchReq.responseCode}: {patchReq.error} | {patchReq.downloadHandler.text}");
            }
            done?.Invoke(ok);
            yield break;
        }

        // Nothing to update
        if (logs) Debug.Log("[LB] No change (name same, score not higher).");
        done?.Invoke(true);
    }

    // ============== Helpers ==============
    void SetStandardHeaders(UnityWebRequest req)
    {
        req.SetRequestHeader("Accept", "application/json");
        req.SetRequestHeader("Cache-Control", "no-store, no-cache, max-age=0");
        req.SetRequestHeader("Pragma", "no-cache");
    }

    string SanitizeDisplayName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Player";

        // Trim và remove control characters
        name = name.Trim();
        var sb = new StringBuilder();
        foreach (char c in name)
        {
            if (!char.IsControl(c))
                sb.Append(c);
        }

        string cleaned = sb.ToString();

        // Clamp length
        if (cleaned.Length < minNameLength)
            return "Player";
        if (cleaned.Length > maxNameLength)
            cleaned = cleaned.Substring(0, maxNameLength);

        return cleaned;
    }

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
        AppendKV(sb, nameKey, displayName); sb.Append(',');
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
    public IEnumerator FetchMyRank(Action<int?> done)
    {
        if (!CheckDbRoot(out var err)) { if (logs) Debug.LogError(err); done?.Invoke(null); yield break; }
        if (authMode == AuthMode.AnonymousAuth && !IsReady) yield return SignInAnonymous();
        if (authMode == AuthMode.AnonymousAuth && !IsReady) { done?.Invoke(null); yield break; }

        string uid = (authMode == AuthMode.PublicNoAuth) ? EnsureLocalUid() : _uid;
        string path = CombinePath(tablePath, uid);

        // 1) Lấy điểm hiện tại của tôi
        long ts = NowUnixMillis();
        string getMeQ = (authMode == AuthMode.AnonymousAuth) ? $"auth={_idToken}&__cb={ts}" : $"__cb={ts}";
        string getMeUrl = DbUrl(path, getMeQ);

        if (logs) Debug.Log("[LB] GET(me) " + getMeUrl);
        var getMe = UnityWebRequest.Get(getMeUrl);
        getMe.timeout = httpTimeoutSec;
        SetStandardHeaders(getMe);
        yield return getMe.SendWebRequest();

        if (getMe.result != UnityWebRequest.Result.Success ||
            string.IsNullOrEmpty(getMe.downloadHandler.text) ||
            getMe.downloadHandler.text == "null")
        {
            if (logs) Debug.LogWarning($"[LB] FetchMyRank: user entry missing/err {(long)getMe.responseCode}: {getMe.error}");
            done?.Invoke(null); yield break;
        }

        var meObj = MiniJson.Deserialize(getMe.downloadHandler.text) as Dictionary<string, object>;
        int myScore = ReadInt(meObj, scoreKey);
        if (myScore <= 0) { done?.Invoke(null); yield break; }

        // 2) Đếm số người có điểm > myScore
        long ts2 = NowUnixMillis();
        string baseQ = $"orderBy=%22{scoreKey}%22&startAt={myScore + 1}&__cb={ts2}";
        if (authMode == AuthMode.AnonymousAuth) baseQ += $"&auth={_idToken}";

        int? rank = null;

        // 2a) Thử shallow (nhẹ)
        {
            string q = baseQ + "&shallow=true";
            string url = DbUrl(tablePath, q);
            if (logs) Debug.Log("[LB] GET(countHigher:shallow) " + url);

            var req = UnityWebRequest.Get(url);
            req.timeout = httpTimeoutSec;
            SetStandardHeaders(req);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                if (!string.IsNullOrEmpty(req.downloadHandler.text) && req.downloadHandler.text != "null")
                {
                    var dict = MiniJson.Deserialize(req.downloadHandler.text) as Dictionary<string, object>;
                    int higher = dict?.Count ?? 0;
                    rank = higher + 1;
                }
                else
                {
                    // Không ai hơn điểm tôi → #1
                    rank = 1;
                }
            }
            else
            {
                if (logs) Debug.LogWarning($"[LB] shallow failed {(long)req.responseCode}: {req.error}");
            }
        }

        // 2b) Fallback nếu shallow lỗi
        if (!rank.HasValue)
        {
            string q = baseQ; // không shallow
            string url2 = DbUrl(tablePath, q);
            if (logs) Debug.Log("[LB] GET(countHigher:fallback) " + url2);

            var req2 = UnityWebRequest.Get(url2);
            req2.timeout = httpTimeoutSec;
            SetStandardHeaders(req2);
            yield return req2.SendWebRequest();

            if (req2.result == UnityWebRequest.Result.Success &&
                !string.IsNullOrEmpty(req2.downloadHandler.text) &&
                req2.downloadHandler.text != "null")
            {
                var root = MiniJson.Deserialize(req2.downloadHandler.text) as Dictionary<string, object>;
                int higher = root?.Count ?? 0;
                rank = higher + 1;
            }
            else if (req2.result == UnityWebRequest.Result.Success && req2.downloadHandler.text == "null")
            {
                rank = 1; // fallback cũng xác nhận không ai hơn
            }
            else
            {
                if (logs) Debug.LogWarning($"[LB] fallback failed {(long)req2.responseCode}: {req2.error}");
                rank = null; // KHÔNG gán #1 khi lỗi
            }
        }

        if (logs && rank.HasValue) Debug.Log($"[LB] MyRank: score={myScore} -> #{rank.Value}");
        done?.Invoke(rank);
    }


}