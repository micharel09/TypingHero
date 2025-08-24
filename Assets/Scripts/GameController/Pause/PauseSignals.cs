using System;

public static class PauseSignals
{
    public static bool Active { get; private set; }
    public static event Action<bool> OnChanged;

    public static void Set(bool on)
    {
        if (Active == on) return;
        Active = on;
        OnChanged?.Invoke(on);
    }
}
