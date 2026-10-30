using UnityEngine;

// Restart/abandon do not save progress; completed and explicitly skipped stay distinct.
internal static class TutorialRun
{
    internal enum Progress { Unseen, Skipped, Completed }
    const string Preference = "TutorialProgress_v1";
    internal static Progress Status => (Progress)PlayerPrefs.GetInt(Preference, 0);
    internal static bool Active { get; private set; }
    internal enum Practice { None, Typing, Parry }
    internal static Practice Practicing { get; set; }

    internal static void Begin()
    {
        Active = true;
        Practicing = Practice.None;
    }

    internal static void End() { Active = false; Practicing = Practice.None; }
    internal static void Complete() => Save(Progress.Completed);
    internal static void Skip() { if (Status != Progress.Completed) Save(Progress.Skipped); }
    static void Save(Progress progress)
    {
        PlayerPrefs.SetInt(Preference, (int)progress);
        PlayerPrefs.Save();
    }
}
