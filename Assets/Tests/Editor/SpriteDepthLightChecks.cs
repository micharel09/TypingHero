using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class SpriteDepthLightChecks
{
    public static void Verify()
    {
        var setup=EditorSceneManager.GetSceneManagerSetup();
        var oldRT=RenderTexture.active;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var material=new Material(Shader.Find("CLACKBLADE/Dungeon Sprite"));
        var albedo=new Texture2D(32,32,TextureFormat.RGBA32,false,true) { filterMode=FilterMode.Point };
        var depth=new Texture2D(32,32,TextureFormat.RGBA32,false,true) { filterMode=FilterMode.Point };
        var normal=new Texture2D(1,1,TextureFormat.RGBA32,false,true);
        var rt=new RenderTexture(64,64,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);
        var read=new Texture2D(64,64,TextureFormat.RGBAFloat,false,true);
        var library=ScriptableObject.CreateInstance<SpriteDepthLibrary>();
        Sprite sprite=null;
        try
        {
            RenderSettings.ambientMode=AmbientMode.Flat; RenderSettings.ambientLight=Color.black; RenderSettings.fog=false;
            var pixels=new Color[1024]; for(int i=0;i<pixels.Length;i++) pixels[i]=new Color(.4f,.4f,.4f,1);
            albedo.SetPixels(pixels); albedo.Apply();
            for(int i=0;i<pixels.Length;i++) pixels[i]=new Color(0,0,0,1);
            depth.SetPixels(pixels); depth.Apply();
            sprite=Sprite.Create(albedo,new Rect(0,0,32,32),Vector2.one*.5f,16);
            var actor=new GameObject("photometric fixture",typeof(SpriteRenderer));
            var renderer=actor.GetComponent<SpriteRenderer>(); renderer.sprite=sprite; renderer.sharedMaterial=material;
            material.SetFloat("_Metallic",0); material.SetFloat("_NormalStrength",0);
            material.SetColor("_EmissionColor",new Color(0,.1f,0,1));
            library.frames=new[] { new SpriteDepthLibrary.Frame { sprite=sprite,depth=depth,
                textureSize=new Vector2(32,32),uvRect=new Vector4(0,0,1,1),pixelsPerUnit=16 } };
            var binding=actor.AddComponent<SpriteDepthBinding>(); binding.Bind(library.CreateLookup());
            var key=new GameObject("red main",typeof(Light)).GetComponent<Light>();
            key.type=LightType.Directional; key.color=Color.red; key.intensity=1;
            key.transform.rotation=Quaternion.LookRotation(new Vector3(-3,0,2)); RenderSettings.sun=key;
            var fill=new GameObject("blue additional",typeof(Light)).GetComponent<Light>();
            fill.type=LightType.Point; fill.color=Color.blue; fill.intensity=24; fill.range=20;
            fill.transform.position=new Vector3(-3,0,-2); fill.renderMode=LightRenderMode.ForcePixel;
            var camera=new GameObject("fixture",typeof(Camera),typeof(DungeonCaptureCamera)).GetComponent<Camera>();
            camera.GetUniversalAdditionalCameraData().SetRenderer(1);
            camera.transform.position=new Vector3(0,0,-10); camera.orthographic=true; camera.orthographicSize=1;
            camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=Color.black;
            camera.allowHDR=true; camera.allowMSAA=false; camera.enabled=false; rt.Create(); camera.targetTexture=rt;
            Func<Color[]> capture=()=> { camera.Render(); RenderTexture.active=rt;
                read.ReadPixels(new Rect(0,0,64,64),0,0); read.Apply(); return read.GetPixels(); };
            material.SetFloat("_SelfShadowStrength",0); capture(); capture(); var off=capture();
            material.SetFloat("_SelfShadowStrength",.65f); var flat=capture();
            float flatError=MaxError(off,flat);
            if(flatError>1f/255) throw new Exception("Flat-depth BRDF parity failed: "+flatError);
            for(int y=0;y<32;y++) for(int x=16;x<32;x++) pixels[y*32+x]=new Color(1,0,0,1);
            depth.SetPixels(pixels); depth.Apply(); var on=capture();
            int receiver=32*64+29;
            if(off[receiver].r-on[receiver].r<.01f || Mathf.Abs(off[receiver].b-on[receiver].b)>1f/255 ||
                Mathf.Abs(off[receiver].g-on[receiver].g)>1f/255 || on[receiver].b<.01f || on[receiver].g<.01f)
                throw new Exception("Per-light isolation failed: off="+off[receiver]+" on="+on[receiver]);
            binding.Bind(null); var missing=capture(); float missingError=MaxError(off,missing);
            if(missingError>1f/255) throw new Exception("Missing-map fallback failed: "+missingError);
            binding.Bind(library.CreateLookup()); fill.enabled=false;
            renderer.flipX=true; binding.Refresh(true);
            material.SetFloat("_SelfShadowStrength",0); var mirroredOff=capture();
            material.SetFloat("_SelfShadowStrength",.65f); var mirroredOn=capture();
            // The raised right half is now on the WORLD left. A light on the
            // world right cannot cast it onto the lower right-hand receiver.
            float mirrorError=MaxError(mirroredOff,mirroredOn);
            if(mirrorError>1f/255) throw new Exception("Actual SpriteRenderer mirror casts towards the light: "+mirrorError);
            key.transform.rotation=Quaternion.LookRotation(new Vector3(3,0,2));
            material.SetFloat("_SelfShadowStrength",0); var oppositeOff=capture();
            material.SetFloat("_SelfShadowStrength",.65f); var oppositeOn=capture();
            int mirroredReceiver=32*64+(63-29);
            if(oppositeOff[mirroredReceiver].r-oppositeOn[mirroredReceiver].r<.01f)
                throw new Exception("Actual SpriteRenderer mirror lost the expected opposite-light shadow");
            key.transform.rotation=Quaternion.LookRotation(new Vector3(-3,0,2));
            renderer.flipX=false; actor.transform.localScale=new Vector3(-1,1,1); binding.Refresh(true);
            material.SetFloat("_SelfShadowStrength",0); var scaleOff=capture();
            material.SetFloat("_SelfShadowStrength",.65f); var scaleOn=capture();
            if(MaxError(scaleOff,scaleOn)>1f/255) throw new Exception("Negative-scale facing failed");
            renderer.flipX=true; binding.Refresh(true);
            material.SetFloat("_SelfShadowStrength",0); var doubleOff=capture();
            material.SetFloat("_SelfShadowStrength",.65f); var doubleOn=capture();
            if(doubleOff[receiver].r-doubleOn[receiver].r<.01f) throw new Exception("Scale plus sprite flip must cancel");
            // A known 45-degree normal points toward +X. Reflecting the sprite
            // must put that plane away from a world +X light (N dot L < 0).
            actor.transform.localScale=Vector3.one; renderer.flipX=false; binding.Refresh(true);
            normal.SetPixel(0,0,new Color(.8535534f,.5f,.8535534f,1)); normal.Apply();
            material.SetTexture("_NormalMap",normal); material.SetFloat("_NormalStrength",1);
            material.SetFloat("_SelfShadowStrength",0); var facingNormal=capture();
            renderer.flipX=true; binding.Refresh(true); var reflectedNormal=capture();
            if(facingNormal[receiver].r<.1f || reflectedNormal[receiver].r>.004f)
                throw new Exception("Mapped normal facing failed: "+facingNormal[receiver]+" / "+reflectedNormal[receiver]);
            Directory.CreateDirectory("/results/depth-gpu");
            File.WriteAllText("/results/depth-gpu/photometric.txt","flatMax="+flatError+"\nmissingMax="+missingError+
                "\nreceiverOff="+off[receiver]+"\nreceiverOn="+on[receiver]+"\nmirrorMax="+mirrorError+"\n");
            Debug.Log("PASS: flat-depth BRDF parity, main-light occlusion, additional-light fill, emission isolation, missing map");
        }
        finally
        {
            RenderTexture.active=oldRT;
            rt.Release(); UnityEngine.Object.DestroyImmediate(rt); UnityEngine.Object.DestroyImmediate(read);
            UnityEngine.Object.DestroyImmediate(material); UnityEngine.Object.DestroyImmediate(sprite);
            UnityEngine.Object.DestroyImmediate(albedo); UnityEngine.Object.DestroyImmediate(depth);
            UnityEngine.Object.DestroyImmediate(normal);
            UnityEngine.Object.DestroyImmediate(library);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            if(setup.Length>0) EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
    static float MaxError(Color[] a,Color[] b)
    {
        float maximum=0;
        for(int i=0;i<a.Length;i++) for(int c=0;c<3;c++) maximum=Mathf.Max(maximum,Mathf.Abs(a[i][c]-b[i][c]));
        return maximum;
    }
}
