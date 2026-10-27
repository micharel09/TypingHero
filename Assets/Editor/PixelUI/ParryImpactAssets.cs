using UnityEditor;
using UnityEngine;

// Only creates a missing stock prefab; authored profiles and prefabs remain authoritative.
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
                var so = new SerializedObject(instance.GetComponent<ImpactVfxBurst>());
                so.FindProperty("impactProfile").objectReferenceValue = profile;
                so.FindProperty("directionalFlashChannel").intValue = 0;
                var bindings = so.FindProperty("rendererBindings");
                string[] materials = { "PimenCore", "PimenFragment", "PimenRay", "PimenSpeck", "PimenRay" };
                for (int i = 0; i < bindings.arraySize; i++)
                {
                    var binding = bindings.GetArrayElementAtIndex(i);
                    int id = binding.FindPropertyRelative("id").intValue;
                    var ps = (ParticleSystem)binding.FindPropertyRelative("particles").objectReferenceValue;
                    var material = AssetDatabase.LoadAssetAtPath<Material>(root + materials[id] + ".mat");
                    binding.FindPropertyRelative("material").objectReferenceValue = material;
                    ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
                    if (id == 3) { var main = ps.main; main.maxParticles = 4; }
                }
                var source = (ParticleSystem)bindings.GetArrayElementAtIndex(0).FindPropertyRelative("particles").objectReferenceValue;
                var horizontal = Object.Instantiate(source, instance.transform);
                horizontal.name = "HorizontalFlash";
                var horizontalMaterial = AssetDatabase.LoadAssetAtPath<Material>(root + "HorizontalFlash.mat");
                horizontal.GetComponent<ParticleSystemRenderer>().sharedMaterial = horizontalMaterial;
                int slot = bindings.arraySize++;
                var horizontalBinding = bindings.GetArrayElementAtIndex(slot);
                horizontalBinding.FindPropertyRelative("id").intValue = 5;
                horizontalBinding.FindPropertyRelative("particles").objectReferenceValue = horizontal;
                horizontalBinding.FindPropertyRelative("material").objectReferenceValue = horizontalMaterial;
                so.ApplyModifiedPropertiesWithoutUndo();
                prefab = PrefabUtility.SaveAsPrefabAsset(instance, root + "ParryImpact.prefab");
            }
            finally { Object.DestroyImmediate(instance); }
        }
        if (!profile.prefab || profile.prefab == ParryClashAssets.Load())
        {
            profile.prefab = prefab;
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssetIfDirty(profile);
        }
    }
}
