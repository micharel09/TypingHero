#if UNITY_EDITOR || (TYPINGHERO_AUTOMATION && TYPINGHERO_DEPTH_PREVIEW)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Render-only A/B control in QA players. Does not alter combat or expose a game API.
public sealed class SpriteDepthPreviewControl : MonoBehaviour
{
    static readonly int Strength = Shader.PropertyToID("_SelfShadowStrength");
    readonly Dictionary<Material, float> configuredStrengths = new Dictionary<Material, float>();
    bool enabledDepth = true;

    #if TYPINGHERO_AUTOMATION && TYPINGHERO_DEPTH_PREVIEW
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        var control = new GameObject("Depth preview A-B").AddComponent<SpriteDepthPreviewControl>();
        DontDestroyOnLoad(control.gameObject);
    }
    #endif
    void OnEnable() { SceneManager.sceneLoaded += Loaded; Apply(); }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= Loaded;
        foreach (var entry in configuredStrengths)
            if (entry.Key) entry.Key.SetFloat(Strength, entry.Value);
        configuredStrengths.Clear();
        enabledDepth = true;
    }
    void Loaded(Scene scene, LoadSceneMode mode) { Apply(); }
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.F8)) return;
        enabledDepth = !enabledDepth;
        Apply();
        Debug.Log("Depth preview: " + (enabledDepth ? "ON (8 samples/light)" : "OFF"));
    }
    void Apply()
    {
        // Scene bind or explicit QA key only; no scan/allocation in the hot update.
        foreach (var binding in FindObjectsOfType<SpriteDepthBinding>(true))
        {
            var material = binding.GetComponent<SpriteRenderer>().sharedMaterial;
            if (!material || !material.HasProperty(Strength)) continue;
            if (!configuredStrengths.TryGetValue(material, out float configured))
            {
                configured = material.GetFloat(Strength);
                configuredStrengths.Add(material, configured);
            }
            material.SetFloat(Strength, enabledDepth ? configured : 0);
        }
    }
}
#endif
