using UnityEngine;

public enum EndlessRole { Swordsman, DoubleStrike, Heavy, Berserker, Guard, Captain }

// One asset owns a role's stats, presentation and attack vocabulary.
public sealed class EndlessOpponent : ScriptableObject
{
    public EndlessRole role;
    public string displayName, hint;
    public int health, damage = 5, hitCost = 12;
    public float recovery = 1.7f;
    public RuntimeAnimatorController controller;
    public Sprite idle;
    public AnimationClip hurt;
    public int[] opening = { 1 }, alternate = { 1 };
    public int[] triple = { 2, 1, 2 };

    internal int[] Pattern(int index, int round, float healthFraction)
    {
        if (role == EndlessRole.Captain)
        {
            int count = healthFraction > .7f ? 2 : 3;
            int choice = (index + (healthFraction < .35f ? round - 1 : 0)) % count;
            return choice == 0 ? opening : choice == 1 ? alternate : triple;
        }
        if (role == EndlessRole.Swordsman && round == 1) return opening;
        return index % 2 == 0 ? opening : alternate;
    }
}
