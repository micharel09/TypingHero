Shader "Hidden/CLACKBLADE/Depth Actor Diagnostic"
{
 Properties {
  [PerRendererData] _MainTex("Sprite",2D)="white"{}
  [PerRendererData] _DepthMap("Depth",2D)="black"{}
  [HideInInspector] _Flip("Flip",Vector)=(1,1,1,1)
 }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
  Cull Off ZWrite On
  Pass {
   Tags { "LightMode"="DungeonActor" }
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
   TEXTURE2D(_DepthMap); SAMPLER(sampler_DepthMap);
   float4 _DepthRect, _DepthTexel, _DepthFacing, _Flip, _ProbePosition;
   float _DepthScale, _SelfShadowBias, _SelfShadowStrength, _DepthReady, _DiagnosticMode;
   static const float _Cutoff=.35;
   #include "Assets/Scripts/Rendering/SpriteDepthVisibility.hlsl"
   struct Attributes { float4 position:POSITION; float2 uv:TEXCOORD0; };
   struct Varyings { float4 position:SV_POSITION; float2 uv:TEXCOORD0; float3 world:TEXCOORD1; };
   Varyings Vert(Attributes v) {
    Varyings o; v.position.xy*=_Flip.xy; o.world=TransformObjectToWorld(v.position.xyz);
    o.position=TransformWorldToHClip(o.world); o.uv=v.uv; return o;
   }
   float4 Frag(Varyings i):SV_Target {
    clip(SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).a-_Cutoff);
    if(_DepthReady<.5) return float4(1,0,1,1);
    float4 depth=SAMPLE_TEXTURE2D_LOD(_DepthMap,sampler_DepthMap,i.uv,0);
    float3 ray=normalize(_ProbePosition.xyz-i.world);
    if(_DiagnosticMode>2.5) {
     float3 local=mul((float3x3)GetWorldToObjectMatrix(),ray); local.xy*=_DepthFacing.xy;
     return float4(normalize(local)*.5+.5,1);
    }
    float value=_DiagnosticMode<1.5?depth.r:SelfVisibility(i.uv,ray,depth.r*_DepthScale);
    return float4(value,value,value,1);
   }
   ENDHLSL
  }
 }
}
