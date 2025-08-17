using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;
    public Animator playerAnimator;
    public ParryTarget enemy;

    [Header("Lock while hit")]
    // Đặt giống hệt path hit trong PlayerHealth (mặc định: Base Layer.player_hit)
    public string playerHitStatePath = "Base Layer.player_hit";

    [Header("Debug")] public bool logs = true;

    bool windowOpen;
    float successUntil;

    float Now => Time.time;

    public bool IsWindowActive => windowOpen;
    public bool IsSuccessActive => Now < successUntil;

    // BẬN nếu đang ở parry/parry_success HOẶC đang ở player_hit
    public bool PoseLocked => InParryOrSuccess() || IsHitPlaying();

    void OnEnable() { if (enemy) enemy.OnStrike += OnEnemyStrike; }
    void OnDisable() { if (enemy) enemy.OnStrike -= OnEnemyStrike; }

    // ===== Helpers =====
    bool InParryOrSuccess()
    {
        if (!playerAnimator || !config) return false;

        bool inPose = !string.IsNullOrEmpty(config.playerParryPoseStatePath) &&
                      AnimUtil.IsInState(playerAnimator, config.playerParryPoseStatePath, out _);

        bool inSucc = !string.IsNullOrEmpty(config.playerParrySuccessStatePath) &&
                      AnimUtil.IsInState(playerAnimator, config.playerParrySuccessStatePath, out _);

        return inPose || inSucc;
    }

    bool IsHitPlaying()
    {
        return playerAnimator &&
               !string.IsNullOrEmpty(playerHitStatePath) &&
               AnimUtil.IsInState(playerAnimator, playerHitStatePath, out _);
    }

    // ===== Animation Events (mở/đóng window) =====
    public void Anim_ParryOpen() { windowOpen = true; successUntil = 0f; if (logs) Debug.Log("[PARRY] Pose opened"); }
    public void Anim_ParryClose() { windowOpen = false; if (logs) Debug.Log("[PARRY] Pose closed"); }

    // ===== Enemy strike-frame =====
    void OnEnemyStrike()
    {
        if (!windowOpen) return;          // fail -> KHÔNG bật i-frame

        windowOpen   = false;
        successUntil = Now + 0.08f;       // i-frame ngắn chỉ khi thành công

        // KHÔNG crossfade nếu đang bị hit
        if (!IsHitPlaying() && playerAnimator && config &&
            !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
        {
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);
        }

        if (logs) Debug.Log("[PARRY] OK");
    }

    public void ParrySuccess() => OnEnemyStrike();

    void Update()
    {
        if (!config) return;

        // Bấm Space
        if (Input.GetKeyDown(config.parryKey))
        {
            // ❌ Đang hit / đang parry / đang success -> KHÔNG cho vào pose
            if (PoseLocked)
            {
                if (logs) Debug.Log("[PARRY] blocked by window/state");
                return;
            }

            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
        }
    }
}
