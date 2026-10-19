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

    [SerializeField] TMP_Text title, detailText, scoreHeader;
    [SerializeField] GameObject tableHeader;
    [SerializeField] EndlessDifficultySelector difficultySelector;
    [SerializeField] GameObject difficultyControls;
    public EndlessDifficulty Difficulty { get; private set; } = EndlessDifficulty.Normal;
    int mode;
    int requestVersion;
    List<EndlessLeaderboard.Entry> endlessRows;
    public void ShowEndless() => SelectMode(0);
    public void ShowClassic() => SelectMode(1);
    public void ShowProfiles() => SelectMode(2);
    public void ShowAchievements() => SelectMode(3);
    public void SelectDifficulty(EndlessDifficulty value)
    {
        Difficulty = value;
        if (difficultySelector) difficultySelector.Display(value);
        if (isActiveAndEnabled) SelectMode(0);
    }
    void SelectMode(int value)
    {
        mode = value; requestVersion++; ClearDisplayedRows();
        StopAllCoroutines(); _refreshing = false;
        _positionInitialized = false;
        if (scrollRect) scrollRect.verticalNormalizedPosition = 1;
        StartCoroutine(Refresh());
        if (autoRefreshWhileVisible) _loop = StartCoroutine(AutoLoop());
    }
    readonly List<LeaderboardRowView> pool = new List<LeaderboardRowView>();
    readonly List<FirebaseLeaderboard.Row> _rows = new List<FirebaseLeaderboard.Row>();
    float _rowHeight;
    int _firstVisible = -1;
    Coroutine _loop;
    bool _refreshing; // Chống chồng chéo
    bool _positionInitialized;

    void ClearDisplayedRows()
    {
        _rows.Clear(); endlessRows = null; _firstVisible = -1;
        foreach (var item in pool) item.gameObject.SetActive(false);
        if (contentRoot) contentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    void OnEnable()
    {
        if (virtualizeRows && scrollRect && rowPrefab)
        {
            _rowHeight = ((RectTransform)rowPrefab.transform).rect.height;
            scrollRect.onValueChanged.AddListener(OnScroll);
        }
        mode = 0;
        Difficulty = EndlessRun.Current != null ? EndlessRun.Current.Difficulty : EndlessDifficulties.Selected;
        if (difficultySelector) difficultySelector.Display(Difficulty);
        ClearDisplayedRows();
        if (refreshOnEnable) StartCoroutine(Refresh());
        if (autoRefreshWhileVisible) _loop = StartCoroutine(AutoLoop());
    }
    void OnDisable()
    {
        requestVersion++; StopAllCoroutines(); _refreshing = false;
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
            int version = requestVersion;
            if (difficultyControls) difficultyControls.SetActive(mode == 0);
            if (title) title.text = mode == 0 ? "ENDLESS / COMMUNITY" : mode == 1 ? "CLASSIC" : mode == 2 ? "OPPONENTS" : "ACHIEVEMENTS";
            if (detailText) detailText.gameObject.SetActive(mode >= 2);
            if (tableHeader) tableHeader.SetActive(mode < 2);
            if (scoreHeader) scoreHeader.text = mode == 0 ? "KILLS / SCORE" : "SCORE";
            if (mode >= 2)
            {
                foreach (var item in pool) item.gameObject.SetActive(false);
                if (emptyLabel) emptyLabel.gameObject.SetActive(false);
                string body = "";
                if (mode == 2)
                {
                    foreach (EndlessRole role in System.Enum.GetValues(typeof(EndlessRole)))
                    {
                        int seen = EndlessProfile.Encounters(role);
                        var definition = Resources.Load<EndlessOpponent>("Endless/" + role);
                        body += seen == 0 ? "??? / NOT ENCOUNTERED\n\n" :
                            $"{definition.displayName} / SEEN {seen} / DEFEATED {EndlessProfile.Defeats(role)}\n{definition.hint}\nElite: seen {EndlessProfile.Elites(role, false)} / defeated {EndlessProfile.Elites(role, true)}. Same attacks, faster recovery.\n\n";
                    }
                }
                else
                {
                    string[] keys = { EndlessProfile.FirstBoss, EndlessProfile.TripleParry, EndlessProfile.PerfectBoss };
                    string[] descriptions = { "FIRST CAPTAIN: Defeat your first boss.", "THREE PARRIES: Parry every strike in a triple chain.", "UNTOUCHED: Defeat a boss without losing HP in that duel." };
                    for (int i = 0; i < keys.Length; i++) body += (EndlessProfile.Has(keys[i]) ? "UNLOCKED: " : "LOCKED: ") + descriptions[i] + "\n\n";
                }
                if (detailText)
                {
                    detailText.text = body; detailText.ForceMeshUpdate();
                    float height = detailText.GetPreferredValues(body, detailText.rectTransform.rect.width, 0).y + 24;
                    detailText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    contentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                }
                yield break;
            }
            if (mode == 0)
            {
                foreach (var item in pool) item.gameObject.SetActive(false);
                if (emptyLabel) { emptyLabel.text = "Loading..."; emptyLabel.gameObject.SetActive(true); }
                EndlessLeaderboard.Result result = null;
                yield return EndlessLeaderboard.Get().Fetch(value => result = value, Difficulty);
                if (version != requestVersion || !isActiveAndEnabled) yield break;
                if (result == null || !result.Success)
                {
                    if (emptyLabel) emptyLabel.text = result?.error ?? "Could not load. Retry with Refresh.";
                    yield break;
                }
                endlessRows = result.entries;
                var display = new List<FirebaseLeaderboard.Row>();
                foreach (var entry in endlessRows) display.Add(new FirebaseLeaderboard.Row { name = entry.name, score = entry.score });
                if (emptyLabel) emptyLabel.text = "No Endless results yet.";
                BuildRows(display);
                yield break;
            }
            bool done = false;
            List<FirebaseLeaderboard.Row> rows = null;
            yield return leaderboard.FetchTop(topCount, r => { rows = r; done = true; });

            if (!done) yield break;
            if (version != requestVersion || !isActiveAndEnabled) yield break;

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

    void OnScroll(Vector2 _) { if (mode < 2) RenderWindow(false); }

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
            if (mode == 0 && endlessRows != null) item.BindEndless(index + 1, endlessRows[index]);
            else item.Bind(index + 1, _rows[index].name, _rows[index].score);
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
