using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlayerSilhouetteGroup : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] bool excludeUnlitShaders = true;   // tránh đụng VFX Unlit/UI

    [Header("Tint")]
    [SerializeField] Color silhouetteColor = Color.black;

    readonly List<SpriteRenderer> _targets = new();
    readonly Dictionary<SpriteRenderer, Color> _orig = new();
    bool _active;

    void Awake()
    {
        // Gom 1 lần tất cả SpriteRenderer con (kể cả inactive)
        GetComponentsInChildren(true, _targets);
        // Lưu màu gốc
        for (int i = 0; i < _targets.Count; i++)
        {
            var sr = _targets[i];
            if (!sr) continue;
            if (excludeUnlitShaders && sr.sharedMaterial != null)
            {
                var sh = sr.sharedMaterial.shader;
                if (sh && (sh.name.Contains("Unlit") || sh.name.Contains("UI"))) continue;
            }
            if (!_orig.ContainsKey(sr)) _orig[sr] = sr.color;
        }
    }

    void OnEnable()
    {
        SlayerModeSignals.OnSetActive += SetActive;
        if (SlayerModeSignals.Active) SetActive(true);
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
        // Ép đen mỗi frame để đè mọi thay đổi màu khác (hitflash…)
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
        _active = on;
        if (on)
        {
            // set đen ngay một lần
            LateUpdate();
        }
        else
        {
            RestoreOriginals();
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
