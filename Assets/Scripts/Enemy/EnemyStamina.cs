using System;
using System.Collections;
using UnityEngine;

public class EnemyStamina : MonoBehaviour
{
    [Header("Stamina")]
    public int max = 25;
    public int parryCost = 25;

    [Header("Hit (normal attack)")]
    [Tooltip("Mức trừ khi dính đòn đánh thường của Player (nhỏ hơn parryCost).")]
    public int hitCost = 12;

    [Header("Regen")]
    [Tooltip("Bắt đầu hồi sau khi không bị parry/hit trong khoảng này (giây).")]
    [SerializeField] float regenDelaySeconds = 4f;
    [Tooltip("Khoảng cách giữa các tick hồi.")]
    [SerializeField] float regenTickInterval = 0.2f;
    [Tooltip("Điểm hồi mỗi tick.")]
    [SerializeField] int regenPerTick = 1;
    [Tooltip("Có hồi khi đang stun không?")]
    [SerializeField] bool regenDuringStun = true;

    [Header("QoL")]
    [Tooltip("Nếu Stamina = 0, sẽ refill FULL ngay khi kết thúc 1 chu kỳ stun kế tiếp.")]
    [SerializeField] bool refillToMaxOnStunEnd = true;

    public event Action<int, int> OnChanged;
    public int Current { get; private set; }

    WaitForSecondsRealtime _waitDelay, _waitTick;
    Coroutine _regenCo;
    Coroutine _refillCo;

    EnemyStunController _stun;
    bool _awaitRefillAfterStun;

    void Awake()
    {
        Current = max;
        _waitDelay = new WaitForSecondsRealtime(regenDelaySeconds);
        _waitTick = new WaitForSecondsRealtime(regenTickInterval);
        TryGetComponent(out _stun);
        NotifyChange();
    }

    void OnValidate()
    {
        if (regenDelaySeconds < 0f) regenDelaySeconds = 0f;
        if (regenTickInterval < 0.05f) regenTickInterval = 0.05f;
        if (regenPerTick < 1) regenPerTick = 1;
        if (hitCost < 0) hitCost = 0;
        if (parryCost < 0) parryCost = 0;
    }

    void NotifyChange() => OnChanged?.Invoke(Current, max);

    // --- gọi khi parry thành công ---
    public void ConsumeParry()
    {
        ApplyCost(parryCost);
    }
    public void NotifyParried() => StartRegenCountdown();

    // --- gọi khi dính đòn đánh thường ---
    public void ConsumeHit() => ApplyCost(hitCost);
    public void ConsumeHit(int customCost) => ApplyCost(Mathf.Max(0, customCost));
    public void NotifyHit() => StartRegenCountdown();

    // --- core trừ stamina chung ---
    void ApplyCost(int cost)
    {
        if (cost <= 0) { StartRegenCountdown(); return; }

        int before = Current;
        Current = Mathf.Max(0, Current - cost);
        if (Current != before) NotifyChange();

        // nếu vừa tụt về 0 -> chờ hết stun để refill
        if (refillToMaxOnStunEnd && Current == 0)
        {
            _awaitRefillAfterStun = true;
            EnsureRefillAfterStunCoroutine();
        }

        // reset countdown regen
        StartRegenCountdown();
    }

    void StartRegenCountdown()
    {
        if (_regenCo != null) StopCoroutine(_regenCo);
        _waitDelay = new WaitForSecondsRealtime(regenDelaySeconds); // nếu đổi số lúc runtime
        _regenCo = StartCoroutine(CoRegen());
    }

    IEnumerator CoRegen()
    {
        yield return _waitDelay;

        while (Current < max)
        {
            if (!regenDuringStun && _stun && _stun.IsStunned)
            {
                yield return _waitTick;
                continue;
            }

            Current = Mathf.Min(max, Current + regenPerTick);
            NotifyChange();
            yield return _waitTick;
        }
        _regenCo = null;
    }

    // --------- Refill FULL ngay sau khi kết thúc stun tiếp theo ---------
    void EnsureRefillAfterStunCoroutine()
    {
        if (_refillCo != null) StopCoroutine(_refillCo);
        _refillCo = StartCoroutine(CoRefillAfterStun());
    }

    IEnumerator CoRefillAfterStun()
    {
        // chờ có reference stun
        while (_stun == null) { TryGetComponent(out _stun); yield return null; }

        // chờ cho tới khi thực sự vào stun ít nhất 1 lần
        while (!_stun.IsStunned) yield return null;

        // và bây giờ đợi stun kết thúc
        while (_stun.IsStunned) yield return null;

        if (_awaitRefillAfterStun)
        {
            _awaitRefillAfterStun = false;

            // dừng regen rồi refill full
            if (_regenCo != null) { StopCoroutine(_regenCo); _regenCo = null; }

            Current = max;
            NotifyChange();
        }
        _refillCo = null;
    }
}
