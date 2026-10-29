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
    EnemyHitFlash _flash;
    MaterialPropertyBlock _block;
    static readonly int TintWeightId = Shader.PropertyToID("_SlayerTintWeight");
    static readonly int HitWeightId = Shader.PropertyToID("_SlayerHitFlashAmount");
    static readonly int HitColorId = Shader.PropertyToID("_SlayerHitFlashColor");
    static readonly Color HitColor = new Color32(255, 243, 207, 255);
    float hitAge = -1;
    int hitBornFrame;
    float hitHold = .02f;
    bool hitPeak;
    EnemyHitAnchor hitAnchor;
    uint hitLifetime;
    internal float HitFlashWeight
    {
        get
        {
            if (hitAge < 0 || (hitAnchor && hitAnchor.Lifetime != hitLifetime)) return 0;
            float contact = Mathf.Clamp01((hitHold + .02f - hitAge) / .02f);
            float peak = hitPeak && hitAge >= .07f ? Mathf.Clamp01((.09f - hitAge) / .02f) : 0;
            return Mathf.Max(contact, peak);
        }
    }
    internal void PulseSlayerHit()
        => StartHitPulse(.02f, false, null, 0);

    internal void PulseSlayerSweep(bool hasPrimary, bool startAtContact, bool lethal, EnemyHitAnchor anchor, uint lifetime)
        => StartHitPulse(hasPrimary ? 0 : lethal ? .15f : .13f, hasPrimary && !startAtContact, anchor, lifetime);

    void StartHitPulse(float hold, bool peak, EnemyHitAnchor anchor, uint lifetime)
    {
        if (!isActiveAndEnabled) return;
        hitHold = hold;
        hitPeak = peak;
        hitAnchor = anchor;
        hitLifetime = lifetime;
        hitAge = 0;
        hitBornFrame = Time.frameCount;
    }
    void ApplyMaterialWeight(SpriteRenderer renderer, float weight)
    {
        if (_block == null) _block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(_block);
        _block.SetFloat(TintWeightId, 0);
        _block.SetFloat(HitWeightId, HitFlashWeight);
        _block.SetColor(HitColorId, HitColor);
        renderer.SetPropertyBlock(_block);
    }

    void Awake() { _flash = GetComponentInParent<EnemyHitFlash>(true); RescanTargets(); SnapshotOriginals(); }
    void OnEnable() { SlayerModeSignals.OnSetActive += SetActive; SetActive(SlayerModeSignals.Active); }
    void OnDisable() { SlayerModeSignals.OnSetActive -= SetActive; bool pulsed = hitAge >= 0; hitAge = -1; if (_active || pulsed) RestoreOriginals(); _active = false; _weight = 0; }

    float _weight;
    void LateUpdate()
    {
        if (SlayerScreenFX.PresentationDriven) SetPresentationWeight(SlayerScreenFX.PresentationWeight);
        bool updatePulse = hitAge >= 0;
        if (GamePauseController.ResultOverlayVisible || (hitAnchor && hitAnchor.Lifetime != hitLifetime)) hitAge = -1;
        else if (hitAge >= 0 && hitBornFrame != Time.frameCount && !GamePauseController.InputBlocked && Time.timeScale != 0)
        {
            hitAge += Time.unscaledDeltaTime;
            if (hitAge >= (hitPeak ? .09f : hitHold + .02f)) hitAge = -1;
        }
        if (_weight <= 0)
        {
            // A first/lethal hit can land at an entry/exit endpoint.
            if (updatePulse) foreach (var sr in _targets) if (sr && ShouldAffect(sr)) ApplyMaterialWeight(sr, 0);
            return;
        }
        if (includeDynamicChildren) RescanTargets();
        foreach (var sr in _targets)
        {
            if (!sr || !ShouldAffect(sr)) continue;
            if (!_orig.ContainsKey(sr)) _orig[sr] = ReadBaseline(sr);
            var current = sr.color;
            var source = _flash && _flash.TryGetFlashColor(sr, out var flash) ? flash : _orig[sr];
            var tint = source; // Preserve authored RGB; the shader still composes the hit pulse.
            tint.a = current.a; // animation/visibility still owns alpha
            sr.color = tint;
            ApplyMaterialWeight(sr, _weight);
        }
    }

    public void SetPresentationWeight(float weight)
    {
        weight = Mathf.Clamp01(weight);
        if (_weight <= 0 && weight > 0) { RescanTargets(); SnapshotOriginals(); }
        if (_weight > 0 && weight <= 0) { hitAge = -1; RestoreOriginals(); }
        _weight = weight;
        _active = weight > 0;
    }

    internal bool TryGetBaseColor(SpriteRenderer renderer, out Color color)
    {
        color = default;
        return isActiveAndEnabled && _weight > 0 && _orig.TryGetValue(renderer, out color);
    }

    Color ReadBaseline(SpriteRenderer renderer) => _flash && _flash.TryGetUnflashedColor(renderer, out var color)
        ? color : renderer.color;

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
            _orig[sr] = ReadBaseline(sr);
        }
    }

    void RestoreOriginals()
    {
        foreach (var kv in _orig)
        {
            var sr = kv.Key; if (!sr) continue;
            var color = _flash && _flash.TryGetFlashColor(sr, out var flash) ? flash : kv.Value;
            color.a = sr.color.a;
            sr.color = color;
            ApplyMaterialWeight(sr, 0);
        }
    }
}
