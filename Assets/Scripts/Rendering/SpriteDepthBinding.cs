using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer))]
[DefaultExecutionOrder(30000)]
public sealed class SpriteDepthBinding : MonoBehaviour
{
    static readonly int Map = Shader.PropertyToID("_DepthMap");
    static readonly int Rect = Shader.PropertyToID("_DepthRect");
    static readonly int Texel = Shader.PropertyToID("_DepthTexel");
    static readonly int Ready = Shader.PropertyToID("_DepthReady");
    static readonly int Facing = Shader.PropertyToID("_DepthFacing");
    SpriteRenderer target;
    MaterialPropertyBlock block;
    Dictionary<Sprite, SpriteDepthLibrary.Frame> frames;
    Sprite previous;
    bool flipX, flipY;

    public void Bind(Dictionary<Sprite, SpriteDepthLibrary.Frame> lookup)
    {
        if (!target) target = GetComponent<SpriteRenderer>();
        if (block == null) block = new MaterialPropertyBlock();
        frames = lookup;
        previous = null;
        Refresh(true);
    }
    void LateUpdate() { Refresh(false); }
    public void Refresh(bool force)
    {
        if (!target) return;
        var sprite = target.sprite;
        if (!force && sprite == previous && flipX == target.flipX && flipY == target.flipY) return;
        previous = sprite; flipX = target.flipX; flipY = target.flipY;
        target.GetPropertyBlock(block);
        // URP14 SpriteRenderer already mirrors its submitted vertices/UVs;
        // the legacy material _Flip stays (1,1). Depth rays need source UV facing.
        block.SetVector(Facing, new Vector4(flipX ? -1 : 1, flipY ? -1 : 1, 0, 0));
        SpriteDepthLibrary.Frame frame = default;
        bool ready = sprite && frames != null && frames.TryGetValue(sprite, out frame);
        block.SetFloat(Ready, ready ? 1 : 0);
        if (ready)
        {
            block.SetTexture(Map, frame.depth);
            block.SetVector(Rect, frame.uvRect);
            block.SetVector(Texel, new Vector4(1 / frame.textureSize.x, 1 / frame.textureSize.y,
                frame.pixelsPerUnit, 0));
        }
        else
        {
            block.SetTexture(Map, Texture2D.blackTexture);
            block.SetVector(Rect, Vector4.zero);
            block.SetVector(Texel, Vector4.zero);
        }
        target.SetPropertyBlock(block);
    }
}
