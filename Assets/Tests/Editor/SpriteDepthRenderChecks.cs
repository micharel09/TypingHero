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
public static class SpriteDepthRenderChecks
{
    static bool diagnostics;
    static bool allFrames;
    public static void CaptureAll()
    {
        allFrames = true;
        try { Capture(); }
        finally { allFrames = false; }
    }
    public static void Investigate()
    {
        SpriteDepthGpuChecks.Verify();
        SpriteDepthLightChecks.Verify();
        diagnostics=true;
        try { Capture(); }
        finally { diagnostics=false; }
    }
    public static void Inventory()
    {
        SpriteDepthAuthoring.ExportInventory("/results/depth-inventory.json");
        Debug.Log("PASS: exported build-referenced sprite keys with original timing");
    }
    [Serializable] sealed class Cell
    {
        public string source, sprite, file;
        public int mode, facing;
        public long localId;
        public Vector3 light;
    }
    [Serializable] sealed class CaptureRecord { public string device; public Cell[] cells; }
    public static void Capture()
    {
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            throw new InvalidOperationException("Shadow/normal verification requires an actual graphics device");
        string output = Environment.GetEnvironmentVariable("TYPINGHERO_LIGHTING_CAPTURE") ?? "/results/lighting-capture";
        Directory.CreateDirectory(output);
        SpriteDepthAuthoring.ExportInventory(Path.Combine(output, "referenced-frames.json"));
        var setup = EditorSceneManager.GetSceneManagerSetup();
        var previousRT = RenderTexture.active;
        Material floorMaterial = null, material = null, diagnostic = null;
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
            material = new Material(profile.actorMaterial);
            if (diagnostics) diagnostic = new Material(AssetDatabase.LoadAssetAtPath<Shader>(
                "Assets/Tests/Fixtures/Lighting/SelfShadow/DepthActorDiagnostic.shader"));
            renderer.sharedMaterial = material;
            var binding = actor.AddComponent<SpriteDepthBinding>();
            var sources = new[] {
                "Assets/Art/male_hero/individual_sheets/male_hero-combo_3.png",
                "Assets/Art/EndlessSkeletons/sword/Attack1.png"
            };
            var authored = AssetDatabase.LoadAssetAtPath<SpriteDepthLibrary>("Assets/Data/Lighting/ActorDepth.asset");
            if (allFrames) SpriteDepthAuthoring.Validate(authored);
            for (int actorIndex = 0; actorIndex < (allFrames ? authored.frames.Length : sources.Length); actorIndex++)
            {
                string path = allFrames ? AssetDatabase.GetAssetPath(authored.frames[actorIndex].sprite) : sources[actorIndex];
                var sprite = allFrames ? authored.frames[actorIndex].sprite : AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>()
                    .OrderBy(x => x.rect.x).ElementAt(actorIndex == 0 ? 1 : 2);
                var depth = allFrames ? authored.frames[actorIndex].depth : AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Tests/Fixtures/Lighting/SelfShadow/" + (actorIndex == 0 ? "hero" : "sword") + ".png");
                var library = ScriptableObject.CreateInstance<SpriteDepthLibrary>();
                library.frames = new[] { SpriteDepthAuthoring.Describe(sprite, depth) };
                renderer.sprite = sprite; binding.Bind(library.CreateLookup());
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localId);
                // Keep the source foot on the receiver: hero bottom is48 pixels
                // above its cel bottom; sword Attack1 reaches the cel bottom.
                actor.transform.position = new Vector3(0, (sprite.pivot.y - (path.Contains("male_hero") ? 48 : 0)) / sprite.pixelsPerUnit, 0);
                for (int facing=0; facing<2; facing++)
                for (int direction=0; direction<4; direction++)
                for (int mode=0; mode<3; mode++)
                {
                    renderer.flipX = facing != 0; binding.Refresh(true);
                    material.SetFloat("_NormalStrength", mode == 0 ? 0 : profile.actorMaterial.GetFloat("_NormalStrength"));
                    material.SetFloat("_SelfShadowStrength", mode == 2 ? .65f : 0);
                    lamp.transform.position = actor.transform.position + (direction < 2
                        ? new Vector3(direction == 0 ? -3 : 3,0,-2)
                        : new Vector3(0,direction == 2 ? 3 : -3,-2));
                    if (cells.Count == 0) { view.Render(); view.Render(); }
                    view.Render(); RenderTexture.active = target;
                    pixels.ReadPixels(new Rect(0,0,320,180),0,0); pixels.Apply();
                    string file = "frame-" + cells.Count.ToString("D4") + ".png";
                    File.WriteAllBytes(Path.Combine(output,file),pixels.EncodeToPNG());
                    cells.Add(new Cell { source = path, sprite = sprite.name, mode = mode,
                        localId = localId, facing = facing, light = lamp.transform.position, file = file });
                    if (diagnostics && mode==2)
                    {
                        diagnostic.SetFloat("_SelfShadowStrength",.65f);
                        diagnostic.SetFloat("_DepthScale",.15f); diagnostic.SetFloat("_SelfShadowBias",.01f);
                        diagnostic.SetVector("_ProbePosition",lamp.transform.position);
                        renderer.sharedMaterial=diagnostic; binding.Refresh(true);
                        for(int debugMode=1;debugMode<=3;debugMode++)
                        {
                            diagnostic.SetFloat("_DiagnosticMode",debugMode);
                            view.Render(); RenderTexture.active=target;
                            pixels.ReadPixels(new Rect(0,0,320,180),0,0); pixels.Apply();
                            File.WriteAllBytes(Path.Combine(output,$"diagnostic-{actorIndex}-{facing}-{direction}-{debugMode}.png"),pixels.EncodeToPNG());
                        }
                        renderer.sharedMaterial=material; binding.Refresh(true);
                    }
                }
                UnityEngine.Object.DestroyImmediate(library);
            }
            if (ShaderUtil.ShaderHasError(material.shader))
                throw new InvalidOperationException("Depth shader compilation failed; images are not acceptance");
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
            if (material) UnityEngine.Object.DestroyImmediate(material);
            if (diagnostic) UnityEngine.Object.DestroyImmediate(diagnostic);
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
}
