using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] EnemyStamina target;

    Slider _slider;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.wholeNumbers = true; // stamina kiểu số nguyên
    }

    void OnEnable()
    {
        if (target) Bind(target);
    }

    void OnDisable()
    {
        if (target) Unbind(target);
    }

    public void Bind(EnemyStamina t)
    {
        if (target) Unbind(target);
        target = t;
        if (!target) return;

        target.OnChanged += OnStaminaChanged;

        // Giá trị khởi tạo
        _slider.maxValue = target.max;       
        _slider.value    = target.Current;
    }

    public void Unbind(EnemyStamina t)
    {
        if (!t) return;
        t.OnChanged -= OnStaminaChanged;
    }

    void OnStaminaChanged(int current, int max)
    {
        if (_slider.maxValue != max) _slider.maxValue = max;
        _slider.value = current;
    }

    // Tuỳ chọn cho code khác gọi
    public void SetTarget(EnemyStamina t) => Bind(t);
}
