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
        PixelSparkAssets.Prepare();
        var normal = ImpactEvent("ev_impact_normal", "medium", .65f);
        var slayer = ImpactEvent("ev_impact_slayer", "medium", .45f);
        var lethal = ImpactEvent("ev_impact_lethal", "heavy", .8f);
        var clang = AssetDatabase.LoadAssetAtPath<SfxEvent>(Events + "Parry/ev_parry_clang.asset");
        Value(clang, "pitchMode", 0); Value(clang, "pitchSemitone", 0f);
        Value(clang, "pitchJitterSemitone", 1f);
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
        foreach (var spark in root.GetComponentsInChildren<HitSparkSpawner>(true))
        {
            Set(spark, "sparkPrefab", PixelSparkAssets.Load("Normal"));
            Set(spark, "lethalPrefab", PixelSparkAssets.Load("Lethal"));
            Value(spark, "groundY", Ground(root));
            var profiles = new SerializedObject(spark);
            var variants = profiles.FindProperty("slayerPrefabs"); variants.arraySize = 1;
            variants.GetArrayElementAtIndex(0).objectReferenceValue = PixelSparkAssets.Load("Slayer");
            profiles.ApplyModifiedPropertiesWithoutUndo();
            Value(spark, "zRotationOffset", 0f); Vector(spark, "angleJitter", Vector2.zero);
            Set(spark, "normalImpact", normal); Set(spark, "slayerImpact", slayer); Set(spark, "lethalImpact", lethal);
            Value(spark, "parentUnderAttacker", false);
            Vector(spark, "worldOffset", Vector3.zero); Vector(spark, "verticalJitter", new Vector2(-.03f, .03f));
            Vector(spark, "scaleRange", Vector2.one);
            var shake = root.GetComponentInChildren<ParryCameraShake>(true);
            var impulse = shake ? Get<Component>(shake, "impulseSource") : null;
            if (impulse)
            {
                var impulseSettings = new SerializedObject(impulse);
                impulseSettings.FindProperty("m_ImpulseDefinition.m_ImpulseDuration").floatValue = .1f;
                impulseSettings.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(impulse); Set(spark, "lethalImpulse", impulse);
                Value(shake, "power", .01f);
                Record(impulse); Record(shake);
            }
            Record(spark);
        }
        foreach (var parry in root.GetComponentsInChildren<ParrySparkVFX_Pooled>(true))
        {
            Set(parry, "sparkPrefab", PixelSparkAssets.Load("Parry"));
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
