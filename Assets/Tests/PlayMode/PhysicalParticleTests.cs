using NUnit.Framework;
using UnityEngine;

public sealed class PhysicalParticleTests
{
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
