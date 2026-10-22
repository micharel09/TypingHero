using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static class DungeonFullEffectsBuild
{
    internal static void Apply(GameObject root)
    {
        foreach (int side in new[] { -1, 1 })
        {
            var light = root.transform.Find("TorchGlow_" + side);
            var position = light.localPosition;
            DungeonAtmosphereBuild.Sheet(root, "TorchBeam" + side, position,
                position + new Vector3(side * .65f, -position.y + .1f, -2.5f), .15f, 2.5f,
                new Color(1, .53f, .19f, .025f), false);
        }
        DungeonAtmosphereBuild.Sheet(root, "RearHaze", new Vector3(1, 4, 9.2f), new Vector3(1, .2f, 9.2f),
            22, 22, new Color(.12f, .18f, .23f, .1f), true);
        DungeonAtmosphereBuild.Sheet(root, "LowHaze", new Vector3(0, .9f, 4), new Vector3(0, .05f, 4),
            19, 19, new Color(.14f, .19f, .23f, .075f), true);

        // Retain legacy serialized bindings without creating a reflection receiver.
        var wet = root.transform.Find("WetStoneReflection");
        if (wet)
        {
            var reflection = wet.GetComponent<DungeonPlanarReflection>();
            if (reflection) reflection.enabled = false;
            var receiver = wet.GetComponent<Renderer>();
            if (receiver) receiver.enabled = false;
            wet.gameObject.SetActive(false);
        }

        var rune = Node(root, "RearRune"); rune.transform.localPosition = new Vector3(-5, 2.6f, 9.76f);
        var runeRenderer = Visual(rune, Mesh("RearRune", new[] {
            new Vector3(-.3f,-.4f,0),new Vector3(.3f,-.4f,0),new Vector3(-.3f,.4f,0),new Vector3(.3f,.4f,0) }),
            Material("RearRune", "CLACKBLADE/Dungeon Rune"));
        var runeNode = Node(root, "RuneLight"); runeNode.transform.localPosition = new Vector3(-5, 2.6f, 9.3f);
        var localLight = runeNode.GetComponent<Light>(); if (!localLight) localLight = runeNode.AddComponent<Light>();
        localLight.type = LightType.Point; localLight.color = new Color(.2f,.6f,1);
        localLight.intensity = .7f; localLight.range = 2; localLight.shadows = LightShadows.None;

        var rain = Rain(root, "RearRain", new Vector3(-7, 5.7f, 8.8f), false);
        var drops = Rain(root, "StoneDrips", new Vector3(-7, .08f, 8.8f), true);
        var motion = root.GetComponent<DungeonEnvironmentMotion>(); if (!motion) motion = root.AddComponent<DungeonEnvironmentMotion>();
        var settings = new SerializedObject(motion);
        var renderers = settings.FindProperty("atmosphere");
        var moving = new[] { root.transform.Find("TorchBeam-1").GetComponent<Renderer>(),
            root.transform.Find("TorchBeam1").GetComponent<Renderer>(), root.transform.Find("RearHaze").GetComponent<Renderer>(),
            root.transform.Find("LowHaze").GetComponent<Renderer>(), runeRenderer };
        renderers.arraySize = moving.Length;
        for (int i=0;i<moving.Length;i++) renderers.GetArrayElementAtIndex(i).objectReferenceValue=moving[i];
        var weather = settings.FindProperty("weather");
        var systems = new[] { rain, drops, root.transform.Find("SparseDust").GetComponent<ParticleSystem>() };
        weather.arraySize=systems.Length;
        for(int i=0;i<systems.Length;i++)weather.GetArrayElementAtIndex(i).objectReferenceValue=systems[i];
        settings.FindProperty("banner").objectReferenceValue=root.transform.Find("PixelRoom/OldBanner").GetComponent<MeshFilter>();
        settings.FindProperty("runeLight").objectReferenceValue=localLight;
        settings.ApplyModifiedPropertiesWithoutUndo();
    }
    static ParticleSystem Rain(GameObject root, string name, Vector3 position, bool splash)
    {
        var node=Node(root,name); node.transform.localPosition=position;
        var system=node.GetComponent<ParticleSystem>();if(!system)system=node.AddComponent<ParticleSystem>();
        system.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        system.useAutoRandomSeed=false;system.randomSeed=splash?7481u:7480u;
        var main=system.main;main.loop=true;main.prewarm=true;main.startLifetime=splash?.24f:1.6f;
        main.startSpeed=0;main.startSize=splash?.025f:.012f;main.maxParticles=splash?8:24;
        main.simulationSpace=ParticleSystemSimulationSpace.Local; main.gravityModifier=splash?.3f:0;
        main.startColor=new Color(.48f,.65f,.8f,splash?.18f:.24f);
        var emission=system.emission;emission.rateOverTime=splash?3:8;
        var shape=system.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(1.5f,.03f,.4f);
        var velocity=system.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.Local;
        velocity.x=.08f;velocity.y=splash?.6f:-3.5f;velocity.z=0;
        var renderer=system.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(DungeonArenaBuild.Art+"DungeonAtmosphere.mat");
        renderer.renderMode=splash?ParticleSystemRenderMode.Billboard:ParticleSystemRenderMode.Stretch;
        renderer.lengthScale=3;renderer.velocityScale=.025f;renderer.shadowCastingMode=ShadowCastingMode.Off;
        renderer.receiveShadows=false;return system;
    }
    static Mesh Mesh(string name,Vector3[] vertices)
    {
        string path=DungeonPixelRoomBuild.Art+name+".asset";
        var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if(!mesh){mesh=new Mesh{name=name};AssetDatabase.CreateAsset(mesh,path);}
        mesh.Clear();mesh.vertices=vertices;mesh.uv=new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one};
        mesh.triangles=new[]{0,2,1,1,2,3};mesh.RecalculateNormals();mesh.RecalculateBounds();EditorUtility.SetDirty(mesh);return mesh;
    }
    static Renderer Visual(GameObject node,Mesh mesh,Material material)
    {
        var filter=node.GetComponent<MeshFilter>();if(!filter)filter=node.AddComponent<MeshFilter>();filter.sharedMesh=mesh;
        var renderer=node.GetComponent<MeshRenderer>();if(!renderer)renderer=node.AddComponent<MeshRenderer>();
        renderer.sharedMaterial=material;renderer.enabled=true;renderer.shadowCastingMode=ShadowCastingMode.Off;
        renderer.receiveShadows=false;return renderer;
    }
    static Material Material(string name,string shader)
    {
        string path=DungeonPixelRoomBuild.Art+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!material){material=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(material,path);}return material;
    }
    static GameObject Node(GameObject root,string name)
    {
        var child=root.transform.Find(name);if(child)return child.gameObject;
        var node=new GameObject(name);node.transform.SetParent(root.transform,false);return node;
    }
    static void Set(Object target,string name,Object value)
    {
        var settings=new SerializedObject(target);settings.FindProperty(name).objectReferenceValue=value;settings.ApplyModifiedPropertiesWithoutUndo();
    }
}
