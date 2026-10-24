// Lantern-only URP14 metallic surface contract: no normal/detail/metallic maps.
#ifndef TYPINGHERO_LANTERN_INPUT_INCLUDED
#define TYPINGHERO_LANTERN_INPUT_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor, _SpecColor, _EmissionColor;
half _Metallic, _Smoothness, _Cutoff, _Surface;
half _AlphaClip, _ReceiveShadows, _SpecularHighlights, _EnvironmentReflections;
CBUFFER_END

half LanternAlpha(half textureAlpha, half4 color, half cutoff)
{
    half alpha = textureAlpha * color.a;
    if (_AlphaClip > .5h) clip(alpha - cutoff);
    return alpha;
}

void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData surfaceData)
{
    half4 albedoSample = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    surfaceData = (SurfaceData)0;
    surfaceData.alpha = LanternAlpha(albedoSample.a, _BaseColor, _Cutoff);
    surfaceData.albedo = albedoSample.rgb * _BaseColor.rgb;
    surfaceData.metallic = _Metallic;
    surfaceData.smoothness = _Smoothness;
    surfaceData.normalTS = half3(0, 0, 1);
    surfaceData.occlusion = 1;
    surfaceData.emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb * _EmissionColor.rgb;
    surfaceData.clearCoatSmoothness = 1;
}
#endif
