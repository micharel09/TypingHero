using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfig", menuName = "Combat/AttackConfig")]
public class AttackConfig : ScriptableObject
{
    [Header("Animator")]
    public string statePath = "Base Layer.attack1";
    [Range(0f, 0.1f)] public float crossfade = 0.02f;
    [Range(0f, 1f)] public float startTime = 0.02f;

    [Header("Hit Window & Damage")]
    public float minOpenSeconds = 0.08f;     // >= 2 * fixedDeltaTime
    public int damage = 10;
    public LayerMask targetLayers;           // set = Enemy
}
