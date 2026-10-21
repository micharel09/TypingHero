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
            if (root.name == "LeaderboardModal") Collection(design);
            if (root.name == "Tutorial") Tutorial(design);
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
        Place(design, "MenuPanel", 48, 0, 864, 398);
        var panel = design.Find("MenuPanel");
        Place(panel, "Title", 24, 4, 816, 52);
        Place(panel, "NameLabel", 24, 56, 816, 38);
        Size(panel.Find("NameLabel").GetComponent<TMP_Text>(), BodySize);
        var input = panel.GetComponentInChildren<TMP_InputField>(true);
        Box(input.GetComponent<RectTransform>(), panel, 24, 94, 816, 104);
        input.pointSize = BodySize;
        Size(input.textComponent, BodySize);
        input.textComponent.alignment = TextAlignmentOptions.MidlineLeft;
        if (input.placeholder is TMP_Text hint) Size(hint, BodySize);
        // The existing viewport/caret performs horizontal scrolling for long input.
        input.textComponent.overflowMode = TextOverflowModes.Overflow;
        Place(panel, "NameError", 24, 198, 816, 48);
        Size(panel.Find("NameError").GetComponent<TMP_Text>(), BodySize);
        Difficulty(panel, 24, 246, 816);
        Place(panel, "DifficultyHint", 24, 294, 816, 48);
        Size(panel.Find("DifficultyHint").GetComponent<TMP_Text>(), CompactSize);
        ButtonAt(panel, "PlayButton", 24, 342, 816, 52);
        Place(panel, "PlayButton/key-enter", 776, 14, 24, 24);
        ButtonAt(design, "How to PlayButton", 102, 402, 246, 48);
        ButtonAt(design, "TutorialButton", 357, 402, 246, 48);
        ButtonAt(design, "Top 1000Button", 612, 402, 246, 48);
        Place(design, "AudioPreferences", 120, 454, 720, 76);
        var audio = design.Find("AudioPreferences");
        Place(audio, "MusicVolume", 312, 6, 408, 12);
        Place(audio, "SFXVolume", 312, 44, 408, 12);
        foreach (var label in design.Find("AudioPreferences").GetComponentsInChildren<TMP_Text>(true))
        {
            Size(label, BodySize);
            label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 38);
        }
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
        Box(panel, design, 48, 0, 600, 540);
        Place(panel, "Title", 16, 8, 568, 52);
        Size(panel.Find("Title").GetComponent<TMP_Text>(), HeadingSize);
        Difficulty(panel, 16, 64, 568);
        Place(panel, "Header", 8, 116, 568, 96);
        var header = panel.Find("Header");
        Place(header, "Rank", 0, 0, 96, 96);
        Place(header, "Name", 104, 0, 216, 96);
        Place(header, "Score", 324, 0, 240, 96);
        foreach (var label in header.GetComponentsInChildren<TMP_Text>(true)) Size(label, BodySize, true);
        Box(list.GetComponent<RectTransform>(), panel, 8, 216, 584, 316);
        Box(list.viewport, list.transform, 0, 0, 568, 316);
        Place(list.transform, "Scrollbar", 572, 0, 12, 316);
        var detail = (TMP_Text)so.FindProperty("detailText").objectReferenceValue;
        Size(detail, BodySize, true);
        var empty = (TMP_Text)so.FindProperty("emptyLabel").objectReferenceValue;
        Size(empty, BodySize, true);
        empty.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 160);
        var names = new[] { "CloseButton", "RefreshButton", "EndlessButton", "ClassicButton", "OpponentsButton", "AchievementsButton" };
        for (int i = 0; i < names.Length; i++) ButtonAt(design, names[i], 672, 16 + i * 60, 272, 52);
    }

    internal static void Prefab(GameObject root)
    {
        if (!root.GetComponent<LeaderboardRowView>()) return;
        var rect = root.GetComponent<RectTransform>();
        if (!rect || !root.transform.Find("Name") || !root.transform.Find("Score")) return;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 224);
        var layout = root.GetComponent<LayoutElement>();
        if (layout) layout.preferredHeight = 224;
        Place(root.transform, "Rank", 0, 0, 96, 224);
        Place(root.transform, "Name", 104, 0, 216, 224);
        Place(root.transform, "Score", 324, 0, 240, 224);
        foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
            Size(label, BodySize, label.name != "Rank");
    }

    static void Tutorial(Transform design)
    {
        var panel = design.Find("TutorialPrompt");
        if (!panel) return;
        Box((RectTransform)panel, design, 48, 107, 864, 170);
        Place(panel, "Heading", 16, 0, 832, 56);
        Size(panel.Find("Heading").GetComponent<TMP_Text>(), HeadingSize, true);
        Place(panel, "Instruction", 16, 58, 832, 48);
        Size(panel.Find("Instruction").GetComponent<TMP_Text>(), BodySize, true);
        Place(panel, "InstructionBackground", 0, 0, 864, 112);
        // Keep the real parry cue and its highlight between the existing actions.
        ButtonAt(panel, "PracticeButton", 16, 118, 360, 52);
        ButtonAt(panel, "Skip TutorialButton", 552, 118, 320, 52);
    }

    static void Results(Transform design)
    {
        var panel = design.Find("CombatOverlay/OverlayPanel");
        if (!panel) return;
        Box((RectTransform)panel, panel.parent, 48, 0, 864, 540);
        foreach (string name in new[] { "PauseLabel", "ThanksLabel" })
        {
            Place(panel, name, 24, 4, 816, 44);
            var title = panel.Find(name).GetComponent<TMP_Text>();
            Size(title, HeadingSize);
            title.alignment = TextAlignmentOptions.Midline;
        }
        Place(panel, "MatchResult", 24, 48, 816, 216);
        var result = panel.Find("MatchResult").GetComponent<TMP_Text>();
        Size(result, BodySize, true);
        // Five compact statistic rows: 36-unit ink plus six units of leading.
        // TMP spacing is percent of em: -12.5% of 48 = -6 reference units.
        result.lineSpacing = -12.5f;
        ButtonAt(panel, "ResumeButton", 24, 218, 816, 42);
        ButtonAt(panel, "RestartButton", 24, 270, 816, 42);
        ButtonAt(panel, "Main MenuButton", 24, 318, 816, 42);
        Place(panel, "AudioPreferences", 24, 366, 816, 76);
        var audio = panel.Find("AudioPreferences");
        Place(audio, "MusicVolume", 312, 6, 504, 12);
        Place(audio, "SFXVolume", 312, 44, 504, 12);
        foreach (var label in audio.GetComponentsInChildren<TMP_Text>(true))
        {
            Size(label, BodySize);
            label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 38);
        }
        ButtonAt(panel, "Top 1000Button", 24, 448, 816, 48);
        Place(panel, "KeyHint", 24, 502, 816, 34);
        Size(panel.Find("KeyHint").GetComponent<TMP_Text>(), CompactSize);
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
        Place(design, "RoundR7", 309, 165, 342, 33);
        Place(design, "RoundReadabilityR7", 306, 162, 348, 39);
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
            if (label.name == "Multiplier" || label.name == "RoundR7") Size(label, CompactSize);
        }
    }
}
