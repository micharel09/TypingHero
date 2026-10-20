using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only. Combat remains the authority for gates, cues and outcomes.
[DisallowMultipleComponent, DefaultExecutionOrder(310)]
public sealed class CombatHUDFeedback : MonoBehaviour
{
    [SerializeField] Image parryKey, hpFrame, staminaIcon;
    [SerializeField] TMP_Text playerHp, guardBreak;
    [SerializeField] Sprite shield, warning;
    [SerializeField] Color red, gold, success;
    PlayerHealth player;
    ParrySystem parry;
    SkeletonController enemy;
    EnemyStamina stamina;
    EnemyStunController stun;
    Color keyIdle, frameIdle, hpIdle;
    float hitRemaining, successRemaining, breakRemaining;
    int depletedFrame = -1, hitFrame = -1, successFrame = -1, breakFrame = -1;
    bool announced, broken, bound;

    void Start() => Bind();
    void OnEnable() { if (player) Bind(); }
    void Bind()
    {
        if (bound) return;
        player = FindObjectOfType<PlayerHealth>();
        parry = FindObjectOfType<ParrySystem>();
        enemy = FindObjectOfType<SkeletonController>();
        stamina = enemy ? enemy.GetComponent<EnemyStamina>() : null;
        stun = enemy ? enemy.GetComponent<EnemyStunController>() : null;
        keyIdle = parryKey.color; frameIdle = hpFrame.color; hpIdle = playerHp.color;
        if (player) player.OnDamaged += OnDamage;
        if (parry) parry.OnParrySuccess += OnSuccess;
        if (stamina) stamina.OnDepleted += OnDepleted;
        if (stun) stun.onStunStart.AddListener(OnStun);
        bound = true;
        ResetFeedback();
    }
    void OnDisable()
    {
        if (!bound) return;
        if (player) player.OnDamaged -= OnDamage;
        if (parry) parry.OnParrySuccess -= OnSuccess;
        if (stamina) stamina.OnDepleted -= OnDepleted;
        if (stun) stun.onStunStart.RemoveListener(OnStun);
        ResetFeedback();
        parryKey.color = keyIdle; hpFrame.color = frameIdle; playerHp.color = hpIdle;
        bound = false;
    }
    void ResetFeedback()
    {
        hitRemaining = successRemaining = breakRemaining = 0;
        depletedFrame = -1; announced = broken = false;
        staminaIcon.sprite = shield;
        guardBreak.enabled = false;
    }
    bool Live => player && !player.IsDead && !CombatSession.Finished;
    void OnDamage()
    {
        if (Live && !GamePauseController.InputBlocked) { hitRemaining = .18f; hitFrame = Time.frameCount; }
    }
    void OnSuccess(ParrySystem.ParryContext context)
    {
        if (Live && !GamePauseController.InputBlocked) { successRemaining = .12f; successFrame = Time.frameCount; }
    }
    void OnDepleted()
    {
        // Only an actual cost crossing >0 to0 emits this event; setup/regen never do.
        depletedFrame = Live && enemy && !enemy.IsDead && !GamePauseController.InputBlocked ? Time.frameCount : -1;
    }
    void OnStun()
    {
        // Cost notifications precede the actual stun in both hit and parry paths.
        if (depletedFrame != Time.frameCount || !Live || !enemy || enemy.IsDead ||
            !stun.IsStunned || GamePauseController.InputBlocked) return;
        depletedFrame = -1;
        broken = true;
        if (!announced) { announced = true; breakRemaining = .9f; breakFrame = Time.frameCount; }
    }
    void LateUpdate()
    {
        if (!bound) return;
        bool blocked = GamePauseController.InputBlocked;
        if (!Live)
        {
            hitRemaining = successRemaining = breakRemaining = 0;
            broken = false;
        }
        playerHp.color = player && player.Current > 0 && player.Current <= player.maxHealth * .25f ? red : hpIdle;
        if (!blocked)
        {
            float dt = Time.unscaledDeltaTime;
            // Events start now; do not charge the preceding frame's time to a new flash.
            if (hitFrame != Time.frameCount) hitRemaining = Mathf.Max(0, hitRemaining - dt);
            if (successFrame != Time.frameCount) successRemaining = Mathf.Max(0, successRemaining - dt);
            if (breakFrame != Time.frameCount) breakRemaining = Mathf.Max(0, breakRemaining - dt);
        }
        hpFrame.color = Color.Lerp(frameIdle, red, hitRemaining / .18f);
        bool ready = Live && parry && parry.isActiveAndEnabled && parry.CanBeginParry;
        bool window = Live && parry && parry.isActiveAndEnabled && parry.IsWindowActive;
        parryKey.color = blocked || !Live ? keyIdle * new Color(.4f, .4f, .4f, 1) :
            successRemaining > 0 ? success : window || (ready && enemy && enemy.CueActive) ? gold :
            ready ? keyIdle : keyIdle * new Color(.4f, .4f, .4f, 1);
        if (!stun || !stun.IsStunned) broken = false;
        staminaIcon.sprite = broken ? warning : shield;
        guardBreak.enabled = Live && breakRemaining > 0;
        var color = gold;
        color.a = Mathf.Clamp01(breakRemaining / .18f);
        guardBreak.color = color;
    }
}
