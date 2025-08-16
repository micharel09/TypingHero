using System.Collections;
using UnityEngine;

public class HitStopper : MonoBehaviour
{
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
    Coroutine co;

    public void Stop(float seconds)
    {
        if (seconds <= 0f) return;
        if (!allowStacking && active && Time.unscaledTime < restoreAt)  // đang hitstop rồi
        {
            // kéo dài thêm
            restoreAt = Time.unscaledTime + seconds;
            return;
        }
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoHitstop(seconds));
    }

    /// Gợi ý tiện dùng: truyền 2 giá trị và lấy cái lớn hơn
    public void Request(float player = 0f, float enemy = 0f)
    {
        float t = Mathf.Max(player, enemy);
        if (t > 0f) Stop(t);
    }

    IEnumerator CoHitstop(float seconds)
    {
        float prev = Time.timeScale;
        active = true;
        restoreAt = Time.unscaledTime + seconds;

        Time.timeScale = Mathf.Max(0.000001f, frozenTimeScale);
        while (Time.unscaledTime < restoreAt)
            yield return null;

        Time.timeScale = prev;
        active = false;
        co = null;
    }
}
