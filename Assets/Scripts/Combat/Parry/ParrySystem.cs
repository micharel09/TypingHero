using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class ParrySystem : MonoBehaviour
{
    [Header("Config & Refs")]
    public ParryConfig config;
    public Animator playerAnimator;
    [SerializeField] ParryContactLibrary contactLibrary;
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
        internal ParryContactSnapshot contact;
    }
    public event System.Action<ParryContext> OnParrySuccess;
    // ===============================================================

    bool windowOpen;
    int poseRequestedFrame = -1;
    float successUntilUnscaled;
    float _lastOpenAt, _lastCloseAt;
    SpriteRenderer contactPlayer, contactEnemy;
    Sprite heldPlayer, heldEnemy, resumePlayer, resumeEnemy;
    bool showingContact;
    float contactPoseAge;
    SlashVolume enemyTrail;
    bool enemyTrailEnabled;
    float enemyTrailHold;
    int successRequestedFrame = -1;
    double followUpUntil = double.NegativeInfinity;

    internal bool InSuccessPose => successRequestedFrame == Time.frameCount ||
        (config && InAnimation(config.playerParrySuccessStatePath));
    internal bool CanQueueFollowUp => InSuccessPose && !SuccessPoseCompleted &&
        Time.unscaledTimeAsDouble < followUpUntil;
    internal bool SuccessPoseCompleted { get; private set; }

    public bool IsWindowActive => windowOpen;
    public bool IsSuccessActive => Time.unscaledTime < successUntilUnscaled;
    public bool PoseLocked => poseRequestedFrame == Time.frameCount ||
        successRequestedFrame == Time.frameCount || InParryOrSuccess();

    void OnEnable()
    {
        if (enemy) enemy.OnStrike += OnEnemyStrike;   // ParryTarget phải phát OnStrike(IParryTarget)
    }

    void OnDisable()
    {
        ClearContactPose();
        RestoreEnemyTrail();
        if (enemy) enemy.OnStrike -= OnEnemyStrike;
        if (gate) gate.PopParryPose(this);
        poseRequestedFrame = -1;
        successRequestedFrame = -1;
        followUpUntil = double.NegativeInfinity;
        SuccessPoseCompleted = false;
        windowOpen = false;
    }

    bool InAnimation(string path)
    {
        if (!playerAnimator || string.IsNullOrEmpty(path)) return false;
        int hash = AnimatorUtil.Hash(path);
        return playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == hash ||
               (playerAnimator.IsInTransition(0) && playerAnimator.GetNextAnimatorStateInfo(0).fullPathHash == hash);
    }

    internal void ResetPracticePose()
    {
        ClearContactPose();
        RestoreEnemyTrail();
        if (gate) gate.PopParryPose(this);
        poseRequestedFrame = successRequestedFrame = -1;
        successUntilUnscaled = 0;
        _lastOpenAt = _lastCloseAt = float.NegativeInfinity;
        followUpUntil = double.NegativeInfinity;
        SuccessPoseCompleted = windowOpen = false;
    }

    bool InParryOrSuccess() => config &&
        (InAnimation(config.playerParryPoseStatePath) || InAnimation(config.playerParrySuccessStatePath));

    // ---------------- Animation Events ----------------
    public void Anim_ParryOpen()
    {
        if (CombatSession.Finished || (gate && (gate.IsDeadLocked || gate.IsHitLocked))) return;    // chặn nếu đã chết
        windowOpen = true;
        successUntilUnscaled = 0f;
        _lastOpenAt = Time.unscaledTime;
        if (logs) Debug.Log("[PARRY] Pose opened");
    }

    public void Anim_ParryClose()
    {
        windowOpen = false;
        _lastCloseAt = Time.unscaledTime;
        if (logs) Debug.Log("[PARRY] Pose closed");
    }

    public void Anim_ParryEnd(int success)
    {
        // Mecanim can already report idle while dispatching the outgoing clip's end event.
        if (success == 1 && !CombatSession.Finished && (!gate || (!gate.IsHitLocked && !gate.IsDeadLocked)))
            SuccessPoseCompleted = true;
    }
    // --------------------------------------------------

    // gom logic chốt parry-success vào 1 chỗ
    void ForceSuccess(Component enemyComp = null)
    {
        Transform targetRoot = enemyComp ? enemyComp.transform.root : null;
        var skeleton = enemyComp ? enemyComp.GetComponentInParent<SkeletonController>() : null;
        var playerSprite = playerAnimator ? playerAnimator.GetComponent<SpriteRenderer>() : null;
        var enemySprite = skeleton ? skeleton.GetComponent<SpriteRenderer>() : (targetRoot ? targetRoot.GetComponentInChildren<SpriteRenderer>() : null);
        var visual = contactLibrary ? contactLibrary.Capture(
            playerSprite, enemySprite, Camera.main) : default;
        ClearContactPose();
        RestoreEnemyTrail();
        if (visual.valid)
        {
            contactPlayer = playerSprite; contactEnemy = enemySprite;
            heldPlayer = visual.defenseCel; heldEnemy = visual.attackCel;
            resumePlayer = playerSprite ? playerSprite.sprite : null; resumeEnemy = enemySprite ? enemySprite.sprite : null;
            showingContact = true; contactPoseAge = 0;
            enemyTrail = enemySprite ? enemySprite.GetComponent<SlashVolume>() : null;
            if (enemyTrail)
            {
                enemyTrailEnabled = enemyTrail.enabled; enemyTrail.enabled = false;
                enemyTrailHold = .2f;
            }
        }
        // i-frame cho player như cũ
        successUntilUnscaled = Time.unscaledTime + successIFrame;
        windowOpen = false;
        _lastOpenAt = _lastCloseAt = float.NegativeInfinity;
        successRequestedFrame = playerAnimator && config && !string.IsNullOrEmpty(config.playerParrySuccessStatePath)
            ? Time.frameCount : -1;
        followUpUntil = Time.unscaledTimeAsDouble + .8;
        SuccessPoseCompleted = false;
        if (gate && successRequestedFrame >= 0) gate.PushParryPose(this);

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

        if (skeleton) skeleton.RecordParry();
        OnParrySuccess?.Invoke(new ParryContext { targetRoot = targetRoot, contact = visual });

        if (logs) Debug.Log($"[PARRY] OK (t={Time.unscaledTime:0.000})");
    }

    // NỚI điều kiện trong OnEnemyStrike bằng grace-time
    void OnEnemyStrike(IParryTarget enemyTarget)
    {
        if (GamePauseController.InputBlocked || CombatSession.Finished ||
            IsSuccessActive || (gate && (gate.IsDeadLocked || gate.IsHitLocked))) return;

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

    internal bool CanBeginParry => (!TutorialRun.Active || TutorialRun.Practicing == TutorialRun.Practice.Parry) &&
        !GamePauseController.InputBlocked && !CombatSession.Finished && config && playerAnimator &&
        !string.IsNullOrEmpty(config.playerParryPoseStatePath) && (!gate || gate.CanParry) && !PoseLocked;
    // Reuse the authored impact cels briefly. Animator states, events and hitboxes keep running.
    // Events arrive before SpriteRenderer curves are applied, so sampling only renderer.sprite
    // would capture the previous wind-up cel and Hurt could replace contact before it is drawn.
    void LateUpdate()
    {
        if (enemyTrail)
        {
            if (!GamePauseController.InputBlocked && Time.timeScale != 0) enemyTrailHold -= Time.unscaledDeltaTime;
            if (CombatSession.Finished || enemyTrailHold <= 0) RestoreEnemyTrail();
        }
        if (!showingContact) return;
        if (CombatSession.Finished) { ClearContactPose(); return; }
        if (contactPlayer && contactPlayer.sprite != heldPlayer) resumePlayer = contactPlayer.sprite;
        if (contactEnemy && contactEnemy.sprite != heldEnemy) resumeEnemy = contactEnemy.sprite;
        if (contactPoseAge >= .07f) { ClearContactPose(); return; }
        if (contactPlayer && heldPlayer) contactPlayer.sprite = heldPlayer;
        if (contactEnemy && heldEnemy) contactEnemy.sprite = heldEnemy;
        if (!GamePauseController.InputBlocked && Time.timeScale != 0) contactPoseAge += Time.unscaledDeltaTime;
    }
    void ClearContactPose()
    {
        if (!showingContact) return;
        if (contactPlayer && contactPlayer.sprite == heldPlayer) contactPlayer.sprite = resumePlayer;
        if (contactEnemy && contactEnemy.sprite == heldEnemy) contactEnemy.sprite = resumeEnemy;
        showingContact = false;
        contactPlayer = contactEnemy = null;
        heldPlayer = heldEnemy = resumePlayer = resumeEnemy = null;
    }
    void RestoreEnemyTrail()
    {
        if (enemyTrail) enemyTrail.enabled = enemyTrailEnabled;
        enemyTrail = null; enemyTrailHold = 0;
    }

    internal bool TryBeginParry()
    {
        if (!CanBeginParry) return false;
        poseRequestedFrame = Time.frameCount;
        if (gate) gate.PushParryPose(this);
        AnimatorUtil.CrossFadePath(playerAnimator, config.playerParryPoseStatePath, config.playerParryPoseCrossfade, 0f);
        return true;
    }

    void Update()
    {
        if (CombatSession.Finished || (gate && (gate.IsHitLocked || gate.IsDeadLocked)))
        {
            successRequestedFrame = -1;
            followUpUntil = double.NegativeInfinity;
            SuccessPoseCompleted = false;
        }
        // Interrupted poses may never reach the clip's close event.
        if (windowOpen && (CombatSession.Finished || (gate && (gate.IsHitLocked || gate.IsDeadLocked)) ||
            (playerAnimator && config && !InAnimation(config.playerParryPoseStatePath))))
            Anim_ParryClose();
        if (gate)
        {
            if (PoseLocked && !CombatSession.Finished) gate.PushParryPose(this);
            else gate.PopParryPose(this);
        }
        if (GamePauseController.InputBlocked)
        {
            followUpUntil += Time.unscaledDeltaTime;
            successUntilUnscaled += Time.unscaledDeltaTime;
            _lastOpenAt += Time.unscaledDeltaTime; _lastCloseAt += Time.unscaledDeltaTime;
            return;
        }
        if (CombatSession.Finished) return;
        if (!config) return;

        // gate từ PlayerInputGate: không cho mở parry nếu đang bị khoá (chết/khác)
        if (gate && !gate.CanParry) return;

        if (Input.GetKeyDown(config.parryKey)) TryBeginParry();
    }
}
