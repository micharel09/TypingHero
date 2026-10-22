using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Final combat-only pass. Existing menu and all gameplay objects retain their owners.
internal static class DungeonArenaBuild
{
    internal const string Art = "Assets/Art/Environment/KayKitDungeon/";
    internal static void Apply()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/UniversalRP.asset");
        const string rendererPath = "Assets/Settings/DungeonForward.asset";
        var forward = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
        if (!forward) { forward = ScriptableObject.CreateInstance<UniversalRendererData>(); AssetDatabase.CreateAsset(forward, rendererPath); }
        forward.rendererFeatures.RemoveAll(feature => !feature || feature.name == "Dungeon Point Blit");
        foreach (var obsolete in AssetDatabase.LoadAllAssetsAtPath(rendererPath).OfType<ScriptableRendererFeature>()
            .Where(feature => feature.name == "Dungeon Point Blit").ToArray()) Object.DestroyImmediate(obsolete, true);
        var world = Feature<DungeonWorldRender>(forward, rendererPath, "Dungeon environment and actors");
        Set(world, "blurShader", Shader.Find("Hidden/CLACKBLADE/Dungeon Depth Blur"));
        var worldSettings = new SerializedObject(world);
        worldSettings.FindProperty("blurRadius").floatValue = .65f;
        worldSettings.ApplyModifiedPropertiesWithoutUndo();
        const string screenPath = "Assets/Settings/DungeonPresentation.asset";
        var screen = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(screenPath);
        if (!screen) { screen = ScriptableObject.CreateInstance<UniversalRendererData>(); AssetDatabase.CreateAsset(screen, screenPath); }
        var present = Feature<DungeonPresentationRender>(screen, screenPath, "Dungeon Point presentation");
        Set(present, "pointShader", Shader.Find("Hidden/CLACKBLADE/Dungeon Present"));
        ResourceReloader.ReloadAllNullIn(screen, "Packages/com.unity.render-pipelines.universal");
        var screenSettings = new SerializedObject(screen);
        screenSettings.FindProperty("postProcessData").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PostProcessData>("Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
        screenSettings.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(screen); AssetDatabase.SaveAssetIfDirty(screen);
        // Restore package resources after the rejected custom-blit experiment.
        var forwardSettings = new SerializedObject(forward);
        forwardSettings.FindProperty("m_CopyDepthMode").intValue = (int)CopyDepthMode.AfterOpaques;
        var coreBlit = AssetDatabase.LoadAssetAtPath<Shader>("Packages/com.unity.render-pipelines.universal/Shaders/Utils/CoreBlit.shader");
        if (!coreBlit) throw new System.InvalidOperationException("URP CoreBlit shader is missing");
        forwardSettings.FindProperty("shaders").FindPropertyRelative("coreBlitPS").objectReferenceValue = coreBlit;
        forwardSettings.FindProperty("postProcessData").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PostProcessData>("Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
        forwardSettings.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(forward); AssetDatabase.SaveAssetIfDirty(forward);
        var settings = new SerializedObject(pipeline);
        var renderers = settings.FindProperty("m_RendererDataList");
        if (renderers.arraySize < 3) renderers.arraySize = 3;
        renderers.GetArrayElementAtIndex(1).objectReferenceValue = forward;
        renderers.GetArrayElementAtIndex(2).objectReferenceValue = screen;
        settings.FindProperty("m_AdditionalLightShadowsSupported").boolValue=true;
        settings.FindProperty("m_MainLightShadowmapResolution").intValue=1024;
        settings.FindProperty("m_AdditionalLightsShadowmapResolution").intValue=2048;
        settings.FindProperty("m_AdditionalLightsShadowResolutionTierLow").intValue=256;
        settings.FindProperty("m_AdditionalLightsShadowResolutionTierMedium").intValue=512;
        settings.FindProperty("m_ShadowCascadeCount").intValue=1;
        settings.FindProperty("m_ShadowDistance").floatValue=36;
        settings.ApplyModifiedPropertiesWithoutUndo();
        pipeline.msaaSampleCount = 1;
        pipeline.renderScale = 1;
        var textureImporter=(TextureImporter)AssetImporter.GetAtPath(Art+"dungeon_texture.png");
        if(textureImporter.textureShape!=TextureImporterShape.Texture2D || textureImporter.textureType!=TextureImporterType.Default || textureImporter.filterMode!=FilterMode.Point || textureImporter.mipmapEnabled)
        { textureImporter.textureShape=TextureImporterShape.Texture2D;textureImporter.textureType=TextureImporterType.Default;textureImporter.filterMode=FilterMode.Point;textureImporter.mipmapEnabled=false;textureImporter.wrapMode=TextureWrapMode.Clamp;textureImporter.SaveAndReimport(); }
        pipeline.upscalingFilter = UpscalingFilterSelection.Point;
        var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(Art + "dungeon_texture.png");
        if(!atlas) throw new System.InvalidOperationException("KayKit atlas must import as Texture2D");
        var stone = Material("DungeonStone", "Universal Render Pipeline/Lit");
        stone.SetTexture("_BaseMap", atlas); stone.SetTexture("_MainTex",atlas);
        var stoneColor=new Color(.45f,.49f,.53f);stone.SetColor("_BaseColor",stoneColor);stone.SetColor("_Color",stoneColor); stone.SetFloat("_Smoothness", .05f);
        EditorUtility.SetDirty(stone);AssetDatabase.SaveAssetIfDirty(stone);
        var floorStone=Surface("DungeonFloor",atlas,new Color(.42f,.46f,.48f),.07f);
        var wetStone=Surface("DungeonDampStone",atlas,new Color(.36f,.41f,.43f),.68f);
        var wood=Surface("DungeonWood",atlas,new Color(.7f,.54f,.4f),.12f);
        var iron=Surface("DungeonIron",atlas,new Color(.33f,.37f,.4f),.3f,.55f);
        var torchMaterial=Surface("DungeonTorch",atlas,new Color(.65f,.57f,.45f),.18f);
        torchMaterial.SetTexture("_EmissionMap",TorchEmissionMask());torchMaterial.SetColor("_EmissionColor",new Color(3.2f,1.25f,.3f));
        torchMaterial.EnableKeyword("_EMISSION");torchMaterial.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;
        EditorUtility.SetDirty(torchMaterial);
        UnityEngine.Rendering.ResourceReloader.ReloadAllNullIn(forward, "Packages/com.unity.render-pipelines.universal");
        var sprite = Material("DungeonSprite", "CLACKBLADE/Dungeon Sprite");
        sprite.SetFloat("_Metallic",1); sprite.SetFloat("_Roughness",1); sprite.SetFloat("_NormalStrength",.8f);
        EditorUtility.SetDirty(sprite); AssetDatabase.SaveAssetIfDirty(sprite);
        var old = Object.FindObjectsOfType<Transform>(true).FirstOrDefault(t => t.name == "DungeonEnvironment");
        if (old)
        {
            foreach(var renderer in old.GetComponentsInChildren<Renderer>(true)) renderer.enabled=false;
            foreach(var backdrop in old.GetComponentsInChildren<CombatR7Backdrop>(true)) backdrop.enabled=false;
        }
        var existing = Object.FindObjectsOfType<Transform>(true).FirstOrDefault(t => t.name == "DungeonArena3D");
        // Reuse serialized nodes so repeated build IDs and references remain stable.
        var root = existing ? existing.gameObject : new GameObject("DungeonArena3D");
        root.SetActive(true);
        var layout = Object.FindObjectOfType<CombatHUDLayout>();
        float floor = layout.LowestFoot.y - .025f;
        root.transform.position = new Vector3(0, floor, 0);
        DungeonPixelRoomBuild.Apply(root);
        foreach (int side in new[]{-1,1})
        {
            float x=side<0?-5.5f:6.5f, height=side<0?3.25f:3.65f;
            Model(root,"Torch_"+side,"torch_lit",new Vector3(x,height,9.35f),new Vector3(.4f,1.05f,.4f),torchMaterial);
            Torch(root,"TorchGlow_"+side,new Vector3(x,height+.35f,8.95f),16,15,side<0?.7f:2.9f,
                root.transform.Find("Torch_"+side).GetComponentsInChildren<Renderer>());
        }
        // No unmotivated directional key or camera-side fill survives a rebuild.
        foreach(string name in new[]{"ShadowKey","CombatPool"})
        {
            var obsolete=root.transform.Find(name);
            if(obsolete && obsolete.TryGetComponent<Light>(out var light))light.enabled=false;
        }
        var pulse=Light(root,"CombatPulse",LightType.Point,new Color(1,.62f,.24f),0,Vector3.zero);
        pulse.enabled=false;pulse.range=12;pulse.shadows=LightShadows.Hard;
        pulse.shadowBias=.015f;pulse.shadowNormalBias=.01f;pulse.lightmapBakeType=LightmapBakeType.Realtime;
        var rig=root.GetComponent<ClashLightingRig>(); if(!rig)rig=root.AddComponent<ClashLightingRig>();
        Set(rig,"combatLight",pulse);
        var parryLight=Light(root,"ParryContactLight",LightType.Point,new Color(1,.67f,.28f),0,Vector3.zero);
        parryLight.range=3.75f; parryLight.shadows=LightShadows.None;
        Set(rig,"parryLight",parryLight);
        foreach(var other in Object.FindObjectsOfType<ClashLightingRig>(true)) if(other!=rig)
        {
            foreach(var legacy in other.GetComponentsInChildren<Light2D>(true))
            { legacy.enabled=false;PrefabUtility.RecordPrefabInstancePropertyModifications(legacy); }
            other.gameObject.SetActive(false);
            PrefabUtility.RecordPrefabInstancePropertyModifications(other.gameObject);
        }
        RenderSettings.ambientMode=AmbientMode.Flat;
        RenderSettings.ambientLight=Color.black;RenderSettings.ambientIntensity=0;
        RenderSettings.ambientSkyColor=Color.black;RenderSettings.ambientEquatorColor=Color.black;
        RenderSettings.ambientGroundColor=Color.black;
        RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogColor=Color.black;
        DungeonAtmosphereBuild.Apply(root);
        var camera=Camera.main; camera.GetUniversalAdditionalCameraData().SetRenderer(1);
        camera.allowMSAA=false; camera.allowHDR=true; camera.GetUniversalAdditionalCameraData().renderPostProcessing=false;
        camera.GetUniversalAdditionalCameraData().antialiasing=AntialiasingMode.None;
        camera.backgroundColor=new Color(.012f,.017f,.026f); camera.clearFlags=CameraClearFlags.SolidColor;
        var driver=camera.GetComponent<DungeonArenaCamera>(); if(!driver)driver=camera.gameObject.AddComponent<DungeonArenaCamera>();
        var player=Object.FindObjectOfType<PlayerHealth>(); var enemy=Object.FindObjectOfType<SkeletonController>();
        driver.player=player.transform;driver.enemy=enemy.transform;
        const float pitch = 5f, normalOrtho = 5.625f;
        const float fieldOfView = 35;
        float distance=normalOrtho/Mathf.Tan(fieldOfView*.5f*Mathf.Deg2Rad);
        RenderSettings.fogStartDistance=distance+2;RenderSettings.fogEndDistance=distance+24;
        var driverSettings = new SerializedObject(driver);
        driverSettings.FindProperty("pitch").floatValue = pitch;
        driverSettings.FindProperty("framingLayout").objectReferenceValue = layout;
        driverSettings.FindProperty("framingDistance").floatValue = distance;
        driverSettings.ApplyModifiedPropertiesWithoutUndo();
        // Match the previous actor scale at the focus plane, with actual perspective.
        float radians = pitch * Mathf.Deg2Rad;
        var center=layout.BodyCenter;
        var cameraPosition=center+new Vector3(0,Mathf.Sin(radians)*distance,-Mathf.Cos(radians)*distance);
        camera.transform.position=cameraPosition;camera.transform.rotation=Quaternion.Euler(pitch,0,0);
        camera.orthographicSize=normalOrtho;
        camera.orthographic=false;camera.fieldOfView=fieldOfView;camera.aspect=16f/9;
        camera.nearClipPlane=.1f;camera.farClipPlane=80;
        foreach(var vcam in Object.FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>(true))
        {
            vcam.transform.position=cameraPosition;vcam.transform.rotation=camera.transform.rotation;
            vcam.m_Lens.ModeOverride=Cinemachine.LensSettings.OverrideModes.Perspective;
            vcam.m_Lens.OrthographicSize=normalOrtho;vcam.m_Lens.FieldOfView=fieldOfView;
            vcam.m_Lens.NearClipPlane=.1f;vcam.m_Lens.FarClipPlane=80;
        }
        DungeonPixelRoomBuild.ApplyGround(root,camera);
        var flame=new Vector3(center.x-root.transform.position.x,3.405f,
            center.z-root.transform.position.z-2);
        var lanternFire = Material("DungeonLanternFlame", "Universal Render Pipeline/Lit");
        lanternFire.CopyPropertiesFromMaterial(torchMaterial);
        lanternFire.SetColor("_BaseColor", Color.black); lanternFire.SetColor("_Color", Color.black);
        lanternFire.SetFloat("_Smoothness", 0); lanternFire.SetFloat("_SpecularHighlights", 0);
        lanternFire.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
        lanternFire.SetColor("_EmissionColor", new Color(1.4f,.9f,.32f));
        EditorUtility.SetDirty(lanternFire);
        HangingLamp(root,flame,lanternFire);
        Torch(root,"TorchGlow_Front",flame,24,16,1.6f,
            new[]{root.transform.Find("Torch_Front/ModelMesh").GetComponent<Renderer>()},0);
        var frontData=root.transform.Find("TorchGlow_Front").GetComponent<Light>().GetUniversalAdditionalLightData();
        var frontSettings=new SerializedObject(frontData);
        frontSettings.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue=UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierMedium;
        frontSettings.ApplyModifiedPropertiesWithoutUndo();
        foreach(var viewport in Object.FindObjectsOfType<CombatViewportCamera>(true))
        {
            var viewportSettings=new SerializedObject(viewport);
            viewportSettings.FindProperty("normalOrtho").floatValue=normalOrtho;
            viewportSettings.FindProperty("anchorFloorDuringZoom").boolValue=false;
            viewportSettings.ApplyModifiedPropertiesWithoutUndo();
        }
        foreach(var actor in new[]{player.gameObject,enemy.gameObject})
        foreach(var renderer in actor.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if(renderer.name=="ContactShadow") { renderer.enabled=false;continue; }
            if(!renderer.sprite)continue;
            renderer.sharedMaterial=sprite;renderer.shadowCastingMode=ShadowCastingMode.TwoSided;renderer.receiveShadows=true;
            renderer.lightProbeUsage=LightProbeUsage.Off;
            EditorUtility.SetDirty(renderer);
            if(PrefabUtility.IsPartOfPrefabInstance(renderer))PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
        }
        var cinematic=Object.FindObjectOfType<SlayerScreenFX>();
        var cinematicSettings=new SerializedObject(cinematic);
        cinematicSettings.FindProperty("normalOrtho").floatValue=normalOrtho;
        cinematicSettings.FindProperty("slayerOrtho").floatValue=3.3f;
        cinematicSettings.ApplyModifiedPropertiesWithoutUndo();
        var red=cinematicSettings.FindProperty("redOverlay").objectReferenceValue as SpriteRenderer;
        if(red)
        {
            red.sharedMaterial=AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            var position=red.transform.position;position.z=3;red.transform.position=position;
            PrefabUtility.RecordPrefabInstancePropertyModifications(red);
            PrefabUtility.RecordPrefabInstancePropertyModifications(red.transform);
        }
        Set(cinematic,"environment",root.transform);
        DungeonFullEffectsBuild.Apply(root);
        EditorUtility.SetDirty(pipeline);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
    }
    static void HangingLamp(GameObject root,Vector3 flame,Material fire)
    {
        // Retain the former torch holder and its mesh node; only fire triangles survive.
        Model(root,"Torch_Front","torch_lit",Vector3.zero,new Vector3(.5f,.8f,.5f),fire);
        var holder=root.transform.Find("Torch_Front");
        var visual=holder.Find("ModelMesh");
        var original=visual.GetComponent<MeshFilter>().sharedMesh;
        var indices=original.triangles;var uv=original.uv;
        var triangles=new System.Collections.Generic.List<int>();
        for(int i=0;i<indices.Length;i+=3)
        {
            var c=(uv[indices[i]]+uv[indices[i+1]]+uv[indices[i+2]])/3;
            if(c.x<.875f||c.x>1||c.y<.25f||c.y>=.5f)continue;
            triangles.AddRange(new[]{indices[i],indices[i+1],indices[i+2]});
        }
        string path=Art+"HangingFlame.asset";
        var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if(!mesh){mesh=new Mesh{name="Hanging lamp fire"};AssetDatabase.CreateAsset(mesh,path);}
        mesh.Clear();mesh.vertices=original.vertices;mesh.uv=uv;mesh.normals=original.normals;
        mesh.triangles=triangles.ToArray();
        var fireBounds=new Bounds(original.vertices[triangles[0]],Vector3.zero);
        foreach(int index in triangles)fireBounds.Encapsulate(original.vertices[index]);
        mesh.bounds=fireBounds;EditorUtility.SetDirty(mesh);
        visual.GetComponent<MeshFilter>().sharedMesh=mesh;
        holder.localPosition=flame-root.transform.InverseTransformPoint(visual.TransformPoint(mesh.bounds.center));
        visual.GetComponent<MeshRenderer>().enabled=true;
        DungeonHangingLanternBuild.Apply(root,flame);
    }
    static void Torch(GameObject root,string name,Vector3 position,float intensity,float range,float phase,Renderer[] renderers,float flickerAmplitude=.02f)
    {
        var visible=renderers.Where(renderer=>renderer.name!="TorchSolidShadow").ToArray();
        Bounds flameBounds=new Bounds();bool found=false;
        foreach(var renderer in visible)
        {
            if(!renderer.TryGetComponent<MeshFilter>(out var filter))continue;
            var mesh=filter.sharedMesh;var uv=mesh.uv;var vertices=mesh.vertices;var indices=mesh.triangles;
            var solid=new System.Collections.Generic.List<int>();
            for(int i=0;i<indices.Length;i+=3)
            {
                Vector2 center=(uv[indices[i]]+uv[indices[i+1]]+uv[indices[i+2]])/3;
                bool fire=center.x>=.875f&&center.x<=1&&center.y>=.25f&&center.y<.5f;
                if(!fire){solid.Add(indices[i]);solid.Add(indices[i+1]);solid.Add(indices[i+2]);continue;}
                for(int j=0;j<3;j++)
                {
                    var point=renderer.transform.TransformPoint(vertices[indices[i+j]]);
                    if(!found){flameBounds=new Bounds(point,Vector3.zero);found=true;}else flameBounds.Encapsulate(point);
                }
            }
            renderer.shadowCastingMode=ShadowCastingMode.Off;
            string path=Art+"TorchSolid_"+mesh.name+".asset";
            var shadowMesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(!shadowMesh){shadowMesh=new Mesh{name="Torch solid shadow"};AssetDatabase.CreateAsset(shadowMesh,path);}
            shadowMesh.Clear();shadowMesh.vertices=vertices;shadowMesh.uv=uv;shadowMesh.normals=mesh.normals;
            shadowMesh.triangles=solid.ToArray();shadowMesh.RecalculateBounds();EditorUtility.SetDirty(shadowMesh);
            var proxy=Node(renderer.gameObject,"TorchSolidShadow");
            var shadowFilter=proxy.GetComponent<MeshFilter>();if(!shadowFilter)shadowFilter=proxy.AddComponent<MeshFilter>();shadowFilter.sharedMesh=shadowMesh;
            var shadowRenderer=proxy.GetComponent<MeshRenderer>();if(!shadowRenderer)shadowRenderer=proxy.AddComponent<MeshRenderer>();
            shadowRenderer.sharedMaterial=renderer.sharedMaterial;shadowRenderer.shadowCastingMode=ShadowCastingMode.ShadowsOnly;
            shadowRenderer.enabled=true;shadowRenderer.receiveShadows=false;
            shadowRenderer.receiveGI=ReceiveGI.LightProbes;shadowRenderer.lightProbeUsage=LightProbeUsage.BlendProbes;
            GameObjectUtility.SetStaticEditorFlags(proxy,StaticEditorFlags.BatchingStatic|StaticEditorFlags.OccludeeStatic|StaticEditorFlags.OccluderStatic|StaticEditorFlags.ReflectionProbeStatic);
        }
        if(!found)throw new System.InvalidOperationException("Torch flame UV region missing: "+name);
        position=root.transform.InverseTransformPoint(flameBounds.center);
        Debug.Log("TORCH_SOURCE "+name+" local="+position+" flame="+flameBounds.size);
        var light=Light(root,name,LightType.Point,new Color(1,.78f,.56f),intensity,position);
        light.enabled=true;light.range=range;light.shadows=LightShadows.Hard;
        light.shadowBias=.015f;light.shadowNormalBias=.02f;light.lightmapBakeType=LightmapBakeType.Realtime;
        var flicker=light.GetComponent<DungeonTorchLight>();if(!flicker)flicker=light.gameObject.AddComponent<DungeonTorchLight>();
        var settings=new SerializedObject(flicker);
        settings.FindProperty("flickerAmplitude").floatValue=flickerAmplitude;
        settings.FindProperty("baseIntensity").floatValue=intensity;settings.FindProperty("phase").floatValue=phase;
        var flames=settings.FindProperty("flameRenderers");flames.arraySize=visible.Length;
        for(int i=0;i<visible.Length;i++)flames.GetArrayElementAtIndex(i).objectReferenceValue=visible[i];
        settings.ApplyModifiedPropertiesWithoutUndo();
    }
    static Texture2D TorchEmissionMask()
    {
        // Data mask: the orange/yellow palette strip is fire; wood/metal live above it.
        string path=Art+"TorchEmission.png";
        {
            var source=new Texture2D(2,2);source.LoadImage(System.IO.File.ReadAllBytes(Art+"dungeon_texture.png"));
            var pixels=source.GetPixels();
            for(int y=0;y<source.height;y++)for(int x=0;x<source.width;x++)
            {
                var c=pixels[y*source.width+x];
                bool fire=y>=source.height/4&&y<source.height/2&&x>=source.width*7/8;
                pixels[y*source.width+x]=fire?c:Color.black;
            }
            source.SetPixels(pixels);source.Apply();var bytes=source.EncodeToPNG();Object.DestroyImmediate(source);
            if(!System.IO.File.Exists(path)||!System.IO.File.ReadAllBytes(path).SequenceEqual(bytes))
            {System.IO.File.WriteAllBytes(path,bytes);AssetDatabase.ImportAsset(path);}
        }
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);
        if(importer.filterMode!=FilterMode.Point||importer.mipmapEnabled){importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.SaveAndReimport();}
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static Material Material(string name,string shader)
    {
        string path=Art+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!material){material=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(material,path);} return material;
    }
    static Material Surface(string name,Texture2D atlas,Color color,float smoothness,float metallic=0)
    {
        var material=Material(name,"Universal Render Pipeline/Lit");
        material.SetTexture("_BaseMap",atlas);material.SetColor("_BaseColor",color);
        material.SetFloat("_Smoothness",smoothness);material.SetFloat("_Metallic",metallic);
        EditorUtility.SetDirty(material);return material;
    }
    static T Feature<T>(UniversalRendererData renderer,string path,string name) where T:ScriptableRendererFeature
    {
        var feature=renderer.rendererFeatures.OfType<T>().FirstOrDefault();
        if(!feature) feature=AssetDatabase.LoadAllAssetsAtPath(path).OfType<T>().FirstOrDefault();
        if(!feature) { feature=ScriptableObject.CreateInstance<T>(); feature.name=name; AssetDatabase.AddObjectToAsset(feature,renderer); }
        if(!renderer.rendererFeatures.Contains(feature)) renderer.rendererFeatures.Add(feature);
        EditorUtility.SetDirty(feature); return feature;
    }
    static GameObject Node(GameObject parent,string name)
    {
        var child=parent.transform.Find(name);if(child)return child.gameObject;
        var go=new GameObject(name);go.transform.SetParent(parent.transform,false);return go;
    }
    static void Model(GameObject root,string name,string asset,Vector3 center,Vector3 size,Material material,float yaw=0)
    {
        var holder=Node(root,name);
        // Keep the serialized holder, and reference the original mesh directly.
        // Local shadow-only children must not become FBX prefab overrides.
        foreach(var child in holder.transform.Cast<Transform>().ToArray())
            if(PrefabUtility.IsPartOfPrefabInstance(child))Object.DestroyImmediate(child.gameObject);
        var source=AssetDatabase.LoadAssetAtPath<GameObject>(Art+asset+".fbx");
        var mesh=source.GetComponentInChildren<MeshFilter>().sharedMesh;
        var visual=Node(holder,"ModelMesh");
        var filter=visual.GetComponent<MeshFilter>();if(!filter)filter=visual.AddComponent<MeshFilter>();filter.sharedMesh=mesh;
        var renderer=visual.GetComponent<MeshRenderer>();if(!renderer)renderer=visual.AddComponent<MeshRenderer>();
        renderer.sharedMaterial=material;renderer.enabled=true;renderer.receiveShadows=true;
        renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;
        holder.transform.localPosition=center;holder.transform.localRotation=Quaternion.Euler(0,yaw,0);holder.transform.localScale=Vector3.one;
        var scale=new Vector3(size.x/mesh.bounds.size.x,size.y/mesh.bounds.size.y,size.z/mesh.bounds.size.z);
        visual.transform.localRotation=Quaternion.identity;visual.transform.localScale=scale;
        visual.transform.localPosition=-Vector3.Scale(mesh.bounds.center,scale);
    }
    static Light Light(GameObject root,string name,LightType type,Color color,float intensity,Vector3 position)
    {
        var go=Node(root,name);go.transform.localPosition=position;var light=go.GetComponent<Light>();if(!light)light=go.AddComponent<Light>();
        light.type=type;light.color=color;light.intensity=intensity;
        var data=light.GetUniversalAdditionalLightData();data.usePipelineSettings=false;
        // URP owns the tier selection; Light.shadowCustomResolution alone does not.
        var settings=new SerializedObject(data);
        settings.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue=UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow;
        settings.ApplyModifiedPropertiesWithoutUndo();
        return light;
    }
    static void Set(Object owner,string property,Object value)
    {var settings=new SerializedObject(owner);settings.FindProperty(property).objectReferenceValue=value;settings.ApplyModifiedPropertiesWithoutUndo();}
}
