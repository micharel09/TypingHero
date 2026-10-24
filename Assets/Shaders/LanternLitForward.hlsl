// Same URP14 BRDF/GI/light loop as Lit; material switches replace local variants.
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"

half4 LanternPBR(InputData inputData, SurfaceData surfaceData)
{
    BRDFData brdfData;
    InitializeBRDFData(surfaceData, brdfData);
    BRDFData clearCoat = (BRDFData)0;
    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor ao = CreateAmbientOcclusionFactor(inputData, surfaceData);
    Light mainLight = GetMainLight(inputData, shadowMask, ao);
    if (_ReceiveShadows < .5h) mainLight.shadowAttenuation = 1;
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    LightingData lighting = CreateLightingData(inputData, surfaceData);

    half3 reflectVector = reflect(-inputData.viewDirectionWS, inputData.normalWS);
    half fresnel = Pow4(1 - saturate(dot(inputData.normalWS, inputData.viewDirectionWS)));
    half3 indirectSpecular = _GlossyEnvironmentColor.rgb;
    if (_EnvironmentReflections > .5h)
        indirectSpecular = GlossyEnvironmentReflection(reflectVector, inputData.positionWS,
            brdfData.perceptualRoughness, 1, inputData.normalizedScreenSpaceUV);
    lighting.giColor = EnvironmentBRDF(brdfData, inputData.bakedGI, indirectSpecular, fresnel)
        * ao.indirectAmbientOcclusion;

    bool specularOff = _SpecularHighlights < .5h;
    lighting.mainLightColor = LightingPhysicallyBased(brdfData, clearCoat, mainLight,
        inputData.normalWS, inputData.viewDirectionWS, 0, specularOff);
#if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();
    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, ao);
        if (_ReceiveShadows < .5h) light.shadowAttenuation = 1;
        lighting.additionalLightsColor += LightingPhysicallyBased(brdfData, clearCoat, light,
            inputData.normalWS, inputData.viewDirectionWS, 0, specularOff);
    LIGHT_LOOP_END
#endif
#if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lighting.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
#endif
#if REAL_IS_HALF
    return min(CalculateFinalColor(lighting, surfaceData.alpha), HALF_MAX);
#else
    return CalculateFinalColor(lighting, surfaceData.alpha);
#endif
}

half4 LanternFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    SurfaceData surface;
    InitializeStandardLitSurfaceData(input.uv, surface);
    InputData data;
    InitializeInputData(input, surface.normalTS, data);
    half4 color = LanternPBR(data, surface);
    color.rgb = MixFog(color.rgb, data.fogCoord);
    color.a = OutputAlpha(color.a, IsSurfaceTypeTransparent(_Surface));
    return color;
}
