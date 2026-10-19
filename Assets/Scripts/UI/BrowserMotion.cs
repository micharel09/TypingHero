using System.Runtime.InteropServices;

internal static class BrowserMotion
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern int TypingHero_ReducedMotion();
    internal static bool Reduced => TypingHero_ReducedMotion() != 0;
#else
    internal static bool Reduced => false;
#endif
}
