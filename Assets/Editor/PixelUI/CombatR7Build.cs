using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.LowLevel;
using static PixelUIFactory;

// Final combat-only migration. Existing controls retain their identities/bindings.
internal static class CombatR7Build
{
    internal const string Art = "Assets/Art/UI/CombatR7/";
    static T Find<T>() where T : Object => Object.FindObjectOfType<T>(true);
    static RectTransform Node(Transform parent, string name)
    {
        var node = parent.Find(name) as RectTransform;
        return node ? node : Rect(name, parent);
    }
    static Sprite Sprite(string name)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + name + ".png");
        if (!sprite) throw new FileNotFoundException("CombatR7 sprite import missing: " + name);
        return sprite;
    }
    static Image Picture(Transform parent, string name, string asset, float x, float y, float w, float h)
    {
        var rt = Node(parent, name); Box(rt, parent, x * 3, y * 3, w * 3, h * 3);
        var image = Image(rt, Color.white); image.sprite = Sprite(asset); image.enabled = true; return image;
    }

    internal static void Apply()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Art + "VT323Pixel16.asset");
        if (!font)
        {
            var source = AssetDatabase.LoadAssetAtPath<Font>(PixelUIFactory.Art + "VT323-Regular.ttf");
            font = TMP_FontAsset.CreateFontAsset(source, 16, 1, GlyphRenderMode.RASTER_HINTED,
                512, 512, AtlasPopulationMode.Dynamic, false);
            font.name = "VT323Pixel16";
            AssetDatabase.CreateAsset(font, Art + "VT323Pixel16.asset");
            font.TryAddCharacters(string.Concat(Enumerable.Range(32, 95).Select(c => (char)c)));
            font.atlasPopulationMode = AtlasPopulationMode.Static;
            AssetDatabase.AddObjectToAsset(font.material, font);
            foreach (var atlas in font.atlasTextures)
            {
                atlas.filterMode = FilterMode.Point;
                atlas.wrapMode = TextureWrapMode.Clamp;
                AssetDatabase.AddObjectToAsset(atlas, font);
            }
            EditorUtility.SetDirty(font);
        }
        if (font.atlasPadding < 1)
        {
            // TMP's bitmap shader adds one pixel of quad padding. A zero-gutter
            // atlas samples neighboring glyphs; repack in place, preserving GUIDs.
            font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            var fontSettings = new SerializedObject(font);
            fontSettings.FindProperty("m_AtlasPadding").intValue = 1;
            fontSettings.ApplyModifiedPropertiesWithoutUndo();
            font.ClearFontAssetData();
            font.TryAddCharacters(string.Concat(Enumerable.Range(32, 95).Select(c => (char)c)));
            font.atlasPopulationMode = AtlasPopulationMode.Static;
            foreach (var atlas in font.atlasTextures)
            {
                atlas.filterMode = FilterMode.Point; atlas.wrapMode = TextureWrapMode.Clamp;
                EditorUtility.SetDirty(atlas);
            }
            EditorUtility.SetDirty(font);
        }
        var font8 = CombatFont(8);
        var font10 = CombatFont(10);
        var font12 = CombatFont(12);
        foreach (var file in Directory.GetFiles(Art, "*.png"))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(file.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Sprite;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point; importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            string name = Path.GetFileNameWithoutExtension(file);
            importer.spriteBorder = name == "typing-frame" ? new Vector4(4, 4, 4, 4) :
                name == "score-frame" ? new Vector4(3, 4, 3, 3) :
                name.StartsWith("secondary-frame") ? new Vector4(2, 2, 2, 2) : Vector4.zero;
            importer.SaveAndReimport();
        }
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var root = GameObject.Find("PixelUI/Reference960x540").transform;
        var hud = root.Find("HUD");
        RectTransform Item(string name) => hud.GetComponentsInChildren<RectTransform>(true).First(t => t.name == name);
        var typing = Find<TypingManager>();
        var tray = Get<Slider>(typing, "countdownSlider").transform.parent as RectTransform;
        Box(tray, root, 183, 438, 594, 90);
        var board = Image(tray, Color.white); board.sprite = Sprite("typing-frame");
        PrefabUtility.RecordPrefabInstancePropertyModifications(board);
        var feedback = tray.GetComponent<TypingPanelFeedback>(); if (feedback) feedback.enabled = false;
        foreach (var edge in tray.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("ErrorBorder"))) edge.gameObject.SetActive(false);
        var rail = Get<RectTransform>(typing, "rail"); Box(rail, tray, 12, 15, 570, 60);
        if (!rail.GetComponent<RectMask2D>()) rail.gameObject.AddComponent<RectMask2D>();
        foreach (string field in new[] { "wordText", "nextWordText", "thirdWordText" })
        {
            var text = Get<TextMeshProUGUI>(typing, field);
            Box(text.rectTransform, rail, 0, 0, 174, 60);
            Style(text, 48); text.alignment = TextAlignmentOptions.TopLeft;
            text.font = font;
            text.enableAutoSizing = false; text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
            text.margin = Vector4.zero;
            text.color = Hex(field == "wordText" ? "f2e7cf" : field == "nextWordText" ? "afa1b3" : "827789");
            var group = text.GetComponent<CanvasGroup>(); if (group) group.alpha = 1;
        }
        Value(typing, "referencePixel", 3f);
        Value(typing, "completedColor", Hex("588b8c"));
        Value(typing, "errorFlashColor", Hex("f16b67"));
        var player = Item("PlayerStatus"); Box(player, hud, 9, 33, 306, 63);
        var enemy = Item("EnemyStatus"); Box(enemy, hud, 645, 33, 306, 108);
        Bar(Item("PlayerHP").GetComponent<Slider>(), player, "hp", false);
        Bar(Item("EnemyHP").GetComponent<Slider>(), enemy, "hp", false);
        Bar(Item("EnemyStamina").GetComponent<Slider>(), enemy, "stamina", true);
        // Preserve the old status decorations' IDs without presenting score as a player resource.
        foreach (string name in new[] { "FrameSecondary", "SocketSecondary", "IconSecondary" })
            if (player.Find(name)) player.Find(name).gameObject.SetActive(false);
        var cluster = Item("ScoreCluster"); Box(cluster, hud, 333, 9, 294, 66);
        var scoreBoard = Picture(cluster, "Metal", "score-frame", 0, 0, 98, 22); scoreBoard.transform.SetAsFirstSibling();
        scoreBoard.color = new Color(.75f, .75f, .75f, 1f);
        var score = Item("ScoreText").GetComponent<TMP_Text>();
        Box(score.rectTransform, cluster, 12, 0, 270, 60); Style(score, 36); score.alignment = TextAlignmentOptions.Center;
        score.rectTransform.pivot = new Vector2(.5f, .5f);
        score.rectTransform.anchoredPosition = new Vector2(147, -30);
        var combo = Item("ComboHUD"); Box(combo, cluster, 0, 69, 294, 54);
        var mult = combo.GetComponentInChildren<TMP_Text>(true);
        Box(mult.rectTransform, combo, 0, 0, 294, 30); Style(mult, 24); mult.alignment = TextAlignmentOptions.Center;
        var decay = Item("ComboDecay").GetComponent<Slider>();
        Box(decay.GetComponent<RectTransform>(), combo, 12, 36, 270, 12);
        foreach (var image in decay.GetComponentsInChildren<Image>(true)) image.enabled = image.transform == decay.fillRect;
        decay.GetComponent<Image>().enabled = false;
        decay.interactable = false;
        decay.navigation = new Navigation { mode = Navigation.Mode.None };
        decay.fillRect.offsetMin = decay.fillRect.offsetMax = Vector2.zero;
        var decayFill = decay.fillRect.GetComponent<Image>();
        decayFill.enabled = true; decayFill.sprite = Sprite("combo-fill"); decayFill.color = Color.white;
        decayFill.type = UnityEngine.UI.Image.Type.Simple;
        var decayTrack = Picture(decay.transform, "TrackR7", "secondary-track", 0, 0, 90, 4);
        decayTrack.transform.SetAsFirstSibling();
        PrefabUtility.RecordPrefabInstancePropertyModifications(decay);
        PrefabUtility.RecordPrefabInstancePropertyModifications(decay.GetComponent<Image>());
        PrefabUtility.RecordPrefabInstancePropertyModifications(decayFill);
        PrefabUtility.RecordPrefabInstancePropertyModifications(decay.fillRect);
        var timer = Get<Slider>(typing, "countdownSlider");
        Box(timer.GetComponent<RectTransform>(), tray, 12, 78, 570, 6);
        foreach (var img in timer.GetComponentsInChildren<Image>(true)) img.sprite = null;
        LabelOnce(root, "ParryR7", "PARRY", 27, 146, 40, 12, 10).color = Hex("dcc18a");
        Picture(root, "ParryKeyR7", "keycap-space", 35, 160, 24, 12);
        var clock = LabelOnce(root, "ClockR7", "00:00", 257, 146, 30, 12, 10);
        var playerHp = LabelOnce(root, "PlayerHpR7", "", 73, 0, 31, 11, 8);
        var enemyHp = LabelOnce(root, "EnemyHpR7", "", 281, 0, 36, 11, 8);
        var round = LabelOnce(root, "RoundR7", "", 109, 46, 102, 11, 8);
        CombatLabel(playerHp, font8); CombatLabel(enemyHp, font8); CombatLabel(round, font8);
        CombatLabel(mult, font8); CombatLabel(clock, font10); CombatLabel(score, font12);
        Backplate(root, "PlayerReadabilityR7", 3, 0, 102, 11);
        Backplate(root, "EnemyReadabilityR7", 215, 0, 102, 11);
        var roundBackground = Backplate(root, "RoundReadabilityR7", 108, 45, 104, 13);
        Backplate(root, "ClockReadabilityR7", 256, 145, 32, 14);
        foreach (string name in new[] { "ParryR7", "ParryKeyR7", "ClockR7", "ClockReadabilityR7" })
            root.Find(name).gameObject.SetActive(false);
        var pauseIcon = Picture(root, "PauseIconR7", "icon-pause", 267, 163, 10, 10);
        pauseIcon.raycastTarget = true;
        var pauseButton = pauseIcon.GetComponent<Button>();
        if (!pauseButton) pauseButton = pauseIcon.gameObject.AddComponent<Button>();
        pauseButton.targetGraphic = pauseIcon; pauseButton.transition = Selectable.Transition.None;
        pauseButton.colors = ColorBlock.defaultColorBlock; pauseButton.spriteState = new SpriteState();
        pauseButton.navigation = new Navigation { mode = Navigation.Mode.None };
        pauseButton.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddBoolPersistentListener(pauseButton.onClick, Find<GamePauseController>().TogglePause, true);
        var pauseSound = pauseIcon.GetComponent<UIButtonFeedback>();
        if (!pauseSound) pauseSound = pauseIcon.gameObject.AddComponent<UIButtonFeedback>();
        Set(pauseSound, "sound", AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"));
        var playerName = Item("PlayerName").GetComponent<TMP_Text>();
        Box(playerName.rectTransform, player, 6, -33, 180, 33); Style(playerName, 24); playerName.gameObject.SetActive(true);
        Find<PlayerNameHUD>().enabled = true;
        Value(Find<PlayerNameHUD>(), "showRank", false);
        var enemyName = Item("EnemyHPLabel").GetComponent<TMP_Text>();
        Box(enemyName.rectTransform, enemy, 6, -33, 192, 33); Style(enemyName, 24); enemyName.gameObject.SetActive(true);
        var status = root.GetComponent<CombatR7Status>();
        if (!status) status = root.gameObject.AddComponent<CombatR7Status>();
        Set(status, "enemyLabel", enemyName); Set(status, "clock", clock);
        Set(status, "playerHp", playerHp); Set(status, "enemyHp", enemyHp); Set(status, "round", round);
        Set(status, "roundBackground", roundBackground);
        Set(status, "comboFill", decayFill);
        Set(status, "normalFill", Sprite("combo-fill")); Set(status, "goldFill", Sprite("combo-gold-fill"));
        Set(Find<TutorialOverlay>(), "staminaTarget", enemy.Find("FrameSecondary"));
        Set(status, "enemySocket", enemy.Find("SocketHP").GetComponent<Image>());
        Set(status, "enemyIcon", enemy.Find("IconHP").GetComponent<Image>());
        Set(status, "normalSocket", Sprite("resource-socket")); Set(status, "eliteSocket", Sprite("elite-socket")); Set(status, "bossSocket", Sprite("boss-socket"));
        Set(status, "normalIcon", Sprite("icon-hp")); Set(status, "eliteIcon", Sprite("elite-badge")); Set(status, "bossIcon", Sprite("captain-badge"));
        var hudFeedback = root.GetComponent<CombatHUDFeedback>();
        if (!hudFeedback) hudFeedback = root.gameObject.AddComponent<CombatHUDFeedback>();
        Set(hudFeedback, "parryKey", root.Find("ParryKeyR7").GetComponent<Image>());
        Set(hudFeedback, "hpFrame", player.Find("FrameHP").GetComponent<Image>());
        Set(hudFeedback, "playerHp", playerHp);
        Set(hudFeedback, "staminaIcon", Picture(enemy, "IconSecondary", "icon-shield", 1, 24, 10, 10));
        Set(hudFeedback, "shield", Sprite("icon-shield")); Set(hudFeedback, "warning", Sprite("icon-warning"));
        // Preserve the serialized node/binding; Slayer already announces the break.
        var guardBreak = LabelOnce(enemy, "GuardBreakR7", "", 0, 37, 102, 11, 8);
        guardBreak.enabled = false;
        Set(hudFeedback, "guardBreak", guardBreak);
        Value(hudFeedback, "red", Hex("f16b67")); Value(hudFeedback, "gold", Hex("dcc18a"));
        Value(hudFeedback, "success", Hex("588b8c"));
        var combat = root.Find("CombatViewport") as RectTransform; Box(combat, root, 0, 0, 960, 540);
        // 48 reference pixels/world unit = 16 native pixels/unit for the existing 16PPU actors.
        // Reframe the camera, never the actor transforms/colliders or attack spacing.
        const float ortho = 540f / 96f;
        Physics2D.SyncTransforms();
        var layout = root.GetComponent<CombatHUDLayout>();
        var cameraPosition = Camera.main.transform.position;
        cameraPosition.y = layout.LowestFoot.y + 3f; // real feet on native y138
        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = ortho;
        foreach (var vcam in Object.FindObjectsOfType<MonoBehaviour>(true))
        {
            if (vcam.GetType().Name != "CinemachineVirtualCamera") continue;
            var position = vcam.transform.position; position.y = cameraPosition.y;
            vcam.transform.position = position;
            var settings = new SerializedObject(vcam);
            settings.FindProperty("m_Lens.OrthographicSize").floatValue = ortho;
            settings.ApplyModifiedPropertiesWithoutUndo();
        }
        Value(Find<SlayerScreenFX>(), "normalOrtho", ortho);
        Value(Find<SlayerScreenFX>(), "slayerOrtho", ortho * .78f);
        Value(Find<SlayerScreenFX>(), "bodyPadding", 24f); // eight native pixels
        var viewportCamera = combat.GetComponent<CombatViewportCamera>();
        Value(viewportCamera, "anchorFloorDuringZoom", true);
        Value(viewportCamera, "normalOrtho", ortho);
        var old = GameObject.Find("ShogunEnvironment"); if (old) old.SetActive(false);
        var backdrop = Node(root, "DungeonR7");
        Box(backdrop, root, 0, 0, 960, 540); backdrop.SetAsFirstSibling();
        // World layers stay behind the real actors, including the floor and outer foreground.
        var environment = GameObject.Find("DungeonEnvironment") ?? new GameObject("DungeonEnvironment");
        int order = -100;
        foreach (string layer in new[] { "far", "mid", "floor", "foreground" })
        {
            var child = environment.transform.Find(layer);
            if (!child) { child = new GameObject(layer).transform; child.SetParent(environment.transform, false); }
            var renderer = child.GetComponent<SpriteRenderer>();
            if (!renderer) renderer = child.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite("scene-" + layer); renderer.sortingOrder = order++; renderer.color = Color.white;
            renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            var fit = child.GetComponent<CombatR7Backdrop>();
            if (!fit) fit = child.gameObject.AddComponent<CombatR7Backdrop>();
            Set(fit, "target", Camera.main);
        }
        Set(Find<SlayerScreenFX>(), "environment", environment.transform);
        foreach (var group in new[] { player, enemy })
        foreach (string name in new[] { "FrameHP", "FrameSecondary", "PlayerName", "EnemyHPLabel",
            "PlayerHP", "EnemyHP", "ComboDecay", "EnemyStamina", "SocketHP", "IconHP", "SocketSecondary", "IconSecondary" })
        {
            var child = group.Find(name);
            if (child) child.SetAsLastSibling();
        }
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static TMP_FontAsset CombatFont(int size)
    {
        string path = Art + "VT323Pixel" + size + ".asset";
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        if (font) return font;
        var source = AssetDatabase.LoadAssetAtPath<Font>(PixelUIFactory.Art + "VT323-Regular.ttf");
        font = TMP_FontAsset.CreateFontAsset(source, size, 1, GlyphRenderMode.RASTER_HINTED,
            512, 512, AtlasPopulationMode.Dynamic, false);
        font.name = "VT323Pixel" + size;
        AssetDatabase.CreateAsset(font, path);
        string characters = string.Concat(Enumerable.Range(32, 95).Select(c => (char)c)) + "\u00b7\u00d7";
        if (!font.TryAddCharacters(characters, out string missing))
            throw new System.InvalidOperationException("Combat font missing glyphs: " + missing);
        font.atlasPopulationMode = AtlasPopulationMode.Static;
        AssetDatabase.AddObjectToAsset(font.material, font);
        foreach (var atlas in font.atlasTextures)
        {
            atlas.filterMode = FilterMode.Point; atlas.wrapMode = TextureWrapMode.Clamp;
            AssetDatabase.AddObjectToAsset(atlas, font);
        }
        EditorUtility.SetDirty(font);
        return font;
    }

    static void CombatLabel(TMP_Text label, TMP_FontAsset font)
    {
        label.font = font; label.fontSharedMaterial = font.material;
        label.enableAutoSizing = false; label.enableWordWrapping = false;
        if (PrefabUtility.IsPartOfPrefabInstance(label))
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
    }

    static Image Backplate(Transform root, string name, float x, float y, float w, float h)
    {
        var rt = Node(root, name); Box(rt, root, x * 3, y * 3, w * 3, h * 3);
        var image = Image(rt, new Color(20f / 255, 17f / 255, 30f / 255, .88f));
        image.sprite = null; image.type = UnityEngine.UI.Image.Type.Simple;
        image.raycastTarget = false;
        // Root siblings precede HUD/tray/labels; overlapping typing-frame pixels stay on top.
        int index = root.Find("HUD").GetSiblingIndex();
        if (rt.GetSiblingIndex() < index) index--;
        rt.SetSiblingIndex(index);
        return image;
    }

    static TMP_Text LabelOnce(Transform parent, string name, string text, float x, float y, float w, float h, float size)
    {
        var rt = Node(parent, name); Box(rt, parent, x * 3, y * 3, w * 3, h * 3);
        var label = rt.GetComponent<TextMeshProUGUI>();
        if (!label) label = rt.gameObject.AddComponent<TextMeshProUGUI>();
        Style(label, size * 3); label.text = text; label.alignment = TextAlignmentOptions.Center; return label;
    }

    static void Bar(Slider slider, Transform parent, string kind, bool secondary)
    {
        // The existing Fill is a direct child of this slider. Size its parent to the
        // actual track, so Slider's normalized anchors also work at0% and1%.
        Box(slider.GetComponent<RectTransform>(), parent, secondary ? 42 : 60, secondary ? 81 : 21,
            secondary ? 252 : 231, secondary ? 12 : 18);
        foreach (var image in slider.GetComponentsInChildren<Image>(true)) image.enabled = image.transform == slider.fillRect;
        slider.GetComponent<Image>().enabled = false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(slider.GetComponent<Image>());
        var frame = Picture(parent, secondary ? "FrameSecondary" : "FrameHP", secondary ? "secondary-frame" : "resource-frame", 0, secondary ? 23 : 0, 102, secondary ? 13 : 21);
        frame.transform.SetAsFirstSibling();
        var fill = slider.fillRect;
        fill.offsetMin = fill.offsetMax = Vector2.zero;
        var imageFill = fill.GetComponent<Image>(); imageFill.enabled = true; imageFill.sprite = Sprite(kind == "hp" ? "hp-fill" : kind + "-fill"); imageFill.color = Color.white; imageFill.type = UnityEngine.UI.Image.Type.Simple;
        PrefabUtility.RecordPrefabInstancePropertyModifications(imageFill);
        PrefabUtility.RecordPrefabInstancePropertyModifications(fill);
        var track = Picture(slider.transform, "TrackR7", secondary ? "secondary-track" : "resource-track", 0, 0, secondary ? 84 : 77, secondary ? 4 : 6);
        track.transform.SetAsFirstSibling();
        Picture(parent, secondary ? "SocketSecondary" : "SocketHP", secondary ? "secondary-socket" : "resource-socket", 0, secondary ? 23 : 0, secondary ? 13 : 21, secondary ? 13 : 21);
        Picture(parent, secondary ? "IconSecondary" : "IconHP", secondary ? "small-icon-" + kind : "icon-hp", secondary ? 3 : 5, secondary ? 26 : 4, secondary ? 7 : 10, secondary ? 7 : 10);
    }
}
