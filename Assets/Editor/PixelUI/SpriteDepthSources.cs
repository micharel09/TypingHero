using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only source discovery. Scene instances own overrides; prefabs alone cannot describe them.
internal sealed class SpriteDepthSources
{
    internal sealed class Curve
    {
        internal AnimationClip clip;
        internal EditorCurveBinding binding;
    }
    internal readonly List<Curve> curves = new List<Curve>();
    readonly HashSet<(AnimationClip, string)> curveKeys = new HashSet<(AnimationClip, string)>();
    internal readonly HashSet<Sprite> sprites = new HashSet<Sprite>();

    internal static SpriteDepthSources Collect()
    {
        var result = new SpriteDepthSources();
        var dependencies = LightingBuildValidation.Dependencies();
        foreach (var prefab in dependencies.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(x => x))
            result.ReadActors(prefab);
        foreach (var path in dependencies.Where(x => x.EndsWith(".unity", StringComparison.Ordinal)).Distinct())
        {
            var scene = SceneManager.GetSceneByPath(path);
            if (scene.IsValid() && scene.isLoaded) result.ReadScene(scene);
            else
            {
                // Opening saved preview scenes is not a public API in Unity 2022.3.
                // Use a temporary additive scene; preserve any existing unloaded entry.
                bool existingEntry = scene.IsValid();
                var active = SceneManager.GetActiveScene();
                var temporary = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try { result.ReadScene(temporary); }
                finally
                {
                    EditorSceneManager.CloseScene(temporary, !existingEntry);
                    if (active.IsValid() && active.isLoaded) SceneManager.SetActiveScene(active);
                }
            }
        }
        return result;
    }

    void ReadScene(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects()) ReadActors(root);
    }

    void ReadActors(GameObject root)
    {
        var actors = root.GetComponentsInChildren<PlayerHealth>(true).Cast<Component>()
            .Concat(root.GetComponentsInChildren<SkeletonController>(true));
        var visited = new HashSet<Component>();
        foreach (var actor in actors)
        {
            foreach (var component in actor.GetComponentsInChildren<Component>(true))
                ReadComponent(component, visited);
            if (actor is SkeletonController)
            {
                var animator = AnimatorFor(actor);
                foreach (var role in Enum.GetValues(typeof(EndlessRole)).Cast<EndlessRole>())
                    ReadOpponent(Resources.Load<EndlessOpponent>("Endless/" + role), animator);
            }
        }
        foreach (var binding in root.GetComponentsInChildren<SpriteDepthBinding>(true))
        {
            ReadComponent(binding.GetComponent<SpriteRenderer>(), visited);
        }
    }

    void ReadComponent(Component component, HashSet<Component> visited)
    {
        if (!component || !visited.Add(component)) return;
        if (component is SpriteRenderer renderer)
        {
            // Do not read m_Sprite again through SerializedObject on excluded receivers.
            if (!IsReceiver(renderer)) return;
            AddSprite(renderer.sprite);
            ReadComponent(renderer.GetComponentInParent<Animator>(true), visited);
            return;
        }
        var animator = component as Animator ?? AnimatorFor(component);
        if (component is Animator owner) ReadController(owner.runtimeAnimatorController, owner);
        using (var serialized = new SerializedObject(component))
        {
            var property = serialized.GetIterator();
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                var reference = property.objectReferenceValue;
                // Persistent prefab component references need the same classification as scene references.
                if (reference is Animator || reference is SpriteRenderer)
                    ReadComponent((Component)reference, visited);
                else if (reference is AnimationClip clip) ReadClip(clip, animator);
                else if (reference is RuntimeAnimatorController controller) ReadController(controller, animator);
                else if (reference is EndlessOpponent opponent) ReadOpponent(opponent, animator);
                else if (reference is ParryContactLibrary contacts)
                    foreach (var frame in contacts.frames)
                    {
                        AddSprite(frame.sprite); AddSprite(frame.contactCel); AddSprite(frame.defenseCel);
                    }
            }
        }
    }

    static bool IsReceiver(SpriteRenderer renderer) => renderer &&
        (renderer.GetComponent<SpriteDepthBinding>() || renderer.GetComponent<PlayerHealth>() ||
         renderer.GetComponent<SkeletonController>());

    static Animator AnimatorFor(Component component)
    {
        using (var serialized = new SerializedObject(component))
        {
            var property = serialized.FindProperty("animator");
            if (property != null && property.objectReferenceValue is Animator animator) return animator;
        }
        var nearest = component.GetComponentInParent<Animator>(true);
        if (nearest) return nearest;
        var actor = component.GetComponentInParent<PlayerHealth>(true) as Component ??
            component.GetComponentInParent<SkeletonController>(true);
        if (actor)
        {
            using (var serialized = new SerializedObject(actor))
            {
                var property = serialized.FindProperty("animator");
                if (property != null && property.objectReferenceValue is Animator animator) return animator;
            }
            return actor.GetComponent<Animator>();
        }
        return component.GetComponentInParent<Animator>(true);
    }

    void ReadOpponent(EndlessOpponent opponent, Animator animator)
    {
        if (!opponent) return;
        AddSprite(opponent.idle);
        ReadController(opponent.controller, animator);
        ReadClip(opponent.hurt, animator);
    }

    void ReadController(RuntimeAnimatorController controller, Animator animator)
    {
        if (!controller) return;
        foreach (var clip in controller.animationClips) ReadClip(clip, animator);
    }

    void ReadClip(AnimationClip clip, Animator animator)
    {
        if (!clip || !animator) return;
        foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
        {
            if (binding.type != typeof(SpriteRenderer) || binding.propertyName != "m_Sprite") continue;
            var target = string.IsNullOrEmpty(binding.path) ? animator.transform : animator.transform.Find(binding.path);
            if (!target || !IsReceiver(target.GetComponent<SpriteRenderer>())) continue;
            // Resolve while the scene is alive; records retain only asset and path values.
            if (!curveKeys.Add((clip, binding.path))) continue;
            curves.Add(new Curve { clip = clip, binding = binding });
            foreach (var frame in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                AddSprite(frame.value as Sprite);
        }
    }

    void AddSprite(Sprite sprite)
    {
        if (sprite) sprites.Add(sprite);
    }
}
