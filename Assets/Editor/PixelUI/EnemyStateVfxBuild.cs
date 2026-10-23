using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Only fills missing state-cue assets/references; authored tuning survives rebuilds.
public static class EnemyStateVfxBuild
{
    public const string Root = "Assets/Art/VFX/EnemyState/";
    const string GuardSource = "Assets/Art/VFX/Pimen/BattleEffects/Effect 2 - Sprite Sheet.png";
    const string DustSource = "Assets/Art/VFX/Pimen/SmokeNDust03/SmokeNDust P03 VFX 5.png";
    const string EnemyPath = "Assets/Characters/Enermy/Skeleton.prefab";

    public static void Prepare()
    {
        Directory.CreateDirectory(Root); AssetDatabase.Refresh();
        string guardSheet = Root + "GuardBreak-Sheet.png";
        if (!File.Exists(guardSheet))
        {
            // Original pack importer is a cubemap. Keep its GUID/settings untouched;
            // import a byte-identical copy for this new 2D flipbook consumer.
            File.Copy(GuardSource, guardSheet);
            AssetDatabase.ImportAsset(guardSheet, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(guardSheet);
            importer.textureType = TextureImporterType.Default; importer.textureShape = TextureImporterShape.Texture2D;
            importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None; importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
        Make("GuardBreak", guardSheet, 1, 5, .15f, 32, .5f, Vector3.zero);
        // Six visible source cels followed by two authored transparent cells.
        Make("BodyFallDust", DustSource, 8, 1, .24f, 24, 1, new Vector3(0, .3125f, 0));
        foreach (var form in new[] { "sword", "sword-shield", "axe" })
            Contact("Assets/Art/EndlessSkeletons/" + form + "/Die.anim", form == "axe" ? .5f : .7f);
        Contact("Assets/Scripts/skeleton_die.anim", .48333332f);
    }
    static void Contact(string path, float time)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        var events = AnimationUtility.GetAnimationEvents(clip);
        if (events.Any(e => e.functionName == "Anim_DeathImpact")) return;
        AnimationUtility.SetAnimationEvents(clip, events.Append(new AnimationEvent {
            time = time, functionName = "Anim_DeathImpact" }).OrderBy(e => e.time).ToArray());
        EditorUtility.SetDirty(clip);
    }
    static void Make(string name, string source, int columns, int rows, float duration, float width, float height, Vector3 pivot)
    {
        var profile = AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(Root + name + ".asset");
        bool created = !profile;
        if (created)
        {
            profile = ScriptableObject.CreateInstance<ImpactVfxProfile>();
            profile.lightEnabled = false;
            var gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            int count = columns * rows;
            var keys = new Keyframe[count + 1];
            for (int i = 0; i <= count; i++) keys[i] = new Keyframe((float)i / count,
                (float)Mathf.Min(i, count - 1) / count, float.PositiveInfinity, float.PositiveInfinity);
            profile.flashes = new[] { new ImpactVfxProfile.FlashLayer {
                channel = 0, sortingOrder = 3, lifetime = duration, sizePixels = width, heightScale = height,
                columns = columns, rows = rows, frameOverTime = new AnimationCurve(keys), colorOverLifetime = gradient } };
            AssetDatabase.CreateAsset(profile, Root + name + ".asset");
        }
        if (!profile.physics)
        {
            var physics = AssetDatabase.LoadAssetAtPath<PhysicalParticleProfile>(Root + "FlashOnly.asset");
            if (!physics) { physics = ScriptableObject.CreateInstance<PhysicalParticleProfile>(); AssetDatabase.CreateAsset(physics, Root + "FlashOnly.asset"); }
            profile.physics = physics; EditorUtility.SetDirty(profile);
        }
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + name + ".mat");
        if (!material)
        {
            material = new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat"));
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(source);
            if (!material.mainTexture) throw new System.InvalidOperationException("Missing stock VFX: " + source +
                "; guid=" + AssetDatabase.AssetPathToGUID(source) + "; assets=" + string.Join(",", AssetDatabase.LoadAllAssetsAtPath(source).Select(a => a.GetType().Name)));
            AssetDatabase.CreateAsset(material, Root + name + ".mat");
        }
        if (!profile.prefab)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + name + ".prefab");
            if (!prefab)
            {
                var root = new GameObject(name); root.SetActive(false);
                try
                {
                    root.AddComponent<PooledVFX>(); var burst = root.AddComponent<ImpactVfxBurst>();
                    var child = new GameObject("Flipbook"); child.transform.SetParent(root.transform, false);
                    var ps = child.AddComponent<ParticleSystem>();
                    var main = ps.main; main.playOnAwake = false; main.loop = false; main.maxParticles = 1;
                    main.simulationSpace = ParticleSystemSimulationSpace.World; main.useUnscaledTime = true;
                    main.startSpeed = 0;
                    var emission = ps.emission; emission.enabled = false;
                    var shape = ps.shape; shape.enabled = false;
                    var renderer = ps.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = material;
                    renderer.pivot = pivot; renderer.sortingOrder = 3;
                    var so = new SerializedObject(burst);
                    so.FindProperty("impactProfile").objectReferenceValue = profile;
                    var list = so.FindProperty("rendererBindings"); list.arraySize = 1;
                    var binding = list.GetArrayElementAtIndex(0);
                    binding.FindPropertyRelative("id").intValue = 0;
                    binding.FindPropertyRelative("particles").objectReferenceValue = ps;
                    binding.FindPropertyRelative("material").objectReferenceValue = material;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    prefab = PrefabUtility.SaveAsPrefabAsset(root, Root + name + ".prefab");
                }
                finally { Object.DestroyImmediate(root); }
            }
            profile.prefab = prefab; EditorUtility.SetDirty(profile);
        }
    }
    public static void Configure(GameObject root, ImpactVfxEmitter deathEmitter = null)
    {
        foreach (var actor in root.GetComponentsInChildren<SkeletonController>(true))
        {
            var binder = actor.GetComponent<EnemyStateVfxBinder>();
            if (!binder) binder = actor.gameObject.AddComponent<EnemyStateVfxBinder>();
            var emitter = actor.GetComponent<ImpactVfxEmitter>();
            if (!emitter) { emitter = actor.gameObject.AddComponent<ImpactVfxEmitter>(); Set(emitter, "maxActive", 2); }
            var body = actor.GetComponent<BoxCollider2D>();
            var anchor = actor.transform.Find("DeathImpactAnchor");
            if (!anchor)
            {
                anchor = new GameObject("DeathImpactAnchor").transform; anchor.SetParent(actor.transform, false);
                anchor.localPosition = body ? (Vector3)body.offset : Vector3.zero;
            }
            Missing(binder, "health", actor); Missing(binder, "stamina", actor.GetComponent<EnemyStamina>());
            Missing(binder, "stun", actor.GetComponent<EnemyStunController>()); Missing(binder, "body", body);
            Missing(binder, "deathAnchor", anchor); Missing(binder, "guardEmitter", emitter);
            bool bindingScene = deathEmitter && !new SerializedObject(binder).FindProperty("deathEmitter").objectReferenceValue;
            Missing(binder, "deathEmitter", deathEmitter);
            Missing(binder, "guardBreak", AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(Root + "GuardBreak.asset"));
            Missing(binder, "bodyFallDust", AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(Root + "BodyFallDust.asset"));
            // Existing scene floor landmark; actor transform/offset works even for inactive colliders.
            var so = new SerializedObject(binder);
            var floor = so.FindProperty("floorY");
            if (body && (floor.floatValue == 0 || (bindingScene && PrefabUtility.IsPartOfPrefabInstance(binder) && !floor.prefabOverride)))
                Set(binder, "floorY", body.transform.TransformPoint(body.offset).y - body.size.y * Mathf.Abs(body.transform.lossyScale.y) * .5f);
        }
    }
    static void Missing(Object owner, string name, Object value)
    {
        var so = new SerializedObject(owner); var field = so.FindProperty(name);
        if (field.objectReferenceValue || !value) return;
        field.objectReferenceValue = value; so.ApplyModifiedPropertiesWithoutUndo(); Record(owner);
    }
    static void Set(Object owner, string name, float value)
    {
        var so = new SerializedObject(owner); var field = so.FindProperty(name);
        if (field.propertyType == SerializedPropertyType.Integer) field.intValue = (int)value; else field.floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo(); Record(owner);
    }
    static void Record(Object owner)
    {
        EditorUtility.SetDirty(owner);
        if (PrefabUtility.IsPartOfPrefabInstance(owner)) PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
    }
    public static void BindScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();
        var emitter = roots.SelectMany(r => r.GetComponentsInChildren<ImpactVfxEmitter>(true))
            .FirstOrDefault(e => e.name == "EnemyDeathVfx");
        if (!emitter)
        {
            var go = new GameObject("EnemyDeathVfx"); emitter = go.AddComponent<ImpactVfxEmitter>(); Set(emitter, "maxActive", 2);
            var gate = roots.SelectMany(r => r.GetComponentsInChildren<DungeonSessionBindings>(true)).FirstOrDefault();
            if (gate && !gate.gameObject.activeSelf)
            {
                go.transform.SetParent(gate.transform, true); gate.roots = gate.roots.Append(go).ToArray(); Record(gate);
            }
        }
        foreach (var root in scene.GetRootGameObjects()) Configure(root, emitter);
    }
    public static void SerializeOnly()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            Prepare();
            var root = PrefabUtility.LoadPrefabContents(EnemyPath);
            try { Configure(root); PrefabUtility.SaveAsPrefabAsset(root, EnemyPath); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity"); BindScene();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
        }
        finally { if (setup.Any(s => s.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup); }
        Debug.Log("Build succeeded!");
    }
}
