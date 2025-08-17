using UnityEngine;

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
    [Tooltip("Miễn thương ngắn sau khi bị đánh (giây)")]
    public float postHitIFrames = 0.05f;

    [Tooltip("Kéo thả ParrySystem ở GameManager vào đây")]
    public ParrySystem parry;

    [Header("Debug")] public bool logs = false;

    float iFramesUntil;
    public bool IsDead => Current <= 0;

    void Awake()
    {
        if (Current <= 0) Current = maxHealth; // tiện test
    }

    // ===== IDamageable =====

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (logs && parry)
            Debug.Log($"[PARRY dbg] win={parry.IsWindowActive} succ={parry.IsSuccessActive} t={Time.time:0.000}");

        if (IsDead) return;
        // (1) Parry cửa sổ HOẶC i-frame thành công
        if (parry && (parry.IsWindowActive || parry.IsSuccessActive))
        {
            if (logs) Debug.Log("[PARRY] blocked by window/state");
            parry.ParrySuccess(); // nếu window đã đóng, OnEnemyStrike() sẽ tự bỏ qua
            return;
        }


        // (2) i-frames sau khi vừa bị đánh
        if (Time.time < iFramesUntil)
        {
            if (logs) Debug.Log("[DMG] ignored (post-hit i-frames)");
            return;
        }

        // (3) Ăn damage bình thường
        Current = Mathf.Max(0, Current - amount);
        if (logs) Debug.Log($"[DMG] player took {amount}, HP: {Current}");
        iFramesUntil = Time.time + postHitIFrames;

        if (Current <= 0)
        {
            if (animator && !string.IsNullOrEmpty(dieStatePath))
                AnimUtil.CrossFadePath(animator, dieStatePath, hitCrossfade, 0f);
            return;
        }

        if (animator && !string.IsNullOrEmpty(hitStatePath))
            AnimUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
    }

}
