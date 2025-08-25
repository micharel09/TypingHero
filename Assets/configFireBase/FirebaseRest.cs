using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public sealed class FirebaseRest : MonoBehaviour
{
    [Header("Firebase Config")]
    [Tooltip("Firebase > Project settings > Web API Key")]
    [SerializeField] string webApiKey = "";

    [Tooltip("Realtime DB URL. VD: https://<id>-default-rtdb.asia-southeast1.firebasedatabase.app/")]
    [SerializeField] string databaseUrl = "";

    [Header("Auto Ping (optional)")]
    [Tooltip("VD: leaderboards/global/testUser1 (không .json). Để trống để bỏ qua.")]
    [SerializeField] string autoPingPath = "";

    [Header("Options")]
    [SerializeField] bool verboseLogs = true;
    [SerializeField] float tokenRefreshSkew = 120f; // refresh trước khi hết hạn ~2 phút

    // ====== Public state ======
    public static FirebaseRest I { get; private set; }
    public string IdToken { get; private set; }
    public string UserId { get; private set; }
    public bool IsReady => !string.IsNullOrEmpty(IdToken) && !string.IsNullOrEmpty(UserId);

    // refresh
    string _refreshToken;
    double _tokenExpiresAtUnix; // seconds since epoch

    // ====== DTOs ======
    [Serializable]
    sealed class SignInResp
    {
        public string idToken;
        public string localId;
        public string refreshToken;
        public string expiresIn; // seconds (string from Firebase)
    }
    [Serializable]
    sealed class RefreshResp
    {
        public string access_token;
        public string user_id;
        public string refresh_token;
        public string expires_in;
        public string token_type;
    }

    // ====== Lifecycle ======
    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        if (string.IsNullOrEmpty(webApiKey) || string.IsNullOrEmpty(databaseUrl))
        {
            Debug.LogError("[FirebaseRest] Thiếu WebApiKey hoặc DatabaseUrl.");
            yield break;
        }

        // 1) Anonymous sign-in
        yield return SignInAnonymous((ok, err) =>
        {
            if (!ok) Debug.LogError($"[FirebaseRest] SignIn fail: {err}");
        });
        if (!IsReady) yield break;

        // 2) Auto ping (tuỳ chọn)
        if (!string.IsNullOrEmpty(autoPingPath))
        {
            yield return GetRawCo(autoPingPath, (ok, json, err) =>
            {
                if (ok && verboseLogs) Debug.Log($"[FirebaseRest] PING OK\nPath: {autoPingPath}\nJSON: {json}");
                else if (!ok) Debug.LogError($"[FirebaseRest] PING FAIL {err}");
            });
        }
    }

    // ====== Auth ======
    public IEnumerator SignInAnonymous(Action<bool, string> done)
    {
        if (IsReady) { done?.Invoke(true, null); yield break; }

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={webApiKey}";
        var req = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes("{}");
        req.uploadHandler   = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            done?.Invoke(false, $"HTTP {req.responseCode}: {req.error} - {req.downloadHandler.text}");
            yield break;
        }

        var resp = JsonUtility.FromJson<SignInResp>(req.downloadHandler.text);
        if (resp == null || string.IsNullOrEmpty(resp.idToken))
        {
            done?.Invoke(false, "Parse sign-in response failed.");
            yield break;
        }

        IdToken       = resp.idToken;
        UserId        = resp.localId;
        _refreshToken = resp.refreshToken;
        _tokenExpiresAtUnix = NowUnix() + SafeParseDouble(resp.expiresIn, 3600) - tokenRefreshSkew;

        if (verboseLogs) Debug.Log($"[FirebaseRest] Signed in. uid={UserId}");
        done?.Invoke(true, null);
    }

    IEnumerator EnsureValidToken()
    {
        if (!IsReady) yield break;
        if (NowUnix() < _tokenExpiresAtUnix) yield break; // còn hạn

        // refresh
        string url = $"https://securetoken.googleapis.com/v1/token?key={webApiKey}";
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "refresh_token");
        form.AddField("refresh_token", _refreshToken);
        using var req = UnityWebRequest.Post(url, form);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[FirebaseRest] Refresh token fail: {req.responseCode} {req.error} {req.downloadHandler.text}");
            // Thử đăng nhập lại ẩn danh
            yield return SignInAnonymous(null);
            yield break;
        }

        var resp = JsonUtility.FromJson<RefreshResp>(req.downloadHandler.text);
        if (resp == null || string.IsNullOrEmpty(resp.access_token))
        {
            Debug.LogWarning("[FirebaseRest] Refresh parse fail → sign-in lại.");
            yield return SignInAnonymous(null);
            yield break;
        }

        IdToken       = resp.access_token;
        _refreshToken = resp.refresh_token;
        UserId        = !string.IsNullOrEmpty(resp.user_id) ? resp.user_id : UserId;
        _tokenExpiresAtUnix = NowUnix() + SafeParseDouble(resp.expires_in, 3600) - tokenRefreshSkew;
        if (verboseLogs) Debug.Log("[FirebaseRest] Token refreshed.");
    }

    // ====== Public GET APIs ======
    public void GetRaw(string path, Action<bool, string, string> done)
        => StartCoroutine(GetRawCo(path, done));

    public void Get<T>(string path, Action<bool, T, string> done) where T : class
        => StartCoroutine(GetTypedCo(path, done));

    // ====== Internals ======
    IEnumerator GetRawCo(string path, Action<bool, string, string> done)
    {
        if (!IsReady)
        {
            yield return SignInAnonymous(null);
            if (!IsReady) { done?.Invoke(false, null, "Auth not ready"); yield break; }
        }
        yield return EnsureValidToken();

        string url = BuildDbUrl(path);
        using var req = UnityWebRequest.Get(url);
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            done?.Invoke(false, null, $"HTTP {req.responseCode}: {req.error} - {req.downloadHandler.text}");
            yield break;
        }
        done?.Invoke(true, req.downloadHandler.text, null);
    }

    IEnumerator GetTypedCo<T>(string path, Action<bool, T, string> done) where T : class
    {
        T obj = null;
        string err = null;
        yield return GetRawCo(path, (ok, json, e) =>
        {
            if (!ok) { err = e; return; }
            try { obj = JsonUtility.FromJson<T>(json); }
            catch (Exception ex) { err = $"Json parse error: {ex.Message}\n{json}"; }
        });

        if (obj != null) done?.Invoke(true, obj, null);
        else done?.Invoke(false, null, err ?? "Unknown error");
    }

    string BuildDbUrl(string pathNoJson)
    {
        string trimmed = pathNoJson.Trim().TrimStart('/');         // leaderboards/global/abc
        if (!databaseUrl.EndsWith("/")) databaseUrl += "/";
        string baseUrl = databaseUrl;
        if (!baseUrl.StartsWith("http")) baseUrl = "https://" + baseUrl; // phòng nhập thiếu protocol
        return $"{baseUrl}{trimmed}.json?auth={IdToken}";
    }

    static double NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    static double SafeParseDouble(string s, double defVal)
        => double.TryParse(s, out var v) ? v : defVal;

    // ====== Public GET map (key động) ======
    public void GetMap<T>(string path, Action<bool, Dictionary<string, T>, string> done) where T : class, new()
        => StartCoroutine(GetMapCo(path, done));

    IEnumerator GetMapCo<T>(string path, Action<bool, Dictionary<string, T>, string> done) where T : class, new()
    {
        string json = null; string err = null;
        yield return GetRawCo(path, (ok, j, e) => { if (!ok) err = e; else json = j; });
        if (err != null) { done?.Invoke(false, null, err); yield break; }

        var dict = new Dictionary<string, T>();
        try
        {
            var root = MiniJson.Deserialize(json) as Dictionary<string, object>;
            if (root != null)
            {
                foreach (var kv in root)
                {
                    // value -> JSON string -> POCO
                    string vJson = MiniJson.Serialize(kv.Value);
                    var obj = string.IsNullOrEmpty(vJson) ? null : JsonUtility.FromJson<T>(vJson);
                    if (obj != null) dict[kv.Key] = obj;
                }
            }
        }
        catch (Exception ex) { done?.Invoke(false, null, $"Parse map error: {ex.Message}"); yield break; }

        done?.Invoke(true, dict, null);
    }

    // ====== Minimal JSON (Dictionary/List) ======
    static class MiniJson
    {
        public static object Deserialize(string json) => new Parser(json).Parse();
        public static string Serialize(object obj) => Serializer.Serialize(obj);

        sealed class Parser
        {
            readonly string _json; int _i;
            public Parser(string json) { _json = json ?? ""; _i = 0; }
            public object Parse() { Skip(); return ParseValue(); }

            char Peek => _i < _json.Length ? _json[_i] : '\0';
            char Next() => _i < _json.Length ? _json[_i++] : '\0';
            void Skip() { while (_i < _json.Length && char.IsWhiteSpace(_json[_i])) _i++; }

            object ParseValue()
            {
                Skip();
                char c = Peek;
                if (c == '{') return ParseObject();
                if (c == '[') return ParseArray();
                if (c == '"') return ParseString();
                if (char.IsDigit(c) || c == '-') return ParseNumber();
                if (Match("true")) return true;
                if (Match("false")) return false;
                if (Match("null")) return null;
                throw new Exception($"Unexpected token at {_i}");
            }
            Dictionary<string, object> ParseObject()
            {
                var d = new Dictionary<string, object>();
                Expect('{'); Skip();
                if (Peek == '}') { Next(); return d; }
                while (true)
                {
                    string key = ParseString(); Skip(); Expect(':'); Skip();
                    d[key] = ParseValue(); Skip();
                    if (Peek == '}') { Next(); break; }
                    Expect(','); Skip();
                }
                return d;
            }
            List<object> ParseArray()
            {
                var a = new List<object>();
                Expect('['); Skip();
                if (Peek == ']') { Next(); return a; }
                while (true)
                {
                    a.Add(ParseValue()); Skip();
                    if (Peek == ']') { Next(); break; }
                    Expect(','); Skip();
                }
                return a;
            }
            string ParseString()
            {
                var sb = new System.Text.StringBuilder();
                Expect('"');
                while (true)
                {
                    if (_i >= _json.Length) throw new Exception("Unterminated string");
                    char c = Next();
                    if (c == '"') break;
                    if (c == '\\')
                    {
                        c = Next();
                        if (c == '"' || c == '\\' || c == '/') sb.Append(c);
                        else if (c == 'b') sb.Append('\b');
                        else if (c == 'f') sb.Append('\f');
                        else if (c == 'n') sb.Append('\n');
                        else if (c == 'r') sb.Append('\r');
                        else if (c == 't') sb.Append('\t');
                        else if (c == 'u') { string h = _json.Substring(_i, 4); _i += 4; sb.Append((char)Convert.ToInt32(h, 16)); }
                        else throw new Exception("Invalid escape");
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }
            object ParseNumber()
            {
                int start = _i;
                if (Peek == '-') Next();
                while (char.IsDigit(Peek)) Next();
                if (Peek == '.') { Next(); while (char.IsDigit(Peek)) Next(); }
                if (Peek == 'e' || Peek == 'E') { Next(); if (Peek == '+' || Peek == '-') Next(); while (char.IsDigit(Peek)) Next(); }
                string s = _json.Substring(start, _i - start);
                if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
                throw new Exception("Bad number");
            }
            void Expect(char ch) { if (Next()!=ch) throw new Exception($"Expected '{ch}'"); }
            bool Match(string kw) { if (_json.AsSpan(_i).StartsWith(kw)) { _i += kw.Length; return true; } return false; }
        }

        sealed class Serializer
        {
            System.Text.StringBuilder _sb = new System.Text.StringBuilder();
            public static string Serialize(object obj) => new Serializer().Do(obj);
            string Do(object obj)
            {
                WriteValue(obj); return _sb.ToString();
            }
            void WriteValue(object obj)
            {
                switch (obj)
                {
                    case null: _sb.Append("null"); break;
                    case string s: WriteString(s); break;
                    case bool b: _sb.Append(b ? "true" : "false"); break;
                    case double or float or int or long or decimal:
                        _sb.Append(Convert.ToString(obj, System.Globalization.CultureInfo.InvariantCulture)); break;
                    case IDictionary dict: WriteObject(dict); break;
                    case IEnumerable list: WriteArray(list); break;
                    default:
                        // POCO → serialize bằng JsonUtility rồi embed vào stream
                        WriteRaw(JsonUtility.ToJson(obj));
                        break;
                }
            }
            void WriteString(string s)
            {
                _sb.Append('"');
                foreach (var c in s)
                {
                    switch (c)
                    {
                        case '"': _sb.Append("\\\""); break;
                        case '\\': _sb.Append("\\\\"); break;
                        case '\b': _sb.Append("\\b"); break;
                        case '\f': _sb.Append("\\f"); break;
                        case '\n': _sb.Append("\\n"); break;
                        case '\r': _sb.Append("\\r"); break;
                        case '\t': _sb.Append("\\t"); break;
                        default:
                            if (c < ' ') _sb.Append("\\u" + ((int)c).ToString("x4"));
                            else _sb.Append(c);
                            break;
                    }
                }
                _sb.Append('"');
            }
            void WriteObject(IDictionary dict)
            {
                _sb.Append('{'); bool first = true;
                foreach (DictionaryEntry kv in dict)
                {
                    if (!first) _sb.Append(',');
                    WriteString(kv.Key.ToString()); _sb.Append(':'); WriteValue(kv.Value);
                    first = false;
                }
                _sb.Append('}');
            }
            void WriteArray(IEnumerable list)
            {
                _sb.Append('['); bool first = true;
                foreach (var v in list)
                {
                    if (!first) _sb.Append(',');
                    WriteValue(v); first = false;
                }
                _sb.Append(']');
            }
            void WriteRaw(string raw) { _sb.Append(raw); }
        }
    }

}
