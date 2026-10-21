using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// One art-review room: original pixel tiles on real, deliberately simple geometry.
// Meshes are authored once into assets and updated in place to retain their GUIDs.
internal static class DungeonPixelRoomBuild
{
    internal const string Art = "Assets/Art/Environment/PixelDungeon/";
    const float DoorX = 2.2f;

    internal static void Apply(GameObject root)
    {
        var pixelRoot = root.transform.Find("PixelRoom");
        foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            if (!pixelRoot || !renderer.transform.IsChildOf(pixelRoot)) renderer.enabled = false;

        var room = Node(root, "PixelRoom");
        var stone = Surface("Stone", "wall_mid", new Color(.69f, .75f, .81f));
        var trim = Surface("CutStone", "wall_mid", new Color(.8f, .83f, .88f));
        var dark = Surface("Recess", "wall_mid", new Color(.19f, .22f, .29f));
        var door = Surface("ClosedDoor", "doors_leaf_closed", new Color(.78f, .72f, .64f), true);
        var banner = Surface("Banner", "wall_banner_red", new Color(.48f, .36f, .36f), true);
        var crack = Surface("DamagedStone", "wall_hole_1", new Color(.6f, .66f, .72f));

        // No old room bake/probe/reflection data may light this new layout.
        Lightmapping.lightingDataAsset = null;
        LightmapSettings.lightmaps = new LightmapData[0];
        const string lightingPath = Art + "PreviewLighting.asset";
        var lighting = AssetDatabase.LoadAssetAtPath<LightingSettings>(lightingPath);
        if (!lighting) { lighting = new LightingSettings(); AssetDatabase.CreateAsset(lighting, lightingPath); }
        lighting.bakedGI = false; lighting.realtimeGI = false;
        Lightmapping.lightingSettings = lighting; EditorUtility.SetDirty(lighting);

        // Preserve legacy GUIDs for rollback; the perspective floor is authored
        // after the final camera pose, rather than extending old tile strips.
        foreach (Transform child in room.transform)
            if (child.name.StartsWith("Floor") && child.TryGetComponent<MeshRenderer>(out var oldFloor)) oldFloor.enabled=false;

        // An actual thick wall and recessed CLOSED door; no corridor/open door.
        Box(room, "WallLeft", new Vector3(-6.95f, 3, 6.8f), new Vector3(14.3f, 6, 1), stone);
        Box(room, "WallRight", new Vector3(9.15f, 3, 6.8f), new Vector3(9.7f, 6, 1), stone);
        Box(room, "WallAboveDoor", new Vector3(DoorX, 5.35f, 6.8f), new Vector3(4.2f, 1.3f, 1), stone);
        Box(room, "DoorShadow", new Vector3(DoorX, 2.25f, 7.52f), new Vector3(4.2f, 4.5f, .25f), dark);
        Plane(room, "ClosedDoor", new Vector3(DoorX, 2.05f, 7.15f), new Vector2(3.3f, 4.1f), door);
        for (int side = -1; side <= 1; side += 2)
        {
            Box(room, "DoorJamb" + side, new Vector3(DoorX + side * 1.87f, 1.3f, 6.38f), new Vector3(.55f, 2.6f, 1.5f), trim);
            for (int i = 0; i < 3; i++)
                Box(room, "JambJoint" + side + "_" + i, new Vector3(DoorX + side * 1.87f, .12f + i * 1.1f, 6.12f), new Vector3(.66f, .13f, 1.2f), stone);
        }
        Arch(room, trim);
        Box(room, "DoorSill", new Vector3(DoorX, .08f, 6.18f), new Vector3(4.3f, .16f, 1.4f), trim);
        foreach (int side in new[] { -1, 1 })
        {
            float x = side < 0 ? -8.6f : 9.3f;
            Box(room, "WallPier" + side, new Vector3(x, 3, 6.03f), new Vector3(.95f, 6, 1.15f), stone);
            Box(room, "PierFoot" + side, new Vector3(x, .3f, 5.95f), new Vector3(1.35f, .6f, 1.4f), trim);
            Box(room, "PierCapital" + side, new Vector3(x, 5.25f, 5.95f), new Vector3(1.35f, .38f, 1.4f), trim);
            Box(room, "FrontPier" + side, new Vector3(side * 9.4f, 3, -2), new Vector3(1.25f, 6, 1.4f), stone, side * 18);
            Box(room, "FrontPierFoot" + side, new Vector3(side * 9.4f, .24f, -2), new Vector3(1.65f, .48f, 1.8f), trim, side * 18);
            for (int i = 0; i < 4; i++)
                Box(room, "Rubble" + side + "_" + i, new Vector3(side * (8.4f + i * .34f), .12f + (i % 2) * .1f, 4.4f + i * .32f),
                    new Vector3(.4f + i * .09f, .24f, .45f), stone, i * 19);
        }
        Box(room, "LeftSkirting", new Vector3(-6.95f, .22f, 6.18f), new Vector3(14.3f, .44f, .35f), dark);
        Box(room, "RightSkirting", new Vector3(9.15f, .22f, 6.18f), new Vector3(9.7f, .44f, .35f), dark);
        Plane(room, "OldBanner", new Vector3(-2.7f, 3.3f, 6.27f), new Vector2(1, 2), banner);
        Plane(room, "ChippedStone", new Vector3(-7, .95f, 6.28f), Vector2.one, crack);
        // Separate the back wall from the combat lane without moving actors.
        // Assign absolute offsets so repeated builder runs cannot accumulate drift.
        foreach (Transform child in room.transform)
        {
            if (child.name.StartsWith("Floor") || child.name.StartsWith("Ground") || child.name.StartsWith("FrontPier")) continue;
            child.localPosition += Vector3.forward * 3.5f;
        }
    }

    internal static void ApplyGround(GameObject root,Camera camera)
    {
        var room=Node(root,"PixelRoom");
        var floor=new UnityEngine.Plane(Vector3.up,root.transform.position);
        // Derive the reviewed coverage explicitly, independently of editor camera
        // projection state; the approved gameplay camera remains unchanged.
        const float coverageFov=45;
        float tangent=Mathf.Tan(coverageFov*.5f*Mathf.Deg2Rad);
        var backPoint=camera.transform.InverseTransformPoint(new Vector3(camera.transform.position.x,root.transform.position.y,root.transform.position.z+11));
        float backV=.5f+.5f*backPoint.y/(backPoint.z*tangent);
        var a=Intersect(camera,new Vector2(0,0),floor,coverageFov)-root.transform.position;
        var b=Intersect(camera,new Vector2(1,0),floor,coverageFov)-root.transform.position;
        var c=Intersect(camera,new Vector2(0,backV),floor,coverageFov)-root.transform.position;
        var d=Intersect(camera,new Vector2(1,backV),floor,coverageFov)-root.transform.position;
        var backing=new UnityEngine.Plane(Vector3.forward,new Vector3(0,0,root.transform.position.z+9.8f));
        float height=Mathf.Max(6,Mathf.Ceil(Intersect(camera,new Vector2(.5f,1),backing,coverageFov).y-root.transform.position.y+.5f));
        float minX=Mathf.Floor(Mathf.Min(a.x,c.x)/2)*2-4;
        float maxX=Mathf.Ceil(Mathf.Max(b.x,d.x)/2)*2+4;
        float minZ=Mathf.Floor(Mathf.Min(a.z,b.z)/2)*2-2;
        float maxZ=12;
        var vertices=new List<Vector3>();
        var uv=new List<Vector2>();
        var triangles=new List<int>[8];
        var materials=new Material[8];
        var tint=AssetDatabase.LoadAssetAtPath<Material>(Art+"Stone.mat").GetColor("_BaseColor");
        for(int i=0;i<8;i++)
        {
            triangles[i]=new List<int>();
            materials[i]=Surface("Floor"+(i+1),"floor_"+(i+1),tint);
        }
        // Coordinate hash keeps the sparse variants stable across builder runs.
        for(int z=(int)minZ;z<(int)maxZ;z++)
            for(int x=(int)minX;x<(int)maxX;x++)
            {
                uint hash=unchecked((uint)x*73856093u ^ (uint)z*19349663u);
                hash^=hash>>16;hash=unchecked(hash*2246822519u);hash^=hash>>13;
                int tile=hash%4u==0 ? 1+(int)((hash/4u)%7u) : 0;
                Quad(vertices,uv,triangles[tile],new Vector3(x,0,z),new Vector3(x,0,z+1),
                    new Vector3(x+1,0,z+1),new Vector3(x+1,0,z),Vector2.one);
            }
        var ground=SaveMesh(room,"GroundSurface",vertices,uv,triangles[0],materials[0]);
        var mesh=ground.GetComponent<MeshFilter>().sharedMesh;
        mesh.subMeshCount=8;
        for(int i=0;i<8;i++)mesh.SetTriangles(triangles[i],i);
        // Variant vertices are referenced only after all submeshes are assigned.
        mesh.RecalculateNormals();
        ground.GetComponent<MeshRenderer>().sharedMaterials=materials;
        EditorUtility.SetDirty(mesh);
        ground.transform.localPosition=Vector3.zero;ground.transform.localRotation=Quaternion.identity;
        ground.GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;
        ground.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
        var recess=AssetDatabase.LoadAssetAtPath<Material>(Art+"Recess.mat");
        Box(room,"GroundSlab",new Vector3((minX+maxX)*.5f,-.0775f,(minZ+maxZ)*.5f),new Vector3(maxX-minX,.15f,maxZ-minZ),recess);
        room.transform.Find("GroundSlab").GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;

        // Extend only the backing outside the original door opening. Perspective
        // reveals a wider/taller back plane than the old orthographic room.
        var stone=AssetDatabase.LoadAssetAtPath<Material>(Art+"Stone.mat");
        var trim=AssetDatabase.LoadAssetAtPath<Material>(Art+"CutStone.mat");
        Box(room,"WallLeft",new Vector3((minX+.2f)*.5f,height*.5f,10.3f),new Vector3(.2f-minX,height,1),stone);
        Box(room,"WallRight",new Vector3((4.3f+maxX)*.5f,height*.5f,10.3f),new Vector3(maxX-4.3f,height,1),stone);
        Box(room,"WallAboveDoor",new Vector3(DoorX,(4.7f+height)*.5f,10.3f),new Vector3(4.2f,height-4.7f,1),stone);
        var frontPlane=new UnityEngine.Plane(Vector3.forward,new Vector3(0,0,root.transform.position.z-2));
        foreach(int side in new[]{-1,1})
        {
            float x=Intersect(camera,new Vector2(side < 0 ? .06f : .94f,.5f),frontPlane).x-root.transform.position.x;
            Box(room,"FrontPier"+side,new Vector3(x,3,-2),new Vector3(1.25f,6,1.4f),stone,side*18);
            Box(room,"FrontPierFoot"+side,new Vector3(x,.24f,-2),new Vector3(1.65f,.48f,1.8f),trim,side*18);
        }
    }
    static Vector3 Intersect(Camera camera,Vector2 viewport,UnityEngine.Plane plane,float fieldOfView=0)
    {
        var ray=camera.ViewportPointToRay(viewport);
        if(fieldOfView>0)
        {
            float tangent=Mathf.Tan(fieldOfView*.5f*Mathf.Deg2Rad);
            ray=new Ray(camera.transform.position,camera.transform.TransformDirection(
                new Vector3((viewport.x*2-1)*tangent*camera.aspect,(viewport.y*2-1)*tangent,1)));
        }
        if(!plane.Raycast(ray,out float distance))throw new System.InvalidOperationException("Dungeon camera does not cover its ground/backing plane");
        return ray.GetPoint(distance);
    }
    static Material Surface(string name, string tile, Color tint, bool cutout = false)
    {
        string texturePath = Art + tile + ".png";
        var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
        if (!importer) throw new System.InvalidOperationException("Missing original pixel tile " + texturePath);
        if (importer.textureType != TextureImporterType.Default || importer.filterMode != FilterMode.Point || importer.mipmapEnabled ||
            importer.wrapMode != TextureWrapMode.Repeat || importer.textureCompression != TextureImporterCompression.Uncompressed || !importer.alphaIsTransparency)
        {
            importer.textureType = TextureImporterType.Default; importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false; importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
        string path = Art + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!material) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
        material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
        material.SetColor("_BaseColor", tint); material.SetFloat("_Smoothness", .02f);
        material.SetTexture("_BumpMap", null); material.SetTexture("_MetallicGlossMap", null);
        material.SetTexture("_OcclusionMap", null);
        material.SetFloat("_Metallic", 0); material.SetFloat("_Cull", cutout ? 0 : 2);
        material.SetFloat("_AlphaClip", cutout ? 1 : 0); material.SetFloat("_Cutoff", .35f);
        if (cutout) material.EnableKeyword("_ALPHATEST_ON");
        material.renderQueue = cutout ? (int)RenderQueue.AlphaTest : (int)RenderQueue.Geometry;
        material.SetOverrideTag("RenderType", cutout ? "TransparentCutout" : "Opaque");
        // Initialize URP's cutout/legacy properties immediately, rather than
        // leaving its import-time validation to mutate a later repeated build.
        UnityEditor.BaseShaderGUI.SetMaterialKeywords(material, UnityEditor.Rendering.Universal.ShaderGUI.LitGUI.SetMaterialKeywords);
        EditorUtility.SetDirty(material); return material;
    }

    static void Box(GameObject room, string name, Vector3 center, Vector3 size, Material material, float yaw = 0)
    {
        var v = new List<Vector3>(); var uv = new List<Vector2>(); var t = new List<int>();
        var half = size * .5f;
        var a = new Vector3(-half.x, -half.y, -half.z); var b = new Vector3(half.x, half.y, half.z);
        Quad(v, uv, t, new Vector3(a.x, a.y, a.z), new Vector3(a.x, b.y, a.z), new Vector3(b.x, b.y, a.z), new Vector3(b.x, a.y, a.z), new Vector2(size.x, size.y));
        Quad(v, uv, t, new Vector3(b.x, a.y, b.z), new Vector3(b.x, b.y, b.z), new Vector3(a.x, b.y, b.z), new Vector3(a.x, a.y, b.z), new Vector2(size.x, size.y));
        Quad(v, uv, t, new Vector3(a.x, a.y, b.z), new Vector3(a.x, b.y, b.z), new Vector3(a.x, b.y, a.z), new Vector3(a.x, a.y, a.z), new Vector2(size.z, size.y));
        Quad(v, uv, t, new Vector3(b.x, a.y, a.z), new Vector3(b.x, b.y, a.z), new Vector3(b.x, b.y, b.z), new Vector3(b.x, a.y, b.z), new Vector2(size.z, size.y));
        Quad(v, uv, t, new Vector3(a.x, b.y, a.z), new Vector3(a.x, b.y, b.z), new Vector3(b.x, b.y, b.z), new Vector3(b.x, b.y, a.z), new Vector2(size.x, size.z));
        Quad(v, uv, t, new Vector3(a.x, a.y, b.z), new Vector3(a.x, a.y, a.z), new Vector3(b.x, a.y, a.z), new Vector3(b.x, a.y, b.z), new Vector2(size.x, size.z));
        var node = SaveMesh(room, name, v, uv, t, material);
        node.transform.localPosition = center; node.transform.localRotation = Quaternion.Euler(0, yaw, 0);
    }

    static void Plane(GameObject room, string name, Vector3 center, Vector2 size, Material material)
    {
        var v = new List<Vector3>(); var uv = new List<Vector2>(); var t = new List<int>();
        Quad(v, uv, t, new Vector3(-size.x / 2, -size.y / 2, 0), new Vector3(-size.x / 2, size.y / 2, 0),
            new Vector3(size.x / 2, size.y / 2, 0), new Vector3(size.x / 2, -size.y / 2, 0), Vector2.one);
        SaveMesh(room, name, v, uv, t, material).transform.localPosition = center;
    }

    static void Arch(GameObject room, Material material)
    {
        var v = new List<Vector3>(); var uv = new List<Vector2>(); var t = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            float a = i * Mathf.PI / 9 + .008f, b = (i + 1) * Mathf.PI / 9 - .008f;
            var ai = new Vector3(Mathf.Cos(a) * 1.6f, 2.6f + Mathf.Sin(a) * 1.6f, 5.6f);
            var ao = new Vector3(Mathf.Cos(a) * 2.1f, 2.6f + Mathf.Sin(a) * 2.1f, 5.6f);
            var bi = new Vector3(Mathf.Cos(b) * 1.6f, 2.6f + Mathf.Sin(b) * 1.6f, 5.6f);
            var bo = new Vector3(Mathf.Cos(b) * 2.1f, 2.6f + Mathf.Sin(b) * 2.1f, 5.6f);
            Quad(v, uv, t, bi, bo, ao, ai, new Vector2(.65f, .5f));
            Quad(v, uv, t, ai, ai + Vector3.forward * 1.2f, bi + Vector3.forward * 1.2f, bi, new Vector2(.65f, 1.2f));
            Quad(v, uv, t, ao, bo, bo + Vector3.forward * 1.2f, ao + Vector3.forward * 1.2f, Vector2.one);
        }
        SaveMesh(room, "StoneArch", v, uv, t, material).transform.localPosition = new Vector3(DoorX, 0, 0);
    }

    static void Quad(List<Vector3> v, List<Vector2> uv, List<int> t, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 tiling)
    {
        int start = v.Count; v.AddRange(new[] { a, b, c, d });
        uv.AddRange(new[] { Vector2.zero, new Vector2(0, tiling.y), tiling, new Vector2(tiling.x, 0) });
        t.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });
    }

    static GameObject SaveMesh(GameObject room, string name, List<Vector3> v, List<Vector2> uv, List<int> t, Material material)
    {
        string path = Art + name + ".asset";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (!mesh) { mesh = new Mesh { name = name }; AssetDatabase.CreateAsset(mesh, path); }
        mesh.Clear(); mesh.SetVertices(v); mesh.SetUVs(0, uv); mesh.SetTriangles(t, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
        EditorUtility.SetDirty(mesh);
        var node = Node(room, name);
        var filter = node.GetComponent<MeshFilter>(); if (!filter) filter = node.AddComponent<MeshFilter>();
        var renderer = node.GetComponent<MeshRenderer>(); if (!renderer) renderer = node.AddComponent<MeshRenderer>();
        filter.sharedMesh = mesh; renderer.sharedMaterial = material; renderer.enabled = true;
        renderer.shadowCastingMode = ShadowCastingMode.On; renderer.receiveShadows = true;
        renderer.lightProbeUsage = LightProbeUsage.Off; renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        renderer.lightmapIndex = -1; GameObjectUtility.SetStaticEditorFlags(node, StaticEditorFlags.BatchingStatic);
        return node;
    }

    static GameObject Node(GameObject root, string name)
    {
        var child = root.transform.Find(name); if (child) return child.gameObject;
        var node = new GameObject(name); node.transform.SetParent(root.transform, false); return node;
    }
}
