using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

internal static class SlashVolumeAssets
{
    const string Root = "Assets/Art/VFX/SlashVolume/";
    [System.Serializable] sealed class Source { public string texture, layer; }
    [System.Serializable] sealed class Manifest { public Source[] sources; }
    internal static SlashLibrary Library() => AssetDatabase.LoadAssetAtPath<SlashLibrary>(Root + "SlashLibrary.asset");
    internal static void Prepare()
    {
        if (Library()) return;
        var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(Root + "sources.json"));
        var library = ScriptableObject.CreateInstance<SlashLibrary>();
        AssetDatabase.CreateAsset(library, Root + "SlashLibrary.asset");
        var material = new Material(Shader.Find("TypingHero/ParryVoxel")); material.SetFloat("_Thermal", 0); material.SetFloat("_Emission", .3f);
        AssetDatabase.CreateAsset(material, Root + "Slash.mat");
        var data = new SerializedObject(library); data.FindProperty("material").objectReferenceValue = material;
        var frames = data.FindProperty("frames"); frames.arraySize = 0;
        foreach (var source in manifest.sources)
        {
            var layer = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            layer.LoadImage(File.ReadAllBytes(Root + source.layer));
            try
            {
                foreach (var sprite in AssetDatabase.LoadAllAssetsAtPath(source.texture).OfType<Sprite>())
                {
                    var front = Build(sprite, layer, false); var back = Build(sprite, layer, true);
                    if (!front && !back) continue;
                    string prefix = AssetDatabase.AssetPathToGUID(source.texture) + "-" + sprite.rect.x + "-" + sprite.rect.y;
                    if (front) AssetDatabase.CreateAsset(front, Root + prefix + "-front.asset");
                    if (back) AssetDatabase.CreateAsset(back, Root + prefix + "-back.asset");
                    int index = frames.arraySize++; var row = frames.GetArrayElementAtIndex(index);
                    row.FindPropertyRelative("sprite").objectReferenceValue = sprite;
                    row.FindPropertyRelative("front").objectReferenceValue = front; row.FindPropertyRelative("back").objectReferenceValue = back;
                }
            }
            finally { Object.DestroyImmediate(layer); }
        }
        data.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(library);
    }
    static Mesh Build(Sprite sprite, Texture2D layer, bool behind)
    {
        var rect = sprite.rect; int w = (int)rect.width, h = (int)rect.height, split = Mathf.CeilToInt(sprite.pivot.x);
        bool Has(int x, int y) => x >= 0 && y >= 0 && x < w && y < h && layer.GetPixel((int)rect.x + x, (int)rect.y + y).a > 0;
        var vertices = new List<Vector3>(); var triangles = new List<int>(); var colors = new List<Color32>();
        void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color32 color)
        {
            int n = vertices.Count; vertices.Add(a); vertices.Add(b); vertices.Add(c); vertices.Add(d);
            for (int i = 0; i < 4; i++) colors.Add(color);
            triangles.Add(n); triangles.Add(n + 1); triangles.Add(n + 2); triangles.Add(n); triangles.Add(n + 2); triangles.Add(n + 3);
        }
        float Z(int x) => Mathf.Sin(x / (float)w * Mathf.PI * 2) * .08f;
        float HalfThickness(int x, int y)
        {
            float sum = 0; int count = 0;
            for (int yy = y - 1; yy <= y; yy++) for (int xx = x - 1; xx <= x; xx++)
                if (Has(xx, yy)) { sum += Has(xx - 1, yy) && Has(xx + 1, yy) ? 1f / 16 : 1f / 32; count++; }
            return sum / count * .5f;
        }
        Vector3 Point(int x, int y, float side) => new Vector3((x - sprite.pivot.x) / sprite.pixelsPerUnit,
            (y - sprite.pivot.y) / sprite.pixelsPerUnit, Z(x) + side * HalfThickness(x, y));
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (!Has(x, y) || (x < sprite.pivot.x) != behind) continue;
            Color32 tint = layer.GetPixel((int)rect.x + x, (int)rect.y + y);
            // Shared corner thickness keeps the front/back shell sealed when the tip thins.
            Vector3 a=Point(x,y,-1), b=Point(x+1,y,-1), c=Point(x+1,y+1,-1), d=Point(x,y+1,-1);
            Vector3 aa=Point(x,y,1), bb=Point(x+1,y,1), cc=Point(x+1,y+1,1), dd=Point(x,y+1,1);
            Quad(d,c,b,a,tint); Quad(aa,bb,cc,dd,tint);
            if (!Has(x-1,y) || x == split) Quad(a,aa,dd,d,tint);
            if (!Has(x+1,y) || x+1 == split) Quad(c,cc,bb,b,tint);
            if (!Has(x,y-1)) Quad(b,bb,aa,a,tint);
            if (!Has(x,y+1)) Quad(d,dd,cc,c,tint);
        }
        if (vertices.Count == 0) return null;
        var mesh = new Mesh {name = sprite.name + (behind ? "-back" : "-front"), indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};
        mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.SetColors(colors); mesh.RecalculateNormals(); mesh.RecalculateBounds();
        return mesh;
    }
}
