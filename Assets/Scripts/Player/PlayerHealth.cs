using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int Current { get; private set; }

    [Header("Anim")]
    public Animator animator;
    public string hitStatePath = "Base Layer.player_hit";
    public string dieStatePath = "Base Layer.player_die";
    [Range(0f, .2f)] public float hitCrossfade = 0.02f;

    [Header("Damage Windows")]
    public float postHitIFrames = 0.05f;

    [Header("Refs")]
    public ParrySystem parry;
    public PlayerInputGate gate;

    [Header("Die Watchdog")]
    [Range(0.1f, 5f)] public float dieWatchdogTimeout = 2f;

    [Header("Debug")] public bool logs = false;

    float iFramesUntil;
    bool diedNotified;
    bool dieFlowStarted;
    public bool IsDead => Current <= 0;

    // Cho hệ khác nếu muốn nghe
    public event Action OnDied;

    void Awake()
    {
        if (Current <= 0) Current = maxHealth;
        diedNotified = false;
        dieFlowStarted = false;
    }

    public void FullRestore()
    {
        Current = maxHealth;
        diedNotified = false;
        dieFlowStarted = false;
        iFramesUntil = 0f;
        if (gate) gate.SetDeadLocked(false);
    }

    public void TakeDamage(int amount, Vector2 _)
    {
        if (IsDead) return;

        // Parry?
        if (parry && (parry.IsWindowActive || parry.IsSuccessActive))
        {
            if (logs) Debug.Log("[PARRY] SUCCESS => no damage");
            parry.ParrySuccess();
            return;
        }

        // i-frames
        if (Time.time < iFramesUntil) return;

        Current = Mathf.Max(0, Current - amount);
        if (logs) Debug.Log($"[DMG] player took {amount}, HP: {Current}");
        iFramesUntil = Time.time + postHitIFrames;

        if (Current <= 0)
        {
            if (!dieFlowStarted)
            {
                dieFlowStarted = true;
                if (gate) gate.SetDeadLocked(true);
                if (animator && !string.IsNullOrEmpty(dieStatePath))
                    AnimatorUtil.CrossFadePath(animator, dieStatePath, hitCrossfade, 0f);
                StartCoroutine(CoDieWatchdog());
            }
            return;
        }

        if (animator && !string.IsNullOrEmpty(hitStatePath))
            AnimatorUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
    }

    // Animation Event ở frame CUỐI clip player_die gọi hàm này
    public void Anim_DieCleanup() => FinalizeDeath();

    System.Collections.IEnumerator CoDieWatchdog()
    {
        float t = 0f;
        // Dự phòng trường hợp thiếu Animation Event
        while (!diedNotified && t < dieWatchdogTimeout)
        {
            if (animator && AnimatorUtil.IsInState(animator, dieStatePath, out var info) && info.normalizedTime >= 0.98f)
                break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        FinalizeDeath();
    }

    void FinalizeDeath()
    {
        if (diedNotified) return;
        diedNotified = true;

        // 1) Phát event cho ai đang lắng nghe
        var hasListener = OnDied != null;
        try { OnDied?.Invoke(); } catch { /* bỏ qua listener rác */ }

        // 2) Fallback tuyệt đối: tự gọi GameOver nếu không ai xử lý
        if (!hasListener)
        {
            var pause = FindObjectOfType<GamePauseController>();
            if (pause && pause.isActiveAndEnabled)
                pause.ShowGameOverAfterPlayerDie();
        }

        Destroy(gameObject);
    }
}
