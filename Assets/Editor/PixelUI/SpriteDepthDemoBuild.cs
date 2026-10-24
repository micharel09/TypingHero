using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SpriteDepthDemoBuild
{
    public static void WebGL()
    {
        const string temporary="Assets/Automation/DepthDemo.generated.unity";
        string output=Environment.GetEnvironmentVariable("TYPINGHERO_BUILD_PATH");
        if(string.IsNullOrEmpty(output)) throw new InvalidOperationException("Missing isolated output path");
        if(File.Exists(temporary)) throw new InvalidOperationException("Refusing to replace an existing demo scene");
        var setup=EditorSceneManager.GetSceneManagerSetup();
        var compression=PlayerSettings.WebGL.compressionFormat;
        int width=PlayerSettings.defaultWebScreenWidth,height=PlayerSettings.defaultWebScreenHeight;
        try
        {
            var session=EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity",OpenSceneMode.Single);
            var roots=session.GetRootGameObjects();
            var hero=roots.SelectMany(x=>x.GetComponentsInChildren<PlayerHealth>(true)).Single().transform;
            var skeleton=roots.SelectMany(x=>x.GetComponentsInChildren<SkeletonController>(true)).Single().transform;
            var transforms=new[] { hero,skeleton };
            var sources=new[] { "Assets/Art/male_hero/individual_sheets/male_hero-combo_3.png",
                "Assets/Art/EndlessSkeletons/sword/Attack1.png" };
            var poses=new SpriteDepthDemo.Pose[2];
            for(int i=0;i<2;i++)
            {
                var sprite=AssetDatabase.LoadAllAssetsAtPath(sources[i]).OfType<Sprite>()
                    .OrderBy(x=>x.rect.x).ElementAt(i==0?1:2);
                var raw=new Texture2D(2,2);
                float bottom=sprite.rect.yMax;
                try
                {
                    raw.LoadImage(File.ReadAllBytes(sources[i]));
                    for(int y=(int)sprite.rect.yMin;y<sprite.rect.yMax;y++)
                    for(int x=(int)sprite.rect.xMin;x<sprite.rect.xMax;x++)
                        if(raw.GetPixel(x,y).a>=.35f) bottom=Mathf.Min(bottom,y);
                }
                finally { UnityEngine.Object.DestroyImmediate(raw); }
                float ground=transforms[i].TransformPoint(new Vector3(0,
                    (bottom-sprite.rect.yMin-sprite.pivot.y)/sprite.pixelsPerUnit,0)).y;
                poses[i]=new SpriteDepthDemo.Pose { sprite=sprite,
                    depth=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Tests/Fixtures/Lighting/SelfShadow/"+(i==0?"hero":"sword")+".png"),
                    position=transforms[i].position,rotation=transforms[i].rotation,scale=transforms[i].lossyScale,groundY=ground };
                Debug.Log("Depth demo pose "+sprite.name+" position="+poses[i].position+" scale="+poses[i].scale);
            }
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/DungeonWorld.unity",OpenSceneMode.Single);
            var demo=new GameObject("DepthDemo").AddComponent<SpriteDepthDemo>();
            demo.poses=poses;
            // Serialize the built-in render mesh; the lab needs no physics collider.
            var floor=GameObject.CreatePrimitive(PrimitiveType.Plane);
            demo.neutralFloorMesh=floor.GetComponent<MeshFilter>().sharedMesh;
            UnityEngine.Object.DestroyImmediate(floor);
            demo.diagnosticShader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Tests/Fixtures/Lighting/SelfShadow/DepthActorDiagnostic.shader");
            EditorSceneManager.SaveScene(scene,temporary);
            PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
            PlayerSettings.defaultWebScreenWidth=1280; PlayerSettings.defaultWebScreenHeight=720;
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes=new[] { temporary },locationPathName=output,target=BuildTarget.WebGL,
                options=BuildOptions.None,extraScriptingDefines=new[] { "TYPINGHERO_DEPTH_DEMO" }
            });
            if(report.summary.result!=BuildResult.Succeeded) throw new Exception("Depth demo build failed: "+report.summary.result);
            File.WriteAllText(Path.Combine(output,"depth-demo.json"),"{\"qa\":true,\"poses\":2,\"animation\":false,\"productionEnabled\":false}");
            Debug.Log("PASS: isolated two-pose WebGL depth demo");
        }
        finally
        {
            PlayerSettings.WebGL.compressionFormat=compression;
            PlayerSettings.defaultWebScreenWidth=width; PlayerSettings.defaultWebScreenHeight=height;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            AssetDatabase.DeleteAsset(temporary);
            if(setup.Any(x=>x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
}
