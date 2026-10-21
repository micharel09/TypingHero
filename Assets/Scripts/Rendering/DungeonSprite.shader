Shader "CLACKBLADE/Dungeon Sprite"
{
 Properties {
  [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
  [PerRendererData] _NormalMap("Normal", 2D) = "bump" {}
  _Color("Tint", Color) = (1,1,1,1)
  _Cutoff("Alpha cutoff", Range(0,1)) = .35
  [HideInInspector] _RendererColor("Renderer color", Color) = (1,1,1,1)
  [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
  _FlashAmount("Flash", Range(0,1)) = 0
  _FlashColor("Flash color", Color) = (1,1,1,1)
 }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
  Cull Off ZWrite On
  HLSLINCLUDE
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
  TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
  TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
  CBUFFER_START(UnityPerMaterial)
  half4 _Color, _RendererColor, _FlashColor; float4 _Flip; half _Cutoff, _FlashAmount;
  CBUFFER_END
  struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };
  struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; float3 positionWS:TEXCOORD1; };
  Varyings Vert(Attributes v) {
   Varyings o; v.positionOS.xy *= _Flip.xy;
   o.positionWS = TransformObjectToWorld(v.positionOS.xyz); o.positionCS = TransformWorldToHClip(o.positionWS);
   o.uv=v.uv; o.color=v.color*_Color*_RendererColor; return o;
  }
  half4 Sample(Varyings i) { half4 c=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv)*i.color; clip(c.a-_Cutoff); return c; }
  ENDHLSL
  Pass {
   Name "ForwardLit" Tags { "LightMode"="DungeonActor" }
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
   #pragma multi_compile _ _ADDITIONAL_LIGHTS
   #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
   half4 Frag(Varyings i):SV_Target {
    half4 c=Sample(i); half3 n=UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,i.uv));
    n.xy *= _Flip.xy; n=normalize(TransformObjectToWorldDir(float3(n.xy,-n.z)));
    Light key=GetMainLight(TransformWorldToShadowCoord(i.positionWS));
    half3 illumination=SampleSH(n)+key.color*saturate(dot(n,key.direction))*key.distanceAttenuation*key.shadowAttenuation;
    uint count=GetAdditionalLightsCount();
    for(uint j=0;j<count;j++) { Light l=GetAdditionalLight(j,i.positionWS,half4(1,1,1,1)); illumination+=l.color*saturate(dot(n,l.direction))*l.distanceAttenuation*l.shadowAttenuation; }
    return half4(lerp(c.rgb*illumination,_FlashColor.rgb,_FlashAmount),c.a);
   }
   ENDHLSL
  }
  Pass {
   Name "ShadowCaster" Tags { "LightMode"="ShadowCaster" }
   ColorMask 0
   HLSLPROGRAM
   #pragma vertex ShadowVert
   #pragma fragment Shadow
   #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
   float3 _LightDirection, _LightPosition;
   Varyings ShadowVert(Attributes v) {
    Varyings o=Vert(v);
    float3 direction=_LightDirection;
    #if _CASTING_PUNCTUAL_LIGHT_SHADOW
    direction=normalize(_LightPosition-o.positionWS);
    #endif
    float3 normal=TransformObjectToWorldNormal(float3(0,0,-1));
    o.positionCS=TransformWorldToHClip(ApplyShadowBias(o.positionWS,normal,direction));
    #if UNITY_REVERSED_Z
    o.positionCS.z=min(o.positionCS.z,UNITY_NEAR_CLIP_VALUE);
    #else
    o.positionCS.z=max(o.positionCS.z,UNITY_NEAR_CLIP_VALUE);
    #endif
    return o;
   }
   half4 Shadow(Varyings i):SV_Target { Sample(i); return 0; }
   ENDHLSL
  }
  Pass {
   Name "DepthOnly" Tags { "LightMode"="DepthOnly" }
   ColorMask 0
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Depth
   half4 Depth(Varyings i):SV_Target { Sample(i); return 0; }
   ENDHLSL
  }
 }
}
