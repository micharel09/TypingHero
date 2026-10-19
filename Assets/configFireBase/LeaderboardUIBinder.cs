using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class LeaderboardUIBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] FirebaseLeaderboard leaderboard;
    [SerializeField] RectTransform contentRoot;
    [SerializeField] LeaderboardRowView rowPrefab;
    [SerializeField] ScrollRect scrollRect; // kéo ScrollView vào slot này
    [SerializeField] TMP_Text emptyLabel;

    [Header("Behavior")]
    [SerializeField, Range(5, 1000)] int topCount = 1000;
    [SerializeField] bool refreshOnEnable = true;
    [SerializeField] bool virtualizeRows;

    [Tooltip("Tự fetch định kỳ khi panel đang mở (WebGL nên bật).")]
    [SerializeField] bool autoRefreshWhileVisible = true;
    [SerializeField, Range(2f, 60f)] float refreshIntervalSec = 8f;

    [Header("Style")]
    [SerializeField] Color rowOdd = new Color(1f, 1f, 1f, 0.06f);
    [SerializeField] Color rowEven = new Color(1f, 1f, 1f, 0.12f);

    readonly List<LeaderboardRowView> pool = new List<LeaderboardRowView>();
    readonly List<FirebaseLeaderboard.Row> _rows = new List<FirebaseLeaderboard.Row>();
    float _rowHeight;
    int _firstVisible = -1;
    Coroutine _loop;
    bool _refreshing; // Chống chồng chéo
    bool _positionInitialized;

    void OnEnable()
    {
        if (virtualizeRows && scrollRect && rowPrefab)
        {
            _rowHeight = ((RectTransform)rowPrefab.transform).rect.height;
            scrollRect.onValueChanged.AddListener(OnScroll);
        }
        if (refreshOnEnable) StartCoroutine(Refresh());
        if (autoRefreshWhileVisible) _loop = StartCoroutine(AutoLoop());
    }
    void OnDisable()
    {
        if (scrollRect) scrollRect.onValueChanged.RemoveListener(OnScroll);
        if (_loop != null) { StopCoroutine(_loop); _loop = null; }
    }

    public void ForceRefresh()
    {
        // Chỉ refresh nếu không đang trong quá trình refresh khác
        if (!_refreshing)
            StartCoroutine(Refresh());
    }

    IEnumerator AutoLoop()
    {
        // làm realtime nhẹ nhàng
        while (enabled && gameObject.activeInHierarchy)
        {
            yield return new WaitForSecondsRealtime(refreshIntervalSec);

            // Chỉ refresh nếu không đang refresh từ nguồn khác
            if (!_refreshing)
                yield return Refresh();
        }
    }

    IEnumerator Refresh()
    {
        if (leaderboard == null) leaderboard = FindObjectOfType<FirebaseLeaderboard>();
        if (leaderboard == null || contentRoot == null || rowPrefab == null) yield break;

        _refreshing = true;
        try
        {
            bool done = false;
            List<FirebaseLeaderboard.Row> rows = null;
            yield return leaderboard.FetchTop(topCount, r => { rows = r; done = true; });

            if (!done) yield break;

            // Nếu rows == null hoặc empty, có thể do throttle/đang fetch
            // FirebaseLeaderboard đã trả về cache nên không cần xóa UI
            if (rows != null && rows.Count >= 0) // Chấp nhận cả empty list
            {
                BuildRows(rows);
                Canvas.ForceUpdateCanvases();
                if (!_positionInitialized && rows.Count > 0)
                {
                    if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
                    _positionInitialized = true;
                }
            }
        }
        finally
        {
            _refreshing = false;
        }
    }

    void BuildRows(List<FirebaseLeaderboard.Row> rows)
    {
        if (emptyLabel) emptyLabel.gameObject.SetActive(rows.Count == 0);
        _rows.Clear();
        _rows.AddRange(rows);
        if (virtualizeRows)
        {
            _rowHeight = ((RectTransform)rowPrefab.transform).rect.height;
            var position = contentRoot.anchoredPosition;
            contentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows.Count * _rowHeight);
            position.y = Mathf.Clamp(position.y, 0,
                Mathf.Max(0, rows.Count * _rowHeight - scrollRect.viewport.rect.height));
            contentRoot.anchoredPosition = position;
            RenderWindow(true);
            return;
        }
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

    void OnScroll(Vector2 _) => RenderWindow(false);

    void RenderWindow(bool refresh)
    {
        // Two spare rows on either side keep wheel/drag scrolling continuous.
        const int overscan = 2;
        int first = _rows.Count == 0 ? 0 : Mathf.Clamp(
            Mathf.FloorToInt(contentRoot.anchoredPosition.y / _rowHeight) - overscan, 0, _rows.Count - 1);
        if (!refresh && first == _firstVisible) return;
        _firstVisible = first;
        int capacity = Mathf.CeilToInt(scrollRect.viewport.rect.height / _rowHeight) + overscan * 2;
        int count = Mathf.Min(capacity, _rows.Count - first);
        EnsurePool(count);
        for (int i = 0; i < pool.Count; i++)
        {
            var item = pool[i];
            item.gameObject.SetActive(i < count);
            if (i >= count) continue;
            int index = first + i;
            var rt = (RectTransform)item.transform;
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(.5f, 1);
            rt.sizeDelta = new Vector2(0, _rowHeight);
            rt.anchoredPosition = new Vector2(0, -index * _rowHeight);
            item.Bind(index + 1, _rows[index].name, _rows[index].score);
            item.SetBackground(index % 2 == 0 ? rowEven : rowOdd);
        }
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
