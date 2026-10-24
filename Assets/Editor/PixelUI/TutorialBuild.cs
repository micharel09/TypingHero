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
        Card("ScoreCard", 328, 116, 304, 96);
        Card("HealthCard", 24, 176, 288, 120);
        Card("EnemyCard", 648, 176, 288, 144);
        Card("TypingCard", 24, 300, 396, 72);
        var score = Text("ScoreGuide", "<color=#DCC47C>Score / Combo</color>\nPoints and successful action streaks.", 340, 128, 280, 72, 24);
        var hp = Text("HealthGuide", "<color=#DCC47C>Your HP</color>\nYour health. When it runs out, you lose.", 36, 188, 264, 96, 24);
        var enemy = Text("EnemyGuide", "<color=#DCC47C>Enemy HP / Guard</color>\nDrain HP to win. Parry lowers guard. Break guard for Slayer.", 660, 188, 264, 120, 24);
        var typing = Text("TypingGuide", "<color=#DCC47C>Typing / Timer</color>\nType the current word to attack. A wrong letter resets it. The bar below is time left.", 36, 306, 372, 60, 18);
        Text("Shortcuts", "Tap Space when the key presses.\nRelease between hits. Esc: pause", 452, 326, 460, 46, 18).alignment = TextAlignmentOptions.Top;
        var next = panel.Find("PracticeButton").GetComponent<Button>();
        Box((RectTransform)next.transform, panel, 500, 480, 360, 36);
        next.navigation = new Navigation { mode = Navigation.Mode.None };
        var action = next.GetComponentInChildren<TMP_Text>(true);
        Box(action.rectTransform, next.transform, 8, 0, 344, 36);
        action.text = "Next \u00b7 Enter"; action.fontSize = action.fontSizeMin = action.fontSizeMax = 24;
        action.enableAutoSizing = false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(next.transform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action.rectTransform);
        Text("Step", "1/2", 428, 484, 60, 32, 24).alignment = TextAlignmentOptions.Center;
        BuildParry(overlay, bounds, panel, font);
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

    [System.Serializable] sealed class CaptureFrame { public float duration; public string caption; }
    [System.Serializable] sealed class Capture { public int columns, cellWidth, cellHeight, width, height; public CaptureFrame[] frames; }

    static void BuildParry(TutorialOverlay overlay, Transform bounds, Transform hud, TMP_FontAsset font)
    {
        const string path = "Assets/Art/UI/TutorialParry/parry.png";
        var capture = JsonUtility.FromJson<Capture>(System.IO.File.ReadAllText("Assets/Art/UI/TutorialParry/parry.json"));
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point; importer.wrapMode = TextureWrapMode.Clamp;
        importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048;
        var sheet = new SpriteMetaData[capture.frames.Length];
        int height = ((capture.frames.Length + capture.columns - 1) / capture.columns) * capture.cellHeight;
        for (int i = 0; i < sheet.Length; i++) sheet[i] = new SpriteMetaData {
            name = "parry_" + i.ToString("D3"), alignment = 0, pivot = Vector2.one * .5f,
            rect = new Rect(i % capture.columns * capture.cellWidth + 2,
                height - (i / capture.columns * capture.cellHeight + 2) - capture.height, capture.width, capture.height)
        };
        importer.spritesheet = sheet; importer.SaveAndReimport();
        RectTransform Node(string name, Transform parent, float x, float y, float w, float h)
        {
            var rt = parent.Find(name) as RectTransform;
            if (!rt) { rt = Rect(name, parent); Box(rt, parent, x, y, w, h); }
            return rt;
        }
        var modal = Node("ParryModal", bounds, 0, 0, 960, 540);
        if (!modal.GetComponent<Image>()) Image(modal, new Color(0, 0, 0, .8f), true);
        var panel = Node("Panel", modal, 72, 30, 816, 480);
        if (!panel.GetComponent<Image>())
        {
            var bg = Image(panel, Color.white, true);
            bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DungeonUISkin.Art + "modal-normal.png");
            bg.type = UnityEngine.UI.Image.Type.Sliced;
        }
        TMP_Text Copy(string name, string text, float x, float y, float w, float h, float size)
        {
            var existing = panel.Find(name);
            if (existing) return existing.GetComponent<TMP_Text>();
            var label = Label(name, text, panel, x, y, w, h, size);
            label.font = font; label.fontSharedMaterial = font.material; label.color = Hex("f2e7cf");
            label.fontSize = label.fontSizeMin = label.fontSizeMax = size;
            label.enableAutoSizing = false; label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            return label;
        }
        Copy("Title", "Parry with Space", 48, 22, 640, 48, 36);
        Copy("Step", "2/2", 708, 26, 60, 40, 24);
        var clip = Node("Clip", panel, 48, 100, 720, 240);
        var clipImage = clip.GetComponent<Image>();
        if (!clipImage) clipImage = Image(clip, Color.white);
        var caption = Copy("Caption", "Wait for the attack.", 48, 358, 720, 36, 24);
        var director = Object.FindObjectOfType<TutorialDirector>(true);
        Button Action(string name, float x, float w, UnityEngine.Events.UnityAction listener)
        {
            var existing = panel.Find(name + "Button");
            if (existing) return existing.GetComponent<Button>();
            var button = Object.Instantiate(hud.Find("PracticeButton").GetComponent<Button>(), panel);
            button.name = name + "Button";
            Box((RectTransform)button.transform, panel, x, 414, w, 40);
            button.onClick = new Button.ButtonClickedEvent();
            if (listener != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, listener);
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var label = button.GetComponentInChildren<TMP_Text>();
            Box(label.rectTransform, button.transform, 8, 0, w - 16, 40);
            label.text = name;
            label.font = font; label.fontSharedMaterial = font.material;
            label.fontSize = label.fontSizeMin = label.fontSizeMax = 24; label.enableAutoSizing = false;
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
            PrefabUtility.RecordPrefabInstancePropertyModifications(button.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
            PrefabUtility.RecordPrefabInstancePropertyModifications(label.rectTransform);
            return button;
        }
        var back = Action("Back", 48, 192, null);
        var start = Action("Start Battle \u00b7 Enter", 288, 480, director.Continue);
        var so = new SerializedObject(overlay);
        void Bind(string field, Object value) { var p = so.FindProperty(field); if (!p.objectReferenceValue) p.objectReferenceValue = value; }
        Bind("parryModal", modal.gameObject); Bind("backButton", back); Bind("startButton", start);
        Bind("clipImage", clipImage); Bind("clipCaption", caption);
        var nodes = new[] { "ScoreCard", "HealthCard", "EnemyCard", "TypingCard", "ScoreGuide", "HealthGuide", "EnemyGuide", "TypingGuide", "Shortcuts", "PracticeButton", "Step" };
        var content = so.FindProperty("hudContent"); content.arraySize = nodes.Length;
        for (int i = 0; i < nodes.Length; i++) content.GetArrayElementAtIndex(i).objectReferenceValue = hud.Find(nodes[i]).gameObject;
        var sprites = new System.Collections.Generic.Dictionary<string, Sprite>();
        foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path)) if (asset is Sprite sprite) sprites.Add(sprite.name, sprite);
        var frames = so.FindProperty("clipFrames"); var durations = so.FindProperty("clipDurations"); var captions = so.FindProperty("clipCaptions");
        frames.arraySize = durations.arraySize = captions.arraySize = sheet.Length;
        for (int i = 0; i < sheet.Length; i++)
        {
            frames.GetArrayElementAtIndex(i).objectReferenceValue = sprites[sheet[i].name];
            durations.GetArrayElementAtIndex(i).floatValue = capture.frames[i].duration;
            captions.GetArrayElementAtIndex(i).stringValue = capture.frames[i].caption;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        clipImage.sprite = sprites[sheet[0].name];
        modal.gameObject.SetActive(false);
    }

    static void Skin(GameObject root) => ShogunTheme.Skin(root,
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"),
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Back.asset"));

}
