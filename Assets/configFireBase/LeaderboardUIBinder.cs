using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class LeaderboardUIBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] FirebaseLeaderboard leaderboard;     // Kéo _Services(FirebaseLeaderboard) vào
    [SerializeField] RectTransform contentRoot;          // Kéo "Content" của ScrollView vào
    [SerializeField] LeaderboardRowView rowPrefab;       // Kéo prefab LBRowView vào

    [Header("Behavior")]
    [SerializeField, Range(5, 100)] int topCount = 20;
    [SerializeField] bool refreshOnEnable = true;
    [SerializeField] KeyCode manualRefreshKey = KeyCode.None;   // optional

    [Header("Style")]
    [SerializeField] Color rowOdd = new Color(1f, 1f, 1f, 0.06f);
    [SerializeField] Color rowEven = new Color(1f, 1f, 1f, 0.12f);

    readonly List<LeaderboardRowView> pool = new List<LeaderboardRowView>();

    void Reset() { topCount = 20; refreshOnEnable = true; }

    void OnEnable()
    {
        if (refreshOnEnable) StartCoroutine(Refresh());
    }

    void Update()
    {
        if (manualRefreshKey != KeyCode.None && Input.GetKeyDown(manualRefreshKey))
            StartCoroutine(Refresh());
    }

    public void ForceRefresh() => StartCoroutine(Refresh());

    IEnumerator Refresh()
    {
        if (leaderboard == null) leaderboard = FindObjectOfType<FirebaseLeaderboard>();
        if (leaderboard == null || contentRoot == null || rowPrefab == null) yield break;

        bool done = false;
        List<FirebaseLeaderboard.Row> rows = null;
        yield return leaderboard.FetchTop(topCount, r => { rows = r; done = true; });
        if (!done || rows == null) yield break;

        BuildRows(rows);
    }

    void BuildRows(List<FirebaseLeaderboard.Row> rows)
    {
        EnsurePool(rows.Count);

        // Ẩn hết trước
        for (int i = 0; i < pool.Count; i++) pool[i].gameObject.SetActive(false);

        // Bind
        for (int i = 0; i < rows.Count; i++)
        {
            var item = pool[i];
            item.gameObject.SetActive(true);
            item.Bind(i + 1, rows[i].name, rows[i].score);
            item.SetBackground((i % 2 == 0) ? rowEven : rowOdd);
        }

        // Rebuild layout cho chắc
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
        Canvas.ForceUpdateCanvases();
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
