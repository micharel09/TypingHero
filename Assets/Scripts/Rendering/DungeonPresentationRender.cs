using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class DungeonPresentationRender : ScriptableRendererFeature
{
    [SerializeField] Shader pointShader;
    Material material;
    PresentPass pass;
    public override void Create()
    {
        CoreUtils.Destroy(material);
        if (pointShader) material = CoreUtils.CreateEngineMaterial(pointShader);
        pass = new PresentPass(material);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (!material || !data.cameraData.camera.TryGetComponent<DungeonPresentationCamera>(out var screen) || !screen.Arena) return;
        pass.texture = screen.Arena.WorldTexture;
        if (pass.texture) renderer.EnqueuePass(pass);
    }
    protected override void Dispose(bool disposing) { CoreUtils.Destroy(material); }
    sealed class PresentPass : ScriptableRenderPass
    {
        readonly Material material;
        readonly MaterialPropertyBlock properties = new MaterialPropertyBlock();
        public RenderTexture texture;
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
            context.ExecuteCommandBuffer(cmd); CommandBufferPool.Release(cmd);
        }
    }
}
