using Cinemachine;
using UnityEngine;

// Consume contacts after animation events and before the Cinemachine LateUpdate.
[DefaultExecutionOrder(-10), DisallowMultipleComponent]
public sealed class CombatCameraImpact : MonoBehaviour
{
    internal enum Kind { None, Normal, Parry, Received, SlayerEnter, Lethal }
    [SerializeField] DungeonArenaCamera arena;
    [SerializeField] PlayerHealth health;
    [SerializeField] CinemachineImpulseSource recoil;
    public CameraImpactProfile profile;
    AnimationCurve recoilCurve;
    public void Configure(CameraImpactProfile value)
    {
        profile = value;
        recoilCurve = value ? new AnimationCurve(value.recoilCurve.keys) {
            preWrapMode = value.recoilCurve.preWrapMode, postWrapMode = value.recoilCurve.postWrapMode } : null;
    }
    Pulse Contact(Kind kind, CameraImpactProfile.Envelope envelope) => new Pulse {
        kind = kind, peak = envelope.peak, attack = envelope.attack, hold = envelope.hold,
        release = envelope.release, pixels = envelope.pixels, source = recoil,
        recoilDuration = profile.recoilDuration, recoilCurve = recoilCurve };
    struct Pulse
    {
        internal Kind kind;
        internal float peak, attack, hold, release, pixels, delay, recoilDuration;
        internal AnimationCurve recoilCurve;
        internal Vector3 point, direction;
        internal CinemachineImpulseSource source;
        internal float Duration => attack + hold + release;
    }
    Pulse pending, active;
    Camera view;
    Collider2D body;
    float age, startZoom, impulseStart, impulseEnd;
    ISignalSource6D ownedSignal;
    bool kicked;
    CinemachineImpulseManager.ImpulseEvent ownedImpulse;
    bool OwnsImpulse => ownedImpulse != null && ownedImpulse.m_StartTime == impulseStart &&
        ReferenceEquals(ownedImpulse.m_SignalSource, ownedSignal);
    bool ImpulseRunning => OwnsImpulse && CinemachineImpulseManager.Instance.CurrentTime < impulseEnd;
    // Cinemachine's stock SignalSource allocates on every CreateAndReturnEvent.
    // This session owns one reusable signal; expired envelopes cannot read its new velocity.
    sealed class KickSignal : ISignalSource6D
    {
        internal AnimationCurve curve;
        internal Vector3 velocity;
        internal float duration;
        public float SignalDuration => duration;
        public void GetSignal(float time, out Vector3 position, out Quaternion rotation)
        {
            position = time < 0 || time > duration ? Vector3.zero : velocity * curve.Evaluate(time / duration);
            rotation = Quaternion.identity;
        }
    }
    readonly KickSignal kickSignal = new KickSignal();
    internal float Zoom { get; private set; }
    internal Kind ActiveKind => active.kind;
    internal Kind LastKind { get; private set; }
    internal int Serial { get; private set; }
    internal int NormalCount { get; private set; }
    internal int ParryCount { get; private set; }
    internal int ReceivedCount { get; private set; }
    internal int LethalCount { get; private set; }
    internal int ImpulseCount { get; private set; }

    void OnEnable()
    {
        if (profile && recoilCurve == null) Configure(profile);
        view = arena ? arena.GetComponent<Camera>() : null;
        var manager = CinemachineImpulseManager.Instance;
        var warm = manager.NewImpulseEvent();
        warm.m_Envelope = new CinemachineImpulseManager.EnvelopeDefinition { m_SustainTime = .0001f };
        warm.m_SignalSource = null; warm.m_Radius = warm.m_DissipationDistance = 0;
        manager.AddImpulseEvent(warm); warm.m_StartTime = manager.CurrentTime - 1;
        manager.GetImpulseAt(Vector3.zero, true, 2, out _, out _);
        if (health)
        {
            body = health.GetComponent<Collider2D>();
            health.DamageAccepted += Received;
        }
    }
    void OnDisable()
    {
        if (health) health.DamageAccepted -= Received;
        Clear();
    }
    internal void Hit(Vector3 point, Vector3 direction, bool wasSlayer, bool lethal)
    {
        // The hitbox accepted this contact before TakeDamage could finish the match.
        if (!lethal && (wasSlayer || CombatSession.Finished)) return;
        var pulse = Contact(lethal ? Kind.Lethal : Kind.Normal,
            lethal ? (wasSlayer ? profile.slayerLethal : profile.lethal) : profile.normal);
        pulse.point = point; pulse.direction = direction; Queue(pulse);
    }
    internal void Parry(Vector3 point, Vector3 direction, float power)
    {
        if (CombatSession.Finished) return;
        var pulse = Contact(Kind.Parry, profile.parry);
        pulse.pixels *= Mathf.Clamp01(power); pulse.point = point; pulse.direction = direction; Queue(pulse);
    }
    void Received(Vector2 point, bool lethal)
    {
        // DamageAccepted excludes parry, rejected damage, and i-frames.
        Vector3 direction = body ? body.bounds.center - (Vector3)point :
            health ? health.transform.position - (Vector3)point : Vector3.left;
        if (direction.sqrMagnitude < .0001f) direction = Vector3.left;
        var pulse = Contact(Kind.Received, profile.received);
        pulse.point = point; pulse.direction = direction; Queue(pulse);
    }
    internal void EnterSlayer(CinemachineImpulseSource source, float delay)
    {
        if (CombatSession.Finished) return;
        Queue(new Pulse { kind = Kind.SlayerEnter, release = profile.slayerPriorityDuration,
            source = source, delay = delay, point = source ? source.transform.position : Vector3.zero });
    }
    internal void ExitSlayer()
    {
        if (pending.kind == Kind.SlayerEnter) pending = default;
        if (active.kind == Kind.SlayerEnter) Clear();
    }
    void Queue(Pulse pulse)
    {
        if (!isActiveAndEnabled || GamePauseController.InputBlocked || DungeonSession.InputSuppressed) return;
        if ((int)pulse.kind <= (int)pending.kind || (int)pulse.kind <= (int)active.kind) return;
        pending = pulse;
    }
    void LateUpdate()
    {
        if (DungeonSession.InputSuppressed || GamePauseController.EncounterReading ||
            GamePauseController.ResultOverlayVisible || TutorialRun.Active ||
            (GamePauseController.InputBlocked && !GamePauseController.IsPaused))
        {
            Clear(); return;
        }
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        Advance(Time.unscaledDeltaTime);
    }
    // One envelope, no per-contact coroutine, random noise, or accumulated lens writes.
    internal void Advance(float delta)
    {
        if (pending.kind != Kind.None)
        {
            CancelImpulse();
            startZoom = pending.kind == Kind.SlayerEnter ? 0 : Zoom;
            active = pending; pending = default; age = 0; kicked = false;
            LastKind = active.kind; Serial++;
            switch (active.kind)
            {
                case Kind.Normal: NormalCount++; break;
                case Kind.Parry: ParryCount++; break;
                case Kind.Received: ReceivedCount++; break;
                case Kind.Lethal: LethalCount++; break;
            }
        }
        if (active.kind == Kind.None) return;
        // A delayed kick retains its chance to render after hitstop, even at a slow frame rate.
        float next = age + Mathf.Max(0, delta);
        age = next;
        if (active.attack > 0 && age < active.attack)
            Zoom = Mathf.Lerp(startZoom, active.peak, Smooth(age / active.attack));
        else if (age < active.attack + active.hold) Zoom = active.peak;
        else Zoom = active.peak * (1 - Smooth((age - active.attack - active.hold) / active.release));
        if (!kicked && active.source && (active.pixels > 0 || active.kind == Kind.SlayerEnter) &&
            next >= active.delay && (active.kind == Kind.SlayerEnter || Time.timeScale >= .01f))
        {
            Emit(); kicked = true;
        }
        bool waitingForKick = !kicked && active.source && active.pixels > 0;
        if (age >= active.Duration && !waitingForKick &&
            !ImpulseRunning)
        {
            active = default; Zoom = 0;
        }
    }
    static float Smooth(float value) => Mathf.SmoothStep(0, 1, Mathf.Clamp01(value));
    void Emit()
    {
        Vector3 velocity;
        if (active.kind == Kind.SlayerEnter) velocity = active.source.m_DefaultVelocity;
        else
        {
            if (!view) return;
            float depth = Mathf.Max(.01f, view.WorldToViewportPoint(active.point).z);
            float pixel = arena ? arena.PixelSizeAtDepth(depth) : view.orthographic ? 2 * view.orthographicSize / view.pixelHeight :
                2 * depth * Mathf.Tan(view.fieldOfView * Mathf.Deg2Rad * .5f) / view.pixelHeight;
            var local = view.transform.InverseTransformDirection(active.direction);
            local.z = 0;
            if (local.sqrMagnitude < .0001f) local = Vector3.left;
            velocity = view.transform.TransformDirection(local.normalized) * (pixel * active.pixels);
        }
        var manager = CinemachineImpulseManager.Instance;
        if (active.kind == Kind.SlayerEnter)
            ownedImpulse = active.source.m_ImpulseDefinition.CreateAndReturnEvent(active.point, velocity);
        else
        {
            // Retire the cancelled predecessor before renting, so rapid preemption stays allocation-free.
            manager.GetImpulseAt(active.point, true, 2, out _, out _);
            ownedImpulse = manager.NewImpulseEvent();
            kickSignal.curve = active.recoilCurve;
            kickSignal.duration = active.recoilDuration;
            kickSignal.velocity = velocity;
            ownedImpulse.m_Envelope = new CinemachineImpulseManager.EnvelopeDefinition { m_SustainTime = kickSignal.duration };
            ownedImpulse.m_SignalSource = kickSignal;
            ownedImpulse.m_Position = active.point; ownedImpulse.m_Channel = 2;
            ownedImpulse.m_Radius = 9999999; ownedImpulse.m_DissipationDistance = 0;
            ownedImpulse.m_PropagationSpeed = float.MaxValue;
            ownedImpulse.m_DirectionMode = CinemachineImpulseManager.ImpulseEvent.DirectionMode.Fixed;
            manager.AddImpulseEvent(ownedImpulse);
        }
        if (ownedImpulse != null)
        {
            impulseStart = ownedImpulse.m_StartTime;
            ownedSignal = ownedImpulse.m_SignalSource;
            impulseEnd = impulseStart + ownedImpulse.m_Envelope.Duration;
            ImpulseCount++;
        }
    }
    internal void Clear()
    {
        pending = active = default; age = startZoom = Zoom = 0; kicked = false;
        CancelImpulse();
    }
    void CancelImpulse()
    {
        // Expired handles can already belong to another user of Cinemachine's pool.
        if (OwnsImpulse)
        {
            // Cancel(startTime) creates a zero-duration (infinite) envelope in CM2.9.
            // Retire only our event explicitly, including a same-frame teardown.
            ownedImpulse.m_SignalSource = null;
            ownedImpulse.m_Envelope = new CinemachineImpulseManager.EnvelopeDefinition { m_SustainTime = .0001f };
            ownedImpulse.m_Radius = ownedImpulse.m_DissipationDistance = 0;
            ownedImpulse.m_StartTime = CinemachineImpulseManager.Instance.CurrentTime - 1;
        }
        ownedImpulse = null; ownedSignal = null;
    }
}
