using UnityEngine;
using System.Collections;

public class PlayerAttackEvents : MonoBehaviour
{
    public PlayerWeaponHitbox hitbox;

    [Header("Window Config")]
    public float minOpenSeconds = 0.08f;

    public event System.Action OnWindowOpen;
    public event System.Action OnWindowClose;


    // id duy nhất cho mỗi cú vung
    private static int globalAttackCounter = 0;
    private int currentAttackId = 0;

    private bool open;
    private float openedAt;
    private Coroutine closeCo;

    public void OnAttackStart()
    {
        if (hitbox == null) return;

        if (closeCo != null) { StopCoroutine(closeCo); closeCo = null; }

        open = true;
        openedAt = Time.time;

        currentAttackId = ++globalAttackCounter;   // tăng id cho cú vung mới
        hitbox.BeginAttack(currentAttackId);       // ✅ truyền attackId
        OnWindowOpen?.Invoke();
    }

    public void OnAttackEnd()
    {
        if (!open) return;

        float remain = Mathf.Max(0f, minOpenSeconds - (Time.time - openedAt));
        if (closeCo != null) StopCoroutine(closeCo);
        closeCo = StartCoroutine(CloseAfter(remain));
        OnWindowClose?.Invoke();
    }

    private IEnumerator CloseAfter(float t)
    {
        if (t > 0f) yield return new WaitForSeconds(t);
        open = false;
        if (hitbox != null) hitbox.EndAttack();
        closeCo = null;
    }
}
