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
        string.Concat(Enumerable.Range('a', 26).Select(c => (char)c)) + "0123456789~!@#$%^&*()_+<>?";

    internal static void Apply()
    {
        var source = AssetDatabase.LoadAssetAtPath<Font>(Art + "ClackbladeSpecimenPixel.ttf");
        if (!source) throw new FileNotFoundException("Missing specimen-derived font");
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (!font)
        {
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
        if (!font.characterTable.Any(character => character.unicode == '@'))
        {
            // Repack the corrected outlines in place; preserve font/material GUIDs.
            font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            font.ClearFontAssetData();
            FontEngine.LoadFontFace(source, 64);
            font.faceInfo = FontEngine.GetFaceInfo();
            if (!font.TryAddCharacters(Characters, out string missing))
                throw new System.InvalidOperationException("Specimen font missing glyphs: " + missing);
            font.atlasPopulationMode = AtlasPopulationMode.Static;
            EditorUtility.SetDirty(font);
            foreach (var atlas in font.atlasTextures) EditorUtility.SetDirty(atlas);
        }
        const string uiPath = Art + "ClackbladeSpecimenUI100.asset";
        var uiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(uiPath);
        if (!uiFont)
        {
            // The specimen's native pixel grid is 100px/em. SDF keeps those same
            // outlines coherent at the smaller UI sizes instead of point-sampling
            // a title bitmap and losing thin strokes.
            uiFont = TMP_FontAsset.CreateFontAsset(source, 100, 8, GlyphRenderMode.SDFAA,
                1024, 1024, AtlasPopulationMode.Dynamic, false);
            uiFont.name = "ClackbladeSpecimenUI100";
            AssetDatabase.CreateAsset(uiFont, uiPath);
            if (!uiFont.TryAddCharacters(Characters, out string missing))
                throw new System.InvalidOperationException("UI font missing glyphs: " + missing);
            uiFont.atlasPopulationMode = AtlasPopulationMode.Static;
            AssetDatabase.AddObjectToAsset(uiFont.material, uiFont);
            foreach (var atlas in uiFont.atlasTextures)
            {
                atlas.filterMode = FilterMode.Bilinear;
                atlas.wrapMode = TextureWrapMode.Clamp;
                AssetDatabase.AddObjectToAsset(atlas, uiFont);
            }
            EditorUtility.SetDirty(uiFont);
        }
        var fallback = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PixelUIFactory.Generated + "VT323.asset");
        if (!fallback) throw new FileNotFoundException("Missing VT323 fallback font");
        foreach (var face in new[] { font, uiFont })
        if (face.fallbackFontAssetTable == null || face.fallbackFontAssetTable.Count != 1 ||
            face.fallbackFontAssetTable[0] != fallback)
        {
            face.fallbackFontAssetTable = new List<TMP_FontAsset> { fallback };
            EditorUtility.SetDirty(face);
        }
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset.GetComponentsInChildren<TMP_Text>(true).Length == 0) continue;
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                ApplyLabels(root, uiFont);
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
            foreach (var root in roots) ApplyLabels(root, uiFont, words);
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
