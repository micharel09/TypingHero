using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

// Final non-combat presentation pass. Never replaces controls or their events.
internal static class DungeonUISkin
{
    internal const string Art = "Assets/Art/UI/DungeonV2/";
    static Sprite Sprite(string name, bool metal = false)
    {
        if (name.StartsWith("secondary-frame"))
            return AssetDatabase.LoadAssetAtPath<Sprite>(Art + name + ".asset");
        return AssetDatabase.LoadAssetAtPath<Sprite>((metal ? CombatR7Build.Art : Art) + name + ".png");
    }
    static void Frame(Image image, string name, bool metal = false)
    {
        image.sprite = Sprite(name, metal);
        if (!image.sprite) throw new FileNotFoundException("Missing UI component: " + name);
        image.type = UnityEngine.UI.Image.Type.Sliced; image.color = Color.white;
        // Component pixels are authored on320x180; UGUI geometry uses960x540.
        image.pixelsPerUnitMultiplier = metal ? 1f / 3f : 1f;
        PrefabUtility.RecordPrefabInstancePropertyModifications(image);
    }
    internal static void Apply()
    {
        // Reuse the metal rim after the ten-pixel socket reserve. No raster is changed.
        foreach (string name in new[] { "secondary-frame", "secondary-frame-gold" })
        {
            string path = Art + name + ".asset";
            if (AssetDatabase.LoadAssetAtPath<Sprite>(path)) continue;
            var source = AssetDatabase.LoadAssetAtPath<Sprite>(CombatR7Build.Art + name + ".png");
            var rect = source.rect; rect.xMin += 10;
            var rim = UnityEngine.Sprite.Create(source.texture, rect, new Vector2(.5f, .5f),
                source.pixelsPerUnit, 0, SpriteMeshType.FullRect, new Vector4(2, 2, 2, 2));
            rim.name = name;
            AssetDatabase.CreateAsset(rim, path);
        }
        foreach (string path in Directory.GetFiles(Art, "*.png"))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.spriteBorder = Path.GetFileName(path).StartsWith("keycap-") ? Vector4.zero : new Vector4(6, 6, 6, 6);
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point; importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        foreach (var sceneName in new[] { "MainMenu", "SampleScene" })
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
            foreach (var canvas in Object.FindObjectsOfType<Canvas>(true))
                if (canvas.name == "HowToPlay" || canvas.name == "LeaderboardModal" ||
                    canvas.name == "PixelOverlay" || canvas.name == "Tutorial" ||
                    (sceneName == "MainMenu" && canvas.name == "PixelUI")) Skin(canvas.gameObject);
            foreach (var binder in Object.FindObjectsOfType<LeaderboardUIBinder>(true))
            {
                // The sprite owns its coal face; tint alternating rows without blackening the metal rim.
                Value(binder, "rowEven", Color.white); Value(binder, "rowOdd", Hex("afa1b3"));
                var settings = new SerializedObject(binder);
                var frames = settings.FindProperty("modeFrames"); frames.arraySize = 4;
                var bounds = Get<ScrollRect>(binder, "scrollRect").transform.parent.parent;
                string[] tabs = { "EndlessButton", "ClassicButton", "OpponentsButton", "AchievementsButton" };
                for (int i = 0; i < tabs.Length; i++)
                    frames.GetArrayElementAtIndex(i).objectReferenceValue = bounds.Find(tabs[i]).GetComponent<Image>();
                settings.ApplyModifiedPropertiesWithoutUndo();
                Set(binder, "activeModeFrame", Sprite("secondary-frame-gold", true));
                Set(binder, "inactiveModeFrame", Sprite("secondary-frame", true));
            }
            foreach (var pause in Object.FindObjectsOfType<GamePauseController>(true))
            {
                Value(pause, "pauseTextColor", Hex("f2e7cf"));
                Value(pause, "gameOverTextColor", Hex("f16b67"));
                Value(pause, "thanksTextColor", Hex("dcc18a"));
            }
            if (sceneName == "MainMenu") Menu();
            EditorSceneManager.SaveScene(scene);
        }
        foreach (string name in new[] { "Panel", "Button", "LeaderboardRow" })
        {
            string path = Generated + name + ".prefab";
            var prefab = PrefabUtility.LoadPrefabContents(path);
            try { Skin(prefab, false); PrefabUtility.SaveAsPrefabAsset(prefab, path); }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }
    }
    static void Skin(GameObject root, bool tintControls = true)
    {
        foreach (var image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.name.StartsWith("HighlightEdge")) continue;
            if (image.name == "key-enter" || image.name == "key-r")
            {
                image.sprite = Sprite(image.name.Replace("key-", "keycap-")); image.color = Color.white;
                image.type = UnityEngine.UI.Image.Type.Simple;
                PrefabUtility.RecordPrefabInstancePropertyModifications(image);
                continue;
            }
            if (image.name == "Reference960x540")
            {
                image.color = new Color(.035f, .03f, .067f, .94f);
                PrefabUtility.RecordPrefabInstancePropertyModifications(image);
                continue;
            }
            if (image.name == "PixelUI" || image.name == "MenuEnvironment") continue;
            image.color = Hex("14111e");
            if (image.name == "Fill" || image.name == "Handle") image.color = Hex("a3844d");
            if (image.name.Contains("Panel") || image.name == "Panel" || image.name == "InstructionBackground")
                Frame(image, image.name == "MenuPanel" ? "panel-normal" : "modal-normal");
            if (image.name == "InstructionBackground")
            {
                // Existing text extends to y80; keep the rim below both lines without moving content/actions.
                var size = image.rectTransform.sizeDelta; size.y = 80; image.rectTransform.sizeDelta = size;
                PrefabUtility.RecordPrefabInstancePropertyModifications(image.rectTransform);
            }
            if (image.GetComponent<LeaderboardRowView>()) Frame(image, "secondary-frame", true);
            if (image.name == "CombatOverlay") image.color = new Color(.035f, .03f, .067f, .88f);
            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
        }
        foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            text.color = Hex(text.name == "NameError" ? "f16b67" :
                text.name == "Title" || text.name == "Heading" ? "dcc18a" :
                text.name.Contains("Label") || text.name == "DifficultyHint" ? "afa1b3" : "f2e7cf");
            PrefabUtility.RecordPrefabInstancePropertyModifications(text);
        }
        foreach (var control in root.GetComponentsInChildren<Selectable>(true))
        {
            if (control is Button || control is TMP_InputField)
            {
                if (control.targetGraphic is Image image) Frame(image, "secondary-frame", true);
                control.transition = Selectable.Transition.SpriteSwap;
                control.spriteState = new SpriteState {
                    highlightedSprite = Sprite("secondary-frame-gold", true),
                    selectedSprite = Sprite("secondary-frame-gold", true),
                    pressedSprite = Sprite("secondary-frame-gold", true),
                    disabledSprite = Sprite("secondary-frame", true)
                };
                control.colors = ColorBlock.defaultColorBlock;
                if (tintControls && !control.GetComponent<DungeonControlSkin>()) control.gameObject.AddComponent<DungeonControlSkin>();
            }
            else
            {
                var colors = ColorBlock.defaultColorBlock;
                colors.highlightedColor = colors.selectedColor = Hex("dcc18a");
                colors.pressedColor = Hex("a3844d"); colors.disabledColor = Hex("58515f");
                control.colors = colors;
            }
            if (control is TMP_InputField input)
            {
                input.caretColor = Hex("dcc18a"); input.customCaretColor = true;
                input.selectionColor = new Color(.64f, .52f, .3f, .5f);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(control);
        }
    }
    static void Menu()
    {
        var root = GameObject.Find("PixelUI/Reference960x540").transform;
        var oldWorld = GameObject.Find("WorldMenuBackdrop");
        if (oldWorld)
        {
            oldWorld.GetComponent<WorldMenuBackdrop>().enabled = false;
            foreach (var renderer in oldWorld.GetComponentsInChildren<SpriteRenderer>(true)) renderer.enabled = false;
        }
        var old = root.Find("MenuEnvironment"); if (old) old.gameObject.SetActive(false);
        var background = root.parent.Find("DungeonMenu") as RectTransform;
        if (!background) background = Rect("DungeonMenu", root);
        Stretch(background, root.parent); background.SetAsFirstSibling();
        foreach (string layer in new[] { "far", "mid", "floor", "foreground" })
        {
            var rect = background.Find(layer) as RectTransform;
            if (!rect) rect = Rect(layer, background);
            Stretch(rect, background);
            var image = Image(rect, Color.white); image.sprite = Sprite("scene-" + layer, true);
        }
        // Existing content bounds are y=24..526. Center the complete group in the viewport.
        foreach (string name in new[] { "MenuPanel", "How to PlayButton", "TutorialButton", "Top 1000Button", "AudioPreferences" })
        {
            var rect = root.Find(name) as RectTransform;
            var position = rect.anchoredPosition; position.y += 5; rect.anchoredPosition = position;
            PrefabUtility.RecordPrefabInstancePropertyModifications(rect);
        }
        var canvasImage = root.parent.GetComponent<Image>(); if (canvasImage) canvasImage.enabled = false;
        var input = Object.FindObjectOfType<MainMenuUI>(true);
        // Validation remains owned by MainMenuUI; only serialized presentation changes here.
        var field = Get<TMP_InputField>(input, "nameInput");
        var skin = field.GetComponent<DungeonControlSkin>();
        if (!skin) skin = field.gameObject.AddComponent<DungeonControlSkin>();
        Set(skin, "error", Get<TMP_Text>(input, "errorLabel"));
    }
}
