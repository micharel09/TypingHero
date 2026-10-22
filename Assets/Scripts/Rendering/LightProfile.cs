using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/Light Profile")]
public sealed class LightProfile : ScriptableObject
{
    // URP 14 exposes this serialized setting through a read-only getter.
    // The pinned package field is also used by its Inspector; configure it once per activation.
    static readonly System.Reflection.FieldInfo ShadowTier = typeof(UniversalAdditionalLightData).GetField(
        "m_AdditionalLightsShadowResolutionTier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
    public enum Flicker { Constant, Perlin, SinePair }
    public bool enabled;
    public LightType type;
    public Color color;
    [Min(0)] public float intensity, range, spotAngle, innerSpotAngle;
    public LightShadows shadows;
    public float shadowBias, shadowNormalBias, shadowNearPlane, shadowStrength;
    public LightmapBakeType bakeType;
    public LayerMask cullingMask;
    public int shadowResolutionTier;
    public Flicker flicker;
    [Range(0, 1)] public float amplitude;
    public float noiseSpeed, noiseRow;
    public Vector2 sineFrequency, sineWeight, sinePhase;
    public bool synchronizeEmission;
    public FlameProfile flame;
    [Min(0)] public float projectedFlameHeight;

    public void Validate()
    {
        if (intensity < 0 || (type != LightType.Directional && range <= 0) || amplitude < 0 || amplitude > 1)
            throw new InvalidOperationException(name + ": invalid light intensity/range/flicker");
        if (flame) { flame.Validate(); if (projectedFlameHeight <= 0) throw new InvalidOperationException(name + ": flame height must be positive"); }
    }
    public float Multiplier(float age, float phase)
    {
        float wave = flicker == Flicker.Perlin ? 2 * Mathf.Clamp01(Mathf.PerlinNoise(phase + age * noiseSpeed, noiseRow)) - 1 :
            flicker == Flicker.SinePair ? sineWeight.x * Mathf.Sin(age * sineFrequency.x + phase * sinePhase.x) +
                sineWeight.y * Mathf.Sin(age * sineFrequency.y + phase * sinePhase.y) : 0;
        return 1 + amplitude * Mathf.Clamp(wave, -1, 1);
    }
    public void Apply(Light light)
    {
        light.enabled = enabled; light.type = type; light.color = color; light.intensity = intensity; light.range = range;
        light.spotAngle = spotAngle; light.innerSpotAngle = innerSpotAngle;
        light.shadows = shadows; light.shadowBias = shadowBias; light.shadowNormalBias = shadowNormalBias;
        light.shadowNearPlane = shadowNearPlane; light.shadowStrength = shadowStrength;
        light.lightmapBakeType = bakeType; light.cullingMask = cullingMask;
        var data = light.GetUniversalAdditionalLightData(); data.usePipelineSettings = false;
        ShadowTier.SetValue(data, shadowResolutionTier);
    }
}
