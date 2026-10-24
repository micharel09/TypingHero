using UnityEditor;
using UnityEngine;

// One-time stock migration; subsequent builds retain authored assets and bindings.
internal static class ParryImpactAssets
{
    internal static void Prepare()
    {
        const string root = ParryClashAssets.Root;
        var profile = AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>(root + "ParryImpact.asset");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(root + "ParryImpact.prefab");
        if (!prefab)
        {
            var instance = Object.Instantiate(ParryClashAssets.Load());
            instance.name = "ParryImpact"; instance.SetActive(false);
            try
            {
                var burst = instance.GetComponent<ImpactVfxBurst>();
                var so = new SerializedObject(burst);
                so.FindProperty("impactProfile").objectReferenceValue = profile;
                so.FindProperty("directionalFlashChannel").intValue = 5;
                var bindings = so.FindProperty("rendererBindings");
                var source = (ParticleSystem)bindings.GetArrayElementAtIndex(0).FindPropertyRelative("particles").objectReferenceValue;
                var arc = Object.Instantiate(source.gameObject, instance.transform).GetComponent<ParticleSystem>();
                arc.name = "ShockArc";
                var main = arc.main; main.maxParticles = 1; main.loop = false;
                main.playOnAwake = false; main.useUnscaledTime = true;
                var emission = arc.emission; emission.enabled = false;
                var material = AssetDatabase.LoadAssetAtPath<Material>(root + "ParryArc.mat");
                arc.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
                int index = bindings.arraySize; bindings.arraySize++;
                var binding = bindings.GetArrayElementAtIndex(index);
                binding.FindPropertyRelative("id").intValue = 5;
                binding.FindPropertyRelative("particles").objectReferenceValue = arc;
                binding.FindPropertyRelative("material").objectReferenceValue = material;
                so.ApplyModifiedPropertiesWithoutUndo();
                prefab = PrefabUtility.SaveAsPrefabAsset(instance, root + "ParryImpact.prefab");
            }
            finally { Object.DestroyImmediate(instance); }
        }
        if (profile.prefab == ParryClashAssets.Load())
        {
            profile.prefab = prefab;
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssetIfDirty(profile);
        }
    }
}
