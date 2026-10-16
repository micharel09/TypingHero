using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class PixelUILeaderboard
{
    internal static void Build(LeaderboardUIBinder binder, Transform parent)
    {
        var panel = PanelAt("LeaderboardPanel", parent, 712, 16, 232, 508);
        Label("Title", "TOP 1000", panel, 16, 12, 200, 40, 32);
        var header = Rect("Header", panel); Box(header, panel, 12, 60, 200, 28);
        Columns(header, true);
        var scrollRt = Rect("ScrollView", panel); Box(scrollRt, panel, 8, 92, 216, 404);
        Image(scrollRt, new Color(0, 0, 0, 0)).raycastTarget = true;
        var scroll = scrollRt.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false; scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 24;
        var viewport = Rect("Viewport", scrollRt); Box(viewport, scrollRt, 0, 0, 200, 404);
        viewport.gameObject.AddComponent<RectMask2D>();
        var content = Rect("Content", viewport);
        content.anchorMin = new Vector2(0, 1); content.anchorMax = Vector2.one;
        content.pivot = new Vector2(.5f, 1); content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;
        var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.childControlWidth = true; layout.childControlHeight = true;
        layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
        var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var track = Rect("Scrollbar", scrollRt); Box(track, scrollRt, 204, 0, 12, 404);
        Image(track, Ink).raycastTarget = true;
        var handle = Rect("Handle", track); Stretch(handle, track, 2);
        var graphic = Image(handle, Muted, true); graphic.raycastTarget = true;
        var scrollbar = track.gameObject.AddComponent<Scrollbar>();
        scrollbar.handleRect = handle; scrollbar.targetGraphic = graphic;
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.numberOfSteps = 0; scrollbar.size = 1;
        scroll.viewport = viewport; scroll.content = content; scroll.verticalScrollbar = scrollbar;
        scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        var row = Rect("LBRowView", null); row.sizeDelta = new Vector2(200, 28);
        var bg = Image(row, Panel); bg.raycastTarget = true;
        var size = row.gameObject.AddComponent<LayoutElement>(); size.preferredHeight = 28;
        var view = row.gameObject.AddComponent<LeaderboardRowView>();
        var labels = Columns(row, false);
        Set(view, "rankText", labels[0]); Set(view, "nameText", labels[1]);
        Set(view, "scoreText", labels[2]); Set(view, "background", bg);
        var prefab = PrefabUtility.SaveAsPrefabAsset(row.gameObject, Generated + "LeaderboardRow.prefab");
        Object.DestroyImmediate(row.gameObject);
        Set(binder, "contentRoot", content); Set(binder, "scrollRect", scroll);
        Set(binder, "rowPrefab", prefab.GetComponent<LeaderboardRowView>());
        Value(binder, "rowEven", Hex("293F4D")); Value(binder, "rowOdd", Hex("223542"));
        Value(binder, "topCount", 1000);
        var empty = Label("EmptyLeaderboard", "NO SCORES YET", viewport, 8, 16, 184, 32, 22);
        empty.alignment = TextAlignmentOptions.Center; empty.color = Muted;
        empty.gameObject.SetActive(false); Set(binder, "emptyLabel", empty);
    }

    static TMP_Text[] Columns(Transform parent, bool header)
    {
        var rank = Label("Rank", header ? "#" : "1000", parent, 4, 0, 36, 28, 20);
        var name = Label("Name", header ? "NAME" : "Player", parent, 44, 0, 84, 28, 20);
        name.richText = false;
        var score = Label("Score", header ? "SCORE" : "0", parent, 132, 0, 64, 28, 20);
        score.alignment = TextAlignmentOptions.MidlineRight;
        score.enableAutoSizing = true; score.fontSizeMin = 12; score.fontSizeMax = 20;
        rank.color = header ? Muted : Gold;
        if (header) { name.color = Muted; score.color = Muted; }
        return new TMP_Text[] { rank, name, score };
    }
}
