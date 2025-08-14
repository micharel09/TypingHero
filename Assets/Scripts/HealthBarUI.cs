using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public bool bindPlayer = true;          // true = Player, false = Skeleton
    public PlayerHealth player;
    public SkeletonController enemy;

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (bindPlayer && player != null) slider.maxValue = player.maxHealth;
        if (!bindPlayer && enemy != null) slider.maxValue = enemy.health; // dùng health hiện tại làm max
    }

    void Update()
    {
        if (bindPlayer && player != null) slider.value = player.current;
        if (!bindPlayer && enemy != null) slider.value = enemy.health;
    }
}
