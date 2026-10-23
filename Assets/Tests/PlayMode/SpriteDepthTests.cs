using System;
using NUnit.Framework;
using UnityEngine;

public sealed class SpriteDepthTests
{
    [Test, Category("Quick")]
    public void FrameFacingClearAndWorldRebindPreserveCombatPropertiesWithoutAllocations()
    {
        var host = new GameObject("depth binding", typeof(SpriteRenderer), typeof(SpriteDepthBinding));
        var texture = new Texture2D(16, 8);
        var a = Sprite.Create(texture, new Rect(0,0,8,8), Vector2.one * .5f, 16);
        var b = Sprite.Create(texture, new Rect(8,0,8,8), Vector2.one * .5f, 32);
        var missing = Sprite.Create(texture, new Rect(0,0,4,4), Vector2.zero, 16);
        var library = ScriptableObject.CreateInstance<SpriteDepthLibrary>();
        library.frames = new[] {
            new SpriteDepthLibrary.Frame { sprite = a, depth = texture, uvRect = new Vector4(0,0,.5f,1), textureSize = new Vector2(16,8), pixelsPerUnit = 16 },
            new SpriteDepthLibrary.Frame { sprite = b, depth = texture, uvRect = new Vector4(.5f,0,1,1), textureSize = new Vector2(16,8), pixelsPerUnit = 32 }
        };
        try
        {
            var renderer = host.GetComponent<SpriteRenderer>();
            var binding = host.GetComponent<SpriteDepthBinding>();
            var block = new MaterialPropertyBlock();
            int flash = Shader.PropertyToID("_FlashAmount"), tint = Shader.PropertyToID("_SlayerTintWeight");
            int ready = Shader.PropertyToID("_DepthReady"), rect = Shader.PropertyToID("_DepthRect");
            int facing = Shader.PropertyToID("_DepthFacing");
            block.SetFloat(flash,.75f); block.SetFloat(tint,.3f); renderer.SetPropertyBlock(block);
            renderer.sprite = a;
            var lookup = library.CreateLookup(); binding.Bind(lookup);
            renderer.GetPropertyBlock(block);
            Assert.That(block.GetFloat(ready), Is.EqualTo(1));
            Assert.That(block.GetVector(rect), Is.EqualTo(library.frames[0].uvRect));
            renderer.sprite = b; renderer.flipX = true; binding.Refresh(false);
            renderer.GetPropertyBlock(block);
            Assert.That(block.GetVector(rect), Is.EqualTo(library.frames[1].uvRect));
            Assert.That(block.GetVector(facing), Is.EqualTo(new Vector4(-1,1,0,0)));
            Assert.That(block.GetFloat(flash), Is.EqualTo(.75f));
            Assert.That(block.GetFloat(tint), Is.EqualTo(.3f));
            binding.Bind(null); renderer.GetPropertyBlock(block);
            Assert.That(block.GetFloat(ready), Is.Zero);
            binding.Bind(lookup); renderer.GetPropertyBlock(block);
            Assert.That(block.GetFloat(ready), Is.EqualTo(1));
            var otherWorld = new System.Collections.Generic.Dictionary<Sprite, SpriteDepthLibrary.Frame> {
                { b, new SpriteDepthLibrary.Frame { sprite = b, depth = texture,
                    uvRect = new Vector4(.25f,0,.75f,1), textureSize = new Vector2(16,8), pixelsPerUnit = 8 } }
            };
            binding.Bind(otherWorld); renderer.GetPropertyBlock(block);
            Assert.That(block.GetVector(rect), Is.EqualTo(otherWorld[b].uvRect));
            binding.Bind(lookup); renderer.GetPropertyBlock(block);
            Assert.That(block.GetVector(rect), Is.EqualTo(library.frames[1].uvRect));
            renderer.sprite = missing; binding.Refresh(false); renderer.GetPropertyBlock(block);
            Assert.That(block.GetFloat(ready), Is.Zero);
            for (int i=0;i<32;i++) { renderer.sprite = i%2==0?a:b; binding.Refresh(false); }
            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i=0;i<256;i++) { renderer.sprite = i%2==0?a:b; renderer.flipX = i%2==0; binding.Refresh(false); }
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.That(allocated, Is.Zero, "warmed frame/facing binding must not allocate");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host); UnityEngine.Object.DestroyImmediate(library);
            UnityEngine.Object.DestroyImmediate(a); UnityEngine.Object.DestroyImmediate(b);
            UnityEngine.Object.DestroyImmediate(missing); UnityEngine.Object.DestroyImmediate(texture);
        }
    }
}
