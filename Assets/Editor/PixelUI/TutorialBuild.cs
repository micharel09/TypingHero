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
        foreach (var entry in new[] { ("How to PlayButton", 264f), ("TutorialButton", 411f), ("Top 1000Button", 558f) })
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
        if (!Object.FindObjectOfType<TutorialDirector>(true))
        {
            var director = new GameObject("TutorialDirector").AddComponent<TutorialDirector>();
            var canvas = Canvas("Tutorial", 30);
            var overlay = canvas.gameObject.AddComponent<TutorialOverlay>();
            var bounds = DesignRoot(canvas);
            var panel = PanelAt("TutorialPrompt", bounds, 144, 80, 672, 124);
            var heading = Label("Heading", "", panel, 16, 6, 640, 24, 20);
            var instruction = Label("Instruction", "", panel, 16, 30, 640, 50, 26);
            instruction.enableWordWrapping = true;
            var next = Button("Practice", panel, 16, 82, 240, director.Continue);
            var skip = Button("Skip Tutorial", panel, 480, 82, 176, director.Skip);
            next.navigation = skip.navigation = new Navigation { mode = Navigation.Mode.None };
            var tray = Border("TrayHighlight", bounds, 140, 432, 680, 96);
            Set(overlay, "heading", heading); Set(overlay, "instruction", instruction);
            Set(overlay, "continueButton", next); Set(overlay, "trayHighlight", tray.gameObject);
            Set(director, "overlay", overlay);
        }
        var tutorial = Object.FindObjectOfType<TutorialOverlay>(true);
        if (!tutorial.GetComponent<CanvasGroup>()) tutorial.gameObject.AddComponent<CanvasGroup>();
        var tutorialBounds = tutorial.transform.Find("Reference960x540");
        var prompt = tutorialBounds.Find("TutorialPrompt");
        var background = prompt.Find("InstructionBackground") as RectTransform;
        if (!background) background = Rect("InstructionBackground", prompt);
        Box(background, prompt, 0, 0, 672, 70);
        Image(background, Ink);
        background.SetAsFirstSibling();
        var stamina = tutorialBounds.Find("StaminaHighlight") as RectTransform;
        if (!stamina) stamina = Border("StaminaHighlight", tutorialBounds, 668, 38, 264, 30);
        var cue = tutorialBounds.Find("CueHighlight") as RectTransform;
        if (!cue)
        {
            cue = Border("CueHighlight", tutorialBounds, 0, 0, 48, 52);
            cue.anchorMin = cue.anchorMax = cue.pivot = new Vector2(.5f, .5f);
        }
        Set(tutorial, "staminaHighlight", stamina.gameObject); Set(tutorial, "cueHighlight", cue);
        Set(tutorial, "cue", Object.FindObjectOfType<SkeletonController>(true).transform.Find("AttackCue"));
        Skin(tutorial.gameObject);
        // The cue sits in the gap between actions. Keep that part of the arena visible.
        var promptImage = prompt.GetComponent<Image>();
        promptImage.enabled = false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(promptImage);
        foreach (var edge in tutorial.GetComponentsInChildren<Image>(true))
            if (edge.name.StartsWith("HighlightEdge")) edge.color = Gold;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void Skin(GameObject root) => ShogunTheme.Skin(root,
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"),
        AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Back.asset"));

    static RectTransform Border(string name, Transform parent, float x, float y, float w, float h)
    {
        var border = Rect(name, parent); Box(border, parent, x, y, w, h);
        var edges = new[] { new Rect(0, 0, w, 2), new Rect(0, h - 2, w, 2), new Rect(0, 0, 2, h), new Rect(w - 2, 0, 2, h) };
        for (int i = 0; i < edges.Length; i++)
        {
            var edge = Rect("HighlightEdge" + i, border); var box = edges[i];
            Box(edge, border, box.x, box.y, box.width, box.height); Image(edge, Gold);
        }
        return border;
    }
}
