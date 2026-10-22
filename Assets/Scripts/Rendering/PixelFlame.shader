Shader "CLACKBLADE/Pixel Flame"
{
 Properties {
  _BaseMap("Flame atlas", 2D) = "white" {}
  _EmissionMap("Flame atlas", 2D) = "white" {}
  [HDR] _EmissionColor("Flame brightness", Color) = (1,1,1,1)
  _FlameRed("Brick red", Color) = (.5294,.2314,.1686,1)
  _FlameDarkOrange("Dark orange", Color) = (.7137,.3255,.1725,1)
  _FlameOrange("Warm orange", Color) = (.8941,.5451,.2275,1)
  _FlameYellow("Pale yellow", Color) = (.9569,.8,.4745,1)
  _BaseColor("Tint", Color) = (0,0,0,1)
  _Cutoff("Alpha cutoff", Range(0,1)) = .5
  _AlphaClip("Alpha clip", Float) = 1
  _AlphaToMask("Alpha to coverage", Float) = 1
  _Cull("Cull", Float) = 0
 }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
  Cull Off ZWrite On
  HLSLINCLUDE
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
  TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
  CBUFFER_START(UnityPerMaterial)
  half4 _EmissionColor;
  half4 _FlameRed, _FlameDarkOrange, _FlameOrange, _FlameYellow;
  half _Cutoff;
  CBUFFER_END
  struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
  struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
  Varyings Vert(Attributes v) {
   Varyings o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.uv=v.uv; return o;
  }
  half4 Sample(Varyings i) {
   half4 c=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv); clip(c.a-_Cutoff); return c;
  }
  ENDHLSL
  Pass {
   // Existing actor pass draws after environment blur, retaining the scene depth test.
   Name "Flame" Tags { "LightMode"="DungeonActor" }
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   half4 Frag(Varyings i):SV_Target {
    half4 c=Sample(i);
    // The source has four exact colors. Thresholds are in the project's linear color space.
    half3 palette = c.g < .03h ? _FlameRed.rgb : c.g < .12h ? _FlameDarkOrange.rgb :
        c.g < .3h ? _FlameOrange.rgb : _FlameYellow.rgb;
    return half4(palette*_EmissionColor.rgb,c.a);
   }
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
