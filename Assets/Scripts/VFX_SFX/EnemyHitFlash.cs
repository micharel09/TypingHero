using System.Collections.Generic;
using UnityEngine;

/// Nháy flash ngắn khi bị trúng đòn. Không dùng coroutine để tránh GC.
[DisallowMultipleComponent]
public sealed class EnemyHitFlash : MonoBehaviour
{
    [Header("Renderers")]
    [Tooltip("Để trống sẽ quét toàn bộ SpriteRenderer con (active).")]
    [SerializeField] List<SpriteRenderer> renderers;

    [Header("Flash Tint")]
    [Tooltip("Tint hơi đỏ để thấy rõ với sprite trắng.")]
    [SerializeField] Color flashColor = new Color(1f, 0.55f, 0.55f, 1f);

    [Tooltip("Thời lượng nháy cơ bản (giây).")]
    [SerializeField, Range(0.02f, 0.20f)] float baseDuration = 0.06f;

    [Header("Timing")]
    [Tooltip("Sử dụng unscaled time để bỏ qua hitstop.")]
    [SerializeField] bool useUnscaledTime = true;

    [Tooltip("Đảm bảo tối thiểu số frame hiển thị flash.")]
    [SerializeField, Range(1, 4)] int minFrames = 2;

    // Cache
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
        _flashUntil = -1f;
        _flashFrameUntil = -1;
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

    /// Gọi khi bị trúng đòn. Có thể override duration (giây).
    public void Tick(float overrideDuration = -1f)
    {
        if (renderers == null || renderers.Count == 0) return;

        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        float dur = (overrideDuration > 0f) ? overrideDuration : baseDuration;

        if (!_active)
        {
            // Chụp màu gốc (phòng trường hợp có hệ thống đổi màu khi hit-react)
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
            // Đang flash → chỉ gia hạn mốc tắt
            for (int i = 0; i < renderers.Count; i++)
            {
                var r = renderers[i];
                if (!r) continue;
                // đảm bảo vẫn là flashColor nếu script khác vừa ghi đè
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

    // API cấu hình động (tuỳ thích)
    public void SetFlashColor(Color c) => flashColor = c;
    public void SetBaseDuration(float d) => baseDuration = Mathf.Clamp(d, 0.02f, 0.2f);
}
