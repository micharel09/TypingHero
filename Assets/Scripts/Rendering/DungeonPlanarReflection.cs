using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// A single horizontal wet-stone plane. Its receiver is excluded during capture.
[DisallowMultipleComponent]
public sealed class DungeonPlanarReflection : MonoBehaviour
{
    [SerializeField] Renderer receiver;
    Camera reflected;
    RenderTexture texture;
    MaterialPropertyBlock block;
    static bool rendering;

    void OnEnable() { RenderPipelineManager.beginCameraRendering += Render; }
    void Render(ScriptableRenderContext context, Camera source)
    {
        if (rendering || !receiver || !receiver.enabled || source.cameraType != CameraType.Game
            || !source.TryGetComponent<DungeonArenaCamera>(out var arena) || arena.Cinematic
            || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) return;
        if (!reflected)
        {
            var node = new GameObject("Dungeon planar capture") { hideFlags = HideFlags.HideAndDontSave };
            node.transform.SetParent(transform, false);
            reflected = node.AddComponent<Camera>(); reflected.enabled = false;
            node.AddComponent<DungeonCaptureCamera>();
        }
        int width = Mathf.Max(1, source.pixelWidth / 2), height = Mathf.Max(1, source.pixelHeight / 2);
        if (!texture || texture.width != width || texture.height != height)
        {
            ReleaseTexture();
            texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBHalf)
            { name = "Dungeon wet stone reflection", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
            texture.Create();
        }
        reflected.CopyFrom(source);
        reflected.enabled = false; reflected.cameraType = CameraType.Game;
        reflected.targetTexture = texture; reflected.rect = new Rect(0, 0, 1, 1);
        reflected.allowMSAA = false;
        var data = reflected.GetUniversalAdditionalCameraData(); data.SetRenderer(1);
        data.renderPostProcessing = false; data.renderShadows = false;
        data.requiresColorOption = data.requiresDepthOption = CameraOverrideOption.Off;
        float planeY = transform.position.y;
        var mirror = Matrix4x4.identity; mirror.m11 = -1; mirror.m13 = 2 * planeY;
        reflected.worldToCameraMatrix = source.worldToCameraMatrix * mirror;
        reflected.transform.position = mirror.MultiplyPoint(source.transform.position);
        reflected.transform.rotation = Quaternion.LookRotation(mirror.MultiplyVector(source.transform.forward), mirror.MultiplyVector(source.transform.up));
        Vector3 point = reflected.worldToCameraMatrix.MultiplyPoint(new Vector3(0, planeY + .02f, 0));
        Vector3 normal = reflected.worldToCameraMatrix.MultiplyVector(Vector3.up).normalized;
        reflected.projectionMatrix = source.CalculateObliqueMatrix(new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(point, normal)));
        if (block == null) block = new MaterialPropertyBlock();
        receiver.GetPropertyBlock(block);
        block.SetTexture("_ReflectionTex", texture);
        block.SetMatrix("_ReflectionVP", GL.GetGPUProjectionMatrix(reflected.projectionMatrix, true) * reflected.worldToCameraMatrix);
        receiver.SetPropertyBlock(block);
        bool previousCulling = GL.invertCulling, previousHidden = receiver.forceRenderingOff;
        try
        {
            rendering = true; receiver.forceRenderingOff = true; GL.invertCulling = !previousCulling;
            UniversalRenderPipeline.RenderSingleCamera(context, reflected);
        }
        finally { GL.invertCulling = previousCulling; receiver.forceRenderingOff = previousHidden; rendering = false; }
    }
    void ReleaseTexture()
    {
        if (!texture) return;
        if (reflected) reflected.targetTexture = null;
        texture.Release(); Destroy(texture); texture = null;
    }
    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= Render;
        ReleaseTexture();
        if (reflected) { Destroy(reflected.gameObject); reflected = null; }
    }
}
