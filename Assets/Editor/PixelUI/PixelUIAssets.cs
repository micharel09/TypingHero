using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using static PixelUIFactory;

internal static class PixelUIAssets
{
    internal static void Prepare()
    {
        Directory.CreateDirectory(Generated);
        AssetDatabase.Refresh();
        foreach (var path in Directory.GetFiles(Art, "*.png", SearchOption.AllDirectories))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 100;
            if (Path.GetFileName(path) == "frame.png") importer.spriteBorder = new Vector4(6, 6, 6, 6);
            importer.SaveAndReimport();
        }
        Frame = AssetDatabase.LoadAssetAtPath<Sprite>(Art + "frame.png");
        if (!Frame) throw new FileNotFoundException("Missing Crusenho Flat frame");
        Typeface = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Generated + "VT323.asset");
        if (!Typeface)
        {
            var source = AssetDatabase.LoadAssetAtPath<UnityEngine.Font>(Art + "VT323-Regular.ttf");
            Typeface = TMP_FontAsset.CreateFontAsset(source, 48, 6, GlyphRenderMode.SDFAA,
                1024, 1024, AtlasPopulationMode.Dynamic, true);
            Typeface.name = "VT323";
            AssetDatabase.CreateAsset(Typeface, Generated + "VT323.asset");
            AssetDatabase.AddObjectToAsset(Typeface.material, Typeface);
            foreach (var atlas in Typeface.atlasTextures) AssetDatabase.AddObjectToAsset(atlas, Typeface);
            var chars = string.Concat(Enumerable.Range(32, 95).Select(c => (char)c)) +
                "ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚÝàáâãèéêìíòóôõùúýĂăĐđĨĩŨũƠơƯư" +
                string.Concat(Enumerable.Range(0x1EA0, 0x1EFA - 0x1EA0).Select(c => (char)c)) + "…×≥≤";
            if (!Typeface.TryAddCharacters(chars, out string missing))
                throw new System.InvalidOperationException("VT323 atlas missing required glyphs: " + missing);
            EditorUtility.SetDirty(Typeface);
        }
        AssetDatabase.SaveAssets();
    }

    internal static void SavePrimitives()
    {
        BuildingTemplates = true;
        try
        {
            var panel = PanelAt("PixelPanel", null, 0, 0, 320, 240);
            PrefabUtility.SaveAsPrefabAsset(panel.gameObject, Generated + "Panel.prefab");
            Object.DestroyImmediate(panel.gameObject);
            var button = Button("Play", null, 0, 0, 240, null);
            PrefabUtility.SaveAsPrefabAsset(button.gameObject, Generated + "Button.prefab");
            Object.DestroyImmediate(button.gameObject);
            var bar = Bar("StatBar", null, 0, 0, 240, Red);
            PrefabUtility.SaveAsPrefabAsset(bar.gameObject, Generated + "StatBar.prefab");
            Object.DestroyImmediate(bar.gameObject);
        }
        finally { BuildingTemplates = false; }
    }
}
