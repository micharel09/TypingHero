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
        var forwardSettings = new SerializedObject(forward);
        var postData = forwardSettings.FindProperty("postProcessData");
        if (!postData.objectReferenceValue)
        {
            postData.objectReferenceValue = AssetDatabase.LoadAssetAtPath<PostProcessData>(
                "Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
            if (!postData.objectReferenceValue) throw new System.InvalidOperationException("Dungeon renderer requires URP post-process data for the parry bloom.");
            forwardSettings.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(forward);
        }
        var settings = new SerializedObject(pipeline);
        var renderers = settings.FindProperty("m_RendererDataList");
        if (renderers.arraySize < 2) renderers.arraySize = 2;
        renderers.GetArrayElementAtIndex(1).objectReferenceValue = forward;
        settings.FindProperty("m_AdditionalLightShadowsSupported").boolValue=true;
        settings.ApplyModifiedPropertiesWithoutUndo();
        pipeline.msaaSampleCount = 1;
        var textureImporter=(TextureImporter)AssetImporter.GetAtPath(Art+"dungeon_texture.png");
        if(textureImporter.filterMode!=FilterMode.Point || textureImporter.mipmapEnabled)
        { textureImporter.filterMode=FilterMode.Point;textureImporter.mipmapEnabled=false;textureImporter.wrapMode=TextureWrapMode.Clamp;textureImporter.SaveAndReimport(); }
        pipeline.upscalingFilter = UpscalingFilterSelection.Point;
        var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(Art + "dungeon_texture.png");
        if (!atlas) throw new System.InvalidOperationException("Dungeon atlas did not import as Texture2D: " + Art + "dungeon_texture.png");
        var stone = Material("DungeonStone", "Universal Render Pipeline/Lit");
        stone.SetTexture("_BaseMap", atlas); stone.SetColor("_BaseColor", new Color(.11f,.12f,.13f)); stone.SetFloat("_Smoothness", .05f);
        // URP validates these legacy aliases on import; keep the first serialization identical.
        stone.SetTexture("_MainTex", atlas); stone.SetColor("_Color", stone.GetColor("_BaseColor"));
        EditorUtility.SetDirty(stone);
        UnityEngine.Rendering.ResourceReloader.ReloadAllNullIn(forward, "Packages/com.unity.render-pipelines.universal");
        var sprite = Material("DungeonSprite", "CLACKBLADE/Dungeon Sprite");
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
        // Slayer hides room geometry; shared combat lighting must remain active for the debris.
        var lighting = GameObject.Find("DungeonCombatLighting") ?? new GameObject("DungeonCombatLighting");
        lighting.transform.position = root.transform.position;
        // All debris rests on the visible mesh floor, including the new received-hit emitter.
        foreach (var spawner in Object.FindObjectsOfType<HitSparkSpawner>(true))
        { var data = new SerializedObject(spawner); data.FindProperty("groundY").floatValue = floor; data.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(spawner); if(PrefabUtility.IsPartOfPrefabInstance(spawner))PrefabUtility.RecordPrefabInstancePropertyModifications(spawner); }
        foreach (var spawner in Object.FindObjectsOfType<ParrySparkVFX_Pooled>(true))
        { var data = new SerializedObject(spawner); data.FindProperty("groundY").floatValue = floor; data.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(spawner); if(PrefabUtility.IsPartOfPrefabInstance(spawner))PrefabUtility.RecordPrefabInstancePropertyModifications(spawner); }
        for (int x=-7;x<=6;x++) for(int z=-4;z<=5;z++)
            Model(root, "Floor_"+x+"_"+z, "floor_tile_large", new Vector3(x*2+1,-.15f,z*2+1), new Vector3(2,.3f,2),stone);
        for (int x=-3;x<=3;x++)
            Model(root,"BackWall_"+x,x==0?"wall_doorway":"wall",new Vector3(x*4,2.5f,4.5f),new Vector3(4,5,.7f),stone);
        // Receding corridor visible through the central doorway.
        Model(root,"CorridorEnd","wall_archedwindow_gated",new Vector3(0,2.4f,9),new Vector3(4,4.8f,.7f),stone);
        foreach (int side in new[]{-1,1})
        {
            for(int z=0;z<3;z++)
                Model(root,"Column_"+side+"_"+z,"column",new Vector3(side*(9-z*.7f),3,z*2+1),new Vector3(1.3f,6,1.3f),stone);
            Model(root,"FrontPillar_"+side,"pillar",new Vector3(side*9.7f,3.5f,-2),new Vector3(1.6f,7,1.6f),stone);
            Model(root,"Rubble_"+side,"rubble_large",new Vector3(side*8,.35f,4),new Vector3(2,.7f,1.4f),stone);
            Model(root,"Barrel_"+side,"barrel_large",new Vector3(side*9,.7f,3.5f),new Vector3(1,1.4f,1),stone);
            Model(root,"Torch_"+side,"torch_mounted",new Vector3(side*5,3.7f,4),new Vector3(.5f,1.2f,.5f),stone);
            var torch = Light(lighting,"TorchGlow_"+side,LightType.Point,new Color(1,.38f,.10f),1.6f,new Vector3(side*5,4.1f,3.5f)); torch.range=6;
        }
        var key=Light(lighting,"ShadowKey",LightType.Directional,new Color(.63f,.73f,.86f),1.1f,Vector3.zero);
        key.transform.rotation=Quaternion.Euler(25,-20,0); key.shadows=LightShadows.Hard; key.shadowBias=.025f; key.shadowNormalBias=.02f;
        var pulse=Light(lighting,"CombatPulse",LightType.Spot,new Color(1,.62f,.24f),0,Vector3.zero); pulse.range=12; pulse.spotAngle=115; pulse.innerSpotAngle=75; pulse.shadows=LightShadows.Hard; pulse.shadowBias=.015f; pulse.shadowNormalBias=.01f; pulse.transform.rotation=Quaternion.Euler(20,0,0);
        var rig=lighting.GetComponent<ClashLightingRig>(); if(!rig)rig=lighting.AddComponent<ClashLightingRig>();
        Set(rig,"combatLight",pulse);
        var parryLight=Light(lighting,"ParryContactLight",LightType.Point,new Color(1,.67f,.28f),0,Vector3.zero);
        parryLight.range=3.75f; parryLight.shadows=LightShadows.None;
        Set(rig,"parryLight",parryLight);
        foreach(var other in Object.FindObjectsOfType<ClashLightingRig>(true)) if(other!=rig)
        {
            foreach(var legacy in other.GetComponentsInChildren<Light2D>(true))
            { legacy.enabled=false;PrefabUtility.RecordPrefabInstancePropertyModifications(legacy); }
            other.gameObject.SetActive(false);
            PrefabUtility.RecordPrefabInstancePropertyModifications(other.gameObject);
        }
        RenderSettings.ambientMode=AmbientMode.Flat; RenderSettings.ambientLight=new Color(.16f,.20f,.28f);
        var camera=Camera.main; camera.GetUniversalAdditionalCameraData().SetRenderer(1);
        // Dungeon Sprite evaluates additional lights per fragment, including the parry point.
        var lightSettings=new SerializedObject(pipeline);
        lightSettings.FindProperty("m_AdditionalLightsRenderingMode").intValue=(int)LightRenderingMode.PerPixel;
        lightSettings.ApplyModifiedPropertiesWithoutUndo();
        camera.allowMSAA=false; camera.allowHDR=true; camera.GetUniversalAdditionalCameraData().renderPostProcessing=true;
        string bloomPath=ParryClashAssets.Root+"ParryBloom.asset";
        var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(bloomPath);
        if(!profile)
        {
            profile=ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile,bloomPath);
            var bloom=profile.Add<Bloom>(); bloom.threshold.Override(4); bloom.intensity.Override(.2f); bloom.scatter.Override(.3f);
            AssetDatabase.AddObjectToAsset(bloom,profile); EditorUtility.SetDirty(profile);
        }
        var volume=lighting.GetComponent<Volume>(); if(!volume)volume=lighting.AddComponent<Volume>();
        volume.isGlobal=true; volume.sharedProfile=profile;
        camera.backgroundColor=new Color(.012f,.017f,.026f); camera.clearFlags=CameraClearFlags.SolidColor;
        var driver=camera.GetComponent<DungeonArenaCamera>(); if(!driver)driver=camera.gameObject.AddComponent<DungeonArenaCamera>();
        var player=Object.FindObjectOfType<PlayerHealth>(); var enemy=Object.FindObjectOfType<SkeletonController>();
        driver.player=player.transform;driver.enemy=enemy.transform;
        // Preserve the current floor framing while tipping the stable side camera down.
        float y=layout.LowestFoot.y+(3+Mathf.Sin(10*Mathf.Deg2Rad)*-camera.transform.position.z)/Mathf.Cos(10*Mathf.Deg2Rad);
        var cameraPosition=camera.transform.position;cameraPosition.y=y;camera.transform.position=cameraPosition;camera.transform.rotation=Quaternion.Euler(10,0,0);
        foreach(var vcam in Object.FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>())
        {
            var pos=vcam.transform.position;pos.y=y;vcam.transform.position=pos;vcam.transform.rotation=camera.transform.rotation;
            var listener=vcam.GetComponents<Cinemachine.CinemachineImpulseListener>().FirstOrDefault(l=>l.m_ChannelMask==2);
            if(!listener)
            {
                listener=vcam.gameObject.AddComponent<Cinemachine.CinemachineImpulseListener>();
                listener.m_ChannelMask=2; listener.m_Gain=1; listener.m_UseCameraSpace=false;
                listener.m_ReactionSettings.m_AmplitudeGain=0;
            }
        }
        foreach(var actor in new[]{player.gameObject,enemy.gameObject})
        foreach(var renderer in actor.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if(renderer.name=="ContactShadow") { renderer.enabled=false;continue; }
            if(!renderer.sprite)continue;
            renderer.sharedMaterial=sprite;renderer.shadowCastingMode=ShadowCastingMode.TwoSided;renderer.receiveShadows=true;
            EditorUtility.SetDirty(renderer);
            if(PrefabUtility.IsPartOfPrefabInstance(renderer))PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
        }
        var cinematic=Object.FindObjectOfType<SlayerScreenFX>();
        var cinematicSettings=new SerializedObject(cinematic);
        var red=cinematicSettings.FindProperty("redOverlay").objectReferenceValue as SpriteRenderer;
        if(red)
        {
            red.sharedMaterial=AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            var position=red.transform.position;position.z=3;red.transform.position=position;
            PrefabUtility.RecordPrefabInstancePropertyModifications(red);
            PrefabUtility.RecordPrefabInstancePropertyModifications(red.transform);
        }
        Set(cinematic,"environment",root.transform);
        EditorUtility.SetDirty(pipeline);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
    }
    static Material Material(string name,string shader)
    {
        string path=Art+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!material){material=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(material,path);} return material;
    }
    static GameObject Node(GameObject parent,string name)
    {
        var child=parent.transform.Find(name);if(child)return child.gameObject;
        var go=new GameObject(name);go.transform.SetParent(parent.transform,false);return go;
    }
    static void Model(GameObject root,string name,string asset,Vector3 center,Vector3 size,Material material)
    {
        var holder=Node(root,name);
        if(holder.transform.childCount==0)
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(Art+asset+".fbx");
            if(!source)throw new System.InvalidOperationException("Missing KayKit model: "+asset);
            var model=(GameObject)PrefabUtility.InstantiatePrefab(source);model.transform.SetParent(holder.transform,false);
        }
        holder.transform.localPosition=Vector3.zero;holder.transform.localScale=Vector3.one;
        var renderers=holder.GetComponentsInChildren<MeshRenderer>();
        var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
        // Normalize authored mesh dimensions without changing source FBX or texture.
        var scale=new Vector3(size.x/bounds.size.x,size.y/bounds.size.y,size.z/bounds.size.z);
        holder.transform.localScale=scale;holder.transform.position=root.transform.position+center-Vector3.Scale(bounds.center-root.transform.position,scale);
        foreach(var renderer in renderers){renderer.sharedMaterials=Enumerable.Repeat(material,renderer.sharedMaterials.Length).ToArray();renderer.shadowCastingMode=ShadowCastingMode.On;renderer.receiveShadows=true;PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);}
    }
    static Light Light(GameObject root,string name,LightType type,Color color,float intensity,Vector3 position)
    {
        var go=Node(root,name);go.transform.localPosition=position;var light=go.GetComponent<Light>();if(!light)light=go.AddComponent<Light>();
        light.type=type;light.color=color;light.intensity=intensity;return light;
    }
    static void Set(Object owner,string property,Object value)
    {var settings=new SerializedObject(owner);settings.FindProperty(property).objectReferenceValue=value;settings.ApplyModifiedPropertiesWithoutUndo();}
}
