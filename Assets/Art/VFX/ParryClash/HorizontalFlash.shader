Shader "TypingHero/ParryHorizontalFlash"
{
    SubShader
    {
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off Lighting Off ZWrite Off ZTest Always
        Blend SrcAlpha One
        CGINCLUDE
        #include "UnityCG.cginc"
        struct Input { float4 vertex : POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };
        struct Output { float4 vertex : SV_POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
        float4 _PresentationFlash, _FlashAspectRotation, _FlashColor;
        Output vert(Input v) { Output o; o.vertex = UnityObjectToClipPos(v.vertex); o.color = v.color; o.uv = v.uv; return o; }
        Output screenVert(Input v)
        {
            Output o; float2 p = (v.uv-.5)*_PresentationFlash.zw;
            p.y /= _FlashAspectRotation.x;
            float sn = sin(_FlashAspectRotation.y), cs = cos(_FlashAspectRotation.y);
            p = float2(cs*p.x-sn*p.y, sn*p.x+cs*p.y); p.y *= _FlashAspectRotation.x;
            float2 clipPoint = (_PresentationFlash.xy+p)*2-1;
            clipPoint.y *= _ProjectionParams.x;
            o.vertex=float4(clipPoint,0,1); o.color=_FlashColor; o.uv=v.uv; return o;
        }
        float4 frag(Output v):SV_Target
        {
            float r=abs(v.uv.x*2-1), tail=saturate(1-r);
            float y=abs(v.uv.y-.5)*6;
            float halfWidth=.625*sqrt(tail);
            float aa=max(fwidth(y),.0001)*.5;
            float core=1-smoothstep(halfWidth-aa,halfWidth+aa,y);
            float halo=pow(saturate(1-y/3),2)*pow(saturate(1-r/.4),2);
            float3 ink=float3(1,.65,.02)*2*tail*tail*core+float3(1,.30,.015)*.35*halo;
            return float4(ink*v.color.rgb,v.color.a);
        }
        ENDCG
        Pass { CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        ENDCG }
        Pass { Name "PARRY_PRESENTATION" Tags { "LightMode"="ParryPresentation" } CGPROGRAM
            #pragma vertex screenVert
            #pragma fragment frag
        ENDCG }
    }
}
