using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class LeaderboardUIBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] FirebaseLeaderboard leaderboard;
    [SerializeField] RectTransform contentRoot;
    [SerializeField] LeaderboardRowView rowPrefab;
    [SerializeField] ScrollRect scrollRect; // kéo ScrollView vào slot này

    [Header("Behavior")]
    [SerializeField, Range(5, 100)] int topCount = 20;
    [SerializeField] bool refreshOnEnable = true;

    [Tooltip("Tự fetch định kỳ khi panel đang mở (WebGL nên bật).")]
    [SerializeField] bool autoRefreshWhileVisible = true;
    [SerializeField, Range(2f, 60f)] float refreshIntervalSec = 8f;

    [Header("Style")]
    [SerializeField] Color rowOdd = new Color(1f, 1f, 1f, 0.06f);
    [SerializeField] Color rowEven = new Color(1f, 1f, 1f, 0.12f);

    readonly List<LeaderboardRowView> pool = new List<LeaderboardRowView>();
    Coroutine _loop;

    void OnEnable()
    {
        if (refreshOnEnable) StartCoroutine(Refresh());
        if (autoRefreshWhileVisible) _loop = StartCoroutine(AutoLoop());
    }
    void OnDisable()
    {
        if (_loop != null) { StopCoroutine(_loop); _loop = null; }
    }

    public void ForceRefresh() => StartCoroutine(Refresh());

    IEnumerator AutoLoop()
    {
        // làm realtime nhẹ nhàng
        while (enabled && gameObject.activeInHierarchy)
        {
            yield return new WaitForSecondsRealtime(refreshIntervalSec);
            yield return Refresh();
        }
    }

    IEnumerator Refresh()
    {
        if (leaderboard == null) leaderboard = FindObjectOfType<FirebaseLeaderboard>();
        if (leaderboard == null || contentRoot == null || rowPrefab == null) yield break;

        bool done = false;
        List<FirebaseLeaderboard.Row> rows = null;
        yield return leaderboard.FetchTop(topCount, r => { rows = r; done = true; });
        if (!done || rows == null) yield break;

        BuildRows(rows);
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
    }

    void BuildRows(List<FirebaseLeaderboard.Row> rows)
    {
        EnsurePool(rows.Count);

        for (int i = 0; i < pool.Count; i++) pool[i].gameObject.SetActive(false);

        for (int i = 0; i < rows.Count; i++)
        {
            var item = pool[i];
            item.gameObject.SetActive(true);
            item.Bind(i + 1, rows[i].name, rows[i].score);
            item.SetBackground((i % 2 == 0) ? rowEven : rowOdd);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
    }

    void EnsurePool(int need)
    {
        while (pool.Count < need)
        {
            var inst = Instantiate(rowPrefab, contentRoot);
            pool.Add(inst);
        }
    }
}
