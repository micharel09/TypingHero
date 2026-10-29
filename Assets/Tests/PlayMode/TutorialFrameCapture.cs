using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Optional, deterministic UGUI evidence for the same frame lookups exercised by TutorialTests.
// No production/QA bridge or gameplay mutation is involved.
internal sealed class TutorialFrameCapture : IDisposable
{
    readonly string output;
    readonly Canvas canvas;
    readonly Camera camera;
    readonly RenderTexture target;
    readonly Texture2D pixels;
    readonly Transform[] nodes;
    readonly int[] layers;

    internal static TutorialFrameCapture Open(TutorialOverlay overlay)
    {
        string path = Environment.GetEnvironmentVariable("TYPINGHERO_TUTORIAL_REVIEW");
        return string.IsNullOrEmpty(path) ? null : new TutorialFrameCapture(overlay, path);
    }

    TutorialFrameCapture(TutorialOverlay overlay, string path)
    {
        output = path;
        canvas = overlay.GetComponent<Canvas>();
        nodes = overlay.GetComponentsInChildren<Transform>(true);
        layers = Array.ConvertAll(nodes, node => node.gameObject.layer);
        camera = new GameObject("Tutorial evidence camera").AddComponent<Camera>();
        camera.enabled = false; camera.cullingMask = 1 << 31;
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.black;
        camera.allowHDR = camera.allowMSAA = false;
        target = new RenderTexture(640, 360, 24, RenderTextureFormat.ARGB32);
        target.Create(); camera.targetTexture = target;
        pixels = new Texture2D(640, 360, TextureFormat.RGB24, false);
    }

    internal void Save(int lesson, int frame, RectTransform clip)
    {
        var mode = canvas.renderMode; var view = canvas.worldCamera;
        var scaler = canvas.GetComponent<CanvasScaler>();
        bool enabled = scaler.enabled; float scale = canvas.scaleFactor;
        var active = RenderTexture.active;
        try
        {
            foreach (var node in nodes) node.gameObject.layer = 31;
            scaler.enabled = false; canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera; canvas.planeDistance = 1; canvas.scaleFactor = 2f / 3f;
            Canvas.ForceUpdateCanvases(); camera.Render();
            RenderTexture.active = target;
            pixels.ReadPixels(new Rect(0, 0, 640, 360), 0, 0);
            pixels.Apply();
            string folder = Path.Combine(output, lesson.ToString()); Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, frame.ToString("D4") + ".png"), pixels.EncodeToPNG());
        }
        finally
        {
            for (int i = 0; i < nodes.Length; i++) nodes[i].gameObject.layer = layers[i];
            canvas.renderMode = mode; canvas.worldCamera = view; canvas.scaleFactor = scale;
            scaler.enabled = enabled; RenderTexture.active = active; Canvas.ForceUpdateCanvases();
        }
    }

    public void Dispose()
    {
        camera.targetTexture = null; target.Release();
        UnityEngine.Object.DestroyImmediate(camera.gameObject);
        UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(pixels);
    }
}
