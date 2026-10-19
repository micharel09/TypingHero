using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class EndlessOnlineTests
{
    // Loopback-only fixture: no production database is ever contacted by these tests.
    sealed class Fixture : IDisposable
    {
        readonly HttpListener listener = new HttpListener();
        readonly Task worker;
        internal readonly GameObject host;
        internal FirebaseLeaderboard Provider => host.GetComponent<FirebaseLeaderboard>();
        internal int reads, writes;
        internal Func<HttpListenerRequest, (int code, string body, string etag)> respond;
        internal Fixture()
        {
            var socket = new TcpListener(IPAddress.Loopback, 0); socket.Start();
            int port = ((IPEndPoint)socket.LocalEndpoint).Port; socket.Stop();
            string root = "http://127.0.0.1:" + port;
            listener.Prefixes.Add(root + "/"); listener.Start();
            host = new GameObject("Loopback Endless fixture"); host.AddComponent<FirebaseLeaderboard>();
            typeof(FirebaseLeaderboard).GetField("databaseRootUrl", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Provider, root);
            typeof(FirebaseLeaderboard).GetField("logs", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Provider, false);
            worker = Task.Run(async () =>
            {
                while (listener.IsListening)
                {
                    HttpListenerContext context;
                    try { context = await listener.GetContextAsync(); }
                    catch (HttpListenerException) { break; }
                    catch (ObjectDisposedException) { break; }
                    if (context.Request.HttpMethod == "GET") reads++; else writes++;
                    var result = respond(context.Request);
                    context.Response.StatusCode = result.code;
                    context.Response.ContentType = "application/json";
                    if (result.etag != null) context.Response.Headers.Add("ETag", result.etag);
                    byte[] data = Encoding.UTF8.GetBytes(result.body);
                    context.Response.ContentLength64 = data.Length;
                    await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
                    context.Response.Close();
                }
            });
        }
        public void Dispose() { listener.Close(); worker.GetAwaiter().GetResult(); UnityEngine.Object.DestroyImmediate(host); }
    }
    static EndlessLeaderboard.Entry Entry(int progress, int score, string uid = "a", long time = 1) =>
        new EndlessLeaderboard.Entry { name = "Player", uid = uid, defeated = progress, score = score, updatedAt = time };

    [UnityTest, Category("Quick")]
    public IEnumerator DifficultyPartitionsReadsWritesAndPendingSurvivesSelectionChange()
    {
        var saved = new Dictionary<string, string>();
        string selection = PlayerPrefs.GetString(EndlessDifficulties.SelectionKey, "normal");
        foreach (var difficulty in EndlessDifficulties.All)
        {
            string key = EndlessDifficulties.StorageKey(difficulty, EndlessLeaderboard.PendingKey);
            saved[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null;
            PlayerPrefs.DeleteKey(key);
        }
        try
        {
            using (var fixture = new Fixture())
            {
                foreach (var difficulty in EndlessDifficulties.All)
                {
                    string expected = "/" + EndlessDifficulties.Path(difficulty);
                    fixture.respond = request =>
                    {
                        Assert.That(request.Url.AbsolutePath, Does.StartWith(expected));
                        return request.HttpMethod == "GET" ? (200, "null", "\"partition\"") : (200, "{}", null);
                    };
                    bool submitted = false;
                    yield return fixture.Provider.SubmitEndless(Entry(2, 10), ok => submitted = ok, difficulty);
                    Assert.That(submitted, Is.True);
                    EndlessLeaderboard.Result result = null;
                    yield return fixture.Provider.FetchEndless(value => result = value, difficulty);
                    Assert.That(result.Success, Is.True);
                }
                fixture.respond = request => (503, "{}", null);
                foreach (var difficulty in EndlessDifficulties.All)
                    EndlessLeaderboard.Queue((int)difficulty + 1, 100, difficulty);
                yield return new WaitForSecondsRealtime(.3f);
                foreach (var difficulty in EndlessDifficulties.All)
                    Assert.That(EndlessLeaderboard.Pending(difficulty).defeated, Is.EqualTo((int)difficulty + 1));
                EndlessDifficulties.Selected = EndlessDifficulty.Hard;
                var paths = new HashSet<string>();
                fixture.respond = request =>
                {
                    if (request.HttpMethod == "PUT") paths.Add(request.Url.AbsolutePath);
                    return request.HttpMethod == "GET" ? (200, "null", "\"retry\"") : (200, "{}", null);
                };
                bool ok = false;
                yield return EndlessLeaderboard.Get().RetryPending(value => ok = value);
                Assert.That(ok, Is.True);
                Assert.That(paths.Count, Is.EqualTo(5));
                foreach (var difficulty in EndlessDifficulties.All)
                {
                    Assert.That(EndlessLeaderboard.Pending(difficulty), Is.Null);
                    Assert.That(paths, Has.Some.StartsWith("/" + EndlessDifficulties.Path(difficulty) + "/"));
                }
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(EndlessLeaderboard.Get().gameObject);
            foreach (var pair in saved)
                if (pair.Value == null) PlayerPrefs.DeleteKey(pair.Key); else PlayerPrefs.SetString(pair.Key, pair.Value);
            PlayerPrefs.SetString(EndlessDifficulties.SelectionKey, selection); PlayerPrefs.Save();
        }
    }

    [Test, Category("Quick")]
    public void TotalOrderUsesProgressScoreTimeThenUidWithoutOverflow()
    {
        var rows = new List<EndlessLeaderboard.Entry> { Entry(1, int.MaxValue), Entry(2, 1, "z", 5),
            Entry(2, 1, "b", 2), Entry(2, 1, "a", 2), Entry(2, 2) };
        rows.Sort(EndlessLeaderboard.Compare);
        Assert.That(rows[0].score, Is.EqualTo(2));
        Assert.That(rows[1].uid, Is.EqualTo("a")); Assert.That(rows[2].uid, Is.EqualTo("b"));
        Assert.That(rows[4].defeated, Is.EqualTo(1));
        Assert.That(EndlessLeaderboard.Better(Entry(1, int.MaxValue), Entry(2, 0)), Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PendingBestSurvivesNetworkErrorAndLowerResultThenRetries()
    {
        bool hadPending = PlayerPrefs.HasKey(EndlessLeaderboard.PendingKey);
        string previous = PlayerPrefs.GetString(EndlessLeaderboard.PendingKey);
        try
        {
            PlayerPrefs.DeleteKey(EndlessLeaderboard.PendingKey);
            using (var fixture = new Fixture())
            {
                fixture.respond = request => (503, "{}", null);
                EndlessLeaderboard.Queue(10, 100);
                yield return new WaitForSecondsRealtime(.2f);
                Assert.That(EndlessLeaderboard.Pending().defeated, Is.EqualTo(10));
                EndlessLeaderboard.Queue(9, int.MaxValue);
                yield return new WaitForSecondsRealtime(.2f);
                Assert.That(EndlessLeaderboard.Pending().defeated, Is.EqualTo(10));
                Assert.That(fixture.writes, Is.Zero);
                fixture.respond = request => request.HttpMethod == "GET" ? (200, "null", "\"v2\"") : (200, "{}", null);
                bool ok = false;
                yield return EndlessLeaderboard.Get().RetryPending(value => ok = value);
                Assert.That(ok, Is.True);
                Assert.That(EndlessLeaderboard.Pending(), Is.Null);
                Assert.That(fixture.writes, Is.EqualTo(1));
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(EndlessLeaderboard.Get().gameObject);
            if (hadPending) PlayerPrefs.SetString(EndlessLeaderboard.PendingKey, previous);
            else PlayerPrefs.DeleteKey(EndlessLeaderboard.PendingKey);
            PlayerPrefs.Save();
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ReadFailureCannotOverwriteRecord()
    {
        using (var fixture = new Fixture())
        {
            fixture.respond = request => (503, "{}", null);
            bool ok = true;
            yield return fixture.Provider.SubmitEndless(Entry(10, 99), value => ok = value);
            Assert.That(ok, Is.False); Assert.That(fixture.writes, Is.Zero);
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ConflictRereadsAndPreservesConcurrentBetterRecord()
    {
        using (var fixture = new Fixture())
        {
            fixture.respond = request =>
            {
                Assert.That(request.RawUrl, Does.StartWith("/leaderboards/endless_v1/"));
                if (request.HttpMethod == "GET") return (200, fixture.reads == 1 ? "null" : JsonUtility.ToJson(Entry(11, 1)), "\"v1\"");
                Assert.That(request.Headers["if-match"], Is.EqualTo("\"v1\""));
                return (412, "{}", null);
            };
            bool ok = false;
            yield return fixture.Provider.SubmitEndless(Entry(10, 999), value => ok = value);
            Assert.That(ok, Is.True); Assert.That(fixture.reads, Is.EqualTo(2)); Assert.That(fixture.writes, Is.EqualTo(1));
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator RepeatedConflictsStopAfterThreeConditionalWrites()
    {
        using (var fixture = new Fixture())
        {
            fixture.respond = request => request.HttpMethod == "GET" ? (200, "null", "\"v1\"") : (412, "{}", null);
            bool ok = true;
            yield return fixture.Provider.SubmitEndless(Entry(1, 1), value => ok = value);
            Assert.That(ok, Is.False); Assert.That(fixture.reads, Is.EqualTo(3)); Assert.That(fixture.writes, Is.EqualTo(3));
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator FetchIncludesEntireBoundaryTieBeforeCuttingTop1000()
    {
        using (var fixture = new Fixture())
        {
            fixture.respond = request =>
            {
                Assert.That(request.Url.AbsolutePath, Is.EqualTo("/leaderboards/endless_v1.json"));
                bool tie = request.QueryString["equalTo"] == "5";
                var json = new StringBuilder("{");
                int count = tie ? 1100 : 1000;
                for (int i = 0; i < count; i++)
                {
                    if (i > 0) json.Append(',');
                    json.Append('"').Append((tie ? "t" : "u") + i.ToString("D4")).Append("\":");
                    json.Append(JsonUtility.ToJson(Entry(!tie && i < 900 ? 6 : 5, tie ? 10000 + i : i)));
                }
                return (200, json.Append('}').ToString(), null);
            };
            EndlessLeaderboard.Result result = null;
            yield return fixture.Provider.FetchEndless(value => result = value);
            Assert.That(result.Success, Is.True); Assert.That(fixture.reads, Is.EqualTo(2));
            Assert.That(result.entries.Count, Is.EqualTo(1000));
            Assert.That(result.entries[900].score, Is.EqualTo(11099));
            Assert.That(result.entries[999].score, Is.EqualTo(11000));
            Assert.That(fixture.writes, Is.Zero);
        }
    }
}
