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

    [Header("Input Gate")]
    public PlayerInputGate gate;
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


    bool IsHitLocked()                   
    {
        return _hp && _hp.animator &&
               AnimUtil.IsInState(_hp.animator, _hp.hitStatePath, out _);
    }

    // --- Animation Events ---
    public void Anim_ParryOpen()
    {
        windowOpen = true;
        successUntilUnscaled = 0f;         // reset i-frame cũ
        gate?.PushParryPose(this);
        if (logs) Debug.Log("[PARRY] Pose opened");
    }
    public void Anim_ParryClose()
    {
        windowOpen = false;
        gate?.PopParryPose(this);
        if (logs) Debug.Log("[PARRY] Pose closed");
    }

    // --- Enemy strike-frame ---
    void OnEnemyStrike()
    {
        if (!windowOpen) return;

        windowOpen = false;
        successUntilUnscaled = Time.unscaledTime + successIFrame;
        gate?.BeginParrySuccessIFrame(successIFrame);  // <-- thêm

        if (hitStopper && parrySuccessHitStop > 0f) hitStopper.Stop(parrySuccessHitStop);

        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);

        if (logs) Debug.Log($"[PARRY] OK");
    }

    public void ParrySuccess() => OnEnemyStrike();

    void Update()
    {
        if (!config) return;

        // đang bị hit hoặc đang ở parry pose → không cho vào nữa
        if (gate && !gate.CanParry) return;        

        if (Input.GetKeyDown(config.parryKey))
        {
            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
        }
    }
}
