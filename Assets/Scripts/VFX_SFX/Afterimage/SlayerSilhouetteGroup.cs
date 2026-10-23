using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlayerSilhouetteGroup : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] bool excludeUnlitShaders = true;
    [SerializeField] bool includeDynamicChildren = true;
    [Tooltip("Bỏ qua mọi SpriteRenderer có tên chứa một trong các chuỗi này.")]
    [SerializeField] string[] ignoreNameContains = new[] { "Afterimage", "HitSpark" }; // <-- Spark & Afterimage KHÔNG bị tô đen

    [Header("Tint")]
    [SerializeField] Color silhouetteColor = Color.black;

    readonly List<SpriteRenderer> _targets = new();
    readonly Dictionary<SpriteRenderer, Color> _orig = new();
    bool _active;
    MaterialPropertyBlock _block;
    static readonly int TintWeightId = Shader.PropertyToID("_SlayerTintWeight");
    void ApplyMaterialWeight(SpriteRenderer renderer, float weight)
    {
        if (_block == null) _block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(_block);
        _block.SetFloat(TintWeightId, weight);
        renderer.SetPropertyBlock(_block);
    }

    void Awake() { RescanTargets(); SnapshotOriginals(); }
    void OnEnable() { SlayerModeSignals.OnSetActive += SetActive; SetActive(SlayerModeSignals.Active); }
    void OnDisable() { SlayerModeSignals.OnSetActive -= SetActive; if (_active) RestoreOriginals(); _active = false; _weight = 0; }

    float _weight;
    void LateUpdate()
    {
        if (SlayerScreenFX.PresentationDriven) SetPresentationWeight(SlayerScreenFX.PresentationWeight);
        if (_weight <= 0) return;
        if (includeDynamicChildren) RescanTargets();
        foreach (var sr in _targets)
        {
            if (!sr || !ShouldAffect(sr)) continue;
            if (!_orig.ContainsKey(sr)) _orig[sr] = sr.color;
            var current = sr.color;
            var tint = Color.Lerp(_orig[sr], silhouetteColor, _weight);
            tint.a = current.a; // animation/visibility still owns alpha
            sr.color = tint;
            ApplyMaterialWeight(sr, _weight);
        }
    }

    public void SetPresentationWeight(float weight)
    {
        weight = Mathf.Clamp01(weight);
        if (_weight <= 0 && weight > 0) { RescanTargets(); SnapshotOriginals(); }
        if (_weight > 0 && weight <= 0) RestoreOriginals();
        _weight = weight;
        _active = weight > 0;
    }

    void SetActive(bool on)
    {
        if (!SlayerScreenFX.PresentationDriven) SetPresentationWeight(on ? 1 : 0);
    }

    // ---------- helpers ----------
    bool ShouldAffect(SpriteRenderer sr)
    {
        // Bỏ qua theo tên (Spark/Afterimage…)
        if (ignoreNameContains != null)
        {
            string n = sr.transform.name;
            for (int i = 0; i < ignoreNameContains.Length; i++)
            {
                var key = ignoreNameContains[i];
                if (!string.IsNullOrEmpty(key) && n.Contains(key)) return false;
            }
        }
        // Bỏ qua Unlit/UI nếu bật filter
        if (excludeUnlitShaders)
        {
            var sh = sr.sharedMaterial ? sr.sharedMaterial.shader : null;
            if (sh && (sh.name.Contains("Unlit") || sh.name.Contains("UI"))) return false;
        }
        return true;
    }

    void RescanTargets()
    {
        _targets.Clear();
        GetComponentsInChildren(true, _targets);
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            var sr = _targets[i];
            if (!sr || !ShouldAffect(sr)) _targets.RemoveAt(i);
        }
    }

    void SnapshotOriginals()
    {
        _orig.Clear();
        for (int i = 0; i < _targets.Count; i++)
        {
            var sr = _targets[i]; if (!sr) continue;
            _orig[sr] = sr.color;
        }
    }

    void RestoreOriginals()
    {
        foreach (var kv in _orig)
        {
            var sr = kv.Key; if (!sr) continue;
            sr.color = kv.Value;
            ApplyMaterialWeight(sr, 0);
        }
    }
}
