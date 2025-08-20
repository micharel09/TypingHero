using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlayerSilhouetteGroup : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] bool excludeUnlitShaders = true;
    [SerializeField] bool excludeAfterimages = true;      // NEW

    [Header("Tint")]
    [SerializeField] Color silhouetteColor = Color.black;

    readonly List<SpriteRenderer> _targets = new();
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
        for (int i = 0; i < _targets.Count; i++)
        {
            var sr = _targets[i];
            if (!sr) continue;
            var c = sr.color;
            c.r = silhouetteColor.r; c.g = silhouetteColor.g; c.b = silhouetteColor.b;
            sr.color = c;
        }
    }

    void SetActive(bool on)
    {
        if (on)
        {
            RescanTargets();
            SnapshotOriginals();
            _active = true;
            LateUpdate();
        }
        else
        {
            _active = false;
            RestoreOriginals();
        }
    }

    // -------- helpers --------
    void RescanTargets()
    {
        _targets.Clear();
        GetComponentsInChildren(true, _targets);

        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            var sr = _targets[i];
            if (!sr) { _targets.RemoveAt(i); continue; }

            // NEW: bỏ qua các object afterimage
            if (excludeAfterimages && sr.gameObject.name == "Afterimage")
            {
                _targets.RemoveAt(i);
                continue;
            }

            if (excludeUnlitShaders)
            {
                var sh = sr.sharedMaterial ? sr.sharedMaterial.shader : null;
                if (sh && (sh.name.Contains("Unlit") || sh.name.Contains("UI")))
                {
                    _targets.RemoveAt(i);
                    continue;
                }
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
            if (!_orig.ContainsKey(sr)) _orig[sr] = sr.color;
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
