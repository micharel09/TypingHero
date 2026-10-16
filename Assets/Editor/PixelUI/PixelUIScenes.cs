using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class PixelUIScenes
{
    static T Find<T>() where T : Object => Object.FindObjectOfType<T>(true);

    internal static void Menu()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        if (GameObject.Find("PixelUI")) return;
        var menu = Find<MainMenuUI>();
        var input = Get<TMP_InputField>(menu, "nameInput");
        var oldCanvases = Object.FindObjectsOfType<Canvas>(true);
        var canvas = Canvas("PixelUI", 20);
        Image((RectTransform)canvas.transform, Ink);
        var root = DesignRoot(canvas);
        var panel = PanelAt("MenuPanel", root, 272, 114, 416, 312);
        var title = Label("Title", "TYPING HERO", panel, 24, 20, 368, 64, 52);
        title.alignment = TextAlignmentOptions.Center; title.color = Teal;
        Label("NameLabel", "PLAYER NAME", panel, 24, 92, 368, 28, 22).color = Muted;
        Box(input.GetComponent<RectTransform>(), panel, 24, 124, 368, 44);
        var inputImage = Image(input.GetComponent<RectTransform>(), Ink, true);
        inputImage.raycastTarget = true; input.targetGraphic = inputImage;
        input.colors = ColorBlock.defaultColorBlock;
        input.fontAsset = Typeface; input.pointSize = 28;
        Stretch(input.textViewport, input.transform, 8);
        foreach (var t in input.GetComponentsInChildren<TMP_Text>(true)) Style(t, 28);
        Stretch(input.textComponent.rectTransform, input.textViewport);
        input.textComponent.overflowMode = TextOverflowModes.Overflow;
        if (input.placeholder is TMP_Text placeholder)
            Stretch(placeholder.rectTransform, input.textViewport);
        input.textComponent.color = Foreground; input.caretColor = Teal; input.customCaretColor = true;
        input.selectionColor = new Color(.2f, .6f, .6f, .5f);
        if (input.placeholder is TMP_Text hint) { hint.text = "Your name"; hint.color = Muted; }
        var error = Label("NameError", "", panel, 24, 174, 368, 28, 20);
        error.color = Red;
        // MainMenuUI owns its validation and Play listener; no competing handler.
        var play = Button("Play", panel, 24, 224, 368, null);
        KeyIcon("key-enter", play.transform, 328, 8);
        Set(menu, "playButton", play); Set(menu, "errorLabel", error);
        var credit = Label("Credits", "UI: Crusenho (CC BY 4.0)  /  Keys: Kenney (CC0)  /  Typeface: VT323 (OFL)",
            root, 16, 500, 928, 24, 18);
        credit.alignment = TextAlignmentOptions.Center; credit.color = Muted;
        foreach (var c in oldCanvases) DisableCanvas(c);
        if (Camera.main) Camera.main.backgroundColor = Ink;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    internal static void Game()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        if (GameObject.Find("PixelUI")) return;
        var oldCanvases = Object.FindObjectsOfType<Canvas>(true);
        var typing = Find<TypingManager>(); var score = Find<ScoreSystem>();
        var pause = Find<GamePauseController>(); var leaderboard = Find<LeaderboardUIBinder>();
        var hudName = Find<PlayerNameHUD>(); var fx = Find<SlayerScreenFX>();
        // Referenced text/rail objects are moved, preserving their identities and event owners.
        var rail = Get<RectTransform>(typing, "rail");
        var canvas = Canvas("PixelUI", 20);
        var root = DesignRoot(canvas);
        var hud = PanelAt("HUD", root, 16, 16, 688, 80);
        var hp = Bar("PlayerHP", hud, 16, 48, 208, Red);
        var health = hp.gameObject.AddComponent<HealthBarUI>();
        Set(health, "slider", hp); Value(health, "bindPlayer", true);
        var name = Label("PlayerName", "PLAYER", hud, 16, 8, 208, 30, 28);
        Set(hudName, "target", name); Value(hudName, "hideWhenEmpty", false);
        Label("PlayerHPLabel", "PLAYER HP", hud, 16, 32, 208, 16, 16).color = Muted;
        Label("ScoreLabel", "SCORE", hud, 240, 4, 208, 20, 18).color = Muted;
        var scoreLabel = Label("ScoreText", "0", hud, 240, 22, 144, 32, 32);
        Set(score, "scoreText", scoreLabel);
        var comboRoot = Rect("ComboHUD", hud); Box(comboRoot, hud, 384, 22, 64, 32);
        var combo = comboRoot.gameObject.AddComponent<ScoreUI_ComboHUD>();
        var multiplier = Label("Multiplier", "x1.0", comboRoot, 0, 0, 64, 32, 24);
        Set(combo, "multiplierText", multiplier); Set(score, "comboHUD", combo);
        var decay = Bar("ComboDecay", hud, 240, 64, 208, Teal);
        Set(combo, "decaySlider", decay);
        var enemyHp = Bar("EnemyHP", hud, 464, 32, 208, Red);
        health = enemyHp.gameObject.AddComponent<HealthBarUI>();
        Set(health, "slider", enemyHp); Value(health, "bindPlayer", false);
        Label("EnemyHPLabel", "SKELETON HP", hud, 464, 8, 208, 22, 22).color = Muted;
        Label("EnemyStaminaLabel", "ENEMY STAMINA", hud, 464, 46, 208, 16, 16).color = Muted;
        var stamina = Bar("EnemyStamina", hud, 464, 64, 208, Gold);
        var staminaBinding = stamina.gameObject.AddComponent<StaminaBarUI>();
        Set(staminaBinding, "target", Find<EnemyStamina>());
        foreach (var binder in Object.FindObjectsOfType<ScoreUI_SlayerBinder>(true))
            Object.DestroyImmediate(binder);
        var combat = Rect("CombatViewport", root);
        Box(combat, root, 16, 104, 688, 420);
        combat.gameObject.AddComponent<RectMask2D>();
        var typingPanel = PanelAt("TypingPanel", combat, 16, 16, 656, 96);
        Box(rail, typingPanel, 0, 0, 656, 72);
        rail.anchorMin = rail.anchorMax = new Vector2(.5f, .5f);
        rail.pivot = new Vector2(.5f, .5f); rail.anchoredPosition = new Vector2(-140, 8);
        foreach (var key in new[] { "wordText", "nextWordText", "thirdWordText" })
        {
            var label = Get<TextMeshProUGUI>(typing, key);
            Style(label, 32); label.alignment = TextAlignmentOptions.Center;
            label.overflowMode = TextOverflowModes.Overflow;
            label.rectTransform.localScale = Vector3.one;
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(.5f, .5f);
            label.rectTransform.pivot = new Vector2(.5f, .5f);
            label.rectTransform.sizeDelta = new Vector2(300, 44);
            label.color = key == "wordText" ? Teal : Foreground;
        }
        Value(typing, "nextGapPixels", 20f); Value(typing, "nextIdleAlpha", .65f);
        Value(typing, "thirdIdleAlpha", .32f);
        var timer = Bar("WordTimer", typingPanel, 16, 76, 624, Teal);
        Set(typing, "countdownSlider", timer);
        // Keep floating combat scores inside the same masked region.
        var floating = Rect("FloatingScores", combat); Stretch(floating, combat);
        var floatingCanvas = floating.gameObject.AddComponent<Canvas>();
        Set(score, "floatingCanvas", floatingCanvas);
        var popup = Rect("PopupAnchor", floating); Box(popup, floating, 344, 230, 1, 1);
        Set(score, "popupAnchor", popup);
        var prefab = Get<FloatingScore>(score, "floatingPrefab");
        if (prefab)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            var root = PrefabUtility.LoadPrefabContents(path);
            foreach (var text in root.GetComponentsInChildren<TMP_Text>(true)) Style(text, 24);
            PrefabUtility.SaveAsPrefabAsset(root, Generated + "FloatingScore.prefab");
            PrefabUtility.UnloadPrefabContents(root);
            Set(score, "floatingPrefab", AssetDatabase.LoadAssetAtPath<FloatingScore>(Generated + "FloatingScore.prefab"));
        }
        PixelUILeaderboard.Build(leaderboard, root);
        Overlay(pause);
        if (fx)
        {
            var effects = Rect("SlayerBars", combat); Stretch(effects, combat);
            foreach (var key in new[] { "topBar", "bottomBar" })
            {
                var bar = Get<RectTransform>(fx, key);
                bar.SetParent(effects, false); bar.localScale = Vector3.one;
                bool top = key == "topBar";
                bar.anchorMin = new Vector2(0, top ? 1 : 0);
                bar.anchorMax = new Vector2(1, top ? 1 : 0);
                bar.pivot = new Vector2(.5f, top ? 1 : 0);
                bar.anchoredPosition = Vector2.zero; bar.sizeDelta = Vector2.zero;
            }
            effects.SetAsFirstSibling();
        }
        foreach (var slayer in Object.FindObjectsOfType<PlayerSlayerMode>(true))
        {
            Value(slayer, "showDebugHUD", false);
            var so = new SerializedObject(slayer); so.FindProperty("disableDuringSlayer").arraySize = 0;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        foreach (var c in oldCanvases) if (c) DisableCanvas(c);
        var retro = GameObject.Find("retron_computer_bg");
        if (retro) retro.SetActive(false);
        // Match the scene camera to the exact reference-space combat rectangle.
        var cam = Camera.main;
        if (cam) { cam.rect = new Rect(16f / 960, 16f / 540, 688f / 960, 420f / 540); cam.backgroundColor = Ink; }
        var viewportCamera = combat.gameObject.AddComponent<CombatViewportCamera>();
        Set(viewportCamera, "target", cam);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void Overlay(GamePauseController pause)
    {
        var canvas = Canvas("PixelOverlay", 40);
        var root = DesignRoot(canvas);
        var bounds = Rect("CombatOverlay", root);
        Box(bounds, root, 16, 104, 688, 420);
        var shade = Image(bounds, new Color(.03f, .06f, .08f, .88f)); shade.raycastTarget = true;
        var panel = PanelAt("OverlayPanel", bounds, 164, 46, 360, 328);
        var title = Label("PauseLabel", "PAUSE", panel, 16, 24, 328, 64, 40);
        title.alignment = TextAlignmentOptions.Center;
        var thanks = Label("ThanksLabel", "THANKS FOR PLAYING DEMO", panel, 16, 24, 328, 64, 26);
        thanks.alignment = TextAlignmentOptions.Center;
        var resume = Button("Resume", panel, 24, 112, 312, pause.Resume);
        var restart = Button("Restart", panel, 24, 168, 312, pause.Restart);
        KeyIcon("key-r", restart.transform, 16, 8);
        Button("Main Menu", panel, 24, 224, 312, pause.ReturnToMenu);
        Set(pause, "uiRoot", null); Set(pause, "pauseCanvas", canvas);
        Set(pause, "pauseLabel", title); Set(pause, "thanksLabel", thanks);
        Set(pause, "restartHintLabel", null); Set(pause, "menuHintLabel", null);
        Set(pause, "resumeButton", resume);
        Value(pause, "pauseTextColor", Foreground); Value(pause, "gameOverTextColor", Red);
        Value(pause, "thanksTextColor", Teal);
        Label("KeyHint", "R  RESTART    M  MAIN MENU", panel, 16, 280, 328, 24, 18).color = Muted;
    }

    static void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
        var raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster) raycaster.enabled = false;
    }
}
