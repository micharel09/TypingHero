using UnityEngine;

// Presentation-independent, absolute-age trajectories. One floor contact and one optional bounce.
internal sealed class PhysicalParticleSimulation
{
    internal struct State
    {
        internal Vector3 origin, launch, coastOrigin, coastVelocity, impactPosition, impactVelocity;
        internal float floor, flight, bounce, total, length, mask;
        internal int channel, width;
        internal bool small;
        internal PhysicalParticleProfile.PhysicsSettings physics;
    }
    internal struct Sample
    {
        internal Vector3 position, velocity;
        internal bool alive;
    }
    readonly State[] states;
    uint random;
    internal int Count { get; private set; }
    internal State this[int index] => states[index];

    internal PhysicalParticleSimulation(int capacity) => states = new State[capacity];

    float Range(float min, float max)
    {
        random ^= random << 13; random ^= random >> 17; random ^= random << 5;
        return Mathf.Lerp(min, max, (random & 0xffffff) / 16777216f);
    }

    internal void Emit(PhysicalParticleProfile profile, Vector3 point, float attackerDirection,
        float playerDirection, float floor, int seed, PhysicalParticleProfile.RecipientSnapshot recipient = default)
    {
        Clear();
        random = unchecked((uint)(seed + 1) * 747796405u + 2891336453u);
        if (random == 0) random = 1;
        attackerDirection = attackerDirection < 0 ? -1 : 1;
        playerDirection = playerDirection == 0 ? -attackerDirection : Mathf.Sign(playerDirection);
        foreach (var group in profile.groups)
        for (int i = 0; i < group.count && Count < states.Length; i++)
        {
            float direction = group.towardPlayer ? playerDirection : attackerDirection;
            float angle = (Mathf.Lerp(group.angles.x, group.angles.y, (i + .5f) / group.count)
                + Range(-group.angleJitter, group.angleJitter)) * Mathf.Deg2Rad;
            float velocity = Range(group.speed.x, group.speed.y);
            var origin = point;
            if (group.spawnRadius > 0)
            {
                float radius = Mathf.Sqrt(Range(0, 1)) * group.spawnRadius;
                float phi = Range(0, Mathf.PI * 2);
                origin += new Vector3(Mathf.Cos(phi) * direction * radius, Mathf.Sin(phi) * radius, 0);
            }
            origin.x += playerDirection * group.playerOffset;
            Vector3 launch = new Vector3(Mathf.Cos(angle) * direction * velocity, Mathf.Sin(angle) * velocity,
                Range(group.depthSpeed.x, group.depthSpeed.y));
            if (recipient.valid && group.towardPlayer)
            {
                Vector2 axis = (Vector2)(recipient.center - point);
                if (axis.sqrMagnitude > .000001f)
                {
                    axis.Normalize();
                    // Reflect the local fan around the actual recipient axis, independently of facing.
                    launch.x = velocity * (axis.x * Mathf.Cos(angle) - axis.y * Mathf.Sin(angle) * direction);
                    launch.y = velocity * (axis.y * Mathf.Cos(angle) + axis.x * Mathf.Sin(angle) * direction);
                }
            }
            if (recipient.valid && group.spawnRegion == PhysicalParticleProfile.SpawnRegion.RecipientBody)
            {
                float phi = i * Mathf.PI * 2 / group.count + Range(-.1f, .1f);
                var outward = new Vector2(Mathf.Cos(phi) * playerDirection, Mathf.Sin(phi));
                origin = recipient.center + new Vector3(outward.x * recipient.halfAxes.x, outward.y * recipient.halfAxes.y, 0);
                launch.x = outward.x * velocity; launch.y = outward.y * velocity;
            }
            var pixels = i % 2 == (seed & 1) ? group.longPixels : group.shortPixels;
            var state = new State {
                origin = origin, floor = floor, channel = group.channel, small = group.small,
                launch = launch,
                length = Mathf.Floor(Range(pixels.x, pixels.y + 1)), width = group.widthPixels,
                total = Range(group.lifetime.x, group.lifetime.y), mask = Mathf.Floor(Range(0, 3)),
                physics = group.physics, flight = float.PositiveInfinity
            };
            if (state.physics.collideWithFloor) state.origin.y = Mathf.Max(state.origin.y, floor);
            Launch(state, state.physics.launchDuration, out state.coastOrigin, out state.coastVelocity);
            if (state.physics.collideWithFloor)
            {
                // Starting on the floor with an upward launch must first reach its apex.
                float lo = 0, hi = state.total;
                Flight(state, hi, out var end, out _);
                if (end.y <= floor)
                {
                    if (state.origin.y <= floor && state.launch.y <= 0) hi = 0;
                    for (int n = 0; n < 28; n++)
                    {
                        float mid = (lo + hi) * .5f;
                        Flight(state, mid, out var position, out _);
                        if (position.y > floor) lo = mid; else hi = mid;
                    }
                    state.flight = (lo + hi) * .5f;
                    Flight(state, state.flight, out state.impactPosition, out state.impactVelocity);
                    state.impactPosition.y = floor;
                    state.bounce = state.physics.gravity > 0 ? Mathf.Max(0,
                        -2 * state.impactVelocity.y * state.physics.restitution / state.physics.gravity) : 0;
                    state.total = Mathf.Min(state.total, state.flight + state.bounce + state.physics.restDuration);
                }
            }
            states[Count++] = state;
        }
    }

    static void DragIntegrals(float drag, float t, out float travel, out float fall)
    {
        if (drag == 0) { travel = t; fall = (float)(.5 * t * t); return; }
        double time = t, x = drag * time;
        // Both closed forms subtract nearly equal numbers near zero. Expand in
        // dimensionless drag*time, not drag alone, and retain double intermediates.
        if (x < .01)
        {
            travel = (float)(time * (1 + x * (-.5 + x * (1d / 6 + x * (-1d / 24 + x / 120)))));
            fall = (float)(time * time * (.5 + x * (-1d / 6 + x * (1d / 24 + x * (-1d / 120 + x / 720)))));
        }
        else
        {
            double distance = (1 - System.Math.Exp(-x)) / drag;
            travel = (float)distance;
            fall = (float)((time - distance) / drag);
        }
    }

    static float Travel(float drag, float t)
    {
        DragIntegrals(drag, t, out float travel, out _);
        return travel;
    }

    static void Launch(State d, float t, out Vector3 position, out Vector3 velocity)
    {
        float drag = d.physics.launchDrag;
        DragIntegrals(drag, t, out float travel, out float fall);
        velocity = d.launch * Mathf.Exp(-drag * t) + Vector3.down * (d.physics.gravity * travel);
        position = d.origin + d.launch * travel + Vector3.down * (d.physics.gravity * fall);
    }

    static void Flight(State d, float t, out Vector3 position, out Vector3 velocity)
    {
        if (t <= d.physics.launchDuration) { Launch(d, t, out position, out velocity); return; }
        t -= d.physics.launchDuration;
        float decay = Mathf.Exp(-d.physics.flightDrag * t), travel = Travel(d.physics.flightDrag, t);
        velocity = new Vector3(d.coastVelocity.x * decay, d.coastVelocity.y - d.physics.gravity * t, d.coastVelocity.z * decay);
        position = d.coastOrigin + new Vector3(d.coastVelocity.x * travel,
            d.coastVelocity.y * t - .5f * d.physics.gravity * t * t, d.coastVelocity.z * travel);
    }

    internal Sample Evaluate(int index, float age)
    {
        var d = states[index];
        if (age >= d.total) return default;
        Flight(d, age, out var position, out var velocity);
        if (age >= d.flight)
        {
            float t = Mathf.Clamp(age - d.flight, 0, d.bounce);
            float travel = Travel(d.physics.flightDrag, t);
            position = d.impactPosition + new Vector3(d.impactVelocity.x * d.physics.tangentRetention * travel,
                .5f * d.physics.gravity * t * (d.bounce - t), d.impactVelocity.z * d.physics.tangentRetention * travel);
            velocity = age >= d.flight + d.bounce ? Vector3.zero : new Vector3(
                d.impactVelocity.x * d.physics.tangentRetention * Mathf.Exp(-d.physics.flightDrag * t),
                -d.impactVelocity.y * d.physics.restitution - d.physics.gravity * t,
                d.impactVelocity.z * d.physics.tangentRetention * Mathf.Exp(-d.physics.flightDrag * t));
        }
        return new Sample { position = position, velocity = velocity, alive = true };
    }

    internal void Clear() { System.Array.Clear(states, 0, states.Length); Count = 0; }
}
