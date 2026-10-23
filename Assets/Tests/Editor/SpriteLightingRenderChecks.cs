using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Explicit offscreen GPU diagnostic, never included in the game or normal builds.
public static class SpriteLightingRenderChecks
{
    [Serializable] sealed class Cell
    {
        public string source, sprite, file;
        public int mode;
        public long localId;
        public Vector3 light;
    }
    [Serializable] sealed class CaptureRecord { public string device; public Cell[] cells; }
    static bool verticalOnly;
    public static void CaptureVertical()
    {
        verticalOnly = true;
        try { Capture(); }
        finally { verticalOnly = false; }
    }
    static float IdleFoot(string directory)
    {
        string path = Directory.GetFiles(directory, "*.png").First(x => Path.GetFileName(x).ToLowerInvariant().Contains("idle")).Replace('\\', '/');
        var sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(x => x.rect.x).First();
        var raw = new Texture2D(2,2);
        try
        {
            raw.LoadImage(File.ReadAllBytes(path));
            int bottom = (int)sprite.rect.yMax;
            for (int y = (int)sprite.rect.y; y < sprite.rect.yMax; y++)
                for (int x = (int)sprite.rect.x; x < sprite.rect.xMax; x++)
                    if (raw.GetPixel(x,y).a >= .35f) bottom = Mathf.Min(bottom,y);
            return (sprite.pivot.y - (bottom - sprite.rect.y)) / sprite.pixelsPerUnit;
        }
        finally { UnityEngine.Object.DestroyImmediate(raw); }
    }
    public static void Capture()
    {
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            throw new InvalidOperationException("Shadow/normal verification requires an actual graphics device");
        string output = Environment.GetEnvironmentVariable("TYPINGHERO_LIGHTING_CAPTURE") ?? "/results/lighting-capture";
        Directory.CreateDirectory(output);
        var setup = EditorSceneManager.GetSceneManagerSetup();
        var previousRT = RenderTexture.active;
        Material floorMaterial = null;
        RenderTexture target = null;
        Texture2D pixels = null;
        try
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var profile = AssetDatabase.LoadAssetAtPath<WorldLightingProfile>("Assets/Data/Lighting/DungeonLighting.asset");
            RenderSettings.ambientMode = AmbientMode.Flat; RenderSettings.ambientLight = Color.black;
            RenderSettings.fog = false;
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.localScale = Vector3.one * 3;
            floorMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            floorMaterial.SetColor("_BaseColor", new Color(.4f,.4f,.4f)); floorMaterial.SetFloat("_Smoothness", 0);
            floor.GetComponent<Renderer>().sharedMaterial = floorMaterial;
            floor.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            var actor = new GameObject("alpha silhouette", typeof(SpriteRenderer));
            var renderer = actor.GetComponent<SpriteRenderer>(); renderer.sharedMaterial = profile.actorMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided; renderer.receiveShadows = true;
            actor.transform.position = new Vector3(0,1.1f,0);
            var lamp = new GameObject("moving white key", typeof(Light)).GetComponent<Light>();
            lamp.type = LightType.Point; lamp.intensity = 24; lamp.range = 20; lamp.color = Color.white;
            lamp.shadows = LightShadows.Hard; lamp.shadowBias = .015f; lamp.shadowNormalBias = .02f;
            lamp.renderMode = LightRenderMode.ForcePixel;
            lamp.GetUniversalAdditionalLightData().usePipelineSettings = false;
            var view = new GameObject("offscreen Game camera", typeof(Camera), typeof(DungeonCaptureCamera)).GetComponent<Camera>();
            view.GetUniversalAdditionalCameraData().SetRenderer(profile.worldRendererIndex);
            view.orthographic = true; view.orthographicSize = 5.625f;
            view.transform.position = new Vector3(0,3,-10);
            view.transform.LookAt(new Vector3(0,1,0));
            view.clearFlags = CameraClearFlags.SolidColor; view.backgroundColor = new Color(.025f,.025f,.025f);
            view.allowHDR = true; view.allowMSAA = false; view.enabled = false;
            target = new RenderTexture(320,180,24,RenderTextureFormat.ARGB32) { filterMode = FilterMode.Point };
            target.Create(); view.targetTexture = target;
            pixels = new Texture2D(320,180,TextureFormat.RGB24,false);
            var cells = new List<Cell>();
            var feet = new Dictionary<string,float>();
            foreach (string path in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art" })
                .Select(AssetDatabase.GUIDToAssetPath).OrderBy(x => x))
            {
                if (!path.EndsWith(".png") || (!path.Contains("male_hero") && !path.ToLowerInvariant().Contains("skeleton"))) continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (!importer || !importer.secondarySpriteTextures.Any(x => x.name == "_NormalMap" && x.texture)) continue;
                string directory = Path.GetDirectoryName(path);
                if (!feet.ContainsKey(directory)) feet[directory] = IdleFoot(directory);
                actor.transform.position = new Vector3(0,feet[directory],0);
                foreach (var sprite in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(x => x.rect.x).ThenBy(x => x.rect.y))
                {
                    if (verticalOnly && (!sprite.name.ToLowerInvariant().Contains("idle") || sprite.rect.x != 0)) continue;
                    renderer.sprite = sprite;
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localId);
                    for (int mode = verticalOnly ? 4 : 0; mode < (verticalOnly ? 6 : 4); mode++)
                    {
                        renderer.flipX = mode >= 2 && mode < 4;
                        lamp.transform.position = mode >= 4 ? new Vector3(0, mode == 4 ? 7 : .4f, -3)
                            : new Vector3(mode % 2 == 0 ? -3 : 3, 4, -4);
                        // Initialize URP's shadow resources before accepting the first image.
                        if (cells.Count == 0) { view.Render(); view.Render(); }
                        view.Render(); RenderTexture.active = target;
                        pixels.ReadPixels(new Rect(0,0,320,180),0,0); pixels.Apply();
                        string file = "frame-" + cells.Count.ToString("D4") + ".png";
                        File.WriteAllBytes(Path.Combine(output,file),pixels.EncodeToPNG());
                        cells.Add(new Cell { source = path, sprite = sprite.name, mode = mode,
                            localId = localId, light = lamp.transform.position, file = file });
                    }
                }
            }
            File.WriteAllText(Path.Combine(output,"frames.json"),JsonUtility.ToJson(new CaptureRecord {
                device = SystemInfo.graphicsDeviceName, cells = cells.ToArray() },true));
            Debug.Log("PASS: rendered " + cells.Count + " alpha-shadow/normal diagnostics on " + SystemInfo.graphicsDeviceName);
        }
        finally
        {
            RenderTexture.active = previousRT;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (target) { target.Release(); UnityEngine.Object.DestroyImmediate(target); }
            if (pixels) UnityEngine.Object.DestroyImmediate(pixels);
            if (floorMaterial) UnityEngine.Object.DestroyImmediate(floorMaterial);
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
}
