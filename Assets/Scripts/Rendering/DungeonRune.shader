Shader "CLACKBLADE/Dungeon Rune"
{
 Properties {
  [HDR] _Tint("Emission", Color) = (.2,.7,1,1)
  _RingRadius("Ring radius", Float) = .65
  _StrokeWidth("Stroke half width", Float) = .025
  _StrokeSoftness("Stroke softness", Float) = .025
  _RingSoftness("Ring softness", Float) = .03
  _CrossLength("Cross half length", Float) = .85
  _SlashRadius("Slash radius", Float) = .8
  _ScanBase("Scan base", Float) = .75
  _ScanAmplitude("Scan amplitude", Float) = .25
  _ScanSpeed("Scan speed", Float) = .06
  _ScanFrequency("Scan frequency", Float) = 25
 }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
  ZWrite Off Cull Off Blend SrcAlpha One
  Pass {
   Tags { "LightMode"="SRPDefaultUnlit" }
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   float _DungeonSlayerWeight;
   half4 _Tint; float _DungeonAge, _DungeonPulse;
   float _RingRadius, _StrokeWidth, _StrokeSoftness, _RingSoftness, _CrossLength, _SlashRadius;
   float _ScanBase, _ScanAmplitude, _ScanSpeed, _ScanFrequency;
   struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
   struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
   V Vert(A v) { V o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.uv=v.uv; return o; }
   half4 Frag(V i):SV_Target {
    float2 p=i.uv*2-1;
    half ring=1-smoothstep(_StrokeWidth,_StrokeWidth+_RingSoftness,abs(length(p)-_RingRadius));
    half cross=(1-smoothstep(_StrokeWidth,_StrokeWidth+_StrokeSoftness,abs(p.x)))*step(abs(p.y),_CrossLength);
    half slash=(1-smoothstep(_StrokeWidth,_StrokeWidth+_StrokeSoftness,abs(abs(p.x)-abs(p.y))))*step(length(p),_SlashRadius);
    half scan=_ScanBase+_ScanAmplitude*sin((i.uv.y+_DungeonAge*_ScanSpeed)*_ScanFrequency);
    return half4(_Tint.rgb*_DungeonPulse,saturate(ring+cross+slash)*scan*_Tint.a);
   }
   ENDHLSL
  }
 }
}
