Shader "TypingHero/ParryVoxel"
{
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; half4 color : COLOR; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 normalWS : TEXCOORD0; half4 color : COLOR; };
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.color = v.color;
                return o;
            }
            half4 frag(Varyings i) : SV_Target
            {
                // Hard face normals retain the cube's volume, including while it is hot.
                float light = saturate(dot(normalize(i.normalWS), normalize(float3(-.4, .7, -.6))));
                float face = .24 + .76 * floor(light * 3 + .5) / 3;
                float heat = saturate((i.color.g - .25) / .65);
                return half4(i.color.rgb * face * lerp(1, 2.1, heat), i.color.a);
            }
            ENDHLSL
        }
    }
}
