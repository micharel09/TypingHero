using UnityEngine;

internal static class EndlessProfile
{
    const string Prefix = "EndlessProfile_v1_";
    internal const string FirstBoss = "FirstBoss", TripleParry = "TripleParry", PerfectBoss = "PerfectBoss";
    internal static int Encounters(EndlessRole role) => PlayerPrefs.GetInt(Prefix + role + "_seen");
    internal static int Defeats(EndlessRole role) => PlayerPrefs.GetInt(Prefix + role + "_kills");
    internal static bool Has(string achievement) => PlayerPrefs.GetInt(Prefix + achievement) == 1;
    internal static int Elites(EndlessRole role, bool kills) => PlayerPrefs.GetInt(Prefix + role + (kills ? "_elite_kills" : "_elite_seen"));
    internal static void Encounter(EndlessRole role, bool elite)
    {
        Increment(role + "_seen");
        if (elite) Increment(role + "_elite_seen");
    }
    internal static void Defeat(EndlessRole role, bool elite)
    {
        Increment(role + "_kills");
        if (elite) Increment(role + "_elite_kills");
    }
    static void Increment(string key) { PlayerPrefs.SetInt(Prefix + key, PlayerPrefs.GetInt(Prefix + key) + 1); PlayerPrefs.Save(); }
    internal static void Unlock(string achievement) { PlayerPrefs.SetInt(Prefix + achievement, 1); PlayerPrefs.Save(); }
}
