using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

internal static class ShogunEnvironment
{
    internal const string Art = "Assets/Art/Environment/TinyPixelJapan/";
    internal static void Prepare()
    {
        foreach (var file in Directory.GetFiles(Art, "*.png"))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(file.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Sprite;
            importer.textureShape = TextureImporterShape.Texture2D;
            bool trees = Path.GetFileName(file) == "Trees.png";
            importer.spriteImportMode = trees ? SpriteImportMode.Multiple : SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.spritePixelsPerUnit = 100;
            var web = importer.GetPlatformTextureSettings("WebGL");
            web.overridden = true; web.format = TextureImporterFormat.RGBA32;
            web.maxTextureSize = 512; importer.SetPlatformTextureSettings(web);
            importer.SaveAndReimport();
            if (trees) SliceTrees(importer);
        }
    }

    static void SliceTrees(TextureImporter importer)
    {
        var factory = new SpriteDataProviderFactories(); factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider(); var previous = provider.GetSpriteRects();
        var slices = new[] {
            new SpriteRect { name = "Trees", rect = new Rect(0, 0, 353, 180) },
            new SpriteRect { name = "TreesLeft", rect = new Rect(0, 30, 140, 150) },
            new SpriteRect { name = "TreesRight", rect = new Rect(300, 30, 53, 150) }
        };
        foreach (var slice in slices)
        {
            slice.pivot = new Vector2(.5f, .5f); slice.alignment = SpriteAlignment.Center;
            slice.spriteID = previous.FirstOrDefault(s => s.name == slice.name)?.spriteID ?? GUID.Generate();
        }
        provider.SetSpriteRects(slices);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(
            slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply(); importer.SaveAndReimport();
    }

    static Sprite Load(string name)
    {
        string file = name.StartsWith("Trees") ? "Trees" : name;
        return AssetDatabase.LoadAllAssetsAtPath(Art + file + ".png").OfType<Sprite>().FirstOrDefault(s => s.name == name);
    }

    internal static void Apply()
    {
        var old = GameObject.Find("BackGround");
        if (old) foreach (var renderer in old.GetComponentsInChildren<Renderer>(true)) renderer.enabled = false;
        var grid = Object.FindObjectOfType<UnityEngine.Tilemaps.TilemapRenderer>();
        if (grid) grid.enabled = false; // Preserve Tilemap/CompositeCollider/Rigidbody.
        var root = GameObject.Find("ShogunEnvironment") ?? new GameObject("ShogunEnvironment");
        Layer(root, "Sky", "Sky", -100, "A6C8DB", 1);
        Layer(root, "Clouds", "Clouds", -95, "CCD5C3", .95f);
        Layer(root, "Fuji", "Fuji", -90, "638F91", .85f);
        Layer(root, "FarRidge", "Mountain_Back", -85, "628E8B", .8f);
        Layer(root, "MiddleRidge", "Mountain_Middle", -80, "496D69", .65f);
        Layer(root, "NearRidge", "Mountain_Front", -75, "354D4C", .5f);
        Layer(root, "Forest", "BackgroundTrees", -70, "346F61", .4f);
        Layer(root, "Shrine", "Shrine_Single", -65, "749A88", .3f);
        Layer(root, "House", "House", -60, "7C9F8F", .25f);
        Layer(root, "Ground", "Ground", -50, "719382", 0);
        Layer(root, "Grass", "Gras", 15, "2D4C44", 0);
        Layer(root, "ForegroundLeft", "TreesLeft", 20, "17352D", 0, -7.029f);
        Layer(root, "ForegroundRight", "TreesRight", 20, "17352D", 0, 9.9f);
        foreach (var name in new[] { "ForegroundLeft", "ForegroundRight" })
        {
            var tree = root.transform.Find(name); var position = tree.localPosition;
            position.y = 1.85f; tree.localPosition = position;
        }
        Layer(root, "GroundShade", "Sky", 21, "162B30", 0);
        var shade = root.transform.Find("GroundShade");
        shade.localPosition = new Vector3(.07f, -4.02f, 0); shade.localScale = new Vector3(6.6f, 1, 1);
    }

    internal static void Menu(Transform parent)
    {
        var backdrop = parent.Find("MenuEnvironment") as RectTransform;
        if (!backdrop) backdrop = PixelUIFactory.Rect("MenuEnvironment", parent);
        PixelUIFactory.Box(backdrop, parent, -50, 0, 1060, 540); backdrop.SetAsFirstSibling();
        var names = new[] { "Sky", "Clouds", "Fuji", "Mountain_Back", "Mountain_Middle", "Mountain_Front", "BackgroundTrees", "House", "Ground", "Trees" };
        var tints = new[] { "94B7CD", "BACABD", "547779", "537975", "44635E", "304744", "346055", "719080", "5E7A68", "234537" };
        for (int i = 0; i < names.Length; i++)
        {
            var rt = backdrop.Find(names[i]) as RectTransform;
            if (!rt) rt = PixelUIFactory.Rect(names[i], backdrop);
            PixelUIFactory.Stretch(rt, backdrop);
            var image = PixelUIFactory.Image(rt, PixelUIFactory.Hex(tints[i]));
            image.sprite = Load(names[i]);
        }
    }

    static void Layer(GameObject root, string name, string sprite, int order, string tint,
        float follow, float x = 0, bool flip = false)
    {
        var layer = root.transform.Find(name);
        if (!layer) { layer = new GameObject(name).transform; layer.SetParent(root.transform, false); }
        layer.localPosition = new Vector3(.07f + x, .86f, 0);
        layer.localScale = Vector3.one * 6.6f;
        var renderer = layer.GetComponent<SpriteRenderer>();
        if (!renderer) renderer = layer.gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Load(sprite);
        if (!renderer.sprite) throw new FileNotFoundException("Missing original Japan layer " + sprite);
        renderer.sortingOrder = order; renderer.color = PixelUIFactory.Hex(tint); renderer.flipX = flip;
        renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        var parallax = layer.GetComponent<ArenaParallax>();
        if (!parallax) parallax = layer.gameObject.AddComponent<ArenaParallax>();
        PixelUIFactory.Set(parallax, "target", Camera.main); PixelUIFactory.Value(parallax, "follow", follow);
    }
}
