Shader "TypingHero/NormalPlayerSlash"
{
    Properties { _Tint ("Trail multipliers", Vector) = (.75,.75,.75,.55) _Thermal ("Particle temperature stream", Float) = 0 _Emission ("Mesh glow", Float) = .08 }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            half4 _Tint;
            float _Thermal, _Emission;
            float _VoxelWorld3D;
            float3 _VoxelKeyPosition, _VoxelFillPosition, _VoxelPulsePosition;
            half4 _VoxelKeyColor, _VoxelFillColor, _VoxelAmbient, _VoxelPulseColor;
            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; half4 color : COLOR; float4 shape : TEXCOORD1; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 normalWS : TEXCOORD0; half4 color : COLOR; float3 positionWS : TEXCOORD1; float heat : TEXCOORD2; };
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.color = v.color * _Tint;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.heat = _Thermal > .5 ? saturate(v.shape.x) : _Emission;
                return o;
            }
            half4 frag(Varyings i) : SV_Target
            {
                // Hard face normals retain the cube's volume, including while it is hot.
                float3 n = normalize(i.normalWS);
                float key = floor(saturate(dot(n, normalize(_VoxelKeyPosition - i.positionWS))) * 3 + .5) / 3;
                float fill = floor(saturate(dot(n, normalize(_VoxelFillPosition - i.positionWS))) * 3 + .5) / 3;
                float3 pulse = _VoxelPulsePosition - i.positionWS;
                float bounce = saturate(dot(n, normalize(pulse))) * saturate(1 - length(pulse) / 4);
                half3 lighting = .18 + _VoxelAmbient.rgb * .3 + _VoxelKeyColor.rgb * key * .85 +
                    _VoxelFillColor.rgb * fill * .5 + _VoxelPulseColor.rgb * bounce * .12;
                if (_VoxelWorld3D > .5)
                {
                    Light main = GetMainLight(TransformWorldToShadowCoord(i.positionWS));
                    float face = floor(saturate(dot(n, main.direction)) * 3 + .5) / 3;
                    lighting = .12 + SampleSH(n) + main.color * face * main.distanceAttenuation * main.shadowAttenuation;
                    uint count = GetAdditionalLightsCount();
                    for (uint j = 0; j < count; j++)
                    {
                        Light light = GetAdditionalLight(j, i.positionWS);
                        face = floor(saturate(dot(n, light.direction)) * 3 + .5) / 3;
                        lighting += light.color * face * light.distanceAttenuation * light.shadowAttenuation;
                    }
                }
                return half4(i.color.rgb * lighting * lerp(1, 2.1, i.heat) + i.color.rgb * i.heat * .12, i.color.a);
            }
            ENDHLSL
        }
    }
}
