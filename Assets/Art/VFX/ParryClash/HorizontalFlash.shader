Shader "TypingHero/ParryHorizontalFlash"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha One
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Input { float4 vertex : POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };
            struct Output { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };
            Output vert(Input v) { Output o; o.vertex = UnityObjectToClipPos(v.vertex); o.color = v.color; o.uv = v.uv; return o; }
            fixed4 frag(Output v) : SV_Target
            {
                float2 p = abs(v.uv - .5) * float2(72, 3);
                // A 1px warm core with a 3px gold halo; the last 8px taper at each end.
                float taper = saturate((36 - p.x) / 8);
                clip(1.5 * taper - p.y);
                fixed3 ink = p.y < .5 * taper ? fixed3(1, .96, .82) : fixed3(1, .67, .2);
                return fixed4(ink * v.color.rgb, v.color.a * taper);
            }
            ENDCG
        }
    }
}
