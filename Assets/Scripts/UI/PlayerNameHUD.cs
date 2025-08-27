using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerNameHUD : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] FirebaseLeaderboard leaderboard;
    [SerializeField] TextMeshProUGUI target;

    [Header("Format")]
    [SerializeField] string prefix = "";          // VD: "PLAYER: "
    [SerializeField] bool upperCase = false;
    [SerializeField] bool hideWhenEmpty = true;

    [Header("Rank")]
    [Tooltip("Hiển thị rank bên cạnh tên nếu có điểm")]
    [SerializeField] bool showRank = true;

    [Tooltip("Chuỗi format khi có rank. {NAME} = tên, {RANK} = số thứ hạng")]
    [SerializeField] string rankFormat = "{NAME}  (#{RANK})";

    [Tooltip("Chuỗi format khi chưa có rank (chưa có điểm). {NAME} = tên")]
    [SerializeField] string noRankFormat = "{NAME}";

    [Tooltip("Tự refresh rank khi bật object")]
    [SerializeField] bool refreshRankOnEnable = true;

    [Tooltip("Tự refresh rank theo chu kỳ (giây). 0 = tắt")]
    [SerializeField, Range(0, 120)] int autoRefreshIntervalSec = 10;

    Coroutine _rankLoop;
    int? _lastRank;   // cache rank gần nhất

    void Reset()
    {
        target = GetComponentInChildren<TextMeshProUGUI>();
        if (!leaderboard) leaderboard = FindObjectOfType<FirebaseLeaderboard>();
    }

    void OnEnable()
    {
        PlayerIdentity.EnsureLoaded();
        PlayerIdentity.OnNameChanged += HandleNameChanged;

        ApplyText(PlayerIdentity.Name, _lastRank);

        if (showRank && refreshRankOnEnable) RefreshRankOnce();

        if (showRank && autoRefreshIntervalSec > 0)
            _rankLoop = StartCoroutine(AutoRefreshRankLoop());
    }

    void OnDisable()
    {
        PlayerIdentity.OnNameChanged -= HandleNameChanged;
        if (_rankLoop != null) { StopCoroutine(_rankLoop); _rankLoop = null; }
    }

    void HandleNameChanged(string newName)
    {
        ApplyText(newName, _lastRank);
        // Không bắt buộc: có thể refresh rank ngay sau khi đổi tên
        // nếu bạn muốn UI luôn sync tức thì:
        // if (showRank) RefreshRankOnce();
    }

    public void RefreshRankOnce()
    {
        if (!showRank || leaderboard == null) return;
        StartCoroutine(CoRefreshRank());
    }

    IEnumerator AutoRefreshRankLoop()
    {
        var wait = new WaitForSecondsRealtime(Mathf.Max(1, autoRefreshIntervalSec));
        while (true)
        {
            yield return CoRefreshRank();
            yield return wait;
        }
    }

    IEnumerator CoRefreshRank()
    {
        int? rank = null;
        yield return leaderboard.FetchMyRank(r => rank = r);
        _lastRank = rank;
        ApplyText(PlayerIdentity.Name, _lastRank);
    }

    void ApplyText(string rawName, int? rank)
    {
        if (!target) return;

        string name = rawName ?? string.Empty;
        if (upperCase) name = name.ToUpperInvariant();

        bool empty = string.IsNullOrWhiteSpace(name);
        if (hideWhenEmpty)
        {
            target.gameObject.SetActive(!empty);
            if (empty) return;
        }
        else
        {
            target.gameObject.SetActive(true);
            if (empty) name = "Guest";
        }

        if (showRank)
        {
            if (rank.HasValue)
            {
                // Có rank
                target.text = $"{prefix}{rankFormat.Replace("{NAME}", name).Replace("{RANK}", rank.Value.ToString())}";
            }
            else
            {
                // Chưa có rank (chưa có điểm)
                target.text = $"{prefix}{noRankFormat.Replace("{NAME}", name)}";
            }
        }
        else
        {
            // Chỉ hiện tên
            target.text = $"{prefix}{name}";
        }
    }
}
