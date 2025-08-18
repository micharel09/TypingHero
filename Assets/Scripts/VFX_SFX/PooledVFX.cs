using UnityEngine;
using UnityEngine.Pool;

public sealed class PooledVFX : MonoBehaviour
{
    IObjectPool<GameObject> _pool;

    // ???c g?i b?i spawner sau khi Instantiate
    public void Setup(IObjectPool<GameObject> pool) => _pool = pool;

    // G?I T? ANIMATION EVENT ? frame cu?i
    // (??t tên event trong clip: AnimEvent_Release)
    public void AnimEvent_Release()
    {
        if (_pool != null) _pool.Release(gameObject);
        else Destroy(gameObject); // fallback n?u ch?a có pool
    }
}
