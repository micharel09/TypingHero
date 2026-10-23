using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CLACKBLADE/Combat Camera Profile")]
public sealed class CombatCameraProfile : ScriptableObject
{
    public enum Event { Normal, Parry, Received, SlayerEnter, Lethal, SlayerLethal }
    public enum ShakeUnits { WorldPixels, WorldUnits }
    [Serializable] public sealed class Preset
    {
        public Event trigger;
        public bool enabled = true;
        public int priority;
        [Tooltip("Seconds of minimum priority ownership, including time waiting for render.")] public float minimumPriorityTime;
        [Tooltip("Fraction of base zoom: 0.02 = 2%. Slayer entry uses the existing base lens tween.")] public float zoomPeak;
        [Tooltip("Seconds")] public float attack, hold, release;
        public AnimationCurve attackCurve, releaseCurve;
        public ShakeUnits units;
        public float strength;
        public bool contactDirection = true;
        [Tooltip("Fixed direction in the camera plane (camera-space X/Y). Used when contactDirection is off.")] public Vector3 direction = Vector3.down;
        [Tooltip("Seconds, on the pause-excluded unscaled clock")] public float shakeDelay;
        [Tooltip("Seconds, on the gameplay clock")] public float shakeDuration;
        public bool afterHitstop = true;
        [Tooltip("Normalized time. Signed displacement multiplier, including the migrated listener gain.")] public AnimationCurve shakeEnvelope;

        internal float shakePeakTime, shakePeakValue;
        internal Preset Prepare()
        {
            var copy = (Preset)MemberwiseClone();
            copy.attackCurve = Clone(attackCurve); copy.releaseCurve = Clone(releaseCurve);
            copy.shakeEnvelope = Clone(shakeEnvelope);
            for (int i = 0; i <= 4096; i++)
            {
                float t = i / 4096f, value = copy.shakeEnvelope.Evaluate(t);
                if (Mathf.Abs(value) > Mathf.Abs(copy.shakePeakValue)) { copy.shakePeakValue = value; copy.shakePeakTime = t; }
            }
            return copy;
        }
        static AnimationCurve Clone(AnimationCurve curve) => new AnimationCurve(curve.keys) {
            preWrapMode = curve.preWrapMode, postWrapMode = curve.postWrapMode };
    }
    public Preset[] presets;
    public void Validate()
    {
        if (presets == null || presets.Length != 6) throw new InvalidOperationException(name + ": six camera presets required");
        int seen = 0;
        foreach (var p in presets)
        {
            if (p == null || (int)p.trigger < 0 || (int)p.trigger >= 6 || (seen & (1 << (int)p.trigger)) != 0)
                throw new InvalidOperationException(name + ": missing/duplicate camera event");
            seen |= 1 << (int)p.trigger;
            foreach (float value in new[] { p.minimumPriorityTime, p.zoomPeak, p.attack, p.hold, p.release,
                p.strength, p.shakeDelay, p.shakeDuration })
                if (float.IsNaN(value) || float.IsInfinity(value) || value < 0) throw new InvalidOperationException(name + ": invalid camera time/strength");
            ValidateCurve(p.attackCurve); ValidateCurve(p.releaseCurve); ValidateCurve(p.shakeEnvelope);
            if (p.zoomPeak > 0 && p.release <= 0 || p.strength > 0 && (p.shakeDuration <= 0 || p.direction.sqrMagnitude == 0 && !p.contactDirection))
                throw new InvalidOperationException(name + ": invalid camera duration/direction");
            if (Mathf.Abs(p.attackCurve.Evaluate(0)) > .0001f || Mathf.Abs(p.attackCurve.Evaluate(1) - 1) > .0001f ||
                Mathf.Abs(p.releaseCurve.Evaluate(0) - 1) > .0001f || Mathf.Abs(p.releaseCurve.Evaluate(1)) > .0001f ||
                Mathf.Abs(p.shakeEnvelope.Evaluate(1)) > .0001f)
                throw new InvalidOperationException(name + ": curves must reach their authored endpoints");
        }
    }
    void ValidateCurve(AnimationCurve curve)
    {
        if (curve == null || curve.length < 2 || curve[0].time != 0 || curve[curve.length - 1].time != 1)
            throw new InvalidOperationException(name + ": camera curve must cover normalized time 0..1");
        foreach (var key in curve.keys)
            if (float.IsNaN(key.time) || float.IsNaN(key.value) || float.IsInfinity(key.value) ||
                float.IsNaN(key.inTangent) || float.IsNaN(key.outTangent)) throw new InvalidOperationException(name + ": invalid camera curve key");
    }
    internal Preset[] Prepare()
    {
        Validate(); var prepared = new Preset[6];
        foreach (var p in presets) prepared[(int)p.trigger] = p.Prepare();
        return prepared;
    }
}
