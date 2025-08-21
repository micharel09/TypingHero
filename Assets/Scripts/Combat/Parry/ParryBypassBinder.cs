using UnityEngine;

public sealed class ParryBypassBinder : MonoBehaviour
{
    [SerializeField] ParrySystem parry;
    [SerializeField] PlayerAttackEvents attackEvents;
    [SerializeField] float parrySuccessTTL = 1.0f;
    [SerializeField] float safetyHoldSeconds = 0.5f;

    Transform _lastTargetRoot;
    float _lastParryTimeUnscaled;

    void OnEnable()
    {
        if (parry) parry.OnParrySuccess += OnParrySuccess;
        if (attackEvents)
        {
            attackEvents.OnWindowOpen += OnOpen;
            attackEvents.OnWindowClose += OnClose;
        }
    }
    void OnDisable()
    {
        if (parry) parry.OnParrySuccess -= OnParrySuccess;
        if (attackEvents)
        {
            attackEvents.OnWindowOpen -= OnOpen;
            attackEvents.OnWindowClose -= OnClose;
        }
    }

    void OnParrySuccess(ParrySystem.ParryContext ctx)
    {
        _lastTargetRoot = ctx.targetRoot;
        _lastParryTimeUnscaled = Time.unscaledTime;
    }

    void OnOpen()
    {
        if (!_lastTargetRoot) return;
        if (Time.unscaledTime - _lastParryTimeUnscaled > parrySuccessTTL) return;
        UninterruptibleBypass.ActivateFor(_lastTargetRoot, safetyHoldSeconds);
    }

    void OnClose()
    {
        if (_lastTargetRoot) UninterruptibleBypass.ClearFor(_lastTargetRoot);
    }
}
