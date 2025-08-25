using System.Linq;
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

    [Header("Success")]
    [SerializeField] float successIFrame = 0.12f;

    [Header("Chặn damage của đòn đang vung (sau khi parry)")]
    [Tooltip("Khoá mọi AttackStrike trên enemy vừa bị parry trong TTL này (unscaled).")]
    [SerializeField] float cancelEnemyStrikeTTL = 0.12f;

    [Header("Input Gate")]
    public PlayerInputGate gate;

    [Header("Parry Tolerance (for WebGL/frame jitter)")]
    [SerializeField] float strikeEarlyGrace = 0.02f; // strike đến hơi SỚM
    [SerializeField] float strikeLateGrace = 0.02f;  // strike đến hơi MUỘN

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
    float _lastOpenAt, _lastCloseAt;

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
                      AnimatorUtil.IsInState(playerAnimator, config.playerParryPoseStatePath, out _);

        bool inSucc = !string.IsNullOrEmpty(config.playerParrySuccessStatePath) &&
                      AnimatorUtil.IsInState(playerAnimator, config.playerParrySuccessStatePath, out _);

        return inPose || inSucc;
    }

    // ---------------- Animation Events ----------------
    public void Anim_ParryOpen()
    {
        if (gate && gate.IsDeadLocked) return;    // chặn nếu đã chết
        windowOpen = true;
        successUntilUnscaled = 0f;
        _lastOpenAt = Time.unscaledTime;
        if (logs) Debug.Log("[PARRY] Pose opened");
    }

    public void Anim_ParryClose()
    {
        if (gate && gate.IsDeadLocked) return;    // chặn nếu đã chết
        windowOpen = false;
        _lastCloseAt = Time.unscaledTime;
        if (logs) Debug.Log("[PARRY] Pose closed");
    }
    // --------------------------------------------------

    // gom logic chốt parry-success vào 1 chỗ
    void ForceSuccess(Component enemyComp = null)
    {
        // i-frame cho player như cũ
        successUntilUnscaled = Time.unscaledTime + successIFrame;
        windowOpen = false;

        // === NEW: khoá damage phía enemy đang vung ===
        if (enemyComp)
        {
            var root = enemyComp.transform.root;
            var strikes = root.GetComponentsInChildren<AttackStrike>(includeInactive: true);
            for (int i = 0; i < strikes.Length; i++)
                strikes[i].ParryLock(cancelEnemyStrikeTTL);
        }

        if (hitStopper && parrySuccessHitStop > 0f)
            hitStopper.Stop(parrySuccessHitStop);

        if (playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath))
            AnimatorUtil.CrossFadePath(playerAnimator, config.playerParrySuccessStatePath,
                                       config.playerParrySuccessCrossfade, 0f);

        Transform targetRoot = enemyComp ? enemyComp.transform.root : null;
        OnParrySuccess?.Invoke(new ParryContext { targetRoot = targetRoot });

        if (logs) Debug.Log($"[PARRY] OK (t={Time.unscaledTime:0.000})");
    }

    // NỚI điều kiện trong OnEnemyStrike bằng grace-time
    void OnEnemyStrike(IParryTarget enemyTarget)
    {
        if (gate && gate.IsDeadLocked) return;

        float now = Time.unscaledTime;
        bool withinEarly = (now - _lastOpenAt) <= strikeEarlyGrace;  // strike vừa sớm
        bool withinLate = (now - _lastCloseAt) <= strikeLateGrace;   // strike vừa muộn

        if (!(windowOpen || withinEarly || withinLate))
            return;

        // chốt parry thành công
        var comp = (enemyTarget as Component);
        ForceSuccess(comp);
    }

    // biến ParrySuccess từ "obsolete" thành fallback thật sự
    public void ParrySuccess() => ForceSuccess(null);

    void Update()
    {
        if (!config) return;

        // gate từ PlayerInputGate: không cho mở parry nếu đang bị khoá (chết/khác)
        if (gate && !gate.CanParry) return;

        if (Input.GetKeyDown(config.parryKey))
        {
            if (InParryOrSuccess()) return;

            if (playerAnimator && !string.IsNullOrEmpty(config.playerParryPoseStatePath))
                AnimatorUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath,
                                           config.playerParryPoseCrossfade, 0f);
        }
    }
}