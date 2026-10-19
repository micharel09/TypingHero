using UnityEngine;
using UnityEngine.Pool;

public sealed class PooledVFX : MonoBehaviour
{
    IObjectPool<GameObject> _pool;
    Vector3 _initial;
    SpriteRenderer _renderer;
    Color _color;
    Animator _animator;
    bool _leased;

    void Awake()
    {
        _initial = transform.localScale;
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer) _color = _renderer.color;
        _animator = GetComponent<Animator>();
    }
    public void Setup(IObjectPool<GameObject> pool) => _pool = pool;

    void OnEnable()
    {
        _leased = true;
        transform.localScale = _initial;
        if (_renderer) { _renderer.color = _color; _renderer.flipX = _renderer.flipY = false; }
        if (_animator)
        {
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _animator.Rebind();
            _animator.speed = Paused ? 0f : 1f;
            _animator.Update(0f);
        }
    }

    bool Paused => GamePauseController.InputBlocked || Time.timeScale == 0f;
    void Update() { if (_animator) _animator.speed = Paused ? 0f : 1f; }

    // The last animation frame owns release; duplicate events cannot return twice.
    public void AnimEvent_Release() => Release();

    public void Release()
    {
        if (!_leased) return;
        _leased = false;
        if (TryGetComponent<PixelSparkBurst>(out var burst)) burst.Clear();
        if (TryGetComponent<ParryClashBurst>(out var clash)) clash.Clear();
        transform.localScale = _initial;
        if (_renderer) _renderer.color = _color;
        if (_animator) _animator.speed = 1f;
        if (_pool != null) _pool.Release(gameObject);
        else Destroy(gameObject);
    }
}
