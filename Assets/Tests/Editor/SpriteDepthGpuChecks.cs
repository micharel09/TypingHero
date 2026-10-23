using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// Analytical ledge oracle, independent of the eight-sample implementation.
public static class SpriteDepthGpuChecks
{
    public static void Verify()
    {
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) throw new Exception("GPU required");
        string output = "/results/depth-gpu";
        Directory.CreateDirectory(output);
        var material = new Material(AssetDatabase.LoadAssetAtPath<Shader>(
            "Assets/Tests/Fixtures/Lighting/SelfShadow/DepthDiagnostic.shader"));
        var texture = new Texture2D(32,32,TextureFormat.RGBA32,false,true) { filterMode=FilterMode.Point };
        var target = new RenderTexture(32,32,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);
        var read = new Texture2D(32,32,TextureFormat.RGBAFloat,false,true);
        var before = RenderTexture.active;
        var results = new List<string> { "case,checkedPixels,mismatches" };
        int failures=0;
        try
        {
            target.Create();
            material.SetTexture("_DepthMap",texture);
            material.SetVector("_DepthRect",new Vector4(0,0,1,1));
            material.SetVector("_DepthTexel",new Vector4(1f/32,1f/32,16,0));
            material.SetVector("_DepthFacing",Vector4.one);
            material.SetFloat("_DepthScale",.15f); material.SetFloat("_SelfShadowBias",.01f);
            material.SetFloat("_SelfShadowStrength",.65f); material.SetFloat("_Cutoff",.35f);
            foreach (string name in new[] { "flat", "ledge-right", "ledge-left", "ledge-up", "ledge-down",
                "mirror-right", "transparent-blocker", "adjacent-cel" })
            {
                bool vertical=name.Contains("up") || name.Contains("down");
                bool negative=name.Contains("left") || name.Contains("down") || name.StartsWith("mirror");
                bool flat=name=="flat", transparent=name=="transparent-blocker", neighbor=name=="adjacent-cel";
                var pixels=new Color[32*32];
                for(int y=0;y<32;y++) for(int x=0;x<32;x++)
                {
                    int coordinate=vertical?y:x;
                    bool high=negative?coordinate<16:coordinate>=16;
                    pixels[y*32+x]=new Color(!flat && high?1:0,0,0,transparent && high?0:1);
                }
                texture.SetPixels(pixels); texture.Apply();
                material.SetVector("_DepthFacing",new Vector4(name.StartsWith("mirror")?-1:1,1,1,1));
                // World mirror flips +X to -X in source coordinates exactly once.
                Vector3 light=vertical?new Vector3(0,negative?-3:3,-2):new Vector3(negative && !name.StartsWith("mirror")?-3:3,0,-2);
                material.SetVector("_TestLight",light);
                material.SetVector("_DepthRect",new Vector4(0,0,neighbor?.5f:1,1));
                Graphics.Blit(Texture2D.whiteTexture,target,material);
                RenderTexture.active=target; read.ReadPixels(new Rect(0,0,32,32),0,0); read.Apply();
                var actual=read.GetPixels(); int count=0,mismatches=0;
                // Height .15 minus bias .01, divided by slope 2/3, gives
                // 3.36 source texels. At pixel centers the ledge shadows exactly
                // the nearest three receiver columns/rows (distances .5,1.5,2.5).
                for(int y=1;y<31;y++) for(int x=1;x<(neighbor?16:31);x++)
                {
                    int coordinate=vertical?y:x;
                    bool high=negative?coordinate<16:coordinate>=16;
                    float distance=negative?coordinate-15.5f:15.5f-coordinate;
                    float expected=!flat && !transparent && !neighbor && !high && distance<3.36f?.35f:1;
                    if(Mathf.Abs(actual[y*32+x].r-expected)>.004f) mismatches++;
                    count++;
                }
                results.Add(name+","+count+","+mismatches); failures+=mismatches;
                File.WriteAllBytes(Path.Combine(output,name+".exr"),read.EncodeToEXR());
            }
            File.WriteAllLines(Path.Combine(output,"checks.csv"),results);
            if(ShaderUtil.ShaderHasError(material.shader)) throw new Exception("Diagnostic shader failed");
            if(failures>0) throw new Exception("Analytical depth GPU mismatches: "+failures);
            Debug.Log("PASS: 8 analytical depth GPU cases; device="+SystemInfo.graphicsDeviceName);
        }
        finally
        {
            RenderTexture.active=before; target.Release();
            UnityEngine.Object.DestroyImmediate(material); UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(read);
        }
    }
}
