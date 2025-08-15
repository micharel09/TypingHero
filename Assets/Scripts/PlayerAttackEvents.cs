using UnityEngine;
using System.Collections;

public class PlayerAttackEvents : MonoBehaviour
{
    public PlayerWeaponHitbox hitbox;

    [Header("Window Config")]
    public float minOpenSeconds = 0.08f;

    bool windowOpen = false;
    float openedAt = -999f;
    float closedAt = -999f;
    Coroutine closeCo;

    // === Debug/leniency expose ===
    public bool IsWindowOpen => windowOpen;
    public float LastOpenTime => openedAt;
    public float LastCloseTime => closedAt;

    public void OnAttackStart()
    {
        ForceCloseWindow();
        windowOpen = true;
        openedAt = Time.time;
        hitbox.BeginAttack();
    }

    public void OnAttackEnd()
    {
        if (!windowOpen) return;
        float remain = Mathf.Max(0f, minOpenSeconds - (Time.time - openedAt));
        if (closeCo != null) StopCoroutine(closeCo);
        closeCo = StartCoroutine(CloseAfter(remain));
    }

    IEnumerator CloseAfter(float t)
    {
        if (t > 0f) yield return new WaitForSeconds(t);
        ForceCloseWindow();
    }

    public void ForceCloseWindow()
    {
        if (!windowOpen) return;
        if (closeCo != null) StopCoroutine(closeCo);
        windowOpen = false;
        closedAt = Time.time;
        hitbox.EndAttack();
    }
}
