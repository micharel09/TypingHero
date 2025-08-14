using System.Collections;
using UnityEngine;

public class HitStopper : MonoBehaviour
{
    public static HitStopper I;

    [Header("Defaults (editable in Inspector)")]
    [Tooltip("Thời gian hitstop khi ENEMY ăn đòn (Player chém trúng)")]
    [Range(0f, 20f)] public float enemyHitStop = 0.06f;

    [Tooltip("Thời gian hitstop khi PLAYER ăn đòn (Enemy đánh trúng)")]
    [Range(0f, 20f)] public float playerHitStop = 0.08f;

    [Tooltip("TimeScale dùng khi hitstop (giữ rất nhỏ để không kẹt animation)")]
    [Range(0.00001f, 0.01f)] public float frozenTimeScale = 0.0001f;

    [Tooltip("Nếu false: hitstop mới sẽ ghi đè hitstop đang chạy")]
    public bool allowStacking = false;

    Coroutine current;
    float prevScale = 1f;

    void Awake()
    {
        // Singleton đơn giản
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // Gọi theo nhu cầu
    public void StopEnemyHit() => Stop(enemyHitStop);
    public void StopPlayerHit() => Stop(playerHitStop);

    public void Stop(float duration)
    {
        if (!allowStacking && current != null) StopCoroutine(current);
        current = StartCoroutine(DoStop(duration));
    }

    IEnumerator DoStop(float t)
    {
        prevScale = Time.timeScale;
        Time.timeScale = frozenTimeScale;

        float elapsed = 0f;
        while (elapsed < t)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = prevScale;
        current = null;
    }
}
