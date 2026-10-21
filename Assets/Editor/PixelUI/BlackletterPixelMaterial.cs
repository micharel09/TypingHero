using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;

// Material-only recipe: changing edge coverage must not regenerate glyph geometry.
internal static class BlackletterPixelMaterial
{
    internal const string ShaderPath = BlackletterTitleBuild.Art + "ClackbladePixelSDF.shader";
    internal const string ShaderName = "CLACKBLADE/Text/Pixel SDF";
    static readonly string[] Zero = {
        "_FaceDilate", "_WeightNormal", "_WeightBold", "_OutlineWidth", "_OutlineSoftness",
        "_UnderlayDilate", "_UnderlaySoftness", "_MaskSoftnessX", "_MaskSoftnessY"
    };
    static readonly string[] Effects = { "OUTLINE_ON", "UNDERLAY_ON", "UNDERLAY_INNER" };

    internal static TMP_FontAsset Apply(TMP_FontAsset font)
    {
        var shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        if (!shader) throw new FileNotFoundException("Missing Blackbit pixel shader", ShaderPath);
        var material = font.material;
        bool changed = material.shader != shader;
        if (changed) material.shader = shader;
        foreach (string property in Zero)
        {
            if (!material.HasProperty(property) || material.GetFloat(property) == 0) continue;
            material.SetFloat(property, 0);
            changed = true;
        }
        foreach (string effect in Effects)
        {
            if (!material.IsKeywordEnabled(effect)) continue;
            material.DisableKeyword(effect);
            changed = true;
        }
        if (changed) { EditorUtility.SetDirty(material); AssetDatabase.SaveAssets(); }
        string path = AssetDatabase.GetAssetPath(font);
        var importer = AssetImporter.GetAtPath(path);
        string hash;
        using (var sha = SHA256.Create())
            hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(ShaderPath))).Replace("-", "");
        string recipe = importer.userData.Split('\n')[0] + "\nBlackbitPixels-v1:" + hash + ":binary:hard-clip:zero-effects";
        if (importer.userData != recipe)
        {
            importer.userData = recipe;
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
        Assert(font);
        return font;
    }

    internal static void Assert(TMP_FontAsset font)
    {
        var material = font.material;
        if (material.shader.name != ShaderName || material.mainTexture != font.atlasTexture ||
            Zero.Any(p => material.HasProperty(p) && material.GetFloat(p) != 0) ||
            Effects.Any(material.IsKeywordEnabled))
            throw new InvalidOperationException("Blackbit pixel material recipe differs: " + font.name);
    }
}
