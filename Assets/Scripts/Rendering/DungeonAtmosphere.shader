Shader "CLACKBLADE/Dungeon Atmosphere"
{
    Properties
    {
        [HDR] _Tint("Tint", Color) = (1,1,1,1)
        _Taper("Taper", Float) = 1.6
        _FadeIn("Fade in end", Float) = .16
        _FadeOut("Fade out start", Float) = .7
        _EllipsePower("Ellipse power", Float) = 2
        _EllipseScale("Ellipse scale XY", Vector) = (1,1,0,0)
        _DriftBase("Drift base", Float) = .82
        _DriftAmplitude("Drift amplitude", Float) = .18
        _DriftFrequency("Drift frequency XY", Vector) = (19,11,0,0)
        _DriftSpeed("Drift speed", Float) = .4
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
   float _DungeonSlayerWeight;
            CBUFFER_START(UnityPerMaterial)
                half4 _Tint;
                half _Haze, _DstBlend;
                float _Taper, _FadeIn, _FadeOut, _EllipsePower, _DriftBase, _DriftAmplitude, _DriftSpeed;
                float4 _EllipseScale, _DriftFrequency;
            CBUFFER_END
            float _DungeonAge, _DungeonPulse;
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
                half across=pow(saturate(1-abs(i.uv.x*2-1)),_Taper);
                half ends=smoothstep(0,_FadeIn,i.uv.y)*(1-smoothstep(_FadeOut,1,i.uv.y));
                half ellipse=pow(saturate(1-dot((i.uv*2-1)*_EllipseScale.xy,(i.uv*2-1)*_EllipseScale.xy)),_EllipsePower);
                half drift=_DriftBase+_DriftAmplitude*sin(dot(i.uv,_DriftFrequency.xy)+_DungeonAge*_DriftSpeed);
                return half4(_Tint.rgb,(1-_DungeonSlayerWeight)*_Tint.a*lerp(across*ends,ellipse,_Haze)*drift);
            }
            ENDHLSL
        }
    }
}
