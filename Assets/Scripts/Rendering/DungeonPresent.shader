Shader "Hidden/CLACKBLADE/Dungeon Present"
{
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" }
  Pass {
   ZWrite Off ZTest Always Cull Off
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
   half4 Frag(Varyings input):SV_Target {
    return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_PointClamp, input.texcoord, 0);
   }
   ENDHLSL
  }
 }
}
