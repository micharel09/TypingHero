using UnityEngine;

// Reading survives restart; only explicit Start Battle marks the guide as viewed.
internal static class TutorialRun
{
    internal enum Progress { Unseen, Skipped, Completed }
    const string Preference = "TutorialProgress_v1";
    internal static Progress Status => (Progress)PlayerPrefs.GetInt(Preference, 0);
    internal static bool Active { get; private set; }

    internal static void Begin()
    {
        Active = true;
    }

    internal static void End() { Active = false; }
    internal static void Complete() => Save(Progress.Completed);
    static void Save(Progress progress)
    {
        PlayerPrefs.SetInt(Preference, (int)progress);
        PlayerPrefs.Save();
    }
}
