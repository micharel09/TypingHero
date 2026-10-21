Shader "CLACKBLADE/Dungeon Atmosphere"
{
    Properties
    {
        [HDR] _Tint("Tint", Color) = (1,1,1,1)
        _Haze("Haze shape", Float) = 0
        [HideInInspector] _DstBlend("Destination blend", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off ZWrite Off ZTest LEqual
        Blend SrcAlpha [_DstBlend]
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _Tint;
                half _Haze, _DstBlend;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS=TransformObjectToHClip(v.positionOS.xyz);
                o.uv=v.uv;
                return o;
            }
            half4 Frag(Varyings i):SV_Target
            {
                half across=pow(saturate(1-abs(i.uv.x*2-1)),1.6);
                half ends=smoothstep(0,.16,i.uv.y)*(1-smoothstep(.7,1,i.uv.y));
                half ellipse=pow(saturate(1-dot(i.uv*2-1,i.uv*2-1)),2);
                return half4(_Tint.rgb,_Tint.a*lerp(across*ends,ellipse,_Haze));
            }
            ENDHLSL
        }
    }
}
