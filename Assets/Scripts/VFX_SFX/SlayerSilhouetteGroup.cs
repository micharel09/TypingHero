using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlayerSilhouetteGroup : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] bool excludeUnlitShaders = true;
    [SerializeField] bool includeDynamicChildren = true; // NEW: bắt cả SR mới spawn

    [Header("Tint")]
    [SerializeField] Color silhouetteColor = Color.black;

    readonly List<SpriteRenderer> _targets = new();
    readonly List<SpriteRenderer> _scanBuf = new();   // NEW: buffer quét nhanh
    readonly Dictionary<SpriteRenderer, Color> _orig = new();
    bool _active;

    void Awake()
    {
        RescanTargets();
        SnapshotOriginals();
    }

    void OnEnable()
    {
        SlayerModeSignals.OnSetActive += SetActive;
        SetActive(SlayerModeSignals.Active);
    }

    void OnDisable()
    {
        SlayerModeSignals.OnSetActive -= SetActive;
        if (_active) RestoreOriginals();
        _active = false;
    }

    void LateUpdate()
    {
        if (!_active) return;

        // tô đen các target đã snapshot
        for (int i = 0; i < _targets.Count; i++)
        {
            var sr = _targets[i];
            if (!sr) continue;
            var c = sr.color; c.r = silhouetteColor.r; c.g = silhouetteColor.g; c.b = silhouetteColor.b;
            sr.color = c;
        }

        // NEW: nếu có child mới (spark/afterimage…) → tô đen luôn
        if (includeDynamicChildren)
        {
            _scanBuf.Clear();
            GetComponentsInChildren(true, _scanBuf);
            for (int i = 0; i < _scanBuf.Count; i++)
            {
                var sr = _scanBuf[i];
                if (!sr) continue;
                if (excludeUnlitShaders)
                {
                    var sh = sr.sharedMaterial ? sr.sharedMaterial.shader : null;
                    if (sh && (sh.name.Contains("Unlit") || sh.name.Contains("UI"))) continue;
                }
                // Nếu chưa có trong snapshot (vừa spawn) -> tô đen tức thì
                if (!_orig.ContainsKey(sr))
                {
                    var c = sr.color; c.r = silhouetteColor.r; c.g = silhouetteColor.g; c.b = silhouetteColor.b;
                    sr.color = c;
                }
            }
        }
    }

    void SetActive(bool on)
    {
        if (on)
        {
            RescanTargets();
            SnapshotOriginals();
            _active = true;
            LateUpdate(); // tô đen ngay
        }
        else
        {
            _active = false;
            RestoreOriginals();
        }
    }

    // ------- helpers -------
    void RescanTargets()
    {
        _targets.Clear();
        GetComponentsInChildren(true, _targets);

        if (excludeUnlitShaders)
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                var sr = _targets[i];
                if (!sr) { _targets.RemoveAt(i); continue; }
                var sh = sr.sharedMaterial ? sr.sharedMaterial.shader : null;
                if (sh && (sh.name.Contains("Unlit") || sh.name.Contains("UI")))
                    _targets.RemoveAt(i);
            }
        }
    }

    void SnapshotOriginals()
    {
        _orig.Clear();
        for (int i = 0; i < _targets.Count; i++)
        {
            var sr = _targets[i];
            if (!sr) continue;
            _orig[sr] = sr.color;
        }
    }

    void RestoreOriginals()
    {
        foreach (var kv in _orig)
        {
            var sr = kv.Key;
            if (!sr) continue;
            sr.color = kv.Value;
        }
    }
}
