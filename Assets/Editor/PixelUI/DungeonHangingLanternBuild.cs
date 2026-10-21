using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// KayKit source stays intact; the derived mesh opens the lower base for combat lighting.
internal static class DungeonHangingLanternBuild
{
    internal const string Art = "Assets/Art/Environment/HangingLantern/";

    internal static void Apply(GameObject room, Vector3 flame)
    {
        var importer = (ModelImporter)AssetImporter.GetAtPath(Art + "lantern_hanging.fbx");
        if (importer.materialImportMode != ModelImporterMaterialImportMode.None || !importer.isReadable || importer.importAnimation)
        {
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.importAnimation = false;
            importer.isReadable = true;
            importer.SaveAndReimport();
        }
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(Art + "lantern_hanging.fbx");
        var filters = source.GetComponentsInChildren<MeshFilter>();
        if (filters.Length != 1) throw new System.InvalidOperationException("Expected the single authored KayKit lantern mesh");
        var original = filters[0].sharedMesh;
        var matrix = filters[0].transform.localToWorldMatrix;
        var vertices = original.vertices.Select(matrix.MultiplyPoint3x4).ToArray();
        var normalMatrix = matrix.inverse.transpose;
        var normals = original.normals.Select(n => normalMatrix.MultiplyVector(n).normalized).ToArray();
        var indices = original.triangles;
        if (indices.Length != 472 * 3) throw new System.InvalidOperationException("KayKit lantern must remain 472 triangles");
        var bounds = BoundsOf(vertices);
        float scale = .78f / bounds.size.x;
        var uv = original.uv;
        var windows = Enumerable.Range(0, 472).Select(t =>
            (uv[indices[t * 3]].y + uv[indices[t * 3 + 1]].y + uv[indices[t * 3 + 2]].y) / 3 < .5f).ToArray();
        if (windows.Count(w => w) != 40) throw new System.InvalidOperationException("Original UVs must identify 40 window and 432 metal triangles");
        var windowBounds = BoundsOf(Enumerable.Range(0, indices.Length).Where(i => windows[i / 3]).Select(i => vertices[indices[i]]));

        // Remove only the 26 triangles of the closed lower plinth. Keep the authored
        // roof, four corner posts, windows and chain; all surviving iron still casts.
        var openBase = Enumerable.Range(0, 472).Select(t => !windows[t] &&
            Enumerable.Range(t * 3, 3).All(i => vertices[indices[i]].y <= windowBounds.min.y + .00001f)).ToArray();
        if (openBase.Count(b => b) != 26) throw new System.InvalidOperationException("Expected the KayKit lower plinth to contain 26 triangles");

        var frame = Node(room.transform, "HangingLampFrame");
        foreach (var child in frame.Cast<Transform>().ToArray())
            if (child.name != "SupportBeam" && child.name != "LanternModel") Object.DestroyImmediate(child.gameObject);
        var lantern = Node(frame, "LanternModel");
        var rotation = Quaternion.Euler(0, 22.5f, 0);
        var chamberCenter = flame - new Vector3(0, .035f, .10f);
        lantern.localPosition = chamberCenter - rotation * (windowBounds.center * scale);
        lantern.localRotation = rotation;
        lantern.localScale = Vector3.one * scale;
        var mesh = PixelMesh(vertices, normals, indices, windows, openBase, scale);
        var atlas = Texture("LanternPixelAtlas.png", TextureWrapMode.Clamp);
        Texture("metal0.png", TextureWrapMode.Clamp);
        var metal = Material("LanternIron", atlas, new Color(.33f, .37f, .40f), true);
        Render(lantern, mesh, metal);
        var glassMesh = Mesh("LanternGlassMesh");
        glassMesh.Clear();
        glassMesh.vertices = mesh.vertices;
        glassMesh.normals = mesh.normals;
        var frameIndices = mesh.triangles;
        var frameUV = mesh.uv;
        glassMesh.triangles = Enumerable.Range(0, frameIndices.Length / 3)
            .Where(t => frameUV[frameIndices[t * 3]].x > .7f)
            .SelectMany(t => frameIndices.Skip(t * 3).Take(3)).ToArray();
        if (glassMesh.triangles.Length != 40 * 3)
            throw new System.InvalidOperationException("Lantern glass must reuse exactly 40 window triangles");
        glassMesh.RecalculateBounds();
        EditorUtility.SetDirty(glassMesh);
        var glass = Material("LanternGlass", null, new Color(0, 0, 0, .12f), false);
        glass.SetColor("_EmissionColor", new Color(1, .38f, .08f));
        glass.EnableKeyword("_EMISSION");
        glass.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        glass.SetFloat("_Surface", 1); glass.SetFloat("_Blend", 0);
        glass.SetFloat("_BlendModePreserveSpecular", 0);
        glass.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        glass.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        glass.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        glass.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
        glass.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
        glass.SetFloat("_ZWrite", 0); glass.SetFloat("_Smoothness", 0);
        glass.SetFloat("_ReceiveShadows", 0); glass.EnableKeyword("_RECEIVE_SHADOWS_OFF");
        glass.SetOverrideTag("RenderType", "Transparent");
        glass.renderQueue = (int)RenderQueue.Transparent;
        glass.SetShaderPassEnabled("ShadowCaster", false);
        var glassNode = Node(lantern, "Glass");
        glassNode.localPosition = Vector3.zero; glassNode.localRotation = Quaternion.identity;
        glassNode.localScale = Vector3.one;
        Render(glassNode, glassMesh, glass);
        var glassRenderer = glassNode.GetComponent<MeshRenderer>();
        glassRenderer.shadowCastingMode = ShadowCastingMode.Off;
        glassRenderer.receiveShadows = false;
        var fire = room.transform.Find("Torch_Front/ModelMesh");
        var fireMesh = fire.GetComponent<MeshFilter>().sharedMesh;
        foreach (int index in fireMesh.triangles)
            if (!windowBounds.Contains(lantern.InverseTransformPoint(fire.TransformPoint(fireMesh.vertices[index]))))
                throw new System.InvalidOperationException("Lantern fire outside window: bounds=" + windowBounds.ToString("F5") + " vertex=" + lantern.InverseTransformPoint(fire.TransformPoint(fireMesh.vertices[index])).ToString("F5") + " scale=" + scale);
        Debug.Log("LANTERN_CHAMBER sourceBounds=" + windowBounds + " center=" + chamberCenter);

        var left = room.transform.Find("PixelRoom/FrontPier-1");
        var right = room.transform.Find("PixelRoom/FrontPier1");
        float hookTop = lantern.localPosition.y + bounds.max.y * scale;
        var beam = Node(frame, "SupportBeam");
        beam.localPosition = new Vector3((left.localPosition.x + right.localPosition.x) * .5f, hookTop + .09f, flame.z);
        beam.localRotation = Quaternion.identity;
        beam.localScale = new Vector3(right.localPosition.x - left.localPosition.x + 1, .28f, .36f);
        var originalBeam = AssetDatabase.LoadAssetAtPath<Mesh>(DungeonArenaBuild.Art + "LampWoodBeam.asset");
        var beamMesh = Mesh("LanternBeam");
        var beamVertices = originalBeam.vertices;
        var beamUV = new Vector2[beamVertices.Length];
        for (int i = 0; i < beamVertices.Length; i++)
            beamUV[i] = Project(Vector3.Scale(beamVertices[i], beam.localScale), originalBeam.normals[i]);
        beamMesh.Clear(); beamMesh.vertices = beamVertices; beamMesh.normals = originalBeam.normals;
        beamMesh.uv = beamUV; beamMesh.triangles = originalBeam.triangles; beamMesh.RecalculateBounds();
        EditorUtility.SetDirty(beamMesh);
        var wood = Material("LanternWood", Texture("oak_planks.png", TextureWrapMode.Repeat), new Color(.60f, .55f, .50f), false);
        Render(beam, beamMesh, wood);
        Debug.Log("LANTERN_ASSET triangles=" + mesh.triangles.Length / 3 + " vertices=" + mesh.vertexCount +
            " glassTriangles=40 submittedTriangles=486 sourceTriangles=472 openBase=26 windows=40 metal=406 flame=" + flame + " width=" + bounds.size.x * scale + " hook=" + hookTop);
    }

    // Connected coplanar faces share an island; neighboring pixels stay aligned across triangle diagonals.
    static UnityEngine.Mesh PixelMesh(Vector3[] vertices, Vector3[] normals, int[] indices, bool[] windows, bool[] openBase, float scale)
    {
        var groups = Enumerable.Range(0, windows.Length).ToArray();
        var faceNormals = new Vector3[windows.Length];
        var edges = new Dictionary<string, List<int>>();
        for (int t = 0; t < groups.Length; t++)
        {
            var a = vertices[indices[t * 3]]; var b = vertices[indices[t * 3 + 1]]; var c = vertices[indices[t * 3 + 2]];
            faceNormals[t] = Vector3.Cross(b - a, c - a).normalized;
            if (windows[t]) continue;
            for (int j = 0; j < 3; j++)
            {
                string p = PointKey(vertices[indices[t * 3 + j]]), q = PointKey(vertices[indices[t * 3 + (j + 1) % 3]]);
                string edge = string.CompareOrdinal(p, q) < 0 ? p + "/" + q : q + "/" + p;
                if (!edges.TryGetValue(edge, out var neighbors)) edges[edge] = neighbors = new List<int>();
                foreach (int previous in neighbors)
                    if (Vector3.Dot(faceNormals[t], faceNormals[previous]) > .99999f) groups[Find(groups, t)] = Find(groups, previous);
                neighbors.Add(t);
            }
        }
        var positions = new List<Vector3>(); var mappedNormals = new List<Vector3>(); var mappedUV = new List<Vector2>();
        var triangles = new int[indices.Length];
        foreach (var island in Enumerable.Range(0, groups.Length).Where(t => !openBase[t]).GroupBy(t => windows[t] ? -1 : Find(groups, t)))
        {
            bool clear = island.Key == -1;
            var direction = faceNormals[island.First()];
            var projected = island.SelectMany(t => Enumerable.Range(t * 3, 3)).Select(i => Project(vertices[indices[i]], direction) * scale * 16).ToArray();
            var minimum = projected.Aggregate(Vector2.Min); var maximum = projected.Aggregate(Vector2.Max);
            if (!clear && Mathf.Max((maximum - minimum).x, (maximum - minimum).y) > 16)
                throw new System.InvalidOperationException("Lantern UV island exceeds the original 16px tile");
            var offset = Vector2.one * 9 - (minimum + maximum) * .5f;
            var lookup = new Dictionary<int, int>();
            foreach (int t in island)
            for (int j = 0; j < 3; j++)
            {
                int index = indices[t * 3 + j];
                if (!lookup.TryGetValue(index, out int mapped))
                {
                    mapped = positions.Count; lookup.Add(index, mapped);
                    positions.Add(vertices[index]); mappedNormals.Add(normals[index]);
                    mappedUV.Add(clear ? new Vector2(24.5f, 24.5f) / 32 : (Project(vertices[index], direction) * scale * 16 + offset) / 32);
                }
                triangles[t * 3 + j] = mapped;
            }
        }
        var mesh = Mesh("LanternPixelMesh");
        mesh.Clear(); mesh.SetVertices(positions); mesh.SetNormals(mappedNormals); mesh.SetUVs(0, mappedUV);
        mesh.triangles = Enumerable.Range(0, groups.Length).Where(t => !openBase[t])
            .SelectMany(t => Enumerable.Range(t * 3, 3).Select(i => triangles[i])).ToArray();
        mesh.RecalculateBounds(); EditorUtility.SetDirty(mesh);
        return mesh;
    }

    static int Find(int[] groups, int t)
    {
        while (groups[t] != t) { groups[t] = groups[groups[t]]; t = groups[t]; }
        return t;
    }
    static string PointKey(Vector3 p) => Mathf.RoundToInt(p.x * 100000) + "," + Mathf.RoundToInt(p.y * 100000) + "," + Mathf.RoundToInt(p.z * 100000);
    static Vector2 Project(Vector3 p, Vector3 normal)
    {
        var n = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
        return n.y >= n.x && n.y >= n.z ? new Vector2(p.x, p.z) : n.x >= n.z ? new Vector2(p.z, p.y) : new Vector2(p.x, p.y);
    }
    static Bounds BoundsOf(IEnumerable<Vector3> points)
    {
        var bounds = new Bounds(points.First(), Vector3.zero);
        foreach (var p in points) bounds.Encapsulate(p);
        return bounds;
    }
    static UnityEngine.Mesh Mesh(string name)
    {
        string path = Art + name + ".asset";
        var mesh = AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(path);
        if (!mesh) { mesh = new UnityEngine.Mesh { name = name }; AssetDatabase.CreateAsset(mesh, path); }
        return mesh;
    }
    static Transform Node(Transform parent, string name)
    {
        var node = parent.Find(name);
        if (!node) { node = new GameObject(name).transform; node.SetParent(parent, false); }
        return node;
    }
    static void Render(Transform node, UnityEngine.Mesh mesh, UnityEngine.Material material)
    {
        var filter = node.GetComponent<MeshFilter>(); if (!filter) filter = node.gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        var renderer = node.GetComponent<MeshRenderer>(); if (!renderer) renderer = node.gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material; renderer.enabled = true;
        renderer.shadowCastingMode = ShadowCastingMode.On; renderer.receiveShadows = true;
        renderer.lightProbeUsage = LightProbeUsage.Off; renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
    }
    static Texture2D Texture(string name, TextureWrapMode wrap)
    {
        string path = Art + name;
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer.filterMode != FilterMode.Point || importer.mipmapEnabled || importer.wrapMode != wrap ||
            importer.textureCompression != TextureImporterCompression.Uncompressed || importer.alphaIsTransparency)
        {
            importer.textureType = TextureImporterType.Default; importer.textureShape = TextureImporterShape.Texture2D;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false; importer.wrapMode = wrap;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.alphaIsTransparency = false;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static UnityEngine.Material Material(string name, Texture2D texture, Color tint, bool cutout)
    {
        string path = Art + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(path);
        if (!material) { material = new UnityEngine.Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
        material.SetTexture("_BaseMap", texture); material.SetTexture("_MainTex", texture);
        material.SetColor("_BaseColor", tint); material.SetColor("_Color", tint);
        material.SetFloat("_Surface", 0); material.SetFloat("_AlphaClip", cutout ? 1 : 0); material.SetFloat("_Cutoff", .5f);
        material.SetFloat("_AlphaToMask", cutout ? 1 : 0);
        material.SetFloat("_Smoothness", .02f); material.SetFloat("_Metallic", 0); material.SetColor("_EmissionColor", Color.black);
        material.DisableKeyword("_EMISSION");
        material.SetFloat("_SpecularHighlights", 0); material.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
        material.SetFloat("_EnvironmentReflections", 0); material.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
        if (cutout) material.EnableKeyword("_ALPHATEST_ON"); else material.DisableKeyword("_ALPHATEST_ON");
        material.SetOverrideTag("RenderType", cutout ? "TransparentCutout" : "Opaque");
        material.renderQueue = cutout ? (int)RenderQueue.AlphaTest : (int)RenderQueue.Geometry;
        material.SetShaderPassEnabled("ShadowCaster", true);
        EditorUtility.SetDirty(material);
        return material;
    }
}
