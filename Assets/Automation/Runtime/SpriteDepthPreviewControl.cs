#if TYPINGHERO_AUTOMATION && TYPINGHERO_DEPTH_PREVIEW
using UnityEngine;
using UnityEngine.SceneManagement;

// Render-only A/B control in QA players. Does not alter combat or expose a game API.
public sealed class SpriteDepthPreviewControl : MonoBehaviour
{
    static readonly int Strength = Shader.PropertyToID("_SelfShadowStrength");
    bool enabledDepth = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        var control = new GameObject("Depth preview A-B").AddComponent<SpriteDepthPreviewControl>();
        DontDestroyOnLoad(control.gameObject);
    }
    void OnEnable() { SceneManager.sceneLoaded += Loaded; }
    void OnDisable() { SceneManager.sceneLoaded -= Loaded; }
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
            if (material && material.HasProperty(Strength))
                material.SetFloat(Strength, enabledDepth ? .65f : 0);
        }
    }
}
#endif
