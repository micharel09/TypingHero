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
    public float postHitIFrames = 0.05f;

    [Header("Refs")]
    public ParrySystem parry;
    public PlayerInputGate gate;

    [Header("Debug")] public bool logs = false;

    float iFramesUntil;
    public bool IsDead => Current <= 0;

    void Awake()
    {
        if (Current <= 0) Current = maxHealth;
    }

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (IsDead) return;

        // Parry chặn đòn?
        if (parry && (parry.IsWindowActive || parry.IsSuccessActive))
        {
            if (logs) Debug.Log("[PARRY] SUCCESS via window/i-frame => no damage");
            parry.ParrySuccess();
            return;
        }

        // i-frames sau khi bị đánh
        if (Time.time < iFramesUntil)
        {
            if (logs) Debug.Log("[DMG] ignored (post-hit i-frames)");
            return;
        }

        // Nhận damage
        Current = Mathf.Max(0, Current - amount);
        if (logs) Debug.Log($"[DMG] player took {amount}, HP: {Current}");
        iFramesUntil = Time.time + postHitIFrames;

        if (Current <= 0)
        {
            // Khóa toàn phần input khi chết
            if (gate) gate.SetDeadLocked(true);

            // Phát clip die, KHÔNG destroy ở đây nữa
            if (animator && !string.IsNullOrEmpty(dieStatePath))
                AnimatorUtil.CrossFadePath(animator, dieStatePath, hitCrossfade, 0f);
            return;
        }

        if (animator && !string.IsNullOrEmpty(hitStatePath))
            AnimatorUtil.CrossFadePath(animator, hitStatePath, hitCrossfade, 0f);
    }

    // Animation Event ở frame CUỐI của clip player_die sẽ gọi hàm này
    public void Anim_DieCleanup()
    {
        Destroy(gameObject);
    }
}
