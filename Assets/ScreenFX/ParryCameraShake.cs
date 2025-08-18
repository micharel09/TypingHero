using UnityEngine;
using Cinemachine;

public sealed class ParryCameraShake : MonoBehaviour
{
    [SerializeField] ParrySystem parrySystem;                 // kéo ParrySystem (Player)
    [SerializeField] CinemachineImpulseSource impulseSource;  
    [SerializeField] float power = 1f;                        

    void OnEnable() { if (parrySystem) parrySystem.OnParrySuccess += OnParry; }
    void OnDisable() { if (parrySystem) parrySystem.OnParrySuccess -= OnParry; }

    void OnParry(ParrySystem.ParryContext ctx)
    {
        if (!impulseSource) return;
        impulseSource.GenerateImpulse(power);
    }
}
