using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class ParryCueBuild
{
    internal const string Art = "Assets/Art/UI/ParryCue/";
    internal static bool Owns(Component component)
    {
        for (var node = component.transform; node; node = node.parent)
            if (node.name == "SpaceCueSample" || node.name == "SpaceExample" || node.GetComponent<ParryCuePresenter>()) return true;
        return false;
    }

    internal static void Apply()
    {
        foreach (var path in Directory.GetFiles(Art, "*.png"))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        foreach (var tone in new[] { "dark", "light", "warning" })
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Art + "SimpleKeys/space-" + tone + ".png");
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp; importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritesheet = new[] { "released", "pressed" }.Select((state, i) => new SpriteMetaData {
                name = tone + "_" + state, rect = new Rect(i * 67, 0, 67, 16),
                alignment = 0, pivot = Vector2.one * .5f,
                border = tone == "dark" ? new Vector4(3, 3, 3, 3) : new Vector4(34, 4, 3, 10)
            }).ToArray();
            importer.SaveAndReimport();
        }
        Sprite Cap(string tone, string state) => AssetDatabase.LoadAllAssetsAtPath(Art + "SimpleKeys/space-" + tone + ".png")
            .OfType<Sprite>().First(s => s.name == tone + "_" + state);
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var root = GameObject.Find("PixelUI/Reference960x540").transform;
        var node = root.Find("ParryCuePresenter");
        var host = node ? node : Rect("ParryCuePresenter", root);
        var presenter = host.GetComponent<ParryCuePresenter>();
        if (!presenter) presenter = host.gameObject.AddComponent<ParryCuePresenter>();
        if (!node) Box((RectTransform)presenter.transform, root, 0, 0, 960, 540);
        Sprite Tile(int id) => AssetDatabase.LoadAssetAtPath<Sprite>(Art + "tile_" + id.ToString("D4") + ".png");
        RectTransform Key(Transform parent, string name, float x, float y)
        {
            var key = parent.Find(name) as RectTransform;
            if (!key) { key = Rect(name, parent); Box(key, parent, x, y, 96, 48); }
            foreach (var part in new[] { ("Left", 0f, 235), ("Right", 48f, 237), ("Locked", 24f, 15) })
            {
                var rt = key.Find(part.Item1) as RectTransform;
                bool created = !rt;
                if (created) { rt = Rect(part.Item1, key); Box(rt, key, part.Item2, 0, 48, 48); }
                var image = rt.GetComponent<Image>();
                if (!image) image = Image(rt, Gold);
                if (!image.sprite) image.sprite = Tile(part.Item3);
                if (created && part.Item1 == "Locked") rt.gameObject.SetActive(false);
            }
            var labelNode = key.Find("Label") as RectTransform;
            var label = labelNode ? labelNode.GetComponent<TextMeshProUGUI>() : null;
            if (!label)
            {
                if (labelNode) label = labelNode.gameObject.AddComponent<TextMeshProUGUI>();
                else label = Label("Label", "SPACE", key, 0, 0, 96, 48, 24);
                label.text = "SPACE"; label.fontSize = 24;
                label.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(CombatR7Build.Art + "VT323Pixel16.asset");
                label.fontSharedMaterial = label.font.material;
                label.alignment = TextAlignmentOptions.Center;
                label.enableWordWrapping = false; label.raycastTarget = false;
            }
            return key;
        }
        var live = Key(presenter.transform, "SpaceKey", 69, 399);
        void StyleKey(RectTransform key)
        {
            // Migrate the previous two-tile key once; keep later authored tuning.
            var face = key.Find("Left").GetComponent<Image>();
            var label = key.Find("Label").GetComponent<TMP_Text>();
            bool migrate = label.gameObject.activeSelf;
            if (migrate) key.sizeDelta = new Vector2(84, 33); // 28x11 logical; compact Point-filtered cap.
            var rect = face.rectTransform;
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            face.type = UnityEngine.UI.Image.Type.Sliced;
            face.pixelsPerUnitMultiplier = 1f / 3f;
            face.sprite = Cap("warning", "released"); face.color = Foreground;
            key.Find("Right").gameObject.SetActive(false);
            label.gameObject.SetActive(false); // SPACE is baked into the supplied cap frames.
            if (migrate) Box((RectTransform)key.Find("Locked"), key, 30, 4.5f, 24, 24);
        }
        StyleKey(live);
        // Only fill missing bindings: preserve authored tuning and existing node geometry.
        void Bind(string field, Object value)
        {
            var serialized = new SerializedObject(presenter);
            if (!serialized.FindProperty(field).objectReferenceValue) Set(presenter, field, value);
        }
        Bind("key", live); Bind("left", live.Find("Left").GetComponent<Image>());
        Bind("right", live.Find("Right").GetComponent<Image>()); Bind("locked", live.Find("Locked").GetComponent<Image>());
        var currentRelease = new SerializedObject(presenter).FindProperty("released").objectReferenceValue;
        if (currentRelease && AssetDatabase.GetAssetPath(currentRelease).EndsWith("space-dark.png"))
            Set(presenter, "released", Cap("warning", "released"));
        Bind("released", Cap("warning", "released")); Bind("pressed", Cap("dark", "pressed"));
        Bind("readyReleased", Cap("light", "released")); Bind("readyPressed", Cap("light", "pressed"));
        var authored = new SerializedObject(presenter);
        var landmarks = authored.FindProperty("headLandmarks");
        if (landmarks.arraySize == 0)
        {
            var heads = new[] {
                ("Assets/Characters/Enermy/AC_Skeleton.controller", new Vector2(-.03875f, 1.235f)),
                ("Assets/Art/EndlessSkeletons/sword/Combat.controller", new Vector2(-.10125f, 1.235f)),
                ("Assets/Art/EndlessSkeletons/axe/Combat.controller", new Vector2(-.10125f, 1.235f)),
                ("Assets/Art/EndlessSkeletons/sword-shield/Combat.controller", new Vector2(-.35125f, 1.36f)) };
            landmarks.arraySize = heads.Length;
            for (int i = 0; i < heads.Length; i++)
            {
                var item = landmarks.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("controller").objectReferenceValue = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(heads[i].Item1);
                item.FindPropertyRelative("local").vector2Value = heads[i].Item2;
            }
            authored.ApplyModifiedPropertiesWithoutUndo();
        }
        Bind("parry", Object.FindObjectOfType<ParrySystem>(true));
        Bind("slayer", Object.FindObjectOfType<PlayerSlayerMode>(true));
        live.gameObject.SetActive(false);
        var tutorial = Object.FindObjectOfType<TutorialOverlay>(true);
        var panel = tutorial.transform.Find("Reference960x540/TutorialPrompt");
        var sampleParent = panel.Find("SpaceExample");
        var sample = Key(sampleParent ? sampleParent : panel, "SpaceCueSample", 452, 270);
        StyleKey(sample);
        var example = panel.Find("SpaceExample") as RectTransform;
        if (!example)
        {
            example = Rect("SpaceExample", panel);
            Box(example, panel, 328, 216, 304, 80);
        }
        TMP_Text ExampleText(string name, string text, float y)
        {
            var existing = example.Find(name);
            if (existing) return existing.GetComponent<TMP_Text>();
            var label = Label(name, text, example, 0, y, 304, 24, 18);
            label.font = panel.Find("Shortcuts").GetComponent<TMP_Text>().font;
            label.fontSharedMaterial = label.font.material;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            return label;
        }
        ExampleText("Heading", "SPACE \u00b7 EXAMPLE", 0);
        var caption = ExampleText("Caption", "Get ready.", 56);
        if (sample.parent != example)
        {
            sample.SetParent(example, false);
            Box(sample, example, 110, 24, 84, 33);
        }
        var bindings = new SerializedObject(tutorial);
        void ExampleBind(string field, Object value)
        {
            var property = bindings.FindProperty(field);
            if (!property.objectReferenceValue) property.objectReferenceValue = value;
        }
        ExampleBind("exampleKey", sample);
        ExampleBind("exampleFace", sample.Find("Left").GetComponent<Image>());
        ExampleBind("exampleLocked", sample.Find("Locked").GetComponent<Image>());
        ExampleBind("exampleCaption", caption);
        ExampleBind("exampleWarning", Cap("warning", "released"));
        ExampleBind("exampleLight", Cap("light", "released"));
        ExampleBind("exampleDown", Cap("light", "pressed"));
        ExampleBind("exampleBlocked", Cap("dark", "pressed"));
        bindings.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
}
