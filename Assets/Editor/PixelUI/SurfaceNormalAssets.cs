using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Alpha/material-region bevels for the art prototype, independent of painted luminance.
// Secondary UVs preserve the source frame/rect/pivot and Unity's flipped tangent basis.
internal static class SurfaceNormalAssets
{
    internal static void Prepare(string sourcePath)
    {
        string guid = AssetDatabase.AssetPathToGUID(sourcePath);
        string output = WorldLightingBuild.Root + "Normals/" + guid + ".png";
        if (!File.Exists(output))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            source.LoadImage(File.ReadAllBytes(sourcePath));
            var pixels = source.GetPixels32(); var normals = new Color32[pixels.Length];
            for (int i = 0; i < normals.Length; i++) normals[i] = new Color32(128,128,255,255);
            var frames = AssetDatabase.LoadAllAssetsAtPath(sourcePath).OfType<Sprite>().ToArray();
            foreach (var frame in frames)
            {
                var rect = frame.rect;
                int x0 = (int)rect.x, y0 = (int)rect.y, x1 = (int)rect.xMax, y1 = (int)rect.yMax;
                int Region(int x, int y)
                {
                    if (x < x0 || y < y0 || x >= x1 || y >= y1) return 0;
                    var c = pixels[y * source.width + x]; if (c.a < 128) return 0;
                    // Distinguish warm skin/wood, colored cloth/leaves, neutral metal/bone.
                    int max = Mathf.Max(c.r, Mathf.Max(c.g,c.b)), min = Mathf.Min(c.r, Mathf.Min(c.g,c.b));
                    if (max - min < 35) return 1;
                    if (c.r > c.b * 1.15f && c.r > c.g * 1.05f) return 2;
                    return 3;
                }
                for (int y = y0; y < y1; y++) for (int x = x0; x < x1; x++)
                {
                    int region = Region(x,y); if (region == 0) continue;
                    float nx = 0, ny = 0;
                    for (int d = 1; d <= 2; d++)
                    {
                        nx += ((Region(x-d,y) == region ? 1 : 0) - (Region(x+d,y) == region ? 1 : 0)) / (float)d;
                        ny += ((Region(x,y-d) == region ? 1 : 0) - (Region(x,y+d) == region ? 1 : 0)) / (float)d;
                    }
                    var n = new Vector3(nx * .36f, ny * .36f, 1).normalized;
                    normals[y * source.width+x] = new Color(n.x*.5f+.5f,n.y*.5f+.5f,n.z*.5f+.5f,1);
                }
            }
            var texture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false, true);
            texture.SetPixels32(normals); texture.Apply(); File.WriteAllBytes(output, texture.EncodeToPNG());
            Object.DestroyImmediate(source); Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(output);
            var normalImporter = (TextureImporter)AssetImporter.GetAtPath(output);
            normalImporter.textureType = TextureImporterType.NormalMap;
            normalImporter.filterMode = FilterMode.Point; normalImporter.mipmapEnabled = false;
            normalImporter.textureCompression = TextureImporterCompression.Uncompressed;
            normalImporter.npotScale = TextureImporterNPOTScale.None; normalImporter.SaveAndReimport();
        }
        var importer = (TextureImporter)AssetImporter.GetAtPath(sourcePath);
        var textures = importer.secondarySpriteTextures.ToList();
        var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(output);
        int index = textures.FindIndex(t => t.name == "_NormalMap");
        if (index >= 0 && textures[index].texture == normal) return;
        var secondary = new SecondarySpriteTexture { name = "_NormalMap", texture = normal };
        if (index < 0) textures.Add(secondary); else textures[index] = secondary;
        importer.secondarySpriteTextures = textures.ToArray(); importer.SaveAndReimport();
    }
}
