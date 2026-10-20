using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(200)]
public sealed class VoxelImpactBurst : MonoBehaviour
{
    // Normal, Slayer, lethal, received, received-lethal. Serialized by CombatVoxelAssets.
    [SerializeField] int profile;
    [SerializeField] ParticleSystem fragments, rear;
    struct Fragment
    {
        public ParticleSystem.Particle particle;
        public Vector3 origin, launch, impact, size, spin, orientation;
        public float flight, bounce, life;
        public Color color;
        public bool accent;
    }
    readonly Fragment[] states = new Fragment[14];
    readonly ParticleSystem.Particle[] output = new ParticleSystem.Particle[14];
    readonly List<Vector4> heatData = new List<Vector4>(14);
    PooledVFX lease;
    float age, floor;
    int count;
    uint random = 1;
    internal bool Lethal => profile == 2 || profile == 4;
    internal int ParticleCount => fragments.particleCount + rear.particleCount;
    internal int Role => profile;
    internal ParticleSystem[] Systems { get; private set; }
    float Next() { random ^= random << 13; random ^= random >> 17; random ^= random << 5; return (random & 0xffffff) / 16777216f; }
    float Range(float a, float b) => Mathf.Lerp(a, b, Next());
    void Awake() { lease = GetComponent<PooledVFX>(); Systems = new[] {fragments, rear}; }
    internal void Emit(Vector3 point, float direction, float groundY, int sequence, Color accent)
    {
        Clear(); random = (uint)(sequence + 1) * 747796405u + (uint)(profile + 1) * 2891336453u;
        floor = groundY; direction = direction < 0 ? -1 : 1;
        count = profile == 0 ? 6 : profile == 1 ? 8 : profile == 2 ? 14 : profile == 3 ? 4 : 6;
        transform.SetPositionAndRotation(point, Quaternion.identity); transform.localScale = Vector3.one;
        var ps = fragments.main; ps.simulationSpeed = 0;
        float force = profile == 1 ? 1.15f : Lethal ? 1.35f : 1;
        for (int i = 0; i < count; i++)
        {
            float edge = (Lethal ? (i % 2 + 2) : (i % 2 + 1)) / 16f;
            var d = new Fragment {
                origin = point, size = new Vector3(edge, edge * (i % 3 == 0 ? 2 : 1), edge),
                launch = new Vector3(direction * (i < count - 1 ? 1 : -1) * Range(2.5f, 5) * force,
                    Range(1.5f, 3.5f) * force, (i % 2 == 0 ? -1 : 1) * Range(1, 2)),
                orientation = new Vector3(Range(15, 65), Range(0, 360), Range(0, 360)),
                spin = new Vector3(Range(180, 540), Range(180, 540), Range(180, 540)),
                accent = accent.a > 0 && (profile == 1 || profile == 2) && i >= count - 2
            };
            d.color = profile >= 3 ? (i % 2 == 0 ? new Color(.46f, .51f, .58f) : new Color(.4f, .29f, .2f)) :
                i < count - 2 ? new Color(.85f, .8f, .67f) : new Color(.55f, .57f, .6f);
            if (d.accent) d.color = accent;
            // A mesh rests on its lower face, not with half its volume under the floor.
            float y = floor + d.size.y * .5f;
            d.origin.y = Mathf.Max(d.origin.y, y);
            d.flight = (d.launch.y + Mathf.Sqrt(d.launch.y * d.launch.y + 19.62f * (d.origin.y - y))) / 9.81f;
            d.impact = d.origin + d.launch * d.flight + Vector3.down * (4.905f * d.flight * d.flight); d.impact.y = y;
            d.bounce = Mathf.Max(0, (9.81f * d.flight - d.launch.y) * .36f / 9.81f);
            d.life = d.flight + d.bounce + .14f;
            Systems[i % 2].Emit(new ParticleSystem.EmitParams {position = point, startSize3D = d.size,
                startLifetime = d.life + .1f, startColor = d.color}, 1);
            states[i] = d;
        }
        for (int group = 0; group < 2; group++)
        {
            Systems[group].GetParticles(output); int slot = 0;
            for (int i = group; i < count; i += 2) states[i].particle = output[slot++];
            Systems[group].Play(false);
        }
        Advance(0);
        ClashLightingRig.Impact(gameObject.scene, point, profile == 1 ? 1.8f : Lethal ? 2.8f : 1.2f,
            profile == 1 ? .03f : Lethal ? .04f : 0, profile == 1 ? .14f : Lethal ? .2f : .1f, this);
    }
    internal void Advance(float delta)
    {
        age += delta;
        for (int group = 0; group < 2; group++)
        {
            int live = 0; heatData.Clear();
            for (int i = 0; i < count; i++)
            {
                if (i % 2 != group) continue;
                var d = states[i]; if (age >= d.life) continue;
                var p = d.particle;
                Vector3 position = d.origin + d.launch * age + Vector3.down * (4.905f * age * age);
                Vector3 velocity = d.launch + Vector3.down * (9.81f * age);
                if (age >= d.flight)
                {
                    float b = Mathf.Clamp(age - d.flight, 0, d.bounce);
                    position = d.impact + new Vector3(d.launch.x * .45f * b, 4.905f * b * (d.bounce - b), d.launch.z * .45f * b);
                    velocity = age >= d.flight + d.bounce ? Vector3.zero : new Vector3(d.launch.x * .45f, 4.905f * (d.bounce - 2 * b), d.launch.z * .45f);
                }
                float spinTime = Mathf.Min(age, d.flight + d.bounce);
                var rotation = d.orientation + d.spin * spinTime;
                float settle = Mathf.SmoothStep(0, 1, Mathf.Clamp01((age - d.flight) / Mathf.Max(.001f, d.bounce)));
                rotation = Vector3.Lerp(rotation, new Vector3(0, rotation.y, 0), settle);
                var q = Quaternion.Euler(rotation); Vector3 half = d.size * .5f;
                float support = Mathf.Abs((q * Vector3.right).y) * half.x + Mathf.Abs((q * Vector3.up).y) * half.y + Mathf.Abs((q * Vector3.forward).y) * half.z;
                position.y = Mathf.Max(position.y, floor + support);
                p.position = position; p.velocity = velocity; p.rotation3D = rotation; p.startSize3D = d.size;
                Color tint = Color.Lerp(d.color, d.color * .42f, Mathf.Clamp01(age / d.life));
                tint.a = 1 - Mathf.Clamp01((age - d.flight - d.bounce) / .14f);
                p.startColor = tint; p.remainingLifetime = d.life - age + .1f;
                output[live++] = p; heatData.Add(new Vector4(d.accent ? (1 - age / d.life) * .35f : 0, 0, 0, 0));
            }
            Systems[group].SetParticles(output, live); Systems[group].SetCustomParticleData(heatData, ParticleSystemCustomData.Custom1);
        }
    }
    void Update()
    {
        if (count == 0) return;
        if (GamePauseController.ResultOverlayVisible || (CombatSession.Finished && !Lethal)) { lease.Release(); return; }
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        Advance(Time.unscaledDeltaTime);
        if (ParticleCount == 0) lease.Release();
    }
    internal void Clear()
    {
        if (fragments) fragments.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (rear) rear.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        age = 0; count = 0; ClashLightingRig.Cancel(this);
    }
    void OnDisable() => Clear();
}
