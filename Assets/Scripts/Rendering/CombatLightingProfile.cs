using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/Combat Lighting Profile")]
public sealed class CombatLightingProfile : ScriptableObject
{
    public enum Event { Parry, LegacyParry, Impact, LethalImpact, AccentImpact }
    [Serializable] public struct Pulse
    {
        public Event kind;
        public float intensity, maximum, hold, duration;
        public AnimationCurve decay;
    }
    public LightProfile combat, parry;
    [Min(1)] public int overlapCapacity;
    public Vector3 combatOffset;
    public float parryDepthOffset, parryRangeScale, defaultPixelUnits;
    public AnimationCurve parryDecay;
    internal Pulse CaptureParry(ImpactVfxProfile.Appearance appearance) => new Pulse {
        kind = Event.Parry, intensity = appearance.lightIntensity, maximum = appearance.lightIntensity,
        hold = appearance.lightHold, duration = appearance.lightDuration, decay = parryDecay
    };
    public Pulse[] pulses = Array.Empty<Pulse>();
    public Pulse Get(Event kind)
    {
        foreach (var pulse in pulses) if (pulse.kind == kind) return pulse;
        throw new InvalidOperationException(name + ": missing lighting event " + kind);
    }
    public void Validate()
    {
        if (!combat || !parry || overlapCapacity <= 0 || parryDecay == null || parryDecay.length == 0) throw new InvalidOperationException(name + ": missing combat lighting");
        combat.Validate(); parry.Validate();
        foreach (Event kind in Enum.GetValues(typeof(Event)))
        {
            if (kind == Event.Parry) continue;
            int count = 0; foreach (var p in pulses) if (p.kind == kind) count++;
            var pulse = Get(kind);
            if (count != 1 || pulse.duration <= pulse.hold || pulse.hold < 0 || pulse.decay == null || pulse.decay.length == 0)
                throw new InvalidOperationException(name + ": invalid/duplicate pulse " + kind);
        }
    }
}
