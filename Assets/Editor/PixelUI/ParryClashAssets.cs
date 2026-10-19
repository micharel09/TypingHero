using System.IO;
using UnityEditor;
using UnityEngine;

internal static class ParryClashAssets
{
    internal const string Root = "Assets/Art/VFX/ParryClash/";
    internal static GameObject Load() => AssetDatabase.LoadAssetAtPath<GameObject>(Root + "ParryClash.prefab");
    internal static void Prepare()
    {
        Directory.CreateDirectory(Root);
        AssetDatabase.Refresh();
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "Clash.mat");
        if (!material)
        {
            material = new Material(Shader.Find("TypingHero/ParryClash"));
            AssetDatabase.CreateAsset(material, Root + "Clash.mat");
        }
        if (Load())
        {
            var existing = PrefabUtility.LoadPrefabContents(Root + "ParryClash.prefab");
            try
            {
                var serialized = new SerializedObject(existing.GetComponent<ParryClashBurst>());
                if (serialized.FindProperty("mainRays").objectReferenceValue)
                {
                    bool migrated = MigrateRayShape(existing, serialized);
                    migrated = MigrateCompact(existing, serialized) || migrated;
                    if (MigrateLengthSupport(existing, serialized) || migrated)
                        PrefabUtility.SaveAsPrefabAsset(existing, Root + "ParryClash.prefab");
                    return;
                }
                var rays = RayMaterial("Rays", 1);
                var main = Group(existing, "MainRays", rays, 31, true, false);
                TuneRay(main, true);
                foreach (string name in new[] { "Front", "Back" })
                {
                    var ps = existing.transform.Find(name).GetComponent<ParticleSystem>();
                    ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = rays;
                    TuneRay(ps, false);
                }
                existing.transform.Find("Embers").GetComponent<ParticleSystemRenderer>().sharedMaterial = RayMaterial("Ember", 2);
                serialized.FindProperty("mainRays").objectReferenceValue = main;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                MigrateRayShape(existing, serialized);
                MigrateLengthSupport(existing, serialized);
                MigrateCompact(existing, serialized);
                PrefabUtility.SaveAsPrefabAsset(existing, Root + "ParryClash.prefab");
            }
            finally { PrefabUtility.UnloadPrefabContents(existing); }
            return;
        }
        var root = new GameObject("ParryClash");
        root.SetActive(false);
        try
        {
            root.AddComponent<PooledVFX>();
            var burst = root.AddComponent<ParryClashBurst>();
            var contact = Group(root, "Contact", material, 30, false, false);
            var rays = RayMaterial("Rays", 1);
            var main = Group(root, "MainRays", rays, 31, true, false);
            var front = Group(root, "Front", rays, 31, true, false);
            var back = Group(root, "Back", rays, -1, true, false);
            var embers = Group(root, "Embers", RayMaterial("Ember", 2), 30, false, true);
            TuneRay(main, true); TuneRay(front, false); TuneRay(back, false);
            var ground = new GameObject("Ground").transform; ground.SetParent(root.transform, false);
            var collision = embers.collision; collision.enabled = true;
            collision.type = ParticleSystemCollisionType.Planes; collision.SetPlane(0, ground);
            collision.mode = ParticleSystemCollisionMode.Collision3D; collision.bounce = 0; collision.lifetimeLoss = 1;
            var so = new SerializedObject(burst);
            so.FindProperty("ground").objectReferenceValue = ground;
            so.FindProperty("contact").objectReferenceValue = contact;
            so.FindProperty("mainRays").objectReferenceValue = main;
            so.FindProperty("front").objectReferenceValue = front;
            so.FindProperty("back").objectReferenceValue = back;
            so.FindProperty("embers").objectReferenceValue = embers;
            so.ApplyModifiedPropertiesWithoutUndo();
            MigrateRayShape(root, so);
            MigrateLengthSupport(root, so);
            MigrateCompact(root, so);
            PrefabUtility.SaveAsPrefabAsset(root, Root + "ParryClash.prefab");
        }
        finally { Object.DestroyImmediate(root); }
    }
    static bool MigrateCompact(GameObject root, SerializedObject so)
    {
        if (so.FindProperty("compactVersion").intValue >= 1) return false;
        var material = RayMaterial("Compact", 1);
        material.SetFloat("_Pixel", 1); EditorUtility.SetDirty(material);
        so.FindProperty("compactMaterial").objectReferenceValue = material;
        var ps = root.transform.Find("Embers").GetComponent<ParticleSystem>();
        ps.GetComponent<ParticleSystemRenderer>().SetActiveVertexStreams(
            new System.Collections.Generic.List<ParticleSystemVertexStream> {
                ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color,
                ParticleSystemVertexStream.UV, ParticleSystemVertexStream.UV2, ParticleSystemVertexStream.Custom1XYZW });
        so.FindProperty("compactVersion").intValue = 1;
        so.ApplyModifiedPropertiesWithoutUndo();
        return true;
    }
    static bool MigrateRayShape(GameObject root, SerializedObject so)
    {
        if (so.FindProperty("rayShapeVersion").intValue >= 1) return false;
        so.FindProperty("mainSettings").FindPropertyRelative("width").floatValue = .1875f;
        so.FindProperty("frontSettings").FindPropertyRelative("width").floatValue = .09375f;
        foreach (string name in new[] { "MainRays", "Front", "Back" })
        {
            var ps = root.transform.Find(name).GetComponent<ParticleSystem>();
            // Explicit data survives simulation without a curve rewriting it every frame.
            var custom = ps.customData; custom.enabled = false;
            ps.GetComponent<ParticleSystemRenderer>().SetActiveVertexStreams(
                new System.Collections.Generic.List<ParticleSystemVertexStream> {
                    ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Color,
                    ParticleSystemVertexStream.UV, ParticleSystemVertexStream.UV2, ParticleSystemVertexStream.Custom1XYZW });
            // Reserve 15% at the tail for independent length variation.
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.lengthScale *= 1.15f;
            renderer.velocityScale *= 1.15f;
        }
        so.FindProperty("rayShapeVersion").intValue = 1;
        so.ApplyModifiedPropertiesWithoutUndo();
        return true;
    }
    static bool MigrateLengthSupport(GameObject root, SerializedObject so)
    {
        if (so.FindProperty("lengthSupportVersion").intValue >= 1) return false;
        foreach (string name in new[] { "MainRays", "Front", "Back" })
        {
            var renderer = root.transform.Find(name).GetComponent<ParticleSystemRenderer>();
            renderer.lengthScale *= 1.30f / 1.15f;
            renderer.velocityScale *= 1.30f / 1.15f;
        }
        so.FindProperty("lengthSupportVersion").intValue = 1;
        so.ApplyModifiedPropertiesWithoutUndo();
        return true;
    }
    static Material RayMaterial(string name, float role)
    {
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + name + ".mat");
        if (material) return material;
        material = new Material(Shader.Find("TypingHero/ParryClash"));
        material.SetFloat("_Role", role);
        AssetDatabase.CreateAsset(material, Root + name + ".mat");
        return material;
    }
    static void TuneRay(ParticleSystem ps, bool mainRay)
    {
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.lengthScale = mainRay ? 1.8f : 2f;
        renderer.velocityScale = mainRay ? .022f : .025f;
        // Hold a readable width, step once, then extinguish instead of shrinking to hair.
        var curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(.65f, 1),
            new Keyframe(.66f, .5f), new Keyframe(1, .5f));
        for (int i = 0; i < curve.length; i++) AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
        var size = ps.sizeOverLifetime; size.size = new ParticleSystem.MinMaxCurve(1, curve);
    }
    static ParticleSystem Group(GameObject parent, string name, Material material, int order, bool stretch, bool gravity)
    {
        var go = new GameObject(name); go.transform.SetParent(parent.transform, false);
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main; main.loop = false; main.duration = .01f; main.playOnAwake = false;
        main.useUnscaledTime = true; main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 64; main.startSpeed = 0; main.gravityModifier = gravity ? 1f : 0f;
        main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
        var emission = ps.emission; emission.enabled = false;
        var shape = ps.shape; shape.enabled = false;
        var limit = ps.limitVelocityOverLifetime; limit.enabled = true; limit.limit = 2f; limit.dampen = .22f;
        var size = ps.sizeOverLifetime; size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, 1, 1, 0));
        var color = ps.colorOverLifetime; color.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(new Color(1,.8f,.22f),.35f),
            new GradientColorKey(new Color(1,.25f,.015f),1) },
            new[] { new GradientAlphaKey(1,0), new GradientAlphaKey(1,.55f), new GradientAlphaKey(0,1) });
        color.color = gradient;
        var renderer = go.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = material;
        renderer.renderMode = stretch ? ParticleSystemRenderMode.Stretch : ParticleSystemRenderMode.Billboard;
        renderer.velocityScale = .025f; renderer.lengthScale = 3f; renderer.sortingOrder = order;
        renderer.maxParticleSize = 1;
        return ps;
    }
}
