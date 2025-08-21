using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySlayerHitPose : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Animator animator;
    [SerializeField] EnemyStunController stunController;   // optional
    [SerializeField] EnemyHitReactGate hitReactGate;      // optional

    [Header("Animator States (full path)")]
    [SerializeField] string hitStatePath = "Base Layer.skeleton_hitBySlayer";
    [SerializeField] string stunStatePath = "Base Layer.skeleton_stun";

    [Header("Hit Clip (để lấy số bước)")]
    [SerializeField] AnimationClip hitClip;
    [Tooltip("Số “bước” trong clip hit (bằng số sprite keyframes). Ví dụ clip 2 ảnh -> 2.")]
    [SerializeField] int clipSteps = 2;

    [Header("Hold / Return")]
    [SerializeField] float backToStunAfter = 2f;

    // --------- JITTER (rung sprite khi trúng đòn) ----------
    [Header("Jitter (per hit)")]
    [SerializeField] Transform jitterRoot;          // để trống -> tự lấy transform chứa SpriteRenderer
    [SerializeField] float jitterDuration = 0.12f;  // thời gian rung 1 hit
    [SerializeField] float jitterAmplitude = 0.04f; // biên độ tối đa (unit thế giới)
    [SerializeField, Range(0f, 1f)] float jitterDamping = 0.85f; // giảm biên độ theo thời gian
    [SerializeField] Vector2 jitterFreqRange = new Vector2(80f, 120f); // Hz ngẫu nhiên

    // runtime
    readonly List<float> _frameTimes = new();
    int _lastIdx = -1;
    float _lastHitAt = -999f;
    Coroutine _holdCo;
    Coroutine _jitterCo;
    Vector3 _jitterBaseLocalPos;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!jitterRoot) jitterRoot = GetFirstSpriteRoot();
        if (jitterRoot) _jitterBaseLocalPos = jitterRoot.localPosition;

        BuildFrameTimes();
        SlayerModeSignals.OnSetActive += OnSlayerToggle;
    }
    void OnDestroy() => SlayerModeSignals.OnSetActive -= OnSlayerToggle;

    void OnDisable()
    {
        animator.speed = 1f;
        UnlockDrivers();
        StopAllCoroutines();
        _holdCo = _jitterCo = null;
        RestoreJitterBase();
        _lastIdx = -1;
    }

    /// Gọi mỗi khi bị player chém trong SLAYER
    public void OnSlayerHit()
    {
        _lastHitAt = Time.time;

        // Toggle frame: luôn khác frame trước đó
        int next = (_lastIdx + 1 + Mathf.Max(1, _frameTimes.Count)) % Mathf.Max(1, _frameTimes.Count);
        float nt = (_frameTimes.Count > 0) ? _frameTimes[next] : 0.5f;
        _lastIdx = next;

        LockDrivers();              // chặn stun/hit-react khác
        animator.speed = 1f;
        animator.Play(hitStatePath, 0, nt);
        animator.Update(0f);        // apply ngay frame này
        animator.speed = 0f;        // freeze đúng frame

        // Jitter burst cho hit này
        StartJitter();

        // Giữ đến khi không bị đánh thêm trong ~backToStunAfter
        if (_holdCo != null) StopCoroutine(_holdCo);
        _holdCo = StartCoroutine(HoldThenBackToStun());
    }

    // ======= internals =======
    void BuildFrameTimes()
    {
        _frameTimes.Clear();
        int steps = Mathf.Max(1, clipSteps > 0 ? clipSteps : EstimateStepsFromClip(hitClip));
        for (int i = 0; i < steps; i++) _frameTimes.Add((i + 0.5f) / steps);
    }

    int EstimateStepsFromClip(AnimationClip clip)
    {
        if (!clip) return 1;
        int totalFrames = Mathf.Max(1, Mathf.RoundToInt(clip.length * clip.frameRate));
        return Mathf.Max(1, totalFrames);
    }

    IEnumerator HoldThenBackToStun()
    {
        while (Time.time - _lastHitAt < backToStunAfter) yield return null;

        animator.speed = 1f;
        animator.Play(stunStatePath, 0, 0f);
        animator.Update(0f);
        UnlockDrivers();
        _holdCo = null;
    }

    void OnSlayerToggle(bool active)
    {
        if (!active)
        {
            animator.speed = 1f;
            animator.Play(stunStatePath, 0, 0f);
            animator.Update(0f);
            UnlockDrivers();
            if (_holdCo   != null) { StopCoroutine(_holdCo); _holdCo = null; }
            if (_jitterCo != null) { StopCoroutine(_jitterCo); _jitterCo = null; }
            RestoreJitterBase();
        }
    }

    void LockDrivers()
    {
        if (stunController) stunController.enabled = false;
        if (hitReactGate) hitReactGate.enabled   = false;
    }
    void UnlockDrivers()
    {
        if (stunController) stunController.enabled = true;
        if (hitReactGate) hitReactGate.enabled   = true;
    }

    // ---------- JITTER ----------
    Transform GetFirstSpriteRoot()
    {
        var sr = GetComponentInChildren<SpriteRenderer>(true);
        return sr ? sr.transform : transform;
    }

    void StartJitter()
    {
        if (!jitterRoot) return;
        if (_jitterCo != null) StopCoroutine(_jitterCo);
        _jitterCo = StartCoroutine(JitterCo());
    }

    IEnumerator JitterCo()
    {
        RestoreJitterBase();
        float tEnd = Time.unscaledTime + jitterDuration;
        float amp = Mathf.Max(0f, jitterAmplitude);
        float freq = Random.Range(Mathf.Max(1f, jitterFreqRange.x), Mathf.Max(jitterFreqRange.x + 1f, jitterFreqRange.y));

        // dùng unscaled để không bị ảnh hưởng hitstop
        while (Time.unscaledTime < tEnd && amp > 0f)
        {
            // offset ngẫu nhiên hình tròn (2D)
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector3 off = new Vector3(dir.x, dir.y, 0f) * amp;
            jitterRoot.localPosition = _jitterBaseLocalPos + off;

            // giảm biên độ theo thời gian
            amp *= Mathf.Pow(jitterDamping, Time.unscaledDeltaTime * freq);
            yield return null;
        }

        RestoreJitterBase();
        _jitterCo = null;
    }

    void RestoreJitterBase()
    {
        if (jitterRoot) jitterRoot.localPosition = _jitterBaseLocalPos;
    }
}
