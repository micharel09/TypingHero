using UnityEngine;

[CreateAssetMenu(menuName = "TypingHero/VFX/Impact VFX Profile")]
public sealed class ImpactVfxProfile : ScriptableObject
{
    public PhysicalParticleProfile physics;
    public GameObject prefab;
    public Color hot = new Color(1, .8667f, .5f), warm = new Color(.8f, .25f, .03f);
    public Color cooled = new Color(.65f, .2f, .025f), legacyCooled = new Color(.18f, .015f, 0);
    [Min(.001f)] public float fadeDuration = .08f;
    [Min(0)] public float lightIntensity = 5, lightDuration = .28f, lightHold = .07f;
    [Min(0)] public float lightRange = 3, lightOffset = .15f;

    // Value snapshot: editing the asset never changes a living burst or pulse.
    internal struct Appearance
    {
        internal Color hot, warm, cooled, legacyCooled;
        internal float fadeDuration, lightIntensity, lightDuration, lightHold, lightRange, lightOffset;
    }
    internal static Appearance DefaultAppearance => new Appearance {
        hot = new Color(1, .8667f, .5f), warm = new Color(.8f, .25f, .03f),
        cooled = new Color(.65f, .2f, .025f), legacyCooled = new Color(.18f, .015f, 0),
        fadeDuration = .08f, lightIntensity = 5, lightDuration = .28f, lightHold = .07f,
        lightRange = 3, lightOffset = .15f
    };
    internal Appearance Capture() => new Appearance {
        hot = hot, warm = warm, cooled = cooled, legacyCooled = legacyCooled,
        fadeDuration = Mathf.Max(.001f, fadeDuration), lightIntensity = lightIntensity,
        lightDuration = lightDuration, lightHold = lightHold, lightRange = lightRange, lightOffset = lightOffset
    };
}
