using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/Flame Profile")]
public sealed class FlameProfile : ScriptableObject
{
    [Serializable] public struct Frame { public Rect uv; [Min(0)] public float seconds; }
    public Material material;
    public Mesh mesh;
    [Tooltip("UV rectangle occupied by the authored mesh. Frame rectangles remap this region.")]
    public Rect meshUV;
    public Frame[] frames = Array.Empty<Frame>();
    [Tooltip("Visible source height divided by the common canvas height; keeps transparent padding and pivot.")]
    [Range(0, 1)] public float contentHeightRatio;

    public void Validate()
    {
        if (!material || !material.HasProperty("_BaseMap") || !material.GetTexture("_BaseMap") || !material.HasProperty("_EmissionColor") || !mesh || meshUV.width <= 0 || meshUV.height <= 0 || contentHeightRatio <= 0 || frames.Length == 0)
            throw new InvalidOperationException(name + ": missing flame material, mesh, UV region, content height or frames");
        foreach (var frame in frames)
            if (frame.seconds <= 0 || frame.uv.width <= 0 || frame.uv.height <= 0 ||
                frame.uv.xMin < 0 || frame.uv.yMin < 0 || frame.uv.xMax > 1 || frame.uv.yMax > 1)
                throw new InvalidOperationException(name + ": invalid frame UV/duration");
    }

    public int FrameAt(float seconds)
    {
        bool uniform = true; double total = 0;
        foreach (var frame in frames) { total += frame.seconds; uniform &= frame.seconds == frames[0].seconds; }
        if (uniform) return ((Mathf.FloorToInt(seconds / frames[0].seconds) % frames.Length) + frames.Length) % frames.Length;
        double cursor = ((seconds % total) + total) % total;
        for (int i = 0; i < frames.Length; i++) { cursor -= frames[i].seconds; if (cursor < 0) return i; }
        return frames.Length - 1;
    }
}
