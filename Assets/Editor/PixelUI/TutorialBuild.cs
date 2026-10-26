using TMPro;
using System.Linq;
using UnityEditor.U2D.Sprites;
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
        Text("Step", "1/6", 428, 484, 60, 32, 24).alignment = TextAlignmentOptions.Center;
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

    [System.Serializable] sealed class CaptureFocus { public float x, y, width, height; public Rect Bounds => new Rect(x, y, width, height); }
    [System.Serializable] sealed class CaptureFrame { public float duration; public string caption; public int atlas, width, height; public CaptureFocus focus; public Vector2 label; }
    [System.Serializable] sealed class Capture { public int columns, cellWidth, cellHeight, width, height; public string[] atlases; public CaptureFrame[] frames; }

    static Sprite[] ImportCapture(string name, out Capture capture)
    {
        const string folder = "Assets/Art/UI/TutorialParry/";
        capture = JsonUtility.FromJson<Capture>(System.IO.File.ReadAllText(folder + name + ".json"));
        var result = new Sprite[capture.frames.Length];
        var files = capture.atlases ?? new[] { name + ".png" };
        // Preserve named frame identities when the atlas grid moves frames between pages.
        var identities = new System.Collections.Generic.Dictionary<string, GUID>();
        var factory = new SpriteDataProviderFactories(); factory.Init();
        foreach (string existingPath in System.IO.Directory.GetFiles(folder, name + "-*.png"))
        {
            var existingImporter = AssetImporter.GetAtPath(existingPath.Replace('\\', '/')) as TextureImporter;
            if (!existingImporter) continue;
            var existingProvider = factory.GetSpriteEditorDataProviderFromObject(existingImporter);
            existingProvider.InitSpriteEditorDataProvider();
            foreach (var rect in existingProvider.GetSpriteRects()) identities[rect.name] = rect.spriteID;
        }
        for (int page = 0; page < files.Length; page++)
        {
            string path = folder + files[page];
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.textureShape = TextureImporterShape.Texture2D;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point; importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048;
            var indices = new System.Collections.Generic.List<int>();
            for (int i = 0; i < capture.frames.Length; i++) if (capture.frames[i].atlas == page) indices.Add(i);
            int height = ((indices.Count + capture.columns - 1) / capture.columns) * capture.cellHeight;
            importer.SaveAndReimport();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();
            var sheet = new SpriteRect[indices.Count];
            for (int j = 0; j < sheet.Length; j++) sheet[j] = new SpriteRect {
                name = name + "_" + indices[j].ToString("D3"), alignment = SpriteAlignment.Center, pivot = Vector2.one * .5f,
                rect = new Rect(j % capture.columns * capture.cellWidth + 2,
                    height - (j / capture.columns * capture.cellHeight + 2) - (capture.frames[indices[j]].height > 0 ? capture.frames[indices[j]].height : capture.height),
                    capture.frames[indices[j]].width > 0 ? capture.frames[indices[j]].width : capture.width,
                    capture.frames[indices[j]].height > 0 ? capture.frames[indices[j]].height : capture.height)
            };
            foreach (var slice in sheet)
                slice.spriteID = identities.TryGetValue(slice.name, out var identity) ? identity : GUID.Generate();
            provider.SetSpriteRects(sheet);
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(
                sheet.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
            var sprites = new System.Collections.Generic.Dictionary<string, Sprite>();
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path)) if (asset is Sprite sprite) sprites.Add(sprite.name, sprite);
            for (int j = 0; j < sheet.Length; j++) result[indices[j]] = sprites[sheet[j].name];
        }
        return result;
    }

    static void BuildParry(TutorialOverlay overlay, Transform bounds, Transform hud, TMP_FontAsset font)
    {
        RectTransform Node(string name, Transform parent, float x, float y, float w, float h)
        {
            var rt = parent.Find(name) as RectTransform;
            if (!rt) { rt = Rect(name, parent); Box(rt, parent, x, y, w, h); }
            return rt;
        }
        var modal = Node("ParryModal", bounds, 0, 0, 960, 540);
        if (!modal.GetComponent<Image>()) Image(modal, new Color(0, 0, 0, .8f), true);
        var panel = Node("Panel", modal, 16, 12, 928, 516);
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
        var title = Copy("Title", "Type to Attack", 48, 8, 756, 36, 36);
        var step = Copy("Step", "2/6", 820, 10, 60, 32, 24);
        void Migrate(RectTransform rect, Vector2 oldPosition, Vector2 oldSize, float x, float y, float w, float h)
        {
            if (rect.anchoredPosition == oldPosition && rect.sizeDelta == oldSize) Box(rect, rect.parent, x, y, w, h);
        }
        if (panel.anchoredPosition == new Vector2(72, -30) && panel.sizeDelta == new Vector2(816, 480))
            Box(panel, modal, 16, 12, 928, 516);
        Migrate(title.rectTransform, new Vector2(48, -18), new Vector2(640, 44), 48, 8, 756, 36);
        Migrate(step.rectTransform, new Vector2(708, -22), new Vector2(60, 32), 820, 10, 60, 32);
        var badge = panel.Find("DemoBadge");
        if (badge) badge.gameObject.SetActive(false);
        var clip = Node("Clip", panel, 144, 52, 640, 360);
        // Migrate only shipped defaults; retain independently authored geometry.
        if (clip.sizeDelta == new Vector2(720, 270) && clip.anchoredPosition == new Vector2(408, -215))
            Box(clip, panel, 144, 52, 640, 360);
        var clipImage = clip.GetComponent<Image>();
        if (!clipImage) clipImage = Image(clip, Color.white);
        clipImage.preserveAspect = true;
        clip.anchoredPosition += Vector2.Scale(clip.sizeDelta, Vector2.one * .5f - clip.pivot);
        clip.pivot = Vector2.one * .5f;
        var rule = Copy("Rule", "Finish the word to attack.", 32, 444, 864, 36, 18);
        Migrate(rule.rectTransform, new Vector2(48, -360), new Vector2(720, 48), 32, 444, 864, 36);
        if (rule.fontSize == 24) rule.fontSize = rule.fontSizeMin = rule.fontSizeMax = 18;
        var captionPanel = (clip.Find("Guidance") as RectTransform) ?? Node("Guidance", panel, 144, 414, 640, 30);
        if (captionPanel.parent != panel)
        {
            captionPanel.SetParent(panel, false);
            Box(captionPanel, panel, 144, 414, 640, 30);
        }
        var captionBackground = captionPanel.GetComponent<Image>();
        if (captionBackground) captionBackground.enabled = false;
        var captionNode = panel.Find("Caption") ?? captionPanel.Find("Caption");
        var caption = captionNode ? captionNode.GetComponent<TMP_Text>() : Copy("Caption", "Wait for the attack.", 0, 0, 640, 30, 24);
        if (caption.transform.parent != captionPanel) caption.transform.SetParent(captionPanel, false);
        Migrate(caption.rectTransform, new Vector2(9, -3), new Vector2(282, 48), 0, 0, 640, 30);
        caption.enableWordWrapping = true;
        var focus = Node("Focus", clip, 0, 0, 90, 30);
        foreach (string edge in new[] { "Top", "Bottom", "Left", "Right" })
        {
            var border = Node(edge, focus, 0, 0, 3, 3);
            Image(border, Gold).raycastTarget = false;
            bool horizontal = edge == "Top" || edge == "Bottom";
            border.anchorMin = new Vector2(0, edge == "Bottom" ? 0 : 1);
            border.anchorMax = new Vector2(horizontal ? 1 : (edge == "Right" ? 1 : 0), horizontal ? border.anchorMin.y : 1);
            if (edge == "Right") border.anchorMin = new Vector2(1, 0);
            if (edge == "Left") border.anchorMin = Vector2.zero;
            border.pivot = new Vector2(edge == "Right" ? 1 : 0, edge == "Bottom" ? 0 : 1);
            border.anchoredPosition = Vector2.zero;
            border.sizeDelta = horizontal ? new Vector2(0, 3) : new Vector2(3, 0);
        }
        RectTransform GuideLine(string name)
        {
            var line = Node(name, clip, 0, 0, 0, 3);
            line.anchorMin = line.anchorMax = line.pivot = Vector2.one * .5f;
            Image(line, Gold).raycastTarget = false;
            return line;
        }
        var shaft = GuideLine("GuideShaft"); var tipA = GuideLine("GuideTipA"); var tipB = GuideLine("GuideTipB");
        var bend = GuideLine("GuideBend"); var end = GuideLine("GuideEnd");
        captionPanel.SetAsLastSibling();
        foreach (var graphic in clip.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = false;
        var director = Object.FindObjectOfType<TutorialDirector>(true);
        Button Action(string name, float x, float w, UnityEngine.Events.UnityAction listener)
        {
            var existing = panel.Find(name + "Button");
            if (existing) return existing.GetComponent<Button>();
            var button = Object.Instantiate(hud.Find("PracticeButton").GetComponent<Button>(), panel);
            button.name = name + "Button";
            Box((RectTransform)button.transform, panel, x, 482, w, 28);
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
        var back = Action("Back", 32, 192, null);
        var start = Action("Start Battle \u00b7 Enter", 448, 448, director.Continue);
        Migrate((RectTransform)back.transform, new Vector2(48, -422), new Vector2(192, 40), 32, 482, 192, 28);
        Migrate((RectTransform)start.transform, new Vector2(288, -422), new Vector2(480, 40), 448, 482, 448, 28);
        foreach (var button in new[] { back, start })
        {
            var labelRect = button.GetComponentInChildren<TMP_Text>(true).rectTransform;
            if (labelRect.sizeDelta.y == 40) Box(labelRect, button.transform, 8, 0, ((RectTransform)button.transform).sizeDelta.x - 16, 28);
        }
        var so = new SerializedObject(overlay);
        void Bind(string field, Object value) { var p = so.FindProperty(field); if (!p.objectReferenceValue) p.objectReferenceValue = value; }
        Bind("parryModal", modal.gameObject); Bind("backButton", back); Bind("startButton", start);
        Bind("clipImage", clipImage); Bind("clipCaption", caption);
        Bind("clipRule", rule); Bind("captionPanel", captionPanel); Bind("clipFocus", focus);
        Bind("guideShaft", shaft); Bind("guideTipA", tipA); Bind("guideTipB", tipB);
        Bind("guideBend", bend); Bind("guideEnd", end);
        Bind("clipTitle", title); Bind("modalStep", step); Bind("hudStep", hud.Find("Step").GetComponent<TMP_Text>());
        Bind("actionLabel", start.GetComponentInChildren<TMP_Text>(true));
        var nodes = new[] { "ScoreCard", "HealthCard", "EnemyCard", "TypingCard", "ScoreGuide", "HealthGuide", "EnemyGuide", "TypingGuide", "Shortcuts", "PracticeButton", "Step" };
        var content = so.FindProperty("hudContent"); content.arraySize = nodes.Length;
        for (int i = 0; i < nodes.Length; i++) content.GetArrayElementAtIndex(i).objectReferenceValue = hud.Find(nodes[i]).gameObject;
        var names = new[] { "typing", "mistakes", "parry-health", "slayer", "score" };
        var titles = new[] { "Type to Attack", "Mistakes & Timer", "Parry with Space", "Break Guard \u00b7 Slayer", "Score & Combo" };
        var rules = new[] { "Finish the word to attack.", "Wrong letters reset the word.\nThe timer keeps running.", "Tap Space as the attack lands.\nRelease between hits.", "Parry until Guard breaks.\nIn Slayer, each correct letter attacks.", "Complete words to score.\nFive words in a row unlock the multiplier." };
        var lessons = so.FindProperty("lessons"); lessons.arraySize = names.Length;
        Sprite first = null;
        for (int lesson = 0; lesson < names.Length; lesson++)
        {
            var sprites = ImportCapture(names[lesson], out var capture);
            if (lesson == 0) first = sprites[0];
            var data = lessons.GetArrayElementAtIndex(lesson);
            data.FindPropertyRelative("title").stringValue = titles[lesson];
            data.FindPropertyRelative("rule").stringValue = rules[lesson];
            var guidance = data.FindPropertyRelative("guidance"); guidance.arraySize = sprites.Length;
            var frames = data.FindPropertyRelative("frames"); var durations = data.FindPropertyRelative("durations"); var captions = data.FindPropertyRelative("captions");
            frames.arraySize = durations.arraySize = captions.arraySize = sprites.Length;
            for (int i = 0; i < sprites.Length; i++)
            {
                frames.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
                durations.GetArrayElementAtIndex(i).floatValue = capture.frames[i].duration;
                captions.GetArrayElementAtIndex(i).stringValue = capture.frames[i].caption;
                var guide = guidance.GetArrayElementAtIndex(i);
                guide.FindPropertyRelative("focus").rectValue = capture.frames[i].focus.Bounds;
                guide.FindPropertyRelative("label").vector2Value = capture.frames[i].label;
            }
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        clipImage.sprite = first;
        modal.gameObject.SetActive(false);
    }

    static void Skin(GameObject root) => ShogunTheme.Skin(root,
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"),
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Back.asset"));

}
