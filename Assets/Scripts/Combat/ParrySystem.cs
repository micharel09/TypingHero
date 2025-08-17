using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;
    public Animator playerAnimator;
    public ParryTarget enemy;

    [Header("Hitstop (parry success)")]
    public HitStopper hitStopper;
    public float parrySuccessHitStop = 0.05f;
    public float successIFrame = 0.08f;

    [Header("Input Gate")]
    public PlayerInputGate gate;                       // <-- đảm bảo đã kéo vào

    [Header("Debug")] public bool logs = true;

    bool windowOpen;
    float successUntilUnscaled;

    public bool IsWindowActive => windowOpen;
    public bool IsSuccessActive => Time.unscaledTime < successUntilUnscaled;
    public bool PoseLocked => InParryOrSuccess();

    void OnEnable() { if (enemy) enemy.OnStrike += OnEnemyStrike; }
    void OnDisable() { if (enemy) enemy.OnStrike -= OnEnemyStrike; }

    bool InParryOrSuccess()
    {
        if (!playerAnimator || !config) return false;
        bool inPose = !string.IsNullOrEmpty(config.playerParryPoseStatePath) &&
                      AnimUtil.IsInState(playerAnimator, config.playerParryPoseStatePath, out _);
        bool inSucc = !string.IsNullOrEmpty(config.playerParrySuccessStatePath) &&
                      AnimUtil.IsInState(playerAnimator, config.playerParrySuccessStatePath, out _);
        return inPose || inSucc;
    }

    // --- Animation Events ---
    public void Anim_ParryOpen()
    {
        if (gate && gate.IsDeadLocked) return;         // <-- CHẶN nếu đã chết
        windowOpen = true;
        successUntilUnscaled = 0f;
        if (logs) Debug.Log("[PARRY] Pose opened");
    }
    public void Anim_ParryClose()
    {
        if (gate && gate.IsDeadLocked) return;         // <-- CHẶN nếu đã chết
        windowOpen = false;
        if (logs) Debug.Log("[PARRY] Pose closed");
    }

    void OnEnemyStrike()
    {
        if (gate && gate.IsDeadLocked) return;         // <-- CHẶN nếu đã chết
        if (!windowOpen) return;

        windowOpen = false;
        successUntilUnscaled = Time.unscaledTime + successIFrame;

        if (hitStopper && parrySuccessHitStop > 0f)
            hitStopper.Stop(parrySuccessHitStop);

        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);

        if (logs) Debug.Log($"[PARRY] OK (t={Time.unscaledTime:0.000})");
    }

    public void ParrySuccess() => OnEnemyStrike();

    void Update()
    {
        if (!config) return;

        // ❌ đã chết hoặc gate không cho parry → bỏ qua
        if (gate && !gate.CanParry) return;            // <-- ĐỦ để chặn Space sau khi chết

        if (Input.GetKeyDown(config.parryKey))
        {
            if (InParryOrSuccess()) return;

            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
        }
    }
}
