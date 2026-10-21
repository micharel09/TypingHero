using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class CompactMenusBuild
{
    internal static void Settings(Transform design, Transform sourcePanel, bool paused)
    {
        var existing = Object.FindObjectOfType<SettingsModal>(true);
        if (existing)
        {
            Style(existing.transform.Find("Reference960x540"));
            StyleButton((paused ? sourcePanel : design).Find("SettingsButton").GetComponent<Button>(), 420);
            return;
        }
        var audio = (RectTransform)(paused ? sourcePanel : design).Find("AudioPreferences");
        var canvas = Canvas("SettingsModal", 90);
        var modal = canvas.gameObject.AddComponent<SettingsModal>();
        var bounds = DesignRoot(canvas);
        Image(bounds, new Color(.03f, .025f, .03f, .85f)).raycastTarget = true;
        var card = Rect("SettingsPanel", bounds);
        Box(card, bounds, 270, 158, 420, 224);
        var face = card.gameObject.AddComponent<Image>();
        EditorUtility.CopySerialized(sourcePanel.GetComponent<Image>(), face);
        face.raycastTarget = true;
        var sourceTitle = sourcePanel.Find(paused ? "PauseLabel" : "NameLabel").GetComponent<TMP_Text>();
        var title = Object.Instantiate(sourceTitle, card);
        title.name = "Title"; title.text = "Settings";
        title.gameObject.SetActive(true);
        title.fontSize = title.fontSizeMin = title.fontSizeMax = 30;
        title.alignment = TextAlignmentOptions.Midline;
        Box(title.rectTransform, card, 24, 12, 372, 40);
        var template = sourcePanel.Find(paused ? "RestartButton" : "PlayButton").GetComponent<Button>();
        var back = CloneButton(template, "Back", card, 24, 164, 372, modal.Close);
        var group = (paused ? sourcePanel : design).GetComponent<CanvasGroup>();
        if (!group) group = (paused ? sourcePanel : design).gameObject.AddComponent<CanvasGroup>();
        Set(modal, "panel", bounds.gameObject); Set(modal, "audio", audio);
        Set(modal, "previousScreen", group); Set(modal, "backButton", back);
        var open = CloneButton(template, "Settings", paused ? sourcePanel : design,
            paused ? 24 : 270, paused ? 0 : 466, 420, modal.Open);
        if (paused) sourcePanel.gameObject.AddComponent<CompactPauseLayout>();
        else audio.gameObject.SetActive(false);
        Style(bounds);
        bounds.gameObject.SetActive(false);
    }

    internal static void Style(Transform bounds)
    {
        Skin(bounds);
        var modal = bounds.GetComponentInParent<SettingsModal>();
        var audio = Get<RectTransform>(modal, "audio");
        foreach (var label in audio.GetComponentsInChildren<TMP_Text>(true)) label.color = Hex("f2e7cf");
        var card = bounds.Find("SettingsPanel");
        var title = card.Find("Title").GetComponent<TMP_Text>();
        title.fontSize = title.fontSizeMin = title.fontSizeMax = 30;
        StyleButton(card.Find("BackButton").GetComponent<Button>(), 372);
    }
    static void StyleButton(Button button, float width)
    {
        Skin(button.transform);
        var label = button.GetComponentInChildren<TMP_Text>(true);
        label.fontSize = label.fontSizeMin = label.fontSizeMax = 24;
        Box(label.rectTransform, button.transform, 8, 0, width - 16, 36);
    }

    internal static void Skin(Transform root)
    {
        var normal = AssetDatabase.LoadAssetAtPath<Sprite>(CombatR7Build.Art + "DarkFantasy/secondary-frame.asset");
        var focused = AssetDatabase.LoadAssetAtPath<Sprite>(CombatR7Build.Art + "DarkFantasy/secondary-frame-gold.asset");
        foreach (var image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.name.Contains("Panel"))
            {
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(CombatR7Build.Art + "DarkFantasy/modal-normal.png");
                image.color = Color.white;
            }
            if (image.GetComponent<LeaderboardRowView>()) { image.sprite = normal; image.color = Color.white; }
            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
        }
        foreach (var button in root.GetComponentsInChildren<Button>(true))
        {
            var image = (Image)button.targetGraphic;
            image.sprite = normal; image.color = Color.white;
            button.spriteState = new SpriteState { highlightedSprite = focused, selectedSprite = focused,
                pressedSprite = focused, disabledSprite = normal };
            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
        }
        foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            label.color = Hex("f2e7cf");
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
        }
    }

    static Button CloneButton(Button template, string caption, Transform parent,
        float x, float y, float width, UnityEngine.Events.UnityAction handler)
    {
        var button = Object.Instantiate(template, parent);
        button.name = caption + "Button";
        button.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(button.onClick, handler);
        foreach (Transform child in button.transform)
            if (!child.GetComponent<TMP_Text>()) Object.DestroyImmediate(child.gameObject);
        Box((RectTransform)button.transform, parent, x, y, width, 36);
        var label = button.GetComponentInChildren<TMP_Text>(true);
        label.text = caption;
        label.fontSize = label.fontSizeMin = label.fontSizeMax = 24;
        Box(label.rectTransform, button.transform, 8, 0, width - 16, 36);
        Skin(button.transform);
        return button;
    }
}
