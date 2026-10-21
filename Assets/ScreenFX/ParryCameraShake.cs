using UnityEngine;
using Cinemachine;

public sealed class ParryCameraShake : MonoBehaviour
{
    [SerializeField] ParrySystem parrySystem;                 // kéo ParrySystem (Player)
    [SerializeField] CinemachineImpulseSource impulseSource;  
    [SerializeField, Range(0, 1)] float power = 1f;
    bool pending;
    float elapsed, delay;
    float impulseEndTime;
    Vector3 position, velocity;
    CinemachineImpulseManager.ImpulseEvent activeImpulse;

    void OnEnable() { if (parrySystem) parrySystem.OnParrySuccess += OnParry; }
    void OnDisable() { if (parrySystem) parrySystem.OnParrySuccess -= OnParry; Cancel(); }

    void OnParry(ParrySystem.ParryContext ctx)
    {
        if (!impulseSource || GamePauseController.InputBlocked || CombatSession.Finished) return;
        Cancel();
        var camera = Camera.main;
        float pixel = camera && camera.orthographic ? camera.orthographicSize * 2 * camera.aspect / 320 : 1f / 16;
        Vector3 direction = ctx.contact.valid ? ctx.contact.incoming : Vector3.left;
        if (camera) direction = Vector3.ProjectOnPlane(direction, camera.transform.forward);
        velocity = direction.normalized * (pixel * power);
        position = ctx.contact.valid ? ctx.contact.point : transform.position;
        delay = parrySystem ? parrySystem.parrySuccessHitStop : .05f;
        elapsed = 0; pending = true;
    }
    void Update()
    {
        if (CombatSession.Finished || GamePauseController.InputBlocked || Time.timeScale == 0) { Cancel(); return; }
        if (!pending) return;
        elapsed += Time.unscaledDeltaTime;
        if (elapsed < delay || Time.timeScale < .01f) return;
        pending = false;
        activeImpulse = impulseSource.m_ImpulseDefinition.CreateAndReturnEvent(position, velocity);
        impulseEndTime = CinemachineImpulseManager.Instance.CurrentTime + impulseSource.m_ImpulseDefinition.m_ImpulseDuration;
    }
    void Cancel()
    {
        pending = false;
        // The manager pools expired events: never cancel a recycled event belonging to a different effect.
        if (activeImpulse != null && CinemachineImpulseManager.Instance.CurrentTime < impulseEndTime)
            activeImpulse.Cancel(CinemachineImpulseManager.Instance.CurrentTime, true);
        activeImpulse = null;
    }
}
