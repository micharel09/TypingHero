Shader "Hidden/CLACKBLADE/Depth Diagnostic"
{
 Properties { _DepthMap("Depth",2D)="black"{} }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" }
  Cull Off ZWrite Off ZTest Always
  Pass {
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   TEXTURE2D(_DepthMap); SAMPLER(sampler_DepthMap);
   float4 _DepthRect, _DepthTexel, _DepthFacing, _TestLight;
   float _DepthScale, _SelfShadowBias, _SelfShadowStrength, _Cutoff;
   #include "Assets/Scripts/Rendering/SpriteDepthVisibility.hlsl"
   struct Attributes { float4 position:POSITION; float2 uv:TEXCOORD0; };
   struct Varyings { float4 position:SV_POSITION; float2 uv:TEXCOORD0; };
   Varyings Vert(Attributes v) {
    Varyings o; o.position=TransformObjectToHClip(v.position.xyz); o.uv=v.uv; return o;
   }
   float4 Frag(Varyings i):SV_Target {
    float4 depth=SAMPLE_TEXTURE2D_LOD(_DepthMap,sampler_DepthMap,i.uv,0);
    float visibility=depth.a < _Cutoff ? 1 : SelfVisibility(i.uv,_TestLight.xyz,depth.r*_DepthScale);
    return float4(visibility,depth.r,i.uv.y,1);
   }
   ENDHLSL
  }
 }
}
