using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;
    public Animator playerAnimator;
    public ParryTarget enemy;

    [Header("Debug")] public bool logs = true;

    bool windowOpen;
    float successUntil;
    const float successIFrame = 0.08f;   // i-frame rất ngắn khi parry thành công

    float Now => Time.time;

    public bool IsWindowActive => windowOpen;
    public bool IsSuccessActive => Now < successUntil;
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
        windowOpen = true;
        successUntil = 0f;
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
        if (!windowOpen) return;

        windowOpen   = false;
        successUntil = Now + successIFrame;


        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);

        if (logs) Debug.Log($"[PARRY] OK (now={Now:0.000} until={successUntil:0.000})");
    }

    public void ParrySuccess() => OnEnemyStrike();

    void Update()
    {
        if (!config) return;

        if (Input.GetKeyDown(config.parryKey))
        {
            if (InParryOrSuccess()) return; // không cho crossfade chồng
            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
        }
    }
}
