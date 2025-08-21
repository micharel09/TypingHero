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
    readonly List<SpriteRenderer> _scanBuf = new();
    readonly Dictionary<SpriteRenderer, Color> _orig = new();
    bool _active;

    void Awake() { RescanTargets(); SnapshotOriginals(); }
    void OnEnable() { SlayerModeSignals.OnSetActive += SetActive; SetActive(SlayerModeSignals.Active); }
    void OnDisable() { SlayerModeSignals.OnSetActive -= SetActive; if (_active) RestoreOriginals(); _active = false; }

    void LateUpdate()
    {
        if (!_active) return;

        for (int i = 0; i < _targets.Count; i++)
        {
            var sr = _targets[i]; if (!sr) continue;
            if (!ShouldAffect(sr)) continue;
            var c = sr.color; c.r = silhouetteColor.r; c.g = silhouetteColor.g; c.b = silhouetteColor.b;
            sr.color = c;
        }

        if (!includeDynamicChildren) return;

        _scanBuf.Clear();
        GetComponentsInChildren(true, _scanBuf);
        for (int i = 0; i < _scanBuf.Count; i++)
        {
            var sr = _scanBuf[i]; if (!sr) continue;
            if (!ShouldAffect(sr)) continue;
            if (_orig.ContainsKey(sr)) continue;
            var c = sr.color; c.r = silhouetteColor.r; c.g = silhouetteColor.g; c.b = silhouetteColor.b;
            sr.color = c;
        }
    }

    void SetActive(bool on)
    {
        if (on)
        {
            RescanTargets(); SnapshotOriginals(); _active = true; LateUpdate();
        }
        else
        {
            _active = false; RestoreOriginals();
        }
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
        }
    }
}
