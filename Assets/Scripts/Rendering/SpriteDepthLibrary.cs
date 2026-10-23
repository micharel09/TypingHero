using System;
using System.Collections.Generic;
using UnityEngine;

// Authored maps and original, unpacked sprite UVs. Prepared once at world bind.
[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/Sprite Depth Library")]
public sealed class SpriteDepthLibrary : ScriptableObject
{
    [Serializable] public struct Frame
    {
        public Sprite sprite;
        public Texture2D depth;
        public Vector4 uvRect;
        public Vector2 textureSize;
        public float pixelsPerUnit;
    }
    public Frame[] frames = Array.Empty<Frame>();
    public Dictionary<Sprite, Frame> CreateLookup()
    {
        var result = new Dictionary<Sprite, Frame>(frames.Length);
        foreach (var frame in frames)
            if (frame.sprite && frame.depth) result.Add(frame.sprite, frame);
        return result;
    }
}
