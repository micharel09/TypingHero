using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class DungeonWorldRender : ScriptableRendererFeature
{
    [SerializeField] Shader blurShader;
    Material blur;
    EnvironmentPass environment;
    ActorPass actors;
    public override void Create()
    {
        CoreUtils.Destroy(blur);
        if (blurShader) blur = CoreUtils.CreateEngineMaterial(blurShader);
        environment = new EnvironmentPass(blur);
        actors = new ActorPass();
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (data.cameraData.camera.TryGetComponent<DungeonCaptureCamera>(out _))
        {
            renderer.EnqueuePass(actors);
            return;
        }
        if (!data.cameraData.camera.TryGetComponent<DungeonArenaCamera>(out var arena) || !arena.WorldTexture) return;
        if (blur && arena.profile.blurRadius > 0 && !arena.Cinematic)
        {
            environment.Configure(arena, arena.profile.blurRadius);
            renderer.EnqueuePass(environment);
        }
        renderer.EnqueuePass(actors);
    }
    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(blur);
    }
    sealed class EnvironmentPass : ScriptableRenderPass
    {
        static readonly int FocusId = Shader.PropertyToID("_DungeonFocus"), EdgeId = Shader.PropertyToID("_DungeonDepthEdgeDistance"), TexelId = Shader.PropertyToID("_BlitTexture_TexelSize");
        readonly Material material;
        DungeonArenaCamera arena;
        float focus, radius;
        public EnvironmentPass(Material material)
        {
            this.material = material;
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }
        public void Configure(DungeonArenaCamera arena, float radius) { this.arena = arena; focus = arena.FocusDepth; this.radius = radius; }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData data)
        {
            var descriptor = data.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0; descriptor.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref arena.EnvironmentScratch, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "Dungeon environment blur");
            ConfigureTarget(data.cameraData.renderer.cameraColorTargetHandle, data.cameraData.renderer.cameraDepthTargetHandle);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData data)
        {
            var color = data.cameraData.renderer.cameraColorTargetHandle;
            var scratch = arena.EnvironmentScratch;
            var cmd = CommandBufferPool.Get("Dungeon environment depth blur");
            material.SetVector(FocusId, new Vector4(focus + arena.profile.focusOffset, arena.profile.focusFalloff, radius, arena.profile.focusHalfWidth));
            material.SetFloat(EdgeId, arena.profile.depthEdgeDistance);
            material.SetVector(TexelId, new Vector4(1f / color.rt.width, 1f / color.rt.height, color.rt.width, color.rt.height));
            Blitter.BlitCameraTexture(cmd, color, scratch, material, 0);
            Blitter.BlitCameraTexture(cmd, scratch, color, material, 1);
            context.ExecuteCommandBuffer(cmd); CommandBufferPool.Release(cmd);
        }
    }
    sealed class ActorPass : ScriptableRenderPass
    {
        static readonly ShaderTagId Tag = new ShaderTagId("DungeonActor");
        public ActorPass() { renderPassEvent = RenderPassEvent.BeforeRenderingTransparents; }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData data)
        {
            ConfigureTarget(data.cameraData.renderer.cameraColorTargetHandle, data.cameraData.renderer.cameraDepthTargetHandle);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData data)
        {
            // Existing SpriteRenderer sorting and alpha/depth remain authoritative.
            var draw = CreateDrawingSettings(Tag, ref data, SortingCriteria.CommonTransparent);
            draw.perObjectData |= PerObjectData.LightProbe | PerObjectData.LightIndices | PerObjectData.LightData;
            var filter = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(data.cullResults, ref draw, ref filter);
        }
    }
}
