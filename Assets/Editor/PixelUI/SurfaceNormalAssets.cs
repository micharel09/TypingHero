using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Alpha/material-region bevels for the art prototype, independent of painted luminance.
// Secondary UVs preserve the source frame/rect/pivot and Unity's flipped tangent basis.
internal static class SurfaceNormalAssets
{
    [MenuItem("CLACKBLADE/Lighting/Replace Selected Normal With Generated Draft")]
    static void ReplaceSelectedDraft()
    {
        foreach (var texture in Selection.GetFiltered<Texture2D>(SelectionMode.Assets))
        {
            string path = AssetDatabase.GetAssetPath(texture);
            if (path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) &&
                AssetImporter.GetAtPath(path) is TextureImporter importer && importer.textureType == TextureImporterType.Sprite)
                Prepare(path, true);
        }
    }

    internal static void Prepare(string sourcePath, bool replaceDraft = false)
    {
        var sourceImporter = (TextureImporter)AssetImporter.GetAtPath(sourcePath);
        // Secondary maps are authored data. Routine builders must not regenerate
        // a painted map, or replace a custom binding with the conventional path.
        if (!replaceDraft && sourceImporter.secondarySpriteTextures.Any(t => t.name == "_NormalMap" && t.texture)) return;
        string guid = AssetDatabase.AssetPathToGUID(sourcePath);
        string output = WorldLightingBuild.Root + "Normals/" + guid + ".png";
        bool actor = sourcePath.Contains("male_hero") || sourcePath.ToLowerInvariant().Contains("skeleton");
        if (!File.Exists(output) || replaceDraft)
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
                    if (actor)
                    {
                        // Fit the local material cross-section, rather than deriving
                        // height from painted brightness or beveling only two edge pixels.
                        int Distance(int dx,int dy)
                        {
                            int d=1;while(d<12 && Region(x+dx*d,y+dy*d)==region)d++;return d;
                        }
                        int left=Distance(-1,0),right=Distance(1,0),down=Distance(0,-1),up=Distance(0,1);
                        nx=(left-right)/(float)(left+right);
                        ny=(down-up)/(float)(down+up);
                        float verticalWeight=Mathf.Clamp((left+right)/(float)(down+up),.25f,1);
                        ny*=verticalWeight;
                    }
                    else
                    {
                        for (int d = 1; d <= 2; d++)
                        {
                            nx += ((Region(x-d,y) == region ? 1 : 0) - (Region(x+d,y) == region ? 1 : 0)) / (float)d;
                            ny += ((Region(x,y-d) == region ? 1 : 0) - (Region(x,y+d) == region ? 1 : 0)) / (float)d;
                        }
                    }
                    var n = new Vector3(nx * (actor ? 1.1f : .36f), ny * (actor ? 1.1f : .36f), 1).normalized;
                    normals[y * source.width+x] = new Color(n.x*.5f+.5f,n.y*.5f+.5f,n.z*.5f+.5f,1);
                }
            }
            var texture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false, true);
            texture.SetPixels32(normals); texture.Apply();
            var encoded=texture.EncodeToPNG();
            int requiredSize=Mathf.NextPowerOfTwo(Mathf.Max(texture.width,texture.height));
            bool changed=!File.Exists(output)||!File.ReadAllBytes(output).SequenceEqual(encoded);
            if(changed)File.WriteAllBytes(output,encoded);
            Object.DestroyImmediate(source); Object.DestroyImmediate(texture);
            if(changed)AssetDatabase.ImportAsset(output);
            var normalImporter = (TextureImporter)AssetImporter.GetAtPath(output);
            normalImporter.textureType = TextureImporterType.NormalMap;
            normalImporter.filterMode = FilterMode.Point; normalImporter.mipmapEnabled = false;
            normalImporter.textureCompression = TextureImporterCompression.Uncompressed;
            normalImporter.npotScale = TextureImporterNPOTScale.None;
            normalImporter.maxTextureSize=Mathf.Max(normalImporter.maxTextureSize,requiredSize);
            normalImporter.SaveAndReimport();
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

    // Palette-based material draft. This never changes albedo, alpha or sprite metadata.
    internal static void PrepareMaterials(string sourcePath)
    {
        if (!sourcePath.Contains("male_hero") && !sourcePath.ToLowerInvariant().Contains("skeleton")) return;
        string output = WorldLightingBuild.Root + "Normals/" + AssetDatabase.AssetPathToGUID(sourcePath) + "-surface.png";
        {
            var source = new Texture2D(2,2,TextureFormat.RGBA32,false);
            source.LoadImage(File.ReadAllBytes(sourcePath));
            var pixels = source.GetPixels32(); var masks = new Color32[pixels.Length];
            bool bone = sourcePath.ToLowerInvariant().Contains("skeleton");
            var weapon = new bool[pixels.Length];
            if (bone)
            {
                // The source skeleton palette reserves neutral204 for steel.
                // Grow only through adjacent neutral highlights; pale blue bone
                // and brown grips stop the fill, including across atlas frames.
                var pending=new System.Collections.Generic.Queue<int>();
                for(int i=0;i<pixels.Length;i++)
                    if(pixels[i].a>=128 && pixels[i].r==204 && pixels[i].g==204 && pixels[i].b==204)
                    {weapon[i]=true;pending.Enqueue(i);}
                while(pending.Count>0)
                {
                    int i=pending.Dequeue(),x=i%source.width,y=i/source.width;
                    foreach(int next in new[]{x>0?i-1:-1,x+1<source.width?i+1:-1,y>0?i-source.width:-1,y+1<source.height?i+source.width:-1})
                    {
                        if(next<0||weapon[next])continue;var c=pixels[next];
                        if(c.a<128||c.r!=c.g||c.g!=c.b||c.r<200)continue;
                        weapon[next]=true;pending.Enqueue(next);
                    }
                }
            }
            for(int i=0;i<pixels.Length;i++)
            {
                var c=pixels[i];
                float maximum=Mathf.Max(c.r,Mathf.Max(c.g,c.b)), minimum=Mathf.Min(c.r,Mathf.Min(c.g,c.b));
                bool neutral=maximum-minimum<35;
                // Pale skeleton regions remain rough bone, rather than becoming metal.
                bool metal=bone?weapon[i]:neutral && maximum>125 && maximum<225 && c.r<=c.b;
                bool skin=c.r>c.g*1.08f && c.g>c.b*1.12f;
                masks[i]=new Color(metal?.7f:0,metal?.28f:skin?.65f:.9f,0,1);
            }
            var map=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false,true);
            map.SetPixels32(masks);map.Apply();var bytes=map.EncodeToPNG();
            int requiredSize=Mathf.NextPowerOfTwo(Mathf.Max(map.width,map.height));
            bool changed=!File.Exists(output)||!File.ReadAllBytes(output).SequenceEqual(bytes);
            if(changed)File.WriteAllBytes(output,bytes);
            Object.DestroyImmediate(source);Object.DestroyImmediate(map);if(changed)AssetDatabase.ImportAsset(output);
            var settings=(TextureImporter)AssetImporter.GetAtPath(output);
            settings.textureType=TextureImporterType.Default;settings.sRGBTexture=false;
            settings.filterMode=FilterMode.Point;settings.mipmapEnabled=false;settings.wrapMode=TextureWrapMode.Clamp;
            settings.textureCompression=TextureImporterCompression.Uncompressed;settings.npotScale=TextureImporterNPOTScale.None;
            settings.maxTextureSize=Mathf.Max(settings.maxTextureSize,requiredSize);
            settings.SaveAndReimport();
        }
        var importer=(TextureImporter)AssetImporter.GetAtPath(sourcePath);
        var textures=importer.secondarySpriteTextures.ToList();var mapTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(output);
        int index=textures.FindIndex(t=>t.name=="_MaterialMap");
        if(index>=0 && textures[index].texture==mapTexture)return;
        var secondary=new SecondarySpriteTexture{name="_MaterialMap",texture=mapTexture};
        if(index<0)textures.Add(secondary);else textures[index]=secondary;
        importer.secondarySpriteTextures=textures.ToArray();importer.SaveAndReimport();
    }
}
