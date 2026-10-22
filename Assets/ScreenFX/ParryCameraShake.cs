using UnityEngine;
using Cinemachine;

public sealed class ParryCameraShake : MonoBehaviour
{
    [SerializeField] ParrySystem parrySystem;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField, Range(0, 1)] float power = 1f;
    [SerializeField] CombatCameraImpact cameraImpact;
    void OnEnable() { if (parrySystem) parrySystem.OnParrySuccess += OnParry; }
    void OnDisable() { if (parrySystem) parrySystem.OnParrySuccess -= OnParry; }
    void OnParry(ParrySystem.ParryContext ctx)
    {
        if (!cameraImpact || GamePauseController.InputBlocked || CombatSession.Finished) return;
        cameraImpact.Parry(ctx.contact.valid ? ctx.contact.point : transform.position,
            ctx.contact.valid ? ctx.contact.incoming : Vector3.left, power);
    }
}
