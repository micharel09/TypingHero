Shader "Hidden/CLACKBLADE/Dungeon Depth Blur"
{
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" }
  ZWrite Off ZTest Always Cull Off
  HLSLINCLUDE
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
  #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
  float4 _DungeonFocus;
  float4 _BlitTexture_TexelSize;
  float EyeDepth(float2 uv) {
   float raw = SampleSceneDepth(uv);
   if (unity_OrthoParams.w > .5) {
    #if UNITY_REVERSED_Z
    raw = 1 - raw;
    #endif
    return lerp(_ProjectionParams.y, _ProjectionParams.z, raw);
   }
   return LinearEyeDepth(raw, _ZBufferParams);
  }
  half4 Blur(float2 uv, float2 axis) {
   float depth = EyeDepth(uv);
   float amount = saturate((abs(depth - (_DungeonFocus.x - .75)) - .75) / _DungeonFocus.y) * _DungeonFocus.z;
   // Five taps span at most radius world pixels, rather than twice the radius.
   float2 delta = axis * _BlitTexture_TexelSize.xy * amount * .5;
   half4 color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, 0) * .40262;
   float total = .40262;
   [unroll] for (int i = -2; i <= 2; i++) {
    if (i == 0) continue;
    float2 sampleUV = uv + delta * i;
    float weight = (abs(i) == 1 ? .24420 : .05449) * saturate(1 - abs(EyeDepth(sampleUV) - depth) * 2);
    color += SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, sampleUV, 0) * weight;
    total += weight;
   }
   return color / total;
  }
  half4 Horizontal(Varyings i):SV_Target { return Blur(i.texcoord, float2(1,0)); }
  half4 Vertical(Varyings i):SV_Target { return Blur(i.texcoord, float2(0,1)); }
  ENDHLSL
  Pass {
   Name "Horizontal"
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Horizontal
   ENDHLSL
  }
  Pass {
   Name "Vertical"
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Vertical
   ENDHLSL
  }
 }
}
