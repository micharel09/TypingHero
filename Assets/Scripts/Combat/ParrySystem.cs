using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;
    public Animator playerAnimator;
    public ParryTarget enemy;

    [Header("Hitstop (parry success)")]
    public HitStopper hitStopper;          // <- kéo GameManager(HitStopper) vào đây
    [Tooltip("Thời gian hit-stop khi parry thành công (giây). 0 = tắt.")]
    public float parrySuccessHitStop = 0.05f;
    [Tooltip("I-frame sau parry thành công (unscaled).")]
    public float successIFrame = 0.08f;

    [Header("Debug")] public bool logs = true;

    bool windowOpen;
    float successUntilUnscaled;            // i-frame dùng đồng hồ unscaled

    // cache để biết Player có đang ở state "bị hit" hay không
    PlayerHealth _hp;                      // <--- THÊM

    public bool IsWindowActive => windowOpen;
    public bool IsSuccessActive => Time.unscaledTime < successUntilUnscaled;
    public bool PoseLocked => InParryOrSuccess();

    void Awake()                           // <--- THÊM
    {
        // tìm PlayerHealth từ animator nếu có, fallback về chính GameObject
        _hp = playerAnimator ? playerAnimator.GetComponent<PlayerHealth>()
                             : GetComponent<PlayerHealth>();
    }

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

    // ĐANG BỊ HIT?
    bool IsHitLocked()                     // <--- THÊM
    {
        return _hp && _hp.animator &&
               AnimUtil.IsInState(_hp.animator, _hp.hitStatePath, out _);
    }

    // --- Animation Events ---
    public void Anim_ParryOpen()
    {
        windowOpen = true;
        successUntilUnscaled = 0f;         // reset i-frame cũ
        if (logs) Debug.Log("[PARRY] Pose opened");
    }
    public void Anim_ParryClose()
    {
        windowOpen = false;
        if (logs) Debug.Log("[PARRY] Pose closed");
    }

    // --- Enemy strike-frame ---
    void OnEnemyStrike()
    {
        if (!windowOpen) return;           // fail -> KHÔNG bật i-frame

        windowOpen = false;

        // i-frame chỉ khi thành công, đo bằng unscaled để không kéo dài bởi hitstop
        successUntilUnscaled = Time.unscaledTime + successIFrame;

        // ⏸ Hit-stop riêng cho parry success
        if (hitStopper && parrySuccessHitStop > 0f)
            hitStopper.Stop(parrySuccessHitStop);

        // hiệu ứng thành công
        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);

        if (logs) Debug.Log($"[PARRY] OK (t={Time.unscaledTime:0.000})");
    }

    public void ParrySuccess() => OnEnemyStrike();

    void Update()
    {
        if (!config) return;

        if (Input.GetKeyDown(config.parryKey))
        {
            // ⛔ Đang bị hit -> không cho vào parry
            if (IsHitLocked())
            {
                if (logs) Debug.Log("[PARRY] blocked: player is in HIT");
                return;
            }

            // ⛔ Đang ở parry/parry_success -> không đè
            if (InParryOrSuccess())
            {
                if (logs) Debug.Log("[PARRY] blocked by window/state");
                return;
            }

            // OK: vào tư thế parry, cửa sổ mở bằng Anim_ParryOpen
            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
        }
    }
}
