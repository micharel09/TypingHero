using NUnit.Framework;
using UnityEngine;

public sealed class PhysicalParticleTests
{
#if UNITY_EDITOR
    [TestCase(-1f), TestCase(1f), Category("Quick")]
    public void RecipientSnapshotOwnsAllRaysAndFourPerimeterSpecks(float side)
    {
        var profile = UnityEditor.AssetDatabase.LoadAssetAtPath<PhysicalParticleProfile>("Assets/Art/VFX/ParryClash/ParryPhysics.asset");
        var simulation = new PhysicalParticleSimulation(40);
        var point = new Vector3(side * 2, 3, 0);
        var body = PhysicalParticleProfile.RecipientSnapshot.FromBounds(new Bounds(new Vector3(0, .5f, 0), new Vector3(1, 2, 0)));
        var center = body.center; var axes = body.halfAxes;
        simulation.Emit(profile, point, -side, side, -3, 4, body); // Both legacy directions deliberately wrong.
        body.center += Vector3.one * 20; // Mutating the caller snapshot cannot drag the emitted burst.
        Assert.That(simulation.Count, Is.EqualTo(22));
        int specks = 0, rays = 0, outsideBody = 0, pastBack = 0;
        for (int i = 0; i < simulation.Count; i++)
        {
            var state = simulation[i];
            if (!state.small)
            {
                rays++; Assert.That(state.origin, Is.EqualTo(point));
                float angle = Mathf.Atan2(state.launch.y, -side * state.launch.x) * Mathf.Rad2Deg;
                Assert.That(angle, Is.InRange(-35, 35));
                Assert.That(((Vector2)state.launch).magnitude, Is.InRange(state.channel == 1 ? 8 : 14, 28));
                Assert.That(state.coastVelocity.x / state.launch.x, Is.EqualTo(Mathf.Exp(-.6f)).Within(.00001f));
                if (state.channel != 1)
                {
                    var flight = simulation.Evaluate(i, .25f);
                    if (state.length >= 6)
                    {
                        Assert.That((flight.position.x - center.x) * -side, Is.GreaterThan(axes.x));
                        pastBack++;
                    }
                    Assert.That(state.physics.flightDrag, Is.EqualTo(3f));
                }
            }
            else
            {
                specks++; var offset = state.origin - center;
                if (Mathf.Abs(state.origin.x) > .5f) outsideBody++;
                Assert.That(offset.x * offset.x / (axes.x * axes.x) + offset.y * offset.y / (axes.y * axes.y), Is.EqualTo(1).Within(.00001));
                Assert.That(Vector2.Dot(offset, state.launch), Is.GreaterThan(0));
                Assert.That(state.length, Is.InRange(1, 2)); Assert.That(state.total, Is.LessThanOrEqualTo(.45f));
                Assert.That(state.physics.flightDrag, Is.EqualTo(6)); Assert.That(state.physics.collideWithFloor, Is.True); Assert.That(state.physics.restitution, Is.Zero);
                Assert.That(((Vector2)state.launch).magnitude, Is.InRange(1, 2.5f));
            }
        }
        Assert.That(rays, Is.EqualTo(18)); Assert.That(specks, Is.EqualTo(4));
        Assert.That(pastBack, Is.GreaterThanOrEqualTo(4), "all lead rays must visibly pass the recipient's back by250ms");
        Assert.That(outsideBody, Is.GreaterThanOrEqualTo(2), "side specks must start outside the body, not all four behind its sprite");
    }
#endif

    [Test, Category("Quick")]
    public void NewEmissionOptionsSnapshotCoolingAndKeepLegacyDefaults()
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            var group = new PhysicalParticleProfile.Group { count = 4, lifetime = Vector2.one * .4f,
                coolingDuration = Vector2.one * .8f, angleJitter = 0,
                angleSampling = PhysicalParticleProfile.AngleSampling.Independent,
                pixelScaleMode = PhysicalParticleProfile.PixelScaleMode.VelocityOnly };
            profile.groups = new[] { group };
            var sim = new PhysicalParticleSimulation(4);
            sim.Emit(profile, Vector3.zero, 1, -1, -10, 3);
            var original = sim[0];
            Assert.That(original.coolingDuration, Is.EqualTo(.4f));
            group.coolingDuration = Vector2.one * .1f;
            group.pixelScaleMode = PhysicalParticleProfile.PixelScaleMode.LegacyDepth;
            group.angleSampling = PhysicalParticleProfile.AngleSampling.Stratified;
            Assert.That(sim[0].coolingDuration, Is.EqualTo(.4f));
            Assert.That(sim[0].pixelScaleMode, Is.EqualTo(PhysicalParticleProfile.PixelScaleMode.VelocityOnly));
            Assert.That(sim[0].angleSampling, Is.EqualTo(PhysicalParticleProfile.AngleSampling.Independent));
            group.coolingDuration = Vector2.zero;
            sim.Emit(profile, Vector3.zero, 1, -1, -10, 3);
            Assert.That(sim[0].coolingDuration, Is.Zero);
            Assert.That(new PhysicalParticleProfile.Group().angleSampling, Is.EqualTo(PhysicalParticleProfile.AngleSampling.Stratified));
            Assert.That(new PhysicalParticleProfile.Group().pixelScaleMode, Is.EqualTo(PhysicalParticleProfile.PixelScaleMode.LegacyDepth));
        }
        finally { Object.DestroyImmediate(profile); }
    }

#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void LeadForceAndIndependentSamplingAreReproducibleAcrossFiniteSeedsAndSides()
    {
        var profile = UnityEditor.AssetDatabase.LoadAssetAtPath<PhysicalParticleProfile>("Assets/Art/VFX/ParryClash/ParryPhysics.asset");
        var first = new PhysicalParticleSimulation(40); var second = new PhysicalParticleSimulation(40);
        for (int seed = 0; seed < 24; seed++)
        foreach (float side in new[] { -1f, 1f })
        {
            var body = new PhysicalParticleProfile.RecipientSnapshot { valid = true,
                center = Vector3.up, halfAxes = new Vector2(.5f, .6f) };
            var point = new Vector3(side * 2, 1, 0);
            first.Emit(profile, point, -side, side, -5, seed, body);
            second.Emit(profile, point, -side, side, -5, seed, body);
            int leads = 0;
            for (int i = 0; i < first.Count; i++)
            {
                var state = first[i];
                Assert.That(state.launch, Is.EqualTo(second[i].launch));
                Assert.That(state.coolingDuration, Is.EqualTo(second[i].coolingDuration));
                Assert.That(state.pixelScaleMode, Is.EqualTo(PhysicalParticleProfile.PixelScaleMode.VelocityOnly));
                Assert.That(state.total, Is.LessThanOrEqualTo(.75f));
                if (!state.small) Assert.That(state.width, Is.EqualTo(1));
                if (state.length < 6) continue;
                leads++;
                Assert.That((first.Evaluate(i, .25f).position.x - body.center.x) * -side,
                    Is.GreaterThan(body.halfAxes.x), "every lead passes the captured rear edge by250ms");
            }
            Assert.That(leads, Is.EqualTo(4));
        }
    }
#endif

    [Test, Category("Quick")]
    public void LegacyRecipientAimStillFollowsBodyAndSnapshotsItsDirection()
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            profile.groups = new[] { new PhysicalParticleProfile.Group {
                count = 1, towardPlayer = true, angles = Vector2.zero, angleJitter = 0,
                speed = Vector2.one * 10, lifetime = Vector2.one
            } };
            Assert.That(profile.groups[0].pastRecipient, Is.False);
            var body = new PhysicalParticleProfile.RecipientSnapshot { valid = true, center = new Vector3(-2, -2, 0) };
            var simulation = new PhysicalParticleSimulation(1);
            simulation.Emit(profile, Vector3.zero, 1, -1, -10, 0, body);
            Assert.That(Vector2.Angle(body.center, simulation[0].launch), Is.LessThan(.01f));
            var before = simulation.Evaluate(0, .2f);
            body.center = Vector3.one * 30;
            Assert.That(simulation.Evaluate(0, .2f).position, Is.EqualTo(before.position));
        }
        finally { Object.DestroyImmediate(profile); }
    }

    [Test, Category("Quick")]
    public void SmallLaunchDragRetainsBallisticGravityInsteadOfDroppingMetres()
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            profile.groups = new[] { new PhysicalParticleProfile.Group {
                count = 1, angles = Vector2.zero, angleJitter = 0, speed = Vector2.one * 2,
                lifetime = Vector2.one * 3,
                physics = new PhysicalParticleProfile.PhysicsSettings {
                    gravity = 9.81f, launchDuration = .12f, launchDrag = .0001f
                }
            } };
            var simulation = new PhysicalParticleSimulation(1);
            simulation.Emit(profile, Vector3.zero, 1, -1, 0, 0);
            var sample = simulation.Evaluate(0, .12f);
            Assert.That(sample.alive, Is.True);
            Assert.That(sample.position.y, Is.EqualTo(-.07063172f).Within(.00001f));
            Assert.That(sample.position.x, Is.EqualTo(.23999856f).Within(.00001f));
        }
        finally { Object.DestroyImmediate(profile); }
    }

    [TestCase(0f), TestCase(1e-8f), TestCase(1e-6f), TestCase(.0001f), TestCase(.001f)]
    [TestCase(.08325f), TestCase(.083333336f), TestCase(.08341667f)]
    [TestCase(2f), TestCase(6f), TestCase(20f), Category("Quick")]
    public void DragTrajectoriesMatchReferenceThroughLaunchAndFlight(float drag)
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            var group = new PhysicalParticleProfile.Group {
                count = 1, angles = Vector2.one * 30, angleJitter = 0,
                speed = Vector2.one * 24, depthSpeed = Vector2.one * -4,
                lifetime = Vector2.one * 3,
                physics = new PhysicalParticleProfile.PhysicsSettings {
                    launchDuration = .12f, launchDrag = drag, flightDrag = drag
                }
            };
            profile.groups = new[] { group };
            var simulation = new PhysicalParticleSimulation(1);
            foreach (float gravity in new[] { 0f, 9.81f })
            {
                group.physics.gravity = gravity;
                simulation.Emit(profile, Vector3.up, 1, -1, 0, 0);
                var state = simulation[0];
                foreach (float age in new[] { 0f, .0001f, .03f, .119999f, .12f, .120001f, .3f, 1f, 2.5f })
                {
                    var sample = simulation.Evaluate(0, age);
                    Assert.That(sample.alive, Is.True);
                    ReferenceFlight(state, age, out var position, out var velocity);
                    AssertVector(sample.position, position, $"position: drag={drag}, gravity={gravity}, age={age}");
                    AssertVector(sample.velocity, velocity, $"velocity: drag={drag}, gravity={gravity}, age={age}");
                }
            }
        }
        finally { Object.DestroyImmediate(profile); }
    }

    [TestCase(0f), TestCase(1e-8f), TestCase(.0001f), TestCase(.001f), Category("Quick")]
    public void SmallDragFloorContactAndBounceRemainContinuous(float drag)
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            profile.groups = new[] { new PhysicalParticleProfile.Group {
                count = 1, angles = Vector2.one * 60, angleJitter = 0,
                speed = Vector2.one * 2, depthSpeed = Vector2.one,
                lifetime = Vector2.one * 3,
                physics = new PhysicalParticleProfile.PhysicsSettings {
                    gravity = 10, launchDuration = .12f, launchDrag = drag, flightDrag = drag,
                    collideWithFloor = true, restitution = .25f, tangentRetention = .5f, restDuration = .2f
                }
            } };
            var simulation = new PhysicalParticleSimulation(1);
            simulation.Emit(profile, Vector3.up, 1, -1, 0, 0);
            var state = simulation[0];
            double lo = 0, hi = 2;
            for (int i = 0; i < 50; i++)
            {
                double mid = (lo + hi) / 2;
                ReferenceFlight(state, mid, out var position, out _);
                if (position[1] > 0) lo = mid; else hi = mid;
            }
            Assert.That(state.flight, Is.EqualTo((lo + hi) / 2).Within(.0001));
            ReferenceFlight(state, (lo + hi) / 2, out var impact, out var impactVelocity);
            double bounce = -2 * impactVelocity[1] * state.physics.restitution / state.physics.gravity;
            Assert.That(state.bounce, Is.EqualTo(bounce).Within(.0001));
            float previousY = 0;
            int rises = 0;
            for (float age = state.flight; age < state.total; age += .001f)
            {
                var sample = simulation.Evaluate(0, age);
                double t = System.Math.Min(System.Math.Max(0, age - state.flight), state.bounce);
                ReferenceIntegrals(drag, t, out double travel, out _);
                AssertVector(sample.position, new[] {
                    impact[0] + impactVelocity[0] * state.physics.tangentRetention * travel,
                    .5 * state.physics.gravity * t * (state.bounce - t),
                    impact[2] + impactVelocity[2] * state.physics.tangentRetention * travel
                }, "bounce and rest position");
                Assert.That(sample.position.y, Is.GreaterThanOrEqualTo(0));
                if (sample.position.y > previousY + .000001f) rises++;
                previousY = sample.position.y;
            }
            Assert.That(rises, Is.GreaterThan(0), "one upward bounce must remain visible");
            var rest = simulation.Evaluate(0, state.flight + state.bounce + .1f);
            Assert.That(rest.position.y, Is.Zero);
            Assert.That(rest.velocity, Is.EqualTo(Vector3.zero));
            Assert.That(simulation.Evaluate(0, state.total).alive, Is.False);
        }
        finally { Object.DestroyImmediate(profile); }
    }

    // Independent double-precision oracle: sum a convergent series well beyond
    // the runtime polynomial near zero, and use the closed form elsewhere.
    static void ReferenceIntegrals(double drag, double t, out double travel, out double fall)
    {
        double x = drag * t;
        if (x < .5)
        {
            double travelTerm = t, fallTerm = t * t / 2;
            travel = travelTerm; fall = fallTerm;
            for (int n = 1; n <= 30; n++)
            {
                travelTerm *= -x / (n + 1);
                fallTerm *= -x / (n + 2);
                travel += travelTerm; fall += fallTerm;
            }
        }
        else
        {
            travel = (1 - System.Math.Exp(-x)) / drag;
            fall = (t - travel) / drag;
        }
    }

    static void ReferenceFlight(PhysicalParticleSimulation.State state, double age,
        out double[] position, out double[] velocity)
    {
        double launchTime = System.Math.Min(age, state.physics.launchDuration);
        ReferenceIntegrals(state.physics.launchDrag, launchTime, out double travel, out double fall);
        double decay = System.Math.Exp(-state.physics.launchDrag * launchTime);
        position = new[] { state.origin.x + state.launch.x * travel,
            state.origin.y + state.launch.y * travel - state.physics.gravity * fall,
            state.origin.z + state.launch.z * travel };
        velocity = new[] { state.launch.x * decay,
            state.launch.y * decay - state.physics.gravity * travel, state.launch.z * decay };
        if (age <= state.physics.launchDuration) return;
        double coastTime = age - state.physics.launchDuration;
        ReferenceIntegrals(state.physics.flightDrag, coastTime, out travel, out _);
        decay = System.Math.Exp(-state.physics.flightDrag * coastTime);
        position[0] += velocity[0] * travel;
        position[1] += velocity[1] * coastTime - .5 * state.physics.gravity * coastTime * coastTime;
        position[2] += velocity[2] * travel;
        velocity[0] *= decay; velocity[1] -= state.physics.gravity * coastTime; velocity[2] *= decay;
    }

    static void AssertVector(Vector3 actual, double[] expected, string context)
    {
        for (int axis = 0; axis < 3; axis++)
            Assert.That(actual[axis], Is.EqualTo(expected[axis]).Within(.00001 + .000001 * System.Math.Abs(expected[axis])),
                $"{context}, axis={axis}");
    }

    [Test, Category("Quick")]
    public void ProfilesDriveIndependentPhysicsAndSnapshotSettingsAtEmission()
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            var empty = new PhysicalParticleSimulation(1);
            empty.Emit(profile, Vector3.zero, 1, -1, 0, 0);
            Assert.That(empty.Count, Is.Zero, "a newly created Inspector profile can be authored before emitting any groups");
            profile.groups = new[] { new PhysicalParticleProfile.Group {
                count = 1, angles = Vector2.zero, angleJitter = 0, speed = Vector2.one * 2,
                lifetime = Vector2.one * 3,
                physics = new PhysicalParticleProfile.PhysicsSettings { gravity = 0, flightDrag = 0 }
            } };
            var simulation = new PhysicalParticleSimulation(1);
            simulation.Emit(profile, Vector3.up, 1, -1, 0, 9);
            profile.groups[0].physics.gravity = 20;
            var first = simulation.Evaluate(0, .5f);
            Assert.That(first.position, Is.EqualTo(new Vector3(1, 1, 0)), "zero drag uses ballistic limit without division by zero");
            Assert.That(first.velocity, Is.EqualTo(Vector3.right * 2));
            simulation.Emit(profile, Vector3.up, 1, -1, 0, 9);
            Assert.That(simulation.Evaluate(0, .5f).position.y, Is.LessThan(first.position.y));
            Assert.That(simulation.Evaluate(0, 3).alive, Is.False);
            simulation.Clear(); Assert.That(simulation.Count, Is.Zero);
        }
        finally { Object.DestroyImmediate(profile); }
    }

    [Test, Category("Quick")]
    public void FloorLaunchHandlesUpwardBirthBelowFloorAndSkippedBounce()
    {
        var profile = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            profile.groups = new[] { new PhysicalParticleProfile.Group {
                count = 1, angles = Vector2.one * 90, angleJitter = 0, speed = Vector2.one * 2,
                lifetime = Vector2.one * 5,
                physics = new PhysicalParticleProfile.PhysicsSettings {
                    gravity = 10, collideWithFloor = true, restitution = .25f, tangentRetention = .5f, restDuration = .2f
                }
            } };
            var simulation = new PhysicalParticleSimulation(1);
            simulation.Emit(profile, Vector3.down, 1, -1, 0, 0);
            Assert.That(simulation.Evaluate(0, 0).position.y, Is.Zero);
            Assert.That(simulation.Evaluate(0, .1f).position.y, Is.GreaterThan(0));
            var state = simulation[0];
            Assert.That(state.flight, Is.EqualTo(.4f).Within(.00001f));
            Assert.That(state.bounce, Is.EqualTo(.1f).Within(.00001f));
            Assert.That(simulation.Evaluate(0, .45f).position.y, Is.GreaterThan(0));
            var rest = simulation.Evaluate(0, .6f);
            Assert.That(rest.position.y, Is.Zero); Assert.That(rest.velocity, Is.EqualTo(Vector3.zero));
            Assert.That(simulation.Evaluate(0, 1).alive, Is.False, "large steps expire instead of repeating bounces");
        }
        finally { Object.DestroyImmediate(profile); }
    }

#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void BuilderRetainsExistingProfileAndInspectorTuningAcrossRepeatedRuns()
    {
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/ParryClash/ParryClash.prefab");
        var instance = Object.Instantiate(prefab);
        var custom = ScriptableObject.CreateInstance<PhysicalParticleProfile>();
        try
        {
            custom.groups = new[] { new PhysicalParticleProfile.Group { count = 3, speed = Vector2.one * 7 } };
            var serialized = new UnityEditor.SerializedObject(instance.GetComponent<ParryClashBurst>());
            serialized.FindProperty("physicalProfile").objectReferenceValue = custom;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var builder = assembly.GetType("ParryClashAssets");
                if (builder == null) continue;
                var bind = builder.GetMethod("BindPhysics", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                for (int repeat = 0; repeat < 2; repeat++)
                {
                    Assert.That((bool)bind.Invoke(null, new object[] { serialized }), Is.False);
                    Assert.That(serialized.FindProperty("physicalProfile").objectReferenceValue, Is.SameAs(custom));
                    Assert.That(custom.groups[0].speed, Is.EqualTo(Vector2.one * 7));
                }
                serialized.FindProperty("physicalProfile").objectReferenceValue = null;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That((bool)bind.Invoke(null, new object[] { serialized }), Is.True);
                Assert.That(serialized.FindProperty("physicalProfile").objectReferenceValue.name, Is.EqualTo("ParryPhysics"));
                return;
            }
            Assert.Fail("ParryClashAssets builder not found");
        }
        finally { Object.DestroyImmediate(instance); Object.DestroyImmediate(custom); }
    }
#endif
}
