using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyHitFlash : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] List<SpriteRenderer> renderers;

    [Header("Flash Tint")]
    [SerializeField] Color flashColor = new Color(1f, 0.55f, 0.55f, 1f);
    [SerializeField, Range(0.02f, 0.20f)] float baseDuration = 0.06f;

    [Header("Timing")]
    [SerializeField] bool useUnscaledTime = true;
    [SerializeField, Range(1, 4)] int minFrames = 2;

    Color[] _originalColors;
    SlayerSilhouetteGroup[] _groups;
    float _flashUntil = -1f;
    int _flashFrameUntil = -1;
    bool _active;

    void Awake()
    {
        if (renderers == null || renderers.Count == 0)
            renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(includeInactive: false));
        EnsureCache();
    }

    void OnEnable()
    {
        _active = false;
        EnsureCache();
        _flashUntil = -1f;
        _flashFrameUntil = -1;
    }

    void OnDisable()
    {
        // Nếu đang flash mà bị tắt (hoặc đổi scene) → trả ngay về màu gốc
        _active = false;
        RestoreInstant();
        _flashUntil = -1f;
        _flashFrameUntil = -1;
    }

    void EnsureCache()
    {
        if (renderers == null) renderers = new List<SpriteRenderer>();
        if (_originalColors == null || _originalColors.Length != renderers.Count)
        {
            _originalColors = new Color[renderers.Count];
            _groups = new SlayerSilhouetteGroup[renderers.Count];
            for (int i = 0; i < renderers.Count; i++)
            {
                _groups[i] = renderers[i] ? renderers[i].GetComponentInParent<SlayerSilhouetteGroup>(true) : null;
                _originalColors[i] = renderers[i] ? renderers[i].color : Color.white;
            }
        }
    }

    void Update()
    {
        if (!_active) return;
        if (GamePauseController.InputBlocked || Time.timeScale == 0f)
        {
            _flashUntil += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _flashFrameUntil++;
            return;
        }

        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        bool timePassed = now >= _flashUntil;
        bool framePassed = Time.frameCount >= _flashFrameUntil;
        if (timePassed && framePassed)
        {
            _active = false;
            RestoreInstant();
        }
    }

    /// Gọi khi enemy trúng đòn
    public void Tick(float overrideDuration = -1f)
    {
        // KHÓA: không cho hit-flash hoạt động trong Slayer, để tránh chụp “màu gốc” = đen
        if (SlayerModeSignals.Active) return;

        if (renderers == null || renderers.Count == 0) return;

        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        float dur = (overrideDuration > 0f) ? overrideDuration : baseDuration;

        if (!_active)
        {
            // Lưu lại màu gốc đúng tại thời điểm BÌNH THƯỜNG (không phải lúc silhouette)
            for (int i = 0; i < renderers.Count; i++)
            {
                var r = renderers[i];
                if (!r) continue;
                _originalColors[i] = _groups[i] && _groups[i].TryGetBaseColor(r, out var baseline)
                    ? baseline : r.color;
            }
            _active = true;
        }
        // The silhouette compositor owns RGB until its presentation ends.
        for (int i = 0; i < renderers.Count; i++)
        {
            var renderer = renderers[i];
            if (!renderer || (_groups[i] && _groups[i].TryGetBaseColor(renderer, out _))) continue;
            var color = flashColor; color.a = renderer.color.a;
            renderer.color = color;
        }

        _flashUntil = now + dur;
        _flashFrameUntil = Time.frameCount + Mathf.Max(1, minFrames);
    }

    // Raw caches only: these reads never call back into the compositor.
    internal bool TryGetUnflashedColor(SpriteRenderer renderer, out Color color)
    {
        int index = renderers == null ? -1 : renderers.IndexOf(renderer);
        bool found = _active && index >= 0 && _originalColors != null && index < _originalColors.Length;
        color = found ? _originalColors[index] : default;
        return found;
    }

    internal bool TryGetFlashColor(SpriteRenderer renderer, out Color color)
    {
        color = flashColor;
        return isActiveAndEnabled && _active && renderers != null && renderers.Contains(renderer);
    }

    void RestoreInstant()
    {
        if (renderers == null || _originalColors == null) return;
        for (int i = 0; i < renderers.Count && i < _originalColors.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;
            if (_groups[i] && _groups[i].TryGetBaseColor(r, out _)) continue;
            var color = _originalColors[i]; color.a = r.color.a;
            r.color = color;
        }
    }
}
