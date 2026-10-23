#ifndef CLACKBLADE_DEPTH_VISIBILITY
#define CLACKBLADE_DEPTH_VISIBILITY
   static const int SELF_SHADOW_SAMPLES_PER_LIGHT = 8;
   static const float SELF_SHADOW_MAX_DISTANCE_TEXELS = 12;
   // Eight source-texel probes, bounded to the current cel. Positive authored
   // height points towards local -Z, matching the existing normal convention.
   half SelfVisibility(float2 uv, float3 lightWS, float height) {
    float3 ray = mul((float3x3)GetWorldToObjectMatrix(), lightWS);
    ray.xy *= _DepthFacing.xy;
    float planar = length(ray.xy);
    if (planar < .00001) return 1;
    float2 direction = ray.xy / planar;
    float slope = -ray.z / planar;
    [unroll] for (int sampleIndex = 1; sampleIndex <= SELF_SHADOW_SAMPLES_PER_LIGHT; sampleIndex++) {
     // Preserve one-texel arm/weapon occluders near the shaded point, then
     // spread the remaining probes towards the twelve-texel search limit.
     float distance = sampleIndex <= 4 ? sampleIndex : 4 + (sampleIndex - 4) * ((SELF_SHADOW_MAX_DISTANCE_TEXELS - 4) / 4);
     float2 probe = uv + direction * distance * _DepthTexel.xy;
     if (any(probe < _DepthRect.xy) || any(probe >= _DepthRect.zw)) break;
     half4 blocker = SAMPLE_TEXTURE2D_LOD(_DepthMap, sampler_DepthMap, probe, 0);
     float rayHeight = height + slope * distance / _DepthTexel.z;
     if (blocker.a >= _Cutoff && blocker.r * _DepthScale > rayHeight + _SelfShadowBias)
      return 1 - _SelfShadowStrength;
    }
    return 1;
   }
#endif
