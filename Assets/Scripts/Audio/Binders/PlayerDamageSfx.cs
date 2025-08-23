using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerDamageSfx : MonoBehaviour, IDamageable
{
    [Header("Forward target (bắt buộc)")]
    [Tooltip("Component implement IDamageable thực sự, ví dụ PlayerHealth.")]
    [SerializeField] MonoBehaviour targetDamageable;

    [Header("SFX")]
    [SerializeField] SfxEvent sfxPlayerHit;          // ev_player_hit_*.asset
    [SerializeField] Transform playAt;               // neo phát (ngực). Trống = this
    [SerializeField, Min(0f)] float cooldown = 0.06f;

    IDamageable _target;
    PlayerHealth _playerHealth; // để đọc chỉ số HP trước/sau
    float _nextAllowedAt;

    public bool IsDead => _target != null && _target.IsDead;

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (_target == null || _target.IsDead) return;

        // Ghi lại HP trước khi forward (nếu target là PlayerHealth)
        int hpBefore = _playerHealth ? _playerHealth.Current : int.MinValue;

        // 1) forward damage cho hệ thống gốc
        _target.TakeDamage(amount, hitPoint);

        // 2) Chỉ phát khi thực sự bị trừ HP (không phát khi parry/i-frame)
        bool applied = _playerHealth
            ? (_playerHealth.Current < hpBefore || _playerHealth.IsDead)
            : true; // nếu không phải PlayerHealth, giả định là đã áp dụng

        if (!applied) return;

        // 3) Chống spam + phát SFX 2D
        if (Time.unscaledTime >= _nextAllowedAt && SfxPlayer.I && sfxPlayerHit)
        {
            var t = playAt ? playAt : transform;
            Vector3 pos = new Vector3(hitPoint.x, hitPoint.y, t.position.z);
            SfxPlayer.I.Play(sfxPlayerHit, pos, t);
            _nextAllowedAt = Time.unscaledTime + cooldown;
        }
    }

    void Reset() { playAt = transform; }

    void Awake()
    {
        _target = targetDamageable as IDamageable;
        if (_target == null)
        {
            _target = GetComponent<IDamageable>();
            if (_target == (IDamageable)this) _target = null;
        }
        Debug.Assert(_target != null, "[PlayerDamageSfx] targetDamageable phải implement IDamageable.");

        _playerHealth = _target as PlayerHealth;
        if (!playAt) playAt = transform;
    }
}
