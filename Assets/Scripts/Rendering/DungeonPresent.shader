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
  Pass {
   ZWrite Off ZTest Always Cull Off
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Letterbox
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
   float4 _LetterboxRect;
   half4 Letterbox(Varyings input):SV_Target {
    float2 uv = input.texcoord;
    #if UNITY_UV_STARTS_AT_TOP
     uv.y = 1 - uv.y;
    #endif
    bool outside = any(uv < _LetterboxRect.xy) || any(uv >= _LetterboxRect.zw);
    clip(outside ? 1 : -1);
    return half4(0, 0, 0, 1);
   }
   ENDHLSL
  }
 }
}
