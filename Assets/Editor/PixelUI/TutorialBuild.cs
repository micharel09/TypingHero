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
        var enemy = Text("EnemyGuide", "<color=#DCC47C>Enemy HP</color>\nDrain enemy HP to win.", 660, 188, 264, 120, 24);
        var typing = Text("TypingGuide", "<color=#DCC47C>Typing / Timer</color>\nType the current word to attack. A wrong letter resets it. The bar below is time left.", 36, 306, 372, 60, 18);
        Text("Shortcuts", "Tap Space when the key presses.\nRelease between hits. Esc: pause", 452, 326, 460, 46, 18).alignment = TextAlignmentOptions.Top;
        var next = panel.Find("PracticeButton").GetComponent<Button>();
        Box((RectTransform)next.transform, panel, 656, 484, 280, 40);
        next.navigation = new Navigation { mode = Navigation.Mode.None };
        var action = next.GetComponentInChildren<TMP_Text>(true);
        Box(action.rectTransform, next.transform, 8, 0, 264, 40);
        action.text = "Next \u00b7 Enter"; action.fontSize = action.fontSizeMin = action.fontSizeMax = 24;
        action.enableAutoSizing = false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(next.transform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action.rectTransform);
        Text("Step", "1/6", 432, 96, 96, 28, 22).alignment = TextAlignmentOptions.Center;
        BuildParry(overlay, bounds, panel, font);
        SkinButtons(overlay);
        overlay.ApplyCompactLayout();
        foreach (var component in overlay.GetComponentsInChildren<Component>(true))
            if (component && PrefabUtility.IsPartOfPrefabInstance(component))
                PrefabUtility.RecordPrefabInstancePropertyModifications(component);
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
            var existing = panel.Find(name) ?? bounds.Find("Practice/" + name);
            var label = existing ? existing.GetComponent<TMP_Text>() : Label(name, text, panel, x, y, w, h, size);
            if (existing && name != "DemoBadge" && name != "PracticeMessage" && name != "PracticeBadge" && name != "Readiness") return label;
            label.font = font; label.fontSharedMaterial = font.material; label.color = Hex("f2e7cf");
            label.fontSize = label.fontSizeMin = label.fontSizeMax = size;
            label.enableAutoSizing = false; label.alignment = TextAlignmentOptions.Center;
            label.enableKerning = true; label.overflowMode = TextOverflowModes.Overflow;
            label.enableWordWrapping = name == "PracticeMessage";
            label.raycastTarget = false;
            return label;
        }
        var title = Copy("Title", "Type to Attack", 32, 8, 640, 36, 30);
        var step = Copy("Step", "2/6", 820, 10, 60, 32, 24);
        void Migrate(RectTransform rect, Vector2 oldPosition, Vector2 oldSize, float x, float y, float w, float h)
        {
            if (rect.anchoredPosition == oldPosition && rect.sizeDelta == oldSize) Box(rect, rect.parent, x, y, w, h);
        }
        if (panel.anchoredPosition == new Vector2(72, -30) && panel.sizeDelta == new Vector2(816, 480))
            Box(panel, modal, 16, 12, 928, 516);
        Migrate(title.rectTransform, new Vector2(48, -18), new Vector2(640, 44), 48, 8, 756, 36);
        Migrate(step.rectTransform, new Vector2(708, -22), new Vector2(60, 32), 820, 10, 60, 32);
        var badge = Copy("DemoBadge", "WATCH ONLY", 684, 8, 120, 36, 16);
        badge.gameObject.SetActive(true);
        Box(title.rectTransform, panel, 32, 8, 640, 36);
        title.fontSize = title.fontSizeMin = title.fontSizeMax = 30;
        Box(step.rectTransform, panel, 820, 10, 60, 32);
        Box(badge.rectTransform, panel, 684, 8, 120, 36);
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
        if (rule.fontSize == 18) rule.fontSize = rule.fontSizeMin = rule.fontSizeMax = 24;
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
        // Explicit learning controls reuse the authored tutorial button skin.
        Button LearningButton(string name, Transform parent, float x, float y, float w, float h)
        {
            var existing = parent.Find(name);
            var button = existing ? existing.GetComponent<Button>() : Object.Instantiate(back, parent);
            button.name = name;
            Box((RectTransform)button.transform, parent, x, y, w, h);
            button.onClick = new Button.ButtonClickedEvent();
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var label = button.GetComponentInChildren<TMP_Text>(true);
            Box(label.rectTransform, button.transform, 4, 0, w - 8, h);
            label.fontSize = label.fontSizeMin = label.fontSizeMax = 22;
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
            PrefabUtility.RecordPrefabInstancePropertyModifications(button.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
            PrefabUtility.RecordPrefabInstancePropertyModifications(label.rectTransform);
            return button;
        }
        // Fixed footer slots in Reference960x540 coordinates, shared by all states.
        var watch = LearningButton("WatchDemo", bounds, 168, 484, 248, 40);
        var skip = LearningButton("SkipTutorial", bounds, 480, 484, 160, 40);
        skip.GetComponentInChildren<TMP_Text>(true).text = "Skip tutorial";
        Box((RectTransform)back.transform, panel, 8, 472, 128, 40);
        Box((RectTransform)start.transform, panel, 640, 472, 280, 40);
        Box(back.GetComponentInChildren<TMP_Text>(true).rectTransform, back.transform, 4, 0, 120, 40);
        Box(start.GetComponentInChildren<TMP_Text>(true).rectTransform, start.transform, 4, 0, 272, 40);
        var practice = Node("Practice", bounds, 0, 0, 960, 540);
        var practiceFocus = practice.Find("Focus") as RectTransform;
        if (!practiceFocus) { practiceFocus = Object.Instantiate(focus, practice); practiceFocus.name = "Focus"; }
        practiceFocus.pivot = Vector2.one * .5f;
        foreach (var edge in practiceFocus.GetComponentsInChildren<Image>(true)) edge.color = Gold;
        var readiness = Copy("Readiness", "READY", 684, 8, 120, 36, 14);
        Box(readiness.rectTransform, panel, 684, 8, 120, 36);
        Bind("readiness", readiness); Bind("practiceFocus", practiceFocus);
        var message = Copy("PracticeMessage", "Type HIT to attack.", 204, 100, 552, 72, 26);
        message.transform.SetParent(practice, false);
        Box(message.rectTransform, practice, 204, 100, 552, 72);
        message.alignment = TextAlignmentOptions.Top;
        var practiceBadge = Copy("PracticeBadge", "YOUR TURN", 204, 64, 552, 28, 18);
        practiceBadge.transform.SetParent(practice, false);
        Box(practiceBadge.rectTransform, practice, 204, 64, 552, 28);
        var practiceBack = LearningButton("Back", practice, 24, 484, 128, 40);
        practiceBack.GetComponentInChildren<TMP_Text>(true).text = "Back";
        var practiceAction = LearningButton("Action", practice, 656, 484, 280, 40);
        Bind("watchButton", watch); Bind("skipButton", skip); Bind("demoBadge", badge);
        Bind("practicePanel", practice.gameObject); Bind("practiceMessage", message); Bind("practiceBadge", practiceBadge);
        Bind("practiceBack", practiceBack); Bind("practiceAction", practiceAction);
        practice.gameObject.SetActive(false);
        watch.transform.SetAsLastSibling(); skip.transform.SetAsLastSibling();
        captionPanel.SetAsLastSibling();
        var nodes = new[] { "ScoreCard", "HealthCard", "EnemyCard", "TypingCard", "ScoreGuide", "HealthGuide", "EnemyGuide", "TypingGuide", "Shortcuts", "PracticeButton", "Step" };
        var content = so.FindProperty("hudContent"); content.arraySize = nodes.Length;
        for (int i = 0; i < nodes.Length; i++) content.GetArrayElementAtIndex(i).objectReferenceValue = hud.Find(nodes[i]).gameObject;
        var names = new[] { "typing", "mistakes", "parry-health", "slayer", "score" };
        var titles = new[] { "Type to Attack", "Mistakes & Timer", "Parry with Space", "Break Guard \u00b7 Slayer", "Score & Combo" };
        var rules = new[] { "Finish the word to attack. Drain enemy HP to win; keep yours above zero.", "Wrong letters reset the word; the timer keeps running.", "Tap Space when the key fills. Release between attempts.", "Parry to break Guard. In Slayer, correct letters attack.", "Words score points. Five in a row unlock the multiplier." };
        var lessons = so.FindProperty("lessons"); lessons.arraySize = names.Length;
        const string resources = "Assets/Resources/Tutorial";
        if (!AssetDatabase.IsValidFolder(resources)) AssetDatabase.CreateFolder("Assets/Resources", "Tutorial");
        for (int lesson = 0; lesson < names.Length; lesson++)
        {
            var sprites = ImportCapture(names[lesson], out var capture);

            var data = lessons.GetArrayElementAtIndex(lesson);
            data.FindPropertyRelative("title").stringValue = titles[lesson];
            data.FindPropertyRelative("rule").stringValue = rules[lesson];
            var guidance = data.FindPropertyRelative("guidance"); guidance.arraySize = sprites.Length;
            var paths = data.FindPropertyRelative("atlasPaths"); paths.arraySize = capture.atlases.Length;
            for (int atlas = 0; atlas < capture.atlases.Length; atlas++)
            {
                string resource = "Tutorial/" + System.IO.Path.GetFileNameWithoutExtension(capture.atlases[atlas]);
                string assetPath = "Assets/Resources/" + resource + ".asset";
                var page = AssetDatabase.LoadAssetAtPath<TutorialAtlasPage>(assetPath);
                if (!page) { page = ScriptableObject.CreateInstance<TutorialAtlasPage>(); AssetDatabase.CreateAsset(page, assetPath); }
                page.sprites = sprites.Where((sprite, index) => capture.frames[index].atlas == atlas).ToArray();
                if (page.sprites.Length == 0 || page.sprites.Any(sprite => !sprite)) throw new System.InvalidOperationException("Missing tutorial sprites: " + resource);
                page.texture = page.sprites[0].texture;
                if (page.sprites.Any(sprite => sprite.texture != page.texture)) throw new System.InvalidOperationException("Mixed tutorial atlas: " + resource);
                EditorUtility.SetDirty(page);
                paths.GetArrayElementAtIndex(atlas).stringValue = resource;
            }
            var frames = data.FindPropertyRelative("frameAtlas");
            var slots = data.FindPropertyRelative("frameSprite"); slots.arraySize = sprites.Length;
            var counts = new int[capture.atlases.Length];
            var durations = data.FindPropertyRelative("durations"); var captions = data.FindPropertyRelative("captions");
            frames.arraySize = durations.arraySize = captions.arraySize = sprites.Length;
            for (int i = 0; i < sprites.Length; i++)
            {
                int atlas = capture.frames[i].atlas;
                if (atlas < 0 || atlas >= counts.Length || !sprites[i] || capture.frames[i].duration <= 0)
                    throw new System.InvalidOperationException("Invalid tutorial frame " + names[lesson] + ":" + i);
                frames.GetArrayElementAtIndex(i).intValue = atlas;
                slots.GetArrayElementAtIndex(i).intValue = counts[atlas]++;
                durations.GetArrayElementAtIndex(i).floatValue = capture.frames[i].duration;
                captions.GetArrayElementAtIndex(i).stringValue = capture.frames[i].caption;
                var guide = guidance.GetArrayElementAtIndex(i);
                guide.FindPropertyRelative("focus").rectValue = capture.frames[i].focus.Bounds;
                guide.FindPropertyRelative("label").vector2Value = capture.frames[i].label;
            }
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        clipImage.sprite = null;
        modal.gameObject.SetActive(false);
    }

    // Batch verification on the isolated Ubuntu snapshot; restore shipped scenes afterwards.
    public static void VerifyAtlasFixtures()
    {
        string[] scenes = { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity" };
        var saved = scenes.Select(System.IO.File.ReadAllBytes).ToArray();
        try
        {
            string selection = System.Environment.GetEnvironmentVariable("TYPINGHERO_TUTORIAL_FIXTURES");
            foreach (string fixture in new[] { "current", "fresh", "tuned" })
            {
                if (!string.IsNullOrEmpty(selection) && !selection.Split(',').Contains(fixture)) continue;
                EditorSceneManager.OpenScene(scenes[1]);
                var overlay = Object.FindObjectOfType<TutorialOverlay>(true);
                if (fixture == "fresh")
                {
                    Object.DestroyImmediate(overlay.gameObject);
                    Object.DestroyImmediate(Object.FindObjectOfType<TutorialDirector>(true).gameObject);
                    // The fixture must also remove the deleted roots' authored world links.
                    var gate = Object.FindObjectOfType<DungeonSessionBindings>(true);
                    gate.roots = gate.roots.Where(root => root).ToArray();
                    gate.links = gate.links.Where(link => link.local).ToArray();
                    EditorUtility.SetDirty(gate);
                }
                else if (fixture == "tuned")
                {
                    var panel = (RectTransform)overlay.transform.Find("Reference960x540/ParryModal/Panel");
                    panel.anchoredPosition = new Vector2(19, -17);
                    var button = panel.GetComponentsInChildren<Button>(true).Last();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick,
                        Object.FindObjectOfType<TutorialDirector>(true).Continue);
                }
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                PixelUIBuild.SerializeOnly(); // QA mode performs two complete passes and compares all outputs.
                if (AssetDatabase.GetDependencies(scenes[1], true).Any(path =>
                    path.StartsWith("Assets/Art/UI/TutorialParry/") && path.EndsWith(".png")))
                    throw new System.InvalidOperationException("Scene still depends on a tutorial texture");
                EditorSceneManager.OpenScene(scenes[1]);
                overlay = Object.FindObjectOfType<TutorialOverlay>(true);
                var serialized = new SerializedObject(overlay);
                if (serialized.FindProperty("clipImage").objectReferenceValue is Image preview && preview.sprite)
                    throw new System.InvalidOperationException("Tutorial scene retains preview sprite");
                var lessons = serialized.FindProperty("lessons");
                int frames = 0;
                for (int lesson = 0; lesson < lessons.arraySize; lesson++)
                {
                    var data = lessons.GetArrayElementAtIndex(lesson);
                    var paths = data.FindPropertyRelative("atlasPaths");
                    var atlas = data.FindPropertyRelative("frameAtlas");
                    var slots = data.FindPropertyRelative("frameSprite");
                    for (int i = 0; i < atlas.arraySize; i++)
                    {
                        var page = AssetDatabase.LoadAssetAtPath<TutorialAtlasPage>("Assets/Resources/" +
                            paths.GetArrayElementAtIndex(atlas.GetArrayElementAtIndex(i).intValue).stringValue + ".asset");
                        if (!page || !page.sprites[slots.GetArrayElementAtIndex(i).intValue])
                            throw new System.InvalidOperationException("Unresolved tutorial frame");
                        frames++;
                    }
                }
                if (frames != 1995) throw new System.InvalidOperationException("Tutorial frame count: " + frames);
                if (fixture == "tuned")
                {
                    var panel = (RectTransform)overlay.transform.Find("Reference960x540/ParryModal/Panel");
                    if (panel.anchoredPosition != new Vector2(19, -17) ||
                        panel.GetComponentsInChildren<Button>(true).Last().onClick.GetPersistentEventCount() != 2)
                        throw new System.InvalidOperationException("Tutorial tuning/listener lost");
                }
                Debug.Log("Tutorial atlas fixture PASS: " + fixture + ", " + frames + " frames");
            }
        }
        finally
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            for (int i = 0; i < scenes.Length; i++) System.IO.File.WriteAllBytes(scenes[i], saved[i]);
            AssetDatabase.Refresh();
        }
    }

    static void Skin(GameObject root) => ShogunTheme.Skin(root,
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"),
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Back.asset"));

    // Final tutorial-only pass: later builder runs cannot restore the shared HUD rim.
    static void SkinButtons(TutorialOverlay overlay)
    {
        const string path = "Assets/Art/UI/GothicPixelUI/split_assets/frames and bars/large_ornate_frame2.png";
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (!sprite) throw new System.IO.FileNotFoundException("Missing AbyssOwl tutorial button", path);
        foreach (var button in overlay.GetComponentsInChildren<Button>(true))
        {
            var oldSkin = button.GetComponent<DungeonControlSkin>();
            if (oldSkin) Object.DestroyImmediate(oldSkin);
            var image = (Image)button.targetGraphic;
            image.sprite = sprite;
            image.overrideSprite = null;
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 1;
            image.color = Color.white;
            button.transition = Selectable.Transition.ColorTint;
            button.spriteState = new SpriteState();
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Hex("b3a080");
            colors.highlightedColor = colors.selectedColor = Color.white;
            colors.pressedColor = Hex("806b4d");
            colors.disabledColor = new Color(.35f, .32f, .28f, .65f);
            colors.fadeDuration = .08f;
            button.colors = colors;
            var label = button.GetComponentInChildren<TMP_Text>(true);
            label.color = Color.white;
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
        }
    }

}
