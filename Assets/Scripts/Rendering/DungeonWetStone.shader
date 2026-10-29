Shader "CLACKBLADE/Dungeon Wet Stone"
{
 Properties { _ReflectionTex("Planar capture", 2D) = "black" {} _Strength("Wetness", Range(0,1)) = .2 }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-20" "RenderType"="Transparent" }
  Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
  Pass {
   Tags { "LightMode"="SRPDefaultUnlit" }
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   float _DungeonSlayerWeight;
   TEXTURE2D(_ReflectionTex); SAMPLER(sampler_ReflectionTex);
   float4x4 _ReflectionVP; half _Strength;
   struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
   struct V { float4 positionCS:SV_POSITION; float4 reflected:TEXCOORD0; float2 uv:TEXCOORD1; float3 world:TEXCOORD2; };
   V Vert(A v) { V o; o.world=TransformObjectToWorld(v.positionOS.xyz); o.positionCS=TransformWorldToHClip(o.world); o.reflected=mul(_ReflectionVP,float4(o.world,1)); o.uv=v.uv; return o; }
   half4 Frag(V i):SV_Target {
    float2 uv=i.reflected.xy/i.reflected.w*.5+.5;
    half edge=saturate((1-length(i.uv*2-1))*5);
    half patches=smoothstep(-.5,.6,sin(i.world.x*2.1+sin(i.world.z*1.3))*cos(i.world.z*1.7));
    half4 c=SAMPLE_TEXTURE2D(_ReflectionTex,sampler_ReflectionTex,uv);
    return half4(c.rgb,_Strength*edge*patches);
   }
   ENDHLSL
  }
 }
}
