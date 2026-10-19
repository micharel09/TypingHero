using System.Collections;
using UnityEngine;

public class HitStopper : MonoBehaviour
{
    // === Singleton ===
    public static HitStopper I { get; private set; }
    void Awake()
    {
        if (I != null && I != this) { Destroy(this); return; }
        I = this;
    }

    [Header("Defaults (editable in Inspector)")]
    [Tooltip("Hitstop mặc định áp cho Enemy khi có va chạm thường (s)")]
    public float enemyHitStop = 0.01f;
    [Tooltip("Hitstop mặc định áp cho Player khi có va chạm thường (s)")]
    public float playerHitStop = 0.03f;
    [Tooltip("Time.timeScale khi đang hitstop (không nên = 0).")]
    public float frozenTimeScale = 0.0001f;
    [Tooltip("Cho phép chồng nhiều hitstop (ít dùng).")]
    public bool allowStacking = false;

    bool active;
    float restoreAt;
    float restoreTimeScale;
    Coroutine co;

    public void Stop(float seconds)
    {
        if (seconds <= 0f || GamePauseController.InputBlocked) return;
        if (!allowStacking && active && Time.unscaledTime < restoreAt)
        {
            restoreAt = Time.unscaledTime + seconds;
            return;
        }
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoHitstop(seconds));
    }

    /// Gọi nhanh theo phía bị hit
    public void StopEnemyHit() => Stop(enemyHitStop);
    public void StopPlayerHit() => Stop(playerHitStop);

    public void Request(float player = 0f, float enemy = 0f)
    {
        float t = Mathf.Max(player, enemy);
        if (t > 0f) Stop(t);
    }

    IEnumerator CoHitstop(float seconds)
    {
        // A new strike may replace an expired coroutine before its continuation
        // restores time. Preserve the original scale across that replacement.
        if (!active) restoreTimeScale = Time.timeScale;
        active = true;
        restoreAt = Time.unscaledTime + seconds;

        Time.timeScale = Mathf.Max(0.000001f, frozenTimeScale);
        while (Time.unscaledTime < restoreAt || GamePauseController.InputBlocked)
        {
            if (GamePauseController.InputBlocked) restoreAt += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = restoreTimeScale;
        active = false;
        co = null;
    }
}
