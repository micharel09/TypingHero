using UnityEngine;

// A scene reload resets combat; only this session checkpoint survives a lesson retry.
internal static class TutorialRun
{
    internal enum Progress { Unseen, Skipped, Completed }
    const string Preference = "TutorialProgress_v1";
    internal static Progress Status => (Progress)PlayerPrefs.GetInt(Preference, 0);
    internal static bool Active { get; private set; }
    internal static int Checkpoint { get; set; }
    internal static bool Retrying { get; set; }

    internal static void Begin()
    {
        Active = true;
        Checkpoint = 0;
        Retrying = false;
    }

    internal static void End() { Active = false; Checkpoint = 0; Retrying = false; }
    internal static void Complete() => Save(Progress.Completed);
    internal static void Skip()
    {
        if (Status != Progress.Completed) Save(Progress.Skipped);
    }

    static void Save(Progress progress)
    {
        PlayerPrefs.SetInt(Preference, (int)progress);
        PlayerPrefs.Save();
    }
}
