using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class EndlessCollectionBuild
{
    internal static void Apply(LeaderboardUIBinder binder, RectTransform bounds, RectTransform panel)
    {
        Box(panel, bounds, 180, 0, 600, 540);
        Box(panel.Find("Title").GetComponent<RectTransform>(), panel, 16, 12, 568, 40);
        Set(binder, "title", panel.Find("Title").GetComponent<TMP_Text>());
        var empty = Get<TMP_Text>(binder, "emptyLabel");
        Box(empty.rectTransform, Get<ScrollRect>(binder, "scrollRect").viewport, 8, 16, 552, 80);
        empty.enableWordWrapping = true;
        var list = Get<ScrollRect>(binder, "scrollRect");
        Box(list.GetComponent<RectTransform>(), panel, 8, 128, 584, 404);
        Box(list.viewport, list.transform, 0, 0, 568, 404);
        Box(list.transform.Find("Scrollbar").GetComponent<RectTransform>(), list.transform, 572, 0, 12, 404);
        var header = panel.Find("Header");
        Box(header.GetComponent<RectTransform>(), panel, 8, 96, 568, 28);
        var selector = EndlessDifficultyBuild.Add(panel, 16, 52, 568, binder);
        Set(binder, "difficultySelector", selector);
        Set(binder, "difficultyControls", selector.transform.parent.gameObject);
        Box(header.Find("Name").GetComponent<RectTransform>(), header, 44, 0, 244, 28);
        Box(header.Find("Score").GetComponent<RectTransform>(), header, 292, 0, 272, 28);
        header.Find("Score").GetComponent<TMP_Text>().text = "KILLS / SCORE";
        Set(binder, "scoreHeader", header.Find("Score").GetComponent<TMP_Text>());
        Set(binder, "tableHeader", header.gameObject);
        var row = PrefabUtility.LoadPrefabContents(PixelUIFactory.Generated + "LeaderboardRow.prefab");
        try
        {
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(568, 28);
            Box(row.transform.Find("Name").GetComponent<RectTransform>(), row.transform, 44, 0, 244, 28);
            Box(row.transform.Find("Score").GetComponent<RectTransform>(), row.transform, 292, 0, 272, 28);
            PrefabUtility.SaveAsPrefabAsset(row, PixelUIFactory.Generated + "LeaderboardRow.prefab");
        }
        finally { PrefabUtility.UnloadPrefabContents(row); }
        var detail = list.content.Find("ProfileDetails")?.GetComponent<TMP_Text>();
        if (!detail) detail = Label("ProfileDetails", "", list.content, 8, 8, 552, 380, 20);
        detail.enableWordWrapping = true; detail.richText = false;
        Set(binder, "detailText", detail); detail.gameObject.SetActive(false);
        if (!bounds.Find("EndlessButton"))
        {
            Button("Endless", bounds, 800, 112, 144, binder.ShowEndless);
            Button("Classic", bounds, 800, 160, 144, binder.ShowClassic);
            Button("Opponents", bounds, 800, 208, 144, binder.ShowProfiles);
            Button("Achievements", bounds, 800, 256, 144, binder.ShowAchievements);
        }
        foreach (var name in new[] { "EndlessButton", "ClassicButton", "OpponentsButton", "AchievementsButton" })
        {
            var label = bounds.Find(name).GetComponentInChildren<TMP_Text>(true);
            label.fontSize = 20; label.enableWordWrapping = false;
        }
    }
    static T Get<T>(Object target, string name) where T : Object
    {
        var serialized = new SerializedObject(target);
        return serialized.FindProperty(name).objectReferenceValue as T;
    }
}
