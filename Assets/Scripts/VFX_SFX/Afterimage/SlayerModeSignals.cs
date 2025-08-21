using System;

public static class SlayerModeSignals
{
    public static bool Active { get; private set; }
    public static event Action<bool> OnSetActive;
    public static void SetActive(bool active)
    {
        if (Active == active) return;
        Active = active;
        OnSetActive?.Invoke(active);
    }
}
