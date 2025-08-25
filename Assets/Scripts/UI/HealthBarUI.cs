using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] bool bindPlayer = true;

    [SerializeField] PlayerHealth player;       // nếu bindPlayer = true
    [SerializeField] SkeletonController enemy;  // nếu bindPlayer = false

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
    }

    void Start()
    {
        RebindIfNeeded(true);   // set maxValue ngay từ đầu
    }

    void Update()
    {
        if (!slider) return;

        // Luôn bảo đảm reference còn sống; nếu mất (sau restart) thì tự tìm lại
        RebindIfNeeded(false);

        if (bindPlayer)
        {
            if (!player) return;
            if (slider.maxValue != player.maxHealth) slider.maxValue = player.maxHealth;
            slider.value = player.Current;
        }
        else
        {
            if (!enemy) return;
            if (slider.maxValue != enemy.maxHealth) slider.maxValue = enemy.maxHealth;
            slider.value = enemy.Current;
        }
    }

    void RebindIfNeeded(bool force)
    {
        if (bindPlayer)
        {
            if (force || !player)
            {
                player = FindObjectOfType<PlayerHealth>();
                if (player && slider) slider.maxValue = player.maxHealth;
            }
        }
        else
        {
            if (force || !enemy)
            {
                enemy = FindObjectOfType<SkeletonController>();
                if (enemy && slider) slider.maxValue = enemy.maxHealth;
            }
        }
    }
}
