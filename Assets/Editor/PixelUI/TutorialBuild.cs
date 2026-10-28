using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class TutorialBuild
{
    internal static void Apply()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        var root = GameObject.Find("PixelUI/Reference960x540").transform;
        var menu = Object.FindObjectOfType<MainMenuUI>(true);
        if (!root.Find("TutorialButton")) Button("Tutorial", root, 411, 398, 138, menu.Tutorial);
        Set(menu, "tutorialButton", root.Find("TutorialButton").GetComponent<Button>());
        foreach (var entry in new[] { ("TutorialButton", 411f), ("Top 1000Button", 558f) })
        {
            var rect = (RectTransform)root.Find(entry.Item1);
            Box(rect, root, entry.Item2, 398, 138, 40);
            var label = rect.GetComponentInChildren<TMP_Text>();
            Box(label.rectTransform, rect, 8, 0, 122, 40);
            label.fontSize = 22;
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
        }
        Skin(root.gameObject);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var director = Object.FindObjectOfType<TutorialDirector>(true);
        if (!director) director = new GameObject("TutorialDirector").AddComponent<TutorialDirector>();
        var tutorial = Object.FindObjectOfType<TutorialOverlay>(true);
        if (!tutorial)
        {
            var canvas = Canvas("Tutorial", 30);
            tutorial = canvas.gameObject.AddComponent<TutorialOverlay>();
            var bounds = DesignRoot(canvas);
            var panel = PanelAt("TutorialPrompt", bounds, 144, 150, 672, 276);
            Button("Practice", panel, 156, 226, 360, director.Continue);
        }
        if (!tutorial.GetComponent<CanvasGroup>()) tutorial.gameObject.AddComponent<CanvasGroup>();
        Set(director, "overlay", tutorial);
        var prompt = tutorial.transform.Find("Reference960x540/TutorialPrompt");
        // Preserve the existing Start control and its persistent Continue listener.
        Set(tutorial, "continueButton", prompt.Find("PracticeButton").GetComponent<Button>());
        foreach (string name in new[] { "Heading", "Instruction", "InstructionBackground", "Skip TutorialButton" })
            if (prompt.Find(name)) prompt.Find(name).gameObject.SetActive(false);
        foreach (string name in new[] { "TrayHighlight", "StaminaHighlight", "CueHighlight" })
        {
            var node = tutorial.transform.Find("Reference960x540/" + name);
            if (node) node.gameObject.SetActive(false);
        }
        Skin(tutorial.gameObject);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    // Run after the HUD and typography migrations so these references and bounds
    // always describe the final, real HUD, including repeated builder passes.
    internal static void Guide()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var overlay = Object.FindObjectOfType<TutorialOverlay>(true);
        var bounds = overlay.transform.Find("Reference960x540");
        var panel = (RectTransform)bounds.Find("TutorialPrompt");
        Box(panel, bounds, 0, 0, 960, 540);
        var image = panel.GetComponent<Image>();
        image.enabled = false; image.color = Color.white; image.type = UnityEngine.UI.Image.Type.Sliced;
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DungeonUISkin.Art + "modal-normal.png");
        image.pixelsPerUnitMultiplier = 1;
        PrefabUtility.RecordPrefabInstancePropertyModifications(image);
        var font = panel.GetComponentsInChildren<TMP_Text>(true)[0].font;
        TMP_Text Text(string name, string content, float x, float y, float w, float h, float size)
        {
            var rect = panel.Find(name) as RectTransform;
            var label = rect ? rect.GetComponent<TMP_Text>() : Label(name, content, panel, x, y, w, h, size);
            Box(label.rectTransform, panel, x, y, w, h);
            label.font = font; label.fontSharedMaterial = font.material; label.text = content;
            label.fontSize = label.fontSizeMin = label.fontSizeMax = size;
            label.color = Hex("f2e7cf"); label.enableKerning = true; label.fontStyle = FontStyles.Normal;
            label.enableAutoSizing = false; label.enableWordWrapping = true;
            label.margin = new Vector4(6, 0, 0, 0);
            label.alignment = TextAlignmentOptions.TopLeft;
            label.overflowMode = TextOverflowModes.Overflow;
            return label;
        }
        // Separate callouts sit beside their HUD targets; the shared root is transparent.
        void Card(string name, float x, float y, float w, float h)
        {
            var rect = panel.Find(name) as RectTransform;
            if (!rect) rect = Rect(name, panel);
            Box(rect, panel, x, y, w, h);
            var background = Image(rect, Color.white);
            background.sprite = image.sprite; background.type = image.type;
            background.color = Color.white; background.raycastTarget = false;
            rect.SetAsFirstSibling();
        }
        Card("ScoreCard", 328, 116, 304, 168);
        Card("HealthCard", 24, 176, 288, 120);
        Card("EnemyCard", 648, 176, 288, 144);
        Card("TypingCard", 24, 300, 396, 72);
        var score = Text("ScoreGuide", "<color=#DCC47C>Score / Combo</color>\nChain correct words to raise your score multiplier. Keep going before the combo bar runs out.", 340, 128, 280, 144, 20);
        var hp = Text("HealthGuide", "<color=#DCC47C>Your HP</color>\nYour health. When it runs out, you lose.", 36, 188, 264, 96, 24);
        var enemy = Text("EnemyGuide", "<color=#DCC47C>Enemy HP / Guard</color>\nDrain HP to win. Parry lowers guard. Break guard for Slayer.", 660, 188, 264, 120, 24);
        var typing = Text("TypingGuide", "<color=#DCC47C>Typing / Timer</color>\nType the current word to attack. A wrong letter resets it. The bar below is time left.", 36, 306, 372, 60, 18);
        Text("Shortcuts", "Space: parry when the key fills.\nEsc: pause", 452, 326, 460, 46, 18).alignment = TextAlignmentOptions.Top;
        var next = panel.Find("PracticeButton").GetComponent<Button>();
        Box((RectTransform)next.transform, panel, 500, 480, 360, 36);
        next.navigation = new Navigation { mode = Navigation.Mode.None };
        var action = next.GetComponentInChildren<TMP_Text>(true);
        Box(action.rectTransform, next.transform, 8, 0, 344, 36);
        action.text = "Start Battle \u00b7 Enter"; action.fontSize = action.fontSizeMin = action.fontSizeMax = 24;
        action.enableAutoSizing = false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(next.transform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action.rectTransform);
        var hud = GameObject.Find("PixelUI/Reference960x540/HUD").transform;
        var so = new SerializedObject(overlay);
        var arrows = so.FindProperty("arrows"); arrows.arraySize = 5;
        void Arrow(int index, RectTransform source, RectTransform target, Vector2 start, Vector2 end)
        {
            var entry = arrows.GetArrayElementAtIndex(index);
            entry.FindPropertyRelative("source").objectReferenceValue = source;
            entry.FindPropertyRelative("target").objectReferenceValue = target;
            entry.FindPropertyRelative("sourceAnchor").vector2Value = start;
            entry.FindPropertyRelative("targetAnchor").vector2Value = end;
            foreach (string part in new[] { "shaft", "tipA", "tipB" })
            {
                string name = "Arrow" + index + part;
                var rect = bounds.Find(name) as RectTransform;
                if (!rect) rect = Rect(name, bounds);
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
                Image(rect, Gold);
                entry.FindPropertyRelative(part).objectReferenceValue = rect;
            }
        }
        Arrow(0, hp.rectTransform, (RectTransform)hud.Find("PlayerStatus/PlayerHP"), new Vector2(.3f, 1), new Vector2(.5f, 0));
        Arrow(1, enemy.rectTransform, (RectTransform)hud.Find("EnemyStatus/EnemyHP"), new Vector2(.3f, 1), new Vector2(.5f, 0));
        Arrow(2, enemy.rectTransform, (RectTransform)hud.Find("EnemyStatus/EnemyStamina"), new Vector2(.85f, 1), new Vector2(1, 0));
        Arrow(3, score.rectTransform, (RectTransform)hud.Find("ScoreCluster"), new Vector2(.5f, 1), new Vector2(.5f, 0));
        // Exit below the typing callout, keeping the connector outside its copy.
        Arrow(4, typing.rectTransform, Get<RectTransform>(overlay, "typingTray"), new Vector2(.1f, -.15f), new Vector2(0, 1));
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void Skin(GameObject root) => ShogunTheme.Skin(root,
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"),
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Back.asset"));

}
