using System.IO;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

// Blackbit brand title; Cobblestone UI and typing labels. Input/color behavior stays owned by TypingManager.
internal static class BlackletterTitleBuild
{
    internal const string Art = "Assets/Art/UI/Branding/";
    const string FontPath = Art + "ClackbladeSpecimenPixel64.asset";
    static readonly string Characters = " " +
        string.Concat(Enumerable.Range('A', 26).Select(c => (char)c)) +
        string.Concat(Enumerable.Range('a', 26).Select(c => (char)c)) + "0123456789~!@#$%^&*()_+<>?";
    [System.Serializable] class FallbackRecipe
    {
        public string source, characters;
        public float letter_scale, slash_scale;
    }
    [System.Serializable] class Metrics
    {
        public int ascent, descent, cap_height, x_height, line_gap;
    }
    [System.Serializable] class FontRecipe
    {
        public FallbackRecipe fallback;
        public Metrics metrics;
        public int units_per_em;
        public string font_sha256;
        public GlyphAnchor[] qa_glyphs;
    }
    [System.Serializable] class GlyphAnchor { public string character; public float[] pixel_snap_anchor; }
    [System.Serializable] class UIRecipe { public string source, font_sha256, characters; }
    const string UIPath = Art + "CobblestoneUI64.asset";
    static Dictionary<string, byte[]> protectedTypingAssets;

    internal static Vector2 ReadPixelAnchor()
    {
        var recipe = JsonUtility.FromJson<FontRecipe>(File.ReadAllText(Art + "glyphs.json"));
        var anchor = recipe.qa_glyphs.Single(g => g.character == "+").pixel_snap_anchor;
        if (anchor == null || anchor.Length != 2) throw new System.InvalidOperationException("Missing plus pixel anchor");
        return new Vector2(anchor[0], anchor[1]);
    }

    internal static void ApplyPixelSnap(TMP_Text label, Vector2 anchor)
    {
        var snap = label.GetComponent<BlackletterPixelSnap>();
        if (!snap) snap = label.gameObject.AddComponent<BlackletterPixelSnap>();
        var serialized = new SerializedObject(snap);
        serialized.FindProperty("plusAnchor").vector2Value = anchor;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        PrefabUtility.RecordPrefabInstancePropertyModifications(snap);
    }

    internal static TMP_FontAsset PrepareFallback()
    {
        var recipe = JsonUtility.FromJson<FontRecipe>(File.ReadAllText(Art + "glyphs.json")).fallback;
        var source = AssetDatabase.LoadAssetAtPath<Font>(recipe.source);
        var font = Rebuild(source, Art + "ClackbladeVT323Fallback.asset", 48,
            GlyphRenderMode.SDFAA, 6, FilterMode.Bilinear, recipe.characters, true);
        bool changed = false;
        foreach (var character in font.characterTable)
        {
            // Match the letter body (VT323 .56/.40em, specimen .76/.54em).
            // Shorten the slash to the smaller digit body; retain other punctuation.
            float scale = char.IsLetter((char)character.unicode) ? recipe.letter_scale : character.unicode == '/' ? recipe.slash_scale : 1;
            if (character.scale == scale) continue;
            character.scale = scale;
            changed = true;
        }
        if (changed) { EditorUtility.SetDirty(font); AssetDatabase.SaveAssets(); }
        return font;
    }

    internal static Dictionary<ulong, string> SnapshotTypingComponents()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var labels = TypingLabels(scene.GetRootGameObjects()).Where(t => t).ToArray();
        var paths = labels.SelectMany(t => new Object[] {
            t.font, t.fontSharedMaterial, t.fontSharedMaterial.mainTexture, t.font.sourceFontFile
        }).Where(asset => asset).Select(AssetDatabase.GetAssetPath).Where(File.Exists).Distinct();
        protectedTypingAssets = paths.SelectMany(path => new[] { path, path + ".meta" })
            .Where(File.Exists).Distinct().ToDictionary(path => path, File.ReadAllBytes);
        return labels.ToDictionary(
            t => GlobalObjectId.GetGlobalObjectIdSlow(t).targetObjectId,
            t => TypingBehavior(SerializedTypingComponent(GlobalObjectId.GetGlobalObjectIdSlow(t).targetObjectId)));
    }

    static string SerializedTypingComponent(ulong id)
    {
        string yaml = File.ReadAllText("Assets/Scenes/SampleScene.unity").Replace("\r\n", "\n");
        var match = System.Text.RegularExpressions.Regex.Match(yaml,
            @"^--- !u!114 &" + id + @"\n.*?(?=^--- !u!|\z)",
            System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.Singleline);
        if (!match.Success) throw new System.InvalidOperationException("Missing serialized typing component " + id);
        return match.Value;
    }

    static string TypingBehavior(string yaml) => System.Text.RegularExpressions.Regex.Replace(
        yaml, @"^  m_(fontAsset|sharedMaterial): .*?$", "", System.Text.RegularExpressions.RegexOptions.Multiline);

    internal static void AssertTypingComponents(Dictionary<ulong, string> before)
    {
        if (before.Count != 3) throw new System.InvalidOperationException("Expected exactly three protected typing labels");
        foreach (var entry in before)
            if (TypingBehavior(SerializedTypingComponent(entry.Key)) != entry.Value)
                throw new System.InvalidOperationException("Typing behavior component changed: " + entry.Key);
        foreach (var asset in protectedTypingAssets)
            if (!File.ReadAllBytes(asset.Key).SequenceEqual(asset.Value))
                throw new System.InvalidOperationException("Protected typing font/material asset changed: " + asset.Key);
    }

    internal static void Apply()
    {
        var source = AssetDatabase.LoadAssetAtPath<Font>(Art + "ClackbladeSpecimenPixel.ttf");
        if (!source) throw new FileNotFoundException("Missing specimen-derived font");
        var font = Rebuild(source, FontPath, 100, GlyphRenderMode.SDF32, 8, FilterMode.Bilinear, hardEdges: true);
        const string uiPath = UIPath;
        var uiRecipe = JsonUtility.FromJson<UIRecipe>(File.ReadAllText(Art + "cobblestone.json"));
        var uiSource = AssetDatabase.LoadAssetAtPath<Font>(uiRecipe.source);
        using (var sha = System.Security.Cryptography.SHA256.Create())
            if (System.BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(uiRecipe.source))).Replace("-", "").ToLowerInvariant() != uiRecipe.font_sha256)
                throw new System.InvalidOperationException("Cobblestone source checksum differs");
        var uiFont = Rebuild(uiSource, uiPath, 64, GlyphRenderMode.SDF32, 8, FilterMode.Bilinear, uiRecipe.characters);
        var fallbackRecipe = JsonUtility.FromJson<FontRecipe>(File.ReadAllText(Art + "glyphs.json")).fallback;
        var uiFallback = Rebuild(AssetDatabase.LoadAssetAtPath<Font>(fallbackRecipe.source),
            Art + "CobblestoneVT323Fallback.asset", 48, GlyphRenderMode.SDF32, 8, FilterMode.Bilinear, fallbackRecipe.characters);
        foreach (var ch in uiFallback.characterTable) ch.scale = 1;
        uiFont.fallbackFontAssetTable = new List<TMP_FontAsset> { uiFallback };
        EditorUtility.SetDirty(uiFont);
        EditorUtility.SetDirty(uiFallback);
        var fallback = PrepareFallback();
        if (!fallback) throw new FileNotFoundException("Missing VT323 fallback font");
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        uiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(uiPath);
        var recipe = JsonUtility.FromJson<FontRecipe>(File.ReadAllText(Art + "glyphs.json"));
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            var hash = System.BitConverter.ToString(sha.ComputeHash(
                File.ReadAllBytes(AssetDatabase.GetAssetPath(source)))).Replace("-", "").ToLowerInvariant();
            if (hash != recipe.font_sha256)
                throw new System.InvalidOperationException("Font source and reconstruction manifest differ");
        }
        foreach (var face in new[] { font })
        {
            var info = face.faceInfo;
            // Unity 2022 serializes this field but does not expose unitsPerEM in FaceInfo.
            var units = new SerializedObject(face).FindProperty("m_FaceInfo.m_UnitsPerEM");
            float unit = info.pointSize / (float)recipe.units_per_em;
            var metrics = recipe.metrics;
            if (units == null || units.intValue != recipe.units_per_em ||
                Mathf.Abs(info.ascentLine - metrics.ascent * unit) > .01f ||
                Mathf.Abs(info.descentLine + metrics.descent * unit) > .01f ||
                Mathf.Abs(info.capLine - metrics.cap_height * unit) > .01f ||
                Mathf.Abs(info.meanLine - metrics.x_height * unit) > .01f ||
                Mathf.Abs(info.lineHeight - (metrics.ascent + metrics.descent + metrics.line_gap) * unit) > .01f)
                throw new System.InvalidOperationException("Unity Face Info differs from source metrics: " + face.name);
        }
        foreach (var face in new[] { font })
        if (face.fallbackFontAssetTable == null || face.fallbackFontAssetTable.Count != 1 ||
            face.fallbackFontAssetTable[0] != fallback)
        {
            face.fallbackFontAssetTable = new List<TMP_FontAsset> { fallback };
            EditorUtility.SetDirty(face);
        }
        AssetDatabase.SaveAssets();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        uiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(uiPath);
        if (!font || !uiFont) throw new System.InvalidOperationException("Font references lost during import");
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset.GetComponentsInChildren<TMP_Text>(true).Length == 0) continue;
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                ApplyLabels(root, uiFont);
                BlackletterUILayout.Prefab(root);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        foreach (var path in new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity" })
        {
            var scene = EditorSceneManager.OpenScene(path);
            uiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(uiPath);
            var roots = scene.GetRootGameObjects();
            var words = TypingLabels(roots);
            foreach (var root in roots) ApplyLabels(root, uiFont, words);
            BlackletterUILayout.Scene(roots);
            EditorSceneManager.SaveScene(scene);
        }
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        var title = GameObject.Find("PixelUI/Reference960x540/Logo")
            .GetComponent<TMP_Text>();
        title.text = "Clackblade";
        title.font = font;
        title.fontSharedMaterial = font.material;
        title.fontSize = title.fontSizeMin = title.fontSizeMax = 56;
        title.enableAutoSizing = false;
        title.enableWordWrapping = false;
        title.alignment = TextAlignmentOptions.MidlineLeft;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        foreach (var path in new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity" })
        {
            var reopened = EditorSceneManager.OpenScene(path);
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
            uiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(uiPath);
            var roots = reopened.GetRootGameObjects();
            BlackletterPixelMaterial.Assert(font);
            foreach (var face in new[] { uiFont, uiFallback })
                if (face.material.shader.name != "TextMeshPro/Mobile/Distance Field" || face.material.mainTexture != face.atlasTexture)
                    throw new System.InvalidOperationException("Unexpected Cobblestone material: " + face.name);
            foreach (var label in roots.SelectMany(r => r.GetComponentsInChildren<TMP_Text>(true)))
            {
                var expected = label == title ? font : uiFont;
                // The title reference belongs to the previous scene instance.
                if (path.EndsWith("MainMenu.unity") && label.name == "Logo" &&
                    label.transform.parent.name == "Reference960x540") expected = font;
                if (label.font != expected || label.fontSharedMaterial != expected.material ||
                    expected.material.mainTexture != expected.atlasTexture)
                    throw new System.InvalidOperationException("Unexpected saved UI font/material: " + path + "/" + label.name);
                if (Characters.Any(c => !expected.characterLookupTable.ContainsKey(c)))
                    throw new System.InvalidOperationException("Primary Latin/digit glyphs missing");
            }
        }
    }

    // Hash the source, character set and complete render contract. Keep every
    // existing asset/subasset identity when regenerating the atlas.
    internal static TMP_FontAsset Rebuild(Font source, string path, int size, GlyphRenderMode mode, int padding, FilterMode filter = FilterMode.Point, string characters = null, bool hardEdges = false)
    {
        characters = characters ?? Characters;
        string sourceHash;
        using (var sha = System.Security.Cryptography.SHA256.Create())
            sourceHash = System.BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(AssetDatabase.GetAssetPath(source)))).Replace("-", "");
        string fingerprint = $"Blackbit-v2:{sourceHash}:{characters}:{size}:{mode}:{padding}:1024:{filter}:Clamp:no-mips";
        var current = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        var importer = current ? AssetImporter.GetAtPath(path) : null;
        if (current && importer.userData.Split('\n')[0] == fingerprint &&
            current.faceInfo.pointSize == size && current.atlasPadding == padding &&
            current.atlasRenderMode == mode && current.material.mainTexture == current.atlasTexture &&
            current.atlasTextures.All(t => t && t.filterMode == filter &&
                t.wrapMode == TextureWrapMode.Clamp && t.mipmapCount == 1))
            return hardEdges ? BlackletterPixelMaterial.Apply(current) : current;
        var fresh = TMP_FontAsset.CreateFontAsset(source, size, padding, mode,
            1024, 1024, AtlasPopulationMode.Dynamic, false);
        if (!fresh || !fresh.TryAddCharacters(characters, out string missing, true))
            throw new System.InvalidOperationException("Unable to generate all specimen characters");
        fresh.atlasPopulationMode = AtlasPopulationMode.Static;
        fresh.name = Path.GetFileNameWithoutExtension(path);
        var generatedMaterial = fresh.material;
        var generatedTextures = fresh.atlasTextures.ToArray();
        if (current)
        {
            var material = current.material;
            // CopySerialized may overwrite an existing managed array in place.
            // Snapshot its entries so they continue to identify the old subassets.
            var textures = current.atlasTextures.ToArray();
            if (textures.Length != generatedTextures.Length)
                throw new System.InvalidOperationException("Unexpected atlas page count; preserve existing subasset references");
            for (int i = 0; i < textures.Length; i++)
            {
                EditorUtility.CopySerialized(generatedTextures[i], textures[i]);
                textures[i].filterMode = filter;
                textures[i].wrapMode = TextureWrapMode.Clamp;
                EditorUtility.SetDirty(textures[i]);
            }
            EditorUtility.CopySerialized(generatedMaterial, material);
            material.mainTexture = textures[0];
            EditorUtility.CopySerialized(fresh, current);
            current.material = material;
            current.atlasTextures = textures;
            EditorUtility.SetDirty(material);
            Object.DestroyImmediate(fresh);
            Object.DestroyImmediate(generatedMaterial);
            foreach (var texture in generatedTextures) Object.DestroyImmediate(texture);
        }
        else
        {
            current = fresh;
            AssetDatabase.CreateAsset(current, path);
            AssetDatabase.AddObjectToAsset(current.material, current);
            foreach (var atlas in current.atlasTextures)
            {
                atlas.filterMode = filter;
                atlas.wrapMode = TextureWrapMode.Clamp;
                AssetDatabase.AddObjectToAsset(atlas, current);
            }
        }
        current.ReadFontAssetDefinition();
        if (current.characterTable.Count != characters.Length || current.atlasTextures.Any(t => t.mipmapCount != 1))
            throw new System.InvalidOperationException("Invalid specimen atlas coverage/mipmaps");
        EditorUtility.SetDirty(current);
        AssetDatabase.SaveAssets();
        importer = AssetImporter.GetAtPath(path);
        importer.userData = fingerprint;
        AssetDatabase.WriteImportSettingsIfDirty(path);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        current = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        return hardEdges ? BlackletterPixelMaterial.Apply(current) : current;
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
        if (!font) throw new System.InvalidOperationException("Missing UI font; never substitute the TMP default");
        words = words ?? TypingLabels(new[] { root });
        foreach (var input in root.GetComponentsInChildren<TMP_InputField>(true))
        {
            input.fontAsset = font;
            EditorUtility.SetDirty(input);
        }
        foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            bool typingWord = words.Contains(label);
            float maximum = label.enableAutoSizing ? label.fontSizeMax : label.fontSize;
            label.font = font;
            label.fontSharedMaterial = font.material;
            if (label.GetComponent<BlackletterPixelSnap>() is BlackletterPixelSnap oldSnap)
                Object.DestroyImmediate(oldSnap);
            if (typingWord)
            {
                EditorUtility.SetDirty(label);
                if (PrefabUtility.IsPartOfPrefabInstance(label)) PrefabUtility.RecordPrefabInstancePropertyModifications(label);
                continue; // Preserve size, palette, wrapping and rich-text behavior.
            }
            label.enableKerning = true;
            label.fontStyle &= ~(FontStyles.Bold | FontStyles.Italic);
            // TMP Ellipsis can remove the whole first line when the font's
            // line box is slightly taller than the control, even if its actual
            // glyphs fit. Size/verify visible glyph bounds instead.
            label.overflowMode = TextOverflowModes.Overflow;
            // Default body em:48reference units,16CSS pixels at320x180.
            // BlackletterUILayout assigns explicit sizes/bounds for compact roles.
            label.enableAutoSizing = false;
            // World-space TMP uses different units (notably the real parry cue).
            float size = label is TextMeshProUGUI ? Mathf.Max(maximum, BlackletterUILayout.BodySize) : maximum;
            label.fontSize = label.fontSizeMax = label.fontSizeMin = size;
            if (PrefabUtility.IsPartOfPrefabInstance(label))
                PrefabUtility.RecordPrefabInstancePropertyModifications(label);
            EditorUtility.SetDirty(label);
        }
    }
}
