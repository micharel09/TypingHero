using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

// Final menu composition after the legacy scene/theme passes. Preserve control identities.
internal static class DungeonMainMenuBuild
{
    internal const string Background = "Assets/Art/UI/DungeonV2/menu-dungeon.png";
    static RectTransform At(Transform parent, string name) => (RectTransform)parent.Find(name);
    static void Text(TMP_Text label, float size, TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft)
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(BlackletterTitleBuild.Art + "CobblestoneUI64.asset");
        label.font = font; label.fontSharedMaterial = font.material;
        label.fontSize = label.fontSizeMin = label.fontSizeMax = size;
        label.enableAutoSizing = false; label.enableWordWrapping = false;
        label.alignment = alignment; label.color = Hex("f2e7cf");
    }
    static Button Action(Transform root, string name, UnityEngine.Events.UnityAction callback)
    {
        var existing = root.Find(name + "Button");
        var button = existing ? existing.GetComponent<Button>() : Button(name, root, 0, 0, 240, null);
        if (callback != null)
        {
            button.onClick = new Button.ButtonClickedEvent();
            UnityEventTools.AddPersistentListener(button.onClick, callback);
        }
        if (!existing) ShogunTheme.Skin(button.gameObject,
            AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Confirm.asset"),
            AssetDatabase.LoadAssetAtPath<SfxEvent>(ShogunTheme.Events + "Back.asset"));
        return button;
    }
    internal static void Apply(Transform root)
    {
        var menu = Object.FindObjectOfType<MainMenuUI>(true);
        var panel = At(root, "MenuPanel");
        // Create Settings before changing the original Play template.
        CompactMenusBuild.Settings(root, panel, false);
        var group = root.GetComponent<CanvasGroup>();
        var panelGroup = panel.GetComponent<CanvasGroup>() ?? panel.gameObject.AddComponent<CanvasGroup>();
        panelGroup.ignoreParentGroups = true;
        panelGroup.interactable = panelGroup.blocksRaycasts = true;
        Box(panel, root, 270, 110, 420, 320);
        panel.GetComponent<Image>().raycastTarget = true;
        var title = panel.Find("Title").GetComponent<TMP_Text>();
        title.text = "Play"; Box(title.rectTransform, panel, 24, 10, 372, 40); Text(title, 30, TextAlignmentOptions.Midline);
        var nameLabel = panel.Find("NameLabel").GetComponent<TMP_Text>();
        nameLabel.text = "Player name"; Box(nameLabel.rectTransform, panel, 24, 54, 372, 28); Text(nameLabel, 24);
        var input = Get<TMP_InputField>(menu, "nameInput");
        Box((RectTransform)input.transform, panel, 24, 84, 372, 40);
        input.pointSize = 27; Text(input.textComponent, 27);
        if (input.placeholder is TMP_Text hint) Text(hint, 27);
        var error = Get<TMP_Text>(menu, "errorLabel");
        Box(error.rectTransform, panel, 24, 126, 372, 28); Text(error, 20); error.color = Hex("f16b67");
        var difficulty = At(panel, "Difficulty"); Box(difficulty, panel, 24, 158, 372, 36);
        var caption = difficulty.Find("Caption").GetComponent<TMP_Text>();
        caption.text = "Difficulty"; Box(caption.rectTransform, difficulty, 0, 0, 112, 36); Text(caption, 24);
        foreach (var entry in new[] { ("<Button", 112f, 36f), ("NormalButton", 148f, 188f), (">Button", 336f, 36f) })
        {
            var rect = At(difficulty, entry.Item1); Box(rect, difficulty, entry.Item2, 0, entry.Item3, 36);
            var label = rect.GetComponentInChildren<TMP_Text>(true);
            Box(label.rectTransform, rect, 4, 0, entry.Item3 - 8, 36); Text(label, 24, TextAlignmentOptions.Midline);
        }
        var description = panel.Find("DifficultyHint").GetComponent<TMP_Text>();
        Box(description.rectTransform, panel, 24, 200, 372, 36); Text(description, 20);
        var start = Get<Button>(menu, "playButton");
        Box((RectTransform)start.transform, panel, 216, 260, 180, 36);
        var startLabel = start.GetComponentInChildren<TMP_Text>(true);
        startLabel.text = "Start"; Box(startLabel.rectTransform, start.transform, 8, 0, 164, 36); Text(startLabel, 24, TextAlignmentOptions.Midline);
        var key = start.transform.Find("key-enter"); if (key) key.gameObject.SetActive(false);
        var back = Action(panel, "Back", menu.ClosePreparation);
        Box((RectTransform)back.transform, panel, 24, 260, 180, 36);
        var backLabel = back.GetComponentInChildren<TMP_Text>(true);
        Box(backLabel.rectTransform, back.transform, 8, 0, 164, 36); Text(backLabel, 24, TextAlignmentOptions.Midline);
        CompactMenusBuild.Skin(back.transform);
        back.transition = Selectable.Transition.SpriteSwap;
        back.colors = ColorBlock.defaultColorBlock;
        var backImage = (Image)back.targetGraphic;
        backImage.type = UnityEngine.UI.Image.Type.Sliced; backImage.pixelsPerUnitMultiplier = 1f / 3f;
        if (!back.GetComponent<DungeonControlSkin>()) back.gameObject.AddComponent<DungeonControlSkin>();
        foreach (var control in panel.GetComponentsInChildren<Selectable>(true)) control.navigation = new Navigation { mode = Navigation.Mode.None };
        var buttons = new[] { Action(root, "Play", menu.OpenPlay), Action(root, "Tutorial", menu.Tutorial),
            Action(root, "Top 1000", null), Action(root, "Settings", null), Action(root, "How to Play", menu.ShowHelp) };
        var serialized = new SerializedObject(menu);
        var items = serialized.FindProperty("menuButtons"); items.arraySize = buttons.Length;
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i]; Box((RectTransform)button.transform, root, 624, 272 + i * 44, 240, 36);
            // Explicit empty links keep pointer selection while MainMenuUI owns arrow/Tab movement.
            button.interactable = true; button.navigation = new Navigation { mode = Navigation.Mode.Explicit };
            button.transition = Selectable.Transition.None;
            button.colors = ColorBlock.defaultColorBlock; button.spriteState = new SpriteState();
            var skin = button.GetComponent<DungeonControlSkin>(); if (skin) Object.DestroyImmediate(skin);
            var image = (Image)button.targetGraphic; image.sprite = null; image.type = UnityEngine.UI.Image.Type.Simple; image.color = Color.clear;
            image.pixelsPerUnitMultiplier = 1;
            var label = button.GetComponentInChildren<TMP_Text>(true);
            label.text = new[] { "Play", "Tutorial", "Top 1000", "Settings", "How to Play" }[i];
            Box(label.rectTransform, button.transform, 24, 0, 216, 36); Text(label, 30, TextAlignmentOptions.MidlineRight);
            var marker = button.transform.Find("Selection") as RectTransform;
            if (!marker) marker = Rect("Selection", button.transform);
            Box(marker, button.transform, 8, 15, 6, 6); Image(marker, Hex("dcc18a"));
            marker.gameObject.SetActive(false);
            items.GetArrayElementAtIndex(i).objectReferenceValue = button;
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
        Set(menu, "preparation", panel.gameObject); Set(menu, "preparationTitle", title);
        Set(menu, "menuActions", group); Set(menu, "backButton", back);
        var logo = root.Find("Logo")?.GetComponent<TMP_Text>();
        if (!logo) logo = Label("Logo", "Clackblade", root, 32, 24, 400, 68, 56);
        Box(logo.rectTransform, root, 32, 24, 400, 68);
        logo.color = Hex("f2e7cf"); logo.overflowMode = TextOverflowModes.Overflow;
        var background = At(root.parent, "DungeonMenu");
        foreach (Transform layer in background) layer.gameObject.SetActive(false);
        var importer = (TextureImporter)AssetImporter.GetAtPath(Background);
        importer.textureShape = TextureImporterShape.Texture2D;
        importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
        importer.spriteBorder = Vector4.zero; importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false; importer.wrapMode = TextureWrapMode.Clamp;
        importer.npotScale = TextureImporterNPOTScale.None; importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        var backdrop = Image(background, Color.white); backdrop.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Background);
        if (!backdrop.sprite) throw new System.InvalidOperationException("Dungeon menu background did not import as a sprite");
        backdrop.preserveAspect = true;
        // The preparation card must cover the menu's focus marks as well as its controls.
        panel.SetAsLastSibling();
        panel.gameObject.SetActive(false);
        foreach (var component in root.GetComponentsInChildren<Component>(true))
        {
            EditorUtility.SetDirty(component);
            PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }
    }
}
