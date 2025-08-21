using UnityEngine.Pool;
using UnityEngine;

public sealed class PooledVFX : MonoBehaviour
{
    IObjectPool<GameObject> _pool;
    Vector3 _initial;

    void Awake() => _initial = transform.localScale;
    public void Setup(IObjectPool<GameObject> pool) => _pool = pool;

    // Gọi bởi animation event
    public void AnimEvent_Release()
    {
        transform.localScale = _initial;   // RESET scale trước khi trả pool
        if (_pool != null) _pool.Release(gameObject);
        else Destroy(gameObject);
    }
}
