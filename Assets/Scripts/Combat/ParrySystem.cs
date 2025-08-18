using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;
    public Animator playerAnimator;
    public ParryTarget enemy;                  // enemy hiện tại (đã có OnStrike)

    [Header("Hitstop (parry success)")]
    public HitStopper hitStopper;
    public float parrySuccessHitStop = 0.05f;
    public float successIFrame = 0.08f;

    [Header("Input Gate")]
    public PlayerInputGate gate;

    [Header("Debug")]
    public bool logs = true;

    // ========= Event dành cho VFX/SFX khi parry thành công =========
    public struct ParryContext
    {
        public Transform targetRoot;          // dùng để xoay spark/sfx về phía enemy (có thể null)
    }
    public event System.Action<ParryContext> OnParrySuccess;
    // ===============================================================

    bool windowOpen;
    float successUntilUnscaled;

    public bool IsWindowActive => windowOpen;
    public bool IsSuccessActive => Time.unscaledTime < successUntilUnscaled;
    public bool PoseLocked => InParryOrSuccess();

    void OnEnable()
    {
        if (enemy) enemy.OnStrike += OnEnemyStrike;   // ParryTarget phải phát OnStrike(IParryTarget)
    }

    void OnDisable()
    {
        if (enemy) enemy.OnStrike -= OnEnemyStrike;
    }

    bool InParryOrSuccess()
    {
        if (!playerAnimator || !config) return false;

        bool inPose = !string.IsNullOrEmpty(config.playerParryPoseStatePath) &&
                      AnimUtil.IsInState(playerAnimator, config.playerParryPoseStatePath, out _);

        bool inSucc = !string.IsNullOrEmpty(config.playerParrySuccessStatePath) &&
                      AnimUtil.IsInState(playerAnimator, config.playerParrySuccessStatePath, out _);

        return inPose || inSucc;
    }

    // ---------------- Animation Events ----------------
    public void Anim_ParryOpen()
    {
        if (gate && gate.IsDeadLocked) return;    // chặn nếu đã chết
        windowOpen = true;
        successUntilUnscaled = 0f;
        if (logs) Debug.Log("[PARRY] Pose opened");
    }

    public void Anim_ParryClose()
    {
        if (gate && gate.IsDeadLocked) return;    // chặn nếu đã chết
        windowOpen = false;
        if (logs) Debug.Log("[PARRY] Pose closed");
    }
    // --------------------------------------------------
    void OnEnemyStrike(IParryTarget enemyTarget)
    {
        if (gate && gate.IsDeadLocked) return;
        if (!windowOpen) return;

        // đánh dấu parry thành công
        windowOpen = false;
        successUntilUnscaled = Time.unscaledTime + successIFrame;

        // hitstop nảy nhẹ
        if (hitStopper && parrySuccessHitStop > 0f)
            hitStopper.Stop(parrySuccessHitStop);

        // phát clip parry_success
        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);   // ← hết crossfade tại đây  :contentReference[oaicite:3]{index=3}

        // phát sự kiện cho VFX/SFX (spark, âm thanh…)
        Transform targetRoot = null;
        if (enemyTarget is Component c) targetRoot = c.transform;
        OnParrySuccess?.Invoke(new ParryContext { targetRoot = targetRoot }); // ← event nằm sau chỗ bạn vừa chèn.  :contentReference[oaicite:4]{index=4}

        if (logs) Debug.Log($"[PARRY] OK (t={Time.unscaledTime:0.000})");
    }


    public void ParrySuccess() { /* obsolete: handled in OnEnemyStrike */ }

    void Update()
    {
        if (!config) return;

        // gate từ PlayerInputGate: không cho mở parry nếu đang bị khoá (chết/khác)
        if (gate && !gate.CanParry) return;

        if (Input.GetKeyDown(config.parryKey))
        {
            if (InParryOrSuccess()) return;

            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
        }
    }
}
