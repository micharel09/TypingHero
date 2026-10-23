using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CLACKBLADE/Lighting/Camera Impact Profile")]
public sealed class CameraImpactProfile : ScriptableObject
{
    [Serializable] public struct Envelope
    {
        public float peak, attack, hold, release, pixels;
        public Envelope(float peak, float attack, float hold, float release, float pixels)
        { this.peak = peak; this.attack = attack; this.hold = hold; this.release = release; this.pixels = pixels; }
    }
    public Envelope normal = new Envelope(.02f, .02f, .02f, .1f, 0);
    public Envelope parry = new Envelope(.03f, .02f, .03f, .13f, 1);
    public Envelope received = new Envelope(0, 0, 0, .1f, 1);
    public Envelope lethal = new Envelope(.05f, .03f, .04f, .18f, 2);
    public Envelope slayerLethal = new Envelope(.02f, .03f, .04f, .18f, 2);
    public float slayerPriorityDuration = .35f;
    public float recoilDuration = .1f;
    public AnimationCurve recoilCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.2f, 1), new Keyframe(1, 0));
}
