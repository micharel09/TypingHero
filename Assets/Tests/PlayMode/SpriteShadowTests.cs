using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

public sealed class SpriteShadowTests
{
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        var session = DungeonSession.Current;
        if (session && session.World) yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(session.World.gameObject.scene);
        if (session) Object.Destroy(session.gameObject);
        GamePauseController.ResetSessionState();
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator AuthoredWorldUsesOneShadowKeyAndTwoFillLights()
    {
        var session = DungeonSession.I;
        yield return session.LoadWorld(LightingTestAssets.Definition);
        int casters = 0, fills = 0;
        foreach (var fixture in session.World.environment.GetComponentsInChildren<DungeonTorchLight>())
        {
            var light = fixture.GetComponent<Light>();
            Assert.That(light.shadows, Is.EqualTo(fixture.profile.shadows));
            if (light.shadows == LightShadows.None) fills++; else casters++;
            Assert.That(light.intensity, Is.InRange(fixture.profile.intensity * (1-fixture.profile.amplitude),
                fixture.profile.intensity * (1+fixture.profile.amplitude)));
        }
        Assert.That(casters, Is.EqualTo(1)); Assert.That(fills, Is.EqualTo(2));
    }

    [Test, Category("Quick")]
    public void ContactLightsCastSpatialShadowsWithoutChangingTheWorldKey()
    {
        var host = new GameObject("spatial pulse test"); host.SetActive(false);
        var hit = new GameObject("hit", typeof(Light)); hit.transform.SetParent(host.transform);
        var parry = new GameObject("parry", typeof(Light)); parry.transform.SetParent(host.transform);
        var owner = new GameObject("contact owner");
        var rig = host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        typeof(ClashLightingRig).GetField("combatLight", flags).SetValue(rig, hit.GetComponent<Light>());
        typeof(ClashLightingRig).GetField("parryLight", flags).SetValue(rig, parry.GetComponent<Light>());
        string before = JsonUtility.ToJson(rig.profile);
        try
        {
            host.SetActive(true);
            var clock = new ClashLightingRig.PulseClock(); clock.Reset(100, false);
            typeof(ClashLightingRig).GetField("clock", flags).SetValue(rig, clock);
            var light = hit.GetComponent<Light>(); var local = parry.GetComponent<Light>();
            Assert.That(light.shadows, Is.EqualTo(LightShadows.Hard));
            Assert.That(local.shadows, Is.EqualTo(LightShadows.Hard));
            var point = new Vector3(1, 2, 0);
            rig.PulseAt(point, CombatLightingProfile.Event.Impact, owner.transform, 100, false);
            Assert.That(light.transform.position, Is.EqualTo(point + rig.profile.combatOffset));
            Assert.That(light.intensity, Is.GreaterThan(0)); Assert.That(local.intensity, Is.Zero);
            rig.CancelAt(owner.transform, 100.01, false);
            Assert.That(light.intensity, Is.Zero);
            var burst = owner.AddComponent<ImpactVfxBurst>();
            var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<ImpactVfxProfile>("Assets/Art/VFX/ParryClash/MetalClash.asset").Capture();
            rig.PulseAt(point, settings, burst, 101, false);
            Assert.That(local.intensity, Is.EqualTo(settings.lightIntensity)); Assert.That(light.intensity, Is.Zero);
            Vector3 emittedAt = local.transform.position;
            rig.SampleAt(101.03, true);
            Assert.That(local.intensity, Is.EqualTo(settings.lightIntensity));
            Assert.That(local.transform.position, Is.EqualTo(emittedAt));
            rig.SampleAt(102, true); rig.SampleAt(102, false);
            rig.SampleAt(102 + settings.lightDuration + .01, false);
            Assert.That(local.intensity, Is.Zero); Assert.That(light.intensity, Is.Zero);
            for (int i = 0; i < 8; i++)
            {
                rig.PulseAt(point, CombatLightingProfile.Event.Impact, owner.transform, 200+i, false);
                rig.SampleAt(200+i+.9, false);
            }
            long allocated = System.GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 32; i++)
            {
                rig.PulseAt(point, CombatLightingProfile.Event.Impact, owner.transform, 300+i, false);
                rig.SampleAt(300+i+.9, false);
            }
            allocated = System.GC.GetAllocatedBytesForCurrentThread() - allocated;
            Assert.That(allocated, Is.Zero, "Warmed contact-light scheduling/update allocates no managed objects");
            Assert.That(JsonUtility.ToJson(rig.profile), Is.EqualTo(before));
        }
        finally { Object.DestroyImmediate(host); Object.DestroyImmediate(owner); }
    }
}
