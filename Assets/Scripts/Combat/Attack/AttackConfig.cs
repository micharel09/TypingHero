using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackConfig", fileName = "AC_Attack")]
public class AttackConfig : ScriptableObject
{
    [Header("Animator State")]
    public string statePath = "Base Layer.attack1";
    [Min(0f)] public float crossfade = 0.0f;     // 0–0.02
    [Range(0f, 1f)] public float startTime = 0.0f; // 0–0.02
}
