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
    private bool closeNotified;
    private Sprite currentOpeningCel;

    void OnDisable() => CancelAttack();

    internal void CancelAttack(Sprite expectedOpeningCel = null)
    {
        if (expectedOpeningCel && currentOpeningCel != expectedOpeningCel) return;
        if (closeCo != null) { StopCoroutine(closeCo); closeCo = null; }
        bool notify = open && !closeNotified;
        open = false;
        closeNotified = true;
        currentOpeningCel = null;
        if (hitbox) hitbox.EndAttack();
        if (notify) OnWindowClose?.Invoke();
    }

    public void OnAttackStart() => StartAttack(null, false);
    internal void OnSlayerAttackStart() => StartAttack(null, true);

    public void OnAttackStartCel(Sprite cel) => StartAttack(cel, true);
    public void OnAttackContact(Sprite cel) { if (hitbox) hitbox.SetContactCel(cel); }
    public void OnParryFollowUpEnd(Sprite openingCel) => CancelAttack(openingCel);

    void StartAttack(Sprite cel, bool confirmImmediately)
    {
        if (hitbox == null) return;

        if (closeCo != null) { StopCoroutine(closeCo); closeCo = null; }

        if (open && !closeNotified) { closeNotified = true; OnWindowClose?.Invoke(); }
        open = true;
        closeNotified = false;
        currentOpeningCel = cel;
        openedAt = Time.time;

        currentAttackId = ++globalAttackCounter;
        hitbox.BeginAttack(currentAttackId);
        hitbox.SetContactCel(cel);

        // Đẩy transform ra PhysX ngay frame này (để tiếp xúc chính xác)
        Physics2D.SyncTransforms();

        OnWindowOpen?.Invoke();
        if (confirmImmediately) hitbox.ConfirmOverlaps();
    }

    public void OnAttackEnd()
    {
        if (!open) return;

        float remain = Mathf.Max(0f, minOpenSeconds - (Time.time - openedAt));
        if (closeCo != null) StopCoroutine(closeCo);
        closeCo = StartCoroutine(CloseAfter(remain));
        if (!closeNotified) { closeNotified = true; OnWindowClose?.Invoke(); }
    }

    private IEnumerator CloseAfter(float t)
    {
        if (t > 0f)
            yield return new WaitForSeconds(t);      // scaled time
        else
            yield return new WaitForFixedUpdate();   // bảo đảm có ÍT NHẤT 1 tick vật lý

        open = false;
        if (hitbox != null) hitbox.EndAttack();
        closeCo = null;
    }
}
