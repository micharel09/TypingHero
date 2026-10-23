using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SpriteLightingAuthoringChecks
{
    public static void Verify()
    {
        var pipeline = AssetDatabase.LoadAssetAtPath<WorldLightingProfile>("Assets/Data/Lighting/DungeonLighting.asset").pipeline;
        if (new SerializedObject(pipeline).FindProperty("m_AdditionalLightsPerObjectLimit").intValue < 6)
            throw new InvalidOperationException("Authored world lights plus combat do not fit per-object lighting");
        string folder = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Fixtures/Lighting/NormalAuthoringTemporary");
        AssetDatabase.CreateFolder(Path.GetDirectoryName(folder).Replace('\\', '/'), Path.GetFileName(folder));
        var texture = new Texture2D(2, 2);
        try
        {
            texture.SetPixels(new[] { Color.red, Color.green, Color.blue, Color.white }); texture.Apply();
            string source = folder + "/Source.png", painted = folder + "/Painted.png";
            File.WriteAllBytes(source, texture.EncodeToPNG()); File.WriteAllBytes(painted, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(source); AssetDatabase.ImportAsset(painted);
            var importer = (TextureImporter)AssetImporter.GetAtPath(source);
            importer.textureType = TextureImporterType.Sprite;
            importer.secondarySpriteTextures = new[] { new SecondarySpriteTexture {
                name = "_NormalMap", texture = AssetDatabase.LoadAssetAtPath<Texture2D>(painted) } };
            importer.SaveAndReimport();
            byte[] before = File.ReadAllBytes(painted), metadata = File.ReadAllBytes(source + ".meta");
            var method = typeof(WorldLightingAuthoring).Assembly.GetType("SurfaceNormalAssets")
                .GetMethod("Prepare", BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[] { source, false });
            method.Invoke(null, new object[] { source, false });
            if (!before.SequenceEqual(File.ReadAllBytes(painted)) || !metadata.SequenceEqual(File.ReadAllBytes(source + ".meta")))
                throw new InvalidOperationException("Routine preparation overwrote a painted normal/custom binding");
        }
        finally { UnityEngine.Object.DestroyImmediate(texture); AssetDatabase.DeleteAsset(folder); }
        Debug.Log("PASS: repeated surface preparation preserves painted normal bytes and custom secondary binding");
        var maps = Directory.GetFiles("Assets/Art/Lighting/Normals", "*.png").ToDictionary(x => x, File.ReadAllBytes);
        LightingAuthoringChecks.VerifyPreviewAndRepeatBuilder();
        foreach (var pair in maps)
            if (!pair.Value.SequenceEqual(File.ReadAllBytes(pair.Key)))
                throw new InvalidOperationException("Repeat builder overwrote authored surface: " + pair.Key);
        Debug.Log("PASS: authored normals survive strict two-world repeated builder");
    }
}
