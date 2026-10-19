using UnityEngine;

// IDs are storage/network contracts. Normal retains all pre-difficulty keys.
public enum EndlessDifficulty { VeryEasy, Easy, Normal, Hard, VeryHard }

internal static class EndlessDifficulties
{
    internal const string SelectionKey = "EndlessDifficulty_v1";
    internal static readonly EndlessDifficulty[] All = { EndlessDifficulty.VeryEasy, EndlessDifficulty.Easy,
        EndlessDifficulty.Normal, EndlessDifficulty.Hard, EndlessDifficulty.VeryHard };
    internal static string Id(this EndlessDifficulty value)
    {
        switch (value)
        {
            case EndlessDifficulty.VeryEasy: return "very_easy";
            case EndlessDifficulty.Easy: return "easy";
            case EndlessDifficulty.Hard: return "hard";
            case EndlessDifficulty.VeryHard: return "very_hard";
            default: return "normal";
        }
    }
    internal static string Label(this EndlessDifficulty value)
    {
        switch (value)
        {
            case EndlessDifficulty.VeryEasy: return "Very Easy";
            case EndlessDifficulty.Easy: return "Easy";
            case EndlessDifficulty.Hard: return "Hard";
            case EndlessDifficulty.VeryHard: return "Very Hard";
            default: return "Normal";
        }
    }
    // Candidate values from the approved plan; runtime reference acceptance is recorded in the checkpoint.
    internal static float Pace(this EndlessDifficulty value)
    {
        switch (value)
        {
            case EndlessDifficulty.VeryEasy: return .6f;
            case EndlessDifficulty.Easy: return .8f;
            case EndlessDifficulty.Hard: return 1.25f;
            case EndlessDifficulty.VeryHard: return 1.5f;
            default: return 1f;
        }
    }
    internal static EndlessDifficulty Selected
    {
        get
        {
            string id = PlayerPrefs.GetString(SelectionKey, "normal");
            foreach (var value in All) if (value.Id() == id) return value;
            return EndlessDifficulty.Normal;
        }
        set { PlayerPrefs.SetString(SelectionKey, value.Id()); PlayerPrefs.Save(); }
    }
    internal static EndlessDifficulty Step(EndlessDifficulty value, int direction) =>
        All[((int)value + direction + All.Length) % All.Length];
    internal static string StorageKey(EndlessDifficulty value, string legacy) =>
        value == EndlessDifficulty.Normal ? legacy : legacy + "/" + value.Id();
    internal static string Path(EndlessDifficulty value) => value == EndlessDifficulty.Normal ?
        "leaderboards/endless_v1" : "leaderboards/endless_v2/" + value.Id();
}
