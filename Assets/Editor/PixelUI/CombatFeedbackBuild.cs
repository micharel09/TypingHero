using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static PixelUIFactory;

// Presentation-only settings, applied after core match and before the UI skin.
internal static class CombatFeedbackBuild
{
    const string Events = "Assets/Audio/SFX/Events/Combat/Player/";
    const string Audio = "Assets/Audio/SFX/Clips/KenneyImpact/";

    internal static void Apply()
    {
        CombatFeedbackAssets.Prepare();
        ParryClashAssets.Prepare();
        ParryImpactAssets.Prepare();
        CombatVoxelAssets.Prepare();
        StrikeAccentAssets.Prepare();
        NormalHitSparkAssets.Prepare();
        SlayerHitVfxAssets.Prepare();
        SlashVolumeAssets.Prepare();
        var normal = ImpactEvent("ev_impact_normal", "medium", .65f);
        var slayer = ImpactEvent("ev_impact_slayer", "medium", .45f);
        var lethal = ImpactEvent("ev_impact_lethal", "heavy", .8f);
        var clang = AssetDatabase.LoadAssetAtPath<SfxEvent>(Events + "Parry/ev_parry_clang.asset");
        Value(clang, "pitchMode", 0); Value(clang, "pitchSemitone", 0f);
        Value(clang, "pitchJitterSemitone", .35f);
        Value(clang, "volumeJitter", .03f);
        Vector(clang, "pitchSemitoneRange", new Vector2(-1, 1));

        foreach (var path in new[] { "Assets/Characters/Player/Player.prefab", "Assets/Characters/Enermy/Skeleton.prefab" })
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Configure(root, normal, slayer, lethal);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            Configure(root, normal, slayer, lethal);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void BindParryProfile(ParrySparkVFX_Pooled parry)
    {
        var profile = Get<ImpactVfxProfile>(parry, "impactProfile");
        if (profile && profile == AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(ParryClashAssets.Root + "MetalClash.asset") &&
            Get<GameObject>(parry, "sparkPrefab") == ParryClashAssets.Load())
            Set(parry, "impactProfile", AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(ParryClashAssets.Root + "ParryImpact.asset"));
        else if (!profile)
        {
            var prefab = Get<GameObject>(parry, "sparkPrefab");
            var template = prefab ? prefab.GetComponent<ImpactVfxBurst>() : null;
            if (template && template.impactProfile)
                Set(parry, "impactProfile", prefab == ParryClashAssets.Load()
                    ? AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(ParryClashAssets.Root + "ParryImpact.asset")
                    : template.impactProfile);
        }
    }

    static SfxEvent ImpactEvent(string name, string kind, float gain)
    {
        string path = Events + name + ".asset";
        var ev = AssetDatabase.LoadAssetAtPath<SfxEvent>(path);
        if (!ev) { ev = ScriptableObject.CreateInstance<SfxEvent>(); AssetDatabase.CreateAsset(ev, path); }
        var so = new SerializedObject(ev);
        var clips = so.FindProperty("clips"); clips.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Audio + $"impactWood_{kind}_{i:000}.ogg");
            if (!clip) throw new System.InvalidOperationException("Missing original Kenney impact clip: " + kind + i);
            clips.GetArrayElementAtIndex(i).objectReferenceValue = clip;
        }
        var whoosh = AssetDatabase.LoadAssetAtPath<SfxEvent>(Events + "sfx_combat_whoosh_light_01.asset");
        so.FindProperty("mixerGroup").objectReferenceValue = new SerializedObject(whoosh).FindProperty("mixerGroup").objectReferenceValue;
        so.ApplyModifiedPropertiesWithoutUndo();
        Value(ev, "volume", gain); Value(ev, "volumeJitter", 0f);
        Value(ev, "pitchMode", 0); Value(ev, "pitchSemitone", 0f); Value(ev, "pitchJitterSemitone", .5f);
        Value(ev, "maxVoices", 8); Value(ev, "cooldown", 0f);
        return ev;
    }

    static void Configure(GameObject root, SfxEvent normal, SfxEvent slayer, SfxEvent lethal)
    {
        // Remove the retired damage route before configuring the outgoing hit spawners.
        foreach (var damage in root.GetComponentsInChildren<PlayerDamageVoxel>(true))
            Object.DestroyImmediate(damage);
        foreach (var actor in root.GetComponentsInChildren<PlayerHealth>(true))
        {
            var received = actor.transform.Find("ReceivedImpact");
            if (received) Object.DestroyImmediate(received.gameObject);
        }
        var shake = root.GetComponentInChildren<ParryCameraShake>(true);
        var previousImpulse = shake ? Get<Cinemachine.CinemachineImpulseSource>(shake, "impulseSource") : null;
        if (shake)
        {
            var child = shake.transform.Find("ParryRecoil");
            if (!child)
            {
                child = new GameObject("ParryRecoil").transform; child.SetParent(shake.transform, false);
                var source = child.gameObject.AddComponent<Cinemachine.CinemachineImpulseSource>();
                source.m_ImpulseDefinition.m_ImpulseChannel = 2;
                source.m_ImpulseDefinition.m_ImpulseType = Cinemachine.CinemachineImpulseDefinition.ImpulseTypes.Uniform;
                Set(shake, "impulseSource", source); Value(shake, "power", 1f); Record(shake);
            }
            Set(shake, "impulseSource", child.GetComponent<Cinemachine.CinemachineImpulseSource>()); Record(shake);
        }
        foreach (var box in root.GetComponentsInChildren<PlayerWeaponHitbox>(true))
        {
            Set(box, "slashLibrary", SlashVolumeAssets.Library());
            Set(box, "slashSource", box.GetComponentInParent<PlayerHealth>().GetComponent<SpriteRenderer>());
            Record(box);
        }
        foreach (var spark in root.GetComponentsInChildren<HitSparkSpawner>(true))
        {
            Set(spark, "sparkPrefab", NormalHitSparkAssets.Load());
            Set(spark, "lethalPrefab", CombatVoxelAssets.Load("Lethal"));
            Set(spark, "slayerLethalPrefab", SlayerHitVfxAssets.Load(true));
            Value(spark, "groundY", Ground(root));
            var profiles = new SerializedObject(spark);
            var variants = profiles.FindProperty("slayerPrefabs"); variants.arraySize = 1;
            variants.GetArrayElementAtIndex(0).objectReferenceValue = SlayerHitVfxAssets.Load(false);
            profiles.ApplyModifiedPropertiesWithoutUndo();
            Value(spark, "zRotationOffset", 0f); Vector(spark, "angleJitter", Vector2.zero);
            Set(spark, "normalImpact", normal); Set(spark, "slayerImpact", slayer); Set(spark, "lethalImpact", lethal);
            Value(spark, "parentUnderAttacker", false);
            Vector(spark, "worldOffset", Vector3.zero); Vector(spark, "verticalJitter", new Vector2(-.03f, .03f));
            Vector(spark, "scaleRange", Vector2.one);
            var impulse = Get<Cinemachine.CinemachineImpulseSource>(spark, "lethalImpulse");
            if (!impulse && previousImpulse && previousImpulse.name != "ParryRecoil") impulse = previousImpulse;
            if (impulse && impulse.name != "ParryRecoil")
            {
                var impulseSettings = new SerializedObject(impulse);
                impulseSettings.FindProperty("m_ImpulseDefinition.m_ImpulseDuration").floatValue = .1f;
                impulseSettings.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(impulse); Set(spark, "lethalImpulse", impulse);
                Record(impulse);
            }
            Record(spark);
        }
        foreach (var actor in root.GetComponentsInChildren<PlayerHealth>(true))
        {
            var accent = actor.GetComponent<NormalStrikeEffects>();
            if (!accent) accent = actor.gameObject.AddComponent<NormalStrikeEffects>();
            Set(accent, "attackEvents", actor.GetComponentInChildren<PlayerAttackEvents>(true));
            Set(accent, "slayer", actor.GetComponentInChildren<PlayerSlayerMode>(true));
            Set(accent, "emitter", actor.GetComponentInChildren<HitSparkSpawner>(true));
            Set(accent, "body", actor.GetComponent<BoxCollider2D>());
            Set(accent, "dustPrefab", StrikeAccentAssets.Load(true));
            Value(accent, "groundY", Ground(actor.gameObject)); Record(accent);
        }
        foreach (var parry in root.GetComponentsInChildren<ParrySparkVFX_Pooled>(true))
        {
            if (!Get<GameObject>(parry, "sparkPrefab")) Set(parry, "sparkPrefab", ParryClashAssets.Load());
            BindParryProfile(parry);
            var system = Get<ParrySystem>(parry, "parrySystem");
            if (system) { Set(system, "contactLibrary", ParryPixelAssets.Contacts()); Record(system); }
            foreach (var audio in parry.GetComponentsInParent<SfxOnParrySuccess>(true)) { Value(audio, "duckSeconds", .08f); Record(audio); }
            Value(parry, "sparkPreset", (int)ParryClashBurst.Preset.BCompact);
            Value(parry, "groundY", Ground(root));
            var anchor = Get<Transform>(parry, "playerSparkAnchor");
            if (anchor)
            {
                // Existing anchor is under Attack1; align its visual with the sword crossing.
                anchor.localPosition = new Vector3(-.55f, 1f, anchor.localPosition.z);
                Record(anchor);
            }
            Value(parry, "parentToAnchor", false); Record(parry);
        }
        foreach (var images in root.GetComponentsInChildren<SlayerAfterimagePool>(true))
        {
            Value(images, "burstCount", 3); Value(images, "burstInterval", .025f);
            Value(images, "life", .12f); Value(images, "lifeStep", .02f);
            Value(images, "alphaFalloff", .6f); Value(images, "freezeFadeOnPause", true);
            var so = new SerializedObject(images); var tint = so.FindProperty("baseTint").colorValue;
            tint.a = .4f; so.FindProperty("baseTint").colorValue = tint; so.ApplyModifiedPropertiesWithoutUndo();
            Record(images);
        }
        foreach (var flash in root.GetComponentsInChildren<EnemyHitFlash>(true))
        {
            Value(flash, "flashColor", new Color(1f, .96f, .85f, 1));
            Value(flash, "baseDuration", .06f); Value(flash, "minFrames", 2); Value(flash, "useUnscaledTime", true);
            Record(flash);
        }
        foreach (var renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (!renderer.GetComponent<Animator>()) continue;
            if (!renderer.GetComponent<PlayerHealth>() && !renderer.GetComponent<SkeletonController>()) continue;
            var slash = renderer.GetComponent<SlashVolume>(); if (!slash) slash = renderer.gameObject.AddComponent<SlashVolume>();
            Set(slash, "source", renderer); Set(slash, "library", SlashVolumeAssets.Library());
            Set(slash, "normalPlayerMaterial", renderer.GetComponent<PlayerHealth>() ? SlashVolumeAssets.NormalPlayerMaterial() : null); Record(slash);
        }
    }
    static void Vector(Object target, string name, Vector3 value)
    {
        var so = new SerializedObject(target); var p = so.FindProperty(name);
        if (p.propertyType == SerializedPropertyType.Vector2) p.vector2Value = value;
        else p.vector3Value = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
    static float Ground(GameObject root)
    {
        var body = root.GetComponentInChildren<BoxCollider2D>();
        // Same fixed standing-body landmark as CombatHUDLayout, independent of sprites.
        return body.transform.TransformPoint(body.offset).y - body.size.y * Mathf.Abs(body.transform.lossyScale.y) * .5f + .12f;
    }
    static void Record(Object target)
    {
        EditorUtility.SetDirty(target);
        if (PrefabUtility.IsPartOfPrefabInstance(target)) PrefabUtility.RecordPrefabInstancePropertyModifications(target);
    }
}
