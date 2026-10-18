using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

// Re-skin serialized objects in place, including hidden help and ending screens.
internal static class DarkAgesSkin
{
    internal const string ArtPath = "Assets/Art/UI/DarkAges/";
    static Sprite[] sprites;
    internal static Sprite Sprite(string name) => sprites.FirstOrDefault(s => s.name == name)
        ?? throw new System.InvalidOperationException("Missing Dark Ages slice " + name + "; loaded: " +
            string.Join(",", sprites.Select(s => s.name)));

    internal static void Prepare()
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(ArtPath + "32x32-Tilesheet.png");
        importer.textureType = TextureImporterType.Sprite;
        importer.textureShape = TextureImporterShape.Texture2D;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 100;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.SaveAndReimport();
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        var slices = new[] {
            Slice("Panel", 0, 0, 96, 96, 24),
            Slice("Inset", 210, 18, 60, 60, 12),
            Slice("Row", 320, 106, 32, 16, 2),
            Slice("Button", 0, 104, 64, 20, 8, 6),
            Slice("ButtonPressed", 64, 104, 64, 20, 8, 6),
            Slice("ButtonSelected", 128, 104, 64, 20, 8, 6),
            Slice("Bar", 104, 140, 80, 9, 6, 3),
            Slice("BlueFill", 296, 144, 82, 4, 2, 1),
            Slice("RedFill", 200, 176, 80, 4, 2, 1),
            Slice("GoldFill", 36, 236, 24, 7, 4, 2),
            Slice("ScrollTrack", 12, 132, 7, 84, 2, 8),
            Slice("ScrollHandle", 12, 228, 7, 24, 2, 2)
        };
        foreach (var slice in slices)
            slice.spriteID = previous.FirstOrDefault(s => s.name == slice.name)?.spriteID ?? GUID.Generate();
        provider.SetSpriteRects(slices);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(
            slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply();
        importer.SaveAndReimport();
        sprites = AssetDatabase.LoadAllAssetsAtPath(ArtPath + "32x32-Tilesheet.png").OfType<Sprite>().ToArray();
    }

    static SpriteRect Slice(string name, int x, int top, int w, int h, int borderX, int borderY = -1)
        => new SpriteRect { name = name, rect = new Rect(x, 352 - top - h, w, h),
            pivot = new Vector2(.5f, .5f), alignment = SpriteAlignment.Center,
            border = new Vector4(borderX, borderY < 0 ? borderX : borderY,
                borderX, borderY < 0 ? borderX : borderY) };

    internal static void SetImage(Image image, string role)
    {
        image.sprite = Sprite(role);
        image.type = role == "Panel" || role == "Inset" || role == "GoldFill"
            ? UnityEngine.UI.Image.Type.Tiled : UnityEngine.UI.Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = image.name == "LeaderboardPanel" ? 2 : 1;
        image.color = Color.white;
        PrefabUtility.RecordPrefabInstancePropertyModifications(image);
    }

    internal static void ButtonSkin(Button button)
    {
        SetImage((Image)button.targetGraphic, "Button");
        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = new SpriteState { highlightedSprite = Sprite("ButtonSelected"),
            selectedSprite = Sprite("ButtonSelected"), pressedSprite = Sprite("ButtonPressed"),
            disabledSprite = Sprite("ButtonPressed") };
        PrefabUtility.RecordPrefabInstancePropertyModifications(button);
    }

    internal static void BarSkin(Slider slider)
    {
        SetImage(slider.GetComponent<Image>(), "Bar");
        var role = slider.name.Contains("HP") ? "RedFill" :
            slider.name.Contains("Stamina") ? "GoldFill" : "BlueFill";
        SetImage(slider.fillRect.GetComponent<Image>(), role);
    }

    internal static void Apply()
    {
        foreach (var scene in new[] { "MainMenu", "SampleScene" })
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + scene + ".unity");
            foreach (var canvas in Object.FindObjectsOfType<Canvas>(true)
                .Where(c => c.name == "PixelUI" || c.name == "PixelOverlay" || c.name == "HowToPlay"))
                ApplyRoot(canvas.gameObject);
            foreach (var binder in Object.FindObjectsOfType<LeaderboardUIBinder>(true))
            {
                Value(binder, "rowEven", Color.white);
                Value(binder, "rowOdd", Hex("D6D6D6"));
            }
            foreach (var camera in Object.FindObjectsOfType<Camera>(true).Where(c => c.name == "PixelBackgroundCamera"))
                camera.backgroundColor = Ink;
            var pause = Object.FindObjectOfType<GamePauseController>(true);
            if (pause)
            {
                Value(pause, "pauseTextColor", Gold);
                Value(pause, "gameOverTextColor", Gold);
                Value(pause, "thanksTextColor", Gold);
            }
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        foreach (var path in new[] { "Panel", "Button", "StatBar", "LeaderboardRow" })
        {
            var root = PrefabUtility.LoadPrefabContents(Generated + path + ".prefab");
            try { ApplyRoot(root); PrefabUtility.SaveAsPrefabAsset(root, Generated + path + ".prefab"); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
    }

    static void ApplyRoot(GameObject root)
    {
        foreach (var image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.sprite && (image.sprite == Frame || image.sprite.texture.name == "32x32-Tilesheet"))
                SetImage(image, image.name == "HUD" || image.name == "TypingPanel" ||
                    image.GetComponent<TMP_InputField>() ? "Inset" : "Panel");
            if (image.name == "PixelUI") image.color = Ink;
            if (image.name == "LBRowView" || image.name == "LeaderboardRow") SetImage(image, "Row");
        }
        foreach (var button in root.GetComponentsInChildren<Button>(true)) ButtonSkin(button);
        foreach (var slider in root.GetComponentsInChildren<Slider>(true)
            .Where(s => new[] { "PlayerHP", "EnemyHP", "EnemyStamina", "WordTimer", "ComboDecay",
                "MusicVolume", "SFXVolume", "StatBar" }.Contains(s.name))) BarSkin(slider);
        foreach (var scrollbar in root.GetComponentsInChildren<Scrollbar>(true))
        {
            SetImage(scrollbar.GetComponent<Image>(), "ScrollTrack");
            SetImage((Image)scrollbar.targetGraphic, "ScrollHandle");
        }
        foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (label.color == Hex("9AAFB9")) label.color = Muted;
            if (label.color == Hex("F2C66D")) label.color = Gold;
            if (label.name == "Title" || label.name == "PauseLabel" || label.name == "ThanksLabel") label.color = Gold;
            if (label.name == "Credits") label.text = "UI: Hypnobius / Keys: Kenney (CC0) / Typeface: VT323 (OFL)";
            PrefabUtility.RecordPrefabInstancePropertyModifications(label);
        }
    }
}
