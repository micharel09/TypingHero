using System.IO;
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
            if (node.name == "SpaceCueSample" || node.GetComponent<ParryCuePresenter>()) return true;
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
        var live = Key(presenter.transform, "SpaceKey", 69, 459);
        // Only fill missing bindings: preserve authored tuning and existing node geometry.
        void Bind(string field, Object value)
        {
            var serialized = new SerializedObject(presenter);
            if (!serialized.FindProperty(field).objectReferenceValue) Set(presenter, field, value);
        }
        Bind("key", live); Bind("left", live.Find("Left").GetComponent<Image>());
        Bind("right", live.Find("Right").GetComponent<Image>()); Bind("locked", live.Find("Locked").GetComponent<Image>());
        Bind("label", live.Find("Label").GetComponent<TMP_Text>());
        Bind("outlineLeft", Tile(235)); Bind("outlineRight", Tile(237));
        Bind("filledLeft", Tile(507)); Bind("filledRight", Tile(509));
        Bind("parry", Object.FindObjectOfType<ParrySystem>(true));
        Bind("slayer", Object.FindObjectOfType<PlayerSlayerMode>(true));
        live.gameObject.SetActive(false);
        var tutorial = Object.FindObjectOfType<TutorialOverlay>(true);
        var panel = tutorial.transform.Find("Reference960x540/TutorialPrompt");
        var sample = Key(panel, "SpaceCueSample", 452, 270);
        sample.Find("Left").GetComponent<Image>().sprite = Tile(507);
        sample.Find("Right").GetComponent<Image>().sprite = Tile(509);
        sample.Find("Label").GetComponent<TMP_Text>().color = Hex("211c19");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
}
