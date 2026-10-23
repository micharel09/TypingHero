using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

// Combat adapter. The timeline owns all transient camera time and displacement.
[DefaultExecutionOrder(-10), DisallowMultipleComponent]
public sealed class CombatCameraImpact : MonoBehaviour
{
    internal enum Kind { None, Normal, Parry, Received, SlayerEnter, Lethal }
    [SerializeField] DungeonArenaCamera arena;
    [SerializeField] PlayerHealth health;
    CombatCameraProfile.Preset[] prepared;
    CombatCameraProfile.Preset pending;
    readonly CameraImpactTimeline timeline = new CameraImpactTimeline();
    Camera view;
    Collider2D body;
    double clock, gameplayClock, lastTime, pendingStart, pendingGameplayStart;
    bool clockReady, clockFrozen;
    Vector3 pendingPoint, pendingDirection, activePoint;
    float pendingPower;
    int renderedSerial, renderedMask, renderFrame, lastShakeSerial;
    internal float Zoom => timeline.Zoom;
    bool UsesPixels => timeline.Active != null && timeline.Active.units == CombatCameraProfile.ShakeUnits.WorldPixels && timeline.Active.strength > 0;
    internal Vector3 Offset => UsesPixels ? timeline.Offset * arena.PixelSizeAtDepth(Mathf.Max(.01f, view.WorldToViewportPoint(activePoint).z)) : timeline.Offset;
    internal Vector3 OffsetForState(CameraState state)
    {
        if (!UsesPixels) return timeline.Offset;
        // The brain has not written its new lens to Camera yet at stage Noise.
        // Use this frame's lens/position and the final arena orientation.
        var forward = Quaternion.Euler(arena.profile.pitch, 0, 0) * Vector3.forward;
        float depth = Mathf.Max(.01f, Vector3.Dot(forward, activePoint - state.FinalPosition));
        return timeline.Offset * arena.PixelSizeAtDepth(depth, state.Lens.OrthographicSize);
    }
    internal Kind ActiveKind => timeline.Active == null ? Kind.None : ToKind(timeline.Active.trigger);
    internal Kind LastKind { get; private set; }
    internal int Serial { get; private set; }
    internal int EffectSerial => timeline.Serial;
    internal int PeakRenderCount => timeline.PeakRenderCount;
    internal int ShakePeakRenderCount => timeline.ShakePeakRenderCount;
    internal int PeakRenderFrame { get; private set; }
    internal int NormalCount { get; private set; }
    internal int ParryCount { get; private set; }
    internal int ReceivedCount { get; private set; }
    internal int LethalCount { get; private set; }
    // Retained read-only probe API: now counts directly applied shake starts.
    internal int ImpulseCount { get; private set; }
    static Kind ToKind(CombatCameraProfile.Event trigger) => trigger == CombatCameraProfile.Event.SlayerLethal ? Kind.Lethal : (Kind)((int)trigger + 1);
    public CombatCameraProfile profile { get; private set; }
    // Explicit reconfiguration prepares future requests; accepted requests retain their snapshot.
    public void Configure(CombatCameraProfile value) { profile = value; prepared = value ? value.Prepare() : null; }
    internal void Bind(CombatCameraProfile profile)
    {
        Clear(); Configure(profile);
        view = arena ? arena.GetComponent<Camera>() : null;
        clockReady = false; clockFrozen = GamePauseController.InputBlocked;
    }
    void OnEnable()
    {
        view = arena ? arena.GetComponent<Camera>() : null;
        if (health) { body = health.GetComponent<Collider2D>(); health.DamageAccepted += Received; }
        clockFrozen = GamePauseController.InputBlocked;
        GamePauseController.FreezeChanged += FreezeChanged;
        RenderPipelineManager.beginCameraRendering += BeginRender;
        RenderPipelineManager.endCameraRendering += EndRender;
    }
    void OnDisable()
    {
        if (health) health.DamageAccepted -= Received;
        GamePauseController.FreezeChanged -= FreezeChanged;
        RenderPipelineManager.beginCameraRendering -= BeginRender;
        RenderPipelineManager.endCameraRendering -= EndRender;
        Clear();
    }
    void BeginRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera != view) return;
        renderedSerial = timeline.Serial; renderedMask = timeline.RenderMask; renderFrame = Time.frameCount;
    }
    void EndRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera != view || renderFrame != Time.frameCount) return;
        int count = timeline.PeakRenderCount, shakeCount = timeline.ShakePeakRenderCount;
        UpdateClock();
        timeline.Acknowledge(renderedSerial, renderedMask, GamePauseController.InputBlocked, clock);
        if (count != timeline.PeakRenderCount || shakeCount != timeline.ShakePeakRenderCount) PeakRenderFrame = Time.frameCount;
    }
    void FreezeChanged(bool frozen) { UpdateClock(); clockFrozen = frozen; }
    void UpdateClock()
    {
        double now = Time.realtimeSinceStartupAsDouble;
        if (clockReady && !clockFrozen)
        {
            double delta = System.Math.Max(0, now - lastTime);
            clock += delta;
        }
        lastTime = now; gameplayClock = Time.timeAsDouble; clockReady = true;
    }
    internal void Hit(Vector3 point, Vector3 direction, bool wasSlayer, bool lethal)
    {
        if (!lethal && (wasSlayer || CombatSession.Finished)) return;
        Queue(lethal ? wasSlayer ? CombatCameraProfile.Event.SlayerLethal : CombatCameraProfile.Event.Lethal : CombatCameraProfile.Event.Normal, point, direction);
    }
    internal void Parry(Vector3 point, Vector3 direction, float power)
    {
        if (!CombatSession.Finished) Queue(CombatCameraProfile.Event.Parry, point, direction, Mathf.Clamp01(power));
    }
    void Received(Vector2 point, bool lethal)
    {
        Vector3 direction = body ? body.bounds.center - (Vector3)point : health ? health.transform.position - (Vector3)point : Vector3.left;
        Queue(CombatCameraProfile.Event.Received, point, direction);
    }
    internal void EnterSlayer(CinemachineImpulseSource source, float delay)
    {
        if (!CombatSession.Finished) Queue(CombatCameraProfile.Event.SlayerEnter, source ? source.transform.position : Vector3.zero, Vector3.zero);
    }
    internal void ExitSlayer()
    {
        if (pending != null && pending.trigger == CombatCameraProfile.Event.SlayerEnter) pending = null;
        if (ActiveKind == Kind.SlayerEnter) timeline.Clear();
    }
    void Queue(CombatCameraProfile.Event trigger, Vector3 point, Vector3 direction, float power = 1)
    {
        if (!isActiveAndEnabled || GamePauseController.InputBlocked || DungeonSession.InputSuppressed || prepared == null) return;
        var preset = prepared[(int)trigger];
        if (!preset.enabled || pending != null && preset.priority <= pending.priority || timeline.Active != null && preset.priority <= timeline.Active.priority) return;
        UpdateClock(); pendingStart = clock; pendingGameplayStart = gameplayClock; pending = preset;
        pendingPoint = point; pendingDirection = direction; pendingPower = power;
    }
    void LateUpdate()
    {
        UpdateClock();
        if (DungeonSession.InputSuppressed || GamePauseController.EncounterReading || GamePauseController.ResultOverlayVisible || TutorialRun.Active ||
            (GamePauseController.InputBlocked && !GamePauseController.IsPaused)) { Clear(); return; }
        if (GamePauseController.InputBlocked) return;
        Sample();
    }
    void Sample()
    {
        if (pending != null)
        {
            var p = pending; pending = null;
            Vector3 direction = p.contactDirection ? pendingDirection : p.direction;
            if (view)
            {
                var local = p.contactDirection ? view.transform.InverseTransformDirection(direction) : direction; local.z = 0;
                if (local.sqrMagnitude < .0001f) local = Vector3.left;
                direction = view.transform.TransformDirection(local.normalized);
            }
            else direction = direction.sqrMagnitude < .0001f ? Vector3.left : direction.normalized;
            if (p.units == CombatCameraProfile.ShakeUnits.WorldPixels && p.strength > 0)
            {
                if (!arena || !arena.WorldTexture) throw new System.InvalidOperationException("Camera impact requires bound world projection/texture");
            }
            if (timeline.Play(p, pendingStart, direction * (p.strength * pendingPower), pendingGameplayStart))
            {
                activePoint = pendingPoint; LastKind = ToKind(p.trigger); Serial++;
                switch (LastKind) { case Kind.Normal: NormalCount++; break; case Kind.Parry: ParryCount++; break;
                    case Kind.Received: ReceivedCount++; break; case Kind.Lethal: LethalCount++; break; }
            }
        }
        timeline.Sample(clock, gameplayClock, HitStopper.IsActive, GamePauseController.InputBlocked);
        if (timeline.ShakeStarted && lastShakeSerial != timeline.Serial) { ImpulseCount++; lastShakeSerial = timeline.Serial; }
    }
    // Deterministic test seam; runtime always samples absolute event timestamps.
    internal void Advance(float delta) { clock += System.Math.Max(0, delta); gameplayClock += System.Math.Max(0, delta) * Time.timeScale; Sample(); }
    internal void Clear() { pending = null; timeline.Clear(); renderedMask = 0; }
}
