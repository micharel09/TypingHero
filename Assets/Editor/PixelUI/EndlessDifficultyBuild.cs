using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class EndlessDifficultyBuild
{
    internal static EndlessDifficultySelector Add(Transform parent, float x, float y, float width,
        LeaderboardUIBinder binder = null)
    {
        var root = parent.Find("Difficulty") as RectTransform;
        if (!root) root = Rect("Difficulty", parent);
        Box(root, parent, x, y, width, 40);
        if (!root.Find("Caption")) Label("Caption", "Difficulty:", root, 0, 0, 112, 40, 20);
        var selector = root.GetComponentInChildren<EndlessDifficultySelector>(true);
        if (!selector)
        {
            var center = Button("Normal", root, 152, 0, width - 192, null);
            var graphic = center.targetGraphic;
            Object.DestroyImmediate(center);
            selector = graphic.gameObject.AddComponent<EndlessDifficultySelector>();
            selector.targetGraphic = graphic;
            Set(selector, "caption", selector.GetComponentInChildren<TMP_Text>());
            Button("<", root, 112, 0, 40, selector.Previous);
            Button(">", root, width - 40, 0, 40, selector.Next);
        }
        Set(selector, "table", binder);
        Set(selector, "rules", AssetDatabase.LoadAssetAtPath<ScoreRules>("Assets/Data/Score/ScoreRules.asset"));
        foreach (var button in root.GetComponentsInChildren<Button>(true))
        {
            button.GetComponentInChildren<TMP_Text>(true).fontSize = 20;
            if (button != selector) button.navigation = new Navigation { mode = Navigation.Mode.None };
        }
        return selector;
    }
    internal static void Menu(Transform root, MainMenuUI menu)
    {
        var panel = root.Find("MenuPanel");
        Box((RectTransform)panel, root, 264, 24, 432, 364);
        var selector = Add(panel, 24, 204, 384);
        var hint = panel.Find("DifficultyHint")?.GetComponent<TMP_Text>();
        if (!hint) hint = Label("DifficultyHint", "", panel, 24, 246, 384, 24, 20);
        Set(selector, "description", hint);
        selector.Display(EndlessDifficulty.Normal);
        var play = Get<Button>(menu, "playButton");
        Box((RectTransform)play.transform, panel, 24, 280, 384, 40);
        var input = Get<TMP_InputField>(menu, "nameInput");
        selector.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = input, selectOnDown = play };
        Set(menu, "difficultySelector", selector);
    }
}
