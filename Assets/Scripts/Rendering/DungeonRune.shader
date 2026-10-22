Shader "CLACKBLADE/Dungeon Rune"
{
 Properties { [HDR] _Tint("Emission", Color) = (.2,.7,1,1) }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
  ZWrite Off Cull Off Blend SrcAlpha One
  Pass {
   Tags { "LightMode"="SRPDefaultUnlit" }
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   half4 _Tint; float _DungeonAge, _DungeonPulse;
   struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
   struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
   V Vert(A v) { V o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.uv=v.uv; return o; }
   half4 Frag(V i):SV_Target {
    float2 p=i.uv*2-1;
    half ring=1-smoothstep(.025,.055,abs(length(p)-.65));
    half cross=(1-smoothstep(.025,.05,abs(p.x)))*step(abs(p.y),.85);
    half slash=(1-smoothstep(.025,.05,abs(abs(p.x)-abs(p.y))))*step(length(p),.8);
    half scan=.75+.25*sin((i.uv.y+_DungeonAge*.06)*25);
    return half4(_Tint.rgb*_DungeonPulse,saturate(ring+cross+slash)*scan*_Tint.a);
   }
   ENDHLSL
  }
 }
}
