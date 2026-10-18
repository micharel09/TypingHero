using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

// Presentation migration: preserve existing object identities and gameplay bindings.
internal static class ShogunTheme
{
    const string Audio = "Assets/Audio/SFX/Clips/KenneyUI/";
    internal const string Events = "Assets/Audio/SFX/Events/UI/";
    static readonly Color Jade = Hex("528D83"), Paper = Hex("F2E7CF"),
        Coal = Hex("152725"), Vermilion = Hex("C65140");
    static T Ensure<T>(GameObject go) where T : Component
    { var component = go.GetComponent<T>(); return component ? component : go.AddComponent<T>(); }
    static RectTransform At(Transform parent, string path) => parent.Find(path).GetComponent<RectTransform>();
    static void Place(Transform root, string path, float x, float y, float w, float h)
    { var rt = At(root, path); Box(rt, rt.parent, x, y, w, h); }

    internal static void Apply()
    {
        ShogunEnvironment.Prepare();
        System.IO.Directory.CreateDirectory(Events); AssetDatabase.Refresh();
        var confirm = Sound("Confirm", "click1.ogg");
        var back = Sound("Back", "switch2.ogg");
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        string firebaseConfig = EditorJsonUtility.ToJson(Object.FindObjectOfType<FirebaseLeaderboard>(true));
        foreach (var scene in new[] { "MainMenu", "SampleScene" })
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + scene + ".unity");
            var root = GameObject.Find("PixelUI/Reference960x540").transform;
            if (scene == "SampleScene") { Game(root); ShogunEnvironment.Apply(); } else Menu(root);
            Leaderboard(root, scene == "SampleScene", firebaseConfig);
            if (!Object.FindObjectOfType<SfxPlayer>(true)) new GameObject("SfxPlayer").AddComponent<SfxPlayer>();
            foreach (var canvas in Object.FindObjectsOfType<Canvas>(true).Where(c =>
                c.name == "PixelUI" || c.name == "PixelOverlay" || c.name == "HowToPlay" || c.name == "LeaderboardModal"))
                Skin(canvas.gameObject, confirm, back);
            if (scene == "SampleScene") SlayerPresentation(root);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        foreach (var name in new[] { "Panel", "Button", "StatBar", "LeaderboardRow" })
        {
            var path = Generated + name + ".prefab";
            var prefab = PrefabUtility.LoadPrefabContents(path);
            try { Skin(prefab, confirm, back, false); PrefabUtility.SaveAsPrefabAsset(prefab, path); }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }
    }

    static void Menu(Transform root)
    {
        ShogunEnvironment.Menu(root);
        Place(root, "MenuPanel", 264, 76, 432, 312);
        Place(root, "How to PlayButton", 264, 398, 138, 40);
        Place(root, "AudioPreferences", 264, 450, 432, 76);
        foreach (var path in new[] { "Title", "NameLabel", "NameError", "PlayButton" })
        {
            var rect = At(root, "MenuPanel/" + path); var size = rect.sizeDelta;
            size.x = 384; rect.sizeDelta = size;
        }
        var menu = Object.FindObjectOfType<MainMenuUI>(true);
        var input = Get<TMP_InputField>(menu, "nameInput").GetComponent<RectTransform>();
        input.sizeDelta = new Vector2(384, input.sizeDelta.y);
        Place(root, "MenuPanel/PlayButton/key-enter", 344, 8, 24, 24);
        Place(root, "AudioPreferences/MusicVolume", 162, 6, 270, 12);
        Place(root, "AudioPreferences/SFXVolume", 162, 44, 270, 12);
        var credits = root.Find("Credits").GetComponent<TMP_Text>();
        credits.gameObject.SetActive(false); // credits ship alongside the game
    }

    static void Game(Transform root)
    {
        var combat = At(root, "CombatViewport"); Box(combat, root, 16, 76, 928, 340);

        var typing = Object.FindObjectOfType<TypingManager>(true);
        var tray = Get<Slider>(typing, "countdownSlider").transform.parent as RectTransform;
        Box(tray, root, 144, 436, 672, 56);
        var rail = Get<RectTransform>(typing, "rail");
        rail.anchoredPosition = new Vector2(-140, 2);
        var timer = Get<Slider>(typing, "countdownSlider"); Box(timer.GetComponent<RectTransform>(), tray, 16, 44, 640, 6);
        var feedback = Ensure<TypingPanelFeedback>(tray.gameObject);
        var feedbackSettings = new SerializedObject(feedback);
        var borders = feedbackSettings.FindProperty("borders"); borders.arraySize = 4;
        var positions = new[] { new Rect(0, 0, 672, 2), new Rect(0, 54, 672, 2), new Rect(0, 0, 2, 56), new Rect(670, 0, 2, 56) };
        for (int i = 0; i < 4; i++)
        {
            string name = i == 0 ? "ErrorBorder" : "ErrorBorder" + i;
            var edge = tray.Find(name) as RectTransform;
            if (!edge) edge = Rect(name, tray);
            var box = positions[i]; Box(edge, tray, box.x, box.y, box.width, box.height);
            borders.GetArrayElementAtIndex(i).objectReferenceValue = Image(edge, Jade);
        }
        feedbackSettings.ApplyModifiedPropertiesWithoutUndo();
        Value(feedback, "idle", Jade); Value(feedback, "error", Vermilion);
        var overlay = GameObject.Find("PixelOverlay/Reference960x540/CombatOverlay").transform;
        Box((RectTransform)overlay, overlay.parent, 0, 0, 960, 540);
        Place(overlay, "OverlayPanel", 256, 28, 448, 484);
        Place(overlay, "OverlayPanel/KeyHint", 24, 454, 400, 24);
        var pause = Object.FindObjectOfType<GamePauseController>(true);
        Ensure<CanvasGroup>(Get<Canvas>(pause, "pauseCanvas").gameObject);
        Value(pause, "pauseTextColor", Paper); Value(pause, "gameOverTextColor", Vermilion);
        Value(pause, "thanksTextColor", Jade);
        var enemy = Object.FindObjectOfType<SkeletonController>(true);
        var cue = enemy.transform.Find("AttackCue").GetComponent<TextMeshPro>();
        cue.fontStyle = FontStyles.Bold; cue.fontSize = 12; cue.color = Gold;
        var camera = Camera.main;
        camera.orthographicSize = 340f / 84f;
        var slayerFX = Object.FindObjectOfType<SlayerScreenFX>(true);
        Value(slayerFX, "normalOrtho", 340f / 84f);
        Value(slayerFX, "slayerOrtho", 340f / 84f * .78f);
        foreach (var behaviour in Object.FindObjectsOfType<MonoBehaviour>(true))
        {
            if (behaviour.GetType().Name != "CinemachineVirtualCamera") continue;
            var so = new SerializedObject(behaviour);
            so.FindProperty("m_Lens.OrthographicSize").floatValue = 340f / 84f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    static void SlayerPresentation(Transform root)
    {
        var fx = Object.FindObjectOfType<SlayerScreenFX>(true);
        var hud = root.Find("HUD");
        var cluster = hud.Find("ScoreCluster") as RectTransform;
        if (!cluster) cluster = Rect("ScoreCluster", hud);
        Box(cluster, hud, 792, 24, 144, 72);
        Box(hud.GetComponentsInChildren<RectTransform>(true).First(t => t.name == "ScoreText"), cluster, 0, 0, 144, 28);
        Box(hud.GetComponentsInChildren<RectTransform>(true).First(t => t.name == "ComboHUD"), cluster, 156, 0, 64, 28);
        var binder = Ensure<ScoreUI_SlayerBinder>(cluster.gameObject);
        Set(binder, "scoreUIRoot", cluster); Set(binder, "targetCam", Camera.main);
        binder.enabled = false;
        var combo = cluster.GetComponentInChildren<ScoreUI_ComboHUD>(true);
        Set(combo, "followScore", null);
        var bars = Get<RectTransform>(fx, "topBar").parent as RectTransform;
        Stretch(bars, root.parent); bars.SetAsFirstSibling();
        foreach (var key in new[] { "topBar", "bottomBar" })
            Get<RectTransform>(fx, key).GetComponent<Image>().color = Color.black;
        var typing = Object.FindObjectOfType<TypingManager>(true);
        var tray = Get<Slider>(typing, "countdownSlider").transform.parent as RectTransform;
        Set(fx, "environment", GameObject.Find("ShogunEnvironment").transform);
        Set(fx, "playerBody", Object.FindObjectOfType<PlayerSlayerMode>(true).GetComponent<BoxCollider2D>());
        Set(fx, "enemyBody", Object.FindObjectOfType<EnemyStamina>(true).GetComponent<BoxCollider2D>());
        Set(fx, "scoreRoot", cluster); Set(fx, "typingTray", tray);
        Set(Object.FindObjectOfType<TutorialOverlay>(true), "typingTray", tray);
        Set(fx, "scoreCaption", hud.Find("ScoreLabel").gameObject);
        Set(fx, "hudPanel", hud.GetComponent<Image>()); Set(fx, "typingPanel", tray.GetComponent<Image>());
        Value(fx, "bodyPadding", 8f);
        var score = Object.FindObjectOfType<ScoreSystem>(true);
        var floating = Get<Canvas>(score, "floatingCanvas");
        Stretch((RectTransform)floating.transform, root);
        var popup = Get<RectTransform>(score, "popupAnchor");
        Box(popup, floating.transform, 600, 406, 1, 1);
        Set(fx, "popupAnchor", popup);
        CombatLayout(root, fx, cluster, tray, popup);
        var scoreSettings = new SerializedObject(score);
        scoreSettings.FindProperty("popupOffset").vector2Value = Vector2.zero;
        scoreSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    static void CombatLayout(Transform root, SlayerScreenFX fx, RectTransform cluster,
        RectTransform tray, RectTransform popup)
    {
        var hud = root.Find("HUD");
        Box((RectTransform)hud, root, 0, 0, 960, 540);
        hud.GetComponent<Image>().enabled = false;
        RectTransform Group(string name)
        {
            var group = hud.Find(name) as RectTransform;
            if (!group) group = Rect(name, hud);
            Box(group, hud, 0, 0, 112, name == "PlayerStatus" ? 6 : 12);
            return group;
        }
        var player = Group("PlayerStatus"); var enemy = Group("EnemyStatus");
        Object.FindObjectOfType<PlayerNameHUD>(true).enabled = false;
        // Preserve object IDs and slider bindings, including subsequent builder passes.
        RectTransform Item(string name) => hud.GetComponentsInChildren<RectTransform>(true).First(t => t.name == name);
        Box(Item("PlayerName"), player, 0, 0, 112, 16);
        Box(Item("PlayerHP"), player, 0, 0, 112, 6);
        Box(Item("EnemyHPLabel"), enemy, 0, 0, 112, 16);
        Item("EnemyHPLabel").GetComponent<TMP_Text>().text = "SKELETON";
        Box(Item("EnemyHP"), enemy, 0, 0, 112, 6);
        Box(Item("EnemyStamina"), enemy, 0, 8, 112, 4);
        var staminaFill = Item("EnemyStamina").GetComponent<Slider>().fillRect.GetComponent<Image>();
        staminaFill.color = Gold;
        PrefabUtility.RecordPrefabInstancePropertyModifications(staminaFill);
        foreach (var name in new[] { "PlayerName", "EnemyHPLabel" })
        {
            Item(name).gameObject.SetActive(false);
            var text = Item(name).GetComponent<TMP_Text>();
            text.fontSize = 16; text.enableAutoSizing = true; text.fontSizeMin = 8; text.fontSizeMax = 16;
            text.alignment = TextAlignmentOptions.Center;
        }
        foreach (var name in new[] { "PlayerHPLabel", "EnemyStaminaLabel", "ScoreLabel" })
            Item(name).gameObject.SetActive(false);
        Box(cluster, hud, 792, 24, 144, 72);
        var score = Item("ScoreText").GetComponent<TMP_Text>();
        Box(score.rectTransform, cluster, 0, 0, 144, 24);
        score.fontSize = 24; score.enableAutoSizing = false;
        score.alignment = TextAlignmentOptions.TopLeft; score.overflowMode = TextOverflowModes.Overflow;
        var combo = cluster.GetComponentInChildren<ScoreUI_ComboHUD>(true);
        Set(combo, "followScore", null);
        var multiplier = Get<TMP_Text>(combo, "multiplierText");
        Box(multiplier.rectTransform, combo.transform, 0, 0, 64, 24);
        multiplier.fontSize = 16; multiplier.enableAutoSizing = false;
        multiplier.alignment = TextAlignmentOptions.TopLeft; multiplier.overflowMode = TextOverflowModes.Overflow;
        Box(Item("ComboDecay"), hud, 424, 424, 112, 4);
        var popupPrefab = Get<FloatingScore>(Object.FindObjectOfType<ScoreSystem>(true), "floatingPrefab");
        popupPrefab.GetComponentInChildren<TMP_Text>(true).fontSize = 18;
        Value(popupPrefab, "riseDistance", 18f);
        Value(popupPrefab, "life", .6f);
        Value(popupPrefab, "fadeDelay", .15f);
        EditorUtility.SetDirty(popupPrefab);
        var layout = Ensure<CombatHUDLayout>(root.gameObject);
        Set(layout, "targetCamera", Camera.main);
        Set(layout, "playerBody", Get<BoxCollider2D>(fx, "playerBody"));
        Set(layout, "enemyBody", Get<BoxCollider2D>(fx, "enemyBody"));
        Set(layout, "playerStatus", player); Set(layout, "enemyStatus", enemy);
        Set(layout, "typingTray", tray); Set(layout, "scoreRoot", cluster); Set(layout, "popupAnchor", popup);
        Set(layout, "scoreText", score); Set(layout, "multiplierText", multiplier);
        Set(layout, "comboRoot", combo.transform); Set(layout, "comboDecay", Item("ComboDecay"));
        Set(fx, "hudLayout", layout);
        var tutorial = Object.FindObjectOfType<TutorialOverlay>(true);
        Set(tutorial, "staminaTarget", Item("EnemyStamina"));
    }

    static void Leaderboard(Transform root, bool game, string config)
    {
        var canvas = Object.FindObjectsOfType<Canvas>(true).FirstOrDefault(c => c.name == "LeaderboardModal");
        if (!canvas) canvas = Canvas("LeaderboardModal", 80);
        var modal = Ensure<LeaderboardModal>(canvas.gameObject);
        var bounds = canvas.transform.Find("Reference960x540") as RectTransform;
        if (!bounds) bounds = DesignRoot(canvas);
        Image(bounds, new Color(.03f, .08f, .07f, .96f)).raycastTarget = true;
        var binder = Object.FindObjectOfType<LeaderboardUIBinder>(true);
        if (!binder)
        {
            var data = new GameObject("LeaderboardData");
            var firebase = data.AddComponent<FirebaseLeaderboard>();
            EditorJsonUtility.FromJsonOverwrite(config, firebase);
            binder = data.AddComponent<LeaderboardUIBinder>(); Set(binder, "leaderboard", firebase);
            PixelUILeaderboard.Build(binder, bounds);
        }
        var panel = Get<ScrollRect>(binder, "scrollRect").transform.parent as RectTransform;
        Box(panel, bounds, 364, 16, 232, 508);
        if (!bounds.Find("CloseButton"))
        {
            var close = Button("Close", bounds, 620, 16, 160, modal.Close); Set(modal, "closeButton", close);
            Button("Refresh", bounds, 620, 64, 160, binder.ForceRefresh);
        }
        var previous = game ? GameObject.Find("PixelOverlay/Reference960x540/CombatOverlay/OverlayPanel").transform : root;
        var group = Ensure<CanvasGroup>(previous.gameObject);
        Set(modal, "previousScreen", group); Set(modal, "panel", bounds.gameObject); Set(modal, "binder", binder);
        if (!previous.Find("Top 1000Button"))
            Button("Top 1000", previous, game ? 24 : 488, game ? 404 : 398, game ? 400 : 208, modal.Open);
        binder.enabled = false; bounds.gameObject.SetActive(false);
        Value(binder, "rowEven", Hex("1A302D")); Value(binder, "rowOdd", Hex("203C36"));
    }

    static SfxEvent Sound(string name, string clip)
    {
        var path = Events + name + ".asset";
        var ev = AssetDatabase.LoadAssetAtPath<SfxEvent>(path);
        if (!ev) { ev = ScriptableObject.CreateInstance<SfxEvent>(); AssetDatabase.CreateAsset(ev, path); }
        var so = new SerializedObject(ev); var clips = so.FindProperty("clips"); clips.arraySize = 1;
        clips.GetArrayElementAtIndex(0).objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(Audio + clip);
        var combat = AssetDatabase.LoadAssetAtPath<SfxEvent>("Assets/Audio/SFX/Events/Combat/Player/ev_impact_normal.asset");
        so.FindProperty("mixerGroup").objectReferenceValue = Get<UnityEngine.Audio.AudioMixerGroup>(combat, "mixerGroup");
        so.ApplyModifiedPropertiesWithoutUndo();
        Value(ev, "volume", .45f); Value(ev, "volumeJitter", 0f); Value(ev, "pitchJitterSemitone", 0f);
        Value(ev, "ignoreListenerPause", true); return ev;
    }

    internal static void Skin(GameObject root, SfxEvent confirm, SfxEvent back, bool behaviours = true)
    {
        foreach (var image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.sprite && AssetDatabase.GetAssetPath(image.sprite).StartsWith(ShogunEnvironment.Art)) continue;
            if (image.sprite && image.sprite.texture.name.StartsWith("key-")) continue;
            if (image.name.StartsWith("ErrorBorder")) continue;
            if (image.name == "Reference960x540") continue;
            image.sprite = null; image.type = UnityEngine.UI.Image.Type.Simple;
            image.color = image.name == "Fill" ? Jade : Coal;
            if (image.name == "CombatOverlay") image.color = new Color(Coal.r, Coal.g, Coal.b, .88f);
            if (image.name == "PixelUI") image.color = Hex("0D1B1B");
            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
        }
        foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            text.color = text.name.Contains("Label") || text.name == "Credits" ? Hex("A5B9AB") : Paper;
            PrefabUtility.RecordPrefabInstancePropertyModifications(text);
        }
        foreach (var slider in root.GetComponentsInChildren<Slider>(true))
            if (slider.fillRect)
            {
                // Flat bars fill their full height; the old 2px inset erased 4px stamina/combo bars.
                slider.fillRect.offsetMin = Vector2.zero; slider.fillRect.offsetMax = Vector2.zero;
                PrefabUtility.RecordPrefabInstancePropertyModifications(slider.fillRect);
                var fill = slider.fillRect.GetComponent<Image>();
                fill.color = slider.name.Contains("HP") ? Vermilion : Jade;
                PrefabUtility.RecordPrefabInstancePropertyModifications(fill);
            }
        foreach (var button in root.GetComponentsInChildren<Button>(true))
        {
            button.targetGraphic.color = Hex("253C36");
            PrefabUtility.RecordPrefabInstancePropertyModifications(button.targetGraphic);
            var label = button.GetComponentInChildren<TMP_Text>(true);
            if (label) Box(label.rectTransform, button.transform, 8, 0,
                ((RectTransform)button.transform).rect.width - 16, 40);
            if (label) PrefabUtility.RecordPrefabInstancePropertyModifications(label.rectTransform);
            button.transition = Selectable.Transition.ColorTint;
            button.spriteState = new SpriteState();
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.white; colors.highlightedColor = new Color(1.7f, 2.3f, 2f);
            colors.selectedColor = colors.highlightedColor; colors.pressedColor = Jade; colors.fadeDuration = .06f;
            button.colors = colors;
            if (behaviours)
            {
                var feedback = Ensure<UIButtonFeedback>(button.gameObject);
                Set(feedback, "sound", button.name == "CloseButton" || button.name == "Main MenuButton" ? back : confirm);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
        }
        foreach (var scrollbar in root.GetComponentsInChildren<Scrollbar>(true))
        {
            var track = scrollbar.GetComponent<Image>(); track.color = Hex("0D1B1B");
            scrollbar.targetGraphic.color = Jade;
            PrefabUtility.RecordPrefabInstancePropertyModifications(track);
            PrefabUtility.RecordPrefabInstancePropertyModifications(scrollbar.targetGraphic);
        }
        foreach (var panel in root.GetComponentsInChildren<RectTransform>(true).Where(t =>
            t.name == "HowToPlayPanel" || t.name == "LeaderboardPanel" || t.name == "OverlayPanel" || t.name == "MenuPanel"))
        { Ensure<CanvasGroup>(panel.gameObject); Ensure<PanelReveal>(panel.gameObject); }
    }
}
