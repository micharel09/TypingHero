using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

// Typography-only bounds adjustments, using the existing controls and handlers.
internal static class BlackletterUILayout
{
    // Reference960x540: 18.7 / 16 / 14 CSS-pixel em at 320x180.
    internal const float HeadingSize = 56, BodySize = 48, CompactSize = 42;
    static RectTransform At(Transform parent, string path) => parent.Find(path) as RectTransform;
    static void Place(Transform parent, string path, float x, float y, float w, float h)
    {
        var rect = At(parent, path);
        if (!rect) return;
        Box(rect, rect.parent, x, y, w, h);
        PrefabUtility.RecordPrefabInstancePropertyModifications(rect);
    }
    static void Size(TMP_Text text, float size, bool wrap = false)
    {
        if (!text) return;
        text.enableAutoSizing = false;
        text.fontSize = text.fontSizeMin = text.fontSizeMax = size;
        text.enableWordWrapping = wrap;
        EditorUtility.SetDirty(text);
        PrefabUtility.RecordPrefabInstancePropertyModifications(text);
    }
    static void ButtonAt(Transform parent, string name, float x, float y, float w, float h, float size = BodySize)
    {
        var rect = At(parent, name);
        if (!rect) return;
        Box(rect, parent, x, y, w, h);
        var label = rect.GetComponentInChildren<TMP_Text>(true);
        Box(label.rectTransform, rect, 8, 0, w - 16, h);
        Size(label, size);
        PrefabUtility.RecordPrefabInstancePropertyModifications(rect);
        PrefabUtility.RecordPrefabInstancePropertyModifications(label.rectTransform);
    }

    internal static void Scene(GameObject[] roots)
    {
        foreach (var root in roots)
        {
            var design = root.transform.Find("Reference960x540");
            if (!design) continue;
            if (root.name == "PixelUI" && design.Find("MenuPanel")) Menu(design);
            if (root.name == "HowToPlay") Help(design);
            if (root.name == "SettingsModal") CompactMenusBuild.Style(design);
            if (root.name == "LeaderboardModal") Collection(design);
            if (root.name == "PixelOverlay") Results(design);
            if (root.name == "PixelUI" && design.Find("HUD")) Combat(design);
        }
    }

    static void Difficulty(Transform parent, float x, float y, float width)
    {
        var root = At(parent, "Difficulty");
        if (!root) return;
        Box(root, parent, x, y, width, 48);
        Place(root, "Caption", 0, 0, 220, 48);
        Size(root.Find("Caption").GetComponent<TMP_Text>(), BodySize);
        // Cobblestone arrows use the normal body size in the existing controls.
        ButtonAt(root, "<Button", 220, 0, 48, 48);
        ButtonAt(root, ">Button", width - 48, 0, 48, 48);
        var selector = root.GetComponentInChildren<EndlessDifficultySelector>(true);
        var rt = selector.GetComponent<RectTransform>();
        Box(rt, root, 268, 0, width - 316, 48);
        var caption = selector.GetComponentInChildren<TMP_Text>(true);
        Box(caption.rectTransform, rt, 8, 0, width - 332, 48);
        Size(caption, BodySize);
    }

    static void Menu(Transform design)
    {
        DungeonMainMenuBuild.Apply(design);
    }

    static void Help(Transform design)
    {
        var panel = design.Find("HowToPlayPanel");
        if (!panel) return;
        Box((RectTransform)panel, design, 48, 24, 864, 492);
        Place(panel, "Title", 24, 16, 816, 60);
        Size(panel.Find("Title").GetComponent<TMP_Text>(), HeadingSize);
        Place(panel, "Instructions", 24, 88, 816, 316);
        Size(panel.Find("Instructions").GetComponent<TMP_Text>(), BodySize, true);
        ButtonAt(panel, "ContinueButton", 24, 424, 816, 52);
    }

    static void Collection(Transform design)
    {
        var binder = Object.FindObjectOfType<LeaderboardUIBinder>(true);
        if (!binder) return;
        var so = new SerializedObject(binder);
        var list = (ScrollRect)so.FindProperty("scrollRect").objectReferenceValue;
        var panel = (RectTransform)list.transform.parent;
        // One centered 840x444 composition, retaining the existing modal input shield.
        Box(panel, design, 60, 48, 640, 444);
        Place(panel, "Title", 16, 8, 608, 38);
        Size(panel.Find("Title").GetComponent<TMP_Text>(), 30);
        Difficulty(panel, 16, 48, 608);
        var difficulty = panel.Find("Difficulty");
        Box((RectTransform)difficulty, panel, 16, 48, 608, 36);
        Place(difficulty, "Caption", 0, 0, 180, 36);
        ButtonAt(difficulty, "<Button", 180, 0, 36, 36, 24);
        ButtonAt(difficulty, ">Button", 572, 0, 36, 36, 24);
        var selector = difficulty.GetComponentInChildren<EndlessDifficultySelector>(true);
        Box((RectTransform)selector.transform, difficulty, 216, 0, 356, 36);
        foreach (var text in difficulty.GetComponentsInChildren<TMP_Text>(true)) Size(text, 24);
        var caption = selector.GetComponentInChildren<TMP_Text>(true);
        Box(caption.rectTransform, selector.transform, 8, 0, 340, 36);
        Place(panel, "Header", 16, 88, 608, 36);
        var header = panel.Find("Header");
        Place(header, "Rank", 0, 0, 72, 36);
        Place(header, "Name", 80, 0, 344, 36);
        Place(header, "Score", 432, 0, 176, 36);
        foreach (var label in header.GetComponentsInChildren<TMP_Text>(true)) Size(label, 24);
        header.Find("Score").GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineRight;
        Box(list.GetComponent<RectTransform>(), panel, 16, 132, 608, 288);
        Box(list.viewport, list.transform, 0, 0, 592, 288);
        Place(list.transform, "Scrollbar", 596, 0, 12, 288);
        var detail = (TMP_Text)so.FindProperty("detailText").objectReferenceValue;
        Size(detail, 27, true);
        var empty = (TMP_Text)so.FindProperty("emptyLabel").objectReferenceValue;
        Size(empty, 27, true);
        empty.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 160);
        var names = new[] { "CloseButton", "RefreshButton", "EndlessButton", "ClassicButton", "OpponentsButton", "AchievementsButton" };
        for (int i = 0; i < names.Length; i++) ButtonAt(design, names[i], 724, 64 + i * 52, 176, 36, 24);
        CompactMenusBuild.Skin(design);
        so.FindProperty("activeModeFrame").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(CombatR7Build.Art + "DarkFantasy/secondary-frame-gold.asset");
        so.FindProperty("inactiveModeFrame").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(CombatR7Build.Art + "DarkFantasy/secondary-frame.asset");
        so.FindProperty("rowEven").colorValue = Color.white;
        so.FindProperty("rowOdd").colorValue = Hex("c5b9a7");
        so.ApplyModifiedPropertiesWithoutUndo();

    }

    internal static void Prefab(GameObject root)
    {
        if (!root.GetComponent<LeaderboardRowView>()) return;
        var rect = root.GetComponent<RectTransform>();
        if (!rect || !root.transform.Find("Name") || !root.transform.Find("Score")) return;
        CompactMenusBuild.Skin(root.transform);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 36);
        var layout = root.GetComponent<LayoutElement>();
        if (layout) layout.preferredHeight = 36;
        Place(root.transform, "Rank", 0, 0, 72, 36);
        Place(root.transform, "Name", 80, 0, 336, 36);
        Place(root.transform, "Score", 424, 0, 168, 36);
        foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            Size(label, 24);
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.alignment = label.name == "Score" ? TextAlignmentOptions.MidlineRight : TextAlignmentOptions.MidlineLeft;
        }
    }

    static void Results(Transform design)
    {
        var panel = design.Find("CombatOverlay/OverlayPanel");
        if (!panel) return;
        Box((RectTransform)panel, panel.parent, 180, 87, 600, 366);
        foreach (string name in new[] { "PauseLabel", "ThanksLabel" })
        {
            Place(panel, name, 24, 16, 552, 40);
            var title = panel.Find(name).GetComponent<TMP_Text>();
            Size(title, 30);
            title.alignment = TextAlignmentOptions.Midline;
        }
        // Keep the existing summary reference for diagnostic consumers, not visual layout.
        panel.Find("MatchResult").gameObject.SetActive(false);
        ResultContent(panel);
        ButtonAt(panel, "ResumeButton", 24, 264, 268, 36, 24);
        ButtonAt(panel, "RestartButton", 24, 264, 304, 36, 24);
        ButtonAt(panel, "Main MenuButton", 344, 264, 232, 36, 24);
        // Results share the existing Settings modal instead of embedding sliders.
        CompactMenusBuild.Settings(design, panel, true);
        ButtonAt(panel, "Top 1000Button", 110, 314, 180, 32, 20);
        ButtonAt(panel, "SettingsButton", 310, 314, 180, 32, 20);
        panel.Find("SettingsButton").gameObject.SetActive(true);
        foreach (string name in new[] { "AudioPreferences", "KeyHint", "RestartButton/key-r" })
            panel.Find(name).gameObject.SetActive(false);
    }

    static void ResultContent(Transform panel)
    {
        var group = At(panel, "ResultContent") ?? Rect("ResultContent", panel);
        Box(group, panel, 24, 54, 552, 194);
        var view = panel.GetComponent<CombatResultView>() ?? panel.gameObject.AddComponent<CombatResultView>();
        var serialized = new SerializedObject(view);
        serialized.FindProperty("content").objectReferenceValue = group.gameObject;
        var template = panel.Find("MatchResult").GetComponent<TMP_Text>();
        TMP_Text Text(string name, string caption, float x, float y, float width, float height,
            float size, TextAlignmentOptions align, bool muted = false)
        {
            var node = group.Find(name);
            var label = node ? node.GetComponent<TMP_Text>()
                : Label(name, caption, group, x, y, width, height, size);
            Box(label.rectTransform, group, x, y, width, height);
            label.font = template.font; label.fontSharedMaterial = template.fontSharedMaterial;
            label.text = caption; label.alignment = align;
            label.color = Hex(muted ? "b8ac94" : "f2e7cf");
            // The pixel font has negative left bearings on several capitals.
            label.margin = new Vector4(2, 0, 2, 0); label.raycastTarget = false;
            label.overflowMode = TextOverflowModes.Ellipsis;
            Size(label, size);
            return label;
        }
        void Bind(string field, TMP_Text label) => serialized.FindProperty(field).objectReferenceValue = label;
        Bind("context", Text("Context", "", 0, 0, 552, 24, 20, TextAlignmentOptions.Midline, true));
        Text("ScoreLabel", "SCORE", 0, 28, 552, 22, 20, TextAlignmentOptions.Midline, true);
        Bind("score", Text("Score", "0", 0, 50, 552, 44, 40, TextAlignmentOptions.Midline));
        void Stat(string field, string name, string caption, float x, float y)
        {
            Text(name + "Label", caption, x, y, 140, 30, 20, TextAlignmentOptions.MidlineLeft, true);
            Bind(field, Text(name, "0", x + 140, y, 112, 30, 24, TextAlignmentOptions.MidlineRight));
        }
        Stat("defeated", "Defeated", "Defeated", 8, 98);
        Stat("time", "Time", "Time", 292, 98);
        Stat("accuracy", "Accuracy", "Accuracy", 8, 134);
        Stat("parries", "Parries", "Parries", 292, 134);
        var best = Text("Best", "", 0, 170, 552, 24, 20, TextAlignmentOptions.Midline);
        best.color = Hex("dcc47c"); Bind("best", best);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        group.gameObject.SetActive(false);
    }

    static void Combat(Transform design)
    {
        // Preserve main's approved score/feedback grouping and bar/rail geometry.
        // Fit the supplied font inside the existing status/score controls.
        // Leave room for stacked Vietnamese marks at the unchanged compact em.
        Place(design, "HUD/PlayerStatus", 9, 60, 306, 63);
        Place(design, "HUD/EnemyStatus", 645, 60, 306, 108);
        Place(design, "PlayerHpR7", 72, 72, 243, 42);
        Place(design, "EnemyHpR7", 708, 72, 243, 42);
        Place(design, "PlayerReadabilityR7", 9, 0, 324, 60);
        Place(design, "EnemyReadabilityR7", 627, 0, 324, 60);
        foreach (var label in design.GetComponentsInChildren<TMP_Text>(true))
        {
            if (label.name == "PlayerName" || label.name == "EnemyHPLabel")
            {
                var rt = label.rectTransform;
                Box(rt, rt.parent, label.name == "PlayerName" ? 6 : -12, -60, 312, 60);
                label.alignment = label.name == "PlayerName" ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
                Size(label, CompactSize);
            }
            if (label.name == "PlayerHpR7" || label.name == "EnemyHpR7")
            {
                Size(label, BodySize);
                label.alignment = TextAlignmentOptions.MidlineRight;
            }
            if (label.name == "ScoreText") Size(label, BodySize);
            if (label.name == "Multiplier") Size(label, CompactSize);
        }
    }
}
