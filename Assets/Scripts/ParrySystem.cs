using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;          // key, paths
    public Animator playerAnimator;     // Player animator
    public SkeletonController boss;     // stun khi parry

    [Header("Debug")] public bool logs = true;

    // --- state ---
    bool windowOpen;        // cửa sổ parry (mở/đóng bằng Animation Event)
    bool isParrying;        // đang trong chuỗi parry -> chặn Space
    float Now => (config && config.useUnscaledTime) ? Time.unscaledTime : Time.time;

    public bool IsWindowActive => windowOpen;

    void OnEnable() { if (boss) boss.OnStrike += OnBossStrike; }
    void OnDisable() { if (boss) boss.OnStrike -= OnBossStrike; }

    // ===== Animation Events trên clip player_parry / player_parry_success =====
    public void Anim_ParryOpen()
    {
        if (windowOpen) return;
        windowOpen = true;
        if (logs) Debug.Log("[PARRY] Pose opened");
    }

    public void Anim_ParryClose()
    {
        windowOpen = false;
        isParrying = false;             // mở khóa Space tại đúng frame event
        if (logs) Debug.Log("[PARRY] Pose closed");
    }

    // ===== Boss báo frame chém =====
    void OnBossStrike()
    {
        if (!windowOpen) return;

        // chốt kết quả, đóng ngay cửa sổ để tránh auto-parry nhiều lần
        windowOpen = false;

        if (boss) boss.Parried(config ? config.stunEnemySeconds : 0.35f);

        // phát animation parry thành công (clip này cũng phải có Anim_ParryClose ở cuối)
        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                   config.playerParrySuccessCrossfade, 0f);

        if (logs) Debug.Log("[PARRY] OK");
    }

    // Cho PlayerHealth gọi khi chặn damage bằng cửa sổ parry
    public void ParrySuccess() => OnBossStrike();

    void Update()
    {
        if (!config) return;

        if (Input.GetKeyDown(config.parryKey))
        {
            if (isParrying) return;        // đang trong chuỗi parry -> bỏ qua
            isParrying = true;             // khóa Space ngay khi bấm

            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                       config.playerParryPoseCrossfade, 0f);
            // KHÔNG mở window ở đây; windowOpen do Animation Event mở.
        }
    }
}
