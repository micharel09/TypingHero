using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/World Lighting Profile")]
public sealed class WorldLightingProfile : ScriptableObject
{
    public AmbientMode ambientMode;
    public Color ambient, sky, equator, ground;
    public float ambientIntensity;
    public bool fog;
    public FogMode fogMode;
    public Color fogColor;
    public float fogStart, fogEnd, fogDensity;
    public VolumeProfile post;
    public LightingSettings baking;
    public Material actorMaterial;
    public UniversalRenderPipelineAsset pipeline;
    public ScriptableRendererData worldRenderer, presentationRenderer;
    [HideInInspector] public int worldRendererIndex, presentationRendererIndex;
    public Vector2Int resolution;
    public RenderTextureFormat textureFormat;
    public int depthBits;
    public Color background;
    public float pitch, normalOrtho, fieldOfView, nearClip, farClip, slayerOrtho;
    public float followLimit, followSmooth;
    public bool postProcessing;
    public float blurRadius, focusOffset, focusFalloff;
    public LightProfile rune;
    public Vector2 runeNoiseOrigin, runeNoiseSpeed;
    public float runeBase, runeNoiseGain, runeEnvelopeGain, runeEnvelopePeriod;
    public float FramingDistance => normalOrtho / Mathf.Tan(fieldOfView * .5f * Mathf.Deg2Rad);

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!pipeline) return;
        var list = new UnityEditor.SerializedObject(pipeline).FindProperty("m_RendererDataList");
        worldRendererIndex = presentationRendererIndex = -1;
        for (int i = 0; i < list.arraySize; i++)
        {
            var value = list.GetArrayElementAtIndex(i).objectReferenceValue;
            if (value == worldRenderer) worldRendererIndex = i;
            if (value == presentationRenderer) presentationRendererIndex = i;
        }
    }
#endif
    public void Validate()
    {
        if (!post || !pipeline || !worldRenderer || !presentationRenderer || !actorMaterial ||
            resolution.x <= 0 || resolution.y <= 0 || fieldOfView <= 0 || fieldOfView >= 180 || normalOrtho <= 0 ||
            nearClip <= 0 || farClip <= nearClip || followSmooth <= 0 || focusFalloff <= 0 || runeEnvelopePeriod <= 0)
            throw new InvalidOperationException(name + ": incomplete world lighting/presentation profile");
        if (rune) rune.Validate();
    }
    public float RuneGain(float age)
    {
        float noise = Mathf.Clamp01(Mathf.PerlinNoise(runeNoiseOrigin.x + age * runeNoiseSpeed.x, runeNoiseOrigin.y + age * runeNoiseSpeed.y));
        return runeBase + runeNoiseGain * noise + runeEnvelopeGain * (.5f + .5f * Mathf.Sin(2 * Mathf.PI * age / runeEnvelopePeriod));
    }
    public void ApplyEnvironment()
    {
        RenderSettings.ambientMode = ambientMode; RenderSettings.ambientLight = ambient; RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.ambientSkyColor = sky; RenderSettings.ambientEquatorColor = equator; RenderSettings.ambientGroundColor = ground;
        RenderSettings.fog = fog; RenderSettings.fogMode = fogMode; RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = fogStart; RenderSettings.fogEndDistance = fogEnd; RenderSettings.fogDensity = fogDensity;
    }
}
