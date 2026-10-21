Shader "TypingHero/ParryClash"
{
    Properties { _Role ("Role: contact / ray / ember", Float) = 0 _Pixel ("Source pixel grid", Float) = 0 }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; float4 shape : TEXCOORD1; };
            struct Varyings { float4 positionCS : SV_POSITION; half4 color : COLOR; float2 uv : TEXCOORD0; float4 shape : TEXCOORD1; };
            Varyings vert(Attributes v) { Varyings o; o.positionCS = TransformObjectToHClip(v.positionOS.xyz); o.color=v.color; o.uv=v.uv; o.shape=v.shape; return o; }
            float _Role, _Pixel;
            half4 frag(Varyings i) : SV_Target
            {
                float2 p = abs(i.uv * 2 - 1);
                float core = saturate(1 - p.x * p.x) * saturate(1 - p.y * p.y);
                if (_Role < .5 && _Pixel > .5)
                {
                    float grid = max(1, i.shape.z);
                    float2 q = ((floor(saturate(i.uv) * grid) + .5) / grid * 2 - 1);
                    float radius = length(q);
                    float mask = i.shape.x < .5 ? step(radius, .9) : step(radius, .98) * step(.62, radius);
                    return half4(i.color.rgb * (i.shape.x < .5 ? 3 : 1.5), i.color.a * mask);
                }
                if (_Role < .5) return half4(i.color.rgb * 3, i.color.a * core * core);
                if (_Pixel > .5)
                {
                    float pixels = max(1, i.shape.z), length = max(1, i.shape.w);
                    float x = min(pixels - 1, floor(saturate(i.uv.x) * pixels));
                    float row = min(length - 1, floor(saturate(1 - i.uv.y) * length));
                    float width = row < ceil(length * .25) ? pixels : (row < ceil(length * .75) ? round(pixels * i.shape.y) : 1);
                    if (i.shape.x < 1.5 && row == 0) width = max(1, pixels - 1);
                    float left = floor((pixels - width) * .5);
                    if (i.shape.x > .5 && i.shape.x < 1.5 && row < 2) left = min(pixels - width, left + 1);
                    float mask = step(left, x) * (1 - step(left + width, x));
                    if (length > 1 && i.shape.x > 1.5 && row == length - 1) mask = 0;
                    return half4(i.color.rgb * (pixels > 1 && row < ceil(length * .25) ? 2.4 : 1), i.color.a * mask);
                }
                // Quantize the local silhouette only; world motion and depth stay continuous.
                float2 uv = (floor(saturate(i.uv) * float2(8, 16)) + .5) / float2(8, 16);
                if (_Role > 1.5)
                    return half4(i.color.rgb, i.color.a * step(p.x, .65) * step(p.y, .65));
                // UV1 leads. Crop from the tail so length never changes head width.
                float along = (1 - i.uv.y) * 1.30 / max(.01, i.shape.w);
                float y = (floor(along * 16) + .5) / 16;
                float pixels = max(1, i.shape.z);
                float x = min(pixels - 1, floor(saturate(i.uv.x) * pixels));
                float head = 1 - step(.25, y);
                float width = y < .25 ? pixels : (y < .75 ? round(pixels * i.shape.y) : 1);
                // Three stable masks: bevel, one-pixel offset head, shorter tail.
                if (i.shape.x < 1.5 && y < .0625) width = max(1, pixels - 1);
                float left = floor((pixels - width) * .5);
                if (i.shape.x > .5 && i.shape.x < 1.5 && y < .125) left = min(pixels - width, left + 1);
                float end = i.shape.x > 1.5 ? .8125 : .9375;
                float mask = step(left, x) * (1 - step(left + width, x)) * (1 - step(end, y));
                return half4(i.color.rgb * lerp(1, 2.4, head), i.color.a * mask);
            }
            ENDHLSL
        }
    }
}
