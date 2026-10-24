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

    [Min(1)] public float pixelsPerUnit = 16, unitsPerMeter = 1.25f;
    [Min(0)] public float depthScale = .7f;
    public Vector2 depthLimits = new Vector2(.75f, 1.5f);
    [SerializeField, Min(0)] int flashSizeReductionPixels;
    [SerializeField, Min(0)] float flashRotationDegrees;

    // Presentation-only samples; never advance the physical or gameplay random stream.
    internal bool TryVaryFlash(int seed, int channel, ref float size, out float rotation)
    {
        rotation = 0;
        if (flashSizeReductionPixels == 0 && flashRotationDegrees == 0) return false;
        uint value = unchecked((uint)seed * 747796405u + (uint)channel * 2891336453u + 277803737u);
        value = unchecked((value ^ (value >> 16)) * 2246822519u);
        value = unchecked((value ^ (value >> 13)) * 3266489917u);
        value ^= value >> 16;
        int reduction = (int)(value % ((uint)flashSizeReductionPixels + 1));
        size = Mathf.Max(0, size - reduction);
        rotation = (((value >> 8) & 0xffffff) / 16777215f * 2 - 1) * flashRotationDegrees;
        return true;
    }

    [System.Serializable] public sealed class FlashLayer
    {
        public int channel;
        [Min(0)] public int count = 1;
        [Min(.001f)] public float lifetime = .2f;
        public Color tint = Color.white;
        [Min(0)] public float sizePixels = 16, maxWorldSize;
        [Min(1)] public int columns = 1, rows = 1, cycles = 1;
        public float startFrame;
        public AnimationCurve frameOverTime = AnimationCurve.Constant(0, 1, 0);
        public Gradient colorOverLifetime = new Gradient();
    }
    public FlashLayer[] flashes = System.Array.Empty<FlashLayer>();
    public bool lightEnabled = true;
    public Color lightColor = Color.white;
    public AnimationCurve lightDecay = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField, HideInInspector] int authoringVersion;
    internal static AnimationCurve CopyCurve(AnimationCurve source) => new AnimationCurve(source.keys) {
        preWrapMode = source.preWrapMode, postWrapMode = source.postWrapMode };
    internal void Validate(int capacity)
    {
        void Fail(string reason) => throw new System.InvalidOperationException(name + ": " + reason);
        if (!physics || physics.groups == null || flashes == null) Fail("missing impact configuration");
        if (pixelsPerUnit <= 0 || unitsPerMeter <= 0 || depthLimits.x <= 0 || depthLimits.y < depthLimits.x)
            Fail("invalid pixel scale");
        int total = 0;
        foreach (var group in physics.groups)
        {
            if (group == null || group.count < 0) Fail("invalid physical group");
            total = checked(total + group.count);
        }
        if (total > capacity) Fail("physical particle count exceeds capacity " + capacity);
        for (int i = 0; i < flashes.Length; i++)
        {
            var flash = flashes[i];
            if (flash == null || flash.count < 0 || flash.lifetime <= 0 || flash.sizePixels < 0 ||
                flash.maxWorldSize < 0 || flash.columns < 1 || flash.rows < 1 || flash.cycles < 1 ||
                flash.frameOverTime == null || flash.frameOverTime.length == 0 || flash.colorOverLifetime == null)
                Fail("invalid flash layer");
            for (int j = 0; j < i; j++) if (flashes[j].channel == flash.channel) Fail("duplicate flash channel");
            foreach (var group in physics.groups)
                if (group.count > 0 && group.channel == flash.channel) Fail("flash and physics share a renderer");
        }
        if (lightEnabled && lightIntensity > 0 && (lightHold < 0 || lightDuration <= lightHold ||
            lightRange <= 0 || lightDecay == null || lightDecay.length == 0)) Fail("invalid local light pulse");
    }

    // Value snapshot: editing the asset never changes a living burst or pulse.
    internal struct Appearance
    {
        internal Color hot, warm, cooled, legacyCooled, lightColor;
        internal AnimationCurve lightDecay;
        internal float fadeDuration, lightIntensity, lightDuration, lightHold, lightRange, lightOffset;
    }
    internal Appearance Capture() => new Appearance {
        hot = hot, warm = warm, cooled = cooled, legacyCooled = legacyCooled,
        fadeDuration = Mathf.Max(.001f, fadeDuration), lightIntensity = lightEnabled ? lightIntensity : 0,
        lightDuration = lightDuration, lightHold = lightHold, lightRange = lightRange, lightOffset = lightOffset, lightColor = lightColor,
        lightDecay = lightEnabled && lightIntensity > 0 ? CopyCurve(lightDecay) : null
    };
}
