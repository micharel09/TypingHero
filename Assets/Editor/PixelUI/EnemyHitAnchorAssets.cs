using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class EnemyHitAnchorAssets : IProcessSceneWithReport
{
    public int callbackOrder => 0;
    internal static EnemyHitAnchorProfile Load(string form) =>
        AssetDatabase.LoadAssetAtPath<EnemyHitAnchorProfile>("Assets/Art/EnemyHitAnchors/" + form + ".asset");

    internal static void Bind(SkeletonController actor)
    {
        var anchor = actor.GetComponent<EnemyHitAnchor>();
        if (anchor) return;
        // Only the existing tutorial skeleton is migrated automatically.
        var profile = Load("ske");
        var body = actor.GetComponent<SpriteRenderer>();
        if (!body || !profile || !profile.TryGet(body.sprite, out _))
            throw new BuildFailedException("Configure EnemyHitAnchor explicitly: " + actor.name);
        anchor = actor.gameObject.AddComponent<EnemyHitAnchor>();
        var so = new SerializedObject(anchor);
        so.FindProperty("body").objectReferenceValue = body;
        so.FindProperty("profile").objectReferenceValue = profile;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(anchor);
    }

    internal static void Validate(EnemyHitAnchorProfile profile, RuntimeAnimatorController controller, Sprite idle, string owner, string bodyPath = "")
    {
        if (!profile || !controller || !idle) throw new BuildFailedException("Missing chest anchor data: " + owner);
        if (profile.poses == null || profile.poses.Any(p => !p.sprite ||
            float.IsNaN(p.chest.x) || float.IsNaN(p.chest.y) || float.IsInfinity(p.chest.x) || float.IsInfinity(p.chest.y) ||
            p.chest.x < 0 || p.chest.y < 0 || p.chest.x > p.sprite.rect.width || p.chest.y > p.sprite.rect.height) ||
            profile.poses.Select(p => p.sprite).Distinct().Count() != profile.poses.Length)
            throw new BuildFailedException("Invalid/duplicate chest anchor pose: " + owner);
        var required = controller.animationClips.SelectMany(clip => AnimationUtility.GetObjectReferenceCurveBindings(clip)
            .Where(b => b.path == bodyPath && b.type == typeof(SpriteRenderer) && b.propertyName == "m_Sprite")
            .SelectMany(b => AnimationUtility.GetObjectReferenceCurve(clip, b)).Select(k => k.value).OfType<Sprite>())
            .Append(idle).Distinct().ToArray();
        foreach (var sprite in required)
            if (!profile.TryGet(sprite, out _)) throw new BuildFailedException("Missing chest anchor: " + owner + "/" + sprite.name);
        // Parry temporarily assigns contact cels outside Animator curves.
        var contacts = AssetDatabase.LoadAssetAtPath<ParryContactLibrary>(ParryClashAssets.Root + "ParryContacts.asset");
        if (contacts)
            foreach (var frame in contacts.frames.Where(f => !f.defense && required.Contains(f.sprite) && f.contactCel))
                if (!profile.TryGet(frame.contactCel, out _))
                    throw new BuildFailedException("Missing held-parry chest anchor: " + owner + "/" + frame.contactCel.name);
    }

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        foreach (var root in scene.GetRootGameObjects()) ValidateRoot(root);
        foreach (var path in AssetDatabase.GetDependencies(scene.path).Where(path => path.EndsWith(".prefab")))
            ValidateRoot(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        foreach (var role in Resources.LoadAll<EndlessOpponent>("Endless"))
            Validate(role.hitAnchor, role.controller, role.idle, role.name);
    }

    static void ValidateRoot(GameObject root)
    {
        foreach (var actor in root.GetComponentsInChildren<MonoBehaviour>(true)
            .Where(actor => actor is IDamageable && !actor.GetComponent<PlayerHealth>()))
        {
            var anchor = actor.GetComponent<EnemyHitAnchor>();
            var animator = actor.GetComponent<Animator>();
            if (!anchor || !anchor.Body || !animator) throw new BuildFailedException("Missing enemy hit anchor/body: " + actor.name);
            if (!anchor.Body.transform.IsChildOf(animator.transform))
                throw new BuildFailedException("Enemy hit anchor body must be under its Animator: " + actor.name);
            Validate(anchor.Profile, animator.runtimeAnimatorController, anchor.Body.sprite, actor.name,
                AnimationUtility.CalculateTransformPath(anchor.Body.transform, animator.transform));
        }
    }
}
