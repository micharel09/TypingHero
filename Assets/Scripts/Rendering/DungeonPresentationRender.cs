using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class DungeonPresentationRender : ScriptableRendererFeature
{
    [SerializeField] Shader pointShader;
    Material material;
    PresentPass pass;
    LetterboxPass letterbox;
    RTHandle backbuffer;
    public override void Create()
    {
        CoreUtils.Destroy(material);
        backbuffer?.Release();
        backbuffer = RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget);
        if (pointShader) material = CoreUtils.CreateEngineMaterial(pointShader);
        pass = new PresentPass(material);
        letterbox = new LetterboxPass(material, backbuffer);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (!material || !data.cameraData.camera.TryGetComponent<DungeonPresentationCamera>(out var screen) || !screen.Arena) return;
        pass.arena = screen.Arena;
        pass.texture = screen.Arena.WorldTexture;
        if (!pass.texture) return;
        renderer.EnqueuePass(pass);
        var rect = screen.Arena.PresentationRect;
        if (rect.xMin > 0 || rect.yMin > 0 || rect.xMax < 1 || rect.yMax < 1)
        {
            letterbox.rect = rect;
            renderer.EnqueuePass(letterbox);
        }
    }
    protected override void Dispose(bool disposing) { CoreUtils.Destroy(material); backbuffer?.Release(); backbuffer = null; }
    // URP14 preserves the backbuffer outside a non-default camera viewport.
    // Mask those pixels after FinalBlit (+1), before overlay UI (+2, enqueued
    // after renderer features). Post-processing must not bloom into the bars.
    sealed class LetterboxPass : ScriptableRenderPass
    {
        readonly Material material;
        readonly RTHandle target;
        readonly MaterialPropertyBlock properties = new MaterialPropertyBlock();
        public Rect rect;
        public LetterboxPass(Material material, RTHandle target)
        {
            this.material = material; this.target = target;
            renderPassEvent = RenderPassEvent.AfterRendering + 2;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData data)
        {
            ConfigureTarget(target);
            ConfigureClear(ClearFlag.None, Color.black);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData data)
        {
            var cmd = CommandBufferPool.Get("Dungeon black letterbox");
            properties.SetVector("_LetterboxRect", new Vector4(rect.xMin, rect.yMin, rect.xMax, rect.yMax));
            properties.SetVector("_BlitScaleBias", new Vector4(1, 1, 0, 0));
            cmd.SetViewport(new Rect(0, 0, Screen.width, Screen.height));
            CoreUtils.DrawFullScreen(cmd, material, properties, 1);
            cmd.SetViewport(data.cameraData.camera.pixelRect);
            context.ExecuteCommandBuffer(cmd); CommandBufferPool.Release(cmd);
        }
    }
    sealed class PresentPass : ScriptableRenderPass
    {
        readonly Material material;
        readonly MaterialPropertyBlock properties = new MaterialPropertyBlock();
        public RenderTexture texture;
        public DungeonArenaCamera arena;
        readonly MaterialPropertyBlock flashProperties = new MaterialPropertyBlock();
        public PresentPass(Material material) { this.material = material; renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData data)
        {
            ConfigureTarget(data.cameraData.renderer.cameraColorTargetHandle, data.cameraData.renderer.cameraDepthTargetHandle);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData data)
        {
            var cmd = CommandBufferPool.Get("Dungeon Point presentation");
            // URP14's RenderTargetIdentifier blit overload retains the shared
            // property block's previous texture (often the color grading LUT).
            // Bind our owned texture on an independent block for this draw.
            properties.SetTexture("_BlitTexture", texture);
            properties.SetVector("_BlitScaleBias", new Vector4(1, 1, 0, 0));
            CoreUtils.DrawFullScreen(cmd, material, properties, 0);
            ImpactVfxBurst.DrawPresentation(cmd, arena, RenderingUtils.fullscreenMesh, flashProperties);
            context.ExecuteCommandBuffer(cmd); CommandBufferPool.Release(cmd);
        }
    }
}
