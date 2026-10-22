using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal static class DungeonAtmosphereBuild
{
    internal static void Apply(GameObject root)
    {
        const string path = DungeonArenaBuild.Art + "DungeonPost.asset";
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
        if (!profile) { profile = ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile, path); }
        var bloom = Effect<Bloom>(profile); bloom.threshold.Override(1.15f); bloom.intensity.Override(.12f);
        bloom.scatter.Override(.65f); bloom.highQualityFiltering.Override(false);
        Effect<Tonemapping>(profile).mode.Override(TonemappingMode.ACES);
        var grade = Effect<ColorAdjustments>(profile); grade.postExposure.Override(.35f);
        grade.contrast.Override(2); grade.saturation.Override(-4);
        Effect<WhiteBalance>(profile).temperature.Override(-2);
        var vignette = Effect<Vignette>(profile); vignette.intensity.Override(.1f); vignette.smoothness.Override(.65f);
        foreach (var effect in profile.components) EditorUtility.SetDirty(effect);
        EditorUtility.SetDirty(profile); AssetDatabase.SaveAssetIfDirty(profile);
        var volume = root.GetComponent<Volume>(); if (!volume) volume = root.AddComponent<Volume>();
        volume.isGlobal = true; volume.priority = 10; volume.sharedProfile = profile;

        var probeObject = Node(root, "BakedReflection"); probeObject.transform.localPosition = new Vector3(0, 2.5f, 2);
        var reflection = probeObject.GetComponent<ReflectionProbe>(); if (!reflection) reflection = probeObject.AddComponent<ReflectionProbe>();
        reflection.enabled = false;
        reflection.mode = ReflectionProbeMode.Custom; reflection.resolution = 64; reflection.hdr = true;
        reflection.customBakedTexture = null;
        reflection.size = new Vector3(28, 10, 24); reflection.center = new Vector3(0, 1, 2);
        reflection.boxProjection = true; reflection.clearFlags = ReflectionProbeClearFlags.SolidColor;
        reflection.backgroundColor = new Color(.035f, .045f, .06f); reflection.intensity = .65f;

        var dust = Node(root, "SparseDust"); dust.transform.localPosition = new Vector3(.5f, 2.8f, 5.2f);
        var system = dust.GetComponent<ParticleSystem>(); if (!system) system = dust.AddComponent<ParticleSystem>();
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        system.useAutoRandomSeed = false; system.randomSeed = 1741;
        var main = system.main; main.loop = true; main.prewarm = true; main.startLifetime = 12;
        main.startSpeed = .045f; main.startSize = new ParticleSystem.MinMaxCurve(.018f, .035f);
        main.maxParticles = 32; main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startColor = new Color(.85f, .65f, .42f, .18f); main.gravityModifier = -.001f;
        var emission = system.emission; emission.rateOverTime = 1.8f;
        var shape = system.shape; shape.shapeType = ParticleSystemShapeType.Box; shape.scale = new Vector3(12, 2.6f, .8f);
        var color = system.colorOverLifetime; color.enabled = true;
        var gradient = new Gradient(); gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
            new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(.4f, .2f), new GradientAlphaKey(.4f, .75f), new GradientAlphaKey(0, 1) });
        color.color = gradient;
        var renderer = system.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = AtmosphereMaterial();
        renderer.sharedMaterial.shader=Shader.Find("Universal Render Pipeline/Particles/Simple Lit");
        EditorUtility.SetDirty(renderer.sharedMaterial);
        renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
        // A torch radiates locally; decorative shafts/haze do not simulate scattering.
        foreach(string name in new[]{"TorchBeam-1","TorchBeam1","OverheadBeam-1","OverheadBeam1","RearHaze","LowHaze"})
        {
            var node=root.transform.Find(name);
            if(node && node.TryGetComponent<Renderer>(out var old))old.enabled=false;
        }
        foreach(int side in new[]{-1,1})
        {
            var ray=root.transform.Find("TorchRay_"+side);
            if(ray && ray.TryGetComponent<LineRenderer>(out var old))old.enabled=false;
        }

    }
    internal static void Sheet(GameObject root,string name,Vector3 top,Vector3 bottom,float topWidth,float bottomWidth,Color tint,bool haze)
    {
        string meshPath=DungeonPixelRoomBuild.Art+name+".asset";
        var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if(!mesh){mesh=new Mesh{name=name};AssetDatabase.CreateAsset(mesh,meshPath);}
        mesh.Clear();
        mesh.vertices=new[]{top-Vector3.right*topWidth*.5f,top+Vector3.right*topWidth*.5f,
            bottom-Vector3.right*bottomWidth*.5f,bottom+Vector3.right*bottomWidth*.5f};
        mesh.uv=new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one};
        mesh.triangles=new[]{0,2,1,1,2,3};mesh.RecalculateBounds();
        EditorUtility.SetDirty(mesh);
        string materialPath=DungeonPixelRoomBuild.Art+name+".mat";
        var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if(!material){material=new Material(Shader.Find("CLACKBLADE/Dungeon Atmosphere"));AssetDatabase.CreateAsset(material,materialPath);}
        material.SetColor("_Tint",tint);material.SetFloat("_Haze",haze?1:0);
        material.SetFloat("_DstBlend",haze?(float)BlendMode.OneMinusSrcAlpha:(float)BlendMode.One);
        EditorUtility.SetDirty(material);
        var node=Node(root,name);node.transform.localPosition=Vector3.zero;
        var filter=node.GetComponent<MeshFilter>();if(!filter)filter=node.AddComponent<MeshFilter>();filter.sharedMesh=mesh;
        var renderer=node.GetComponent<MeshRenderer>();if(!renderer)renderer=node.AddComponent<MeshRenderer>();
        renderer.sharedMaterial=material;renderer.enabled=true;
        renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
        renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;
    }
    static T Effect<T>(VolumeProfile profile) where T : VolumeComponent
    {
        if (!profile.TryGet<T>(out var effect)) { effect = profile.Add<T>(); AssetDatabase.AddObjectToAsset(effect, profile); }
        return effect;
    }
    static GameObject Node(GameObject root, string name)
    {
        var child = root.transform.Find(name); if (child) return child.gameObject;
        var node = new GameObject(name); node.transform.SetParent(root.transform, false); return node;
    }
    static Material AtmosphereMaterial()
    {
        const string path = DungeonArenaBuild.Art + "DungeonAtmosphere.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!material)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Particles/Simple Lit"));
            AssetDatabase.CreateAsset(material, path);
        }
        material.SetFloat("_Surface", 1); material.SetFloat("_Blend", 0);
        material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        material.SetFloat("_ZWrite", 0); material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue = (int)RenderQueue.Transparent;
        EditorUtility.SetDirty(material); return material;
    }
}
