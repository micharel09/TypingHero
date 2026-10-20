using System.IO;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

// Final UI typography pass. TypingManager owns the three excluded word labels.
internal static class BlackletterTitleBuild
{
    internal const string Art = "Assets/Art/UI/Branding/";
    const string FontPath = Art + "ClackbladeSpecimenPixel64.asset";
    static readonly string Characters = " " +
        string.Concat(Enumerable.Range('A', 26).Select(c => (char)c)) +
        string.Concat(Enumerable.Range('a', 26).Select(c => (char)c)) + "0123456789";

    internal static void Apply()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (!font)
        {
            var source = AssetDatabase.LoadAssetAtPath<Font>(Art + "ClackbladeSpecimenPixel.ttf");
            if (!source) throw new FileNotFoundException("Missing specimen-derived title font");
            font = TMP_FontAsset.CreateFontAsset(source, 64, 1, GlyphRenderMode.RASTER_HINTED,
                1024, 1024, AtlasPopulationMode.Dynamic, false);
            font.name = "ClackbladeSpecimenPixel64";
            AssetDatabase.CreateAsset(font, FontPath);
            if (!font.TryAddCharacters(Characters, out string missing))
                throw new System.InvalidOperationException("Title font missing glyphs: " + missing);
            font.atlasPopulationMode = AtlasPopulationMode.Static;
            AssetDatabase.AddObjectToAsset(font.material, font);
            foreach (var atlas in font.atlasTextures)
            {
                atlas.filterMode = FilterMode.Point;
                atlas.wrapMode = TextureWrapMode.Clamp;
                AssetDatabase.AddObjectToAsset(atlas, font);
            }
            EditorUtility.SetDirty(font);
        }
        var fallback = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PixelUIFactory.Generated + "VT323.asset");
        if (!fallback) throw new FileNotFoundException("Missing VT323 fallback font");
        if (font.fallbackFontAssetTable == null || font.fallbackFontAssetTable.Count != 1 ||
            font.fallbackFontAssetTable[0] != fallback)
        {
            font.fallbackFontAssetTable = new List<TMP_FontAsset> { fallback };
            EditorUtility.SetDirty(font);
        }
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset.GetComponentsInChildren<TMP_Text>(true).Length == 0) continue;
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                ApplyLabels(root, font);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        foreach (var path in new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity" })
        {
            var scene = EditorSceneManager.OpenScene(path);
            var roots = scene.GetRootGameObjects();
            var words = TypingLabels(roots);
            var original = words.Where(word => word).Select(word =>
                (label: word, font: word.font, material: word.fontSharedMaterial)).ToArray();
            foreach (var root in roots) ApplyLabels(root, font, words);
            foreach (var word in original)
                if (word.label.font != word.font || word.label.fontSharedMaterial != word.material)
                    throw new System.InvalidOperationException("Typing font/material changed: " + word.label.name);
            EditorSceneManager.SaveScene(scene);
        }
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        var title = GameObject.Find("PixelUI/Reference960x540/MenuPanel/Title")
            .GetComponent<TMP_Text>();
        title.text = "Clackblade";
        title.font = font;
        title.fontSharedMaterial = font.material;
        title.fontSize = 52;
        title.enableAutoSizing = false;
        title.enableWordWrapping = false;
        title.alignment = TextAlignmentOptions.Center;
        // Fit the existing title bounds uniformly; do not move other menu controls.
        float width = title.GetPreferredValues(title.text).x;
        if (width > title.rectTransform.rect.width)
            title.fontSize *= title.rectTransform.rect.width / width;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static HashSet<TMP_Text> TypingLabels(IEnumerable<GameObject> roots)
    {
        var words = new HashSet<TMP_Text>();
        foreach (var typing in roots.SelectMany(root => root.GetComponentsInChildren<TypingManager>(true)))
        {
            var serialized = new SerializedObject(typing);
            foreach (var field in new[] { "wordText", "nextWordText", "thirdWordText" })
                words.Add(serialized.FindProperty(field).objectReferenceValue as TMP_Text);
        }
        return words;
    }

    static void ApplyLabels(GameObject root, TMP_FontAsset font, HashSet<TMP_Text> words = null)
    {
        words = words ?? TypingLabels(new[] { root });
        foreach (var input in root.GetComponentsInChildren<TMP_InputField>(true))
        {
            if (words.Contains(input.textComponent)) continue;
            input.fontAsset = font;
            EditorUtility.SetDirty(input);
        }
        foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (words.Contains(label)) continue;
            float maximum = label.font == font && label.enableAutoSizing ? label.fontSizeMax : label.fontSize;
            label.font = font;
            label.fontSharedMaterial = font.material;
            // Preserve authored bounds, colors and wrapping. Fit changing names,
            // counts and translated punctuation inside those same bounds.
            label.enableAutoSizing = true;
            label.fontSizeMax = maximum;
            label.fontSizeMin = Mathf.Min(maximum, 12);
            if (PrefabUtility.IsPartOfPrefabInstance(label))
                PrefabUtility.RecordPrefabInstancePropertyModifications(label);
            EditorUtility.SetDirty(label);
        }
    }
}
