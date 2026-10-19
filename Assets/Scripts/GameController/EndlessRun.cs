using UnityEngine;

// Only explicit run data survives encounter scene reloads. Combat objects never do.
internal sealed class EndlessRun
{
    internal const int DuelsPerRound = 6, BossHeal = 10;
    internal static readonly bool RepeatAfterBoss = false;
    internal bool Completed => !RepeatAfterBoss && Defeated >= DuelsPerRound;
    internal static EndlessRun Current { get; private set; }
    internal static bool Continuing { get; set; }
    static bool prepared;
    internal EndlessDifficulty Difficulty { get; private set; }
    internal int Defeated, Health, Score, Letters, Correct, Parries;
    internal float Seconds;
    internal int Round => Defeated / DuelsPerRound + 1;
    internal int Slot => Defeated % DuelsPerRound;
    internal static EndlessRun Begin(EndlessDifficulty difficulty = EndlessDifficulty.Normal) =>
        Current = new EndlessRun { Difficulty = difficulty };
    internal static void Prepare()
    { Begin(EndlessDifficulties.Selected); prepared = true; }
    internal static void Restart()
    {
        var difficulty = Current != null ? Current.Difficulty : EndlessDifficulties.Selected;
        Begin(difficulty); Continuing = false; prepared = true;
    }
    internal static void Clear() { Current = null; Continuing = prepared = false; }
    internal static EndlessRun EnterScene()
    {
        var run = (Continuing || prepared) && Current != null ? Current : Begin(EndlessDifficulties.Selected);
        Continuing = prepared = false;
        return run;
    }

    internal EndlessRole Role
    {
        get
        {
            if (Slot == 5) return EndlessRole.Captain;
            if (Slot == 4) return Round == 1 ? EndlessRole.Swordsman : Round == 2 ? EndlessRole.Heavy : (EndlessRole)((Round - 3) % 5);
            if (Round == 1) return new[] { EndlessRole.Swordsman, EndlessRole.DoubleStrike, EndlessRole.Heavy, EndlessRole.Guard }[Slot];
            if (Round == 2) return new[] { EndlessRole.Berserker, EndlessRole.DoubleStrike, EndlessRole.Guard, EndlessRole.Heavy }[Slot];
            return (EndlessRole)((Round - 3 + Slot) % 5);
        }
    }
    internal EndlessOpponent Definition => Resources.Load<EndlessOpponent>("Endless/" + Role);
    internal readonly struct Opponent
    {
        internal readonly string Name, Hint;
        internal readonly int Health, Stamina;
        internal Opponent(string name, string hint, int health, int stamina)
        { Name = name; Hint = hint; Health = health; Stamina = stamina; }
    }
    internal Opponent Enemy
    {
        get
        {
            var definition = Definition;
            return new Opponent((Slot == 4 ? "ELITE / " : "") + definition.displayName,
                definition.hint, Slot == 4 ? 300 : definition.health, Slot == 5 ? 200 : Slot == 4 ? 150 : 100);
        }
    }
    internal float Recovery => Mathf.Max(.7f, Definition.recovery - (Slot == 4 ? .2f : 0) - Mathf.Min(Round - 1, 8) * .12f) / Difficulty.Pace();

    internal void Capture(CombatSession session, int hp)
    {
        Health = hp; Score = session.FinalScore; Seconds = session.Seconds;
        Letters = session.Letters; Correct = session.Correct; Parries = session.Parries;
    }
    internal void Win(int maxHealth)
    {
        if (Completed) return;
        if (RepeatAfterBoss && Slot == DuelsPerRound - 1) Health = Mathf.Min(maxHealth, Health + BossHeal);
        Defeated++;
    }

    internal const string BestDefeatedKey = "EndlessBestDefeated_v1", BestScoreKey = "EndlessBestScore_v1";
    internal bool SaveBest()
    {
        string defeatedKey = EndlessDifficulties.StorageKey(Difficulty, BestDefeatedKey);
        string scoreKey = EndlessDifficulties.StorageKey(Difficulty, BestScoreKey);
        int defeated = PlayerPrefs.GetInt(defeatedKey, 0), score = PlayerPrefs.GetInt(scoreKey, 0);
        if (Defeated < defeated || (Defeated == defeated && Score <= score)) return false;
        PlayerPrefs.SetInt(defeatedKey, Defeated); PlayerPrefs.SetInt(scoreKey, Score);
        PlayerPrefs.Save();
        return true;
    }
}
