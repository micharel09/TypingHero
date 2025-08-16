using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    [Tooltip("true = thanh máu Player, false = thanh máu Enemy")]
    public bool bindPlayer = true;

    public PlayerHealth player;            // nếu bindPlayer = true
    public SkeletonController enemy;       // nếu bindPlayer = false

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
    }

    void Start()
    {
        // set Max 1 lần khi bắt đầu (nếu sau này bạn có thay Max lúc chơi,
        // có thể cập nhật lại trong Update)
        if (bindPlayer && player)
            slider.maxValue = player.maxHealth;
        else if (!bindPlayer && enemy)
            slider.maxValue = enemy.health;     // lấy máu hiện có làm max cho enemy
    }

    void Update()
    {
        if (!slider) return;

        if (bindPlayer && player)
        {
            slider.value = player.Current;
            // đề phòng khi Max thay đổi lúc đang chơi
            if (slider.maxValue != player.maxHealth)
                slider.maxValue = player.maxHealth;
        }
        else if (!bindPlayer && enemy)
        {
            slider.value = enemy.health;
        }
    }
}
