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
    float _flashUntil = -1f;
    int _flashFrameUntil = -1;
    bool _active;

    void Awake()
    {
        if (renderers == null || renderers.Count == 0)
            renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(includeInactive: false));
        EnsureCache();
        RestoreInstant();
    }

    void OnEnable()
    {
        EnsureCache();
        RestoreInstant();
        _active = false;
        _flashUntil = -1f;
        _flashFrameUntil = -1;
    }

    void OnDisable()
    {
        // Nếu đang flash mà bị tắt (hoặc đổi scene) → trả ngay về màu gốc
        RestoreInstant();
        _active = false;
    }

    void EnsureCache()
    {
        if (renderers == null) renderers = new List<SpriteRenderer>();
        if (_originalColors == null || _originalColors.Length != renderers.Count)
        {
            _originalColors = new Color[renderers.Count];
            for (int i = 0; i < renderers.Count; i++)
                _originalColors[i] = renderers[i] ? renderers[i].color : Color.white;
        }
    }

    void Update()
    {
        if (!_active) return;

        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        bool timePassed = now >= _flashUntil;
        bool framePassed = Time.frameCount >= _flashFrameUntil;
        if (timePassed && framePassed)
        {
            RestoreInstant();
            _active = false;
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
                _originalColors[i] = r.color;
                r.color = flashColor;
            }
            _active = true;
        }
        else
        {
            // Gia hạn và đảm bảo vẫn là flashColor
            for (int i = 0; i < renderers.Count; i++)
            {
                var r = renderers[i];
                if (!r) continue;
                r.color = flashColor;
            }
        }

        _flashUntil = now + dur;
        _flashFrameUntil = Time.frameCount + Mathf.Max(1, minFrames);
    }

    void RestoreInstant()
    {
        if (renderers == null || _originalColors == null) return;
        for (int i = 0; i < renderers.Count && i < _originalColors.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;
            r.color = _originalColors[i];
        }
    }
}
